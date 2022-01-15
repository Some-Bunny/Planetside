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
	public class BlazeyBullet : AIActor
	{

		public static void Init()
		{
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/blazey/";
			string[] spritePaths = new string[]
			{
				TemplatePath+"blazeybullet_idle_001.png",
				TemplatePath+"blazeybullet_idle_002.png",
				TemplatePath+"blazeybullet_idle_003.png",
				TemplatePath+"blazeybullet_idle_004.png",

				TemplatePath+"blazeybullet_die_001.png",
				TemplatePath+"blazeybullet_die_002.png",
				TemplatePath+"blazeybullet_die_003.png",
				TemplatePath+"blazeybullet_die_004.png",
				TemplatePath+"blazeybullet_die_005.png",
			};
			EnemyToolbox.CreateNewBulletBankerEnemy("blazey_bullet", "Blazeykat", 18, 20, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8 }, null, new SkellScript());
		}
		public class SkellScript : Script
		{
			protected override IEnumerator Top()
			{
				AkSoundEngine.PostEvent("Play_WPN_stickycrossbow_shot_01", this.BulletBank.aiActor.gameObject);
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));
				}
				for (int i = 0; i <= 18; i++)
				{
					this.Fire(new Direction(i * 20, DirectionType.Aim, -1f), new Speed(7f+(i/2), SpeedType.Absolute), new SkellBullet());
				}
				yield break;
			}
		}


		public class SkellBullet : Bullet
		{
			public SkellBullet() : base("sweep", false, false, false)
			{

			}
		}
	}
}








