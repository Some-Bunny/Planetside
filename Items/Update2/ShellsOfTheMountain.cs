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
	public class ShellsOfTheMountain : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Shells Of The Mountain";
			//string resourceName = "Planetside/Resources/bulletofthemountain.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<ShellsOfTheMountain>();

            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("bulletofthemountain"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Forged For The Mountain King";
			string longDesc = "No matter how steep it is, the bullets climb to the top to stay level with their foes.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.A;
			ShellsOfTheMountain.ShellsOfTheMountainID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

		}
		public static int ShellsOfTheMountainID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			try
			{
				float num = 1f;
				GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
				if (lastLoadedLevelDefinition != null)
				{
					num = lastLoadedLevelDefinition.enemyHealthMultiplier - 1;
				}
				sourceProjectile.baseData.damage = sourceProjectile.baseData.damage*((num/3)+1);

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
				float num = 1f;
				GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
				if (lastLoadedLevelDefinition != null)
				{
					num = lastLoadedLevelDefinition.enemyHealthMultiplier - 1;
				}
				obj.projectile.baseData.damage = obj.projectile.baseData.damage * ((num / 3) + 1);
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

		public override void OnDestroy()
		{
			if (base.Owner)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
				base.Owner.PostProcessBeam -= this.PostProcessBeam;
			}
			base.OnDestroy();
		}
	}
}