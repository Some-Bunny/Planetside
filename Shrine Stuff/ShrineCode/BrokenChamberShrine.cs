
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
				ShadowOffsetY = -0.25f,
                AdditionalComponent = typeof(BrokenChamberShrineController)
			};
			GameObject self = iei.Build();
			SpriteID = ItemAPI.SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Shrines/brokenchambershrinelifted.png", self.GetComponent<tk2dBaseSprite>().Collection);
            SpriteID2 = ItemAPI.SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Shrines/EOEShrine.png", self.GetComponent<tk2dBaseSprite>().Collection);

        }
        private static int SpriteID;
        private static int SpriteID2;

        public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses == 0;
		}

        public class BrokenChamberShrineController : BraveBehaviour
        {

            public BrokenChamberShrineController()
            {
            }
            public void Start()
            {
                bool Shrine = SaveAPIManager.GetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED);
                if (Shrine == true)
                {
                    tk2dSprite sprite = base.gameObject.GetComponent<tk2dSprite>();
                    sprite.GetComponent<tk2dBaseSprite>().SetSprite(SpriteID2);
                    try
                    {
                        SimpleShrine shrine = base.gameObject.GetComponent<SimpleShrine>();
                        shrine.text = "A shrine with 4 engravings carved onto it. Although the engravings shift, you can slightly make out what they are...";
                        shrine.OnAccept = Accept;
                        shrine.OnDecline = null;
                        shrine.acceptText = "Kneel.";
                        shrine.declineText = "Leave.";
                        //shrine.CanUse = CanUse;
                    }
                    catch
                    {
                        ETGModConsole.Log("Failure in modifying shrines (1)");
                    }
                }
            }
            public static void Accept(PlayerController player, GameObject shrine)
            {
                Gun gun = PickupObjectDatabase.GetById(InitialiseGTEE.HOneToShootIt) as Gun;
                int StoredGunID = gun.PickupObjectId;

                //PickupObject Item1 = PickupObjectDatabase.GetByName(InitialiseGTEE.HOneToFireIt);
                int Item1ID = Game.Items[InitialiseGTEE.HOneToFireIt].PickupObjectId;
                int Item2ID = Game.Items[InitialiseGTEE.HOneToPrimeIt].PickupObjectId;
                int Item3ID = Game.Items[InitialiseGTEE.HOneToHoldIt].PickupObjectId;

                string encounterNameOrDisplayName1 = (PickupObjectDatabase.GetById(StoredGunID) as Gun).EncounterNameOrDisplayName;
                string encounterNameOrDisplayName2 = (PickupObjectDatabase.GetById(Item1ID)).EncounterNameOrDisplayName;
                string encounterNameOrDisplayName3 = (PickupObjectDatabase.GetById(Item2ID)).EncounterNameOrDisplayName;
                string encounterNameOrDisplayName4 = (PickupObjectDatabase.GetById(Item3ID)).EncounterNameOrDisplayName;

                string header;
                string text;

                header = encounterNameOrDisplayName1 + " / " + encounterNameOrDisplayName2;
                text = "Filler.";
                BrokenChamberShrineController.Notify(header, text);


                header = encounterNameOrDisplayName3 + " / " + encounterNameOrDisplayName4;
                text = "Filler.";
                BrokenChamberShrineController.Notify(header, text);


                shrine.GetComponent<ShrineFactory.CustomShrineController>().numUses++;
                shrine.GetComponent<ShrineFactory.CustomShrineController>().GetRidOfMinimapIcon();
            }
            private static void Notify(string header, string text)
            {
                tk2dSpriteCollectionData encounterIconCollection = AmmonomiconController.Instance.EncounterIconCollection;
                int spriteIdByName = encounterIconCollection.GetSpriteIdByName("Planetside/Resources/shellheart");
                GameUIRoot.Instance.notificationController.DoCustomNotification(header, text, null, spriteIdByName, UINotificationController.NotificationColor.PURPLE, true, true);
            }
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



