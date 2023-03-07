using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;	
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;
using NpcApi;

using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Reflection;

using System.Text;


namespace Planetside
{

	public class HMPrimeEngageDoer : CustomEngageDoer
	{
		public void Awake()
		{
			this.StartIntro();
		}
		public void Start()
		{
			this.StartIntro();
		}


		public void Update()
		{

		}

		public override void StartIntro()
		{
			if (this.m_isFinished)
			{
				return;
			}
			base.StartCoroutine(this.DoIntro());
		}


		private IEnumerator DoIntro()
		{
			this.aiActor.enabled = false;
			this.behaviorSpeculator.enabled = false;
			this.aiActor.ToggleRenderers(false);
			this.specRigidbody.enabled = false;
			this.aiActor.IgnoreForRoomClear = true;
			this.aiActor.IsGone = true;
			this.aiActor.ToggleRenderers(false);
			if (this.aiShooter)
			{
				this.aiShooter.ToggleGunAndHandRenderers(false, "GuardIsSpawning");
			}
			this.aiActor.healthHaver.PreventAllDamage = true;
			this.aiActor.enabled = true;
			this.specRigidbody.enabled = true;
			this.aiActor.IsGone = false;
			this.aiActor.IgnoreForRoomClear = false;
			this.aiActor.ToggleRenderers(true);
			this.aiAnimator.PlayDefaultAwakenedState();
			this.aiActor.State = AIActor.ActorState.Awakening;
			int playerMask = CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.PlayerHitBox);
			this.aiActor.specRigidbody.AddCollisionLayerIgnoreOverride(playerMask);

	
			while (this.aiAnimator.IsPlaying("awaken"))
			{
				this.behaviorSpeculator.enabled = false;
				if (this.aiShooter)
				{
					this.aiShooter.ToggleGunAndHandRenderers(false, "GuardIsSpawning");
				}
				yield return null;
			}
			if (this.aiShooter)
			{
				this.aiShooter.ToggleGunAndHandRenderers(true, "GuardIsSpawning");
			}
			yield return new WaitForSeconds(0.25f);
			this.behaviorSpeculator.enabled = true;
			this.aiActor.HasBeenEngaged = true;
			this.aiActor.State = AIActor.ActorState.Normal;
			this.StartIntro();
			m_isFinished = true;
			yield break;
		}
		public override bool IsFinished
		{
			get
			{
				return this.m_isFinished;
			}
		}
		private bool m_isFinished;
	}



	public class HMPrimeApproachEnemiesBehavior : MovementBehaviorBase
	{
		public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
		{
			base.Init(gameObject, aiActor, aiShooter);
		}

		public override void Upkeep()
		{
			base.Upkeep();
			base.DecrementTimer(ref this.repathTimer, false);
		}

		public override BehaviorResult Update()
		{
			SpeculativeRigidbody overrideTarget = this.m_aiActor.OverrideTarget;
			bool flag = this.repathTimer > 0f;
			BehaviorResult result;
			if (flag)
			{
				result = ((overrideTarget == null) ? BehaviorResult.Continue : BehaviorResult.SkipRemainingClassBehaviors);
			}
			else
			{
				bool flag2 = overrideTarget == null;
				if (flag2)
				{
					this.PickNewTarget();
					result = BehaviorResult.Continue;
				}
				else
				{
					this.isInRange = (Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, overrideTarget.UnitCenter) <= this.DesiredDistance);
					bool flag3 = overrideTarget != null && !this.isInRange;
					if (flag3)
					{
						this.m_aiActor.PathfindToPosition(overrideTarget.UnitCenter, null, true, null, null, null, false);
						this.repathTimer = this.PathInterval;
						result = BehaviorResult.SkipRemainingClassBehaviors;
					}
					else
					{
						bool flag4 = overrideTarget != null && this.repathTimer >= 0f;
						if (flag4)
						{
							this.m_aiActor.ClearPath();
							this.repathTimer = -1f;
						}
						result = BehaviorResult.Continue;
					}
				}
			}
			return result;
		}

		private void PickNewTarget()
		{

			bool flag = this.m_aiActor == null;
			if (!flag)
			{
				bool flag2 = this.Owner == null;
				if (flag2)
				{
					this.Owner = this.m_aiActor.CompanionOwner;
				}
				this.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref this.roomEnemies);
				for (int i = 0; i < this.roomEnemies.Count; i++)
				{
					AIActor aiactor = this.roomEnemies[i];
					bool flag3 = aiactor.IsHarmlessEnemy || !aiactor.IsNormalEnemy || aiactor.healthHaver.IsDead || aiactor == this.m_aiActor || aiactor.EnemyGuid == "ba928393c8ed47819c2c5f593100a5bc";
					if (flag3)
					{

						this.roomEnemies.Remove(aiactor);

					}
				}
				bool flag4 = this.roomEnemies.Count == 0;
				if (flag4)
				{
					this.m_aiActor.OverrideTarget = null;
				}
				else
				{
					AIActor aiActor = this.m_aiActor;
					AIActor aiactor2 = this.roomEnemies[UnityEngine.Random.Range(0, this.roomEnemies.Count)];
					aiActor.OverrideTarget = ((aiactor2 != null) ? aiactor2.specRigidbody : null);
				}
			}
		}


		public float PathInterval = 0.25f;
		public float DesiredDistance = 5f;
		private float repathTimer;
		private List<AIActor> roomEnemies = new List<AIActor>();
		private bool isInRange;
		private PlayerController Owner;
	}


	public class FriendlyHMPrime : AIActor
	{
		public static GameObject robotShopkeeperprefab;
		public static readonly string guid = "RobotShopkeeperBoss_friendly";
		//private static tk2dSpriteCollectionData RobotShopkeeperCollection;

		public static void Init()
		{
			FriendlyHMPrime.BuildPrefab();

			AIBulletBank.Entry MinigunEntry = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));	
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(519) as Gun).DefaultModule.projectiles[0]));
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			projectile.baseData.damage = 5;

			MinigunEntry.Name = "minigun";
			MinigunEntry.MuzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
			MinigunEntry.BulletObject = projectile.gameObject;

			minigunEntry = MinigunEntry;

			AIBulletBank.Entry MinigunEntryStronger = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));
			Projectile projectileTwo = UnityEngine.Object.Instantiate<Projectile>(UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(519) as Gun).DefaultModule.projectiles[0]));
			projectileTwo.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectileTwo.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectileTwo);
			projectile.baseData.damage = 12;

			MinigunEntryStronger.Name = "minigunTwo";
			MinigunEntryStronger.MuzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
			MinigunEntryStronger.BulletObject = projectileTwo.gameObject;

			minigunEntryTwo = MinigunEntryStronger;

			/*
			AIBulletBank.Entry MissileEntry = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));

			Projectile missileProjectile = UnityEngine.Object.Instantiate<Projectile>(UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(39) as Gun).DefaultModule.projectiles[0]));
			missileProjectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(missileProjectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(missileProjectile);

			HomingModifier HomingMod = missileProjectile.gameObject.GetOrAddComponent<HomingModifier>();
			HomingMod.AngularVelocity = 180;
			HomingMod.HomingRadius = 30;
			missileProjectile.baseData.speed *= 0.6f;

			MissileEntry.Name = "missile";
			MissileEntry.MuzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
			MissileEntry.AudioEvent = null;

			MissileEntry.BulletObject = missileProjectile.gameObject;
			*/
			//missileEntry = MissileEntry;


			AIBulletBank.Entry TaserEntry = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));

			Projectile electricProjectile = UnityEngine.Object.Instantiate<Projectile>(UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0]));
			FakePrefab.MarkAsFakePrefab(electricProjectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(electricProjectile);
			PierceProjModifier pierce = electricProjectile.gameObject.GetOrAddComponent<PierceProjModifier>();
			electricProjectile.gameObject.SetActive(false);
			pierce.penetration = 10;
			TaserEntry.Name = "taser";
			TaserEntry.MuzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
			TaserEntry.BulletObject = electricProjectile.gameObject;
			TaserEntry.AudioEvent = null;

			taserEntry = TaserEntry;
		}
		private static AIBulletBank.Entry minigunEntryTwo;
		private static AIBulletBank.Entry minigunEntry;
		//private static AIBulletBank.Entry missileEntry;
		private static AIBulletBank.Entry taserEntry;

		public static void BuildPrefab()
		{

            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("HMPrimeCollection").GetComponent<tk2dSpriteCollectionData>();
            Material matRobot = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("hmprime material");

            if (robotShopkeeperprefab == null || !BossBuilder.Dictionary.ContainsKey(guid))
			{
				robotShopkeeperprefab = BossBuilder.BuildPrefab("Friendly_HMPrime", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false, true);
				var companion = robotShopkeeperprefab.AddComponent<FriendlyHMPrimeController>();
				CompanionController yup = companion.gameObject.AddComponent<CompanionController>();
				yup.companionID = CompanionController.CompanionIdentifier.NONE;
                EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, Collection, matRobot, false);

                companion.aiActor.knockbackDoer.weight = 10000;
				companion.aiActor.MovementSpeed = 2.4f;
				companion.aiActor.healthHaver.PreventAllDamage = true;
				companion.aiActor.CollisionDamage = 0f;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(1500f);
				companion.aiActor.healthHaver.SetHealthMaximum(1500f);
				companion.aiActor.CollisionKnockbackStrength = 2f;
				companion.aiActor.CanTargetPlayers = false;
				companion.aiActor.CanTargetEnemies = true;
				companion.aiActor.IgnoreForRoomClear = true;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.gameObject.AddComponent<AfterImageTrailController>().spawnShadows = false;
				companion.aiActor.gameObject.AddComponent<tk2dSpriteAttachPoint>();
				companion.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();
				companion.aiActor.HasShadow = true;
				companion.aiActor.ToggleRenderers(false);

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(1.5f, 0.25f), "shadowPos");



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
					ManualWidth = 36,
					ManualHeight = 40,
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
					ManualWidth = 36,
					ManualHeight = 40,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});


				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				companion.aiActor.reinforceType = ReinforceType.SkipVfx;
				companion.gameObject.AddComponent<HMPrimeEngageDoer>();

				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;
				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.FourWay,
					Prefix = "active",
					AnimNames = new string[]
					{
						"active_top_right",
						"active_bottom_right",
						"active_bottom_left",
						"active_top_left",
					},
					Flipped = new DirectionalAnimation.FlipType[4]
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Prefix = "move",
					Type = DirectionalAnimation.DirectionType.FourWay,
					Flipped = new DirectionalAnimation.FlipType[4],
					AnimNames = new string[]
					{
						"move_top_right",
						"move_bottom_right",
						"move_bottom_left",
						"move_top_left",
					}
				};


				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "broken", new string[] { "broken_right", "broken_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "move", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death_right", "death_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "deathproper", new string[] { "death"}, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "primelaser", new string[1], new DirectionalAnimation.FlipType[1]);

				/*
					 * laserSpeenDown
					 * laserSpeenDownRight
					 * laserSpeenRight
					 * laserSpeenUpRight
					 * laserSpeenUp
					 * laserSpeenUpLeft
					 * laserSpeenLeft
					 * laserSpeenDownLeft
					*/
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "laserSpeen", new string[]{ 
				"laserSpeenRight", 
				"laserSpeenDownRight", 
				"laserSpeenDown",
				"laserSpeenDownLeft", 
				"laserSpeenLeft", 
				"laserSpeenUpLeft",
				"laserSpeenUp",
				"laserSpeenUpRight",
				}, new DirectionalAnimation.FlipType[8], DirectionalAnimation.DirectionType.EightWay);
				
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "cooldownlaser", new string[1], new DirectionalAnimation.FlipType[1]);


				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "ubercharge", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "overcharged", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "chargeball", new string[] { "chargeball_up_right", "chargeball_down_right", "chargeball_down_left", "chargeball_up_left" }, new DirectionalAnimation.FlipType[4], DirectionalAnimation.DirectionType.FourWay);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "fireball", new string[] { "fireball_up_right", "fireball_down_right", "fireball_down_left", "fireball_up_left" }, new DirectionalAnimation.FlipType[4], DirectionalAnimation.DirectionType.FourWay);

				var enemy = EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5");
				Projectile beam = null;
				foreach (Component item in enemy.GetComponentsInChildren(typeof(Component)))
				{
					if (item is BossFinalRogueLaserGun laser)
					{
						if (laser.beamProjectile)
						{
							beam = laser.beamProjectile;
							break;
						}
					}
				}
				

				//companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;

				//bool flag3 = RobotShopkeeperCollection == null;
				//if (flag3)
				{
					/*
					RobotShopkeeperCollection = SpriteBuilder.ConstructCollection(robotShopkeeperprefab, "RobotShopkeeperCollection");
					UnityEngine.Object.DontDestroyOnLoad(RobotShopkeeperCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], RobotShopkeeperCollection);
					}
					*/
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9
					}, "broken_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 2.8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9
					}, "broken_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 2.8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9
					}, "broken", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 2.8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					65,
					66,
					67,
					68,
					}, "active_bottom_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					69,
					70,
					71,
					72,
					}, "active_bottom_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					73,
					74,
					75,
					76,
					}, "active_top_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					77,
					78,
					79,
					80,
					}, "active_top_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					81,
					81,
					81,
					82,
					83,
					84,
					84,
					84,
					85,
					86,
					}, "move_bottom_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6.66f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					87,
					87,
					87,
					88,
					89,
					90,
					90,
					90,
					91,
					92,
					}, "move_bottom_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6.66f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					93,
					93,
					93,
					94,
					95,
					96,
					96,
					96,
					97,
					98,
					}, "move_top_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6.66f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					99,
					99,
					99,
					100,
					101,
					102,
					103,
					104,
					104,
					104
					}, "move_top_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6.66f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					196,
					197,
					196,
					197,
					196,
					197,
					196,
					197,
					196,
					197,
					198,
					199,
					200,
					201,
					202,
					203,
					204,
					205,
					}, "death_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					196,//0
					197,
					196,
					197,
					196,
					197,
					196,
					197,
					196,
					197,//9

					198,
					199,
					200,
					201,
					202,
					203,//15
					204,
					205,
					}, "death_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					206,//0
					207,
					208,
					207,
					208,
					207,
					208,
					207,
					208,
					207,
					208,
					207,
					208,
					207,
					208,//14

					209,//15
					210,
					211,
					212,//18

					213,//19
					213,//20

					214,//21
					214,//22

					215,//23
					215,//24
					
					216,//25
					216,	
					217,//27

					218,//28
					218,//29

					219,//30
					219,
					220,
					220,
					221,
					221,
					221,
					222,
					222,
					223,
					223,
					223,
					224,
					224,
					225,//44
					225,
					225,
					225,
					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					160,//0
					161,//1
					162,
					162,//2
					163,
					163,//3
					164,
					164,
					164,//4
					165,
					165,//5
					165,
					165,
					165,

					166,//6
					166,

					167,//7
					167,

					168,//8
					169,//9
					170,
					
					171,
					172,
					171,
					172,
					171,
					172,
					171,
					172,
					171,
					172,
					171,
					172,

					173,
					173,
					174
					}, "primelaser", tk2dSpriteAnimationClip.WrapMode.Once).fps = 14f;			
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					175,
					}, "laserSpeenDown", tk2dSpriteAnimationClip.WrapMode.Once).fps = 2f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					176,
					}, "laserSpeenDownRight", tk2dSpriteAnimationClip.WrapMode.Once).fps = 2f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					177,
					}, "laserSpeenRight", tk2dSpriteAnimationClip.WrapMode.Once).fps = 2f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					178,
					}, "laserSpeenUpRight", tk2dSpriteAnimationClip.WrapMode.Once).fps = 2f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					179,
					}, "laserSpeenUp", tk2dSpriteAnimationClip.WrapMode.Once).fps = 2f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					180,
					}, "laserSpeenUpLeft", tk2dSpriteAnimationClip.WrapMode.Once).fps = 2f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					181,
					}, "laserSpeenLeft", tk2dSpriteAnimationClip.WrapMode.Once).fps = 2f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					182,
					}, "laserSpeenDownLeft", tk2dSpriteAnimationClip.WrapMode.Once).fps = 2f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					179,
					179,
					179,
					180,
					180,
					181,
					181,
					181,
					182,
					182,
					182,
					175,
					175,
					175,
					175,
					175,

					183,//16
					183,
					183,
					184,
					184,
					185,
					185,
					185,
					185,
					185,
					185,
					185,
					186,
					187,//29
					188,
					189,
					189,
					189,
					190,
					191,
					192,
					193,
					194,
					195
					}, "cooldownlaser", tk2dSpriteAnimationClip.WrapMode.Once).fps = 12f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					128,
					129,
					130,
					131,
					132,
					133,
					133,
					134,
					134,
					134,
					135,
					135
					}, "chargeball_down_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					135,
					134,
					133,
					132,
					130,
					128
					}, "fireball_down_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					136,
					137,
					138,
					139,
					140,
					141,
					141,
					142,
					142,
					142,
					143,
					143
					}, "chargeball_down_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					143,
					142,
					141,
					140,
					138,
					136
					}, "fireball_down_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					144,
					145,
					146,
					147,
					148,
					149,
					149,
					150,
					150,
					150,
					151,
					151
					}, "chargeball_up_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					151,
					150,
					149,
					148,
					146,
					144
					}, "fireball_up_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					152,
					153,
					154,
					155,
					156,
					157,
					157,
					158,
					158,
					158,
					159,
					159
					}, "chargeball_up_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					159,
					158,
					157,
					156,
					154,
					152
					}, "fireball_up_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					105,//0
					105,
					105,
					106,
					106,
					106,
					107,
					107,
					107,
					108,
					108,
					108,
					109,
					109,
					109,//14

					110,//15
					111,
					110,
					111,
					110,
					111,
					110,
					111,//22
					}, "ubercharge", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					112,//0
					113,
					114,
					115,
					116,
					117,
					118,
					119,//7
					120,
					120,
					120,
					120,
					120,
					120,
					120,
					120,
					120,//16

					120,//17
					121,
					120,
					121,
					120,
					121,
					120,
					121,
					120,
					121,
					120,
					121,
					120,
					121,//30

					122,//31
					123,
					124,
					125,
					126,
					127//36
					}, "overcharged", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9
					}, "talk", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 2.8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					10,//0
					10,
					10,
					11,
					11,
					12,
					12,
					13,
					13,
					14,
					14,
					15,
					15,
					16,
					16,
					17,
					17,
					18,
					18,
					19,
					19,//20

					20,//21
					21,
					22,
					23,
					24,
					25,//26

					26,//27
					26,//28

					27,//29
					27,
					27,//31

					28,//32
					29,
					30,
					31,
					32,
					33,//37

					34,//38
					34,//39

					35,//40
					36,
					37,
					38,
					39,
					40,//45
					41,
					42,//47

					43,//48
					43,
					44,//50
					
					

					45,//51
					46,//52

					47,//53
					47,//54

					48,//55
					49,
					50,//57
					51,
					52,
					53,
					54,
					55,//62
					56,
					57,
					58,
					59,
					60,//67

					61,//Sits around for a bit // 68
					62,
					63,
					64,
					
					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7.5f;

				}

				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> {
					{ 2, "Play_ENM_hammer_target_01" },
					{ 4, "Play_ENM_hammer_target_01" },
					{ 6, "Play_ENM_hammer_target_01" },
					{ 8, "Play_ENM_hammer_target_01" },
					{ 10, "Play_ENM_hammer_target_01" },
					{ 12, "Play_ENM_hammer_target_01" },
					{ 14, "Play_ENM_hammer_target_01" },

					{ 16, "Play_BOSS_omegaBeam_charge_01" },

					{ 20, "Play_BOSS_RatMech_Barrel_01" },
					{ 24, "Play_BOSS_RatMech_Barrel_01" },

					{ 30, "Play_WPN_bsg_charge_01" },

					{ 38, "Play_ENM_statue_charge_01" },
					{ 43, "Play_OBJ_nuke_blast_01" },
					{ 44, "Play_BOSS_RatMech_Stomp_01" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> {
					{ 10, "CryingAboutIt" },
					{ 42, "KaBoom" },
					{ 43, "Lights" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "death_left", new Dictionary<int, string> {
					{ 0, "Play_ENM_hammer_target_01" },
					{ 2, "Play_ENM_hammer_target_01" },
					{ 4, "Play_ENM_hammer_target_01" },
					{ 6, "Play_ENM_hammer_target_01" },
					{ 8, "Play_ENM_hammer_target_01" },
					{ 10, "Play_BOSS_RatMech_Squat_01" },
					{ 15, "Play_BOSS_doormimic_land_01" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "death_right", new Dictionary<int, string> {
					{ 0, "Play_ENM_hammer_target_01" },
					{ 2, "Play_ENM_hammer_target_01" },
					{ 4, "Play_ENM_hammer_target_01" },
					{ 6, "Play_ENM_hammer_target_01" },
					{ 8, "Play_ENM_hammer_target_01" },
					{ 10, "Play_BOSS_RatMech_Squat_01" },
					{ 15, "Play_BOSS_doormimic_land_01" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "death_left", new Dictionary<int, string> {
					{ 0, "SetToNotDieKinda" },
					{ 1, "Wimper" },
					{ 15, "Fartd" },

				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "death_right", new Dictionary<int, string> {
					{ 0, "SetToNotDieKinda" },
					{ 1, "Wimper" },
					{ 15, "Fartd" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "cooldownlaser", new Dictionary<int, string> {
					{ 16, "ReleaseSparks" },
					{ 17, "ReleaseSparks" },
					{ 18, "ReleaseSparks" },
					{ 19, "ReleaseSparks" },
					{ 20, "ReleaseSparks" },
					{ 21, "ReleaseSparks" },
					{ 22, "ReleaseSparks" },
					{ 23, "ReleaseSparks" },
					{ 24, "ReleaseSparks" },
					{ 25, "ReleaseSparks" },
					{ 26, "ReleaseSparks" },
					{ 27, "ReleaseSparks" },
					{ 28, "ReleaseSparks" },
					{ 29, "ReleaseSparks" },
				});

				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "cooldownlaser", new Dictionary<int, string> {
					{ 1, "Play_BOSS_omegaBeam_fade_01" },
					{ 17, "Play_BOSS_RatMech_Squat_01" },
					{ 30, "Play_BOSS_RatMech_Target_01" },
					{ 33, "Play_BOSS_RatMech_Barrel_01" },

				});

				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "primelaser", new Dictionary<int, string> {
					{ 2, "Play_BOSS_RatMech_Squat_01" },
					{ 5, "Play_BOSS_RatMech_Lights_01" },
					{ 22, "Play_ENM_hammer_target_01" },
					{ 24, "Play_ENM_hammer_target_01" },
					{ 26, "Play_ENM_hammer_target_01" },
					{ 28, "Play_ENM_hammer_target_01" },
					{ 30, "Play_ENM_hammer_target_01" },
					{ 32, "Play_ENM_hammer_target_01" },
					{ 34, "Play_ENM_hammer_target_01" },
					{ 29, "Play_BOSS_omegaBeam_charge_01" },
				});

				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "primelaser", new Dictionary<int, string> {
					{ 0, "Stop" },
					{ 30, "PrimeLasers" },
				});

				//"fireball_up_right", "fireball_down_right", "fireball_down_left", "fireball_up_left"
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "talk", new Dictionary<int, string> {
					{ 1, "Play_DistressSiren" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "broken", new Dictionary<int, string> {
					{ 1, "Play_DistressSiren" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "broken_left", new Dictionary<int, string> {
					{ 1, "Play_DistressSiren" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "broken_right", new Dictionary<int, string> {
					{ 1, "Play_DistressSiren" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "fireball_up_right", new Dictionary<int, string> {
					{ 1, "Play_BOSS_RatMech_Squat_01" },
					{ 3, "Play_WPN_planetgun_reload_01" },
				});

				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "fireball_down_right", new Dictionary<int, string> {
					{ 1, "Play_BOSS_RatMech_Squat_01" },
					{ 3, "Play_WPN_planetgun_reload_01" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "fireball_down_left", new Dictionary<int, string> {
					{ 1, "Play_BOSS_RatMech_Squat_01" },
					{ 3, "Play_WPN_planetgun_reload_01" },
				});

				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "fireball_up_left", new Dictionary<int, string> {
					{ 1, "Play_BOSS_RatMech_Squat_01" },
					{ 3, "Play_WPN_planetgun_reload_01" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "chargeball_up_right", new Dictionary<int, string> {
					{ 0, "Stop" },
					{ 7, "CreateChargeEffect(UR)" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "chargeball_up_right", new Dictionary<int, string> {
					{ 0, "Play_BOSS_RatMech_Eye_01" },
					{ 1, "Play_BOSS_RatMech_Squat_01" },
					{ 7, "Play_BOSS_RatMech_Shutter_01" },
					{ 11, "Play_BOSS_RatMech_Hop_01" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "chargeball_down_left", new Dictionary<int, string> {
					{ 0, "Stop" },
					{ 7, "CreateChargeEffect(DL)" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "chargeball_down_left", new Dictionary<int, string> {
					{ 0, "Play_BOSS_RatMech_Eye_01" },
					{ 1, "Play_BOSS_RatMech_Squat_01" },
					{ 7, "Play_BOSS_RatMech_Shutter_01" },
					{ 11, "Play_BOSS_RatMech_Hop_01" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "chargeball_up_left", new Dictionary<int, string> {
					{ 0, "Stop" },
					{ 7, "CreateChargeEffect(UL)" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "chargeball_up_left", new Dictionary<int, string> {
					{ 0, "Play_BOSS_RatMech_Eye_01" },
					{ 1, "Play_BOSS_RatMech_Squat_01" },
					{ 7, "Play_BOSS_RatMech_Shutter_01" },
					{ 11, "Play_BOSS_RatMech_Hop_01" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "chargeball_down_right", new Dictionary<int, string> {
					{ 0, "Stop" },
					{ 7, "CreateChargeEffect(DR)" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "chargeball_down_right", new Dictionary<int, string> {
					{ 0, "Play_BOSS_RatMech_Eye_01" },
					{ 1, "Play_BOSS_RatMech_Squat_01" },
					{ 7, "Play_BOSS_RatMech_Shutter_01" },
					{ 11, "Play_BOSS_RatMech_Hop_01" },
				});

				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "ubercharge", new Dictionary<int, string> {
					{ 0, "Stop" },
					{ 22, "BAM" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "overcharged", new Dictionary<int, string> {
					{ 0, "Stop" },
					{ 1, "ReleaseSparks" },
					{ 2, "ReleaseSparks" },
					{ 3, "ReleaseSparks" },
					{ 4, "ReleaseSparks" },
					{ 5, "ReleaseSparks" },
					{ 6, "ReleaseSparks" },
					{ 7, "ReleaseSparks" },
					{ 8, "ReleaseSparks" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "ubercharge", new Dictionary<int, string> {
					{ 0, "Play_BOSS_RatMech_Target_01" },
					{ 2, "Play_BOSS_RatMech_Barrel_01" },
					{ 5, "Play_BOSS_RatMech_Barrel_01" },
					{ 8, "Play_BOSS_RatMech_Barrel_01" },
					{ 11, "Play_BOSS_RatMech_Barrel_01" },
					{ 17, "Play_BOSS_RatMech_Shutter_01" }
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "overcharged", new Dictionary<int, string> {
					{ 0, "Play_BOSS_RatMech_Stomp_01" },
					{ 17, "Play_BOSS_RatMech_Target_01" },
					{ 32, "Play_BOSS_RatMech_Squat_01" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "awaken", new Dictionary<int, string> {
					{ 1, "Play_BOSS_RatMech_Target_01" },
					{ 20, "Play_BOSS_RatMech_Bomb_01" },
					{ 22, "Play_BOSS_RatMech_Squat_01" },
					{ 33, "Play_BOSS_RatMech_Barrel_01" } ,
					{ 43, "Play_BOSS_RatMech_Barrel_01" } ,
					{ 45, "Play_BOSS_RatMech_Stand_01" } ,
					{ 57, "Play_BOSS_RatMech_Eye_01" } ,
					{ 62, "Play_BOSS_RatMech_Target_01" } ,
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "awaken", new Dictionary<int, string> {
					{ 1, "IntroSpeak1" },
					{ 22, "IntroSpeak2" },
					{ 57, "IntroSpeak3" },
					{ 69, "IntroSpeak4" },
					{ 32, "DoStandupDustClouds" },
					{ 0, "StaticBlast" },
					{ 51, "DoublePoof" },
					{ 39, "GunPositionPoofs" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_bottom_left", new Dictionary<int, string> {
					{ 0, "StopVFX(RBDL)" },
					{ 1, "Stop" },
					{ 2, "Stop" },

					{ 3, "Start" },
					{ 4, "Start" },

					{ 5, "StopVFX(LBDL)" },
					{ 6, "Stop" },
					{ 7, "Stop" },

					{ 8, "Start" },
					{ 9, "Start" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_bottom_left", new Dictionary<int, string> {
					{ 2, "Play_CHR_robot_roll_01" },
					{ 7, "Play_CHR_robot_roll_01" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_bottom_right", new Dictionary<int, string> {
					{ 0, "StopVFX(LBDR)" },
					{ 1, "Stop" },
					{ 2, "Stop" },

					{ 3, "Start" },
					{ 4, "Start" },

					{ 5, "StopVFX(RBDR)" },
					{ 6, "Stop" },
					{ 7, "Stop" },

					{ 8, "Start" },
					{ 9, "Start" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_bottom_right", new Dictionary<int, string> {
					{ 2, "Play_CHR_robot_roll_01" },
					{ 7, "Play_CHR_robot_roll_01" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_top_left", new Dictionary<int, string> {
					{ 0, "StopVFX(LTDR)" },
					{ 1, "Stop" },
					{ 2, "Stop" },

					{ 3, "Start" },
					{ 4, "Start" },

					{ 5, "StopVFX(RTDR)" },
					{ 6, "Stop" },
					{ 7, "Stop" },

					{ 8, "Start" },
					{ 9, "Start" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_top_left", new Dictionary<int, string> {
					{ 2, "Play_CHR_robot_roll_01" },
					{ 7, "Play_CHR_robot_roll_01" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_top_right", new Dictionary<int, string> {
					{ 0, "StopVFX(LTDL)" },
					{ 1, "Stop" },
					{ 2, "Stop" },

					{ 3, "Start" },
					{ 4, "Start" },

					{ 5, "StopVFX(RTDL)" },
					{ 6, "Stop" },
					{ 7, "Stop" },

					{ 8, "Start" },
					{ 9, "Start" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_top_right", new Dictionary<int, string> {
					{ 2, "Play_CHR_robot_roll_01" },
					{ 7, "Play_CHR_robot_roll_01" },
				});


				var bs = robotShopkeeperprefab.GetComponent<BehaviorSpeculator>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

				GameObject fuck = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.5f, 1.8125f), "LeftGun_Down(Left)");
				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(2.3125f, 1.5f), "RightGun_Down(Left)");

				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.75f, 1.4375f), "LeftGun_Down(Right)");
				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(2.5625f, 1.8125f), "RightGun_Down(Right)");

				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.375f, 1.625f), "LeftGun_Up(Left)");
				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(2.25f, 2.125f), "RightGun_Up(Left)");

				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.8125f, 2.125f), "LeftGun_Up(Right)");
				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(2.5625f, 1.8125f), "RightGun_Up(Right)");

				GameObject center = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(1.5f, 1.25f), "AbsoluteCenter");
				GameObject Lasercenter = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(1.5f, 1.625f), "BeamAbsoluteCenter");

				BeholsterController behCont = EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b").GetComponent<BeholsterController>();


				AIBeamShooter2 bholsterbeam1 = companion.gameObject.AddComponent<AIBeamShooter2>();
				bholsterbeam1.beamTransform = Lasercenter.transform;
				bholsterbeam1.beamModule = behCont.beamModule;
				bholsterbeam1.beamProjectile = behCont.projectile;
				bholsterbeam1.firingEllipseCenter = Lasercenter.transform.position;
				bholsterbeam1.name = "LeftBeam";
				bholsterbeam1.northAngleTolerance = 180;
				bholsterbeam1.firingEllipseA = 1;
				bholsterbeam1.firingEllipseB = 1;

				AIBeamShooter2 bholsterbeam2 = companion.gameObject.AddComponent<AIBeamShooter2>();
				bholsterbeam2.beamTransform = Lasercenter.transform;
				bholsterbeam2.beamModule = behCont.beamModule;
				bholsterbeam2.beamProjectile = behCont.projectile;
				bholsterbeam2.firingEllipseCenter = Lasercenter.transform.position;
				bholsterbeam2.name = "RightBeam";
				bholsterbeam2.northAngleTolerance = 0;
				bholsterbeam2.firingEllipseA = 1;
				bholsterbeam2.firingEllipseB = 1;





				//bs.MovementBehaviors.Add(seek);



				bs.TargetBehaviors = new List<TargetBehaviorBase>
				{
				new TargetEnemiesBehavior
				{
					LineOfSight = false,
					ObjectPermanence = true,
					SearchInterval = 0.25f,
				}
				};




				bs.MovementBehaviors = new List<MovementBehaviorBase>
			{
				new SeekTargetBehavior() {
					StopWhenInRange = false,
					CustomRange = 6,
					LineOfSight = true,
					ReturnToSpawn = true,
					SpawnTetherDistance = 0,
					PathInterval = 0.5f,
					SpecifyRange = false,
					MinActiveRange = -0.25f,

					MaxActiveRange = 0
				}
				};
				CompanionFollowPlayerBehavior comp = new CompanionFollowPlayerBehavior();
				comp.CanRollOverPits = false;
				comp.DisableInCombat = true;
				comp.IdleAnimations = new string[] { "idle" };
				comp.PathInterval = 0.25f;
				comp.IdealRadius = 3;
				comp.CatchUpRadius = 5;
				comp.CatchUpAccelTime = 1;
				comp.TemporarilyDisabled = false;
				comp.CatchUpMaxSpeed = 2.4f;
				comp.CatchUpSpeed = 2.4f;

				bs.MovementBehaviors.Add(comp);
				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
				{
					
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 4f,
					Behavior = new ShootBehavior{
					ShootPoint = fuck,
					BulletScript = new CustomBulletScriptSelector(typeof(FriendlyHMPrime.CleanSweeps)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 5f,
					RequiresLineOfSight = true,
					MultipleFireEvents = true,
					Uninterruptible = true,
						},
						NickName = "aaaaa"

					},
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 01f,
					Behavior = new ShootBehavior{
					ShootPoint = fuck,
					BulletScript = new CustomBulletScriptSelector(typeof(FriendlyHMPrime.ShootGun)),
					LeadAmount = 0f,
					AttackCooldown = 0.5f,
					Cooldown = 4f,
					//TellAnimation = "chargeball",
					//FireAnimation = "bottle",
					RequiresLineOfSight = true,

					MultipleFireEvents = true,
					Uninterruptible = true,
						},
						NickName = "Gun"

					},
					/*
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 5f,
					Behavior = new ShootBehavior{
					ShootPoint = center,
					BulletScript = new CustomBulletScriptSelector(typeof(RobotShopkeeperBoss.BigBomb)),
					LeadAmount = 0f,
					AttackCooldown = 0.33f,
					Cooldown = 12f,
					RequiresLineOfSight = true,
					InitialCooldown = 8,
					MultipleFireEvents = true,
					Uninterruptible = true,
					TellAnimation = "chargeball",
					FireAnimation = "fireball",
					MaxEnemiesInRoom = 4,
						},
						NickName = "LargeBomb"

					},
					*/
					
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 1f,
					Behavior = new ShootBehavior{
					ShootPoint = fuck,
					BulletScript = new CustomBulletScriptSelector(typeof(FriendlyHMPrime.EatMissiles)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 4f,
					RequiresLineOfSight = true,

					MultipleFireEvents = false,
					Uninterruptible = true,

						},
						NickName = "ROKCTE"

					},
						new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 5f,
					Behavior = new ShootBehavior{
					ShootPoint = center,
					BulletScript = new CustomBulletScriptSelector(typeof(FriendlyHMPrime.Taser)),
					LeadAmount = 0f,
					AttackCooldown = 2f,
					Cooldown = 20f,
					TellAnimation = "ubercharge",
					FireAnimation = "overcharged",
					RequiresLineOfSight = true,
					MultipleFireEvents = true,
					Uninterruptible = true,
						StopDuring = ShootBehavior.StopType.Attack

						},
						NickName = "Taser"
					},
						
				};



				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:ally_hm_prime", companion.aiActor);


			
				/*
				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Ammocom/hmprimeAmmoIcon", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:hm_prime";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Ammocom/hmprimeAmmoIcon";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\hmprimesheet.png");
				PlanetsideModule.Strings.Enemies.Set("#HMPRIME_NAME", "H.M Prime");
				PlanetsideModule.Strings.Enemies.Set("#HMPRIME_SD", "Battle Tower");
				PlanetsideModule.Strings.Enemies.Set("#HMPRIME_LD", "Built by the Hegemony Of Man in preparation for the invasion of the Gungeon, they hoped that this one-core war machince could push back against the forces of the Gungeon.\n\nWhile it mostly failed, it gained the respect of the Gundead with its massive firepower, and left it in the confines of the Gungeon, albeit slightly re-programmed and disassembled.\n\nAny Gungeoneer who finds the machine in low power mode is heavily advised NOT to approach it.");

				companion.encounterTrackable.journalData.PrimaryDisplayName = "#HMPRIME_NAME";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#HMPRIME_SD";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#HMPRIME_LD";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:hm_prime");
				EnemyDatabase.GetEntry("psog:hm_prime").ForcedPositionInAmmonomicon = 201;
				EnemyDatabase.GetEntry("psog:hm_prime").isInBossTab = true;
				EnemyDatabase.GetEntry("psog:hm_prime").isNormalEnemy = true;
				*/

			

				//==================
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(companion.aiActor.healthHaver);

				//==================
			}
		}
	
		public class CleanSweeps : Script
        {
			public override IEnumerator Top()
            {
				base.BulletBank.Bullets.Add(minigunEntryTwo);
				for (int e = (-3); e < (4); e++)
                {
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(true, base.AimDirection, (8)*e, this, 0.75f));
					yield return this.Wait(1);
				}
				yield return this.Wait(30);
				for (int e = (-3); e < (4); e++)
				{
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(false, base.AimDirection, (8) * e, this, 0.75f));
					yield return this.Wait(1);
				}
				yield return this.Wait(75);
				
				yield break;
			}

			private IEnumerator ChargeMinigunsLeft()
			{
				int amount = base.BulletBank.aiActor.GetComponent<RobotShopkeeperEngageDoer>().AmountOfPurchases;
				amount = amount * 2;
				for (int e = (-1 - amount); e < (2 + amount); e++)
				{
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(true, base.AimDirection, (5 + amount) * e, this, 0.75f));
					yield return this.Wait(2.5f);
				}
				yield return this.Wait(90);
				yield break;
            }
			private IEnumerator ChargeMinigunsRight()
			{
				int amount = base.BulletBank.aiActor.GetComponent<RobotShopkeeperEngageDoer>().AmountOfPurchases;
				amount = amount * 2;
				for (int e = (2 + amount); e > (-1 - amount); e--)
				{
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(false, base.AimDirection, (5 + amount) * e, this, 0.75f));
					yield return this.Wait(2.5f);
				}
				yield return this.Wait(90);
				yield break;
			}

			public class BasicBullet : Bullet
			{ public BasicBullet() : base(minigunEntryTwo.Name, false, false, false) { } }
			private IEnumerator QuickscopeNoob(bool isLeft, float aimDir, float offset ,CleanSweeps parent, float chargeTime = 0.5f)
			{
				Vector2 positionLeft = base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2();
				Vector2 positionRight = base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2();
				GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
				Vector2 sre = isLeft == true ? positionLeft : positionRight;

				component2.transform.position = new Vector3(sre.x, sre.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir + offset);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laser = new Color(1f, 0f, 0f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
				float elapsed = 0;
				float Time = chargeTime;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;

					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					if (component2 != null)
					{
						component2.transform.position = isLeft == true ? base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2() : base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2();
						component2.dimensions = new Vector2(Mathf.Lerp(0, 1000, t), 1f);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 50);
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 20);
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir + offset);
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				float die = aimDir + offset;
				elapsed = 0;
				Time = 0.25f;
				while (elapsed < Time)
				{
					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					if (component2 != null)
					{
						component2.transform.position = isLeft == true ? base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2() : base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2();
						component2.dimensions = new Vector2(1000f, 1f);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 50);
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 20);
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.UpdateZDepth();
						bool enabled = elapsed % 0.05f > 0.025f;
						component2.renderer.enabled = enabled;
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				Destroy(component2.gameObject);
				base.PostWwiseEvent("Play_CombineShot", null);
				base.Fire(Offset.OverridePosition(isLeft == true ? base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2() : base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2()), new Direction(die, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new BasicBullet());
				yield break;
			}
		}


		public class AdditionalLinesOfBullets : Script
        {
			public class Shrapnel : Bullet
			{
				public Shrapnel(AdditionalLinesOfBullets parent) : base("poundSmall", false, false, false)
				{ father = parent; }
				public override IEnumerator Top()
				{
					
					while (base.Projectile != null)
                    {
						if (father.IsEnded || father.Destroyed)
                        {
							base.Vanish(false);
                        }
						yield return this.Wait(1);
					}
					yield break;
				}
				private AdditionalLinesOfBullets father;
			}
			
			public override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("amuletRing"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));
				yield return this.Wait(122);
				AIBeamShooter2[] beams = this.BulletBank.aiActor.GetComponents<AIBeamShooter2>();
				for (int e = 0; e < 4; e++)
				{
					if (beams == null || beams.Length == 0)
					{
						break;
					}
					this.PostWwiseEvent("Play_BOSS_doormimic_land_01", null);
					foreach (AIBeamShooter2 beam in beams)
					{
						if (beam && beam.LaserBeam)
						{
							Vector2 overridePosition = beam.LaserBeam.Origin + beam.LaserBeam.Direction.normalized * beam.MaxBeamLength;
							Vector2 vector = beam.LaserBeam.Origin;
							Vector2 vector2 = new Vector2();
							Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;
							int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.EnemyHitBox, CollisionLayer.PlayerHitBox);
							RaycastResult raycastResult2;
							Vector2 Point = MathToolbox.GetUnitOnCircle(beam.LaserBeam.Direction.ToAngle(), 1);
							if (PhysicsEngine.Instance.Raycast(beam.LaserBeam.Origin, Point, 1000, out raycastResult2, true, false, rayMask2, null, false, rigidbodyExcluder, base.BulletBank.aiActor.specRigidbody))
							{
								vector2 = raycastResult2.Contact;
							}
							RaycastResult.Pool.Free(ref raycastResult2);
							int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector, vector2)), 1);
							for (int i = 0; i < num2; i++)
							{
								float t = (float)i / (float)num2;
								Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
								this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle(), DirectionType.Absolute, -1f), new Speed(0), new AdditionalLinesOfBullets.Shrapnel(this));
							}
						}
					}
					yield return this.Wait(37.5f);
				}
				for (int e = 0; e < 10; e++)
                {
					this.PostWwiseEvent("Play_BOSS_lichC_zap_01", null);
					for (int i = 0; i < 20; i++)
					{
						this.Fire(new Direction((18 * i) + 9, DirectionType.Aim, -1f), new Speed(8), new Taser.BasicBullet());
					}
					yield return this.Wait(60);
				}
				yield break;
			}
		}

		public class Taser : Script
        {
			public override IEnumerator Top()
			{
				//Taser.LargeBomb

				//base.BulletBank.aiActor.GetComponent<FriendlyHMPrimeController>().StartDisableAttackFor("LargeBomb", 10f, 5);
				this.m_clms = new List<ChainLightningModifier>();
				this.m_clms2 = new List<ChainLightningModifier>();
				this.m_firstCLM = null;
				this.m_lastCLM = null;
				base.BulletBank.Bullets.Add(taserEntry);
				for (int i = 0; i < 12; i++)
				{
					this.Fire(new Direction(30*i, DirectionType.Absolute, -1f), new Speed(8), new Taser.LingeringBullet(this));
				}
				for (int i = 0; i < 12; i++)
                {
					this.Fire(new Direction(30 * i, DirectionType.Absolute, -1f), new Speed(8), new Taser.BasicBullet());
					this.Fire(new Direction((30 * i)+15, DirectionType.Absolute, -1f), new Speed(7), new Taser.BasicBullet());
				}
				this.m_firstCLM.ForcedLinkProjectile = this.m_lastCLM.projectile;
				this.m_lastCLM.BackLinkProjectile = this.m_firstCLM.projectile;
				yield return this.Wait(30);
				
				yield break;
			}
			private ChainLightningModifier m_firstCLM;
			private ChainLightningModifier m_lastCLM;
			private List<ChainLightningModifier> m_clms;
			private List<ChainLightningModifier> m_clms2;

			public class BasicBullet : Bullet
			{
				public BasicBullet() : base(taserEntry.Name, false, false, false)
				{
				}
			}
			public class LingeringBullet : Bullet
			{
				public LingeringBullet(Taser parent, bool Primary = true) : base(taserEntry.Name, false, false, false)
				{
					father = parent;
					IsPrimary = Primary;
				}
                public override void Initialize()
                {
                    base.Initialize();
					ChainLightningModifier orAddComponent = this.Projectile.gameObject.GetOrAddComponent<ChainLightningModifier>();
					orAddComponent.DamagesPlayers = false;
					orAddComponent.DamagesEnemies = true;
					orAddComponent.RequiresSameProjectileClass = true;
					orAddComponent.LinkVFXPrefab = StaticVFXStorage.FriendlyElectricLinkVFX;
					orAddComponent.damageTypes = CoreDamageTypes.Electric;
					orAddComponent.maximumLinkDistance = 100f;
					orAddComponent.damagePerHit = 0.5f;
					orAddComponent.damageCooldown = 1f;
					orAddComponent.UsesDispersalParticles = false;
					orAddComponent.UseForcedLinkProjectile = true;
					if (father.m_lastCLM != null)
					{
						orAddComponent.ForcedLinkProjectile = father.m_lastCLM.projectile;
						father.m_lastCLM.BackLinkProjectile = orAddComponent.projectile;
					}
					if (father.m_firstCLM == null)
					{
						father.m_firstCLM = orAddComponent;
					}
					father.m_lastCLM = orAddComponent;
					if (IsPrimary == true) { father.m_clms.Add(orAddComponent); }
                    else { father.m_clms2.Add(orAddComponent); }
                }

                public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
					ChainLightningModifier comp = this.Projectile.gameObject.GetOrAddComponent<ChainLightningModifier>();
					comp.UseForcedLinkProjectile = false;
					comp.ForcedLinkProjectile = null;
					Destroy(comp);
					base.OnBulletDestruction(destroyType, hitRigidbody, preventSpawningProjectiles);
				}

                public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 120);
					yield return this.Wait(870);
					GameObject instanceVFX = SpawnManager.SpawnVFX(StaticVFXStorage.EnemyZappyTellVFX, base.Projectile.transform.position, Quaternion.identity);
					tk2dBaseSprite instanceSprite = instanceVFX.GetComponent<tk2dBaseSprite>();
					instanceSprite.PlaceAtPositionByAnchor(base.Projectile.transform.position, tk2dBaseSprite.Anchor.MiddleCenter);

					yield return this.Wait(30);

					if (IsPrimary == true)
                    {
						for (int i = father.m_clms.Count - 1; i >= 0; i--)
						{
							ChainLightningModifier chainLightningModifier = father.m_clms[i];
							if (chainLightningModifier)
							{
								chainLightningModifier.ForcedLinkProjectile = null;
								if (chainLightningModifier.projectile)
								{
									chainLightningModifier.projectile.ForceDestruction();
								}
							}
						}
					}
					else
                    {
						for (int i = father.m_clms2.Count - 1; i >= 0; i--)
						{
							ChainLightningModifier chainLightningModifier = father.m_clms2[i];
							if (chainLightningModifier)
							{
								chainLightningModifier.ForcedLinkProjectile = null;
								if (chainLightningModifier.projectile)
								{
									chainLightningModifier.projectile.ForceDestruction();
								}
							}
						}
					}
				
					if (IsPrimary == true) { father.m_clms.Clear(); }
					else { father.m_clms2.Clear(); }
					base.Vanish(false);
					yield break;
				}
				private bool IsPrimary;
				private Taser father;
			}
		}

		public class EatMissiles : Script
        {
			public override IEnumerator Top()
			{
				//base.BulletBank.Bullets.Add(missileEntry);
				Vector2 positionLeft = base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2();
				Vector2 positionRight = base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2();
				for (int i = 0; i < 4; i++)
				{
					//this.Fire(Offset.OverridePosition(positionLeft),new Direction(UnityEngine.Random.Range(-10, 10), DirectionType.Absolute, -1f), new Speed(9), new EatMissiles.HomingBullet());
					FireRocket(positionLeft);

					yield return this.Wait(20);
					//this.Fire(Offset.OverridePosition(positionRight), new Direction(UnityEngine.Random.Range(-10, 10), DirectionType.Aim, -1f), new Speed(9), new EatMissiles.HomingBullet());
					FireRocket(positionRight);
					yield return this.Wait(20);
				}
				
				yield break;
			}
			public void FireRocket(Vector2 pos)
            {
				GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(39) as Gun).DefaultModule.projectiles[0].gameObject, pos, Quaternion.Euler(0f, 0f, this.AimDirection + UnityEngine.Random.Range(-10, 10)), true);
				Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
				if (component != null)
				{
					component.Owner = GameManager.Instance.BestActivePlayer;
					component.Shooter = base.BulletBank.aiActor.specRigidbody;
					HomingModifier bouncy = component.gameObject.GetOrAddComponent<HomingModifier>();
					bouncy.AngularVelocity = 120;
					bouncy.HomingRadius = 12;
				}
			}
		}
		public class ShootGun : Script
		{
			public override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(minigunEntry);
			
                {
					for (int q = 0; q < 24; q++)
					{
						base.PostWwiseEvent("Play_ITM_Macho_Brace_Fade_01", null);
						base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(true, base.AimDirection, this, 0.75f, 45));
						base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(false, base.AimDirection, this, 0.75f,45));
						yield return this.Wait(5);
					}
				}
			
				yield return this.Wait(120);
				yield break;
			}
			private IEnumerator QuickscopeNoob(bool isLeft, float aimDir, ShootGun parent, float chargeTime = 0.5f, float AimAccuracy = 60)
			{
				Vector2 positionLeft = base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2();
				Vector2 positionRight = base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2();
				GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
				Vector2 sre = isLeft == true ? positionLeft : positionRight;

				component2.transform.position = new Vector3(sre.x, sre.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laser = new Color(1f, 0f, 0f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
				float elapsed = 0;
				float Time = chargeTime;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;

					if (parent.IsEnded|| parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					if (component2 != null)
					{
						component2.transform.position = isLeft == true ? base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2() : base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2();
						Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition(), this.BulletManager.PlayerVelocity(), sre, AimAccuracy);
						component2.dimensions = new Vector2(Mathf.Lerp(0, 1000, t), 1f);
						float CentreAngle = (predictedPosition - this.Position).ToAngle();
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 50);
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 20);
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, CentreAngle);
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				Vector2 predictedPositionOme = BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition(), this.BulletManager.PlayerVelocity(), this.Position, AimAccuracy);

				float shitfart = (predictedPositionOme - this.Position).ToAngle();
				float die = shitfart;
				elapsed = 0;
				Time = 0.25f;
				while (elapsed < Time)
				{
					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					if (component2 != null)
					{
						component2.transform.position = isLeft == true ? base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2() : base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2();
						component2.dimensions = new Vector2(1000f, 1f);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 50);
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 20);
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.UpdateZDepth();
						bool enabled = elapsed % 0.05f > 0.025f;
						component2.renderer.enabled = enabled;
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				Destroy(component2.gameObject);
				base.PostWwiseEvent("Play_CombineShot", null);
				base.Fire(Offset.OverridePosition(isLeft == true ? base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2() : base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2()), new Direction(die + UnityEngine.Random.Range(-4, 4), DirectionType.Absolute, -1f), new Speed(36f, SpeedType.Absolute), new BasicBullet());
				yield break;
			}
			public class BasicBullet : Bullet
			{public BasicBullet() : base("minigun", false, false, false){}		}
		}


		public class FriendlyHMPrimeController : BraveBehaviour
		{
			public void LerpMaterialGlow(Material targetMaterial, float startGlow, float targetGlow, float duration)
			{
				base.StartCoroutine(this.LerpMaterialGlowCR(targetMaterial, startGlow, targetGlow, duration));
			}
			private IEnumerator LerpMaterialGlowCR(Material targetMaterial, float startGlow, float targetGlow, float duration)
			{
				float elapsed = 0f;
				while (elapsed < duration)
				{
					elapsed += BraveTime.DeltaTime; ;
					float t = elapsed / duration;
					if (targetMaterial != null)
					{
						targetMaterial.SetFloat("_EmissivePower", Mathf.Lerp(startGlow, targetGlow, t));
					}
					yield return null;
				}
				yield break;
			}

			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{

				if (clip.GetFrame(frameIdx).eventInfo.Contains("CreateChargeEffect"))
                {
					Vector2 position = ReturnChargeVFXPos(clip.GetFrame(frameIdx).eventInfo);
					VFXPool pool = clip.GetFrame(frameIdx).eventInfo.Contains("U") == true ? StaticVFXStorage.BeholsterChargeUpVFX : StaticVFXStorage.BeholsterChargeDownVFX;
					pool.SpawnAtPosition(base.aiActor.transform.position + position.ToVector3ZisY());
				}
				//deathkinda
				if (clip.GetFrame(frameIdx).eventInfo.Contains("SetToNotDieKinda"))
                {
					base.aiActor.healthHaver.persistsOnDeath = true;
					base.aiActor.healthHaver.PreventAllDamage = true;
				}

				if (clip.GetFrame(frameIdx).eventInfo.Contains("PrimeLasers"))
                {
					for (int i = 0; i < 2; i++)
					{
						Vector2 OFsset = new Vector2(-1.5f, -0.25f);
						if (i == 0)
						{
							OFsset = new Vector2(1.5f, -0.25f);
						}
						StaticVFXStorage.BeholsterChargeUpVFX.SpawnAtPosition(base.aiActor.sprite.WorldCenter + OFsset);
					}
				}
					//BAM
				if (clip.GetFrame(frameIdx).eventInfo.Contains("BAM"))
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
					GameObject vfx = GameObject.Instantiate(silencerVFX.gameObject, base.aiActor.sprite.WorldCenter, Quaternion.identity);
					Destroy(vfx, 2);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("StaticBlast"))
                {
					for (int i = 0; i < 5; i++)
					{
						GameObject breakVFX = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(156) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects[0].effects[0].effect,  base.aiActor.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(1.25f, -1.25f) , UnityEngine.Random.Range(0.625f, -0.625f)), Quaternion.identity);
						tk2dBaseSprite component = breakVFX.GetComponent<tk2dBaseSprite>();
						component.PlaceAtPositionByAnchor(base.aiActor.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(1.25f, -1.25f), UnityEngine.Random.Range(0.625f, -1.25f)), tk2dBaseSprite.Anchor.MiddleCenter);
						component.HeightOffGround = 35f;
						component.UpdateZDepth();
						tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
						if (component2 != null)
						{
							component2.ignoreTimeScale = true;
							component2.AlwaysIgnoreTimeScale = true;
							component2.AnimateDuringBossIntros = true;
							component2.alwaysUpdateOffscreen = true;
							component2.playAutomatically = true;
						}
					}
					
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("DoStandupDustClouds"))
				{
					GameObject PoofVFX = GameManager.Instance.RewardManager.D_Chest.VFX_PreSpawn;
					PoofVFX.SetActive(true);
					GameObject obj = UnityEngine.Object.Instantiate<GameObject>(PoofVFX, base.aiActor.transform.position - new Vector3(0, 1), Quaternion.identity);
					tk2dBaseSprite component = obj.GetComponent<tk2dBaseSprite>();
					component.HeightOffGround = 35f;
					tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
					if (component2 != null)
					{
						component2.ignoreTimeScale = true;
						component2.AlwaysIgnoreTimeScale = true;
						component2.AnimateDuringBossIntros = true;
						component2.alwaysUpdateOffscreen = true;
						component2.playAutomatically = true;
					}
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("DoublePoof"))
				{
					for (int i = 0; i < 2; i++)
					{
						Vector2 OFsset = new Vector2(-1, -1);
						if (i == 0)
						{
							OFsset = new Vector2(1, -1);
						}
						LootEngine.DoDefaultItemPoof(base.aiActor.sprite.WorldCenter + OFsset, true, true);
					}
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("GunPositionPoofs"))
				{
					for (int i = 0; i < 2; i++)
					{
						Vector2 OFsset = new Vector2(-1.5f, 0);
						if (i == 0)
						{
							OFsset = new Vector2(1.5f, 0);
						}
						GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), base.aiActor.sprite.WorldCenter + OFsset, Quaternion.identity);
						tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
						component.PlaceAtPositionByAnchor(base.aiActor.sprite.WorldCenter + OFsset, tk2dBaseSprite.Anchor.MiddleCenter);
						component.HeightOffGround = 35f;
						component.UpdateZDepth();
						tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
						if (component2 != null)
						{
							component2.ignoreTimeScale = true;
							component2.AlwaysIgnoreTimeScale = true;
							component2.AnimateDuringBossIntros = true;
							component2.alwaysUpdateOffscreen = true;
							component2.playAutomatically = true;
							component2.sprite.usesOverrideMaterial = true;
							component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
							component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
							component2.sprite.renderer.material.SetFloat("_EmissivePower", 1);
							component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.1f);
							component2.sprite.renderer.material.SetColor("_OverrideColor", Color.gray);
							component2.sprite.renderer.material.SetColor("_EmissiveColor", Color.gray);
						}
					}
				}
				//Fartd
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Fartd"))
				{
					Exploder.DoRadialMinorBreakableBreak(base.gameObject.transform.PositionVector2(), 5);
					GameObject PoofVFX = GameManager.Instance.RewardManager.D_Chest.VFX_PreSpawn;
					PoofVFX.SetActive(true);
					GameObject obj = UnityEngine.Object.Instantiate<GameObject>(PoofVFX, base.aiActor.sprite.WorldBottomCenter - new Vector2(1.5f, 1), Quaternion.identity);
					Destroy(obj, 2);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Wimper"))
				{
					ShowBoxInternal(base.aiActor.GetComponent<TalkDoerLite>().transform.position + new Vector3(3, 3), base.aiActor.transform, 2f, StringTableManager.GetString("#ROBOTSHOPKEEPER_WAWA1"), "TextBox", 0.5f, "golem", false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("CryingAboutIt"))
                {
					ShowBoxInternal(base.aiActor.GetComponent<TalkDoerLite>().transform.position + new Vector3(3, 3), base.aiActor.transform, 2, StringTableManager.GetString("#ROBOTSHOPKEEPER_CRYABOUTIT"), "TextBox", 0.5f, "golem", false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
				}

				if (clip.GetFrame(frameIdx).eventInfo.Contains("Lights"))
                {
					Pixelator.Instance.FadeToColor(1f, Color.white, true, 1f);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("KaBoom"))
				{
					GameObject epicwin = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").GetComponent<BossFinalRogueDeathController>().DeathStarExplosionVFX);
					epicwin.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(base.aiActor.sprite.WorldCenter, tk2dBaseSprite.Anchor.LowerCenter);
					Destroy(epicwin, 10);
				}
			
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Start"))
                {
					base.aiActor.MovementSpeed = 2.4f; 
                }
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Stop"))
                {
					base.aiActor.MovementSpeed = 0.1f;
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("ReleaseSparks"))
                {
					GameObject breakVFX = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(156) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects[0].effects[0].effect, base.aiActor.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(1.25f, -1.25f), UnityEngine.Random.Range(0.625f, -0.625f)), Quaternion.identity);
					tk2dBaseSprite component = breakVFX.GetComponent<tk2dBaseSprite>();
					component.PlaceAtPositionByAnchor(base.aiActor.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(1.25f, -1.25f), UnityEngine.Random.Range(0.625f, -1.25f)), tk2dBaseSprite.Anchor.MiddleCenter);
					component.HeightOffGround = 35f;
					component.UpdateZDepth();
					tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
					if (component2 != null)
					{
						component2.ignoreTimeScale = true;
						component2.AlwaysIgnoreTimeScale = true;
						component2.AnimateDuringBossIntros = true;
						component2.alwaysUpdateOffscreen = true;
						component2.playAutomatically = true;
					}
				}

				if (clip.GetFrame(frameIdx).eventInfo.Contains("StopVFX"))
				{
					base.aiActor.MovementSpeed = 0.1f;
					Dictionary<Vector2, Vector2> positions = ReturnVFXPos(clip.GetFrame(frameIdx).eventInfo);
					if (positions != null)
					{
						foreach (var KeysAndValues in positions)
						{
                            {
								GameObject gameObject = SpawnManager.SpawnVFX(BraveResources.Load<GameObject>("Global VFX/VFX_DBZ_Charge", ".prefab"), false);
								gameObject.transform.position = base.aiActor.transform.position + KeysAndValues.Key.ToVector3ZisY();
								tk2dSpriteAnimator component2 = gameObject.GetComponent<tk2dSpriteAnimator>();
								if (component2 != null)
								{
									foreach (var value in component2.GetClipById(component2.DefaultClipId).frames)
									{
										value.triggerEvent = false;
									}

									component2.ignoreTimeScale = true;
									component2.AlwaysIgnoreTimeScale = true;
									component2.AnimateDuringBossIntros = true;
									component2.alwaysUpdateOffscreen = true;
									component2.playAutomatically = true;
								}
							}
							{
								GameObject gameObject = SpawnManager.SpawnVFX(BraveResources.Load<GameObject>("Global VFX/VFX_DBZ_Charge", ".prefab"), false);
								gameObject.transform.position = base.aiActor.transform.position + KeysAndValues.Value.ToVector3ZisY();
								tk2dSpriteAnimator component2 = gameObject.GetComponent<tk2dSpriteAnimator>();
								if (component2 != null)
								{
									foreach (var value in component2.GetClipById(component2.DefaultClipId).frames)
									{
										value.triggerEvent = false;
									}
									component2.ignoreTimeScale = true;
									component2.AlwaysIgnoreTimeScale = true;
									component2.AnimateDuringBossIntros = true;
									component2.alwaysUpdateOffscreen = true;
									component2.playAutomatically = true;
								}
							}

						}
					}
				}
			}
			public void StartDisableAttackFor(string attackNickName, float Time = 7, float resetWeightTo = 5)
            {
				base.aiActor.StartCoroutine(this.DisableAttackFor(attackNickName, Time, resetWeightTo));
            }

			public IEnumerator DisableAttackFor(string attackNickName, float Time = 7, float resetWeightTo = 5)
			{
				AttackBehaviorGroup.AttackGroupItem yes= new AttackBehaviorGroup.AttackGroupItem();
				for (int j = 0; j < base.aiActor.behaviorSpeculator.AttackBehaviors.Count; j++)
				{
					if (base.behaviorSpeculator.AttackBehaviors[j] is AttackBehaviorGroup && base.behaviorSpeculator.AttackBehaviors[j] != null)
					{
						for (int i = 0; i < (base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup).AttackBehaviors.Count; i++)
						{
							AttackBehaviorGroup.AttackGroupItem attackGroupItem = (base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup).AttackBehaviors[i];
							if ((base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup) != null && attackGroupItem.NickName == attackNickName)
							{
								attackGroupItem.Probability = 0f;
								yes = attackGroupItem;
							}
						}
					}
				}
				float elapsed = 0;
				while (elapsed < Time)
				{
					if (base.aiActor == null || base.aiActor.healthHaver.IsDead) { yield break; }
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				if (yes != null) { yes.Probability = resetWeightTo; }
				yield break;
			}
			private static Vector2 ReturnChargeVFXPos(string Key)
            {
				Vector2 vector2 = new Vector2(0, 0);
				if (Key.Contains("(DL)")) { vector2 = new Vector2(1.375f, 1.4375f);}
				if (Key.Contains("(DR)")) { vector2 = new Vector2(1.6875f, 1.4375f);}
				if (Key.Contains("(UL)")) { vector2 = new Vector2(1.4375f, 1.875f);}
				if (Key.Contains("(UR)")) { vector2 = new Vector2(1.5625f, 1.875f);}
				return vector2;
			}


			private static Dictionary<Vector2, Vector2> ReturnVFXPos(string Key)
            {
				Dictionary<Vector2, Vector2> poses = new Dictionary<Vector2, Vector2>();
				if (Key.Contains("(LBDL)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(2.1875f, 0.375f), new Vector2(1.0625f, 0.8125f) } }; }
				if (Key.Contains("(RBDL)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(0.5625f, 0.5f), new Vector2(2.375f, 0.75f) } };}

				if (Key.Contains("(LBDR)")) { poses = new Dictionary<Vector2, Vector2> { {new Vector2(0.9375f, 0.3125f), new Vector2(2f, 0.8125f) } }; }
				if (Key.Contains("(RBDR)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(2.5625f, 0.5625f), new Vector2(0.75f, 0.75f) } }; }

				if (Key.Contains("(LTDR)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(0.5625f, 0.5f), new Vector2(2.375f, 0.75f) } }; }
				if (Key.Contains("(RTDR)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(2.1875f, 0.375f), new Vector2(1.0625f, 0.8125f) } }; }

				if (Key.Contains("(LTDL)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(2.5625f, 0.5625f), new Vector2(0.75f, 0.75f) } }; }
				if (Key.Contains("(RTDL)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(0.9375f, 0.3125f), new Vector2(2f, 0.8125f) } }; }
				return poses; 
            }

			private static void ShowBoxInternal(Vector3 worldPosition, Transform parent, float duration, string text, string prefabName, float padding, string audioTag, bool instant, TextBoxManager.BoxSlideOrientation slideOrientation, bool showContinueText, bool UseAlienLanguage = false)
			{
				Vector2 dimensions = new Vector2(-1f, -1f);
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load(prefabName, ".prefab"));
				TextBoxManager component = gameObject.GetComponent<TextBoxManager>();
				component.IsScalingUp = true;

				FieldInfo ikjonhhuuh = typeof(TextBoxManager).GetField("boxPadding", BindingFlags.Instance | BindingFlags.NonPublic);
				ikjonhhuuh.SetValue(component, padding);


				FieldInfo leEnabler = typeof(TextBoxManager).GetField("audioTag", BindingFlags.Instance | BindingFlags.NonPublic);
				leEnabler.SetValue(component, audioTag);

				SetText(component ,text, worldPosition, instant, slideOrientation, true, UseAlienLanguage, prefabName == "ThoughtBubble");
				if (parent != null)
				{
					component.transform.parent = parent;
				}
				if (duration >= 0f)
				{
					component.StartCoroutine(TextBoxLifespanCR(gameObject, parent, duration, component));
				}
				if (showContinueText)
				{
					component.ShowContinueText();
				}
				component.StartCoroutine(HandleScaleUp(dimensions, component));
			}
			public static void SetText(TextBoxManager man,string text, Vector3 worldPosition, bool instant = true, TextBoxManager.BoxSlideOrientation slideOrientation = TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, bool showContinueText = true, bool UseAlienLanguage = false, bool clampThoughtBubble = false)
			{

				Transform boxSpriteTransform = PlanetsideReflectionHelper.ReflectGetField<Transform>(typeof(TextBoxManager), "boxSpriteTransform", man);
				tk2dSlicedSprite boxSprite = PlanetsideReflectionHelper.ReflectGetField<tk2dSlicedSprite>(typeof(TextBoxManager), "boxSprite", man);

				Transform textMeshTransform = PlanetsideReflectionHelper.ReflectGetField<Transform>(typeof(TextBoxManager), "textMeshTransform", man);
				tk2dTextMesh textMesh = PlanetsideReflectionHelper.ReflectGetField<tk2dTextMesh>(typeof(TextBoxManager), "textMesh", man);

				Transform continueTextMeshTransform = PlanetsideReflectionHelper.ReflectGetField<Transform>(typeof(TextBoxManager), "continueTextMeshTransform", man);
				tk2dTextMesh continueTextMesh = PlanetsideReflectionHelper.ReflectGetField<tk2dTextMesh>(typeof(TextBoxManager), "continueTextMeshTransform", man);

				float boxPadding = PlanetsideReflectionHelper.ReflectGetField<float>(typeof(TextBoxManager), "boxPadding", man);

				float additionalPaddingLeft = PlanetsideReflectionHelper.ReflectGetField<float>(typeof(TextBoxManager), "additionalPaddingLeft", man);
				float additionalPaddingRight = PlanetsideReflectionHelper.ReflectGetField<float>(typeof(TextBoxManager), "additionalPaddingRight", man);
				float additionalPaddingTop = PlanetsideReflectionHelper.ReflectGetField<float>(typeof(TextBoxManager), "additionalPaddingTop", man);
				float additionalPaddingBottom = PlanetsideReflectionHelper.ReflectGetField<float>(typeof(TextBoxManager), "additionalPaddingBottom", man);

				FieldInfo m_basePosition = typeof(TextBoxManager).GetField("m_basePosition", BindingFlags.Instance | BindingFlags.NonPublic);


				if (boxSpriteTransform == null)
				{
					boxSpriteTransform = boxSprite.transform;
				}
				if (textMeshTransform == null)
				{
					textMeshTransform = textMesh.transform;
				}
				if (continueTextMeshTransform == null && continueTextMesh)
				{
					continueTextMeshTransform = continueTextMesh.transform;
				}
				if (text == string.Empty)
				{
					return;
				}
				text = text.Replace("\\n", Environment.NewLine);
				float x = boxSpriteTransform.localPosition.x;
				float num = -x / (boxSprite.dimensions.x / 16f);
				string text2 = textMesh.GetStrippedWoobleString(text);
				if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE)
				{
					textMesh.LineSpacing = 0.125f;
				}
				else
				{
					textMesh.LineSpacing = 0f;
				}
				if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE)
				{
					textMesh.wordWrapWidth = 350;
				}
				else if (text2.Length < 25)
				{
					textMesh.wordWrapWidth = 250;
				}
				else
				{
					textMesh.wordWrapWidth = 200 + (text2.Length - 25) / 4;
					if (!text2.EndsWith(" "))
					{
						text2 += " ";
					}
				}
				if (Application.isPlaying)
				{
					bool flag = false;
					for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
					{
						flag |= GameManager.Instance.AllPlayers[i].UnderstandsGleepGlorp;
					}
					textMesh.font = GameManager.Instance.DefaultNormalConversationFont;

				}
				textMesh.text = text2;
				textMesh.CheckFontsForLanguage();
				textMesh.ForceBuild();
				Bounds trueBounds =textMesh.GetTrueBounds();
				float num2 = Mathf.Ceil((trueBounds.size.x + boxPadding * 2f + additionalPaddingLeft + additionalPaddingRight) * 16f) / 16f;
				float num3 = Mathf.Ceil((trueBounds.size.y + boxPadding * 2f + additionalPaddingTop + additionalPaddingBottom) * 16f) / 16f;
				if (showContinueText && continueTextMesh)
				{
					num2 += continueTextMesh.GetEstimatedMeshBoundsForString("...").extents.x * 2f;
				}
				float num4 = num2 * 16f;
				float num5 = num3 * 16f;
				if (clampThoughtBubble)
				{
					float num6 = 47f + (Mathf.Max(47f, num4) - 47f).Quantize(23f, VectorConversions.Floor);
					float num7 = 57f + (Mathf.Max(57f, num4) - 57f).Quantize(23f, VectorConversions.Floor);
					if (num6 < num4)
					{
						num6 += 23f;
					}
					if (num7 < num4)
					{
						num7 += 23f;
					}
					float num8 = Mathf.Abs(num6 - num4);
					float num9 = Mathf.Abs(num7 - num4);
					num4 = ((num8 >= num9) ? num7 : num6);
				}
				Vector3 lhs = new Vector3(0f, 0f);
				tk2dSpriteDefinition currentSpriteDef = boxSprite.GetCurrentSpriteDef();
				Vector3 boundsDataExtents = currentSpriteDef.boundsDataExtents;
				if (currentSpriteDef.texelSize.x != 0f && currentSpriteDef.texelSize.y != 0f && boundsDataExtents.x != 0f && boundsDataExtents.y != 0f)
				{
					lhs = new Vector3(boundsDataExtents.x / currentSpriteDef.texelSize.x, boundsDataExtents.y / currentSpriteDef.texelSize.y, 1f);
				}
				lhs = Vector3.Max(lhs, Vector3.one);
				num4 = Mathf.Max(num4, (boxSprite.borderLeft + boxSprite.borderRight) * lhs.x);
				num5 = Mathf.Max(num5, (boxSprite.borderTop + boxSprite.borderBottom) * lhs.y);
				boxSprite.dimensions = new Vector2(num4, num5);
				if (boxSprite.dimensions.x < (boxSprite.borderLeft +boxSprite.borderRight) * lhs.x || boxSprite.dimensions.y < (boxSprite.borderTop + boxSprite.borderBottom) * lhs.y)
				{
					boxSprite.BorderOnly = true;
				}
				else
				{
					boxSprite.BorderOnly = false;
				}
				boxSprite.ForceBuild();
				textMesh.color = man.textColor;
				if (instant)
				{
					textMesh.text = textMesh.PreprocessWoobleSignifiers(text);
					textMesh.Commit();
				}
				else
				{
					textMesh.text = string.Empty;
					textMesh.Commit();
					string text3 = textMesh.PreprocessWoobleSignifiers(text);
					man.StartCoroutine(RevealTextCharacters(man, text3));
				}
				float y = BraveMathCollege.QuantizeFloat(boxSprite.dimensions.y / 16f - boxPadding - additionalPaddingTop, 0.0625f);
				if (textMesh.anchor == TextAnchor.UpperLeft)
				{
					textMeshTransform.localPosition = new Vector3(boxPadding + additionalPaddingLeft, y, -0.1f);
				}
				else if (textMesh.anchor == TextAnchor.UpperCenter)
				{
					textMeshTransform.localPosition = new Vector3(num2 / 2f, y, -0.1f);
				}
				textMeshTransform.localPosition += new Vector3(0.0234375f, 0.0234375f, 0f);
				if (continueTextMesh)
				{
					if (showContinueText)
					{
						Bounds estimatedMeshBoundsForString = continueTextMesh.GetEstimatedMeshBoundsForString("...");
						continueTextMeshTransform.localPosition = new Vector3(num2 - man.continuePaddingRight - estimatedMeshBoundsForString.extents.x * 2f, man.continuePaddingBottom, -0.1f);
					}
					else
					{
						continueTextMesh.text = string.Empty;
						continueTextMesh.Commit();
					}
				}
				switch (slideOrientation)
				{
					case TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT:
						boxSpriteTransform.localPosition = boxSpriteTransform.localPosition.WithX(BraveMathCollege.QuantizeFloat(-1f * num * (boxSprite.dimensions.x / 16f), 0.0625f));
						break;
					case TextBoxManager.BoxSlideOrientation.FORCE_RIGHT:
						num = 0.1f;
						boxSpriteTransform.localPosition = boxSpriteTransform.localPosition.WithX(BraveMathCollege.QuantizeFloat(-1f * num * (boxSprite.dimensions.x / 16f), 0.0625f));
						break;
					case TextBoxManager.BoxSlideOrientation.FORCE_LEFT:
						num = 0.85f;
						boxSpriteTransform.localPosition = boxSpriteTransform.localPosition.WithX(BraveMathCollege.QuantizeFloat(-1f * num * (boxSprite.dimensions.x / 16f), 0.0625f));
						break;
					default:
						boxSpriteTransform.localPosition = boxSpriteTransform.localPosition.WithX(BraveMathCollege.QuantizeFloat(-1f * num * (boxSprite.dimensions.x / 16f), 0.0625f));
						break;
				}
				if (slideOrientation == TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT)
				{
					float num10 = (!Application.isPlaying) ? 0f : GameManager.Instance.MainCameraController.transform.position.x;
					if (worldPosition.x > num10)
					{
						boxSpriteTransform.localPosition = boxSpriteTransform.localPosition.WithX(-1f * (1f - num) * (boxSprite.dimensions.x / 16f));
					}
				}
				man.transform.position = worldPosition;
				man.transform.localScale = Vector3.one * (float)Mathf.Max(1, Mathf.FloorToInt(1f / GameManager.Instance.MainCameraController.CurrentZoomScale));
				m_basePosition.SetValue(man, worldPosition);
				man.UpdateForCameraPosition();
				man.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
			}
			private static IEnumerator RevealTextCharacters(TextBoxManager man, string strippedString)
			{
				Transform boxSpriteTransform = PlanetsideReflectionHelper.ReflectGetField<Transform>(typeof(TextBoxManager), "boxSpriteTransform", man);
				tk2dSlicedSprite boxSprite = PlanetsideReflectionHelper.ReflectGetField<tk2dSlicedSprite>(typeof(TextBoxManager), "boxSprite", man);
				if (boxSpriteTransform == null) { boxSpriteTransform = boxSprite.transform; }
				tk2dTextMesh textMesh = PlanetsideReflectionHelper.ReflectGetField<tk2dTextMesh>(typeof(TextBoxManager), "textMesh", man);
				if (boxSpriteTransform == null) { ETGModConsole.Log("boxSpriteTransform is NULL"); }
				if (textMesh == null) { ETGModConsole.Log("textMesh is NULL"); }

				FieldInfo m_isRevealingText = typeof(TextBoxManager).GetField("m_isRevealingText", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo skipTextReveal = typeof(TextBoxManager).GetField("skipTextReveal", BindingFlags.Instance | BindingFlags.NonPublic);

				string audioTag = PlanetsideReflectionHelper.ReflectGetField<string>(typeof(TextBoxManager), "audioTag", man);
				if (audioTag == null) { ETGModConsole.Log("audioTag is NULL"); }


				m_isRevealingText.SetValue(man, true);
				skipTextReveal.SetValue(man, false);

				while (man.IsScalingUp)
				{
					yield return null;
				}
				float elapsed = 0f;
				float duration = (float)strippedString.Length / 100;
				Renderer boxRenderer = boxSpriteTransform.GetComponent<Renderer>();
				textMesh.inlineStyling = true;
				textMesh.color = Color.black;
				textMesh.visibleCharacters = 0;
				int visibleCharacters = 0;
				if (duration > 0f)
				{
					while (elapsed < duration)
					{
						elapsed += 0.016f;
						float t = elapsed / duration;
						int numCharacters = Mathf.FloorToInt((float)strippedString.Length * t);
						if (numCharacters > 100000)
						{
							numCharacters = 0;
						}
						if (numCharacters > visibleCharacters && boxRenderer.isVisible)
						{
							visibleCharacters = numCharacters;
							if (!string.IsNullOrEmpty(audioTag))
							{
								AkSoundEngine.PostEvent("Play_CHR_" + audioTag + "_voice_01", man.gameObject);
							}
						}
						textMesh.visibleCharacters = visibleCharacters;
						textMesh.text = strippedString;
						textMesh.Commit();
						yield return null;
					}
				}
				textMesh.visibleCharacters = int.MaxValue;
				textMesh.text = strippedString;
				textMesh.Commit();
				m_isRevealingText.SetValue(man, false);
				skipTextReveal.SetValue(man, false);
				yield break;
			}
			private static IEnumerator TextBoxLifespanCR(GameObject target, Transform parent, float lifespan, TextBoxManager man)
			{
				yield return null;
				while (PlanetsideReflectionHelper.ReflectGetField<bool>(typeof(TextBoxManager), "m_isRevealingText", man) == true)
				{
					yield return null;
				}
				float ela = 0f;
				float dura = lifespan;
				while (ela < dura)
				{
					ela += 0.016f;
					yield return null;
				}
				if (parent != null && dura <= ela)
				{
					UnityEngine.Object.Destroy(target);
				}
				
				yield break;
			}
			private static IEnumerator HandleScaleUp(Vector2 prevBoxSize, TextBoxManager man)
			{
				man.IsScalingUp = true;
				if (prevBoxSize.x <= 0f || prevBoxSize.y <= 0f)
				{
					if (man == null) { ETGModConsole.Log("AEEEEEEEEEEEEEE"); }
					Transform targetTransform = man.transform;
					if (targetTransform == null) { ETGModConsole.Log("ada"); }
					float elapsed = 0f;
					float duration = 0.06f;
					while (elapsed < duration)
					{
						elapsed += GameManager.INVARIANT_DELTA_TIME;
						targetTransform.localScale = Vector3.Lerp(new Vector3(0.01f, 0.01f, 1f), Vector3.one * (float)Mathf.Max(1, Mathf.FloorToInt(1f / GameManager.Instance.MainCameraController.CurrentZoomScale)), Mathf.SmoothStep(0f, 1f, elapsed / duration));
						yield return null;
					}
				}
				else
				{
					if (PlanetsideReflectionHelper.ReflectGetField<tk2dSlicedSprite>(typeof(TextBoxManager), "boxSprite", man) == null) { ETGModConsole.Log("AAAAAAAAAAAAAAAAAAA"); }
					Vector2 targetdimensions = PlanetsideReflectionHelper.ReflectGetField<tk2dSlicedSprite>(typeof(TextBoxManager), "boxSprite", man).dimensions;
					if (targetdimensions == null) { ETGModConsole.Log("fuc"); }
					float elapsed2 = 0f;
					float durationModifier = Mathf.Clamp01(((targetdimensions - prevBoxSize).magnitude - 5f) / 10f);
					float duration2 = Mathf.Lerp(0.025f, 0.06f, durationModifier);
					while (elapsed2 < duration2)
					{
						elapsed2 += GameManager.INVARIANT_DELTA_TIME;
						PlanetsideReflectionHelper.ReflectGetField<tk2dSlicedSprite>(typeof(TextBoxManager), "boxSprite", man).dimensions = Vector2.Lerp(prevBoxSize, targetdimensions, Mathf.SmoothStep(0f, 1f, elapsed2 / duration2));
						yield return null;
					}
				}
				man.IsScalingUp = false;
				yield break;
			}



			private void Start()
			{

				//CompanionController comp = base.aiActor.gameActor.GetComponent<CompanionController>();
				//comp.Initialize(GameManager.Instance.BestActivePlayer);
				//base.aiActor.CompanionOwner = GameManager.Instance.BestActivePlayer;

				if (!base.aiActor.IsBlackPhantom)
				{


					Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
					mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
					mat.SetColor("_EmissiveColor", new Color32(0, 255, 54, 255));
					mat.SetFloat("_EmissiveColorPower", 1.55f);
					mat.SetFloat("_EmissivePower", 100);
					mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);

					base.aiActor.sprite.renderer.material = mat;
				}
				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;

				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(base.aiActor.healthHaver);
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
				};
				base.healthHaver.healthHaver.OnDeath += (obj) =>
				{

				}; ;
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");
			}
		}

		public static List<int> Lootdrops = new List<int>
		{
			73,
			85,
			120,
			67,
			224,
			600,
			78
		};





		private static string[] spritePaths = new string[]
		{		
			//idle (broken)
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_001.png",//0
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_002.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_003.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_004.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_005.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_006.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_007.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_008.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_009.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_010.png",//9

			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro1.png",//10
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro2.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro3.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro4.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro5.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro6.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro7.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro8.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro9.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro10.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro11.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro12.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro13.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro14.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro15.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro16.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro17.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro18.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro19.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro20.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro21.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro22.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro23.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro24.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro25.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro26.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro27.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro28.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro29.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro30.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro31.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro32.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro33.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro34.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro35.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro36.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro37.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro38.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro39.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro40.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro41.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro42.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro43.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro44.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro45.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro46.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro47.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro48.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro49.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro50.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro51.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro52.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro53.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro54.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro55.png",//64

			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_down1.png",//65
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_down2.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_down3.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_down4.png",//68

			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_down1.png",//69
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_down2.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_down3.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_down4.png",//72

			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_top1.png",//73
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_top2.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_top3.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_top4.png",//76

			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_top1.png",//77
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_top2.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_top3.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_top4.png",//80



			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_down1.png",//81
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_down2.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_down3.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_down4.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_down5.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_down6.png",//86

			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_down1.png",//87
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_down2.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_down3.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_down4.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_down5.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_down6.png",//92

			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_up1.png",//93
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_up2.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_up3.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_up4.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_up5.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_up6.png",//98

			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_up1.png",//99
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_up2.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_up3.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_up4.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_up5.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_up6.png",//104

			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_uberhcharge_001.png",//105
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_uberhcharge_002.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_uberhcharge_003.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_uberhcharge_004.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_uberhcharge_005.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_uberhcharge_006.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_uberhcharge_007.png",//111

			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_001.png",//112
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_002.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_003.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_004.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_005.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_006.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_007.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_008.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_009.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_010.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_011.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_012.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_013.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_014.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_015.png",
			"Planetside/Resources/Bosses/HMPrime/OverCharge/hmprime_overcharged_016.png",//127

			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_left_down1.png",//128
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_left_down2.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_left_down3.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_left_down4.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_left_down5.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_left_down6.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_left_down7.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_left_down8.png",//135

			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_right_down1.png",//136
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_right_down2.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_right_down3.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_right_down4.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_right_down5.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_right_down6.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_right_down7.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_right_down8.png",//143

			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_left1.png",//144
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_left2.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_left3.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_left4.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_left5.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_left6.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_left7.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_left8.png",//151

			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_right1.png",//152
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_right2.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_right3.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_right4.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_right5.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_right6.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_right7.png",
			"Planetside/Resources/Bosses/HMPrime/FireBall/hmprime_chargeball_up_right8.png",//159

			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_001.png",//160
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_002.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_003.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_004.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_005.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_006.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_007.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_008.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_009.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_010.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_011.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_012.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_013.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_014.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargeuplaser_015.png",//174

			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_firelaser_001.png",//175
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_firelaser_002.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_firelaser_003.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_firelaser_004.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_firelaser_005.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_firelaser_006.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_firelaser_007.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_firelaser_008.png",//182

			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_001.png",//183
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_002.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_003.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_004.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_005.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_006.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_007.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_008.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_009.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_010.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_011.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_012.png",
			"Planetside/Resources/Bosses/HMPrime/FireGigabeam/hmprime_chargedownlaser_013.png",//195

			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_kindadeath1.png",//196
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_kindadeath2.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_kindadeath3.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_kindadeath4.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_kindadeath5.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_kindadeath6.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_kindadeath7.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_kindadeath8.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_kindadeath9.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_kindadeath10.png",//205

			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_001.png",//206
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_002.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_003.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_004.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_005.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_006.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_007.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_008.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_009.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_010.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_011.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_012.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_013.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_014.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_015.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_016.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_017.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_018.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_019.png",
			"Planetside/Resources/Bosses/HMPrime/Death/hmprime_truedeath_020.png",//225
		};
	}
}








