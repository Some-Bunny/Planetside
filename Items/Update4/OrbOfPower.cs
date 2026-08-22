using System;
using System.Collections.Generic;
using System.Linq;
using Alexandria.Integrations;
using Dungeonator;
using ItemAPI;
using UnityEngine;

namespace Planetside
{
	public class OrbOfPower : PassiveItem
	{
		public static void Init()
		{
			string name = "The Prison";
			//string resourcePath = "Planetside/Resources/orbofpower.png";
			GameObject gameObject = new GameObject(name);
			OrbOfPower warVase = gameObject.AddComponent<OrbOfPower>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("orbofpower"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Banishment";
			string longDesc = "A reminder of those you banished from the Gungeon for the rest of time.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog");
			warVase.quality = PickupObject.ItemQuality.EXCLUDED;
			warVase.IgnoredByRat = true;
			warVase.RespawnsIfPitfall = true;
			OrbOfPower.PrisonItemID = warVase.PickupObjectId;
			EncounterDatabase.GetEntry(warVase.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
			ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.Health, 1, StatModifier.ModifyMethod.ADDITIVE);
			warVase.AddItemTip("No walls hold forever.");

        }
		public static int PrisonItemID;	
	}
}
