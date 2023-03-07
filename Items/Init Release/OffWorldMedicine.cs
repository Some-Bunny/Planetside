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


namespace Planetside
{
    public class OffWorldMedicine  : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Off-World Medicine";
            //string resourceName = "Planetside/Resources/offworldmedicine.png";
            GameObject obj = new GameObject(itemName);
            OffWorldMedicine activeitem = obj.AddComponent<OffWorldMedicine>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("offworldmedicine"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "I'll Show You Off-World";
            string longDesc = "An off-world medicine from a high-tech civilzation, capable of curing even the most sturdiest addictions and festering wounds.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 5f);
            activeitem.consumable = true;
            activeitem.quality = PickupObject.ItemQuality.A;

            activeitem.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);
            activeitem.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);

            OffWorldMedicine.OffWorldMedicineID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

        }
        public static int OffWorldMedicineID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        public override void DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", base.gameObject);
            user.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Healing_Sparkles_001") as GameObject, Vector3.zero, true, false, false);
            user.healthHaver.FullHeal();
            user.spiceCount = 0;       
        }
    }
}



