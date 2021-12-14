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
            //GameObject original;
            this.projectile = base.GetComponent<Projectile>();
            Projectile projectile = this.projectile;
            PlayerController playerController = projectile.Owner as PlayerController;
            Projectile component = base.gameObject.GetComponent<Projectile>();
            bool flag = component != null;
            bool flag2 = flag;
            if (flag2)
            {
                if (Capactior.Chrager == true)
                {
                    projectile.baseData.range = 0;
                    projectile.sprite.renderer.enabled = false;
                    projectile.baseData.damage = 0;
                }
                else
                {

                }
            }
        }

        private FieldInfo remainingTimeCooldown = typeof(PlayerItem).GetField("remainingTimeCooldown", BindingFlags.Instance | BindingFlags.NonPublic);
        private FieldInfo remainingDamageCooldown = typeof(PlayerItem).GetField("remainingDamageCooldown", BindingFlags.Instance | BindingFlags.NonPublic);
        private Projectile projectile;
	}
}

