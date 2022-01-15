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
	public class SkilotarBullet : AIActor
	{
		public static void Init()
		{
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/";
			string folderName = "skilotar";
			string idleFrameName = "skilotarbullet_idle_00";
			string deathFrameName = "skilotarbullet_die_00";
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
			EnemyToolbox.CreateNewBulletBankerEnemy("skilotar_bullet", "Skilotar_", 20, 19, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8, 9 }, null, new SalamanderScript(), 3f);
		}
		public class SalamanderScript : Script 
		{
			protected override IEnumerator Top()
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1b5810fafbec445d89921a4efb4e42b7").bulletBank.GetBullet("bouncingRing"));
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1b5810fafbec445d89921a4efb4e42b7").bulletBank.GetBullet("bouncingMouth"));

				}
				for (int i = 0; i < 1; i++)
				{
					float aim = base.GetAimDirection((float)((UnityEngine.Random.value >= 0.4f) ? 0 : 1), 8f) + UnityEngine.Random.Range(-10f, 10f);
					for (int j = 0; j < 18; j++)
					{
						float angle = (float)j * 20f;
						Vector2 desiredOffset = BraveMathCollege.DegreesToVector(angle, 1.8f);
						base.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new SalamanderScript.BouncingRingBullet("bouncingRing", desiredOffset));
					}
					base.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new SalamanderScript.BouncingRingBullet("bouncingRing", new Vector2(-0.7f, 0.7f)));
					base.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new SalamanderScript.BouncingRingBullet("bouncingMouth", new Vector2(0f, 0.4f)));
					base.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new SalamanderScript.BouncingRingBullet("bouncingRing", new Vector2(0.7f, 0.7f)));
					yield return base.Wait(40);
				}
				yield break;
			}

			private const int NumBlobs = 8;

			private const int NumBullets = 18;

			public class BouncingRingBullet : Bullet
			{
				public BouncingRingBullet(string name, Vector2 desiredOffset) : base(name, false, false, false)
				{
					this.m_desiredOffset = desiredOffset;
				}

				protected override IEnumerator Top()
				{
					Vector2 centerPoint = base.Position;
					Vector2 lowestOffset = BraveMathCollege.DegreesToVector(-90f, 1.5f);
					Vector2 currentOffset = Vector2.zero;
					float squishFactor = 1f;
					float verticalOffset = 0f;
					int unsquishIndex = 100;
					base.ManualControl = true;
					for (int i = 0; i < 300; i++)
					{
						if (i < 30)
						{
							currentOffset = Vector2.Lerp(Vector2.zero, this.m_desiredOffset, (float)i / 30f);
						}
						verticalOffset = (Mathf.Abs(Mathf.Cos((float)i / 90f * 3.14159274f)) - 1f) * 2.5f;
						if (unsquishIndex <= 10)
						{
							squishFactor = Mathf.Abs(Mathf.SmoothStep(0.6f, 1f, (float)unsquishIndex / 10f));
							unsquishIndex++;
						}
						Vector2 relativeOffset = currentOffset - lowestOffset;
						Vector2 squishedOffset = lowestOffset + relativeOffset.Scale(1f, squishFactor);
						base.UpdateVelocity();
						centerPoint += this.Velocity / 60f;
						base.Position = centerPoint + squishedOffset + new Vector2(0f, verticalOffset);
						if (i % 90 == 45)
						{
							for (int j = 1; j <= 10; j++)
							{
								squishFactor = Mathf.Abs(Mathf.SmoothStep(1f, 0.5f, (float)j / 10f));
								relativeOffset = currentOffset - lowestOffset;
								squishedOffset = lowestOffset + relativeOffset.Scale(1f, squishFactor);
								centerPoint += 0.333f * this.Velocity / 60f;
								base.Position = centerPoint + squishedOffset + new Vector2(0f, verticalOffset);
								yield return base.Wait(1);
							}
							unsquishIndex = 1;
						}
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}

				private Vector2 m_desiredOffset;
			}
		}


		public class WallBullet : Bullet
		{
			public WallBullet() : base("bigBullet", false, false, false)
			{
			}
			protected override IEnumerator Top()
			{
				yield return this.Wait(60);
				base.ChangeSpeed(new Speed(16f, SpeedType.Absolute), 60);
				yield break;
			}
		}
	}
}








