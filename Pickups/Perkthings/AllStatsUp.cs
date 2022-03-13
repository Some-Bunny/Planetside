using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;

namespace Planetside
{
    class AllStatsUp : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "All Stats Up";
            string resourcePath = "Planetside/Resources/PerkThings/lazyAllstatsUp.png";
            GameObject gameObject = new GameObject(name);
            AllStatsUp item = gameObject.AddComponent<AllStatsUp>();
          
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Literally just an all stats up.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            AllStatsUp.AllStatsUpID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.red;
            particles.ParticleSystemColor2 = Color.red;
            OutlineColor = new Color(0.2f, 0f, 0f);

        }
        public static int AllStatsUpID;
        private static Color OutlineColor;

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
            OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.MovementSpeed, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.ChargeAmountMultiplier, 0.85f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.Accuracy, 0.85f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.Coolness, 1f, StatModifier.ModifyMethod.ADDITIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.DamageToBosses, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.RateOfFire, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.Health, 1f, StatModifier.ModifyMethod.ADDITIVE);
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");

            OtherTools.Notify("All Stats Up", "Grants a boost to most stats!","Planetside/Resources/PerkThings/lazyAllstatsUp", UINotificationController.NotificationColor.GOLD);
            UnityEngine.Object.Destroy(base.gameObject);
        }

        public float distortionMaxRadius = 30f;
        public float distortionDuration = 2f;
        public float distortionIntensity = 0.7f;
        public float distortionThickness = 0.1f;
        protected void Start()
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

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite)
            {
                return 1000f;
            }
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return 1f;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (!interactor.CurrentRoom.IsRegistered(this) && !RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        private void Update()
        {
            if (!this.m_hasBeenPickedUp && !this.m_isBeingEyedByRat && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
            {
                GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
            }
        }

        public void Interact(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                RoomHandler.unassignedInteractableObjects.Remove(this);
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            this.Pickup(interactor);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        private bool m_hasBeenPickedUp;
    }
}
