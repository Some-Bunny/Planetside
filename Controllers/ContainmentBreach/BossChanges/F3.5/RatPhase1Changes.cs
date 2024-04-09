using System;
using System.Collections;
using System.Collections.Generic;
using Brave.BulletScript;
using Dungeonator;
using FullInspector;
using UnityEngine;
using ItemAPI;
using EnemyBulletBuilder;
using static Planetside.AdvancedDragunChanges;

namespace Planetside
{


	public class RatPhase1Changes : OverrideBehavior
	{
		public override string OverrideAIActorGUID => Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
                                                                                                       // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
        public override void DoOverride()
		{
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableCannonBullet);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableSlam);
			actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableGroundDefault);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnDodgeableCheese);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnDodgeableDagger);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnDodgeableTailProj);

            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnDodgeablecheeseWedge0);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnDodgeablecheeseWedge1);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnDodgeablecheeseWedge2);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnDodgeablecheeseWedge3);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnDodgeablecheeseWedge4);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnDodgeablecheeseWedge5);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnDodgeablecheeseWedge6);
            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnDodgeablecheeseWedge7);

            actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UnDodgeableCheeseWheel);


            ShootBehavior ShootBehavior1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[0].Behavior as ShootBehavior;
            ShootBehavior1.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedResourcefulRatSpinFire1));

            ShootBehavior ShootBehavior2 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[2].Behavior as ShootBehavior;
            ShootBehavior2.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedResourcefulRatDaggers1));

            ShootBehavior ShootBehavior3 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[3].Behavior as ShootBehavior;
            ShootBehavior3.BulletScript = new CustomBulletScriptSelector(typeof(ModifiedResourcefulRatDaggers1Quick));

            SequentialAttackBehaviorGroup Seq_1 = behaviorSpec.AttackBehaviorGroup.AttackBehaviors[5].Behavior as SequentialAttackBehaviorGroup;
            (Seq_1.AttackBehaviors[1] as ShootBehavior).BulletScript = new CustomBulletScriptSelector(typeof(ModifuedResourcefulRatCheeseWheel1));

        }
        public class ModifiedResourcefulRatSpinFire1 : Script
        {
            public override IEnumerator Top()
            {
                yield return base.Wait(5);
                float deltaAngle = 15.652174f * 1.5f;
                float deltaT = 3.0434783f;
                float t = 0f;
                int i = 0;
                while ((float)i < 16)
                {
                    float angle = -90f - (float)i * deltaAngle;
                    for (t += deltaT; t > 1f; t -= 1f)
                    {
                        yield return base.Wait(1);
                    }
                    Vector2 offset = BraveMathCollege.GetEllipsePoint(Vector2.zero, 1.39f, 0.92f, angle);
                    base.Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle - 2.5f, DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UnDodgeableCheese.Name, true, false, false));
                    base.Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle, DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UnDodgeableCheese.Name, true, false, false));
                    base.Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle + 2.5f, DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new Bullet(StaticUndodgeableBulletEntries.UnDodgeableCheese.Name, true, false, false));
                    
                    base.Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction((angle - 2.5f) + 20, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UnDodgeableCheese.Name, 16f, 60, -1, true));
                    base.Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle + 20, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UnDodgeableCheese.Name, 16f, 60, -1, true));
                    base.Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction((angle + 2.5f) + 20, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UnDodgeableCheese.Name, 16f, 60, -1, true));

                    base.Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction((angle - 2.5f) + 30, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UnDodgeableCheese.Name, 16f, 120, -1, true));
                    base.Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle + 30, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UnDodgeableCheese.Name, 16f, 120, -1, true));
                    base.Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction((angle + 2.5f) + 30, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.UnDodgeableCheese.Name, 16f, 120, -1, true));

                    i++;
                }
                yield return base.Wait(75);

                yield break;
            }
        }
        public class ModifiedResourcefulRatDaggers1 : Script
        {
            public override IEnumerator Top()
            {
                int DaggerCap = 11;

                yield return base.Wait(18);
                float[] angles = new float[DaggerCap];
                CellArea area = base.BulletBank.aiActor.ParentRoom.area;
                int totalAttackTicks = 56;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < DaggerCap; j++)
                    {
                        float angle = base.AimDirection;
                        float timeUntilFire = (float)(totalAttackTicks - j) / 45f;
                        if (j != DaggerCap - 1)
                        {
                            int num = j / 2;
                            bool flag = j % 2 == 1;
                            Vector2 vector = IntVector2.CardinalsAndOrdinals[num].ToVector2();
                            float d = (!flag) ? 8.5f : 11f;
                            Vector2 vector2 = this.BulletManager.PlayerPosition();
                            Vector2 a = vector.normalized * d;
                            vector2 += a * timeUntilFire;
                            Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2, this.BulletManager.PlayerVelocity(), base.Position, 60f);
                            angle = (predictedPosition - base.Position).ToAngle();
                        }
                        for (int k = 0; k < j; k++)
                        {
                            if (!float.IsNaN(angles[k]) && BraveMathCollege.AbsAngleBetween(angles[k], angle) < 3f)
                            {
                                angle = float.NaN;
                            }
                        }
                        angles[j] = angle;
                        if (!float.IsNaN(angles[j]))
                        {
                            float num2 = 20f;
                            Vector2 zero = Vector2.zero;
                            if (BraveMathCollege.LineSegmentRectangleIntersection(base.Position, base.Position + BraveMathCollege.DegreesToVector(angle, 60f), area.UnitBottomLeft, area.UnitTopRight - new Vector2(0f, 6f), ref zero))
                            {
                                num2 = (zero - base.Position).magnitude;
                            }
                            GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
                            tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                            component2.transform.position = new Vector2(base.Position.x, base.Position.y) + BraveMathCollege.DegreesToVector(angle, 2f);
                            component2.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
                            component2.dimensions = new Vector2((num2 - 3f) * 16f, 1f);
                            component2.UpdateZDepth();

                            Color laser = new Color(0f, 1f, 1f, 1f);
                            component2.sprite.usesOverrideMaterial = true;
                            component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                            component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                            component2.sprite.renderer.material.SetFloat("_EmissivePower", 50);
                            component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 5f);
                            component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                            component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);


                            this.m_reticles.Add(gameObject);
                        }
                        if (j < DaggerCap)
                        {
                            yield return base.Wait(1);
                        }
                    }
                    yield return base.Wait(15);
                    this.CleanupReticles();
                    yield return base.Wait(25);
                    for (int l = 0; l < DaggerCap; l++)
                    {
                        if (!float.IsNaN(angles[l]))
                        {
                            base.Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("UnDodgeableDagger", 30, 160));
                            base.Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("UnDodgeableDagger", 30, 120));
                            base.Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("UnDodgeableDagger", 30, 80));
                            base.Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("UnDodgeableDagger", 30, 40));
                        }
                    }

                    yield return base.Wait(30);
                }
                yield break;
            }

            public override void OnForceEnded()
            {
                this.CleanupReticles();
            }

            public Vector2 GetPredictedTargetPosition(float leadAmount, float speed, float fireDelay)
            {
                Vector2 vector = this.BulletManager.PlayerPosition();
                Vector2 a = this.BulletManager.PlayerVelocity();
                vector += a * fireDelay;
                return BraveMathCollege.GetPredictedPosition(vector, this.BulletManager.PlayerVelocity(), base.Position, speed);
            }

            private void CleanupReticles()
            {
                for (int i = 0; i < this.m_reticles.Count; i++)
                {
                    SpawnManager.Despawn(this.m_reticles[i]);
                }
                this.m_reticles.Clear();
            }


            private List<GameObject> m_reticles = new List<GameObject>();
        }
        public class ModifiedResourcefulRatDaggers1Quick : Script
        {
            public override IEnumerator Top()
            {
                int DaggerCap = 3;

                yield return base.Wait(18);
                float[] angles = new float[DaggerCap];
                CellArea area = base.BulletBank.aiActor.ParentRoom.area;
                int totalAttackTicks = 56;
                for (int j = 0; j < DaggerCap; j++)
                {
                    float angle = base.AimDirection;
                    float timeUntilFire = (float)(totalAttackTicks - j) / 45f;
                    if (j != DaggerCap - 1)
                    {
                        int num = j / 2;
                        bool flag = j % 2 == 1;
                        Vector2 vector = IntVector2.CardinalsAndOrdinals[num].ToVector2();
                        float d = (!flag) ? 5f : 6.5f;
                        Vector2 vector2 = this.BulletManager.PlayerPosition();
                        Vector2 a = vector.normalized * d;
                        vector2 += a * timeUntilFire;
                        Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2, this.BulletManager.PlayerVelocity(), base.Position, 60f);
                        angle = (predictedPosition - base.Position).ToAngle();
                    }
                    for (int k = 0; k < j; k++)
                    {
                        if (!float.IsNaN(angles[k]) && BraveMathCollege.AbsAngleBetween(angles[k], angle) < 3f)
                        {
                            angle = float.NaN;
                        }
                    }
                    angles[j] = angle;
                    if (!float.IsNaN(angles[j]))
                    {
                        //ResourcefulRatController component = base.BulletBank.GetComponent<ResourcefulRatController>();
                        float num2 = 20f;
                        Vector2 zero = Vector2.zero;
                        if (BraveMathCollege.LineSegmentRectangleIntersection(base.Position, base.Position + BraveMathCollege.DegreesToVector(angle, 60f), area.UnitBottomLeft, area.UnitTopRight - new Vector2(0f, 6f), ref zero))
                        {
                            num2 = (zero - base.Position).magnitude;
                        }
                        GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
                        tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                        component2.transform.position = new Vector2(base.Position.x, base.Position.y) + BraveMathCollege.DegreesToVector(angle, 2f);
                        component2.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
                        component2.dimensions = new Vector2((num2 - 3f) * 16f, 1f);
                        component2.UpdateZDepth();

                        Color laser = new Color(0f, 1f, 1f, 1f);
                        component2.sprite.usesOverrideMaterial = true;
                        component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                        component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                        component2.sprite.renderer.material.SetFloat("_EmissivePower", 50);
                        component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 5f);
                        component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                        component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);

                        this.m_reticles.Add(gameObject);
                    }
                    if (j < DaggerCap)
                    {
                        yield return base.Wait(1);
                    }
                }
                yield return base.Wait(20);
                this.CleanupReticles();
                for (int l = 0; l < DaggerCap; l++)
                {
                    if (!float.IsNaN(angles[l]))
                    {
                        base.Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("UnDodgeableDagger", 30, 240));
                        base.Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("UnDodgeableDagger", 30, 200));
                        base.Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("UnDodgeableDagger", 30, 160));
                        base.Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("UnDodgeableDagger", 30, 120));
                        base.Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("UnDodgeableDagger", 30, 80));
                        base.Fire(new Offset(new Vector2(0.5f, 0f), angles[l], string.Empty, DirectionType.Absolute), new Direction(angles[l], DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("UnDodgeableDagger", 30, 40));

                    }
                }
                yield break;
            }

            public override void OnForceEnded()
            {
                this.CleanupReticles();
            }

            public Vector2 GetPredictedTargetPosition(float leadAmount, float speed, float fireDelay)
            {
                Vector2 vector = this.BulletManager.PlayerPosition();
                Vector2 a = this.BulletManager.PlayerVelocity();
                vector += a * fireDelay;
                return BraveMathCollege.GetPredictedPosition(vector, this.BulletManager.PlayerVelocity(), base.Position, speed);
            }

            private void CleanupReticles()
            {
                for (int i = 0; i < this.m_reticles.Count; i++)
                {
                    SpawnManager.Despawn(this.m_reticles[i]);
                }
                this.m_reticles.Clear();
            }


            private List<GameObject> m_reticles = new List<GameObject>();
        }

        public class ModifuedResourcefulRatCheeseWheel1 : Script
        {
            public override IEnumerator Top()
            {
                CellArea area = base.BulletBank.aiActor.ParentRoom.area;
                Vector2 roomLowerLeft = area.UnitBottomLeft;
                Vector2 roomUpperRight = area.UnitTopRight - new Vector2(0f, 3f);
                Vector2 roomCenter = area.UnitCenter - new Vector2(0f, 2.5f);
                base.PostWwiseEvent("Play_BOSS_Rat_Cheese_Summon_01", null);

                int AmountOfCheese = 5;
                for (int i = 0; i < 3; i++)
                {
                    int misfireIndex = UnityEngine.Random.Range(0, 15);
                    for (int j = 0; j < AmountOfCheese; j++)
                    {
                        Vector2 vector = new Vector2(roomLowerLeft.x, base.SubdivideRange(roomLowerLeft.y, roomUpperRight.y, AmountOfCheese + 1, j, true));
                        //vector += new Vector2(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-0.25f, 0.25f));
                        vector.x -= 1.25f;
                        bool isMisfire = j >= misfireIndex && j < misfireIndex + 5;
                        this.FireWallBullet(0f, vector, roomCenter, isMisfire);
                    }
                    misfireIndex = UnityEngine.Random.Range(0, 15);
                    for (int k = 0; k < AmountOfCheese; k++)
                    {
                        Vector2 vector2 = new Vector2(base.SubdivideRange(roomLowerLeft.x, roomUpperRight.x, AmountOfCheese + 1, k, true), roomUpperRight.y);
                        //vector2 += new Vector2(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-0.25f, 0.25f));
                        vector2.y += 3.25f;
                        bool isMisfire2 = k >= misfireIndex && k < misfireIndex + 5;
                        this.FireWallBullet(-90f, vector2, roomCenter, isMisfire2);
                    }
                    misfireIndex = UnityEngine.Random.Range(0, 15);
                    for (int l = 0; l < AmountOfCheese; l++)
                    {
                        Vector2 vector3 = new Vector2(roomUpperRight.x, base.SubdivideRange(roomLowerLeft.y, roomUpperRight.y, AmountOfCheese + 1, l, true));
                        //vector3 += new Vector2(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-0.25f, 0.25f));
                        vector3.x += 1.25f;
                        bool isMisfire3 = l >= misfireIndex && l < misfireIndex + 5;
                        this.FireWallBullet(180f, vector3, roomCenter, isMisfire3);
                    }
                    misfireIndex = UnityEngine.Random.Range(0, 15);
                    for (int m = 0; m < AmountOfCheese; m++)
                    {
                        Vector2 vector4 = new Vector2(base.SubdivideRange(roomLowerLeft.x, roomUpperRight.x, AmountOfCheese + 1, m, true), roomLowerLeft.y);
                        //vector4 += new Vector2(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-0.25f, 0.25f));
                        vector4.y -= 1.25f;
                        bool isMisfire4 = m >= misfireIndex && m < misfireIndex + 5;
                        this.FireWallBullet(90f, vector4, roomCenter, isMisfire4);
                    }
                    if (i == 2)
                    {
                        base.EndOnBlank = true;
                    }
                    yield return base.Wait(75);
                }
                yield return base.Wait(105);
                AIActor aiActor = base.BulletBank.aiActor;
                aiActor.aiAnimator.PlayUntilFinished("cheese_wheel_out", false, null, -1f, false);
                aiActor.IsGone = true;
                aiActor.specRigidbody.CollideWithOthers = false;
                base.Fire(Offset.OverridePosition(roomCenter), new Speed(0f, SpeedType.Absolute), new ModifuedResourcefulRatCheeseWheel1.CheeseWheelBullet());
                yield return base.Wait(65);
                aiActor.IsGone = false;
                aiActor.specRigidbody.CollideWithOthers = true;
                yield return base.Wait(120);
                yield break;
            }

            public override void OnForceEnded()
            {
                AIActor aiActor = base.BulletBank.aiActor;
                aiActor.IsGone = false;
                aiActor.specRigidbody.CollideWithOthers = true;
            }

            private void FireWallBullet(float facingDir, Vector2 spawnPos, Vector2 roomCenter, bool isMisfire)
            {
                float angleDeg = (spawnPos - roomCenter).ToAngle();
                spawnPos += new Vector2(UnityEngine.Random.Range(-0.75f, 0.75f), UnityEngine.Random.Range(-0.75f, 0.75f));
                int num = Mathf.RoundToInt(BraveMathCollege.ClampAngle360(angleDeg) / 45f) % 8;
                float num2 = (float)num * 45f;
                Vector2 targetPos = (roomCenter + BraveMathCollege.DegreesToVector(num2, 0.875f) + ResourcefulRatCheeseWheel1.TargetOffsets[num]).Quantize(0.0625f);
                base.Fire(Offset.OverridePosition(spawnPos), new Direction(facingDir, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new ModifuedResourcefulRatCheeseWheel1.CheeseWedgeBullet(this, ModifuedResourcefulRatCheeseWheel1.TargetNames[num], ModifuedResourcefulRatCheeseWheel1.RampHeights[num], targetPos, num2 + 180f, isMisfire));
            }

            public IEnumerator DoSpawnCheeses(float facingDir, Vector2 spawnPos, Vector2 roomCenter, bool isMisfire)
            {
                for (int i = 0; i < 1; i++)
                {

                }

                yield break;
            }


          
            private static string[] TargetNames = new string[]
            {
            "UnDodgeablecheeseWedge0",
            "UnDodgeablecheeseWedge1",
            "UnDodgeablecheeseWedge2",
            "UnDodgeablecheeseWedge3",
            "UnDodgeablecheeseWedge4",
            "UnDodgeablecheeseWedge5",
            "UnDodgeablecheeseWedge6",
            "UnDodgeablecheeseWedge7"
            };

            // Token: 0x04000B80 RID: 2944
            private static float[] RampHeights = new float[]
            {
        2f,
        1f,
        0f,
        1f,
        2f,
        3f,
        4f,
        2f
            };

            private static Vector2[] TargetOffsets = new Vector2[]
            {
        new Vector2(0f, 0.0625f),
        new Vector2(0.0625f, -0.0625f),
        new Vector2(0.0625f, 0f),
        new Vector2(0.0625f, -0.0625f),
        new Vector2(0.0625f, 0.0625f),
        new Vector2(0f, 0f),
        new Vector2(0.0625f, 0f),
        new Vector2(0.125f, -0.125f)
            };

            public class CheeseWedgeBullet : Bullet
            {
                public CheeseWedgeBullet(ModifuedResourcefulRatCheeseWheel1 parent, string bulletName, float additionalRampHeight, Vector2 targetPos, float endingAngle, bool isMisfire) : base(bulletName, true, false, false)
                {
                    this.m_parent = parent;
                    this.m_targetPos = targetPos;
                    this.m_endingAngle = endingAngle;
                    this.m_isMisfire = isMisfire;
                    this.m_additionalRampHeight = additionalRampHeight;
                }

                public override IEnumerator Top()
                {
                    int travelTime = UnityEngine.Random.Range(90, 136);
                    this.Projectile.IgnoreTileCollisionsFor(90f);
                    this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
                    this.Projectile.sprite.HeightOffGround = 10f + this.m_additionalRampHeight + UnityEngine.Random.value / 2f;
                    this.Projectile.sprite.ForceRotationRebuild();
                    this.Projectile.sprite.UpdateZDepth();
                    int r = UnityEngine.Random.Range(0, 20);
                    yield return base.Wait(15 + r);
                    this.Speed = 2.5f;
                    yield return base.Wait(50 - r);
                    this.Speed = 0f;
                    if (this.m_isMisfire)
                    {
                        this.Direction += 180f;
                        this.Speed = 2.5f;
                        yield return base.Wait(180);
                        base.Vanish(true);
                        yield break;
                    }
                    this.Direction = (this.m_targetPos - base.Position).ToAngle();
                    base.ChangeSpeed(new Speed((this.m_targetPos - base.Position).magnitude / ((float)(travelTime - 15) / 60f), SpeedType.Absolute), 30);
                    yield return base.Wait(travelTime);
                    this.Speed = 0f;
                    base.Position = this.m_targetPos;
                    this.Direction = this.m_endingAngle;
                    if (this.Projectile && this.Projectile.sprite)
                    {
                        this.Projectile.sprite.HeightOffGround -= 1f;
                        this.Projectile.sprite.UpdateZDepth();
                    }
                    int totalTime = 350;
                    yield return base.Wait(totalTime - this.m_parent.Tick);
                    base.Vanish(true);
                    yield break;
                }

                private ModifuedResourcefulRatCheeseWheel1 m_parent;

                private Vector2 m_targetPos;

                private float m_endingAngle;

                private bool m_isMisfire;

                private float m_additionalRampHeight;
            }

            // Token: 0x020002CA RID: 714
            public class CheeseWheelBullet : Bullet
            {
                public CheeseWheelBullet() : base("UnDodgeableCheeseWheel", true, false, false)
                {
                }

                public override IEnumerator Top()
                {
                    this.Projectile.spriteAnimator.Play("cheese_wheel_burst");
                    this.Projectile.ImmuneToSustainedBlanks = true;
                    yield return base.Wait(45);
                    this.Projectile.Ramp(-1.5f, 100f);
                    yield return base.Wait(80);

                    for (int f = 0; f < 24; f++)
                    {
                        SpeedChangingBullet bullet = new SpeedChangingBullet("UnDodgeableCheese", 16, 120);
                        base.Fire(new Direction((15 * f), DirectionType.Absolute, -1f), new Speed(9, SpeedType.Absolute), bullet);
                        bullet.Projectile.ImmuneToSustainedBlanks = true;

                        SpeedChangingBullet bullet_1 = new SpeedChangingBullet("UnDodgeableCheese", 7, 120);
                        base.Fire(new Direction((15 * f) + 7.5f, DirectionType.Absolute, -1f), new Speed(15, SpeedType.Absolute), bullet_1);
                        bullet_1.Projectile.ImmuneToSustainedBlanks = true;
                    }


                    if (base.BulletBank)
                    {
                        ResourcefulRatController component = base.BulletBank.GetComponent<ResourcefulRatController>();
                        if (component)
                        {
                            GameManager.Instance.MainCameraController.DoScreenShake(component.cheeseSlamScreenShake, null, false);
                        }
                    }
                    yield return base.Wait(25);
                    base.Vanish(true);
                    yield break;
                }
            }
        }
    }
}
