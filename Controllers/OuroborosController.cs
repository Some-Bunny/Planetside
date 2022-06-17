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
using GungeonAPI;
using SaveAPI;
using Brave.BulletScript;
using Pathfinding;
namespace Planetside
{
	public class OuroborosController : MonoBehaviour
	{
		public void Start()
		{
			Debug.Log("Starting OuroborosController setup...");
			try
			{
				var EliteTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(BasicEliteType)));
				foreach (var EliteType in EliteTypes)
				{
					var item = (BasicEliteType)System.Activator.CreateInstance(EliteType);
					basicEliteTypes.Add(item.GetType());
					
				}

				var SpecialEliteTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(SpecialEliteType)));
				foreach (var SpecialEliteType in SpecialEliteTypes)
				{
					var item = (SpecialEliteType)System.Activator.CreateInstance(SpecialEliteType);
					specialEliteTypes.Add(item.GetType());
				}

				var BossEliteTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(BossEliteType)));
				foreach (var bossEliteType in BossEliteTypes)
				{
					var item = (BossEliteType)System.Activator.CreateInstance(bossEliteType);
					bossEliteTypes.Add(item.GetType());
				}
				AIBulletBank.Entry entryCopy = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"));
				RobotechProjectile projectile = UnityEngine.Object.Instantiate<GameObject>(entryCopy.BulletObject).GetComponent<RobotechProjectile>();
				projectile.gameObject.SetActive(false);
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				projectile.projectileHitHealth = 1;
				entryCopy.Name = "homingOuroboros";
				entryCopy.MuzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
				entryCopy.BulletObject = projectile.gameObject;
				entryCopy.SpawnShells = false;


				AIBulletBank.Entry sewwpCopy = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));
				sewwpCopy.Name = "sweepOuroboros";

				BulletList.Add(sewwpCopy);
				BulletList.Add(entryCopy);
				BulletList.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("homingPop"));

				new Hook(typeof(AIActor).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public),
					typeof(OuroborosController).GetMethod("StartHookAIActor"));

				new Hook(typeof(AIActor).GetMethod("OnPlayerEntered", BindingFlags.Instance | BindingFlags.NonPublic),
					typeof(OuroborosController).GetMethod("OnPlayerEnteredHook"));

				new Hook(typeof(LootEngine).GetMethod("PostprocessGunSpawn", BindingFlags.Static | BindingFlags.NonPublic),
					typeof(OuroborosController).GetMethod("MimicGunScaler"));

				new Hook(typeof(BehaviorSpeculator).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic),
					typeof(OuroborosController).GetMethod("StartHookBehaviorSpeculator"));

				new Hook(typeof(FlippableCover).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic),
					typeof(OuroborosController).GetMethod("StartTableHook"));

				new Hook(typeof(Gun).GetMethod("Awake", BindingFlags.Instance | BindingFlags.Public),
					typeof(OuroborosController).GetMethod("GunAwakeHook"));

				new Hook(typeof(ShopItemController).GetMethods().Single(
					m =>
						m.Name == "Initialize" &&
						m.GetParameters().Length == 2 &&
						m.GetParameters()[1].ParameterType == typeof(BaseShopController)),
					typeof(OuroborosController).GetMethod("InitializeHookBase", BindingFlags.Static | BindingFlags.Public));

				new Hook(typeof(ShopItemController).GetMethods().Single(
				   m =>
					   m.Name == "Initialize" &&
					   m.GetParameters().Length == 2 &&
					   m.GetParameters()[1].ParameterType == typeof(ShopController)),
				   typeof(OuroborosController).GetMethod("InitializeHook", BindingFlags.Static | BindingFlags.Public));


				new Hook(typeof(Chest).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic),
					typeof(OuroborosController).GetMethod("ChestAwakeHook"));

				chestPickupsLootTable = LootTableTools.CreateLootTable();
				chestPickupsLootTable.AddItemsToPool(new Dictionary<int, float> { { 120, 1 }, {224, 0.8f }, {600, 0.5f}, {78, 0.75f } });

				Debug.Log("Finished OuroborosController setup without failure!");
			}
			catch (Exception e)
			{
				Debug.Log("Unable to finish OuroborosController setup!");
				Debug.Log(e);
			}
		}

		private static List<Type> basicEliteTypes = new List<Type>();
		private static List<Type> specialEliteTypes = new List<Type>();
		private static List<Type> bossEliteTypes = new List<Type>();
		private static GenericLootTable chestPickupsLootTable;


		public static Dictionary<string, GameObject> eliteCrownKeys = new Dictionary<string, GameObject>();

	
		public static void ChestAwakeHook(Action<Chest> orig, Chest self)
        {
			orig(self);
			if (self != null && OuroborosMode() == true)
            {
				if (UnityEngine.Random.value <= ChanceAccordingToGivenValues(0.01f, 0.5f, 100))
				{self.lootTable.overrideItemLootTables.Add(chestPickupsLootTable);}
			}
		}

		public static void InitializeHookBase(Action<ShopItemController, PickupObject, BaseShopController> orig, ShopItemController self, PickupObject i, BaseShopController shop)
        {
			orig(self, i, shop);
			if (self != null && OuroborosMode() == true && shop.baseShopType != BaseShopController.AdditionalShopType.FOYER_META && UnityEngine.Random.value <= ChanceAccordingToGivenValues(0.01f, 1f, 100))
			{
				float Mult = UnityEngine.Random.value < 0.5f ? 2 : 0.5f;
				self.CurrentPrice = (int)(self.CurrentPrice * Mult);
            }
		}
		public static void InitializeHook(Action<ShopItemController, PickupObject, ShopController> orig, ShopItemController self, PickupObject i, ShopController shop)
		{
			orig(self, i, shop);
			if (self != null && OuroborosMode() == true && UnityEngine.Random.value <= ChanceAccordingToGivenValues(0.01f, 1f, 100))
			{
				float Mult = UnityEngine.Random.value < 0.5f ? 2 : 0.5f;
				self.CurrentPrice = (int)(self.CurrentPrice * Mult);
			}
		}
		public static void GunAwakeHook(Action<Gun> orig, Gun self)
		{
			orig(self);
			if (self != null && OuroborosMode() == true)
			{
				if (self.InfiniteAmmo == false)
                {
					if (UnityEngine.Random.value <= ChanceAccordingToGivenValues(0.01f, 1f, 100))
					{
						bool Check = self.CanGainAmmo == true || self.InfiniteAmmo == false;
						if (self.HasBeenPickedUp == false && Check == true)
						{
							float H = self.GetBaseMaxAmmo() * ChanceAccordingToGivenValues(0, 0.875f, 25);
							self.LoseAmmo((int)H);
						}
					}
				}
			}
		}
		public static void StartTableHook(Action<FlippableCover> orig, FlippableCover self)
        {
			orig(self);
			if (self != null && OuroborosMode() == true)
            {
				if (self.majorBreakable != null)
                {self.majorBreakable.HitPoints = ChanceAccordingToGivenValues(1, 0.33f, 25);}
			}
        }
		public static void MimicGunScaler(Action<Gun> orig, Gun spawnedGun)
		{
			if (OuroborosMode() == true)
			{
				spawnedGun.gameObject.SetActive(true);
				if (GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_HAS_BEEN_PEDESTAL_MIMICKED) && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.NONE && UnityEngine.Random.value < ChanceAccordingToGivenValues(0.001f, 0.005f, 25))
				{
					spawnedGun.gameObject.AddComponent<MimicGunMimicModifier>();
				}
			}
			else
			{
				spawnedGun.gameObject.SetActive(true);
				if (GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_HAS_BEEN_PEDESTAL_MIMICKED) && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.NONE && UnityEngine.Random.value < 0.001)
				{
					spawnedGun.gameObject.AddComponent<MimicGunMimicModifier>();
				}
			}
		}
		public static void OnPlayerEnteredHook(Action<AIActor, PlayerController> orig, AIActor self, PlayerController player)
		{
			orig(self, player);
			if (OuroborosMode() == true)
			{
			
			}
		}
		public static void StartHookAIActor(Action<AIActor> orig, AIActor self)
        {
            orig(self);
            if (OuroborosMode() == true && self != null && EnemyISFUCKINGKILLPILLARFUCKYOUFUCKYOU(self) == false)
            {
				if (!enemiesNotAffectedByBehaviorSpecMultiplier.Contains(self.EnemyGuid)) { self.MovementSpeed *= ChanceAccordingToGivenValues(1f, 2f, 250); }
				if (!enemiesNotAffectedByBehaviorSpecMultiplier.Contains(self.EnemyGuid) && self.behaviorSpeculator != null) { self.behaviorSpeculator.CooldownScale *= ChanceAccordingToGivenValues(1f, 0.4f, 250); }
				if (!enemiesNotAffectedByBehaviorSpecMultiplier.Contains(self.EnemyGuid)) { self.healthHaver.SetHealthMaximum(self.healthHaver.GetCurrentHealth() * ChanceAccordingToGivenValues(1f, 1.3f, 250)); }
				if (self != null && self.aiActor != null && EnemyIsValid(self.aiActor) == true && !bannedEnemiesForOrbitingSkulls.Contains(self.aiActor.EnemyGuid) && UnityEngine.Random.value <= ChanceAccordingToGivenValues(0.0015f, 0.5f, 75) && !enemiesNotAffectedByBehaviorSpecMultiplier.Contains(self.EnemyGuid))
				{
					GameObject transPos = self.aiActor.transform.Find("SkullAttackPointOuroboros").gameObject;
					if (transPos != null)
					{
						float SkullAmount = ReturnSkullAmount(CurrentLoop());
						SpinBulletsController spinBulletsController = self.aiActor.gameObject.AddComponent<SpinBulletsController>();
						spinBulletsController.ShootPoint = self.aiActor.transform.Find("SkullAttackPointOuroboros").gameObject;
						spinBulletsController.OverrideBulletName = "homingOuroboros";
						spinBulletsController.NumBullets = 2;
						spinBulletsController.BulletMinRadius = 2.25f;
						spinBulletsController.BulletMaxRadius = 2.5f;
						spinBulletsController.BulletCircleSpeed = 60;
						spinBulletsController.BulletsIgnoreTiles = true;
						spinBulletsController.RegenTimer = (CurrentLoop() / 25) + 0.25f;
						spinBulletsController.AmountOFLines = SkullAmount;
					}
				}
			}
        }
		public static List<string> enemiesNotAffectedByBehaviorSpecMultiplier => new List<string>()
		{"3f11bbbc439c4086a180eb0fb9990cb4"};



		public static float ChanceAccordingToGivenValues(float minimumChance, float maximumChance, float LoopToCapAt)
        {
			float T = Mathf.Min(CurrentLoop() / LoopToCapAt, 1);
			float Chance = Mathf.Lerp(minimumChance, maximumChance, T);
			return Chance;
        }
        private static List<AIBulletBank.Entry> BulletList = new List<AIBulletBank.Entry>();
        public static void StartHookBehaviorSpeculator(Action<BehaviorSpeculator> orig, BehaviorSpeculator self)
		{
			orig(self);
			if (OuroborosMode() == true)
            {
                if (self != null && self.aiActor != null && EnemyIsValid(self.aiActor) == true)
                {
					if (self.aiActor.bulletBank == null)
					{
						AIBulletBank bulletBank = new AIBulletBank();
						bulletBank.Bullets = new List<AIBulletBank.Entry>();

						foreach (AIBulletBank.Entry entry in BulletList)
						{
							bulletBank.Bullets.Add(entry);
						}

						bulletBank.FixedPlayerRigidbody = self.aiActor.specRigidbody;
						bulletBank.ActorName = self.aiActor.name != null ? self.aiActor.name : "Toddy";
						self.aiActor.gameObject.AddComponent(bulletBank);
					}
					else
                    {
						foreach (AIBulletBank.Entry entry in BulletList)
						{
							self.aiActor.bulletBank.Bullets.Add(entry);
						}
					}		
					EnemyToolbox.GenerateShootPoint(self.aiActor.gameObject, self.aiActor.CenterPosition, "SkullAttackPointOuroboros");
					if (self.OtherBehaviors == null)
                    {
						self.OtherBehaviors = new List<BehaviorBase>();
                    }
				}
				if (!EliteBlackListDefault.Contains(self.aiActor.EnemyGuid) && EnemyIsValid(self.aiActor) == true)
                {
					if (UnityEngine.Random.value <= ChanceAccordingToGivenValues(0.05f, 0.75f, 50))
					{
						bool BossCheck = self.aiActor.healthHaver.IsBoss | self.aiActor.healthHaver.IsSubboss;
						float specialChance = ChanceAccordingToGivenValues(0.01f, 1f, 1);
						if (UnityEngine.Random.value <= specialChance && BossCheck == false)//if (UnityEngine.Random.value <= ChanceAccordingToGivenValues(0.03f, 0.1f, 50))
						{
							var SpecialElite = specialEliteTypes[UnityEngine.Random.Range(0, specialEliteTypes.Count)];
							self.aiActor.gameObject.AddComponent(SpecialElite);
						}
						else
						{
							self.aiActor.gameObject.AddComponent(basicEliteTypes[UnityEngine.Random.Range(0, basicEliteTypes.Count)]);
						}
					}
				}		
			}
		}

		private static bool EnemyISFUCKINGKILLPILLARFUCKYOUFUCKYOU(AIActor aI)
        {
			if (aI.gameObject.GetComponent<BossStatueController>() != null || aI.gameObject.GetComponentInChildren<BossStatueController>() != null) { return true; }
			return false;
		}

		private static bool EnemyIsValid(AIActor aI)
		{
			if (aI.IgnoreForRoomClear == true) { return false; }
			if (aI.gameObject.GetComponent<MirrorImageController>() != null) { return false; }
			if (aI.gameObject.GetComponent<DisplacedImageController>() != null) { return false; ; }
			if (aI.CompanionOwner != null) { return false;}
			if (aI.gameObject.GetComponent<CompanionController>() != null) { return false; }
			if (StaticInformation.ModderBulletGUIDs.Contains(aI.EnemyGuid)) { return false; }
			if (aI.EnemyGuid == "3f11bbbc439c4086a180eb0fb9990cb4") { return false; }
			if (aI.gameObject.GetComponent<BossStatueController>() != null || aI.gameObject.GetComponentInChildren<BossStatueController>() != null) { return false; }
				return true;
		}
		public static List<string> EnemyBlackListForReplicatorEliteFuckFuckFuckFuck => new List<string>()
		{
			EnemyGuidDatabase.Entries["blobulon"],
			EnemyGuidDatabase.Entries["blobuloid"],
			EnemyGuidDatabase.Entries["blobulin"],

			EnemyGuidDatabase.Entries["poisbulon"],
			EnemyGuidDatabase.Entries["poisbuloid"],
			EnemyGuidDatabase.Entries["poisbulin"],

			EnemyGuidDatabase.Entries["king_bullat"],

			EnemyGuidDatabase.Entries["spectre"],
			EnemyGuidDatabase.Entries["king_bullat"],

			EnemyGuidDatabase.Entries["mountain_cube"],
			EnemyGuidDatabase.Entries["lead_cube"],
			EnemyGuidDatabase.Entries["flesh_cube"],

			EnemyGuidDatabase.Entries["brown_chest_mimic"],

			EnemyGuidDatabase.Entries["brown_chest_mimic"],
			EnemyGuidDatabase.Entries["blue_chest_mimic"],
			EnemyGuidDatabase.Entries["green_chest_mimic"],
			EnemyGuidDatabase.Entries["red_chest_mimic"],
			EnemyGuidDatabase.Entries["black_chest_mimic"],
			EnemyGuidDatabase.Entries["rat_chest_mimic"],
			EnemyGuidDatabase.Entries["pedestal_mimic"],
			EnemyGuidDatabase.Entries["wall_mimic"],

			EnemyGuidDatabase.Entries["misfire_beast"],

			EnemyGuidDatabase.Entries["shambling_round"],
			EnemyGuidDatabase.Entries["killithid"],

			EnemyGuidDatabase.Entries["gat"],

			EnemyGuidDatabase.Entries["lead_maiden"],
			EnemyGuidDatabase.Entries["fridge_maiden"],
		};

		private static List<string> bannedEnemiesForOrbitingSkulls = new List<string>()
		{
			EnemyGuidDatabase.Entries["gummy_spent"],
			EnemyGuidDatabase.Entries["brown_chest_mimic"],
			EnemyGuidDatabase.Entries["blue_chest_mimic"],
			EnemyGuidDatabase.Entries["green_chest_mimic"],
			EnemyGuidDatabase.Entries["red_chest_mimic"],
			EnemyGuidDatabase.Entries["black_chest_mimic"],
			EnemyGuidDatabase.Entries["rat_chest_mimic"],
			EnemyGuidDatabase.Entries["pedestal_mimic"],
			EnemyGuidDatabase.Entries["wall_mimic"],
			EnemyGuidDatabase.Entries["hollowpoint"],
			EnemyGuidDatabase.Entries["spectre"],
			EnemyGuidDatabase.Entries["leadbulon"],
			EnemyGuidDatabase.Entries["bloodbulon"],

			EnemyGuidDatabase.Entries["apprentice_gunjurer"],
			EnemyGuidDatabase.Entries["gunjurer"],
			EnemyGuidDatabase.Entries["high_gunjurer"],
			EnemyGuidDatabase.Entries["lore_gunjurer"],

			EnemyGuidDatabase.Entries["gat"],
			EnemyGuidDatabase.Entries["rubber_kin"],
			EnemyGuidDatabase.Entries["tazie"],
			EnemyGuidDatabase.Entries["bullet_shark"],
			EnemyGuidDatabase.Entries["phaser_spider"],
			EnemyGuidDatabase.Entries["great_bullet_shark"],

			EnemyGuidDatabase.Entries["blobulon"],
			EnemyGuidDatabase.Entries["blobuloid"],
			EnemyGuidDatabase.Entries["blobulin"],

			EnemyGuidDatabase.Entries["poisbulon"],
			EnemyGuidDatabase.Entries["poisbuloid"],
			EnemyGuidDatabase.Entries["poisbulin"],
		};

		private static List<string> EliteBlackListDefault = new List<string>()
		{
			"deturretleft_enemy",
			"deturret_enemy",
			"fodder_enemy",
			EnemyGuidDatabase.Entries["mine_flayers_bell"],
			EnemyGuidDatabase.Entries["gunreaper"],
			EnemyGuidDatabase.Entries["key_bullet_kin"],
			EnemyGuidDatabase.Entries["chance_bullet_kin"],
			EnemyGuidDatabase.Entries["grip_master"],
			EnemyGuidDatabase.Entries["mine_flayers_claymore"],

			EnemyGuidDatabase.Entries["chicken"],
			EnemyGuidDatabase.Entries["snake"],
			EnemyGuidDatabase.Entries["poopulons_corn"],
			EnemyGuidDatabase.Entries["rat"],
			EnemyGuidDatabase.Entries["rat_candle"],
			EnemyGuidDatabase.Entries["dragun_egg_slimeguy"],
		};


		private static List<string> EnemiesThatLiterallyShouldNeverGetModifiedInAnyWay = new List<string>()
		{
			"shamber_psog",
			EnemyGuidDatabase.Entries["spent"],
			EnemyGuidDatabase.Entries["mouser"],

			EnemyGuidDatabase.Entries["mountain_cube"],
			EnemyGuidDatabase.Entries["lead_cube"],
			EnemyGuidDatabase.Entries["flesh_cube"],

			EnemyGuidDatabase.Entries["bullet_kings_toadie"],
			EnemyGuidDatabase.Entries["bullet_kings_toadie_revenge"],
			EnemyGuidDatabase.Entries["old_kings_toadie"],
			EnemyGuidDatabase.Entries["fusebot"],

			EnemyGuidDatabase.Entries["ammoconda_ball"],
			EnemyGuidDatabase.Entries["mine_flayers_claymore"],
		};


		/*
		 *spectre
		 *leadbulon
		 *bloodbulon
		 *apprentice_gunjurer
		 *gunjurer
		 *high_gunjurer
		 *lore_gunjurer
		 *mountain_cube
		 *lead_cube
		 *flesh_cube
		 *gat
		 *rubber_kin
		 *tazie
		 *bullet_shark
		 *phaser_spider
		 *grip_master
		 *great_bullet_shark
		 *ammoconda_ball
		 *mine_flayers_claymore
		 *dragun_knife_advanced
		 *chicken
		 *snake
		 *poopulons_corn
		 *tiny_blobulord
		 *rat
		 *rat_candle
		 *dragun_egg_slimeguy
		 */

		public static float ReturnSkullAmount(float Loop)
        {
            float AmountOfSkulls = 3;
            for (int i = 0; i < Loop; i++)
            {
                if (i % 10 == 0)
                {AmountOfSkulls++;}
            }
            return AmountOfSkulls;
        }


        public static bool OuroborosMode()
        {
            return AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.LOOPING_ON);
        }
        public static float CurrentLoop()
        {
            return AdvancedGameStatsManager.Instance.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
        }

        public static void OnHealthHaverDie(Action<HealthHaver, Vector2> orig, HealthHaver self, Vector2 finalDamageDir)
        {
            orig(self, finalDamageDir);
        }

        private void ResetFloorSpecificData()
        {
            
        }
    }
}

