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
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/";
			string folderName = "kyle";
			string idleFrameName = "kylebullet_idle_00";
			string deathFrameName = "kylebullet_die_00";

			string[] spritePaths = new string[]
			{
			TemplatePath+folderName+"/"+idleFrameName+"1.png",
			TemplatePath+folderName+"/"+idleFrameName+"2.png",
			TemplatePath+folderName+"/"+idleFrameName+"3.png",
			TemplatePath+folderName+"/"+idleFrameName+"4.png",

			TemplatePath+folderName+"/"+deathFrameName+"1.png",
			TemplatePath+folderName+"/"+deathFrameName+"2.png",
			TemplatePath+folderName+"/"+deathFrameName+"3.png",
			TemplatePath+folderName+"/"+deathFrameName+"4.png",
			TemplatePath+folderName+"/"+deathFrameName+"5.png",

			};
			AIActor kyle = EnemyToolbox.CreateNewBulletBankerEnemy("kyle_bullet", "Kyle The Scientist", 16, 17, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8 }, null, null, 3f);
			GameObject m_CachedGunAttachPoint = kyle.transform.Find("baseShootpoint").gameObject;
			var bs = kyle.gameObject.GetComponent<BehaviorSpeculator>();
			bs.OtherBehaviors = new List<BehaviorBase>() {
				new CustomSpinBulletsBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					OverrideBulletName = "homing",
					NumBullets = 2,
					BulletMinRadius = 4.5f,
					BulletMaxRadius = 5,
					BulletCircleSpeed = 90,
					BulletsIgnoreTiles = true,
					RegenTimer = 0.35f,
					AmountOFLines = 5,
				}
			};

		}
	}
}








