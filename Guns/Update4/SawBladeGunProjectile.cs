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
using System.Collections.ObjectModel;
using static UnityEngine.UI.CanvasScaler;
using Planetside.DungeonPlaceables;




namespace Planetside
{
	internal class SawBladeGunProjectile : Projectile
	{

        public SpeculativeRigidbody _SpeculativeRigidbody;
        public override void Start()
        {
            if (this.m_initialized)
            {
                return;
            }
            this.m_initialized = true;
            this.m_transform = base.transform;
            if (!string.IsNullOrEmpty(this.additionalStartEventName))
            {
                AkSoundEngine.PostEvent(this.additionalStartEventName, base.gameObject);
            }
            StaticReferenceManager.AddProjectile(this);
            if (base.GetComponent<BeamController>())
            {
                base.enabled = false;
                return;
            }
            if (this.m_renderer)
            {
                DepthLookupManager.ProcessRenderer(this.m_renderer);
            }
            if (base.sprite)
            {
                this.m_currentRampHeight = 0f;
                float num = BraveMathCollege.ClampAngle360(this.m_transform.eulerAngles.z);
                if (this.Owner is PlayerController)
                {
                    float num2 = (this.Owner as PlayerController).BulletScaleModifier;
                    num2 = Mathf.Clamp(num2 * this.AdditionalScaleMultiplier, 0.01f, Projectile.s_maxProjectileScale);
                    base.sprite.scale = new Vector3(num2, num2, num2);
                    if (num2 != 1f)
                    {
                        if (base.specRigidbody != null)
                        {
                            base.specRigidbody.UpdateCollidersOnScale = true;
                            base.specRigidbody.ForceRegenerate(null, null);
                        }
                        if (base.sprite.transform != this.m_transform)
                        {
                            base.sprite.transform.localPosition = Vector3.Scale(base.sprite.transform.localPosition, base.sprite.scale);
                        }
                        this.DoWallExitClipping(1f);
                    }
                    if (this.HasDefaultTint)
                    {
                        this.AdjustPlayerProjectileTint(this.DefaultTintColor, 0, 0f);
                    }
                }
                if (this.shouldRotate && this.shouldFlipVertically)
                {
                    base.sprite.FlipY = (num < 270f && num > 90f);
                }
                if (this.shouldFlipHorizontally)
                {
                    base.sprite.FlipX = (num > 90f && num < 270f);
                }
            }
            if (base.specRigidbody != null && this.Owner is PlayerController)
            {
                base.specRigidbody.UpdateCollidersOnRotation = true;
                base.specRigidbody.UpdateCollidersOnScale = true;
            }
            if (this.isFakeBullet)
            {
                base.enabled = false;
                base.sprite.UpdateZDepth();
                return;
            }
            if (base.specRigidbody == null)
            {
                UnityEngine.Debug.LogError("No speculative rigidbody found on projectile!", this);
            }
            if (GameManager.PVP_ENABLED && !this.TreatedAsNonProjectileForChallenge)
            {
                this.collidesWithPlayer = true;
            }
            if (this.collidesWithPlayer && this.Owner is AIActor && (this.Owner as AIActor).CompanionOwner != null)
            {
                this.collidesWithPlayer = false;
            }
            if (this.collidesWithProjectiles)
            {
                for (int i = 0; i < base.specRigidbody.PixelColliders.Count; i++)
                {
                    base.specRigidbody.PixelColliders[i].CollisionLayerCollidableOverride |= CollisionMask.LayerToMask(CollisionLayer.Projectile);
                }
            }
            if (!this.collidesWithPlayer)
            {
                for (int j = 0; j < base.specRigidbody.PixelColliders.Count; j++)
                {
                    base.specRigidbody.PixelColliders[j].CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox);
                }
            }
            if (!this.collidesWithEnemies)
            {
                for (int k = 0; k < base.specRigidbody.PixelColliders.Count; k++)
                {
                    base.specRigidbody.PixelColliders[k].CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox);
                }
            }
            if (this.Owner is PlayerController)
            {
                for (int l = 0; l < base.specRigidbody.PixelColliders.Count; l++)
                {
                    base.specRigidbody.PixelColliders[l].CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.EnemyBulletBlocker);
                }
            }
            else if (this.Owner is AIActor && this.collidesWithEnemies && PassiveItem.IsFlagSetAtAll(typeof(BattleStandardItem)))
            {
                this.baseData.damage *= BattleStandardItem.BattleStandardCompanionDamageMultiplier;
            }
            if (this.Owner is PlayerController)
            {
                this.PostprocessPlayerBullet();
            }
            if (base.specRigidbody.UpdateCollidersOnRotation)
            {
                base.specRigidbody.ForceRegenerate(null, null);
            }
            this.m_timeElapsed = 0f;
            this.LastPosition = this.m_transform.position;
            this.m_currentSpeed = this.baseData.speed;
            this.m_currentDirection = this.m_transform.right;
            if (!this.shouldRotate)
            {
                this.m_transform.rotation = Quaternion.identity;
            }
            if (this.CanKillBosses)
            {
                base.StartCoroutine(this.CheckIfBossKillShot());
            }
            if (!this.shouldRotate)
            {
                base.specRigidbody.IgnorePixelGrid = true;
                //base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
            }
            if (this.angularVelocity != 0f)
            {
                this.angularVelocity = BraveUtility.RandomSign() * this.angularVelocity + UnityEngine.Random.Range(-this.angularVelocityVariance, this.angularVelocityVariance);
            }
            this.CheckBlackPhantomness();
            this.specRigidbody.CollideWithTileMap = true;
            this.specRigidbody.OnPostRigidbodyMovement += this.OnPostRigidbodyMovement;
            //this.projectile.DestroyMode = Projectile.ProjectileDestroyMode.None;
            this.sprite.IsPerpendicular = true;
            this.sprite.IsPerpendicular = false;
        }

        public override void OnRigidbodyCollision(CollisionData rigidbodyCollision)
        {


            base.OnRigidbodyCollision(rigidbodyCollision);
        }


        private bool TemporaryDisabler = false;

        public override void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
        {
            /*
            Debug.Log("======");
            Debug.Log(otherRigidbody.gameObject);
            Debug.Log("-");

            foreach (var entry in otherRigidbody.gameObject.GetComponents(typeof(Component)))
            {
                Debug.Log(entry.GetType());
                Debug.Log("-");
            }
            Debug.Log("======");
            */


            if (specRigidbody.TemporaryCollisionExceptions != null)
            {
                if (specRigidbody.TemporaryCollisionExceptions.Where(x => x.SpecRigidbody == otherRigidbody).Count() > 0)
                {
                    PhysicsEngine.SkipCollision = true;
                    return;
                }
            }



            if (isAttached)
            {
                bool Returns = false;
                var c = otherRigidbody.GetComponentsInParent(typeof(Component));

                if (otherRigidbody.IsActuallyOubiletteEntranceRoom())
                {
                    TemporaryDisabler = true;
                    this.Invoke("DoWait", Time.deltaTime);
                    PhysicsEngine.SkipCollision = true;
                    DoBonkParticles(32);
                    AkSoundEngine.PostEvent("Play_MetalImpactHit", this.gameObject);
                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = sprite.WorldCenter,
                        startSize = 16,
                        rotation = 0,
                        startLifetime = 0.333f,
                        startColor = Color.white * 0.4f
                    });
                    specRigidbody.RegisterTemporaryCollisionException(otherRigidbody, 1);

                    //ForColliderCheck = previousNormal;
                    //previousNormal = previousNormal.Value.Rotate(this._Direction * -1);
                    //_LastVelocity.Rotate(this._Direction * -1);
                    _LastVelocity *= -1;
                    _Direction *= -1;


                    return;
                }



                foreach (var c2 in c)
                {

                    if (c2 is DungeonDoorController door)
                    {
                        //ForColliderCheck = previousNormal;
                        //previousNormal = previousNormal.Value.Rotate(this._Direction * -1);
                        //_LastVelocity.Rotate(this._Direction * -1);

                        //_LastVelocity *= -1;
                        //_Direction *= -1;
                        foreach (var d in door.doorModules)
                        {
                          
                            specRigidbody.RegisterTemporaryCollisionException(d.rigidbody, this.LastVelocity.magnitude * BraveTime.DeltaTime * 3);
                        }

                        foreach (var c_1 in door.GetComponentsInChildren<SpeculativeRigidbody>())
                        {
                            specRigidbody.RegisterTemporaryCollisionException(c_1, this.LastVelocity.magnitude * BraveTime.DeltaTime * 3);
                        }

                        Returns = true;



                        _LastVelocity = _LastVelocity.Rotate(this._Direction * -1);
                        ForColliderCheck = previousNormal;
                        previousNormal = previousNormal.Value.Rotate(this._Direction);

                        PhysicsEngine.SkipCollision = true;
                        break;
                    }

                   
                }
                c = otherRigidbody.GetComponents(typeof(Component));
                foreach (var c2 in c)
                {

                    if (c2 is FireplaceController fireplace)
                    {
                        PhysicsEngine.SkipCollision = true;
                        Returns = true;

                        _LastVelocity *= -1;
                        _Direction *= -1;

                        DoBonkParticles(32);
                        specRigidbody.RegisterTemporaryCollisionException(fireplace.specRigidbody, 0.3f);
                        break;
                    }

                    if (c2 is MajorBreakable breakable)
                    {
                        if (breakable.GetComponent<Idol>())
                        {
                            PhysicsEngine.SkipCollision = true;
                            DoBonkParticles(32);
                            AkSoundEngine.PostEvent("Play_MetalImpactHit", this.gameObject);
                            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                            {
                                position = sprite.WorldCenter,
                                startSize = 16,
                                rotation = 0,
                                startLifetime = 0.333f,
                                startColor = Color.white * 0.4f
                            });
                            breakable.ApplyDamage(1E+10f, Vector2.zero, false, true, true);
                            break;
                        }


                        if (breakable.IsSecretDoor)
                        {
                            PhysicsEngine.SkipCollision = true;
                            Returns = true;

                            _LastVelocity *= -1;
                            _Direction *= -1;


                            DoBonkParticles(32);
                            AkSoundEngine.PostEvent("Play_MetalImpactHit", this.gameObject);
                            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                            {
                                position = sprite.WorldCenter,
                                startSize = 16,
                                rotation = 0,
                                startLifetime = 0.333f,
                                startColor = Color.white * 0.4f
                            });
                            specRigidbody.RegisterTemporaryCollisionException(breakable.specRigidbody, 0.3f);
                            break;

                        }

                    }

                    if (c2 is DungeonDoorSubsidiaryBlocker blocker)
                    {


                        specRigidbody.RegisterTemporaryCollisionException(blocker.parentDoor.specRigidbody, 0.3f);
                        foreach (var d in blocker.parentDoor.doorModules)
                        {
                            specRigidbody.RegisterTemporaryCollisionException(d.rigidbody, 0.3f);
                        }
                        PhysicsEngine.SkipCollision = true;
                        Returns = true;


                        _LastVelocity = _LastVelocity.Rotate(this._Direction * -1);
                        ForColliderCheck = previousNormal;
                        previousNormal = previousNormal.Value.Rotate(this._Direction);

                        //ForColliderCheck = previousNormal;
                        //previousNormal = previousNormal.Value.Rotate(this._Direction * -1);
                        //_LastVelocity *= -1;
                        //_Direction *= -1;
                        break;

                    }
                    if (c2 is ForgeCrushDoorController crusher)
                    {
                        if (crusher.m_isCrushing)
                        {

                            PhysicsEngine.SkipCollision = true;
                            specRigidbody.RegisterTemporaryCollisionException(crusher.specRigidbody, 0.3f);
                            Returns = true;



                            //_LastVelocity *= -1;
                            //_Direction *= -1;


                            _LastVelocity = _LastVelocity.Rotate(this._Direction * -1);
                            ForColliderCheck = previousNormal;
                            previousNormal = previousNormal.Value.Rotate(this._Direction);
                            break;
                        }
                    }
                }
                if (Returns)
                {
                    TemporaryDisabler = true;
                    //this.Invoke("DoWait", Time.deltaTime);
                    return;
                }
            }
            
            if (otherRigidbody == this.m_shooter && !this.allowSelfShooting)
            {
                PhysicsEngine.SkipCollision = true;
                return;
            }
            if (otherRigidbody.gameActor != null && otherRigidbody.gameActor is PlayerController && (!this.collidesWithPlayer || (otherRigidbody.gameActor as PlayerController).IsGhost || (otherRigidbody.gameActor as PlayerController).IsEthereal))
            {
                PhysicsEngine.SkipCollision = true;
                return;
            }
            if (otherRigidbody.aiActor)
            {
                if (this.Owner is PlayerController && !otherRigidbody.aiActor.IsNormalEnemy)
                {
                    PhysicsEngine.SkipCollision = true;
                    return;
                }
                if (this.Owner is AIActor && !this.collidesWithEnemies && otherRigidbody.aiActor.IsNormalEnemy && !otherRigidbody.aiActor.HitByEnemyBullets)
                {
                    PhysicsEngine.SkipCollision = true;
                    return;
                }
            }
            if (!GameManager.PVP_ENABLED && this.Owner is PlayerController && otherRigidbody.GetComponent<PlayerController>() != null && !this.allowSelfShooting)
            {
                PhysicsEngine.SkipCollision = true;
                return;
            }
            if (GameManager.Instance.InTutorial)
            {
                PlayerController component = otherRigidbody.GetComponent<PlayerController>();
                if (component)
                {
                    if (component.spriteAnimator.QueryInvulnerabilityFrame())
                    {
                        GameManager.BroadcastRoomTalkDoerFsmEvent("playerDodgedBullet");
                    }
                    else if (component.IsDodgeRolling)
                    {
                        GameManager.BroadcastRoomTalkDoerFsmEvent("playerAlmostDodgedBullet");
                    }
                    else
                    {
                        GameManager.BroadcastRoomTalkDoerFsmEvent("playerDidNotDodgeBullet");
                    }
                }
            }
            if (otherRigidbody.healthHaver != null && otherRigidbody.healthHaver.spriteAnimator != null && otherCollider.CollisionLayer == CollisionLayer.PlayerHitBox && otherRigidbody.spriteAnimator.QueryInvulnerabilityFrame())
            {
                PhysicsEngine.SkipCollision = true;
                base.StartCoroutine(this.HandlePostInvulnerabilityFrameExceptions(otherRigidbody));
                return;
            }
            if (this.collidesWithProjectiles && this.collidesOnlyWithPlayerProjectiles && otherRigidbody.projectile && !(otherRigidbody.projectile.Owner is PlayerController))
            {
                PhysicsEngine.SkipCollision = true;
                return;
            }
        }


        private void DoWait()
        {
            TemporaryDisabler = false;
        }

        public override void OnPreTileCollision(SpeculativeRigidbody myrigidbody, PixelCollider mypixelcollider, PhysicsEngine.Tile tile, PixelCollider tilepixelcollider)
        {
            if (myrigidbody)
            {
                this.projectile.DestroyMode = Projectile.ProjectileDestroyMode.None;
                this.projectile.specRigidbody.CollideWithTileMap = true;
            }
        }
            SpeculativeRigidbody lastTile;


        public static float RelAngleTo(float angle, float other)
        {
            return BraveMathCollege.ClampAngle180((other - angle));
        }



        public override void OnTileCollision(CollisionData tileCollision)
        {
            if (tileCollision.Normal != Vector2.right && tileCollision.Normal != Vector2.up && tileCollision.Normal != Vector2.left && tileCollision.Normal != Vector2.down)
                return;
            if (previousNormal != null && tileCollision.Normal == previousNormal)
                return;
            if (ForColliderCheck != null && ForColliderCheck == tileCollision.Normal)
                return;
            lastTile = tileCollision.OtherRigidbody;
            if (previousNormal != null)
            {
                ForColliderCheck = previousNormal;
            }
            previousNormal = tileCollision.Normal;

            if (!isAttached) 
            {

                //saw_RAGH
                this.spriteAnimator.Play("saw_RAGH");

                AkSoundEngine.PostEvent("Play_MetalImpactHit", this.gameObject);

                _LastVelocity = this.LastVelocity;
                ClockWise = RelAngleTo((-tileCollision.Normal).ToAngle(), this._LastVelocity.ToAngle()) > 0;
                _Direction = ClockWise ? -90f : 90f;
                isAttached = true;
                m_timeElapsed = -1;
                this.baseData.speed *= 0.6f;
                this.UpdateSpeed();
                this.baseData.CustomAccelerationCurveDuration = 1.25f;
                this.baseData.AccelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);
                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = sprite.WorldCenter,
                    startSize = 24,
                    rotation = 0,
                    startLifetime = 0.5f,
                    startColor = Color.white * 0.4f
                });
                this.ResetDistance();
                float angle = tileCollision.Normal.ToAngle();

                DoBonkParticles(64);

            }
            else
            {
                DoBonkParticles(32);
            }
            //AkSoundEngine.PostEvent("Play_MetalImpactHit", this.gameObject);

            TemporaryDisabler = false;
            _LastVelocity = previousNormal.Value.Rotate(this._Direction);
        }


        public void DoBonkParticles(int Amount)
        {
            for (int i = 0; i < Amount; i++)
            {
                var unit = MathToolbox.GetUnitOnCircle(_Direction, UnityEngine.Random.Range(0.1f, 2));

                ParticleBase.EmitParticles("SawSpark", 1, new ParticleSystem.EmitParams()
                {
                    position = sprite.WorldCenter + (unit * (BraveUtility.RandomBool() ? 1 : -1)),
                    rotation = 0,
                    startLifetime = 0.2f,
                    startSize = 0.125f,
                    velocity = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(this.baseData.speed * 0.7f, this.baseData.speed * 1.2f))
                });
            }
        }

        private Vector2 _LastVelocity;
        private Vector2? ForColliderCheck;
        private Vector2? previousNormal;

        
        private float _Direction;
        private bool isAttached = false;
        private bool ClockWise;
        private float _ElapsedSaw = -1;
        public override void Move()
        {

            if (isAttached)
            {
                _ElapsedSaw += this.LocalDeltaTime;
                /*
                int x;
                int y;

                GameManager.Instance.Dungeon.data.tilemap.GetTileAtPosition(this.transform.position + new Vector3(-previousNormal.Value.x, -previousNormal.Value.y), out x, out y);
                var n = newVelocity.normalized.ToIntVector2();
                var rot = newVelocity.normalized;
                var _ = new IntVector2(n.x + rot.ToIntVector2().x, n.y + rot.ToIntVector2().y);
                var cell = GameManager.Instance.Dungeon.data[new IntVector2(x + _.x, y + _.y)];
                if (cell != null && !cell.IsAnyFaceWall())
                {
                     newVelocity = previousNormal.Value.Rotate(this._Direction);
                    ForColliderCheck = previousNormal;
                    previousNormal = previousNormal.Value.Rotate(this._Direction);

                }
                */
                float time = Mathf.Clamp01((_ElapsedSaw - this.baseData.IgnoreAccelCurveTime) / this.baseData.CustomAccelerationCurveDuration);
                this.m_currentSpeed = this.baseData.AccelerationCurve.Evaluate(time) * this.baseData.speed;

                


                this.m_timeElapsed += this.LocalDeltaTime;
                base.specRigidbody.Velocity = _LastVelocity * this.m_currentSpeed;
                this.m_currentSpeed *= 1f - this.baseData.damping * this.LocalDeltaTime;
                this.LastVelocity = base.specRigidbody.Velocity;
                //_SpeculativeRigidbody.Velocity = LastVelocity;
                return;
            }
            base.Move();
        }

        private Vector2 TrueDir = Vector2.left;
        private void OnPostRigidbodyMovement(SpeculativeRigidbody specRigidbody, Vector2 unitDelta, IntVector2 pixelDelta)
        {
            if (TemporaryDisabler)
                return;
            if (_ElapsedSaw < 0)
                return;

            if (!this.isAttached)
                return; // don't do anything until we've hit a wall once
            if (this.specRigidbody.IsAgainstWall(-this.previousNormal.Value.ToIntVector2()))
                return; // if we're already up against a wall, no adjustments are needed
                        // move slightly towards the direction we're supposed to be going

            Vector2 _newVelocity = -this.previousNormal.Value.normalized;//.Rotate(_Direction * -1);



            this.specRigidbody.transform.position += (0.0625f * _newVelocity).ToVector3ZUp(0f);
            this.specRigidbody.Reinitialize();
            this.projectile.specRigidbody.Reinitialize();


            // snap to the wall in the opposite direction we've overshot from
            int pixelsAdjusted = this.specRigidbody.PushAgainstWalls(-_LastVelocity.normalized.ToIntVector2());
            this.projectile.specRigidbody.Reinitialize();

            // set the new normal to our current velocity

            ForColliderCheck = null;// this.previousNormal;
            this.previousNormal = _newVelocity.normalized;
            DoBonkParticles(16);

            _LastVelocity = previousNormal.Value.Rotate(this._Direction);
        }




        bool b = false;

        public override void Update()
        {
            base.Update();
            if (_ElapsedSaw > 0)
            {
                if (!b)
                {
                    b = true;
                    AkSoundEngine.PostEvent("Play_SawLoop", this.gameObject);
                    AkSoundEngine.PostEvent("Play_SawStart", this.gameObject);
                }
                this.sprite.transform.Rotate(0, 0, 120 * this.LastVelocity.magnitude * BraveTime.DeltaTime);

                /*
                ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = sprite.WorldCenter,
                    rotation = 0,
                    startLifetime = 0.25f,
                    startSize = 0.125f,
                    startColor = Color.red,
                    velocity = _LastVelocity.normalized * 10
                });
                if (previousNormal != null)
                {
                    ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = sprite.WorldCenter,
                        rotation = 0,
                        startLifetime = 0.25f,
                        startSize = 0.125f,
                        startColor = Color.green,
                        velocity = previousNormal.Value.normalized * 10
                    });
                }

                if (ForColliderCheck != null)
                {
                    ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = sprite.WorldCenter,
                        rotation = 0,
                        startLifetime = 0.25f,
                        startSize = 0.125f,
                        startColor = Color.blue,
                        velocity = previousNormal.Value.normalized * 5
                    });
                }

                */
                if (_ElapsedSaw > 0.5f)
                {
                    /*
                    var unit = (previousNormal.Value.Rotate(90) * transform.localScale.magnitude);

                    ParticleBase.EmitParticles("SawSpark", 1, new ParticleSystem.EmitParams()
                    {
                        position = sprite.WorldCenter + unit,
                        rotation = 0,
                        startLifetime = 0.5f,
                        startSize = 0.125f,
                        velocity = this.LastVelocity.normalized
                    });
                    ParticleBase.EmitParticles("SawSpark", 1, new ParticleSystem.EmitParams()
                    {
                        position = sprite.WorldCenter - unit,
                        rotation = 0,
                        startLifetime = 0.5f,
                        startSize = 0.125f,
                        velocity = this.LastVelocity.normalized
                    });
                    */
                }

            }     
            var room = this.transform.position.GetAbsoluteRoom();
            if (room != null)
            {
                var enm = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (enm != null)
                {
                    for (int a = enm.Count - 1; a > -1; a--)
                    {
                        var entry = enm[a];
                        if (entry.State == AIActor.ActorState.Normal && entry.healthHaver.IsVulnerable)
                        {
                            if (Vector2.Distance(entry.transform.position, this.transform.position) < 1.75f * this.transform.localScale.magnitude)
                            {
                                if (!EnemyHitCooldown.ContainsKey(entry))
                                {
                                    EnemyHitCooldown.Add(entry, 0.2f);
                                    this.ForceCollision(entry.specRigidbody, new LinearCastResult()
                                    {
                                        CollidedX = true,
                                        CollidedY = true,
                                        Contact = this.specRigidbody.UnitCenter,
                                        MyPixelCollider = this.specRigidbody.PixelColliders[0],
                                        NewPixelsToMove = new IntVector2(0, 0),
                                        Normal = Vector2.zero,
                                        OtherPixelCollider = this.specRigidbody.PixelColliders[0],
                                        Overlap = true,
                                        TimeUsed = 0,
                                    });

                                    for (int i = 0; i < 25; i++)
                                    {
                                        var unit = MathToolbox.GetUnitOnCircle(_Direction, UnityEngine.Random.Range(0.1f, 2));

                                        ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                                        {
                                            position = entry.sprite.WorldCenter,
                                            rotation = 0,
                                            startLifetime = 0.5f,
                                            startColor = Color.red,
                                            velocity = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(this.baseData.speed * 0.05f, this.baseData.speed * 0.1f)) + this.LastVelocity * 0.1f
                                        });
                                    }
                                    DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Alexandria.Misc.GoopUtility.BloodDef).TimedAddGoopCircle(entry.sprite.WorldBottomCenter, 2, 0.7f);

                                }
                            }
                        }
                    }

   
                }
            }
            int count = EnemyHitCooldown.Count();
            if (count > 0)
            {
                var keys = EnemyHitCooldown.Keys.ToList();
                for (int i = count - 1; i > -1; i--)
                {
                    EnemyHitCooldown[keys[i]] -= LocalDeltaTime;
                    if (EnemyHitCooldown[keys[i]] <= 0)
                    {
                        EnemyHitCooldown.Remove(keys[i]);
                    }
                }
            }
        }

        private Dictionary<AIActor, float> EnemyHitCooldown = new Dictionary<AIActor, float>();

        public override Projectile.HandleDamageResult HandleDamage(SpeculativeRigidbody rigidbody, PixelCollider hitPixelCollider, out bool killedTarget, PlayerController player, bool alreadyPlayerDelayed = false)
        {
            killedTarget = false;
            if (rigidbody.ReflectProjectiles)
            {
                return Projectile.HandleDamageResult.NO_HEALTH;
            }
            if (!rigidbody.healthHaver)
            {
                return Projectile.HandleDamageResult.NO_HEALTH;
            }
            if (!alreadyPlayerDelayed && Projectile.s_delayPlayerDamage && player)
            {
                return Projectile.HandleDamageResult.HEALTH;
            }
            if (rigidbody.spriteAnimator != null && rigidbody.spriteAnimator.QueryInvulnerabilityFrame())
            {
                return Projectile.HandleDamageResult.HEALTH;
            }
            bool flag = !rigidbody.healthHaver.IsDead;
            float num = this.ModifiedDamage;
            if (_ElapsedSaw > 0)
            {
                num = this.ModifiedDamage * 0.5f;
                num += this.specRigidbody.Velocity.magnitude * 0.1f;
            }



            if (this.Owner is AIActor && rigidbody && rigidbody.aiActor && (this.Owner as AIActor).IsNormalEnemy)
            {
                num = ProjectileData.FixedFallbackDamageToEnemies;
                if (rigidbody.aiActor.HitByEnemyBullets)
                {
                    num /= 4f;
                }
            }
            if (this.Owner is PlayerController && this.m_hasPierced && this.m_healthHaverHitCount >= 1)
            {
                int num2 = Mathf.Clamp(this.m_healthHaverHitCount - 1, 0, GameManager.Instance.PierceDamageScaling.Length - 1);
                num *= GameManager.Instance.PierceDamageScaling[num2];
            }
            if (this.OnWillKillEnemy != null && num >= rigidbody.healthHaver.GetCurrentHealth())
            {
                this.OnWillKillEnemy(this, rigidbody);
            }
            if (rigidbody.healthHaver.IsBoss)
            {
                num *= this.BossDamageMultiplier;
            }
            if (this.BlackPhantomDamageMultiplier != 1f && rigidbody.aiActor && rigidbody.aiActor.IsBlackPhantom)
            {
                num *= this.BlackPhantomDamageMultiplier;
            }
            bool flag2 = false;
            if (this.DelayedDamageToExploders)
            {
                flag2 = (rigidbody.GetComponent<ExplodeOnDeath>() && rigidbody.healthHaver.GetCurrentHealth() <= num);
            }
            if (!flag2)
            {
                HealthHaver healthHaver = rigidbody.healthHaver;
                float damage = num;
                Vector2 velocity = base.specRigidbody.Velocity;
                string ownerName = this.OwnerName;
                CoreDamageTypes coreDamageTypes = this.damageTypes;
                DamageCategory damageCategory = (!this.IsBlackBullet) ? DamageCategory.Normal : DamageCategory.BlackBullet;
                healthHaver.ApplyDamage(damage, velocity, ownerName, coreDamageTypes, damageCategory, false, hitPixelCollider, this.ignoreDamageCaps);
                if (player && player.OnHitByProjectile != null)
                {
                    player.OnHitByProjectile(this, player);
                }
            }
            else
            {
                rigidbody.StartCoroutine(this.HandleDelayedDamage(rigidbody, num, base.specRigidbody.Velocity, hitPixelCollider));
            }
            if (this.Owner && this.Owner is AIActor && player)
            {
                (this.Owner as AIActor).HasDamagedPlayer = true;
            }
            killedTarget = (flag && rigidbody.healthHaver.IsDead);
            if (!killedTarget && rigidbody.gameActor != null)
            {
                if (this.AppliesPoison && UnityEngine.Random.value < this.PoisonApplyChance)
                {
                    rigidbody.gameActor.ApplyEffect(this.healthEffect, 1f, null);
                }
                if (this.AppliesSpeedModifier && UnityEngine.Random.value < this.SpeedApplyChance)
                {
                    rigidbody.gameActor.ApplyEffect(this.speedEffect, 1f, null);
                }
                if (this.AppliesCharm && UnityEngine.Random.value < this.CharmApplyChance)
                {
                    rigidbody.gameActor.ApplyEffect(this.charmEffect, 1f, null);
                }
                if (this.AppliesFreeze && UnityEngine.Random.value < this.FreezeApplyChance)
                {
                    rigidbody.gameActor.ApplyEffect(this.freezeEffect, 1f, null);
                }
                if (this.AppliesCheese && UnityEngine.Random.value < this.CheeseApplyChance)
                {
                    rigidbody.gameActor.ApplyEffect(this.cheeseEffect, 1f, null);
                }
                if (this.AppliesBleed && UnityEngine.Random.value < this.BleedApplyChance)
                {
                    rigidbody.gameActor.ApplyEffect(this.bleedEffect, -1f, this);
                }
                if (this.AppliesFire && UnityEngine.Random.value < this.FireApplyChance)
                {
                    rigidbody.gameActor.ApplyEffect(this.fireEffect, 1f, null);
                }
                if (this.AppliesStun && UnityEngine.Random.value < this.StunApplyChance && rigidbody.gameActor.behaviorSpeculator)
                {
                    rigidbody.gameActor.behaviorSpeculator.Stun(this.AppliedStunDuration, true);
                }
                for (int i = 0; i < this.statusEffectsToApply.Count; i++)
                {
                    rigidbody.gameActor.ApplyEffect(this.statusEffectsToApply[i], 1f, null);
                }
            }
            this.m_healthHaverHitCount++;
            return (!killedTarget) ? Projectile.HandleDamageResult.HEALTH : Projectile.HandleDamageResult.HEALTH_AND_KILLED;
        }




        public override void OnDestroy()
        {
            AkSoundEngine.PostEvent("Stop_SawLoop", this.gameObject);
            var _ = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericLargeExplosion);
            _.ignoreList = new List<SpeculativeRigidbody>();
            foreach (var p in GameManager.Instance.AllPlayers)
            {
                _.ignoreList.Add(p.specRigidbody);
            }
            Exploder.Explode(this.transform.position, _, Vector2.zero);
            if (Owner && Owner is PlayerController player && player.PlayerHasActiveSynergy("Blades Of Wrath"))
            {
                if (player.HasGun(Guns.Buzzkill.PickupObjectId))
                {
                    for (int i = 0; i < 12; i++)
                    {
                        bool isGlaive = player.CurrentGun.PickupObjectId == 656;

                        GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(Guns.Super_Meat_Gun.DefaultModule.projectiles[0].gameObject, this.transform.position, Quaternion.Euler(0f, 0f, 30 * i), true);
                        Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                        if (component != null)
                        {
                            if (isGlaive)
                            {
                                component.baseData.damage *= 0.5f;
                                component.baseData.speed *= 1.5f;
                                component.UpdateSpeed();
                            }
                            component.Owner = player;
                            component.Shooter = player.specRigidbody;
                            player.DoPostProcessProjectile(component);
                        }
                    }
                }
                if (player.HasGun(Guns.Super_Meat_Gun.PickupObjectId))
                {
                    for (int i = 0; i < 12; i++)
                    {
                        bool isGlaive = player.CurrentGun.PickupObjectId == 656;

                        GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(Guns.Super_Meat_Gun.DefaultModule.projectiles[0].gameObject, this.transform.position, Quaternion.Euler(0f, 0f, 30 * i), true);
                        Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                        if (component != null)
                        {
                            if (isGlaive)
                            {
                                component.baseData.damage *= 0.5f;
                                component.baseData.speed *= 1.5f;
                                component.UpdateSpeed();
                            }
                            component.Owner = player;
                            component.Shooter = player.specRigidbody;
                            player.DoPostProcessProjectile(component);
                        }
                    }
                }
            }
            base.OnDestroy();
        }

    }
}

