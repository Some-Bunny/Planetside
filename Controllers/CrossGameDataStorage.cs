using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace Planetside
{
    public class CrossGameDataStorage
    {
        public static void Start()
        {
            CreateOrLoadConfiguration();

        }
        private static string PlanetsideSpecialDataJsonPath = Path.Combine(ETGMod.ResourcesDirectory, "planetsidespecialdata.json");
        private static void CreateOrLoadConfiguration()
        {
            if (!File.Exists(CrossGameDataStorage.PlanetsideSpecialDataJsonPath))
            {
                ETGModConsole.Log("Planetside Cross Game Storage: Unable to find existing file, making a new one!", false);
                File.Create(CrossGameDataStorage.PlanetsideSpecialDataJsonPath).Close();
                UpdateConfiguration();
            }
            else
            {
                string text = File.ReadAllText(CrossGameDataStorage.PlanetsideSpecialDataJsonPath);
                if (!string.IsNullOrEmpty(text))
                {
                    CrossGameStorage = JsonUtility.FromJson<CrossGameDataStorage.Configuration>(text);
                }
                else
                {
                    UpdateConfiguration();
                }
            }
        }

        public static void UpdateConfiguration()
        {
            if (!File.Exists(CrossGameDataStorage.PlanetsideSpecialDataJsonPath))
            {
                ETGModConsole.Log("Planetside Cross Game Storage: Unable to find existing file, making a new one!", false);
                File.Create(CrossGameDataStorage.PlanetsideSpecialDataJsonPath).Close();
            }
            File.WriteAllText(CrossGameDataStorage.PlanetsideSpecialDataJsonPath, JsonUtility.ToJson(CrossGameStorage, true));
        }

        public static CrossGameDataStorage.Configuration CrossGameStorage = new CrossGameDataStorage.Configuration
        {
            primaryGunSaved = string.Empty,
            secondaryGunSaved = string.Empty,
            primaryGunSavedID = 69420,
            secondaryGunSavedID = 69420,

            AmountOfPerksToChooseFromOnRunStart = 0,
            FoolModeMonth = 0,
            FoolModeDay = 0,
        };
        public struct Configuration
        {
            public string primaryGunSaved;
            public string secondaryGunSaved;
            public int AmountOfPerksToChooseFromOnRunStart;
            public int primaryGunSavedID;
            public int secondaryGunSavedID;
            public int FoolModeMonth;
            public int FoolModeDay;

        }
    }
}
