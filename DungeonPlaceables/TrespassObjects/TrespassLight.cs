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
    public class TrespassLight
    {
        public static void Init()
        {
            /*
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassLight/";
            string[] idlePaths = new string[]
            {
                defaultPath+"trespassPillar1.png",
                defaultPath+"trespassPillar2.png",
                defaultPath+"trespassPillar3.png",
                defaultPath+"trespassPillar4.png",
                defaultPath+"trespassPillar5.png",
                defaultPath+"trespassPillar6.png",
                defaultPath+"trespassPillar7.png",
            };
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_light", idlePaths, 6, null, 1, 15000, true, 12, 20, 4, -4, true, null, null, true, null);
            BreakableAPIToolbox.GenerateShadow(defaultPath + "trespassPillarShadow.png", "trespassPillarShadow", statue.gameObject.transform, new Vector3(0.1875f, -0.1875f));
            */
            var tearObject = Alexandria.PrefabAPI.PrefabBuilder.BuildObject("Trespass Light");
            var sprite = tearObject.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.Trespass_Room_Object_Data, "trespassPillar1");
            var animator = tearObject.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName("trespass_Light");
            sprite.IsPerpendicular = false;

            var majorBreakable = tearObject.AddComponent<MajorBreakable>();
            majorBreakable.HitPoints = 15000;
            majorBreakable.sprite = sprite;
            majorBreakable.spriteAnimator = animator;

            tearObject.CreateFastBody(new IntVector2(12, 20), new IntVector2(4, -4), CollisionLayer.HighObstacle);
            tearObject.CreateFastBody(new IntVector2(12, 20), new IntVector2(4, -4), CollisionLayer.BeamBlocker);
            tearObject.CreateFastBody(new IntVector2(12, 20), new IntVector2(4, -4), CollisionLayer.BulletBlocker);
            tearObject.CreateFastBody(new IntVector2(12, 20), new IntVector2(4, -4), CollisionLayer.EnemyBlocker);
            tearObject.CreateFastBody(new IntVector2(12, 20), new IntVector2(4, -4), CollisionLayer.PlayerBlocker);

            sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 11f);
            mat.SetFloat("_EmissivePower", 3);
            sprite.renderer.material = mat;


            majorBreakable.gameObject.AddComponent<TresspassLightController>();
            majorBreakable.DamageReduction = 1000;
            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { majorBreakable.gameObject, 0.5f },
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("trespassLight", placeable);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("PSOG_trespassLight", placeable);

        }
    }
}
