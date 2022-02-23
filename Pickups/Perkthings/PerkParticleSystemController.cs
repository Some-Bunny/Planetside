using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections;



namespace Planetside
{ 
    public class PerkParticleSystemController : MonoBehaviour
    { 

        public void Start()
        {
            particleObject = UnityEngine.Object.Instantiate(StaticVFXStorage.PerkParticleObject).GetComponent<ParticleSystem>();
        }
        private static ParticleSystem particleObject;
        public Color ParticleSystemColor;
        public Color ParticleSystemColor2;
        public void Update()
        {
            tk2dBaseSprite sprite = base.GetComponent<tk2dBaseSprite>();
            if (sprite && !GameManager.Instance.IsPaused && (UnityEngine.Random.value > 0.5f))
            {
                Vector3 vector = sprite.WorldBottomLeft.ToVector3ZisY(0);
                Vector3 vector2 = sprite.WorldTopRight.ToVector3ZisY(0);
                Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
                ParticleSystem particleSystem = particleObject;
                var trails = particleSystem.trails;
                trails.worldSpace = false;
                var main = particleSystem.main;
                main.startColor = new ParticleSystem.MinMaxGradient(ParticleSystemColor != null ? ParticleSystemColor : Color.white, ParticleSystemColor2 != null ? ParticleSystemColor2 : Color.white);
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

        public void DoBigBurst(PlayerController player)
        {
            /*
            Vector3 vector = player.sprite.WorldBottomLeft.ToVector3ZisY(0);
            Vector3 vector2 = player.sprite.WorldTopRight.ToVector3ZisY(0);
            Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
            ParticleSystem particleSystem = particleObject;
            var trails = particleSystem.trails;
            trails.worldSpace = false;
            var main = particleSystem.main;
            main.startColor = new ParticleSystem.MinMaxGradient(ParticleSystemColor != null ? ParticleSystemColor : Color.white, ParticleSystemColor2 != null ? ParticleSystemColor2 : Color.white);
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
            {
                position = position,
                randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
            };
            var emission = particleSystem.emission;
            emission.enabled = false;
            particleSystem.gameObject.SetActive(true);
            for (int e = 0; e < 20; e++)
            {
                emitParams.randomSeed = (uint)UnityEngine.Random.Range(1, 1000);
                particleSystem.Emit(emitParams, 1);
            }
            */
        }  
    }
}
