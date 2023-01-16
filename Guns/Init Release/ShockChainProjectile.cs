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
	internal class ShockChainProjectile : MonoBehaviour
	{
		public ShockChainProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            player = projectile.Owner as PlayerController;      
            if (this.projectile != null)
			{

			}
        }

        private Dictionary<Projectile, GameObject> ExtantTethers = new Dictionary<Projectile, GameObject>();
        private HashSet<AIActor> m_damagedEnemies = new HashSet<AIActor>();

        public void Update()
        {
            if (this.projectile != null)
            {
                PlayerController player = this.projectile.Owner as PlayerController;
                if (player != null)
                {

                    List<Projectile> activeProjectiles = StaticReferenceManager.AllProjectiles.ToList<Projectile>();
                    if (activeProjectiles != null && activeProjectiles.Count > 0)
                    {
                        foreach (Projectile ai in activeProjectiles)
                        {
                            bool flag8 = ai && ai != null && Vector2.Distance(ai.transform.PositionVector2(), this.projectile.sprite.WorldCenter) < 8f && ai.gameObject.GetComponent<ShockChainProjectile>() != null && ai != this.projectile;
                            if (flag8)
                            {
                                if (!ExtantTethers.ContainsKey(ai))
                                {
                                    GameObject obj = SpawnManager.SpawnVFX(StatiBlast.LinkVFXPrefab, false).GetComponent<tk2dTiledSprite>().gameObject;
                                    ExtantTethers.Add(ai, obj);
                                }
                            }
                            bool fuckoff = ai && ai != null && Vector2.Distance(ai.transform.PositionVector2(), this.projectile.sprite.WorldCenter) > 8f;
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

        private void UpdateLink(Projectile target, tk2dTiledSprite m_extantLink, Projectile actor)
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
            float num = getCalculateddamage();
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

        public float getCalculateddamage()
        {
            float ElectricDamage = 2.2f;
            if (player == null) { return ElectricDamage; }
            bool flagA = player.PlayerHasActiveSynergy("Single A");
            if (flagA)
            {
                ElectricDamage *= 2;
            }
            float dmg = (player.stats.GetStatValue(PlayerStats.StatType.Damage));
            return ElectricDamage * dmg;
        }

        private PlayerController player;
        private Projectile projectile;
	}
}

