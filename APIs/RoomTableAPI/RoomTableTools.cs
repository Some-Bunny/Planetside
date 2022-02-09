using System.Collections.Generic;
using UnityEngine;

namespace Planetside
{
    static class RoomTableTools
    {

        /// <summary>
        /// Creates a blank room table
        /// </summary>
        /// <param name="roomCollection">i think this adds all the rooms in the collection into the table</param> 
        /// <param name="includedRoomTables">i think this adds all the rooms in the roomtable given here into the table</param> 

        public static GenericRoomTable CreateRoomTable(WeightedRoomCollection roomCollection = null, List<GenericRoomTable> includedRoomTables = null)
        {
            var roomTable = ScriptableObject.CreateInstance<GenericRoomTable>();
            roomTable.includedRoomTables = new List<GenericRoomTable>();
            if (includedRoomTables != null)
            {
                roomTable.includedRoomTables.AddRange(includedRoomTables);
            }
            if (roomCollection != null)
            {
                foreach (var room in roomCollection.elements)
                {
                    roomTable.includedRooms.Add(room);
                }
            }
            else
            {
                roomTable.includedRooms = new WeightedRoomCollection();
            }
            return roomTable;
        }

        /// <summary>
        /// Creates a weighted room from a room given
        /// </summary>
        /// <param name="room">The room in question.</param> 
        /// <param name="Weight">The chance it has for appearing.</param> 
        /// <param name="prerequisites">A required pre-requisite for it appearing, if it actually works.</param> 
        /// <param name="LimitedCopies">No idea.</param> 
        /// <param name="maxCopies">Limits the amount of copies of the room created. Again, not sure if its useful/used.</param> 

        public static WeightedRoom GenerateWeightedRoom(PrototypeDungeonRoom room,float Weight = 1, DungeonPrerequisite[] prerequisites = null, bool LimitedCopies = false, int maxCopies = 3)
        {
            var weightedRoom = new WeightedRoom();
            if (prerequisites != null)
            {
                weightedRoom.additionalPrerequisites = prerequisites;
            }
            else
            {
                weightedRoom.additionalPrerequisites = new DungeonPrerequisite[0];
            }
            weightedRoom.room = room;
            weightedRoom.weight = Weight;
            weightedRoom.limitedCopies = LimitedCopies;
            weightedRoom.maxCopies = maxCopies;
            return weightedRoom;
        }

        /// <summary>
        /// Creates a weighted room collection
        /// </summary>
        /// <param name="weightedRooms">Any weighted rooms given here will be immediately added.</param> 

        public static WeightedRoomCollection GenerateWeightedRoomColection(List<WeightedRoom> weightedRooms = null)
        {
            var weightedRoomCollection = new WeightedRoomCollection();
            if (weightedRooms != null)
            {
                weightedRoomCollection.elements.AddRange(weightedRooms);
            }
            return weightedRoomCollection;
        }

        /// <summary>
        /// Not that useful/Too lazy to remove it
        /// </summary>
        public static IndividualBossFloorEntry GenerateIndividualBossFloorEntry(GenericRoomTable roomTable, DungeonPrerequisite[] prerequisites = null, string name = "DefName", float Weight = 1)
        {
            var individualBossFloorEntry = new IndividualBossFloorEntry();
            if (prerequisites != null)
            {
                individualBossFloorEntry.GlobalBossPrerequisites = prerequisites;
            }
            else
            {
                individualBossFloorEntry.GlobalBossPrerequisites = new DungeonPrerequisite[0];
            }
            individualBossFloorEntry.TargetRoomTable = roomTable;
            individualBossFloorEntry.TargetRoomTable.name = name;
            individualBossFloorEntry.BossWeight = Weight;
            return individualBossFloorEntry;
        }
        /// <summary>
        /// Not that useful/Too lazy to remove it
        /// </summary>
        public static BossFloorEntry GenerateBossFloorEntry(List<IndividualBossFloorEntry> BossEntries, GlobalDungeonData.ValidTilesets tileset, string Annotation)
        {
            var BossFloorEntry = new BossFloorEntry();
            BossFloorEntry.Annotation = Annotation;
            BossFloorEntry.Bosses = BossEntries;
            BossFloorEntry.AssociatedTilesets = tileset;
            return BossFloorEntry;
        }

        /// <summary>
        /// Not that useful/Too lazy to remove it
        /// </summary>
        /// 
        /// 
        /*
        public static void AddBossTableToManager(IndividualBossFloorEntry entry)
        {
            BossManager manager = GameManager.Instance.BossManager;
        
            
            foreach (BossFloorEntry cum in manager.BossFloorData)
            {

                if(cum.AssociatedTilesets == GlobalDungeonData.ValidTilesets.CASTLEGEON)
                {
                    cum.Bosses.Add(entry);
                }
                foreach (IndividualBossFloorEntry fycjyou in cum.Bosses)
                {
                    ETGModConsole.Log(fycjyou.TargetRoomTable.name);
                }
            }
        }
       */
    }
}