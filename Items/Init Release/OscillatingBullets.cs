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
	public class OscilaltingBullets : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Oscillating Bullets";
			string resourceName = "Planetside/Resources/oscillatingbullets.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<OscilaltingBullets>();
			ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
			string shortDesc = "Back & Forth";
			string longDesc = "These bullets were forged by a smith who couldn't decide how to forge them, so they ended up forged half-and-half in different ways." +
				"\n\nThis makes them fight over the bullets speed on how they should move.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 1.25f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RangeMultiplier, 2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			item.quality = PickupObject.ItemQuality.A;
			OscilaltingBullets.OscillatingBulletsID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

		}
		public static int OscillatingBulletsID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
				sourceProjectile.OverrideMotionModule = new OscillatingeMotionModule();
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
				obj.projectile.OverrideMotionModule = new OscillatingeMotionModule();
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
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