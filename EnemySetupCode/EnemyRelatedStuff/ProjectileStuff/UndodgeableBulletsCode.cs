
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using static UnityEngine.ParticleSystem;
using Brave.BulletScript;
using Dungeonator;

namespace Planetside
{

    public class MarkForUndodgeAbleBullet : MonoBehaviour
    {
        public MarkForUndodgeAbleBullet()
        {
        }
        public void Start()
        {
            if (base.gameObject.GetComponent<Projectile>() != null)
            {
                var updateComp = base.gameObject.GetComponent<Projectile>();
                DepthLookupManager.AssignRendererToSortingLayer(updateComp.sprite.renderer, DepthLookupManager.GungeonSortingLayer.FOREGROUND);
                DepthLookupManager.UpdateRenderer(updateComp.sprite.renderer);
                DepthLookupManager.UpdateRendererWithWorldYPosition(updateComp.sprite.renderer, 18);


                updateComp.sprite.usesOverrideMaterial = true;
                if (updateComp.spriteAnimator != null)
                {
                    updateComp.spriteAnimator.sprite.usesOverrideMaterial = true;
                    updateComp.spriteAnimator.renderer.material.shader = PlanetsideModule.InverseGlowShader;
                    updateComp.spriteAnimator.renderer.material.SetFloat("_EmissiveColorPower", 3);
                }
                updateComp.sprite.renderer.material.shader = PlanetsideModule.InverseGlowShader;
                updateComp.sprite.renderer.material.SetFloat("_EmissiveColorPower", 3);
                updateComp.sprite.renderer.material.SetFloat("_EmissivePower", 4);

                if (updateComp.IsBlackBullet) { updateComp.sprite.renderer.material.SetFloat("_BlackBullet", -1); }
            }
        }
        public void ForceHurtPlayer(PlayerController p, Projectile projectile) 
        {
            float num = projectile.ModifiedDamage;

            HealthHaver healthHaver = p.healthHaver;
            float damage = num;
            Vector2 velocity = projectile.specRigidbody.Velocity;
            string ownerName = projectile.OwnerName;
            CoreDamageTypes coreDamageTypes = projectile.damageTypes;
            DamageCategory damageCategory = (!projectile.IsBlackBullet) ? DamageCategory.Normal : DamageCategory.BlackBullet;
            GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
            component.PlaceAtPositionByAnchor(p.transform.position + new Vector3(0.375f, 0.375f), tk2dBaseSprite.Anchor.MiddleCenter);
            component.HeightOffGround = 35f;
            component.UpdateZDepth();
            tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
            if (component2 != null)
            {
                component2.ignoreTimeScale = true;
                component2.AlwaysIgnoreTimeScale = true;
                component2.AnimateDuringBossIntros = true;
                component2.alwaysUpdateOffscreen = true;
                component2.playAutomatically = true;
            }
            healthHaver.ApplyDamage(damage, velocity, ownerName, coreDamageTypes, damageCategory, true, null, projectile.ignoreDamageCaps);
            if (p && p.OnHitByProjectile != null)
            {
                p.OnHitByProjectile(projectile, p);
            }
            if (projectile.Owner && projectile.Owner is AIActor && p)
            {
                AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", p.gameObject);
                (projectile.Owner as AIActor).HasDamagedPlayer = true;
            }
        }
     
    }

    public class MarkForUndodgeAbleBeam : MonoBehaviour { }


    public class ThirdDimensionalProjectile : Projectile
    {
        public override void Start()
        {
            base.Start();
            InitMaterial(); //For instancing purposes
        }
        private bool InitedMat = false;
        private void InitMaterial()
        {
            if (InitedMat) { return; }
            InitedMat = true;
            this.sprite.renderer.material = new Material(sprite.renderer.material); 
        }
        public void ForceHurtPlayer(PlayerController p, Projectile projectile)
        {
            float num = projectile.ModifiedDamage;

            HealthHaver healthHaver = p.healthHaver;
            float damage = num;
            Vector2 velocity = projectile.specRigidbody.Velocity;
            string ownerName = projectile.OwnerName;
            CoreDamageTypes coreDamageTypes = projectile.damageTypes;
            DamageCategory damageCategory = (!projectile.IsBlackBullet) ? DamageCategory.Normal : DamageCategory.BlackBullet;
            GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
            component.PlaceAtPositionByAnchor(p.transform.position + new Vector3(0.375f, 0.375f), tk2dBaseSprite.Anchor.MiddleCenter);
            component.HeightOffGround = 35f;
            component.UpdateZDepth();
            tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
            if (component2 != null)
            {
                component2.ignoreTimeScale = true;
                component2.AlwaysIgnoreTimeScale = true;
                component2.AnimateDuringBossIntros = true;
                component2.alwaysUpdateOffscreen = true;
                component2.playAutomatically = true;
            }
            healthHaver.ApplyDamage(damage, velocity, ownerName, coreDamageTypes, damageCategory, true, null, projectile.ignoreDamageCaps);
            if (p && p.OnHitByProjectile != null)
            {
                p.OnHitByProjectile(projectile, p);
            }
            if (projectile.Owner && projectile.Owner is AIActor && p)
            {
                AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", p.gameObject);
                (projectile.Owner as AIActor).HasDamagedPlayer = true;
            }
        }


