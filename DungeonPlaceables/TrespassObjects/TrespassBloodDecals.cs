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
    public class TrespassBloodDecals
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassDecals/BloodSmall/";
            Dictionary<GameObject, float> decal2x2List = new Dictionary<GameObject, float>()
            {
                { BreakableAPIToolbox.GenerateDecalObject("blood_spallter_1", new string[] {defaultPath+ "bloodsplatter_001.png" }, 1), 1f },
                { BreakableAPIToolbox.GenerateDecalObject("blood_spallter_2", new string[] {defaultPath+ "bloodsplatter_002.png" }, 1), 1f },
                { BreakableAPIToolbox.GenerateDecalObject("blood_spallter_3", new string[] {defaultPath+ "bloodsplatter_003.png" }, 1), 1f },
                { BreakableAPIToolbox.GenerateDecalObject("blood_spallter_4", new string[] {defaultPath+ "bloodsplatter_004.png" }, 1), 1f },
                { BreakableAPIToolbox.GenerateDecalObject("blood_spallter_5", new string[] {defaultPath+ "bloodsplatter_005.png" }, 1), 1f },
                { BreakableAPIToolbox.GenerateDecalObject("blood_spallter_6", new string[] {defaultPath+ "bloodsplatter_006.png" }, 1), 1f },
            };
            foreach(var Entry in decal2x2List)
            {
                Entry.Key.gameObject.GetOrAddComponent<TresspassUnlitShaderController>();
            }
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(decal2x2List);
            StaticReferences.StoredDungeonPlaceables.Add("smallRandomBloodDecal", placeable);

            string defaultPath2 = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassDecals/Blood/";
            Dictionary<GameObject, float> decal2x2ListBlood = new Dictionary<GameObject, float>()
            {
                { BreakableAPIToolbox.GenerateDecalObject("blood_spallter_1", new string[] { defaultPath2 + "blooddecal_1.png" }, 1), 1f },
                { BreakableAPIToolbox.GenerateDecalObject("blood_spallter_1", new string[] { defaultPath2 + "blooddecal_2.png" }, 1), 1f },
                { BreakableAPIToolbox.GenerateDecalObject("blood_spallter_1", new string[] { defaultPath2 + "blooddecal_3.png" }, 1), 1f },

            };
            foreach (var Entry in decal2x2ListBlood)
            {
                Entry.Key.gameObject.GetOrAddComponent<TresspassUnlitShaderController>();
            }
            DungeonPlaceable placeable2 = BreakableAPIToolbox.GenerateDungeonPlaceable(decal2x2List);
            StaticReferences.StoredDungeonPlaceables.Add("RandomBloodDecal", placeable2);


        }
    }
}
