using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;	
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;
using Random = System.Random;
using Pathfinding;


namespace Planetside
{

	public class PrisonerPhaseOne : AIActor
	{
		public static GameObject fuckyouprefab;
		public static readonly string guid = "Prisoner_Cloaked";
		private static tk2dSpriteCollectionData PrisonerPhaseOneSpriteCollection;
		public static GameObject shootpoint;
		private static Texture2D BossCardTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside/Resources/BossCards/prisonerbosscard.png");
		public static string TargetVFX;
		public static Texture _gradTexture;




		public static GameObject PrisonerChainVFX;
		public static void Init()
		{
			PrisonerPhaseOne.BuildPrefab();
			PrisonerPhaseOne.BuildChain();
		}

		public static void BuildChain()
        {
			GameObject prisonerChain = OtherTools.MakeLine("Planetside/Resources/Bosses/Prisoner/chain", new Vector2(10, 8), new Vector2(0, 0), new List<string> { "Planetside/Resources/Bosses/Prisoner/chain"});
			prisonerChain.SetActive(false);
			FakePrefab.MarkAsFakePrefab(prisonerChain);
			UnityEngine.Object.DontDestroyOnLoad(prisonerChain);
			PrisonerPhaseOne.PrisonerChainVFX = prisonerChain;
		}

