using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using SaveAPI;
using UnityEngine;

namespace Planetside
{

    public class PerkDisplayContainer
    {
        public CustomDungeonFlags FlagToTrack = CustomDungeonFlags.SPECIAL_LOCK;
        public string UnlockedString = "Wow, you know things!";
        public string LockedString = AlphabetController.ConvertString("You Dont know shit yet");
        public int AmountToBuyBeforeReveal;
        public bool requiresFlag = true;
        public bool requiresStack = true;

    }

    public class PerkPickupObject : PickupObject , IPlayerInteractable
    {


        public virtual List<PerkDisplayContainer> perkDisplayContainers
        {
            get
            {
                return new List<PerkDisplayContainer>() { new PerkDisplayContainer() };
            }
        }

        public virtual CustomTrackedStats StatToIncreaseOnPickup
        {
            get
            {
                return CustomTrackedStats.DUMMY_BOUGHT;
            }
        }


        public float distortionMaxRadius = 30f;
        public float distortionDuration = 2f;
        public float distortionIntensity = 0.7f;
        public float distortionThickness = 0.1f;

        public ModifiedDefaultLabelManager extantLabel;


        public virtual string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public Color OutlineColor;

        public virtual float GetDistanceToPoint(Vector2 point)
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

        public virtual float GetOverrideMaxDistance()
        {
            return 1f;
        }

        public virtual void Interact(PlayerController interactor)
        {
            if (extantLabel != null) { Destroy(extantLabel.gameObject); }
            if (!this)
            {
                return;
            }
            if (RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                RoomHandler.unassignedInteractableObjects.Remove(this);
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SaveAPI.AdvancedGameStatsManager.Instance.RegisterStatChange(StatToIncreaseOnPickup, 1);
            this.Pickup(interactor);
        }

        public virtual void OnEnteredRange(PlayerController interactor)
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

            if (extantLabel != null) { Destroy(extantLabel); }

            string Text = StringTableManager.GetItemsString(this.encounterTrackable.journalData.PrimaryDisplayName);
            foreach (var enrty in this.perkDisplayContainers)
            {
                bool req = enrty.requiresFlag == false ? false : SaveAPI.AdvancedGameStatsManager.Instance.GetFlag(enrty.FlagToTrack);
                bool req2 = enrty.requiresStack == false ? false : SaveAPI.AdvancedGameStatsManager.Instance.GetPlayerStatValue(StatToIncreaseOnPickup) >= enrty.AmountToBuyBeforeReveal;
                if (req == true || req2 == true)
                {
                    Text += "\n- " + enrty.UnlockedString;
                }
                else
                {
                    Text += "\n- " + enrty.LockedString;
                }
            }
            extantLabel = UIToolbox.GenerateText(this.transform, new Vector2(1.25f, -0.25f), 0.5f, Text, new Color32(0, 12, 50, 200));

        }

        public virtual void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
            if (extantLabel != null) { extantLabel.Inv(); }

        }

        public override void Pickup(PlayerController player)
        {
        }
    }
}
