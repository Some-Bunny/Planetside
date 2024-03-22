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
	public class BulletBankMan : AIActor
	{
		public static GameObject fuckyouprefab;
		public static readonly string guid = "Bullet_Banker";
		//private static tk2dSpriteCollectionData TheBulletBankClooection;
		public static GameObject shootpoint;
		public static GameObject shootpoint1;
		//private static Texture2D BossCardTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside/Resources/BossCards/bulletbanker_bosscard.png");
		public static string TargetVFX;
		public static Texture _gradTexture;

		public static void Init()
		{

			BulletBankMan.BuildPrefab();
		}

		public static void BuildPrefab()
		{

            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("BulletBankerCollection").GetComponent<tk2dSpriteCollectionData>();
            Material mate = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("bulletbanker material");

            if (fuckyouprefab == null || !BossBuilder.Dictionary.ContainsKey(guid))
			{
				fuckyouprefab = BossBuilder.BuildPrefabBundle("Bullet Banker", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(8, 9), false, true);
				var companion = fuckyouprefab.AddComponent<BankerEnemyBehavior>();

                EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, Collection, mate);

                companion.aiActor.knockbackDoer.weight = 200;
				companion.aiActor.MovementSpeed = 3f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0.05f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(350f);
				companion.aiActor.healthHaver.SetHealthMaximum(350f);
				companion.aiActor.CollisionKnockbackStrength = 2f;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.HasShadow = true;
				companion.aiActor.SetIsFlying(true, "Gamemode: Creative");
				//companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.gameObject.AddComponent<AfterImageTrailController>().spawnShadows = false;
				companion.aiActor.gameObject.GetComponent<tk2dSpriteAttachPoint>();
				companion.aiActor.gameObject.GetOrAddComponent<ObjectVisibilityManager>();
				//fuckyouprefab.name = companion.aiActor.OverrideDisplayName;
				companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("4db03291a12144d69fe940d5a01de376").ShadowObject;
				companion.aiActor.HasShadow = true;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(1.5f, 0.25f));

                companion.aiActor.healthHaver.damageTypeModifiers = new List<DamageTypeModifier>();
                DamageTypeModifier fireRes = new DamageTypeModifier();
                fireRes.damageMultiplier = 0f;
                fireRes.damageType = CoreDamageTypes.Fire;
                companion.aiActor.healthHaver.damageTypeModifiers.Add(fireRes);

                var nur = companion.aiActor;
                nur.EffectResistances = new ActorEffectResistance[]
                {
                    new ActorEffectResistance()
                    {
                        resistAmount = 1,
                        resistType = EffectResistanceType.Fire
                    },
                };


                TrailRenderer tr;

				var tro = companion.aiActor.gameObject.AddChild("trail object");
				tro.transform.position = companion.aiActor.transform.position;
				tro.transform.localPosition = new Vector2(1.5f, 2.5f);

				tr = tro.AddComponent<TrailRenderer>();
				tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				tr.receiveShadows = false;
				var mat = new Material(Shader.Find("Sprites/Default"));
				mat.mainTexture = _gradTexture;
				mat.SetColor("_Color", new Color(3f, 3f, 3f, 0.9f));
				tr.material = mat;
				tr.time = 1f;
				tr.minVertexDistance = 0.1f;
				tr.startWidth = 1.2f;
				tr.endWidth = 0f;
				tr.startColor = Color.white;
				tr.endColor = new Color(1f, 0f, 0f, 0f);


				companion.aiActor.specRigidbody.PixelColliders.Clear();
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
					ManualWidth = 36,
					ManualHeight = 40,
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
					ManualWidth = 36,
					ManualHeight = 40,
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

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "start_heal", new string[] { "start_heal" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "heal", new string[] { "heal" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "endheal", new string[] { "endheal" }, new DirectionalAnimation.FlipType[0]);


                //endheal
                //bool flag3 = TheBulletBankClooection == null;
                //if (flag3)
                {
                    /*
					TheBulletBankClooection = SpriteBuilder.ConstructCollection(fuckyouprefab, "TheBulletBank-Clooection");
					UnityEngine.Object.DontDestroyOnLoad(TheBulletBankClooection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], TheBulletBankClooection);
					}
					*/
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
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
					9
					

					}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					10,
					11,
					12,
					13

					}, "blooptell", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
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

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					18,
					19,
					20,
					21,
					22,
					23

					}, "bottletell", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
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


					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				40,
				41,
				42,
				43,
				44,
				45,
				46
					}, "roartell", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
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

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				53,
				54,
				55,
				56,
				57,
				58,
				59,
				60,
				61,
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
				72,
				73


					}, "intro", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				74,
				75,
				76,
				77,
				78,
				79,
				80,
				81,
				82,
				83,
				84
					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

                    SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
                    {
                85,
				86,
				87,
				87,
				88,
				88,
				89,
				89,
                    }, "start_heal", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
                    {
                89,
				89,
				88,
				87,
				86,
				85
                    }, "endheal", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
                    {
                90,
				91,
				92,
				93
                    }, "heal", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;


                }
				EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "start_heal", new Dictionary<int, string>() { {1, "Play_ENM_screamer_scream_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "heal", new Dictionary<int, string>() { { 0, "Play_ENM_blobulord_reform_01" } });

                //					AkSoundEngine.PostEvent("Play_ENM_screamer_scream_01", base.gameObject);

                fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[15].eventAudio = "Play_ENV_time_shatter_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[15].triggerEvent = true;

				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[17].eventAudio = "Play_ENM_bombshee_scream_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[17].triggerEvent = true;

				var bs = fuckyouprefab.GetComponent<BehaviorSpeculator>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				
				shootpoint = new GameObject("attach");
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = new Vector2(1.5f, 2.5f);
				GameObject m_CachedGunAttachPoint = companion.transform.Find("attach").gameObject;

				shootpoint1 = new GameObject("bollocks");
				shootpoint1.transform.parent = companion.transform;
				shootpoint1.transform.position = new Vector2(1.1f, 1.1f);
				GameObject m_CachedGunAttachPoint1 = companion.transform.Find("bollocks").gameObject;
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

					Probability = 0.4f,
					Behavior = new ShootBehavior{
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(BigWhips)),
					LeadAmount = 0f,
					AttackCooldown = 1.5f,
					Cooldown = 3f,
					TellAnimation = "roartell",
					FireAnimation = "roar",
					RequiresLineOfSight = true,
					MultipleFireEvents = true,
					Uninterruptible = false,
                    MaxEnemiesInRoom = 3,
                        },
						NickName = "ROAR"

					},
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0.7f,
					Behavior = new ShootBehavior{
					ShootPoint = m_CachedGunAttachPoint1,
					BulletScript = new CustomBulletScriptSelector(typeof(SpawnBottle)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 2f,
					TellAnimation = "bottletell",
					FireAnimation = "bottle",
					RequiresLineOfSight = true,

					MultipleFireEvents = true,
					Uninterruptible = false,
					MaxEnemiesInRoom = 6,
					
						},
						NickName = "Bottle"

					},
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0.7f,
					Behavior = new ShootBehavior{
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(BloopScript)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 2f,
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

                    Probability = 0f,
                    Behavior = new ShootBehavior{
                    ShootPoint = m_CachedGunAttachPoint,
                    BulletScript = new CustomBulletScriptSelector(typeof(HealScript)),
                    LeadAmount = 0f,
                    AttackCooldown = 1f,
                    Cooldown = 24,
					InitialCooldown = 10,
                    TellAnimation = "start_heal",
                    FireAnimation = "heal",
					PostFireAnimation = "endheal",
                    RequiresLineOfSight = true,
                    MultipleFireEvents = true,
                    Uninterruptible = true,
					MaxUsages = 6,
                        },
                        NickName = "FleshPrison"

                    },

                    new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0.85f,
					Behavior = new DashBehavior{
					MaxHealthThreshold = 1f,
					AttackCooldown = 1f,
					InitialCooldown = 0f,
					RequiresLineOfSight = false,
					GlobalCooldown = 0.2f,
					Cooldown = 4.5f,
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
					dashDistance = 6f,
					dashTime = 0.2f,
					doubleDashChance = 0.7f,
					dashDirection = DashBehavior.DashDirection.Random,
					warpDashAnimLength = true,
					hideShadow = true,
					fireAtDashStart = true,
					MaxEnemiesInRoom = 6,
					bulletScript = new CustomBulletScriptSelector(typeof(SpawnDash)),
					},
					NickName = "Dash"
					},

				};



				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:bullet_banker", companion.aiActor);



				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/BulletBanker/bulletbanker_idle_001", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:bullet_banker";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/BulletBanker/bulletbanker_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("bankericon");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\bankericon.png");
                PlanetsideModule.Strings.Enemies.Set("#BULLETBANKNAME", "Bullet Banker");
				PlanetsideModule.Strings.Enemies.Set("#BULLETBANKSHDES", "Ammunition Exception");
				PlanetsideModule.Strings.Enemies.Set("#BULLETBANKSLES", "An enormous, sentient bullet who has tasked itself with gathering the souls of those who have begun forging their own creations within the confines of the Gungeon.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#BULLETBANKNAME";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#BULLETBANKSHDES";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#BULLETBANKSLES";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:bullet_banker");
				EnemyDatabase.GetEntry("psog:bullet_banker").ForcedPositionInAmmonomicon = 201;
				EnemyDatabase.GetEntry("psog:bullet_banker").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:bullet_banker").isNormalEnemy = true;

				GenericIntroDoer miniBossIntroDoer = fuckyouprefab.AddComponent<GenericIntroDoer>();
				fuckyouprefab.AddComponent<BulletBankIntro>();
				miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
				miniBossIntroDoer.initialDelay = 0.15f;
				miniBossIntroDoer.cameraMoveSpeed = 14;
				miniBossIntroDoer.specifyIntroAiAnimator = null;
				miniBossIntroDoer.BossMusicEvent = "Play_MUS_Lich_Double_01";
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
				PlanetsideModule.Strings.Enemies.Set("#QUOTE", "");

				miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
				{
					bossNameString = "#BULLETBANKNAME",
					bossSubtitleString = "#BULLETBANKSHDES",
					bossQuoteString = "#QUOTE",
					bossSpritePxOffset = IntVector2.Zero,
					topLeftTextPxOffset = IntVector2.Zero,
					bottomRightTextPxOffset = IntVector2.Zero,
					bgColor = Color.cyan
				};
                var BossCardTexture = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("bulletbanker_bosscard");
                if (BossCardTexture)
                {
                    miniBossIntroDoer.portraitSlideSettings.bossArtSprite = BossCardTexture;
                    miniBossIntroDoer.SkipBossCard = false;
                    companion.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.SubbossBar;
                }
                else
				{
					miniBossIntroDoer.SkipBossCard = true;
					companion.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.SubbossBar;
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
			public override IEnumerator Top()
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
				public override IEnumerator Top()
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
						string guid = BraveUtility.RandomElement<string>(StaticInformation.ModderBulletGUIDs);
						var Enemy = EnemyDatabase.GetOrLoadByGuid(guid);
                        Enemy.healthHaver.SetHealthMaximum(14f);
                        var en = AIActor.Spawn(Enemy.aiActor, this.Projectile.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Default, true);
                        base.BulletBank.aiActor.GetComponent<BankerEnemyBehavior>().spawnBullets.Add(en);

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
				public override IEnumerator Top()
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
			public override IEnumerator Top()
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
				public override IEnumerator Top()
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
						string guid = BraveUtility.RandomElement<string>(StaticInformation.ModderBulletGUIDs);
						var Enemy = EnemyDatabase.GetOrLoadByGuid(guid);
						Enemy.healthHaver.SetHealthMaximum(14f);
						var en = AIActor.Spawn(Enemy.aiActor, this.Projectile.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Default, true);
						base.BulletBank.aiActor.GetComponent<BankerEnemyBehavior>().spawnBullets.Add(en);

						float num = base.RandomAngle();
						for (int i = 0; i < 12; i++)
						{
							base.Fire(new Direction((i * 30)+num, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new BurstBullet());
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
				public override IEnumerator Top()
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
			public override IEnumerator Top()
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("snakeBullet"));
				}
				float Aim = base.AimDirection;
				base.PostWwiseEvent("Play_ENM_bigshroom_roar_01", null);

				for (int i = 0; i < 30; i++)
				{
					base.Fire(new Direction(((i*12f)+180)+Aim, DirectionType.Absolute, -1f), new Speed(9, SpeedType.Absolute), new BigWhips.BasicBullet());
					base.Fire(new Direction(((i*12f)+Aim), DirectionType.Absolute, -1f), new Speed(9, SpeedType.Absolute), new BigWhips.BasicBullet());
					yield return this.Wait(4f);

				}
				yield break;
			}
			public class BasicBullet : Bullet
			{
				public BasicBullet() : base("snakeBullet", false, false, false)
				{
				}
                public override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 120);
                    base.ChangeDirection(new Brave.BulletScript.Direction(120, DirectionType.Relative), 120);
                    yield return base.Wait(120);
                    yield break;
                }
            }
		}

        public class HealScript : Script
		{
			public override IEnumerator Top()
			{
				List<AIActor> l = base.BulletBank.aiActor.GetComponent<BankerEnemyBehavior>().spawnBullets;
				for (int i = 0; i < l.Count; i++)
				{
					if (l[i] == null)
					{
						l.Remove(l[i]);
					}
				}
                if (l.Count == 0 | l == null) { yield break; }
                for (int e = 0; e < l.Count; e++)
				{
                    AIActor enemy = l[e];
					if (enemy != null)
					{
						GameManager.Instance.StartCoroutine(DisplayHealTether(enemy));
					}
                }
                for (int i = 0; i < 180; i++)
				{
                    if (l.Count == 0 | l == null) { yield break; }         
					if (i % 10 ==0)
					{
                        base.Fire(new Direction(base.RandomAngle(), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(7, 10), SpeedType.Absolute), new Shrapnel());
                        base.Fire(new Direction(base.RandomAngle(), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(7, 10), SpeedType.Absolute), new Shrapnel());
                    }
					for (int e = 0; e < l.Count; e++)
					{
						AIActor enemy = l[e];
						if (enemy == null)
						{ l.Remove(enemy); }
						else
						{
							enemy.behaviorSpeculator.Stun(2f, true);
							if (i % 30 == 0)
							{
								var ob = SpawnManager.SpawnVFX(StaticVFXStorage.EnemyZappyTellVFX, enemy.sprite.WorldCenter, Quaternion.identity);
								Destroy(ob, 3);
							}
						}
						if (i == 120)
						{
                            StaticVFXStorage.HighPriestClapVFX.SpawnAtPosition(enemy.sprite.WorldCenter);
                        }
                    }
                    yield return this.Wait(1f);
                }
                if (l.Count == 0 | l == null) { yield break; }
                for (int e = 0; e < l.Count; e++)
                {
                    AIActor enemy = l[e];
                    if (enemy != null)
                    {
                        Exploder.DoDistortionWave(base.BulletBank.sprite.WorldCenter, 2f, 0.2f, 50, 1);
                        AkSoundEngine.PostEvent("Play_OBJ_power_up_01", base.BulletBank.aiActor.gameObject);
                        base.PostWwiseEvent("Play_BOSS_Rat_Cheese_Burst_02", null);
                        for (int r = 0; r < 16; r++)
						{
                            base.Fire(Offset.OverridePosition(enemy.transform.PositionVector2()), new Direction(base.RandomAngle(), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(7, 12), SpeedType.Absolute), new Shrapnel());
                        }
                        enemy.healthHaver.ApplyDamage(100000, base.BulletBank.aiActor.transform.PositionVector2(), "Consumed");
                        enemy.healthHaver.Die(base.BulletBank.aiActor.transform.PositionVector2());
                        base.BulletBank.aiActor.healthHaver.ApplyHealing(35f);
                        yield return this.Wait(15f);
                    }
                }
				base.BulletBank.aiActor.GetComponent<BankerEnemyBehavior>().spawnBullets.Clear();
                yield break;
			}

            public class Shrapnel : Bullet
            {
                public Shrapnel() : base("poundSmall", false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(5f, SpeedType.Absolute), 90);
                    yield break;
                }
            }

            private IEnumerator DisplayHealTether(AIActor enemy)
			{
				AIActor banker = base.BulletBank.aiActor;

				while (enemy != null)
				{
                    Vector2 vector = banker.sprite.WorldCenter;
                    Vector2 vector2 = enemy.sprite.WorldCenter;
                    int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector, vector2)), 1);
                    for (int i = 0; i < num2; i++)
                    {
                        if (enemy == null) { yield break; }
                        if (banker == null) { yield break; }

                        float t = (float)i / (float)num2;
                        Vector3 vector3 = Vector3.Lerp(enemy.sprite.WorldCenter, banker.sprite.WorldCenter, t);
                        GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.HealingSparklesVFX, false);
                        gameObject.transform.position = vector3;
                        tk2dSpriteAnimator component2 = gameObject.GetComponent<tk2dSpriteAnimator>();
                        if (component2 != null)
                        {
                            component2.ignoreTimeScale = true;
                            component2.AlwaysIgnoreTimeScale = true;
                            component2.AnimateDuringBossIntros = true;
                            component2.alwaysUpdateOffscreen = true;
                            component2.playAutomatically = true;
                        }
                        Destroy(gameObject, 2);
                        yield return this.Wait(2f);
                    }
                    yield return this.Wait(60f);
                }
                yield break;
            }
        }



        public class BloopScript : Script
		{
			public override IEnumerator Top()
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

				public override IEnumerator Top()
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




		public class BankerEnemyBehavior : BraveBehaviour
		{
			private RoomHandler m_StartRoom;
			public List<AIActor> spawnBullets = new List<AIActor>();


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
			public void LerpMaterialGlow(Material targetMaterial, float startGlow, float targetGlow, float duration)
			{
				base.StartCoroutine(this.LerpMaterialGlowCR(targetMaterial, startGlow, targetGlow, duration));
			}
			private IEnumerator LerpMaterialGlowCR(Material targetMaterial, float startGlow, float targetGlow, float duration)
			{
				float elapsed = 0f;
				while (elapsed < duration)
				{
					elapsed += BraveTime.DeltaTime; ;
					float t = elapsed / duration;
					if (targetMaterial != null)
					{
						targetMaterial.SetFloat("_EmissivePower", Mathf.Lerp(startGlow, targetGlow, t));
					}
					yield return null;
				}
				yield break;
			}
			private void Start()
			{
				if (!base.aiActor.IsBlackPhantom)
                {
					
					if (base.aiActor.sprite && base.aiActor.sprite.renderer)
					{

						Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
						mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
						mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
						mat.SetFloat("_EmissiveColorPower", 1.55f);
						mat.SetFloat("_EmissivePower", 30);
						mat.SetFloat("_EmissiveThresholdSensitivity", 0.6f);

						base.aiActor.sprite.renderer.material = mat;
						/*
						Material sharedMaterial = base.aiActor.sprite.renderer.sharedMaterial;
						base.aiActor.sprite.usesOverrideMaterial = true;
						Material material = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
						material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
						//material.SetColor("_OverrideColor", new Color(1f, 1f, 1f, 1f));
						this.LerpMaterialGlow(material, 0f, 22f, 0.4f);
						material.SetFloat("_EmissiveColorPower", 80f);
						material.SetColor("_EmissiveColor", Color.white);
						base.aiActor.sprite.renderer.material = material;
						*/
					}
					
				}
				else
				{
					TrailRenderer tr = base.aiActor.gameObject.GetOrAddComponent<TrailRenderer>();
					if (tr != null)
					{
						tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
						tr.receiveShadows = false;
						var mat = new Material(Shader.Find("Sprites/Default"));
						mat.mainTexture = _gradTexture;
						mat.SetColor("_Color", new Color(3f, 0f, 0f, 0.9f));
						tr.material = mat;
						tr.time = 1f;
						tr.minVertexDistance = 0.1f;
						tr.startWidth = 1.2f;
						tr.endWidth = 0f;
						tr.startColor = Color.red;
						tr.endColor = Color.black;
					}
				}
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(base.aiActor.healthHaver);
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					AkSoundEngine.PostEvent("Play_ENM_screamer_scream_01", base.gameObject);
				};
				base.healthHaver.healthHaver.OnDeath += (obj) =>
				{
					float itemsToSpawn = UnityEngine.Random.Range(2, 6);
					float spewItemDir = 360 / itemsToSpawn;
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BULLETBANK_DEFEATED, true);//Done
					for (int i = 0; i < itemsToSpawn; i++)
					{
						int id = BraveUtility.RandomElement<int>(Shellrax.Lootdrops);
						LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, base.aiActor.sprite.WorldCenter, new Vector2(spewItemDir * itemsToSpawn, spewItemDir * itemsToSpawn), 2.2f, false, true, false);
					}


					float value = UnityEngine.Random.Range(0.00f, 1.00f);
					if (value <= 0.4f)
					{
						Chest chest2 = GameManager.Instance.RewardManager.SpawnTotallyRandomChest(GameManager.Instance.PrimaryPlayer.CurrentRoom.GetRandomVisibleClearSpot(1, 1));
						chest2.IsLocked = false;
						chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
					}

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
			
			//idle
			"Planetside/Resources/BulletBanker/bulletbanker_idle_001.png",
			"Planetside/Resources/BulletBanker/bulletbanker_idle_002.png",
			"Planetside/Resources/BulletBanker/bulletbanker_idle_003.png",
			"Planetside/Resources/BulletBanker/bulletbanker_idle_004.png",
			"Planetside/Resources/BulletBanker/bulletbanker_idle_005.png",
			"Planetside/Resources/BulletBanker/bulletbanker_idle_006.png",
			"Planetside/Resources/BulletBanker/bulletbanker_idle_007.png",
			"Planetside/Resources/BulletBanker/bulletbanker_idle_008.png",
			"Planetside/Resources/BulletBanker/bulletbanker_idle_009.png",
			"Planetside/Resources/BulletBanker/bulletbanker_idle_010.png",


			//6
			//bloop tell
			"Planetside/Resources/BulletBanker/bulletbanker_blooptell_001.png",
			"Planetside/Resources/BulletBanker/bulletbanker_blooptell_002.png",
			"Planetside/Resources/BulletBanker/bulletbanker_blooptell_003.png",
			"Planetside/Resources/BulletBanker/bulletbanker_blooptell_004.png",
			//bloop
			"Planetside/Resources/BulletBanker/bulletbanker_bloop_001.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bloop_002.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bloop_003.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bloop_004.png",

			//11

			//bottle slam tell
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslamtell_001.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslamtell_002.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslamtell_003.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslamtell_004.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslamtell_005.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslamtell_006.png",
			//bottle slam
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_001.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_002.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_003.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_004.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_005.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_006.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_007.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_008.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_009.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_010.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_011.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_012.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_013.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_014.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_015.png",
			"Planetside/Resources/BulletBanker/bulletbanker_bottleslaml_016.png",

			//roar tell
			"Planetside/Resources/BulletBanker/bulletbanker_roartell_001.png",
			"Planetside/Resources/BulletBanker/bulletbanker_roartell_002.png",
			"Planetside/Resources/BulletBanker/bulletbanker_roartell_003.png",
			"Planetside/Resources/BulletBanker/bulletbanker_roartell_004.png",
			"Planetside/Resources/BulletBanker/bulletbanker_roartell_005.png",
			"Planetside/Resources/BulletBanker/bulletbanker_roartell_006.png",
			"Planetside/Resources/BulletBanker/bulletbanker_roartell_007.png",

			//roar
			"Planetside/Resources/BulletBanker/bulletbanker_roar_001.png",
			"Planetside/Resources/BulletBanker/bulletbanker_roar_002.png",
			"Planetside/Resources/BulletBanker/bulletbanker_roar_003.png",
			"Planetside/Resources/BulletBanker/bulletbanker_roar_004.png",
			"Planetside/Resources/BulletBanker/bulletbanker_roar_005.png",
			"Planetside/Resources/BulletBanker/bulletbanker_roar_006.png",


			//intro /
			"Planetside/Resources/BulletBanker/bulletbanker_introl_001.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_002.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_003.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_004.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_005.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_006.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_007.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_008.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_009.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_010.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_011.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_012.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_013.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_014.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_015.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_016.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_017.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_018.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_019.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_020.png",
			"Planetside/Resources/BulletBanker/bulletbanker_introl_021.png",


			//66

			//die /
			"Planetside/Resources/BulletBanker/bulletbanker_death_001.png",
			"Planetside/Resources/BulletBanker/bulletbanker_death_002.png",
			"Planetside/Resources/BulletBanker/bulletbanker_death_003.png",
			"Planetside/Resources/BulletBanker/bulletbanker_death_004.png",
			"Planetside/Resources/BulletBanker/bulletbanker_death_005.png",
			"Planetside/Resources/BulletBanker/bulletbanker_death_006.png",
			"Planetside/Resources/BulletBanker/bulletbanker_death_007.png",
			"Planetside/Resources/BulletBanker/bulletbanker_death_008.png",
			"Planetside/Resources/BulletBanker/bulletbanker_death_009.png",
			"Planetside/Resources/BulletBanker/bulletbanker_death_010.png",
			"Planetside/Resources/BulletBanker/bulletbanker_death_011.png",


			//85


		};
	}

}








