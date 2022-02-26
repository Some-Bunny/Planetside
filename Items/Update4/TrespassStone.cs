﻿using System;
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
            table.includedRooms.Add(roomer);
            table.includedRooms.Add(roomer2);
            foreach (WeightedRoom weightRoom in table.includedRooms.elements)
            {
                weightRoom.room.customAmbientLight = new Color(0, 0.3f, 0.9f);
                weightRoom.room.usesCustomAmbientLight = true;
                weightRoom.room.overriddenTilesets = GlobalDungeonData.ValidTilesets.FORGEGEON;
                weightRoom.room.drawPrecludedCeilingTiles = true;
            }
            
            trespassTable = table;
        }

        public static GenericRoomTable trespassTable;
        public static int TrespassStoneID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
           
        }

        protected override void DoEffect(PlayerController user)
        {
            var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal"));
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


