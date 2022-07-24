 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using Brave.BulletScript;
using Dungeonator;
using System.Reflection;

namespace Planetside
{
    internal class PrisonerSecondSubPhaseController : BraveBehaviour
    {
        public void Start()
        {
            HoleTriggered = false;
            HasTriggeredLastStand = false;
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
            if (Actor.healthHaver.GetCurrentHealth() == 1 && SubPhaseEnded == true && FirstSubPhaseController.IsSubPhaseEnded() == true && HasTriggeredLastStand == false)
            {
                HasTriggeredLastStand = true;
                StartDeathSequence();
            }
        }

        public bool HasTriggeredLastStand;

        public void StartDeathSequence()
        {
            KillRing = true;
            if (controllerOfTheVoid) { controllerOfTheVoid.CanHurt = false; }
            Pixelator.Instance.FadeToColor(1f, Color.cyan, true, 0.5f);
            AkSoundEngine.PostEvent("Stop_MUS_All", base.gameObject);
            AkSoundEngine.PostEvent("Play_BOSS_DragunGold_Crackle_01", Actor.gameObject);
            AkSoundEngine.PostEvent("Play_PrisonerCharge", Actor.gameObject);
            Actor.specRigidbody.enabled = false;
            StaticReferenceManager.DestroyAllEnemyProjectiles();
            Actor.behaviorSpeculator.InterruptAndDisable();
            GameUIBossHealthController gameUIBossHealthController = GameUIRoot.Instance.bossController;
            gameUIBossHealthController.DisableBossHealth();
            GameManager.Instance.StartCoroutine(StartDeathSequenceCoroutine());
        }



