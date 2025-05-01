using Alexandria.BreakableAPI;
using Alexandria.PrefabAPI;
using Dungeonator;
using GungeonAPI;
using Newtonsoft.Json.Linq;
using Pathfinding;
using Planetside.Static_Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Planetside.DungeonPlaceables
{
    public class TheHelpfulPad : MonoBehaviour
    {
        public static void Init()
        {
            List<int> frames = new List<int>();

            //enableCollider
            var keepAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("keep_movingfloor_pullup");
            foreach (var entry in keepAnim.frames) { if (!frames.Contains(entry.spriteId)) { StaticSpriteDefinitions.RoomObject_Sheet_Data.spriteDefinitions[entry.spriteId].AddOffset(new Vector2(0, -0.5f)); frames.Add(entry.spriteId); } }
            keepAnim.frames[7].triggerEvent = true;
            keepAnim.frames[7].eventInfo = "enableCollider";
            keepAnim.frames[1].triggerEvent = true;
            keepAnim.frames[1].eventAudio = "Play_Moondoor_close";

            keepAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("keep_movingfloor_pullup_bb");
            keepAnim.frames[7].triggerEvent = true;
            keepAnim.frames[7].eventInfo = "enableCollider";
            keepAnim.frames[1].triggerEvent = true;
            keepAnim.frames[1].eventAudio = "Play_Moondoor_close";

            //=============================================================================

            var properAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("proper_movingfloor_pullup");
            foreach (var entry in properAnim.frames) { if (!frames.Contains(entry.spriteId)) { StaticSpriteDefinitions.RoomObject_Sheet_Data.spriteDefinitions[entry.spriteId].AddOffset(new Vector2(0, -0.5f)); frames.Add(entry.spriteId); } }
            properAnim.frames[7].triggerEvent = true;
            properAnim.frames[7].eventInfo = "enableCollider";
            properAnim.frames[1].triggerEvent = true;
            properAnim.frames[1].eventAudio = "Play_Moondoor_close";

            properAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("proper_movingfloor_pullup_bb");
            properAnim.frames[7].triggerEvent = true;
            properAnim.frames[7].eventInfo = "enableCollider";
            properAnim.frames[1].triggerEvent = true;
            properAnim.frames[1].eventAudio = "Play_Moondoor_close";

            //=============================================================================

            var minesAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("mines_movingfloor_pullup");
            foreach (var entry in minesAnim.frames) { if (!frames.Contains(entry.spriteId)) { StaticSpriteDefinitions.RoomObject_Sheet_Data.spriteDefinitions[entry.spriteId].AddOffset(new Vector2(0, -0.5f)); frames.Add(entry.spriteId); } }
            minesAnim.frames[6].triggerEvent = true;
            minesAnim.frames[6].eventInfo = "enableCollider";
            minesAnim.frames[1].triggerEvent = true;
            minesAnim.frames[1].eventAudio = "Play_Moondoor_close";

            minesAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("mines_movingfloor_pullup_bb");
            minesAnim.frames[6].triggerEvent = true;
            minesAnim.frames[6].eventInfo = "enableCollider";
            minesAnim.frames[1].triggerEvent = true;
            minesAnim.frames[1].eventAudio = "Play_Moondoor_close";

            //=============================================================================

            var hollowAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("hollow_movingfloor_pullup");
            foreach (var entry in hollowAnim.frames) { if (!frames.Contains(entry.spriteId)) { StaticSpriteDefinitions.RoomObject_Sheet_Data.spriteDefinitions[entry.spriteId].AddOffset(new Vector2(0, -0.5f)); frames.Add(entry.spriteId); } }
            hollowAnim.frames[3].triggerEvent = true;
            hollowAnim.frames[3].eventInfo = "enableCollider";
            hollowAnim.frames[1].triggerEvent = true;
            hollowAnim.frames[1].eventAudio = "Play_Moondoor_close";

            hollowAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("hollow_movingfloor_pullup_bb");
            hollowAnim.frames[3].triggerEvent = true;
            hollowAnim.frames[3].eventInfo = "enableCollider";
            hollowAnim.frames[1].triggerEvent = true;
            hollowAnim.frames[1].eventAudio = "Play_Moondoor_close";

            //=============================================================================

            var forgeAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("forge_movingfloor_pullup");
            foreach (var entry in forgeAnim.frames) { if (!frames.Contains(entry.spriteId)) { StaticSpriteDefinitions.RoomObject_Sheet_Data.spriteDefinitions[entry.spriteId].AddOffset(new Vector2(0, -0.625f)); frames.Add(entry.spriteId); } }
            forgeAnim.frames[6].triggerEvent = true;
            forgeAnim.frames[6].eventInfo = "enableCollider";
            forgeAnim.frames[1].triggerEvent = true;
            forgeAnim.frames[1].eventAudio = "Play_CryoClose_01";

            forgeAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("forge_movingfloor_pullup_bb");
            forgeAnim.frames[6].triggerEvent = true;
            forgeAnim.frames[6].eventInfo = "enableCollider";
            forgeAnim.frames[1].triggerEvent = true;
            forgeAnim.frames[1].eventAudio = "Play_CryoClose_01";

            //=============================================================================

            var hellAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("hell_movingfloor_pullup");
            foreach (var entry in hellAnim.frames) { if (!frames.Contains(entry.spriteId)) { StaticSpriteDefinitions.RoomObject_Sheet_Data.spriteDefinitions[entry.spriteId].AddOffset(new Vector2(0, -0.5f)); frames.Add(entry.spriteId); } }
            hellAnim.frames[6].triggerEvent = true;
            hellAnim.frames[6].eventInfo = "enableCollider";
            hellAnim.frames[1].triggerEvent = true;
            hellAnim.frames[1].eventAudio = "Play_Moondoor_close";

            hellAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("hell_movingfloor_pullup_bb");
            hellAnim.frames[6].triggerEvent = true;
            hellAnim.frames[6].eventInfo = "enableCollider";
            hellAnim.frames[1].triggerEvent = true;
            hellAnim.frames[1].eventAudio = "Play_Moondoor_close";

            //=============================================================================

            var sewerAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("sewer_movingfloor_pullup");
            foreach (var entry in sewerAnim.frames) { if (!frames.Contains(entry.spriteId)) { StaticSpriteDefinitions.RoomObject_Sheet_Data.spriteDefinitions[entry.spriteId].AddOffset(new Vector2(0, -0.8125f)); frames.Add(entry.spriteId); } }
            sewerAnim.frames[6].triggerEvent = true;
            sewerAnim.frames[6].eventInfo = "enableCollider";
            sewerAnim.frames[1].triggerEvent = true;
            sewerAnim.frames[1].eventAudio = "Play_Moondoor_close";

            sewerAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("sewer_movingfloor_pullup_bb");
            sewerAnim.frames[6].triggerEvent = true;
            sewerAnim.frames[6].eventInfo = "enableCollider";
            sewerAnim.frames[1].triggerEvent = true;
            sewerAnim.frames[1].eventAudio = "Play_Moondoor_close";
            //=============================================================================

            var abbeyAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("abbey_movingfloor_pullup");
            foreach (var entry in abbeyAnim.frames) { if (!frames.Contains(entry.spriteId)) { StaticSpriteDefinitions.RoomObject_Sheet_Data.spriteDefinitions[entry.spriteId].AddOffset(new Vector2(0, -0.5625f)); frames.Add(entry.spriteId); } }
            abbeyAnim.frames[7].triggerEvent = true;
            abbeyAnim.frames[7].eventInfo = "enableCollider";
            abbeyAnim.frames[1].triggerEvent = true;
            abbeyAnim.frames[1].eventAudio = "Play_Moondoor_close";

            abbeyAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("abbey_movingfloor_pullup_bb");
            abbeyAnim.frames[7].triggerEvent = true;
            abbeyAnim.frames[7].eventInfo = "enableCollider";
            abbeyAnim.frames[1].triggerEvent = true;
            abbeyAnim.frames[1].eventAudio = "Play_Moondoor_close";
            //=============================================================================

            GameObject obj = PrefabBuilder.BuildObject("HelpfulPad");
            var tk2d = obj.AddComponent<tk2dSprite>();
            var animator = obj.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            tk2d.Collection = StaticSpriteDefinitions.RoomObject_Sheet_Data;
            tk2d.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("lil_pad_001"));
            tk2d.SortingOrder = -3;
            tk2d.HeightOffGround = -2;

            tk2d.renderer.sortingOrder = 10;
            tk2d.renderer.sortingLayerName = "Background";

            tk2d.IsPerpendicular = false;
            //tk2d.sprite.usesOverrideMaterial = true;      
            var body = obj.CreateFastBody(new IntVector2(16, 16), new IntVector2(0, 0), CollisionLayer.MovingPlatform, false, true);
            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(StaticShaders.FloorTileMaterial_Transparency);
            tk2d.renderer.material = mat;
            var pad = obj.AddComponent<TheHelpfulPad>();
            pad.speculativeRigidbody = body;
            pad.tk2DBaseSprite = tk2d;
            pad.spriteAnimator = animator;
            
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("PSOG_HelpfulPad", obj);
            StaticReferences.StoredRoomObjects.Add("PSOG_HelpfulPad", obj.gameObject);
            Alexandria.DungeonAPI.RoomFactory.OnCustomProperty += OnAction;
        }

        public static GameObject OnAction(string ObjName, GameObject Original, JObject jObject)
        {
            if (ObjName != "PSOG_HelpfulPad") { return Original; }
            Original = UnityEngine.Object.Instantiate(Alexandria.DungeonAPI.StaticReferences.customObjects["PSOG_HelpfulPad"]);
            ItemAPI.FakePrefab.MarkAsFakePrefab(Original);
            DontDestroyOnLoad(Original);
            var tearHolder = Original.GetComponent<TheHelpfulPad>();
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

            //tk2DBaseSprite.FlipX = UnityEngine.Random.value < 0.222f;

            roomHandler = this.transform.position.GetAbsoluteRoom();
            CheckCellsIsBottomBorder();
            AssignFloor();
            //speculativeRigidbody.enabled = false;
            //tk2DBaseSprite.renderer.enabled = false;
            this.Invoke("InitializeDelayed", 0.15f);
        }

        public void InitializeDelayed()
        {
            if (roomHandler != null)
            {

                if (WaveClearRequirement == -1)
                {
                    roomHandler.OnEnemiesCleared += () =>
                    {
                        this.StartCoroutine(DestroyBlocker());
                    };
                }
                else
                {
                    Actions.OnReinforcementWaveTriggered += DoWacky;
                }
            }
            this.spriteAnimator.AnimationEventTriggered += (obj1, obj2, obj3) =>
            {
                if (obj2.GetFrame(obj3).eventInfo == "enableCollider")
                {
                    speculativeRigidbody.enabled = true;
                    MarkCells();
                    //Debug.Log("aiee!");
                }
            };
            speculativeRigidbody.enabled = false;
            tk2DBaseSprite.renderer.enabled = false;
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

        public IEnumerator DestroyBlocker()
        {
            yield return new WaitForSeconds(DelayWaitTime);
            tk2DBaseSprite.renderer.enabled = true;
            this.spriteAnimator.Play(AnimationKey + "_movingfloor_pullup" + (isBottomBorder ? "_bb" : string.Empty));
            yield break;
        }




        public void MarkCells()
        {

            IntVector2 b = base.transform.position.IntXY(VectorConversions.Floor);
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    IntVector2 key = new IntVector2(i, j) + b;
                    CellData cellData = GameManager.Instance.Dungeon.data[key];
                    cellData.forceAllowGoop = true;
                    cellData.isOccupied = false;
                    cellData.fallingPrevented = true;
                    cellData.containsTrap = false;
                    cellData.IsTrapZone = false;
                    cellData.type = CellType.FLOOR;
                    Pathfinder.Instance.RecalculateRoomClearances(roomHandler);

                    //Pathfinder.Instance.m_nodes.Where(self => self.Position == key).FirstOrDefault().CellData


                }
            }

            /*
            PixelCollider primaryPixelCollider = this.speculativeRigidbody.PrimaryPixelCollider;
            IntVector2 intVector = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.LowerLeft).ToIntVector2(VectorConversions.Floor);
            IntVector2 intVector2 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.UpperRight).ToIntVector2(VectorConversions.Floor);
            for (int i = intVector.x; i <= intVector2.x; i++)
            {
                for (int j = intVector.y; j <= intVector2.y; j++)
                {
                    if (GameManager.Instance.Dungeon.data.cellData[i][j] != null)
                    {
                        List<SpeculativeRigidbody> list = GameManager.Instance.Dungeon.data.cellData[i][j].platforms;
                        if (list == null)
                        {
                            list = new List<SpeculativeRigidbody>();
                            GameManager.Instance.Dungeon.data.cellData[i][j].platforms = list;
                        }
                        if (!list.Contains(this.speculativeRigidbody))
                        {
                            list.Add(this.speculativeRigidbody);
                        }
                        GameManager.Instance.Dungeon.data.cellData[i][j].forceAllowGoop = true;
                        GameManager.Instance.Dungeon.data.cellData[i][j].isOccupied = false;
                        GameManager.Instance.Dungeon.data.cellData[i][j].type = CellType.PIT;
                        GameManager.Instance.Dungeon.data.cellData[i][j].fallingPrevented = true;
                        //roomHandler.UpdateCellVisualData(i, j);
                        //Debug.Log("gg!");

                    }
                }
            }
            */
        }
        public void CheckCellsIsBottomBorder()
        {
            PixelCollider primaryPixelCollider = this.speculativeRigidbody.PrimaryPixelCollider;
            IntVector2 intVector = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.LowerLeft).ToIntVector2(VectorConversions.Floor);
            IntVector2 intVector2 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.UpperRight).ToIntVector2(VectorConversions.Floor);
            for (int i = intVector.x; i <= intVector2.x; i++)
            {
                for (int j = intVector.y; j <= intVector2.y; j++)
                {
                    var c = GameManager.Instance.Dungeon.data.cellData[i][j - 1];
                    if (c != null)
                    {
                        if (c.type == CellType.FLOOR)
                        {
                            isBottomBorder = true;
                        }
                    }
                }
            }
        }
        private bool isBottomBorder = false;

        private void OnEnterTrigger(SpeculativeRigidbody obj, SpeculativeRigidbody source, CollisionData collisionData)
        {
            if (obj.gameActor && obj.gameActor is PlayerController)
            {
                //tk2DBaseSprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("lil_pad_002"));
            }
        }
        private void OnExitTrigger(SpeculativeRigidbody obj, SpeculativeRigidbody source)
        {
            if (obj && obj.gameActor && obj.gameActor is PlayerController)
            {
                //tk2DBaseSprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("lil_pad_001"));
            }
        }

        private string AnimationKey = "keep";
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

        public SpeculativeRigidbody speculativeRigidbody;
        public tk2dBaseSprite tk2DBaseSprite;
        public tk2dSpriteAnimator spriteAnimator;

    }



}
