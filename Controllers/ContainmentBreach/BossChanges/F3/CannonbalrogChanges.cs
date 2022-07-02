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


	public class CannonbalrogChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "5e0af7f7d9de4755a68d2fd3bbc15df4"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			//actor.MovementSpeed *= 2; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);

			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableCannonBullet);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableSlam);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableGroundDefault);
			//UndodgeableCannonBullet

			GiantPowderSkullRollBehavior rollBehavior = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[4].Behavior as GiantPowderSkullRollBehavior;
			rollBehavior.collisionBulletScript = new CustomBulletScriptSelector(typeof(ModifiedGiantPowderSkullRollSlam1));
			rollBehavior.rollSpeed = 12;
			rollBehavior.numBounces = 4;


			GiantPowderSkullArmosBehavior armos = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as GiantPowderSkullArmosBehavior;
			armos.bulletScript = new CustomBulletScriptSelector(typeof(ModifiedGiantPowderSkullPound1));
			armos.AttackCooldown = 2;

			GiantPowderSkullMergoBehavior me3rgo = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[5].Behavior as GiantPowderSkullMergoBehavior;
			me3rgo.shootBulletScript = new CustomBulletScriptSelector(typeof(ModifiedGiantPowderSkullCannonVolley1));
			me3rgo.fireMainDistVariance = 0;
			me3rgo.fireMainMidTime = 1;
			me3rgo.fireMainDist = 7;
			me3rgo.fireTime = 9;
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


		public class ModifiedGiantPowderSkullCannonVolley1 : Script
		{
			protected override IEnumerator Top()
			{
				AIAnimator aiAnimator = this.BulletBank.aiAnimator;
				string name = "eyeflash";
				Vector2? position = new Vector2?(this.Position);
				aiAnimator.PlayVfx(name, null, null, position);
				yield return this.Wait(30);
				float angle = this.AimDirection;
				for (int i = 0; i < 1; i++)
				{
					this.Fire(new Direction(angle, DirectionType.Absolute, -1f), new Speed(14f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableCannonBullet.Name, false, false, false));
					this.Fire(new Direction(angle, DirectionType.Absolute, -1f), new Speed(14f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableCannonBullet.Name, 12, 60));
					yield return this.Wait(4);
				}
				yield break;
			}

			private const int NumBullets = 5;
			private const float HalfWidth = 4.5f;
		}

		public class ModifiedGiantPowderSkullPound1 : Script
		{
			protected override IEnumerator Top()
			{
				int num = BraveUtility.SequentialRandomRange(0, 4, ModifiedGiantPowderSkullPound1.s_lastPatternNum, null, true);
				ModifiedGiantPowderSkullPound1.s_lastPatternNum = num;
				switch (num)
				{
					case 0:
						{
							float num2 = base.AimDirection - 48f;
							for (int i = 0; i < 12; i++)
							{
								float num3 = num2 + (float)i * 9f;
								base.Fire(new Offset(new Vector2(1f, 0f), num3, string.Empty, DirectionType.Absolute), new Direction(num3, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableGroundDefault.Name, false, false, false));
							}
							break;
						}
					case 1:
						{
							float num2 = base.AimDirection - 50f + UnityEngine.Random.Range(-35f, 35f);
							for (int j = 0; j < 5; j++)
							{
								float num4 = num2 + (float)j * 20f;
								base.Fire(new Offset(new Vector2(1f, 0f), num4, string.Empty, DirectionType.Absolute), new Direction(num4, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableGroundDefault.Name, false, false, false));
								base.Fire(new Offset(new Vector2(1f, 0f), num4, string.Empty, DirectionType.Absolute), new Direction(num4+10, DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableGroundDefault.Name, false, false, false));

							}
							break;
						}
					case 2:
						{
							float num2 = base.RandomAngle();
							for (int k = 0; k < 8; k++)
							{
								float num5 = num2 + (float)k * 45f;
								base.Fire(new Offset(new Vector2(1f, 0f), num5, string.Empty, DirectionType.Absolute), new Direction(num5, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableGroundDefault.Name, false, false, false));
								base.Fire(new Offset(new Vector2(1f, 0f), num5, string.Empty, DirectionType.Absolute), new Direction(num5+22.5f, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableGroundDefault.Name, false, false, false));
							}
							break;
						}
					case 3:
						{
							float num2 = base.RandomAngle();
							for (int l = 0; l < 16; l++)
							{
								float num6 = num2 + (float)l * 22.5f;
								base.Fire(new Offset(new Vector2(1f, 0f), num6, string.Empty, DirectionType.Absolute), new Direction(num6, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableGroundDefault.Name, false, false, false));
								base.Fire(new Offset(new Vector2(1f, 0f), num6, string.Empty, DirectionType.Absolute), new Direction(num6+11.25f, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableGroundDefault.Name, false, false, false));

							}
							break;
						}
				}
				return null;
			}
			private const float WaveSeparation = 12f;
			private const float OffsetDist = 1f;
			private static int s_lastPatternNum;
		}

		public class ModifiedGiantPowderSkullRollSlam1 : Script
		{
			protected override IEnumerator Top()
			{
				AkSoundEngine.PostEvent("Play_BOSS_doormimic_blast_01", this.BulletBank.gameObject);
				float startDirection = 0f;
				for (int i = 0; i < 24; i++)
				{
					float num = startDirection + (float)(i * 15);
					this.Fire(new Offset(new Vector2(1.5f, 0f), num, string.Empty, DirectionType.Absolute), new Direction(num, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableSlam.Name, 9f, 180, -1, false));
				}
				yield return this.Wait(12);
				startDirection = 7.5f;
				for (int k = 0; k < 24; k++)
				{
					float num3 = startDirection + (float)(k * 15);
					this.Fire(new Offset(new Vector2(1.5f, 0f), num3, string.Empty, DirectionType.Absolute), new Direction(num3, DirectionType.Absolute, -1f), new Speed(3f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableSlam.Name, 12f, 120, -1, false));
				}
				yield break;
			}
			private const float OffsetDist = 1.5f;
		}
	}
}
