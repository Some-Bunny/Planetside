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
    public class TrespassDecals
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassDecals/TwoByTwo/";
            Dictionary<GameObject, float> decal2x2List = new Dictionary<GameObject, float>()
            {
                { BreakableAPIToolbox.GenerateDecalObject("trespass_decal", new string[] {defaultPath+ "decalOne1.png" }, 1), 1f },
                { BreakableAPIToolbox.GenerateDecalObject("trespass_decal", new string[] {defaultPath+ "decalOne2.png" }, 1), 0.8f },
                { BreakableAPIToolbox.GenerateDecalObject("trespass_decal", new string[] {defaultPath+ "decalOne3.png" }, 1), 0.1f },
                { BreakableAPIToolbox.GenerateDecalObject("trespass_decal", new string[] {defaultPath+ "decalOne4.png" }, 1), 0.2f },
            };
            foreach(var Entry in decal2x2List)
            {
                Entry.Key.gameObject.GetOrAddComponent<TresspassUnlitShaderController>();
            }
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(decal2x2List);
            StaticReferences.StoredDungeonPlaceables.Add("trespassRandom2x2Decal", placeable);

            GameObject megaDecal = BreakableAPIToolbox.GenerateDecalObject("trespass_decal", new string[] { "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassDecals/Mega/hehehehaha.png" });
            megaDecal.gameObject.GetOrAddComponent<TresspassUnlitShaderController>();
            StaticReferences.StoredRoomObjects.Add("megaDecal", megaDecal);


            GameObject horizontalDecal = BreakableAPIToolbox.GenerateDecalObject("trespass_decal_horizontal", new string[] { "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassDecals/TrespassDirectionalDecal/decalHorizontal.png" });
            horizontalDecal.gameObject.GetOrAddComponent<TresspassUnlitShaderController>();
            StaticReferences.StoredRoomObjects.Add("horizontalDecal", horizontalDecal);

            GameObject verticalDecal = BreakableAPIToolbox.GenerateDecalObject("trespass_decal_vertical", new string[] { "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassDecals/TrespassDirectionalDecal/decalVertical.png" });
            verticalDecal.gameObject.GetOrAddComponent<TresspassUnlitShaderController>();
            StaticReferences.StoredRoomObjects.Add("verticalDecal", verticalDecal);
        }
    }
}
