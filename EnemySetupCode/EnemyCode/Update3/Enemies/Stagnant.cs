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
	public class Stagnant : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "stagnant";
		private static tk2dSpriteCollectionData StagnantCollection;


		public static void Init()
		{
			Stagnant.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Stagnant", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false, true);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 100000;
				companion.aiActor.MovementSpeed = 0f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(22f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.defaultShadow, new Vector2(0.625f, 0.25f), "shadowPos");


				companion.aiActor.healthHaver.SetHealthMaximum(22f, null, false);
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
					ManualWidth = 15,
					ManualHeight = 20,
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
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "attack", new string[] { "attack" }, new DirectionalAnimation.FlipType[0], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "chargeattack", new string[0], new DirectionalAnimation.FlipType[0], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				companion.aiActor.reinforceType = ReinforceType.SkipVfx;
				Creationist.TrespassEnemyEngageDoer trespassEngager = companion.aiActor.gameObject.AddComponent<Creationist.TrespassEnemyEngageDoer>();


				bool flag3 = StagnantCollection == null;
				if (flag3)
				{
					StagnantCollection = SpriteBuilder.ConstructCollection(prefab, "StagnantCollection");
					UnityEngine.Object.DontDestroyOnLoad(StagnantCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], StagnantCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, StagnantCollection, new List<int>
					{
					0,
					1,
					2,
					3,
					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, StagnantCollection, new List<int>
					{
					4,
					5,
					6,
					7
					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
				
					SpriteBuilder.AddAnimation(companion.spriteAnimator, StagnantCollection, new List<int>
					{
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15,
					15,
					16,
					16,
					16
					}, "chargeattack", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					
					SpriteBuilder.AddAnimation(companion.spriteAnimator, StagnantCollection, new List<int>
					{
					16,
					15,
					13,
					11,
					10,
					8
					}, "attack", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, StagnantCollection, new List<int>
					{
					17,
					18,
					19,
					20,
					21,
					22,
					23
					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, StagnantCollection, new List<int>
					{
					24,
					25,
					26,
					27,
					28,
					29,
					30,
					31,
					32,
					33,
					34,
					35,
					36,
					37,
					38
					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;

					

				}

				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Play_Squeal" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack", new Dictionary<int, string> { { 0, "Play_ENM_rubber_blast_01" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "attack", new Dictionary<int, string> { { 0, "Blast" } });

				/*
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 5, "deathBurst" }});
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "chargeattack", new Dictionary<int, string> { { 0, "Play_EnergySwirl" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "attack", new Dictionary<int, string> { { 0, "Play_Stomp" } });
				*/

				GameObject shootpoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.75f, 1f), "StatgnantShootpoint");

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
						ShootPoint = shootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(BasicStagnantAttack)),
						LeadAmount = 0f,
						AttackCooldown = 2.25f,
						Cooldown = 1f,
						InitialCooldown = 0.5f,
						ChargeTime = 1f,
						RequiresLineOfSight = false,
						MultipleFireEvents = false,
						Uninterruptible = true,
						ChargeAnimation = "chargeattack",
						PostFireAnimation = "attack",
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
				Game.Enemies.Add("psog:stagnant", companion.aiActor);




				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Stagnant/lilthing_die_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:stagnant";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Stagnant/lilthing_die_001.png";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\stagnantTrespass.png");
				PlanetsideModule.Strings.Enemies.Set("#STAGNANT", "Stagnant");
				PlanetsideModule.Strings.Enemies.Set("#STAGNANT_SHORTDESC", "Regressive Defensive");
				PlanetsideModule.Strings.Enemies.Set("#STAGNANT_LONGDESC", "Overwhelmed by their new sight, Stagnants chain themselves to a surface and refuse to move from it.\n\nBringing themselves their own downfall.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#STAGNANT";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#STAGNANT_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#STAGNANT_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:stagnant");
				EnemyDatabase.GetEntry("psog:stagnant").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:stagnant").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:stagnant").isNormalEnemy = true;


				companion.healthHaver.spawnBulletScript = true;
				companion.healthHaver.chanceToSpawnBulletScript = 1f;
				companion.healthHaver.bulletScriptType = HealthHaver.BulletScriptType.OnPreDeath;
				companion.healthHaver.bulletScript = new CustomBulletScriptSelector(typeof(DeathSpore));

				companion.aiActor.sprite.usesOverrideMaterial = true;
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
				mat.SetFloat("_EmissiveColorPower", 3f);
				mat.SetFloat("_EmissivePower", 60);
				companion.aiActor.sprite.renderer.material = mat;

			}
		}



		private static string[] spritePaths = new string[]
		{
			"Planetside/Resources/Enemies/Stagnant/lilthing_idle_left_001.png",//0
			"Planetside/Resources/Enemies/Stagnant/lilthing_idle_left_002.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_idle_left_003.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_idle_left_004.png",//3

			"Planetside/Resources/Enemies/Stagnant/lilthing_idle_right_001.png",//4
			"Planetside/Resources/Enemies/Stagnant/lilthing_idle_right_002.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_idle_right_003.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_idle_right_004.png",//7

			"Planetside/Resources/Enemies/Stagnant/lilthing_charge_001.png",//8
			"Planetside/Resources/Enemies/Stagnant/lilthing_charge_002.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_charge_003.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_charge_004.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_charge_005.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_charge_006.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_charge_007.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_charge_008.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_charge_009.png",//16

			"Planetside/Resources/Enemies/Stagnant/lilthing_die_001.png",//17
			"Planetside/Resources/Enemies/Stagnant/lilthing_die_002.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_die_003.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_die_004.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_die_005.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_die_006.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_die_007.png",//23

			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_001.png",//24
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_002.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_003.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_004.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_005.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_006.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_007.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_008.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_009.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_010.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_011.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_012.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_013.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_014.png",
			"Planetside/Resources/Enemies/Stagnant/lilthing_awaken_015.png",//38

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
				//this.aiActor.knockbackDoer.SetImmobile(true, "hehe.");
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{

				};
			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{	
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
					GameObject.Instantiate(silencerVFX.gameObject, base.aiActor.transform.Find("StatgnantShootpoint").position, Quaternion.identity);
					Exploder.DoDistortionWave(base.aiActor.transform.Find("StatgnantShootpoint").position, 10f, 0.4f, 3, 0.066f);
				}
			}
		}

		public class DeathSpore : Script
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
				for (int i = 0; i < 12; i++)
                {
					this.Fire(new Direction(UnityEngine.Random.Range(-180, 180), DirectionType.Aim, -1f), new Speed(5f, SpeedType.Absolute), new Spore());
				}
				yield break;
			}
			public class Spore : Bullet
			{
				public Spore() : base("undodgeableSpore", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), UnityEngine.Random.Range(10, 90));
					yield return this.Wait(300);
					base.Vanish(false);
					yield break;
				}
			}
		}


		public class BasicStagnantAttack : Script
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableQuickHoming);
				this.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new BasicBigBall());
				yield break;
			}
			public class BasicBigBall : Bullet
			{
				public BasicBigBall() : base("UndodgeablequickHoming", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					this.ChangeSpeed(new Speed(11f, SpeedType.Absolute), 90);
					for (int i = 0; i < 90; i++)
					{
						float aim = this.GetAimDirection(1f, 16f);
						float delta = BraveMathCollege.ClampAngle180(aim - this.Direction);
						if (Mathf.Abs(delta) > 180f)
						{
							yield break;
						}
						this.Direction += Mathf.MoveTowards(0f, delta, 10f);
						yield return this.Wait(1);
					}
				}
			}
		}



	}
}





