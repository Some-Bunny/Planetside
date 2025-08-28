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
	public class LostAdventurersSword : PassiveItem
	{
		public static void Init()
		{
			string name = "Stolen Sword";
			GameObject gameObject = new GameObject(name);
            LostAdventurersSword item = gameObject.AddComponent<LostAdventurersSword>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("lost_adventurer_sword_001"), data, gameObject); 
			string shortDesc = "Not Yours To Take";
			string longDesc = "Fears enemies near you.\n\nYou feel terrible just carrying it around.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.SPECIAL;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 2f, StatModifier.ModifyMethod.ADDITIVE);
            ID = item.PickupObjectId;
		}

        public FleePlayerData data = new FleePlayerData()
        {
            StartDistance = 10,
            StopDistance = 12,
            DeathDistance = 12,
        };

        public override void Update()
        {
            base.Update();
            if (this.Owner != null && this.Owner.CurrentRoom != null)
            {
                this.Owner.CurrentRoom.ApplyActionToNearbyEnemies(this.m_owner.CenterPosition, 20, (_, __) => 
                {
                    var d = Vector2.Distance(_.sprite.WorldCenter, Owner.sprite.WorldCenter);
                    if (d <= 5.5f)
                    {
                        data.Player = Owner;
                        _.behaviorSpeculator.FleePlayerData = data;
                    }
                    else if (d > 10f && _.behaviorSpeculator.FleePlayerData == data)
                    {
                        _.behaviorSpeculator.FleePlayerData = null;
                    }
                });
            }
        }

        public static int ID;
	}
}
