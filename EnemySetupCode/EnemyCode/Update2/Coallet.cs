using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using GungeonAPI;
using SpriteBuilder = ItemAPI.SpriteBuilder;
using DirectionType = DirectionalAnimation.DirectionType;
using static DirectionalAnimation;

namespace Planetside
{
	class Coallet : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "coallet_psog";
		public static GameObject shootpoint;
		private static tk2dSpriteCollectionData CoalletCollection;

		public static void Init()
		{
			Coallet.BuildPrefab();
		}
		public static void BuildPrefab()
		{
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);

			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Coallet", guid, "Planetside/Resources/Enemies/Coallet/Idle/coallet_idle_001", new IntVector2(0, 0), new IntVector2(0, 0), false, true);
				var enemy = prefab.AddComponent<EnemyBehavior>();
				CoalletController pain = prefab.AddComponent<CoalletController>();
				enemy.aiActor.knockbackDoer.weight = 35;
				enemy.aiActor.MovementSpeed = 2f;
				enemy.aiActor.healthHaver.PreventAllDamage = false;
				enemy.aiActor.CollisionDamage = 1f;
				enemy.aiActor.HasShadow = false;
				enemy.aiActor.IgnoreForRoomClear = false;
				enemy.aiActor.aiAnimator.HitReactChance = 0f;
				enemy.aiActor.specRigidbody.CollideWithOthers = true;
				enemy.aiActor.specRigidbody.CollideWithTileMap = true;
				enemy.aiActor.PreventFallingInPitsEver = true;
				enemy.aiActor.healthHaver.ForceSetCurrentHealth(25f);
				enemy.aiActor.CollisionKnockbackStrength = 10f;
				enemy.aiActor.CanTargetPlayers = true;
				enemy.aiActor.healthHaver.SetHealthMaximum(25f, null, false);


