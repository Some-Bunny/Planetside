using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;

using UnityEngine.Serialization;
using Brave.BulletScript;

namespace Planetside
{

	//TO DO
	public class LuckyCharm : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Lucky Charm";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<LuckyCharm>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("luckycharm"), data, obj);
            string shortDesc = "You feelin' lucky punk?";
			string longDesc = "Tips the scales of fortune *slightly* in your favor in many, yet fairly small ways.\n\nA charm with a 4 leaf clover attached to it. Unsurprisingly not affiliated with the cereal brand.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.C;
			new Hook(typeof(RewardManager).GetMethod("GetDaveStyleItemQuality", BindingFlags.Instance | BindingFlags.NonPublic), typeof(LuckyCharm).GetMethod("DreamLuck"));
			new Hook(typeof(FloorRewardData).GetMethod("GetRandomBossTargetQuality", BindingFlags.Instance | BindingFlags.Public), typeof(LuckyCharm).GetMethod("DreamLuckGun"));
			new Hook(typeof(FloorRewardData).GetMethod("GetShopTargetQuality", BindingFlags.Instance | BindingFlags.Public), typeof(LuckyCharm).GetMethod("DreamLuckShops"));
			new Hook(typeof(Chest).GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic), typeof(LuckyCharm).GetMethod("DisableFuses"));
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Coolness, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.AddToSubShop(ItemAPI.ItemBuilder.ShopType.Cursula, 1f);


            CustomFloorRewardData = new FloorRewardData();

			//Average Money MAX
			CustomFloorRewardData.AverageCurrencyDropsThisFloor = 60f;
			//Average Deviation
			CustomFloorRewardData.CurrencyDropsStandardDeviation = 15f;
			//Average Money MIN
			CustomFloorRewardData.MinimumCurrencyDropsThisFloor = 40f;

			CustomFloorRewardData.D_Chest_Chance = 0.1f;
			CustomFloorRewardData.C_Chest_Chance = 0.25f;
			CustomFloorRewardData.B_Chest_Chance = 0.225f;
			CustomFloorRewardData.A_Chest_Chance = 0.225f;
			CustomFloorRewardData.S_Chest_Chance = 0.2f;

			CustomFloorRewardData.ChestSystem_ChestChanceLowerBound = 0.01f;
			CustomFloorRewardData.ChestSystem_ChestChanceUpperBound = 0.2f;
			CustomFloorRewardData.ChestSystem_Increment = 0.03f;

			CustomFloorRewardData.GunVersusItemPercentChance = 0.5f;
			//Chest Drop Room Clear Chances
			CustomFloorRewardData.PercentOfRoomClearRewardsThatAreChests = 0.24f;
			//Ammo Drop Room Clear Chances

			CustomFloorRewardData.FloorChanceToDropAmmo = 0.075f;
			//Spread Ammo Drop Floor Chances
			CustomFloorRewardData.FloorChanceForSpreadAmmo = 0.625f;
			//Chest Chances
			CustomFloorRewardData.D_RoomChest_Chance = 0.05f;
			CustomFloorRewardData.C_RoomChest_Chance = 0.25f;
			CustomFloorRewardData.B_RoomChest_Chance = 0.25f;
			CustomFloorRewardData.A_RoomChest_Chance = 0.25f;
			CustomFloorRewardData.S_RoomChest_Chance = 0.2f;
			//Boss Gun Chances
			//CustomFloorRewardData.D_BossGun_Chance = 0f;
			//CustomFloorRewardData.C_BossGun_Chance = 0.35f;
			//CustomFloorRewardData.B_BossGun_Chance = 0.3f;
			//CustomFloorRewardData.A_BossGun_Chance = 0.25f;
			//CustomFloorRewardData.S_BossGun_Chance = 0.1f;

			CustomFloorRewardData.D_BossGun_Chance = 0.01f;
			CustomFloorRewardData.C_BossGun_Chance = 0.01f;
			CustomFloorRewardData.B_BossGun_Chance = 0.01f;
			CustomFloorRewardData.A_BossGun_Chance = 0.01f;
			CustomFloorRewardData.S_BossGun_Chance = 0.95f;

			//Shop Items
			CustomFloorRewardData.D_Shop_Chance = 0f;
			CustomFloorRewardData.C_Shop_Chance = 0.35f;
			CustomFloorRewardData.B_Shop_Chance = 0.30f;
			CustomFloorRewardData.A_Shop_Chance = 0.25f;
			CustomFloorRewardData.S_Shop_Chance = 0.1f;
			//Something
			CustomFloorRewardData.ReplaceFirstRewardWithPickup = 0.2f;
			//Chest Chances agani
			CustomFloorRewardData.D_Item_Chest_Chance = 0.1f;
			CustomFloorRewardData.C_Item_Chest_Chance = 0.25f;
			CustomFloorRewardData.B_Item_Chest_Chance = 0.25f;
			CustomFloorRewardData.A_Item_Chest_Chance = 0.2f;
			CustomFloorRewardData.S_Item_Chest_Chance = 0.2f;

			List<string> mandatoryConsoleIDs = new List<string>
			{
				"psog:lucky_charm",
				"seven_leaf_clover"
			};		
			CustomSynergies.Add("1 in 177 Billion", mandatoryConsoleIDs, null, true);
		}

		public static FloorRewardData CustomFloorRewardData;

		public static void DisableFuses(Action<Chest> orig, Chest self)
		{
			if (IsLucky == true)
            {
				self.PreventFuse = true;
            }
			orig(self);
		}
		public static PickupObject.ItemQuality DreamLuck(Func<RewardManager, PickupObject.ItemQuality> orig, RewardManager self)
		{
			if (IsLucky == true)
            {
				bool isFav = PlayerHasFavouritism();

				float num = 0f;
				float num2 = isFav == false ? 0.35f : 0.15f;
				float num3 = isFav == false ? 0.65f : 0.5f;
				float num4 = isFav == false ? 0.9f : 0.8f;
				float value = UnityEngine.Random.value;
				PickupObject.ItemQuality result = PickupObject.ItemQuality.D;
				if (value > num && value <= num2)
				{
					result = PickupObject.ItemQuality.C;
				}
				else if (value > num2 && value <= num3)
				{
					result = PickupObject.ItemQuality.B;
				}
				else if (value > num3 && value <= num4)
				{
					result = PickupObject.ItemQuality.A;
				}
				else if (value > num4)
				{
					result = PickupObject.ItemQuality.S;
				}
				return result;
			}
			return orig(self);
		}

		public static bool PlayerHasFavouritism()
		{
			foreach (PlayerController player in GameManager.Instance.AllPlayers)
			{
				if (player.PlayerHasActiveSynergy("1 in 177 Billion")) { return true; }
			}
			return false;
		}

		public static PickupObject.ItemQuality DreamLuckGun(Func<FloorRewardData, System.Random, PickupObject.ItemQuality> orig, FloorRewardData self, System.Random safeRandom)
		{
			if (IsLucky == true)
            {
				float num = self.SumBossGunChances();
				float fran = ((safeRandom == null) ? UnityEngine.Random.value : ((float)safeRandom.NextDouble())) * num;

				bool isFav = PlayerHasFavouritism();
				PickupObject.ItemQuality targetQualityFromChances = isFav == false ? self.GetTargetQualityFromChances(fran, 0, 0.35f, 0.3f, 0.25f, 0.1f, false) : self.GetTargetQualityFromChances(fran, 0, 0.15f, 0.35f, 0.3f, 0.2f, false);
				Debug.Log(targetQualityFromChances + " <= boss quality");
				return targetQualityFromChances;
			}
			return orig(self, safeRandom);		
		}
		public static PickupObject.ItemQuality DreamLuckShops(Func<FloorRewardData, bool, PickupObject.ItemQuality> orig, FloorRewardData self, bool useSeedRandom)
		{
			if (IsLucky == true)
			{
				bool isFav = PlayerHasFavouritism();
				float num = self.SumShopChances();
				float fran = ((!useSeedRandom) ? UnityEngine.Random.value : BraveRandom.GenerationRandomValue()) * num;
				return isFav == false ? self.GetTargetQualityFromChances(fran, 0, 0.35f, 0.3f, 0.25f, 0.1f, false) : self.GetTargetQualityFromChances(fran, 0, 0.15f, 0.35f, 0.3f, 0.2f, false);
			}
			return orig(self, useSeedRandom);
		}
		public static bool IsLucky;




		public override void Update()
        {
			base.Update();
			if (base.Owner != null && base.Owner.CurrentRoom != null)
			{
				List<AIActor> activeEnemies = base.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				if (activeEnemies != null && activeEnemies.Count > 0)
				{
					foreach (AIActor enemy in activeEnemies)
                    {
						if (enemy != null) { enemy.HasDamagedPlayer = false; }
					}
				}
			}
        }
		public override DebrisObject Drop(PlayerController player)
		{
			IsLucky = false;
			DebrisObject result = base.Drop(player);
			return result;
		}
		public override void Pickup(PlayerController player)
		{
			IsLucky = true;
			base.Pickup(player);
		}

		public override void OnDestroy()
		{
			IsLucky = false;
			if (base.Owner != null)
            {
				base.OnDestroy();
			}
		}
	}
}