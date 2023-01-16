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
    public class OphanaimDecals
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/OphanaimDecals/";
            var obj1 = BreakableAPIToolbox.GenerateDecalObject("ophanaim_decal", new string[] { defaultPath + "ophanaim_tile_001.png" }, 1);
            var obj2 = BreakableAPIToolbox.GenerateDecalObject("ophanaim_decal", new string[] { defaultPath + "ophanaim_tile_002.png" }, 1);
            var obj3 = BreakableAPIToolbox.GenerateDecalObject("ophanaim_decal", new string[] { defaultPath + "ophanaim_tile_003.png" }, 1);
            var obj4 = BreakableAPIToolbox.GenerateDecalObject("ophanaim_decal", new string[] { defaultPath + "ophanaim_tile_004.png" }, 1);
            Dictionary<GameObject, float> decal2x2List = new Dictionary<GameObject, float>()
            {
                { obj1, 2f },
                { obj2, 0.8f },
                { obj3, 0.1f },
                { obj4, 0.3f },
            };
            //LitTk2dCustomFalloffTilted
            //tk2d/CutoutVertexColorTilted
            //Brave/GungeonTilemapFloorReflection
            //
            foreach (var variable in decal2x2List)
            {
                variable.Key.gameObject.AddComponent<TresspassUnlitShaderController>();
                variable.Key.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));

            }


            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(decal2x2List);
            StaticReferences.StoredDungeonPlaceables.Add("ophanaimRandom2x2Decal", placeable);

            StaticReferences.StoredRoomObjects.Add("ophanaim2x2Decal_1", obj1);
            StaticReferences.StoredRoomObjects.Add("ophanaim2x2Decal_2", obj2);
            StaticReferences.StoredRoomObjects.Add("ophanaim2x2Decal_3", obj3);
            StaticReferences.StoredRoomObjects.Add("ophanaim2x2Decal_4", obj4);

        }
    }
}
