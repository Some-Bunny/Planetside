﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planetside;
using UnityEngine;
using MonoMod.RuntimeDetour;
using Brave.BulletScript;
using DirectionType = DirectionalAnimation.DirectionType;
using FlipType = DirectionalAnimation.FlipType;
using FullInspector;

namespace ItemAPI
{
    public static class BossBuilder
    {
        private static GameObject behaviorSpeculatorPrefab;
        public static Dictionary<string, GameObject> Dictionary = new Dictionary<string, GameObject>();

        public static void Init()
        {
            var actor = EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c");
            behaviorSpeculatorPrefab = GameObject.Instantiate(actor.gameObject);

            foreach (Transform child in behaviorSpeculatorPrefab.transform)
            {
                if (child != behaviorSpeculatorPrefab.transform)
                    GameObject.DestroyImmediate(child);
            }

            foreach (var comp in behaviorSpeculatorPrefab.GetComponents<Component>())
            {
                if (comp.GetType() != typeof(BehaviorSpeculator))
                {
                    GameObject.DestroyImmediate(comp);
                }
            }

            GameObject.DontDestroyOnLoad(behaviorSpeculatorPrefab);
            FakePrefab.MarkAsFakePrefab(behaviorSpeculatorPrefab);
            behaviorSpeculatorPrefab.SetActive(false);

            Hook enemyHook = new Hook(
                typeof(EnemyDatabase).GetMethod("GetOrLoadByGuid", BindingFlags.Public | BindingFlags.Static),
                typeof(BossBuilder).GetMethod("GetOrLoadByGuid")
            );
        }

        public static AIActor GetOrLoadByGuid(Func<string, AIActor> orig, string guid)
        {
            foreach (var id in Dictionary.Keys)
            {
                if (id == guid)
                    return Dictionary[id].GetComponent<AIActor>();
            }

            return orig(guid);
        }


