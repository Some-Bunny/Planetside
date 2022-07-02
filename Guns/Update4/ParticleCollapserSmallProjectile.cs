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
	internal class ParticleCollapserSmallProjectile : MonoBehaviour
	{
		public ParticleCollapserSmallProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
        } 
        private Projectile projectile;
	}
}

