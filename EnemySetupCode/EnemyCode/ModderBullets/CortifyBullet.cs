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
	public class CortifyBullet : AIActor
	{

		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("cortify_bullet", "Cortify", 20, 20,new List<int> { 63, 64, 65, 66 }, new List<int> { 67, 68, 69, 70, 71, 72, 73 }, null, new SkellScript(), 2.1f);
		}

		public class SkellScript : Script 
		{
			public override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("icicle"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
				for (int e = 0; e < 4; e++)
                {
					float Aim = UnityEngine.Random.Range(-20f, 20);
					for (int j = 0; j < 7; j++)
					{
						base.Fire(new Direction(Aim, DirectionType.Aim, -1f), new Speed(5, SpeedType.Absolute), new SpearPart(new Vector2(1.3125f -(0.375f * j), 0),10, 30,j == 0 ? "icicle" : "spore2"));
					}
					yield return Wait(15);
				}


				yield break;
			}
		}


		public class SpearPart : Bullet
		{
			public SpearPart(Vector2 offset, int setupDelay, int setupTime, string BulletType) : base(BulletType, false, false, false)
			{
				this.m_offset = offset;
				this.m_setupDelay = setupDelay;
				this.m_setupTime = setupTime;
				this.bullettype = BulletType;
			}

			public override IEnumerator Top()
			{
				this.ManualControl = true;
				this.m_offset = this.m_offset.Rotate(this.Direction);
				for (int i = 0; i < 360; i++)
				{
					if (i == 20)
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 60);
					}
					if (i == 100)
					{
						base.ChangeSpeed(new Speed(20, SpeedType.Absolute), 45);
					}
					if (i > this.m_setupDelay && i < this.m_setupDelay + this.m_setupTime)
					{
						this.Position += this.m_offset / (float)this.m_setupTime;
					}
					this.Position += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
					yield return this.Wait(1);
				}
				this.Vanish(false);
				yield break;
			}
			private string bullettype;
			private Vector2 m_offset;
			private int m_setupDelay;
			private int m_setupTime;
		}
	}
}








