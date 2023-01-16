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
    public class Preigniter : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Weapon Pre-Igniter";
            //string resourceName = "Planetside/Resources/preigniter.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Preigniter>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("preigniter"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "They Never Saw It Coming";
            string longDesc = "Grants 3 instant reloads on room entry." +
                "\n\nBrought to the Gungeon, and clumsily lost here by the Pilot, this pre-igniter would keep any weapon system plugged into it hot and ready for action at a moments notice.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.C;
			item.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);
			Preigniter.PreigniterID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);
		}
		public static int PreigniterID;

		private int FreeReloadsLeft;

		public override DebrisObject Drop(PlayerController player)
		{
			player.OnEnteredCombat = (Action)Delegate.Remove(player.OnEnteredCombat, new Action(this.AddCooldowns));
			player.OnReloadedGun -= SkipReload;
			DebrisObject result = base.Drop(player);
			return result;
		}
		public override void Pickup(PlayerController player)
		{
			player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.AddCooldowns));
			player.OnReloadedGun += SkipReload;
			base.Pickup(player);
		}

		public void SkipReload(PlayerController player, Gun gun)
        {
			if (FreeReloadsLeft >= 0)
            {
				FreeReloadsLeft -= 1;
				player.CurrentGun.ForceImmediateReload();
				AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Player_Charge_01", base.gameObject);
				player.StartCoroutine(HandleEffects(player, gun));
			}
		}
		private IEnumerator HandleEffects(PlayerController player, Gun gun)
		{
			yield return null;
			if (gun.CurrentOwner is PlayerController)
			{
				int i = (gun.CurrentOwner as PlayerController).PlayerIDX;
				GameUIRoot.Instance.ForceClearReload(i);
			}
			yield break;
		}
		private void AddCooldowns()
        {
			FreeReloadsLeft = 2;
        }
		protected override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.OnEnteredCombat = (Action)Delegate.Remove(base.Owner.OnEnteredCombat, new Action(this.AddCooldowns));
				base.Owner.OnReloadedGun -= SkipReload;
			}
			base.OnDestroy();
		}
	}
}