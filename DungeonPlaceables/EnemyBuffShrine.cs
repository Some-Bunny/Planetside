using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dungeonator;
using ItemAPI;
using UnityEngine;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Planetside;
using BreakAbleAPI;
using Brave.BulletScript;
using System.Collections;
using static Planetside.Inquisitor;
using UnityEngine.Playables;
using Alexandria.PrefabAPI;
using static Planetside.EnemyBuffShrineController;


namespace Planetside
{
    public class EnemyBuffShrineController : BraveBehaviour
    {
        private class TripleTag : BraveBehaviour
        {
            public List<EffectType> effectTypes = new List<EffectType>();

            
        }


        public Transform GemTransform;

        public void Start()
        {
            if (this.majorBreakable)
            {
                this.majorBreakable.InvulnerableToEnemyBullets = true;
                currentRoom = this.majorBreakable.transform.position.GetAbsoluteRoom();
                if (gemObjectToSpawn)
                {
                    gem = (GameObject)UnityEngine.Object.Instantiate(gemObjectToSpawn, GemTransform.position, Quaternion.identity);
                    tk2dBaseSprite component = gem.GetComponent<tk2dBaseSprite>();
                    component.HeightOffGround = 35f;
                    component.UpdateZDepth();
                    component.SortingOrder = 24;
                }
                this.majorBreakable.OnDamaged = (obj) =>{
                    AkSoundEngine.PostEvent("Play_OBJ_rock_break_01", this.majorBreakable.gameObject);
                };
                this.majorBreakable.OnBreak = () => {
                    AkSoundEngine.PostEvent("Play_OBJ_rock_break_01", this.majorBreakable.gameObject);
                    AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", this.majorBreakable.gameObject);

                    //GameManager.Instance.StartCoroutine(ShrineParticlesOnDestory(this.majorBreakable.sprite.WorldBottomLeft, this.majorBreakable.sprite.WorldTopRight));
                    GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                    GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, gem.GetComponent<tk2dBaseSprite>().sprite.WorldCenter, Quaternion.identity);
                    blankObj.transform.localScale *= 1.33f;
                    Destroy(blankObj, 2f);

                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = this.GemTransform.position,
                        startColor = ColorTouse().WithAlpha(0.333f),
                        startLifetime = 0.5f,
                        startSize = 12
                    });


