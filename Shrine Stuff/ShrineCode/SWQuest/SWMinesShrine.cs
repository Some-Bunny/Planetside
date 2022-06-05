
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.ShrineFactory;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Brave.BulletScript;
using System.Collections;
using SaveAPI;
using Gungeon;

namespace Planetside
{

	public static class SWMinesShrine
	{

		public static void Add()
		{
			ShrineFactory iei = new ShrineFactory
			{
				name = "BlueCasingShrine",
				modID = "psog",
				text = "A pedestal with a blue casing lodged into its hole.",
				spritePath = "Planetside/Resources/Shrines/SWMines/BlueCasingShrine.png",
				acceptText = "Take the casing out.",
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
			SpriteID = ItemAPI.SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Shrines/SWMines/BlueCasingShrineEmpty.png", self.GetComponent<tk2dBaseSprite>().Collection);
		}
		private static int SpriteID;
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses == 0;
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			LootEngine.TryGivePrefabToPlayer(ETGMod.Databases.Items["Blue Casing"].gameObject, player, true);
			shrine.GetComponent<CustomShrineController>().numUses++;
			shrine.GetComponent<CustomShrineController>().GetRidOfMinimapIcon();
			SimpleShrine shrineonj = shrine.GetComponent<SimpleShrine>();
			shrineonj.text = "The casing that was once there has new been removed.";
			shrineonj.acceptText = "Kneel.";
			shrineonj.declineText = "Leave.";
			tk2dSprite sprite = shrine.GetComponent<tk2dSprite>();
			sprite.GetComponent<tk2dBaseSprite>().SetSprite(SpriteID);
		}
	}
}



