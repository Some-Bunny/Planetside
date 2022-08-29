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
using System.Collections;


namespace Planetside
{

    public class MovingTile2X2
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/MovingFloorTiles/TwoByTwo/";
            string[] breakPaths = new string[]
            {
                defaultPath+"pillar2x2_break1.png",
                defaultPath+"pillar2x2_break2.png",
                defaultPath+"pillar2x2_break3.png",
                defaultPath+"pillar2x2_break4.png",
                defaultPath+"pillar2x2_break5.png",
                defaultPath+"pillar2x2_break6.png",
                defaultPath+"pillar2x2_break7.png",
                defaultPath+"pillar2x2_break8.png",
                defaultPath+"pillar2x2_break9.png",
                defaultPath+"pillar2x2_break10.png",
                defaultPath+"pillar2x2_break11.png",
                defaultPath+"pillar2x2_break12.png",
                defaultPath+"pillar2x2_break13.png",
                defaultPath+"pillar2x2_break14.png",
                defaultPath+"pillar2x2_break15.png",

            };
            GenerateCube(new string[] { defaultPath + "pillar2x2_idle.png" }, breakPaths, false, "trespassMovingBlock_D0");
            GenerateCube(new string[] { defaultPath + "pillar2x2_idle.png" }, breakPaths, true, "trespassMovingBlock_D1");

        }

        public static void GenerateCube(string[] idleP, string[] breakP, bool delay, string Name)
        {
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_light", idleP, 14, breakP, 13, 15000, null, 0.1875f, -0.1875f, true, 32, 40, 4, -4, true, null, null, true, null);
            TresspassLightController t = statue.gameObject.AddComponent<TresspassLightController>();
            t.GlowIntensity = 30;
            MovingBlockController cont = statue.gameObject.AddComponent<MovingBlockController>();
            cont.Trigger = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;
            cont.T = delay;
            statue.DamageReduction = 1000;
            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { statue.gameObject, 1f },
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add(Name, placeable);
        }

    }
}
