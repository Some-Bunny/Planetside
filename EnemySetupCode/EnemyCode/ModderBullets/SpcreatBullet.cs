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
			var ae = EnemyToolbox.CreateNewBulletBankerEnemy("spcreat_bullet", "Spcreat", 18, 18,  new List<int> { 197, 198, 199, 200 }, new List<int> { 201, 202, 203, 204, 205, 206 }, null, new SpcreatAttack(), 3f);
            ae.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6868795625bd46f3ae3e4377adce288b").bulletBank.GetBullet("dagger"));

        }


        public class SpcreatAttack : Script 
		{

			public override IEnumerator Top()
			{
                if (base.BulletBank && base.BulletBank.aiActor && base.BulletBank.aiActor.TargetRigidbody)
                {
                    this.m_targetRigidbody = base.BulletBank.aiActor.TargetRigidbody;
                }
                else if (GameManager.Instance.BestActivePlayer)
                {
                    this.m_targetRigidbody = GameManager.Instance.BestActivePlayer.specRigidbody;
                }
                float angle = UnityEngine.Random.Range(0f, 360f);
                for (int i = 0; i < 24; i++)
                {
                    base.Fire(new Offset(new Vector2(1f, 0f), angle, string.Empty, DirectionType.Absolute), new Direction(angle, DirectionType.Absolute, -1f), new SpcreatAttack.RingBullet(angle, this));
                    yield return base.Wait(3);
                }
                yield break;
            }
            private float RetargetAngle
            {
                get
                {
                    if (this.m_targetRigidbody)
                    {
                        return (this.m_targetRigidbody.HitboxPixelCollider.UnitCenter - base.Position).ToAngle();
                    }
                    float? cachedRetargetAngle = this.m_cachedRetargetAngle;
                    if (cachedRetargetAngle == null)
                    {
                        this.m_cachedRetargetAngle = new float?(UnityEngine.Random.Range(0f, 360f));
                    }
                    return this.m_cachedRetargetAngle.Value;
                }
            }
            private const float SpinSpeed = 720f;
            private const float FireRadius = 1f;

            private SpeculativeRigidbody m_targetRigidbody;

            private float? m_cachedRetargetAngle;

            public class RingBullet : Bullet
            {
                public RingBullet(float angle, SpcreatAttack parentScript) : base("dagger", false, false, false)
                {
                    this.m_angle = angle;
                    this.m_parentScript = parentScript;
                }

                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    this.Projectile.specRigidbody.CollideWithTileMap = false;
                    Vector2 center = this.m_parentScript.Position;
                    for (int i = 0; i < 60; i++)
                    {
                        this.m_angle += 9f;
                        float shownAngle = this.m_angle;
                        if (i >= 50)
                        {
                            shownAngle = Mathf.LerpAngle(this.m_angle, this.m_parentScript.RetargetAngle, (float)(i - 49) / 10f);
                        }
                        base.Position = center + BraveMathCollege.DegreesToVector(shownAngle, 1f);
                        yield return base.Wait(1);
                    }
                    this.Projectile.specRigidbody.CollideWithTileMap = true;
                    this.Direction = this.m_parentScript.RetargetAngle;
                    this.Speed = 3f;
                    base.ManualControl = false;
                    this.ChangeSpeed(new Brave.BulletScript.Speed(18, SpeedType.Absolute), 120);
                    yield break;
                }
                private float m_angle;
                private SpcreatAttack m_parentScript;
            }
        }
	}
}