        public static GameObject BuildPrefabBundle(string name, string guid, tk2dSpriteCollectionData customCollection, int spriteID, IntVector2 hitboxOffset, IntVector2 hitBoxSize, bool HasAiShooter, bool UsesAttackGroup = false)
        {
            if (BossBuilder.Dictionary.ContainsKey(guid))
            {
                ETGModConsole.Log("BossBuilder: Yea something went wrong. Complain to Neighborino about it.");
                return null;
            }
            var prefab = GameObject.Instantiate(behaviorSpeculatorPrefab);
            prefab.name = name;

            //setup misc components

            var sprite = prefab.AddComponent<tk2dSprite>();


            /*
            for (int i = 0; i < customCollection.spriteDefinitions.Count() - 1; i++)
            {
                var c = customCollection.spriteDefinitions[i];


                // ETGModConsole.Log(c.GetBounds());
                c.boundsDataCenter = new Vector3(bounds.x / 2f, bounds.y / 2f, 0f);
                c.boundsDataExtents = new Vector3(bounds.x, bounds.y, 0f);
                c.untrimmedBoundsDataCenter = new Vector3(bounds.x / 2f, bounds.y / 2f, 0f);
                c.untrimmedBoundsDataExtents = new Vector3(bounds.x, bounds.y, 0f);

            }
            */

            sprite.Collection = customCollection;
            customCollection.InitDictionary();
            sprite.SetSprite(customCollection, spriteID);
            sprite.Build();


            sprite.SortingOrder = 0;
            sprite.IsPerpendicular = true;



            //var sprite = SpriteBuilder.SpriteFromResource(defaultSpritePath, prefab).GetComponent<tk2dSprite>();

            sprite.SetUpSpeculativeRigidbody(hitboxOffset, hitBoxSize).CollideWithOthers = true;
            prefab.AddComponent<tk2dSpriteAnimator>();
            prefab.AddComponent<AIAnimator>();
            prefab.GetOrAddComponent<ObjectVisibilityManager>();
            //setup knockback
            var knockback = prefab.AddComponent<KnockbackDoer>();
            knockback.weight = 1;




            //setup health haver
            var healthHaver = prefab.AddComponent<HealthHaver>();
            healthHaver.RegisterBodySprite(sprite);
            healthHaver.PreventAllDamage = false;
            healthHaver.SetHealthMaximum(15000);
            healthHaver.FullHeal();

            //setup AI Actor
            var aiActor = prefab.AddComponent<AIActor>();
            aiActor.State = AIActor.ActorState.Normal;
            aiActor.EnemyGuid = guid;
            aiActor.HasShadow = false;
            //setup behavior speculator
            var bs = prefab.GetComponent<BehaviorSpeculator>();

            bs.MovementBehaviors = new List<MovementBehaviorBase>();
            bs.TargetBehaviors = new List<TargetBehaviorBase>();
            bs.OverrideBehaviors = new List<OverrideBehaviorBase>();
            bs.OtherBehaviors = new List<BehaviorBase>();
            bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>();
            if (HasAiShooter)
            {

                var actor = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
                behaviorSpeculatorPrefab = GameObject.Instantiate(actor.gameObject);
                foreach (Transform child in behaviorSpeculatorPrefab.transform)
                {
                    if (child != behaviorSpeculatorPrefab.transform)
                        GameObject.DestroyImmediate(child);
                }

                foreach (var comp in behaviorSpeculatorPrefab.GetComponents<Component>())
                {
                    if (comp.GetType() != typeof(BehaviorSpeculator))
                    {
                        GameObject.DestroyImmediate(comp);
                    }
                }

                GameObject.DontDestroyOnLoad(behaviorSpeculatorPrefab);
                FakePrefab.MarkAsFakePrefab(behaviorSpeculatorPrefab);
                behaviorSpeculatorPrefab.SetActive(false);

            }
            else
            {
                AIBulletBank aibulletBank = prefab.AddComponent<AIBulletBank>();
                aibulletBank.Bullets = new List<AIBulletBank.Entry>();
            }

                        ((ISerializedObject)bs).SerializedObjectReferences = new List<UnityEngine.Object>
                        {

                        };
            ((ISerializedObject)bs).SerializedStateKeys = new List<string>
            {
                "OverrideBehaviors",
                "TargetBehaviors",
                "MovementBehaviors",
                "AttackBehaviors",
                "OtherBehaviors",
                "AttackBehaviorGroup"
            };
            ((ISerializedObject)bs).SerializedStateValues = new List<string>(0);
            bs._serializedStateKeys = new List<string>
            {
                "OverrideBehaviors",
                "TargetBehaviors",
                "MovementBehaviors",
                "AttackBehaviors",
                "OtherBehaviors",
                "AttackBehaviorGroup"
            };
            bs._serializedStateValues = new List<string>(0);

            //Add to enemy database
            EnemyDatabaseEntry enemyDatabaseEntry = new EnemyDatabaseEntry()
            {
                myGuid = guid,
                placeableWidth = 2,
                placeableHeight = 2,
                isNormalEnemy = true
            };

            EnemyDatabase.Instance.Entries.Add(enemyDatabaseEntry);
            BossBuilder.Dictionary.Add(guid, prefab);
            prefab.AddComponent<Tint>();


            //finalize
            GameObject.DontDestroyOnLoad(prefab);
            FakePrefab.MarkAsFakePrefab(prefab);
            prefab.SetActive(false);

            return prefab;
        }


