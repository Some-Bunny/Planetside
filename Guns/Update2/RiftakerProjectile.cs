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

namespace Planetside
{
	internal class RiftakerProjectile : MonoBehaviour
	{
		public RiftakerProjectile()
		{
		}

        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();

            if (this.projectile != null)
            {
                this.player = this.projectile.Owner as PlayerController;
                this.projectile.sprite.usesOverrideMaterial = true;
                Material mat1 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat1.mainTexture = this.projectile.sprite.renderer.material.mainTexture;
                mat1.SetColor("_EmissiveColor", new Color32(215, 225, 255, 255));
                mat1.SetFloat("_EmissiveColorPower", 15.5f);
                mat1.SetFloat("_EmissivePower", 100);
                mat1.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                this.projectile.OnDestruction += this.Zzap;
            }
                       
        }
        private void Zzap(Projectile projectile)
        {
            if (projectile.Owner as PlayerController)
            {
                Exploder.DoDistortionWave(projectile.sprite.WorldCenter, 0.1f, 0.5f, 3, 0.25f);
                GameObject portalObj = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX, projectile.transform.position - new Vector3(0f, 0f, 1.5f), Quaternion.Euler(0f, 0f, 0f));
                portalObj.layer = projectile.Owner.gameObject.layer + (int)GameManager.Instance.MainCameraController.CurrentZOffset;
                portalObj.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
                MeshRenderer mesh = portalObj.GetComponent<MeshRenderer>();
                var texture = StaticTextures.NebulaTexture;
                mesh.material.SetTexture("_PortalTex", texture);
                mesh.material.SetFloat("_UVDistCutoff", 0f);
                WhyYouGottaMakeThingsSoComplicated why = portalObj.AddComponent<WhyYouGottaMakeThingsSoComplicated>();
                why.AceCourtBot = mesh;
                why.player = this.player;
            }
        }
        private Projectile projectile;
        private PlayerController player;

    }
}

namespace Planetside
{
    public class WhyYouGottaMakeThingsSoComplicated : BraveBehaviour
    {
        public WhyYouGottaMakeThingsSoComplicated()
        {
            AceCourtBot = null;
            player = GameManager.Instance.PrimaryPlayer;
        }
        public void Start()
        {
            if (AceCourtBot != null)
            {
                base.StartCoroutine(this.HoldPortalOpen(AceCourtBot));
            }
        }
        private IEnumerator HoldPortalOpen(MeshRenderer portal) 
        {
            float elapsed = 0f;
            float duration = 2;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                float t = Mathf.Sin(elapsed*0.67f);
                if (portal == null) { yield break; }
                if (portal.gameObject == null) { yield break; }
                portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(elapsed/2f, 0, t));
                portal.material.SetFloat("_HoleEdgeDepth", Mathf.Lerp(12, 2, t));
                        
                if (player != null)
                {
                    float Rad = portal.material.GetFloat("_UVDistCutoff");
                    float num = player != null ? player.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
                    List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    if (AceCourtBot != null)
                    {
                        Vector2 centerPosition = AceCourtBot.transform.position;
                        if (activeEnemies != null && activeEnemies.Count >= 0)
                        {
                            for (int i = 0; i < activeEnemies.Count; i++)
                            {
                                AIActor aiactor = activeEnemies[i];
                                bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < Rad * 16 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null;
                                if (ae)
                                {
                                    aiactor.healthHaver.ApplyDamage((27f * num) * BraveTime.DeltaTime, Vector2.zero, "fwomp", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                                    if (player.PlayerHasActiveSynergy("Event Horizon"))
                                    {
                                        if (!aiactor.healthHaver.IsBoss)
                                        {
                                            aiactor.knockbackDoer.weight = 150f;
                                            Vector2 a = aiactor.transform.position - portal.transform.position;
                                            aiactor.knockbackDoer.ApplyKnockback(-a, 1.33f * (Vector2.Distance(portal.transform.position, aiactor.transform.position) + 0.005f), false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
               
                else
                {
                    if (portal.gameObject != null) { Destroy(portal.gameObject); }
                    yield break;
                }
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_WPN_blackhole_impact_01", portal.gameObject);
            if (portal.gameObject != null){ Destroy(portal.gameObject);}
            yield break;
        }

        public MeshRenderer AceCourtBot;
        public PlayerController player;

    }
}