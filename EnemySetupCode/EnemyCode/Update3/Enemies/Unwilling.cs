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
	public class Unwilling : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "unwilling";
		private static tk2dSpriteCollectionData UnwillingCollection;

		public static void Init()
		{
			Unwilling.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Unwilling", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false, true);
				var companion = prefab.AddComponent<EnemyBehavior>();
				prefab.AddComponent<ForgottenEnemyComponent>();
				companion.aiActor.knockbackDoer.weight = 120;
				companion.aiActor.MovementSpeed = 1.5f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;

				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;
				companion.aiActor.FallingProhibited =true;
				companion.aiActor.SetIsFlying(true, "I can fly", true, true);


				companion.aiActor.healthHaver.ForceSetCurrentHealth(12f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.defaultShadow, new Vector2(0.625f, 0.25f), "shadowPos");


				companion.aiActor.healthHaver.SetHealthMaximum(12f, null, false);
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
					ManualOffsetY = 2,
					ManualWidth = 11,
					ManualHeight = 17,
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
					ManualOffsetY = 2,
					ManualWidth = 11,
					ManualHeight = 17,
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

			

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "charge", new string[] { "charge" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				companion.aiActor.reinforceType = ReinforceType.SkipVfx;

				bool flag3 = UnwillingCollection == null;
				if (flag3)
				{
					UnwillingCollection = SpriteBuilder.ConstructCollection(prefab, "UnwillingCollection");
					UnityEngine.Object.DontDestroyOnLoad(UnwillingCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], UnwillingCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, UnwillingCollection, new List<int>
					{
					0,
					1,
					2,
					3,
					4,
					5,
					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, UnwillingCollection, new List<int>
					{
					6,
					7,
					8,
					9,
					10,
					11
					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, UnwillingCollection, new List<int>
					{
					12,
					13,
					14,
					14,
					15,
					15,
					16,
					17,
					16,
					17,
					16,
					17,
					16,
					17,
					}, "charge", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, UnwillingCollection, new List<int>
					{
					26,
					27,
					28,
					29,
					30,
					31,
					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, UnwillingCollection, new List<int>
					{
					18,
					19,
					20,
					21,
					22,
					23,
					24,
					25
					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
				}

				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Play_Squeal" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "awaken", new Dictionary<int, string> { { 2, "Play_ENM_critter_poof_01" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "charge", new Dictionary<int, string> { { 6, "Play_ENM_cannonarmor_charge_01" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge", new Dictionary<int, string> { { 8, "SpawnBlueChargy" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "DeathMper" } });


				/*
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "attack", new Dictionary<int, string> { { 0, "Blast" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 5, "deathBurst" }});
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "chargeattack", new Dictionary<int, string> { { 0, "Play_EnergySwirl" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack", new Dictionary<int, string> { { 0, "Play_Stomp" } });
				*/

				GameObject shootpoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.5f, 0.5f), "UnwillingShootpoint");

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
						Behavior = new SuicideShotBehavior{
						bulletBankName = StaticUndodgeableBulletEntries.UnwillingShot.Name,
						chargeAnim = "charge",
						Range = 20,
						degreesBetween = 5,
						numBullets = 5,
						invulnerableDuringAnimatoin = true,
						InitialCooldown = 5,
						InitialCooldownVariance = 2,
						RequiresLineOfSight = true,		
						
						}
					},
				};
				
		
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:unwilling", companion.aiActor);




				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Unwilling/willing_chargeup_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:unwilling";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Unwilling/willing_chargeup_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("sheetUnwillingTrespass");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\sheetUnwillingTrespass.png");
                PlanetsideModule.Strings.Enemies.Set("#UNWILLING", "Unwilling");
				PlanetsideModule.Strings.Enemies.Set("#UNWILLING_SHORTDESC", "Forced Conversion");
				PlanetsideModule.Strings.Enemies.Set("#UNWILLING_LONGDESC", "Those who had the weakest resistance to its influence suffered the quickest, and usually an unwanted metamorphosis.\n\nAnd yet, it still hungered for more power.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#UNWILLING";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#UNWILLING_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#UNWILLING_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:unwilling");
				EnemyDatabase.GetEntry("psog:unwilling").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:unwilling").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:unwilling").isNormalEnemy = true;


				//companion.healthHaver.spawnBulletScript = true;
				//companion.healthHaver.chanceToSpawnBulletScript = 1f;
				//companion.healthHaver.bulletScriptType = HealthHaver.BulletScriptType.OnPreDeath;
				//companion.healthHaver.bulletScript = new CustomBulletScriptSelector(typeof(EatPants));

				companion.aiActor.sprite.usesOverrideMaterial = true;
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
				mat.SetFloat("_EmissiveColorPower", 3f);
				mat.SetFloat("_EmissivePower", 50);
				companion.aiActor.sprite.renderer.material = mat;

				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnwillingShot);
				companion.aiActor.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().baseData.speed = 14f;

				//companion.aiActor.healthHaver.persistsOnDeath = true;
				//if (companion.aiActor.bulletBank != null) { ETGModConsole.Log("bank exists, just add bullets in Init()"); }

			}
		}



		private static string[] spritePaths = new string[]
		{
			"Planetside/Resources/Enemies/Unwilling/willing_idle_left_001.png",//0
			"Planetside/Resources/Enemies/Unwilling/willing_idle_left_002.png",
			"Planetside/Resources/Enemies/Unwilling/willing_idle_left_003.png",
			"Planetside/Resources/Enemies/Unwilling/willing_idle_left_004.png",
			"Planetside/Resources/Enemies/Unwilling/willing_idle_left_005.png",
			"Planetside/Resources/Enemies/Unwilling/willing_idle_left_006.png",//5

			"Planetside/Resources/Enemies/Unwilling/willing_idle_right_001.png",//6
			"Planetside/Resources/Enemies/Unwilling/willing_idle_right_002.png",
			"Planetside/Resources/Enemies/Unwilling/willing_idle_right_003.png",
			"Planetside/Resources/Enemies/Unwilling/willing_idle_right_004.png",
			"Planetside/Resources/Enemies/Unwilling/willing_idle_right_005.png",
			"Planetside/Resources/Enemies/Unwilling/willing_idle_right_006.png",//11

			"Planetside/Resources/Enemies/Unwilling/willing_chargeup_001.png",//12
			"Planetside/Resources/Enemies/Unwilling/willing_chargeup_002.png",
			"Planetside/Resources/Enemies/Unwilling/willing_chargeup_003.png",
			"Planetside/Resources/Enemies/Unwilling/willing_chargeup_004.png",
			"Planetside/Resources/Enemies/Unwilling/willing_chargeup_005.png",
			"Planetside/Resources/Enemies/Unwilling/willing_chargeup_006.png",//17

			"Planetside/Resources/Enemies/Unwilling/willing_awaken_001.png",//18
			"Planetside/Resources/Enemies/Unwilling/willing_awaken_002.png",
			"Planetside/Resources/Enemies/Unwilling/willing_awaken_003.png",
			"Planetside/Resources/Enemies/Unwilling/willing_awaken_004.png",
			"Planetside/Resources/Enemies/Unwilling/willing_awaken_005.png",
			"Planetside/Resources/Enemies/Unwilling/willing_awaken_006.png",
			"Planetside/Resources/Enemies/Unwilling/willing_awaken_007.png",
			"Planetside/Resources/Enemies/Unwilling/willing_awaken_008.png",//25

			"Planetside/Resources/Enemies/Unwilling/willing_die_001.png",//26
			"Planetside/Resources/Enemies/Unwilling/willing_die_002.png",
			"Planetside/Resources/Enemies/Unwilling/willing_die_003.png",
			"Planetside/Resources/Enemies/Unwilling/willing_die_004.png",
			"Planetside/Resources/Enemies/Unwilling/willing_die_005.png",
			"Planetside/Resources/Enemies/Unwilling/willing_die_006.png",//31
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
				base.aiActor.healthHaver.OnPreDeath += (obj) =>{};
			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo.Contains("DeathMper"))
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
					GameObject gameObject = GameObject.Instantiate(silencerVFX.gameObject, base.aiActor.transform.Find("UnwillingShootpoint").position, Quaternion.identity);
					Destroy(gameObject, 2.5f);

				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("SpawnBlueChargy"))
				{
					GameObject gameObject = GameObject.Instantiate(StaticVFXStorage.MachoBraceBurstVFX, base.aiActor.transform.Find("UnwillingShootpoint").position - new Vector3(2, 0), Quaternion.identity);
					tk2dSprite sprite = gameObject.GetComponent<tk2dSprite>();
					sprite.usesOverrideMaterial = true;

					sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
					sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
					sprite.renderer.material.SetFloat("_EmissivePower", 100);
					sprite.renderer.material.SetFloat("_EmissiveColorPower", 10f);
					sprite.renderer.material.SetColor("_OverrideColor", Color.cyan);
					sprite.renderer.material.SetColor("_EmissiveColor", Color.cyan);
					Destroy(gameObject, 2.5f);
				}
			}
		}
	}
}