        //i hate this, i hate this, i hate this, i hate this, i hate this, i hate this, i hate this, i hate this, i hate this, i hate this, i hate this, i hate this, i hate this, i hate this, i hate this, i hate this, i hate this, 
        private IEnumerator StartDeathSequenceCoroutine()
        {
            Vector2 positionToGoOffOf = Actor.sprite.WorldCenter;
            float elaWait = 0f;
            while (elaWait < 1f)
            {
                if (controllerOfTheVoid) { controllerOfTheVoid.ChangeHoleSize(Mathf.Lerp(24, 50, elaWait)); }
                if (controllerOfTheVoid) { controllerOfTheVoid.gameObject.transform.localScale = Vector3.Lerp(Vector3.one * 50, Vector3.one * 100, elaWait); }

                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            if (controllerOfTheVoid.gameObject) { Destroy(controllerOfTheVoid.gameObject, 0); }
            Controller.MoveTowardsCenterMethod(2f);
            AkSoundEngine.PostEvent("Play_PrisonerCough", base.gameObject);
            while (elaWait < 3f)
            {
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            Actor.aiAnimator.PlayUntilFinished("chargedeath", true, null, -1f, false);
            elaWait = 0f;
            while (elaWait < 0.5f)
            {
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(Actor.transform.Find("OrbPoint").position, 10, 0.1f, 30, 2f));
            StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(Actor.transform.Find("OrbPoint").position);
            Actor.aiAnimator.PlayUntilFinished("firedeath", true, null, -1f, false);

            GameObject portalObject = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal"));
            portalObject.transform.position = Actor.sprite.WorldCenter;
            portalObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            AkSoundEngine.PostEvent("Play_PortalOpen", portalObject.gameObject);

            GameManager.Instance.StartCoroutine(DoInversePulse(portalObject));
            elaWait = 0f;

            while (elaWait < 15f)
            {
                float t = (float)elaWait / (float)15;
                float t1 = Mathf.Sin(t * (Mathf.PI / 2));
                float r = (float)elaWait / (float)5;
                float r1 = Mathf.Sin(t * (Mathf.PI / 2));
                if (r1 > 1) { r1 = 1; }

                if (CheckIfPlayerInVoidHole(Mathf.Lerp(1, 12, t1), base.aiActor.sprite.WorldCenter))
                {
                    DoHoleFall(portalObject);
                    yield break;
                }
              
                DoSmallPush(t1*5.33f, base.aiActor.sprite.WorldCenter);
                portalObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 22.5f, t1);
                portalObject.GetComponent<MeshRenderer>().material.SetFloat("_OutlineWidth", Mathf.Lerp(0, 0.025f, r1));
                portalObject.GetComponent<MeshRenderer>().material.SetFloat("_OutlinePower", Mathf.Lerp(0, 100, r1));
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            Actor.renderer.enabled = false;
            Actor.SetOutlines(false);

            elaWait = 0f;
            while (elaWait < 5f)
            {
                
                if (CheckIfPlayerInVoidHole(12, base.aiActor.sprite.WorldCenter))
                {
                    DoHoleFall(portalObject);
                    yield break;
                }
                DoSmallPush(80, base.aiActor.sprite.WorldCenter);
                elaWait += BraveTime.DeltaTime;
            }
            Actor.healthHaver.minimumHealth = 0;
            Actor.healthHaver.ApplyDamage(1000, GameManager.Instance.BestActivePlayer.sprite.WorldCenter, "nerd", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                player.specRigidbody.Velocity = Vector2.zero;
            }
            AkSoundEngine.PostEvent("Play_BOSS_DragunGold_Crackle_01", portalObject.gameObject);
            GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(positionToGoOffOf, 2, 0.5f, 50f, 2f));

            bool hkjbsa = false;
            elaWait = 0f;
            while (elaWait < 2f)
            {
                float t = (float)elaWait / (float)2;
                float t1 = Mathf.Sin(t * (Mathf.PI / 2));
                portalObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 22.5f, 1-t1);
                portalObject.GetComponent<MeshRenderer>().material.SetFloat("_OutlineWidth", Mathf.Lerp(0, 0.025f, 1-t1));
                portalObject.GetComponent<MeshRenderer>().material.SetFloat("_OutlinePower", Mathf.Lerp(0, 100, 1-t1));
                if (hkjbsa == false && elaWait > 1.8f)
                {
                    hkjbsa = true;
                    var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PortalClose"));

                    AkSoundEngine.PostEvent("Play_PortalOpen", partObj.gameObject);
                    AkSoundEngine.PostEvent("Play_BOSS_spacebaby_explode_01", partObj.gameObject);
                    Exploder.DoDistortionWave(positionToGoOffOf, 5, 3, 100, 5);
                    partObj.transform.position = positionToGoOffOf;
                    partObj.transform.localScale *= 7f;
                    Destroy(partObj, 3.4f);
                    Pixelator.Instance.FadeToColor(1f, Color.cyan, true, 1f);
                }
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            Destroy(portalObject);


            SpawnReward(new IntVector2(0, 2), TatteredRobe.PrisonItemID);
            if (TookDamage() == false) { SpawnReward(new IntVector2(0, 4), OrbOfPower.PrisonItemID); }

            yield break;
        }

        public void SpawnReward(IntVector2 offset, int ID)
        {
            GameObject gameObject = GameManager.Instance.Dungeon.sharedSettingsPrefab.ChestsForBosses.SelectByWeight();
            Chest chest = gameObject.GetComponent<Chest>();
            if (chest != null)
            {

            }
            else
            {
                DungeonData data = GameManager.Instance.Dungeon.data;
                RewardPedestal component = gameObject.GetComponent<RewardPedestal>();
                if (component)
                {
                    RewardPedestal rewardPedestal = RewardPedestal.Spawn(component, GameManager.Instance.PrimaryPlayer.CurrentRoom.GetCenteredVisibleClearSpot(2, 2) + offset);
                    rewardPedestal.contents = PickupObjectDatabase.GetById(ID);

                }
            }
        }

        public bool TookDamage()
        {
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                if (player.CurrentRoom.PlayerHasTakenDamageInThisRoom == true) { return true; }
            }
            return false;
        }


        public bool HoleTriggered;

        public IEnumerator DoInversePulse(GameObject portal)
        {
            float elaWait = 0f;
            float power = 0.25f;
            float delay = 1;
            for (int j = 0; j < 22; j++)
            {
                if (HoleTriggered == true) { yield break; }
                elaWait = 0f;
                while (elaWait < delay)
                {
                    elaWait += BraveTime.DeltaTime;
                    yield return null;
                }
                GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(portal.transform.position, power, 0.2f, 40f, delay / 2));
                AkSoundEngine.PostEvent("Play_PortalCall", portal.gameObject);
                if (delay > 0.5f) { delay -= 0.05f; }
                power += 0.0625f;
            }
            yield break;
        }


