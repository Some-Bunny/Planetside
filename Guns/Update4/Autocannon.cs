﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MonoMod.RuntimeDetour;
using SaveAPI;

namespace Planetside
{
    public class Autocannon : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("H.M Auto-Cannon", "autocannon");
            Game.Items.Rename("outdated_gun_mods:h.m_auto-cannon", "psog:h.m_auto-cannon");
            gun.gameObject.AddComponent<Autocannon>();
            gun.SetShortDescription("So What If I Like Really Big Guns?");
            gun.SetLongDescription("Fires faster the longer you hold the trigger, loaded with depleted uranium rounds.\n\nManufactured by the same people that produced the H.M Prime models, these portable auto-cannons deliver the same kick to your enemies as they do to you.");

            gun.SetupSprite(null, "autocannon_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 36);
            gun.SetAnimationFPS(gun.reloadAnimation, 7);
            gun.SetAnimationFPS(gun.idleAnimation, 1);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(56) as Gun, true, false);
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(56) as Gun, true, false);

            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> { { 0, "Play_WPN_rpg_reload_01" }, { 2, "Play_OBJ_mine_beep_01" }, { 4, "Play_OBJ_mine_beep_01" }, { 6, "Play_OBJ_mine_beep_01" }, { 8, "Play_OBJ_lock_unlock_01" }, { 17, "Play_OBJ_lock_unlock_01" }, { 18, "Play_OBJ_mine_set_01" } });

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(387) as Gun).gunSwitchGroup;

            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(146) as Gun).muzzleFlashEffects;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 3.2f;
            gun.DefaultModule.cooldownTime = 0.6f;
            gun.DefaultModule.numberOfShotsInClip = 110;
            gun.SetBaseMaxAmmo(550);
            gun.InfiniteAmmo = false;
            gun.gunClass = GunClass.EXPLOSIVE;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("AutoCannon", "Planetside/Resources/GunClips/AutoCannon/autocannonfull", "Planetside/Resources/GunClips/AutoCannon/autocannonempty");

            gun.barrelOffset.transform.localPosition = new Vector3(2.4375f, 0.375f, 0f);

            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {
                mod.ammoCost = 1;
                mod.shootStyle = ProjectileModule.ShootStyle.Automatic;
                mod.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Ordered;
                mod.cooldownTime = 0.25f;
                mod.angleVariance = 6f;
                mod.numberOfShotsInClip = 110;

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(mod.projectiles[0]);
                mod.projectiles[0] = projectile;
                projectile.SetProjectileSpriteRight("autocannon_projectile_001", 13, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 13, 5);
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 6f;
                projectile.baseData.speed *= 3f;


                ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
                yes.spawnShadows = true;
                yes.shadowLifetime = 0.1f;
                yes.shadowTimeDelay = 0.000001f;
                yes.dashColor = new Color(0.9f, 0.6f, 0f, 1f);


                mod.positionOffset = new Vector2(0.125f, 0.1875f);
                if (mod != gun.DefaultModule)
                {
                    mod.positionOffset = new Vector2(-0.125f, -0.1875f);
                    mod.ammoCost = 0;
                }
                projectile.transform.parent = gun.barrelOffset;
            }

            gun.GainsRateOfFireAsContinueAttack = true;
            gun.RateOfFireMultiplierAdditionPerSecond = 1.05f;

            //BULLET STATS

            gun.DefaultModule.angleVariance = 6f;
            gun.Volley.ModulesAreTiers = true;
            gun.gameObject.transform.Find("Casing").transform.position = new Vector3(1.8125f, 0.875f);
            gun.shellCasing = BreakAbleAPI.BreakableAPIToolbox.GenerateDebrisObject("Planetside/Resources/GunObjects/Casings/casingBigGun.png", true, 2, 4, 540, 300, null, 0.75f, null, null, 2).gameObject;
            gun.shellsToLaunchOnFire = 2;
            gun.shellsToLaunchOnReload = 0;
            gun.reloadShellLaunchFrame = 0;
            gun.shellCasingOnFireFrameDelay = 0;


            gun.gameObject.transform.Find("Clip").transform.position = new Vector3(1.125f, 0.1875f);
            gun.clipObject = BreakAbleAPI.BreakableAPIToolbox.GenerateDebrisObject("Planetside/Resources/GunObjects/Clips/bigassmag.png", true, 1, 3, 60, 20, null, 2, "Play_ITM_Crisis_Stone_Impact_02", null, 1).gameObject;
            gun.reloadClipLaunchFrame = 9;
            gun.clipsToLaunchOnReload = 1;

            gun.gunClass = GunClass.FULLAUTO;

            gun.quality = PickupObject.ItemQuality.B;
            ETGMod.Databases.Items.Add(gun, null, "ANY");


            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 54, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 50);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.2f);
            MeshRenderer component = gun.GetComponent<MeshRenderer>();
            if (!component)
            {
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == mat)
                {
                    return;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(mat);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;

            gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.HM_PRIME_DEFEATED, true);

            Autocannon.AutoCannonID = gun.PickupObjectId;
            ItemIDs.AddToList(gun.PickupObjectId);

        }
        public static int AutoCannonID;


        protected override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
        }

        protected override void OnPostDrop(PlayerController player)
        {
            base.OnPostDrop(player);  
        }
        public void OnDestroy()
        {
        }       
    }
}
