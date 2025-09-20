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
using DirectionType = DirectionalAnimation.DirectionType;
using static DirectionalAnimation;

namespace Planetside
{
	class Shamber : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "shamber_psog";



		public static void Init()
		{
            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ShamberCollection").GetComponent<tk2dSpriteCollectionData>();
            Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("shamber material");

            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {


				prefab = EnemyBuilder.BuildPrefabBundle("Shamber", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(0, 0), false, true);
				var enemy = prefab.AddComponent<EnemyBehavior>();




                enemy.sprite.usesOverrideMaterial = true;
                mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat.mainTexture = enemy.aiActor.sprite.renderer.material.mainTexture;
                mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
                mat.SetFloat("_EmissivePower", 40);
                mat.SetFloat("_EmissiveThresholdSensitivity", 1f);
                mat.SetFloat("_EmissiveColorPower", 2f);
                enemy.sprite.renderer.material = mat;

                var shamebr = prefab.AddComponent<ShamberController>();
                //prefab.AddComponent<KillOnRoomClear>();


                var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("ShamberParticles"));//this is the name of the object which by default will be "Particle System"
                partObj.transform.parent = enemy.transform;
                partObj.transform.position = enemy.aiActor.sprite.WorldCenter + new Vector2(0, -0.25f);
				shamebr.particle = partObj.GetComponent<ParticleSystem>();

                enemy.aiActor.IgnoreForRoomClear = true;
				enemy.aiActor.knockbackDoer.weight = 35;
				enemy.aiActor.MovementSpeed = 0.9f;
				enemy.aiActor.healthHaver.PreventAllDamage = false;
				enemy.aiActor.CollisionDamage = 0f;
				enemy.aiActor.aiAnimator.HitReactChance = 0f;
				enemy.aiActor.specRigidbody.CollideWithOthers = true;
				enemy.aiActor.specRigidbody.CollideWithTileMap = true;
				enemy.aiActor.PreventFallingInPitsEver = false;
				enemy.aiActor.healthHaver.ForceSetCurrentHealth(50f);
				enemy.aiActor.CollisionKnockbackStrength = 10f;
				enemy.aiActor.CanTargetPlayers = true;
				enemy.aiActor.healthHaver.SetHealthMaximum(50f, null, false);
				enemy.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;
				enemy.aiActor.SetIsFlying(true, "Gamemode: Creative", true, true);

				enemy.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("4db03291a12144d69fe940d5a01de376").ShadowObject;
				enemy.aiActor.HasShadow = true;

				AIAnimator aiAnimator = enemy.aiAnimator;


                var h = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ShamberAnimation").GetComponent<tk2dSpriteAnimation>();
                enemy.aiActor.spriteAnimator.Library = h;
                enemy.aiActor.spriteAnimator.library = h;
                enemy.aiActor.aiAnimator.spriteAnimator = enemy.aiActor.spriteAnimator;
                aiAnimator.IdleAnimation = Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "idle", new string[] { "idle" }, new DirectionalAnimation.FlipType[1]);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "death",
                    new string[] { "death" },
                    new DirectionalAnimation.FlipType[1]);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "awaken",
                    new string[] { "awaken" },
                    new DirectionalAnimation.FlipType[1]);



                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "fadein",
                    new string[] { "fadein" },
                    new DirectionalAnimation.FlipType[1]);


                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "fadeout",
                    new string[] { "fadeout" },
                    new DirectionalAnimation.FlipType[1]);


                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "vomit",
                    new string[] { "vomit" },
                    new DirectionalAnimation.FlipType[1]);


                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "expel",
                    new string[] { "expel" },
                    new DirectionalAnimation.FlipType[1]);


                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "fast",
                    new string[] { "fast" },
                    new DirectionalAnimation.FlipType[1]);


                enemy.aiActor.AwakenAnimType = AwakenAnimationType.Spawn;



				var intro = enemy.spriteAnimator.GetClipByName("fadein");
				intro.frames[1].eventInfo = "turnontrail";
				intro.frames[1].triggerEvent = true;

				var clip1 = enemy.spriteAnimator.GetClipByName("fadeout");
				clip1.frames[8].eventInfo = "turnofftrail";
				clip1.frames[8].triggerEvent = true;

                enemy.spriteAnimator.GetClipByName("fadeout").frames[3].eventAudio = "Play_VO_gorgun_laugh_01";
                enemy.spriteAnimator.GetClipByName("fadeout").frames[3].triggerEvent = true;
                enemy.spriteAnimator.GetClipByName("death").frames[1].eventAudio = "Play_BOSS_doormimic_vanish_01";
                enemy.spriteAnimator.GetClipByName("death").frames[1].triggerEvent = true;


				enemy.aiActor.specRigidbody.PixelColliders.Clear();
				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 10,
					ManualOffsetY = 0,
					ManualWidth = 13,
                    ManualHeight = 18,
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
					ManualOffsetX = 10,
					ManualOffsetY = 0,
					ManualWidth = 13,
					ManualHeight = 18,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});

				enemy.aiActor.PreventBlackPhantom = false;

				var shootpoint = new GameObject("fuck");
				shootpoint.transform.parent = enemy.transform;
				shootpoint.transform.position = new Vector3(0,0);

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

				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new TeleportBehavior()
				{

					AttackableDuringAnimation = true,
					AllowCrossRoomTeleportation = false,
					teleportRequiresTransparency = false,
					hasOutlinesDuringAnim = true,
					ManuallyDefineRoom = false,
					MaxHealthThreshold = 1f,
					StayOnScreen = true,
					AvoidWalls = false,
					GoneTime = 1f,
					OnlyTeleportIfPlayerUnreachable = false,
					MinDistanceFromPlayer = 4.25f,
					MaxDistanceFromPlayer = -1f,
					teleportInAnim = "fadein",
					teleportOutAnim = "fadeout",
					AttackCooldown = 0f,
					InitialCooldown = 4f,
					RequiresLineOfSight = false,
					roomMax = new Vector2(0,0),
					roomMin = new Vector2(0,0),
					GlobalCooldown = 0.5f,
					Cooldown = 5.5f,
					
					CooldownVariance = 1.5f,
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
				}
				};

				
				bs.MovementBehaviors = new List<MovementBehaviorBase>
				{

				new MoveErraticallyBehavior
				{
				   PointReachedPauseTime = 2f,
					PathInterval = 0.4f,
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
				Game.Enemies.Add("psog:shamber", enemy.aiActor);

                enemy.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));


                SpriteBuilder.AddSpriteToCollection(Collection.GetSpriteDefinition("shamber_idle_001"), SpriteBuilder.ammonomiconCollection);
				if (enemy.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
				}
				enemy.encounterTrackable = enemy.gameObject.AddComponent<EncounterTrackable>();
				enemy.encounterTrackable.journalData = new JournalEntry();
				enemy.encounterTrackable.EncounterGuid = "psog:shamber";
				enemy.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				enemy.encounterTrackable.journalData.SuppressKnownState = false;
				enemy.encounterTrackable.journalData.IsEnemy = true;
				enemy.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				enemy.encounterTrackable.ProxyEncounterGuid = "";
				enemy.encounterTrackable.journalData.AmmonomiconSprite = "shamber_idle_001";
				enemy.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("shamberammonomicoenrtytab");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\shamberammonomicoenrtytab.png");
                PlanetsideModule.Strings.Enemies.Set("#SHAMBER", "Shamber");
				PlanetsideModule.Strings.Enemies.Set("#SHAMBER_SHORT", "Tee Hee Hee!");
				PlanetsideModule.Strings.Enemies.Set("#SHAMBER_LONGDESC", "A lively, hungry spirit that feeds itself on bullets of any kind.\n\nIn the past, Shambers would seek out Gungeoneers and loot their ammo supplies until they realised that it was much easier to just catch bullets from mid-air without the hassle of dealing with a surprised Gungeoneer.");
				enemy.encounterTrackable.journalData.PrimaryDisplayName = "#SHAMBER";
				enemy.encounterTrackable.journalData.NotificationPanelDescription = "#SHAMBER_SHORT";
				enemy.encounterTrackable.journalData.AmmonomiconFullEntry = "#SHAMBER_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(enemy.gameObject, "psog:shamber");
				EnemyDatabase.GetEntry("psog:shamber").ForcedPositionInAmmonomicon = 27;
				EnemyDatabase.GetEntry("psog:shamber").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:shamber").isNormalEnemy = true;
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
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom && GameManager.Instance.PrimaryPlayer.IsInCombat == true)
				{
					base.aiActor.HasBeenEngaged = true;
					//ShamberController.BulletsEaten = 0;
				}
				yield break;
			}
			private void Start()
			{
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) => {  };
			}
		}
	}
}

