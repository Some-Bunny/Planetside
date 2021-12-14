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


	public class TestOverrideBehavior : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "3a077fa5872d462196bb9a3cb1af02a3"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
			// In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

			//actor.MovementSpeed *= 2; // Doubles the enemy movement speed

			//healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

			//The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
			// Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
			//ToolsEnemy.DebugInformation(behaviorSpec);

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
			//actor.bulletBank.Bullets.Add(BulletBuilder.GetBulletEntryByName("HammerOfTheMoon"));



			// Next, we're going to change another few things on the ShootGunBehavior so that it has a custom BulletScript.
			// Makes it so the bullet kin will shoot our bullet script instead of his own gun shot.
			//shootGunBehavior.BulletScript = new CustomBulletScriptSelector(typeof(Death)); // Sets the bullet kin's bullet script to our custom bullet script.
			//behaviorSpec.aiActor.get
		}
		//public AIBulletBank.Entry ThesporeEntry;
		public AIBulletBank.Entry lol; 


		public class Bullettest : Script
		{
			protected override IEnumerator Top()
			{
				this.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(1f, SpeedType.Absolute), new SkellBullet());

				yield break;
			}
		}


		public class SkellBullet : Bullet
		{
			public SkellBullet() : base("big", false, false, false)
			{

			}

			protected override IEnumerator Top()
			{
				
				OtherTools.EasyTrailComponent yup = base.Projectile.gameObject.AddComponent<OtherTools.EasyTrailComponent>();
				//yup.StartPositionX = 40;
				//yup.StartPositionY = 23;
				yup.LifeTime = 2.5f;
				yup.StartWidth = 35;
				yup.StartColor = Color.white;
				yup.BaseColor = Color.white;
				yup.EndColor = Color.red;
				
				base.ChangeSpeed(new Speed(20f, SpeedType.Absolute), 60);
				yield break;
			}
		}
	}
}
