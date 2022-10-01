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
	public class BotBullet : AIActor
	{

		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("bot_bullet", "Not A Bot", 16, 16, new List<int> { 35, 36, 37, 38 }, new List<int> { 39, 40, 41, 42, 43 }, null, new SkellScript(), 3f);
		}

		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));
				float aim = base.AimDirection;
				for (int i = 0; i < 20; i++)
				{
					base.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(15, SpeedType.Absolute), new WallBullet());
					yield return this.Wait(3);
				}
				yield break;
			}
		}


		public class WallBullet : Bullet
		{
			public WallBullet() : base("directedfire", false, false, false)
			{

			}
		}
	}
}








