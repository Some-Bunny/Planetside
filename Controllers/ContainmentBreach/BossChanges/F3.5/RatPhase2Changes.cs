using System;
using System.Collections;
using System.Collections.Generic;
using Brave.BulletScript;
using Dungeonator;
using FullInspector;
using UnityEngine;
using ItemAPI;
using EnemyBulletBuilder;
using static Planetside.RatPhase1Changes;

namespace Planetside
{
	public class RatPhase2Changes : OverrideBehavior
	{
		public override string OverrideAIActorGUID => Alexandria.EnemyGUIDs.Resourceful_Rat_Mech_Boss_GUID; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
                                                                                                       // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
        public override void DoOverride()
		{

            //ToolsEnemy.DebugInformation(behaviorSpec);

            //actor.MovementSpeed *= 0.75f; // Doubles the enemy movement speed
            //actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableBig);
            //actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableBigBullet);

            actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableDefault);
            actor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableSmallSpore);
            actor.bulletBank.Bullets.Add(StaticBulletEntries.UnDodgeableSpinner);
            actor.bulletBank.Bullets.Add(StaticBulletEntries.UnDodgeableBigOne);



            ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as ShootBehavior;
            ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMetalGearRatJumpPound1));


            ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootBehavior;
            ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(MetalGearRatSidePoundLeft1Modified));

            ShootBehavior ShootBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
            ShootBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(MetalGearRatSidePoundRight1Modified));

            MetalGearRatBeamsBehavior metalGearRatBeams = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[6].Behavior as MetalGearRatBeamsBehavior;
            metalGearRatBeams.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMetalGearRatLaserBullets1));

            ShootBehavior ShootBehavior4 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[8].Behavior as ShootBehavior;
            ShootBehavior4.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMetalGearRatSpinners1));

            ShootBehavior ShootBehavior5 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[10].Behavior as ShootBehavior;
            ShootBehavior5.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedMetalGearRatTailgun1));
        }

        public class ModifiedMetalGearRatJumpPound1 : Script
        {
            public override IEnumerator Top()
            {
                float deltaAngle = 8.372093f;
                bool boolean = BraveUtility.RandomBool();

                for (int i = 0; i < 2; i++)
                {
                    boolean = !boolean;
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < 43; k++)
                        {
                            bool b = k % 4 > 1;
                            if (boolean == false)
                            {
                                b = !b;
                            }
                            float num = -90f - ((float)k + (float)j * 0.5f) * deltaAngle;
                            Vector2 ellipsePointSmooth = BraveMathCollege.GetEllipsePointSmooth(Vector2.zero, 6f, 2f, num);
                            base.Fire(new Offset(ellipsePointSmooth, 0f, string.Empty, DirectionType.Absolute), new Direction(num, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new DelayedBullet(b ? "default_noramp" : StaticBulletEntries.undodgeableDefault.Name, j * 4));
                        }
                    }
                    yield return base.Wait(60);
                }
                yield return base.Wait(20);

                yield break;
            }
        }
        public class MetalGearRatSidePoundLeft1Modified : ModifiedMetalGearRatSidePound1
        {

            protected override float StartAngle
            {
                get
                {
                    return 100f;
                }
            }

            protected override float SweepAngle
            {
                get
                {
                    return 100f;
                }
            }
        }
        public class MetalGearRatSidePoundRight1Modified : ModifiedMetalGearRatSidePound1
        {

            protected override float StartAngle
            {
                get
                {
                    return 80f;
                }
            }

            protected override float SweepAngle
            {
                get
                {
                    return -100f;
                }
            }
        }
        public abstract class ModifiedMetalGearRatSidePound1 : Script
        {
 
            protected abstract float StartAngle { get; }
            protected abstract float SweepAngle { get; }
            public override IEnumerator Top()
            {
                int i = 0;
                while ((float)i < 7f)
                {
                    bool isOffset = i % 2 == 1;
                    int numBullets = 9 - i;
                    for (int j = 0; j < numBullets + ((!isOffset) ? 0 : -1); j++)
                    {
                        float num = base.SubdivideArc(this.StartAngle, this.SweepAngle, numBullets, j, isOffset);
                        Vector2 ellipsePointSmooth = BraveMathCollege.GetEllipsePointSmooth(Vector2.zero, 2.5f, 1f, num);
                        base.Fire(new Offset(ellipsePointSmooth, 0f, string.Empty, DirectionType.Absolute), new Direction(num, DirectionType.Absolute, -1f), new Speed((float)(14 - i * 2), SpeedType.Absolute), new ModifiedMetalGearRatSidePound1.WaftBullet());
                    }
                    yield return base.Wait(6);
                    i++;
                }
                yield break;
            }

            
            public class WaftBullet : Bullet
            {
                public WaftBullet() : base(StaticBulletEntries.undodgeableDefault.Name, false, false, false)
                {
                }

                public override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 150);
                    yield return base.Wait(150);
                    base.ManualControl = true;
                    Vector2 truePosition = base.Position;
                    float xOffset = UnityEngine.Random.Range(0f, 3f);
                    float yOffset = UnityEngine.Random.Range(0f, 1f);
                    truePosition -= new Vector2(Mathf.Sin(xOffset * 3.1415927f / 3f) * 0.65f, Mathf.Sin(yOffset * 3.1415927f / 1f) * 0.25f);
                    for (int i = 0; i < 300; i++)
                    {
                        truePosition += new Vector2(0f, 0.008333334f);
                        float t = (float)i / 60f;
                        float waftXOffset = Mathf.Sin((t + xOffset) * 3.1415927f / 3f) * 0.65f;
                        float waftYOffset = Mathf.Sin((t + yOffset) * 3.1415927f / 1f) * 0.25f;
                        base.Position = truePosition + new Vector2(waftXOffset, waftYOffset);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }
            }
        }
        public class ModifiedMetalGearRatLaserBullets1 : Script
        {
            public override IEnumerator Top()
            {
                AIBeamShooter[] beams = base.BulletBank.GetComponents<AIBeamShooter>();
                for (; ; )
                {
                    yield return base.Wait(25);
                    if (beams == null || beams.Length == 0)
                    {
                        break;
                    }
                    AIBeamShooter beam = beams[UnityEngine.Random.Range(1, beams.Length)];
                    if (beam && beam.LaserBeam)
                    {
                        Vector2 overridePosition = beam.LaserBeam.Origin + beam.LaserBeam.Direction.normalized * beam.MaxBeamLength;
                        base.Fire(Offset.OverridePosition(overridePosition), new ModifiedMetalGearRatLaserBullets1.LaserBullet());
                    }
                }
                yield break;
            }

            private const int NumBullets = 12;

            public class LaserBullet : Bullet
            {
                public LaserBullet() : base(StaticBulletEntries.undodgeableSmallSpore.Name, false, false, false)
                {
                }

                public override IEnumerator Top()
                {
                    this.Projectile.IgnoreTileCollisionsFor(1.25f);
                    yield return base.Wait(600);
                    base.Vanish();
                    yield break;
                }
            }
        }
        public class ModifiedMetalGearRatSpinners1 : Script
        {
            private Vector2 CenterPoint { get; set; }

            private bool Done { get; set; }

            public override IEnumerator Top()
            {
                yield return base.Wait(75);
                base.EndOnBlank = true;
                this.CenterPoint = base.BulletBank.aiActor.ParentRoom.area.UnitCenter + new Vector2(0f, 4.5f);
                this.m_circleDummies.Clear();
                base.StartTask(this.SpawnInnerRing());
                base.StartTask(this.SpawnOuterRing());
                int spinTime = 915;
                for (int i = 0; i < spinTime; i++)
                {
                    for (int j = 0; j < this.m_circleDummies.Count; j++)
                    {
                        this.m_circleDummies[j].DoTick();
                    }
                    if (i == spinTime - 60)
                    {
                        this.Done = true;
                    }
                    yield return base.Wait(1);
                }
                yield break;
            }

            private IEnumerator SpawnInnerRing()
            {
                int spinTime = 500;
                int numCircles = 3;
                float initialRadiusBoost = 12f;
                float orbitSpeed = 360f / ((float)spinTime / 60f);
                float radius = 7.5f;
                float rotationSpeed = -45f;
                
                this.SpawnCircleDebris(radius, 140f, 0f, 0.35f, 0f, new float?(initialRadiusBoost));
                //this.SpawnCircleDebris(radius, 130f, 0f, 0.37f, 0f, new float?(initialRadiusBoost));
                //this.SpawnCircleDebris(radius, 120f, 0f, 0.31f, 0f, new float?(initialRadiusBoost));
                //this.SpawnCircleDebris(radius, 110f, 0f, 0.23f, 0f, new float?(initialRadiusBoost));
                //this.SpawnCircleDebris(radius, 100f, 0f, 0.27f, 0f, new float?(initialRadiusBoost));
                //this.SpawnCircleDebris(radius, 90f, 0f, 0.29f, 0f, new float?(initialRadiusBoost));
                //this.SpawnCircleDebris(radius, 80f, 0f, 0.27f, 0f, new float?(initialRadiusBoost));
                //this.SpawnCircleDebris(radius, 70f, 0f, 0.23f, 0f, new float?(initialRadiusBoost));
                //this.SpawnCircleDebris(radius, 60f, 0f, 0.31f, 0f, new float?(initialRadiusBoost));
                //this.SpawnCircleDebris(radius, 50f, 0f, 0.37f, 0f, new float?(initialRadiusBoost));
                this.SpawnCircleDebris(radius, 40f, 0f, 0.35f, 0f, new float?(initialRadiusBoost));
                
                for (int i = 0; i < numCircles; i++)
                {
                    float spawnAngle = 90f;
                    this.SpawnCircle(radius, spawnAngle, orbitSpeed, rotationSpeed, new float?(initialRadiusBoost));
                    
                    
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.95f, -50f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.95f, -30f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.95f, 30f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.95f, 50f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.78f, -40f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.78f, 60f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.78f, 40f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.6f, -50f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.6f, 50f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.45f, 60f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.25f, 60f, new float?(initialRadiusBoost));
                    
                    yield return base.Wait(spinTime / numCircles);
                }
                yield break;
            }

            private IEnumerator SpawnOuterRing()
            {
                int spinTime = 1100;
                int numCircles = 8;
                float initialRadiusBoost = 12f;
                float orbitSpeed = 360f / ((float)spinTime / 60f);
                float radius = 18.5f;
                float rotationSpeed = 45f;
                float deltaAngle = 0f;
                for (int i = 0; i < numCircles; i++)
                {
                    float spawnAngle = deltaAngle;
                    this.SpawnCircle(radius, spawnAngle, -orbitSpeed, rotationSpeed, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 0f, -17.5f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 0f, 17.5f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 0.15f, 22.5f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 0.3f, 22.5f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 0.45f, 22.5f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 0.6f, 22.5f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 0.75f, -19f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 0.75f, 19f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 0.9f, 22.5f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 1.05f, -17.5f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 1.05f, 17.5f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 1.2f, 22.5f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 1.2f, 0f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 1.35f, -15f, new float?(initialRadiusBoost));
                    //this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 1.35f, 15f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, -orbitSpeed, 1.5f, 22.5f, new float?(initialRadiusBoost));
                    
                    deltaAngle += 360f / (float)numCircles;
                }
                yield return base.Wait(1);
                yield break;
            }

            private void SpawnCircle(float spawnRadius, float spawnAngle, float orbitSpeed, float rotationSpeed, float? initialRadiusBoost = null)
            {
                float magnitude = spawnRadius + ((initialRadiusBoost == null) ? 0f : initialRadiusBoost.Value);
                Vector2 vector = this.CenterPoint + BraveMathCollege.DegreesToVector(spawnAngle, magnitude);
                ModifiedMetalGearRatSpinners1.CircleDummy circleDummy = new ModifiedMetalGearRatSpinners1.CircleDummy(this, this.CenterPoint, spawnRadius, spawnAngle, orbitSpeed, initialRadiusBoost);
                circleDummy.Position = vector;
                circleDummy.Direction = base.AimDirection;
                circleDummy.BulletManager = this.BulletManager;
                circleDummy.Initialize();
                this.m_circleDummies.Add(circleDummy);
                for (int i = 0; i < 9; i++)
                {
                    float angle = base.SubdivideCircle(0f, 9, i, 1f, false);
                    Vector2 vector2 = vector + BraveMathCollege.DegreesToVector(angle, 5f);
                    base.Fire(Offset.OverridePosition(vector2), new ModifiedMetalGearRatSpinners1.CircleBullet(this, circleDummy, rotationSpeed, i, vector2 - vector));
                }
            }

            private void SpawnCircleDebris(float spawnRadius, float spawnAngle, float orbitSpeed, float tRadius, float deltaAngle, float? initialRadiusBoost = null)
            {
                float angle = spawnAngle + deltaAngle;
                float num = Mathf.LerpUnclamped(spawnRadius - 5f, spawnRadius + 5f, tRadius);
                Vector2 overridePosition = this.CenterPoint + BraveMathCollege.DegreesToVector(angle, num + ((initialRadiusBoost == null) ? 0f : initialRadiusBoost.Value));
                base.Fire(Offset.OverridePosition(overridePosition), new ModifiedMetalGearRatSpinners1.OrbitBullet(this, num, angle, orbitSpeed, initialRadiusBoost));
            }

            private const int BulletsPerCircle = 21;

            private const float CircleRadius = 5f;

            private const int NearTimeForAttack = 100;

            private List<ModifiedMetalGearRatSpinners1.CircleDummy> m_circleDummies = new List<ModifiedMetalGearRatSpinners1.CircleDummy>();

            public class CircleDummy : Bullet
            {
                public CircleDummy(ModifiedMetalGearRatSpinners1 parent, Vector2 centerPoint, float centerRadius, float centerAngle, float orbitSpeed, float? initialRadiusBoostBoost = null) : base("UnDodgeableSpinner", false, false, false)
                {
                    this.m_parent = parent;
                    this.m_centerPoint = centerPoint;
                    this.m_centerRadius = centerRadius;
                    this.m_centerAngle = centerAngle;
                    this.m_orbitSpeed = orbitSpeed;
                    this.m_initialRadiusBoost = initialRadiusBoostBoost;
                }

                public override IEnumerator Top()
{
                    float radius = this.m_centerRadius;
                    base.ManualControl = true;
                    for (; ; )
                    {
                        float? initialRadiusBoost = this.m_initialRadiusBoost;
                        if (initialRadiusBoost != null && base.Tick <= 60)
                        {
                            radius = this.m_centerRadius + Mathf.Lerp(this.m_initialRadiusBoost.Value, 0f, (float)base.Tick / 60f);
                        }
                        else
                        {
                            this.m_centerAngle += this.m_orbitSpeed / 60f;
                        }
                        base.Position = this.m_centerPoint + BraveMathCollege.DegreesToVector(this.m_centerAngle, radius);
                        float playerDist = (this.BulletManager.PlayerPosition() - base.Position).magnitude;
                        if (playerDist < 5.5f || this.NearTime < 0)
                        {
                            this.NearTime++;
                        }
                        else
                        {
                            this.NearTime = Mathf.Max(0, this.NearTime - 2);
                        }
                        if (this.NearTime >= 100)
                        {
                            this.FireTick = base.Tick;
                            this.NearTime = -60;
                        }
                        yield return base.Wait(1);
                    }
                    //yield break;
                }

                public int NearTime;

                public int FireTick = -1;

                private ModifiedMetalGearRatSpinners1 m_parent;

                private ModifiedMetalGearRatSpinners1.CircleDummy m_circleDummy;

                private Vector2 m_centerPoint;

                private float m_centerRadius;

                private float m_centerAngle;

                private float m_orbitSpeed;

                private float? m_initialRadiusBoost;
            }

            public class CircleBullet : Bullet
            {
                public CircleBullet(ModifiedMetalGearRatSpinners1 parent, ModifiedMetalGearRatSpinners1.CircleDummy circleDummy, float rotationSpeed, int index, Vector2 offset) : base("UnDodgeableSpinner", false, false, false)
                {
                    this.m_parent = parent;
                    this.m_circleDummy = circleDummy;
                    this.m_rotationSpeed = rotationSpeed;
                    this.m_index = index;
                    this.m_offset = offset;
                }

                public override IEnumerator Top()
                {
                    this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
                    int remainingLife = -1;
                    bool isWarning = false;
                    base.ManualControl = true;
                    this.Projectile.specRigidbody.CollideWithTileMap = false;
                    this.Projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
                    while (!this.m_parent.Destroyed && remainingLife != 0)
                    {
                        if (this.m_parent.IsEnded || this.m_parent.Done)
                        {
                            if (remainingLife < 0)
                            {
                                remainingLife = UnityEngine.Random.Range(0, 60);
                            }
                            else
                            {
                                remainingLife--;
                            }
                        }
                        this.m_offset = this.m_offset.Rotate(this.m_rotationSpeed / 60f);
                        base.Position = this.m_circleDummy.Position + this.m_offset;
                        if (this.m_circleDummy.FireTick == base.Tick && remainingLife < 0)
                        {
                            Vector2 vector = this.m_circleDummy.Position - base.Position;
                            TimedBullet timedBullet = new TimedBullet(StaticBulletEntries.undodgeableSmallSpore.Name, 30);
                            base.Fire(new Direction(vector.ToAngle(), DirectionType.Absolute, -1f), new Speed(vector.magnitude * 2f, SpeedType.Absolute), timedBullet);
                            timedBullet.Projectile.IgnoreTileCollisionsFor(1f);
                        }
                        bool shouldWarn = false;
                        if (this.m_circleDummy.NearTime > 0 && remainingLife < 0)
                        {
                            shouldWarn = (this.m_circleDummy.NearTime + 30 >= 100);
                        }
                        if (shouldWarn && !isWarning)
                        {
                            tk2dSpriteAnimationClip defaultClip = this.Projectile.spriteAnimator.DefaultClip;
                            float clipStartTime = (float)this.m_circleDummy.NearTime / 60f % defaultClip.BaseClipLength;
                            this.Projectile.spriteAnimator.Play(defaultClip, clipStartTime, defaultClip.fps, false);
                            isWarning = true;
                        }
                        else if (!shouldWarn && isWarning)
                        {
                            this.Projectile.spriteAnimator.StopAndResetFrameToDefault();
                            isWarning = false;
                        }
                        yield return base.Wait(1);
                    }
                    base.Vanish(true);
                    yield break;
                }

                private ModifiedMetalGearRatSpinners1 m_parent;

                private ModifiedMetalGearRatSpinners1.CircleDummy m_circleDummy;

                private float m_rotationSpeed;

                private int m_index;

                private Vector2 m_offset;
            }

            public class OrbitBullet : Bullet
            {
                public OrbitBullet(ModifiedMetalGearRatSpinners1 parent, float radius, float angle, float orbitSpeed, float? initialRadiusBoost = null) : base("UnDodgeableSpinner", false, false, false)
                {
                    this.m_parent = parent;
                    this.m_radius = radius;
                    this.m_angle = angle;
                    this.m_orbitSpeed = orbitSpeed;
                    this.m_initialRadiusBoost = initialRadiusBoost;
                }

                public override IEnumerator Top()
                {
                    this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
                    float radius = this.m_radius;
                    int remainingLife = -1;
                    base.ManualControl = true;
                    this.Projectile.specRigidbody.CollideWithTileMap = false;
                    this.Projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
                    while (!this.m_parent.Destroyed && remainingLife != 0)
                    {
                        if (this.m_parent.IsEnded || this.m_parent.Done)
                        {
                            if (remainingLife < 0)
                            {
                                remainingLife = UnityEngine.Random.Range(0, 60);
                            }
                            else
                            {
                                remainingLife--;
                            }
                        }
                        float? initialRadiusBoost = this.m_initialRadiusBoost;
                        if (initialRadiusBoost != null && base.Tick <= 60)
                        {
                            radius = this.m_radius + Mathf.Lerp(this.m_initialRadiusBoost.Value, 0f, (float)base.Tick / 60f);
                        }
                        else
                        {
                            this.m_angle += this.m_orbitSpeed / 60f;
                        }
                        base.Position = this.m_parent.CenterPoint + BraveMathCollege.DegreesToVector(this.m_angle, radius + Mathf.SmoothStep(-0.25f, 0.25f, Mathf.PingPong((float)base.Tick, 30f) / 30f));
                        yield return base.Wait(1);
                    }
                    base.Vanish(true);
                    yield break;
                }

                private ModifiedMetalGearRatSpinners1 m_parent;

                private Vector2 m_centerPoint;

                private float m_radius;

                private float m_angle;

                private float m_orbitSpeed;

                private float? m_initialRadiusBoost;
            }
        }

        public class ModifiedMetalGearRatTailgun1 : Script
        {
            private bool Center { get; set; }

            private bool Done { get; set; }
            public override IEnumerator Top()
            {
                base.EndOnBlank = true;
                ModifiedMetalGearRatTailgun1.TargetDummy targetDummy = new ModifiedMetalGearRatTailgun1.TargetDummy();
                targetDummy.Position = base.BulletBank.aiActor.ParentRoom.area.UnitCenter + new Vector2(0f, 4.5f);
                targetDummy.Direction = base.AimDirection;
                targetDummy.BulletManager = this.BulletManager;
                targetDummy.Initialize();
                for (int j = 0; j < 16; j++)
                {
                    float angle = base.SubdivideCircle(0f, 16, j, 1f, false);
                    Vector2 overridePosition = targetDummy.Position + BraveMathCollege.DegreesToVector(angle, 0.75f);
                    base.Fire(Offset.OverridePosition(overridePosition), new ModifiedMetalGearRatTailgun1.TargetBullet(this, targetDummy, StaticBulletEntries.undodgeableDefault.Name));
                }
                base.Fire(Offset.OverridePosition(targetDummy.Position), new ModifiedMetalGearRatTailgun1.TargetBullet(this, targetDummy, StaticBulletEntries.undodgeableDefault.Name));
                for (int k = 0; k < 4; k++)
                {
                    float angle2 = (float)(k * 90);
                    for (int l = 1; l < 6; l++)
                    {
                        float magnitude = 0.75f + Mathf.Lerp(0f, 1.5f, (float)l / 6f);
                        Vector2 overridePosition2 = targetDummy.Position + BraveMathCollege.DegreesToVector(angle2, magnitude);
                        base.Fire(Offset.OverridePosition(overridePosition2), new ModifiedMetalGearRatTailgun1.TargetBullet(this, targetDummy, l == 5 ? StaticBulletEntries.undodgeableDefault.Name : "target"));
                    }
                }
                for (int i = 0; i < 360; i++)
                {
                    targetDummy.DoTick();
                    yield return base.Wait(1);
                }
                base.Fire(Offset.OverridePosition(targetDummy.Position + new Vector2(0f, 30f)), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new ModifiedMetalGearRatTailgun1.BigBullet());
                base.PostWwiseEvent("Play_BOSS_RatMech_Whistle_01", null);
                this.Center = true;
                yield return base.Wait(60);
                this.Done = true;
                yield return base.Wait(60);
                yield break;
            }

            private const int NumTargetBullets = 16;

            private const float TargetRadius = 3f;
            private const float TargetLegLength = 2.5f;

            public const int TargetTrackTime = 360;

            private const float TargetRotationSpeed = 80f;

            private const int BigOneHeight = 30;

            private const int NumDeathWaves = 4;

            private const int NumDeathBullets = 39;

            public class TargetDummy : Bullet
            {
                public TargetDummy() : base(null, false, false, false)
                {
                }

                public override IEnumerator Top()
                {
                    for (; ; )
                    {
                        float distToTarget = (this.BulletManager.PlayerPosition() - base.Position).magnitude;
                        if (base.Tick < 30)
                        {
                            this.Speed = 0f;
                        }
                        else
                        {
                            float a = Mathf.Lerp(10f, 3f, Mathf.InverseLerp(7f, 4f, distToTarget));
                            this.Speed = Mathf.Min(a, (float)(base.Tick - 30) / 60f * 10f);
                        }
                        base.ChangeDirection(new Direction(0f, DirectionType.Aim, 3f), 1);
                        yield return base.Wait(1);
                    }
                    //yield break;
                }
            }

            public class TargetBullet : Bullet
            {
                public TargetBullet(ModifiedMetalGearRatTailgun1 parent, ModifiedMetalGearRatTailgun1.TargetDummy targetDummy, string BulletName) : base(BulletName, false, false, false)
                {
                    this.m_parent = parent;
                    this.m_targetDummy = targetDummy;
                }

                public override IEnumerator Top()
                {
                    Vector2 toCenter = base.Position - this.m_targetDummy.Position;
                    float angle = toCenter.ToAngle();
                    float radius = toCenter.magnitude;
                    float deltaRadius = radius / 60f;
                    base.ManualControl = true;
                    this.Projectile.specRigidbody.CollideWithTileMap = false;
                    this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
                    while (!this.m_parent.Destroyed && !this.m_parent.IsEnded && !this.m_parent.Done)
                    {
                        if (base.Tick < 60)
                        {
                            radius += deltaRadius * 3f;
                        }
                        if (this.m_parent.Center)
                        {
                            radius -= deltaRadius * 2f;
                        }
                        angle += 1.166f;
                        base.Position = this.m_targetDummy.Position + BraveMathCollege.DegreesToVector(angle, radius);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    base.PostWwiseEvent("Play_BOSS_RatMech_Bomb_01", null);
                    yield break;
                }

                private ModifiedMetalGearRatTailgun1 m_parent;

                private ModifiedMetalGearRatTailgun1.TargetDummy m_targetDummy;
            }

            private class BigBullet : Bullet
            {
                public BigBullet() : base("UnDodgeableBigOne", false, false, false)
                {
                }

                public override void Initialize()
                {
                    this.Projectile.spriteAnimator.StopAndResetFrameToDefault();
                    base.Initialize();
                }

                public override IEnumerator Top()
                {
                    this.Projectile.specRigidbody.CollideWithTileMap = false;
                    this.Projectile.specRigidbody.CollideWithOthers = false;
                    yield return base.Wait(60);
                    this.Speed = 0f;
                    this.Projectile.spriteAnimator.Play();
                    float startingAngle = base.RandomAngle();
                    bool b = BraveUtility.RandomBool();
                    for (int i = 0; i < 4; i++)
                    {
                        bool flag = i % 2 == 0;
                        for (int j = 0; j < 36; j++)
                        {
                            bool g = j % 6 > 2;
                            if (b == true)
                            {
                                g = !g;
                            }
                            float startAngle = startingAngle;
                            int numBullets = 36;
                            int i2 = j;
                            bool offset = flag;
                            float direction = base.SubdivideCircle(startAngle, numBullets, i2, 1f, offset);

                            base.Fire(new Direction(direction, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet(g ? StaticBulletEntries.undodgeableDefault.Name : "target", 15f, (6 * i) + 30, -1));
                        }
                    }
                    yield return base.Wait(30);
                    base.Vanish(true);
                    yield break;
                }

                public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
                    if (preventSpawningProjectiles)
                    {
                        return;
                    }
                }
            }
        }
    }
}
