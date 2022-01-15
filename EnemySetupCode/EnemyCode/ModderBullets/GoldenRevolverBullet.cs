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
	public class GoldenRevolverBullet : AIActor
	{
		public static void Init()
		{
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/goldenrevolver/";
			string[] spritePaths = new string[]
			{
				TemplatePath+"grbullet_idle_001.png",
				TemplatePath+"grbullet_idle_002.png",
				TemplatePath+"grbullet_idle_003.png",
				TemplatePath+"grbullet_idle_004.png",

				TemplatePath+"grbullet_die_001.png",
				TemplatePath+"grbullet_die_002.png",
				TemplatePath+"grbullet_die_003.png",
				TemplatePath+"grbullet_die_004.png",
				TemplatePath+"grbullet_die_005.png",
			};
			EnemyToolbox.CreateNewBulletBankerEnemy("gr_bullet", "GoldenRevolver", 20, 19, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8 }, null, new SkellScript());
		}
		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));
				}
				for (int i = 0; i < UnityEngine.Random.Range(3, 7); i++)
				{
					base.PostWwiseEvent("Play_ENM_wizardred_appear_01", null);
					base.Fire(new Direction(UnityEngine.Random.Range(-180, 180), DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new WallBullet());
					yield return this.Wait(10);
				}
				yield break;
			}
		}
		public class WallBullet : Bullet
		{
			public WallBullet() : base("bigBullet", false, false, false)
			{
			}
			protected override IEnumerator Top()
			{
				base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
				yield return this.Wait(60 + UnityEngine.Random.Range(0, 60));
				float aim = base.AimDirection;
				this.Direction = aim;
				base.ChangeSpeed(new Speed(18f, SpeedType.Absolute), 30);
				yield break;
			}
		}
	}
}








