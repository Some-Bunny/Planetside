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
	public class MaintainDamageOnPierce : MonoBehaviour
	{
		//Thanks Nevernamed! :D
		public MaintainDamageOnPierce()
		{
			this.damageMultOnPierce = 1f;
			this.AmountOfPiercesBeforeFalloff = -1f;
		}

		public void Start()
		{
			this.m_projectile = base.GetComponent<Projectile>();
			if (this.m_projectile)
			{
				SpeculativeRigidbody specRigidbody = this.m_projectile.specRigidbody;
				specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePierce));
			}
		}
		private void HandlePierce(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			if (AmountOfPiercesBeforeFalloff != 0)
            {
				AmountOfPiercesBeforeFalloff--;
				FieldInfo field = typeof(Projectile).GetField("m_hasPierced", BindingFlags.Instance | BindingFlags.NonPublic);
				field.SetValue(myRigidbody.projectile, false);
				myRigidbody.projectile.baseData.damage *= this.damageMultOnPierce;
			}
		}
		public float AmountOfPiercesBeforeFalloff;
		public float damageMultOnPierce;
		private Projectile m_projectile;
	}
}
