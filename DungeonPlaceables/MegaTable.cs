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

namespace Planetside
{
    public class MegaTable
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/megaTable/";
            string[] outlinePaths = new string[]
            {
                defaultPath+"megatable_outlineNorth_001.png",
                defaultPath+"megatable_outlineEast_001.png",
                defaultPath+"megatable_outlineWest_001.png",
                defaultPath+"megatable_outlineSouth_001.png"
            };
            string[] northFlipPaths = new string[]
            {
                defaultPath+"megatable_northflip_001.png",
                defaultPath+"megatable_northflip_002.png",
                defaultPath+"megatable_northflip_003.png",
                defaultPath+"megatable_northflip_004.png",
                defaultPath+"megatable_northflip_005.png",
                defaultPath+"megatable_northflip_006.png",
            };
            string[] southFlipPaths = new string[]
            {
                defaultPath+"megatable_southflip_001.png",
                defaultPath+"megatable_southflip_002.png",
                defaultPath+"megatable_southflip_003.png",
                defaultPath+"megatable_southflip_004.png",
                defaultPath+"megatable_southflip_005.png",
                defaultPath+"megatable_southflip_006.png",
            };
            string[] northBreakPaths = new string[]
            {
                defaultPath+"megatable_northflipbreakdown_001.png",
                defaultPath+"megatable_northflipbreakdown_002.png",
                defaultPath+"megatable_northflipbreakdown_001.png",
            };
            string[] southBreakPaths = new string[]
            {
                defaultPath+"megatable_southflipbreakdown_001.png",
                defaultPath+"megatable_southflipbreakdown_002.png",
                defaultPath+"megatable_southflipbreakdown_003.png",
            };
            string[] unflippedBreakPaths = new string[]
            {
                defaultPath+"megatable_idlebreakdown_001.png",
                defaultPath+"megatable_idlebreakdown_002.png",
                defaultPath+"megatable_idlebreakdown_003.png",
            };


          

            Dictionary<float, string> north = new Dictionary<float, string>()
            {
                {25, defaultPath+"megatable_flipbreak_001_north.png"}
            };
            Dictionary<float, string> south = new Dictionary<float, string>()
            {
                {25, defaultPath+"megatable_flipbreak_001_south.png"}
            };


            Dictionary<float,string> aTwo = new Dictionary<float, string>()
            {
                {25, defaultPath+"megatable_idlebreak_001.png"}
            };
            FlippableCover breakable = BreakableAPIToolbox.GenerateTable("Mega_Table", new string[] { defaultPath + "megatable_idle_001.png" }, outlinePaths, northFlipPaths, southFlipPaths, null, null, northBreakPaths, southBreakPaths, null, null, unflippedBreakPaths, 1, 10, 7, 7, true, 64, 30, 0, 8, 64, 5, 64, 5, FlippableCover.FlipStyle.ONLY_FLIPS_UP_DOWN, 300, null, north, south, null, null, aTwo, true, true, 1);
            

        }

        
    }
}
