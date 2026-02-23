using Brave.BulletScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planetside.Misc_Stuff
{
    public class BlueSpeedChangingBullet : Bullet
    {
        public BlueSpeedChangingBullet(float newSpeed, int term, int destroyTimer = -1) : base(null, false, false, false)
        {
            this.m_newSpeed = newSpeed;
            this.m_term = term;
            this.m_destroyTimer = destroyTimer;
        }

        public BlueSpeedChangingBullet(string name, float newSpeed, int term, int destroyTimer = -1, bool suppressVfx = false) : base(name, suppressVfx, false, false)
        {
            this.m_newSpeed = newSpeed;
            this.m_term = term;
            this.m_destroyTimer = destroyTimer;
        }

        public override IEnumerator Top()
        {
            (Projectile as ThirdDimensionalProjectile).SetUnDodgeableState(true);
            base.ChangeSpeed(new Speed(this.m_newSpeed, SpeedType.Absolute), this.m_term);
            if (this.m_destroyTimer < 0)
            {
                yield break;
            }
            yield return base.Wait(this.m_term + this.m_destroyTimer);
            base.Vanish(false);
            yield break;
        }

        private float m_newSpeed;

        private int m_term;
        private int m_destroyTimer;
    }

    public class BlueBullet : Bullet
    {
        public BlueBullet(string bankName = null, bool suppressVfx = false, bool firstBulletOfAttack = false, bool forceBlackBullet = false)
        {
            this.BankName = bankName;
            this.SuppressVfx = suppressVfx;
            this.FirstBulletOfAttack = firstBulletOfAttack;
            this.ForceBlackBullet = forceBlackBullet;
        }
        public override IEnumerator Top()
        {
            (Projectile as ThirdDimensionalProjectile).SetUnDodgeableState(true);
            yield break;
        }
    }
}
