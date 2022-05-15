using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using Brave.BulletScript;
using Dungeonator;

namespace Planetside
{
    internal class PrisonerSecondSubPhaseController : BraveBehaviour
    {
        public void Start()
        {
            SubPhaseEnded = false;
            SubPhaseActivated = false;
            Actor = base.aiActor;
            if (Actor != null)
            {
                Controller = Actor.GetComponent<PrisonerPhaseOne.PrisonerController>();
                FirstSubPhaseController = Actor.GetComponent<PrisonerFirstSubPhaseController>();
            }
        }   
        public void Update()
        {
            if (Actor.healthHaver.GetCurrentHealth() == Actor.healthHaver.minimumHealth && SubPhaseActivated != true && FirstSubPhaseController.IsSubPhaseEnded() == true)
            {
                if (Actor.IsBlackPhantom == true) { WasJammed = true; }
                SubPhaseActivated = true;
                StaticReferenceManager.DestroyAllEnemyProjectiles();
                Actor.behaviorSpeculator.InterruptAndDisable();
                Actor.StartCoroutine(DoTransition());
            }
        }
        private IEnumerator DoTransition()
        {
            Exploder.DoDistortionWave(Actor.sprite.WorldCenter, 4, 0.2f, 50, 3f);
            Actor.specRigidbody.enabled = false;
            int lay = Actor.gameObject.layer;

            float elaWait = 0f;
            while (elaWait < 1f)
            {
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            Controller.MoveTowardsCenterMethod(3f);
            Actor.aiAnimator.PlayUntilFinished("subphaseoneanimation", true, null, -1f, false);
            Actor.renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
            if (WasJammed == true)
            {
                GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, Actor.sprite.WorldBottomLeft, Quaternion.identity, false);
                if (gameObject && gameObject.GetComponent<tk2dSprite>())
                {
                    tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                    component.scale *= 2.5f;
                    component.HeightOffGround = 5f;
                    component.UpdateZDepth();
                }
                Destroy(gameObject, 2);
            }
            elaWait = 0f;
            
            GameManager.Instance.BestActivePlayer.CurrentRoom.BecomeTerrifyingDarkRoom(5f, 0.5f, 0.1f, "Play_ENM_darken_world_01");
           
            SpawnManager.SpawnBulletScript(Actor, Actor.sprite.WorldCenter, Actor.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(SubphaseTwoAttack)), StringTableManager.GetEnemiesString("#PRISONERPHASEONENAME", -1));
            while (elaWait < 3f)
            {
                float t = elaWait / 3;
                Actor.renderer.material.SetFloat("_Fade", 1-t);
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            Actor.SetOutlines(false);
            Actor.renderer.enabled = false;
            elaWait = 0f;
            while (elaWait < 30f)
            {
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            
            SubPhaseEnded = true;
            GameManager.Instance.BestActivePlayer.CurrentRoom.EndTerrifyingDarkRoom(2.5f);
            ImprovedAfterImage yeah = Actor.gameObject.GetComponent<ImprovedAfterImage>();
            yeah.spawnShadows = false;
            Actor.gameObject.layer = lay;
            for (int j = 0; j < Actor.behaviorSpeculator.AttackBehaviors.Count; j++)
            {
                if (base.behaviorSpeculator.AttackBehaviors[j] is AttackBehaviorGroup && base.behaviorSpeculator.AttackBehaviors[j] != null)
                {
                    this.ProcessAttackGroup(base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup);
                }
            }
            elaWait = 0f;
            while (elaWait < 3f)
            {
                float t = elaWait / 3;
                Actor.renderer.material.SetFloat("_Fade", t);
                elaWait += BraveTime.DeltaTime;
                Actor.renderer.enabled = true;
                yield return null;
            }

            if (WasJammed == true)
            {
                GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, Actor.sprite.WorldBottomLeft, Quaternion.identity, false);
                if (gameObject && gameObject.GetComponent<tk2dSprite>())
                {
                    tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                    component.scale *= 2.5f;
                    component.HeightOffGround = 5f;
                    component.UpdateZDepth();
                }
                Destroy(gameObject, 2);
                Actor.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
            }

            Actor.SetOutlines(true);
            Controller.CurrentSubPhase = PrisonerPhaseOne.PrisonerController.SubPhases.PHASE_3;
            Actor.behaviorSpeculator.enabled = true;
            Actor.specRigidbody.enabled = true;
            Actor.healthHaver.minimumHealth = 1;

            yield break;
        }

