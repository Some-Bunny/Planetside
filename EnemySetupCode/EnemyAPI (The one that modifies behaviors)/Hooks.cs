using MonoMod.RuntimeDetour;
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
                if (self.OverrideDisplayName != "#BOSSSTATUES_ENCNAME")
                {
                    var obehaviors = ToolsEnemy.overrideBehaviors.Where(ob => ob.OverrideAIActorGUID == self.EnemyGuid);
                    foreach (var obehavior in obehaviors)
                    {
                        obehavior.SetupOB(self);
                        if (obehavior.ShouldOverride() || (self.EnemyGuid == "05b8afe0b6cc4fffa9dc6036fa24c8ec" && ContainmentBreachController.CurrentState == ContainmentBreachController.States.ALLOWED))
                        {
                            obehavior.DoOverride();
                        }
                    }
                }
                else
                {
                    if (ContainmentBreachController.CurrentState == ContainmentBreachController.States.ALLOWED)
                    {
                        new KillPillarsChanges.KillPillarChanges().OverrideAllKillPillars(self);

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
