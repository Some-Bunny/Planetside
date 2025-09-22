using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside
{
    public class ParticleBase
    {
        public static Dictionary<string, ParticleSystem> _ParticleSystems = new Dictionary<string, ParticleSystem>();
        public static void InitParticleBase()
        {
            _ParticleSystems = new Dictionary<string, ParticleSystem>()
            {
                {"BloodSplatterParticle", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("BloodSplatter").GetComponent<ParticleSystem>()) },
                {"CeramicParticle", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("CeramicParticles").GetComponent<ParticleSystem>()) },
                {"Eliteparticle", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("EliteVariantEnemy").GetComponent<ParticleSystem>()) },
                {"PerfectedParticle", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PerfectedParticles").GetComponent<ParticleSystem>()) },
                {"ShamberParticle", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("ShamberParticles").GetComponent<ParticleSystem>()) },
                {"ShellraxEyeParticle", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("ShellraxEye").GetComponent<ParticleSystem>()) },
                {"WaveParticle", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("WaveParticles").GetComponent<ParticleSystem>()) },
                {"DonutWaveParticle", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("WaveParticles_Donut").GetComponent<ParticleSystem>()) },
                {"WaveParticleInverse", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("WaveParticlesInverse").GetComponent<ParticleSystem>()) },
                {"HexParticleFull", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("HexParticlesFull").GetComponent<ParticleSystem>()) },
                {"HexParticle", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("HexParticles").GetComponent<ParticleSystem>()) },
                {"PerkParticle", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("TheperkParticle").GetComponent<ParticleSystem>()) },

                
                {"EmberParticle_BG", ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/EmberSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>() },
                {"ChaffParticle_BG", ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/ChaffSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>() },
                {"FireParticle_BG", ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/GlobalFireSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>() },
                {"DarkMagics_BG", ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/DarkMagicSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>() },
                {"BlueOrbParticle", UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PortalClose").GetComponent<ParticleSystem>()) },

            };
            _ParticleSystems["BlueOrbParticle"].gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            foreach (var item in _ParticleSystems){UnityEngine.Object.DontDestroyOnLoad(item.Value);}
        }

        public static ParticleSystem ReturnParticleSystem(string name)
        {
            if (!_ParticleSystems.ContainsKey(name))
            {
                return null;
            }
            return _ParticleSystems[name];
        }

        public  static void EmitParticles(string name, int amount, ParticleSystem.EmitParams newParams)
        {
            if (!_ParticleSystems.ContainsKey(name))
            {
                return;
            }
            var ParticleSystem = _ParticleSystems[name];
            ParticleSystem.Emit(newParams, amount);
        }

        public static void EmitParticles(string name, int amount, Vector2 position, Vector3 Velocity, float rotation = 0, float lifeTime = 1, float size = 1)
        {
            if (!_ParticleSystems.ContainsKey(name))
            {
                return;
            }
            var newParams = new ParticleSystem.EmitParams();
            newParams.position = position;
            newParams.rotation = rotation;
            newParams.startLifetime = lifeTime;
            newParams.startSize = size;
            newParams.velocity = Velocity;
            var ParticleSystem = _ParticleSystems[name];

            ParticleSystem.Emit(newParams, amount);
        }
        public static void EmitParticles(ParticleSystem particleSystem, int amount, Vector2 position, Vector3 Velocity, float rotation = 0, float lifeTime = 1, float size = 1)
        {
            var newParams = new ParticleSystem.EmitParams();
            newParams.position = position;
            newParams.rotation = rotation;
            newParams.startLifetime = lifeTime;
            newParams.startSize = size;
            newParams.velocity = Velocity;

            particleSystem.Emit(newParams, amount);
        }

        public static void EmitParticles(ParticleSystem particleSystem, int amount, ParticleSystem.EmitParams newParams)
        {
            particleSystem.Emit(newParams, amount);
        }
    }
}
