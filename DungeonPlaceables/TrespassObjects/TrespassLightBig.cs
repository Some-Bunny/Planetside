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


    public class TresspassLightController : MonoBehaviour
    {
        public MajorBreakable self;
        public void Start()
        {
            self = base.gameObject.GetComponent<MajorBreakable>();
            if (base.gameObject != null)
            {
                tk2dBaseSprite sprite = base.gameObject.GetComponent<tk2dBaseSprite>();
                Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                sprite.usesOverrideMaterial = true;
                mat.mainTexture = sprite.renderer.material.mainTexture;
                mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
                mat.SetFloat("_EmissiveColorPower", 1.55f);
                mat.SetFloat("_EmissivePower", 70);
                sprite.renderer.material = mat;
               
            }
        }
    }
    public class TrespassLightBig
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassBigLight/";
            string[] idlePaths = new string[]
            {
                defaultPath+"tresspasslightBig_001.png",
                defaultPath+"tresspasslightBig_002.png",
                defaultPath+"tresspasslightBig_003.png",
                defaultPath+"tresspasslightBig_004.png",
                defaultPath+"tresspasslightBig_005.png",
                defaultPath+"tresspasslightBig_006.png",
                defaultPath+"tresspasslightBig_007.png",
            };
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_big_light", idlePaths, 5, null, 1, 15000, null, 0f, -0.1875f, true, 28, 32, 4, -4, true, null, null, true, null);
            statue.gameObject.AddComponent<TresspassLightController>();
            statue.DamageReduction = 1000;

            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { statue.gameObject, 0.5f },
            };

            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("trespassBigLight", placeable);
        }
    }
}
