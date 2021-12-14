using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planetside 
{
    class LockedRangeComponent : BraveBehaviour
    {
		public void Awake()
		{
			bool flag = base.projectile != null;
			if (flag)
			{
				this.m_origRange = base.projectile.baseData.range;
			}
		}

		public void Update()
		{
			bool flag = base.projectile != null && base.projectile.baseData.range != this.m_origRange;
			if (flag)
			{
				base.projectile.baseData.range = this.m_origRange;
			}
		}
		private float m_origRange;
	}
}
