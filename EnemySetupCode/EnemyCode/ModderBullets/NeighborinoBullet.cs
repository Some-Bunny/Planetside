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
	public class NeighborinoBullet : AIActor
	{
		public static void Init()
		{
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/";
			string folderName = "neighborino";
			string idleFrameName = "neighbullet_idle_00";
			string deathFrameName = "neighbullet_die_00";

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
			EnemyToolbox.CreateNewBulletBankerEnemy("neighborino_bullet", "Neighborino", 20, 20, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8 }, null, new SkellScript(), 2f);
		}
		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("icicle"));
				for (int i = -7; i <= 3; i++)
				{
					base.Fire(new Direction((float)(i * 2), DirectionType.Aim, -1f), new Speed(13f - (float)Mathf.Abs(i) * 0.3f, SpeedType.Absolute), new Frost());

				}
				yield return Wait(30);
				for (int i = -3; i <= 7; i++)
				{
					base.Fire(new Direction((float)(i * 2), DirectionType.Aim, -1f), new Speed(13f - (float)Mathf.Abs(i) * 0.3f, SpeedType.Absolute), new Gunfire());

				}
				yield break;
			}
		}
		public class Frost : Bullet
		{
			public Frost() : base("icicle", false, false, false)
			{

			}
		}

		public class Gunfire : Bullet
		{
			public Gunfire() : base("frogger", false, false, false)
			{

			}
		}
	}
}








