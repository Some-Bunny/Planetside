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
using static Planetside.WallmongerChanges;

namespace Planetside
{
	public class JuneBullet : AIActor
	{

		public static void Init()
		{
			var ae = EnemyToolbox.CreateNewBulletBankerEnemy("june_bullet", "June", 24, 20, new List<int> { 17, 18, 19, 20 }, new List<int> { 21, 22, 23, 24, 25 }, null, new ArrowLad());
            ae.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6868795625bd46f3ae3e4377adce288b").bulletBank.GetBullet("dagger"));

        }


        public class ArrowLad : Script
        {
            public override IEnumerator Top()
            {
                int randomizer = UnityEngine.Random.Range(3, 11);
                List<int> r = new List<int>()
                {
                    randomizer - 1,
                    randomizer,
                    randomizer + 1,
                };
                for (int i = 0; i < 13; i++)
                {
                    if (!r.Contains(i))
                    {
                        base.Fire(new Direction((6 * i)-42, DirectionType.Aim, -1f), new Speed(5.75f, SpeedType.Absolute), new SpeedChangingBullet("dagger", 15, 120));
                    }
                }
                yield break;
            }
        }       
	}
}








