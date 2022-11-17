using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
//using DirectionType = DirectionalAnimation.DirectionType;
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
	public class Glockulus : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "glockulus";
		//private static tk2dSpriteCollectionData GlockulusCollection;
		public static GameObject shootpoint;
		public static void Init()
		{
			Glockulus.BuildPrefab();
		}

		public static void BuildPrefab()
		{
            //

            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("GlockulusCollection").GetComponent<tk2dSpriteCollectionData>();
            Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("glockulus material");

            bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefabBundle("Glockulus", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
                EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, Collection, mat);

                companion.aiActor.knockbackDoer.weight = 50;
				companion.aiActor.MovementSpeed = 0.8f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(22.5f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.HasShadow = true;
				companion.aiActor.SetIsFlying(true, "Gamemode: Creative");
				companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;

				companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject; 
				companion.aiActor.healthHaver.SetHealthMaximum(22.5f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				
				ImprovedAfterImage image = companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>();
				image.dashColor = new Color(1, 0.85f, 0.7f);
				image.spawnShadows = true;

				//255, 210, 178, 255)
				AfterImageTrailController im = companion.aiActor.gameObject.AddComponent<AfterImageTrailController>();
				im.spawnShadows = false;
                companion.aiActor.gameObject.GetOrAddComponent<tk2dSpriteAttachPoint>();
                companion.aiActor.gameObject.GetOrAddComponent<ObjectVisibilityManager>();
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
					ManualHeight = 20,
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
					ManualHeight = 20,
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
				aiAnimator.MoveAnimation = new DirectionalAnimation
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

				

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "charge", new string[] { "charge_back",
						"charge_back_right",
						"charge_front_right",
						"charge_front",
						"charge_front_left",
						"charge_back_left"}, new DirectionalAnimation.FlipType[6], DirectionalAnimation.DirectionType.SixWay);

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "attack", new string[] { "attack_back",
						"attack_back_right",
						"attack_front_right",
						"attack_front",
						"attack_front_left",
						"attack_back_left"}, new DirectionalAnimation.FlipType[6], DirectionalAnimation.DirectionType.SixWay);


                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[1]);
                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[0]);

				List<int> idle_front = new List<int>()
				{
					0,
					1,
					2,
					3
				};
				List<int> idle_front_left = new List<int>()
				{
					4,
					5,
					6,
					7
				};
				List<int> idle_front_right = new List<int>()
				{
					8,
					9,
					10,
					11
				};
				List<int> idle_back = new List<int>()
				{
				 	12,
					13,
					14,
					15
				};
				List<int> idle_back_left = new List<int>()
				{
					16,
					17,
					18,
					19
				};
				List<int> idle_back_right = new List<int>()
				{
					20,
					21,
					22,
					23
				};


				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;

				//bool flag3 = GlockulusCollection == null;
				//if (flag3)
				{
					/*
					GlockulusCollection = SpriteBuilder.ConstructCollection(prefab, "Glockulus_Collection");
					UnityEngine.Object.DontDestroyOnLoad(GlockulusCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], GlockulusCollection);
					}
					*/
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_front, "idle_front", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_front_left, "idle_front_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_front_right, "idle_front_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_back, "idle_back", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_back_left, "idle_back_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_back_right, "idle_back_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;


					//=====================
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_front, "charge_front", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_front_left, "charge_front_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_front_right, "charge_front_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_back, "charge_back", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_back_left, "charge_back_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_back_right, "charge_back_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;


					//=====================

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_front, "attack_front", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_front_left, "attack_front_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_front_right, "attack_front_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_back, "attack_back", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_back_left, "attack_back_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, idle_back_right, "attack_back_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;

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
					 32

					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					0
					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5;
                    EnemyToolbox.MarkAnimationAsSpawn(companion.gameObject.GetComponent<tk2dSpriteAnimator>(), "awaken");


                }

                EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_back", new Dictionary<int, string> { { 0, "Charge" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_back_right", new Dictionary<int, string> { { 0, "Charge" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_front_right", new Dictionary<int, string> { { 0, "Charge" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_front", new Dictionary<int, string> { { 0, "Charge" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_front_left", new Dictionary<int, string> { { 0, "Charge" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_back_left", new Dictionary<int, string> { { 0, "Charge" } });

				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack_back", new Dictionary<int, string> { { 0, "Play_ENM_cult_spew_01" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack_back_right", new Dictionary<int, string> { { 0, "Play_ENM_cult_spew_01" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack_front_right", new Dictionary<int, string> { { 0, "Play_ENM_cult_spew_01" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack_front", new Dictionary<int, string> { { 0, "Play_ENM_cult_spew_01" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack_front_left", new Dictionary<int, string> { { 0, "Play_ENM_cult_spew_01" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack_back_left", new Dictionary<int, string> { { 0, "Play_ENM_cult_spew_01" } });

				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Play_BOSS_doormimic_charge_01" }, { 3, "Play_BOSS_doormimic_eyes_01" } });


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
				bs.MovementBehaviors = new List<MovementBehaviorBase>() {
				new SeekTargetBehavior() {
					StopWhenInRange = true,
					CustomRange = 4,
					LineOfSight = true,
					ReturnToSpawn = true,
					SpawnTetherDistance = 0,
					PathInterval = 0.5f,
					SpecifyRange = false,
					MinActiveRange = 1,
					MaxActiveRange = 10
				} };
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(NormalAttack)),
					LeadAmount = 0f,
					AttackCooldown = 0f,
					Cooldown = 2f,
					InitialCooldown = 0.5f,
					TellAnimation = "charge",
					PostFireAnimation = "attack",

					//FireAnimation = "tell",
					RequiresLineOfSight = true,
					MultipleFireEvents = true,
					//EnabledDuringAttack = new PowderSkullSpinBulletsBehavior(),
					//StopDuring = ShootBehavior.StopType.Attack,
					Uninterruptible = false,


				},
				new CustomDashBehavior()
				{
					//dashAnim = "wail",
					ShootPoint = m_CachedGunAttachPoint,
					dashDistance = 7f,
					dashTime = 0.33f,
					AmountOfDashes = 1,
					enableShadowTrail = false,
					Cooldown = 1,
					dashDirection = DashBehavior.DashDirection.Random,
					warpDashAnimLength = true,
					hideShadow = true,
					fireAtDashStart = true,
					InitialCooldown = 1f,
					AttackCooldown = 3,

					bulletScript = new CustomBulletScriptSelector(typeof(DashAttack)),
					
					RequiresLineOfSight = false,
					//Uninterruptible = true,
					//FireVfx = ,
					//ChargeVfx = ,
					//	MoveSpeedModifier = 0f,
				}//GlobalCooldown = 0.5f,
				};
				
			
				/*
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBeamBehavior() {
					//ShootPoint = m_CachedGunAttachPoint,

					firingTime = 3f,
					stopWhileFiring = true,
					//beam
					//BulletScript = new CustomBulletScriptSelector(typeof(SkellScript)),
					//LeadAmount = 0f,
					AttackCooldown = 5f,
					//InitialCooldown = 4f,
					RequiresLineOfSight = true,
					//StopDuring = ShootBehavior.StopType.Attack,
					//Uninterruptible = true,

				}
				};
				*/				
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;

                Material mat2 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat2.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
                mat2.SetColor("_EmissiveColor", new Color32(255, 210, 178, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                mat2.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                companion.sprite.renderer.material = mat2;

                Game.Enemies.Add("psog:glockulus", companion.aiActor);




				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Glockulus/glockulus_idle_front1", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:glockulus";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Glockulus/glockulus_idle_front1";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("glockulussheet");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\glockulussheet.png");
                PlanetsideModule.Strings.Enemies.Set("#THE_GLOCKULUS", "Glockulus");
				PlanetsideModule.Strings.Enemies.Set("#THE_GLOCKULUS_SHORTDESC", "Eye Spy");
				PlanetsideModule.Strings.Enemies.Set("#THE_GLOCKULUS_LONGDESC", "A sentinel from beyond the Curtain, it watches over the Gungeon for any... suspicious activity.\n\nSome Gungeonologists speculate that Glockuli are an off-breed of the Beholster, but no evidence has pointed towards that.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#THE_GLOCKULUS";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#THE_GLOCKULUS_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#THE_GLOCKULUS_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:glockulus");
				EnemyDatabase.GetEntry("psog:glockulus").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:glockulus").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:glockulus").isNormalEnemy = true;
			}
		}



		private static string[] spritePaths = new string[]
		{
			//Front
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_front1.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_front2.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_front3.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_front4.png",
			//Front left
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_front_left1.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_front_left2.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_front_left3.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_front_left4.png",
			//Front right
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_front_right1.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_front_right2.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_front_right3.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_front_right4.png",
			//Back
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_back1.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_back2.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_back3.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_back4.png",
			//Back left
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_back_left1.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_back_left2.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_back_left3.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_back_left4.png",
			//back right
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_back_right1.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_back_right2.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_back_right3.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_idle_back_right4.png",
			//Death
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_001.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_002.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_003.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_004.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_005.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_006.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_007.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_008.png",
			"Planetside/Resources/Enemies/Glockulus/glockulus_die_009.png",

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
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.bulletBank.GetBullet("homing"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));


				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{ 	
				  AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Bite_01", base.aiActor.gameObject);


				};
				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Charge"))
				{
					//GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.SecretRoomDoorImpactVFX);
					//gameObject.transform.position = base.aiActor.sprite.WorldCenter;
					 base.aiActor.PlayEffectOnActor(StaticVFXStorage.MachoBraceBurstVFX, new Vector3(0f, 1f, 0f), false, false, false);

					AkSoundEngine.PostEvent("Play_ENM_book_charge_01", base.aiActor.gameObject);

					//StaticVFXStorage.HighPriestClapVFX.SpawnAtPosition(base.aiActor.sprite.WorldCenter, 0, base.aiActor.gameObject.transform.Find("fuck").gameObject.transform);
				}
			}
		}

		public class NormalAttack : Script 
		{
			protected override IEnumerator Top() 
			{
				base.PostWwiseEvent("Play_WPN_eyeballgun_shot_01", null);
				base.PostWwiseEvent("Play_WPN_eyeballgun_impact_01", null);
				for (int i = -2; i <= 2; i++)
				{
					this.Fire(new Direction(i * 20, DirectionType.Aim, -1f), new Speed(9f, SpeedType.Absolute), new SpitNormal());
					this.Fire(new Direction(i * 20, DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new SpitNormal());
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
		public class DashAttack : Script 
		{
			protected override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				base.PostWwiseEvent("Play_WPN_eyeballgun_shot_01", null);
				base.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(2.5f, SpeedType.Absolute), new Spit());
				yield break;
			}
		}
		public class Spit : Bullet
		{
			public Spit() : base("sniper", false, false, false)
			{

			}
			protected override IEnumerator Top()
			{

				base.ChangeSpeed(new Speed(13f, SpeedType.Absolute), 30);
				yield break;
			}
		}
	}
}





