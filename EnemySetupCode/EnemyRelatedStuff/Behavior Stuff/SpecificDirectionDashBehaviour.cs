using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Brave.BulletScript;
using Dungeonator;
using FullInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Alexandria.Misc;
namespace Planetside
{
    public class SpecificDirectionDashBehaviour : BasicAttackBehavior
    {


        public override void Start()
        {
            base.Start();
            SpeculativeRigidbody specRigidbody = this.m_aiActor.specRigidbody;
            specRigidbody.OnCollision = (Action<CollisionData>)Delegate.Combine(specRigidbody.OnCollision, new Action<CollisionData>(this.OnCollision));
            this.m_cachedKnockback = this.m_aiActor.CollisionKnockbackStrength;
            this.m_cachedDamage = this.m_aiActor.CollisionDamage;
            this.m_cachedVfx = this.m_aiActor.CollisionVFX;
            this.m_cachedPathableTiles = this.m_aiActor.PathableTiles;
            this.m_cachedDoDustUps = this.m_aiActor.DoDustUps;
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
            if (this.stopAtPits)
            {
                SpeculativeRigidbody specRigidbody2 = this.m_aiActor.specRigidbody;
                specRigidbody2.MovementRestrictor = (SpeculativeRigidbody.MovementRestrictorDelegate)Delegate.Combine(specRigidbody2.MovementRestrictor, new SpeculativeRigidbody.MovementRestrictorDelegate(this.PitMovementRestrictor));
            }
            if (!string.IsNullOrEmpty(this.primeAnim))
            {
                //this.m_primeAnimTime = this.m_aiAnimator.GetDirectionalAnimationLength(this.primeAnim);
            }
            this.m_aiActor.OverrideHitEnemies = true;
            this.m_aiActor.StartCoroutine(Wait());
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(0.05f);
            DelayPlaceDirt();
            yield break;
        }


        public void DelayPlaceDirt()
        {
            if (this.m_aiActor.State != AIActor.ActorState.Normal) { return; }
            var centerpoint = this.m_aiActor.sprite.WorldCenter - new Vector2(0, 0.3125f);
            UnityEngine.Object.Instantiate(Alexandria.DungeonAPI.StaticReferences.customObjects["psog_dirt_m"], centerpoint, Quaternion.identity);

            if (CanChargeUp)
            {
                DoDirtPlacement(new Vector2(0, 1), true);
            }
            if (CanChargeDown)
            {
                DoDirtPlacement(new Vector2(0, -1), true);
            }
            if (CanChargeLeft)
            {
                DoDirtPlacement(new Vector2(-1, 0), false);
            }
            if (CanChargeRight)
            {
                DoDirtPlacement(new Vector2(1, 0), false);
            }
        }

        private void DoDirtPlacement(Vector2 Direction, bool isUp = false)
        {
            Vector2 centerpoint = this.m_aiActor.sprite.WorldCenter - new Vector2(0, 0.3125f);
            int complexEnemyVisibilityMask = CollisionMask.GetComplexEnemyVisibilityMask(this.m_aiActor.CanTargetPlayers, this.m_aiActor.CanTargetEnemies);

            RaycastResult raycastResult2;
            PhysicsEngine.Instance.Raycast(centerpoint + (Direction * 1.5f), Direction, 100, out raycastResult2, true, true, complexEnemyVisibilityMask, null, false, 
            (_) => 
            {
                return true;
            }, 
            this.m_aiActor.specRigidbody);
            if (raycastResult2 != null)
            {

                var dist = raycastResult2.Distance;
                var end = centerpoint + (Direction * dist);
                float amounts = Mathf.Min(24, Vector2.Distance(centerpoint, end));
                for (float i = 0; i < amounts; i++)
                {
                    float t = i / amounts;
                    var place = Vector3.Lerp(centerpoint, end, t);
                    if (CheckCellIsPit(place) == true) { break;}

                    if (UnityEngine.Random.value > (amounts * 0.025f))
                    {

                        UnityEngine.Object.Instantiate(Alexandria.DungeonAPI.StaticReferences.customObjects[isUp ? "psog_dirt_updown" : "psog_dirt_leftright"], place, Quaternion.identity);
                    }
                }
            }
            RaycastResult.Pool.Free(ref raycastResult2);
        }

