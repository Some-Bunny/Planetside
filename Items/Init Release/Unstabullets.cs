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
			string resourceName = "Planetside/Resources/unstab-ullets.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<Unstabullets>();
			ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
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
				
				Unstabullets.Char = UnityEngine.Random.Range(0, 5);
				switch (Unstabullets.Char)
				{
					case 1:
						float num2 = 10f;
						{
							bool isInCombat = owner.IsInCombat;
							if (isInCombat)
							{
								{
									List<AIActor> activeEnemies = owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
									bool flag5 = activeEnemies == null | activeEnemies.Count <= 0;
									{
										AIActor nearestEnemy = this.GetNearestEnemy(activeEnemies, owner.sprite.WorldCenter, out num2, null);
										bool flag8 = nearestEnemy && nearestEnemy != null;
										if (flag8)
										{
											Vector2 unitCenter3 = nearestEnemy.specRigidbody.HitboxPixelCollider.UnitCenter;
											sourceProjectile.transform.position = unitCenter3;
											sourceProjectile.baseData.damage *= 0.75f;
										}
									}
								}
							}
						}
						break;
					case 2:
						bool flag = BraveInput.GetInstanceForPlayer(owner.PlayerIDX).IsKeyboardAndMouse(false);
						if (flag)
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
						break;
				}
			
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
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

		protected override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
			}
			base.OnDestroy();
		}
		public static int Char = 0;
		private float m_currentAngle;
		private float m_currentDistance;
		private Vector2 aimpoint;
	}
}