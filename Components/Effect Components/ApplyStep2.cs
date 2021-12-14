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
	public class ApplyStep2 : MonoBehaviour
	{
		public ApplyStep2()
		{
			this.chanceToslow = 1f;
		}

		private void Start()
		{
			this.m_projectile = base.GetComponent<Projectile>();
			bool flag = UnityEngine.Random.value <= this.chanceToslow;
			if (flag)
			{
				Projectile projectile = this.m_projectile;
				projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.ApplyStun));
			}
		}

		private void ApplyStun(Projectile bullet, SpeculativeRigidbody enemy, bool fatal)
		{
			if (enemy != null)
            {
				enemy.gameObject.GetOrAddComponent<Step2>();
			}
		}
		private Projectile m_projectile;
		public float chanceToslow;
		public float SpeedMultiplier;

		public bool doTint;
		public Color Color;
		public float slowLength;
	}
}