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
using EnemyBulletBuilder;
using Pathfinding;
using SaveAPI;

namespace Planetside
{
	class Ophanaim : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "Ophanaim";
		public static GameObject shootpoint;

		public static GameObject Centre;
		public static GameObject LeftEye;
		public static GameObject RightEye;

		public static GameObject LeftEyeTop;
		public static GameObject LeftEyeBottom;

		public static GameObject RightEyeTop;
		public static GameObject RightEyeBottom;

		private static tk2dSpriteCollectionData OphanaimCollectiom;


		public static List<int> spriteIds2 = new List<int>();
		public static GameObject OphanaimShiled;

		private static Texture2D BossCardTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside/Resources/BossCards/ophanaim_bosscard.png");

		public static void Init()
		{
			Ophanaim.BuildPrefab();
		}
		public static void BuildPrefab()
		{
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);

			bool flag2 = flag;
			if (!flag2)
			{
				prefab = BossBuilder.BuildPrefab("Ophanaim", guid, "Planetside/Resources/OphanaimNew/ophanaimnew_idle_001.png", new IntVector2(0, 0), new IntVector2(0, 0), false, true);
				var enemy = prefab.AddComponent<EyeEnemyBehavior>();
				AIAnimator aiAnimator = enemy.aiAnimator;

				enemy.aiActor.knockbackDoer.weight = 35;
				enemy.aiActor.MovementSpeed = 2.4f;
				enemy.aiActor.healthHaver.PreventAllDamage = false;
				enemy.aiActor.CollisionDamage = 1f;
				enemy.aiActor.HasShadow = false;
				enemy.aiActor.IgnoreForRoomClear = false;
				enemy.aiActor.aiAnimator.HitReactChance = 0f;
				enemy.aiActor.specRigidbody.CollideWithOthers = true;
				enemy.aiActor.specRigidbody.CollideWithTileMap = true;
				enemy.aiActor.PreventFallingInPitsEver = false;
				enemy.aiActor.healthHaver.ForceSetCurrentHealth(1450f);
				enemy.aiActor.CollisionKnockbackStrength = 10f;
				enemy.aiActor.CanTargetPlayers = true;
				enemy.aiActor.healthHaver.SetHealthMaximum(1450f, null, false);

				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "idle",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};

				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
					{
						"moveright", //Good
						"moveleft",//Good


	                }
				};
				DirectionalAnimation lasercharge = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"lasercharge",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "lasercharge",
						anim = lasercharge
					}
				};
				//=====================================================================================
				DirectionalAnimation laserattack = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"laserattack",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "laserattack",
						anim = laserattack
					}
				};
				//=====================================================================================
				DirectionalAnimation charge1 = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"charge1",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "charge1",
						anim = charge1
					}
				};
				//=====================================================================================
				DirectionalAnimation fire1 = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"fire1",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "fire1",
						anim = fire1
					}
				};
				//=====================================================================================
				DirectionalAnimation SunCharge = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"BigSunCharge",
					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "BigSunCharge",
						anim = SunCharge
					}
				};
				//=====================================================================================
				DirectionalAnimation Sun = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"BigSun",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "BigSun",
						anim = Sun
					}
				};
				//=====================================================================================
				DirectionalAnimation almostdone = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "intro",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "intro",
						anim = almostdone
					}
				};
				DirectionalAnimation done = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "death",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "death",
						anim = done
					}
				};


				bool flag3 = OphanaimCollectiom == null;
				if (flag3)
				{
					OphanaimCollectiom = SpriteBuilder.ConstructCollection(prefab, "OphanaimCollection");
					UnityEngine.Object.DontDestroyOnLoad(OphanaimCollectiom);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], OphanaimCollectiom);
					}
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, OphanaimCollectiom, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8

					}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 10f;

					SpriteBuilder.AddAnimation(enemy.spriteAnimator, OphanaimCollectiom, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6

					}, "moveleft", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, OphanaimCollectiom, new List<int>
					{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8
					}, "moveright", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

					SpriteBuilder.AddAnimation(enemy.spriteAnimator, OphanaimCollectiom, new List<int>
					{

					9,
					10,
					11,
					12

					}, "charge1", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, OphanaimCollectiom, new List<int>
					{

					13,
					14,
					15,
					16,
					17

					}, "fire1", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;

					SpriteBuilder.AddAnimation(enemy.spriteAnimator, OphanaimCollectiom, new List<int>
					{

					18,
					19,
					20,
					21,
					22,
					23,
					24,
					25

					}, "lasercharge", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;

					SpriteBuilder.AddAnimation(enemy.spriteAnimator, OphanaimCollectiom, new List<int>
					{

					26,
					27,
					28,
					29,
					30,
					31,
					32,
					33,
					34,
										26,
					27,
					28,
					29,
					30,
					31,
					32,
					33,
					34,
										26,
					27,
					28,
					29,
					30,
					31,
					32,
					33,
					34,
					26,
					27,
					28,
					29,
					30,
					31,
					32,
					33,
					34,
					26,
					27,
					28,
					29,
					30,
					31,
					32,
					33,
					34,
					26,
					25,
					24,
					23,
					22,
					21,
					20,
					19,
					18
					}, "laserattack", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;


					SpriteBuilder.AddAnimation(enemy.spriteAnimator, OphanaimCollectiom, new List<int>
					{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					62,
					63,
					64,
					65,
					66,
					67,
					68,
					69,
					70,
					71,
					72


					}, "intro", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, OphanaimCollectiom, new List<int>
					{
					73,
					73,
					74,
					74,
					75,
					76,
					77,
					78,
					79,
					80,
					81

					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;

					SpriteBuilder.AddAnimation(enemy.spriteAnimator, OphanaimCollectiom, new List<int>
					{
					35,
					36,
					37,
					38,
					39,
					40,
					41,
					42,
					43,
					44,
					45,
					46,
					47,
					47,
					48,
					48,
					48

					}, "BigSunCharge", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, OphanaimCollectiom, new List<int>
					{
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
						49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					49,
					50,
					51,
					52,
					52,
					53,
					54,
					55,
					56,
					57,
					58,
					59,
					60,
					61

					}, "BigSun", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;

				}

				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[0].eventAudio = "Play_Baboom";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[0].triggerEvent = true;


				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("death").frames[8].eventAudio = "Play_BigSlam";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("death").frames[8].triggerEvent = true;


				var clip = enemy.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro");
				clip.frames[0].eventInfo = "Flash";
				clip.frames[0].triggerEvent = true;

				var clip1 = enemy.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("death");
				clip1.frames[10].eventInfo = "DeathBoom";
				clip1.frames[10].triggerEvent = true;

				var THESUN = enemy.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("BigSunCharge");
				THESUN.frames[1].eventInfo = "SUNTELL";
				THESUN.frames[1].triggerEvent = true;
				THESUN.frames[8].eventInfo = "SUN";
				THESUN.frames[8].triggerEvent = true;
				THESUN.frames[10].eventInfo = "SpawnFire";
				THESUN.frames[10].triggerEvent = true;

				var THESUUN = enemy.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("BigSun");
				THESUUN.frames[1].eventInfo = "StartSun";
				THESUUN.frames[1].triggerEvent = true;
				THESUUN.frames[9].eventInfo = "EndSun";
				THESUUN.frames[9].triggerEvent = true;

				enemy.aiActor.specRigidbody.PixelColliders.Clear();
				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 39,
					ManualOffsetY = 13,
					ManualWidth = 78,
					ManualHeight = 51,
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
					ManualOffsetX = 39,
					ManualOffsetY = 13,
					ManualWidth = 79,
					ManualHeight = 51,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});

				enemy.aiActor.PreventBlackPhantom = false;



				Centre = new GameObject("Center");
				Centre.transform.parent = enemy.transform;
				Centre.transform.position = new Vector2(4.9375f, 2.6825f);
				GameObject TheMainEye = enemy.transform.Find("Center").gameObject;

				LeftEye = new GameObject("LeftEye");
				LeftEye.transform.parent = enemy.transform;
				LeftEye.transform.position = new Vector2(2.875f, 1.25f);
				GameObject TheLeftEye = enemy.transform.Find("LeftEye").gameObject;

				RightEye = new GameObject("RightEye");
				RightEye.transform.parent = enemy.transform;
				RightEye.transform.position = new Vector2(7.0625f, 1.25f);
				GameObject TheRightEye = enemy.transform.Find("RightEye").gameObject;

				var bs = prefab.GetComponent<BehaviorSpeculator>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;

				Vector2 offset = new Vector2(3.125f, 2.5f);
				shootpoint = new GameObject("fuck");
				shootpoint.transform.parent = enemy.transform;
				shootpoint.transform.position = offset;

				LeftEyeTop = new GameObject("LeftEyeTop");
				LeftEyeTop.transform.parent = enemy.transform;
				LeftEyeTop.transform.position = new Vector2(2.875f, 1.4375f);
				GameObject TheLeftEyeTop = enemy.transform.Find("LeftEyeTop").gameObject;

				LeftEyeBottom = new GameObject("LeftEyeBottom");
				LeftEyeBottom.transform.parent = enemy.transform;
				LeftEyeBottom.transform.position = new Vector2(2.875f, 1.0625f);
				GameObject TheLeftEyeBottom = enemy.transform.Find("LeftEyeBottom").gameObject;

				RightEyeTop = new GameObject("RightEyeTop");
				RightEyeTop.transform.parent = enemy.transform;
				RightEyeTop.transform.position = new Vector2(7.0625f, 1.4375f);
				GameObject TheRightEyeTop = enemy.transform.Find("RightEyeTop").gameObject;
				RightEyeBottom = new GameObject("RightEyeBottom");
				RightEyeBottom.transform.parent = enemy.transform;
				RightEyeBottom.transform.position = new Vector2(7.0625f, 1.0625f);
				GameObject TheRightEyeBottom = enemy.transform.Find("RightEyeBottom").gameObject;



				AIBeamShooter aIBeamShooter = prefab.GetOrAddComponent<AIBeamShooter>();
				AIActor actor = EnemyDatabase.GetOrLoadByGuid("21dd14e5ca2a4a388adab5b11b69a1e1");
				AIBeamShooter aIBeamShooter2 = actor.GetComponent<AIBeamShooter>();
				aIBeamShooter.beamTransform = LeftEyeTop.transform;
				aIBeamShooter.beamModule = aIBeamShooter2.beamModule;
				aIBeamShooter.beamProjectile = aIBeamShooter2.beamProjectile;
				aIBeamShooter.name = "LeftEyeTop";
				aIBeamShooter.PreventBeamContinuation = true;

				AIBeamShooter aIBeamShooter11 = prefab.AddComponent<AIBeamShooter>();
				AIBeamShooter aIBeamShooter2A = actor.GetComponent<AIBeamShooter>();
				aIBeamShooter11.beamTransform = LeftEyeBottom.transform;
				aIBeamShooter11.beamModule = aIBeamShooter2A.beamModule;
				aIBeamShooter11.beamProjectile = aIBeamShooter2A.beamProjectile;
				aIBeamShooter11.name = "LeftEyeBottom";
				aIBeamShooter11.PreventBeamContinuation = true;

				AIBeamShooter aIBeamShooter1 = prefab.AddComponent<AIBeamShooter>();
				AIBeamShooter aIBeamShooter21 = actor.GetComponent<AIBeamShooter>();
				aIBeamShooter1.beamTransform = RightEyeBottom.transform;
				aIBeamShooter1.beamModule = aIBeamShooter21.beamModule;
				aIBeamShooter1.beamProjectile = aIBeamShooter21.beamProjectile;
				aIBeamShooter1.name = "RightEyeBottom";
				aIBeamShooter1.PreventBeamContinuation = true;

				AIBeamShooter aIBeamShooterH = prefab.AddComponent<AIBeamShooter>();
				AIBeamShooter aIBeamShooterN = actor.GetComponent<AIBeamShooter>();
				aIBeamShooterH.beamTransform = RightEyeTop.transform;
				aIBeamShooterH.beamModule = aIBeamShooterN.beamModule;
				aIBeamShooterH.beamProjectile = aIBeamShooterN.beamProjectile;
				aIBeamShooterH.name = "RightEyeTop";
				aIBeamShooterH.PreventBeamContinuation = true;

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
						Probability = 1f,
						Behavior = new ShootBehavior()
						{
							TellAnimation = "charge1",
							FireAnimation = "fire1",
							BulletScript = new CustomBulletScriptSelector(typeof(Ophanaim.FireCircles)),
							LeadAmount = 0,
							//StopDuring = ShootBehavior.StopType.Attack,
							AttackCooldown = 1f,
							Cooldown = 3f,

							RequiresLineOfSight = true,
							ShootPoint = TheMainEye,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 0,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
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
							MoveSpeedModifier = 0.8f,


						},
						NickName = "dasddsasad"
					},
				new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 1f,
						Behavior = new ShootBehavior()
						{
							TellAnimation = "charge1",
							FireAnimation = "fire1",
							BulletScript = new CustomBulletScriptSelector(typeof(Ophanaim.BouncyCicleTheShoot)),
							LeadAmount = 0,
							//StopDuring = ShootBehavior.StopType.Attack,
							AttackCooldown = 1.5f,
							Cooldown = 4f,

							RequiresLineOfSight = true,
							ShootPoint = TheMainEye,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 0,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
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
						NickName = "firefsss"
					},
				new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 1.2f,
						Behavior = new ShootBehavior()
						{
							TellAnimation = "charge1",
							FireAnimation = "fire1",
							BulletScript = new CustomBulletScriptSelector(typeof(Ophanaim.MithrixSlam)),
							LeadAmount = 0,
							//StopDuring = ShootBehavior.StopType.Attack,
							AttackCooldown = 1f,
							Cooldown = 6f,

							RequiresLineOfSight = true,
							ShootPoint = TheMainEye,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 0,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
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
							MoveSpeedModifier = 0.6f,
						},
						NickName = "mithrix"
					},
					//=============

					new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 0.85f,
						Behavior = new ShootBehavior()
						{
							TellAnimation = "charge1",
							FireAnimation = "fire1",
							BulletScript = new CustomBulletScriptSelector(typeof(Ophanaim.FireWalls)),
							LeadAmount = 0,
							//StopDuring = ShootBehavior.StopType.Attack,
							AttackCooldown = 3f,
							Cooldown = 12f,

							RequiresLineOfSight = true,
							ShootPoint = TheMainEye,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 0,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
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
							MoveSpeedModifier = 0.6f,

						},
						NickName = "Why"
					},
					/*
				new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 1f,
						Behavior = new ShootBehavior()
						{
							TellAnimation = "charge1",
							FireAnimation = "fire1",
							BulletScript = new CustomBulletScriptSelector(typeof(Ophanaim.SummonSmallSuns)),
							LeadAmount = 0,
							//StopDuring = ShootBehavior.StopType.Attack,
							AttackCooldown = 1f,
							Cooldown = 3f,

							RequiresLineOfSight = true,
							ShootPoint = TheMainEye,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 0,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
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
						NickName = "LiteralFireWall"
					},
				*/
					new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 0.85f,
						Behavior = new ShootBehavior()
						{
							TellAnimation = "lasercharge",
							FireAnimation = "laserattack",
							BulletScript = new CustomBulletScriptSelector(typeof(Ophanaim.SomeBigAttackThatGetsReplacedInPhase2)),
							LeadAmount = 0,
							StopDuring = ShootBehavior.StopType.Attack,
							AttackCooldown = 3f,
							Cooldown = 12f,

							RequiresLineOfSight = true,
							ShootPoint = TheMainEye,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 0,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
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
						NickName = "TheAttackThatGetsReplacedInPhase2"
					},
					new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 0f,
						Behavior = new ShootBehavior()
						{
							TellAnimation = "BigSunCharge",
							FireAnimation = "BigSun",
							BulletScript = new CustomBulletScriptSelector(typeof(Ophanaim.THESUN)),
							LeadAmount = 0,
							StopDuring = ShootBehavior.StopType.Charge,
							AttackCooldown = 4f,
							Cooldown = 60f,

							RequiresLineOfSight = true,
							ShootPoint = TheMainEye,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 0,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
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
							MoveSpeedModifier = 0f,
						},
						NickName = "TheSun"
					},
					new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 1f,
						Behavior = new MetalGearRatBeamsBehavior()
						{
							TellAnimation = "lasercharge",
							FireAnimation = "laserattack",
							AttackCooldown = 2,


						//beamSelection = ShootBeamBehavior.BeamSelection.Specify,
						firingTime = 6f,
						stopWhileFiring = false,
						//restrictBeamLengthToAim = false,
						RequiresLineOfSight = true,
						specificBeamShooters = new List<AIBeamShooter>{ aIBeamShooter, aIBeamShooter11, aIBeamShooter1, aIBeamShooterH },
						BulletScript = new CustomBulletScriptSelector(typeof(Ophanaim.TheFlameThing)),
						Cooldown = 7,
						slitherPeriod = 0.5f,
						slitherMagnitude = 12,
						targetMoveSpeed = 5,
						targetMoveAcceleration = 0.25f,
						randomTargets = 1,
						randomRetargetMax = 2,
						randomRetargetMin = 2,
						ShootPoint = Centre.transform,

						},
						NickName = "Beams nuts"
					},

				};
				bs.MovementBehaviors = new List<MovementBehaviorBase>() {
				new SeekTargetBehavior() {
					StopWhenInRange = false,
					CustomRange = 6,
					LineOfSight = true,
					ReturnToSpawn = true,
					SpawnTetherDistance = 0,
					PathInterval = 0.5f,
					SpecifyRange = false,
					MinActiveRange = -0.25f,
					MaxActiveRange = 0
				}
				};
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:ophanaim", enemy.aiActor);
				var nur = enemy.aiActor;
				nur.EffectResistances = new ActorEffectResistance[]
				{
					new ActorEffectResistance()
					{
						resistAmount = 1,
						resistType = EffectResistanceType.Fire
					},
				};

				//SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Coallet/Idle/coallet_idle_006", SpriteBuilder.ammonomiconCollection);
				if (enemy.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
				}
				GenericIntroDoer miniBossIntroDoer = prefab.AddComponent<GenericIntroDoer>();
				prefab.AddComponent<FungannonIntroController>();

				miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
				miniBossIntroDoer.initialDelay = 0.5f;
				miniBossIntroDoer.cameraMoveSpeed = 10;
				miniBossIntroDoer.specifyIntroAiAnimator = null;
				miniBossIntroDoer.BossMusicEvent = "Play_MUS_Boss_Theme_Beholster";
				//miniBossIntroDoer.BossMusicEvent = "Play_MUS_Lich_Double_01";
				miniBossIntroDoer.PreventBossMusic = false;
				miniBossIntroDoer.InvisibleBeforeIntroAnim = true;
				miniBossIntroDoer.preIntroAnim = string.Empty;
				miniBossIntroDoer.preIntroDirectionalAnim = string.Empty;
				miniBossIntroDoer.introAnim = "intro";
				miniBossIntroDoer.introDirectionalAnim = string.Empty;
				miniBossIntroDoer.continueAnimDuringOutro = false;
				miniBossIntroDoer.cameraFocus = null;
				miniBossIntroDoer.roomPositionCameraFocus = Vector2.zero;
				miniBossIntroDoer.restrictPlayerMotionToRoom = false;
				miniBossIntroDoer.fusebombLock = false;
				miniBossIntroDoer.AdditionalHeightOffset = 0;
				PlanetsideModule.Strings.Enemies.Set("#OPHANAIM_NAME", "OPHANAIM");
				PlanetsideModule.Strings.Enemies.Set("#OPHANAIM_NAME_SMALL", "Ophanaim");

				PlanetsideModule.Strings.Enemies.Set("AAAAAAA", "ETERNAL EYE");
				PlanetsideModule.Strings.Enemies.Set("#QUOTE", "");
				enemy.aiActor.OverrideDisplayName = "#OPHANAIM_NAME_SMALL";

				miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
				{
					bossNameString = "#OPHANAIM_NAME",
					bossSubtitleString = "AAAAAAA",
					bossQuoteString = "#QUOTE",
					bossSpritePxOffset = IntVector2.Zero,
					topLeftTextPxOffset = IntVector2.Zero,
					bottomRightTextPxOffset = IntVector2.Zero,
					bgColor = Color.blue
				};
				if (BossCardTexture)
				{
					miniBossIntroDoer.portraitSlideSettings.bossArtSprite = BossCardTexture;
					miniBossIntroDoer.SkipBossCard = false;
					enemy.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
				}
				else
				{
					miniBossIntroDoer.SkipBossCard = true;
					enemy.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
				}
				
				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Ammocom/ophanaimbossiconpng", SpriteBuilder.ammonomiconCollection);
				if (enemy.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
				}
				enemy.encounterTrackable = enemy.gameObject.AddComponent<EncounterTrackable>();
				enemy.encounterTrackable.journalData = new JournalEntry();
				enemy.encounterTrackable.EncounterGuid = "psog:ophanaim";
				enemy.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				enemy.encounterTrackable.journalData.SuppressKnownState = false;
				enemy.encounterTrackable.journalData.IsEnemy = true;
				enemy.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				enemy.encounterTrackable.ProxyEncounterGuid = "";
				enemy.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Ammocom/ophanaimbossiconpng";
				enemy.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\ophanaimsheetrt.png");
				PlanetsideModule.Strings.Enemies.Set("#OPHANAIMAMMONOMICON", "Ophanaim");
				PlanetsideModule.Strings.Enemies.Set("#OPHANAIMAMMONOMICONSHORT", "Eternal Eye");
				PlanetsideModule.Strings.Enemies.Set("#OPHANAIMAMMONOMICONLONG", "A colossal, angelic eye empowered by the light of the star that Gunymede orbits.\n\nThe Ophanaim, one of many, leads the forces of the ocular sentinels that have surged into the Gungeon.");
				enemy.encounterTrackable.journalData.PrimaryDisplayName = "#OPHANAIMAMMONOMICON";
				enemy.encounterTrackable.journalData.NotificationPanelDescription = "#OPHANAIMAMMONOMICONSHORT";
				enemy.encounterTrackable.journalData.AmmonomiconFullEntry = "#OPHANAIMAMMONOMICONLONG";
				EnemyBuilder.AddEnemyToDatabase(enemy.gameObject, "psog:ophanaim");
				EnemyDatabase.GetEntry("psog:ophanaim").ForcedPositionInAmmonomicon = 8;
				EnemyDatabase.GetEntry("psog:ophanaim").isInBossTab = true;
				EnemyDatabase.GetEntry("psog:ophanaim").isNormalEnemy = true;
				


				//==================
				ImprovedAfterImage yeah = enemy.aiActor.gameObject.AddComponent<ImprovedAfterImage>();
				yeah.dashColor = Color.white;
				yeah.spawnShadows = false;
				yeah.shadowTimeDelay = 0.033f;
				yeah.shadowLifetime = 0.5f;


				//==================
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(enemy.aiActor.healthHaver);
				//==================
				GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/ophanaim_ring", null, true);
				gameObject.SetActive(false);
				FakePrefab.MarkAsFakePrefab(gameObject);
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				GameObject gameObject2 = new GameObject("OphanaimHolyShield");
				tk2dSprite tk2dSprite = gameObject2.AddComponent<tk2dSprite>();
				tk2dSprite.SetSprite(gameObject.GetComponent<tk2dBaseSprite>().Collection, gameObject.GetComponent<tk2dBaseSprite>().spriteId);

				Ophanaim.spriteIds2.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/ophanaim_ring", tk2dSprite.Collection));

				Ophanaim.spriteIds2.Add(tk2dSprite.spriteId);
				gameObject2.SetActive(false);


				tk2dSprite.SetSprite(Ophanaim.spriteIds2[0]);
				tk2dSprite.sprite.usesOverrideMaterial = true;
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = tk2dSprite.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
				mat.SetFloat("_EmissiveColorPower", 1.55f);
				mat.SetFloat("_EmissivePower", 100);

				FakePrefab.MarkAsFakePrefab(gameObject2);
				UnityEngine.Object.DontDestroyOnLoad(gameObject2);
				Ophanaim.OphanaimShiled = gameObject2;
				//==================
			}
		}


		private static string[] spritePaths = new string[]
		{
			//Idle
			"Planetside/Resources/OphanaimNew/ophanaimnew_idle_001.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_idle_002.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_idle_003.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_idle_004.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_idle_005.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_idle_006.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_idle_007.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_idle_008.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_idle_009.png",

			//Charge1
			"Planetside/Resources/OphanaimNew/ophanaimnew_charge1_001.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_charge1_002.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_charge1_003.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_charge1_004.png",
			//Fire1
			"Planetside/Resources/OphanaimNew/ophanaimnew_fire1_001.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_fire1_002.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_fire1_003.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_fire1_004.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_fire1_005.png",
			//Laser Charge
			"Planetside/Resources/OphanaimNew/ophanaimnew_lasercharge_001.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_lasercharge_002.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_lasercharge_003.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_lasercharge_004.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_lasercharge_005.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_lasercharge_006.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_lasercharge_007.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_lasercharge_008.png",
			//Laser Fire
			"Planetside/Resources/OphanaimNew/ophanaimnew_laserfire_001.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_laserfire_002.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_laserfire_003.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_laserfire_004.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_laserfire_005.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_laserfire_006.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_laserfire_007.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_laserfire_008.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_laserfire_009.png",
			//Sun Flare Charge
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_001.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_002.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_003.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_004.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_005.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_006.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_007.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_008.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_009.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_010.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_011.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_012.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_013.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflarecharge_014.png",
			//Sun Flare Blast
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_001.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_002.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_003.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_004.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_005.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_006.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_007.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_008.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_009.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_010.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_011.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_012.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_sunflareblast_013.png",
			//Intro
			"Planetside/Resources/OphanaimNew/ophanaimnew_intro_001.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_intro_002.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_intro_003.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_intro_004.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_intro_005.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_intro_006.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_intro_007.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_intro_008.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_intro_009.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_intro_010.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_intro_011.png",
			//Death
			"Planetside/Resources/OphanaimNew/ophanaimnew_death_001.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_death_002.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_death_003.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_death_004.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_death_005.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_death_006.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_death_007.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_death_008.png",
			"Planetside/Resources/OphanaimNew/ophanaimnew_death_009.png"

		};
		public class EyeEnemyBehavior : BraveBehaviour
		{

			public List<GameObject> extantReticles = new List<GameObject>();

			public GameObject LaserShit;

			public float distortionMaxRadius = 30f;
			public float distortionDuration = 2f;
			public float distortionIntensity = 0.7f;
			public float distortionThickness = 0.1f;
			private RoomHandler m_StartRoom;
			public void Update()
			{
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				if (!base.aiActor.HasBeenEngaged)
				{
					CheckPlayerRoom();
				}
				bool flag = base.aiActor && base.aiActor.healthHaver;
				if (flag)
				{
					float maxHealth = base.aiActor.healthHaver.GetMaxHealth();
					float num = maxHealth * 0.5f;
					float currentHealth = base.aiActor.healthHaver.GetCurrentHealth();
					bool flag2 = currentHealth < num;
					if (flag2)
					{
						if (Phase2Check != true)
						{
							for (int i = 0; i < StaticReferenceManager.AllGoops.Count; i++)
							{
								DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = StaticReferenceManager.AllGoops[i];
								deadlyDeadlyGoopManager.RemoveGoopCircle(base.aiActor.sprite.WorldCenter, 25);
							}
							StaticReferenceManager.DestroyAllEnemyProjectiles();
							Phase2Check = true;

							base.aiActor.MovementSpeed = TurboModeController.MaybeModifyEnemyMovementSpeed(3f);

							Exploder.DoDistortionWave(base.aiActor.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);

							ImprovedAfterImage yeah = base.aiActor.gameObject.GetComponent<ImprovedAfterImage>();
							yeah.dashColor = Color.white;
							yeah.spawnShadows = true;
							yeah.shadowTimeDelay = 0.02f;
							yeah.shadowLifetime = 0.2f;

							float FlashTime = 0.05f;
							float FlashFadetime = 0.20f;
							Pixelator.Instance.FadeToColor(FlashFadetime, Color.white, true, FlashTime);
							StickyFrictionManager.Instance.RegisterCustomStickyFriction(0.15f, 1f, false, false); FlashTime = 1f;
							base.aiActor.behaviorSpeculator.Interrupt();
							AkSoundEngine.PostEvent("Play_EyeRoar", base.gameObject);
							for (int i = 0; i < Ophanaim.reticles.Count; i++)
							{
								SpawnManager.Despawn(Ophanaim.reticles[i]);
								Destroy(Ophanaim.reticles[i]);
							}
							Ophanaim.reticles.Clear();
							base.aiActor.behaviorSpeculator.AttackCooldown *= 0.8f;
							for (int j = 0; j < base.aiActor.behaviorSpeculator.AttackBehaviors.Count; j++)
							{
								if (base.behaviorSpeculator.AttackBehaviors[j] is AttackBehaviorGroup && base.behaviorSpeculator.AttackBehaviors[j] != null)
								{
									this.ProcessAttackGroup(base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup);
								}
							}

						}
					}
				}

			}
			private void ProcessAttackGroup(AttackBehaviorGroup attackGroup)
			{
				for (int i = 0; i < attackGroup.AttackBehaviors.Count; i++)
				{
					AttackBehaviorGroup.AttackGroupItem attackGroupItem = attackGroup.AttackBehaviors[i];
					if (attackGroupItem.Behavior is ShootBehavior && attackGroup != null && attackGroupItem.NickName == "TheAttackThatGetsReplacedInPhase2")
					{
						attackGroupItem.Probability = 0f;
					}
					else if (attackGroupItem.Behavior is ShootBehavior && attackGroup != null && attackGroupItem.NickName == "TheSun")
					{
						attackGroupItem.Probability = 7f;
					}
				}
			}
			public static bool Phase2Check;
			private void CheckPlayerRoom()
			{
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom && GameManager.Instance.PrimaryPlayer.IsInCombat == true)
				{
					GameManager.Instance.StartCoroutine(LateEngage());
				}
				else
				{
					base.aiActor.HasBeenEngaged = false;
				}
			}
			private IEnumerator LateEngage()
			{
				yield return new WaitForSeconds(0.5f);
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
				{
					base.aiActor.HasBeenEngaged = true;
				}
				yield break;
			}

			public void Start()
			{
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(base.aiActor.healthHaver);
				Phase2Check = false;
				if (!base.aiActor.IsBlackPhantom)
				{
					Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
					mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
					mat.SetColor("_EmissiveColor", new Color32(255, 210, 178, 255));
					mat.SetFloat("_EmissiveColorPower", 1.55f);
					mat.SetFloat("_EmissivePower", 100);
					mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);

					aiActor.sprite.renderer.material = mat;

				}
				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered; 
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("383175a55879441d90933b5c4e60cf6f").bulletBank.GetBullet("bigBullet"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").bulletBank.GetBullet("teeth_football"));
				base.aiActor.HasBeenEngaged = false;
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					AkSoundEngine.PostEvent("Stop_Burn", base.gameObject);
					EyeEnemyBehavior controller = base.aiActor.GetComponent<EyeEnemyBehavior>();
					if (controller != null)
					{
						for (int i = 0; i < controller.extantReticles.Count; i++)
						{
							SpawnManager.Despawn(controller.extantReticles[i]);
							Destroy(controller.extantReticles[i]);
						}
						controller.extantReticles.Clear();
					}
				};
				base.aiActor.healthHaver.OnDeath += (obj) =>
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_OPHANAIM, true);
				};
			}

			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo == "SUNTELL")
				{
					AkSoundEngine.PostEvent("Play_Baboom", base.gameObject);
				}
				if (clip.GetFrame(frameIdx).eventInfo == "Flash")
				{
					float FlashTime = 0.25f;
					float FlashFadetime = 0.75f;
					Pixelator.Instance.FadeToColor(FlashFadetime, Color.white, true, FlashTime);
					StickyFrictionManager.Instance.RegisterCustomStickyFriction(0.15f, 1f, false, false); FlashTime = 0.1f;
				}
				if (clip.GetFrame(frameIdx).eventInfo == "DeathBoom")
				{
					float FlashTime = 1f;
					float FlashFadetime = 2f;
					Pixelator.Instance.FadeToColor(FlashFadetime, Color.white, true, FlashTime);
					StickyFrictionManager.Instance.RegisterCustomStickyFriction(0.15f, 1f, false, false); FlashTime = 0.1f;
					GameObject epicwin = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").GetComponent<BossFinalRogueDeathController>().DeathStarExplosionVFX);
					epicwin.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(base.aiActor.sprite.WorldCenter, tk2dBaseSprite.Anchor.LowerCenter);
					epicwin.transform.position = base.aiActor.sprite.WorldCenter.Quantize(0.0625f);
					epicwin.GetComponent<tk2dBaseSprite>().UpdateZDepth();
					for (int i = 0; i < StaticReferenceManager.AllGoops.Count; i++)
					{
						DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = StaticReferenceManager.AllGoops[i];
						deadlyDeadlyGoopManager.RemoveGoopCircle(base.aiActor.sprite.WorldCenter, 25);
					}

				}
				if (clip.GetFrame(frameIdx).eventInfo == "SUN")
				{
					for (int i = 0; i < StaticReferenceManager.AllGoops.Count; i++)
                    {
						DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = StaticReferenceManager.AllGoops[i];
						deadlyDeadlyGoopManager.RemoveGoopCircle(base.aiActor.sprite.WorldCenter, 40);
					}
					StaticReferenceManager.DestroyAllEnemyProjectiles();
					CellArea area = base.aiActor.ParentRoom.area;
					Vector2 Center = area.UnitCenter;
					//AkSoundEngine.PostEvent("Play_Baboom", base.gameObject);
					Vector2 b = base.aiActor.specRigidbody.UnitBottomCenter - base.aiActor.transform.position.XY();
					IntVector2? CentreCell = Center.ToIntVector2();
					base.aiActor.transform.position = Pathfinder.GetClearanceOffset(CentreCell.Value - new IntVector2(2 , 0), base.aiActor.Clearance).WithY((float)CentreCell.Value.y) - b;
					base.aiActor.specRigidbody.Reinitialize();
					float FlashTime = 0.125f;
					float FlashFadetime = 0.375f;
					Pixelator.Instance.FadeToColor(FlashFadetime, Color.white, true, FlashTime);
					StickyFrictionManager.Instance.RegisterCustomStickyFriction(0.15f, 1f, false, false); FlashTime = 0.1f;
				}
				if (clip.GetFrame(frameIdx).eventInfo == "StartSun")
				{
					base.aiActor.healthHaver.AllDamageMultiplier = 0.5f;
					base.StartCoroutine(this.unmatchedpowerofthesun());

				}
				if (clip.GetFrame(frameIdx).eventInfo == "EndSun")
				{
					base.aiActor.healthHaver.AllDamageMultiplier = 1f;
				}
			}
			private IEnumerator unmatchedpowerofthesun()
			{

				
				Material targetMaterial = base.aiAnimator.sprite.renderer.material;
				float ela = 0f;
				float dura = 1f;
				while (ela < dura)
				{
					ela += BraveTime.DeltaTime;
					base.aiActor.sprite.renderer.material.SetColor("_EmissiveColor", new Color32(255, 210, 178, 255));
					base.aiActor.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
					base.aiActor.sprite.renderer.material.SetFloat("_EmissivePower", 100+(ela*400));
					base.aiActor.sprite.renderer.material.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
					yield return null;
				}
				yield return new WaitForSeconds(10f);
				ela = 0f;
				dura = 4f;
				while (ela < dura)
				{
					base.aiActor.sprite.renderer.material.SetColor("_EmissiveColor", new Color32(255, 210, 178, 255));
					base.aiActor.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
					base.aiActor.sprite.renderer.material.SetFloat("_EmissivePower", 500 - (ela * 100));
					base.aiActor.sprite.renderer.material.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
					yield return null;
				}
				Pixelator.Instance.DeregisterAdditionalRenderPass(Sun);
				yield break;
			}

		}
		public static Material Sun = new Material(ShaderCache.Acquire("Brave/LitCutoutUber"));

		private static List<GameObject> reticles = new List<GameObject>();


		//Basic Attack
		public class TheFlameThing : Script
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				float aim = base.AimDirection;
				for (int i = 0; i < 360; i++)
				{
					float newAim = base.AimDirection;
					aim = Mathf.MoveTowardsAngle(aim, newAim, 1f);

					if (i % 10 == 0)
                    {
						for (int j = 0; j < 3; j++)
                        {
							base.Fire(new Direction(120*j + (i*2), Brave.BulletScript.DirectionType.Aim, -1f), new Speed(0, SpeedType.Absolute), new TheFlameThing.Flames1());
						}
						base.PostWwiseEvent("Play_BOSS_doormimic_flame_01", null);
					}
					float Amount = 60;
					if (Ophanaim.EyeEnemyBehavior.Phase2Check == true)
					{
						Amount = 40;
					}

					if (i % Amount == 0)
					{
						float radius = 0.0166f;
						if (Ophanaim.EyeEnemyBehavior.Phase2Check == true)
						{
							radius = 0.0233f;
						}
						float delta = 30f;
						for (int j = 0; j < 12; j++)
						{
							base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
							base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new TheFlameThing.TheGear("frogger", this, aim + (float)j * delta, radius));
						}

					}

					yield return base.Wait(1);
				}
				yield break;

				
				
			}
			public class FirehoseBullet : Bullet
			{
				public FirehoseBullet(float direction) : base("frogger", false, false, false)
				{
					this.m_direction = direction;
				}

				protected override IEnumerator Top()
				{
					yield return base.Wait(UnityEngine.Random.Range(5, 30));
					this.Direction += this.m_direction * UnityEngine.Random.Range(10f, 25f);
					yield break;
				}
				private float m_direction;
			}
			public class Flames1 : Bullet
			{
				public Flames1() : base("frogger", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(9f, SpeedType.Absolute), 90);
					yield return base.Wait(900);
					yield break;
				}
			}
			public class TheGear : Bullet
			{
				public TheGear(string BulletType, TheFlameThing parent, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
				{
					this.m_parent = parent;
					this.m_angle = angle;
					this.m_radius = aradius;
					this.m_bulletype = BulletType;
					this.SuppressVfx = true;
				}

				protected override IEnumerator Top()
				{
					base.ManualControl = true;
					Vector2 centerPosition = base.Position;
					float radius = 0f;
					this.m_spinSpeed = 40f;
					for (int i = 0; i < 300; i++)
					{
						if (i == 40)
						{
							base.ChangeSpeed(new Speed(11f, SpeedType.Absolute), 150);
							base.ChangeDirection(new Direction(this.m_parent.GetAimDirection(1f, 10f), Brave.BulletScript.DirectionType.Absolute, -1f), 20);
							base.StartTask(this.ChangeSpinSpeedTask(180f, 240));
						}
						base.UpdateVelocity();
						centerPosition += this.Velocity / 60f;
						//if (i < 40)
						{
							radius += 0.0066f;
						}
						this.m_angle += this.m_spinSpeed / 60f;
						base.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}

				private IEnumerator ChangeSpinSpeedTask(float newSpinSpeed, int term)
				{
					float delta = (newSpinSpeed - this.m_spinSpeed) / (float)term;
					for (int i = 0; i < term; i++)
					{
						this.m_spinSpeed += delta;
						yield return base.Wait(1);
					}
					yield break;
				}
				private const float ExpandSpeed = 4.5f;
				private const float SpinSpeed = 40f;
				private TheFlameThing m_parent;
				private float m_angle;
				private float m_spinSpeed;
				private float m_radius;
				private string m_bulletype;

			}
		}
		//Attack For Special Beam Attack


		//Attack that fires 5 sets of curvy bullets
		public class FireCircles : Script
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1a4872dafdb34fd29fe8ac90bd2cea67").bulletBank.bulletBank.GetBullet("default"));

				base.EndOnBlank = true;

				float floatDirection = base.AimDirection + UnityEngine.Random.Range(10f, -10f);
				Vector2 floatVelocity = BraveMathCollege.DegreesToVector(floatDirection, 5f);
				base.PostWwiseEvent("Play_BigSlam", null);
				for (int j = 0; j < 20; j++)
				{
					this.Fire(new Offset(new Vector2(2.125f, -1.3125f)), new Direction(this.SubdivideCircle(0, 20, j, 1f, false), Brave.BulletScript.DirectionType.Aim, -1f), new Speed(5f, SpeedType.Absolute), new FireCircles.BurstBullet("frogger", floatVelocity));
				}
				yield return base.Wait(60f);
				floatDirection = base.AimDirection + UnityEngine.Random.Range(10f, -10f);
				base.PostWwiseEvent("Play_BigSlam", null);
				for (int j = 0; j < 20; j++)
				{
					this.Fire(new Offset(new Vector2(-2.125f, -1.3125f)), new Direction(this.SubdivideCircle(0, 20, j, 1f, false), Brave.BulletScript.DirectionType.Aim, -1f), new Speed(5f, SpeedType.Absolute), new FireCircles.BurstBullet("frogger", floatVelocity));
				}
				if (Ophanaim.EyeEnemyBehavior.Phase2Check == true)
				{
					yield return base.Wait(60f);
					floatDirection = base.AimDirection + UnityEngine.Random.Range(10f, -10f);
					base.PostWwiseEvent("Play_BigSlam", null);
					for (int j = 0; j < 20; j++)
					{
						this.Fire(new Direction(this.SubdivideCircle(0, 20, j, 1f, false), Brave.BulletScript.DirectionType.Aim, -1f), new Speed(4f, SpeedType.Absolute), new FireCircles.BurstBullet("frogger", floatVelocity));
					}
					yield return base.Wait(60f);
				}
				yield break;
			}

			public class BurstBullet : Bullet
			{
				public BurstBullet(string bulletname, Vector2 additionalVelocity) : base(bulletname, true, false, false)
				{
					this.m_addtionalVelocity = additionalVelocity;
				}

				protected override IEnumerator Top()
				{
					this.ManualControl = true;
					for (int i = 0; i < 300; i++)
					{
						this.UpdateVelocity();
						this.Velocity += this.m_addtionalVelocity * Mathf.Min(10f, (float)i / 60f);
						this.UpdatePosition();
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}

				private Vector2 m_addtionalVelocity;
			}
			public class Break : Bullet
			{
				public Break(Vector2 centerPoint, bool speeen, float startAngle, float spinspeed) : base("spore1", false, false, false)
				{
					this.centerPoint = centerPoint;
					this.yesToSpeenOneWay = speeen;
					this.startAngle = startAngle;
					this.SpinSpeed = spinspeed;
				}
				protected override IEnumerator Top()
				{
					base.ManualControl = true;
					float radius = Vector2.Distance(this.centerPoint, base.Position);
					float speed = this.Speed;
					float spinAngle = this.startAngle;
					float spinSpeed = 0f;
					for (int i = 0; i < 250; i++)
					{
						//speed += 0.00333f;
						radius += speed / 60f;
						if (yesToSpeenOneWay == true)
						{
							spinSpeed -= SpinSpeed / 0.8f;
						}
						else
						{
							spinSpeed += SpinSpeed / 0.8f;

						}
						spinAngle += spinSpeed / 60f;
						base.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
						base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(0, SpeedType.Absolute), new Flames());
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}
				public Vector2 centerPoint;
				public bool yesToSpeenOneWay;
				public float startAngle;
				public float SpinSpeed;
			}
			public class Flames : Bullet
			{
				public Flames() : base("spore1", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					yield return base.Wait(30);
					base.Vanish(false);
					yield break;
				}
			}
			public class Flames1 : Bullet
			{
				public Flames1() : base("frogger", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					yield return base.Wait(900);
					yield break;
				}
			}
		}
		public class BouncyCicleTheShoot : Script
		{
			protected override IEnumerator Top()
			{


					
					DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(base.BulletBank.aiActor.sprite.WorldCenter, 25);
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
					base.EndOnBlank = true;
					float startAngle = base.RandomAngle();
					float delta = 12f;
					float Amount = 30;

				base.PostWwiseEvent("Play_BigSlam", null);
					for (int A = 0; A < Amount; A++)
					{
					float num = startAngle + (float)A * delta;

					base.Fire(new Direction(num, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(7, SpeedType.Absolute), new BouncyCicleTheShoot.WaveBullet());
					if (Ophanaim.EyeEnemyBehavior.Phase2Check == true)
                    {
						base.Fire(new Direction(num, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(3, SpeedType.Absolute), new BouncyCicleTheShoot.WaveBullet());

					}
				}


				//yield return base.Wait(1);
				for (int A = 0; A < 20; A++)
					{
						base.PostWwiseEvent("Play_BOSS_doormimic_flame_01", null);
						base.Fire(new Offset(new Vector2(-2.125f, -1.3125f)), new Direction(UnityEngine.Random.Range(-30, 30), Brave.BulletScript.DirectionType.Aim, -1f), new Speed(6, SpeedType.Absolute), new BouncyCicleTheShoot.Fard());
						base.Fire(new Offset(new Vector2(2.125f, -1.3125f)), new Direction(UnityEngine.Random.Range(-30, 30), Brave.BulletScript.DirectionType.Aim, -1f), new Speed(6, SpeedType.Absolute), new BouncyCicleTheShoot.Fard());
						yield return base.Wait(10);
					}

				startAngle = base.RandomAngle();

				yield break;
			}
			public class Fard : Bullet
			{
				public Fard() : base("frogger", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					yield break;
				}
			}
			public class Flames1 : Bullet
			{
				public Flames1() : base("frogger", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					yield break;
				}
			}

			private class WaveBullet : Bullet
			{
				public WaveBullet() : base("frogger", false, false, false)
				{
				}

				protected override IEnumerator Top()
				{
					base.ManualControl = true;
					Vector2 truePosition = base.Position;
					//float Amount = 1.66f;
					for (int e = 0; e < 8; e++)
					{
						for (int i = 0; i < 75; i++)
						{
							base.UpdateVelocity();
							truePosition += this.Velocity / 60f;
							base.Position = truePosition + new Vector2(0f, Mathf.Sin((float)base.Tick / 60f / 0.75f * 3.14159274f) * 1.5f);
							yield return base.Wait(1);
						}

						//if (Ophanaim.EyeEnemyBehavior.Phase2Check == true)
						//{
							//DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.FireDef).TimedAddGoopCircle(base.Projectile.sprite.WorldCenter, Amount, 1f, false);
						//}
					}
					base.Vanish(false);
					yield break;
				}

				private const float SinPeriod = 0.75f;
				private const float SinMagnitude = 1.5f;
			}
		}
		public class FireWalls : Script
		{
			private const int SpinTime = 450;
			protected override IEnumerator Top()
			{
				float Amount = 8;
				if (Ophanaim.EyeEnemyBehavior.Phase2Check == true)
				{
					Amount = 12;
				}
				for (int j = 0; j < Amount; j++)
				{
					base.PostWwiseEvent("Play_BOSS_doormimic_flame_01", null);
					Exploder.DoDistortionWave(base.BulletBank.sprite.WorldCenter, 3, 0.02f, 15, 0.3f);
					base.Fire(new Direction((UnityEngine.Random.Range(-180, 180)), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(9, 16), SpeedType.Absolute), new FireWalls.Superball());
					yield return base.Wait(450 / Amount);
				}
				yield break;
			}
			public class Fard : Bullet
			{
				public Fard() : base("frogger", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					yield break;
				}
			}
			public class Superball : Bullet
			{
				public Superball() : base("big", false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
					{
						base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
					}
					for (int i = 0; i < 90; i++)
					{
						float Speed = base.Speed;
						base.ChangeSpeed(new Speed(Speed * 0.98f, SpeedType.Absolute), 0);
						yield return this.Wait(1);
					}

					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 1);
					base.Vanish(true);
					yield break;
				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (!preventSpawningProjectiles)
					{
						base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);
						float Fart = UnityEngine.Random.Range(-180, 180);
						if (Ophanaim.EyeEnemyBehavior.Phase2Check == true)
						{
							for (int j = 0; j < 6; j++)
							{
								base.Fire(new Direction((60 * j), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SomeBigAttackThatGetsReplacedInPhase2.Break(base.Position, true, (60 * j), 0.06f));
								base.Fire(new Direction((60 * j), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SomeBigAttackThatGetsReplacedInPhase2.Break(base.Position, false, (60 * j), 0.06f));

							}
						}
						else
                        {
							for (int j = 0; j < 12; j++)
							{
								base.Fire(new Direction((30 * j), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SomeBigAttackThatGetsReplacedInPhase2.Break(base.Position, true, (30 * j), 0.04f));
								base.Fire(new Direction((30 * j), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SomeBigAttackThatGetsReplacedInPhase2.Break(base.Position, false, (30 * j), 0.04f));
							}
						}



						for (int j = 0; j < 15; j++)
						{
							for (int e = 0; e < 4; e++)
                            {
								this.Fire(new Offset(new Vector2(UnityEngine.Random.Range(-0.125f, 0.125f), UnityEngine.Random.Range(-0.5f, 0.5f))), new Direction(90 * e, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(8.5f, SpeedType.Relative), new Break(j*5f));
							}
						}
						return;
					}
				}

				public class Break : Bullet
				{
					public Break(float aaaa) : base("frogger", false, false, false)
					{
						this.Delay = aaaa;
					}
					protected override IEnumerator Top()
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 0);
						yield return base.Wait(Delay);
						base.ChangeSpeed(new Speed(8, SpeedType.Absolute), 0);
						yield break;
					}
					private float Delay;
				}
			}
		}

		public class SomeBigAttackThatGetsReplacedInPhase2 : Script
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				base.PostWwiseEvent("Play_EyeRoar", null);
				float RNG = UnityEngine.Random.Range(0, 60);
				Exploder.DoDistortionWave(base.BulletBank.sprite.WorldCenter, 2, 0.05f, 10, 0.7f);
				yield return base.Wait(15);
				for (int E = 0; E < 6; E++)
                {
					base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
					for (int n = 0; n < 12; n++)
					{
						base.Fire(new Direction((30 * n)+ (E*2), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SomeBigAttackThatGetsReplacedInPhase2.Break(base.Position, false, (30 * n), 0.08f));
					}
					yield return base.Wait(15);
					base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
					for (int n = 0; n < 12; n++)
					{
						base.Fire(new Direction((30 * n) + (E * 2), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SomeBigAttackThatGetsReplacedInPhase2.Flames1());
					}
					yield return base.Wait(15);
					base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
					for (int n = 0; n < 12; n++)
					{
						base.Fire(new Direction((30 * n) + (E * 2), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SomeBigAttackThatGetsReplacedInPhase2.Break(base.Position, true, (30 * n), 0.08f));
					}
					yield return base.Wait(15);
					base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
					for (int n = 0; n < 12; n++)
					{
						base.Fire(new Direction((30 * n) + (E * 2), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SomeBigAttackThatGetsReplacedInPhase2.Flames1());
					}
					yield return base.Wait(15);
				}

				
				yield break;
			}

			public class Break : Bullet
			{
				public Break(Vector2 centerPoint, bool speeen, float startAngle, float spinspeed) : base("frogger", false, false, false)
				{
					this.centerPoint = centerPoint;
					this.yesToSpeenOneWay = speeen;
					this.startAngle = startAngle;
					this.SpinSpeed = spinspeed;
				}
				protected override IEnumerator Top()
				{
					base.ManualControl = true;
					float radius = Vector2.Distance(this.centerPoint, base.Position);
					float speed = this.Speed;
					float spinAngle = this.startAngle;
					float spinSpeed = 0f;
					for (int i = 0; i < 600; i++)
					{
						//speed += 0.08333f;
						radius += speed / 60f;
						if (yesToSpeenOneWay == true)
						{
							spinSpeed -= SpinSpeed;
						}
						else
						{
							spinSpeed += SpinSpeed;

						}
						spinAngle += spinSpeed / 60f;
						base.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}
				public Vector2 centerPoint;
				public bool yesToSpeenOneWay;
				public float startAngle;
				public float SpinSpeed;
			}
			public class Flames1 : Bullet
			{
				public Flames1() : base("frogger", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					yield return base.Wait(900);
					yield break;
				}
			}
			public class SnakeBullet : Bullet
			{
				public SnakeBullet(int delay) : base("teeth_football", false, false, false)
				{
					this.delay = delay;
				}

				protected override IEnumerator Top()
				{
					yield return base.Wait(this.delay);
					base.ChangeSpeed(new Speed(9f, SpeedType.Absolute), 0);
					yield break;
				}
				private int delay;
			}
		}

		public class THESUN : Script
		{
			protected override IEnumerator Top()
			{
				Exploder.DoDistortionWave(base.BulletBank.sprite.WorldCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("383175a55879441d90933b5c4e60cf6f").bulletBank.GetBullet("bigBullet"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("suck"));

				for (int E = 0; E < 4; E++)
                {
					//base.Fire(new Direction(90*E, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(15, SpeedType.Absolute), new THESUN.FireThatSpawnsFireGoop());
				}
				base.PostWwiseEvent("Play_Burn", null);
				for (int R = 0; R < 6; R++)
                {
					{
						for (int E = 0; E < 30; E++)
						{
							yield return base.Wait(5f);

							base.PostWwiseEvent("Play_BOSS_doormimic_flame_01", null);
							for (int n = 0; n < 3; n++)
							{
								base.Fire(new Direction((120 * n) + (12f * E), Brave.BulletScript.DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new THESUN.Break(base.Position, false, (120 * n) + (12f * E), 0.0125f));
								base.Fire(new Direction((120 * n) - (12f * E) + 60, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new THESUN.Break(base.Position, true, (120 * n) + (12f * E) + 60, -0.025f));
								//base.Fire(new Direction((120 * n) - (6f * E) + 40, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(6.5f, SpeedType.Absolute), new THESUN.Break(base.Position, true, (120 * n) + (12f * E) + 40, 0.005f));
							}
						}
					}
					base.PostWwiseEvent("Play_BigSlam", null);
					base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(6.5f, SpeedType.Absolute), new THESUN.FireThatSpawnsFireGoop());
					base.Fire(new Direction(120, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(6.5f, SpeedType.Absolute), new THESUN.FireThatSpawnsFireGoop());
					base.Fire(new Direction(240, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(6.5f, SpeedType.Absolute), new THESUN.FireThatSpawnsFireGoop());

				}
				base.PostWwiseEvent("Stop_Burn", null);
				yield break;
			}
			public float distortionMaxRadius = 50f;
			public float distortionDuration = 1f;
			public float distortionIntensity = 0.2f;
			public float distortionThickness = 0.3f;
			public class Flames1 : Bullet
			{
				public Flames1() : base("frogger", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					yield return base.Wait(900);
					yield break;
				}
			}
			public class Break : Bullet
			{
				public Break(Vector2 centerPoint, bool speeen, float startAngle, float spinspeed) : base("frogger", false, false, false)
				{
					this.centerPoint = centerPoint;
					this.yesToSpeenOneWay = speeen;
					this.startAngle = startAngle;
					this.SpinSpeed = spinspeed;
				}
				protected override IEnumerator Top()
				{
					base.ManualControl = true;
					float radius = Vector2.Distance(this.centerPoint, base.Position);
					float speed = this.Speed;
					float spinAngle = this.startAngle;
					float spinSpeed = 0f;
					for (int i = 0; i < 600; i++)
					{
						//speed += 0.08333f;
						radius += speed / 60f;
						if (yesToSpeenOneWay == true)
						{
							spinSpeed -= SpinSpeed;
						}
						else
						{
							spinSpeed += SpinSpeed;

						}
						spinAngle += spinSpeed / 60f;
						base.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}
				public Vector2 centerPoint;
				public bool yesToSpeenOneWay;
				public float startAngle;
				public float SpinSpeed;
			}

			public class aaa : Bullet
			{
				public aaa() : base("suck", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					this.Projectile.spriteAnimator.Play("enemy_projectile_small_fire_tell");
					yield return base.Wait(900);
					yield break;
				}
			}
			public class FireThatSpawnsFireGoop : Bullet
			{
				public FireThatSpawnsFireGoop() : base("bigBullet", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					base.Projectile.ImmuneToSustainedBlanks = true;
					for (int j = 0; j < 300; j++)
					{
						DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.FireDef).TimedAddGoopCircle(base.Projectile.sprite.WorldCenter, 1, 1f, false);
						yield return base.Wait(1);
					}
					yield break;
				}
			}
		}

		public class MithrixSlam : Script
		{
			private const int SpinTime = 450;
			protected override IEnumerator Top()
			{
				CellArea area = base.BulletBank.aiActor.ParentRoom.area;
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("383175a55879441d90933b5c4e60cf6f").bulletBank.GetBullet("bigBullet"));
				base.PostWwiseEvent("Play_EyeRoar", null);
				yield return this.Wait(30f);
				float Amount = 4;
				float WaitTime = 75;
				float delta = 30;
				float Lines = 12;


				if (Ophanaim.EyeEnemyBehavior.Phase2Check == true)
				{
					Amount = 5;
					WaitTime = 60f;
					delta = 20;
					Lines = 18;
				}

				for (int e = 0; e < Amount; e++)
                {
					List<float> log = new List<float>()
					{


					};
					EyeEnemyBehavior vfx = base.BulletBank.GetComponent<EyeEnemyBehavior>();
					vfx.LaserShit = EnemyDatabase.GetOrLoadByGuid("6868795625bd46f3ae3e4377adce288b").GetComponent<ResourcefulRatController>().ReticleQuad;
					//vfx.name = "LaserTellEye" + j.ToString();
					float fuck = (UnityEngine.Random.Range(-180, 180));
					for (int i = 0; i < Lines; i++)
					{
						float num2 = delta;
						float angle = base.AimDirection + fuck + (delta * i);
						Vector2 zero = Vector2.zero;
						if (BraveMathCollege.LineSegmentRectangleIntersection(this.Position, this.Position + BraveMathCollege.DegreesToVector(angle, 60f), new Vector2(-40, -40), new Vector2(40, 40), ref zero))
						{
							num2 = (zero - this.Position).magnitude;
						}
						if (vfx == null)
						{
							vfx.LaserShit = EnemyDatabase.GetOrLoadByGuid("6868795625bd46f3ae3e4377adce288b").GetComponent<ResourcefulRatController>().ReticleQuad;
							//ETGModConsole.Log("TRIPPED NULLCHECK");
						}
						GameObject gameObject = SpawnManager.SpawnVFX(vfx.LaserShit, false);
						tk2dSlicedSprite component2 = gameObject.GetComponent<tk2dSlicedSprite>();
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);// + BraveMathCollege.DegreesToVector(angle, 2f).ToVector3ZUp(0);
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
						component2.dimensions = new Vector2((num2) * 50f, 5f);
						component2.UpdateZDepth();

						component2.HeightOffGround = -2;
						component2.gameObject.layer = 28;

						component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
						component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 0);
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);

						vfx.extantReticles.Add(gameObject);
						log.Add(angle);

					}
					bool LeftOrRight = (UnityEngine.Random.value > 0.5f) ? false : true;
					float AddOnNum = 0;
					base.PostWwiseEvent("Play_BOSS_lichB_charge_02", null);
					for (int i = 0; i < 60; i++)
					{
						foreach (GameObject obj in vfx.extantReticles)
						{
							tk2dSlicedSprite component2 = obj.GetComponent<tk2dSlicedSprite>();
							float ix = component2.transform.localRotation.eulerAngles.x;
							float wai = component2.transform.localRotation.eulerAngles.y;
							if (base.BulletBank.aiActor != null)
                            {
								ix = component2.transform.localRotation.eulerAngles.x + base.BulletBank.aiActor.transform.localRotation.x;
								wai = component2.transform.localRotation.eulerAngles.y + base.BulletBank.aiActor.transform.localRotation.y;
							}
							float zee = component2.transform.localRotation.eulerAngles.z;
							component2.transform.position.WithZ(component2.transform.position.z + 99999);
							component2.transform.position = this.Position.ToVector3ZisY(0);
							component2.HeightOffGround = -2;
							component2.gameObject.layer = 28;

							if (i < 20 || i == 20)
                            {
								component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
								component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
								component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * i);
								component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
							}
							if (i > 20)
                            {
								component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
								component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
								component2.sprite.renderer.material.SetFloat("_EmissivePower", 200 + (i - 20) * 200);
								component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f + 1 / (i-20));
							}
							if (LeftOrRight == true)
							{
								component2.transform.localRotation = Quaternion.Euler(ix, wai, zee + ((3f - (i / 20) + 3600)));
								AddOnNum += (3f - (i / 20));// - (i / 16);
							}
							else
							{
								component2.transform.localRotation = Quaternion.Euler(ix, wai, zee - ((3f - (i / 20) + 3600)));
								AddOnNum -= (3f - (i / 20));// - (i / 16);
							}
							component2.UpdateZDepth();
						}
						yield return this.Wait(1f);

					}
					yield return this.Wait(15f);
					this.CleanupReticles();
					base.PostWwiseEvent("Play_BigSlam", null);
					base.PostWwiseEvent("Play_BOSS_spacebaby_explode_01", null);

					Exploder.DoDistortionWave(base.BulletBank.aiActor.transform.position, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
					foreach (float h in log)
					{
						base.Fire(new Direction(h + AddOnNum, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(50f, SpeedType.Absolute), new MithrixSlam.Flameert());
					}
					for (int i = 0; i < 60; i++)
                    {
						base.Fire(new Direction(6*i, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new MithrixSlam.BasicFlame());
					}
					yield return this.Wait(WaitTime);
				}

				yield break;
			}

			public float distortionMaxRadius = 30f;
			public float distortionDuration = 0.5f;
			public float distortionIntensity = 0.9f;
			public float distortionThickness = 0.4f;
			public void CleanupReticles()
			{
				EyeEnemyBehavior controller = base.BulletBank.aiActor.GetComponent<EyeEnemyBehavior>();
				if (controller)
				{
					for (int i = 0; i < controller.extantReticles.Count; i++)
					{
						SpawnManager.Despawn(controller.extantReticles[i]);
						Destroy(controller.extantReticles[i]);
					}
					controller.extantReticles.Clear();
				}
			}
			public class BasicFlame : Bullet
			{
				public BasicFlame() : base("frogger", false, false, false)
				{

				}
			}
			public class Flameert : Bullet
			{
				public Flameert() : base("frogger", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
					for (int i = 0; i < 200; i++)
					{
						base.Fire(new Direction(UnityEngine.Random.Range(140, 200), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(0f, SpeedType.Absolute), new MithrixSlam.Flames1(80 - i));
						yield return this.Wait(1f);

					}
					yield break;
				}
			}
			public class Flames1 : Bullet
			{
				public Flames1(float timetilldie) : base("frogger", false, false, false)
				{
					TimeTillDeath = timetilldie;
				}

				protected override IEnumerator Top()
				{
					this.ManualControl = true;
					Vector2 truePosition = this.Position;

					for (int i = 0; i < 80-TimeTillDeath; i++)
					{
						this.UpdateVelocity();
						truePosition += this.Velocity / 30f;
						this.Position = truePosition + new Vector2(0f, Mathf.Sin((float)this.Tick / 30f / 0.75f * 3.14159274f) * 1.5f);
						yield return this.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}
				private float TimeTillDeath;
			}
		}

	}
}

