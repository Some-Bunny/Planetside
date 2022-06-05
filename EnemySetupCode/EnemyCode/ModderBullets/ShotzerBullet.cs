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
	public class ShotzerBullet : AIActor
	{
		public static void Init()
		{
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/";
			string folderName = "shotzer";
			string idleFrameName = "shotzer_idle_00";
			string deathFrameName = "shotzer_die_00";
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
			AIActor shotzer = EnemyToolbox.CreateNewBulletBankerEnemy("shotzer_bullet", "Shotzer", 18, 18, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8, 9 }, null, null, 1.5f);
			ImprovedAfterImage image = shotzer.aiActor.gameObject.AddComponent<ImprovedAfterImage>();
			image.dashColor = new Color(1, 0, 0f);
			image.spawnShadows = true;
			AfterImageTrailController im = shotzer.aiActor.gameObject.AddComponent<AfterImageTrailController>();
			im.spawnShadows = false;
			shotzer.aiActor.gameObject.AddComponent<tk2dSpriteAttachPoint>();
			shotzer.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();


			var bs = shotzer.gameObject.GetComponent<BehaviorSpeculator>();
			bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new CustomDashBehavior()
				{
					ShootPoint = shotzer.gameObject.transform.Find("baseShootpoint").gameObject,
					dashDistance = 10f,
					dashTime = 0.75f,
					AmountOfDashes = 1,
					enableShadowTrail = false,
					Cooldown = 1,
					dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
					warpDashAnimLength = true,
					hideShadow = true,
					fireAtDashStart = true,
					InitialCooldown = 1f,
					AttackCooldown = 3,
					bulletScript = new CustomBulletScriptSelector(typeof(SalamanderScript)),
					RequiresLineOfSight = false,
				}
			};
		}
		public class SalamanderScript : Script 
		{
			protected override IEnumerator Top() 
			{
				AkSoundEngine.PostEvent("Play_WPN_stickycrossbow_shot_01", this.BulletBank.aiActor.gameObject);
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));
				for (int i = -4; i <= 4; i++)
				{
					base.Fire(new Direction((float)(i * 3f), DirectionType.Aim, -1f), new Speed(4f - (float)Mathf.Abs(i) * 0.3f, SpeedType.Absolute), new WallBullet());
				}
				yield break;
			}
		}

		public class WallBullet : Bullet
		{
			public WallBullet() : base("sweep", false, false, false)
			{
			}
			protected override IEnumerator Top()
			{
				yield return this.Wait(30);
				base.ChangeSpeed(new Speed(16f, SpeedType.Absolute), 45);
				yield break;
			}
		}
	}
}








