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
    public class StaticUndodgeableBulletEntries
    {
        public static void Init()
        {
            undodgeableSniper = StaticUndodgeableBulletEntries.CopyFields<AIBulletBank.Entry>(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
            undodgeableSniper.Name = "sniperUndodgeable";
            Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(undodgeableSniper.BulletObject).GetComponent<Projectile>();
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.gameObject.AddComponent<MarkForUndodgeAbleBullet>();
            undodgeableSniper.BulletObject = projectile.gameObject;




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



        public static AIBulletBank.Entry undodgeableSniper;
        public static AIBulletBank.Entry undodgeableBig;

    }
}
