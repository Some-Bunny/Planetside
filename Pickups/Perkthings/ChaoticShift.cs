using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections.Generic;
using SaveAPI;
using HarmonyLib;
using static PlayerStats;
using static StatModifier;

using Pathfinding;

using System.Linq;
using tk2dRuntime.TileMap;


namespace Planetside
{
    class FuckYoGunsUp : MonoBehaviour
    {
        public FuckYoGunsUp() { }
        public ChaoticShiftController shiftController;
        public void Start()
        {
            BannedIDs.Add(self.PickupObjectId);
        }

        public void OnDestroy()
        {
            shiftController.RemoveOldModifier(self);

            //RemoveOldModifier();
        }





        public void AddModule()
        {
            AmountOfModulesCurrently++;
            Gun randomGun = GetRandomGunOfQualitiesAndShootStyle(new System.Random(UnityEngine.Random.Range(1, 100000)), BannedIDs, self.DefaultModule.shootStyle, ReturnQualities(self.quality));
            BannedIDs.Add(randomGun.PickupObjectId);
            ProjectileVolleyData projectileVolleyData = CombineVolleys(self, randomGun);

            ReconfigureVolley(projectileVolleyData);
            self.RawSourceVolley = projectileVolleyData;
            self.SetBaseMaxAmmo(self.GetBaseMaxAmmo() + (int)(randomGun.GetBaseMaxAmmo() * 0.3f));
            self.GainAmmo((int)(randomGun.CurrentAmmo * 0.3f));

            self.OnPrePlayerChange();
            player.inventory.ChangeGun(0, false, false);
        }

        public PickupObject.ItemQuality[] ReturnQualities(PickupObject.ItemQuality itemQuality)
        {
            switch(itemQuality)
            {
                case PickupObject.ItemQuality.D:
                    return new PickupObject.ItemQuality[] { PickupObject.ItemQuality.D };
                case PickupObject.ItemQuality.C:
                    return new PickupObject.ItemQuality[] { PickupObject.ItemQuality.D, PickupObject.ItemQuality.C };
                case PickupObject.ItemQuality.B:
                    return new PickupObject.ItemQuality[] { PickupObject.ItemQuality.D, PickupObject.ItemQuality.C, PickupObject.ItemQuality.B };
                case PickupObject.ItemQuality.A:
                    return new PickupObject.ItemQuality[] { PickupObject.ItemQuality.D, PickupObject.ItemQuality.C, PickupObject.ItemQuality.B, PickupObject.ItemQuality.A };
                case PickupObject.ItemQuality.S:
                    return new PickupObject.ItemQuality[] { PickupObject.ItemQuality.D, PickupObject.ItemQuality.C, PickupObject.ItemQuality.B, PickupObject.ItemQuality.A, PickupObject.ItemQuality.S };
                default:
                    return new PickupObject.ItemQuality[] { PickupObject.ItemQuality.D };
            }
        }


