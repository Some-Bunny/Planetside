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
using NpcApi;

namespace Planetside
{   

    public class ExampleShopDiscountSetup
    {
        public static void Init()
        {
            /*
            ShopDiscountMegaMind.DiscountsToAdd.Add(new ShopDiscount()
            {
                IdentificationKey = "Example_Discount",
                PriceMultiplier = 0.5f, //Halves the price of selected ids
                CanDiscountCondition = Example_CanBuy, // your example condition (required)
                CustomPriceMultiplier = Example_Multiplier,// your example price multiplier controller (can be left as null and it will just use PriceMultiplier always)
                ItemToDiscount = Example_Criteria
            });
            */
        }

        public static bool Example_Criteria(ShopItemController shopItemController)//this will be ran to see if your shop item even has the right criteria for being discounted. if returns TRUE, will be able to be discounted (if there is disocunt conditions in the first place), else will not be
        {
            if(shopItemController.item.PickupObjectId == 69) { return true; } //has a ShopItemController accessible that you can *yoink* any information you need
            return false;
        }
        public static bool Example_CanBuy() //this will be ran to see if your discount should activate. if returns TRUE, will be active, else will not be
        {
            foreach (var p in GameManager.Instance.AllPlayers)
            {
                if (p.HasPickupID(CandyHeart.CandyHeartID) == true)
                {
                    return true;
                }
            }
            return false;
        }

        public static float Example_Multiplier() //this will be ran to see what multiplier your shop item should use. You can add your own code to make it smoothly switch prices and stuff
        {
            foreach (var p in GameManager.Instance.AllPlayers)
            {
                if (p.PlayerHasActiveSynergy("your synergy") == true)
                {
                    return 0.33f;
                }
            }
            return 0.5f;
        }
    }



    public class ShopDiscountMegaMind
    {

        //INITIALISE THIS, VERY IMPORTANT!!!
        public static void Init()
        {
            new Hook(typeof(ShopItemController).GetMethods().Single(
               a =>
                   a.Name == "Initialize" &&
                   a.GetParameters().Length == 2 &&
                   a.GetParameters()[1].ParameterType == typeof(BaseShopController)),
               typeof(ShopDiscountMegaMind).GetMethod("InitializeViaBaseShopController", BindingFlags.Static | BindingFlags.Public));

            new Hook(typeof(ShopItemController).GetMethods().Single(
              a =>
                  a.Name == "Initialize" &&
                  a.GetParameters().Length == 2 &&
                  a.GetParameters()[1].ParameterType == typeof(ShopController)),
              typeof(ShopDiscountMegaMind).GetMethod("InitializeViaShopController", BindingFlags.Static | BindingFlags.Public));

            OnShopItemStarted += OnMyShopItemStartedGlobal;
        }


        /// <summary>
        /// A subscribable Action for when any ShopItemController is started or pops into existence.
        /// </summary>
        public static Action<ShopItemController> OnShopItemStarted;

        /// <summary>
        /// The list you add your ShopDiscounts to. These will be added automatically when ShopDiscountController starts anywhere.
        /// </summary>
        public static List<ShopDiscount> DiscountsToAdd = new List<ShopDiscount>();

        public static void OnMyShopItemStartedGlobal(ShopItemController shopItemController)
        {
            if (shopItemController.gameObject.GetComponent<ShopDiscountController>() != null) { UnityEngine.Object.Destroy(shopItemController.gameObject.GetComponent<ShopDiscountController>()); }
            ShopDiscountController steamSale = shopItemController.gameObject.AddComponent<ShopDiscountController>();
            steamSale.UpdatePlacement();
            steamSale.discounts = DiscountsToAdd ?? new List<ShopDiscount>() { };
        }


        //The hooks required for the shop items to register
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
    }
    public class ShopDiscount : MonoBehaviour
    {
        // The shop discount itself. This class controls how your discount should work, the price reduction amount and the purchase condition, and other things.
        /// <summary>
        /// The name of your discount. Mostly just for organization and other things.
        /// </summary>
        public string IdentificationKey = "ShopDisc";
        /// <summary>
        /// Price multipler, self explanatory. Set it to 0.5f and whatever items you set it to will be half price!
        /// </summary>
        public float PriceMultiplier = 1f;
        /// <summary>
        /// A list of item IDs you want to be discounted when the discount condition is active.
        /// </summary>
        public Func<ShopItemController, bool> ItemToDiscount;

