using System;
using System.Collections;
using System.Collections.Generic;
using Brave.BulletScript;
using Dungeonator;
using FullInspector;
using UnityEngine;
using ItemAPI;
using EnemyBulletBuilder;
using static Planetside.OldKingChanges;
using static Planetside.Ophanaim;

namespace Planetside
{


    public class OphanaimChanges : OverrideBehavior
    {
        public override string OverrideAIActorGUID => Ophanaim.guid; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
                                                                     // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
        public override void DoOverride()
        {
            actor.MovementSpeed *= 0.8f; // Doubles the enemy movement speed


            //ToolsEnemy.DebugInformation(behaviorSpec);

            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableFrogger);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBig);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBulletKingSlam);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableOldKingSuckBullet);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBig);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableSlam);


            OphanaimFlightBehavior ModifiedTHESUN = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as OphanaimFlightBehavior;
            ModifiedTHESUN.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedTHESUN));

            ShootBehavior SweepAttackModified = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[1].Behavior as ShootBehavior;
            SweepAttackModified.BulletScript = new CustomBulletScriptSelector(typeof(SweepAttackModified));

            ShootBehavior MithrixSlamTwoModified = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootBehavior;
            MithrixSlamTwoModified.BulletScript = new CustomBulletScriptSelector(typeof(MithrixSlamTwoModified));

            ShootBehavior MithrixSlamOmegaModified = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
            MithrixSlamOmegaModified.BulletScript = new CustomBulletScriptSelector(typeof(MithrixSlamOmegaModified));

            SequentialAttackBehaviorGroup SequentialAttackBehaviorGroup1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[6].Behavior as SequentialAttackBehaviorGroup;
            var SequentialAttackBehaviorGroup1List = SequentialAttackBehaviorGroup1.AttackBehaviors;
            var hSequentialAttackBehaviorGroup1Entry1 = SequentialAttackBehaviorGroup1List[1] as ShootBehavior;
            hSequentialAttackBehaviorGroup1Entry1.BulletScript = new CustomBulletScriptSelector(typeof(BlastAttackModified));

            SequentialAttackBehaviorGroup SequentialAttackBehaviorGroup2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[7].Behavior as SequentialAttackBehaviorGroup;
            var SequentialAttackBehaviorGroup2List = SequentialAttackBehaviorGroup2.AttackBehaviors;
            foreach (var t in SequentialAttackBehaviorGroup2List)
            {
                if (t is TeleportBehavior s)
                {
                    s.teleportInBulletScript = new CustomBulletScriptSelector(typeof(Teleport2Modified));
                    s.Cooldown += 1;
                }
            }

            SequentialAttackBehaviorGroup SequentialAttackBehaviorGroup3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[8].Behavior as SequentialAttackBehaviorGroup;
            var SequentialAttackBehaviorGroup3List = SequentialAttackBehaviorGroup3.AttackBehaviors;
            var hSequentialAttackBehaviorGroup3Entry1 = SequentialAttackBehaviorGroup3List[1] as CustomBeholsterLaserBehavior;
            hSequentialAttackBehaviorGroup3Entry1.BulletScript = new CustomBulletScriptSelector(typeof(FireSplitModified));

            var hSequentialAttackBehaviorGroup3Entry2 = SequentialAttackBehaviorGroup3List[3] as CustomBeholsterLaserBehavior;
            hSequentialAttackBehaviorGroup3Entry2.BulletScript = new CustomBulletScriptSelector(typeof(FireSplit720Modified));

            var hSequentialAttackBehaviorGroup3Entry3 = SequentialAttackBehaviorGroup3List[5] as CustomBeholsterLaserBehavior;
            hSequentialAttackBehaviorGroup3Entry3.BulletScript = new CustomBulletScriptSelector(typeof(FireSplit540Modified));
            var hSequentialAttackBehaviorGroup3Entry4 = SequentialAttackBehaviorGroup3List[7] as CustomBeholsterLaserBehavior;
            hSequentialAttackBehaviorGroup3Entry4.BulletScript = new CustomBulletScriptSelector(typeof(FireSplitFinaleModified));


        }
        public class ModifiedTHESUN : Script
        {
            public override IEnumerator Top()
            {
                yield return base.Wait(60);

                var room = this.BulletBank.aiActor.gameObject.GetComponent<EyeEnemyBehavior>().arena;
                var roomCenter = room.GetCenterCell();
                var star = StaticVFXStorage.MourningStarVFXController.SpawnMourningStar(roomCenter.ToCenterVector2());
                var bank = star.gameObject.AddComponent<AIBulletBank>();
                bank.Bullets = new List<AIBulletBank.Entry>()
                {
                    EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"),
                    suckLessEntry,
                    StaticUndodgeableBulletEntries.undodgeableBulletKingSlam
                };

                AdditionalBraveLight braveLight = star.gameObject.AddComponent<AdditionalBraveLight>();
                braveLight.transform.position = star.sprite.WorldCenter;
                braveLight.LightColor = new Color(1, 0.82f, 0.625f);
                braveLight.LightIntensity = 5f;
                braveLight.LightRadius = 0f;


                yield return base.Wait(20);
                var orbiter = GenerateOrbit(star.gameObject, 10);
                var orbiter2 = GenerateOrbitMinor(star.gameObject, -10);
                var orbiter3 = GenerateOrbitMinor(star.gameObject, -10);

                //base.PostWwiseEvent("Play_Burn", null);
                float m = 1;
                starObject = star.gameObject;
                star.OnBeamUpdate += (obj1, obj2) =>
                {
                    float gjkgj = obj2 - 0.33f;
                    float t = Mathf.Min(1, (gjkgj / 5));
                    Vector3 centerPoint = star.transform.position;
                    float a = (this.BulletManager.PlayerPosition() - new Vector2(centerPoint.x, centerPoint.y)).ToAngle();
                    float playerDist = (this.BulletManager.PlayerPosition() - star.transform.PositionVector2()).magnitude;
                    float dist = Mathf.Min(1, (Mathf.Min(12, playerDist) / 40));
                    dist *= t;
                    dist *= m;
                    star.transform.position = centerPoint + BraveMathCollege.DegreesToVector(a, 0.6f * dist).ToVector3ZisY();
                };
                yield return base.Wait(30);
                for (int i = 0; i < 1350; i++)
                {
                    float t = (float)i / (float)750;
                    orbiter.BulletMaxRadius = (float)Mathf.Lerp(0, 12, (float)t);
                    orbiter.BulletMinRadius = (float)Mathf.Lerp(0, 1, (float)t);
                    orbiter.Update();
                    if (i == 1050) { base.PostWwiseEvent("Play_ENM_CannonArmor_Charge_01", null); this.StartTask(DoBlast(braveLight)); }
                    if (i > 1050)
                    {
                        float ads = i - 1050;
                        float t2 = (float)ads / (float)240;
                        m = Mathf.Lerp(1, 0, MathToolbox.SinLerpTValue(t2));
                    }
                    if (i == 300) { base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null); }
                    if (i >= 300)
                    {
                        float ads = i - 300;
                        float t2 = (float)ads / (float)750;
                        orbiter2.BulletMaxRadius = (float)Mathf.Lerp(0, 9, (float)t2);
                        orbiter2.BulletMinRadius = (float)Mathf.Lerp(0, 1, (float)t2);
                        orbiter3.BulletMaxRadius = (float)Mathf.Lerp(0, 3.25f, (float)t2);
                        orbiter3.BulletMinRadius = (float)Mathf.Lerp(0, 1, (float)t2);
                    }

                    if (i % 60 == 0 && i < 1050)
                    {
                        base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
                        float startingDirection = UnityEngine.Random.Range(-120f, 120f);
                        Vector2 targetPos = base.GetPredictedTargetPosition((float)(((double)UnityEngine.Random.value >= 0.5) ? 1 : 0), 12f);
                        for (int j = 0; j < 4; j++)
                        {
                            base.Fire(Offset.OverridePosition(star.transform.PositionVector2()), new Direction(startingDirection, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new SnakeBullet(j * 4, targetPos));
                        }
                    }
                    yield return base.Wait(1);
                }
                star.Dissipate();
                yield break;
            }

            public GameObject starObject;

            public IEnumerator DoBlast(AdditionalBraveLight light)
            {
                for (int i = 0; i < 300; i++)
                {
                    float t = (float)i / (float)240;
                    if (i == 60 | i == 150 | i == 240)
                    {
                        GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(starObject.transform.PositionVector2(), Mathf.Lerp(1, 20, t), 0.075f, 100, 0.5f));
                        base.PostWwiseEvent("Play_Immolate", null);
                        base.PostWwiseEvent("Play_Immolate", null);

                    }
                    yield return base.Wait(1);
                    if (light)
                    { light.LightRadius = Mathf.Lerp(0, 20, MathToolbox.SinLerpTValue(t)); light.LightIntensity = Mathf.Lerp(2, 12, t); }

                }
                Vector2 pos = starObject.transform.PositionVector2();
                Exploder.DoDistortionWave(pos, 12f, 1f, 40, 0.7f);
                base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);
                base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);
                base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);

                bool b = BraveUtility.RandomBool();
                float E = BraveUtility.RandomAngle();
                for (int n = 0; n < 8; n++)
                {
                    for (int r = 0; r < 3; r++)
                    {
                        base.Fire(Offset.OverridePosition(pos), new Direction((45f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f + ((float)r / 6), SpeedType.Absolute), new BlastAttackModified.RotatingBullet(pos, b, (45f * n), 0.05f));
                        base.Fire(Offset.OverridePosition(pos), new Direction((45f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f + ((float)r / 6), SpeedType.Absolute), new BlastAttackModified.RotatingBullet(pos, b, (45f * n) + 4, 0.05f));
                        base.Fire(Offset.OverridePosition(pos), new Direction((45f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f + ((float)r / 6), SpeedType.Absolute), new BlastAttackModified.RotatingBullet(pos, b, (45f * n) - 4, 0.05f));
                    }
                }
                b = !b;
                yield return base.Wait(30);
                for (int n = 0; n < 8; n++)
                {
                    for (int r = 0; r < 3; r++)
                    {
                        base.Fire(Offset.OverridePosition(pos), new Direction((45f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6f + ((float)r / 6), SpeedType.Absolute), new BlastAttackModified.RotatingBullet(pos, b, (45f * n), 0.05f));
                        base.Fire(Offset.OverridePosition(pos), new Direction((45f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6f + ((float)r / 6), SpeedType.Absolute), new BlastAttackModified.RotatingBullet(pos, b, (45f * n) + 4, 0.05f));
                        base.Fire(Offset.OverridePosition(pos), new Direction((45f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6f + ((float)r / 6), SpeedType.Absolute), new BlastAttackModified.RotatingBullet(pos, b, (45f * n) - 4, 0.05f));
                    }
                }
                b = !b;
                yield return base.Wait(30);
                for (int n = 0; n < 8; n++)
                {
                    for (int r = 0; r < 3; r++)
                    {
                        base.Fire(Offset.OverridePosition(pos), new Direction((45f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(5f + ((float)r / 6), SpeedType.Absolute), new BlastAttackModified.RotatingBullet(pos, b, (45f * n), 0.05f));
                        base.Fire(Offset.OverridePosition(pos), new Direction((45f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(5f + ((float)r / 6), SpeedType.Absolute), new BlastAttackModified.RotatingBullet(pos, b, (45f * n) + 4, 0.05f));
                        base.Fire(Offset.OverridePosition(pos), new Direction((45f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(5f + ((float)r / 6), SpeedType.Absolute), new BlastAttackModified.RotatingBullet(pos, b, (45f * n) - 4, 0.05f));
                    }
                }
                yield break;
            }


            public SpinBulletsController GenerateOrbit(GameObject star, int speed)
            {
                SpinBulletsController spinBulletsController = star.AddComponent<SpinBulletsController>();
                spinBulletsController.ShootPoint = star.gameObject;
                spinBulletsController.OverrideBulletName = StaticUndodgeableBulletEntries.undodgeableBulletKingSlam.Name;
                spinBulletsController.NumBullets = 3;
                spinBulletsController.BulletMinRadius = 0.1f;
                spinBulletsController.BulletMaxRadius = 0f;
                spinBulletsController.BulletCircleSpeed = speed;
                spinBulletsController.BulletsIgnoreTiles = true;
                spinBulletsController.RegenTimer = 0.33f;
                spinBulletsController.AmountOFLines = 4;
                return spinBulletsController;
            }
            public SpinBulletsController GenerateOrbitMinor(GameObject star, int speed)
            {
                SpinBulletsController spinBulletsController = star.AddComponent<SpinBulletsController>();
                spinBulletsController.ShootPoint = star.gameObject;
                spinBulletsController.OverrideBulletName = StaticUndodgeableBulletEntries.undodgeableBulletKingSlam.Name;
                spinBulletsController.NumBullets = 2;
                spinBulletsController.BulletMinRadius = 0.1f;
                spinBulletsController.BulletMaxRadius = 0f;
                spinBulletsController.BulletCircleSpeed = speed;
                spinBulletsController.BulletsIgnoreTiles = true;
                spinBulletsController.RegenTimer = 0.33f;
                spinBulletsController.AmountOFLines = 4;
                return spinBulletsController;
            }

            public class RotatingBullet : Bullet
            {
                public RotatingBullet(Vector2 centerPoint, bool speeen, float startAngle, float spinspeed) : base(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false)
                {
                    this.centerPoint = centerPoint;
                    this.yesToSpeenOneWay = speeen;
                    this.startAngle = startAngle;
                    this.SpinSpeed = spinspeed;
                }
                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    float radius = Vector2.Distance(this.centerPoint, base.Position);
                    float speed = this.Speed;
                    float spinAngle = this.startAngle;
                    float spinSpeed = 0f;
                    for (int i = 0; i < 600; i++)
                    {
                        //speed += 0.08333f;
                        radius += speed / 60f;
                        if (yesToSpeenOneWay == true)
                        {
                            spinSpeed -= SpinSpeed;
                        }
                        else
                        {
                            spinSpeed += SpinSpeed;

                        }
                        spinAngle += spinSpeed / 60f;
                        base.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }
                public Vector2 centerPoint;
                public bool yesToSpeenOneWay;
                public float startAngle;
                public float SpinSpeed;
            }



            public class LingeringFlame : Bullet
            {
                public LingeringFlame() : base("frogger", false, false, false)
                {
                }

                public override IEnumerator Top()
                {
                    yield return this.Wait(600);
                    base.Vanish();
                    yield break;
                }
            }

            public class SnakeBullet : Bullet
            {
                public SnakeBullet(int delay, Vector2 target) : base("suck_more", false, false, false)
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
                        float offsetMagnitude = Mathf.SmoothStep(-0.45f, 0.45f, Mathf.PingPong(0.5f + (float)i / 60f * 3f, 1f));
                        if (i > 20 && i < 60 | i > 100 && i < 140 | i > 180 && i < 220)
                        {
                            float num = (this.target - truePosition).ToAngle();
                            float value = BraveMathCollege.ClampAngle180(num - this.Direction);
                            this.Direction += Mathf.Clamp(value, -6f, 6f);
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
        public class SweepAttackModified : Script
        {

            public bool fire;
            public float angle;

            public override IEnumerator Top()
            {

                float dur1 = 0;
                float dur2 = 0;
                fire = false;
                {
                    Vector2 vector2 = new Vector2();
                    Vector2 vector = this.Position;
                    int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.BulletBreakable);
                    var cast = RaycastToolbox.ReturnRaycast(this.Position, Vector2.right, rayMask, 1000, this.BulletBank.aiActor.specRigidbody);
                    vector2 = cast.Contact;
                    int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector, vector2)), 1);
                    num2 /= 5;
                    dur1 = 7.5f * num2;
                    for (int e = 0; e < num2; e++)
                    {
                        float t = (float)e / (float)num2;
                        Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
                        this.StartTask(this.SpawnFucker(vector3, 7.5f * e));
                    }
                }
                {
                    Vector2 vector2 = new Vector2();
                    Vector2 vector = this.Position;
                    int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.BulletBreakable);
                    var cast = RaycastToolbox.ReturnRaycast(this.Position, Vector2.left, rayMask, 1000, this.BulletBank.aiActor.specRigidbody);
                    vector2 = cast.Contact;
                    int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector, vector2)), 1);
                    num2 /= 5;

                    dur2 = 7.5f * num2;

                    for (int e = 0; e < num2; e++)
                    {
                        float t = (float)e / (float)num2;
                        Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
                        this.StartTask(this.SpawnFucker(vector3, 7.5f * e));
                    }
                }
                yield return this.Wait(Mathf.Max(dur1, dur2) + 20);
                fire = true;

                for (int e = 1; e < 101; e++)
                {
                    if (e % 20 == 0)
                    {
                        GlobalMessageRadio.BroadcastMessage("eye_gun_laser_blue");
                    }
                    float spare = 36;
                    float Angle = this.AimDirection;
                    if (Angle.IsBetweenRange(Vector2.left.ToAngle(), Vector2.left.ToAngle() + spare)) { Angle = Vector2.left.ToAngle() + spare; }
                    if (Angle.IsBetweenRange(Vector2.left.ToAngle() - spare, Vector2.left.ToAngle())) { Angle = Vector2.left.ToAngle() - spare; }
                    if (Angle.IsBetweenRange(Vector2.right.ToAngle(), Vector2.right.ToAngle() + spare)) { Angle = Vector2.right.ToAngle() + spare; }
                    if (Angle.IsBetweenRange(Vector2.right.ToAngle() - spare, Vector2.right.ToAngle())) { Angle = Vector2.right.ToAngle() - spare; }
                    angle = Angle;
                    yield return this.Wait(5);

                }
                yield break;
            }


            public IEnumerator SpawnFucker(Vector2 position, float delay)
            {
                yield return this.Wait(delay);
                StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(position);

                yield return this.Wait(60);
                //Play_Stomp
                base.PostWwiseEvent("Play_BOSS_agunim_deflect_01", null);//m_BOSS_agunim_deflect_01
                this.Fire(Offset.OverridePosition(position), new Direction(0, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new SummonerBall(this));
                yield break;
            }
            public class SummonerBall : Bullet
            {
                public SweepAttackModified parent;
                public SummonerBall(SweepAttackModified p) : base(StaticUndodgeableBulletEntries.undodgeableBig.Name, false, false, false)
                {
                    parent = p;
                }
                public override IEnumerator Top()
                {
                    int i = 0;
                    while (parent.IsEnded == false)
                    {
                        if (parent.fire == true)
                        {
                            if (i % 35 == 0)
                            {
                                base.PostWwiseEvent("Play_BOSS_doormimic_flame_01");
                                for (int q = 0; q < 2; q++)
                                {
                                    this.Fire(new Direction(parent.angle + (q * 180), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(2), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, 6, 120));
                                }
                            }
                        }
                        i++;
                        yield return this.Wait(1);

                    }
                    yield return this.Wait(BraveUtility.RandomAngle() / 3);
                    base.Vanish();
                    i++;
                    yield break;
                }
            }

        }
        public class MithrixSlamTwoModified : Script
        {
            public override IEnumerator Top()
            {
                for (int q = 0; q < 6; q++)
                {
                    base.PostWwiseEvent("Play_ENM_CannonArmor_Charge_01", null);
                    bool b = true;
                    float Dir = BraveUtility.RandomAngle();
                    float helpme = UnityEngine.Random.Range(180, -180);
                    float M = UnityEngine.Random.value < 0.5f ? 135 : -135;
                    for (int i = 0; i < 9; i++)
                    {

                        base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob((40 * i) + Dir, this, M, 1f, b));
                        b = !b;
                    }
                    GlobalMessageRadio.BroadcastMessage("eye_gun_simple_blue");

                    yield return this.Wait(75);
                }
                yield return this.Wait(120);
                yield break;
            }




            private IEnumerator QuickscopeNoob(float aimDir, MithrixSlamTwoModified parent, float rotSet, float chargeTime = 0.5f, bool PlaysAudio = false)
            {

                GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
                tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                component2.transform.position = this.Position;
                component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
                component2.dimensions = new Vector2(1f, 1f);
                component2.UpdateZDepth();
                component2.HeightOffGround = -2;


                Color laser = new Color(1, 0.85f, 0.7f);
                component2.sprite.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
                component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);


                float elapsed = 0;
                float Time = chargeTime;
                while (elapsed < Time)
                {
                    float t = (float)elapsed / (float)Time;

                    if (parent.IsEnded || parent.Destroyed)
                    {
                        UnityEngine.Object.Destroy(component2.gameObject);
                        yield break;
                    }
                    if (component2 != null)
                    {
                        float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
                        float Q = Mathf.Lerp(0, rotSet, throne1);
                        component2.transform.position = this.Position;
                        component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (75 * t));
                        component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
                        component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir + Q);
                        component2.HeightOffGround = -2;
                        component2.renderer.gameObject.layer = 23;
                        component2.dimensions = new Vector2(Mathf.Lerp(1, 750, throne1), 1f);
                        component2.UpdateZDepth();
                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }

                elapsed = 0;
                Time = 0.75f;
                StaticVFXStorage.HighPriestClapVFX.SpawnAtPosition(this.Position);
                while (elapsed < Time)
                {
                    if (parent.IsEnded || parent.Destroyed)
                    {
                        UnityEngine.Object.Destroy(component2.gameObject);
                        yield break;
                    }
                    if (component2 != null)
                    {
                        component2.transform.position = this.Position;
                        component2.dimensions = new Vector2(1000f, 1f);
                        component2.HeightOffGround = -2;
                        component2.renderer.gameObject.layer = 23;
                        component2.UpdateZDepth();

                        bool enabled = elapsed % 0.25f > 0.125f;
                        component2.sprite.renderer.enabled = enabled;

                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                UnityEngine.Object.Destroy(component2.gameObject);
                //base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
                if (PlaysAudio == true) { base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null); }

                base.Fire(new Direction(aimDir + rotSet, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(50f, SpeedType.Absolute), new Spawner());
                yield break;
            }

            public class Spawner : Bullet
            {
                public Spawner() : base("bigBullet", false, false, false)
                {

                }

                public override IEnumerator Top()
                {
                    for (int i = 0; i < 200; i++)
                    {
                        base.Fire(new Direction(UnityEngine.Random.Range(140, 200), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(0f, SpeedType.Absolute), new MithrixSlamTwo.BouncyFlame(80 - i));
                        yield return this.Wait(1f);
                    }
                    yield break;
                }
            }
            public class BouncyFlame : Bullet
            {
                public BouncyFlame(float timetilldie) : base("frogger", false, false, false)
                {
                    TimeTillDeath = timetilldie;
                }

                public override IEnumerator Top()
                {
                    this.ManualControl = true;
                    Vector2 truePosition = this.Position;

                    for (int i = 0; i < 80 - TimeTillDeath; i++)
                    {
                        this.UpdateVelocity();
                        truePosition += this.Velocity / 20f;
                        this.Position = truePosition + new Vector2(0f, Mathf.Sin((float)this.Tick / 30f / 0.75f * 3.14159274f) * 1.5f);
                        yield return this.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }
                private float TimeTillDeath;
            }

        }
        public class MithrixSlamOmegaModified : Script
        {
            public override IEnumerator Top()
            {
                for (int q = 0; q < 4; q++)
                {
                    base.PostWwiseEvent("Play_ENM_CannonArmor_Charge_01", null);
                    bool b = true;
                    float Dir = BraveUtility.RandomAngle();
                    float M = UnityEngine.Random.value < 0.5f ? 75 : -75;
                    bool masd = BraveUtility.RandomBool();
                    for (int i = 0; i < 6; i++)
                    {
                        base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob((60 * i) + Dir, this, M, 0.5f, b, masd));
                        b = !b;
                    }
                    yield return this.Wait(75);
                    GlobalMessageRadio.BroadcastMessage("eye_gun_predict_blue");
                    yield return this.Wait(90);
                }
                yield return this.Wait(120);
                yield break;
            }




            private IEnumerator QuickscopeNoob(float aimDir, MithrixSlamOmegaModified parent, float rotSet, float chargeTime = 0.5f, bool PlaysAudio = false, bool fcdfa = true)
            {

                GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
                tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                component2.transform.position = this.Position;
                component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
                component2.dimensions = new Vector2(1f, 1f);
                component2.UpdateZDepth();
                component2.HeightOffGround = -2;


                Color laser = new Color(1, 0.85f, 0.7f);
                component2.sprite.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
                component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);

                float elapsed = 0;
                float Time = chargeTime;
                while (elapsed < Time)
                {
                    float t = (float)elapsed / (float)Time;

                    if (parent.IsEnded || parent.Destroyed)
                    {
                        UnityEngine.Object.Destroy(component2.gameObject);
                        yield break;
                    }
                    if (component2 != null)
                    {
                        float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
                        float Q = Mathf.Lerp(0, rotSet, throne1);
                        component2.transform.position = this.Position;
                        component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (75 * t));
                        component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
                        component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir + Q);
                        component2.HeightOffGround = -2;
                        component2.renderer.gameObject.layer = 23;
                        component2.dimensions = new Vector2(Mathf.Lerp(1, 750, throne1), 1f);
                        component2.UpdateZDepth();
                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }

                elapsed = 0;
                Time = 0.75f;
                StaticVFXStorage.HighPriestClapVFX.SpawnAtPosition(this.Position);
                while (elapsed < Time)
                {
                    if (parent.IsEnded || parent.Destroyed)
                    {
                        UnityEngine.Object.Destroy(component2.gameObject);
                        yield break;
                    }
                    if (component2 != null)
                    {
                        component2.transform.position = this.Position;
                        component2.dimensions = new Vector2(1000f, 1f);
                        component2.HeightOffGround = -2;
                        component2.renderer.gameObject.layer = 23;
                        component2.UpdateZDepth();

                        bool enabled = elapsed % 0.25f > 0.125f;
                        component2.sprite.renderer.enabled = enabled;

                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                UnityEngine.Object.Destroy(component2.gameObject);
                if (PlaysAudio == true) { base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null); }

                base.Fire(new Direction(aimDir + rotSet, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(50f, SpeedType.Absolute), new Spawner(fcdfa));
                yield break;
            }

            public class Spawner : Bullet
            {
                public Spawner(bool h) : base("bigBullet", false, false, false)
                {
                    b = h;
                }

                public override IEnumerator Top()
                {
                    for (int i = 0; i < 45; i++)
                    {
                        base.Fire(new Direction(b == true ? 90 : -90, Brave.BulletScript.DirectionType.Relative, -1f), new Speed(0f, SpeedType.Absolute), new MithrixSlamOmega.BouncyFlame(45 - i));
                        yield return this.Wait(1f);
                    }
                    yield break;
                }
                private bool b;
            }
            public class BouncyFlame : Bullet
            {
                private float wait;
                public BouncyFlame(float t) : base("frogger", false, false, false)
                {
                    wait = t;
                }

                public override IEnumerator Top()
                {
                    yield return this.Wait(wait);
                    this.ChangeSpeed(new Brave.BulletScript.Speed(12, SpeedType.Absolute), 150);
                    yield break;
                }
            }
        }
        public class BlastAttackModified : Script
        {
            public virtual bool IsHard
            {
                get
                {
                    return false;
                }
            }

            public override IEnumerator Top()
            {
                //base.PostWwiseEvent("Play_ENM_gunknight_shockwave_01", null);

                GlobalMessageRadio.BroadcastMessage("eye_gun_laser");

                base.PostWwiseEvent("Play_BigSlam", null);
                base.PostWwiseEvent("Play_BigSlam", null);

                float h = BraveUtility.RandomBool() == true ? -120 : 120;
                float E = BraveUtility.RandomAngle();
                bool b = BraveUtility.RandomBool();

                for (int n = 0; n < 10; n++)
                {
                    base.Fire(new Direction((36f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new BlastAttackModified.RotatingBullet(base.Position, !b, (36f * n), 0.06f));
                    base.Fire(new Direction((36f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new BlastAttackModified.RotatingBullet(base.Position, b, (36f * n), 0.06f));
                    base.Fire(new Direction((36f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new BlastAttackModified.RotatingBullet(base.Position, !b, (36f * n), 0.06f));
                    base.Fire(new Direction((36f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new BlastAttackModified.RotatingBullet(base.Position, b, (36f * n), 0.06f));

                }
                yield break;
            }
            public class RotatingBullet : Bullet
            {
                public RotatingBullet(Vector2 centerPoint, bool speeen, float startAngle, float spinspeed) : base(StaticUndodgeableBulletEntries.UndodgeableFrogger.Name, false, false, false)
                {
                    this.centerPoint = centerPoint;
                    this.yesToSpeenOneWay = speeen;
                    this.startAngle = startAngle;
                    this.SpinSpeed = spinspeed;
                }
                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    float radius = Vector2.Distance(this.centerPoint, base.Position);
                    float speed = this.Speed;
                    float spinAngle = this.startAngle;
                    float spinSpeed = 0f;
                    for (int i = 0; i < 600; i++)
                    {
                        //speed += 0.08333f;
                        radius += speed / 60f;
                        if (yesToSpeenOneWay == true)
                        {
                            spinSpeed -= SpinSpeed;
                        }
                        else
                        {
                            spinSpeed += SpinSpeed;

                        }
                        spinAngle += spinSpeed / 60f;
                        base.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }
                public Vector2 centerPoint;
                public bool yesToSpeenOneWay;
                public float startAngle;
                public float SpinSpeed;
            }
        }
        public class Teleport2Modified : Script
        {
            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);
                GlobalMessageRadio.BroadcastMessage("eye_gun_laser_blue");
                float h = BraveUtility.RandomBool() == true ? -120 : 120;
                float E = BraveUtility.RandomAngle();
                bool b = BraveUtility.RandomBool();

                for (int n = 0; n < 10; n++)
                {
                    base.Fire(new Direction((36f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(4f, SpeedType.Absolute), new BlastAttackModified.RotatingBullet(base.Position, !b, (36f * n), 0.07f));
                    base.Fire(new Direction((36f * n) + (E), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new BlastAttackModified.RotatingBullet(base.Position, b, (36f * n), 0.07f));
                }

                yield break;
            }

        }

        public class FireSplit720Modified : FireSplitModified
        {
            public override float KillDelay
            {
                get
                {
                    return 540;
                }
            }
        }
        public class FireSplit540Modified : FireSplitModified
        {
            public override float KillDelay
            {
                get
                {
                    return 420;
                }
            }
        }

        public class FireSplitModified : Script
        {
            public virtual float KillDelay
            {
                get
                {
                    return 780;
                }
            }


            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);

                float pp = this.AimDirection;
                base.Fire(new Direction(75, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6.5f, SpeedType.Absolute), new RotatingBullet(base.Position, true, 60 + pp, 0.0833f, KillDelay));
                base.Fire(new Direction(165, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6.5f, SpeedType.Absolute), new RotatingBullet(base.Position, true, 150 + pp, 0.133f, KillDelay));
                base.Fire(new Direction(-165, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6.5f, SpeedType.Absolute), new RotatingBullet(base.Position, false, -150 + pp, 0.133f, KillDelay));
                base.Fire(new Direction(-75, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(6.5f, SpeedType.Absolute), new RotatingBullet(base.Position, false, -60 + pp, 0.0833f, KillDelay));


                AIBeamShooter2[] beams = this.BulletBank.aiActor.GetComponents<AIBeamShooter2>();
                yield return this.Wait(90);
                GlobalMessageRadio.BroadcastMessage("eye_gun_predict_blue");
                yield return this.Wait(30);

                int i = 0;
                bool b = false;
                while (beams != null || beams.Length > 0)
                {
                    if (b == false)
                    {
                        b = !b;

                        foreach (AIBeamShooter2 beam in beams)
                        {
                            if (beam && beam.LaserBeam)
                            {
                                Vector2 overridePosition = beam.LaserBeam.Origin + beam.LaserBeam.Direction.normalized * beam.MaxBeamLength;

                                Vector2 vector = beam.LaserBeam.Origin;

                                Vector2 vector2 = new Vector2();
                                Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;
                                int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.EnemyHitBox, CollisionLayer.PlayerHitBox);
                                RaycastResult raycastResult2;

                                Vector2 Point = MathToolbox.GetUnitOnCircle(beam.LaserBeam.Direction.ToAngle(), 1);
                                if (PhysicsEngine.Instance.Raycast(beam.LaserBeam.Origin, Point, 1000, out raycastResult2, true, false, rayMask2, null, false, rigidbodyExcluder, base.BulletBank.aiActor.specRigidbody))
                                {
                                    vector2 = raycastResult2.Contact;
                                }
                                RaycastResult.Pool.Free(ref raycastResult2);
                                int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector, vector2)), 1);
                                num2 /= 3;
                                for (int e = 0; e < num2; e++)
                                {
                                    float t = (float)e / (float)num2;
                                    Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() + 90, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new FireSplitModified.FireFire());
                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() - 90, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new FireSplitModified.FireFire());

                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() + 90, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(2), new FireSplitModified.FireFire());
                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() - 90, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(2), new FireSplitModified.FireFire());
                                }
                            }
                        }



                    }
                    i++;
                    yield return this.Wait(1);
                }

                i++;
                yield break;

            }
            public class RotatingBullet : Bullet
            {
                public RotatingBullet(Vector2 centerPoint, bool speeen, float startAngle, float spinspeed, float Killdelay) : base("bigBullet", false, false, false)
                {
                    this.centerPoint = centerPoint;
                    this.yesToSpeenOneWay = speeen;
                    this.startAngle = startAngle;
                    this.SpinSpeed = spinspeed;
                    this.kd = Killdelay;
                }
                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    float radius = Vector2.Distance(this.centerPoint, base.Position);
                    float speed = this.Speed;
                    float spinAngle = this.startAngle;
                    float spinSpeed = 0f;
                    for (int i = 0; i < 600; i++)
                    {
                        //speed += 0.08333f;
                        if (i % 6 == 0)
                        {
                            this.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Dissipate(kd));
                        }
                        radius += speed / 60f;
                        if (yesToSpeenOneWay == true)
                        {
                            spinSpeed -= SpinSpeed;
                        }
                        else
                        {
                            spinSpeed += SpinSpeed;

                        }
                        spinAngle += spinSpeed / 60f;
                        base.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }
                public Vector2 centerPoint;
                public bool yesToSpeenOneWay;
                public float startAngle;
                public float SpinSpeed;
                public float kd;

            }


            public class Dissipate : Bullet
            {
                private float killDelay;
                public Dissipate(float kd) : base("frogger", false, false, false)
                {
                    killDelay = kd;
                }
                public override IEnumerator Top()
                {
                    yield return base.Wait(killDelay);
                    base.Vanish(false);
                    yield break;
                }
            }

            public class FireFire : Bullet
            {

                public FireFire() : base(StaticUndodgeableBulletEntries.UndodgeableOldKingSuckBullet.Name, false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    this.Projectile.IgnoreTileCollisionsFor(90f);
                    yield return this.Wait(30);
                    base.ChangeSpeed(new Speed(10f, SpeedType.Absolute), 135);
                    yield return this.Wait(300);
                    base.Vanish(false);
                    yield break;
                }
            }
        }

        public class FireSplitFinaleModified : Script
        {

            public override void OnForceEnded()
            {
                base.OnForceEnded();
            }

            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);
                //float pp = this.AimDirection;
                //base.Fire(new Direction(75, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new RotatingBullet(base.Position, true, 60 + pp, 0.1f));
                //base.Fire(new Direction(165, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new RotatingBullet(base.Position, true, 150 + pp, 0.133f));
                //base.Fire(new Direction(-165, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new RotatingBullet(base.Position, false, -150 + pp, 0.133f));
                //base.Fire(new Direction(-75, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new RotatingBullet(base.Position, false, -60 + pp, 0.1f));

                AIBeamShooter2[] beams = this.BulletBank.aiActor.GetComponents<AIBeamShooter2>();
                yield return this.Wait(120);
                int i = 0;
                while (beams != null || beams.Length > 0)
                {
                    if (i % 60 == 0)
                    {
                        GlobalMessageRadio.BroadcastMessage("eye_gun_simple_blue");
                    }
                    if (i % 90 == 0)
                    {
                        GlobalMessageRadio.BroadcastMessage("eye_gun_predict_blue");

                        foreach (AIBeamShooter2 beam in beams)
                        {
                            if (beam && beam.LaserBeam)
                            {
                                Vector2 overridePosition = beam.LaserBeam.Origin + beam.LaserBeam.Direction.normalized * beam.MaxBeamLength;

                                Vector2 vector = beam.LaserBeam.Origin;

                                Vector2 vector2 = new Vector2();
                                Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;
                                int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.EnemyHitBox, CollisionLayer.PlayerHitBox);
                                RaycastResult raycastResult2;

                                Vector2 Point = MathToolbox.GetUnitOnCircle(beam.LaserBeam.Direction.ToAngle(), 1);
                                if (PhysicsEngine.Instance.Raycast(beam.LaserBeam.Origin, Point, 1000, out raycastResult2, true, false, rayMask2, null, false, rigidbodyExcluder, base.BulletBank.aiActor.specRigidbody))
                                {
                                    vector2 = raycastResult2.Contact;
                                }
                                RaycastResult.Pool.Free(ref raycastResult2);
                                int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector, vector2)), 1);
                                num2 /= 5;
                                for (int e = 0; e < num2; e++)
                                {
                                    float t = (float)e / (float)num2;
                                    Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() + 90, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new FireSplitModified.FireFire());
                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() - 90, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new FireSplitModified.FireFire());

                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() + 75, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new FireSplitModified.FireFire());
                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() - 75, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new FireSplitModified.FireFire());

                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() + 105, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new FireSplitModified.FireFire());
                                    this.Fire(Offset.OverridePosition(vector3), new Direction(beam.LaserBeam.Direction.ToAngle() - 105, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0), new FireSplitModified.FireFire());

                                }
                            }
                        }
                    }
                    i++;
                    yield return this.Wait(1);
                }
                i++;
                yield break;

            }

            public class RotatingBullet : Bullet
            {
                public RotatingBullet(Vector2 centerPoint, bool speeen, float startAngle, float spinspeed) : base("bigBullet", false, false, false)
                {
                    this.centerPoint = centerPoint;
                    this.yesToSpeenOneWay = speeen;
                    this.startAngle = startAngle;
                    this.SpinSpeed = spinspeed;
                }
                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    float radius = Vector2.Distance(this.centerPoint, base.Position);
                    float speed = this.Speed;
                    float spinAngle = this.startAngle;
                    float spinSpeed = 0f;
                    for (int i = 0; i < 600; i++)
                    {
                        //speed += 0.08333f;
                        if (i % 6 == 0)
                        {
                            this.Fire(new Direction(0, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Dissipate());
                        }
                        radius += speed / 60f;
                        if (yesToSpeenOneWay == true)
                        {
                            spinSpeed -= SpinSpeed;
                        }
                        else
                        {
                            spinSpeed += SpinSpeed;

                        }
                        spinAngle += spinSpeed / 60f;
                        base.Position = this.centerPoint + BraveMathCollege.DegreesToVector(spinAngle, radius);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }
                public Vector2 centerPoint;
                public bool yesToSpeenOneWay;
                public float startAngle;
                public float SpinSpeed;
            }
            public class Dissipate : Bullet
            {
                public Dissipate() : base("frogger", false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    yield return base.Wait(300);
                    base.Vanish(false);
                    yield break;
                }
            }

            public class FireFire : Bullet
            {

                public FireFire() : base("suck", false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    this.Projectile.IgnoreTileCollisionsFor(300f);
                    yield return this.Wait(30);
                    base.ChangeSpeed(new Speed(11f, SpeedType.Absolute), 120);
                    yield return this.Wait(300);
                    base.Vanish(false);
                    yield break;
                }
            }
        }


    }
}
