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
	public class SpcreatBullet : AIActor
	{
		public static void Init()
		{
			string TemplatePath = "Planetside/Resources/Enemies/ModderBullets/";
			string folderName = "spcreat";
			string idleFrameName = "spcreatbullet_idle_00";
			string deathFrameName = "spcreatbullet_death_00";
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
			EnemyToolbox.CreateNewBulletBankerEnemy("spcreat_bullet", "Spcreat", 18, 18, spritePaths[0], spritePaths, new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7, 8, 9 }, null, new SalamanderScript(), 3f);
		}


		public class SalamanderScript : Script 
		{

			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				Vector2 zero = Vector2.zero;
				Vector2 p = zero + new Vector2(-1.5f, -1.5f);
				Vector2 vector = new Vector2(2f, -5f);
				Vector2 p2 = vector + new Vector2(-1.5f, 0.4f);
				Vector2 vector2 = new Vector2(-0.5f, -1.5f);
				Vector2 p3 = vector2 + new Vector2(0.75f, 0.75f);
				Vector2 vector3 = new Vector2(-0.5f, 1.5f);
				Vector2 p4 = vector3 + new Vector2(0.75f, -0.75f);
				float num = BraveMathCollege.ClampAngle180((this.BulletManager.PlayerPosition() - base.BulletBank.specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle());
				bool flag = num > -45f && num < 120f;
				Vector2 phantomBulletPoint = base.Position + BraveMathCollege.CalculateBezierPoint(0.5f, zero, p, p2, vector);
				for (int i = 0; i < 6; i++)
				{
					float num2 = (float)i / 11f;
					Vector2 offset = BraveMathCollege.CalculateBezierPoint(num2, zero, p, p2, vector);
					if (flag)
					{
						num2 = 1f - num2;
					}
					Vector2 offset2 = BraveMathCollege.CalculateBezierPoint(num2, vector2, p3, p4, vector3);
					base.Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(0f, DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new SalamanderScript.ReaperBullet(phantomBulletPoint, offset2));
				}
				return null;
			}

			public class ReaperBullet : Bullet
			{
				public ReaperBullet(Vector2 phantomBulletPoint, Vector2 offset) : base("default", false, false, false)
				{
					this.m_phantomBulletPoint = phantomBulletPoint;
					this.m_offset = offset;
				}

				protected override IEnumerator Top()
				{
					base.ManualControl = true;
					yield return base.Wait(5);
					for (int i = 0; i < 180; i++)
					{
						this.Projectile.ResetDistance();
						this.Direction = Mathf.MoveTowardsAngle(this.Direction, base.GetAimDirection(this.m_phantomBulletPoint, 0f, this.Speed), 2f);
						base.UpdateVelocity();
						this.m_phantomBulletPoint += this.Velocity / 60f;
						base.Position += this.Velocity / 60f;
						float rotation = this.Velocity.ToAngle();
						Vector2 goalPos = this.m_phantomBulletPoint + this.m_offset.Rotate(rotation);
						if (i < 30)
						{
							Vector2 a = goalPos - base.Position;
							base.Position += a / (float)(30 - i);
						}
						else
						{
							base.Position = goalPos;
						}
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}
				private Vector2 m_phantomBulletPoint;
				private Vector2 m_offset;
			}
		}
	}
}








