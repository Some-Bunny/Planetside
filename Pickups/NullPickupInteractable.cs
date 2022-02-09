using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;

namespace Planetside
{
    class NullPickupInteractable : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Null Pickup Interactable";
            string resourcePath = "Planetside/Resources/Pickups/NullPickup.png";
            GameObject gameObject = new GameObject(name);
            NullPickupInteractable item = gameObject.AddComponent<NullPickupInteractable>();
          
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Nolla.";
            string longDesc = "Nullifies you, for an inner reward.";
            item.SetupItem(shortDesc, longDesc, "psog");
            NullPickupInteractable.NollahID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;

        }
        public static int NollahID;


        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            if (!player.CurrentItem)
            {
                return;
            }
            m_hasBeenPickedUp = true;
            float keys = player.carriedConsumables.KeyBullets;
            float Money = player.carriedConsumables.Currency;
            float Blank = player.Blanks;
            float NullPoints = 0;
            NullPoints += Money / 20 + keys / 2 + Blank / 2;
            //ETGModConsole.Log("Points Gotten: " + NullPoints.ToString(), false);
            OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, NullPoints / 21, StatModifier.ModifyMethod.ADDITIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.MovementSpeed, NullPoints / 21f, StatModifier.ModifyMethod.ADDITIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.ChargeAmountMultiplier, NullPoints / 21, StatModifier.ModifyMethod.ADDITIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.KnockbackMultiplier, NullPoints / 21, StatModifier.ModifyMethod.ADDITIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.ProjectileSpeed, NullPoints / 21, StatModifier.ModifyMethod.ADDITIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.AdditionalClipCapacityMultiplier, NullPoints / 21, StatModifier.ModifyMethod.ADDITIVE);
            OtherTools.ApplyStat(player, PlayerStats.StatType.AmmoCapacityMultiplier, NullPoints / 21, StatModifier.ModifyMethod.ADDITIVE);

            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.carriedConsumables.KeyBullets = 0;
            player.carriedConsumables.Currency = 0;
            player.Blanks = 0;
            player.BloopItemAboveHead(base.sprite, "");
            AkSoundEngine.PostEvent("Play_ENM_critter_poof_01", base.gameObject);
            OtherTools.Notify("Nullified:", keys.ToString() + CheckIfMultiple(" Key", keys) + Money.ToString() + CheckIfMultiple(" Casing", Money) + Blank.ToString() + CheckIfMultiple(" Blank", Blank), "Planetside/Resources/Pickups/NullPickup");
            UnityEngine.Object.Destroy(base.gameObject);
        }

        public string CheckIfMultiple(string PickupString, float valueOfPickup)
        {
            string isMult = valueOfPickup >= 1 ? "s, " : ", ";
            return PickupString + isMult;
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
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
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
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
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
            float keys = interactor.carriedConsumables.KeyBullets;
            float Money = interactor.carriedConsumables.Currency;
            float Blank = interactor.Blanks;
            float NullPoints = Money+ keys + Blank;
            if (!this)
            {
                return;
            }
            if (NullPoints != 0)
            {
                if (RoomHandler.unassignedInteractableObjects.Contains(this))
                {
                    RoomHandler.unassignedInteractableObjects.Remove(this);
                }
                SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
                this.Pickup(interactor);
            }
            else
            {
                if (NullPoints == 0)
                {
                    PlanetsideModule.Strings.Items.Set("#NULL_TEXT", "NULL");
                    GameUIRoot.Instance.InformNeedsReload(interactor, new Vector3(interactor.specRigidbody.UnitCenter.x - interactor.transform.position.x, 1.25f, 0f), 1f, "NULL");
                }
            }
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        private bool m_hasBeenPickedUp;
    }
}