        private void ProcessAttackGroup(AttackBehaviorGroup attackGroup)
        {
            for (int i = 0; i < attackGroup.AttackBehaviors.Count; i++)
            {
                AttackBehaviorGroup.AttackGroupItem attackGroupItem = attackGroup.AttackBehaviors[i];
                if (attackGroup != null && attackGroupItem.NickName.Contains("One"))
                {
                    attackGroupItem.Probability = 0f;
                }
                if (attackGroup != null && attackGroupItem.NickName.Contains("Two"))
                {
                    float val = 1;
                    AttackNamesAndProbabilities.TryGetValue(attackGroupItem.NickName, out val);
                    attackGroupItem.Probability = val;
                }
            }
        }

        private static Dictionary<string, float> AttackNamesAndProbabilities = new Dictionary<string, float>()
        {
            {"SimpleBlastsTwo", 0.8f },
            {"WallSweepTwo", 1f },
            {"LaserCrossTwo", 2 },
            {"SweepJukeAttackTwo", 0.9f },
            {"BasicLaserAttackTellTwo", 1.2f },
            {"ChainRotatorsTwo", 0.8f },
        };


        public bool WasJammed;
        public AIActor Actor;
        public PrisonerPhaseOne.PrisonerController Controller;
        public PrisonerFirstSubPhaseController FirstSubPhaseController;
        public bool SubPhaseActivated;
        public static bool SubPhaseEnded;

        public class SubphaseTwoAttack : Script
        {

            private bool Center;

            private bool Done;

            protected override IEnumerator Top()
            {
                Center = false;
                Done = false;
                this.EndOnBlank = false;
                Vector2 spawnPos = base.BulletBank.aiActor.ParentRoom.GetCenterCell().ToCenterVector2();
                if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
                {
                    base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
                    base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBig);
                }
                this.EndOnBlank = false;
                this.SpawnRingOfHell();
             
                
                int e = 0;
                for (; ; )
                {
                    if (SubPhaseEnded == true) { this.Destroyed = true; yield break; }

                    e++;
                    yield return base.Wait(1);
                }
            }

            public void SpawnRingOfHell()
            {
                SubphaseTwoAttack.TargetDummy targetDummy = new SubphaseTwoAttack.TargetDummy(this);
                targetDummy.Position = this.BulletBank.aiActor.ParentRoom.area.UnitCenter;
                targetDummy.Direction = this.AimDirection;
                targetDummy.BulletManager = this.BulletManager;
                targetDummy.Initialize();
                this.Fire(Offset.OverridePosition(targetDummy.Position), targetDummy);



                for (int j = 0; j < 30; j++)
                {
                    float angle = this.SubdivideCircle(0f, 30, j, 1f, false);
                    this.Fire(Offset.OverridePosition(targetDummy.Position + BraveMathCollege.DegreesToVector(angle, 0.625f)), new SubphaseTwoAttack.TargetBullet(this, targetDummy));
                }
                this.Fire(Offset.OverridePosition(targetDummy.Position), new SubphaseTwoAttack.TargetBullet(this, targetDummy));
            }

            public class TargetDummy : Bullet
            {
                public TargetDummy(SubphaseTwoAttack parent) : base("undodgeableBig", false, false, false)
                {
                    this.parent = parent;
                }

