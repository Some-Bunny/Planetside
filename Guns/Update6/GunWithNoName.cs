using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using Alexandria.Assetbundle;
using Pathfinding;
using Alexandria.EnemyAPI;
using Planetside.Components.Projectile_Components;
using static Planetside.Glass;
using static Planetside.PrisonerSecondSubPhaseController;
using Planetside.Static_Storage;
using static ETGMod;

namespace Planetside
{

	public class GWNN_Hitscan : Projectile
	{
        Path myPathToDestiny;
        public AIActor MyTarget;
        private AIActor GetSimplifiedNewTarget()
        {
            List<AIActor> enmL = new List<AIActor>();
            (Owner as PlayerController).CurrentRoom?.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref enmL);
            if (enmL == null) { return null; }
            if (enmL.Count == 0) { return null; }
            float RangeMult = CalculateRangeMultiplier();
            enmL.RemoveAll(self => self.State != AIActor.ActorState.Normal);
            enmL.RemoveAll(self => self.specRigidbody == null);
            enmL = enmL.OrderByDescending(self => self.healthHaver.currentHealth).ToList();
            if (WasInitial == true)
            {
                enmL.RemoveAll(self => Vector2.Distance(this.specRigidbody.UnitCenter, self.specRigidbody.UnitCenter) > (BounceModifiersToAdd[0].Second * RangeMult)); 
            }

            if (enmL.Count == 0) { return null; }
            enmL.RemoveAll(self => self.healthHaver.IsDead);
            enmL.RemoveAll(self => self.healthHaver.vulnerable == false);
            enmL.RemoveAll(self => self.spriteAnimator.QueryInvulnerabilityFrame() == true);
            if (enmL.Count == 0) { return null; }
            AIActor t = null;
            if (enmL.Count == 1)
            {
                return enmL[0];
            }
            for (int i = 0; i < enmL.Count; i++)
            {
                if (enmL[i] == MyTarget) { continue; }
                t = enmL[i]; break;
            }
            return t;
        }



        public override void Start()
        {
            base.Start();    
            this.BulletScriptSettings = new BulletScriptSettings() { overrideMotion = false, surviveRigidbodyCollisions = true, surviveTileCollisions = true };
            this.specRigidbody.OnRigidbodyCollision += (_) =>
            {
                PhysicsEngine.SkipCollision = true;
            };
            var pierce = this.GetComponent<PierceProjModifier>();
            var bounceOnit = this.GetComponent<BounceProjModifier>();
            var homer = this.GetComponent<HomingModifier>();
            if (homer != null)
            {
                int amountOfExtra = 1 + Mathf.RoundToInt( (homer.AngularVelocity / 300));
                PostInitialHitBounceRange += (homer.HomingRadius * 0.02f);
                for (int i = 0; i < amountOfExtra; i++)
                {
                    BounceModifiersToAdd.Add(new Tuple<float, float>(1, PostInitialHitBounceRange));
                }
            }
            if (pierce != null)
            {
                for (int i = 0; i < pierce.penetration; i++)
                {
                    BounceModifiersToAdd.Add(new Tuple<float, float>(0.75f, PostInitialHitBounceRange));
                }
            }
            if (bounceOnit != null)
            {
                for (int i = 0; i < bounceOnit.numberOfBounces; i++)
                {
                    BounceModifiersToAdd.Add(new Tuple<float, float>(bounceOnit.damageMultiplierOnBounce, PostInitialHitBounceRange));
                }
            }
            Cooldown = 0.05f;
            MyTarget = MyTarget ?? GetSimplifiedNewTarget();
            Scale = this.AdditionalScaleMultiplier;
        }
        public List<Tuple<float, float>> BounceModifiersToAdd = new List<Tuple<float, float>>() { };
        public float PostInitialHitBounceRange = 15f;
        private bool WasInitial = false;
        private float Scale = 1;


        private float CalculateRangeMultiplier()
        {
            return Mathf.Min(25, this.baseData.range / 10f);
        }

