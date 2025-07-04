﻿using Alexandria.Misc;
using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside
{
    public abstract class CustomSlashData : ScriptableObject
    {
        public abstract CustomSlashData ReturnClone();


        public virtual void OnHitEnemy(GameActor gameActor, bool b, float Angle, Vector2 arcOrigin, Vector2 contact)
        {
        }

        public virtual void OnProjectileReflect(Projectile p, bool retargetReflectedBullet, GameActor newOwner, float minReflectedBulletSpeed, bool doPostProcessing = false, float scaleModifier = 1f, float baseDamage = 10f, float spread = 0f, string sfx = null)
        {
            p.RemoveBulletScriptControl();
            if (sfx != null)
            {
                AkSoundEngine.PostEvent(sfx, GameManager.Instance.gameObject);
            }

            if (retargetReflectedBullet && (bool)p.Owner && (bool)p.Owner.specRigidbody)
            {
                p.Direction = (p.Owner.specRigidbody.GetUnitCenter(ColliderType.HitBox) - p.specRigidbody.UnitCenter).normalized;
            }

            if (spread != 0f)
            {
                p.Direction = p.Direction.Rotate(UnityEngine.Random.Range(0f - spread, spread));
            }

            if ((bool)p.Owner && (bool)p.Owner.specRigidbody)
            {
                p.specRigidbody.DeregisterSpecificCollisionException(p.Owner.specRigidbody);
            }

            p.Owner = newOwner;
            p.SetNewShooter(newOwner.specRigidbody);
            p.allowSelfShooting = false;
            if (newOwner is AIActor)
            {
                p.collidesWithPlayer = true;
                p.collidesWithEnemies = false;
            }
            else if (newOwner is PlayerController)
            {
                p.collidesWithPlayer = false;
                p.collidesWithEnemies = true;
            }

            if (scaleModifier != 1f)
            {
                SpawnManager.PoolManager.Remove(p.transform);
                p.RuntimeUpdateScale(scaleModifier);
            }

            if (p.Speed < minReflectedBulletSpeed)
            {
                p.Speed = minReflectedBulletSpeed;
            }

            p.baseData.damage = baseDamage;
            if (doPostProcessing && doPostProcessing && newOwner is PlayerController)
            {
                PlayerController playerController = newOwner as PlayerController;
                if (playerController != null)
                {
                    p.baseData.damage *= playerController.stats.GetStatValue(PlayerStats.StatType.Damage);
                    p.baseData.speed *= playerController.stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                    p.UpdateSpeed();
                    p.baseData.force *= playerController.stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
                    p.baseData.range *= playerController.stats.GetStatValue(PlayerStats.StatType.RangeMultiplier);
                    p.BossDamageMultiplier *= playerController.stats.GetStatValue(PlayerStats.StatType.DamageToBosses);
                    p.RuntimeUpdateScale(playerController.stats.GetStatValue(PlayerStats.StatType.PlayerBulletScale));
                    playerController.DoPostProcessProjectile(p);
                }
            }

            if (newOwner is AIActor)
            {
                p.baseData.damage = 0.5f;
                p.baseData.SetAll((newOwner as AIActor).bulletBank.GetBullet().ProjectileData);
                p.specRigidbody.CollideWithTileMap = false;
                p.ResetDistance();
                p.collidesWithEnemies = (newOwner as AIActor).CanTargetEnemies;
                p.collidesWithPlayer = true;
                p.UpdateCollisionMask();
                p.sprite.color = new Color(1f, 0.1f, 0.1f);
                p.MakeLookLikeEnemyBullet();
                p.RemovePlayerOnlyModifiers();
                if ((newOwner as AIActor).IsBlackPhantom)
                {
                    p.baseData.damage = 1f;
                    p.BecomeBlackBullet();
                }
            }

            p.UpdateCollisionMask();
            p.Reflected();
            p.SendInDirection(p.Direction, resetDistance: true);
        }


        public bool doVFX = true;
        public VFXPool VFX = (PickupObjectDatabase.GetById(417) as Gun).muzzleFlashEffects;
        public bool doHitVFX = true;
        public VFXPool hitVFX = (PickupObjectDatabase.GetById(417) as Gun).DefaultModule.projectiles[0].hitEffects.enemy;
        public CustomSlashDoer.ProjInteractMode projInteractMode = CustomSlashDoer.ProjInteractMode.IGNORE;
        public float playerKnockbackForce = 5;
        public float enemyKnockbackForce = 10;
        public List<GameActorEffect> statusEffects = new List<GameActorEffect>();
        public float jammedDamageMult = 1;
        public float bossDamageMult = 1;
        public bool doOnSlash = true;
        public bool doPostProcessSlash = true;
        public float slashRange = 2.5f;
        public float slashDegrees = 90f;
        public float damage = 5f;
        public bool damagesBreakables = true;
        public string soundEvent = "Play_WPN_blasphemy_shot_01";
        public Action<GameActor, bool> OnHitTarget = null;
        public Action<Projectile> OnHitBullet = null;
        public Action<MinorBreakable> OnHitMinorBreakable = null;
        public Action<MajorBreakable> OnHitMajorBreakable = null;
        public Action<Projectile, GameActor> CustomReflectProjectile = null;
        public bool HitSecretRoomWalls = false;
    }

    public class BasicSlash : CustomSlashData
    {
        public override CustomSlashData ReturnClone()
        {

            BasicSlash newData = ScriptableObject.CreateInstance<BasicSlash>();
            newData.doVFX = this.doVFX;
            newData.VFX = this.VFX;
            newData.doHitVFX = this.doHitVFX;
            newData.hitVFX = this.hitVFX;
            newData.projInteractMode = this.projInteractMode;
            newData.playerKnockbackForce = this.playerKnockbackForce;
            newData.enemyKnockbackForce = this.enemyKnockbackForce;
            newData.statusEffects = this.statusEffects;
            newData.jammedDamageMult = this.jammedDamageMult;
            newData.bossDamageMult = this.bossDamageMult;
            newData.doOnSlash = this.doOnSlash;
            newData.doPostProcessSlash = this.doPostProcessSlash;
            newData.slashRange = this.slashRange;
            newData.slashDegrees = this.slashDegrees;
            newData.damage = this.damage;
            newData.damagesBreakables = this.damagesBreakables;
            newData.soundEvent = this.soundEvent;
            newData.OnHitTarget = this.OnHitTarget;
            newData.OnHitBullet = this.OnHitBullet;
            newData.OnHitMinorBreakable = this.OnHitMinorBreakable;
            newData.OnHitMajorBreakable = this.OnHitMajorBreakable;
            return newData;
        }
    }


    public class CustomSlashDoer
    {
        public static void DoSwordSlash(
            Vector2 position,
            float angle,
            GameActor owner,
            CustomSlashData slashParameters,
            Transform parentTransform = null)
        {
            if ((owner is PlayerController) && (owner as PlayerController).GetExtComp() != null && (owner as PlayerController).GetExtComp().PreProcessSlash != null)
            {
                //(owner as PlayerController).GetExtComp().PreProcessSlash((owner as PlayerController), position, slashParameters);
            }
            if (slashParameters.doVFX && slashParameters.VFX != null) slashParameters.VFX.SpawnAtPosition(position, angle, parentTransform, null, null, -0.05f);
            if (!string.IsNullOrEmpty(slashParameters.soundEvent) && owner != null && owner.gameObject != null) AkSoundEngine.PostEvent(slashParameters.soundEvent, owner.gameObject);
            GameManager.Instance.StartCoroutine(HandleSlash(position, angle, owner, slashParameters));
        }
        private static IEnumerator HandleSlash(Vector2 position, float angle, GameActor owner, CustomSlashData slashParameters)
        {
            int slashId = Time.frameCount;
            List<SpeculativeRigidbody> alreadyHit = new List<SpeculativeRigidbody>();
            if (slashParameters.playerKnockbackForce != 0f && owner != null) owner.knockbackDoer.ApplyKnockback(BraveMathCollege.DegreesToVector(angle, 1f), slashParameters.playerKnockbackForce, 0.25f, false);
            float ela = 0f;
            while (ela < 0.2f)
            {
                ela += BraveTime.DeltaTime;
                HandleHeroSwordSlash(alreadyHit, position, angle, slashId, owner, slashParameters);
                yield return null;
            }
            if ((owner is PlayerController) && (owner as PlayerController).GetExtComp() != null && (owner as PlayerController).GetExtComp().PostProcessSlash != null)
            {
                //(owner as PlayerController).GetExtComp().PostProcessSlash((owner as PlayerController), position, slashParameters);
            }
            yield break;
        }



        public enum ProjInteractMode
        {
            IGNORE,
            DESTROY,
            REFLECT,
            REFLECTANDPOSTPROCESS,
        }
        private static bool SlasherIsPlayerOrFriendly(GameActor slasher)
        {
            if (slasher is PlayerController) return true;
            if (slasher is AIActor)
            {
                if (slasher.GetComponent<CompanionController>()) return true;
                if (!slasher.aiActor.CanTargetPlayers && slasher.aiActor.CanTargetEnemies) return true;
            }
            return false;
        }
        private static bool ProjectileIsValid(Projectile proj, GameActor slashOwner)
        {
            if (proj)
            {
                if (slashOwner == null)
                {
                    return false;
                }
                if (SlasherIsPlayerOrFriendly(slashOwner))
                {
                    if ((proj.Owner && !(proj.Owner is PlayerController)) || proj.ForcePlayerBlankable) return true;
                }
                else if (slashOwner is AIActor)
                {
                    if (proj.Owner && proj.Owner is PlayerController) return true;
                }
                else
                {
                    if (proj.Owner) return true;
                }
            }

            return false;
        }
        private static bool ObjectWasHitBySlash(Vector2 ObjectPosition, Vector2 SlashPosition, float slashAngle, float SlashRange, float SlashDimensions)
        {
            if (Vector2.Distance(ObjectPosition, SlashPosition) < SlashRange)
            {
                float num7 = BraveMathCollege.Atan2Degrees(ObjectPosition - SlashPosition);
                float minRawAngle = Math.Min(SlashDimensions, -SlashDimensions);
                float maxRawAngle = Math.Max(SlashDimensions, -SlashDimensions);
                bool isInRange = false;
                float actualMaxAngle = slashAngle + maxRawAngle;
                float actualMinAngle = slashAngle + minRawAngle;

                if (num7.IsBetweenRange(actualMinAngle, actualMaxAngle)) isInRange = true;
                if (actualMaxAngle > 180)
                {
                    float Overflow = actualMaxAngle - 180;
                    if (num7.IsBetweenRange(-180, (-180 + Overflow))) isInRange = true;
                }
                if (actualMinAngle < -180)
                {
                    float Underflow = actualMinAngle + 180;
                    if (num7.IsBetweenRange((180 + Underflow), 180)) isInRange = true;
                }
                return isInRange;
            }
            return false;
        }
        private static void HandleHeroSwordSlash(List<SpeculativeRigidbody> alreadyHit, Vector2 arcOrigin, float slashAngle, int slashId, GameActor owner, CustomSlashData slashParameters)
        {
            float degreesOfSlash = slashParameters.slashDegrees;
            float slashRange = slashParameters.slashRange;

            ReadOnlyCollection<Projectile> allProjectiles2 = StaticReferenceManager.AllProjectiles;
            for (int j = allProjectiles2.Count - 1; j >= 0; j--)
            {
                Projectile projectile2 = allProjectiles2[j];
                if (ProjectileIsValid(projectile2, owner))
                {
                    Vector2 projectileCenter = projectile2.sprite.WorldCenter;
                    if (ObjectWasHitBySlash(projectileCenter, arcOrigin, slashAngle, slashRange, degreesOfSlash))
                    {
                        if (slashParameters.OnHitBullet != null) slashParameters.OnHitBullet(projectile2);
                        if (slashParameters.projInteractMode != ProjInteractMode.IGNORE || projectile2.collidesWithProjectiles)
                        {
                            if (slashParameters.projInteractMode == ProjInteractMode.DESTROY || slashParameters.projInteractMode == ProjInteractMode.IGNORE) projectile2.DieInAir(false, true, true, true);
                            else if (slashParameters.projInteractMode == ProjInteractMode.REFLECT || slashParameters.projInteractMode == ProjInteractMode.REFLECTANDPOSTPROCESS)
                            {
                                if (projectile2.Owner != null && projectile2.LastReflectedSlashId != slashId)
                                {
                                    slashParameters.OnProjectileReflect(projectile2, true, owner, 5, (slashParameters.projInteractMode == ProjInteractMode.REFLECTANDPOSTPROCESS), 1, 5, 0, null);
                                    projectile2.LastReflectedSlashId = slashId;
                                }
                            }
                        }
                    }
                }
            }
            DealDamageToEnemiesInArc(owner, arcOrigin, slashAngle, slashRange, slashParameters, alreadyHit);

            if (slashParameters.damagesBreakables)
            {
                List<MinorBreakable> allMinorBreakables = StaticReferenceManager.AllMinorBreakables;
                for (int k = allMinorBreakables.Count - 1; k >= 0; k--)
                {
                    MinorBreakable minorBreakable = allMinorBreakables[k];
                    if (minorBreakable && minorBreakable.specRigidbody)
                    {
                        if (!minorBreakable.IsBroken && minorBreakable.sprite)
                        {
                            if (ObjectWasHitBySlash(minorBreakable.sprite.WorldCenter, arcOrigin, slashAngle, slashRange, degreesOfSlash))
                            {
                                if (slashParameters.OnHitMinorBreakable != null) slashParameters.OnHitMinorBreakable(minorBreakable);
                                minorBreakable.Break();
                            }
                        }
                    }
                }
                List<MajorBreakable> allMajorBreakables = StaticReferenceManager.AllMajorBreakables;
                for (int l = allMajorBreakables.Count - 1; l >= 0; l--)
                {
                    MajorBreakable majorBreakable = allMajorBreakables[l];
                    if (majorBreakable && majorBreakable.specRigidbody)
                    {
                        if (!alreadyHit.Contains(majorBreakable.specRigidbody))
                        {               
                            if (slashParameters.HitSecretRoomWalls == false && majorBreakable.IsSecretDoor)
                            {
                                continue;
                            }
                            if (!majorBreakable.IsDestroyed)
                            {
                                if (ObjectWasHitBySlash(majorBreakable.specRigidbody.UnitCenter, arcOrigin, slashAngle, slashRange, degreesOfSlash))
                                {
                                    float num9 = slashParameters.damage;
                                    if (majorBreakable.healthHaver)
                                    {
                                        num9 *= 0.2f;
                                    }
                                    if (slashParameters.OnHitMajorBreakable != null) slashParameters.OnHitMajorBreakable(majorBreakable);
                                    majorBreakable.ApplyDamage(num9, majorBreakable.specRigidbody.UnitCenter - arcOrigin, false, false, false);
                                    alreadyHit.Add(majorBreakable.specRigidbody);
                                }
                            }
                        }
                    }
                }
            }
        }
        private static void DealDamageToEnemiesInArc(GameActor owner, Vector2 arcOrigin, float arcAngle, float arcRadius, CustomSlashData slashParameters, List<SpeculativeRigidbody> alreadyHit = null)
        {
            RoomHandler roomHandler = arcOrigin.GetAbsoluteRoom();
            if (roomHandler == null) return;
            if (SlasherIsPlayerOrFriendly(owner))
            {
                List<AIActor> activeEnemies = roomHandler.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (activeEnemies == null) return;

                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    AIActor aiactor = activeEnemies[i];
                    if (aiactor && aiactor.specRigidbody && aiactor.IsNormalEnemy && !aiactor.IsGone && aiactor.healthHaver)
                    {
                        if (alreadyHit == null || !alreadyHit.Contains(aiactor.specRigidbody))
                        {
                            for (int j = 0; j < aiactor.healthHaver.NumBodyRigidbodies; j++)
                            {
                                SpeculativeRigidbody bodyRigidbody = aiactor.healthHaver.GetBodyRigidbody(j);
                                PixelCollider hitboxPixelCollider = bodyRigidbody.HitboxPixelCollider;
                                if (hitboxPixelCollider != null)
                                {
                                    Vector2 vector = BraveMathCollege.ClosestPointOnRectangle(arcOrigin, hitboxPixelCollider.UnitBottomLeft, hitboxPixelCollider.UnitDimensions);
                                    if (ObjectWasHitBySlash(vector, arcOrigin, arcAngle, arcRadius, 90))
                                    {
                                        bool attackIsNotBlocked = true;
                                        int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable);
                                        RaycastResult raycastResult;
                                        if (PhysicsEngine.Instance.Raycast(arcOrigin, vector - arcOrigin, Vector2.Distance(vector, arcOrigin), out raycastResult, true, true, rayMask, null, false, null, null) && raycastResult.SpeculativeRigidbody != bodyRigidbody)
                                        {
                                            attackIsNotBlocked = false;
                                        }
                                        RaycastResult.Pool.Free(ref raycastResult);
                                        if (attackIsNotBlocked)
                                        {
                                            float damage = DealSwordDamageToEnemy(owner, aiactor, arcOrigin, vector, arcAngle, slashParameters);
                                            if (alreadyHit != null)
                                            {
                                                if (alreadyHit.Count == 0) StickyFrictionManager.Instance.RegisterSwordDamageStickyFriction(damage);
                                                alreadyHit.Add(aiactor.specRigidbody);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                List<PlayerController> AllPlayers = new List<PlayerController>();
                if (GameManager.Instance.PrimaryPlayer) AllPlayers.Add(GameManager.Instance.PrimaryPlayer);
                if (GameManager.Instance.SecondaryPlayer) AllPlayers.Add(GameManager.Instance.SecondaryPlayer);
                for (int i = 0; i < AllPlayers.Count; i++)
                {
                    PlayerController player = AllPlayers[i];
                    if (player && player.specRigidbody && player.healthHaver && !player.IsGhost)
                    {
                        if (alreadyHit == null || !alreadyHit.Contains(player.specRigidbody))
                        {
                            SpeculativeRigidbody bodyRigidbody = player.specRigidbody;
                            PixelCollider hitboxPixelCollider = bodyRigidbody.HitboxPixelCollider;
                            if (hitboxPixelCollider != null)
                            {
                                Vector2 vector = BraveMathCollege.ClosestPointOnRectangle(arcOrigin, hitboxPixelCollider.UnitBottomLeft, hitboxPixelCollider.UnitDimensions);
                                if (ObjectWasHitBySlash(vector, arcOrigin, arcAngle, arcRadius, 90))
                                {
                                    bool attackIsNotBlocked = true;
                                    int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable);
                                    RaycastResult raycastResult;
                                    if (PhysicsEngine.Instance.Raycast(arcOrigin, vector - arcOrigin, Vector2.Distance(vector, arcOrigin), out raycastResult, true, true, rayMask, null, false, null, null) && raycastResult.SpeculativeRigidbody != bodyRigidbody)
                                    {
                                        attackIsNotBlocked = false;
                                    }
                                    RaycastResult.Pool.Free(ref raycastResult);
                                    if (attackIsNotBlocked)
                                    {
                                        float damage = DealSwordDamageToEnemy(owner, player, arcOrigin, vector, arcAngle, slashParameters);
                                        if (alreadyHit != null)
                                        {
                                            if (alreadyHit.Count == 0) StickyFrictionManager.Instance.RegisterSwordDamageStickyFriction(damage);
                                            alreadyHit.Add(player.specRigidbody);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
        private static float DealSwordDamageToEnemy(GameActor owner, GameActor targetEnemy, Vector2 arcOrigin, Vector2 contact, float angle, CustomSlashData slashParameters)
        {
            if (targetEnemy.healthHaver)
            {
                float damageToDeal = slashParameters.damage;
                if (targetEnemy.healthHaver && targetEnemy.healthHaver.IsBoss) damageToDeal *= slashParameters.bossDamageMult;
                if ((targetEnemy is AIActor) && (targetEnemy as AIActor).IsBlackPhantom) damageToDeal *= slashParameters.jammedDamageMult;
                DamageCategory category = DamageCategory.Normal;
                if ((owner is AIActor) && (owner as AIActor).IsBlackPhantom) category = DamageCategory.BlackBullet;

                //VFX
                if (slashParameters.doHitVFX && slashParameters.hitVFX != null)
                {
                    slashParameters.hitVFX.SpawnAtPosition(new Vector3(contact.x, contact.y), 0, targetEnemy.transform);


                    bool wasAlivePreviously = targetEnemy.healthHaver.IsAlive;

                    targetEnemy.healthHaver.ApplyDamage(damageToDeal, contact - arcOrigin, owner.ActorName, CoreDamageTypes.None, category, false, null, false);
                    if ((owner is PlayerController) && (owner as PlayerController).GetExtComp() != null && (owner as PlayerController).GetExtComp().OnSlashHitEnemy != null && (targetEnemy is AIActor))
                    {
                        //(owner as PlayerController).GetExtComp().OnSlashHitEnemy((owner as PlayerController), arcOrigin, slashParameters, targetEnemy as AIActor);
                    }
                    bool fatal = false;
                    if (wasAlivePreviously && targetEnemy.healthHaver.IsDead) fatal = true;

                    if (slashParameters.OnHitTarget != null) slashParameters.OnHitTarget(targetEnemy, fatal);
                    slashParameters.OnHitEnemy(targetEnemy, fatal, angle, arcOrigin, contact);

                }
                if (targetEnemy.knockbackDoer)
                {
                    targetEnemy.knockbackDoer.ApplyKnockback(contact - arcOrigin, slashParameters.enemyKnockbackForce, false);
                }
                if (slashParameters.statusEffects != null && slashParameters.statusEffects.Count > 0)
                {
                    foreach (GameActorEffect effect in slashParameters.statusEffects)
                    {
                        targetEnemy.ApplyEffect(effect);
                    }
                }
            }
            return slashParameters.damage;
        }


    }
}
