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
    public class GunWarrant : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Gun Warrant";
            //string resourceName = "Planetside/Resources/gunwarrant.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<GunWarrant>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("gunwarrant"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "The Buy To Carry";
            string longDesc = "Although being a Gungeoneer doesn't require a warrant, the benefits of having one is hard to pass by, especially for Gungeoneers that enjoy a steady supply of new weaponry.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.A;
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:gun_warrant",
                "siren"
            };
            CustomSynergies.Add("Ultra", mandatoryConsoleIDs, null, true);
            item.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);

            GunWarrant.GunWarrantID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);
            new Hook(typeof(BaseShopController).GetMethod("DoSetup", BindingFlags.Instance | BindingFlags.NonPublic), typeof(GunWarrant).GetMethod("DoSetupHook"));

            ShopDiscountMegaMind.DiscountsToAdd.Add(new ShopDiscount()
            {
                IdentificationKey = "Gun_Warrant",
                PriceMultiplier = 0.5f,
                CanDiscountCondition = CanBuy,
                ItemToDiscount = Can
            });

            GameManager.Instance.RainbowRunForceExcludedIDs.Add(item.PickupObjectId);
        }

        public static bool Can(ShopItemController s)
        {
            if (s.item is Gun) { return true; }
            return false;
        }

        public static bool CanBuy()
        {
            foreach (var p in GameManager.Instance.AllPlayers)
            {
                if (p.HasPickupID(GunWarrant.GunWarrantID) == true)
                {
                    return true;
                }
            }

            return false;
        }

        public static int GunWarrantID;

        public static void DoSetupHook(Action<BaseShopController> orig, BaseShopController self)
        {
            orig(self);
            PlayerController[] players = GameManager.Instance.AllPlayers;
            for (int i = 0; i < players.Length; i++)
            {
                PlayerController player = players[i];
                if (player.HasPassiveItem(GunWarrantID))
                {
                    //Yes I got lazy here, fight me
                    if (self.baseShopType == BaseShopController.AdditionalShopType.NONE && self.cat == true)
                    {
                        Type type = typeof(BaseShopController); FieldInfo _property = type.GetField("m_shopItems", BindingFlags.NonPublic | BindingFlags.Instance); _property.GetValue(self);
                        List<GameObject> uses = (List<GameObject>)_property.GetValue(self);
                        Gun gun = PickupObjectDatabase.GetRandomGun();
                        GameObject gunObj = gun.gameObject;

                        uses.Add(gunObj);
                        GameObject gameObject8 = new GameObject("Shop 2 item ");
                        Transform transform4 = gameObject8.transform;

                        GameObject transObj = new GameObject();
                        transObj.transform.position = PlanetsideReflectionHelper.ReflectGetField<Transform[]>(typeof(BaseShopController), "spawnPositionsGroup2", self).Last().position + new Vector3(0, -3.125f);
                        transform4.position = transObj.transform.position;
                        transform4.parent = transObj.transform;

                        EncounterTrackable component9 = gunObj.GetComponent<EncounterTrackable>();
                        if (component9 != null)
                        {
                            GameManager.Instance.ExtantShopTrackableGuids.Add(component9.EncounterGuid);
                        }
                        GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
                        float num4 = (lastLoadedLevelDefinition == null) ? 1f : lastLoadedLevelDefinition.priceMultiplier;

                        ShopItemController shopItemController2 = gameObject8.AddComponent<ShopItemController>();

                        PlanetsideReflectionHelper.InvokeMethod(typeof(BaseShopController), "AssignItemFacing", self, new object[] { PlanetsideReflectionHelper.ReflectGetField<Transform[]>(typeof(BaseShopController), "spawnPositionsGroup2", self).Last(), shopItemController2 });

                        RoomHandler theRoom = PlanetsideReflectionHelper.ReflectGetField<RoomHandler>(typeof(BaseShopController), "m_room", self);
                        if (!theRoom.IsRegistered(shopItemController2))
                        {
                            theRoom.RegisterInteractable(shopItemController2);
                        }
                        shopItemController2.Initialize(gunObj.GetComponent<PickupObject>(), self);

                        Type typeOne = typeof(BaseShopController); FieldInfo _propertyOne = typeOne.GetField("m_itemControllers", BindingFlags.NonPublic | BindingFlags.Instance); _propertyOne.GetValue(self);
                        List<ShopItemController> listOCont = (List<ShopItemController>)_propertyOne.GetValue(self);
                        listOCont.Add(shopItemController2);

                        FieldInfo leEnabler = typeof(BaseShopController).GetField("m_shopItems", BindingFlags.Instance | BindingFlags.NonPublic);
                        leEnabler.SetValue(self, uses);
                        /*
                        List<ShopSubsidiaryZone> componentsInRoom = theRoom.GetComponentsInRoom<ShopSubsidiaryZone>();
                        for (int num5 = 0; num5 < componentsInRoom.Count; num5++)
                        {
                            if (!componentsInRoom[num5].IsShopRoundTable == false && self.cat == true)
                            {
                                componentsInRoom[num5].HandleSetup(self, theRoom, uses, listOCont);
                            }
                        }
                        */
                    }
                    if (self.baseShopType == BaseShopController.AdditionalShopType.BLACKSMITH)
                    {
                        Type type = typeof(BaseShopController); FieldInfo _property = type.GetField("m_shopItems", BindingFlags.NonPublic | BindingFlags.Instance); _property.GetValue(self);
                        List<GameObject> uses = (List<GameObject>)_property.GetValue(self);
                        Gun gun = PickupObjectDatabase.GetRandomGun();
                        GameObject gunObj = gun.gameObject;

                        uses.Add(gunObj);
                        GameObject gameObject8 = new GameObject("Shop 2 item ");
                        Transform transform4 = gameObject8.transform;

                        GameObject transObj = new GameObject();
                        transObj.transform.position = PlanetsideReflectionHelper.ReflectGetField<Transform[]>(typeof(BaseShopController), "spawnPositionsGroup2", self).Last().position + new Vector3(-3f, 2.25f);
                        transform4.position = transObj.transform.position;
                        transform4.parent = transObj.transform;

                        EncounterTrackable component9 = gunObj.GetComponent<EncounterTrackable>();
                        if (component9 != null)
                        {
                            GameManager.Instance.ExtantShopTrackableGuids.Add(component9.EncounterGuid);
                        }
                        GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
                        float num4 = (lastLoadedLevelDefinition == null) ? 1f : lastLoadedLevelDefinition.priceMultiplier;

                        ShopItemController shopItemController2 = gameObject8.AddComponent<ShopItemController>();


                        PlanetsideReflectionHelper.InvokeMethod(typeof(BaseShopController), "AssignItemFacing", self, new object[] { PlanetsideReflectionHelper.ReflectGetField<Transform[]>(typeof(BaseShopController), "spawnPositionsGroup2", self).Last(), shopItemController2 });

                        RoomHandler theRoom = PlanetsideReflectionHelper.ReflectGetField<RoomHandler>(typeof(BaseShopController), "m_room", self);
                        if (!theRoom.IsRegistered(shopItemController2))
                        {
                            theRoom.RegisterInteractable(shopItemController2);
                        }
                        shopItemController2.Initialize(gunObj.GetComponent<PickupObject>(), self);

                        Type typeOne = typeof(BaseShopController); FieldInfo _propertyOne = typeOne.GetField("m_itemControllers", BindingFlags.NonPublic | BindingFlags.Instance); _propertyOne.GetValue(self);
                        List<ShopItemController> listOCont = (List<ShopItemController>)_propertyOne.GetValue(self);
                        listOCont.Add(shopItemController2);

                        FieldInfo leEnabler = typeof(BaseShopController).GetField("m_shopItems", BindingFlags.Instance | BindingFlags.NonPublic);
                        leEnabler.SetValue(self, uses);
                    }
                }
            }


                 
        }


        private void Warrant()
        {
            //GameManager.Instance.StartCoroutine(this.GunWarrantTime(base.Owner));
        }

        private IEnumerator GunWarrantTime(PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_SND_OBJ_ammo_pickup_01", base.gameObject);
            player.InfiniteAmmo.SetOverride("Warrant", true, null);
            player.OnlyFinalProjectiles.SetOverride("Warrant", true, null);
            yield return new WaitForSeconds(7f);
            AkSoundEngine.PostEvent("Play_SND_OBJ_metalskin_end_01", base.gameObject);
            player.InfiniteAmmo.SetOverride("Warrant", false, null);
            player.OnlyFinalProjectiles.SetOverride("Warrant", false, null);
            yield break;
        }
        public override DebrisObject Drop(PlayerController player)
		{
            //player.OnEnteredCombat = (Action)Delegate.Remove(player.OnEnteredCombat, new Action(this.Warrant));
            DebrisObject result = base.Drop(player);	
			return result;
		}

		public override void Pickup(PlayerController player)
		{
            if (this.m_pickedUpThisRun != true)
            {
                AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", player.gameObject);
                for (int i = 0; i < player.inventory.AllGuns.Count; i++)
                {
                    player.inventory.AllGuns[i].GainAmmo(Mathf.FloorToInt((float)player.inventory.AllGuns[i].AdjustedMaxAmmo));
                }
            }
            //player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.Warrant));
            base.Pickup(player);
		}
	}
}


