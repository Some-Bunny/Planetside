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

namespace Planetside
{
	public class DeTurretLeft : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "deturretleft_enemy";
		private static tk2dSpriteCollectionData DeturretColection;
		public static GameObject shootpoint;
		public static void Init()
		{
			DeTurretLeft.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Deturretleft Enemy", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 10000;
				companion.aiActor.MovementSpeed = 0f;
				companion.aiActor.healthHaver.PreventAllDamage = true;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = true;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = false;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(100f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.IsWorthShootingAt = false;

				companion.aiActor.healthHaver.SetHealthMaximum(100f, null, false);
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
				companion.aiActor.gameActor.PreventAutoAimVelocity = true;
				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").CorpseObject;
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
				bool flag3 = DeturretColection == null;
				if (flag3)
				{
					DeturretColection = SpriteBuilder.ConstructCollection(prefab, "DeturretRight_Collection");
					UnityEngine.Object.DontDestroyOnLoad(DeturretColection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], DeturretColection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, DeturretColection, new List<int>
					{

					9,
					8,
					7,
					6,
					5,
					4,
					3,
					2,
					1,
					0

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, DeturretColection, new List<int>
					{
					9,
					8,
					7,
					6,
					5,
					4,
					3,
					2,
					1,
					0

					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, DeturretColection, new List<int>
					{
					9,
					8,
					7,
					6,
					5,
					4,
					3,
					2,
					1,
					0


					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, DeturretColection, new List<int>
					{
					9,
					8,
					7,
					6,
					5,
					4,
					3,
					2,
					1,
					0


					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, DeturretColection, new List<int>
					{

					10




					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, DeturretColection, new List<int>
					{
					 10

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;

				}
				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
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
				bs.OtherBehaviors = new List<BehaviorBase>() {
				new CustomSpinBulletsBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					OverrideBulletName = "default",
					NumBullets = 5,
					BulletMinRadius = 1,
					BulletMaxRadius = 4,
					BulletCircleSpeed = -60,
					BulletsIgnoreTiles = false,
					RegenTimer = 0.02f,
					AmountOFLines = 4,
				}
				};
				
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:deturret_left", companion.aiActor);


			}
		}



		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/Deturret/deturret_idle_001.png",
			"Planetside/Resources/Enemies/Deturret/deturret_idle_002.png",
			"Planetside/Resources/Enemies/Deturret/deturret_idle_003.png",
			"Planetside/Resources/Enemies/Deturret/deturret_idle_004.png",
			"Planetside/Resources/Enemies/Deturret/deturret_idle_005.png",
			"Planetside/Resources/Enemies/Deturret/deturret_idle_006.png",
			"Planetside/Resources/Enemies/Deturret/deturret_idle_007.png",
			"Planetside/Resources/Enemies/Deturret/deturret_idle_008.png",
			"Planetside/Resources/Enemies/Deturret/deturret_idle_009.png",
			"Planetside/Resources/Enemies/Deturret/deturret_idle_010.png",

			//death
			"Planetside/Resources/Enemies/Deturret/deturret_idle_001.png",
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
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");
				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet("default"));

				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{

				};
			}

		}
	}

}