        public override void Move()
        {
            if (Cooldown > 0) { Cooldown -= Time.deltaTime;return; }
            //Debug.Log(BounceModifiersToAdd.Count);
            RebuildPath();
            if (this.MyTarget == null) {  return; }
            Vector3 lastPos = LastPosition;
            if (myPathToDestiny == null)
            {
                this.DieInAir(); return; 
            }
            float DistTick = 0;
            while (myPathToDestiny.Count > 0)
            {
                var PathTo = myPathToDestiny.GetFirstCenterVector2();
                var PathMe = lastPos;
                DistTick = (Vector2.Distance(PathMe, PathTo) * 8);
                this.LastVelocity = (PathTo.ToVector3ZUp() - PathMe);
                for (float i = 0; i < DistTick; i++)
                {
                    float t = (float)i / DistTick;
                    Vector3 vector3 = Vector3.Lerp(PathMe, PathTo, t);
                    GlobalSparksDoer.DoSingleParticle(
                        vector3,
                        Vector3.zero,
                        0.1f * Scale,
                        0.333f,
                        new Color(1, 0.4f, 0, 1),
                        GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                }
                lastPos = PathTo;
                myPathToDestiny.RemoveFirst();
            }
            AkSoundEngine.PostEvent("Play_WPN_HLD_shot_03", this.gameObject);
            ParticleBase.EmitParticles("HexParticle", 1, new ParticleSystem.EmitParams()
            {
                position = this.sprite.WorldCenter,
                startSize = 6 * Scale,
                rotation = 0,
                startLifetime = 0.125f,
                startColor = new Color(1, 0.4f, 0).WithAlpha(1f),
            });
            Vector2 unitCenter3 = MyTarget.specRigidbody.HitboxPixelCollider.UnitCenter;
            LastPosition = unitCenter3;
            this.transform.position = unitCenter3;
            this.specRigidbody.Reinitialize();
            DistTick = (Vector2.Distance(lastPos, unitCenter3) * 8);
            for (float i = 0; i < DistTick; i++)
            {
                float t = (float)i / DistTick;
                Vector3 vector3 = Vector3.Lerp(lastPos, unitCenter3, t);
                GlobalSparksDoer.DoSingleParticle(
                    vector3,
                    Vector3.zero,
                    0.1f * Scale,
                    0.333f,
                    new Color(1, 0.4f, 0, 1),
                    GlobalSparksDoer.SparksType.FLOATY_CHAFF);
            }
            ParticleBase.EmitParticles("HexParticleFull", 1, new ParticleSystem.EmitParams()
            {
                position = unitCenter3,
                startSize = 3 * Scale,
                rotation = 0,
                startLifetime = 0.333f,
                startColor = new Color(1, 0.4f, 0).WithAlpha(2f),
            });
            if (WasInitial)
            {
                this.baseData.damage *= BounceModifiersToAdd[0].First;
                BounceModifiersToAdd.RemoveAt(0);
            }
            this.ForceCollision(MyTarget.specRigidbody, new LinearCastResult() 
            {
                CollidedX = true,
                CollidedY = true,
                Contact = MyTarget.specRigidbody.UnitCenter,
                MyPixelCollider = this.specRigidbody.PixelColliders[0],
                NewPixelsToMove = new IntVector2(0,0),
                Normal = Vector2.zero,
                OtherPixelCollider = MyTarget.specRigidbody.PixelColliders[0],
                Overlap = true,
                TimeUsed = 0,
            });
            //MyTarget.healthHaver.ApplyDamage(this.ModifiedDamage, Vector2.zero, base.OwnerName, CoreDamageTypes.Void, DamageCategory.Unstoppable, false, null, false);
            base.LastVelocity = (MyTarget.CenterPosition - base.Owner.CenterPosition).normalized * 0.1f;
            //base.HandleKnockback(MyTarget.specRigidbody, base.Owner as PlayerController, true, false);
            if (BounceModifiersToAdd.Count == 0)
            {
                this.DieInAir();
            }
            else
            {
                WasInitial = true;
                Cooldown = 0.15f;
                MyTarget = GetSimplifiedNewTarget();
            }
        }


        public override void OnRigidbodyCollision(CollisionData rigidbodyCollision)
        {
            if (base.specRigidbody.IsGhostCollisionException(rigidbodyCollision.OtherRigidbody))
            {
                return;
            }
            GameObject gameObject = rigidbodyCollision.OtherRigidbody.gameObject;
            SpeculativeRigidbody otherRigidbody = rigidbodyCollision.OtherRigidbody;
            PlayerController component = otherRigidbody.GetComponent<PlayerController>();
            bool flag;
            Projectile.HandleDamageResult handleDamageResult = this.HandleDamage(rigidbodyCollision.OtherRigidbody, rigidbodyCollision.OtherPixelCollider, out flag, component, false);
            bool flag2 = handleDamageResult != Projectile.HandleDamageResult.NO_HEALTH;
            if (this.braveBulletScript && this.braveBulletScript.bullet != null && this.BulletScriptSettings.surviveTileCollisions && !flag2 && rigidbodyCollision.OtherPixelCollider.CollisionLayer == CollisionLayer.HighObstacle)
            {
                if (!otherRigidbody.minorBreakable)
                {
                    this.braveBulletScript.bullet.ManualControl = true;
                    this.braveBulletScript.bullet.Position = base.specRigidbody.UnitCenter;
                    PhysicsEngine.PostSliceVelocity = new Vector2?(Vector2.zero);
                }
                return;
            }
            this.HandleSparks(new Vector2?(rigidbodyCollision.Contact));
            if (flag2)
            {
                this.m_hasImpactedEnemy = true;
                if (this.OnHitEnemy != null)
                {
                    this.OnHitEnemy(this, rigidbodyCollision.OtherRigidbody, flag);
                }
            }
            else if (ChallengeManager.CHALLENGE_MODE_ACTIVE && (otherRigidbody.GetComponent<BeholsterBounceRocket>() || otherRigidbody.healthHaver || otherRigidbody.GetComponent<BashelliskBodyPickupController>() || otherRigidbody.projectile))
            {
                this.m_hasImpactedEnemy = true;
            }
            PierceProjModifier pierceProjModifier = base.GetComponent<PierceProjModifier>();
            BounceProjModifier bounceProjModifier = base.GetComponent<BounceProjModifier>();
            if (this.m_hasImpactedEnemy && pierceProjModifier && otherRigidbody.healthHaver && otherRigidbody.healthHaver.IsBoss)
            {
                if (pierceProjModifier.HandleBossImpact())
                {
                    bounceProjModifier = null;
                    pierceProjModifier = null;
                }
            }
            if (base.GetComponent<KeyProjModifier>())
            {
                Chest component2 = otherRigidbody.GetComponent<Chest>();
                if (component2 && component2.IsLocked && component2.ChestIdentifier != Chest.SpecialChestIdentifier.RAT)
                {
                    component2.ForceUnlock();
                }
            }
            MinorBreakable minorBreakable = otherRigidbody.minorBreakable;
            MajorBreakable majorBreakable = otherRigidbody.majorBreakable;
            if (majorBreakable != null)
            {
                float num = 1f;
                if (((this.m_shooter != null && this.m_shooter.aiActor != null) || this.m_owner is AIActor) && majorBreakable.InvulnerableToEnemyBullets)
                {
                    num = 0f;
                }
                if (pierceProjModifier != null && pierceProjModifier.BeastModeLevel == PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE)
                {
                    if (majorBreakable.ImmuneToBeastMode)
                    {
                        num += 1f;
                    }
                    else
                    {
                        num = 1000f;
                    }
                }
                if (!majorBreakable.IsSecretDoor || !(this.PossibleSourceGun != null) || !this.PossibleSourceGun.InfiniteAmmo)
                {
                    float num2 = (!(this.Owner is AIActor)) ? this.ModifiedDamage : ProjectileData.FixedEnemyDamageToBreakables;
                    if (num2 <= 0f && GameManager.Instance.InTutorial)
                    {
                        majorBreakable.ApplyDamage(1.5f, base.specRigidbody.Velocity, false, false, false);
                    }
                    else
                    {
                        majorBreakable.ApplyDamage(num2 * num, base.specRigidbody.Velocity, this.Owner is AIActor, false, false);
                    }
                }
            }
            if (rigidbodyCollision.OtherRigidbody.PreventPiercing)
            {
                pierceProjModifier = null;
            }
            if (!flag2 && bounceProjModifier && !minorBreakable && (!bounceProjModifier.onlyBounceOffTiles || !majorBreakable) && !pierceProjModifier && (!bounceProjModifier.useLayerLimit || rigidbodyCollision.OtherPixelCollider.CollisionLayer == bounceProjModifier.layerLimit))
            {
                this.OnTileCollision(rigidbodyCollision);
                return;
            }
            bool flag4 = majorBreakable && majorBreakable.IsSecretDoor;
            if (!majorBreakable && otherRigidbody.name.StartsWith("secret exit collider"))
            {
                flag4 = true;
            }
            if (flag4)
            {
                this.OnTileCollision(rigidbodyCollision);
                return;
            }
            if (otherRigidbody.ReflectProjectiles)
            {
                AkSoundEngine.PostEvent("Play_OBJ_metalskin_deflect_01", GameManager.Instance.gameObject);
                if (this.IsBulletScript && bounceProjModifier && bounceProjModifier.removeBulletScriptControl)
                {
                    this.RemoveBulletScriptControl();
                }
                Vector2 vector = rigidbodyCollision.Normal;
                if (otherRigidbody.ReflectProjectilesNormalGenerator != null)
                {
                    vector = otherRigidbody.ReflectProjectilesNormalGenerator(rigidbodyCollision.Contact, rigidbodyCollision.Normal);
                }
                float num3 = (-rigidbodyCollision.MyRigidbody.Velocity).ToAngle();
                float num4 = vector.ToAngle();
                float num5 = BraveMathCollege.ClampAngle360(num3 + 2f * (num4 - num3));
                if (this.shouldRotate)
                {
                    this.m_transform.rotation = Quaternion.Euler(0f, 0f, num5);
                }
                this.m_currentDirection = BraveMathCollege.DegreesToVector(num5, 1f);
                if (this.braveBulletScript && this.braveBulletScript.bullet != null)
                {
                    this.braveBulletScript.bullet.Direction = num5;
                }
                if (!bounceProjModifier || !bounceProjModifier.suppressHitEffectsOnBounce)
                {
                    this.HandleHitEffectsEnemy(rigidbodyCollision.OtherRigidbody, rigidbodyCollision, false);
                }
                Vector2 value = this.m_currentDirection * this.m_currentSpeed * this.LocalTimeScale;
                PhysicsEngine.PostSliceVelocity = new Vector2?(value);
                if (rigidbodyCollision.OtherRigidbody)
                {
                    base.specRigidbody.RegisterTemporaryCollisionException(rigidbodyCollision.OtherRigidbody, 0f, new float?(0.5f));
                    rigidbodyCollision.OtherRigidbody.RegisterTemporaryCollisionException(base.specRigidbody, 0f, new float?(0.5f));
                }
                if (otherRigidbody.knockbackDoer && otherRigidbody.knockbackDoer.knockbackWhileReflecting)
                {
                    this.HandleKnockback(otherRigidbody, component, false, false);
                }
                return;
            }
            bool flag5 = false;
            bool flag6 = false;
            if (flag2)
            {
                if (!flag || !(component != null))
                {
                    flag5 = true;
                }
            }
            else
            {
                flag6 = true;
            }
            if (!Projectile.s_delayPlayerDamage || !component)
            {
                if (flag2)
                {
                    if (!rigidbodyCollision.OtherRigidbody.healthHaver.IsDead || flag)
                    {
                        this.HandleKnockback(rigidbodyCollision.OtherRigidbody, component, flag, false);
                    }
                }
                else
                {
                    this.HandleKnockback(rigidbodyCollision.OtherRigidbody, component, false, false);
                }
            }
            if (!component)
            {
                AppliedEffectBase[] components = base.GetComponents<AppliedEffectBase>();
                foreach (AppliedEffectBase appliedEffectBase in components)
                {
                    appliedEffectBase.AddSelfToTarget(gameObject);
                }
            }
            base.specRigidbody.RegisterTemporaryCollisionException(rigidbodyCollision.OtherRigidbody, 0.01f, new float?(0.5f));
            PhysicsEngine.CollisionHaltsVelocity = new bool?(false);
            Projectile projectile = rigidbodyCollision.OtherRigidbody.projectile;
            if (this.CanTransmogrify && flag2 && handleDamageResult != Projectile.HandleDamageResult.HEALTH_AND_KILLED && UnityEngine.Random.value < this.ChanceToTransmogrify && otherRigidbody.aiActor && !otherRigidbody.aiActor.IsMimicEnemy && otherRigidbody.aiActor.healthHaver && !otherRigidbody.aiActor.healthHaver.IsBoss && otherRigidbody.aiActor.healthHaver.IsVulnerable)
            {
                otherRigidbody.aiActor.Transmogrify(EnemyDatabase.GetOrLoadByGuid(this.TransmogrifyTargetGuids[UnityEngine.Random.Range(0, this.TransmogrifyTargetGuids.Length)]), (GameObject)ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"));
            }
            if (pierceProjModifier != null && pierceProjModifier.preventPenetrationOfActors && flag2)
            {
                pierceProjModifier = null;
            }
            bool flag7 = false;
            bool flag8 = false;
            bool flag9 = otherRigidbody && otherRigidbody.GetComponent<PlayerOrbital>();
            if (this.BulletScriptSettings.surviveRigidbodyCollisions)
            {
                flag7 = true;
                flag8 = true;
            }
            else if (pierceProjModifier != null && pierceProjModifier.BeastModeLevel == PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE)
            {
                flag7 = true;
                flag8 = true;
            }
            else if (pierceProjModifier != null && pierceProjModifier.penetration > 0 && flag2)
            {
                flag7 = true;
                flag8 = true;
            }
            else if (pierceProjModifier != null && pierceProjModifier.penetratesBreakables && pierceProjModifier.penetration > 0)
            {
                flag7 = true;
                flag8 = true;
            }
            else if (projectile && this.projectileHitHealth > 0)
            {
                PierceProjModifier component3 = projectile.GetComponent<PierceProjModifier>();
                if ((component3 && component3.BeastModeLevel == PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE) || projectile is RobotechProjectile)
                {
                    projectile.m_hasImpactedEnemy = true;
                }
                else
                {
                    projectile.m_hasImpactedEnemy = true;
                }
                flag7 = (this.projectileHitHealth >= 0);
                flag8 = flag7;
            }
            else if (minorBreakable && this.pierceMinorBreakables)
            {
                flag7 = true;
                flag8 = true;
            }
            else if (bounceProjModifier != null && !flag2 && !this.m_hasImpactedEnemy)
            {
                bounceProjModifier.HandleChanceToDie();
                if (flag2 && bounceProjModifier.ExplodeOnEnemyBounce)
                {
                    ExplosiveModifier component4 = base.GetComponent<ExplosiveModifier>();
                    if (component4)
                    {
                        bounceProjModifier.numberOfBounces = 0;
                    }
                }
                int num6 = 1;
                PierceProjModifier pierceProjModifier2 = null;
                if (otherRigidbody && otherRigidbody.projectile)
                {
                    pierceProjModifier2 = otherRigidbody.GetComponent<PierceProjModifier>();
                }
                if (pierceProjModifier2 && pierceProjModifier2.BeastModeLevel == PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE)
                {
                    num6 = 2;
                }
                bool flag10 = bounceProjModifier.numberOfBounces - num6 >= 0;
                flag10 &= (!bounceProjModifier.useLayerLimit || rigidbodyCollision.OtherPixelCollider.CollisionLayer == bounceProjModifier.layerLimit);
                flag10 &= !flag9;
                if (flag10)
                {
                    if (this.IsBulletScript && bounceProjModifier.removeBulletScriptControl)
                    {
                        this.RemoveBulletScriptControl();
                    }
                    Vector2 normal = rigidbodyCollision.Normal;
                    if (rigidbodyCollision.MyRigidbody)
                    {
                        Vector2 velocity = rigidbodyCollision.MyRigidbody.Velocity;
                        float num7 = (-velocity).ToAngle();
                        float num8 = normal.ToAngle();
                        float num9 = BraveMathCollege.ClampAngle360(num7 + 2f * (num8 - num7));
                        if (this.shouldRotate)
                        {
                            this.m_transform.rotation = Quaternion.Euler(0f, 0f, num9);
                        }
                        this.m_currentDirection = BraveMathCollege.DegreesToVector(num9, 1f);
                        this.m_currentSpeed *= 1f - bounceProjModifier.percentVelocityToLoseOnBounce;
                        if (this.braveBulletScript && this.braveBulletScript.bullet != null)
                        {
                            this.braveBulletScript.bullet.Direction = num9;
                            this.braveBulletScript.bullet.Speed *= 1f - bounceProjModifier.percentVelocityToLoseOnBounce;
                        }
                        Vector2 vector2 = this.m_currentDirection * this.m_currentSpeed * this.LocalTimeScale;
                        vector2 = bounceProjModifier.AdjustBounceVector(this, vector2, otherRigidbody);
                        if (this.shouldRotate && vector2.normalized != this.m_currentDirection)
                        {
                            this.m_transform.rotation = Quaternion.Euler(0f, 0f, BraveMathCollege.Atan2Degrees(vector2.normalized));
                        }
                        this.m_currentDirection = vector2.normalized;
  
                        if (this.OverrideMotionModule != null)
                        {
                            this.OverrideMotionModule.UpdateDataOnBounce(Mathf.DeltaAngle(velocity.ToAngle(), num9));
                        }
                        bounceProjModifier.Bounce(this, rigidbodyCollision.Contact, otherRigidbody);
                        PhysicsEngine.PostSliceVelocity = new Vector2?(vector2);
                        if (rigidbodyCollision.OtherRigidbody)
                        {
                            base.specRigidbody.RegisterTemporaryCollisionException(rigidbodyCollision.OtherRigidbody, 0f, new float?(0.5f));
                            rigidbodyCollision.OtherRigidbody.RegisterTemporaryCollisionException(base.specRigidbody, 0f, new float?(0.5f));
                        }
                        flag7 = true;
                    }
                }
            }
            if (flag5)
            {
                this.HandleHitEffectsEnemy(rigidbodyCollision.OtherRigidbody, rigidbodyCollision, !flag8 && !flag7);
            }
            if (flag6)
            {
                this.HandleHitEffectsObject(rigidbodyCollision.OtherRigidbody, rigidbodyCollision, !flag8 && !flag7);
            }
            this.m_hasPierced = (this.m_hasPierced || flag8);
            if (!flag8 && !flag7 && !this.m_hasImpactedObject)
            {
                this.m_hasImpactedObject = true;
                for (int j = 0; j < base.specRigidbody.PixelColliders.Count; j++)
                {
                    base.specRigidbody.PixelColliders[j].IsTrigger = true;
                }
                if (flag2 && base.gameObject.activeInHierarchy)
                {
                    base.StartCoroutine(this.HandlePostCollisionPersistence(rigidbodyCollision, component));
                }
                else
                {
                    this.HandleNormalProjectileDeath(rigidbodyCollision, !flag9);
                    PhysicsEngine.HaltRemainingMovement = true;
                }
            }
        }

        private float Cooldown = 0;




        public void RebuildPath()
        {
            if (this == null) { return;}
            if (MyTarget == null)
            {
                MyTarget = GetSimplifiedNewTarget();
                if (MyTarget == null)
                {
                    this.DieInAir();
                    return;
                }
            }
            Pathfinder.Instance.GetPath(new IntVector2((int)this.transform.position.x, (int)this.transform.position.y),
                new IntVector2((int)MyTarget.specRigidbody.UnitCenter.x, (int)MyTarget.specRigidbody.UnitCenter.y),
                out myPathToDestiny, null, CellTypes.WALL | CellTypes.FLOOR, null, null, true);
        }
    }


    public class GWNNSpawnedProjectile : MonoBehaviour
	{

		public Projectile Projectile;
		public PlayerController Owner;

        public void Start()
		{

            AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", this.gameObject);
            if (Projectile.Owner is PlayerController player)
			{
                Owner = player;

                if (!GunWithNoName.StoredGWNNs.ContainsKey(player))
				{
                    GunWithNoName.StoredGWNNs.Add(player, new List<GWNNSpawnedProjectile>());
                }
				GunWithNoName.StoredGWNNs[player].Add(this);
                GunWithNoName.StoredGWNNs[player].ForEach(self => self.WaitTime += (Owner ? 0.0333f / Owner.stats.GetStatValue(PlayerStats.StatType.RateOfFire) : 0.025f));
                //Debug.Log((Owner != null ? Owner.CurrentGun.GetPrimaryCooldown() / Owner.stats.GetStatValue(PlayerStats.StatType.RateOfFire) : 0.0333f));
                this.Invoke("Emit", 0.05f);
                this.StartCoroutine(DoLerpParticle());
            }
        }

       
        public void Emit()
        {
            int b = (BraveUtility.RandomBool() ? 1 : -1);

            ParticleBase.EmitParticles("HexParticle", 1, new ParticleSystem.EmitParams()
            {
                position = Projectile.sprite.WorldCenter,
                startSize = 2,
                rotation = 0,
                startLifetime = 0.333f,
                startColor = new Color(1, 0.4f, 0).WithAlpha(1f),
                angularVelocity = 240 * b
            });
            ParticleBase.EmitParticles("HexParticle", 1, new ParticleSystem.EmitParams()
            {
                position = Projectile.sprite.WorldCenter,
                startSize = 4.5f,
                rotation = 0,
                startLifetime = 0.333f,
                startColor = new Color(1, 0.4f, 0).WithAlpha(1f),
                angularVelocity = 240 * (b * -1f)
            });
        }

        public AIActor Target
        {
            get
            {
                if (_Target == null)
                {
                    _Target = GetNewTarget();
                }
                else if (_Target.healthHaver.IsDead)
                {
                    _Target = GetNewTarget();
                }
                return _Target;
            }
        }

        private AIActor GetNewTarget()
        {
            List<AIActor> enmL = new List<AIActor>();
            (Owner as PlayerController).CurrentRoom?.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref enmL);
            if (enmL == null) { return null; }

            enmL.RemoveAll(self => self.State != AIActor.ActorState.Normal);
            enmL.RemoveAll(self => self.specRigidbody == null);
           
            enmL = enmL.OrderBy(self => GetThreat(self)).ToList();
            enmL.RemoveAll(self => self.healthHaver.IsDead);
            enmL.RemoveAll(self => self.healthHaver.vulnerable == false);
            enmL.RemoveAll(self => self.spriteAnimator.QueryInvulnerabilityFrame() == true);
            if (enmL.Count == 0) { return null; }
            AIActor t = null;
            for (int i = 0; i < enmL.Count; i++)
            {
                var _ = enmL[i];
                bool b = false;

                if (GunWithNoName.StoredGWNNs[Owner as PlayerController].Count <= enmL.Count)
                {
                    foreach (var hitScan in GunWithNoName.StoredGWNNs[Owner as PlayerController])
                    {
                        if (hitScan == this) { continue; }
                        if (hitScan._Target == _) { b = true; break; }
                    }
                }
                if (b) { continue; }
                t = _;
            }
            return t;
        }
        public float GetThreat(AIActor aIActor)
        {
            float Health = (aIActor.healthHaver.currentHealth / 10);
            float dist = Mathf.Sqrt(Vector2.Distance(this.Projectile.specRigidbody.UnitCenter, aIActor.specRigidbody.UnitCenter) * 0.333f);
            return dist + Health;
        }



        public AIActor _Target;


        float WaitTime = 0.5f;
		public void Update()
		{
            if (Owner && Owner.CurrentGun.m_isCurrentlyFiring)
			{
                return;
            }
            if (Target == null) 
            {
                return;
            }
 
            ParticleBase.EmitParticles("HexParticle", 1, new ParticleSystem.EmitParams()
            {
                position = Target.sprite.WorldCenter,
                startSize = 1f,
                rotation = 0,
                startLifetime = 0.1f,
                startColor = new Color(1, 0.4f, 0).WithAlpha(1.25f),
                angularVelocity = 0
            });
            WaitTime -= Time.deltaTime;
			if (WaitTime <= 0)
			{
                DoFireHitscan();
            }
        }

        public IEnumerator DoLerpParticle()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.01f, 0.333f));

            while (this.Projectile)
            {
                if (Target != null)
                {
                    Path myPath = null;
                    Pathfinder.Instance.GetPath(new IntVector2((int)this.transform.position.x, (int)this.transform.position.y),
                        new IntVector2((int)Target.specRigidbody.UnitCenter.x, (int)Target.specRigidbody.UnitCenter.y),
                        out myPath, null, CellTypes.WALL | CellTypes.FLOOR, null, null, true);
                    if (myPath != null)
                    {
                        Vector3 lastPos = this.transform.position;
                        float DistTick = 0;
                        while (myPath.Count > 0)
                        {
                            var PathTo = myPath.GetFirstCenterVector2();
                            var PathMe = lastPos;
                            DistTick = (Vector2.Distance(PathMe, PathTo) * 3);
                            for (float i = 0; i < DistTick; i++)
                            {
                                float t = (float)i / DistTick;
                                Vector3 vector3 = Vector3.Lerp(PathMe, PathTo, t);
                                GlobalSparksDoer.DoSingleParticle(
                                    vector3,
                                    Vector3.zero,
                                    0.075f,
                                    0.125f,
                                    new Color(0.5f, 0.2f, 0, 0.4f),
                                    GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                            }
                            lastPos = PathTo;
                            myPath.RemoveFirst();
                        }
                      
                        Vector2 unitCenter3 = Target.specRigidbody.HitboxPixelCollider.UnitCenter;
                        DistTick = (Vector2.Distance(lastPos, unitCenter3) * 3);
                        for (float i = 0; i < DistTick; i++)
                        {
                            float t = (float)i / DistTick;
                            Vector3 vector3 = Vector3.Lerp(lastPos, unitCenter3, t);
                            GlobalSparksDoer.DoSingleParticle(
                                vector3,
                                Vector3.zero,
                                    0.075f,
                                    0.125f,
                                    new Color(0.5f, 0.2f, 0, 0.4f),
                                GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                        }
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
            yield break;
        }


        public void DoFireHitscan()
		{
            AkSoundEngine.PostEvent("Play_WPN_magnum_shot_01", this.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(GunWithNoName.Hitscan.gameObject, Projectile.sprite.WorldCenter, 
				Quaternion.Euler(0f, 0f, Owner.m_currentGunAngle), true);
            GWNN_Hitscan component = gameObject.GetComponent<GWNN_Hitscan>();
            if (component != null)
            {
                Owner.DoPostProcessProjectile(component);
                component.Owner = Owner  != null ? Owner : Projectile.Owner;
                component.Shooter = Owner != null ? Owner.specRigidbody : Projectile.Owner.specRigidbody;
                component.MyTarget = Target;
            }
            Emit();
            Projectile.DieInAir(true);
        }

		public void OnDestroy()
		{
            ParticleBase.EmitParticles("HexParticleFull", 1, new ParticleSystem.EmitParams()
            {
                position = this.transform.position,
                startSize = 1.25f,
                rotation = 0,
                startLifetime = 0.125f,
                startColor = new Color(1, 0.4f, 0).WithAlpha(2f),
            });

            if (Owner == null)return;
			if (GunWithNoName.StoredGWNNs[Owner].Contains(this))
			{
				GunWithNoName.StoredGWNNs[Owner].Remove(this);
            }
        }

    }

	public class GunWithNoName : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Gun With No Name", "gunwithnoname");
			Game.Items.Rename("outdated_gun_mods:gun_with_no_name", "psog:gun_with_no_name");
			gun.gameObject.AddComponent<GunWithNoName>();
			gun.SetShortDescription("...");
            gun.SetLongDescription("There is no hiding from the Gun With No Name.\n\nA silence has befallen the Proper. Gundead, by the dozens, line the halls and corridors, ready. The Gungeon will remain theirs to rule.\n\nBut there is no hiding from the Gun With No Name." +
                "\n\nA lone gunslinger, revolvers for ribs and break-action for an arm, enters the room. The Gundead ready.\n\nBut there is no hiding from the Gun With No Name." +
                "\n\nGundead on the ground fall to the ground. Gundead up high fall to the ground. Gundead in places the lone gunslinger never even looked towards fall to the ground. Just as soon as gunfights started, they would finish. Soon, interrupted only by lone footsteps, was silence.\n\nFor there is no hiding from the Gun With No Name.");


            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "gwnn_ammonomicon");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.reloadAnimation = "gwnn_reload";
            gun.idleAnimation = "gwnn_idle";
            gun.shootAnimation = "gwnn_fire";

            EnemyToolbox.AddSoundsToAnimationFrame(gun.spriteAnimator, "gwnn_reload", new Dictionary<int, string>() 
            {
                {2 , "Play_wpn_chamberabbey_reload_01" },
                {34 , "Play_TRP_spikes_shoot_02" }
            });
            EnemyToolbox.AddSoundsToAnimationFrame(gun.spriteAnimator, "gwnn_fire", new Dictionary<int, string>()
            {
                {0 , "Play_OBJ_mine_beep_01" },
            });
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(35) as Gun, true, false);

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[0].projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 0f;
            projectile.baseData.speed = 0f;
			projectile.ManualControl = true;

            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.08f;
            yes.shadowTimeDelay = 0.02f;
            yes.dashColor = new Color(1f, 0.4f, 0f, 0.333f);
            yes.minTranslation = -1;
            yes.name = "Gun Trail";

            int amount = 8;


            Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(projectile, "GWNN_charge", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "GWNN_charge",
            AnimateBullet.ConstructListOfSameValues<IntVector2>(new IntVector2(15, 14), amount),
			AnimateBullet.ConstructListOfSameValues(false, amount),
			AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, amount),
			AnimateBullet.ConstructListOfSameValues(true, amount),
			AnimateBullet.ConstructListOfSameValues(false, amount),
			AnimateBullet.ConstructListOfSameValues<Vector3?>(new Vector2(0, 0), amount),
			AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, amount),
			AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, amount),
			AnimateBullet.ConstructListOfSameValues<Projectile>(null, amount));
            projectile.shouldRotate = false;
			projectile.collidesWithEnemies = false;
            projectile.baseData.range = 420;

            projectile.sprite.usesOverrideMaterial = true;
            Material mater = projectile.sprite.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mater.mainTexture = projectile.sprite.renderer.material.mainTexture;
            mater.SetColor("_EmissiveColor", new Color32(255, 197, 111, 255));
            mater.SetFloat("_EmissiveColorPower", 5f);
            mater.SetFloat("_EmissivePower", 30);
            projectile.sprite.renderer.material = mater;


            var gwnn = projectile.gameObject.AddComponent<GWNNSpawnedProjectile>();
            gwnn.Projectile = projectile;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.cooldownTime = 0.1f;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.numberOfShotsInClip = 36;
            gun.DefaultModule.projectiles[0] = projectile;

            gun.gunClass = GunClass.PISTOL;
			gun.barrelOffset.transform.localPosition = new Vector3(0.5f, 0.25f, 0f);
			gun.reloadTime = 4f;
			gun.SetBaseMaxAmmo(666);
			gun.quality = PickupObject.ItemQuality.A;
            gun.gunHandedness = GunHandedness.HiddenOneHanded;
            gun.carryPixelOffset = new IntVector2(-8, 8);

            ETGMod.Databases.Items.Add(gun, false, "ANY");


            var _Hitscan = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(35) as Gun).DefaultModule.projectiles[0]);

            _Hitscan.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(_Hitscan.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(_Hitscan);

            //_Hitscan.AddComponent<PierceDeadActors>();
            Alexandria.Assetbundle.ProjectileBuilders.SetProjectileCollisionRight(_Hitscan, 
            "lilpew_projectile", StaticSpriteDefinitions.Projectile_Sheet_Data, 4, 4, false, tk2dBaseSprite.Anchor.MiddleCenter, 4, 4);



            _Hitscan.baseData.range = 10000;
            var hit = _Hitscan.gameObject.AddComponent<GWNN_Hitscan>();
            _Hitscan.sprite.renderer.enabled = false;
            hit.baseData = new ProjectileData();
            hit.CopyFrom<Projectile>(_Hitscan);
            hit.baseData.CopyFrom<ProjectileData>(_Hitscan.baseData);
            hit.baseData.damage = 5.75f;
            hit.baseData.speed = 0.01f;
            Destroy(_Hitscan);
            Hitscan = hit;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(3) as Gun).gunSwitchGroup;
            gun.PreventNormalFireAudio = true;
            gun.OverrideNormalFireAudioEvent = "";
            gun.muzzleFlashEffects = new VFXPool() { effects = new VFXComplex[0], type = VFXPoolType.None };
            gun.DefaultModule.ammoType = (PickupObjectDatabase.GetById(121) as Gun).DefaultModule.ammoType;
            gun.DefaultModule.customAmmoType = (PickupObjectDatabase.GetById(121) as Gun).DefaultModule.customAmmoType;


            ItemID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
            

            Actions.OnGameManagerUpdate += (_) =>
			{
				if (StoredGWNNs.Count == 0) {return; }
				foreach (var entry in StoredGWNNs)
				{
					int count = entry.Value.Count;

                    for (int i = 0; i < count; i++)
					{      
                        float M = 1f + ((int)(i / 12) * 0.75f);
                        var proj = entry.Value[i];
						proj.transform.position = entry.Key.sprite.WorldCenter.ToVector3ZUp(1) + (PositionsToTake[i % 12] * M);
                        proj.Projectile.specRigidbody.Reinitialize();
                    }
				}
			};

        }
		public static Projectile Hitscan;
		public static int ItemID;
        public static Dictionary<PlayerController, List<GWNNSpawnedProjectile>> StoredGWNNs = new Dictionary<PlayerController, List<GWNNSpawnedProjectile>>();
        public static Vector3[] PositionsToTake = new Vector3[12]
		{
            MathToolbox.GetUnitOnCircle3(60, 1.3125f),
            MathToolbox.GetUnitOnCircle3(120, 1.3125f),
            MathToolbox.GetUnitOnCircle3(180, 1.3125f),
            MathToolbox.GetUnitOnCircle3(240, 1.3125f),
            MathToolbox.GetUnitOnCircle3(300, 1.3125f),
            MathToolbox.GetUnitOnCircle3(360, 1.3125f),
            MathToolbox.GetUnitOnCircle3(90, 1.4375f),
            MathToolbox.GetUnitOnCircle3(150, 1.4375f),
            MathToolbox.GetUnitOnCircle3(210, 1.4375f),
            MathToolbox.GetUnitOnCircle3(270, 1.4375f),
            MathToolbox.GetUnitOnCircle3(330, 1.4375f),
            MathToolbox.GetUnitOnCircle3(30, 1.4375f),
        };
	}
}