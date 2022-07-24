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

namespace Planetside
{


    public class PunctureWound : GunBehaviour
    {

        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Puncture Wound", "puncturewound");
            Game.Items.Rename("outdated_gun_mods:puncture_wound", "psog:puncture_wound");
            PunctureWound wound = gun.gameObject.AddComponent<PunctureWound>();
            gun.SetShortDescription("Aging Agent");
            gun.SetLongDescription("Fires vials of tarnishing liquid.\n\nDO NOT HANDLE THE VIALS WITHOUT GLOVES OR PROPER PROTECTIVE EQUIPMENT.");

            gun.SetupSprite(null, "puncturewound_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 16);
            gun.SetAnimationFPS(gun.idleAnimation, 6);
            gun.SetAnimationFPS(gun.chargeAnimation, 6);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);


            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(210) as Gun).gunSwitchGroup;

            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> { { 0, "Play_WPN_rpg_reload_01" }, { 2, "Play_OBJ_bottle_cork_01" }, {9, "Play_WPN_38special_reload_01" } });
            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.shootAnimation, new Dictionary<int, string> { { 0, "Play_Railgun" } });


            EnemyToolbox.AddEventTriggersToAnimation(gun.spriteAnimator, gun.chargeAnimation, new Dictionary<int, string> { {6, "SpeedUpAnim" } });
            EnemyToolbox.AddEventTriggersToAnimation(gun.spriteAnimator, gun.shootAnimation, new Dictionary<int, string> { { 0, "SlowDownAnim" } });
            EnemyToolbox.AddEventTriggersToAnimation(gun.spriteAnimator, gun.idleAnimation, new Dictionary<int, string> { { 0, "SlowDownAnim" } });
            EnemyToolbox.AddEventTriggersToAnimation(gun.spriteAnimator, gun.reloadAnimation, new Dictionary<int, string> { { 0, "SlowDownAnim" } });


            gun.spriteAnimator.AnimationEventTriggered += wound.AnimationEventTriggered;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 6;

            //GUN STATS
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(444) as Gun).muzzleFlashEffects;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.7f;
            gun.DefaultModule.cooldownTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.barrelOffset.transform.localPosition = new Vector3(24f / 16f, 8f / 16f, 0f);
            gun.SetBaseMaxAmmo(35);
            gun.gunClass = GunClass.CHARGE;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("PunctureWoundPSOG", "Planetside/Resources/GunClips/PunctureWound/punctureFull", "Planetside/Resources/GunClips/PunctureWound/punctureEmpty");

            //BULLET STATS
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 18;
            projectile.baseData.force = 11f;
            projectile.baseData.speed = 600f;
            projectile.AppliesFire = false;
            projectile.baseData.range = 9;
            projectile.sprite.renderer.enabled = false;

            projectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(146) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(146) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(146) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(146) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            gun.DefaultModule.angleVariance = 0f;
            projectile.hitEffects.CenterDeathVFXOnProjectile = false;
            projectile.gameObject.AddComponent<PunctureWoundProjectile>();

            List<string> BeamAnimPaths = new List<string>()
            {
                "Planetside/Resources2/ProjectileTrails/PunctureWound/puncturewound_trailmid_001",
                "Planetside/Resources2/ProjectileTrails/PunctureWound/puncturewound_trailmid_002",
                "Planetside/Resources2/ProjectileTrails/PunctureWound/puncturewound_trailmid_003",
                "Planetside/Resources2/ProjectileTrails/PunctureWound/puncturewound_trailmid_004",
            };
            List<string> ImpactAnimPaths = new List<string>()
            {
                "Planetside/Resources2/ProjectileTrails/PunctureWound/puncturewound_startmid_001",
                "Planetside/Resources2/ProjectileTrails/PunctureWound/puncturewound_startmid_002",
                "Planetside/Resources2/ProjectileTrails/PunctureWound/puncturewound_startmid_003",
                "Planetside/Resources2/ProjectileTrails/PunctureWound/puncturewound_startmid_004",
            };

               TrailController trail = projectile.AddTrailToProjectile(
                "Planetside/Resources2/ProjectileTrails/PunctureWound/puncturewound_trailmid_001",
                new Vector2(16, 8),
                new Vector2(0, 4),
                BeamAnimPaths, 20,
                ImpactAnimPaths, 20,
                0.1f,
                30,
                30,
                true
                );


            EmmisiveTrail emis = projectile.gameObject.AddComponent<EmmisiveTrail>();
            emis.EmissiveColorPower = 10;
            emis.EmissivePower = 100;



            ProjectileModule.ChargeProjectile chargeproj = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 1f,
                AdditionalWwiseEvent = "Play_OBJ_pastkiller_charge_01",
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            { chargeproj };

            gun.gameObject.transform.Find("Clip").transform.position = new Vector3(0.3125f, 0.5f);
            gun.clipObject = BreakAbleAPI.BreakableAPIToolbox.GenerateDebrisObject("Planetside/Resources/GunObjects/Clips/glassVialClip.png", true, 1, 3, 180, 60, null, 0.6f, null, null, 3).gameObject;
            gun.reloadClipLaunchFrame = 2;
            gun.clipsToLaunchOnReload = 1;

            gun.quality = PickupObject.ItemQuality.C;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            PunctureWoundID = gun.PickupObjectId;
            ItemIDs.AddToList(gun.PickupObjectId);
        }
        public static int PunctureWoundID;
        private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
        {
            if (clip.GetFrame(frameIdx).eventInfo.Contains("SpeedUpAnim"))
            {
                gun.spriteAnimator.GetClipByName(gun.chargeAnimation).fps = 26;
                gun.spriteAnimator.UpdateAnimation(0.1f);
            }
            if (clip.GetFrame(frameIdx).eventInfo.Contains("SlowDownAnim"))
            {
                gun.spriteAnimator.GetClipByName(gun.chargeAnimation).fps = 6;
                gun.spriteAnimator.UpdateAnimation(0.1f);
            }
        }
    }
}

