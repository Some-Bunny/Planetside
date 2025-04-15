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
        public float GlowIntensity = 70;
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
                mat.SetFloat("_EmissivePower", GlowIntensity);
                sprite.renderer.material = mat;
               
            }
        }
    }

    public class TresspassUnlitShaderController : MonoBehaviour
    {
        public void Start()
        {
            if (base.gameObject != null)
            {
                tk2dBaseSprite sprite = base.gameObject.GetComponent<tk2dBaseSprite>();
                Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                sprite.usesOverrideMaterial = true;
                mat.mainTexture = sprite.renderer.material.mainTexture;
                mat.SetColor("_EmissiveColor", new Color32(0, 255, 0, 255));
                mat.SetFloat("_EmissiveColorPower", 0f);
                mat.SetFloat("_EmissivePower", 0);
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
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_big_light", idlePaths, 5, null, 1, 15000, true, 28, 32, 4, -4, true, null, null, true, null);
            BreakableAPIToolbox.GenerateShadow(defaultPath + "tresspasslightBigShadow.png", "trespass_decorative_pillar_shadow", statue.gameObject.transform, new Vector3(0.125f, -0.25f));

            statue.gameObject.AddComponent<TresspassLightController>();
            statue.DamageReduction = 1000;
            statue.gameObject.AddComponent<PushImmunity>();

            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { statue.gameObject, 0.5f },
            };

            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("trespassBigLight", placeable);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("PSOG_trespassBigLight", placeable);

        }
    }
}
