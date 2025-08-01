using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections;



namespace Planetside
{ 
    public class PerkParticleSystemController : MonoBehaviour
    { 

        public Color ParticleSystemColor;
        public Color ParticleSystemColor2;
        public bool Active = true;
        public void Update()
        {

            if (Active == false) {return;}
            tk2dBaseSprite sprite = base.GetComponent<tk2dBaseSprite>();
            if (sprite && !GameManager.Instance.IsPaused && (UnityEngine.Random.value > Time.deltaTime * 20))
            {
                Vector3 vector = sprite.WorldBottomLeft.ToVector3ZisY(0);
                Vector3 vector2 = sprite.WorldTopRight.ToVector3ZisY(0);
                Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
                ParticleSystem particleSystem = ParticleBase.ReturnParticleSystem("PerkParticle");
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
            Color Color1 = ParticleSystemColor != null ? ParticleSystemColor : Color.white;
            Color Color2 = ParticleSystemColor2 != null ? ParticleSystemColor2 : Color.white;

            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                startColor = Color.Lerp(Color1, Color2, UnityEngine.Random.value),
                position = player.sprite.WorldCenter,
                startSize = 22,
                startLifetime = 0.75f,
                velocity = Vector3.zero,
            });


            for (int e = 0; e < 24; e++)
            {
                Vector3 vector = player.sprite.WorldBottomLeft.ToVector3ZisY(0);
                Vector3 vector2 = player.sprite.WorldTopRight.ToVector3ZisY(0);
                Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
                ParticleSystem particleSystem = ParticleBase.ReturnParticleSystem("PerkParticle");

                var main = particleSystem.main;
                main.startColor = new ParticleSystem.MinMaxGradient(Color1, Color2);
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                {
                    position = position,
                };
                var emission = particleSystem.emission;
                emission.enabled = false;
                particleSystem.gameObject.SetActive(true);
                emitParams.randomSeed = (uint)UnityEngine.Random.Range(1, 1000);
                particleSystem.Emit(emitParams, 1);
            }
           
        }  
    }
}
