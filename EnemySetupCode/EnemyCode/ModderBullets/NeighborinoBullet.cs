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
	public class NeighborinoBullet : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "neighborino_bullet";
		private static tk2dSpriteCollectionData NeighborinoBulleet;
		public static GameObject shootpoint;
		public static void Init()
		{
			NeighborinoBullet.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				//float AttackAnimationThingAMaWhatIts = 0.5f;
				prefab = EnemyBuilder.BuildPrefab("Neighborino Bullet", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 800;
				companion.aiActor.MovementSpeed = 2f;
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
					ManualWidth = 20,
					ManualHeight = 20,
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
				bool flag3 = NeighborinoBulleet == null;
				if (flag3)
				{
					NeighborinoBulleet = SpriteBuilder.ConstructCollection(prefab, "An3sBullet_Collection");
					UnityEngine.Object.DontDestroyOnLoad(NeighborinoBulleet);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], NeighborinoBulleet);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, NeighborinoBulleet, new List<int>
					{

					0,
					1,
					2,
					3

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, NeighborinoBulleet, new List<int>
					{

					0,
					1,
					2,
					3


					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, NeighborinoBulleet, new List<int>
					{

					0,
					1,
					2,
					3


					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, NeighborinoBulleet, new List<int>
					{

					0,
					1,
					2,
					3


					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, NeighborinoBulleet, new List<int>
					{

				 4,
				 5,
				 6,
				 7,
				 8
				 
					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, NeighborinoBulleet, new List<int>
					{


				 4,
				 5,
				 6,
				 7,
				 8
				 
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
					BulletScript = new CustomBulletScriptSelector(typeof(SkellScript)),
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
				Game.Enemies.Add("psog:neighborino_bullet", companion.aiActor);

				PlanetsideModule.Strings.Enemies.Set("#YABOYWHOMADEENEMIES", "Neighborino");
				companion.aiActor.OverrideDisplayName = "#YABOYWHOMADEENEMIES";
				companion.aiActor.ActorName = "#YABOYWHOMADEENEMIES";
				companion.aiActor.name = "#YABOYWHOMADEENEMIES";
			}
		}



		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/ModderBullets/neighborino/neighbullet_idle_001.png",
			"Planetside/Resources/Enemies/ModderBullets/neighborino/neighbullet_idle_002.png",
			"Planetside/Resources/Enemies/ModderBullets/neighborino/neighbullet_idle_003.png",
			"Planetside/Resources/Enemies/ModderBullets/neighborino/neighbullet_idle_004.png",



			//death
			"Planetside/Resources/Enemies/ModderBullets/neighborino/neighbullet_die_001.png",
			"Planetside/Resources/Enemies/ModderBullets/neighborino/neighbullet_die_002.png",
			"Planetside/Resources/Enemies/ModderBullets/neighborino/neighbullet_die_003.png",
			"Planetside/Resources/Enemies/ModderBullets/neighborino/neighbullet_die_004.png",
			"Planetside/Resources/Enemies/ModderBullets/neighborino/neighbullet_die_005.png",

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
		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("icicle"));
				for (int i = -7; i <= 3; i++)
				{
					base.Fire(new Direction((float)(i * 2), DirectionType.Aim, -1f), new Speed(13f - (float)Mathf.Abs(i) * 0.3f, SpeedType.Absolute), new Frost());

				}
				yield return Wait(30);
				for (int i = -3; i <= 7; i++)
				{
					base.Fire(new Direction((float)(i * 2), DirectionType.Aim, -1f), new Speed(13f - (float)Mathf.Abs(i) * 0.3f, SpeedType.Absolute), new Gunfire());

				}
				yield break;
			}
		}
		public class Frost : Bullet
		{
			public Frost() : base("icicle", false, false, false)
			{

			}
		}

		public class Gunfire : Bullet
		{
			public Gunfire() : base("frogger", false, false, false)
			{

			}
		}
	}
}








