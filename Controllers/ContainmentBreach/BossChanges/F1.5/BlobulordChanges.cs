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


	public class BlobulordChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "1b5810fafbec445d89921a4efb4e42b7"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			actor.MovementSpeed *= 0.75f; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);

			actor.bulletBank.Bullets.Add(StaticBulletEntries.UndodgeableSlam);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.UndodgeableSpew);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableLargeSpore);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableSmallSpore);


			ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBlobulordMoveSpray1));

			ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[5].Behavior as ShootBehavior;
			ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBlobulordSlam1));

			ShootBehavior ShootBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as ShootBehavior;
			ShootBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBlobulordBouncingRings1));

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

		public class ModifiedBlobulordBouncingRings1 : Script
		{
			public override IEnumerator Top()
			{
				for (int i = 0; i < 7; i++)
				{
					float aim = this.GetAimDirection((float)((UnityEngine.Random.value >= 0.4f) ? 0 : 1), 8f) + UnityEngine.Random.Range(-15f, 15f);
					for (int j = 0; j < 18; j++)
					{
						float angle = (float)j * 20f;
						Vector2 desiredOffset = BraveMathCollege.DegreesToVector(angle, 1.8f);
						this.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new ModifiedBlobulordBouncingRings1.BouncingRingBullet("bouncingRing", desiredOffset));
					}
					this.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new ModifiedBlobulordBouncingRings1.BouncingRingBullet("bouncingRing", new Vector2(-0.7f, 0.7f)));
					this.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new ModifiedBlobulordBouncingRings1.BouncingRingBullet("bouncingMouth", new Vector2(0f, 0.4f), true));
					this.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new ModifiedBlobulordBouncingRings1.BouncingRingBullet("bouncingRing", new Vector2(0.7f, 0.7f)));
					yield return this.Wait(50);
				}
				yield break;
			}

			private const int NumBlobs = 8;

			private const int NumBullets = 18;

			public class BouncingRingBullet : Bullet
			{
				public BouncingRingBullet(string name, Vector2 desiredOffset, bool Can = false) : base(name, false, false, false)
				{
					this.m_desiredOffset = desiredOffset;
					this.CanFire = Can;
				}
				public override IEnumerator Top()
				{
					Vector2 centerPoint = this.Position;
					Vector2 lowestOffset = BraveMathCollege.DegreesToVector(-90f, 1.5f);
					Vector2 currentOffset = Vector2.zero;
					float squishFactor = 1f;
					float verticalOffset = 0f;
					int unsquishIndex = 100;
					this.ManualControl = true;
					for (int i = 0; i < 300; i++)
					{
						if (i < 30)
						{
							currentOffset = Vector2.Lerp(Vector2.zero, this.m_desiredOffset, (float)i / 30f);
						}
						verticalOffset = (Mathf.Abs(Mathf.Cos((float)i / 90f * 3.1415927f)) - 1f) * 2.5f;
						if (unsquishIndex <= 10)
						{
							squishFactor = Mathf.Abs(Mathf.SmoothStep(0.6f, 1f, (float)unsquishIndex / 10f));
							unsquishIndex++;
						}
						Vector2 relativeOffset = currentOffset - lowestOffset;
						Vector2 squishedOffset = lowestOffset + relativeOffset.Scale(1f, squishFactor);
						this.UpdateVelocity();
						centerPoint += this.Velocity / 60f;
						this.Position = centerPoint + squishedOffset + new Vector2(0f, verticalOffset);
						if (i % 90 == 45)
						{
							for (int j = 1; j <= 10; j++)
							{
								squishFactor = Mathf.Abs(Mathf.SmoothStep(1f, 0.5f, (float)j / 10f));
								relativeOffset = currentOffset - lowestOffset;
								squishedOffset = lowestOffset + relativeOffset.Scale(1f, squishFactor);
								centerPoint += 0.333f * this.Velocity / 60f;
								this.Position = centerPoint + squishedOffset + new Vector2(0f, verticalOffset);
								if (CanFire == true && UnityEngine.Random.value < 0.33f) { this.Fire(new Direction(this.RandomAngle(), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(0.6f, 2.4f), SpeedType.Absolute), new Spore()); }

								yield return this.Wait(1);
							}
							unsquishIndex = 1;
						}
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}
				private Vector2 m_desiredOffset;
				private bool CanFire;

			}
			public class Spore : Bullet
			{
				public Spore() : base(UnityEngine.Random.value > 0.33f ? StaticBulletEntries.undodgeableSmallSpore.Name : StaticBulletEntries.undodgeableLargeSpore.Name, false, false, false) { }
				public override IEnumerator Top()
				{
					yield return this.Wait(30);
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), UnityEngine.Random.Range(60, 120));
					yield return this.Wait(UnityEngine.Random.Range(360, 660));
					base.Vanish(false);
					yield break;
				}
			}
		}


		public class ModifiedBlobulordSlam1 : Script
		{
			public override IEnumerator Top()
			{
				for (int i = 0; i < 3; i++)
				{
					float num = this.RandomAngle();
					for (int j = 0; j < 16; j++)
					{
						float num2 = num + (float)j * 22.5f;
						this.Fire(new Offset(2f, 0f, num2, string.Empty, DirectionType.Absolute), new Direction(num2, DirectionType.Absolute, -1f), new Speed(0.7f, SpeedType.Absolute), new ModifiedBlobulordSlam1.SlamBullet(i*30, 11));
						this.Fire(new Offset(2f, 0f, num2, string.Empty, DirectionType.Absolute), new Direction(num2 +11.25f, DirectionType.Absolute, -1f), new Speed(0.7f, SpeedType.Absolute), new ModifiedBlobulordSlam1.SlamBullet((i*30)+30, 16));
					}
				}
				yield return this.Wait(80);
				yield break;
			}

			private const int NumBullets = 32;

			private const int NumWaves = 4;

			public class SlamBullet : Bullet
			{
				public SlamBullet(int spawnDelay, float speed) : base(StaticBulletEntries.UndodgeableSlam.Name, false, false, false)
				{
					this.m_spawnDelay = spawnDelay;
					this.m_speed = speed;
				}

				public override IEnumerator Top()
				{
					int slowTime = this.m_spawnDelay;
					int i = 0;
					for (; ; )
					{
						yield return this.Wait(1);
						if (i == slowTime)
						{
							this.ChangeSpeed(new Speed(m_speed, SpeedType.Absolute), 60);
						}
						i++;
					}
				}
				private float m_speed;

				private int m_spawnDelay;
			}
		}


		public class ModifiedBlobulordMoveSpray1 : Script
		{
			public override IEnumerator Top()
			{
				int i = 0;
				while ((float)i < 64f)
				{
					this.Fire(new Direction(UnityEngine.Random.Range(-180, 180), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(6f, 9), SpeedType.Absolute), new Spore());
					yield return this.Wait(2);
					i++;
				}
				yield break;
			}
			public class Spore : Bullet
			{
				public Spore() : base(StaticBulletEntries.UndodgeableSpew.Name, false, false, false) { }
				public override IEnumerator Top()
				{
					yield return this.Wait(30);
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), UnityEngine.Random.Range(60, 120));
					yield return this.Wait(450);
					base.Vanish(false);
					yield break;
				}
			}
			private const float NumBullets = 30f;

			private const float ArcDegrees = 150f;
		}
	}
}
