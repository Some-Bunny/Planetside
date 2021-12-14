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
	internal class GunknownProjectile : MonoBehaviour
	{
		public GunknownProjectile()
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
                for (int i = 0; i < 12; i++)
                {
                    GameObject gameobject2 = PlayerOrbitalItem.CreateOrbital(playerController, UnknownGun.GunknownGuon, false);
                    GunknownGuonComponent yes = gameobject2.AddComponent<GunknownGuonComponent>();
                    yes.maxDuration = 15;
                    yes.player = playerController;
                }
            }
        }
        private Projectile projectile;
	}
}

