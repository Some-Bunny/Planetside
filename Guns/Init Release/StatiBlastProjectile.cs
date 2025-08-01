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
	internal class StatiBlastProjectile : MonoBehaviour
	{
		public StatiBlastProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile != null)
            {
                this.projectile.OnDestruction += this.Zzap;
            }
        }

        private Dictionary<AIActor, GameObject> ExtantTethers = new Dictionary<AIActor, GameObject>();
        public void Update()
        {
            if (this.projectile != null)
            {
                PlayerController player = this.projectile.Owner as PlayerController;
                if (player != null)
                {
                    float Mult = player.PlayerHasActiveSynergy("Big Shocking Gun 9000") ? 2.5f : 1;

                    List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    if (activeEnemies != null && activeEnemies.Count > 0)
                    {
                        foreach (AIActor ai in activeEnemies)
                        {
                            if (ai && ai != null && Vector2.Distance(ai.CenterPosition, this.projectile.sprite.WorldCenter) < 3.5f * Mult)
                            {
                                if (!ExtantTethers.ContainsKey(ai))
                                {
                                    GameObject obj = SpawnManager.SpawnVFX(StatiBlast.LinkVFXPrefab, false).GetComponent<tk2dTiledSprite>().gameObject;
                                    ExtantTethers.Add(ai, obj);
                                }
                            }
                            if (ai && ai != null && Vector2.Distance(ai.CenterPosition, this.projectile.sprite.WorldCenter) > 3.5f * Mult)
                            {
                                if (ExtantTethers.ContainsKey(ai))
                                {
                                    GameObject obj;
                                    ExtantTethers.TryGetValue(ai, out obj);
                                    SpawnManager.Despawn(obj);
                                    ExtantTethers.Remove(ai);
                                }
                            }
                        }
                    }
                }
                
            }
            foreach (var si in ExtantTethers)
            {
                if (this.projectile && si.Value != null && si.Key != null)
                {
                    UpdateLink(this.projectile, si.Value.GetComponent<tk2dTiledSprite>(), si.Key);
                }
                if (si.Key != null && si.Value != null && this.projectile == null)
                {
                    SpawnManager.Despawn(si.Value.gameObject);
                    ExtantTethers.Remove(si.Key);
                    return;
                }
                if (si.Key == null && si.Value != null)
                {
                    SpawnManager.Despawn(si.Value.gameObject);
                    ExtantTethers.Remove(si.Key);
                    return;
                }
            }
        }

        public void OnDestroy()
        {
            foreach (var si in ExtantTethers)
            {
                if (si.Value != null)
                {
                    SpawnManager.Despawn(si.Value.gameObject);
                }
            }
            ExtantTethers.Clear();
        }

        private void UpdateLink(Projectile target, tk2dTiledSprite m_extantLink, AIActor actor)
        {
            Vector2 unitCenter = actor.specRigidbody.HitboxPixelCollider.UnitCenter;
            Vector2 unitCenter2 = target.specRigidbody.HitboxPixelCollider.UnitCenter;
            m_extantLink.transform.position = unitCenter;
            Vector2 vector = unitCenter2 - unitCenter;
            float num = BraveMathCollege.Atan2Degrees(vector.normalized);
            int num2 = Mathf.RoundToInt(vector.magnitude / 0.0625f);
            m_extantLink.dimensions = new Vector2((float)num2, m_extantLink.dimensions.y);
            m_extantLink.transform.rotation = Quaternion.Euler(0f, 0f, num);
            m_extantLink.UpdateZDepth();
            this.ApplyLinearDamage(unitCenter, unitCenter2);

        }
        private void ApplyLinearDamage(Vector2 p1, Vector2 p2)
        {
            float num = 7f;
            for (int i = 0; i < StaticReferenceManager.AllEnemies.Count; i++)
            {
                AIActor aiactor = StaticReferenceManager.AllEnemies[i];
                if (!this.m_damagedEnemies.Contains(aiactor))
                {
                    if (aiactor && aiactor.HasBeenEngaged && aiactor.IsNormalEnemy && aiactor.specRigidbody)
                    {
                        Vector2 zero = Vector2.zero;
                        if (BraveUtility.LineIntersectsAABB(p1, p2, aiactor.specRigidbody.HitboxPixelCollider.UnitBottomLeft, aiactor.specRigidbody.HitboxPixelCollider.UnitDimensions, out zero))
                        {
                            aiactor.healthHaver.ApplyDamage(num, Vector2.zero, "Chain Lightning", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                            GameManager.Instance.StartCoroutine(this.HandleDamageCooldown(aiactor));
                        }
                    }
                }
            }
        }

        private IEnumerator HandleDamageCooldown(AIActor damagedTarget)
        {
            this.m_damagedEnemies.Add(damagedTarget);
            yield return new WaitForSeconds(0.25f);
            this.m_damagedEnemies.Remove(damagedTarget);
            yield break;
        }
        private HashSet<AIActor> m_damagedEnemies = new HashSet<AIActor>();

        private void Zzap(Projectile projectile)
		{
            if (projectile.Owner as PlayerController)
            {
                PlayerController player = projectile.Owner as PlayerController;
                if (player.IsInCombat)
                {
                    float num2 = 10f;
                    List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    if (activeEnemies != null | activeEnemies.Count > 0)
                    {
                        AIActor nearestEnemy = this.GetNearestEnemy(activeEnemies, projectile.sprite.WorldCenter, out num2, null);
                        if (nearestEnemy && nearestEnemy != null)
                        {
                            float dmg = player.stats.GetStatValue(PlayerStats.StatType.Damage);
                            Vector2 worldCenter3 = projectile.sprite.WorldCenter;
                            Vector2 unitCenter3 = nearestEnemy.specRigidbody.HitboxPixelCollider.UnitCenter;
                            float z3 = BraveMathCollege.Atan2Degrees((unitCenter3 - worldCenter3).normalized);

                            Projectile projectile3 = ((Gun)ETGMod.Databases.Items[153]).DefaultModule.projectiles[0];
                            Projectile proj = SpawnManager.SpawnProjectile(projectile3.gameObject, projectile.sprite.WorldCenter, Quaternion.Euler(0f, 0f, z3), true).GetComponent<Projectile>();
                            if (proj != null)
                            {
                                proj.Owner = projectile.PossibleSourceGun.CurrentOwner as PlayerController;
                                proj.baseData.range *= 5f;
                                proj.pierceMinorBreakables = true;
                                proj.collidesWithPlayer = false;
                                proj.AdditionalScaleMultiplier = 0.5f;
                                proj.baseData.damage = 5.25f * dmg;

                                proj.AddComponent<RecursionPreventer>();


                                PierceProjModifier spook = proj.gameObject.AddComponent<PierceProjModifier>();
                                spook.penetration = 10;
                            }
                        }
                    }
                }
            }
		}
		public AIActor GetNearestEnemy(List<AIActor> activeEnemies, Vector2 position, out float nearestDistance, string[] filter)
		{
			AIActor aiactor = null;
			nearestDistance = float.MaxValue;
			AIActor result;
			if (activeEnemies == null)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < activeEnemies.Count; i++)
				{
					AIActor aiactor2 = activeEnemies[i];
					if (!aiactor2.healthHaver.IsDead && aiactor2.healthHaver.IsVulnerable)
					{
						if (filter == null || !filter.Contains(aiactor2.EnemyGuid))
						{
							float num = Vector2.Distance(position, aiactor2.CenterPosition);
							if (num < nearestDistance)
							{
								nearestDistance = num;
								aiactor = aiactor2;
							}
						}
					}
				}
				result = aiactor;
			}
			return result;
		}
		private Projectile projectile;
	}
}

