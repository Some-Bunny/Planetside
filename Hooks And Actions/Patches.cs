using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside.Hooks_And_Actions
{
    public class Patches
    {

        [HarmonyPatch(typeof(BuffEnemiesBehavior), nameof(BuffEnemiesBehavior.BuffEnemy))]
        public class Patch_BuffEnemy_Class
        {
            [HarmonyPrefix]
            private static bool Awake(BuffEnemiesBehavior __instance, AIActor enemy)
            {
                if (!enemy)
                {
                    return false;
                }
                if (__instance.JamEnemies)
                {
                    if (enemy.specRigidbody)
                    {
                        if (enemy.IsSignatureEnemy)
                        {
                            enemy.PlaySmallExplosionsStyleEffect(__instance.LargeJamEffect, 8, 0.025f);
                        }
                        else
                        {
                            enemy.PlayEffectOnActor(__instance.SmallJamEffect, Vector3.zero, true, false, true);
                        }
                    }
                    if (enemy.IsBlackPhantom == true)
                    {
                        enemy.gameObject.GetOrAddComponent<UmbraController>();
                    }
                    else if (enemy.IsBlackPhantom == false)
                    {
                        enemy.BecomeBlackPhantom();
                    }


                }
                if (__instance.UsesBuffEffect)
                {
                    enemy.ApplyEffect(__instance.buffEffect, 1f, null);
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(BuffEnemiesBehavior), nameof(BuffEnemiesBehavior.UnbuffEnemy))]
        public class Patch_UnbuffEnemy_Class
        {
            [HarmonyPrefix]
            private static bool Awake(BuffEnemiesBehavior __instance, AIActor enemy)
            {
                if (!enemy)
                {
                    return false;
                }
                if (__instance.JamEnemies)
                {
                    if (enemy.GetComponent<UmbraController>() != null)
                    {
                        enemy.GetComponent<UmbraController>().UnBecomeUmbra();
                    }
                    else
                    {
                        enemy.UnbecomeBlackPhantom();
                    }
                }
                if (__instance.UsesBuffEffect)
                {
                    enemy.RemoveEffect(__instance.buffEffect);
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(BuffEnemiesBehavior), nameof(BuffEnemiesBehavior.IsGoodBuffTarget))]
        public class Patch_IsGoodBuffTarget_Class
        {
            [HarmonyPostfix]
            private static void IsGoodBuffTarget(BuffEnemiesBehavior __instance, AIActor enemy, ref bool __result)
            {
                __result = enemy && !enemy.IsBuffEnemy && !enemy.IsHarmlessEnemy && (!enemy.healthHaver || (enemy.healthHaver.IsVulnerable && !enemy.healthHaver.PreventAllDamage));
            }
        }
    }
}
