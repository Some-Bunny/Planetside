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
    public class ShopDiscountController : MonoBehaviour
    {
        public ShopDiscountController()
        {
            isPriceReduced = false;
            IdentificationKey = "None";
            PriceMultiplier = 0.5f;
            PriceReductionItemID = -1;
            shopItemSelf = this.GetComponent<ShopItemController>();
        }

        public void Update()
        {
            if (PlayerHasValidItem() == true && ValidID(shopItemSelf.item.PickupObjectId))
            {
                DoPriceReduction();
            }
            else
            {
                ReturnPriceToDefault();
            }
        }
        public void DoPriceReduction()
        {
            if (isPriceReduced == true) { return; }
            if (shopItemSelf == null) { return; }
            isPriceReduced = true;
            GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
            float newCost = shopItemSelf.item.PurchasePrice;
            float num4 = (lastLoadedLevelDefinition == null) ? 1f : lastLoadedLevelDefinition.priceMultiplier;
            newCost *= num4;
            shopItemSelf.OverridePrice = (int)(newCost *= PriceMultiplier);
        }
        public void ReturnPriceToDefault()
        {
            if (isPriceReduced == false) { return; }
            if (shopItemSelf == null) { return; }

            isPriceReduced = false;
            GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
            float newCost = shopItemSelf.item.PurchasePrice;
            float num4 = (lastLoadedLevelDefinition == null) ? 1f : lastLoadedLevelDefinition.priceMultiplier;
            newCost *= num4;
            shopItemSelf.OverridePrice = (int)(newCost);
        }

        public void OnDestroy()
        {
            ReturnPriceToDefault();
        }

        public bool PlayerHasValidItem()
        {
            PlayerController[] players = GameManager.Instance.AllPlayers;
            for (int i = 0; i < players.Length; i++)
            {
                PlayerController player = players[i];
                if (player.HasPassiveItem(PriceReductionItemID))
                {
                    return true;
                }
            }
            return false;
        }
        public bool ValidID(int ID)
        {
            if (IDsToReducePriceOf.Contains(ID)) { return true; }
            return false;
        }
        public string IdentificationKey;
        public float PriceMultiplier;
        public List<int> IDsToReducePriceOf = new List<int>();
        public int PriceReductionItemID;
        public ShopItemController shopItemSelf;
        public bool isPriceReduced;
    }

    public class ShopDiscountItem : PassiveItem
    {
        public static void Init()
        {
            string itemName = "testPriceItem";
            string resourceName = "Planetside/Resources/gunwarrant.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ShopDiscountItem>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "The Buy To Carry";
            string longDesc = "Although being a Gungeoneer doesn't require a warrant, the benefits of having one is hard to pass by, especially for Gungeoneers that enjoy a steady supply of new weaponry.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.EXCLUDED;
            ShopDiscountItem.ShopDiscountItemID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);
            //new Hook(typeof(BaseShopController).GetMethod("DoSetup", BindingFlags.Instance | BindingFlags.NonPublic), typeof(ShopDiscountItem).GetMethod("DoSetupHook"));


           new Hook(typeof(ShopItemController).GetMethods().Single(
                a =>
                    a.Name == "Initialize" &&
                    a.GetParameters().Length == 2 &&
                    a.GetParameters()[1].ParameterType == typeof(BaseShopController)),
                typeof(ShopDiscountItem).GetMethod("InitializeViaBaseShopController", BindingFlags.Static | BindingFlags.Public));
            
            new Hook(typeof(ShopItemController).GetMethods().Single(
              a =>
                  a.Name == "Initialize" &&
                  a.GetParameters().Length == 2 &&
                  a.GetParameters()[1].ParameterType == typeof(ShopController)),
              typeof(ShopDiscountItem).GetMethod("InitializeViaShopController", BindingFlags.Static | BindingFlags.Public));

            OnShopItemStarted += OnMyShopItemStarted;
        }
        public static int ShopDiscountItemID;


        public static void InitializeViaBaseShopController(Action<ShopItemController, PickupObject, BaseShopController> orig, ShopItemController self, PickupObject i, BaseShopController parent)
        {
            orig(self, i, parent);
            if (OnShopItemStarted != null)
            {
                OnShopItemStarted(self);
            }
        }

        public static void InitializeViaShopController(Action<ShopItemController, PickupObject, ShopController> orig, ShopItemController self, PickupObject i, ShopController parent)
        {
            orig(self, i, parent);
            if (OnShopItemStarted != null)
            {
                OnShopItemStarted(self);
            }
        }

        public static void OnMyShopItemStarted(ShopItemController shopItemController)
        {
            ShopDiscountController steamSale = shopItemController.gameObject.AddComponent<ShopDiscountController>();
            steamSale.IdentificationKey = "HP_Reduction";
            steamSale.IDsToReducePriceOf = new List<int>() { 73, 85, 120 };
            steamSale.PriceReductionItemID = ShopDiscountItemID;
        }
        public static void DoSetupHook(Action<BaseShopController> orig, BaseShopController self)
        {
            orig(self);

            Type typeOne = typeof(BaseShopController); FieldInfo _propertyOne = typeOne.GetField("m_itemControllers", BindingFlags.NonPublic | BindingFlags.Instance); _propertyOne.GetValue(self);
            List<ShopItemController> listOCont = (List<ShopItemController>)_propertyOne.GetValue(self);
            for (int e = 0; e < listOCont.Count; e++)
            {
                if (OnShopItemStarted  != null)
                {
                    OnShopItemStarted(listOCont[e]);
                }    
            }           
        }

        public static Action<ShopItemController> OnShopItemStarted;


        /*
         *      { 73, 0.5f }, //Half Heart
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
            DebrisObject result = base.Drop(player);	
			return result;
		}

		public override void Pickup(PlayerController player)
		{
            base.Pickup(player);
		}
	}
}


