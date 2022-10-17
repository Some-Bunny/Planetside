using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Collections;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;
using GungeonAPI;
using static Planetside.MultiActiveReloadManager;

namespace Planetside
{
    public class MasteryTraderSpawnController : MonoBehaviour
    {
        public void Start()
        {

            Debug.Log("Starting MasteryTraderSpawnController setup...");
            try
            {
                HasOpenedPortalOnFloor = false;

                new Hook(
                typeof(RoomHandler).GetMethod("HandleBossClearReward", BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(MasteryTraderSpawnController).GetMethod("HandleBossClearRewardHook", BindingFlags.Static | BindingFlags.Public));

                new Hook(
                typeof(PlayerController).GetMethod("StartDodgeRoll", BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(MasteryTraderSpawnController).GetMethod("StartDodgeRollHook", BindingFlags.Static | BindingFlags.Public));

                new Hook(
                typeof(PlayerController).GetMethod("BlinkToPoint", BindingFlags.Instance | BindingFlags.Public),
                typeof(MasteryTraderSpawnController).GetMethod("BlinkToPointHook", BindingFlags.Static | BindingFlags.Public));

                new Hook(
                typeof(PlayerController).GetMethod("Damaged", BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(MasteryTraderSpawnController).GetMethod("DamagedHook", BindingFlags.Static | BindingFlags.Public));



                DungeonHooks.OnPostDungeonGeneration += this.ClearFloorSpecificData;
                Debug.Log("Finished MasteryTraderSpawnController setup without failure!");

            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish MasteryTraderSpawnController setup!");
                Debug.Log(e);
            }  
        }


        public static bool StartDodgeRollHook(Func<PlayerController, Vector2, bool> orig, PlayerController self, Vector2 direction)
        {
            if (self.CurrentRoom != null && self.IsInCombat == true)
            {
                if (!roomsRolledIn.Contains(self.CurrentRoom) && !roomsHurtIn.Contains(self.CurrentRoom))
                {roomsRolledIn.Add(self.CurrentRoom);}
            }            
            return orig(self, direction);
        }

        public static void BlinkToPointHook(Action<PlayerController, Vector2> orig, PlayerController self, Vector2 direction)
        {
            orig(self, direction);
            if (self.CurrentRoom != null && self.IsInCombat == true)
            {
                if (!roomsHurtIn.Contains(self.CurrentRoom) && !roomsRolledIn.Contains(self.CurrentRoom))
                { roomsHurtIn.Add(self.CurrentRoom); }
            }
        }

        
        public static void DamagedHook(Action<PlayerController, Single, Single, CoreDamageTypes, DamageCategory, Vector2> orig, PlayerController self, float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            orig(self, resultValue, maxValue, damageTypes, damageCategory, damageDirection);
            if (self.IsInCombat == true && damageCategory != DamageCategory.Environment)
            {
                if (self.CurrentRoom!= null && !roomsHurtIn.Contains(self.CurrentRoom))
                {
                    roomsHurtIn.Add(self.CurrentRoom);
                }
            }
        }
        public static List<RoomHandler> roomsHurtIn = new List<RoomHandler>();
        public static List<RoomHandler> roomsRolledIn = new List<RoomHandler>();
        private void ClearFloorSpecificData()
        {
            roomsRolledIn.Clear();
            roomsHurtIn.Clear();
            HasOpenedPortalOnFloor = false;
        }

        public static bool CheckIfRoomValid(RoomHandler room, Dungeon currentFloor)
        {
            if (room.IsSecretRoom == true) { return false; }
            if (room.IsShop == true) { return false; }
            if (currentFloor.data.Entrance == room) { return false; }
            if (currentFloor.data.Exit == room) { return false; }
            if (room.EverHadEnemies == false) { return false; }
            if (room.visibility == RoomHandler.VisibilityStatus.VISITED || room.visibility == RoomHandler.VisibilityStatus.CURRENT) { return true; }
            return false;
        }

        public static bool FloorWhitelist(Dungeon floor)
        {
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON) { return true; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON) { return true; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON) { return true; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON) { return true; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON) { return true; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.SEWERGEON) { return true; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATHEDRALGEON) { return true; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON) { return true; }
            return false;
        }

        public static float FloorMultiplier(Dungeon floor)
        {
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON) { return 0.777f; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON) { return 0.825f; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON) { return 0.9f; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON) { return 1; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON) { return 1; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.SEWERGEON) { return 1f; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATHEDRALGEON) { return 1.1f; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON) { return 1.2f; }
            return 1f;
        }

        public static float GetMasteryMultiplier(PlayerController player)
        {
            float Multiplier = 0;
            foreach (PassiveItem item in player.passiveItems)
            {
                if (item is BasicStatPickup mastery)
                {
                    if (mastery.IsMasteryToken == true)
                    {
                        Multiplier += 0.033f;
                    }
                }
            }
            return Multiplier;
        }

        public static bool HasOpenedPortalOnFloor;


        public static bool PlayerHasAnchor()
        {
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                if (player.HasPickupID(StableVector.AnchorID)) { return true; }
            }
            return false;
        }

