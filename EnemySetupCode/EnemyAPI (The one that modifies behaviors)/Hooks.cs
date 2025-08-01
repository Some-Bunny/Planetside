using Alexandria;
using MonoMod.RuntimeDetour;
using SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Planetside
{
    public static class EnemyHooks
    {
        public static void Init()
        {
            
            Hook customEnemyChangesHook = new Hook(
                typeof(AIActor).GetMethod("Awake", BindingFlags.Instance | BindingFlags.Public),
                typeof(EnemyHooks).GetMethod("HandleCustomEnemyChanges")
            );
            
        }

        public static void HandleCustomEnemyChanges(Action<AIActor> orig, AIActor self)
        {
            orig(self);
            
            try
            {
                if (self.OverrideDisplayName == "#BOSSSTATUES_ENCNAME")
                {
                    new KillPillarsChanges.KillPillarChanges().OverrideAllKillPillars(self);
                    return;
                }
                var obehaviors = ToolsEnemy.overrideBehaviors.Where(ob => ob.OverrideAIActorGUID == self.EnemyGuid);
                foreach (var obehavior in obehaviors)
                {
                    obehavior.SetupOB(self);
                    if (obehavior.ShouldOverride())
                    {
                        obehavior.DoOverride();
                    }
                }
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
            }
        }
    }
}
