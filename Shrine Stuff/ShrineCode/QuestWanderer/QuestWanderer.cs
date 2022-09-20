using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GungeonAPI;
using SaveAPI;
using UnityEngine;

namespace Planetside
{

    public class ShitPisseController : MonoBehaviour
    {
        public void Start()
        {


            bool Q = SaveAPIManager.GetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED);
            bool W = SaveAPIManager.GetFlag(CustomDungeonFlags.BEAT_LOOP_1);
            bool E = SaveAPIManager.GetFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND);
            bool R = SaveAPIManager.GetFlag(CustomDungeonFlags.BULLETBANK_DEFEATED);
            bool T = SaveAPIManager.GetFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED);
            bool Y = SaveAPIManager.GetFlag(CustomDungeonFlags.SHELLRAX_DEFEATED);
            bool U = SaveAPIManager.GetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED);
            bool I = SaveAPIManager.GetFlag(CustomDungeonFlags.DEFEAT_FUNGANNON);
            bool O = SaveAPIManager.GetFlag(CustomDungeonFlags.DEFEAT_OPHANAIM);
            bool P = SaveAPIManager.GetFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER);
            bool A = SaveAPIManager.GetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK);
            bool S = SaveAPIManager.GetFlag(CustomDungeonFlags.HAS_COMPLETED_SOMETHING_WICKED);
            if (Q == true && W == true && E == true && R == true && T == true && Y == true && U == true && I == true && O == true && P == true && A == true && S == true)
            {
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    public static class QuestWanderer
	{



		public static void Add()
		{
			ShrineFactory shrineFactory = new ShrineFactory
			{
				name = "Wandererer",
				modID = "psog",
				spritePath = "Planetside/Resources/Shrines/Wanderer/wanderer_idle_001.png",
				acceptText = "<Talk to them>",
				declineText = "<Walk away>",
				OnAccept = new Action<PlayerController, GameObject>(QuestWanderer.Accept),
				OnDecline = new Action<PlayerController, GameObject>(QuestWanderer.Decline),
				CanUse = new Func<PlayerController, GameObject, bool>(QuestWanderer.CanUse),
				offset = new Vector3(46.8125f, 28.75f, 31.3125f),
				talkPointOffset = new Vector3(0f, 0f, 0f),
				isToggle = false,
				isBreachShrine = true,
				AdditionalComponent = typeof(ShitPisseController),
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
