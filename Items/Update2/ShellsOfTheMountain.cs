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
	public class ShellsOfTheMountain : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Shells Of The Mountain";
			//string resourceName = "Planetside/Resources/bulletofthemountain.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<ShellsOfTheMountain>();

            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("bulletofthemountain"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Forged For The Mountain King";
			string longDesc = "Increased Challenge, Increased Reward.\nNo matter how steep the challenge, these rounds climb to the top to stay level with their foes.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.A;
			ShellsOfTheMountain.ShellsOfTheMountainID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);
            new Hook(typeof(AIActor).GetMethod("OnPlayerEntered", BindingFlags.Instance | BindingFlags.NonPublic),
				typeof(ShellsOfTheMountain).GetMethod("OnPlayerEnteredHook"));
            new Hook(typeof(BehaviorSpeculator).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(ShellsOfTheMountain).GetMethod("StartHookBehaviorSpeculator"));
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 1.25f, StatModifier.ModifyMethod.MULTIPLICATIVE);

            Alexandria.RoomRewardAPI.OnRoomRewardDetermineContents += ORDC;
        }

        public static void ORDC(RoomHandler room, Alexandria.RoomRewardAPI.ValidRoomRewardContents validRoomRewardContents, float chance)
        {
            if (CanElite() == true && validRoomRewardContents != null)
            {
                validRoomRewardContents.additionalRewardChance -= 0.1f;
            }
        }

        public static bool CanElite()
        {
            foreach (var p in GameManager.Instance.AllPlayers)
            {
                if (p.HasPickupID(ShellsOfTheMountain.ShellsOfTheMountainID) == true)
                {
                    return true;
                }
            }
            return false;
        }
        public static void OnPlayerEnteredHook(Action<AIActor, PlayerController> orig, AIActor self, PlayerController player)
        {
            orig(self, player);
            if (self != null && CanElite() == true && self.aiActor != null && !OuroborosController.EliteBlackListDefault.Contains(self.aiActor.EnemyGuid) && OuroborosController.EnemyIsValid(self.aiActor) == true)
            {
                bool BossCheck = self.aiActor.healthHaver.IsBoss | self.aiActor.healthHaver.IsSubboss;
                float specialChance = UnityEngine.Random.Range(0f, 1.0f);
                if (specialChance < 0.35f | BossCheck == true)
                {
                    if (UnityEngine.Random.value <= 0.2f && BossCheck == false)
                    {
                        var SpecialElite = OuroborosController.specialEliteTypes[UnityEngine.Random.Range(0, OuroborosController.specialEliteTypes.Count)];
                        if (self.aiActor.gameObject.GetComponent(SpecialElite) == null)
                        {
                            self.aiActor.gameObject.AddComponent(SpecialElite);
                        }
                    }
                    else
                    {
                        var elite = OuroborosController.basicEliteTypes[UnityEngine.Random.Range(0, OuroborosController.basicEliteTypes.Count)];
                        if (self.aiActor.gameObject.GetComponent(elite) == null)
                        {
                            self.aiActor.gameObject.AddComponent(elite);
                        }
                    }
                }
            }
        }


        public static void StartHookBehaviorSpeculator(Action<BehaviorSpeculator> orig, BehaviorSpeculator self)
        {
            orig(self);
            if (self != null && CanElite() == true && self.aiActor != null && !OuroborosController.EliteBlackListDefault.Contains(self.aiActor.EnemyGuid) && OuroborosController.EnemyIsValid(self.aiActor) == true)
            {
                bool BossCheck = self.aiActor.healthHaver.IsBoss | self.aiActor.healthHaver.IsSubboss;
                float specialChance = UnityEngine.Random.Range(0f, 1.0f);
                if (specialChance < 0.35f | BossCheck == true)
                {
                    if (UnityEngine.Random.value <= 0.2f && BossCheck == false)
                    {
                        var SpecialElite = OuroborosController.specialEliteTypes[UnityEngine.Random.Range(0, OuroborosController.specialEliteTypes.Count)];
                        if (self.aiActor.gameObject.GetComponent(SpecialElite) == null)
                        {
                            self.aiActor.gameObject.AddComponent(SpecialElite);
                        }
                    }
                    else
                    {
                        var elite = OuroborosController.basicEliteTypes[UnityEngine.Random.Range(0, OuroborosController.basicEliteTypes.Count)];
                        if (self.aiActor.gameObject.GetComponent(elite) == null)
                        {
                            self.aiActor.gameObject.AddComponent(elite);
                        }
                    }
                }
            }
        }



        public static int ShellsOfTheMountainID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
				float num = 1f;
				GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
				if (lastLoadedLevelDefinition != null)
				{
					num = lastLoadedLevelDefinition.enemyHealthMultiplier - 1;
				}
				sourceProjectile.baseData.damage = sourceProjectile.baseData.damage*((num/3)+1);

            }
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
		}
		private void PostProcessBeam(BeamController obj)
		{
			try
			{
				float num = 1f;
				GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
				if (lastLoadedLevelDefinition != null)
				{
					num = lastLoadedLevelDefinition.enemyHealthMultiplier - 1;
				}
				obj.projectile.baseData.damage = obj.projectile.baseData.damage * ((num / 3) + 1);
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeam -= this.PostProcessBeam;
			return result;
		}
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeam += this.PostProcessBeam;
		}

		public override void OnDestroy()
		{
			if (base.Owner)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
				base.Owner.PostProcessBeam -= this.PostProcessBeam;
			}
			base.OnDestroy();
		}
	}
}