                protected override IEnumerator Top()
                {
                    this.Projectile.ImmuneToBlanks = true;
                    this.Projectile.ImmuneToSustainedBlanks = true;
                    int i = 0;
                    for (; ; )
                    {
                        float distToTarget = (this.BulletManager.PlayerPosition() - this.Position).magnitude;              
                        {
                            float a = Mathf.Lerp(12f, 4f, Mathf.InverseLerp(7f, 4f, distToTarget));
                            this.Speed = Mathf.Min(a, (float)(this.Tick - 30) / 60f * 10f); 
                        }
                        this.ChangeDirection(new Direction(0f, DirectionType.Aim, 3f), 1);
                        i++;
                    yield return this.Wait(1);
                    }
                }

                public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
                    parent.SpawnRingOfHell();
                    base.OnBulletDestruction(destroyType, hitRigidbody, preventSpawningProjectiles);
                }
                private SubphaseTwoAttack parent;
            }

            public class TargetBullet : Bullet
            {
                public TargetBullet(SubphaseTwoAttack parent, SubphaseTwoAttack.TargetDummy targetDummy) : base("undodgeableDefault", false, false, false)
                {
                    this.m_parent = parent;
                    this.m_targetDummy = targetDummy;
                }

                protected override IEnumerator Top()
                {
                    this.Projectile.ImmuneToBlanks = true;
                    this.Projectile.ImmuneToSustainedBlanks = true;
                    Vector2 toCenter = this.Position - this.m_targetDummy.Position;
                    float angle = toCenter.ToAngle();
                    float radius = toCenter.magnitude;
                    float deltaRadius = radius / 60f;
                    this.ManualControl = true;
                    this.Projectile.specRigidbody.CollideWithTileMap = false;
                    this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
                    while (!this.m_parent.Destroyed && !this.m_parent.IsEnded && this.m_parent.Done == false)
                    {
                        if (m_targetDummy == null) { this.Vanish(false); yield break; }

                        if (this.Tick < 60)
                        {
                            radius += deltaRadius * 3f;
                        }
                        if (this.m_parent.Center)
                        {
                            radius -= deltaRadius * 2f;
                        }
                        angle += 1.3333334f;
                        this.Position = this.m_targetDummy.Position + BraveMathCollege.DegreesToVector(angle, radius);
                        yield return this.Wait(1);
                    }
                    this.Vanish(false);
                    //this.PostWwiseEvent("Play_BOSS_RatMech_Bomb_01", null);
                    yield break;
                }

                // Token: 0x04000A2D RID: 2605
                private SubphaseTwoAttack m_parent;

                // Token: 0x04000A2E RID: 2606
                private SubphaseTwoAttack.TargetDummy m_targetDummy;
            }
        


