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
	public class BulletSlowModifier : MonoBehaviour
	{
		public BulletSlowModifier()
		{
			this.chanceToslow = 0f;
			this.slowLength = 1f;
			//this.doVFX = true;
			this.SpeedMultiplier = 1;
			this.Color = Color.white;
			this.doTint = false;
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
				EasyApplyDirectSlow.ApplyDirectSlow(enemy.gameActor, slowLength, SpeedMultiplier, Color, Color, EffectResistanceType.None, "Slonedown", doTint, doTint);
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