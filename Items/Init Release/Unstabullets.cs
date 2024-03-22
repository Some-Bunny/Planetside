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
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 1.20f, StatModifier.ModifyMethod.MULTIPLICATIVE);
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
				
				int c = UnityEngine.Random.Range(0, 7);
				switch (c)
				{
					case 1:
						float num2 = 10f;
						{
							if (owner.IsInCombat)
							{
                                List<AIActor> activeEnemies = owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                                if (activeEnemies == null | activeEnemies.Count < 0)
                                {
                                    AIActor nearestEnemy = this.GetNearestEnemy(activeEnemies, owner.sprite.WorldCenter, out num2, null);
                                    if (nearestEnemy != null)
                                    {
										StaticVFXStorage.SpiratTeleportVFX.SpawnAtPosition(sourceProjectile.transform.position);
                                        Vector2 unitCenter3 = nearestEnemy.specRigidbody.HitboxPixelCollider.UnitCenter;
                                        sourceProjectile.transform.position = unitCenter3;
                                        sourceProjectile.baseData.damage *= 0.6f;
                                        StaticVFXStorage.SpiratTeleportVFX.SpawnAtPosition(sourceProjectile.transform.position);

                                    }
                                }
                            }
						}
						break;
					case 2:
                        StaticVFXStorage.SpiratTeleportVFX.SpawnAtPosition(sourceProjectile.transform.position);
                        if (BraveInput.GetInstanceForPlayer(owner.PlayerIDX).IsKeyboardAndMouse(false))
						{
							this.aimpoint = owner.unadjustedAimPoint.XY();
						}
						else
						{
							BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(owner.PlayerIDX);
							Vector2 a = owner.CenterPosition + (Quaternion.Euler(0f, 0f, this.m_currentAngle) * Vector2.right).XY() * this.m_currentDistance;
							a += instanceForPlayer.ActiveActions.Aim.Vector * 8f * BraveTime.DeltaTime;
							this.m_currentAngle = BraveMathCollege.Atan2Degrees(a - owner.CenterPosition);
							this.m_currentDistance = Vector2.Distance(a, owner.CenterPosition);
							this.m_currentDistance = Mathf.Min(this.m_currentDistance, 15f);
							this.aimpoint = owner.CenterPosition + (Quaternion.Euler(0f, 0f, this.m_currentAngle) * Vector2.right).XY() * this.m_currentDistance;
						}
						sourceProjectile.transform.position = aimpoint;
                        StaticVFXStorage.SpiratTeleportVFX.SpawnAtPosition(sourceProjectile.transform.position);
                        break;
				}
			
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				//ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
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