using System;
using UnityEngine;
using System.Collections.Generic;

namespace Planetside
{
	public class TinyPlanetMotionModule : ProjectileAndBeamMotionModule
	{
		public static int GetOrbitersInGroup(int group)
		{
			if (TinyPlanetMotionModule.m_currentOrbiters.ContainsKey(group))
			{
				return (TinyPlanetMotionModule.m_currentOrbiters[group] == null) ? 0 : TinyPlanetMotionModule.m_currentOrbiters[group].Count;
			}
			return 0;
		}

		public float BeamOrbitRadius
		{
			get
			{
				return this.m_beamOrbitRadius;
			}
			set
			{
				this.m_beamOrbitRadius = value;
				this.m_beamOrbitRadiusCircumference = 6.28318548f * this.m_beamOrbitRadius;
			}
		}

		public override void UpdateDataOnBounce(float angleDiff)
		{
			if (!float.IsNaN(angleDiff))
			{
				this.m_initialUpVector = Quaternion.Euler(0f, 0f, angleDiff) * this.m_initialUpVector;
				this.m_initialRightVector = Quaternion.Euler(0f, 0f, angleDiff) * this.m_initialRightVector;
			}
		}

		public override void AdjustRightVector(float angleDiff)
		{
			if (!float.IsNaN(angleDiff))
			{
				this.m_initialUpVector = Quaternion.Euler(0f, 0f, angleDiff) * this.m_initialUpVector;
				this.m_initialRightVector = Quaternion.Euler(0f, 0f, angleDiff) * this.m_initialRightVector;
			}
		}


		public override void Move(Projectile source, Transform projectileTransform, tk2dBaseSprite projectileSprite, SpeculativeRigidbody specRigidbody, ref float m_timeElapsed, ref Vector2 m_currentDirection, bool Inverted, bool shouldRotate)
		{

			Vector2 vector = (!projectileSprite) ? projectileTransform.position.XY() : projectileSprite.WorldCenter;
			source.sprite.renderer.enabled = true;
			if (!this.m_initialized)
			{
				this.m_initialized = true;
				this.m_initialRightVector = ((!shouldRotate) ? m_currentDirection : projectileTransform.right.XY());
				this.m_initialUpVector = ((!shouldRotate) ? (Quaternion.Euler(0f, 0f, 90f) * m_currentDirection) : projectileTransform.up);
				this.m_radius = 0.1f;
				this.m_currentAngle = this.m_initialRightVector.ToAngle();
				source.OnDestruction += this.OnDestroyed;
			}
			m_timeElapsed += BraveTime.DeltaTime;
			float radius = this.m_radius + (m_timeElapsed*5);
			radius = Mathf.Min(MaxRadius, radius);
			float num = source.Speed * BraveTime.DeltaTime;
			float num2 = num / (OrbitTightness * radius) * 360f;
			if (this.ForceInvert)
            {
				this.m_currentAngle += num2;
			}
			else
            {
				this.m_currentAngle -= num2;
			}
			Vector2 vector2 = Vector2.zero;

			if (this.usesAlternateOrbitTarget)
			{
				vector2 = vector;
			}
			else
			{
				vector2 = vector;
			}


			Vector2 vector3 = vector2 + (Quaternion.Euler(0f, 0f, this.m_currentAngle) * Vector2.right * radius).XY();
			if (this.StackHelix)
			{
				float num3 = 2f;
				float num4 = 1f;
				int num5 = (!this.ForceInvert) ? 1 : -1;
				float d = (float)num5 * num4 * Mathf.Sin(source.GetElapsedDistance() / num3);
				vector3 += (vector3 - vector2).normalized * d;
			}

			Vector2 velocity = (vector3 - vector) / BraveTime.DeltaTime;
			m_currentDirection = velocity.normalized;
			if (shouldRotate)
			{
				float num6 = m_currentDirection.ToAngle();
				if (float.IsNaN(num6) || float.IsInfinity(num6))
				{
					num6 = 0f;
				}
				projectileTransform.localRotation = Quaternion.Euler(0f, 0f, num6);
			}
			specRigidbody.Velocity = velocity/(60*(1+BraveTime.DeltaTime));
		}

