using Alexandria.PrefabAPI;
using Brave.BulletScript;
using BreakAbleAPI;
using Dungeonator;
using Newtonsoft.Json.Linq;
using Planetside.Static_Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;
using static Planetside.PrisonerSecondSubPhaseController;

namespace Planetside.DungeonPlaceables
{
    public class BuriedObject
    {
        public static void CreateBuriedTreasure()
        {
            Dungeon mines = DungeonDatabase.GetOrLoadByName("base_mines");
            var TrapDoorObject = mines.RatTrapdoor.GetComponent<ResourcefulRatMinesHiddenTrapdoor>();
            var objSpr = mines.RatTrapdoor.GetComponent<tk2dSprite>();


            var buriedObject = PrefabBuilder.BuildObject("Test_Buried_Object").AddComponent<BuriedObjectController>();
            buriedObject.Tiles = new IntVector2(2, 2);
            buriedObject.gameObject.layer = objSpr.gameObject.layer;

            /*
            var place = BreakableAPI_Bundled.GenerateDungeonPlaceable(new Dictionary<GameObject, float>()
            {
                { buriedObject.gameObject, 1 }
            }, 2, 2);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("buriedobject", place);
            */

            var buriedObjectTile = PrefabBuilder.BuildObject("HiddenGroundTile").AddComponent<BuriedObjectTile>();


            var sprite = buriedObjectTile.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("emptyTile_Buried"));
            sprite.SortingOrder = objSpr.SortingOrder;
            sprite.renderLayer = objSpr.renderLayer;
            buriedObjectTile.gameObject.layer = objSpr.gameObject.layer;
            
            var BlendMaterial = new Material(TrapDoorObject.BlendMaterial);
            BlendMaterial.SetFloat("_BlendMin", 0);
            //BlendMaterial.SetTexture("_MainTex", sprite.renderer.material.mainTexture);
            
            
            sprite.renderer.material = BlendMaterial;
            buriedObjectTile.BlendMaterial = BlendMaterial;
            buriedObjectTile.sprite = sprite;
            BuriedObjectController.buriedObjectTilePrefab = buriedObjectTile;

            var amorPickup = PrefabBuilder.BuildObject("Test_Buried_Object").AddComponent<BuriedPickup>();
            amorPickup.Tiles = new IntVector2(2, 2);
            amorPickup.gameObject.layer = objSpr.gameObject.layer;
            amorPickup.RevealednessBeforeFullReveal = 0.4f;

            var place2 = BreakableAPI_Bundled.GenerateDungeonPlaceable(new Dictionary<GameObject, float>()
            {
                { amorPickup.gameObject, 1 }
            }, 2, 2);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("pickupburied", amorPickup.gameObject);

            Alexandria.DungeonAPI.RoomFactory.OnCustomProperty += OnAction;
        }
        public static GameObject OnAction(string ObjName, GameObject Original, JObject jObject)
        {
            if (ObjName != "pickupburied") { return Original; }

            var clone = UnityEngine.Object.Instantiate(Alexandria.DungeonAPI.StaticReferences.customObjects["pickupburied"]);
            ItemAPI.FakePrefab.MarkAsFakePrefab(clone);
            UnityEngine.Object.DontDestroyOnLoad(clone);

            var tearHolder = clone.GetComponent<BuriedPickup>();
            JToken value = null;

            string GUID = jObject.TryGetValue("PickupType", out value) ? ((string)value) : "PickupType";
            switch (GUID)
            {
                case "Armor":
                    tearHolder.OverridePickup = Pickups.Armor;
                    break;
                case "Key":
                    tearHolder.OverridePickup = Pickups.Key;
                    break;
                case "HalfHeart":
                    tearHolder.OverridePickup = Pickups.Half_Heart;
                    break;
                case "Heart":
                    tearHolder.OverridePickup = Pickups.Heart;
                    break;
                case "Blank":
                    tearHolder.OverridePickup = Pickups.Blank;
                    break;
                case "Ammo":
                    tearHolder.OverridePickup = Pickups.Ammo;
                    break;
                case "AmmoSplit":
                    tearHolder.OverridePickup = Pickups.Spread_Ammo;
                    break;
                case "GlassGuon":
                    tearHolder.OverridePickup = Pickups.Glass_Guon_Stone;
                    break;
                case "Casing5":
                    tearHolder.OverridePickup = Pickups.Casing_5;
                    break;
                case "Casing50":
                    tearHolder.OverridePickup = Pickups.Casing_50;
                    break;
            }

            float x = jObject.TryGetValue("percFill", out value) ? ((float)value) : 0.75f;
            tearHolder.RevealednessBeforeFullReveal = x;

            /*
            tearHolder.EnemyGUIDToSpawn = GUID;

            float x = jObject.TryGetValue("offsetx", out value) ? ((float)value) : 0;
            float y = jObject.TryGetValue("offsety", out value) ? ((float)value) : -2;

            float speed = jObject.TryGetValue("SpeedMultEnemy", out value) ? ((float)value) : 0.75f;
            float cooldown = jObject.TryGetValue("CooldownMultEnemy", out value) ? ((float)value) : 1f;

            tearHolder.EnemySpawnOffset = new Vector2(x, y);
            tearHolder.EnemyMovementSpeedMult = speed;
            tearHolder.EnemyAttackSpeedMult = cooldown;
            */


            return clone;

        }



