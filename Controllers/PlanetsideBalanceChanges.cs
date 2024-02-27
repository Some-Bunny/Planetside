using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Collections;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;
using GungeonAPI;
using System.IO;
using ItemAPI;
using UnityEngine.UI;


using static Planetside.MultiActiveReloadManager;
using static ProjectileModule;

namespace Planetside
{
    public class PlanetsideBalanceChanges
    {
        public static void Init()
        {
            Debug.Log("Starting PlanetsideBalanceChanges setup...");
            try
            {
                CreateOrLoadConfiguration();
                MakeCommands();

                if (BalanceConfig.HeartItemChanges == true)
                {
                    new Hook(typeof(HeartDispenser).GetMethod("Update", BindingFlags.Instance | BindingFlags.Public),
                        typeof(PlanetsideBalanceChanges).GetMethod("HeartBottleBuff"));
                    GameManager.Instance.OnNewLevelFullyLoaded += NewFloor;

                    BasicStatPickup HeartPurseItem = PickupObjectDatabase.GetById(425) as BasicStatPickup;
                    HeartPurseItem.AddPassiveStatModifier(PlayerStats.StatType.GlobalPriceMultiplier, 0.85f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    HeartPurseItem.CurrencyToGive = 15;
                    HeartPurseItem.GivesCurrency = true;

                    new Hook(typeof(Gun).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public),
                        typeof(PlanetsideBalanceChanges).GetMethod("HealOnGunPickup"));

                    BasicStatPickup HeartLunchboxItem = PickupObjectDatabase.GetById(422) as BasicStatPickup;
                    HeartLunchboxItem.AddPassiveStatModifier(PlayerStats.StatType.AdditionalItemCapacity, 1f, StatModifier.ModifyMethod.ADDITIVE);
                    HeartLunchboxItem.AddPassiveStatModifier(PlayerStats.StatType.AmmoCapacityMultiplier, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);

                    ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPreStart, new Action<AIActor>(AIActorMods));        
                }
                if (BalanceConfig.WeaponChanges == true)
                {
                    //Buffs
                    Gun Vertebreak = PickupObjectDatabase.GetById(29) as Gun;
                    Vertebreak.SetBaseMaxAmmo(360);//Up from 300
                    Vertebreak.ammo = 360;

                    Gun Excaliber = PickupObjectDatabase.GetById(377) as Gun;
                    Excaliber.SetBaseMaxAmmo(300); //up from 280
                    Excaliber.ammo = 300;
                    Excaliber.DefaultModule.numberOfShotsInClip = 30; //down from 32

                    Gun ExcaliberArmoredCorps = PickupObjectDatabase.GetById(680) as Gun;
                    ExcaliberArmoredCorps.SetBaseMaxAmmo(300); //up from 280
                    ExcaliberArmoredCorps.ammo = 300;
                    ExcaliberArmoredCorps.DefaultModule.numberOfShotsInClip = 30; //down from 32

                    Gun ShotBow = PickupObjectDatabase.GetById(126) as Gun;
                    ShotBow.SetBaseMaxAmmo(120); //Up from 80
                    ShotBow.ammo = 120;    
                    foreach (ProjectileModule projectileModule in ShotBow.Volley.projectiles)
                    {
                        projectileModule.cooldownTime *= 0.7f; //reduced by 30%
                        projectileModule.numberOfShotsInClip = 4; //up from 3
                    }

                    Gun ShotBowSecondAccident = PickupObjectDatabase.GetById(749) as Gun;
                    ShotBowSecondAccident.SetBaseMaxAmmo(120); //Up from 80
                    ShotBowSecondAccident.ammo = 120;
                    foreach (ProjectileModule projectileModule in ShotBowSecondAccident.Volley.projectiles)
                    {
                        projectileModule.cooldownTime *= 0.7f; //reduced by 30%
                        projectileModule.numberOfShotsInClip = 4; //up from 3
                    }


                    Gun Blooper = PickupObjectDatabase.GetById(18) as Gun;
                    Blooper.SetBaseMaxAmmo(100);//Up from 80
                    Blooper.ammo = 100;
                    foreach (ProjectileModule projectileModule in Blooper.Volley.projectiles)
                    { projectileModule.cooldownTime *= 0.8f;  } //increased by 20%

                    Gun Winchester = PickupObjectDatabase.GetById(1) as Gun;
                    Winchester.SetBaseMaxAmmo(120); //Up from 100
                    Winchester.ammo = 120;

                    Gun H4mmer = PickupObjectDatabase.GetById(91) as Gun;
                    H4mmer.SetBaseMaxAmmo(500); //Up from 450
                    H4mmer.ammo = 500;
                    H4mmer.DefaultModule.numberOfShotsInClip = 25;//down from 30, makes the final shot used more often

                    Gun H4mmerAndNail = PickupObjectDatabase.GetById(715) as Gun;
                    H4mmerAndNail.SetBaseMaxAmmo(500); //Up from 450
                    H4mmerAndNail.ammo = 500;
                    H4mmerAndNail.DefaultModule.numberOfShotsInClip = 25;//down from 30, makes the final shot used more often

                    Gun Ashes = PickupObjectDatabase.GetById(198) as Gun;
                    Ashes.SetBaseMaxAmmo(90);//up from 80
                    Ashes.ammo = 90;
                    Ashes.DefaultModule.cooldownTime *= 0.85f; //increased by 20%

                    Gun FrostGiant = PickupObjectDatabase.GetById(387) as Gun;
                    FrostGiant.SetBaseMaxAmmo(180);//up from 130
                    FrostGiant.ammo = 180;

                    Gun Glacier = PickupObjectDatabase.GetById(130) as Gun;
                    Glacier.DefaultModule.cooldownTime *= 0.75f; //increased by 25%
                    Glacier.SetBaseMaxAmmo(180); //up from 120
                    Glacier.ammo = 180;

                    Gun TurboGun = PickupObjectDatabase.GetById(577) as Gun;
                    TurboGun.SetBaseMaxAmmo(200); //up from 180
                    TurboGun.ammo = 200;
                    TurboGun.DefaultModule.numberOfShotsInClip = 12; //Up from 6
                    TurboGun.reloadTime *= 1.25f; //increased by 25%, makes doing the reload gimmick easier

                    Gun GreyMauser = PickupObjectDatabase.GetById(130) as Gun;
                    GreyMauser.SetBaseMaxAmmo(150); //up from 100
                    GreyMauser.ammo = 150;

                    Gun Polaris = PickupObjectDatabase.GetById(97) as Gun;
                    foreach (ProjectileModule projectileModule in Polaris.Volley.projectiles)
                    {
                       projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                    }
                    Gun Polaris_Syn = PickupObjectDatabase.GetById(718) as Gun;
                    foreach (ProjectileModule projectileModule in Polaris_Syn.Volley.projectiles)
                    {
                        projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                    }


                    //Nerfs        
                    Gun MegaHand = PickupObjectDatabase.GetById(36) as Gun;
                    MegaHand.SetBaseMaxAmmo(120);
                    MegaHand.ammo = 120;
                    MegaHand.DefaultModule.numberOfShotsInClip = 15;

                    ModifyGunStats(617 ,120);
                    ModifyGunStats(618, 120);
                    ModifyGunStats(619, 120);
                    ModifyGunStats(620, 120);
                    ModifyGunStats(621, 120);
                    ModifyGunStats(622, 120);
                    ModifyGunStats(623, 120);
                    ModifyGunStats(624, 120);

                    Gun Heroine = PickupObjectDatabase.GetById(41) as Gun;
                    Heroine.SetBaseMaxAmmo(120);
                    Heroine.ammo = 120;

                    ModifyGunStats(612, 120);
                    ModifyGunStats(613, 120);
                    ModifyGunStats(614, 120);
                    ModifyGunStats(615, 120);

                }

                Debug.Log("Finished PlanetsideBalanceChanges setup without failure!");
            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish PlanetsideBalanceChanges setup!");
                Debug.Log(e);
            }
        }

