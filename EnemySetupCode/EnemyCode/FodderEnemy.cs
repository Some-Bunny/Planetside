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

namespace Planetside
{
	public class FodderEnemy : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "fodder_enemy";
		//private static tk2dSpriteCollectionData FodderColection;
		public static GameObject shootpoint;
		public static void Init()
		{
			FodderEnemy.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Fodder Enemy", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(0,0), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 100000;
				companion.aiActor.MovementSpeed = 0f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = true;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(2f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.IgnoreForRoomClear = true;

				companion.gameObject.GetOrAddComponent<TeleportationImmunity>();

				companion.aiActor.healthHaver.SetHealthMaximum(2f, null, false);
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
					ManualWidth = 14,
					ManualHeight = 20,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				//companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "die", new string[] { "die" }, new DirectionalAnimation.FlipType[1]);
				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
					{
						"idle",
						"idle"
					}
				};


                tk2dSpriteCollectionData FodderColection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("FodderCollection").GetComponent<tk2dSpriteCollectionData>();
                Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("assets/Enemies/Fodder/FodderCollection Data/atlas0 material.mat");
				EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, FodderColection, mat);
             			
                {
                SpriteBuilder.AddAnimation(companion.spriteAnimator, FodderColection, new List<int>
                {
                0,
                1,
                2,
                3,
                4,
                5,
                6,
                7
                }, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;

                SpriteBuilder.AddAnimation(companion.spriteAnimator, FodderColection, new List<int>
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
                //17
                }, "die", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;
                }
				


                var bs = prefab.GetComponent<BehaviorSpeculator>();
				var OvM =  prefab.GetComponent<ObjectVisibilityManager>();

				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
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
				Game.Enemies.Add("psog:fodder", companion.aiActor);


				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Fodder/fodder_idle_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
                Material mat2 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat2.mainTexture = companion.sprite.renderer.material.mainTexture;
                mat2.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                mat2.SetFloat("_EmissiveThresholdSensitivity", 0.05f);

                companion.sprite.renderer.material = mat2;


                companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:fodder";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Fodder/fodder_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("fodderammonomiconentry");
                //StaticSpriteDefinitions.Amooncomicon_Enemy_Sheet_Data.spriteDefinitions[11].material.mainTexture
                PlanetsideModule.Strings.Enemies.Set("#THE_FODDER", "Fodder");
				PlanetsideModule.Strings.Enemies.Set("#THE_FODDER_SHORTDESC", "Hells Bells");
				PlanetsideModule.Strings.Enemies.Set("#THE_FODDER_LONGDESC", "Filled with the remnants of lively, yet deceased Gundead, though Gungeoneers often have little respect for Gundead practices.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#THE_FODDER";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#THE_FODDER_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#THE_FODDER_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:fodder");
				EnemyDatabase.GetEntry("psog:fodder").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:fodder").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:fodder").isNormalEnemy = true;
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

			"Planetside/Resources/Fodder/fodder_idle_001.png",
			"Planetside/Resources/Fodder/fodder_idle_002.png",
			"Planetside/Resources/Fodder/fodder_idle_003.png",
			"Planetside/Resources/Fodder/fodder_idle_004.png",
			"Planetside/Resources/Fodder/fodder_idle_005.png",
			"Planetside/Resources/Fodder/fodder_idle_006.png",
			"Planetside/Resources/Fodder/fodder_idle_007.png",
			"Planetside/Resources/Fodder/fodder_idle_008.png",
			//death
			"Planetside/Resources/Fodder/fodder_die_001.png",
			"Planetside/Resources/Fodder/fodder_die_002.png",
			"Planetside/Resources/Fodder/fodder_die_003.png",
			"Planetside/Resources/Fodder/fodder_die_004.png",
			"Planetside/Resources/Fodder/fodder_die_005.png",
			"Planetside/Resources/Fodder/fodder_die_006.png",
			"Planetside/Resources/Fodder/fodder_die_007.png",
			"Planetside/Resources/Fodder/fodder_die_008.png",
			"Planetside/Resources/Fodder/fodder_die_009.png",
			"Planetside/Resources/Fodder/fodder_die_010.png",


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
				/*
                Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(this.aiActor.sprite);
                if (outlineMaterial1 != null)
				{
                    outlineMaterial1.SetColor("_OverrideColor", new Color(10f, 10f, 42f));
                }
				else
				{
					ETGModConsole.Log(outlineMaterial1 + "is NULL");
				}
				*/

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
				this.aiActor.knockbackDoer.SetImmobile(true, "IM A BELL.");
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));

				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{ 
				  AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Fade_01", base.aiActor.gameObject);
					AkSoundEngine.PostEvent("Play_BOSS_mineflayer_belldrop_01", base.aiActor.gameObject);

				};


            }

		}

		public class EatPants : Script 
		{
			protected override IEnumerator Top()
			{
				for (int i = 0; i <= 12; i++)
				{
					this.Fire(new Direction(i * 30, DirectionType.Aim, -1f), new Speed(1f, SpeedType.Absolute), new SkellBullet());
				}
				yield break;
			}
		}


		public class SkellBullet : Bullet
		{
			public SkellBullet() : base("sweep", false, false, false){}
			protected override IEnumerator Top()
			{
				base.ChangeSpeed(new Speed(20f, SpeedType.Absolute), 60);
				yield break;
			}
		}

	}
}





