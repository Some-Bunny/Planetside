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
using Alexandria.cAPI;
using Alexandria.PrefabAPI;
using Planetside.Static_Storage;


namespace Planetside
{ 
    public class OphanaimDecals
    {
        public static void Init()
        {
            /*
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
                
                variable.Key.GetComponent<tk2dBaseSprite>().SortingOrder = 2;
                variable.Key.GetComponent<tk2dBaseSprite>().HeightOffGround = -1.75f;
                variable.Key.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));

            }
            */


            var OphanaimDecal_1 = PrefabBuilder.BuildObject("Ophanaim Decal (1)");
            OphanaimDecal_1.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
            var sprite = OphanaimDecal_1.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "ophanaim_tile_001");
            sprite.SortingOrder = 2;
            sprite.HeightOffGround = -1.75f;
            sprite.usesOverrideMaterial = true;
            sprite.IsPerpendicular = false;
            var mat = new Material(StaticShaders.Default_Object_Shader);
            sprite.renderer.material = mat;

            var OphanaimDecal_2 = PrefabBuilder.BuildObject("Ophanaim Decal (2)");
            OphanaimDecal_2.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
            sprite = OphanaimDecal_2.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "ophanaim_tile_002");
            sprite.SortingOrder = 2;
            sprite.HeightOffGround = -1.75f;
            sprite.usesOverrideMaterial = true;
            sprite.IsPerpendicular = false;
            mat = new Material(StaticShaders.Default_Object_Shader);
            sprite.renderer.material = mat;

            var OphanaimDecal_3 = PrefabBuilder.BuildObject("Ophanaim Decal (3)");
            OphanaimDecal_3.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
            sprite = OphanaimDecal_3.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "ophanaim_tile_003");
            sprite.SortingOrder = 2;
            sprite.HeightOffGround = -1.75f;
            sprite.usesOverrideMaterial = true;
            sprite.IsPerpendicular = false;
            mat = new Material(StaticShaders.Default_Object_Shader);
            sprite.renderer.material = mat;

            var OphanaimDecal_4 = PrefabBuilder.BuildObject("Ophanaim Decal (4)");
            OphanaimDecal_4.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
            sprite = OphanaimDecal_4.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "ophanaim_tile_004");
            sprite.SortingOrder = 2;
            sprite.HeightOffGround = -1.75f;
            sprite.usesOverrideMaterial = true;
            sprite.IsPerpendicular = false;

            mat = new Material(StaticShaders.Default_Object_Shader);
            sprite.renderer.material = mat;


            Dictionary<GameObject, float> decal2x2List = new Dictionary<GameObject, float>()
            {
                { OphanaimDecal_1, 2f },
                { OphanaimDecal_2, 0.8f },
                { OphanaimDecal_3, 0.1f },
                { OphanaimDecal_4, 0.3f },
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(decal2x2List);
            StaticReferences.StoredDungeonPlaceables.Add("ophanaimRandom2x2Decal", placeable);

            StaticReferences.StoredRoomObjects.Add("ophanaim2x2Decal_1", OphanaimDecal_1);
            StaticReferences.StoredRoomObjects.Add("ophanaim2x2Decal_2", OphanaimDecal_2);
            StaticReferences.StoredRoomObjects.Add("ophanaim2x2Decal_3", OphanaimDecal_3);
            StaticReferences.StoredRoomObjects.Add("ophanaim2x2Decal_4", OphanaimDecal_4);
        }
    }
}
