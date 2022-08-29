using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Planetside
{
    class HoveringGunsAdder
    {
        public static void AddHovers()
        {
            AdvancedHoveringGunSynergyProcessor UglyDuckling1 = (PickupObjectDatabase.GetById(SwanOff.SwanOffID) as Gun).gameObject.AddComponent<AdvancedHoveringGunSynergyProcessor>();
            UglyDuckling1.ConsumesTargetGunAmmo = false;
            UglyDuckling1.AimType = HoveringGunController.AimType.PLAYER_AIM;
            UglyDuckling1.PositionType = HoveringGunController.HoverPosition.CIRCULATE;
            UglyDuckling1.FireType = HoveringGunController.FireType.ON_FIRED_GUN;

            UglyDuckling1.UsesMultipleGuns = false;
            UglyDuckling1.TargetGunID = UglyDuckling.DuckyID;
            UglyDuckling1.RequiredSynergy = "Ugly Duckling";
            UglyDuckling1.FireCooldown = .33f;
            UglyDuckling1.FireDuration = 0.1f;

            AdvancedHoveringGunProcessor DroneHover = (PickupObjectDatabase.GetById(ParasiticHeart.HeartID) as Gun).gameObject.AddComponent<AdvancedHoveringGunProcessor>();
            DroneHover.Activate = true;
            DroneHover.ConsumesTargetGunAmmo = false;
            DroneHover.AimType = CustomHoveringGunController.AimType.PLAYER_AIM;
            DroneHover.PositionType = CustomHoveringGunController.HoverPosition.CIRCULATE;
            DroneHover.FireType = CustomHoveringGunController.FireType.ON_FIRED_GUN;
            DroneHover.UsesMultipleGuns = true;
            DroneHover.TargetGunIDs = new List<int> { PickupObjectDatabase.GetById(EyeOfAnnihilation.EyeOfAnnihilationID).PickupObjectId, PickupObjectDatabase.GetById(EyeOfAnnihilation.EyeOfAnnihilationID).PickupObjectId, PickupObjectDatabase.GetById(EyeOfAnnihilation.EyeOfAnnihilationID).PickupObjectId, PickupObjectDatabase.GetById(EyeOfAnnihilation.EyeOfAnnihilationID).PickupObjectId, PickupObjectDatabase.GetById(EyeOfAnnihilation.EyeOfAnnihilationID).PickupObjectId, PickupObjectDatabase.GetById(EyeOfAnnihilation.EyeOfAnnihilationID).PickupObjectId};
            DroneHover.FireCooldown = .25f;
            DroneHover.FireDuration = 0;
            DroneHover.NumToTrigger = 6;
            DroneHover.Radius = 2.25f;
            DroneHover.RotationSpeed = 0f;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(255, 225, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 100);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.2f);
            DroneHover.Material_To_Use = mat;

        }
    }
}