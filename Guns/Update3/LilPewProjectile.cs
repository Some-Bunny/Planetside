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
	internal class AirBlasterProjectile : MonoBehaviour
	{
		public AirBlasterProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            PlayerController playerController = this.projectile.Owner as PlayerController;
            if (this.projectile)
            {
                float GunClip = playerController.CurrentGun.ClipShotsRemaining != 0 ? playerController.CurrentGun.ClipShotsRemaining : 0;
                this.projectile.baseData.speed += (GunClip/2);
                projectile.UpdateSpeed();
            }
        }
		private Projectile projectile;
	}
}

