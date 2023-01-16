using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Collections;
using SaveAPI;
using Brave.BulletScript;
using Gungeon;
using GungeonAPI;

namespace Planetside
{
    public class UnstableTeslaCoil : PassiveItem
    {
        public static void Init()
        {
            string name = "Volatile Tesla-Pack";
            //string resourcePath = "Planetside/Resources/plaetunstableteslacoil.png";
            GameObject gameObject = new GameObject(name);
            UnstableTeslaCoil item = gameObject.AddComponent<UnstableTeslaCoil>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("plaetunstableteslacoil"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Hair-Raising Experience";
            string longDesc = "A very volatile tesla-pack that's been hidden away in a chest to prevent harm. The arcs connect to nearby things and can erupt powerfully enough to confuse enemies.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.B;
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:volatile_tesla-pack",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "face_melter",
                "prototype_railgun",
                "heavy_bullets",
                "potion_of_lead_skin",
                "platinum_bullets"
            };
            CustomSynergies.Add("Heavy Metals", mandatoryConsoleIDs, optionalConsoleIDs, true);
            LinkVFXPrefab = FakePrefab.Clone(Game.Items["shock_rounds"].GetComponent<ComplexProjectileModifier>().ChainLightningVFX);
            UnstableTeslaCoil.VolatileTeslaPackID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);

        }
        public static int VolatileTeslaPackID;

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            DungeonHooks.OnPostDungeonGeneration += this.ResetFloorSpecificData;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            enemyList.Clear();
            DungeonHooks.OnPostDungeonGeneration -= this.ResetFloorSpecificData;
            DebrisObject result = base.Drop(player);
            return result;
        }

        private void ResetFloorSpecificData()
        {
            enemyList.Clear();
        }
        private static Dictionary<AIActor, GameObject> Shit = new Dictionary<AIActor, GameObject>();
        protected override void Update()
		{
			base.Update();
            foreach (var si in Shit)
            {
                if (base.Owner && si.Value != null && si.Key != null)
                {
                    UpdateLink(base.Owner, si.Value.GetComponent<tk2dTiledSprite>(), si.Key);
                }
                if (si.Key != null && si.Value != null && base.Owner == null)
                {
                    SpawnManager.Despawn(si.Value.gameObject);
                    Shit.Remove(si.Key);
                    return;
                }
                if (si.Key == null && si.Value != null)
                {
                    SpawnManager.Despawn(si.Value.gameObject);
                    Shit.Remove(si.Key);
                    return;
                }
            }      
            if (base.Owner != null)
            {
                bool isInCombat = base.Owner.IsInCombat;
                if (isInCombat)
                {
                    List<AIActor> activeEnemies = base.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    bool flag5 = activeEnemies != null && activeEnemies.Count >= 0;
                    if (flag5)
                    {
                        foreach (AIActor ai in activeEnemies)
                        {
                            if (ActorIsActive(ai) == true)
                            {
                                bool flag8 = ai && ai != null && Vector2.Distance(ai.CenterPosition, base.Owner.sprite.WorldCenter) < 4.5f;
                                if (flag8)
                                {
                                    if (!Shit.ContainsKey(ai))
                                    {
                                        GameObject obj = SpawnManager.SpawnVFX(LinkVFXPrefab, false).GetComponent<tk2dTiledSprite>().gameObject;
                                        Shit.Add(ai, obj);
                                    }
                                }
                                bool fuckoff = ai && ai != null && Vector2.Distance(ai.CenterPosition, base.Owner.sprite.WorldCenter) > 4.5f;
                                if (fuckoff)
                                {
                                    if (Shit.ContainsKey(ai))
                                    {
                                        GameObject obj;
                                        Shit.TryGetValue(ai, out obj);
                                        SpawnManager.Despawn(obj);
                                        Shit.Remove(ai);
                                    }
                                }
                            }             
                        }     
                    }
                }                
            }
        }

        public bool ActorIsActive(AIActor enemy)
        {
            if (enemy.State == AIActor.ActorState.Normal) { return true; }
            return false;
        }



        private void UpdateLink(PlayerController target, tk2dTiledSprite m_extantLink, AIActor actor)
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
            PlayerController player = (GameManager.Instance.PrimaryPlayer);
            float dmg = player != null? (player.stats.GetStatValue(PlayerStats.StatType.Damage)):1;
            float num = 4.5f* dmg;
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
                            if (num >= aiactor.healthHaver.GetCurrentHealth() && !enemyList.Contains(aiactor) || num == aiactor.healthHaver.GetCurrentHealth() && !enemyList.Contains(aiactor))
                            {
                                enemyList.Add(aiactor);
                                float StunRadius = 2.5f;
                                bool flagA = base.Owner.PlayerHasActiveSynergy("Heavy Metals");
                                if (flagA)
                                {
                                    StunRadius = 4f;
                                }
                                Vector2 bector2 = aiactor.sprite.WorldCenter;
                                float death = aiactor.healthHaver.GetCurrentHealth();
                                player.CurrentRoom.ApplyActionToNearbyEnemies(bector2, StunRadius, new Action<AIActor, float>(this.ProcessEnemy));
                                aiactor.healthHaver.ApplyDamage(death, Vector2.zero, "Chain Lightning", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                                GameManager.Instance.StartCoroutine(this.HandleDamageCooldown(aiactor));
                                Exploder.DoDistortionWave(bector2, 10f, 0.033f, StunRadius, 0.2f);

                            }
                            else
                            {
                                aiactor.healthHaver.ApplyDamage(num, Vector2.zero, "Chain Lightning", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                                GameManager.Instance.StartCoroutine(this.HandleDamageCooldown(aiactor));
                            }
                        }
                    }
                }
            }
        }

        private static List<AIActor> enemyList = new List<AIActor>();

        private void ProcessEnemy(AIActor target, float distance)
        {
            float StunTime = 3f;
            bool flagA = base.Owner.PlayerHasActiveSynergy("Heavy Metals");
            if (flagA)
            {
                StunTime = 5f;
            }
            if (target.healthHaver.IsBoss != true)
            {
                target.behaviorSpeculator.Stun(StunTime, true);
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
        private static GameObject LinkVFXPrefab;
    }
}
