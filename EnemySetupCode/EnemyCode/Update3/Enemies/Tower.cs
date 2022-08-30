using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using Pathfinding;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Reflection;

namespace Planetside
{
	public class Tower : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "the_tower_psog";
		private static tk2dSpriteCollectionData TowerCollection;
		public static GameObject shootpoint;
		public static void Init()
		{
            Tower.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			if (!flag)
			{
				prefab = EnemyBuilder.BuildPrefab("Tower", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false, true);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 10000000;
				companion.aiActor.MovementSpeed = 0f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 0f;
				companion.aiActor.HasShadow = true;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(15f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.IgnoreForRoomClear = false;


                companion.gameObject.GetOrAddComponent<TeleportationImmunity>();

                EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.defaultShadow, new Vector2(0.5f, 0f), "shadowPos");


                companion.aiActor.healthHaver.SetHealthMaximum(15f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 2,
					ManualOffsetY = 0,
					ManualWidth = 12,
					ManualHeight = 22,
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
                    ManualOffsetX = 2,
                    ManualOffsetY = 0,
                    ManualWidth = 12,
                    ManualHeight = 22,
                    ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				companion.aiActor.PreventBlackPhantom = false;

                companion.aiActor.AwakenAnimType = AwakenAnimationType.Spawn;
                companion.aiActor.reinforceType = ReinforceType.SkipVfx;

                AIAnimator aiAnimator = companion.aiAnimator;
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "die", new string[] { "die" }, new DirectionalAnimation.FlipType[1]);
				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.EightWay,
					Flipped = new DirectionalAnimation.FlipType[8],
					AnimNames = new string[]
					{
						"idle",
                        "idle",
                        "idle",
                        "idle",
                        "idle",
                        "idle",
                        "idle",
                        "idle",
                    }
                };

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "lift", new string[]
				{
                    "lift_n",
                    "lift_nr",
                    "lift_r",
                    "lift_sr",
                    "lift_s",
                    "lift_sl",
                    "lift_l",
                    "lift_nl"
                }, new DirectionalAnimation.FlipType[8], DirectionalAnimation.DirectionType.EightWay);

                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[]
                {
                    "awaken",
                    "awaken",
                    "awaken",
                    "awaken",
                    "awaken",
                    "awaken",
                    "awaken",
                    "awaken",
                }, new DirectionalAnimation.FlipType[8], DirectionalAnimation.DirectionType.EightWay);

                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "slowlift", new string[]
                {
                    "slowlift_n",
                    "slowlift_nr",
                    "slowlift_r",
                    "slowlift_sr",
                    "slowlift_s",
                    "slowlift_sl",
                    "slowlift_l",
                    "slowlift_nl"
                }, new DirectionalAnimation.FlipType[8], DirectionalAnimation.DirectionType.EightWay);
                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "unlift", new string[]
				{
                    "unlift_n",
                    "unlift_nr",
                    "unlift_r",
                    "unlift_sr",
                    "unlift_s",
                    "unlift_sl",
                    "unlift_l",
                    "unlift_nl"
				}, new DirectionalAnimation.FlipType[8], DirectionalAnimation.DirectionType.EightWay);
                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "fire", new string[]
				{
                    "fire_n",
                    "fire_nr",
                    "fire_r",
                    "fire_sr",
                    "fire_s",
                    "fire_sl",
                    "fire_l",
                    "fire_nl"
				}, new DirectionalAnimation.FlipType[8], DirectionalAnimation.DirectionType.EightWay);


                if (TowerCollection == null)
				{
                    TowerCollection = SpriteBuilder.ConstructCollection(prefab, "BlockadeColection");
					UnityEngine.Object.DontDestroyOnLoad(TowerCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], TowerCollection);
					}
					

				


					SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int>{0}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;

                    foreach (var frame in companion.spriteAnimator.GetClipByName("idle").frames)
					{
						frame.invulnerableFrame = true;
					}

                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> {0,0}, "die", tk2dSpriteAnimationClip.WrapMode.Once).fps = 30f;
                    EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "die", new Dictionary<int, string> { { 0, "Play_RockBreaking" } });
                    EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "die", new Dictionary<int, string> { { 0, "Blast" } });


                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 1,2,3,4,5,6 }, "lift_s", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 7,8,9,10,11,12 }, "lift_sl", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 13,14,15,16,17,18 }, "lift_sr", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 19,20,21,22,23,24 }, "lift_l", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 25,26,27,28,29,30 }, "lift_r", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 31,32,33,34,35,36 }, "lift_nl", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 37,38,39,40,41,42 }, "lift_nr", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 43,44,45,46,47,48 }, "lift_n", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1 }, "slowlift_s", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 7, 8, 9, 10, 11, 12, 11, 10, 9, 8, 7 }, "slowlift_sl", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 13, 14, 15, 16, 17, 18, 17, 16, 15, 14, 13 }, "slowlift_sr", tk2dSpriteAnimationClip.WrapMode.Once).fps = 4f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 19, 20, 21, 22, 23, 24, 23, 22, 21, 20, 19 }, "slowlift_l", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 25, 26, 27, 28, 29, 30, 29, 28, 27, 26, 25 }, "slowlift_r", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 31, 32, 33, 34, 35, 36, 35, 34, 33, 32, 31 }, "slowlift_nl", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 37, 38, 39, 40, 41, 42, 41, 40, 39, 38, 37 }, "slowlift_nr", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 43, 44, 45, 46, 47, 48, 47, 46, 45, 44, 43 }, "slowlift_n", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;



                    foreach (var frame in companion.spriteAnimator.GetClipByName("lift_s").frames)
                    {
                        frame.invulnerableFrame = true;
                    }
                    foreach (var frame in companion.spriteAnimator.GetClipByName("lift_sl").frames)
                    {
                        frame.invulnerableFrame = true;
                    }
                    foreach (var frame in companion.spriteAnimator.GetClipByName("lift_sr").frames)
                    {
                        frame.invulnerableFrame = true;
                    }
                    foreach (var frame in companion.spriteAnimator.GetClipByName("lift_l").frames)
                    {
                        frame.invulnerableFrame = true;
                    }
                    foreach (var frame in companion.spriteAnimator.GetClipByName("lift_r").frames)
                    {
                        frame.invulnerableFrame = true;
                    }
                    foreach (var frame in companion.spriteAnimator.GetClipByName("lift_nl").frames)
                    {
                        frame.invulnerableFrame = true;
                    }
                    foreach (var frame in companion.spriteAnimator.GetClipByName("lift_nr").frames)
                    {
                        frame.invulnerableFrame = true;
                    }
                    foreach (var frame in companion.spriteAnimator.GetClipByName("lift_n").frames)
                    {
                        frame.invulnerableFrame = true;
                    }


                    for (int h = 0; h < 3; h++)
                    {
                        companion.spriteAnimator.GetClipByName("slowlift_s").frames[h].invulnerableFrame = true;
                        companion.spriteAnimator.GetClipByName("slowlift_sl").frames[h].invulnerableFrame = true;
                        companion.spriteAnimator.GetClipByName("slowlift_sr").frames[h].invulnerableFrame = true;
                        companion.spriteAnimator.GetClipByName("slowlift_l").frames[h].invulnerableFrame = true;
                        companion.spriteAnimator.GetClipByName("slowlift_r").frames[h].invulnerableFrame = true;
                        companion.spriteAnimator.GetClipByName("slowlift_nl").frames[h].invulnerableFrame = true;
                        companion.spriteAnimator.GetClipByName("slowlift_nr").frames[h].invulnerableFrame = true;
                        companion.spriteAnimator.GetClipByName("slowlift_n").frames[h].invulnerableFrame = true;
                    }

                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 49, 49, 50, 51}, "fire_s", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 52, 52, 53, 54 }, "fire_sl", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 55, 55, 56, 57 }, "fire_sr", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 55, 55, 59, 60 }, "fire_l", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 61, 61, 62, 63 }, "fire_r", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 64, 64, 65, 66 }, "fire_nl", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 67, 67, 68, 69 }, "fire_nr", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 70, 70, 71, 72 }, "fire_n", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;


                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 6, 5, 4, 3, 2, 1 }, "unlift_s", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 12, 11, 10, 9, 8, 7 }, "unlift_sl", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 18, 17, 16, 15, 14, 13 }, "unlift_sr", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 24, 23, 22, 21, 20, 19 }, "unlift_l", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 30, 29, 28, 27, 26, 25 }, "unlift_r", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 36, 35, 34, 33, 32, 31 }, "unlift_nl", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 42, 41, 40, 39, 38, 37 }, "unlift_nr", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 48, 47, 46, 45, 44, 43 }, "unlift_n", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

                    SpriteBuilder.AddAnimation(companion.spriteAnimator, TowerCollection, new List<int> { 73, 74,75,76,77,78,79,80,81 }, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

                }
                var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				GameObject sh = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.5f, 0.625f), "turrt");

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

				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
				{
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
						Behavior = new ShootBehavior{
						ShootPoint = sh,
						BulletScript = new CustomBulletScriptSelector(typeof(TurretAttack)),
						LeadAmount = 0f,
						AttackCooldown = 1f,
						Cooldown = 3f,
						InitialCooldown = 1f,
						ChargeTime = 1f,
						RequiresLineOfSight = false,
						MultipleFireEvents = false,
						Uninterruptible = true,
						FireAnimation = "fire",
						ChargeAnimation = "lift",
						PostFireAnimation = "unlift",
						Range = 6
						}
					},
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0.5f,
                        Behavior = new ShootBehavior{
                        ShootPoint = sh,
                        BulletScript = new CustomBulletScriptSelector(typeof(TelegraphAttack)),
                        LeadAmount = 0f,
                        AttackCooldown = 1f,
                        Cooldown = 7f,
                        InitialCooldown = 2f,
                        ChargeTime = 0f,
                        RequiresLineOfSight = false,
                        MultipleFireEvents = false,
                        Uninterruptible = true,
                        FireAnimation = "slowlift",
                        Range = 10,
						MinRange = 5,
                        }
                    },
                };

                companion.aiActor.sprite.usesOverrideMaterial = true;
                Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
                mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
                mat.SetFloat("_EmissiveColorPower", 3f);
                mat.SetFloat("_EmissivePower", 40);
                companion.aiActor.sprite.renderer.material = mat;


                bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:tower", companion.aiActor);


				SpriteBuilder.AddSpriteToCollection(Defpath + "turretthing_idle_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:tower";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = Defpath + "turretthing_idle_001.png";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\fodderammonomiconentry.png");
				PlanetsideModule.Strings.Enemies.Set("#TOWER", "Blockade");
				PlanetsideModule.Strings.Enemies.Set("#TOWER_SHORTDESC", "Iron Wall");
				PlanetsideModule.Strings.Enemies.Set("#TOWER_LONGDESC", "Filled with the remnants of lively, yet deceased Gundead, though Gungeoneers often have little respect for Gundead practices.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#TOWER";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#TOWER_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#TOWER_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:tower");
				EnemyDatabase.GetEntry("psog:tower").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:tower").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:tower").isNormalEnemy = true;

                companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableHitscan);
                companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableOldKingSlamBullet);
                companion.aiActor.knockbackDoer.SetImmobile(true, "Im a rock");

            }
        }

		private static string Defpath = "Planetside/Resources/Enemies/Tower/";


        private static string[] spritePaths = new string[]
		{

			Defpath+"turretthing_idle_001.png",//idle
			//Charge Animations
			//South
			Defpath+"turretthing_lift_south_001.png",
            Defpath+"turretthing_lift_south_002.png",
            Defpath+"turretthing_lift_south_003.png",
            Defpath+"turretthing_lift_south_004.png",
            Defpath+"turretthing_lift_south_005.png",
            Defpath+"turretthing_lift_south_006.png",
			//South Left
            Defpath+"turretthing_lift_south_left_001.png",
            Defpath+"turretthing_lift_south_left_002.png",
            Defpath+"turretthing_lift_south_left_003.png",
            Defpath+"turretthing_lift_south_left_004.png",
            Defpath+"turretthing_lift_south_left_005.png",
            Defpath+"turretthing_lift_south_left_006.png",
			//South Right
			Defpath+"turretthing_lift_south_right_001.png",
            Defpath+"turretthing_lift_south_right_002.png",
            Defpath+"turretthing_lift_south_right_003.png",
            Defpath+"turretthing_lift_south_right_004.png",
            Defpath+"turretthing_lift_south_right_005.png",
            Defpath+"turretthing_lift_south_right_006.png",
			//Left
			Defpath+"turretthing_lift_left_001.png",
            Defpath+"turretthing_lift_left_002.png",
            Defpath+"turretthing_lift_left_003.png",
            Defpath+"turretthing_lift_left_004.png",
            Defpath+"turretthing_lift_left_005.png",
            Defpath+"turretthing_lift_left_006.png",
			//Right
			Defpath+"turretthing_lift_right_001.png",
            Defpath+"turretthing_lift_right_002.png",
            Defpath+"turretthing_lift_right_003.png",
            Defpath+"turretthing_lift_right_004.png",
            Defpath+"turretthing_lift_right_005.png",
            Defpath+"turretthing_lift_right_006.png",
			//North left
			Defpath+"turretthing_lift_north_left_001.png",
            Defpath+"turretthing_lift_north_left_002.png",
            Defpath+"turretthing_lift_north_left_003.png",
            Defpath+"turretthing_lift_north_left_004.png",
            Defpath+"turretthing_lift_north_left_005.png",
            Defpath+"turretthing_lift_north_left_006.png",
			//North Right
			Defpath+"turretthing_lift_north_right_001.png",
            Defpath+"turretthing_lift_north_right_002.png",
            Defpath+"turretthing_lift_north_right_003.png",
            Defpath+"turretthing_lift_north_right_004.png",
            Defpath+"turretthing_lift_north_right_005.png",
            Defpath+"turretthing_lift_north_right_006.png",
			//North
			Defpath+"turretthing_lift_north_001.png",
            Defpath+"turretthing_lift_north_002.png",
            Defpath+"turretthing_lift_north_003.png",
            Defpath+"turretthing_lift_north_004.png",
            Defpath+"turretthing_lift_north_005.png",
            Defpath+"turretthing_lift_north_006.png",
			//Fire Animations
			//South
			Defpath+"turretthing_fire_south_001.png",
            Defpath+"turretthing_fire_south_002.png",
            Defpath+"turretthing_fire_south_003.png",
			//South Left
			Defpath+"turretthing_fire_south_left_001.png",
            Defpath+"turretthing_fire_south_left_002.png",
            Defpath+"turretthing_fire_south_left_003.png",
			//South Right
			Defpath+"turretthing_fire_south_right_001.png",
            Defpath+"turretthing_fire_south_right_002.png",
            Defpath+"turretthing_fire_south_right_003.png",
			//Left
			Defpath+"turretthing_fire_left_001.png",
            Defpath+"turretthing_fire_left_002.png",
            Defpath+"turretthing_fire_left_003.png",
			//Right
			Defpath+"turretthing_fire_right_001.png",
            Defpath+"turretthing_fire_right_002.png",
            Defpath+"turretthing_fire_right_003.png",
			//North Left
			Defpath+"turretthing_fire_north_left_001.png",
            Defpath+"turretthing_fire_north_left_002.png",
            Defpath+"turretthing_fire_north_left_003.png",
			//North Right
			Defpath+"turretthing_fire_north_right_001.png",
            Defpath+"turretthing_fire_north_right_002.png",
            Defpath+"turretthing_fire_north_right_003.png",
			//North
			Defpath+"turretthing_fire_north_001.png",
            Defpath+"turretthing_fire_north_002.png",
            Defpath+"turretthing_fire_north_003.png",
            //Awaken
			Defpath+"turretthing_awaken_001.png",
            Defpath+"turretthing_awaken_002.png",
            Defpath+"turretthing_awaken_003.png",
            Defpath+"turretthing_awaken_004.png",
            Defpath+"turretthing_awaken_005.png",
            Defpath+"turretthing_awaken_006.png",
            Defpath+"turretthing_awaken_007.png",
            Defpath+"turretthing_awaken_008.png",
            Defpath+"turretthing_awaken_009.png",

        };

        public class TelegraphAttack : Script
        {
            protected override IEnumerator Top()
            {
                float Angle = base.AimDirection;
                GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
                tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
                component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
                component2.dimensions = new Vector2(1000f, 1f);
                component2.UpdateZDepth();
                component2.HeightOffGround = -2;
                Color laser = new Color(0f, 1f, 1f, 1f);
                component2.sprite.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
                component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
                GameManager.Instance.StartCoroutine(FlashReticles(component2, false, this));

                yield return this.Wait(105);
                yield break;
            }

            private IEnumerator FlashReticles(tk2dTiledSprite tiledspriteObject, bool isDodgeAble, TelegraphAttack parent)
            {
                tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
                float elapsed = 0;
                float Time = 1f;
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
                        float Ang = (this.GetPredictedTargetPositionExact(1, 600) - this.BulletBank.aiActor.sprite.WorldCenter).ToAngle();
                        tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);
                        float math = isDodgeAble == true ? 250 : 25;
                        tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (math * t));
                        tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (10 * t));
                        tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, Ang);
                        tiledsprite.HeightOffGround = -2;
                        tiledsprite.renderer.gameObject.layer = 23;
                        tiledsprite.dimensions = new Vector2(1000f, 1f);
                        tiledsprite.UpdateZDepth();
                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                elapsed = 0;
                Time = 0.25f;
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
                float a = tiledsprite.transform.localRotation.eulerAngles.z;
                Destroy(tiledspriteObject.gameObject);
                if (base.BulletBank.aiActor != null)
                {
                    base.PostWwiseEvent("Play_AbyssBlast", null);
                    base.Fire(new Direction(a, DirectionType.Absolute, -1f), new Speed(600f, SpeedType.Absolute), new HitScan());
                }
                yield break;
            }


            public class HitScan : Bullet
            {
                public HitScan() : base(StaticUndodgeableBulletEntries.UndodgeableHitscan.Name, false, false, false)
                {

                }
                protected override IEnumerator Top()
                {
                    SpawnManager.PoolManager.Remove(this.Projectile.gameObject.transform);
                    this.Projectile.BulletScriptSettings.preventPooling = true;

                    yield break;
                }
            }
        }

        public class TurretAttack : Script
		{
			protected override IEnumerator Top()
			{
				float aimDirection = base.GetAimDirection((float)((UnityEngine.Random.value >= 0.25f) ? 0 : 1), 10f);
                float h = BraveUtility.RandomBool() == true ? 30 : -30;

                for (int e = -1; e < 2; e++)
				{
                    base.PostWwiseEvent("Play_AbyssBlast");

                    GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(228) as Gun).muzzleFlashEffects.effects[0].effects[0].effect, true);
                    vfx.transform.position = this.BulletBank.aiActor.transform.Find("turrt").transform.position;
                    vfx.transform.localRotation = Quaternion.Euler(0f, 0f, aimDirection + (15 * e));
                    vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;

                    for (int i = 0; i < 4; i++)
					{
						base.Fire(new Direction(aimDirection + (h * e), DirectionType.Absolute, -1f), new Speed(2+(i*2), SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableOldKingSlamBullet.Name, 15, 60));
					}
					yield return this.Wait(10);
				}
				yield break;
			}
          

        }

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
                if (clip.GetFrame(frameIdx).eventInfo.Contains("Blast"))
                {
                    GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(365) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects.First().effects.First().effect, true);
                    vfx.transform.position = this.aiActor.sprite.WorldCenter;
                    vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                    Destroy(vfx, 1);
                }
            }
        }
	}
}





