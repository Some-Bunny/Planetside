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
            //string resourceName = "Planetside/Resources/keepersBox2.png";
            GameObject obj = new GameObject(itemName);
            ShopInABox activeitem = obj.AddComponent<ShopInABox>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("keepersBox2"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
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
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:shop-in-a-box",
                "ring_of_miserly_protection"
            };
            CustomSynergies.Add("Make Your Choice.", mandatoryConsoleIDs, null, true);

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

        public override void DoEffect(PlayerController user)
        {

            GameObject obj = new GameObject();
            StaticReferences.StoredRoomObjects.TryGetValue("gregthlyShop", out obj);
            LootEngine.DoDefaultItemPoof(user.transform.PositionVector2() + new Vector2(0.25f, 0.25f));
            GameObject shopObj = DungeonPlaceableUtility.InstantiateDungeonPlaceable(obj, user.CurrentRoom, new IntVector2((int)user.transform.position.x, (int)user.transform.position.y) - user.CurrentRoom.area.basePosition, false);
            CustomShopController shopCont = shopObj.GetComponent<CustomShopController>();
            if (shopCont != null)
            {
                if (UnityEngine.Random.value < 0.3f)
                { shopCont.shopItems = UnityEngine.Random.value > 0.5f ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable; }
                if (user.PlayerHasActiveSynergy("Make Your Choice."))
                {
                    List<Vector2> itemPositions = new List<Vector2>()
                    {
                         shopObj.transform.PositionVector2() + new Vector2(0.75f, 0.875f),
                         shopObj.transform.PositionVector2() +  new Vector2(-0.5f, 0.375f),
                         shopObj.transform.PositionVector2() +   new Vector2(2f, 0.375f)
                    };
                    var posList = new List<Transform>();
                    for (int i = 0; i < itemPositions.Count; i++)
                    {
                        var ItemPoint = new GameObject("ItemPoint" + i);
                        ItemPoint.transform.position = itemPositions[i];
                        FakePrefab.MarkAsFakePrefab(ItemPoint);
                        UnityEngine.Object.DontDestroyOnLoad(ItemPoint);
                        ItemPoint.SetActive(true);
                        posList.Add(ItemPoint.transform);
                    }
                    shopCont.spawnPositions = posList.ToArray();

                    foreach (var pos in shopCont.spawnPositions)
                    {
                        pos.parent = shopObj.gameObject.transform;
                    }
                }
            }
        }
    }
}

