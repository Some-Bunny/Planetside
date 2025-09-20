using Gungeon;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Planetside.Toolboxes
{

    public static class ImprovedSynergySetup
    {
        public static Hook synergyHook = new Hook(typeof(StringTableManager).GetMethod("GetSynergyString", BindingFlags.Static | BindingFlags.Public), typeof(ImprovedSynergySetup).GetMethod("SynergyStringHook"));

        public static string SynergyStringHook(Func<string, int, string> orig, string key, int index = -1)
        {
            string text = orig(key, index);
            if (string.IsNullOrEmpty(text))
            {
                text = key;
            }

            return text;
        }



        public static AdvancedSynergyEntry Add(string name, List<PickupObject> mandatoryItems, List<PickupObject> optionalItems = null, bool affectedByLichesEye = true)
        {
            if (mandatoryItems == null)
            {
                ETGModConsole.Log("Synergy " + name + " has no mandatory items/guns.");
                return null;
            }

            List<int> MandatoryItemsID = new List<int>();
            List<int> MandatoryGunsID = new List<int>();

            List<int> OptionalItemsID = new List<int>();
            List<int> OptionalGunsID = new List<int>();

            foreach (PickupObject mandatoryConsoleID in mandatoryItems)
            {
                if (mandatoryConsoleID is Gun) { MandatoryGunsID.Add(mandatoryConsoleID.PickupObjectId); continue; }
                MandatoryItemsID.Add(mandatoryConsoleID.PickupObjectId);
            }

            if (optionalItems != null)
            {
                foreach (PickupObject mandatoryConsoleID in optionalItems)
                {
                    if (mandatoryConsoleID is Gun) { OptionalGunsID.Add(mandatoryConsoleID.PickupObjectId); continue; }
                    OptionalItemsID.Add(mandatoryConsoleID.PickupObjectId);
                }
            }



            AdvancedSynergyEntry advancedSynergyEntry = new AdvancedSynergyEntry
            {
                NameKey = name,
                MandatoryItemIDs = MandatoryItemsID,
                MandatoryGunIDs = MandatoryGunsID,
                OptionalItemIDs = OptionalItemsID,
                OptionalGunIDs = OptionalGunsID,
                bonusSynergies = new List<CustomSynergyType>(),
                statModifiers = new List<StatModifier>(),
                IgnoreLichEyeBullets = !affectedByLichesEye
            };
            Add(advancedSynergyEntry);
            return advancedSynergyEntry;
        }

        public static AdvancedSynergyEntry AddSynergy(this PickupObject mandatoryItem, string name, List<PickupObject> optionalItems = null, bool affectedByLichesEye = false)
        {
            if (mandatoryItem == null)
            {
                ETGModConsole.Log("Synergy " + name + " has no mandatory items/guns.");
                return null;
            }

            List<int> MandatoryItemsID = new List<int>();
            List<int> MandatoryGunsID = new List<int>();

            List<int> OptionalItemsID = new List<int>();
            List<int> OptionalGunsID = new List<int>();
            if (mandatoryItem is Gun) 
            {
                MandatoryGunsID.Add(mandatoryItem.PickupObjectId); 
            }
            else
            {
                MandatoryItemsID.Add(mandatoryItem.PickupObjectId);
            }

            if (optionalItems != null)
            {
                foreach (PickupObject mandatoryConsoleID in optionalItems)
                {
                    if (mandatoryConsoleID is Gun) { OptionalGunsID.Add(mandatoryConsoleID.PickupObjectId); continue; }
                    OptionalItemsID.Add(mandatoryConsoleID.PickupObjectId);
                }
            }



            AdvancedSynergyEntry advancedSynergyEntry = new AdvancedSynergyEntry
            {
                NameKey = name,
                MandatoryItemIDs = MandatoryItemsID,
                MandatoryGunIDs = MandatoryGunsID,
                OptionalItemIDs = OptionalItemsID,
                OptionalGunIDs = OptionalGunsID,
                bonusSynergies = new List<CustomSynergyType>(),
                statModifiers = new List<StatModifier>(),
                IgnoreLichEyeBullets = !affectedByLichesEye
            };
            Add(advancedSynergyEntry);
            return advancedSynergyEntry;
        }


        public static void Add(AdvancedSynergyEntry synergyEntry)
        {
            AdvancedSynergyEntry[] second = new AdvancedSynergyEntry[1] { synergyEntry };
            GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(second).ToArray();
        }
    }
}
