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
using GungeonAPI;
using Pathfinding;
using SaveAPI;

namespace Planetside
{
    public class ShipmentRequestForm : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Hegemony Shipment Ticket";
            string resourceName = "Planetside/Resources/shormActual.png";
            GameObject obj = new GameObject(itemName);
            ShipmentRequestForm activeitem = obj.AddComponent<ShipmentRequestForm>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Oh **** The Shipping and Recieving!";
            string longDesc = "Hello STRING.EMPTY, we would like you to know that your package is available for collection at these collection stations:\n\nSTRING.EMPTY\n\nSTRING.EMPTY\n\nSTRING.EMPTY\n\nYour package can be retrieved using this ticket as at any date, up to 01/03/XXXX. Your package is also able to be collected past your expiration date, with additional paperwork required to be printed and filled out, and is to be handed in with your ticket.\n\nRegards, Hegemony Shipment Services.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 1);
            activeitem.consumable = true;
            activeitem.quality = PickupObject.ItemQuality.C;

            ShipmentRequestForm.ShipmentRequestFormID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

            RequestInRunTable = LootTableTools.CreateLootTable();
            RequestInRunTable.AddItemsToPool(new Dictionary<int, float>() { 
                { 73, 0.5f }, //Half Heart
                { 78, 0.375f }, //Normal Ammo
                { 600, 0.25f }, //Partial Ammo
                { 77, 0f }, //Supply Drop
                { 120, 0.75f }, //Armor
                { 85, 0.5f }, //Full heart
                { 565, 0.25f }, //Glass Guon Stone
                { 224, 0.5f }, //Blank
                { 67, 0.625f }, //Key
                { LeSackPickup.SaccID, 0.1f }, });

