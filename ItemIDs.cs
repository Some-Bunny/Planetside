using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Gungeon;
using MonoMod;


namespace Planetside
{
    public class Balls
    {


    }


    public static class ItemIDs
    {

        public static List<int> AllItemIDs = new List<int>();
        public static void AddToList(int ItemID)
        {
            AllItemIDs.Add(ItemID);

        }
        public static bool itemsHaveBeenRarityBoosted;
        public static float PreviousWeight = 0;
        public static AutocompletionSettings _GiveAutocompletionSettings;
        public static void MakeCommand()
        {
            _GiveAutocompletionSettings = new AutocompletionSettings(delegate (string input)
            {
                List<string> list = new List<string>();
                foreach (string text in Game.Items.IDs)
                {
                    if (text.AutocompletionMatch(input.ToLower()))
                    {
                        Console.WriteLine(string.Format("INPUT {0} KEY {1} MATCH!", input, text));
                        list.Add(text.Replace("gungeon:", ""));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("INPUT {0} KEY {1} NO MATCH!", input, text));
                    }
                }
                return list.ToArray();
            });
            ETGModConsole.Commands.GetGroup("psog").AddUnit("spawn", delegate (string[] args)
            {
                if (!ETGModConsole.ArgCount(args, 1, 2))
                {
                    return;
                }
                if (!GameManager.Instance.PrimaryPlayer)
                {
                    ETGModConsole.Log("Couldn't access Player Controller", false);
                    return;
                }
                string text = args[0];
                if (!Game.Items.ContainsID(text))
                {
                    ETGModConsole.Log(string.Format("Invalid item ID {0}!", text), false);
                    return;
                }
                ETGModConsole.Log(string.Concat(new object[]
                {
                        "Attempting to spawn item ID ",
                        args[0],
                        " (numeric ",
                        text,
                        "), class ",
                        Game.Items.Get(text).GetType()
                }), false);
                if (args.Length == 2)
                {
                    int num = int.Parse(args[1]);
                    for (int i = 0; i < num; i++)
                    {
                        IPlayerInteractable[] interfacesInChildren2 = GameObjectExtensions.GetInterfacesInChildren<IPlayerInteractable>(LootEngine.SpawnItem(Game.Items[text].gameObject, GameManager.Instance.PrimaryPlayer.sprite.WorldCenter, Vector2.zero, 0).gameObject);
                        for (int j = 0; j < interfacesInChildren2.Length; j++)
                        {
                            GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(interfacesInChildren2[j]);
                        }
                    }
                    return;
                }
                var gameObject2 = LootEngine.SpawnItem(Game.Items[text].gameObject, GameManager.Instance.PrimaryPlayer.sprite.WorldCenter, Vector2.zero, 0);
                IPlayerInteractable[] interfacesInChildren = GameObjectExtensions.GetInterfacesInChildren<IPlayerInteractable>(gameObject2.gameObject);
                for (int i = 0; i < interfacesInChildren.Length; i++)
                {
                    GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(interfacesInChildren[i]);
                }

            }, _GiveAutocompletionSettings);

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
