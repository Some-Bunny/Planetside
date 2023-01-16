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
	public class NetheriteChamber : PassiveItem
	{
		public static void Init()
		{
			string name = "Netherite Chamber";
			//string resourcePath = "Planetside/Resources/brokenchamberfixedtier2.png";
			GameObject gameObject = new GameObject(name);
			NetheriteChamber chamber = gameObject.AddComponent<NetheriteChamber>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("brokenchamberfixedtier2"), data, gameObject);

            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Forged From Bullet Hells Metals";
			string longDesc = "A diamond-encrusted chamber plated with a metal unique to Bullet Hell. Its reinforced power calls upon the artifacts from planetside to reveal themselves.";
			ItemBuilder.SetupItem(chamber, shortDesc, longDesc, "psog");
			chamber.quality = PickupObject.ItemQuality.EXCLUDED;
			NetheriteChamber.ChaamberID = chamber.PickupObjectId;

		}
		public static int ChaamberID;

		protected override void OnDestroy()
        {
			base.OnDestroy();
			if (IsHoldingNetheriteChamber == true)
            {
				foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.elements)
				{
					if (ItemIDs.AllItemIDs.Contains(obj.pickupId))
					{
						obj.weight /= 100;

					}
				}
				foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.elements)
				{
					if (ItemIDs.AllItemIDs.Contains(obj.pickupId))
					{
						obj.weight /= 100;

					}
				}
				IsHoldingNetheriteChamber = false;

			}
			
		}


		public override void Pickup(PlayerController player)
		{
			IsHoldingNetheriteChamber = true;
			foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.elements)
			{
				if (ItemIDs.AllItemIDs.Contains(obj.pickupId))
				{
					obj.weight *= 100;

				}
			}
			foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.elements)
			{
				if (ItemIDs.AllItemIDs.Contains(obj.pickupId))
				{
					obj.weight *= 100;

				}
			}
			base.Pickup(player);
		}

		public override DebrisObject Drop(PlayerController player)
		{
			IsHoldingNetheriteChamber = false;
			foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.elements)
			{
				if (ItemIDs.AllItemIDs.Contains(obj.pickupId))
				{
					obj.weight /= 100;

				}
			}
			foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.elements)
			{
				if (ItemIDs.AllItemIDs.Contains(obj.pickupId))
				{
					obj.weight /= 100;

				}
			}
			DebrisObject result = base.Drop(player);
			return result;
		}
		public bool IsHoldingNetheriteChamber;	
			
		
		protected override void Update()
		{
			base.Update();
			bool flag = base.Owner != null;
			if (flag)
			{
				this.CalculateStats(base.Owner);

			}
		}
		private void CalculateStats(PlayerController player)
		{
			this.currentItems = player.passiveItems.Count;
			bool flag = this.currentItems != this.lastItems;
			if (flag)	
			{
				this.RemoveStat(PlayerStats.StatType.Health);
				this.RemoveStat(PlayerStats.StatType.Damage);
				this.RemoveStat(PlayerStats.StatType.RateOfFire);
				foreach (PassiveItem passiveItem in player.passiveItems)
				{
					bool flag2 = passiveItem is BasicStatPickup && (passiveItem as BasicStatPickup).IsMasteryToken;
					if (flag2)
					{
						this.AddStat(PlayerStats.StatType.Health, 1f, StatModifier.ModifyMethod.ADDITIVE);
						this.AddStat(PlayerStats.StatType.RateOfFire, 0.1f, StatModifier.ModifyMethod.ADDITIVE);

					}
					string encounterNameOrDisplayName = passiveItem.EncounterNameOrDisplayName;
					bool fucker = encounterNameOrDisplayName.Contains("Chamber") || encounterNameOrDisplayName.Contains("chamber");
					if (fucker)
					{
						this.AddStat(PlayerStats.StatType.Damage, .25f, StatModifier.ModifyMethod.ADDITIVE);
					}
				}

					this.lastItems = this.currentItems;
				player.stats.RecalculateStats(player, true, false);
			}
		}
		private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
		{
			StatModifier statModifier = new StatModifier
			{
				amount = amount,
				statToBoost = statType,
				modifyType = method
			};
			bool flag = this.passiveStatModifiers == null;
			if (flag)
			{
				this.passiveStatModifiers = new StatModifier[]
				{
					statModifier
				};
			}
			else
			{
				this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[]
				{
					statModifier
				}).ToArray<StatModifier>();
			}
		}
		private void RemoveStat(PlayerStats.StatType statType)
		{
			List<StatModifier> list = new List<StatModifier>();
			for (int i = 0; i < this.passiveStatModifiers.Length; i++)
			{
				bool flag = this.passiveStatModifiers[i].statToBoost != statType;
				if (flag)
				{
					list.Add(this.passiveStatModifiers[i]);
				}
			}
			this.passiveStatModifiers = list.ToArray();
		}
		private int currentItems;
		private int lastItems;
	}
}
