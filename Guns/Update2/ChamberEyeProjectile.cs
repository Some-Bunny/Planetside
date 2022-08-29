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

namespace Planetside
{
	internal class ChamberEyeProjectile : MonoBehaviour
	{
		public ChamberEyeProjectile()
		{
		}

        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile != null)
            {
                projectile.OverrideMotionModule = new HelixProjectileMotionModule
                {
                    helixAmplitude = 1.7f,
                    helixWavelength = 6f,
                    ForceInvert = (UnityEngine.Random.value > 0.5f) ? false : true
                };
            }
        }
        private Projectile projectile;
	}
}

