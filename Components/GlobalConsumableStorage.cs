using AmmonomiconAPI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside.Components
{

   


    public class ConsumableStorage : MonoBehaviour
    {
        public static Dictionary<PlayerController, ConsumableStorage> PlayersWithConsumables = new Dictionary<PlayerController, ConsumableStorage> ();
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.Start))]
        public class Patch_GetSecondTapeDescriptor_Class
            {
            [HarmonyPostfix]
            private static void GetSecondTapeDescriptor(PlayerController __instance)
            {
                if (!PlayersWithConsumables.ContainsKey(__instance))
                {
                    PlayersWithConsumables.Add(__instance, __instance.gameObject.AddComponent<ConsumableStorage>());
                }
            }
        }

        public int GetConsumableOfName(string name)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { return entry.Second; }
            }
            return 0;
        }
        public void AddNewConsumable(string name, int StartingAmount = 0)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { return; }
            }
            NewCurrency.Add(new Tuple<string, int>(name, StartingAmount));
        }

        public void AddConsumableAmount(string name, int Amount = 0)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { entry.Second += Amount; }
            }
        }
        public void SetConsumableAmount(string name, int Amount = 0)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { entry.Second = Amount; }
            }
        }
        public void RemoveConsumableAmount(string name, int Amount = 0)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { entry.Second -= Amount; if (entry.Second < 0) { entry.Second = 0; } }
            }
        }

        public int ReturnConsumableAmount(string name)
        {
            foreach (var entry in NewCurrency)
            {
                if (entry.First == name) { return entry.Second; }
            }
            return 0;
        }

        public List<Tuple<string, int>> NewCurrency = new List<Tuple<string, int>>();
    }
}
