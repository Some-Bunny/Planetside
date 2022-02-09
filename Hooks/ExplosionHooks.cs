using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Dungeonator;
using SaveAPI;
using Gungeon;
using ItemAPI;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using System.Collections;
using Brave.BulletScript;

namespace Planetside
{
    public static class ExplosionHooks
    {
        public static void Init()
        {
            new Hook(typeof(Exploder).GetMethod("Explode", BindingFlags.Static | BindingFlags.Public),typeof(ExplosionHooks).GetMethod("ExplosionHook", BindingFlags.Static | BindingFlags.Public));
        }
        public static void ExplosionHook(Action<Vector3, ExplosionData, Vector2, Action, bool, CoreDamageTypes, bool> orig, Vector3 position, ExplosionData data, Vector2 sourceNormal, Action onExplosionBegin = null, bool ignoreQueues = false, CoreDamageTypes damageTypes = CoreDamageTypes.None, bool ignoreDamageCaps = false)
        {
            orig(position, data, sourceNormal, onExplosionBegin, ignoreQueues, damageTypes, ignoreDamageCaps);
            try
            {
                PlayerController[] players = GameManager.Instance.AllPlayers;
                for (int i = 0; i < players.Length; i++)
                {
                    PlayerController player = players[i];
                    if (player.GetComponent<BlastProjectilesCheck>() != null)
                    {
                        BlastProjectilesCheck blast = player.GetComponent<BlastProjectilesCheck>();
                        float RNG = UnityEngine.Random.Range(-180, 180);
                        int projectileToSpawn = blast.MinToSpawn + (int)(data.damage / 8);
                        if (projectileToSpawn >= blast.Cap) { projectileToSpawn = blast.Cap; }
                        for (int e = 0; e < projectileToSpawn; e++)
                        {
                            float H = 360 / projectileToSpawn;
                            float finaldir = RNG + (H*e);
                            GameObject prefab = BlastProjectiles.VengefulProjectile.gameObject;
                            GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(prefab, position, Quaternion.Euler(0f, 0f,  finaldir), true);
                            Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                            if (component != null)
                            {
                                component.baseData.speed = 12;
                                component.Owner = player;
                                component.Shooter = player.specRigidbody;
                                SpriteOutlineManager.AddOutlineToSprite(component.sprite, Color.black);
                                HomingModifier HomingMod = component.gameObject.GetOrAddComponent<HomingModifier>();
                                HomingMod.AngularVelocity = 300;
                                HomingMod.HomingRadius = 20;
                                PierceProjModifier spook = component.gameObject.GetOrAddComponent<PierceProjModifier>();
                                spook.penetration = 2;
                                spook.penetratesBreakables = true;
                            }
                        }
                    }
                }
             
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }
        public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }
}
