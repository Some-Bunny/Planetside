using UnityEngine;
using Gungeon;
using ItemAPI;
using SaveAPI;
using System.Collections.Generic;

using System;
using System.Linq;

namespace Planetside
{
    class Riftaker : GunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Riftaker", "riftaker");
            Game.Items.Rename("outdated_gun_mods:riftaker", "psog:riftaker");
            gun.gameObject.AddComponent<Riftaker>();
            gun.SetShortDescription("Breaking the 3rd Wall");
            gun.SetLongDescription("A common theme with mad scientists is inventing some form of weapon that would be able to tear holes in reality to time travel, which ends up killing the creator.\n\nThis gun is no exception.");
            gun.SetupSprite(null, "riftaker_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 25);
            gun.SetAnimationFPS(gun.idleAnimation, 7);
            gun.SetAnimationFPS(gun.reloadAnimation,10);

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(35) as Gun, true, false);
            gun.SetBaseMaxAmmo(120);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_energy_accent_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[2].eventAudio = "Play_WPN_plasmacell_reload_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[2].triggerEvent = true;
          
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(33) as Gun).gunSwitchGroup;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SMALL_BLASTER;
            gun.reloadTime = 1.9f;
            gun.DefaultModule.angleVariance = 7f;
            gun.DefaultModule.cooldownTime = .35f;
            gun.DefaultModule.numberOfShotsInClip = 8;
            gun.quality = PickupObject.ItemQuality.B;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.DefaultModule.angleVariance = 0f;
            gun.encounterTrackable.EncounterGuid = "what the dog doin";
            gun.sprite.IsPerpendicular = true;
            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 5f;
            projectile.transform.parent = gun.barrelOffset;
            projectile.baseData.range = 8;
            projectile.baseData.force *= 0f;
            Gun gun2 = PickupObjectDatabase.GetById(97) as Gun;
            projectile.hitEffects = gun.DefaultModule.projectiles[0].hitEffects;
            projectile.AdditionalScaleMultiplier *= 1;
            projectile.gameObject.AddComponent<RiftakerProjectile>();
            projectile.SetProjectileSpriteRight("bloopky_projectile_001", 15, 7, false, tk2dBaseSprite.Anchor.MiddleCenter, 15, 7);
            projectile.shouldRotate = true;
            projectile.baseData.UsesCustomAccelerationCurve = true;
            projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1.5f, 1.5f, 0.3f);

            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(47) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(47) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            projectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);

            gun.gunClass = GunClass.RIFLE;

            gun.sprite.usesOverrideMaterial = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(215, 225, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 60);
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


            ETGMod.Databases.Items.Add(gun, false, "ANY");
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:riftaker",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "black_hole_gun",
                "singularity",
                "psog:immateria"
            };
            CustomSynergies.Add("Event Horizon", mandatoryConsoleIDs, optionalConsoleIDs, true);
            Riftaker.RiftakerID = gun.PickupObjectId;
            ItemIDs.AddToList(gun.PickupObjectId);
        }
        public static int RiftakerID;


        private bool HasReloaded;

        public override void Update()
        {
            PlayerController player = gun.CurrentOwner as PlayerController;
            if (gun.CurrentOwner)
            {
                if (gun.CurrentOwner)
                {

                    if (!gun.PreventNormalFireAudio)
                    {
                        this.gun.PreventNormalFireAudio = true;
                    }
                    if (!gun.IsReloading && !HasReloaded)
                    {
                        this.HasReloaded = true;
                    }
                }
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                base.OnReloadPressed(player, gun, bSOMETHING);
            }
            
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
        }

    }
}