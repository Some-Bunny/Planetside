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
using Gungeon;


namespace Planetside
{

    public class  MovingBlockController : MonoBehaviour
    {
        public RoomHandler parent_room;
        public MajorBreakable self;
        public bool T;
        public RoomEventTriggerCondition Trigger;
        public string DestroyAnimation = "1x1_brick_move";
        public void Start()
        {
            self = base.gameObject.GetComponent<MajorBreakable>();
            Trigger = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;

            parent_room = self.transform.position.GetAbsoluteRoom();
            if (parent_room != null)
            {
                Actions.OnReinforcementWaveTriggered += DoWacky;
            }
        }
        public void DoWacky(RoomHandler room, RoomEventTriggerCondition roomEventTriggerAction)
        {
            //ETGModConsole.Log(1);
            if (room == parent_room && roomEventTriggerAction == Trigger)
            {
                if (T == false)
                {
                    T = true;
                    self.StartCoroutine(DestroyBlocker());
                }
                else
                {
                    T = false;
                }
            }     
        }
        public IEnumerator DestroyBlocker()
        {
            float elapsed = 0;
            float Time = UnityEngine.Random.Range(0.2f, 0.8f);
            while (elapsed < Time)
            {
                if (self == null) { yield break; }
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            self.spriteAnimator.PlayAndDestroyObject(DestroyAnimation);
            yield break;
        }
    }

    public class MovingTile1X1
    {
        public static void Init()
        {
            GenerateCube(false, "trespassSmallMovingBlock_D0");
            GenerateCube(true, "trespassSmallMovingBlock_D1");

        }

        public static void GenerateCube(bool delay, string Name)
        {
            /*
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_light", idleP, 14, breakP, 13, 15000, true, 16, 24, 4, -4, true, null, null, true, null);
            TresspassLightController t = statue.gameObject.AddComponent<TresspassLightController>();
            t.GlowIntensity = 30;
            */

            var tearObject = Alexandria.PrefabAPI.PrefabBuilder.BuildObject($"Trespass Moving Block {Name}");
            var sprite = tearObject.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.Trespass_Room_Object_Data, "pillar1x1_break_001");

            var animator = tearObject.AddComponent<tk2dSpriteAnimator>();


            var majorBreakable = tearObject.AddComponent<MajorBreakable>();
            majorBreakable.HitPoints = 15000;
            majorBreakable.sprite = sprite;
            majorBreakable.spriteAnimator = animator;

            tearObject.CreateFastBody(new IntVector2(16, 24), new IntVector2(3, -4), CollisionLayer.HighObstacle);
            tearObject.CreateFastBody(new IntVector2(16, 24), new IntVector2(3, -4), CollisionLayer.BeamBlocker);
            tearObject.CreateFastBody(new IntVector2(16, 24), new IntVector2(3, -4), CollisionLayer.BulletBlocker);
            tearObject.CreateFastBody(new IntVector2(16, 24), new IntVector2(3, -4), CollisionLayer.EnemyBlocker);
            tearObject.CreateFastBody(new IntVector2(16, 24), new IntVector2(3, -4), CollisionLayer.PlayerBlocker);

            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;


            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            sprite.usesOverrideMaterial = true;
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 12f);
            mat.SetFloat("_EmissivePower", 11);
            sprite.renderer.material = mat;

            majorBreakable.DamageReduction = 1000;

            MovingBlockController cont = tearObject.gameObject.AddComponent<MovingBlockController>();
            cont.Trigger = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;
            cont.T = delay;
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
