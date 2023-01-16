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
	internal class OscillatorProjectile : MonoBehaviour
	{
        public void Start()
        {
            var projectile = base.GetComponent<Projectile>();
            if (projectile != null){ projectile.StartCoroutine(OscilaltingBullets.DoOscillate(projectile)); }
        }
	}
}

