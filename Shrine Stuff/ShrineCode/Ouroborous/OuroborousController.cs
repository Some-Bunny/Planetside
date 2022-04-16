using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dungeonator;
using Gungeon;
using GungeonAPI;
using ItemAPI;
using UnityEngine;
using SaveAPI;
using Brave.BulletScript;

namespace Planetside
{

	public class Ouroborous : ETGModule
	{

		public static bool LoopingOn;
		bool disabled = false;


		public override void Exit()
		{
		}

		public override void Start()
		{
			try
			{
				this.CreateOrLoadConfiguration();
			}
			catch (Exception e)
			{
				Tools.PrintException(e, "FF0000");
				ETGModConsole.Log("Shits fucked, man");
			}
		}

		private void CreateOrLoadConfiguration()
		{
			bool flag = !File.Exists(SaveFilePath);
			if (flag)
			{
				global::ETGModConsole.Log("", false);
				Directory.CreateDirectory(ConfigDirectory);
				File.Create(SaveFilePath).Close();
				UpdateConfiguration();
			}
			else
			{
				string text = File.ReadAllText(SaveFilePath);
				bool flag2 = !string.IsNullOrEmpty(text);
				if (flag2)
				{
					File.WriteAllText(SaveFilePath, EnableLooping);
				}
				else
				{
					this.UpdateConfiguration();
				}
			}
		}
		public void UpdateConfiguration()
		{
			bool flag = !File.Exists(SaveFilePath);
			if (flag)
			{
				global::ETGModConsole.Log("", false);
				Directory.CreateDirectory(ConfigDirectory);
				File.Create(SaveFilePath).Close();
			}
			File.WriteAllText(SaveFilePath, EnableLooping);
		}
		private static string ConfigDirectory = Path.Combine(global::ETGMod.ResourcesDirectory, "psogconfig");
		public static string SaveFilePath = Path.Combine(ConfigDirectory, "loopingenabled.json");
		private static string EnableLooping = LoopingOn ? "true" : "false";
		public override void Init()
		{
			if (File.Exists(SaveFilePath))
			{
				string[] lines = File.ReadAllLines(SaveFilePath);
				if (lines.Contains("true"))
				{

					LoopingOn = true;

				}
				else
				{
					LoopingOn = false;
				}
			}
			else
			{
				LoopingOn = false;

			}
			global::ETGModConsole.Commands.AddGroup("psog", delegate (string[] args)
			{
				global::ETGModConsole.Log("Please specify a command.", false);
			});
			global::ETGModConsole.Commands.GetGroup("psog").AddUnit("toggleloops", delegate (string[] args)
			{
				bool LoopOn = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.LOOPING_ON);
				if (LoopOn == true)
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.LOOPING_ON, false);
					global::ETGModConsole.Log("Ouroborous Disabled.", false);
				}
				else
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.LOOPING_ON, true);
					float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
					global::ETGModConsole.Log("Ouroborous set to: " + Loop, false);
				}
			});
			global::ETGModConsole.Commands.GetGroup("psog").AddUnit("unlock_all", delegate (string[] args)
			{
				ETGModConsole.Log("<size=100><color=#ff0000ff>*Hits locks with oversized hammer*</color></size>", false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.SHELLRAX_DEFEATED, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BULLETBANK_DEFEATED, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_LOOP_1, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_FUNGANNON, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_OPHANAIM, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HAS_COMPLETED_SOMETHING_WICKED, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.TRESPASS_INTO_OTHER_PLACE, true);
				AdvancedGameStatsManager.Instance.SetStat(CustomTrackedStats.UMBRAL_ENEMIES_KILLED, 10);
				AdvancedGameStatsManager.Instance.SetStat(CustomTrackedStats.JAMMED_ARCHGUNJURERS_KILLED, 20);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED_T4, true);
			});
			global::ETGModConsole.Commands.GetGroup("psog").AddUnit("surface", delegate (string[] args)
			{
				ETGModConsole.Log("<size=100><color=#ff0000ff>*Resurfacing...*</color></size>", false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HAS_TREADED_DEEPER, false);
			});

			global::ETGModConsole.Commands.GetGroup("psog").AddUnit("lock_all", delegate (string[] args)
			{
				ETGModConsole.Log("<size=100><color=#ff0000ff>Refitting the locks... don't hit them that hard next time, okay?</color></size>", false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.SHELLRAX_DEFEATED, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BULLETBANK_DEFEATED, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_LOOP_1, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_FUNGANNON, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_OPHANAIM, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HAS_COMPLETED_SOMETHING_WICKED, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.TRESPASS_INTO_OTHER_PLACE, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED_T4, false);
				AdvancedGameStatsManager.Instance.SetStat(CustomTrackedStats.UMBRAL_ENEMIES_KILLED, 0);
				AdvancedGameStatsManager.Instance.SetStat(CustomTrackedStats.JAMMED_ARCHGUNJURERS_KILLED, 0);



				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEJAM, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEPETRIFY, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEDARKEN, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEBOLSTER, false);

			});

			ETGModConsole.Commands.GetGroup("psog").AddUnit("reset_loop", delegate (string[] args)
			{
				float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
				SaveAPIManager.RegisterStatChange(CustomTrackedStats.TIMES_LOOPED, Loop - (Loop*2));
				ETGModConsole.Log("Current Loop: " + SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED).ToString());
			});
			
			ETGModConsole.Commands.GetGroup("psog").AddUnit("set_loop", delegate (string[] args)
			{
				float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
				SaveAPIManager.RegisterStatChange(CustomTrackedStats.TIMES_LOOPED, float.Parse(args[0]) - Loop);
				ETGModConsole.Log("Current Loop: " + SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED).ToString());
			});
			ETGModConsole.Commands.GetGroup("psog").AddUnit("current_loop", delegate (string[] args)
			{
				ETGModConsole.Log("Current Loop: " + SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED).ToString());
			});


			global::ETGModConsole.Commands.GetGroup("psog").AddUnit("to_do_list", delegate (string[] args)
			{
				string a = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED) ? " Done!\n" : " -Defeat The Dragun At A Higher Curse.\n";
				string b = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED) ? " Done!\n" : " -Defeat The Guardian Of The Holy Chamber.\n";
				string c = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.SHELLRAX_DEFEATED) ? " Done!\n" : " -Defeat The Failed Demi-Lich\n";
				string d = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BULLETBANK_DEFEATED) ? " Done!\n" : " -Defeat The Banker Of Bullets.\n";
				string e = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED) ? " Done!\n" : " -Defeat The Lich With A Broken Remnant In Hand.\n";
				string f = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_LOOP_1) ? " Done!\n" : " -Beat The Game On Ouroborous Level 0.\n";
				string g = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND) ? " Done!\n" : " -Kill A Boss After Dealing 500 Damage Or More At Once.\n";
				string h = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEFEAT_FUNGANNON) ? " Done!\n" : " -Defeat The Fungal Beast Of The Sewers.\n";
				string i = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEFEAT_OPHANAIM) ? " Done!\n" : " -Defeat The Eternal Eye Of The Abbey.\n";
				string j = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER) ? " Done!\n" : " -Defeat A Ravenous, Violent Chamber.\n";
				string k = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK) ? " Done!\n" : " -Remove Each Hell-Bound Curse At Least Once.\n";
				string l = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HAS_COMPLETED_SOMETHING_WICKED) ? " Done!\n" : " -Survive An Encounter With Something Wicked.\n";
				string m = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.TRESPASS_INTO_OTHER_PLACE) ? " Done!\n" : " -Trespass Into Somewhere Else.\n";
				string n = AdvancedGameStatsManager.Instance.GetPlayerStatValue(CustomTrackedStats.UMBRAL_ENEMIES_KILLED) >= 4 ? " Done!\n" : " -Slay 5 Umbral Enemies.\n";
				string o = AdvancedGameStatsManager.Instance.GetPlayerStatValue(CustomTrackedStats.JAMMED_ARCHGUNJURERS_KILLED) >= 14 ? " Done!\n" : " -Defeat 15 Jammed Arch Gunjurers.\n";
				string p = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED_T4) ? " Done!\n" : " -Perform the Highest Level Maintenance On The Damaged Robot.\n";

				string color1 = "9006FF";
				OtherTools.PrintNoID("Unlock List:\n" + a + b + c + d + e + f + g + h+i+j+k+l+m+n+o+p, color1);
			});

			ETGModConsole.Commands.GetGroup("psog").AddUnit("help", delegate (string[] args)
			{
				string color1 = "9006FF";
				OtherTools.PrintNoID("List Of Commands:", color1);
				OtherTools.PrintNoID("=========.", color1);
				OtherTools.PrintNoID("psog toggleloops" + ": Enables/Disables Ouroborous mode.", color1);
				OtherTools.PrintNoID("psog current_loop" + ": Displays the current player loop.", color1);
				OtherTools.PrintNoID("psog set_loop" + ": Sets the current loop to a given number.", color1);
				OtherTools.PrintNoID("psog reset_loop" + ": Sets the loop to 0.", color1);
				OtherTools.PrintNoID("=========.", color1);
				OtherTools.PrintNoID("psog to_do_list" + ": Displays all unlock conditions.", color1);
				OtherTools.PrintNoID("psog lock_all" + ": Forces all Planetside unlocks to be locked.", color1);
				OtherTools.PrintNoID("psog unlock_all" + ": Forces all Planetside unlocks to be unlocked.", color1);
				OtherTools.PrintNoID("=========.", color1);

				OtherTools.PrintNoID("psog set_item_weight" + ": Changes how often Planetside items and guns appear to a given value.", color1);

			});

			global::ETGModConsole.Commands.GetGroup("psog").AddUnit("uitoggle", delegate (string[] args)
			{
				if (!disabled)
				{
					ETGModConsole.Log("Ui is disabled");
					GameUIRoot.Instance.HideCoreUI("disabled");
					GameUIRoot.Instance.ForceHideGunPanel = true;
					GameUIRoot.Instance.ForceHideItemPanel = true;
					disabled = true;
				}
				else if (disabled)
				{
					ETGModConsole.Log("Ui is enabled");
					GameUIRoot.Instance.ShowCoreUI("disabled");
					GameUIRoot.Instance.ForceHideGunPanel = false;
					GameUIRoot.Instance.ForceHideItemPanel = false;
					disabled = false;
				}
			});

		}
		private void AssignUnlocks(AIActor target)
		{
			//Lich Kill unlocks
			
		}
		public float AddedMasterRoundChance;
		public Color magenta = Color.magenta;
		public Color yellow = Color.yellow;
		public Color cyan = Color.cyan;
		public static List<AIActor> LoopEffects = new List<AIActor>();
		private void LoopScale(AIActor target)
		{
			float value = UnityEngine.Random.value;
			bool LoopOn = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.LOOPING_ON);
			if (LoopOn && !target.CompanionOwner && !BannedEnemies.Contains(target.EnemyGuid))
			{
				if (!LoopOn)
				{

				}
				else
				{
					LoopEffectsEnemy(target);
					LoopEffects.Add(target);
				}
			}
		}

		private void LoopEffectsEnemy(AIActor target)
		{
			if (!Ouroborous.BannedEnemies.Contains(target.EnemyGuid))
            {
				float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
				float DownScaler = Loop / 50f;
				
				if (Loop == 50 || Loop >= 50)
                {
					target.MovementSpeed *= 3f + ((Loop / 33f)) - DownScaler;
				}
				else
                {
					target.MovementSpeed *= 1 + ((Loop / 25f))-DownScaler;
				}
				target.healthHaver.SetHealthMaximum(target.healthHaver.GetMaxHealth() * ((1 + Loop/10f))- DownScaler);
				target.knockbackDoer.weight *= 1 + ((Loop / 1.5f))- -DownScaler;
				target.behaviorSpeculator.CooldownScale *= ((1f+(Loop/2f)))-DownScaler;
				//target.behaviorSpeculator.CooldownScale *= 0;
				float random = UnityEngine.Random.Range(0.0f, 1.0f);
				if (random <= Loop/15)
				{
					target.healthHaver.spawnBulletScript = true;
					target.healthHaver.chanceToSpawnBulletScript = 1f;
					target.healthHaver.bulletScriptType = HealthHaver.BulletScriptType.OnPreDeath;
					target.healthHaver.bulletScript = new CustomBulletScriptSelector(typeof(ExplosiveDeath));
				}
			}
		}
		private void fuckYOU(Projectile proj)
        {

			List<string> log = new List<string>()
			{


			};
			foreach (Component fuck in proj.GetComponents(typeof(Component)))
            {
				log.Add("Componenet: " + fuck.GetType().Name);
				log.Add("----------");
            }
			var Hatred = string.Join("\n", log.ToArray());
			ETGModConsole.Log(Hatred);
        }

		public static void TrappedChest(Action<Chest, PlayerController> orig, Chest self, PlayerController player)
		{
			orig(self, player);
            {
				bool LoopOn = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.LOOPING_ON);
				if (LoopOn == true)
                {
					float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
					if (Loop == 10 || Loop >= 10)
                    {
						int num3 = UnityEngine.Random.Range(0, 5);
						bool ItsATrap = num3 == 1;
						if (ItsATrap)
						{
							GameObject gameObject = new GameObject();
							gameObject.transform.position = self.transform.position + new Vector3(0, -1f, 0f);
							BulletScriptSource source = gameObject.GetOrAddComponent<BulletScriptSource>();
							gameObject.AddComponent<BulletSourceKiller>();
							var bulletScriptSelected = new CustomBulletScriptSelector(typeof(GrenadeYahyeet));
							AIActor aIActor = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
							AIBulletBank bulletBank = aIActor.GetComponent<AIBulletBank>();
							bulletBank.CollidesWithEnemies = true;
							source.BulletManager = bulletBank;
							source.BulletScript = bulletScriptSelected;
							source.Initialize();//to fire the script once
						}
					}
					else
                    {
						int num3 = (int)UnityEngine.Random.Range(0, 15 - (Loop));
						bool ItsATrap = num3 == 1;
						if (ItsATrap)
						{
							
							GameObject gameObject = new GameObject();
							gameObject.transform.position = self.transform.position + new Vector3(0, -1f, 0f);
							BulletScriptSource source = gameObject.GetOrAddComponent<BulletScriptSource>();
							gameObject.AddComponent<BulletSourceKiller>();
							var bulletScriptSelected = new CustomBulletScriptSelector(typeof(GrenadeYahyeet));
							AIActor aIActor = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
							AIBulletBank bulletBank = aIActor.GetComponent<AIBulletBank>();
							bulletBank.CollidesWithEnemies = true;
							source.BulletManager = bulletBank;
							source.BulletScript = bulletScriptSelected;
							source.Initialize();//to fire the script once

							
						}
					}
				}
            }
		}

		

		public static void DoFairy(Action<MinorBreakable> orig, MinorBreakable self)
		{
			orig(self);
			if (self != null)
            {
				for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
				{
					PlayerController player = GameManager.Instance.AllPlayers[i];
					if (player.HasPickupID(GildedPots.GildedPotsID) && player != null && self != null)
					{
						float coinchance = 0.04f;
						bool flagA = player.PlayerHasActiveSynergy("Expert Demolitionist");
						if (flagA)
						{
							coinchance *= 2;
						}
						float num = UnityEngine.Random.Range(0f, 1f);
						bool flag2 = (double)num < coinchance;
						if (flag2)
						{
							LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, self.transform.PositionVector2(), Vector2.zero, 1f, false, false, false);
						}
					}

					bool LoopOn = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.LOOPING_ON);
					if (LoopOn == true)
					{
						float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
						int GoopScaler;
						GoopScaler = (int)UnityEngine.Random.Range(0, 25 - Loop);
						if (GoopScaler == 1)
						{
							Ouroborous yes = new Ouroborous();
							bool bankName = (UnityEngine.Random.value > 0.50f) ? true : false;
							if (bankName == true && self != null)
							{
								yes.DoPoisonGoop(self.transform.position);
							}
							if (bankName == false && self != null)
							{
								yes.DoFireGoop(self.transform.position);
							}
						}
						bool flag = self.name.ToLower().Contains("pot");
						if (flag && self != null)
						{
							if (Loop == 75 | Loop >= 75)
							{
								int FairyScaler;
								FairyScaler = UnityEngine.Random.Range(0, 50);
								if (FairyScaler == 1)
								{
									PotFairyEngageDoer.InstantSpawn = true;
									PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
									AIActor prefabActor = Game.Enemies["pot_fairy"];
									AIActor.Spawn(prefabActor, self.sprite.WorldCenter, primaryPlayer.CurrentRoom, false, AIActor.AwakenAnimationType.Default, true);
								}
							}
							else
							{
								int FairyScaler;
								FairyScaler = (int)UnityEngine.Random.Range(0, 100 - (Loop / 5));
								if (FairyScaler == 1)
								{
									PotFairyEngageDoer.InstantSpawn = true;
									PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
									AIActor prefabActor = Game.Enemies["pot_fairy"];
									AIActor.Spawn(prefabActor, self.sprite.WorldCenter, primaryPlayer.CurrentRoom, false, AIActor.AwakenAnimationType.Default, true);
								}
							}
						}
					}
				}
			}		
		}
		public void DoPoisonGoop(Vector2 v)
		{
			float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
			AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
			GoopDefinition goopDef = assetBundle.LoadAsset<GoopDefinition>("assets/data/goops/poison goop.asset");
			DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDef);
			goopManagerForGoopType.TimedAddGoopCircle(v, 2f + (Loop / 10)+InitialScaling, 0.35f, false);
			goopDef.damagesEnemies = false;
		}

		public void DoFireGoop(Vector2 v)
		{
			float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
			AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
			GoopDefinition goopDef = assetBundle.LoadAsset<GoopDefinition>("assets/data/goops/napalmgoopquickignite.asset");
			DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDef);
			goopManagerForGoopType.TimedAddGoopCircle(v, 2f+(Loop/10) + InitialScaling, 0.35f, false);
			goopDef.damagesEnemies = false;
		}
		public static string[] BannedEnemies = new string[]
		{
			"fodder",
		};
		public static string[] NoNo = new string[]
		{
			 "c0260c286c8d4538a697c5bf24976ccf",
			 "4d37ce3d666b4ddda8039929225b7ede",
			 "3cadf10c489b461f9fb8814abc1a09c1",
		};
		public static float InitialScaling = 0.4f;
	}
}

