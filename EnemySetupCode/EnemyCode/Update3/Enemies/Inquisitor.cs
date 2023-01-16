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
using BreakAbleAPI;

namespace Planetside
{
	public class Inquisitor : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "inquisitor";
		//private static tk2dSpriteCollectionData InquisitorCollection;

		public static void Init()
		{
			Inquisitor.BuildPrefab();
		}

		public static void BuildPrefab()
		{
            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("InquisitorCollection").GetComponent<tk2dSpriteCollectionData>();
            Material matEye = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("inquisitor material");
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
			{
				prefab = EnemyBuilder.BuildPrefabBundle("Inquisitor", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(8, 9), false, true);
                var companion = prefab.AddComponent<EnemyBehavior>();
                EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, Collection, matEye, false);
                prefab.AddComponent<ForgottenEnemyComponent>();
				companion.aiActor.knockbackDoer.weight = 1000;
				companion.aiActor.MovementSpeed = 1.6f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(115f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(2.125f, 0.25f), "shadowPos");


				companion.aiActor.healthHaver.SetHealthMaximum(115f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 18,
					ManualOffsetY = 13,
					ManualWidth = 32,
					ManualHeight = 39,
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
					ManualOffsetX = 18,
					ManualOffsetY = 13,
					ManualWidth = 32,
					ManualHeight = 39,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				//companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;

				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Prefix = "idle",
					AnimNames = new string[] { "idle_right", "idle_left"},
					Flipped = new DirectionalAnimation.FlipType[2]
				};

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "die", new string[] { "death" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "chargeup", new string[] { "chargeup_right", "chargeup_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "chargeupslow", new string[] { "chargeupslow_right", "chargeupslow_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "chargefire", new string[] { "chargefire_right", "chargefire_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "lasercharge", new string[] { "lasercharge_right", "lasercharge_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "laser", new string[] { "laser_right", "laser_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "unlaser", new string[] { "unlaser_right", "unlaser_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);

			
				AIActor actor = EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b");
				BeholsterController beholsterbeam = actor.GetComponent<BeholsterController>();

				
				List<string> BeamAnimPaths = new List<string>()
				{
					"Planetside/Resources/Beams/Inquistor/inqBeam_mid_001",
					"Planetside/Resources/Beams/Inquistor/inqBeam_mid_002",
					"Planetside/Resources/Beams/Inquistor/inqBeam_mid_003",
					"Planetside/Resources/Beams/Inquistor/inqBeam_mid_004",
				};
				List<string> StartAnimPaths = new List<string>()
				{
					"Planetside/Resources/Beams/Inquistor/inqBeam_start_001",
					"Planetside/Resources/Beams/Inquistor/inqBeam_start_002",
					"Planetside/Resources/Beams/Inquistor/inqBeam_start_003",
					"Planetside/Resources/Beams/Inquistor/inqBeam_start_004",
				};
				List<string> End = new List<string>()
				{
					"Planetside/Resources/Beams/Inquistor/inqBeam_end_001",
					"Planetside/Resources/Beams/Inquistor/inqBeam_end_002",
					"Planetside/Resources/Beams/Inquistor/inqBeam_end_003",
					"Planetside/Resources/Beams/Inquistor/inqBeam_end_004",
				};

				List<string> BeamAnimPathsTele = new List<string>()
				{
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_mid_001",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_mid_002",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_mid_003",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_mid_004",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_mid_005",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_mid_006",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_mid_007",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_mid_008",
				};
				List<string> BeamStartPathsTele = new List<string>()
				{
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_start_001",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_start_002",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_start_003",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_start_004",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_start_005",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_start_006",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_start_007",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_start_008",

				};
				List<string> BeamEndPathsTele = new List<string>()
				{
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_end_001",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_end_002",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_end_003",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_end_004",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_end_005",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_end_006",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_end_007",
					"Planetside/Resources/Beams/Inquistor/inqBeamtelegraph_end_008",

				};

				List<string> BeamAnimPathsDiss = new List<string>()
				{
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_mid_001",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_mid_002",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_mid_003",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_mid_004",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_mid_005",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_mid_006",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_mid_007",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_mid_008",

				};

				List<string> BeamStartAnimPathsDiss = new List<string>()
				{
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_start_001",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_start_002",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_start_003",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_start_004",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_start_005",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_start_006",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_start_007",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_start_008",


				};

				List<string> BeamEndAnimPathsDiss = new List<string>()
				{
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_end_001",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_end_002",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_end_003",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_end_004",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_end_005",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_end_006",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_end_007",
					"Planetside/Resources/Beams/Inquistor/inqBeamdissipate_end_008",



				};

				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);


				BasicBeamController beamComp = projectile.GenerateBeamPrefab(
					"Planetside/Resources/Beams/Inquistor/inqBeam_start_001",
					new Vector2(16, 16),
					new Vector2(0, 0),
					BeamAnimPaths,
					6,
					//Beam Impact
					null,
					6,
					new Vector2(16, 16),
					new Vector2(0, 0),
					//End of the Beam
					End,
					-1,
					null,
					null,
					//Start of the Beam
					StartAnimPaths,
					12,
					new Vector2(16, 16),
					new Vector2(0, 0), false,
					true,
					BeamAnimPathsTele, 8,
					BeamStartPathsTele, 8,
					BeamEndPathsTele, 8,
					1,
					true,
					BeamAnimPathsDiss, 9,
					BeamStartAnimPathsDiss, 9,
					BeamEndAnimPathsDiss, 9,
					1
					);


				projectile.gameObject.SetActive(false);
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				projectile.gameObject.AddComponent<MarkForUndodgeAbleBeam>();

				EmmisiveBeams emiss = beamComp.gameObject.AddComponent<EmmisiveBeams>();
				emiss.EmissiveColorPower = 10f;
				emiss.EmissivePower = 100;

				ProjectileModule projectileModule = ProjectileModule.CreateClone(beholsterbeam.beamModule, false, -1);
				projectileModule.projectiles[0] = projectile;

				GameObject LaserOne = EnemyToolbox.GenerateShootPoint(companion.gameObject, companion.sprite.WorldCenter, "Laser1");
				AIBeamShooter2 bholsterbeam1 = companion.gameObject.AddComponent<AIBeamShooter2>();
				bholsterbeam1.beamTransform = LaserOne.transform;
				bholsterbeam1.beamModule = projectileModule;
				bholsterbeam1.beamProjectile = projectile;
				bholsterbeam1.firingEllipseCenter = LaserOne.transform.position;
				bholsterbeam1.northAngleTolerance = -36;
				bholsterbeam1.firingEllipseA = 1.75f;
				bholsterbeam1.firingEllipseB = 0.75f;
				bholsterbeam1.eyeballFudgeAngle = 5;


				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				companion.aiActor.reinforceType = ReinforceType.SkipVfx;
				Creationist.TrespassEnemyEngageDoer trespassEngager = companion.aiActor.gameObject.AddComponent<Creationist.TrespassEnemyEngageDoer>();
				trespassEngager.PortalLifeTime = 5;
				trespassEngager.PortalSize = 0.3f;

				//bool flag3 = InquisitorCollection == null;
				//if (flag3)
				{
					/*
					InquisitorCollection = SpriteBuilder.ConstructCollection(prefab, "InquisitorCollection");
					UnityEngine.Object.DontDestroyOnLoad(InquisitorCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], InquisitorCollection);
					}
					*/
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					0,
					1,
					2,
					2,
					3,
					3,
					4,			
					4,
					3,
					2,
					1,
					1,
					0,
					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					5,
					6,
					7,
					7,
					8,
					8,
					9,
					9,		
					8,
					7,
					6,
					6,
					5,				
					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					10,
					11,
					12,
					13,
					14,
					15,
					15,
					16,
					16,
					16,
					17,
					18,
					19,
					20,
					21,
					22,
					23,
					24
					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Play_BigGuyDeath" }, { 8, "Play_BiGGuyDethAgain" } });
					EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 3, "SpawnDeathPortal" } });

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					25,
					26,
					27,
					28,
					28,
					29,
					29,
					29,
					30,
					31,
					32,
					32,
					33,
					33,
					34,
					35,
					36,
					37,
					38,
					39,
					40,
					41

					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "awaken", new Dictionary<int, string> { { 0, "Play_BOSS_lichA_turn_01" }, { 8, "Play_BOSS_dragun_stomp_01" }, { 11, "Play_BigGuyGrowl" }, { 14, "Play_ENM_blobulord_reform_01" } });

