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



    public class TrespassTrollRock
    {
        public class TrollRockStickOn : MonoBehaviour
        {
            public MajorBreakable self;
            public void Start()
            {
                self = base.gameObject.GetComponent<MajorBreakable>();
                if (self != null)
                {
                    SpriteOutlineManager.AddOutlineToSprite(self.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
                }
            }
        }
    
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/Enemies/Tower/";
            string[] idlePaths = new string[]
            {
                defaultPath+"turretthing_idle_001.png",
            };
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_troll_rock", idlePaths, 6, null, 1, 15000, null, 0.1875f, -0.1875f, true, 12, 20, 4, -4, true, null, null, true, null);
            statue.gameObject.AddComponent<TrollRockStickOn>();
            statue.gameObject.AddComponent<TresspassLightController>();
            statue.DamageReduction = 1000;
            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { statue.gameObject, 1f },
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("trespassTrollRock", placeable);

            Dictionary<GameObject, float> dict2 = new Dictionary<GameObject, float>()
            {
                { Tower.prefab, 1f },
                { statue.gameObject, 1f },
            };
            DungeonPlaceable placeable2 = BreakableAPIToolbox.GenerateDungeonPlaceable(dict2);
            StaticReferences.StoredDungeonPlaceables.Add("isItARockOrAlive", placeable2);
        }
    }
}
