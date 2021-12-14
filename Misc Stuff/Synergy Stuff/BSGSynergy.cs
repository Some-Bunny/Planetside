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
    public class BSGSynergy : MonoBehaviour
    {
		public void Start()
		{
            this.m_gun = base.GetComponent<Gun>();
            if (m_gun != null)
            {
                foreach (ProjectileModule mod in m_gun.Volley.projectiles)
                {
                    List<ProjectileModule.ChargeProjectile> list = mod.chargeProjectiles;
                    foreach (ProjectileModule.ChargeProjectile proj in list)
                    {
                        Projectile ectile = proj.Projectile; //.gameObject.AddComponent<BSGSynergyPorj>();
                        if (ectile != null)
                        {
                            ectile.gameObject.AddComponent<BSGSynergyPorj>();
                        }
                    }
                }
            }
		}
		public string SynergyNameToCheck;
		private Gun m_gun;
	}

}




namespace Planetside
{
    internal class BSGSynergyPorj : MonoBehaviour
    {
        public BSGSynergyPorj()
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

            }
        }

        private Dictionary<AIActor, GameObject> ExtantTethers = new Dictionary<AIActor, GameObject>();
        public void Update()
        {
            if (this.projectile != null)
            {
                PlayerController player = this.projectile.Owner as PlayerController;
                if (player != null && player.PlayerHasActiveSynergy("Big Shocking Gun 9000"))
                {

                    List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    bool flag5 = activeEnemies != null && activeEnemies.Count >= 0;
                    if (flag5)
                    {
                        foreach (AIActor ai in activeEnemies)
                        {
                            bool flag8 = ai && ai != null && Vector2.Distance(ai.CenterPosition, this.projectile.sprite.WorldCenter) < 30f;
                            if (flag8)
                            {
                                if (!ExtantTethers.ContainsKey(ai))
                                {
                                    GameObject obj = SpawnManager.SpawnVFX(StatiBlast.LinkVFXPrefab, false).GetComponent<tk2dTiledSprite>().gameObject;
                                    tk2dTiledSprite tiledSprite = obj.GetComponentInChildren<tk2dTiledSprite>();
                                    tiledSprite.sprite.usesOverrideMaterial = true;
                                    Material material = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
                                    Material sharedMaterial = tiledSprite.sprite.renderer.sharedMaterial;
                                    material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
                                    material.SetColor("_OverrideColor", new Color(0f, 1f, 0.1f, 1f));
                                    material.SetFloat("_EmissivePower", 20);
                                    material.SetFloat("_EmissiveColorPower", 3f);
                                    tiledSprite.sprite.renderer.material = material;
                                    var lel = tiledSprite.scale;
                                    lel.y *= 2;

                                    ExtantTethers.Add(ai, obj);
                                }
                            }
                            bool fuckoff = ai && ai != null && Vector2.Distance(ai.CenterPosition, this.projectile.sprite.WorldCenter) > 30f;
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
            Vector2 unitCenter2 = target.sprite.WorldCenter;
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
            float num = 7.5f;
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

       
        private Projectile projectile;
    }
}


