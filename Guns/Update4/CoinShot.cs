using System;
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
using HarmonyLib;

namespace Planetside
{
    public class CoinShot : AdvancedGunBehavior
    {

        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Deadeyes Handgun", "coinshot");
            Game.Items.Rename("outdated_gun_mods:deadeyes_handgun", "psog:deadeyes_handgun");
            gun.gameObject.AddComponent<CoinShot>();
            gun.SetShortDescription("Accuracy Brings Power");
            gun.SetLongDescription("A weak handgun fitted with a small coin printer. Charge up to expel a shootable coin, at a high cost.\n\nOnce carried by a Gungeoneer with no name, despite never aiming for their target, would always walk out unscathed, their foes with lead between their eyes.");

            gun.SetupSprite(null, "coinshot_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            gun.SetAnimationFPS(gun.idleAnimation, 1);
            gun.SetAnimationFPS(gun.chargeAnimation, 6);
            gun.SetAnimationFPS(gun.alternateShootAnimation, 13);

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(384) as Gun).gunSwitchGroup;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(401) as Gun).muzzleFlashEffects;

            /*
            gun.spriteAnimator.Library.clips.AddItem(
            new tk2dSpriteAnimationClip()
            {
                fps = 10,
                name = "coinshot_chargefire",
                wrapMode = tk2dSpriteAnimationClip.WrapMode.Once,
                frames = new tk2dSpriteAnimationFrame[]
                {

                }
            }
            );
            */


            for (int i = 0; i < 1; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(384) as Gun, true, true);
            }
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>();
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.ammoCost = 1;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.Charged;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.cooldownTime = 0.3f;
                projectileModule.angleVariance = 0;
                projectileModule.numberOfShotsInClip = 10;
                

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(88) as Gun).DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                projectile.baseData.damage = 5f;
                projectile.AdditionalScaleMultiplier = 1f;
                projectile.shouldRotate = true;
                projectile.baseData.range = 18f;
                projectile.baseData.speed *= 1.5f;
                var cADM = projectile.gameObject.AddComponent<CoinArbitraryDamageMultiplier>();
                cADM.Multiplier = 8f;
                cADM.CanChangeMultiplier = true;
                cADM.CustomMultiplierChangeValue = 1f;
                //CoinShotComponent



                //projectile.SetProjectileSpriteRight("chargergun_projectile_001", 7, 3, false, tk2dBaseSprite.Anchor.MiddleCenter, 7, 3);
                gun.DefaultModule.projectiles[0] = projectile;

                projectile.objectImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
                projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;


                projectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(43) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
                projectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(43) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
                projectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(43) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
                projectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(43) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);

                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                if (projectileModule != gun.DefaultModule)
                {
                    projectileModule.ammoCost = 0;
                }

                ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
                {
                    Projectile = projectile,
                    ChargeTime = 0f
                };
                ProjectileModule.ChargeProjectile coinshot = new ProjectileModule.ChargeProjectile
                {
                    Projectile = CoinTosser.CoinProjectile,
                    ChargeTime = 0.8f,
                    AmmoCost = 15,
                    OverrideShootAnimation = gun.alternateShootAnimation,
                    AdditionalWwiseEvent = "Play_WPN_m1rifle_reload_01",
                    UsedProperties = ProjectileModule.ChargeProjectileProperties.shootAnim | ProjectileModule.ChargeProjectileProperties.ammo | ProjectileModule.ChargeProjectileProperties.additionalWwiseEvent,
                };
              

                projectileModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>() { item2, coinshot };
                gun.DefaultModule.chargeProjectiles.Add(item2);
            }

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 6;


            gun.gameObject.transform.Find("Casing").transform.position = new Vector3(1.1875f, 0.4375f);
            gun.shellsToLaunchOnFire = 0;
            gun.shellsToLaunchOnReload = 0;
            gun.reloadShellLaunchFrame = 0;
            gun.shellCasingOnFireFrameDelay = 0;


            gun.gameObject.transform.Find("Clip").transform.position = new Vector3(1.3125f, 0.4375f);
            gun.clipObject = BreakAbleAPI.BreakableAPIToolbox.GenerateDebrisObject("Planetside/Resources/GunObjects/Clips/bulldogclip.png").gameObject;
            gun.reloadClipLaunchFrame = 8;
            gun.clipsToLaunchOnReload = 1;

            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, 0.5f, 0f);
            gun.reloadTime = 2.2f;
            gun.ammo = 300;

            gun.quality = PickupObject.ItemQuality.D;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.DODGELOAD);

            
            ThunderShot.ThunderShotID = gun.PickupObjectId;
            ItemIDs.AddToList(gun.PickupObjectId);
            gun.gunClass = GunClass.PISTOL;

        }
        public static int CoinShotID;


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

