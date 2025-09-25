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
using System.Reflection;
using UnityEngine.UI;
using Alexandria.PrefabAPI;
using static Planetside.ArchGunjurer;
using static Planetside.Revenant_Enemy.Attack_Curvers;
using BreakAbleAPI;
using Alexandria;

namespace Planetside
{
	public class Revenant_Enemy : AIActor
	{
		public static readonly string guid = "PSOG_Revenant";

        public static GameObject hook;

		public static void Init(bool isFaker)
		{
            
            {

                

                if (isFaker)
                {
                    hook = PrefabBuilder.BuildObject("Revenant Chain Hook");
                    DontDestroyOnLoad(hook);
                    var chainSprite = hook.AddComponent<tk2dSprite>();
                    chainSprite.SetSprite(StaticSpriteDefinitions.EnemySpecific_Sheet_Data, "revenant_chain_hook");
                    var body = hook.CreateFastBody(new IntVector2(10, 10), new IntVector2(-5, -5), CollisionLayer.Projectile);
                    hook.CreateFastBody(new IntVector2(10, 10), new IntVector2(-5, -5), CollisionLayer.EnemyHitBox);
                    body.CollideWithTileMap = true;
                    var revenantChain = hook.AddComponent<RevenantChainAttacher>();
                    var mat = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
                    mat.mainTexture = chainSprite.renderer.material.mainTexture;
                    mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    mat.SetFloat("_EmissivePower", 20);
                    mat.SetFloat("_EmissiveColorPower", 32);
                    chainSprite.renderer.material = mat;


                    var chain = PrefabBuilder.BuildObject("Revenant Chain");
                    var spr = chain.gameObject.AddComponent<tk2dTiledSprite>();
                    spr.SetSprite(StaticSpriteDefinitions.EnemySpecific_Sheet_Data, "revenant_chain_chain");
                    spr.dimensions = new Vector2(6, 6);
                    spr.SortingOrder = 20;
                    spr.HeightOffGround = 2;
                    spr.ShouldDoTilt = false;
                    spr.IsPerpendicular = false;
                    spr._anchor = tk2dBaseSprite.Anchor.MiddleLeft;
                    spr.gameObject.layer = LayerMask.NameToLayer("FG_Nonsense");
                    spr.ignoresTiltworldDepth = false;
                    spr.renderer.sortingLayerName = "Foreground";

                    spr.usesOverrideMaterial = true;
                    mat = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
                    mat.mainTexture = spr.renderer.material.mainTexture;
                    mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    mat.SetFloat("_EmissivePower", 20);
                    mat.SetFloat("_EmissiveColorPower", 32);
                    spr.renderer.material = mat;

                    chain.transform.SetParent(hook.transform, false);
                    revenantChain.ChainTiles = spr;
                }




                var prefab = EnemyBuilder.BuildPrefabBundle("Revenant Enemy", isFaker ? guid : "psog:snipeidolon", StaticSpriteDefinitions.EnemySpecific_Sheet_Data, 0, new IntVector2(0, 0), new IntVector2(0, 0), false);
                
                var companion = prefab.AddComponent<RevenantHide>();
                companion.isaHider = isFaker;

                prefab.AddComponent<TeleportationImmunity>();

                var animator = companion.gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
                animator.library = StaticSpriteDefinitions.EnemySpecific_Animation_Data;

                companion.aiActor.knockbackDoer.weight = 1000;
                companion.aiActor.MovementSpeed = 0.7f;
                companion.aiActor.healthHaver.PreventAllDamage = false;
                companion.aiActor.CollisionDamage = 0f;
                companion.aiActor.HasShadow = false;
                companion.aiActor.IgnoreForRoomClear = false;
                companion.aiActor.aiAnimator.HitReactChance = 0f;
                companion.aiActor.specRigidbody.CollideWithOthers = true;
                companion.aiActor.specRigidbody.CollideWithTileMap = true;
                companion.aiActor.PreventFallingInPitsEver = true;
                companion.aiActor.healthHaver.ForceSetCurrentHealth(85f);
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = false;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.healthHaver.AllDamageMultiplier = 0;
                companion.aiActor.CanDropCurrency = true;
                companion.aiActor.healthHaver.SuppressDeathSounds = false;

                EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(0.9375f, -0.25f), "shadowPos");
                companion.aiActor.SetIsFlying(true, "Gamemode: Creative", true, true);
                companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;

                companion.aiActor.healthHaver.SetHealthMaximum(85f, null, false);
                companion.aiActor.specRigidbody.PixelColliders.Clear();
                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 8,
                    ManualOffsetY = 1,
                    ManualWidth = 17,
                    ManualHeight = 50,
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
                    ManualOffsetX = 8,
                    ManualOffsetY = 1,
                    ManualWidth = 17,
                    ManualHeight = 50,
                    ManualDiameter = 0,
                    ManualLeftX = 0,
                    ManualLeftY = 0,
                    ManualRightX = 0,
                    ManualRightY = 0,



                });
                companion.aiActor.PreventBlackPhantom = true;
                AIAnimator aiAnimator = companion.aiAnimator;
                aiAnimator.IdleAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.SixWay,
                    Flipped = new DirectionalAnimation.FlipType[6],
                    AnimNames = new string[]
                    {
                        "idle_back",
                        "idle_backright",
                        "idle_frontright",
                        "idle_front",
                        "idle_frontleft",
                        "idle_backleft"
                    }
                };

                EnemyToolbox.AddOffsetToFrames(animator, "idle_frontleft", new Dictionary<int, Vector3>()
                {
                    { 0, new Vector3(-0.0625f, 0) },
                    { 1, new Vector3(-0.0625f, 0) },
                    { 2, new Vector3(-0.0625f, 0) },
                    { 3, new Vector3(-0.0625f, 0) },
                    { 4, new Vector3(-0.0625f, 0) },
                    { 5, new Vector3(-0.0625f, 0) },
                    { 6, new Vector3(-0.0625f, 0) },
                });
                EnemyToolbox.AddOffsetToFrames(animator, "idle_backleft", new Dictionary<int, Vector3>()
                {
                    { 0, new Vector3(0.0625f, 0) },
                    { 1, new Vector3(0.0625f, 0) },
                    { 2, new Vector3(0.0625f, 0) },
                    { 3, new Vector3(0.0625f, 0) },
                    { 4, new Vector3(0.0625f, 0) },
                    { 5, new Vector3(0.0625f, 0) },
                    { 6, new Vector3(0.0625f, 0) },
                });

                EnemyToolbox.AddOffsetToFrames(animator, "idle_frontright", new Dictionary<int, Vector3>()
                {
                    { 0, new Vector3(-0.0625f, 0) },
                    { 1, new Vector3(-0.0625f, 0) },
                    { 2, new Vector3(-0.0625f, 0) },
                    { 3, new Vector3(-0.0625f, 0) },
                    { 4, new Vector3(-0.0625f, 0) },
                    { 5, new Vector3(-0.0625f, 0) },
                    { 6, new Vector3(-0.0625f, 0) },
                });
                EnemyToolbox.AddOffsetToFrames(animator, "idle_backright", new Dictionary<int, Vector3>()
                {
                    { 0, new Vector3(0.0625f, 0) },
                    { 1, new Vector3(0.0625f, 0) },
                    { 2, new Vector3(0.0625f, 0) },
                    { 3, new Vector3(0.0625f, 0) },
                    { 4, new Vector3(0.0625f, 0) },
                    { 5, new Vector3(0.0625f, 0) },
                    { 6, new Vector3(0.0625f, 0) },
                });


                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "awaken",
                    new string[] { "awaken" },
                    new DirectionalAnimation.FlipType[1]);

                EnemyToolbox.AddOffsetToFrames(animator, "awaken", new Dictionary<int, Vector3>()
                {
                    { 0, new Vector3(0.125f, 0.0625f) },
                    { 1, new Vector3(0.125f, 0.0625f) },
                    { 2, new Vector3(0.125f, 0.0625f) },
                    { 3, new Vector3(0.125f, 0.0625f) },
                    { 4, new Vector3(0.125f, 0.0625f) },
                    { 5, new Vector3(0.125f, 0.0625f) },
                    { 6, new Vector3(0.125f, 0.0625f) },

                });

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "charge",
                    new string[] { "charge" },
                    new DirectionalAnimation.FlipType[1]);


                EnemyToolbox.AddSoundsToAnimationFrame(animator, "charge", new Dictionary<int, string>()
                {
                    { 0, "Play_Snipeilodon_Tell" },
                });

                //Play_Snipeilodon_Tell
                EnemyToolbox.AddOffsetToFrames(animator, "charge", new Dictionary<int, Vector3>()
                {
                    { 0, new Vector3(-0, -0.125f) },
                    { 1, new Vector3(-0, -0.125f) },
                    { 2, new Vector3(-0, -0.125f) },
                    { 3, new Vector3(-0, -0.125f) },
                    { 4, new Vector3(-0, -0.125f) },
                    { 5, new Vector3(-0, -0.125f) },
                    { 6, new Vector3(-0, -0.125f) },
                });

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "dechain",
                    new string[] { "dechain" },
                    new DirectionalAnimation.FlipType[1]);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "death",
                    new string[] 
                    { 
                        "death_right",
                        "death_left"
                    },       
                new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);




                EnemyToolbox.AddSoundsToAnimationFrame(animator, "death_right", new Dictionary<int, string>()
                {
                    { 0, "Play_Snipeilodon_Death" },
                });
                EnemyToolbox.AddSoundsToAnimationFrame(animator, "death_left", new Dictionary<int, string>()
                {
                    { 0, "Play_Snipeilodon_Death" },
                });
                EnemyToolbox.AddOffsetToFrames(animator, "death_right", new Dictionary<int, Vector3>()
                {
                    { 0, new Vector3(-0.25f, -0.125f) },
                });
                EnemyToolbox.AddOffsetToFrames(animator, "death_left", new Dictionary<int, Vector3>()
                {
                    { 0, new Vector3(-0.25f, -0.125f) },
                });




                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "hook_in",
                    new string[]
                    {
                        "chain_up",
                        "chain_upright",
                        "chain_downright",
                        "chain_down",
                        "chain_downleft",
                        "chain_upleft"
                    },
                new DirectionalAnimation.FlipType[6], DirectionalAnimation.DirectionType.SixWay);

                EnemyToolbox.AddOffsetToFrames(animator, "chain_upright", new Dictionary<int, Vector3>()
                {
                    { 0, new Vector3(-0.25f, 0.125f) },
                    { 1, new Vector3(-0.25f, 0.125f) },
                    { 2, new Vector3(-0.25f, 0.125f) },
                });
                EnemyToolbox.AddOffsetToFrames(animator, "chain_up", new Dictionary<int, Vector3>()
                {
                    { 0, new Vector3(0, -0.125f) },
                    { 1, new Vector3(0, -0.125f) },
                    { 2, new Vector3(0, -0.125f) },
                });
                EnemyToolbox.AddOffsetToFrames(animator, "chain_down", new Dictionary<int, Vector3>()
                {
                    { 0, new Vector3(0, -0.125f) },
                    { 1, new Vector3(0, -0.125f) },
                    { 2, new Vector3(0, -0.125f) },
                });
                EnemyToolbox.AddOffsetToFrames(animator, "chain_downright", new Dictionary<int, Vector3>()
                {
                    { 0, new Vector3(-0.25f, 0.125f) },
                    { 1, new Vector3(-0.25f, 0.125f) },
                    { 2, new Vector3(-0.25f, 0.125f) },
                });



                var bs = prefab.GetComponent<BehaviorSpeculator>();
                var OvM = prefab.GetComponent<ObjectVisibilityManager>();

                BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
                bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
                bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
                var shootpoint = new GameObject("fuck");
                shootpoint.transform.parent = companion.transform;
                shootpoint.transform.position = companion.sprite.WorldCenter - new Vector2(0.125f, 0.25f);
                GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;


                bs.AttackBehaviors = new List<AttackBehaviorBase>()
                {
                            new RevenantChainBehavior()
                            {
                                ShootPoint = shootpoint,
                                AttackCooldown = 3f,
                                Cooldown = 4f,
                                RequiresLineOfSight = true,
                                Uninterruptible = true,
                                Chainprefab = hook,
                                BulletScript_Shoot_When_Chain = new CustomBulletScriptSelector(typeof(Attack_Sexo)),
                                BulletScript_Shoot_When_Missed = new CustomBulletScriptSelector(typeof(Attack_Curvers)),
                                BulletScript_Shoot_Orbital = new CustomBulletScriptSelector(typeof(Monolith)),
                                MoveSpeedModifier = 0,
                                CooldownVariance = 2,
                                InitialCooldown = 2.25f,
                                FireAnimation = "hook_in",
                                ChargeAnimation = "charge",
                                ChargeTime = 2.5f
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
                        PauseTime = 0.25f,
                    }
                };
                bs.MovementBehaviors = new List<MovementBehaviorBase>
                {
                    new MoveErraticallyBehavior
                    {
                        PointReachedPauseTime = 0.5f,
                        PathInterval = 0.1f,
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
                Game.Enemies.Add(isFaker ? "psog:revenant" : "psog:snipeidolon", companion.aiActor);

                Material matShader = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                matShader.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
                matShader.SetColor("_EmissiveColor", new Color32(255, 156, 0, 255));
                matShader.SetFloat("_EmissiveColorPower", 15f);
                matShader.SetFloat("_EmissivePower", 8);
                matShader.SetFloat("_EmissiveThresholdSensitivity", 0.125f);

                companion.aiActor.sprite.renderer.material = matShader;


                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));

                if (companion.GetComponent<EncounterTrackable>() != null)
                {
                    UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
                }
                companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
                companion.encounterTrackable.journalData = new JournalEntry();
                companion.encounterTrackable.EncounterGuid = "psog:revenant";
                companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
                companion.encounterTrackable.journalData.SuppressKnownState = false;
                companion.encounterTrackable.journalData.IsEnemy = true;
                companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
                companion.encounterTrackable.ProxyEncounterGuid = "";
                if (isFaker)
                {
                    SpriteBuilder.AddSpriteToCollection(StaticSpriteDefinitions.EnemySpecific_Sheet_Data.GetSpriteDefinition("revenantnew_idle_down_001"), SpriteBuilder.ammonomiconCollection);

                    companion.encounterTrackable.journalData.AmmonomiconSprite = "revenantnew_idle_down_001";
                    companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("snipeidolonsheet");// ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\archgunjurericon.png");


                    PlanetsideModule.Strings.Enemies.Set("#SNIPEIDOLON", "Snipeidolon");
                    PlanetsideModule.Strings.Enemies.Set("#SNIPEIDOLON_SHORTDESC", "Lich Informer");
                    PlanetsideModule.Strings.Enemies.Set("#SNIPEIDOLON_LONGDESC", "Fearless Gundead, who, like their Revolvenant brethren, studied closely under the Gungeon Master himself, and act as informants for the chambers above. As part of the steps to Lichdon, parts of their bodies are removed, to be returned to them enchanced in ammunition once they succeed in their current step.");
                    companion.encounterTrackable.journalData.PrimaryDisplayName = "#SNIPEIDOLON";
                    companion.encounterTrackable.journalData.NotificationPanelDescription = "#SNIPEIDOLON_SHORTDESC";
                    companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#SNIPEIDOLON_LONGDESC";
                    EnemyBuilder.AddEnemyToDatabase(companion.gameObject, isFaker ? "psog:revenant" : "psog:snipeidolon");
                    EnemyDatabase.GetEntry("psog:revenant").ForcedPositionInAmmonomicon = 97;
                    EnemyDatabase.GetEntry("psog:revenant").isInBossTab = false;
                    EnemyDatabase.GetEntry("psog:revenant").isNormalEnemy = true;
                }
                else
                {
                    EncounterDatabaseEntry encounterDatabaseEntry = new EncounterDatabaseEntry(companion.aiActor.encounterTrackable)
                    {
                        path = "psog:snipeidolon",
                        myGuid = "psog:snipeidolon"
                    };
                    EncounterDatabase.Instance.Entries.Add(encounterDatabaseEntry);
                }




                DebrisObject shoulder1 = BreakableAPI_Bundled.GenerateDebrisObject("revanantnew_skull", StaticSpriteDefinitions.EnemySpecific_Sheet_Data,
                    true, 0.5f, 3, 10, 2, null, 0.9f, "Play_WPN_shell_impact_01", null, 2);
                ShardCluster BONES = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shoulder1 }, 0.4f, 2f, 1, 1, 1f);
                SpawnShardsOnDeath BodyAndStuff = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
                BodyAndStuff.deathType = OnDeathBehavior.DeathType.Death;
                BodyAndStuff.breakStyle = MinorBreakable.BreakStyle.BURST;
                BodyAndStuff.verticalSpeed = 0.4f;
                BodyAndStuff.heightOffGround = 3.75f;
                BodyAndStuff.shardClusters = new ShardCluster[] { BONES };
                

                shoulder1 = BreakableAPI_Bundled.GenerateDebrisObject("revanantnew_ribs", StaticSpriteDefinitions.EnemySpecific_Sheet_Data,
                    true, 0.5f, 3, 10, 2, null, 0.9f, "Play_BOSS_lichA_crack_01", null, 1);
                BONES = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shoulder1 }, 0.4f, 2f, 1, 1, 1f);
                BodyAndStuff = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
                BodyAndStuff.deathType = OnDeathBehavior.DeathType.Death;
                BodyAndStuff.breakStyle = MinorBreakable.BreakStyle.BURST;
                BodyAndStuff.verticalSpeed = 0.4f;
                BodyAndStuff.heightOffGround = 3f;
                BodyAndStuff.shardClusters = new ShardCluster[] { BONES };

                shoulder1 = BreakableAPI_Bundled.GenerateDebrisObject("revanantnew_spine", StaticSpriteDefinitions.EnemySpecific_Sheet_Data,
                    true, 0.5f, 3, 10, 2, null, 0.9f, "Play_FS_shelleton_stone_01", null, 1);
                BONES = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shoulder1 }, 0.4f, 2f, 1, 1, 1f);
                BodyAndStuff = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
                BodyAndStuff.deathType = OnDeathBehavior.DeathType.Death;
                BodyAndStuff.breakStyle = MinorBreakable.BreakStyle.BURST;
                BodyAndStuff.verticalSpeed = 0.4f;
                BodyAndStuff.heightOffGround = 2.75f;
                BodyAndStuff.shardClusters = new ShardCluster[] { BONES };

                shoulder1 = BreakableAPI_Bundled.GenerateDebrisObject("revanantnew_belt", StaticSpriteDefinitions.EnemySpecific_Sheet_Data,
                    true, 0.5f, 3, 10, 2, null, 1.25f, "Play_WPN_metalbullet_impact_01", null, 0);
                BONES = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shoulder1 }, 0.4f, 2f, 1, 1, 1f);
                BodyAndStuff = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
                BodyAndStuff.deathType = OnDeathBehavior.DeathType.Death;
                BodyAndStuff.breakStyle = MinorBreakable.BreakStyle.BURST;
                BodyAndStuff.verticalSpeed = 0.4f;
                BodyAndStuff.heightOffGround = 2.75f;
                BodyAndStuff.shardClusters = new ShardCluster[] { BONES };

                shoulder1 = BreakableAPI_Bundled.GenerateDebrisObject("revanantnew_scarf_001", StaticSpriteDefinitions.EnemySpecific_Sheet_Data,
                    true, 0.5f, 3, 0, 0, null, 2, "", null, 0);
                var obj_animator = shoulder1.AddComponent<tk2dSpriteAnimator>();
                obj_animator.Library = StaticSpriteDefinitions.EnemySpecific_Animation_Data;
                obj_animator.playAutomatically = true;
                obj_animator.DefaultClipId = StaticSpriteDefinitions.EnemySpecific_Animation_Data.GetClipIdByName("scarf_fade");
                var killer = shoulder1.AddComponent<SpriteAnimatorKiller>();
                killer.animator = obj_animator;
                killer.fadeTime = -1;

                BONES = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shoulder1 }, 0.1f, 2f, 1, 1, 1f);
                BodyAndStuff = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
                BodyAndStuff.deathType = OnDeathBehavior.DeathType.Death;
                BodyAndStuff.breakStyle = MinorBreakable.BreakStyle.CONE;
                BodyAndStuff.verticalSpeed = 0.01f;
                BodyAndStuff.heightOffGround = 3.25f;
                BodyAndStuff.shardClusters = new ShardCluster[] { BONES };

                shoulder1 = BreakableAPI_Bundled.GenerateDebrisObject("revanantnew_breech_001", StaticSpriteDefinitions.EnemySpecific_Sheet_Data,
                    true, 0.5f, 3, 0, 0, null, 2, "", null, 0);
                obj_animator = shoulder1.AddComponent<tk2dSpriteAnimator>();
                obj_animator.Library = StaticSpriteDefinitions.EnemySpecific_Animation_Data;
                obj_animator.playAutomatically = true;
                obj_animator.DefaultClipId = StaticSpriteDefinitions.EnemySpecific_Animation_Data.GetClipIdByName("breech_fade");
                killer = shoulder1.AddComponent<SpriteAnimatorKiller>();
                killer.animator = obj_animator;
                killer.fadeTime = -1;

                BONES = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shoulder1 }, 0.1f, 2f, 1, 1, 1f);
                BodyAndStuff = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
                BodyAndStuff.deathType = OnDeathBehavior.DeathType.Death;
                BodyAndStuff.breakStyle = MinorBreakable.BreakStyle.CONE;
                BodyAndStuff.verticalSpeed = 0.01f;
                BodyAndStuff.heightOffGround = 3.25f;
                BodyAndStuff.shardClusters = new ShardCluster[] { BONES };



            }
        }

        public class Monolith : Script
        {
            public override IEnumerator Top()
            {
                float aim = this.AimDirection;
                base.PostWwiseEvent("Play_ENM_Grip_Master_Swipe_01", null);

                for (float i = 0; i < 3; i++)
                {
                    ParticleBase.EmitParticles("WaveParticleInverse", 1, new ParticleSystem.EmitParams()
                    {
                        position = this.Position,
                        startSize = 32,
                        rotation = 0,
                        startLifetime = 0.5f,
                        startColor = new Color(1, 0.2f, 0).WithAlpha(0.4f),
                        angularVelocity = 0
                    });
                    yield return Wait(15);
                }
                base.PostWwiseEvent("Play_BOSS_RatMech_Cannon_01", null);
                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = this.Position,
                    startSize = 48,
                    rotation = 0,
                    startLifetime = 0.75f,
                    startColor = new Color(1, 0.2f, 0).WithAlpha(0.2f),
                    angularVelocity = 0
                });
                for (float i = 0; i < 210; i++)
                {
                    if (i % 5 == 0)
                        base.PostWwiseEvent("Play_ENM_ironmaiden_blast_01", null);

                    aim = Mathf.MoveTowardsAngle(aim, this.AimDirection, 1f);

                    var a = Mathf.Max(30, 180 - (i * 2));

                    var v = Vector2.Lerp(Vector2.zero, new Vector2(0.5f, 0.5f), i  / 120);

                    base.Fire(Offset.OverridePosition(this.Position + BraveUtility.RandomVector2(v * -1, v)), new Direction(UnityEngine.Random.Range(-1, 1) + aim, DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(0.1f, 1.2f), SpeedType.Absolute), new SpeedChangingBullet("sweep", 17, (int)a));
                    base.Fire(Offset.OverridePosition(this.Position + BraveUtility.RandomVector2(v * -1, v)),  new Direction(UnityEngine.Random.Range(-3, 3) + aim, DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(0.4f, 2.2f), SpeedType.Absolute), new SpeedChangingBullet("sweep", 20, (int)a));
                    base.Fire(Offset.OverridePosition(this.Position + BraveUtility.RandomVector2(v * -1, v)),  new Direction(UnityEngine.Random.Range(-4, 4) + aim, DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(0.1f, 1.2f), SpeedType.Absolute), new SpeedChangingBullet("sweep", 24, (int)a));
                    yield return Wait(1);
                }
            }
        }

        public class Attack_Curvers : Script
        {
            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_ENM_ironmaiden_blast_01", null);
                for (int i = 0; i < 6; i++)
                {
                    base.Fire(new Direction(-UnityEngine.Random.Range(63, 81), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(5.1f, 7.2f), SpeedType.Absolute), new Curver("sweep", 12 + (i * 1.5f)));
                    base.Fire(new Direction(-UnityEngine.Random.Range(63, 81), DirectionType.Aim, -1f), new Speed(4, SpeedType.Absolute), new Curver("sweep", 16 + (i * 1.3f)));
                    base.Fire(new Direction(-UnityEngine.Random.Range(63, 81), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(1.1f, 4.2f), SpeedType.Absolute), new Curver("sweep", 20 + (i * 1.1f)));

                }
                float m = BraveUtility.RandomAngle();
                for (int f = 0; f < 5; f++)
                {
                    base.Fire(new Direction(m + (72 * f), DirectionType.Absolute, -1f), new Speed(2, SpeedType.Absolute), new SpeedChangingBullet("poundSmall", 7, 180));
                    base.Fire(new Direction(m + (72 * f) + 36, DirectionType.Absolute, -1f), new Speed(4, SpeedType.Absolute), new SpeedChangingBullet("poundSmall", 7, 180));

                }
                yield return Wait(20);

                base.PostWwiseEvent("Play_ENM_ironmaiden_blast_01", null);
                for (int i = 0; i < 6; i++)
                {
                    base.Fire(new Direction(UnityEngine.Random.Range(63, 81), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(5.1f, 7.2f), SpeedType.Absolute), new Curver("sweep", -12 - (i * 1.5f)));
                    base.Fire(new Direction(UnityEngine.Random.Range(63, 81), DirectionType.Aim, -1f), new Speed(4, SpeedType.Absolute), new Curver("sweep", -16 - (i * 1.3f)));
                    base.Fire(new Direction(UnityEngine.Random.Range(63, 81), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(1.1f, 4.2f), SpeedType.Absolute), new Curver("sweep", -20 - (i * 1.1f)));
                }
                m = BraveUtility.RandomAngle();
                for (int f = 0; f < 5; f++)
                {
                    base.Fire(new Direction(m + (72 * f), DirectionType.Absolute, -1f), new Speed(2, SpeedType.Absolute), new SpeedChangingBullet("poundSmall", 7, 180));
                    base.Fire(new Direction(m + (72 * f) + 36, DirectionType.Absolute, -1f), new Speed(4, SpeedType.Absolute), new SpeedChangingBullet("poundSmall", 7, 180));
                }
                yield break;
            }
            public class Curver : Bullet
            {
                private float CurveAmountPerSecond;
                public Curver(string Bullet, float curveAmountPerSecond) : base(Bullet, false, false, false)
                {
                    CurveAmountPerSecond = curveAmountPerSecond / 60;
                }
                public override IEnumerator Top()
                {
                    this.ChangeSpeed(new Brave.BulletScript.Speed(UnityEngine.Random.Range(6.25f, 9.25f)), 240);

                    while (Projectile)
                    {
                        this.ChangeDirection(new Brave.BulletScript.Direction(CurveAmountPerSecond, DirectionType.Relative));
                        yield return Wait(1);
                    }
                    yield break;
                }
            }
        }

        public class Attack_No: Script
        {
            public override IEnumerator Top()
            {
                float m = BraveUtility.RandomAngle();
                base.PostWwiseEvent("Play_ENM_ironmaiden_blast_01", null);
                for (int i = 0; i < 8; i++)
                {
                    for (int f = 0; f < 8; f++)
                    {
                        base.Fire(new Direction(m + (45 * i) + (f * 7.5f), DirectionType.Absolute, -1f), new Speed(3.5f - (f * 0.333f), SpeedType.Absolute), new SpeedChangingBullet("sweep", 11, 180));
                    }
                }
                yield break;
            }

        }



        public class Attack_Sexo : Script
        {
            public override IEnumerator Top()
            {

                if (!base.BulletBank.aiActor.healthHaver.IsDead && base.BulletBank.aiActor != null && base.BulletBank.aiActor.healthHaver.GetCurrentHealth() >= 1)
                {
                    yield return this.Wait(105);
                    float i = 1;
                    float aim = this.BulletBank.aiActor.aiAnimator.FacingDirection;
                    for (; ; )
                    {

                        var m = Mathf.PingPong(i / 28f, 1);
                        float m_ =  1 - m;


                        if (i % 5 == 0)
                        {
                            float Speed = Mathf.Min(i * 0.25f, 18);
                            base.Fire(new Direction(aim + UnityEngine.Random.Range(60, 66), DirectionType.Absolute, -1f), new Speed(Speed, SpeedType.Absolute), new SpeedChanger("sweep"));
                            base.Fire(new Direction(aim - UnityEngine.Random.Range(60, 66), DirectionType.Absolute, -1f), new Speed(Speed, SpeedType.Absolute), new SpeedChanger("sweep"));

                            
                            base.Fire(new Direction(aim + Mathf.Lerp(60, 135, m), DirectionType.Absolute, -1f), new Speed(Speed * 0.625f, SpeedType.Absolute), new SpeedChanger("sweep"));
                            base.Fire(new Direction(aim + Mathf.Lerp(60, 210, m_), DirectionType.Absolute, -1f), new Speed(Speed * 0.75f, SpeedType.Absolute), new SpeedChanger("sweep"));

                            base.Fire(new Direction(aim + Mathf.Lerp(-60, -135, m), DirectionType.Absolute, -1f), new Speed(Speed * 0.625f, SpeedType.Absolute), new SpeedChanger("sweep"));
                            base.Fire(new Direction(aim + Mathf.Lerp(-60, -210, m_), DirectionType.Absolute, -1f), new Speed(Speed * 0.75f, SpeedType.Absolute), new SpeedChanger("sweep"));
                        }
                        if (i % 15 == 0)
                        {
                            base.PostWwiseEvent("Play_ENM_ironmaiden_blast_01", null);
                        }
                        if (i > 180)
                        {
                            if (i % 60 == 1)
                            {
                                base.PostWwiseEvent("Play_BOSS_lichC_zap_01", null);
                                base.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(7, SpeedType.Absolute), new SpeedChanger("poundSmall"));
                                base.Fire(new Direction(1.5f, DirectionType.Aim, -1f), new Speed(6, SpeedType.Absolute), new SpeedChanger("poundSmall"));
                                base.Fire(new Direction(-1.5f, DirectionType.Aim, -1f), new Speed(6, SpeedType.Absolute), new SpeedChanger("poundSmall"));
                                base.Fire(new Direction(3, DirectionType.Aim, -1f), new Speed(5, SpeedType.Absolute), new SpeedChanger("poundSmall"));
                                base.Fire(new Direction(-3, DirectionType.Aim, -1f), new Speed(5, SpeedType.Absolute), new SpeedChanger("poundSmall"));

                            }
                        }


                            i++;
                        yield return this.Wait(1);
                    }
                }
                yield break;
            }

            public class SpeedChanger : Bullet
            {
                public SpeedChanger(string ullet) : base(ullet, false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    yield return this.Wait(60);
                    base.ChangeSpeed(new Speed(4f, SpeedType.Absolute), 30);
                    yield return this.Wait(90);
                    base.ChangeSpeed(new Speed(16f, SpeedType.Absolute), 30);
                    yield break;
                }
            }
        }



        public class RevenantHide : BraveBehaviour
		{
            public bool isaHider = false;
			private bool isFading = false;

            private Material OldMaterial;
            private Material OldMaterial_2;

            private void Start()
            {
                if (isaHider)
                {
                    OldMaterial = this.aiActor.sprite.renderer.material;

                    Material matShader = new Material(Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit"));
                    matShader.mainTexture = this.aiActor.sprite.renderer.material.mainTexture;
                    matShader.SetFloat("_Fade", 1);
                    OldMaterial_2 = matShader;
                    this.aiActor.sprite.renderer.material = matShader;
                    this.behaviorSpeculator.InterruptAndDisable();
                    this.aiActor.sprite.renderer.material.SetFloat("_Fade", 0.1f);
                    this.specRigidbody.CollideWithOthers = false;
                    this.aiActor.PreventBlackPhantom = true;
                }
            }
            private ActorState actorState = ActorState.Inactive;

            public void Update()
			{

                if (isaHider)
                {
                    if (actorState != base.aiActor.State)
                    {
                        actorState = base.aiActor.State;
                        if (actorState == ActorState.Normal)
                        {
                            this.behaviorSpeculator.InterruptAndDisable();
                            this.specRigidbody.CollideWithOthers = false;
                        }
                    }
                    if (base.aiActor.State == ActorState.Normal)
                    {
                        if (this.aiActor.IsBlackPhantom)
                        {
                            this.aiActor.UnbecomeBlackPhantom();
                            this.aiActor.sprite.renderer.material = OldMaterial_2;
                        }
                        if (isFading == true)
                        {

                            this.behaviorSpeculator.InterruptAndDisable();
                            this.aiActor.sprite.renderer.material.SetFloat("_Fade", 0.1f);
                            this.specRigidbody.CollideWithOthers = false;

                            var room = base.aiActor.parentRoom;
                            if (room != null)
                            {
                                int amount = 1;
                                base.aiActor.IgnoreForRoomClear = ShouldCountForRoomProgress(room, ref amount);
                                if (amount == 0)
                                {
                                    isaHider = false;
                                    this.StartCoroutine(DoAppear());
                                }
                            }
                            else
                            {
                                isaHider = false;
                                this.StartCoroutine(DoAppear());
                            }
                        }
                        if (isFading == false)
                        {
                            if (this.aiActor.parentRoom != null && this.aiActor.parentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear).Count() > 1)
                            {
                                isFading = true;
                            }
                            else if (this.aiActor.parentRoom == null)
                            {
                                isFading = true;
                            }

                        }

                    }
                }

                
			}
			private IEnumerator DoAppear()
			{
                //SND_ENM_beholster_teleport_02
                base.aiActor.IgnoreForRoomClear = false;

                float e = 0;
                if (UnityEngine.Random.value > 0.2f)
                {
                    while (e < 1)
                    {
                        e += Time.deltaTime * 3f;
                        this.aiActor.sprite.renderer.material.SetFloat("_Fade", Mathf.Lerp(0.2f, 0, e));
                        yield return null;
                    }
                    this.aiActor.healthHaver.SuppressDeathSounds = true;
                    Destroy(this.aiActor.gameObject);
                    yield break;
                }

                AkSoundEngine.PostEvent("Play_ENM_beholster_teleport_01", this.gameObject);
                AkSoundEngine.PostEvent("Play_VO_lichA_cackle_01", this.gameObject);

                while (e < 1)
				{
                    ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = BraveUtility.RandomVector2(this.aiActor.sprite.WorldBottomLeft, this.aiActor.sprite.WorldTopRight),
                        startLifetime = 0.5f,
                    });
                    e += Time.deltaTime * 2f;
					this.aiActor.sprite.renderer.material.SetFloat("_Fade", Mathf.Lerp(0.2f, 1, e));
					yield return null;
				}
                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = this.aiActor.sprite.WorldCenter,
                    startColor = Color.red.WithAlpha(0.33f),
                    startLifetime = 0.5f,
                    startSize = 32
                });
                base.aiActor.behaviorSpeculator.CooldownScale *= 2;
                base.aiActor.healthHaver.SetHealthMaximum(base.aiActor.healthHaver.GetMaxHealth() * 0.6f, null, true);
                base.aiActor.procedurallyOutlined = true;
                base.aiActor.SetOutlines(true);
                this.behaviorSpeculator.enabled = true;
                this.aiActor.sprite.renderer.material = OldMaterial;
                this.specRigidbody.CollideWithOthers = true;
                OldMaterial = null;
                yield break;
			}

            private bool ShouldCountForRoomProgress(RoomHandler handler, ref int amount)
            {
                List<AIActor> EnemyList = GetTheseActiveEnemies(handler, RoomHandler.ActiveEnemyType.RoomClear);
                EnemyList.RemoveAll(self => self.EnemyGuid == this.aiActor.EnemyGuid);
                amount = EnemyList.Count;

                if (handler.remainingReinforcementLayers == null) { return false; }

                bool remainingReinforcements = true;
                if (handler.remainingReinforcementLayers.Count == 0) { remainingReinforcements = false; }




                if (EnemyList.Count == 0 && remainingReinforcements == true) { return false; }
                if (EnemyList.Count > 0 && remainingReinforcements == true) { return true; }


                if (EnemyList.Count > 0 && remainingReinforcements == false) { return false; }
                if (EnemyList.Count == 0 && remainingReinforcements == false) { return false; }
                return false;
            }
            public List<AIActor> GetTheseActiveEnemies(RoomHandler room, RoomHandler.ActiveEnemyType type)
            {
                var outList = new List<AIActor>();
                if (room.activeEnemies == null)
                {
                    return outList;
                }
                if (type == RoomHandler.ActiveEnemyType.RoomClear)
                {
                    for (int i = 0; i < room.activeEnemies.Count; i++)
                    {
                        if (!room.activeEnemies[i].IgnoreForRoomClear)
                        {
                            outList.Add(room.activeEnemies[i]);
                        }
                    }
                }
                else
                {
                    outList.AddRange(room.activeEnemies);
                }
                return outList;
            }



        }
    }
}





