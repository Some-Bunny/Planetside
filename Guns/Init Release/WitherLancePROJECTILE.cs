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
	internal class WitherLancePROJECTILE : MonoBehaviour
	{
		public WitherLancePROJECTILE()
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
                projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHit));

            }
        }
		private void HandleHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
		{
			PlayerController player = projectile.Owner as PlayerController;

			float scalar = 0f;
			float scalarboss = 0f;
			scalar = (player.stats.GetStatValue(PlayerStats.StatType.Damage) / 8);
			scalarboss = (player.stats.GetStatValue(PlayerStats.StatType.Damage) / 40);
			if (arg2.aiActor != null && arg2.aiActor.IsBlackPhantom)
			{
				scalar *= 2;
				scalarboss *= 2;

			}
			bool flag = arg2.aiActor != null && !arg2.healthHaver.IsBoss && !arg2.healthHaver.IsDead && arg2.aiActor.behaviorSpeculator && !arg2.aiActor.IsHarmlessEnemy && arg2.aiActor != null;
			if (flag)
			{
				arg2.healthHaver.SetHealthMaximum(arg2.healthHaver.GetMaxHealth() * (1 - scalar));

			}
			bool e = arg2.aiActor != null && arg2.healthHaver.IsBoss && !arg2.healthHaver.IsDead && arg2.aiActor.behaviorSpeculator && !arg2.aiActor.IsHarmlessEnemy && arg2.aiActor != null;
			if (e)
			{
				arg2.healthHaver.SetHealthMaximum(arg2.healthHaver.GetMaxHealth() * (1 - scalarboss));

			}
		}

		private Projectile projectile;
	}
}

