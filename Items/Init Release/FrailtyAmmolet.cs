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
	public class FrailtyAmmolet : BlankModificationItem
	{

		public static void Init()
		{
			string name = "Frailty Ammolet";
			string resourcePath = "Planetside/Resources/frailtyammolet.png";
			GameObject gameObject = new GameObject(name);
			FrailtyAmmolet ammolet = gameObject.AddComponent<FrailtyAmmolet>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Untempered";
			string longDesc = "An ammolet that has been soaked with an exotic toxin. The energy released by blanks causes it to expel some of its toxins in a gaseous form.";
			ammolet.SetupItem(shortDesc, longDesc, "psog");
			ammolet.quality = PickupObject.ItemQuality.B;
			ammolet.AddPassiveStatModifier(PlayerStats.StatType.AdditionalBlanksPerFloor, 1f, StatModifier.ModifyMethod.ADDITIVE);
			ammolet.AddToSubShop(ItemBuilder.ShopType.OldRed, 1f);
			FrailtyAmmolet.FrailtyAmmoletID = ammolet.PickupObjectId;
			ItemIDs.AddToList(ammolet.PickupObjectId);
			SynergyAPI.SynergyBuilder.AddItemToSynergy(ammolet, CustomSynergyType.MINOR_BLANKABLES);
			SynergyAPI.SynergyBuilder.AddItemToSynergy(ammolet, CustomSynergyType.RELODESTAR);

			ammolet.SetupUnlockOnCustomFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED, true);
		}

		private static int FrailtyAmmoletID;
		private static Hook BlankHook = new Hook(typeof(SilencerInstance).GetMethod("ProcessBlankModificationItemAdditionalEffects", BindingFlags.Instance | BindingFlags.NonPublic), typeof(FrailtyAmmolet).GetMethod("BlankModHook", BindingFlags.Instance | BindingFlags.Public), typeof(SilencerInstance));

		public void BlankModHook(Action<SilencerInstance, BlankModificationItem, Vector2, PlayerController> orig, SilencerInstance silencer, BlankModificationItem bmi, Vector2 centerPoint, PlayerController user)
		{
			orig(silencer, bmi, centerPoint, user);
			try
			{
				if (user.HasPickupID(FrailtyAmmoletID))
				{
					RoomHandler currentRoom = user.CurrentRoom;
					if (currentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
					{
						foreach (AIActor aiactor in currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
						{
							if (aiactor.behaviorSpeculator != null)
							{
								aiactor.ApplyEffect(DebuffLibrary.Frailty, 1f, null);
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



