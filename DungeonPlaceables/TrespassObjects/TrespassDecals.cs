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
using Alexandria.PrefabAPI;
using Planetside.Static_Storage;


namespace Planetside
{ 
    public class TrespassDecals
    {
        public static void Init()
        {
            /*
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassDecals/TwoByTwo/";
            Dictionary<GameObject, float> decal2x2List = new Dictionary<GameObject, float>()
            {
                { BreakableAPIToolbox.GenerateDecalObject("trespass_decal", new string[] {defaultPath+ "decalOne1.png" }, 1), 1f },
                { BreakableAPIToolbox.GenerateDecalObject("trespass_decal", new string[] {defaultPath+ "decalOne2.png" }, 1), 0.8f },
                { BreakableAPIToolbox.GenerateDecalObject("trespass_decal", new string[] {defaultPath+ "decalOne3.png" }, 1), 0.1f },
                { BreakableAPIToolbox.GenerateDecalObject("trespass_decal", new string[] {defaultPath+ "decalOne4.png" }, 1), 0.2f },
            };

            foreach (var variable in decal2x2List)
            {
                //variable.Key.gameObject.AddComponent<TresspassUnlitShaderController>();
                variable.Key.GetComponent<tk2dBaseSprite>().SortingOrder = 2;
                variable.Key.GetComponent<tk2dBaseSprite>().HeightOffGround = -1.75f;
                variable.Key.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));

            }
            */

            var Decal_2x2_Random = PrefabBuilder.BuildObject("trespass decal");
            Decal_2x2_Random.layer = 20;
            var sprite = Decal_2x2_Random.AddComponent<tk2dSprite>();
            sprite.SortingOrder = 2;
            sprite.HeightOffGround = -1.75f;
            var animator = Decal_2x2_Random.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName("decals_2x2");
            sprite.usesOverrideMaterial = true;
            sprite.IsPerpendicular = false;
            Material mat = new Material(StaticShaders.Default_Shader);
            sprite.renderer.material = mat;
            Dictionary<GameObject, float> decal2x2List = new Dictionary<GameObject, float>()
            {
                {Decal_2x2_Random, 1 }
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(decal2x2List);
            StaticReferences.StoredDungeonPlaceables.Add("trespassRandom2x2Decal", placeable);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("PSOG_trespassRandom2x2Decal", placeable);



            var megaDecal = PrefabBuilder.BuildObject("Prisoner Trespass Decal");
            megaDecal.layer = 20;
            sprite = megaDecal.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.Trespass_Room_Object_Data, "hehehehaha");
            sprite.SortingOrder = 2;
            sprite.HeightOffGround = -1.75f;
            sprite.usesOverrideMaterial = true;
            mat = new Material(StaticShaders.Default_Shader);
            sprite.renderer.material = mat;

            //GameObject megaDecal = BreakableAPIToolbox.GenerateDecalObject("trespass_decal", new string[] { "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassDecals/Mega/hehehehaha.png" });
            //megaDecal.gameObject.GetOrAddComponent<TresspassUnlitShaderController>();
            StaticReferences.StoredRoomObjects.Add("megaDecal", megaDecal);

            //GameObject mediumDecal = BreakableAPIToolbox.GenerateDecalObject("trespass_decal", new string[] { "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassDecals/Mega/mdeiumDecal.png" });
            //mediumDecal.gameObject.GetOrAddComponent<TresspassLightController>();
            var mediumDecal = PrefabBuilder.BuildObject("Prisoner Trespass Decal");
            mediumDecal.layer = 20;
            sprite = mediumDecal.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.Trespass_Room_Object_Data, "mdeiumDecal");
            sprite.IsPerpendicular = false;
            sprite.SortingOrder = 2;
            sprite.HeightOffGround = -1.75f;
            sprite.usesOverrideMaterial = true;

            mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 8f);
            mat.SetFloat("_EmissivePower", 4);
            sprite.renderer.material = mat;

            StaticReferences.StoredRoomObjects.Add("mediumDecal", mediumDecal);

            //GameObject horizontalDecal = BreakableAPIToolbox.GenerateDecalObject("trespass_decal_horizontal", new string[] { "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassDecals/TrespassDirectionalDecal/decalHorizontal.png" });
            //horizontalDecal.gameObject.GetOrAddComponent<TresspassUnlitShaderController>();

            var horizontalDecal = PrefabBuilder.BuildObject("Prisoner Trespass Decal");
            horizontalDecal.layer = 20;
            sprite = horizontalDecal.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.Trespass_Room_Object_Data, "decalHorizontal");
            sprite.SortingOrder = 2;
            sprite.HeightOffGround = -1.75f;
            sprite.IsPerpendicular = false;
            sprite.usesOverrideMaterial = true;
            mat = new Material(StaticShaders.Default_Shader);
            sprite.renderer.material = mat;

            decal2x2List = new Dictionary<GameObject, float>()
            {
                {horizontalDecal, 1 }
            };
            placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(decal2x2List);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("PSOG_horizontalDecal", placeable);
            StaticReferences.StoredRoomObjects.Add("horizontalDecal", horizontalDecal);


            //GameObject verticalDecal = BreakableAPIToolbox.GenerateDecalObject("trespass_decal_vertical", new string[] { "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassDecals/TrespassDirectionalDecal/decalVertical.png" });
            //verticalDecal.gameObject.GetOrAddComponent<TresspassUnlitShaderController>();
            var verticalDecal = PrefabBuilder.BuildObject("Prisoner Trespass Decal");
            verticalDecal.layer = 20;
            sprite = verticalDecal.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.Trespass_Room_Object_Data, "decalVertical");
            sprite.SortingOrder = 2;
            sprite.HeightOffGround = -1.75f;
            sprite.usesOverrideMaterial = true;
            sprite.IsPerpendicular = false;
            mat = new Material(StaticShaders.Default_Shader);
            sprite.renderer.material = mat;

            decal2x2List = new Dictionary<GameObject, float>()
            {
                {verticalDecal, 1 }
            };
            placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(decal2x2List);



            StaticReferences.StoredRoomObjects.Add("verticalDecal", verticalDecal);   
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("PSOG_verticalDecal", placeable);
        }
    }
}
