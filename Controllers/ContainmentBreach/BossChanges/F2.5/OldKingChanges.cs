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


	public class OldKingChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "5729c8b5ffa7415bb3d01205663a33ef"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			//actor.MovementSpeed *= 2; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableOldKingSlamBullet);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableOldKingSuckBullet);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBullet);

			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
			//UndodgeableOldKingSlamBullet
			SpinAttackBehavior shootWave = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as SpinAttackBehavior;
			shootWave.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletKingCrazySpin1));
			shootWave.AttackCooldown *= 2.25f; 

			ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletKingSlam1));
			ShootBehavior1.AttackCooldown *= 1.5f;

			ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[11].Behavior as ShootBehavior;
			ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletKingSuck1));

			ShootBehavior ShootBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[10].Behavior as ShootBehavior;
			ShootBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletKingHomingRing1));
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
		//public AIBulletBank.Entry ThesporeEntry;
		public AIBulletBank.Entry lol;


		public class ModifiedBulletKingHomingRing1 : Script
		{
			public override IEnumerator Top()
			{
				this.EndOnBlank = true;
				float startDirection = this.RandomAngle();
				float delta = 60f;
				for (int i = 0; i < 6; i++)
				{
					this.Fire(new Offset(0.0625f, 3.5625f, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Aim, -1f), new Speed(4.5f, SpeedType.Absolute), new ModifiedBulletKingHomingRing1.SmokeBullet(startDirection + (float)i * delta));
				}
				yield return this.Wait(45);
				yield break;
			}

			private const int NumBullets = 24;

			public class SmokeBullet : Bullet
			{
				public SmokeBullet(float angle) : base(StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBullet.Name, false, false, true)
				{
					this.m_angle = angle;
				}

				public override IEnumerator Top()
				{
					this.ManualControl = true;
					Vector2 centerPosition = this.Position;
					float radius = 0f;
					int i = 0;
					while ((float)i < 600f)
					{
						float desiredAngle = (this.BulletManager.PlayerPosition() - centerPosition).ToAngle();
						this.Direction = Mathf.MoveTowardsAngle(this.Direction, desiredAngle, 1f);
						float speedScale = 1f;
						if (i < 60)
						{
							speedScale = Mathf.SmoothStep(0f, 1f, (float)i / 60f);
						}
						this.UpdateVelocity();
						centerPosition += this.Velocity / 60f * speedScale;
						if (i < 60)
						{
							radius += 0.01666f;
						}
						this.m_angle += 2f;
						this.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);
						yield return this.Wait(1);
						i++;
					}
					this.Vanish(false);
					yield break;
				}

				private const float ExpandSpeed = 2f;

				private const float SpinSpeed = 120f;

				private const float Lifetime = 600f;

				private float m_angle;
			}
		}

		public class ModifiedBulletKingSuck1 : Script
		{
			public override IEnumerator Top()
			{
				this.EndOnBlank = true;
				float startAngle = this.RandomAngle();
				float radius = 1f;
				for (int j = 0; j < 15; j++)
				{
					for (int k = 0; k < 4; k++)
					{
						float num = this.SubdivideCircle(startAngle, 4, k, 1f, false);

						Vector2 overridePosition = this.Position + BraveMathCollege.DegreesToVector(num, radius);

						this.Fire(Offset.OverridePosition(overridePosition), new Direction(num, DirectionType.Absolute, -1f), new ModifiedBulletKingSuck1.SuckBullet(this.Position, num, j, true));
						this.Fire(Offset.OverridePosition(overridePosition), new Direction(num, DirectionType.Absolute, -1f), new ModifiedBulletKingSuck1.SuckBullet(this.Position, num, j, false));

					}
				}
				yield return this.Wait(110);
			
				yield break;
			}

			static ModifiedBulletKingSuck1()
			{
				ModifiedBulletKingSuck1.SpinningBulletSpinSpeed = 180f;
			}

			private const int NumBulletRings = 20;

			private const int BulletsPerRing = 6;

			private const float AngleDeltaPerRing = 10f;

			private const float StartRadius = 1f;

			private const float RadiusPerRing = 1f;

			public static float SpinningBulletSpinSpeed;

			public class SuckBullet : Bullet
			{
				public SuckBullet(Vector2 centerPoint, float startAngle, int i, bool fli) : base(StaticUndodgeableBulletEntries.UndodgeableOldKingSuckBullet.Name, false, false, false)
				{
					this.m_centerPoint = centerPoint;
					this.m_startAngle = startAngle;
					this.m_index = i;
					this.flipp = fli;
				}

				public override IEnumerator Top()
				{
					this.ManualControl = true;
					float radius = 1f;
					float angle = this.m_startAngle;
					int remainingWait = 130;
					for (int i = 1; i < this.m_index; i++)
					{
						int steps = Mathf.Max(5, 15 - i);
						float deltaRadius = 2f / (float)steps;
						float deltaAngle = 30f / (float)steps;
						for (int j = 0; j < steps; j++)
						{
							radius += deltaRadius;
							if (flipp == true) { angle += deltaAngle; }
							else { angle -= deltaAngle; }
							this.Position = this.m_centerPoint + BraveMathCollege.DegreesToVector(angle, radius);
							yield return this.Wait(1.5f);
							remainingWait--;
						}
					}
					bool isDoingTell = false;
					while (remainingWait > 0)
					{
						if (!isDoingTell && remainingWait <= 60)
						{
							this.Projectile.spriteAnimator.Play("enemy_projectile_small_fire_tell");
							isDoingTell = true;
						}
						remainingWait--;
						yield return this.Wait(1);
					}
					this.Direction = (this.m_centerPoint - this.Position).ToAngle();
					float distToTravel = (this.m_centerPoint - this.Position).magnitude;
					this.ManualControl = false;
					for (int k = 0; k < 240; k++)
					{
						if (k < 40)
						{
							this.Speed += 0.2f;
						}
						distToTravel -= this.Speed / 60f;
						if (distToTravel < 2f)
						{
							this.Vanish(false);
							break;
						}
						yield return null;
					}
					this.Vanish(false);
					yield break;
				}

				private Vector2 m_centerPoint;

				private float m_startAngle;

				private int m_index;
				private bool flipp;
			}
		}


		public class ModifiedBulletKingSlam1 : Script
		{
			public override IEnumerator Top()
			{
				this.EndOnBlank = true;
				float startAngle = this.RandomAngle();
				float delta = 22.5f;
				for (int i = 0; i < 16; i++)
				{
					float num = startAngle + (float)i * delta;
					this.Fire(new Offset(1f, 0f, num, string.Empty, DirectionType.Absolute), new Direction(num, DirectionType.Absolute, -1f), new Speed(7, SpeedType.Absolute), new ModifiedBulletKingSlam1.SpinningBullet(this.Position, num, 0.25f));
				}
				for (int j = 0; j < 16; j++)
				{
					float num2 = startAngle + (float)j * delta;
					this.Fire(new Offset(1f, 0f, num2, string.Empty, DirectionType.Absolute), new Direction(num2, DirectionType.Absolute, -1f), new Speed(3, SpeedType.Absolute), new ModifiedBulletKingSlam1.SpinningBullet(this.Position, num2, -0.25f));
				}
				yield return this.Wait(90);
				yield break;
			}

			static ModifiedBulletKingSlam1()
			{
				ModifiedBulletKingSlam1.SpinningBulletSpinSpeed = 180f;
			}

			private const int NumBullets = 36;

			private const int NumHardBullets = 12;

			private const float RadiusAcceleration = 8f;

			private const float SpinAccelration = 10f;

			public static float SpinningBulletSpinSpeed;

			private const int Time = 180;

			public class SpinningBullet : Bullet
			{
				public SpinningBullet(Vector2 centerPoint, float startAngle, float SpinSpeed = 0.16666667f) : base(StaticUndodgeableBulletEntries.UndodgeableOldKingSlamBullet.Name)
				{
					this.centerPoint = centerPoint;
					this.startAngle = startAngle;
					this.SpinSpeed = SpinSpeed;
				}

				public override IEnumerator Top()
				{
					this.ManualControl = true;
					float radius = Vector2.Distance(this.centerPoint, this.Position);
					float speed = this.Speed;
					float spinAngle = this.startAngle;
					float spinSpeed = 0f;
					for (int i = 0; i < 180; i++)
					{
						speed += 0.13333334f;
						radius += speed / 60f;
						spinSpeed += SpinSpeed;
						spinAngle += spinSpeed / 60f;
						this.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}
				private float SpinSpeed;

				private Vector2 centerPoint;
				private float startAngle;
			}
		}


		public class ModifiedBulletKingCrazySpin1 : Script
		{
			public override IEnumerator Top()
			{
				Stop = false;
				for (int i = 0; i < 29; i++)
				{
					for (int j = 0; j < 6; j++)
					{
						float num = (float)j * 60f + 37f * (float)i;
						this.Fire(new Offset(1.66f, 0f, num, string.Empty, DirectionType.Absolute), new Direction(num, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SlowDown(this, i*10));
					}
					yield return this.Wait(6);
				}
				Stop = true;
				yield return this.Wait(30);

				for (int k = 0; k < 30; k++)
				{
					this.Fire(new Direction((float)k * 360f / 30f, DirectionType.Absolute, -1f), new Speed(12, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name));
				}
				yield break;
			}
			public bool Stop;
			private const int NumWaves = 29;
			private const int NumBulletsPerWave = 6;
			private const float AngleDeltaEachWave = 37f;
			private const int NumBulletsFinalWave = 64;
		}


		private class SlowDown : Bullet
		{
			public SlowDown(ModifiedBulletKingCrazySpin1 p, int W) : base(StaticUndodgeableBulletEntries.undodgeableDefault.Name, false, false, false)
			{
				parent = p;
				WaitTime = W;
			}
			public override IEnumerator Top()
			{
				while(parent.Stop == false)
                {
					yield return this.Wait(1);
				}
				this.StartTask(DoWackySlowdown());
				yield break;
			}
			public IEnumerator DoWackySlowdown()
            {
				base.ChangeSpeed(new Speed(1, SpeedType.Absolute), 60);
				yield return this.Wait(310 - WaitTime);
				base.Vanish(false);
				yield break;
			}
			private int WaitTime;

			private ModifiedBulletKingCrazySpin1 parent;
		}

	}
}
