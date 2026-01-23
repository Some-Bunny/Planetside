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


            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "colossus_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.reloadAnimation = "colossus_reload";
            gun.idleAnimation = "colossus_idle";
            gun.shootAnimation = "colossus_fire";


            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> {
                { 6, "Play_ENM_statue_stomp_01" }, 
            });

            //                AkSoundEngine.PostEvent("Play_ENM_statue_stomp_01", player.gameObject);

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

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

                BasicBeamController beamComp = projectile.GenerateBeamPrefabBundle(
                "colossusbeam_mid_001",
                StaticSpriteDefinitions.Beam_Sheet_Data,
                StaticSpriteDefinitions.Beam_Animation_Data,
                "colossus_mid", new Vector2(12, 8), new Vector2(0, 2), //Main
                "colossus_impact", new Vector2(12, 8), new Vector2(0, 2), //Impact
                "colossus_mid", new Vector2(12, 8), new Vector2(0, 2), //End Of beam
                "colossus_start", new Vector2(12, 8), new Vector2(0, 2), //Start Of Beam
                true);



                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 55f;
                projectile.baseData.force *= 1f;
                projectile.baseData.range *= 5;
                projectile.baseData.speed *= 5f;
                projectile.specRigidbody.CollideWithOthers = false;

                EmmisiveBeams emiss = beamComp.gameObject.AddComponent<EmmisiveBeams>();
                emiss.EmissiveColorPower = 10f;
                emiss.EmissivePower = 100;

                beamComp.boneType = BasicBeamController.BeamBoneType.Projectile;

                beamComp.startAudioEvent = "Play_ENM_deathray_shot_01";
                beamComp.projectile.baseData.damage = 55;
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


                int Length = 4;
                Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(perfProjectile, "PerfectedProjectile", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "PerfectedProjectile",
                 AnimateBullet.ConstructListOfSameValues<IntVector2>(new IntVector2(11, 11), Length),
                AnimateBullet.ConstructListOfSameValues(true, Length),
                AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, Length),
                AnimateBullet.ConstructListOfSameValues(true, Length),
                AnimateBullet.ConstructListOfSameValues(false, Length),
                AnimateBullet.ConstructListOfSameValues<Vector3?>(null, Length),
                AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(13, 13), Length),
                AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(-1, -1), Length),
                AnimateBullet.ConstructListOfSameValues<Projectile>(null, Length));

                /*
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
                
                */
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

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(121) as Gun).gunSwitchGroup;

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
                "heart_bottle",
                "psog:dead_kings_desparation"
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
            if (gun.IsReloading)
            {
                gun.CeaseAttack(false, null);
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                if (player.PlayerHasActiveSynergy("Perfected") && gun.ClipShotsRemaining <= (gun.ClipCapacity * 0.5f))
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
                    }
                }
            }
        }
    }
}
