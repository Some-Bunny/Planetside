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
using static UnityEngine.UI.GridLayoutGroup;

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
            new Hook(typeof(AmmoPickup).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("CanINotHaveTwoHookMethodsWithTheSameName"));
            new Hook(typeof(GameUIHeartController).GetMethod("UpdateHealth", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("UpdateHealthHook"));
            new Hook(typeof(ShopItemController).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PickupHooks).GetMethod("UpdateShopItemHook"));
            new Hook(typeof(IounStoneOrbitalItem).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(PickupHooks).GetMethod("PickupGuonStoneHook"));
            new Hook(typeof(EscapeRopeItem).GetMethod("DoEffect", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PickupHooks).GetMethod("DoEffectHook"));

            new Hook(typeof(PassiveReflectItem).GetMethod("OnPreCollision", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PickupHooks).GetMethod("OnPreCollisionHook"));
            new Hook(typeof(RatPackItem).GetMethod("EatBullet", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PickupHooks).GetMethod("EatBulletHook"));


            new Hook(typeof(Chest).GetMethod("Open", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PickupHooks).GetMethod("OpenHook"));

        }

        public static void EatBulletHook(Action<RatPackItem, Projectile> orig, RatPackItem self, Projectile p) 
        {
            if (p is ThirdDimensionalProjectile third) 
            {
                third.ForceHurtPlayer(self.LastOwner, p);
                p.DieInAir(false, true, true, false);
                PlanetsideReflectionHelper.ReflectSetField<int>(typeof(RatPackItem), "m_containedBullets", PlanetsideReflectionHelper.ReflectGetField<int>(typeof(RatPackItem), "m_containedBullets", self) + 1, self);
                PlanetsideReflectionHelper.ReflectSetField<int>(typeof(RatPackItem), "m_containedBullets", Mathf.Clamp(PlanetsideReflectionHelper.ReflectGetField<int>(typeof(RatPackItem), "m_containedBullets", self), 0, PlanetsideReflectionHelper.ReflectGetField<int>(typeof(RatPackItem), "MaxContainedBullets", self)), self);
            }

            var un = p.GetComponent<MarkForUndodgeAbleBullet>();
            if (p.Owner is AIActor && un != null)
            {
                un.ForceHurtPlayer(self.LastOwner, p);
                p.DieInAir(false, true, true, false);
                PlanetsideReflectionHelper.ReflectSetField<int>(typeof(RatPackItem), "m_containedBullets", PlanetsideReflectionHelper.ReflectGetField<int>(typeof(RatPackItem), "m_containedBullets", self)+1, self);
                PlanetsideReflectionHelper.ReflectSetField<int>(typeof(RatPackItem), "m_containedBullets", Mathf.Clamp(PlanetsideReflectionHelper.ReflectGetField<int>(typeof(RatPackItem), "m_containedBullets", self), 0, PlanetsideReflectionHelper.ReflectGetField<int>(typeof(RatPackItem), "MaxContainedBullets", self)), self);
            }
            else
            { orig(self, p); }
        }

        public static void OnPreCollisionHook(Action<PassiveReflectItem, SpeculativeRigidbody, PixelCollider, SpeculativeRigidbody, PixelCollider> orig, PassiveReflectItem self, SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
        {
            var proj = otherRigidbody.GetComponent<Projectile>();
            if (proj)
            {
                if (proj is ThirdDimensionalProjectile projectile | otherRigidbody.GetComponent<MarkForUndodgeAbleBullet>() != null)
                {
                    if (self.condition == PassiveReflectItem.Condition.WhileDodgeRolling && !self.Owner.spriteAnimator.QueryInvulnerabilityFrame())
                    {
                        return;
                    }
                    Projectile component = otherRigidbody.GetComponent<Projectile>();
                    if (component != null)
                    {
                        otherRigidbody.GetComponent<MarkForUndodgeAbleBullet>().ForceHurtPlayer(self.Owner, component);
                        PassiveReflectItem.ReflectBullet(component, self.retargetReflectedBullet, self.Owner, self.minReflectedBulletSpeed, 1f, 1f, 0f);
                        if (self.AmmoGainedOnReflection > 0)
                        {
                            Gun currentGun = self.Owner.CurrentGun;
                            if (currentGun && currentGun.CanGainAmmo)
                            {
                                currentGun.GainAmmo(self.AmmoGainedOnReflection);
                            }
                        }
                        AkSoundEngine.PostEvent("Play_OBJ_metalskin_deflect_01", component.gameObject);
                        otherRigidbody.transform.position += component.Direction.ToVector3ZUp(0f) * 0.5f;
                        otherRigidbody.Reinitialize();
                        PhysicsEngine.SkipCollision = true;
                    }
                
                }
            }
            else
            {
                orig(self, myRigidbody, myCollider, otherRigidbody, otherCollider);
            }
        }


        public static void DoEffectHook(Action<EscapeRopeItem, PlayerController> orig, EscapeRopeItem self, PlayerController user)
        {
            if (user.CurrentRoom.CompletelyPreventLeaving)
            {
                return;
            }
            if (user.IsInMinecart)
            {
                user.currentMineCart.EvacuateSpecificPlayer(user, true);
            }
            AkSoundEngine.PostEvent("Play_OBJ_rope_escape_01", self.gameObject);
            if (!user.CurrentRoom.IsWildWestEntrance)
            {
                RoomHandler targetRoom = null;
                BaseShopController[] componentsInChildren = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<BaseShopController>(true);
                if (componentsInChildren != null && componentsInChildren.Length > 0)
                {
                    foreach (var a in componentsInChildren)
                    {
                        if (a.gameObject.GetComponent<HMPrimeEngageDoer>() == null)
                        {
                            targetRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(componentsInChildren[0].transform.position.IntXY(VectorConversions.Round));
                        }
                    }
                }
                user.EscapeRoom(PlayerController.EscapeSealedRoomStyle.ESCAPE_SPIN, true, targetRoom);
            }
        }


        public static void PickupGuonStoneHook(Action<IounStoneOrbitalItem, PlayerController> orig, IounStoneOrbitalItem self, PlayerController player)
        {
            var CWC = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
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
                var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
                if (perl != null)
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
            if (ID == 565) { return true; }//glass guon stone I think
            if (ID == 67) { return true; }//key
            return false;
        }


        public static void UpdateHealthHook(Action<GameUIHeartController, HealthHaver> orig, GameUIHeartController self, HealthHaver hh)
        {
            orig(self, hh);
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                if (player.healthHaver == hh)
                {
                    var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
                    if (perl != null)
                    {
                        List<dfSprite> c = self.extantArmors;
                        if (c != null || c.Count > 0)
                        {
                            for (int i = 0; i < c.Count; i++)
                            {
                                if (i > (c.Count - 1) - perl.AmountOfArmorConsumed)
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
                    /*
                        var CWC = player.GetComponent<CorruptedWealthController>();
                    if (CWC != null)
                    {
                        List<dfSprite> c = self.extantArmors;
                        if (c != null || c.Count > 0)
                        {
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
                    */
                }
            }
        }


        public static void CanINotHaveTwoHookMethodsWithTheSameName(Action<AmmoPickup, PlayerController> orig, AmmoPickup self, PlayerController player)
        {       
            orig(self, player);
            try
            {
                bool b = false;
                if (b == true) { return; }
                var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
                if (perl != null)
                {
                    SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.CORRUPTEDWEALTH_FLAG_AMMO, true);

                    var obj = UnityEngine.Object.Instantiate<tk2dSpriteAnimator>(CorruptedWealth.CorruptionVFXObject, self.sprite.WorldCenter, Quaternion.identity);
                    obj.PlayAndDestroyObject("corrupt_ammo");

                    AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Capture_01", player.gameObject);

                    Gun gungeon = player.CurrentGun;

                    //int amo = Mathf.Max((int)(gungeon.AdjustedMaxAmmo * 0.75f), 1);
                    //gungeon.SetBaseMaxAmmo(amo);

                    AdvancedHoveringGunProcessor DroneHover = gungeon.gameObject.AddComponent<AdvancedHoveringGunProcessor>();
                    DroneHover.Activate = true;
                    DroneHover.ConsumesTargetGunAmmo = true;
                    DroneHover.AimType = CustomHoveringGunController.AimType.PLAYER_AIM;
                    DroneHover.PositionType = CustomHoveringGunController.HoverPosition.CIRCULATE;
                    DroneHover.FireType = CustomHoveringGunController.FireType.ON_FIRED_GUN;
                    DroneHover.UsesMultipleGuns = true;
                    DroneHover.TargetGunIDs = new List<int> { gungeon.PickupObjectId };
                    DroneHover.FireCooldown = player.CurrentGun.DefaultModule != null ? player.CurrentGun.DefaultModule.cooldownTime * 0.85f : 0.1f;
                    DroneHover.FireDuration = 0.1f;
                    DroneHover.NumToTrigger = 1;
                    b = true;
                }
                /*
                    //bool b = false;
                if (b == false && player.GetComponent<CorruptedWealthController>() != null && player.CurrentGun != null)
                {
                    SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.CORRUPTEDWEALTH_FLAG_AMMO, true);

                    GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(365) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects.First().effects.First().effect, true);
                    vfx.transform.position = player.sprite.WorldCenter;
                    vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                    vfx.transform.localScale *= 1.5f;
                    UnityEngine.Object.Destroy(vfx, 1);

                    AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Capture_01", player.gameObject);

                    Gun gungeon = player.CurrentGun;

                    //int amo = Mathf.Max((int)(gungeon.AdjustedMaxAmmo * 0.75f), 1);
                    //gungeon.SetBaseMaxAmmo(amo);

                    AdvancedHoveringGunProcessor DroneHover = gungeon.gameObject.AddComponent<AdvancedHoveringGunProcessor>();
                    DroneHover.Activate = true;
                    DroneHover.ConsumesTargetGunAmmo = true;
                    DroneHover.AimType = CustomHoveringGunController.AimType.PLAYER_AIM;
                    DroneHover.PositionType = CustomHoveringGunController.HoverPosition.CIRCULATE;
                    DroneHover.FireType = CustomHoveringGunController.FireType.ON_FIRED_GUN;
                    DroneHover.UsesMultipleGuns = true;
                    DroneHover.TargetGunIDs = new List<int> { gungeon.PickupObjectId };
                    DroneHover.FireCooldown = player.CurrentGun.DefaultModule != null ? player.CurrentGun.DefaultModule.cooldownTime * 0.85f : 0.1f;
                    DroneHover.FireDuration = 0.1f;
                    DroneHover.NumToTrigger = 1;
                    //gungeon.ammo = Mathf.Max(gungeon.ammo, gungeon.AdjustedMaxAmmo);
                    //b = true;
                }
                */
            }
            catch (Exception e)
            {

                Debug.Log("Corrupted Ammo Pickup Hook is BrOke");
                Debug.Log(e);
            }
        }



        public static void OpenHook(Action<Chest, PlayerController> orig, Chest self, PlayerController player)
        {

            var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
            if (perl != null)
            {
                if (perl.AmountOfCorruptKeys > 0)
                {
                    perl.AmountOfCorruptKeys--;
                }
                float MA = perl.AmountOfCorruptKeys;
                if (MA > 0)
                {
                    SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.CORRUPTEDWEALTH_FLAG_KEY, true);

                    var obj = UnityEngine.Object.Instantiate<tk2dSpriteAnimator>(CorruptedWealth.CorruptionVFXObject, self.sprite.WorldCenter, Quaternion.identity);
                    obj.PlayAndDestroyObject("corrupt_key");
                    AkSoundEngine.PostEvent("Play_BOSS_DragunGold_Crackle_01", player.gameObject);
                }
                for (int i = 0; i < MA; i++)
                {
                    if (player.carriedConsumables.KeyBullets == 0) { return; }
                    //float H = i + 0.5f / MA;
                    //PickupObject pickupObject = self.lootTable.GetSingleItemForPlayer(player, 0);
                    //Vector2 t = MathToolbox.GetUnitOnCircle(Vector2.down.ToAngle() + Mathf.Lerp(-90, 90, H), 2.5f);
                    //DebrisObject j = LootEngine.SpawnItem(pickupObject.gameObject, self.sprite.WorldCenter - new Vector2(0.5f, 1), t, 0.5f, true, false, false);
                    if (self.contents == null)
                    {
                        self.contents = new List<PickupObject>();
                    }
                    self.contents.Add(self.lootTable.GetItemsForPlayer(player, 0, null).First());

                    perl.AmountOfCorruptKeys--;
                    player.carriedConsumables.KeyBullets--;
                }
            }
            orig(self, player);

            /*
                CorruptedWealthController cont = player.GetComponent<CorruptedWealthController>();
            if (cont != null)
            {
                if (self.IsLocked == false && self.IsLockBroken == false)
                {
                    if (cont.AmountOfCorruptKeys > 0)
                    {
                        cont.AmountOfCorruptKeys--;
                    }
                    float MA = cont.AmountOfCorruptKeys;
                    if (MA > 0)
                    {
                        SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.CORRUPTEDWEALTH_FLAG_KEY, true);
                        GameObject vfx = SpawnManager.SpawnVFX(StaticVFXStorage.MachoBraceDustupVFX, true);
                        vfx.transform.position = self.sprite.WorldCenter;
                        vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                        vfx.transform.localScale *= 1.25f;
                        UnityEngine.Object.Destroy(vfx, 2);

                        AkSoundEngine.PostEvent("Play_BOSS_DragunGold_Crackle_01", player.gameObject);
                    }
                    for (int i = 0; i < MA; i++)
                    {
                        if (player.carriedConsumables.KeyBullets == 0) { return; }
                        float H = i + 0.5f / MA;
                        PickupObject pickupObject = self.lootTable.GetSingleItemForPlayer(player, 0);
                        Vector2 t = MathToolbox.GetUnitOnCircle(Vector2.down.ToAngle() + Mathf.Lerp(-90, 90, H), 2.5f);
                        DebrisObject j = LootEngine.SpawnItem(pickupObject.gameObject, self.sprite.WorldCenter - new Vector2(0.5f, 1), t, 0.5f, true, false, false);
                        cont.AmountOfCorruptKeys--;
                        player.carriedConsumables.KeyBullets--;
                    }
                }
            }    
            */
        }


        public static void UpdateBlanksHook(Action<GameUIRoot, PlayerController> orig, GameUIRoot self, PlayerController player)
        {
            try
            {
                var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
                if (perl != null)
                {
                    if (self.blankControllers[player.IsPrimaryPlayer ? 0 : 1] == null) { return; }
                    List<dfSprite> c = self.blankControllers[player.IsPrimaryPlayer ? 0 : 1].extantBlanks;
                    if (c != null || c.Count > 0)
                    {
                        for (int i = 0; i < c.Count; i++)
                        {
                            if (i > (c.Count - 1) - perl.AmountOfCorruptBlanks)
                            {
                                c[i].Color = new Color(0.02f, 0.4f, 1, 0.6f);
                            }
                            else
                            {
                                c[i].Color = new Color32(255, 255, 255, 1);
                            }
                        }
                    }

                    dfSprite dfSprite = self.p_playerKeyLabel.Parent.gameObject.transform.GetChild(1).GetComponent<dfSprite>();
                    if (dfSprite != null)
                    {
                        if (perl != null && perl.AmountOfCorruptKeys > 0)
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
            var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
            if (perl != null)
            {
                perl.AmountOfCorruptKeys++;
            }
        }

        public static void SilencerItemPickup(Action<SilencerItem, PlayerController> orig, SilencerItem self, PlayerController player)
        {
            orig(self, player);
            var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
            if (perl != null)
            {
                perl.AmountOfCorruptBlanks++;
            }
        }

        public static void HealthPickup(Action<HealthPickup, PlayerController> orig, HealthPickup self, PlayerController player)
        {
           
            orig(self, player);

            var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
            if (perl != null)
            {
                perl.ProcessDamageModsRedHP(self.healAmount);
                if (self.armorAmount > 0)
                {
                    GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(385) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects.First().effects.First().effect, true);
                    vfx.transform.position = player.sprite.WorldCenter;
                    vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                    vfx.transform.localScale *= 2;
                    vfx.transform.localRotation = Quaternion.Euler(0, 0, Vector2.up.ToAngle());
                    UnityEngine.Object.Destroy(vfx, 2);
                }
                perl.AmountOfArmorConsumed += self.armorAmount;
            }
            /*
            CorruptedWealthController c = player.GetComponent<CorruptedWealthController>();
            if (c != null)
            {


                c.ProcessDamageModsRedHP(self.healAmount);
                if (self.armorAmount > 0)
                {
                    GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(385) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects.First().effects.First().effect, true);
                    vfx.transform.position = player.sprite.WorldCenter;
                    vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                    vfx.transform.localScale *= 2;
                    vfx.transform.localRotation = Quaternion.Euler(0, 0, Vector2.up.ToAngle()); 
                    UnityEngine.Object.Destroy(vfx, 2);
                }
                c.AmountOfArmorConsumed += self.armorAmount;
            }
            */
        }

        public static void KeyBulletPickupUpdateHook(Action<KeyBulletPickup> orig, KeyBulletPickup self)
        {
            orig(self);
            foreach(PlayerController player in GameManager.Instance.AllPlayers)
            {
                var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
                if (perl != null && self.IsRatKey == false)
                {
                    CorruptedPickupController corruptedPickup = self.GetOrAddComponent<CorruptedPickupController>();
                    corruptedPickup.pickup = CorruptedPickupController.PickupType.KEY;
                }
            }
        }

        public static void AmmoPickupUpdateHook(Action<AmmoPickup> orig, AmmoPickup self)
        {
            orig(self);

            //self.sprite.color = new Color(UnityEngine.Random.Range(0, 256), UnityEngine.Random.Range(0, 256), UnityEngine.Random.Range(0, 256), 255);
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
                if (perl != null)
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
                var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
                if (perl != null)
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
                    var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
                    if (perl != null)
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
                    var perl = player.HasPerk(CorruptedWealth.CorruptedWealthID) as CorruptedWealth;
                    if (perl != null)
                    {
                        SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.CORRUPTEDWEALTH_FLAG_GUON, true);
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
            if (self && player.gameActor && player.gameActor is PlayerController playerCont)
            {
                var glassPerk = playerCont.HasPerk(Glass.GlassID) as Glass;
                if (glassPerk != null)
                {
                    if (self.armorAmount > 0)
                    {
                        orig(self, player, selfBody);
                        return;

                    }
                    if (playerCont.ForceZeroHealthState == true | playerCont.healthHaver.currentHealth >= 1)
                    {
                        OtherTools.NotifyCustom("A Glass Curse", "Destroyed This Heart!", "glass", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);
                        glassPerk.DoHurty((self.healAmount >= 1f) ? true : false);
                        self.GetRidOfMinimapIcon();

                        self.ToggleLabel(false);
                        UnityEngine.Object.Destroy(self.gameObject);
                    }
                    else
                    {
                        orig(self, player, selfBody);
                    }
                }
                else
                {
                    orig(self, player, selfBody);
                }
            }
        }
    }
}
    

