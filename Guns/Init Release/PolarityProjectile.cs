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

namespace Planetside
{
    internal class PolarityProjectile : MonoBehaviour
    {
        public PolarityProjectile()
        {
			IsDown = false;
			IsUp = false;
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
				//PlayerController player = playerController as PlayerController;
				bool flagA = playerController.PlayerHasActiveSynergy("Refridgeration") && playerController != null;
				if (flagA)
				{
					bool flag6 = IsUp == true;
					if (flag6)
					{
						projectile.OverrideMotionModule = new HelixProjectileMotionModule
						{
							helixAmplitude = 2f,
							helixWavelength = 8f,
							ForceInvert = false
						};
					}
					bool flag7 = IsDown == true;
					if (flag7)
					{
						projectile.OverrideMotionModule = new HelixProjectileMotionModule
						{
							helixAmplitude = 2f,
							helixWavelength = 8f,
							ForceInvert = true
						};
					}
				}
			}
        }
		public bool IsUp;
		public bool IsDown;
		private Projectile projectile;
    }
}
