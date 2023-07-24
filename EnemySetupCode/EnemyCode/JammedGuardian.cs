using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using SaveAPI;
using BreakAbleAPI;

namespace Planetside
{


	public class JammedGuardEngageDoer : CustomEngageDoer
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

		private IEnumerator PortalDoer(MeshRenderer portal, float duration = 6, bool DestroyWhenDone = false)
		{
			AkSoundEngine.PostEvent("Play_ENM_reaper_spawn_01", portal.gameObject);
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				float t = elapsed / duration;
				float throne1 = Mathf.Sin(t * (Mathf.PI));
				portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0, 0.3f, throne1));
				portal.material.SetFloat("_HoleEdgeDepth", Mathf.Lerp(12, 2, throne1));
				if (portal.gameObject == null) { yield break;}
				yield return null;
			}
			
			if (DestroyWhenDone == true)
            {
				Destroy(portal.gameObject);
            }
			yield break;
		}

		private IEnumerator DoIntro()
		{
			m_isFinished = false;
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

			if (HasSpawnedPortal != true)
            {
				GameObject portalObj = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX, this.aiActor.sprite.WorldBottomCenter, Quaternion.Euler(0f, 0f, 0f));
				portalObj.layer = this.aiActor.gameObject.layer + (int)GameManager.Instance.MainCameraController.CurrentZOffset;
				portalObj.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
				MeshRenderer mesh = portalObj.GetComponent<MeshRenderer>();
				GameManager.Instance.StartCoroutine(PortalDoer(mesh, 6, true));
				HasSpawnedPortal = true;
			}
			yield return new WaitForSeconds(1f);


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
			this.aiActor.healthHaver.PreventAllDamage = false;
			this.behaviorSpeculator.enabled = true;
			this.aiActor.specRigidbody.RemoveCollisionLayerIgnoreOverride(playerMask);
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


		private bool HasSpawnedPortal;
		private bool m_isFinished;
	}

	public class JammedGuard : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "jammed_guardian";
		//private static tk2dSpriteCollectionData JammedGuardian;
		public static GameObject shootpoint;
		public static void Init()
		{
			JammedGuard.BuildPrefab();
		}

		public static void BuildPrefab()
		{


            var Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("JammedGuardianCollection").GetComponent<tk2dSpriteCollectionData>();
            Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("jamguard material");


            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
			{
				prefab = EnemyBuilder.BuildPrefabBundle("Jammed Guardian", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(0, 0), false, true);
				var companion = prefab.AddComponent<EnemyBehavior>();
                EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, Collection, mat);

                companion.aiActor.knockbackDoer.weight = 10000;
				companion.aiActor.MovementSpeed = 1.75f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(250f);
				companion.aiActor.CollisionKnockbackStrength = 2f;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.healthHaver.SetHealthMaximum(250f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.ActorName = "Jammed Guardian";
				companion.aiActor.name = "Jammed Guardian";
				companion.aiActor.HasShadow = true;
				companion.aiActor.SetIsFlying(true, "Gamemode: Creative");
				companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;

				companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
				companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>().dashColor = Color.red;
				companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>().spawnShadows = true;
				companion.aiActor.gameObject.AddComponent<AfterImageTrailController>().spawnShadows = false;
				companion.aiActor.gameObject.AddComponent<tk2dSpriteAttachPoint>();
				companion.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();
				companion.aiActor.gameObject.AddComponent<AIBeamShooter>();
				companion.aiActor.gameObject.AddComponent<AIBulletBank>();


				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 9,
					ManualOffsetY = 4,
					ManualWidth = 30,
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
					ManualOffsetX = 9,
					ManualOffsetY = 4,
					ManualWidth = 30,
					ManualHeight = 34,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});
				//companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;
				aiAnimator.IdleAnimation = EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "idle", new string[] { "idle_right", "idle_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
			
				aiAnimator.MoveAnimation = EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "run", new string[] { "run_right", "run_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "die", new string[] { "die_right", "die_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "tell", new string[] { "tell_right", "tell_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "attack", new string[] { "attack_right", "attack_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "armtell", new string[] { "armtell_right", "armtell_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "arm", new string[] { "arm_right", "arm_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "armunraise", new string[] { "armunraise_right", "armunraise_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);


				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				companion.aiActor.reinforceType = ReinforceType.SkipVfx;
				companion.aiActor.gameObject.AddComponent<JammedGuardEngageDoer>();

				//bool flag3 = JammedGuardian == null;
				//if (flag3)
				{
					/*
					JammedGuardian = SpriteBuilder.ConstructCollection(prefab, "Guarder_Collection");
					UnityEngine.Object.DontDestroyOnLoad(JammedGuardian);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], JammedGuardian);
					}
					*/
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					6,
					7,
					8,
					9,
					10,
					11



					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5

					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					6,
					7,
					8,
					9,
					10,
					11
					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					12
					}, "tell_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 20f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					19
					}, "tell_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 20f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					12,
					13,
					14,
					15,
					16,
					17,
					18,
					}, "attack_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 12f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					19,
					20,
					21,
					22,
					23,
					24,
					25,
					
					}, "attack_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 12f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					26,

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					34,

					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					42,
					43,
					44,
					45
					}, "armtell_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					46,
					47,
					48,
					49
					}, "armtell_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{			
					50,
					51,
					52,
					}, "arm_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					53,
					54,
					55,
					}, "arm_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					45,
					44,
					43,
					42
					}, "armunraise_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					49,
					48,
					47,
					46
					}, "armunraise_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					56,
					57,
					58,
					59,
					60,
					61,
					62,
					63,
					64,
					65,
					66,
					67,
					68,
					69,//nice
					70,
					71,
					72,
					73,
					74,
					75
					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7;

				}

				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "awaken", new Dictionary<int, string> { { 12, "Play_BOSS_doormimic_vanish_01" }, { 14, "Play_ENM_beholster_teleport_01" } });



				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				shootpoint = new GameObject("fuck");
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = companion.sprite.WorldCenter;
				GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;

				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(1.625f, 1.3125f), "leftHandShootpoint");
				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(1.25f, 1.3125f), "rightHandShootpoint");


				bs.TargetBehaviors = new List<TargetBehaviorBase>
				{
				new TargetPlayerBehavior
				{
					Radius = 150f,
					LineOfSight = false,
					ObjectPermanence = true,
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.1f
					
				}
				};

				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
				{

					new AttackBehaviorGroup.AttackGroupItem()
					{
					Probability = 2f,
					Behavior = new ShootBehavior() {
                    ShootPoint = m_CachedGunAttachPoint,
                    BulletScript = new CustomBulletScriptSelector(typeof(BigBallAttack)),
                    LeadAmount = 0f,
                    AttackCooldown = 1f,
                    Cooldown = 2f,
                    TellAnimation = "attack",
                    FireAnimation = "tell",
                    RequiresLineOfSight = true,
                    MultipleFireEvents = true,
                    Uninterruptible = false,
					},
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1f,
                    Behavior = new ShootBehavior() {
                    ShootPoint = m_CachedGunAttachPoint,
                    BulletScript = new CustomBulletScriptSelector(typeof(FireFastHoming)),
                    LeadAmount = 0f,
                    AttackCooldown = 1f,
                    Cooldown = 3f,
                    TellAnimation = "armtell",
                    FireAnimation = "arm",
                    PostFireAnimation = "armunraise",
                    RequiresLineOfSight = true,
                    MultipleFireEvents = true,
                    Uninterruptible = false,
                    },
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.7f,
                    Behavior = new ShootBehavior() {
                    ShootPoint = m_CachedGunAttachPoint,
                    BulletScript = new CustomBulletScriptSelector(typeof(FireFastHomingHard)),
                    LeadAmount = 0f,
                    AttackCooldown = 1f,
                    Cooldown = 4f,
                    TellAnimation = "armtell",
                    FireAnimation = "arm",
                    PostFireAnimation = "armunraise",
                    RequiresLineOfSight = true,
                    MultipleFireEvents = true,
                    Uninterruptible = false,
                    },
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
					{
					Probability = 1.2f,
					Behavior = new ShootBehavior() {
                    ShootPoint = m_CachedGunAttachPoint,
                    BulletScript = new CustomBulletScriptSelector(typeof(Crosslads)),
                    LeadAmount = 0f,
                    AttackCooldown = 1f,
                    Cooldown = 13,
					InitialCooldown = 5,
                    TellAnimation = "attack",
                    FireAnimation = "tell",
                    RequiresLineOfSight = false,
                    MultipleFireEvents = true,
                    Uninterruptible = false,
                    },
					}
                };


                /*
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				
					new ShootBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(BigBallAttack)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 7f,
					TellAnimation = "attack",
					FireAnimation = "tell",
					RequiresLineOfSight = true,
					MultipleFireEvents = true,
					Uninterruptible = false,
				},
				new ShootBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(FireFastHoming)),
					LeadAmount = 0f,
					AttackCooldown = 2f,
					Cooldown = 1f,
					TellAnimation = "armtell",
					FireAnimation = "arm",
					PostFireAnimation = "armunraise",
					RequiresLineOfSight = true,
					MultipleFireEvents = true,
					Uninterruptible = false,
				},
				
				new ShootBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(Crosslads)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 4,
					TellAnimation = "attack",
					FireAnimation = "tell",
					RequiresLineOfSight = false,
					MultipleFireEvents = true,
					Uninterruptible = false,
				}
				*/
                /*
				new DashBehavior()
				{
					ShootPoint = m_CachedGunAttachPoint,
					dashDistance = 3f,
					dashTime = 0.2f,
					doubleDashChance = 1,
					enableShadowTrail = false,
					Cooldown = 2,
					dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
					warpDashAnimLength = true,
					hideShadow = true,
					RequiresLineOfSight = false,			
				}
				*/
                bs.OtherBehaviors = new List<BehaviorBase>() {
				new CustomSpinBulletsBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					OverrideBulletName = "homing",
					NumBullets = 2,
					BulletMinRadius = 4.5f,
					BulletMaxRadius = 5,
					BulletCircleSpeed = 30,
					BulletsIgnoreTiles = true,
					RegenTimer = 0.1f,
					AmountOFLines = 6,
					
				}
				};
				
				bs.MovementBehaviors = new List<MovementBehaviorBase>() {
				new SeekTargetBehavior() {
					StopWhenInRange = true,
					CustomRange = 120,
					LineOfSight = true,
					ReturnToSpawn = true,
					SpawnTetherDistance = 0,
					PathInterval = 0.5f,
					SpecifyRange = false,
					MinActiveRange = 1,
					MaxActiveRange = 10
				}
				};


				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:jammed_guardian", companion.aiActor);


				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_001", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:jammed_guardian";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("jammedguardicon");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\jammedguardicon.png");

                string basepath = "Planetside/Resources/Enemies/JammedGuardian/SteveShards/";


				DebrisObject skull = BreakableAPIToolbox.GenerateDebrisObject(basepath + "steveSkull.png", true, 0.5f, 3, 540, 180, null, 2.2f, "Play_OBJ_pot_shatter_01", null, 1);
				ShardCluster headCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { skull }, 0.5f, 1.2f, 1, 1, 0.8f);
				SpawnShardsOnDeath HeadAndShoulders = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
				HeadAndShoulders.deathType = OnDeathBehavior.DeathType.Death;
				HeadAndShoulders.breakStyle = MinorBreakable.BreakStyle.BURST;
				HeadAndShoulders.verticalSpeed = 3f;
				HeadAndShoulders.heightOffGround = 2.5f;
				HeadAndShoulders.shardClusters = new ShardCluster[] { headCluster };

				DebrisObject shoulder1 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "bonePart1.png", true, 0.5f, 3, 540, 120, null, 0.9f, "Play_obj_box_break_01", null, 0);
				DebrisObject shoulder2 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "bonePart2.png", true, 0.5f, 3, 360, 120, null, 0.7f, "Play_BOSS_lichA_crack_01", null, 1);
				DebrisObject shoulder3 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "bonePart3.png", true, 0.5f, 3, 720, 180, null, 0.8f, "Play_obj_box_break_01", null, 1);
				DebrisObject shoulder4 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "bonePart4.png", true, 0.5f, 3, 180, 240, null, 0.9f, "Play_BOSS_lichA_crack_01", null, 0);
				ShardCluster BONES = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shoulder1, shoulder2, shoulder3, shoulder4 }, 0.9f, 2f, 2, 4, 1f);
				SpawnShardsOnDeath BodyAndStuff = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
				BodyAndStuff.deathType = OnDeathBehavior.DeathType.Death;
				BodyAndStuff.breakStyle = MinorBreakable.BreakStyle.BURST;
				BodyAndStuff.verticalSpeed = 1.5f;
				BodyAndStuff.heightOffGround = 2f;
				BodyAndStuff.shardClusters = new ShardCluster[] { BONES };

				DebrisObject ribLeft = BreakableAPIToolbox.GenerateDebrisObject(basepath + "steveRibLeft.png", true, 0.5f, 3, 420, 120, null, 0.9f, null, null, 1);
				DebrisObject ribRight = BreakableAPIToolbox.GenerateDebrisObject(basepath + "steveRibRight.png", true, 0.5f, 3, 360, 200, null, 1.1f, null, null, 1);
				ShardCluster ribCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { ribLeft, ribRight }, 1.2f, 2f, 5, 8, 1.33f);
				SpawnShardsOnDeath ribbies = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
				ribbies.deathType = OnDeathBehavior.DeathType.Death;
				ribbies.breakStyle = MinorBreakable.BreakStyle.BURST;
				ribbies.verticalSpeed = 2f;
				ribbies.heightOffGround = 1.5f;
				ribbies.shardClusters = new ShardCluster[] { ribCluster };

				DebrisObject scythe = BreakableAPIToolbox.GenerateDebrisObject(basepath + "scythe.png", true, 0.5f, 3, 120, 180, null, 3f, "Play_OBJ_chalice_clank_01", null, 0);
				ShardCluster scytheCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { scythe }, 0.2f, 1.8f, 1, 1, 0.8f);
				SpawnShardsOnDeath scytheH = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
				scytheH.deathType = OnDeathBehavior.DeathType.Death;
				scytheH.breakStyle = MinorBreakable.BreakStyle.BURST;
				scytheH.verticalSpeed = 3f;
				scytheH.heightOffGround = 1.66f;
				scytheH.shardClusters = new ShardCluster[] { scytheCluster };

				DebrisObject arm1 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "stevearmbottomright.png", true, 0.33f, 3, 360, 180, null, 1.2f, null, null, 1);
				DebrisObject arm2 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "stevearmtomright.png", true, 0.33f, 3, 420, 120, null, 1.2f, null, null, 1);
				ShardCluster armCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { arm1, arm2 }, 0.66f, 2f, 3, 6, 1.2f);
				SpawnShardsOnDeath armC = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
				armC.deathType = OnDeathBehavior.DeathType.Death;
				armC.breakStyle = MinorBreakable.BreakStyle.BURST;
				armC.verticalSpeed = 3f;
				armC.heightOffGround = 1.33f;
				armC.shardClusters = new ShardCluster[] { armCluster };

				DebrisObject tatter1 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "tatter1.png", true, 0.33f, 3, 240, 180, null, 0.4f, null, null, 0);
				DebrisObject tatter2 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "tatter2.png", true, 0.33f, 3, 300, 150, null, 0.2f, null, null, 0);
				DebrisObject tatter3 = BreakableAPIToolbox.GenerateDebrisObject(basepath + "tatter3.png", true, 0.33f, 3, 200, 120, null, 0.33f, null, null, 0);
				ShardCluster tatterCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { tatter1, tatter2, tatter3 }, 1f, 1f, 3, 6, 1.2f);
				SpawnShardsOnDeath taters = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
				taters.deathType = OnDeathBehavior.DeathType.Death;
				taters.breakStyle = MinorBreakable.BreakStyle.BURST;
				taters.verticalSpeed = 3f;
				taters.heightOffGround = 1f;
				taters.shardClusters = new ShardCluster[] { tatterCluster };

				//companion.aiActor.CorpseObject
				PlanetsideModule.Strings.Enemies.Set("#JAMMED_GUARD", "Jammed Guardian");
				PlanetsideModule.Strings.Enemies.Set("#JAMMED_GUARD_SHORTDESC", "You Have Angered The Gods");
				PlanetsideModule.Strings.Enemies.Set("#JAMMED_GUARD_LONGDESC", "Only sent out if Kaliber has been enraged in a particular way, this guardian uses the souls of slain Gungeoneers to shield itself from oncoming attacks.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#JAMMED_GUARD";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#JAMMED_GUARD_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#JAMMED_GUARD_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:jammed_guardian");
				EnemyDatabase.GetEntry("psog:jammed_guardian").ForcedPositionInAmmonomicon = 1000;
				EnemyDatabase.GetEntry("psog:jammed_guardian").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:jammed_guardian").isNormalEnemy = true;


                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet("default"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("homing"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("homingPop"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("quickHoming"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));

            }
		}


		private static string[] spritePaths = new string[]
		{
			
			//idles
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_001.png",//0
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_left_006.png",//5

			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_right_001.png",//6
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_right_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_right_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_right_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_right_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_idle_right_006.png",//11

			
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_001.png",//12
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_006.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_left_007.png",//18

			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_001.png",//19
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_006.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_attack_right_007.png",//25

			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_001.png",//26
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_006.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_007.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_left_008.png",//33

			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_001.png",//34
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_006.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_007.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_death_right_008.png",//41

			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shootwitharm_left_001.png",//42
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shootwitharm_left_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shootwitharm_left_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shootwitharm_left_004.png",//45

			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shootwitharm_right_001.png",//46
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shootwitharm_right_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shootwitharm_right_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shootwitharm_right_004.png",//49

			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shooty_left_001.png",//50
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shooty_left_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shooty_left_003.png",//52

			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shooty_right_001.png",//53
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shooty_right_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_shooty_right_003.png",//55
			
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_001.png",//56
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_002.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_003.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_004.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_005.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_006.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_007.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_008.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_009.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_010.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_011.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_012.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_013.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_014.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_015.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_016.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_017.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_018.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_019.png",
			"Planetside/Resources/Enemies/JammedGuardian/stevelord_spawn_right_020.png",//75

		};

		public class EnemyBehavior : BraveBehaviour
		{

			private RoomHandler m_StartRoom;
			private void Update()
			{
                //if (!base.aiActor.HasBeenEngaged) { CheckPlayerRoom(); }
				//base.aiActor.sprite.sprite.CurrentSprite.dim

            }
			private void CheckPlayerRoom()
			{
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
				{
					base.aiActor.HasBeenEngaged = true;
				}
			}
			private void Start()
			{
				if (!base.aiActor.IsBlackPhantom)
				{
					Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
					mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
					mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
					mat.SetFloat("_EmissiveColorPower", 1.55f);
					mat.SetFloat("_EmissivePower", 100);
					aiActor.sprite.renderer.material = mat;
				}

				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(70).gameObject, base.aiActor.sprite.WorldCenter, Vector2.up, 1f, false, true, false);
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(70).gameObject, base.aiActor.sprite.WorldCenter, Vector2.down, 1f, false, true, false);
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(70).gameObject, base.aiActor.sprite.WorldCenter, Vector2.left, 1f, false, true, false);
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(70).gameObject, base.aiActor.sprite.WorldCenter, Vector2.right, 1f, false, true, false);

                    AkSoundEngine.PostEvent("Play_GuardDie", base.aiActor.gameObject);
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED, true);
				};
			}
		}

		public class Crosslads : Script
		{
			public override IEnumerator Top()
			{
                base.PostWwiseEvent("Play_EnergySwirl", null);

                this.EndOnBlank = true;
				float startDirection = this.RandomAngle();
				float Bullets = 18;

				float scytleBullets = 18;
				for (int i = 0; i < Bullets; i++)
				{
					float t = (float)i / (float)Bullets;
					this.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new Crosslads.SmokeBullet(startDirection, Mathf.Lerp(-3, 3, t)));
					if (i <= 5) 
					{
						this.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new Crosslads.SmokeBullet(startDirection-3, Mathf.Lerp(-3, 3, t)));
						this.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new Crosslads.SmokeBullet(startDirection+3, Mathf.Lerp(-3, 3, t)));
					}
				}
				for (int i = 0; i < scytleBullets; i++)
                {
					float t = (float)i / (float)scytleBullets;
					this.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new Crosslads.SmokeBullet(startDirection + Mathf.Lerp(-5, 75, t), 2.75f, true)); 
					this.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new Crosslads.SmokeBullet(startDirection + Mathf.Lerp(-5, 60, t), 2.875f, true));
					this.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new Crosslads.SmokeBullet(startDirection + Mathf.Lerp(-5,45, t), 3f, true));
				}

				yield return this.Wait(45);
				yield break;
			}

			public class SmokeBullet : Bullet
			{
				public SmokeBullet(float angle, float radiuscap, bool isScytheHead = false) : base( isScytheHead == false ? "spore2" : "default", false, false, false)
				{
					this.m_angle = angle;
					this.radiusCap = radiuscap;
				}

				public override IEnumerator Top()
				{
					this.ManualControl = true;
					Vector2 centerPosition = this.Position;
					float radius = 0f;
					int i = 0;
					while ((float)i < 600f)
					{
						float desiredAngle = (this.BulletManager.PlayerPosition() - centerPosition).ToAngle();
						this.Direction = Mathf.MoveTowardsAngle(this.Direction, desiredAngle, 1.33f);
						float speedScale = 0.5f;
						if (i < 60)
						{
							speedScale = Mathf.SmoothStep(0f, 1f, (float)i / 60f);
						}
						this.UpdateVelocity();
						centerPosition += this.Velocity / 60f * speedScale;
						if (i < 60)
						{
							radius += radiusCap/60;

						}
						this.m_angle += 2f;
						this.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);
						yield return this.Wait(1);
						i++;
					}
					this.Vanish(false);
					yield break;
				}

				private const float ExpandSpeed = 2f;

				private const float SpinSpeed = 120f;

				private const float Lifetime = 600f;

				private float radiusCap;
				private float m_angle;
			}
		}

		public class FireFastHomingHard : FireFastHoming
		{
			public override bool IsHard => true;
		}


        public class FireFastHoming : Script
        {
			public virtual bool IsHard
			{
				get 
				{
					return false;
				}
			}

			public override IEnumerator Top()
			{
				if (IsHard == true)
				{
                    for (int i = 0; i < 12; i++)
                    {
                        this.Fire(new Offset(((BraveMathCollege.AbsAngleBetween(this.BulletBank.aiAnimator.FacingDirection, 0f) <= 90f) ? "leftHandShootpoint" : "rightHandShootpoint")), new Direction(i * 30, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new FireFastHoming.FastHomingShot(5, 20, 210));
                    }
                    yield return this.Wait(30);
                    for (int i = -3; i < 4; i++)
                    {
                        this.Fire(new Offset(((BraveMathCollege.AbsAngleBetween(this.BulletBank.aiAnimator.FacingDirection, 0f) <= 90f) ? "leftHandShootpoint" : "rightHandShootpoint")), new Direction(i * 10, DirectionType.Aim, -1f), new Speed(14f, SpeedType.Absolute), new FireFastHoming.FastHomingShot(1, 9, 150));
                    }
                }
                else
				{
                    for (int e = 0; e < 4; e++)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            this.Fire(new Offset(((BraveMathCollege.AbsAngleBetween(this.BulletBank.aiAnimator.FacingDirection, 0f) <= 90f) ? "leftHandShootpoint" : "rightHandShootpoint")), new Direction(i * 60, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new FireFastHoming.FastHomingShot(2));
                        }
                        yield return this.Wait(30);
                    }
                }

                 
                yield break;
			}

			public class FastHomingShot : Bullet
			{
				public FastHomingShot(float delta, float postSpeed = 14, int term = 120) : base("quickHoming", false, false, false)
				{
					Delta = delta;
					PostSpeed = postSpeed;
					Term = term;
                }

				public override IEnumerator Top()
				{
					this.ChangeSpeed(new Speed(PostSpeed, SpeedType.Absolute), Term);
					for (int i = 0; i < 30; i++)
					{
						float aim = this.GetAimDirection(1f, 16f);
						float delta = BraveMathCollege.ClampAngle180(aim - this.Direction);
						if (Mathf.Abs(delta) > 180f)
						{
							yield break;
						}
						this.Direction += Mathf.MoveTowards(0f, delta, Delta);
						yield return this.Wait(1);
					}
					yield break;
				}
				private float Delta;
                private float PostSpeed;
                private int Term;

            }
        }

		public class BigBallAttack : Script
		{
			public override IEnumerator Top() 
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
				}
				base.PostWwiseEvent("Play_ENM_kali_shockwave_01", null);
				this.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new BigBallAttack.Superball());
				yield return base.Wait(20);
				yield break;
			}
			public class Flames : Bullet
			{
				public Flames() : base("spiral", false, false, false)
				{

				}
				public override IEnumerator Top()
				{
					this.Projectile.specRigidbody.CollideWithOthers = false;
					base.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 25);
					yield break;
				}
			}
			public class Superball : Bullet
			{
				public Superball() : base("big", false, false, false)
				{
				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(40f, SpeedType.Absolute), 180);
					yield break;

				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
					{
					}
					if (!preventSpawningProjectiles)
					{
						base.PostWwiseEvent("Play_OBJ_nuke_blast_01", null);
						float num = base.RandomAngle();
						float Amount = 12;
						float Angle = 360 / Amount;
						for (int i = 0; i < Amount; i++)
						{
							base.Fire(new Direction(num + Angle * (float)i, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new SpeedChangingBullet("default", 7, 150));
							base.Fire(new Direction(num + Angle * (float)i+15, DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new SpeedChangingBullet("default", 7, 150));
                            base.Fire(new Direction(num + Angle * (float)i, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new SpeedChangingBullet("default", 7, 150));
                        }
					}
				}
			}

		}
	}
}








