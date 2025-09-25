using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AK.Wwise;
using Alexandria;
using Brave.BulletScript;
using Dungeonator;
using FullInspector;
using HarmonyLib;
using UnityEngine;
using static ETGMod;
using static Planetside.PrisonerSecondSubPhaseController;
using static UnityEngine.UI.GridLayoutGroup;


namespace Planetside
{

    [HarmonyPatch(typeof(GrappleModule), nameof(GrappleModule.OnPreCollision))]
    public class Patch_GrappleModule_ImpactedRigidbody
    {
        [HarmonyPrefix]
        private static bool Awake(GrappleModule __instance, SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
        {
            if (otherRigidbody != null)
            {
                var Chain = otherRigidbody.GetComponent<RevenantChainAttacher>();
                if (Chain != null)
                {

                    if (Chain.currentState == RevenantChainAttacher.State.Go)
                    {
                        var player = __instance.m_lastUser;
                        AkSoundEngine.PostEvent("Play_PARRY", __instance.m_lastUser.gameObject);
                        GameObject epicwin = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").GetComponent<BossFinalRogueDeathController>().DeathStarExplosionVFX, Chain.specRigidbody.UnitCenter, Quaternion.identity);
                        var enemy = Chain.revenantChainBehavior.m_aiActor;
                        __instance.m_impactedEnemy = enemy;
                        __instance.m_hasImpactedEnemy = true;
                        __instance.m_lastUser.gameObject.DoHitStop(1, () =>
                        {
                            Exploder.DoDistortionWave(Chain.specRigidbody.UnitCenter, 10f, 0.05f, 50, 0.25f);

                            AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", player.gameObject);
                            enemy.healthHaver.ApplyDamage(1000000, Vector2.zero, "Fart Attack");
                            Exploder.DoRadialPush(Chain.specRigidbody.UnitCenter, 250, 50);
                            Exploder.DoRadialKnockback(Chain.specRigidbody.UnitCenter, 250, 50);
                            Exploder.DoRadialMinorBreakableBreak(Chain.specRigidbody.UnitCenter, 50);
                            var tempExplosion = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);
                            tempExplosion.damage = 125;
                            tempExplosion.damageRadius = 50;
                            tempExplosion.ignoreList = new List<SpeculativeRigidbody>() { };
                            foreach (var player_ in GameManager.Instance.AllPlayers)
                            {
                                tempExplosion.ignoreList.Add(player_.specRigidbody);
                                player_.knockbackDoer.ApplyKnockback(player_.transform.position - Chain.transform.position, 50, 2.5f);
                            }
                            AkSoundEngine.PostEvent("Play_Hooray", player.gameObject);

                            Exploder.Explode(Chain.specRigidbody.UnitCenter, tempExplosion, Vector2.zero);
                        });
                        myRigidbody.Velocity = Vector2.zero;
                        return false;
                    }
                    PhysicsEngine.SkipCollision = true;
                }
            }
            return true;
        }
    }

    public class RevenantChainAttacher : BraveBehaviour
    {
        public RevenantChainBehavior revenantChainBehavior;
        public tk2dTiledSprite ChainTiles;
        public Vector2 Velocity;
        private float VelocityMult = 1;

        private float Timer = 0;

        private PlayerController playerController;
        private PlayerOrbital Orbital = null;


