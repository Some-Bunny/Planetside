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
    public class TrespassCubes
    {
        public static void Init()
        {

            /*
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/SmallCubes/";
            string defaultPathShadow = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassLight/";
            MajorBreakable cube1 = BreakableAPIToolbox.GenerateMajorBreakable("cube1", new string[] { defaultPath + "cube1.png" }, 1, new string[] { defaultPath + "cube1.png" }, 1, 15000, true, 14, 14, 1, -2, true, null, null, true, new List<CollisionLayer>() { CollisionLayer.EnemyBlocker, CollisionLayer.PlayerBlocker });
            MajorBreakable cube2 = BreakableAPIToolbox.GenerateMajorBreakable("cube2", new string[] { defaultPath + "cube2.png" }, 1, new string[] { defaultPath + "cube2.png" }, 1, 15000, true, 12, 12, 2, -2, true, null, null, true, new List<CollisionLayer>() { CollisionLayer.EnemyBlocker, CollisionLayer.PlayerBlocker});
            MajorBreakable cube3 = BreakableAPIToolbox.GenerateMajorBreakable("cube3", new string[] { defaultPath + "cube3.png" }, 1, new string[] { defaultPath + "cube3.png" }, 1, 15000, true, 10, 10, 3, -2, true, null, null, true, new List<CollisionLayer>() { CollisionLayer.EnemyBlocker, CollisionLayer.PlayerBlocker });

            BreakableAPIToolbox.GenerateShadow(defaultPathShadow + "trespassPillarShadow.png", "trespass_cube_shadow", cube1.gameObject.transform, new Vector3(0.1875f, -0.125f));
            BreakableAPIToolbox.GenerateShadow(defaultPathShadow + "trespassPillarShadow.png", "trespass_cube_shadow", cube2.gameObject.transform, new Vector3(0.1875f, -0.1875f));
            BreakableAPIToolbox.GenerateShadow(defaultPathShadow + "trespassPillarShadow.png", "trespass_cube_shadow", cube3.gameObject.transform, new Vector3(0.125f, 0));



            cube1.gameObject.AddComponent<TresspassUnlitShaderController>();
            cube2.gameObject.AddComponent<TresspassUnlitShaderController>();
            cube3.gameObject.AddComponent<TresspassUnlitShaderController>();
            */

            var Cube = PrefabBuilder.BuildObject("Cube");
            Cube.layer = 20;
            var sprite = Cube.AddComponent<tk2dSprite>();
            sprite.IsPerpendicular = false;
            var animator = Cube.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName("LargeCube");
            sprite.usesOverrideMaterial = true;
            Material mat = new Material(StaticShaders.Default_Shader);
            sprite.renderer.material = mat;
            Cube.CreateFastBody(new IntVector2(14, 12), new IntVector2(2, -2), CollisionLayer.PlayerBlocker);
            Cube.CreateFastBody(new IntVector2(14, 12), new IntVector2(2, -2), CollisionLayer.EnemyBlocker);
            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { Cube, 1 },
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("tresPassRandomCubes", placeable);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("PSOG_tresPassRandomCubes", placeable);

        }
    }
}
