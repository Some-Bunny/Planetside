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


	public class GorgunChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "c367f00240a64d5d9f3c26484dc35833"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			//actor.MovementSpeed *= 2; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);

			//actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableDonut);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableDefault);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableDonut);

			/*
			SequentialAttackBehaviorGroup SequentialAttackBehaviorGroup = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as SequentialAttackBehaviorGroup;
			AttackBehaviorGroup groupNest = SequentialAttackBehaviorGroup.AttackBehaviors[0] as AttackBehaviorGroup;
			ShootBehavior uziOne = groupNest.AttackBehaviors[0].Behavior as ShootBehavior;
			uziOne.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMeduziUziFire1));

			ShootBehavior uziTwo = groupNest.AttackBehaviors[1].Behavior as ShootBehavior;
			uziTwo.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMeduziUziFire2));

			ShootBehavior uziThree = groupNest.AttackBehaviors[2].Behavior as ShootBehavior;
			uziThree.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMeduziUziFire3));
			*/
			/*

			myballs.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMeduziUziFire1)); //ModifiedMeduziUziFire1
			*/
			SpinAttackBehavior shootWave = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as SpinAttackBehavior;
			shootWave.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedSpin));


			ShootBehavior shootShootBehavior = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[5].Behavior as ShootBehavior;
			shootShootBehavior.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMeduziScream1));
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


		public class ModifiedMeduziUziFire1 : Script
		{
			public override IEnumerator Top()
			{
				Animation animation = this.BulletManager.GetUnityAnimation();
				AnimationClip clip = animation.GetClip("MeduziFireDown");
				for (int i = 0; i < 45; i++)
				{
					clip.SampleAnimation(animation.gameObject, (float)i / 45f);
					this.Fire(new Offset("left hand shoot point"), new Direction((float)UnityEngine.Random.Range(-7, 7), DirectionType.Relative, -1f), new Speed(11f, SpeedType.Absolute), null);
					this.Fire(new Offset("right hand shoot point"), new Direction((float)UnityEngine.Random.Range(-7, 7), DirectionType.Relative, -1f), new Speed(11f, SpeedType.Absolute), null);
					yield return this.Wait(1);
				}
				for (int i = 0; i < 24; i++)
                {
					this.Fire(new Direction(15*i, DirectionType.Relative, -1f), new Speed(8f, SpeedType.Absolute), new Bullet(StaticBulletEntries.undodgeableDefault.Name));
				}

				yield break;
			}
			private const int NumBullets = 60;
		}

		public class ModifiedMeduziUziFire2 : Script
		{
			public override IEnumerator Top()
			{
				Animation animation = this.BulletManager.GetUnityAnimation();
				AnimationClip clip = animation.GetClip("MeduziFireLeft");
				for (int i = 0; i < 45; i++)
				{
					clip.SampleAnimation(animation.gameObject, (float)i / 45f);
					this.Fire(new Offset("left hand shoot point"), new Direction((float)UnityEngine.Random.Range(-7, 7), DirectionType.Relative, -1f), new Speed(11f, SpeedType.Absolute), null);
					this.Fire(new Offset("right hand shoot point"), new Direction((float)UnityEngine.Random.Range(-7, 7), DirectionType.Relative, -1f), new Speed(11f, SpeedType.Absolute), null);
				

					yield return this.Wait(1);
				}
				for (int i = 0; i < 24; i++)
				{
					this.Fire(new Direction(15 * i, DirectionType.Relative, -1f), new Speed(8f, SpeedType.Absolute), new Bullet(StaticBulletEntries.undodgeableDefault.Name));
				}
				yield break;
			}
			private const int NumBullets = 60;
		}
		public class ModifiedMeduziUziFire3 : Script
		{
			public override IEnumerator Top()
			{
				Animation animation = this.BulletManager.GetUnityAnimation();
				AnimationClip clip = animation.GetClip("MeduziFireRight");
				for (int i = 0; i < 45; i++)
				{
					clip.SampleAnimation(animation.gameObject, (float)i / 45f);
					this.Fire(new Offset("left hand shoot point"), new Direction((float)UnityEngine.Random.Range(-7, 7), DirectionType.Relative, -1f), new Speed(11f, SpeedType.Absolute), null);
					this.Fire(new Offset("right hand shoot point"), new Direction((float)UnityEngine.Random.Range(-7, 7), DirectionType.Relative, -1f), new Speed(11f, SpeedType.Absolute), null);
			
					yield return this.Wait(1);
				}
				for (int i = 0; i < 24; i++)
				{
					this.Fire(new Direction(15 * i, DirectionType.Relative, -1f), new Speed(8f, SpeedType.Absolute), new Bullet(StaticBulletEntries.undodgeableDefault.Name));
				}
				yield break;
			}
			private const int NumBullets = 60;
		}


		public class ModifiedSpin : Script
		{
			public override IEnumerator Top()
			{
				float F = 11;
				for (int i = 0; i < 30; i++)
				{
					F -= 0.25f;
					for (int j = 0; j < 6; j++)
					{
						float num = (float)j * 60f + -37f * (float)i;
						this.Fire(new Offset(1.66f, 0f, num, string.Empty, DirectionType.Absolute), new Direction(num, DirectionType.Absolute, -1f), new Speed(F, SpeedType.Absolute), new Bullet(StaticBulletEntries.undodgeableDefault.Name, false, false, false));
					}
					yield return this.Wait(6);
				}
			
				yield break;
			}
		}

		public class ModifiedMeduziScream1 : Script
		{
			public override IEnumerator Top()
			{
				bool isCoop = GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER;
				SpeculativeRigidbody target = GameManager.Instance.PrimaryPlayer.specRigidbody;
				SpeculativeRigidbody target2 = (!isCoop) ? null : GameManager.Instance.SecondaryPlayer.specRigidbody;
				int numGaps = (!isCoop) ? 3 : 2;
				if (ModifiedMeduziScream1.s_gapAngles == null || ModifiedMeduziScream1.s_gapAngles.Length != numGaps)
				{
					ModifiedMeduziScream1.s_gapAngles = new float[numGaps];
				}
				this.EndOnBlank = true;
				this.m_goopDefinition = this.BulletBank.GetComponent<GoopDoer>().goopDefinition;
				float delta = 5.625f;
				float idealGapAngle = (target.GetUnitCenter(ColliderType.HitBox) - this.Position).ToAngle();
				float idealGapAngle2 = (!isCoop) ? 0f : (target2.GetUnitCenter(ColliderType.HitBox) - this.Position).ToAngle();
				for (int i = 0; i < 20; i++)
				{
					if (isCoop && numGaps > 1 && BraveMathCollege.AbsAngleBetween(idealGapAngle, idealGapAngle2) < 22.5f)
					{
						numGaps = 1;
					}
					if (isCoop && numGaps > 1)
					{
						ModifiedMeduziScream1.s_gapAngles[0] = idealGapAngle;
						ModifiedMeduziScream1.s_gapAngles[1] = idealGapAngle2;
					}
					else
					{
						for (int j = 0; j < numGaps; j++)
						{
							ModifiedMeduziScream1.s_gapAngles[j] = this.SubdivideCircle(idealGapAngle, numGaps, j, 1f, false);
						}
					}
					int skipCount = -1000;
					bool skipDirection = BraveUtility.RandomBool();
					//if (i % 2 == 0)
					{
						skipCount = 1;
					}
					for (int k = 0; k < 64; k++)
					{
						float num = this.SubdivideCircle(idealGapAngle, 64, k, 1f, false);
						float num2 = (numGaps != 1) ? BraveMathCollege.GetNearestAngle(num, ModifiedMeduziScream1.s_gapAngles) : ModifiedMeduziScream1.s_gapAngles[0];
						float num3 = BraveMathCollege.ClampAngle180(num2 - num);
						int num4 = Mathf.RoundToInt(Mathf.Abs(num3 / delta));
						bool H = false;
						if (num4 == skipCount && (num4 == 0 || num3 > 0f == skipDirection))
						{
							H = true;
							num4 = 100;
						}
						this.Fire(new Direction(num, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new ModifiedMeduziScream1.TimedBullet(num4, Mathf.Sign(num3), H == false ? "scream" : StaticBulletEntries.undodgeableDefault.Name));
					}
					yield return this.Wait(20);
					if (isCoop && numGaps > 1)
					{
						ModifiedMeduziScream1.s_gapAngles[0] = idealGapAngle;
						ModifiedMeduziScream1.s_gapAngles[1] = idealGapAngle2;
					}
					else
					{
						for (int l = 0; l < numGaps; l++)
						{
							ModifiedMeduziScream1.s_gapAngles[l] = this.SubdivideCircle(idealGapAngle, numGaps, l, 1f, false);
						}
					}
					if (!isCoop)
					{
						idealGapAngle = BraveMathCollege.GetNearestAngle(this.AimDirection, ModifiedMeduziScream1.s_gapAngles);
					}
					this.SafeUpdateAngle(ref idealGapAngle, target);
					if (isCoop && numGaps > 1)
					{
						this.SafeUpdateAngle(ref idealGapAngle2, target2);
					}
				}
				yield break;
			}

			private void SafeUpdateAngle(ref float idealGapAngle, SpeculativeRigidbody target)
			{
				bool flag = this.IsSafeAngle(idealGapAngle + 12f, target);
				bool flag2 = this.IsSafeAngle(idealGapAngle - 12f, target);
				if ((flag && flag2) || (!flag && !flag2))
				{
					idealGapAngle += BraveUtility.RandomSign() * 12f;
				}
				else
				{
					idealGapAngle += (float)((!flag) ? -1 : 1) * 12f;
				}
			}

			private bool IsSafeAngle(float angle, SpeculativeRigidbody target)
			{
				float magnitude = Vector2.Distance(target.GetUnitCenter(ColliderType.HitBox), base.Position);
				Vector2 position = base.Position + BraveMathCollege.DegreesToVector(angle, magnitude);
				return !GameManager.Instance.Dungeon.data.isWall((int)position.x, (int)position.y) && !DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.m_goopDefinition).IsPositionInGoop(position);
			}

			// Token: 0x0400089E RID: 2206
			private const int NumWaves = 16;

			// Token: 0x0400089F RID: 2207
			private const int NumBulletsPerWave = 64;

			// Token: 0x040008A0 RID: 2208
			private const int NumGaps = 3;

			// Token: 0x040008A1 RID: 2209
			private const int StepOpenTime = 14;

			// Token: 0x040008A2 RID: 2210
			private const int GapHalfWidth = 3;

			private const int GapHoldWaves = 6;

			private const float TurnDegPerWave = 12f;

			private static float[] s_gapAngles;

			private GoopDefinition m_goopDefinition;

			private class TimedBullet : Bullet
			{
				public TimedBullet(int bulletsFromSafeDir, float direction, string BulletType) : base(BulletType, false, false, false)
				{
					this.m_bulletsFromSafeDir = bulletsFromSafeDir;
					this.m_direction = direction;
				}

				public override IEnumerator Top()
				{
					if (this.m_bulletsFromSafeDir == 4)
					{
						yield return this.Wait(95);
						UVScrollTriggerableInitializer animator = this.Projectile.sprite.GetComponent<UVScrollTriggerableInitializer>();
						animator.TriggerAnimation();
						this.Projectile.Ramp(2f, 10f);
						yield return this.Wait(45);
						animator.ResetAnimation();
						yield return this.Wait(200);
						this.Vanish(false);
					}
					else if (this.m_bulletsFromSafeDir > 3)
					{
						//yield return this.Wait(420);
						//this.Vanish(false);
					}
					else
					{
						Vector2 origin = this.Position;
						int preDelay = 14 * this.m_bulletsFromSafeDir;
						if (preDelay > 0)
						{
							yield return this.Wait(preDelay);
						}
						float radius = Vector2.Distance(this.Position, origin);
						float angle = this.Direction;
						float deltaAngle = 0.4017857f;
						this.ManualControl = true;
						int moveTime = (3 - this.m_bulletsFromSafeDir + 1) * 14;
						for (int i = 0; i < moveTime; i++)
						{
							this.UpdateVelocity();
							radius += this.Speed / 60f;
							angle -= this.m_direction * deltaAngle;
							this.Position = origin + BraveMathCollege.DegreesToVector(angle, radius);
							yield return this.Wait(1);
						}
						this.ManualControl = false;
						this.Direction = angle;
						yield return this.Wait(1);
						yield return this.Wait(84);
						UVScrollTriggerableInitializer animator2 = this.Projectile.sprite.GetComponent<UVScrollTriggerableInitializer>();
						animator2.TriggerAnimation();
						radius = Vector2.Distance(this.Position, origin);
						this.ManualControl = true;
						moveTime = (3 - this.m_bulletsFromSafeDir + 1) * 14;
						for (int j = 0; j < moveTime; j++)
						{
							this.UpdateVelocity();
							radius += this.Speed / 60f;
							angle += this.m_direction * deltaAngle;
							this.Position = origin + BraveMathCollege.DegreesToVector(angle, radius);
							yield return this.Wait(1);
						}
						this.ManualControl = false;
						this.Direction = angle;
						yield return this.Wait(240);
						this.Vanish(true);
					}
					yield break;
				}

				// Token: 0x040008A7 RID: 2215
				private int m_bulletsFromSafeDir;

				// Token: 0x040008A8 RID: 2216
				private float m_direction;
			}
		}
	}
}
