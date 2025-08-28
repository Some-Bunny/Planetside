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
using System.Timers;

using UnityEngine.Serialization;

namespace Planetside
{
	internal class LaserChainsawProjectile : MonoBehaviour
	{
        public void Start()
        {
			BasicBeamController component = this.gameObject.GetComponent<BasicBeamController>();;
			if (component != null)
			{
				component.projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(component.projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHit));
				component.projectile.OnWillKillEnemy = (Action<Projectile, SpeculativeRigidbody>)Delegate.Combine(component.projectile.OnWillKillEnemy, new Action<Projectile, SpeculativeRigidbody>(this.OnKill));
			}
		}

        private void HandleHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
		{
			if (arg2.aiActor != null && !arg2.healthHaver.IsBoss && !arg2.healthHaver.IsDead && arg2.aiActor.behaviorSpeculator && !arg2.aiActor.IsHarmlessEnemy && arg2.aiActor != null && UnityEngine.Random.value <= 0.11f)
			{
				arg2.aiActor.behaviorSpeculator.Stun(0.15f, true);
				var effect = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportTelefragVFX, arg2.specRigidbody.UnitCenter, Quaternion.identity);
				Destroy(effect, 3);
			}
		}

		private void OnKill(Projectile arg1, SpeculativeRigidbody arg2)
		{
			if (!arg2.aiActor.healthHaver.IsDead)
			{
				PlayerController player = arg1.PossibleSourceGun.CurrentOwner as PlayerController;
                var effect = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportTelefragVFX, arg2.specRigidbody.UnitCenter, Quaternion.identity);
                Destroy(effect, 3);

                List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				Vector2 centerPosition = arg1.sprite.WorldCenter;
				if (activeEnemies != null)
				{
					foreach (AIActor aiactor in activeEnemies)
					{
						if (Vector2.Distance(aiactor.CenterPosition, centerPosition) < 4 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null)
						{
							arg2.aiActor.behaviorSpeculator.Stun(1, true);
						}
					}
				}
			}
		}
    }
}

