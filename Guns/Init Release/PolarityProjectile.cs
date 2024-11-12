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

namespace Planetside
{
    internal class PolarityProjectile : MonoBehaviour
    {
        public void Start()
        {
            Projectile component = base.gameObject.GetComponent<Projectile>();
            PlayerController playerController = component.Owner as PlayerController;
            if (component != null && playerController != null)
            {
				if (playerController.PlayerHasActiveSynergy("Refridgeration"))
				{
                    component.OverrideMotionModule = new HelixProjectileMotionModule
                    {
                        helixAmplitude = 2f,
                        helixWavelength = 8f,
                        ForceInvert = IsUp
                    };
                }
			}
        }
		public bool IsUp;
    }
}
