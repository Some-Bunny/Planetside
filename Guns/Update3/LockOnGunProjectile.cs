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

using UnityEngine.Serialization;
using static UnityEngine.UI.GridLayoutGroup;
using SynergyAPI;
using static ETGMod;
using static AkMIDIEvent;
using MultiplayerBasicExample;

namespace Planetside
{
	public class LockOnGunProjectile : Projectile
	{
        public static List<LockOnGunProjectile> AllLockOns = new List<LockOnGunProjectile>();



        public float FireRateMult = 1;
        public float ClipMult = 1;
        public float ReloadMult = 1;

        private float FireRate = 0.3f;
        private float Clip = 4;
        private float Reload = 1.5f;


        public float AngualrVelocity;


        private bool SynergyRedirect = false;
        private bool SynergyMissiles = false;
        private bool SynergyLaser = false;
        //To The Point
        public override void Start()
        {
            base.Start();
            AllLockOns.Add(this);            
            if (Owner as PlayerController )
            {
                SynergyRedirect = (Owner as PlayerController).PlayerHasActiveSynergy("No Virus Included");
                SynergyMissiles = (Owner as PlayerController).PlayerHasActiveSynergy("Full Arsenal");
                SynergyLaser = (Owner as PlayerController).PlayerHasActiveSynergy("To The Point");
            }
            this.specRigidbody.OnCollision += (_) =>
            {
                if (laserPointer)
                {
                    AkSoundEngine.PostEvent("Play_ENM_cannonball_blast_01", this.gameObject);
                    var ex = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
                    ex.effect = null;
                    ex.damage = (projectile.baseData.damage * 0.3f) + 5;
                    ex.force = 100;
                    ex.doForce = true;
                    ex.preventPlayerForce = true;
                    ex.damageRadius = 3.5f;
                    ex.pushRadius = 3.5f;
                    ex.ignoreList = new List<SpeculativeRigidbody>()
                    {

                    };
                    foreach (var player in GameManager.Instance.AllPlayers)
                    {
                        if (player != null)
                        {
                            ex.ignoreList.Add(player.specRigidbody);
                        }
                    }

                    Exploder.Explode(this.sprite.WorldCenter, ex, Vector2.zero);
                    Exploder.DoDistortionWave(this.sprite.WorldCenter, 4, 0.05f, 3.5f, 0.2f);
                }
            };
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (laserPointer)
                Destroy(laserPointer.gameObject);
            AllLockOns.Remove(this);
        }

        private bool isHoldingTarget = false;
        private bool EnemyInSights = false;
        private float t = 1;
        private float t2 = 1;
        private float t3 = 1;
        private float t4 = 0;

        public void SetPointTowards(Vector3 p)
        {
            bool Controller = false;
            isHoldingTarget = true;
            var ___ = GetPointTowards(ref Controller, p).normalized;
            float s = 60 * BraveTime.DeltaTime;

            if (SynergyRedirect && Controller)
            {
                s *= 2;
            }

            this.SendInDirection(Vector2.MoveTowards(this.Direction, ___, s), false);

        }



