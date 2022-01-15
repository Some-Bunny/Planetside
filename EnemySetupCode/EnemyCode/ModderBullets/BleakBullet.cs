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
	public class BleakBullet : AIActor
	{

		public static void Init()
		{
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/bleak/";
			string[] spritePaths = new string[]
			{
				TemplatePath+"bleakbullet_idle_001.png",
				TemplatePath+"bleakbullet_idle_002.png",
				TemplatePath+"bleakbullet_idle_003.png",
				TemplatePath+"bleakbullet_idle_004.png",

				TemplatePath+"bleakbullet_die_001.png",
				TemplatePath+"bleakbullet_die_002.png",
				TemplatePath+"bleakbullet_die_003.png",
				TemplatePath+"bleakbullet_die_004.png",
				TemplatePath+"bleakbullet_die_005.png",
			};
			EnemyToolbox.CreateNewBulletBankerEnemy("bleak_bullet", "BleakBullets", 20, 20, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8 }, null, new SkellScript());
		}

		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				for (int i = 0; i < 5; i++)
				{
					base.PostWwiseEvent("Play_WPN_colt1851_shot_01", null);
					for (int a = 0; a <= 1; a++)
					{
						base.Fire(new Direction((-15f + (i * 3f)) + (a * (30 - (i * 6))), DirectionType.Aim, -1f), new Speed(11, SpeedType.Absolute), null);
					}
					
					yield return this.Wait(2);
				}
				base.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(15, SpeedType.Absolute), null);
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


				yield break;
			}

		}
	}
}