		public void BeamDestroyed()
		{
			this.OnDestroyed(null);
		}

		private void OnDestroyed(Projectile obj)
		{
			if (this.m_isBeam)
			{
				this.m_isBeam = false;
				TinyPlanetMotionModule.ActiveBeams--;
			}
		}

		public override void SentInDirection(ProjectileData baseData, Transform projectileTransform, tk2dBaseSprite projectileSprite, SpeculativeRigidbody specRigidbody, ref float m_timeElapsed, ref Vector2 m_currentDirection, bool shouldRotate, Vector2 dirVec, bool resetDistance, bool updateRotation)
		{
		}

		public void RegisterAsBeam(BeamController beam)
		{
			if (!this.m_isBeam)
			{
				BasicBeamController basicBeamController = beam as BasicBeamController;
				if (basicBeamController && !basicBeamController.IsReflectedBeam)
				{
					basicBeamController.IgnoreTilesDistance = this.m_beamOrbitRadiusCircumference;
				}
				this.m_isBeam = true;
				TinyPlanetMotionModule.ActiveBeams++;
			}
		}

		public override Vector2 GetBoneOffset(BasicBeamController.BeamBone bone, BeamController sourceBeam, bool inverted)
		{
            if (sourceBeam.IsReflectedBeam)
            {
                return Vector2.zero;
            }
            PlayerController playerController = sourceBeam.Owner as PlayerController;
            Vector2 vector = playerController.unadjustedAimPoint.XY() - playerController.CenterPosition;
            float num = vector.ToAngle();
            Vector2 b = bone.Position - playerController.CenterPosition;
            Vector2 vector2;
			float m = Mathf.PingPong(Time.timeSinceLevelLoad, 2) * 0.5f;
            if (bone.PosX < this.m_beamOrbitRadiusCircumference)
            {
                float num2 = bone.PosX / (this.m_beamOrbitRadiusCircumference) * 360f + num;
                float x = Mathf.Cos(0.017453292f * num2) * this.BeamOrbitRadius * m;
                float y = Mathf.Sin(0.017453292f * num2) * this.BeamOrbitRadius * m;
                bone.RotationAngle = num2 + 90f;
                vector2 = new Vector2(x, y) - b;
            }
            else
            {
                bone.RotationAngle = num;
                vector2 = vector.normalized * (bone.PosX - (this.m_beamOrbitRadiusCircumference) + (this.BeamOrbitRadius * m)) - b;
            }
            if (this.StackHelix)
            {
                float num3 = 3f;
                float num4 = 1f;
                float num5 = 6f;
                int num6 = (!(inverted ^ this.ForceInvert)) ? 1 : -1;
                float num7 = bone.PosX - num5 * (Time.timeSinceLevelLoad % 600000f);
                float to = (float)num6 * num4 * Mathf.Sin(num7 * 3.1415927f / num3);
                vector2 += BraveMathCollege.DegreesToVector(bone.RotationAngle + 90f, Mathf.SmoothStep(0f, to, bone.PosX));
            }
            return vector2;
        }

		private static Dictionary<int, List<TinyPlanetMotionModule>> m_currentOrbiters = new Dictionary<int, List<TinyPlanetMotionModule>>();

		public float OrbitTightness = 6.28318548f;
		public float MinRadius = 0f;
		public float MaxRadius = 5f;


		public bool ForceInvert;

		private float m_radius;

		private float m_currentAngle;

		private bool m_initialized;

		private Vector2 m_initialRightVector;

		private Vector2 m_initialUpVector;

		public float lifespan = -1f;

		[NonSerialized]
		public bool usesAlternateOrbitTarget;

		[NonSerialized]
		public SpeculativeRigidbody alternateOrbitTarget;

		private float m_beamOrbitRadius = 1f;

		private float m_beamOrbitRadiusCircumference = 17.27876f;


		public bool m_isBeam;

		public static int ActiveBeams = 0;

		public bool StackHelix;

	}
}