using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planetside
{
    public class InitNewPlaceables
    {
        public static void InitPlaceables()
        {

            EmberPot.Init();
            TrespassLight.Init();
            TrespassLightBig.Init();
            TrespassPot.Init();
            Targets.Init();
            HolyChamberStatue.Init();
            TrespassPortalBack.Init();
            TrespassFadingBlocker.Init();
            TutorialNotes.Init();
            BoxOfGrenades.Init();
            TrespassContainer.Init();
            TrespassDecals.Init();
            TrespassCandle.Init();
            TrespassDecorativePillar.Init();
            HMPrimeBattery.Init();
            Deturrets.Init();
            SniperTurrets.Init();
            AbyssFloorDoor.Init();
            TrespassSmallCubes.Init();
            TrespassCubes.Init();
            TrespassSniperTurrets.Init();
            TrespassBloodDecals.Init();
            TrespassTentaclePillar.Init();
            TearInReality.Init();
            MovingTile1X1.Init();
            MovingTile2X2.Init();
            //TrespassTrollRock.Init();
        }
    }
}
