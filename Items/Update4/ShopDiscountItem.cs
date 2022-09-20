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
using SaveAPI;

namespace Planetside
{   


    public class ShopDiscount : MonoBehaviour
    {
        public string IdentificationKey;
        public float PriceMultiplier;
        public List<int> IDsToReducePriceOf = new List<int>();
        public int PriceReductionItemID;
        public bool OverridePriceReduction;
        public string SynergyToTrack;

        public bool PriceReductionActive()
        {
            if (OverridePriceReduction == true) { return false; }
            PlayerController[] players = GameManager.Instance.AllPlayers;
            for (int i = 0; i < players.Length; i++)
            {
                PlayerController player = players[i];
                if (discountTrigger == Trigger.ITEM)
                {
                    if (player.HasPassiveItem(PriceReductionItemID))
                    {
                        return true;
                    }
                }
                if (discountTrigger == Trigger.SYNERGY)
                {
                    if (SynergyToTrack != null && player.PlayerHasActiveSynergy(SynergyToTrack))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Trigger discountTrigger = Trigger.ITEM;
        public enum Trigger
        {
            ITEM,
            SYNERGY,
        };

    }


    public class ShopDiscountController : MonoBehaviour
    {
        public ShopDiscountController()
        {         
            shopItemSelf = this.GetComponent<ShopItemController>();
        }

        

        public void Update()
        {
            
                DoPriceReduction();

        }
        public void DoPriceReduction()
        {
            if (shopItemSelf == null) {return; }

            if (GameStatsManager.Instance != null)
            {
                if (shopItemSelf.item is PaydayDrillItem && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_STOLE_DRILL) == false) { return; }
                if (shopItemSelf.item is BankMaskItem && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_STOLE_BANKMASK) == false) { return; }
                if (shopItemSelf.item is BankBagItem && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_STOLE_BANKBAG) == false) { return; }
            }

            float mult = 1;
            foreach (var DiscountVar in discounts)
            {
                if (ValidID(DiscountVar, shopItemSelf.item.PickupObjectId) == true && DiscountVar.PriceReductionActive() == true)
                {
                    mult *= DiscountVar.PriceMultiplier;
                }
            }
            DoTotalDiscount(mult);
        }
        public void DoTotalDiscount(float H)
        {
            if (shopItemSelf == null) { return; }
            if (GameManager.Instance == null) { return; }
            if (GameManager.Instance.PrimaryPlayer == null) { return; }

            GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
            float newCost = shopItemSelf.item.PurchasePrice;
            float num4 = (lastLoadedLevelDefinition == null) ? 1f : lastLoadedLevelDefinition.priceMultiplier;
            float num3 = GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier);
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.SecondaryPlayer)
            {
                num3 *= GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier);
            }
            newCost *= num4 * num3;
            shopItemSelf.OverridePrice = (int)(newCost *= H);
        }

        public void ReturnPriceToDefault()
        {
            if (shopItemSelf == null) { return; }
            GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance != null ? GameManager.Instance.GetLastLoadedLevelDefinition() : null;
            float newCost = shopItemSelf.item.PurchasePrice;
            float num4 = (lastLoadedLevelDefinition == null) ? 1f : lastLoadedLevelDefinition.priceMultiplier;

            float num3 = GameManager.Instance.PrimaryPlayer != null ? GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier) : 1;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.SecondaryPlayer)
            {
                num3 *= GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier);
            }

            newCost *= num4 * num3;
            shopItemSelf.OverridePrice = (int)(newCost);
        }


        public void DisableSetShopDiscount(string stringID, bool b)
        {
            foreach (var DiscountVar in discounts)
            {
                if (DiscountVar.IdentificationKey == stringID) { DiscountVar.OverridePriceReduction = b; }
            }
        }

        public ShopDiscount ReturnShopDiscountFromController(string IDTag)
        {
            foreach (var DiscountVar in discounts)
            {
                if (DiscountVar.IdentificationKey == IDTag) {return DiscountVar; }
            }
            return null;
        }

        public void OnDestroy()
        {
            if (shopItemSelf != null)
            {
                ReturnPriceToDefault();
            }
        }

      
        public bool ValidID(ShopDiscount shopDiscount,int ID)
        {
            if (shopDiscount.IDsToReducePriceOf.Contains(ID)) { return true; }
            return false;
        }


        public List<ShopDiscount> discounts = new List<ShopDiscount>();
        public ShopItemController shopItemSelf;
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
        public static Action<ShopItemController> OnShopItemStarted;

        public static void OnMyShopItemStarted(ShopItemController shopItemController)
        {
            ShopDiscountController steamSale = shopItemController.gameObject.AddComponent<ShopDiscountController>();
            steamSale.discounts = new List<ShopDiscount>() { new ShopDiscount()
            {
                IdentificationKey = "One",
                PriceMultiplier = 0.5f,
                IDsToReducePriceOf =  new List<int>() { 73, 85, 120 },
                PriceReductionItemID = ShopDiscountItemID,

            }
            };
            /*
            new ShopDiscount()
            {
                IdentificationKey = "Two",
                PriceMultiplier = 0.5f,
                IDsToReducePriceOf =  new List<int>() { 73, 85, 224 },
                PriceReductionItemID = TarnishedAmmolet.TarnishedAmmoletID,
            }
            };
            */
            //steamSale.IdentificationKey = "HP_Reduction";
            //steamSale.IDsToReducePriceOf = new List<int>() { 73, 85, 120 };
            //steamSale.PriceReductionItemID = ShopDiscountItemID;
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


