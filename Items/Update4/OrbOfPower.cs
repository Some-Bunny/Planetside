using System;
using System.Collections.Generic;
using System.Linq;
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
			string resourcePath = "Planetside/Resources/orbofpower.png";
			GameObject gameObject = new GameObject(name);
			OrbOfPower warVase = gameObject.AddComponent<OrbOfPower>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Banishment";
			string longDesc = "A reminder of those you banished from the Gungeon, for the rest of eternity.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog");
			warVase.quality = PickupObject.ItemQuality.EXCLUDED;
			warVase.IgnoredByRat = true;
			warVase.RespawnsIfPitfall = true;
			OrbOfPower.PrisonItemID = warVase.PickupObjectId;
			EncounterDatabase.GetEntry(warVase.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
			ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.Health, 1, StatModifier.ModifyMethod.ADDITIVE);

		}
		public static int PrisonItemID;

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			return result;
		}		
	}
}
