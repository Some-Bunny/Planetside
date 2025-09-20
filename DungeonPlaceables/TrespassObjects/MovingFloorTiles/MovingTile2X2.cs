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

    public class MovingTile2X2
    {
        public static void Init()
        {
            GenerateCube(false, "trespassMovingBlock_D0");
            GenerateCube(true, "trespassMovingBlock_D1");

        }

        public static void GenerateCube(bool delay, string Name)
        {
            //MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_light", idleP, 14, breakP, 13, 15000, true, 32, 40, 4, -4, true, null, null, true, null);
            //TresspassLightController t = statue.gameObject.AddComponent<TresspassLightController>();
            //t.GlowIntensity = 30;

            var tearObject = Alexandria.PrefabAPI.PrefabBuilder.BuildObject($"Trespass Large Moving Block {Name}");
            var sprite = tearObject.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.Trespass_Room_Object_Data, "pillar2x2_break1");

            var animator = tearObject.AddComponent<tk2dSpriteAnimator>();


            var majorBreakable = tearObject.AddComponent<MajorBreakable>();
            majorBreakable.HitPoints = 15000;
            majorBreakable.sprite = sprite;
            majorBreakable.spriteAnimator = animator;

            tearObject.CreateFastBody(new IntVector2(32, 40), new IntVector2(4, -4), CollisionLayer.HighObstacle);
            tearObject.CreateFastBody(new IntVector2(32, 40), new IntVector2(4, -4), CollisionLayer.BeamBlocker);
            tearObject.CreateFastBody(new IntVector2(32, 40), new IntVector2(4, -4), CollisionLayer.BulletBlocker);
            tearObject.CreateFastBody(new IntVector2(32, 40), new IntVector2(4, -4), CollisionLayer.EnemyBlocker);
            tearObject.CreateFastBody(new IntVector2(32, 40), new IntVector2(4, -4), CollisionLayer.PlayerBlocker);

            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;


            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            sprite.usesOverrideMaterial = true;
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 12f);
            mat.SetFloat("_EmissivePower", 11);
            sprite.renderer.material = mat;

            majorBreakable.DamageReduction = 1000;

            MovingBlockController cont = majorBreakable.gameObject.AddComponent<MovingBlockController>();
            cont.Trigger = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;
            cont.T = delay;
            cont.DestroyAnimation = "2x2_brick_move";
            majorBreakable.DamageReduction = 1000;
            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { majorBreakable.gameObject, 1f },
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add(Name, placeable);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add($"PSOG_{Name}", placeable);

        }
    }
}
