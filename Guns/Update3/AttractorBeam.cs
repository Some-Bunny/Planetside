using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Alexandria.Assetbundle;



namespace Planetside
{
    public class AttractorBeam : GunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Tractor Beam", "attractorbeam");
            Game.Items.Rename("outdated_gun_mods:tractor_beam", "psog:tractor_beam");
            var behav = gun.gameObject.AddComponent<AttractorBeam>();            
            gun.SetShortDescription("Get Over Here!");
            gun.SetLongDescription("Pulls enemies towards you.\n\nA standard Hegemony Low-Gravity Object Relocator that has been left in the Gungeon.\n\nIt's stuck on the 'Pull' option.");


            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "attractorbeam_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.reloadAnimation = "attractorbeam_reload";
            gun.idleAnimation = "attractorbeam_idle";
            gun.shootAnimation = "attractorbeam_fire";

            /*
            gun.SetupSprite(null, "attractorbeam_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            */


            gun.isAudioLoop = true;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_WPN_blackhole_impact_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;

            for (int i = 0; i < 1; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(180) as Gun, true, false);
            }
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {
                mod.ammoCost = 4;
                if (mod != gun.DefaultModule) { mod.ammoCost = 0; }
                mod.shootStyle = ProjectileModule.ShootStyle.Beam;
                mod.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                mod.cooldownTime = 0.01f;
                mod.numberOfShotsInClip = 100;
                mod.angleVariance = 0;

            List<string> BeamAnimPaths = new List<string>()
            {
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_mid_001",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_mid_002",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_mid_003",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_mid_004",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_mid_005",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_mid_006",
            };
            List<string> StartAnimPaths = new List<string>()
            {
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_start_001",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_start_002",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_start_003",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_start_004",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_start_005",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_start_006",
            };
                
            List<string> ImpactAnimPaths = new List<string>()
            {
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_impact_001",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_impact_002",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_impact_003",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_impact_004",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_impact_005",
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_impact_006",
            };

            List<string> End = new List<string>()
            {
                "Planetside/Resources/Beams/TractorBeam/tractorbeam_mid_001",
            };

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

                BasicBeamController beamComp = projectile.GenerateBeamPrefab(
                    "Planetside/Resources/Beams/TractorBeam/tractorbeam_mid_001",
                    new Vector2(10, 10),
                    new Vector2(0, -2),
                    BeamAnimPaths,
                    48,
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
                projectile.baseData.damage = 25f;
                projectile.baseData.force *= -2f;
                projectile.baseData.range *= 5;
                projectile.baseData.speed *= 5f;
                projectile.specRigidbody.CollideWithOthers = false;
                
                EmmisiveBeams emiss = beamComp.gameObject.AddComponent<EmmisiveBeams>();
                emiss.EmissiveColorPower = 10f;
                emiss.EmissivePower = 10f;

                /*
                ImprovedAfterImageForTiled yes1 = beamComp.gameObject.AddComponent<ImprovedAfterImageForTiled>();
                yes1.spawnShadows = true;
                yes1.shadowLifetime = 0.1f;
                yes1.shadowTimeDelay = 0.001f;
                yes1.dashColor = new Color(0f, 1f, 0.3f, 0.2f);
                */

                beamComp.boneType = BasicBeamController.BeamBoneType.Straight;

                beamComp.startAudioEvent = "Play_WPN_moonscraperLaser_shot_01";
                beamComp.projectile.baseData.damage = 35;
                beamComp.endAudioEvent = "Stop_WPN_All";
                beamComp.penetration = 0;
                beamComp.reflections = 0;
                beamComp.IsReflectedBeam = false;
                

                mod.projectiles[0] = projectile;

            }

            //GUN STATS
            gun.doesScreenShake = false;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(370) as Gun).gunSwitchGroup;

            gun.reloadTime = 2.4f;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.barrelOffset.transform.localPosition = new Vector3(1.625f, 0.375f, 0f);
            gun.SetBaseMaxAmmo(400);
            gun.ammo = 400;
            gun.PreventNormalFireAudio = true;
            gun.gunClass = GunClass.BEAM;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("TractorBeam", "Planetside/Resources/GunClips/TractorBeam/tractorfull", "Planetside/Resources/GunClips/TractorBeam/tractorempty");



            gun.quality = PickupObject.ItemQuality.C; //D
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            Colossus.ColossusID = gun.PickupObjectId;


            ItemIDs.AddToList(gun.PickupObjectId);
        }
        public static int ColossusID;

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsFiring == true)
            {
                gun.CeaseAttack(false);
            }
            if (gun.IsReloading && this.HasReloaded)
            {
                gun.CeaseAttack(false, null);
                gun.PreventNormalFireAudio = true;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;
                AkSoundEngine.PostEvent("Play_WPN_blackhole_reload_01", player.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);

            }
            
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
        }
        private bool HasReloaded;
        public bool HasFlipped;

        public override void Update()
        {
            gun.PreventNormalFireAudio = true;
        }
    }
}
