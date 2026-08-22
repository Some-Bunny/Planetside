using System;
using System.Collections.Generic;
using System.Linq;
using Alexandria.Integrations;
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
			GameObject gameObject = new GameObject(name);
			LostVoidPotential warVase = gameObject.AddComponent<LostVoidPotential>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("LostVoidPotential"), data, gameObject);
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
			warVase.AddItemTip("Grants coolness, +1 blank per floor, +1 active item slot and 20% ammo capacity. Can be sold for 75 casings.");

        }
		public static int LostVoidPotentialID;
	}
}
