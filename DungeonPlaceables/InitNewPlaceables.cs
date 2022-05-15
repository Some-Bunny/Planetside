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
        }
    }
}
