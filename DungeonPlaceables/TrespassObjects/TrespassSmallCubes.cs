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
using SaveAPI;
using Alexandria.PrefabAPI;
using Planetside.Static_Storage;

namespace Planetside
{
    public class TrespassSmallCubes
    {
        public static void Init()
        {
            //SmallCube

            /*
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/SmallCubes/";

            MajorBreakable cube1 = BreakableAPIToolbox.GenerateMajorBreakable("cube1", new string[] { defaultPath + "smallcube1.png" }, 1, new string[] { defaultPath + "smallcube1.png" }, 1, 15000, true, 0, 0, 1, -2, true, null, null, true, new List<CollisionLayer>() { CollisionLayer.EnemyBlocker, CollisionLayer.PlayerBlocker });

            MajorBreakable cube2 = BreakableAPIToolbox.GenerateMajorBreakable("cube2", new string[] { defaultPath + "smallcube2.png" }, 1, new string[] { defaultPath + "smallcube2.png" }, 1, 15000, true, 0, 0, 0, -2, true, null, null, true, new List<CollisionLayer>() { CollisionLayer.EnemyBlocker, CollisionLayer.PlayerBlocker });

            MajorBreakable cube3 = BreakableAPIToolbox.GenerateMajorBreakable("cube3", new string[] { defaultPath + "smallcube3.png" }, 1, new string[] { defaultPath + "smallcube3.png" }, 1, 15000, true, 0, 0, 0, -2, true, null, null, true, new List<CollisionLayer>() { CollisionLayer.EnemyBlocker, CollisionLayer.PlayerBlocker });

            MajorBreakable cube4 = BreakableAPIToolbox.GenerateMajorBreakable("cube4", new string[] { defaultPath + "smallcube4.png" }, 1, new string[] { defaultPath + "smallcube4.png" }, 1, 15000, true, 0, 0, 0, -2, true, null, null, true, new List<CollisionLayer>() { CollisionLayer.EnemyBlocker, CollisionLayer.PlayerBlocker });

            MajorBreakable cube5 = BreakableAPIToolbox.GenerateMajorBreakable("cube5", new string[] { defaultPath + "smallcube5.png" }, 1, new string[] { defaultPath + "smallcube5.png" }, 1, 15000, true, 0, 0, 0, -2, true, null, null, true, new List<CollisionLayer>() { CollisionLayer.EnemyBlocker, CollisionLayer.PlayerBlocker });

            cube1.gameObject.AddComponent<TresspassUnlitShaderController>();
            cube2.gameObject.AddComponent<TresspassUnlitShaderController>();
            cube3.gameObject.AddComponent<TresspassUnlitShaderController>();
            cube4.gameObject.AddComponent<TresspassUnlitShaderController>();
            cube5.gameObject.AddComponent<TresspassUnlitShaderController>();

            

            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { cube1.gameObject, 1f },
                { cube2.gameObject, 0.8f },
                { cube3.gameObject, 0.8f },
                { cube4.gameObject, 0.6f },
                { cube5.gameObject, 0.3f },
            };
            foreach (var variable in dict)
            {
                variable.Key.GetComponent<tk2dSprite>().SortingOrder = 2;
                variable.Key.layer = 20;
            }
            */


            var Cube = PrefabBuilder.BuildObject("Small Cube");
            Cube.layer = 20;
            var sprite = Cube.AddComponent<tk2dSprite>();
            sprite.SortingOrder = 2;
            var animator = Cube.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName("SmallCube");
            sprite.usesOverrideMaterial = true;
            sprite.IsPerpendicular = false;
            Material mat = new Material(StaticShaders.Default_Shader);
            sprite.renderer.material = mat;

            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { Cube.gameObject, 1f },
            };

            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);

            StaticReferences.StoredDungeonPlaceables.Add("tresPassRandomCubesSmall", placeable);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("PSOG_tresPassRandomCubesSmall", placeable);

        }
    }
}
