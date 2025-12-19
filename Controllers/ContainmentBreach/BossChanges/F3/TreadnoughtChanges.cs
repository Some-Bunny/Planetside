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


	public class TreadnoughtChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "fa76c8cfdf1c4a88b55173666b4bc7fb"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			actor.MovementSpeed *= 0.8f; // Doubles the enemy movement speed


			//actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableDefault);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableMine);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableBigBullet);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableSmallSpore);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.UndodgeableScatterBullet);
            actor.bulletBank.Bullets.Add(StaticBulletEntries.TreadnaughtHoming);

            //scatterBullet
			
            ShootBehavior shootWave = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootBehavior;
			shootWave.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedTankTreaderHomingShot1));



			ShootBehavior shootWave1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[5].Behavior as ShootBehavior;
			shootWave1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedTankTreaderAreaDenial1));

			ShootBehavior shootWave2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
			shootWave2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedTankTreaderScatterShot1));
			behaviorSpec.AttackBehaviorGroup.AttackBehaviors[4].Probability *= 0.5f;
			

        }


		public class ModifiedTankTreaderScatterShot1 : Script
		{
			public override IEnumerator Top()
			{
				base.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new ModifiedTankTreaderScatterShot1.ScatterBullet());
				return null;
			}

			private const int AirTime = 30;
			private const int NumDeathBullets = 16;

			private class ScatterBullet : Bullet
			{
				public ScatterBullet() : base(StaticBulletEntries.UndodgeableScatterBullet.Name, false, false, false)
				{
				}

				public override IEnumerator Top()
				{
					yield return this.Wait(45);
					for (int i = -4; i < 5; i++)
					{
						this.Fire(new Direction(12*i, DirectionType.Relative, -1f), new Speed(7, SpeedType.Absolute), new ModifiedTankTreaderScatterShot1.LittleScatterBullet());
						this.Fire(new Direction(12 * i, DirectionType.Relative, -1f), new Speed(8.5f, SpeedType.Absolute), new ModifiedTankTreaderScatterShot1.LittleScatterBullet());
						this.Fire(new Direction(12 * i, DirectionType.Relative, -1f), new Speed(10, SpeedType.Absolute), new ModifiedTankTreaderScatterShot1.LittleScatterBullet());
					}
					this.Vanish(false);
					yield break;
				}
			}

			private class LittleScatterBullet : Bullet
			{
				public LittleScatterBullet() : base(StaticBulletEntries.undodgeableDefault.Name, false, false, false)
				{
				}

				public override IEnumerator Top()
				{
					this.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 40);
					yield return this.Wait(300);
					this.Vanish(false);
					yield break;
				}
			}
		}




		public class ModifiedTankTreaderAreaDenial1 : Script
		{
			public override IEnumerator Top()
			{
				base.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(ModifiedTankTreaderAreaDenial1.HugeBulletStartSpeed, SpeedType.Absolute), new ModifiedTankTreaderAreaDenial1.HugeBullet());
				return null;
			}

			static ModifiedTankTreaderAreaDenial1()
			{
				ModifiedTankTreaderAreaDenial1.HugeBulletStartSpeed = 6f;
				ModifiedTankTreaderAreaDenial1.HugeBulletDecelerationTime = 120;
				ModifiedTankTreaderAreaDenial1.HugeBulletHangTime = 300f;
				ModifiedTankTreaderAreaDenial1.SpinningBulletSpinSpeed = 120f;
			}

			public static float HugeBulletStartSpeed;

			public static int HugeBulletDecelerationTime;

			public static float HugeBulletHangTime;

			public static float SpinningBulletSpinSpeed;

			public class HugeBullet : Bullet
			{
				public HugeBullet() : base(StaticBulletEntries.undodgeableMine.Name, false, false, false)
				{
				}

				public override IEnumerator Top()
				{
					this.m_fireSemicircles = true;
					this.StartTask(this.FireSemicircles());
					this.ChangeSpeed(new Speed(0f, SpeedType.Absolute), ModifiedTankTreaderAreaDenial1.HugeBulletDecelerationTime);
					yield return this.Wait(ModifiedTankTreaderAreaDenial1.HugeBulletDecelerationTime);
					Vector2 truePosition = this.Position;
					this.ManualControl = true;
					int i = 0;
					while ((float)i < ModifiedTankTreaderAreaDenial1.HugeBulletHangTime)
					{
						if (this.m_fireSemicircles && (float)i > ModifiedTankTreaderAreaDenial1.HugeBulletHangTime - 45f)
						{
							this.m_fireSemicircles = false;
						}
						this.Position = truePosition + new Vector2(0.12f * ((float)i / ModifiedTankTreaderAreaDenial1.HugeBulletHangTime), 0f) * Mathf.Sin((float)i / 5f * 3.1415927f);
						yield return this.Wait(1);
						i++;
					}
				
					for (int k = 0; k < 6; k++)
					{
						base.Fire(new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new RotatedBulletBasic(-20, 0, 0, StaticBulletEntries.undodgeableBigBullet.Name, (k * 60), 0.133f));
						base.Fire(new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new RotatedBulletBasic(20, 0, 0, StaticBulletEntries.undodgeableBigBullet.Name, (k * 60), 0.133f));
					}
					this.Vanish(false);
					yield break;
				}

				private IEnumerator FireSemicircles()
				{
					yield return this.Wait(60);
					int phase = 0;
					while (this.m_fireSemicircles)
					{
						for (int i = 0; i < 36; i++)
						{
							if (i / 4 % 3 == phase)
							{
								this.Fire(new Direction((float)(i * 10), DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new SpeedChangingBullet(StaticBulletEntries.undodgeableDefault.Name, 4, 90));
								yield return this.Wait(2);
							}
						}
						yield return this.Wait(40);
						phase = (phase + 1) % 3;
					}
					yield break;
				}

				public class RotatedBulletBasic : Bullet
				{
					public RotatedBulletBasic(float spinspeed, float RevUp, float StartSpeenAgain, string BulletType, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
					{
						this.m_spinSpeed = spinspeed;
						this.TimeToRevUp = RevUp;
						this.StartAgain = StartSpeenAgain;
						this.m_angle = angle;
						this.m_radius = aradius;
						this.m_bulletype = BulletType;
						this.SuppressVfx = true;
					}

					public override IEnumerator Top()
					{
						base.ManualControl = true;
						Vector2 centerPosition = base.Position;
						float radius = 0f;
						for (int i = 0; i < 2400; i++)
						{
							if (i % 2 == 0)
							{
								base.Fire(new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new WeakSpore());
							}
							radius += m_radius;
							centerPosition += this.Velocity / 60f;
							base.UpdateVelocity();
							this.m_angle += this.m_spinSpeed / 60f;
							base.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);
							yield return base.Wait(1);
						}
						base.Vanish(false);
						yield break;
					}
					private const float ExpandSpeed = 4.5f;
					private const float SpinSpeed = 40f;
					private float m_angle;
					private float m_spinSpeed;
					private float m_radius;
					private string m_bulletype;
					private float TimeToRevUp;
					private float StartAgain;


				}
				public class WeakSpore : Bullet
				{
					public WeakSpore() : base(StaticBulletEntries.undodgeableSmallSpore.Name, false, false, false) { }
					public override IEnumerator Top()
					{
						yield return this.Wait(20);
						base.Vanish(false);
						yield break;
					}
				}


				private const int SemiCircleNumBullets = 4;
				private const int SemiCirclePhases = 3;
				private bool m_fireSemicircles;
			}
		}



		public class ModifiedTankTreaderHomingShot1 : Script
		{
			public override IEnumerator Top()
			{
				base.Fire(new Direction(0f, DirectionType.Aim, -1f), new Speed(5.5f, SpeedType.Absolute), new ModifiedTankTreaderHomingShot1.HomingBullet());
				return null;
			}

			private const int AirTime = 75;

			private const int NumDeathBullets = 16;

			private class HomingBullet : Bullet
			{
				public HomingBullet() : base(StaticBulletEntries.TreadnaughtHoming.Name, false, false, false)
				{
				}

				public override IEnumerator Top()
				{
					for (int i = 0; i < 75; i++)
					{
						this.ChangeDirection(new Direction(0f, DirectionType.Aim, 3f), 1);
						if (i == 45)
						{
							this.Projectile.spriteAnimator.Play("enemy_projectile_rocket_impact");
						}
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}

				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (preventSpawningProjectiles)
					{
						return;
					}
					float num = base.RandomAngle();
					float num2 = 40f;
					for (int i = 0; i < 9; i++)
					{
						base.Fire(new Direction(num + num2 * (float)i, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new SpeedChangingBullet(StaticBulletEntries.undodgeableDefault.Name, 8, 210));
						base.Fire(new Direction((num + num2 * (float)i)+22.5f, DirectionType.Absolute, -1f), new Speed(3f, SpeedType.Absolute), new SpeedChangingBullet(StaticBulletEntries.undodgeableDefault.Name, 12, 120));
					}
					AkSoundEngine.PostEvent("Play_WPN_golddoublebarrelshotgun_shot_01", this.Projectile.gameObject);
				}
			}
		}
	}
}
