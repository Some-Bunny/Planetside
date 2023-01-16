using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;

namespace Planetside
{
	public class LostVoidPotential : PassiveItem
	{
		public static void Init()
		{
			string name = "Lost Potential";
			//string resourcePath = "Planetside/Resources/LostVoidPotential.png";
			GameObject gameObject = new GameObject(name);
			LostVoidPotential warVase = gameObject.AddComponent<LostVoidPotential>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("resourcePath"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "A Moment, Forgotten";
			string longDesc = "A shard of dimmed energy from a collapsed portal.\n\nMaybe it still has some value.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog");

            ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.AdditionalBlanksPerFloor, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.Coolness, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.AdditionalItemCapacity, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.AmmoCapacityMultiplier, 1.2f, StatModifier.ModifyMethod.MULTIPLICATIVE);

            warVase.quality = PickupObject.ItemQuality.EXCLUDED;
			warVase.IgnoredByRat = true;
			warVase.RespawnsIfPitfall = true;
			warVase.UsesCustomCost = true;
			warVase.CustomCost = 75;
			LostVoidPotential.LostVoidPotentialID = warVase.PickupObjectId;
			EncounterDatabase.GetEntry(warVase.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
		}
		public static int LostVoidPotentialID;
	}
}
