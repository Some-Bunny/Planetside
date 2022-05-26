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
	internal class WitherLanceProjectile : MonoBehaviour
	{
		public WitherLanceProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile)
			{
                projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHit));

            }
        }
		private void HandleHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
		{
			PlayerController player = projectile.Owner as PlayerController;

			float scalar = (player.stats.GetStatValue(PlayerStats.StatType.Damage) / 3);
			float scalarboss = (player.stats.GetStatValue(PlayerStats.StatType.Damage) / 25);


			if (arg2.aiActor != null && arg2.aiActor.IsBlackPhantom)
			{
				scalar *= 2;
				scalarboss *= 2;
			}
			bool flag = arg2.aiActor != null && !arg2.healthHaver.IsBoss && !arg2.healthHaver.IsDead && arg2.aiActor.behaviorSpeculator && !arg2.aiActor.IsHarmlessEnemy && arg2.aiActor != null;
			if (flag)
			{
				if ((arg2.healthHaver.minimumHealth > arg2.healthHaver.GetMaxHealth() - scalar))
                {
					arg2.healthHaver.SetHealthMaximum(arg2.healthHaver.GetMaxHealth() * (1 - scalar));

				}
				else
                {
					projectile.baseData.damage *= 3.5f;
				}
			}
			bool e = arg2.aiActor != null && arg2.healthHaver.IsBoss && !arg2.healthHaver.IsDead && arg2.aiActor.behaviorSpeculator && !arg2.aiActor.IsHarmlessEnemy && arg2.aiActor != null;
			if (e)
			{
				if ((arg2.healthHaver.minimumHealth > arg2.healthHaver.GetMaxHealth() - scalarboss))
                {
					arg2.healthHaver.SetHealthMaximum(arg2.healthHaver.GetMaxHealth() * (1 - scalarboss));
				}
				else
                {
					projectile.baseData.damage *= 3.5f;
                }

			}
		}
		private Projectile projectile;
	}
}

