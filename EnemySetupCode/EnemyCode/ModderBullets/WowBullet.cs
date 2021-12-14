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

namespace Planetside
{
	public class WowBullet : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "wow_bullet";
		private static tk2dSpriteCollectionData WowBullete;
		public static GameObject shootpoint;
		public static DamageTypeModifier fireaa;


		public static void Init()
		{
			WowBullet.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Wow Bullet", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 800;
				companion.aiActor.MovementSpeed = 5f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = true;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(20f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = false;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.healthHaver.SetHealthMaximum(20f, null, false);
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
					ManualWidth = 18,
					ManualHeight = 19,
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
					ManualWidth = 18,
					ManualHeight = 19,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "die",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{

						   "die_left",
						   "die_right"

							}

						}
					}
				};
				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
					{
						"idle_left",
						"idle_right"
					}
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
						{
						"run_left",
						"run_right"
						}
				};
				bool flag3 = WowBullete == null;
				if (flag3)
				{
					WowBullete = SpriteBuilder.ConstructCollection(prefab, "HunterBullet_Collection");
					UnityEngine.Object.DontDestroyOnLoad(WowBullete);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], WowBullete);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, WowBullete, new List<int>
					{

					0,
					1,
					2,
					3

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, WowBullete, new List<int>
					{

					0,
					1,
					2,
					3


					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, WowBullete, new List<int>
					{

					0,
					1,
					2,
					3


					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, WowBullete, new List<int>
					{

					0,
					1,
					2,
					3


					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, WowBullete, new List<int>
					{

				 4,
				 5,
				 6,
				 7,
				 8,
				 9
				 



					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, WowBullete, new List<int>
					{


				 4,
				 5,
				 6,
				 7,
				 8,
				 9
				 

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;

				}
				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
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
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(SalamanderScript)),
					LeadAmount = 0f,
					AttackCooldown = 5f,
					InitialCooldown = 1f,
					RequiresLineOfSight = true,
					StopDuring = ShootBehavior.StopType.Attack,
					Uninterruptible = true,

				}
				};
				bs.MovementBehaviors = new List<MovementBehaviorBase>
			{
				new SeekTargetBehavior
				{
					StopWhenInRange = true,
					CustomRange = 7f,
					LineOfSight = false,
					ReturnToSpawn = false,
					SpawnTetherDistance = 0f,
					PathInterval = 0.5f,
					SpecifyRange = false,
					MinActiveRange = 0f,
					MaxActiveRange = 0f
				}
			};

				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				var nur = companion.aiActor;
				nur.EffectResistances = new ActorEffectResistance[]
{
					new ActorEffectResistance()
					{
						resistAmount = 1,
						resistType = EffectResistanceType.Fire
					},
};


				Game.Enemies.Add("psog:wow_bullet", companion.aiActor);


				PlanetsideModule.Strings.Enemies.Set("#SIRBESTFRIEND", "SirWow");
				companion.aiActor.OverrideDisplayName = "#SIRBESTFRIEND";
				companion.aiActor.ActorName = "#SIRBESTFRIEND";
				companion.aiActor.name = "#SIRBESTFRIEND";
			}

		}



		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/ModderBullets/wow/wowbullet_idle_001.png",
			"Planetside/Resources/Enemies/ModderBullets/wow/wowbullet_idle_002.png",
			"Planetside/Resources/Enemies/ModderBullets/wow/wowbullet_idle_003.png",
			"Planetside/Resources/Enemies/ModderBullets/wow/wowbullet_idle_004.png",

			//death
			"Planetside/Resources/Enemies/ModderBullets/wow/wowbullet_die_001.png",
			"Planetside/Resources/Enemies/ModderBullets/wow/wowbullet_die_002.png",
			"Planetside/Resources/Enemies/ModderBullets/wow/wowbullet_die_003.png",
			"Planetside/Resources/Enemies/ModderBullets/wow/wowbullet_die_004.png",
			"Planetside/Resources/Enemies/ModderBullets/wow/wowbullet_die_005.png",
			"Planetside/Resources/Enemies/ModderBullets/wow/wowbullet_die_006.png",



		};

		public class EnemyBehavior : BraveBehaviour
		{

			private RoomHandler m_StartRoom;
			private void Update()
			{
				if (!base.aiActor.HasBeenEngaged) { CheckPlayerRoom(); }
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

				fireaa = new DamageTypeModifier();

				fireaa.damageMultiplier = 0f;
				fireaa.damageType = CoreDamageTypes.Fire;
				base.aiActor.healthHaver.damageTypeModifiers.Add(fireaa);
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				if (base.aiActor != null && !base.aiActor.IsBlackPhantom)
				{
					/*
					Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
					mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
					mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
					mat.SetFloat("_EmissiveColorPower", 1.55f);
					mat.SetFloat("_EmissivePower", 30);
					mat.SetFloat("_EmissiveThresholdSensitivity", 0.6f);
					*/
					//base.aiActor.sprite.renderer.material = mat;
					base.aiActor.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
					base.aiActor.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
					base.aiActor.sprite.renderer.material.SetFloat("_EmissivePower", 40);
					base.aiActor.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.2f);
					base.aiActor.sprite.renderer.material.SetColor("_EmissiveColor", Color.white);

				}
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					AkSoundEngine.PostEvent("Play_ENM_highpriest_blast_01", base.aiActor.gameObject);
					DoFireGoop((base.aiActor.sprite.WorldCenter));
				};
			}
			private void DoFireGoop(Vector2 v)
			{
				AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
				GoopDefinition goopDef = assetBundle.LoadAsset<GoopDefinition>("assets/data/goops/napalmgoopquickignite.asset");
				DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDef);
				goopManagerForGoopType.TimedAddGoopCircle(v, 5f, 0.35f, false);
				goopDef.damagesEnemies = false;
			}

		}

		public class SalamanderScript : Script 
		{
			protected override IEnumerator Top() 
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				}
				for (int i = 0; i < 4; i++)
				{
					base.Fire(new Direction(0 + (UnityEngine.Random.Range(-20,20)), DirectionType.Aim, -1f), new Speed(12f, SpeedType.Absolute), new WallBullet());
					yield return Wait(10);

				}

				yield break;
			}
		}


		public class WallBullet : Bullet
		{
			public WallBullet() : base("default", false, false, false)
			{

			}
			protected override IEnumerator Top()
			{
				yield return Wait(UnityEngine.Random.Range(30, 90));
				DoFireGoop((base.Projectile.sprite.WorldCenter));
				base.Vanish(false);
				yield break;
			}
			private void DoFireGoop(Vector2 v)
			{
				AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
				GoopDefinition goopDef = assetBundle.LoadAsset<GoopDefinition>("assets/data/goops/napalmgoopquickignite.asset");
				DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDef);
				goopManagerForGoopType.TimedAddGoopCircle(v, 2.5f, 0.35f, false);
				goopDef.damagesEnemies = false;
			}
		}
	}
}








