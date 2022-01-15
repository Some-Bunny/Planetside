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
	public class BotBullet : AIActor
	{

		public static void Init()
		{
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/bot/";
			string[] spritePaths = new string[]
			{
				TemplatePath+"botbullet_idle_001.png",
				TemplatePath+"botbullet_idle_002.png",
				TemplatePath+"botbullet_idle_003.png",
				TemplatePath+"botbullet_idle_004.png",

				TemplatePath+"botbullet_die_001.png",
				TemplatePath+"botbullet_die_002.png",
				TemplatePath+"botbullet_die_003.png",
				TemplatePath+"botbullet_die_004.png",
				TemplatePath+"botbullet_die_005.png",
			};
			EnemyToolbox.CreateNewBulletBankerEnemy("bot_bullet", "Not A Bot", 16, 16, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8 }, null, new SkellScript(), 3f);
		}

		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));
				float aim = base.AimDirection;
				for (int i = 0; i < 20; i++)
				{
					base.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(15, SpeedType.Absolute), new WallBullet());
					yield return this.Wait(3);
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