        public static BuriedObjectController objectController;
        public class BuriedObjectController : DungeonPlaceableBehaviour, IPlaceConfigurable
        {
            public static BuriedObjectTile buriedObjectTilePrefab;
            public IntVector2 Tiles;

            public BuriedObjectTile[] TileInstances;
            public float RevealednessBeforeFullReveal = 0.33f;

            //public bool InstantUncover = false;
            public void LateUpdate()
            {
                if (isFullyRevealed)
                {
                    return;
                }
                float c = GetAverageRevealedness();
                //if (c == -1) { return; }

                if (TileInstances != null)
                {

                    for (int i = 0; i < TileInstances.Length; i++)
                    {
                        TileInstances[i].DoUpdate();
                    }
                }

                if (!this.m_revealing)
                {
                    if (c > RevealednessBeforeFullReveal)
                    {
                        if (this.m_revealing)
                        {
                            return;
                        }
                        m_revealing = true;
                        this.StartCoroutine(GraduallyReveal());
                        /*
                        for (int i = 0; i < TileInstances.Length; i++)
                        {
                            TileInstances[i].StartCoroutine(TileInstances[i].GraduallyReveal());
                        }
                        */
                    }
                }
                if (this.m_revealing)
                {
                    return;
                }
                //Debug.Log(c);
                UpdatePlayerPositions(c);
            }



            public bool isFinishedReveal = false;
            public IEnumerator GraduallyReveal()
            {
  
                this.m_revealing = true;
                float t = 0;
                bool b = false;
                while (!b)
                {


                    if (GetAverageRevealedness() >= 1)
                    {
                        break;
                    }

                    int am = 0;
                    UpdatePlayerDustups();
                    t += Mathf.Max(Tiles.x, Tiles.y) * 12 * Time.deltaTime;
                    foreach (var entry in TileInstances)
                    {
                        if (entry.CalcAvgRevealedness() >= 1)
                        {
                            am++;
                            continue;
                        }
                        Vector2 vector = this.transform.position + new Vector3((float)Tiles.x * 0.5f, (float)Tiles.y * 0.5f) -  entry.transform.position;
                        IntVector2 pxCenter = new IntVector2(Mathf.FloorToInt(vector.x * 16), Mathf.FloorToInt(vector.y * 16));
                        if (entry.SoftUpdateRadius(pxCenter, Mathf.CeilToInt(t), 100))
                        {
                            entry.m_blendTexDirty = true;
                        }
                    }

                    if (am >= TileInstances.Length)
                    {
                        b = true;
                    }

                    /*
                    this.RevealPercentage = Mathf.Clamp01(this.RevealPercentage + Time.deltaTime);
                    this.UpdatePlayerDustups();
                    //this.BlendMaterial.SetFloat("_BlendMin", this.RevealPercentage);


                    for (int i = 0; i < 16; i++)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            Color color = Color.black;
                            color.r = this.m_blendTexColors[(j * 64) + i].r;
                            color.r += Time.deltaTime;
                            Color color1 = Color.black;
                            this.m_blendTexColors[(j * 64) + i] = color;


                            if (color.r < 1)
                            {
                                m_blendTexDirty = true;
                            }
                        }
                    }
                    if (this.m_blendTexDirty)
                    {
                        this.m_blendTex.SetPixels(this.m_blendTexColors);
                        this.m_blendTex.Apply();
                    }
                    */

                    yield return null;
                }
                UpdatePlayerDustups();
                isFinishedReveal = true;
                OnFullReveal();
                isFullyRevealed = true;
                yield break;
            }



