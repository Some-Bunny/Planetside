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
                if (ContainmentBreachController.CurrentState == ContainmentBreachController.States.ALLOWED && SaveAPIManager.GetFlag(CustomDungeonFlags.HAS_TREADED_DEEPER) == true)
                {
                    if (self.OverrideDisplayName == "#BOSSSTATUES_ENCNAME")
                    {
                        new KillPillarsChanges.KillPillarChanges().OverrideAllKillPillars(self);
                    }
                    else
                    {
                        var obehaviors = ToolsEnemy.overrideBehaviors.Where(ob => ob.OverrideAIActorGUID == self.EnemyGuid);
                        foreach (var obehavior in obehaviors)
                        {
                            obehavior.SetupOB(self);
                            if (obehavior.ShouldOverride() || self.EnemyGuid == "05b8afe0b6cc4fffa9dc6036fa24c8ec")
                            {
                                obehavior.DoOverride();
                            }
                        }
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
