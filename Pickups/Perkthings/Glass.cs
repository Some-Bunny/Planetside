using Dungeonator;
using HarmonyLib;
using ItemAPI;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using tk2dRuntime.TileMap;
using UnityEngine;

namespace Planetside
{
    
    class Glass : PerkPickupObject, IPlayerInteractable
    {

        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 0,
                    LockedString = "\"Fragile, yet Fatal.\"",
                    UnlockedString = "\"Fragile, yet Fatal.\"",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 1,
                    LockedString = AlphabetController.ConvertString("Glass Cannon"),
                    UnlockedString = "Very Low Health, Much Higher Damage.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 2,
                    LockedString = AlphabetController.ConvertString("Limit Breaker"),
                    UnlockedString = "Damage Caps become very lenient.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 3,
                    LockedString = AlphabetController.ConvertString("Health Cap"),
                    UnlockedString = "Amount Of Health is capped to 4 hits maximum.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 5,
                    LockedString = AlphabetController.ConvertString("Overheal Good"),
                    UnlockedString = "Overhealing slightly increases Damage.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.GLASS_FLAG_OVERHEAL
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 6,
                    LockedString = AlphabetController.ConvertString("Stacking Adds Effectiveness"),
                    UnlockedString = "Stacking increases damage, but reduces the HP cap to 3 hits.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.GLASS_FLAG_STACK
                },
        };
        public override CustomDungeonFlags FlagToSetOnStack => CustomDungeonFlags.GLASS_FLAG_STACK;
        public static void Init()
        {
            string name = "Glass";
            //string resourcePath = "Planetside/Resources/PerkThings/glass.png";
            GameObject gameObject = new GameObject(name);
            Glass item = gameObject.AddComponent<Glass>();

            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("glass"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Fragile, Yet Fatal.";
            string longDesc = "One of the first Perks ever made, created from the melted scraps of powerful guns that dying Gungeoneers left in the Deep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            Glass.GlassID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.cyan;
            particles.ParticleSystemColor2 = Color.blue;
            item.encounterTrackable.DoNotificationOnEncounter = false;

            item.InitialPickupNotificationText = "Fragile, yet Fatal.";
            item.StackPickupNotificationText = "Even More Fatal.";


            Alexandria.DungeonAPI.DungeonHooks.OnPostDungeonGeneration += () =>
            {
                ConvertedEnemies.Clear();
            };
            item.OutlineColor = new Color(0, 0.045f, 0.9f);
        }

        public static List<AIActor> ConvertedEnemies = new List<AIActor>();

        public static int GlassID;

        
        [HarmonyPatch(typeof(HealthHaver), nameof(HealthHaver.ApplyDamageDirectional))]
        public class __
        {
            [HarmonyPrefix]
            private static bool A(HealthHaver __instance, float damage, Vector2 direction, string damageSource, CoreDamageTypes damageTypes, DamageCategory damageCategory = DamageCategory.Normal, bool ignoreInvulnerabilityFrames = false, PixelCollider hitPixelCollider = null, bool ignoreDamageCaps = false)
            {
                if (__instance && __instance.gameActor != null && __instance.gameActor is AIActor enemy)
                {
                    if (!ConvertedEnemies.Contains(enemy))
                    {
                        foreach (var entry in GameManager.Instance.AllPlayers)
                        {
                            if (entry.HasPerk(GlassID) != null)
                            {
                                __instance.m_bossDpsCap *= 3f;
                                __instance.m_damageCap *= 3f;
                                break;
                            }
                        }
                        ConvertedEnemies.Add(enemy);
                    }
                }
                return true;
            }
        }



        /*
        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            base.HandleEncounterable(player);

            SaveAPI.AdvancedGameStatsManager.Instance.RegisterStatChange(StatToIncreaseOnPickup, 1);

            m_hasBeenPickedUp = true;
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);
            GlassComponent glass = player.gameObject.GetOrAddComponent<GlassComponent>();
            glass.player = player;  
            if (glass.hasBeenPickedup == true)
            { glass.IncrementStack(); SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.GLASS_FLAG_STACK, true); }
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = glass.hasBeenPickedup == true ? "Even More Fatal." : "Fragile, yet Fatal.";
            OtherTools.NotifyCustom("Glass", BlurbText, "glass", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);
            UnityEngine.Object.Destroy(base.gameObject);
        }
        */

        public override void OnInitialPickup(PlayerController player)
        {
            LeniencyProtection = 0;
            if (player.ForceZeroHealthState == true && player.healthHaver.Armor > 4)
            {
                player.healthHaver.Armor = 4;
            }
            else
            {
                float HPtOremove = (player.stats.GetStatValue(PlayerStats.StatType.Health));
                OtherTools.ApplyStat(player, PlayerStats.StatType.Health, (-HPtOremove) + 1, StatModifier.ModifyMethod.ADDITIVE);
                if (player.healthHaver.Armor > 1) { player.healthHaver.Armor = 1; }
            }
            StatModifier item = new StatModifier
            {
                statToBoost = PlayerStats.StatType.Damage,
                amount = DamageMult,
                modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
            };
            this.DamageStat = item;
            player.ownerlessStatModifiers.Add(item);
            player.stats.RecalculateStats(player, true, true);
        }


        public override void Update()
        {
            if (_Owner)
            {
                int ArmorAllowed = _Owner.ForceZeroHealthState == true ? 4 : 2; 
                ArmorAllowed += LeniencyProtection;
                if (_Owner.healthHaver.Armor > ArmorAllowed)
                {
                    DoHurty();
                    OtherTools.NotifyCustom("A Glass Curse", "Prevented Armor Increase!", "glass", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);
                    _Owner.healthHaver.Armor = ArmorAllowed;
                }
                if ((_Owner.stats.GetStatValue(PlayerStats.StatType.Health) != 1 && _Owner.stats.GetStatValue(PlayerStats.StatType.Health) >= 1))
                {
                    DoHurty();
                    float HPtOremove = (_Owner.stats.GetStatValue(PlayerStats.StatType.Health));
                    OtherTools.ApplyStat(_Owner, PlayerStats.StatType.Health, (-HPtOremove) + 1, StatModifier.ModifyMethod.ADDITIVE);
                    OtherTools.NotifyCustom("A Glass Curse", "Prevented Health Increase!", "glass", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);
                }
            }
        }

        public void DoHurty(bool IsBigHeart = false)
        {
            if (_Owner)
            {
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.GLASS_FLAG_OVERHEAL, true);
                //if (IsBigHeart == true) { DamageMult += DamageToGetFromOverHeal*2; } else { DamageMult += DamageToGetFromOverHeal; }
                AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", _Owner.gameObject);
                GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
                tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
                component.PlaceAtPositionByAnchor(_Owner.transform.position + new Vector3(0.375f, 0.375f), tk2dBaseSprite.Anchor.MiddleCenter);
                //player.ownerlessStatModifiers.Remove(this.DamageStat);
                GiveDamage(IsBigHeart);
            }
        }
        public void GiveDamage(bool IsBig = false)
        {
            StatModifier item = new StatModifier
            {
                statToBoost = PlayerStats.StatType.Damage,
                amount = DamageToGetFromOverHeal * (IsBig ? 1.5f : 1),
                modifyType = StatModifier.ModifyMethod.ADDITIVE
            };
            TotalAddedDamage += DamageToGetFromOverHeal * (IsBig ? 1.5f : 1);
            _Owner.ownerlessStatModifiers.Add(item);
            _Owner.stats.RecalculateStats(_Owner, true, true);
        }


        public void IncrementStack()
        {
            this.DamageToGetFromOverHeal += 0.025f;
            this.DamageMult += 0.2f;
            this.DamageStat.amount = this.DamageMult;
            _Owner.stats.RecalculateStats(_Owner, true, true);
            if (LeniencyProtection != -1) { LeniencyProtection = -1; }
        }





        private StatModifier DamageStat;
        public float DamageMult = 1.75f;
        public int LeniencyProtection = 0;
        public float DamageToGetFromOverHeal = 0.05f;

        private float TotalAddedDamage = 0;
    }
}
