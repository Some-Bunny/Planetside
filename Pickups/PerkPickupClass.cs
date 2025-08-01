using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmmonomiconAPI;
using Dungeonator;
using HarmonyLib;
using Planetside.Components;

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

    public class PerkPickupObject : PassiveItem , IPlayerInteractable
    {

        [HarmonyPatch(typeof(MinimapUIController), nameof(MinimapUIController.AddPassiveItemToDock))]
        public class Patch_MinimapUIController_AddPassiveItemToDock
        {
            [HarmonyPrefix]
            private static bool PatchOut(MinimapUIController __instance, PassiveItem item, PlayerController itemOwner)
            {
                if (item is PerkPickupObject) { return false; }

                return true;
            }
        }


        public virtual List<PerkDisplayContainer> perkDisplayContainers
        {
            get
            {
                return new List<PerkDisplayContainer>() { new PerkDisplayContainer() };
            }
        }

        public virtual CustomDungeonFlags FlagToSetOnStack
        {
            get
            {
                return CustomDungeonFlags.NONE;
            }
        }
        

        public int CurrentStack
        {
            get
            {
                return _CurrentStack;//return ConsumableStorage.PlayersWithConsumables[Own].AddConsumableAmount(perk.itemName, 1);

            }
        }

        private int _CurrentStack = 0;

        private float distortionMaxRadius = 30f;
        private float distortionDuration = 2f;
        private float distortionIntensity = 0.7f;
        private float distortionThickness = 0.1f;

        public ModifiedDefaultLabelManager extantLabel;

        public string InitialPickupNotificationText = "";
        public string StackPickupNotificationText = "";

        public Color OutlineColor;

        //public Type PerkMonobehavior;


        #region Generic Pick up stuff
        public new string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }


        public new float GetDistanceToPoint(Vector2 point)
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

        public new float GetOverrideMaxDistance()
        {
            return 1f;
        }



        public new void OnEnteredRange(PlayerController interactor)
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

            if (extantLabel != null) { Destroy(extantLabel.gameObject); }

            string Text = StringTableManager.GetItemsString(this.encounterTrackable.journalData.PrimaryDisplayName);
            foreach (var enrty in this.perkDisplayContainers)
            {
                bool req = enrty.requiresFlag == false ? false : SaveAPI.AdvancedGameStatsManager.Instance.GetFlag(enrty.FlagToTrack);
                bool req2 = enrty.requiresStack == false ? false : GameStatsManager.Instance.m_encounteredTrackables[this.encounterTrackable.EncounterGuid].encounterCount >= enrty.AmountToBuyBeforeReveal;
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

        public new void OnExitRange(PlayerController interactor)
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
        public bool isDummy = false;

        public PlayerController _Owner;

        public override void Pickup(PlayerController interactor)
        {
            bool isNotLoadScreen = GameManager.Instance.Dungeon != null;

            string t = InitialPickupNotificationText;
            this.encounterTrackable.SuppressInInventory = true;

            base.Pickup(interactor);
            m_owner = interactor;
            _Owner = interactor;


            if (interactor.passiveItems.Where(self => self.PickupObjectId == this.PickupObjectId).Count() == 1)
            {
                OnInitialPickup(interactor);
                UpdateStack(interactor, true);
            }
            else
            {
                SaveAPIManager.SetFlag(FlagToSetOnStack, true);
                t = StackPickupNotificationText;
                isDummy = true;
                (interactor.passiveItems.Where(self => self.PickupObjectId == PickupObjectId && (self as PerkPickupObject).isDummy == false).FirstOrDefault() as PerkPickupObject).UpdateStack(interactor);
            }

            if (isNotLoadScreen)
            {
                OtherTools.NotifyCustom(this.encounterTrackable.journalData.GetPrimaryDisplayName(), t,
                this.sprite.collection.spriteDefinitions[this.sprite._spriteId].name,
                this.sprite.collection,
                UINotificationController.NotificationColor.GOLD);

                Exploder.DoDistortionWave(interactor.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
                AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", interactor.gameObject);
            }




            if (extantLabel != null) { Destroy(extantLabel.gameObject); }



            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(interactor); }
            cont.Active = false;
    

            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
        }
        #endregion

        
        public void UpdateStack(PlayerController playerController, bool isFirstStack = false)
        {
            _CurrentStack++;
            OnStack(playerController);
        }

        public virtual void OnInitialPickup(PlayerController playerController)
        {

        }
        public virtual void OnStack(PlayerController playerController)
        {

        }
        public override void Update()
        {

        }
    }
}
