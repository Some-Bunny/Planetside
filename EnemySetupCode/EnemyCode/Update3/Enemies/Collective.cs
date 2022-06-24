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
	public class Collective : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "collective";
		private static tk2dSpriteCollectionData CollectiveCollection;

		public static void Init()
		{
			Collective.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("collective", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false, true);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 300;
				companion.aiActor.MovementSpeed = 1.2f;
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


				companion.aiActor.healthHaver.ForceSetCurrentHealth(88f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(1f, 0.5f), "shadowPos");


				companion.aiActor.healthHaver.SetHealthMaximum(88f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 6,
					ManualOffsetY = 17,
					ManualWidth = 21,
					ManualHeight = 34,
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
					ManualOffsetX = 6,
					ManualOffsetY = 17,
					ManualWidth = 21,
					ManualHeight = 34,
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
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "idle",
					AnimNames = new string[] { "idle" },
					Flipped = new DirectionalAnimation.FlipType[1]
				};

			

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				//
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "charge_small", new string[] { "charge_small" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "charge_large", new string[] { "charge_large" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "fire", new string[] { "fire" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "firetwo", new string[] { "firetwo" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "precharge_small", new string[] { "precharge_small" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				companion.aiActor.reinforceType = ReinforceType.SkipVfx;

				bool flag3 = CollectiveCollection == null;
				if (flag3)
				{
					CollectiveCollection = SpriteBuilder.ConstructCollection(prefab, "CollectiveCollection");
					UnityEngine.Object.DontDestroyOnLoad(CollectiveCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], CollectiveCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CollectiveCollection, new List<int>
					{
					0,
					1,
					2,
					3,
					4,
					5,
					}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, CollectiveCollection, new List<int>
					{
					6,
					7,
					8,
					9,
					9,
					10,
					10,
					11,
					11,
					11
					}, "precharge_small", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 10f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CollectiveCollection, new List<int>
					{
					6,
					7,
					8,
					9,
					9,
					10,
					10,
					11,
					11,
					11
					}, "charge_small", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 30f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, CollectiveCollection, new List<int>
					{
					6,
					7,
					8,
					9,
					9,
					10,
					10,
					11,
					11,
					11,
					12,
					12,
					13,
					14,
					15,
					16,
					16,
					16,
					17,
					17,
					}, "charge_large", tk2dSpriteAnimationClip.WrapMode.Once).fps = 15f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, CollectiveCollection, new List<int>
					{
					6,
					7,
					8,
					9,
					9,
					10,
					16,
					16,
					16,
					17,
					17,
					18
					}, "fire", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CollectiveCollection, new List<int>
					{
					16,
					16,
					16,
					17,
					17,
					18
					}, "firetwo", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CollectiveCollection, new List<int>
					{
					19,
					20,
					21,
					22,
					23,
					24,
					25,
					26,
					27,
					28,
					29,
					30
					
					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, CollectiveCollection, new List<int>
					{
					31,
					32,
					33,
					34,
					35,
					36,
					37,
					38,
					39,
					40,
					41
					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
				}

				Creationist.TrespassEnemyEngageDoerPortalless trespassEngager = companion.aiActor.gameObject.AddComponent<Creationist.TrespassEnemyEngageDoerPortalless>();


				//m_ENM_PhaseSpider_Weave_01
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_large", new Dictionary<int, string> { { 0, "PepsiRage" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_large", new Dictionary<int, string> { { 0, "Play_ENM_PhaseSpider_Weave_01" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "fire", new Dictionary<int, string> { { 0, "ORDER" } });

				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "precharge_small", new Dictionary<int, string> { { 2, "Play_BOSS_dragun_charge_01" } });

				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Surprise" }, { 6, "PepsiRage" } });

				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_small", new Dictionary<int, string> { { 0, "PaPew" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Play_VesselDeath" }, { 6, "Play_ENM_Tarnisher_Bite_01" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Surprise" }, {6, "PepsiRage" } });
				//m_ENM_blobulord_reform_01
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "awaken", new Dictionary<int, string> { { 5, "Play_ENM_blobulord_reform_01" } });

				GameObject shootpoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(1.5f, 2f), "CollectiveShootpoint");

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
						BulletScript = new CustomBulletScriptSelector(typeof(BasicCreationistAttack)),
						LeadAmount = 0f,
						AttackCooldown = 1f,
						Cooldown = 4f,
						InitialCooldown = 3f,
						ChargeTime = 0.75f,
						RequiresLineOfSight = false,
						MultipleFireEvents = false,
						Uninterruptible = true,
						ChargeAnimation = "precharge_small",
						FireAnimation = "charge_small",
						PostFireAnimation = "firetwo",
						StopDuring = ShootBehavior.StopType.Attack
						}
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
						Behavior = new ShootBehavior{
						ShootPoint = shootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(CrowdControl)),
						LeadAmount = 0f,
						AttackCooldown = 2f,
						Cooldown = 4f,
						InitialCooldown = 0f,
						ChargeTime = 0f,
						RequiresLineOfSight = false,
						MultipleFireEvents = false,
						Uninterruptible = true,
						FireAnimation = "charge_large",
						PostFireAnimation = "fire",
						StopDuring = ShootBehavior.StopType.Attack
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
				Game.Enemies.Add("psog:collective", companion.aiActor);




				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Collective/collective_idle_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:collective";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Collective/collective_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\sheetcollectiveTrespass.png");
				PlanetsideModule.Strings.Enemies.Set("#COLLCTIVE", "Collective");
				PlanetsideModule.Strings.Enemies.Set("#COLLCTIVE_SHORTDESC", "Consciousness");
				PlanetsideModule.Strings.Enemies.Set("#UCOLLCTIVE_LONGDESC", "A mass of those affected by its influence. Unable to resist it by themselves, they merged together and try to overpower it through strength in numbers.\n\nThey never succeed.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#COLLCTIVE";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#COLLCTIVE_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#UCOLLCTIVE_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:collective");
				EnemyDatabase.GetEntry("psog:collective").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:collective").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:collective").isNormalEnemy = true;



				companion.aiActor.sprite.usesOverrideMaterial = true;
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
				mat.SetFloat("_EmissiveColorPower", 3f);
				mat.SetFloat("_EmissivePower", 40);
				companion.aiActor.sprite.renderer.material = mat;

				SpawnEnemyOnDeath spawnEnemy = companion.gameObject.AddComponent<SpawnEnemyOnDeath>();
				spawnEnemy.deathType = OnDeathBehavior.DeathType.DeathAnimTrigger;
				spawnEnemy.DoNormalReinforcement = false;
				spawnEnemy.spawnsCanDropLoot = false;
				spawnEnemy.spawnAnim = "idle";
				spawnEnemy.spawnRadius = 1.25f;
				spawnEnemy.minSpawnCount = 1;
				spawnEnemy.maxSpawnCount = 3;
				spawnEnemy.enemySelection = SpawnEnemyOnDeath.EnemySelection.Random;
				spawnEnemy.enemyGuidsToSpawn = new string[] { "unwilling", "unwilling", "unwilling", "unwilling" };
				spawnEnemy.triggerName = "spawnBaddies";
				spawnEnemy.spawnPosition = SpawnEnemyOnDeath.SpawnPosition.InsideRadius;

				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 7, "spawnBaddies" } });
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBouncyBatBullet);
				companion.aiActor.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().baseData.speed *= 1.2f;
				companion.aiActor.bulletBank.Bullets[0].BulletObject.GetComponent<BounceProjModifier>().numberOfBounces += 2;

				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);


			}
		}

		public class CrowdControl : Script
		{
			public List<Spore> list = new List<Spore>();

			protected override IEnumerator Top()
			{
				list = new List<Spore>();
				for (int e = 0; e < 30; e++)
				{
					base.PostWwiseEvent("Play_BOSS_doormimic_flame_01");
					base.Fire(new Direction(UnityEngine.Random.Range(-180, 180), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(2, 7), SpeedType.Absolute), new CrowdControl.Spore(this));
					yield return this.Wait(3);
				}
				yield return this.Wait(120);
				base.PostWwiseEvent("Play_BOSS_doormimic_appear_01");
				for (int e = 0; e < list.Count; e++)
                {
					if (list[e] != null)
                    {
						list[e].StartMove();
                    }
                }
				yield break;
			}

			public class Spore : Bullet
			{
				public Spore(CrowdControl parent) : base(UnityEngine.Random.value > 0.4f ? StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name : StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name, false, false, false)
				{
					this.parent = parent;
				}
				protected override IEnumerator Top()
				{
					hasTriggeredMove = false;
					parent.list.Add(this);
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), UnityEngine.Random.Range(30, 90));
					while (hasTriggeredMove == false)
                    {
						if (parent.IsEnded || parent.Destroyed) { StartMove(); yield break;}
						yield return this.Wait(1);
					}
					yield break;
				}

                public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
                    base.OnBulletDestruction(destroyType, hitRigidbody, preventSpawningProjectiles);
                }
                public void StartMove()
                {
					this.StartTask(StartSpeed());
                }
				public IEnumerator StartSpeed()
				{
					hasTriggeredMove = true;
					yield return this.Wait(UnityEngine.Random.Range(0, 60));
					base.ChangeDirection(new Brave.BulletScript.Direction((this.BulletManager.PlayerPosition() - this.Position).ToAngle()));
					base.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 60);
					yield break;
				}
				private bool hasTriggeredMove;
				private CrowdControl parent;
			}
		}

		public class BasicCreationistAttack : Script
		{
			protected override IEnumerator Top()
			{
				float aimDirection = base.GetAimDirection((float)((UnityEngine.Random.value >= 0.25f) ? 0 : 1), 10f);
				for (int e = 0; e < 5; e++)
                {
					base.PostWwiseEvent("Play_ENM_bulletking_skull_01");
					for (int i = -1; i < 2; i++)
					{
						base.Fire(new Direction(aimDirection + (8 * i), DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new BasicCreationistAttack.Basic());
					}
					yield return this.Wait(12);
				}
				yield break;
			}

			public class Basic: Bullet
			{
				public Basic() : base(StaticUndodgeableBulletEntries.undodgeableBouncyBatBullet.Name, false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(20f, SpeedType.Absolute), 120);
					yield break;
				}
			}

			public class CrossBullet : Bullet
			{
				public CrossBullet(Vector2 offset, int setupDelay, int setupTime) : base(StaticUndodgeableBulletEntries.undodgeableBouncyBatBullet.Name, false, false, false)
				{
					this.m_offset = offset;
					this.m_setupDelay = setupDelay;
					this.m_setupTime = setupTime;
				}

				protected override IEnumerator Top()
				{
					this.ManualControl = true;
					this.m_offset = this.m_offset.Rotate(this.Direction);
					for (int i = 0; i < 360; i++)
					{
						if (i > this.m_setupDelay && i < this.m_setupDelay + this.m_setupTime)
						{
							this.Position += this.m_offset / (float)this.m_setupTime;
						}
						this.Position += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}

				private Vector2 m_offset;

				private int m_setupDelay;

				private int m_setupTime;
			}
		}


		private static string[] spritePaths = new string[]
		{
			"Planetside/Resources/Enemies/Collective/collective_idle_001.png",//0
			"Planetside/Resources/Enemies/Collective/collective_idle_002.png",
			"Planetside/Resources/Enemies/Collective/collective_idle_003.png",
			"Planetside/Resources/Enemies/Collective/collective_idle_004.png",
			"Planetside/Resources/Enemies/Collective/collective_idle_005.png",
			"Planetside/Resources/Enemies/Collective/collective_idle_006.png",//5

			"Planetside/Resources/Enemies/Collective/collective_chargesmall_001.png",//6
			"Planetside/Resources/Enemies/Collective/collective_chargesmall_002.png",
			"Planetside/Resources/Enemies/Collective/collective_chargesmall_003.png",
			"Planetside/Resources/Enemies/Collective/collective_chargesmall_004.png",
			"Planetside/Resources/Enemies/Collective/collective_chargesmall_005.png",
			"Planetside/Resources/Enemies/Collective/collective_chargesmall_006.png",//11

			"Planetside/Resources/Enemies/Collective/collective_chargelargel_001.png",//12
			"Planetside/Resources/Enemies/Collective/collective_chargelargel_002.png",
			"Planetside/Resources/Enemies/Collective/collective_chargelargel_003.png",
			"Planetside/Resources/Enemies/Collective/collective_chargelargel_004.png",//15

			"Planetside/Resources/Enemies/Collective/collective_firesmall_001.png",//16
			"Planetside/Resources/Enemies/Collective/collective_firesmall_002.png",
			"Planetside/Resources/Enemies/Collective/collective_firesmall_003.png",//18

			"Planetside/Resources/Enemies/Collective/collective_death_001.png",//19
			"Planetside/Resources/Enemies/Collective/collective_death_002.png",
			"Planetside/Resources/Enemies/Collective/collective_death_003.png",
			"Planetside/Resources/Enemies/Collective/collective_death_004.png",
			"Planetside/Resources/Enemies/Collective/collective_death_005.png",
			"Planetside/Resources/Enemies/Collective/collective_death_006.png",
			"Planetside/Resources/Enemies/Collective/collective_death_007.png",
			"Planetside/Resources/Enemies/Collective/collective_death_008.png",
			"Planetside/Resources/Enemies/Collective/collective_death_009.png",
			"Planetside/Resources/Enemies/Collective/collective_death_010.png",
			"Planetside/Resources/Enemies/Collective/collective_death_011.png",
			"Planetside/Resources/Enemies/Collective/collective_death_012.png",//30

			"Planetside/Resources/Enemies/Collective/collective_awaken_001.png",//31
			"Planetside/Resources/Enemies/Collective/collective_awaken_002.png",
			"Planetside/Resources/Enemies/Collective/collective_awaken_003.png",
			"Planetside/Resources/Enemies/Collective/collective_awaken_004.png",
			"Planetside/Resources/Enemies/Collective/collective_awaken_005.png",
			"Planetside/Resources/Enemies/Collective/collective_awaken_006.png",
			"Planetside/Resources/Enemies/Collective/collective_awaken_007.png",
			"Planetside/Resources/Enemies/Collective/collective_awaken_008.png",
			"Planetside/Resources/Enemies/Collective/collective_awaken_009.png",
			"Planetside/Resources/Enemies/Collective/collective_awaken_010.png",
			"Planetside/Resources/Enemies/Collective/collective_awaken_011.png",//41

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
			{//Surprise
				if (clip.GetFrame(frameIdx).eventInfo.Contains("PepsiRage"))
                {
					StaticVFXStorage.BeholsterChargeUpVFXInverse.SpawnAtPosition(base.aiActor.transform.Find("CollectiveShootpoint").position);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Surprise"))
                {
					StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(base.aiActor.transform.Find("CollectiveShootpoint").position);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("ORDER"))
				{
					Exploder.DoDistortionWave(base.aiActor.transform.Find("CollectiveShootpoint").position, 10f, 0.1f, 10, 1f);

				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("PaPew"))
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
					GameObject gameObject = GameObject.Instantiate(silencerVFX.gameObject, base.aiActor.transform.Find("CollectiveShootpoint").position, Quaternion.identity);
					Destroy(gameObject, 2.5f);
				}
			}
		}
	}
}





