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
	public class GlaurBullet : AIActor
	{

		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("glaurung_bullet", "Glaurung", 20, 16,new List<int> { 74, 75, 76, 77 }, new List<int> { 78, 79, 80, 81, 82, 83 }, null, new SkellScript(), 2f);
		}

		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				for (int j = 0; j < 8; j++)
				{
					float Aim = UnityEngine.Random.Range(-25f, 25);
					base.Fire(new Direction(Aim, DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(3f, 7f), SpeedType.Absolute), new Flames());
				}
				yield break;
			}
		}


		public class Flames : Bullet
		{
			public Flames() : base("frogger", false, false, false)
			{

			}

			protected override IEnumerator Top()
			{
				float speed = base.Speed;
				base.ChangeSpeed(new Speed(speed*2f, SpeedType.Absolute), 120);
				yield break;
			}
		}
	}
}








