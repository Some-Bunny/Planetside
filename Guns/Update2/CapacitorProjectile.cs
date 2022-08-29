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
	internal class CapacitorProjectile : MonoBehaviour
	{
		public CapacitorProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            this.gunComp = projectile.PossibleSourceGun.GetComponent<Capactior>();
            this.gun = projectile.PossibleSourceGun;

            if (this.projectile != null)
            {
                if (gun != null && gunComp != null)
                {
                    if (gun.LocalInfiniteAmmo == true || gun.InfiniteAmmo == true)
                    {
                        projectile.baseData.damage *= 0.125f;
                    }
                    else
                    {
                        if (gunComp.IsChargeUp() == true)
                        {
                            projectile.baseData.range = 0;
                            projectile.sprite.renderer.enabled = false;
                            projectile.baseData.damage = 0;
                        }   
                    }             
                }

            }
        }
        public Capactior gunComp;
        public Gun gun;
        private Projectile projectile;
	}
}

