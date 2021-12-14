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

namespace Planetside
{
	public class SkilotarBullet : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "skilotar_bullet";
		private static tk2dSpriteCollectionData SkiBullete;
		public static GameObject shootpoint;
		public static void Init()
		{
			SkilotarBullet.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				//float AttackAnimationThingAMaWhatIts = 0.5f;
				prefab = EnemyBuilder.BuildPrefab("Skilotar Bullet", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 800;
				companion.aiActor.MovementSpeed = 3f;
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
					ManualWidth = 20,
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
					ManualWidth = 20,
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
				bool flag3 = SkiBullete == null;
				if (flag3)
				{
					SkiBullete = SpriteBuilder.ConstructCollection(prefab, "SkilotarBullet_Collection");
					UnityEngine.Object.DontDestroyOnLoad(SkiBullete);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], SkiBullete);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SkiBullete, new List<int>
					{

					0,
					1,
					2,
					3

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SkiBullete, new List<int>
					{

					0,
					1,
					2,
					3


					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SkiBullete, new List<int>
					{

					0,
					1,
					2,
					3


					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SkiBullete, new List<int>
					{

					0,
					1,
					2,
					3


					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SkiBullete, new List<int>
					{

				 4,
				 5,
				 6,
				 7,
				 8,
				 9
				 



					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SkiBullete, new List<int>
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
				//BehaviorSpeculator load = EnemyDatabase.GetOrLoadByGuid("6e972cd3b11e4b429b888b488e308551").behaviorSpeculator;
				//Tools.DebugInformation(load);
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:skilotar_bullet", companion.aiActor);
				PlanetsideModule.Strings.Enemies.Set("#SKILOTAR", "Skilotar");
				companion.aiActor.OverrideDisplayName = "#SKILOTAR";
				companion.aiActor.ActorName = "#SKILOTAR";
				companion.aiActor.name = "#SKILOTAR";

			}
		}



		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/ModderBullets/skilotar/skilotarbullet_idle_001.png",
			"Planetside/Resources/Enemies/ModderBullets/skilotar/skilotarbullet_idle_002.png",
			"Planetside/Resources/Enemies/ModderBullets/skilotar/skilotarbullet_idle_003.png",
			"Planetside/Resources/Enemies/ModderBullets/skilotar/skilotarbullet_idle_004.png",
			//death
			"Planetside/Resources/Enemies/ModderBullets/skilotar/skilotarbullet_die_001.png",
			"Planetside/Resources/Enemies/ModderBullets/skilotar/skilotarbullet_die_002.png",
			"Planetside/Resources/Enemies/ModderBullets/skilotar/skilotarbullet_die_003.png",
			"Planetside/Resources/Enemies/ModderBullets/skilotar/skilotarbullet_die_004.png",
			"Planetside/Resources/Enemies/ModderBullets/skilotar/skilotarbullet_die_005.png",
			"Planetside/Resources/Enemies/ModderBullets/skilotar/skilotarbullet_die_006.png",


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
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				if (base.aiActor != null && !base.aiActor.IsBlackPhantom)
				{
					base.aiActor.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
					base.aiActor.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
					base.aiActor.sprite.renderer.material.SetFloat("_EmissivePower", 40);
					base.aiActor.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.2f);
				}
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					AkSoundEngine.PostEvent("Play_ENM_highpriest_blast_01", base.aiActor.gameObject);
				};
			}


		}

		public class SalamanderScript : Script 
		{
			protected override IEnumerator Top()
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1b5810fafbec445d89921a4efb4e42b7").bulletBank.GetBullet("bouncingRing"));
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1b5810fafbec445d89921a4efb4e42b7").bulletBank.GetBullet("bouncingMouth"));

				}
				for (int i = 0; i < 1; i++)
				{
					float aim = base.GetAimDirection((float)((UnityEngine.Random.value >= 0.4f) ? 0 : 1), 8f) + UnityEngine.Random.Range(-10f, 10f);
					for (int j = 0; j < 18; j++)
					{
						float angle = (float)j * 20f;
						Vector2 desiredOffset = BraveMathCollege.DegreesToVector(angle, 1.8f);
						base.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new SalamanderScript.BouncingRingBullet("bouncingRing", desiredOffset));
					}
					base.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new SalamanderScript.BouncingRingBullet("bouncingRing", new Vector2(-0.7f, 0.7f)));
					base.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new SalamanderScript.BouncingRingBullet("bouncingMouth", new Vector2(0f, 0.4f)));
					base.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new SalamanderScript.BouncingRingBullet("bouncingRing", new Vector2(0.7f, 0.7f)));
					yield return base.Wait(40);
				}
				yield break;
			}

			private const int NumBlobs = 8;

			private const int NumBullets = 18;

			public class BouncingRingBullet : Bullet
			{
				public BouncingRingBullet(string name, Vector2 desiredOffset) : base(name, false, false, false)
				{
					this.m_desiredOffset = desiredOffset;
				}

				protected override IEnumerator Top()
				{
					Vector2 centerPoint = base.Position;
					Vector2 lowestOffset = BraveMathCollege.DegreesToVector(-90f, 1.5f);
					Vector2 currentOffset = Vector2.zero;
					float squishFactor = 1f;
					float verticalOffset = 0f;
					int unsquishIndex = 100;
					base.ManualControl = true;
					for (int i = 0; i < 300; i++)
					{
						if (i < 30)
						{
							currentOffset = Vector2.Lerp(Vector2.zero, this.m_desiredOffset, (float)i / 30f);
						}
						verticalOffset = (Mathf.Abs(Mathf.Cos((float)i / 90f * 3.14159274f)) - 1f) * 2.5f;
						if (unsquishIndex <= 10)
						{
							squishFactor = Mathf.Abs(Mathf.SmoothStep(0.6f, 1f, (float)unsquishIndex / 10f));
							unsquishIndex++;
						}
						Vector2 relativeOffset = currentOffset - lowestOffset;
						Vector2 squishedOffset = lowestOffset + relativeOffset.Scale(1f, squishFactor);
						base.UpdateVelocity();
						centerPoint += this.Velocity / 60f;
						base.Position = centerPoint + squishedOffset + new Vector2(0f, verticalOffset);
						if (i % 90 == 45)
						{
							for (int j = 1; j <= 10; j++)
							{
								squishFactor = Mathf.Abs(Mathf.SmoothStep(1f, 0.5f, (float)j / 10f));
								relativeOffset = currentOffset - lowestOffset;
								squishedOffset = lowestOffset + relativeOffset.Scale(1f, squishFactor);
								centerPoint += 0.333f * this.Velocity / 60f;
								base.Position = centerPoint + squishedOffset + new Vector2(0f, verticalOffset);
								yield return base.Wait(1);
							}
							unsquishIndex = 1;
						}
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}

				private Vector2 m_desiredOffset;
			}
		}


		public class WallBullet : Bullet
		{
			public WallBullet() : base("bigBullet", false, false, false)
			{
			}
			protected override IEnumerator Top()
			{
				yield return this.Wait(60);
				base.ChangeSpeed(new Speed(16f, SpeedType.Absolute), 60);
				yield break;
			}
		}
	}
}








