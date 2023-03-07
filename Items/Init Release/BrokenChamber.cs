using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Collections;
using SaveAPI;
using Brave.BulletScript;

namespace Planetside
{
	// Token: 0x02000035 RID: 53
	public class BrokenChamber : PassiveItem
	{
		public static void Init()
		{
			string name = "Broken Chamber";
			GameObject gameObject = new GameObject(name);
			BrokenChamber warVase = gameObject.AddComponent<BrokenChamber>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("brokenchamber"), data, gameObject); 
			
			string shortDesc = "A bad omen looms...";
			string longDesc = "A broken, forgotten chamber. Dark power seeps from it.\n\nYou feel uneasy carrying it, yet you feel like you must keep it, and take it very, very far.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog");
			warVase.quality = PickupObject.ItemQuality.EXCLUDED;
			warVase.CanBeDropped = false;
			BrokenChamber.BrokenChamberID = warVase.PickupObjectId;
		}
		public static int BrokenChamberID;
		public override void Pickup(PlayerController player)
		{
			BrokenChamberComponent Values = player.gameObject.AddComponent<BrokenChamberComponent>();
			Values.TimeBetweenRockFalls = 9f;
			Values.player = player;
			base.Pickup(player);
		}
		public override void OnDestroy()
		{
			if (base.Owner != null)
            {
				PlayerController player = base.Owner;
				BrokenChamberComponent Values = player.gameObject.GetComponent<BrokenChamberComponent>();
				Destroy(Values);
			}
			base.OnDestroy();
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			return result;
		}
	}
}