        public bool CheckCellIsPit(Vector3 position)
        {
            IntVector2 key = position.IntXY(VectorConversions.Floor);
            if (GameManager.Instance.Dungeon == null) { return true; }
            CellData cellData = GameManager.Instance.Dungeon.data[key];
            if (cellData == null) { return true; }
            return cellData.type == CellType.PIT;
        }


        public override void Upkeep()
        {
            base.Upkeep();
        }

        public override BehaviorResult Update()
        {
            base.Update();
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
            {
                return behaviorResult;
            }
            if (!this.IsReady())
            {
                return BehaviorResult.Continue;
            }
            for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
            {
                PlayerController playerController = GameManager.Instance.AllPlayers[i];
                if (playerController && !playerController.healthHaver.IsDead && !playerController.IsFalling)
                {
                    CurrentHeadedDirection = DungeonData.Direction.NORTH;
                    if (this.ShouldChargePlayer(GameManager.Instance.AllPlayers[i], ref CurrentHeadedDirection))
                    {
                        SetFireState(SpecificDirectionDashBehaviour.FireState.Priming);
                        this.m_updateEveryFrame = true;
                        return BehaviorResult.RunContinuous;
                    }
                }
            }
            return BehaviorResult.Continue;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            if (this.State == SpecificDirectionDashBehaviour.FireState.Priming)
            {
                if (!this.m_aiAnimator.IsPlaying(this.primeAnim))
                {
                    if (!this.m_aiActor.TargetRigidbody)
                    {
                        return ContinuousBehaviorResult.Finished;
                    }
                    SetFireState(SpecificDirectionDashBehaviour.FireState.Charging);

                }
            }
            else if (this.State == SpecificDirectionDashBehaviour.FireState.Charging)
            {
                if (this.endWhenChargeAnimFinishes && !this.m_aiAnimator.IsPlaying(this.chargeAnim))
                {
                    return ContinuousBehaviorResult.Finished;
                }
            }
            else if (this.State == SpecificDirectionDashBehaviour.FireState.Bouncing && !this.m_aiAnimator.IsPlaying(this.hitAnim) && !this.m_aiAnimator.IsPlaying(this.hitPlayerAnim))
            {
                if (this.delayWallRecoil && this.m_storedCollisionNormal != null)
                {
                    this.m_aiActor.knockbackDoer.ApplyKnockback(this.m_storedCollisionNormal.Value, this.wallRecoilForce, false);
                    this.m_storedCollisionNormal = null;
                }
                return ContinuousBehaviorResult.Finished;
            }
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_updateEveryFrame = false;
            SetFireState(SpecificDirectionDashBehaviour.FireState.Idle);

            this.UpdateCooldowns();
        }

        public override void Destroy()
        {
            base.Destroy();
            if (this.stopAtPits)
            {
                SpeculativeRigidbody specRigidbody = this.m_aiActor.specRigidbody;
                specRigidbody.MovementRestrictor = (SpeculativeRigidbody.MovementRestrictorDelegate)Delegate.Remove(specRigidbody.MovementRestrictor, new SpeculativeRigidbody.MovementRestrictorDelegate(this.PitMovementRestrictor));
            }
        }

