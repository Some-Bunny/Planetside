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
using Pathfinding;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Planetside
{
	public class Barretina : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "barretina";
		private static tk2dSpriteCollectionData BarretinaCollection;
		public static GameObject shootpoint;
		public static void Init()
		{
			Barretina.BuildPrefab();
		}


		public static void BuildPrefab()
		{
			
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{

				prefab = EnemyBuilder.BuildPrefab("Barretina", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 200;
				companion.aiActor.MovementSpeed = 0.75f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(40f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.HasShadow = true;
				companion.aiActor.SetIsFlying(true, "Gamemode: Creative");
				companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject; 
				companion.aiActor.healthHaver.SetHealthMaximum(40f, null, false);

				companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>().dashColor = Color.grey;
				companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>().spawnShadows = true;

				companion.aiActor.specRigidbody.PixelColliders.Clear();
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
					ManualWidth = 23,
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
					ManualWidth = 23,
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
				bool flag3 = BarretinaCollection == null;
				if (flag3)
				{
					BarretinaCollection = SpriteBuilder.ConstructCollection(prefab, "Barretina_Collection");
					UnityEngine.Object.DontDestroyOnLoad(BarretinaCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], BarretinaCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, new List<int>
					{
		            0,
					1,
					2,
					3
					}, "idle_north", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, new List<int>
					{
					8,
					9,
					10,
					11
					}, "idle_north_east", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, new List<int>
					{
					4,
					5,
					6,
					7

					}, "idle_north_west", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, new List<int>
					{
		            12,
					13,
					14,
					15

					}, "idle_south", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, new List<int>
					{
		            20,
					21,
					22,
					23
					}, "idle_south_east", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, new List<int>
					{
		            16,
					17,
					18,
					19
					}, "idle_south_west", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, new List<int>
					{
					28,
					29,
					30,
					31
					}, "idle_east", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, new List<int>
					{
		            24,
					25,
					26,
					27
					}, "idle_west", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, new List<int>
					{
		             32,
					 33,
					 34,
					 35,
					 36,
					 37,
					 38,
					 39,
					 40

					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, new List<int>
					{
					 32,
					 33,
					 34,
					 35,
					 36,
					 37,
					 38,
					 39,
					 40

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;



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
					AttackCooldown = 2f,
					Cooldown = 4f,
					InitialCooldown = 0.5f,
					RequiresLineOfSight = true,
					MultipleFireEvents = false,
					Uninterruptible = true,
					

				},
				new CustomDashBehavior()
				{
					ShootPoint = m_CachedGunAttachPoint,
					dashDistance = 7f,
					dashTime = 0.5f,
					AmountOfDashes = 2,
					enableShadowTrail = false,
					Cooldown = 2f,
					dashDirection = DashBehavior.DashDirection.Random,
					warpDashAnimLength = true,
					hideShadow = true,
					fireAtDashStart = true,
					InitialCooldown = 1f,
					bulletScript = new CustomBulletScriptSelector(typeof(DashAttack)),
					RequiresLineOfSight = false,
					AttackCooldown = 0.1f,

				}
				};
				bs.MovementBehaviors = new List<MovementBehaviorBase>() {
				new SeekTargetBehavior() {
					StopWhenInRange = true,
					CustomRange = 6,
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
				Game.Enemies.Add("psog:barretina", companion.aiActor);

				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Berretina/berretina_idle_south_001", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:barretina";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Berretina/berretina_idle_south_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\barretinaiconthing.png");
				PlanetsideModule.Strings.Enemies.Set("#THE_BARRETINA", "Barretina");
				PlanetsideModule.Strings.Enemies.Set("#THE_BARRETINA_SHORTDESC", "Sn-eye-per");
				PlanetsideModule.Strings.Enemies.Set("#THE_BARRETINA_LONGDESC", "A sentinel of greater size from beyond the Curtain, blessed with the gift of greater manueverability.\n\nTheories circulated that these foes were a sign of something greater to be discovered in the Gungeon, but again, no evidence supports that... yet.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#THE_BARRETINA";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#THE_BARRETINA_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#THE_BARRETINA_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:barretina");
				EnemyDatabase.GetEntry("psog:barretina").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:barretina").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:barretina").isNormalEnemy = true;
			}

		}




		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/Berretina/berretina_idle_north_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_north_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_north_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_north_004.png",

			"Planetside/Resources/Enemies/Berretina/berretina_idle_northeast_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northwest_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northwest_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northwest_004.png",

			"Planetside/Resources/Enemies/Berretina/berretina_idle_northeast_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northeast_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northeast_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northeast_004.png",

			"Planetside/Resources/Enemies/Berretina/berretina_idle_south_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_south_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_south_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_south_004.png",

			"Planetside/Resources/Enemies/Berretina/berretina_idle_southwest_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southwest_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southwest_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southwest_004.png",

			"Planetside/Resources/Enemies/Berretina/berretina_idle_southeast_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southeast_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southeast_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southeast_004.png",

			"Planetside/Resources/Enemies/Berretina/berretina_idle_west_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_west_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_west_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_west_004.png",

			"Planetside/Resources/Enemies/Berretina/berretina_idle_east_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_east_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_east_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_east_004.png",

			//death
			"Planetside/Resources/Enemies/Berretina/berretina_die_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_004.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_005.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_006.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_007.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_008.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_009.png",

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
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
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
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));

				}
				base.PostWwiseEvent("Play_WPN_eyeballgun_shot_01", null);
				base.PostWwiseEvent("Play_WPN_eyeballgun_impact_01", null);
				for (int i = -4; i <= -1; i++)
				{
					this.Fire(new Direction(i * 20, DirectionType.Aim, -1f), new Speed(9f, SpeedType.Absolute), new SpitNormal());
				}
				this.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(3f, SpeedType.Absolute), new SpitLarge());
				for (int i = 1; i <= 4; i++)
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
		public class SpitLarge : Bullet
		{
			public SpitLarge() : base("sniper", false, false, false)
			{
			}
			protected override IEnumerator Top()
			{
				base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 20);
				yield return base.Wait(20);
				base.ChangeSpeed(new Speed(20f, SpeedType.Absolute), 20);
				yield break;
			}
		}
		public class DashAttack : Script 
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				base.PostWwiseEvent("Play_WPN_eyeballgun_shot_01", null);
				for (int i = -1; i <= 1; i++)
				{
					base.Fire(new Direction(10*i, DirectionType.Aim, -1f), new Speed(2f, SpeedType.Absolute), new Spit());
				}
				base.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(12f, SpeedType.Absolute), new SpitNormal());
				yield break;
			}
		}
		public class Spit : Bullet
		{
			public Spit() : base("default", false, false, false)
			{
			}
			protected override IEnumerator Top()
			{
				base.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 20);
				yield break;
			}
		}
	}
}





