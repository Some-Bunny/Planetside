using System;
using System.Collections;
using System.Collections.Generic;
using Brave.BulletScript;
using Dungeonator;
using FullInspector;
using UnityEngine;
using ItemAPI;
using EnemyBulletBuilder;

namespace Planetside
{


	public class MineFlayerChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "8b0dd96e2fe74ec7bebc1bc689c0008a"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			//actor.MovementSpeed *= 0.8f; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);

			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableMineflayerBounce);

			//scatterBullet

			ShootBehavior shoot1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as ShootBehavior;
			shoot1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMineFlayerSoundWaves1));

			ShootBehavior shoot2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootBehavior;
			shoot2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMineFlayerBong1));

			ShootBehavior shoot3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[4].Behavior as ShootBehavior;
			shoot3.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMineFlayerMineSeeking1));
	
			ShootBehavior shoot4 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
			shoot4.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMineFlayerMineCircle1));

			/*
			BeholsterLaserBehavior shootLaser = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[4].Behavior as BeholsterLaserBehavior;
			shootLaser.trackingType = BeholsterLaserBehavior.TrackingType.ConstantTurn;
			shootLaser.maxTurnRate = 144;
			*/
			//behaviorSpec.aiActor.CanTargetPlayers = true;
			//behaviorSpec.aiActor.CanTargetEnemies = false;

			// For this first change, we're just going to increase the lead amount of the bullet kin's ShootGunBehavior so its shots fire like veteran kin.
			//ShootGunBehavior shootGunBehavior = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as ShootGunBehavior;
			//ShootBehavior shootGunBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as ShootBehavior;
			//ShootBehavior shootGunBehavior = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootBehavior;
			//ShootBehavior shootGunBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as ShootBehavior;
			//ShootGunBehavior shootGunBehavior = behaviorSpec.AttackBehaviors[0] as ShootGunBehavior;
			//base.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
			//base.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));
			//yah.chargeSpeed = 30;
			//yah.chargeRange = 30;
			//yah.AttackCooldown = 1f;
			//yah.delayWallRecoil = false;
			//yah.wallRecoilForce = 160000;	
			//shootGunBehavior.WeaponType = WeaponType.BulletScript;

			//shootGunBehavior.BulletScript = new CustomBulletScriptSelector(typeof(Bullettest));



			// Next, we're going to change another few things on the ShootGunBehavior so that it has a custom BulletScript.
			// Makes it so the bullet kin will shoot our bullet script instead of his own gun shot.
			//shootGunBehavior.BulletScript = new CustomBulletScriptSelector(typeof(Death)); // Sets the bullet kin's bullet script to our custom bullet script.
			//behaviorSpec.aiActor.get
		}



		public class ModifiedMineFlayerMineCircle1 : Script
		{
			public override IEnumerator Top()
			{
				int numMines = ((double)this.BulletBank.healthHaver.GetCurrentHealthPercentage() <= 0.5) ? 12 : 9;
				int goopExceptionId = DeadlyDeadlyGoopManager.RegisterUngoopableCircle(this.BulletBank.specRigidbody.UnitCenter, 2f);
				yield return this.Wait(72);
				List<AIActor> spawnedActors = new List<AIActor>();
				Vector2 roomCenter = this.BulletBank.aiActor.ParentRoom.area.UnitCenter;
				for (int i = 0; i < numMines; i++)
				{
					float angle = UnityEngine.Random.Range(-60f, 60f) + (float)((i % 2 != 0) ? 180 : 0);
					float radius = ModifiedMineFlayerMineCircle1.CircleRadii[i % ModifiedMineFlayerMineCircle1.CircleRadii.Length];
					Vector2 goal = roomCenter + BraveMathCollege.DegreesToVector(angle, radius);
					this.Fire(new Direction(angle, DirectionType.Absolute, -1f), new ModifiedMineFlayerMineCircle1.MineBullet(radius, goal, spawnedActors));
					yield return this.Wait(21);
				}
				yield return this.Wait(63);
				for (int j = 0; j < 19; j++)
				{
					float facingAngle = (float)((j % 2 != 0) ? 180 : 0);
					float targetAngle = (this.BulletManager.PlayerPosition() - this.Position).ToAngle();
					if (BraveMathCollege.AbsAngleBetween(facingAngle, targetAngle) < 90f && BraveUtility.RandomBool())
					{
						bool H = false;
						for (int l = -5; l < 5; l++)
						{
							
							this.Fire(new Direction(12.5f * l, DirectionType.Aim, -1f), new Speed(H == false ? 7 : 4, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name, 10, H == false ? 60 : 150));
							H = !H;
						}
					}
					yield return this.Wait(21);
				}
				for (int k = 0; k < spawnedActors.Count; k++)
				{
					AIActor actor = spawnedActors[k];
					if (actor && actor.healthHaver.IsAlive)
					{
						ExplodeOnDeath explodeOnDeath = actor.GetComponent<ExplodeOnDeath>();
						if (explodeOnDeath)
						{
							UnityEngine.Object.Destroy(explodeOnDeath);
						}
						actor.healthHaver.ApplyDamage(1E+10f, Vector2.zero, "Claymore Death", CoreDamageTypes.None, DamageCategory.Unstoppable, false, null, false);
						yield return this.Wait(3);
					}
				}
				DeadlyDeadlyGoopManager.DeregisterUngoopableCircle(goopExceptionId);
				yield break;
			}

			static ModifiedMineFlayerMineCircle1()
			{
				ModifiedMineFlayerMineCircle1.CircleRadii = new float[]
				{
			4f,
			9f,
			14f
				};
			}

			private const int FlightTime = 60;

			private const string EnemyGuid = "566ecca5f3b04945ac6ce1f26dedbf4f";

			private const float EnemyAngularSpeed = 30f;

			private const float EnemyAngularSpeedDelta = 5f;

			private const int BulletsPerSpray = 5;

			private static readonly float[] CircleRadii;

			private class MineBullet : Bullet
			{
				// Token: 0x06000A27 RID: 2599 RVA: 0x0002EC03 File Offset: 0x0002CE03
				public MineBullet(float radius, Vector2 goalPos, List<AIActor> spawnedActors) : base("mine", false, false, false)
				{
					this.m_radius = radius;
					this.m_goalPos = goalPos;
					this.m_spawnedActors = spawnedActors;
				}

				// Token: 0x06000A28 RID: 2600 RVA: 0x0002EC28 File Offset: 0x0002CE28
				public override IEnumerator Top()
				{
					this.ManualControl = true;
					this.Direction = (this.m_goalPos - this.Position).ToAngle();
					this.Speed = Vector2.Distance(this.m_goalPos, this.Position) / 1f;
					Vector2 truePosition = this.Position;
					for (int i = 0; i < 60; i++)
					{
						truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
						this.Position = truePosition + new Vector2(0f, Mathf.Sin((float)i / 60f * 3.1415927f) * 3.5f);
						yield return this.Wait(1);
					}
					Vector2 spawnPos = this.Projectile.specRigidbody.UnitBottomLeft;
					AIActor spawnedEnemy = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid("566ecca5f3b04945ac6ce1f26dedbf4f"), spawnPos, this.BulletBank.aiActor.ParentRoom, true, AIActor.AwakenAnimationType.Awaken, true);
					this.m_spawnedActors.Add(spawnedEnemy);
					CircleRoomBehavior circleRoomBehavior = spawnedEnemy.behaviorSpeculator.MovementBehaviors[0] as CircleRoomBehavior;
					circleRoomBehavior.Radius = this.m_radius;
					circleRoomBehavior.Direction = (float)((this.m_radius != 9f) ? 1 : -1);
					float angularSpeed = 30f + UnityEngine.Random.Range(-5f, 5f);
					spawnedEnemy.MovementSpeed = angularSpeed * 0.017453292f * this.m_radius;
					this.Vanish(false);
					yield break;
				}

				// Token: 0x04000A74 RID: 2676
				private float m_radius;

				// Token: 0x04000A75 RID: 2677
				private Vector2 m_goalPos;

				// Token: 0x04000A76 RID: 2678
				private List<AIActor> m_spawnedActors;
			}
		}


		public class ModifiedMineFlayerMineSeeking1 : Script
		{
			public override IEnumerator Top()
			{
				int numMines = ((double)this.BulletBank.healthHaver.GetCurrentHealthPercentage() <= 0.5) ? 12 : 9;
				int goopExceptionId = DeadlyDeadlyGoopManager.RegisterUngoopableCircle(this.BulletBank.specRigidbody.UnitCenter, 2f);
				yield return this.Wait(72);
				List<AIActor> spawnedActors = new List<AIActor>();
				Vector2 roomCenter = this.BulletBank.aiActor.ParentRoom.area.UnitCenter;
				for (int i = 0; i < numMines; i++)
				{
					float angle = UnityEngine.Random.Range(-60f, 60f) + (float)((i % 2 != 0) ? 180 : 0);
					Vector2 goal = roomCenter + BraveMathCollege.DegreesToVector(angle, ModifiedMineFlayerMineSeeking1.CircleRadius);
					this.Fire(new Direction(angle, DirectionType.Absolute, -1f), new ModifiedMineFlayerMineSeeking1.MineBullet(goal, spawnedActors, i));
					yield return this.Wait(21);
				}
				yield return this.Wait(63);
				for (int j = 0; j < 19; j++)
				{
					float facingAngle = (float)((j % 2 != 0) ? 180 : 0);
					float targetAngle = (this.BulletManager.PlayerPosition() - this.Position).ToAngle();
					if (BraveMathCollege.AbsAngleBetween(facingAngle, targetAngle) < 90f && BraveUtility.RandomBool())
					{
						for (int l = 0; l < 10; l++)
						{
							float direction = this.SubdivideArc(targetAngle - 50f, 100f, 5, l, false);
							this.Fire(new Direction(direction, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name));
							this.Fire(new Direction(direction, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name));
						}
					}
					yield return this.Wait(21);
				}
				for (int k = 0; k < spawnedActors.Count; k++)
				{
					AIActor actor = spawnedActors[k];
					if (actor && actor.healthHaver.IsAlive)
					{
						ExplodeOnDeath explodeOnDeath = actor.GetComponent<ExplodeOnDeath>();
						if (explodeOnDeath)
						{
							UnityEngine.Object.Destroy(explodeOnDeath);
						}
						actor.healthHaver.ApplyDamage(1E+10f, Vector2.zero, "Claymore Death", CoreDamageTypes.None, DamageCategory.Unstoppable, false, null, false);
						yield return this.Wait(3);
					}
				}
				DeadlyDeadlyGoopManager.DeregisterUngoopableCircle(goopExceptionId);
				yield break;
			}
			static ModifiedMineFlayerMineSeeking1()
			{
				ModifiedMineFlayerMineSeeking1.CircleRadius = 14f;
			}

			private const int FlightTime = 60;

			private const string EnemyGuid = "566ecca5f3b04945ac6ce1f26dedbf4f";

			private const float EnemyAngularSpeed = 30f;

			private const float EnemyAngularSpeedDelta = 5f;

			private const int BulletsPerSpray = 5;

			private static readonly float CircleRadius;

			// Token: 0x0200029C RID: 668
			private class MineBullet : Bullet
			{
				public MineBullet(Vector2 goalPos, List<AIActor> spawnedActors, int index) : base("mine", false, false, false)
				{
					this.m_goalPos = goalPos;
					this.m_spawnedActors = spawnedActors;
					this.m_index = index;
				}

				public override IEnumerator Top()
				{
					this.ManualControl = true;
					this.Direction = (this.m_goalPos - this.Position).ToAngle();
					this.Speed = Vector2.Distance(this.m_goalPos, this.Position) / 1f;
					Vector2 truePosition = this.Position;
					for (int i = 0; i < 60; i++)
					{
						truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
						this.Position = truePosition + new Vector2(0f, Mathf.Sin((float)i / 60f * 3.1415927f) * 3.5f);
						yield return this.Wait(1);
					}
					Vector2 spawnPos = this.Projectile.specRigidbody.UnitBottomLeft;
					AIActor spawnedEnemy = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid("566ecca5f3b04945ac6ce1f26dedbf4f"), spawnPos, this.BulletBank.aiActor.ParentRoom, true, AIActor.AwakenAnimationType.Awaken, true);
					this.m_spawnedActors.Add(spawnedEnemy);
					WaitThenChargeBehavior waitThenChargeBehavior = new WaitThenChargeBehavior();
					spawnedEnemy.behaviorSpeculator.MovementBehaviors[0] = waitThenChargeBehavior;
					waitThenChargeBehavior.Delay = (float)(9 - this.m_index) * 0.35f + 0.7f * (float)this.m_index;
					this.Vanish(false);
					yield break;
				}

				private Vector2 m_goalPos;

				private List<AIActor> m_spawnedActors;

				private int m_index;
			}
		}


		public class ModifiedMineFlayerBong1 : Script
		{
			public override IEnumerator Top()
			{
				float startDirection = this.RandomAngle();
				float delta = 15f;
				for (int i = 0; i < 100; i++)
				{
					this.Fire(new Direction(startDirection + (float)i * delta, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new SlowDown());
					this.Fire(new Direction(startDirection + (float)i * delta + 180f, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new SlowDown());
					yield return this.Wait(1);
				}
				yield break;
			}

			private class SlowDown : Bullet
			{
				public SlowDown() : base(StaticUndodgeableBulletEntries.undodgeableDefault.Name, false, false, false)
				{
				}

				public override IEnumerator Top()
				{
					this.Projectile.IgnoreTileCollisionsFor(1f);

					this.ChangeSpeed(new Speed(2f, SpeedType.Absolute), 50);
					yield return this.Wait(90);
					this.ChangeSpeed(new Speed(11f, SpeedType.Absolute), 75);
					yield return this.Wait(120);
					
					yield break;
				}
			}

			private const int NumBullets = 90;
		}


		public class ModifiedMineFlayerSoundWaves1 : Script
		{
			public override IEnumerator Top()
			{
				float delta = 22.5f;
				for (int i = 0; i < 5; i++)
				{
					yield return this.Wait(33);
					int numBullets = 16;
					float startDirection = this.RandomAngle();
					if (i == 4)
					{
						for (int j = 0; j < numBullets; j++)
                        {
							this.Fire(new Direction((startDirection + (float)j * delta) + 11.25f, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new ModifiedMineFlayerSoundWaves1.ReflectBullet());

						}
					}
					for (int j = 0; j < numBullets; j++)
					{
						this.Fire(new Direction(startDirection + (float)j * delta, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new ModifiedMineFlayerSoundWaves1.ReflectBullet());
					}
					yield return this.Wait(20);
				}
				yield break;
			}

			private const int NumWaves = 5;

			private const int NumBullets = 18;

			private class ReflectBullet : Bullet
			{
				public ReflectBullet() : base(StaticUndodgeableBulletEntries.undodgeableMineflayerBounce.Name, false, false, false)
				{
					this.m_ticksLeft = -1;
				}

				public override IEnumerator Top()
				{
					SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
					specRigidbody.OnTileCollision = (SpeculativeRigidbody.OnTileCollisionDelegate)Delegate.Combine(specRigidbody.OnTileCollision, new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision));
					this.Projectile.BulletScriptSettings.surviveTileCollisions = true;
					while (this.m_ticksLeft < 0)
					{
						if (this.ManualControl)
						{
							this.Reflect();
							this.ManualControl = false;
						}
						yield return this.Wait(1);
					}
					yield return this.Wait(this.m_ticksLeft);
					this.Vanish(false);
					yield break;
				}

				private void OnTileCollision(CollisionData tilecollision)
				{
					this.Reflect();
				}

				private void Reflect()
				{
					this.Speed = 8f;
					this.Direction += 180f + UnityEngine.Random.Range(-10f, 10f);
					this.Velocity = BraveMathCollege.DegreesToVector(this.Direction, this.Speed);
					PhysicsEngine.PostSliceVelocity = new Vector2?(this.Velocity);
					this.m_ticksLeft = (int)((float)base.Tick * 1.5f);
					if (this.Projectile.TrailRendererController)
					{
						this.Projectile.TrailRendererController.Stop();
					}
					this.Projectile.BulletScriptSettings.surviveTileCollisions = false;
					SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
					specRigidbody.OnTileCollision = (SpeculativeRigidbody.OnTileCollisionDelegate)Delegate.Remove(specRigidbody.OnTileCollision, new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision));
				}

				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (this.Projectile)
					{
						SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
						specRigidbody.OnTileCollision = (SpeculativeRigidbody.OnTileCollisionDelegate)Delegate.Remove(specRigidbody.OnTileCollision, new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision));
					}
				}

				private int m_ticksLeft;
			}
		}

	}
}
