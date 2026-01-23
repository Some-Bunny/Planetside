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
using Alexandria;
using static Planetside.NeighborinoBullet;

namespace Planetside
{
	public class MarcyBullet : AIActor
	{
		public static void Init()
		{
			var enemy = EnemyToolbox.CreateNewBulletBankerEnemy("marcy_bullet", "Rat Queen Marcy", 16, 16, 
				new List<int> { 256, 259, 253, 260 }, 
				new List<int> { 255, 254, 257, 258 }, null, new SkellScript(), 2.5f);
			enemy.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("cheeseWheel"));
            enemy.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("cheese"));

        }
        public class SkellScript : Script 
		{
			public override IEnumerator Top() 
			{
                base.PostWwiseEvent("Play_BOSS_Rat_Cheese_Summon_01", null);
                base.Fire(new Direction(0, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new cHHEESE());
                yield break;
			}
		}

        public class cHHEESE : Bullet
        {
            public cHHEESE() : base("cheeseWheel", false, false, false)
            {

            }
  
            public override IEnumerator Top()
            {
                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = this.Projectile.transform.position,
                    startSize = 12,
                    rotation = 0,
                    startLifetime = 0.5f,
                    startColor = Color.red.WithAlpha(0.4f)
                });

                yield return this.Wait(30);
                
                //base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 30);
                
                
                this.Projectile.spriteAnimator.Play("cheese_wheel_burst");
                this.Projectile.ImmuneToSustainedBlanks = true;
                yield return base.Wait(45);
                this.Projectile.Ramp(-1.5f, 100f);
                yield return base.Wait(80);
                base.PostWwiseEvent("Play_BOSS_Rat_Cheese_Burst_01", null);
                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = this.Projectile.transform.position,
                    startSize = 16,
                    rotation = 0,
                    startLifetime = 0.25f,
                    startColor = Color.red.WithAlpha(0.25f)
                });
                for (int i = 0; i < 18; i++)
                {
                    Bullet bullet = new Bullet("cheese", true, false, false);
                    base.Fire(new Direction(UnityEngine.Random.Range(-20, 20), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(8f, 18f), SpeedType.Absolute), bullet);
                    bullet.Projectile.ImmuneToSustainedBlanks = true;
                }
                for (int i = 0; i < 12; i++)
                {
                    Bullet bullet = new Bullet("cheese", true, false, false);
                    base.Fire(new Direction(RandomAngle(), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(7f, 11f), SpeedType.Absolute), bullet);
                    bullet.Projectile.ImmuneToSustainedBlanks = true;
                }
                yield return base.Wait(25);
                base.Vanish(true);
                yield break;
            }
        }

    }
}








