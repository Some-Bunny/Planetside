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
using MonoMod.Utils;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;


namespace Planetside
{
    public class PortablePylon : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Portable Pylon";
            string resourceName = "Planetside/Resources/portablepylon.png";
            GameObject obj = new GameObject(itemName);
            PortablePylon activeitem = obj.AddComponent<PortablePylon>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Swing By";
            string longDesc = "A portable tesla pylon that runs a current through the air and into your body.\n\nScience genuinely cannot explain why it doesn't just instantly kill you.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 500f);
            activeitem.consumable = false;
            activeitem.numberOfUses = 2;
            activeitem.UsesNumberOfUsesBeforeCooldown = true;
            activeitem.quality = PickupObject.ItemQuality.C;
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:portable_pylon",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "prototype_railgun",
                "armor_synthesizer",
                "blast_helmet",
                "grappling_hook",
                "bionic_leg"
            };
            CustomSynergies.Add("Loader Chassis", mandatoryConsoleIDs, optionalConsoleIDs, true);
            List<string> optionalConsoleID1s = new List<string>
            {
                "gungeon_blueprint"
            };
            CustomSynergies.Add("Sentry Goin' Up!", mandatoryConsoleIDs, optionalConsoleID1s, true);
            SynergyAPI.SynergyBuilder.AddItemToSynergy(activeitem, CustomSynergyType.BATTERY_POWERED);

            activeitem.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);

            PortablePylon.PortablePylonID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);


            GameObject pylon = ItemBuilder.AddSpriteToObject("pylon", "Planetside/Resources/LoaderPylon/Idle/laoderpylon_idle_001", null);
            FakePrefab.MarkAsFakePrefab(pylon);
            UnityEngine.Object.DontDestroyOnLoad(pylon);
            tk2dSpriteAnimator animator = pylon.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = pylon.AddComponent<tk2dSpriteAnimation>();



            tk2dSpriteCollectionData pylonCollection = SpriteBuilder.ConstructCollection(pylon, ("pylon_Collection"));
            UnityEngine.Object.DontDestroyOnLoad(pylonCollection);
            for (int i = 0; i < spritePaths.Length; i++)
            {
                SpriteBuilder.AddSpriteToCollection(spritePaths[i], pylonCollection);
            }
            var defIdle = SpriteBuilder.AddAnimation(animator, pylonCollection, new List<int>() {0,1,2,3 }, "default_idle", tk2dSpriteAnimationClip.WrapMode.Loop);
            var synIdle = SpriteBuilder.AddAnimation(animator, pylonCollection, new List<int>() { 4, 5, 6, 7 }, "synergy_idle", tk2dSpriteAnimationClip.WrapMode.Loop);

            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { defIdle, synIdle };
            animator.DefaultClipId = animator.GetClipIdByName("default_idle");
            animator.playAutomatically = true;

            pylon.AddComponent<PylonController>();

            pylonObject = pylon;

        }

        public static GameObject pylonObject;

        private static string[] spritePaths = new string[]
        {
            "Planetside/Resources/LoaderPylon/Idle/laoderpylon_idle_001.png",
            "Planetside/Resources/LoaderPylon/Idle/laoderpylon_idle_002.png",
            "Planetside/Resources/LoaderPylon/Idle/laoderpylon_idle_003.png",
            "Planetside/Resources/LoaderPylon/Idle/laoderpylon_idle_004.png",

            "Planetside/Resources/LoaderPylon/SynergyForme/laoderpylonsynergy_idle_001.png",
            "Planetside/Resources/LoaderPylon/SynergyForme/laoderpylonsynergy_idle_002.png",
            "Planetside/Resources/LoaderPylon/SynergyForme/laoderpylonsynergy_idle_003.png",
            "Planetside/Resources/LoaderPylon/SynergyForme/laoderpylonsynergy_idle_004.png"
        };

        public static int PortablePylonID;


        public override void Update()
        {
            base.Update();
            if (base.LastOwner)
            {
                base.m_cachedNumberOfUses = LastOwner.PlayerHasActiveSynergy("Sentry Goin' Up!") == true ? 3 : 2;
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }


        public static List<GameObject> allPylons = new List<GameObject>();

        public override bool CanBeUsed(PlayerController user)
        {
            return user.IsInCombat;
        }
        protected override void DoEffect(PlayerController user)
        {
            PylonController gameObject = UnityEngine.Object.Instantiate<GameObject>(pylonObject, user.specRigidbody.UnitCenter, Quaternion.identity).GetComponent<PylonController>();
            if (user.PlayerHasActiveSynergy("Loader Chassis")) 
            {
                gameObject.DPS = 4;
                gameObject.IsSynergy = true;
            }
            if (user.PlayerHasActiveSynergy("Sentry Goin' Up!"))
            {
                gameObject.DPS += 0.5f;
            }
        }
    }
    public class PylonController : MonoBehaviour
    {

        public PlayerController Owner;
        public bool IsSynergy= false;
        private tk2dSpriteAnimator animator;
        private RoomHandler roomHandler;
        public float DPS = 2.5f;
        private HeatIndicatorController m_radialIndicator;

        public float PlayerRange = 7.5f;
        public float PylonRange = 25;


        public void Start()
        {
            PortablePylon.allPylons.Add(gameObject);
            animator = this.GetComponent<tk2dSpriteAnimator>();
            AkSoundEngine.PostEvent("Play_OBJ_turret_set_01", this.gameObject);
            LootEngine.DoDefaultSynergyPoof(gameObject.transform.PositionVector2());
            roomHandler = this.gameObject.transform.PositionVector2().GetAbsoluteRoom();
            if (roomHandler != null)
            {
                roomHandler.OnEnemiesCleared += OnCleared;
            }
            if (IsSynergy == true) 
            {
                animator.Play("synergy_idle");
                this.m_radialIndicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), animator.sprite.WorldBottomCenter, Quaternion.identity, animator.transform)).GetComponent<HeatIndicatorController>();
                this.m_radialIndicator.CurrentColor = Color.cyan.WithAlpha(5f);
                this.m_radialIndicator.IsFire = false;
                this.m_radialIndicator.CurrentRadius = 4.5f;
                this.m_radialIndicator.transform.parent = this.gameObject.transform;
                PlayerRange = 15;

            }
            this.StartCoroutine(HandleTimedDestroy());
        }
        private Dictionary<GameObject, GameObject> ExtantTethers = new Dictionary<GameObject, GameObject>();

        public void OnCleared() 
        {
            DoDestroy();
        }

        public void ProcessGameObject(GameObject ai, float Distance = 35) 
        {
            bool flag8 = ai && ai != null && Vector2.Distance(ai.transform.PositionVector2(), this.animator.sprite.WorldTopCenter) < Distance && ai != this.gameObject;
            if (flag8)
            {
                if (!ExtantTethers.ContainsKey(ai))
                {
                    GameObject obj = SpawnManager.SpawnVFX(StatiBlast.LinkVFXPrefab, false).GetComponent<tk2dTiledSprite>().gameObject;
                    ExtantTethers.Add(ai, obj);
                }
            }
            if (ai && ai != null && Vector2.Distance(ai.transform.PositionVector2(), this.animator.sprite.WorldTopCenter) > Distance)
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

        public void Update()
        {
            if (this.gameObject != null)
            {
                List<GameObject> activeObjects = PortablePylon.allPylons;
                if (activeObjects != null && activeObjects.Count > 0)
                {
                    foreach (GameObject ai in activeObjects)
                    {
                        ProcessGameObject(ai, PylonRange);
                    }
                }
                foreach (PlayerController p in GameManager.Instance.AllPlayers) 
                {
                    ProcessGameObject(p.gameObject, PlayerRange);
                }

            }
            foreach (var si in ExtantTethers)
            {
                if (this.gameObject && si.Value != null && si.Key != null)
                {
                    UpdateLink(this.gameObject, si.Value.GetComponent<tk2dTiledSprite>(), si.Key);
                }
                if (si.Key != null && si.Value != null && this.gameObject == null)
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

          

          


            if (IsSynergy == true) 
            {
                List<AIActor> activeEnemies = roomHandler.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                Vector2 centerPosition = animator.sprite.WorldBottomCenter;
                if (activeEnemies != null)
                {
                    foreach (AIActor aiactor in activeEnemies)
                    {
                        if (Vector2.Distance(aiactor.CenterPosition, centerPosition) < 4.5f && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null)
                        {
                            aiactor.healthHaver.ApplyDamage(3 * BraveTime.DeltaTime, Vector2.zero, "AoE Shock", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                        }
                    }
                }
            } 
        }
        private void UpdateLink(GameObject target, tk2dTiledSprite m_extantLink, GameObject actor)
        {
            Vector2 unitCenter = actor.GetComponent<PlayerController>() != null ? actor.GetComponent<PlayerController>().sprite.WorldCenter : actor.GetComponent<tk2dBaseSprite>().sprite.WorldTopCenter;
            Vector2 unitCenter2 = target.GetComponent<tk2dBaseSprite>().sprite.WorldTopCenter;
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
            float num = DPS;
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

        private IEnumerator HandleTimedDestroy()
        {
            float d = 60; float e = 0;
            while (e < d)
            {
                if (this.gameObject == null) { yield break; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            DoDestroy();
            yield break;
        }

        private void OnDestroy()
        {
            if (PortablePylon.allPylons.Contains(gameObject)){
                PortablePylon.allPylons.Remove(gameObject);
            }
            foreach (var si in ExtantTethers)
            {
                if (si.Value != null)
                {
                    SpawnManager.Despawn(si.Value.gameObject);
                }
            }
            ExtantTethers.Clear();
        }

        private void DoDestroy()
        {
            if (this.gameObject)
            {
                PortablePylon.allPylons.Remove(gameObject);
                AkSoundEngine.PostEvent("Play_OBJ_turret_fade_01", this.gameObject);
                LootEngine.DoDefaultSynergyPoof(gameObject.transform.PositionVector2());
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }
    }

}



