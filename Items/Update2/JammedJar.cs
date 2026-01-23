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
    public class JammedJar  : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Jammed Jar";
            GameObject obj = new GameObject(itemName);
            JammedJar activeitem = obj.AddComponent<JammedJar>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("cursejar_001"), data, obj);

            string shortDesc = "Jar Full Of The Jammed";
            string longDesc = "A jar full of Jammed protective relics. Do you dare sink your hand into it?";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 1f);
            activeitem.consumable = true;
            activeitem.numberOfUses = 3;
            activeitem.quality = PickupObject.ItemQuality.C;
            JammedJar.spriteIDs = new int[4];
            JammedJar.spriteIDs[3] = data.GetSpriteIdByName("cursejar_001");//ItemAPI.SpriteBuilder.AddSpriteToCollection(JammedJar.spritePaths[0], activeitem.sprite.Collection);
            JammedJar.spriteIDs[2] = data.GetSpriteIdByName("cursejar_002");//SpriteBuilder.AddSpriteToCollection(JammedJar.spritePaths[1], activeitem.sprite.Collection);
            JammedJar.spriteIDs[1] = data.GetSpriteIdByName("cursejar_003");//SpriteBuilder.AddSpriteToCollection(JammedJar.spritePaths[2], activeitem.sprite.Collection);
            JammedJar.spriteIDs[0] = data.GetSpriteIdByName("cursejar_004");//SpriteBuilder.AddSpriteToCollection(JammedJar.spritePaths[3], activeitem.sprite.Collection);
            activeitem.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);

            JammedJar.JammedJarID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);
            GameManager.Instance.RainbowRunForceExcludedIDs.Add(activeitem.PickupObjectId);

        }
        public static int JammedJarID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        public override void DoEffect(PlayerController user)
        {
           base.sprite.SetSprite(JammedJar.spriteIDs[base.numberOfUses]);
           AkSoundEngine.PostEvent("Play_OBJ_cursepot_shatter_01", base.gameObject);
            PickupObject pickupObject = Game.Items["psog:damned_guon_stone"];
            user.AcquirePassiveItemPrefabDirectly(pickupObject as PassiveItem);
        }
        private static int[] spriteIDs;
        public static int Uses;
    }
}



