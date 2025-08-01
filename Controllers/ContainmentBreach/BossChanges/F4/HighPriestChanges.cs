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


	public class HighPriestChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "6c43fddfd401456c916089fdd1c99b1c"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{

			//ToolsEnemy.DebugInformation(behaviorSpec);

			actor.bulletBank.Bullets.Add(StaticBulletEntries.UndodgeableSweep);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.UndodgeableMergoWave);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.UndodgeableCross);

			ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as ShootBehavior;
			ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedHighPriestSweepAttacks1Left));

			ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootBehavior;
			ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedHighPriestSweepAttacks1Right));

			ShootBehavior ShootBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
			ShootBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedHighPriestSweepAttacks1Both));



			ShootBehavior ShootBehavior4 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[5].Behavior as ShootBehavior;
			ShootBehavior4.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedHighPriestCrossSprinkler1));


			HighPriestMergoBehavior HighPriestMergoBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[7].Behavior as HighPriestMergoBehavior;
			HighPriestMergoBehavior1.shootBulletScript = new CustomBulletScriptSelector(typeof(ModifiedHighPriestMergoWave1));
			HighPriestMergoBehavior1.fireMainDist = 9;
			HighPriestMergoBehavior1.fireMainDistVariance = 2;
			HighPriestMergoBehavior1.fireMainMidTime = 1f;
			HighPriestMergoBehavior1.fireWallMidTime = 2f;
		}



		public class ModifiedHighPriestMergoWave1 : Script
		{
			public override IEnumerator Top()
			{
				float startAngle = -60f;
				float deltaAngle = 30;
				AIAnimator aiAnimator = this.BulletBank.aiAnimator;
				//string name = "mergo";
				Vector2? position = new Vector2?(this.Position);
				StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(position.Value);
				yield return this.Wait(60);
				for (int i = 0; i < 12; i++)
				{
					this.Fire(new Direction(startAngle + (float)i * deltaAngle+3, DirectionType.Aim, -1f), new Speed(7f, SpeedType.Absolute), new SpeedChangingBullet(StaticBulletEntries.UndodgeableMergoWave.Name, 9, 45));
					this.Fire(new Direction(startAngle + (float)i * deltaAngle-3, DirectionType.Aim, -1f), new Speed(7f, SpeedType.Absolute), new SpeedChangingBullet(StaticBulletEntries.UndodgeableMergoWave.Name, 9, 45));
					this.Fire(new Direction(startAngle + (float)i * deltaAngle, DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new SpeedChangingBullet(StaticBulletEntries.UndodgeableMergoWave.Name, 9, 45));

					this.Fire(new Direction((startAngle + (float)i * deltaAngle) + 15, DirectionType.Aim, -1f), new Speed(4f, SpeedType.Absolute), new SpeedChangingBullet(StaticBulletEntries.UndodgeableMergoWave.Name, 12, 120));
					this.Fire(new Direction((startAngle + (float)i * deltaAngle) + 21, DirectionType.Aim, -1f), new Speed(4f, SpeedType.Absolute), new SpeedChangingBullet(StaticBulletEntries.UndodgeableMergoWave.Name, 12, 120));
					this.Fire(new Direction((startAngle + (float)i * deltaAngle) + 18, DirectionType.Aim, -1f), new Speed(5f, SpeedType.Absolute), new SpeedChangingBullet(StaticBulletEntries.UndodgeableMergoWave.Name, 12, 120));
				}
				yield break;
			}
			private const int NumBullets = 15;
			private const float Angle = 120f;
		}




		public class ModifiedHighPriestCrossSprinkler1 : Script
		{
			public override IEnumerator Top()
			{
				for (int i = 0; i < 105; i++)
				{
					float d = (float)i / 105f;
					if (i % 3==0)
                    {
						this.Fire(new Offset("left hand"), new Direction(-65f - d * 230f * 3.5f, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new Bullet(StaticBulletEntries.UndodgeableCross.Name, false, false, false));
						this.Fire(new Offset("right hand"), new Direction(-65f - d * 230f * 3.5f, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new Bullet(StaticBulletEntries.UndodgeableCross.Name, false, false, false));

						this.Fire(new Offset("right hand"), new Direction(-115f + d * 230f * 3.5f, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new Bullet(StaticBulletEntries.UndodgeableCross.Name, false, false, false));
						this.Fire(new Offset("left hand"), new Direction(-115f + d * 230f * 3.5f, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new Bullet(StaticBulletEntries.UndodgeableCross.Name, false, false, false));
					}
					yield return this.Wait(2);
				}
				yield break;
			}
			private const int NumBullets = 105;
		}



		public class ModifiedHighPriestSweepAttacks1Both : ModifiedHighPriestSweepAttacks1
		{
			public ModifiedHighPriestSweepAttacks1Both() : base(true, true)
			{

			}
		}

		public class ModifiedHighPriestSweepAttacks1Left : ModifiedHighPriestSweepAttacks1
        {
			public ModifiedHighPriestSweepAttacks1Left() : base(true, false)
			{

			}
		}


		public class ModifiedHighPriestSweepAttacks1Right : ModifiedHighPriestSweepAttacks1
		{
			public ModifiedHighPriestSweepAttacks1Right() : base(false, true)
			{

			}
		}

		public abstract class ModifiedHighPriestSweepAttacks1 : Script
		{
			public ModifiedHighPriestSweepAttacks1(bool shootLeft, bool shootRight)
			{
				this.m_shootLeft = shootLeft;
				this.m_shootRight = shootRight;
			}

			public override IEnumerator Top()
			{
				float angleDelta = 9f;
				bool T = false;
				for (int i = 0; i < 11; i++)
				{
					if (this.m_shootLeft)
					{
						this.Fire(new Offset(0f, -2.5f, -30f - (float)i * angleDelta, string.Empty, DirectionType.Absolute), new Direction((float)((15 - i) * 5), DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedHighPriestSweepAttacks1.SweepBullet(i, T));
					}
					if (this.m_shootRight)
					{
						this.Fire(new Offset(0f, -2.5f, 30f + (float)i * angleDelta, string.Empty, DirectionType.Absolute), new Direction((float)((15 - i) * -5), DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new ModifiedHighPriestSweepAttacks1.SweepBullet(i, T));
					}
					T = !T;
					yield return this.Wait(1);
				}
				yield break;
			}

			private const int NumBullets = 15;

			private bool m_shootLeft;

			private bool m_shootRight;

			public class SweepBullet : Bullet
			{
				public SweepBullet(int delay, bool Speed) : base(StaticBulletEntries.UndodgeableSweep.Name, false, false, false)
				{
					this.m_delay = delay;
					S = Speed;
				}

				public override IEnumerator Top()
				{
					yield return this.Wait(45 - this.m_delay);
					
					if (S == true)
                    {
						this.Speed = 11f;
					}
					else 
					{
						this.Speed = 6.5f;
					}
					yield return this.Wait(330);
					this.Vanish(false);
					yield break;
				}
				private bool S;
				private int m_delay;
			}
		}

	}
}
