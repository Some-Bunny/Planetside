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
	public class LostAdventurersBackpack : PassiveItem
	{
		public static void Init()
		{
			string name = "Stolen Backpack";
			GameObject gameObject = new GameObject(name);
            LostAdventurersBackpack item = gameObject.AddComponent<LostAdventurersBackpack>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("lost_adventurer_sack_001"), data, gameObject); 
			string shortDesc = "Stolen Goods";
			string longDesc = "Increased item capacity, holds something inside.\n\nWhat happened to him?";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.SPECIAL;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 2, StatModifier.ModifyMethod.ADDITIVE);
            ID = item.PickupObjectId;
		}
		public static int ID;
		public override void Pickup(PlayerController player)
		{
			if (!m_pickedUp)
			{
				var active = ItemTools.GetRandomActiveOfQualities(new System.Random(), new List<int>() { }, new ItemQuality[] 
				{
					ItemQuality.D,
                    ItemQuality.D,
                    ItemQuality.D,

                    ItemQuality.C,
                    ItemQuality.C,
                    ItemQuality.C,

                    ItemQuality.B,
                    ItemQuality.B,

                    ItemQuality.A,
					ItemQuality.S
				});

                var item = LootEngine.SpawnItem(active.gameObject, player.transform.position, Vector2.zero, 0, false).GetComponentInChildren<PickupObject>();
                item.Pickup(player);

            }
            base.Pickup(player);
		}
	}
}
