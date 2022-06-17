using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;



namespace Planetside
{
    public class PerfectedColossus : GunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Perfected Colossus", "prefectedcolossus");
            Game.Items.Rename("outdated_gun_mods:perfected_colossus", "psog:perfected_colossus");
            var behav = gun.gameObject.AddComponent<PerfectedColossus>();
            gun.SetShortDescription("Lunar Design");
            gun.SetLongDescription("A collection of rocks powered by the blood of a now-dead demi-god, who used their own blood to create the Titans found on a planet far, far away.");

            gun.SetupSprite(null, "prefectedcolossus_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);

            gun.isAudioLoop = true;

            //int iterator = 0;
            for (int i = 0; i < 1; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);
            }
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {
                //if (iterator == 1) mod.angleFromAim = 30;
                //if (iterator == 2) mod.angleFromAim = -30;
                //iterator++;
                mod.ammoCost = 10;
                if (mod != gun.DefaultModule) { mod.ammoCost = 0; }
                mod.shootStyle = ProjectileModule.ShootStyle.Beam;
                mod.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                mod.cooldownTime = 0.001f;
                mod.numberOfShotsInClip = 40;
                mod.ammoType = GameUIAmmoType.AmmoType.BEAM;

                List<string> BeamAnimPaths = new List<string>()
            {
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_mid_001",
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_mid_002",
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_mid_003",
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_mid_004",

            };
                List<string> StartAnimPaths = new List<string>()
            {
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_start_001",
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_start_002",
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_start_003",
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_start_004",


            };
                
            List<string> ImpactAnimPaths = new List<string>()
            {
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_impact_001",
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_impact_002",
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_impact_003",
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_impact_004",

            };

            List<string> End = new List<string>()
            {
                "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_end_001",

            };

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

                BasicBeamController beamComp = projectile.GenerateBeamPrefab(
                    "Planetside/Resources/Beams/ColossusPerfected/colossusbeam_mid_001",
                    new Vector2(10, 2),
                    new Vector2(0, 4),
                    BeamAnimPaths,
                    12,
                    //Beam Impact
                    ImpactAnimPaths,
                    12,
                    new Vector2(4, 4),
                    new Vector2(7, 7),
                    //End of the Beam
                    null,
                    -1,
                    null,
                    null,
                    //Start of the Beam
                    StartAnimPaths,
                    12,
                    new Vector2(10, 2),
                    new Vector2(0, 4)
                    );

                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 100f;
                projectile.baseData.force *= 1f;
                projectile.baseData.range *= 5;
                projectile.baseData.speed *= 5f;
                beamComp.gameObject.AddComponent<EmmisiveBeams>();
                beamComp.boneType = BasicBeamController.BeamBoneType.Straight;

                beamComp.startAudioEvent = "Play_ENM_deathray_shot_01";
                beamComp.projectile.baseData.damage = 100;
                beamComp.endAudioEvent = "Stop_ENM_deathray_loop_01";
                //beamComp.interpolateStretchedBones = false;

                mod.projectiles[0] = projectile;

                gun.sprite.usesOverrideMaterial = true;

                Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat.SetColor("_EmissiveColor", new Color32(108, 172, 255, 255));
                mat.SetFloat("_EmissiveColorPower", 1.55f);
                mat.SetFloat("_EmissivePower", 100);
                mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
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

            //GUN STATS
            gun.doesScreenShake = false;
            gun.reloadTime = 1f;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.barrelOffset.transform.localPosition = new Vector3(1.625f, 0.5625f, 0f);
            gun.SetBaseMaxAmmo(360);
            gun.ammo = 360;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 1;

            gun.quality = PickupObject.ItemQuality.EXCLUDED; //D
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            PerfectedColossus.PerfectedColossusID = gun.PickupObjectId;
        }
        public static int PerfectedColossusID;

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;
                AkSoundEngine.PostEvent("Play_ENM_statue_stomp_01", player.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
            }
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
        }
        private bool HasReloaded;

        protected void Update()
        {
            PlayerController player = gun.CurrentOwner as PlayerController;
            if (gun.CurrentOwner)
            {
                
                gun.PreventNormalFireAudio = true;

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
        public PerfectedColossus()
        {

        }
    }
}
