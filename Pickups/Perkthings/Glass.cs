using Dungeonator;
using ItemAPI;
using SaveAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Planetside
{
    class GlassComponent : MonoBehaviour
    {
        public GlassComponent()
        {
            this.DamageMult = 2.25f;
            this.DamageToGetFromOverHeal = 0.25f;
            this.hasBeenPickedup = false;
            this.LeniencyProtection = 0;
        }
        public void Start() 
        {
            GiveDamage();
            this.hasBeenPickedup = true; 
            if (player.ForceZeroHealthState == true){
                player.healthHaver.Armor = 2;
            }
            else{
                float HPtOremove = (player.stats.GetStatValue(PlayerStats.StatType.Health));
                OtherTools.ApplyStat(player, PlayerStats.StatType.Health, (-HPtOremove) + 1, StatModifier.ModifyMethod.ADDITIVE);
                if (player.healthHaver.Armor > 1) { player.healthHaver.Armor = 1; }
            }
        }
        public void Update()
        {
            int ArmorAllowed = player.ForceZeroHealthState == true ? 4 : 2;
            ArmorAllowed -= LeniencyProtection;
            if (player.healthHaver.Armor > ArmorAllowed){
                DoHurty();
                OtherTools.NotifyCustom("A Glass Curse", "Prevented Armor Increase!", "glass", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);
                player.healthHaver.Armor = ArmorAllowed-1; 
            }
            if ((player.stats.GetStatValue(PlayerStats.StatType.Health) != 1 && player.stats.GetStatValue(PlayerStats.StatType.Health) >= 1)){
                DoHurty();
                float HPtOremove = (player.stats.GetStatValue(PlayerStats.StatType.Health));
                OtherTools.ApplyStat(player, PlayerStats.StatType.Health, (-HPtOremove) + 1, StatModifier.ModifyMethod.ADDITIVE);
                OtherTools.NotifyCustom("A Glass Curse", "Prevented Health Increase!", "glass", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);
            }
        }

        public void DoHurty(bool IsBigHeart = false)
        {
            if (player){
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.GLASS_FLAG_OVERHEAL, true);
                //if (IsBigHeart == true) { DamageMult += DamageToGetFromOverHeal*2; } else { DamageMult += DamageToGetFromOverHeal; }
                AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", player.gameObject);
                GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
                tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
                component.PlaceAtPositionByAnchor(player.transform.position + new Vector3(0.375f, 0.375f), tk2dBaseSprite.Anchor.MiddleCenter);
                //player.ownerlessStatModifiers.Remove(this.DamageStat);
                GiveDamage();
            }
        }
        public void GiveDamage()
        {
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

        
        public void IncrementStack()
        {
            this.DamageToGetFromOverHeal += 0.125f;
            this.DamageMult += 0.5f;
            if (LeniencyProtection != -1) {LeniencyProtection = -1;}
        }
        private StatModifier DamageStat;
        public PlayerController player;
        public bool hasBeenPickedup;
        public float DamageMult;
        public int LeniencyProtection;
        public float DamageToGetFromOverHeal;
    }
    class Glass : PerkPickupObject, IPlayerInteractable
    {

        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 1,
                    LockedString = AlphabetController.ConvertString("Glass Cannon"),
                    UnlockedString = "Very Low Health, Much Higher Damage.",
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
        public override CustomTrackedStats StatToIncreaseOnPickup => SaveAPI.CustomTrackedStats.AMOUNT_BOUGHT_GLASS;
        public static void Init()
        {
            string name = "Glass";
            //string resourcePath = "Planetside/Resources/PerkThings/glass.png";
            GameObject gameObject = new GameObject(name);
            Glass item = gameObject.AddComponent<Glass>();

            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("glass"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Literally just an all stats up.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            Glass.GlassID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.cyan;
            particles.ParticleSystemColor2 = Color.blue;

           


            item.OutlineColor = new Color(0, 0.045f, 0.9f);
        }
        public static int GlassID;


        public new bool PrerequisitesMet()
        {
            EncounterTrackable component = base.GetComponent<EncounterTrackable>();
            return component == null || component.PrerequisitesMet();
        }

        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
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

            //OtherTools.Notify("Glass", BlurbText, "Planetside/Resources/PerkThings/glass", UINotificationController.NotificationColor.GOLD);
            UnityEngine.Object.Destroy(base.gameObject);
        }

        public void Start()
        {
            try
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(this);
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            }
            catch (Exception er)
            {
                ETGModConsole.Log(er.Message, false);
            }
        }

      

        private void Update()
        {
            if (!this.m_hasBeenPickedUp && !this.m_isBeingEyedByRat && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
            {
                GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
            }
        }


        private bool m_hasBeenPickedUp;
    }
}
