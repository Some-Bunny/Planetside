using HarmonyLib;
using System.Reflection.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MonoMod.Cil;

namespace Planetside.Hooks_And_Actions
{
    public class Patches
    {
        #region Glass DPS Cap Leniency




        //idk how to do this and its so fucking hot and ive got other stuff to do aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
        /*
        [HarmonyPatch(typeof(HealthHaver), nameof(HealthHaver.ApplyDamageDirectional))]
        public static class HealthHaver_ApplyDamageDirectional
        {
            [HarmonyILManipulator]
            private static void Do_HealthHaver_ApplyDamageDirectional(ILContext il)
            {
                ILCursor cursor = new ILCursor(il);
                if (!cursor.TryGotoNext(MoveType.Before,
                  instr => instr.MatchLdfld("float32", "m_damageCap"),
                  instr => instr.MatchCall<UnityEngine.Mathf>("Min")))
                {
                    return;
                }
                ILLabel startPoint = cursor.MarkLabel(); // mark our own label right before the call to CleanupLists(), which is at the end of the conditional we want to skip
                cursor.Index = 0;

                if (!cursor.TryGotoNext(MoveType.Before,
                    instr => instr.MatchLdfld("float32", "m_damageCap"),
                    instr => instr.MatchCall<UnityEngine.Mathf>("Min")))
                {
                    return;
                }

            }

            public static void OverrideCheck(HealthHaver Self)
            {

            }

        }
        */
        #endregion

        #region Dissuade Rat Theft in the Deep
        [HarmonyPatch(typeof(PickupObject), nameof(PickupObject.ShouldBeTakenByRat))]
        public class Patch_PickupObject_ShouldBeTakenByRatSpawnProjModifier
        {
            [HarmonyPrefix]
            private static bool Awake(PickupObject __instance, ref bool __result)
            {
                if (GameManager.Instance.Dungeon && GameManager.Instance.Dungeon.DungeonFloorName == "The Deep.")
                {
                    __result = false;
                    return false; 
                }
                return true;
            }
        }
        #endregion


        #region SpawnProjModifier Recursion Preventers
        [HarmonyPatch(typeof(SpawnProjModifier), nameof(SpawnProjModifier.SpawnProjectile))]
        public class Patch_SpawnProjModifier_SpawnProjectile
        {
            [HarmonyPrefix]
            private static bool Awake(SpawnProjModifier __instance, Projectile proj, Vector3 spawnPosition, float zRotation, SpeculativeRigidbody collidedRigidbody = null)
            {
                GameObject gameObject = SpawnManager.SpawnProjectile(proj.gameObject, spawnPosition, Quaternion.Euler(0f, 0f, zRotation), true);
                
                gameObject.AddComponent<RecursionPreventer>();
                
                Projectile component = gameObject.GetComponent<Projectile>();
                if (component)
                {
                    component.SpawnedFromOtherPlayerProjectile = true;
                    if (component is HelixProjectile)
                    {
                        component.Inverted = (UnityEngine.Random.value < 0.5f);
                    }
                }
                if (!__instance.m_hasCheckedProjectile)
                {
                    __instance.m_hasCheckedProjectile = true;
                    __instance.m_projectile = __instance.GetComponent<Projectile>();
                }
                if (__instance.m_projectile && __instance.PostprocessSpawnedProjectiles && __instance.m_projectile.Owner && __instance.m_projectile.Owner is PlayerController)
                {
                    PlayerController playerController = __instance.m_projectile.Owner as PlayerController;
                    playerController.DoPostProcessProjectile(component);
                }
                if (__instance.SpawnedProjectilesInheritAppearance && component.sprite && __instance.m_projectile.sprite)
                {
                    component.shouldRotate = __instance.m_projectile.shouldRotate;
                    component.shouldFlipHorizontally = __instance.m_projectile.shouldFlipHorizontally;
                    component.shouldFlipVertically = __instance.m_projectile.shouldFlipVertically;
                    component.sprite.SetSprite(__instance.m_projectile.sprite.Collection, __instance.m_projectile.sprite.spriteId);
                    Vector2 vector = component.transform.position.XY() - component.sprite.WorldCenter;
                    component.transform.position += vector.ToVector3ZUp(0f);
                    component.specRigidbody.Reinitialize();
                }
                if (__instance.SpawnedProjectileScaleModifier != 1f)
                {
                    component.AdditionalScaleMultiplier *= __instance.SpawnedProjectileScaleModifier;
                }
                if (__instance.m_projectile && __instance.m_projectile.GetCachedBaseDamage > 0f)
                {
                    component.baseData.damage = component.baseData.damage * Mathf.Min(__instance.m_projectile.baseData.damage / __instance.m_projectile.GetCachedBaseDamage, 1f);
                }
                if (__instance.p)
                {
                    component.Owner = __instance.p.Owner;
                    component.Shooter = __instance.p.Shooter;
                    if (component is RobotechProjectile)
                    {
                        RobotechProjectile robotechProjectile = component as RobotechProjectile;
                        robotechProjectile.initialOverrideTargetPoint = new Vector2?(spawnPosition.XY() + (Quaternion.Euler(0f, 0f, zRotation) * Vector2.right * 10f).XY());
                    }
                    if (__instance.SpawnedProjectilesInheritData)
                    {
                        component.baseData.damage = Mathf.Max(component.baseData.damage, __instance.p.baseData.damage / (float)__instance.numberToSpawnOnCollison);
                        component.baseData.speed = Mathf.Max(component.baseData.speed, __instance.p.baseData.speed / ((float)__instance.numberToSpawnOnCollison / 2f));
                        component.baseData.force = Mathf.Max(component.baseData.force, __instance.p.baseData.force / (float)__instance.numberToSpawnOnCollison);
                    }
                }
                if (component.specRigidbody)
                {
                    if (collidedRigidbody)
                    {
                        component.specRigidbody.RegisterTemporaryCollisionException(collidedRigidbody, 0.25f, new float?(0.5f));
                    }
                    PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(component.specRigidbody, null, false);
                }
                return false;
            }
        }
        #endregion




        #region Umbral Jammomancer Patches

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
        #endregion
    }
}
