using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MonoMod.RuntimeDetour;


namespace Planetside
{


    public class ThunderShot : AdvancedGunBehavior
    {

        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Thunder-Shot", "bulldog");
            Game.Items.Rename("outdated_gun_mods:thundershot", "psog:thundershot");
            gun.gameObject.AddComponent<ThunderShot>();
            gun.SetShortDescription("Make It Go Boom!");
            gun.SetLongDescription("Fires rounds with embedded detonators in them. Explosives go off on enemy death.\n\nSmells of beard shampoo.");

            gun.SetupSprite(null, "bulldog_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 16);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            gun.SetAnimationFPS(gun.idleAnimation, 1);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(56) as Gun, true, false);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(81) as Gun).gunSwitchGroup;

            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(81) as Gun).muzzleFlashEffects;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2f;
            gun.DefaultModule.cooldownTime = 0.2f;
            gun.DefaultModule.numberOfShotsInClip = 4;
            gun.barrelOffset.transform.localPosition = new Vector3(15f / 16f, 8f / 16f, 0f);
            gun.SetBaseMaxAmmo(200);
            gun.InfiniteAmmo = false;
            gun.gunClass = GunClass.EXPLOSIVE;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN;
            
            //BULLET STATS
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 7f;
            projectile.baseData.force = 10f;
            projectile.baseData.speed = 40f;
            projectile.AppliesFire = false;
            projectile.baseData.range = 50;

            projectile.gameObject.GetOrAddComponent<ThunderShotProjectile>();
            gun.DefaultModule.angleVariance = 9f;

            gun.gameObject.transform.Find("Casing").transform.position = new Vector3(1.1875f, 0.4375f);
            gun.shellsToLaunchOnFire = 0;
            gun.shellsToLaunchOnReload = 0;
            gun.reloadShellLaunchFrame = 0;
            gun.shellCasingOnFireFrameDelay = 0;


            gun.gameObject.transform.Find("Clip").transform.position = new Vector3(1.1875f, 0.4375f);
            gun.clipObject = BreakAbleAPI.BreakableAPIToolbox.GenerateDebrisObject("Planetside/Resources/GunObjects/Clips/bulldogclip.png").gameObject;
            gun.reloadClipLaunchFrame = 4;
            gun.clipsToLaunchOnReload = 1;

            gun.quality = PickupObject.ItemQuality.C;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.DODGELOAD);

            List<string> AAA = new List<string>
            {
                "psog:thundershot"
            };
            List<string> aee = new List<string>
            {
                "bomb",
                "melted_rock",
                "explosive_rounds",
                "proximity_mine",
                "cluster_mine",
                "air_strike"
            };
            CustomSynergies.Add("KA-BLEWY!", AAA, aee, true);
            
            List<string> eee = new List<string>
            {
                "psog:thundershot",
                "double_vision"
            };
            CustomSynergies.Add("   a", eee, null, true);
            
            ThunderShot.ThunderShotID = gun.PickupObjectId;
            ItemIDs.AddToList(gun.PickupObjectId);
            ThunderShot.fleeData = new FleePlayerData();
            ThunderShot.fleeData.StartDistance = 100f;
            gun.gunClass = GunClass.EXPLOSIVE;

        }
        public static int ThunderShotID;

        private static FleePlayerData fleeData;

        protected override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
            ETGMod.AIActor.OnPreStart += AIActorMods;
        }

        protected override void OnPostDrop(PlayerController player)
        {
            ETGMod.AIActor.OnPreStart -= AIActorMods;
            base.OnPostDrop(player);  
        }
        public void OnDestroy()
        {
            ETGMod.AIActor.OnPreStart -= AIActorMods;
        }
        public void AIActorMods(AIActor target)
        {
            if (target && target.aiActor && target.aiActor.EnemyGuid != null)
            {
                string enemyGuid = target?.aiActor?.EnemyGuid;
                if (!string.IsNullOrEmpty(enemyGuid))
                {
                    PlayerController player = gun.CurrentOwner as PlayerController;
                    if (player != null)
                    {
                        try
                        {
                            if (bugCreatures.Contains(enemyGuid) && player.PlayerHasActiveSynergy("ROCK, AAAAND, STOOOOONE!"))
                            {
                                target.healthHaver.SetHealthMaximum(target.healthHaver.GetMaxHealth() * 0.6f);
                                FleePlayerData data = ThunderShot.fleeData;
                                data.Player = player;
                                target.behaviorSpeculator.FleePlayerData = data;
                                return;
                            }
                            else if (enemyGuid == EnemyGuidDatabase.Entries["minelet"] && player.PlayerHasActiveSynergy("ROCK, AAAAND, STOOOOONE!"))
                            {
                                target.ApplyEffect(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultPermanentCharmEffect, 1f, null);
                                target.gameObject.AddComponent<KillOnRoomClear>();
                                target.IsHarmlessEnemy = true;
                                target.IgnoreForRoomClear = true;
                                if (target.gameObject.GetComponent<SpawnEnemyOnDeath>())
                                {
                                    Destroy(target.gameObject.GetComponent<SpawnEnemyOnDeath>());
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            ETGModConsole.Log(e.Message);
                        }
                    }        
                }
            }
        }
        public static List<string> bugCreatures = new List<string>()
        {
            EnemyGuidDatabase.Entries["shotgrub"],
            EnemyGuidDatabase.Entries["creech"],
            EnemyGuidDatabase.Entries["bullat"],
            EnemyGuidDatabase.Entries["shotgat"],
            EnemyGuidDatabase.Entries["grenat"],
            EnemyGuidDatabase.Entries["spirat"],
            EnemyGuidDatabase.Entries["king_bullat"],
            EnemyGuidDatabase.Entries["gargoyle"],
            EnemyGuidDatabase.Entries["gigi"],
            EnemyGuidDatabase.Entries["misfire_beast"],
            EnemyGuidDatabase.Entries["phaser_spider"],
            EnemyGuidDatabase.Entries["gunzookie"],
            EnemyGuidDatabase.Entries["gunzockie"],
            EnemyGuidDatabase.Entries["bird_parrot"],
            EnemyGuidDatabase.Entries["western_snake"],
            EnemyGuidDatabase.Entries["blue_fish_bullet_kin"],
            EnemyGuidDatabase.Entries["green_fish_bullet_kin"],
            EnemyGuidDatabase.Entries["tarnisher"],
            EnemyGuidDatabase.Entries["great_bullet_shark"],
            EnemyGuidDatabase.Entries["bullet_shark"],

        };
    }
}

