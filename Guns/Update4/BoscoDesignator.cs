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


    public class BoscoDesignator : GunBehaviour
    {

        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("B-05-CO Designator", "boscotargeter");
            Game.Items.Rename("outdated_gun_mods:b-05-co_designator", "psog:b-05-co_designator");
            gun.gameObject.AddComponent<LaserWelder>();
            gun.SetShortDescription("Mine It!");
            gun.SetLongDescription("Targets objects and foes for the B-05-CO drone to prioritize.\n\nSmells of beard shampoo.");

            gun.SetupSprite(null, "boscotargeter_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 16);
            gun.SetAnimationFPS(gun.idleAnimation, 1);


            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(475) as Gun).gunSwitchGroup;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_OBJ_mine_beep_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;

            //GUN STATS
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(58) as Gun).muzzleFlashEffects;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0f;
            gun.DefaultModule.cooldownTime = 0.5f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.barrelOffset.transform.localPosition = new Vector3(15f / 16f, 8f / 16f, 0f);
            gun.SetBaseMaxAmmo(1000);
            gun.InfiniteAmmo = true;
            gun.gunClass = GunClass.NONE;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BLASTER;
            
            //BULLET STATS
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 0;
            projectile.baseData.force = 0f;
            projectile.baseData.speed = 600f;
            projectile.AppliesFire = false;
            projectile.baseData.range = 1000;
            projectile.sprite.renderer.enabled = false;

            projectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(32) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(32) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(32) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(32) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            gun.DefaultModule.angleVariance = 0f;
            projectile.hitEffects.CenterDeathVFXOnProjectile = false;

            List<string> BeamAnimPaths = new List<string>()
            {
                "Planetside/Resources2/ProjectileTrails/Targeter/trageter_mid_001",
                "Planetside/Resources2/ProjectileTrails/Targeter/trageter_mid_002",
                "Planetside/Resources2/ProjectileTrails/Targeter/trageter_mid_003",
                "Planetside/Resources2/ProjectileTrails/Targeter/trageter_mid_004",
                "Planetside/Resources2/ProjectileTrails/Targeter/trageter_mid_005",
            };
            List<string> ImpactAnimPaths = new List<string>()
            {
                "Planetside/Resources2/ProjectileTrails/Targeter/trageter_start_001",
                "Planetside/Resources2/ProjectileTrails/Targeter/trageter_start_002",
                "Planetside/Resources2/ProjectileTrails/Targeter/trageter_start_003",
                "Planetside/Resources2/ProjectileTrails/Targeter/trageter_start_004",
                "Planetside/Resources2/ProjectileTrails/Targeter/trageter_start_005",
            };

                projectile.AddTrailToProjectile(
                "Planetside/Resources2/ProjectileTrails/Targeter/trageter_mid_001",
                new Vector2(11, 11),
                new Vector2(11, 11),
                BeamAnimPaths, 20,
                ImpactAnimPaths, 20,
                0.1f,
                30,
                30,
                true
                );

            gun.quality = PickupObject.ItemQuality.SPECIAL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(255, 51, 31, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 100);
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

        }
        public override void Update()
        {
            if (gun != null) { foreach (ProjectileModule mod in gun.RawSourceVolley.projectiles) { if (mod != null) { mod.angleVariance = 0f; } } }
        }
        

    }
}

