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
	public class PierceDeadActors : MonoBehaviour
	{
		private void Start()
		{
			this.m_projectile = base.GetComponent<Projectile>();
			SpeculativeRigidbody specRigidbody = this.m_projectile.specRigidbody;
			specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PreCollision));
		}

		private void PreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			bool flag = myRigidbody != null && otherRigidbody != null;
			if (flag)
			{
				bool flag2 = otherRigidbody.healthHaver != null && otherRigidbody.healthHaver.IsDead;
				if (flag2)
				{
					PhysicsEngine.SkipCollision = true;
				}
			}
		}

		private Projectile m_projectile;
	}
}