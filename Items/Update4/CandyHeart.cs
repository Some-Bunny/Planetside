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
    public class CandyHeart : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Candy Heart";
            string resourceName = "Planetside/Resources/candyheart.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<CandyHeart>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "My Sweet Heart";
            string longDesc = "Hearts and armor are cheaper.\n\nA tacky heart-shaped candy with 'You're sweet <3' written on it.\n\nSadly, like most Gungeoneers, you have no one to give it to.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.D;
            CandyHeart.CandyHeartID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);
            ShopDiscountItem.OnShopItemStarted += OnMyShopItemStarted;
        }
        public static int CandyHeartID;
        public static void OnMyShopItemStarted(ShopItemController shopItemController)
        {
            ShopDiscountController steamSale = shopItemController.gameObject.AddComponent<ShopDiscountController>();
            steamSale.discounts = new List<ShopDiscount>() { new ShopDiscount()
            {
                IdentificationKey = "Candy_Heart",
                PriceMultiplier = 0.5f,
                IDsToReducePriceOf =  new List<int>() { 73, 85, 120 },
                PriceReductionItemID = CandyHeartID,
                discountTrigger = ShopDiscount.Trigger.ITEM
            }
            };      
        }
        public override DebrisObject Drop(PlayerController player)
		{
            DebrisObject result = base.Drop(player);	
			return result;
		}

		public override void Pickup(PlayerController player)
		{
            base.Pickup(player);
		}
	}
}