        /// <summary>
        /// A function for your *condition* in which your discount will be active. Make sure to return it as TRUE when it should be active.
        /// </summary>
        public Func<bool> CanDiscountCondition;

        /// <summary>
        /// A function that lets you give a *custom* price multipler, for more dynamic price reductions..
        /// </summary>
        public Func<ShopItemController ,float> CustomPriceMultiplier;


        private bool OverridePriceReduction = false;
        /// <summary>
        /// Returns the current override value. Your discount will NOT be active while the override value is TRUE.
        /// </summary>
        public bool GetOverride() { return OverridePriceReduction; }
        /// <summary>
        /// Sets the override value. Your discount will NOT be active while the override value is TRUE.
        /// </summary>
        public void SetOverride(bool overrideType) { OverridePriceReduction = overrideType; }
        /// <summary>
        /// Returns TRUE if your discount is active.
        /// </summary>
        public bool CanBeDiscounted()
        {
            if (OverridePriceReduction == true) { return false; }
            if (CanDiscountCondition != null)
            {
                return CanDiscountCondition();
            }
            return false;
        }


        public float ReturnCustomPriceMultiplier(ShopItemController itemController)
        {
            if (CustomPriceMultiplier != null)
            {
                return CustomPriceMultiplier(itemController);
            }
            return PriceMultiplier;
        }
    }

    public class ShopDiscountController : MonoBehaviour
    {
        public ShopDiscountController()
        {
        }

        public void UpdatePlacement()
        {
            shopItemSelf = this.GetComponent<ShopItemController>();
            if (shopItemSelf != null)
            {
                shopItemSelf.StartCoroutine(FrameDelay());
            }
        }
        public IEnumerator FrameDelay()
        {
            yield return null;
            if (DoManyChecks() == true)
            {
                if (shopItemSelf is CustomShopItemController)
                {
                    StartPrice = shopItemSelf.OverridePrice ?? (shopItemSelf as CustomShopItemController).ModifiedPrice;//shopItemSelf.ModifiedPrice;
                }
                else
                {
                    StartPrice = shopItemSelf.OverridePrice ?? shopItemSelf.ModifiedPrice;
                }
            }
            FullyInited = true;
            yield break;
        }

        public void ResetPrice(ShopItemController newSelf, int? currentOverridePrice)
        {
            shopItemSelf = newSelf;
            if (shopItemSelf is CustomShopItemController)
            {
                StartPrice = currentOverridePrice ?? (shopItemSelf as CustomShopItemController).ModifiedPrice;//shopItemSelf.ModifiedPrice;
            }
            else
            {
                StartPrice = currentOverridePrice ?? shopItemSelf.ModifiedPrice;
            }
        }
        private bool FullyInited = false;

        private bool DoManyChecks()
        {
            if (GameManager.Instance == null) { return false; }
            if (GameManager.Instance.PrimaryPlayer == null) { return false; }
            return true;
        }

