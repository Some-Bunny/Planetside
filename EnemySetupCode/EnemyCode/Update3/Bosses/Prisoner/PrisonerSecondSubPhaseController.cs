﻿ using System;
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
            KillRing = false;
            SubPhaseEnded = false;
            SubPhaseActivated = false;
            Actor = base.aiActor;
            if (Actor != null)
            {
                Controller = Actor.GetComponent<PrisonerPhaseOne.PrisonerController>();
                FirstSubPhaseController = Actor.GetComponent<PrisonerFirstSubPhaseController>();
                Actor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSkull);
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

            /*
             GameManager.Instance.BestActivePlayer.CurrentRoom.BecomeTerrifyingDarkRoom(5f, 0.5f, 0.1f, "Play_ENM_darken_world_01");
             while (elaWait < 3f)
             {
                 float t = elaWait / 3;
                 Actor.renderer.material.SetFloat("_Fade", 1-t);
                 elaWait += BraveTime.DeltaTime;
                 yield return null;
             }
             SpawnManager.SpawnBulletScript(Actor, Actor.sprite.WorldCenter, Actor.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(SubphaseTwoAttack)), StringTableManager.GetEnemiesString("#PRISONERPHASEONENAME", -1));
             Actor.SetOutlines(false);
             Actor.renderer.enabled = false;
             elaWait = 0f;
             while (elaWait < 30f)
             {
                 elaWait += BraveTime.DeltaTime;
                 yield return null;
             }
             */
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
            SpawnManager.SpawnBulletScript(Actor, Actor.sprite.WorldCenter, Actor.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(A)), StringTableManager.GetEnemiesString("#PRISONERPHASEONENAME", -1));
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
                if (attackGroup != null && attackGroupItem.NickName.Contains("Two"))
                {
                    attackGroupItem.Probability = 0f;
                }
                if (attackGroup != null && attackGroupItem.NickName.Contains("Three"))
                {
                    float val = 1;
                    AttackNamesAndProbabilities.TryGetValue(attackGroupItem.NickName, out val);
                    attackGroupItem.Probability = val;
                }
            }
        }

        private static Dictionary<string, float> AttackNamesAndProbabilities = new Dictionary<string, float>()
        {
            {"SimpleBlastsThree", 0.8f },
            {"WallSweepThree", 1f },
            {"LaserCrossThree", 2 },
            {"SweepJukeAttackThree", 0.9f },
            {"BasicLaserAttackTellThree", 1.2f },
            {"ChainRotatorsThree", 0.8f },
        };


        public bool WasJammed;
        public AIActor Actor;
        public PrisonerPhaseOne.PrisonerController Controller;
        public PrisonerFirstSubPhaseController FirstSubPhaseController;
        public bool SubPhaseActivated;
        public static bool SubPhaseEnded;
        public static bool KillRing;

        public class A : Script
        {
            protected override IEnumerator Top()
            {
                this.EndOnBlank = false;
                Vector2 spawnPos = base.BulletBank.aiActor.ParentRoom.GetCenterCell().ToCenterVector2();
                if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
                {
                    base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
                }

                for (int l = 0; l < 12; l++)
                {
                    for (int j = 0; j < 40; j++)
                    {
                        float angle = this.SubdivideCircle(0f, 40, j, 1f, false);
                        this.Fire(Offset.OverridePosition(this.BulletBank.aiActor.transform.PositionVector2() + MathToolbox.GetUnitOnCircle(angle, 30)), new A.TargetBullet(this, this.BulletBank.aiActor, 10f + (l*1.5f)));
                    }
                    yield return base.Wait(4.5f);
                }
                int e = 0;
                for (; ; )
                {
                    if (KillRing == true) { this.Destroyed = true; yield break; }
                    e++;
                    yield return base.Wait(1);
                }
            }
            public class TargetBullet : Bullet
            {
                public TargetBullet(A parent, AIActor targetDummy, float radiusCap) : base("undodgeableDefault", false, false, false)
                {
                    this.m_parent = parent;
                    this.dummy = targetDummy;
                    this.radCap = radiusCap;
                }

                protected override IEnumerator Top()
                {
                    this.Projectile.ImmuneToBlanks = true;
                    this.Projectile.ImmuneToSustainedBlanks = true;
                    this.Projectile.ForcePlayerBlankable = true;
                    this.Projectile.IgnoreTileCollisionsFor(6000f);


                    Vector2 toCenter = this.Position - this.dummy.transform.PositionVector2();
                    float angle = toCenter.ToAngle();
                    float radius = 30;
                    this.ManualControl = true;
                    this.Projectile.specRigidbody.CollideWithTileMap = false;
                    this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
                    while (!this.m_parent.Destroyed && !this.m_parent.IsEnded)
                    {

                        if (radius > radCap) { radius -= BraveTime.DeltaTime; }
                        angle += 1f;
                        this.Position = this.dummy.sprite.WorldCenter + BraveMathCollege.DegreesToVector(angle, radius);
                        yield return this.Wait(1);
                    }
                    this.Vanish(false);
                    yield break;
                }
                private float radCap;
                private A m_parent;
                private AIActor dummy;
            }
        }
            public class SubphaseTwoAttack : Script
            {
                private bool Center;

                private bool Done;


                public List<TargetBullet> activeRingSegments = new List<TargetBullet>();
                public TargetDummy activeDummy;

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
                        base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
                    }
                    this.StartTask(ContinuallySpawnRings());
                    this.EndOnBlank = false;
                    this.StartTask(this.SpawnRingOfHell());
                    int e = 0;
                    for (; ; )
                    {
                        if (e > 1620) { Center = true; }
                        if (SubPhaseEnded == true) { this.Destroyed = true; yield break; }
                        e++;
                        yield return base.Wait(1);
                    }
                }

                private IEnumerator ContinuallySpawnRings()
                {
                    while (SubPhaseEnded == false || !this.Destroyed || !this.IsEnded)
                    {
                        bool LeftOrRight = (UnityEngine.Random.value > 0.5f) ? false : true;
                        float RNGSPIN = LeftOrRight == true ? 20 : -20;
                        for (int e = 0; e < 60; e++)
                        {
                            base.Fire(new Offset(MathToolbox.GetUnitOnCircle(e * 6, 30)), new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SubphaseTwoAttack.RotatedBullet(RNGSPIN, 0, 0, "undodgeableDefault", this, e == 0, e * 6, 30));
                        }
                        yield return this.Wait(80f);
                    }
                    if (SubPhaseEnded == true)
                    {
                        yield break;
                    }
                }

                public IEnumerator SpawnRingOfHell()
                {

                    if (SubPhaseEnded == false)
                    {
                        for (int i = 0; i < activeRingSegments.Count; i++)
                        {
                            if (activeRingSegments[i] != null)
                            {
                                activeRingSegments[i].Vanish(false);
                            }
                        }
                        GameManager.Instance.StartCoroutine(HandleSpawnVFXPortal());
                    }
                    yield return this.Wait(90f);
                    if (SubPhaseEnded == false)
                    {

                        SubphaseTwoAttack.TargetDummy targetDummy = new SubphaseTwoAttack.TargetDummy(this);
                        targetDummy.Position = this.BulletBank.aiActor.ParentRoom.area.UnitCenter;
                        targetDummy.Direction = this.AimDirection;
                        targetDummy.BulletManager = this.BulletManager;
                        activeDummy = targetDummy;
                        this.Fire(Offset.OverridePosition(targetDummy.Position), targetDummy);
                        for (int j = 0; j < 30; j++)
                        {
                            float angle = this.SubdivideCircle(0f, 30, j, 1f, false);
                            this.Fire(Offset.OverridePosition(targetDummy.Position + BraveMathCollege.DegreesToVector(angle, 0.375f)), new SubphaseTwoAttack.TargetBullet(this, targetDummy.Projectile));
                        }
                    }
                    yield break;
                }

                public IEnumerator HandleSpawnVFXPortal()
                {
                    GameObject portalObject = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal"));
                    portalObject.transform.position = this.BulletBank.aiActor.ParentRoom.area.UnitCenter;
                    portalObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));

                    AkSoundEngine.PostEvent("Play_PortalOpen", portalObject.gameObject);
                    for (int j = 0; j < 90; j++)
                    {
                        float t = (float)j / (float)90;
                        float t1 = Mathf.Sin(t * (Mathf.PI / 2));
                        portalObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 3, t1);
                        portalObject.GetComponent<MeshRenderer>().material.SetFloat("_OutlineWidth", Mathf.Lerp(0, 0.125f, t1));
                        portalObject.GetComponent<MeshRenderer>().material.SetFloat("_OutlinePower", Mathf.Lerp(0, 100, t1));

                        yield return this.Wait(1f);
                    }
                    for (int j = 0; j < 60; j++)
                    {
                        float t = (float)j / (float)60;
                        float t1 = Mathf.Sin(t * (Mathf.PI / 2));
                        portalObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 3, 1 - t1);
                        portalObject.GetComponent<MeshRenderer>().material.SetFloat("_OutlineWidth", Mathf.Lerp(0, 0.125f, 1 - t1));
                        portalObject.GetComponent<MeshRenderer>().material.SetFloat("_OutlinePower", Mathf.Lerp(0, 100, 1 - t1));
                        yield return this.Wait(1f);
                    }

                    Destroy(portalObject);
                    yield break;
                }




                public class TargetDummy : Bullet
                {
                    public TargetDummy(SubphaseTwoAttack parent) : base("undodgeableBig", false, false, false)
                    {
                        this.parent = parent;
                    }

                    protected override IEnumerator Top()
                    {
                        WeightedIntCollection attackWeights = new WeightedIntCollection();
                        attackWeights.elements = new WeightedInt[]
                        {
                    new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "Attack1", value = 1, weight = 1},
                    new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "Attack2", value = 2, weight = 0.9f},
                    new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "Attack3", value = 3, weight = 0.5f},
                            //new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "Attack4", value = 4, weight = 0.8f},
                        };


                        this.PostWwiseEvent("Play_PortalOpen", null);
                        Exploder.DoDistortionWave(this.Position, 5, 0.2f, 50, 0.75f);
                        this.Projectile.ImmuneToBlanks = true;
                        this.Projectile.ImmuneToSustainedBlanks = true;
                        this.Projectile.ForcePlayerBlankable = true;
                        this.Projectile.IgnoreTileCollisionsFor(6000f);
                        yield return this.Wait(60f);
                        int time = 180;
                        int i = 0;
                        for (; ; )
                        {
                            if (parent.IsEnded || parent.Destroyed)
                            {
                                base.Vanish(false);
                            }
                            if (i % time == 0 && parent.Center == false)
                            {
                                if (time > 91) { time -= 10; }
                                if (time < 90) { time = 90; }
                                switch (attackWeights.SelectByWeight(new System.Random(UnityEngine.Random.Range(1, 100))))
                                {
                                    case 1:
                                        for (int j = 0; j < 6; j++)
                                        {
                                            GameManager.Instance.StartCoroutine(QuickscopeNoob(0, parent, j * 60, 80, 30, 0.75f, 3, 0.5f));
                                        }
                                        yield return base.Wait(1);
                                        i++;
                                        break;
                                    case 2:
                                        float Dir = UnityEngine.Random.value > 0.5f ? 0 : 45f;
                                        for (int e = 0; e < 4; e++)
                                        {
                                            GameManager.Instance.StartCoroutine(QuickReticleNoAngleChange(this.Position, (90f * e) + Dir, parent, 0.75f, 25, 4));
                                        }
                                        yield return base.Wait(1);
                                        i++;
                                        break;
                                    case 3:
                                        GameManager.Instance.StartCoroutine(QuickscopeNoob(0, parent, 0, 80, 20, 0.5f, 3, 0.5f));
                                        GameManager.Instance.StartCoroutine(QuickscopeNoob(0, parent, 0, 80, 20, 1f, 3, 0.5f));
                                        GameManager.Instance.StartCoroutine(QuickscopeNoob(0, parent, 0, 80, 20, 1.5f, 3, 0.5f));

                                        yield return base.Wait(1);
                                        i++;
                                        break;

                                    case 4:
                                        GameManager.Instance.StartCoroutine(QuickscopeNoob(0, parent, 0, 80, 30, 3, 1, 0.5f));

                                        yield return base.Wait(1);
                                        i++;
                                        break;
                                }
                            }

                            float distToTarget = (this.BulletManager.PlayerPosition() - this.Position).magnitude;
                            float a = Mathf.Lerp(9f, 2f, Mathf.InverseLerp(6f, 2f, distToTarget));
                            this.Speed = Mathf.Min(a, (float)(this.Tick - 30) / 60f * 10f);

                            this.ChangeDirection(new Direction(0f, DirectionType.Aim, 3f), 1);
                            i++;
                            yield return this.Wait(1);
                        }
                    }

                    public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                    {
                        base.OnBulletDestruction(destroyType, hitRigidbody, preventSpawningProjectiles);
                        if (!parent.IsEnded || !parent.Destroyed)
                        {
                            parent.StartTask(parent.SpawnRingOfHell());
                            this.PostWwiseEvent("Play_WPN_Life_Orb_Blast_01", null);
                            float sadFace = UnityEngine.Random.Range(0, 30);
                            Exploder.DoDistortionWave(this.Position, 4, 0.5f, 50, 1f);
                            for (int e = 0; e < 24; e++)
                            {
                                GameManager.Instance.StartCoroutine(QuickReticleNoAngleChange(this.Position, (15f * e) + sadFace, parent, 0.625f, 30, 8, false));
                            }
                            parent.activeDummy = null;
                        }
                    }

                    private IEnumerator QuickscopeNoob(float Angle, SubphaseTwoAttack parent, float offset, float PredictionSpeedStart, float PredictionSpeedEnd, float delay = 0.25f, int BulletAmount = 3, float chargeTime = 0.25f)
                    {

                        GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
                        tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                        component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
                        component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
                        component2.dimensions = new Vector2(1000f, 1f);
                        component2.UpdateZDepth();
                        component2.HeightOffGround = -2;
                        Color laser = new Color(0f, 1f, 1f, 1f);
                        component2.sprite.usesOverrideMaterial = true;
                        component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                        component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                        component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
                        component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
                        component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                        component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
                        float elapsed = 0;
                        float Time = delay;
                        while (elapsed < Time)
                        {
                            float t = (float)elapsed / (float)Time;
                            if (parent.activeDummy == null)
                            {
                                UnityEngine.Object.Destroy(component2.gameObject);
                                yield break;
                            }

                            if (parent.IsEnded || parent.Destroyed)
                            {
                                UnityEngine.Object.Destroy(component2.gameObject);
                                yield break;
                            }

                            if (component2 != null)
                            {
                                float Pos = (base.GetPredictedTargetPosition(Mathf.Lerp(0, 0.6f, t), Mathf.Lerp(PredictionSpeedStart, PredictionSpeedEnd, t)) - parent.activeDummy.Position).ToAngle();
                                component2.transform.position = new Vector3(parent.activeDummy.Position.x, parent.activeDummy.Position.y, 0);
                                component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (25 * t));
                                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
                                component2.transform.localRotation = Quaternion.Euler(0f, 0f, Pos + offset);
                                component2.HeightOffGround = -2;
                                component2.renderer.gameObject.layer = 23;
                                component2.dimensions = new Vector2(1000f, 1f);
                                component2.UpdateZDepth();
                                Angle = Pos + offset;
                            }
                            elapsed += BraveTime.DeltaTime;
                            yield return null;
                        }
                        elapsed = 0;
                        Time = chargeTime;
                        base.PostWwiseEvent("Play_FlashTell");
                        while (elapsed < Time)
                        {
                            if (parent.activeDummy == null)
                            {
                                UnityEngine.Object.Destroy(component2.gameObject);
                                yield break;
                            }
                            if (parent.IsEnded || parent.Destroyed)
                            {
                                UnityEngine.Object.Destroy(component2.gameObject);
                                yield break;
                            }
                            float t = (float)elapsed / (float)Time;
                            if (component2 != null)
                            {
                                component2.transform.position = new Vector3(parent.activeDummy.Position.x, parent.activeDummy.Position.y, 0);
                                component2.dimensions = new Vector2(1000f, 1f);
                                component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (60 * t));
                                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (20 * t));
                                component2.HeightOffGround = -2;
                                component2.renderer.gameObject.layer = 23;
                                component2.UpdateZDepth();
                            }
                            elapsed += BraveTime.DeltaTime;
                            yield return null;
                        }
                        UnityEngine.Object.Destroy(component2.gameObject);
                        base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
                        for (int i = 0; i < BulletAmount; i++)
                        {
                            base.Fire(Offset.OverridePosition(parent.activeDummy.Position), new Direction(Angle, DirectionType.Absolute, -1f), new Speed(12, SpeedType.Absolute), new WallBulletNoDodge("sniperUndodgeable"));
                            yield return new WaitForSeconds(0.025f);
                        }
                        yield break;
                    }


                    private IEnumerator QuickReticleNoAngleChange(Vector2 startPos, float aimDir, SubphaseTwoAttack parent, float chargeTime = 0.5f, float BulletSpeed = 15, int buletAmount = 10, bool ActiveDummyRequired = true)
                    {

                        GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
                        tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                        component2.transform.position = new Vector3(startPos.x, startPos.y, 99999);
                        component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
                        component2.dimensions = new Vector2(1000f, 1f);
                        component2.UpdateZDepth();
                        component2.HeightOffGround = -2;
                        Color laser = new Color(0f, 1f, 1f, 1f);
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

                            if (parent.activeDummy == null && ActiveDummyRequired == true)
                            {
                                Destroy(component2.gameObject);
                                yield break;
                            }

                            if (parent.IsEnded || parent.Destroyed)
                            {
                                Destroy(component2.gameObject);
                                yield break;
                            }
                            if (component2 != null)
                            {
                                if (ActiveDummyRequired == true) { component2.transform.position = new Vector3(parent.activeDummy.Position.x, parent.activeDummy.Position.y, 0); }

                                component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (25 * t));
                                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
                                component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
                                component2.HeightOffGround = -2;
                                component2.renderer.gameObject.layer = 23;
                                component2.dimensions = new Vector2(1000f, 1f);
                                component2.UpdateZDepth();
                            }
                            elapsed += BraveTime.DeltaTime;
                            yield return null;
                        }
                        elapsed = 0;
                        Time = 0.25f;
                        base.PostWwiseEvent("Play_FlashTell");
                        while (elapsed < Time)
                        {
                            if (parent.activeDummy == null && ActiveDummyRequired == true)
                            {
                                Destroy(component2.gameObject);
                                yield break;
                            }
                            if (parent.IsEnded || parent.Destroyed)
                            {
                                Destroy(component2.gameObject);
                                yield break;
                            }
                            float t = (float)elapsed / (float)Time;
                            if (component2 != null)
                            {
                                if (ActiveDummyRequired == true) { component2.transform.position = new Vector3(parent.activeDummy.Position.x, parent.activeDummy.Position.y, 0); }
                                component2.dimensions = new Vector2(1000f, 1f);
                                component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (60 * t));
                                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (20 * t));
                                component2.HeightOffGround = -2;
                                component2.renderer.gameObject.layer = 23;
                                component2.UpdateZDepth();
                            }
                            elapsed += BraveTime.DeltaTime;
                            yield return null;
                        }
                        Vector2 position = component2.gameObject.transform.PositionVector2();
                        Destroy(component2.gameObject);
                        base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
                        for (int i = 0; i < buletAmount; i++)
                        {
                            base.Fire(Offset.OverridePosition(ActiveDummyRequired == true ? parent.activeDummy.Position : position), new Direction(aimDir, DirectionType.Absolute, -1f), new Speed(BulletSpeed, SpeedType.Absolute), new WallBulletNoDodge("sniperUndodgeable"));
                            yield return new WaitForSeconds(0.025f);
                        }
                        yield break;
                    }
                    private SubphaseTwoAttack parent;
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

                public class TargetBullet : Bullet
                {
                    public TargetBullet(SubphaseTwoAttack parent, Projectile targetDummy) : base("undodgeableDefault", false, false, false)
                    {
                        this.m_parent = parent;
                        this.m_targetDummy = targetDummy;
                    }

                    protected override IEnumerator Top()
                    {
                        this.m_parent.activeRingSegments.Add(this);
                        this.Projectile.ImmuneToBlanks = true;
                        this.Projectile.ImmuneToSustainedBlanks = true;
                        this.Projectile.ForcePlayerBlankable = true;
                        this.Projectile.IgnoreTileCollisionsFor(6000f);


                        Vector2 toCenter = this.Position - this.m_targetDummy.transform.PositionVector2();
                        float angle = toCenter.ToAngle();
                        float radius = toCenter.magnitude;
                        float deltaRadius = radius / 60f;
                        this.ManualControl = true;
                        this.Projectile.specRigidbody.CollideWithTileMap = false;
                        this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
                        while (!this.m_parent.Destroyed && !this.m_parent.IsEnded && this.m_parent.Done == false)
                        {
                            if (m_targetDummy == null)
                            {
                                this.Vanish(false);
                                yield break;
                            }

                            if (this.Tick < 60)
                            {
                                radius += deltaRadius * 3f;
                            }
                            if (this.m_parent.Center)
                            {
                                radius += deltaRadius;
                            }
                            angle += 1.3333334f;
                            this.Position = this.m_targetDummy.transform.PositionVector2() + BraveMathCollege.DegreesToVector(angle, radius);
                            yield return this.Wait(1);
                        }
                        this.Vanish(false);
                        //this.PostWwiseEvent("Play_BOSS_RatMech_Bomb_01", null);
                        yield break;
                    }

                    private SubphaseTwoAttack m_parent;

                    private Projectile m_targetDummy;
                }

                public class RotatedBullet : Bullet
                {
                    public RotatedBullet(float spinspeed, float RevUp, float StartSpeenAgain, string BulletType, SubphaseTwoAttack parent, bool IsEarliestRing, float angle = 0f, float startRadius = 50) : base(BulletType, false, false, false)
                    {
                        this.m_spinSpeed = spinspeed;
                        this.TimeToRevUp = RevUp;
                        this.StartAgain = StartSpeenAgain;

                        this.m_parent = parent;
                        this.m_angle = angle;
                        this.m_bulletype = BulletType;
                        this.SuppressVfx = true;
                        this.startRadius = startRadius;
                        this.IsEarliestRing = IsEarliestRing;
                    }

                    protected override IEnumerator Top()
                    {
                        this.Projectile.IgnoreTileCollisionsFor(6000f);
                        this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));

                        this.Projectile.ImmuneToBlanks = true;
                        this.Projectile.ImmuneToSustainedBlanks = true;

                        this.Projectile.pierceMinorBreakables = true;

                        base.Projectile.transform.localRotation = Quaternion.Euler(0f, 0f, this.m_angle);
                        base.ManualControl = true;
                        Vector2 centerPosition = base.BulletBank.aiActor.ParentRoom.GetCenterCell().ToCenterVector2();
                        float radius = startRadius;
                        for (int i = 0; i < 6000; i++)
                        {

                            radius -= (BraveTime.DeltaTime * 1.15f);
                            if (radius < 8.75f) { this.Projectile.DieInAir(false); yield break; }
                            if (m_parent.IsEnded || m_parent.Destroyed)
                            {
                                this.StartTask(ChangeSpinSpeedTask(radius, centerPosition));
                                yield break;
                            }
                            centerPosition += this.Velocity / 60f;
                            base.UpdateVelocity();
                            this.m_angle += this.m_spinSpeed / 60f;
                            base.Projectile.shouldRotate = true;
                            base.Direction = this.m_angle;
                            base.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);

                            yield return base.Wait(1);
                        }
                        base.Vanish(false);
                        yield break;
                    }

                    private IEnumerator ChangeSpinSpeedTask(float ra, Vector2 CenterPos)
                    {
                        float radius = ra;
                        Vector2 centerPosition = CenterPos;
                        for (int i = 0; i < 150; i++)
                        {
                            radius += 0.01f * (i / 2);
                            base.UpdateVelocity();
                            this.m_angle += this.m_spinSpeed / 60f;
                            base.Projectile.shouldRotate = true;
                            base.Direction = this.m_angle;
                            base.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);
                            yield return base.Wait(1);
                        }
                        this.Projectile.DieInAir(false);
                        yield break;
                    }
                    private const float ExpandSpeed = 4.5f;
                    private const float SpinSpeed = 40f;
                    private SubphaseTwoAttack m_parent;
                    private float m_angle;
                    private float m_spinSpeed;
                    private string m_bulletype;
                    private float TimeToRevUp;
                    private float StartAgain;
                    private float startRadius;
                    private bool IsEarliestRing;

                }
            
        }
    }
}