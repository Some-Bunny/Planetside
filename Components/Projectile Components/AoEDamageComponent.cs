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
using static ETGMod;

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
			this.AreaIncreasesWithProjectileSizeStat = false;
			this.DamageValuesAlsoScalesWithDamageStat = false;
		}

		private void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (projectile)
            {
                player = projectile.Owner as PlayerController;
                if (player)
                {
                    MultiplierScale = AreaIncreasesWithProjectileSizeStat == true && player ? player.stats.GetStatValue(PlayerStats.StatType.PlayerBulletScale) : 1;
                    MultiplierDamage = DamageValuesAlsoScalesWithDamageStat == true && player ? player.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
                }
            }
        }
        private float MultiplierScale = 1;
        private float MultiplierDamage = 1;

        private void Update()
		{
            this.elapsed += BraveTime.DeltaTime;
            if (this.elapsed > TimeBetweenDamageEvents)
            {
                elapsed = 0;
                List<AIActor> activeEnemies = projectile.transform.position.GetAbsoluteRoom().GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				Vector2 centerPosition = projectile.sprite.WorldCenter;
				if (activeEnemies != null)
                {
					for (int em = 0; em < activeEnemies.Count; em++)
					{
                        AIActor aiactor = activeEnemies[em];
                        if (aiactor != null)
                        {
                            if (Vector2.Distance(aiactor.CenterPosition, centerPosition) < Radius * MultiplierScale)
                            {
                                if (DealsDamage == true)
                                {
                                    if (aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null)
                                    {
                                        aiactor.healthHaver.ApplyDamage(DamageperDamageEvent * MultiplierDamage, Vector2.zero, "Aura", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                                    }
                                }
                                foreach (var Entry in debuffs)
                                {
                                    if (UnityEngine.Random.value < Entry.Value)
                                    {
                                        aiactor.ApplyEffect(Entry.Key, 1, null);
                                    }
                                }
                                foreach (var Entry in conditionalDebuffs)
                                {
                                    if (Entry.Value != null)
                                    {
                                        if (Entry.Value() == true)
                                        {
                                            aiactor.ApplyEffect(Entry.Key, 1, null);
                                        }
                                    }
                                }
                            }
                        }
                    }			
				}
			}
		}
		private Projectile projectile;
        private PlayerController player;

        public float Radius;
		public float DamageperDamageEvent;
		public float TimeBetweenDamageEvents;
		public bool DealsDamage;


        public Dictionary<GameActorEffect, float> debuffs = new Dictionary<GameActorEffect, float>();

        public Dictionary<GameActorEffect, Func<bool>> conditionalDebuffs = new Dictionary<GameActorEffect, Func<bool>>();


        public bool AreaIncreasesWithProjectileSizeStat;
		public bool DamageValuesAlsoScalesWithDamageStat;
		private float elapsed;
	}
}