using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



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
            DroneHover.AimType = HoveringGunController.AimType.PLAYER_AIM;
            DroneHover.PositionType = HoveringGunController.HoverPosition.CIRCULATE;
            DroneHover.FireType = HoveringGunController.FireType.ON_FIRED_GUN;
            DroneHover.UsesMultipleGuns = true;
            DroneHover.TargetGunIDs = new List<int> { PickupObjectDatabase.GetById(EyeOfAnnihilation.EyeOfAnnihilationID).PickupObjectId, PickupObjectDatabase.GetById(EyeOfAnnihilation.EyeOfAnnihilationID).PickupObjectId, PickupObjectDatabase.GetById(EyeOfAnnihilation.EyeOfAnnihilationID).PickupObjectId, PickupObjectDatabase.GetById(EyeOfAnnihilation.EyeOfAnnihilationID).PickupObjectId, PickupObjectDatabase.GetById(EyeOfAnnihilation.EyeOfAnnihilationID).PickupObjectId, PickupObjectDatabase.GetById(EyeOfAnnihilation.EyeOfAnnihilationID).PickupObjectId};
            DroneHover.FireCooldown = .25f;
            DroneHover.FireDuration = 0;
            DroneHover.NumToTrigger = 6;
        }
    }
}