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
using BreakAbleAPI;

namespace Planetside
{
	public class Observant : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "observant";
		private static tk2dSpriteCollectionData ObservantCollection;


		public static void Init()
		{
			Observant.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Observant", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false, true);
				var companion = prefab.AddComponent<EnemyBehavior>();
				prefab.AddComponent<ForgottenEnemyComponent>();
				companion.aiActor.knockbackDoer.weight = 120;
				companion.aiActor.MovementSpeed = 0.6f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(35f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.defaultShadow, new Vector2(0.625f, 0.25f), "shadowPos");


				companion.aiActor.healthHaver.SetHealthMaximum(35f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 5,
					ManualOffsetY = 5,
					ManualWidth = 12,
					ManualHeight = 23,
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
					ManualOffsetX = 5,
					ManualOffsetY = 5,
					ManualWidth = 12,
					ManualHeight = 23,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});
				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;

				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Prefix = "idle",
					AnimNames = new string[] { "idle_right", "idle_left"},
					Flipped = new DirectionalAnimation.FlipType[2]
				};

				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Prefix = "run",
					AnimNames = new string[] { "run_right", "run_left" },
					Flipped = new DirectionalAnimation.FlipType[2]
				};

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[0], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "fire", new string[] { "fire_right", "fire_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "charge", new string[] {"charge_right", "charge_left"}, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "overcharge", new string[] { "overcharge_right", "overcharge_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "disappear", new string[] { "disappear" }, new DirectionalAnimation.FlipType[0], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "reappear", new string[] { "reappear" }, new DirectionalAnimation.FlipType[0], DirectionalAnimation.DirectionType.Single);

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				companion.aiActor.reinforceType = ReinforceType.SkipVfx;
				Creationist.TrespassEnemyEngageDoer trespassEngager = companion.aiActor.gameObject.AddComponent<Creationist.TrespassEnemyEngageDoer>();


				bool flag3 = ObservantCollection == null;
				if (flag3)
				{
					ObservantCollection = SpriteBuilder.ConstructCollection(prefab, "ObservantCollection");
					UnityEngine.Object.DontDestroyOnLoad(ObservantCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], ObservantCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					0,
					1,
					2,
					3,
					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					4,
					5,
					6,
					7
					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					0,
					1,
					2,
					3,
					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					4,
					5,
					6,
					7
					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					8,
					9,
					10,
					11
					}, "charge_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					12,
					13,
					14,
					15
					}, "charge_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					11
					}, "overcharge_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					15
					}, "overcharge_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					16,
					17,
					18,
					19
					}, "fire_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 4f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					20,
					21,
					22,
					23
					}, "fire_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 4f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					24,
					25,
					26,
					27,
					28,
					29,
					30,
					31
					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					32,
					33,
					34,
					35,
					36,
					37,
					38,
					39,
					40,
					41,
					42,
					43
					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					43,
					42,
					41,
					40,
					39,
					38,
					37,
					36,
					35,
					34,
					33,
					32
					}, "disappear", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, ObservantCollection, new List<int>
					{
					32,
					33,
					34,
					35,
					36,
					37,
					38,
					39,
					40,
					41,
					42,
					43
					}, "reappear", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
				}

				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "disappear", new Dictionary<int, string> { { 0, "spawnGloop" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "reappear", new Dictionary<int, string> { { 0, "spawnGloop" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "disappear", new Dictionary<int, string> { { 2, "Play_ENM_blobulord_reform_01" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "reappear", new Dictionary<int, string> { { 0, "Play_ENM_blobulord_charge_01" } });


				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 5, "deathBurst" }});
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Play_Squeal" } });
				//EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "chargeattack", new Dictionary<int, string> { { 0, "Play_EnergySwirl" } });
				//EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack", new Dictionary<int, string> { { 0, "Play_Stomp" } });


				GameObject shootpointLeft = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.75f, 1.8125f), "ObservantShootpointLeft");

				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

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
				new SeekTargetBehavior
				{
					ExternalCooldownSource = true,
					StopWhenInRange = true,
					CustomRange = 7f,
					LineOfSight = false,
					ReturnToSpawn = false,
					SpawnTetherDistance = 0f,
					PathInterval = 0.125f,
					SpecifyRange = false,
					MinActiveRange = 0f,
					MaxActiveRange = 0f
				}
				};
				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
				{
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
						Behavior = new ShootBehavior{
						ShootPoint = shootpointLeft,
						BulletScript = new CustomBulletScriptSelector(typeof(TelegraphScript)),
						LeadAmount = 0f,
						AttackCooldown = 1f,
						Cooldown = 1f,
						InitialCooldown = 0.5f,
						ChargeTime = 1f,
						RequiresLineOfSight = trespassEngager,
						MultipleFireEvents = false,
						Uninterruptible = true,
						ChargeAnimation = "charge",
						FireAnimation = "overcharge",
						PostFireAnimation = "fire",
						}
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 2f,
					Behavior = new TeleportBehavior{
					AttackableDuringAnimation = true,
					AllowCrossRoomTeleportation = false,
					teleportRequiresTransparency = false,
					hasOutlinesDuringAnim = true,
					ManuallyDefineRoom = false,
					MaxHealthThreshold = 1f,
					StayOnScreen = true,
					AvoidWalls = true,
					GoneTime = 1.4f,
					OnlyTeleportIfPlayerUnreachable = false,
					MinDistanceFromPlayer = 6f,
					MaxDistanceFromPlayer = 9f,
					teleportInAnim = "reappear",
					teleportOutAnim = "disappear",
					AttackCooldown = 1f,
					InitialCooldown = 0.5f,
					RequiresLineOfSight = false,
					roomMax = new Vector2(0,0),
					roomMin = new Vector2(0,0),
					//teleportInBulletScript = new CustomBulletScriptSelector(typeof(TeleportScript)),
					teleportOutBulletScript = new CustomBulletScriptSelector(typeof(TeleportScript)),
					GlobalCooldown = 0.5f,
					Cooldown = 6f,

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

					},

				};
			
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:observant", companion.aiActor);




				SpriteBuilder.AddSpriteToCollection(basePath + "observant_idle_left_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:observant";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Observant/observant_idle_left_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("observantsheetTrespass");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\observantsheetTrespass.png");
                PlanetsideModule.Strings.Enemies.Set("#OBSERVANT", "Observant");
				PlanetsideModule.Strings.Enemies.Set("#OBSERVANT_SHORTDESC", "Foresighted");
				PlanetsideModule.Strings.Enemies.Set("#OBSERVANT_LONGDESC", "Some say the lights that illumate the sky of Gunymede are trapped Observants, seeing into their old home world.\n\nWishing they could return, from the cells of their eternal prison.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#OBSERVANT";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#OBSERVANT_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#OBSERVANT_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:observant");
				EnemyDatabase.GetEntry("psog:observant").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:observant").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:observant").isNormalEnemy = true;


				//companion.healthHaver.spawnBulletScript = true;
				//companion.healthHaver.chanceToSpawnBulletScript = 1f;
				//companion.healthHaver.bulletScriptType = HealthHaver.BulletScriptType.OnPreDeath;
				//companion.healthHaver.bulletScript = new CustomBulletScriptSelector(typeof(EatPants));

				companion.aiActor.sprite.usesOverrideMaterial = true;
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
				mat.SetFloat("_EmissiveColorPower", 3f);
				mat.SetFloat("_EmissivePower", 80);
				companion.aiActor.sprite.renderer.material = mat;
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableHitscan);
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
			}
		}


		private static readonly string basePath = "Planetside/Resources/Enemies/Observant/";
		private static string[] spritePaths = new string[]
		{
			basePath+"observant_idle_left_001.png",//0
			basePath+"observant_idle_left_002.png",
			basePath+"observant_idle_left_003.png",
			basePath+"observant_idle_left_004.png",//3

			basePath+"observant_idle_right_001.png",//4
			basePath+"observant_idle_right_002.png",
			basePath+"observant_idle_right_003.png",
			basePath+"observant_idle_right_004.png",//7

			basePath+"observant_charge_left_001.png",//8
			basePath+"observant_charge_left_002.png",
			basePath+"observant_charge_left_003.png",
			basePath+"observant_charge_left_004.png",//11

			basePath+"observant_charge_right_001.png",//12
			basePath+"observant_charge_right_002.png",
			basePath+"observant_charge_right_003.png",
			basePath+"observant_charge_right_004.png",//15

			basePath+"observant_fire_left_001.png",//16
			basePath+"observant_fire_left_002.png",
			basePath+"observant_fire_left_003.png",
			basePath+"observant_fire_left_004.png",//19

			basePath+"observant_fire_right_001.png",//20
			basePath+"observant_fire_right_002.png",
			basePath+"observant_fire_right_003.png",
			basePath+"observant_fire_right_004.png",//23

			basePath+"observant_death_001.png",//24
			basePath+"observant_death_002.png",
			basePath+"observant_death_003.png",
			basePath+"observant_death_004.png",
			basePath+"observant_death_005.png",
			basePath+"observant_death_006.png",
			basePath+"observant_death_007.png",
			basePath+"observant_death_008.png",//31

			basePath+"observant_awaken_001.png",//32
			basePath+"observant_awaken_002.png",
			basePath+"observant_awaken_003.png",
			basePath+"observant_awaken_004.png",
			basePath+"observant_awaken_005.png",
			basePath+"observant_awaken_006.png",
			basePath+"observant_awaken_007.png",
			basePath+"observant_awaken_008.png",
			basePath+"observant_awaken_009.png",
			basePath+"observant_awaken_010.png",
			basePath+"observant_awaken_011.png",
			basePath+"observant_awaken_012.png",//43


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
				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{

				};
			}
			private IEnumerator PortalDoer(MeshRenderer portal, float duration = 6, bool DestroyWhenDone = false)
			{
				float elapsed = 0f;
				while (elapsed < duration)
				{
					elapsed += BraveTime.DeltaTime;
					float t = elapsed / duration;
					if (portal.gameObject == null) { yield break; }
					float throne1 = Mathf.Sin(t * (Mathf.PI));
					portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0, 0.1f, throne1));
					portal.material.SetFloat("_HoleEdgeDepth", Mathf.Lerp(12, 2, throne1));
					yield return null;
				}

				if (DestroyWhenDone == true)
				{
					Destroy(portal.gameObject);
				}
				yield break;
			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo.Contains("spawnGloop"))
				{
					GameObject portalObj = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX, this.aiActor.sprite.WorldBottomCenter, Quaternion.Euler(0f, 0f, 0f));
					portalObj.layer = this.aiActor.gameObject.layer + (int)GameManager.Instance.MainCameraController.CurrentZOffset;
					portalObj.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
					MeshRenderer mesh = portalObj.GetComponent<MeshRenderer>();
					mesh.material.SetTexture("_PortalTex", StaticTextures.NebulaTexture);
					GameManager.Instance.StartCoroutine(PortalDoer(mesh, 1.75f, true));
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("deathBurst"))
				{
					
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Blast"))
				{
					GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
					tk2dSpriteAnimator objanimator = silencerVFX.GetComponentInChildren<tk2dSpriteAnimator>();
					objanimator.ignoreTimeScale = true;
					objanimator.AlwaysIgnoreTimeScale = true;
					objanimator.AnimateDuringBossIntros = true;
					objanimator.alwaysUpdateOffscreen = true;
					objanimator.playAutomatically = true;
					ParticleSystem objparticles = silencerVFX.GetComponentInChildren<ParticleSystem>();
					var main = objparticles.main;
					main.useUnscaledTime = true;
					GameObject gameObject = GameObject.Instantiate(silencerVFX.gameObject, base.aiActor.transform.Find("ObservantShootpointLeft").position, Quaternion.identity);
					Destroy(gameObject, 2.5f);

					Exploder.DoDistortionWave(base.aiActor.transform.Find("ObservantShootpointLeft").position, 10f, 0.4f, 3, 0.066f);
				}
			}
		}


		public class TeleportScript : Script
		{
			public override IEnumerator Top()
			{
				for (int i = 0; i < 12; i++)
                {
					this.Fire(new Direction(30 * i, DirectionType.Aim, -1f), new Speed(3f, SpeedType.Absolute), new BasicBigBall());
				}
				yield break;
			}
			public class BasicBigBall : Bullet
			{
				public BasicBigBall() : base("undodgeableSpore", false, false, false)
				{

				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
					yield return this.Wait(360f);
					//base.Fire(new Direction(this.Projectile.Direction.ToAngle(), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(600, SpeedType.Absolute), new HitScan());
					base.Vanish(false);
					yield break;
				}
			}
			public class HitScan : Bullet
            {
				public HitScan() : base(StaticUndodgeableBulletEntries.UndodgeableHitscan.Name, false, false, false)
				{

				}
				public override IEnumerator Top()
				{
					SpawnManager.PoolManager.Remove(this.Projectile.gameObject.transform);
					this.Projectile.BulletScriptSettings.preventPooling = true;

					yield break;
				}
			}

		}

		public class TelegraphScript : Script 
		{
			public override IEnumerator Top()
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
				GameManager.Instance.StartCoroutine(FlashReticles(component2, false, Angle, this, "directedfire"));
				yield return this.Wait(75 * PlayerStats.GetTotalEnemyProjectileSpeedMultiplier());
				yield break;
			}
			public class UndodgeableBullshit : Bullet
			{
				public UndodgeableBullshit() : base("sniperUndodgeable", false, false, false)
				{

				}
				public override IEnumerator Top()
				{
					for (int i = 0; i < 100; i++)
					{
						base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new TelegraphScript.UndodgeableSpore());
						yield return this.Wait(1f);
					}
					yield break;
				}
			}
			public class UndodgeableSpore : Bullet
			{
				public UndodgeableSpore() : base("undodgeableSpore", false, false, false)
				{

				}
				public override IEnumerator Top()
				{
					yield return this.Wait(300f);
					base.Vanish(false);
					yield break;
				}
			}
			private IEnumerator FlashReticles(tk2dTiledSprite tiledspriteObject, bool isDodgeAble, float Angle, TelegraphScript parent, string BulletType)
			{
				tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
				float elapsed = 0;
				float Time = 0.125f;
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
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);


						float math = isDodgeAble == true ? 250 : 25;
						tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (math * t));
						tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (10 * t));
						tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.dimensions = new Vector2(1000f, 1f);
						tiledsprite.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.375f;
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
				Destroy(tiledspriteObject.gameObject);
				if (isDodgeAble == false)
				{
					base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
				}
				if (base.BulletBank.aiActor != null)
                {
					base.PostWwiseEvent("Play_Stomp");
					GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
					tk2dSpriteAnimator objanimator = silencerVFX.GetComponentInChildren<tk2dSpriteAnimator>();
					objanimator.ignoreTimeScale = true;
					objanimator.AlwaysIgnoreTimeScale = true;
					objanimator.AnimateDuringBossIntros = true;
					objanimator.alwaysUpdateOffscreen = true;
					objanimator.playAutomatically = true;
					ParticleSystem objparticles = silencerVFX.GetComponentInChildren<ParticleSystem>();
					var main = objparticles.main;
					main.useUnscaledTime = true;
					GameObject.Instantiate(silencerVFX.gameObject, base.BulletBank.aiActor.transform.Find("ObservantShootpointLeft").position, Quaternion.identity);
					Exploder.DoDistortionWave(base.BulletBank.aiActor.transform.Find("ObservantShootpointLeft").position, 10f, 0.4f, 3, 0.066f);
					base.Fire(new Direction(Angle, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new UndodgeableBullshit());
					for (int i = 0; i < 6; i++)
					{
						this.Fire(new Direction(60 * i, DirectionType.Aim, -1f), new Speed(2f, SpeedType.Absolute), new TeleportScript.BasicBigBall());
					}
				}			
				yield break;
			}
		}
	}
}





