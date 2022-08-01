using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using GungeonAPI;

namespace Planetside
{
    public class StaticInformation 
    {
        public static void Init()
        {
            StaticUndodgeableBulletEntries.Init();
            StaticEnemyShadows.Init();
            StaticTextures.Init();
            ModderBulletGUIDs = new List<string>();
        }
        public static List<string> ModderBulletGUIDs;
    }
    public class StaticEnemyShadows
    {
        public static void Init()
        {
            massiveShadow = (EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").ShadowPrefab);
            if (massiveShadow == null) { ETGModConsole.Log("massiveShadow IS NULL"); }

            largeShadow = (EnemyDatabase.GetOrLoadByGuid("eed5addcc15148179f300cc0d9ee7f94").ShadowPrefab);
            if (largeShadow == null) { ETGModConsole.Log("largeShadow IS NULL"); }

            highPriestShadow = (EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowPrefab);
            if (highPriestShadow == null) { ETGModConsole.Log("highPriestShadow IS NULL"); }

            defaultShadow = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("DefaultShadowSprite"));
            if (defaultShadow == null) { ETGModConsole.Log("defaultShadow IS NULL"); }
        }
        public static GameObject highPriestShadow;
        public static GameObject massiveShadow;
        public static GameObject largeShadow;
        public static GameObject defaultShadow;

    }
    public class StaticTextures
    {
        public static void Init()
        {
            NebulaTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\nebula_reducednoise.png");
            VoidTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\voidTex.png");
            AdvancedParticleBlue = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources2\\ParticleTextures\\advancedparticles.png");

        }
        public static Texture AdvancedParticleBlue;
        public static Texture VoidTexture;
        public static Texture NebulaTexture;
    }

    public class StaticUndodgeableBulletEntries
    {

        private static AIBulletBank.Entry CopyBulletBankEntry(AIBulletBank.Entry entryToCopy, string Name, string AudioEvent = null, VFXPool muzzleflashVFX = null, bool ChangeMuzzleFlashToEmpty = true)
        {
            AIBulletBank.Entry entry = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(entryToCopy);
            entry.Name = Name;
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
        public static void Init()
        {
            {
                undodgeableSniper = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
                undodgeableSniper.Name = "sniperUndodgeable";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableSniper.BulletObject).GetComponent<Projectile>();
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                undodgeableSniper.BulletObject = projectile.gameObject;
            }


            {

                AIBulletBank.Entry undodgeableBigVar = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
                undodgeableBigVar.Name = "undodgeableBig";
                Projectile projectileOne = UnityEngine.Object.Instantiate<GameObject>(undodgeableBigVar.BulletObject).GetComponent<Projectile>();
                projectileOne.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectileOne.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectileOne.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectileOne);
                undodgeableBigVar.BulletObject = projectileOne.gameObject;
                undodgeableBig = undodgeableBigVar;
            }



            {
                AIBulletBank.Entry undodgeableSmallSporeVar = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
                undodgeableSmallSporeVar.Name = "undodgeableSpore";
                Projectile projectileSpore = UnityEngine.Object.Instantiate<GameObject>(undodgeableSmallSporeVar.BulletObject).GetComponent<Projectile>();
                projectileSpore.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectileSpore.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectileSpore.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectileSpore);
                undodgeableSmallSporeVar.BulletObject = projectileSpore.gameObject;
                undodgeableSmallSpore = undodgeableSmallSporeVar;
            }