        private float StartPrice = -1;
        public void Update()
        {
            if (FullyInited == false) { return; }
            DoPriceReduction();
        }
        private void DoPriceReduction()
        {
            if (shopItemSelf == null) { return; }

            if (GameStatsManager.Instance != null)
            {
                if (shopItemSelf.item is PaydayDrillItem && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_STOLE_DRILL) == false) { return; } //Payday item failsafes because dodge roll hates us
                if (shopItemSelf.item is BankMaskItem && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_STOLE_BANKMASK) == false) { return; }
                if (shopItemSelf.item is BankBagItem && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_STOLE_BANKBAG) == false) { return; }
            }

            float mult = 1;
            foreach (var DiscountVar in discounts)
            {
                if (Valid(DiscountVar) == true)
                {
                    if (DiscountVar.CanBeDiscounted() == true)
                    {
                        mult *= DiscountVar.ReturnCustomPriceMultiplier(shopItemSelf);
                    }
                }
            }
            DoTotalDiscount(mult);
        }

        public bool ReturnMoneyCurrencyType()
        {
            return shopItemSelf.CurrencyType == ShopItemController.ShopCurrencyType.COINS;
        }

        private void DoTotalDiscount(float H)
        {
            if (shopItemSelf == null) { return; }   
            if (GameManager.Instance == null) { return; }
            if (GameManager.Instance.PrimaryPlayer == null) { return; }

            //GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();


            float newCost = StartPrice != -1 ? StartPrice : ReturnMoneyCurrencyType() == false ? shopItemSelf.CurrentPrice : shopItemSelf.ModifiedPrice;
            //float num4 = (lastLoadedLevelDefinition == null) ? 1f : lastLoadedLevelDefinition.priceMultiplier;
            
            float num3 = GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier);
           
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.SecondaryPlayer)
            {
                num3 *= GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier);
            }
      
            newCost *= num3;
            shopItemSelf.OverridePrice = (int)(newCost *= H);
        }

    

        /// <summary>
        /// Sets the override for a ShopDiscount with a specific IdentificationKey.
        /// </summary>
        public void DisableSetShopDiscount(string stringID, bool b)
        {
            foreach (var DiscountVar in discounts)
            {
                if (DiscountVar.IdentificationKey == stringID) { DiscountVar.SetOverride(b); }
            }
        }
        /// <summary>
        /// Returns a ShopDiscount with a specific IdentificationKey.
        /// </summary>
        public ShopDiscount ReturnShopDiscountFromController(string IDTag)
        {
            foreach (var DiscountVar in discounts)
            {
                if (DiscountVar.IdentificationKey == IDTag) { return DiscountVar; }
            }
            return null;
        }
        private void OnDestroy()
        {
            if (shopItemSelf != null)
            {
                shopItemSelf.OverridePrice = null;
            }
        }

        //checks if the item itself is valid in the first place
        private bool Valid(ShopDiscount shopDiscount)
        {
            if (shopItemSelf == null) { return false; }
            if (shopDiscount.ItemToDiscount != null) { return shopDiscount.ItemToDiscount(shopItemSelf); }
            return false;
        }

        public List<ShopDiscount> discounts = new List<ShopDiscount>();
        private ShopItemController shopItemSelf;
    }


    //=============================================================================
    //=============================================================================
    //=============================================================================
    //=============================================================================
    //=============================================================================
    //=============================================================================









    public class ShopDiscountItem : PassiveItem
    {
        public static void Init()
        {
            string itemName = "testPriceItem";
            //string resourceName = "Planetside/Resources/gunwarrant.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ShopDiscountItem>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("gunwarrant"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "The Buy To Carry";
            string longDesc = "Although being a Gungeoneer doesn't require a warrant, the benefits of having one is hard to pass by, especially for Gungeoneers that enjoy a steady supply of new weaponry.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.EXCLUDED;
            ShopDiscountItem.ShopDiscountItemID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);
            //new Hook(typeof(BaseShopController).GetMethod("DoSetup", BindingFlags.Instance | BindingFlags.NonPublic), typeof(ShopDiscountItem).GetMethod("DoSetupHook"));


          
        }
        public static int ShopDiscountItemID;


       

        public static bool CanBuyItem()
        {
            PlayerController[] players = GameManager.Instance.AllPlayers;
            for (int i = 0; i < players.Length; i++)
            {
                PlayerController player = players[i];
                if (player.HasPassiveItem(ShopDiscountItem.ShopDiscountItemID))
                {
                    return true;
                }
            }
            return false;
        }

        public static void OnMyShopItemStarted(ShopItemController shopItemController)
        {
            /*
            ShopDiscountController steamSale = shopItemController.gameObject.AddComponent<ShopDiscountController>();
            steamSale.discounts = new List<ShopDiscount>() { new ShopDiscount()
            {
                IdentificationKey = "One",
                PriceMultiplier = 0.5f,
                IDsToReducePriceOf =  new List<int>() { 73, 85, 120 },
                CanBuyCondition = CanBuyItem
            }
            };
            */
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

        /*
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
        */


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


