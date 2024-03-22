
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

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


    public class UndodgeableProjectile : Projectile
    {
        public static BindingFlags AnyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        public static FieldInfo ProjectileHealthHaverHitCountInfo = typeof(Projectile).GetField("m_healthHaverHitCount", AnyBindingFlags);
        public static FieldInfo ProjectileHasPiercedInfo = typeof(Projectile).GetField("m_hasPierced", AnyBindingFlags);
        public static MethodInfo ProjectileHandleDelayedDamageInfo = typeof(Projectile).GetMethod("HandleDelayedDamage", AnyBindingFlags);

        public delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, out T4 arg4, T5 arg5, T6 arg6);
        public static HandleDamageResult HandleDamageHook(Func<Projectile, SpeculativeRigidbody, PixelCollider, bool, PlayerController, bool, HandleDamageResult> orig, Projectile self, SpeculativeRigidbody rigidbody, PixelCollider hitPixelCollider, out bool killedTarget,
            PlayerController player, bool alreadyPlayerDelayed)
        {
            if (self.GetType() == typeof(Projectile) && (self.Owner == null || !(self.Owner is PlayerController)) && self.gameObject.GetComponent<MarkForUndodgeAbleBullet>() != null && player != null)
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
    }
}