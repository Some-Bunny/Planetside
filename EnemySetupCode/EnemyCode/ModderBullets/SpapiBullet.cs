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
	public class SpapiBullet : AIActor
	{
		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("spapi_bullet", "SpecialAPI", 20, 22, new List<int> { 188, 189, 190, 191 }, new List<int> { 192, 193, 194, 195, 196 }, null, new SalamanderScript(), 2f);
		}
		public class SalamanderScript : Script 
		{
			public override IEnumerator Top() 
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));
				}
				float angleDelta = 60f;
				for (int i = 0; i < 6; i++)
				{
					base.Fire(new Offset(0f, -2.5f, -30f - (float)i * angleDelta, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new WallBullet());
				}

				yield break;
			}
		}

		public class WallBullet : Bullet
		{
			public WallBullet() : base("bigBullet", false, false, false)
			{
			}
			public override IEnumerator Top()
			{
				yield return this.Wait(60);
				base.ChangeSpeed(new Speed(16f, SpeedType.Absolute), 60);
				yield break;
			}
		}
	}
}








