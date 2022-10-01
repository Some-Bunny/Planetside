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
	public class NeighborinoBullet : AIActor
	{
		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("neighborino_bullet", "Neighborino", 20, 20, new List<int> { 121, 122, 123, 124 }, new List<int> { 125, 126, 127, 128, 129 }, null, new SkellScript(), 2f);
		}
		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("icicle"));
				for (int i = -7; i <= 3; i++)
				{
					base.Fire(new Direction((float)(i * 2), DirectionType.Aim, -1f), new Speed(11f - (float)Mathf.Abs(i) * 0.3f, SpeedType.Absolute), new Frost());

				}
				yield return Wait(30);
				for (int i = -3; i <= 7; i++)
				{
					base.Fire(new Direction((float)(i * 2), DirectionType.Aim, -1f), new Speed(11f - (float)Mathf.Abs(i) * 0.3f, SpeedType.Absolute), new Gunfire());

				}
				yield break;
			}
		}
		public class Frost : Bullet
		{
			public Frost() : base("icicle", false, false, false)
			{

			}
		}

		public class Gunfire : Bullet
		{
			public Gunfire() : base("frogger", false, false, false)
			{

			}
		}
	}
}