        public static Gun GetRandomGunOfQualitiesAndShootStyle(System.Random usedRandom, List<int> excludedIDs, ProjectileModule.ShootStyle shootStyle , params PickupObject.ItemQuality[] qualities )
        {
            List<Gun> list = new List<Gun>();
            for (int i = 0; i < PickupObjectDatabase.Instance.Objects.Count; i++)
            {
                if (PickupObjectDatabase.Instance.Objects[i] != null && PickupObjectDatabase.Instance.Objects[i] is Gun)
                {
                    if (PickupObjectDatabase.Instance.Objects[i].quality != PickupObject.ItemQuality.EXCLUDED && PickupObjectDatabase.Instance.Objects[i].quality != PickupObject.ItemQuality.SPECIAL)
                    {
                        if (!(PickupObjectDatabase.Instance.Objects[i] is ContentTeaserGun))
                        {
                            if (Array.IndexOf<PickupObject.ItemQuality>(qualities, PickupObjectDatabase.Instance.Objects[i].quality) != -1)
                            {
                                if (!excludedIDs.Contains(PickupObjectDatabase.Instance.Objects[i].PickupObjectId))
                                {
                                    if (PickupObjectDatabase.Instance.Objects[i].PickupObjectId != GlobalItemIds.UnfinishedGun || !GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_AMMONOMICON_COMPLETE))
                                    {
                                        EncounterTrackable component = PickupObjectDatabase.Instance.Objects[i].GetComponent<EncounterTrackable>();
                                        if (component && component.PrerequisitesMet())
                                        {
                                            if ((PickupObjectDatabase.Instance.Objects[i] as Gun).DefaultModule.shootStyle == shootStyle)
                                            {
                                                list.Add(PickupObjectDatabase.Instance.Objects[i] as Gun);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            int num = usedRandom.Next(list.Count);
            if (num < 0 || num >= list.Count)
            {
                return null;
            }
            return list[num];
        }

        private List<int> BannedIDs = new List<int>();
        public static void ReconfigureVolley(ProjectileVolleyData newVolley)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            int num = 0;
            for (int i = 0; i < newVolley.projectiles.Count; i++)
            {
                if (newVolley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Automatic)
                {
                    flag = true;
                }
                if (newVolley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Beam)
                {
                    flag = true;
                }
                if (newVolley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Burst)
                {
                    flag4 = true;
                }
                if (newVolley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Charged)
                {
                    flag3 = true;
                    num++;
                }
            }
            if (!flag && !flag2 && !flag3 && !flag4)
            {
                return;
            }
            if (!flag && !flag2 && !flag3 && flag4)
            {
                return;
            }
            int num2 = 0;
            for (int j = 0; j < newVolley.projectiles.Count; j++)
            {
                if (newVolley.projectiles[j].shootStyle == ProjectileModule.ShootStyle.SemiAutomatic)
                {
                    newVolley.projectiles[j].shootStyle = ProjectileModule.ShootStyle.Automatic;
                }
                if (newVolley.projectiles[j].shootStyle == ProjectileModule.ShootStyle.Charged && num > 1)
                {
                    num2++;
                    if (num > 1)
                    {
                    }
                }
            }
        }
        private static ProjectileVolleyData CombineVolleys(Gun sourceGun, Gun mergeGun)
        {
            ProjectileVolleyData projectileVolleyData = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            if (sourceGun.RawSourceVolley != null)
            {
                projectileVolleyData.InitializeFrom(sourceGun.RawSourceVolley);
            }
            else
            {
                projectileVolleyData.projectiles = new List<ProjectileModule>();
                projectileVolleyData.projectiles.Add(ProjectileModule.CreateClone(sourceGun.singleModule, true, -1));
                projectileVolleyData.BeamRotationDegreesPerSecond = float.MaxValue;
            }
            if (mergeGun.RawSourceVolley != null)
            {
                for (int i = 0; i < mergeGun.RawSourceVolley.projectiles.Count; i++)
                {
                    ProjectileModule projectileModule = ProjectileModule.CreateClone(mergeGun.RawSourceVolley.projectiles[i], true, -1);
                    projectileModule.IsDuctTapeModule = true;
                    projectileModule.ignoredForReloadPurposes = (projectileModule.ammoCost <= 0 || projectileModule.numberOfShotsInClip <= 0);
                    projectileVolleyData.projectiles.Add(projectileModule);
                    if (!string.IsNullOrEmpty(mergeGun.gunSwitchGroup) && i == 0)
                    {
                        projectileModule.runtimeGuid = ((projectileModule.runtimeGuid == null) ? Guid.NewGuid().ToString() : projectileModule.runtimeGuid);
                        sourceGun.AdditionalShootSoundsByModule.Add(projectileModule.runtimeGuid, mergeGun.gunSwitchGroup);

                    }
                    if (mergeGun.RawSourceVolley.projectiles[i].runtimeGuid != null && mergeGun.AdditionalShootSoundsByModule.ContainsKey(mergeGun.RawSourceVolley.projectiles[i].runtimeGuid))
                    {
                        sourceGun.AdditionalShootSoundsByModule.Add(mergeGun.RawSourceVolley.projectiles[i].runtimeGuid, mergeGun.AdditionalShootSoundsByModule[mergeGun.RawSourceVolley.projectiles[i].runtimeGuid]);
                    }
                }
            }
            else
            {
                ProjectileModule projectileModule2 = ProjectileModule.CreateClone(mergeGun.singleModule, true, -1);
                projectileModule2.IsDuctTapeModule = true;
                projectileModule2.ignoredForReloadPurposes = (projectileModule2.ammoCost <= 0 || projectileModule2.numberOfShotsInClip <= 0);
                projectileVolleyData.projectiles.Add(projectileModule2);
                if (!string.IsNullOrEmpty(mergeGun.gunSwitchGroup))
                {
                    projectileModule2.runtimeGuid = ((projectileModule2.runtimeGuid == null) ? Guid.NewGuid().ToString() : projectileModule2.runtimeGuid);
                    sourceGun.AdditionalShootSoundsByModule.Add(projectileModule2.runtimeGuid, mergeGun.gunSwitchGroup);

                }
            }
            return projectileVolleyData;
        }

        public PlayerController player;
        public Gun self;
        public int AmountOfModulesCurrently;
    }

    class ChaoticShiftController : MonoBehaviour
    {
        public ChaoticShiftController()
        {
            this.AmountOfModules = 1;
            this.hasBeenPickedup = false;
        }
        public void Start() 
        {
            if (statModifier == null)
            {
                statModifier = new StatModifier()
                {
                    amount = 1,
                    ignoredForSaveData = true,
                    modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE,
                    statToBoost = PlayerStats.StatType.Damage,
                    isMeatBunBuff = true,
                };
            }
            this.hasBeenPickedup = true;
            RecalculateDamageNumber();
            player.GunChanged += GunSwitched;
        }

        public void GunSwitched(Gun g1, Gun g2, bool b)
        {
            AddModifierIfPossible(g2);
        }

        public StatModifier statModifier;


        public void AddModifierIfPossible(Gun g2)
        {
            if (IsGunValid(g2))
            {
                RecalculateDamageNumber();
                RemoveOldModifier(g2);
                if (!g2.currentGunStatModifiers.Contains(statModifier))
                {
                    g2.currentGunStatModifiers = g2.currentGunStatModifiers.Concat(new StatModifier[]
                    {
                        statModifier
                    }).ToArray<StatModifier>();
                }
            }
        }

        public void RecalculateDamageNumber()
        {
            float BaseDamage = 1f;
            for (int i = 0; i < AmountOfModules; i++)
            {
                BaseDamage *= 0.55f;
            }
            statModifier.amount = BaseDamage;
            if (player == null) { return; }
            player.stats.RecalculateStats(player, true, true);
        }

        public void RemoveOldModifier(Gun gun)
        {
            List<StatModifier> L = gun.currentGunStatModifiers.ToList();
            L.RemoveAll(self => self.isMeatBunBuff == true);
            gun.currentGunStatModifiers = L.ToArray();
        }

        



        public void IncrementStack()
        {
            AmountOfModules++;
            RecalculateDamageNumber();
        }

        public void Update()
        {
            if(player)
            {
                List<Gun> gunList = player.inventory.AllGuns;
                for (int i = 0; i < gunList.Count; i++)
                {
                    Gun currentGunInList = gunList[i];
                    if (currentGunInList != null)
                    {
                        if (IsGunValid(currentGunInList))
                        {

                            if (currentGunInList.gameObject.GetComponent<FuckYoGunsUp>() != null)
                            {
                                FuckYoGunsUp guns = currentGunInList.gameObject.GetComponent<FuckYoGunsUp>();
                                if (guns.AmountOfModulesCurrently != AmountOfModules && AmountOfModules >= guns.AmountOfModulesCurrently) 
                                {
                                    guns.AddModule();
                                }
                            }
                            else
                            {
                                FuckYoGunsUp guns = currentGunInList.gameObject.GetOrAddComponent<FuckYoGunsUp>();
                                guns.self = currentGunInList;
                                guns.player = player;
                                guns.shiftController = this;
                                guns.AddModule();
                                //RemoveOldModifier(currentGunInList);
                                AddModifierIfPossible(currentGunInList);

                            }
                        }
                    }         
                }
               
            }
        }
        public bool IsGunValid(Gun g)
        {
            return !g.InfiniteAmmo && g.CanActuallyBeDropped(g.CurrentOwner as PlayerController);
        }

        public bool hasBeenPickedup;
        public int AmountOfModules;
        public PlayerController player;
    }

    class ChaoticShift : PerkPickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Chaotic Shift";
            //string resourcePath = "Planetside/Resources/PerkThings/chaoticShift.png";
            GameObject gameObject = new GameObject(name);
            ChaoticShift item = gameObject.AddComponent<ChaoticShift>();

            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("chaoticShift"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Literally just an all stats up.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            ChaoticShift.ChaoticShiftID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = new Color(0, 255, 0);
            particles.ParticleSystemColor2 = new Color(216, 191, 216);
            item.OutlineColor = new Color(0f, 0.2f, 0f);

          
        }
        public override CustomTrackedStats StatToIncreaseOnPickup => SaveAPI.CustomTrackedStats.AMOUNT_BOUGHT_CHAOTICSHIFT;
        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 0,
                    LockedString = "\"All Weapons are Doubled.\"",
                    UnlockedString = "\"All Weapons are Doubled.\"",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 3,
                    LockedString = AlphabetController.ConvertString("Stat Change Damage Ammo"),
                    UnlockedString = "Reduces your damage, but increases your ammo.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 4,
                    LockedString = AlphabetController.ConvertString("Stacking Adds Guns"),
                    UnlockedString = "Stacking adds another gun, increases ammo and decreases damage more.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.CHAOTICSHIFT_FLAG_STACK
                },
        };


        public static int ChaoticShiftID;


        public new bool PrerequisitesMet()
        {
            EncounterTrackable component = base.GetComponent<EncounterTrackable>();
            return component == null || component.PrerequisitesMet();
        }
        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            SaveAPI.AdvancedGameStatsManager.Instance.RegisterStatChange(StatToIncreaseOnPickup, 1);
            m_hasBeenPickedUp = true;
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);
            OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, 0.1f, StatModifier.ModifyMethod.ADDITIVE);
            //OtherTools.ApplyStat(player, PlayerStats.StatType.AmmoCapacityMultiplier, 1.05f, StatModifier.ModifyMethod.MULTIPLICATIVE);

            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }
            ChaoticShiftController chaos = player.gameObject.GetOrAddComponent<ChaoticShiftController>();
            chaos.player = player;
            if (chaos.hasBeenPickedup == true)
            { chaos.IncrementStack(); SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.CHAOTICSHIFT_FLAG_STACK, true); }
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = chaos.hasBeenPickedup == true ? "Another Gun Added." : "All Weapons are Doubled.";
            //OtherTools.Notify("Chaotic Shift", BlurbText, "Planetside/Resources/PerkThings/chaoticShift", UINotificationController.NotificationColor.GOLD);
            OtherTools.NotifyCustom("Chaotic Shift", BlurbText, "chaoticShift", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);


            UnityEngine.Object.Destroy(base.gameObject);
        }

        public void Start()
        {
            try
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(this);
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            }
            catch (Exception er)
            {
                ETGModConsole.Log(er.Message, false);
            }
        }

      
        private void Update()
        {
            if (!this.m_hasBeenPickedUp && !this.m_isBeingEyedByRat && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
            {
                GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
            }
        }



        private bool m_hasBeenPickedUp;
    }
}
