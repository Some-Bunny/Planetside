
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using System.IO;
using Dungeonator;
using Gungeon;
using ItemAPI;
using SaveAPI;

namespace Planetside
{

	public static class OuroborousShrine
	{
		// Token: 0x0600004D RID: 77 RVA: 0x00004700 File Offset: 0x00002900
		public static void Add()
		{
			OldShrineFactory sf = new OldShrineFactory()
			{

				name = "Ouroborous Shrine",
				modID = "psog",
				text = "A shrine with an infinity symbol carved onto it.",
				spritePath = "Planetside/Resources/Shrines/ouroboroushrine.png",
				acceptText = "Toggle Ouroborous?",
				declineText = "Leave.",
				OnAccept = Accept,
				OnDecline = null,
				//offset = new Vector3(43.8f, 42.4f, 42.9f),
				offset = new Vector3(24.6875f, 62.5f, 65.0f),
				talkPointOffset = new Vector3(0, 0, 0),
				isToggle = false,
				isBreachShrine = true
			};
			//register shrine
			sf.Build();
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			string header = "";
			string text = "";
			float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);

			bool LoopOn =AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.LOOPING_ON);

			if (LoopOn == true)
			{
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.LOOPING_ON, false);
				//Ouroborous.LoopingOn = false;
				//File.WriteAllText(Ouroborous.SaveFilePath, "false");
				header = "Ouroborous Disabled.";

			}
			else
			{
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.LOOPING_ON, true);
				//Ouroborous.LoopingOn = true;
				//File.WriteAllText(Ouroborous.SaveFilePath, "true");
				header = "Ouroborous set to: " + Loop;
			}
			Notify(header, text);

		}

		private static void Notify(string header, string text)
		{
			tk2dBaseSprite notificationObjectSprite = GameUIRoot.Instance.notificationController.notificationObjectSprite;
			GameUIRoot.Instance.notificationController.DoCustomNotification(header, text, notificationObjectSprite.Collection, notificationObjectSprite.spriteId, UINotificationController.NotificationColor.PURPLE, false, true);
		}

	}
}