namespace Planetside
{
	public class StoneEyesElite : BasicEliteType
	{
		public override float HealthMultiplier => 1.3f;
		public override float CooldownMultiplier => 0.9f;
		public override float MovementSpeedMultiplier => 1.3f;
		public override Color EliteOutlineColor => Color.blue;
		public override Color EliteParticleColor => Color.blue;
		public override Color SecondaryEliteParticleColor => Color.blue;
		public override List<string> EnemyBlackList => new List<string>() { };
		public override List<ActorEffectResistance> DebuffImmunities => new List<ActorEffectResistance> { new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.Freeze } };
		public override void Start()
		{
			Timer = 2f;
			base.Start();
		}
		public override void OnPreDeath(Vector2 obj)
		{

		}

		public override void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
		{
			if (Timer == 0 | Timer <= 0)
			{
				Timer = 5f;
				if (base.aiActor != null)
				{
					GameManager.Instance.StartCoroutine(this.LaunchWave(base.aiActor.CenterPosition));
				}
			}
		}
		private IEnumerator LaunchWave(Vector2 startPoint, float Time = 2.5f)
		{
			float m_prevWaveDist = 0f;
			float distortionMaxRadius = 20f;
			float distortionDuration = 1.5f;
			float distortionIntensity = 0.5f;
			float distortionThickness = 0.04f;
			GameObject instanceVFX = SpawnManager.SpawnVFX(StaticVFXStorage.GorgunEyesVFX, startPoint.ToVector3ZUp(0f) + new Vector3(-3.1875f, -3f, 0f), Quaternion.identity);
			tk2dSprite instanceSprite = instanceVFX.GetComponent<tk2dSprite>();
			float elapsedTime = 0f;
			while (instanceVFX && instanceVFX.activeSelf)
			{
				elapsedTime += BraveTime.DeltaTime;
				if (instanceSprite)
				{
					instanceSprite.PlaceAtPositionByAnchor(startPoint, tk2dBaseSprite.Anchor.MiddleCenter);
				}
				if (elapsedTime > 0.75f)
				{
					AkSoundEngine.PostEvent("Play_ENM_gorgun_gaze_01", instanceVFX.gameObject);
					elapsedTime -= 1000f;
				}
				yield return null;
			}
			Exploder.DoDistortionWave(startPoint, distortionIntensity, distortionThickness, distortionMaxRadius, distortionDuration);
			float waveRemaining = distortionDuration - BraveTime.DeltaTime;
			while (waveRemaining > 0f)
			{
				waveRemaining -= BraveTime.DeltaTime;
				float waveDist = BraveMathCollege.LinearToSmoothStepInterpolate(0f, distortionMaxRadius, 1f - waveRemaining / distortionDuration);
				for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
				{
					PlayerController playerController = GameManager.Instance.AllPlayers[i];
					if (!playerController.healthHaver.IsDead)
					{
						if (!playerController.spriteAnimator.QueryInvulnerabilityFrame() && playerController.healthHaver.IsVulnerable)
						{
							Vector2 unitCenter = playerController.specRigidbody.GetUnitCenter(ColliderType.HitBox);
							float num = Vector2.Distance(unitCenter, startPoint);
							if (num >= m_prevWaveDist - 0.25f && num <= waveDist + 0.25f)
							{
								float b = (unitCenter - startPoint).ToAngle();
								if (BraveMathCollege.AbsAngleBetween(playerController.FacingDirection, b) >= 60f)
								{
									playerController.CurrentStoneGunTimer += Time;
								}
							}
						}
					}
				}
				m_prevWaveDist = waveDist;
				yield return null;
			}
			yield break;
		}
		public override void Update()
		{
			base.Update();
			if (base.aiActor)
			{
				if (Timer >= 0) { Timer -= BraveTime.DeltaTime; }
			}
		}

