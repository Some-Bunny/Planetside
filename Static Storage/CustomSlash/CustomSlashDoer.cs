using Alexandria.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside
{



    public class CustomProjectileSlashingBehaviour : MonoBehaviour
    {
        public float initialDelay;

        private float timer;

        public float timeBetweenSlashes;

        public bool SlashDamageUsesBaseProjectileDamage;

        public bool DestroyBaseAfterFirstSlash;

        public bool DestroysOnlyComponentAfterFirstSlash = false;

        public CustomSlashData slashParameters;

        private Projectile m_projectile;

        private PlayerController owner;

        public float timeBetweenCustomSequenceSlashes;

        public List<float> customSequence;

        public float angleVariance;

        public CustomProjectileSlashingBehaviour()
        {
            DestroyBaseAfterFirstSlash = false;
            timeBetweenSlashes = 1f;
            initialDelay = 0f;
            slashParameters = ScriptableObject.CreateInstance<BasicSlash>();
            SlashDamageUsesBaseProjectileDamage = true;
            timeBetweenCustomSequenceSlashes = 0.15f;
            customSequence = null;
            angleVariance = 0f;
        }

        private void Start()
        {
            m_projectile = GetComponent<Projectile>();
            timer = initialDelay;
            if ((bool)m_projectile.Owner && m_projectile.Owner is PlayerController)
            {
                owner = m_projectile.Owner as PlayerController;
            }
        }

        private void Update()
        {
            if ((bool)m_projectile)
            {
                if (timer > 0f)
                {
                    timer -= BraveTime.DeltaTime;
                }
                else
                {
                    m_projectile.StartCoroutine(DoAttackSequence());
                }
            }
        }

        private IEnumerator DoAttackSequence()
        {
            if (customSequence != null && customSequence.Count > 0)
            {
                foreach (float angle in customSequence)
                {
                    DoSlash(angle);
                    yield return new WaitForSeconds(timeBetweenCustomSequenceSlashes);
                }
            }
            else
            {
                DoSlash(0f);
            }

            timer = timeBetweenSlashes;
            if (DestroyBaseAfterFirstSlash)
            {
                StartCoroutine(Suicide());
            }
        }

        private void DoSlash(float angle)
        {
            Projectile projectile = m_projectile;
            List<GameActorEffect> list = new List<GameActorEffect>();
            list.AddRange(projectile.GetFullListOfStatusEffects(ignoresProbability: true));
            var slashData = slashParameters.ReturnClone();
            if (SlashDamageUsesBaseProjectileDamage)
            {
                slashData.damage = m_projectile.baseData.damage;
                slashData.bossDamageMult = m_projectile.BossDamageMultiplier;
                slashData.jammedDamageMult = m_projectile.BlackPhantomDamageMultiplier;
                slashData.enemyKnockbackForce = m_projectile.baseData.force;
            }

            slashData.OnHitTarget = (Action<GameActor, bool>)Delegate.Combine(slashData.OnHitTarget, new Action<GameActor, bool>(SlashHitTarget));
            slashData.OnHitBullet = (Action<Projectile>)Delegate.Combine(slashData.OnHitBullet, new Action<Projectile>(SlashHitBullet));
            slashData.OnHitMajorBreakable = (Action<MajorBreakable>)Delegate.Combine(slashData.OnHitMajorBreakable, new Action<MajorBreakable>(SlashHitMajorBreakable));
            slashData.OnHitMinorBreakable = (Action<MinorBreakable>)Delegate.Combine(slashData.OnHitMinorBreakable, new Action<MinorBreakable>(SlashHitMinorBreakable));
            angle += UnityEngine.Random.Range(angleVariance, 0f - angleVariance);
            CustomSlashDoer.DoSwordSlash(m_projectile.specRigidbody.UnitCenter, m_projectile.Direction.ToAngle() + angle, owner, slashData);
        }

        private IEnumerator Suicide()
        {
            yield return null;
            if (DestroysOnlyComponentAfterFirstSlash)
            {
                UnityEngine.Object.Destroy(this);
            }
            else
            {
                UnityEngine.Object.Destroy(m_projectile.gameObject);
            }
        }

        public virtual void SlashHitTarget(GameActor target, bool fatal)
        {
        }

        public virtual void SlashHitBullet(Projectile target)
        {
        }

        public virtual void SlashHitMinorBreakable(MinorBreakable target)
        {
        }

        public virtual void SlashHitMajorBreakable(MajorBreakable target)
        {
        }
    }
}
