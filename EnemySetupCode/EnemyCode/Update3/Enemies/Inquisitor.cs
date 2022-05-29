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
	public class Inquisitor : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "inquisitor";
		private static tk2dSpriteCollectionData InquisitorCollection;

		public static void Init()
		{
			Inquisitor.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Inquisitor", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false, true);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 1000;
				companion.aiActor.MovementSpeed = 1.6f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(125f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(2.125f, 0.25f), "shadowPos");


				companion.aiActor.healthHaver.SetHealthMaximum(125f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 18,
					ManualOffsetY = 13,
					ManualWidth = 32,
					ManualHeight = 39,
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
					ManualOffsetX = 18,
					ManualOffsetY = 13,
					ManualWidth = 32,
					ManualHeight = 39,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				//companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;

				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Prefix = "idle",
					AnimNames = new string[] { "idle_right", "idle_left"},
					Flipped = new DirectionalAnimation.FlipType[2]
				};

			/*
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[0], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "attack", new string[] { "attack" }, new DirectionalAnimation.FlipType[0], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "chargeattack", new string[0], new DirectionalAnimation.FlipType[0], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "chargespecialattack", new string[0], new DirectionalAnimation.FlipType[0], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "attackspec", new string[0], new DirectionalAnimation.FlipType[0], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				*/

				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				companion.aiActor.reinforceType = ReinforceType.SkipVfx;
				Creationist.TrespassEnemyEngageDoer trespassEngager = companion.aiActor.gameObject.AddComponent<Creationist.TrespassEnemyEngageDoer>();
				trespassEngager.PortalLifeTime = 5;
				trespassEngager.PortalSize = 0.3f;


				bool flag3 = InquisitorCollection == null;
				if (flag3)
				{
					InquisitorCollection = SpriteBuilder.ConstructCollection(prefab, "InquisitorCollection");
					UnityEngine.Object.DontDestroyOnLoad(InquisitorCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], InquisitorCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, InquisitorCollection, new List<int>
					{
					0,
					1,
					2,
					2,
					3,
					3,
					4,
					
					4,
					3,
					2,
					1,
					1,
					0,
					

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, InquisitorCollection, new List<int>
					{

					5,
					6,
					7,
					7,
					8,
					8,
					9,
					9,
					
					8,
					7,
					6,
					6,
					5,
					
					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					/*
					SpriteBuilder.AddAnimation(companion.spriteAnimator, InquisitorCollection, new List<int>
					{
					16,
					17,
					18,
					19,
					20,
					21,
					22,
					23,
					24
					}, "chargeattack", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, InquisitorCollection, new List<int>
					{
					16,
					17,
					18,
					19,
					20,
					21,
					22,
					23,
					24
					}, "chargespecialattack", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, InquisitorCollection, new List<int>
					{
					25,
					26,
					27,
					28,
					29,
					30,
					31,
					32
					}, "attack", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, InquisitorCollection, new List<int>
					{
					25,
					26,
					27,
					28,
					29,
					30,
					31,
					32
					}, "attackspec", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					*/

					SpriteBuilder.AddAnimation(companion.spriteAnimator, InquisitorCollection, new List<int>
					{
					10,
					11,
					12,
					13,
					14,
					15,
					15,
					16,
					16,
					16,
					17,
					18,
					19,
					20,
					21,
					22,
					23,
					24
					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, InquisitorCollection, new List<int>
					{
					25,
					26,
					27,
					28,
					28,
					29,
					29,
					29,
					30,
					31,
					32,
					32,
					33,
					33,
					34,
					35,
					36,
					37,
					38,
					39,
					40,
					41

					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

					

				}

				/*
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "attack", new Dictionary<int, string> { { 0, "Blast" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 5, "deathBurst" }});
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Play_Squeal" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "chargeattack", new Dictionary<int, string> { { 0, "Play_EnergySwirl" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack", new Dictionary<int, string> { { 0, "Play_Stomp" } });
				*/

				GameObject shootpoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.5f, 0.5f), "CreationistShootpoint");

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
				/*
				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
				{
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
						Behavior = new ShootBehavior{
						ShootPoint = shootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(BasicCreationistAttack)),
						LeadAmount = 0f,
						AttackCooldown = 2.25f,
						Cooldown = 1f,
						InitialCooldown = 0.5f,
						ChargeTime = 1f,
						RequiresLineOfSight = false,
						MultipleFireEvents = false,
						Uninterruptible = true,
						//ChargeAnimation = "chargeattack",
						//PostFireAnimation = "attack",
						}
					},

					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 2f,
						Behavior = new ShootBehavior{
						ShootPoint = shootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(TelegraphScript)),
						LeadAmount = 0f,
						AttackCooldown = 2f,
						Cooldown = 6f,
						InitialCooldown = 2f,
						RequiresLineOfSight = false,
						MultipleFireEvents = false,
						Uninterruptible = true,
						//FireAnimation = "chargespecialattack",
						//PostFireAnimation = "attackspec",
						}
					}
				};
				*/
				/*
				string basepath = "Planetside/Resources/Enemies/Creationist/";
				DebrisObject shoulder1 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "creationistDebris1.png", true, 0.5f, 3, 540, 120, null, 0.9f, "Play_BOSS_lichA_crack_01", null, 0);
				DebrisObject shoulder2 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "creationistDebris2.png", true, 0.5f, 3, 360, 120, null, 0.7f, "Play_BOSS_lichA_crack_01", null, 1);
				ShardCluster BONES = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shoulder1, shoulder2}, 0.9f, 2f, 1, 2, 1f);
				SpawnShardsOnDeath BodyAndStuff = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
				BodyAndStuff.deathType = OnDeathBehavior.DeathType.Death;
				BodyAndStuff.breakStyle = MinorBreakable.BreakStyle.BURST;
				BodyAndStuff.verticalSpeed = 0.4f;
				BodyAndStuff.heightOffGround = 2f;
				BodyAndStuff.shardClusters = new ShardCluster[] { BONES };
				*/

				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:inquisitor", companion.aiActor);




				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Creationist/creationist_charge_attack_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:inquisitor";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Creationist/creationist_charge_attack_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\creationistTemplateTrespass.png");
				PlanetsideModule.Strings.Enemies.Set("#INQUISITOR", "Inquisitor");
				PlanetsideModule.Strings.Enemies.Set("#INQUISITOR_SHORTDESC", "Power Crept");
				PlanetsideModule.Strings.Enemies.Set("#INQUISITOR_LONGDESC", "Now with infinite foresight.\n\nNow with infinite time.\n\nNow with infinite potential.\n\nYet, blinded by their new powers, they failed to see their eventual, eternal entombment.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#INQUISITOR";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#INQUISITOR_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#INQUISITOR_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:inquisitor");
				EnemyDatabase.GetEntry("psog:inquisitor").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:inquisitor").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:inquisitor").isNormalEnemy = true;


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

			}
		}


		private static string basePath = "Planetside/Resources/Enemies/Inquisitor/";

		private static string[] spritePaths = new string[]
		{
			basePath+"inquisitor_idle_left_001.png",//0
			basePath+"inquisitor_idle_left_002.png",
			basePath+"inquisitor_idle_left_003.png",
			basePath+"inquisitor_idle_left_004.png",
			basePath+"inquisitor_idle_left_005.png",//4

			basePath+"inquisitor_idle_right_001.png",//5
			basePath+"inquisitor_idle_right_002.png",
			basePath+"inquisitor_idle_right_003.png",
			basePath+"inquisitor_idle_right_004.png",
			basePath+"inquisitor_idle_right_005.png",//9

			basePath+"inquisitor_death_001.png",//10
			basePath+"inquisitor_death_002.png",
			basePath+"inquisitor_death_003.png",
			basePath+"inquisitor_death_004.png",
			basePath+"inquisitor_death_005.png",
			basePath+"inquisitor_death_006.png",
			basePath+"inquisitor_death_007.png",
			basePath+"inquisitor_death_008.png",
			basePath+"inquisitor_death_009.png",
			basePath+"inquisitor_death_010.png",
			basePath+"inquisitor_death_011.png",
			basePath+"inquisitor_death_012.png",
			basePath+"inquisitor_death_013.png",
			basePath+"inquisitor_death_014.png",
			basePath+"inquisitor_death_015.png",//24

			basePath+"inquisitor_awaken_001.png",//25
			basePath+"inquisitor_awaken_002.png",
			basePath+"inquisitor_awaken_003.png",
			basePath+"inquisitor_awaken_004.png",
			basePath+"inquisitor_awaken_005.png",
			basePath+"inquisitor_awaken_006.png",
			basePath+"inquisitor_awaken_007.png",
			basePath+"inquisitor_awaken_008.png",
			basePath+"inquisitor_awaken_009.png",
			basePath+"inquisitor_awaken_010.png",
			basePath+"inquisitor_awaken_011.png",
			basePath+"inquisitor_awaken_012.png",
			basePath+"inquisitor_awaken_013.png",
			basePath+"inquisitor_awaken_014.png",
			basePath+"inquisitor_awaken_015.png",
			basePath+"inquisitor_awaken_016.png",
			basePath+"inquisitor_awaken_017.png",//41

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
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo.Contains("deathBurst"))
				{
					var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PortalClose"));
					partObj.transform.position = base.aiActor.transform.Find("CreationistShootpoint").position;
					partObj.transform.localScale *= 1f;
					Destroy(partObj, 3.4f);
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
					GameObject.Instantiate(silencerVFX.gameObject, base.aiActor.transform.Find("CreationistShootpoint").position, Quaternion.identity);
					Exploder.DoDistortionWave(base.aiActor.transform.Find("CreationistShootpoint").position, 10f, 0.4f, 3, 0.066f);
				}
			}
		}


		public class BasicCreationistAttack : Script
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBig);
				this.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new BasicBigBall());
				yield break;
			}
			public class BasicBigBall : Bullet
			{
				public BasicBigBall() : base("undodgeableBig", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(20f, SpeedType.Absolute), 120);
					yield break;
				}
			}
		}

		public class TelegraphScript : Script 
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
				for (int i = -1; i < 2; i++)
				{
					float Angle = base.AimDirection + (20 * i);
					float Offset = (20 * i);
					GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);

					tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
					component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
					component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
					component2.dimensions = new Vector2(1000f, 1f);
					component2.UpdateZDepth();
					component2.HeightOffGround = -2;
					Color laser = new Color(0f, 1f, 1f, 1f);
					Color laserRed = new Color(1f, 0f, 0f, 1f);
					component2.sprite.usesOverrideMaterial = true;
					component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
					component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
					component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
					component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
					component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
					component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
					GameManager.Instance.StartCoroutine(FlashReticles(component2, false, Angle, Offset, this, "directedfire"));
				}
				yield return this.Wait(75);
				yield break;
			}
			public class UndodgeableBullshit : Bullet
			{
				public UndodgeableBullshit() : base("sniperUndodgeable", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					yield break;
				}
			}
			private IEnumerator FlashReticles(tk2dTiledSprite tiledspriteObject, bool isDodgeAble, float Angle, float Offset, TelegraphScript parent, string BulletType)
			{
				tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
				float elapsed = 0;
				float Time = 0.5f;
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
						tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, base.AimDirection + Mathf.Lerp(0, Offset, t));
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.dimensions = new Vector2(1000f, 1f);
						tiledsprite.UpdateZDepth();

						Angle = base.AimDirection + Mathf.Lerp(0, Offset, t);
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.5f;
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
				for (int i = 0; i < 3; i++)
				{
					base.Fire(new Direction(Angle, DirectionType.Absolute, -1f), new Speed(11f + (i * 1.5f), SpeedType.Absolute), new UndodgeableBullshit());
				}
				yield break;
			}

		}


	}
}