		private void HealthHaver_OnPreDeath(Vector2 obj)
		{
		}

		private float Timer;
	}
	public class FrenzyElite : BasicEliteType
	{
		public override float HealthMultiplier => 0.5f;
		public override float CooldownMultiplier => 0.33f;
		public override float MovementSpeedMultiplier => 1.55f;
		public override Color EliteOutlineColor => Color.yellow;
		public override Color EliteParticleColor => Color.yellow;
		public override Color SecondaryEliteParticleColor => Color.yellow;
		public override List<string> EnemyBlackList => new List<string>() { };
		public override List<ActorEffectResistance> DebuffImmunities => new List<ActorEffectResistance> { new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.Freeze } };
		public override void Start()
		{
			base.Start();
		}
		public override void OnPreDeath(Vector2 obj)
		{

		}

		public override void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
		{


		}
		public override void Update()
		{
			base.Update();

		}

		private void HealthHaver_OnPreDeath(Vector2 obj)
		{
		}
	}
	public class HealingElite : BasicEliteType
	{
		public override float HealthMultiplier => 1.33f;
		public override float CooldownMultiplier => 0.4f;
		public override float MovementSpeedMultiplier => 1f;
		public override Color EliteOutlineColor => Color.green;
		public override Color EliteParticleColor => Color.green;
		public override Color SecondaryEliteParticleColor => Color.green;
		public override List<string> EnemyBlackList => new List<string>()
		{
		};
		public override List<ActorEffectResistance> DebuffImmunities => new List<ActorEffectResistance> {
			new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.None },
		};
		public override void Start()
		{
			base.Start();
		}

        public override void OnPreDeath(Vector2 obj)
        {
			GameManager.Instance.StartCoroutine(SpawnRadialPoofs(base.aiActor.CenterPosition, base.aiActor.GetAbsoluteParentRoom()));
		}
		private void ProcessEnemy(AIActor target, float distance)
		{
			if (target != null)
            {
				target.PlayEffectOnActor(StaticVFXStorage.HealingSparklesVFX, Vector3.zero, true, false, false);
				target.healthHaver.FullHeal();
			}
		}
		private IEnumerator SpawnRadialPoofs(Vector2 centre, RoomHandler room)
		{
			float elapsed = 0f;
			float duration = 1.5f;
			HeatIndicatorController radialIndicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), centre, Quaternion.identity)).GetComponent<HeatIndicatorController>();
			radialIndicator.CurrentColor = Color.green.WithAlpha(50);
			radialIndicator.IsFire = false;
			radialIndicator.CurrentRadius = 0;
			while (elapsed < duration)
			{
				if (radialIndicator.gameObject == null) { break; }
				elapsed += BraveTime.DeltaTime;
				float t = elapsed;
				radialIndicator.CurrentRadius = Mathf.Lerp(0, 7, t);
				yield return null;
			}
			AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", radialIndicator.gameObject);
			room.ApplyActionToNearbyEnemies(centre, 5, new Action<AIActor, float>(this.ProcessEnemy));
			elapsed = 0f;
			duration = 0.25f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
				radialIndicator.CurrentRadius = Mathf.Lerp(5, 0, t);
				for (int i = 0; i < 2; i++)
				{
					float Dist = Mathf.Lerp(0.1f, 7, t);
					Vector2 Point = MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), Dist);
					GameObject obj = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.HealingSparklesVFX, centre + Point, Quaternion.identity);
					obj.transform.localScale *= 1.2f;
				}
				yield return null;
			}
			Destroy(radialIndicator.gameObject);
			
			yield break;
		}
	}
	public class ResistantElite : BasicEliteType
	{
		public override float HealthMultiplier => 1.25f;
		public override float CooldownMultiplier => 0.9f;
		public override float MovementSpeedMultiplier => 1.15f;
		public override Color EliteOutlineColor => Color.white;
		public override Color EliteParticleColor => Color.white;
		public override Color SecondaryEliteParticleColor => Color.white;
		public override List<string> EnemyBlackList => new List<string>()
		{
			EnemyGuidDatabase.Entries["mine_flayers_bell"],
		};

		public override List<ActorEffectResistance> DebuffImmunities => new List<ActorEffectResistance> { 
			new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.Fire },
			new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.Poison },
			new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.Charm },
			new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.Freeze },
		};
		public override void Start()
		{
			base.Start();
			base.aiActor.knockbackDoer.SetImmobile(true, "Elite.");
		}
		private float Timer;

		private IEnumerator IncreaseInSize(tk2dSprite CircleSprite, float SizeMultiplier = 1)
		{
			float elapsed = 0f;
			float duration = 0.75f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
				if (CircleSprite != null && CircleSprite.gameObject != null)
				{
					CircleSprite.scale = Vector3.Lerp(Vector3.zero, Vector3.one * SizeMultiplier, t);
				}
				yield return null;
			}
			yield break;
		}
		public override void Update()
        {
            base.Update();
			if (base.aiActor)
			{
				if (Timer >= 0) { Timer -= BraveTime.DeltaTime; }
				if (Timer == 0 | Timer <= 0)
				{
					Timer = 5f;
					if (base.aiActor != null)
					{
						GameObject dragunBoulder = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyBoulder;
						foreach (Component item in dragunBoulder.GetComponentsInChildren(typeof(Component)))
						{
							if (item is SkyRocket laser)
							{
								foreach (Component item2 in UnityEngine.Object.Instantiate<GameObject>(laser.SpawnObject, base.aiActor.CenterPosition, Quaternion.identity).GetComponentsInChildren(typeof(Component)))
								{
									if (item2 is DraGunBoulderController laser2)
									{
										laser2.LifeTime = 15;
										GameManager.Instance.Dungeon.StartCoroutine(this.IncreaseInSize(laser2.CircleSprite, 0.5f));
										SpeculativeRigidbody body = laser2.GetComponentInChildren<SpeculativeRigidbody>();
										if (body)
										{
											List<PixelCollider> colliders = body.PixelColliders.ToList();
											foreach (PixelCollider collider in colliders)
											{
												collider.ManualOffsetX = (int)(collider.ManualOffsetX * 0.5f);
												collider.ManualOffsetY = (int)(collider.ManualOffsetY * 0.5f);
												collider.ManualDiameter = (int)(collider.ManualDiameter * 0.5f);
												collider.ManualHeight = (int)(collider.ManualHeight * 0.5f);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
    }
	public class CursedElite : BasicEliteType
	{
		public override float HealthMultiplier => 1.15f;
		public override float CooldownMultiplier => 0.8f;
		public override float MovementSpeedMultiplier => 1.33f;
		public override Color EliteOutlineColor => new Color(50, 0, 50);
		public override Color EliteParticleColor => Color.magenta;
		public override Color SecondaryEliteParticleColor => new Color(10, 0, 10);
		public override List<string> EnemyBlackList => new List<string>()
		{
			EnemyGuidDatabase.Entries["bullet_kings_toadie_revenge"],
		};

		public override List<ActorEffectResistance> DebuffImmunities => new List<ActorEffectResistance> {
			new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.None },
		};
		public override void Start()
		{
			base.Start();
		}
		public override void OnPreDeath(Vector2 obj)
		{
			DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(DebuffLibrary.CursebulonGoop);
			goopManagerForGoopType.TimedAddGoopCircle(base.aiActor.transform.PositionVector2(), OuroborosController.ChanceAccordingToGivenValues(3.5f, 7f, 100), 1f, false);
		}
		public override void Update()
		{
			base.Update();
			if (base.aiActor && !GameManager.Instance.IsPaused)
			{
				Vector3 vector = base.aiActor.sprite.WorldBottomLeft.ToVector3ZisY(0);
				Vector3 vector2 = base.aiActor.sprite.WorldTopRight.ToVector3ZisY(0);
				Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
				GlobalSparksDoer.DoSingleParticle(position, Vector3.up, null, null, null, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
			}
		}
	}
	public class ReflectiveElite : BasicEliteType
	{
		public override float HealthMultiplier => 1.35f;
		public override float CooldownMultiplier => 1f;
		public override float MovementSpeedMultiplier => 0.85f;
        public override Color EliteOutlineColor => new Color(0, 10, 50);
		public override Color EliteParticleColor => Color.blue;
		public override Color SecondaryEliteParticleColor => Color.cyan;
		public override List<string> EnemyBlackList => new List<string>()
		{
			EnemyGuidDatabase.Entries["rat_candle"],
		};

        public override List<ActorEffectResistance> DebuffImmunities => new List<ActorEffectResistance> {
			new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.None },
		};
		public override void Start()
		{
			Timer = 0.75f;
			base.Start();
		}

        public override void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
			if (Timer == 0 | Timer <= 0)
            {
				Timer = 0.5f;
				if (base.aiActor != null)
				{
					SpawnManager.SpawnBulletScript(base.aiActor, new CustomBulletScriptSelector(typeof(EliteReflect)));
				}
			}
		}
		public override void Update()
		{
			base.Update();
			if (base.aiActor)
			{
				if (Timer >= 0) { Timer -= BraveTime.DeltaTime; }
			}
		}
		private float Timer;
	}
	public class EliteReflect : Script
	{
		protected override IEnumerator Top()
		{
			if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));
			}
			base.PostWwiseEvent("Play_ENM_ironmaiden_blast_01", null);
			float RNG = UnityEngine.Random.Range(0, 60);
			for (int i = 0; i <= 6; i++)
			{
				this.Fire(new Direction((i * 60)+ RNG, DirectionType.Aim, -1f), new Speed(1f, SpeedType.Absolute), new SkellBullet());
			}
			yield break;
		}
		public class SkellBullet : Bullet
		{
			public SkellBullet() : base("sweepOuroboros", false, false, false)
			{

			}
			protected override IEnumerator Top()
			{
				base.ChangeSpeed(new Speed(1f, SpeedType.Absolute), 20);
				yield return this.Wait(60);
				base.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 40);
				yield break;
			}
		}
	}
}
namespace Planetside
{ 
	public class DetonatorElite : SpecialEliteType
    {
        public override float HealthMultiplier => 5f;
        public override float CooldownMultiplier => 0.5f;
        public override float MovementSpeedMultiplier => 0.5f;
        public override Color EliteOutlineColor => Color.red;
        public override Color EliteParticleColor => Color.red;
        public override Color SecondaryEliteParticleColor => Color.red;
		public override List<string> EnemyBlackList => new List<string>()
		{

		};
		public override List<ActorEffectResistance> DebuffImmunities => new List<ActorEffectResistance> { new ActorEffectResistance() {resistAmount = 1, resistType =  EffectResistanceType.Fire} };
		public override void Start()
        {
			base.Start();
			GoopDoer goop = base.aiActor.GetOrAddComponent<GoopDoer>();
			goop.goopDefinition = EasyGoopDefinitions.FireDef;
			goop.goopTime = 0.1f;
			goop.defaultGoopRadius = 1.2f;
		}

        public override void OnPreDeath(Vector2 obj)
        {
			GameManager.Instance.StartCoroutine(OnPreDeathBoom(base.aiActor.transform.position));
        }

		private IEnumerator OnPreDeathBoom(Vector3 DeathPosition)
        {
			float elapsed = 0f;
			float duration = 2f;
			HeatIndicatorController radialIndicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), DeathPosition, Quaternion.identity)).GetComponent<HeatIndicatorController>();
			radialIndicator.CurrentColor = Color.red.WithAlpha(50);
			radialIndicator.IsFire = false;
			radialIndicator.CurrentRadius = 0;
			AkSoundEngine.PostEvent("Play_ENM_hammer_target_01", radialIndicator.gameObject);
			while (elapsed < duration)
			{
				if (radialIndicator.gameObject == null) { break; }
				elapsed += BraveTime.DeltaTime;
				float t = elapsed / 1.5f;
				radialIndicator.CurrentRadius = Mathf.Lerp(0, 7, t);
				yield return null;
			}
			ExplosionData explo = StaticExplosionDatas.genericLargeExplosion;
			explo.forceUseThisRadius = true;
			explo.damageRadius = 7;
			explo.damage = 350;
			Exploder.Explode(DeathPosition, explo, Vector2.zero, null, false, CoreDamageTypes.None, false);
			GameObject Blast = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").GetComponent<BossFinalRogueDeathController>().DeathStarExplosionVFX);
			Blast.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(DeathPosition, tk2dBaseSprite.Anchor.LowerCenter);
			Blast.transform.position = DeathPosition.Quantize(0.0625f);
			Blast.GetComponent<tk2dBaseSprite>().UpdateZDepth();
			Destroy(Blast, 10);
			Destroy(radialIndicator.gameObject);
			AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", Blast);
			yield break;
        }


		public override void Update()
        {
			base.Update();
			if (base.aiActor && !GameManager.Instance.IsPaused)
            {
				Vector3 vector = base.aiActor.sprite.WorldBottomLeft.ToVector3ZisY(0);
				Vector3 vector2 = base.aiActor.sprite.WorldTopRight.ToVector3ZisY(0);
				Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
				GlobalSparksDoer.DoSingleParticle(position, Vector3.up, null, null, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
			}
		}
    }
}
namespace Planetside
{
	public class ReplicatorElite : SpecialEliteType
	{
		public override float HealthMultiplier => 3f;
		public override float CooldownMultiplier => 0.9f;
		public override float MovementSpeedMultiplier => 0.7f;
		public override Color EliteOutlineColor => Color.blue;
		public override Color EliteParticleColor => Color.white;
		public override Color SecondaryEliteParticleColor => Color.blue;
		public override List<string> EnemyBlackList => new List<string>()
		{
			EnemyGuidDatabase.Entries["blobulon"],
			EnemyGuidDatabase.Entries["blobuloid"],
			EnemyGuidDatabase.Entries["blobulin"],

			EnemyGuidDatabase.Entries["poisbulon"],
			EnemyGuidDatabase.Entries["poisbuloid"],
			EnemyGuidDatabase.Entries["poisbulin"],

			EnemyGuidDatabase.Entries["king_bullat"],

			EnemyGuidDatabase.Entries["spectre"],
			EnemyGuidDatabase.Entries["king_bullat"],

			EnemyGuidDatabase.Entries["mountain_cube"],
			EnemyGuidDatabase.Entries["lead_cube"],
			EnemyGuidDatabase.Entries["flesh_cube"],

			EnemyGuidDatabase.Entries["brown_chest_mimic"],

			EnemyGuidDatabase.Entries["brown_chest_mimic"],
			EnemyGuidDatabase.Entries["blue_chest_mimic"],
			EnemyGuidDatabase.Entries["green_chest_mimic"],
			EnemyGuidDatabase.Entries["red_chest_mimic"],
			EnemyGuidDatabase.Entries["black_chest_mimic"],
			EnemyGuidDatabase.Entries["rat_chest_mimic"],
			EnemyGuidDatabase.Entries["pedestal_mimic"],
			EnemyGuidDatabase.Entries["wall_mimic"],

			EnemyGuidDatabase.Entries["misfire_beast"],

			EnemyGuidDatabase.Entries["shambling_round"],
			EnemyGuidDatabase.Entries["killithid"],
			EnemyGuidDatabase.Entries["lead_maiden"],
			EnemyGuidDatabase.Entries["fridge_maiden"],

		};
		public override List<ActorEffectResistance> DebuffImmunities => new List<ActorEffectResistance> { new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.Charm } };
		public override void Start()
		{
			base.Start();
		}
		public override void OnPreDeath(Vector2 obj)
		{

		}
		public override void Update()
		{
			base.Update();
		}
	}
}
namespace Planetside
{
	public class ChaoticShiftedElite : SpecialEliteType
	{
		public override float HealthMultiplier => 2.5f;
		public override float CooldownMultiplier => 0.8f;
		public override float MovementSpeedMultiplier => 1.5f;
		public override Color EliteOutlineColor => Color.green;
		public override Color EliteParticleColor => Color.green;
		public override Color SecondaryEliteParticleColor => Color.magenta;
		public override List<string> EnemyBlackList => new List<string>()
		{

			EnemyGuidDatabase.Entries["mountain_cube"],
			EnemyGuidDatabase.Entries["lead_cube"],
			EnemyGuidDatabase.Entries["flesh_cube"],

			EnemyGuidDatabase.Entries["brown_chest_mimic"],

			EnemyGuidDatabase.Entries["brown_chest_mimic"],
			EnemyGuidDatabase.Entries["blue_chest_mimic"],
			EnemyGuidDatabase.Entries["green_chest_mimic"],
			EnemyGuidDatabase.Entries["red_chest_mimic"],
			EnemyGuidDatabase.Entries["black_chest_mimic"],
			EnemyGuidDatabase.Entries["rat_chest_mimic"],
			EnemyGuidDatabase.Entries["pedestal_mimic"],
			EnemyGuidDatabase.Entries["wall_mimic"],

			EnemyGuidDatabase.Entries["misfire_beast"],

			EnemyGuidDatabase.Entries["shambling_round"],
			EnemyGuidDatabase.Entries["killithid"],
		};
		//lead_maiden
		/*
		 * fridge_maiden
		 * megalich
		 * infinilich
		 */
		public override List<ActorEffectResistance> DebuffImmunities => new List<ActorEffectResistance> { new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.Freeze } };
		public override void Start()
		{
			Timer = 2.5f;
			base.Start();
		}
		public override void OnPreDeath(Vector2 obj)
		{

		}
		public override void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
		{
			if (Timer == 0 | Timer <= 0)
			{
				Timer = 2.5f;
				if (base.aiActor != null && base.aiActor.GetAbsoluteParentRoom() != null)
				{
					UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>().TelefragVFXPrefab, base.aiActor.sprite.WorldCenter, Quaternion.identity);
					CellArea area = base.aiActor.ParentRoom.area;
					Vector2 Center = area.UnitCenter;
					Vector2 b = base.aiActor.specRigidbody.UnitBottomCenter - base.aiActor.transform.position.XY();
					IntVector2? bestRewardLocation = base.aiActor.GetAbsoluteParentRoom().GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
					base.aiActor.transform.position = Pathfinder.GetClearanceOffset(bestRewardLocation.Value, base.aiActor.Clearance).WithY((float)bestRewardLocation.Value.y) - b;
					base.aiActor.specRigidbody.Reinitialize();
				}
			}
		}
		public override void Update()
		{
			base.Update();
			if (base.aiActor)
			{
				if (Timer >= 0) { Timer -= BraveTime.DeltaTime; }
			}
		}
		private float Timer;
	}
}
namespace Planetside
{
	public abstract class BasicEliteType : BraveBehaviour
	{
		public abstract float MovementSpeedMultiplier { get; }
		public abstract float CooldownMultiplier { get; }
		public abstract float HealthMultiplier { get; }
		public abstract List<ActorEffectResistance> DebuffImmunities { get; }
		public abstract Color EliteOutlineColor { get; }
		private ParticleSystem ParticleSystem;
		public abstract Color EliteParticleColor { get; }
		public abstract Color SecondaryEliteParticleColor { get; }
		public abstract List<string> EnemyBlackList {get;}

