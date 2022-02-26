﻿using System;
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


    public class TresspassLightSmallController : MonoBehaviour
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
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_light", idlePaths, 6, null, 1, 15000, null, 0f, -0.1875f, true, 12, 20, 4, -4, true, null, null, true, null);
            statue.gameObject.AddComponent<TresspassLightSmallController>();
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