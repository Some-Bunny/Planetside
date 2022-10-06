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
	public class BleakBullet : AIActor
	{

		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("bleak_bullet", "BleakBullets", 20, 20, new List<int> { 26, 27, 28, 29 }, new List<int> { 30, 31, 32, 33, 34 }, null, new SkellScript());
		}

		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				for (int i = 0; i < 5; i++)
				{
					base.PostWwiseEvent("Play_WPN_colt1851_shot_01", null);
					for (int a = 0; a <= 1; a++)
					{
						base.Fire(new Direction((-15f + (i * 3f)) + (a * (30 - (i * 6))), DirectionType.Aim, -1f), new Speed(11, SpeedType.Absolute), null);
					}
					
					yield return this.Wait(2);
				}
				base.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(12, SpeedType.Absolute), null);
                base.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(11, SpeedType.Absolute), null);
                base.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(10, SpeedType.Absolute), null);

                yield break;
			}
		}
		public class WallBullet : Bullet
		{
			public WallBullet() : base("default", false, false, false)
			{

			}
			protected override IEnumerator Top()
			{


				yield break;
			}

		}
	}
}








