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
	public class TinyPlanetBullets : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Corrupt Bullets";
			//string resourceName = "Planetside/Resources/sirpaler.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<TinyPlanetBullets>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("sirpaler"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Downwards Spiral, Downwards Spiral...";
			string longDesc = "Warped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\nWarped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\n." +
				"Warped by a fracture in reality, these bullets no longer adhere to normal standards of movement.\n\n";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 1.75f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RateOfFire, 1.33f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RangeMultiplier, 5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			item.quality = PickupObject.ItemQuality.D;
			item.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);
			item.gameObject.AddComponent<RustyItemPool>();

			TinyPlanetBullets.CorruptBulletsID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);
		}
		public static int CorruptBulletsID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
				TinyPlanetMotionModule mod = new TinyPlanetMotionModule();
				mod.ForceInvert = (UnityEngine.Random.value > 0.5f) ? false : true;
				mod.OrbitTightness = UnityEngine.Random.Range(4.5f, 7.5f);
				sourceProjectile.OverrideMotionModule = mod;
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
		}
		private void PostProcessBeam(BeamController obj)
		{

			try
			{
				TinyPlanetMotionModule mod = new TinyPlanetMotionModule();
				mod.ForceInvert = (UnityEngine.Random.value > 0.5f) ? false : true;
				obj.projectile.OverrideMotionModule = mod;
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeam -= this.PostProcessBeam;
			return result;
		}
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeam += this.PostProcessBeam;
		}

		protected override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
				base.Owner.PostProcessBeam -= this.PostProcessBeam;
			}
			base.OnDestroy();
		}
	}
}