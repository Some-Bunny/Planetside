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
	public class KyleBullet : AIActor
	{
		public static void Init()
		{
			AIActor kyle = EnemyToolbox.CreateNewBulletBankerEnemy("kyle_bullet", "Kyle The Scientist", 16, 17, new List<int> {112, 113, 114, 115 }, new List<int> {116, 117, 118, 119, 120 }, null, null, 3f);
			GameObject m_CachedGunAttachPoint = kyle.transform.Find("baseShootpoint").gameObject;
			var bs = kyle.gameObject.GetComponent<BehaviorSpeculator>();
			bs.OtherBehaviors = new List<BehaviorBase>() {
				new CustomSpinBulletsBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					OverrideBulletName = "homing",
					NumBullets = 2,
					BulletMinRadius = 4.5f,
					BulletMaxRadius = 5,
					BulletCircleSpeed = 60,
					BulletsIgnoreTiles = true,
					RegenTimer = 0.5f,
					AmountOFLines = 5,
				}
			};
		}
	}
}








