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
	internal class ShockChainProjectile : MonoBehaviour
	{
		public ShockChainProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            Projectile projectile = this.projectile;
            PlayerController playerController = projectile.Owner as PlayerController;
            Projectile component = base.gameObject.GetComponent<Projectile>();
            bool flag = component != null;
            bool flag2 = flag;
            if (flag2)
            {
				float ElectricDamage = 10.5f;
				bool flagA = playerController.PlayerHasActiveSynergy("Single A");
				if (flagA)
				{
					ElectricDamage *= 2;
				}
				float dmg = (playerController.stats.GetStatValue(PlayerStats.StatType.Damage));

				ComplexProjectileModifier complexProjectileModifier = PickupObjectDatabase.GetById(298) as ComplexProjectileModifier;
				ChainLightningModifier orAddComponent = projectile.gameObject.GetOrAddComponent<ChainLightningModifier>();
				orAddComponent.LinkVFXPrefab = complexProjectileModifier.ChainLightningVFX;
				orAddComponent.damageTypes = complexProjectileModifier.ChainLightningDamageTypes;
				orAddComponent.maximumLinkDistance = 100;
				orAddComponent.damagePerHit = ElectricDamage * dmg;
				orAddComponent.damageCooldown = complexProjectileModifier.ChainLightningDamageCooldown;
				orAddComponent.UsesDispersalParticles = true;
				orAddComponent.DispersalParticleSystemPrefab = complexProjectileModifier.ChainLightningDispersalParticles;
				orAddComponent.DispersalDensity = complexProjectileModifier.ChainLightningDispersalDensity;
				orAddComponent.DispersalMinCoherency = complexProjectileModifier.ChainLightningDispersalMinCoherence;
				orAddComponent.DispersalMaxCoherency = complexProjectileModifier.ChainLightningDispersalMaxCoherence;
				orAddComponent.UsesDispersalParticles = true;
				orAddComponent.CanChainToAnyProjectile = false;
			}
        }
        

        private Projectile projectile;
	}
}

