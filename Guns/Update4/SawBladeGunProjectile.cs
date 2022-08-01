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
	internal class SawBladeGunProjectile : MonoBehaviour
	{
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile != null)
            {
                this.projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(this.projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreTileCollision));
            }
        }

        private void OnPreTileCollision(SpeculativeRigidbody myrigidbody, PixelCollider mypixelcollider, PhysicsEngine.Tile tile, PixelCollider tilepixelcollider)
        {
            if (myrigidbody)
            {
                this.projectile.DestroyMode = Projectile.ProjectileDestroyMode.None;
                this.projectile.specRigidbody.CollideWithTileMap = true;
                this.projectile.ManualControl = true;         
            }
        }

        private Projectile projectile;
	}
}

