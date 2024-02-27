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
	internal class BanditsRevolverFinaleProjectile : MonoBehaviour
	{
		public BanditsRevolverFinaleProjectile()
		{
		}

        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile != null)
            {
                this.projectile.OnWillKillEnemy = (Action<Projectile, SpeculativeRigidbody>)Delegate.Combine(projectile.OnWillKillEnemy, new Action<Projectile, SpeculativeRigidbody>(this.OnKill));
            }
        }
        private void OnKill(Projectile arg1, SpeculativeRigidbody arg2)
        {
            
            PlayerController playerController = arg1.Owner as PlayerController;
            if (arg2 != null && arg2.healthHaver != null && HasKilled != true)
            {
                HasKilled = true;
                AkSoundEngine.PostEvent("Play_perfectshot", playerController.gameObject);
                playerController.CurrentGun.ForceImmediateReload();
            }
        }
        bool HasKilled;
        private Projectile projectile;
	}
}