		public virtual void Start()
		{
			if (!EnemyBlackList.Contains(base.aiActor.EnemyGuid) && EnemyBlackList != null)
			{
				ParticleSystem = StaticVFXStorage.EliteParticleSystem.GetComponent<ParticleSystem>();
				if (base.aiActor != null)
				{
					base.aiActor.MovementSpeed *= MovementSpeedMultiplier;
					base.aiActor.behaviorSpeculator.CooldownScale *= CooldownMultiplier;
					if (!base.aiActor.healthHaver.IsBoss || !base.aiActor.healthHaver.IsSubboss) { base.aiActor.healthHaver.SetHealthMaximum(base.aiActor.healthHaver.GetCurrentHealth() * HealthMultiplier); }

					if (DebuffImmunities != null)
					{
						if (base.aiActor.EffectResistances == null)
						{
							base.aiActor.EffectResistances = new ActorEffectResistance[0];
						}
						base.aiActor.EffectResistances = DebuffImmunities.ToArray();
					}
				}
				base.aiActor.healthHaver.OnPreDeath += OnPreDeath;
				base.aiActor.healthHaver.OnDamaged += OnDamaged;
			}
			else
			{ Destroy(this); }
		}
		public virtual void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection) { }

		public virtual void OnPreDeath(Vector2 obj) { }
		public virtual void Update()
        {
			if (!EnemyBlackList.Contains(base.aiActor.EnemyGuid) && EnemyBlackList != null)
			{
				Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(base.aiActor.sprite);
				if (base.aiActor.healthHaver != null && base.aiActor != null)
				{
					if (!base.aiActor.healthHaver.IsDead && outlineMaterial1 != null)
					{
						outlineMaterial1.SetColor("_OverrideColor", EliteOutlineColor);
					}
				}
				if (base.aiActor.sprite && !GameManager.Instance.IsPaused && (UnityEngine.Random.value > 0.5f))
				{
					Vector3 vector = sprite.WorldBottomLeft.ToVector3ZisY(0);
					Vector3 vector2 = sprite.WorldTopRight.ToVector3ZisY(0);
					Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
					ParticleSystem particleSystem = ParticleSystem;
					var trails = particleSystem.trails;
					trails.worldSpace = false;
					var main = particleSystem.main;
					main.startColor = new ParticleSystem.MinMaxGradient(EliteParticleColor != null ? EliteParticleColor : Color.white, SecondaryEliteParticleColor != null ? SecondaryEliteParticleColor : Color.white);
					ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
					{
						position = position,
						randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
					};
					var emission = particleSystem.emission;
					emission.enabled = false;
					particleSystem.gameObject.SetActive(true);
					particleSystem.Emit(emitParams, 1);
				}
			}			
		}
	}
}
namespace Planetside
{
	public abstract class SpecialEliteType : BraveBehaviour
	{
		public abstract float MovementSpeedMultiplier { get; }
		public abstract float CooldownMultiplier { get; }
		public abstract float HealthMultiplier { get; }
		public abstract List<ActorEffectResistance> DebuffImmunities { get; }
		public abstract Color EliteOutlineColor { get; }
		private ParticleSystem ParticleSystem;
		public abstract Color EliteParticleColor { get; }
		public abstract Color SecondaryEliteParticleColor { get; }
		public abstract List<string> EnemyBlackList { get; }

