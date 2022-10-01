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
	public class HunterBullet : AIActor
	{
		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("hunter_bullet", "Bl4ckHunter", 18, 19, new List<int> { 93, 94, 95, 96 }, new List<int> { 97, 98, 99, 100, 101 }, null, new SalamanderScript(), 3f);
		}
		
		public class SalamanderScript : Script 
		{
			protected override IEnumerator Top() 
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				}
				for (int i = -4; i < 4; i++)
				{
					base.Fire(new Direction(0 + (10 * i), DirectionType.Aim, -1f), new Speed(12f, SpeedType.Absolute), new WallBullet());

				}

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
				for (int i = 0; i < 10; i++)
                {
					base.ChangeSpeed(new Speed((UnityEngine.Random.Range(7, 16)), SpeedType.Absolute), UnityEngine.Random.Range(20, 120));
					yield return Wait(UnityEngine.Random.Range(30, 240));
					base.ChangeSpeed(new Speed((UnityEngine.Random.Range(2, 7)), SpeedType.Absolute), UnityEngine.Random.Range(20, 120));
					yield return Wait(UnityEngine.Random.Range(30, 240));
				}

				yield break;
			}

		}
	}
}








