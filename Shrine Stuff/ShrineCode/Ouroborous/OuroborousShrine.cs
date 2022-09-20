
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
	public class OuroborousShrineController : MonoBehaviour
	{
		public void Start()
		{
			if (GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_LICH) == false)
			{
				Destroy(this.gameObject);
			}
		}
	}


    public static class OuroborousShrine
	{
		public static void Add()
		{
			ShrineFactory sf = new ShrineFactory()
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
				isBreachShrine = true,
				shadowPath = "Planetside/Resources/Shrines/ouroborousshrineShadow.png",
				ShadowOffsetX = 0.0625f,
				ShadowOffsetY = -0.25f,
				AdditionalComponent = typeof(OuroborousShrineController),
			};
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
				header = "Ouroborous Disabled.";

			}
			else
			{
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.LOOPING_ON, true);
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


