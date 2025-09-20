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
using FullInspector;
using Planetside.Static_Storage;


namespace Planetside
{ 
    public class TrespassBloodDecals
    {
        public static void Init()
        {


            var splatterSmall = PrefabBuilder.BuildObject("Small Blood Splatter");
            splatterSmall.layer = 20;
            var sprite = splatterSmall.AddComponent<tk2dSprite>();
            var animator = splatterSmall.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName("bloodSplatter_small");       
            sprite.usesOverrideMaterial = true;
            Material mat = new Material(StaticShaders.Default_Shader);
            sprite.renderer.material = mat;
            sprite.IsPerpendicular = false;


            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(new Dictionary<GameObject, float>() { { splatterSmall, 1 } });
            StaticReferences.StoredDungeonPlaceables.Add("smallRandomBloodDecal", placeable);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("PSOG_smallRandomBloodDecal", placeable);


            var splatterLarge = PrefabBuilder.BuildObject("Large Blood Splatter");
            splatterSmall.layer = 20;
            sprite = splatterLarge.AddComponent<tk2dSprite>();
            animator = splatterLarge.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName("bloodSplatter_large");
            sprite.usesOverrideMaterial = true;
            mat = new Material(StaticShaders.Default_Shader);
            sprite.renderer.material = mat;
            sprite.IsPerpendicular = false;


            DungeonPlaceable placeable2 = BreakableAPIToolbox.GenerateDungeonPlaceable(new Dictionary<GameObject, float>() { { splatterLarge, 1 } });
            StaticReferences.StoredDungeonPlaceables.Add("RandomBloodDecal", placeable2);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("PSOG_RandomBloodDecal", placeable);
        }
    }
}
