using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using Alexandria.ItemAPI;
using UnityEngine;
using Dungeonator;
using Planetside;

namespace Planetside
{
    using Gungeon;
    using Alexandria.ItemAPI;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    namespace Items
    {
        class SpiritOfTheDragun : ItemAPI.AdvancedGunBehavior
        {
            public static int gunID;
            public static void Add()
            {
                Gun gun = ETGMod.Databases.Items.NewGun("Spirit Of The Dragun", "colossus");
                Game.Items.Rename("outdated_gun_mods:spirit_of_the_dragun", "cel:spirit_of_the_dragun");
                gun.gameObject.AddComponent<SpiritOfTheDragun>();
                gun.SetShortDescription("Ego Sum Aeternae");
                gun.SetLongDescription("Cycles between multiple attack types on reloading an empty clip.\n\nA gun birthed from the vengeful soul of a Dragun. Even it death, it radiates searing heat.");
                gun.SetupSprite(null, "colossus_idle_001", 13);
                gun.SetAnimationFPS(gun.shootAnimation, 12);
                gun.SetAnimationFPS(gun.reloadAnimation, 8);
                gun.AddProjectileModuleFrom("ak-47");
                gun.reloadTime = 1.8f;
                gun.SetBaseMaxAmmo(213);
                gun.gunClass = GunClass.FULLAUTO;
                gun.InfiniteAmmo = true;
                gun.quality = PickupObject.ItemQuality.EXCLUDED;
                Gun gun2 = PickupObjectDatabase.GetById(15) as Gun;
                Gun gun3 = PickupObjectDatabase.GetById(336) as Gun;
                gun.gunSwitchGroup = gun3.gunSwitchGroup;
                gun.barrelOffset.transform.localPosition = new Vector3(2.5f, .5f, 0f);
                gun.muzzleFlashEffects = gun2.muzzleFlashEffects;
                gun.encounterTrackable.EncounterGuid = "Easily the strongest gun ive made.";
                gun.sprite.IsPerpendicular = true;

                //Fireball Module
                fireballData.InitializeFrom(gun.RawSourceVolley);
                fireballModule = fireballData.projectiles[0];



                fireballModule.ammoCost = 1;
                fireballModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
                fireballModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

                fireballModule.angleVariance = 20f;
                fireballModule.cooldownTime = .02f;

                fireballModule.numberOfShotsInClip = 50;

                Projectile fireballProjectile = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(336) as Gun).DefaultModule.projectiles[0]);
                fireballProjectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(fireballProjectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(fireballProjectile);
                fireballModule.projectiles[0] = fireballProjectile;
                fireballProjectile.baseData.damage *= 1.5f;
                fireballProjectile.baseData.speed *= .5f;
                PierceProjModifier orAddComponent = fireballProjectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                orAddComponent.penetration -= 2;
                fireballProjectile.transform.parent = gun.barrelOffset;
                fireballProjectile.ignoreDamageCaps = true;

                //Bouncing Module
                bouncingData.InitializeFrom(gun.RawSourceVolley);
                bouncingModule = bouncingData.projectiles[0];


                bouncingModule.ammoCost = 1;
                bouncingModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
                bouncingModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

                bouncingModule.angleVariance = 5f;
                bouncingModule.cooldownTime = .05f;

                bouncingModule.numberOfShotsInClip = 30;

                Projectile bouncingProjectile = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(30) as Gun).DefaultModule.projectiles[0]);
                bouncingProjectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(bouncingProjectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(bouncingProjectile);
                bouncingModule.projectiles[0] = bouncingProjectile;
                bouncingProjectile.baseData.damage *= 1.5f;
                BounceProjModifier bmod = bouncingProjectile.gameObject.AddComponent<BounceProjModifier>();
                bmod.damageMultiplierOnBounce = 1;
                bmod.numberOfBounces = 3;

                bouncingProjectile.transform.parent = gun.barrelOffset;
                bouncingProjectile.ignoreDamageCaps = true;

                //Rocket Module
                rocketData.InitializeFrom(gun.RawSourceVolley);
                rocketModule = rocketData.projectiles[0];

                rocketModule.ammoCost = 1;
                rocketModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
                rocketModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

                rocketModule.angleVariance = 0f;
                rocketModule.cooldownTime = .3f;

                rocketModule.numberOfShotsInClip = 3;

                Projectile rocketProjectile = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(39) as Gun).DefaultModule.projectiles[0]);
                rocketProjectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(rocketProjectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(rocketProjectile);
                rocketModule.projectiles[0] = rocketProjectile;
                ExplosiveModifier explosiveModifier = rocketProjectile.GetComponent<ExplosiveModifier>();
                ExplosionData data = explosiveModifier.explosionData;
                data.damage *= 2;
                rocketProjectile.gameObject.AddComponent<SpawnFireballsOnHit>();



                rocketProjectile.transform.parent = gun.barrelOffset;
                rocketProjectile.ignoreDamageCaps = true;
                //Setting Initial Module
                BraveUtility.Swap<ProjectileVolleyData>(ref gun.rawVolley, ref fireballData);
                ETGMod.Databases.Items.Add(gun.GetComponent<PickupObject>());
                gunID = gun.PickupObjectId;


            }
            private static ProjectileVolleyData fireballData = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            private static ProjectileVolleyData bouncingData = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            private static ProjectileVolleyData rocketData = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            private static ProjectileModule fireballModule;
            private static ProjectileModule bouncingModule;
            private static ProjectileModule rocketModule;
            [SerializeField]
            public int currentModule = 0;
            private bool HasReloaded;
            public override void Update()
            {
                if (gun.CurrentOwner)
                {


                    if (!gun.IsReloading && !HasReloaded)
                    {
                        this.HasReloaded = true;
                    }
                }
            }
            public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
            {
                if (gun.IsReloading && this.HasReloaded)
                {
                    HasReloaded = false;
                    AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                    base.OnReloadPressed(player, gun, bSOMETHING);
                    aaa(player, gun);
                }
            }

            public void aaa(PlayerController player, Gun gun)
            {
                currentModule++;
                if (currentModule >= 3)
                {
                    currentModule = 0;
                }
                Debug.Log(currentModule);

                switch (currentModule)
                {
                    case 0:
                        BraveUtility.Swap<ProjectileVolleyData>(ref this.gun.rawVolley, ref fireballData);
                        break;
                    case 1:
                        BraveUtility.Swap<ProjectileVolleyData>(ref this.gun.rawVolley, ref bouncingData);
                        break;
                    case 2:
                        BraveUtility.Swap<ProjectileVolleyData>(ref this.gun.rawVolley, ref rocketData);
                        break;
                }
                player.stats.RecalculateStats(player, false, false);
                GameUIRoot.Instance.UpdateGunData(player.inventory, 0, player);


                base.OnReload(player, gun);
            }

            public SpiritOfTheDragun()
            {

            }
            class SpawnFireballsOnHit : MonoBehaviour
            {
                void Start()
                {
                    proj = gameObject.GetComponent<Projectile>();
                    proj.specRigidbody.OnPreRigidbodyCollision += OnCollide;
                    proj.OnDestruction += SpawnFireGoop;
                    owner = proj.Owner;
                    shooter = proj.Shooter;
                }

                private void OnCollide(SpeculativeRigidbody pBody, PixelCollider pCollider, SpeculativeRigidbody otherBody, PixelCollider otherCollider)
                {
                    AIActor potentialEnemy = null;
                    if (otherBody.gameObject.GetComponent<AIActor>())
                    {
                        potentialEnemy = otherBody.gameObject.GetComponent<AIActor>();
                    }
                    if (potentialEnemy)
                    {
                        for (int i = 0; i < 16; i++)
                        {

                            GameObject gameObject = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(336) as Gun).DefaultModule.projectiles[0].gameObject, pBody.sprite.WorldCenter, Quaternion.Euler(0f, 0f, 22.5f * i), true);
                            Projectile p2 = gameObject.GetComponent<Projectile>();
                            p2.Owner = owner;
                            p2.Shooter = shooter;
                            p2.baseData.speed *= .75f;
                            if (potentialEnemy != null)
                            {
                                p2.specRigidbody.RegisterSpecificCollisionException(potentialEnemy.specRigidbody);
                            }

                            p2.AdditionalScaleMultiplier = 1f;
                        }
                    }
                }

                private void SpawnFireGoop(Projectile p)
                {
                    //DeadlyDeadlyGoopManager ddgm = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Library.FireDef);
                    //ddgm.TimedAddGoopCircle(p.sprite.WorldCenter, 4f, .3f);
                }


                private Projectile proj;
                private GameActor owner;
                private SpeculativeRigidbody shooter;
            }
        }
    }


}