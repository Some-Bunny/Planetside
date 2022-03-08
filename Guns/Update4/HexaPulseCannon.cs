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
    public class HexaPulseCannon : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Hexagonal Pulse-Cannon", "pulsecannonYellow");
            Game.Items.Rename("outdated_gun_mods:hexagonal_pulse-cannon", "psog:hexagonal_pulse-cannon");
            gun.gameObject.AddComponent<HexaPulseCannon>();
            gun.SetShortDescription("Good For Frying Dwarves");
            gun.SetLongDescription("Fires rounds with embedded detonators in them.\n\nSmells of beard shampoo.");

            gun.SetupSprite(null, "pulsecannonYellow_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 16);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            gun.SetAnimationFPS(gun.idleAnimation, 1);

            for (int e = 0; e < 6; e++)
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
                mod.angleFromAim = 60 * i;
                mod.numberOfShotsInClip = 80;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(mod.projectiles[0]);
                mod.projectiles[0] = projectile;

                gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
                gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("HexPulseCannon", "Planetside/Resources/GunClips/Pulse/pulseclipFull", "Planetside/Resources/GunClips/Pulse/pulseclipEmpty");

                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 23f;
                projectile.baseData.speed *= 0.15f;
                projectile.shouldRotate = true;
                projectile.objectImpactEventName = (PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
                projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

                projectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects.First().effects.First().effect);
                projectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects.First().effects.First().effect);
                projectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects.First().effects.First().effect);
                projectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects.First().effects.First().effect);


                ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
                yes.spawnShadows = true;
                yes.shadowLifetime = 0.25f;
                yes.shadowTimeDelay = 0.01f;
                yes.dashColor = new Color(1f, 0.2f, 0f, 1f);

                projectile.AnimateProjectile(new List<string> {
                "pulseShotYellow_001",
                "pulseShotYellow_002",
                "pulseShotYellow_003",
                "pulseShotYellow_004"
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
            gun.gunClass = GunClass.SILLY;

            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            HexaPluseID = gun.PickupObjectId;
        }
        public static int HexaPluseID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            float AddOn = UnityEngine.Random.Range(30, 75);
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            { mod.angleFromAim += AddOn;}
        }

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

