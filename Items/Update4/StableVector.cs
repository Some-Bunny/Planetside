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
	public class StableVector : PassiveItem
	{
		public static void Init()
		{
			string itemName = "The Anchor";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<StableVector>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("stableVector"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "See Beyond";
			string longDesc = "Weakens the fabric of reality to grant consistent entry points between dimensions.\n\nHolds a small blue orb.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.A;
			item.SetupUnlockOnCustomFlag(CustomDungeonFlags.TRESPASS_INTO_OTHER_PLACE, true);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);
			AnchorID = item.PickupObjectId;
		}
		public static int AnchorID;
	}
}