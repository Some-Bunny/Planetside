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
	public class OscillatingeMotionModule : ProjectileAndBeamMotionModule
	{
		public override void UpdateDataOnBounce(float angleDiff)
		{
			if (!float.IsNaN(angleDiff))
			{
				this.m_initialVerticalVector = Quaternion.Euler(0f, 0f, angleDiff) * this.m_initialVerticalVector;
			}
		}

		public override void AdjustRightVector(float angleDiff)
		{
			if (!float.IsNaN(angleDiff))
			{
				this.m_initialVerticalVector = Quaternion.Euler(0f, 0f, angleDiff) * this.m_initialVerticalVector;
			}
		}

		public override void Move(Projectile source, Transform projectileTransform, tk2dBaseSprite projectileSprite, SpeculativeRigidbody specRigidbody, ref float m_timeElapsed, ref Vector2 m_currentDirection, bool Inverted, bool shouldRotate)
		{
			ProjectileData baseData = source.baseData;
			Vector2 vector = (!projectileSprite) ? projectileTransform.position.XY() : projectileSprite.WorldCenter;
			if (!this.m_OscillateInitialized)
			{
				this.m_OscillateInitialized = true;
				this.m_initialVerticalVector = ((!shouldRotate) ? m_currentDirection : projectileTransform.right.XY());
				this.m_privateLastPosition = vector;
				this.m_displacement = 0f;
				this.m_vertDisplacement = 0f;
			}
			m_timeElapsed += BraveTime.DeltaTime;
			int num = (!(Inverted ^ this.ForceInvert)) ? 1 : -1;
			float num2 = m_timeElapsed * baseData.speed;
			float num3 = (float)num * this.pulseAmplitude * Mathf.Sin(m_timeElapsed * 3.14159274f * baseData.speed / this.pulseWavelength);
			float d = num2 - this.m_displacement;
			float d2 = num3 - this.m_vertDisplacement;
			Vector2 vector2 = this.m_privateLastPosition + this.m_initialVerticalVector * d * d2;
			this.m_privateLastPosition = vector2;
			if (shouldRotate)
			{
				float num4 = (m_timeElapsed + 0.01f) * baseData.speed;
				float num5 = (float)num * this.pulseAmplitude * Mathf.Sin((m_timeElapsed + 0.01f) * 3.14159274f * baseData.speed / this.pulseWavelength);
				float num6 = BraveMathCollege.Atan2Degrees(num5 - num3, num4 - num2);
				projectileTransform.localRotation = Quaternion.Euler(0f, 0f, num6 + this.m_initialVerticalVector.ToAngle());
			}
			Vector2 vector3 = (vector2 - vector) / BraveTime.DeltaTime;
			float f = BraveMathCollege.Atan2Degrees(vector3);
			if (!float.IsNaN(f))
			{
				m_currentDirection = vector3.normalized;
			}
			this.m_displacement = num2 - 0.2f;
			this.m_vertDisplacement = num3 - 0.2f;
			specRigidbody.Velocity = vector3;
		}

		public override void SentInDirection(ProjectileData baseData, Transform projectileTransform, tk2dBaseSprite projectileSprite, SpeculativeRigidbody specRigidbody, ref float m_timeElapsed, ref Vector2 m_currentDirection, bool shouldRotate, Vector2 dirVec, bool resetDistance, bool updateRotation)
		{
			Vector2 privateLastPosition = (!projectileSprite) ? projectileTransform.position.XY() : projectileSprite.WorldCenter;
			this.m_OscillateInitialized = true;
			this.m_initialVerticalVector = ((!shouldRotate) ? m_currentDirection : projectileTransform.right.XY());
			this.m_privateLastPosition = privateLastPosition;
			this.m_displacement = 0f;
			this.m_vertDisplacement = 0f;
			m_timeElapsed = 0f;
		}

		public override Vector2 GetBoneOffset(BasicBeamController.BeamBone bone, BeamController sourceBeam, bool inverted)
		{		
			float num2 = bone.PosX - this.oscillateOffsetPerSecond * (Time.timeSinceLevelLoad % 600000f);
			float to = (this.pulseAmplitude * 1.666f) * Mathf.Sin(num2 * 3.14159274f / this.pulseWavelength * 4f);
			return BraveMathCollege.DegreesToVector(bone.RotationAngle + 00f, Mathf.SmoothStep(0f, to, bone.PosX));
			
			//return new Vector2(0f, 0f);
		}

		public float pulseWavelength = 7f;
		public float pulseAmplitude = 2f;

		public float oscillateOffsetPerSecond = 6f;
		public bool ForceInvert;

		private bool m_OscillateInitialized;

		private Vector2 m_initialVerticalVector;
		private Vector2 m_privateLastPosition;

		private float m_displacement;
		private float m_vertDisplacement;

	}
}