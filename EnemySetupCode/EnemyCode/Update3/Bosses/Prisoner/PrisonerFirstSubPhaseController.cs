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
    internal class PrisonerFirstSubPhaseController : BraveBehaviour
    {
        public void Start()
        {
            SubPhaseEnded = false;
            SubPhaseActivated = false;
            Actor = base.aiActor;
            if (Actor != null)
            {
                Controller = Actor.GetComponent<PrisonerPhaseOne.PrisonerController>();
                {
                    //Controller.CurrentSubPhase = PrisonerPhaseOne.PrisonerController.SubPhases.PHASE_2;
                    Actor.healthHaver.minimumHealth = Actor.healthHaver.GetMaxHealth() * 0.66f;
                }
            }
        }   
        public void Update()
        {
            if (Actor.healthHaver.GetCurrentHealth() == Actor.healthHaver.minimumHealth && SubPhaseActivated != true)
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
           
            SpawnManager.SpawnBulletScript(Actor, Actor.sprite.WorldCenter, Actor.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(SubphaseOneAttack)), StringTableManager.GetEnemiesString("#PRISONERPHASEONENAME", -1));
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
            Controller.CurrentSubPhase = PrisonerPhaseOne.PrisonerController.SubPhases.PHASE_2;
            Actor.behaviorSpeculator.enabled = true;
            Actor.specRigidbody.enabled = true;
            Actor.healthHaver.minimumHealth = Actor.healthHaver.GetMaxHealth() * 0.33f;

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
                    attackGroupItem.Probability = 1f;
                }
            }
        }

        public bool WasJammed;
        public AIActor Actor;
        public PrisonerPhaseOne.PrisonerController Controller;
        public bool SubPhaseActivated;
        public static bool SubPhaseEnded;

        public class SubphaseOneAttack : Script
        {
            public static float CurrentRingRadius;
            protected override IEnumerator Top()
            {
                
                WeightedIntCollection attackWeights = new WeightedIntCollection();
                attackWeights.elements = new WeightedInt[]
                {
                    new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "Attack1", value = 1, weight = 1},
                    new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "Attack2", value = 2, weight = 0.9f},
                    new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "Attack3", value = 3, weight = 0.7f},
                    new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "Attack4", value = 4, weight = 0f},
                };
                this.EndOnBlank = false;
                if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
                {
                    base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
                    base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
                }


                Vector2 TopRight = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitTopRight;
                Vector2 BottomLeft = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitBottomLeft;
                Dictionary<Vector2, Vector2> wallcornerPositions = new Dictionary<Vector2, Vector2>()
                {
                    {new Vector2(BottomLeft.x, TopRight.y), TopRight },//Bottom wall
					{new Vector2(TopRight.x, BottomLeft.y), BottomLeft },//Top wall
					{BottomLeft, new Vector2(BottomLeft.x, TopRight.y) },//Left wall
					{TopRight, new Vector2(TopRight.x, BottomLeft.y) },//Right wall
				};
                //All of these are flipped. IDFK WHY BUT I NEED TO GET THE "TOP" to spawn it at THE BOTTOM AAAAAAAAAAAAAAAAA
                Dictionary<Vector2, string> wallcornerstrings = new Dictionary<Vector2, string>()
                    {
                        {new Vector2(BottomLeft.x, TopRight.y), "bottom" },//Bottom wall
						{new Vector2(TopRight.x, BottomLeft.y), "top" },//Top wall
						{BottomLeft, "left" },//Left wall
						{TopRight, "right" },//Right wall
					};
                Vector2 spawnPos = base.BulletBank.aiActor.ParentRoom.GetCenterCell().ToCenterVector2();
                StartTask(ContinuallySpawnRings());
                yield return base.Wait(180);
                int i = 0;
                for (; ; )
                {
                    if (SubPhaseEnded == true) { this.Destroyed = true; yield break; }

                    if (i % 80 == 1)
                    {
                        bool LeftOrRight = (UnityEngine.Random.value > 0.5f) ? false : true;
                        float RNGSPIN = LeftOrRight == true ? 10 : -10;
                        float OffsetF = UnityEngine.Random.Range(0, 30);
                        for (int e = 0; e < 12; e++)
                        {
                            base.Fire(Offset.OverridePosition(spawnPos), new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SubphaseOneAttack.RotatedBulletBasic(RNGSPIN, 0, 0, "undodgeableDefault", this, (e * 30)+ OffsetF, 0.05f));
                        }
                    }
                    int Speed = 120;
                    if (i % Speed == 1)
                    {
                        if (Speed > 31) { Speed -= 30; }
                        switch (attackWeights.SelectByWeight(new System.Random(UnityEngine.Random.Range(1, 100))))
                        {
                            case 1:
                                float Dir = UnityEngine.Random.value > 0.5f ? 0 : 22.5f;
                                float cu = UnityEngine.Random.Range(0, 30);
                                for (int e = 0; e < 12; e++)
                                {
                                    GameManager.Instance.StartCoroutine(QuickReticleNoAngleChange(spawnPos, (30f * e) + cu, this, 0.625f, 18, 5));
                                }
                                yield return base.Wait(1);
                                i++;
                                break;
                            case 2:
                                for (int e = 0; e < GameManager.Instance.AllPlayers.Length; e++)
                                {
                                    GameManager.Instance.StartCoroutine(QuickReticleRedirectTowardsPlayer(spawnPos, 0, this));
                                }
                                yield return base.Wait(1);
                                i++;
                                break;
                              case 3:
                                for (int e = 0; e < GameManager.Instance.AllPlayers.Length; e++)
                                {
                                    float Dir1 = UnityEngine.Random.value > 0.5f ? 0 : 45f;
                                    for (int q = 0; q < 4; q++)
                                    {
                                        base.BulletBank.aiActor.StartCoroutine(QuickReticleNoAngleChange(GameManager.Instance.AllPlayers[e].transform.PositionVector2(), (90 * q) + Dir1, this, 0.75f, 25, 3));
                                    }
                                }
                                yield return base.Wait(1);
                                i++;
                                break;
                            case 4:
                                for (int e = 0; e < 12; e++)
                                {
                                    GameManager.Instance.StartCoroutine(QuickReticleNoAngleChange(spawnPos, (30f * e) + UnityEngine.Random.Range(0, 30), this, 1f, 18, 5));
                                }
                                yield return base.Wait(1);
                                i++;
                                break;
                        }
                    }
                    yield return base.Wait(1);
                    i++;
                }




                /*
                for (int q = 0; q < 3; q++)
                {
                    for (int e = 0; e < 60; e++)
                    {
                        base.Fire(new Offset(MathToolbox.GetUnitOnCircle(e*6, 30)), new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SubphaseOneAttack.RotatedBullet(RNGSPIN, 0, 0, "undodgeableDefault" ,this, e == 0, e * 6, 30));
                    }
                    yield return this.Wait(80f);
                }
                for (int q = 0; q < 22; q++)
                {
                    for (int e = 0; e < 60; e++)
                    {
                        base.Fire(new Offset(MathToolbox.GetUnitOnCircle(e * 6, 30)), new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SubphaseOneAttack.RotatedBullet(RNGSPIN, 0, 0, "undodgeableDefault", this,false , e * 6, 30));
                    }
                    switch (attackWeights.SelectByWeight(new System.Random(UnityEngine.Random.Range(1, 100))))
                    {
                        case 1:
                            float Dir = UnityEngine.Random.value > 0.5f ? 0 : 22.5f;
                            for (int i = 0; i < 6; i++)
                            {
                                GameManager.Instance.StartCoroutine(QuickReticleNoAngleChange(base.Position, (60 * i) + Dir, this, 0.75f, 18, 5));
                                GameManager.Instance.StartCoroutine(QuickReticleNoAngleChange(base.Position, (60 * i) + (Dir + 30f), this, 1.25f, 18, 5));
                            }
                            break;
                        case 2:

                            for (int i = 0; i < 4; i++)
                            {
                                GameManager.Instance.StartCoroutine(QuickReticleRedirectTowardsPlayer(base.Position + MathToolbox.GetUnitOnCircle(90 * i, CurrentRingRadius / 10), 0, this));
                            }

                            break;
                        case 3:
                            for (int e = 0; e < GameManager.Instance.AllPlayers.Length; e++)
                            {
                                float Dir1 = UnityEngine.Random.value > 0.5f ? 0 : 45f;
                                for (int i = 0; i < 4; i++)
                                {
                                    base.BulletBank.aiActor.StartCoroutine(QuickReticleNoAngleChange(GameManager.Instance.AllPlayers[e].transform.PositionVector2(), (90 * i) + Dir1, this, 0.75f, 25, 3));
                                }
                            }

                            break;
                        case 4:
                            string String = "NULL";
                            List<Vector2> keyList = new List<Vector2>(wallcornerPositions.Keys);
                            System.Random rand = new System.Random();
                            Vector2 randomKey = keyList[rand.Next(keyList.Count)];
                            wallcornerstrings.TryGetValue(randomKey, out String);
                            this.FireWallBullets(String);
                            break;
                    }
                    yield return this.Wait(80f);
                }
                */
                //yield break;
            }

            private IEnumerator ContinuallySpawnRings()
            {
                while (SubPhaseEnded == false || !this.Destroyed || !this.IsEnded)
                {
                    bool LeftOrRight = (UnityEngine.Random.value > 0.5f) ? false : true;
                    float RNGSPIN = LeftOrRight == true ? 20 : -20;
                    for (int e = 0; e < 60; e++)
                    {
                        base.Fire(new Offset(MathToolbox.GetUnitOnCircle(e * 6, 30)), new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SubphaseOneAttack.RotatedBullet(RNGSPIN, 0, 0, "undodgeableDefault", this, e == 0, e * 6, 30));
                    }
                    yield return this.Wait(80f);
                }
                if (SubPhaseEnded == true)
                {
                    yield break;
                }
            }

            private void FireWallBullets(string Placement)
            {
                Vector2 TopRight = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitTopRight;
                Vector2 BottomLeft = base.BulletBank.aiActor.GetAbsoluteParentRoom().area.UnitBottomLeft;
                Dictionary<Vector2, Vector2> wallcornerPositions = new Dictionary<Vector2, Vector2>()
                {
                    {new Vector2(BottomLeft.x, TopRight.y), TopRight },//Bottom wall
					{new Vector2(TopRight.x, BottomLeft.y), BottomLeft },//Top wall
					{BottomLeft, new Vector2(BottomLeft.x, TopRight.y) },//Left wall
					{TopRight, new Vector2(TopRight.x, BottomLeft.y) },//Right wall
				};
                //All of these are flipped. IDFK WHY BUT I NEED TO GET THE "TOP" to spawn it at THE BOTTOM AAAAAAAAAAAAAAAAA
                Dictionary<string, Vector2> wallcornerstrings = new Dictionary<string, Vector2>()
                {
                    { "bottom" ,new Vector2(TopRight.x, BottomLeft.y)},
                    { "top" ,new Vector2(BottomLeft.x, TopRight.y)},
                    { "left" ,TopRight},
                    { "right" ,BottomLeft}
                };

                Vector2 OneCorner = new Vector2();
                wallcornerstrings.TryGetValue(Placement, out OneCorner);
                Vector2 OtherCorner = new Vector2();
                wallcornerPositions.TryGetValue(OneCorner, out OtherCorner);
                float Tiles = Vector2.Distance(OneCorner, OtherCorner);
                float facingDir = 0;
                if (Placement == "bottom") { facingDir = 90; }
                if (Placement == "top") { facingDir = 270; }
                if (Placement == "left") { facingDir = 180; }
                if (Placement == "right") { facingDir = 0; }

                base.PostWwiseEvent("Play_RockBreaking", null);
                for (int l = 0; l < Tiles; l++)
                {
                    float t = (float)l / (float)Tiles;
                    Vector2 SpawnPos = Vector2.Lerp(OneCorner, OtherCorner, t);
                    if (Placement == "top") { SpawnPos = SpawnPos + new Vector2(0, 1.5f); }
                    if (l % 3 == 1)
                    {
                        GameManager.Instance.StartCoroutine(QuickReticleNoAngleChange(SpawnPos, facingDir, this, 0.75f, 35, 5));
                    }
                    //base.Fire(Offset.OverridePosition(SpawnPos), new Direction(facingDir, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(18f, SpeedType.Absolute), new WallBulletNoDodge("sniperUndodgeable"));
                }
            }


            private IEnumerator QuickReticleRedirectTowardsPlayer(Vector2 attachPos,float Angle, SubphaseOneAttack parent)
            {
                GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);

                tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                component2.transform.position = new Vector3(attachPos.x, attachPos.y, 99999);
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
                float Time = 0.5f;
                while (elapsed < Time)
                {
                    float t = (float)elapsed / (float)Time;

                    if (parent.IsEnded || parent.Destroyed)
                    {
                        Destroy(component2.gameObject);
                        yield break;
                    }
                    if (component2 != null)
                    {
                        component2.transform.position = new Vector3(attachPos.x, attachPos.y, 0);

                        Vector2 vector = this.BulletManager.PlayerPosition();
                        Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector, this.BulletManager.PlayerVelocity(), component2.transform.PositionVector2(), 100);
                        vector = new Vector2(vector.x + (predictedPosition.x - vector.x) * 1, vector.y + (predictedPosition.y - vector.y) * 1);
                        float angle = (vector -new Vector2(component2.transform.position.x, component2.transform.position.y)).ToAngle();

                        component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (25 * t));
                        component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
                        component2.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
                        component2.HeightOffGround = -2;
                        component2.renderer.gameObject.layer = 23;
                        component2.dimensions = new Vector2(1000f, 1f);
                        component2.UpdateZDepth();

                        Angle = angle;
                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                elapsed = 0;
                Time = 0.375f;
                base.PostWwiseEvent("Play_FlashTell");
                while (elapsed < Time)
                {
                    if (parent.IsEnded || parent.Destroyed)
                    {
                        Destroy(component2.gameObject);
                        yield break;
                    }
                    float t = (float)elapsed / (float)Time;
                    if (component2 != null)
                    {
                        component2.transform.position = new Vector3(attachPos.x, attachPos.y, 0);
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
                Destroy(component2.gameObject);
                base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
                for (int q = 0; q < 4; q++)
                {
                    base.Fire(Offset.OverridePosition(attachPos), new Direction(Angle, DirectionType.Absolute, -1f), new Speed(30, SpeedType.Absolute), new WallBulletNoDodge("sniperUndodgeable"));
                    yield return new WaitForSeconds(0.025f);

                }

                yield break;
            }

            private IEnumerator QuickReticleNoAngleChange(Vector2 startPos, float aimDir, SubphaseOneAttack parent, float chargeTime = 0.5f, float BulletSpeed = 15, int buletAmount = 10)
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

                    if (parent.IsEnded || parent.Destroyed)
                    {
                        Destroy(component2.gameObject);
                        yield break;
                    }
                    if (component2 != null)
                    {
                        component2.transform.position = new Vector3(startPos.x, startPos.y, 0);

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
                    if (parent.IsEnded || parent.Destroyed)
                    {
                        Destroy(component2.gameObject);
                        yield break;
                    }
                    float t = (float)elapsed / (float)Time;
                    if (component2 != null)
                    {
                        component2.transform.position = new Vector3(startPos.x, startPos.y, 0);
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
                Destroy(component2.gameObject);
                base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
                for (int i = 0; i < buletAmount; i++)
                {
                    base.Fire(Offset.OverridePosition(startPos), new Direction(aimDir, DirectionType.Absolute, -1f), new Speed(BulletSpeed, SpeedType.Absolute), new WallBulletNoDodge("sniperUndodgeable"));
                    yield return new WaitForSeconds(0.025f);
                }
                yield break;
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
                public RotatedBulletBasic(float spinspeed, float RevUp, float StartSpeenAgain, string BulletType, SubphaseOneAttack parent, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
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
                private SubphaseOneAttack m_parent;
                private float m_angle;
                private float m_spinSpeed;
                private float m_radius;
                private string m_bulletype;
                private float TimeToRevUp;
                private float StartAgain;


            }

            public class RotatedBullet : Bullet
            {
                public RotatedBullet(float spinspeed, float RevUp, float StartSpeenAgain, string BulletType, SubphaseOneAttack parent, bool IsEarliestRing, float angle = 0f, float startRadius = 50) : base(BulletType, false, false, false)
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

                        radius -= (BraveTime.DeltaTime*1.15f);
                        if (radius < 8.75f) { this.Projectile.DieInAir(false);yield break; }
                        if (m_parent.IsEnded || m_parent.Destroyed) {
                            this.StartTask(ChangeSpinSpeedTask(radius,centerPosition));
                            yield break;
                        }
                        centerPosition += this.Velocity / 60f;
                        base.UpdateVelocity();
                        this.m_angle += this.m_spinSpeed / 60f;
                        base.Projectile.shouldRotate = true;
                        base.Direction = this.m_angle;
                        base.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);
                        if (IsEarliestRing == true) { CurrentRingRadius = radius; }

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
                        radius += 0.01f*(i/2);
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
                private SubphaseOneAttack m_parent;
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
