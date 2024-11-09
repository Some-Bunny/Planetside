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
    public class Colossus : GunBehaviour
    {
        public Material Material;
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Colossus", "colossus");
            Game.Items.Rename("outdated_gun_mods:colossus", "psog:colossus");
            var behav = gun.gameObject.AddComponent<Colossus>();
            //behav.overrideNormalFireAudio = "Play_ENM_shelleton_beam_01";

            gun.SetShortDescription("Titanic");
            gun.SetLongDescription("A collection of rocks powered by only remaining blood of a slain demi-god, who originally used their own blood to create and fuel Titans found on a distant planet.");

            gun.SetupSprite(null, "colossus_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);

            gun.isAudioLoop = true;
            gun.gunSwitchGroup = string.Empty;
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


                List<string> BeamAnimPaths = new List<string>()
            {
                "Planetside/Resources/Beams/Colossus/colossusbeam_mid_001",
                "Planetside/Resources/Beams/Colossus/colossusbeam_mid_002",
                "Planetside/Resources/Beams/Colossus/colossusbeam_mid_003",
                "Planetside/Resources/Beams/Colossus/colossusbeam_mid_004",

            };
                List<string> StartAnimPaths = new List<string>()
            {
                "Planetside/Resources/Beams/Colossus/colossusbeam_start_001",
                "Planetside/Resources/Beams/Colossus/colossusbeam_start_002",
                "Planetside/Resources/Beams/Colossus/colossusbeam_start_003",
                "Planetside/Resources/Beams/Colossus/colossusbeam_start_004",


            };

                List<string> ImpactAnimPaths = new List<string>()
            {
                "Planetside/Resources/Beams/Colossus/colossusbeam_impact_001",
                "Planetside/Resources/Beams/Colossus/colossusbeam_impact_002",
                "Planetside/Resources/Beams/Colossus/colossusbeam_impact_003",
                "Planetside/Resources/Beams/Colossus/colossusbeam_impact_004",

            };

                List<string> End = new List<string>()
            {
                "Planetside/Resources/Beams/Colossus/colossusbeam_end_001",

            };

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

                BasicBeamController beamComp = projectile.GenerateBeamPrefab(
                    "Planetside/Resources/Beams/Colossus/colossusbeam_mid_001",
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
                projectile.baseData.damage = 90f;
                projectile.baseData.force *= 1f;
                projectile.baseData.range *= 5;
                projectile.baseData.speed *= 5f;
                projectile.specRigidbody.CollideWithOthers = false;

                EmmisiveBeams emiss = beamComp.gameObject.AddComponent<EmmisiveBeams>();
                emiss.EmissiveColorPower = 10f;
                emiss.EmissivePower = 100;

                beamComp.boneType = BasicBeamController.BeamBoneType.Projectile;

                beamComp.startAudioEvent = "Play_ENM_deathray_shot_01";
                beamComp.projectile.baseData.damage = 80;
                beamComp.endAudioEvent = "Stop_ENM_deathray_loop_01";
                beamComp.penetration = 1;
                beamComp.reflections = 0;
                beamComp.IsReflectedBeam = false;
                mod.projectiles[0] = projectile;

                gun.sprite.usesOverrideMaterial = true;

                Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat.SetColor("_EmissiveColor", new Color32(255, 225, 255, 255));
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

                behav.Material = material;

                sharedMaterials[sharedMaterials.Length - 1] = material;
                component.sharedMaterials = sharedMaterials;

                Gun CopyProjectileFrom = PickupObjectDatabase.GetById(56) as Gun;
                Projectile perfProjectile = UnityEngine.Object.Instantiate<Projectile>(CopyProjectileFrom.DefaultModule.projectiles[0]);
                perfProjectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(perfProjectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(perfProjectile);
                perfProjectile.baseData.damage = 22f;
                perfProjectile.baseData.speed = 1f;
                perfProjectile.AdditionalScaleMultiplier *= 1f;
                perfProjectile.shouldRotate = true;
                perfProjectile.pierceMinorBreakables = true;
                perfProjectile.gameObject.AddComponent<PerfectedProjectileComponent>();
                perfProjectile.AnimateProjectile(new List<string> {
                "perfectedProj_001",
                "perfectedProj_002",
                "perfectedProj_003",
                "perfectedProj_004",
            }, 13, true, new List<IntVector2> {
                new IntVector2(11, 11),
                new IntVector2(11, 11),
                new IntVector2(11, 11),
                new IntVector2(11, 11)
            }, AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 7), AnimateBullet.ConstructListOfSameValues(true, 7), AnimateBullet.ConstructListOfSameValues(false, 7),
                AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 7));
                perfProjectile.hitEffects.alwaysUseMidair = true;
                perfProjectile.hitEffects.overrideMidairDeathVFX = (PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
                PerfectedProjectile = perfProjectile;
            }

            //GUN STATS
            gun.doesScreenShake = false;
            gun.reloadTime = 1.3f;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.barrelOffset.transform.localPosition = new Vector3(1.625f, 0.5625f, 0f);
            gun.SetBaseMaxAmmo(360);
            gun.ammo = 360;
            gun.PreventNormalFireAudio = true;
            gun.gunClass = GunClass.BEAM;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Colossus Red", "Planetside/Resources/GunClips/Colossus/colossusfull", "Planetside/Resources/GunClips/Colossus/colossusempty");

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 1;



            gun.quality = PickupObject.ItemQuality.B;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:colossus",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "clear_guon_stone",
                "bottle",
                "heart_bottle"
            };
            CustomSynergies.Add("Perfected", mandatoryConsoleIDs, optionalConsoleIDs, false);
            Colossus.ColossusID = gun.PickupObjectId;
            ItemIDs.AddToList(gun.PickupObjectId);

            AdvancedTransformGunSynergyProcessor colos = (PickupObjectDatabase.GetById(gun.PickupObjectId) as Gun).gameObject.AddComponent<AdvancedTransformGunSynergyProcessor>();
            colos.NonSynergyGunId = gun.PickupObjectId;
            colos.SynergyGunId = PerfectedColossus.PerfectedColossusID;
            colos.SynergyToCheck = "Perfected";

        }
        public static int ColossusID;
        public static Projectile PerfectedProjectile;


        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsFiring == true)
            {
                gun.CeaseAttack(false);
            }
            if (gun.IsReloading && this.HasReloaded)
            {
                gun.CeaseAttack(false, null);
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;
                AkSoundEngine.PostEvent("Play_ENM_statue_stomp_01", player.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                if (player.PlayerHasActiveSynergy("Perfected") && gun.ClipShotsRemaining <= gun.ClipCapacity / 2)
                {
                    player.StartCoroutine(SpawnPerfectedShots(player));
                }
            }
        }

        public IEnumerator SpawnPerfectedShots(PlayerController player)
        {
            for (int i = 0; i < 3; i++)
            {
                AkSoundEngine.PostEvent("Play_WPN_chargelaser_shot_01", player.gameObject);
                float finaldir = ProjSpawnHelper.GetAccuracyAngled(player.CurrentGun.CurrentAngle, 0, player);
                if (i == 0)
                {
                    Vector2 Point1 = MathToolbox.GetUnitOnCircle(90, 1f);
                    GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(PerfectedProjectile.gameObject, player.sprite.WorldCenter + Point1, Quaternion.Euler(0f, 0f, finaldir), true);
                    Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                    if (component != null)
                    {
                        component.Owner = player;
                        component.Shooter = player.specRigidbody;
                    }
                    GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
                    tk2dBaseSprite ballib = vfx.GetComponent<tk2dBaseSprite>();
                    ballib.PlaceAtPositionByAnchor(Point1, tk2dBaseSprite.Anchor.MiddleCenter);
                    ballib.HeightOffGround = 35f;
                    ballib.UpdateZDepth();
                }
                else
                {
                    for (int e = -1; e < 1; e++)
                    {
                        float Maff = e == -1 ? -45 : 45;
                        Vector2 Point1 = MathToolbox.GetUnitOnCircle((Maff * i) + 90, 1f);
                        GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(PerfectedProjectile.gameObject, player.sprite.WorldCenter + Point1, Quaternion.Euler(0f, 0f, finaldir), true);
                        Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                        if (component != null)
                        {
                            component.Owner = player;
                            component.Shooter = player.specRigidbody;
                        }
                        GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
                        tk2dBaseSprite ballib = vfx.GetComponent<tk2dBaseSprite>();
                        ballib.PlaceAtPositionByAnchor(Point1, tk2dBaseSprite.Anchor.MiddleCenter);
                        ballib.HeightOffGround = 35f;
                        ballib.UpdateZDepth();
                    }
                }
                yield return new WaitForSeconds(0.15f);
            }
            yield break;
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
        }
        private bool HasReloaded;
        public bool HasFlipped;

        public override void Update()
        {
            PlayerController player = gun.CurrentOwner as PlayerController;
            if (gun.CurrentOwner)
            {

                if (player.PlayerHasActiveSynergy("Perfected") && HasFlipped == false)
                {
                    HasFlipped = true;
                    Material.SetColor("_EmissiveColor", new Color32(108, 172, 255, 255));
                    Material.SetFloat("_EmissiveColorPower", 1.55f);
                    Material.SetFloat("_EmissivePower", 100);
                    Material.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                    Material.SetTexture("_MainTex", gun.sprite.renderer.material.GetTexture("_MainTex"));
                    gun.sprite.usesOverrideMaterial = true;
                    gun.sprite.renderer.material = Material;

                }
                else if (!player.PlayerHasActiveSynergy("Perfected") && HasFlipped != false)
                {
                    HasFlipped = false;
                    Material.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
                    Material.SetFloat("_EmissiveColorPower", 1.55f);
                    Material.SetFloat("_EmissivePower", 100);
                    Material.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                    Material.SetTexture("_MainTex", gun.sprite.renderer.material.GetTexture("_MainTex"));
                    gun.sprite.usesOverrideMaterial = true;
                    gun.sprite.renderer.material = Material;
                    /*
                    Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                    mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
                    mat.SetFloat("_EmissiveColorPower", 1.55f);
                    mat.SetFloat("_EmissivePower", 100);
                    mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                    MeshRenderer component = gun.GetComponent<MeshRenderer>();
                    if (!component)
                    {
                        return;
                    }
                    Material[] sharedMaterials = component.sharedMaterials;

                    Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
                    Material material = new Material(mat);
                    material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
                    sharedMaterials[sharedMaterials.Length - 1] = material;
                    component.sharedMaterials = sharedMaterials;
                    */
                    //}

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

        }
    }
}
