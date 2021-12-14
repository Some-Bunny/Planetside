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
	public class InjectorRounds : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Injector Rounds";
			string resourceName = "Planetside/Resources/injectorrounds.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<InjectorRounds>();
			ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
			string shortDesc = "Now To Practice Medicine";
			string longDesc = "These rounds are custom-made to inject any ailments of an enemy deeper into it, causing it to burst when killed.\n\nAn experimental ammo type that would allow the storage of nutrients for ease of use, they fell out of sight quickly when people realised you literally had to shoot yourself in the foot to gain their benefit. Some crafty adventurers, however, improvised.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ProjectileSpeed, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);

			item.quality = PickupObject.ItemQuality.C;
			List<string> mandatoryConsoleIDs = new List<string>
			{
				"psog:injector_rounds",
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"plunger",
				"antibody"
			};
			CustomSynergies.Add("Thats How I Lost My Medical License", mandatoryConsoleIDs, optionalConsoleIDs, true);

			InjectorRounds.InjectorRoundsID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

		}
		public static int InjectorRoundsID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
				sourceProjectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(sourceProjectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHit));
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
		}
		private void PostProcessBeam(BeamController obj)
		{

			try
			{
				obj.projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(obj.projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHit));
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
			}
		}
		private void HandleHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
		{
			bool flag = arg2.aiActor != null  && !arg2.healthHaver.IsDead && arg2.aiActor.behaviorSpeculator && !arg2.aiActor.IsHarmlessEnemy && arg2.aiActor != null;
			if (flag)
			{
				InjectorRoundsComponent yee = arg2.aiActor.gameObject.GetOrAddComponent<InjectorRoundsComponent>();
				bool flagA = base.Owner.PlayerHasActiveSynergy("Thats How I Lost My Medical License");
				if (flagA)
				{
					yee.GoopPoolSize = 5;
				}
				else
                {
					yee.GoopPoolSize = 2.5f;
				}
			}
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeam -= this.PostProcessBeam;
			return result;
		}
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeam += this.PostProcessBeam;
		}

		protected override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
				base.Owner.PostProcessBeam -= this.PostProcessBeam;
			}
			base.OnDestroy();
		}
	}
}