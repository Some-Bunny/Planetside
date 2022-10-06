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
	public class TurboBullet : AIActor
	{
		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("turbo_bullet", "TurboGTXS", 18, 18, new List<int> { 207, 208, 209, 210 }, new List<int> { 211, 212, 213, 214, 215, 216 }, null, new SkellScript(), 3f);
		}

		
		public class SkellScript : Script 
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.GetBullet("default"));
				for (int i = 0; i < 8; i++)
                {
					base.Fire(new Direction(UnityEngine.Random.Range(-25, 25), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(7, 13), SpeedType.Absolute), new HelixChainBullet((UnityEngine.Random.value > 0.5f) ? false : true, UnityEngine.Random.Range(0.3f, 0.9f)));
					yield return base.Wait(6);
				}
				yield break;
			}
		}
		public class HelixChainBullet : Bullet
		{
			public HelixChainBullet(bool reverse, float str) : base("default", false, false, false)
			{
				this.reverse = reverse;
				this.HelixStrength = str;
				//base.SuppressVfx = true;
			}
			protected override IEnumerator Top()
			{
				float Back = HelixStrength - (HelixStrength * 2);
				this.ManualControl = true;
				Vector2 truePosition = this.Position;
				float startVal = 1;
				for (int i = 0; i < 360; i++)
				{
					float offsetMagnitude = Mathf.SmoothStep(Back, HelixStrength, Mathf.PingPong(startVal + (float)i / 90f * 3f, 1f));
					truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 90f);
					this.Position = truePosition + (this.reverse ? BraveMathCollege.DegreesToVector(this.Direction + 90f, offsetMagnitude) : BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude));
					yield return this.Wait(1);
				}
				this.Vanish(false);
				yield break;
			}
			private bool reverse;
			private float HelixStrength;

		}
	}
}