		public virtual void Start()
        {
			if (!EnemyBlackList.Contains(base.aiActor.EnemyGuid) && EnemyBlackList != null)
			{
				ParticleSystem = StaticVFXStorage.EliteParticleSystem.GetComponent<ParticleSystem>();
				if (base.aiActor != null)
				{
					base.aiActor.MovementSpeed *= MovementSpeedMultiplier;
					base.aiActor.behaviorSpeculator.CooldownScale *= CooldownMultiplier;
					if (!base.aiActor.healthHaver.IsBoss || !base.aiActor.healthHaver.IsSubboss) { base.aiActor.healthHaver.SetHealthMaximum(base.aiActor.healthHaver.GetCurrentHealth() * HealthMultiplier); }

					if (DebuffImmunities != null)
					{
						if (base.aiActor.EffectResistances == null)
						{
							base.aiActor.EffectResistances = new ActorEffectResistance[0];
						}
						base.aiActor.EffectResistances = DebuffImmunities.ToArray();
					}
				}
				base.aiActor.healthHaver.OnPreDeath += OnPreDeath;
				base.aiActor.healthHaver.OnDamaged += OnDamaged;
			}
			else
			{ Destroy(this); }
		}
		public virtual void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection) {}
		public virtual void OnPreDeath(Vector2 obj){}
		public virtual void Update()
		{
			if (!EnemyBlackList.Contains(base.aiActor.EnemyGuid) && EnemyBlackList != null)
			{
				Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(base.aiActor.sprite);
				if (base.aiActor.healthHaver != null && base.aiActor != null)
				{
					if (!base.aiActor.healthHaver.IsDead && outlineMaterial1 != null)
					{
						outlineMaterial1.SetColor("_OverrideColor", EliteOutlineColor);
					}
				}
				if (base.aiActor.sprite && !GameManager.Instance.IsPaused && (UnityEngine.Random.value > 0.5f))
				{
					Vector3 vector = sprite.WorldBottomLeft.ToVector3ZisY(0);
					Vector3 vector2 = sprite.WorldTopRight.ToVector3ZisY(0);
					Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
					ParticleSystem particleSystem = ParticleSystem;
					var trails = particleSystem.trails;
					trails.worldSpace = false;
					var main = particleSystem.main;
					main.startColor = new ParticleSystem.MinMaxGradient(EliteParticleColor != null ? EliteParticleColor : Color.white, SecondaryEliteParticleColor != null ? SecondaryEliteParticleColor : Color.white);
					ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
					{
						position = position,
						randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
					};
					var emission = particleSystem.emission;
					emission.enabled = false;
					particleSystem.gameObject.SetActive(true);
					particleSystem.Emit(emitParams, 1);
				}
			}
				
		}
	}
}


