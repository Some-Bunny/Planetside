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
using EnemyBulletBuilder;
using SaveAPI;
namespace Planetside
{
	class Fungannon : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "Fungannon";
		public static GameObject shootpoint;

		public static GameObject Cannon1;
		public static GameObject Cannon2;
		public static GameObject Cannon3;
		public static GameObject Cannon4;

		private static tk2dSpriteCollectionData FunganonClooection;


		public static List<int> spriteIds2 = new List<int>();

		private static Texture2D BossCardTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside/Resources/BossCards/fungannon_bosscard.png");

		public static void Init()
		{
			Fungannon.BuildPrefab();
		}
		public static void BuildPrefab()
		{
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);

			bool flag2 = flag;
			if (!flag2)
			{
				prefab = BossBuilder.BuildPrefab("Fungannon", guid, "Planetside/Resources/Fungannon/Fungannon_idle_001.png", new IntVector2(0, 0), new IntVector2(0, 0), false, true);
				var enemy = prefab.AddComponent<EnemyBehavior>();
				FungannonController pain = prefab.AddComponent<FungannonController>();

				AIAnimator aiAnimator = enemy.aiAnimator;

				enemy.aiActor.knockbackDoer.weight = 35;
				enemy.aiActor.MovementSpeed = 1.75f;
				enemy.aiActor.healthHaver.PreventAllDamage = false;
				enemy.aiActor.CollisionDamage = 1f;
				enemy.aiActor.HasShadow = false;
				enemy.aiActor.IgnoreForRoomClear = false;
				enemy.aiActor.aiAnimator.HitReactChance = 0f;
				enemy.aiActor.specRigidbody.CollideWithOthers = true;
				enemy.aiActor.specRigidbody.CollideWithTileMap = true;
				enemy.aiActor.PreventFallingInPitsEver = false;
				enemy.aiActor.healthHaver.ForceSetCurrentHealth(1075f);
				enemy.aiActor.CollisionKnockbackStrength = 10f;
				enemy.aiActor.CanTargetPlayers = true;
				enemy.aiActor.healthHaver.SetHealthMaximum(1075f, null, false);

				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "idle",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};

				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
	                {
						"moveright", //Good
						"moveleft",//Good


	                }
				};

				//=====================================================================================
				DirectionalAnimation anim = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"roar",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "roar",
						anim = anim
					}
				};
				//=====================================================================================
				DirectionalAnimation ctahge = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
	{
						"charge",

	},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "charge",
						anim = ctahge
					}
				};
				//=====================================================================================
				DirectionalAnimation BirdUp = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"chargecannon",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "chargecannon",
						anim = ctahge
					}
				};
				//=====================================================================================
				DirectionalAnimation eee = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
	                {
						"jump",

	                },
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "jump",
						anim = eee
					}
				};
				//=====================================================================================

				//=====================================================================================
				DirectionalAnimation anim3 = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"jumpland",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "jumpland",
						anim = anim3
					}
				};
				//=====================================================================================

				DirectionalAnimation almostdone = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "intro",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "intro",
						anim = almostdone
					}
				};
				DirectionalAnimation done = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "death",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "death",
						anim = done
					}
				};


				bool flag3 = FunganonClooection == null;
				if (flag3)
				{
					FunganonClooection = SpriteBuilder.ConstructCollection(prefab, "FungalLadCollection");
					UnityEngine.Object.DontDestroyOnLoad(FunganonClooection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], FunganonClooection);
					}
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, FunganonClooection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6

					}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;

					SpriteBuilder.AddAnimation(enemy.spriteAnimator, FunganonClooection, new List<int>
					{

					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15

					}, "moveleft", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, FunganonClooection, new List<int>
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

					}, "moveright", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;

					SpriteBuilder.AddAnimation(enemy.spriteAnimator, FunganonClooection, new List<int>
					{

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
					35,//
					36,
					37,
					
					35,//
					36,
					37,
					
					35,//
					36,
					37,
					35,//
					36,
					37,
					38,
					39,
					40

					}, "roar", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;


					SpriteBuilder.AddAnimation(enemy.spriteAnimator, FunganonClooection, new List<int>
					{

					41,
					42,
					43,
					44,
					45,
					46,
					47,
					48,
					49,
					50,
					51,
					51,
					52,
					53,

					}, "jump", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;


					SpriteBuilder.AddAnimation(enemy.spriteAnimator, FunganonClooection, new List<int>
					{
					
					53,
					53,
					54,
					55

					}, "jumpland", tk2dSpriteAnimationClip.WrapMode.Once).fps =9f;



					SpriteBuilder.AddAnimation(enemy.spriteAnimator, FunganonClooection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6,
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					0,
					1,
					2,
					3,
					4,
					5,
					6,
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
					35,//
					36,
					37,


					}, "intro", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(enemy.spriteAnimator, FunganonClooection, new List<int>
					{
				66,
				67,
				68,
				69,
				70,
				71,
				72,
				73,
				74,
				75,
				76,
				77,
				78,
				76,
				77,
				78,
				76,
				77,
				78,
				76,
				77,
				78,
				76,
				77,
				78,
				76,
				77,
				78
					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

					SpriteBuilder.AddAnimation(enemy.spriteAnimator, FunganonClooection, new List<int>
					{
				    25,
					26,
					27,
					28,
					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					27,
					28,
					29,

					27,
					26,
					25
					}, "charge", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;

					SpriteBuilder.AddAnimation(enemy.spriteAnimator, FunganonClooection, new List<int>
					{
					25,
					26,
					27,
					28,
					27,
					26,
					27,
					28,
					29,
					28,
					29,

					}, "chargecannon", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
				}

				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("chargecannon").frames[1].eventAudio = "Play_BOSS_dragun_charge_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("chargecannon").frames[1].triggerEvent = true;

				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("jump").frames[4].eventAudio = "Play_ENM_bigshroom_jump_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("jump").frames[4].triggerEvent = true;
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("jump").frames[3].eventAudio = "Play_ENM_statue_jump_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("jump").frames[3].triggerEvent = true;
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("jumpland").frames[1].eventAudio = "Play_ENM_cannonball_blast_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("jumpland").frames[1].triggerEvent = true;
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("roar").frames[5].eventAudio = "Play_VO_lichB_death_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("roar").frames[5].triggerEvent = true;
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[26].eventAudio = "Play_VO_lichB_death_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[26].triggerEvent = true;

				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("death").frames[8].eventAudio = "Play_VO_lichB_death_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("death").frames[8].triggerEvent = true;



				enemy.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[3].eventInfo = "spawnSizeUp";
				enemy.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("death").frames[10].eventInfo = "deathOno";




				enemy.aiActor.specRigidbody.PixelColliders.Clear();
				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 17,
					ManualOffsetY = 0,
					ManualWidth = 66,
					ManualHeight = 66,
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
					ManualOffsetX = 17,
					ManualOffsetY = 0,
					ManualWidth = 66,
					ManualHeight = 66,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});

				enemy.aiActor.PreventBlackPhantom = false;


				shootpoint = new GameObject("CentreOfAllCannons");
				shootpoint.transform.parent = enemy.transform;
				shootpoint.transform.position = new Vector2(2.875f, 2.875f);
				GameObject CentreOfAllCannons = enemy.transform.Find("CentreOfAllCannons").gameObject;

				Cannon1 = new GameObject("CannonNorth");
				Cannon1.transform.parent = enemy.transform;
				Cannon1.transform.position = new Vector2(2.875f, 2.75f);
				GameObject CannonNorth = enemy.transform.Find("CannonNorth").gameObject;

				Cannon2 = new GameObject("CannonEast");
				Cannon2.transform.parent = enemy.transform;
				Cannon2.transform.position = new Vector2(5.125f, 2.625f);
				GameObject CannonEast= enemy.transform.Find("CannonEast").gameObject;

				Cannon3 = new GameObject("CannonSouth");
				Cannon3.transform.parent = enemy.transform;
				Cannon3.transform.position = new Vector2(2.875f, 3.1875f);
				GameObject CannonSouth = enemy.transform.Find("CannonSouth").gameObject;

				Cannon4 = new GameObject("CannonWest");
				Cannon4.transform.parent = enemy.transform;
				Cannon4.transform.position = new Vector2(0.625f, 2.625f);
				GameObject CannonWest = enemy.transform.Find("CannonWest").gameObject;


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

				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>()
				{

					new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 1.5f,
						Behavior = new ShootBehavior()
						{

							BulletScript = new CustomBulletScriptSelector(typeof(MegaCannon)),
							LeadAmount = 0,

							AttackCooldown = 3f,
							Cooldown = 8f,

							RequiresLineOfSight = true,
							ShootPoint = CentreOfAllCannons,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 1,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
							MinWallDistance = 0,
							MaxEnemiesInRoom = -1,
							MinHealthThreshold = 0,
							MaxHealthThreshold = 1,
							HealthThresholds = new float[0],
							AccumulateHealthThresholds = true,
							targetAreaStyle = null,
							IsBlackPhantom = false,
							resetCooldownOnDamage = null,
							MaxUsages = 0,
							ImmobileDuringStop = false,
							ChargeAnimation = "chargecannon",
							//FireAnimation = "jumpland",
							PostFireAnimation = "jumpland",
							StopDuring = ShootBehavior.StopType.Charge

						},
						NickName = "ffefefefe"
					},

					new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 1.1f,
						Behavior = new ShootBehavior()
						{

							BulletScript = new CustomBulletScriptSelector(typeof(PrimaryCannonScript)),
							LeadAmount = 0,
							
							AttackCooldown = 2f,
							Cooldown = 6f,

							RequiresLineOfSight = true,
							ShootPoint = CentreOfAllCannons,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 4,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
							MinWallDistance = 0,
							MaxEnemiesInRoom = -1,
							MinHealthThreshold = 0,
							MaxHealthThreshold = 1,
							HealthThresholds = new float[0],
							AccumulateHealthThresholds = true,
							targetAreaStyle = null,
							IsBlackPhantom = false,
							resetCooldownOnDamage = null,
							MaxUsages = 0,
							ImmobileDuringStop = false,
							ChargeAnimation = "jump",
							//FireAnimation = "jumpland",
							PostFireAnimation = "jumpland",
							StopDuring = ShootBehavior.StopType.Charge

						},
						NickName = "ffefefefe"
					},
					new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 1f,
						Behavior = new ShootBehavior()
						{

							BulletScript = new CustomBulletScriptSelector(typeof(PooterCannon)),
							LeadAmount = 0,

							AttackCooldown = 0.25f,
							Cooldown = 1f,

							RequiresLineOfSight = true,
							ShootPoint = CentreOfAllCannons,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 0,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
							MinWallDistance = 0,
							MaxEnemiesInRoom = -1,
							MinHealthThreshold = 0,
							MaxHealthThreshold = 1,
							HealthThresholds = new float[0],
							AccumulateHealthThresholds = true,
							targetAreaStyle = null,
							IsBlackPhantom = false,
							resetCooldownOnDamage = null,
							MaxUsages = 0,

						},
						NickName = "dasddsasad"
					},
					new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 1f,
						Behavior = new ShootBehavior()
						{
							StopDuring = ShootBehavior.StopType.Attack,
							BulletScript = new CustomBulletScriptSelector(typeof(RainingPoot)),
							LeadAmount = 0,

							AttackCooldown = 1f,
							Cooldown = 5f,

							RequiresLineOfSight = true,
							ShootPoint = CentreOfAllCannons,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 8,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
							MinWallDistance = 0,
							MaxEnemiesInRoom = -1,
							MinHealthThreshold = 0,
							MaxHealthThreshold = 1,
							HealthThresholds = new float[0],
							AccumulateHealthThresholds = true,
							targetAreaStyle = null,
							IsBlackPhantom = false,
							resetCooldownOnDamage = null,
							MaxUsages = 0,
							FireAnimation = "roar",

						},
						NickName = "raor"

					},
					new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 0.85f,
						Behavior = new ShootBehavior()
						{
							StopDuring = ShootBehavior.StopType.Attack,
							BulletScript = new CustomBulletScriptSelector(typeof(BigEverywhereAttack)),
							LeadAmount = 0,
							
							AttackCooldown = 3f,
							Cooldown = 20f,

							RequiresLineOfSight = true,
							ShootPoint = CentreOfAllCannons,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 8,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
							MinWallDistance = 0,
							MaxEnemiesInRoom = -1,
							MinHealthThreshold = 0,
							MaxHealthThreshold = 1,
							HealthThresholds = new float[0],
							AccumulateHealthThresholds = true,
							targetAreaStyle = null,
							IsBlackPhantom = false,
							resetCooldownOnDamage = null,
							MaxUsages = 0,
							FireAnimation = "charge",
							
						},
						NickName = "pissandshit"

					},
					new AttackBehaviorGroup.AttackGroupItem()
						{
						Probability = 1f,
						Behavior = new ShootBehavior()
						{

							BulletScript = new CustomBulletScriptSelector(typeof(FastCannons)),
							LeadAmount = 0,

							AttackCooldown = 0.7f,
							Cooldown = 0.3f,

							RequiresLineOfSight = true,
							ShootPoint = CentreOfAllCannons,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 2,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
							MinWallDistance = 0,
							MaxEnemiesInRoom = -1,
							MinHealthThreshold = 0,
							MaxHealthThreshold = 1,
							HealthThresholds = new float[0],
							AccumulateHealthThresholds = true,
							targetAreaStyle = null,
							IsBlackPhantom = false,
							resetCooldownOnDamage = null,
							MaxUsages = 0,

						},
						NickName = "fastFire"
					},
				};
				bs.MovementBehaviors = new List<MovementBehaviorBase>() {
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
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:fungannon", enemy.aiActor);
				var nur = enemy.aiActor;
				nur.EffectResistances = new ActorEffectResistance[]
                {
					new ActorEffectResistance()
					{
						resistAmount = 1,
						resistType = EffectResistanceType.Poison
					},
				};

				//SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Coallet/Idle/coallet_idle_006", SpriteBuilder.ammonomiconCollection);
				if (enemy.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
				}
				GenericIntroDoer miniBossIntroDoer = prefab.AddComponent<GenericIntroDoer>();
				prefab.AddComponent<FungannonIntroController>();

				miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
				miniBossIntroDoer.initialDelay = 0.15f;
				miniBossIntroDoer.cameraMoveSpeed = 14;
				miniBossIntroDoer.specifyIntroAiAnimator = null;
				miniBossIntroDoer.BossMusicEvent = "Play_MUS_Boss_Theme_Beholster";
				//miniBossIntroDoer.BossMusicEvent = "Play_MUS_Lich_Double_01";
				miniBossIntroDoer.PreventBossMusic = false;
				miniBossIntroDoer.InvisibleBeforeIntroAnim = false;
				miniBossIntroDoer.preIntroAnim = string.Empty;
				miniBossIntroDoer.preIntroDirectionalAnim = string.Empty;
				miniBossIntroDoer.introAnim = "intro";
				miniBossIntroDoer.introDirectionalAnim = string.Empty;
				miniBossIntroDoer.continueAnimDuringOutro = false;
				miniBossIntroDoer.cameraFocus = null;
				miniBossIntroDoer.roomPositionCameraFocus = Vector2.zero;
				miniBossIntroDoer.restrictPlayerMotionToRoom = false;
				miniBossIntroDoer.fusebombLock = false;
				miniBossIntroDoer.AdditionalHeightOffset = 0;
				PlanetsideModule.Strings.Enemies.Set("#FUNGANNON_NAME", "FUNGANNON");
				PlanetsideModule.Strings.Enemies.Set("#FUNGANNON_NAME_SMALL", "Fungannon");

				PlanetsideModule.Strings.Enemies.Set("SPORANGIO_WAR", "SPORANGIO-WAR");
				PlanetsideModule.Strings.Enemies.Set("#QUOTE", "");
				enemy.aiActor.OverrideDisplayName = "#FUNGANNON_NAME_SMALL";

				miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
				{
					bossNameString = "#FUNGANNON_NAME",
					bossSubtitleString = "SPORANGIO_WAR",
					bossQuoteString = "#QUOTE",
					bossSpritePxOffset = IntVector2.Zero,
					topLeftTextPxOffset = IntVector2.Zero,
					bottomRightTextPxOffset = IntVector2.Zero,
					bgColor = Color.blue
				};
				if (BossCardTexture)
				{
					miniBossIntroDoer.portraitSlideSettings.bossArtSprite = BossCardTexture;
					miniBossIntroDoer.SkipBossCard = false;
					enemy.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
				}
				else
				{
					miniBossIntroDoer.SkipBossCard = true;
					enemy.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
				}

				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Ammocom/ammonimiconasdsadsa", SpriteBuilder.ammonomiconCollection);
				if (enemy.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
				}
				enemy.encounterTrackable = enemy.gameObject.AddComponent<EncounterTrackable>();
				enemy.encounterTrackable.journalData = new JournalEntry();
				enemy.encounterTrackable.EncounterGuid = "psog:fungannon";
				enemy.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				enemy.encounterTrackable.journalData.SuppressKnownState = false;
				enemy.encounterTrackable.journalData.IsEnemy = true;
				enemy.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				enemy.encounterTrackable.ProxyEncounterGuid = "";
				enemy.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Ammocom/ammonimiconasdsadsa";
				enemy.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\ammoentryshrrom.png");
				PlanetsideModule.Strings.Enemies.Set("#FUNGANNONAMMONOMICON", "Fungannon");
				PlanetsideModule.Strings.Enemies.Set("#FUNGANNONAMMONOMICONSHORT", "Sporangio-War");
				PlanetsideModule.Strings.Enemies.Set("#FUNGANNONAMMONOMICONLONG", "The eldest of the Fungal species that roam the Gungeon. With their great cannons on their heads, and an appetite matched by few, many Gungeoneers tend to avoid them, with little surviving a close-quarters match against the collosal beasts.");
				enemy.encounterTrackable.journalData.PrimaryDisplayName = "#FUNGANNONAMMONOMICON";
				enemy.encounterTrackable.journalData.NotificationPanelDescription = "#FUNGANNONAMMONOMICONSHORT";
				enemy.encounterTrackable.journalData.AmmonomiconFullEntry = "#FUNGANNONAMMONOMICONLONG";
				EnemyBuilder.AddEnemyToDatabase(enemy.gameObject, "psog:fungannon");
				EnemyDatabase.GetEntry("psog:fungannon").ForcedPositionInAmmonomicon = 4;
				EnemyDatabase.GetEntry("psog:fungannon").isInBossTab = true;
				EnemyDatabase.GetEntry("psog:fungannon").isNormalEnemy = true;

				miniBossIntroDoer.SkipFinalizeAnimation = true;
				miniBossIntroDoer.RegenerateCache();

				//==================
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(enemy.aiActor.healthHaver);
				//==================

			}
		}


		private static string[] spritePaths = new string[]
		{
			//Idle
			"Planetside/Resources/Fungannon/Fungannon_idle_001.png",
			"Planetside/Resources/Fungannon/Fungannon_idle_002.png",
			"Planetside/Resources/Fungannon/Fungannon_idle_003.png",
			"Planetside/Resources/Fungannon/Fungannon_idle_004.png",
			"Planetside/Resources/Fungannon/Fungannon_idle_005.png",
			"Planetside/Resources/Fungannon/Fungannon_idle_006.png",
			"Planetside/Resources/Fungannon/Fungannon_idle_007.png",
			//MoveLeft
			"Planetside/Resources/Fungannon/Fungannon_move_001.png",
			"Planetside/Resources/Fungannon/Fungannon_move_002.png",
			"Planetside/Resources/Fungannon/Fungannon_move_003.png",
			"Planetside/Resources/Fungannon/Fungannon_move_004.png",
			"Planetside/Resources/Fungannon/Fungannon_move_005.png",
			"Planetside/Resources/Fungannon/Fungannon_move_006.png",
			"Planetside/Resources/Fungannon/Fungannon_move_007.png",
			"Planetside/Resources/Fungannon/Fungannon_move_008.png",
			"Planetside/Resources/Fungannon/Fungannon_move_009.png",
			//MoveRight
			"Planetside/Resources/Fungannon/Fungannon_moveright_001.png",
			"Planetside/Resources/Fungannon/Fungannon_moveright_002.png",
			"Planetside/Resources/Fungannon/Fungannon_moveright_003.png",
			"Planetside/Resources/Fungannon/Fungannon_moveright_004.png",
			"Planetside/Resources/Fungannon/Fungannon_moveright_005.png",
			"Planetside/Resources/Fungannon/Fungannon_moveright_006.png",
			"Planetside/Resources/Fungannon/Fungannon_moveright_007.png",
			"Planetside/Resources/Fungannon/Fungannon_moveright_008.png",
			"Planetside/Resources/Fungannon/Fungannon_moveright_009.png",
			//Roar
			"Planetside/Resources/Fungannon/Fungannon_roar_001.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_002.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_003.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_004.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_005.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_006.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_007.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_008.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_009.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_010.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_011.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_012.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_013.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_014.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_015.png",
			"Planetside/Resources/Fungannon/Fungannon_roar_016.png",
			//Jump
			"Planetside/Resources/Fungannon/Fungannon_jump_001.png",
			"Planetside/Resources/Fungannon/Fungannon_jump_002.png",
			"Planetside/Resources/Fungannon/Fungannon_jump_003.png",
			"Planetside/Resources/Fungannon/Fungannon_jump_004.png",
			"Planetside/Resources/Fungannon/Fungannon_jump_005.png",
			"Planetside/Resources/Fungannon/Fungannon_jump_006.png",
			"Planetside/Resources/Fungannon/Fungannon_jump_007.png",
			"Planetside/Resources/Fungannon/Fungannon_jump_008.png",
			"Planetside/Resources/Fungannon/Fungannon_jump_009.png",
			"Planetside/Resources/Fungannon/Fungannon_jump_010.png",
			"Planetside/Resources/Fungannon/Fungannon_jump_011.png",
			"Planetside/Resources/Fungannon/Fungannon_jump_012.png",
			//JumpLand
			"Planetside/Resources/Fungannon/Fungannon_jumpland_001.png",
			"Planetside/Resources/Fungannon/Fungannon_jumpland_002.png",
			"Planetside/Resources/Fungannon/Fungannon_jumpland_003.png",
			//Intro
			"Planetside/Resources/Fungannon/Fungannon_spawn_001.png",
			"Planetside/Resources/Fungannon/Fungannon_spawn_002.png",
			"Planetside/Resources/Fungannon/Fungannon_spawn_003.png",
			"Planetside/Resources/Fungannon/Fungannon_spawn_004.png",
			"Planetside/Resources/Fungannon/Fungannon_spawn_005.png",
			"Planetside/Resources/Fungannon/Fungannon_spawn_006.png",
			"Planetside/Resources/Fungannon/Fungannon_spawn_007.png",
			"Planetside/Resources/Fungannon/Fungannon_spawn_008.png",
			"Planetside/Resources/Fungannon/Fungannon_spawn_009.png",
			"Planetside/Resources/Fungannon/Fungannon_spawn_010.png",
			//Death
			"Planetside/Resources/Fungannon/Fungannon_death_001.png",
			"Planetside/Resources/Fungannon/Fungannon_death_002.png",
			"Planetside/Resources/Fungannon/Fungannon_death_003.png",
			"Planetside/Resources/Fungannon/Fungannon_death_004.png",
			"Planetside/Resources/Fungannon/Fungannon_death_005.png",
			"Planetside/Resources/Fungannon/Fungannon_death_006.png",
			"Planetside/Resources/Fungannon/Fungannon_death_007.png",
			"Planetside/Resources/Fungannon/Fungannon_death_008.png",
			"Planetside/Resources/Fungannon/Fungannon_death_009.png",
			"Planetside/Resources/Fungannon/Fungannon_death_010.png",
			"Planetside/Resources/Fungannon/Fungannon_death_011.png",
			"Planetside/Resources/Fungannon/Fungannon_death_012.png",
			"Planetside/Resources/Fungannon/Fungannon_death_013.png",

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
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom && GameManager.Instance.PrimaryPlayer.IsInCombat == true)
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

			public void Start()
			{
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("383175a55879441d90933b5c4e60cf6f").bulletBank.GetBullet("bigBullet"));
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
				base.aiActor.HasBeenEngaged = false;
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(base.aiActor.healthHaver);


				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					FungannonController controller = base.aiActor.GetComponent<FungannonController>();
					if (controller != null)
                    {
						for (int i = 0; i < controller.extantReticles.Count; i++)
						{
							SpawnManager.Despawn(controller.extantReticles[i]);
							Destroy(controller.extantReticles[i]);
						}
						controller.extantReticles.Clear();
					}
					
				};
				base.aiActor.healthHaver.OnDeath += (obj) =>
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_FUNGANNON, true);//Done
				};
			}

		}
		public class FastCannons : Script
		{
			protected override IEnumerator Top()
			{
				CellArea area = base.BulletBank.aiActor.ParentRoom.area;

				for (int j = 0; j < 2; j++)
				{
					List<float> log = new List<float>()
					{


					};
					FungannonController vfx = base.BulletBank.GetComponent<FungannonController>();
					vfx.LaserShit = EnemyDatabase.GetOrLoadByGuid("6868795625bd46f3ae3e4377adce288b").GetComponent<ResourcefulRatController>().ReticleQuad;
					vfx.name = "LaserTell"+j.ToString();

					base.PostWwiseEvent("Play_BOSS_RatMech_Barrel_01", null);
					for (int i = 0; i < 3+j; i++)
					{
						float angle = base.AimDirection + (UnityEngine.Random.Range(-60, 60));
						float num2 = 20f;

						Vector2 zero = Vector2.zero;
						if (BraveMathCollege.LineSegmentRectangleIntersection(this.Position, this.Position + BraveMathCollege.DegreesToVector(angle, 60f), area.UnitBottomLeft, area.UnitTopRight, ref zero))
						{
							num2 = (zero - this.Position).magnitude;
						}
						if (vfx == null)
						{
							vfx.LaserShit = EnemyDatabase.GetOrLoadByGuid("6868795625bd46f3ae3e4377adce288b").GetComponent<ResourcefulRatController>().ReticleQuad;
						}
						GameObject gameObject = SpawnManager.SpawnVFX(vfx.LaserShit, false);
						tk2dSlicedSprite component2 = gameObject.GetComponent<tk2dSlicedSprite>();
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, this.Position.y) + BraveMathCollege.DegreesToVector(angle, 2f).ToVector3ZUp(0);
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
						component2.dimensions = new Vector2((num2) * 16f, 5f);
						component2.UpdateZDepth();
						vfx.extantReticles.Add(gameObject);
						log.Add(angle);
						yield return this.Wait(1);
					}
					yield return this.Wait(20f);
					this.CleanupReticles();
					ExplosionData aww = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
					GameManager.Instance.MainCameraController.DoScreenShake(aww.ss, new Vector2?(base.Position), false);
					foreach (float h in log)
					{
						base.PostWwiseEvent("Play_ENM_cannonball_blast_01", null);
						string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
						base.Fire(new Direction(h, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(25f, SpeedType.Absolute), new FastCannons.Cannonball());
						yield return this.Wait(3);

					}
					yield return this.Wait(30);
				}
				yield break;
			}
			public class Cannonball : Bullet
			{
				public Cannonball() : base("bigBullet", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
					for (int i = 0; i < 600; i++)
					{
						string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
						base.Fire(new Direction(UnityEngine.Random.Range(140, 200), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(3f, SpeedType.Absolute), new PrimaryCannonScript.Spore(bankName, UnityEngine.Random.Range(2, 20)));
						yield return this.Wait(2f);

					}
					yield break;
				}
			}
			public class SporeSmall : Bullet
			{
				public SporeSmall(string bulletname, float Airtime) : base(bulletname, false, false, false)
				{
					this.BulletName = bulletname;
					this.AirTime = Airtime;
				}

				protected override IEnumerator Top()
				{
					if (this.BulletName == "spore2")
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 240);
					}
					else
					{
						base.ChangeSpeed(new Speed(1, SpeedType.Absolute), 180);
					}
					yield return this.Wait(AirTime);
					base.Vanish(false);
					yield break;
				}
				public string BulletName;
				public float AirTime;
			}
			public void CleanupReticles()
			{
				FungannonController controller = base.BulletBank.aiActor.GetComponent<FungannonController>();
				if (controller)
				{
					for (int i = 0; i < controller.extantReticles.Count; i++)
					{
						SpawnManager.Despawn(controller.extantReticles[i]);
						Destroy(controller.extantReticles[i]);
					}
					controller.extantReticles.Clear();
				}
			}
		}
		public class BigEverywhereAttack : Script
		{
			protected override IEnumerator Top()
			{
				CellArea area = base.BulletBank.aiActor.ParentRoom.area;
				float delta = 20f;
				float startDirection = AimDirection;
				float radius = 0.04f;
				base.PostWwiseEvent("Play_BOSS_Rat_Cheese_Summon_01", null);
				for (int j = 0; j < 18; j++)
				{
					base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new BigEverywhereAttack.TheGear(15, 25, 240 ,"spore2", this, (startDirection + (float)j * delta) + 180, radius));
					base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new BigEverywhereAttack.TheGear(-15, 75, 360, "spore1", this, (startDirection + (float)j * delta) + 180, radius));
					base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new BigEverywhereAttack.TheGear(15, 50, 480,"spore2", this, (startDirection + (float)j * delta) + 180, radius));
					base.Fire(new Direction(-90f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new BigEverywhereAttack.TheGear(-15, 100, 600,"spore1", this, (startDirection + (float)j * delta) + 180, radius));
				}

				for (int j = 0; j < 8; j++)
				{
					yield return this.Wait(60f);
					List<float> log = new List<float>()
					{


					};
					FungannonController vfx = base.BulletBank.GetComponent<FungannonController>();

					vfx.LaserShit = EnemyDatabase.GetOrLoadByGuid("6868795625bd46f3ae3e4377adce288b").GetComponent<ResourcefulRatController>().ReticleQuad;
					vfx.name = "LaserTell" + j.ToString();
					base.PostWwiseEvent("Play_BOSS_RatMech_Barrel_01", null);
					int Amount = 12+j;
					for (int e = 0; e < Amount; e++)
                    {
						float anim = (360 / Amount) * e;
						float angle = base.AimDirection + anim;
						log.Add(angle);
					}
					List<float> list = log.Shuffle<float>();
					for (int i = 0; i < Amount; i++)
					{
						float num2 = 20f;
						Vector2 zero = Vector2.zero;
						if (BraveMathCollege.LineSegmentRectangleIntersection(this.Position, this.Position + BraveMathCollege.DegreesToVector(list[i], 60f), area.UnitBottomLeft, area.UnitTopRight, ref zero))
						{
							num2 = (zero - this.Position).magnitude;
						}
						if (vfx == null)
						{
							vfx.LaserShit = EnemyDatabase.GetOrLoadByGuid("6868795625bd46f3ae3e4377adce288b").GetComponent<ResourcefulRatController>().ReticleQuad;
						}
						GameObject gameObject = SpawnManager.SpawnVFX(vfx.LaserShit, false);
						tk2dSlicedSprite component2 = gameObject.GetComponent<tk2dSlicedSprite>();
						component2.transform.position = new Vector3(this.Position.x, this.Position.y, this.Position.y) + BraveMathCollege.DegreesToVector(list[i], 0f).ToVector3ZUp(0);
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, list[i]);
						component2.dimensions = new Vector2((num2) * 16f, 5f);
						component2.UpdateZDepth();
						vfx.extantReticles.Add(gameObject);
						yield return this.Wait(1);
					}
					yield return this.Wait(20);
					this.CleanupReticles();
					base.PostWwiseEvent("Play_ENM_cannonball_blast_01", null);
					ExplosionData aww = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
					GameManager.Instance.MainCameraController.DoScreenShake(aww.ss, new Vector2?(base.Position), false);
					foreach (float h in log)
					{
						string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
						base.Fire(new Direction(h, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(35f, SpeedType.Absolute), new BigEverywhereAttack.Cannonball());
						if (h > 0)
                        {
							base.EndOnBlank = true;
						}
					}
					base.EndOnBlank = false;
				}
				yield break;
			}
			public class Cannonball : Bullet
			{
				public Cannonball() : base("bigBullet", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
					for (int i = 0; i < 600; i++)
					{
						string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
						base.Fire(new Direction(UnityEngine.Random.Range(150, 210), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(1f, SpeedType.Absolute), new PrimaryCannonScript.Spore(bankName, UnityEngine.Random.Range(15, 40)));
						yield return this.Wait(1f);

					}
					yield break;
				}
			}
			public class SporeSmall : Bullet
			{
				public SporeSmall(string bulletname, float Airtime) : base(bulletname, false, false, false)
				{
					this.BulletName = bulletname;
					this.AirTime = Airtime;
				}

				protected override IEnumerator Top()
				{
					if (this.BulletName == "spore2")
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 180);
					}
					else
					{
						base.ChangeSpeed(new Speed(1, SpeedType.Absolute), 120);
					}
					yield return this.Wait(AirTime);
					base.Vanish(false);
					yield break;
				}
				public string BulletName;
				public float AirTime;
			}
			public void CleanupReticles()
			{
				FungannonController controller = base.BulletBank.aiActor.GetComponent<FungannonController>();
				if (controller)
                {
					for (int i = 0; i < controller.extantReticles.Count; i++)
					{
						SpawnManager.Despawn(controller.extantReticles[i]);
						Destroy(controller.extantReticles[i]);
					}
					controller.extantReticles.Clear();
				}				
			}

			public class TheGear : Bullet
			{
				public TheGear(float spinspeed, float RevUp,float StartSpeenAgain ,string BulletType, BigEverywhereAttack parent, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
				{
					this.m_spinSpeed = spinspeed;
					this.TimeToRevUp = RevUp;
					this.StartAgain = StartSpeenAgain;

					this.m_parent = parent;
					this.m_angle = angle;
					this.m_radius = aradius;
					this.m_bulletype = BulletType;
					this.SuppressVfx = true;
				}

				protected override IEnumerator Top()
				{
					base.ManualControl = true;
					this.Projectile.collidesOnlyWithPlayerProjectiles = true;
					this.Projectile.collidesWithProjectiles = true;
					this.Projectile.UpdateCollisionMask();
					Vector2 centerPosition = base.Position;
					float radius = 0f;
					for (int i = 0; i < 2400; i++)
					{
						if (i < TimeToRevUp)
						{
							radius += m_radius;
						}
						if (StartAgain < i)
                        {
							radius += m_radius*2;
						}
						if (i == StartAgain)
						{
							this.Projectile.spriteAnimator.Play();		
						}
						centerPosition += this.Velocity / 60f;
						base.UpdateVelocity();
						this.m_angle += this.m_spinSpeed / 60f;
						base.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}

				private IEnumerator ChangeSpinSpeedTask(float newSpinSpeed, int term)
				{
					float delta = (newSpinSpeed - this.m_spinSpeed) / (float)term;
					for (int i = 0; i < term; i++)
					{
						this.m_spinSpeed += delta;
						yield return base.Wait(1);
					}
					yield break;
				}
				private const float ExpandSpeed = 4.5f;
				private const float SpinSpeed = 40f;
				private BigEverywhereAttack m_parent;
				private float m_angle;
				private float m_spinSpeed;
				private float m_radius;
				private string m_bulletype;
				private float TimeToRevUp;
				private float StartAgain;


			}
			public class Superball : Bullet
			{
				public Superball() : base("big", false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
					{
						base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
						base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
					}
					for (int i = 0; i < 90; i++)
					{
						float Speed = base.Speed;
						base.ChangeSpeed(new Speed(Speed * 0.98f, SpeedType.Absolute), 0);

						float aim = this.GetAimDirection(5f, UnityEngine.Random.Range(6f, 6f));
						float delta = BraveMathCollege.ClampAngle180(aim - this.Direction);
						if (Mathf.Abs(delta) > 100f)
						{
							yield break;
						}
						this.Direction += Mathf.MoveTowards(0f, delta, 3f);
						yield return this.Wait(1);
					}

					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 1);
					base.Vanish(true);
					yield break;
				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (!preventSpawningProjectiles)
					{
						base.PostWwiseEvent("Play_BOSS_Rat_Cheese_Burst_02", null);
						for (int i = 0; i < 10; i++)
						{
							string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
							float speed = 1f;
							if (bankName == "spore2")
							{
								speed *= UnityEngine.Random.Range(1.5f, 2f);
							}
							else
							{
								speed *= UnityEngine.Random.Range(0.6f, 1.4f);

							}
							base.Fire(new Direction(UnityEngine.Random.Range(-180, 180), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(5f * speed, SpeedType.Absolute), new PrimaryCannonScript.Spore(bankName, 180));
						}
						return;
					}
				}
			}

			public class Spore : Bullet
			{
				public Spore(string bulletname) : base(bulletname, false, false, false)
				{
					this.BulletName = bulletname;
				}

				protected override IEnumerator Top()
				{
					if (this.BulletName == "spore2")
					{
						base.ChangeSpeed(new Speed(22, SpeedType.Absolute), 60);
					}
					else
					{
						base.ChangeSpeed(new Speed(18, SpeedType.Absolute), 60);
					}
					yield break;
				}
				public string BulletName;
			}
		}

		public class RainingPoot : Script
		{

			protected override IEnumerator Top()
			{

				CellArea area = base.BulletBank.aiActor.ParentRoom.area;
				AIActor aiActor = base.BulletBank.aiActor;
				yield return this.Wait(40f);
				Vector2 roomLowerLeft = area.UnitBottomLeft;
				Vector2 roomUpperRight = area.UnitTopRight - new Vector2(0f, 3.125f);
				Vector2 roomCenter = area.UnitCenter - new Vector2(0f, 2.25f);
				for (int i = 0; i < 3; i++)
				{
					int fired;
					fired = UnityEngine.Random.Range(2, 4);
					for (int j = 0; j < fired; j++)
					{
						Vector2 vector = new Vector2(roomLowerLeft.x, base.SubdivideRange(roomLowerLeft.y, roomUpperRight.y, fired+1, j, true));
						vector += new Vector2(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-1, 1));
						vector.x -= 2.5f;
						this.FireWallBullet(0f, vector, roomCenter);
					}
					fired = UnityEngine.Random.Range(11, 18);
					for (int k = 0; k < fired; k++)
					{
						Vector2 vector2 = new Vector2(base.SubdivideRange(roomLowerLeft.x, roomUpperRight.x, fired+1, k, true), roomUpperRight.y);
						vector2 += new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-0.25f, 0.25f));
						vector2.y += 6.5f;
						this.FireWallBullet(-90f, vector2, roomCenter);
					}
					fired = UnityEngine.Random.Range(11, 18);
					for (int l = 0; l < fired; l++)
					{
						Vector2 vector3 = new Vector2(roomUpperRight.x, base.SubdivideRange(roomLowerLeft.y, roomUpperRight.y, fired + 1, l, true));
						vector3 += new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-0.25f, 0.25f));
						vector3.x += 2.5f;
						this.FireWallBullet(180f, vector3, roomCenter);
					}
					fired = UnityEngine.Random.Range(11, 18);
					for (int m = 0; m < fired; m++)
					{
						Vector2 vector4 = new Vector2(base.SubdivideRange(roomLowerLeft.x, roomUpperRight.x, fired + 1, m, true), roomLowerLeft.y);
						vector4 += new Vector2(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-1f, 1f));
						vector4.y -= 2.5f;
						this.FireWallBullet(90f, vector4, roomCenter);
					}
					yield return base.Wait(25);
				}
				yield return base.Wait(240);


				yield break;
			}
			private void FireWallBullet(float facingDir, Vector2 spawnPos, Vector2 roomCenter)
			{
				float angleDeg = (spawnPos - roomCenter).ToAngle();
				int num = Mathf.RoundToInt(BraveMathCollege.ClampAngle360(angleDeg) / 45f) % 8;
				float num2 = (float)num * 45f;
				base.Fire(Offset.OverridePosition(spawnPos), new Direction(facingDir, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new RainingPoot.PAin(this, (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1"));
			}

			public class PAin : Bullet
			{
				public PAin(RainingPoot parent, string bulletName) : base(bulletName, true, false, false)
				{
					this.m_parent = parent;
				}
				protected override IEnumerator Top()
				{
					int travelTime = UnityEngine.Random.Range(150, 450);
					this.Projectile.IgnoreTileCollisionsFor(90f);
					this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
					this.Projectile.sprite.ForceRotationRebuild();
					this.Projectile.sprite.UpdateZDepth();
					int r = UnityEngine.Random.Range(0, 20);

					Vector2 area = base.BulletBank.aiActor.sprite.WorldCenter;
					this.Direction = (area - base.Position).ToAngle();

					base.ChangeSpeed(new Speed(11f, SpeedType.Absolute), UnityEngine.Random.Range(150, 600));
					yield return base.Wait(travelTime);
					base.Vanish(false);
					yield break;
				}
				private RainingPoot m_parent;
			}
		}

		public class PooterCannon : Script
		{
			protected override IEnumerator Top()
			{
				for (int e = 0; e < 5; e++)
                {
					ExplosionData aww = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
					GameManager.Instance.MainCameraController.DoScreenShake(aww.ss, new Vector2?(base.Position), false);
					base.PostWwiseEvent("Play_ENM_cannonball_blast_01", null);
					float Num;
					int RNG = UnityEngine.Random.Range(0, 4);
					bool WeakShot = RNG == 0;
					if (WeakShot)
					{
						Num = 0;
						//base.Fire(new Offset("CannonNorth"), new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new PooterCannon.Cannonball());
						base.Fire(new Offset("CannonNorth"), new Direction(UnityEngine.Random.Range(Num - 45, Num + 45), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(14f, SpeedType.Absolute), new PooterCannon.Cannonball());
						for (int i = 0; i < 8; i++)
						{
							string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
							float speed = 1f;
							if (bankName == "spore2")
							{
								speed *= UnityEngine.Random.Range(1.5f, 2f);
							}
							else
							{
								speed *= UnityEngine.Random.Range(0.6f, 1.4f);

							}
							base.Fire(new Offset("CentreOfAllCannons"), new Direction(UnityEngine.Random.Range(Num - 45, Num + 45)+180, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(4.5f * speed, SpeedType.Absolute), new PrimaryCannonScript.Spore(bankName, 180));
						}
					}
					bool NormalShot = RNG == 1;
					if (NormalShot)
					{
						Num = 90;
						//base.Fire(new Offset("CannonSouth"), new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new PooterCannon.Cannonball());
						base.Fire(new Offset("CannonSouth"), new Direction(UnityEngine.Random.Range(Num - 45, Num + 45), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(14f, SpeedType.Absolute), new PooterCannon.Cannonball());
						for (int i = 0; i < 8; i++)
						{
							string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
							float speed = 1f;
							if (bankName == "spore2")
							{
								speed *= UnityEngine.Random.Range(1.5f, 2f);
							}
							else
							{
								speed *= UnityEngine.Random.Range(0.6f, 1.4f);

							}
							base.Fire(new Offset("CentreOfAllCannons"), new Direction(UnityEngine.Random.Range(Num - 45, Num + 45) + 180, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(4.5f * speed, SpeedType.Absolute), new PrimaryCannonScript.Spore(bankName, 180));
						}
					}
					bool yeet = RNG == 2;
					if (yeet)
					{
						Num = 180;
						//base.Fire(new Offset("CannonSouth"), new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new PooterCannon.Cannonball());
						base.Fire(new Offset("CannonSouth"), new Direction(UnityEngine.Random.Range(Num - 45, Num + 45), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(14f, SpeedType.Absolute), new PooterCannon.Cannonball());
						for (int i = 0; i < 8; i++)
						{
							string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
							float speed = 1f;
							if (bankName == "spore2")
							{
								speed *= UnityEngine.Random.Range(1.5f, 2f);
							}
							else
							{
								speed *= UnityEngine.Random.Range(0.6f, 1.4f);

							}
							base.Fire(new Offset("CentreOfAllCannons"), new Direction(UnityEngine.Random.Range(Num - 45, Num + 45) + 180, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(4.5f * speed, SpeedType.Absolute), new PrimaryCannonScript.Spore(bankName, 180));
						}
					}
					bool rr = RNG == 3;
					if (rr)
					{
						Num = 270;
						//base.Fire(new Offset("CannonWest"), new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new PooterCannon.Cannonball());
						base.Fire(new Offset("CannonWest"), new Direction(UnityEngine.Random.Range(Num - 45, Num + 45), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(14f, SpeedType.Absolute), new PooterCannon.Cannonball());
						for (int i = 0; i < 8; i++)
						{
							string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
							float speed = 1f;
							if (bankName == "spore2")
							{
								speed *= UnityEngine.Random.Range(1.5f, 2f);
							}
							else
							{
								speed *= UnityEngine.Random.Range(0.6f, 1.4f);
							}
							base.Fire(new Offset("CentreOfAllCannons"), new Direction(UnityEngine.Random.Range(Num - 45, Num + 45) + 180, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(4.5f * speed, SpeedType.Absolute), new PrimaryCannonScript.Spore(bankName, 180));
						}
					}
					yield return this.Wait(60f);
				}
				yield break;
			}


			public class Cannonball : Bullet
			{
				public Cannonball() : base("bigBullet", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
					for (int i = 0; i < 600; i++)
					{
						string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
						base.Fire(new Direction(UnityEngine.Random.Range(150, 210), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(4, SpeedType.Absolute), new PrimaryCannonScript.Spore(bankName, UnityEngine.Random.Range(30, 120)));
						yield return this.Wait(2f);

					}
					yield break;
				}
			}
			public class Spore : Bullet
			{
				public Spore(string bulletname, float Airtime) : base(bulletname, false, false, false)
				{
					this.BulletName = bulletname;
					this.AirTime = Airtime;
				}

				protected override IEnumerator Top()
				{
					if (this.BulletName == "spore2")
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 180);
					}
					else
					{
						base.ChangeSpeed(new Speed(1, SpeedType.Absolute), 120);
					}
					yield return this.Wait(AirTime);
					base.Vanish(false);
					yield break;
				}
				public string BulletName;
				public float AirTime;
			}
		}

		public class PrimaryCannonScript : Script
		{
			protected override IEnumerator Top()
			{
				base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);
				ExplosionData aww = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
				GameManager.Instance.MainCameraController.DoScreenShake(aww.ss, new Vector2?(base.Position), false);
				base.Fire(new Offset("CannonNorth"), new Direction(45, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(7f, SpeedType.Absolute), new PrimaryCannonScript.Cannonball());
				base.Fire(new Offset("CannonSouth"), new Direction(225, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(7f, SpeedType.Absolute), new PrimaryCannonScript.Cannonball());
				base.Fire(new Offset("CannonEast"), new Direction(135, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(7f, SpeedType.Absolute), new PrimaryCannonScript.Cannonball());
				base.Fire(new Offset("CannonWest"), new Direction(315, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(7f, SpeedType.Absolute), new PrimaryCannonScript.Cannonball());
				/*
				for (int i = 0; i < 28; i++)
                {
					string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
					float speed = 1f;
					if (bankName == "spore2")
                    {
						speed *= UnityEngine.Random.Range(1.2f, 1.666f);
						base.Fire(new Offset("CentreOfAllCannons"), new Direction(UnityEngine.Random.Range(180, -180), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(8f * speed, SpeedType.Absolute), new PrimaryCannonScript.Spore(bankName, 360));
					}
					if (bankName == "spore1")
					{
						speed *= UnityEngine.Random.Range(0.75f, 1.1f);
						base.Fire(new Offset("CentreOfAllCannons"), new Direction(UnityEngine.Random.Range(180, -180), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(5f * speed, SpeedType.Absolute), new PrimaryCannonScript.Spore(bankName, 450));
					}
				}
				*/
				yield break;
			}


			public class Cannonball : Bullet
			{
				public Cannonball() : base("bigBullet", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
					base.ChangeSpeed(new Speed(18f, SpeedType.Absolute), 60);
					for (int i = 0; i < 600; i++)
					{
						string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
						base.Fire(new Direction(UnityEngine.Random.Range(150 ,210), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(1.5f, SpeedType.Absolute), new PrimaryCannonScript.Spore(bankName, UnityEngine.Random.Range(150, 600)));
						yield return this.Wait(2f);

					}
					yield break;
				}
			}
			public class Spore : Bullet
			{
				public Spore(string bulletname, float Airtime) : base(bulletname, false, false, false)
				{
					this.BulletName = bulletname;
					this.AirTime = Airtime;
				}

				protected override IEnumerator Top()
				{
					if (this.BulletName == "spore2")
                    {
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 120);
					}
					else
                    {
						base.ChangeSpeed(new Speed(1, SpeedType.Absolute), 180);
					}
					yield return this.Wait(AirTime);
					base.Vanish(false);
					yield break;
				}
				public string BulletName;
				public float AirTime;
			}
		}


		public class MegaCannon : Script
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
				float ANim = base.AimDirection;
				base.PostWwiseEvent("Play_ENM_hammer_target_01", null);
				yield return this.Wait(20f);
				for (int i = 0; i < 30; i++)
				{
					string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
					base.Fire(new Direction(UnityEngine.Random.Range(-180, 180), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(UnityEngine.Random.Range(4, 8), SpeedType.Absolute), new MegaCannon.Spore(bankName, UnityEngine.Random.Range(30, 120)));

				}
				base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);
				base.Fire(new Direction(ANim, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(25, SpeedType.Absolute), new MegaCannon.Superball());


				yield break;
			}

			public class Superball : Bullet
			{
				public Superball() : base("big", false, false, false)
				{
				}
				protected override IEnumerator Top()
				{
					for (int i = 0; i < 100; i++)
					{
						base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new MegaCannon.Cannonball());
						yield return this.Wait(4f);

					}
					yield break;
				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (!preventSpawningProjectiles)
					{
						for (int i = 0; i < 60; i++)
						{
							string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
							base.Fire(new Direction(UnityEngine.Random.Range(-180, 180), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(UnityEngine.Random.Range(7, 11), SpeedType.Absolute), new MegaCannon.Spore(bankName, UnityEngine.Random.Range(30, 120)));

						}
						return;
					}
				}
			}
			public class Cannonball : Bullet
			{
				public Cannonball() : base("bigBullet", false, false, false)
				{

				}

				protected override IEnumerator Top()
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
					yield return this.Wait(180f);
					for (int i = 0; i < 7; i++)
					{
						string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
						base.Fire(new Direction(UnityEngine.Random.Range(-180, 180), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(UnityEngine.Random.Range(3, 6), SpeedType.Absolute), new MegaCannon.Spore(bankName, UnityEngine.Random.Range(90, 450)));

					}
					base.Vanish(false);
					yield break;
				}
			}
			public class Spore : Bullet
			{
				public Spore(string bulletname, float Airtime) : base(bulletname, false, false, false)
				{
					this.BulletName = bulletname;
					this.AirTime = Airtime;
				}

				protected override IEnumerator Top()
				{
					if (this.BulletName == "spore2")
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 60);
					}
					else
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 120);
					}
					yield return this.Wait(AirTime);
					base.Vanish(false);
					yield break;
				}
				public string BulletName;
				public float AirTime;
			}
		}

	}
}

