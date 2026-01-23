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
	public class QueenBullet : AIActor
	{
		public static void Init()
		{
			var _ =EnemyToolbox.CreateNewBulletBankerEnemy("queen_bullet", "Round Queen", 16, 20,new List<int> { 287, 280, 282, 281 }, new List<int> { 286, 279, 285, 283, 284, 278 }, null, new SkellScript());
            _.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
        }
        public class SkellScript : Script 
		{
			public override IEnumerator Top()
			{
				float angle = base.RandomAngle();
				for (int i = 0; i < 36; i++)
                {
					base.Fire(new Direction(angle + (10 * i), DirectionType.Aim, -1f), new Speed(12f, SpeedType.Absolute), new WallBullet());
				}
				yield break;
			}
		}
		public class WallBullet : Bullet
		{
			public WallBullet() : base("default", false, false, false)
			{

			}
            public override IEnumerator Top()
            {
                base.ChangeSpeed(new Speed(6, SpeedType.Absolute), 90);
                yield break;
            }
        }
	}
}








