using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;

using UnityEngine.Serialization;

namespace Planetside
{
	internal class EmmisiveTrail : MonoBehaviour
	{
		public EmmisiveTrail()
		{
            this.EmissivePower = 100;
            this.EmissiveColorPower = 1.55f;
        }
        public void Start()
        {
            Shader glowshader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");

            foreach (Transform transform in base.transform)
            {
                if (TransformList.Contains(transform.name))
                {
                    tk2dBaseSprite sproot = transform.GetComponent<tk2dBaseSprite>();
                    if (sproot != null)
                    {
                        sproot.usesOverrideMaterial = true;
                        sproot.renderer.material.shader = glowshader;
                        sproot.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                        sproot.renderer.material.SetFloat("_EmissivePower", EmissivePower);
                        sproot.renderer.material.SetFloat("_EmissiveColorPower", EmissiveColorPower);
                    }
                }
            }  
        }
        private List<string> TransformList = new List<string>()
        {
            "trailObject",
        };
        public float EmissivePower;
        public float EmissiveColorPower;
    }
}

