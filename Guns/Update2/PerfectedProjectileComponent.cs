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
	internal class PerfectedProjectileComponent : MonoBehaviour
	{
		public PerfectedProjectileComponent()
		{
		}

        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
         
            if (this.projectile != null)
            {
                Material mat1 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat1.mainTexture = this.projectile.sprite.renderer.material.mainTexture;
                mat1.SetColor("_EmissiveColor", new Color32(55, 181, 222, 255));
                mat1.SetFloat("_EmissiveColorPower", 1.55f);
                mat1.SetFloat("_EmissivePower", 100);
                mat1.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                this.projectile.StartCoroutine(this.Speed(this.projectile));
            }
        }

        public IEnumerator Speed(Projectile projectile)
        {
            if (projectile != null)
            {
                for (int i = 0; i < 25; i++)
                {
                    projectile.baseData.speed += 1f;
                    projectile.UpdateSpeed();
                    yield return new WaitForSeconds(0.025f);
                }
            }
            else{ yield break;}
            yield break;
        }

        public void Update()
        {
            if (this.projectile != null)
            {
                Vector3 vector = this.projectile.sprite.WorldBottomLeft.ToVector3ZisY(0);
                Vector3 vector2 = this.projectile.sprite.WorldTopRight.ToVector3ZisY(0);
                Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
                ParticleSystem particleSystem = UnityEngine.Object.Instantiate(StaticVFXStorage.PerfectedParticleSystem).GetComponent<ParticleSystem>();
                var trails = particleSystem.trails;
                trails.worldSpace = false;
                var main = particleSystem.main;
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                {
                    position = position,
                    randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
                };
                var emission = particleSystem.emission;
                emission.enabled = false;
                particleSystem.gameObject.SetActive(true);
                particleSystem.Emit(emitParams, 1);
            }
        }
        private Projectile projectile;
	}
}

