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

    public class  MovingBlockController : MonoBehaviour
    {
        public RoomHandler parent_room;
        public MajorBreakable self;
        public bool T;
        public RoomEventTriggerCondition Trigger;

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
            self.spriteAnimator.PlayAndDestroyObject("break");
            yield break;
        }
    }

    public class MovingTile1X1
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/MovingFloorTiles/OneByOne/";
            string[] breakPaths = new string[]
            {
                defaultPath+"pillar1x1_break_001.png",
                defaultPath+"pillar1x1_break_002.png",
                defaultPath+"pillar1x1_break_003.png",
                defaultPath+"pillar1x1_break_004.png",
                defaultPath+"pillar1x1_break_005.png",
                defaultPath+"pillar1x1_break_006.png",
                defaultPath+"pillar1x1_break_007.png",
                defaultPath+"pillar1x1_break_008.png",
                defaultPath+"pillar1x1_break_009.png",
                defaultPath+"pillar1x1_break_010.png",
                defaultPath+"pillar1x1_break_011.png",
                defaultPath+"pillar1x1_break_012.png",
                defaultPath+"pillar1x1_break_013.png",
                defaultPath+"pillar1x1_break_014.png",
                defaultPath+"pillar1x1_break_015.png",
                defaultPath+"pillar1x1_break_016.png",
                defaultPath+"pillar1x1_break_017.png",
                defaultPath+"pillar1x1_break_018.png",
            };
            GenerateCube(new string[] { defaultPath + "pillar1x1_idle.png" }, breakPaths, false, "trespassSmallMovingBlock_D0");
            GenerateCube(new string[] { defaultPath + "pillar1x1_idle.png" }, breakPaths, true, "trespassSmallMovingBlock_D1");

        }

        public static void GenerateCube(string[] idleP, string[] breakP, bool delay, string Name)
        {
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_light", idleP, 14, breakP, 13, 15000, null, 0.1875f, -0.1875f, true, 16, 24, 4, -4, true, null, null, true, null);
            TresspassLightController t = statue.gameObject.AddComponent<TresspassLightController>();
            t.GlowIntensity = 30;

            MovingBlockController cont = statue.gameObject.AddComponent<MovingBlockController>();
            cont.Trigger = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;
            cont.T = delay;
            statue.DamageReduction = 1000;
            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { statue.gameObject, 1f },
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add(Name, placeable);
        }

    }
}