namespace Planetside
{
	public abstract class BossEliteType : BraveBehaviour
	{
		public abstract float MovementSpeedMultiplier { get; }
		public abstract float CooldownMultiplier { get; }
		public abstract float HealthMultiplier { get; }
		public abstract List<ActorEffectResistance> DebuffImmunities { get; }
		public abstract Color EliteOutlineColor { get; }
		private ParticleSystem ParticleSystem;
		public abstract Color EliteParticleColor { get; }
		public abstract Color SecondaryEliteParticleColor { get; }
		public abstract List<string> EnemyBlackList { get; }

		public virtual void Start()
		{
			if (!EnemyBlackList.Contains(base.aiActor.EnemyGuid) && EnemyBlackList != null)
			{
				ParticleSystem = StaticVFXStorage.EliteParticleSystem.GetComponent<ParticleSystem>();
				if (base.aiActor != null)
				{
					base.aiActor.MovementSpeed *= MovementSpeedMultiplier;
					base.aiActor.behaviorSpeculator.CooldownScale *= CooldownMultiplier;
					base.aiActor.healthHaver.SetHealthMaximum(base.aiActor.healthHaver.GetCurrentHealth() * HealthMultiplier);
					if (DebuffImmunities != null)
					{
						if (base.aiActor.EffectResistances == null)
						{
							base.aiActor.EffectResistances = new ActorEffectResistance[0];
						}
						base.aiActor.EffectResistances = DebuffImmunities.ToArray();
					}
				}
				base.aiActor.healthHaver.OnPreDeath += OnPreDeath;
				base.aiActor.healthHaver.OnDamaged += OnDamaged;
			}
			else
			{ Destroy(this); }
		}
		public virtual void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection) { }
		public virtual void OnPreDeath(Vector2 obj) { }
		public virtual void Update()
		{
			if (!EnemyBlackList.Contains(base.aiActor.EnemyGuid) && EnemyBlackList != null)
			{
				Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(base.aiActor.sprite);
				if (base.aiActor.healthHaver != null && base.aiActor != null)
				{
					if (!base.aiActor.healthHaver.IsDead && outlineMaterial1 != null)
					{
						outlineMaterial1.SetColor("_OverrideColor", EliteOutlineColor);
					}
				}
				if (base.aiActor.sprite && !GameManager.Instance.IsPaused && (UnityEngine.Random.value > 0.5f))
				{
					Vector3 vector = sprite.WorldBottomLeft.ToVector3ZisY(0);
					Vector3 vector2 = sprite.WorldTopRight.ToVector3ZisY(0);
					Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
					ParticleSystem particleSystem = ParticleSystem;
					var trails = particleSystem.trails;
					trails.worldSpace = false;
					var main = particleSystem.main;
					main.startColor = new ParticleSystem.MinMaxGradient(EliteParticleColor != null ? EliteParticleColor : Color.white, SecondaryEliteParticleColor != null ? SecondaryEliteParticleColor : Color.white);
					ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
					{
						position = position,
						randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
					};
					var emission = particleSystem.emission;
					emission.enabled = false;
					particleSystem.gameObject.SetActive(true);
					particleSystem.Emit(emitParams, 1);
				}
			}

		}
	}
}