        private void OnCollision(CollisionData collisionData)
        {
            if (this.State == SpecificDirectionDashBehaviour.FireState.Charging)
            {
                if (collisionData.OtherRigidbody)
                {
                    SpeculativeRigidbody otherRigidbody = collisionData.OtherRigidbody;
                    if (otherRigidbody.projectile)
                    {
                        return;
                    }
                    if (otherRigidbody.aiActor)
                    {
                        if (!otherRigidbody.aiActor.OverrideHitEnemies)
                        {
                            collisionData.OtherRigidbody.RegisterTemporaryCollisionException(collisionData.MyRigidbody, 0.1f, null);
                            collisionData.MyRigidbody.RegisterTemporaryCollisionException(collisionData.OtherRigidbody, 0.1f, null);
                            return;
                        }
                        float num = collisionData.MyRigidbody.Velocity.ToAngle();
                        float num2 = collisionData.Normal.ToAngle();
                        if (Mathf.Abs(BraveMathCollege.ClampAngle180(num - num2)) <= 91f)
                        {
                            return;
                        }
                        float magnitude = collisionData.MyRigidbody.Velocity.magnitude;
                        float magnitude2 = otherRigidbody.Velocity.magnitude;
                        float num3 = otherRigidbody.Velocity.ToAngle();
                        if (Mathf.Abs(BraveMathCollege.ClampAngle180(num - num3)) < 45f && magnitude < magnitude2 * 1.25f)
                        {
                            return;
                        }
                    }
                }
                this.m_hitPlayer = (collisionData.OtherRigidbody && collisionData.OtherRigidbody.GetComponent<PlayerController>());
                this.m_hitWall = (collisionData.collisionType == CollisionData.CollisionType.TileMap);

                SetFireState(SpecificDirectionDashBehaviour.FireState.Bouncing);
                if (!collisionData.OtherRigidbody || !collisionData.OtherRigidbody.knockbackDoer)
                {
                    if (this.delayWallRecoil)
                    {
                        this.m_storedCollisionNormal = new Vector2?(collisionData.Normal);
                        if (collisionData.Normal == Vector2.zero)
                        {
                            Vector2? chargeDir = this.m_chargeDir;
                            this.m_storedCollisionNormal = ((chargeDir == null) ? null : new Vector2?(-chargeDir.Value));
                        }
                    }
                    else
                    {
                        this.m_storedCollisionNormal = null;
                        this.m_aiActor.knockbackDoer.ApplyKnockback(collisionData.Normal, this.wallRecoilForce, false);
                    }
                }
                else
                {
                    this.m_storedCollisionNormal = null;
                }
                if (!collisionData.OtherRigidbody && !string.IsNullOrEmpty(this.hitWallVfxString))
                {
                    string arg;
                    if (this.m_storedCollisionNormal.Value.x < -0.75f)
                    {
                        arg = "right";
                    }
                    else if (this.m_storedCollisionNormal.Value.x > 0.75f)
                    {
                        arg = "left";
                    }
                    else if (this.m_storedCollisionNormal.Value.y < -0.75f)
                    {
                        arg = "up";
                    }
                    else
                    {
                        arg = "down";
                    }
                    this.m_aiAnimator.PlayVfx(string.Format(this.hitWallVfxString, arg), null, null, null);
                }
            }
        }

