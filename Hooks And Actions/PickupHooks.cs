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
            new Hook(typeof(KeyBulletPickup).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("KeyBulletPickup"));


            //may god have mercy on my soul
            new Hook(typeof(GameUIRoot).GetMethod("UpdatePlayerBlankUI", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("UpdateBlanksHook"));

            new Hook(typeof(Chest).GetMethod("Interact", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("InteractHook"));

            new Hook(typeof(AmmoPickup).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("CanINotHaveTwoHookMethodsWithTheSameName"));

            new Hook(typeof(GameUIHeartController).GetMethod("UpdateHealth", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("UpdateHealthHook"));

            new Hook(typeof(ShopItemController).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PickupHooks).GetMethod("UpdateShopItemHook"));

            new Hook(typeof(IounStoneOrbitalItem).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("PickupGuonStoneHook"));
        }

        public static void PickupGuonStoneHook(Action<IounStoneOrbitalItem, PlayerController> orig, IounStoneOrbitalItem self, PlayerController player)
        {
            var CWC = player.GetComponent<CorruptedWealthController>();
            if (CWC != null && self.PickupObjectId == 565)
            {
                self.OrbitalPrefab = CorruptedWealth.VoidGlassStonePrefab.GetComponent<PlayerOrbital>();
            }
            orig(self, player);
        }


        public static void UpdateShopItemHook(Action<ShopItemController> orig, ShopItemController self)
        {
            orig(self);
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                var CWC = player.GetComponent<CorruptedWealthController>();
                if (CWC != null)
                {
                    if (LazyAssCheck(self.item.PickupObjectId) == true)
                    {
                        self.sprite.usesOverrideMaterial = true;
                        Material mat = self.sprite.renderer.material;
                        mat.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                        mat.SetFloat("_EmissivePower", 100);
                        mat.SetFloat("_EmissiveColorPower", 10);
                        mat.SetColor("_OverrideColor", new Color(0.02f, 0.4f, 1, 0.6f));
                    }
                    else
                    {
                        self.sprite.usesOverrideMaterial = false;
                    }
                }
            }
        }

        public static bool LazyAssCheck(int ID)
        {
            if (ID == 73) { return true; }//half heart
            if (ID == 85) { return true; }//heart
            if (ID == 120) { return true; }//armor
            if (ID == 78) { return true; }//Ammo
            if (ID == 600) { return true; }//PartialAmmo
            if (ID == 224) { return true; }//blank
            if (ID == 565) { return true; }
            return false;
        }


        public static void UpdateHealthHook(Action<GameUIHeartController, HealthHaver> orig, GameUIHeartController self, HealthHaver hh)
        {
            orig(self, hh);
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                if (player.healthHaver == hh)
                {
                    var CWC = player.GetComponent<CorruptedWealthController>();
                    if (CWC != null)
                    {
                        List<dfSprite> c = self.extantArmors;
                        for (int i = 0; i < c.Count; i++)
                        {
                            if (i > (c.Count - 1) - CWC.AmountOfArmorConsumed)
                            {
                                c[i].Color = new Color(0.02f, 0.4f, 1, 0.6f);
                            }
                            else
                            {
                                c[i].Color = new Color32(255, 255, 255, 1);
                            }
                        }
                    }
                }
            }
        }


        public static void CanINotHaveTwoHookMethodsWithTheSameName(Action<AmmoPickup, PlayerController> orig, AmmoPickup self, PlayerController player)
        {
            orig(self, player);
            bool b = false;
            if (b == false && player.GetComponent<CorruptedWealthController>() != null && player.CurrentGun != null)
            {
                Gun gungeon = player.CurrentGun;
                int amo = Mathf.Max((int)(gungeon.AdjustedMaxAmmo * 0.85f), 1);
                gungeon.SetBaseMaxAmmo(amo);
                AdvancedHoveringGunProcessor DroneHover = gungeon.gameObject.AddComponent<AdvancedHoveringGunProcessor>();
                DroneHover.Activate = true;
                DroneHover.ConsumesTargetGunAmmo = true;
                DroneHover.AimType = CustomHoveringGunController.AimType.PLAYER_AIM;
                DroneHover.PositionType = CustomHoveringGunController.HoverPosition.CIRCULATE;
                DroneHover.FireType = CustomHoveringGunController.FireType.ON_FIRED_GUN;
                DroneHover.UsesMultipleGuns = true;
                DroneHover.TargetGunIDs = new List<int> { gungeon.PickupObjectId };
                DroneHover.FireCooldown = player.CurrentGun.DefaultModule != null ? player.CurrentGun.DefaultModule.cooldownTime *0.85f : 0.1f;
                DroneHover.FireDuration = 0.1f;
                DroneHover.NumToTrigger = 1;
                gungeon.ammo = Mathf.Max(gungeon.ammo, gungeon.AdjustedMaxAmmo);
                b = true;
            }
        }


   
        public static void InteractHook(Action<Chest, PlayerController> orig, Chest self, PlayerController player)
        {
            orig(self, player);
            
            CorruptedWealthController cont = player.GetComponent<CorruptedWealthController>();
            if (cont != null)
            {
                if (cont.AmountOfCorruptKeys > 0)
                {
                    cont.AmountOfCorruptKeys--;
                }
                float MA = cont.AmountOfCorruptKeys;
                for (int i = 0; i < MA; i++)
                {
                    float H = i +0.5f / MA;
                    PickupObject pickupObject = self.lootTable.GetSingleItemForPlayer(player, 0);
                    Vector2 t = MathToolbox.GetUnitOnCircle(Vector2.down.ToAngle() + Mathf.Lerp(-90, 90, H), 2.5f);
                    DebrisObject j= LootEngine.SpawnItem(pickupObject.gameObject, self.sprite.WorldCenter - new Vector2(0.5f, 1), t, 0.5f, true, false, false);
                    cont.AmountOfCorruptKeys--;
                    player.carriedConsumables.KeyBullets--;
                }
            }       
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
                    dfSprite dfSprite = self.p_playerKeyLabel.Parent.gameObject.transform.GetChild(1).GetComponent<dfSprite>();
                    if (dfSprite != null)
                    {
                        if (cont != null && cont.AmountOfCorruptKeys > 0)
                        {
                            dfSprite.Color = new Color(0.02f, 0.4f, 1, 0.6f);

                        }
                        else
                        {
                            dfSprite.Color = new Color32(255, 255, 255, 1);
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

        public static void KeyBulletPickup(Action<KeyBulletPickup, PlayerController> orig, KeyBulletPickup self, PlayerController player)
        {
            orig(self, player);
            CorruptedWealthController c = player.GetComponent<CorruptedWealthController>();
            if (c != null && self.IsRatKey == false)
            {
                c.AmountOfCorruptKeys++;
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
    

