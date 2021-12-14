
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.OldShrineFactory;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Brave.BulletScript;
using System.Collections;
using SaveAPI;
using Gungeon;

namespace Planetside
{

	public static class BlueShrine
	{

		public static void Add()
		{
			OldShrineFactory iei = new OldShrineFactory
			{
				name = "BlueShrine",
				modID = "psog",
				text = "A blue pedestal with a hole in it.",
				spritePath = "Planetside/Resources/Shrines/SWMines/BlueShrineEmpty.png",
				acceptText = "Insert The Blue Casing.",
				declineText = "Leave It.",
				OnAccept = Accept,
				OnDecline = null,

				CanUse = CanUse,
				offset = new Vector3(0, 0, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,

			};
			GameObject self = iei.Build();
			SpriteID = ItemAPI.SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Shrines/SWMines/BlueShrine.png", self.GetComponent<tk2dBaseSprite>().Collection);
		}
		private static int SpriteID;
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses == 0 && player.HasPickupID(Thing.BlueCasingID);
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			AkSoundEngine.PostEvent("Play_OBJ_lock_unlock_01", shrine.gameObject);
			player.RemovePassiveItem(Thing.BlueCasingID);
			LootEngine.SpawnItem(PickupObjectDatabase.GetById(RedThing.RedCasingID).gameObject, player.specRigidbody.UnitCenter, Vector2.right, 0f, false, true, false);
			shrine.GetComponent<CustomShrineController>().numUses++;
			shrine.GetComponent<CustomShrineController>().GetRidOfMinimapIcon();
			SimpleShrine shrineonj = shrine.GetComponent<SimpleShrine>();
			shrineonj.text = "The blue casing sits in the hole snugly.";
			shrineonj.acceptText = "Kneel.";
			shrineonj.declineText = "Leave.";
			tk2dSprite sprite = shrine.GetComponent<tk2dSprite>();
			sprite.GetComponent<tk2dBaseSprite>().SetSprite(SpriteID);
			LootEngine.DoDefaultPurplePoof(sprite.WorldCenter);
			/*
			for (int i = 0; i < GameManager.Instance.Dungeon.data.rooms.Count; i++)
			{
				RoomHandler roomHandler = GameManager.Instance.Dungeon.data.rooms[i];
				if (roomHandler != null)
                {
					roomHandler.RevealedOnMap = false;
					roomHandler.visibility = RoomHandler.VisibilityStatus.REOBSCURED;
				}
			}
			*/
		}
	}
}



