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
using NpcApi;

namespace Planetside
{
    public class ShopInABox : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Shop-In-A-Box";
            string resourceName = "Planetside/Resources/keepersBox2.png";
            GameObject obj = new GameObject(itemName);
            ShopInABox activeitem = obj.AddComponent<ShopInABox>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Faster Than Prime Shipping!";
            string longDesc = "Creates a small, purhasable item. \n\nThis this new-fangled box technology, you too, can support local bullet kin in their business endavours!";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 800f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.C;

            ShopInABox.ShopInABoxID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

            shopInABoxPickupTable = LootTableTools.CreateLootTable();
            shopInABoxPickupTable.AddItemsToPool(new Dictionary<int, float>() { { 73, 0.7f }, { 78, 0.66f }, { 600, 0.5f }, { 77, 0.2f }, { 120, 0.6f }, { 85, 0.6f }, { 565, 0.5f }, { 224, 0.7f }, { 67, 0.5f }, { LeSackPickup.SaccID, 0.33f }, });

        }
        public static GenericLootTable shopInABoxPickupTable;

        public static int ShopInABoxID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
           
        }

        public override bool CanBeUsed(PlayerController user)
        {
            //Literally just stole this from Lead Key because im honestly not bothered to make a check myself
            if (!user) { return false; }
            if (user?.CurrentRoom?.area?.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS) { return false; }
            if (user.IsInCombat | user.IsInMinecart | user.InExitCell) { return false; }
            if (user.CurrentRoom != null && user.CurrentRoom.IsSealed) { return false; }
            return true;
        }

        protected override void DoEffect(PlayerController user)
        {

            GameObject obj = new GameObject();
            StaticReferences.StoredRoomObjects.TryGetValue("gregthlyShop", out obj);
            if (UnityEngine.Random.value < 0.3f)
            {obj.GetComponent<CustomShopController>().shopItems = UnityEngine.Random.value > 0.5f ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable;}
            LootEngine.DoDefaultItemPoof(user.transform.PositionVector2() + new Vector2(0.25f, 0.25f));
            DungeonPlaceableUtility.InstantiateDungeonPlaceable(obj, user.CurrentRoom, new IntVector2((int)user.transform.position.x, (int)user.transform.position.y) - user.CurrentRoom.area.basePosition, false);
        }
    }
}

