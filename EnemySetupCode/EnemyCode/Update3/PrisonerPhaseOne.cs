using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;	
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;

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
				//companion.aiActor.healthHaver.SetHealthMaximum(1400f);
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

				/*
				DirectionalAnimation blooptell = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					AnimNames = new string[]
					{
						"blooptell",

					},
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "blooptell",
						anim = blooptell
					}
				};

				DirectionalAnimation bloop = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					AnimNames = new string[]
	{
						"bloop",

	},
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "bloop",
						anim = bloop
					}
				};

				DirectionalAnimation bottletell = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					AnimNames = new string[]
					{
						"bottletell",

					},
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "bottletell",
						anim = bottletell
					}
				};
				DirectionalAnimation bottle = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					AnimNames = new string[]
	{
						"bottle",

	},
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "bottle",
						anim = bottle
					}
				};

				DirectionalAnimation roartell = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					AnimNames = new string[]
{
						"roartell",

},
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "roartell",
						anim = roartell
					}
				};

				DirectionalAnimation roar = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					AnimNames = new string[]
{
						"roar",

},
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "roar",
						anim = roar
					}
				};
				*/

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "introidle", new string[1], new DirectionalAnimation.FlipType[1]);

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "intro", new string[1], new DirectionalAnimation.FlipType[1]);

				/*
				DirectionalAnimation introidle = new DirectionalAnimation
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
						anim = introidle
					}
				};
				*/

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
					12,
					13,
					12,
					11

					}, "introidle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 4f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, PrisonerPhaseOneSpriteCollection, new List<int>
					{

					11,
					12,
					13,
					12,

					11,
					12,
					13,
					12,
					
					11,
					12,
					13,
					12,
					11,
					12,
					13,
					12,
					11,
					12,
					13,
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
					18,//ChainBreak1 (27)
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
					24,//Chainbreak2 (43)
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
					35,
					36,
					37,
					36,
					35,
					35,
					36,
					37,
					37,
					36,
					35,
					35,
					36,
					37,



					}, "intro", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;



					EnemyToolbox.AddSoundsToAnimationFrame(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "intro", new Dictionary<int, string> { {27, "Play_BOSS_lichC_hook_01" }, { 43, "Play_BOSS_lichC_hook_01" } });


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
				/*
				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
				{

					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0.4f,
					Behavior = new ShootBehavior{
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(BigWhips)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 3f,
					TellAnimation = "roartell",
					FireAnimation = "roar",
					RequiresLineOfSight = true,
					MultipleFireEvents = true,
					Uninterruptible = false,
						},
						NickName = "ROAR"

					},
					
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0.6f,
					Behavior = new ShootBehavior{
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(BloopScript)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 3f,
					TellAnimation = "blooptell",
					FireAnimation = "bloop",
					RequiresLineOfSight = true,
					MultipleFireEvents = true,

					Uninterruptible = false,
						},
						NickName = "Bloop"

					},
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0.7f,
					Behavior = new DashBehavior{
					MaxHealthThreshold = 1f,
					AttackCooldown = 1f,
					InitialCooldown = 0f,
					RequiresLineOfSight = false,
					GlobalCooldown = 0.2f,
					Cooldown = 6f,
					CooldownVariance = 1f,
					InitialCooldownVariance = 0f,
					IsBlackPhantom = false,
					GroupName = null,
					GroupCooldown = 0f,
					MinRange = 0,
					Range = 0,
					MinHealthThreshold = 0,
					enableShadowTrail = false,
					MaxUsages = 0,
					AccumulateHealthThresholds = true,
					targetAreaStyle = null,
					HealthThresholds = new float[0],
					MinWallDistance = 0,
					ShootPoint = m_CachedGunAttachPoint,
					dashDistance = 7f,
					dashTime = 0.2f,
					doubleDashChance = 0.7f,
					dashDirection = DashBehavior.DashDirection.Random,
					warpDashAnimLength = true,
					hideShadow = true,
					fireAtDashStart = true,
					bulletScript = new CustomBulletScriptSelector(typeof(SpawnDash)),
					},
					NickName = "Dash"
					},

				};
				*/


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

				GenericIntroDoer miniBossIntroDoer = fuckyouprefab.AddComponent<GenericIntroDoer>();
				fuckyouprefab.AddComponent<BulletBankIntro>();
				miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
				miniBossIntroDoer.initialDelay = 0.15f;
				miniBossIntroDoer.cameraMoveSpeed = 14;
				miniBossIntroDoer.specifyIntroAiAnimator = null;
				miniBossIntroDoer.BossMusicEvent = "Play_PrisonerTheme";
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

		public class SpawnDash : Script
		{
			protected override IEnumerator Top()
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("snakeBullet"));
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
				}

				Exploder.DoDistortionWave(base.BulletBank.sprite.WorldCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
				base.PostWwiseEvent("Play_ENM_blobulord_reform_01", null);
				base.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(0, SpeedType.Absolute), new SpawnDash.Superball());

				yield break;
			}
			public float distortionMaxRadius = 30f;
			public float distortionDuration = 0.6f;
			public float distortionIntensity = 0.1f;
			public float distortionThickness = 0.2f;
			public class Superball : Bullet
			{
				public Superball() : base("big", false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
					{
						base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("snakeBullet"));
					}
					yield return this.Wait(120);
					base.Vanish(false);
					yield break;
				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (!preventSpawningProjectiles)
					{
						base.PostWwiseEvent("Play_BOSS_Rat_Cheese_Burst_02", null);
						int RNG = UnityEngine.Random.Range(0, 10);
						bool WeakShot = RNG == 1;
						if (WeakShot)
						{
							var advanced = new List<string>
						{
						"apache_bullet",
						"kyle_bullet",
						"wow_bullet",
						"spcreat_bullet",
						"gr_bullet",
						 "spapi_bullet",


						};
							string guid = BraveUtility.RandomElement<string>(advanced);
							var Enemy = EnemyDatabase.GetOrLoadByGuid(guid);
							Enemy.healthHaver.SetHealthMaximum(15f);
							AIActor.Spawn(Enemy.aiActor, this.Projectile.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Default, true);
						}
						else
						{
							var basic = new List<string> {

						"an3s_bullet",
						"blazey_bullet",
					   "bleak_bullet",
					   "turbo_bullet",
					   "bot_bullet",
					   "bunny_bullet",
					   "cel_bullet",
					   "glaurung_bullet",
					   "hunter_bullet",
					   "king_bullet",
					   "neighborino_bullet",
					   "nevernamed_bullet",
					   "panda_bullet",
					   "retrash_bullet",
					   "skilotar_bullet",
					   "notsoai_bullet"
						};
							string guid = BraveUtility.RandomElement<string>(basic);
							var Enemy = EnemyDatabase.GetOrLoadByGuid(guid);
							Enemy.healthHaver.SetHealthMaximum(20f);
							
							AIActor.Spawn(Enemy.aiActor, this.Projectile.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Default, true);

						}
						float num = base.RandomAngle();
						float Amount = 12;
						float Angle = 360 / Amount;
						for (int i = 0; i < Amount; i++)
						{
							base.Fire(new Direction(num + Angle * (float)i + 10, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SpawnDash.BurstBullet());
						}
						return;
					}
				}
			}
			public class BurstBullet : Bullet
			{
				public BurstBullet() : base("snakeBullet", false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
					yield return base.Wait(60);
					base.Vanish(false);
					yield break;
				}
			}
		}

		public class SpawnBottle : Script
		{
			protected override IEnumerator Top()
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
				}

				base.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(3, SpeedType.Absolute), new SpawnBottle.Superball());
				float Amount = UnityEngine.Random.Range(20, 33);
				base.PostWwiseEvent("Play_ENV_time_shatter_01", null);
				Exploder.DoDistortionWave(base.BulletBank.sprite.WorldCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
				for (int i = 0; i < Amount; i++)
				{
					float num = base.RandomAngle();
					base.Fire(new Direction(num, DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(7, 12), SpeedType.Absolute), new SpawnBottle.Shrapnel());
					yield return this.Wait(3);
				}
				yield break;
			}
			public float distortionMaxRadius = 30f;
			public float distortionDuration = 2f;
			public float distortionIntensity = 0.7f;
			public float distortionThickness = 0.1f;
			public class Superball : Bullet
			{
				public Superball() : base("big", false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
					{
						base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("snakeBullet"));
					}
					base.PostWwiseEvent("Play_OBJ_lantern_shatter_01", null);
					base.ChangeSpeed(new Speed(20f, SpeedType.Absolute), 80);
					yield return this.Wait(120);
					base.Vanish(false);
					yield break;
				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (!preventSpawningProjectiles)
					{
						base.PostWwiseEvent("Play_BOSS_Rat_Cheese_Burst_02", null);
						int RNG = UnityEngine.Random.Range(0, 10);
						bool WeakShot = RNG == 1;
						if (WeakShot)
						{
						var advanced = new List<string>
						{
						"apache_bullet",
						"kyle_bullet",
						"wow_bullet",
						"spcreat_bullet",
						"gr_bullet",
						"spapi_bullet",

						};
							string guid = BraveUtility.RandomElement<string>(advanced);
							var Enemy = EnemyDatabase.GetOrLoadByGuid(guid);
							Enemy.healthHaver.SetHealthMaximum(15f);
							AIActor.Spawn(Enemy.aiActor, this.Projectile.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Default, true);
						}
						else
						{
						var basic = new List<string> { 
						
				        "an3s_bullet",
						"turbo_bullet",
				        "blazey_bullet",
					   "bleak_bullet",
					   "bot_bullet",
					   "bunny_bullet",
					   "cel_bullet",
					   "glaurung_bullet",
					   "hunter_bullet",
					   "king_bullet",
					   "neighborino_bullet",
					   "nevernamed_bullet",
					   "panda_bullet",
					   "retrash_bullet",
					   "skilotar_bullet",
					   "notsoai_bullet",
					    };
							string guid = BraveUtility.RandomElement<string>(basic);
							var Enemy = EnemyDatabase.GetOrLoadByGuid(guid);
							Enemy.healthHaver.SetHealthMaximum(24f);
							AIActor.Spawn(Enemy.aiActor, this.Projectile.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Default, true);
						}


						float num = base.RandomAngle();
						float Amount = 12;
						float Angle = 360 / Amount;
						for (int i = 0; i < Amount; i++)
						{
							base.Fire(new Direction(num + Angle * (float)i + 10, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new BurstBullet());
						}
						return;
					}

				}

			}
			public class BurstBullet : Bullet
			{
				public BurstBullet() : base("snakeBullet", false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
					yield return base.Wait(60);
					base.Vanish(false);
					yield break;
				}
			}
			public class Shrapnel : Bullet
			{
				public Shrapnel() : base("poundSmall", false, false, false)
				{
				}
			}
		}

		public class BigWhips : Script
		{
			protected override IEnumerator Top()
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("snakeBullet"));
				}
				float Aim = base.AimDirection;
				base.PostWwiseEvent("Play_ENM_bigshroom_roar_01", null);

				for (int i = 0; i < 50; i++)
				{
					base.Fire(new Direction(Aim +i*7.2f, DirectionType.Absolute, -1f), new Speed(9, SpeedType.Absolute), new BigWhips.BasicBullet());
					base.Fire(new Direction(Aim - i*7.2f, DirectionType.Absolute, -1f), new Speed(9, SpeedType.Absolute), new BigWhips.BasicBullet());
					yield return this.Wait(1.66f);

				}
				yield break;
			}
			public class BasicBullet : Bullet
			{
				public BasicBullet() : base("snakeBullet", false, false, false)
				{
				}

			}
		}

		public class BloopScript : Script
		{
			protected override IEnumerator Top()
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("ring"));
				}
				float Aim = base.AimDirection;
				float radius = 0.075f;
				float delta = 15f;
				float startDirection = AimDirection;
				base.PostWwiseEvent("Play_ENM_blobulord_bubble_01", null);
				for (int j = 0; j < 24; j++)
				{
					base.Fire(new Direction(-90f, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new BloopScript.OopsANull("ring", this, startDirection + (float)j * delta, radius));
				}
			    radius = 0.115f;
				delta = 180f;
				for (int j = 0; j < 11; j++)
				{
					base.Fire(new Direction(-90f, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new BloopScript.OopsANull("ring", this, startDirection + (float)j * delta, radius));
					base.Fire(new Direction(-90f, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new BloopScript.OopsANull("ring", this, startDirection + (float)j * delta+180, radius));
					radius -= 0.01f;
				}
				yield break;
			}
			public class OopsANull : Bullet
			{
				public OopsANull(string BulletType, BloopScript parent, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
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
							base.ChangeSpeed(new Speed(18f, SpeedType.Absolute), 120);
							base.ChangeDirection(new Direction(this.m_parent.GetAimDirection(1f, 10f), DirectionType.Absolute, -1f), 20);
							base.StartTask(this.ChangeSpinSpeedTask(180f, 240));
						}
						base.UpdateVelocity();
						centerPosition += this.Velocity / 60f;
						if (i < 40)
						{
							radius += m_radius;
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
				private BloopScript m_parent;
				private float m_angle;
				private float m_spinSpeed;
				private float m_radius;
				private string m_bulletype;

			}

		}




		public class PrisonerController : BraveBehaviour
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

		};
	}

}








