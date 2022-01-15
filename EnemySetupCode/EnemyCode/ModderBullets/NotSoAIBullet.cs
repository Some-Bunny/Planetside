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
	public class NotSoAIBullet : AIActor
	{
		public static void Init()
		{
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/";
			string folderName = "notsoai";
			string idleFrameName = "aibullet_idle_00";
			string deathFrameName = "aibullet_die_00";
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
			EnemyToolbox.CreateNewBulletBankerEnemy("notsoai_bullet", "NotSoAI", 20, 19, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8 }, null, new SkellScript());
		}
		public class SkellScript : Script 
		{
			protected override IEnumerator Top() 
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				float startDirection = base.RandomAngle();
				for (int e = -1; e < 2; e++)
                {
					float radius = 0.0125f;
					float delta = 30f;

					for (int j = 0; j < 12; j++)
					{
						base.Fire(new Direction((90*e), DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new TheGear(this, startDirection + (float)j * delta, radius, 60*e));
					}
					radius = 0.025f;
					delta = 45f;
					for (int j = 0; j < 8; j++)
					{
						base.Fire(new Direction((90 * e), DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new TheGear(this, startDirection + (float)j * delta, radius, 60 * e));
					}
				}

				
				yield break;
			}
		}


		public class TheGear : Bullet
		{
			public TheGear(SkellScript parent, float angle = 0f, float aradius = 0, float RotAngle = 0) : base("default", false, false, false)
			{
				this.m_parent = parent;
				this.m_angle = angle;
				this.m_radius = aradius;
				this.RotationAngle = RotAngle;

			}

			protected override IEnumerator Top()
			{
				base.ManualControl = true;
				Vector2 centerPosition = base.Position;
				float radius = 0f;
				this.m_spinSpeed = 40f;
				for (int i = 0; i < 300; i++)
				{
					if (i == 40)
					{
						base.ChangeSpeed(new Speed(11f, SpeedType.Absolute), 120);
						base.ChangeDirection(new Direction(this.m_parent.GetAimDirection(1f, 10f)+ RotationAngle, DirectionType.Absolute, -1f), 20);
						base.StartTask(this.ChangeSpinSpeedTask(180f, 240));
					}
					base.UpdateVelocity();
					centerPosition += this.Velocity / 60f;
					if (i < 40)
					{
						radius += m_radius;
					}
					this.m_angle += this.m_spinSpeed / 60f;
					base.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);
					yield return base.Wait(1);
				}
				base.Vanish(false);
				yield break;
			}

			private IEnumerator ChangeSpinSpeedTask(float newSpinSpeed, int term)
			{
				float delta = (newSpinSpeed - this.m_spinSpeed) / (float)term;
				for (int i = 0; i < term; i++)
				{
					this.m_spinSpeed += delta;
					yield return base.Wait(1);
				}
				yield break;
			}
			private const float ExpandSpeed = 4.5f;
			private const float SpinSpeed = 40f;
			private SkellScript m_parent;
			private float m_angle;
			private float m_spinSpeed;
			private float m_radius;
			private float RotationAngle;

		}
	}
}








