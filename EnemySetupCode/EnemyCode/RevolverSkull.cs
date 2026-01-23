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
	public class RevolverSkull : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "skullvenant";
		//private static tk2dSpriteCollectionData SkullVenantCollection;
		//public static GameObject shootpoint;
		public static void Init()
		{
			RevolverSkull.BuildPrefab();
		}

		public static void BuildPrefab()
		{

            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("SkullvenantCollection").GetComponent<tk2dSpriteCollectionData>();
            Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("skullvenant material");

            bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefabBundle("Skullvenant", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(8, 9),  false);
				var companion = prefab.AddComponent<EnemyBehavior>();
                EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, Collection, mat);

                Alexandria.ItemAPI.AlexandriaTags.SetTag(companion.aiActor, "skeleton");

                companion.aiActor.knockbackDoer.weight = 50;
				companion.aiActor.MovementSpeed = 2f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = false;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(35f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.SetIsFlying(true, "Gamemode: Creative");
				companion.aiActor.healthHaver.SetHealthMaximum(35f, null, false);
				companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;

                EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.defaultShadow, new Vector2(0.5f, 0.125f), "shadowPos");
                
				mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
                mat.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
                mat.SetFloat("_EmissiveColorPower", 1.55f);
                mat.SetFloat("_EmissivePower", 100);
                companion.sprite.renderer.material = mat;

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
					ManualWidth = 15,
					ManualHeight = 15,
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
					ManualHeight = 15,
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
					Type = DirectionalAnimation.DirectionType.EightWayOrdinal,
					Flipped = new DirectionalAnimation.FlipType[8],
					AnimNames = new string[]
					{
						"idle_north",
					   "idle_north_east",
						"idle_east",
					   "idle_south_east",
					   "idle_south",
						"idle_south_west",
					   "idle_west",
						"idle_north_west",


					}
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.EightWayOrdinal,
					Flipped = new DirectionalAnimation.FlipType[8],
					AnimNames = new string[]
					{
						"idle_north",
					   "idle_north_east",
						"idle_east",
					   "idle_south_east",
					   "idle_south",
						"idle_south_west",
					   "idle_west",
						"idle_north_west",
					}
				};
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
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "attack",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.EightWayOrdinal,
							Flipped = new DirectionalAnimation.FlipType[8],
							AnimNames = new string[]
							{
						"idle_north",
					   "idle_north_east",
						"idle_east",
					   "idle_south_east",
					   "idle_south",
						"idle_south_west",
					   "idle_west",
						"idle_north_west",
							}

						}
					}
				};
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);
				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				//bool flag3 = SkullVenantCollection == null;
				//if (flag3)
				{
					/*
					SkullVenantCollection = SpriteBuilder.ConstructCollection(prefab, "Skullvenant_Collection");
					UnityEngine.Object.DontDestroyOnLoad(SkullVenantCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], SkullVenantCollection);
					}
					*/

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
		            0
					}, "idle_north", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					2
					}, "idle_north_east", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					1
					}, "idle_north_west", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
		            3
					}, "idle_south", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
		            5
					}, "idle_south_east", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
		            4
					}, "idle_south_west", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					7
					}, "idle_east", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
		            6
					}, "idle_west", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
		             8

					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
                     8

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					0
					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 4;


				}
				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				var shootpoint = new GameObject("fuck");
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = companion.sprite.WorldTopCenter;
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
				bs.OtherBehaviors = new List<BehaviorBase>() {
				new CustomSpinBulletsBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					OverrideBulletName = "sweep",
				NumBullets = 7,
					BulletMinRadius = 1,
					BulletMaxRadius = 4,
					BulletCircleSpeed = 105,
					BulletsIgnoreTiles = true,
					RegenTimer = 0.25f,
					AmountOFLines = 3,
				}
				};
				bs.MovementBehaviors = new List<MovementBehaviorBase>
			{
				new MoveErraticallyBehavior
				{
                   PointReachedPauseTime = 0.1f,
					PathInterval = 0.2f,		
					PreventFiringWhileMoving = false,
					StayOnScreen = false,
					AvoidTarget = true,
					

				}
			};
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(SkullScript)),
					LeadAmount = 0f,
					AttackCooldown = 0f,
					Cooldown = 5f,
					RequiresLineOfSight = true,
					MultipleFireEvents = true,
					Uninterruptible = false,


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
				//BehaviorSpeculator load = EnemyDatabase.GetOrLoadByGuid("6e972cd3b11e4b429b888b488e308551").behaviorSpeculator;
				//Tools.DebugInformation(load);

				companion.healthHaver.spawnBulletScript = true;
				companion.healthHaver.chanceToSpawnBulletScript = 1f;
				companion.healthHaver.bulletScriptType = HealthHaver.BulletScriptType.OnPreDeath;
				companion.healthHaver.bulletScript = new CustomBulletScriptSelector(typeof(EatPants));
				
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:skullvenant", companion.aiActor);


				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/RevolverSkull/revolverskull_idle_front_001", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:skullvenant";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/RevolverSkull/revolverskull_idle_front_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("skullvenanticonaoo");// ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\skullvenanticonaoo.png");
				PlanetsideModule.Strings.Enemies.Set("#THE_SKULLVENANT", "Skullvenant");
				PlanetsideModule.Strings.Enemies.Set("#THE_SKULLVENANT_SHORTDESC", "Headache");
				PlanetsideModule.Strings.Enemies.Set("#THE_SKULLVENANT_LONGDESC", "The remnants of slain Revolvenants, these heads have been given life once more to continue on as servants to the Gungeon and its Master.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#THE_SKULLVENANT";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#THE_SKULLVENANT_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#THE_SKULLVENANT_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:skullvenant");
				EnemyDatabase.GetEntry("psog:skullvenant").ForcedPositionInAmmonomicon = 45;
				EnemyDatabase.GetEntry("psog:skullvenant").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:skullvenant").isNormalEnemy = true;

                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.bulletBank.GetBullet("homing"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));
            }
		}



		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/RevolverSkull/revolverskull_idle_back_001.png",
			"Planetside/Resources/Enemies/RevolverSkull/revolverskull_idle_back_left_001.png",
			"Planetside/Resources/Enemies/RevolverSkull/revolverskull_idle_back_right_001.png",
			"Planetside/Resources/Enemies/RevolverSkull/revolverskull_idle_front_001.png",
			"Planetside/Resources/Enemies/RevolverSkull/revolverskull_idle_front_left_001.png",
			"Planetside/Resources/Enemies/RevolverSkull/revolverskull_idle_front_right_001.png",
			"Planetside/Resources/Enemies/RevolverSkull/revolverskull_idle_left_001.png",
			"Planetside/Resources/Enemies/RevolverSkull/revolverskull_idle_right_001.png",

			//death
			"Planetside/Resources/Enemies/RevolverSkull/revolverskull_die_001.png",



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

				};
			}

		}

		public class EatPants : Script 
		{
			public override IEnumerator Top()
			{
				base.PostWwiseEvent("Play_ENM_ironmaiden_blast_01", null);
				for (int i = 0; i <= 5; i++)
				{
					this.Fire(new Direction(i * 72, DirectionType.Aim, -1f), new Speed(1f, SpeedType.Absolute), new SkellBullet());
                    this.Fire(new Direction(i * 72, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new SkellBullet());

                }
                yield return this.Wait(10);
				base.PostWwiseEvent("Play_ENM_ironmaiden_blast_01", null);
				for (int i = 0; i <= 5; i++)
				{
					this.Fire(new Direction((i * 72)+36, DirectionType.Aim, -1f), new Speed(2f, SpeedType.Absolute), new SkellBullet());
                    this.Fire(new Direction((i * 72) + 36, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new SkellBullet());

                }
                yield break;
			}
		}


		public class SkellBullet : Bullet
		{
			public SkellBullet() : base("sweep", false, false, false)
			{

			}
			public override IEnumerator Top()
			{
				base.ChangeSpeed(new Speed(1f, SpeedType.Absolute), 20);
				yield return this.Wait(60);
				base.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 40);
				yield break;
			}
		}
		public class SkullScript : Script 
		{
			public override IEnumerator Top() 
			{
				base.PostWwiseEvent("Play_WPN_woodbow_shot_02", null);
				base.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(3f, SpeedType.Absolute), new Bullet("homing", false, false, false));
                for (int i = -2; i <= 3; i++)
                {
                    this.Fire(new Direction((i * 24), DirectionType.Aim, -1f), new Speed(2f, SpeedType.Absolute), new SkellBullet());
                    this.Fire(new Direction((i * 24), DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new SkellBullet());
                }
                yield break;
			}
		}
		public class SkullShot : Bullet
		{
			public SkullShot() : base("homing", false, false, false)
			{
			}
		}
	}
}





