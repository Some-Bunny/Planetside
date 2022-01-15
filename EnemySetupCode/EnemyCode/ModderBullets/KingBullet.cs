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
	public class KingBullet : AIActor
	{
		public static void Init()
		{
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/";
			string folderName = "king";
			string idleFrameName = "kingbullet_idle_00";
			string deathFrameName = "kingbullet_die_00";

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
			TemplatePath+folderName+"/"+deathFrameName+"6.png",

			};
			EnemyToolbox.CreateNewBulletBankerEnemy("king_bullet", "Round King", 16, 20, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8, 9 }, null, new SkellScript());
		}



		
		public class SkellScript : Script 
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				float angle = base.RandomAngle();
				for (int i = 0; i < 20; i++)
                {
					base.Fire(new Direction(angle + (18 * i), DirectionType.Aim, -1f), new Speed(11f, SpeedType.Absolute), new WallBullet());
				}
				yield break;
			}
		}
		public class WallBullet : Bullet
		{
			public WallBullet() : base("default", false, false, false)
			{

			}
		}
	}
}








