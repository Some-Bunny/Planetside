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
using static AkSoundEngine;
using HarmonyLib;
using static Planetside.ProperCube.TrapperCubeBehaviour;
using Alexandria.PrefabAPI;
using Planetside.Static_Storage;
using UnityEngine.Playables;



namespace Planetside
{
	public class ProperCube : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "proper_cube";


        public static void Init(bool North, bool South, bool East, bool West, string AdditionalNameAddon = null, bool isDupe = false)
		{
            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ProperCubeCollection").GetComponent<tk2dSpriteCollectionData>();

            string Myguid = guid;
            if (AdditionalNameAddon != null)
            {
                Myguid += AdditionalNameAddon;
            }
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(Myguid))
            {
				prefab = EnemyBuilder.BuildPrefabBundle("Trapper Cube", Myguid, Collection, 68, new IntVector2(0, 0), new IntVector2(8, 9),  false);
				
                var companion = prefab.GetComponent<AIActor>();
				var trapper = prefab.AddComponent<TrapperCubeBehaviour>();
                Alexandria.ItemAPI.AlexandriaTags.SetTag(companion.aiActor, "sliding_cube");

                companion.AddComponent<TeleportationImmunity>();

                if (!isDupe)
                {
                    var UpDown = PrefabBuilder.BuildObject("TrapperCube Dirt UpDown");
                    UpDown.layer = 20;
                    var sprite = UpDown.AddComponent<tk2dSprite>();
                    sprite.SortingOrder = 2;
                    sprite.HeightOffGround = -1.75f;
                    sprite.IsPerpendicular = false;
                    var animator = UpDown.AddComponent<tk2dSpriteAnimator>();
                    animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
                    animator.playAutomatically = true;
                    animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("dirt_updown");
                    sprite.usesOverrideMaterial = true;
                    Material mat = new Material(StaticShaders.Default_Shader);
                    sprite.renderer.material = mat;
                    Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog_dirt_updown", UpDown);

                    var LeftRight = PrefabBuilder.BuildObject("TrapperCube Dirt LeftRight");
                    LeftRight.layer = 20;
                    sprite = LeftRight.AddComponent<tk2dSprite>();
                    sprite.SortingOrder = 2;
                    sprite.HeightOffGround = -1.75f;
                    sprite.IsPerpendicular = false;
                    animator = LeftRight.AddComponent<tk2dSpriteAnimator>();
                    animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
                    animator.playAutomatically = true;
                    animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("dirt_leftright");
                    sprite.usesOverrideMaterial = true;
                    mat = new Material(StaticShaders.Default_Shader);
                    sprite.renderer.material = mat;
                    Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog_dirt_leftright", LeftRight);

                    var Mid = PrefabBuilder.BuildObject("TrapperCube Dirt M");
                    Mid.layer = 20;
                    sprite = Mid.AddComponent<tk2dSprite>();
                    sprite.IsPerpendicular = false;
                    sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "trappercube_dirt_001");
                    sprite.usesOverrideMaterial = true;
                    mat = new Material(StaticShaders.Default_Shader);
                    sprite.renderer.material = mat;
                    Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog_dirt_m", Mid);

                }



                //EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, Collection, mat, false);
                var h = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ProperCubeAnimation_Proper").GetComponent<tk2dSpriteAnimation>();
                var h1 = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ProperCubeAnimation_Hollow").GetComponent<tk2dSpriteAnimation>();
                var h2 = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ProperCubeAnimation_Forge").GetComponent<tk2dSpriteAnimation>();

