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
	public class KingBullet : AIActor
	{
		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("king_bullet", "Round King", 16, 20,new List<int> { 102, 103, 104, 105 }, new List<int> { 106, 107, 108, 109, 110, 111 }, null, new SkellScript());
		}	
		public class SkellScript : Script 
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				float angle = base.RandomAngle();
				for (int i = 0; i < 20; i++)
                {
					base.Fire(new Direction(angle + (18 * i), DirectionType.Aim, -1f), new Speed(11f, SpeedType.Absolute), new WallBullet());
				}
				yield break;
			}
		}
		public class WallBullet : Bullet
		{
			public WallBullet() : base("default", false, false, false)
			{

			}
		}
	}
}








