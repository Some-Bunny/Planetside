using System;
using System.Collections;
using System.Collections.Generic;
using Brave.BulletScript;
using Dungeonator;
using FullInspector;
using UnityEngine;
using ItemAPI;
using EnemyBulletBuilder;
using static Planetside.BulletKingChanges;

namespace Planetside
{


	public class AgunimChanges : OverrideBehavior
	{
		public override string OverrideAIActorGUID => "41ee1c8538e8474a82a74c4aff99c712"; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
																						  // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
		public override void DoOverride()
		{
            // In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".

            //actor.MovementSpeed *= 2; // Doubles the enemy movement speed

            //healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 0.5f); // Halves the enemy health

            //The BehaviorSpeculator is responsible for almost everything an enemy does, from shooting a gun to teleporting.
            // Tip: To debug an enemy's BehaviorSpeculator, you can uncomment the line below. This will print all the behavior information to the console.
            //ToolsEnemy.DebugInformation(behaviorSpec);
            //SpinAttackBehavior 


            ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as ShootBehavior;
            ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(HelicopterRandomSimple1Modified));
            ShootBehavior1.Cooldown *= 2;


            AttackMoveBehavior AttackMoveBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as AttackMoveBehavior;
            AttackMoveBehavior1.bulletScript = new CustomBulletScriptSelector(typeof(HelicopterRandomRapid1Modified));
            AttackMoveBehavior1.Cooldown *= 3;

            ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
            ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(HelicopterLightning1Modified));
            ShootBehavior2.AttackCooldown *= 2;

