using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside
{
    public class StaticInformation 
    {
        public static void Init()
        {
            ModderBulletGUIDs = new List<string>();
        }
        public static List<string> ModderBulletGUIDs;
    }
}
