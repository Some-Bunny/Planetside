using DaikonForge.Tween;
using Dungeonator;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planetside
{
    class AllSeeingEyeMiniPickup : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "All Seeing Interactable";
            //string resourcePath = "Planetside/Resources/Pickups/eye_pickup.png";
            GameObject gameObject = new GameObject(name);
            AllSeeingEyeMiniPickup item = gameObject.AddComponent<AllSeeingEyeMiniPickup>();

            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("eye_pickup"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Nolla.";
            string longDesc = "Nullifies you, for an inner reward.";
            item.SetupItem(shortDesc, longDesc, "psog");
            AllSeeingEyeMiniPickup.MiniEye = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            item.IgnoredByRat = true;
        }
        public static int MiniEye;


        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            m_hasBeenPickedUp = true;
            AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", player.gameObject);

            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX);
            gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(player.transform.PositionVector2() + new Vector2(0f, -0.5f), tk2dBaseSprite.Anchor.LowerCenter);
            gameObject2.transform.position = gameObject2.transform.position.Quantize(0.0625f);
            gameObject2.GetComponent<tk2dBaseSprite>().UpdateZDepth();

            FloorRewardData currentRewardData = GameManager.Instance.RewardManager.CurrentRewardData;
            List<DebrisObject> perksThatExist = new List<DebrisObject>() { };
            for (int e = 0; e < 3; e++)
            {
                DebrisObject debrisSpawned = LootEngine.SpawnItem(currentRewardData.SingleItemRewardTable.SelectByWeight().gameObject, player.sprite.WorldCenter.ToVector3ZisY() + MathToolbox.GetUnitOnCircle((360 / 3) * e, 0.875f).ToVector3ZisY() + new Vector3(-0.5f, 0), MathToolbox.GetUnitOnCircle((360 / 3) * e, 5), 3).GetComponent<DebrisObject>();
                if (debrisSpawned.gameObject.GetComponent<PickupMover>() != null) { Destroy(debrisSpawned.gameObject.GetComponent<PickupMover>()); }
                perksThatExist.Add(debrisSpawned);
            }
            if (perksThatExist.Count >= 0 || perksThatExist != null)
            {
                GameManager.Instance.Dungeon.StartCoroutine(ItemChoiceCoroutine(perksThatExist));
            }
            OtherTools.NotifyCustom("Choose One.", "You Foresee What You Desire", "eye_pickup", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);

            //OtherTools.Notify("Choose One.", "You Foresee What You Desire.", "Planetside/Resources/Pickups/eye_pickup", UINotificationController.NotificationColor.GOLD);

            player.BloopItemAboveHead(base.sprite, "");
            UnityEngine.Object.Destroy(base.gameObject);
        }

        public static IEnumerator ItemChoiceCoroutine(List<DebrisObject> pickups)
        {
            for (; ; )
            {
                foreach (DebrisObject obj in pickups)
                {
                    if (!obj)
                    {
                        pickups.Remove(obj);
                        foreach (DebrisObject obj2 in pickups)
                        {
                            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX);
                            gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(obj2.transform.PositionVector2() + new Vector2(0f, -0.5f), tk2dBaseSprite.Anchor.LowerCenter);
                            gameObject2.transform.position = gameObject2.transform.position.Quantize(0.0625f);
                            gameObject2.GetComponent<tk2dBaseSprite>().UpdateZDepth();
                            UnityEngine.Object.Destroy(obj2.gameObject);
                        }
                        yield break;
                    }
                }
                yield return null;
            }
        }


        public float distortionMaxRadius = 30f;
        public float distortionDuration = 2f;
        public float distortionIntensity = 0.7f;
        public float distortionThickness = 0.1f;
        public void Start()
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
