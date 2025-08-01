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


	public class GatlingGullChanges : OverrideBehavior
	{//ffca09398635467da3b1f4a54bcfda80 
		public override string OverrideAIActorGUID => "ec6b674e0acd4553b47ee94493d66422"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{

			//ToolsEnemy.DebugInformation(behaviorSpec);

			actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableDefault);

			GatlingGullWalkAndShoot GatlingGullWalkAndShoot1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as GatlingGullWalkAndShoot;
			GatlingGullWalkAndShoot1.OverrideBulletName = StaticBulletEntries.undodgeableDefault.Name;
			GatlingGullWalkAndShoot1.AttackCooldown = (GatlingGullWalkAndShoot1.AttackCooldown) + 1;
			GatlingGullWalkAndShoot1.AngleVariance = 36;

			GatlingGullFanSpray GatlingGullFanSpray1= behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as GatlingGullFanSpray;
			GatlingGullFanSpray1.OverrideBulletName = StaticBulletEntries.undodgeableDefault.Name;
			GatlingGullFanSpray1.AttackCooldown = (GatlingGullWalkAndShoot1.AttackCooldown) + 1.5f;
			GatlingGullFanSpray1.SprayAngle = 120;
			GatlingGullFanSpray1.SpraySpeed = 200;
			GatlingGullFanSpray1.SprayIterations = 5;

			ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifeidGatlingGullFanBursts1));
		
		}


		public class ModifeidGatlingGullFanBursts1 : Script
		{
			public override IEnumerator Top()
			{
				float startAngle = this.AimDirection - 84f;
				float deltaAngle = 7;
				this.BulletBank.aiAnimator.LockFacingDirection = true;
				this.BulletBank.aiAnimator.FacingDirection = this.AimDirection;
				for (int i = 0; i < 2; i++)
				{
					bool r = BraveUtility.RandomBool();
					int p = 0;
					float angle = startAngle;
					for (int j = 0; j < 24; j++)
					{
						this.Fire(new Direction(angle, DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new Bullet(r == true ? "defaultWithVfx" : StaticBulletEntries.undodgeableDefault.Name, false, false, false));
						angle += deltaAngle;
						p++;
						if (p > 8) { p = 0;r = !r; }

					}
					if (i < 1)
					{
						yield return this.Wait(90);
					}
				}
				yield return this.Wait(20);
				this.BulletBank.aiAnimator.LockFacingDirection = false;
				yield break;
			}
			private const int NumWaves = 2;

			private const int NumBulletsPerWave = 20;
			private const float WaveArcLength = 130f;
		}
	}
}
