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
	public class RetrashBullet : AIActor
	{
		public static void Init()
		{

			EnemyToolbox.CreateNewBulletBankerEnemy("retrash_bullet", "Retrash", 15, 15, new List<int> { 159, 160, 161, 162 }, new List<int> { 163, 164, 165, 166, 167 }, null, new SalamanderScript(), 2f);
		}
		public class SalamanderScript : Script 
		{
			protected override IEnumerator Top() 
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));
				}
				for (int i = -1; i < 2; i++)
				{
					base.Fire(new Direction(0 + (30 * i), DirectionType.Aim, -1f), new Speed(13f, SpeedType.Absolute), new WallBullet());

				}
				yield return Wait(20);
				base.Fire(new Direction(15, DirectionType.Aim, -1f), new Speed(13f, SpeedType.Absolute), new WallBullet());
				base.Fire(new Direction(-15, DirectionType.Aim, -1f), new Speed(13f, SpeedType.Absolute), new WallBullet());
				yield return Wait(20);
				for (int i = -1; i < 2; i++)
				{
					base.Fire(new Direction(0 + (30 * i), DirectionType.Aim, -1f), new Speed(13f, SpeedType.Absolute), new WallBullet());

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








