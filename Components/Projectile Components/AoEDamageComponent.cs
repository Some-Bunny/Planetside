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
	public class AoEDamageComponent : MonoBehaviour
	{
		public AoEDamageComponent()
		{
			this.Radius = 1;
			this.DamageperDamageEvent = 1;
			this.TimeBetweenDamageEvents = 1;
			this.DealsDamage = true;
			this.InflictsFire = false;
			this.InflictsPoison = false;
			this.InflictsFreeze = false;
			this.AreaIncreasesWithProjectileSizeStat = false;
			this.DamageValuesAlsoScalesWithDamageStat = false;
			this.HeatStrokeSynergy = false;
			this.EffectProcChance = 0.1f;
		}

		private void Awake()
		{
			if (this.m_projectile != null)
            {
				this.m_projectile = base.GetComponent<Projectile>();
			}
		}

		private void Update()
		{
			bool flag = this.m_projectile == null;
			if (flag)
			{
				this.m_projectile = base.GetComponent<Projectile>();
			}
			this.elapsed += BraveTime.DeltaTime;
			bool flag3 = this.elapsed > TimeBetweenDamageEvents;
			if (flag3)
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				float num = 1f;
				float dmg = 1f;
				if (AreaIncreasesWithProjectileSizeStat == true)
				{
					num = (player.stats.GetStatValue(PlayerStats.StatType.PlayerBulletScale));
				}
				else
				{
					num = 1;
				}
				if (DamageValuesAlsoScalesWithDamageStat == true)
				{
					dmg = (player.stats.GetStatValue(PlayerStats.StatType.Damage));
				}
				else
				{
					dmg = 1f;
				}
				List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				Vector2 centerPosition = m_projectile.sprite.WorldCenter;
				if (activeEnemies != null)
                {
					foreach (AIActor aiactor in activeEnemies)
					{
						if (DealsDamage == true)
						{
							bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < Radius * num && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null;
							if (ae)
							{
								aiactor.healthHaver.ApplyDamage(DamageperDamageEvent*dmg, Vector2.zero, "fuckigjmnkbjnbbnjbnjnjbnjbnjbnjbjn", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
							}
						}

						if (InflictsFreeze == true)
						{
							float RNG = UnityEngine.Random.Range(0.00f, 1.00f);
							if (RNG <= EffectProcChance)
                            {
								BulletStatusEffectItem Freezzecomponent = PickupObjectDatabase.GetById(278).GetComponent<BulletStatusEffectItem>();
								GameActorFreezeEffect gameActorFreeze = Freezzecomponent.FreezeModifierEffect;
								bool peep = Vector2.Distance(aiactor.CenterPosition, centerPosition) < Radius * num && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null;
								if (peep)
								{
									aiactor.ApplyEffect(gameActorFreeze, 0.5f, null);
								}
							}
						}
						if (InflictsPoison == true)
						{
							float RNG = UnityEngine.Random.Range(0.00f, 1.00f);
							if (RNG <= EffectProcChance)
                            {
								BulletStatusEffectItem Poisoncomponent = PickupObjectDatabase.GetById(204).GetComponent<BulletStatusEffectItem>();
								GameActorHealthEffect gameActorPoison = Poisoncomponent.HealthModifierEffect;
								bool kenki = Vector2.Distance(aiactor.CenterPosition, centerPosition) < Radius * num && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null;
								if (kenki)
								{
									aiactor.ApplyEffect(gameActorPoison, 1f, null);

								}
							}
								
						}
						if (InflictsFire == true)
						{
							float RNG = UnityEngine.Random.Range(0.00f, 1.00f);
							if (RNG <= EffectProcChance)
                            {
								BulletStatusEffectItem Firecomponent = PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>();
								GameActorFireEffect gameActorFire = Firecomponent.FireModifierEffect;
								bool banko = Vector2.Distance(aiactor.CenterPosition, centerPosition) < Radius * num && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null;
								if (banko)
								{
									aiactor.ApplyEffect(gameActorFire, 1f, null);
									if (HeatStrokeSynergy == true)
									{
										bool flagA = player.PlayerHasActiveSynergy("Praise The Gun!");
										if (flagA)
										{
											var Frail = aiactor.transform.Find("heatStrokeVFX");
											bool fra = Frail == null;
											if (fra)
											{
												aiactor.ApplyEffect(DebuffLibrary.HeatStroke, 1f, null);
											}
										}
									}
								}
							}
								
						}
					}

				}
				this.elapsed = 0f;
			}

		}
		private Projectile m_projectile;

		public float Radius;
		public float DamageperDamageEvent;
		public float TimeBetweenDamageEvents;
		public bool DealsDamage;
		public bool InflictsFire;
		public bool InflictsPoison;
		public bool InflictsFreeze;
		public bool AreaIncreasesWithProjectileSizeStat;
		public bool DamageValuesAlsoScalesWithDamageStat;
		public bool HeatStrokeSynergy;
		public float EffectProcChance;

		private float elapsed;
	}
}