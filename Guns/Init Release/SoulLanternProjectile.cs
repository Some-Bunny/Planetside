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
	internal class SoulLanternProjectile : MonoBehaviour
	{
		public SoulLanternProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile)
            {
				projectile.statusEffectsToApply = new List<GameActorEffect> { DebuffLibrary.Possessed };
				projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHit));
            }
        }
		
		private void HandleHit(Projectile projectile, SpeculativeRigidbody arg2, bool arg3)
		{
			if (arg2.aiActor != null && !arg2.healthHaver.IsDead && arg2.aiActor.behaviorSpeculator && !arg2.aiActor.IsHarmlessEnemy && arg2.aiActor != null)
			{
				projectile.AppliesPoison = true;
				projectile.PoisonApplyChance = 1f;
				projectile.healthEffect = DebuffLibrary.Possessed;
			}
		}
		private Projectile projectile;
	}
}

