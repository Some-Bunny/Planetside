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
            //string resourceName = "Planetside/Resources/candyheart.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<CandyHeart>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("candyheart"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "My Sweet Heart";
            string longDesc = "Hearts and armor are cheaper.\n\nA tacky heart-shaped candy with 'You're sweet <3' written on it.\n\nLike most Gungeoneers, you have no one to give it to.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.D;
            CandyHeart.CandyHeartID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);
            /*
            ShopDiscountMegaMind.DiscountsToAdd.Add(new ShopDiscount()
            {
                IdentificationKey = "Candy_Heart",
                PriceMultiplier = 0.5f,
                CanDiscountCondition = CanBuy,
                CustomPriceMultiplier = CanMult,
                ItemToDiscount = Can
            });
            */

            Alexandria.NPCAPI.CustomDiscountManager.DiscountsToAdd.Add(new Alexandria.NPCAPI.ShopDiscount()
            {
                IdentificationKey = "Candy_Heart",
                CanDiscountCondition = CanBuy,
                PriceMultiplier = 0.5f,
                ItemIsValidForDiscount = Can,
                CustomPriceMultiplier = CanMult,
            });

            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:candy_heart",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "antibody"
            };
            CustomSynergies.Add("Hearts In Halves", mandatoryConsoleIDs, optionalConsoleIDs, true);
        }
        public static int CandyHeartID;


        public static bool Can(ShopItemController s)
        {
            if (s.item.PickupObjectId == 73) { return true; }
            if (s.item.PickupObjectId == 85) { return true; }
            if (s.item.PickupObjectId == 120) { return true; }

            return false;
        }

        public static bool CanBuy()
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

        public static float CanMult()
        {
            foreach (var p in GameManager.Instance.AllPlayers)
            {
                if (p.PlayerHasActiveSynergy("Hearts In Halves") == true)
                {
                    return 0.25f;
                }
            }
            return 0.5f;
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


