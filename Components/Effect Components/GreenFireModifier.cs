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
	public class GreenFireModifier : MonoBehaviour
	{
		public GreenFireModifier()
		{
			this.chance = 0f;
			//this.doVFX = true;
		}
		GameActorFireEffect fireModifierEffect = new GameActorFireEffect
		{
			IsGreenFire = true,
			AffectsEnemies = true,
			DamagePerSecondToEnemies = 10f
		};
		private void Start()
		{
			this.m_projectile = base.GetComponent<Projectile>();
			bool flag = UnityEngine.Random.value <= this.chance;
			if (flag)
			{
				Projectile projectile = this.m_projectile;
				projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.ApplyStun));
			}
		}

		private void ApplyStun(Projectile bullet, SpeculativeRigidbody enemy, bool fatal)
		{

			//GameActorEffect greenFire = (PickupObjectDatabase.GetById(722) as Gun).DefaultModule.projectiles[0].fireEffect;
			if (enemy != null)
			{

				//enemy.aiActor.ApplyEffect(fireModifierEffect);
			}
		}
		private Projectile m_projectile;
		public float chance;
	}
}