namespace Planetside
{
	public class SpinBulletsController : BraveBehaviour
	{
		public void Start()
		{
			base.aiActor.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;
			m_regenTimer = RegenTimer;
			this.m_bulletBank = this.aiActor.gameObject.GetComponent<AIBulletBank>();
			float num = float.MaxValue;
			if (this.aiActor && this.aiActor.TargetRigidbody)
			{
				num = (this.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox)).magnitude;
			}
			for (int i = 0; i < this.NumBullets; i++)
			{
				float num2 = Mathf.Lerp(this.BulletMinRadius, this.BulletMaxRadius, (float)i / ((float)this.NumBullets - 1f));
				if (num2 * 2f > num)
				{
					for (int e = 0; e < AmountOFLines; e++)

					{
						this.m_projectiles.Add(new SpinBulletsController.ProjectileContainer
						{
							projectile = null,
							angle = (360f / AmountOFLines) * e,
							distFromCenter = num2
						});
					}
				}
				else
				{
					for (int e = 0; e < AmountOFLines; e++)
					{
						GameObject gameObject = this.m_bulletBank.CreateProjectileFromBank(this.GetBulletPosition(0f, num2), (360f / AmountOFLines) * e, this.OverrideBulletName, null, false, true, false);
						Projectile component = gameObject.GetComponent<Projectile>();
						component.specRigidbody.Velocity = Vector2.zero;
						component.ManualControl = true;
						if (this.BulletsIgnoreTiles)
						{
							component.specRigidbody.CollideWithTileMap = false;
						}
						this.m_projectiles.Add(new SpinBulletsController.ProjectileContainer
						{
							projectile = component,
							angle = (360f / AmountOFLines) * e,
							distFromCenter = num2
						});
					}
				}
			}
		}