            {
                AIBulletBank.Entry undodgeableSmallSporeVar = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
                undodgeableSmallSporeVar.Name = "undodgeableSporeLarge";
                Projectile projectileSpore = UnityEngine.Object.Instantiate<GameObject>(undodgeableSmallSporeVar.BulletObject).GetComponent<Projectile>();
                projectileSpore.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectileSpore.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectileSpore.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectileSpore);
                undodgeableSmallSporeVar.BulletObject = projectileSpore.gameObject;
                undodgeableLargeSpore = undodgeableSmallSporeVar;
            }

            {
                AIBulletBank.Entry undodgeableDefaultVar = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet("default"));
                undodgeableDefaultVar.Name = "undodgeableDefault";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableDefaultVar.BulletObject).GetComponent<Projectile>();
                projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                undodgeableDefaultVar.BulletObject = projectile.gameObject;
                undodgeableDefault = undodgeableDefaultVar;
            }
            {
                AIBulletBank.Entry undodgeableQuickHomingVar = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("quickHoming"));
                undodgeableQuickHomingVar.Name = "UndodgeablequickHoming";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableQuickHomingVar.BulletObject).GetComponent<Projectile>();
                projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                undodgeableQuickHomingVar.BulletObject = projectile.gameObject;
                undodgeableQuickHoming = undodgeableQuickHomingVar;
                undodgeableQuickHoming.AudioEvent = null;
            }
            {
                AIBulletBank.Entry undodgeableIcicleVar = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("icicle"));
                undodgeableIcicleVar.Name = "UndodgeableIcicle";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableIcicleVar.BulletObject).GetComponent<Projectile>();
                projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                undodgeableIcicleVar.BulletObject = projectile.gameObject;
                undodgeableIcicle = undodgeableIcicleVar;
                undodgeableIcicle.AudioEvent = null;
            }
            {
                AIBulletBank.Entry undodgeableChainLinkVar = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("463d16121f884984abe759de38418e48").bulletBank.GetBullet("link"));
                undodgeableChainLinkVar.Name = "UndodgeableLink";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableChainLinkVar.BulletObject).GetComponent<Projectile>();
                projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                undodgeableChainLinkVar.BulletObject = projectile.gameObject;
                undodgeableChainLink = undodgeableChainLinkVar;
                undodgeableChainLink.AudioEvent = null;
            }
            {
                AIBulletBank.Entry undodgeableSkullvar = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"));
                undodgeableSkullvar.Name = "UndodgeableSkull";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableSkullvar.BulletObject).GetComponent<Projectile>();
                projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();

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

                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                undodgeableSkullvar.BulletObject = projectile.gameObject;



                undodgeableSkull = undodgeableSkullvar;
                undodgeableSkull.AudioEvent = null;
                undodgeableSkull.MuzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            }

            {
                AIBulletBank.Entry undodgeableSkullvar = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"));
                undodgeableSkullvar.Name = "UndodgeableSkullUnmodded";
                Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableSkullvar.BulletObject).GetComponent<Projectile>();
                projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                undodgeableSkullvar.BulletObject = projectile.gameObject;
                undodgeableSkullAudio = undodgeableSkullvar;
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
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("1a4872dafdb34fd29fe8ac90bd2cea67").bulletBank.Bullets[0], "undodgeableDefaultBouncy");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                ProjectileTrailRendererController projectileTrailRendererController = entry.BulletObject.GetComponent<ProjectileTrailRendererController>();
                //projectileTrailRendererController.trailRenderer.startColor = Color.cyan;
                //projectileTrailRendererController.trailRenderer.endColor = Color.white;
                projectileTrailRendererController.customTrailRenderer.colors = new Color[] { Color.cyan };
                undodgeableBouncyBatBullet = entry;
                /*
                foreach (Component comp in entry.BulletObject.GetComponents(typeof(Component)))
                {
                    ETGModConsole.Log(comp.GetType().ToString() ?? "null");
                }
                */
            }


            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"), "NemesisGuon");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
          
                CustomTrailRenderer projectileTrailRendererController = entry.BulletObject.GetComponentInChildren<CustomTrailRenderer>();
                projectileTrailRendererController.colors = new Color[] { Color.cyan };
                NemesisGuon = entry;
             
            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"), "undodgeableBigBullet");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                undodgeableBigBullet = entry;
            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet("default"), "undodgeableHitscan");

                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                Projectile proj = entry.BulletObject.GetComponent<Projectile>();



                EmmisiveTrail emis = proj.gameObject.AddComponent<EmmisiveTrail>();
                emis.EmissiveColorPower = 10;
                emis.EmissivePower = 100;
                proj.shouldRotate = true;

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
                UndodgeableHitscan = entry;
            }


            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"), "undodgeableUnwillingShot");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UnwillingShot = entry;
            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("2feb50a6a40f4f50982e89fd276f6f15").bulletBank.Bullets[1], "undodgeableBatBullet");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                undodgeableBatBullet = entry;
            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("fa76c8cfdf1c4a88b55173666b4bc7fb").bulletBank.GetBullet("hugeBullet"), "undodgeableMine");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                undodgeableMine = entry;
            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b").bulletBank.GetBullet("donut"), "undodgeableDonut", "DNC");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                undodgeableDonut = entry;
            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("c367f00240a64d5d9f3c26484dc35833").bulletBank.GetBullet("scream"), "undodgeableScream", "DNC");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                undodgeableScream = entry;
            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("randomBullet"), "UndodgeableRandomBullet", "DNC");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableRandomBullet = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("bigBullet"), "UndodgeableAmmocondaBigBullet", "DNC");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableAmmocondaBigBullet = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("1b5810fafbec445d89921a4efb4e42b7").bulletBank.GetBullet("spew"), "UndodgeableSpew", "DNC");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableSpew = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("1b5810fafbec445d89921a4efb4e42b7").bulletBank.GetBullet("slam"), "UndodgeableSlam", "DNC");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableSlam = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("5e0af7f7d9de4755a68d2fd3bbc15df4").bulletBank.GetBullet("default_ground"), "UndodgeableGroundDefault", "DNC");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableGroundDefault = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("5e0af7f7d9de4755a68d2fd3bbc15df4").bulletBank.GetBullet("cannon"), "UndodgeableCannonBullet", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableCannonBullet = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("fa76c8cfdf1c4a88b55173666b4bc7fb").bulletBank.GetBullet("scatterBullet"), "UndodgeableScatterBullet", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableScatterBullet = entry;
            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("8b0dd96e2fe74ec7bebc1bc689c0008a").bulletBank.GetBullet("bounce"), "undodgeableMineflayerBounce");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();

                CustomTrailRenderer projectileTrailRendererController = entry.BulletObject.GetComponentInChildren<CustomTrailRenderer>();
                projectileTrailRendererController.colors = new Color[] { Color.cyan };
                undodgeableMineflayerBounce = entry;

            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("slam"), "UndodgeableOldKingSlamBullet", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableOldKingSlamBullet = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("suck"), "UndodgeableOldKingSuckBullet", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableOldKingSuckBullet = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("homingRing"), "UndodgeableOldKingHomingRingBullet", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableOldKingHomingRingBullet = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("homingRing"), "UndodgeableOldKingHomingRingBullet", null, null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableOldKingHomingRingBulletSoundless = entry;
            }

            
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"), "UndodgeableFrogger", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableFrogger = entry;
            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("f3b04a067a65492f8b279130323b41f0").bulletBank.GetBullet("wave"), "UndodgeableWallWave", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableWallWave = entry;
            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"), "UndodgeableSweep", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableSweep = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("mergoWave"), "UndodgeableMergoWave", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableMergoWave = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("cross"), "UndodgeableCross", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableCross = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("slam"), "UndodgeableSlammo", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                undodgeableBulletKingSlam = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"), "UndodgeableDirectedfire", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                entry.BulletObject.GetComponent<Projectile>().shouldRotate = true;
                UndodgeableDirectedfire = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"), "UndodgeableDirectedfireNoAudio", null, null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                entry.BulletObject.GetComponent<Projectile>().shouldRotate = true;
                UndodgeableDirectedfireSoundless = entry;
            }

            //UndodgeableDirectedfireSoundless
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("c00390483f394a849c36143eb878998f").bulletBank.GetBullet("angrybullet"), "UndodgeableAngrybullet", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableAngrybullet = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("9189f46c47564ed588b9108965f975c9").bulletBank.GetBullet("burst"), "UndodgeableDoorLordBurst", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableDoorLordBurst = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("9189f46c47564ed588b9108965f975c9").bulletBank.GetBullet("flame"), "UndodgeableDoorLordFlame", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableDoorLordFlame = entry;
            }
            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("9189f46c47564ed588b9108965f975c9").bulletBank.GetBullet("puke_burst"), "UndodgeableDoorLordPuke", "DNC", null, false);
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                UndodgeableDoorLordPuke = entry;
            }
        }

        //flame
        //burst//puke_burst

        //angrybullet

        //				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("fa76c8cfdf1c4a88b55173666b4bc7fb").bulletBank.GetBullet("hugeBullet"));

        /*mergoWave//ffca09398635467da3b1f4a54bcfda80
         *4b992de5b4274168a8878ef9bf7ea36b 
         *homingRing
         *bounce
         *
         *8b0dd96e2fe74ec7bebc1bc689c0008a
         *scatterBullet
         *cross
         *
         *frogger
         *68a238ed6a82467ea85474c595c49c6e
         */
        //randomBullet//bigBullet
        //1b5810fafbec445d89921a4efb4e42b7

        //5729c8b5ffa7415bb3d01205663a33ef

        //f3b04a067a65492f8b279130323b41f0

        //directedfire

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

        public static AIBulletBank.Entry undodgeableBouncyBatBullet;
        public static AIBulletBank.Entry NemesisGuon;

    }
}