				AIAnimator aiAnimator = enemy.aiAnimator;
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "die",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{

						   "die_left",
						   "die_right"

							}

						}
					}
				};
				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
					{
						"idle_left",
						"idle_right"
					}
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
						{
						"run_left",
						"run_right"
						}
				};

				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "runfire",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{
					   "runfire_left",
					   "runfire_right",

							}
						}
					}
				};
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);
				enemy.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				bool flag3 = CoalletCollection == null;
				if (flag3)
				{
					CoalletCollection = SpriteBuilder.ConstructCollection(prefab, "CoalletCollection");
					UnityEngine.Object.DontDestroyOnLoad(CoalletCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], CoalletCollection);
					}
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, CoalletCollection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, CoalletCollection, new List<int>
					{
					0,
					1,
					2,
					3,
					4,
					5

					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, CoalletCollection, new List<int>
					{

					6,
					7,
					8,
					9,
					10,
					11


					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, CoalletCollection, new List<int>
					{


					6,
					7,
					8,
					9,
					10,
					11


					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, CoalletCollection, new List<int>
					{

				 12,
				 13,
				 14,
				 15,
				 16,
				 17

					}, "runfire_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, CoalletCollection, new List<int>
					{
				 12,
				 13,
				 14,
				 15,
				 16,
				 17

					}, "runfire_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, CoalletCollection, new List<int>
					{

				 18,
				 19,
				 20,
				 21,
				 22,
				 23,
				 24,
				 25




					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, CoalletCollection, new List<int>
					{

				 18,
				 19,
				 20,
				 21,
				 22,
				 23,
				 24,
				 25

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, CoalletCollection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5

					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;

				}

				//enemy.aiAnimator.AssignDirectionalAnimation("runfireidle", firewalkidle, AnimationType.Other);

				pain.overrideMoveSpeed = 4f;
				pain.overridePauseTime = 1f;
				//pain.overrideAnimation = "runfireidle";


				enemy.aiActor.specRigidbody.PixelColliders.Clear();
				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 0,
					ManualOffsetY = 0,
					ManualWidth = 13,
					ManualHeight = 14,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0
				});

				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{

					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyHitBox,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 0,
					ManualOffsetY = 0,
					ManualWidth = 13,
					ManualHeight = 14,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});

				enemy.aiActor.PreventBlackPhantom = false;

				//AIAnimator aiAnimator = enemy.aiAnimator;
				shootpoint = new GameObject("fuck");
				shootpoint.transform.parent = enemy.transform;
				shootpoint.transform.position = new Vector3(0,0);
				GameObject shootpoint1 = enemy.transform.Find("fuck").gameObject;

				var bs = prefab.GetComponent<BehaviorSpeculator>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;

				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				bs.TargetBehaviors = new List<TargetBehaviorBase>
				{
					new TargetPlayerBehavior
					{
						Radius = 45f,
						LineOfSight = true,
						ObjectPermanence = true,
						SearchInterval = 0.25f,
						PauseOnTargetSwitch = false,
						PauseTime = 0.25f
					},

				};

				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>()
				{

					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 0f,
						Behavior = new ShootBehavior()
						{
							
							BulletScript = new CustomBulletScriptSelector(typeof(BurnScript)),
							LeadAmount = 0,
							AttackCooldown = 1f,
							RequiresLineOfSight = true,
							ShootPoint = shootpoint1,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 100,
							MinWallDistance = 0,
							MaxEnemiesInRoom = -1,
							MinHealthThreshold = 0,
							MaxHealthThreshold = 1,
							HealthThresholds = new float[0],
							AccumulateHealthThresholds = true,
							targetAreaStyle = null,
							IsBlackPhantom = false,
							resetCooldownOnDamage = null,
							MaxUsages = 0,

						},
						NickName = "Cry About It"
					},
					
				};
				bs.MovementBehaviors = new List<MovementBehaviorBase>
				{

				new MoveErraticallyBehavior
				{
				   PointReachedPauseTime = 2f,
					PathInterval = 0.4f,
					PreventFiringWhileMoving = false,
					StayOnScreen = false,
					AvoidTarget = true,

				}
				};
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:coallet", enemy.aiActor);

				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Coallet/Idle/coallet_idle_006", SpriteBuilder.ammonomiconCollection);
				if (enemy.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
				}
				enemy.encounterTrackable = enemy.gameObject.AddComponent<EncounterTrackable>();
				enemy.encounterTrackable.journalData = new JournalEntry();
				enemy.encounterTrackable.EncounterGuid = "psog:coallet";
				enemy.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				enemy.encounterTrackable.journalData.SuppressKnownState = false;
				enemy.encounterTrackable.journalData.IsEnemy = true;
				enemy.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				enemy.encounterTrackable.ProxyEncounterGuid = "";
				enemy.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Coallet/Idle/coallet_idle_006";
				enemy.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("coalleticon");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\coalleticon.png");
                PlanetsideModule.Strings.Enemies.Set("#COALLET", "Coallet");
				PlanetsideModule.Strings.Enemies.Set("#COALLET_SHORTDESC", "Just For Me");
				PlanetsideModule.Strings.Enemies.Set("#COALLET_LONGDESC", "A tiny coaler from the freshly excavated veins of the Black Powder Mines. It's spends its time sleeping, but can erupt in flame just like its larger cousins.");
				enemy.encounterTrackable.journalData.PrimaryDisplayName = "#COALLET";
				enemy.encounterTrackable.journalData.NotificationPanelDescription = "#COALLET_SHORTDESC";
				enemy.encounterTrackable.journalData.AmmonomiconFullEntry = "#COALLET_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(enemy.gameObject, "psog:coallet");
				EnemyDatabase.GetEntry("psog:coallet").ForcedPositionInAmmonomicon = 70;
				EnemyDatabase.GetEntry("psog:coallet").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:coallet").isNormalEnemy = true;
			}

		}

		private static string[] spritePaths = new string[]
		{
			"Planetside/Resources/Enemies/Coallet/Idle/coallet_idle_001.png",
			"Planetside/Resources/Enemies/Coallet/Idle/coallet_idle_002.png",
			"Planetside/Resources/Enemies/Coallet/Idle/coallet_idle_003.png",
			"Planetside/Resources/Enemies/Coallet/Idle/coallet_idle_004.png",
			"Planetside/Resources/Enemies/Coallet/Idle/coallet_idle_005.png",
			"Planetside/Resources/Enemies/Coallet/Idle/coallet_idle_006.png",

			"Planetside/Resources/Enemies/Coallet/Walk/coallet_walk_001.png",
			"Planetside/Resources/Enemies/Coallet/Walk/coallet_walk_002.png",
			"Planetside/Resources/Enemies/Coallet/Walk/coallet_walk_003.png",
			"Planetside/Resources/Enemies/Coallet/Walk/coallet_walk_004.png",
			"Planetside/Resources/Enemies/Coallet/Walk/coallet_walk_005.png",
			"Planetside/Resources/Enemies/Coallet/Walk/coallet_walk_006.png",

			"Planetside/Resources/Enemies/Coallet/FireWalk/coallet_firewalk_001.png",
			"Planetside/Resources/Enemies/Coallet/FireWalk/coallet_firewalk_002.png",
			"Planetside/Resources/Enemies/Coallet/FireWalk/coallet_firewalk_003.png",
			"Planetside/Resources/Enemies/Coallet/FireWalk/coallet_firewalk_004.png",
			"Planetside/Resources/Enemies/Coallet/FireWalk/coallet_firewalk_005.png",
			"Planetside/Resources/Enemies/Coallet/FireWalk/coallet_firewalk_006.png",

			"Planetside/Resources/Enemies/Coallet/Death/coallet_die_001.png",
			"Planetside/Resources/Enemies/Coallet/Death/coallet_die_002.png",
			"Planetside/Resources/Enemies/Coallet/Death/coallet_die_003.png",
			"Planetside/Resources/Enemies/Coallet/Death/coallet_die_004.png",
			"Planetside/Resources/Enemies/Coallet/Death/coallet_die_005.png",
			"Planetside/Resources/Enemies/Coallet/Death/coallet_die_006.png",
			"Planetside/Resources/Enemies/Coallet/Death/coallet_die_007.png",
			"Planetside/Resources/Enemies/Coallet/Death/coallet_die_008.png",

		};


		public class EnemyBehavior : BraveBehaviour
		{
			//RIP
		}





		public class BurnScript : Script
		{
			protected override IEnumerator Top()
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody) { base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("05891b158cd542b1a5f3df30fb67a7ff").bulletBank.GetBullet("default")); }
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				int i = 0;
				for (; ; )
				{
					base.PostWwiseEvent("Play_BOSS_doormimic_flame_01", null);
					base.Fire(new Direction(UnityEngine.Random.Range(-180, 180)), new Speed(UnityEngine.Random.Range(6f, 8f), SpeedType.Absolute), new Flames());
					yield return this.Wait(16f);
					i++;
				}
			}
		}
		public class Flames : Bullet
		{
			public Flames() : base("frogger", false, false, false)
			{

			}

			protected override IEnumerator Top()
			{
				float speed = base.Speed;
				base.ChangeSpeed(new Speed(speed * 1.5f, SpeedType.Absolute), 60);
				yield break;
			}
		}
	}
}