        public static BindingFlags AnyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        public static FieldInfo ProjectileHealthHaverHitCountInfo = typeof(Projectile).GetField("m_healthHaverHitCount", AnyBindingFlags);
        public static FieldInfo ProjectileHasPiercedInfo = typeof(Projectile).GetField("m_hasPierced", AnyBindingFlags);
        public static MethodInfo ProjectileHandleDelayedDamageInfo = typeof(Projectile).GetMethod("HandleDelayedDamage", AnyBindingFlags);

        public float StartSpriteHeight = 0;
        public float MaxHeight = 10;
        public float CurrentHeight = 0;
        public float interpolatedHeight = 10;


        public bool IsUnDodgeable = true;
        private float CurrentDodgeState = 1;

        public void SetHeight(float Height)
        {
            CurrentHeight = MaxHeight * Height;
            
            this.sprite.HeightOffGround = (int)CurrentHeight;
            this.sprite.SortingOrder = -(int)CurrentHeight;
            interpolatedHeight = 0;
    
            if (CurrentHeight > 0.5f)
            {
                interpolatedHeight = CurrentHeight / (MaxHeight - 0.5f);
                this.sprite.gameObject.layer = LayerMask.NameToLayer("FG_Critical");
            }
            else if (CurrentHeight < -0.5f)
            {
                interpolatedHeight = CurrentHeight / (MaxHeight - 0.5f);
                this.sprite.gameObject.layer = LayerMask.NameToLayer("BG_Critical");

            }
            else
            {
                this.sprite.gameObject.layer = LayerMask.NameToLayer("Unpixelated");

            }


            this.sprite.renderer.material.SetFloat("_Height", interpolatedHeight);
            float m = 1 + (interpolatedHeight * 0.5f);
            this.gameObject.transform.localScale = new Vector3(m, m, m);
        }

        public void SetUnDodgeableState(bool isNowUndodgeable = true)
        {
            IsUnDodgeable = isNowUndodgeable;
            InitMaterial();
            this.sprite.renderer.material.SetFloat("_IsBlue", IsUnDodgeable ? 1 : 0);
        }


        private Vector3 LastHeightPosition;
        private float ParticleTick = 0.01f;
        private float _ParticleTick = 0f;

        public override void Update()
        {            
            //this.transform.position = LastHeightPosition;
            //this.specRigidbody.Reinitialize();
            base.Update();
            this.sprite.renderer.material.SetFloat("_IsBlue", IsUnDodgeable ? 1 : 0);
            LastHeightPosition = this.transform.position;
            if (CurrentHeight >= 0.5f && CurrentHeight <= 0.75f)
            {
                _ParticleTick += Time.deltaTime;
                if (_ParticleTick > ParticleTick)
                {
                    _ParticleTick = 0;
                    
                    //GlobalSparksDoer.DoSingleParticle(this.sprite.WorldCenter, Vector3.zero, sprite.GetBounds().size.x, 0.333f, Color.cyan * 3, GlobalSparksDoer.SparksType.SOLID_SPARKLES);
                    ParticleBase.EmitParticles("WaveParticle", 1, new EmitParams()
                    {
                        startSize = 4,
                        startLifetime = 0.33f,
                        startColor = IsUnDodgeable ? Color.cyan.WithAlpha(0.2f) : Color.red.WithAlpha(0.2f),
                    });
                }
            }
            if (CurrentHeight <= -0.5f && CurrentHeight >= -0.75f)
            {
                _ParticleTick += Time.deltaTime;
                if (_ParticleTick > ParticleTick)
                {
                    _ParticleTick = 0;
                    //GlobalSparksDoer.DoSingleParticle(this.sprite.WorldCenter, MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(1.5f, 1.75f)), 0.125f, 0.75f, Color.blue, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    //GlobalSparksDoer.DoSingleParticle(this.sprite.WorldCenter, Vector3.zero, sprite.GetBounds().size.x, 0.333f, Color.blue * 3, GlobalSparksDoer.SparksType.SOLID_SPARKLES);
                    ParticleBase.EmitParticles("WaveParticle", 1, new EmitParams()
                    {
                        startSize = 4,
                        startLifetime = 0.33f,
                        startColor = IsUnDodgeable ? Color.cyan.WithAlpha(0.2f) : Color.red.WithAlpha(0.2f),
                    });
                }
            }

            this.sprite.gameObject.transform.localPosition = new Vector3(0, CurrentHeight, 0);



            //this.transform.position += new Vector3(0, CurrentHeight, 0);
            //this.specRigidbody.Reinitialize();

            /*
            if (CurrentDodgeState != Dodge)
            {
                CurrentDodgeState = Mathf.MoveTowards(CurrentDodgeState, Dodge, 5 * Time.deltaTime);
            }
            */
        }


