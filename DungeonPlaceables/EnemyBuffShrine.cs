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


namespace Planetside
{
    public class EnemyBuffShrineController : BraveBehaviour
    {
        public void Start()
        {
            self = GetComponent<MajorBreakable>();
            if (self){

                self.InvulnerableToEnemyBullets = true;

                currentRoom = self.transform.position.GetAbsoluteRoom();
                if (gemObjectToSpawn)
                {
                    gem = (GameObject)UnityEngine.Object.Instantiate(gemObjectToSpawn, self.transform.Find("gemPoint").transform.position, Quaternion.identity);
                    tk2dBaseSprite component = gem.GetComponent<tk2dBaseSprite>();
                    component.PlaceAtPositionByAnchor(self.transform.Find("gemPoint").transform.position, tk2dBaseSprite.Anchor.MiddleCenter);
                    component.HeightOffGround = 35f;
                    component.UpdateZDepth();
                    component.SortingOrder = 24;
                }
                self.OnDamaged = (obj) =>{
                    AkSoundEngine.PostEvent("Play_OBJ_rock_break_01", self.gameObject);
                };
                self.OnBreak = () => {
                    AkSoundEngine.PostEvent("Play_RockBreaking", self.gameObject);
                    AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", self.gameObject);

                    GameManager.Instance.StartCoroutine(ShrineParticlesOnDestory(self.sprite.WorldBottomLeft, self.sprite.WorldTopRight));
                    GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                    GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, gem.GetComponent<tk2dBaseSprite>().sprite.WorldCenter, Quaternion.identity);
                    blankObj.transform.localScale *= 1.33f;
                    Destroy(blankObj, 2f);
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


        public void Update()
        {
            if (this && currentRoom != null)
            {
                 var aiactorList = currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (aiactorList != null)
                {
                    if (aiactorList.Count == 0 | aiactorList.Count < 0) { return; }
                    for (int i = 0; i < aiactorList.Count; i++) {
                        AIActor enemy = aiactorList[i];
                        if (enemy){

                            List<GameActorEffect> list = PlanetsideReflectionHelper.ReflectGetField<List<GameActorEffect>>(typeof(AIActor), "m_activeEffects", enemy);
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
                                                    GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                                                    GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, gem.GetComponent<tk2dBaseSprite>().sprite.WorldCenter, Quaternion.identity);
                                                    blankObj.transform.localScale /= 2.5f;
                                                    Destroy(blankObj, 2f);
                                                }
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
            AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Capture_01", enemy.gameObject);
            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
            GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, enemy.sprite.WorldCenter, Quaternion.identity);
            blankObj.transform.localScale /= 2.5f;
            Destroy(blankObj, 2f);

            enemy.healthHaver.damageTypeModifiers.Add(r);
            float e = 0;
            float d = 2.5f;
            var cl = ColorTouse();
            var cl2 = enemy.sprite.color;

            while (e < d) 
            {
                if (enemy == null) { yield break; }
                enemy.sprite.color = LerpColor(cl, cl2, e);

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

        protected override void OnDestroy()
        {

        }

        public CoreDamageTypes EffectToResist()
        {
            if (this.ownType == EffectType.FIRE) { return CoreDamageTypes.Fire; }
            if (this.ownType == EffectType.POISON) { return CoreDamageTypes.Poison; }
            if (this.ownType == EffectType.ICE) { return CoreDamageTypes.Ice; }
            return CoreDamageTypes.Fire;
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

        public MajorBreakable self;
    }


    public class EnemyBuffShrine
    {
        public static void Init()
        {
            string shardDefaultPath = "Planetside/Resources/DungeonObjects/EnemyBuffShrine/Debris/";
            string[] shardsNormal = new string[]
            {
                shardDefaultPath+"debris_normal_002.png",
                shardDefaultPath+"debris_normal_003.png",
                shardDefaultPath+"debris_normal_005.png",
                shardDefaultPath+"debris_normal_007.png",
            };


            /*
             *                 shardDefaultPath+"debris_normal_001.png",
             shardDefaultPath+"debris_normal_004.png",
                            shardDefaultPath+"debris_normal_006.png",
            */
            string[] shardsNormalsmall = new string[]
            {
                shardDefaultPath+"debris_normal_001.png",
                shardDefaultPath+"debris_normal_004.png",
                shardDefaultPath+"debris_normal_006.png",
            };


            string[] shardsPedestal = new string[]
            {
                shardDefaultPath+"debris_pedestal_001.png",
                shardDefaultPath+"debris_pedestal_002.png",
                shardDefaultPath+"debris_pedestal_003.png",
                shardDefaultPath+"debris_pedestal_004.png",
                shardDefaultPath+"debris_pedestal_005.png",
                shardDefaultPath+"debris_pedestal_006.png",
                shardDefaultPath+"debris_pedestal_007.png",
            };

            string[] shardsHand = new string[]
            {
                shardDefaultPath+"debris_hand_001.png",
            };


            DebrisObject[] shardObjectsNormalsmall = BreakableAPIToolbox.GenerateDebrisObjects(shardsNormalsmall, true, 1, 5, 480, 150, null, 0.8f, "Play_OBJ_rock_break_01", null, 1, false);

            DebrisObject[] shardObjectsNormal= BreakableAPIToolbox.GenerateDebrisObjects(shardsNormal, true, 1, 5, 240, 90, null, 1.25f, "Play_OBJ_rock_break_01", null, 1, false);


            DebrisObject[] shardObjectsHand= BreakableAPIToolbox.GenerateDebrisObjects(shardsHand, true, 1, 5, 720, 240, null, 0.8f, "Play_enm_bigbulletboy_step_01", null, 1, false);
            DebrisObject[] shardObjectspedestal= BreakableAPIToolbox.GenerateDebrisObjects(shardsPedestal, true, 1, 5, 120, 30, null, 2f, "Play_FS_bone_step_01", null, 0, false);


            ShardCluster shardObjectsTopCluster = BreakableAPIToolbox.GenerateShardCluster(shardObjectsNormal, 0.5f, 1.1f, 1, 4, 1.4f);
            ShardCluster shardObjectsTopClustersmall = BreakableAPIToolbox.GenerateShardCluster(shardObjectsNormalsmall, 1.23f, 1.4f, 2, 4, 3f);


            ShardCluster shardObjectsTopCluster1 = BreakableAPIToolbox.GenerateShardCluster(shardObjectsHand, 1.5f, 1f, 1, 1, 1.1f);
            ShardCluster shardObjectsTopsasaas = BreakableAPIToolbox.GenerateShardCluster(shardObjectspedestal, 0.7f, 1.2f, 1, 4, 1.2f);


            ShardCluster[] array = new ShardCluster[] { shardObjectsTopCluster, shardObjectsTopCluster1, shardObjectsTopsasaas, shardObjectsTopClustersmall };

            GenerateIceShrine(array);
            GenerateFireShrine(array);
            GeneratePoisonShrine(array);
        }

        public static void GenerateIceShrine(ShardCluster[] array)
        {
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
            StaticReferences.StoredRoomObjects.Add("ice_buffer_statue", statue.gameObject);
        }

        public static void GeneratePoisonShrine(ShardCluster[] array)
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/EnemyBuffShrine/";
            var gem = VFXToolbox.CreateVFX("poison_gem", new List<string>()
            {
                defaultPath + "Gems/poison_gem_idle_001.png",
                defaultPath + "Gems/poison_gem_idle_002.png",
                defaultPath + "Gems/poison_gem_idle_003.png",
                defaultPath + "Gems/poison_gem_idle_004.png",
                defaultPath + "Gems/poison_gem_idle_005.png",
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
            enemyBuffer.ownType = EnemyBuffShrineController.EffectType.POISON;
            statue.distributeShards = true;
            statue.shardBreakStyle = MinorBreakable.BreakStyle.CONE;
            statue.maxShardPercentSpeed = 0.5f;
            statue.minShardPercentSpeed = 1.5f;

            StaticReferences.StoredRoomObjects.Add("poison_buffer_statue", statue.gameObject);
        }


        public static void GenerateFireShrine(ShardCluster[] array)
        {
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


            StaticReferences.StoredRoomObjects.Add("fire_buffer_statue", statue.gameObject);
        }
    }
}
