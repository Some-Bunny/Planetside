using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;

using UnityEngine.Serialization;
using static ETGMod;

namespace Planetside
{
	internal class RevenantProjectile : MonoBehaviour
	{
		public RevenantProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile)
            {
                projectile.BlackPhantomDamageMultiplier *= 3;
                projectile.ignoreDamageCaps = true;
                float num = 1;
                GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
                if (lastLoadedLevelDefinition != null)
                {
                    num = lastLoadedLevelDefinition.enemyHealthMultiplier - 1;
                }
                projectile.baseData.damage = projectile.baseData.damage * (num + 1);

                try
                {
                    var player = this.projectile.Owner as PlayerController;
                    if (player != null)
                    {
                        if (player.PlayerHasActiveSynergy("Ashes To Ashes, To Ashes"))
                        {
                            if (player.IsOnFire)
                            {
                                projectile.baseData.damage *= 1.33f;
                                projectile.OnHitEnemy += HandleHit;
                            }
                        }
                    }
                    projectile.IgnoreTileCollisionsFor(0.25f);
                    projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));
                }
                catch (Exception ex)
                {
                    ETGModConsole.Log(ex.Message, false);
                }
            }
        }
        private void HandleHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor != null && !arg2.healthHaver.IsBoss && arg3 == true)
            {
                GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemyDeath(arg2.aiActor));

            }
        }

        private IEnumerator HandleEnemyDeath(AIActor target)
        {

            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.FireDef).TimedAddGoopCircle(target.sprite.WorldCenter, 3, 0.33f, false);

            target.EraseFromExistenceWithRewards(false);
            Transform copyTransform = this.CreateEmptySprite(target);
            tk2dSprite copySprite = copyTransform.GetComponentInChildren<tk2dSprite>();
            
            float elapsed = 0f;
            float duration = 1f;
            copySprite.renderer.material.DisableKeyword("TINTING_OFF");
            copySprite.renderer.material.EnableKeyword("TINTING_ON");
            copySprite.renderer.material.DisableKeyword("EMISSIVE_OFF");
            copySprite.renderer.material.EnableKeyword("EMISSIVE_ON");
            copySprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
            copySprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
            copySprite.renderer.material.SetFloat("_EmissiveThresholdSensitivity", 5f);
            copySprite.renderer.material.SetFloat("_EmissiveColorPower", 1f);
            int emId = Shader.PropertyToID("_EmissivePower");
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                float t = elapsed / duration;
                copySprite.renderer.material.SetFloat(emId, Mathf.Lerp(1f, 10f, t));
                copySprite.renderer.material.SetFloat("_BurnAmount", t);
                copyTransform.position += Vector3.up * BraveTime.DeltaTime * 1f;
                yield return null;
            }
            Destroy(copyTransform.gameObject);
            yield break;
        }


        private Transform CreateEmptySprite(AIActor target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            tk2dSprite.usesOverrideMaterial = true;
            if (target.optionalPalette != null)
            {
                tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
            }
            if (tk2dSprite.renderer.material.shader.name.Contains("ColorEmissive"))
            {
            }
            return gameObject2.transform;
        }


        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody.gameObject.name != null)
            {
                if (otherRigidbody.gameObject.name == "Table_Vertical" || otherRigidbody.gameObject.name == "Table_Horizontal")
                {
                    PhysicsEngine.SkipCollision = true;
                }
            }
        }
        private Projectile projectile;
	}
}