            /*
            public class SubphaseTwoAttack : Script
            {
                //public static float CurrentRingRadius;

                private bool Done;
                private Vector2 CenterPoint;

                public void SetCenterpoint(Vector2 vector2)
                {
                    CenterPoint = vector2;
                }

                public void SetDone(bool boolean)
                {
                    Done = boolean;
                }

                protected override IEnumerator Top()
                {
                    this.EndOnBlank = false;
                    Vector2 spawnPos = base.BulletBank.aiActor.ParentRoom.GetCenterCell().ToCenterVector2();
                    if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
                    {
                        base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
                        base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
                    }
                    yield return this.Wait(30);
                    this.EndOnBlank = true;
                    SetCenterpoint(this.BulletBank.aiActor.ParentRoom.area.UnitCenter);
                    m_circleDummies = new List<CircleDummy>();

                    this.m_circleDummies.Clear();
                    this.StartTask(this.SpawnInnerRing());

                    int spinTime = 960;
                    for (int i = 0; i < spinTime; i++)
                    {
                        for (int j = 0; j < this.m_circleDummies.Count; j++)
                        {
                            this.m_circleDummies[j].DoTick();
                        }
                        if (i == spinTime - 60)
                        {
                            SetDone(true);
                        }
                        yield return this.Wait(1);
                    }

                    int e = 0;
                    for (; ; )
                    {
                        if (SubPhaseEnded == true) { this.Destroyed = true; yield break; }

                        e++;
                        yield return base.Wait(1);
                    }
                    /*
                    int i = 0;
                    for (; ; )
                    {
                        if (SubPhaseEnded == true) { this.Destroyed = true; yield break; }
                        if (i % 20 == 1)
                        {
                            for (int e = 0; e < 8; e++)
                            {
                                //base.Fire(Offset.OverridePosition(spawnPos), new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SubphaseTwoAttack.RotatedBulletBasic(10, 0, 0, "undodgeableDefault", this, (e * 45) + (i / 2), 0.05f));
                                //base.Fire(Offset.OverridePosition(spawnPos), new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SubphaseTwoAttack.RotatedBulletBasic(-10, 0, 0, "undodgeableDefault", this, (e * 45) - (i/2), 0.05f));
                            }
                        }

                        i++;
                        yield return base.Wait(1);
                    }
                    
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
                this.SpawnCircleDebris(radius, 130f, 0f, 0.37f, 0f, new float?(initialRadiusBoost));
                this.SpawnCircleDebris(radius, 120f, 0f, 0.31f, 0f, new float?(initialRadiusBoost));
                this.SpawnCircleDebris(radius, 110f, 0f, 0.23f, 0f, new float?(initialRadiusBoost));
                this.SpawnCircleDebris(radius, 100f, 0f, 0.27f, 0f, new float?(initialRadiusBoost));
                this.SpawnCircleDebris(radius, 90f, 0f, 0.29f, 0f, new float?(initialRadiusBoost));
                this.SpawnCircleDebris(radius, 80f, 0f, 0.27f, 0f, new float?(initialRadiusBoost));
                this.SpawnCircleDebris(radius, 70f, 0f, 0.23f, 0f, new float?(initialRadiusBoost));
                this.SpawnCircleDebris(radius, 60f, 0f, 0.31f, 0f, new float?(initialRadiusBoost));
                this.SpawnCircleDebris(radius, 50f, 0f, 0.37f, 0f, new float?(initialRadiusBoost));
                this.SpawnCircleDebris(radius, 40f, 0f, 0.35f, 0f, new float?(initialRadiusBoost));
                for (int i = 0; i < numCircles; i++)
                {
                    float spawnAngle = 90f;
                    this.SpawnCircle(radius, spawnAngle, orbitSpeed, rotationSpeed, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.95f, -50f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.95f, -30f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.95f, 30f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.95f, 50f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.78f, -40f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.78f, 60f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.78f, 40f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.6f, -50f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.6f, 50f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.45f, 60f, new float?(initialRadiusBoost));
                    this.SpawnCircleDebris(radius, spawnAngle, orbitSpeed, 0.25f, 60f, new float?(initialRadiusBoost));
                    yield return this.Wait(spinTime / numCircles);
                }
                yield break;
            }

            private void SpawnCircleDebris(float spawnRadius, float spawnAngle, float orbitSpeed, float tRadius, float deltaAngle, float? initialRadiusBoost = null)
            {
                float angle = spawnAngle + deltaAngle;
                float num = Mathf.LerpUnclamped(spawnRadius - 5f, spawnRadius + 5f, tRadius);
                Vector2 overridePosition = this.CenterPoint + BraveMathCollege.DegreesToVector(angle, num + ((initialRadiusBoost == null) ? 0f : initialRadiusBoost.Value));
                base.Fire(Offset.OverridePosition(overridePosition), new SubphaseTwoAttack.OrbitBullet(this, num, angle, orbitSpeed, initialRadiusBoost));
            }
            public class WallBulletNoDodge : Bullet
            {
                public WallBulletNoDodge(string BulletType) : base(BulletType, false, false, false)
                {
                }
                protected override IEnumerator Top()
                {
                    this.Projectile.IgnoreTileCollisionsFor(300f);
                    this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));

                    this.Projectile.ImmuneToBlanks = true;
                    this.Projectile.ImmuneToSustainedBlanks = true;

                    this.Projectile.pierceMinorBreakables = true;
                    yield break;
                }
            }


            public class RotatedBulletBasic : Bullet
            {
                public RotatedBulletBasic(float spinspeed, float RevUp, float StartSpeenAgain, string BulletType, SubphaseTwoAttack parent, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
                {
                    this.m_spinSpeed = spinspeed;
                    this.TimeToRevUp = RevUp;
                    this.StartAgain = StartSpeenAgain;

                    this.m_parent = parent;
                    this.m_angle = angle;
                    this.m_radius = aradius;
                    this.m_bulletype = BulletType;
                    this.SuppressVfx = true;
                }

                protected override IEnumerator Top()
                {
                    base.ManualControl = true;
                    this.Projectile.collidesOnlyWithPlayerProjectiles = true;
                    this.Projectile.collidesWithProjectiles = true;
                    this.Projectile.UpdateCollisionMask();
                    Vector2 centerPosition = base.Position;
                    float radius = 0f;
                    for (int i = 0; i < 2400; i++)
                    {
                        if (m_parent.IsEnded || m_parent.Destroyed) { this.Projectile.DieInAir(false); yield break; }
                        radius += m_radius;
                        centerPosition += this.Velocity / 60f;
                        base.UpdateVelocity();
                        this.m_angle += this.m_spinSpeed / 60f;
                        base.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }


                private const float ExpandSpeed = 4.5f;
                private const float SpinSpeed = 40f;
                private SubphaseTwoAttack m_parent;
                private float m_angle;
                private float m_spinSpeed;
                private float m_radius;
                private string m_bulletype;
                private float TimeToRevUp;
                private float StartAgain;


            }

            private void SpawnCircle(float spawnRadius, float spawnAngle, float orbitSpeed, float rotationSpeed, float? initialRadiusBoost = null)
            {
                float magnitude = spawnRadius + ((initialRadiusBoost == null) ? 0f : initialRadiusBoost.Value);
                Vector2 vector = this.CenterPoint + BraveMathCollege.DegreesToVector(spawnAngle, magnitude);
                SubphaseTwoAttack.CircleDummy circleDummy = new SubphaseTwoAttack.CircleDummy(this, this.CenterPoint, spawnRadius, spawnAngle, orbitSpeed, initialRadiusBoost);
                circleDummy.Position = vector;
                circleDummy.Direction = base.AimDirection;
                circleDummy.BulletManager = this.BulletManager;
                circleDummy.Initialize();
                this.m_circleDummies.Add(circleDummy);
                for (int i = 0; i < 21; i++)
                {
                    float angle = base.SubdivideCircle(0f, 21, i, 1f, false);
                    Vector2 vector2 = vector + BraveMathCollege.DegreesToVector(angle, 5f);
                    base.Fire(Offset.OverridePosition(vector2), new SubphaseTwoAttack.CircleBullet(this, circleDummy, rotationSpeed, i, vector2 - vector));
                }
            }
            private List<SubphaseTwoAttack.CircleDummy> m_circleDummies;


            public class CircleDummy : Bullet
            {
                public CircleDummy(SubphaseTwoAttack parent, Vector2 centerPoint, float centerRadius, float centerAngle, float orbitSpeed, float? initialRadiusBoostBoost = null)
                {
                    this.FireTick = -1;
                    //base..ctor("spinner", false, false, false);
                    this.m_parent = parent;
                    this.m_centerPoint = centerPoint;
                    this.m_centerRadius = centerRadius;
                    this.m_centerAngle = centerAngle;
                    this.m_orbitSpeed = orbitSpeed;
                    this.m_initialRadiusBoost = initialRadiusBoostBoost;
                }
                protected override IEnumerator Top()
                {
                    float radius = this.m_centerRadius;
                    this.ManualControl = true;
                    for (; ; )
                    {
                        float? initialRadiusBoost = this.m_initialRadiusBoost;
                        if (initialRadiusBoost != null && this.Tick <= 60)
                        {
                            radius = this.m_centerRadius + Mathf.Lerp(this.m_initialRadiusBoost.Value, 0f, (float)this.Tick / 60f);
                        }
                        else
                        {
                            this.m_centerAngle += this.m_orbitSpeed / 60f;
                        }
                        this.Position = this.m_centerPoint + BraveMathCollege.DegreesToVector(this.m_centerAngle, radius);
                        float playerDist = (this.BulletManager.PlayerPosition() - this.Position).magnitude;
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
                            this.FireTick = this.Tick;
                            this.NearTime = -60;
                        }
                        yield return this.Wait(1);
                    }
                }

                public int NearTime;

                public int FireTick;

                private SubphaseTwoAttack m_parent;

                private SubphaseTwoAttack.CircleDummy m_circleDummy;

                private Vector2 m_centerPoint;

                private float m_centerRadius;

                private float m_centerAngle;

                private float m_orbitSpeed;

                private float? m_initialRadiusBoost;
            }

            public class CircleBullet : Bullet
            {
                public CircleBullet(SubphaseTwoAttack parent, SubphaseTwoAttack.CircleDummy circleDummy, float rotationSpeed, int index, Vector2 offset) : base("undodgeableDefault", false, false, false)
                {
                    this.m_parent = parent;
                    this.m_circleDummy = circleDummy;
                    this.m_rotationSpeed = rotationSpeed;
                    this.m_index = index;
                    this.m_offset = offset;
                }

                protected override IEnumerator Top()
                {
                    this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
                    int remainingLife = -1;
                    bool isWarning = false;
                    this.ManualControl = true;
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
                        this.Position = this.m_circleDummy.Position + this.m_offset;
                        if (this.m_circleDummy.FireTick == this.Tick && remainingLife < 0)
                        {
                            Vector2 vector = this.m_circleDummy.Position - this.Position;
                            TimedBullet timedBullet = new TimedBullet(30);
                            this.Fire(new Direction(vector.ToAngle(), DirectionType.Absolute, -1f), new Speed(vector.magnitude * 2f, SpeedType.Absolute), timedBullet);
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
                        yield return this.Wait(1);
                    }
                    this.Vanish(true);
                    yield break;
                }

                private SubphaseTwoAttack m_parent;

                private SubphaseTwoAttack.CircleDummy m_circleDummy;

                private float m_rotationSpeed;

                private int m_index;

                private Vector2 m_offset;
            }



            public class OrbitBullet : Bullet
            {
                public OrbitBullet(SubphaseTwoAttack parent, float radius, float angle, float orbitSpeed, float? initialRadiusBoost = null) : base("undodgeableDefault", false, false, false)
                {
                    this.m_parent = parent;
                    this.m_radius = radius;
                    this.m_angle = angle;
                    this.m_orbitSpeed = orbitSpeed;
                    this.m_initialRadiusBoost = initialRadiusBoost;
                }

                protected override IEnumerator Top()
                {
                    this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
                    float radius = this.m_radius;
                    int remainingLife = -1;
                    this.ManualControl = true;
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
                        if (initialRadiusBoost != null && this.Tick <= 60)
                        {
                            radius = this.m_radius + Mathf.Lerp(this.m_initialRadiusBoost.Value, 0f, (float)this.Tick / 60f);
                        }
                        else
                        {
                            this.m_angle += this.m_orbitSpeed / 60f;
                        }
                        this.Position = this.m_parent.CenterPoint + BraveMathCollege.DegreesToVector(this.m_angle, radius + Mathf.SmoothStep(-0.25f, 0.25f, Mathf.PingPong((float)this.Tick, 30f) / 30f));
                        yield return this.Wait(1);
                    }
                    this.Vanish(true);
                    yield break;
                }

                private SubphaseTwoAttack m_parent;

                private Vector2 m_centerPoint;

                private float m_radius;

                private float m_angle;

                private float m_orbitSpeed;

                private float? m_initialRadiusBoost;


            }
        }
        */
        }
    }
}
