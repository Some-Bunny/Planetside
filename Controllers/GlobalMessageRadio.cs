using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Collections;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;
using GungeonAPI;
using static Planetside.MultiActiveReloadManager;

namespace Planetside
{
    public static class GlobalMessageRadio
    {
        public static void BroadcastMessage(string message)
        {
            if (OnMessageRecieved != null) { OnMessageRecieved(message); }
        }
        public static Action<string> OnMessageRecieved;        
    }
}
