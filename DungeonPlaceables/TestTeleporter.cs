using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dungeonator;
using ItemAPI;
using UnityEngine;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Planetside;
using BreakAbleAPI;
using Brave.BulletScript;
using System.Collections;

namespace Planetside
{

    public class TestTeleporter
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources2/TestTeleporter/";
            string[] idlePaths = new string[]
            {
                defaultPath+"testteleporter_idle_001.png",
                defaultPath+"testteleporter_idle_002.png",
            };
            string[] activationPaths = new string[]
            {
                defaultPath+"testteleporter_activate_001.png",
                defaultPath+"testteleporter_activate_002.png",
                defaultPath+"testteleporter_activate_003.png",
                defaultPath+"testteleporter_activate_004.png",
                defaultPath+"testteleporter_activate_005.png",
            };
            string[] activeIdlePaths = new string[]
            {
                defaultPath+"testteleporter_activeidle_001.png",
                defaultPath+"testteleporter_activeidle_002.png",
            };
            TeleporterController telly = BreakableAPIToolbox.GenerateTeleporterController("testTeleporter", idlePaths, activationPaths, activeIdlePaths, defaultPath+ "testtelerooicon.png");
         
      
            StaticReferences.StoredRoomObjects.Add("test", telly.gameObject);

            //StaticReferences.StoredRoomObjects.Add("test", breakable.gameObject);
        }
    }
}
