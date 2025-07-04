using Dungeonator;
using ItemAPI;
using SaveAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Planetside
{
    class AllStatsUp : PerkPickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "All Stats Up";
           //string resourcePath = "Planetside/Resources/PerkThings/lazyAllstatsUp.png";
            GameObject gameObject = new GameObject(name);
            AllStatsUp item = gameObject.AddComponent<AllStatsUp>();

            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("lazyAllstatsUp"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Simplicity At Its Finest.";
            string longDesc = "Sometimes all one needs is an additional small push to get going.";
            item.SetupItem(shortDesc, longDesc, "psog");
            item.encounterTrackable.DoNotificationOnEncounter = false;

            AllStatsUp.AllStatsUpID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.red;
            particles.ParticleSystemColor2 = Color.red;
            item.OutlineColor = new Color(0.2f, 0f, 0f);


        }
        public override CustomTrackedStats StatToIncreaseOnPickup => SaveAPI.CustomTrackedStats.AMOUNT_BOUGHT_ALLSTATUP;
        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 1,
                    LockedString = AlphabetController.ConvertString("Stats Up"),
                    UnlockedString = "Grants a minor buff to most stats.",
                    requiresFlag = false
                },
        };


        public static int AllStatsUpID;

        public new bool PrerequisitesMet()
        {
            EncounterTrackable component = base.GetComponent<EncounterTrackable>();
            return component == null || component.PrerequisitesMet();
        }
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
            OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, 1.075f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.MovementSpeed, 1.05f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.ChargeAmountMultiplier, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.Accuracy, 0.9f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.Coolness, 1f, StatModifier.ModifyMethod.ADDITIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.DamageToBosses, 1.025f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.RateOfFire, 1.05f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.KnockbackMultiplier, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            var stack = player.GetOrAddComponent<AllStatsTrackable>();
            stack.Stacks++;
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");

            OtherTools.NotifyCustom("All Stats Up", "Grants a boost to most stats!", "lazyAllstatsUp", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);

            //OtherTools.Notify("All Stats Up", "Grants a boost to most stats!","Planetside/Resources/PerkThings/lazyAllstatsUp", UINotificationController.NotificationColor.GOLD);
            UnityEngine.Object.Destroy(base.gameObject);
        }

        public class AllStatsTrackable : MonoBehaviour { public int Stacks = 0; }

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
