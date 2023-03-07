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
using Brave.BulletScript;
using SaveAPI;

namespace Planetside
{

	public class GunClassToken : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Favouritism";
			//string resourceName = "Planetside/Resources/favouritism.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<GunClassToken>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("favouritism"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Gee, Why Do You Get Two?";
			string longDesc = "Increases rate of fire. Adds favouritism to any weapons you may find.\n\nA necklace made my someone who really, *really* preferred a certain type of weaponry.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.D;
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RateOfFire, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			GunClassTokenID = item.PickupObjectId;
			new Hook(typeof(LootDataGlobalSettings).GetMethod("GetModifierForClass", BindingFlags.Instance | BindingFlags.Public), typeof(GunClassToken).GetMethod("GetModifierForClassHook", BindingFlags.Static | BindingFlags.Public));
		}
		public static int GunClassTokenID;
		public static float GetModifierForClassHook(Func<LootDataGlobalSettings, GunClass, float> orig, LootDataGlobalSettings self, GunClass gunClass)
        {
			float f = orig(self,gunClass);
			if (PlayerHasFavouritism() == true) { f = 5f; }
			return f;
        }
		public static bool PlayerHasFavouritism()
		{
			foreach (PlayerController player in GameManager.Instance.AllPlayers)
			{
				if (player.HasPickupID(GunClassToken.GunClassTokenID)) { return true; }
			}
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
		public override void OnDestroy()
		{
			base.OnDestroy();
		}
	}
}