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
	public class PingPongComponent : BraveBehaviour
	{
		public void Start()
		{
			float[] angles = { 45, 135, 225, 135 };
			PingPongAroundBehavior yeah = new PingPongAroundBehavior()
			{
				motionType = PingPongAroundBehavior.MotionType.Diagonals,
				startingAngles = angles
			};
			base.aiActor.MovementSpeed = 9;
			base.aiActor.behaviorSpeculator.MovementBehaviors.Add(yeah);
			foreach (MovementBehaviorBase att in base.aiActor.behaviorSpeculator.MovementBehaviors)
			{
				if (att is PingPongAroundBehavior) { }
				else
                {
					base.aiActor.behaviorSpeculator.MovementBehaviors.Remove(att);
				}
			}
		}
	}
}