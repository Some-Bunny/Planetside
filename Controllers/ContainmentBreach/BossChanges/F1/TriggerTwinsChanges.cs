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


	public class SmileyChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "ea40fcc863d34b0088f490f4e57f8913"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			//actor.MovementSpeed *= 2; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);

			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);

			/*
			ShootGunBehavior ShootGunBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootGunBehavior;
			ShootGunBehavior1.OverrideBulletName = StaticUndodgeableBulletEntries.undodgeableDefault.Name;

			ShootGunBehavior ShootGunBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootGunBehavior;
			ShootGunBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletBrosAngryAttack1));
			*/

			ShootGunBehavior ShootGunBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[5].Behavior as ShootGunBehavior;
			ShootGunBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(BulletBrosSweepAttack1));

			ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[6].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletBrosJumpBurst1));

			ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[7].Behavior as ShootBehavior;
			ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletBrosJumpBurst2));
			//shootWave.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedShootWave));
		}

		public class ModifiedBulletBrosJumpBurst2 : Script
		{
			public override IEnumerator Top()
			{
				float num = base.RandomAngle();
				for (int i = 0; i < 18; i++)
				{
					base.Fire(new Direction(base.SubdivideCircle(num, 12, i, 1f, false), DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new Bullet("jump", true, false, false));
				}
				for (int j = 0; j < 12; j++)
				{
					base.Fire(new Direction(base.SubdivideCircle(num, 12, j, 1f, false) + 15, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name, 12f, 75, -1, true));
				}
				return null;
			}

			private const int NumFastBullets = 18;

			private const int NumSlowBullets = 9;
		}


		public class ModifiedBulletBrosJumpBurst1 : Script
		{
			public override IEnumerator Top()
			{
				float num = base.RandomAngle();
				float num2 = 45f;
				for (int i = 0; i < 8; i++)
				{
					base.Fire(new Direction(num + (float)i * num2, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new Bullet("jump", true, false, false));
					base.Fire(new Direction((num + (float)i * num2)+22.5f, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name, true, false, false));

				}
				return null;
			}
			private const int NumBullets = 12;
		}

		public class BulletBrosSweepAttack1 : Script
		{
			public override IEnumerator Top()
			{
				float sign = 1f;
				if (this.BulletManager.PlayerVelocity() != Vector2.zero)
				{
					float a = this.AimDirection + 90f;
					float b = this.BulletManager.PlayerVelocity().ToAngle();
					if (BraveMathCollege.AbsAngleBetween(a, b) > 90f)
					{
						sign = -1f;
					}
				}
				for (int i = 0; i < 9; i++)
				{
					this.Fire(new Direction(this.SubdivideArc(-sign * 60f / 2f, sign * 60f, 9, i, false), DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name));
					yield return this.Wait(5);
				}
				yield break;
			}

			private const int NumBullets = 15;
			private const float ArcDegrees = 60f;
		}

	


	}
	public class ShadesChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "c00390483f394a849c36143eb878998f"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			//actor.MovementSpeed *= 2; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);

			//actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDonut);

			//BeholsterShootBehavior shootWave = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as BeholsterShootBehavior;
			//shootWave.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedShootWave));

			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableAngrybullet);


			ShootGunBehavior ShootGunBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootGunBehavior;
			ShootGunBehavior1.OverrideBulletName = StaticUndodgeableBulletEntries.undodgeableDefault.Name;

			ShootGunBehavior ShootGunBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootGunBehavior;
			ShootGunBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletBrosAngryAttack1));

			ShootGunBehavior ShootGunBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[5].Behavior as ShootGunBehavior;
			ShootGunBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBulletBrosTridentAttack1));
		}

		public class ModifiedBulletBrosTridentAttack1 : Script
		{
			public override IEnumerator Top()
			{
				for (int i = 0; i < 3; i++)
				{
					float aim = (i % 2 != 0) ? this.GetAimDirection(1f, 10f) : this.AimDirection;
					this.Fire(new Direction(aim - 15f, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name, false, false, false));
					this.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name, false, false, false));
					this.Fire(new Direction(aim + 15f, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name, false, false, false));
					yield return this.Wait(45);
				}
				yield break;
			}
		}

		public class ModifiedBulletBrosAngryAttack1 : Script
		{
			public override IEnumerator Top()
			{
				for (float num = -2f; num <= 2f; num += 1f)
				{
					this.Fire(new Direction(num * 20f, DirectionType.Aim, -1f), new Speed(12f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableAngrybullet.Name, false, false, false));
				}
				yield return this.Wait(40);
				float num2 = -1.5f;
				while ((double)num2 <= 1.5)
				{
					this.Fire(new Direction(num2 * 20f, DirectionType.Aim, -1f), new Speed(12f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableAngrybullet.Name, false, false, false));
					num2 += 1f;
				}
				yield return this.Wait(40);
				for (float num3 = -2f; num3 <= 2f; num3 += 0.5f)
				{
					this.Fire(new Direction(num3 * 30f, DirectionType.Aim, -1f), new Speed(12f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableDefault.Name, false, false, false));
				}
				yield break;
			}
		}
	}
}
