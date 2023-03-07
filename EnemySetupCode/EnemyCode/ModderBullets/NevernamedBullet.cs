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
	public class NevernamedBullet : AIActor
	{
		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("nevernamed_bullet", "Nevernamed", 16, 22, new List<int> { 130, 131, 132, 133 }, new List<int> { 134, 135, 136, 137, 138, 139 }, null, new SalamanderScript(), 1.5f);
		}
		public class SalamanderScript : Script 
		{
			public override IEnumerator Top()
			{
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				}
				float startingDirection = UnityEngine.Random.Range(-45f, 45f);
				Vector2 targetPos = base.GetPredictedTargetPosition((float)(((double)UnityEngine.Random.value >= 0.5) ? 1 : 0), 12f);
				for (int j = 0; j < 10; j++)
				{
					base.Fire(new Direction(startingDirection, DirectionType.Absolute, -1f), new Speed(11f, SpeedType.Absolute), new SalamanderScript.SnakeBullet(j * 3, targetPos));
				}
				yield break;
			}

			private const int NumSnakes = 10;
			private const int NumBullets = 5;
			private const int BulletSpeed = 12;
			private const float SnakeMagnitude = 0.75f;
			private const float SnakePeriod = 3f;
			public class SnakeBullet : Bullet
			{
				public SnakeBullet(int delay, Vector2 target) : base("default", false, false, false)
				{
					this.delay = delay;
					this.target = target;
				}

				public override IEnumerator Top()
				{
					base.ManualControl = true;
					yield return base.Wait(this.delay);
					Vector2 truePosition = base.Position;
					for (int i = 0; i < 360; i++)
					{
						float offsetMagnitude = Mathf.SmoothStep(-0.9f, 0.9f, Mathf.PingPong(0.5f + (float)i / 60f * 3f, 1f));
						if (i > 20 && i < 60)
						{
							float num = (this.target - truePosition).ToAngle();
							float value = BraveMathCollege.ClampAngle180(num - this.Direction);
							this.Direction += Mathf.Clamp(value, -8f, 8f);
						}
						truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
						base.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude);
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}

				private int delay;
				private Vector2 target;
			}
		}


		public class WallBullet : Bullet
		{
			public WallBullet() : base("default", false, false, false)
			{
			}
			public override IEnumerator Top()
			{
				yield return this.Wait(60);
				base.ChangeSpeed(new Speed(16f, SpeedType.Absolute), 60);
				yield break;
			}
		}
	}
}








