using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planetside
{
    public static class ItemIDs
    {
        public static List<int> AllItemIDs = new List<int>();
        public static void AddToList(int ItemID)
        {
            AllItemIDs.Add(ItemID);

        }
        public static bool itemsHaveBeenRarityBoosted;
        public static float PreviousWeight = 0;

        public static void MakeCommand()
        {
            ETGModConsole.Commands.GetGroup("psog").AddUnit("set_item_weight", delegate (string[] args)
            {
                if (itemsHaveBeenRarityBoosted)
                {
                    //PlanetsideModule.Log($"Modified the weight of all Planetside items and guns to {float.Parse(args[0]).ToString()} weight.", PlanetsideModule.TEXT_COLOR);
                    foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.elements)
                    {
                        if (AllItemIDs.Contains(obj.pickupId))
                        {
                            obj.weight /= PreviousWeight;

                        }
                    }
                    foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.elements)
                    {
                        if (AllItemIDs.Contains(obj.pickupId))
                        {
                            obj.weight /= PreviousWeight;

                        }
                    }
                    itemsHaveBeenRarityBoosted = false;
                    //PlanetsideModule.Log($"cleared old weight.", PlanetsideModule.TEXT_COLOR);

                }
                foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.elements)
                {
                    if (AllItemIDs.Contains(obj.pickupId))
                    {
                        obj.weight *= float.Parse(args[0]);
                    }
                }
                foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.elements)
                {
                    if (AllItemIDs.Contains(obj.pickupId))
                    {
                        obj.weight *= float.Parse(args[0]);
                    }
                }
                PlanetsideModule.Log($"Modified the weight of all Planetside items and guns to {float.Parse(args[0]).ToString()} weight.", PlanetsideModule.TEXT_COLOR);
                itemsHaveBeenRarityBoosted = true;
                PreviousWeight = float.Parse(args[0]);
            });
        }
    }
}
