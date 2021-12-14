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
            Projectile projectile = this.projectile;
            PlayerController playerController = projectile.Owner as PlayerController;
            Projectile component = base.gameObject.GetComponent<Projectile>();
            bool flag = component != null;
            bool flag2 = flag;
            if (flag2)
            {
                component.OnDestruction += this.Zzap;

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
                    bool flag5 = activeEnemies != null && activeEnemies.Count >= 0;
                    if (flag5)
                    {
                        foreach (AIActor ai in activeEnemies)
                        {
                            bool flag8 = ai && ai != null && Vector2.Distance(ai.CenterPosition, this.projectile.sprite.WorldCenter) < 3.5f * Mult;
                            if (flag8)
                            {
                                if (!ExtantTethers.ContainsKey(ai))
                                {
                                    GameObject obj = SpawnManager.SpawnVFX(StatiBlast.LinkVFXPrefab, false).GetComponent<tk2dTiledSprite>().gameObject;
                                    ExtantTethers.Add(ai, obj);
                                }
                            }
                            bool fuckoff = ai && ai != null && Vector2.Distance(ai.CenterPosition, this.projectile.sprite.WorldCenter) > 3.5f * Mult;
                            if (fuckoff)
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
            float num = 2f;
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
			PlayerController player = projectile.Owner as PlayerController;
			bool isInCombat = player.IsInCombat;
			if (isInCombat)
			{
				float num2 = 10f;
				List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				bool flag5 = activeEnemies == null | activeEnemies.Count <= 0;
				{
					AIActor nearestEnemy = this.GetNearestEnemy(activeEnemies, projectile.sprite.WorldCenter, out num2, null);
					bool flag8 = nearestEnemy && nearestEnemy != null;
					if (flag8)
					{
						float dmg = (player.stats.GetStatValue(PlayerStats.StatType.Damage));
						Vector2 worldCenter3 = projectile.sprite.WorldCenter;
						Vector2 unitCenter3 = nearestEnemy.specRigidbody.HitboxPixelCollider.UnitCenter;
						float z3 = BraveMathCollege.Atan2Degrees((unitCenter3 - worldCenter3).normalized);
						Projectile projectile3 = ((Gun)ETGMod.Databases.Items[153]).DefaultModule.projectiles[0];
						GameObject gameObject3 = SpawnManager.SpawnProjectile(projectile3.gameObject, projectile.sprite.WorldCenter, Quaternion.Euler(0f, 0f, z3), true);
						Projectile component3 = gameObject3.GetComponent<Projectile>();
						bool flag15 = component3 != null;
						bool flag16 = flag15;
						if (flag16)
						{
							component3.Owner = projectile.PossibleSourceGun.CurrentOwner as PlayerController;
							component3.baseData.range *= 5f;
							component3.pierceMinorBreakables = true;
							component3.collidesWithPlayer = false;
							PierceProjModifier spook = component3.gameObject.AddComponent<PierceProjModifier>();
							spook.penetration = 10;
							component3.AdditionalScaleMultiplier = 0.5f;
							component3.baseData.damage = 3f * dmg;
						}
					}
				}
			}
		}
		public AIActor GetNearestEnemy(List<AIActor> activeEnemies, Vector2 position, out float nearestDistance, string[] filter)
		{
			AIActor aiactor = null;
			nearestDistance = float.MaxValue;
			bool flag = activeEnemies == null;
			bool flag2 = flag;
			bool flag3 = flag2;
			bool flag4 = flag3;
			bool flag5 = flag4;
			AIActor result;
			if (flag5)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < activeEnemies.Count; i++)
				{
					AIActor aiactor2 = activeEnemies[i];
					bool flag6 = !aiactor2.healthHaver.IsDead && aiactor2.healthHaver.IsVulnerable;
					bool flag7 = flag6;
					if (flag7)
					{
						bool flag8 = filter == null || !filter.Contains(aiactor2.EnemyGuid);
						bool flag9 = flag8;
						if (flag9)
						{
							float num = Vector2.Distance(position, aiactor2.CenterPosition);
							bool flag10 = num < nearestDistance;
							bool flag11 = flag10;
							if (flag11)
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

