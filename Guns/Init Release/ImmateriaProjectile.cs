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
using HutongGames.PlayMaker.Actions;
using static tk2dSpriteCollectionDefinition;

namespace Planetside
{
	internal class ImmateriaProjectile : MonoBehaviour
	{
        public string SFX;
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            var wraps = this.projectile.GetOrAddComponent<WraparoundProjectile>();
            PlayerController player = this.projectile.Owner as PlayerController;
            wraps.OnWrappedAround = (proj, pos1, pos2) =>
            {
                //GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect, pos1, Quaternion.identity);
                //GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect, pos2, Quaternion.identity);
                //Destroy(gameObject1, 2);
                //Destroy(gameObject2, 2);
                Exploder.Explode(pos2, Immateria.ExplosionData, Vector2.zero, null, true);
                Exploder.Explode(pos1, Immateria.ExplosionData, Vector2.zero, null, true);
                AkSoundEngine.PostEvent("Play_WPN_" + SFX + "_impact_01", projectile.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_" + WrapaRounds.SFX + "_impact_01", projectile.gameObject);
                if (player)
                {
                    if (player.PlayerHasActiveSynergy("Continuum"))
                    {
                        int Dist = (int)Vector2.Distance(pos1, pos2);
                        Dist = Mathf.Min(Dist, 4);
                        for (int i = 0; i < Dist; i++)
                        {
                            float t = (float)i / (float)Dist;
                            Vector3 vector3 = Vector3.Lerp(pos1, pos2, t);
                            GameObject portalObj = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX, vector3, Quaternion.Euler(0f, 0f, 0f));                            
                            portalObj.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
                            portalObj.gameObject.transform.position = vector3;
                            MeshRenderer mesh = portalObj.GetComponent<MeshRenderer>();
                            mesh.material.SetTexture("_PortalTex", StaticTextures.NebulaTexture);
                            mesh.material.SetFloat("_UVDistCutoff", 0f);
                            GameManager.Instance.StartCoroutine(HoldPortalOpen(mesh));
                        }
                    }
                }
            };
        }
        private IEnumerator HoldPortalOpen(MeshRenderer portal)
        {
            float elapsed = 0f;
            float duration = 2;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                float t = Mathf.Sin(elapsed * 0.67f);
                if (portal == null) { yield break; }
                if (portal.gameObject == null) { yield break; }

                List<AIActor> activeEnemies = portal.transform.position.GetAbsoluteRoom().GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (activeEnemies != null && activeEnemies.Count >= 0)
                {
                    for (int i = 0; i < activeEnemies.Count; i++)
                    {
                        AIActor aiactor = activeEnemies[i];
                        bool ae = Vector2.Distance(aiactor.CenterPosition, portal.transform.position) < portal.material.GetFloat("_UVDistCutoff") * 16 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null;
                        if (ae)
                        {
                            aiactor.healthHaver.ApplyDamage(3 * BraveTime.DeltaTime, Vector2.zero, "fwomp", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);           
                        }
                    }
                }

                portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(elapsed / 5f, 0, t));
                portal.material.SetFloat("_HoleEdgeDepth", Mathf.Lerp(12, 2, t));
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_WPN_blackhole_impact_01", portal.gameObject);
            if (portal.gameObject != null) { Destroy(portal.gameObject); }

            yield break;
        }


        private Projectile projectile;
	}
}