            private bool isFullyRevealed = false;
            private bool m_revealing = false;
            public float Leniency = 0;

            private bool isLastStandingOn = false;

            private void UpdatePlayerPositions(float AverageRevealedness)
            {
                if (AverageRevealedness >= 1)
                {
                    return;
                }
                for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
                {
                    PlayerController playerController = GameManager.Instance.AllPlayers[i];
                    Vector2 a = playerController.SpriteBottomCenter;
                    bool flag = false;



                    bool XCheck = a.x > (base.transform.position.x - Leniency) && a.x < ((base.transform.position.x + Tiles.x) + Leniency);
                    bool YCheck = a.y > (base.transform.position.y - Leniency) && a.y < ((base.transform.position.y + Tiles.y) + Leniency);

                    if (XCheck && YCheck && (playerController.IsGrounded || playerController.IsFlying) && !playerController.IsGhost)
                    {
                        flag = true;
                        isLastStandingOn = true;


                        playerController.OverrideDustUp = (ResourceCache.Acquire("Global VFX/VFX_RatDoor_DustUp") as GameObject);
                        if (playerController.Velocity.magnitude > 0f)
                        {
                            foreach (var entry in TileInstances)
                            {
                                Vector2 vector = a - entry.transform.position.XY();
                                IntVector2 pxCenter = new IntVector2(Mathf.FloorToInt(vector.x * 16), Mathf.FloorToInt(vector.y * 16));
                                if (entry.SoftUpdateRadius(pxCenter, 4, 2f * Time.deltaTime))
                                {
                                    entry.m_blendTexDirty = true;
                                }
                                //Debug.Log("AIEEEE");
                            }


                        }
                    }
                    if (!flag && isLastStandingOn)
                    {
                        isLastStandingOn = false;
                        playerController.OverrideDustUp = null;
                    }
                }
            }

            private bool inited = false;
            private bool FullInit = false;
            public void ConfigureOnPlacement(RoomHandler roomHandler)
            {
                if (InitializeStart_ != null)
                    { return; }
                InitializeStart_ = this.StartCoroutine(InitializeStart());
            }
            private Coroutine InitializeStart_;
            public static List<BuriedObjectController> allBurieds = new List<BuriedObjectController>();

            public void Start()
            {
                if (InitializeStart_ != null)
                { return; }
                InitializeStart_ = this.StartCoroutine(InitializeStart());
            }

            public IEnumerator InitializeStart()
            {
                allBurieds.Add(this);
                //Debug.Log($"S: {this.transform.position} | {this.transform.position.GetAbsoluteRoom()}");

                if (inited)         
                {
                    yield break;
                }
                while (Dungeon.IsGenerating)
                {
                    yield return null;
                }

                

                yield return null;
                yield return null;
                yield return null;
                yield return null;
                yield return null;
                while (allBurieds[0] != this)
                {
                    yield return null;
                }
                //Debug.Log($"{this.transform.position} | {this.transform.position.GetAbsoluteRoom()}");

                inited = true;
                IntVector2 b = base.transform.position.IntXY(VectorConversions.Floor);
                TileInstances = new BuriedObjectTile[Tiles.x * Tiles.y];
                for (int x = 0; x < Tiles.x; x++)
                {
                    for (int y = 0; y < Tiles.y; y++)
                    {
                        IntVector2 key = new IntVector2(x, y) + b;
                        CellData cellData = GameManager.Instance.Dungeon.data[key];
                        var tile = UnityEngine.Object.Instantiate<BuriedObjectTile>(buriedObjectTilePrefab, this.transform);
                        tile.transform.position = key.ToVector3();
                        TileInstances[(x * Tiles.y) + y] = tile;
                        tile.CreateTile(key);
                    }
                }
                OnControllerSpawned();
                for (int x = 0; x < RandomObjectsToSpawn; x++)
                {
                    OnHintObjectRoll(x);
                }
                yield return null;
                allBurieds.Remove(this);
                yield break;
            }

