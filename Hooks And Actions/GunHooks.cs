using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Dungeonator;
using SaveAPI;
using Gungeon;
using ItemAPI;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using System.Collections;
using Brave.BulletScript;

namespace Planetside
{
    public static class GunHooks
    {
        public static void Init()
        {
            //new Hook(typeof(Gun).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(GunHooks).GetMethod("PickupHook"));
		}

		public static void PickupHook(Action<Gun, PlayerController> orig, Gun self, PlayerController playerController)
        {
            int h = self.ammo;
            
			orig(self, playerController);
            if (playerController.GetComponent<CorruptedWealth>() != null)
            {
                self.ammo = h;
            }
		}   
	}
}
    

