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
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/";
			string folderName = "hunter";
			string idleFrameName = "hunterbullet_idle_00";
			string deathFrameName = "hunterbullet_die_00";

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
			EnemyToolbox.CreateNewBulletBankerEnemy("hunter_bullet", "Bl4ckHunter", 18, 19, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8 }, null, new SalamanderScript(), 3f);
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