            public int RandomObjectsToSpawn = 3;


            public void UpdatePlayerDustups()
            {
                for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
                {
                    PlayerController playerController = GameManager.Instance.AllPlayers[i];
                    if (playerController && playerController.OverrideDustUp && playerController.OverrideDustUp.name.StartsWith("VFX_RatDoor_DustUp", StringComparison.Ordinal))
                    {
                        playerController.OverrideDustUp = null;
                    }
                }
            }

            private float GetAverageRevealedness()
            {
                if (this.TileInstances == null)
                {
                    return -1;
                }
                float amount = 0f;


                for (int i = 0; i < TileInstances.Length; i++)
                {
                    amount += TileInstances[i].CalcAvgRevealedness();
                }
                return amount / (float)TileInstances.Length;
            }


            public virtual void OnControllerSpawned()
            {

            }
            public virtual void OnHintObjectRoll(int num)
            {

            }
            public virtual void OnFullReveal()
            {

            }


            public void Awake()
            {
                Actions.OnConsumableBlank += OnBlank;
                Actions.OnExplosionCompleted += OnNearbyExplosion;
            }

            public override void OnDestroy()
            {
                base.OnDestroy();
                Actions.OnConsumableBlank -= OnBlank;
                Actions.OnExplosionCompleted -= OnNearbyExplosion;
            }

            public void OnBlank(PlayerController player)
            {
                if (GameManager.Instance.BestActivePlayer.CurrentRoom == base.transform.position.GetAbsoluteRoom())
                {
                    foreach (var entry in TileInstances)
                    {
                        IntVector2 pxCenter = new IntVector2(8, 8);
                        if (entry.SoftUpdateRadius(pxCenter, 64, 1000))
                        {
                            entry.m_blendTexDirty = true;
                        }
                    }
                }
            }
            public void OnNearbyExplosion(Exploder exploder, ExplosionData explosionData, Vector3 center)
            {
                foreach (var entry in TileInstances)
                {

                    Vector2 vector = center - entry.transform.position;
                    IntVector2 pxCenter = new IntVector2(Mathf.FloorToInt(vector.x * 16), Mathf.FloorToInt(vector.y * 16));
                    if (entry.SoftUpdateRadius(pxCenter, Mathf.FloorToInt(explosionData.damageRadius * 16), explosionData.damage * 5))
                    {
                        entry.m_blendTexDirty = true;
                    }
                }
            }
            public float ExplosionReactDistance = 8;

        }
        public class BuriedObjectTile : MonoBehaviour
        {
            public void CreateTile(IntVector2 Position)
            {
                Texture2D texture = new Texture2D(64, 64);
                texture.filterMode = FilterMode.Point;
                this.CopyFloorTex = texture;
                var map = GameManager.Instance.Dungeon.MainTilemap;

                CellData cell = GameManager.Instance.Dungeon.data[Position];
                int ID = map.GetTileIdAtPosition(this.transform.position, GlobalDungeonData.floorLayerIndex);




                int colonthree = -1;

                map.Layers[GlobalDungeonData.patternLayerIndex].GetRawTileValue(Position.x, Position.y, ref colonthree);
                if (colonthree == -1)
                    map.Layers[GlobalDungeonData.objectStampLayerIndex].GetRawTileValue(Position.x, Position.y, ref colonthree);

                if (colonthree == -1)
                    map.Layers[GlobalDungeonData.decalLayerIndex].GetRawTileValue(Position.x, Position.y, ref colonthree);

                if (colonthree == -1)
                    map.Layers[GlobalDungeonData.floorLayerIndex].GetRawTileValue(Position.x, Position.y, ref colonthree);

                texture.SetPixels(0, 0, 16, 16, map.spriteCollection.spriteDefinitions[colonthree].DesheetTexture().GetPixels(), 0);

                texture.SetPixels(0, 0, 16, 16, map.spriteCollection.spriteDefinitions[colonthree].DesheetTexture().GetPixels(), 0);




                texture.wrapMode = TextureWrapMode.Clamp;
                texture.Apply();

                RevealPercentage = 0;

                this.m_blendTex = new Texture2D(64, 64, TextureFormat.RGBA32, false);
                this.m_blendTexColors = new Color[4096];

                for (int i = 0; i < this.m_blendTexColors.Length; i++)
                {
                    this.m_blendTexColors[i] = Color.black;
                }
                this.m_blendTex.SetPixels(this.m_blendTexColors);
                this.m_blendTex.Apply();

                var mainTex = this.sprite.renderer.material.mainTexture;
                this.BlendMaterial = new Material(BlendMaterial);

                
                this.BlendMaterial.SetFloat("_BlendMin", 0);
                this.BlendMaterial.SetTexture("_BlendTex", this.m_blendTex);
                this.BlendMaterial.SetVector("_BaseWorldPosition", new Vector4(base.transform.position.x, base.transform.position.y, base.transform.position.z, 0f));

                this.BlendMaterial.SetTexture("_SubTex", CopyFloorTex);
                sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("emptyTile_Buried"));
                this.BlendMaterial.SetTexture("_MainTex", this.sprite.renderer.material.mainTexture);