public class GrenadeYahyeet : Script
{
	protected override IEnumerator Top()
	{
		PlayerController player = GameManager.Instance.PrimaryPlayer;
		DraGunController dragunController = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>();
		AkSoundEngine.PostEvent("Play_BOSS_wall_slam_01", base.BulletBank.aiActor.gameObject);
		yield return base.Wait(75);
		this.FireRocket(dragunController.skyRocket, player.sprite.WorldCenter);

		yield break;
	}
	private void FireRocket(GameObject skyRocket, Vector2 target)
	{
		SkyRocket component = SpawnManager.SpawnProjectile(skyRocket, base.Position, Quaternion.identity, true).GetComponent<SkyRocket>();
		component.TargetVector2 = target;
		tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
		component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
		component.ExplosionData.ignoreList.Add(base.BulletBank.specRigidbody);
	}
}

public class ExplosiveDeath : Script
{
	protected override IEnumerator Top()
	{
		base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("880bbe4ce1014740ba6b4e2ea521e49d").bulletBank.GetBullet("grenade"));
		float airTime = base.BulletBank.GetBullet("grenade").BulletObject.GetComponent<ArcProjectile>().GetTimeInFlight();
		Vector2 vector = this.BulletManager.PlayerPosition();
		Bullet bullet2 = new Bullet("grenade", false, false, false);
		float direction2 = (vector - base.Position).ToAngle();
		base.Fire(new Direction(direction2, DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), bullet2);
		(bullet2.Projectile as ArcProjectile).AdjustSpeedToHit(vector);
		bullet2.Projectile.ImmuneToSustainedBlanks = true;
		yield break;
	}

}
