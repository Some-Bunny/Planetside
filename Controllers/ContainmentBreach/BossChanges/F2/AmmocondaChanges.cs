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


	public class AmmocondaChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "da797878d215453abba824ff902e21b4"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			actor.MovementSpeed *= 0.6f; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);

			actor.bulletBank.Bullets.Add(StaticBulletEntries.UndodgeableAmmocondaBigBullet);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableBigBullet);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableSmallSpore);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.UndodgeableRandomBullet);

			BashelliskShootBehavior wavyBullets = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as BashelliskShootBehavior;
			wavyBullets.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBashelliskSideWave1));
			wavyBullets.SegmentDelay = 0.05f;
			wavyBullets.SegmentPercentage = 0.4f;


			BashelliskShootBehavior Circles = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as BashelliskShootBehavior;
			Circles.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedCircle));
			Circles.Cooldown *= 1.25f;
			Circles.SegmentDelay = 1f;
			Circles.SegmentPercentage = 0.25f;

			BashelliskShootBehavior Random = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[5].Behavior as BashelliskShootBehavior;
			Random.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedBashelliskRandomShots1));
			Random.Cooldown *= 1.25f;
			Random.SegmentDelay = 0.33f;
			Random.SegmentPercentage = 0.5f;

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


		public class ModifiedBashelliskRandomShots1 : Script
		{
			public ModifiedBashelliskRandomShots1()
			{
				this.NumBullets = 3;
				this.BulletSpeed = 8.5f;
			}
			public override IEnumerator Top()
			{
				for (int i = 0; i < this.NumBullets; i++)
				{
					this.Fire(new Direction(this.GetAimDirection(1f, this.BulletSpeed) + (float)UnityEngine.Random.Range(-15f, 15f), DirectionType.Absolute, -1f), new Speed(this.BulletSpeed, SpeedType.Absolute), new Bullet(StaticBulletEntries.UndodgeableRandomBullet.Name, false, false, false));
					yield return this.Wait(6);
				}
				yield break;
			}
			public int NumBullets;
			public float BulletSpeed;
		}
		public class ModifiedBashelliskSideWave1 : Script
		{
			public override IEnumerator Top()
			{
				base.Fire(new Direction(-90f, DirectionType.Relative, -1f), new Speed(8f, SpeedType.Absolute), new ModifiedBashelliskSideWave1.WaveBullet());
				base.Fire(new Direction(-180f, DirectionType.Relative, -1f), new Speed(8f, SpeedType.Absolute), new ModifiedBashelliskSideWave1.WaveBullet());
				base.Fire(new Direction(-0f, DirectionType.Relative, -1f), new Speed(8f, SpeedType.Absolute), new ModifiedBashelliskSideWave1.WaveBullet());
				base.Fire(new Direction(90f, DirectionType.Relative, -1f), new Speed(8f, SpeedType.Absolute), new ModifiedBashelliskSideWave1.WaveBullet());
				return null;
			}

			public class WaveBullet : Bullet
			{
				public WaveBullet() : base(StaticBulletEntries.UndodgeableAmmocondaBigBullet.Name, false, false, false)
				{
				}

				public override IEnumerator Top()
				{
					yield return this.Wait(60);
					for (int i = 0; i < 3; i++)
					{
						this.ChangeSpeed(new Speed(-2f, SpeedType.Absolute), 30);
						yield return this.Wait(70);
						this.ChangeSpeed(new Speed(7f, SpeedType.Absolute), 30);
						yield return this.Wait(70);
					}
					this.Vanish(false);
					yield break;
				}
			}
		}

		public class ModifiedCircle: Script
		{
			public override IEnumerator Top()
			{
				bool LeftOrRight = (UnityEngine.Random.value > 0.5f) ? false : true;
				float RNGSPIN = LeftOrRight == true ? 18 : -18;
				float OffsetF = UnityEngine.Random.Range(0, 90);
				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				for (int e = 0; e < 4; e++)
				{
					base.Fire(Offset.OverridePosition(base.BulletBank.aiActor.sprite.WorldCenter), new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedCircle.RotatedBulletBasic(RNGSPIN, 0, 0, StaticBulletEntries.undodgeableBigBullet.Name, (e * 90) + OffsetF, 0.133f));
				}
				return null;
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
						if (i % 3 == 0)
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
					yield return this.Wait(30);
					base.Vanish(false);
					yield break;
				}
			}
		}
	}
}
