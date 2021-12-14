using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using SaveAPI;

namespace Planetside
{
	public class JammedGuard : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "jammed_guardian";
		private static tk2dSpriteCollectionData JammedGuardian;
		public static GameObject shootpoint;
		public static void Init()
		{
			JammedGuard.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Jammed Guardian", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 800;
				companion.aiActor.MovementSpeed = 1.25f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(150f);
				companion.aiActor.CollisionKnockbackStrength = 2f;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.healthHaver.SetHealthMaximum(150f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.ActorName = "Jammed Guardian";
				companion.aiActor.name = "Jammed Guardian";
				companion.aiActor.HasShadow = true;
				companion.aiActor.SetIsFlying(true, "Gamemode: Creative");
				companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
				companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>().dashColor = Color.red;
				companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>().spawnShadows = true;
				companion.aiActor.gameObject.AddComponent<AfterImageTrailController>().spawnShadows = false;
				companion.aiActor.gameObject.AddComponent<tk2dSpriteAttachPoint>();
				companion.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();
				companion.aiActor.gameObject.AddComponent<AIBeamShooter>();
				companion.aiActor.gameObject.AddComponent<AIBulletBank>();
				//companion.aiActor.gameObject.GetComponent<AfterImageTrailController>().spawnShadows = true;
				//companion.aiActor.gameObject.GetComponent<AfterImageTrailController>().OptionalImageShader = companion.AfterImageTrailController.Shadow;
				//companion.aiActor.gameObject.AddComponent(Trail);
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider


				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 0,
					ManualOffsetY = 0,
					ManualWidth = 24,
					ManualHeight = 24,
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
					ManualOffsetX = 0,
					ManualOffsetY = 0,
					ManualWidth = 24,
					ManualHeight = 24,
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
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
					{
						"idle_right", //Good
						"idle_left",//Good
						

					}
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
					{
						"run_right", //Good
						"run_left",//Good
						

					}
				};
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

					   "die_right",
						   "die_left"

							}

						}
					}
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "tell",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{
						"tell_right", //Good
						"tell_left",//Good
							}

						}
					}
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "attack",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{
					   "attack_right",
					   "attack_left",

							}

						}
					}
				};
				bool flag3 = JammedGuardian == null;
				if (flag3)
				{
					JammedGuardian = SpriteBuilder.ConstructCollection(prefab, "Guarder_Collection");
					UnityEngine.Object.DontDestroyOnLoad(JammedGuardian);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], JammedGuardian);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, JammedGuardian, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, JammedGuardian, new List<int>
					{

					6,
					7,
					8,
					9,
					10,
					11



					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, JammedGuardian, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5

					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, JammedGuardian, new List<int>
					{
					6,
					7,
					8,
					9,
					10,
					11



					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, JammedGuardian, new List<int>
					{
        12
					}, "tell_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 20f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, JammedGuardian, new List<int>
					{
		19
					}, "tell_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 20f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, JammedGuardian, new List<int>
					{
		12,
		13,
		14,
		15,
		16,
		17,
		18,
					}, "attack_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 12f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, JammedGuardian, new List<int>
					{
		19,
		20,
		21,
		22,
		23,
		24,
		25
					}, "attack_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 12f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, JammedGuardian, new List<int>
					{
		26,
		27,
		28,
		29,
		30,
		30,
		31,
		32,


					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, JammedGuardian, new List<int>
					{
		33,
		34,
		35,
		36,
		37,
		38,
		39,
		40

					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7;



				}
				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				shootpoint = new GameObject("fuck");
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = companion.sprite.WorldCenter;
				GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;
				bs.TargetBehaviors = new List<TargetBehaviorBase>
			{
				new TargetPlayerBehavior
				{
					Radius = 150f,
					LineOfSight = false,
					ObjectPermanence = true,
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.1f
					
				}
			};

				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(SalamanderScript)),
					LeadAmount = 0f,
					AttackCooldown = 0f,
					Cooldown = 1f,
					TellAnimation = "attack",
					FireAnimation = "tell",
					RequiresLineOfSight = true,
					MultipleFireEvents = true,
					Uninterruptible = false,


				},
				new DashBehavior()
				{
					ShootPoint = m_CachedGunAttachPoint,
					dashDistance = 3f,
					dashTime = 0.2f,
					doubleDashChance = 1,
					enableShadowTrail = false,
					Cooldown = 2,
					dashDirection = DashBehavior.DashDirection.Random,
					warpDashAnimLength = true,
					hideShadow = true,
					RequiresLineOfSight = false,
					
				}
				};
				bs.OtherBehaviors = new List<BehaviorBase>() {
				new CustomSpinBulletsBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					OverrideBulletName = "homing",
					NumBullets = 2,
					BulletMinRadius = 3.5f,
					BulletMaxRadius = 4,
					BulletCircleSpeed = 20,
					BulletsIgnoreTiles = true,
					RegenTimer = 0.1f,
					AmountOFLines = 12,
				}
				};
				bs.MovementBehaviors = new List<MovementBehaviorBase>() {
				new SeekTargetBehavior() {
					StopWhenInRange = true,
					CustomRange = 120,
					LineOfSight = true,
					ReturnToSpawn = true,
					SpawnTetherDistance = 0,
					PathInterval = 0.5f,
					SpecifyRange = false,
					MinActiveRange = 1,
					MaxActiveRange = 10
				}
			};


				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:jammed_guardian", companion.aiActor);


				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_001", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:jammed_guardian";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\jammedguardicon.png");
				PlanetsideModule.Strings.Enemies.Set("#JAMMED_GUARD", "Jammed Guardian");
				PlanetsideModule.Strings.Enemies.Set("#JAMMED_GUARD_SHORTDESC", "You Have Angered The Gods");
				PlanetsideModule.Strings.Enemies.Set("#JAMMED_GUARD_LONGDESC", "Only sent out if Kaliber has been enraged in a particular way, this guardian uses the souls of slain Gungeoneers to shield itself from oncoming attacks.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#JAMMED_GUARD";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#JAMMED_GUARD_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#JAMMED_GUARD_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:jammed_guardian");
				EnemyDatabase.GetEntry("psog:jammed_guardian").ForcedPositionInAmmonomicon = 1000;
				EnemyDatabase.GetEntry("psog:jammed_guardian").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:jammed_guardian").isNormalEnemy = true;
			}
		}


		private static string[] spritePaths = new string[]
		{
			
			//idles
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_001.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_006.png",

			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_right_001.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_right_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_right_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_right_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_right_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_right_006.png",

			
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_001.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_006.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_007.png",

			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_001.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_006.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_007.png",

			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_001.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_006.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_007.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_008.png",

			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_001.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_006.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_007.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_008.png",


		};

		public class EnemyBehavior : BraveBehaviour
		{

			private RoomHandler m_StartRoom;
			private void Update()
			{
				if (!base.aiActor.HasBeenEngaged) { CheckPlayerRoom(); }
			}
			private void CheckPlayerRoom()
			{

				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
				{
					base.aiActor.HasBeenEngaged = true;
				}

			}
			private void Start()
			{
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"));
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				//base.aiActor.HasBeenEngaged = true;
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(74).gameObject, base.aiActor.sprite.WorldCenter, Vector2.zero, 1f, false, true, false);
					AkSoundEngine.PostEvent("Play_BOSS_cannon_dash_01", base.aiActor.gameObject);
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED, true);//Done
				};

			}


		}

		public class SalamanderScript : Script
		{
			protected override IEnumerator Top() 
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
				}
				base.PostWwiseEvent("Play_ENM_kali_shockwave_01", null);
				this.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new SalamanderScript.Superball());
				yield return base.Wait(20);
				yield break;
			}
			public class Flames : Bullet
			{
				public Flames() : base("spiral", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					this.Projectile.specRigidbody.CollideWithOthers = false;
					base.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 25);
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
					base.ChangeSpeed(new Speed(40f, SpeedType.Absolute), 180);
					yield break;

				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
					{
						base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
					}
					if (!preventSpawningProjectiles)
					{

						base.PostWwiseEvent("Play_OBJ_nuke_blast_01", null);
						float num = base.RandomAngle();
						float Amount = 24;
						float Angle = 360 / Amount;
						for (int i = 0; i < Amount; i++)
						{
							base.Fire(new Direction(num + Angle * (float)i+10, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new BurstBullet());
							base.Fire(new Direction(num + Angle * (float)i+5, DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new BurstBullet());
							base.Fire(new Direction(num + Angle * (float)i, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new BurstBullet());
						}
					}
				}
			}

		}
		public class BurstBullet : Bullet
		{
			public BurstBullet() : base("reversible", false, false, false)
			{
			}
			protected override IEnumerator Top()
			{
				yield break;
			}
		}

		public class WallBullet : Bullet
		{
			public WallBullet() : base("default", false, false, false)
			{

			}
		}
		
	}
}