        public static GameObject BuildPrefab(string name, string guid, string defaultSpritePath, IntVector2 hitboxOffset, IntVector2 hitBoxSize, bool HasAiShooter, bool UsesAttackGroup = false)
        {
            if (BossBuilder.Dictionary.ContainsKey(guid))
            {
                ETGModConsole.Log("BossBuilder: Yea something went wrong. Complain to Neighborino about it.");
                return null;
            }
            var prefab = GameObject.Instantiate(behaviorSpeculatorPrefab);
            prefab.name = name;

            //setup misc components
            var sprite = SpriteBuilder.SpriteFromResource(defaultSpritePath, prefab).GetComponent<tk2dSprite>();

            sprite.SetUpSpeculativeRigidbody(hitboxOffset, hitBoxSize).CollideWithOthers = true;
            prefab.AddComponent<tk2dSpriteAnimator>();
            prefab.AddComponent<AIAnimator>();
            prefab.GetOrAddComponent<ObjectVisibilityManager>();
            //setup knockback
            var knockback = prefab.AddComponent<KnockbackDoer>();
            knockback.weight = 1;




            //setup health haver
            var healthHaver = prefab.AddComponent<HealthHaver>();
            healthHaver.RegisterBodySprite(sprite);
            healthHaver.PreventAllDamage = false;
            healthHaver.SetHealthMaximum(15000);
            healthHaver.FullHeal();

            //setup AI Actor
            var aiActor = prefab.AddComponent<AIActor>();
            aiActor.State = AIActor.ActorState.Normal;
            aiActor.EnemyGuid = guid;
            aiActor.HasShadow = false;
            //setup behavior speculator
            var bs = prefab.GetComponent<BehaviorSpeculator>();

            bs.MovementBehaviors = new List<MovementBehaviorBase>();
            bs.TargetBehaviors = new List<TargetBehaviorBase>();
            bs.OverrideBehaviors = new List<OverrideBehaviorBase>();
            bs.OtherBehaviors = new List<BehaviorBase>();
            bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>();
            if (HasAiShooter)
            {

                var actor = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
                behaviorSpeculatorPrefab = GameObject.Instantiate(actor.gameObject);
                foreach (Transform child in behaviorSpeculatorPrefab.transform)
                {
                    if (child != behaviorSpeculatorPrefab.transform)
                        GameObject.DestroyImmediate(child);
                }

                foreach (var comp in behaviorSpeculatorPrefab.GetComponents<Component>())
                {
                    if (comp.GetType() != typeof(BehaviorSpeculator))
                    {
                        GameObject.DestroyImmediate(comp);
                    }
                }

                GameObject.DontDestroyOnLoad(behaviorSpeculatorPrefab);
                FakePrefab.MarkAsFakePrefab(behaviorSpeculatorPrefab);
                behaviorSpeculatorPrefab.SetActive(false);

            }
            else
            {
                AIBulletBank aibulletBank = prefab.AddComponent<AIBulletBank>();
                aibulletBank.Bullets = new List<AIBulletBank.Entry>();
            }

            //Add to enemy database
            EnemyDatabaseEntry enemyDatabaseEntry = new EnemyDatabaseEntry()
            {
                myGuid = guid,
                placeableWidth = 2,
                placeableHeight = 2,
                isNormalEnemy = true
            };

            EnemyDatabase.Instance.Entries.Add(enemyDatabaseEntry);
            BossBuilder.Dictionary.Add(guid, prefab);
            prefab.AddComponent<Tint>();


            //finalize
            GameObject.DontDestroyOnLoad(prefab);
            FakePrefab.MarkAsFakePrefab(prefab);
            prefab.SetActive(false);

            return prefab;
        }

        public enum AnimationType { Move, Idle, Fidget, Flight, Hit, Talk, Other }
        public static tk2dSpriteAnimationClip AddAnimation(this GameObject obj, string name, string spriteDirectory, int fps,
            AnimationType type, DirectionType directionType = DirectionType.None, FlipType flipType = FlipType.None)
        {
            AIAnimator aiAnimator = obj.GetOrAddComponent<AIAnimator>();
            DirectionalAnimation animation = aiAnimator.GetDirectionalAnimation(name, directionType, type);
            if (animation == null)
            {
                animation = new DirectionalAnimation()
                {
                    AnimNames = new string[0],
                    Flipped = new FlipType[0],
                    Type = directionType,
                    Prefix = string.Empty
                };
            }

            animation.AnimNames = animation.AnimNames.Concat(new string[] { name }).ToArray();
            animation.Flipped = animation.Flipped.Concat(new FlipType[] { flipType }).ToArray();
            aiAnimator.AssignDirectionalAnimation(name, animation, type);
            return BuildAnimation(aiAnimator, name, spriteDirectory, fps);
        }

