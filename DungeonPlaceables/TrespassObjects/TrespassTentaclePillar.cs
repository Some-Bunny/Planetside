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
            /*
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassTentacleCube/";
            string[] idlePaths = new string[]
            {
                defaultPath+"pillartentacle_idle_001.png",
                defaultPath+"pillartentacle_idle_002.png",
                defaultPath+"pillartentacle_idle_003.png",
                defaultPath+"pillartentacle_idle_004.png",
                defaultPath+"pillartentacle_idle_005.png",
            };
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_tentacle_light", idlePaths, 4, null, 1, 15000, true, 24, 32, 4, -4, true, null, null, true, null);
            BreakableAPIToolbox.GenerateShadow("Planetside/Resources/DungeonObjects/TrespassObjects/TrespassContainer/trespassContainer_shadow.png", "trespass_tentacle_light_shadow", statue.gameObject.transform, new Vector3(0.125f, -0.25f));

            statue.gameObject.AddComponent<TresspassLightController>();
            statue.DamageReduction = 1000;

            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { statue.gameObject, 0.5f },
            };
            */
            var tearObject = Alexandria.PrefabAPI.PrefabBuilder.BuildObject("Tentacle Pillar");
            var sprite = tearObject.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.Trespass_Room_Object_Data, "pillartentacle_idle_001");
            var animator = tearObject.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName("tentacle_pillar");
            sprite.IsPerpendicular = false;
            var majorBreakable = tearObject.AddComponent<MajorBreakable>();
            majorBreakable.HitPoints = 15000;
            majorBreakable.sprite = sprite;
            majorBreakable.spriteAnimator = animator;

            tearObject.CreateFastBody(new IntVector2(24, 32), new IntVector2(4, -4), CollisionLayer.HighObstacle);
            tearObject.CreateFastBody(new IntVector2(24, 32), new IntVector2(4, -4), CollisionLayer.BeamBlocker);
            tearObject.CreateFastBody(new IntVector2(24, 32), new IntVector2(4, -4), CollisionLayer.BulletBlocker);
            tearObject.CreateFastBody(new IntVector2(24, 32), new IntVector2(4, -4), CollisionLayer.EnemyBlocker);
            tearObject.CreateFastBody(new IntVector2(24, 32), new IntVector2(4, -4), CollisionLayer.PlayerBlocker);

            sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 11f);
            mat.SetFloat("_EmissivePower", 3);
            sprite.renderer.material = mat;

            majorBreakable.DamageReduction = 1000;
            majorBreakable.gameObject.AddComponent<PushImmunity>();
            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { majorBreakable.gameObject, 0.5f },
            };

            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("trespassTentacleLight", placeable);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("PSOG_trespassTentacleLight", placeable);
        }
    }
}
