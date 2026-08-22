using Alexandria.Misc;
using Alexandria.PrefabAPI;
using Brave.BulletScript;
using BreakAbleAPI;
using Dungeonator;
using PathologicalGames;
using Planetside.Static_Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using static Dungeonator.CellVisualData;
using static Planetside.BoxOfGrenadesController;
using static UnityEngine.UI.GridLayoutGroup;

namespace Planetside.DungeonPlaceables
{
    public class Landmine
    {
        public static void Init()
        {
            InitSigns();
            InitMines();
        }


        private static MinorBreakable InitSign(string SignName, string MainObjectSpriteName, string breakAnimation, string shadowSprite)
        {
            var EmberPot = PrefabBuilder.BuildObject(SignName);
            EmberPot.layer = Layers.FG_Critical;
            var sprite = EmberPot.AddComponent<tk2dSprite>();

            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, MainObjectSpriteName);

            var animator = EmberPot.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.playAutomatically = false;
            sprite.usesOverrideMaterial = true;
            Material mat = new Material(StaticShaders.Default_Object_Shader);
            sprite.renderer.material = mat;
            EmberPot.CreateFastBody(new IntVector2(16, 12), new IntVector2(0, -1), CollisionLayer.PlayerBlocker);
            EmberPot.CreateFastBody(new IntVector2(16, 12), new IntVector2(0, -1), CollisionLayer.EnemyBlocker);
            EmberPot.CreateFastBody(new IntVector2(16, 12), new IntVector2(0, -1), CollisionLayer.BulletBlocker);
            EmberPot.CreateFastBody(new IntVector2(16, 12), new IntVector2(0, -1), CollisionLayer.BeamBlocker);

            var breakable = EmberPot.AddComponent<MinorBreakable>();
            sprite.IsPerpendicular = false;
            breakable.breakAudioEventName = "Play_obj_box_break_01";
            breakable.stopsBullets = true;
            breakable.OnlyPlayerProjectilesCanBreak = false;
            breakable.OnlyBreaksOnScreen = false;
            breakable.resistsExplosions = false;
            breakable.canSpawnFairy = false;
            breakable.chanceToRain = 1;
            breakable.dropCoins = false;
            breakable.breakAnimName = breakAnimation;

            var grenadeBoxShadow = PrefabBuilder.BuildObject("Shadow");
            sprite = grenadeBoxShadow.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, shadowSprite);
            sprite.transform.localPosition = new Vector3(0, -.25f);
            sprite.IsPerpendicular = false;
            grenadeBoxShadow.gameObject.transform.SetParent(breakable.transform, false);