				EnemyToolbox.AddSoundsToAnimationFrame(h, "charge", new Dictionary<int, string>() { { 0, "Play_OBJ_chalice_clank_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h1, "charge", new Dictionary<int, string>() { { 0, "Play_OBJ_chalice_clank_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h2, "charge", new Dictionary<int, string>() { { 0, "Play_OBJ_chalice_clank_01" } });

                EnemyToolbox.AddEventTriggersToAnimation(h, "charge", new Dictionary<int, string>() { { 0, "DoWoke" } });
                EnemyToolbox.AddEventTriggersToAnimation(h1, "charge", new Dictionary<int, string>() { { 0, "DoWoke" } });
                EnemyToolbox.AddEventTriggersToAnimation(h2, "charge", new Dictionary<int, string>() { { 0, "DoWoke" } });

                EnemyToolbox.AddSoundsToAnimationFrame(h, "impact_north", new Dictionary<int, string>() { { 0, "Play_BOSS_wall_slam_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h1, "impact_north", new Dictionary<int, string>() { { 0, "Play_BOSS_wall_slam_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h2, "impact_north", new Dictionary<int, string>() { { 0, "Play_BOSS_wall_slam_01" } });

                EnemyToolbox.AddSoundsToAnimationFrame(h, "impact_south", new Dictionary<int, string>() { { 0, "Play_BOSS_wall_slam_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h1, "impact_south", new Dictionary<int, string>() { { 0, "Play_BOSS_wall_slam_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h2, "impact_south", new Dictionary<int, string>() { { 0, "Play_BOSS_wall_slam_01" } });

                EnemyToolbox.AddSoundsToAnimationFrame(h, "impact_east", new Dictionary<int, string>() { { 0, "Play_BOSS_wall_slam_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h1, "impact_east", new Dictionary<int, string>() { { 0, "Play_BOSS_wall_slam_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h2, "impact_east", new Dictionary<int, string>() { { 0, "Play_BOSS_wall_slam_01" } });

                EnemyToolbox.AddSoundsToAnimationFrame(h, "impact_west", new Dictionary<int, string>() { { 0, "Play_BOSS_wall_slam_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h1, "impact_west", new Dictionary<int, string>() { { 0, "Play_BOSS_wall_slam_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h2, "impact_west", new Dictionary<int, string>() { { 0, "Play_BOSS_wall_slam_01" } });



                EnemyToolbox.AddEventTriggersToAnimation(h, "impact_north", new Dictionary<int, string>() { { 0, "HitWall" } });
                EnemyToolbox.AddEventTriggersToAnimation(h1, "impact_north", new Dictionary<int, string>() { { 0, "HitWall" } });
                EnemyToolbox.AddEventTriggersToAnimation(h2, "impact_north", new Dictionary<int, string>() { { 0, "HitWall" } });

                EnemyToolbox.AddEventTriggersToAnimation(h, "impact_south", new Dictionary<int, string>() { { 0, "HitWall" } });
                EnemyToolbox.AddEventTriggersToAnimation(h1, "impact_south", new Dictionary<int, string>() { { 0, "HitWall" } });
                EnemyToolbox.AddEventTriggersToAnimation(h2, "impact_south", new Dictionary<int, string>() { { 0, "HitWall" } });

                EnemyToolbox.AddEventTriggersToAnimation(h, "impact_east", new Dictionary<int, string>() { { 0, "HitWall" } });
                EnemyToolbox.AddEventTriggersToAnimation(h1, "impact_east", new Dictionary<int, string>() { { 0, "HitWall" } });
                EnemyToolbox.AddEventTriggersToAnimation(h2, "impact_east", new Dictionary<int, string>() { { 0, "HitWall" } });

                EnemyToolbox.AddEventTriggersToAnimation(h, "impact_west", new Dictionary<int, string>() { { 0, "HitWall" } });
                EnemyToolbox.AddEventTriggersToAnimation(h1, "impact_west", new Dictionary<int, string>() { { 0, "HitWall" } });
                EnemyToolbox.AddEventTriggersToAnimation(h2, "impact_west", new Dictionary<int, string>() { { 0, "HitWall" } });



                EnemyToolbox.AddSoundsToAnimationFrame(h, "death", new Dictionary<int, string>() { { 1, "Play_ENM_rock_blast_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h1, "death", new Dictionary<int, string>() { { 1, "Play_ENM_rock_blast_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h2, "death", new Dictionary<int, string>() { { 1, "Play_ENM_rock_blast_01" } });

                EnemyToolbox.AddEventTriggersToAnimation(h, "death", new Dictionary<int, string>() { { 1, "Boom" } });
                EnemyToolbox.AddEventTriggersToAnimation(h1, "death", new Dictionary<int, string>() { { 1, "Boom" } });
                EnemyToolbox.AddEventTriggersToAnimation(h2, "death", new Dictionary<int, string>() { { 1, "Boom" } });


                #region Variants
                cubeVariants.Add(
				new CubeVariant(new GlobalDungeonData.ValidTilesets[] 
				{
					GlobalDungeonData.ValidTilesets.GUNGEON
				},
				h, 
				15, 
				3, 
				"FireParticle_BG", 
				new ParticleSystem.EmitParams() 
				{
                    startLifetime = 0.5f,
                    velocity = Vector3.up * 0.5f,
                }, 
				12,
				new IntVector2(45, 210)));

               cubeVariants.Add(
               new CubeVariant(new GlobalDungeonData.ValidTilesets[]
               {
                    GlobalDungeonData.ValidTilesets.CATACOMBGEON
               },
               h1,
               10,
               3,
               "DarkMagics_BG",
               new ParticleSystem.EmitParams()
               {
                   startLifetime = 2f,
                   velocity = Vector3.up * 0.1f,
				   startColor = Color.cyan,
                   startSize = 0.3125f
               },
               5,
               new IntVector2(45, 315)));

               cubeVariants.Add(
               new CubeVariant(new GlobalDungeonData.ValidTilesets[]
               {
                    GlobalDungeonData.ValidTilesets.FORGEGEON
               },
               h2,
               6,
               2,
               "DarkMagics_BG",
               new ParticleSystem.EmitParams()
               {
                   startLifetime = 1f,
                   velocity = Vector3.up * 0.5f,
				   startSize = 0.25f,
				   startColor = Color.gray
               },
               32,
               new IntVector2(75, 450)));
				#endregion
                companion.gameObject.layer = 22;
                companion.sprite.SortingOrder = 2;


                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;


                prefab.AddComponent<KillOnRoomClear>();
				companion.knockbackDoer.knockbackMultiplier = 0;

                companion.aiActor.knockbackDoer.weight = 800;
				companion.aiActor.MovementSpeed = 0f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = true;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(20f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.healthHaver.SetHealthMaximum(20f, null, false);
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
					ManualHeight = 16,
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
                    ManualHeight = 16,
                    ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").CorpseObject;
				AIAnimator aiAnimator = companion.aiAnimator;



				aiAnimator.IdleAnimation = Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "idle", new string[] { "idle" }, new DirectionalAnimation.FlipType[1]);

				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>() { };

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
					aiAnimator, "death",
					new string[] { "death" },
					new DirectionalAnimation.FlipType[1]);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
					   aiAnimator, "charge",
					   new string[] { "charge" },
					   new DirectionalAnimation.FlipType[1]);
				
				Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
						aiAnimator, "dash",
						new string[] { "dash" },
						new DirectionalAnimation.FlipType[1]);

				Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
						aiAnimator, "impact_north",
						new string[] { "impact_north" },
						new DirectionalAnimation.FlipType[1]);
				Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
						aiAnimator, "impact_south",
						new string[] { "impact_south" },
						new DirectionalAnimation.FlipType[1]);
				Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
						aiAnimator, "impact_east",
						new string[] { "impact_east" },
						new DirectionalAnimation.FlipType[1]);

				Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
					aiAnimator, "impact_west",
					new string[] { "impact_west" },
					new DirectionalAnimation.FlipType[1]);
                

                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[] {DirectionalAnimation.FlipType.None}, DirectionalAnimation.DirectionType.Single);
				companion.aiActor.AwakenAnimType = AwakenAnimationType.Spawn;




                var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
                GameObject shootpoint = new GameObject("shootPoint");
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = companion.sprite.WorldCenter;


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
				new SpecificDirectionDashBehaviour() {

					primeAnim = "charge",
					chargeAnim = "dash",
					
					ImpactAnimation_North = "impact_north",
                    ImpactAnimation_South = "impact_south",
                    ImpactAnimation_East = "impact_east",
                    ImpactAnimation_West = "impact_west",

					shootPoint = shootpoint,
                    endWhenChargeAnimFinishes = false,
					
					switchCollidersOnCharge = false,
					chargeSpeed = 14,
					stopAtPits = true,
					AttackCooldown = 0,
					delayWallRecoil = true,
					chargeRange = 999,
					CanChargeDown = South,
					CanChargeUp = North,
					CanChargeLeft = West,
					CanChargeRight = East,
					dashBulletScript = new CustomBulletScriptSelector(typeof(DashScript)),


					wallRecoilForce = 400,
					m_primeAnimTime = 1
				}
				};



                bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;

                string Name = "psog:youngling_cube";
                if (AdditionalNameAddon != null)
                {
                    Name += AdditionalNameAddon;
                }


                Game.Enemies.Add(Name, companion.aiActor);
				
                if (!isDupe)
                {
                    SpriteBuilder.AddSpriteToCollection(Collection.GetSpriteDefinition("trappercube_proper_idle_001"), SpriteBuilder.ammonomiconCollection);
                }

                if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:youngling_cube";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "trappercube_proper_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("bigcubeicon");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\bigcubeicon.png");
                PlanetsideModule.Strings.Enemies.Set("#TODDY", "Trapper Cube");
				PlanetsideModule.Strings.Enemies.Set("#TODDY_SHORTDESC", "Smash And Dash");
				PlanetsideModule.Strings.Enemies.Set("#TODDY_LONGDESC", "A chunk of the Gungeons brickwork, animated and carved with skulls. Leave gashes on the ground on they've travelled.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#TODDY";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#TODDY_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#TODDY_LONGDESC";
                if (!isDupe)
                {
                    EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:youngling_cube");
                }
                EnemyDatabase.GetEntry("psog:youngling_cube").ForcedPositionInAmmonomicon = 85;
				EnemyDatabase.GetEntry("psog:youngling_cube").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:youngling_cube").isNormalEnemy = true;

                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundLarge"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));


            }
        }



        public class DashScript : Script
        {
            public override IEnumerator Top()
            {
                base.EndOnBlank = false;
				var cube = base.BulletBank.aiActor.GetComponent<TrapperCubeBehaviour>();
                int ShotIntervalSmall= cube.MyVariant.ShotIntervals.x;
                int ShotIntervalLarge = cube.MyVariant.ShotIntervals.y;
				string BulletType = cube.MyVariant.largeBulletType;


                int ShotIntervalSmall_l = cube.MyVariant.bulletLingerTimes.x;
                int ShotIntervalLarge_l = cube.MyVariant.bulletLingerTimes.y;

                if (!base.BulletBank.aiActor.healthHaver.IsDead && base.BulletBank.aiActor != null && base.BulletBank.aiActor.healthHaver.GetCurrentHealth() >= 1)
                {
                    float i = 0;
                    for (; ; )
                    {
                        
						if (i % ShotIntervalLarge == 1)
						{
                            base.PostWwiseEvent("Play_Vertebreak_Shot", null);

                            var v = base.BulletBank.aiActor.Velocity.ToAngle() + 180;
                            base.Fire(new Direction(v, DirectionType.Absolute, -1f), new Speed(3, SpeedType.Absolute), new DelayedBullet(ShotIntervalLarge_l, BulletType));

                        }
                        if (i % ShotIntervalSmall == 1)
						{
                            base.Fire(new Direction(BraveUtility.RandomAngle(), DirectionType.Absolute, -1f), new Speed(2, SpeedType.Absolute), new DelayedBullet(ShotIntervalSmall_l, "poundSmall"));
                        }
						
                        yield return this.Wait(1f * PlayerStats.GetTotalEnemyProjectileSpeedMultiplier());
                        i++;
                    }
                }

            }
            public class DelayedBullet : Bullet
            {
                public DelayedBullet(float Wait, string bulletType) : base(bulletType, false, false, false)
                {
                    base.SuppressVfx = true;
                    this.Waittime = Wait;
                }
                public override IEnumerator Top()
                {
                    if (this.Projectile.spriteAnimator != null)
					{
                        this.Projectile.spriteAnimator.Play();
                    }
                    base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 30);
                    yield return this.Wait(Waittime);
					this.Vanish();
                    yield break;
                }
                private float Waittime;
            }

        }


        public class TrapperCubeBehaviour : BraveBehaviour
		{
			public class CubeVariant
			{
				public CubeVariant(GlobalDungeonData.ValidTilesets[] validTilesets, tk2dSpriteAnimation library, int Interval_LargeBullet, int Interval_SmallBullet,  string particle, ParticleSystem.EmitParams emitParams, float AmountOverTime, IntVector2 BulletLingerTimes, string LargeBulletType = "reversible")
				{
                    TileSets = validTilesets;
                    AnimationLibrary = library;
					ShotIntervals = new IntVector2(Interval_SmallBullet, Interval_LargeBullet);
					bulletLingerTimes = BulletLingerTimes;


                    sparks = particle;
                    EmitParams = emitParams;
					amountOverTime = AmountOverTime;
					largeBulletType = LargeBulletType; 
                }
				public GlobalDungeonData.ValidTilesets[] TileSets;
				public tk2dSpriteAnimation AnimationLibrary;
				public IntVector2 ShotIntervals;
                public IntVector2 bulletLingerTimes;

                public ParticleSystem.EmitParams EmitParams;
                public string sparks;
                public float amountOverTime;

                public string largeBulletType;

            }

			public static List<CubeVariant> cubeVariants = new List<CubeVariant>()
			{

			};
            public static CubeVariant Default = new CubeVariant(
				new GlobalDungeonData.ValidTilesets[0], 
				null, 
				6, 
				2,
                "FireParticle_BG", 
				new ParticleSystem.EmitParams() 
				{
					//startColor = Color.gray,
					startLifetime = 0.5f,
					velocity = Vector3.up * 0.5f,
					//startSize = 0.0625f
				},
				8,
				new IntVector2(45, 375)
				);


            public void Update()
			{
				if (MyVariant != null)
				{

                    if (UnityEngine.Random.value < MyVariant.amountOverTime * BraveTime.DeltaTime)
					{
                        var p = MyVariant.EmitParams;
                        p.position = this.aiActor.sprite.WorldCenter + new Vector2(0, 0.6875f) + BraveUtility.RandomVector2(new Vector2(-0.0625f, -0.0625f), new Vector2(0.0625f, 0.0625f));
						ParticleBase.EmitParticles(MyVariant.sparks, 1, p);

                    }
                }

                if (base.aiActor.spriteAnimator.currentClip != null && base.aiActor.spriteAnimator.currentClip.name == "dash")
				{

                    var v = base.aiActor.Velocity.normalized;
					var v_1 = new Vector2(base.aiActor.Velocity.y, base.aiActor.Velocity.x).normalized;

                    ParticleBase.EmitParticles("ChaffParticle_BG", 1, new ParticleSystem.EmitParams()
                    {
                        position = this.aiActor.sprite.WorldCenter + (v * 0.5f) + v_1 * UnityEngine.Random.Range(-0.5f, 0.5f),
                        startLifetime = (UnityEngine.Random.value * 0.625f) + 1.1f,
                        startColor = new Color(0.05f, 0.05f, 0.05f, 1),
                        startSize = 0.125f,
                    });
                }
            }

			private void Start()
			{
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					
				};
                DetermineVariant();

                if (MyVariant != null)
				{
					if (MyVariant.AnimationLibrary != null)
					{
                        this.aiActor.spriteAnimator.library = MyVariant.AnimationLibrary;
                        this.aiActor.spriteAnimator.Library = MyVariant.AnimationLibrary;
                    }
                }
                base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
            }

			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{

				if (clip.GetFrame(frameIdx).eventInfo.Contains("DoWoke"))
				{
                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = this.aiActor.sprite.WorldCenter + new Vector2(0.3125f, 0),
                        startLifetime = 0.25f,
                        startColor = new Color(1, 0, 0, 0.4f),
                        startSize = 4f,
                    });
                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = this.aiActor.sprite.WorldCenter - new Vector2(0.3125f, 0),
                        startLifetime = 0.25f,
                        startColor = new Color(1, 0, 0, 0.4f),
                        startSize = 4f,
                    });
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("HitWall"))
                {
                    var t =UnityEngine.Object.Instantiate(StaticVFXStorage.SeriousCannonImpact, this.aiActor.sprite.WorldCenter, Quaternion.identity);
                    Destroy(t, 2.5f);
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("Boom"))
                {
                    var t = UnityEngine.Object.Instantiate(StaticVFXStorage.BigShotgunExplosion, this.aiActor.sprite.WorldCenter, Quaternion.identity);
                    Destroy(t, 2.5f);
                }
            }

            private void DetermineVariant()
			{

                var t = cubeVariants.Where(self => self.TileSets.Contains(GameManager.Instance.Dungeon.tileIndices.tilesetId));
                if (t != null && t.Count() > 0)
				{
                    MyVariant = t.FirstOrDefault();
					return;
                }
                MyVariant = Default;
            }
			public CubeVariant MyVariant;

		}
	}
}