            ShootBehavior ShootBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[4].Behavior as ShootBehavior;
            ShootBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(HelicopterFlames1Modified));

            actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableDefault);
			actor.bulletBank.Bullets.Add(StaticBulletEntries.UndodgeableOldKingSuckBullet);//UndodgeableDirectedfire
			actor.bulletBank.Bullets.Add(StaticBulletEntries.UndodgeableDirectedfire);
            actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableBig);


        }
        public class HelicopterRandomSimple1Modified : Script
        {
            public override IEnumerator Top()
            {
                if (UnityEngine.Random.value < 0.5f)
                {
                    int numBullets = 3;
                    float startDirection = base.RandomAngle();
                    string transform = "shoot point 1";
                    string transform2 = "shoot point 4";
                    if (BraveUtility.RandomBool())
                    {
                        BraveUtility.Swap<string>(ref transform, ref transform2);
                    }
                    for (int i = 0; i < numBullets; i++)
                    {
                        base.Fire(new Offset(transform), new Direction(base.SubdivideCircle(startDirection, numBullets, i, 1f, false), DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new HelicopterRandomSimple1Modified.BigBullet());
                    }
                    yield return base.Wait(30);
                    for (int j = 0; j < numBullets; j++)
                    {
                        base.Fire(new Offset(transform2), new Direction(base.SubdivideCircle(startDirection, numBullets, j, 1f, true), DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new HelicopterRandomSimple1Modified.BigBullet());
                    }
                }
                else
                {
                    int numBullets2 = 3;
                    float arc = 75f;
                    string transform3 = "shoot point 2";
                    string transform4 = "shoot point 3";
                    if (BraveUtility.RandomBool())
                    {
                        BraveUtility.Swap<string>(ref transform3, ref transform4);
                    }
                    float aimDirection = base.GetAimDirection(transform3);
                    for (int k = 0; k < numBullets2; k++)
                    {
                        base.Fire(new Offset(transform3), new Direction(base.SubdivideArc(aimDirection - arc, arc * 2f, numBullets2, k, false), DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new HelicopterRandomSimple1Modified.BigBullet());
                    }
                    yield return base.Wait(30);
                    aimDirection = base.GetAimDirection(transform4);
                    for (int l = 0; l < numBullets2; l++)
                    {
                        base.Fire(new Offset(transform4), new Direction(base.SubdivideArc(aimDirection - arc, arc * 2f, numBullets2, l, false), DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new HelicopterRandomSimple1Modified.BigBullet());
                    }
                }
                yield break;
            }

            public class BigBullet : Bullet
            {
                public BigBullet() : base(StaticBulletEntries.undodgeableBig.Name, false, false, false)
                {
                }

                public override IEnumerator Top()
                {
                    this.Projectile.Ramp(UnityEngine.Random.Range(2f, 3f), 2f);
                    return null;
                }
            }
        }
        public class HelicopterRandomRapid1Modified : Script
        {
            public override IEnumerator Top()
            {
                float startDirection = base.RandomAngle();
                string transform = BraveUtility.RandomElement<string>(HelicopterRandomRapid1.Transforms);
                for (int i = 0; i < 3; i++)
                {
                    base.Fire(new Offset(transform), new Direction(base.SubdivideCircle(startDirection, 3, i, 1f, false), DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new HelicopterRandomRapid1Modified.BigBullet());
                }
                if (BraveUtility.RandomBool())
                {
                    yield break;
                }
                yield return base.Wait(15);
                startDirection = base.RandomAngle();
                transform = BraveUtility.RandomElement<string>(HelicopterRandomRapid1.Transforms);
                for (int j = 0; j < 2; j++)
                {
                    base.Fire(new Offset(transform), new Direction(base.SubdivideCircle(startDirection, 3, j, 1f, false), DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new HelicopterRandomRapid1Modified.BigBullet());
                }
                yield break;
            }

            public class BigBullet : Bullet
            {
                public BigBullet() : base(StaticBulletEntries.undodgeableBig.Name, false, false, false)
                {
                }

                public override IEnumerator Top()
                {
                    this.Projectile.Ramp(UnityEngine.Random.Range(2f, 3f), 2f);
                    return null;
                }
            }

            private const int NumBullets = 6;

            private static string[] Transforms = new string[]
            {
                 "shoot point 1",
                 "shoot point 2",
                "shoot point 3",
                "shoot point 4"
            };
        }
        public class HelicopterLightning1Modified : Script
        {
            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_BOSS_agunim_ribbons_01", null);
                float aimDir = this.AimDirection;
                base.Fire(new Offset(-0.5f, 0.5f, 0f, string.Empty, DirectionType.Aim), new HelicopterLightning1Modified.LightningBullet(90 + aimDir, -1f, 40, -4, null));
                base.Fire(new Offset(-0.5f, 0.5f, 0f, string.Empty, DirectionType.Aim), new HelicopterLightning1Modified.LightningBullet(75 + aimDir, -1f, 40, -4, null));
                base.Fire(new Offset(-0.5f, 0.5f, 0f, string.Empty, DirectionType.Aim), new HelicopterLightning1Modified.LightningBullet(0+ aimDir, -1f, 40, 4, null));
                base.Fire(new Offset(0.5f, 0.5f, 0f, string.Empty, DirectionType.Aim), new HelicopterLightning1Modified.LightningBullet(-75+ aimDir, 1f, 40, -4, null));
                base.Fire(new Offset(0.5f, 0.5f, 0f, string.Empty, DirectionType.Aim), new HelicopterLightning1Modified.LightningBullet(-90 + aimDir, 1f, 40, -4, null));

                return null;
            }

            public const float Dist = 0.8f;

            public const int MaxBulletDepth = 40;

            public const float RandomOffset = 0.3f;

            public const float TurnChance = 0.2f;

            public const float TurnAngle = 24f;

            private class LightningBullet : Bullet
            {
                public LightningBullet(float direction, float sign, int maxRemainingBullets, int timeSinceLastTurn, Vector2? truePosition = null) : base(StaticBulletEntries.undodgeableDefault.Name, false, false, false)
                {
                    this.m_direction = direction;
                    this.m_sign = sign;
                    this.m_maxRemainingBullets = maxRemainingBullets;
                    this.m_timeSinceLastTurn = timeSinceLastTurn;
                    this.m_truePosition = truePosition;
                }

                public override IEnumerator Top()
                {
                    yield return base.Wait(4);
                    Vector2? truePosition = this.m_truePosition;
                    if (truePosition == null)
                    {
                        this.m_truePosition = new Vector2?(base.Position);
                    }
                    if (this.m_maxRemainingBullets > 0)
                    {
                        if (this.m_timeSinceLastTurn > 0 && this.m_timeSinceLastTurn != 2 && this.m_timeSinceLastTurn != 3 && UnityEngine.Random.value < 0.2f)
                        {
                            this.m_sign *= -1f;
                            this.m_timeSinceLastTurn = 0;
                        }
                        float num = this.m_direction + this.m_sign * 30f;
                        Vector2 vector = this.m_truePosition.Value + BraveMathCollege.DegreesToVector(num, 0.8f);
                        Vector2 vector2 = vector + BraveMathCollege.DegreesToVector(num + 90f, UnityEngine.Random.Range(-0.3f, 0.3f));
                        if (!base.IsPointInTile(vector2))
                        {
                            HelicopterLightning1Modified.LightningBullet lightningBullet = new HelicopterLightning1Modified.LightningBullet(this.m_direction, this.m_sign, this.m_maxRemainingBullets - 1, this.m_timeSinceLastTurn + 1, new Vector2?(vector));
                            base.Fire(Offset.OverridePosition(vector2), lightningBullet);
                            if (lightningBullet.Projectile && lightningBullet.Projectile.specRigidbody && PhysicsEngine.Instance.OverlapCast(lightningBullet.Projectile.specRigidbody, null, true, false, null, null, false, null, null, new SpeculativeRigidbody[0]))
                            {
                                lightningBullet.Projectile.DieInAir(false, true, true, false);
                            }
                        }
                    }
                    yield return base.Wait(30);
                    base.Vanish(true);
                    yield break;
                }

                private float m_direction;

                private float m_sign;

                private int m_maxRemainingBullets;

                private int m_timeSinceLastTurn;

                private Vector2? m_truePosition;
            }
        }

        public class HelicopterFlames1Modified : Script
        {
            public override IEnumerator Top()
            {
                List<AIActor> spawnedActors = new List<AIActor>();
                Vector2 basePos = base.BulletBank.aiActor.ParentRoom.area.UnitBottomLeft + new Vector2(5f, 22.8f);
                float height = 3.75f;
                float radius = 0.35f;
                float[] xPos = new float[]
                {
            (float)UnityEngine.Random.Range(4, 13),
            (float)UnityEngine.Random.Range(21, 30)
                };
                for (int i = 0; i < 2; i++)
                {
                    float num = xPos[i];
                    float num2 = UnityEngine.Random.Range(0f, 0.8f * height);
                    for (int j = 0; j < 5; j++)
                    {
                        Vector2 vector = basePos + new Vector2(num - radius, (float)j * -height - num2);
                        float direction = (vector - base.Position).ToAngle();
                        base.Fire(new Offset(HelicopterFlames1.s_Transforms[i * 2]), new Direction(direction, DirectionType.Absolute, -1f), new HelicopterFlames1Modified.FlameBullet(vector, spawnedActors, 60 + 5 * j));                  
                            
                        for (int jg = 0; jg < 5; jg++)
                            {
                                vector.x += 1.02f * radius;
                                direction = (vector - base.Position).ToAngle();
                                base.Fire(new Offset(HelicopterFlames1.s_Transforms[i * 2 + 1]), new Direction(direction, DirectionType.Absolute, -1f), new HelicopterFlames1Modified.FlameBullet(vector, spawnedActors, 60 + 5 * j));
                            }
                    }
                }
                yield return base.Wait(105);
                yield break;
            }

            private static string[] s_Transforms = new string[]
            {
        "shoot point 1",
        "shoot point 2",
        "shoot point 3",
        "shoot point 4"
            };

            public const int NumFlamesPerRow = 9;

            private class FlameBullet : Bullet
            {
                public FlameBullet(Vector2 goalPos, List<AIActor> spawnedActors, int flightTime) : base(StaticBulletEntries.UndodgeableOldKingSuckBullet.Name, false, false, false)
                {
                    this.m_goalPos = goalPos;
                    this.m_flightTime = flightTime;
                }

                public override IEnumerator Top()
                {
                    this.Projectile.IgnoreTileCollisionsFor((float)(this.m_flightTime - 5) / 60f);
                    this.Projectile.spriteAnimator.Play();
                    base.ManualControl = true;
                    this.Direction = (this.m_goalPos - base.Position).ToAngle();
                    this.Speed = Vector2.Distance(this.m_goalPos, base.Position) / ((float)this.m_flightTime / 60f);
                    Vector2 truePosition = base.Position;
                    for (int i = 0; i < this.m_flightTime; i++)
                    {
                        truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
                        base.Position = truePosition + new Vector2(0f, Mathf.Sin((float)i / (float)this.m_flightTime * 3.1415927f) * 5f);
                        yield return base.Wait(1);
                    }
                    yield return base.Wait(480);
                    base.Vanish(false);
                    yield break;
                }

                private Vector2 m_goalPos;

                private int m_flightTime;
            }
        }

    }
}
