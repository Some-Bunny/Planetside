using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;

namespace Planetside
{
	public class PretzelBullet : AIActor
	{
		public static void Init()
		{
			var enm = EnemyToolbox.CreateNewBulletBankerEnemy("captainpretzel_bullet", "CaptainPretzel", 20, 20,new List<int> { 275, 272, 270, 269 }, new List<int> { 271, 274, 277, 276, 273 }, null, new SkellScript());
            enm.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));

        }
        public class SkellScript : Script 
		{
			public override IEnumerator Top()
			{
				float angle = base.AimDirection;
                base.Fire(new Direction(angle, DirectionType.Absolute, -1f), new Speed(11f, SpeedType.Absolute), new SplitterBullet(2, angle));

                yield break;
			}
		}
		public class SplitterBullet : Bullet
		{
			public int SplitsUntilAttack = 2;
			public float RedirectAngle;
			public SplitterBullet(int splits, float Redirect) : base("default", false, false, false)
			{
                SplitsUntilAttack = splits;
				RedirectAngle = Redirect;
            }
            public override IEnumerator Top()
            {
				if (SplitsUntilAttack != 2)
				{
                    yield return Wait(SplitsUntilAttack == 1 ? 12 : 6);
                    base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 12);
                    yield return Wait(12);
					base.ChangeDirection(new Brave.BulletScript.Direction(RedirectAngle));
                    base.ChangeSpeed(new Speed(11, SpeedType.Absolute), 12);

                }

                yield return Wait(12);
                base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 12);
                yield return Wait(12);

                if (SplitsUntilAttack == 0)
				{
                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = this.Projectile.transform.position,
                        startSize = 8,
                        rotation = 0,
                        startLifetime = 0.25f,
                        startColor = Color.red.WithAlpha(1)
                    });
                    base.ChangeDirection(new Brave.BulletScript.Direction(0, DirectionType.Aim));
                    base.ChangeSpeed(new Speed(25, SpeedType.Absolute), 30);

                }
                else
				{
                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = this.Projectile.transform.position,
                        startSize = 12,
                        rotation = 0,
                        startLifetime = 0.75f,
                        startColor = Color.red.WithAlpha(0.25f)
                    });
                    base.Fire(new Direction(90, DirectionType.Relative, -1f), new Speed(11f, SpeedType.Absolute), new SplitterBullet(SplitsUntilAttack - 1, RedirectAngle));
                    base.Fire(new Direction(-90, DirectionType.Relative, -1f), new Speed(11f, SpeedType.Absolute), new SplitterBullet(SplitsUntilAttack - 1, RedirectAngle));
					base.Vanish(true);
				}
                yield break;
            }
        }
	}
}