        private void PitMovementRestrictor(SpeculativeRigidbody specRigidbody, IntVector2 prevPixelOffset, IntVector2 pixelOffset, ref bool validLocation)
        {
            if (!validLocation)
            {
                return;
            }
            Func<IntVector2, bool> func = delegate (IntVector2 pixel)
            {
                Vector2 v = PhysicsEngine.PixelToUnitMidpoint(pixel);
                if (!GameManager.Instance.Dungeon.CellSupportsFalling(v))
                {
                    return false;
                }
                List<SpeculativeRigidbody> platformsAt = GameManager.Instance.Dungeon.GetPlatformsAt(v);
                if (platformsAt != null)
                {
                    for (int i = 0; i < platformsAt.Count; i++)
                    {
                        if (platformsAt[i].PrimaryPixelCollider.ContainsPixel(pixel))
                        {
                            return false;
                        }
                    }
                }
                return true;
            };
            PixelCollider primaryPixelCollider = specRigidbody.PrimaryPixelCollider;
            if (primaryPixelCollider != null)
            {
                IntVector2 a = pixelOffset - prevPixelOffset;
                if (a == IntVector2.Down && func(primaryPixelCollider.LowerLeft + pixelOffset) && func(primaryPixelCollider.LowerRight + pixelOffset) && (!func(primaryPixelCollider.UpperRight + prevPixelOffset) || !func(primaryPixelCollider.UpperLeft + prevPixelOffset)))
                {
                    validLocation = false;
                    return;
                }
                if (a == IntVector2.Right && func(primaryPixelCollider.LowerRight + pixelOffset) && func(primaryPixelCollider.UpperRight + pixelOffset) && (!func(primaryPixelCollider.UpperLeft + prevPixelOffset) || !func(primaryPixelCollider.LowerLeft + prevPixelOffset)))
                {
                    validLocation = false;
                    return;
                }
                if (a == IntVector2.Up && func(primaryPixelCollider.UpperRight + pixelOffset) && func(primaryPixelCollider.UpperLeft + pixelOffset) && (!func(primaryPixelCollider.LowerLeft + prevPixelOffset) || !func(primaryPixelCollider.LowerRight + prevPixelOffset)))
                {
                    validLocation = false;
                    return;
                }
                if (a == IntVector2.Left && func(primaryPixelCollider.UpperLeft + pixelOffset) && func(primaryPixelCollider.LowerLeft + pixelOffset) && (!func(primaryPixelCollider.LowerRight + prevPixelOffset) || !func(primaryPixelCollider.UpperRight + prevPixelOffset)))
                {
                    validLocation = false;
                    return;
                }
            }
        }

        private SpecificDirectionDashBehaviour.FireState State
        {
            get
            {
                return this.m_state;
            }
        }

        private void SetFireState(SpecificDirectionDashBehaviour.FireState state)
        {
            if (this.m_state != state)
            {
                this.EndState(this.m_state);
                this.m_state = state;
                this.BeginState(this.m_state);
            }
        }