            return breakable;
        }


        private static void InitSigns()
        {

            MinorBreakable Sign1 = InitSign("Minefield Sign Default", "minefield_sign_001", "minesign_break_1", "minefield_shadow_001");
            MinorBreakable Sign2 = InitSign("Minefield Sign Left", "minefield_sign_002", "minesign_break_2", "minefield_shadow_002");
            MinorBreakable Sign3 = InitSign("Minefield Sign Right", "minefield_sign_003", "minesign_break_3", "minefield_shadow_003");


            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { Sign1.gameObject, 1f },
                { Sign2.gameObject, 1f },
                { Sign3.gameObject, 1f },
            };

            DebrisObject shardObject = BreakableAPI_Bundled.GenerateDebrisObject("minefield_sign_debris_001", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 1, 5, 360, 120, null, 0.5f, null, null, 0, false);
            var animator = shardObject.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("minesign_debris");
            animator.sprite.usesOverrideMaterial = true;
            var mat = new Material(StaticShaders.Default_Object_Shader);
            animator.sprite.renderer.material = mat;

            ShardCluster paperCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shardObject }, 0.4f, 1f, 4, 7, 0.8f);


            Sign1.shardClusters = new ShardCluster[] { paperCluster };
            Sign2.shardClusters = new ShardCluster[] { paperCluster };
            Sign3.shardClusters = new ShardCluster[] { paperCluster };


            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("psog:randomminefieldsign", placeable);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:minefieldsignfront", Sign1.gameObject);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:minefieldsignleft", Sign2.gameObject);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:minefieldsignright", Sign3.gameObject);

        }
        public static ExplosionData mineVFXExplosionData;
        private static void InitMines()
        {

            mineVFXExplosionData = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericLargeExplosion);
            mineVFXExplosionData.damageRadius = 0;
            mineVFXExplosionData.debrisForce = 10;
            mineVFXExplosionData.forceUseThisRadius = true;
            mineVFXExplosionData.pushRadius = 0;
            mineVFXExplosionData.damage = 0;
            mineVFXExplosionData.doDamage = false;

            var EMPTY = PrefabBuilder.BuildObject("GenericEmptyObject");


            var lockbox = PrefabBuilder.BuildObject("Jumping Basic Mine");
            var sprite = lockbox.AddComponent<tk2dSprite>();
            var animator = lockbox.AddComponent<tk2dSpriteAnimator>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("landline_jump_001"));
            animator.playAutomatically = false;
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            var mineController = lockbox.AddComponent<MineController>();
            mineController.sprite = sprite;
            mineController.spriteAnimator = animator;
            lockbox.layer = Layers.FG_Nonsense;
            sprite.SortingOrder = 0;
            sprite.renderer.material = new Material(StaticShaders.Default_Shader);
            var b = lockbox.AddComponent<AIBulletBank>();
            b.Bullets = new List<AIBulletBank.Entry>();
            b.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
            b.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));
            b.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));

            var grenadeBoxShadow = PrefabBuilder.BuildObject("Shadow");
            var sprite_Shadow = grenadeBoxShadow.AddComponent<tk2dSprite>();
            sprite_Shadow.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "mine_shadow_001");
            sprite_Shadow.transform.localPosition = new Vector3(0, 0);
            sprite_Shadow.IsPerpendicular = false;
            sprite_Shadow.SortingOrder = -2;
            grenadeBoxShadow.gameObject.transform.SetParent(mineController.transform, false);

            //EnemyToolbox.AddEventTriggersToAnimation(animator, "minejump", new Dictionary<int, string> { { 0, "Surprise" }, { 6, "PepsiRage" } });
            //m_ENM_blobulord_reform_01
            EnemyToolbox.AddSoundsToAnimationFrame(animator, "minejump", new Dictionary<int, string> 
            {
                { 0, "Play_PET_dog_dig_02" },
                { 2, "Play_PET_dog_dig_01" },
                { 4, "Play_PET_dog_dig_02" },

                { 7, "Play_OBJ_mine_set_01" },
                { 9, "Play_OBJ_mine_beep_01" },
                { 11, "Play_OBJ_mine_beep_01" },
                { 13, "Play_OBJ_mine_beep_01" },
                
                { 14, "Play_ENM_bulletking_throw_01" },
                { 19, "Play_CHR_pit_fall_01" },

                { 26, "Play_BOSS_mineflayer_trigger_01" },
            });
            EnemyToolbox.AddEventTriggersToAnimation(animator, "minejump", new Dictionary<int, string> 
            { 
                { 14, "LEAP" },
                { 26, "Boop" },
                { 28, "KaBlewy" } 
            });

            var amorPickup = PrefabBuilder.BuildObject("BuriedBasicLandmine").AddComponent<BuriedLandmine>();
            amorPickup.Tiles = new IntVector2(1, 1);
            amorPickup.gameObject.layer = Layers.BG_Critical;
            amorPickup.RandomObjectsToSpawn = 0;
            amorPickup.RevealednessBeforeFullReveal = 0.3f;
            amorPickup.MinePrefab = mineController;
            UnityEngine.Object.DontDestroyOnLoad(amorPickup);




            var place2 = BreakableAPI_Bundled.GenerateDungeonPlaceable(new Dictionary<GameObject, float>()
            {
                { amorPickup.gameObject, 1 }
            }, 1, 1);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("psog:buriedbasiclandmine_100", place2);
            
            
            place2 = BreakableAPI_Bundled.GenerateDungeonPlaceable(new Dictionary<GameObject, float>()
            {
                { amorPickup.gameObject, 1 },
                { EMPTY, 1 }
            }, 1, 1);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("psog:buriedbasiclandmine_50", place2);

            place2 = BreakableAPI_Bundled.GenerateDungeonPlaceable(new Dictionary<GameObject, float>()
            {
                { amorPickup.gameObject, 1 },
                { EMPTY, 3 }
            }, 1, 1);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("psog:buriedbasiclandmine_25", place2);

            place2 = BreakableAPI_Bundled.GenerateDungeonPlaceable(new Dictionary<GameObject, float>()
            {
                { amorPickup.gameObject, 3 },
                { EMPTY, 1 }
            }, 1, 1);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("psog:buriedbasiclandmine_75", place2);



            var ForgeVent = PrefabBuilder.BuildObject("ForgeVent");
            var ventSprite = ForgeVent.AddComponent<tk2dSprite>();
            var ventAnimator = ForgeVent.AddComponent<tk2dSpriteAnimator>();
            ventSprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("oldvent_uncover_001"));
            ventAnimator.playAutomatically = false;
            ventAnimator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            var mineControllerVent = ForgeVent.AddComponent<MineController>();
            mineControllerVent.sprite = ventSprite;
            mineControllerVent.spriteAnimator = ventAnimator;
            mineControllerVent.isVent = true;
            ForgeVent.layer = Layers.BG_Nonsense;
            ventSprite.SortingOrder = 0;
            ventSprite.HeightOffGround = 0;
            ventSprite.usesOverrideMaterial = true;
            ventSprite.IsPerpendicular = false;
            ventSprite.renderer.material = new Material(StaticShaders.Default_Decal_Shader);
            var b2 = ForgeVent.AddComponent<AIBulletBank>();
            b2.Bullets = new List<AIBulletBank.Entry>();

            b2.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"));
            b2.Bullets.Add(StaticBulletEntries.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("homingRing"), "ventFire", null, null, false));


            var forgeVent = PrefabBuilder.BuildObject("BuriedForgeVent").AddComponent<BuriedLandmine>();
            forgeVent.Tiles = new IntVector2(2, 2);
            forgeVent.gameObject.layer = Layers.BG_Critical;
            forgeVent.RandomObjectsToSpawn = 0;
            forgeVent.RevealednessBeforeFullReveal = 0.3f;
            forgeVent.MinePrefab = mineControllerVent;
            forgeVent.isForgeVent = true;
            UnityEngine.Object.DontDestroyOnLoad(forgeVent);

            var ventPlaceable = BreakableAPI_Bundled.GenerateDungeonPlaceable(new Dictionary<GameObject, float>()
            {
                { forgeVent.gameObject, 1 }
            }, 1, 1);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("psog:buriedforgevent", ventPlaceable);

            EnemyToolbox.AddEventTriggersToAnimation(ventAnimator, "oldvent_popup", new Dictionary<int, string>
            {
                { 0, "vent_state_0" },

                { 17, "vent_state_1" },
                { 18, "vent_state_2" },
                { 19, "vent_state_3" },
                { 20, "vent_state_4" },
                { 21, "vent_state_5" },
                { 22, "DIVINEDEATH" },
                { 25, "vent_state_5" },
                { 26, "vent_state_4" },
                { 27, "vent_state_3" },
                { 28, "vent_state_2" },
                { 29, "vent_state_1" },

            });

            var ForgeVentCover = PrefabBuilder.BuildObject("ForgeVentCover");
            var ventSpriteCover = ForgeVentCover.AddComponent<tk2dSprite>();
            var ventAnimatorCover = ForgeVentCover.AddComponent<tk2dSpriteAnimator>();
            ventSpriteCover.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("oldvent_uncover_001"));
            ventAnimatorCover.playAutomatically = true;
            ventAnimatorCover.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            ForgeVentCover.layer = Layers.BG_Nonsense;
            ventSpriteCover.SortingOrder = 2;
            ventSpriteCover.HeightOffGround = 0;
            ventSpriteCover.usesOverrideMaterial = true;
            ventSpriteCover.IsPerpendicular = false;
            ventSpriteCover.renderer.material = new Material(StaticShaders.Default_Decal_Shader);
            ventAnimatorCover.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("oldvent_empty");

            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:forgeventcover", ForgeVentCover);

            var ForgeVentLining = PrefabBuilder.BuildObject("ForgeVentLining");
            var ventSpriteLining = ForgeVentLining.AddComponent<tk2dSprite>();
            ventSpriteLining.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("oldvent_coverAround_001"));
            ForgeVentLining.layer = Layers.BG_Nonsense;
            ventSpriteLining.SortingOrder = 1;
            ventSpriteLining.HeightOffGround = -1f;
            ventSpriteLining.usesOverrideMaterial = true;
            ventSpriteLining.IsPerpendicular = false;
            ventSpriteLining.renderer.material = new Material(StaticShaders.Default_Decal_Shader);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:forgeventlining", ForgeVentLining);
        }

        public class MineController : BraveBehaviour
        {
            public bool isVent = false;



            public void Start()
            {
                base.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
                if (isVent)
                {
                    var obj = new GameObject();
                    obj.transform.SetParent(this.transform, false);
                    obj.transform.position = this.sprite.WorldCenter;
                    easyLight = obj.AddComponent<AdditionalBraveLight>();
                    easyLight.LightColor = new Color(1, 0.65f, 0);
                    easyLight.LightRadius = 4.5f;
                    easyLight.LightIntensity = 0;
                    easyLight.Initialize();
                    IntVector2 b = base.transform.position.IntXY(VectorConversions.Floor);
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            IntVector2 key = new IntVector2(i, j) + b;
                            CellData cell = GameManager.Instance.Dungeon.data[key];
                            cell.isOccupied = true;
                            cell.containsTrap = true;
                            cell.cellVisualData.floorType = CellFloorType.Stone;
                        }
                    }
                    //easyLight = EasyLight.Create(sprite.WorldBottomLeft + new Vector2(0.5f, 0.5f), this.transform, new Color(1, 0.65f, 0), -1, 4, false, 10);
                }
                else
                {
                    IntVector2 b = base.transform.position.IntXY(VectorConversions.Floor);
                    for (int i = 0; i < 1; i++)
                    {
                        for (int j = 0; j < 1; j++)
                        {
                            IntVector2 key = new IntVector2(i, j) + b;
                            CellData cell = GameManager.Instance.Dungeon.data[key];
                            cell.isOccupied = true;
                            cell.containsTrap = true;
                        }
                    }
                }
            }
            private float VentStateInst = 0;
            public float VentState = 0;
            public AdditionalBraveLight easyLight;


            public void Update()
            {
                if (isVent)
                {
                    VentStateInst = Mathf.MoveTowards(VentStateInst, VentState, 4 * BraveTime.DeltaTime);
                    if (easyLight)
                    {
                        easyLight.LightIntensity = (VentStateInst * 0.625f);
                    }

                    if (!TrapDefusalKit.TrapsShouldBeDefused())
                    {
                        if (UnityEngine.Random.value < (VentStateInst * BraveTime.DeltaTime) * 2)
                        {
                            ParticleBase.EmitParticles("FireParticle_BG", 1, new ParticleSystem.EmitParams()
                            {
                                position = this.sprite.WorldCenter + BraveUtility.RandomVector2(new Vector2(-0.625f, -0.625f), new Vector2(0.625f, 0.625f)),
                                velocity = Vector3.up * UnityEngine.Random.Range(1 + VentStateInst, 3 + VentStateInst),
                                startLifetime = UnityEngine.Random.Range(0.25f, 0.75f)
                            });
                        }
                    }
                }
            }

            private IEnumerator DoDivineDeath()
            {

                //m_BOSS_lichB_charge_02
                AkSoundEngine.PostEvent("Play_BOSS_lichB_charge_02", this.gameObject);
                AkSoundEngine.PostEvent("Play_BOSS_cyborg_charge_01", this.gameObject);

                //m_BOSS_cyborg_charge_01
                yield return new WaitForSeconds(0.25f);
                AkSoundEngine.PostEvent("Play_TRP_fire_burst_01", this.gameObject);

                var vv = this.sprite.WorldCenter + new Vector2(-0.375f, -0.375f);
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopUtility.FireDef).TimedAddGoopLine(vv, vv + new Vector2(0, 8), 0.5f, 2);
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopUtility.FireDef).TimedAddGoopLine(vv, vv + new Vector2(0, -8), 0.5f, 2);
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopUtility.FireDef).TimedAddGoopLine(vv, vv + new Vector2(-8, 0), 0.5f, 2);
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopUtility.FireDef).TimedAddGoopLine(vv, vv + new Vector2(8, 0), 0.5f, 2);

                SpawnManager.SpawnBulletScript(null, this.sprite.WorldCenter, this.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(LandmineFire)), StringTableManager.GetEnemiesString("#TRAP", -1));

                yield break;
            }

            private bool Landed = false;
            public IEnumerator DoSloeMoveTowardsPlayer()
            {
                float Speed = 1.8f;
                Vector3 dir = GameManager.Instance.AllPlayers[UnityEngine.Random.Range(0, GameManager.Instance.AllPlayers.Length)].transform.position - this.transform.position;

                dir = dir.normalized * Speed;

                while (this.gameObject)
                {
                    if (Landed)
                        break;
                    this.transform.position += dir * BraveTime.DeltaTime;
                    yield return null;
                }
                yield break;
            }

            private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
            {
                if (clip.GetFrame(frameIdx).eventInfo.Contains("vent_state_0"))
                {
                    VentState = 0;
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("vent_state_1"))
                {
                    VentState = 1;
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("vent_state_2"))
                {
                    VentState = 2;
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("vent_state_3"))
                {
                    VentState = 3;
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("vent_state_4"))
                {
                    VentState = 4;
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("vent_state_5"))
                {
                    VentState = 5;
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("DIVINEDEATH"))
                {
                    VentState = 10;
                    if (!TrapDefusalKit.TrapsShouldBeDefused())
                    {
                        this.StartCoroutine(DoDivineDeath());
                    }
                }

                if (clip.GetFrame(frameIdx).eventInfo.Contains("LEAP"))
                {
                    this.StartCoroutine(DoSloeMoveTowardsPlayer());
                    for (int i  =0; i < 16; i++)
                    {
                        GlobalSparksDoer.DoRandomParticleBurst(1,
                        this.sprite.WorldBottomLeft + new Vector2(0.5f, 0.5f),
                        this.sprite.WorldBottomLeft + new Vector2(0.5f, 0.5f),
                        BraveUtility.RandomVector2(new Vector2(-1, -1), new Vector2(1, 1)) * UnityEngine.Random.Range(6, 9),
                        2f,
                        1f,
                        0.125f,
                        0.25f,
                        Color.gray,
                        GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    }
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("Boop"))
                {
                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = this.sprite.WorldBottomLeft + new Vector2(0.5f, 0.5f),
                        startSize = 4,
                        rotation = 0,
                        startLifetime = 0.333f,
                        startColor = Color.red.WithAlpha(0.333f)
                    });
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("KaBlewy"))
                {
                    Landed = true;
                    sprite.HeightOffGround = -2;
                    sprite.Awake();
                    if (!TrapDefusalKit.TrapsShouldBeDefused())
                    {
                        Exploder.Explode(this.sprite.WorldBottomLeft + new Vector2(0.5f, 0.5f), mineVFXExplosionData, Vector2.zero);
                        SpawnManager.SpawnBulletScript(null, this.sprite.WorldBottomLeft + new Vector2(0.5f, 0.5f), this.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(LandmineScript)), StringTableManager.GetEnemiesString("#TRAP", -1));
                        Destroy(this.gameObject, 0.1f);
                    }
                    else
                    {
                        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = this.sprite.WorldBottomLeft + new Vector2(0.5f, 0.5f),
                            startSize = 4,
                            rotation = 0,
                            startLifetime = 0.333f,
                            startColor = Color.white.WithAlpha(0.333f)
                        });
                    }
                }
            }
            
            public void VentReveal()
            {
                IntVector2 b = base.transform.position.IntXY(VectorConversions.Floor);
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        IntVector2 key = new IntVector2(i, j) + b;
                        CellData cell = GameManager.Instance.Dungeon.data[key];
                        cell.isOccupied = true;
                        cell.forceDisallowGoop = true;
                        cell.containsTrap = true;
                    }
                }
            }


        }




        public class BuriedLandmine : BuriedObject.BuriedObjectController
        {
            public MineController MineInst;
            public MineController MinePrefab;

            private int oldLayer;
            private int oldGameObjectLayer;
            public Material BuriedMaterial;

            public bool isForgeVent = false;

            public override void OnControllerSpawned()
            {
                MineInst = UnityEngine.Object.Instantiate(MinePrefab, this.transform.position, Quaternion.identity);
                oldLayer = MineInst.sprite.renderLayer;
                oldGameObjectLayer = MineInst.gameObject.layer;
                BuriedMaterial = MineInst.sprite.renderer.material;
                
                MineInst.sprite.renderer.sortingOrder = TileInstances[0].sprite.SortingOrder + 1;
                
                MineInst.sprite.renderLayer = TileInstances[0].sprite.renderLayer;
                MineInst.gameObject.layer = TileInstances[0].gameObject.layer;
                
                MineInst.renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                MineInst.renderer.receiveShadows = false;
                MineInst.sprite.usesOverrideMaterial = true;

                var mainTex = MineInst.sprite.renderer.material.mainTexture;
                MineInst.sprite.renderer.material = new Material(StaticShaders.Default_Shader_Basic);
                MineInst.sprite.renderer.material.mainTexture = mainTex;
                if (!isForgeVent)
                {
                    MineInst.transform.GetChild(0).gameObject.SetActive(false);
                }
            }

            public override void OnFullReveal()
            {
                MineInst.sprite.renderer.sortingOrder++;
                MineInst.sprite.renderLayer = oldLayer;
                MineInst.gameObject.layer = oldGameObjectLayer;
                MineInst.sprite.renderer.material = BuriedMaterial;

                if (isForgeVent)
                {
                    LootEngine.DoDefaultItemPoof(MineInst.sprite.WorldCenter, false, true);
                    MineInst.spriteAnimator.Play("oldvent_popup");

                    //MineInst.sprite.gameObject.layer = Layers.BG_Critical;
                    MineInst.sprite.SortingOrder = 2;
                    MineInst.sprite.HeightOffGround = 0.125f;
                    MineInst.sprite.Awake();
                    MineInst.VentReveal();
                }
                else
                {
                    LootEngine.DoDefaultItemPoof(MineInst.sprite.WorldBottomLeft + new Vector2(0.5f, 0.5f), false, true);
                    this.Invoke("DoJump", UnityEngine.Random.Range(0.125f, 0.625f));
                    MineInst.transform.GetChild(0).gameObject.SetActive(true);
                    SpriteOutlineManager.AddOutlineToSprite(MineInst.sprite, Color.black, 4);
                }

            }

            private void DoJump()
            {
                MineInst.spriteAnimator.Play("minejump");
            }


            public override void OnHintObjectRoll(int num)
            {

            }
        }

        public class LandmineScript : Script
        {
            public override IEnumerator Top()
            {
                for (int i = 0; i < 12; i++)
                {
                    base.Fire(new Direction(30 * i, DirectionType.Aim), new Speed(12, SpeedType.Absolute), new SpeedChangingBullet("sweep", 9, 90));
                    base.Fire(new Direction(30 * i, DirectionType.Aim), new Speed(11, SpeedType.Absolute), new SpeedChangingBullet("poundSmall", 9, 90));
                }
                for (int i = 0; i < 24; i++)
                {
                    base.Fire(new Direction(15 * i, DirectionType.Aim), new Speed(2, SpeedType.Absolute), new SpeedChangingBullet("poundSmall", 0, 45, 180));
                }
                yield break;
            }
        }
        public class LandmineFire: Script
        {
            public override IEnumerator Top()
            {
                int t = 1;
                int tt = 1;
                for (int i = 0; i < 6; i++)
                {
                    base.Fire(new Direction(60 * i, DirectionType.Aim), new Speed(UnityEngine.Random.Range(5, 6.5f), SpeedType.Absolute), new SpeedChangingBullet((UnityEngine.Random.value < 0.33f * t ? "frogger" : "ventFire"), UnityEngine.Random.Range(3, 4), 210));
                }

                for (int e = 0; e < 45; e++)
                {
                    t--;
                    if (t == 0)
                    {
                        tt+=3;
                        t = tt;
                        base.Fire(new Offset(UnityEngine.Random.Range(-0.25f, -0.25f), UnityEngine.Random.Range(-0.25f, -0.25f)), new Direction(0 + UnityEngine.Random.Range(-2, 3), DirectionType.Absolute), new Speed(UnityEngine.Random.Range(7, 12), SpeedType.Absolute), new SpeedChangingBullet((UnityEngine.Random.value < 0.33f * t ? "frogger" : "ventFire"), UnityEngine.Random.Range(2, 5), 135));
                        base.Fire(new Offset(UnityEngine.Random.Range(-0.25f, -0.25f), UnityEngine.Random.Range(-0.25f, -0.25f)), new Direction(90 + UnityEngine.Random.Range(-2, 3), DirectionType.Absolute), new Speed(UnityEngine.Random.Range(7, 12), SpeedType.Absolute), new SpeedChangingBullet((UnityEngine.Random.value < 0.33f * t ? "frogger" : "ventFire"), UnityEngine.Random.Range(2, 5), 135));
                        base.Fire(new Offset(UnityEngine.Random.Range(-0.25f, -0.25f), UnityEngine.Random.Range(-0.25f, -0.25f)), new Direction(180 + UnityEngine.Random.Range(-2, 3), DirectionType.Absolute), new Speed(UnityEngine.Random.Range(7, 12), SpeedType.Absolute), new SpeedChangingBullet((UnityEngine.Random.value < 0.33f * t ? "frogger" : "ventFire"), UnityEngine.Random.Range(2, 5), 135));
                        base.Fire(new Offset(UnityEngine.Random.Range(-0.25f, -0.25f), UnityEngine.Random.Range(-0.25f, -0.25f)), new Direction(270 + UnityEngine.Random.Range(-2, 3), DirectionType.Absolute), new Speed(UnityEngine.Random.Range(7, 12), SpeedType.Absolute), new SpeedChangingBullet((UnityEngine.Random.value < 0.33f * t ? "frogger" : "ventFire"), UnityEngine.Random.Range(2, 5), 135));
                        base.PostWwiseEvent("Play_BOSS_doormimic_flame_01", null);
                    }
                    yield return this.Wait(1);
                }



                yield break;
            }
        }
    }
}