		private void HealthHaver_OnPreDeath(Vector2 obj)
		{
			IsEnabled = false;
			this.DestroyProjectiles();
		}

		public void Upkeep()
		{
			//base.DecrementTimer(ref this.m_regenTimer, false);
		}

		public void Update()
		{

			RunContinuousUpdate();

		}

		public void RunContinuousUpdate()
		{
			m_regenTimer -= BraveTime.DeltaTime;
			IsEnabled = true;
			if (this.aiActor)
			{
				bool flag = this.aiActor.CanTargetEnemies && !this.aiActor.CanTargetPlayers;
				if (this.m_cachedCharm != flag)
				{
					for (int i = 0; i < this.m_projectiles.Count; i++)
					{
						if (this.m_projectiles[i] != null && this.m_projectiles[i].projectile && this.m_projectiles[i].projectile.gameObject.activeSelf)
						{
							this.m_projectiles[i].projectile.DieInAir(false, false, true, false);
							this.m_projectiles[i].projectile = null;
						}
					}
					this.m_cachedCharm = flag;
				}
			}
			for (int j = 0; j < this.m_projectiles.Count; j++)
			{
				if (!this.m_projectiles[j].projectile || !this.m_projectiles[j].projectile.gameObject.activeSelf)
				{
					this.m_projectiles[j].projectile = null;
				}
			}
			for (int k = 0; k < this.m_projectiles.Count; k++)
			{
				float angle = this.m_projectiles[k].angle + BraveTime.DeltaTime * (float)this.BulletCircleSpeed;
				this.m_projectiles[k].angle = angle;
				Projectile projectile = this.m_projectiles[k].projectile;
				if (projectile)
				{


					Vector2 bulletPosition = this.GetBulletPosition(angle, this.m_projectiles[k].distFromCenter);
					projectile.specRigidbody.Velocity = (bulletPosition - (Vector2)projectile.transform.position) / BraveTime.DeltaTime;
					if (projectile.shouldRotate)
					{
						projectile.transform.rotation = Quaternion.Euler(0f, 0f, 180f + (Quaternion.Euler(0f, 0f, 90f) * (this.ShootPoint.transform.position.XY() - bulletPosition)).XY().ToAngle());
					}

					projectile.ResetDistance();
				}
				else if (this.m_regenTimer <= 0f)
				{
					Vector2 bulletPosition2 = this.GetBulletPosition(this.m_projectiles[k].angle, this.m_projectiles[k].distFromCenter);
					if (GameManager.Instance.Dungeon.CellExists(bulletPosition2) && !GameManager.Instance.Dungeon.data.isWall((int)bulletPosition2.x, (int)bulletPosition2.y))
					{
						GameObject gameObject = this.m_bulletBank.CreateProjectileFromBank(bulletPosition2, 0f, this.OverrideBulletName, null, false, true, false);
						projectile = gameObject.GetComponent<Projectile>();
						projectile.specRigidbody.Velocity = Vector2.zero;
						projectile.ManualControl = true;
						if (this.BulletsIgnoreTiles)
						{
							projectile.specRigidbody.CollideWithTileMap = false;
							projectile.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyBlocker));
							projectile.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle));
							projectile.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.LowObstacle));
							projectile.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.BeamBlocker));
							projectile.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBreakable));
							projectile.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.TileBlocker));
							projectile.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBlocker));
							projectile.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyBulletBlocker));

						}
						this.m_projectiles[k].projectile = projectile;
						this.m_regenTimer = this.RegenTimer;
					}
				}
			}
			for (int l = 0; l < this.m_projectiles.Count; l++)
			{
				if (this.m_projectiles[l] != null && this.m_projectiles[l].projectile)
				{
					bool flag2 = this.aiActor && this.aiActor.CanTargetEnemies;
					this.m_projectiles[l].projectile.collidesWithEnemies = (this.m_projectiles[l].projectile.collidesWithEnemies || flag2);
				}
			}
		}

		public void EndContinuousUpdate()
		{
			IsEnabled = false;
			this.DestroyProjectiles();
		}

		public void StartContinuousUpdate()
		{
			//this.m_updateEveryFrame = true;
		}


		protected override void OnDestroy()
		{
			base.OnDestroy();
			IsEnabled = false;
		}

		private Vector2 GetBulletPosition(float angle, float distFromCenter)
		{
			return this.ShootPoint.transform.position.XY() + BraveMathCollege.DegreesToVector(angle, distFromCenter);
		}

		public void DestroyProjectiles()
		{
			for (int i = 0; i < this.m_projectiles.Count; i++)
			{
				Projectile projectile = this.m_projectiles[i].projectile;
				if (projectile != null)
				{
					projectile.DieInAir(false, true, true, false);
				}
			}
			this.m_projectiles.Clear();
		}

		public bool IsEnabled;

		public string OverrideBulletName;

		public GameObject ShootPoint;

		public int NumBullets;

		public float BulletMinRadius;

		public float BulletMaxRadius;

		public int BulletCircleSpeed;

		public bool BulletsIgnoreTiles;

		public float RegenTimer;

		public float AmountOFLines;

		private readonly List<SpinBulletsController.ProjectileContainer> m_projectiles = new List<SpinBulletsController.ProjectileContainer>();

		private AIBulletBank m_bulletBank;

		private bool m_cachedCharm;

		private float m_regenTimer;

		private class ProjectileContainer
		{
			public Projectile projectile;
			public float angle;
			public float distFromCenter;
		}
	}

}