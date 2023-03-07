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

			AIActor shotzer = EnemyToolbox.CreateNewBulletBankerEnemy("shotzer_bullet", "Shotzer", 18, 18, new List<int> { 168, 169, 170, 171 }, new List<int> { 172, 173, 174, 175, 176, 177 }, null, null, 1.5f);
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
			public override IEnumerator Top() 
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
			public override IEnumerator Top()
			{
				yield return this.Wait(30);
				base.ChangeSpeed(new Speed(16f, SpeedType.Absolute), 60);
				yield break;
			}
		}
	}
}








