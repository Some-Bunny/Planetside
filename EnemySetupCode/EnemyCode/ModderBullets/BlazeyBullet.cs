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
	public class BlazeyBullet : AIActor
	{

		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("blazey_bullet", "Blazeykat", 18, 20, new List<int> { 17, 18, 19, 20 }, new List<int> { 21, 22, 23, 24, 25 }, null, new SkellScript());
		}
		public class SkellScript : Script
		{
			protected override IEnumerator Top()
			{
				AkSoundEngine.PostEvent("Play_WPN_stickycrossbow_shot_01", this.BulletBank.aiActor.gameObject);
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));

				for (int i = 0; i <= 18; i++)
				{
					this.Fire(new Direction(i * 20, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new SkellBullet());
					yield return this.Wait(5);
				}
				yield break;
			}
		}


		public class SkellBullet : Bullet
		{
			public SkellBullet() : base("sweep", false, false, false)
			{

			}
		}
	}
}








