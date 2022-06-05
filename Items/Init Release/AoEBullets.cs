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

//Garbage Code Incoming
namespace Planetside
{
    public class AoEBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Aura Bullets";
            string resourceName = "Planetside/Resources/aurabullets.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<AoEBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Radiant";
            string longDesc = "Makes bullets deal damage to enemies near them." +
                "\n\nThese bullets contain a very rare and powerful radioactive isotope. Don't lick them!";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ProjectileSpeed, 0.7f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			item.quality = PickupObject.ItemQuality.B;
			List<string> mandatoryConsoleIDs = new List<string>
			{
				"psog:aura_bullets",
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"irradiated_lead",
				"uranium_ammolet",
				"monster_blood",
				"poison_vial",
				"big_boy",
				"shotgrub",
				"plunger",
				"plague_pistol"
			};
			CustomSynergies.Add("Khh..k k k k", mandatoryConsoleIDs, optionalConsoleIDs, true);
			List<string> optionalConsoleID1s = new List<string>
			{
				"hot_lead",
				"copper_ammolet",
				"napalm_strike",
				"ring_of_fire_resistance",
				"flame_hand",
				"phoenix",
				"pitchfork"
			};
			CustomSynergies.Add("Handle The Heat", mandatoryConsoleIDs, optionalConsoleID1s, true);
			List<string> optionalConsoleID2s = new List<string>
			{
				"frost_bullets",
				"frost_ammolet",
				"heart_of_ice",
				"ice_cube",
				"ice_bomb",
				"glacier"
			};
			CustomSynergies.Add("Below Zero", mandatoryConsoleIDs, optionalConsoleID2s, true);
			AoEBullets.AuraBulletsID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

		}
		public static int AuraBulletsID;

		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
				if (sourceProjectile.baseData.range >= 3)
				{
					PlayerController player = base.Owner;
					AoEDamageComponent Values = sourceProjectile.gameObject.AddComponent<AoEDamageComponent>();
					Values.DamageperDamageEvent = ((sourceProjectile.baseData.damage * 0.2f)+0.5f);
					Values.Radius = 2.2f;
					Values.TimeBetweenDamageEvents = 0.2f;
					Values.DealsDamage = true;
					Values.AreaIncreasesWithProjectileSizeStat = true;
					Values.DamageValuesAlsoScalesWithDamageStat = true;
					Values.EffectProcChance = 0.05f;
					if (player.PlayerHasActiveSynergy("Khh..k k k k"))
					{
						Values.InflictsPoison = true;
					}
					if (player.PlayerHasActiveSynergy("Handle The Heat"))
					{
						Values.InflictsFire = true;
					}
					if (player.PlayerHasActiveSynergy("Below Zero"))
					{
						Values.InflictsFreeze = true;
					}
					this.ShockRing(sourceProjectile);
				}
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
			}
		}
		private void EndRingEffect(Projectile projectile)
		{
			this.m_radialIndicator.EndEffect();
		}
		private void ShockRing(Projectile projectile)
		{
			PlayerController player = projectile.Owner as PlayerController;
			float num = 0f;
			num = (player.stats.GetStatValue(PlayerStats.StatType.PlayerBulletScale));
			this.m_radialIndicator = ((GameObject)UnityEngine.Object.Instantiate(StaticVFXStorage.RadialRing, projectile.sprite.WorldCenter, Quaternion.identity, projectile.transform)).GetComponent<HeatIndicatorController>();
			this.m_radialIndicator.CurrentColor = Color.white.WithAlpha(4f);
			this.m_radialIndicator.CurrentRadius = 1.75f* num;

		}
		private HeatIndicatorController m_radialIndicator;
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			return result;
		}
		protected override void OnDestroy()
		{
			if (Owner != null)
			{
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
			}
			base.OnDestroy();
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
		}

	}
}


