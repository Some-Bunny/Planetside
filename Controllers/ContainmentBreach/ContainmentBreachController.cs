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
using GungeonAPI;
using static EnemyBulletBuilder.BulletBuilderFakePrefabHooks;
using SaveAPI;
using NpcApi;
using static Dungeonator.ProceduralFlowModifierData;
using static Planetside.RealityTearData;
using Planetside.Controllers.ContainmentBreach.BossChanges.Misc;

namespace Planetside
{
    public class ContainmentBreachController : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("Starting ContainmentBreachController setup...");
            try
            {
                CurrentState = States.DISABLED;
                DungeonHooks.OnPostDungeonGeneration += this.ResetFloorSpecificData;

                GameManager.Instance.OnNewLevelFullyLoaded += Instance_OnNewLevelFullyLoaded;
                ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.InfectionChanges));

                List<ProceduralFlowModifierData.FlowModifierPlacementType> flowModifierPlacementTypes = new List<ProceduralFlowModifierData.FlowModifierPlacementType>()
                { ProceduralFlowModifierData.FlowModifierPlacementType.RANDOM_NODE_CHILD};
                List<DungeonPrerequisite> dungeonPrerequisites = new List<DungeonPrerequisite>()
                {
                    new DungeonGenToolbox.AdvancedDungeonPrerequisite
                    {
                        advancedPrerequisiteType = CustomDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_STAT_COMPARISION,
                        customStatToCheck = CustomTrackedStats.INFECTION_FLOORS_ACTIVATED,
                        useSessionStatValue = true,
                        prerequisiteOperation = DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO,
                        comparisonValue = 0
                    },
                    new DungeonPrerequisite()
                    {
                        saveFlagToCheck = GungeonFlags.BOSSKILLED_LICH,
                        requireFlag = true,
                        prerequisiteType = DungeonPrerequisite.PrerequisiteType.FLAG,
                        prerequisiteOperation = DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO,
                    },
                    new CustomDungeonPrerequisite()
                    {
                        customStatToCheck = CustomTrackedStats.PERKS_BOUGHT,
                        comparisonValue = 11,
                        prerequisiteOperation = DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN,
                        useSessionStatValue = false,
                        advancedPrerequisiteType = CustomDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_STAT_COMPARISION
                    },
                    new CustomDungeonPrerequisite()
                    {
                        customFlagToCheck = CustomDungeonFlags.HAS_TREADED_DEEPER,
                        requireCustomFlag = true,
                        advancedPrerequisiteType = CustomDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG

                    },
                    new DungeonGenToolbox.AdvancedDungeonPrerequisite
                    {
                        advancedAdvancedPrerequisiteType = DungeonGenToolbox.AdvancedDungeonPrerequisite.AdvancedAdvancedPrerequisiteType.MULTIPLE_FLOORS,
                        validTilesets = new List<GlobalDungeonData.ValidTilesets>() {GlobalDungeonData.ValidTilesets.CASTLEGEON, GlobalDungeonData.ValidTilesets.GUNGEON, GlobalDungeonData.ValidTilesets.SEWERGEON, GlobalDungeonData.ValidTilesets.CATHEDRALGEON, GlobalDungeonData.ValidTilesets.MINEGEON, GlobalDungeonData.ValidTilesets.CATACOMBGEON}
                    },
                };
                RoomFactory.AddInjection(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/PrisonUnlockRoom.room").room, "Prison Containment Shrine", flowModifierPlacementTypes, 0, dungeonPrerequisites, "Prison Containment Shrine", 1f, 0.1f);


                List<DungeonPrerequisite> dungeonPrerequisites1 = new List<DungeonPrerequisite>()
                {
                    new DungeonGenToolbox.AdvancedDungeonPrerequisite
                    {
                        advancedPrerequisiteType = CustomDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_STAT_COMPARISION,
                        customStatToCheck = CustomTrackedStats.INFECTION_FLOORS_ACTIVATED,
                        useSessionStatValue = true,
                        prerequisiteOperation = DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO,
                        comparisonValue = 1
                    },
                };
                ETGModMainBehaviour.Instance.gameObject.AddComponent<TheGames>();

                RoomFactory.AddInjection(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/MortalCombat.room").room, "Combat Shrine", flowModifierPlacementTypes, 0, dungeonPrerequisites1, "Combat Shrine", 1, 1f);

                Actions.PostDungeonTrueStart += PostFloorgen;

                ETGModConsole.Commands.AddGroup("psog_void", args =>
                {
                });
                ETGModConsole.Commands.GetGroup("psog_void").AddUnit("enable_mixed_floor", ForceEnableMixedFloor);

                //GameStatsManager.m_instance.IsInSession;
                //GameManager.Instance.

                Debug.Log("Finished ContainmentBreachController setup without failure!");
            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish ContainmentBreachController setup!");
                Debug.Log(e);
            }
        }

        public static void AddInjecto(GlobalDungeonData.ValidTilesets s)
        {
          
        }

        public static void ForceEnableMixedFloor(string[] s)
        {
            CurrentState = States.ALLOWED;
            SaveAPIManager.SetStat(CustomTrackedStats.INFECTION_FLOORS_ACTIVATED, 1);

        }


        public static void PostFloorgen(Dungeon dungeon)
        {
            if (CurrentState == States.ALLOWED)// && AdvancedGameStatsManager.Instance.GetSessionStatValue(CustomTrackedStats.INFECTION_FLOORS_ACTIVATED) == 1)
            {


                dungeon.DungeonFloorName = GameUIRoot.Instance.GetComponent<dfLanguageManager>().GetValue(dungeon.DungeonFloorName) + "?";
                dungeon.DungeonFloorLevelTextOverride = "Mixed Chamber";
                var deco = dungeon.decoSettings;

                float r = 0.03f; // im fucking lazy, okay?
                float g = 0.05f; // im fucking lazy, okay?
                float b = 0.17f; // im fucking lazy, okay?

                deco.ambientLightColor = new Color(0.05f / r, 0.15f / g, 0.9f / b);
                deco.generateLights = true;
                deco.ambientLightColorTwo = new Color(0.05f / r, 0.10f / g, 0.7f / b);
                deco.lowQualityAmbientLightColor = new Color(0.05f / r, 0.15f / g, 0.7f / b);
                deco.lowQualityAmbientLightColorTwo = new Color(0.05f / r, 0.15f / g, 0.7f / b);
                dungeon.PlayerIsLight = true;
                dungeon.PlayerLightColor = Color.blue;
                dungeon.PlayerLightIntensity = 2.5f;
                dungeon.PlayerLightRadius = 6.5f;
            }
        }
  
        private void Instance_OnNewLevelFullyLoaded()
        {
            InfectionReplacement.InitSpecialMods();
        }

        public void InfectionChanges(AIActor target)
        {
            if (CurrentState == States.ENABLED)
            {
                if (EnemyIsValid(target) == true)
                {

                    if (target.healthHaver.IsBoss || target.healthHaver.IsSubboss)
                    {
                        target.ApplyEffect(DebuffLibrary.InfectedBossEffect);
                    }
                    else
                    {
                        target.ApplyEffect(DebuffLibrary.InfectedEnemyEffect);
                    }
                }
            }
        }

        private static bool EnemyIsValid(AIActor aI)
        {
            if (aI == null) { return false; }
            if (aI.gameObject.activeSelf == false) { return false; }
            if (aI.IgnoreForRoomClear == true) { return false; }
            if (aI.gameObject.GetComponent<MirrorImageController>() != null) { return false; }
            if (aI.gameObject.GetComponent<DisplacedImageController>() != null) { return false; }
            if (aI.CompanionOwner != null) { return false; }
            if (aI.gameObject.GetComponent<CompanionController>() != null) { return false; }
            if (StaticInformation.ModderBulletGUIDs.Contains(aI.EnemyGuid)) { return false; }
            if (aI.EnemyGuid == "3f11bbbc439c4086a180eb0fb9990cb4") { return false; }
            if (EliteBlackListDefault.Contains(aI.EnemyGuid)) { return false; }
            if (aI.gameObject.GetComponent<ForgottenEnemyComponent>() != null) { return false; }
            return true;
        }

        private static List<string> EliteBlackListDefault = new List<string>()
        {
            "deturretleft_enemy",
            "deturret_enemy",
            "fodder_enemy",
            EnemyGuidDatabase.Entries["mine_flayers_bell"],
            EnemyGuidDatabase.Entries["gunreaper"],
            EnemyGuidDatabase.Entries["key_bullet_kin"],
            EnemyGuidDatabase.Entries["chance_bullet_kin"],
            EnemyGuidDatabase.Entries["grip_master"],
            EnemyGuidDatabase.Entries["mine_flayers_claymore"],

            EnemyGuidDatabase.Entries["chicken"],
            EnemyGuidDatabase.Entries["snake"],
            EnemyGuidDatabase.Entries["poopulons_corn"],
            EnemyGuidDatabase.Entries["rat"],
            EnemyGuidDatabase.Entries["rat_candle"],
            EnemyGuidDatabase.Entries["dragun_egg_slimeguy"],
        };


        public static Chest ReplaceChestWithContainers(Func<RewardManager, IntVector2, RoomHandler, PickupObject.ItemQuality?, float, Chest> orig, RewardManager self, IntVector2 positionInRoom, RoomHandler targetRoom, PickupObject.ItemQuality? targetQuality, float overrideMimicChance)
        {
            if (CurrentState == States.ALLOWED)
            {
                GameObject g = new GameObject();
                StaticReferences.StoredRoomObjects.TryGetValue("trespassContainer", out g);
                g.GetComponent<DungeonPlaceableBehaviour>().InstantiateObject(targetRoom, positionInRoom + new IntVector2(0, -1), true);
                targetRoom.RegisterInteractable(g.GetInterfaceInChildren<IPlayerInteractable>());
                return null;
            }
            else
            {
                return orig(self, positionInRoom, targetRoom, targetQuality, overrideMimicChance);
            }
        }

        public static States CurrentState;
        public enum States
        {
            ALLOWED,
            ENABLED,
            DISABLED
        };


        public static int MasterTraderCustomPriceOverride(CustomShopController shop, CustomShopItemController itemCont, PickupObject item)
        {
            return 60;
        }

        public static bool MasterTraderCustomCanBuyOverride(CustomShopController shop, PlayerController player, int cost)
        {
            return player.carriedConsumables.Currency >= 60;
        }

        public static int MasterTraderRemoveCurrency(CustomShopController shop, PlayerController player, int cost)
        {
            player.carriedConsumables.Currency -= 60;
            SaveAPI.AdvancedGameStatsManager.Instance.RegisterStatChange(SaveAPI.CustomTrackedStats.PERKS_BOUGHT, 1);
            return 1;
        }

        public IEnumerator Delay(CustomShopController shop)
        {
            yield return null;
            var list = shop.m_itemControllers;
            foreach (var item in list)
            {
                if (item is CustomShopItemController customShopItem)
                {

                    customShopItem.customCanBuy = MasterTraderCustomCanBuyOverride;
                    customShopItem.customPrice = MasterTraderCustomPriceOverride;
                    customShopItem.removeCurrency = MasterTraderRemoveCurrency;
                    customShopItem.CurrencyType = CustomShopItemController.ShopCurrencyType.CUSTOM;
                    customShopItem.customPriceSprite = "ui_coin";
                    customShopItem.Initialize(customShopItem.item, shop);


                }
            }
            yield break;
        }


        private void ResetFloorSpecificData()
        {
            if (CurrentState == States.ALLOWED && AdvancedGameStatsManager.Instance.GetSessionStatValue(CustomTrackedStats.INFECTION_FLOORS_ACTIVATED) == 1)
            {


                SaveAPIManager.RegisterStatChange(CustomTrackedStats.INFECTION_FLOORS_ACTIVATED, 1);

                CurrentState = States.ENABLED;
                bool ShopPlaced = false;
                bool triggered = false;

                List<RoomHandler> rooms = GameManager.Instance.Dungeon.data.rooms;

                foreach (RoomHandler roomHandler in rooms)
                {
                    BaseShopController[] componentsInChildren = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<BaseShopController>(true);
                    bool flag3 = componentsInChildren != null && componentsInChildren.Length != 0;
                    if (flag3)
                    {
                        foreach (BaseShopController shope in componentsInChildren)
                        {
                            Minimap.Instance.DeregisterRoomIcon(roomHandler, shope.OptionalMinimapIcon);
                            List<ShopItemController> shopitem = PlanetsideReflectionHelper.ReflectGetField<List<ShopItemController>>(typeof(BaseShopController), "m_itemControllers", shope);
                            for (int i = 0; i < shopitem.Count; i++)
                            {
                                if (shopitem[i] && shopitem[i].IsResourcefulRatKey == false)
                                {
                                    Destroy(shopitem[i].gameObject);
                                }
                            }

                            if (ShopPlaced == false)
                            {
                                ShopPlaced = !ShopPlaced;
                                GameObject obj = new GameObject();
                                StaticReferences.StoredRoomObjects.TryGetValue("masteryRewardTrader", out obj);

                                bool b = GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON;
                                IntVector2 offset = b == true ? new IntVector2(1, -2) : new IntVector2(6, -2);



                                GameObject shopObj = DungeonPlaceableUtility.InstantiateDungeonPlaceable(obj, shope.GetAbsoluteParentRoom(), new IntVector2((int)shope.gameObject.transform.position.x + (offset.x), ((int)shope.gameObject.transform.position.y)- offset.y) - shope.GetAbsoluteParentRoom().area.basePosition, false);
                                CustomShopController shopCont = shopObj.GetComponent<CustomShopController>();
                                if (shopCont != null)
                                {
                                    List<Vector2> itemPositions = new List<Vector2>()
                                    {
                                   shopObj.transform.PositionVector2() +  new Vector2(-1f, -0.25f),
                                   shopObj.transform.PositionVector2() +  new Vector2(0.5f, -1f),
                                   shopObj.transform.PositionVector2() + new Vector2(2.25f, -1.5f),
                                   shopObj.transform.PositionVector2() +  new Vector2(4f, -1f),
                                   shopObj.transform.PositionVector2() +  new Vector2(5.5f, -0.25f)
                                    };
                                    var posList = new List<Transform>();
                                    for (int i = 0; i < itemPositions.Count; i++)
                                    {
                                        var ItemPoint = new GameObject("ItemPoint" + i);
                                        ItemPoint.transform.position = itemPositions[i];
                                        ItemPoint.SetActive(true);
                                        posList.Add(ItemPoint.transform);
                                    }
                                    shopCont.spawnPositions = posList.ToArray();
                                    shopCont.currencyType = CustomShopItemController.ShopCurrencyType.COINS;
                                    shopCont.customCanBuy = MasterTraderCustomCanBuyOverride;
                                    shopCont.customPrice = MasterTraderCustomPriceOverride;
                                    shopCont.removeCurrency = MasterTraderRemoveCurrency;
                                    //shopCont.
                                    foreach (var pos in shopCont.spawnPositions)
                                    {
                                        pos.parent = shopObj.gameObject.transform;
                                    }

                                    GameManager.Instance.StartCoroutine(Delay(shopCont));

                                    //var list = shopCont.m_itemControllers;
                                    /*
                                    foreach (var item in list)
                                    {
                                        ETGModConsole.Log(5);

                                        if (item is CustomShopItemController customShopItem)
                                        {
                                            customShopItem.customCanBuy = null;
                                            customShopItem.customPrice = null;
                                            customShopItem.removeCurrency = null;
                                            ETGModConsole.Log(6);

                                            customShopItem.customCanBuy = MasterTraderCustomCanBuyOverride;
                                            customShopItem.customPrice = MasterTraderCustomPriceOverride;
                                            customShopItem.removeCurrency = MasterTraderRemoveCurrency;
                                        }
                                        ETGModConsole.Log(7);

                                    }
                                    */
                                }
                            }
                            if (shope.OptionalMinimapIcon)
                            {
                                Minimap.Instance.DeregisterRoomIcon(roomHandler, shope.OptionalMinimapIcon);
                            }
                            Destroy(shope.gameObject);                    
                        }
                    }

                    GunberMuncherController[] muncher = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<GunberMuncherController>(true);
                    if (muncher != null && muncher.Length > 0)
                    {
                        foreach (GunberMuncherController shope in muncher)
                        {
                            if (triggered == false)
                            {
                                RoomHandler room = shope.transform.position.GetAbsoluteRoom();
                                List<GameObject> ICONS = new List<GameObject>();
                                var rTI = PlanetsideReflectionHelper.ReflectGetField<Dictionary<RoomHandler, List<GameObject>>>(typeof(Minimap), "roomToIconsMap", Minimap.Instance);
                                if (rTI.ContainsKey(room))
                                {
                                    rTI.TryGetValue(room, out ICONS);
                                    if (ICONS != null && ICONS.Count > 0)
                                    {
                                        for (int i = 0; i < ICONS.Count; i++)
                                        {
                                            if (ICONS[i].name.ToLower().Contains("muncher"))
                                            {
                                                Minimap.Instance.DeregisterRoomIcon(room, ICONS[i]);

                                            }
                                        }
                                    }                 
                                }

                                GameObject obj = new GameObject();
                                RoomHandler roomIn = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
                                StaticReferences.StoredRoomObjects.TryGetValue("VoidMuncher", out obj);
                                GameObject shopObj = DungeonPlaceableUtility.InstantiateDungeonPlaceable(obj, roomIn, new IntVector2((int)shope.gameObject.transform.position.x +1, (int)shope.gameObject.transform.position.y + 1) - roomIn.area.basePosition, false);
                                IPlayerInteractable[] interfaces = shopObj.GetInterfaces<IPlayerInteractable>();
                                for (int j = 0; j < interfaces.Length; j++)
                                {
                                    room.RegisterInteractable(interfaces[j]);
                                }
                                triggered = true;
                            }
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
                            //God fucking damnit i hate room icons so goddamn much
                            ShopController shopController = shope.GetComponent<ShopController>();
                            if (shopController != null)
                            {
                            }
                            if (!shope.gameObject.name.ToLower().Contains("jailed"))
                            {
                                if (!shope.gameObject.name.ToLower().Contains("bowlercell"))
                                {
                                    Destroy(shope.gameObject);
                                }
                            }
                        }
                    }
                }
            }
            else if (CurrentState == States.ENABLED)
            {
                CurrentState = States.DISABLED;
            }
            else
            {
                CurrentState = States.DISABLED;
            }
        }
    }
}
