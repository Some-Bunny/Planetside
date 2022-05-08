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

namespace Planetside
{
    public class TrespassStone : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Trespass Stone";
            string resourceName = "Planetside/Resources/trespassStone.png";
            GameObject obj = new GameObject(itemName);
            TrespassStone activeitem = obj.AddComponent<TrespassStone>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "See Another Dimension";
            string longDesc = "Opens a gateway to forgotten places.\n\nLocked away by Kaliber herself, these places may still contain something of value, if you're not being watched...";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 1000f);
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
            WeightedRoom roomer5 = RoomTableTools.GenerateWeightedRoom(RoomFactory.BuildFromResource("Planetside/Resources/TrespassRooms/tutorialblueroom.room").room, 2);

            table.includedRooms.Add(roomer);
            table.includedRooms.Add(roomer2);
            table.includedRooms.Add(roomer3);
            table.includedRooms.Add(roomer4);
            table.includedRooms.Add(roomer5);

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

            foreach (WeightedRoom weightRoom in tableDeeper.includedRooms.elements)
            {
                //weightRoom.room.customAmbientLight = new Color(0, 0.3f, 0.9f);
                //weightRoom.room.usesCustomAmbientLight = true;
                weightRoom.room.overriddenTilesets = GlobalDungeonData.ValidTilesets.FORGEGEON;
                weightRoom.room.drawPrecludedCeilingTiles = true;

                weightRoom.room.UseCustomMusic = true;
                weightRoom.room.UseCustomMusicState = false;
                weightRoom.room.UseCustomMusicSwitch = true;

                weightRoom.room.CustomMusicSwitch = "Play_MUS_Ending_Pilot_01";
                weightRoom.room.CustomMusicEvent = "Play_MUS_Dungeon_State_NPC";
            }
            trespassDeeperTable = tableDeeper;
        }

        public static GenericRoomTable trespassTable;
        public static GenericRoomTable trespassDeeperTable;

        public static int TrespassStoneID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
           
        }

        protected override void DoEffect(PlayerController user)
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



