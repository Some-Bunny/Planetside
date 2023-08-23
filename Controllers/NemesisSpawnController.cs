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
using System.Collections.ObjectModel;
using GungeonAPI;
using SaveAPI;
using HutongGames.PlayMaker.Actions;

namespace Planetside
{
    internal class NemesisSpawnController : MonoBehaviour
    {

        private void Start()
        {
            Debug.Log("Starting NemesisSpawnController setup...");
            try
            {
                var room = RoomFactory.BuildFromResource("Planetside/Resources/OtherSpecialRooms/nemesis_miniboss_room.room").room;
                room.prerequisites = new List<DungeonPrerequisite>()
                {

                };
                AssetBundle shared_auto_001 = ResourceManager.LoadAssetBundle("shared_auto_001");
                room.associatedMinimapIcon = shared_auto_001.LoadAsset("assets/data/prefabs/room icons/minimap_boss_icon.prefab") as GameObject; WeightedRoom r = new WeightedRoom();
                r.room = room;
                r.weight = 0.8f;
                r.additionalPrerequisites = new DungeonPrerequisite[]
                {
                    new CustomDungeonPrerequisite()
                    {
                        advancedPrerequisiteType = CustomDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG,
                        customFlagToCheck = CustomDungeonFlags.INFECTED_FLOOR_COMPLETED,
                        requireCustomFlag = true
                    }
                };
                r.limitedCopies = false;
                r.maxCopies = 100;

                StaticReferences.RoomTables["blockner"].includedRooms.Add(r);
                StaticReferences.RoomTables["shadeagunim"].includedRooms.Add(r);


                //GameManager.Instance.OnNewLevelFullyLoaded += this.DoEventChecks;
                Debug.Log("Finished NemesisSpawnController setup without failure!");

            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish NemesisSpawnController setup!");
                Debug.Log(e);
            }
        }
        void DoEventChecks()
        {
            
        }
    }
}
