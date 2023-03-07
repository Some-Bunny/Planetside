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


	public class WallmongerChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "f3b04a067a65492f8b279130323b41f0"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			//actor.MovementSpeed *= 2; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);

			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBulletSoundless);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableFrogger);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableWallWave);

			//UndodgeableWallWave
			DemonWallSpewBehavior spew = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as DemonWallSpewBehavior;
			spew.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDemonWallSpew1));
			spew.goopRadius = 1.5f;


			ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[4].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDemonWallLeapLine1));
			ShootBehavior1.AttackCooldown *= 1.33f;

			ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
			ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedDemonWallBasicWaves1));
			ShootBehavior2.AttackCooldown = ShootBehavior2.AttackCooldown + 1.5f;
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


		public class ModifiedDemonWallBasicWaves1 : Script
		{
			public override IEnumerator Top()
			{
				int group = 1;
				for (int i = 0; i < 12; i++)
				{
					group = BraveUtility.SequentialRandomRange(0, ModifiedDemonWallBasicWaves1.shootPoints.Length, group, null, true);
					this.FireWave(BraveUtility.RandomElement<string>(ModifiedDemonWallBasicWaves1.shootPoints[group]));
					yield return this.Wait(25);
				}
				yield return this.Wait(30);

				
				for (int i = 0; i < 3; i++)
                {
					group = BraveUtility.SequentialRandomRange(0, ModifiedDemonWallBasicWaves1.shootPoints.Length, group, null, true);
					string st = BraveUtility.RandomElement<string>(ModifiedDemonWallBasicWaves1.shootPoints[group]);
					float aimDirection = base.GetAimDirection(st);

					for (int o = 0; o < 10; o++)
                    {
						base.Fire(new Offset(st), new Direction(aimDirection + (36*o), DirectionType.Absolute, -1f), new Speed(4f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableWallWave.Name, 8, 75));
						base.Fire(new Offset(st), new Direction(aimDirection + (36 * o) + 18, DirectionType.Absolute, -1f), new Speed(2f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableWallWave.Name, 11, 120));
					}
					yield return this.Wait(20);
				}


				yield break;
			}

			private void FireWave(string transform)
			{
				List<int> H = new List<int>()
				{	
					-1,
					 0,
					 1
				};
				bool l = UnityEngine.Random.value > 0.5f ? true : false;
				for (int i = -4; i < 5; i++)
				{
					base.Fire(new Offset(transform), new Direction(-90 + (6*i), DirectionType.Absolute, -1f), new Speed(5.5f, SpeedType.Absolute), new Bullet(H.Contains(i) == l? StaticUndodgeableBulletEntries.UndodgeableWallWave.Name : "wave", i != 3, false, false));
				}
			
			}


			static ModifiedDemonWallBasicWaves1()
			{
				ModifiedDemonWallBasicWaves1.shootPoints = new string[][]
				{
			new string[]
			{
				"sad bullet",
				"blobulon",
				"dopey bullet"
			},
			new string[]
			{
				"left eye",
				"right eye",
				"crashed bullet"
			},
			new string[]
			{
				"sideways bullet",
				"shotgun bullet",
				"cultist",
				"angry bullet"
			}
				};
			}

			public static string[][] shootPoints;

			public const int NumBursts = 10;
		}





		public class ModifiedDemonWallLeapLine1 : Script
		{
			public override IEnumerator Top()
			{
				float num = 1f;
				int randomizer = UnityEngine.Random.Range(6, 18);
				List<int> r = new List<int>()
				{
					randomizer - 3,
					randomizer - 2,
					randomizer - 1,
					randomizer,
					randomizer + 1,
					randomizer + 2,
					randomizer + 3,
				};

				for (int i = 0; i < 24; i++)
				{
					base.Fire(new Offset(-11.5f + (float)i * num, 0f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(5.75f, SpeedType.Absolute), new ModifiedDemonWallLeapLine1.WaveBullet(r.Contains(i)));
				}
				return null;
			}

			private const int NumBullets = 24;

			private class WaveBullet : Bullet
			{
				public WaveBullet(bool r) : base(r == false ? StaticUndodgeableBulletEntries.undodgeableDefault.Name : "leap", false, false, false)
				{
				}

				public override IEnumerator Top()
				{
					this.ManualControl = true;
					Vector2 truePosition = this.Position;
					for (int i = 0; i < 600; i++)
					{
						this.UpdateVelocity();
						truePosition += this.Velocity / 60f;
						this.Position = truePosition + new Vector2(0f, Mathf.Sin((float)this.Tick / 60f / 0.75f * 3.1415927f) * 0.625f);
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}
				private const float SinPeriod = 0.75f;
				private const float SinMagnitude = 1.5f;
			}
		}


		public class ModifiedDemonWallSpew1 : Script
		{
			public override IEnumerator Top()
			{
				for (int i = 0; i < 4; i++)
				{
					this.StartTask(this.FireWall((float)((i % 2 != 0) ? 1 : -1)));
					this.StartTask(this.FireWaves((i + 1) % 2));
					yield return this.Wait(110);
				}
				yield break;
			}

			private IEnumerator FireWall(float sign)
			{
				for (int i = 0; i < 3; i++)
				{
					bool offset = i % 2 == 1;
					for (int j = 0; j < ((!offset) ? 12 : 11); j++)
					{
						this.Fire(new Offset(sign * this.SubdivideArc(2f, 9.5f, 12, j, offset), 0f, 0f, string.Empty, DirectionType.Absolute), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false));
					}
					yield return this.Wait(12);
				}
				yield break;
			}

			private IEnumerator FireWaves(int index)
			{
				for (int i = 0; i < 3; i++)
				{
					string transform = BraveUtility.RandomElement<string>(ModifiedDemonWallSpew1.shootPoints[index]);
					for (int j = 0; j < 5; j++)
					{
						this.Fire(new Offset(transform), new Direction(this.SubdivideArc(-115f, 50f, 5, j, false), DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new Bullet(j==2 ? StaticUndodgeableBulletEntries.UndodgeableFrogger.Name : "wave", j != 2, false, false));
					}
					float aimDirection = this.GetAimDirection(transform);
					if (UnityEngine.Random.value < 0.2f && BraveMathCollege.AbsAngleBetween(-90f, aimDirection) < 45f)
					{
						this.Fire(new Offset(transform), new Direction(aimDirection, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, true, false, false));
					}
					yield return this.Wait(40);
				}
				yield break;
			}
			static ModifiedDemonWallSpew1()
			{
				ModifiedDemonWallSpew1.shootPoints = new string[][]
				{
			new string[]
			{
				"sad bullet",
				"blobulon",
				"dopey bullet"
			},
			new string[]
			{
				"sideways bullet",
				"shotgun bullet",
				"cultist",
				"angry bullet"
			}
				};
			}
			public static string[][] shootPoints;
			private const int NumBullets = 12;
		}
	}
}
