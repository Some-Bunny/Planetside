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


namespace Planetside
{


    public class PulseCannon : AdvancedGunBehavior
    {

        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Radial Pulse-Cannon", "pulsecannon");
            Game.Items.Rename("outdated_gun_mods:radial_pulsecannon", "psog:radial_pulsecannon");
            gun.gameObject.AddComponent<PulseCannon>();
            gun.SetShortDescription("Good For Frying Dwarves");
            gun.SetLongDescription("A downscaled version of a pulse cannon that would be attached to a colossal battle-tower. They were decommisioned after a rise of employees reporting their legs were getting cut off.");

            GunExt.SetupSprite(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "pulsecannon_idle_001", 11);
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.reloadAnimation = "pulsecannon_reload";
            gun.idleAnimation = "pulsecannon_idle";
            gun.shootAnimation = "pulsecannon_fire";

            /*
            gun.SetupSprite(null, "pulsecannon_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 16);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            gun.SetAnimationFPS(gun.idleAnimation, 1);
            */


            for (int e = 0; e < 4; e++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(181) as Gun, true, false);
            }
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(23) as Gun).muzzleFlashEffects;
            int i = 0;
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {
                i++;
                mod.ammoCost = 1;
                mod.shootStyle = ProjectileModule.ShootStyle.Automatic;
                mod.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                mod.cooldownTime = 0.275f;
                mod.angleVariance = 0f;
                mod.angleFromAim = 90 * i;
                mod.numberOfShotsInClip = 80;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(mod.projectiles[0]);
                mod.projectiles[0] = projectile;

                gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
                gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("RadPulseCannon", "Planetside/Resources/GunClips/PulseCannon/pulseclipFull", "Planetside/Resources/GunClips/PulseCannon/pulseclipEmpty");

                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 20f;
                projectile.baseData.speed *= 0.2f;
                projectile.shouldRotate = true;
                projectile.objectImpactEventName = (PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
                projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

                projectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
                projectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
                projectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
                projectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);


                ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
                yes.spawnShadows = true;
                yes.shadowLifetime = 0.2f;
                yes.shadowTimeDelay = 0.01f;
                yes.dashColor = new Color(1f, 0f, 0.6f, 1f);

                projectile.AnimateProjectile(new List<string> {
                "pulseShot_001",
                "pulseShot_002",
                "pulseShot_003",
                "pulseShot_004"
                }, 11, true, new List<IntVector2> {
                new IntVector2(5, 14),
                new IntVector2(5, 14),
                new IntVector2(5, 14),
                new IntVector2(5, 14),
                }, AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 7), AnimateBullet.ConstructListOfSameValues(true, 7), AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 7));
            }
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(54) as Gun).gunSwitchGroup;
            gun.reloadTime = 1.4f;
            gun.barrelOffset.transform.localPosition = new Vector3(1f, 0.8f, 0f);
            gun.SetBaseMaxAmmo(1200);
            gun.ammo = 1200;

            gun.gunClass = GunClass.SILLY;

            gun.quality = PickupObject.ItemQuality.C;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            List<string> mandatoryConsoleIDs1 = new List<string>
            {
                "psog:radial_pulsecannon"
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "honeycomb",
                "hexagun",
                "sixth_chamber",
            };
            CustomSynergies.Add("Hexadecimally!", mandatoryConsoleIDs1, optionalConsoleIDs, true);
            PulseCannonID = gun.PickupObjectId;
            ItemIDs.AddToList(gun.PickupObjectId);
        }
        public static int PulseCannonID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            float AddOn = UnityEngine.Random.Range(30, 75);
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            { mod.angleFromAim += AddOn;}
        }

        public override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
        }

        public override void OnPostDrop(PlayerController player)
        {
            base.OnPostDrop(player);  
        }
        public void OnDestroy()
        {
        }
    }
}


