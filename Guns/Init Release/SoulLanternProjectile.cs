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
            Projectile projectile = this.projectile;
            PlayerController playerController = projectile.Owner as PlayerController;
            Projectile component = base.gameObject.GetComponent<Projectile>();
            bool flag = component != null;
            bool flag2 = flag;
            if (flag2)
            {
				projectile.statusEffectsToApply = new List<GameActorEffect> { DebuffLibrary.Possessed };
				projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHit));
                //base.StartCoroutine(this.Speed(projectile));

            }
        }
		public IEnumerator Speed(Projectile projectile)
		{
			bool flag = projectile != null;
			bool flag3 = flag;
			if (flag3)
			{
				for (int i = 0; i < 15; i++)
				{
					projectile.baseData.speed += 1f;
					projectile.UpdateSpeed();
					yield return new WaitForSeconds(0.1f);
				}
			}
			yield break;
		}
		private void HandleHit(Projectile projectile, SpeculativeRigidbody arg2, bool arg3)
		{
			bool flag = arg2.aiActor != null && !arg2.healthHaver.IsDead && arg2.aiActor.behaviorSpeculator && !arg2.aiActor.IsHarmlessEnemy && arg2.aiActor != null;
			if (flag)
			{
				projectile.AppliesPoison = true;
				projectile.PoisonApplyChance = 1f;
				projectile.healthEffect = DebuffLibrary.Possessed;
			}

		}
		private Projectile projectile;
	}
}

