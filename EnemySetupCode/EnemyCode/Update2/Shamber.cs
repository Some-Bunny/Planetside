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
	class Shamber : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "shamber_psog";
		public static GameObject shootpoint;
		//private static tk2dSpriteCollectionData ShamberCollection;

		public static Texture2D ShamberParticleTexture;


		public static void Init()
		{
			Shamber.BuildPrefab();
			ShamberParticleTexture = ResourceExtractor.GetTextureFromResource("Planetside/Resources2/ParticleTextures/shamberparticles.png");

		}
		public static void BuildPrefab()
		{
            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ShamberCollection").GetComponent<tk2dSpriteCollectionData>();
            Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("shamber material");

            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {


				prefab = EnemyBuilder.BuildPrefabBundle("Shamber", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(0, 0), false, true);
				var enemy = prefab.AddComponent<EnemyBehavior>();

                EnemyToolbox.QuickAssetBundleSpriteSetup(enemy.aiActor, Collection, mat);


                prefab.AddComponent<ShamberController>();
				prefab.AddComponent<KillOnRoomClear>();
				enemy.aiActor.IgnoreForRoomClear = true;
				enemy.aiActor.knockbackDoer.weight = 35;
				enemy.aiActor.MovementSpeed = 1f;
				enemy.aiActor.healthHaver.PreventAllDamage = false;
				enemy.aiActor.CollisionDamage = 1f;
				enemy.aiActor.aiAnimator.HitReactChance = 0f;
				enemy.aiActor.specRigidbody.CollideWithOthers = true;
				enemy.aiActor.specRigidbody.CollideWithTileMap = true;
				enemy.aiActor.PreventFallingInPitsEver = false;
				enemy.aiActor.healthHaver.ForceSetCurrentHealth(15f);
				enemy.aiActor.CollisionKnockbackStrength = 10f;
				enemy.aiActor.CanTargetPlayers = true;
				enemy.aiActor.healthHaver.SetHealthMaximum(15f, null, false);
				enemy.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;
				enemy.aiActor.SetIsFlying(true, "Gamemode: Creative", true, true);

				enemy.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("4db03291a12144d69fe940d5a01de376").ShadowObject;
				enemy.aiActor.HasShadow = true;

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
				DirectionalAnimation done = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "warpout",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "warpout",
						anim = done
					}
				};
				DirectionalAnimation aa = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "waprin",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "waprin",
						anim = aa
					}
				};
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);
				enemy.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				//bool flag3 = ShamberCollection == null;
				//if (flag3)
				{
					/*
					ShamberCollection = SpriteBuilder.ConstructCollection(prefab, "ArchGunjurer_Collection");
					UnityEngine.Object.DontDestroyOnLoad(ShamberCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], ShamberCollection);
					}
					*/
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
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

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
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


					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
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


					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
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


					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
					{

				 20,
				 21,
				 22,
				 23,
				 24,

					}, "waprin", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
					{

				 11,
				 12,
				 13,
				 14,
				 13,
				 14,
				 13,
				 14,
				 13,
				 14,
				 15,
				 16,
				 17,
				 18,
				 19

					}, "warpout", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
					{

				 25,
				 26,
				 27,
				 28,
				 29,
				 30,
				 31,
				 32




					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
					{

				 25,
				 26,
				 27,
				 28,
				 29,
				 30,
				 31,
				 32

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
					{

				 20,
				 21,
				 22,
				 23,
				 24,

					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
				}

				var intro = prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("waprin");
				intro.frames[1].eventInfo = "turnontrail";
				intro.frames[1].triggerEvent = true;

				var clip1 = prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("warpout");
				clip1.frames[8].eventInfo = "turnofftrail";
				clip1.frames[8].triggerEvent = true;

				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("warpout").frames[3].eventAudio = "Play_VO_gorgun_laugh_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("warpout").frames[3].triggerEvent = true;
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("die_right").frames[1].eventAudio = "Play_BOSS_doormimic_vanish_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("die_right").frames[1].triggerEvent = true;
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("die_left").frames[1].eventAudio = "Play_BOSS_doormimic_vanish_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("die_left").frames[1].triggerEvent = true;

				enemy.aiActor.specRigidbody.PixelColliders.Clear();
				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 10,
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

				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new TeleportBehavior()
				{

					AttackableDuringAnimation = true,
					AllowCrossRoomTeleportation = false,
					teleportRequiresTransparency = false,
					hasOutlinesDuringAnim = true,
					ManuallyDefineRoom = false,
					MaxHealthThreshold = 1f,
					StayOnScreen = false,
					AvoidWalls = false,
					GoneTime = 1f,
					OnlyTeleportIfPlayerUnreachable = false,
					MinDistanceFromPlayer = 4f,
					MaxDistanceFromPlayer = -1f,
					teleportInAnim = "waprin",
					teleportOutAnim = "warpout",
					AttackCooldown = 3f,
					InitialCooldown = 0f,
					RequiresLineOfSight = false,
					roomMax = new Vector2(0,0),
					roomMin = new Vector2(0,0),
					GlobalCooldown = 0.5f,
					Cooldown = 5f,

					CooldownVariance = 1f,
					InitialCooldownVariance = 0f,
					goneAttackBehavior = null,
					IsBlackPhantom = false,
					GroupName = null,
					GroupCooldown = 0f,
					MinRange = 0,
					Range = 0,
					MinHealthThreshold = 0,
					MaxUsages = 0,
					AccumulateHealthThresholds = true,
					targetAreaStyle = null,
					HealthThresholds = new float[0],
					MinWallDistance = 0,
				}
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
				Game.Enemies.Add("psog:shamber", enemy.aiActor);

				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Ammocom/shamberammonomiconICON", SpriteBuilder.ammonomiconCollection);
				if (enemy.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
				}
				enemy.encounterTrackable = enemy.gameObject.AddComponent<EncounterTrackable>();
				enemy.encounterTrackable.journalData = new JournalEntry();
				enemy.encounterTrackable.EncounterGuid = "psog:shamber";
				enemy.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				enemy.encounterTrackable.journalData.SuppressKnownState = false;
				enemy.encounterTrackable.journalData.IsEnemy = true;
				enemy.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				enemy.encounterTrackable.ProxyEncounterGuid = "";
				enemy.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Ammocom/shamberammonomiconICON";
				enemy.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("shamberammonomicoenrtytab");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\shamberammonomicoenrtytab.png");
                PlanetsideModule.Strings.Enemies.Set("#SHAMBER", "Shamber");
				PlanetsideModule.Strings.Enemies.Set("#SHAMBER_SHORT", "Tee Hee Hee!");
				PlanetsideModule.Strings.Enemies.Set("#SHAMBER_LONGDESC", "A lively, hungry spirit that feeds itself on bullets of any kind.\n\nIn the past, Shambers would seek out Gungeoneers and loot their ammo supplies until they realised that it was much easier to just catch bullets from mid-air without the hassle of dealing with a surprised Gungeoneer.");
				enemy.encounterTrackable.journalData.PrimaryDisplayName = "#SHAMBER";
				enemy.encounterTrackable.journalData.NotificationPanelDescription = "#SHAMBER_SHORT";
				enemy.encounterTrackable.journalData.AmmonomiconFullEntry = "#SHAMBER_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(enemy.gameObject, "psog:shamber");
				EnemyDatabase.GetEntry("psog:shamber").ForcedPositionInAmmonomicon = 27;
				EnemyDatabase.GetEntry("psog:shamber").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:shamber").isNormalEnemy = true;
			}

		}

		private static string[] spritePaths = new string[]
        {
			//Idle
			"Planetside/Resources/Enemies/Shamber/shamber_idle_001.png",
			"Planetside/Resources/Enemies/Shamber/shamber_idle_002.png",
			"Planetside/Resources/Enemies/Shamber/shamber_idle_003.png",
			"Planetside/Resources/Enemies/Shamber/shamber_idle_004.png",
			"Planetside/Resources/Enemies/Shamber/shamber_idle_005.png",
			"Planetside/Resources/Enemies/Shamber/shamber_idle_006.png",
			"Planetside/Resources/Enemies/Shamber/shamber_idle_007.png",
			"Planetside/Resources/Enemies/Shamber/shamber_idle_008.png",
			"Planetside/Resources/Enemies/Shamber/shamber_idle_009.png",
			"Planetside/Resources/Enemies/Shamber/shamber_idle_010.png",
			"Planetside/Resources/Enemies/Shamber/shamber_idle_011.png",
			//Fade Out
			"Planetside/Resources/Enemies/Shamber/shamber_fadeout_001.png",
			"Planetside/Resources/Enemies/Shamber/shamber_fadeout_002.png",
			"Planetside/Resources/Enemies/Shamber/shamber_fadeout_003.png",
			"Planetside/Resources/Enemies/Shamber/shamber_fadeout_004.png",
			"Planetside/Resources/Enemies/Shamber/shamber_fadeout_005.png",
			"Planetside/Resources/Enemies/Shamber/shamber_fadeout_006.png",
			"Planetside/Resources/Enemies/Shamber/shamber_fadeout_007.png",
			"Planetside/Resources/Enemies/Shamber/shamber_fadeout_008.png",
			"Planetside/Resources/Enemies/Shamber/shamber_fadeout_009.png",

			"Planetside/Resources/Enemies/Shamber/shamber_fadein_001.png",
			"Planetside/Resources/Enemies/Shamber/shamber_fadein_002.png",
			"Planetside/Resources/Enemies/Shamber/shamber_fadein_003.png",
			"Planetside/Resources/Enemies/Shamber/shamber_fadein_004.png",
			"Planetside/Resources/Enemies/Shamber/shamber_fadein_005.png",

			"Planetside/Resources/Enemies/Shamber/shamber_death_001.png",
			"Planetside/Resources/Enemies/Shamber/shamber_death_002.png",
			"Planetside/Resources/Enemies/Shamber/shamber_death_003.png",
			"Planetside/Resources/Enemies/Shamber/shamber_death_004.png",
			"Planetside/Resources/Enemies/Shamber/shamber_death_005.png",
			"Planetside/Resources/Enemies/Shamber/shamber_death_006.png",
			"Planetside/Resources/Enemies/Shamber/shamber_death_007.png",
			"Planetside/Resources/Enemies/Shamber/shamber_death_008.png",

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
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom && GameManager.Instance.PrimaryPlayer.IsInCombat == true)
				{
					base.aiActor.HasBeenEngaged = true;
					//ShamberController.BulletsEaten = 0;
				}
				yield break;
			}
			private void Start()
			{
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) => {  };
			}
		}
	}
}