            RequestStartRunTable = LootTableTools.CreateLootTable();
            RequestStartRunTable.AddItemsToPool(new Dictionary<int, float>() {
                { 77, 0.1f }, //Supply Drop
                { 120, 0.75f }, //Armor
                { 565, 0.5f }, //Glass Guon Stone
                { 224, 0.7f }, //Blank
                { 67, 0.75f }, //Key
                });
            Actions.OnRunStart += OnRunStart;
        }
        public static void OnRunStart(PlayerController player)
        {
            if (SaveAPIManager.GetFlag(CustomDungeonFlags.SHIPMENT_TICKET_HAD) == true)
            {
                SaveAPIManager.SetFlag(CustomDungeonFlags.SHIPMENT_TICKET_HAD, false);
                GameManager.Instance.StartCoroutine(SpawnCrates(true, player));
            }
        }

        public static GenericLootTable RequestInRunTable;
        public static GenericLootTable RequestStartRunTable;

        public static int ShipmentRequestFormID;
        public override void Pickup(PlayerController player)
        {
            SaveAPIManager.SetFlag(CustomDungeonFlags.SHIPMENT_TICKET_HAD, true);
            base.Pickup(player);     
        }

        public override bool CanBeUsed(PlayerController user)
        {
            //Literally just stole this from Lead Key because im honestly not bothered to make a check myself
            if (!user) { return false; }
            if (user.IsInCombat | user.IsInMinecart | user.InExitCell) { return false; }
            if (user.CurrentRoom != null && user.CurrentRoom.IsSealed) { return false; }
            return true;
        }

        protected override void DoEffect(PlayerController user)
        {
            LootEngine.DoDefaultItemPoof(user.sprite.WorldBottomCenter);
            SaveAPIManager.SetFlag(CustomDungeonFlags.SHIPMENT_TICKET_HAD, false);
            GameManager.Instance.StartCoroutine(SpawnCrates(false, user));
        }

        protected override void OnPreDrop(PlayerController user)
        {
            SaveAPIManager.SetFlag(CustomDungeonFlags.SHIPMENT_TICKET_HAD, false);
            base.OnPreDrop(user);
        }

        private static IEnumerator SpawnCrates(bool isStartRun, PlayerController player)
        {
            float ang = BraveUtility.RandomAngle();
            Vector2 pos = player.sprite.WorldCenter;
            for (int e = 0; e < 6; e++)
            {
                float elapsed = 0;
                while (elapsed < 0.33f)
                {
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                AkSoundEngine.PostEvent("Play_OBJ_supplydrop_activate_01", player.gameObject);
                SpawnCrate(isStartRun == false ? RequestInRunTable.SelectByWeight().GetComponent<PickupObject>().PickupObjectId : RequestStartRunTable.SelectByWeight().GetComponent<PickupObject>().PickupObjectId, player, e, ang, pos);
            }
            yield break;
        }

        private static void SpawnCrate(int item, PlayerController p, int i, float ang, Vector2 position)
        {
            GameObject gameObject = (GameObject)BraveResources.Load("EmergencyCrate", ".prefab");
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
            EmergencyCrateController component = gameObject2.GetComponent<EmergencyCrateController>();
            SimplerCrateBehaviour simpleCrate = component.TurnIntoSimplerCrate();
            simpleCrate.LootID = item; 
            
            
            Vector3 pos = position + MathToolbox.GetUnitOnCircle((60*i)+ang, 2f);
            simpleCrate.Trigger(new Vector3(-5f, -5f, -5f), pos + new Vector3(15f, 15f, 15f), p.CurrentRoom);
        }
    }


    public static class CrateExtensions
    {
        public static SimplerCrateBehaviour TurnIntoSimplerCrate(this EmergencyCrateController self)
        {
            GameObject obj = self.gameObject;
            if (obj != null)
            {
                SimplerCrateBehaviour newCrateBehav = obj.AddComponent<SimplerCrateBehaviour>();
                newCrateBehav.driftAnimationName = self.driftAnimationName;
                newCrateBehav.landedAnimationName = self.landedAnimationName;
                newCrateBehav.chuteLandedAnimationName = self.chuteLandedAnimationName;
                newCrateBehav.crateDisappearAnimationName = self.crateDisappearAnimationName;
                newCrateBehav.chuteAnimator = self.chuteAnimator;
                newCrateBehav.landingTargetSprite = self.landingTargetSprite;

                UnityEngine.Object.Destroy(self);
                return newCrateBehav;
            }
            else return null;
        }
    }
    public class SimplerCrateBehaviour : BraveBehaviour
    {
        public void Trigger(Vector3 startingVelocity, Vector3 startingPosition, RoomHandler room)
        {
            this.m_parentRoom = room;
            this.m_currentPosition = startingPosition;
            this.m_currentVelocity = startingVelocity;
            this.m_hasBeenTriggered = true;
            base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            float num = startingPosition.z / -startingVelocity.z;
            Vector3 position = startingPosition + num * startingVelocity;
            this.m_landingTarget = SpawnManager.SpawnVFX(this.landingTargetSprite, position, Quaternion.identity);
            this.m_landingTarget.GetComponentInChildren<tk2dSprite>().UpdateZDepth();
        }
        private void Update()
        {
            if (this.m_hasBeenTriggered)
            {
                this.m_currentPosition += this.m_currentVelocity * BraveTime.DeltaTime;
                if (this.m_currentPosition.z <= 0f)
                {
                    this.m_currentPosition.z = 0f;
                    this.OnLanded();
                }
                base.transform.position = BraveUtility.QuantizeVector(this.m_currentPosition.WithZ(this.m_currentPosition.y - this.m_currentPosition.z), (float)PhysicsEngine.Instance.PixelsPerUnit);
                base.sprite.HeightOffGround = this.m_currentPosition.z;
                base.sprite.UpdateZDepth();
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void OnLanded()
        {
            this.m_hasBeenTriggered = false;
            base.sprite.gameObject.layer = LayerMask.NameToLayer("FG_Critical");
            base.sprite.renderer.sortingLayerName = "Background";
            base.sprite.IsPerpendicular = false;
            base.sprite.HeightOffGround = -1f;
            this.m_currentPosition.z = -1f;
            base.spriteAnimator.Play(this.landedAnimationName);
            this.chuteAnimator.PlayAndDestroyObject(this.chuteLandedAnimationName, null);
            if (this.m_landingTarget)
            {
                SpawnManager.Despawn(this.m_landingTarget);
            }
            this.m_landingTarget = null;

            GameObject gameObject = PickupObjectDatabase.GetById(LootID).gameObject;

            DebrisObject spawned = LootEngine.SpawnItem(gameObject, base.sprite.WorldCenter.ToVector3ZUp(0f) + new Vector3(-0.5f, 0.5f, 0f), Vector2.zero, 0f, false, false, false);
            base.StartCoroutine(this.DestroyCrateWhenPickedUp(spawned));
        }

        private IEnumerator DestroyCrateDelayed()
        {
            yield return new WaitForSeconds(1.5f);
            if (this.m_landingTarget)
            {
                SpawnManager.Despawn(this.m_landingTarget);
            }
            this.m_landingTarget = null;
            if (this.m_parentRoom.ExtantEmergencyCrate == base.gameObject)
            {
                this.m_parentRoom.ExtantEmergencyCrate = null;
            }
            base.spriteAnimator.Play(this.crateDisappearAnimationName);
            yield break;
        }
        private IEnumerator DestroyCrateWhenPickedUp(DebrisObject spawned)
        {
            while (spawned)
            {
                yield return new WaitForSeconds(0.25f);
            }
            if (this.m_landingTarget)
            {
                SpawnManager.Despawn(this.m_landingTarget);
            }
            this.m_landingTarget = null;
            if (this.m_parentRoom.ExtantEmergencyCrate == base.gameObject)
            {
                this.m_parentRoom.ExtantEmergencyCrate = null;
            }
            base.spriteAnimator.Play(this.crateDisappearAnimationName);
            yield break;
        }
        public int LootID;

        public void ClearLandingTarget()
        {
            if (this.m_landingTarget)
            {
                SpawnManager.Despawn(this.m_landingTarget);
            }
            this.m_landingTarget = null;
        }
        public string driftAnimationName;
        public string landedAnimationName;
        public string chuteLandedAnimationName;
        public string crateDisappearAnimationName;
        public tk2dSpriteAnimator chuteAnimator;
        public GameObject landingTargetSprite;
        private bool m_hasBeenTriggered;
        private Vector3 m_currentPosition;
        private Vector3 m_currentVelocity;
        private RoomHandler m_parentRoom;
        private GameObject m_landingTarget;
    }

}

