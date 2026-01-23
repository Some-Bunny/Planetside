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
	public class Detscavator : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "detscavator";
		//private static tk2dSpriteCollectionData DescavatorCollection;
		public static GameObject shootpoint;
		public static GameObject shootpoin1;

		public static void Init()
		{
			Detscavator.BuildPrefab();
		}

		public static void BuildPrefab()
		{

            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("DetscavatorCollection").GetComponent<tk2dSpriteCollectionData>();
            Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("detscavator material");

            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
			{
				prefab = EnemyBuilder.BuildPrefabBundle("Detscavator", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();

                EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, Collection, mat);

                Alexandria.ItemAPI.AlexandriaTags.SetTag(companion.aiActor, "robotic_mechanical");

                companion.aiActor.knockbackDoer.weight = 800;
				companion.aiActor.MovementSpeed = 0f;
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
				companion.aiActor.healthHaver.SetHealthMaximum(40f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("4d37ce3d666b4ddda8039929225b7ede");
				ExplodeOnDeath explodeOnDeath = companion.gameObject.AddComponent<ExplodeOnDeath>();
				ExplosionData explosionData = orLoadByGuid.GetComponent<ExplodeOnDeath>().explosionData;
				explodeOnDeath.explosionData = explosionData;

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
					ManualWidth = 28,
					ManualHeight = 30,
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
					ManualWidth = 28,
					ManualHeight = 30,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});
				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").CorpseObject;
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

				DirectionalAnimation charge = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					AnimNames = new string[]
					{
						"charge",

					},
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "charge",
						anim = charge
					}
				};


				DirectionalAnimation killme = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"shootlaser_right",
						"shootlaser_left",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "shootlaser",
						anim = killme
					}
				};


				DirectionalAnimation aaaaaaaaaaa = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"finishlaser_right",
						"finishlaser_left",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "finishlaser",
						anim = aaaaaaaaaaa
					}
				};
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);
				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				//bool flag3 = DescavatorCollection == null;
				//if (flag3)
				{
					/*
					DescavatorCollection = SpriteBuilder.ConstructCollection(prefab, "Detscavator_Collection");
					UnityEngine.Object.DontDestroyOnLoad(DescavatorCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], DescavatorCollection);
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
					7

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7


					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7


					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7


					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				 25
					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				 25
					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				 8,
				 9,
				 10,
				 11,
				 12,
				 13,
				 14,
				 15,
				 16,
				 17,
				 18
					}, "charge", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				19,
				19,
				19,
					}, "shootlaser_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				19,
				19,
				19,
					}, "shootlaser_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				20,
				21,
				22,
				23,
				24
					}, "finishlaser_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				20,
				21,
				22,
				23,
				24
					}, "finishlaser_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
				SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
				{
				20,
				21,
				22,
				23,
				24
				}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 12f;
				}
				
				EnemyToolbox.AddSoundsToAnimationFrame(companion.aiActor.GetComponent<tk2dSpriteAnimator>(), "charge", new Dictionary<int, string> { {3, "Play_ENM_hammer_target_01" }, { 5, "Play_ENM_hammer_target_01" } , { 7, "Play_ENM_hammer_target_01" } });


				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				
				shootpoint = new GameObject("fuck");
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = companion.sprite.WorldCenter + new Vector2(0, 0.1875f);
				GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;


				AIActor actor = EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b");

				AIBeamShooter2 bholsterbeam1 = companion.gameObject.AddComponent<AIBeamShooter2>();
				BeholsterController beholsterbeam = actor.GetComponent<BeholsterController>();
				bholsterbeam1.beamTransform = m_CachedGunAttachPoint.transform;
				bholsterbeam1.beamModule = beholsterbeam.beamModule;
				bholsterbeam1.beamProjectile = beholsterbeam.projectile;
				bholsterbeam1.firingEllipseCenter = m_CachedGunAttachPoint.transform.position;
				bholsterbeam1.name = "Detscavator Beamafadesfdf";
				bholsterbeam1.northAngleTolerance = 8;



				AIBeamShooter2 bholsterbeam2 = companion.gameObject.AddComponent<AIBeamShooter2>();
				bholsterbeam2.beamTransform = m_CachedGunAttachPoint.transform;
				bholsterbeam2.beamModule = beholsterbeam.beamModule;
				bholsterbeam2.beamProjectile = beholsterbeam.projectile;
				bholsterbeam2.firingEllipseCenter = m_CachedGunAttachPoint.transform.position;
				bholsterbeam2.name = "Detscavator afaus";
				bholsterbeam2.northAngleTolerance = -8;

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
				new CustomBeholsterLaserBehavior() {
					//ShootPoint = m_CachedGunAttachPoint,

					InitialCooldown = 1.5f,
					firingTime = 3f,
					AttackCooldown = 3.5f,
					RequiresLineOfSight = true,
					UsesCustomAngle = true,
					
					chargeTime = 1,
					UsesBaseSounds = false,
					LaserFiringSound = "Play_BigBeam",
					StopLaserFiringSound = "Stop_BigBeam",
					ChargeAnimation = "charge",
					FireAnimation = "shootlaser",
					PostFireAnimation = "finishlaser",
					beamSelection = ShootBeamBehavior.BeamSelection.Random,
					trackingType = CustomBeholsterLaserBehavior.TrackingType.Follow,
				//initialAimType = CustomShootBeamBehavior.InitialAimType.Aim,

					unitCatchUpSpeed = 3,
					maxTurnRate = 3,
					turnRateAcceleration = 3,
					useDegreeCatchUp = companion.transform,
					minDegreesForCatchUp = 3,
					degreeCatchUpSpeed = 180,
					useUnitCatchUp = true,
					minUnitForCatchUp = 2,
					maxUnitForCatchUp = 2,
					useUnitOvershoot = true,
					minUnitForOvershoot = 1,
					firingType = CustomBeholsterLaserBehavior.FiringType.TOWARDS_PLAYER_AND_NORTHANGLEVARIANCE,
					unitOvershootTime = 0.25f,
					unitOvershootSpeed = 3,
					//ShootPoint = m_CachedGunAttachPoint.transform,
					//BulletScript = new CustomBulletScriptSelector(typeof(Wailer.Wail))
				}
				};
				/*
				 * 	public string ChargeAnimation;
	public string FireAnimation;

				*/
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:detscavator", companion.aiActor);


				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Detscavator/detscavator_idle-001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:detscavator";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Detscavator/detscavator_idle-001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("detscavatorivonammo");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\detscavatorivonammo.png");
                PlanetsideModule.Strings.Enemies.Set("#DETSCAVATOR", "Detscavator");
				PlanetsideModule.Strings.Enemies.Set("#DETSCAVATOR_SHORTDESC", "Subtract And Divide");
				PlanetsideModule.Strings.Enemies.Set("#DETSCAVATOR_LONGDESC", "Unlike their robotic brethren that fire beams in 4 different directions, the single, high-powered mining beam used by the Detscavator has prevented it from being sacrificed for speeding up mining.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#DETSCAVATOR";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#DETSCAVATOR_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#DETSCAVATOR_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:detscavator");
				EnemyDatabase.GetEntry("psog:detscavator").ForcedPositionInAmmonomicon = 73;
				EnemyDatabase.GetEntry("psog:detscavator").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:detscavator").isNormalEnemy = true;


			}
		}



		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/Detscavator/detscavator_idle-001.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_idle-002.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_idle-003.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_idle-004.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_idle-005.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_idle-006.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_idle-007.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_idle-008.png",

			//death
			"Planetside/Resources/Enemies/Detscavator/detscavator_charge_001.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_charge_002.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_charge_003.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_charge_004.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_charge_005.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_charge_006.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_charge_007.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_charge_008.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_charge_009.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_charge_010.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_charge_011.png",

			"Planetside/Resources/Enemies/Detscavator/detscavator_fire_001.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_fire_002.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_fire_003.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_fire_004.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_fire_005.png",
			"Planetside/Resources/Enemies/Detscavator/detscavator_fire_006.png",

			"Planetside/Resources/Enemies/Detscavator/detscavator_death_001.png",


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
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");
				//base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
				if (!base.aiActor.IsBlackPhantom)
				{
					Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
					mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
					mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
					mat.SetFloat("_EmissiveColorPower", 1.55f);
					mat.SetFloat("_EmissivePower", 100);
					mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);

					aiActor.sprite.renderer.material = mat;

				}
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					//AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Fade_01", base.aiActor.gameObject);
					AkSoundEngine.PostEvent("Play_enm_mech_death_01", base.aiActor.gameObject);

				};
			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo == "warn")
				{
					AkSoundEngine.PostEvent("Play_ENM_hammer_target_01", base.gameObject);
				}
			}
		}
	}
}





