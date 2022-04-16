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

    public class TrespassFadingBlockerController : MonoBehaviour
    {
        public MajorBreakable self;
        public void Start()
        {
            self = base.gameObject.GetComponent<MajorBreakable>();
            if (self != null)
            {
                IntVector2 intVec2 = new IntVector2((int)self.transform.position.x, (int)self.transform.position.y);
                RoomHandler currentRoom = GameManager.Instance.Dungeon.GetRoomFromPosition(intVec2);
                if (currentRoom != null)
                {
                    currentRoom.OnEnemiesCleared += OnEnemiesCleared;
                }
            }
        }
        public void OnEnemiesCleared()
        {
            self.StartCoroutine(DestroyBlocker());
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

    public class TrespassFadingBlocker
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassFadingBlocker/";
            string[] idlePaths = new string[]
            {
                defaultPath+"trespassblocker_idle_001.png",
                defaultPath+"trespassblocker_idle_002.png",
                defaultPath+"trespassblocker_idle_003.png",
                defaultPath+"trespassblocker_idle_004.png",
                defaultPath+"trespassblocker_idle_005.png",
            };
            string[] breakPaths = new string[]
            {
                defaultPath+"trespassblocker_destroy_001.png",
                defaultPath+"trespassblocker_destroy_002.png",
                defaultPath+"trespassblocker_destroy_003.png",
                defaultPath+"trespassblocker_destroy_004.png",
                defaultPath+"trespassblocker_destroy_005.png",
                defaultPath+"trespassblocker_destroy_006.png",
                defaultPath+"trespassblocker_destroy_007.png",
                defaultPath+"trespassblocker_destroy_008.png",
                defaultPath+"trespassblocker_destroy_009.png",
                defaultPath+"trespassblocker_destroy_010.png",
            };
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_fadingblocker", idlePaths, 6, breakPaths, 11, 15000, null, 0f, -0.1875f, true, 16, 20, 0, -4, true, null, null, true, null);
            statue.gameObject.AddComponent<TresspassLightController>();
            statue.gameObject.AddComponent<TrespassFadingBlockerController>();
            statue.DamageReduction = 1000;

            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { statue.gameObject, 0.5f },
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("trespassFadingBlocker", placeable);
        }
    }
}
