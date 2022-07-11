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


	public class DoorLordChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "9189f46c47564ed588b9108965f975c9"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			//actor.MovementSpeed *= 2; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);

			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableDoorLordBurst);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableDoorLordFlame);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBulletSoundless);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableDoorLordPuke);

			ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBossDoorMimicBurst2));

			ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[4].Behavior as ShootBehavior;
			ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBossDoorMimicFlames1));

			ShootBehavior ShootBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as ShootBehavior;
			ShootBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBossDoorMimicWaves1));

			ShootBehavior ShootBehavior4 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
			ShootBehavior4.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBossDoorMimicPuke1));
		}


		public class ModifiedBossDoorMimicPuke1 : Script
		{
			protected override IEnumerator Top()
			{
				float pulseStartAngle = this.RandomAngle();
				for (int j = 0; j < 16; j++)
				{
					float direction2 = this.SubdivideCircle(pulseStartAngle, 16, j, 1f, false);
					this.Fire(new Direction(direction2, DirectionType.Absolute, -1f), new Speed(4.5f, SpeedType.Absolute), new ModifiedBossDoorMimicPuke1.PulseBullet((float)(2 * j) / 32f));
				}
				for (int k = 0; k < 8; k++)
				{
					float num = this.RandomAngle();
					Vector2 predictedTargetPosition = this.GetPredictedTargetPosition((float)(((double)UnityEngine.Random.value >= 0.5) ? 1 : 0), 8f);
					int num2 = UnityEngine.Random.Range(0, 10);
					for (int l = 0; l < 5; l++)
					{
						this.Fire(new Offset(new Vector2(0f, 1f), num, string.Empty, DirectionType.Absolute), new Direction(num, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new ModifiedBossDoorMimicPuke1.SnakeBullet(num2 + l * 3, predictedTargetPosition, false));
					}
				}
				yield return this.Wait(40);
				for (int m = 0; m < 24; m++)
				{
					float direction3 = this.SubdivideCircle(pulseStartAngle, 24, m, 1f, true);
					this.Fire(new Direction(direction3, DirectionType.Absolute, -1f), new Speed(4.5f, SpeedType.Absolute), new ModifiedBossDoorMimicPuke1.PulseBullet((float)(2 * m) / 32f));
				}
				for (int i = 0; i < 5; i++)
				{
					float direction = this.RandomAngle();
					Vector2 targetPos = this.GetPredictedTargetPosition((float)(((double)UnityEngine.Random.value >= 0.5) ? 1 : 0), 8f);
					bool shouldHome = UnityEngine.Random.value < 0.33f;
					for (int n = 0; n < 5; n++)
					{
						this.Fire(new Offset(new Vector2(0f, 1f), direction, string.Empty, DirectionType.Absolute), new Direction(direction, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new ModifiedBossDoorMimicPuke1.SnakeBullet(n * 3, targetPos, shouldHome));
					}
					yield return this.Wait(10);
				}
				for (int m = 0; m < 32; m++)
				{
					float direction3 = this.SubdivideCircle(pulseStartAngle, 32, m, 1f, true);
					this.Fire(new Direction(direction3, DirectionType.Absolute, -1f), new Speed(4.5f, SpeedType.Absolute), new ModifiedBossDoorMimicPuke1.PulseBullet((float)(2 * m) / 32f));
				}
				yield return this.Wait(90);
				yield break;
			}

			private const int NumPulseBullets = 32;

			private const float PulseBulletSpeed = 4.5f;

			private const int NumInitialSnakes = 8;

			private const int NumLateSnakes = 6;

			private const int NumBulletsInSnake = 5;

			private const int SnakeBulletSpeed = 8;

			public class PulseBullet : Bullet
			{
				public PulseBullet(float initialOffest) : base(StaticUndodgeableBulletEntries.UndodgeableDoorLordPuke.Name, false, false, false)
				{
					this.m_initialOffest = initialOffest;
				}

				protected override IEnumerator Top()
				{
					this.ManualControl = true;
					Vector2 truePosition = this.Position;
					Vector2 offsetDir = BraveMathCollege.DegreesToVector(this.Direction, 1f);
					for (int i = 0; i < 600; i++)
					{
						this.UpdateVelocity();
						truePosition += this.Velocity / 60f;
						float mag = Mathf.Sin((this.m_initialOffest + (float)this.Tick / 60f / 0.75f) * 3.1415927f) * 0.75f;
						if (i < 60)
						{
							mag *= (float)i / 60f;
						}
						this.Position = truePosition + offsetDir * mag;
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}

				private const float SinPeriod = 0.75f;

				private const float SinMagnitude = 0.75f;

				private float m_initialOffest;
			}

			public class SnakeBullet : Bullet
			{
				public SnakeBullet(int delay, Vector2 target, bool shouldHome) : base("puke_snake", false, false, false)
				{
					this.m_delay = delay;
					this.m_target = target;
					this.m_shouldHome = shouldHome;
				}

				protected override IEnumerator Top()
				{
					this.ManualControl = true;
					yield return this.Wait(this.m_delay);
					Vector2 truePosition = this.Position;
					for (int i = 0; i < 360; i++)
					{
						float offsetMagnitude = Mathf.SmoothStep(-0.75f, 0.75f, Mathf.PingPong(0.5f + (float)i / 60f * 3f, 1f));
						if (this.m_shouldHome && i > 20 && i < 60)
						{
							float num = (this.m_target - truePosition).ToAngle();
							float value = BraveMathCollege.ClampAngle180(num - this.Direction);
							this.Direction += Mathf.Clamp(value, -6f, 6f);
						}
						truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
						this.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude);
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}

				private const float SnakeMagnitude = 0.75f;

				private const float SnakePeriod = 3f;
				private int m_delay;
				private Vector2 m_target;

				private bool m_shouldHome;
			}
		}

		public class ModifiedBossDoorMimicWaves1 : Script
		{
			protected override IEnumerator Top()
			{
				for (int i = 0; i < 9; i++)
				{
					bool offset = false;
					int numBullets = 7;
					if (i % 2 == 1)
					{
						offset = true;
						numBullets--;
					}
					float startDirection = this.AimDirection - 40f;
					for (int j = 0; j < numBullets; j++)
					{
						this.Fire(new Direction(this.SubdivideArc(startDirection, 80f, 5, j, offset), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(4, 6), SpeedType.Absolute), new SpeedChangingBullet("teeth_wave", 12, 120));
					}
					this.Fire(new Direction(this.AimDirection - UnityEngine.Random.Range(39, 48), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(8, 10), SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBulletSoundless.Name, false, false, false));
					this.Fire(new Direction(this.AimDirection + UnityEngine.Random.Range(39, 48), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(8, 10), SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBulletSoundless.Name, false, false, false));
					yield return this.Wait(30);
				}
				yield break;
			}

			private const int NumWaves = 7;
			private const int NumBulletsPerWave = 5;
			private const float Arc = 60f;
		}

		public class ModifiedBossDoorMimicFlames1 : Script
		{
			protected override IEnumerator Top()
			{
				for (int i = 0; i < 60; i++)
				{
					this.Fire(new Offset((i % 2 != 0) ? "right eye" : "left eye"), new Direction(UnityEngine.Random.Range(-60f, 60f), DirectionType.Aim, -1f), new Speed(13f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableDoorLordFlame.Name, 6, 120));
					yield return this.Wait(8);
				}
				yield break;
			}
			private const int NumBullets = 70;
		}

		public class ModifiedBossDoorMimicBurst2 : Script
		{
			protected override IEnumerator Top()
			{
				float floatDirection = this.RandomAngle();
				for (int i = 0; i < 4; i++)
				{
					float startDirection = this.RandomAngle();
					Vector2 floatVelocity = BraveMathCollege.DegreesToVector(floatDirection, 3f);
					for (int j = 0; j < 20; j++)
					{
						this.Fire(new Direction(this.SubdivideCircle(startDirection, 20, j, 1f, false), DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new ModifiedBossDoorMimicBurst2.BurstBullet(floatVelocity));
					}
					floatDirection = floatDirection + 180f + UnityEngine.Random.Range(-60f, 60f);
					yield return this.Wait(90);
				}
				yield return this.Wait(75);
				yield break;
			}

			private const int NumBursts = 5;

			private const int NumBullets = 36;

			public class BurstBullet : Bullet
			{
				public BurstBullet(Vector2 additionalVelocity) : base(StaticUndodgeableBulletEntries.UndodgeableDoorLordBurst.Name, false, false, false)
				{
					this.m_addtionalVelocity = additionalVelocity;
				}

				protected override IEnumerator Top()
				{
					this.ManualControl = true;
					for (int i = 0; i < 300; i++)
					{
						this.UpdateVelocity();
						this.Velocity += this.m_addtionalVelocity * Mathf.Min(9f, (float)i / 30f);
						this.UpdatePosition();
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}

				private Vector2 m_addtionalVelocity;
			}
		}
	}
}
