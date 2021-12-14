using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GungeonAPI;
using SaveAPI;
using UnityEngine;

namespace Planetside
{

	public static class QuestWanderer
	{
		public static void Add()
		{
			ShrineFactory shrineFactory = new ShrineFactory
			{
				name = "Wandererer",
				modID = "psog",
				spritePath = "Planetside/Resources/Shrines/Wanderer/wanderer_idle_001.png",
				acceptText = "<Talk to him>",
				declineText = "<Walk away>",
				OnAccept = new Action<PlayerController, GameObject>(QuestWanderer.Accept),
				OnDecline = new Action<PlayerController, GameObject>(QuestWanderer.Decline),
				CanUse = new Func<PlayerController, GameObject, bool>(QuestWanderer.CanUse),
				offset = new Vector3(46.8125f, 28.75f, 31.3125f),
				talkPointOffset = new Vector3(0f, 0f, 0f),
				isToggle = false,
				isBreachShrine = true,
				interactableComponent = typeof(QuestWandererInteractable)
			};
			GameObject gameObject = shrineFactory.Build();
			QuestWandererInteractable component = gameObject.GetComponent<QuestWandererInteractable>();
			component.conversation = new List<string>
				{
				"The silence is deafening..."
				};
			gameObject.SetActive(false);
		}

		private static bool CanUse(PlayerController player, GameObject npc)
		{
			return true;
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x0002FAC4 File Offset: 0x0002DCC4
		public static void Accept(PlayerController player, GameObject npc)
		{
			if (!SaveAPIManager.GetFlag(CustomDungeonFlags.HASDONEEVRYTHING) == true)
            {
				QuestWandererInteractable talk = npc.GetComponent<QuestWandererInteractable>();
				talk.Talk(player);
			}
			else
            {

            }
			
		}

		public static void Decline(PlayerController player, GameObject npc)
		{
		}
		public static int Char = 0;
		public static GameObject GameObject;
	}

}
