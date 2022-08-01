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

            new Hook(typeof(AmmoPickup).GetMethod("Interact", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("AmmoInteractHook"));

            //AAAAAAAAAAAAAAAAAAAAAA
            new Hook(typeof(KeyBulletPickup).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PickupHooks).GetMethod("KeyBulletPickupUpdateHook"));
            new Hook(typeof(AmmoPickup).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PickupHooks).GetMethod("AmmoPickupUpdateHook"));
            new Hook(typeof(HealthPickup).GetMethod("Update", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("HealthPickupUpdateHook"));
            new Hook(typeof(PlayerItem).GetMethod("Update", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("PlayerItemPickupUpdateHook"));
            new Hook(typeof(PlayerOrbitalItem).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PickupHooks).GetMethod("PlayerOrbitalItemUpdateHook"));
            
            //AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            new Hook(typeof(HealthPickup).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("HealthPickup"));
            new Hook(typeof(SilencerItem).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("SilencerItemPickup"));
            
            //may god have mercy on my soul
            new Hook(typeof(GameUIRoot).GetMethod("UpdatePlayerBlankUI", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("UpdateBlanksHook"));

        }
        public static void UpdateBlanksHook(Action<GameUIRoot, PlayerController> orig, GameUIRoot self, PlayerController player)
        {
            try
            {
                CorruptedWealthController cont = player.GetComponent<CorruptedWealthController>();
                if (cont != null)
                {
                    if (self.blankControllers[player.IsPrimaryPlayer ? 0 : 1] == null) { return; }
                    List<dfSprite> c = self.blankControllers[player.IsPrimaryPlayer ? 0 : 1].extantBlanks;
                    for (int i = 0; i < c.Count; i++)
                    {
                        if (i > (c.Count - 1) - cont.AmountOfCorruptBlanks)
                        {
                            c[i].Color = new Color(0.02f, 0.4f, 1, 0.6f);
                        }
                        else
                        {
                            c[i].Color = new Color32(255, 255, 255, 1);
                        }
                    }
                }
                orig(self, player);
            }
            catch (Exception E)
            {
                Debug.Log(E);
                orig(self, player);
            }
        }


        public static void SilencerItemPickup(Action<SilencerItem, PlayerController> orig, SilencerItem self, PlayerController player)
        {
            orig(self, player);
            CorruptedWealthController c = player.GetComponent<CorruptedWealthController>();
            if (c != null)
            {
                c.AmountOfCorruptBlanks++;
            }
        }

        public static void HealthPickup(Action<HealthPickup, PlayerController> orig, HealthPickup self, PlayerController player)
        {
           
            orig(self, player);
            CorruptedWealthController c = player.GetComponent<CorruptedWealthController>();
            if (c != null)
            {
                c.ProcessDamageModsRedHP(self.healAmount);
                c.AmountOfArmorConsumed += self.armorAmount;
            }
        }

        public static void KeyBulletPickupUpdateHook(Action<KeyBulletPickup> orig, KeyBulletPickup self)
        {
            orig(self);
            foreach(PlayerController player in GameManager.Instance.AllPlayers)
            {
                if (player.GetComponent<CorruptedWealthController>() != null && self.IsRatKey == false)
                {
                    CorruptedPickupController corruptedPickup = self.GetOrAddComponent<CorruptedPickupController>();
                    corruptedPickup.pickup = CorruptedPickupController.PickupType.KEY;
                }
            }
        }

        public static void AmmoPickupUpdateHook(Action<AmmoPickup> orig, AmmoPickup self)
        {
            orig(self);
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                if (player.GetComponent<CorruptedWealthController>() != null)
                {
                    CorruptedPickupController corruptedPickup = self.GetOrAddComponent<CorruptedPickupController>();
                    corruptedPickup.pickup = CorruptedPickupController.PickupType.AMMO;
                }
            }
        }
        public static void HealthPickupUpdateHook(Action<HealthPickup> orig, HealthPickup self)
        {
            orig(self);
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                if (player.GetComponent<CorruptedWealthController>() != null)
                {
                    CorruptedPickupController corruptedPickup = self.GetOrAddComponent<CorruptedPickupController>();
                    corruptedPickup.pickup = CorruptedPickupController.PickupType.HP;
                }
            }
        }
        public static void PlayerItemPickupUpdateHook(Action<PlayerItem> orig, PlayerItem self)
        {
            orig(self);
            if (self.PickupObjectId == 224)
            {
                foreach (PlayerController player in GameManager.Instance.AllPlayers)
                {
                    if (player.GetComponent<CorruptedWealthController>() != null)
                    {
                        CorruptedPickupController corruptedPickup = self.GetOrAddComponent<CorruptedPickupController>();
                        corruptedPickup.pickup = CorruptedPickupController.PickupType.BLANK;
                    }
                }
            }
        }

        public static void PlayerOrbitalItemUpdateHook(Action<PlayerOrbitalItem> orig, PlayerOrbitalItem self)
        {
            orig(self);
            if (self.PickupObjectId == 565)
            {
                foreach (PlayerController player in GameManager.Instance.AllPlayers)
                {
                    if (player.GetComponent<CorruptedWealthController>() != null)
                    {
                        CorruptedPickupController corruptedPickup = self.GetOrAddComponent<CorruptedPickupController>();
                        corruptedPickup.pickup = CorruptedPickupController.PickupType.GLASS_GUON;
                    }
                }
            }
        }


        public static void AmmoInteractHook(Action<AmmoPickup, PlayerController> orig, AmmoPickup self, PlayerController interactor)
        {
            if (!self)
            {
                return;
            }
            if (interactor != null && interactor.CurrentGun != null && interactor.CurrentGun.CanGainAmmo == false)
            {
                return;
            }
            orig(self, interactor);
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
    

