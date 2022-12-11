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
using HutongGames.PlayMaker.Actions;

namespace Planetside
{
	internal class ImmateriaProjectile : MonoBehaviour
	{
		public ImmateriaProjectile()
		{
            this.damageMultiplierPerFrame = 1.01f;
            this.gravitationalForce = 10;
            this.radius = 10;
            this.radiusSquared = radius * radius;
            this.MinimumSpeed = 13;
            this.Duration = 5;
        }
		public void Start()
		{
			this.projectile = base.GetComponent<Projectile>();
			if (this.projectile != null)
			{

                this.m_distortMaterial = new Material(ShaderCache.Acquire("Brave/Internal/DistortionRadius"));
                this.m_distortMaterial.SetFloat("_Strength", 300f);
                this.m_distortMaterial.SetFloat("_TimePulse", 0.15f);
                this.m_distortMaterial.SetFloat("_RadiusFactor", 0.15f);
                this.m_distortMaterial.SetVector("_WaveCenter", this.GetCenterPointInScreenUV(projectile.sprite.WorldCenter));
                Pixelator.Instance.RegisterAdditionalRenderPass(this.m_distortMaterial);

                this.Invoke("Die", Duration);
			}
		}
        private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
        {
            Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
            return new Vector4(vector.x, vector.y, 0f, 0f);
        }
        private Material m_distortMaterial;


        public void OnDestroy()
        {
            if (Pixelator.Instance != null && this.m_distortMaterial != null)
            {
                Pixelator.Instance.DeregisterAdditionalRenderPass(this.m_distortMaterial);
            }
        }

        public void Die()
        {
            if (projectile != null) {
                Exploder.DoDistortionWave(projectile.sprite.WorldTopCenter, gravitationalForce, 0.25f, 30, 0.5f);
                AkSoundEngine.PostEvent("Play_BOSS_queenship_explode_01", projectile.gameObject);
                this.projectile.DieInAir(false);
                {
                    Pixelator.Instance.DeregisterAdditionalRenderPass(this.m_distortMaterial);
                }

            }
        }


        public void Update()
        {
            if (this.projectile != null)
            {
                if (this.m_distortMaterial != null)
                {
                    this.m_distortMaterial.SetVector("_WaveCenter", this.GetCenterPointInScreenUV(projectile.sprite.WorldCenter));
                }

                for (int i = 0; i < StaticReferenceManager.AllProjectiles.Count; i++)
                {
                    if (StaticReferenceManager.AllProjectiles[i].gameObject.activeSelf)
                    {
                        if (StaticReferenceManager.AllProjectiles[i].enabled)
                        {
                            this.AdjustRigidbodyVelocity(StaticReferenceManager.AllProjectiles[i].specRigidbody);
                        }
                    }
                }
            }
        }

        public float MinimumSpeed;

        public float damageMultiplierPerFrame;
        public float gravitationalForce;
        public float radius;
        private float radiusSquared;
        public float Duration;

        private bool AdjustRigidbodyVelocity(SpeculativeRigidbody other)
        {
            Vector2 a = other.UnitCenter - this.projectile.specRigidbody.UnitCenter;
            float num = Vector2.SqrMagnitude(a);
            if (num < this.radiusSquared)
            {
                float g = this.gravitationalForce;
                Vector2 velocity = other.Velocity;
                Projectile projectile = other.projectile;
                if (projectile)
                {
                    projectile.collidesWithPlayer = false;
                    if (other.GetComponent<BlackHoleDoer>() != null)
                    {
                        return false;
                    }
                    if (other.GetComponent<ParticleCollapserLargeProjectile>() != null)
                    {
                        return false;
                    }
                    if (other.GetComponent<ImmateriaProjectile>() != null)
                    {
                        return false;
                    }
                    if (velocity == Vector2.zero)
                    {
                        return false;
                    }
                    g = this.gravitationalForce;
                }

                Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(other.UnitCenter, Mathf.Sqrt(num), g);
                float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
                Vector2 b = frameAccelerationForRigidbody * d;
                Vector2 vector = velocity + b;
                if (BraveTime.DeltaTime > 0.02f)
                {
                    vector *= 0.02f / BraveTime.DeltaTime;
                }
                other.Velocity = vector;
                if (projectile != null)
                {
                    projectile.collidesWithPlayer = false;
                    if (projectile.IsBulletScript)
                    {
                        projectile.RemoveBulletScriptControl();
                    }
                    if (vector != Vector2.zero)
                    {
                        projectile.baseData.damage *= damageMultiplierPerFrame;
                        projectile.baseData.range += 1f;
                        BounceProjModifier BpM= projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
                        BpM.numberOfBounces += Mathf.Max(10 - BpM.numberOfBounces, 0);

                        PierceProjModifier spookCollapse = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                        spookCollapse.penetration = 10;
                        spookCollapse.penetratesBreakables = true;
                        MaintainDamageOnPierce noDamageLossCollapse = projectile.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
                        noDamageLossCollapse.damageMultOnPierce = 1f;

                        projectile.Direction = vector.normalized;
                        projectile.Speed = Mathf.Max(MinimumSpeed, vector.magnitude);
                        projectile.Speed *= 0.995f;
                        other.Velocity = projectile.Direction * projectile.Speed;
                        if (projectile.shouldRotate && (vector.x != 0f || vector.y != 0f))
                        {
                            float num2 = BraveMathCollege.Atan2Degrees(projectile.Direction);
                            if (!float.IsNaN(num2) && !float.IsInfinity(num2))
                            {
                                Quaternion rotation = Quaternion.Euler(0f, 0f, num2);
                                if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y))
                                {
                                    projectile.transform.rotation = rotation;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }
        private Vector2 GetFrameAccelerationForRigidbody(Vector2 unitCenter, float currentDistance, float g)
        {
            Vector2 zero = Vector2.zero;
            float num = Mathf.Clamp01(1f - currentDistance / this.radius);
            float d = g * num * num;
            Vector2 normalized = (this.projectile.specRigidbody.UnitCenter - unitCenter).normalized;
            return (normalized * d) * 9;
        }

        private Projectile projectile;
	}
}

