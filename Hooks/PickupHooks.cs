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
    public static class PickupHooks
    {
        public static void Init()
        {
            new Hook(typeof(HealthPickup).GetMethod("PrePickupLogic", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PickupHooks).GetMethod("PrePickuphookLogic"));
        }
        public static void PrePickuphookLogic(Action<HealthPickup, SpeculativeRigidbody, SpeculativeRigidbody> orig, HealthPickup self, SpeculativeRigidbody player, SpeculativeRigidbody selfBody)
        {
            if (self && player.gameActor && player.gameActor is PlayerController)
            {
                PlayerController playerCont = player.gameActor as PlayerController;
                if (playerCont.GetComponent<GlassComponent>() != null && playerCont.ForceZeroHealthState == true)
                {
                    OtherTools.Notify("A Glass Curse", "Destroyed This Heart!", "Planetside/Resources/PerkThings/glass", UINotificationController.NotificationColor.GOLD);
                    GlassComponent glassComponent = playerCont.GetComponent<GlassComponent>();
                    glassComponent.DoHurty((self.healAmount >= 1f) ? true : false);
                    var type = typeof(HealthPickup);
                    var func = type.GetMethod("GetRidOfMinimapIcon", BindingFlags.Instance | BindingFlags.NonPublic);
                    var ret = func.Invoke(self, null);

                    self.ToggleLabel(false);
                    UnityEngine.Object.Destroy(self.gameObject);
                }
                else
                {
                    orig(self, player, selfBody);
                }
            }
        }
    }
}
    

