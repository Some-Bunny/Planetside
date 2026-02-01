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

namespace Planetside
{
	public class Wailer : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "wailer";
		public static GameObject shootpointwail;
		public static AIBeamShooter AIBeamShooter;

		public static void Init()
		{
			Wailer.BuildPrefab();
		}

		public static void BuildPrefab()
		{


            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("WailerCollection").GetComponent<tk2dSpriteCollectionData>();
            Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("wailer material");

			if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
			{
				AIActor aIActor = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
				prefab = EnemyBuilder.BuildPrefabBundle("wailer", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(8, 9), true);
				var companion = prefab.AddComponent<EnemyBehavior>();

                EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, Collection, mat);

                //prefab.AddComponent<AIBeamShooter>(); ;
                companion.aiActor.knockbackDoer.weight = 800;
				companion.aiActor.MovementSpeed = 2f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = false;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(30f);
				companion.aiActor.CollisionKnockbackStrength = 5f;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.healthHaver.SetHealthMaximum(30f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();

                EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.defaultShadow, new Vector2(0.5f, -0.25f), "shadowPos");

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
					ManualHeight = 17,
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
					ManualHeight = 17,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;
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
				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.FourWay,
					Flipped = new DirectionalAnimation.FlipType[4],
					AnimNames = new string[]
					{
						"idle_back_left",
						"idle_front_right",
						"idle_front_left",
						"idle_back_right",

					}
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.FourWay,
					Flipped = new DirectionalAnimation.FlipType[4],
					AnimNames = new string[]
						{
						"run_back_left",
						"run_front_right",
						"run_front_left",
						"run_back_right",

					}
				};
				DirectionalAnimation aaa = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					AnimNames = new string[]
					{
						"wail",
					},
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "wail",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{

						   "wail",
						   "wail"

							}

						}
					}
				};
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);
				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;


				//bool flag3 = WailerCollection == null;
				//if (flag3)
				{
					/*
					WailerCollection = SpriteBuilder.ConstructCollection(prefab, "Wailer_Collection");
					UnityEngine.Object.DontDestroyOnLoad(WailerCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], WailerCollection);
					}
					*/
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					2,
					3

					}, "idle_front_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 3f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					4,
					5


					}, "idle_front_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1

					}, "idle_back_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 3f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1


					}, "idle_back_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					6,
					7,
					8,
					9,
					10,
					11,

					}, "run_back_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					12,
					13,
					14,
					15,
					16,
					17
					}, "run_front_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					18,
					19,
					20,
					21,
					22,
					23


					}, "run_front_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					24,
					25,
					26,
					27,
					28,
					29


					}, "run_back_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

				 30,
				 31,
				 32,
				 33,
				 34,



					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

				35,
				36,
				37,
				38,
				39

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

				40,
				41,
				42,
				43,
				44,
				45,
				46,
				47,
				48,
				49,
				50,
				51,
				52,
				53,
				54,
				55,
				56


					}, "wail", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
				SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
				{

				
				52,
				53,
				54,
				55,
				56


				}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
				}

				var bs = prefab.GetComponent<BehaviorSpeculator>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				bs.AttackBehaviors = behaviorSpeculator.AttackBehaviors;
				bs.TargetBehaviors = behaviorSpeculator.TargetBehaviors;
				shootpointwail = new GameObject("fuck");
				shootpointwail.transform.parent = companion.transform;
				shootpointwail.transform.position = companion.sprite.WorldCenter;
				GameObject m_CachedGunAttachPoint1 = companion.transform.Find("fuck").gameObject;
				//cunt.beamTransform = companion.transform;
				//cunt.firingEllipseCenter = companion.sprite.WorldCenter;
				/*
				foreach (AttackBehaviorBase att in EnemyDatabase.GetOrLoadByGuid("4db03291a12144d69fe940d5a01de376").behaviorSpeculator.AttackBehaviors)
				{
					if (att is TeleportBehavior)
					{
						bs.behaviorSpeculator.AttackBehaviors.Add(att);
					}
				}
				*/
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

			bs.MovementBehaviors = new List<MovementBehaviorBase>
			{
				new SeekTargetBehavior
				{
					ExternalCooldownSource = true,
					StopWhenInRange = true,
					CustomRange = 7f,
					LineOfSight = false,
					ReturnToSpawn = false,
					SpawnTetherDistance = 0f,
					PathInterval = 0.5f,
					SpecifyRange = false,
					MinActiveRange = 0f,
					MaxActiveRange = 0f


				}
			};
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
					
					/*new TeleportBehavior()
				{
						
					AttackableDuringAnimation = true,
					AllowCrossRoomTeleportation = false,
					teleportRequiresTransparency = false,
					hasOutlinesDuringAnim = true,
					ManuallyDefineRoom = false,
					MaxHealthThreshold = 1f,
					StayOnScreen = true,
					AvoidWalls = true,
					GoneTime = 1f,
					OnlyTeleportIfPlayerUnreachable = false,
					MinDistanceFromPlayer = 4f,
					MaxDistanceFromPlayer = -1f,
					//teleportInAnim = "wail",
					//teleportOutAnim = "wail",
					AttackCooldown = 1f,
					InitialCooldown = 0f,
					RequiresLineOfSight = false,
					roomMax = new Vector2(0,0),
					roomMin = new Vector2(0,0),
					//teleportInBulletScript = new CustomBulletScriptSelector(typeof(Wail)),
					//teleportOutBulletScript = new CustomBulletScriptSelector(typeof(Wail)),
					GlobalCooldown = 0.5f,
					Cooldown = 4f,
					CooldownVariance = 1f,
					InitialCooldownVariance = 0f,
					goneAttackBehavior = null,
					IsBlackPhantom = false,
					GroupName = null,
					GroupCooldown = 2f,
					MinRange = 0,
					Range = 0,
					MinHealthThreshold = 0,
					MaxEnemiesInRoom = 1,
					MaxUsages = 0,
					AccumulateHealthThresholds = true,
					//shadowInAnim = null,
					//shadowOutAnim = null,
					targetAreaStyle = null,
					HealthThresholds = new float[0],
					MinWallDistance = 0,
					//resetCooldownOnDamage = null,
					//shadowSupport = (TeleportBehavior.ShadowSupport)1,

				},*/
					
				/*
				new ShootGunBehavior() {
					GroupCooldownVariance = 0.2f,
					LineOfSight = false,
					WeaponType = WeaponType.BulletScript,
					OverrideBulletName = null,
					BulletScript = new CustomBulletScriptSelector(typeof(SkellScript)),
					FixTargetDuringAttack = true,
					StopDuringAttack = true,
					LeadAmount = 0,
					LeadChance = 1,
					RespectReload = true,
					MagazineCapacity = 4,
					ReloadSpeed = 3f,
					EmptiesClip = true,
					SuppressReloadAnim = false,
					TimeBetweenShots = -1,
					PreventTargetSwitching = true,
					OverrideAnimation = null,
					OverrideDirectionalAnimation = null,
					HideGun = false,
					UseLaserSight = false,
					UseGreenLaser = false,
					PreFireLaserTime = -1,
					AimAtFacingDirectionWhenSafe = false,
					Cooldown = 0.4f,
					CooldownVariance = 0,
					AttackCooldown = 0,
					GlobalCooldown = 0,
					InitialCooldown = 0,
					InitialCooldownVariance = 0,
					GroupName = null,
					GroupCooldown = 0,
					MinRange = 0,
					Range = 30,
					MinWallDistance = 0,
					MaxEnemiesInRoom = 0,
					MinHealthThreshold = 0,
					MaxHealthThreshold = 1,
					HealthThresholds = new float[0],
					AccumulateHealthThresholds = true,
					targetAreaStyle = null,
					IsBlackPhantom = false,
					resetCooldownOnDamage = null,
					RequiresLineOfSight = true,
					MaxUsages = 0,

				},
				*/
				new ShootBehavior()
				{
					ShootPoint = m_CachedGunAttachPoint1,
					BulletScript = new CustomBulletScriptSelector(typeof(Wail)),
					LeadAmount = 0f,
					AttackCooldown = 3.5f,
					InitialCooldown = 0.8f,
					//TellAnimation = "wail",
					FireAnimation = "wail",
					RequiresLineOfSight = true,
					StopDuring = ShootBehavior.StopType.Attack,
					Uninterruptible = true,
					//FireVfx = ,
					//ChargeVfx = ,
					MoveSpeedModifier = 0f,
					
					//GlobalCooldown = 0.5f,
				}/*,
				new ShootBeamBehavior()
				{
					//specificBeamShooter = m_CachedGunAttachPoint1,
					//BulletScript = new CustomBulletScriptSelector(typeof(Wail)),
					//LeadAmount = 0f,
					//AttackCooldown = 5f,
					//TellAnimation = "wail",

					//StopDuring = ShootBehavior.StopType.Attack,
					//Uninterruptible = true,
					//FireVfx = ,
					//ChargeVfx = ,
					//MoveSpeedModifier = 0f,
					//GlobalCooldown = 0.5f,
					beamSelection = ShootBeamBehavior.BeamSelection.All,
					//specificBeamShooter = m_CachedGunAttachPoint1,
					FireAnimation = "wail",
					RequiresLineOfSight = true,
					InitialCooldown = 0f,
					initialAimType = ShootBeamBehavior.InitialAimType.Aim,
					beamLengthOFfset = 1,
					trackingType = ShootBeamBehavior.TrackingType.Follow,
					beamLengthSinPeriod = 1,
					maxUnitTurnRate = 8,
					degreeCatchUpSpeed = 45,
					useUnitCatchUp = true,
					minUnitForCatchUp = 2,
					maxUnitForCatchUp = 10,
					unitCatchUpSpeed = 8,
					useUnitOvershoot = true,
					minUnitForOvershoot = 0.1f,
					unitOvershootTime = 0.7f,
					unitOvershootSpeed = 10,
					maxDegTurnRate = 0,
					degTurnRateAcceleration = 0,
					Cooldown = 2,
					GroupName = "beam",
					GroupCooldown = 3,
					IsBlackPhantom = false,

				}
				*/

			};
				/*
				foreach (AttackBehaviorBase att in EnemyDatabase.GetOrLoadByGuid("4db03291a12144d69fe940d5a01de376").behaviorSpeculator.AttackBehaviors)
				{
					if (att is TeleportBehavior)
					{
						bs.behaviorSpeculator.AttackBehaviors.Add(att);
					}
				}
				*/



				//BehaviorSpeculator load = EnemyDatabase.GetOrLoadByGuid("206405acad4d4c33aac6717d184dc8d4").behaviorSpeculator;
				//Tools.DebugInformation(load);

				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				bs.GlobalCooldown = behaviorSpeculator.GlobalCooldown;

				GameObject hand = companion.transform.Find("GunAttachPoint").gameObject;
				Destroy(hand);

				Game.Enemies.Add("psog:wailer", companion.aiActor);

				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Wailer/warped_scream_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:wailer";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Wailer/warped_scream_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("wailericonammo");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\wailericonammo.png");
                PlanetsideModule.Strings.Enemies.Set("#THE_WAILER", "Wailer");
				PlanetsideModule.Strings.Enemies.Set("#THE_WAILER_SHORTDESC", "Scream The Pain Away");
				PlanetsideModule.Strings.Enemies.Set("#THE_WAILER_LONGDESC", "A writhing, agonized mess of a bullet kin that managed to escape the pits of Bullet Hell. The pain it feels is so unbearable it manifists physically as bullets.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#THE_WAILER";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#THE_WAILER_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#THE_WAILER_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:wailer");
				EnemyDatabase.GetEntry("psog:wailer").ForcedPositionInAmmonomicon = 9;
				EnemyDatabase.GetEntry("psog:wailer").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:wailer").isNormalEnemy = true;
                companion.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));

            }
        }

		public class zappies : Script
		{
			public override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet("default"));
				float direction = BraveMathCollege.QuantizeFloat(base.AimDirection, 45f);

				base.Fire( new zappies.LightningBullet(direction, -1f, 30, -4, null));
				base.Fire( new zappies.LightningBullet(direction, -1f, 30, 4, null));
				if (BraveUtility.RandomBool())
				{
					base.Fire( new zappies.LightningBullet(direction, -1f, 30, 4, null));
				}
				else
				{
					base.Fire(new zappies.LightningBullet(direction, 1f, 30, 4, null));
				}
				base.Fire( new zappies.LightningBullet(direction, 1f, 30, 4, null));
				base.Fire( new zappies.LightningBullet(direction, 1f, 30, -4, null));
				return null;
			}

			public const float Dist = 0.8f;

			public const int MaxBulletDepth = 30;

			public const float RandomOffset = 0.3f;

			public const float TurnChance = 0.2f;

			public const float TurnAngle = 30f;

			private class LightningBullet : Bullet
			{
				public LightningBullet(float direction, float sign, int maxRemainingBullets, int timeSinceLastTurn, Vector2? truePosition = null) : base("default", false, false, false)
				{
					this.m_direction = direction;
					this.m_sign = sign;
					this.m_maxRemainingBullets = maxRemainingBullets;
					this.m_timeSinceLastTurn = timeSinceLastTurn;
					this.m_truePosition = truePosition;
				}

				public override IEnumerator Top()
				{
					yield return base.Wait(2);
					Vector2? truePosition = this.m_truePosition;
					if (truePosition == null)
					{
						this.m_truePosition = new Vector2?(base.Position);
					}
					if (this.m_maxRemainingBullets > 0)
					{
						if (this.m_timeSinceLastTurn > 0 && this.m_timeSinceLastTurn != 2 && this.m_timeSinceLastTurn != 3 && UnityEngine.Random.value < 0.2f)
						{
							this.m_sign *= -1f;
							this.m_timeSinceLastTurn = 0;
						}
						float num = this.m_direction + this.m_sign * 30f;
						Vector2 vector = this.m_truePosition.Value + BraveMathCollege.DegreesToVector(num, 0.8f);
						Vector2 vector2 = vector + BraveMathCollege.DegreesToVector(num + 90f, UnityEngine.Random.Range(-0.3f, 0.3f));
						if (!base.IsPointInTile(vector2))
						{
							zappies.LightningBullet lightningBullet = new zappies.LightningBullet(this.m_direction, this.m_sign, this.m_maxRemainingBullets - 1, this.m_timeSinceLastTurn + 1, new Vector2?(vector));
							base.Fire(Offset.OverridePosition(vector2), lightningBullet);
							if (lightningBullet.Projectile && lightningBullet.Projectile.specRigidbody && PhysicsEngine.Instance.OverlapCast(lightningBullet.Projectile.specRigidbody, null, true, false, null, null, false, null, null, new SpeculativeRigidbody[0]))
							{
								lightningBullet.Projectile.DieInAir(false, true, true, false);
							}
						}
					}
					yield return base.Wait(30);
					base.Vanish(true);
					yield break;
				}

				private float m_direction;

				private float m_sign;

				private int m_maxRemainingBullets;

				private int m_timeSinceLastTurn;

				private Vector2? m_truePosition;
			}
		}


		public class SkellScript : Script 
		{
			public override IEnumerator Top() 
			{
				AkSoundEngine.PostEvent("Play_WPN_eyeballgun_shot_01", this.BulletBank.aiActor.gameObject);
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("044a9f39712f456597b9762893fbc19c").bulletBank.GetBullet("gross"));
				}
				for (int i = 0; i < 1; i++)
				{
					base.Fire(new Direction((UnityEngine.Random.Range(15, -15)), DirectionType.Aim, -1f), new Speed(9f, SpeedType.Absolute), new GrubBullet());
				}
				yield break;
			}

		}
		public class GrubBullet : Bullet
		{
			public GrubBullet() : base("gross", false, false, false)
			{
				//base.SuppressVfx = true;

			}

			public override IEnumerator Top()
			{
				base.ManualControl = true;
				Vector2 truePosition = base.Position;
				float startVal = UnityEngine.Random.value;
				for (int i = 0; i < 360; i++)
				{
					float offsetMagnitude = Mathf.SmoothStep(-0.75f, 0.75f, Mathf.PingPong(startVal + (float)i / 60f * 3f, 1f));
					truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
					base.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude);
					yield return base.Wait(1);
				}
				base.Vanish(false);
				yield break;
			}
		}



		public class Shoot : Script
		{
			public override IEnumerator Top()
			{


				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("383175a55879441d90933b5c4e60cf6f").bulletBank.GetBullet("bigBullet"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet("default"));
				//Adds the Bullet type into the scripts bullet bank. imprtant to have because if you dont have the bullet it will break the script

				//For ints used to create multiple bullets, but instead of starting at 0, i start at -1 to make the script fire from the correct angles much easier
				for (int k = -1; k < 2; k++)
				{
					//Direction adds an additional angle to where-ever it initially fires
					//DirectionType is used to control where the bullet will aim at, and *tehn* add the angle given by direction, so this script will fire bullets towards the player with an anglular offset of -30, 0  and 30
					base.Fire(new Direction(30 * k, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(4f, SpeedType.Absolute), new Shoot.BigBullet());
					//The "new Ballin.TootthBullet());" lets you choose what bullet behavior oyu want to use for more advanced stuff by adding additional code to the new Bullet you created, such as making them last a certain amount of time by a given value, or by making them undodgeable
				}
				//Same principle applies as above, only with a check to make it not fire the bullet at the 10 * 0 angle, also the bullet type this party uses is of the UndodgeableBullert set up below
				for (int k = -2; k < 3; k++)
				{
					if (k != 0)
					{
						base.Fire(new Direction(10 * k, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(4f, SpeedType.Absolute), new Shoot.UndodgeableBullert());
					}
				}
				yield break;
			}
			public class BigBullet : Bullet
			{
				//The string with the bullert names are imprtant, if you dont have the bullet you need, it will break
				public BigBullet() : base("bigBullet", false, false, false)
				{

				}
				public override IEnumerator Top()
				{
					//You can leave this empty as this bullet has no special properties needed
					yield break;
				}
			}
			public class UndodgeableBullert : Bullet
			{
				public UndodgeableBullert() : base("default", false, false, false)
				{

				}
				public override IEnumerator Top()
				{
					if (this.Projectile.gameObject.GetComponent<Projectile>() != null)
					{
						OtherTools.CopyFields<ThirdDimensionalProjectile>(this.Projectile.gameObject.GetComponent<Projectile>());
					}
					else
					{
						ETGModConsole.Log("HOW");
					}
					yield break;
				}
			}
		}
		public class SkellBullet : Bullet
		{
			public SkellBullet() : base("sweep", false, false, false)
			{

			}
		}
		public class Wail : Script
		{
			public override IEnumerator Top()
			{
				AkSoundEngine.PostEvent("Play_ENM_screamer_scream_01", this.BulletBank.aiActor.gameObject);
				for (int k = 0; k < 24; k++)
				{
					yield return this.Wait(4f);
					this.Fire(new Direction(UnityEngine.Random.Range(0f, 360f), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(3f, 6f), SpeedType.Absolute), new ReverseBullet());
				}
				yield break;
			}
		}
		public class ReverseBullet : Bullet
		{
			public ReverseBullet() : base("reversible", false, false, false)
			{
				base.SuppressVfx = true;
			}

			public override IEnumerator Top()
			{
				float speed = this.Speed;
				yield return this.Wait(10);
				this.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 40);
				yield return this.Wait((UnityEngine.Random.Range(50, 120)));
				this.Direction += 180f;
				this.Projectile.spriteAnimator.Play();
				yield return this.Wait((UnityEngine.Random.Range(20, 50)));
				this.ChangeSpeed(new Speed(Mathf.Max(speed * 4, 16), SpeedType.Absolute), (UnityEngine.Random.Range(30, 75)));
				yield return this.Wait(130);
				//this.Vanish(true);
				yield break;
			}
		}

		private static string[] spritePaths = new string[]
		{
			
			//idles
			"Planetside/Resources/Wailer/bullet_idle_back_001.png",
			"Planetside/Resources/Wailer/bullet_idle_back_002.png",

			"Planetside/Resources/Wailer/bullet_idle_left_001.png",
			"Planetside/Resources/Wailer/bullet_idle_left_002.png",

			"Planetside/Resources/Wailer/bullet_idle_right_001.png",
			"Planetside/Resources/Wailer/bullet_idle_right_002.png",


			//run
			"Planetside/Resources/Wailer/bullet_run_left_back_001.png",
			"Planetside/Resources/Wailer/bullet_run_left_back_002.png",
			"Planetside/Resources/Wailer/bullet_run_left_back_003.png",
			"Planetside/Resources/Wailer/bullet_run_left_back_004.png",
			"Planetside/Resources/Wailer/bullet_run_left_back_005.png",
			"Planetside/Resources/Wailer/bullet_run_left_back_006.png",

			"Planetside/Resources/Wailer/bullet_run_left_001.png",
			"Planetside/Resources/Wailer/bullet_run_left_002.png",
			"Planetside/Resources/Wailer/bullet_run_left_003.png",
			"Planetside/Resources/Wailer/bullet_run_left_004.png",
			"Planetside/Resources/Wailer/bullet_run_left_005.png",
			"Planetside/Resources/Wailer/bullet_run_left_006.png",
			
			"Planetside/Resources/Wailer/bullet_run_right_001.png",
			"Planetside/Resources/Wailer/bullet_run_right_002.png",
			"Planetside/Resources/Wailer/bullet_run_right_003.png",
			"Planetside/Resources/Wailer/bullet_run_right_004.png",
			"Planetside/Resources/Wailer/bullet_run_right_005.png",
			"Planetside/Resources/Wailer/bullet_run_right_006.png",

			"Planetside/Resources/Wailer/bullet_run_right_back_001.png",
			"Planetside/Resources/Wailer/bullet_run_right_back_002.png",
			"Planetside/Resources/Wailer/bullet_run_right_back_003.png",
			"Planetside/Resources/Wailer/bullet_run_right_back_004.png",
			"Planetside/Resources/Wailer/bullet_run_right_back_005.png",
			"Planetside/Resources/Wailer/bullet_run_right_back_006.png",

			//death
			"Planetside/Resources/Wailer/bullet_death_right_front_001.png",
			"Planetside/Resources/Wailer/bullet_death_right_front_002.png",
			"Planetside/Resources/Wailer/bullet_death_right_front_003.png",
			"Planetside/Resources/Wailer/bullet_death_right_front_004.png",
			"Planetside/Resources/Wailer/bullet_death_right_front_005.png",

			"Planetside/Resources/Wailer/bullet_death_left_front_001.png",
			"Planetside/Resources/Wailer/bullet_death_left_front_002.png",
			"Planetside/Resources/Wailer/bullet_death_left_front_003.png",
			"Planetside/Resources/Wailer/bullet_death_left_front_004.png",
			"Planetside/Resources/Wailer/bullet_death_left_front_005.png",

			"Planetside/Resources/Wailer/warped_scream_001.png",
			"Planetside/Resources/Wailer/warped_scream_002.png",
			"Planetside/Resources/Wailer/warped_scream_003.png",
			"Planetside/Resources/Wailer/warped_scream_004.png",
			"Planetside/Resources/Wailer/warped_scream_005.png",
			"Planetside/Resources/Wailer/warped_scream_006.png",
			"Planetside/Resources/Wailer/warped_scream_007.png",
			"Planetside/Resources/Wailer/warped_scream_008.png",
			"Planetside/Resources/Wailer/warped_scream_009.png",
			"Planetside/Resources/Wailer/warped_scream_010.png",
			"Planetside/Resources/Wailer/warped_scream_011.png",
			"Planetside/Resources/Wailer/warped_scream_012.png",
			"Planetside/Resources/Wailer/warped_scream_013.png",
			"Planetside/Resources/Wailer/warped_scream_014.png",
			"Planetside/Resources/Wailer/warped_scream_015.png",
			"Planetside/Resources/Wailer/warped_scream_016.png",
			"Planetside/Resources/Wailer/warped_scream_017.png",

		};
		//private float m_globalCooldownTimer = 0;
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
				if (!base.aiActor.IsBlackPhantom)
                {
					Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
					mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
					mat.SetColor("_EmissiveColor", new Color32(253, 130, 255, 255));
					mat.SetFloat("_EmissiveColorPower", 1.55f);
					mat.SetFloat("_EmissivePower", 100);
					aiActor.sprite.renderer.material = mat;
					m_StartRoom = aiActor.GetAbsoluteParentRoom();
					//base.aiActor.HasBeenEngaged = true;
					base.aiActor.healthHaver.OnPreDeath += (obj) =>
					{
						AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Spit_01", base.aiActor.gameObject);
					};
				}
			}
		}
	}
}