                    DebrisObject orAddComponent = gem.gameObject.GetOrAddComponent<DebrisObject>();
                    orAddComponent.angularVelocity = UnityEngine.Random.Range(70, 120);
                    orAddComponent.angularVelocityVariance = UnityEngine.Random.Range(10, 60);
                    orAddComponent.decayOnBounce = 0.5f;
                    orAddComponent.bounceCount = 1;
                    orAddComponent.canRotate = true;
                    orAddComponent.shouldUseSRBMotion = true;
                    orAddComponent.AssignFinalWorldDepth(-0.5f);
                    orAddComponent.sprite = gem.GetComponent<tk2dBaseSprite>();
                    orAddComponent.animatePitFall = true;
                    orAddComponent.Trigger(MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(0.5f, 2f)), 3.5f, 1f);
                    orAddComponent.audioEventName = "Play_OBJ_glass_shatter_01";
                    foreach (var entry in AllEnemies)
                    {
                        if (entry.Key != null)
                        {
                            entry.Key.healthHaver.OnDeath -= entry.Value;
                        }
                    }
                };
            }
        }


        private IEnumerator ShrineParticlesOnDestory(Vector2 BottmLeft, Vector2 TopRight)
        {
            Vector2 lol = TopRight.RoundToInt();
            float yes = lol.x.RoundToNearest(1);
            for (int j = 0; j < 5; j++)
            {
                Vector2 pos = BraveUtility.RandomVector2(BottmLeft, TopRight, new Vector2(0.025f, 0.025f));
                LootEngine.DoDefaultItemPoof(pos, false, true);
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.01f, 0.03f));
            }
            yield break;
        }

        private Dictionary<AIActor, Action<Vector2>> AllEnemies = new Dictionary<AIActor, Action<Vector2>>();

        public void Update()
        {
            if (this && currentRoom != null)
            {
                var aiactorList = currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (aiactorList != null)
                {
                    if (aiactorList == null | aiactorList.Count <= 0) { return; }
                    for (int i = 0; i < aiactorList.Count; i++)
                    {
                        AIActor enemy = aiactorList[i];
                        if (enemy)
                        {
                            if (enemy.EnemyGuid == FodderEnemy.guid) { continue; }
                            if (!AllEnemies.ContainsKey(enemy))
                            {
                                var tag = enemy.GetComponent<TripleTag>();
                                if (tag == null) { tag = enemy.gameObject.AddComponent<TripleTag>(); }

                                if (tag.effectTypes.Contains(this.ownType)) { goto Skip; }

                                tag.effectTypes.Add(this.ownType);

                                Action<Vector2> action = (_) =>
                                {
                                    AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Capture_01", enemy.gameObject);
                                    var am = 1;

                                    var __ = enemy.gameObject.GetComponent<TripleTag>();
                                    if (__ != null)
                                    {
                                        am = __.effectTypes.Count;
                                    }
                                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                                    {
                                        position = enemy.sprite.WorldCenter,
                                        startColor = ColorTouse().WithAlpha(0.75f),
                                        startLifetime = 0.5f,
                                        startSize = 6
                                    });


                                    if (am == 1)
                                    {
                                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopToSpawn()).TimedAddGoopCircle(enemy.sprite.WorldBottomCenter, 2.5f, 0.5f);
                                    }
                                    else
                                    {
                                        for (int a = 0; a < am; a++)
                                        {
                                            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopToSpawn(__.effectTypes[a])).TimedAddGoopCircle(enemy.sprite.WorldBottomCenter, 2.5f - (0.625f * a), 0.5f);
                                            if (__.effectTypes[a] == EffectType.ICE)
                                            {
                                                DeadlyDeadlyGoopManager.FreezeGoopsCircle(enemy.sprite.WorldBottomCenter, 4);
                                            }
                                        }
                                    }

                                    var amount = Vector2.Distance(this.GemTransform.position, enemy.sprite.WorldCenter) * 3;
                                    var m = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 1).normalized * 5;


                                    for (int v = 0; v < amount; v++)
                                    {
                                        float t = v / amount;
                                        ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                                        {
                                            position = Vector3.Lerp(this.GemTransform.position, enemy.sprite.WorldCenter, t) + (m.ToVector3ZUp() * MathToolbox.EaseInAndBack(t)),
                                            startColor = ColorTouse().WithAlpha(0.25f),
                                            startLifetime = UnityEngine.Random.Range(0.5f, 0.75f),
                                            startSize = 0.45f
                                        });

                                    }


                
                                };
                                enemy.healthHaver.OnDeath += action;
                                AllEnemies.Add(enemy, action);
                            }
                            Skip:
                            List<GameActorEffect> list = enemy.m_activeEffects;//PlanetsideReflectionHelper.ReflectGetField<List<GameActorEffect>>(typeof(AIActor), "m_activeEffects", enemy);
                            if (list != null | list.Count > 0)
                            {
                                for (int e = 0; e < list.Count; e++)
                                {
                                    if (!enemy.healthHaver.IsDead)
                                    {
                                        var debuff = list[e];
                                        if (debuff.GetType() == EffectToCleanse())
                                        {
                                            enemy.RemoveEffect(debuff.effectIdentifier);

                                            if (!this.enemiesToAvoid.Contains(enemy))
                                            {
                                                var res = new DamageTypeModifier();
                                                res.damageMultiplier = 0;
                                                res.damageType = EffectToResist();
                                                if (gem)
                                                {
                                                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                                                    {
                                                        position = this.GemTransform.position,
                                                        startColor = ColorTouse().WithAlpha(0.75f),
                                                        startLifetime = 0.5f,
                                                        startSize = 4
                                                    });
                                                }
                                                AkSoundEngine.PostEvent("Play_OBJ_dead_again_01", enemy.gameObject);
                                                GameManager.Instance.StartCoroutine(this.DoResistShield(enemy, res, this));
                                            }
                                        }
                                    }
                                }
                            }                  
                        }
                    }
                }
            }
        }



        public List<AIActor> enemiesToAvoid = new List<AIActor>();

        public IEnumerator DoResistShield(AIActor enemy, DamageTypeModifier r, EnemyBuffShrineController s)
        {
            if (enemy == null) { yield break; }
            if (s != null) { s.enemiesToAvoid.Add(enemy); }

            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = enemy.sprite.WorldCenter,
                startColor = ColorTouse().WithAlpha(0.75f),
                startLifetime = 0.5f,
                startSize = 4
            });

            var amount = Vector2.Distance(this.GemTransform.position, enemy.sprite.WorldCenter) * 3;
            var m = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 1).normalized * 6;


            for (int i = 0; i < amount; i++)
            {
                float t = i / amount;
                ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = Vector3.Lerp(this.GemTransform.position, enemy.sprite.WorldCenter, t) + (m.ToVector3ZUp() * MathToolbox.EaseInAndBack(t)),
                    startColor = ColorTouse().WithAlpha(0.333f),
                    startLifetime = UnityEngine.Random.Range(0.25f, 0.5f),
                    startSize = 0.5f
                });

            }


            enemy.healthHaver.damageTypeModifiers.Add(r);
            float e = 0;
            float d = 3.5f;
            var cl = ColorTouse();
            var cl2 = enemy.sprite.color;

            while (e < d) 
            {
                if (enemy == null) { yield break; }
                enemy.sprite.color = Color.Lerp(cl, cl2, e);// LerpColor(cl, cl2, e);
                enemy.sprite.UpdateColors();
                e += BraveTime.DeltaTime;
                yield return null;
            }
            if (enemy == null) { yield break; }
            if (s != null) { s.enemiesToAvoid.Remove(enemy); }

            enemy.sprite.color = cl2;
            enemy.healthHaver.damageTypeModifiers.Remove(r);
            yield break;
        }


        public static Color LerpColor(Color a, Color b, float t)
        {
            Color c = new Color();
            c.r = Mathf.Lerp(a.r, b.r, t);
            c.g = Mathf.Lerp(a.g, b.g, t);
            c.b = Mathf.Lerp(a.b, b.b, t);
            c.a = Mathf.Lerp(a.a, b.a, t);
            return c;
        }

        public override void OnDestroy()
        {

        }

        public CoreDamageTypes EffectToResist()
        {
            if (this.ownType == EffectType.FIRE) { return CoreDamageTypes.Fire; }
            if (this.ownType == EffectType.POISON) { return CoreDamageTypes.Poison; }
            if (this.ownType == EffectType.ICE) { return CoreDamageTypes.Ice; }
            return CoreDamageTypes.Fire;
        }

        public GoopDefinition GoopToSpawn()
        {
            switch (this.ownType) 
            {
                case EffectType.FIRE:
                    return Alexandria.Misc.GoopUtility.FireDef;
                case EffectType.POISON:
                    return Alexandria.Misc.GoopUtility.PoisonDef;
                case EffectType.ICE:
                    return Alexandria.Misc.GoopUtility.WaterDef;
                default:
                    return Alexandria.Misc.GoopUtility.FireDef;
            }
        }

        public GoopDefinition GoopToSpawn(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.FIRE:
                    return Alexandria.Misc.GoopUtility.FireDef;
                case EffectType.POISON:
                    return Alexandria.Misc.GoopUtility.PoisonDef;
                case EffectType.ICE:
                    return Alexandria.Misc.GoopUtility.WaterDef;
                default:
                    return Alexandria.Misc.GoopUtility.FireDef;
            }
        }


        public Color ColorTouse()
        {
            if (this.ownType == EffectType.FIRE) { return Color.red; }
            if (this.ownType == EffectType.POISON) { return Color.green; }
            if (this.ownType == EffectType.ICE) { return Color.blue; }
            return Color.red; 
        }

        public Type EffectToCleanse()
        {
            if (this.ownType == EffectType.FIRE) { return typeof(GameActorFireEffect); }
            if (this.ownType == EffectType.POISON) { return typeof(GameActorHealthEffect); }
            if (this.ownType == EffectType.ICE) { return typeof(GameActorFreezeEffect); }
            return typeof(GameActorFireEffect);
        }



        private RoomHandler currentRoom;
        private GameObject gem;
        public GameObject gemObjectToSpawn;

        public EffectType ownType;

        public enum EffectType
        {
            FIRE, 
            POISON,
            ICE
        }
    }


    public class EnemyBuffShrine
    {
        public static void Init()
        {
            string[] shardsNormal = new string[]
            {
                "debris_normal_002",
                "debris_normal_003",
                "debris_normal_005",
                "debris_normal_007",
            };
            string[] shardsNormalsmall = new string[]
            {
                "debris_normal_001",
                "debris_normal_004",
                "debris_normal_006",
            };
            string[] shardsPedestal = new string[]
            {
                "debris_pedestal_001",
                "debris_pedestal_002",
                "debris_pedestal_003",
                "debris_pedestal_004",
                "debris_pedestal_005",
                "debris_pedestal_006",
                "debris_pedestal_007",
            };
            string[] shardsHand = new string[]
            {
                "debris_hand_001",
            };

            DebrisObject[] shardObjectsNormalsmall = BreakableAPI_Bundled.GenerateDebrisObjects(shardsNormalsmall, StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 1, 5, 480, 150, null, 0.8f, "Play_OBJ_rock_break_01", null, 1, false);
            DebrisObject[] shardObjectsNormal= BreakableAPI_Bundled.GenerateDebrisObjects(shardsNormal, StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 1, 5, 240, 90, null, 1.25f, "Play_OBJ_rock_break_01", null, 1, false);
            DebrisObject[] shardObjectsHand= BreakableAPI_Bundled.GenerateDebrisObjects(shardsHand, StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 1, 5, 720, 240, null, 0.8f, "Play_enm_bigbulletboy_step_01", null, 1, false);
            DebrisObject[] shardObjectspedestal= BreakableAPI_Bundled.GenerateDebrisObjects(shardsPedestal, StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 1, 5, 120, 30, null, 2f, "Play_FS_bone_step_01", null, 0, false);


            ShardCluster shardObjectsTopCluster = BreakableAPI_Bundled.GenerateShardCluster(shardObjectsNormal, 0.5f, 1.1f, 3, 6, 1.4f);
            ShardCluster shardObjectsTopClustersmall = BreakableAPI_Bundled.GenerateShardCluster(shardObjectsNormalsmall, 1.23f, 1.4f, 3, 6, 3f);
            ShardCluster shardObjectsTopCluster1 = BreakableAPIToolbox.GenerateShardCluster(shardObjectsHand, 1.5f, 1f, 2, 2, 1.1f);
            ShardCluster shardObjectsTopsasaas = BreakableAPIToolbox.GenerateShardCluster(shardObjectspedestal, 0.7f, 1.2f, 2, 4, 1.2f);


            ShardCluster[] array = new ShardCluster[] { shardObjectsTopCluster, shardObjectsTopCluster1, shardObjectsTopsasaas, shardObjectsTopClustersmall };

            GenerateIceShrine(array);
            GenerateFireShrine(array);
            GeneratePoisonShrine(array);
        }

        public static void GenerateIceShrine(ShardCluster[] array)
        {
            /*
            string defaultPath = "Planetside/Resources/DungeonObjects/EnemyBuffShrine/";
            var gem = VFXToolbox.CreateVFX("ice_gem", new List<string>()
            {
                defaultPath + "Gems/ice_gem_idle_001.png",
                defaultPath + "Gems/ice_gem_idle_002.png",
                defaultPath + "Gems/ice_gem_idle_003.png",
                defaultPath + "Gems/ice_gem_idle_004.png",
                defaultPath + "Gems/ice_gem_idle_005.png",
            }, 7, new IntVector2(10, 10), tk2dBaseSprite.Anchor.LowerLeft, true, 3, -1, null, false, tk2dSpriteAnimationClip.WrapMode.Loop);
            string[] idlePaths = new string[]
            {
                defaultPath+"enemybuffshrine_idle_001.png",
            };
            Dictionary<float, string> prebreaks = new Dictionary<float, string>()
            {
                {90, defaultPath+"enemybuffshrine_idle_002.png"},
                {35, defaultPath+"enemybuffshrine_idle_003.png"},
            };
            
            
            
            
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("enemy_buff_totem", idlePaths, 1, null, 1, 25, true, 16, 16, 0, -4, true, null, null, true, null, prebreaks);
            statue.shardClusters = array;

            statue.ScaleWithEnemyHealth = true;
            statue.destroyedOnBreak = true;
            statue.handlesOwnPrebreakFrames = true;
            BreakableAPIToolbox.GenerateShadow(defaultPath + "enemybuffshrine_shadow.png", "pedestal_shadow", statue.gameObject.transform, new Vector3(0, -0.1875f));
            BreakableAPIToolbox.GenerateTransformObject(statue.gameObject, new Vector2(0.5f, 3.5f), "gemPoint");
            var enemyBuffer = statue.gameObject.AddComponent<EnemyBuffShrineController>();
            enemyBuffer.gemObjectToSpawn = gem;
            enemyBuffer.ownType = EnemyBuffShrineController.EffectType.ICE;
            statue.distributeShards = true;
            statue.shardBreakStyle = MinorBreakable.BreakStyle.CONE;
            statue.maxShardPercentSpeed = 0.5f;
            statue.minShardPercentSpeed = 1.5f;
            */

            var gem = PrefabBuilder.BuildObject("gem [Ice]");
            var spriteGem = gem.AddComponent<tk2dSprite>();
            spriteGem.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "ice_gem_idle_001");

            var animatorGem = gem.AddComponent<tk2dSpriteAnimator>();
            animatorGem.Library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animatorGem.playAutomatically = true;
            animatorGem.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("gemfrost");



            var shrineObject = PrefabBuilder.BuildObject("enemy_buff_totem_ice");
            var sprite = shrineObject.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "enemybuffshrine_idle_001");
            sprite.SortingOrder = 4;
            spriteGem.SortingOrder = 5;

            var specBody = shrineObject.CreateFastBody(new IntVector2(18, 18), new IntVector2(-1, -5), CollisionLayer.BulletBlocker);
            shrineObject.CreateFastBody(new IntVector2(18, 18), new IntVector2(-1, -5), CollisionLayer.BeamBlocker);
            shrineObject.CreateFastBody(new IntVector2(18, 18), new IntVector2(-1, -5), CollisionLayer.EnemyBlocker);
            shrineObject.CreateFastBody(new IntVector2(18, 18), new IntVector2(-1, -5), CollisionLayer.EnemyBulletBlocker);

            MajorBreakable statue = shrineObject.AddComponent<MajorBreakable>();
            statue.HitPoints = 25;



            statue.prebreakFrames = new BreakFrame[]
            {
                new BreakFrame{healthPercentage = 66, sprite = "enemybuffshrine_idle_002" },
                new BreakFrame{healthPercentage = 33, sprite = "enemybuffshrine_idle_003" }
            };



            statue.shardClusters = array;
            statue.shardBreakStyle = MinorBreakable.BreakStyle.BURST;
            statue.distributeShards = true;
            statue.spawnShards = true;

            statue.ScaleWithEnemyHealth = true;
            statue.destroyedOnBreak = true;
            statue.handlesOwnPrebreakFrames = true;


            
            var enemyBuffer = statue.gameObject.AddComponent<EnemyBuffShrineController>();
            enemyBuffer.gemObjectToSpawn = gem;
            enemyBuffer.ownType = EnemyBuffShrineController.EffectType.ICE;
            enemyBuffer.GemTransform = BreakableAPI_Bundled.GenerateTransformObject(statue.gameObject, new Vector2(0.5f, 3.5f), "gemPoint").transform;
            enemyBuffer.majorBreakable = statue;

            statue.distributeShards = true;
            statue.shardBreakStyle = MinorBreakable.BreakStyle.CONE;
            statue.maxShardPercentSpeed = 0.5f;
            statue.minShardPercentSpeed = 1.5f;
            var shadowSprite = PrefabBuilder.BuildObject("_Shadow").gameObject.AddComponent<tk2dSprite>();
            shadowSprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "enemybuffshrine_shadow");
            shadowSprite.transform.SetParent(statue.transform);


            StaticReferences.StoredRoomObjects.Add("ice_buffer_statue", statue.gameObject);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:ice_buffer_statue", statue.gameObject);

        }

        public static void GeneratePoisonShrine(ShardCluster[] array)
        {

            var gem = PrefabBuilder.BuildObject("gem [Poison]");
            var spriteGem = gem.AddComponent<tk2dSprite>();
            spriteGem.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "poison_gem_idle_001");

            var animatorGem = gem.AddComponent<tk2dSpriteAnimator>();
            animatorGem.Library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animatorGem.playAutomatically = true;
            animatorGem.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("gempoison");



            var shrineObject = PrefabBuilder.BuildObject("enemy_buff_totem_poison");
            var sprite = shrineObject.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "enemybuffshrine_idle_001");
            sprite.SortingOrder = 4;
            spriteGem.SortingOrder = 5;

            var specBody = shrineObject.CreateFastBody(new IntVector2(18, 18), new IntVector2(-1, -5), CollisionLayer.BulletBlocker);
            shrineObject.CreateFastBody(new IntVector2(18, 18), new IntVector2(-1, -5), CollisionLayer.BeamBlocker);
            shrineObject.CreateFastBody(new IntVector2(18, 18), new IntVector2(-1, -5), CollisionLayer.EnemyBlocker);
            shrineObject.CreateFastBody(new IntVector2(18, 18), new IntVector2(-1, -5), CollisionLayer.EnemyBulletBlocker);

            MajorBreakable statue = shrineObject.AddComponent<MajorBreakable>();
            statue.HitPoints = 25;



            statue.prebreakFrames = new BreakFrame[]
            {
                new BreakFrame{healthPercentage = 66, sprite = "enemybuffshrine_idle_002" },
                new BreakFrame{healthPercentage = 33, sprite = "enemybuffshrine_idle_003" }
            };



            statue.shardClusters = array;
            statue.shardBreakStyle = MinorBreakable.BreakStyle.BURST;
            statue.distributeShards = true;
            statue.spawnShards = true;

            statue.ScaleWithEnemyHealth = true;
            statue.destroyedOnBreak = true;
            statue.handlesOwnPrebreakFrames = true;


            //BreakableAPI_Bundled.GenerateTransformObject(statue.gameObject, new Vector2(0.5f, 3.5f), "gemPoint");
            var enemyBuffer = statue.gameObject.AddComponent<EnemyBuffShrineController>();
            enemyBuffer.gemObjectToSpawn = gem;
            enemyBuffer.ownType = EnemyBuffShrineController.EffectType.POISON;
            enemyBuffer.GemTransform = BreakableAPI_Bundled.GenerateTransformObject(statue.gameObject, new Vector2(0.5f, 3.5f), "gemPoint").transform;
            enemyBuffer.majorBreakable = statue;



            statue.distributeShards = true;
            statue.shardBreakStyle = MinorBreakable.BreakStyle.CONE;
            statue.maxShardPercentSpeed = 0.5f;
            statue.minShardPercentSpeed = 1.5f;
            var shadowSprite = PrefabBuilder.BuildObject("_Shadow").gameObject.AddComponent<tk2dSprite>();
            shadowSprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "enemybuffshrine_shadow");
            shadowSprite.transform.SetParent(statue.transform);

            
            
            StaticReferences.StoredRoomObjects.Add("poison_buffer_statue", statue.gameObject);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:poison_buffer_statue", statue.gameObject);
        }


        public static void GenerateFireShrine(ShardCluster[] array)
        {
            /*
            string defaultPath = "Planetside/Resources/DungeonObjects/EnemyBuffShrine/";
            var gem = VFXToolbox.CreateVFX("fire_gem", new List<string>() 
            {
                defaultPath + "Gems/fire_gem_idle_001.png",
                defaultPath + "Gems/fire_gem_idle_002.png",
                defaultPath + "Gems/fire_gem_idle_003.png",
                defaultPath + "Gems/fire_gem_idle_004.png",
                defaultPath + "Gems/fire_gem_idle_005.png",
            }, 7 ,new IntVector2(10 , 10), tk2dBaseSprite.Anchor.LowerLeft, true, 3, -1, null, false, tk2dSpriteAnimationClip.WrapMode.Loop);
            string[] idlePaths = new string[]
            {
                defaultPath+"enemybuffshrine_idle_001.png",
            };
            Dictionary<float, string> prebreaks = new Dictionary<float, string>()
            {
                {90, defaultPath+"enemybuffshrine_idle_002.png"},
                {35, defaultPath+"enemybuffshrine_idle_003.png"},
            };
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("enemy_buff_totem", idlePaths, 1, null, 1, 25, true, 16, 16, 0, -4, true, null, null, true, null, prebreaks);
            statue.shardClusters = array;

            statue.ScaleWithEnemyHealth = true;
            statue.destroyedOnBreak = true;
            statue.handlesOwnPrebreakFrames = true;
            BreakableAPIToolbox.GenerateShadow(defaultPath + "enemybuffshrine_shadow.png", "pedestal_shadow", statue.gameObject.transform, new Vector3(0, -0.1875f));
            BreakableAPIToolbox.GenerateTransformObject(statue.gameObject, new Vector2(0.5f, 3.5f), "gemPoint");
            var enemyBuffer =  statue.gameObject.AddComponent<EnemyBuffShrineController>();
            enemyBuffer.gemObjectToSpawn = gem;
            enemyBuffer.ownType = EnemyBuffShrineController.EffectType.FIRE;

            statue.distributeShards = true;
            statue.shardBreakStyle = MinorBreakable.BreakStyle.CONE;
            statue.maxShardPercentSpeed = 0.5f;
            statue.minShardPercentSpeed = 1.5f;
            */


            var gem = PrefabBuilder.BuildObject("gem [Fire]");
            var spriteGem = gem.AddComponent<tk2dSprite>();
            spriteGem.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "fire_gem_idle_001");

            var animatorGem = gem.AddComponent<tk2dSpriteAnimator>();
            animatorGem.Library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animatorGem.playAutomatically = true;
            animatorGem.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("gemfire");



            var shrineObject = PrefabBuilder.BuildObject("enemy_buff_totem_fire");
            var sprite = shrineObject.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "enemybuffshrine_idle_001");

            var specBody = shrineObject.CreateFastBody(new IntVector2(18, 18), new IntVector2(-1, -5), CollisionLayer.BulletBlocker);
            shrineObject.CreateFastBody(new IntVector2(18, 18), new IntVector2(-1, -5), CollisionLayer.BeamBlocker);
            shrineObject.CreateFastBody(new IntVector2(18, 18), new IntVector2(-1, -5), CollisionLayer.EnemyBlocker);
            shrineObject.CreateFastBody(new IntVector2(18, 18), new IntVector2(-1, -5), CollisionLayer.EnemyBulletBlocker);

            MajorBreakable statue = shrineObject.AddComponent<MajorBreakable>();
            statue.HitPoints = 25;



            statue.prebreakFrames = new BreakFrame[]
            {
                new BreakFrame{healthPercentage = 66, sprite = "enemybuffshrine_idle_002" },
                new BreakFrame{healthPercentage = 33, sprite = "enemybuffshrine_idle_003" }
            };



            statue.shardClusters = array;
            statue.ScaleWithEnemyHealth = true;
            statue.destroyedOnBreak = true;
            statue.handlesOwnPrebreakFrames = true;
            statue.shardBreakStyle = MinorBreakable.BreakStyle.BURST;
            statue.distributeShards = true;
            statue.spawnShards = true;

            sprite.SortingOrder = 4;
            spriteGem.SortingOrder = 5;
            //BreakableAPI_Bundled.GenerateTransformObject(statue.gameObject, new Vector2(0.5f, 3.5f), "gemPoint");
            var enemyBuffer = statue.gameObject.AddComponent<EnemyBuffShrineController>();
            enemyBuffer.gemObjectToSpawn = gem;
            enemyBuffer.ownType = EnemyBuffShrineController.EffectType.FIRE;
            enemyBuffer.GemTransform = BreakableAPI_Bundled.GenerateTransformObject(statue.gameObject, new Vector2(0.5f, 3.5f), "gemPoint").transform;
            enemyBuffer.majorBreakable = statue;

            statue.distributeShards = true;
            statue.shardBreakStyle = MinorBreakable.BreakStyle.CONE;
            statue.maxShardPercentSpeed = 0.5f;
            statue.minShardPercentSpeed = 1.5f;
            var shadowSprite = PrefabBuilder.BuildObject("_Shadow").gameObject.AddComponent<tk2dSprite>();
            shadowSprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "enemybuffshrine_shadow");
            shadowSprite.transform.SetParent(statue.transform);


            StaticReferences.StoredRoomObjects.Add("fire_buffer_statue", statue.gameObject);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:fire_buffer_statue", statue.gameObject);
            
        }
    }
}
