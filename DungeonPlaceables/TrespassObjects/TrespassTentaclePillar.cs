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
    public class TrespassTentaclePillar
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassTentacleCube/";
            string[] idlePaths = new string[]
            {
                defaultPath+"pillartentacle_idle_001.png",
                defaultPath+"pillartentacle_idle_002.png",
                defaultPath+"pillartentacle_idle_003.png",
                defaultPath+"pillartentacle_idle_004.png",
                defaultPath+"pillartentacle_idle_005.png",

            };
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_tentacle_light", idlePaths, 4, null, 1, 15000, "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassContainer/trespassContainer_shadow.png", 0.125f, -0.25f, true, 24, 32, 4, -4, true, null, null, true, null);
            statue.gameObject.AddComponent<TresspassLightController>();
            statue.DamageReduction = 1000;

            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { statue.gameObject, 0.5f },
            };

            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("trespassTentacleLight", placeable);
        }
    }
}
