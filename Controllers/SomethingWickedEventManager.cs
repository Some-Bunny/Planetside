using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;

namespace Planetside
{
    public class SomethingWickedEventManager : MonoBehaviour
    {
        public SomethingWickedEventManager()
        {
            //IsFakeOut = false;
        }
        public static OverridableBool shouldBeDark = new OverridableBool(false);
        public static OverridableBool shouldBeLightOverride = new OverridableBool(false);
        //public static bool ShouldDoSomethingWickedEvent;


        private void Start()
        {
            Debug.Log("Starting SomethingWickedEventManager setup...");
            try
            {
                currentSWState = States.DISABLED;
                GameManager.Instance.OnNewLevelFullyLoaded += this.DoEventChecks;
                Actions.PostDungeonTrueStart += PostFloorgen;
                Actions.OnRunStart += OnRunStart;
                Debug.Log("Finished SomethingWickedEventManager setup without failure!");

            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish SomethingWickedEventManager setup!");
                Debug.Log(e);
            }
        }

        public void OnRunStart(PlayerController player_1, PlayerController player_2, GameManager.GameMode gameMode)
        {
            NevernamedsDarknessHandler.DisableDarkness(0);
            currentSWState = States.DISABLED;
            TrapDefusalKit.RemoveTrapDefuseOverride("somethingwicked");
        }


        public static void PostFloorgen(Dungeon dungeon)
        {
            if (currentSWState == States.ALLOWED)
            {
                dungeon.DungeonFloorName = GameUIRoot.Instance.GetComponent<dfLanguageManager>().GetValue(dungeon.DungeonFloorName) + "";
                dungeon.DungeonFloorLevelTextOverride = "Silenced Chamber";
                var deco = dungeon.decoSettings;
                deco.generateLights = false;
            }      
        }

        void DoEventChecks()
        {
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.RATGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.HELLGEON)
            {
                if (currentSWState == States.ALLOWED)
                {
                    bool returnedBool = HasPlacedAllShrines();
                    if (returnedBool == true)
                    {
                        currentSWState = States.ENABLED;
                        Minimap.Instance.PreventAllTeleports = true;
                        Minimap.Instance.TemporarilyPreventMinimap = true;
                        NevernamedsDarknessHandler.EnableDarkness(0, 0);
                        GameManager.Instance.StartCoroutine(DelaySW());
                        TrapDefusalKit.AddTrapDefuseOverride("somethingwicked");
                        StartSWEvent();
                    }
                    else
                    {
                        currentSWState = States.DISABLED;
                    }
                }
                else if (currentSWState == States.ENABLED)
                {
                    currentSWState = States.DISABLED;
                    NevernamedsDarknessHandler.DisableDarkness(0);
                    Minimap.Instance.TemporarilyPreventMinimap = false;
                    TrapDefusalKit.RemoveTrapDefuseOverride("somethingwicked");
                    ResetMusic(GameManager.Instance.Dungeon);

                }
                else if (currentSWState == States.DISABLED)
                {
                    NevernamedsDarknessHandler.DisableDarkness(0);
                    TrapDefusalKit.RemoveTrapDefuseOverride("somethingwicked");
                    ResetMusic(GameManager.Instance.Dungeon);
                }
            }
            else
            {
                NevernamedsDarknessHandler.DisableDarkness(0);
                currentSWState = States.DISABLED;
                TrapDefusalKit.RemoveTrapDefuseOverride("somethingwicked");
                ResetMusic(GameManager.Instance.Dungeon);
            }

            
        }


