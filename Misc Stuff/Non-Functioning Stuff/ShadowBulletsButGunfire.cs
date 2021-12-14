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

namespace Planetside
{

    public class SpawnShit : MonoBehaviour
    {
		public PlayerController Owner
		{
			get
			{
				return this.m_owner;
			}
		}
		public void SpawngunfireBullet(Projectile obj)
		{
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			float num2 = 10f;
			List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
			bool flag5 = activeEnemies == null | activeEnemies.Count <= 0;
			{
				AIActor nearestEnemy = this.GetNearestEnemy(activeEnemies, player.sprite.WorldCenter, out num2, null);
				bool flag8 = nearestEnemy && nearestEnemy != null;
				if (flag8)
				{
					if (obj)
					{
						int pain = UnityEngine.Random.Range(-180, 180);
						Vector2 unitCenter3 = nearestEnemy.specRigidbody.HitboxPixelCollider.UnitCenter;
						Projectile projectile = player.CurrentGun.DefaultModule.finalProjectile;
						if (player.CurrentGun.DefaultModule.finalVolley != null)
						{
							projectile = player.CurrentGun.DefaultModule.finalVolley.projectiles[0].GetCurrentProjectile();
						}
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(projectile.gameObject, unitCenter3, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-180, 180)));
						gameObject.transform.position = gameObject.transform.position;
						Projectile component = gameObject.GetComponent<Projectile>();
						component.specRigidbody.Reinitialize();
						component.collidesWithPlayer = false;
						component.Owner = obj.Owner;
						component.Shooter = obj.Shooter;
					}
				}
			}
		}

		public AIActor GetNearestEnemy(List<AIActor> activeEnemies, Vector2 position, out float nearestDistance, string[] filter)
		{
			AIActor aiactor = null;
			nearestDistance = float.MaxValue;
			bool flag = activeEnemies == null;
			bool flag2 = flag;
			bool flag3 = flag2;
			bool flag4 = flag3;
			bool flag5 = flag4;
			AIActor result;
			if (flag5)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < activeEnemies.Count; i++)
				{
					AIActor aiactor2 = activeEnemies[i];
					bool flag6 = !aiactor2.healthHaver.IsDead && aiactor2.healthHaver.IsVulnerable;
					bool flag7 = flag6;
					if (flag7)
					{
						bool flag8 = filter == null || !filter.Contains(aiactor2.EnemyGuid);
						bool flag9 = flag8;
						if (flag9)
						{
							float num = Vector2.Distance(position, aiactor2.CenterPosition);
							bool flag10 = num < nearestDistance;
							bool flag11 = flag10;
							if (flag11)
							{
								nearestDistance = num;
								aiactor = aiactor2;
							}
						}
					}
				}
				result = aiactor;
			}
			return result;
		}
		protected PlayerController m_owner;
	}
}
