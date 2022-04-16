using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;

namespace Planetside
{
    public class StaticVFXStorage
    {
        public static void Init()
        {
            RadialRing = (GameObject)ResourceCache.Acquire("Global VFX/HeatIndicator");
            TeleportDistortVFX = (PickupObjectDatabase.GetById(573) as ChestTeleporterItem).TeleportVFX;
            TeleportVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Teleport_Beam");

            var scarf = PickupObjectDatabase.GetById(436) as BlinkPassiveItem;
            ScarfObject = scarf.ScarfPrefab;

            var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("TheperkParticle"));//this is the name of the object which by default will be "Particle System"
            PerkParticleObject = partObj;
            FakePrefab.MarkAsFakePrefab(partObj);
            PerkParticleSystem = partObj.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(PerkParticleSystem.gameObject);
            var main = PerkParticleSystem.main;
            main.stopAction = ParticleSystemStopAction.None;
            main.maxParticles = 2000;
            main.duration = 1200;
            JammedDeathVFX = (GameObject)BraveResources.Load("Global VFX/VFX_BlackPhantomDeath", ".prefab");

            var PerfpartObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PerfectedParticles"));
            FakePrefab.MarkAsFakePrefab(PerfpartObj);
            PerfectedParticleSystem = PerfpartObj.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(PerfectedParticleSystem.gameObject);


            var mainperf = PerkParticleSystem.main;
            mainperf.stopAction = ParticleSystemStopAction.None;
            mainperf.maxParticles = 2000;
            mainperf.duration = 1200;


            var ceramicObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("CeramicParticles"));
            FakePrefab.MarkAsFakePrefab(ceramicObj);
            CeramicParticleSystem = ceramicObj.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(CeramicParticleSystem.gameObject);

            var bloodSplatterObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("BloodSplatter"));
            FakePrefab.MarkAsFakePrefab(bloodSplatterObj);
            BloodSplatterParticleSystem = bloodSplatterObj.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(BloodSplatterParticleSystem.gameObject);

            var shamberParticlesObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("ShamberParticles"));
            ShamberParticleSystemGameObject = shamberParticlesObj;
            FakePrefab.MarkAsFakePrefab(shamberParticlesObj);
            ShamberParticleSystem = shamberParticlesObj.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(ShamberParticleSystem.gameObject);

            var EliteVariantEnemyParticlesObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("EliteVariantEnemy"));
            EliteParticleSystemGameObject = EliteVariantEnemyParticlesObj;
            FakePrefab.MarkAsFakePrefab(EliteVariantEnemyParticlesObj);
            EliteParticleSystem = EliteVariantEnemyParticlesObj.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(EliteParticleSystem.gameObject);


            var portalParticleObject = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PortalClose"));
            PortalParticleSystemGameObject = portalParticleObject;
            FakePrefab.MarkAsFakePrefab(portalParticleObject);
            PortalParticleSystem = portalParticleObject.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(PortalParticleSystem.gameObject);

            EnemySpawnVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_SpawnEnemy_Reticle");
            ShootGroundVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Bullet_Spawn");
            BlueSynergyPoofVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001");

            HealingSparklesVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Healing_Sparkles_001");

            GameObject ChallengeManagerReference = LoadHelper.LoadAssetFromAnywhere<GameObject>("_ChallengeManager");
            GorgunEyesVFX = (ChallengeManagerReference.GetComponent<ChallengeManager>().PossibleChallenges[20].challenge as FloorShockwaveChallengeModifier).EyesVFX;

            EnemyElectricLinkVFX = (ChallengeManagerReference.GetComponent<ChallengeManager>().PossibleChallenges[18].challenge as CircleBurstChallengeModifier).ChainLightningVFX;
            EnemyZappyTellVFX = (ChallengeManagerReference.GetComponent<ChallengeManager>().PossibleChallenges[18].challenge as CircleBurstChallengeModifier).tellVFX;

            GameObject dragunBoulder = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyBoulder;
            foreach (Component item in dragunBoulder.GetComponentsInChildren(typeof(Component)))
            {
                if (item is SkyRocket laser)
                {
                    DragunBoulderLandVFX = laser.ExplosionData.effect;       
                }
            }

            BeholsterController behCont = EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b").GetComponent<BeholsterController>();
            BeholsterChargeUpVFX = behCont.chargeUpVfx;
            BeholsterChargeDownVFX = behCont.chargeDownVfx;

            GunFullyChargedVFX = BraveResources.Load<GameObject>("Global VFX/VFX_DBZ_Charge", ".prefab");

            FriendlyElectricLinkVFX = FakePrefab.Clone(Game.Items["shock_rounds"].GetComponent<ComplexProjectileModifier>().ChainLightningVFX);

        }
        public static VFXPool BeholsterChargeUpVFX;
        public static VFXPool BeholsterChargeDownVFX;


        public static GameObject DragunBoulderLandVFX;

        public static GameObject GunFullyChargedVFX;

        public static GameObject GorgunEyesVFX;
        public static GameObject EnemyElectricLinkVFX;
        public static GameObject EnemyZappyTellVFX;
        public static GameObject FriendlyElectricLinkVFX;


        public static ScarfAttachmentDoer ScarfObject;
        public static GameObject RadialRing;
        public static GameObject TeleportDistortVFX;
        public static GameObject TeleportVFX;
        public static GameObject EnemySpawnVFX;
        public static GameObject ShootGroundVFX;
        public static GameObject BlueSynergyPoofVFX;
        public static GameObject HealingSparklesVFX;

        public static ParticleSystem PerkParticleSystem;
        public static ParticleSystem PerfectedParticleSystem;
        public static ParticleSystem PortalParticleSystem;



        public static GameObject PerkParticleObject;

        public static ParticleSystem CeramicParticleSystem;
        public static ParticleSystem BloodSplatterParticleSystem;
        public static ParticleSystem ShamberParticleSystem;
        public static ParticleSystem EliteParticleSystem;

        public static GameObject ShamberParticleSystemGameObject;
        public static GameObject EliteParticleSystemGameObject;
        public static GameObject PortalParticleSystemGameObject;


        public static GameObject JammedDeathVFX;
        

    }
}
