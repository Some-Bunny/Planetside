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


	public class BeholsterChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "4b992de5b4274168a8878ef9bf7ea36b"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			//actor.MovementSpeed *= 2; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);

			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDonut);

			BeholsterShootBehavior shootWave = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as BeholsterShootBehavior;
			shootWave.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedShootWave));

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


		public class ModifiedShootWave : Script
		{
			public override IEnumerator Top()
			{
				int q = -1;
				bool Flip = true;
				for (int i = -28; i <= 28; i++)
				{
					q++;
					if (q > 2) { q = 0; Flip = !Flip; }
					this.Fire(new Direction((float)(i * 6), DirectionType.Aim, -1f), new Speed(6.5f, SpeedType.Absolute), new Bullet(Flip == true ? StaticUndodgeableBulletEntries.undodgeableDonut.Name : "donut", false, false, false));
				}	
				q = -1;
				Flip = true;
				yield return this.Wait(75);
				for (int i = -28; i <= 28; i++)
				{
					q++;
					if (q > 2) { q = 0; Flip = !Flip; }
					this.Fire(new Direction((float)(i * 5), DirectionType.Aim, -1f), new Speed(6.5f, SpeedType.Absolute), new Bullet(Flip == true ? StaticUndodgeableBulletEntries.undodgeableDonut.Name : "donut", false, false, false));
				}
				yield return this.Wait(75);
				for (int i = 0; i <= 12; i++)
				{
					this.Fire(new Direction((float)(i * 30f), DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableDonut.Name, false, false, false));
				}
				yield return this.Wait(20);
				for (int i = 0; i <= 12; i++)
				{
					this.Fire(new Direction((float)(i * 30f) + 15, DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.undodgeableDonut.Name, false, false, false));
				}
				yield break;
			}
		}
	}
}
