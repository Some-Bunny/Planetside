using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.TK2DROOT.tk2dUI.Editor
{
    [Serializable]
    public class AttachPointData
    {
        public AttachPointData(tk2dSpriteDefinition.AttachPoint[] bcs)
        {
            this.attachPoints = bcs;
        }
        public tk2dSpriteDefinition.AttachPoint[] attachPoints;
    }
}
