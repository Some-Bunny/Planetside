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
	public class SpcreatBullet : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "spcreat_bullet";
		private static tk2dSpriteCollectionData SpecreatBullete;
		public static GameObject shootpoint;
		public static void Init()
		{
			SpcreatBullet.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				//float AttackAnimationThingAMaWhatIts = 0.5f;
				prefab = EnemyBuilder.BuildPrefab("Spcreat Bullet", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
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
					ManualWidth = 18,
					ManualHeight = 18,
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
					ManualHeight = 18,
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
				bool flag3 = SpecreatBullete == null;
				if (flag3)
				{
					SpecreatBullete = SpriteBuilder.ConstructCollection(prefab, "SpcreatBullet_Collection");
					UnityEngine.Object.DontDestroyOnLoad(SpecreatBullete);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], SpecreatBullete);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SpecreatBullete, new List<int>
					{

					0,
					1,
					2,
					3

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SpecreatBullete, new List<int>
					{

					0,
					1,
					2,
					3


					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SpecreatBullete, new List<int>
					{

					0,
					1,
					2,
					3


					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SpecreatBullete, new List<int>
					{

					0,
					1,
					2,
					3


					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SpecreatBullete, new List<int>
					{

				 4,
				 5,
				 6,
				 7,
				 8,
				 9




					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SpecreatBullete, new List<int>
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
				Game.Enemies.Add("psog:spcreat_bullet", companion.aiActor);

				PlanetsideModule.Strings.Enemies.Set("#HARDMODEMAN", "Spcreat");
				companion.aiActor.OverrideDisplayName = "#HARDMODEMAN";
				companion.aiActor.ActorName = "#HARDMODEMAN";
				companion.aiActor.name = "#HARDMODEMAN";
			}
		}



		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/ModderBullets/spcreat/spcreatbullet_idle_001.png",
			"Planetside/Resources/Enemies/ModderBullets/spcreat/spcreatbullet_idle_002.png",
			"Planetside/Resources/Enemies/ModderBullets/spcreat/spcreatbullet_idle_003.png",
			"Planetside/Resources/Enemies/ModderBullets/spcreat/spcreatbullet_idle_004.png",

			//death
			"Planetside/Resources/Enemies/ModderBullets/spcreat/spcreatbullet_death_001.png",
			"Planetside/Resources/Enemies/ModderBullets/spcreat/spcreatbullet_death_002.png",
			"Planetside/Resources/Enemies/ModderBullets/spcreat/spcreatbullet_death_003.png",
			"Planetside/Resources/Enemies/ModderBullets/spcreat/spcreatbullet_death_004.png",
			"Planetside/Resources/Enemies/ModderBullets/spcreat/spcreatbullet_death_005.png",
			"Planetside/Resources/Enemies/ModderBullets/spcreat/spcreatbullet_death_006.png",
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
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				Vector2 zero = Vector2.zero;
				Vector2 p = zero + new Vector2(-1.5f, -1.5f);
				Vector2 vector = new Vector2(2f, -5f);
				Vector2 p2 = vector + new Vector2(-1.5f, 0.4f);
				Vector2 vector2 = new Vector2(-0.5f, -1.5f);
				Vector2 p3 = vector2 + new Vector2(0.75f, 0.75f);
				Vector2 vector3 = new Vector2(-0.5f, 1.5f);
				Vector2 p4 = vector3 + new Vector2(0.75f, -0.75f);
				float num = BraveMathCollege.ClampAngle180((this.BulletManager.PlayerPosition() - base.BulletBank.specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle());
				bool flag = num > -45f && num < 120f;
				Vector2 phantomBulletPoint = base.Position + BraveMathCollege.CalculateBezierPoint(0.5f, zero, p, p2, vector);
				for (int i = 0; i < 6; i++)
				{
					float num2 = (float)i / 11f;
					Vector2 offset = BraveMathCollege.CalculateBezierPoint(num2, zero, p, p2, vector);
					if (flag)
					{
						num2 = 1f - num2;
					}
					Vector2 offset2 = BraveMathCollege.CalculateBezierPoint(num2, vector2, p3, p4, vector3);
					base.Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new SalamanderScript.ReaperBullet(phantomBulletPoint, offset2));
				}
				return null;
			}

			public class ReaperBullet : Bullet
			{
				public ReaperBullet(Vector2 phantomBulletPoint, Vector2 offset) : base("default", false, false, false)
				{
					this.m_phantomBulletPoint = phantomBulletPoint;
					this.m_offset = offset;
				}

				protected override IEnumerator Top()
				{
					base.ManualControl = true;
					yield return base.Wait(5);
					for (int i = 0; i < 180; i++)
					{
						this.Projectile.ResetDistance();
						this.Direction = Mathf.MoveTowardsAngle(this.Direction, base.GetAimDirection(this.m_phantomBulletPoint, 0f, this.Speed), 2f);
						base.UpdateVelocity();
						this.m_phantomBulletPoint += this.Velocity / 60f;
						base.Position += this.Velocity / 60f;
						float rotation = this.Velocity.ToAngle();
						Vector2 goalPos = this.m_phantomBulletPoint + this.m_offset.Rotate(rotation);
						if (i < 30)
						{
							Vector2 a = goalPos - base.Position;
							base.Position += a / (float)(30 - i);
						}
						else
						{
							base.Position = goalPos;
						}
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}
				private Vector2 m_phantomBulletPoint;
				private Vector2 m_offset;
			}
		}
	}
}








