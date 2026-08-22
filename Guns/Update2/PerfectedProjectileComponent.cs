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
        public void Start()
        {
            this.projectile.StartCoroutine(this.Speed(this.projectile));
        }

        public IEnumerator Speed(Projectile projectile)
        {

            for (int i = 0; i < 25; i++)
            {
                if (projectile != null)
                {
                    projectile.baseData.speed += 1f;
                    projectile.UpdateSpeed();
                    yield return new WaitForSeconds(0.025f);
                }
                else
                {
                    yield break;
                }
            }
            yield break;
        }

        public void FixedUpdate()
        {
            if (this.projectile != null)
            {
                Vector3 vector = this.projectile.sprite.WorldBottomLeft.ToVector3ZisY(0);
                Vector3 vector2 = this.projectile.sprite.WorldTopRight.ToVector3ZisY(0);
                Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
                ParticleSystem particleSystem = StaticVFXStorage.PerfectedParticleSystem.GetComponent<ParticleSystem>();
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
        public Projectile projectile;
	}
}

