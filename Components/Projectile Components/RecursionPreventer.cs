using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;

namespace Planetside
{
    public class RecursionPreventer : MonoBehaviour
    {
        public RecursionPreventer()
        {
            IsProjectileFiredFromHoveringGun = true;
            IsProjectileFiredFromTeleportingGunfire = true;
            ChanceToNotPreventRecursion = 0;
        }
        public void Awake()
        {
            if (ChanceToNotPreventRecursion == 0) { return; }
            if (UnityEngine.Random.value < ChanceToNotPreventRecursion) { Destroy(this); }
        }
        public bool IsProjectileFiredFromHoveringGun;
        public bool IsProjectileFiredFromTeleportingGunfire;
        public float ChanceToNotPreventRecursion = 0;
    }
}
