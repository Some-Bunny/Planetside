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


	public class BulletKingChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "ffca09398635467da3b1f4a54bcfda80"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			//actor.MovementSpeed *= 2; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);
			//SpinAttackBehavior 

			SpinAttackBehavior SpinAttackBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as SpinAttackBehavior;
			SpinAttackBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifeidBulletKingCrazySpin1));

			ShootBehavior ShootBehavior1= behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifeidBulletKingSlam1));


			ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[5].Behavior as ShootBehavior;
			ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletKingDirectedFireRight));

			ShootBehavior ShootBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[6].Behavior as ShootBehavior;
			ShootBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletKingDirectedFireLeft));


			ShootBehavior ShootBehavior4 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[7].Behavior as ShootBehavior;
			ShootBehavior4.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletKingDirectedFireDownLeft));

			ShootBehavior ShootBehavior5 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[8].Behavior as ShootBehavior;
			ShootBehavior5.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletKingDirectedFireDownRight));


			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBulletKingSlam);//UndodgeableDirectedfire
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableDirectedfire);


		}

		public class ModifiedBulletKingDirectedFireDownRight : ModifiedBulletKingDirectedFire
		{
			protected override IEnumerator Top()
			{
				yield return this.Wait(10);
				this.DirectedShots(1.875f, 0.25f, 4f);
				yield return this.Wait(5);
				this.DirectedShots(1.625f, -0.1875f, 34f);
				yield return this.Wait(5);
				this.DirectedShots(1.4275f, -0.4375f, 64f);

				yield return this.Wait(35);
				this.DirectedShots(1.875f, 0.25f, 4f);
				yield return this.Wait(5);
				this.DirectedShots(1.625f, -0.1875f, 34f);
				yield return this.Wait(5);
				this.DirectedShots(1.4275f, -0.4375f, 64f);
				yield break;
			}
		}


		public class ModifiedBulletKingDirectedFireDownLeft : ModifiedBulletKingDirectedFire
		{
			protected override IEnumerator Top()
			{
				yield return this.Wait(10);
				this.DirectedShots(-1.3125f, -0.4375f, -4f);
				yield return this.Wait(5);
				this.DirectedShots(-1.5f, -0.1875f, -34f);
				yield return this.Wait(5);
				this.DirectedShots(-1.75f, 0.25f, -64f);

				yield return this.Wait(35);
				this.DirectedShots(-1.3125f, -0.4375f, -4f);
				yield return this.Wait(5);
				this.DirectedShots(-1.5f, -0.1875f, -34f);
				yield return this.Wait(5);
				this.DirectedShots(-1.75f, 0.25f, -64f);
				yield break;
			}
		}


		public class ModifiedBulletKingDirectedFireLeft : ModifiedBulletKingDirectedFire
		{
			protected override IEnumerator Top()
			{
				yield return this.Wait(10);
				this.DirectedShots(-1.3125f, -0.4375f, -120f);
				yield return this.Wait(5);
				this.DirectedShots(-1.5f, -0.1875f, -90f);
				yield return this.Wait(5);
				this.DirectedShots(-1.75f, 0.25f, -60f);

				yield return this.Wait(35);
				this.DirectedShots(-1.3125f, -0.4375f, -120f);
				yield return this.Wait(5);
				this.DirectedShots(-1.5f, -0.1875f, -90f);
				yield return this.Wait(5);
				this.DirectedShots(-1.75f, 0.25f, -60f);
				yield break;
			}
		}


		public class ModifiedBulletKingDirectedFireRight : ModifiedBulletKingDirectedFire
		{
			protected override IEnumerator Top()
			{
				yield return this.Wait(10);
				this.DirectedShots(2.125f, 2.375f, 120f);
				yield return this.Wait(5);
				this.DirectedShots(2.1875f, 1.3125f, 90f);
				yield return this.Wait(5);
				this.DirectedShots(1.875f, 0.25f, 60f);
				yield return this.Wait(35);
				this.DirectedShots(2.125f, 2.375f, 120f);
				yield return this.Wait(5);
				this.DirectedShots(2.1875f, 1.3125f, 120f);
				yield return this.Wait(5);
				this.DirectedShots(1.875f, 0.25f, 60f);
				yield break;
			}
		}

		public abstract class ModifiedBulletKingDirectedFire : Script
		{	
			protected void DirectedShots(float x, float y, float direction)
			{
				direction -= 90f;
				base.Fire(new Offset(x, y, 0f, string.Empty, DirectionType.Absolute), new Direction(direction, DirectionType.Absolute, -1f), new Speed(11, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableDirectedfire.Name, false, false, false));
			}
		}


		public class ModifeidBulletKingSlam1 : Script
		{
			protected override IEnumerator Top()
			{
				this.EndOnBlank = true;
				float startAngle = this.RandomAngle();
				float delta = 15f;
				for (int i = 0; i < 24; i++)
				{
					float num = startAngle + (float)i * delta;
					this.Fire(new Offset(1f, 0f, num, string.Empty, DirectionType.Absolute), new Direction(num, DirectionType.Absolute, -1f), new Speed(5, SpeedType.Absolute), new ModifeidBulletKingSlam1.SpinningBullet(this.Position, num));
				}
			
				yield return this.Wait(90);
				yield break;
			}

			static ModifeidBulletKingSlam1()
			{
				ModifeidBulletKingSlam1.SpinningBulletSpinSpeed = 180f;
			}

			private const int NumBullets = 36;

			private const int NumHardBullets = 12;

			private const float RadiusAcceleration = 8f;

			private const float SpinAccelration = 10f;

			public static float SpinningBulletSpinSpeed;

			private const int Time = 180;
			public class SpinningBullet : Bullet
			{
				public SpinningBullet(Vector2 centerPoint, float startAngle) : base(StaticUndodgeableBulletEntries.undodgeableBulletKingSlam.Name)
				{
					this.centerPoint = centerPoint;
					this.startAngle = startAngle;
				}

				protected override IEnumerator Top()
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
						spinSpeed += 0.16666667f;
						spinAngle += spinSpeed / 60f;
						this.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}
				private Vector2 centerPoint;
				private float startAngle;
			}
		}



		public class ModifeidBulletKingCrazySpin1 : Script
		{
		
			protected override IEnumerator Top()
			{
				for (int i = 0; i < 29; i++)
				{
					for (int j = 0; j < 6; j++)
					{
						float num = (float)j * 60f + 37f * (float)i;
						this.Fire(new Offset(1.66f, 0f, num, string.Empty, DirectionType.Absolute), new Direction(num, DirectionType.Absolute, -1f), new Speed(7.33f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name, false, false, false));
					}
					yield return this.Wait(6);
				}
				for (int k = 0; k < 36; k++)
				{
					this.Fire(new Direction((float)k * 360f / 36f, DirectionType.Absolute, -1f), new Speed(7, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name, 7, 60));
					this.Fire(new Direction((float)k * 360f / 36f, DirectionType.Absolute, -1f), new Speed(6, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name, 7, 60));

				}
				yield break;
			}
			private const int NumWaves = 29;
			private const int NumBulletsPerWave = 6;
			private const float AngleDeltaEachWave = 37f;
			private const int NumBulletsFinalWave = 64;
		}
	}
}