		public static void BuildPrefab()
		{
			bool flag = fuckyouprefab != null || BossBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				fuckyouprefab = BossBuilder.BuildPrefab("Prisoner Phase One", guid, spritePaths[0], new IntVector2(15, 4), new IntVector2(34, 51), false, true);
				var companion = fuckyouprefab.AddComponent<PrisonerController>();
				companion.aiActor.knockbackDoer.weight = 200;
				companion.aiActor.MovementSpeed = 3.2f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0.05f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(1400f);
				companion.aiActor.healthHaver.SetHealthMaximum(1400f);
				companion.aiActor.CollisionKnockbackStrength = 2f;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.HasShadow = true;
				companion.aiActor.SetIsFlying(true, "Gamemode: Creative");
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.gameObject.AddComponent<AfterImageTrailController>().spawnShadows = false;
				companion.aiActor.gameObject.AddComponent<tk2dSpriteAttachPoint>();
				companion.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();
				companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("4db03291a12144d69fe940d5a01de376").ShadowObject;
				companion.aiActor.HasShadow = true;
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider

				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 15,
					ManualOffsetY = 4,
					ManualWidth = 34,
					ManualHeight = 51,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0
				});
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{

					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyHitBox,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 15,
					ManualOffsetY = 4,
					ManualWidth = 34,
					ManualHeight = 51,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;
				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "idle",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "introidle", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "intro", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "death", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "chargelaser", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "firelaser", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "postlaser", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "raisearm", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "sweengarm", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "lowerarm", new string[1], new DirectionalAnimation.FlipType[1]);

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "swipehandback", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "swipehandcharge", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "swipehandmoveback", new string[1], new DirectionalAnimation.FlipType[1]);

				//sweengarm

				bool flag3 = PrisonerPhaseOneSpriteCollection == null;
				if (flag3)
				{
					PrisonerPhaseOneSpriteCollection = SpriteBuilder.ConstructCollection(fuckyouprefab, "Prisoner Phase One Collection");
					UnityEngine.Object.DontDestroyOnLoad(PrisonerPhaseOneSpriteCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], PrisonerPhaseOneSpriteCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
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
					9,
					10
					

					}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
					{

					11,
					12,
					13,
					12,

					}, "introidle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 4f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
					{

					11,
					11,
					11,
					11,
					12,
					12,
					12,
					12,

					11,
					11,
					11,
					11,
					12,
					12,
					12,
					12,
					11,
					11,
					11,
					11,
					12,
					12,
					12,
					12,
					11,
					11,
					11,
					11,
					12,
					12,
					12,
					12,

					13,
					13,
					13,
					13,
					12,
					12,
					12,
					12,



					14,
					14,
					15,
					15,
					15,
					16,
					16,
					17,
					17,
					18,//ChainBreak1 (49)
					19,
					19,
					
					20,
					20,
					19,
					
					20,
					21,
					21,
					21,
					22,
					22,
					23,
					23,
					24,//Chainbreak2 (63)
					25,
					26,
					27,
					28,
					29,
					30,
					31,
					32,
					33,
					34,

					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,

					35,
					36,
					37,
					35,
					36,
					37,
					35,
					36,
					37,
					35,
					36,
					37,

					}, "intro", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
					{
					38,
					39,
					40,
					41,
					42,
					43,
					44,
					}, "chargelaser", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
					{
					45,
					46,
					47
					}, "firelaser", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
					{
					44,
					43,
					41,
					39,
					38
					}, "postlaser", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
					{
					48,
					49,
					50,
					51,
					51,
					52,
					52
					}, "raisearm", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
					{
					53,
					54,
					55,
					56
					}, "sweengarm", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
					{
					52,
					51,
					50,
					49,
					48
					}, "lowerarm", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					
					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
					{
					57,
					58,
					59,
					59,
					60,
					60
					}, "swipehandback", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
					{
					61,
					60,

					}, "swipehandcharge", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
					{
					62,
					63,
					64,
					65,
					66,
					67
					}, "swipehandmoveback", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;

					EnemyToolbox.AddSoundsToAnimationFrame(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "intro", new Dictionary<int, string> { { 47, "Play_BOSS_lichA_crack_01" }, { 61, "Play_BOSS_lichA_crack_01" }, { 49, "Play_BOSS_lichC_hook_01" }, { 63, "Play_BOSS_lichC_hook_01" }, { 50, "Play_OBJ_hook_pull_01" }, { 64, "Play_OBJ_hook_pull_01" }, { 51, "Play_OBJ_hook_pull_01" }, { 65, "Play_OBJ_hook_pull_01" }, { 71, "Play_BOSS_lichA_turn_01" }, { 86, "Play_PrisonerLaugh" } });
					EnemyToolbox.AddSoundsToAnimationFrame(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "intro", new Dictionary<int, string> { { 49, "Play_ChainBreak" }, { 63, "Play_ChainBreak" } });
					EnemyToolbox.AddSoundsToAnimationFrame(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "intro", new Dictionary<int, string> { { 49, "Play_OBJ_lock_pick_01" }, { 63, "Play_OBJ_lock_pick_01" } });
					EnemyToolbox.AddEventTriggersToAnimation(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "intro", new Dictionary<int, string> { { 49, "LeftHandBreakFree" }, { 63, "RightHandBreakFree" }, { 54, "LeftHandCastMagic" }, { 87, "PrisonerLaugh" }, { 90, "PrisonerLaugh" }, { 93, "PrisonerLaugh" }, { 96, "PrisonerLaugh" } });
					
					EnemyToolbox.AddEventTriggersToAnimation(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "firelaser", new Dictionary<int, string> { { 1, "FancyMagics" } });
					EnemyToolbox.AddEventTriggersToAnimation(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "chargelaser", new Dictionary<int, string> { { 1, "Charge" } });

					EnemyToolbox.AddSoundsToAnimationFrame(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "chargelaser", new Dictionary<int, string> { { 1, "Play_PrisonerCharge" } });
					EnemyToolbox.AddEventTriggersToAnimation(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "sweengarm", new Dictionary<int, string> { { 0, "SummonRunes" }, { 2, "SummonRunes" } });
					EnemyToolbox.AddSoundsToAnimationFrame(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "raisearm", new Dictionary<int, string> { { 0, "Play_PrisonerCough" }, { 5, "Play_EnergySwirl" } });



					/*
					SpriteBuilder.AddAnimation(companion.spriteAnimator, TheBulletBankClooection, new List<int>
					{
					10,
					11,
					12,
					13

					}, "blooptell", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, TheBulletBankClooection, new List<int>
					{

					14,
					15,
					16,
					17,
					14,
					15,
					16,
					17,
					14,
					15,
					16,
					17,
					13,
					12,
					11

					}, "bloop", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3.5f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, TheBulletBankClooection, new List<int>
					{
					18,
					19,
					20,
					21,
					22,
					23

					}, "bottletell", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, TheBulletBankClooection, new List<int>
					{

					24,
					25,
					26,
					27,
					28,
					29,
					30,
					31,
					32,
					33,
					34,
					35,
					36,
					37,
					38,
					39

					

					}, "bottle", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, TheBulletBankClooection, new List<int>
					{
				40,
				41,
				42,
				43,
				44,
				45,
				46
					}, "roartell", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, TheBulletBankClooection, new List<int>
					{
				47,
				48,
				49,
				50,
				51,
				52,
				48,
				49,
				50,
				51,
				52,
				46,
				45,
				44,
				43,
				42,
				41

					}, "roar", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					*/

					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
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
					9,
					10


					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

				}

				var bs = fuckyouprefab.GetComponent<BehaviorSpeculator>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				
				shootpoint = new GameObject("attach");
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = new Vector2(1.5f, 2.5f);
				GameObject m_CachedGunAttachPoint = companion.transform.Find("attach").gameObject;

				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.8125f, 1.625f), "LeftHandChainPoint");
				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(3.125f, 1.625f), "RightHandChainPoint");
				GameObject RaisedArmLaserAttachPoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(3.5f, 3.4375f), "RaisedArmLaserAttachPoint");
				GameObject OrbPoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(1.875f, 2f), "OrbPoint");
				GameObject RightHandChargePoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(1.375f, 2.4375f), "RightHandChargePoint");



				Projectile beam = null;
				var enemy = EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5");
				foreach (Component item in enemy.GetComponentsInChildren(typeof(Component)))
				{
					if (item is BossFinalRogueLaserGun laser)
					{
						if (laser.beamProjectile)
						{
							beam = laser.beamProjectile;
							break;
						}
					}
				}

				AIBeamShooter2 A = EnemyToolbox.AddAIBeamShooter(companion.aiActor, m_CachedGunAttachPoint.transform, "laserBase", beam, null, -90);
				AIBeamShooter2 B = EnemyToolbox.AddAIBeamShooter(companion.aiActor, m_CachedGunAttachPoint.transform, "laserBase", beam, null, -210);
				AIBeamShooter2 C = EnemyToolbox.AddAIBeamShooter(companion.aiActor, m_CachedGunAttachPoint.transform, "laserBase", beam, null, -330);

			bs.TargetBehaviors = new List<TargetBehaviorBase>
			{
				new TargetPlayerBehavior
				{
					Radius = 35f,
					LineOfSight = false,
					ObjectPermanence = true,
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.25f
				}
			};

			bs.TargetBehaviors = new List<TargetBehaviorBase>
			{
				new TargetPlayerBehavior
				{
					Radius = 35f,
					LineOfSight = true,
					ObjectPermanence = true,
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.25f
				},
			};

				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
				{

					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 1f,
					Behavior = new CustomBeholsterLaserBehavior{
					UsesBeamProjectileWithoutModule = true,
					InitialCooldown = 0f,
					firingTime = 10f,
					Cooldown = 12,
					AttackCooldown = 1.33f,
					RequiresLineOfSight = false,
					FiresDirectlyTowardsPlayer = false,
					UsesCustomAngle = true,
					chargeTime = 1f,
					UsesBaseSounds = false,
					ChargeAnimation = "chargelaser",
					FireAnimation = "firelaser",
					PostFireAnimation = "postlaser",
					beamSelection = ShootBeamBehavior.BeamSelection.Specify,
					specificBeamShooters = new List<AIBeamShooter2>(){A,B,C},
					trackingType = CustomBeholsterLaserBehavior.TrackingType.ConstantTurn,
					BulletScript = new CustomBulletScriptSelector(typeof(SimpleBlasts)),
					ShootPoint = OrbPoint.transform,
					unitCatchUpSpeed = 24f,
					maxTurnRate = 24f,
					turnRateAcceleration = 24f,
					useDegreeCatchUp = true,
					minDegreesForCatchUp = 2.4f,
					degreeCatchUpSpeed = 120,
					useUnitCatchUp = true,
					minUnitForCatchUp = 2,
					maxUnitForCatchUp = 2,
					useUnitOvershoot = true,
					minUnitForOvershoot = 1,
					firingType = CustomBeholsterLaserBehavior.FiringType.ONLY_NORTHANGLEVARIANCE,

					},
						NickName = "Death Laser"
					},
					
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 0.6f,
						Behavior = new ShootBehavior{
						ShootPoint = RaisedArmLaserAttachPoint,
						BulletScript = new CustomBulletScriptSelector(typeof(WallSweep)),
						LeadAmount = 0f,
						AttackCooldown = 0.35f,
						Cooldown = 4f,
						ChargeAnimation = "raisearm",
						FireAnimation = "sweengarm",
						PostFireAnimation = "lowerarm",
						RequiresLineOfSight = true,
						MultipleFireEvents = true,
						Uninterruptible = false,
						},
						NickName = "Bloop"

					},
					
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 2f,
						Behavior = new ShootBehavior{
						ShootPoint = RaisedArmLaserAttachPoint,
						BulletScript = new CustomBulletScriptSelector(typeof(LaserCross)),
						LeadAmount = 0f,
						AttackCooldown = 0f,
						Cooldown = 4f,
						TellAnimation = "chargelaser",
						FireAnimation = "firelaser",
						PostFireAnimation = "postlaser",
						RequiresLineOfSight = true,
						MultipleFireEvents = true,
						Uninterruptible = false,
						},
						NickName = "aaaaa"

					},
					
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 0.9f,
						Behavior = new ShootBehavior{
						ShootPoint = RaisedArmLaserAttachPoint,
						BulletScript = new CustomBulletScriptSelector(typeof(SweepJukeAttack)),
						LeadAmount = 0f,
						AttackCooldown = 0f,
						Cooldown = 12f,
						TellAnimation = "raisearm",
						FireAnimation = "sweengarm",
						PostFireAnimation = "lowerarm",
						RequiresLineOfSight = true,
						MultipleFireEvents = true,
						Uninterruptible = false,
						},
						NickName = "Bloop"

					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1.2f,
						Behavior = new ShootBehavior{
						ShootPoint = RightHandChargePoint,
						BulletScript = new CustomBulletScriptSelector(typeof(BasicLaserAttackTell)),
						LeadAmount = 0f,
						AttackCooldown = 0f,
						Cooldown = 2f,
						TellAnimation = "swipehandback",
						FireAnimation = "swipehandcharge",
						PostFireAnimation = "swipehandmoveback",
						RequiresLineOfSight = true,
						MultipleFireEvents = true,
						Uninterruptible = false,
						},
						NickName = "BasicLaserAttackTell"

					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 0.8f,
						Behavior = new ShootBehavior{
						ShootPoint = RaisedArmLaserAttachPoint,
						BulletScript = new CustomBulletScriptSelector(typeof(ChainRotators)),
						LeadAmount = 0f,
						AttackCooldown = 1f,
						Cooldown = 7f,
						TellAnimation = "raisearm",
						FireAnimation = "sweengarm",
						PostFireAnimation = "lowerarm",
						RequiresLineOfSight = true,
						MultipleFireEvents = true,
						Uninterruptible = false,
						},
						NickName = "ChaosBlasts"

					},
				};
				


				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:cloaked_prisoner", companion.aiActor);


				/*
				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/BulletBanker/bulletbanker_idle_001", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:cloaked_prisoner";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/BulletBanker/bulletbanker_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\bankericon.png");
				PlanetsideModule.Strings.Enemies.Set("#PRISONERPHASEONENAME", "Prisoner");
				PlanetsideModule.Strings.Enemies.Set("#PRISONERPHASEONENAMESD", "Impure Answer");
				PlanetsideModule.Strings.Enemies.Set("#PRISONERPHASEONELONGDESC", "An enormous, sentient bullet who has tasked itself with gathering the souls of those who have begun forging their own creations within the confines of the Gungeon.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#PRISONERPHASEONENAME";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#PRISONERPHASEONENAMESD";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#BULLETBANKSLES";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:cloaked_prisoner");
				EnemyDatabase.GetEntry("psog:cloaked_prisoner").ForcedPositionInAmmonomicon = 9999;
				EnemyDatabase.GetEntry("psog:cloaked_prisoner").isInBossTab = true;
				EnemyDatabase.GetEntry("psog:cloaked_prisoner").isNormalEnemy = true;
				*/

				PlanetsideModule.Strings.Enemies.Set("#PRISONERPHASEONENAME", "Prisoner");
				PlanetsideModule.Strings.Enemies.Set("#PRISONERPHASEONENAMESD", "Impure Answer");
				companion.aiActor.OverrideDisplayName = "#PRISONERPHASEONENAME";
				companion.aiActor.ActorName = "#PRISONERPHASEONENAME";
				companion.aiActor.name = "#PRISONERPHASEONENAME";

				ImprovedAfterImage yeah = companion.gameObject.AddComponent<ImprovedAfterImage>();
				yeah.dashColor = Color.clear;
				yeah.spawnShadows = false;
				yeah.shadowTimeDelay = 0.01f;
				yeah.shadowLifetime = 1f;
				yeah.OverrideImageShader = Shader.Find("Brave/PlayerShader");


				GenericIntroDoer miniBossIntroDoer = fuckyouprefab.AddComponent<GenericIntroDoer>();
				fuckyouprefab.AddComponent<PrisonerPhaseOneIntroController>();
				miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
				miniBossIntroDoer.initialDelay = 0.15f;
				miniBossIntroDoer.cameraMoveSpeed = 14;
				miniBossIntroDoer.specifyIntroAiAnimator = null;
				miniBossIntroDoer.BossMusicEvent = "Play_MUS_PrisonerTheme";
				miniBossIntroDoer.PreventBossMusic = false;
				miniBossIntroDoer.InvisibleBeforeIntroAnim = false;
				miniBossIntroDoer.preIntroAnim = "introidle";
				miniBossIntroDoer.preIntroDirectionalAnim = string.Empty;
				miniBossIntroDoer.introAnim = "intro";
				miniBossIntroDoer.introDirectionalAnim = string.Empty;
				miniBossIntroDoer.continueAnimDuringOutro = false;
				miniBossIntroDoer.cameraFocus = null;
				miniBossIntroDoer.roomPositionCameraFocus = Vector2.zero;
				miniBossIntroDoer.restrictPlayerMotionToRoom = false;
				miniBossIntroDoer.fusebombLock = false;
				miniBossIntroDoer.AdditionalHeightOffset = 0;
				PlanetsideModule.Strings.Enemies.Set("#QUOTE", "");

				miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
				{
					bossNameString = "#PRISONERPHASEONENAME",
					bossSubtitleString = "#PRISONERPHASEONENAMESD",
					bossQuoteString = "#QUOTE",
					bossSpritePxOffset = IntVector2.Zero,
					topLeftTextPxOffset = IntVector2.Zero,
					bottomRightTextPxOffset = IntVector2.Zero,
					bgColor = Color.red
				};
				if (BossCardTexture)
				{
					miniBossIntroDoer.portraitSlideSettings.bossArtSprite = BossCardTexture;
					miniBossIntroDoer.SkipBossCard = false;
					companion.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
				}
				else
				{
					miniBossIntroDoer.SkipBossCard = true;
					companion.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
				}
				miniBossIntroDoer.SkipFinalizeAnimation = true;
				miniBossIntroDoer.RegenerateCache();
				//==================
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(companion.aiActor.healthHaver);
				//==================
			}
		}

		public class Blasty : Script
		{
			protected override IEnumerator Top()
			{
				PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerController>();
				controller.MoveTowardsPositionMethod(4f, 11);

				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
				for (int i = 0; i < 8; i++)
				{
					float Displace = UnityEngine.Random.Range(-3, 3);
					base.Fire(new Offset(Displace), new Direction(0f, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new MegaBullet());
					for (int e = 0; e < 12; e++)
                    {
						base.Fire(new Offset(Displace), new Direction(30*e, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new BasicBullet());
						base.Fire(new Offset(Displace), new Direction((30*e)+15, DirectionType.Aim, -1f), new Speed(7f, SpeedType.Absolute), new BasicBullet());

					}
					float basevalue = 60 - (i*5);					
					yield return this.Wait(basevalue);
				}
				for (int i = 0; i < 10; i++)
                {
					float Displace = UnityEngine.Random.Range(-3, 3);
					base.Fire(new Offset(Displace), new Direction(0f, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new MegaBullet());
					for (int e = 0; e < 12; e++)
					{
						base.Fire(new Offset(Displace), new Direction(30 * e, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new BasicBullet());
						base.Fire(new Offset(Displace), new Direction((30 * e) + 15, DirectionType.Aim, -1f), new Speed(7f, SpeedType.Absolute), new BasicBullet());

					}
					yield return this.Wait(20);
				}
				yield break;
			}
			public class BasicBullet : Bullet
			{
				public BasicBullet() : base("sniper", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(base.Speed+11, SpeedType.Absolute), 60);
					yield break;
				}
			}
			public class MegaBullet : Bullet
			{
				public MegaBullet() : base("big", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
					SpawnManager.PoolManager.Remove(base.Projectile.transform);
					base.ChangeSpeed(new Speed(30f, SpeedType.Absolute), 150);
					yield break;
				}
			}
		}

		public class ChainRotators : Script
        {
			public const int NumBullets = 9;
			public const int BaseTurnSpeed = 540;
			public const float MaxDist = 6f;
			public const int ExtendTime = 30;
			public const int Lifetime = 120;
			public const int ContractTime = 45;
			public const int TellTime = 30;
			public float TurnSpeed;
			public int TicksRemaining;
			public static string[] Transforms;
			public float Divider;
			public float TempaletAngle;

			static ChainRotators()
			{
				ChainRotators.Transforms = new string[]
				{
				"bullet hand",
				"bullet limb 1",
				"bullet limb 2",
				"bullet limb 3"
				};
			}
			public override void OnForceEnded()
			{
				base.OnForceEnded();
				List<List<ChainRotators.SpinBullet>> lists = new List<List<ChainRotators.SpinBullet>>
				{
					bullets,
					bullets2,
					bullets3,
					bullets4
				};
				this.ClearAllLists(lists);
			}
			private List<ChainRotators.SpinBullet> bullets;
			private List<ChainRotators.SpinBullet> bullets2;
			private List<ChainRotators.SpinBullet> bullets3;
			private List<ChainRotators.SpinBullet> bullets4;


			private void SpawnChainsOf(List<ChainRotators.SpinBullet> ListToUse, float Angle, int Delay, bool isTemplateAngle = false)
            {
				base.PostWwiseEvent("Play_ChainBreak_01", null);
				for (int i = 0; i < 60; i++)
				{
					float num = ((float)i + 0.5f) / 10f;
					int num2 = Mathf.CeilToInt(Mathf.Lerp((float)(ChainRotators.Transforms.Length - 1), 0f, num));
					ChainRotators.SpinBullet spinBullet = new ChainRotators.SpinBullet(this, num * 6f, Angle, Delay, isTemplateAngle, i % 6 == 1);
					this.Fire(new Offset(ChainRotators.Transforms[num2]), new Speed(0f, SpeedType.Absolute), spinBullet);
					ListToUse.Add(spinBullet);
				}
			}

			protected override IEnumerator Top()
			{
				PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerController>();
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("463d16121f884984abe759de38418e48").bulletBank.GetBullet("link"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("463d16121f884984abe759de38418e48").bulletBank.GetBullet("ball"));
				Divider = 60;
				this.EndOnBlank = true;
				float turnSign = (float)((BraveMathCollege.AbsAngleBetween(this.BulletBank.aiAnimator.FacingDirection, 0f) <= 90f) ? -1 : 1);
				this.TurnSpeed = 84f * turnSign;
				this.bullets = new List<ChainRotators.SpinBullet>(60);
				this.bullets2 = new List<ChainRotators.SpinBullet>(60);
				this.bullets3 = new List<ChainRotators.SpinBullet>(60);
				this.bullets4 = new List<ChainRotators.SpinBullet>(60);
				TempaletAngle = 90;
				controller.MoveTowardsCenterMethod(1.5f) ;
				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				GameManager.Instance.StartCoroutine(SpawnReticle(0, controller, this));
				this.SpawnChainsOf(bullets, 90, 60, true);
				for (int i = 0; i < 60; i++)
                {
					TempaletAngle += (1 * turnSign);
					yield return this.Wait(1);
				}
				for (int i = 0; i < 3; i++)
                {
					FireQuickBurst();
					yield return this.Wait(60);
				}
				FireQuickBurst();
				GameManager.Instance.StartCoroutine(SpawnReticle(180, controller, this));
				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				for (int i = 0; i < 60; i++)
                {
					float t = (float)i / (float)60;
					this.TurnSpeed = Mathf.SmoothStep(84, 72, t) * turnSign;
					yield return this.Wait(1);
				}

				this.SpawnChainsOf(bullets2, 180 + this.TempaletAngle, 310);

				for (int i = 0; i < 3; i++)
				{
					FireQuickBurst();
					yield return this.Wait(60);
				}
				FireQuickBurst();
				GameManager.Instance.StartCoroutine(SpawnReticle(90, controller, this));
				GameManager.Instance.StartCoroutine(SpawnReticle(270, controller, this));
				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				for (int i = 0; i < 60; i++)
				{
					float t = (float)i / (float)60;
					this.TurnSpeed = Mathf.SmoothStep(72, 60, t)*turnSign;
					yield return this.Wait(1);
					
				}
				controller.extantReticles.Clear();
				this.SpawnChainsOf(bullets3, 90 + this.TempaletAngle, 550);
				this.SpawnChainsOf(bullets4, 270 + this.TempaletAngle, 550);
				for (int i = 0; i < 3; i++)
				{
					FireQuickBurst();
					yield return this.Wait(60);
				}
				FireQuickBurst();
				for (int i = 0; i < 60; i++)
				{
					float t = (float)i / (float)60;
					this.TurnSpeed = Mathf.SmoothStep(60, 0, t) * turnSign;
					yield return this.Wait(1);
				}
				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				yield return this.Wait(15);

				List<List<ChainRotators.SpinBullet>> lists = new List<List<ChainRotators.SpinBullet>>
				{
					bullets,
					bullets2,
					bullets3,
					bullets4
				};
				this.ClearAllLists(lists);
				yield break;
			}

			private IEnumerator SpawnReticle(float Offset, PrisonerController controller, ChainRotators parent, bool ISDodgeAble = true)
			{
				GameObject reticle = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = reticle.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, 90 + this.TempaletAngle);
				component2.dimensions = new Vector2(1f, 1f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laser = new Color(0f, 1f, 1f, 1f);
				Color laserRed = new Color(1f, 0f, 0f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", ISDodgeAble == false ? laser : laserRed);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", ISDodgeAble == false ? laser : laserRed);
				controller.extantReticles.Add(reticle);
				tk2dTiledSprite tiledsprite = reticle.GetComponent<tk2dTiledSprite>();
				float elapsed = 0;
				float Time = 60f;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					if (parent.Destroyed || parent.IsEnded)
                    {
						Destroy(reticle);
						yield break;
					}
					if (reticle != null)
					{
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);

						tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (250 * t));
						tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (10 * t));
						tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, this.TempaletAngle + Offset);
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.dimensions = new Vector2(1000f*t, 1f);
						tiledsprite.UpdateZDepth();
					}
					elapsed++;
					yield return this.Wait(1);
				}

				elapsed = 30;
				Time = 0f;
				while (elapsed > Time)
				{
					float t = (float)elapsed / (float)30;
					if (parent.Destroyed || parent.IsEnded)
					{
						Destroy(reticle);
						yield break;
					}
					if (reticle != null)
					{
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);

						tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 2500);
						tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 10.5f);
						tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, this.TempaletAngle + Offset);
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.dimensions = new Vector2(1000f * t, 1f);
						tiledsprite.UpdateZDepth();
					}
					elapsed--;
					yield return this.Wait(1);
				}
				Destroy(reticle);
				yield break;
			}

			private void ClearAllLists(List<List<ChainRotators.SpinBullet>> bulletLists)
            {

				for (int e = 0; e < bulletLists.Count; e++)
                {
					for (int k = 0; k < bulletLists[e].Count; k++)
					{
						if (bulletLists[e][k] != null)
						{
							bulletLists[e][k].Vanish();
						}
					}
					bulletLists[e] = null;
				}			
			}

			public void FireQuickBurst()
            {
				base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
				for (int e = 0; e < 12; e++)
				{
					base.Fire(new Direction((30 * e), DirectionType.Aim, -1f), new Speed(1f, SpeedType.Absolute), new ChainRotators.BasicBullet());
					base.Fire(new Direction((30 * e), DirectionType.Aim, -1f), new Speed(1.33f, SpeedType.Absolute), new ChainRotators.BasicBullet());
					base.Fire(new Direction((30 * e), DirectionType.Aim, -1f), new Speed(1.66f, SpeedType.Absolute), new ChainRotators.BasicBullet());
				}
			}

			private class SpinBullet : Bullet
			{
				public SpinBullet(ChainRotators parentScript, float maxDist, float angleOffset, int Delay, bool isAngleDecider =false, bool SpawnsUnDodgeablesOnDeath = false) : base("link", false, false, false)
				{
					this.m_parentScript = parentScript;
					this.m_maxDist = maxDist;
					this.m_angOffset = angleOffset;
					this.Delay = Delay;
					this.isDecider = isAngleDecider;
					this.spawnsUndodgeables = SpawnsUnDodgeablesOnDeath;
				}

				protected override IEnumerator Top()
				{
					//this.lifeTime = 0;
					this.ManualControl = true;
					this.Projectile.specRigidbody.CollideWithTileMap = false;
					this.Projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
					float startDist = Vector2.Distance(this.Position, this.m_parentScript.Position);
					float Lifetime = 0;
					float Addon = this.isDecider == true ? -90 : 0;
					this.Direction = m_angOffset + (Addon+3);
					while (!this.m_parentScript.Destroyed && !this.m_parentScript.IsEnded)
					{

						Lifetime++;
						if (this.m_parentScript.BulletBank.healthHaver.IsDead)
						{
							this.Vanish(false);
							yield break;
						}
						this.Direction += this.m_parentScript.TurnSpeed / 60;
						if(isDecider == true) { this.m_parentScript.TempaletAngle = this.Direction; }
						float dist;
						dist = Mathf.Lerp(startDist, this.m_maxDist, (float)(this.m_parentScript.Tick-Delay) / 120f);
						this.Position = this.m_parentScript.Position + BraveMathCollege.DegreesToVector(this.Direction, dist);
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (!preventSpawningProjectiles && spawnsUndodgeables == true)
					{
						//base.Fire(new Direction(-90, DirectionType.Relative, -1f), new Speed(0f, SpeedType.Absolute), new ChainRotators.BasicBullet(12));
						//base.Fire(new Direction(90, DirectionType.Relative, -1f), new Speed(0f, SpeedType.Absolute), new ChainRotators.BasicBullet(12));
					}
				}

				private ChainRotators m_parentScript;
				private int Delay;

				private bool isDecider;
				private float m_angOffset;
				private float m_maxDist;
				private bool spawnsUndodgeables;
			}
			public class BasicBullet : Bullet
			{
				public BasicBullet(float SpeedIncrease = 7) : base("sniper", false, false, false)
				{
					this.Inc = SpeedIncrease;
				}
				protected override IEnumerator Top()
				{
					base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
					SpawnManager.PoolManager.Remove(base.Projectile.transform);
					base.ChangeSpeed(new Speed(base.Speed + Inc, SpeedType.Absolute), 60);
					yield break;
				}
				private float Inc;
			}
		}

		public class BasicLaserAttackTell : Script
        {
			protected override IEnumerator Top()
			{
				bool ISDodgeAble = true;
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
				PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerController>();
				controller.MoveTowardsPositionMethod(0.5f, 7);
				for (int i = -3; i < 4; i++)
				{
					float Angle = base.AimDirection + (20 * i);
					float Offset = (20 * i);
					GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);

					tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
					component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
					component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
					component2.dimensions = new Vector2(1000f, 1f);
					component2.UpdateZDepth();
					component2.HeightOffGround = -2;
					Color laser = new Color(0f, 1f, 1f, 1f);
					Color laserRed = new Color(1f, 0f, 0f, 1f);
					component2.sprite.usesOverrideMaterial = true;
					component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
					component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
					component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
					component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
					component2.sprite.renderer.material.SetColor("_OverrideColor", ISDodgeAble == false ? laser : laserRed);
					component2.sprite.renderer.material.SetColor("_EmissiveColor", ISDodgeAble == false ? laser : laserRed);
					GameManager.Instance.StartCoroutine(FlashReticles(component2, ISDodgeAble, Angle, Offset, this, "directedfire"));
					controller.extantReticles.Add(gameObject);

				}
				yield return this.Wait(30);
				ISDodgeAble = false;
				for (int i = -1; i < 2; i++)
				{
					float Angle = base.AimDirection + (30 * i);
					float Offset = (30 * i);
					GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);

					tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
					component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
					component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
					component2.dimensions = new Vector2(1000f, 1f);
					component2.UpdateZDepth();
					component2.HeightOffGround = -2;
					Color laser = new Color(0f, 1f, 1f, 1f);
					Color laserRed = new Color(1f, 0f, 0f, 1f);
					component2.sprite.usesOverrideMaterial = true;
					component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
					component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
					component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
					component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
					component2.sprite.renderer.material.SetColor("_OverrideColor", ISDodgeAble == false ? laser : laserRed);
					component2.sprite.renderer.material.SetColor("_EmissiveColor", ISDodgeAble == false ? laser : laserRed);
					GameManager.Instance.StartCoroutine(FlashReticles(component2, ISDodgeAble, Angle, Offset, this, "sniper"));
					controller.extantReticles.Add(gameObject);
				}
				yield return this.Wait(60);
				yield break;
			}
			private IEnumerator FlashReticles(tk2dTiledSprite tiledspriteObject, bool isDodgeAble, float Angle, float Offset, BasicLaserAttackTell parent, string BulletType)
            {
				tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
				float elapsed = 0;
				float Time = 0.4f;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;

					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(tiledspriteObject.gameObject);
						yield break;
					}
					if (tiledspriteObject != null)
					{
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);


						float math = isDodgeAble == true ? 250 : 25;
						tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (math * t));
						tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (10 * t));
						tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, base.AimDirection + Offset);
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.dimensions = new Vector2(1000f, 1f);
						tiledsprite.UpdateZDepth();

						Angle = base.AimDirection + Offset;
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.35f;
				base.PostWwiseEvent("Play_FlashTell");
				while (elapsed < Time)
				{

					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(tiledspriteObject.gameObject);
						yield break;
					}
					float t = (float)elapsed / (float)Time;
					if (tiledspriteObject != null)
					{
						float math = isDodgeAble == true ? 350 : 35;
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);
						tiledsprite.dimensions = new Vector2(1000f, 1f);
						tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (math * t));
						tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (20 * t));
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				Destroy(tiledspriteObject.gameObject);

				base.BulletBank.aiActor.GetComponent<PrisonerController>().extantReticles.Clear();
				if (isDodgeAble == false)
				{
					base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
				}
				for (int i = 0; i < 6; i++)
                {
					if (isDodgeAble == true)
					{
						base.Fire(new Direction(Angle, DirectionType.Absolute, -1f), new Speed(13f + (i * 1.5f), SpeedType.Absolute), new WallBullet(BulletType));
					}
					else
                    {
						base.Fire(new Direction(Angle, DirectionType.Absolute, -1f), new Speed(13f + (i * 1.5f), SpeedType.Absolute), new WallBulletNoDodge(BulletType, Angle));
					}
				}
				yield break;
			}
			public class WallBullet : Bullet
			{
				public WallBullet(string BulletType) : base(BulletType, false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					
					yield break;
				}
			}
			public class WallBulletNoDodge : Bullet
			{
				public WallBulletNoDodge(string BulletType, float Angle) : base(BulletType, false, false, false)
				{
					ang = Angle;
				}
				protected override IEnumerator Top()
				{
					base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
					SpawnManager.PoolManager.Remove(base.Projectile.transform);
					yield break;
				}
				private float ang;
			}
		}

		public class WallSweep : Script
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
				Vector2 TopRight = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitTopRight;
				Vector2 BottomLeft = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitBottomLeft;
				Dictionary<Vector2, Vector2> wallcornerPositions = new Dictionary<Vector2, Vector2>()
				{
					{new Vector2(BottomLeft.x, TopRight.y), TopRight },//Bottom wall
					{new Vector2(TopRight.x, BottomLeft.y), BottomLeft },//Top wall
					{BottomLeft, new Vector2(BottomLeft.x, TopRight.y) },//Left wall
					{TopRight, new Vector2(TopRight.x, BottomLeft.y) },//Right wall
				};

				for (int l = 0; l < 4; l++)
                {
					PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerController>();
					controller.MoveTowardsPositionMethod(1f, 7);
					bool ISDodgeAble = true;
					Dictionary<Vector2, string> wallcornerstrings = new Dictionary<Vector2, string>()
					{
						{new Vector2(BottomLeft.x, TopRight.y), "bottom" },//Bottom wall
						{new Vector2(TopRight.x, BottomLeft.y), "top" },//Top wall
						{BottomLeft, "left" },//Left wall
						{TopRight, "right" },//Right wall
					};
					List<Vector2> keyList = new List<Vector2>(wallcornerPositions.Keys);
					Random rand = new Random();
					Vector2 randomKey = keyList[rand.Next(keyList.Count)];
					Vector2 RandomValue = new Vector2();
					wallcornerPositions.TryGetValue(randomKey, out RandomValue);

					if (UnityEngine.Random.value <= 0.66f)
                    {
						wallcornerPositions.Remove(randomKey);
					}
					Dictionary<GameObject, Dictionary<Vector2, Vector2>> list = new Dictionary<GameObject, Dictionary<Vector2, Vector2>>();
					for (int i = 0; i < 2; i++)
					{
						float angle = i == 0 ? (base.Position - randomKey).ToAngle() : (base.Position - RandomValue).ToAngle();
						GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
						tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
						component2.dimensions = new Vector2(1f, 1f);
						component2.UpdateZDepth();
						component2.HeightOffGround = -2;
						Color laser = new Color(0f, 1f, 1f, 1f);
						Color laserRed = new Color(1f, 0f, 0f, 1f);
						component2.sprite.usesOverrideMaterial = true;
						component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
						component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
						component2.sprite.renderer.material.SetColor("_OverrideColor", ISDodgeAble == false ? laser : laserRed);
						component2.sprite.renderer.material.SetColor("_EmissiveColor", ISDodgeAble == false ? laser : laserRed);
						controller.extantReticles.Add(gameObject);
						if (i == 0) { gameObject.name = "CanSpawn"; }
						list.Add(gameObject, i == 0 ? new Dictionary<Vector2, Vector2>() { { randomKey, RandomValue } } : new Dictionary<Vector2, Vector2>() { { RandomValue, randomKey } });
					}
					string String = "NULL";
					wallcornerstrings.TryGetValue(randomKey, out String);
					bool JustForOne = false;
					foreach (var listObj in list)
					{
						if (listObj.Key.name == "CanSpawn") { JustForOne = true; } else { JustForOne = false; }
						GameManager.Instance.StartCoroutine(MoveReticlesSmoothly(listObj.Key, listObj.Value, String, JustForOne, ISDodgeAble));
					}
					yield return this.Wait(105f);
				}
				yield break;
			}

			private IEnumerator MoveReticlesSmoothly(GameObject tiledspriteObject, Dictionary<Vector2, Vector2> posDictionary, string placement, bool CanSpawn, bool isDodgeAble)
            {
				Vector2 TopRight = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitTopRight;
				Vector2 BottomLeft = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitBottomLeft;
				/*
				Dictionary<Vector2, Vector2> wallcornerPositions = new Dictionary<Vector2, Vector2>()
				{
					{new Vector2(BottomLeft.x, TopRight.y), TopRight },//Bottom wall
					{new Vector2(TopRight.x, BottomLeft.y), BottomLeft },//Top wall
					{BottomLeft, new Vector2(BottomLeft.x, TopRight.y) },//Left wall
					{TopRight, new Vector2(TopRight.x, BottomLeft.y) },//Right wall
				};
				*/
				float elapsed = 0;
				float Time = 0.66f;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					if (tiledspriteObject != null)
                    {
						tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
						float ix = tiledsprite.transform.localRotation.eulerAngles.x;
						float wai = tiledsprite.transform.localRotation.eulerAngles.y;
						if (base.BulletBank.aiActor != null)
						{
							ix = tiledsprite.transform.localRotation.eulerAngles.x + base.BulletBank.aiActor.transform.localRotation.x;
							wai = tiledsprite.transform.localRotation.eulerAngles.y + base.BulletBank.aiActor.transform.localRotation.y;
						}
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.transform.position.WithZ(tiledsprite.transform.position.z + 99999);
						tiledsprite.UpdateZDepth();
						Dictionary<Vector2, Vector2> Pos = posDictionary;
						foreach (var yes in Pos)
						{
							float Dist = Vector2.Distance(tiledsprite.transform.position, yes.Key) +4;
							Vector2 vector3 = Vector2.Lerp(yes.Key, yes.Value, t);
							float angle = (base.Position - vector3).ToAngle();
							tiledsprite.transform.localRotation = Quaternion.Euler(ix, wai, angle);
							tiledsprite.dimensions = new Vector2((Dist*16) * t, 1f);
							tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f+(5*t));
							ImprovedAfterImageForTiled yes1 = tiledsprite.gameObject.GetOrAddComponent<ImprovedAfterImageForTiled>();
							yes1.spawnShadows = true;
							yes1.shadowLifetime = 0.2f;
							yes1.shadowTimeDelay = 0.005f;
							yes1.dashColor = isDodgeAble == false ? new Color(0f, 1f, 1f, 1f) : new Color(1f, 0f, 0f, 1f);
						}
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.33f;
				base.PostWwiseEvent("Play_FlashTell");
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					if (tiledspriteObject != null)
					{
						tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
						float ix = tiledsprite.transform.localRotation.eulerAngles.x;
						float wai = tiledsprite.transform.localRotation.eulerAngles.y;
						if (base.BulletBank.aiActor != null)
						{
							ix = tiledsprite.transform.localRotation.eulerAngles.x + base.BulletBank.aiActor.transform.localRotation.x;
							wai = tiledsprite.transform.localRotation.eulerAngles.y + base.BulletBank.aiActor.transform.localRotation.y;
						}
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.transform.position.WithZ(tiledsprite.transform.position.z + 99999);
						tiledsprite.UpdateZDepth();
						Dictionary<Vector2, Vector2> Pos = posDictionary;
						foreach (var yes in Pos)
						{
							Vector2 vector3 = Vector2.Lerp(yes.Value, yes.Key, t);
							float angle = (base.Position - vector3).ToAngle();
							tiledsprite.transform.localRotation = Quaternion.Euler(ix, wai, angle);
							ImprovedAfterImageForTiled yes1 = tiledsprite.gameObject.GetOrAddComponent<ImprovedAfterImageForTiled>();
						}
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				foreach (GameObject reticles in base.BulletBank.aiActor.GetComponent<PrisonerController>().extantReticles)
				{
					Destroy(reticles);
				}
				base.BulletBank.aiActor.GetComponent<PrisonerController>().extantReticles.Clear();
				if (CanSpawn == true)
				{
					this.FireWallBullets(placement, isDodgeAble);
				}
				yield break;
            }

			private void FireWallBullets(string Placement, bool IsDodgeAble)
			{
				Vector2 TopRight = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitTopRight;
				Vector2 BottomLeft = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitBottomLeft;
				Dictionary<Vector2, Vector2> wallcornerPositions = new Dictionary<Vector2, Vector2>()
				{
					{new Vector2(BottomLeft.x, TopRight.y), TopRight },//Bottom wall
					{new Vector2(TopRight.x, BottomLeft.y), BottomLeft },//Top wall
					{BottomLeft, new Vector2(BottomLeft.x, TopRight.y) },//Left wall
					{TopRight, new Vector2(TopRight.x, BottomLeft.y) },//Right wall
				};
				//All of these are flipped. IDFK WHY BUT I NEED TO GET THE "TOP" to spawn it at THE BOTTOM AAAAAAAAAAAAAAAAA
				Dictionary<string, Vector2> wallcornerstrings = new Dictionary<string, Vector2>()
				{
					{ "bottom" ,new Vector2(TopRight.x, BottomLeft.y)},
					{ "top" ,new Vector2(BottomLeft.x, TopRight.y)},
					{ "left" ,TopRight},
					{ "right" ,BottomLeft}
				};

				Vector2 OneCorner = new Vector2();
				wallcornerstrings.TryGetValue(Placement, out OneCorner);
				Vector2 OtherCorner = new Vector2();
				wallcornerPositions.TryGetValue(OneCorner, out OtherCorner);
				float Tiles = Vector2.Distance(OneCorner, OtherCorner);
				float facingDir = 0;
				if (Placement == "bottom") { facingDir = 90; }
				if (Placement == "top") { facingDir = 270; }
				if (Placement == "left") { facingDir = 180; }
				if (Placement == "right") { facingDir = 0; }

				base.PostWwiseEvent("Play_RockBreaking", null);
				for (int l = 0; l < Tiles; l++)
				{
					float t = (float)l / (float)Tiles;
					Vector2 SpawnPos = Vector2.Lerp(OneCorner, OtherCorner, t);
					if (Placement == "top") { SpawnPos = SpawnPos + new Vector2(0, 1.5f);}

					if (UnityEngine.Random.value <= 0.15f)
                    {
						GameObject dragunBoulder = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyBoulder;
						foreach (Component item in dragunBoulder.GetComponentsInChildren(typeof(Component)))
						{
							if (item is SkyRocket laser)
							{
								ExplosionData explosionData = laser.ExplosionData;
								GameObject vfx = UnityEngine.Object.Instantiate<GameObject>(explosionData.effect, SpawnPos, Quaternion.identity);
								tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
								component.PlaceAtPositionByAnchor(SpawnPos, tk2dBaseSprite.Anchor.MiddleCenter);
								component.HeightOffGround = 35f;
								component.transform.rotation = Quaternion.Euler(0f, 0f, facingDir+90);
								component.UpdateZDepth();
								tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
								if (component2 != null)
								{
									component.usesOverrideMaterial = true;
									if (IsDodgeAble == false) { component2.renderer.material.shader = PlanetsideModule.ModAssets.LoadAsset<Shader>("inverseglowshader"); }
									component2.ignoreTimeScale = true;
									component2.AlwaysIgnoreTimeScale = true;
									component2.AnimateDuringBossIntros = true;
									component2.alwaysUpdateOffscreen = true;
									component2.playAutomatically = true;
								}
							}
						}
					}
					base.Fire(Offset.OverridePosition(SpawnPos), new Direction(facingDir, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(15f, SpeedType.Absolute), new WallSweep.WallBullets(this, "spore2", facingDir, IsDodgeAble));
				}
			}
			public class WallBullets : Bullet
			{
				public WallBullets(WallSweep parent, string bulletName, float angle, bool IsDodgeAble) : base(bulletName, true, false, false)
				{
					this.m_parent = parent;
					this.Angle = angle;
					this.IsDoge = IsDodgeAble;
				}
				protected override IEnumerator Top()
				{
					if (IsDoge == false)
                    {
						base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
						SpawnManager.PoolManager.Remove(base.Projectile.transform);
					}
					int travelTime = UnityEngine.Random.Range(90, 360);
					this.Projectile.IgnoreTileCollisionsFor(180f);
					this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
					if (UnityEngine.Random.value <= 0.5f)
                    {
						base.ChangeSpeed(new Speed(2f, SpeedType.Absolute), UnityEngine.Random.Range(15, 120));
					}
					else
                    {
						base.ChangeSpeed(new Speed(3f, SpeedType.Absolute), UnityEngine.Random.Range(75, 300));
					}

					yield return base.Wait(travelTime);
					base.Vanish(false);
					yield break;
				}
				private WallSweep m_parent;
				private float Angle;
				private bool IsDoge;
			}
		}
		public class SimpleBlasts : Script
		{
			protected override IEnumerator Top()
			{
				this.EndOnBlank = true;
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("icicle"));
				}
				PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerController>();
				controller.MoveTowardsCenterMethod(2f);
				yield return this.Wait(100f);

				for (int l = 0; l < 7; l++)
				{
					base.PostWwiseEvent("Play_ENM_blobulord_bubble_01", null);
					bool LeftOrRight = (UnityEngine.Random.value > 0.5f) ? false : true;
					float RNGSPIN = LeftOrRight == true ? 15 : -15;
					for (int j = 0; j < 5; j++)
					{
						for (int e = 0; e < 20; e++)
                        {
							base.Fire(new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SimpleBlasts.RotatedBullet(RNGSPIN, 0, 0, "icicle", this, (0 + (float)e * 18 + (RNGSPIN/12)*j),0.07f,  e %5 ==0 ? false : true));
						}
						yield return this.Wait(6f);
					}
					yield return this.Wait(60f);
				}
				
				yield break;
			}
			public class RotatedBullet : Bullet
			{
				public RotatedBullet(float spinspeed, float RevUp, float StartSpeenAgain, string BulletType, SimpleBlasts parent, float angle = 0f, float aradius = 0, bool IsdodgeAble = true) : base(BulletType, false, false, false)
				{
					this.m_spinSpeed = spinspeed;
					this.TimeToRevUp = RevUp;
					this.StartAgain = StartSpeenAgain;

					this.m_parent = parent;
					this.m_angle = angle;
					this.m_radius = aradius;
					this.m_bulletype = BulletType;
					this.SuppressVfx = true;
					this.isDodgeable = IsdodgeAble;
				}

				protected override IEnumerator Top()
				{
					if (isDodgeable == false)
                    {
						base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
						SpawnManager.PoolManager.Remove(base.Projectile.transform);
					}
					base.Projectile.transform.localRotation = Quaternion.Euler(0f, 0f, this.m_angle);
					base.ManualControl = true;
					Vector2 centerPosition = base.Position;
					float radius = 0f;
					for (int i = 0; i < 2400; i++)
					{
						radius += m_radius;
						centerPosition += this.Velocity / 60f;
						base.UpdateVelocity();
						this.m_angle += this.m_spinSpeed / 60f;
						base.Projectile.shouldRotate = true;
						base.Direction = this.m_angle;
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
				private SimpleBlasts m_parent;
				private float m_angle;
				private float m_spinSpeed;
				private float m_radius;
				private string m_bulletype;
				private float TimeToRevUp;
				private float StartAgain;
				bool isDodgeable;

			}
		}

		public class SweepJukeAttack : Script
		{
			protected override IEnumerator Top()
			{
				this.EndOnBlank = true;
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));

				PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerController>();
				controller.MoveTowardsPositionMethod(2f, 11);
				Vector2 vector2 = this.BulletManager.PlayerPosition();
				Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2, this.BulletManager.PlayerVelocity(), this.Position, 18f);
				float CentreAngle = (predictedPosition - this.Position).ToAngle();
				base.PostWwiseEvent("Play_BOSS_omegaBeam_charge_01");
				base.BulletBank.aiActor.StartCoroutine(FlashReticles(CentreAngle - 180, 168, this));
				base.BulletBank.aiActor.StartCoroutine(FlashReticles(CentreAngle - 180, -168, this));
				for (int e = 0; e < 4; e++)
                {
					yield return this.Wait(30);
					base.PostWwiseEvent("Play_BOSS_omegaBeam_charge_01");
					base.BulletBank.aiActor.StartCoroutine(SwipeLaser(CentreAngle - 180, -168, this, 1.5f - (0.5f * e)));
					base.BulletBank.aiActor.StartCoroutine(SwipeLaser(CentreAngle - 180, 168, this, 1.5f - (0.5f * e)));
				}
				for (int e = 0; e < 8; e++)
                {
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(CentreAngle, this));
					yield return this.Wait(40);
				}
				yield break;
			}


			private IEnumerator SwipeLaser(float StartAngle, float AddOrSubtract, SweepJukeAttack parent, float Time)
            {
				GameObject reticle = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = reticle.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, StartAngle);
				component2.dimensions = new Vector2(1f, 1000f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laserRed = new Color(1f, 0f, 0f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", laserRed);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", laserRed);

				ImprovedAfterImageForTiled yes1 = component2.gameObject.GetOrAddComponent<ImprovedAfterImageForTiled>();
				yes1.spawnShadows = true;
				yes1.shadowLifetime = 0.2f;
				yes1.shadowTimeDelay = 0.005f;
				yes1.dashColor = new Color(1f, 0f, 0f, 1f);
				float elapsed = 0;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					if (component2.gameObject != null)
					{
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 0);

						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (250 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (10 * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(StartAngle, StartAngle + AddOrSubtract, t));
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				Destroy(component2.gameObject);
				yield break;
            }


			private IEnumerator FlashReticles(float StartAngle, float AddOrSubtract, SweepJukeAttack parent)
			{

				GameObject reticle = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = reticle.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, StartAngle);
				component2.dimensions = new Vector2(1f, 1000f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laserRed = new Color(1f, 0f, 0f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", laserRed);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", laserRed);

				ImprovedAfterImageForTiled yes1 = component2.gameObject.GetOrAddComponent<ImprovedAfterImageForTiled>();
				yes1.spawnShadows = true;
				yes1.shadowLifetime = 0.2f;
				yes1.shadowTimeDelay = 0.005f;
				yes1.dashColor = new Color(1f, 0f, 0f, 1f);

				float elapsed = 0;
				float Time = 1.25f;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					if (component2.gameObject != null)
					{
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 0);

						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (250 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (10 * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(StartAngle, StartAngle + AddOrSubtract, t));
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.75f;
				while (elapsed < Time)
				{

					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				Destroy(component2.gameObject);

				base.BulletBank.aiActor.GetComponent<PrisonerController>().extantReticles.Clear();
				base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);

				for (int e = 0; e < 15; e++)
                {
					for (int i = 0; i < 36; i++)
					{
						float t = (float)i / (float)36;
						base.Fire(new Direction(Mathf.Lerp(StartAngle, StartAngle + AddOrSubtract, t), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(28, 32), SpeedType.Absolute), new SweepJukeAttack.BasicBullet());
					}
					base.PostWwiseEvent("Play_BOSS_doormimic_zap_01");
					yield return new WaitForSeconds(0.33f);
				}
				yield break;
			}

			private IEnumerator QuickscopeNoob(float Angle, SweepJukeAttack parent)
			{

				GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);

				tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
				component2.dimensions = new Vector2(1000f, 1f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laser = new Color(0f, 1f, 1f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
				float elapsed = 0;
				float Time = 0.25f;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;

					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					if (component2 != null)
					{
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 0);

						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (25 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, base.AimDirection);
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();

						Angle = base.AimDirection;
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.25f;
				base.PostWwiseEvent("Play_FlashTell");
				while (elapsed < Time)
				{
					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					float t = (float)elapsed / (float)Time;
					if (component2 != null)
					{
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, 0);
						component2.dimensions = new Vector2(1000f, 1f);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (60 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (20 * t));
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				Destroy(component2.gameObject);
				base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
				base.Fire(new Direction(Angle, DirectionType.Absolute, -1f), new Speed(32.5f, SpeedType.Absolute), new WallBulletNoDodge("sniper", Angle));
				yield break;
			}


			public class WallBulletNoDodge : Bullet
			{
				public WallBulletNoDodge(string BulletType, float Angle) : base(BulletType, false, false, false)
				{
					ang = Angle;
				}
				protected override IEnumerator Top()
				{
					base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
					SpawnManager.PoolManager.Remove(base.Projectile.transform);
					yield break;
				}
				private float ang;
			}
			public class BasicBullet : Bullet
			{
				public BasicBullet(float SpeedIncrease = 7) : base("sniper", false, false, false)
				{
					this.Inc = SpeedIncrease;
				}
				protected override IEnumerator Top()
				{
					yield break;
				}
				private float Inc;
			}
		}


		public class LaserCross : Script
		{
			protected override IEnumerator Top()
			{
				PrisonerController controller = base.BulletBank.aiActor.GetComponent<PrisonerController>();
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
				controller.MoveTowardsPositionMethod(3f, 7);
				for (int e = 0; e < GameManager.Instance.AllPlayers.Length; e++)
                {
					float Dir = UnityEngine.Random.value > 0.5f ? 0 : 45f;
					for (int i = 0; i < 8; i++)
					{			
						base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(GameManager.Instance.AllPlayers[e].transform.PositionVector2(), (45 * i) + Dir, this));
					}
				}		
				yield return this.Wait(75);
				for (int q = 0; q < 4; q++)
                {
					for (int e = 0; e < GameManager.Instance.AllPlayers.Length; e++)
					{
						float Dir = UnityEngine.Random.Range(-180, 180);
						float helpme = UnityEngine.Random.Range(180, -180);
						for (int i = 0; i < 6; i++)
						{
							base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(GameManager.Instance.AllPlayers[e].transform.PositionVector2() + MathToolbox.GetUnitOnCircle(helpme, 5), (60 * i) + Dir, this));
						}
					}
					yield return this.Wait(20);
				}
				controller.MoveTowardsPositionMethod(1f, 5);
				yield return this.Wait(60);
				yield break;
			}


			private IEnumerator QuickscopeNoob(Vector2 startPos,float aimDir,  LaserCross parent, float chargeTime = 0.5f)
			{

				GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(startPos.x, startPos.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
				component2.dimensions = new Vector2(1000f, 1f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laser = new Color(0f, 1f, 1f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
				float elapsed = 0;
				float Time = chargeTime;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;

					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					if (component2 != null)
					{
						component2.transform.position = new Vector3(startPos.x, startPos.y, 0);

						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (25 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.25f;
				base.PostWwiseEvent("Play_FlashTell");
				while (elapsed < Time)
				{
					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					float t = (float)elapsed / (float)Time;
					if (component2 != null)
					{
						component2.transform.position = new Vector3(startPos.x, startPos.y, 0);
						component2.dimensions = new Vector2(1000f, 1f);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (60 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (20 * t));
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				Destroy(component2.gameObject);
				base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
				for (int i = 0; i < 10; i++)
                {
					base.Fire(Offset.OverridePosition(startPos), new Direction(aimDir, DirectionType.Absolute, -1f), new Speed(20f, SpeedType.Absolute), new WallBulletNoDodge("sniper"));
					yield return new WaitForSeconds(0.025f);
				}
				yield break;
			}

			public class WallBulletNoDodge : Bullet
			{
				public WallBulletNoDodge(string BulletType) : base(BulletType, false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
					SpawnManager.PoolManager.Remove(base.Projectile.transform);

					yield break;
				}
			}
			public class WallBulletDodge : Bullet
			{
				public WallBulletDodge(string BulletType) : base(BulletType, false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(8f, SpeedType.Absolute), 30);
					//base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
					//SpawnManager.PoolManager.Remove(base.Projectile.transform);
					yield break;
				}
			}
			public class BasicBullet : Bullet
			{
				public BasicBullet() : base("sniper", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(base.Speed + 11, SpeedType.Absolute), 60);
					yield break;
				}
			}
			public class MegaBullet : Bullet
			{
				public MegaBullet() : base("big", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					base.Projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
					SpawnManager.PoolManager.Remove(base.Projectile.transform);
					base.ChangeSpeed(new Speed(30f, SpeedType.Absolute), 150);
					yield break;
				}
			}
		}


		public class PrisonerController : BraveBehaviour
		{
			public List<GameObject> extantReticles = new List<GameObject>();
			private RoomHandler m_StartRoom;

			public void Update()
			{
				m_StartRoom = aiActor.GetAbsoluteParentRoom();

			}
			private void CheckPlayerRoom()
			{
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
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
			
			private void Start()
			{
				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(base.aiActor.healthHaver);
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{

				};
				base.healthHaver.healthHaver.OnDeath += (obj) =>
				{

				}; ;
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");

			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo.Contains("FancyMagics"))
				{
					GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");					
					tk2dSpriteAnimator objanimator = silencerVFX.GetComponentInChildren<tk2dSpriteAnimator>();
					objanimator.ignoreTimeScale = true;
					objanimator.AlwaysIgnoreTimeScale = true;
					objanimator.AnimateDuringBossIntros = true;
					objanimator.alwaysUpdateOffscreen = true;
					objanimator.playAutomatically = true;
					ParticleSystem objparticles = silencerVFX.GetComponentInChildren<ParticleSystem>();
					var main = objparticles.main;
					main.useUnscaledTime = true;
					GameObject.Instantiate(silencerVFX.gameObject, base.aiActor.transform.Find("OrbPoint").position, Quaternion.identity);
					GlobalSparksDoer.DoLinearParticleBurst(40, base.aiActor.transform.Find("OrbPoint").position, base.aiActor.transform.Find("OrbPoint").position, 20f, 10f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Charge"))
                {
					Exploder.DoDistortionWave(base.aiActor.transform.Find("OrbPoint").position + new Vector3(0, 0.5f), 3, 0.1f, 15, 0.5f);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("SummonRunes"))
				{
					Vector3 pos= MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(1, 4));
					Instantiate((PickupObjectDatabase.GetById(145) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects[0].effects[0].effect, base.aiActor.transform.Find("RaisedArmLaserAttachPoint").position+ pos, Quaternion.identity);
				}
			}
			public void MoveTowardsCenterMethod(float Time)
			{
				GameManager.Instance.StartCoroutine(this.MoveTowardsCenter(Time));
			}
			private IEnumerator MoveTowardsCenter(float Time)
			{
				ImprovedAfterImage yeah = base.aiActor.gameObject.GetComponent<ImprovedAfterImage>();
				yeah.spawnShadows = true;
				IntVector2? intVector = base.aiActor.ParentRoom.GetCenterCell();
				float elaWait = 0f;
				float duraWait = Time;
				Vector3 pos = base.aiActor.transform.position;
				while (elaWait < duraWait)
				{
					float t = (float)elaWait / (float)duraWait;
					float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
					Vector3 vector3 = Vector3.Lerp(pos, new Vector3(intVector.Value.X, intVector.Value.Y), throne1);
					base.aiActor.transform.position = vector3;
					elaWait += BraveTime.DeltaTime;
					base.aiActor.specRigidbody.Reinitialize();
					yield return null;
				}
				yeah.spawnShadows = false;
				yield break;
			}

			public void MoveTowardsPositionMethod(float Time,float TileDistance ,float MinDistFromPlayer = 6, float MaxDistFromPlayer = 12)
            {
				GameManager.Instance.StartCoroutine(this.MoveTowardsPosition(Time, TileDistance, MinDistFromPlayer, MaxDistFromPlayer));
            }

			

			private IEnumerator MoveTowardsPosition(float Time, float TileDistance, float MinDistFromPlayer = 6, float MaxDistFromPlayer = 12)
            {
				ImprovedAfterImage yeah = base.aiActor.gameObject.GetComponent<ImprovedAfterImage>();
				yeah.spawnShadows = true;
				IntVector2? intVector = this.GetRandomAvailableCellWithinDistance(base.aiActor.ParentRoom ,5, new IntVector2?(base.aiActor.Clearance), new CellTypes?(CellTypes.FLOOR), false, null);
				float elaWait = 0f;
				float duraWait = Time;
				Vector3 pos = base.aiActor.transform.position;
				while (elaWait < duraWait)
				{
					float t = (float)elaWait / (float)duraWait;
					float throne1 = Mathf.Sin(t*(Mathf.PI/2));
					Vector3 vector3 = Vector3.Lerp(pos, new Vector3(intVector.Value.X, intVector.Value.Y), throne1);
					base.aiActor.transform.position = vector3;
					elaWait += BraveTime.DeltaTime;
					base.aiActor.specRigidbody.Reinitialize();
					yield return null;
				}
				yeah.spawnShadows = false;

				yield break;
            }



			public IntVector2? GetRandomAvailableCellWithinDistance(RoomHandler room, float Distance, IntVector2? footprint = null, CellTypes? passableCellTypes = null, bool canPassOccupied = true, CellValidator cellValidator = null)
			{
				if (footprint == null)
				{
					footprint = new IntVector2?(IntVector2.One);
				}
				if (passableCellTypes == null)
				{
					passableCellTypes = new CellTypes?((CellTypes)2147483647);
				}

				CellData tile = null;
				if (GameManager.HasInstance && GameManager.Instance.Dungeon != null && GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(new IntVector2(((int)base.aiActor.transform.position.x), ((int)base.aiActor.transform.position.y))))
				{
					tile = GameManager.Instance.Dungeon.data[new IntVector2(((int)base.aiActor.transform.position.x), ((int)base.aiActor.transform.position.y))];
				}
				List<IntVector2> list = new List<IntVector2>();
				if (tile != null && tile.parentRoom == room && !tile.isExitCell)
				{
					IntVector2 intVector = tile.position;
					Vector2 b = footprint.Value.ToVector2() / 2f;
					Vector2 b2 = intVector.ToVector2() + b;
					for (int i = 0; i < 36; i++)
                    {
						Vector2 vec = MathToolbox.GetUnitOnCircle(i * 10, Distance);
						IntVector2 intVectorDos = tile.position + new IntVector2(((int)vec.x), ((int)vec.y));
						CellData tileTwo = null;
						if (GameManager.HasInstance && GameManager.Instance.Dungeon != null && GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVectorDos))
						{
							tileTwo = GameManager.Instance.Dungeon.data[intVectorDos];
							if (tileTwo != null && tileTwo.parentRoom == room && !tileTwo.isExitCell && !tileTwo.isNextToWall && !tileTwo.IsPlayerInaccessible)
							{
								if (Pathfinder.Instance.IsPassable(intVectorDos, footprint, passableCellTypes, canPassOccupied, cellValidator))
								{
									list.Add(intVectorDos);
								}
							}
						}
					}				
				}
				
				if (list.Count > 0)
				{
					IntVector2 value = list[UnityEngine.Random.Range(0, list.Count)];
					return new IntVector2?(value);
				}
				return null;
			}

		}

		public static List<int> Lootdrops = new List<int>
		{
			73,
			85,
			120,
			67,
			224,
			600,
			78
		};





		private static string[] spritePaths = new string[]
		{
			
			"Planetside/Resources/Bosses/Prisoner/prisonerPH1_trueidle_001.png",//0
			"Planetside/Resources/Bosses/Prisoner/prisonerPH1_trueidle_002.png",
			"Planetside/Resources/Bosses/Prisoner/prisonerPH1_trueidle_003.png",
			"Planetside/Resources/Bosses/Prisoner/prisonerPH1_trueidle_004.png",
			"Planetside/Resources/Bosses/Prisoner/prisonerPH1_trueidle_005.png",
			"Planetside/Resources/Bosses/Prisoner/prisonerPH1_trueidle_006.png",
			"Planetside/Resources/Bosses/Prisoner/prisonerPH1_trueidle_007.png",
			"Planetside/Resources/Bosses/Prisoner/prisonerPH1_trueidle_008.png",
			"Planetside/Resources/Bosses/Prisoner/prisonerPH1_trueidle_009.png",
			"Planetside/Resources/Bosses/Prisoner/prisonerPH1_trueidle_010.png",
			"Planetside/Resources/Bosses/Prisoner/prisonerPH1_trueidle_011.png",//10

			"Planetside/Resources/Bosses/Prisoner/IntroIdle/prisonerPH1_introidle_001.png",//11
			"Planetside/Resources/Bosses/Prisoner/IntroIdle/prisonerPH1_introidle_002.png",
			"Planetside/Resources/Bosses/Prisoner/IntroIdle/prisonerPH1_introidle_003.png",//13

			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_001.png",//14
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_002.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_003.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_004.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_005.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_006.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_007.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_008.png",//21
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_009.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_010.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_011.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_012.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_013.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_014.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_015.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_016.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_017.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_018.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_019.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_020.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_021.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_022.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_023.png",
			"Planetside/Resources/Bosses/Prisoner/Intro/prisonerPH1_intro_024.png",//37

			"Planetside/Resources/Bosses/Prisoner/LaserAttack/Charge/prisonerPH1_chargelaser_001.png",//38
			"Planetside/Resources/Bosses/Prisoner/LaserAttack/Charge/prisonerPH1_chargelaser_002.png",
			"Planetside/Resources/Bosses/Prisoner/LaserAttack/Charge/prisonerPH1_chargelaser_003.png",
			"Planetside/Resources/Bosses/Prisoner/LaserAttack/Charge/prisonerPH1_chargelaser_004.png",
			"Planetside/Resources/Bosses/Prisoner/LaserAttack/Charge/prisonerPH1_chargelaser_005.png",
			"Planetside/Resources/Bosses/Prisoner/LaserAttack/Charge/prisonerPH1_chargelaser_006.png",
			"Planetside/Resources/Bosses/Prisoner/LaserAttack/Charge/prisonerPH1_chargelaser_007.png",//44

			"Planetside/Resources/Bosses/Prisoner/LaserAttack/Fire/prisonerPH1_firelaser_001.png",//45
			"Planetside/Resources/Bosses/Prisoner/LaserAttack/Fire/prisonerPH1_firelaser_002.png",
			"Planetside/Resources/Bosses/Prisoner/LaserAttack/Fire/prisonerPH1_firelaser_003.png",//47

			"Planetside/Resources/Bosses/Prisoner/FlailArm/prisonerPH1_raisearm_001.png",//48
			"Planetside/Resources/Bosses/Prisoner/FlailArm/prisonerPH1_raisearm_002.png",
			"Planetside/Resources/Bosses/Prisoner/FlailArm/prisonerPH1_raisearm_003.png",
			"Planetside/Resources/Bosses/Prisoner/FlailArm/prisonerPH1_raisearm_004.png",
			"Planetside/Resources/Bosses/Prisoner/FlailArm/prisonerPH1_raisearm_005.png",//52

			"Planetside/Resources/Bosses/Prisoner/FlailArm/prisonerPH1_sweengarm_001.png",//53
			"Planetside/Resources/Bosses/Prisoner/FlailArm/prisonerPH1_sweengarm_002.png",
			"Planetside/Resources/Bosses/Prisoner/FlailArm/prisonerPH1_sweengarm_003.png",
			"Planetside/Resources/Bosses/Prisoner/FlailArm/prisonerPH1_sweengarm_004.png",//56

			"Planetside/Resources/Bosses/Prisoner/Swipe/prisonerPH1_movehandback_001.png",//57
			"Planetside/Resources/Bosses/Prisoner/Swipe/prisonerPH1_movehandback_002.png",
			"Planetside/Resources/Bosses/Prisoner/Swipe/prisonerPH1_movehandback_003.png",
			"Planetside/Resources/Bosses/Prisoner/Swipe/prisonerPH1_movehandback_004.png",//60

			"Planetside/Resources/Bosses/Prisoner/Swipe/prisonerPH1_stayinplacehand_001.png",//61

			"Planetside/Resources/Bosses/Prisoner/Swipe/prisonerPH1_swipeproperhand_001.png",//62
			"Planetside/Resources/Bosses/Prisoner/Swipe/prisonerPH1_swipeproperhand_002.png",
			"Planetside/Resources/Bosses/Prisoner/Swipe/prisonerPH1_swipeproperhand_003.png",
			"Planetside/Resources/Bosses/Prisoner/Swipe/prisonerPH1_swipeproperhand_004.png",
			"Planetside/Resources/Bosses/Prisoner/Swipe/prisonerPH1_swipeproperhand_005.png",
			"Planetside/Resources/Bosses/Prisoner/Swipe/prisonerPH1_swipeproperhand_006.png",//67

		};
	}
}