        public static tk2dSpriteAnimationClip BuildAnimation(AIAnimator aiAnimator, string name, string spriteDirectory, int fps)
        {
            tk2dSpriteCollectionData collection = aiAnimator.GetComponent<tk2dSpriteCollectionData>();
            if (!collection)
                collection = SpriteBuilder.ConstructCollection(aiAnimator.gameObject, $"{aiAnimator.name}_collection");

            string[] resources = ResourceExtractor.GetResourceNames();
            List<int> indices = new List<int>();
            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i].StartsWith(spriteDirectory.Replace('/', '.'), StringComparison.OrdinalIgnoreCase))
                {
                    indices.Add(SpriteBuilder.AddSpriteToCollection(resources[i], collection));
                }
            }
            tk2dSpriteAnimationClip clip = SpriteBuilder.AddAnimation(aiAnimator.spriteAnimator, collection, indices, name, tk2dSpriteAnimationClip.WrapMode.Once);
            clip.fps = fps;
            return clip;
        }

        public static DirectionalAnimation GetDirectionalAnimation(this AIAnimator aiAnimator, string name, DirectionType directionType, AnimationType type)
        {
            DirectionalAnimation result = null;
            switch (type)
            {
                case AnimationType.Idle:
                    result = aiAnimator.IdleAnimation;
                    break;
                case AnimationType.Move:
                    result = aiAnimator.MoveAnimation;
                    break;
                case AnimationType.Flight:
                    result = aiAnimator.FlightAnimation;
                    break;
                case AnimationType.Hit:
                    result = aiAnimator.HitAnimation;
                    break;
                case AnimationType.Talk:
                    result = aiAnimator.TalkAnimation;
                    break;
            }
            if (result != null)
                return result;

            return null;
        }

        public static void AssignDirectionalAnimation(this AIAnimator aiAnimator, string name, DirectionalAnimation animation, AnimationType type)
        {
            switch (type)
            {
                case AnimationType.Idle:
                    aiAnimator.IdleAnimation = animation;
                    break;
                case AnimationType.Move:
                    aiAnimator.MoveAnimation = animation;
                    break;
                case AnimationType.Flight:
                    aiAnimator.FlightAnimation = animation;
                    break;
                case AnimationType.Hit:
                    aiAnimator.HitAnimation = animation;
                    break;
                case AnimationType.Talk:
                    aiAnimator.TalkAnimation = animation;
                    break;
                case AnimationType.Fidget:
                    aiAnimator.IdleFidgetAnimations.Add(animation);
                    break;
                default:
                    aiAnimator.OtherAnimations.Add(new AIAnimator.NamedDirectionalAnimation()
                    {
                        anim = animation,
                        name = name
                    });
                    break;
            }
        }

        public static void DuplicateAIShooterAndAIBulletBank(GameObject targetObject, AIShooter sourceShooter, AIBulletBank sourceBulletBank, int startingGunOverrideID = 0, Transform gunAttachPointOverride = null, Transform bulletScriptAttachPointOverride = null, PlayerHandController overrideHandObject = null)
        {
            if (targetObject.GetComponent<AIShooter>() && targetObject.GetComponent<AIBulletBank>())
            {
                return;
            }
            if (!targetObject.GetComponent<AIBulletBank>())
            {
                AIBulletBank aibulletBank = targetObject.AddComponent<AIBulletBank>();
                aibulletBank.Bullets = new List<AIBulletBank.Entry>(0);
                if (sourceBulletBank.Bullets.Count > 0)
                {
                    foreach (AIBulletBank.Entry entry in sourceBulletBank.Bullets)
                    {
                        aibulletBank.Bullets.Add(new AIBulletBank.Entry
                        {
                            Name = entry.Name,
                            BulletObject = entry.BulletObject,
                            OverrideProjectile = entry.OverrideProjectile,
                            ProjectileData = new ProjectileData
                            {
                                damage = entry.ProjectileData.damage,
                                speed = entry.ProjectileData.speed,
                                range = entry.ProjectileData.range,
                                force = entry.ProjectileData.force,
                                damping = entry.ProjectileData.damping,
                                UsesCustomAccelerationCurve = entry.ProjectileData.UsesCustomAccelerationCurve,
                                AccelerationCurve = entry.ProjectileData.AccelerationCurve,
                                CustomAccelerationCurveDuration = entry.ProjectileData.CustomAccelerationCurveDuration,
                                onDestroyBulletScript = entry.ProjectileData.onDestroyBulletScript,
                                IgnoreAccelCurveTime = entry.ProjectileData.IgnoreAccelCurveTime
                            },
                            PlayAudio = entry.PlayAudio,
                            AudioSwitch = entry.AudioSwitch,
                            AudioEvent = entry.AudioEvent,
                            AudioLimitOncePerFrame = entry.AudioLimitOncePerFrame,
                            AudioLimitOncePerAttack = entry.AudioLimitOncePerAttack,
                            MuzzleFlashEffects = new VFXPool
                            {
                                effects = entry.MuzzleFlashEffects.effects,
                                type = entry.MuzzleFlashEffects.type
                            },
                            MuzzleLimitOncePerFrame = entry.MuzzleLimitOncePerFrame,
                            MuzzleInheritsTransformDirection = entry.MuzzleInheritsTransformDirection,
                            ShellTransform = entry.ShellTransform,
                            ShellPrefab = entry.ShellPrefab,
                            ShellForce = entry.ShellForce,
                            ShellForceVariance = entry.ShellForceVariance,
                            DontRotateShell = entry.DontRotateShell,
                            ShellGroundOffset = entry.ShellGroundOffset,
                            ShellsLimitOncePerFrame = entry.ShellsLimitOncePerFrame,
                            rampBullets = entry.rampBullets,
                            conditionalMinDegFromNorth = entry.conditionalMinDegFromNorth,
                            forceCanHitEnemies = entry.forceCanHitEnemies,
                            suppressHitEffectsIfOffscreen = entry.suppressHitEffectsIfOffscreen,
                            preloadCount = entry.preloadCount
                        });
                    }
                }
                aibulletBank.useDefaultBulletIfMissing = true;
                aibulletBank.transforms = new List<Transform>();
                if (sourceBulletBank.transforms != null && sourceBulletBank.transforms.Count > 0)
                {
                    foreach (Transform item in sourceBulletBank.transforms)
                    {
                        aibulletBank.transforms.Add(item);
                    }
                }
                aibulletBank.RegenerateCache();
            }
            if (!targetObject.GetComponent<AIShooter>())
            {
                AIShooter aishooter = targetObject.AddComponent<AIShooter>();
                aishooter.volley = sourceShooter.volley;
                if (startingGunOverrideID != 0)
                {
                    aishooter.equippedGunId = startingGunOverrideID;
                }
                else
                {
                    aishooter.equippedGunId = sourceShooter.equippedGunId;
                }
                aishooter.shouldUseGunReload = true;
                aishooter.volleyShootPosition = sourceShooter.volleyShootPosition;
                aishooter.volleyShellCasing = sourceShooter.volleyShellCasing;
                aishooter.volleyShellTransform = sourceShooter.volleyShellTransform;
                aishooter.volleyShootVfx = sourceShooter.volleyShootVfx;
                aishooter.usesOctantShootVFX = sourceShooter.usesOctantShootVFX;
                aishooter.bulletName = sourceShooter.bulletName;
                aishooter.customShootCooldownPeriod = sourceShooter.customShootCooldownPeriod;
                aishooter.doesScreenShake = sourceShooter.doesScreenShake;
                aishooter.rampBullets = sourceShooter.rampBullets;
                aishooter.rampStartHeight = sourceShooter.rampStartHeight;
                aishooter.rampTime = sourceShooter.rampTime;
                if (gunAttachPointOverride)
                {
                    aishooter.gunAttachPoint = gunAttachPointOverride;
                }
                else
                {
                    aishooter.gunAttachPoint = sourceShooter.gunAttachPoint;
                }
                if (bulletScriptAttachPointOverride)
                {
                    aishooter.bulletScriptAttachPoint = bulletScriptAttachPointOverride;
                }
                else
                {
                    aishooter.bulletScriptAttachPoint = sourceShooter.bulletScriptAttachPoint;
                }
                aishooter.overallGunAttachOffset = sourceShooter.overallGunAttachOffset;
                aishooter.flippedGunAttachOffset = sourceShooter.flippedGunAttachOffset;
                if (overrideHandObject)
                {
                    aishooter.handObject = overrideHandObject;
                }
                else
                {
                    aishooter.handObject = sourceShooter.handObject;
                }
                aishooter.AllowTwoHands = sourceShooter.AllowTwoHands;
                aishooter.ForceGunOnTop = sourceShooter.ForceGunOnTop;
                aishooter.IsReallyBigBoy = sourceShooter.IsReallyBigBoy;
                aishooter.BackupAimInMoveDirection = sourceShooter.BackupAimInMoveDirection;
                aishooter.RegenerateCache();
            }
        }
        public class Tint : BraveBehaviour
        {
            private void Start()
            {
                OtherTools.DisableSuperTinting(base.aiActor);
            }
        }
    }
}


