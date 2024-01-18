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
    public class NoTeleporterPlaceable
    {
        public class NoTeleporterPlaceableComponent : MonoBehaviour { }


        public static void Init()
        {

            GameObject obj = new GameObject("No_Teleporter");
            FakePrefab.MarkAsFakePrefab(obj);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.AddComponent<NoTeleporterPlaceableComponent>();

            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(new Dictionary<GameObject, float>() { { obj, 1 } });
            StaticReferences.StoredDungeonPlaceables.Add("No_Teleporter", placeable);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("psog:No_Teleporter", placeable);

        }
    }
}