        private void BeginState(SpecificDirectionDashBehaviour.FireState state)
        {
            if (state == SpecificDirectionDashBehaviour.FireState.Idle)
            {
                this.m_aiActor.BehaviorOverridesVelocity = false;
                this.m_aiAnimator.LockFacingDirection = false;
            }
            else if (state == SpecificDirectionDashBehaviour.FireState.Priming)
            {
                this.m_aiAnimator.PlayUntilFinished(this.primeAnim, true, null, -1f, false);
                this.m_aiActor.ClearPath();
                this.m_aiActor.BehaviorOverridesVelocity = true;
                this.m_aiActor.BehaviorVelocity = Vector2.zero;
            }
            else if (state == SpecificDirectionDashBehaviour.FireState.Charging)
            {
                AkSoundEngine.PostEvent("Play_ENM_cube_dash_01", GameManager.Instance.PrimaryPlayer.gameObject);

                if (this.dashBulletScript != null && !this.dashBulletScript.IsNull)
                {
                    if (!this.m_bulletSource)
                    {
                        this.m_bulletSource = this.shootPoint.GetOrAddComponent<BulletScriptSource>();
                    }
                    this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
                    this.m_bulletSource.BulletScript = this.dashBulletScript;
                    this.m_bulletSource.Initialize();
                }


                this.m_aiActor.ClearPath();
                this.m_aiActor.BehaviorOverridesVelocity = true;
                this.m_aiActor.BehaviorVelocity = this.m_chargeDir.Value.normalized * this.chargeSpeed;
                float num = this.m_aiActor.BehaviorVelocity.ToAngle();
                this.m_aiAnimator.LockFacingDirection = true;
                this.m_aiAnimator.FacingDirection = num;
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
                this.m_aiActor.PathableTiles = (CellTypes.FLOOR | CellTypes.PIT);
                if (this.switchCollidersOnCharge)
                {
                    this.m_enemyCollider.CollisionLayer = CollisionLayer.TileBlocker;
                    this.m_enemyHitbox.Enabled = false;
                    this.m_projectileCollider.Enabled = true;
                }
                this.m_aiActor.DoDustUps = false;
                this.m_aiAnimator.PlayUntilFinished(this.chargeAnim, true, null, -1f, false);
                if (this.launchVfx)
                {
                    SpawnManager.SpawnVFX(this.launchVfx, this.m_aiActor.specRigidbody.UnitCenter, Quaternion.identity);
                }
                if (this.trailVfx)
                {
                    this.m_trailVfx = SpawnManager.SpawnParticleSystem(this.trailVfx, this.m_aiActor.sprite.WorldCenter, Quaternion.Euler(0f, 0f, num));
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
                if (!string.IsNullOrEmpty(this.trailVfxString))
                {
                    Vector2 normalized = this.m_aiActor.BehaviorVelocity.normalized;
                    string arg;
                    if (normalized.x > 0.75f)
                    {
                        arg = "right";
                    }
                    else if (normalized.x < -0.75f)
                    {
                        arg = "left";
                    }
                    else if (normalized.y > 0.75f)
                    {
                        arg = "up";
                    }
                    else
                    {
                        arg = "down";
                    }
                    this.m_cachedTrailString = string.Format(this.trailVfxString, arg);
                    AIAnimator aiAnimator = this.m_aiAnimator;
                    string cachedTrailString = this.m_cachedTrailString;
                    Vector2? sourceVelocity = new Vector2?(normalized);
                    aiAnimator.PlayVfx(cachedTrailString, null, sourceVelocity, null);
                }
                else
                {
                    this.m_cachedTrailString = null;
                }
                this.m_aiActor.specRigidbody.ForceRegenerate(null, null);
            }
            else if (state == SpecificDirectionDashBehaviour.FireState.Bouncing)
            {
                if (this.m_bulletSource) 
                {
                    this.m_bulletSource.ForceStop();
                }

                switch (CurrentHeadedDirection)
                {
                    case DungeonData.Direction.NORTH:
                        this.m_aiAnimator.PlayUntilFinished(this.ImpactAnimation_North, true, null, -1f, false);
                        break;
                    case DungeonData.Direction.SOUTH:
                        this.m_aiAnimator.PlayUntilFinished(this.ImpactAnimation_South, true, null, -1f, false);
                        break;
                    case DungeonData.Direction.EAST:
                        this.m_aiAnimator.PlayUntilFinished(this.ImpactAnimation_East, true, null, -1f, false);
                        break;
                    case DungeonData.Direction.WEST:
                        this.m_aiAnimator.PlayUntilFinished(this.ImpactAnimation_West, true, null, -1f, false);
                        break;
                }

                if (this.impactBulletScript != null && !this.impactBulletScript.IsNull && this.m_hitWall)
                {
                    if (!this.m_bulletSource)
                    {
                        this.m_bulletSource = this.shootPoint.GetOrAddComponent<BulletScriptSource>();
                    }
                    this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
                    this.m_bulletSource.BulletScript = this.impactBulletScript;
                    this.m_bulletSource.Initialize();
                }
            }
        }

        private void EndState(SpecificDirectionDashBehaviour.FireState state)
        {
            if (state == SpecificDirectionDashBehaviour.FireState.Charging)
            {
                this.m_aiActor.BehaviorVelocity = Vector2.zero;
                this.m_aiActor.CollisionKnockbackStrength = this.m_cachedKnockback;
                this.m_aiActor.CollisionDamage = this.m_cachedDamage;
                this.m_aiActor.CollisionVFX = this.m_cachedVfx;
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
                if (!string.IsNullOrEmpty(this.m_cachedTrailString))
                {
                    this.m_aiAnimator.StopVfx(this.m_cachedTrailString);
                    this.m_cachedTrailString = null;
                }
                this.m_aiActor.DoDustUps = this.m_cachedDoDustUps;
                this.m_aiActor.PathableTiles = this.m_cachedPathableTiles;
                if (this.switchCollidersOnCharge)
                {
                    this.m_enemyCollider.CollisionLayer = CollisionLayer.EnemyCollider;
                    this.m_enemyHitbox.Enabled = true;
                    this.m_projectileCollider.Enabled = false;
                    PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.m_aiActor.specRigidbody, null, false);
                }
            }
        }

