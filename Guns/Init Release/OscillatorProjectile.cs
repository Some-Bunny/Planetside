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
	internal class OscillatorProjectile : MonoBehaviour
	{
		public OscillatorProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            Projectile projectile = this.projectile;
            PlayerController playerController = projectile.Owner as PlayerController;
            Projectile component = base.gameObject.GetComponent<Projectile>();
            bool flag = component != null;
            bool flag2 = flag;
            if (flag2)
            {
                projectile.OverrideMotionModule = new OscillatingeMotionModule();

            }
        }
        private Projectile projectile;
	}
}

