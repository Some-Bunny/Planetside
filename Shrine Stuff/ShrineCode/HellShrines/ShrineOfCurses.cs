
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
	public static class ShrineOfCurses
	{

		public static void Add()
		{
			ShrineFactory aa = new ShrineFactory
			{

				name = "ShrineOfCurses",
				modID = "psog",
				text = "A shrine with an icon of the Jammed. Do you dare?",
				spritePath = "Planetside/Resources/Shrines/HellShrines/shrineofthejammed.png",
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
			aa.BuildWithAnimations(new string[] { DefPath+ "shrineofthejammed.png" }, 1, new string[] {DefPath + "shrinebroken.png" }, 1);
			SpriteBuilder.AddSpriteToCollection("Planetside/Resources/ShrineIcons/JammedIcon", SpriteBuilder.ammonomiconCollection);
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
			CursesController.EnableJamnation();


			//OtherTools.Notify("You Obtained The", "Curse Of Jamnation.", "Planetside/Resources/ShrineIcons/JammedIcon");
			//AkSoundEngine.PostEvent("Play_ENM_darken_world_01", shrine);
			shrine.GetComponent<CustomShrineController>().numUses++;
			//JamTime dark = player.gameObject.AddComponent<JamTime>();
			//dark.playeroue = player;
		}
	}
}