					List<int> chargeUpLeft = new List<int>()
					{
					43,
					44,
					44,
					45,
					45,
					45,
					46,
					46,
					46,
					46,
					};
					List<int> chargeUpRight = new List<int>()
					{
					48,
					49,
					50,
					50,
					51,
					51,
					51,
					52,
					52,
					53,
					53
					};


					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, chargeUpLeft, "chargeup_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					//EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "chargeup_left", new Dictionary<int, string> { { 0, "Blast" } });
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "chargeup_left", new Dictionary<int, string> { { 0, "Play_ENM_blobulord_charge_01" }, { 9, "Play_ENM_mummy_cast_01" } });
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, chargeUpRight, "chargeup_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "chargeup_right", new Dictionary<int, string> { { 0, "Play_ENM_blobulord_charge_01" }, { 9, "Play_ENM_mummy_cast_01" } });


					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, chargeUpRight, "chargeupslow_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, chargeUpLeft, "chargeupslow_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "chargeupslow_left", new Dictionary<int, string> { { 0, "Play_ENM_blobulord_intro_01" }, { 6, "Play_BOSS_dragun_charge_01" } });
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "chargeupslow_right", new Dictionary<int, string> { { 0, "Play_ENM_blobulord_intro_01" }, { 6, "Play_BOSS_dragun_charge_01" } });


					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					47,
					47,
					46,
					45,
					44,
					44,
					43,
					43,
					42,
					42,
					}, "chargefire_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 28f;
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "chargefire_left", new Dictionary<int, string> { { 0, "Play_Stomp" } });
					EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "chargefire_left", new Dictionary<int, string> { { 0, "Stompy" } });

					EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(2.125f, 3.3125f), "LeftHandFire");
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					53,
					53,
					52,
					51,
					50,
					50,
					49,
					49,
					48,
					48,
					}, "chargefire_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 28f;
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "chargefire_right", new Dictionary<int, string> { { 0, "Play_Stomp" } });
					EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "chargefire_right", new Dictionary<int, string> { { 0, "Stompy" } });

					EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(2.875f, 3.3125f), "RightHandFire");

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					54,
					55,
					56,
					56,
					57,
					57,
					}, "lasercharge_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "lasercharge_left", new Dictionary<int, string> { { 0, "Play_ENM_squidface_illusion_01" }, {5, "Play_BOSS_omegaBeam_charge_01" } });
					EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "lasercharge_left", new Dictionary<int, string> { { 3, "LaserCharge" }});

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					58,
					59,
					60,
					60,
					61,
					61
					}, "lasercharge_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "lasercharge_right", new Dictionary<int, string> { { 0, "Play_ENM_squidface_illusion_01" }, { 5, "Play_BOSS_omegaBeam_charge_01" } });
					EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "lasercharge_right", new Dictionary<int, string> { { 3, "LaserCharge" } });
					//pewGobrr
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					62,
					63,
					64,
					65,
					}, "laser_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
					EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "laser_left", new Dictionary<int, string> { { 2, "pewGobrr" } });

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					66,
					67,
					68,
					69
					}, "laser_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
					EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "laser_right", new Dictionary<int, string> { { 2, "pewGobrr" } });

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					57,
					56,
					55,
					55,
					54,
					54
					}, "unlaser_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					61,
					60,
					59,
					59,
					58,
					58
					}, "unlaser_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
				}

				/*
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "attack", new Dictionary<int, string> { { 0, "Blast" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 5, "deathBurst" }});
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Play_Squeal" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "chargeattack", new Dictionary<int, string> { { 0, "Play_EnergySwirl" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack", new Dictionary<int, string> { { 0, "Play_Stomp" } });
				*/

				GameObject shootpoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.5f, 0.5f), "CreationistShootpoint");

				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

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
					PathInterval = 0.125f,
					SpecifyRange = false,
					MinActiveRange = 0f,
					MaxActiveRange = 0f
				}
				};
				//lasercharge//laser//unlaser//chargeup//chargefire

				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
				{
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
						Behavior = new ShootBehavior{
						ShootPoint = shootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(Force)),
						LeadAmount = 0f,
						AttackCooldown = 2.25f,
						Cooldown = 2f,
						InitialCooldown = 0.5f,
						ChargeTime = 0.8f,
						RequiresLineOfSight = false,
						MultipleFireEvents = false,
						Uninterruptible = true,
						ChargeAnimation = "chargeup",
						PostFireAnimation = "chargefire",
						}
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
						Behavior = new ShootBehavior{
						ShootPoint = shootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(Repel)),
						LeadAmount = 0f,
						AttackCooldown = 3f,
						Cooldown = 5,
						InitialCooldown = 2f,
						ChargeTime = 1.6f,
						RequiresLineOfSight = false,
						MultipleFireEvents = false,
						Uninterruptible = true,
						ChargeAnimation = "chargeupslow",
						PostFireAnimation = "chargefire",
						}
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 2f,
						Behavior = new CustomBeholsterLaserBehavior{
						InitialCooldown = 4f,
						firingTime = 6f,
						AttackCooldown = 3f,
						Cooldown = 8f,
						RequiresLineOfSight = false,
						UsesCustomAngle = true,
						chargeTime = 1.5f,
						UsesBaseSounds = false,
						LaserFiringSound = "Play_ENM_deathray_shot_01",
						StopLaserFiringSound = "Stop_ENM_deathray_loop_01",
						ChargeAnimation = "lasercharge",
						FireAnimation = "laser",
						PostFireAnimation = "unlaser",
						beamSelection = ShootBeamBehavior.BeamSelection.Random,
						trackingType = CustomBeholsterLaserBehavior.TrackingType.ConstantTurn,

						DoesSpeedLerp = true,
						TimeToReachFullSpeed = 1.5f,

						DoesReverseSpeedLerp = true,
						TimeToStayAtZeroSpeedAt = 0.5f,
						TimeToReachEndingSpeed = 1.5f,
				
						unitCatchUpSpeed = 1f,
						maxTurnRate = 30f,
						turnRateAcceleration = 1.2f,
						useDegreeCatchUp = companion.transform,
						minDegreesForCatchUp = 0,
						degreeCatchUpSpeed = 1,
						useUnitCatchUp = true,
						minUnitForCatchUp = 1,
						maxUnitForCatchUp = 1,
						useUnitOvershoot = true,
						minUnitForOvershoot = 1,
						FacesLaserAngle = true,
						firingType = CustomBeholsterLaserBehavior.FiringType.TOWARDS_PLAYER_AND_NORTHANGLEVARIANCE,
						unitOvershootTime = 0.5f,
						unitOvershootSpeed = 1.25f,
						BulletScript = new CustomBulletScriptSelector(typeof(FakeUndodgeableBeam)),
						ShootPoint = shootpoint.transform,
						StopDuring = CustomBeholsterLaserBehavior.StopType.All
						}
					},

				};
				//lasercharge//laser//unlaser

				
				
				/*
				string basepath = "Planetside/Resources/Enemies/Creationist/";
				DebrisObject shoulder1 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "creationistDebris1.png", true, 0.5f, 3, 540, 120, null, 0.9f, "Play_BOSS_lichA_crack_01", null, 0);
				DebrisObject shoulder2 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "creationistDebris2.png", true, 0.5f, 3, 360, 120, null, 0.7f, "Play_BOSS_lichA_crack_01", null, 1);
				ShardCluster BONES = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shoulder1, shoulder2}, 0.9f, 2f, 1, 2, 1f);
				SpawnShardsOnDeath BodyAndStuff = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
				BodyAndStuff.deathType = OnDeathBehavior.DeathType.Death;
				BodyAndStuff.breakStyle = MinorBreakable.BreakStyle.BURST;
				BodyAndStuff.verticalSpeed = 0.4f;
				BodyAndStuff.heightOffGround = 2f;
				BodyAndStuff.shardClusters = new ShardCluster[] { BONES };
				*/

				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:inquisitor", companion.aiActor);




				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Ammocom/inqICon.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:inquisitor";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Ammocom/inqICon";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("sheetInquisitorTrespass");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\sheetInquisitorTrespass.png");
                PlanetsideModule.Strings.Enemies.Set("#INQUISITOR", "Inquisitor");
				PlanetsideModule.Strings.Enemies.Set("#INQUISITOR_SHORTDESC", "With Great Power");
				PlanetsideModule.Strings.Enemies.Set("#INQUISITOR_LONGDESC", "Those who are able to stand against their new found powers are able to keep a semblance of their former selves.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#INQUISITOR";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#INQUISITOR_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#INQUISITOR_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:inquisitor");
				EnemyDatabase.GetEntry("psog:inquisitor").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:inquisitor").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:inquisitor").isNormalEnemy = true;



				companion.aiActor.sprite.usesOverrideMaterial = true;
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
				mat.SetFloat("_EmissiveColorPower", 3f);
				mat.SetFloat("_EmissivePower", 50);
				companion.aiActor.sprite.renderer.material = mat;

			}
		}


		private static string basePath = "Planetside/Resources/Enemies/Inquisitor/";

		private static string[] spritePaths = new string[]
		{
			basePath+"inquisitor_idle_left_001.png",//0
			basePath+"inquisitor_idle_left_002.png",
			basePath+"inquisitor_idle_left_003.png",
			basePath+"inquisitor_idle_left_004.png",
			basePath+"inquisitor_idle_left_005.png",//4

			basePath+"inquisitor_idle_right_001.png",//5
			basePath+"inquisitor_idle_right_002.png",
			basePath+"inquisitor_idle_right_003.png",
			basePath+"inquisitor_idle_right_004.png",
			basePath+"inquisitor_idle_right_005.png",//9

			basePath+"inquisitor_death_001.png",//10
			basePath+"inquisitor_death_002.png",
			basePath+"inquisitor_death_003.png",
			basePath+"inquisitor_death_004.png",
			basePath+"inquisitor_death_005.png",
			basePath+"inquisitor_death_006.png",
			basePath+"inquisitor_death_007.png",
			basePath+"inquisitor_death_008.png",
			basePath+"inquisitor_death_009.png",
			basePath+"inquisitor_death_010.png",
			basePath+"inquisitor_death_011.png",
			basePath+"inquisitor_death_012.png",
			basePath+"inquisitor_death_013.png",
			basePath+"inquisitor_death_014.png",
			basePath+"inquisitor_death_015.png",//24

			basePath+"inquisitor_awaken_001.png",//25
			basePath+"inquisitor_awaken_002.png",
			basePath+"inquisitor_awaken_003.png",
			basePath+"inquisitor_awaken_004.png",
			basePath+"inquisitor_awaken_005.png",
			basePath+"inquisitor_awaken_006.png",
			basePath+"inquisitor_awaken_007.png",
			basePath+"inquisitor_awaken_008.png",
			basePath+"inquisitor_awaken_009.png",
			basePath+"inquisitor_awaken_010.png",
			basePath+"inquisitor_awaken_011.png",
			basePath+"inquisitor_awaken_012.png",
			basePath+"inquisitor_awaken_013.png",
			basePath+"inquisitor_awaken_014.png",
			basePath+"inquisitor_awaken_015.png",
			basePath+"inquisitor_awaken_016.png",
			basePath+"inquisitor_awaken_017.png",//41

			basePath+"inquisitor_chargeleft_001.png",//42
			basePath+"inquisitor_chargeleft_002.png",
			basePath+"inquisitor_chargeleft_003.png",
			basePath+"inquisitor_chargeleft_004.png",
			basePath+"inquisitor_chargeleft_005.png",
			basePath+"inquisitor_chargeleft_006.png",//47

			basePath+"inquisitor_chargeright_001.png",//48
			basePath+"inquisitor_chargeright_002.png",
			basePath+"inquisitor_chargeright_003.png",
			basePath+"inquisitor_chargeright_004.png",
			basePath+"inquisitor_chargeright_005.png",
			basePath+"inquisitor_chargeright_006.png",//53

			basePath+"inquisitor_lasahleft_001.png",//54
			basePath+"inquisitor_lasahleft_002.png",
			basePath+"inquisitor_lasahleft_003.png",
			basePath+"inquisitor_lasahleft_004.png",//57

			basePath+"inquisitor_lasahright_001.png",//58
			basePath+"inquisitor_lasahright_002.png",
			basePath+"inquisitor_lasahright_003.png",
			basePath+"inquisitor_lasahright_004.png",//61

			basePath+"inquisitor_lasahpewleft_001.png",//62
			basePath+"inquisitor_lasahpewleft_002.png",
			basePath+"inquisitor_lasahpewleft_003.png",
			basePath+"inquisitor_lasahpewleft_004.png",//65

			basePath+"inquisitor_lasahpewright_001.png",//66
			basePath+"inquisitor_lasahpewright_002.png",
			basePath+"inquisitor_lasahpewright_003.png",
			basePath+"inquisitor_lasahpewright_004.png",//69 //nice

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
				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{

				};
			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo.Contains("ddasasd"))
				{
					var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PortalClose"));
					partObj.transform.position = base.aiActor.transform.Find("CreationistShootpoint").position;
					partObj.transform.localScale *= 1f;
					Destroy(partObj, 3.4f);
					//LaserCharge
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Stompy"))
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

					string shootPointCalc = (string)((BraveMathCollege.AbsAngleBetween(this.aiActor.aiAnimator.FacingDirection, 0f) <= 90f) ? "RightHandFire" : "LeftHandFire");

					GameObject gameObject = GameObject.Instantiate(silencerVFX.gameObject, base.aiActor.transform.Find(shootPointCalc).position, Quaternion.identity);
					Destroy(gameObject, 2.5f);

					Exploder.DoDistortionWave(base.aiActor.transform.Find(shootPointCalc).position, 10f, 0.05f, 10, 0.5f);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("LaserCharge"))
                {
					string shootPointCalc = (string)((BraveMathCollege.AbsAngleBetween(this.aiActor.aiAnimator.FacingDirection, 0f) <= 90f) ? "RightHandFire" : "LeftHandFire");
					Vector2 pos = base.aiActor.transform.Find(shootPointCalc).position;
					GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(pos, 12, 0.075f, 30, 0.5f));
					StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(pos, 0, base.aiActor.transform.Find(shootPointCalc));

				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("pewGobrr"))
                {
					string shootPointCalc = (string)((BraveMathCollege.AbsAngleBetween(this.aiActor.aiAnimator.FacingDirection, 0f) <= 90f) ? "RightHandFire" : "LeftHandFire");
					Exploder.DoDistortionWave(base.aiActor.transform.Find(shootPointCalc).position - new Vector3(0, 0.5f), 5f, 0.05f, 2, 0.25f);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("SpawnDeathPortal"))
                {
					GameObject portalObj = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX, this.aiActor.sprite.WorldBottomCenter, Quaternion.Euler(0f, 0f, 0f));
					portalObj.layer = this.aiActor.gameObject.layer + (int)GameManager.Instance.MainCameraController.CurrentZOffset;
					portalObj.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
					MeshRenderer mesh = portalObj.GetComponent<MeshRenderer>();
					mesh.material.SetTexture("_PortalTex", StaticTextures.NebulaTexture);
					GameManager.Instance.StartCoroutine(PortalDoer(mesh, true));
				}
			}
			private IEnumerator PortalDoer(MeshRenderer portal, bool DestroyWhenDone = false)
			{
				float elapsed = 0f;
				while (elapsed < 3)
				{
					elapsed += BraveTime.DeltaTime;
					float t = elapsed / 3;
					if (portal.gameObject == null) { yield break; }
					float throne1 = Mathf.Sin(t * (Mathf.PI));
					portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0, 0.4f, throne1));
					portal.material.SetFloat("_HoleEdgeDepth", Mathf.Lerp(12, 2, throne1));
					yield return null;
				}

				if (DestroyWhenDone == true)
				{
					Destroy(portal.gameObject);
				}
				yield break;
			}

		}

		public class Repel : Script
        {
			protected override IEnumerator Top()
			{
				string shootPointCalc = (string)((BraveMathCollege.AbsAngleBetween(this.BulletBank.aiActor.aiAnimator.FacingDirection, 0f) <= 90f) ? "RightHandFire" : "LeftHandFire");
				float Pos = (base.GetPredictedTargetPosition(0.6f, 34) - base.BulletBank.aiActor.transform.Find(shootPointCalc).PositionVector2()).ToAngle();
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableQuickHoming);
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);

				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBig);

				for (int e = -3; e < 4; e++)
                {
					this.Fire(Offset.OverridePosition(base.BulletBank.aiActor.transform.Find(shootPointCalc).position), new Direction(Pos + (20 * e), DirectionType.Absolute, -1f), new Speed(3f, SpeedType.Absolute), new Repel.Def());
				}
				for (int e = -3; e < 3; e++)
                {
					this.Fire(Offset.OverridePosition(base.BulletBank.aiActor.transform.Find(shootPointCalc).position), new Direction(Pos + (20 * e)+10, DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), new Repel.Def(30));
				}
				this.Fire(Offset.OverridePosition(base.BulletBank.aiActor.transform.Find(shootPointCalc).position), new Direction(Pos, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new Repel.Ballin());

				yield break;
			}
			public class Def : Bullet
			{
				public Def(float delay = 0) : base(StaticUndodgeableBulletEntries.undodgeableQuickHoming.Name, false, false, false)
				{
					Delay = delay;
				}
				protected override IEnumerator Top()
				{
					yield return this.Wait(Delay);
					base.ChangeSpeed(new Speed(8f, SpeedType.Absolute), 60);
					yield break;
				}
				private float Delay;
			}
			public class Spore : Bullet
			{
				public Spore() : base("undodgeableSpore", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
					yield return this.Wait(480);
					base.Vanish(false);
					yield break;
				}
			}
			public class Ballin : Bullet
			{
				public Ballin() : base(StaticUndodgeableBulletEntries.undodgeableBig.Name, false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(18f, SpeedType.Absolute), 60);
					yield break;
				}
				public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					float H = UnityEngine.Random.Range(-180, 180);
					base.PostWwiseEvent("Play_ENM_creecher_burst_01");

					for (int i = 0; i < 20; i++)
					{
						this.Fire(new Direction((18 * i) + H, DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(1f, 8), SpeedType.Absolute), new Spore());
					}
					base.OnBulletDestruction(destroyType, hitRigidbody, preventSpawningProjectiles);
				}
			}

		}
		public class FakeUndodgeableBeam : Script
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
				AIBeamShooter2[] beams = this.BulletBank.aiActor.GetComponents<AIBeamShooter2>();
				yield return this.Wait(30);
				int i = 0;
				while (beams != null || beams.Length > 0)
                {
					foreach (AIBeamShooter2 beam in beams)
					{
						if (beam && beam.LaserBeam && i % 3 == 0)
						{
							LinkedList<BasicBeamController.BeamBone> linkedList = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", beam.m_laserBeam);
							LinkedListNode<BasicBeamController.BeamBone> last = linkedList.Last;
							Vector2 bonePosition = beam.m_laserBeam.GetBonePosition(last.Value);
							this.Fire(Offset.OverridePosition(bonePosition), new Direction(UnityEngine.Random.Range(-180, 180), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(1, 3)), new FakeBeamPart());
						}
					}
					i++;
					yield return this.Wait(1);
				}
				yield break;

			}
			public class FakeBeamPart : Bullet
			{
				public FakeBeamPart() : base(StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name, false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					this.Projectile.IgnoreTileCollisionsFor(6000f);
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 75);
					yield return this.Wait(600);
					base.Vanish(false);
					yield break;
				}
			}
		
		}

		public class Force : Script
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBig);
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBigBullet);
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableChainLink);

				string shootPointCalc = (string)((BraveMathCollege.AbsAngleBetween(this.BulletBank.aiActor.aiAnimator.FacingDirection, 0f) <= 90f) ? "RightHandFire" : "LeftHandFire");
				float Pos = (base.GetPredictedTargetPosition(0.6f, 34) - base.BulletBank.aiActor.transform.Find(shootPointCalc).PositionVector2()).ToAngle();
				this.Fire(Offset.OverridePosition(base.BulletBank.aiActor.transform.Find(shootPointCalc).position), new Direction(Pos, DirectionType.Absolute, -1f), new Speed(13f, SpeedType.Absolute), new Force.HelixBullet(false, 0 , StaticUndodgeableBulletEntries.undodgeableBig.Name, 0 ,0));
				for (int e = 0; e < 7; e++)
				{
					base.Fire(Offset.OverridePosition(base.BulletBank.aiActor.transform.Find(shootPointCalc).position), new Direction(Pos, DirectionType.Absolute, -1f), new Speed(13, SpeedType.Absolute), new Force.HelixBullet(false, (4 * e)+6, StaticUndodgeableBulletEntries.undodgeableChainLink.Name, -0.75f, 0.75f));
					base.Fire(Offset.OverridePosition(base.BulletBank.aiActor.transform.Find(shootPointCalc).position), new Direction(Pos, DirectionType.Absolute, -1f), new Speed(13, SpeedType.Absolute), new Force.HelixBullet(true, (4 * e)+6, StaticUndodgeableBulletEntries.undodgeableChainLink.Name, - 0.75f, 0.75f));

				}
				yield break;
			}
			public class BasicBigBall : Bullet
			{
				public BasicBigBall() : base(StaticUndodgeableBulletEntries.undodgeableBig.Name, false, false, false) { }
				protected override IEnumerator Top()
				{
					while (this.Projectile)
                    {
						base.PostWwiseEvent("Play_OBJ_ash_burst_02");
						for (int i = 0; i < 12; i++)
                        {
							this.Fire(new Direction(30*i, DirectionType.Aim, -1f), new Speed(3f, SpeedType.Absolute), new Spore());
						}
						yield return this.Wait(45);
					}
					yield break;
				}
                public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
					float H = UnityEngine.Random.Range(-180, 180);
					base.PostWwiseEvent("Play_ENM_creecher_burst_01");
					for (int i = 0; i < 6; i++)
					{
						this.Fire(new Direction((60 * i)+H, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Big());
					}
					for (int i = 0; i < 20; i++)
					{
						this.Fire(new Direction((18 * i) + H, DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(0.5f, 6), SpeedType.Absolute), new Spore());
					}
					base.OnBulletDestruction(destroyType, hitRigidbody, preventSpawningProjectiles);
                }
            }

			public class HelixBullet : Bullet
			{
				public HelixBullet(bool reverse, int delay, string BulletType, float Power, float NegativePower) : base(BulletType, false, false, false)
				{
					this.reverse = reverse;
					this.Delay = delay;
					this.bullettype = BulletType;
					base.SuppressVfx = true;
					this.Power = Power;
					this.NegativePower = NegativePower;

				}
				protected override IEnumerator Top()
				{
					this.ManualControl = true;
					yield return base.Wait(this.Delay);
					Vector2 truePosition = this.Position;
					float startVal = 1;
					for (int i = 0; i < 360; i++)
					{
						if (i % 45 == 0 && bullettype == StaticUndodgeableBulletEntries.undodgeableBig.Name)
                        {
							for (int t = 0; t < 12; t++)
							{
								this.Fire(new Direction(30 * t, DirectionType.Aim, -1f), new Speed(3f, SpeedType.Absolute), new Spore());
							}
						}
						float offsetMagnitude = Mathf.SmoothStep(NegativePower, Power, Mathf.PingPong(startVal + (float)i / 90f * 3f, 1f));
						truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 90f);
						this.Position = truePosition + (this.reverse ? BraveMathCollege.DegreesToVector(this.Direction + 90f, offsetMagnitude) : BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude));
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}

				public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (bullettype == StaticUndodgeableBulletEntries.undodgeableBig.Name)
                    {
						float H = UnityEngine.Random.Range(-180, 180);
						base.PostWwiseEvent("Play_ENM_creecher_burst_01");
						for (int i = 0; i < 6; i++)
						{
							this.Fire(new Direction((60 * i) + H, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Big());
						}
						for (int i = 0; i < 20; i++)
						{
							this.Fire(new Direction((18 * i) + H, DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(0.5f, 6), SpeedType.Absolute), new Spore());
						}
					}
						
					base.OnBulletDestruction(destroyType, hitRigidbody, preventSpawningProjectiles);
				}
				private float Power;
				private float NegativePower;

				private bool reverse;
				private int Delay;
				private string bullettype;
			}

			public class Big : Bullet
			{
				public Big() : base(StaticUndodgeableBulletEntries.undodgeableBigBullet.Name, false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(18f, SpeedType.Absolute), 75);
					yield return this.Wait(480);
					base.Vanish(false);
					yield break;
				}
			}

			public class Spore : Bullet
			{
				public Spore() : base("undodgeableSpore", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
					yield return this.Wait(480);
					base.Vanish(false);
					yield break;
				}
			}
			
		}

		public class TelegraphScript : Script 
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
				for (int i = -1; i < 2; i++)
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
					component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
					component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
					GameManager.Instance.StartCoroutine(FlashReticles(component2, false, Angle, Offset, this, "directedfire"));
				}
				yield return this.Wait(75);
				yield break;
			}
			public class UndodgeableBullshit : Bullet
			{
				public UndodgeableBullshit() : base("sniperUndodgeable", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					yield break;
				}
			}
			private IEnumerator FlashReticles(tk2dTiledSprite tiledspriteObject, bool isDodgeAble, float Angle, float Offset, TelegraphScript parent, string BulletType)
			{
				tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
				float elapsed = 0;
				float Time = 0.5f;
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
						tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, base.AimDirection + Mathf.Lerp(0, Offset, t));
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.dimensions = new Vector2(1000f, 1f);
						tiledsprite.UpdateZDepth();

						Angle = base.AimDirection + Mathf.Lerp(0, Offset, t);
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.5f;
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
				if (isDodgeAble == false)
				{
					base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
				}
				for (int i = 0; i < 3; i++)
				{
					base.Fire(new Direction(Angle, DirectionType.Absolute, -1f), new Speed(11f + (i * 1.5f), SpeedType.Absolute), new UndodgeableBullshit());
				}
				yield break;
			}

		}


	}
}