        public override void Update()
        {
            bool PlayerOwned = projectile.Owner is PlayerController;

            base.Update();
            if (this == null)
                return;
            PlayerController play = projectile.Owner as PlayerController; 


            if (this.shouldRotate)
            {
                this.sprite.m_transform.eulerAngles = new Vector3(0f, 0f, Direction.ToAngle());
            }

            if (laserPointer)
            {
                laserPointer.transform.localRotation = Quaternion.Euler(0, 0, MathToolbox.ToAngle(this.Direction));
                laserPointer.transform.position = this.transform.position;

                laserPointer.ForceRotationRebuild();
                laserPointer.UpdateZDepth();

                Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;

                CollisionLayer layer2 = CollisionLayer.EnemyHitBox;
                int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, layer2, CollisionLayer.BulletBreakable);
                RaycastResult raycastResult2;
                if (PhysicsEngine.Instance.Raycast(this.sprite.WorldCenter, this.Direction, 1000, out raycastResult2, true, true, rayMask2, null, false, rigidbodyExcluder, null))
                {
                    if (raycastResult2.SpeculativeRigidbody && raycastResult2.SpeculativeRigidbody.aiActor)
                    {
                        EnemyInSights = true;
                        if (SynergyLaser)
                        {
                            raycastResult2.SpeculativeRigidbody.aiActor.ApplyEffect(DebuffStatics.tripleCrossbowSlowEffect, 1);
                            raycastResult2.SpeculativeRigidbody.aiActor.healthHaver.ApplyDamage(4 * BraveTime.DeltaTime, Vector2.zero, "Laser", CoreDamageTypes.Fire);
                            t4 += BraveTime.DeltaTime;
                            if (t4 >= 0.0625f)
                            {
                                t4 -= 0.0625f;
                                GlobalSparksDoer.DoRandomParticleBurst(1,
                                raycastResult2.SpeculativeRigidbody.aiActor.sprite.WorldCenter,
                                raycastResult2.SpeculativeRigidbody.aiActor.sprite.WorldCenter,
                                BraveUtility.RandomVector2(new Vector2(-1, -1), new Vector2(1,1)) * UnityEngine.Random.Range(12, 16),
                                2f,
                                1f,
                                0.125f,
                                0.25f,
                                GoRedirectMode ? Color.green * 3 : Color.red * 3,
                                GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                            }
                        }    
                    }
                    else
                    {
                        EnemyInSights = false;
                    }
                }
                laserPointer.dimensions = new Vector2((raycastResult2.Distance * 16), 1f);
                RaycastResult.Pool.Free(ref raycastResult2);

                //t = Mathf.MoveTowards(t, EnemyInSights ? 120 : 1, 5 * BraveTime.DeltaTime);
                t2 = Mathf.MoveTowards(t2, GoRedirectMode ? 0.025f : 1, 5 * BraveTime.DeltaTime);
                t3 = Mathf.MoveTowards(t3,  EnemyInSights ? (SynergyLaser ? 2.5f : 40) : 1, 80 * BraveTime.DeltaTime);
                if (EnemyInSights && SynergyMissiles)
                {
                    if (PlayerOwned)
                    {
                        t += BraveTime.DeltaTime;
                        if (t >= 0.75f)
                        {
                            t -= 0.75f - (MissilesFired * 0.25f);

                            MissilesFired++;
                            var a = Direction.ToAngle() + UnityEngine.Random.Range(-20, 21);
                            a *= play.stats.GetStatValue(PlayerStats.StatType.Accuracy);
                            var p = SpawnManager.SpawnProjectile(Guns.Yari_Launcher.DefaultModule.projectiles[0].gameObject, this.sprite.WorldCenter, Quaternion.Euler(0, 0, a)).GetComponent<Projectile>();
                            if (p)
                            {
                                AkSoundEngine.PostEvent("Play_BOSS_RatMech_Missile_01", play.gameObject);
                                p.Owner = play;
                                p.Shooter = play.specRigidbody;
                                p.baseData.damage = 6;
                                play.DoPostProcessProjectile(p);
                            }
                        }
                    }
                }
            }
        }

        private float MissilesFired = 0;

        public override void Move()
        {
            this.m_timeElapsed += this.LocalDeltaTime;
            if (this.angularVelocity != 0f)
            {
                this.m_transform.RotateAround(this.m_transform.position.XY(), Vector3.forward, this.angularVelocity * this.LocalDeltaTime);
            }
            if (this.baseData.UsesCustomAccelerationCurve)
            {
                float time = Mathf.Clamp01((this.m_timeElapsed - this.baseData.IgnoreAccelCurveTime) / this.baseData.CustomAccelerationCurveDuration);
                this.m_currentSpeed = this.baseData.AccelerationCurve.Evaluate(time) * this.baseData.speed;
            }
            base.specRigidbody.Velocity = this.m_currentDirection * this.m_currentSpeed * t3 * t2;

            this.m_currentSpeed *= 1f - this.baseData.damping * this.LocalDeltaTime;
            this.LastVelocity = base.specRigidbody.Velocity;
        }