        public override void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
        {

            if (CurrentHeight > 0.5f | CurrentHeight < -0.5f)
            {
                PhysicsEngine.SkipCollision = true;
                return;

            }

            if (otherRigidbody == this.Shooter && !this.allowSelfShooting)
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
            if (IsUnDodgeable == false)
            {
                if (otherRigidbody.healthHaver != null && otherRigidbody.healthHaver.spriteAnimator != null && otherCollider.CollisionLayer == CollisionLayer.PlayerHitBox && otherRigidbody.spriteAnimator.QueryInvulnerabilityFrame())
                {
                    PhysicsEngine.SkipCollision = true;
                    base.StartCoroutine(this.HandlePostInvulnerabilityFrameExceptions(otherRigidbody));
                    return;
                }
            }


            if (this.collidesWithProjectiles && this.collidesOnlyWithPlayerProjectiles && otherRigidbody.projectile && !(otherRigidbody.projectile.Owner is PlayerController))
            {
                PhysicsEngine.SkipCollision = true;
                return;
            }
        }

        public override HandleDamageResult HandleDamage(SpeculativeRigidbody rigidbody, PixelCollider hitPixelCollider, out bool killedTarget, PlayerController player, bool alreadyPlayerDelayed = false)
        {

            if (IsUnDodgeable == true)
            {
                return base.HandleDamage(rigidbody, hitPixelCollider, out killedTarget, player, alreadyPlayerDelayed);
            }

            killedTarget = false;

            if (rigidbody.ReflectProjectiles)
            {
                return HandleDamageResult.NO_HEALTH;
            }
            if (!rigidbody.healthHaver)
            {
                return HandleDamageResult.NO_HEALTH;
            }
            if (!alreadyPlayerDelayed && s_delayPlayerDamage && player)
            {
                return HandleDamageResult.HEALTH;
            }
            if (!alreadyPlayerDelayed && s_delayPlayerDamage && player == null)
            {
                return HandleDamageResult.HEALTH;
            }

            bool flag = !rigidbody.healthHaver.IsDead;
            float num = this.ModifiedDamage;
            if (this.Owner is AIActor && rigidbody && rigidbody.aiActor && (this.Owner as AIActor).IsNormalEnemy)
            {
                num = ProjectileData.FixedFallbackDamageToEnemies;
                if (rigidbody.aiActor.HitByEnemyBullets)
                {
                    num /= 4f;
                }
            }
            int healthHaverHitCount = (int)ProjectileHealthHaverHitCountInfo.GetValue(this);
            if (this.Owner is PlayerController && (bool)ProjectileHasPiercedInfo.GetValue(this) && healthHaverHitCount >= 1)
            {
                int num2 = Mathf.Clamp(healthHaverHitCount - 1, 0, GameManager.Instance.PierceDamageScaling.Length - 1);
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
                Vector2 velocity = this.specRigidbody.Velocity;
                string ownerName = this.OwnerName;
                CoreDamageTypes coreDamageTypes = this.damageTypes;
                DamageCategory damageCategory = (!this.IsBlackBullet) ? DamageCategory.Normal : DamageCategory.BlackBullet;
                if (IsUnDodgeable)
                {
                    AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", player.gameObject);
                    GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
                    tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
                    component.PlaceAtPositionByAnchor(player.transform.position + new Vector3(0.375f, 0.375f), tk2dBaseSprite.Anchor.MiddleCenter);
                    component.HeightOffGround = 35f;
                    component.UpdateZDepth();
                    tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                    if (component2 != null)
                    {
                        component2.ignoreTimeScale = true;
                        component2.AlwaysIgnoreTimeScale = true;
                        component2.AnimateDuringBossIntros = true;
                        component2.alwaysUpdateOffscreen = true;
                        component2.playAutomatically = true;
                    }
                }
                
                healthHaver.ApplyDamage(damage, velocity, ownerName, coreDamageTypes, damageCategory, true, hitPixelCollider, this.ignoreDamageCaps);
                if (player && player.OnHitByProjectile != null)
                {
                    player.OnHitByProjectile(this, player);
                }
            }
            else
            {
                rigidbody.StartCoroutine((IEnumerator)ProjectileHandleDelayedDamageInfo.Invoke(this, new object[] { rigidbody, num, this.specRigidbody.Velocity, hitPixelCollider }));
            }
            if (this.Owner && this.Owner is AIActor && player)
            {
                AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", player.gameObject);
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
                if (this && UnityEngine.Random.value < this.StunApplyChance && rigidbody.gameActor.behaviorSpeculator)
                {
                    rigidbody.gameActor.behaviorSpeculator.Stun(this.AppliedStunDuration, true);
                }
                for (int i = 0; i < this.statusEffectsToApply.Count; i++)
                {
                    rigidbody.gameActor.ApplyEffect(this.statusEffectsToApply[i], 1f, null);
                }
            }
            ProjectileHealthHaverHitCountInfo.SetValue(this, healthHaverHitCount + 1);
            return (!killedTarget) ? HandleDamageResult.HEALTH : HandleDamageResult.HEALTH_AND_KILLED;

            //return base.HandleDamage(rigidbody, hitPixelCollider, out killedTarget, player, alreadyPlayerDelayed);
        }


