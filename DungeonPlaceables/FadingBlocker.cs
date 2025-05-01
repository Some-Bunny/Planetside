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
using System.Collections;
using Alexandria.PrefabAPI;
using Planetside.Static_Storage;
using UnityEngine.Playables;
using Newtonsoft.Json.Linq;
using ChallengeAPI;

namespace Planetside
{


    //SND_OBJ_gate_open_01
    //m_OBJ_boulder_break_03
    public class FadingBlocker : BraveBehaviour
    {
        public static void Init()
        {
            var abbeyAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("abbey_movingblock_godown");
            abbeyAnim.frames[16].triggerEvent = true;
            abbeyAnim.frames[16].eventInfo = "disableCollider";
            abbeyAnim.frames[2].triggerEvent = true;
            abbeyAnim.frames[2].eventAudio = "Play_Moondoor_close";
            abbeyAnim.frames[2].eventInfo = "isNowLowering";

            var forgeAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("forge_movingblock_godown");
            forgeAnim.frames[15].triggerEvent = true;
            forgeAnim.frames[15].eventInfo = "disableCollider";
            forgeAnim.frames[1].triggerEvent = true;
            forgeAnim.frames[1].eventAudio = "Play_CryoClose_01";

            forgeAnim.frames[10].triggerEvent = true;
            forgeAnim.frames[10].eventInfo = "isNowLowering";

            var hellAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("hell_movingblock_godown");
            hellAnim.frames[21].triggerEvent = true;
            hellAnim.frames[21].eventInfo = "disableCollider";
            hellAnim.frames[13].triggerEvent = true;
            hellAnim.frames[13].eventAudio = "Play_OBJ_moondoor_close_01";
            hellAnim.frames[13].eventInfo = "isNowLowering";
            hellAnim.frames[2].triggerEvent = true;
            hellAnim.frames[2].eventAudio = "Play_OBJ_hook_pull_01";
            hellAnim.frames[5].triggerEvent = true;
            hellAnim.frames[5].eventAudio = "Play_FS_shelleton_stone_01";


            var hollowAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("hollow_movingblock_godown");
            hollowAnim.frames[16].triggerEvent = true;
            hollowAnim.frames[16].eventInfo = "disableCollider";
            hollowAnim.frames[1].triggerEvent = true;
            hollowAnim.frames[1].eventAudio = "Play_Moondoor_close";
            hollowAnim.frames[1].eventInfo = "isNowLowering";

            var keepAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("keep_movingblock_godown");
            keepAnim.frames[19].triggerEvent = true;
            keepAnim.frames[19].eventInfo = "disableCollider";
            keepAnim.frames[1].triggerEvent = true;
            keepAnim.frames[1].eventAudio = "Play_Moondoor_close";
            keepAnim.frames[1].eventInfo = "isNowLowering";

            var minesAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("mines_movingblock_godown");
            minesAnim.frames[12].triggerEvent = true;
            minesAnim.frames[12].eventInfo = "disableCollider";
            minesAnim.frames[1].triggerEvent = true;
            minesAnim.frames[1].eventAudio = "Play_StoneCrumble";
            minesAnim.frames[1].eventInfo = "isNowLowering";

            var properAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("proper_movingblock_godown");
            properAnim.frames[17].triggerEvent = true;
            properAnim.frames[17].eventInfo = "disableCollider";
            properAnim.frames[2].triggerEvent = true;
            properAnim.frames[2].eventAudio = "Play_Moondoor_close";
            properAnim.frames[2].eventInfo = "isNowLowering";

            var sewerAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("sewer_movingblock_godown");
            sewerAnim.frames[15].triggerEvent = true;
            sewerAnim.frames[15].eventInfo = "disableCollider";
            sewerAnim.frames[2].triggerEvent = true;
            sewerAnim.frames[2].eventAudio = "Play_Moondoor_close";
            sewerAnim.frames[2].eventInfo = "isNowLowering";


            var block = PrefabBuilder.BuildObject("MoveOutBlock");
            block.MarkAsFakePrefab();
            DontDestroyOnLoad(block);

            var movingblock = block.AddComponent<FadingBlocker>();
            var sprite = block.AddComponent<tk2dSprite>();


            block.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Nonsense"));


            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("keep_movingblock_idle_001"));

            sprite.usesOverrideMaterial = true;

            Material mat = new Material(StaticShaders.FloorTileMaterial_Transparency);
            mat.SetTexture("_MainTex", sprite.renderer.material.mainTexture);
            sprite.renderer.material = mat;