        public static void ModifyGunStats(int gunID, int NewMaxAmmo = -1, float FirerateMultiplier = -1, int clipShots = -1, float reloadTimeMultiplier = -1)
        {
            Gun gun = PickupObjectDatabase.GetById(gunID) as Gun;
            if (NewMaxAmmo != -1) { gun.SetBaseMaxAmmo(NewMaxAmmo); gun.ammo = NewMaxAmmo; }
            if (FirerateMultiplier != -1) { gun.DefaultModule.cooldownTime *= FirerateMultiplier; }
            if (clipShots != -1) { gun.DefaultModule.numberOfShotsInClip = clipShots; }
            if (reloadTimeMultiplier != -1) { gun.reloadTime = reloadTimeMultiplier; }
        }





        public static readonly string ENABLED_COLOR = "#00d652";
        public static readonly string DISABLED_COLOR = "#bc0000";
        public static readonly string TEXT_COLOR = "#bc9545";

        public static Dictionary<string, float> bulletEnemiesToChanceCharm = new Dictionary<string, float>
        {
            {EnemyGuidDatabase.Entries["bullet_kin"], 0.2f},
            {EnemyGuidDatabase.Entries["ak47_bullet_kin"], 0.15f},
            {EnemyGuidDatabase.Entries["bandana_bullet_kin"], 0.15f},
            {EnemyGuidDatabase.Entries["minelet"], 0.125f},
            {EnemyGuidDatabase.Entries["cardinal"], 0.1f},
            {EnemyGuidDatabase.Entries["shroomer"], 0.5f},
            {EnemyGuidDatabase.Entries["ashen_bullet_kin"], 0.1f},
            {EnemyGuidDatabase.Entries["mutant_bullet_kin"], 0.166f},
            {EnemyGuidDatabase.Entries["fallen_bullet_kin"], 0.0666f},
            {EnemyGuidDatabase.Entries["office_bullet_kin"], 0.166f},
            {EnemyGuidDatabase.Entries["office_bullette_kin"], 0.1666f},
            {EnemyGuidDatabase.Entries["western_bullet_kin"], 0.20f},
            {EnemyGuidDatabase.Entries["pirate_bullet_kin"], 0.20f},
        };
        public static void AIActorMods(AIActor target)
        {
            if (AnyPlayerHasItemOfID(423) == true)
            {
                if (target && target.aiActor && target.aiActor.EnemyGuid != null)
                {
                    try
                    {
                        if (PlanetsideBalanceChanges.bulletEnemiesToChanceCharm.ContainsKey(target.aiActor.EnemyGuid))
                        {
                            float chance = 0;
                            PlanetsideBalanceChanges.bulletEnemiesToChanceCharm.TryGetValue(target.aiActor.EnemyGuid, out chance);
                            if (UnityEngine.Random.value < chance)
                            {
                                target.ApplyEffect(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultPermanentCharmEffect, 1f, null);
                                target.gameObject.AddComponent<KillOnRoomClear>();
                                target.IsHarmlessEnemy = true;
                                target.IgnoreForRoomClear = true;
                                if (target.gameObject.GetComponent<SpawnEnemyOnDeath>())
                                {
                                    UnityEngine.Object.Destroy(target.gameObject.GetComponent<SpawnEnemyOnDeath>());
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        ETGModConsole.Log(ex.Message, false);
                    }
                }
            }        
        }
        public static void HealOnGunPickup(Action<Gun, PlayerController> orig, Gun self, PlayerController player)
        {
            orig(self, player);
            if (player.HasPickupID(421) && self != null && self.HasBeenPickedUp == false)
            {
                AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", player.gameObject);
                player.PlayEffectOnActor(PickupObjectDatabase.GetById(73).GetComponent<HealthPickup>().healVFX, Vector3.zero, true, false, false);
                if (player.AllowZeroHealthState == true)
                {player.healthHaver.Armor += 1; }
                else
                {player.healthHaver.ApplyHealing(0.5f);}
            }
        }
        private static void NewFloor()
        {
            if (AnyPlayerHasItemOfID(424) == true)
            {
               HeartDispenser.CurrentHalfHeartsStored = HeartsStored;
            }
        }
        private static int HeartsStored;
        public static void HeartBottleBuff(Action<HeartDispenser> orig, HeartDispenser self)
        {
            orig(self);
            if (self != null)
            {
                if (PlanetsideReflectionHelper.ReflectGetField<bool>(typeof(HeartDispenser), "m_isVisible", self) == true && HeartDispenser.CurrentHalfHeartsStored > 0)
                {          
                    HeartsStored = (int)Mathf.Min(HeartDispenser.CurrentHalfHeartsStored, 2);
                }
            }
        }
        public static bool AnyPlayerHasItemOfID(int ID)
        {
            for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
            {
                PlayerController player = GameManager.Instance.AllPlayers[i];
                if (player.HasPickupID(ID))
                {
                    return true; 
                }
            }
            return false;
        }


        public static void MakeCommands()
        {
            ETGModConsole.Commands.AddGroup("psog_balance");
            ETGModConsole.Commands.GetGroup("psog_balance").AddUnit("help", delegate (string[] args)
            {
                PlanetsideBalanceChanges.Log("Balance Options", PlanetsideBalanceChanges.TEXT_COLOR);
                PlanetsideBalanceChanges.Log("Green : Enabled", PlanetsideBalanceChanges.ENABLED_COLOR);
                PlanetsideBalanceChanges.Log("Red : Disabled", PlanetsideBalanceChanges.DISABLED_COLOR);
                PlanetsideBalanceChanges.Log("Note: using these commands will toggle them on/off but will also require closing and opening the game.", PlanetsideBalanceChanges.TEXT_COLOR);
                PlanetsideBalanceChanges.Log("==============================", PlanetsideBalanceChanges.TEXT_COLOR);
                PlanetsideQOL.Log("psog_balance heartitemchanges : Makes the 5 Heart Items differ slightly from each other to make them more unique.", BalanceConfig.HeartItemChanges == true ? ENABLED_COLOR : DISABLED_COLOR);
            });
            ETGModConsole.Commands.GetGroup("psog_balance").AddUnit("heartitemchanges", delegate (string[] args)
            {
                BalanceConfig.HeartItemChanges = !BalanceConfig.HeartItemChanges;
                string str = BalanceConfig.HeartItemChanges ? "enabled." : "disabled.";
                PlanetsideBalanceChanges.Log("Heart Item Buffs are now " + str, BalanceConfig.HeartItemChanges == true ? ENABLED_COLOR : DISABLED_COLOR);
                PlanetsideBalanceChanges.Log("Your changes will be applied once the game has been reloaded." + str, BalanceConfig.HeartItemChanges == true ? ENABLED_COLOR : DISABLED_COLOR);
                UpdateConfiguration();
            });
        }
        public static void Log(string text, string color = "#FFFFFF")
        {
            ETGModConsole.Log(string.Concat(new string[]
            {
                "<color=",
                color,
                ">",
                text,
                "</color>"
            }), false);
        }
        private static string BalanceJsonPath = Path.Combine(ETGMod.ResourcesDirectory, "planetsidebalance.json");
        private static void CreateOrLoadConfiguration()
        {
            bool flag = !File.Exists(PlanetsideBalanceChanges.BalanceJsonPath);
            bool flag2 = flag;
            if (flag2)
            {
                ETGModConsole.Log("Planetside Balances: Unable to find existing config, making a new one!", false);
                File.Create(PlanetsideBalanceChanges.BalanceJsonPath).Close();
                UpdateConfiguration();
            }
            else
            {
                string text = File.ReadAllText(PlanetsideBalanceChanges.BalanceJsonPath);
                bool flag3 = !string.IsNullOrEmpty(text);
                bool flag4 = flag3;
                if (flag4)
                {
                    BalanceConfig = JsonUtility.FromJson<PlanetsideBalanceChanges.Configuration>(text);
                }
                else
                {
                    UpdateConfiguration();
                }
            }
        }

        private static void UpdateConfiguration()
        {
            bool flag = !File.Exists(PlanetsideBalanceChanges.BalanceJsonPath);
            bool flag2 = flag;
            if (flag2)
            {
                ETGModConsole.Log("Planetside Balances: Unable to find existing config, making a new one!", false);
                File.Create(PlanetsideBalanceChanges.BalanceJsonPath).Close();
            }
            File.WriteAllText(PlanetsideBalanceChanges.BalanceJsonPath, JsonUtility.ToJson(BalanceConfig, true));
        }

        public static PlanetsideBalanceChanges.Configuration BalanceConfig = new PlanetsideBalanceChanges.Configuration
        {
            HeartItemChanges = true,
            WeaponChanges = true,

        };
        public struct Configuration
        {
            public bool HeartItemChanges;
            public bool WeaponChanges;
        }
    }
}