                this.sprite.renderer.material = this.BlendMaterial;

            }

            public void DoUpdate()
            {
                if (this.RevealPercentage < 1f)
                {
                    this.UpdateGoopedCells();
                }

            }

            public void LateUpdate()
            {
                if (this.RevealPercentage < 1f)
                {
                    if (this.m_blendTexDirty)
                    {
                        //Debug.Log("AIEEE");
                        this.m_blendTex.SetPixels(this.m_blendTexColors);
                        this.m_blendTex.Apply();

                    }
                }
            }


            public float CalcAvgRevealedness()
            {
                if (this.RevealPercentage >= 1f)
                {
                    return 1f;
                }
                float num = 0f;
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        float r = this.m_blendTexColors[j * 64 + i].r;
                        num += Mathf.Max(r, this.RevealPercentage);
                    }
                }
                return num / (float)256f;
            }




            public void OnDestroy()
            {

            }









            public bool SoftUpdateRadius(IntVector2 pxCenter, int radius, float amt)
            {
                bool result = false;
                var _ = pxCenter.ToVector2();

                for (int i = (pxCenter.x - radius); i < (pxCenter.x + radius); i++)
                {
                    for (int j = (pxCenter.y - radius); j < (pxCenter.y + radius); j++)
                    {
                        if (i >= 0 && j >= 0 && i < 16 && j < 16)
                        {
                            //Debug.Log($"{i} / {j}");
                            Color color = this.m_blendTexColors[(j * 64) + i];
                            float num = Vector2.Distance(_, new Vector2((float)i, (float)j));
                            float num2 = Mathf.Clamp01(((float)radius - num) / (float)radius);
                            float num3 =  Mathf.Min(Mathf.Ceil(color.r + amt * num2), 1);

                            if (num3 != color.r)
                            {
                                color.r = num3;
                                //Debug.Log(num3);


                                this.m_blendTexColors[(j * 64) + i] = color;
                                result = true;
                                this.m_blendTexDirty = true;
                            }
                        }
                    }
                }
                return result;
            }

            private void UpdateGoopedCells()
            {
                if (this.RevealPercentage >= 1f)
                {
                    return;
                }
                Vector2 b = base.transform.position.XY();


                float c_i = 4;
                float c_j = 4;


                for (int i = 0; i < c_i; i++)
                {
                    for (int j = 0; j < c_j; j++)
                    {
                        Vector2 a = new Vector2((float)i / 4f, (float)j / 4f) + b;
                        IntVector2 intVector = (a / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
                        if (DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(intVector) && !this.m_goopedSpots.Contains(intVector))
                        {
                            this.m_goopedSpots.Add(intVector);
                            IntVector2 intVector2 = new IntVector2(i * 4, j * 4);
                            for (int k = intVector2.x; k < intVector2.x + 4; k++)
                            {
                                for (int l = intVector2.y; l < intVector2.y + 4; l++)
                                {
                                    this.m_blendTexColors[l * 64 + k] = new Color(1f, 1f, 1f, 1f);
                                }
                            }
                            this.m_blendTexDirty = true;
                        }
                    }
                }
            }












            //public TileIndexGrid OverridePitGrid;

            public Material BlendMaterial;

            public Texture2D CopyFloorTex;


            public float ExplosionReactDistance = 8f;


            [NonSerialized]
            public float RevealPercentage;

            public tk2dSprite sprite;

            private Texture2D m_blendTex;

            private Color[] m_blendTexColors;

            public bool m_blendTexDirty;

            private HashSet<IntVector2> m_goopedSpots = new HashSet<IntVector2>();

            private float m_timeHovering;

            private bool m_revealing;
        }

        public class BuriedPickup : BuriedObjectController
        {
            private GameObject MinimapIconInst;
            private PickupObject m_healthPickup;
            private int oldLayer;
            private int oldGameObjectLayer;
            private int oldSortingOrder;

            public Material BuriedMaterial;

            public PickupObject OverridePickup = null;
            public GameObject AmmoShadow;

            public override void OnControllerSpawned()
            {
                FloorRewardData currentRewardData = GameManager.Instance.RewardManager.CurrentRewardData;
                GameObject data = currentRewardData.SingleItemRewardTable.SelectByWeight(false);

                int failsafe = 100;
                while (failsafe > 0)
                {
                    failsafe--;
                    data = currentRewardData.SingleItemRewardTable.SelectByWeight(false);
                    if (data.gameObject != Pickups.Hegemony_Credit.gameObject | data.gameObject != Pickups.Map.gameObject)
                    {
                        break;
                    }
                }



                m_healthPickup = UnityEngine.Object.Instantiate<PickupObject>(OverridePickup ?? data.GetComponent(typeof(PickupObject)) as PickupObject, this.transform.position + new Vector3(0.75f, 0.75f), Quaternion.identity);
                m_healthPickup.IgnoredByRat = true;
                if (m_healthPickup.specRigidbody)
                {
                    m_healthPickup.specRigidbody.enabled = false;
                }
                var _ = gameObject.GetComponent<SquishyBounceWiggler>();
                if (_)
                    _.enabled = false;

                var __ = gameObject.GetComponent<tk2dSpriteAnimator>();
                if (__)
                {
                    __.enabled = false;
                    __.Stop();
                }


                if (m_healthPickup is HealthPickup health)
                {
                    MinimapIconInst = health.minimapIcon;
                    health.minimapIcon = null;
                    //GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor)).DeregisterInteractable(health);
                    health.enabled = false;
                }
                if (m_healthPickup is KeyBulletPickup key)
                {
                    MinimapIconInst = key.minimapIcon;
                    key.minimapIcon = null;
                    key.enabled = false;
                }
                if (m_healthPickup is PassiveItem item)
                {
                    MinimapIconInst = item.minimapIcon;
                    item.minimapIcon = null;
                    item.m_pickedUp = false;
                    //GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor)).DeregisterInteractable(item);
                    item.enabled = false;
                    item.GetRidOfMinimapIcon();
                }
                if (m_healthPickup is SilencerItem blank)
                {
                    MinimapIconInst = blank.minimapIcon;
                    blank.minimapIcon = null;
                    blank.m_pickedUp = false;
                    var playerItem = (blank as PlayerItem);
                    playerItem.GetRidOfMinimapIcon();
                    blank.GetRidOfMinimapIcon();

                    blank.ForceAsExtant = true;
                    blank.enabled = false;
                }
                if (m_healthPickup is AmmoPickup ammo)
                {
                    ammo.enabled = false;
                    MinimapIconInst = ammo.minimapIcon;
                    ammo.minimapIcon = null;
                    ammo.m_pickedUp = false;
                    AmmoShadow = ammo.transform.GetChild(0).gameObject;
                    AmmoShadow.SetActive(false);
                }
                if (m_healthPickup is IounStoneOrbitalItem prbital)
                {
                    prbital.minimapIcon = null;
                    prbital.m_pickedUp = false;
                    (prbital as PassiveItem).GetRidOfMinimapIcon();
                    prbital.enabled = false;
                }
                if (m_healthPickup is CurrencyPickup money)
                {
                    money.enabled = false;

                }


                oldLayer = m_healthPickup.sprite.renderLayer;
                oldGameObjectLayer = m_healthPickup.gameObject.layer;
                oldSortingOrder = m_healthPickup.sprite.renderer.sortingOrder;

                BuriedMaterial = m_healthPickup.sprite.renderer.material;


                m_healthPickup.sprite.renderer.sortingOrder = TileInstances[0].sprite.SortingOrder + 1;
                m_healthPickup.sprite.renderLayer = TileInstances[0].sprite.renderLayer;
                m_healthPickup.gameObject.layer = TileInstances[0].gameObject.layer;
                m_healthPickup.renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                m_healthPickup.renderer.receiveShadows = false;


                m_healthPickup.sprite.usesOverrideMaterial = true;
                var mar = new Material(StaticShaders.Default_Shader_Basic);
                mar.mainTexture = m_healthPickup.sprite.renderer.material.mainTexture;
                m_healthPickup.sprite.renderer.material = mar;
                SpriteOutlineManager.RemoveOutlineFromSprite(m_healthPickup.sprite);

            }

            public override void OnFullReveal()
            {
                if (MinimapIconInst)
                {
                    if (m_healthPickup is HealthPickup health)
                    {
                        RoomHandler.unassignedInteractableObjects.Remove(health);
                        health.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
                        health.m_minimapIconRoom.RegisterInteractable(health);
                        health.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(health.m_minimapIconRoom, MinimapIconInst, false);
                        health.enabled = true;
                        health.Start();
                    }
                    if (m_healthPickup is KeyBulletPickup key)
                    {
                        key.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
                        key.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(key.m_minimapIconRoom, MinimapIconInst, false);
                        key.enabled = true;
                        key.Start();
                    }
                    if (m_healthPickup is PassiveItem item)
                    {
                        RoomHandler.unassignedInteractableObjects.Remove(item);
                        item.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
                        item.m_minimapIconRoom.RegisterInteractable(item);
                        item.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(item.m_minimapIconRoom, MinimapIconInst, false);
                        item.enabled = true;
                        item.Start();
                    }
                    if (m_healthPickup is SilencerItem blank)
                    {
                        RoomHandler.unassignedInteractableObjects.Remove(blank);
                        blank.minimapIcon = MinimapIconInst;
                        blank.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
                        blank.m_minimapIconRoom.RegisterInteractable(blank);
                        //blank.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(blank.m_minimapIconRoom, MinimapIconInst, false);
                        blank.enabled = true;
                        //blank.Start();
                    }
                    if (m_healthPickup is AmmoPickup ammo)
                    {
                        RoomHandler.unassignedInteractableObjects.Remove(ammo);
                        ammo.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
                        ammo.m_minimapIconRoom.RegisterInteractable(ammo);
                        //ammo.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(ammo.m_minimapIconRoom, MinimapIconInst, false);
                        ammo.Start();
                        ammo.enabled = true;
                        AmmoShadow.SetActive(true);
                    }

                }
                else
                {
                    if (m_healthPickup is IounStoneOrbitalItem prbital)
                    {
                        RoomHandler.unassignedInteractableObjects.Remove(prbital);
                        prbital.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
                        prbital.m_minimapIconRoom.RegisterInteractable(prbital);
                        prbital.enabled = true;
                        //prbital.Start();
                    }
                }


                if (m_healthPickup is CurrencyPickup money)
                {
                    money.enabled = true;
                    money.Start();
                }


                m_healthPickup.sprite.renderer.sortingOrder++;
                if (m_healthPickup.specRigidbody)
                {
                    m_healthPickup.specRigidbody.enabled = true;
                    m_healthPickup.specRigidbody.Start();
                }

                var _ = gameObject.GetComponent<SquishyBounceWiggler>();
                if (_)
                    _.enabled = true;

                var __ = gameObject.GetComponent<tk2dSpriteAnimator>();
                if (__)
                {
                    __.enabled = true;
                    __.Play();
                }

                m_healthPickup.sprite.renderLayer = oldLayer;
                m_healthPickup.gameObject.layer = oldGameObjectLayer;
                m_healthPickup.sprite.renderer.sortingOrder = oldSortingOrder;

                m_healthPickup.sprite.renderer.material = BuriedMaterial;

                LootEngine.DoDefaultItemPoof(m_healthPickup.sprite.WorldCenter, false, false);
            }
        }




    }
}
