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
using Alexandria.Misc;
using static ETGMod;
using static UnityEngine.UI.GridLayoutGroup;

namespace Planetside
{
	internal class BurningSunProjectile : MonoBehaviour
	{

        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile != null)
            {
				AkSoundEngine.PostEvent("Play_Burn", this.projectile.gameObject);
				this.ShockRing(projectile);
                projectile.OnDestruction += this.EndRingEffect;
                EasyLight.Create(projectile.specRigidbody.UnitCenter, projectile.transform, new Color(1, 0.6f, 0, 1), -1, 10, true, 10, 1, 1, true);
            }
            cachedgravitationalForceActors = gravitationalForceActors;
            this.m_radiusSquared = this.radius * this.radius;
        }
		public void FixedUpdate()
        {			
			if (this.projectile != null)
            {
				if (!GameManager.Instance.IsPaused) 
                {
                    Exploder.DoDistortionWave(this.projectile.sprite.WorldCenter, 0.02f, 0.25f, 6, 0.5f); 
                }
			}

            Elapsed += Time.fixedDeltaTime;
            gravitationalForceActors = Mathf.Lerp(0, cachedgravitationalForceActors, Elapsed);
            for (int i = 0; i < PhysicsEngine.Instance.AllRigidbodies.Count; i++)
            {
                if (PhysicsEngine.Instance.AllRigidbodies[i].gameObject.activeSelf)
                {
                    if (PhysicsEngine.Instance.AllRigidbodies[i].enabled)
                    {
                        this.AdjustRigidbodyVelocity(PhysicsEngine.Instance.AllRigidbodies[i]);
                    }
                }
            }
            for (int j = 0; j < StaticReferenceManager.AllDebris.Count; j++)
            {
                this.AdjustDebrisVelocity(StaticReferenceManager.AllDebris[j]);
            }

        }
		private void EndRingEffect(Projectile projectile)
		{
			Exploder.DoDistortionWave(projectile.sprite.WorldCenter, 1f, 0.25f, 8, 0.5f);
			AkSoundEngine.PostEvent("Stop_Burn", projectile.gameObject);
			AkSoundEngine.PostEvent("Play_BOSS_lichB_charge_01", projectile.gameObject);
			DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.FireDef);
            goopManagerForGoopType.TimedAddGoopCircle(projectile.sprite.WorldCenter, 4f, 0.8f, false);
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = projectile.sprite.WorldCenter,
                startSize = 8,
                rotation = 0,
                startLifetime = 0.3f,
                startColor = Color.red.WithAlpha(0.4f)
            });
            EasyLight.Create(projectile.specRigidbody.UnitCenter, null, new Color(1, 0.6f, 0, 1), 1, 15, false, 10, 0, 1, false, false);
        }
		private HeatIndicatorController m_radialIndicator;

        private void ShockRing(Projectile projectile)
		{
			this.m_radialIndicator = (UnityEngine.Object.Instantiate(StaticVFXStorage.RadialRing, projectile.sprite.WorldCenter, Quaternion.identity, projectile.transform)).GetComponent<HeatIndicatorController>();
			this.m_radialIndicator.CurrentColor = Color.red.WithAlpha(0f);
			this.m_radialIndicator.IsFire = true;
			this.m_radialIndicator.CurrentRadius = 4f;
		}



        public float radius = 40f;
        public float gravitationalForce = 10f;
        public float gravitationalForceActors = 50f;
        private float m_radiusSquared;
        public int Stack = 1;

        private float cachedgravitationalForceActors;

        private float Elapsed = 0;



        private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
        {
            Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
            return new Vector4(vector.x, vector.y, 0f, 0f);
        }

        private float GetDistanceToRigidbody(SpeculativeRigidbody other)
        {
            return Vector2.Distance(other.UnitCenter, projectile.specRigidbody.UnitCenter);
        }

        private Vector2 GetFrameAccelerationForRigidbody(Vector2 unitCenter, float currentDistance, float g)
        {
            Vector2 zero = Vector2.zero;
            float num = Mathf.Clamp01(1f - currentDistance / this.radius);
            float d = g * num * num;
            Vector2 normalized = (projectile.specRigidbody.UnitCenter - unitCenter).normalized;
            return normalized * d;
        }

        private bool AdjustDebrisVelocity(DebrisObject debris)
        {
            if (debris.IsPickupObject)
            {
                return false;
            }
            if (debris.GetComponent<BlackHoleDoer>() != null)
            {
                return false;
            }
            Vector2 a = debris.sprite.WorldCenter - projectile.specRigidbody.UnitCenter;
            float num = Vector2.SqrMagnitude(a);
            if (num >= this.m_radiusSquared)
            {
                return false;
            }
            float g = this.gravitationalForceActors;
            float num2 = Mathf.Sqrt(num);
            if (num2 < 3)
            {
                UnityEngine.Object.Destroy(debris.gameObject);
                return true;
            }
            Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(debris.sprite.WorldCenter, num2, g);
            float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
            if (debris.HasBeenTriggered)
            {
                debris.ApplyVelocity(frameAccelerationForRigidbody * d);
            }
            else if (num2 < this.radius / 2f)
            {
                debris.Trigger(frameAccelerationForRigidbody * d, 0.5f, 1f);
            }
            return true;
        }
        private bool AdjustRigidbodyVelocity(SpeculativeRigidbody other)
        {
            Vector2 a = other.UnitCenter - projectile.specRigidbody.UnitCenter;
            float num = Vector2.SqrMagnitude(a);
            if (num < this.m_radiusSquared)
            {
                float g = this.gravitationalForce;
                Vector2 velocity = other.Velocity;
                Projectile projectile = other.projectile;
                if (projectile)
                {
                    return false;
                }
                else
                {
                    if (!other.aiActor)
                    {
                        return false;
                    }
                    g = this.gravitationalForceActors;
                    if (!other.aiActor.enabled)
                    {
                        return false;
                    }
                    if (!other.aiActor.HasBeenEngaged)
                    {
                        return false;
                    }
                    if (other.healthHaver.IsBoss)
                    {
                        return false;
                    }
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
                /*
                if (projectile != null)
                {
                    projectile.collidesWithPlayer = false;
                    if (projectile.IsBulletScript)
                    {
                        projectile.RemoveBulletScriptControl();
                    }
                    if (vector != Vector2.zero)
                    {
                        projectile.Direction = vector.normalized;
                        projectile.Speed = Mathf.Max(3f, vector.magnitude);
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
                */
                return true;
            }
            return false;
        }

        private AdditionalBraveLight lightObject;
		private Projectile projectile;
	}
}