        public static void HandleBossClearRewardHook(Action<RoomHandler> orig, RoomHandler self)
        {
            orig(self);
            if (HasOpenedPortalOnFloor == false)
            {
                HasOpenedPortalOnFloor = true;
                Debug.Log("\nStarting Portal Chance Calculation! =======");
                var currentFloor = GameManager.Instance.Dungeon;
                if (FloorWhitelist(currentFloor) == true)
                {
                    int RoomCount = 0;
                    float RolledIn = 1;
                    float HurtIn = 0;

                    for (int j = 0; j < GameManager.Instance.Dungeon.data.rooms.Count; j++)
                    {
                        RoomHandler room = GameManager.Instance.Dungeon.data.rooms[j];
                        if (CheckIfRoomValid(room, currentFloor) == true)
                        {
                            RoomCount++;
                            if (!roomsRolledIn.Contains(room))
                            { RolledIn++; }
                            if (!roomsHurtIn.Contains(room))
                            { HurtIn++; }
                        }
                    }
                    Debug.Log("Room count used for calculation: " + RoomCount);
                    Debug.Log("Rooms NOT rolled in (Including +1 Leeway): " + RolledIn);
                    Debug.Log("Rooms NOT hurt in: " + HurtIn);



                    float Chance = Mathf.Min(1, RolledIn / RoomCount);
                    float ChanceMult = HurtIn / RoomCount;
                    float checc = Mathf.Min(1, ChanceMult * Chance);

                    if (checc == 1 | PlayerHasAnchor() == true)
                    {
                        Debug.Log("Opnening Guaranteed Portal!");
                        RoomHandler currentRoom = self;
                        GameObject partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal"));
                        MeshRenderer rend = partObj.GetComponent<MeshRenderer>();
                        rend.allowOcclusionWhenDynamic = true;
                        partObj.transform.position = currentRoom.area.Center - new Vector2(0, 2);
                        partObj.transform.localScale = Vector3.one;
                        partObj.name = "ShopPortal";
                        partObj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                        TrespassPortalController romas = partObj.AddComponent<TrespassPortalController>();
                        romas.m_room = currentRoom;
                    }
                    else
                    {
                        float MultMastery = 1f;
                        foreach (PlayerController player in GameManager.Instance.AllPlayers)
                        {
                            MultMastery += GetMasteryMultiplier(player);
                        }
                        Debug.Log("Mastery Multipler: " + MultMastery);

                        Debug.Log("Floor Multipler: " + FloorMultiplier(currentFloor));
                        Chance = Chance * (Chance * FloorMultiplier(currentFloor));
                        Chance *= MultMastery;

                        Debug.Log("Post Calculation Dodgeroll chance: " + Chance);

                        ChanceMult = ChanceMult * (ChanceMult * 1.1f);
                        Debug.Log("No Damage Chance: " + ChanceMult.ToString());

                        float Total = ChanceMult * Chance;
                        for (int L = 0; L < SaveAPI.AdvancedGameStatsManager.Instance.GetSessionStatValue(SaveAPI.CustomTrackedStats.PERKS_BOUGHT); L++)
                        {
                            Total *= 0.8f;
                        }
                        Total = Mathf.Min(1, Total);

                        Debug.Log("Total Chance: " + Total.ToString());
                        if (UnityEngine.Random.value < Total)
                        {
                            Debug.Log("Opnening Portal with " + Total + " Chance!");
                            RoomHandler currentRoom = self;
                            GameObject partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal"));
                            MeshRenderer rend = partObj.GetComponent<MeshRenderer>();
                            rend.allowOcclusionWhenDynamic = true;
                            partObj.transform.position = currentRoom.area.Center - new Vector2(0, 2);
                            partObj.transform.localScale = Vector3.one;
                            partObj.name = "ShopPortal";
                            partObj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                            TrespassPortalController romas = partObj.AddComponent<TrespassPortalController>();
                            romas.m_room = currentRoom;
                        }
                        else
                        {
                            Debug.Log("Portal failed to open with " + Total + " Chance!");
                        }
                        Debug.Log("Ending Portal Chance Calculation! =======\n");
                    }             
                }
            }       
        }
    }
}
