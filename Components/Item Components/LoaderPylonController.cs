using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;
using UnityEngine.Serialization;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using Brave.BulletScript;
using GungeonAPI;
using SpriteBuilder = ItemAPI.SpriteBuilder;
using DirectionType = DirectionalAnimation.DirectionType;
using static DirectionalAnimation;

namespace Planetside
{
	public class LoaderPylonController : MonoBehaviour
	{
		public static GameObject Turretprefab;
		public static readonly string guid = "loader_pylon";

		public static void Init()
        {
			Turretprefab = EnemyBuilder.BuildPrefab("Loader Pylon", guid, "Planetside/Resources/LoaderPylon/Idle/laoderpylon_idle_001.png", new IntVector2(0, 0), new IntVector2(0, 0), false, true);
			var enemy = Turretprefab.AddComponent<EnemyBehavior>();
            {
				enemy.aiActor.knockbackDoer.weight = 800;
				enemy.aiActor.healthHaver.PreventAllDamage = true;
				enemy.aiActor.HasShadow = false;
				enemy.aiActor.IgnoreForRoomClear = true;
				enemy.aiActor.aiAnimator.HitReactChance = 0f;
				enemy.aiActor.specRigidbody.CollideWithOthers = false;
				enemy.aiActor.specRigidbody.CollideWithTileMap = true;
				enemy.aiActor.PreventFallingInPitsEver = false;
				enemy.aiActor.healthHaver.ForceSetCurrentHealth(100f);
				enemy.aiActor.CollisionKnockbackStrength = 0f;
				enemy.aiActor.CanTargetPlayers = true;
				enemy.aiActor.healthHaver.SetHealthMaximum(100f, null, false);
				enemy.aiActor.IsHarmlessEnemy = true;
				enemy.aiActor.sprite.renderer.enabled = false;
				Turretprefab.AddAnimation("turret_idle", "Planetside/Resources/LoaderPylon/Idle", fps: 5, AnimationType.Idle, DirectionType.Single);


				var bs = Turretprefab.GetComponent<BehaviorSpeculator>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("998807b57e454f00a63d67883fcf90d6").behaviorSpeculator;

				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:loader_pylon", enemy.aiActor);

			}

			GameObject Pylon = ItemBuilder.AddSpriteToObject("loaderpylon_thing", "Planetside/Resources/LoaderPylon/Idle/laoderpylon_idle_001.png", null);
			FakePrefab.MarkAsFakePrefab(Pylon);
			UnityEngine.Object.DontDestroyOnLoad(Pylon);
			tk2dSpriteAnimator animator = Pylon.AddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 8;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "start";

			GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/LoaderPylon/Idle/laoderpylon_idle_001", spriteObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 5; i++)
			{
				GameObject spriteForObject = new GameObject("spriteForObject");
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/LoaderPylon/Idle/laoderpylon_idle_00{i}", spriteForObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
				frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
				animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
			animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
			animator.DefaultClipId = animator.GetClipIdByName("start");
			animator.playAutomatically = true;
			PylonPrefab = Pylon;
		}


		private static GameObject PylonPrefab;

		public void Awake()
		{
			this.actor = base.GetComponent<AIActor>();
			this.actor.PreventFallingInPitsEver = true;

			this.player = base.GetComponent<PlayerController>();
		}

		public void Start()
		{
			AkSoundEngine.PostEvent("Play_OBJ_turret_set_01", actor.gameObject);
			if (this.actor == null)
            {
				this.actor = base.GetComponent<AIActor>();
			}
			if (this.player == null)
			{
				this.player = base.GetComponent<PlayerController>();
			}
			LootEngine.DoDefaultItemPoof(actor.sprite.WorldCenter, false, false);

			this.actor.CanTargetPlayers = false;
			this.actor.ParentRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
			this.actor.HasBeenEngaged = true;
			RoomHandler parentRoom = this.actor.ParentRoom;
			parentRoom.OnEnemiesCleared = (Action)Delegate.Combine(parentRoom.OnEnemiesCleared, new Action(this.HandleRoomCleared));
			this.actor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider));
			this.actor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox));
			this.actor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.Projectile));
			this.actor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.BeamBlocker));
			this.LinkVFXPrefab = FakePrefab.Clone(Game.Items["shock_rounds"].GetComponent<ComplexProjectileModifier>().ChainLightningVFX);

			
			GameObject pylon = this.actor.PlayEffectOnActor(PylonPrefab, new Vector3(0f, -1f, 0f));
			pylon.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(this.actor.sprite.WorldBottomCenter, tk2dBaseSprite.Anchor.LowerCenter);
			pylon.transform.position.WithZ(transform.position.z);

			tk2dSprite ahfuck = pylon.GetComponent<tk2dSprite>();
			SpriteOutlineManager.AddOutlineToSprite(ahfuck.sprite, Color.black);
			pylon.GetComponent<tk2dSpriteAnimator>().Play("start");
			
			actor.StartCoroutine(this.HandleTimedDestroy());
			PlayerController player = GameManager.Instance.PrimaryPlayer;

		}

		public void Update()
		{
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			if (player && this.extantLink == null)
			{
				tk2dTiledSprite component = SpawnManager.SpawnVFX(this.LinkVFXPrefab, false).GetComponent<tk2dTiledSprite>();
				this.extantLink = component;
			}
			else if (player && this.extantLink != null)
			{
				UpdateLink(player, this.extantLink);
			}
			else if (extantLink != null || actor == null)
			{
				SpawnManager.Despawn(extantLink.gameObject);
				extantLink = null;
			}
		}
		private void UpdateLink(PlayerController target, tk2dTiledSprite m_extantLink)
		{
			SpeculativeRigidbody specRigidbody = this.actor.specRigidbody;
			SpeculativeRigidbody speculativeRigidbody = specRigidbody;
			Vector2 unitCenter = speculativeRigidbody.UnitTopCenter;
			Vector2 unitCenter2 = target.specRigidbody.HitboxPixelCollider.UnitCenter;
			m_extantLink.transform.position = unitCenter;
			Vector2 vector = unitCenter2 - unitCenter;
			float num = BraveMathCollege.Atan2Degrees(vector.normalized);
			int num2 = Mathf.RoundToInt(vector.magnitude / 0.0625f);
			m_extantLink.dimensions = new Vector2((float)num2, m_extantLink.dimensions.y);
			m_extantLink.transform.rotation = Quaternion.Euler(0f, 0f, num);
			m_extantLink.UpdateZDepth();
			this.ApplyLinearDamage(unitCenter, unitCenter2);

		}
		private void ApplyLinearDamage(Vector2 p1, Vector2 p2)
		{
			float num = 12;
			for (int i = 0; i < StaticReferenceManager.AllEnemies.Count; i++)
			{
				AIActor aiactor = StaticReferenceManager.AllEnemies[i];
				if (!this.m_damagedEnemies.Contains(aiactor))
				{
					if (aiactor && aiactor.HasBeenEngaged && aiactor.IsNormalEnemy && aiactor.specRigidbody && !aiactor.IsHarmlessEnemy)
					{
						Vector2 zero = Vector2.zero;
						if (BraveUtility.LineIntersectsAABB(p1, p2, aiactor.specRigidbody.HitboxPixelCollider.UnitBottomLeft, aiactor.specRigidbody.HitboxPixelCollider.UnitDimensions, out zero))
						{
							aiactor.healthHaver.ApplyDamage(num, Vector2.zero, "Chain Lightning", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
							GameManager.Instance.StartCoroutine(this.HandleDamageCooldown(aiactor));
						}
					}
				}
			}
		}
		private IEnumerator HandleDamageCooldown(AIActor damagedTarget)
		{
			this.m_damagedEnemies.Add(damagedTarget);
			yield return new WaitForSeconds(0.33f);
			this.m_damagedEnemies.Remove(damagedTarget);
			yield break;
		}
		private HashSet<AIActor> m_damagedEnemies = new HashSet<AIActor>();
		private GameObject LinkVFXPrefab;
		private tk2dTiledSprite extantLink;


		public void NotifyDropped()
		{
			this.HandleRoomCleared();
		}

		private IEnumerator HandleTimedDestroy()
		{
			yield return new WaitForSeconds(this.maxDuration);
			AkSoundEngine.PostEvent("Play_OBJ_turret_fade_01", actor.gameObject);
			LootEngine.DoDefaultItemPoof(actor.sprite.WorldCenter, false, true);
			SpawnManager.Despawn(extantLink.gameObject);
			UnityEngine.Object.Destroy(base.gameObject);
			yield break;
		}

		private void HandleRoomCleared()
		{
			if (this.actor)
			{
				SpawnManager.Despawn(extantLink.gameObject);
				AkSoundEngine.PostEvent("Play_OBJ_turret_fade_01", actor.gameObject);
				LootEngine.DoDefaultItemPoof(actor.sprite.WorldCenter, false, true);
				UnityEngine.Object.Destroy(base.gameObject);
				SpawnManager.Despawn(extantLink.gameObject);

			}
		}


		[NonSerialized]
		public PlayerController sourcePlayer;

		public float maxDuration = 10f;

		private AIActor actor;
		private PlayerController player;

		

	}

}
