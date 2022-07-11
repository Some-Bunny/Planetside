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
using System.Collections.ObjectModel;

using UnityEngine.Serialization;

namespace Planetside
{   
    public class ShopDiscountItem : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Gun Warrant";
            string resourceName = "Planetside/Resources/gunwarrant.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ShopDiscountItem>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "The Buy To Carry";
            string longDesc = "Although being a Gungeoneer doesn't require a warrant, the benefits of having one is hard to pass by, especially for Gungeoneers that enjoy a steady supply of new weaponry.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.D;
            ShopDiscountItem.ShopDiscountItemID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);
            new Hook(typeof(BaseShopController).GetMethod("DoSetup", BindingFlags.Instance | BindingFlags.NonPublic), typeof(ShopDiscountItem).GetMethod("DoSetupHook"));

        }
        public static int ShopDiscountItemID;

        public static void DoSetupHook(Action<BaseShopController> orig, BaseShopController self)
        {
            orig(self);
            PlayerController[] players = GameManager.Instance.AllPlayers;
            for (int i = 0; i < players.Length; i++)
            {
                PlayerController player = players[i];
                if (player.HasPassiveItem(ShopDiscountItemID))
                {
                 
                    Type typeOne = typeof(BaseShopController); FieldInfo _propertyOne = typeOne.GetField("m_itemControllers", BindingFlags.NonPublic | BindingFlags.Instance); _propertyOne.GetValue(self);
                    List<ShopItemController> listOCont = (List<ShopItemController>)_propertyOne.GetValue(self);
                    for (int e = 0; e < listOCont.Count; e++)
                    {
                        ShopItemController shopItemController = listOCont[e];
                        if (ValidID(shopItemController.item.PickupObjectId))
                        {
                            GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
                            float newCost = shopItemController.item.PurchasePrice;
                            float num4 = (lastLoadedLevelDefinition == null) ? 1f : lastLoadedLevelDefinition.priceMultiplier;
                            newCost *= num4;
                            shopItemController.OverridePrice = (int)(newCost *= 0.5f);
                        }
                    }
                }
            }
        }

        /*
         *                 { 73, 0.5f }, //Half Heart
                { 78, 0.375f }, //Normal Ammo
                { 600, 0.25f }, //Partial Ammo
                { 77, 0f }, //Supply Drop
                { 120, 0.75f }, //Armor
                { 85, 0.5f }, //Full heart
                { 565, 0.25f }, //Glass Guon Stone
                { 224, 0.5f }, //Blank
                { 67, 0.625f }, //Key
        */
        public static bool ValidID(int ID)
        {
            if (ID == 73) { return true; }
            if (ID == 120) { return true; }
            if (ID == 85) { return true; }
            return false;
        }


      
        public override DebrisObject Drop(PlayerController player)
		{
            //player.OnEnteredCombat = (Action)Delegate.Remove(player.OnEnteredCombat, new Action(this.Warrant));
            DebrisObject result = base.Drop(player);	
			return result;
		}

		public override void Pickup(PlayerController player)
		{
            base.Pickup(player);
		}
	}
}


