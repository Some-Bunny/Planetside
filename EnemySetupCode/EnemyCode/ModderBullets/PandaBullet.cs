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
	public class PandaBullet : AIActor
	{
		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("panda_bullet", "Explosive Panda", 20, 19, new List<int> { 149, 150, 151, 152 }, new List<int> { 153, 154, 155, 156, 157, 158 }, null, new SkellScript(), 4f);
		}
		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("880bbe4ce1014740ba6b4e2ea521e49d").bulletBank.GetBullet("grenade"));
				}
				float airTime = base.BulletBank.GetBullet("grenade").BulletObject.GetComponent<ArcProjectile>().GetTimeInFlight();
				Vector2 vector = this.BulletManager.PlayerPosition();
				Bullet bullet2 = new Bullet("grenade", false, false, false);
				float direction2 = (vector - base.Position).ToAngle();
				base.Fire(new Direction(direction2, DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), bullet2);
				(bullet2.Projectile as ArcProjectile).AdjustSpeedToHit(vector);
				bullet2.Projectile.ImmuneToSustainedBlanks = true;
				yield break;
			}
		}
	}
}








