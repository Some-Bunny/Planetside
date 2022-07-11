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


	public class AdvancedDragunKnifeChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "2e6223e42e574775b56c6349921f42cb"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".


			healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);
			//actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableOldKingHomingRingBulletSoundless);

			ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifeidDraGunKnifeDaggers1));
			ShootBehavior1.Cooldown *= 2f;
			//actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDonut);

			//BeholsterShootBehavior shootWave = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as BeholsterShootBehavior;
			//shootWave.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedShootWave));

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


		public class ModifeidDraGunKnifeDaggers1 : Script
		{
				public ModifeidDraGunKnifeDaggers1()
			{
				this.m_reticles = new List<LineReticleController>();
			}

			protected override IEnumerator Top()
			{
				float[] angles = new float[12];
				CellArea area = this.BulletBank.aiActor.ParentRoom.area;
				Vector2 roomLowerLeft = area.UnitBottomLeft + new Vector2(0f, 19f);
				Vector2 roomUpperRight = roomLowerLeft + new Vector2(36f, 14f);
				DraGunKnifeController knifeController = this.BulletBank.GetComponent<DraGunKnifeController>();
				for (int i = 0; i < 1; i++)
				{
					for (int j = 0; j < 7; j++)
					{
						float num = this.AimDirection;
						float d = 0.7f;
						if (j != 6)
						{
							int num2 = j / 2;
							bool flag = j % 2 == 1;
							Vector2 vector = IntVector2.CardinalsAndOrdinals[num2].ToVector2();
							float d2 = (!flag) ? 7f : 8.15f;
							Vector2 vector2 = this.BulletManager.PlayerPosition();
							Vector2 a = vector.normalized * d2;
							vector2 += a * d;
							Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2, this.BulletManager.PlayerVelocity(), this.Position, 60f);
							num = (predictedPosition - this.Position).ToAngle();
						}
						for (int k = 0; k < j; k++)
						{
							if (!float.IsNaN(angles[k]) && BraveMathCollege.AbsAngleBetween(angles[k], num) < 3f)
							{
								num = float.NaN;
							}
						}
						angles[j] = num;
						if (!float.IsNaN(angles[j]))
						{
							float num3 = 40f;
							Vector2 zero = Vector2.zero;
							if (BraveMathCollege.LineSegmentRectangleIntersection(this.Position, this.Position + BraveMathCollege.DegreesToVector(num, 60f), roomLowerLeft, roomUpperRight, ref zero))
							{
								num3 = (zero - this.Position).magnitude;
							}
							GameObject gameObject = SpawnManager.SpawnVFX(knifeController.ReticleQuad, false);
							LineReticleController component = gameObject.GetComponent<LineReticleController>();
							component.Init(new Vector2(this.Position.x, this.Position.y) + BraveMathCollege.DegreesToVector(num, 2f), Quaternion.Euler(0f, 0f, num), num3 - 3f);
							this.m_reticles.Add(component);
						}
					}
					yield return this.Wait(37);
					this.CleanupReticles();
					yield return this.Wait(5);
					for (int l = 0; l < 12; l++)
					{
						if (!float.IsNaN(angles[l]))
						{
							this.Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(60f, SpeedType.Absolute), new Bullet("dagger", true, false, false));
						}
					}
				}
				yield break;
			}

			public override void OnForceEnded()
			{
				this.CleanupReticles();
			}

			public Vector2 GetPredictedTargetPosition(float leadAmount, float speed, float fireDelay)
			{
				Vector2 vector = this.BulletManager.PlayerPosition();
				Vector2 a = this.BulletManager.PlayerVelocity();
				vector += a * fireDelay;
				return BraveMathCollege.GetPredictedPosition(vector, this.BulletManager.PlayerVelocity(), base.Position, speed);
			}

			private void CleanupReticles()
			{
				for (int i = 0; i < this.m_reticles.Count; i++)
				{
					this.m_reticles[i].Cleanup();
				}
				this.m_reticles.Clear();
			}

			// Token: 0x04000564 RID: 1380
			private const int NumWaves = 1;

			// Token: 0x04000565 RID: 1381
			private const int NumDaggersPerWave = 7;

			// Token: 0x04000566 RID: 1382
			private const int AttackDelay = 42;

			// Token: 0x04000567 RID: 1383
			private const float DaggerSpeed = 60f;

			// Token: 0x04000568 RID: 1384
			private List<LineReticleController> m_reticles;
		}
	}
}
