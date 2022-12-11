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
using SaveAPI;


namespace Planetside
{
    public class RoomDropModifier : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("Starting RoomDropModifier setup...");
            try
            {
                //new Hook(
                //typeof(RoomHandler).GetMethod("HandleRoomClearReward", BindingFlags.Instance | BindingFlags.Public),
                //typeof(RoomDropModifier).GetMethod("HandleRoomClearRewardHook", BindingFlags.Static | BindingFlags.Public));
            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish RoomDropModifier setup!");
                Debug.Log(e);
            }
        }

        public static void HandleRoomClearRewardHook(Action<RoomHandler> orig, RoomHandler self)
        {
            if (GameManager.Instance.IsFoyer || GameManager.Instance.InTutorial)
            {
                return;
            }
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST)
            {
                return;
            }

            if (PlanetsideReflectionHelper.ReflectGetField<bool>(typeof(RoomHandler), "m_hasGivenReward", self) == true)
            {
                return;
            }

            if (self.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.REWARD)
            {
                return;
            }

            //self.m_hasGivenReward = true;
            PlanetsideReflectionHelper.ReflectSetField(typeof(RoomHandler), "m_hasGivenReward", true, self);



            if (self.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS && self.area.PrototypeRoomBossSubcategory == PrototypeDungeonRoom.RoomBossSubCategory.FLOOR_BOSS)
            {
                PlanetsideReflectionHelper.InvokeMethod(typeof(RoomHandler), "HandleBossClearReward", self, new object[] {});
                return;
            }
            if (self.PreventStandardRoomReward)
            {
                return;
            }

            FloorRewardData currentRewardData = GameManager.Instance.RewardManager.CurrentRewardData;
            LootEngine.AmmoDropType ammoDropType = LootEngine.AmmoDropType.DEFAULT_AMMO;
            bool flag = LootEngine.DoAmmoClipCheck(currentRewardData, out ammoDropType);
            string path = (ammoDropType != LootEngine.AmmoDropType.SPREAD_AMMO) ? "Ammo_Pickup" : "Ammo_Pickup_Spread";
            float value = UnityEngine.Random.value;
            float num = currentRewardData.ChestSystem_ChestChanceLowerBound;
            float num2 = GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.Coolness) / 100f;
            float num3 = -(GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.Curse) / 100f);
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                num2 += GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.Coolness) / 100f;
                num3 -= GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.Curse) / 100f;
            }
            if (PassiveItem.IsFlagSetAtAll(typeof(ChamberOfEvilItem)))
            {
                num3 *= -2f;
            }
            num = Mathf.Clamp(num + GameManager.Instance.PrimaryPlayer.AdditionalChestSpawnChance, currentRewardData.ChestSystem_ChestChanceLowerBound, currentRewardData.ChestSystem_ChestChanceUpperBound) + num2 + num3;
            bool flag2 = currentRewardData.SingleItemRewardTable != null;
            bool flag3 = false;
            float num4 = 0.1f;
            if (!RoomHandler.HasGivenRoomChestRewardThisRun && MetaInjectionData.ForceEarlyChest)
            {
                flag3 = true;
            }
            if (flag3)
            {
                if (!RoomHandler.HasGivenRoomChestRewardThisRun && (GameManager.Instance.CurrentFloor == 1 || GameManager.Instance.CurrentFloor == -1))
                {
                    flag2 = false;
                    num += num4;
                    if (GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.NumRoomsCleared > 4)
                    {
                        num = 1f;
                    }
                }
                if (!RoomHandler.HasGivenRoomChestRewardThisRun && self.distanceFromEntrance < RoomHandler.NumberOfRoomsToPreventChestSpawning)
                {
                    GameManager.Instance.Dungeon.InformRoomCleared(false, false);
                    return;
                }
            }
            BraveUtility.Log("Current chest spawn chance: " + num, Color.yellow, BraveUtility.LogVerbosity.IMPORTANT);



            List<RoomDropModifierData> roomDropModifiersOverride = new List<RoomDropModifierData>();
            List<RoomDropModifierData> roomDropModifiersDefault = new List<RoomDropModifierData>();

            foreach (var l in roomDropModifierDatas)
            {
                if (l.overridePriority == RoomDropModifierData.OverridePriority.FULL_OVERRIDE) { roomDropModifiersOverride.Add(l); }
                if (l.overridePriority == RoomDropModifierData.OverridePriority.DEFAULT) { roomDropModifiersDefault.Add(l); }
            }

            if (value > num)
            {
                if (flag)
                {
                    IntVector2 bestRewardLocation = self.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.CameraCenter, true);

                    if (roomDropModifiersOverride.Count > 0)
                    {
                        var h = BraveUtility.RandomElement<RoomDropModifierData>(roomDropModifiersOverride);
                        if (h.CanOverride() == true)
                        {
                            var g = h.OverrideTable.SelectByWeight();
                            LootEngine.SpawnItem(g, bestRewardLocation.ToVector3(), Vector2.up, 1f, true, true, false);
                        }
                        else
                        {
                            LootEngine.SpawnItem((GameObject)BraveResources.Load(path, ".prefab"), bestRewardLocation.ToVector3(), Vector2.up, 1f, true, true, false);
                        }
                    }
                    else
                    {
                        LootEngine.SpawnItem((GameObject)BraveResources.Load(path, ".prefab"), bestRewardLocation.ToVector3(), Vector2.up, 1f, true, true, false);
                    }
                }
                GameManager.Instance.Dungeon.InformRoomCleared(false, false);
                return;
            }
            if (flag2)
            {
                float num5 = currentRewardData.PercentOfRoomClearRewardsThatAreChests;
                if (PassiveItem.IsFlagSetAtAll(typeof(AmazingChestAheadItem)))
                {
                    num5 *= 2f;
                    num5 = Mathf.Max(0.5f, num5);
                }
                flag2 = (UnityEngine.Random.value > num5);
            }
            if (flag2)
            {
                if (roomDropModifiersOverride.Count > 0)
                {

                    var h = BraveUtility.RandomElement<RoomDropModifierData>(roomDropModifiersOverride);
                    if (h.CanOverride() == true)
                    {
                        var g = h.OverrideTable.SelectByWeight();
                        UnityEngine.Debug.Log(g.name + "SPAWNED");
                        DebrisObject debrisObject = LootEngine.SpawnItem(g, self.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.CameraCenter, true).ToVector3() + new Vector3(0.25f, 0f, 0f), Vector2.up, 1f, true, true, false);
                        Exploder.DoRadialPush(debrisObject.sprite.WorldCenter.ToVector3ZUp(debrisObject.sprite.WorldCenter.y), 8f, 3f);
                        AkSoundEngine.PostEvent("Play_OBJ_item_spawn_01", debrisObject.gameObject);
                        GameManager.Instance.Dungeon.InformRoomCleared(true, false);
                    }
                    else
                    {
                        ExtraBajangle(roomDropModifiersDefault, currentRewardData, self);
                    }
                }
                else
                {
                    ExtraBajangle(roomDropModifiersDefault, currentRewardData, self);
                }
            }
            else
            {
                IntVector2 bestRewardLocation = self.GetBestRewardLocation(new IntVector2(2, 1), RoomHandler.RewardLocationStyle.CameraCenter, true);
                if (GameStatsManager.Instance.IsRainbowRun == true)
                {
                    LootEngine.SpawnBowlerNote(GameManager.Instance.RewardManager.BowlerNoteChest, bestRewardLocation.ToCenterVector2(), self, true);
                    RoomHandler.HasGivenRoomChestRewardThisRun = true;
                }
                else
                {
                    Chest exists = self.SpawnRoomRewardChest(null, bestRewardLocation);
                    if (exists)
                    {
                        RoomHandler.HasGivenRoomChestRewardThisRun = true;
                    }
                }
                GameManager.Instance.Dungeon.InformRoomCleared(true, true);
            }
            if (flag)
            {
                IntVector2 bestRewardLocation = self.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.CameraCenter, true);
                LootEngine.DelayedSpawnItem(1f, (GameObject)BraveResources.Load(path, ".prefab"), bestRewardLocation.ToVector3() + new Vector3(0.25f, 0f, 0f), Vector2.up, 1f, true, true, false);
            }
        }

        public static void ExtraBajangle(List<RoomDropModifierData> roomDropModifiersDefault, FloorRewardData currentRewardData, RoomHandler self)
        {
            float num6 = (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER) ? GameManager.Instance.RewardManager.SinglePlayerPickupIncrementModifier : GameManager.Instance.RewardManager.CoopPickupIncrementModifier;
            GameObject gameObject;
            if (UnityEngine.Random.value < 1f / num6)
            {
                var dummy = RoomDropModifierData.CreateDummyTable();
                dummy.includedLootTables.Add(currentRewardData.SingleItemRewardTable);

                foreach (var h in roomDropModifiersDefault)
                {
                    if (h.CanOverride() == true) 
                    {
                        dummy.includedLootTables.Add(h.OverrideTable); 
                    }

                }
                gameObject = dummy.SelectByWeight(false);
            }
            else
            {
                gameObject = ((UnityEngine.Random.value >= 0.9f) ? GameManager.Instance.RewardManager.FullHeartPrefab.gameObject : GameManager.Instance.RewardManager.HalfHeartPrefab.gameObject);
            }

            UnityEngine.Debug.Log(gameObject.name + "SPAWNED");
            DebrisObject debrisObject = LootEngine.SpawnItem(gameObject, self.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.CameraCenter, true).ToVector3() + new Vector3(0.25f, 0f, 0f), Vector2.up, 1f, true, true, false);
            Exploder.DoRadialPush(debrisObject.sprite.WorldCenter.ToVector3ZUp(debrisObject.sprite.WorldCenter.y), 8f, 3f);
            AkSoundEngine.PostEvent("Play_OBJ_item_spawn_01", debrisObject.gameObject);
            GameManager.Instance.Dungeon.InformRoomCleared(true, false);
        }


        public static List<RoomDropModifierData> roomDropModifierDatas = new List<RoomDropModifierData>();


        public class RoomDropModifierData
        {
            // The shop discount itself. This class controls how your discount should work, the price reduction amount and the purchase condition, and other things.
            /// <summary>
            /// The name of your discount. Mostly just for organization and other things.
            /// </summary>
            public string IdentificationKey = "RoomDropOverride";

            /// <summary>
            /// A function for your *condition* in which your discount will be active. Make sure to return it as TRUE when it should be active.
            /// </summary>
            public Func<bool> CanOverrideDropCondition;

            public GenericLootTable OverrideTable;

            public GameObject OverrideChest;


            public static GenericLootTable CreateDummyTable()
            {
                var lT = ScriptableObject.CreateInstance<GenericLootTable>();
                lT.defaultItemDrops = new WeightedGameObjectCollection()
                {
                    elements = new List<WeightedGameObject>()
                    {

                    }
                };
                lT.includedLootTables = new List<GenericLootTable>();
                lT.tablePrerequisites = new DungeonPrerequisite[] { };
                return lT;
            }

            /// <summary>
            /// A function that lets you give a *custom* price multipler, for more dynamic price reductions..
            /// </summary>
            public Func<float> OverrideChance;

            public enum OverridePriority
            {
                DEFAULT,
                FULL_OVERRIDE,
                CHEST_DEFAULT,
                CHEST_OVERRIDE
            };

            public OverridePriority overridePriority = OverridePriority.DEFAULT;


            /// Returns TRUE if your discount is active.
            /// </summary>
            public bool CanOverride()
            {
                if (CanOverrideDropCondition != null)
                {
                    return CanOverrideDropCondition();
                }
                return false;
            }
        }
    }
}