        public void DoHoleFall(GameObject portal)
        {
            StaticReferenceManager.DestroyAllEnemyProjectiles();
            Minimap.Instance.ToggleMinimap(false, false);
            GameManager.IsBossIntro = true;
         
            GameManager.Instance.PreventPausing = true;
            GameUIRoot.Instance.HideCoreUI(string.Empty);
            GameUIRoot.Instance.ToggleLowerPanels(false, false, string.Empty);
               
            CameraController m_camera = GameManager.Instance.MainCameraController;
            m_camera.StopTrackingPlayer();
            m_camera.SetManualControl(true, false);
            m_camera.OverridePosition = m_camera.transform.position;
            Minimap.Instance.TemporarilyPreventMinimap = true;


            GameManager.Instance.StartCoroutine(DoPortalExpand(portal));
            HoleTriggered = true;

        }

        public IEnumerator DoPortalExpand(GameObject portal)
        {
            AkSoundEngine.PostEvent("Play_CHR_forever_fall_01", portal.gameObject);
            Vector3 scale = portal.transform.localScale;
            float elaWait = 0f;
            elaWait = 0f;
            while (elaWait < 1.33f)
            {
                float t = elaWait * 0.8f;
                portal.transform.localScale = Vector3.Lerp(scale, Vector3.one * 100f, t);
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_PrisonerLaugh", portal.gameObject);
            elaWait = 0f;
            Pixelator.Instance.FadeToBlack(2f, false, 0f);
            while (elaWait < 2.5f)
            {         
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            GameManager.Instance.LoadCustomLevel("tt_abyss");

            yield break;
        }

        public void DoSmallPush(float PushPower, Vector2 centerPosition)
        {
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                Vector2 pushPos = centerPosition - player.sprite.WorldCenter;
                player.specRigidbody.Velocity += BraveMathCollege.DegreesToVector(pushPos.ToAngle()).normalized * PushPower;
            }
        }

        public bool CheckIfPlayerInVoidHole(float radius, Vector2 centerPosition)
        {
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                if (Vector2.Distance(player.transform.position, centerPosition) < radius) { return true; }
            }
            return false;
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


            GameObject partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Amogus"));
            MeshRenderer rend = partObj.GetComponentInChildren<MeshRenderer>();
            rend.allowOcclusionWhenDynamic = true;
            partObj.transform.position = Actor.ParentRoom.GetCenterCell().ToVector3().WithZ(50);
            partObj.name = "VoidHole";
            partObj.transform.localScale = Vector3.zero;
            VoidHoleController voidHoleController = partObj.AddComponent<VoidHoleController>();
            voidHoleController.trueCenter = Actor.sprite.WorldCenter;
            voidHoleController.CanHurt = false;
            voidHoleController.Radius = 30;
            controllerOfTheVoid = voidHoleController;

            EmergencyPlayerDisappearedFromRoom emergencyPlayerDisappeared = Actor.gameObject.AddComponent<EmergencyPlayerDisappearedFromRoom>();
            emergencyPlayerDisappeared.roomAssigned = Actor.GetAbsoluteParentRoom();
            emergencyPlayerDisappeared.PlayerSuddenlyDisappearedFromRoom = (obj) =>
            {
                if (obj.IsDarkAndTerrifying == true)
                {
                    obj.EndTerrifyingDarkRoom(1);
                }
                if (partObj != null) { Destroy(partObj); }
            };

            GameManager.Instance.BestActivePlayer.CurrentRoom.BecomeTerrifyingDarkRoom(5f, 0.5f, 0.1f, "Play_ENM_darken_world_01");
             while (elaWait < 3f)
             {
                float t = elaWait / 3;
                float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
                partObj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 6.25f, throne1);
                Actor.renderer.material.SetFloat("_Fade", 1 - t);
                elaWait += BraveTime.DeltaTime;
                 yield return null;
             }
            SpawnManager.SpawnBulletScript(Actor, Actor.sprite.WorldCenter, Actor.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(SubphaseTwoAttack)), StringTableManager.GetEnemiesString("#PRISONERPHASEONENAME", -1));
             Actor.SetOutlines(false);
             Actor.renderer.enabled = false;
             elaWait = 0f;
            voidHoleController.CanHurt = true;
            while (elaWait < 30f)
             {
                 elaWait += BraveTime.DeltaTime;
                float t = Mathf.Min((elaWait / 20), 1);
                voidHoleController.Radius = Mathf.Lerp(28.5f, 8.75f, t);
                partObj.transform.localScale = Vector3.Lerp(Vector3.one * 6.25f, Vector3.one * 2, t);
                yield return null;
             }
            voidHoleController.actorToFollow = Actor;
            voidHoleController.trueCenter = Actor.sprite.WorldCenter;
            Actor.CollisionDamage = 0;

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
            //SpawnManager.SpawnBulletScript(Actor, Actor.sprite.WorldCenter, Actor.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(A)), StringTableManager.GetEnemiesString("#PRISONERPHASEONENAME", -1));
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


        public VoidHoleController controllerOfTheVoid;


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
            {"WallSweepThree", 0f },
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
                        angle += 0.5f;
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
                    base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableDoorLordBurst);
                    base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBig);

                }
                this.EndOnBlank = false;

                int e = 0;
                int H = 0;
                int Speed = 180;
                for (; ; )
                {
                    if (SubPhaseEnded == true) { this.Destroyed = true; yield break; }
                    if (e % 40 == 0)
                    {
                        this.FireWallBullets("top");
                    }
                    if (H == Speed)
                    {
                        H = 0; Speed -= 12;
                        Speed = Mathf.Max(70, Speed);
                        this.StartTask(SpawnBoulder());
                    }
                    e++; H++;
                    yield return base.Wait(1);
                }
            }

            public IEnumerator SpawnBoulder()
            {
                Vector2 pos = GameManager.Instance.BestActivePlayer.sprite.WorldCenter + MathToolbox.GetUnitOnCircle(this.RandomAngle(), UnityEngine.Random.Range(7, 9));
                StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(pos);
                yield return this.Wait(60f);
                base.PostWwiseEvent("Play_BOSS_spacebaby_explode_01", null);
                base.Fire(Offset.OverridePosition(pos), new Direction(this.GetAimDirection(pos, 1,300), Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new MegaBoulder());
            }

            public class MegaBoulder : Bullet
            {
                public MegaBoulder() : base(StaticUndodgeableBulletEntries.undodgeableBig.Name, true, false, true)
                {
                }
                protected override IEnumerator Top()
                {
                    this.Projectile.IgnoreTileCollisionsFor(180f);
                    this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
                    base.ChangeSpeed(new Brave.BulletScript.Speed(20, SpeedType.Absolute),90);
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

                //base.PostWwiseEvent("Play_RockBreaking", null);
                for (int l = 0; l < Tiles; l++)
                {
                    float t = (float)l / (float)Tiles;
                    Vector2 SpawnPos = Vector2.Lerp(OneCorner, OtherCorner, t);
                    if (l % 4 == 1)
                    {
                        int travelTime = UnityEngine.Random.Range(90, 360);
                        base.Fire(Offset.OverridePosition(SpawnPos), new Direction(facingDir, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(5.5f, 6.25f), SpeedType.Absolute), new WallBullets(StaticUndodgeableBulletEntries.UndodgeableDoorLordBurst.Name, facingDir));
                    }
                }
            }

            public class WallBullets : Bullet
            {
                public WallBullets(string bulletName, float angle) : base(bulletName, true, false, false)
                {
                    this.Angle = angle;
                }
                protected override IEnumerator Top()
                {
                    this.Projectile.IgnoreTileCollisionsFor(180f);
                    this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
                    base.ChangeDirection(new Brave.BulletScript.Direction(UnityEngine.Random.Range(-15, 15), DirectionType.Relative), UnityEngine.Random.Range(100, 300));
                    yield break;
                }

                private float Angle;
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
