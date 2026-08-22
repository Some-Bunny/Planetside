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


		public static void Init()
		{

            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("CoalletCollection").GetComponent<tk2dSpriteCollectionData>();
            Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("coallet material");
            var h = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("CoalletAnimation").GetComponent<tk2dSpriteAnimation>();

            if (prefab == null ||!EnemyBuilder.Dictionary.ContainsKey(guid))
			{
				prefab = EnemyBuilder.BuildPrefabBundle("Coallet", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(0, 0),false, true);;
				var enemy = prefab.GetComponent<AIActor>();

                EnemyToolbox.QuickAssetBundleSpriteSetup(enemy.aiActor, Collection, mat, false);


                enemy.gameObject.layer = 22;
                enemy.sprite.SortingOrder = 2;


                enemy.aiActor.spriteAnimator.Library = h;
                enemy.aiActor.spriteAnimator.library = h;
                enemy.aiActor.aiAnimator.spriteAnimator = enemy.aiActor.spriteAnimator;

                CoalletController pain = prefab.AddComponent<CoalletController>();
				enemy.aiActor.knockbackDoer.weight = 35;
				enemy.aiActor.MovementSpeed = 2f;
				enemy.aiActor.healthHaver.PreventAllDamage = false;
				enemy.aiActor.CollisionDamage = 1f;
				enemy.aiActor.HasShadow = true;
                EnemyToolbox.AddShadowToAIActor(enemy.aiActor, StaticEnemyShadows.defaultShadow, new Vector2(0.375f, 0f), "shadowPos");

                enemy.aiActor.IgnoreForRoomClear = false;
				enemy.aiActor.aiAnimator.HitReactChance = 0f;
				enemy.aiActor.specRigidbody.CollideWithOthers = true;
				enemy.aiActor.specRigidbody.CollideWithTileMap = true;
				enemy.aiActor.PreventFallingInPitsEver = false;
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
							Type = DirectionalAnimation.DirectionType.None,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{
								"die"
							}
						}
					}
				};
                aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
                {
                    new AIAnimator.NamedDirectionalAnimation
                    {
						name = "pitfall",
						anim = new DirectionalAnimation
						{
                            Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
                            Flipped = new DirectionalAnimation.FlipType[2],
                            AnimNames = new string[]
                            {

                                "pitfall",
								"pitfall"
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
                        "idle",
						"idle"
					}
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
					{
                        "run",
						"run"
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
                                "runfire",
								"runfire",
							}
						}
					}
				};
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);
				enemy.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				

				pain.overrideMoveSpeed = 4f;
				pain.overridePauseTime = 1f;


				enemy.aiActor.specRigidbody.PixelColliders.Clear();
				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
                    ManualOffsetX = -1,
                    ManualOffsetY = -1,
                    ManualWidth = 15,
                    ManualHeight = 16,
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
					ManualOffsetX = -1,
					ManualOffsetY = -1,
					ManualWidth = 15,
					ManualHeight = 16,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});

				enemy.aiActor.PreventBlackPhantom = false;

				//AIAnimator aiAnimator = enemy.aiAnimator;
				var shootpoint = new GameObject("fuck");
				shootpoint.transform.parent = enemy.transform;
				shootpoint.transform.position = enemy.sprite.WorldCenter;

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
							ShootPoint = shootpoint,
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

                SpriteBuilder.AddSpriteToCollection(Collection.GetSpriteDefinition("coallet_idle_001"), SpriteBuilder.ammonomiconCollection);
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
				enemy.encounterTrackable.journalData.AmmonomiconSprite = "coallet_idle_001";
				enemy.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("coalleticon");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\coalleticon.png");
                PlanetsideModule.Strings.Enemies.Set("#COALLET", "Coallet");
				PlanetsideModule.Strings.Enemies.Set("#COALLET_SHORTDESC", "Just For Me");
				PlanetsideModule.Strings.Enemies.Set("#COALLET_LONGDESC", "A tiny coaler from the freshly excavated veins of the Black Powder Mines. It spends its time sleeping, but can erupt into flame just like its larger cousins.");
				enemy.encounterTrackable.journalData.PrimaryDisplayName = "#COALLET";
				enemy.encounterTrackable.journalData.NotificationPanelDescription = "#COALLET_SHORTDESC";
				enemy.encounterTrackable.journalData.AmmonomiconFullEntry = "#COALLET_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(enemy.gameObject, "psog:coallet");
				EnemyDatabase.GetEntry("psog:coallet").ForcedPositionInAmmonomicon = 70;
				EnemyDatabase.GetEntry("psog:coallet").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:coallet").isNormalEnemy = true;

                enemy.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				enemy.aiActor.bulletBank.Bullets.Add(StaticBulletEntries.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("homingRing"), "coalletSmall", null, null, false));

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








		public class BurnScript : Script
		{
			public override IEnumerator Top()
			{
				int i = 1;

				for (; ; )
				{
					bool _ = UnityEngine.Random.value < 0.33f;
					float s = _ ? UnityEngine.Random.Range(6f, 9f) : UnityEngine.Random.Range(2f, 3f);
                    base.PostWwiseEvent("Play_BOSS_doormimic_flame_01", null);
					base.Fire(new Direction(UnityEngine.Random.Range(-180, 180)), new Speed(s, SpeedType.Absolute), new Flames(_? "frogger" : "coalletSmall"));
					yield return this.Wait(i);
					i = Mathf.Min(i + 1, 30);
				}
			}
		}
		public class Flames : Bullet
		{
			public Flames(string t) : base(t, false, false, false)
			{

			}

			public override IEnumerator Top()
			{
				float speed = base.Speed;
				base.ChangeSpeed(new Speed(speed * 0.5f, SpeedType.Absolute), 30);
				yield break;
			}
		}
	}
}

