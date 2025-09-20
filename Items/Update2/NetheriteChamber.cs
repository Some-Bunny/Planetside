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

		public override void OnDestroy()
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

            RemoveStat(DamageMod);
            RemoveStat(MoneyMod);
            RemoveStat(HealthMod);
            RemoveStat(RoFMod);
			lastItems = -1;
            DebrisObject result = base.Drop(player);
			return result;
		}
		public bool IsHoldingNetheriteChamber;	
			
		
		public override void Update()
		{
			base.Update();
			if (base.Owner != null)
			{
				this.CalculateStats(base.Owner);
			}
		}
		private void CalculateStats(PlayerController player)
		{
			var currentItems = player.passiveItems.Count;
			if (this.lastItems != currentItems)	
			{
                RemoveStat(DamageMod);
                RemoveStat(MoneyMod);
                RemoveStat(HealthMod);
                RemoveStat(RoFMod);

                float MasterRounds = 0;
                float Chambers = 0;
                foreach (PassiveItem passiveItem in player.passiveItems)
                {
                    if (passiveItem is BasicStatPickup && (passiveItem as BasicStatPickup).IsMasteryToken)
                    {
                        MasterRounds++;
                    }
                    string encounterNameOrDisplayName = passiveItem.EncounterNameOrDisplayName;
                    if (encounterNameOrDisplayName.ToLower().Contains("chamber"))
                    {
                        Chambers++;
                    }
                }
                if (MasterRounds > 0)
                {
                    HealthMod = this.AddStat(PlayerStats.StatType.Health, MasterRounds, StatModifier.ModifyMethod.ADDITIVE);
                    MoneyMod = this.AddStat(PlayerStats.StatType.MoneyMultiplierFromEnemies, MasterRounds / 10f, StatModifier.ModifyMethod.ADDITIVE);
                    RoFMod = this.AddStat(PlayerStats.StatType.RateOfFire, MasterRounds / 10f, StatModifier.ModifyMethod.ADDITIVE);
                }
                if (Chambers > 0)
                {
                    DamageMod = this.AddStat(PlayerStats.StatType.Damage, 0.25f * Chambers, StatModifier.ModifyMethod.ADDITIVE);
                }


                this.lastItems = currentItems;
                player.stats.RecalculateStats(player, true, false);
            }
        }
        private StatModifier AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier statModifier = new StatModifier
            {
                amount = amount,
                statToBoost = statType,
                modifyType = method
            };
            if (this.passiveStatModifiers == null)
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
            return statModifier;

        }

        private StatModifier DamageMod;
        private StatModifier MoneyMod;
        private StatModifier HealthMod;
        private StatModifier RoFMod;



        private void RemoveStat(StatModifier statType)
        {
            if (statType == null) { return; }
            List<StatModifier> list = new List<StatModifier>();
            for (int i = 0; i < this.passiveStatModifiers.Length; i++)
            {
                bool flag = this.passiveStatModifiers[i] != statType;
                if (flag)
                {
                    list.Add(this.passiveStatModifiers[i]);
                }
            }
            this.passiveStatModifiers = list.ToArray();
        }
		private int lastItems = -1;
	}
}
