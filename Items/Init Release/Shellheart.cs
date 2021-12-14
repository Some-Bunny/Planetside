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
using SaveAPI;


namespace Planetside
{
	public class Shellheart : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Shell-Heart";
			string resourceName = "Planetside/Resources/shellheart.png";
			GameObject obj = new GameObject(itemName);
			Shellheart counterChamber = obj.AddComponent<Shellheart>();
			ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
			string shortDesc = "Lead-Lining";
			string longDesc = "Armor damage no longer prevents Mastery.\n\nA hollow heart decorated with hundreds of spent shells. A guarding presence surrounds you.";
			counterChamber.SetupItem(shortDesc, longDesc, "psog");
			counterChamber.quality = PickupObject.ItemQuality.B;
			counterChamber.SetupUnlockOnCustomFlag(CustomDungeonFlags.SHELLRAX_DEFEATED, true);
			counterChamber.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);


			Shellheart.ShellHeartID = counterChamber.PickupObjectId;
			ItemIDs.AddToList(counterChamber.PickupObjectId);

		}
		public static int ShellHeartID;

		private void ModifyIncomingDamage(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
		{
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			if (args == EventArgs.Empty)
			{
				return;
			}
			if (player.healthHaver.Armor > 0f)
			{
				GameManager.Instance.StartCoroutine(this.SaveFlawless());
			}
			AkSoundEngine.PostEvent("Play_ENM_shells_gather_01", base.gameObject);
			player.inventory.CurrentGun.GainAmmo(Mathf.FloorToInt((float)player.inventory.CurrentGun.AdjustedMaxAmmo * 0.25f));
		}
		private IEnumerator SaveFlawless()
		{
			yield return new WaitForSeconds(0.1f);
			PlayerController player = base.Owner;
			bool flag = player.CurrentRoom != null;
			if (flag)
			{
				player.CurrentRoom.PlayerHasTakenDamageInThisRoom = false;
			}
			yield break;
		}
		public GameObject OnIgnoredDamageVFX;

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			HealthHaver healthHaver = player.healthHaver;
			healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Combine(healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));
			base.Pickup(player);
		}
		public override DebrisObject Drop(PlayerController player)
		{
			HealthHaver healthHaver = player.healthHaver;
			healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Remove(healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));
			return base.Drop(player);
		}
		protected override void OnDestroy()
		{
			if (base.Owner != null)
			{
				HealthHaver healthHaver = base.Owner.healthHaver;
				healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Remove(healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));
			}
			base.OnDestroy();
		}
	}
}