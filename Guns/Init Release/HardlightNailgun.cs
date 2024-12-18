using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.Assetbundle;
using Gungeon;
using ItemAPI;
using UnityEngine;

namespace Planetside
{
    class HardlightNailgun : MultiActiveReloadController
    {
        public static void Add()
        {
            string shorthandName = "hardlight_nailgun";
            Gun gun = ETGMod.Databases.Items.NewGun("Hardlight Nailgun", "rake");
            Game.Items.Rename("outdated_gun_mods:" + shorthandName, "psog:" + shorthandName);
            var behav = gun.gameObject.AddComponent<HardlightNailgun>();
            gun.SetShortDescription("Hammer To A Gunfight");
            gun.SetLongDescription("On reloading, activating the active reload switches modes. \n\nA revolutionary nailgun that nails together materials with solid light! This one was weaponized.");



            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "hardlightnailgun_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.idleAnimation = "hardlightnailgun_idle";
            gun.shootAnimation = "hardlightnailgun_fire";
            gun.reloadAnimation = "hardlightnailgun_reload";

            gun.alternateIdleAnimation = "hardlightnailgun_altidle";
            gun.alternateShootAnimation = "hardlightnailgun_altfire";
            gun.alternateReloadAnimation = "hardlightnailgun_altreload";

            //gun.SetupSprite(null, "rake_idle_001", 6);
            //gun.SetAnimationFPS(gun.shootAnimation, 24);


            //gun.alternateIdleAnimation


            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(26) as Gun, true, false);
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.ammoCost = 1;

            
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_dl45heavylaser_shot_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_WPN_dl45heavylaser_reload";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.alternateShootAnimation).frames[0].eventAudio = "Play_WPN_dl45heavylaser_shot_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.alternateShootAnimation).frames[0].triggerEvent = true;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.alternateReloadAnimation).frames[0].eventAudio = "Play_WPN_dl45heavylaser_reload";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.alternateReloadAnimation).frames[0].triggerEvent = true;


            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(89) as Gun).gunSwitchGroup;

            Projectile spear = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(377) as Gun).DefaultModule.projectiles[0]);
            spear.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(spear.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(spear);
            spear.baseData.damage = 13.5f;
            spear.baseData.speed *= 3f;
            spear.baseData.force = 0f;
            spear.baseData.range = 10;


            Projectile replacementProjectile = spear.projectile;
            gun.DefaultModule.usesOptionalFinalProjectile = true;
            gun.DefaultModule.numberOfFinalProjectiles = 0;
            gun.DefaultModule.finalProjectile = replacementProjectile;
            gun.DefaultModule.finalCustomAmmoType = gun.DefaultModule.customAmmoType;
            gun.DefaultModule.finalAmmoType = gun.DefaultModule.ammoType;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.4f;
            gun.DefaultModule.cooldownTime = 0.3f;
            //gun.gunClass = GunClass.SHITTY;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.quality = PickupObject.ItemQuality.C;
            Gun gun2 = PickupObjectDatabase.GetById(223) as Gun;
            gun.muzzleFlashEffects = gun2.muzzleFlashEffects;
            gun.gunClass = GunClass.PISTOL;
            gun.PreventNormalFireAudio = true;


            gun.barrelOffset.transform.localPosition = new Vector3(0.75f, .625f, 0f);
            gun.DefaultModule.angleVariance = 4f;
            gun.encounterTrackable.EncounterGuid = "MUL-T GANG";
            gun.sprite.IsPerpendicular = true;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 18f;
            projectile.baseData.speed *= 1.25f;
            projectile.baseData.force *= 4f;
            projectile.baseData.range *= 4;
            projectile.HasDefaultTint = true;
            projectile.DefaultTintColor = new Color(255f, 255f, 255f).WithAlpha(0.95f);

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            projectile.transform.parent = gun.barrelOffset;
            behav.activeReloadEnabled = true;
            behav.canAttemptActiveReload = true;


            behav.reloads = new List<MultiActiveReloadData>
            {
                new MultiActiveReloadData(0.5f, 40, 60, 32, 20, false, false, new ActiveReloadData
                {
                    damageMultiply = 1f,
                }, true, "SwitchClip"),

            };
            HardlightNailgun.HardAsNailsID = gun.PickupObjectId;
            ItemIDs.AddToList(gun.PickupObjectId);
        }
        public static int HardAsNailsID;

        private bool HasReloaded;
        private bool Switchclip = false;
        public override void Update()
        {
            base.Update();
            if (gun.CurrentOwner)
            {
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
        }

        public override void OnPickup(GameActor owner)
        {
            base.OnPickup(owner);
        }

        public override void OnPickedUpByPlayer(PlayerController player)
        {
            base.OnPickedUpByPlayer(player);
        }

        public override void OnPostDroppedByPlayer(PlayerController player)
        {
            base.OnPostDroppedByPlayer(player);
        }

        public override void OnReloadEndedSafe(PlayerController player, Gun gun)
        {
            base.OnReloadEndedSafe(player, gun);
            //this.NoClip(player);
        }
        public override void PostProcessVolley(ProjectileVolleyData volley)
        {
            base.PostProcessVolley(volley);
        }
        public override void OnActiveReloadSuccess(MultiActiveReload reload)
        {
            base.OnActiveReloadSuccess(reload);
            PlayerController player = gun.CurrentOwner as PlayerController;
            if (reload.Name == "SwitchClip")
            {
                if (Switchclip == false)
                {
                    this.AllClip(player);
                }
                else
                {
                    this.NoClip(player);
                }
            }
        }


        public override void OnActiveReloadFailure(MultiActiveReload reload)
        {
            base.OnActiveReloadFailure(reload);
            //this.NoClip(gun.CurrentOwner as PlayerController);
        }

        private void AllClip(PlayerController player)
        {
            if (Switchclip == true ) { return; }
            this.gun.spriteAnimator.Play("hardlightnailgun_switch");
            Switchclip = true;
            float clip = (player.stats.GetStatValue(PlayerStats.StatType.AdditionalClipCapacityMultiplier));
            int num = (int)(10 * clip);
            foreach (ProjectileModule projectileModule in this.gun.Volley.projectiles)
            {
                this.gun.DefaultModule.numberOfFinalProjectiles = num;
            }
            BraveUtility.Swap(ref gun.idleAnimation, ref gun.alternateIdleAnimation);
            BraveUtility.Swap(ref gun.shootAnimation, ref gun.alternateShootAnimation);
            BraveUtility.Swap(ref gun.reloadAnimation, ref gun.alternateReloadAnimation);
        }
        private void NoClip(PlayerController player)
        {
            if (Switchclip == false) {return; }
            this.gun.spriteAnimator.Play("hardlightnailgun_switchback");
            Switchclip = false;
            foreach (ProjectileModule projectileModule in this.gun.Volley.projectiles)
            {
                this.gun.DefaultModule.numberOfFinalProjectiles = 0;
            }
            BraveUtility.Swap(ref gun.idleAnimation, ref gun.alternateIdleAnimation);
            BraveUtility.Swap(ref gun.shootAnimation, ref gun.alternateShootAnimation);
            BraveUtility.Swap(ref gun.reloadAnimation, ref gun.alternateReloadAnimation);
        }
    }
}