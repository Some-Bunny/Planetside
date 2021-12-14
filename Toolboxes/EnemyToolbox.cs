using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gungeon;
using ItemAPI;
using UnityEngine;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using Brave.BulletScript;

namespace Planetside
{
    public static class EnemyToolbox
    {
        public static DirectionalAnimation AddNewDirectionAnimation(AIAnimator animator, string Prefix, string[] animationNames, DirectionalAnimation.FlipType[] flipType, DirectionalAnimation.DirectionType directionType = DirectionalAnimation.DirectionType.Single)
        {
			DirectionalAnimation newDirectionalAnimation = new DirectionalAnimation
			{
				Type = directionType,
				Prefix = Prefix,
				AnimNames = animationNames,
				Flipped = flipType
			};
			animator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
			{
				new AIAnimator.NamedDirectionalAnimation
				{
					name = Prefix,
					anim = newDirectionalAnimation
				}
			};
			return newDirectionalAnimation;
        }
		public static void AddSoundsToAnimationFrame(tk2dSpriteAnimator animator, string animationName, Dictionary<int, string> frameAndSoundName)//int frame, string soundName)
        {
			foreach (var value in frameAndSoundName)
            {
				animator.GetClipByName(animationName).frames[value.Key].eventAudio = value.Value;
				animator.GetClipByName(animationName).frames[value.Key].triggerEvent = true;
			}
		}
		public static AIActor CreateNewBulletBankerEnemy(string guid, string DisplayName,int sizeX, int sizeY, string firstIdleFrame, string[] spritePaths ,List<int> IdleFrameKeys, List<int> DeathFrameKeys, List<int> AttackFrameKeys,Script bulletScript = null, float MovementSpeed = 2.5f, float HP = 20, float IdleFPS = 5f,float MovementFPS = 10f, float DeathFPS = 8f)
        {
		   tk2dSpriteCollectionData collectionData = new tk2dSpriteCollectionData();

			GameObject prefab = EnemyBuilder.BuildPrefab(guid, guid, firstIdleFrame, new IntVector2(0, 0), new IntVector2(8, 9), false);
			var companion = prefab.AddComponent<BulletEnemyBehavior>();
			companion.aiActor.knockbackDoer.weight = 800;
			companion.aiActor.MovementSpeed = MovementSpeed;
			companion.aiActor.healthHaver.PreventAllDamage = false;
			companion.aiActor.CollisionDamage = 1f;
			companion.aiActor.HasShadow = false;
			companion.aiActor.IgnoreForRoomClear = true;
			companion.aiActor.aiAnimator.HitReactChance = 0f;
			companion.aiActor.specRigidbody.CollideWithOthers = true;
			companion.aiActor.specRigidbody.CollideWithTileMap = true;
			companion.aiActor.PreventFallingInPitsEver = true;
			companion.aiActor.healthHaver.ForceSetCurrentHealth(HP);
			companion.aiActor.CollisionKnockbackStrength = 0f;
			companion.aiActor.procedurallyOutlined = false;
			companion.aiActor.CanTargetPlayers = true;
			companion.aiActor.healthHaver.SetHealthMaximum(HP, null, false);
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
				ManualWidth = sizeX,
				ManualHeight = sizeY,
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
				ManualWidth = sizeX,
				ManualHeight = sizeY,
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

			if (AttackFrameKeys != null)
            {
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "attack",
						anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{
							"attack_left",
							"attack_right"

							}
						}
					}
				};
			}

			



			collectionData = SpriteBuilder.ConstructCollection(prefab, guid+"_Collection");
			UnityEngine.Object.DontDestroyOnLoad(collectionData);
			
			
			for (int i = 0; i < spritePaths.Length; i++)
			{
				SpriteBuilder.AddSpriteToCollection(spritePaths[i], collectionData);
			}
			SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, IdleFrameKeys, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = IdleFPS;
			SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, IdleFrameKeys, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = IdleFPS;
			SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, IdleFrameKeys, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = MovementFPS;
			SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, IdleFrameKeys, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = MovementFPS;
			SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, DeathFrameKeys, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = DeathFPS;
			SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, DeathFrameKeys, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = DeathFPS;
			if (AttackFrameKeys != null)
            {
				SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, AttackFrameKeys, "attack_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = DeathFPS;
				SpriteBuilder.AddAnimation(companion.spriteAnimator, collectionData, AttackFrameKeys, "attack_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = DeathFPS;
			}
			var bs = prefab.GetComponent<BehaviorSpeculator>();
			prefab.GetComponent<ObjectVisibilityManager>();
			BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
			bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
			bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;


			GameObject shootpoint = new GameObject("fuck");
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

			if (bulletScript != null)
            {
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(bulletScript.GetType()),
					LeadAmount = 0f,
					AttackCooldown = 5f,
					InitialCooldown = 1f,
					RequiresLineOfSight = true,
					StopDuring = ShootBehavior.StopType.Attack,
					Uninterruptible = true,
					ChargeAnimation = AttackFrameKeys != null ? "attack" : null
				}
			};
			}
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
			Game.Enemies.Add("psog:"+guid, companion.aiActor);

			PlanetsideModule.Strings.Enemies.Set("#DisplayName"+ DisplayName, DisplayName);
			companion.aiActor.OverrideDisplayName = "#DisplayName" + DisplayName;
			companion.aiActor.ActorName = "#DisplayName" + DisplayName;
			companion.aiActor.name = "#DisplayName" + DisplayName;
			return companion.aiActor;
		}

		private class BulletEnemyBehavior : BraveBehaviour
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
				if (base.aiActor != null && !base.aiActor.IsBlackPhantom)
				{
					base.aiActor.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
					base.aiActor.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
					base.aiActor.sprite.renderer.material.SetFloat("_EmissivePower", 40);
					base.aiActor.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.2f);
				}
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>{AkSoundEngine.PostEvent("Play_ENM_highpriest_blast_01", base.aiActor.gameObject);};
			}
		}
	}
}
