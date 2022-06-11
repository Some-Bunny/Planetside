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

        }
        public static Texture VoidTexture;
        public static Texture NebulaTexture;
    }

    public class StaticUndodgeableBulletEntries
    {
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
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                undodgeableSkullvar.BulletObject = projectile.gameObject;
                undodgeableSkull = undodgeableSkullvar;
                undodgeableSkull.AudioEvent = null;
                undodgeableSkull.MuzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };




                //					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));

            }

            {
                AIBulletBank.Entry entry = CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"), "undodgeableBigBullet");
                entry.BulletObject.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
                undodgeableBigBullet = entry;
            }
        }

        private static AIBulletBank.Entry CopyBulletBankEntry(AIBulletBank.Entry entryToCopy, string Name, string AudioEvent = null, VFXPool muzzleflashVFX = null)
        {
            AIBulletBank.Entry entry = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(entryToCopy);
            entry.Name = Name;
            Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(entry.BulletObject).GetComponent<Projectile>();
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            entry.BulletObject = projectile.gameObject;
            entry.AudioEvent = AudioEvent;
            entry.MuzzleFlashEffects = muzzleflashVFX == null ? new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] } : muzzleflashVFX;
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
        public static AIBulletBank.Entry undodgeableBigBullet;

        public static AIBulletBank.Entry undodgeableSniper;
        public static AIBulletBank.Entry undodgeableBig;
        public static AIBulletBank.Entry undodgeableSmallSpore;
        public static AIBulletBank.Entry undodgeableLargeSpore;

        public static AIBulletBank.Entry undodgeableDefault;
        public static AIBulletBank.Entry undodgeableQuickHoming;
        public static AIBulletBank.Entry undodgeableIcicle;
        public static AIBulletBank.Entry undodgeableChainLink;
        public static AIBulletBank.Entry undodgeableSkull;

    }
}
