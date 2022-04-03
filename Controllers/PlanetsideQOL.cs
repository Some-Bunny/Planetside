using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Collections;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;
using GungeonAPI;
using System.IO;
using ItemAPI;
using UnityEngine.UI;


using static Planetside.MultiActiveReloadManager;

namespace Planetside
{
    public class PlanetsideQOL
    {
        public static void Init()
        {
            Debug.Log("Starting PlanetsideQOL setup...");
            try
            {
                CreateOrLoadConfiguration();
                new Hook(
                typeof(Chest).GetMethod("TriggerCountdownTimer", BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(PlanetsideQOL).GetMethod("TriggerCountdownTimerHook", BindingFlags.Static | BindingFlags.Public));

                new Hook(
                typeof(SecretRoomDoorBeer).GetMethod("InitializeShootToBreak", BindingFlags.Instance | BindingFlags.Public),
                typeof(PlanetsideQOL).GetMethod("InitializeShootToBreakHook", BindingFlags.Static | BindingFlags.Public));

                //new Hook(
                //typeof(SemioticDungeonGenSettings).GetMethod("GetRandomFlow", BindingFlags.Instance | BindingFlags.Public),
                //typeof(PlanetsideQOL).GetMethod("GetRandomFlowHook", BindingFlags.Static | BindingFlags.Public));
                MakeCommands();
                Debug.Log("Finished PlanetsideQOL setup without failure!");
            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish PlanetsideQOL setup!");
                Debug.Log(e);
            }
        }
        public static readonly string ENABLED_COLOR = "#00d652";
        public static readonly string DISABLED_COLOR = "#bc0000";
        public static readonly string TEXT_COLOR = "#bc9545";


        public static void MakeCommands()
        {
            ETGModConsole.Commands.AddGroup("psog_qol");
            ETGModConsole.Commands.GetGroup("psog_qol").AddUnit("help", delegate (string[] args)
            {
                PlanetsideQOL.Log("Quality Of Life Options", PlanetsideQOL.TEXT_COLOR);
                PlanetsideQOL.Log("Green : Enabled", PlanetsideQOL.ENABLED_COLOR);
                PlanetsideQOL.Log("Red : Disabled", PlanetsideQOL.DISABLED_COLOR);
                PlanetsideQOL.Log("Note: using these commands will instantly toggle them on/off.", PlanetsideQOL.TEXT_COLOR);
                PlanetsideQOL.Log("==============================", PlanetsideQOL.TEXT_COLOR);
                PlanetsideQOL.Log("psog_qol dewicksynergychests : Makes synergy chests spawned in chest rooms adhere to default wick rules instead of always being wicked.", QOLConfig.SynergyChestDeWicking == true ? ENABLED_COLOR : DISABLED_COLOR);
                PlanetsideQOL.Log("psog_qol crackedsecretroomentries : Makes walls belonging to secret rooms cracked.", QOLConfig.RevealSecretRoomWalls == true ? ENABLED_COLOR : DISABLED_COLOR);
            });
            ETGModConsole.Commands.GetGroup("psog_qol").AddUnit("dewicksynergychests", delegate (string[] args)
            {
                QOLConfig.SynergyChestDeWicking = !QOLConfig.SynergyChestDeWicking;
                string str = QOLConfig.SynergyChestDeWicking ? "enabled." : "disabled.";
                PlanetsideQOL.Log("Synergy Chest De-Wicking is now " + str, QOLConfig.SynergyChestDeWicking == true ? ENABLED_COLOR : DISABLED_COLOR);
                UpdateConfiguration();
            });
            ETGModConsole.Commands.GetGroup("psog_qol").AddUnit("crackedsecretroomentries", delegate (string[] args)
            {
                QOLConfig.RevealSecretRoomWalls = !QOLConfig.RevealSecretRoomWalls;
                string str = QOLConfig.RevealSecretRoomWalls ? "enabled." : "disabled.";
                PlanetsideQOL.Log("Cracked Secret Room Walls is now " + str, QOLConfig.RevealSecretRoomWalls == true ? ENABLED_COLOR : DISABLED_COLOR);
                UpdateConfiguration();
            });
        }
        public static void Log(string text, string color = "#FFFFFF")
        {
            ETGModConsole.Log(string.Concat(new string[]
            {
                "<color=",
                color,
                ">",
                text,
                "</color>"
            }), false);
        }


        public static void InitializeShootToBreakHook(Action<SecretRoomDoorBeer> orig, SecretRoomDoorBeer self)
        {          
            orig(self);
            if (QOLConfig.RevealSecretRoomWalls == true)
            {
                if (!GameManager.Instance.InTutorial)
                {
                    MajorBreakable breakable = self.collider.colliderObject.GetComponent<MajorBreakable>();
                    breakable.MaxHitPoints = breakable.HitPoints;
                    breakable.HitPoints = breakable.HitPoints / 2;
                    breakable.ApplyDamage(1f, Vector2.zero, false, true, true);
                }
            }
        }


        public static DungeonFlow GetRandomFlowHook(Func<SemioticDungeonGenSettings, DungeonFlow> orig, SemioticDungeonGenSettings self)
        {
           
            return orig(self);
          
        }
        public static HashSet<IntVector2> CorrectForDoubledSecretRoomnessHook(Func<RoomHandler, DungeonData, HashSet<IntVector2>> orig, RoomHandler room, DungeonData data)
        {
            ETGModConsole.Log("pray");
            return orig(room, data);
        }


        public static void TriggerCountdownTimerHook(Action<Chest> orig, Chest self)
        {
            if (QOLConfig.SynergyChestDeWicking == true)
            {
                float num = 0.02f;
                num += (float)PlayerStats.GetTotalCurse() * 0.05f;
                num += (float)PlayerStats.GetTotalCoolness() * -0.025f;
                num = Mathf.Max(0.01f, Mathf.Clamp01(num));
                if (self.lootTable != null && self.lootTable.CompletesSynergy && (UnityEngine.Random.value < num))
                {
                    orig(self);
                }
                else
                {
                    orig(self);
                }
            }
            else
            {
                orig(self);
            }
        }









        private static string QOLJsonPath = Path.Combine(ETGMod.ResourcesDirectory, "planetsideqol.json");
        private static void CreateOrLoadConfiguration()
        {
            bool flag = !File.Exists(PlanetsideQOL.QOLJsonPath);
            bool flag2 = flag;
            if (flag2)
            {
                ETGModConsole.Log("Planetside QOL: Unable to find existing config, making a new one!", false);
                File.Create(PlanetsideQOL.QOLJsonPath).Close();
                UpdateConfiguration();
            }
            else
            {
                string text = File.ReadAllText(PlanetsideQOL.QOLJsonPath);
                bool flag3 = !string.IsNullOrEmpty(text);
                bool flag4 = flag3;
                if (flag4)
                {
                    QOLConfig = JsonUtility.FromJson<PlanetsideQOL.Configuration>(text);
                }
                else
                {
                    UpdateConfiguration();
                }
            }
        }

        private static void UpdateConfiguration()
        {
            bool flag = !File.Exists(PlanetsideQOL.QOLJsonPath);
            bool flag2 = flag;
            if (flag2)
            {
                ETGModConsole.Log("Planetside QOL: Unable to find existing config, making a new one!", false);
                File.Create(PlanetsideQOL.QOLJsonPath).Close();
            }
            File.WriteAllText(PlanetsideQOL.QOLJsonPath, JsonUtility.ToJson(QOLConfig, true));
        }

        public static PlanetsideQOL.Configuration QOLConfig = new PlanetsideQOL.Configuration
        {
            SynergyChestDeWicking = true,
            RevealSecretRoomWalls = true
        };
        public struct Configuration
        {
            public bool SynergyChestDeWicking;
            public bool RevealSecretRoomWalls;
        }
    }
}