        public void StartSWEvent()
        {
            List<RoomHandler> rooms = GameManager.Instance.Dungeon.data.rooms;
            foreach (RoomHandler roomHandler in rooms)
            {
                try
                {
                    roomHandler.ClearReinforcementLayers();
                    /*
                    List<AIActor> activeEnemies = roomHandler.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
                    if (activeEnemies != null && activeEnemies.Count > 0)
                    {
                        for (int i = 0; i < activeEnemies.Count; i++)
                        {
                            if (activeEnemies[i] != null)
                            {
                                roomHandler.DeregisterEnemy(activeEnemies[i], false);
                                UnityEngine.Object.Destroy(activeEnemies[i].gameObject);
                            }
                        }
                    }
                    */
                    List<AIActor> e = PlanetsideReflectionHelper.ReflectGetField<List<AIActor>>(typeof(RoomHandler), "activeEnemies", roomHandler);
                    if (e != null && e.Count > 0)
                    {
                        for (int i = 0; i < e.Count; i++)
                        {
                            if (e[i] != null)
                            {
                                UnityEngine.Object.Destroy(e[i].gameObject);
                            }
                        }
                    }

                    ReadOnlyCollection<IPlayerInteractable> yes = roomHandler.GetRoomInteractables();
                    for (int i = 0; i < yes.Count; i++)
                    {
                        IPlayerInteractable touchy = yes[i];
                        if (touchy is TeleporterController interaactableObj)
                        {
                            roomHandler.DeregisterInteractable(interaactableObj);
                            interaactableObj.enabled = false;
                            Minimap minimap = Minimap.Instance;
                            Dictionary<RoomHandler, GameObject> rTTIM = PlanetsideReflectionHelper.ReflectGetField<Dictionary<RoomHandler, GameObject>>(typeof(Minimap), "roomToTeleportIconMap", minimap);
                            rTTIM.Clear();
                            minimap.roomsContainingTeleporters.Remove(roomHandler);
                            Minimap.Instance.DeregisterRoomIcon(roomHandler, interaactableObj.gameObject);
                        }
                    }


                    BaseShopController[] componentsInChildren = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<BaseShopController>(true);
                    bool flag3 = componentsInChildren != null && componentsInChildren.Length != 0;
                    if (flag3)
                    {

                        foreach (BaseShopController shope in componentsInChildren)
                        {
                            List<ShopItemController> shopitem = PlanetsideReflectionHelper.ReflectGetField<List<ShopItemController>>(typeof(BaseShopController), "m_itemControllers", shope);
                            for (int i = 0; i < shopitem.Count; i++)
                            {
                                if (shopitem[i])
                                {
                                    Destroy(shopitem[i]);
                                }
                            }
                            Destroy(shope.gameObject);
                            if (shope.OptionalMinimapIcon)
                            {
                                Minimap.Instance.DeregisterRoomIcon(roomHandler, shope.OptionalMinimapIcon);
                            }
                        }
                    }
                    GunberMuncherController[] muncher = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<GunberMuncherController>(true);
                    bool muncjyflag = muncher != null && muncher.Length != 0;
                    if (muncjyflag)
                    {
                        foreach (GunberMuncherController shope in muncher)
                        {
                            Destroy(shope.gameObject);
                        }
                    }

                    SellCellController[] sellcreep = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<SellCellController>(true);
                    bool sellcreepflag = sellcreep != null && sellcreep.Length != 0;
                    if (sellcreepflag)
                    {
                        foreach (SellCellController shope in sellcreep)
                        {
                            Destroy(shope.gameObject);
                        }
                    }
                    TalkDoerLite[] talkers = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<TalkDoerLite>(true);
                    bool talkersflag = talkers != null && talkers.Length != 0;
                    if (talkersflag)
                    {
                        foreach (TalkDoerLite shope in talkers)
                        {
                            Destroy(shope.gameObject);
                        }
                    }

                }
                catch (Exception e)
                {
                    ETGModConsole.Log("Catastrophic Failure In Modifying Floor for Something Wicked Event\nSend A Screenshot of this and associated error in F3 Console.\n");
                    ETGModConsole.Log(e.ToString());
                }
            }
        }

