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
//using DirectionType = DirectionalAnimation.DirectionType;
using static DirectionalAnimation;
using EnemyBulletBuilder;
using Pathfinding;
using SaveAPI;
using HutongGames.PlayMaker.Actions;
using static Planetside.PrisonerSubPhase2Attacks;
using static ETGMod;
using Planetside;
using System.Security.Cryptography;

namespace Planetside
{
	class Ophanaim : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "Ophanaim";
		//private static tk2dSpriteCollectionData OphanaimCollectiom;

		public static void Init()
		{
			Ophanaim.BuildPrefab();
		}
		public static void BuildPrefab()
		{
            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("OphanaimCollection").GetComponent<tk2dSpriteCollectionData>();
            Material matEye = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("ophanaim material");

            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
			{
				prefab = BossBuilder.BuildPrefabBundle("Ophanaim", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(0, 0), false, true);
				var enemy = prefab.AddComponent<EyeEnemyBehavior>();
				AIAnimator aiAnimator = enemy.aiAnimator;
                EnemyToolbox.QuickAssetBundleSpriteSetup(enemy.aiActor, Collection, matEye, false);


                enemy.aiActor.knockbackDoer.weight = 35;
				enemy.aiActor.MovementSpeed = 0.5f;
				enemy.aiActor.healthHaver.PreventAllDamage = false;
				enemy.aiActor.CollisionDamage = 1f;
				enemy.aiActor.HasShadow = false;
				enemy.aiActor.IgnoreForRoomClear = false;
				enemy.aiActor.aiAnimator.HitReactChance = 0f;
				enemy.aiActor.specRigidbody.CollideWithOthers = true;
				enemy.aiActor.specRigidbody.CollideWithTileMap = true;
				enemy.aiActor.PreventFallingInPitsEver = false;
				enemy.aiActor.healthHaver.ForceSetCurrentHealth(1150f);
				enemy.aiActor.SetIsFlying(true, "Gamemode: Creative", true, true);
				enemy.aiActor.CollisionKnockbackStrength = 10f;
				enemy.aiActor.CanTargetPlayers = true;
				enemy.aiActor.healthHaver.SetHealthMaximum(1150f, null, false);

                EnemyToolbox.AddShadowToAIActor(enemy.aiActor, StaticEnemyShadows.massiveShadow, new Vector2(4f, 0.25f), "shadowPos");


                enemy.aiActor.gameObject.GetOrAddComponent<ObjectVisibilityManager>();


                aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "idle",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};

				EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "dash", new string[] { "dash" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "blast", new string[] { "blast" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "blast_charge", new string[] { "blast_charge" }, new DirectionalAnimation.FlipType[0]);

                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "uncharge_laser", new string[] { "uncharge_laser" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "charge_laser", new string[] { "charge_laser" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "laser", new string[] { "laser" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "fade_out", new string[] { "fade_out" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "fade_in", new string[] { "fade_in" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "superlaser", new string[] { "superlaser" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "begin_cast", new string[] { "begin_cast" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "cast", new string[] { "cast" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "un_cast", new string[] { "un_cast" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "charge_s", new string[] { "charge_s" }, new DirectionalAnimation.FlipType[0]);

                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "uncharge_mithrix", new string[] { "uncharge_mithrix" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "charge_mithrix", new string[] { "charge_mithrix" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "mithrix", new string[] { "mithrix" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(enemy.aiAnimator, "wizardshit", new string[] { "wizardshit" }, new DirectionalAnimation.FlipType[0]);



                //if (OphanaimCollectiom == null)
				{
                    /*
					OphanaimCollectiom = SpriteBuilder.ConstructCollection(prefab, "OphanaimCollection");
					UnityEngine.Object.DontDestroyOnLoad(OphanaimCollectiom);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], OphanaimCollectiom);
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
					}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 10f;


                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    8,
					9,
					10,
					11,
					12,
					13,
					13,
					14,
					14,
					15,
					15,
					15,
					15,
					14,
					14,
					13,
					12,
					11,
					10,
					9,
					8
                    }, "dash", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 30f;
                    EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "dash", new Dictionary<int, string> { { 2, "Play_ENM_highpriest_dash_01" } });
                    EnemyToolbox.AddEventTriggersToAnimation(enemy.spriteAnimator, "dash", new Dictionary<int, string>() { { 4, "enableImage" }, { 17, "disableImage" } });

                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    16,
					17,
					18,
					19,
					20,
					20,
                    19,
                    20,
                    19,
                    20,
                    19,
                    19,
                    20,
                    20,
                    20,
                    }, "blast_charge", tk2dSpriteAnimationClip.WrapMode.Once).fps = 15f;
                    EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "blast_charge", new Dictionary<int, string> { { 1, "Play_BOSS_agunim_charge_03" } });


                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    21,
                    21,
                    22,
                    22,
                    23,
				    24,
				    25,
				    26,
				    27,
				    28,
				    29
                    }, "blast", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    47,
					48,
					49,
					50,
					50,
					51,
					51,
					52,
					52,
					53,
					53,
					53
                    }, "charge_laser", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;

                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    54,
					55,
					56,
					57,
					58,
					59
                    }, "laser", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 10f;
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    53,
					52,
					51,
					50,
					49,
					48,
					47
                    }, "uncharge_laser", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;


                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    47,
                    48,
                    49,
                    50,
                    50,
                    51,
                    51,
                    52,
                    52,
                    53,
                    53,
                    53
                    }, "charge_mithrix", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    47,
                    48,
                    49,
                    50,
                    50,
                    51,
                    51,
                    52,
                    52,
                    53,
                    53,
                    53,
                    53,
                    53,
                    53,
                    53,
                    53,
                    53,
                    53,
                    53,
                    53,
                    }, "charge_s", tk2dSpriteAnimationClip.WrapMode.Once).fps = 4f;


                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    54,
                    55,
                    56,
                    57,
                    58,
                    59
                    }, "mithrix", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 10f;

                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    53,
                    52,
                    51,
                    50,
                    49,
                    48,
                    47
                    }, "uncharge_mithrix", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    47,
                    48,
                    49,
                    50,
                    50,
                    51,
                    51,
                    52,
                    52,
                    53,
                    53,
                    53,
                    54,
                    55,
                    56,
                    57,
                    58,
                    59,
                    54,
                    55,
                    56,
                    57,
                    58,
                    59,
                    55,
                    56,
                    57,
                    58,
                    59,
                    53,
                    52,
                    51,
                    50,
                    49,
                    48,
                    47,
                    
                    }, "superlaser", tk2dSpriteAnimationClip.WrapMode.Once).fps = 12f;


                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    60,
					61,
					62,
					63,
					64,
					65,
					66,
					67,
					68
                    }, "fade_out", tk2dSpriteAnimationClip.WrapMode.Once).fps = 15f;
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    68,
                    67,
                    66,
                    65,
                    64,
                    63,
                    62,
                    61,
                    60
                    }, "fade_in", tk2dSpriteAnimationClip.WrapMode.Once).fps = 15f;
                    //tp1
                    //m_BOSS_agunim_intro_01

                    EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "fade_out", new Dictionary<int, string> { { 3, "Play_BOSS_agunim_intro_01" } });
                    EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "fade_in", new Dictionary<int, string> { { 1, "Play_ENM_beholster_teleport_02" } });

                    EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "fade_out", new Dictionary<int, string> { { 3, "tp1" } });
                    EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "fade_in", new Dictionary<int, string> { { 5, "tp2" } });

                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    69,
                    70,
                    71,
                    72,
                    73,
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
                    }, "begin_cast", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
                    EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "begin_cast", new Dictionary<int, string> { { 4, "Play_BOSS_agunim_charge_02" }, { 14, "Play_BOSS_agunim_volley_01" } });

                    //m_BOSS_agunim_charge_02
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    76,
                    75,
                    74,
                    73,
                    72,
                    71,
                    70,
                    69
                    }, "un_cast", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;

                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    77,
                    78,
                    79,
                    80,
                    81,
                    82,
                    83,
                    84
                    }, "cast", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 15f;

                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    69,
                    70,
                    71,
                    72,
                    73,
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
                    84,
                    77,
                    78,
                    79,
                    80,
                    81,
                    82,
                    83,
                    84,
                    77,
                    78,
                    79,
                    80,
                    81,
                    82,
                    83,
                    84,
                    77,
                    78,
                    79,
                    80,
                    81,
                    82,
                    83,
                    84,
                                        77,
                    78,
                    79,
                    80,
                    81,
                    82,
                    83,
                    84,
                    77,
                    78,
                    79,
                    80,
                    81,
                    82,
                    83,
                    84,
                    77,
                    78,
                    79,
                    80,
                    81,
                    82,
                    83,
                    84,
                    77,
                    78,
                    79,
                    80,
                    81,
                    82,
                    83,
                    84,


                    76,
                    75,
                    74,
                    73,
                    72,
                    71,
                    70,
                    69

                    }, "wizardshit", tk2dSpriteAnimationClip.WrapMode.Once).fps = 17f;

                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    47,
                    48,
                    49,
                    50,
                    50,
                    51,
                    51,
                    52,
                    52,
                    53,
                    53,
                    53,
                    54,
                    55,
                    56,
                    57,
                    58,
                    59,
                    54,
                    55,
                    56,
                    57,
                    58,
                    59,
                    54,
                    55,
                    56,
                    57,
                    58,
                    59,
                    54,
                    55,
                    56,
                    57,
                    58,
                    59
                    }, "begin_flight", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {
                    54,
                    55,
                    56,
                    57,
                    58,
                    59,
                    }, "flight", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 10f;





                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
					{
                    68,
                    67,
                    66,
                    65,
                    64,
                    63,
                    62,
                    61,
                    60,
                    0,
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    0,
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    0,
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,

                    }, "intro", tk2dSpriteAnimationClip.WrapMode.Once).fps = 12f;
                    EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "intro", new Dictionary<int, string> { { 0, "fuck_me" } });

                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
					{
					85,
                    85,
                    85,
                    86,
                    86,
                    86,
                    87,
                    87,
                    88,
                    88,
                    89,
                    89,
                    90,
                    91,
                    92,
                    93,
                    94,
                    95,
                    96

					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;
                    EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 16, "fuck_me" } });
                    EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 1, "Play_EyeRoar" } });

                }



                enemy.aiActor.specRigidbody.PixelColliders.Clear();
				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 42,
					ManualOffsetY = 24,
					ManualWidth = 44,
					ManualHeight = 47,
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
                    ManualOffsetX = 42,
                    ManualOffsetY = 24,
                    ManualWidth = 44,
					ManualHeight = 47,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});

				enemy.aiActor.PreventBlackPhantom = false;


                AIActor actor = EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b");
                BeholsterController beholsterbeam = actor.GetComponent<BeholsterController>();

                GameObject mainShootpoint = EnemyToolbox.GenerateShootPoint(enemy.gameObject, enemy.sprite.WorldCenter, "center");
                GameObject mainbeamShootpoint = EnemyToolbox.GenerateShootPoint(enemy.gameObject, enemy.sprite.WorldCenter + new Vector2(0, -0.875f), "beamCenter1");

                AIBeamShooter2 beamShooter1 = EnemyToolbox.AddAIBeamShooter(enemy.aiActor, mainbeamShootpoint.transform, "middle_small_beam", beholsterbeam.projectile, beholsterbeam.beamModule);



                Projectile beam = null;
                var HM = EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5");
                foreach (Component item in HM.GetComponentsInChildren(typeof(Component)))
                {
                    if (item is BossFinalRogueLaserGun laser)
                    {
                        if (laser.beamProjectile)
                        {
                            beam = laser.beamProjectile;
                            break;
                        }
                    }
                }


                AIBeamShooter2 A = EnemyToolbox.AddAIBeamShooter(enemy.aiActor, mainbeamShootpoint.transform, "superbeam", beam, null, 0);
                List<int> ids = new List<int>();
				EnemyToolbox.AddOffsetToFrames(enemy.spriteAnimator, "blast_charge", new Dictionary<int, Vector3>() { 
					{0, new Vector3(-1, 0) },
                    {1, new Vector3(-1, 0) },
                    {2, new Vector3(-1, 0) },
                    {3, new Vector3(-1, 0) },
                    {4, new Vector3(-1, 0) },
                    {5, new Vector3(-1, 0) },
                },
                ids);
                EnemyToolbox.AddOffsetToFrames(enemy.spriteAnimator, "blast", new Dictionary<int, Vector3>() {
                    {0, new Vector3(-1, 0) },
                    {1, new Vector3(-1, 0) },
                    {2, new Vector3(-1, 0) },
                    {3, new Vector3(-1, 0) },
                    {4, new Vector3(-1, 0) },
                    {5, new Vector3(-1, 0) },
                    {6, new Vector3(-1, 0) },
                    {7, new Vector3(-1, 0) },
                    {8, new Vector3(-1, 0) },
                    {9, new Vector3(-1, 0) },
                    {10, new Vector3(-1, 0) },
                },
                ids);
                EnemyToolbox.AddOffsetToFrames(enemy.spriteAnimator, "begin_cast", new Dictionary<int, Vector3>() {
                    {0, new Vector3(-1, 0) },
                    {1, new Vector3(-1, 0) },
                    {2, new Vector3(-1, 0) },
                    {3, new Vector3(-1, 0) },
                    {4, new Vector3(-1, 0) },
                    {5, new Vector3(-1, 0) },
                    {6, new Vector3(-1, 0) },
                    {7, new Vector3(-1, 0) },

                },
                ids);
                EnemyToolbox.AddOffsetToFrames(enemy.spriteAnimator, "cast", new Dictionary<int, Vector3>() {
                    {0, new Vector3(-1, 0) },
                    {1, new Vector3(-1, 0) },
                    {2, new Vector3(-1, 0) },
                    {3, new Vector3(-1, 0) },
                    {4, new Vector3(-1, 0) },
                    {5, new Vector3(-1, 0) },
                    {6, new Vector3(-1, 0) },
                    {7, new Vector3(-1, 0) },
                },
                ids);
                EnemyToolbox.AddOffsetToFrames(enemy.spriteAnimator, "un_cast", new Dictionary<int, Vector3>() {
                    {0, new Vector3(-1, 0) },
                    {1, new Vector3(-1, 0) },
                    {2, new Vector3(-1, 0) },
                    {3, new Vector3(-1, 0) },
                    {4, new Vector3(-1, 0) },
                    {5, new Vector3(-1, 0) },
                    {6, new Vector3(-1, 0) },
                    {7, new Vector3(-1, 0) },
                },
               ids);

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
                /*
                 * new ShootBehavior() {
						ShootPoint = mainShootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(BlastAttack)),
						LeadAmount = 0f,
						AttackCooldown = 1.5f,
						Cooldown = 3f,
						InitialCooldown = 0.5f,
						RequiresLineOfSight = true,
						MultipleFireEvents = false,
						Uninterruptible = true,
						ChargeAnimation = "blast_charge",
						PostFireAnimation = "blast"
                        },
                */

                bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>()
                {
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0f,
                    Behavior =new OphanaimFlightBehavior() {
                        ShootPoint = mainShootpoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(THESUN)),
                        LeadAmount = 0f,
                        AttackCooldown = 4f,
                        ChargeTime = 3.5f,
                        Cooldown = 90f,
                        InitialCooldown = 0.5f,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = false,
                        Uninterruptible = true,
                        ChargeAnimation = "charge_s",
                        FireAnimation = "mithrix",
                        PostFireAnimation = "uncharge_mithrix",
                        MaxUsages = 1
                        },
                    NickName = "supermegacoolattack"
                    },//0       
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.8f,
                    Behavior =new ShootBehavior() {
                        ShootPoint = mainShootpoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(SweepAttack)),
                        LeadAmount = 0f,
                        AttackCooldown = 1.5f,
                        Cooldown = 10f,
                        InitialCooldown = 0.5f,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = false,
                        Uninterruptible = true,
                        ChargeAnimation = "begin_cast",
                        FireAnimation = "cast",
                        PostFireAnimation = "un_cast"
                        },
                    NickName = "endme"
                    },//1
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1.1f,
                    Behavior =new ShootBehavior() {
                        ShootPoint = mainbeamShootpoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(MithrixSlamTwo)),
                        LeadAmount = 0f,
                        AttackCooldown = 1.5f,
                        Cooldown = 7f,
                        InitialCooldown = 0.5f,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = false,
                        Uninterruptible = true,
                        ChargeAnimation = "charge_mithrix",
                        FireAnimation = "mithrix",
                        PostFireAnimation = "uncharge_mithrix"
                    },
                    NickName = "endme"
                    },//2
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0f,
                        Behavior =new ShootBehavior() {
                        ShootPoint = mainbeamShootpoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(MithrixSlamOmega)),
                        LeadAmount = 0f,
                        AttackCooldown = 1.5f,
                        Cooldown = 10f,
                        InitialCooldown = 0.5f,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = false,
                        Uninterruptible = true,
                        ChargeAnimation = "charge_mithrix",
                        FireAnimation = "mithrix",
                        PostFireAnimation = "uncharge_mithrix"
                    },
                    NickName = "mihtrixslam_2"
                    },//3
                    //mihtrixslam_2
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1f,
                    Behavior = new CustomBeholsterLaserBehavior() {
                    InitialCooldown = 0f,
                    firingTime = 7.5f,
                    AttackCooldown = 2f,
                    Cooldown = 10,
                    RequiresLineOfSight = true,
                    UsesCustomAngle = true,
                    RampHeight = 14,
                    firingType = CustomBeholsterLaserBehavior.FiringType.TOWARDS_PLAYER,
                    chargeTime = 1.5f,
                    UsesBaseSounds = false,
                    AdditionalHeightOffset = 11,
                    EnemyChargeSound = "Play_BOSS_omegaBeam_charge_01",
                    LaserFiringSound = "Play_ENM_deathray_shot_01",
                    StopLaserFiringSound = "Stop_ENM_deathray_loop_01",
                    ChargeAnimation = "charge_laser",
                    FireAnimation = "laser",
                    PostFireAnimation = "uncharge_laser",
                    hurtsOtherHealthhavers = false,
                    beamSelection = ShootBeamBehavior.BeamSelection.Specify,
                    specificBeamShooters = new List<AIBeamShooter2>() { beamShooter1 },

                    trackingType = CustomBeholsterLaserBehavior.TrackingType.Follow,
                    DoesSpeedLerp = true,
                    InitialStartingSpeed = 0,
                    TimeToStayAtZeroSpeedAt = 0.5f,
                    TimeToReachFullSpeed = 1f,
                    LocksFacingDirection = false,	
                    
                    maxTurnRate = 10,
                    maxUnitForCatchUp = 10f,

                    minDegreesForCatchUp = 15,
                    minUnitForCatchUp = 2f,
                    minUnitForOvershoot = 1,

                    turnRateAcceleration = 20,
                    
					unitCatchUpSpeed = 10,
                    unitOvershootSpeed = 10,
                    unitOvershootTime = 0.25f,

                    degreeCatchUpSpeed = 180,

                    useDegreeCatchUp = enemy.transform,
                    useUnitCatchUp = true,
                    useUnitOvershoot = true,


                    ShootPoint = mainbeamShootpoint.transform,
                    BulletScript = new CustomBulletScriptSelector(typeof(FireBurst)),

					},
                    NickName = "LASERZ"
                    },//4
                    new AttackBehaviorGroup.AttackGroupItem()//5
                    {
                        Probability = 1f,
                        NickName = "Def2",
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {
                        new CustomDashBehavior()
						{
                            dashAnim = "dash",
                            ShootPoint = mainShootpoint,
                            dashDistance = 13f,
                            dashTime = 0.625f,
                            AmountOfDashes = 1,
                            enableShadowTrail = false,
                            Cooldown = 0.25f,
                            dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
                            warpDashAnimLength = true,
                            hideShadow = true,
                            fireAtDashStart = true,
                            InitialCooldown = 1f,
                            RequiresLineOfSight = false,
                            AttackCooldown = 0.25f,

                            bulletScript = new CustomBulletScriptSelector(typeof(BasicAttack)),
                        },   
                        new CustomDashBehavior()
                        {
                            dashAnim = "dash",
                            ShootPoint = mainShootpoint,
                            dashDistance = 13f,
                            dashTime = 0.625f,
                            AmountOfDashes = 1,
                            enableShadowTrail = false,
                            Cooldown = 0.25f,
                            dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
                            warpDashAnimLength = true,
                            hideShadow = true,
                            fireAtDashStart = true,
                            InitialCooldown = 1f,
                            RequiresLineOfSight = false,
                            AttackCooldown = 0.75f,
							bulletScript = new CustomBulletScriptSelector(typeof(BasicAttack)),
                        },
                        new CustomDashBehavior()
                        {
                            dashAnim = "dash",
                            ShootPoint = mainShootpoint,
                            dashDistance = 13f,
                            dashTime = 0.25f,
                            AmountOfDashes = 1,
                            enableShadowTrail = false,
                            Cooldown = 5f,
                            
                            dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
                            warpDashAnimLength = true,
                            hideShadow = true,
                            fireAtDashStart = true,
                            InitialCooldown = 1f,
                            RequiresLineOfSight = false,
                            AttackCooldown = 1.5f,
                            bulletScript = new CustomBulletScriptSelector(typeof(SpecialAttack)),
                        },
                        }
                        },
                    },//5
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 1f,
                        NickName = "agony",
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {
                        new TeleportBehavior{
						AttackableDuringAnimation = true,
						AllowCrossRoomTeleportation = false,
						teleportRequiresTransparency = false,
						hasOutlinesDuringAnim = true,
						ManuallyDefineRoom = false,
						MaxHealthThreshold = 1f,
						StayOnScreen = true,
						AvoidWalls = true,
						GoneTime = 0.5f,
						OnlyTeleportIfPlayerUnreachable = false,
						MinDistanceFromPlayer = 8f,
						MaxDistanceFromPlayer = 14f,
						teleportInAnim = "fade_in",
						teleportOutAnim = "fade_out",
						AttackCooldown = 0f,
						InitialCooldown = 0.5f,
						RequiresLineOfSight = false,
						roomMax = new Vector2(0,0),
						roomMin = new Vector2(0,0),
						teleportOutBulletScript = new CustomBulletScriptSelector(typeof(TeleportAttack)),
						GlobalCooldown = 0f,
						Cooldown = 0f,

						CooldownVariance = 0f,
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
						},
						new ShootBehavior() {
						ShootPoint = mainShootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(BlastAttack)),
						LeadAmount = 0f,
						AttackCooldown = 1f,
						Cooldown = 5f,
						InitialCooldown = 0.5f,
						RequiresLineOfSight = true,
						MultipleFireEvents = false,
						Uninterruptible = true,
						ChargeAnimation = "blast_charge",
						PostFireAnimation = "blast"
                        },
                        }
                        },
                    },//6
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0f,
                        NickName = "phase_2teleport",
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {
                        new TeleportBehavior{
                        AttackableDuringAnimation = true,
                        AllowCrossRoomTeleportation = false,
                        teleportRequiresTransparency = false,
                        hasOutlinesDuringAnim = true,
                        ManuallyDefineRoom = false,
                        MaxHealthThreshold = 1f,
                        StayOnScreen = true,
                        AvoidWalls = true,
                        GoneTime = 0.25f,
                        OnlyTeleportIfPlayerUnreachable = false,
                        MinDistanceFromPlayer = 8f,
                        MaxDistanceFromPlayer = 13f,
                        teleportInAnim = "fade_in",
                        teleportOutAnim = "fade_out",
                        AttackCooldown = 2.5f,
                        InitialCooldown = 0.5f,
                        RequiresLineOfSight = false,
                        roomMax = new Vector2(0,0),
                        roomMin = new Vector2(0,0),
						teleportInBulletScript = new CustomBulletScriptSelector(typeof(Teleport2)),
                        GlobalCooldown = 0f,
                        Cooldown = 0f,
                        CooldownVariance = 0f,
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
                        },
                        new TeleportBehavior{
                        AttackableDuringAnimation = true,
                        AllowCrossRoomTeleportation = false,
                        teleportRequiresTransparency = false,
                        hasOutlinesDuringAnim = true,
                        ManuallyDefineRoom = false,
                        MaxHealthThreshold = 1f,
                        StayOnScreen = true,
                        AvoidWalls = true,
                        GoneTime = 0.25f,
                        OnlyTeleportIfPlayerUnreachable = false,
                        MinDistanceFromPlayer = 8f,
                        MaxDistanceFromPlayer = 13f,
                        teleportInAnim = "fade_in",
                        teleportOutAnim = "fade_out",
                        AttackCooldown = 2.5f,
                        InitialCooldown = 0.5f,
                        RequiresLineOfSight = false,
                        roomMax = new Vector2(0,0),
                        roomMin = new Vector2(0,0),
                        teleportInBulletScript = new CustomBulletScriptSelector(typeof(Teleport2)),
						//teleportOutBulletScript = new CustomBulletScriptSelector(typeof(TeleportAttack)),
                        GlobalCooldown = 0f,
                        Cooldown = 0f,
                        CooldownVariance = 0f,
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
                        },
                        new TeleportBehavior{
                        AttackableDuringAnimation = true,
                        AllowCrossRoomTeleportation = false,
                        teleportRequiresTransparency = false,
                        hasOutlinesDuringAnim = true,
                        ManuallyDefineRoom = false,
                        MaxHealthThreshold = 1f,
                        StayOnScreen = true,
                        AvoidWalls = true,
                        GoneTime = 0.25f,
                        OnlyTeleportIfPlayerUnreachable = false,
                        MinDistanceFromPlayer = 8f,
                        MaxDistanceFromPlayer = 13f,
                        teleportInAnim = "fade_in",
                        teleportOutAnim = "fade_out",
                        AttackCooldown = 2.5f,
                        InitialCooldown = 0.5f,
                        RequiresLineOfSight = false,
                        roomMax = new Vector2(0,0),
                        roomMin = new Vector2(0,0),
                        teleportInBulletScript = new CustomBulletScriptSelector(typeof(Teleport2)),
						//teleportOutBulletScript = new CustomBulletScriptSelector(typeof(TeleportAttack)),
                        GlobalCooldown = 0f,
                        Cooldown = 0f,
                        CooldownVariance = 0f,
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
                        },
                        new TeleportBehavior{
                        AttackableDuringAnimation = true,
                        AllowCrossRoomTeleportation = false,
                        teleportRequiresTransparency = false,
                        hasOutlinesDuringAnim = true,
                        ManuallyDefineRoom = false,
                        MaxHealthThreshold = 1f,
                        StayOnScreen = true,
                        AvoidWalls = true,
                        GoneTime = 0.25f,
                        OnlyTeleportIfPlayerUnreachable = false,
                        MinDistanceFromPlayer = 7f,
                        MaxDistanceFromPlayer = 12f,
                        teleportInAnim = "fade_in",
                        teleportOutAnim = "fade_out",
                        AttackCooldown = 2f,
                        InitialCooldown = 0.5f,
                        RequiresLineOfSight = false,
                        roomMax = new Vector2(0,0),
                        roomMin = new Vector2(0,0),
                        teleportInBulletScript = new CustomBulletScriptSelector(typeof(Teleport2)),
						//teleportOutBulletScript = new CustomBulletScriptSelector(typeof(TeleportAttack)),
                        GlobalCooldown = 0f,
                        Cooldown = 16f,
                        CooldownVariance = 0f,
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
                        },
                        }
                        },
                    },//7
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0.8f,
                        NickName = "shitfest",
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {
                        new TeleportBehavior{
                        AttackableDuringAnimation = true,
                        AllowCrossRoomTeleportation = false,
                        teleportRequiresTransparency = false,
                        hasOutlinesDuringAnim = true,
                        ManuallyDefineRoom = false,
                        MaxHealthThreshold = 1f,
                        StayOnScreen = true,
                        AvoidWalls = true,
                        GoneTime = 0.05f,
                        OnlyTeleportIfPlayerUnreachable = false,
                        MinDistanceFromPlayer = 9f,
                        MaxDistanceFromPlayer = 15f,
                        teleportInAnim = "fade_in",
                        teleportOutAnim = "fade_out",

                        AttackCooldown = 0f,
                        InitialCooldown = 0f,
                        RequiresLineOfSight = false,
                        roomMax = new Vector2(0,0),
                        roomMin = new Vector2(0,0),
                        GlobalCooldown = 0f,
                        Cooldown = 0f,
                        CooldownVariance = 0f,
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
                        },
                        new CustomBeholsterLaserBehavior() {

				        InitialCooldown = 0f,
                        firingTime = 3f,
                        AttackCooldown = 0f,
						Cooldown = 0,
					    RequiresLineOfSight = true,
				        UsesCustomAngle = true,
						RampHeight = 14,
						firingType = CustomBeholsterLaserBehavior.FiringType.TOWARDS_PLAYER,
					    chargeTime = 0f,
				        UsesBaseSounds = false,
						AdditionalHeightOffset = 11,
                        UsesBeamProjectileWithoutModule = true,
                        EnemyChargeSound = "Play_BOSS_omegaBeam_charge_01",
                        FireAnimation = "superlaser",
                        beamSelection = ShootBeamBehavior.BeamSelection.Specify,
                        specificBeamShooters = new List<AIBeamShooter2>() { A },
                        trackingType = CustomBeholsterLaserBehavior.TrackingType.ConstantTurn,
				        DoesSpeedLerp = true,
			            InitialStartingSpeed = 0,
		                TimeToStayAtZeroSpeedAt = 0.5f,
						TimeToReachFullSpeed = 1f,
					    LocksFacingDirection = true,
				        maxTurnRate = 0,
			            maxUnitForCatchUp = 0f,

		                minDegreesForCatchUp = 0,
	                    minUnitForCatchUp = 0f,
					    minUnitForOvershoot = 0,

				        turnRateAcceleration = 0,

			            unitCatchUpSpeed = 0,
		                unitOvershootSpeed = 0,
	                    unitOvershootTime = 0,

						degreeCatchUpSpeed = 0,

						useDegreeCatchUp = enemy.transform,
						useUnitCatchUp = true,
						useUnitOvershoot = true,


						ShootPoint = mainbeamShootpoint.transform,
						BulletScript = new CustomBulletScriptSelector(typeof(FireSplit)),
                        },
                         new TeleportBehavior{
                        AttackableDuringAnimation = true,
                        AllowCrossRoomTeleportation = false,
                        teleportRequiresTransparency = false,
                        hasOutlinesDuringAnim = true,
                        ManuallyDefineRoom = false,
                        MaxHealthThreshold = 1f,
                        StayOnScreen = true,
                        AvoidWalls = true,
                        GoneTime = 0.05f,
                        OnlyTeleportIfPlayerUnreachable = false,
                        MinDistanceFromPlayer = 9f,
                        MaxDistanceFromPlayer = 15f,
                        teleportInAnim = "fade_in",
                        teleportOutAnim = "fade_out",

                        AttackCooldown = 0f,
                        RequiresLineOfSight = false,
                        roomMax = new Vector2(0,0),
                        roomMin = new Vector2(0,0),
						//teleportInBulletScript = new CustomBulletScriptSelector(typeof(TeleportScript)),
                        GlobalCooldown = 0f,
                        Cooldown = 0f,
                        CooldownVariance = 0f,
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
                        },
                        new CustomBeholsterLaserBehavior() {

                        InitialCooldown = 0f,
                        firingTime = 3f,
                        AttackCooldown = 0f,
                        Cooldown = 0,
                        RequiresLineOfSight = true,
                        UsesCustomAngle = true,
                        RampHeight = 14,
                        firingType = CustomBeholsterLaserBehavior.FiringType.TOWARDS_PLAYER,
                        chargeTime = 0f,
                        UsesBaseSounds = false,
                        UsesBeamProjectileWithoutModule = true,
                        AdditionalHeightOffset = 11,
                        EnemyChargeSound = "Play_BOSS_omegaBeam_charge_01",
                        FireAnimation = "superlaser",
                        beamSelection = ShootBeamBehavior.BeamSelection.Specify,
                        specificBeamShooters = new List<AIBeamShooter2>() { A },
                        trackingType = CustomBeholsterLaserBehavior.TrackingType.ConstantTurn,
                        DoesSpeedLerp = true,
                        InitialStartingSpeed = 0,
                        TimeToStayAtZeroSpeedAt = 0.5f,
                        TimeToReachFullSpeed = 1f,
                        LocksFacingDirection = true,
                        maxTurnRate = 0,
                        maxUnitForCatchUp = 0f,

                        minDegreesForCatchUp = 0,
                        minUnitForCatchUp = 0f,
                        minUnitForOvershoot = 0,

                        turnRateAcceleration = 20,

                        unitCatchUpSpeed = 0,
                        unitOvershootSpeed = 0,
                        unitOvershootTime = 0,

                        degreeCatchUpSpeed = 0,

                        useDegreeCatchUp = enemy.transform,
                        useUnitCatchUp = true,
                        useUnitOvershoot = true,


                        ShootPoint = mainbeamShootpoint.transform,
                        BulletScript = new CustomBulletScriptSelector(typeof(FireSplit720)),
                        },
                         new TeleportBehavior{
                        AttackableDuringAnimation = true,
                        AllowCrossRoomTeleportation = false,
                        teleportRequiresTransparency = false,
                        hasOutlinesDuringAnim = true,
                        ManuallyDefineRoom = false,
                        MaxHealthThreshold = 1f,
                        StayOnScreen = true,
                        AvoidWalls = true,
                        GoneTime = 0.05f,
                        OnlyTeleportIfPlayerUnreachable = false,
                        MinDistanceFromPlayer = 9f,
                        MaxDistanceFromPlayer = 15f,
                        teleportInAnim = "fade_in",
                                                teleportOutAnim = "fade_out",

                        AttackCooldown = 0f,
                        InitialCooldown = 0.5f,
                        RequiresLineOfSight = false,
                        roomMax = new Vector2(0,0),
                        roomMin = new Vector2(0,0),
						//teleportInBulletScript = new CustomBulletScriptSelector(typeof(TeleportScript)),
                        GlobalCooldown = 0f,
                        Cooldown = 0f,
                        CooldownVariance = 0f,
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
                        },
                        new CustomBeholsterLaserBehavior() {

                        InitialCooldown = 0f,
                        firingTime = 3f,
                        AttackCooldown = 0f,
                        Cooldown = 0,
                        RequiresLineOfSight = true,
                        UsesCustomAngle = true,
                        RampHeight = 14,
                        firingType = CustomBeholsterLaserBehavior.FiringType.TOWARDS_PLAYER,
                        chargeTime = 0f,
                        UsesBaseSounds = false,
                        AdditionalHeightOffset = 11,
                        EnemyChargeSound = "Play_BOSS_omegaBeam_charge_01",
                        FireAnimation = "superlaser",
                        UsesBeamProjectileWithoutModule = true,
                        beamSelection = ShootBeamBehavior.BeamSelection.Specify,
                        specificBeamShooters = new List<AIBeamShooter2>() { A },
                        trackingType = CustomBeholsterLaserBehavior.TrackingType.ConstantTurn,
                        DoesSpeedLerp = true,
                        InitialStartingSpeed = 0,
                        TimeToStayAtZeroSpeedAt = 0.5f,
                        TimeToReachFullSpeed = 1f,
                        LocksFacingDirection = true,
                        maxTurnRate = 0,
                        maxUnitForCatchUp = 0f,
                        minDegreesForCatchUp = 0,
                        minUnitForCatchUp = 0f,
                        minUnitForOvershoot = 0,

                        turnRateAcceleration = 24,

                        unitCatchUpSpeed = 0,
                        unitOvershootSpeed = 0,
                        unitOvershootTime = 0,

                        degreeCatchUpSpeed = 0,

                        useDegreeCatchUp = enemy.transform,
                        useUnitCatchUp = true,
                        useUnitOvershoot = true,


                        ShootPoint = mainbeamShootpoint.transform,
                        BulletScript = new CustomBulletScriptSelector(typeof(FireSplit540)),
                        },
                         new TeleportBehavior{
                        AttackableDuringAnimation = true,
                        AllowCrossRoomTeleportation = false,
                        teleportRequiresTransparency = false,
                        hasOutlinesDuringAnim = true,
                        ManuallyDefineRoom = false,
                        MaxHealthThreshold = 1f,
                        StayOnScreen = true,
                        AvoidWalls = true,
                        GoneTime = 0.05f,
                        OnlyTeleportIfPlayerUnreachable = false,
                        MinDistanceFromPlayer = 9f,
                        MaxDistanceFromPlayer = 15f,
                        teleportInAnim = "fade_in",
                                                teleportOutAnim = "fade_out",

                        AttackCooldown = 0f,
                        InitialCooldown = 0.5f,
                        RequiresLineOfSight = false,
                        roomMax = new Vector2(0,0),
                        roomMin = new Vector2(0,0),
						//teleportInBulletScript = new CustomBulletScriptSelector(typeof(TeleportScript)),
                        GlobalCooldown = 0f,
                        Cooldown = 0f,
                        CooldownVariance = 0f,
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
                        },
                        new CustomBeholsterLaserBehavior() {

                        InitialCooldown = 0f,
                        firingTime = 8f,
                        AttackCooldown = 2.5f,
                        Cooldown = 10,
                        RequiresLineOfSight = true,
                        UsesCustomAngle = true,
                        RampHeight = 14,
                        firingType = CustomBeholsterLaserBehavior.FiringType.TOWARDS_PLAYER,
                        chargeTime = 0.5f,
                        UsesBaseSounds = false,
                        AdditionalHeightOffset = 11,
                        EnemyChargeSound = "Play_BOSS_omegaBeam_charge_01",

                        ChargeAnimation = "charge_laser",
                        FireAnimation = "laser",
                        PostFireAnimation = "uncharge_laser",


                        UsesBeamProjectileWithoutModule = true,
                        beamSelection = ShootBeamBehavior.BeamSelection.Specify,
                        specificBeamShooters = new List<AIBeamShooter2>() { A },
                        trackingType = CustomBeholsterLaserBehavior.TrackingType.ConstantTurn,
                        DoesSpeedLerp = true,
                        InitialStartingSpeed = 0,
                        TimeToStayAtZeroSpeedAt = 0.5f,
                        TimeToReachFullSpeed = 1f,
                        LocksFacingDirection = true,
                        maxTurnRate = 0,
                        maxUnitForCatchUp = 0f,

                        minDegreesForCatchUp = 0,
                        minUnitForCatchUp = 0f,
                        minUnitForOvershoot = 0,

                        turnRateAcceleration = 24,

                        unitCatchUpSpeed = 0,
                        unitOvershootSpeed = 0,
                        unitOvershootTime = 0,

                        degreeCatchUpSpeed = 0,

                        useDegreeCatchUp = enemy.transform,
                        useUnitCatchUp = true,
                        useUnitOvershoot = true,


                        ShootPoint = mainbeamShootpoint.transform,
                        BulletScript = new CustomBulletScriptSelector(typeof(FireSplitFinale)),
                        },
                        }
                        },
                    },//8
                    

                };

               
                bs.MovementBehaviors = new List<MovementBehaviorBase>() {
				new SeekTargetBehavior() {
					StopWhenInRange = false,
					CustomRange = 6,
					LineOfSight = true,
					ReturnToSpawn = true,
					SpawnTetherDistance = 0,
					PathInterval = 0.5f,
					SpecifyRange = false,
					MinActiveRange = -0.25f,
					MaxActiveRange = 0
				}
				};
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:ophanaim", enemy.aiActor);
				var nur = enemy.aiActor;

                nur.EffectResistances = new ActorEffectResistance[]
				{
					new ActorEffectResistance()
					{
						resistAmount = 1,
						resistType = EffectResistanceType.Fire
					},
				};

				if (enemy.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
				}
				GenericIntroDoer miniBossIntroDoer = prefab.AddComponent<GenericIntroDoer>();
				prefab.AddComponent<FungannonIntroController>();

				miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
				miniBossIntroDoer.initialDelay = 0.5f;
				miniBossIntroDoer.cameraMoveSpeed = 10;
				miniBossIntroDoer.specifyIntroAiAnimator = null;
				miniBossIntroDoer.BossMusicEvent = "Play_MUS_Boss_Theme_Beholster";
				//miniBossIntroDoer.BossMusicEvent = "Play_MUS_Lich_Double_01";
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
				PlanetsideModule.Strings.Enemies.Set("#OPHANAIM_NAME", "OPHANAIM");
				PlanetsideModule.Strings.Enemies.Set("#OPHANAIM_NAME_SMALL", "Ophanaim");

				PlanetsideModule.Strings.Enemies.Set("AAAAAAA", "OBSERVANT AIMGEL");
				PlanetsideModule.Strings.Enemies.Set("#QUOTE", "");
				enemy.aiActor.OverrideDisplayName = "#OPHANAIM_NAME_SMALL";

                miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
				{
					bossNameString = "#OPHANAIM_NAME",
					bossSubtitleString = "AAAAAAA",
					bossQuoteString = "#QUOTE",
					bossSpritePxOffset = IntVector2.Zero,
					topLeftTextPxOffset = IntVector2.Zero,
					bottomRightTextPxOffset = IntVector2.Zero,
					bgColor = Color.blue
				};
                var BossCardTexture = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("ophanaim_bosscard");
                if (BossCardTexture)
                {
                    miniBossIntroDoer.portraitSlideSettings.bossArtSprite = BossCardTexture;
                    miniBossIntroDoer.SkipBossCard = false;
                    enemy.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
                }
                else
				{
					miniBossIntroDoer.SkipBossCard = true;
					enemy.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
				}
				
				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Ammocom/ophanaimbossiconpng", SpriteBuilder.ammonomiconCollection);
				if (enemy.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
				}



                enemy.encounterTrackable = enemy.gameObject.AddComponent<EncounterTrackable>();
				enemy.encounterTrackable.journalData = new JournalEntry();
				enemy.encounterTrackable.EncounterGuid = "psog:ophanaim";
				enemy.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				enemy.encounterTrackable.journalData.SuppressKnownState = false;
				enemy.encounterTrackable.journalData.IsEnemy = true;
				enemy.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				enemy.encounterTrackable.ProxyEncounterGuid = "";
				enemy.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Ammocom/ophanaimbossiconpng";
				enemy.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("ophanaimsheetrt");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\ophanaimsheetrt.png");
                PlanetsideModule.Strings.Enemies.Set("#OPHANAIMAMMONOMICON", "Ophanaim");
				PlanetsideModule.Strings.Enemies.Set("#OPHANAIMAMMONOMICONSHORT", "Observant Aimgel");
				PlanetsideModule.Strings.Enemies.Set("#OPHANAIMAMMONOMICONLONG", "Ophanaim are some of the stronger of the Aimgels, and thus are tasked with commanding ocular forces to observe the Gungeon.\n\nTheir job, along with the jobs of the ocular force, is invaluable to the sanctity of the Gungeon.");
				enemy.encounterTrackable.journalData.PrimaryDisplayName = "#OPHANAIMAMMONOMICON";
				enemy.encounterTrackable.journalData.NotificationPanelDescription = "#OPHANAIMAMMONOMICONSHORT";
				enemy.encounterTrackable.journalData.AmmonomiconFullEntry = "#OPHANAIMAMMONOMICONLONG";
				EnemyBuilder.AddEnemyToDatabase(enemy.gameObject, "psog:ophanaim");
				EnemyDatabase.GetEntry("psog:ophanaim").ForcedPositionInAmmonomicon = 8;
				EnemyDatabase.GetEntry("psog:ophanaim").isInBossTab = true;
				EnemyDatabase.GetEntry("psog:ophanaim").isNormalEnemy = true;


                Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat.mainTexture = enemy.aiActor.sprite.renderer.material.mainTexture;
                mat.SetColor("_EmissiveColor", new Color32(255, 210, 178, 255));
                mat.SetFloat("_EmissiveColorPower", 1.55f);
                mat.SetFloat("_EmissivePower", 50);
                mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);

                enemy.aiActor.sprite.renderer.material = mat;

                AdditionalBraveLight braveLight = enemy.gameObject.AddComponent<AdditionalBraveLight>();
                braveLight.transform.position = enemy.sprite.WorldCenter;
                braveLight.LightColor = new Color(1, 0.82f, 0.625f);
                braveLight.LightIntensity = 5f;
                braveLight.LightRadius = 7f;
                braveLight.FadeOnActorDeath = true;
                braveLight.SpecifyActor = enemy.aiActor;

                //==================
                ImprovedAfterImage yeah = enemy.aiActor.gameObject.AddComponent<ImprovedAfterImage>();
                yeah.dashColor = new Color(1, 0.85f, 0.7f);
                yeah.spawnShadows = false;
				yeah.shadowTimeDelay = 0.033f;
				yeah.shadowLifetime = 0.5f;
				enemy.ownAfterImage = yeah;

				//==================
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(enemy.aiActor.healthHaver);
                enemy.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
                enemy.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("383175a55879441d90933b5c4e60cf6f").bulletBank.GetBullet("bigBullet"));
                enemy.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
                enemy.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").bulletBank.GetBullet("teeth_football"));
                enemy.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				enemy.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("suck"));
                enemy.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
                suckLessEntry = StaticUndodgeableBulletEntries.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("suck"), "suck_more");
                enemy.aiActor.bulletBank.Bullets.Add(suckLessEntry);
                //==================


                var fuckOff = EnemyDatabase.GetOrLoadByGuid("1a4872dafdb34fd29fe8ac90bd2cea67").bulletBank.Bullets[0].BulletObject.GetComponent<ProjectileTrailRendererController>().customTrailRenderer;


                Material matTrail = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                matTrail.mainTexture = fuckOff.material.mainTexture;
                matTrail.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
                matTrail.SetFloat("_EmissiveColorPower", 1.55f);
                matTrail.SetFloat("_EmissivePower", 50);
                matTrail.SetFloat("_EmissiveThresholdSensitivity", 255);

                {

                    TrailRenderer tr;
                    var tro = enemy.gameObject.AddChild("trail object 1");
                    tro.transform.position = enemy.aiActor.sprite.WorldCenter + new Vector2(-2.25f, -0.5f);
                    tr = tro.AddComponent<TrailRenderer>();
                    tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    tr.receiveShadows = false;
                    var mater = matTrail;
                    tr.material = mater;
                    tr.minVertexDistance = 0.1f;
                    //======
                    mat.SetColor("_Color", new Color(1, 0.85f, 0.7f));
                    tr.startColor = new Color(1, 0.85f, 0.7f);
                    tr.endColor = new Color(0.5f, 0.3f, 0);
                    //======
                    tr.time = 1.25f;
                    //======
                    tr.startWidth = 0.3125f;
                    tr.endWidth = 0;
                    tr.autodestruct = false;
                    tr.enabled = false;
                    enemy.trails.Add(tr);

                }
                {
                    TrailRenderer tr;
                    var tro = enemy.gameObject.AddChild("trail object 2");
                    tro.transform.position = enemy.aiActor.sprite.WorldCenter + new Vector2(2.25f, -0.5f);
                    tr = tro.AddComponent<TrailRenderer>();
                    tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    tr.receiveShadows = false;
                    var mater = matTrail;
                    tr.material = mater;
                    tr.minVertexDistance = 0.1f;
                    //======
                    mat.SetColor("_Color", new Color(1, 0.85f, 0.7f));
                    tr.startColor = new Color(1, 0.85f, 0.7f);
                    tr.endColor = new Color(0.5f, 0.3f, 0);
                    //======
                    tr.time = 1.25f;
                    //======
                    tr.startWidth = 0.3125f;
                    tr.endWidth = 0;
                    tr.autodestruct = false;
                    tr.enabled = false;
                    enemy.trails.Add(tr);

                }



                {
                    PlanetsideModule.Strings.Enemies.Set("#LENSHOT", "Lenshot");


                    var blessingObj = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Ophanaim/solarblast_idle_001", null, false);
                    FakePrefab.MarkAsFakePrefab(blessingObj);
                    UnityEngine.Object.DontDestroyOnLoad(blessingObj);
                    tk2dSpriteAnimator animator = blessingObj.GetOrAddComponent<tk2dSpriteAnimator>();
                    tk2dSpriteAnimation animation = blessingObj.AddComponent<tk2dSpriteAnimation>();

                    tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(blessingObj, ("SolarBlast_Collection"));

                    tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 10 };
                    List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

                    for (int i = 1; i < 8; i++)
                    {
                        tk2dSpriteCollectionData collection = DeathMarkcollection;
                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/Ophanaim/solarblast_idle_00{i}", collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter);
                        frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }
                    idleClip.frames = frames.ToArray();
                    idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
                    animator.Library = animation;
                    animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
                    animator.DefaultClipId = animator.GetClipIdByName("idle");
                    animator.playAutomatically = true;
                    

                    animator.sprite.usesOverrideMaterial = true;
                    Material spriteMat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                    spriteMat.mainTexture = animator.sprite.renderer.material.mainTexture;
                    spriteMat.SetColor("_EmissiveColor", new Color32(255, 210, 178, 255));
                    spriteMat.SetFloat("_EmissiveColorPower", 1.55f);
                    spriteMat.SetFloat("_EmissivePower", 50);

                    animator.sprite.renderer.material = mat;

                    SolarClap = blessingObj;
                }
                {
                    //string eyeDefString = "Planetside/Resources/VFX/Ophanaim/Minion/";
                    GameObject vfxObj = ItemBuilder.SpriteFromBundle("EyeballMinion", Collection.GetSpriteIdByName("babyeye_idle_front_001"), Collection);//ItemBuilder.AddSpriteToObject("EyeballMinion", eyeDefString + "babyeye_idle_front_001", null);
                    tk2dSpriteAnimator eyeAnimator = vfxObj.GetOrAddComponent<tk2dSpriteAnimator>();
                    AIAnimator aiAnimatorBody = vfxObj.AddComponent<AIAnimator>();

                    eyeAnimator.sprite.usesOverrideMaterial = true;
                    Material Handmat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                    Handmat.mainTexture = eyeAnimator.sprite.renderer.material.mainTexture;
                    Handmat.SetColor("_EmissiveColor", new Color32(255, 210, 178, 255));
                    Handmat.SetFloat("_EmissiveColorPower", 1.55f);
                    Handmat.SetFloat("_EmissivePower", 50);
                    Handmat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);


                    AdditionalBraveLight braveLight2 = eyeAnimator.gameObject.AddComponent<AdditionalBraveLight>();
                    braveLight2.transform.position = eyeAnimator.sprite.WorldCenter;
                    braveLight2.LightColor = new Color(1, 0.82f, 0.625f);
                    braveLight2.LightIntensity = 3f;
                    braveLight2.LightRadius = 3f;

                    eyeAnimator.sprite.renderer.material = Handmat;

                    aiAnimatorBody.IdleAnimation = new DirectionalAnimation
                    {
                        Type = DirectionalAnimation.DirectionType.SixWay,
                        Flipped = new DirectionalAnimation.FlipType[6],
                        AnimNames = new string[]
                        {
                        "idle_right",
                        "idle_right",
                        "idle_right",
                        "idle_front",
                        "idle_left",
                        "idle_left"
                        }
                    };
                    EnemyToolbox.AddNewDirectionAnimation(aiAnimatorBody, "die", new string[] { "die" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                    vfxObj.AddComponent<MeshFilter>();
                    vfxObj.AddComponent<MeshRenderer>();

                    SpriteBuilder.AddAnimation(eyeAnimator, Collection, new List<int>()
                    {
                        Collection.GetSpriteIdByName("babyeye_idle_front_001"),
                        Collection.GetSpriteIdByName("babyeye_idle_front_002"),
                        Collection.GetSpriteIdByName("babyeye_idle_front_003"),
                        Collection.GetSpriteIdByName("babyeye_idle_front_004"),

                    }, "idle_front", tk2dSpriteAnimationClip.WrapMode.Loop, 7);

                    SpriteBuilder.AddAnimation(eyeAnimator, Collection, new List<int>()
                    {
                        Collection.GetSpriteIdByName("babyeye_idle_left_001"),
                        Collection.GetSpriteIdByName("babyeye_idle_left_002"),
                        Collection.GetSpriteIdByName("babyeye_idle_left_003"),
                        Collection.GetSpriteIdByName("babyeye_idle_left_004"),

                    }, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop, 7);

                    SpriteBuilder.AddAnimation(eyeAnimator, Collection, new List<int>()
                    {
                        Collection.GetSpriteIdByName("babyeye_idle_right_001"),
                        Collection.GetSpriteIdByName("babyeye_idle_right_002"),
                        Collection.GetSpriteIdByName("babyeye_idle_right_003"),
                        Collection.GetSpriteIdByName("babyeye_idle_right_004"),

                    }, "idle_right", tk2dSpriteAnimationClip.WrapMode.Loop, 7);

                    SpriteBuilder.AddAnimation(eyeAnimator, Collection, new List<int>()
                    {
                        Collection.GetSpriteIdByName("babyeye_idle_spawn_001"),
                        Collection.GetSpriteIdByName("babyeye_idle_spawn_002"),
                        Collection.GetSpriteIdByName("babyeye_idle_spawn_003"),
                        Collection.GetSpriteIdByName("babyeye_idle_spawn_004"),
                        Collection.GetSpriteIdByName("babyeye_idle_spawn_005"),
                        Collection.GetSpriteIdByName("babyeye_idle_spawn_006"),
                    }, "spawn", tk2dSpriteAnimationClip.WrapMode.Once, 7);
                    

                    AdvancedBodyPartController bodyPart = vfxObj.AddComponent<AdvancedBodyPartController>();
                    bodyPart.Name = "eye";
                    bodyPart.Render = true;
                    bodyPart.OverrideFacingDirection = false;
                    bodyPart.faceTarget = true;
                    bodyPart.faceTargetTurnSpeed = 120;

                    //bodyPart.renderer.enabled = false;
                    

                    SpeculativeRigidbody body = vfxObj.AddComponent<SpeculativeRigidbody>();

                    body.CollideWithOthers = true;
                    body.CollideWithTileMap = false;

                    vfxObj.GetComponent<tk2dBaseSprite>().OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;

                    body.PixelColliders = new List<PixelCollider>();
                    body.PixelColliders.Add(new PixelCollider
                    {

                        ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                        CollisionLayer = CollisionLayer.EnemyHitBox,
                        IsTrigger = false,
                        BagleUseFirstFrameOnly = false,
                        SpecifyBagelFrame = string.Empty,
                        BagelColliderNumber = 0,
                        ManualOffsetX = -1,
                        ManualOffsetY = -1,
                        ManualWidth = 12,
                        ManualHeight = 13,
                        ManualDiameter = 0,
                        ManualLeftX = 0,
                        ManualLeftY = 0,
                        ManualRightX = 0,
                        ManualRightY = 0,

                    });
                    HealthHaver healthHaver = vfxObj.AddComponent<HealthHaver>();
                    healthHaver.SetHealthMaximum(100);
                    healthHaver.ForceSetCurrentHealth(100);
                    healthHaver.flashesOnDamage = true;
                    vfxObj.GetOrAddComponent<GameActor>();


                    ImprovedAfterImage uhasgd = vfxObj.gameObject.AddComponent<ImprovedAfterImage>();
                    uhasgd.dashColor = new Color(1, 0.85f, 0.7f);
                    uhasgd.spawnShadows = true;
                    uhasgd.shadowTimeDelay = 0.05f;
                    uhasgd.shadowLifetime = 0.33f;

                    bodyPart.ownBody = body;
                    bodyPart.ownHealthHaver = healthHaver;
                    FakePrefab.MarkAsFakePrefab(vfxObj);
                    UnityEngine.Object.DontDestroyOnLoad(vfxObj);
                    vfxObj.SetActive(false);
                    vfxObj.gameObject.AddComponent<OphanaimMinionController>();
                    var bulletb = vfxObj.gameObject.AddComponent<AIBulletBank>();
                    bulletb.Bullets = new List<AIBulletBank.Entry>()
                    {
                        EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"),
                        StaticUndodgeableBulletEntries.UndodgeableFrogger
                    };
                    EyeBallMinion = vfxObj;
                }
                {
                    var blessingObj = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Ophanaim/ophanaim_wing_left", null, false);
                    FakePrefab.MarkAsFakePrefab(blessingObj);
                    UnityEngine.Object.DontDestroyOnLoad(blessingObj);
                    var spr = blessingObj.GetComponent<tk2dBaseSprite>();

                    spr.usesOverrideMaterial = true;
                    Material spriteMat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                    spriteMat.mainTexture = spr.renderer.material.mainTexture;
                    spriteMat.SetColor("_EmissiveColor", new Color32(255, 210, 178, 255));
                    spriteMat.SetFloat("_EmissiveColorPower", 1.55f);
                    spriteMat.SetFloat("_EmissivePower", 50);
                    spr.renderer.material = mat;
                    ImprovedAfterImage aI = blessingObj.gameObject.AddComponent<ImprovedAfterImage>();
                    aI.dashColor = new Color(1, 0.85f, 0.7f);
                    aI.spawnShadows = false;
                    aI.shadowTimeDelay = 0.033f;
                    aI.shadowLifetime = 0.5f;
                    WingLeft = blessingObj;
                }
                {
                    var blessingObj = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Ophanaim/ophanaim_wing_right", null, false);
                    FakePrefab.MarkAsFakePrefab(blessingObj);
                    UnityEngine.Object.DontDestroyOnLoad(blessingObj);
                    var spr = blessingObj.GetComponent<tk2dBaseSprite>();

                    spr.usesOverrideMaterial = true;
                    Material spriteMat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                    spriteMat.mainTexture = spr.renderer.material.mainTexture;
                    spriteMat.SetColor("_EmissiveColor", new Color32(255, 210, 178, 255));
                    spriteMat.SetFloat("_EmissiveColorPower", 1.55f);
                    spriteMat.SetFloat("_EmissivePower", 50);
                    spr.renderer.material = mat;
                    ImprovedAfterImage aI = blessingObj.gameObject.AddComponent<ImprovedAfterImage>();
                    aI.dashColor = new Color(1, 0.85f, 0.7f);
                    aI.spawnShadows = false;
                    aI.shadowTimeDelay = 0.033f;
                    aI.shadowLifetime = 0.5f;
                    WingRight = blessingObj;

                }
            }
        }
        public static GameObject WingLeft;
        public static GameObject WingRight;
        public static AIBulletBank.Entry suckLessEntry;


        public static GameObject EyeBallMinion;

        public static GameObject SolarClap;
		private static string DefPath = "Planetside/Resources/Bosses/OphanaimUltra/";
		private static string[] spritePaths = new string[]
		{
			DefPath+"ophanaimultra_idle_001.png",//0
            DefPath+"ophanaimultra_idle_002.png",
            DefPath+"ophanaimultra_idle_003.png",
            DefPath+"ophanaimultra_idle_004.png",
            DefPath+"ophanaimultra_idle_005.png",
            DefPath+"ophanaimultra_idle_006.png",
            DefPath+"ophanaimultra_idle_007.png",
            DefPath+"ophanaimultra_idle_008.png",//7

            DefPath+"ophanaimultra_dash_001.png",//8
            DefPath+"ophanaimultra_dash_002.png",
            DefPath+"ophanaimultra_dash_003.png",
            DefPath+"ophanaimultra_dash_004.png",
            DefPath+"ophanaimultra_dash_005.png",
            DefPath+"ophanaimultra_dash_006.png",
            DefPath+"ophanaimultra_dash_007.png",
            DefPath+"ophanaimultra_dash_008.png",//15

            DefPath+"ophanaimultra_blast_001.png",//16
            DefPath+"ophanaimultra_blast_002.png",
            DefPath+"ophanaimultra_blast_003.png",
            DefPath+"ophanaimultra_blast_004.png",
            DefPath+"ophanaimultra_blast_005.png",
            DefPath+"ophanaimultra_blast_006.png",
            DefPath+"ophanaimultra_blast_007.png",
            DefPath+"ophanaimultra_blast_008.png",
            DefPath+"ophanaimultra_blast_009.png",
            DefPath+"ophanaimultra_blast_010.png",
            DefPath+"ophanaimultra_blast_011.png",
            DefPath+"ophanaimultra_blast_012.png",
            DefPath+"ophanaimultra_blast_013.png",
            DefPath+"ophanaimultra_blast_014.png",//29

            DefPath+"ophanaimultra_burst_001.png",//30
            DefPath+"ophanaimultra_burst_002.png",
            DefPath+"ophanaimultra_burst_003.png",
            DefPath+"ophanaimultra_burst_004.png",
            DefPath+"ophanaimultra_burst_005.png",
            DefPath+"ophanaimultra_burst_006.png",
            DefPath+"ophanaimultra_burst_007.png",
            DefPath+"ophanaimultra_burst_008.png",
            DefPath+"ophanaimultra_burst_009.png",//38

            DefPath+"ophanaimultra_burstshooty_001.png",//39
            DefPath+"ophanaimultra_burstshooty_002.png",
            DefPath+"ophanaimultra_burstshooty_003.png",
            DefPath+"ophanaimultra_burstshooty_004.png",
            DefPath+"ophanaimultra_burstshooty_005.png",
            DefPath+"ophanaimultra_burstshooty_006.png",
            DefPath+"ophanaimultra_burstshooty_007.png",
            DefPath+"ophanaimultra_burstshooty_008.png",//46

            DefPath+"ophanaimultra_chargesmall_001.png",//47
            DefPath+"ophanaimultra_chargesmall_002.png",
            DefPath+"ophanaimultra_chargesmall_003.png",
            DefPath+"ophanaimultra_chargesmall_004.png",
            DefPath+"ophanaimultra_chargesmall_005.png",
            DefPath+"ophanaimultra_chargesmall_006.png",
            DefPath+"ophanaimultra_chargesmall_007.png",//53

            DefPath+"ophanaimultra_firesmall_001.png",//54
            DefPath+"ophanaimultra_firesmall_002.png",
            DefPath+"ophanaimultra_firesmall_003.png",
            DefPath+"ophanaimultra_firesmall_004.png",
            DefPath+"ophanaimultra_firesmall_005.png",
            DefPath+"ophanaimultra_firesmall_006.png",//59

            DefPath+"ophanaimultra_fade_001.png",//60
            DefPath+"ophanaimultra_fade_002.png",
            DefPath+"ophanaimultra_fade_003.png",
            DefPath+"ophanaimultra_fade_004.png",
            DefPath+"ophanaimultra_fade_005.png",
            DefPath+"ophanaimultra_fade_006.png",
            DefPath+"ophanaimultra_fade_007.png",
            DefPath+"ophanaimultra_fade_008.png",
            DefPath+"ophanaimultra_fade_009.png",//68

            DefPath+"ophanaimultra_begincast_001.png",//69
            DefPath+"ophanaimultra_begincast_002.png",
            DefPath+"ophanaimultra_begincast_003.png",
            DefPath+"ophanaimultra_begincast_004.png",
            DefPath+"ophanaimultra_begincast_005.png",
            DefPath+"ophanaimultra_begincast_006.png",
            DefPath+"ophanaimultra_begincast_007.png",
            DefPath+"ophanaimultra_begincast_008.png",//76

            DefPath+"ophanaimultra_cast_001.png",//77
            DefPath+"ophanaimultra_cast_002.png",
            DefPath+"ophanaimultra_cast_003.png",
            DefPath+"ophanaimultra_cast_004.png",
            DefPath+"ophanaimultra_cast_005.png",
            DefPath+"ophanaimultra_cast_006.png",
            DefPath+"ophanaimultra_cast_007.png",
            DefPath+"ophanaimultra_cast_008.png",//84

            DefPath+"ophanaimultra_die_001.png",//85
            DefPath+"ophanaimultra_die_002.png",
            DefPath+"ophanaimultra_die_003.png",
            DefPath+"ophanaimultra_die_004.png",
            DefPath+"ophanaimultra_die_005.png",
            DefPath+"ophanaimultra_die_006.png",
            DefPath+"ophanaimultra_die_007.png",
            DefPath+"ophanaimultra_die_008.png",
            DefPath+"ophanaimultra_die_009.png",
            DefPath+"ophanaimultra_die_010.png",
            DefPath+"ophanaimultra_die_011.png",
            DefPath+"ophanaimultra_die_012.png",//96



        };
		public class EyeEnemyBehavior : BraveBehaviour
		{
			//public List<GameObject> extantReticles = new List<GameObject>();

			public ImprovedAfterImage ownAfterImage;

			public float distortionMaxRadius = 30f;
			public float distortionDuration = 2f;
			public float distortionIntensity = 0.7f;
			public float distortionThickness = 0.1f;
			public RoomHandler m_StartRoom;
            public RoomHandler arena;
            public List<TrailRenderer> trails = new List<TrailRenderer>();

            public void Update()
			{
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
                foreach (TrailRenderer t in trails)
                {
                    if (t) { t.enabled = false; }
                }

				if (!base.aiActor.HasBeenEngaged)
				{
					CheckPlayerRoom();
				}
				if (base.aiActor && base.aiActor.healthHaver)
				{
                    if (base.aiActor.healthHaver.GetCurrentHealth() == base.aiActor.healthHaver.minimumHealth && Phase2Check != true)
                    {
                        Phase2Check = true;
                        StaticReferenceManager.DestroyAllEnemyProjectiles();
                        base.aiActor.behaviorSpeculator.InterruptAndDisable();
                        base.aiActor.StartCoroutine(DoTransition());
                    }
                    if (P2 == true && isPissed == false && eyes.Count == 0)
                    {
                        isPissed = true;
                        Exploder.DoDistortionWave(base.aiActor.sprite.WorldCenter, 4, 0.2f, 50, 0.5f);
                        AkSoundEngine.PostEvent("Play_VO_gorgun_gasp_01", this.aiActor.gameObject);
                        AkSoundEngine.PostEvent("Play_BOSS_dragun_spin_02", this.aiActor.gameObject);

                        this.AlterAttackProbability("agony", 1f);
                        this.AlterAttackProbability("LASERZ", 1);
                        this.AlterAttackProbability("endme", 0.1f);
                        this.AlterAttackProbability("Def2", 1.33f);
                        this.AlterAttackProbability("mihtrixslam_2", 0.5f);
                        this.AlterAttackProbability("phase_2teleport", 0f);
                    }
                }
			}
            private bool P2 = false;

            public List<GameObject> eyes = new List<GameObject>();
            public bool isPissed = false;

            private IEnumerator DoTransition()
            {

                Exploder.DoDistortionWave(base.aiActor.sprite.WorldCenter, 4, 0.2f, 50, 0.5f);
                base.aiActor.behaviorSpeculator.enabled = false;
                base.aiActor.aiAnimator.PlayUntilFinished("wizardshit", true, null, -1f, false);


                this.AlterAttackProbability("agony", 0.15f);
                this.AlterAttackProbability("LASERZ", 0);
                this.AlterAttackProbability("endme", 0.5f);
                this.AlterAttackProbability("Def2", 0.7f);
                this.AlterAttackProbability("mihtrixslam_2", 1.1f);
                this.AlterAttackProbability("phase_2teleport", 0.9f);
                this.AlterAttackProbability("supermegacoolattack", 1f);


                List<Vector2> pos = new List<Vector2>()
                {
                    new Vector2(2.5f, 1.5f),
                    new Vector2(-5, 1.5f),
                    new Vector2(2.5f, -3),
                    new Vector2(-5, -3),
                };
                AkSoundEngine.PostEvent("Play_BOSS_dragun_spin_02", this.aiActor.gameObject);
                float elaWait = 0f;
                while (elaWait < 1f)
                {
                    elaWait += BraveTime.DeltaTime;
                    yield return null;
                }
                AkSoundEngine.PostEvent("Play_BOSS_mineflayer_bong_01", this.aiActor.gameObject);
                elaWait = 0f;
                for (int i = 0; i < 4; i++)
                {
                    base.aiActor.StartCoroutine(SpawnEyeball(pos[i]));
                    while (elaWait < 0.5f)
                    {
                        elaWait += BraveTime.DeltaTime;
                        yield return null;
                    }
                    elaWait = 0;
                }
                elaWait = 0f;
                while (elaWait < 0.5f)
                {
                    elaWait += BraveTime.DeltaTime;
                    yield return null;
                }
                base.aiActor.behaviorSpeculator.enabled = true;
                base.aiActor.healthHaver.minimumHealth = 0;
                P2 = true;
                yield break;
            }
            public void AlterAttackProbability(string Name, float Probability)
            {
                for (int j = 0; j < base.aiActor.behaviorSpeculator.AttackBehaviors.Count; j++)
                {
                    if (base.behaviorSpeculator.AttackBehaviors[j] is AttackBehaviorGroup && base.behaviorSpeculator.AttackBehaviors[j] != null)
                    {
                        for (int i = 0; i < (base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup).AttackBehaviors.Count; i++)
                        {
                            AttackBehaviorGroup.AttackGroupItem attackGroupItem = (base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup).AttackBehaviors[i];
                            if ((base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup) != null && attackGroupItem.NickName.Contains(Name))
                            {
                                attackGroupItem.Probability = Probability;
                            }
                        }
                    }
                }
            }


            private IEnumerator SpawnEyeball(Vector2 pos)
            {
                float elaWait = 0f;
                StaticVFXStorage.HighPriestClapVFX.SpawnAtPosition(base.aiActor.sprite.WorldCenter + pos);
                while (elaWait < 0.875f)
                {
                    elaWait += BraveTime.DeltaTime;
                    yield return null;
                }
                AkSoundEngine.PostEvent("Play_NPC_magic_blessing_01", base.aiActor.gameObject);
                var pp = UnityEngine.Object.Instantiate<GameObject>(Ophanaim.EyeBallMinion, base.aiActor.sprite.WorldCenter + pos, Quaternion.identity, null).GetComponent<AdvancedBodyPartController>();
                eyes.Add(pp.gameObject);
                pp.gameObject.GetComponent<OphanaimMinionController>().DoPoof();
                pp.MainBody = base.aiActor;
                pp.gameObject.transform.parent = base.aiActor.transform;
                elaWait = 0;
                yield break;
            }


			public bool Phase2Check;
			private void CheckPlayerRoom()
			{
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom && GameManager.Instance.PrimaryPlayer.IsInCombat == true)
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

			public void Start()
			{
                arena = aiActor.GetAbsoluteParentRoom();


                //Important for not breaking basegame stuff!
				Phase2Check = false;

                base.aiActor.healthHaver.minimumHealth = base.aiActor.healthHaver.GetMaxHealth() * 0.6f;

                base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered; 
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");
				base.aiActor.HasBeenEngaged = false;
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					
				};
				base.aiActor.healthHaver.OnDeath += (obj) =>
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_OPHANAIM, true);
				};
			}

            private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
                //ownAfterImage
                if (clip.GetFrame(frameIdx).eventInfo == "disableImage")
				{
					ownAfterImage.spawnShadows = false;
                }

                if (clip.GetFrame(frameIdx).eventInfo == "enableImage")
                {
                    ownAfterImage.spawnShadows = true;
                }


                if (clip.GetFrame(frameIdx).eventInfo == "tp1")
                {
                    GlobalMessageRadio.BroadcastMessage("eye_shot_hide");
                }

                if (clip.GetFrame(frameIdx).eventInfo == "tp2")
                {
                    GlobalMessageRadio.BroadcastMessage("eye_shot_appear");
                }






                if (clip.GetFrame(frameIdx).eventInfo == "fuck_me")
				{
                    AkSoundEngine.PostEvent("Play_PortalOpen", base.gameObject);
                    AkSoundEngine.PostEvent("Play_BOSS_spacebaby_explode_01", base.gameObject);
                    float FlashTime = 0.25f;
					float FlashFadetime = 0.75f;
					Pixelator.Instance.FadeToColor(FlashFadetime, Color.white, true, FlashTime);
					StickyFrictionManager.Instance.RegisterCustomStickyFriction(0.15f, 1f, false, false); FlashTime = 0.1f;
				}
				if (clip.GetFrame(frameIdx).eventInfo == "DeathBoom")
				{
					float FlashTime = 1f;
					float FlashFadetime = 2f;
					Pixelator.Instance.FadeToColor(FlashFadetime, Color.white, true, FlashTime);
					StickyFrictionManager.Instance.RegisterCustomStickyFriction(0.15f, 1f, false, false); FlashTime = 0.1f;
					GameObject epicwin = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").GetComponent<BossFinalRogueDeathController>().DeathStarExplosionVFX);
					epicwin.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(base.aiActor.sprite.WorldCenter, tk2dBaseSprite.Anchor.LowerCenter);
					epicwin.transform.position = base.aiActor.sprite.WorldCenter.Quantize(0.0625f);
					epicwin.GetComponent<tk2dBaseSprite>().UpdateZDepth();
					for (int i = 0; i < StaticReferenceManager.AllGoops.Count; i++)
					{
						DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = StaticReferenceManager.AllGoops[i];
						deadlyDeadlyGoopManager.RemoveGoopCircle(base.aiActor.sprite.WorldCenter, 25);
					}

				}
				if (clip.GetFrame(frameIdx).eventInfo == "SUN")
				{
					for (int i = 0; i < StaticReferenceManager.AllGoops.Count; i++)
                    {
						DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = StaticReferenceManager.AllGoops[i];
						deadlyDeadlyGoopManager.RemoveGoopCircle(base.aiActor.sprite.WorldCenter, 40);
					}
					StaticReferenceManager.DestroyAllEnemyProjectiles();
					CellArea area = base.aiActor.ParentRoom.area;
					Vector2 Center = area.UnitCenter;
					//AkSoundEngine.PostEvent("Play_Baboom", base.gameObject);
					Vector2 b = base.aiActor.specRigidbody.UnitBottomCenter - base.aiActor.transform.position.XY();
					IntVector2? CentreCell = Center.ToIntVector2();
					base.aiActor.transform.position = Pathfinder.GetClearanceOffset(CentreCell.Value - new IntVector2(2 , 0), base.aiActor.Clearance).WithY((float)CentreCell.Value.y) - b;
					base.aiActor.specRigidbody.Reinitialize();
					float FlashTime = 0.125f;
					float FlashFadetime = 0.375f;
					Pixelator.Instance.FadeToColor(FlashFadetime, Color.white, true, FlashTime);
					StickyFrictionManager.Instance.RegisterCustomStickyFriction(0.15f, 1f, false, false); FlashTime = 0.1f;
				}
			}
			private IEnumerator unmatchedpowerofthesun()
			{
			
				Material targetMaterial = base.aiAnimator.sprite.renderer.material;
				float ela = 0f;
				float dura = 1f;
				while (ela < dura)
				{
					ela += BraveTime.DeltaTime;
					base.aiActor.sprite.renderer.material.SetColor("_EmissiveColor", new Color32(255, 210, 178, 255));
					base.aiActor.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
					base.aiActor.sprite.renderer.material.SetFloat("_EmissivePower", 100+(ela*400));
					base.aiActor.sprite.renderer.material.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
					yield return null;
				}
				yield return new WaitForSeconds(10f);
				ela = 0f;
				dura = 4f;
				while (ela < dura)
				{
					base.aiActor.sprite.renderer.material.SetColor("_EmissiveColor", new Color32(255, 210, 178, 255));
					base.aiActor.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
					base.aiActor.sprite.renderer.material.SetFloat("_EmissivePower", 500 - (ela * 100));
					base.aiActor.sprite.renderer.material.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
					yield return null;
				}
				Pixelator.Instance.DeregisterAdditionalRenderPass(Sun);
				yield break;
			}

		}
		public static Material Sun = new Material(ShaderCache.Acquire("Brave/LitCutoutUber"));

        public class SpecialAttack : BasicAttack
        {
            public override bool IsHard
            {
                get
                {
                    return true;
                }
            }
            public override float LifetimeMax
            {
                get
                {
                    return 105;
                }
            }
        }
        public class BasicAttack : Script
        {
            public virtual bool IsHard
            {
                get
                {
                    return false;
                }
            }

            public virtual float LifetimeMax
            {
                get
                {
                    return 105;
                }
            }

            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);

                //m_ENM_bulletking_shot_01
                float h = BraveUtility.RandomBool() == true ? -120 : 120;
				float r = BraveUtility.RandomAngle();
                for (int i = 0; i < 6; i++)
                {
                    this.Fire(new Direction(r +(i * 60), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new Gordo(this, h));
					if (IsHard == true)
					{
                        this.Fire(new Direction(r + (i * 60), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new Gordo(this, -h));
                    }
                }
                yield break;
            }
            public class Gordo : Bullet
            {
                public Gordo(BasicAttack p, float tMius = 0) : base("bigBullet", false, false, false)
                {
                    t = tMius;
					parent = p;
                }
                public override IEnumerator Top()
                {
                    yield return base.Wait(20);
                    base.ChangeSpeed(new Speed(20f, SpeedType.Absolute), 120);
                    base.ChangeDirection(new Brave.BulletScript.Direction(t, Brave.BulletScript.DirectionType.Relative), 105);

                    int i = 0;
                    while (this.Projectile)
                    {
                        i++;
                        yield return base.Wait(1);
                        if (i == 4)
                        {
                            i = 0;
                            this.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Dissipate(parent.LifetimeMax));
                        }
                        yield return null;
                    }
                    yield break;
                }
                private float t;
                private BasicAttack parent;
            }
            public class Dissipate : Bullet
            {
                public Dissipate(float p) : base("frogger", false, false, false)
                {
                    LifetimeMax = p;
                }
                public override IEnumerator Top()
                {
                    yield return base.Wait((Mathf.Max(LifetimeMax, BraveUtility.RandomAngle()) / 1.75f));
                    base.Vanish(false);
                    yield break;
                }
				private float LifetimeMax;
            }
        }


        public class FireSplit720 : FireSplit
        {
            public override float KillDelay
            {
                get
                {
                    return 540;
                }
            }
        }
        public class FireSplit540 : FireSplit
        {
            public override float KillDelay
            {
                get
                {
                    return 420;
                }
            }
        }

        public class FireSplit : Script
        {
            public virtual float KillDelay
            {
                get
                {
                    return 780;
                }
            }


            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);

                float pp = this.AimDirection;
                base.Fire(new Direction(75, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new RotatingBullet(base.Position, true, 60 + pp, 0.0833f, KillDelay));
                base.Fire(new Direction(165, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new RotatingBullet(base.Position, true, 150 + pp, 0.133f, KillDelay));
                base.Fire(new Direction(-165, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new RotatingBullet(base.Position, false, -150 + pp, 0.133f, KillDelay));
                base.Fire(new Direction(-75, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new RotatingBullet(base.Position, false, -60 + pp, 0.0833f, KillDelay));


                AIBeamShooter2[] beams = this.BulletBank.aiActor.GetComponents<AIBeamShooter2>();
                yield return this.Wait(90);
                GlobalMessageRadio.BroadcastMessage("eye_gun_laser");
                yield return this.Wait(30);

                int i = 0;
				bool b = false;
                while (beams != null || beams.Length > 0)
                {
                    if (b == false)
                    {
                        b = !b;

                        foreach (AIBeamShooter2 beam in beams)
                        {
                            if (beam && beam.LaserBeam)
                            {
                                Vector2 overridePosition = beam.LaserBeam.Origin + beam.LaserBeam.Direction.normalized * beam.MaxBeamLength;

                                Vector2 vector = beam.LaserBeam.Origin;

                                Vector2 vector2 = new Vector2();
                                Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;
                                int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.EnemyHitBox, CollisionLayer.PlayerHitBox);
                                RaycastResult raycastResult2;

                                Vector2 Point = MathToolbox.GetUnitOnCircle(beam.LaserBeam.Direction.ToAngle(), 1);
                                if (PhysicsEngine.Instance.Raycast(beam.LaserBeam.Origin, Point, 1000, out raycastResult2, true, false, rayMask2, null, false, rigidbodyExcluder, base.BulletBank.aiActor.specRigidbody))
                                {
                                    vector2 = raycastResult2.Contact;
                                }
                                RaycastResult.Pool.Free(ref raycastResult2);
                                int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector, vector2)), 1);
                                num2 /= 2;
                                for (int e = 0; e < num2; e++)
                                {
                                    float t = (float)e / (float)num2;
                                    Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() + 90, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new FireSplit.FireFire());
                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() - 90, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new FireSplit.FireFire());

                                }
                            }
                        }
                   


                    }
                    i++;
                    yield return this.Wait(1);
                }
               
                i++;
                yield break;

            }
            public class RotatingBullet : Bullet
            {
                public RotatingBullet(Vector2 centerPoint, bool speeen, float startAngle, float spinspeed, float Killdelay) : base("bigBullet", false, false, false)
                {
                    this.centerPoint = centerPoint;
                    this.yesToSpeenOneWay = speeen;
                    this.startAngle = startAngle;
                    this.SpinSpeed = spinspeed;
                    this.kd = Killdelay;
                }
                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    float radius = Vector2.Distance(this.centerPoint, base.Position);
                    float speed = this.Speed;
                    float spinAngle = this.startAngle;
                    float spinSpeed = 0f;
                    for (int i = 0; i < 600; i++)
                    {
                        //speed += 0.08333f;
                        if (i % 6 == 0)
                        {
                            this.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Dissipate(kd));
                        }
                        radius += speed / 60f;
                        if (yesToSpeenOneWay == true)
                        {
                            spinSpeed -= SpinSpeed;
                        }
                        else
                        {
                            spinSpeed += SpinSpeed;

                        }
                        spinAngle += spinSpeed / 60f;
                        base.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }
                public Vector2 centerPoint;
                public bool yesToSpeenOneWay;
                public float startAngle;
                public float SpinSpeed;
                public float kd;

            }


            public class Dissipate : Bullet
            {
                private float killDelay;
                public Dissipate(float kd) : base("frogger", false, false, false)
                {
                    killDelay = kd;
                }
                public override IEnumerator Top()
                {
                    yield return base.Wait(killDelay);
                    base.Vanish(false);
                    yield break;
                }
            }

            public class FireFire : Bullet
            {

                public FireFire() : base("suck", false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    this.Projectile.IgnoreTileCollisionsFor(90f);
                    yield return this.Wait(30);
                    base.ChangeSpeed(new Speed(9f, SpeedType.Absolute), 120);
                    yield return this.Wait(300);
                    base.Vanish(false);
                    yield break;
                }
            }
        }

        public class FireSplitFinale : Script
        {

            public override void OnForceEnded()
            {
                base.OnForceEnded();
            }

            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);
                //float pp = this.AimDirection;
                //base.Fire(new Direction(75, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new RotatingBullet(base.Position, true, 60 + pp, 0.1f));
                //base.Fire(new Direction(165, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new RotatingBullet(base.Position, true, 150 + pp, 0.133f));
                //base.Fire(new Direction(-165, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new RotatingBullet(base.Position, false, -150 + pp, 0.133f));
                //base.Fire(new Direction(-75, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new RotatingBullet(base.Position, false, -60 + pp, 0.1f));

                AIBeamShooter2[] beams = this.BulletBank.aiActor.GetComponents<AIBeamShooter2>();
                yield return this.Wait(120);
                int i = 0;
                while (beams != null || beams.Length > 0)
                {
                    if (i % 40 == 0)
                    {
                        GlobalMessageRadio.BroadcastMessage("eye_gun_simple");
                    }
                    if (i % 80 == 0)
                    {
                        GlobalMessageRadio.BroadcastMessage("eye_gun_predict");

                        foreach (AIBeamShooter2 beam in beams)
                        {
                            if (beam && beam.LaserBeam)
                            {
                                Vector2 overridePosition = beam.LaserBeam.Origin + beam.LaserBeam.Direction.normalized * beam.MaxBeamLength;

                                Vector2 vector = beam.LaserBeam.Origin;

                                Vector2 vector2 = new Vector2();
                                Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;
                                int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.EnemyHitBox, CollisionLayer.PlayerHitBox);
                                RaycastResult raycastResult2;

                                Vector2 Point = MathToolbox.GetUnitOnCircle(beam.LaserBeam.Direction.ToAngle(), 1);
                                if (PhysicsEngine.Instance.Raycast(beam.LaserBeam.Origin, Point, 1000, out raycastResult2, true, false, rayMask2, null, false, rigidbodyExcluder, base.BulletBank.aiActor.specRigidbody))
                                {
                                    vector2 = raycastResult2.Contact;
                                }
                                RaycastResult.Pool.Free(ref raycastResult2);
                                int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector, vector2)), 1);
                                num2 /= 3;
                                for (int e = 0; e < num2; e++)
                                {
                                    float t = (float)e / (float)num2;
                                    Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() + 90, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new FireSplit.FireFire());
                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() - 90, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new FireSplit.FireFire());
                                }
                            }
                        }
                    }
                    i++;
                    yield return this.Wait(1);
                }
                i++;
                yield break;

            }

            public class RotatingBullet : Bullet
            {
                public RotatingBullet(Vector2 centerPoint, bool speeen, float startAngle, float spinspeed) : base("bigBullet", false, false, false)
                {
                    this.centerPoint = centerPoint;
                    this.yesToSpeenOneWay = speeen;
                    this.startAngle = startAngle;
                    this.SpinSpeed = spinspeed;
                }
                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    float radius = Vector2.Distance(this.centerPoint, base.Position);
                    float speed = this.Speed;
                    float spinAngle = this.startAngle;
                    float spinSpeed = 0f;
                    for (int i = 0; i < 600; i++)
                    {
                        //speed += 0.08333f;
                        if (i % 6 == 0)
                        {
                            this.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Dissipate());
                        }
                        radius += speed / 60f;
                        if (yesToSpeenOneWay == true)
                        {
                            spinSpeed -= SpinSpeed;
                        }
                        else
                        {
                            spinSpeed += SpinSpeed;

                        }
                        spinAngle += spinSpeed / 60f;
                        base.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }
                public Vector2 centerPoint;
                public bool yesToSpeenOneWay;
                public float startAngle;
                public float SpinSpeed;
            }
            public class Dissipate : Bullet
            {
                public Dissipate() : base("frogger", false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    yield return base.Wait(300);
                    base.Vanish(false);
                    yield break;
                }
            }

            public class FireFire : Bullet
            {

                public FireFire() : base("suck", false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    this.Projectile.IgnoreTileCollisionsFor(300f);
                    yield return this.Wait(30);
                    base.ChangeSpeed(new Speed(11f, SpeedType.Absolute), 120);
                    yield return this.Wait(300);
                    base.Vanish(false);
                    yield break;
                }
            }
        }


        public class SweepAttack : Script
        {

            public bool fire;
            public float angle;

            public override IEnumerator Top()
            {

                float dur1 = 0;
                float dur2 = 0;
                this.EndOnBlank = false;
                fire = false;
                {
                    Vector2 vector2 = new Vector2();
                    Vector2 vector = this.Position;
                    int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.BulletBreakable);
                    var cast = RaycastToolbox.ReturnRaycast(this.Position, Vector2.right, rayMask, 1000, this.BulletBank.aiActor.specRigidbody);
                    vector2 = cast.Contact;
                    int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector, vector2)), 1);
                    num2 /= 3;
                    dur1 = 7.5f * num2;
                    for (int e = 0; e < num2; e++)
                    {
                        float t = (float)e / (float)num2;
                        Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
                        this.StartTask(this.SpawnFucker(vector3, 7.5f * e));
                    }
                }
                {
                    Vector2 vector2 = new Vector2();
                    Vector2 vector = this.Position;
                    int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.BulletBreakable);
                    var cast = RaycastToolbox.ReturnRaycast(this.Position,Vector2.left, rayMask, 1000, this.BulletBank.aiActor.specRigidbody);
                    vector2 = cast.Contact;
                    int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector, vector2)), 1);
                    num2 /= 3;

                    dur2 = 7.5f * num2;

                    for (int e = 0; e < num2; e++)
                    {
                        float t = (float)e / (float)num2;
                        Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
                        this.StartTask(this.SpawnFucker(vector3, 7.5f * e));
                    }
                }
                yield return this.Wait(Mathf.Max(dur1, dur2) + 20);
                fire = true;
                
                for (int e = 1; e < 51; e++)
                {
                    if (e % 10 == 0)
                    {
                        GlobalMessageRadio.BroadcastMessage("eye_gun_laser");
                    }
                    this.EndOnBlank = true;
                    float spare = 36;
                    float Angle = this.AimDirection;
                    if (Angle.IsBetweenRange(Vector2.left.ToAngle(), Vector2.left.ToAngle() + spare)) { Angle = Vector2.left.ToAngle() + spare; }
                    if (Angle.IsBetweenRange(Vector2.left.ToAngle() - spare , Vector2.left.ToAngle())) { Angle = Vector2.left.ToAngle() - spare; }
                    if (Angle.IsBetweenRange(Vector2.right.ToAngle(), Vector2.right.ToAngle() + spare)) { Angle = Vector2.right.ToAngle() + spare; }
                    if (Angle.IsBetweenRange(Vector2.right.ToAngle() - spare, Vector2.right.ToAngle())) { Angle = Vector2.right.ToAngle() - spare; }
                    angle = Angle;
                    yield return this.Wait(10);

                }
                yield break;
            }


            public IEnumerator SpawnFucker(Vector2 position, float delay)
            {
                yield return this.Wait(delay);
                StaticVFXStorage.HighPriestClapVFX.SpawnAtPosition(position);

                yield return this.Wait(60);
                //Play_Stomp
                base.PostWwiseEvent("Play_BOSS_agunim_deflect_01", null);//m_BOSS_agunim_deflect_01
                this.Fire(Offset.OverridePosition(position), new Direction(0, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new SummonerBall(this));
                yield break;
            }
            public class SummonerBall : Bullet
            {
                public SweepAttack parent;
                public SummonerBall(SweepAttack p ) : base("big", false, false, false)
                {
                    parent = p;
                }
                public override IEnumerator Top()
                {
                    int i = 0;
                    while (parent.IsEnded == false)
                    {
                        if (parent.fire == true)
                        {
                            if (i % 40 == 0)
                            {
                                base.PostWwiseEvent("Play_BOSS_doormimic_flame_01");
                                for (int q = 0; q < 2; q++)
                                {
                                    this.Fire(new Direction(parent.angle + (q*180), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(2), new SpeedChangingBullet("frogger", 6, 120));
                                }
                            }
                        }
                        i++;
                        yield return this.Wait(1);

                    }
                    yield return this.Wait(BraveUtility.RandomAngle() / 3);
                    base.Vanish();
                    i++;
                    yield break;
                }
            }

        }

        public class FireBurst : Script
		{
			public override IEnumerator Top()
			{
				AIBeamShooter2[] beams = this.BulletBank.aiActor.GetComponents<AIBeamShooter2>();
				yield return this.Wait(60);
				int i = 0;
				while (beams != null || beams.Length > 0)
				{
					foreach (AIBeamShooter2 beam in beams)
					{
                        if (beam && beam.LaserBeam )
						{
                            GlobalMessageRadio.BroadcastMessage("eye_gun_predict");

                            List<SpeculativeRigidbody> r = new List<SpeculativeRigidbody>();
                            for (int h = 0; h < this.BulletBank.aiActor.gameObject.transform.childCount; h++)
                            {
                                Transform t = this.BulletBank.aiActor.gameObject.transform.GetChild(h);
                                if (t != null)
                                {
                                    var spc = t.gameObject.GetComponent<SpeculativeRigidbody>();
                                    if (spc != null)
                                    {
                                        r.Add(spc);
                                    }
                                }
                            }
                            beam.LaserBeam.IgnoreRigidbodes = r;
                            if (i % 60 == 0)
                           {
                                float h = BraveUtility.RandomBool() == true ? -60 : 60;
                                float a = this.AimDirection;
                                base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
                                LinkedList<BasicBeamController.BeamBone> linkedList = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", beam.m_laserBeam);
                                LinkedListNode<BasicBeamController.BeamBone> last = linkedList.Last;
                                Vector2 bonePosition = beam.m_laserBeam.GetBonePosition(last.Value);
                                for (int l = 0; l < 6; l++)
                                {
                                    this.Fire(Offset.OverridePosition(bonePosition), new Direction((60 * l) + a, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(3), new FireBeam(h));
                                    this.Fire(new Direction((60 * l) + a, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(3), new FireBeam(-h));
                                }
                           }

                           
                        }
					}
					i++;
					yield return this.Wait(1);
				}
				yield break;

			}
			public class FireBeam : Bullet
			{
                private float t;

                public FireBeam(float th) : base("frogger", false, false, false)
				{
					t = th;
				}
				public override IEnumerator Top()
				{
                    this.Projectile.IgnoreTileCollisionsFor(300f);
                    yield return this.Wait(30);
                    base.ChangeSpeed(new Speed(8f, SpeedType.Absolute), 120);
                    base.ChangeDirection(new Brave.BulletScript.Direction(t, Brave.BulletScript.DirectionType.Relative), 135); 
					yield return this.Wait(300);
					base.Vanish(false);
					yield break;
				}
			}
		}


        public class TeleportAttack : BasicAttack
        {
            public override bool IsHard
            {
                get
                {
                    return false;
                }
            }
            public override float LifetimeMax
            {
                get
                {
                    return 180;
                }
            }
        }

        public class Teleport2 : Script
        {
           

            public override IEnumerator Top()
            {
                //base.PostWwiseEvent("Play_ENM_gunknight_shockwave_01", null);

                base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);
                GlobalMessageRadio.BroadcastMessage("eye_gun_laser");
                float h = BraveUtility.RandomBool() == true ? -120 : 120;
                float E = BraveUtility.RandomAngle();
                bool b = BraveUtility.RandomBool();

                for (int n = 0; n < 16; n++)
                {
                    base.Fire(new Direction((22.5f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new BlastAttack.RotatingBullet(base.Position, !b, (22.5f * n), 0.05f));
                }
                for (int n = 0; n < 16; n++)
                {
                    base.Fire(new Direction((22.5f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6.5f, SpeedType.Absolute), new BlastAttack.RotatingBullet(base.Position, b, (22.5f * n), 0.065f));
                }
                for (int n = 0; n < 16; n++)
                {
                    base.Fire(new Direction((22.5f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new BlastAttack.RotatingBullet(base.Position, !b, (22.5f * n), 0.8f));
                }

                yield break;
            }
        }




        public class BlastAttack : Script
        {
            public virtual bool IsHard
            {
                get
                {
                    return false;
                }
            }

            public override IEnumerator Top()
            {
                //base.PostWwiseEvent("Play_ENM_gunknight_shockwave_01", null);

                GlobalMessageRadio.BroadcastMessage("eye_gun_laser");

                base.PostWwiseEvent("Play_BigSlam", null);
                base.PostWwiseEvent("Play_BigSlam", null);

                float h = BraveUtility.RandomBool() == true ? -120 : 120;
                float E = BraveUtility.RandomAngle();
				bool b = BraveUtility.RandomBool();

                for (int n = 0; n < 24; n++)
				{
					base.Fire(new Direction((15f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new BlastAttack.RotatingBullet(base.Position, !b, (15f * n), 0.05f));
					base.Fire(new Direction((15f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new BlastAttack.RotatingBullet(base.Position, b, (15f * n), 0.0625f));
                    base.Fire(new Direction((15f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new BlastAttack.RotatingBullet(base.Position, !b, (15f * n), 0.075f));
                    base.Fire(new Direction((15f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new BlastAttack.RotatingBullet(base.Position, b, (15f * n), 0.0875f));

                }
                yield break;
            }
            public class RotatingBullet : Bullet
            {
                public RotatingBullet(Vector2 centerPoint, bool speeen, float startAngle, float spinspeed) : base("frogger", false, false, false)
                {
                    this.centerPoint = centerPoint;
                    this.yesToSpeenOneWay = speeen;
                    this.startAngle = startAngle;
                    this.SpinSpeed = spinspeed;
                }
                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    float radius = Vector2.Distance(this.centerPoint, base.Position);
                    float speed = this.Speed;
                    float spinAngle = this.startAngle;
                    float spinSpeed = 0f;
                    for (int i = 0; i < 600; i++)
                    {
                        //speed += 0.08333f;
                        radius += speed / 60f;
                        if (yesToSpeenOneWay == true)
                        {
                            spinSpeed -= SpinSpeed;
                        }
                        else
                        {
                            spinSpeed += SpinSpeed;

                        }
                        spinAngle += spinSpeed / 60f;
                        base.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }
                public Vector2 centerPoint;
                public bool yesToSpeenOneWay;
                public float startAngle;
                public float SpinSpeed;
            }
        }


        public class MithrixSlamTwo : Script
        {
            public override IEnumerator Top()
            {              
                for (int q = 0; q < 6; q++)
                {
                    base.PostWwiseEvent("Play_ENM_CannonArmor_Charge_01", null);
                    bool b = true;
                    float Dir =BraveUtility.RandomAngle();
                    float helpme = UnityEngine.Random.Range(180, -180);
                    float M = UnityEngine.Random.value < 0.5f ? 135 : -135;
                    for (int i = 0; i < 10; i++)
                    {
                       
                        base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob((36 * i) + Dir, this, M, 1f, b));
                        b = !b;
                    }
                    GlobalMessageRadio.BroadcastMessage("eye_gun_simple");

                    yield return this.Wait(75);
                }
                yield return this.Wait(120);
                yield break;
            }




            private IEnumerator QuickscopeNoob(float aimDir, MithrixSlamTwo parent, float rotSet, float chargeTime = 0.5f, bool PlaysAudio = false)
            {

                GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
                tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                component2.transform.position = this.Position;
                component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
                component2.dimensions = new Vector2(1f, 1f);
                component2.UpdateZDepth();
                component2.HeightOffGround = -2;


                Color laser = new Color(1, 0.85f, 0.7f);
                component2.sprite.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
                component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);


                float elapsed = 0;
                float Time = chargeTime;
                while (elapsed < Time)
                {
                    float t = (float)elapsed / (float)Time;

                    if (parent.IsEnded || parent.Destroyed)
                    {
                        UnityEngine.Object.Destroy(component2.gameObject);
                        yield break;
                    }
                    if (component2 != null)
                    {
                        float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
                        float Q = Mathf.Lerp(0, rotSet, throne1);
                        component2.transform.position = this.Position;
                        component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (25 * t));
                        component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (5 * t));
                        component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir + Q);
                        component2.HeightOffGround = -2;
                        component2.renderer.gameObject.layer = 22;
                        component2.dimensions = new Vector2(Mathf.Lerp(1, 750, throne1), 1f);
                        component2.UpdateZDepth();
                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }

                elapsed = 0;
                Time = 0.75f;
                StaticVFXStorage.HighPriestClapVFX.SpawnAtPosition(this.Position);
                while (elapsed < Time)
                {
                    if (parent.IsEnded || parent.Destroyed)
                    {
                        UnityEngine.Object.Destroy(component2.gameObject);
                        yield break;
                    }
                    if (component2 != null)
                    {
                        component2.transform.position = this.Position;
                        component2.dimensions = new Vector2(1000f, 1f);
                        component2.HeightOffGround = -2;
                        component2.renderer.gameObject.layer = 22;
                        component2.UpdateZDepth();

                        bool enabled = elapsed % 0.25f > 0.125f;
                        component2.sprite.renderer.enabled = enabled;

                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                UnityEngine.Object.Destroy(component2.gameObject);
                //base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
                if (PlaysAudio == true) { base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null); }

                base.Fire(new Direction(aimDir + rotSet, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(50f, SpeedType.Absolute), new Spawner());
                yield break;
            }

            public class Spawner : Bullet
            {
                public Spawner() : base("bigBullet", false, false, false)
                {

                }

                public override IEnumerator Top()
                {
                    for (int i = 0; i < 200; i++)
                    {
                        base.Fire(new Direction(UnityEngine.Random.Range(140, 200), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(0f, SpeedType.Absolute), new MithrixSlamTwo.BouncyFlame(80 - i));
                        yield return this.Wait(1f);
                    }
                    yield break;
                }
            }
            public class BouncyFlame : Bullet
            {
                public BouncyFlame(float timetilldie) : base("frogger", false, false, false)
                {
                    TimeTillDeath = timetilldie;
                }

                public override IEnumerator Top()
                {
                    this.ManualControl = true;
                    Vector2 truePosition = this.Position;

                    for (int i = 0; i < 80 - TimeTillDeath; i++)
                    {
                        this.UpdateVelocity();
                        truePosition += this.Velocity / 20f;
                        this.Position = truePosition + new Vector2(0f, Mathf.Sin((float)this.Tick / 30f / 0.75f * 3.14159274f) * 1.5f);
                        yield return this.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }
                private float TimeTillDeath;
            }

        }


        public class MithrixSlamOmega : Script
        {
            public override IEnumerator Top()
            {
                for (int q = 0; q < 4; q++)
                {
                    base.PostWwiseEvent("Play_ENM_CannonArmor_Charge_01", null);
                    bool b = true;
                    float Dir = BraveUtility.RandomAngle();
                    float M = UnityEngine.Random.value < 0.5f ? 75 : -75;
                    bool masd = BraveUtility.RandomBool();
                    for (int i = 0; i < 6; i++)
                    {
                        base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob((60 * i) + Dir, this, M, 0.5f, b, masd));
                        b = !b;
                    }
                    yield return this.Wait(75);
                    GlobalMessageRadio.BroadcastMessage("eye_gun_predict");
                    yield return this.Wait(75);
                }
                yield return this.Wait(120);
                yield break;
            }




            private IEnumerator QuickscopeNoob(float aimDir, MithrixSlamOmega parent, float rotSet, float chargeTime = 0.5f, bool PlaysAudio = false, bool fcdfa = true)
            {

                GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
                tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                component2.transform.position = this.Position;
                component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
                component2.dimensions = new Vector2(1f, 1f);
                component2.UpdateZDepth();
                component2.HeightOffGround = -2;


                Color laser = new Color(1, 0.85f, 0.7f);
                component2.sprite.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
                component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);

                float elapsed = 0;
                float Time = chargeTime;
                while (elapsed < Time)
                {
                    float t = (float)elapsed / (float)Time;

                    if (parent.IsEnded || parent.Destroyed)
                    {
                        UnityEngine.Object.Destroy(component2.gameObject);
                        yield break;
                    }
                    if (component2 != null)
                    {
                        float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
                        float Q = Mathf.Lerp(0, rotSet, throne1);
                        component2.transform.position = this.Position;
                        component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (25 * t));
                        component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (5 * t));
                        component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir + Q);
                        component2.HeightOffGround = -2;
                        component2.renderer.gameObject.layer = 22;
                        component2.dimensions = new Vector2(Mathf.Lerp(1, 750, throne1), 1f);
                        component2.UpdateZDepth();
                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }

                elapsed = 0;
                Time = 0.75f;
                StaticVFXStorage.HighPriestClapVFX.SpawnAtPosition(this.Position);
                while (elapsed < Time)
                {
                    if (parent.IsEnded || parent.Destroyed)
                    {
                        UnityEngine.Object.Destroy(component2.gameObject);
                        yield break;
                    }
                    if (component2 != null)
                    {
                        component2.transform.position = this.Position;
                        component2.dimensions = new Vector2(1000f, 1f);
                        component2.HeightOffGround = -2;
                        component2.renderer.gameObject.layer = 22;
                        component2.UpdateZDepth();

                        bool enabled = elapsed % 0.25f > 0.125f;
                        component2.sprite.renderer.enabled = enabled;

                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                UnityEngine.Object.Destroy(component2.gameObject);
                if (PlaysAudio == true) { base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null); }

                base.Fire(new Direction(aimDir + rotSet, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(50f, SpeedType.Absolute), new Spawner(fcdfa));
                yield break;
            }

            public class Spawner : Bullet
            {
                public Spawner(bool h) : base("bigBullet", false, false, false)
                {
                    b = h;
                }

                public override IEnumerator Top()
                {
                    for (int i = 0; i < 120; i++)
                    {
                        base.Fire(new Direction(b == true ? 90 : -90, Brave.BulletScript.DirectionType.Relative, -1f), new Speed(0f, SpeedType.Absolute), new MithrixSlamOmega.BouncyFlame(45));
                        yield return this.Wait(1f);
                    }
                    yield break;
                }
                private bool b;
            }
            public class BouncyFlame : Bullet
            {
                private float wait;
                public BouncyFlame(float t) : base("frogger", false, false, false)
                {
                    wait = t;
                }

                public override IEnumerator Top()
                {
                    yield return this.Wait(wait);
                    this.ChangeSpeed(new Brave.BulletScript.Speed(15, SpeedType.Absolute), 120);
                    yield break;
                }
            }
        }




        //
        public class SomeBigAttackThatGetsReplacedInPhase2 : Script
		{
			public override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				base.PostWwiseEvent("Play_EyeRoar", null);
				float RNG = UnityEngine.Random.Range(0, 60);
				Exploder.DoDistortionWave(base.BulletBank.sprite.WorldCenter, 2, 0.05f, 10, 0.7f);
				yield return base.Wait(15);
				for (int E = 0; E < 6; E++)
                {
					base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
					for (int n = 0; n < 12; n++)
					{
						base.Fire(new Direction((30 * n)+ (E*2), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SomeBigAttackThatGetsReplacedInPhase2.Break(base.Position, false, (30 * n), 0.08f));
					}
					yield return base.Wait(15);
					base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
					for (int n = 0; n < 12; n++)
					{
						base.Fire(new Direction((30 * n) + (E * 2), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SomeBigAttackThatGetsReplacedInPhase2.Flames1());
					}
					yield return base.Wait(15);
					base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
					for (int n = 0; n < 12; n++)
					{
						base.Fire(new Direction((30 * n) + (E * 2), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SomeBigAttackThatGetsReplacedInPhase2.Break(base.Position, true, (30 * n), 0.08f));
					}
					yield return base.Wait(15);
					base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
					for (int n = 0; n < 12; n++)
					{
						base.Fire(new Direction((30 * n) + (E * 2), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SomeBigAttackThatGetsReplacedInPhase2.Flames1());
					}
					yield return base.Wait(15);
				}

				
				yield break;
			}

			public class Break : Bullet
			{
				public Break(Vector2 centerPoint, bool speeen, float startAngle, float spinspeed) : base("frogger", false, false, false)
				{
					this.centerPoint = centerPoint;
					this.yesToSpeenOneWay = speeen;
					this.startAngle = startAngle;
					this.SpinSpeed = spinspeed;
				}
				public override IEnumerator Top()
				{
					base.ManualControl = true;
					float radius = Vector2.Distance(this.centerPoint, base.Position);
					float speed = this.Speed;
					float spinAngle = this.startAngle;
					float spinSpeed = 0f;
					for (int i = 0; i < 600; i++)
					{
						//speed += 0.08333f;
						radius += speed / 60f;
						if (yesToSpeenOneWay == true)
						{
							spinSpeed -= SpinSpeed;
						}
						else
						{
							spinSpeed += SpinSpeed;

						}
						spinAngle += spinSpeed / 60f;
						base.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}
				public Vector2 centerPoint;
				public bool yesToSpeenOneWay;
				public float startAngle;
				public float SpinSpeed;
			}
			public class Flames1 : Bullet
			{
				public Flames1() : base("frogger", false, false, false)
				{

				}

				public override IEnumerator Top()
				{
					yield return base.Wait(900);
					yield break;
				}
			}
			public class SnakeBullet : Bullet
			{
				public SnakeBullet(int delay) : base("teeth_football", false, false, false)
				{
					this.delay = delay;
				}

				public override IEnumerator Top()
				{
					yield return base.Wait(this.delay);
					base.ChangeSpeed(new Speed(9f, SpeedType.Absolute), 0);
					yield break;
				}
				private int delay;
			}
		}

        public class THESUN : Script
        {
            public override IEnumerator Top()
            {
                yield return base.Wait(60);

                var room = this.BulletBank.aiActor.gameObject.GetComponent<EyeEnemyBehavior>().arena;
                var roomCenter = room.GetCenterCell();
                var star = StaticVFXStorage.MourningStarVFXController.SpawnMourningStar(roomCenter.ToCenterVector2());
                var bank = star.gameObject.AddComponent<AIBulletBank>();
                bank.Bullets = new List<AIBulletBank.Entry>()
                {
                    EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"),
                    suckLessEntry
                };

                AdditionalBraveLight braveLight = star.gameObject.AddComponent<AdditionalBraveLight>();
                braveLight.transform.position = star.sprite.WorldCenter;
                braveLight.LightColor = new Color(1, 0.82f, 0.625f);
                braveLight.LightIntensity = 5f;
                braveLight.LightRadius = 0f;


                yield return base.Wait(20);
                var orbiter = GenerateOrbit(star.gameObject, 12);
                var orbiter2 = GenerateOrbitMinor(star.gameObject, -12);
                var orbiter3 = GenerateOrbitMinor(star.gameObject, -12);

                //base.PostWwiseEvent("Play_Burn", null);
                float m = 1;
                starObject = star.gameObject;
                star.OnBeamUpdate += (obj1, obj2) =>
                {
                    float gjkgj = obj2 - 0.33f;
                    float t = Mathf.Min(1, (gjkgj * 0.2f));
                    Vector3 centerPoint = star.transform.position;
                    float a = (this.BulletManager.PlayerPosition() - new Vector2(centerPoint.x, centerPoint.y)).ToAngle();
                    float playerDist = (this.BulletManager.PlayerPosition() - star.transform.PositionVector2()).magnitude;
                    float dist = (Mathf.Min(10.5f, playerDist) * Time.deltaTime);
                    dist *= t;
                    dist *= m;
                    star.transform.position = centerPoint + BraveMathCollege.DegreesToVector(a, 0.6f * dist).ToVector3ZisY();
                };
                yield return base.Wait(30);
                for (int i = 0; i < 1350; i++)
                {
                    float t = (float)i / (float)750;
                    orbiter.BulletMaxRadius = (float)Mathf.Lerp(0, 12, (float)t);
                    orbiter.BulletMinRadius = (float)Mathf.Lerp(0, 1, (float)t);
                    orbiter.Update();
                    if (i == 1050){ base.PostWwiseEvent("Play_ENM_CannonArmor_Charge_01", null); this.StartTask(DoBlast(braveLight)); }
                    if (i > 1050)
                    {
                        float ads = i - 1050;
                        float t2 = (float)ads / (float)240;
                        m = Mathf.Lerp(1, 0, MathToolbox.SinLerpTValue(t2));
                    }
                    if (i == 300) { base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null); }
                    if (i >= 300)
                    {
                        float ads = i - 300;
                        float t2 = (float)ads/ (float)750;
                        orbiter2.BulletMaxRadius = (float)Mathf.Lerp(0, 9, (float)t2);
                        orbiter2.BulletMinRadius = (float)Mathf.Lerp(0, 1, (float)t2);
                        orbiter3.BulletMaxRadius = (float)Mathf.Lerp(0, 3.25f, (float)t2);
                        orbiter3.BulletMinRadius = (float)Mathf.Lerp(0, 1, (float)t2);
                    }

                    if (i % 45 == 0 && i < 1050)
                    {
                        base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
                        float startingDirection = UnityEngine.Random.Range(-120f, 120f);
                        Vector2 targetPos = base.GetPredictedTargetPosition((float)(((double)UnityEngine.Random.value >= 0.5) ? 1 : 0), 12f);
                        for (int j = 0; j < 5; j++)
                        {
                            base.Fire(Offset.OverridePosition(star.transform.PositionVector2()), new Direction(startingDirection, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new SnakeBullet(j * 5, targetPos));
                        }
                    }
                    yield return base.Wait(1);
                }
                star.Dissipate();
                yield break;
            }

            public GameObject starObject;

            public IEnumerator DoBlast(AdditionalBraveLight light)
            {
                for (int i = 0; i < 300; i++)
                {
                    float t = (float)i / (float)240;
                    if (i == 60 | i == 150 | i == 240) 
                    {
                        GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(starObject.transform.PositionVector2(), Mathf.Lerp(1, 20, t), 0.075f, 100, 0.5f));
                        base.PostWwiseEvent("Play_Immolate", null);
                        base.PostWwiseEvent("Play_Immolate", null);

                    }
                    yield return base.Wait(1);
                    if (light)
                    {light.LightRadius = Mathf.Lerp(0, 20, MathToolbox.SinLerpTValue(t)); light.LightIntensity = Mathf.Lerp(2, 12, t); }

                }
                Vector2 pos = starObject.transform.PositionVector2();
                Exploder.DoDistortionWave(pos, 12f, 1f, 40, 0.7f);
                base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);
                base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);
                base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);

                bool b = BraveUtility.RandomBool();
                float E = BraveUtility.RandomAngle();
                for (int n = 0; n < 12; n++)
                {
                    for (int r = 0; r < 6; r++)
                    {
                        base.Fire(Offset.OverridePosition(pos), new Direction((30f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f + ((float)r/6), SpeedType.Absolute), new BlastAttack.RotatingBullet(pos, b, (30f * n), 0.05f));
                        base.Fire(Offset.OverridePosition(pos), new Direction((30f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f + ((float)r / 6), SpeedType.Absolute), new BlastAttack.RotatingBullet(pos, b, (30f * n) +4, 0.05f));
                        base.Fire(Offset.OverridePosition(pos), new Direction((30f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f + ((float)r / 6), SpeedType.Absolute), new BlastAttack.RotatingBullet(pos, b, (30f * n)-4, 0.05f));
                    }
                }
                b = !b;
                yield return base.Wait(30);
                for (int n = 0; n < 12; n++)
                {
                    for (int r = 0; r < 6; r++)
                    {
                        base.Fire(Offset.OverridePosition(pos), new Direction((30f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6f + ((float)r / 6), SpeedType.Absolute), new BlastAttack.RotatingBullet(pos, b, (30f * n), 0.05f));
                        base.Fire(Offset.OverridePosition(pos), new Direction((30f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6f + ((float)r / 6), SpeedType.Absolute), new BlastAttack.RotatingBullet(pos, b, (30f * n) + 4, 0.05f));
                        base.Fire(Offset.OverridePosition(pos), new Direction((30f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6f + ((float)r / 6), SpeedType.Absolute), new BlastAttack.RotatingBullet(pos, b, (30f * n) - 4, 0.05f));
                    }
                }
                b = !b;
                yield return base.Wait(30);
                for (int n = 0; n < 12; n++)
                {
                    for (int r = 0; r < 6; r++)
                    {
                        base.Fire(Offset.OverridePosition(pos), new Direction((30f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(5f + ((float)r /6), SpeedType.Absolute), new BlastAttack.RotatingBullet(pos, b, (30f * n), 0.05f));
                        base.Fire(Offset.OverridePosition(pos), new Direction((30f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(5f + ((float)r /6), SpeedType.Absolute), new BlastAttack.RotatingBullet(pos, b, (30f * n) + 4, 0.05f));
                        base.Fire(Offset.OverridePosition(pos), new Direction((30f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(5f + ((float)r / 6), SpeedType.Absolute), new BlastAttack.RotatingBullet(pos, b, (30f * n) - 4, 0.05f));
                    }
                }
                yield break;
            }


            public SpinBulletsController GenerateOrbit(GameObject star, int speed) 
            {
                SpinBulletsController spinBulletsController = star.AddComponent<SpinBulletsController>();
                spinBulletsController.ShootPoint = star.gameObject;
                spinBulletsController.OverrideBulletName = suckLessEntry.Name;
                spinBulletsController.NumBullets = 3;
                spinBulletsController.BulletMinRadius = 0.1f;
                spinBulletsController.BulletMaxRadius = 0f;
                spinBulletsController.BulletCircleSpeed = speed;
                spinBulletsController.BulletsIgnoreTiles = true;
                spinBulletsController.RegenTimer = 0.33f;
                spinBulletsController.AmountOFLines = 6;
                return spinBulletsController;
            }
            public SpinBulletsController GenerateOrbitMinor(GameObject star, int speed)
            {
                SpinBulletsController spinBulletsController = star.AddComponent<SpinBulletsController>();
                spinBulletsController.ShootPoint = star.gameObject;
                spinBulletsController.OverrideBulletName = suckLessEntry.Name;
                spinBulletsController.NumBullets = 2;
                spinBulletsController.BulletMinRadius = 0.1f;
                spinBulletsController.BulletMaxRadius = 0f;
                spinBulletsController.BulletCircleSpeed = speed;
                spinBulletsController.BulletsIgnoreTiles = true;
                spinBulletsController.RegenTimer = 0.33f;
                spinBulletsController.AmountOFLines = 6;
                return spinBulletsController;
            }

            public class RotatingBullet : Bullet
            {
                public RotatingBullet(Vector2 centerPoint, bool speeen, float startAngle, float spinspeed) : base("frogger", false, false, false)
                {
                    this.centerPoint = centerPoint;
                    this.yesToSpeenOneWay = speeen;
                    this.startAngle = startAngle;
                    this.SpinSpeed = spinspeed;
                }
                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    float radius = Vector2.Distance(this.centerPoint, base.Position);
                    float speed = this.Speed;
                    float spinAngle = this.startAngle;
                    float spinSpeed = 0f;
                    for (int i = 0; i < 600; i++)
                    {
                        //speed += 0.08333f;
                        radius += speed / 60f;
                        if (yesToSpeenOneWay == true)
                        {
                            spinSpeed -= SpinSpeed;
                        }
                        else
                        {
                            spinSpeed += SpinSpeed;

                        }
                        spinAngle += spinSpeed / 60f;
                        base.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }
                public Vector2 centerPoint;
                public bool yesToSpeenOneWay;
                public float startAngle;
                public float SpinSpeed;
            }



            public class LingeringFlame : Bullet
            {
                public LingeringFlame() : base("frogger", false, false, false)
                {
                }

                public override IEnumerator Top()
                {
                    yield return this.Wait(600);
                    base.Vanish();
                    yield break;
                }
            }

            public class SnakeBullet : Bullet
            {
                public SnakeBullet(int delay, Vector2 target) : base("suck_more", false, false, false)
                {
                    this.delay = delay;
                    this.target = target;
                }

                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    yield return base.Wait(this.delay);
                    Vector2 truePosition = base.Position;
                    for (int i = 0; i < 360; i++)
                    {
                        float offsetMagnitude = Mathf.SmoothStep(-0.45f, 0.45f, Mathf.PingPong(0.5f + (float)i / 60f * 3f, 1f));
                        if (i > 20 && i < 60 | i > 100 && i < 140 | i > 180 && i < 220)
                        {
                            float num = (this.target - truePosition).ToAngle();
                            float value = BraveMathCollege.ClampAngle180(num - this.Direction);
                            this.Direction += Mathf.Clamp(value, -6f, 6f);
                        }
                        truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
                        base.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }

                private int delay;
                private Vector2 target;
            }

        }
	}
}

