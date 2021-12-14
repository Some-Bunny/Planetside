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
            string resourceName = "Planetside/Resources/gunwarrant.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<GunWarrant>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Buns Glazing";
            string longDesc = "Although being a Gungeoneer doesn't require a warrant, the benefits of having one is hard to pass by, especially for Gungeoneers that have trouble keeping their ammunition.";
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

        }
        public static int GunWarrantID;

        private void Warrant()
        {
            GameManager.Instance.StartCoroutine(this.GunWarrantTime(base.Owner));
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
            player.OnEnteredCombat = (Action)Delegate.Remove(player.OnEnteredCombat, new Action(this.Warrant));
            DebrisObject result = base.Drop(player);	
			return result;
		}

		public override void Pickup(PlayerController player)
		{
            player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.Warrant));
            base.Pickup(player);
		}
	}
}


