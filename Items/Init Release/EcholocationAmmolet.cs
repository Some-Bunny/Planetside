using System;
using System.Collections;
using ItemAPI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
namespace Planetside
{
    public class EcholocationAmmolet : BlankModificationItem
    {
        public static void Init()
        {
            string itemName = "Echolocation Ammolet";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<EcholocationAmmolet>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("echolcationammolet"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Bat-lanks";
            string longDesc = "Using blanks uncovers the map." +
                "\n\nOriginally an Ammolet wrapped in copper, this friendly Bullat offers assistance via echolocation in exchange for the Ammolets heat.";
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.A;
            EcholocationAmmolet.EcholocationAmmoletID = item.PickupObjectId;

            item.AddToSubShop(ItemBuilder.ShopType.OldRed, 1f);
            SynergyAPI.SynergyBuilder.AddItemToSynergy(item, CustomSynergyType.MINOR_BLANKABLES);
            SynergyAPI.SynergyBuilder.AddItemToSynergy(item, CustomSynergyType.RELODESTAR);

            EcholocationAmmolet.EcholocationAmmoletID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);

        }
        public static int EcholocationAmmoletID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            //player.OnUsedBlank += this.MapOut;
        }
        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            //player.OnUsedBlank -= this.MapOut;
            return debrisObject;
        }
        private void MapOut(PlayerController player, int blanks)
        {     
 
        }

        private static Hook BlankHook = new Hook(typeof(SilencerInstance).GetMethod("ProcessBlankModificationItemAdditionalEffects", BindingFlags.Instance | BindingFlags.NonPublic), typeof(FrailtyAmmolet).GetMethod("BlankModHook", BindingFlags.Instance | BindingFlags.Public), typeof(SilencerInstance));



        public void BlankModHook(Action<SilencerInstance, BlankModificationItem, Vector2, PlayerController> orig, SilencerInstance silencer, BlankModificationItem bmi, Vector2 centerPoint, PlayerController user)
        {
            orig(silencer, bmi, centerPoint, user);
            try
            {
                if (user.HasPickupID(EcholocationAmmoletID))
                {
                    GameObject original = (GameObject)BraveResources.Load("Global VFX/VFX_Item_Pickup", typeof(GameObject), ".prefab");
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original);
                    tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                    component.PlaceAtPositionByAnchor(user.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                    component.UpdateZDepth();
                    if (Minimap.Instance != null)
                    {
                        Minimap.Instance.RevealAllRooms(true);
                    }
                }
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }
    }
}