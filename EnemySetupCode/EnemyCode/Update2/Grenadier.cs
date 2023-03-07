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

namespace Planetside
{
	class Grenadier : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "grenadier_bullet";
		public static void Init()
		{
			Grenadier.BuildPrefab();
		}
		public static void BuildPrefab()
		{
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);

			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Grenadier Kin", guid, "Planetside/Resources/Enemies/Grenadier/Idle/FrontLeftIdle/bullet_idle_left_001", new IntVector2(0, 0), new IntVector2(0, 0), true, true);
				var enemy = prefab.AddComponent<EnemyBehavior>();

				enemy.aiActor.knockbackDoer.weight = 35;
				enemy.aiActor.MovementSpeed = 3f;
				enemy.aiActor.healthHaver.PreventAllDamage = false;
				enemy.aiActor.CollisionDamage = 1f;
				enemy.aiActor.HasShadow = false;
				enemy.aiActor.IgnoreForRoomClear = false;
				enemy.aiActor.aiAnimator.HitReactChance = 0f;
				enemy.aiActor.specRigidbody.CollideWithOthers = true;
				enemy.aiActor.specRigidbody.CollideWithTileMap = true;
				enemy.aiActor.PreventFallingInPitsEver = false;
				enemy.aiActor.healthHaver.ForceSetCurrentHealth(18f);
				enemy.aiActor.CollisionKnockbackStrength = 5f;
				enemy.aiActor.CanTargetPlayers = true;
				enemy.aiActor.healthHaver.SetHealthMaximum(18f, null, false);

				tk2dSpriteAnimator component2 = prefab.GetComponent<tk2dSpriteAnimator>();

				prefab.AddAnimation("idle_back_right", "Planetside/Resources/Enemies/Grenadier/Idle/BackRightIdle", fps: 3, AnimationType.Idle, DirectionType.FourWay);
				prefab.AddAnimation("idle_front_right", "Planetside/Resources/Enemies/Grenadier/Idle/FrontRightIdle", fps: 3, AnimationType.Idle, DirectionType.FourWay);
				prefab.AddAnimation("idle_front_left", "Planetside/Resources/Enemies/Grenadier/Idle/FrontLeftIdle", fps: 3, AnimationType.Idle, DirectionType.FourWay);
				prefab.AddAnimation("idle_back_left", "Planetside/Resources/Enemies/Grenadier/Idle/BackLeftIdle", fps: 3, AnimationType.Idle, DirectionType.FourWay);

				//
				prefab.AddAnimation("run_back_right", "Planetside/Resources/Enemies/Grenadier/Move/BackRightMove", fps: 8, AnimationType.Move, DirectionType.FourWay);
				prefab.AddAnimation("run_front_right", "Planetside/Resources/Enemies/Grenadier/Move/FrontRightMove", fps: 8, AnimationType.Move, DirectionType.FourWay);
				prefab.AddAnimation("run_front_left", "Planetside/Resources/Enemies/Grenadier/Move/FrontLeftMove", fps: 8, AnimationType.Move, DirectionType.FourWay);
				prefab.AddAnimation("run_back_left", "Planetside/Resources/Enemies/Grenadier/Move/BackLeftMove", fps: 8, AnimationType.Move, DirectionType.FourWay);
				//

				prefab.AddAnimation("die", "Planetside/Resources/Enemies/Grenadier/Death", fps: 20, AnimationType.Move, DirectionType.Single, tk2dSpriteAnimationClip.WrapMode.Once, assignAnimation: false);

				DirectionalAnimation die = new DirectionalAnimation()
				{
					AnimNames = new string[] { "die" },
					Flipped = new FlipType[] { FlipType.None },
					Type = DirectionType.TwoWayHorizontal,
					Prefix = string.Empty
				};



				enemy.aiAnimator.AssignDirectionalAnimation("die", die, AnimationType.Other);

				//If someone is able to do this better than me, im genuinely curious and would love to know
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_back_right").frames[0].eventAudio = "Play_FS_bell_step_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_back_right").frames[0].triggerEvent = true;
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_back_right").frames[3].eventAudio = "Play_FS_bell_step_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_back_right").frames[3].triggerEvent = true;

				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_front_right").frames[0].eventAudio = "Play_FS_bell_step_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_front_right").frames[0].triggerEvent = true;
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_front_right").frames[3].eventAudio = "Play_FS_bell_step_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_front_right").frames[3].triggerEvent = true;

				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_front_left").frames[0].eventAudio = "Play_FS_bell_step_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_front_left").frames[0].triggerEvent = true;
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_front_left").frames[3].eventAudio = "Play_FS_bell_step_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_front_left").frames[3].triggerEvent = true;

				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_back_left").frames[0].eventAudio = "Play_FS_bell_step_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_back_left").frames[0].triggerEvent = true;
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_back_left").frames[3].eventAudio = "Play_FS_bell_step_01";
				prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("run_back_left").frames[3].triggerEvent = true;