        public delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, out T4 arg4, T5 arg5, T6 arg6);
        public static HandleDamageResult HandleDamageHook(Func<Projectile, SpeculativeRigidbody, PixelCollider, bool, PlayerController, bool, HandleDamageResult> orig, Projectile self, SpeculativeRigidbody rigidbody, PixelCollider hitPixelCollider, out bool killedTarget,
            PlayerController player, bool alreadyPlayerDelayed)
        {
            bool THREE_DEE = (self is ThirdDimensionalProjectile thirdDimensionBullet | self.GetComponent<MarkForUndodgeAbleBullet>() != null);
            if (!(self.Owner is PlayerController) && THREE_DEE == true && player != null)
            {
                killedTarget = false;
                if (rigidbody.ReflectProjectiles)
                {
                    return HandleDamageResult.NO_HEALTH;
                }
                if (!rigidbody.healthHaver)
                {
                    return HandleDamageResult.NO_HEALTH;
                }
                if (!alreadyPlayerDelayed && s_delayPlayerDamage && player)
                {
                    return HandleDamageResult.HEALTH;
                }
                if (!alreadyPlayerDelayed && s_delayPlayerDamage && player == null)
                {
                    return HandleDamageResult.HEALTH;
                }

                bool flag = !rigidbody.healthHaver.IsDead;
                float num = self.ModifiedDamage;
                if (self.Owner is AIActor && rigidbody && rigidbody.aiActor && (self.Owner as AIActor).IsNormalEnemy)
                {
                    num = ProjectileData.FixedFallbackDamageToEnemies;
                    if (rigidbody.aiActor.HitByEnemyBullets)
                    {
                        num /= 4f;
                    }
                }
                int healthHaverHitCount = (int)ProjectileHealthHaverHitCountInfo.GetValue(self);
                if (self.Owner is PlayerController && (bool)ProjectileHasPiercedInfo.GetValue(self) && healthHaverHitCount >= 1)
                {
                    int num2 = Mathf.Clamp(healthHaverHitCount - 1, 0, GameManager.Instance.PierceDamageScaling.Length - 1);
                    num *= GameManager.Instance.PierceDamageScaling[num2];
                }
                if (self.OnWillKillEnemy != null && num >= rigidbody.healthHaver.GetCurrentHealth())
                {
                    self.OnWillKillEnemy(self, rigidbody);
                }
                if (rigidbody.healthHaver.IsBoss)
                {
                    num *= self.BossDamageMultiplier;
                }
                if (self.BlackPhantomDamageMultiplier != 1f && rigidbody.aiActor && rigidbody.aiActor.IsBlackPhantom)
                {
                    num *= self.BlackPhantomDamageMultiplier;
                }
                bool flag2 = false;
                if (self.DelayedDamageToExploders)
                {
                    flag2 = (rigidbody.GetComponent<ExplodeOnDeath>() && rigidbody.healthHaver.GetCurrentHealth() <= num);
                }

                if (!flag2)
                {
                    HealthHaver healthHaver = rigidbody.healthHaver;
                    float damage = num;
                    Vector2 velocity = self.specRigidbody.Velocity;
                    string ownerName = self.OwnerName;
                    CoreDamageTypes coreDamageTypes = self.damageTypes;
                    DamageCategory damageCategory = (!self.IsBlackBullet) ? DamageCategory.Normal : DamageCategory.BlackBullet;
                    AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", player.gameObject);
                    GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
                    tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
                    component.PlaceAtPositionByAnchor(player.transform.position+ new Vector3(0.375f, 0.375f), tk2dBaseSprite.Anchor.MiddleCenter);
                    component.HeightOffGround = 35f;
                    component.UpdateZDepth();
                    tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                    if (component2 != null)
                    {
                        component2.ignoreTimeScale = true;
                        component2.AlwaysIgnoreTimeScale = true;
                        component2.AnimateDuringBossIntros = true;
                        component2.alwaysUpdateOffscreen = true;
                        component2.playAutomatically = true;
                    }
                    healthHaver.ApplyDamage(damage, velocity, ownerName, coreDamageTypes, damageCategory, true, hitPixelCollider, self.ignoreDamageCaps);
                    if (player && player.OnHitByProjectile != null)
                    {
                        player.OnHitByProjectile(self, player);
                    }
                }
                else
                {
                    rigidbody.StartCoroutine((IEnumerator)ProjectileHandleDelayedDamageInfo.Invoke(self, new object[] { rigidbody, num, self.specRigidbody.Velocity, hitPixelCollider }));
                }
                if (self.Owner && self.Owner is AIActor && player)
                {
                    AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", player.gameObject);
                    (self.Owner as AIActor).HasDamagedPlayer = true;
                }
                killedTarget = (flag && rigidbody.healthHaver.IsDead);
                if (!killedTarget && rigidbody.gameActor != null)
                {
                    if (self.AppliesPoison && UnityEngine.Random.value < self.PoisonApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(self.healthEffect, 1f, null);
                    }
                    if (self.AppliesSpeedModifier && UnityEngine.Random.value < self.SpeedApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(self.speedEffect, 1f, null);
                    }
                    if (self.AppliesCharm && UnityEngine.Random.value < self.CharmApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(self.charmEffect, 1f, null);
                    }
                    if (self.AppliesFreeze && UnityEngine.Random.value < self.FreezeApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(self.freezeEffect, 1f, null);
                    }
                    if (self.AppliesCheese && UnityEngine.Random.value < self.CheeseApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(self.cheeseEffect, 1f, null);
                    }
                    if (self.AppliesBleed && UnityEngine.Random.value < self.BleedApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(self.bleedEffect, -1f, self);
                    }
                    if (self.AppliesFire && UnityEngine.Random.value < self.FireApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(self.fireEffect, 1f, null);
                    }
                    if (self && UnityEngine.Random.value < self.StunApplyChance && rigidbody.gameActor.behaviorSpeculator)
                    {
                        rigidbody.gameActor.behaviorSpeculator.Stun(self.AppliedStunDuration, true);
                    }
                    for (int i = 0; i < self.statusEffectsToApply.Count; i++)
                    {
                        rigidbody.gameActor.ApplyEffect(self.statusEffectsToApply[i], 1f, null);
                    }
                }
                ProjectileHealthHaverHitCountInfo.SetValue(self, healthHaverHitCount + 1);
                return (!killedTarget) ? HandleDamageResult.HEALTH : HandleDamageResult.HEALTH_AND_KILLED;
            }
            else
            {
                return orig(self, rigidbody, hitPixelCollider, out killedTarget, player, alreadyPlayerDelayed);
            }
        }

        public override void HandleDestruction(CollisionData lcr, bool allowActorSpawns = true, bool allowProjectileSpawns = true)
        {
            this.sprite.renderer.material.SetFloat("_IsBlue", IsUnDodgeable ? 1 : 0);
            base.HandleDestruction(lcr, allowActorSpawns, allowProjectileSpawns);
        }
    }
}