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
			string resourcePath = "Planetside/Resources/LostVoidPotential.png";
			GameObject gameObject = new GameObject(name);
			LostVoidPotential warVase = gameObject.AddComponent<LostVoidPotential>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "A Moment, Forgotten";
			string longDesc = "A shard of dimmed energy from a collapsed portal.\n\nMaybe it still has some value.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog");
			warVase.quality = PickupObject.ItemQuality.EXCLUDED;
			warVase.IgnoredByRat = true;
			warVase.RespawnsIfPitfall = true;
			warVase.UsesCustomCost = true;
			warVase.CustomCost = 40;
			LostVoidPotential.LostVoidPotentialID = warVase.PickupObjectId;
		}
		public static int LostVoidPotentialID;

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
