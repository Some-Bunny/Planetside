using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Gungeon;
using System.Collections;

using ItemAPI;
using Dungeonator;
using System.Collections.ObjectModel;

namespace Planetside

{
    public class ProjectileSlashingBehaviour : MonoBehaviour
    {
        public ProjectileSlashingBehaviour()
        {
            DestroyBaseAfterFirstSlash = true;
            timeBetweenSlashes = 1;
            DoSound = true;
            slashKnockback = 5;
            SlashDamage = 15;
            SlashBossMult = 1;
            SlashJammedMult = 1;
            playerKnockback = 1;
            SlashDamageUsesBaseProjectileDamage = true;
            InteractMode = SlashDoer.ProjInteractMode.IGNORE;
            SlashDimensions = 90;
            SlashRange = 2.5f;
            SlashVFX = (ETGMod.Databases.Items["wonderboy"] as Gun).muzzleFlashEffects;
            soundToPlay = "Play_WPN_blasphemy_shot_01";
            DoesMultipleSlashes = false;
            UsesAngleVariance = false;
            MinSlashAngleOffset = 1;
            MaxSlashAngleOffset = 4;
            delayBeforeSlash = 0;
        }
        private void Start()
        {
            this.m_projectile = base.GetComponent<Projectile>();
            if (this.m_projectile.Owner && this.m_projectile.Owner is PlayerController) this.owner = this.m_projectile.Owner as PlayerController;
            this.m_projectile.sprite.renderer.enabled = false;
            if (this.m_projectile)
            {
                if (doSpinAttack)
                {
                    DestroyBaseAfterFirstSlash = false;
                    this.m_projectile.StartCoroutine(DoSlash(90, 0.15f + delayBeforeSlash));
                    this.m_projectile.StartCoroutine(DoSlash(180, 0.30f + delayBeforeSlash));
                    this.m_projectile.StartCoroutine(DoSlash(-90, 0.45f + delayBeforeSlash));
                    Invoke("Suicide", 0.01f);
                }
                else if (DoesMultipleSlashes)
                {
                    this.m_projectile.StartCoroutine(DoMultiSlash(0, delayBeforeSlash, AmountOfMultiSlashes, DelayBetweenMultiSlashes));
                }
                else
                {
                    this.m_projectile.StartCoroutine(DoSlash(0, 0 + delayBeforeSlash));
                }
            }
        }
        private void Update()
        {


        }
        private IEnumerator DoSlash(float angle, float delay)
        {
            yield return new WaitForSeconds(delay);
            float actDamage = this.SlashDamage;
            float actKnockback = this.slashKnockback;
            float bossDMGMult = this.SlashBossMult;
            float jammedDMGMult = this.SlashJammedMult;

            if (SlashDamageUsesBaseProjectileDamage)
            {
                actDamage = this.m_projectile.baseData.damage;
                bossDMGMult = this.m_projectile.BossDamageMultiplier;
                jammedDMGMult = this.m_projectile.BlackPhantomDamageMultiplier;
                actKnockback = this.m_projectile.baseData.force;
            }
            if (UsesAngleVariance)
            {
                angle += UnityEngine.Random.Range(MinSlashAngleOffset, MaxSlashAngleOffset);
            }
            SlashDoer.DoSwordSlash(this.m_projectile.specRigidbody.UnitCenter, (this.m_projectile.Direction.ToAngle() + angle), owner, playerKnockback, this.InteractMode, actDamage, actKnockback, this.m_projectile.statusEffectsToApply, null, jammedDMGMult, bossDMGMult, SlashRange, SlashDimensions);
            if (DoSound) AkSoundEngine.PostEvent(soundToPlay, this.m_projectile.gameObject);
            SlashVFX.SpawnAtPosition(this.m_projectile.specRigidbody.UnitCenter, this.m_projectile.Direction.ToAngle() + angle, null, null, null, -0.05f);
            if (DestroyBaseAfterFirstSlash) Suicide();
            yield break;
        }
        private IEnumerator DoMultiSlash(float angle, float delay, int AmountOfMultiSlashes, float DelayBetweenMultiSlashes)
        {
            yield return new WaitForSeconds(delay);
            float actDamage = this.SlashDamage;
            float actKnockback = this.slashKnockback;
            float bossDMGMult = this.SlashBossMult;
            float jammedDMGMult = this.SlashJammedMult;
            for (int i = 0; i < AmountOfMultiSlashes; i++)
            {
                if (SlashDamageUsesBaseProjectileDamage)
                {
                    actDamage = this.m_projectile.baseData.damage;
                    bossDMGMult = this.m_projectile.BossDamageMultiplier;
                    jammedDMGMult = this.m_projectile.BlackPhantomDamageMultiplier;
                    actKnockback = this.m_projectile.baseData.force;
                }
                if (UsesAngleVariance)
                {
                    angle += UnityEngine.Random.Range(MinSlashAngleOffset, MaxSlashAngleOffset);
                }
                SlashDoer.DoSwordSlash(this.m_projectile.specRigidbody.UnitCenter, (this.m_projectile.Direction.ToAngle() + angle), owner, playerKnockback, this.InteractMode, actDamage, actKnockback, this.m_projectile.statusEffectsToApply, null, jammedDMGMult, bossDMGMult, SlashRange, SlashDimensions);
                if (DoSound) AkSoundEngine.PostEvent(soundToPlay, this.m_projectile.gameObject);
                SlashVFX.SpawnAtPosition(this.m_projectile.specRigidbody.UnitCenter, this.m_projectile.Direction.ToAngle() + angle, null, null, null, -0.05f);
                yield return new WaitForSeconds(DelayBetweenMultiSlashes);
            }
            Suicide();
            yield break;
        }
        private void Suicide() { UnityEngine.Object.Destroy(this.m_projectile.gameObject); }
        //private float timer;
        /// <summary>
        /// The sound that will play when the slash goes off. You can set this to be a custom sound. If not set, it will use Blasphemy's instead. If you are using this, make sure to have the weapon's PreventNormalFireAudio to false! 
        /// </summary>
        public string soundToPlay;
        /// <summary>
        /// The delay before the first slash
        /// </summary>
        public float delayBeforeSlash;
        /// <summary>
        /// The VFX of a slash. Use CreateMuzzleFlash in VFXLibrary to make a custom one. Uses Blasphemy's if not set. If you make a custom slash vfx, place the sprites in a folder called VFXCollection, inside of your sprites folder (the one that has weapon sprites)
        /// </summary>
        public VFXPool SlashVFX;
        /// <summary>
        /// If you want to do something like Katana Bullets I guess
        /// </summary>
        public float timeBetweenSlashes;
        /// <summary>
        /// If the weapon does a spin attack
        /// </summary>
        public bool doSpinAttack;
        /// <summary>
        /// Don't know what this does. Best to not change it
        /// </summary>
        public float playerKnockback;
        /// <summary>
        /// Knockback of the slash. Doesn't need to be set if SlashDamageUsesBaseProjectileDamage is true
        /// </summary>
        public float slashKnockback;
        /// <summary>
        /// If there is a sound when the slash goes off.
        /// </summary>
        public bool DoSound;
        /// <summary>
        /// The jammed damage multiplier of the slash. Doesn't need to be set if SlashDamageUsesBaseProjectileDamage is true
        /// </summary>
        public float SlashJammedMult;
        /// <summary>
        /// The boss damage multiplier of the slash. Doesn't need to be set if SlashDamageUsesBaseProjectileDamage is true
        /// </summary>
        public float SlashBossMult;
        /// <summary>
        /// The damage of the slash. Doesn't need to be set if SlashDamageUsesBaseProjectileDamage is true
        /// </summary>
        public float SlashDamage;
        /// <summary>
        /// How far a slash will damage the enemies from the point of it being spawned
        /// </summary>
        public float SlashRange;
        /// <summary>
        /// The angle width of a slash
        /// </summary>
        public float SlashDimensions;
        /// <summary>
        /// If the slash uses the base stats of the projectile it is attached to. Best left true
        /// </summary>
        public bool SlashDamageUsesBaseProjectileDamage;
        /// <summary>
        /// How the slash interacts with enemy projectiles
        /// </summary>
        public SlashDoer.ProjInteractMode InteractMode;
        /// <summary>
        /// If the project is destroyed after the first slash or not. Is automatically set to false if DoesMultipleSlashes or doSpinAttack is set to true
        /// </summary>
        public bool DestroyBaseAfterFirstSlash;
        /// <summary>
        /// Allows the weapon to do a burst of multiple slashes
        /// </summary>
        public bool DoesMultipleSlashes;
        /// <summary>
        /// The minimum angle offset a slash can have
        /// </summary>
        public float MinSlashAngleOffset;
        /// <summary>
        /// The maximum angle offset a slash can have
        /// </summary>
        public float MaxSlashAngleOffset;
        /// <summary>
        /// If the slash will have an angle offset from the aim of the player
        /// </summary>
        public bool UsesAngleVariance;
        /// <summary>
        /// Amount of slashes in a burst
        /// </summary>
        public int AmountOfMultiSlashes;
        /// <summary>
        /// Determines how much time is between each slash in a burst
        /// </summary>
        public float DelayBetweenMultiSlashes;
        private Projectile m_projectile;
        private PlayerController owner;
    }
}