        public void Start()
        {
            AkSoundEngine.PostEvent("Play_Snipeilodon_Launch2", this.gameObject);
            AkSoundEngine.PostEvent("Play_OBJ_chainpot_drop_01", this.gameObject);
            AkSoundEngine.PostEvent("Play_WPN_saw_impact_01", this.gameObject);

            this.specRigidbody.OnPreTileCollision += (myRigidbody, myPixelCollider, tile, tilePixelCollider) =>
            {
                if (currentState == State.Go)
                {

                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = this.sprite.WorldCenter,
                        startSize = 8,
                        rotation = 0,
                        startLifetime = 0.25f,
                        startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                        angularVelocity = 0
                    });
                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 Launch = MathToolbox.GetUnitOnCircle(MathToolbox.ToAngle(Velocity) + UnityEngine.Random.Range(-22.5f, 22.5f), 1);
                        ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = this.sprite.WorldCenter,
                            rotation = 0,
                            startLifetime = UnityEngine.Random.Range(0.375f, 1.25f),
                            startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                            angularVelocity = 0,
                            velocity = Launch.normalized * UnityEngine.Random.Range(-2, -5),
                            startSize = 0.5f
                        });
                    }

                    
                    AkSoundEngine.PostEvent("Play_OBJ_hook_shot_01", this.gameObject);
                    AkSoundEngine.PostEvent("Play_WPN_saw_impact_01", this.gameObject);
                    AkSoundEngine.PostEvent("Play_ENM_shelleton_impact_02", this.gameObject);

                    currentState = State.Retract;
                    Timer = -1;
                    VelocityMult = 2f;
                    revenantChainBehavior.RecieveChainInformation(State.Retract);
                }
            };
            
            this.specRigidbody.OnPreRigidbodyCollision += (myBody, myPixelCollider, otherBody, otherPixelCollider) =>
            {
                var _otherBody = (otherBody as SpeculativeRigidbody);
                var _otherPixelCollider = (otherPixelCollider as PixelCollider);
                if (_otherBody.GetComponent<PlayerOrbital>() != null)
                {
                    if (currentState == State.Go)
                    {
                        Orbital = _otherBody.GetComponent<PlayerOrbital>();
                        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = this.sprite.WorldCenter,
                            startSize = 8,
                            rotation = 0,
                            startLifetime = 0.25f,
                            startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                            angularVelocity = 0
                        });
                        for (int i = 0; i < 16; i++)
                        {
                            Vector2 Launch = MathToolbox.GetUnitOnCircle(MathToolbox.ToAngle(Velocity) + UnityEngine.Random.Range(-22.5f, 22.5f), 1);
                            ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                            {
                                position = this.sprite.WorldCenter,
                                rotation = 0,
                                startLifetime = UnityEngine.Random.Range(0.375f, 1.25f),
                                startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                                angularVelocity = 0,
                                velocity = Launch.normalized * UnityEngine.Random.Range(-2, -5),
                                startSize = 0.5f
                            });
                        }
                        AkSoundEngine.PostEvent("Play_BOSS_lichC_morph_01", this.gameObject);
                        AkSoundEngine.PostEvent("Play_OBJ_hook_shot_01", this.gameObject);
                        AkSoundEngine.PostEvent("Play_WPN_saw_impact_01", this.gameObject);
                        AkSoundEngine.PostEvent("Play_ENM_shelleton_impact_02", this.gameObject);
                        currentState = State.Retract;
                        Timer = 0;
                        VelocityMult = 3f;
                        revenantChainBehavior.RecieveChainInformation(State.Retract);
                    }
                    return;
                }


                if (_otherBody.aiActor != null)
                {
                    PhysicsEngine.SkipCollision = true;
                    return;
                }
                if (_otherBody.gameActor != null && _otherBody.gameActor is PlayerController player)
                {

                    PhysicsEngine.SkipCollision = true;
                    if (player.IsEthereal || player.IsStealthed)
                    {
                        return;
                    }
                    if (currentState == State.Go)
                    {
                        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = player.sprite.WorldCenter,
                            startSize = 8,
                            rotation = 0,
                            startLifetime = 0.7f,
                            startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                            angularVelocity = 0
                        });
                        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = player.sprite.WorldCenter,
                            startSize = 8,
                            rotation = 0,
                            startLifetime = 0.4f,
                            startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                            angularVelocity = 0
                        });
                        for (int i = 0; i < 16; i++)
                        {

                            Vector2 Launch = MathToolbox.GetUnitOnCircle(MathToolbox.ToAngle(Velocity) + UnityEngine.Random.Range(-24, 24), 1);
                            ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                            {
                                position = player.sprite.WorldCenter,
                                rotation = 0,
                                startLifetime = UnityEngine.Random.Range(0.125f, 0.5f),
                                startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                                angularVelocity = 0,
                                velocity = Launch.normalized * UnityEngine.Random.Range(17, 25),
                                startSize = 1
                            });
                        }


                        currentState = State.Attach;
                        AkSoundEngine.PostEvent("Play_OBJ_hook_shot_01", this.gameObject);
                        AkSoundEngine.PostEvent("Play_WPN_saw_impact_01", this.gameObject);
                        AkSoundEngine.PostEvent("Play_ENM_shelleton_impact_02", this.gameObject);
                        AkSoundEngine.PostEvent("Play_WPN_saw_impact_01", this.gameObject);
                        AkSoundEngine.PostEvent("Play_ENM_shelleton_impact_02", this.gameObject);
                        AkSoundEngine.PostEvent("Play_Snipeilodon_Chain", this.gameObject);

                        playerController = player;
                        VelocityMult = 0;
                        revenantChainBehavior.RecieveChainInformation(State.Attach);
                        this.specRigidbody.RegisterSpecificCollisionException(player.specRigidbody);
                        Timer = -1;
                        Slot = playerController.knockbackDoer.ApplySourcedKnockback(Velocity, 1,10000000, this.gameObject, true);

                        playerController.OnReceivedDamage += PlayerWasDamaged;
                        playerController.OnPreDodgeRoll += PlayerDodgerolled;

                        return;
                    }
                }
                if (_otherBody.minorBreakable != null)
                {
                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = this.sprite.WorldCenter,
                        startSize = 2,
                        rotation = 0,
                        startLifetime = 0.25f,
                        startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                        angularVelocity = 0
                    });
                    AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Impact_01", this.gameObject);
                    PhysicsEngine.SkipCollision = true;
                    _otherBody.minorBreakable.Break();
                    return;
                }

                if (_otherBody.specRigidbody != null)
                {
                    if (currentState == State.Go)
                    {

                        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = this.sprite.WorldCenter,
                            startSize = 8,
                            rotation = 0,
                            startLifetime = 0.25f,
                            startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                            angularVelocity = 0
                        });
                        for (int i = 0; i < 16; i++)
                        {
                            Vector2 Launch = MathToolbox.GetUnitOnCircle(MathToolbox.ToAngle(Velocity) + UnityEngine.Random.Range(-22.5f, 22.5f), 1);
                            ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                            {
                                position = this.sprite.WorldCenter,
                                rotation = 0,
                                startLifetime = UnityEngine.Random.Range(0.375f, 1.25f),
                                startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                                angularVelocity = 0,
                                velocity = Launch.normalized * UnityEngine.Random.Range(-2, -5),
                                startSize = 0.5f
                            });
                        }
                        AkSoundEngine.PostEvent("Play_OBJ_hook_shot_01", this.gameObject);
                        AkSoundEngine.PostEvent("Play_WPN_saw_impact_01", this.gameObject);
                        AkSoundEngine.PostEvent("Play_ENM_shelleton_impact_02", this.gameObject);
                        currentState = State.Retract;
                        Timer = -1;
                        VelocityMult = 2f;
                        revenantChainBehavior.RecieveChainInformation(State.Retract);
                    }
                }
            };
        }

        private float ChainTimer = 24;


        private void PlayerWasDamaged(PlayerController obj)
        {
            HitThreshold--;
            ChainTimer -= 5f;
            if (HitThreshold == 0)
            {
                DetachFromPlayer();
            }
        }
        public int HitThreshold = 3;

        private void PlayerDodgerolled(PlayerController obj)
        {
            if (RollCooldown > 0) { return; }
            RollsTillBreak--;
            RollCooldown = 0.5f;
            ChainTimer -= 1.25f;

            AkSoundEngine.PostEvent("Play_WPN_egg_impact_01", this.gameObject);

            for (int i = 0; i < 3; i++)
            {

                Vector2 Launch = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 1);
                ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = obj.sprite.WorldCenter,
                    rotation = 0,
                    startLifetime = UnityEngine.Random.Range(0.375f, 0.625f),
                    startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                    angularVelocity = 0,
                    velocity = Launch.normalized * UnityEngine.Random.Range(12.5f - RollsTillBreak, 16f - RollsTillBreak),
                });
            }
            if (RollsTillBreak < 4)
            {
                AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Impact_02", this.gameObject);
            }
            if (RollsTillBreak == 0)
            {
                DetachFromPlayer();
            }
        }
        private int RollsTillBreak = 10;
        private float RollCooldown = 0;

        private ActiveKnockbackData Slot;

        private bool Event = false;

        public void Update()
        {
            if (RollCooldown > 0)
            {
                RollCooldown -= BraveTime.DeltaTime;
            }
            switch (currentState)
            {
                case State.Go:
                    this.specRigidbody.Velocity = Velocity * VelocityMult;
                    this.transform.localRotation = Quaternion.Euler(0f, 0f, MathToolbox.ToAngle(specRigidbody.Velocity));
                    Vector2 vector = revenantChainBehavior.ShootPoint.transform.position - this.transform.position;
                    float z = BraveMathCollege.Atan2Degrees(vector.normalized);
                    int num2 = Mathf.RoundToInt(vector.magnitude / 0.0625f);
                    ChainTiles.dimensions = new Vector2((float)num2, ChainTiles.dimensions.y);
                    ChainTiles.transform.rotation = Quaternion.Euler(0f, 0f, z);
                    ChainTiles.UpdateZDepth();


                    Vector2 Launch = MathToolbox.GetUnitOnCircle(MathToolbox.ToAngle(Velocity) + UnityEngine.Random.Range(-172.5f, -187.5f), 1);
                    ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = this.sprite.WorldCenter,
                        rotation = 0,
                        startLifetime = UnityEngine.Random.Range(0.25f, 0.5f),
                        startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                        angularVelocity = 0,
                        velocity = Launch.normalized * UnityEngine.Random.Range(11, 17),
                    });

                    break;
                case State.Retract:

                    this.transform.localRotation = Quaternion.Euler(0f, 0f, MathToolbox.ToAngle(this.transform.position - revenantChainBehavior.ShootPoint.transform.position));

                    Vector2 Launch_ = MathToolbox.GetUnitOnCircle(MathToolbox.ToAngle(Velocity) + UnityEngine.Random.Range(-7.5f, 7.5f), 1);


                    Vector2 vector_2 = revenantChainBehavior.ShootPoint.transform.position - this.transform.position;
                    var _s = Mathf.Min(1, Mathf.Max(0, Timer));

                    Velocity = (vector_2.normalized * 30 * _s) * -1;

                    if (Velocity.magnitude > 0.5)
                    {
                        ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = this.sprite.WorldCenter,
                            rotation = 0,
                            startLifetime = UnityEngine.Random.Range(0.75f, 1.25f),
                            startColor = new Color(1, 0.03f, 0).WithAlpha(1f),
                            angularVelocity = 0,
                            velocity = Launch_.normalized * UnityEngine.Random.Range(1, 3),
                        });
                    }

                    float z_2 = BraveMathCollege.Atan2Degrees(vector_2.normalized);
                    int num2_2 = Mathf.RoundToInt(vector_2.magnitude / 0.0625f);
                    ChainTiles.dimensions = new Vector2((float)num2_2, ChainTiles.dimensions.y);
                    ChainTiles.transform.rotation = Quaternion.Euler(0f, 0f, z_2);
                    ChainTiles.UpdateZDepth();
                    this.specRigidbody.Velocity = Velocity * VelocityMult;

                    if (Timer < 1) 
                    {
                        Timer += Time.deltaTime * 2;
                    }

                    VelocityMult = -1.5f;

                    if (vector_2.magnitude < 0.5f)
                    {
                        revenantChainBehavior.RecieveChainInformation(Orbital != null ? State.DestroyOrbital :  playerController != null ? State.Destroy : State.DestroyNoPlayer);

                        Destroy(this.gameObject);
                    }
                    break;
                case State.Attach:

                    if (ChainTimer > 0)
                    {
                        ChainTimer -= Time.deltaTime;
                    }
                    else
                    {
                        DetachFromPlayer();
                        return;
                    }
                    if (ChainTimer < 3 && Event == false)
                    {
                        Event = true;
                        AkSoundEngine.PostEvent("Play_ENM_shells_gather_01", this.gameObject);
                    }


                    if (playerController.IsFalling || playerController.m_dodgeRollState == PlayerController.DodgeRollState.Blink || playerController.IsEthereal)
                    {
                        DetachFromPlayer();
                        return;
                    }


                    if (Velocity.magnitude > 5f)
                    {
                        var s = Mathf.Min(1, Mathf.Max(0, Timer));
                        var maths = Velocity.normalized * Mathf.Min(8.75f, Mathf.Max(0, Mathf.Min(revenantChainBehavior.m_aiActor.IsBlackPhantom ? 4.5f : 3f, (Velocity.magnitude - 3.5f)) * s));
                        Slot.initialKnockback = maths;
                        Slot.knockback = maths;
                    }
                    else
                    {
                        Slot.initialKnockback = Vector2.zero;
                        Slot.knockback = Vector2.zero;
                    }


                    this.transform.position = playerController.sprite.WorldCenter;
                    Velocity = (revenantChainBehavior.ShootPoint.transform.position - this.transform.position);
                    this.transform.localRotation = Quaternion.Euler(0f, 0f, MathToolbox.ToAngle(Velocity) + 180);
                    this.specRigidbody.Velocity = Vector2.zero;

                    Vector2 vector_3 = revenantChainBehavior.ShootPoint.transform.position - this.transform.position;

                    float z_3 = BraveMathCollege.Atan2Degrees(vector_3.normalized);
                    int num2_3 = Mathf.RoundToInt(vector_3.magnitude / 0.0625f);
                    ChainTiles.dimensions = new Vector2((float)num2_3, ChainTiles.dimensions.y);
                    ChainTiles.transform.rotation = Quaternion.Euler(0f, 0f, z_3);
                    ChainTiles.UpdateZDepth();

                    if (Timer < 1)
                    {
                        Timer += Time.deltaTime;
                        return;
                    }
                    break;

            }
        }
        private void DetachFromPlayer()
        {
            AkSoundEngine.PostEvent("Stop_Snipeilodon_Chain", this.gameObject);
            this.specRigidbody.Reinitialize();
            Timer = -0.5f;
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = playerController.sprite.WorldCenter,
                startSize = 12,
                rotation = 0,
                startLifetime = 0.25f,
                startColor = new Color(1, 0.2f, 0).WithAlpha(1f),
                angularVelocity = 0
            });
            AkSoundEngine.PostEvent("Play_WPN_woodbeam_impact_01", this.gameObject);
            AkSoundEngine.PostEvent("Play_WPN_deck4rd_impact_01", this.gameObject);
            playerController.knockbackDoer.m_activeKnockbacks.Remove(Slot);
            this.currentState = State.Retract;
            revenantChainBehavior.RecieveChainInformation(State.Retract);
            VelocityMult = -3;
            playerController.OnReceivedDamage -= PlayerWasDamaged;
            playerController.OnPreDodgeRoll -= PlayerDodgerolled;
        }


        public override void OnDestroy()
        {
            AkSoundEngine.PostEvent("Stop_Snipeilodon_Chain", this.gameObject);
            revenantChainBehavior.RecieveChainInformation(State.Destroy);
            if (playerController)
            {
                playerController.knockbackDoer.m_activeKnockbacks.Remove(Slot);
                playerController.OnReceivedDamage -= PlayerWasDamaged;
                playerController.OnPreDodgeRoll -= PlayerDodgerolled;
            }
        }

        public State currentState = State.Go;

        public enum State
        {
            Go,
            Attach,
            Retract,
            Destroy,
            DestroyNoPlayer,
            DestroyOrbital
        }
    }


    public class RevenantChainBehavior : BasicAttackBehavior
    {

        private bool ShowImmobileDuringStop()
        {
            return this.StopDuring != RevenantChainBehavior.StopType.None;
        }

        private bool ShowChargeTime()
        {
            return !string.IsNullOrEmpty(this.ChargeAnimation);
        }

        private bool ShowOverrideFireDirection()
        {
            return this.ShouldOverrideFireDirection;
        }

        public bool IsBulletScript
        {
            get
            {
                return this.BulletScript_Shoot_When_Chain != null && !string.IsNullOrEmpty(this.BulletScript_Shoot_When_Chain.scriptTypeName) ||
                    this.BulletScript_Shoot_When_Missed != null && !string.IsNullOrEmpty(this.BulletScript_Shoot_When_Missed.scriptTypeName);
            }
        }



        public override void Start()
        {
            base.Start();
            if (this.SpecifyAiAnimator)
            {
                this.m_aiAnimator = this.SpecifyAiAnimator;
            }
            tk2dSpriteAnimator spriteAnimator = this.m_aiAnimator.spriteAnimator;
            spriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(spriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered));
            if (this.m_aiAnimator.ChildAnimator)
            {
                tk2dSpriteAnimator spriteAnimator2 = this.m_aiAnimator.ChildAnimator.spriteAnimator;
                spriteAnimator2.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(spriteAnimator2.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered));
            }
            this.m_aiActor.healthHaver.OnPreDeath += (_) =>
            {
                if(Inst != null)
                {
                    UnityEngine.Object.Destroy(Inst.gameObject);
                }
            };
        }

        public override void Upkeep()
        {
            base.Upkeep();
            if (this.state == RevenantChainBehavior.State.Charging)
            {
                base.DecrementTimer(ref this.m_chargeTimer, false);
            }
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
            {
                return behaviorResult;
            }
            if (!this.IsReady())
            {
                return BehaviorResult.Continue;
            }
            if (this.RequiresTarget && this.m_behaviorSpeculator.TargetRigidbody == null)
            {
                return BehaviorResult.Continue;
            }

            if (!this.m_gameObject.activeSelf)
            {
                this.m_gameObject.SetActive(true);
                this.m_beganInactive = true;
            }
            if (this.m_behaviorSpeculator.TargetRigidbody)
            {
                this.m_cachedTargetCenter = this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
            }

            this.state = RevenantChainBehavior.State.Idle;

            StopMoving();



            if (this.LockFacingDirection)
            {
                this.m_aiAnimator.FacingDirection = (this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
                this.m_aiAnimator.LockFacingDirection = true;
            }
            if (this.PreventTargetSwitching && this.m_aiActor)
            {
                this.m_aiActor.SuppressTargetSwitch = true;
            }
            this.m_updateEveryFrame = true;
            if (this.OverrideBaseAnims && this.m_aiAnimator)
            {
                if (!string.IsNullOrEmpty(this.OverrideIdleAnim))
                {
                    this.m_aiAnimator.OverrideIdleAnimation = this.OverrideIdleAnim;
                }
            }
            if (this.StopDuring == RevenantChainBehavior.StopType.None || this.StopDuring == RevenantChainBehavior.StopType.TellOnly)
            {
                return BehaviorResult.RunContinuousInClass;
            }
            return BehaviorResult.RunContinuous;
        }

        public virtual Vector2 GetPredictedTargetPosition(Vector2 targetCenter, Vector2 targetVelocity, Vector2? overridePos = null, float? overrideProjectileSpeed = null)
        {
            Vector2 aimOrigin = (overridePos == null) ? base.m_aiActor.specRigidbody.UnitCenter : overridePos.Value;
            float firingSpeed = overrideProjectileSpeed.Value;
            return BraveMathCollege.GetPredictedPosition(targetCenter, targetVelocity, aimOrigin, firingSpeed);
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            base.ContinuousUpdate();
            if (this.m_behaviorSpeculator.TargetRigidbody)
            {
                this.m_cachedTargetCenter = this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
            }
            if (this.state == State.Idle)
            {
                m_chargeTimer = ChargeTime;
                this.state = RevenantChainBehavior.State.Charging;
                if (!string.IsNullOrEmpty(this.ChargeAnimation))
                {
                    this.m_aiAnimator.PlayUntilFinished(this.ChargeAnimation, true, null, -1f, false);
                }
                ParticleBase.EmitParticles("WaveParticleInverse", 1, new ParticleSystem.EmitParams()
                {
                    position = ShootPoint.transform.position,
                    startSize = 16,
                    rotation = 0,
                    startLifetime = 1.25f,
                    startColor = new Color(1, 0.2f, 0).WithAlpha(0.333f),
                    angularVelocity = 0
                });
                UnityEngine.Object.Instantiate(StaticVFXStorage.EnemyZappyTellVFX, ShootPoint.transform);

                return ContinuousBehaviorResult.Continue;
            }

            if (this.state == RevenantChainBehavior.State.Charging)
            {
                m_chargeTimer -= BraveTime.DeltaTime;
                if (m_chargeTimer <= 0)
                {
                    if (this.MoveSpeedModifier != 1f)
                    {
                        this.m_cachedMovementSpeed = this.m_aiActor.MovementSpeed;
                        this.m_aiActor.MovementSpeed *= this.MoveSpeedModifier;
                    }
                    if (!string.IsNullOrEmpty(this.FireAnimation))
                    {
                        this.m_aiAnimator.PlayUntilFinished(this.FireAnimation, true, null, -1f, false);
                    }
                    this.state = RevenantChainBehavior.State.Shoot;
                    Inst = UnityEngine.Object.Instantiate(Chainprefab, ShootPoint.transform).GetComponent<RevenantChainAttacher>();
                    Inst.revenantChainBehavior = this;
                    Inst.specRigidbody.RegisterSpecificCollisionException(m_aiActor.specRigidbody);




                    Vector2 value = this.ShootPoint.transform.position;

                    Vector2 predictedTargetPosition = BraveMathCollege.GetPredictedPosition(m_cachedTargetCenter, this.m_behaviorSpeculator.TargetVelocity, 
                        ShootPoint.transform.position, 
                        50);
                    this.m_aiAnimator.LockFacingDirection = true;

                    Vector2 vector2 = predictedTargetPosition - this.ShootPoint.transform.position.XY();
                    var direction = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;

                    Inst.Velocity = MathToolbox.GetUnitOnCircle(direction,  50);

                    bool a = this.m_aiActor.spriteAnimator.CurrentClip.name.Contains("down");

                    Inst.ChainTiles.HeightOffGround = a ? 12 : 2;

                }
                return ContinuousBehaviorResult.Continue;
            }
            if (this.state == RevenantChainBehavior.State.Shoot)
            {
                if (Inst == null)
                {
                    EndContinuousUpdate();
                    return ContinuousBehaviorResult.Finished;
                }
                //Inst.specRigidbody.Velocity = new Vector2(1, 1);
                return ContinuousBehaviorResult.Continue;
            }

            if (this.state == RevenantChainBehavior.State.Retract)
            {
                if (Inst != null)
                {
                    return ContinuousBehaviorResult.Continue;
                }
                if (this.m_bulletSource != null && m_bulletSource.IsEnded  == false)
                {

                    return ContinuousBehaviorResult.Continue;
                }
                
            }

            /*
            if (this.state == RevenantChainBehavior.State.WaitingForCharge)
            {
                if ((this.ChargeTime > 0f && this.m_chargeTimer <= 0f) || (this.ChargeTime <= 0f && !this.m_aiAnimator.IsPlaying(this.ChargeAnimation)))
                {
                    if (!string.IsNullOrEmpty(this.TellAnimation))
                    {
                        this.m_aiAnimator.PlayUntilFinished(this.TellAnimation, true, null, -1f, false);
                        this.state = RevenantChainBehavior.State.WaitingForTell;
                    }
                    else
                    {
                        this.Fire();
                    }
                }
                return ContinuousBehaviorResult.Continue;
            }
            if (this.state == RevenantChainBehavior.State.WaitingForTell)
            {
                if (this.LockFacingDirection && this.ContinueAimingDuringTell && !this.m_isAimLocked && this.m_behaviorSpeculator.TargetRigidbody)
                {
                    this.m_aiAnimator.FacingDirection = (this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
                }
                if (!this.m_aiAnimator.IsPlaying(this.TellAnimation))
                {
                    this.Fire();
                }
                return ContinuousBehaviorResult.Continue;
            }
            if (this.state == RevenantChainBehavior.State.Firing)
            {
                if (!this.IsBulletScriptEnded)
                {
                    return ContinuousBehaviorResult.Continue;
                }
                tk2dSpriteAnimationClip.WrapMode wrapMode;
                if (!string.IsNullOrEmpty(this.TellAnimation) && this.m_aiAnimator.IsPlaying(this.TellAnimation) && this.m_aiAnimator.GetWrapType(this.TellAnimation, out wrapMode) && wrapMode == tk2dSpriteAnimationClip.WrapMode.Once)
                {
                    return ContinuousBehaviorResult.Continue;
                }
                if (!string.IsNullOrEmpty(this.FireAnimation) && this.m_aiAnimator.IsPlaying(this.FireAnimation) && this.m_aiAnimator.GetWrapType(this.FireAnimation, out wrapMode) && wrapMode == tk2dSpriteAnimationClip.WrapMode.Once)
                {
                    return ContinuousBehaviorResult.Continue;
                }
                if (!string.IsNullOrEmpty(this.PostFireAnimation))
                {
                    this.state = RevenantChainBehavior.State.WaitingForPostAnim;
                    this.m_aiAnimator.PlayUntilFinished(this.PostFireAnimation, false, null, -1f, false);
                    return ContinuousBehaviorResult.Continue;
                }
                return ContinuousBehaviorResult.Finished;
            }
            else
            {
                if (this.state == RevenantChainBehavior.State.WaitingForPostAnim)
                {
                    return (!this.m_aiAnimator.IsPlaying(this.PostFireAnimation)) ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
                }
            }
            */
            return ContinuousBehaviorResult.Finished;
        }


        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            //this.CeaseFire();

            ResetMoving();
            this.m_aiAnimator.LockFacingDirection = false;

            if (!string.IsNullOrEmpty(this.ChargeAnimation))
            {
                this.m_aiAnimator.EndAnimationIf(this.ChargeAnimation);
            }
            if (!string.IsNullOrEmpty(this.ChainedAnimation))
            {
                this.m_aiAnimator.EndAnimationIf(this.ChainedAnimation);
            }
            if (!string.IsNullOrEmpty(this.FireAnimation))
            {
                this.m_aiAnimator.EndAnimationIf(this.FireAnimation);
            }
            if (!string.IsNullOrEmpty(this.RetractAnimation))
            {
                this.m_aiAnimator.EndAnimationIf(this.RetractAnimation);
            }

            if (!string.IsNullOrEmpty(this.RetractAnimation))
            {
                this.m_aiAnimator.PlayUntilFinished(this.RetractAnimation, true, null, -1f, false);
            }

            if (this.m_beganInactive)
            {
                this.m_aiAnimator.gameObject.SetActive(false);
                this.m_beganInactive = false;
            }
            if (this.MoveSpeedModifier != 1f)
            {
                this.m_aiActor.MovementSpeed = this.m_cachedMovementSpeed;
            }

            if (this.LockFacingDirection)
            {
                this.m_aiAnimator.LockFacingDirection = false;
            }
            if (this.PreventTargetSwitching && this.m_aiActor)
            {
                this.m_aiActor.SuppressTargetSwitch = false;
            }
            if (this.OverrideBaseAnims && this.m_aiAnimator)
            {
                if (!string.IsNullOrEmpty(this.OverrideIdleAnim))
                {
                    this.m_aiAnimator.OverrideIdleAnimation = null;
                }
            }
            this.m_updateEveryFrame = false;
            this.state = RevenantChainBehavior.State.Idle;
            this.UpdateCooldowns();
        }

        public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
        {
            base.Init(gameObject, aiActor, aiShooter);
            this.m_specRigidbody = this.m_behaviorSpeculator.specRigidbody;
            this.m_bulletBank = this.m_behaviorSpeculator.bulletBank;
        }

        public override bool IsOverridable()
        {
            return !this.Uninterruptible;
        }
        /*

        private void Fire()
        {
            if (this.LockFacingDirection && this.ReaimOnFire && this.m_behaviorSpeculator.TargetRigidbody)
            {
                this.m_aiAnimator.FacingDirection = (this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
            }
            if (!string.IsNullOrEmpty(this.FireAnimation))
            {
                this.m_aiAnimator.EndAnimation();
                this.m_aiAnimator.PlayUntilFinished(this.FireAnimation, false, null, -1f, false);
            }


            this.SpawnProjectiles();

            if (this.StopDuring == RevenantChainBehavior.StopType.TellOnly)
            {
                this.m_behaviorSpeculator.PreventMovement = false;
                if (this.m_aiActor && this.ImmobileDuringStop)
                {
                    this.m_aiActor.knockbackDoer.SetImmobile(false, "ShootBulletScript");
                }
            }
            else if (this.StopDuring != RevenantChainBehavior.StopType.None)
            {
                this.StopMoving();
            }
            this.state = RevenantChainBehavior.State.Firing;
            if (this.HideGun && this.m_aiShooter)
            {
                this.m_aiShooter.ToggleGunAndHandRenderers(false, "ShootBulletScript");
            }
        }
        */

        private void CeaseFire()
        {
            if (this.IsBulletScript && this.m_bulletSource && !this.m_bulletSource.IsEnded)
            {
                this.m_bulletSource.ForceStop();
            }
        }

        private void StopMoving()
        {
            if (this.m_aiActor)
            {
                this.m_aiActor.ClearPath();
                this.m_behaviorSpeculator.PreventMovement = true;
                this.m_aiActor.knockbackDoer.SetImmobile(true, "ShootBulletScript");
            }
        }

        private void ResetMoving()
        {
            if (this.m_aiActor)
            {
                this.m_behaviorSpeculator.PreventMovement = false;
                this.m_aiActor.knockbackDoer.SetImmobile(false, "ShootBulletScript");
            }
        }

        public override Vector2 GetOrigin(ShootBehavior.TargetAreaOrigin origin)
        {
            if (origin == ShootBehavior.TargetAreaOrigin.ShootPoint)
            {
                return this.ShootPoint.transform.position.XY();
            }
            return base.GetOrigin(origin);
        }



        public void RecieveChainInformation(RevenantChainAttacher.State state)
        {
            switch (state) 
            {
                case RevenantChainAttacher.State.Retract:
                    if (m_bulletSource)
                    {
                        m_bulletSource.ForceStop();
                    }
                    this.state = State.Retract;
                    break;

                case RevenantChainAttacher.State.DestroyNoPlayer:
                    if (m_bulletSource)
                    {
                        m_bulletSource.ForceStop();
                    }
                    if (this.IsBulletScript && BulletScript_Shoot_When_Missed != null)
                    {

                        if (!this.m_bulletSource)
                        {

                            this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
                        }

                        this.m_bulletSource.BulletManager = this.m_bulletBank;
                        this.m_bulletSource.BulletScript = this.BulletScript_Shoot_When_Missed;
                        this.m_bulletSource.Initialize();
                        this.state = State.Retract;
                        return;
                    }
                    break;
                case RevenantChainAttacher.State.DestroyOrbital:
                    if (m_bulletSource)
                    {
                        m_bulletSource.ForceStop();
                    }
                    if (this.IsBulletScript && BulletScript_Shoot_Orbital != null)
                    {
                        if (!this.m_bulletSource)
                        {
                            this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
                        }
                        this.m_bulletSource.BulletManager = this.m_bulletBank;
                        this.m_bulletSource.BulletScript = this.BulletScript_Shoot_Orbital;
                        this.m_bulletSource.Initialize();
                        this.state = State.Retract;
                        return;
                    }
                    break;


                case RevenantChainAttacher.State.Attach:
                    if (m_bulletSource)
                    {
                        m_bulletSource.ForceStop();
                    }
                    if (this.IsBulletScript && BulletScript_Shoot_When_Chain != null)
                    {
                        if (!this.m_bulletSource)
                        {
                            this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
                        }
                        this.m_bulletSource.BulletManager = this.m_bulletBank;
                        this.m_bulletSource.BulletScript = this.BulletScript_Shoot_When_Chain;
                        this.m_bulletSource.Initialize();
                        return;
                    }
                    break;
            }
        }


        public bool IsBulletScriptEnded
        {
            get
            {
                if (this.IsBulletScript)
                {
                    return this.m_bulletSource.IsEnded;
                }
                return true;
            }
        }


        private RevenantChainBehavior.State state
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

        


        private void BeginState(RevenantChainBehavior.State state)
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Charging:
                    break;
                case State.Shoot:


                    break;
                case State.Chain:
                    break;
                case State.Retract:
                    break;
            }

            /*
            if (state == RevenantChainBehavior.State.WaitingForCharge)
            {
                if (this.StopDuring == RevenantChainBehavior.StopType.Charge)
                {
                    this.StopMoving();
                }
                this.m_chargeTimer = this.ChargeTime;
            }
            else if (state == RevenantChainBehavior.State.WaitingForTell)
            {
                if (this.StopDuring == RevenantChainBehavior.StopType.Tell || this.StopDuring == RevenantChainBehavior.StopType.TellOnly)
                {
                    this.StopMoving();
                }
                this.m_isAimLocked = false;
            }
            */
        }

        private void EndState(RevenantChainBehavior.State state)
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Charging:
                    break;
                case State.Shoot:
                    break;
                case State.Chain:
                    break;
                case State.Retract:
                    break;
            }
            /*
            if (state == RevenantChainBehavior.State.re)
            {

            }
            else if (state == RevenantChainBehavior.State.WaitingForTell)
            {
                if (this.OverrideBaseAnims)
                {
                    if (!string.IsNullOrEmpty(this.OverrideIdleAnim))
                    {
                        this.m_aiAnimator.OverrideIdleAnimation = this.OverrideIdleAnim;
                    }
                    if (!string.IsNullOrEmpty(this.TellAnimation))
                    {
                        this.m_aiAnimator.EndAnimationIf(this.TellAnimation);
                    }
                }
            }
            */
        }

        private void AnimEventTriggered(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frameNum)
        {
            /*
            tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
            bool flag = this.state == ShootBehavior.State.WaitingForTell;
            if (this.MultipleFireEvents)
            {
                flag |= (this.state == ShootBehavior.State.Firing);
            }
            if (flag && frame.eventInfo == "fire")
            {
                this.Fire();
            }
            if (this.LockFacingDirection && this.ContinueAimingDuringTell && frame.eventInfo == "stopAiming")
            {
                this.m_isAimLocked = true;
            }
            */
        }

        public GameObject Chainprefab;
        public RevenantChainAttacher Inst;


        public GameObject ShootPoint;

        public BulletScriptSelector BulletScript_Shoot_When_Chain;
        public BulletScriptSelector BulletScript_Shoot_When_Missed;
        public BulletScriptSelector BulletScript_Shoot_Orbital;



        public RevenantChainBehavior.StopType StopDuring;

        [InspectorShowIf("ShowImmobileDuringStop")]
        public bool ImmobileDuringStop;

        public float MoveSpeedModifier = 1f;

        public bool LockFacingDirection;

        [InspectorIndent]
        [InspectorShowIf("LockFacingDirection")]
        public bool ContinueAimingDuringTell;

        [InspectorIndent]
        [InspectorShowIf("LockFacingDirection")]
        public bool ReaimOnFire;


        public bool RequiresTarget = true;

        public bool PreventTargetSwitching;

        public bool Uninterruptible;


        [InspectorShowIf("ShowBulletName")]
        public bool ShouldOverrideFireDirection;

        [InspectorIndent]
        [InspectorShowIf("ShowOverrideFireDirection")]
        public float OverrideFireDirection;

        [InspectorCategory("Visuals")]
        public AIAnimator SpecifyAiAnimator;

        [InspectorCategory("Visuals")]
        public string ChargeAnimation;

        [InspectorCategory("Visuals")]
        [InspectorShowIf("ShowChargeTime")]
        public float ChargeTime = 1.5f;

        public string ChargeAnimatiom;
        public string FireAnimation;
        public string ChainedAnimation;
        public string RetractAnimation;



        [InspectorCategory("Visuals")]
        public bool OverrideBaseAnims;

        [InspectorShowIf("OverrideBaseAnims")]
        [InspectorIndent]
        [InspectorCategory("Visuals")]
        public string OverrideIdleAnim;

       



        private SpeculativeRigidbody m_specRigidbody;

        private AIBulletBank m_bulletBank;

        private BulletScriptSource m_bulletSource;

        private float m_chargeTimer;

        private bool m_beganInactive;

        private bool m_isAimLocked;

        private float m_cachedMovementSpeed;

        private Vector2 m_cachedTargetCenter;


        private RevenantChainBehavior.State m_state;

        public enum StopType
        {
            None,
            Tell,
            Attack,
            Charge,
            TellOnly
        }
        private enum State
        {
            Idle,
            Charging,
            Shoot,
            Chain,
            Retract
        }

        public enum TargetAreaOrigin
        {
            HitboxCenter,
            ShootPoint
        }      
    }
}
