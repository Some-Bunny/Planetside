
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

	public static class BrokenChamberShrine
	{

		public static void Add()
		{
			ShrineFactory iei = new ShrineFactory
			{
				name = "BrokenChamberShrine",
				modID = "psog",
				text = "A shrine with a half-broken chamber on it. It's seems loose...",
				spritePath = "Planetside/Resources/Shrines/brokenchambershrine.png",
				//room = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/BrokenChamberRoom.room").room,
				//RoomWeight = 10,
				acceptText = "Lift the remnant.",
				declineText = "Leave.",
				OnAccept = Accept,
				OnDecline = null,
				CanUse = CanUse,
				offset = new Vector3(0, 0, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,
				shadowPath = "Planetside/Resources/Shrines/defaultShrineShadow.png",
				ShadowOffsetX = 0f,
				ShadowOffsetY = -0.5f
			};
			GameObject self = iei.Build();
			SpriteID = ItemAPI.SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Shrines/brokenchambershrinelifted.png", self.GetComponent<tk2dBaseSprite>().Collection);
		}
		private static int SpriteID;
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses == 0;
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			tk2dSprite sprite = shrine.GetComponent<tk2dSprite>();
			sprite.GetComponent<tk2dBaseSprite>().SetSprite(SpriteID);
			AkSoundEngine.PostEvent("Play_ENM_darken_world_01", shrine);
			LootEngine.TryGivePrefabToPlayer(ETGMod.Databases.Items["Broken Chamber"].gameObject, player, true);
			shrine.GetComponent<CustomShrineController>().numUses++;
			shrine.GetComponent<CustomShrineController>().GetRidOfMinimapIcon();
		}
	}
}



