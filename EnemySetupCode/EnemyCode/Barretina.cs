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
	public class Barretina : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "barretina";
		private static tk2dSpriteCollectionData BarretinaCollection;
		public static GameObject shootpoint;
		public static void Init()
		{
			Barretina.BuildPrefab();
		}


		public static void BuildPrefab()
		{
			
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Barretina", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false, true);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 200;
				companion.aiActor.MovementSpeed = 0.8f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(40f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.SetIsFlying(true, "Gamemode: Creative");
				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(1f, 0.25f), "shadowPos");

				companion.aiActor.healthHaver.SetHealthMaximum(40f, null, false);
				companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;

				ImprovedAfterImage image = companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>();
				image.dashColor = new Color(1, 0.85f, 0.7f);
				image.spawnShadows = true;

				//255, 210, 178, 255)
				AfterImageTrailController im = companion.aiActor.gameObject.AddComponent<AfterImageTrailController>();
				im.spawnShadows = false;
				companion.aiActor.gameObject.AddComponent<tk2dSpriteAttachPoint>();
				companion.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();

				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 3,
					ManualOffsetY = 10,
					ManualWidth = 27,
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
					ManualOffsetX = 3,
					ManualOffsetY = 10,
					ManualWidth = 27,
					ManualHeight = 22,
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
					Type = DirectionalAnimation.DirectionType.SixWay,
					Flipped = new DirectionalAnimation.FlipType[6],
					AnimNames = new string[]
					{
						"idle_back",
						"idle_back_right",
						"idle_front_right",
						"idle_front",
						"idle_front_left",
						"idle_back_left"

					}
				};

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[0]);

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "attack", new string[] {  
						"attack_back",
						"attack_back_right",
						"attack_front_right",
						"attack_front",
						"attack_front_left",
						"attack_back_left" }, new DirectionalAnimation.FlipType[6], DirectionalAnimation.DirectionType.SixWay);

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "charge", new string[] {
						"charge_back",
						"charge_back_right",
						"charge_front_right",
						"charge_front",
						"charge_front_left",
						"charge_back_left" }, new DirectionalAnimation.FlipType[6], DirectionalAnimation.DirectionType.SixWay);

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);




				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;

				List<int> idle_front = new List<int>()
				{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7
				};
				List<int> idle_front_left = new List<int>()
				{
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				};
				List<int> idle_front_right = new List<int>()
				{
					16,
					17,
					18,
					19,
					20,
					21,
					22,
					23
				};
				List<int> idle_back = new List<int>()
				{
				   24,
				   25,
				   26,
				   27,
				   28,
				   29,
				   30,
				   31
				};
				List<int> idle_back_left = new List<int>()
				{
					32,
					33,
					34,
					35,
					36,
					37,
					38,
					39
				};
				List<int> idle_back_right = new List<int>()
				{
					40,
					41,
					42,
					43,
					44,
					45,
					46,
					47
				};

				bool flag3 = BarretinaCollection == null;
				if (flag3)
				{
					BarretinaCollection = SpriteBuilder.ConstructCollection(prefab, "Barretina_Collection");
					UnityEngine.Object.DontDestroyOnLoad(BarretinaCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], BarretinaCollection);
					}


					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_front, "idle_front", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_front_left, "idle_front_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_front_right, "idle_front_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_back, "idle_back", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_back_left, "idle_back_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_back_right, "idle_back_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_front, "charge_front", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_front_left, "charge_front_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_front_right, "charge_front_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_back, "charge_back", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_back_left, "charge_back_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_back_right, "charge_back_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_front, "attack_front", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_front_left, "attack_front_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_front_right, "attack_front_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_back, "attack_back", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_back_left, "attack_back_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, idle_back_right, "attack_back_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, new List<int>
					{
					 48,
					 49,
					 50,
					 51,
					 52,
					 53,
					 54,
					 55,
					 56,
					 57,
					 58,
					 59,
					 60,
					 61

					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, BarretinaCollection, new List<int>
					{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7
					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 19;

				}

				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_back", new Dictionary<int, string> { { 1, "Charge" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_back_right", new Dictionary<int, string> { { 1, "Charge" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_front_right", new Dictionary<int, string> { { 1, "Charge" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_front", new Dictionary<int, string> { { 1, "Charge" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_front_left", new Dictionary<int, string> { { 1, "Charge" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_back_left", new Dictionary<int, string> { { 1, "Charge" } });

				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack_back", new Dictionary<int, string> { { 0, "Play_ENM_cult_spew_01" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack_back_right", new Dictionary<int, string> { { 0, "Play_ENM_cult_spew_01" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack_front_right", new Dictionary<int, string> { { 0, "Play_ENM_cult_spew_01" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack_front", new Dictionary<int, string> { { 0, "Play_ENM_cult_spew_01" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack_front_left", new Dictionary<int, string> { { 0, "Play_ENM_cult_spew_01" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack_back_left", new Dictionary<int, string> { { 0, "Play_ENM_cult_spew_01" } });

				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Play_BOSS_doormimic_charge_01" }, { 4, "Play_BOSS_doormimic_appear_01" } });


				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				shootpoint = new GameObject("fuck");
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = companion.sprite.WorldCenter;
				GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;
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
				/*
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(NormalAttack)),
					LeadAmount = 0f,
					AttackCooldown = 2f,
					Cooldown = 4f,
					InitialCooldown = 0.5f,
					RequiresLineOfSight = true,
					MultipleFireEvents = false,
					Uninterruptible = true,
					ChargeAnimation = "charge",
					PostFireAnimation = "attack"

				},
				*/


                bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
                {
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.5f,
                    Behavior =   new ShootBehavior() {
                    ShootPoint = m_CachedGunAttachPoint,
                    BulletScript = new CustomBulletScriptSelector(typeof(SpecialAttack)),
                    LeadAmount = 0f,
                    AttackCooldown = 1.5f,
                    Cooldown = 6f,
                    InitialCooldown = 2f,
                    RequiresLineOfSight = true,
                    MultipleFireEvents = false,
                    Uninterruptible = true,
                    ChargeAnimation = "charge",
                    PostFireAnimation = "attack"
                    },
					},
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.8f,
                    Behavior =   new ShootBehavior() {
                    ShootPoint = m_CachedGunAttachPoint,
                    BulletScript = new CustomBulletScriptSelector(typeof(NormalAttack)),
                    LeadAmount = 0f,
                    AttackCooldown = 1.5f,
                    Cooldown = 3f,
                    InitialCooldown = 0.5f,
                    RequiresLineOfSight = true,
                    MultipleFireEvents = false,
                    Uninterruptible = true,
                    ChargeAnimation = "charge",
                    PostFireAnimation = "attack"
                    },
                    },
                     new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1f,
                    Behavior =    new CustomDashBehavior()
                {
                    ShootPoint = m_CachedGunAttachPoint,
                    dashDistance = 8f,
                    dashTime = 0.75f,
                    AmountOfDashes = 2,
                    enableShadowTrail = false,
                    Cooldown = 4f,
                    dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
                    warpDashAnimLength = true,
                    hideShadow = true,
                    fireAtDashStart = true,
                    InitialCooldown = 1f,
                    bulletScript = new CustomBulletScriptSelector(typeof(DashAttack)),
                    RequiresLineOfSight = false,
                    AttackCooldown = 0.1f,
                
                    },
                    },
                };


				/*
                new ShootBehavior() {
                    ShootPoint = m_CachedGunAttachPoint,
                    BulletScript = new CustomBulletScriptSelector(typeof(SpecialAttack)),
                    LeadAmount = 0f,
                    AttackCooldown = 3f,
                    Cooldown = 6f,
                    InitialCooldown = 0.5f,
                    RequiresLineOfSight = true,
                    MultipleFireEvents = false,
                    Uninterruptible = true,
                    ChargeAnimation = "charge",
                    PostFireAnimation = "attack"

                },
                new CustomDashBehavior()
				{
					ShootPoint = m_CachedGunAttachPoint,
					dashDistance = 8f,
					dashTime = 0.75f,
					AmountOfDashes = 2,
					enableShadowTrail = false,
					Cooldown = 3f,
					dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
					warpDashAnimLength = true,
					hideShadow = true,
					fireAtDashStart = true,
					InitialCooldown = 1f,
					bulletScript = new CustomBulletScriptSelector(typeof(DashAttack)),
					RequiresLineOfSight = false,
					AttackCooldown = 0.1f,
				}
				};

				*/
				bs.MovementBehaviors = new List<MovementBehaviorBase>() {
				new SeekTargetBehavior() {
					StopWhenInRange = true,
					CustomRange = 6,
					LineOfSight = true,
					ReturnToSpawn = true,
					SpawnTetherDistance = 0,
					PathInterval = 0.5f,
					SpecifyRange = false,
					MinActiveRange = 1,
					MaxActiveRange = 10
				} };



                bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;

                Game.Enemies.Add("psog:barretina", companion.aiActor);

                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Berretina/berretina_idle_south_001", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:barretina";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Berretina/berretina_idle_south_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("barretinaiconthing");// ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\barretinaiconthing.png");
                PlanetsideModule.Strings.Enemies.Set("#THE_BARRETINA", "Barretina");
				PlanetsideModule.Strings.Enemies.Set("#THE_BARRETINA_SHORTDESC", "Sn-eye-per");
				PlanetsideModule.Strings.Enemies.Set("#THE_BARRETINA_LONGDESC", "A sentinel of greater size from beyond the Curtain, blessed with the gift of greater manueverability.\n\nTheories circulated that these foes were a sign of something greater to be discovered in the Gungeon, but again, no evidence supports that... yet.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#THE_BARRETINA";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#THE_BARRETINA_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#THE_BARRETINA_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.aiActor.gameObject, "psog:barretina");
				EnemyDatabase.GetEntry("psog:barretina").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:barretina").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:barretina").isNormalEnemy = true;


                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));
				
            }

        }




		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/Berretina/berretina_idle_south_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_south_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_south_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_south_004.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_south_005.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_south_006.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_south_007.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_south_008.png",

			"Planetside/Resources/Enemies/Berretina/berretina_idle_southleft_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southleft_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southleft_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southleft_004.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southleft_005.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southleft_006.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southleft_007.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southleft_008.png",

			"Planetside/Resources/Enemies/Berretina/berretina_idle_southright_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southright_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southright_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southright_004.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southright_005.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southright_006.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southright_007.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_southright_008.png",

			"Planetside/Resources/Enemies/Berretina/berretina_idle_north_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_north_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_north_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_north_004.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_north_005.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_north_006.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_north_007.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_north_008.png",

			"Planetside/Resources/Enemies/Berretina/berretina_idle_northleft_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northleft_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northleft_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northleft_004.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northleft_005.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northleft_006.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northleft_007.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northleft_008.png",

			"Planetside/Resources/Enemies/Berretina/berretina_idle_northright_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northright_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northright_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northright_004.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northright_005.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northright_006.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northright_007.png",
			"Planetside/Resources/Enemies/Berretina/berretina_idle_northright_008.png",

			"Planetside/Resources/Enemies/Berretina/berretina_die_001.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_002.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_003.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_004.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_005.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_006.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_007.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_008.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_009.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_010.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_011.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_012.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_013.png",
			"Planetside/Resources/Enemies/Berretina/berretina_die_014.png",


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
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{ 
				  AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Bite_01", base.aiActor.gameObject);
				};
				if (!base.aiActor.IsBlackPhantom)
				{
					Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
					mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
					mat.SetColor("_EmissiveColor", new Color32(255, 210, 178, 255));
					mat.SetFloat("_EmissiveColorPower", 1.55f);
					mat.SetFloat("_EmissivePower", 100);
					mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);

					aiActor.sprite.renderer.material = mat;

				}
				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Charge"))
				{
					StaticVFXStorage.HighPriestClapVFX.SpawnAtPosition(base.aiActor.sprite.WorldCenter, 0, base.aiActor.gameObject.transform.Find("fuck").gameObject.transform);
				}
			}
		}

        public class SpecialAttack : Script
        {
            protected override IEnumerator Top()
            {

                base.PostWwiseEvent("Play_ENM_gunknight_shockwave_01", null);
                base.PostWwiseEvent("Play_ENM_gunknight_shockwave_01", null);

                this.Fire(new Direction(150, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Gordo(-180));
                this.Fire(new Direction(100, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Gordo(-120));
                this.Fire(new Direction(50, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Gordo(-60));
                this.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Gordo(0));
                this.Fire(new Direction(-50, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Gordo(60));
                this.Fire(new Direction(-100, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Gordo(120));
                this.Fire(new Direction(-150, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Gordo(180));



                yield break;
            }
            public class Gordo : Bullet
            {
                public Gordo(float tMius = 0) : base("bigBullet", false, false, false)
                {
                    t = tMius;
                }
                protected override IEnumerator Top()
                {
                    yield return base.Wait(20);
                    base.ChangeSpeed(new Speed(20f, SpeedType.Absolute), 120);
                    base.ChangeDirection(new Brave.BulletScript.Direction(t, DirectionType.Relative), 105);

                    int i = 0;
                    while (this.Projectile)
                    {
                        i++;
                        yield return base.Wait(1);
                        if (i == 4)
                        {
                            i = 0;
                            this.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Dissipate());
                        }
                        yield return null;
                    }
                    yield break;
                }
                private float t;
            }
            public class Dissipate : Bullet
            {
                public Dissipate() : base("frogger", false, false, false)
                {

                }
                protected override IEnumerator Top()
                {
                    yield return base.Wait((Mathf.Max(180, BraveUtility.RandomAngle()) / 2.25f));
                    base.Vanish(false);
                    yield break;
                }
            }
        }



        public class NormalAttack : Script 
		{
			protected override IEnumerator Top() 
			{

				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));

				}
				base.PostWwiseEvent("Play_WPN_eyeballgun_shot_01", null);
				for (int i = -4; i <= 5; i++)
				{
					if (i != 0) { 
						this.Fire(new Direction(i * 20, DirectionType.Aim, -1f), new Speed(9f, SpeedType.Absolute), new SpitNormal());
						this.Fire(new Direction(i * 20, DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new SpitNormal());
					}
				}

                for (int i = 0; i <= 5; i++)
				{
                    this.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(3f + i, SpeedType.Absolute), new SpitLarge());
                }
				yield break;
			}
		}


		public class SpitNormal : Bullet
		{
			public SpitNormal() : base("frogger", false, false, false)
			{

			}
		}
		public class SpitLarge : Bullet
		{
			public SpitLarge() : base("sniper", false, false, false)
			{
			}
			protected override IEnumerator Top()
			{
				yield return base.Wait(20);
				base.ChangeSpeed(new Speed(20f, SpeedType.Absolute), 20);
				yield break;
			}
		}
		public class DashAttack : Script 
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				base.PostWwiseEvent("Play_WPN_eyeballgun_shot_01", null);
				for (int i = -1; i <= 1; i++)
				{
					base.Fire(new Direction(10*i, DirectionType.Aim, -1f), new Speed(2f, SpeedType.Absolute), new Spit());
				}
				//base.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(12f, SpeedType.Absolute), new SpitNormal());
				yield break;
			}
		}
		public class Spit : Bullet
		{
			public Spit() : base("frogger", false, false, false)
			{
			}
			protected override IEnumerator Top()
			{
				base.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 20);
				yield break;
			}
		}
	}
}