        public void LateUpdate()
        {
            isHoldingTarget = false;
        }

        private AIActor AIActor;

        public Vector3 GetPointTowards(ref bool Controller, Vector3 vector2)
        {
            if ((this.Owner as AIActor))
            {
                return Direction;
            }

            if (!BraveInput.GetInstanceForPlayer((this.Owner as PlayerController).PlayerIDX).IsKeyboardAndMouse(false))
            {
                if (AIActor == null)
                {
                    AIActor = GetSimplifiedNewTarget();
                    if (AIActor == null)
                    {
                        Controller = true;
                        return this.Direction;
                    }
                    Controller = true;
                    return this.transform.position - AIActor.transform.position;
                }
                Controller = true;
                return this.transform.position - AIActor.transform.position;
            }
            else
            {
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.Owner as PlayerController).PlayerIDX);
                return SynergyRedirect ? (this.Owner as PlayerController).unadjustedAimPoint - this.transform.position : MathToolbox.GetUnitOnCircle3((this.Owner as PlayerController).m_currentGunAngle, 1);
            }
        }

        private AIActor GetSimplifiedNewTarget()
        {
            List<AIActor> enmL = new List<AIActor>();
            (this.Owner as PlayerController).CurrentRoom?.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref enmL);
            if (enmL == null) { return null; }
            if (enmL.Count == 0) { return null; }
            enmL.RemoveAll(self => self.State != AIActor.ActorState.Normal);
            enmL.RemoveAll(self => self.specRigidbody == null);
            enmL = enmL.OrderByDescending(self => self.healthHaver.currentHealth).ToList();
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
            return t;
        }



        public void SetRedirectMode(bool val)
        {
            GoRedirectMode = val;
            if (GoRedirectMode == true)
            {
                RedirectsDone++;
                if (RedirectsDone == 1)
                {
                    projectile.baseData.damage *= 3;
                }
                t2 = 0.025f;
                Color laser = Color.green;
                if (laserPointer == null)
                {
                    laserPointer = Instantiate(RandomPiecesOfStuffToInitialise.LaserReticle, this.transform.position, Quaternion.identity).GetComponent<tk2dTiledSprite>();
                    laserPointer.IsPerpendicular = false;
                    laserPointer.sprite.usesOverrideMaterial = true;
                    laserPointer.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                    laserPointer.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    laserPointer.sprite.renderer.material.SetFloat("_EmissivePower", 10);
                    laserPointer.sprite.renderer.material.SetFloat("_EmissiveColorPower", 3.5f);
                }

                laserPointer.sprite.renderer.material.SetColor("_OverrideColor", laser);
                laserPointer.sprite.renderer.material.SetColor("_EmissiveColor", laser);
                this.projectile.baseData.speed = 2.5f;
                this.UpdateSpeed();
                this.baseData.UsesCustomAccelerationCurve = false;
                this.ResetDistance();
                this.m_timeElapsed = 0;

                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = this.sprite.WorldCenter,
                    startSize = 5,
                    rotation = 0,
                    startLifetime = 0.333f,
                    startColor = Color.green.WithAlpha(0.333f)
                });
            }
            else
            {

                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = this.sprite.WorldCenter,
                    startSize = 5,
                    rotation = 0,
                    startLifetime = 0.333f,
                    startColor = Color.red.WithAlpha(0.333f)
                });
                Color laser = Color.red;
                laserPointer.sprite.renderer.material.SetColor("_OverrideColor", laser);
                laserPointer.sprite.renderer.material.SetColor("_EmissiveColor", laser);
            }
        }

        private int RedirectsDone = 0;
        private bool GoRedirectMode = false;

        private tk2dTiledSprite laserPointer;
	}
}