namespace Planetside
{
    public class SlashDoer
    {

        public static void DoSwordSlash(Vector2 position, float angle, PlayerController owner, float playerKnockbackForce, ProjInteractMode intmode, float damageToDeal, float enemyKnockbackForce, List<GameActorEffect> statusEffects, Transform parentTransform = null, float jammedDamageMult = 1, float bossDamageMult = 1, float SlashRange = 2.5f, float SlashDimensions = 90f)
        {
            GameManager.Instance.StartCoroutine(HandleSlash(position, angle, owner, playerKnockbackForce, intmode, damageToDeal, enemyKnockbackForce, statusEffects, jammedDamageMult, bossDamageMult, SlashRange, SlashDimensions));
        }


        private static IEnumerator HandleSlash(Vector2 position, float angle, PlayerController owner, float knockbackForce, ProjInteractMode intmode, float damageToDeal, float enemyKnockback, List<GameActorEffect> statusEffects, float jammedDMGMult, float bossDMGMult, float SlashRange, float SlashDimensions)
        {
            int slashId = Time.frameCount;
            List<SpeculativeRigidbody> alreadyHit = new List<SpeculativeRigidbody>();
            if (knockbackForce != 0f && owner != null) owner.knockbackDoer.ApplyKnockback(BraveMathCollege.DegreesToVector(angle, 1f), knockbackForce, 0.25f, false);
            float ela = 0f;
            while (ela < 0.2f)
            {
                ela += BraveTime.DeltaTime;
                HandleHeroSwordSlash(alreadyHit, position, angle, slashId, owner, intmode, damageToDeal, enemyKnockback, statusEffects, jammedDMGMult, bossDMGMult, SlashRange, SlashDimensions);
                yield return null;
            }
            yield break;
        }
        public enum ProjInteractMode
        {
            IGNORE,
            DESTROY,
            REFLECT
        }
        private static bool ProjectileIsValid(Projectile proj)
        {
            if (proj && (!(proj.Owner is PlayerController) || proj.ForcePlayerBlankable)) return true;
            else return false;
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
        private static void HandleHeroSwordSlash(List<SpeculativeRigidbody> alreadyHit, Vector2 arcOrigin, float slashAngle, int slashId, PlayerController owner, ProjInteractMode intmode, float damageToDeal, float enemyKnockback, List<GameActorEffect> statusEffects, float jammedDMGMult, float bossDMGMult, float slashRange, float slashDimensions)
        {

            ReadOnlyCollection<Projectile> allProjectiles2 = StaticReferenceManager.AllProjectiles;
            for (int j = allProjectiles2.Count - 1; j >= 0; j--)
            {
                Projectile projectile2 = allProjectiles2[j];
                if (ProjectileIsValid(projectile2))
                {
                    Vector2 projectileCenter = projectile2.sprite.WorldCenter;
                    if (ObjectWasHitBySlash(projectileCenter, arcOrigin, slashAngle, slashRange, slashDimensions))
                    {
                        if (intmode != ProjInteractMode.IGNORE || projectile2.collidesWithProjectiles)
                        {
                            if (intmode == ProjInteractMode.DESTROY || intmode == ProjInteractMode.IGNORE) projectile2.DieInAir(false, true, true, true);
                            else if (intmode == ProjInteractMode.REFLECT)
                            {
                                if (projectile2.LastReflectedSlashId != slashId)
                                {
                                    PassiveReflectItem.ReflectBullet(projectile2, true, owner, 2f, 1f, 1f, 0f);
                                    projectile2.LastReflectedSlashId = slashId;
                                }
                            }
                        }
                    }
                }
            }
            DealDamageToEnemiesInArc(owner, arcOrigin, slashAngle, slashRange, damageToDeal, enemyKnockback, statusEffects, jammedDMGMult, bossDMGMult, slashDimensions, alreadyHit);

            List<MinorBreakable> allMinorBreakables = StaticReferenceManager.AllMinorBreakables;
            for (int k = allMinorBreakables.Count - 1; k >= 0; k--)
            {
                MinorBreakable minorBreakable = allMinorBreakables[k];
                if (minorBreakable && minorBreakable.specRigidbody)
                {
                    if (!minorBreakable.IsBroken && minorBreakable.sprite)
                    {
                        if (ObjectWasHitBySlash(minorBreakable.sprite.WorldCenter, arcOrigin, slashAngle, slashRange, slashDimensions))
                        {
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
                        if (!majorBreakable.IsSecretDoor && !majorBreakable.IsDestroyed)
                        {
                            if (ObjectWasHitBySlash(majorBreakable.specRigidbody.UnitCenter, arcOrigin, slashAngle, slashRange, slashDimensions))
                            {
                                float num9 = damageToDeal;
                                if (majorBreakable.healthHaver)
                                {
                                    num9 *= 0.2f;
                                }
                                majorBreakable.ApplyDamage(num9, majorBreakable.specRigidbody.UnitCenter - arcOrigin, false, false, false);
                                alreadyHit.Add(majorBreakable.specRigidbody);
                            }
                        }
                    }
                }
            }
        }
        private static void DealDamageToEnemiesInArc(PlayerController owner, Vector2 arcOrigin, float arcAngle, float arcRadius, float overrideDamage, float overrideForce, List<GameActorEffect> statusEffects, float jammedDMGMult, float bossDMGMult, float slashDimensions, List<SpeculativeRigidbody> alreadyHit = null)
        {
            RoomHandler roomHandler = owner.CurrentRoom;
            if (roomHandler == null) return;
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
                                float num = Vector2.Distance(vector, arcOrigin);
                                if (ObjectWasHitBySlash(vector, arcOrigin, arcAngle, arcRadius, slashDimensions))
                                {
                                    bool flag = true;
                                    int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable);
                                    RaycastResult raycastResult;
                                    if (PhysicsEngine.Instance.Raycast(arcOrigin, vector - arcOrigin, num, out raycastResult, true, true, rayMask, null, false, null, null) && raycastResult.SpeculativeRigidbody != bodyRigidbody)
                                    {
                                        flag = false;
                                    }
                                    RaycastResult.Pool.Free(ref raycastResult);
                                    if (flag)
                                    {
                                        float damage = DealSwordDamageToEnemy(owner, aiactor, arcOrigin, vector, arcAngle, overrideDamage, overrideForce, statusEffects, bossDMGMult, jammedDMGMult);
                                        if (alreadyHit != null)
                                        {
                                            if (alreadyHit.Count == 0)
                                            {
                                                StickyFrictionManager.Instance.RegisterSwordDamageStickyFriction(damage);
                                            }
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
        private static float DealSwordDamageToEnemy(PlayerController owner, AIActor targetEnemy, Vector2 arcOrigin, Vector2 contact, float angle, float damage, float knockback, List<GameActorEffect> statusEffects, float bossDMGMult, float jammedDMGMult)
        {
            if (targetEnemy.healthHaver)
            {
                float damageToDeal = damage;
                if (targetEnemy.healthHaver.IsBoss) damageToDeal *= bossDMGMult;
                if (targetEnemy.IsBlackPhantom) damageToDeal *= jammedDMGMult;
                targetEnemy.healthHaver.ApplyDamage(damageToDeal, contact - arcOrigin, owner.ActorName, CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
            }
            if (targetEnemy.knockbackDoer)
            {
                targetEnemy.knockbackDoer.ApplyKnockback(contact - arcOrigin, knockback, false);
            }
            if (statusEffects != null && statusEffects.Count > 0)
            {
                foreach (GameActorEffect effect in statusEffects)
                {
                    targetEnemy.ApplyEffect(effect);
                }
            }
            return damage;
        }
    }
    public static class Logic
    {
        public static bool IsBetweenRange(this float numberToCheck, float bottom, float top)
        {
            return (numberToCheck >= bottom && numberToCheck <= top);
        }
    }
}