            var animator = block.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.Library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.sprite = sprite;
            //new IntVector2(16, 16), new IntVector2(0, 0), CollisionLayer.BeamBlocker);
            var body = block.CreateFastBody(new IntVector2(16, 16), new IntVector2(0, 0), CollisionLayer.BeamBlocker);
            block.CreateFastBody(new IntVector2(16, 16), new IntVector2(0, -4), CollisionLayer.PlayerBlocker);
            block.CreateFastBody(new IntVector2(16, 16), new IntVector2(0, 0), CollisionLayer.BulletBlocker);
            block.CreateFastBody(new IntVector2(16, 16), new IntVector2(0, -4), CollisionLayer.EnemyBlocker);

            movingblock.Bodies = new SpeculativeRigidbody[1] { body };
            movingblock.DelayWaitTime = 0.25f;
            movingblock.sprite = sprite;
            movingblock.spriteAnimator = animator;


            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("PSOG_movingblock", block);

            Alexandria.DungeonAPI.RoomFactory.OnCustomProperty += OnAction;
        }

        public static GameObject OnAction(string ObjName, GameObject Original, JObject jObject)
        {
            if (ObjName != "PSOG_movingblock") {return Original; }
            Original = UnityEngine.Object.Instantiate(Alexandria.DungeonAPI.StaticReferences.customObjects["PSOG_movingblock"]);
            ItemAPI.FakePrefab.MarkAsFakePrefab(Original);
            DontDestroyOnLoad(Original);
            var tearHolder = Original.GetComponent<FadingBlocker>();
            JToken value = null;
            float GUID = jObject.TryGetValue("delay_", out value) ? ((float)value) : 0.25f;
            tearHolder.DelayWaitTime = GUID;
            int waveClearReq = jObject.TryGetValue("waveTriggerOn", out value) ? ((int)value) : 1;
            tearHolder.WaveClearRequirement = waveClearReq;
            return Original;
        }

        private RoomHandler roomHandler;
        public void Start()
        {
            AssignFloor();
            spriteAnimator.Play(AnimationKey + "_movingblock_idle");
            IntVector2 intVec2 = new IntVector2((int)this.transform.position.x, (int)this.transform.position.y);
            roomHandler = GameManager.Instance.Dungeon.GetRoomFromPosition(intVec2);

            this.Invoke("Inv", 0.1f);
            if (roomHandler != null)
            {
                if (WaveClearRequirement == -1)
                {
                    roomHandler.OnEnemiesCleared += OnEnemiesCleared;
                }
                else
                {
                    Actions.OnReinforcementWaveTriggered += DoWacky;
                }
            }


            this.spriteAnimator.AnimationEventTriggered += (obj1, obj2, obj3) =>
            {
                if (obj2.GetFrame(obj3).eventInfo == "disableCollider")
                {
                    sprite.SortingOrder = -2;
                    foreach (var entry in Bodies)
                    {
                        entry.enabled = false;
                    }
                    isLowered = true;
                    MarkCells(false);
                }
                if (obj2.GetFrame(obj3).eventInfo == "isNowLowering")
                {
                    isLowered = false;
                }
            };

            if (GameManager.Instance.Dungeon != null)
            {
                switch (GameManager.Instance.Dungeon.tileIndices.tilesetId)
                {
                    case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
                        cellFloorType =  CellVisualData.CellFloorType.Water;
                        particleColor = new Color(1f, 0, 0, 1f);
                        //sparksType = GlobalSparksDoer.SparksType.BLOODY_BLOOD;
                        ParticleSize = 0.1f;
                        break;
                    case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
                        cellFloorType = CellVisualData.CellFloorType.Ice;
                        particleColor = new Color(0.5f, 0.5f, 0.75f, 1f);
                        ParticleSize = 0.2f;

                        break;
                    case GlobalDungeonData.ValidTilesets.SEWERGEON:
                        cellFloorType = CellVisualData.CellFloorType.ThickGoop;
                        particleColor = new Color(0.3f, 0.5f, 0.3f, 1f);
                        break;
                    case GlobalDungeonData.ValidTilesets.FORGEGEON:
                        particleColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                        sparksType = GlobalSparksDoer.SparksType.SOLID_SPARKLES;
                        ParticleSize = 0.1f;
                        break;
                    case GlobalDungeonData.ValidTilesets.MINEGEON:
                        particleColor = new Color(0.6f, 0.3f, 0f, 1f);
                        ParticleSize = 0.15f;
                        break;
                    default:
                        cellFloorType = CellVisualData.CellFloorType.Stone;                     
                        break;
                }
            }
        }

        private void Inv()
        {
            MarkCells(true);
        }