				enemy.aiActor.specRigidbody.PixelColliders.Clear();
				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 0,
					ManualOffsetY = 0,
					ManualWidth = 14,
					ManualHeight = 24,
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
					ManualOffsetX = 0,
					ManualOffsetY = 0,
					ManualWidth = 14,
					ManualHeight = 24,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});

				enemy.aiActor.PreventBlackPhantom = false;

				AIAnimator aiAnimator = enemy.aiAnimator;
				var yah = enemy.transform.Find("GunAttachPoint").gameObject;
				yah.transform.position = enemy.aiActor.transform.position;
				yah.transform.localPosition = new Vector2(-0.2f, .3f);
				tk2dBaseSprite bals = yah.GetOrAddComponent<tk2dBaseSprite>();
				AIActor SourceEnemy = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
				//enemy.aiShooter.handObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").aiShooter.handObject;
				EnemyBuilder.DuplicateAIShooterAndAIBulletBank(prefab, SourceEnemy.aiShooter, SourceEnemy.GetComponent<AIBulletBank>(), 19, yah.transform);
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
						Probability = 1f,
						Behavior = new ShootGunBehavior()
						{
							WeaponType = WeaponType.BulletScript,
							BulletScript = new CustomBulletScriptSelector(typeof(HunterKinScript)),
							LeadAmount = 0,
							LeadChance = 1,
							AttackCooldown = 1f,
							RequiresLineOfSight = false,
							FixTargetDuringAttack = false,
							StopDuringAttack = false,
							RespectReload = true,
							MagazineCapacity = 1,
							ReloadSpeed = 3f,
							EmptiesClip = true,
							SuppressReloadAnim = false,
							CooldownVariance = 0,
							GlobalCooldown = 0,
							InitialCooldown = 0,
							InitialCooldownVariance = 0,
							GroupName = null,
							GroupCooldown = 0,
							MinRange = 0,
							Range = 1337,
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
						NickName = "GRENADE"
					},
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
				Game.Enemies.Add("psog:grenadier", enemy.aiActor);


			}

		}


		public class EnemyBehavior : BraveBehaviour
		{
			private RoomHandler m_StartRoom;
			private void Update()
			{
				if (!base.aiActor.HasBeenEngaged) { CheckPlayerRoom(); }
			}
			private void CheckPlayerRoom()
			{
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom) { base.aiActor.HasBeenEngaged = true; }
			}
			private void Start()
			{
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) => { AkSoundEngine.PostEvent("Play_VO_phantom_death_01", base.aiActor.gameObject); AkSoundEngine.PostEvent("Play_OBJ_mine_set_01", base.aiActor.gameObject); base.aiShooter.ToggleGunAndHandRenderers(false, "fuck you no hand");
				};
				base.aiActor.healthHaver.OnDeath += (obj) => { Exploder.DoDefaultExplosion(base.aiActor.sprite.WorldCenter, default(Vector2), null, false, CoreDamageTypes.None, true); };

			}

		}	
		public class HunterKinScript : Script
		{
			public override IEnumerator Top()
			{

				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("880bbe4ce1014740ba6b4e2ea521e49d").bulletBank.GetBullet("grenade"));
				}
				float airTime = base.BulletBank.GetBullet("grenade").BulletObject.GetComponent<ArcProjectile>().GetTimeInFlight();
				Vector2 vector = this.BulletManager.PlayerPosition();
				Bullet bullet2 = new Bullet("grenade", false, false, false);
				float direction2 = (vector - base.Position).ToAngle();
				base.Fire(new Direction(direction2, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), bullet2);
				(bullet2.Projectile as ArcProjectile).AdjustSpeedToHit(vector);
				(bullet2.Projectile as ArcProjectile).specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine((bullet2.Projectile as ArcProjectile).specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));

				bullet2.Projectile.ImmuneToSustainedBlanks = true;
				yield break;
			}
			private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
			{
				bool flag = otherRigidbody.gameObject.name != null;
				if (flag)
				{
					bool flag2 = otherRigidbody.gameObject.name == "Table_Vertical" || otherRigidbody.gameObject.name == "Table_Horizontal";
					if (flag2)
					{
						PhysicsEngine.SkipCollision = true;
					}
				}
			}
		}
		
	}
}

