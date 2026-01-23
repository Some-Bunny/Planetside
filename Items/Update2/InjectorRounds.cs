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
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<InjectorRounds>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("injectorrounds"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Now To Practice Medicine";
			string longDesc = "These custom-made rounds pump in any ailments or debuffs into your enemies, causing it to burst into a pool when killed. Is also pointer and aerodynamic!\n\nAn experimental ammo type that would allow the storage of nutrients for ease of use, they fell out of sight quickly when people realised you literally had to shoot yourself in the foot to gain their benefit. Some crafty adventurers, however, improvised.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ProjectileSpeed, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            item.AddToSubShop(ItemAPI.ItemBuilder.ShopType.Trorc, 1);

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
			item.SetupUnlockOnFlag(GungeonFlags.BOSSKILLED_BLOBULORD, true);


			InjectorRounds.InjectorRoundsID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

			templateDef = ScriptableObject.CreateInstance<GoopDefinition>();
			templateDef.baseColor32 = new Color32(120, 30, 10, 255);
			templateDef.goopTexture = ResourceExtractor.GetTextureFromResource("Planetside/Resources/goop_standard_base_001.png");
			templateDef.lifespan = 7;
		}

		public static GoopDefinition templateDef;
		public static int InjectorRoundsID;


        public static Dictionary<GameActorEffect, string> DebuffKeys = new Dictionary<GameActorEffect, string>()
		{
        {DebuffStatics.irradiatedLeadEffect, "poison" },
        {DebuffStatics.hotLeadEffect, "fire" },
        {DebuffStatics.cheeseeffect, "cheese" },
        {DebuffStatics.charmingRoundsEffect, "charm" },
        {DebuffStatics.frostBulletsEffect, "freeze" },
        {DebuffStatics.chaosBulletsFreeze, "freeze" },
        {DebuffStatics.greenFireEffect, "hellfire" },
        {DebuffLibrary.Possessed, "possessed" },
        {DebuffLibrary.Frailty, "frailty" },
        {DebuffLibrary.Corrosion, "tarnish" },
		};
        public static List<GameActorEffect> BlacklistedKeys = new List<GameActorEffect>()
		{
        DebuffLibrary.HeatStroke,
        DebuffLibrary.brokenArmor,
        DebuffLibrary.Holy,
		};
        public static List<string> BlacklistedNames= new List<string>()
        {
        "jamBuff",
		"leadBuff",
        "blob",
        "web",
        "Infection",
        "InfectionBoss",
        "BrainHost",
        "broken Armor"
        };

        public static Dictionary<string, GoopDefinition> GoopKeys = new Dictionary<string, GoopDefinition>()
		{
        {"poison",  EasyGoopDefinitions.PoisonDef},
        {"fire",  EasyGoopDefinitions.FireDef},
        {"cheese",  EasyGoopDefinitions.CheeseDef},
        {"charm",  EasyGoopDefinitions.CharmGoopDef},
        {"freeze",  EasyGoopDefinitions.WaterGoop},
        {"hellfire",  EasyGoopDefinitions.GreenFireDef},
        {"possessed",  DebuffLibrary.PossesedPuddle},
        {"frailty",  DebuffLibrary.FrailPuddle},
        {"tarnish",  DebuffLibrary.TarnishedGoop},
		};

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

		public override void OnDestroy()
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