using System.Collections.Generic;
using UnityEngine;

namespace Planetside
{
    
    static class LootTableTools
    {

        public static GameObject SelectByWeightNoExclusions(this GenericLootTable table, bool useSeedRandom = false)
        {
            int outIndex = -1;
            List<WeightedGameObject> list = new List<WeightedGameObject>();
            float num = 0f;
            for (int i = 0; i < table.defaultItemDrops.elements.Count; i++)
            {
                WeightedGameObject weightedGameObject = table.defaultItemDrops.elements[i];
                bool flag = true;
                if (weightedGameObject.additionalPrerequisites != null)
                {
                    for (int j = 0; j < weightedGameObject.additionalPrerequisites.Length; j++)
                    {

                    }
                }
                if (weightedGameObject.gameObject != null)
                {

                }
                if (flag)
                {
                    list.Add(weightedGameObject);
                    num += weightedGameObject.weight;
                }
            }
            float num2 = ((!useSeedRandom) ? UnityEngine.Random.value : BraveRandom.GenerationRandomValue()) * num;
            float num3 = 0f;
            for (int k = 0; k < list.Count; k++)
            {
                num3 += list[k].weight;
                if (num3 > num2)
                {
                    outIndex = table.defaultItemDrops.elements.IndexOf(list[k]);
                    return list[k].gameObject;
                }
            }
            outIndex = table.defaultItemDrops.elements.IndexOf(list[list.Count - 1]);
            return list[list.Count - 1].gameObject;
        }
        /// <summary>
        /// Creates a new blank loot table
        /// </summary>
        /// <param name="includedLootTables">i think this litterally dose fuck all</param> 
        /// <param name="prerequisites">the prerequisites of the loot table... whatever the fuck that means</param>
        /// <returns></returns>
        public static GenericLootTable CreateLootTable(List<GenericLootTable> includedLootTables = null, DungeonPrerequisite[] prerequisites = null)
        {
            var lootTable = ScriptableObject.CreateInstance<GenericLootTable>();
            lootTable.defaultItemDrops = new WeightedGameObjectCollection()
            {
                elements = new List<WeightedGameObject>()
            };


            if (prerequisites != null)
            {
                lootTable.tablePrerequisites = prerequisites;
            }
            else
            {
                lootTable.tablePrerequisites = new DungeonPrerequisite[0];
            }

            if (includedLootTables != null)
            {
                lootTable.includedLootTables = includedLootTables;
            }
            else
            {
                lootTable.includedLootTables = new List<GenericLootTable>();
            }


            return lootTable;
        }

        /// <summary>
        /// Adds an item to a loot table via PickupObject
        /// </summary>
        /// <param name="lootTable">The loot table you want to add to</param> 
        /// <param name="po">The PickupObject you're adding</param>
        /// <param name="weight">The Weight of the item you're adding (default is 1)</param>
        /// <returns></returns>
        public static void AddItemToPool(this GenericLootTable lootTable, PickupObject po, float weight = 1, DungeonPrerequisite[] preReq = null)
        {
            lootTable.defaultItemDrops.Add(new WeightedGameObject()
            {
                pickupId = po.PickupObjectId,
                weight = weight,
                rawGameObject = po.gameObject,
                forceDuplicatesPossible = false,
                additionalPrerequisites = preReq ?? new DungeonPrerequisite[0]
            });
        }
        /// <summary>
        /// Adds an item to a loot table via PickupObjectId
        /// </summary>
        /// <param name="lootTable">The loot table you want to add to</param> 
        /// <param name="poID">The id of the PickupObject you're adding</param>
        /// <param name="weight">The Weight of the item you're adding (default is 1)</param>
        /// <returns></returns>
        public static void AddItemToPool(this GenericLootTable lootTable, int poID, float weight = 1)
        {

            var po = PickupObjectDatabase.GetById(poID);
            lootTable.defaultItemDrops.Add(new WeightedGameObject()
            {
                pickupId = po.PickupObjectId,
                weight = weight,
                rawGameObject = po.gameObject,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            });
        }
        public static void AddItemsToPool(this GenericLootTable lootTable, Dictionary<int, float> poIDListWithWeights, DungeonPrerequisite[] preReq = null)
        {
            foreach (var entry in poIDListWithWeights)
            {
                var po = PickupObjectDatabase.GetById(entry.Key);
                lootTable.defaultItemDrops.Add(new WeightedGameObject()
                {
                    pickupId = po.PickupObjectId,
                    weight = entry.Value,
                    rawGameObject = po.gameObject,
                    forceDuplicatesPossible = false,
                    additionalPrerequisites = preReq ?? new DungeonPrerequisite[0]
                });
            }            
        }
       
    }
}