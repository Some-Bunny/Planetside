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
using Pathfinding;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Planetside
{
	public class Glockulus : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "glockulus";
		private static tk2dSpriteCollectionData GlockulusCollection;
		public static GameObject shootpoint;
		public static void Init()
		{
			Glockulus.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Glockulus", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 50;
				companion.aiActor.MovementSpeed = 0;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(22.5f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.HasShadow = true;
				companion.aiActor.SetIsFlying(true, "Gamemode: Creative");
				companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;

				companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject; 
				companion.aiActor.healthHaver.SetHealthMaximum(22.5f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();

				companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>().dashColor = Color.grey;
				companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>().spawnShadows = true;

				companion.aiActor.gameObject.AddComponent<AfterImageTrailController>().spawnShadows = false;
				companion.aiActor.gameObject.AddComponent<tk2dSpriteAttachPoint>();
				companion.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();
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
					ManualWidth = 15,
					ManualHeight = 20,
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
					ManualWidth = 15,
					ManualHeight = 20,
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
					Type = DirectionalAnimation.DirectionType.EightWayOrdinal,
					Flipped = new DirectionalAnimation.FlipType[8],
					AnimNames = new string[]
					{
						"idle_north",
					   "idle_north_east",
						"idle_east",
					   "idle_south_east",
					   "idle_south",
						"idle_south_west",
					   "idle_west",
						"idle_north_west",


					}
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.EightWayOrdinal,
					Flipped = new DirectionalAnimation.FlipType[8],
					AnimNames = new string[]
					{
						"idle_north",
					   "idle_north_east",
						"idle_east",
					   "idle_south_east",
					   "idle_south",
						"idle_south_west",
					   "idle_west",
						"idle_north_west",
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
					name = "attack",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.EightWayOrdinal,
							Flipped = new DirectionalAnimation.FlipType[8],
							AnimNames = new string[]
							{
						"idle_north",
					   "idle_north_east",
						"idle_east",
					   "idle_south_east",
					   "idle_south",
						"idle_south_west",
					   "idle_west",
						"idle_north_west",
							}

						}
					}
				};
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);
				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;

				bool flag3 = GlockulusCollection == null;
				if (flag3)
				{
					GlockulusCollection = SpriteBuilder.ConstructCollection(prefab, "Glockulus_Collection");
					UnityEngine.Object.DontDestroyOnLoad(GlockulusCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], GlockulusCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, GlockulusCollection, new List<int>
					{
		            0
					}, "idle_north", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, GlockulusCollection, new List<int>
					{
					2
					}, "idle_north_east", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, GlockulusCollection, new List<int>
					{
					1
					}, "idle_north_west", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, GlockulusCollection, new List<int>
					{
		            3
					}, "idle_south", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, GlockulusCollection, new List<int>
					{
		            5
					}, "idle_south_east", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, GlockulusCollection, new List<int>
					{
		            4
					}, "idle_south_west", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, GlockulusCollection, new List<int>
					{
					7
					}, "idle_east", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, GlockulusCollection, new List<int>
					{
		            6
					}, "idle_west", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, GlockulusCollection, new List<int>
					{
		             8,
					 9,
					 10,
					 11,
					 12,
					 13

					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, GlockulusCollection, new List<int>
					{
					 8,
					 9,
					 10,
					 11,
					 12,
					 13

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, GlockulusCollection, new List<int>
					{
					0
					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;


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
					Radius = 35f,
					LineOfSight = true,
					ObjectPermanence = true,
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.25f,
				}
			};
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(NormalAttack)),
					LeadAmount = 0f,
					AttackCooldown = 0f,
					Cooldown = 2f,
					InitialCooldown = 0.5f,
					//TellAnimation = "attack",
					//FireAnimation = "tell",
					RequiresLineOfSight = true,
					MultipleFireEvents = true,
					//EnabledDuringAttack = new PowderSkullSpinBulletsBehavior(),
					//StopDuring = ShootBehavior.StopType.Attack,
					Uninterruptible = false,


				},
				new CustomDashBehavior()
				{
					//dashAnim = "wail",
					ShootPoint = m_CachedGunAttachPoint,
					dashDistance = 7f,
					dashTime = 0.33f,
					AmountOfDashes = 1,
					enableShadowTrail = true,
					Cooldown = 1,
					dashDirection = DashBehavior.DashDirection.Random,
					warpDashAnimLength = true,
					hideShadow = true,
					fireAtDashStart = true,
					InitialCooldown = 1f,
					AttackCooldown = 3,
					bulletScript = new CustomBulletScriptSelector(typeof(DashAttack)),
					//BulletScript = new CustomBulletScriptSelector(typeof(Wail)),
					//LeadAmount = 0f,
					//AttackCooldown = 5f,
					//InitialCooldown = 4f,
					//TellAnimation = "wail",
					//FireAnimation = "wail",
					RequiresLineOfSight = false,
					//Uninterruptible = true,
					//FireVfx = ,
					//ChargeVfx = ,
					//	MoveSpeedModifier = 0f,
				}//GlobalCooldown = 0.5f,
				};
				
			
				/*
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBeamBehavior() {
					//ShootPoint = m_CachedGunAttachPoint,

					firingTime = 3f,
					stopWhileFiring = true,
					//beam
					//BulletScript = new CustomBulletScriptSelector(typeof(SkellScript)),
					//LeadAmount = 0f,
					AttackCooldown = 5f,
					//InitialCooldown = 4f,
					RequiresLineOfSight = true,
					//StopDuring = ShootBehavior.StopType.Attack,
					//Uninterruptible = true,

				}
				};
				*/				
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:glockulus", companion.aiActor);

				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Glockulus/glockulus_idle_south_001", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:glockulus";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Glockulus/glockulus_idle_south_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\glockulussheet.png");
				PlanetsideModule.Strings.Enemies.Set("#THE_GLOCKULUS", "Glockulus");
				PlanetsideModule.Strings.Enemies.Set("#THE_GLOCKULUS_SHORTDESC", "Eye Spy");
				PlanetsideModule.Strings.Enemies.Set("#THE_GLOCKULUS_LONGDESC", "A sentinel from beyond the Curtain, it watches over the Gungeon for any... suspicious activity.\n\nSome Gungeonologists speculate that Glockuli are an off-breed of the Beholster, but no evidence has pointed towards that.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#THE_GLOCKULUS";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#THE_GLOCKULUS_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#THE_GLOCKULUS_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:glockulus");
				EnemyDatabase.GetEntry("psog:glockulus").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:glockulus").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:glockulus").isNormalEnemy = true;
			}
		}



		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_north_001.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_northeast_001.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_northwest_001.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_south_001.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_southwest_001.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_southeast_001.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_west_001.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_east_001.png",

			//death
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_001.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_002.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_003.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_004.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_005.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_006.png",



		};

		public class EnemyBehavior : BraveBehaviour
		{
			private RoomHandler m_StartRoom;

			public void Update()
			{
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				if (!base.aiActor.HasBeenEngaged)
				{
					CheckPlayerRoom();
				}
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
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.bulletBank.GetBullet("homing"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));

				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{ 	
				  AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Bite_01", base.aiActor.gameObject);


				};
			}
		}

		public class NormalAttack : Script 
		{
			protected override IEnumerator Top() 
			{

				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				}
				base.PostWwiseEvent("Play_WPN_eyeballgun_shot_01", null);
				base.PostWwiseEvent("Play_WPN_eyeballgun_impact_01", null);
				for (int i = -2; i <= 2; i++)
				{
					this.Fire(new Direction(i * 20, DirectionType.Aim, -1f), new Speed(9f, SpeedType.Absolute), new SpitNormal());
				}
				yield break;
			}
		}


		public class SpitNormal : Bullet
		{

			public SpitNormal() : base("default", false, false, false)
			{
			}
		}
		public class DashAttack : Script 
		{
			protected override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				base.PostWwiseEvent("Play_WPN_eyeballgun_shot_01", null);
				base.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(2.5f, SpeedType.Absolute), new Spit());
				yield break;
			}
		}
		public class Spit : Bullet
		{
			public Spit() : base("sniper", false, false, false)
			{

			}
			protected override IEnumerator Top()
			{

				base.ChangeSpeed(new Speed(13f, SpeedType.Absolute), 30);
				yield break;
			}
		}
	}
}





