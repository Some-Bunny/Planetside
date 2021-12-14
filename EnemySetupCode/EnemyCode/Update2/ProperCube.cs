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
	public class ProperCube : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "proper_cube";
		private static tk2dSpriteCollectionData ProperCubeColection;
		public static GameObject shootpoint;
		public static GameObject noVFX;
		public static void Init()
		{
			ProperCube.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Toddy Enemy", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				prefab.AddComponent<KillOnRoomClear>();
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
					ManualOffsetX = 0,
					ManualOffsetY = 0,
					ManualWidth = 20,
					ManualHeight = 24,
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
					ManualWidth = 20,
					ManualHeight = 24,
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
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "charge",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{

						   "charge_left",
						   "charge_right"

							}

						}
					}
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "dash",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{

						   "dash_left",
						   "dash_right"

							}

						}
					}
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "impact",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{

						   "impact_left",
						   "impact_right"
							}

						}
					}
				};

				bool flag3 = ProperCubeColection == null;
				if (flag3)
				{
					ProperCubeColection = SpriteBuilder.ConstructCollection(prefab, "Toddy_Collection");
					UnityEngine.Object.DontDestroyOnLoad(ProperCubeColection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], ProperCubeColection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ProperCubeColection, new List<int>
					{

					0,
					1,
					2,
					3,
					4

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ProperCubeColection, new List<int>
					{
					0,
					1,
					2,
					3,
					4


					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, ProperCubeColection, new List<int>
					{
					5,
					6,
					7,
					8


					}, "charge_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ProperCubeColection, new List<int>
					{
					5,
					6,
					7,
					8


					}, "charge_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, ProperCubeColection, new List<int>
					{
					9,
					20

					}, "dash_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ProperCubeColection, new List<int>
					{
					9,
					20
									

					}, "dash_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, ProperCubeColection, new List<int>
					{
					10,
					11,
					12


					}, "impact_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ProperCubeColection, new List<int>
					{
					10,
					11,
					12
					}, "impact_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, ProperCubeColection, new List<int>
					{

				 13,
				 14,
				 15,
				 16,
				 17,
				 18,
				 19




					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 19f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ProperCubeColection, new List<int>
					{

				 13,
				 14,
				 15,
				 16,
				 17,
				 18,
				 19

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 19f;

				}
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("impact_right").frames[1].eventAudio = "Play_BOSS_wall_slam_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("impact_right").frames[1].triggerEvent = true;
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("impact_left").frames[1].eventAudio = "Play_BOSS_wall_slam_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("impact_left").frames[1].triggerEvent = true;
				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				shootpoint = new GameObject("fuck");
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = companion.sprite.WorldCenter;
				GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"));



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
				new EpicDashbehav() {
					//CanChargeLeft = false,
					//CanChargeRight = false,
					primeAnim = "charge_left",
					chargeAnim = "dash_left",
					hitPlayerAnim = "impact_left",
					endWhenChargeAnimFinishes = false,
					hitAnim = "impact_left",
					switchCollidersOnCharge = false,
					chargeSpeed = 14,
					stopAtPits = true,
					AttackCooldown = 0,
					delayWallRecoil = true,
					chargeRange = 999,
					CanChargeDown = true,
					CanChargeUp = true,
					CanChargeLeft = false,
					CanChargeRight = false,

					//hitVfx = gameObject,
					//trailVfx = gameObject,
					//launchVfx = gameObject,
					//hitWallVfxString = null,
					//trailVfxParent = companion.transform,
					//trailVfxString = null,

					wallRecoilForce = 400,
					

				//trailVfxString = trail_{0},

			}
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
				Game.Enemies.Add("psog:youngling_cube", companion.aiActor);
				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/ProperCube/toddy_idle_001.png", SpriteBuilder.ammonomiconCollection);
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
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/ProperCube/toddy_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\bigcubeicon.png");
				PlanetsideModule.Strings.Enemies.Set("#TODDY", "Youngling Cube");
				PlanetsideModule.Strings.Enemies.Set("#TODDY_SHORTDESC", "Smash And Dash");
				PlanetsideModule.Strings.Enemies.Set("#TODDY_LONGDESC", "A very small, yet lively stone cube ripped from the walls of the earlier chambers of the Gungeon.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#TODDY";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#TODDY_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#TODDY_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:youngling_cube");
				EnemyDatabase.GetEntry("psog:youngling_cube").ForcedPositionInAmmonomicon = 85;
				EnemyDatabase.GetEntry("psog:youngling_cube").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:youngling_cube").isNormalEnemy = true;
				//EnemyBuilder.SetupEntry(companion.aiActor, "Hells Bells", "These urns of past Gundead can be seen scattered around the Gungeon, with Gungeonners showing little respect to the contents inside.", "Planetside/Resources/Ammocom/johan", "Planetside/Resources/Fodder/fodder_idle_001", "Fodder");
				/*
				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Fodder/fodder_idle_001.png", SpriteBuilder.ammonomiconCollection);
				//FOR BOSSES USE BOSS ICONS
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:fodder";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\johan.png");
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Fodder/fodder_idle_001.png";

				PlanetsideModule.Strings.Enemies.Set("#FODDER", "Fodder");
				PlanetsideModule.Strings.Enemies.Set("#FODDER_SHORTDESC", "Hells Bells");
				PlanetsideModule.Strings.Enemies.Set("#FODDER_LONGDESC", "Some Bunny will never add ammonomicon descriptions for enemi- PFFFFFFFFFFT");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#FODDER";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#FODDER_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#FODDER_LONGDESC";
				//EnemyBuilder.AddEnemyToDatabase2(companion.gameObject, "psog:fodder", true, true);
				EnemyDatabase.GetEntry("psog:fodder").ForcedPositionInAmmonomicon = 10000;
				EnemyDatabase.GetEntry("psog:fodder").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:fodder").isNormalEnemy = true;
				*/

			}
		}



		private static string[] spritePaths = new string[]
		{
			//idle
			"Planetside/Resources/Enemies/ProperCube/toddy_idle_001.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_idle_002.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_idle_003.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_idle_004.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_idle_005.png",
			//prime
			"Planetside/Resources/Enemies/ProperCube/toddy_charge_001.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_charge_002.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_charge_003.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_charge_004.png",
			//charge
			"Planetside/Resources/Enemies/ProperCube/toddy_dash_001.png",
			//hit
			"Planetside/Resources/Enemies/ProperCube/toddy_impact_001.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_impact_002.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_impact_003.png",

			//death
			"Planetside/Resources/Enemies/ProperCube/toddy_die_001.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_die_002.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_die_003.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_die_004.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_die_005.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_die_006.png",
			"Planetside/Resources/Enemies/ProperCube/toddy_die_007.png",

						"Planetside/Resources/Enemies/ProperCube/toddy_dash_002.png",


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
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					AkSoundEngine.PostEvent("Play_OBJ_rock_break_01", base.aiActor.gameObject);
					LootEngine.DoDefaultItemPoof(base.aiActor.sprite.WorldCenter, false, true);
				};
			}

		}
	}
}





