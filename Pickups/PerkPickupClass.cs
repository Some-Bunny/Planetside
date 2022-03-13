using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planetside
{
    public class PerkPickupObject : PickupObject
    {
        public override void Pickup(PlayerController player)
        {
        }
        public new bool PrerequisitesMet()
        {
            EncounterTrackable component = base.GetComponent<EncounterTrackable>();
            return component == null || component.PrerequisitesMet();
        }
    }
}
