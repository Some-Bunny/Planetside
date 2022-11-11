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
		public LaserChainsawProjectile()
		{
		}
        public void Start()
        {
			BasicBeamController component = this.gameObject.GetComponent<BasicBeamController>();;
			if (component != null)
			{
				component.projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(component.projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHit));
				component.projectile.OnWillKillEnemy = (Action<Projectile, SpeculativeRigidbody>)Delegate.Combine(component.projectile.OnWillKillEnemy, new Action<Projectile, SpeculativeRigidbody>(this.OnKill));
			}
		}
		private void Update()
        {
			
		}

        private void Player_PostProcessBeamTick(BeamController arg1, SpeculativeRigidbody arg2, float arg3)
        {
			arg1.ChanceBasedHomingRadius += 1000000000;
			arg1.ChanceBasedHomingAngularVelocity += 10000;
		}

        private void HandleHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
		{
			bool flag = arg2.aiActor != null && !arg2.healthHaver.IsBoss && !arg2.healthHaver.IsDead && arg2.aiActor.behaviorSpeculator && !arg2.aiActor.IsHarmlessEnemy && arg2.aiActor != null && UnityEngine.Random.value <= 0.11f;
			if (flag)
			{
				arg2.aiActor.behaviorSpeculator.Stun(0.25f, true);
				this.teleporter = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>();
				UnityEngine.Object.Instantiate<GameObject>(this.teleporter.TelefragVFXPrefab, arg2.specRigidbody.UnitCenter, Quaternion.identity);
			}
		}
		private TeleporterPrototypeItem teleporter;

		private void OnKill(Projectile arg1, SpeculativeRigidbody arg2)
		{
			bool flag = !arg2.aiActor.healthHaver.IsDead;
			if (flag)
			{
				PlayerController player = arg1.PossibleSourceGun.CurrentOwner as PlayerController;
				this.teleporter = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>();
				UnityEngine.Object.Instantiate<GameObject>(this.teleporter.TelefragVFXPrefab, arg2.specRigidbody.UnitCenter, Quaternion.identity);
				
				List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				Vector2 centerPosition = arg1.sprite.WorldCenter;
				if (activeEnemies != null)
				{
					foreach (AIActor aiactor in activeEnemies)
					{
						bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < 4 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null;
						if (ae)
						{
							arg2.aiActor.behaviorSpeculator.Stun(1, true);
						}
					}
				}
			}
		}

    }
}

