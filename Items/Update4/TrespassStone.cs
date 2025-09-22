using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using GungeonAPI;
using Pathfinding;
using SaveAPI;
using Alexandria.Misc;

namespace Planetside
{
    public class TrespassStone : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Trespass Stone";
            GameObject obj = new GameObject(itemName);
            TrespassStone activeitem = obj.AddComponent<TrespassStone>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("trespassStone"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "See Another Dimension";
            string longDesc = "Opens a gateway to forgotten places.\n\nLocked away by Kaliber herself, these places may still contain something of value, if you're not being watched...";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 1250f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.EXCLUDED;

            activeitem.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);

            TrespassStone.TrespassStoneID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

            GenericRoomTable table = RoomTableTools.CreateRoomTable();
            WeightedRoom roomer =  RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/trespassRoom1.room").room);
            WeightedRoom roomer2 = RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/trespassRoom2.room").room);
            WeightedRoom roomer3 = RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/trespassRoom3.room").room);
            WeightedRoom roomer4 = RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/trespassRoom4.room").room);
            WeightedRoom roomer5 = RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/trespassRoom5.room").room);
            WeightedRoom roomer6 = RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/trespassRoom6.room").room);
            WeightedRoom roomer7 = RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/trespassRoom7.room").room);

            WeightedRoom roomerTut = RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/tutorialblueroom.room").room, 2.5f);
            roomerTut.additionalPrerequisites = new DungeonPrerequisite[]
            {
                new CustomDungeonPrerequisite()
                {
                    customStatToCheck = CustomTrackedStats.PERKS_BOUGHT,
                    comparisonValue = 5,
                    prerequisiteOperation = DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN,
                    useSessionStatValue = false,
                    advancedPrerequisiteType = CustomDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_STAT_COMPARISION
                },
                new CustomDungeonPrerequisite()
                {
                    advancedPrerequisiteType = CustomDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG,
                    customFlagToCheck = CustomDungeonFlags.HAS_TREADED_DEEPER,
                    requireFlag = true,
                    RequiredValueFlag = false
                }
            };

            
            table.includedRooms.Add(roomer);
            table.includedRooms.Add(roomer2);
            table.includedRooms.Add(roomer3);
            table.includedRooms.Add(roomer4);
            table.includedRooms.Add(roomer5);
            table.includedRooms.Add(roomer6);
            table.includedRooms.Add(roomer7);
            table.includedRooms.Add(roomerTut);
            

            table.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/trespassbasic_prisoners.newroom").room, 0.4f));
            table.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/trespassbasic_scarilylarge.newroom").room, 1f));
            table.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/trespassbasic_weirdshowoffthing.newroom").room, 1f));   
            table.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/trespassbasic_anoutcast.newroom").room, 0.9f));           
            table.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/trespassbasic_lockedaway.newroom").room, 0.8f));
            table.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/trespassbasic_pitfalls.newroom").room, 0.5f));
            table.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/trespassbasic_cubeworld.newroom").room, 0.4f));
            table.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/trespassbasic_garden_of_forking_paths.newroom").room, 0.7f));
            table.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/trespassbasic_thegates.newroom").room, 0.4f));
            table.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/trespassbasic_thin.newroom").room, 0.9f));
            table.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/trespassbasic_treadlightly.newroom").room, 0.7f));


            foreach (WeightedRoom weightRoom in table.includedRooms.elements)
            {
                weightRoom.room.customAmbientLight = new Color(0, 0.3f, 0.9f);
                weightRoom.room.usesCustomAmbientLight = true;
                weightRoom.room.overriddenTilesets = GlobalDungeonData.ValidTilesets.FORGEGEON;
                weightRoom.room.drawPrecludedCeilingTiles = true;

                weightRoom.room.UseCustomMusic = true;
                weightRoom.room.UseCustomMusicState = false;
                weightRoom.room.UseCustomMusicSwitch = true;

                weightRoom.room.CustomMusicSwitch = "Play_MUS_Ending_Pilot_01";
                weightRoom.room.CustomMusicEvent = "Play_MUS_Dungeon_State_NPC";
            }
            



            trespassTable = table;

            GenericRoomTable tableDeeper = RoomTableTools.CreateRoomTable();
            
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassHarderRoom1.room").room));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassHarderRoom2.room").room));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassHarderRoom3.room").room, 0.8f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassharderroom4.room").room, 1.1f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassharderroom5.room").room, 1.1f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/TrespassInquisitorInvades.room").room, 1.2f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/TrespassvesselRoom1.room").room, 1.2f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassvesselRoomytwoey.room").room, 1.2f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/tresspassevilInqy.room").room, 1.2f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassharderroom6.room").room, 1f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassharderroom7.room").room, 1.2f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassharderroom8.room").room, 1.1f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassharderroom9.room").room, 0.9f));
            
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trollface.room").room, 0.4f));

            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassroom_2bigwalls.newroom").room, 1f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassroom_5bats.newroom").room, 1f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassroom_suspiciouslyprisonerlike.newroom").room, 1f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassroom_a_void.newroom").room, 0.4f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassroom_asmallpocketinspace.newroom").room, 0.4f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassroom_potentiallyintentionallyimprisoned.newroom").room, 0.4f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespassroom_weezer.newroom").room, 0.4f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespass_fourofthemagain.newroom").room, 0.9f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespass_pathway.newroom").room, 0.4f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespass_TheBouncer.newroom").room, 0.5f));
            tableDeeper.includedRooms.Add(RoomTableTools.GenerateWeightedRoom(Alexandria.DungeonAPI.RoomFactory.BuildNewRoomFromResource("Planetside/Resources/TrespassRooms/TrespassDeeperRooms/trespass_TheBoxesAndtentacles.newroom").room, 0.7f));


            foreach (WeightedRoom weightRoom in tableDeeper.includedRooms.elements)
            {
                weightRoom.room.overriddenTilesets = GlobalDungeonData.ValidTilesets.FORGEGEON;
                weightRoom.room.drawPrecludedCeilingTiles = true;

                weightRoom.room.UseCustomMusic = true;
                weightRoom.room.UseCustomMusicState = false;
                weightRoom.room.UseCustomMusicSwitch = true;

                weightRoom.room.CustomMusicSwitch = "Play_MUS_Ending_Pilot_01";
                weightRoom.room.CustomMusicEvent = "Play_MUS_Dungeon_State_NPC";
            }
            trespassDeeperTable = tableDeeper;
            activeitem.RemovePickupFromLootTables();
        }

        public static GenericRoomTable trespassTable;
        public static GenericRoomTable trespassDeeperTable;

        public static int TrespassStoneID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);   
        }

        public override void DoEffect(PlayerController user)
        {
            GameObject partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal"));
            MeshRenderer rend = partObj.GetComponent<MeshRenderer>();
            rend.allowOcclusionWhenDynamic = true;
            partObj.transform.position = user.transform.position;
            partObj.transform.localScale = Vector3.one;
            partObj.name = "yes";
            partObj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            TrespassPortalController romas = partObj.AddComponent<TrespassPortalController>();
            romas.m_room = user.CurrentRoom;
        }
    }
}



