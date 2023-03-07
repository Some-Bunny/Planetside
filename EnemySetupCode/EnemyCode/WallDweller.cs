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

/*
namespace Planetside
{
	public class WallDeweller : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "wall_dweller";
		private static tk2dSpriteCollectionData WallDweller;
		public static GameObject shootpoint;
		public static void Init()
		{
			WallDeweller.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				//float AttackAnimationThingAMaWhatIts = 0.5f;
				prefab = EnemyBuilder.BuildPrefab("Wall Dweller", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 800;
				companion.aiActor.MovementSpeed = 0f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = true;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = false;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(40f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = false;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.healthHaver.SetHealthMaximum(40f, null, false);
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
					ManualWidth = 16,
					ManualHeight = 16,
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
					ManualOffsetY = -4,
					ManualWidth = 16,
					ManualHeight = 16,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;
				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "idle",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				DirectionalAnimation anim = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					AnimNames = new string[]
					{
						"attack",

					},
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "attack",
						anim = anim
					}
				};
				DirectionalAnimation yourhonor = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					AnimNames = new string[]
					{
						"death",

					},
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "death",
						anim = yourhonor
					}
				};
				DirectionalAnimation aaa = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					AnimNames = new string[]
	                {
						"tell",

	                },
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "tell",
						anim = aaa
					}
				};
				bool flag3 = WallDweller == null;
				if (flag3)
				{
					WallDweller = SpriteBuilder.ConstructCollection(prefab, "WallLad_Collection");
					UnityEngine.Object.DontDestroyOnLoad(WallDweller);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], WallDweller);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, WallDweller, new List<int>
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

					}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 0.9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, WallDweller, new List<int>
					{

					9,
					10,
				    11,
					12,
					13,



					}, "attack", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, WallDweller, new List<int>
					{
		            14,
	             	15,
		            16,
		            17,
		            18,
					19,
		            
					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, WallDweller, new List<int>
					{
						20,
						21,
						22,
						23,
						24

					}, "tell", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;

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
					InitialCooldown = 4f,
					TellAnimation = "tell",
					FireAnimation = "attack",
					RequiresLineOfSight = true,
					StopDuring = ShootBehavior.StopType.Attack,
					Uninterruptible = true,
					
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
				Game.Enemies.Add("psog:wall_dweller", companion.aiActor);


			}
		}



		private static string[] spritePaths = new string[]
		{
			
			//idles
			"Planetside/WallDweller/cavedweller_idle_001.png",
			"Planetside/WallDweller/cavedweller_idle_002.png",
			"Planetside/WallDweller/cavedweller_idle_003.png",
			"Planetside/WallDweller/cavedweller_idle_004.png",
			"Planetside/WallDweller/cavedweller_idle_005.png",
			"Planetside/WallDweller/cavedweller_idle_006.png",
			"Planetside/WallDweller/cavedweller_idle_007.png",
			"Planetside/WallDweller/cavedweller_idle_008.png",
			"Planetside/WallDweller/cavedweller_idle_009.png",

			//
			//tell

			//attack
			"Planetside/WallDweller/cavedweller_attack_001.png",
			"Planetside/WallDweller/cavedweller_attack_002.png",
			"Planetside/WallDweller/cavedweller_attack_003.png",
			"Planetside/WallDweller/cavedweller_attack_004.png",
			"Planetside/WallDweller/cavedweller_attack_005.png",

			//death
			"Planetside/WallDweller/cavedweller_death_001.png",
			"Planetside/WallDweller/cavedweller_death_002.png",
			"Planetside/WallDweller/cavedweller_death_003.png",
			"Planetside/WallDweller/cavedweller_death_004.png",
			"Planetside/WallDweller/cavedweller_death_005.png",
			"Planetside/WallDweller/cavedweller_death_006.png",

			"Planetside/WallDweller/cavedweller_tell_001.png",
			"Planetside/WallDweller/cavedweller_tell_002.png",
			"Planetside/WallDweller/cavedweller_tell_003.png",
			"Planetside/WallDweller/cavedweller_tell_004.png",
			"Planetside/WallDweller/cavedweller_tell_005.png",


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
				//base.aiActor.HasBeenEngaged = true;
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Bite_01", base.aiActor.gameObject);
				};
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");
			}


		}

		public class SalamanderScript : Script // This BulletScript is just a modified version of the script BulletManShroomed, which you can find with dnSpy.
		{
			public override IEnumerator Top() // This is just a simple example, but bullet scripts can do so much more.
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				}
				base.PostWwiseEvent("Play_ENM_Tarnisher_Spit_01", null);
				float angleDelta = 90f;
				for (int i = 0; i < 8; i++)
				{
					base.Fire(new Offset(-0.25f, 0f, ((float)i + 0.5f)*angleDelta, string.Empty, DirectionType.Absolute), new Direction((float)i * angleDelta, DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new WallBullet());
					yield return base.Wait(4);
				}

				yield break;
			}
		}


		public class WallBullet : Bullet
		{
			// Token: 0x06000A91 RID: 2705 RVA: 0x00030B38 File Offset: 0x0002ED38
			public WallBullet() : base("default", false, false, false)
			{
			}

		}
	}
}



*/




