using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside
{
    public static class NewModuleExtensions
    {
        public static ProjectileVolleyData CombineVolleys(Gun sourceGun, Gun mergeGun)
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

        public static Gun GetRandomGunOfQualitiesAndShootStyle(System.Random usedRandom, List<int> excludedIDs, ProjectileModule.ShootStyle shootStyle, params PickupObject.ItemQuality[] qualities)
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

    }
}
