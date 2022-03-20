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
using System.Collections.ObjectModel;
using GungeonAPI;

namespace Planetside
{
    public class TimeTraderSpawnController : MonoBehaviour
    {
        public void Start()
        {

            Debug.Log("Starting TimeTraderSpawnController setup...");
            try
            {
                TimeToBeat = 0;
                ShopAllowedToSpawn = false;
                new Hook(
                    typeof(RoomHandler).GetMethod("HandleBossClearReward", BindingFlags.Instance | BindingFlags.NonPublic),
                    typeof(TimeTraderSpawnController).GetMethod("HandleBossClearRewardHook", BindingFlags.Static | BindingFlags.Public));

                List<ProceduralFlowModifierData.FlowModifierPlacementType> flowModifierPlacementTypes = new List<ProceduralFlowModifierData.FlowModifierPlacementType>()
                { ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN};
                List<DungeonPrerequisite> dungeonPrerequisites = new List<DungeonPrerequisite>()
                {
                    new DungeonGenToolbox.AdvancedDungeonPrerequisite
                    {
                    advancedAdvancedPrerequisiteType = DungeonGenToolbox.AdvancedDungeonPrerequisite.AdvancedAdvancedPrerequisiteType.SPEEDRUNSHOP,
                    }
                };
                RoomFactory.AddInjection(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/ShopRooms/TimeTraderShop.room").room, "Time Trader Special Shop", flowModifierPlacementTypes, 0, dungeonPrerequisites, "Time Trader Special Shop");
                List<DungeonPrerequisite> dungeonPrerequisitesOther = new List<DungeonPrerequisite>()
                {
                new DungeonGenToolbox.AdvancedDungeonPrerequisite
                    {
                    advancedAdvancedPrerequisiteType = DungeonGenToolbox.AdvancedDungeonPrerequisite.AdvancedAdvancedPrerequisiteType.SPEEDRUNSHOPDISALLOWED,
                    requireTileset = true,
                    requiredTileset = GlobalDungeonData.ValidTilesets.MINEGEON
                    }
                };
                GameObject TooLate;
                OldShrineFactory.builtShrines.TryGetValue("psog:toolate", out TooLate);

                RoomFactory.AddInjection(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/ShopRooms/TimeTraderShopMinesEmpty.room").room, "Too Late For Special Shop", flowModifierPlacementTypes, 0, dungeonPrerequisitesOther, "Too Late For Special Shop", TooLate, -0.75f, -0.75f);
                DungeonHooks.OnPostDungeonGeneration += this.ResetFloorSpecificData;
                Debug.Log("Finished TimeTraderSpawnController setup without failure!");

            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish TimeTraderSpawnController setup!");
                Debug.Log(e);
            }

           

        }
        private void ResetFloorSpecificData()
        {
            ShopAllowedToSpawn = false;
            TimeToBeat = GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED) + 210;
            Debug.Log("Player must beat boss under time time: " +TimeToBeat + " for shop to spawn!");
        }
        public static void HandleBossClearRewardHook(Action<RoomHandler> orig, RoomHandler self)
        {
            orig(self);
            if (GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED) < TimeToBeat) { ShopAllowedToSpawn = true; Debug.Log("Shop allowed to spawn on next possible floor!");}
            else { ShopAllowedToSpawn = false; Debug.Log("Shop not allowed to spawn on next possible floor!"); }
        }
        private static float TimeToBeat;
        public static bool ShopAllowedToSpawn;
    }
}