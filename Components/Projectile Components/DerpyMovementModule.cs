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
	public class DerpyMovementModule : ProjectileAndBeamMotionModule
	{

		//Used to presumably prevent itself from instantly destroying itself upon colliding with a wall if it moves in a weird way.
		public override void UpdateDataOnBounce(float angleDiff)
		{
			if (!float.IsNaN(angleDiff))
			{
				this.InitialVerticalVector = Quaternion.Euler(0f, 0f, angleDiff) * this.InitialVerticalVector;
			}
		}
		//Honestly no idea but it seems to always match UpdateDataOnBounce so if you're changing one its probably best to change the other.
		public override void AdjustRightVector(float angleDiff)
		{
			if (!float.IsNaN(angleDiff))
			{
				this.InitialVerticalVector = Quaternion.Euler(0f, 0f, angleDiff) * this.InitialVerticalVector;
			}
		}


		//The actual method that controls *how* it should move. you can insert formulas
		public override void Move(Projectile source, Transform projectileTransform, tk2dBaseSprite projectileSprite, SpeculativeRigidbody specRigidbody, ref float timeElapsed, ref Vector2 m_currentDirection, bool Inverted, bool shouldRotate)
		{
			//Some variables that could potentially help.
			ProjectileData baseData = source.baseData;
			//Centre position of the projectile thats moving.
			Vector2 ProjectilePosition = (!projectileSprite) ? projectileTransform.position.XY() : projectileSprite.WorldCenter;
			

			if (!this.Initialized)
			{
				this.Initialized = true;
				this.InitialVerticalVector =  ((!shouldRotate) ? m_currentDirection : projectileTransform.right.XY());
				this.LastPosition = ProjectilePosition;
				this.currentAngle = m_currentDirection.ToAngle();
			}

			//The speed of the projectile, which is taken into account for obvious reasons.
			float currentSpeed = baseData.speed;

			//To keep track of how much time has passed.
			timeElapsed += BraveTime.DeltaTime;
			TimerCount += BraveTime.DeltaTime;

			//I assume this is used to roate the projectile sprite in the direction it's shot towards.
			if (source.angularVelocity != 0f)
			{
				projectileTransform.RotateAround(projectileTransform.position.XY(), Vector3.forward, (source.angularVelocity) * timeElapsed);
			}

			//No fucking idea what this is.
			if (baseData.UsesCustomAccelerationCurve)
			{
				float time = Mathf.Clamp01((timeElapsed - baseData.IgnoreAccelCurveTime) / baseData.CustomAccelerationCurveDuration);
				currentSpeed = baseData.AccelerationCurve.Evaluate(time) * baseData.speed;
			}


			//For best results, you want to modify the "yourVector" that gets accounted into Velocity with your own code/maths.
			Vector2 yourVector = this.InitialVerticalVector + (Quaternion.Euler(0f, 0f, this.currentAngle) * Vector2.up * 0).XY();

			specRigidbody.Velocity = yourVector * currentSpeed;
			
			timeElapsed *= 1f - baseData.damping * timeElapsed;

			source.LastVelocity = specRigidbody.Velocity;

			if (TimerCount >= TimeDelay)
            {
				float RNG = (UnityEngine.Random.value > 0.5f) ? 90 : -90;
				TimerCount = 0;
				Vector2 Point = MathToolbox.GetUnitOnCircle(m_currentDirection.ToAngle() + RNG, 10);
				source.SendInDirection(Point, false, true);
			}
		}
		private float TimerCount;
		public float TimeDelay = 0.25f;

		//No idea what this is, probably some form of Start method to set some stuff up
		public override void SentInDirection(ProjectileData baseData, Transform projectileTransform, tk2dBaseSprite projectileSprite, SpeculativeRigidbody specRigidbody, ref float m_timeElapsed, ref Vector2 m_currentDirection, bool shouldRotate, Vector2 dirVec, bool resetDistance, bool updateRotation)
		{
			Vector2 privateLastPosition = (!projectileSprite) ? projectileTransform.position.XY() : projectileSprite.WorldCenter;
			this.Initialized = true;
			this.InitialVerticalVector = ((!shouldRotate) ? m_currentDirection : projectileTransform.right.XY());
			this.LastPosition = privateLastPosition;
			m_timeElapsed = 0f;
		}

		// can modifiy each individual beam bone in a beam to be located in a different place, make it return a vector of some kind
		public override Vector2 GetBoneOffset(BasicBeamController.BeamBone bone, BeamController sourceBeam, bool inverted)
		{
			//float RNG = (UnityEngine.Random.value > 0.5f) ? 90 : -90;
			//return BraveMathCollege.DegreesToVector(bone.RotationAngle + RNG, Mathf.SmoothStep(0f, 0, bone.PosX));
			
			if (UnityEngine.Random.value < 0.0002f && FlipperVal == 90)
            {
				FlipperVal = -90;
            }
			else if (UnityEngine.Random.value < 0.0002f && FlipperVal == -90)
            {
				FlipperVal = 90;
			}
			int num = 1;//(!(inverted ^ this.ForceInvert)) ? 1 : -1;
			BasicBeamController beaer = sourceBeam as BasicBeamController;
			float num2 = bone.PosX - 1 * (Time.timeSinceLevelLoad % 600000f);
			float to = (float)num * 1 * Mathf.Sign(Mathf.Sin(num2 * 3.14159274f)) / 1;
			return BraveMathCollege.DegreesToVector(bone.RotationAngle + FlipperVal, Mathf.SmoothStep(0f, to, bone.PosX));


			/*
			PlayerController playerController = sourceBeam.Owner as PlayerController;
			Vector2 vector = playerController.unadjustedAimPoint.XY() - playerController.CenterPosition;
			float num = vector.ToAngle();
			Vector2 barrel = playerController.CurrentGun.barrelOffset.transform.position;
			Vector2 b = bone.Position - barrel;
			Vector2 vector2 = new Vector2(0,0);
			List<int> boneList = new List<int>();
			BasicBeamController beaer = sourceBeam as BasicBeamController;
			
			if (beaer.GetBoneCount() >= 1 && UnityEngine.Random.value < 0.02f)
			{
				float RNG = (UnityEngine.Random.value > 0.5f) ? 90 : -90;
				beaer.GetIndexedBone(beaer.GetBoneCount() - 1).RotationAngle = beaer.GetIndexedBone(beaer.GetBoneCount() - 2).RotationAngle + RNG;
				Vector2 Point = MathToolbox.GetUnitOnCircle(beaer.GetIndexedBone(beaer.GetBoneCount() - 1).RotationAngle = beaer.GetIndexedBone(beaer.GetBoneCount() - 2).RotationAngle + RNG, 1f);
				vector2 = new Vector2(Point.x, Point.y);
			}

			return vector2;
			*/

		}
		private float FlipperVal = 90;


		private bool Initialized;

		private Vector2 InitialVerticalVector;
		private Vector2 LastPosition;
		private float currentAngle;

	}
}
//beam.GetBoneCount()
//beam.GetFinalBoneDirection()
//beam.GetIndexedBone(index)
//beam.GetIndexedBonePosition(index)
//beam.GetBonePosition(bone)