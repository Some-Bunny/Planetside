using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using GungeonAPI;
using Alexandria;
using Planetside.Static_Storage;
using Alexandria.Assetbundle;
using static Planetside.Wailer;


namespace Planetside
{
    public static class StaticBulletEntries
    {
        public static AIBulletBank.Entry CopyBulletBankEntry(AIBulletBank.Entry entryToCopy, string Name, string AudioEvent = null, VFXPool muzzleflashVFX = null, bool ChangeMuzzleFlashToEmpty = true)
        {
            AIBulletBank.Entry entry = StaticBulletEntries.CopyFields<AIBulletBank.Entry>(entryToCopy);
            entry.Name = Name;
            entry.preloadCount = Mathf.Max(entry.preloadCount, 25);
            Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(entry.BulletObject).GetComponent<Projectile>();
            projectile.gameObject.SetLayerRecursively(18);
            projectile.transform.position = projectile.transform.position.WithZ(210.5125f);

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);

            

            
            
            entry.BulletObject = projectile.gameObject;
            if (AudioEvent != "DNC") { entry.AudioEvent = AudioEvent; }
            if (ChangeMuzzleFlashToEmpty == true) { entry.MuzzleFlashEffects = muzzleflashVFX == null ? new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] } : muzzleflashVFX; }


            return entry;
        }
        public static AIBulletBank.Entry CopyBulletBankEntryToBlue(AIBulletBank.Entry entryToCopy, string Name, string AudioEvent = null, VFXPool muzzleflashVFX = null, bool ChangeMuzzleFlashToEmpty = true)
        {
            AIBulletBank.Entry entry = StaticBulletEntries.CopyFields<AIBulletBank.Entry>(entryToCopy);
            entry.Name = Name;
            entry.preloadCount = Mathf.Max(entry.preloadCount, 25);

            Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(entry.BulletObject).GetComponent<Projectile>();
            projectile.gameObject.SetLayerRecursively(18);
            projectile.transform.position = projectile.transform.position.WithZ(210.5125f);

            var thirdDimension = OtherTools.CopyFields<ThirdDimensionalProjectile>(projectile);
            thirdDimension.ApplyShader();
            thirdDimension.gameObject.SetActive(false);
            thirdDimension.SetUnDodgeableState(true);
            FakePrefab.MarkAsFakePrefab(thirdDimension.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(thirdDimension);


            entry.BulletObject = thirdDimension.gameObject;
            if (AudioEvent != "DNC") { entry.AudioEvent = AudioEvent; }
            if (ChangeMuzzleFlashToEmpty == true) { entry.MuzzleFlashEffects = muzzleflashVFX == null ? new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] } : muzzleflashVFX; }


            return entry;
        }

        public static AIBulletBank.Entry CopyFields<T>(AIBulletBank.Entry sample2) where T : AIBulletBank.Entry
        {
            AIBulletBank.Entry sample = new AIBulletBank.Entry();
            sample.AudioEvent = sample2.AudioEvent;
            sample.AudioLimitOncePerAttack = sample2.AudioLimitOncePerAttack;
            sample.AudioLimitOncePerFrame = sample2.AudioLimitOncePerFrame;
            sample.AudioSwitch = sample2.AudioSwitch;
            sample.PlayAudio = sample2.PlayAudio;
            sample.BulletObject = sample2.BulletObject;
            sample.conditionalMinDegFromNorth = sample2.conditionalMinDegFromNorth;

            sample.DontRotateShell = sample2.DontRotateShell;
            sample.forceCanHitEnemies = sample2.forceCanHitEnemies;
            sample.MuzzleFlashEffects = sample2.MuzzleFlashEffects;
            sample.MuzzleInheritsTransformDirection = sample2.MuzzleInheritsTransformDirection;
            sample.MuzzleLimitOncePerFrame = sample2.MuzzleLimitOncePerFrame;
            sample.Name = sample2.Name;
            sample.OverrideProjectile = sample2.OverrideProjectile;
            sample.preloadCount = sample2.preloadCount;
            sample.ProjectileData = sample2.ProjectileData;
            sample.rampBullets = sample2.rampBullets;

            sample.rampStartHeight = sample2.rampStartHeight;
            sample.rampTime = sample2.rampTime;
            sample.ShellForce = sample2.ShellForce;
            sample.ShellForceVariance = sample2.ShellForceVariance;
            sample.ShellGroundOffset = sample2.ShellGroundOffset;
            sample.ShellPrefab = sample2.ShellPrefab;
            sample.ShellsLimitOncePerFrame = sample2.ShellsLimitOncePerFrame;
            sample.ShellTransform = sample2.ShellTransform;
            sample.SpawnShells = sample2.SpawnShells;
            sample.suppressHitEffectsIfOffscreen = sample2.suppressHitEffectsIfOffscreen;

            return sample;
        }

        public static void ApplyShader(this ThirdDimensionalProjectile thirdDimensionalProjectile)
        {
            DepthLookupManager.AssignRendererToSortingLayer(thirdDimensionalProjectile.sprite.renderer, DepthLookupManager.GungeonSortingLayer.FOREGROUND);
            DepthLookupManager.UpdateRenderer(thirdDimensionalProjectile.sprite.renderer);
            DepthLookupManager.UpdateRendererWithWorldYPosition(thirdDimensionalProjectile.sprite.renderer, 12);

            thirdDimensionalProjectile.sprite.usesOverrideMaterial = true;


            Material material = new Material(PlanetsideModule.InverseGlowShader);
            //material.mainTexture = thirdDimensionalProjectile.sprite.renderer.material.mainTexture;
            thirdDimensionalProjectile.sprite.renderer.material = material;
            thirdDimensionalProjectile.sprite.renderer.material.SetFloat("_EmissiveColorPower", 6);
            thirdDimensionalProjectile.sprite.renderer.material.SetFloat("_EmissivePower", 8);
            thirdDimensionalProjectile.sprite.renderer.material.SetFloat("_IsBlue", 1);

            //if (updateComp.IsBlackBullet) { updateComp.sprite.renderer.material.SetFloat("_BlackBullet", -1); }
        }

        public static void Init()
        {
            {
                /*
                undodgeableSniper = StaticBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
                undodgeableSniper.Name = "sniperUndodgeable";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableSniper.BulletObject).GetComponent<Projectile>();
                var thirdDimension = OtherTools.CopyFields<ThirdDimensionalProjectile>(projectile);
                thirdDimension.ApplyShader();
                thirdDimension.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(thirdDimension.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(thirdDimension);
                undodgeableSniper.BulletObject = thirdDimension.gameObject;
                undodgeableSniper.OverrideProjectile = thirdDimension;
                */
                undodgeableSniper = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperUndodgeable", "DNC", null, false);


            }


            {
                /*
                undodgeableBig = StaticBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
                undodgeableBig.Name = "undodgeableBig";
                Projectile projectileOne = UnityEngine.Object.Instantiate<GameObject>(undodgeableBig.BulletObject).GetComponent<Projectile>();
                var thirdDimension = OtherTools.CopyFields<ThirdDimensionalProjectile>(projectileOne);
                thirdDimension.ApplyShader();
                thirdDimension.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(thirdDimension.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(thirdDimension);
                undodgeableBig.BulletObject = thirdDimension.gameObject;
                undodgeableBig.OverrideProjectile = thirdDimension;
                */

                undodgeableBig = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"), "undodgeableBig", "DNC", null, false);

            }



            {   /*
                undodgeableSmallSpore = StaticBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
                undodgeableSmallSpore.Name = "undodgeableSpore";
                Projectile projectileSpore = UnityEngine.Object.Instantiate<GameObject>(undodgeableSmallSpore.BulletObject).GetComponent<Projectile>();
                var thirdDimension = OtherTools.CopyFields<ThirdDimensionalProjectile>(projectileSpore);
                thirdDimension.ApplyShader();
                thirdDimension.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(thirdDimension.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(thirdDimension);
                undodgeableSmallSpore.BulletObject = thirdDimension.gameObject;
                undodgeableSmallSpore.OverrideProjectile = thirdDimension;
                */
                undodgeableSmallSpore = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"), "undodgeableSpore", "DNC", null, false);

                /*
                projectileSpore.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectileSpore.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectileSpore.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectileSpore);
                undodgeableSmallSporeVar.BulletObject = projectileSpore.gameObject;
                undodgeableSmallSpore = undodgeableSmallSporeVar;
                */
            }

            {
                /*
                undodgeableLargeSpore = StaticBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
                undodgeableLargeSpore.Name = "undodgeableSporeLarge";
                Projectile projectileSpore = UnityEngine.Object.Instantiate<GameObject>(undodgeableLargeSpore.BulletObject).GetComponent<Projectile>();
                var thirdDimension = OtherTools.CopyFields<ThirdDimensionalProjectile>(projectileSpore);
                thirdDimension.ApplyShader();
                thirdDimension.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(thirdDimension.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(thirdDimension);
                undodgeableLargeSpore.BulletObject = thirdDimension.gameObject;
                undodgeableLargeSpore.OverrideProjectile = thirdDimension;
                */
                undodgeableLargeSpore = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"), "undodgeableSporeLarge", "DNC", null, false);

                /*
                projectileSpore.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectileSpore.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectileSpore.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectileSpore);
                undodgeableLargeSpore.BulletObject = projectileSpore.gameObject;
                */
            }

            {
                /*
                undodgeableDefault = StaticBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet("default"));
                undodgeableDefault.Name = "undodgeableDefault";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableDefault.BulletObject).GetComponent<Projectile>();

                var thirdDimension = OtherTools.CopyFields<ThirdDimensionalProjectile>(projectile);
                thirdDimension.ApplyShader();
                thirdDimension.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(thirdDimension.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(thirdDimension);
                undodgeableDefault.BulletObject = thirdDimension.gameObject;
                undodgeableDefault.OverrideProjectile = thirdDimension;
                */
                undodgeableDefault = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet("default"), "undodgeableDefault", "DNC", null, false);

                /*
                projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                undodgeableDefaultVar.BulletObject = projectile.gameObject;
                undodgeableDefault = undodgeableDefaultVar;
                */
            }
            {
                /*
                undodgeableQuickHoming = StaticBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("quickHoming"));
                undodgeableQuickHoming.Name = "UndodgeablequickHoming";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableQuickHoming.BulletObject).GetComponent<Projectile>();

                var thirdDimension = OtherTools.CopyFields<ThirdDimensionalProjectile>(projectile);
                thirdDimension.ApplyShader();
                thirdDimension.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(thirdDimension.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(thirdDimension);
                undodgeableQuickHoming.BulletObject = thirdDimension.gameObject;
                undodgeableQuickHoming.OverrideProjectile = thirdDimension;
                undodgeableQuickHoming.AudioEvent = null;
                */
                undodgeableQuickHoming = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("quickHoming"), "UndodgeablequickHoming", null, null, true);

                /*
                projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                undodgeableQuickHomingVar.BulletObject = projectile.gameObject;
                undodgeableQuickHoming = undodgeableQuickHomingVar;
                undodgeableQuickHoming.AudioEvent = null;
                */
            }
            {
                /*
                undodgeableIcicle = StaticBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("icicle"));
                undodgeableIcicle.Name = "UndodgeableIcicle";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableIcicle.BulletObject).GetComponent<Projectile>();

                var thirdDimension = OtherTools.CopyFields<ThirdDimensionalProjectile>(projectile);
                thirdDimension.ApplyShader();
                thirdDimension.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(thirdDimension.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(thirdDimension);
                undodgeableIcicle.BulletObject = thirdDimension.gameObject;
                undodgeableIcicle.OverrideProjectile = thirdDimension;
                undodgeableIcicle.AudioEvent = null;
                */
                undodgeableIcicle = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("icicle"), "UndodgeableIcicle", "DNC", null, false);

                /*
                projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                undodgeableIcicleVar.BulletObject = projectile.gameObject;
                undodgeableIcicle = undodgeableIcicleVar;
                undodgeableIcicle.AudioEvent = null;
                */
            }
            {
                /*
                undodgeableChainLink = StaticBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("463d16121f884984abe759de38418e48").bulletBank.GetBullet("link"));
                undodgeableChainLink.Name = "UndodgeableLink";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableChainLink.BulletObject).GetComponent<Projectile>();

                var thirdDimension = OtherTools.CopyFields<ThirdDimensionalProjectile>(projectile);
                thirdDimension.ApplyShader();
                thirdDimension.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(thirdDimension.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(thirdDimension);
                undodgeableChainLink.BulletObject = thirdDimension.gameObject;
                undodgeableChainLink.OverrideProjectile = thirdDimension;
                undodgeableChainLink.AudioEvent = null;
                */
                undodgeableChainLink = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("463d16121f884984abe759de38418e48").bulletBank.GetBullet("link"), "UndodgeableLink", "DNC", null, false);

                /*
                projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                undodgeableChainLinkVar.BulletObject = projectile.gameObject;
                undodgeableChainLink = undodgeableChainLinkVar;
                undodgeableChainLink.AudioEvent = null;
                */
            }
            {

                undodgeableSkull = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"), "UndodgeableSkull", "DNC", null, false);


                /*
                undodgeableSkull = StaticBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"));
                undodgeableSkull.Name = "UndodgeableSkull";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableSkull.BulletObject).GetComponent<Projectile>();

                CustomTrailRenderer projectileTrailRendererController = projectile.gameObject.GetComponentInChildren<CustomTrailRenderer>();
                projectileTrailRendererController.colors = new Color[] { Color.cyan };
                /*
                foreach (Component comp in projectile.gameObject.GetComponents(typeof(Component)))
                {
                    ETGModConsole.Log(comp.GetType().ToString() ?? "null");
                }
                */
                //ProjectileTrailRendererController projectileTrailRendererController = projectile.GetComponent<ProjectileTrailRendererController>();
                //projectileTrailRendererController.customTrailRenderer.colors = new Color[] { Color.cyan };

                /*
                var thirdDimension = OtherTools.CopyFields<ThirdDimensionalProjectile>(projectile);
                thirdDimension.ApplyShader();
                thirdDimension.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(thirdDimension.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(thirdDimension);
                undodgeableSkull.BulletObject = thirdDimension.gameObject;
                undodgeableSkull.OverrideProjectile = thirdDimension;
                undodgeableSkull.AudioEvent = null;
                */



                undodgeableSkull.AudioEvent = null;
                undodgeableSkull.MuzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
                
            }

            {
                undodgeableSkullAudio = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"), "UndodgeableSkullUnmodded", "DNC", null, false);

                /*
                undodgeableSkullAudio = StaticBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"));
                undodgeableSkullAudio.Name = "UndodgeableSkullUnmodded";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableSkullAudio.BulletObject).GetComponent<Projectile>();
                var thirdDimension = OtherTools.CopyFields<ThirdDimensionalProjectile>(projectile);
                thirdDimension.ApplyShader();
                thirdDimension.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(thirdDimension.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(thirdDimension);
                undodgeableSkullAudio.BulletObject = thirdDimension.gameObject;
                undodgeableSkullAudio.OverrideProjectile = thirdDimension;
                */
                /*
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableSkullvar.BulletObject).GetComponent<Projectile>();
                projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                undodgeableSkullvar.BulletObject = projectile.gameObject;
                undodgeableSkullAudio = undodgeableSkullvar;
                */
            }

            //undodgeableBouncyBatBullet

            //

            //2feb50a6a40f4f50982e89fd276f6f15
            // EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank
            /*
            for (int i = 0; i < EnemyDatabase.GetOrLoadByGuid("1a4872dafdb34fd29fe8ac90bd2cea67").bulletBank.Bullets.Count; i++)
            {
                ETGModConsole.Log(EnemyDatabase.GetOrLoadByGuid("1a4872dafdb34fd29fe8ac90bd2cea67").bulletBank.Bullets[i].Name);
            }
            */

            {
                undodgeableBouncyBatBullet = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("1a4872dafdb34fd29fe8ac90bd2cea67").bulletBank.Bullets[0], "undodgeableDefaultBouncy");
                //entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                ProjectileTrailRendererController projectileTrailRendererController = undodgeableBouncyBatBullet.BulletObject.GetComponent<ProjectileTrailRendererController>();
                projectileTrailRendererController.customTrailRenderer.colors = new Color[] { Color.cyan };
            }


            {
                NemesisGuon = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"), "NemesisGuon");
                //entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();


                CustomTrailRenderer projectileTrailRendererController = NemesisGuon.BulletObject.GetComponentInChildren<CustomTrailRenderer>();
                projectileTrailRendererController.colors = new Color[] { Color.cyan };
                

            }

            {
                undodgeableBigBullet = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"), "undodgeableBigBullet");




                //entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                //undodgeableBigBullet = entry;
            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet("default"), "undodgeableHitscan");

                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                Projectile proj = entry.BulletObject.GetComponent<Projectile>();


                //EmmisiveTrail emis = proj.gameObject.AddComponent<EmmisiveTrail>();
                /*
                emis.EmissiveColorPower = 10;
                emis.EmissivePower = 100;
                */
                proj.shouldRotate = true;

                /*
                List<string> BeamAnimPaths = new List<string>()
                {
                "Planetside/Resources2/ProjectileTrails/Undodgeable/hitscan_mid_001",
                "Planetside/Resources2/ProjectileTrails/Undodgeable/hitscan_mid_002",
                "Planetside/Resources2/ProjectileTrails/Undodgeable/hitscan_mid_003",
                "Planetside/Resources2/ProjectileTrails/Undodgeable/hitscan_mid_004",
                "Planetside/Resources2/ProjectileTrails/Undodgeable/hitscan_mid_005",


                };
                List<string> ImpactAnimPaths = new List<string>()
                {
                "Planetside/Resources2/ProjectileTrails/Undodgeable/hitscan_front_001",
                "Planetside/Resources2/ProjectileTrails/Undodgeable/hitscan_front_002",
                "Planetside/Resources2/ProjectileTrails/Undodgeable/hitscan_front_003",
                "Planetside/Resources2/ProjectileTrails/Undodgeable/hitscan_front_004",
                "Planetside/Resources2/ProjectileTrails/Undodgeable/hitscan_front_005",

                };

                proj.AddTrailToProjectile(
                "Planetside/Resources2/ProjectileTrails/Undodgeable/hitscan_mid_001",
                new Vector2(12, 12),
                new Vector2(0, 0),
                BeamAnimPaths, 20,
                ImpactAnimPaths, 20,
                0.1f,
                30,
                30,
                false
                );
                */

                proj.AddTrailToProjectileBundle(StaticSpriteDefinitions.Beam_Sheet_Data, "hitscan_mid_001",
                StaticSpriteDefinitions.Beam_Animation_Data,
                "undodgeablehitscan_mid", new Vector2(12, 10), new Vector2(0, 1), false, "undodgeablehitscan_start");

                Material glowshader = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));

                proj.sprite.usesOverrideMaterial = true;
                glowshader.mainTexture = proj.sprite.renderer.material.mainTexture;
                glowshader.SetFloat("_EmissivePower", 20);
                glowshader.SetFloat("_EmissiveColorPower", 20);
                proj.sprite.renderer.material = glowshader;

               UndodgeableHitscan = entry;
            }


            {
                UnwillingShot = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"), "undodgeableUnwillingShot");


                //entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                //UnwillingShot = entry;
            }

            {
                undodgeableBatBullet = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("2feb50a6a40f4f50982e89fd276f6f15").bulletBank.Bullets[1], "undodgeableBatBullet");
   
            }

            {
                undodgeableDonut = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("fa76c8cfdf1c4a88b55173666b4bc7fb").bulletBank.GetBullet("hugeBullet"), "undodgeableMine");
                
            }

            {
                undodgeableDonut = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b").bulletBank.GetBullet("donut"), "undodgeableDonut", "DNC");
                
            }

            {
                undodgeableScream = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("c367f00240a64d5d9f3c26484dc35833").bulletBank.GetBullet("scream"), "undodgeableScream", "DNC");
                
            }

            {
                UndodgeableRandomBullet = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("randomBullet"), "UndodgeableRandomBullet", "DNC");
                
            }
            {
                UndodgeableAmmocondaBigBullet = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("bigBullet"), "UndodgeableAmmocondaBigBullet", "DNC");
                
            }
            {
                UndodgeableSpew = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("1b5810fafbec445d89921a4efb4e42b7").bulletBank.GetBullet("spew"), "UndodgeableSpew", "DNC");
                
            }
            {
                UndodgeableSlam = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("1b5810fafbec445d89921a4efb4e42b7").bulletBank.GetBullet("slam"), "UndodgeableSlam", "DNC");
               
            }
            {
                UndodgeableGroundDefault = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("5e0af7f7d9de4755a68d2fd3bbc15df4").bulletBank.GetBullet("default_ground"), "UndodgeableGroundDefault", "DNC");
                
            }
            {
                UndodgeableCannonBullet = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("5e0af7f7d9de4755a68d2fd3bbc15df4").bulletBank.GetBullet("cannon"), "UndodgeableCannonBullet", "DNC", null, false);
                
            }
            {
                UndodgeableScatterBullet = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("fa76c8cfdf1c4a88b55173666b4bc7fb").bulletBank.GetBullet("scatterBullet"), "UndodgeableScatterBullet", "DNC", null, false);
            }

            {
                undodgeableMineflayerBounce = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("8b0dd96e2fe74ec7bebc1bc689c0008a").bulletBank.GetBullet("bounce"), "undodgeableMineflayerBounce");
                undodgeableMineflayerBounce.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                CustomTrailRenderer projectileTrailRendererController = undodgeableMineflayerBounce.BulletObject.GetComponentInChildren<CustomTrailRenderer>();
                projectileTrailRendererController.colors = new Color[] { Color.cyan };

            }

            {
                UndodgeableOldKingSlamBullet = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("slam"), "UndodgeableOldKingSlamBullet", "DNC", null, false);

            }
            {
                UndodgeableOldKingSuckBullet = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("suck"), "UndodgeableOldKingSuckBullet", "DNC", null, false);
                
            }
            {
                UndodgeableOldKingHomingRingBullet = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("homingRing"), "UndodgeableOldKingHomingRingBullet", "DNC", null, false);
                
            }
            {
                UndodgeableOldKingHomingRingBulletSoundless = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("homingRing"), "UndodgeableOldKingHomingRingBullet", null, null, false);
       
            }


            {
                UndodgeableFrogger = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"), "UndodgeableFrogger", "DNC", null, false);
               
            }

            {
                UndodgeableWallWave = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("f3b04a067a65492f8b279130323b41f0").bulletBank.GetBullet("wave"), "UndodgeableWallWave", "DNC", null, false);
                
            }

            {
                UndodgeableSweep = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"), "UndodgeableSweep", "DNC", null, false);
                
            }
            {
                UndodgeableMergoWave = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("mergoWave"), "UndodgeableMergoWave", "DNC", null, false);
                
            }
            {
                UndodgeableCross = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("cross"), "UndodgeableCross", "DNC", null, false);
            }
            {
                undodgeableBulletKingSlam = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("slam"), "UndodgeableSlammo", "DNC", null, false);
            }
            {
                UndodgeableDirectedfire = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"), "UndodgeableDirectedfire", "DNC", null, false);
                UndodgeableDirectedfire.BulletObject.GetComponent<Projectile>().shouldRotate = true;
            }
            {
                UndodgeableDirectedfireSoundless = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"), "UndodgeableDirectedfireNoAudio", null, null, false);
                UndodgeableDirectedfireSoundless.BulletObject.GetComponent<Projectile>().shouldRotate = true;
            }

            //UndodgeableDirectedfireSoundless
            {
                UndodgeableAngrybullet = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("c00390483f394a849c36143eb878998f").bulletBank.GetBullet("angrybullet"), "UndodgeableAngrybullet", "DNC", null, false);
                
            }
            {
                UndodgeableDoorLordBurst = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("9189f46c47564ed588b9108965f975c9").bulletBank.GetBullet("burst"), "UndodgeableDoorLordBurst", "DNC", null, false);
                
            }
            {
                UndodgeableDoorLordFlame = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("9189f46c47564ed588b9108965f975c9").bulletBank.GetBullet("flame"), "UndodgeableDoorLordFlame", "DNC", null, false);
                
            }
            {
                UndodgeableDoorLordPuke = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("9189f46c47564ed588b9108965f975c9").bulletBank.GetBullet("puke_burst"), "UndodgeableDoorLordPuke", "DNC", null, false);
                
            }
            {
                undodgeableGrenade = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid("880bbe4ce1014740ba6b4e2ea521e49d").bulletBank.GetBullet("grenade"), "UndodgeableGrenade", "DNC", null, false);
                
            }
            {
                UnDodgeableCheese = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("cheese"), "UndodgeableCheese", "DNC", null, false);
                
            }
            {
                UnDodgeableDagger = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("dagger"), "UnDodgeableDagger", "DNC", null, false);
                
            }
            {
                UnDodgeableTailProj = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("tail"), "UnDodgeableTailProj", "DNC", null, false);
                
            }
            {
                UnDodgeablecheeseWedge0 = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("cheeseWedge0"), "UnDodgeablecheeseWedge0", "DNC", null, false);
                
            }
            {
                UnDodgeablecheeseWedge1 = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("cheeseWedge1"), "UnDodgeablecheeseWedge1", "DNC", null, false);
                
            }
            {
                UnDodgeablecheeseWedge2 = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("cheeseWedge2"), "UnDodgeablecheeseWedge2", "DNC", null, false);
                
            }
            {
                UnDodgeablecheeseWedge3 = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("cheeseWedge3"), "UnDodgeablecheeseWedge3", "DNC", null, false);
            }
            {
                UnDodgeablecheeseWedge4 = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("cheeseWedge4"), "UnDodgeablecheeseWedge4", "DNC", null, false);

            }
            {
                UnDodgeablecheeseWedge5 = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("cheeseWedge5"), "UnDodgeablecheeseWedge5", "DNC", null, false);
            }
            {
                UnDodgeablecheeseWedge6 = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("cheeseWedge6"), "UnDodgeablecheeseWedge6", "DNC", null, false);
            }
            {
                UnDodgeablecheeseWedge7 = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("cheeseWedge7"), "UnDodgeablecheeseWedge7", "DNC", null, false);
            }
            {
                UnDodgeableCheeseWheel = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Boss_GUID).bulletBank.GetBullet("cheeseWheel"), "UnDodgeableCheeseWheel", "DNC", null, false);
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Mech_Boss_GUID).bulletBank.GetBullet("spinner"), "UnDodgeableSpinner", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UnDodgeableSpinner = entry;
            }
            {
                UnDodgeableBigOne = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Resourceful_Rat_Mech_Boss_GUID).bulletBank.GetBullet("big_one"), "UnDodgeableBigOne", "DNC", null, false);
            }
            {
                UnDodgeableMolotov = CopyBulletBankEntryToBlue(EnemyDatabase.GetOrLoadByGuid(Alexandria.EnemyGUIDs.Black_Stache_GUID).bulletBank.GetBullet("molotov"), "UnDodgeableMolotov", "DNC", null, false);
            }
        }


        //cheeseWedge0//cheeseWheel
        public static AIBulletBank.Entry UnDodgeableMolotov;
        public static AIBulletBank.Entry UnDodgeableBigOne;
        public static AIBulletBank.Entry UnDodgeableSpinner;
        public static AIBulletBank.Entry UnDodgeableCheeseWheel;
        public static AIBulletBank.Entry UnDodgeablecheeseWedge0;
        public static AIBulletBank.Entry UnDodgeablecheeseWedge1;
        public static AIBulletBank.Entry UnDodgeablecheeseWedge2;
        public static AIBulletBank.Entry UnDodgeablecheeseWedge3;
        public static AIBulletBank.Entry UnDodgeablecheeseWedge4;
        public static AIBulletBank.Entry UnDodgeablecheeseWedge5;
        public static AIBulletBank.Entry UnDodgeablecheeseWedge6;
        public static AIBulletBank.Entry UnDodgeablecheeseWedge7;
        public static AIBulletBank.Entry UnDodgeableDagger;
        public static AIBulletBank.Entry UnDodgeableTailProj;

        public static AIBulletBank.Entry UnDodgeableCheese;


        public static AIBulletBank.Entry UndodgeableHitscan;


        public static AIBulletBank.Entry UndodgeableDoorLordPuke;
        public static AIBulletBank.Entry UndodgeableDoorLordFlame;
        public static AIBulletBank.Entry UndodgeableDoorLordBurst;

        public static AIBulletBank.Entry UndodgeableAngrybullet;


        public static AIBulletBank.Entry undodgeableBulletKingSlam;
        public static AIBulletBank.Entry UndodgeableDirectedfire;
        public static AIBulletBank.Entry UndodgeableDirectedfireSoundless;


        public static AIBulletBank.Entry undodgeableSkullAudio;

        public static AIBulletBank.Entry UndodgeableCross;

        public static AIBulletBank.Entry UndodgeableSweep;
        public static AIBulletBank.Entry UndodgeableMergoWave;


        public static AIBulletBank.Entry UndodgeableWallWave;

        public static AIBulletBank.Entry UndodgeableOldKingHomingRingBulletSoundless;


        public static AIBulletBank.Entry UndodgeableOldKingHomingRingBullet;
        public static AIBulletBank.Entry UndodgeableFrogger;


        public static AIBulletBank.Entry UndodgeableOldKingSlamBullet;
        public static AIBulletBank.Entry UndodgeableOldKingSuckBullet;


        public static AIBulletBank.Entry undodgeableMineflayerBounce;

        public static AIBulletBank.Entry UndodgeableScatterBullet;


        public static AIBulletBank.Entry UndodgeableGroundDefault;
        public static AIBulletBank.Entry UndodgeableCannonBullet;


        public static AIBulletBank.Entry UndodgeableSpew;
        public static AIBulletBank.Entry UndodgeableSlam;


        public static AIBulletBank.Entry UndodgeableRandomBullet;
        public static AIBulletBank.Entry UndodgeableAmmocondaBigBullet;

        public static AIBulletBank.Entry undodgeableDonut;
        public static AIBulletBank.Entry undodgeableScream;


        public static AIBulletBank.Entry undodgeableBigBullet;
        public static AIBulletBank.Entry UnwillingShot;



        public static AIBulletBank.Entry undodgeableSniper;
        public static AIBulletBank.Entry undodgeableBig;
        public static AIBulletBank.Entry undodgeableSmallSpore;
        public static AIBulletBank.Entry undodgeableLargeSpore;

        public static AIBulletBank.Entry undodgeableDefault;
        public static AIBulletBank.Entry undodgeableQuickHoming;
        public static AIBulletBank.Entry undodgeableIcicle;
        public static AIBulletBank.Entry undodgeableChainLink;
        public static AIBulletBank.Entry undodgeableSkull;
        public static AIBulletBank.Entry undodgeableBatBullet;
        public static AIBulletBank.Entry undodgeableMine;

        public static AIBulletBank.Entry undodgeableGrenade;

        public static AIBulletBank.Entry undodgeableBouncyBatBullet;
        public static AIBulletBank.Entry NemesisGuon;
    }
}
