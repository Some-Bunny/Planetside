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
	internal class PetrifierProjectile : MonoBehaviour
	{
		public PetrifierProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            this.player = projectile.Owner as PlayerController;
			this.gun = projectile.PossibleSourceGun.GetComponent<Petrifier>();

			if (this.projectile != null)
            {
				projectile.OnHitEnemy += ProjectileHit;
			}
		}

		private void ProjectileHit(Projectile projectile, SpeculativeRigidbody otherBody, bool fatal)
        {
			if (otherBody.aiActor != null && otherBody.aiActor.behaviorSpeculator && !otherBody.aiActor.IsHarmlessEnemy && fatal == true)
			{
				if (gun != null)
                {
					gun.ExtraClipBonus++;
				}
			}		
		}
	
		public PlayerController player;
		public Petrifier gun;
        private Projectile projectile;
	}
}

