
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.ShrineFactory;
using Gungeon;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;


namespace Planetside
{
	public static class ShrineOfSomething
	{

		public static void Add()
		{
			ShrineFactory aa = new ShrineFactory
			{

				name = "ShrineOfSomething",
				modID = "psog",
				text = "A shrine with two firearms raised to the sky. Do you dare?",
				spritePath = "Planetside/Resources/Shrines/HellShrines/shrineofbolster.png",
				//room = RoomFactory.BuildFromResource("Planetside/ShrineRooms/ShrineOfEvilShrineRoomHell.room").room,
				//RoomWeight = 200f,
				acceptText = "Dare.",
				declineText = "Leave",
				OnAccept = Accept,
				OnDecline = null,
				CanUse = CanUse,
				offset = new Vector3(-1, -1, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,
				HasRoomIcon = false
			};
			string DefPath = "Planetside/Resources/Shrines/HellShrines/";
			aa.BuildWithAnimations(new string[] { DefPath + "shrineofbolster.png" }, 1, new string[] { DefPath + "shrinebroken.png" }, 1);
			SpriteBuilder.AddSpriteToCollection("Planetside/Resources/ShrineIcons/BolsterIcon", SpriteBuilder.ammonomiconCollection);
		}
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses <= 0;
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			SimpleShrine simple = shrine.gameObject.GetComponent<SimpleShrine>();
			simple.text = "The spirits that have once inhabited the shrine have departed.";
			LootEngine.DoDefaultPurplePoof(player.specRigidbody.UnitBottomCenter, false);
			CursesController.EnableBolster();

			//AkSoundEngine.PostEvent("Play_ENM_darken_world_01", shrine);
			//OtherTools.Notify("You Obtained The", "Curse Of Bolstering.", "Planetside/Resources/ShrineIcons/BolsterIcon");
			shrine.GetComponent<CustomShrineController>().numUses++;
			//SomethingTime dark = player.gameObject.AddComponent<SomethingTime>();
			//dark.playeroue = player;
		}
	}
}



