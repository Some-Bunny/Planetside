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

            statue.gameObject.AddComponent<TresspassLightController>();
            statue.DamageReduction = 1000;
            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { statue.gameObject, 0.5f },
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("trespassLight", placeable);
        }
    }
}
