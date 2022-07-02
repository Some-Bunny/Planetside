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
	internal class ChamberEyeProjectile : MonoBehaviour
	{
		public ChamberEyeProjectile()
		{
		}

        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile != null)
            {
                Material mat1 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat1.mainTexture = this.projectile.sprite.renderer.material.mainTexture;
                mat1.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
                mat1.SetFloat("_EmissiveColorPower", 1.55f);
                mat1.SetFloat("_EmissivePower", 100);
                mat1.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                this.projectile.OverrideMotionModule = new HelixProjectileMotionModule
                {
                    helixAmplitude = 1.7f,
                    helixWavelength = 6f,
                    ForceInvert = (UnityEngine.Random.value > 0.5f) ? false : true
                };
                ImprovedAfterImage yes = this.projectile.gameObject.AddComponent<ImprovedAfterImage>();
                yes.spawnShadows = true;
                yes.shadowLifetime = (UnityEngine.Random.Range(0.15f, 0.5f));
                yes.shadowTimeDelay = 0.01f;
                yes.dashColor = new Color(1,0,0 , 0.05f);
            }
        }
        private Projectile projectile;
	}
}

