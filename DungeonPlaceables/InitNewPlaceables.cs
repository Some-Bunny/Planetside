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
            Targets.Init();
            HolyChamberStatue.Init();
            TrespassPortalBack.Init();
        }
    }
}
