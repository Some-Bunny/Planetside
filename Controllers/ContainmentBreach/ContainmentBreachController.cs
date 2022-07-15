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
                };

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

                RoomFactory.AddInjection(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/PrisonUnlockRoom.room").room, "Prison Containment Shrine", flowModifierPlacementTypes, 0, dungeonPrerequisites, "Prison Containment Shrine", 1, 1f);
                RoomFactory.AddInjection(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/MortalCombat.room").room, "Combat Shrine", flowModifierPlacementTypes, 0, dungeonPrerequisites1, "Combat Shrine", 1, 1f);

                Actions.PostDungeonTrueStart += PostFloorgen;

                Debug.Log("Finished ContainmentBreachController setup without failure!");
            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish ContainmentBreachController setup!");
                Debug.Log(e);
            }
        }

     
        public static void PostFloorgen(Dungeon dungeon)
        {
            if (CurrentState == States.ALLOWED)
            {                
                dungeon.DungeonFloorName = GameUIRoot.Instance.GetComponent<dfLanguageManager>().GetValue(dungeon.DungeonFloorName) + "?";
                dungeon.DungeonFloorLevelTextOverride = "Mixed Chamber";
                var deco = dungeon.decoSettings;
                deco.ambientLightColor = new Color(0.05f, 0.15f, 0.9f);
                deco.generateLights = true;
                deco.ambientLightColorTwo = new Color(0.05f, 0.15f, 0.9f);
                deco.lowQualityAmbientLightColor = new Color(0.05f, 0.15f, 0.9f);
                deco.lowQualityAmbientLightColorTwo = new Color(0.05f, 0.15f, 0.9f);
                dungeon.PlayerIsLight = true;
                dungeon.PlayerLightColor = Color.cyan;
                dungeon.PlayerLightIntensity = 2;
                dungeon.PlayerLightRadius = 3;
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
        private void ResetFloorSpecificData()
        {
            if (CurrentState == States.ALLOWED)
            {
                SaveAPIManager.RegisterStatChange(CustomTrackedStats.INFECTION_FLOORS_ACTIVATED, 1);

                CurrentState = States.ENABLED;
                bool ShopPlaced = false;
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
                                GameObject shopObj = DungeonPlaceableUtility.InstantiateDungeonPlaceable(obj, shope.GetAbsoluteParentRoom(), new IntVector2((int)shope.gameObject.transform.position.x + (7), (int)shope.gameObject.transform.position.y) - shope.GetAbsoluteParentRoom().area.basePosition, false);
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
                                        FakePrefab.MarkAsFakePrefab(ItemPoint);
                                        UnityEngine.Object.DontDestroyOnLoad(ItemPoint);
                                        ItemPoint.SetActive(true);
                                        posList.Add(ItemPoint.transform);
                                    }
                                    shopCont.spawnPositions = posList.ToArray();

                                    foreach (var pos in shopCont.spawnPositions)
                                    {
                                        pos.parent = shopObj.gameObject.transform;
                                    }
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
                    bool muncjyflag = muncher != null && muncher.Length != 0;
                    if (muncjyflag)
                    {
                        foreach (GunberMuncherController shope in muncher)
                        {
                            Minimap.Instance.DeregisterRoomIcon(roomHandler, (GameObject)ResourceCache.Acquire("Global Prefabs/Minimap_Muncher_Icon"));
                            //Minimap.Instance.RegisterRoomIcon
                            GameObject obj = new GameObject();
                            RoomHandler roomIn = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
                            StaticReferences.StoredRoomObjects.TryGetValue("VoidMuncher", out obj);
                            GameObject shopObj = DungeonPlaceableUtility.InstantiateDungeonPlaceable(obj, roomIn, new IntVector2((int)shope.gameObject.transform.position.x + (7), (int)shope.gameObject.transform.position.y) - roomIn.area.basePosition, false);
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
                                //shopController.
                            }
                            Destroy(shope.gameObject);
                        }
                    }
                }
            }

            else if (CurrentState == States.ENABLED)
            {
                CurrentState = States.DISABLED;
            }
        }
    }
}
