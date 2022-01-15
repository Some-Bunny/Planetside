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
	public class GlaurBullet : AIActor
	{

		public static void Init()
		{
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/glaur/";
			string[] spritePaths = new string[]
			{
				TemplatePath+"glaurbullet_idle_001.png",
				TemplatePath+"glaurbullet_idle_002.png",
				TemplatePath+"glaurbullet_idle_003.png",
				TemplatePath+"glaurbullet_idle_004.png",

				TemplatePath+"glaurbullet_die_001.png",
				TemplatePath+"glaurbullet_die_002.png",
				TemplatePath+"glaurbullet_die_003.png",
				TemplatePath+"glaurbullet_die_004.png",
				TemplatePath+"glaurbullet_die_005.png",
				TemplatePath+"glaurbullet_die_006.png",
			};
			EnemyToolbox.CreateNewBulletBankerEnemy("glaurung_bullet", "Glaurung", 20, 16, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8, 9 }, null, new SkellScript(), 2f);
		}

		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
				for (int j = 0; j < 8; j++)
				{
					float Aim = UnityEngine.Random.Range(-25f, 25);
					base.Fire(new Direction(Aim, DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(8f, 12f), SpeedType.Absolute), new Flames());
				}
				yield break;
			}
		}


		public class Flames : Bullet
		{
			public Flames() : base("frogger", false, false, false)
			{

			}

			protected override IEnumerator Top()
			{
				float speed = base.Speed;
				base.ChangeSpeed(new Speed(speed*1.5f, SpeedType.Absolute), 120);
				yield break;
			}
		}
	}
}








