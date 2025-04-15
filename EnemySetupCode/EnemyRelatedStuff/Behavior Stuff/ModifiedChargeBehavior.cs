using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planetside.EnemySetupCode.EnemyRelatedStuff.Behavior_Stuff
{
    using System;
    using System.Collections.Generic;
    using Dungeonator;
    using FullInspector;
    using UnityEngine;

    // Token: 0x02000D15 RID: 3349
    public class ModifiedChargeBehavior : BasicAttackBehavior
    {
        // Token: 0x060046A7 RID: 18087 RVA: 0x0016F7CC File Offset: 0x0016D9CC
        public override void Start()
        {
            base.Start();
            this.m_cachedKnockback = this.m_aiActor.CollisionKnockbackStrength;
            this.m_cachedDamage = this.m_aiActor.CollisionDamage;
            this.m_cachedVfx = this.m_aiActor.CollisionVFX;
            this.m_cachedNonActorWallVfx = this.m_aiActor.NonActorCollisionVFX;
            this.m_cachedPathableTiles = this.m_aiActor.PathableTiles;
            this.m_cachedDoDustUps = this.m_aiActor.DoDustUps;
            this.m_cachedDustUpInterval = this.m_aiActor.DustUpInterval;
            if (this.switchCollidersOnCharge)
            {
                for (int i = 0; i < this.m_aiActor.specRigidbody.PixelColliders.Count; i++)
                {
                    PixelCollider pixelCollider = this.m_aiActor.specRigidbody.PixelColliders[i];
                    if (pixelCollider.CollisionLayer == CollisionLayer.EnemyCollider)
                    {
                        this.m_enemyCollider = pixelCollider;
                    }
                    if (pixelCollider.CollisionLayer == CollisionLayer.EnemyHitBox)
                    {
                        this.m_enemyHitbox = pixelCollider;
                    }
                    if (!pixelCollider.Enabled && pixelCollider.CollisionLayer == CollisionLayer.Projectile)
                    {
                        this.m_projectileCollider = pixelCollider;
                        this.m_projectileCollider.CollisionLayerCollidableOverride |= CollisionMask.LayerToMask(CollisionLayer.Projectile);
                    }
                }
            }
            if (!this.collidesWithDodgeRollingPlayers)
            {
                SpeculativeRigidbody specRigidbody = this.m_aiActor.specRigidbody;
                specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision));
            }
        }

        // Token: 0x060046A8 RID: 18088 RVA: 0x0016F934 File Offset: 0x0016DB34
        public override void Upkeep()
        {
            base.Upkeep();
            base.DecrementTimer(ref this.m_timer, false);
        }

        // Token: 0x060046A9 RID: 18089 RVA: 0x0016F94C File Offset: 0x0016DB4C
        public override BehaviorResult Update()
        {
            base.Update();
            if (!this.m_initialized)
            {
                SpeculativeRigidbody specRigidbody = this.m_aiActor.specRigidbody;
                specRigidbody.OnCollision = (Action<CollisionData>)Delegate.Combine(specRigidbody.OnCollision, new Action<CollisionData>(this.OnCollision));
                specRigidbody.OnTileCollision += OnTileCollision;

                this.m_initialized = true;
            }
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
            {
                return behaviorResult;
            }
            if (!this.IsReady())
            {
                return BehaviorResult.Continue;
            }
            if (!this.m_aiActor.TargetRigidbody)
            {
                return BehaviorResult.Continue;
            }
            Vector2 vector = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
            if (this.leadAmount > 0f)
            {
                Vector2 b = vector + this.m_aiActor.TargetRigidbody.specRigidbody.Velocity * 0.75f;
                b = BraveMathCollege.GetPredictedPosition(vector, this.m_aiActor.TargetVelocity, this.m_aiActor.specRigidbody.UnitCenter, this.chargeSpeed);
                vector = Vector2.Lerp(vector, b, this.leadAmount);
            }
            float num = Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, vector);
            if (num > this.minRange)
            {
                if (!string.IsNullOrEmpty(this.primeAnim) || this.primeTime > 0f)
                {
                    this.State = ChargeBehavior.FireState.Priming;
                }
                else
                {
                    this.State = ChargeBehavior.FireState.Charging;
                }
                this.m_updateEveryFrame = true;
                return BehaviorResult.RunContinuous;
            }
            return BehaviorResult.Continue;
        }

        // Token: 0x060046AA RID: 18090 RVA: 0x0016FAB4 File Offset: 0x0016DCB4
        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            if (this.State == ChargeBehavior.FireState.Priming)
            {
                if (!this.m_aiActor.TargetRigidbody)
                {
                    return ContinuousBehaviorResult.Finished;
                }
                if (this.m_timer > 0f)
                {
                    float facingDirection = this.m_aiAnimator.FacingDirection;
                    float num = (this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.specRigidbody.UnitCenter).ToAngle();
                    float b = BraveMathCollege.ClampAngle180(num - facingDirection);
                    float facingDirection2 = facingDirection + Mathf.Lerp(0f, b, this.m_deltaTime / (this.m_timer + this.m_deltaTime));
                    this.m_aiAnimator.FacingDirection = facingDirection2;
                }
                if (!this.stopDuringPrime)
                {
                    float magnitude = this.m_aiActor.BehaviorVelocity.magnitude;
                    float magnitude2 = Mathf.Lerp(magnitude, 0f, this.m_deltaTime / (this.m_timer + this.m_deltaTime));
                    this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_aiAnimator.FacingDirection, magnitude2);
                }
                if ((this.primeTime <= 0f) ? (!this.m_aiAnimator.IsPlaying(this.primeAnim)) : (this.m_timer <= 0f))
                {
                    this.State = ChargeBehavior.FireState.Charging;
                }
            }
            else if (this.State == ChargeBehavior.FireState.Charging)
            {
                if (this.chargeAcceleration > 0f)
                {
                    this.m_currentSpeed = Mathf.Min(this.chargeSpeed, this.m_currentSpeed + this.chargeAcceleration * this.m_deltaTime);
                    this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_chargeDirection, this.m_currentSpeed);
                }
                if (this.endWhenChargeAnimFinishes && !this.m_aiAnimator.IsPlaying(this.chargeAnim))
                {
                    return ContinuousBehaviorResult.Finished;
                }
                if (this.maxChargeDistance > 0f)
                {
                    this.m_chargeTime += this.m_deltaTime;
                    if (this.m_chargeTime * this.chargeSpeed > this.maxChargeDistance)
                    {
                        return ContinuousBehaviorResult.Finished;
                    }
                }
            }
            else if (this.State == ChargeBehavior.FireState.Bouncing)
            {
                if (!this.m_aiAnimator.IsPlaying(this.hitAnim))
                {
                    return ContinuousBehaviorResult.Finished;
                }
            }
            else if (this.State == ChargeBehavior.FireState.Idle)
            {
                return ContinuousBehaviorResult.Finished;
            }
            return ContinuousBehaviorResult.Continue;
        }

        // Token: 0x060046AB RID: 18091 RVA: 0x0016FD08 File Offset: 0x0016DF08
        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_updateEveryFrame = false;
            this.State = ChargeBehavior.FireState.Idle;
            this.UpdateCooldowns();
        }

        // Token: 0x060046AC RID: 18092 RVA: 0x0016FD24 File Offset: 0x0016DF24
        public override void Destroy()
        {
            if (this.m_aiActor)
            {
                SpeculativeRigidbody specRigidbody = this.m_aiActor.specRigidbody;
                specRigidbody.OnPostRigidbodyMovement = (Action<SpeculativeRigidbody, Vector2, IntVector2>)Delegate.Remove(specRigidbody.OnPostRigidbodyMovement, new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.OnPostRigidbodyMovement));
            }
            base.Destroy();
        }

        // Token: 0x060046AD RID: 18093 RVA: 0x0016FD74 File Offset: 0x0016DF74
        private void Fire()
        {
            if (!this.m_bulletSource)
            {
                this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
            }
            this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
            this.m_bulletSource.BulletScript = this.bulletScript;
            this.m_bulletSource.Initialize();
        }

        // Token: 0x060046AE RID: 18094 RVA: 0x0016FDD4 File Offset: 0x0016DFD4
        private void OnPreRigidbodyCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (this.m_state == ChargeBehavior.FireState.Charging)
            {
                PlayerController playerController = otherRigidbody.gameActor as PlayerController;
                if (playerController && playerController.spriteAnimator.QueryInvulnerabilityFrame())
                {
                    PhysicsEngine.SkipCollision = true;
                }
            }
        }

        // Token: 0x060046AF RID: 18095 RVA: 0x0016FE1C File Offset: 0x0016E01C
        private void OnCollision(CollisionData collisionData)
        {
            if (this.State == ChargeBehavior.FireState.Charging && !this.m_aiActor.healthHaver.IsDead)
            {
                if (collisionData.OtherRigidbody)
                {
                    Projectile projectile = collisionData.OtherRigidbody.projectile;
                    if (projectile)
                    {
                        if (!(projectile.Owner is PlayerController))
                        {
                            return;
                        }
                        if (!this.stoppedByProjectiles)
                        {
                            return;
                        }
                    }
                    
                }


                if (!string.IsNullOrEmpty(this.hitAnim))
                {
                    this.State = ChargeBehavior.FireState.Bouncing;
                }
                else
                {
                    this.State = ChargeBehavior.FireState.Idle;
                }
                if (this.switchCollidersOnCharge)
                {
                    PhysicsEngine.CollisionHaltsVelocity = new bool?(true);
                    PhysicsEngine.HaltRemainingMovement = true;
                    PhysicsEngine.PostSliceVelocity = new Vector2?(Vector2.zero);
                    this.m_collisionNormal = collisionData.Normal;
                    SpeculativeRigidbody specRigidbody = this.m_aiActor.specRigidbody;
                    specRigidbody.OnPostRigidbodyMovement = (Action<SpeculativeRigidbody, Vector2, IntVector2>)Delegate.Combine(specRigidbody.OnPostRigidbodyMovement, new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.OnPostRigidbodyMovement));
                }
                if (!collisionData.OtherRigidbody || !collisionData.OtherRigidbody.knockbackDoer)
                {
                    this.m_aiActor.knockbackDoer.ApplyKnockback(collisionData.Normal, this.wallRecoilForce, false);
                }
            }
        }

        private void OnTileCollision(CollisionData collisionData)
        {
            if (endOnWallCollision == false) { return; }
            if (!string.IsNullOrEmpty(this.hitAnim))
            {
                this.State = ChargeBehavior.FireState.Bouncing;
            }
            else
            {
                this.State = ChargeBehavior.FireState.Idle;
            }
            if (this.switchCollidersOnCharge)
            {
                PhysicsEngine.CollisionHaltsVelocity = new bool?(true);
                PhysicsEngine.HaltRemainingMovement = true;
                PhysicsEngine.PostSliceVelocity = new Vector2?(Vector2.zero);
                this.m_collisionNormal = collisionData.Normal;
                SpeculativeRigidbody specRigidbody = this.m_aiActor.specRigidbody;
                specRigidbody.OnPostRigidbodyMovement = (Action<SpeculativeRigidbody, Vector2, IntVector2>)Delegate.Combine(specRigidbody.OnPostRigidbodyMovement, new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.OnPostRigidbodyMovement));
            }
            if (!collisionData.OtherRigidbody || !collisionData.OtherRigidbody.knockbackDoer)
            {
                this.m_aiActor.knockbackDoer.ApplyKnockback(collisionData.Normal, this.wallRecoilForce, false);
            }
        }

        // Token: 0x060046B0 RID: 18096 RVA: 0x0016FF58 File Offset: 0x0016E158
        private void OnPostRigidbodyMovement(SpeculativeRigidbody specRigidbody, Vector2 unitDelta, IntVector2 pixelDelta)
        {
            if (!this.m_behaviorSpeculator)
            {
                return;
            }
            List<CollisionData> list = new List<CollisionData>();
            bool flag = false;
            if (PhysicsEngine.Instance.OverlapCast(this.m_aiActor.specRigidbody, list, true, true, null, null, false, null, null, new SpeculativeRigidbody[0]))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    SpeculativeRigidbody otherRigidbody = list[i].OtherRigidbody;
                    if (otherRigidbody && otherRigidbody.transform.parent)
                    {
                        if (otherRigidbody.transform.parent.GetComponent<DungeonDoorSubsidiaryBlocker>() || otherRigidbody.transform.parent.GetComponent<DungeonDoorController>())
                        {
                            flag = true;
                            break;
                        }
                    }
                }
            }
            if (flag)
            {
                if (this.m_collisionNormal.y >= 0.5f)
                {
                    this.m_aiActor.transform.position += new Vector3(0f, 0.5f);
                }
                if (this.m_collisionNormal.x <= -0.5f)
                {
                    this.m_aiActor.transform.position += new Vector3(-0.3125f, 0f);
                }
                if (this.m_collisionNormal.x >= 0.5f)
                {
                    this.m_aiActor.transform.position += new Vector3(0.3125f, 0f);
                }
                this.m_aiActor.specRigidbody.Reinitialize();
            }
            else
            {
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.m_aiActor.specRigidbody, null, false);
            }
            SpeculativeRigidbody specRigidbody2 = this.m_aiActor.specRigidbody;
            specRigidbody2.OnPostRigidbodyMovement = (Action<SpeculativeRigidbody, Vector2, IntVector2>)Delegate.Remove(specRigidbody2.OnPostRigidbodyMovement, new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.OnPostRigidbodyMovement));
        }

        // Token: 0x17000A58 RID: 2648
        // (get) Token: 0x060046B1 RID: 18097 RVA: 0x00170170 File Offset: 0x0016E370
        // (set) Token: 0x060046B2 RID: 18098 RVA: 0x00170178 File Offset: 0x0016E378
        private ChargeBehavior.FireState State
        {
            get
            {
                return this.m_state;
            }
            set
            {
                if (this.m_state != value)
                {
                    this.EndState(this.m_state);
                    this.m_state = value;
                    this.BeginState(this.m_state);
                }
            }
        }

        // Token: 0x060046B3 RID: 18099 RVA: 0x001701A8 File Offset: 0x0016E3A8
        private void BeginState(ChargeBehavior.FireState state)
        {
            if (state == ChargeBehavior.FireState.Idle)
            {
                if (this.HideGun)
                {
                    this.m_aiShooter.ToggleGunAndHandRenderers(true, "ChargeBehavior");
                }
                this.m_aiActor.BehaviorOverridesVelocity = false;
                this.m_aiAnimator.LockFacingDirection = false;
            }
            else if (state == ChargeBehavior.FireState.Priming)
            {
                if (this.HideGun)
                {
                    this.m_aiShooter.ToggleGunAndHandRenderers(false, "ChargeBehavior");
                }
                this.m_aiAnimator.PlayUntilFinished(this.primeAnim, true, null, -1f, false);
                if (this.primeTime > 0f)
                {
                    this.m_timer = this.primeTime;
                }
                else
                {
                    this.m_timer = this.m_aiAnimator.CurrentClipLength;
                }
                if (this.stopDuringPrime)
                {
                    this.m_aiActor.ClearPath();
                    this.m_aiActor.BehaviorOverridesVelocity = true;
                    this.m_aiActor.BehaviorVelocity = Vector2.zero;
                }
                else
                {
                    this.m_aiActor.BehaviorOverridesVelocity = true;
                    this.m_aiActor.BehaviorVelocity = this.m_aiActor.specRigidbody.Velocity;
                }
            }
            else if (state == ChargeBehavior.FireState.Charging)
            {
                if (this.HideGun)
                {
                    this.m_aiShooter.ToggleGunAndHandRenderers(false, "ChargeBehavior");
                }
                this.m_chargeTime = 0f;
                Vector2 vector = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
                if (this.leadAmount > 0f)
                {
                    Vector2 b = vector + this.m_aiActor.TargetRigidbody.specRigidbody.Velocity * 0.75f;
                    b = BraveMathCollege.GetPredictedPosition(vector, this.m_aiActor.TargetVelocity, this.m_aiActor.specRigidbody.UnitCenter, this.chargeSpeed);
                    vector = Vector2.Lerp(vector, b, this.leadAmount);
                }
                this.m_aiActor.ClearPath();
                this.m_aiActor.BehaviorOverridesVelocity = true;
                this.m_currentSpeed = ((this.chargeAcceleration <= 0f) ? this.chargeSpeed : 0f);
                this.m_chargeDirection = (vector - this.m_aiActor.specRigidbody.UnitCenter).ToAngle();
                this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_chargeDirection, this.m_currentSpeed);
                this.m_aiAnimator.LockFacingDirection = true;
                this.m_aiAnimator.FacingDirection = this.m_chargeDirection;
                this.m_aiActor.CollisionKnockbackStrength = this.chargeKnockback;
                this.m_aiActor.CollisionDamage = this.chargeDamage;
                if (this.hitVfx)
                {
                    VFXObject vfxobject = new VFXObject();
                    vfxobject.effect = this.hitVfx;
                    VFXComplex vfxcomplex = new VFXComplex();
                    vfxcomplex.effects = new VFXObject[]
                    {
                    vfxobject
                    };
                    VFXPool vfxpool = new VFXPool();
                    vfxpool.type = VFXPoolType.Single;
                    vfxpool.effects = new VFXComplex[]
                    {
                    vfxcomplex
                    };
                    this.m_aiActor.CollisionVFX = vfxpool;
                }
                if (this.nonActorHitVfx)
                {
                    VFXObject vfxobject2 = new VFXObject();
                    vfxobject2.effect = this.nonActorHitVfx;
                    VFXComplex vfxcomplex2 = new VFXComplex();
                    vfxcomplex2.effects = new VFXObject[]
                    {
                    vfxobject2
                    };
                    VFXPool vfxpool2 = new VFXPool();
                    vfxpool2.type = VFXPoolType.Single;
                    vfxpool2.effects = new VFXComplex[]
                    {
                    vfxcomplex2
                    };
                    this.m_aiActor.NonActorCollisionVFX = vfxpool2;
                }
                this.m_aiActor.PathableTiles = (CellTypes.FLOOR | CellTypes.PIT);
                if (this.switchCollidersOnCharge)
                {
                    this.m_enemyCollider.CollisionLayer = CollisionLayer.TileBlocker;
                    this.m_enemyHitbox.Enabled = false;
                    this.m_projectileCollider.Enabled = true;
                }
                this.m_aiActor.DoDustUps = this.chargeDustUps;
                this.m_aiActor.DustUpInterval = this.chargeDustUpInterval;
                this.m_aiAnimator.PlayUntilFinished(this.chargeAnim, true, null, -1f, false);
                if (this.launchVfx)
                {
                    SpawnManager.SpawnVFX(this.launchVfx, this.m_aiActor.specRigidbody.UnitCenter, Quaternion.identity);
                }
                if (this.trailVfx)
                {
                    this.m_trailVfx = SpawnManager.SpawnParticleSystem(this.trailVfx, this.m_aiActor.sprite.WorldCenter, Quaternion.Euler(0f, 0f, this.m_chargeDirection));
                    if (this.trailVfxParent)
                    {
                        this.m_trailVfx.transform.parent = this.trailVfxParent;
                    }
                    else
                    {
                        this.m_trailVfx.transform.parent = this.m_aiActor.transform;
                    }
                    ParticleKiller component = this.m_trailVfx.GetComponent<ParticleKiller>();
                    if (component != null)
                    {
                        component.Awake();
                    }
                }
                if (this.bulletScript != null && !this.bulletScript.IsNull)
                {
                    this.Fire();
                }
                this.m_aiActor.specRigidbody.ForceRegenerate(null, null);
            }
            else if (state == ChargeBehavior.FireState.Bouncing)
            {
                this.m_aiAnimator.PlayUntilFinished(this.hitAnim, true, null, -1f, false);
            }
        }

        // Token: 0x060046B4 RID: 18100 RVA: 0x001706CC File Offset: 0x0016E8CC
        private void EndState(ChargeBehavior.FireState state)
        {
            if (state == ChargeBehavior.FireState.Charging)
            {
                this.m_aiActor.BehaviorVelocity = Vector2.zero;
                this.m_aiActor.CollisionKnockbackStrength = this.m_cachedKnockback;
                this.m_aiActor.CollisionDamage = this.m_cachedDamage;
                this.m_aiActor.CollisionVFX = this.m_cachedVfx;
                this.m_aiActor.NonActorCollisionVFX = this.m_cachedNonActorWallVfx;
                if (this.m_trailVfx)
                {
                    ParticleKiller component = this.m_trailVfx.GetComponent<ParticleKiller>();
                    if (component)
                    {
                        component.StopEmitting();
                    }
                    else
                    {
                        SpawnManager.Despawn(this.m_trailVfx);
                    }
                    this.m_trailVfx = null;
                }
                this.m_aiActor.DoDustUps = this.m_cachedDoDustUps;
                this.m_aiActor.DustUpInterval = this.m_cachedDustUpInterval;
                this.m_aiActor.PathableTiles = this.m_cachedPathableTiles;
                if (this.switchCollidersOnCharge)
                {
                    this.m_enemyCollider.CollisionLayer = CollisionLayer.EnemyCollider;
                    this.m_enemyHitbox.Enabled = true;
                    this.m_projectileCollider.Enabled = false;
                }
                if (this.m_bulletSource != null)
                {
                    this.m_bulletSource.ForceStop();
                }
                this.m_aiAnimator.EndAnimationIf(this.chargeAnim);
            }
        }

        // Token: 0x0400393C RID: 14652
        [InspectorCategory("Conditions")]
        public float minRange;

        // Token: 0x0400393D RID: 14653
        [InspectorHeader("Prime")]
        public float primeTime = -1f;

        // Token: 0x0400393E RID: 14654
        public bool stopDuringPrime = true;

        // Token: 0x0400393F RID: 14655
        [InspectorHeader("Charge")]
        public float leadAmount;

        // Token: 0x04003940 RID: 14656
        public float chargeSpeed;

        // Token: 0x04003941 RID: 14657
        public float chargeAcceleration = -1f;

        // Token: 0x04003942 RID: 14658
        public float maxChargeDistance = -1f;

        // Token: 0x04003943 RID: 14659
        public float chargeKnockback = 50f;

        // Token: 0x04003944 RID: 14660
        public float chargeDamage = 0.5f;

        // Token: 0x04003945 RID: 14661
        public float wallRecoilForce = 10f;

        // Token: 0x04003946 RID: 14662
        public bool stoppedByProjectiles = true;

        // Token: 0x04003947 RID: 14663
        public bool endWhenChargeAnimFinishes;

        // Token: 0x04003948 RID: 14664
        public bool switchCollidersOnCharge;

        // Token: 0x04003949 RID: 14665
        public bool collidesWithDodgeRollingPlayers = true;

        // Token: 0x0400394A RID: 14666
        [InspectorCategory("Attack")]
        public GameObject ShootPoint;

        // Token: 0x0400394B RID: 14667
        [InspectorCategory("Attack")]
        public BulletScriptSelector bulletScript;

        // Token: 0x0400394C RID: 14668
        [InspectorCategory("Visuals")]
        public string primeAnim;

        // Token: 0x0400394D RID: 14669
        [InspectorCategory("Visuals")]
        public string chargeAnim;

        // Token: 0x0400394E RID: 14670
        [InspectorCategory("Visuals")]
        public string hitAnim;

        // Token: 0x0400394F RID: 14671
        [InspectorCategory("Visuals")]
        public bool HideGun;

        // Token: 0x04003950 RID: 14672
        [InspectorCategory("Visuals")]
        public GameObject launchVfx;

        // Token: 0x04003951 RID: 14673
        [InspectorCategory("Visuals")]
        public GameObject trailVfx;

        // Token: 0x04003952 RID: 14674
        [InspectorCategory("Visuals")]
        public Transform trailVfxParent;

        // Token: 0x04003953 RID: 14675
        [InspectorCategory("Visuals")]
        public GameObject hitVfx;

        // Token: 0x04003954 RID: 14676
        [InspectorCategory("Visuals")]
        public GameObject nonActorHitVfx;

        // Token: 0x04003955 RID: 14677
        [InspectorCategory("Visuals")]
        public bool chargeDustUps;

        // Token: 0x04003956 RID: 14678
        [InspectorShowIf("chargeDustUps")]
        [InspectorCategory("Visuals")]
        [InspectorIndent]
        public float chargeDustUpInterval;

        // Token: 0x04003957 RID: 14679
        private BulletScriptSource m_bulletSource;

        // Token: 0x04003958 RID: 14680
        private bool m_initialized;

        // Token: 0x04003959 RID: 14681
        private float m_timer;

        // Token: 0x0400395A RID: 14682
        private float m_chargeTime;

        // Token: 0x0400395B RID: 14683
        private float m_cachedKnockback;

        // Token: 0x0400395C RID: 14684
        private float m_cachedDamage;

        // Token: 0x0400395D RID: 14685
        private VFXPool m_cachedVfx;

        // Token: 0x0400395E RID: 14686
        private VFXPool m_cachedNonActorWallVfx;

        // Token: 0x0400395F RID: 14687
        private float m_currentSpeed;

        // Token: 0x04003960 RID: 14688
        private float m_chargeDirection;

        // Token: 0x04003961 RID: 14689
        private CellTypes m_cachedPathableTiles;

        // Token: 0x04003962 RID: 14690
        private bool m_cachedDoDustUps;

        // Token: 0x04003963 RID: 14691
        private float m_cachedDustUpInterval;

        // Token: 0x04003964 RID: 14692
        private PixelCollider m_enemyCollider;

        // Token: 0x04003965 RID: 14693
        private PixelCollider m_enemyHitbox;

        // Token: 0x04003966 RID: 14694
        private PixelCollider m_projectileCollider;

        // Token: 0x04003967 RID: 14695
        private GameObject m_trailVfx;

        // Token: 0x04003968 RID: 14696
        private Vector2 m_collisionNormal;

        // Token: 0x04003969 RID: 14697
        private ChargeBehavior.FireState m_state;

        public bool endOnWallCollision = false;

        // Token: 0x02000D16 RID: 3350
        private enum FireState
        {
            // Token: 0x0400396B RID: 14699
            Idle,
            // Token: 0x0400396C RID: 14700
            Priming,
            // Token: 0x0400396D RID: 14701
            Charging,
            // Token: 0x0400396E RID: 14702
            Bouncing
        }
    }

}
