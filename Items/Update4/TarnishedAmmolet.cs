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
using SaveAPI;

namespace Planetside
{
	public class TarnishedAmmolet : BlankModificationItem
	{

		public static void Init()
		{
			string name = "Tarnished Ammolet";
			string resourcePath = "Planetside/Resources/tarnishedammolet.png";
			GameObject gameObject = new GameObject(name);
			TarnishedAmmolet ammolet = gameObject.AddComponent<TarnishedAmmolet>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Eroded With Time";
			string longDesc = "This ammolet has been subjected to the rare effects of time, and has completely tarnished.\n\nDespite this, its ability to affect enemies has only gotten stronger.";
			ammolet.SetupItem(shortDesc, longDesc, "psog");
			ammolet.quality = PickupObject.ItemQuality.A;
			ammolet.AddPassiveStatModifier(PlayerStats.StatType.AdditionalBlanksPerFloor, 1f, StatModifier.ModifyMethod.ADDITIVE);
			ammolet.AddToSubShop(ItemBuilder.ShopType.OldRed, 1f);
			TarnishedAmmolet.TarnishedAmmoletID = ammolet.PickupObjectId;
			ItemIDs.AddToList(ammolet.PickupObjectId);
			SynergyAPI.SynergyBuilder.AddItemToSynergy(ammolet, CustomSynergyType.MINOR_BLANKABLES);
			SynergyAPI.SynergyBuilder.AddItemToSynergy(ammolet, CustomSynergyType.RELODESTAR);

		}

		private static int TarnishedAmmoletID;
		private static Hook BlankHook = new Hook(typeof(SilencerInstance).GetMethod("ProcessBlankModificationItemAdditionalEffects", BindingFlags.Instance | BindingFlags.NonPublic), typeof(TarnishedAmmolet).GetMethod("BlankModHook", BindingFlags.Instance | BindingFlags.Public), typeof(SilencerInstance));

		public void BlankModHook(Action<SilencerInstance, BlankModificationItem, Vector2, PlayerController> orig, SilencerInstance silencer, BlankModificationItem bmi, Vector2 centerPoint, PlayerController user)
		{
			orig(silencer, bmi, centerPoint, user);
			try
			{
				if (user.HasPickupID(TarnishedAmmoletID))
				{
					RoomHandler currentRoom = user.CurrentRoom;
					if (currentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
					{
						foreach (AIActor aiactor in currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
						{
							if (aiactor.behaviorSpeculator != null)
							{
								aiactor.ApplyEffect(DebuffLibrary.Corrosion, 1f, null);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				ETGModConsole.Log(e.Message);
				ETGModConsole.Log(e.StackTrace);
			}
		}
	}
}