        private bool ShouldChargePlayer(PlayerController player, ref Dungeonator.DungeonData.Direction direction)
        {
            Vector2 vector = player.specRigidbody.GetUnitCenter(ColliderType.HitBox);
            if (this.leadAmount > 0f)
            {
                Vector2 b = vector + player.specRigidbody.Velocity * this.m_primeAnimTime;
                vector = Vector2.Lerp(vector, b, this.leadAmount);
            }
            Vector2 unitBottomLeft = this.m_aiActor.specRigidbody.UnitBottomLeft;
            Vector2 unitTopRight = this.m_aiActor.specRigidbody.UnitTopRight;
            this.m_chargeDir = null;
            if (BraveMathCollege.AABBContains(new Vector2(unitBottomLeft.x - this.chargeRange, unitBottomLeft.y), unitTopRight, vector) && CanChargeLeft == true)
            {
                direction = DungeonData.Direction.WEST;
                this.m_chargeDir = new Vector2?(-Vector2.right);
            }
            else if (BraveMathCollege.AABBContains(unitBottomLeft, new Vector2(unitTopRight.x + this.chargeRange, unitTopRight.y), vector) && CanChargeRight == true)
            {
                direction = DungeonData.Direction.EAST;
                this.m_chargeDir = new Vector2?(Vector2.right);
            }
            else if (BraveMathCollege.AABBContains(new Vector2(unitBottomLeft.x, unitBottomLeft.y - this.chargeRange), unitTopRight, vector) && CanChargeDown == true)
            {
                direction = DungeonData.Direction.SOUTH;
                this.m_chargeDir = new Vector2?(-Vector2.up);
            }
            else if (BraveMathCollege.AABBContains(unitBottomLeft, new Vector2(unitTopRight.x, unitTopRight.y + this.chargeRange), vector) && CanChargeUp == true)
            {
                direction = DungeonData.Direction.NORTH;
                this.m_chargeDir = new Vector2?(Vector2.up);
            }
            return this.m_chargeDir != null;
        }
        private  DungeonData.Direction CurrentHeadedDirection;

        

        public bool CanChargeUp;
        public bool CanChargeDown;
        public bool CanChargeLeft;
        public bool CanChargeRight;

        public string primeAnim;
        public string chargeAnim;

        public string ImpactAnimation_North;
        public string ImpactAnimation_South;
        public string ImpactAnimation_East;
        public string ImpactAnimation_West;


        public bool endWhenChargeAnimFinishes;

        public bool switchCollidersOnCharge;

        public string hitAnim;

        public string hitPlayerAnim;

        public float leadAmount;

        public float chargeRange = 15f;

        public float chargeSpeed;

        public float chargeKnockback = 50f;

        public float chargeDamage = 0.5f;

        public bool delayWallRecoil;

        public float wallRecoilForce = 10f;

        public bool stopAtPits = true;

        public GameObject launchVfx;

        public GameObject trailVfx;

        public Transform trailVfxParent;


        public GameObject hitVfx;
        public string trailVfxString;

        public string hitWallVfxString;
        [InspectorHeader("Impact BulletScript")]
        public GameObject shootPoint;
        public BulletScriptSelector impactBulletScript;

        public BulletScriptSelector dashBulletScript;


        private SpecificDirectionDashBehaviour.FireState m_state;
        public float m_primeAnimTime;
        private Vector2? m_chargeDir;
        private Vector2? m_storedCollisionNormal;
        private bool m_hitPlayer;
        private bool m_hitWall;
        private float m_cachedKnockback;
        private float m_cachedDamage;
        private VFXPool m_cachedVfx;
        private CellTypes m_cachedPathableTiles;
        private bool m_cachedDoDustUps;
        private PixelCollider m_enemyCollider;
        private PixelCollider m_enemyHitbox;
        private PixelCollider m_projectileCollider;
        private GameObject m_trailVfx;
        private string m_cachedTrailString;
        private BulletScriptSource m_bulletSource;

        private enum FireState
        {
            Idle,
            Priming,
            Charging,
            Bouncing
        }
    }
}