        public void MarkCells(bool Occuiped)
        {
            PixelCollider primaryPixelCollider = this.Bodies[0].PrimaryPixelCollider;
            IntVector2 intVector = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.LowerLeft).ToIntVector2(VectorConversions.Floor);
            IntVector2 intVector2 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.UpperRight).ToIntVector2(VectorConversions.Floor);
            for (int i = intVector.x; i <= intVector2.x; i++)
            {
                for (int j = intVector.y; j <= intVector2.y; j++)
                {
                    if (GameManager.Instance.Dungeon.data.cellData[i][j] != null)
                    {
                        var cell = GameManager.Instance.Dungeon.data.cellData[i][j];
                        cell.isOccupied = Occuiped;
                        cell.forceDisallowGoop = Occuiped;
                        cell.PreventRewardSpawn = Occuiped;
                        cell.containsTrap = Occuiped;
                        cell.cellVisualData.floorType = Occuiped ? CellVisualData.CellFloorType.Stone : cellFloorType;
                    }
                }
            }
        }
        private bool isLowered = true;
        private float ParticleDelay = 0.05f;
        public void Update()
        {
            if (this.spriteAnimator.IsPlaying(AnimationKey + "_movingblock_godown") && isLowered == false)
            {
                ParticleDelay -= Time.deltaTime;
                if (ParticleDelay <= 0)
                {
                    ParticleDelay = 0.05f;
                    int selection = UnityEngine.Random.Range(0, 3);
                    Vector2 p_1 = Vector2.zero;
                    Vector2 p_2 = Vector2.zero;
                    switch (selection)
                    {
                        case 0:
                            p_1 = this.transform.position;
                            p_2 = this.transform.position + new Vector3(0, 1);
                            break;
                        case 1:
                            p_1 = this.transform.position;
                            p_2 = this.transform.position + new Vector3(1, 0);
                            break;
                        case 2:
                            p_1 = this.transform.position + new Vector3(1, 0);
                            p_2 = this.transform.position + new Vector3(1, 1);
                            break;
                    }
                    GlobalSparksDoer.DoRandomParticleBurst(1, p_1, p_2, Vector3.up, 1f, 0.15f, ParticleSize, 1, particleColor, sparksType);
                }            
            }
        }


        public void OnEnemiesCleared()
        {
            this.StartCoroutine(DestroyBlocker());

        }
        public IEnumerator DestroyBlocker()
        {  
            yield return new WaitForSeconds(DelayWaitTime);
            if (DestroysObjectOnBreak)
            {
                this.spriteAnimator.PlayAndDestroyObject(AnimationKey+"_movingblock_godown");
            }
            else
            {
                this.spriteAnimator.Play(AnimationKey + "_movingblock_godown");
            }
            yield break;
        }



        public void DoWacky(RoomHandler room, RoomEventTriggerCondition roomEventTriggerAction)
        {
            if (room == roomHandler && roomEventTriggerAction == RoomEventTriggerCondition.ON_ENEMIES_CLEARED)
            {
                WaveClearRequirement--;
                if (WaveClearRequirement == 0 && T == false)
                {
                    T = true;
                    this.StartCoroutine(DestroyBlocker());
                }
            }
        }

        private bool T;
        public float DelayWaitTime = 0.25f;
        public int WaveClearRequirement = -1;

        public string AnimationKey = "keep";
        public SpeculativeRigidbody[] Bodies;
        public bool DestroysObjectOnBreak = false;


        public CellVisualData.CellFloorType cellFloorType;
        public Color particleColor = Color.gray;
        public GlobalSparksDoer.SparksType sparksType = GlobalSparksDoer.SparksType.DARK_MAGICKS;
        public float ParticleSize = 0.125f;

        private void AssignFloor()
        {
            if (GameManager.Instance.Dungeon != null)
            {
                switch (GameManager.Instance.Dungeon.tileIndices.tilesetId)
                {
                    case GlobalDungeonData.ValidTilesets.CASTLEGEON:
                        AnimationKey = "keep";
                        break;
                    case GlobalDungeonData.ValidTilesets.GUNGEON:
                        AnimationKey = "proper";
                        break;
                    case GlobalDungeonData.ValidTilesets.MINEGEON:
                        AnimationKey = "mines";
                        break;
                    case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
                        AnimationKey = "hollow";
                        break;
                    case GlobalDungeonData.ValidTilesets.FORGEGEON:
                        AnimationKey = "forge";
                        break;
                    case GlobalDungeonData.ValidTilesets.HELLGEON:
                        AnimationKey = "hell";
                        break;
                    case GlobalDungeonData.ValidTilesets.SEWERGEON:
                        AnimationKey = "sewer";
                        break;
                    case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
                        AnimationKey = "abbey";
                        break;
                }
            }
        }
    }
}