        private bool PlaceSpecificShrine(RoomHandler specRoom, string objName, Vector2 offset)
        {
            try
            {
                IntVector2 randomVisibleClearSpot = specRoom.GetCenterCell();
                bool flag4 = randomVisibleClearSpot != IntVector2.Zero;
                if (flag4)
                {
                    GameObject original;
                    GungeonAPI.ShrineFactory.registeredShrines.TryGetValue("psog:" + objName, out original);
                    GameObject gObj = UnityEngine.Object.Instantiate<GameObject>(original, new Vector3((float)randomVisibleClearSpot.x + offset.x, (float)randomVisibleClearSpot.y + offset.y), Quaternion.identity);
                    IPlayerInteractable[] interfaces1 = gObj.GetInterfaces<IPlayerInteractable>();
                    IPlaceConfigurable[] interfaces21 = gObj.GetInterfaces<IPlaceConfigurable>();
                    for (int j = 0; j < interfaces1.Length; j++)
                    {
                        specRoom.RegisterInteractable(interfaces1[j]);
                    }
                    for (int k = 0; k < interfaces21.Length; k++)
                    {
                        interfaces21[k].ConfigureOnPlacement(specRoom);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                ETGModConsole.Log("Catastrophic Failure In Placing Color Shrine:" + objName + "\nSend A Screenshot of this and associated error in F3 Console.\n");
                ETGModConsole.Log(e.ToString());
                return false;
            }
        }


        public bool HasPlacedAllShrines()
        {
            bool EntranceShrine = PlaceSpecificShrine(GameManager.Instance.Dungeon.data.Entrance, "redshrine", new Vector2(-0.75f, -8));
            bool ExitShrine = PlaceSpecificShrine(GameManager.Instance.Dungeon.data.Exit, "blueshrine", new Vector2(-0.75f, -1));
            if (EntranceShrine == true && ExitShrine == true)
            {
                return true;
            }
            return false;
        }

        public enum States
        {
            ALLOWED,
            ENABLED,
            DISABLED
        };

        public static States currentSWState;



        public void ResetMusic(Dungeon d)
        {
            bool flag = !string.IsNullOrEmpty(d.musicEventName);
            if (flag)
            {
                this.m_cachedMusicEventCore = d.musicEventName;
            }
            else
            {
                this.m_cachedMusicEventCore = "Play_MUS_Dungeon_Theme_01";
            }
            AkSoundEngine.PostEvent(m_cachedMusicEventCore, GameManager.Instance.gameObject);

        }
        private string m_cachedMusicEventCore;

        private void OnDestroy()
        {
            if (Pixelator.Instance.AdditionalCoreStackRenderPass != null)
            {
                Pixelator.Instance.AdditionalCoreStackRenderPass = null;
            }
        }

        private IEnumerator DelaySW()
        {
            float ela = 0f;
            while (ela < 10f)
            {
                ela += BraveTime.DeltaTime;
                yield return null;
            }
            if (currentSWState == States.ENABLED)
            {
                Vector2 Point1 = MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), 180f);
                UnityEngine.Object.Instantiate<GameObject>(SomethingWickedEnemy.SomethingWickedObject, GameManager.Instance.BestActivePlayer.gameObject.transform.PositionVector2() - Point1, Quaternion.identity);
                AkSoundEngine.PostEvent("Play_SomethingWickedThisWayComes", base.gameObject);
                ela = 0f;
                while (ela < 5f)
                {
                    ela += BraveTime.DeltaTime;
                    yield return null;
                }
                OtherTools.Notify("Something Wicked", "This Way Comes.", "Planetside/Resources/ShrineIcons/DarknessIcon");
            }

            yield break;
        }

        public void RemoveEnemies(AIActor AIActor)
        {
            bool flag = AIActor && AIActor.aiActor && AIActor.aiActor.EnemyGuid != null;
            if (flag)
            {
                string text;
                if (AIActor == null)
                {
                    text = null;
                }
                else
                {
                    AIActor aiActor = AIActor.aiActor;
                    text = ((aiActor != null) ? aiActor.EnemyGuid : null);
                }
                string text2 = text;
                bool flag2 = !string.IsNullOrEmpty(text2);
                if (flag2)
                {
                    try
                    {
                        if (AIActor.gameObject.GetComponent<SpawnEnemyOnDeath>())
                        {
                            UnityEngine.Object.Destroy(AIActor.gameObject.GetComponent<SpawnEnemyOnDeath>());
                        }
                        Destroy(AIActor.gameObject);
                    }
                    catch (Exception e)
                    {
                        ETGModConsole.Log(e.ToString());
                    }
                }
            }
        }

        private void Update()
        {
            if (currentSWState == States.ENABLED)
            {
                AkSoundEngine.PostEvent("Stop_MUS_All", base.gameObject);
                if (NevernamedsDarknessHandler.IsItDark() == false) { NevernamedsDarknessHandler.EnableDarkness(0, 0); }
            }
            if (GameManager.Instance.PrimaryPlayer == null) { currentSWState = States.DISABLED; }

        }
        //public static bool isDark = false;
        //public static Shader DarknessEffectShader;
    }
}