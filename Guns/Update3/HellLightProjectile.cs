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
	internal class HellLightProjectile : MonoBehaviour
	{
		public HellLightProjectile()
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
                component.baseData.damage += 2.5f;

                SpeculativeRigidbody specRigidbody2 = component.specRigidbody;
				specRigidbody2.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(specRigidbody2.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreTileCollision));
                component.collidesWithPlayer = false;

            }
        }
        public IEnumerator ResetSpearTimer()
        {
            HasSpeared = true;
            yield return new WaitForSeconds(0.125f);
            HasSpeared = false;
            yield break;
        }
        private bool HasSpeared;

        private void OnPreTileCollision(SpeculativeRigidbody myrigidbody, PixelCollider mypixelcollider, PhysicsEngine.Tile tile, PixelCollider tilepixelcollider)
		{
            if (myrigidbody && HasSpeared != true)
            {
                PlayerController playerController = myrigidbody.GetComponent<Projectile>().Owner as PlayerController;
                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(HellLight.HellLightProjectile.gameObject, myrigidbody.sprite.WorldCenter, Quaternion.Euler(0f, 0f, 0f), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {
                    component.Owner = playerController;
                    component.Shooter = playerController.specRigidbody;

                    component.baseData.damage = 10;
                    component.gameObject.AddComponent<MaintainDamageOnPierce>();
                    component.baseData.speed = 0;
                    PierceProjModifier spook = component.gameObject.GetOrAddComponent<PierceProjModifier>();
                    spook.penetration = 10000;
                    DivineLightObjectController divine = component.gameObject.AddComponent<DivineLightObjectController>();
                    divine.player = playerController;
                    divine.self = component;
                }
                GameManager.Instance.StartCoroutine(this.ResetSpearTimer());
            }
        }
        private Projectile projectile;
	}
}

