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
using GungeonAPI;
using System.Reflection;


namespace Planetside
{
	public class CelBullet : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "cel_bullet";
		private static tk2dSpriteCollectionData CelBulletCollection;
		public static GameObject shootpoint;


		public static void Init()
		{

			CelBullet.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				AIActor aIActor = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
				prefab = EnemyBuilder.BuildPrefab("cel_bullet", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(0, 0), false);
				var companion = prefab.AddComponent<EnemyBehavior>(); ;
				companion.aiActor.knockbackDoer.weight = 800;
				companion.aiActor.MovementSpeed = 4f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(20f);
				companion.aiActor.CollisionKnockbackStrength = 5f;
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
					ManualHeight = 22,
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
					ManualHeight = 22,
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

				bool flag3 = CelBulletCollection == null;
				if (flag3)
				{
					CelBulletCollection = SpriteBuilder.ConstructCollection(prefab, "CelBullet_Collection");
					UnityEngine.Object.DontDestroyOnLoad(CelBulletCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], CelBulletCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CelBulletCollection, new List<int>
					{

					0,
					1,
					2,
					3

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CelBulletCollection, new List<int>
					{

					0,
					1,
					2,
					3


					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CelBulletCollection, new List<int>
					{

					0,
					1,
					2,
					3
					
					
					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CelBulletCollection, new List<int>
					{

					0,
					1,
					2,
					3


					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CelBulletCollection, new List<int>
					{

				 4,
				 5,
				 6,
				 7,
				 8,
				 9



					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CelBulletCollection, new List<int>
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
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;

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
					PauseTime = 0.25f
				}
			};
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootGunBehavior() {
					GroupCooldownVariance = 0.2f,
					LineOfSight = false,
					WeaponType = WeaponType.BulletScript,
					OverrideBulletName = null,
					BulletScript = new CustomBulletScriptSelector(typeof(SkellScript)),
					FixTargetDuringAttack = true,
					StopDuringAttack = true,
					LeadAmount = 0,
					LeadChance = 1,
					RespectReload = true,
					MagazineCapacity = 3,
					ReloadSpeed = 5f,
					EmptiesClip = true,
					SuppressReloadAnim = false,
					TimeBetweenShots = -1,
					PreventTargetSwitching = true,
					OverrideAnimation = null,
					OverrideDirectionalAnimation = null,
					HideGun = false,
					UseLaserSight = false,
					UseGreenLaser = false,
					PreFireLaserTime = -1,
					AimAtFacingDirectionWhenSafe = false,
					Cooldown = 0.2f,
					CooldownVariance = 0,
					AttackCooldown = 0,
					GlobalCooldown = 0,
					InitialCooldown = 0,
					InitialCooldownVariance = 0,
					GroupName = null,
					GroupCooldown = 0,
					MinRange = 0,
					Range = 16,
					MinWallDistance = 0,
					MaxEnemiesInRoom = 0,
					MinHealthThreshold = 0,
					MaxHealthThreshold = 1,
					HealthThresholds = new float[0],
					AccumulateHealthThresholds = true,
					targetAreaStyle = null,
					IsBlackPhantom = false,
					resetCooldownOnDamage = null,
					RequiresLineOfSight = true,
					MaxUsages = 0,

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
				//BehaviorSpeculator load = EnemyDatabase.GetOrLoadByGuid("206405acad4d4c33aac6717d184dc8d4").behaviorSpeculator;
				//Tools.DebugInformation(load);
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = false;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;

				//shootpoint.transform.parent = companion.transform;
				//shootpoint.transform.position = new Vector2(1.5f, 0.25f);
				GameObject m_CachedGunAttachPoint = companion.transform.Find("GunAttachPoint").gameObject;
				var yah = companion.transform.Find("GunAttachPoint").gameObject;
				yah.transform.position = companion.aiActor.transform.position;
				yah.transform.localPosition = new Vector2(0f, 0f);
				EnemyBuilder.DuplicateAIShooterAndAIBulletBank(prefab, aIActor.aiShooter, aIActor.GetComponent<AIBulletBank>(), 38, yah.transform);



				Game.Enemies.Add("psog:sophia_bullet", companion.aiActor);
				PlanetsideModule.Strings.Enemies.Set("#CEL", "Sophia");
				companion.aiActor.OverrideDisplayName = "#CEL";
				companion.aiActor.ActorName = "#CEL";
				companion.aiActor.name = "#CEL";

			}
		}

		public class SkellScript : Script
		{
			protected override IEnumerator Top() 
			{
				AkSoundEngine.PostEvent("Play_WPN_magnum_shot_01", this.BulletBank.aiActor.gameObject);
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				}
				for (int i = -1; i <= 1; i++)
				{
					this.Fire(new Direction((float)(i * 3), DirectionType.Aim, -1f), new Speed(7f, SpeedType.Absolute), new SkellBullet());
				}
				yield break;
			}
		}
		

		public class SkellBullet : Bullet
		{
			public SkellBullet() : base("default", false, false, false)
			{

			}
			protected override IEnumerator Top()
			{
				base.ChangeSpeed(new Speed(20f, SpeedType.Absolute), 60);
				yield break;
			}
		}


		private static string[] spritePaths = new string[]
		{
			
			//idles
			"Planetside/Resources/Enemies/ModderBullets/cel/celbullet_idle_001.png",
			"Planetside/Resources/Enemies/ModderBullets/cel/celbullet_idle_002.png",
			"Planetside/Resources/Enemies/ModderBullets/cel/celbullet_idle_003.png",
			"Planetside/Resources/Enemies/ModderBullets/cel/celbullet_idle_004.png",
			//death
			"Planetside/Resources/Enemies/ModderBullets/cel/celbullet_die_001.png",
			"Planetside/Resources/Enemies/ModderBullets/cel/celbullet_die_002.png",
			"Planetside/Resources/Enemies/ModderBullets/cel/celbullet_die_003.png",
			"Planetside/Resources/Enemies/ModderBullets/cel/celbullet_die_004.png",
			"Planetside/Resources/Enemies/ModderBullets/cel/celbullet_die_005.png",
			"Planetside/Resources/Enemies/ModderBullets/cel/celbullet_die_006.png",
			};

		public class EnemyBehavior : BraveBehaviour
		{

			private RoomHandler m_StartRoom;
			private void Update()
			{
				if (!base.aiActor.HasBeenEngaged) { CheckPlayerRoom(); }

				/*
				base.aiActor.aiShooter.CurrentGun.GetSprite().AttachRenderer(base.aiShooter.gameObject.GetComponent<AIShooter>().handObject.sprite);

				for (int i = 0; i < base.aiActor.healthHaver.bodySprites.Count; i++)
                {
					tk2dBaseSprite tk2dBaseSprite = base.healthHaver.bodySprites[i];
					tk2dBaseSprite.usesOverrideMaterial = true;
					tk2dBaseSprite.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/RainbowChestShader");
					tk2dBaseSprite.sprite.renderer.material.SetFloat("_ValueMaximum", 100);
				}

				base.aiActor.healthHaver.bodySprites.Add(base.aiActor.gameObject.GetComponent<AIShooter>().handObject.sprite);

				PlayerHandController hand = base.aiActor.gameObject.GetComponent<AIShooter>().handObject;
				if (hand)
				{
					tk2dSprite component2 = hand.GetComponent<tk2dSprite>();
					base.aiActor.healthHaver.RegisterBodySprite(component2, false, 0);
					Type type = typeof(AIShooter); FieldInfo _property = type.GetField("m_attachedHands", BindingFlags.NonPublic | BindingFlags.Instance); _property.GetValue(base.aiShooter);
					List<PlayerHandController> HELP = (List<PlayerHandController>)_property.GetValue(base.aiShooter);
					HELP.Add(base.aiActor.gameObject.GetComponent<AIShooter>().handObject);
				}
				base.aiActor.aiShooter.UpdateHandRenderers();
				*/
			}

			private void CheckPlayerRoom()
			{

				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
				{
					GameManager.Instance.StartCoroutine(LateEngage());
				}

			}

			private IEnumerator LateEngage()
			{
				yield return new WaitForSeconds(0.5f);
				base.aiActor.HasBeenEngaged = true;
				yield break;
			}
			private void Start()
			{
				if (base.aiActor != null && !base.aiActor.IsBlackPhantom)
				{
					base.aiActor.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
					base.aiActor.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
					base.aiActor.sprite.renderer.material.SetFloat("_EmissivePower", 40);
					base.aiActor.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.2f);
				}
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				//base.aiActor.HasBeenEngaged = true;
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					AkSoundEngine.PostEvent("Play_ENM_highpriest_blast_01", base.aiActor.gameObject);
				};
			}


		}


	}
}








