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
	public class Unstabullets : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Unsta-bullets";
			//string resourceName = "Planetside/Resources/unstab-ullets.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<Unstabullets>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("unstab-ullets"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Quantum Mechanics";
			string longDesc = "These bullets were forged with lead from beyond the Curtain." +
				"\n\nThey seem to shift their positions fairly unpredictably.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			item.quality = PickupObject.ItemQuality.B;
			item.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);
			item.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);

			Unstabullets.UnstabulletsID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

		}
		public static int UnstabulletsID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			PlayerController owner = base.Owner;
			try
			{
				
				int c = UnityEngine.Random.Range(0, 5);
				switch (c)
				{
					case 0:
						float num2 = 10f;
						{
							if (owner.IsInCombat)
							{
                                List<AIActor> activeEnemies = owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                                if (activeEnemies != null)
                                {
                                    AIActor nearestEnemy = this.GetNearestEnemy(activeEnemies, owner.sprite.WorldCenter, out num2, null);
                                    if (nearestEnemy != null)
                                    {
                                        PierceProjModifier spook = sourceProjectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                                        spook.penetration += 1;

                                        StaticVFXStorage.SpiratTeleportVFX.SpawnAtPosition(sourceProjectile.transform.position);
                                        Vector2 unitCenter3 = nearestEnemy.specRigidbody.HitboxPixelCollider.UnitCenter;
                                        sourceProjectile.transform.position = unitCenter3;
                                        sourceProjectile.baseData.damage *= 0.75f;
                                        StaticVFXStorage.SpiratTeleportVFX.SpawnAtPosition(sourceProjectile.transform.position);
                                    }
                                }
                            }
						}
						break;
					case 1:
						if (sourceProjectile.GetComponent<RecursionPreventer>())
						{
							return;
						}
                        sourceProjectile.StartCoroutine(DoProjectileSpilt(sourceProjectile));
						break;
				}
			
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				//ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
		}


		public IEnumerator DoProjectileSpilt(Projectile projectile)
		{
			float e = 0;

			projectile.ResetDistance();

			bool lastData = projectile.baseData.UsesCustomAccelerationCurve;
            float lastDur = projectile.baseData.CustomAccelerationCurveDuration;
            AnimationCurve lastCurve = projectile.baseData.AccelerationCurve;

			projectile.m_timeElapsed = 0;
            projectile.baseData.UsesCustomAccelerationCurve = true;
			projectile.baseData.CustomAccelerationCurveDuration = 1f;
			projectile.baseData.AccelerationCurve = AnimationCurve.EaseInOut(0, 1, 0.625f, 0);


            while (e < 0.5f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }

            for (int i = 1; i < 4; i++)
			{
                ParticleBase.EmitParticles("WaveParticleInverse", 1, new ParticleSystem.EmitParams()
                {
                    position = projectile.sprite.WorldCenter,
                    startSize = projectile.sprite.GetBounds().size.magnitude * 1 + (0.5f * i),
                    rotation = 0,
                    startLifetime = 0.5f,
                    startColor = Color.white * 0.8f
                });
                e = 0;
                while (e < 0.1666f)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
            }
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = projectile.sprite.WorldCenter,
                startSize = projectile.sprite.GetBounds().size.magnitude * 8,
                rotation = 0,
                startLifetime = 0.3f,
                startColor = Color.white * 0.8f
            });

			for (int i = 0; i < 2; i++)
			{
                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(projectile.gameObject, projectile.sprite.WorldCenter, Quaternion.Euler(0f, 0f, MathToolbox.ToAngle(projectile.Direction) + (i == 0 ? 90 : -90)), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {
                    component.baseData.UsesCustomAccelerationCurve = lastData;
                    component.baseData.CustomAccelerationCurveDuration = lastDur;
                    component.baseData.AccelerationCurve = lastCurve;

                    component.gameObject.AddComponent<RecursionPreventer>();
                    component.Owner = this.Owner;
                    component.Shooter = Owner.specRigidbody;
                    component.baseData.damage *= 0.6f;
					component.RuntimeUpdateScale(0.7f);
                    component.UpdateSpeed();
                    component.ResetDistance();


                }
            }
            UnityEngine.Object.Destroy(projectile.gameObject);



            yield break;
		}

		public AIActor GetNearestEnemy(List<AIActor> activeEnemies, Vector2 position, out float nearestDistance, string[] filter)
		{
			AIActor aiactor = null;
			nearestDistance = float.MaxValue;
			AIActor result;
			if (activeEnemies == null)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < activeEnemies.Count; i++)
				{
					AIActor aiactor2 = activeEnemies[i];
					if (!aiactor2.healthHaver.IsDead && aiactor2.healthHaver.IsVulnerable)
					{
						if (filter == null || !filter.Contains(aiactor2.EnemyGuid))
						{
							float num = Vector2.Distance(position, aiactor2.CenterPosition);
							if (num < nearestDistance)
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

		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			return result;
		}
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
		}

		public override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
			}
			base.OnDestroy();
		}
		private float m_currentAngle;
		private float m_currentDistance;
		private Vector2 aimpoint;
	}
}