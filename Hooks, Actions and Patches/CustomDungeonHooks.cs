using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;

namespace Planetside
{ 
    public class CustomDungeonHooks : DungeonDatabase
    {
        public static void InitDungeonHook()
        {
            /*
            Hook GetOrLoadByNameHook = new Hook(
                typeof(DungeonDatabase).GetMethod("GetOrLoadByName", BindingFlags.Static | BindingFlags.Public),
                typeof(CustomDungeonHooks).GetMethod("GetOrLoadByNameHook", BindingFlags.Static | BindingFlags.Public)
            );
            */
            new Hook(
                  typeof(RoomHandler).GetMethod("AddProceduralTeleporterToRoom", BindingFlags.Instance | BindingFlags.Public),
                  typeof(CustomDungeonHooks).GetMethod("AddProceduralTeleporterToRoomHook", BindingFlags.Static | BindingFlags.Public)
              );
        }

        public static void AddProceduralTeleporterToRoomHook(Action<RoomHandler> orig, RoomHandler roomHandler)
        {
            if (GameManager.Instance.Dungeon.DungeonFloorName == "The Deep.") 
            {
                if (Minimap.Instance.HasTeleporterIcon(roomHandler))
                {
                    return;
                }
                GameObject objectToInstantiate = Alexandria.DungeonAPI.StaticReferences.customObjects["DeepTeleporter"];
                DungeonData dungeonData = GameManager.Instance.Dungeon.data;
                bool isStrict = true;
                Func<CellData, bool> canContainTeleporter = (CellData a) => a != null && !a.isOccupied && !a.doesDamage && !a.containsTrap && !a.IsTrapZone && !a.cellVisualData.hasStampedPath && (!isStrict || !a.HasPitNeighbor(dungeonData)) && a.type == CellType.FLOOR;
                roomHandler.ProcessTeleporterTiles(canContainTeleporter);
                Func<CellData, bool> isInvalidFunction = (CellData a) => a == null || !a.cachedCanContainTeleporter || a.parentRoom != roomHandler;
                Tuple<IntVector2, IntVector2> tuple = Carpetron.RawMaxSubmatrix(dungeonData.cellData, roomHandler.area.basePosition, roomHandler.area.dimensions, isInvalidFunction);
                if (tuple.Second.x < 3 || tuple.Second.y < 3)
                {
                    isStrict = false;
                    roomHandler.ProcessTeleporterTiles(canContainTeleporter);
                    tuple = Carpetron.RawMaxSubmatrix(dungeonData.cellData, roomHandler.area.basePosition, roomHandler.area.dimensions, isInvalidFunction);
                }
                BraveUtility.DrawDebugSquare(tuple.First.ToVector2(), tuple.Second.ToVector2(), Color.red, 1000f);
                if (tuple.Second.x >= 3 && tuple.Second.y >= 3)
                {
                    IntVector2 intVector = tuple.First;
                    IntVector2 intVector2 = tuple.Second - tuple.First;
                    int x = (intVector2.x % 2 != 1 && intVector2.x != 4) ? -1 : 0;
                    int y = (intVector2.y % 2 != 1 && intVector2.y != 4) ? -1 : 0;
                    while (intVector2.x > 3)
                    {
                        intVector.x++;
                        intVector2.x -= 2;
                    }
                    while (intVector2.y > 3)
                    {
                        intVector.y++;
                        intVector2.y -= 2;
                    }
                    intVector += new IntVector2(x, y);
                    GameObject gameObject = DungeonPlaceableUtility.InstantiateDungeonPlaceable(objectToInstantiate, roomHandler, intVector, false, AIActor.AwakenAnimationType.Default, false);
                    TeleporterController component = gameObject.GetComponent<TeleporterController>();
                    roomHandler.RegisterInteractable(component);
                }
                return;
            }

            if (roomHandler != null)
            {
                if (roomHandler.hierarchyParent != null)
                {
                    int ChildCount = roomHandler.hierarchyParent.childCount;

                    for (int i = 0; i < ChildCount; i++)
                    {
                        Transform childTransform = roomHandler.hierarchyParent.GetChild(i);
                        if (childTransform?.gameObject)
                        {
                            if (childTransform?.gameObject.GetComponent<NoTeleporterPlaceable.NoTeleporterPlaceableComponent>() != null) { return; }
                        }
                    }
                }
                
            }
            orig(roomHandler);
        }




        public static Dungeon GetOrLoadByNameHook(Func<string, Dungeon> orig, string name)
        {
            Dungeon dungeon = null;
            if (name.ToLower() == "base_cathedral")
            {
                dungeon = AbbeyDungeonMods(GetOrLoadByName_Orig(name));
            }
            else if (name.ToLower() == "base_sewer")
            {
                dungeon = SewerDungeonMods(GetOrLoadByName_Orig(name));
            }
            else if (name.ToLower() == "base_nakatomi")
            {
                dungeon = RNGDungeonMods(GetOrLoadByName_Orig(name));
            }
            if (dungeon)
            {
                DebugTime.RecordStartTime();
                DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[] { name });
                return dungeon;
            }
            else
            {
                return orig(name);
            }
        }

        public static Dungeon GetOrLoadByName_Orig(string name)
        {
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("dungeons/" + name.ToLower());
            DebugTime.RecordStartTime();
            Dungeon component = assetBundle.LoadAsset<GameObject>(name).GetComponent<Dungeon>();
            DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[] { name });
            return component;
        }

        public static Dungeon AbbeyDungeonMods(Dungeon dungeon)
        {
            // Here is where you'll do your mods to existing Dungeon prefab	
            dungeon.BossMasteryTokenItemId = ForgottenRoundAbbey.ForgottenRoundAbbeyID; // Item ID for Third Floor Master Round. Replace with Item ID of your choosing. 
            return dungeon;
        }
        public static Dungeon SewerDungeonMods(Dungeon dungeon)
        {
            // Here is where you'll do your mods to existing Dungeon prefab	
            var finalMasteryRewardOub = ForgottenRoundOubliette.ForgottenRoundOublietteID;
            dungeon.BossMasteryTokenItemId = finalMasteryRewardOub; // Item ID for Third Floor Master Round. Replace with Item ID of your choosing. 
            return dungeon;
        }
        public static Dungeon RNGDungeonMods(Dungeon dungeon)
        {
            // Here is where you'll do your mods to existing Dungeon prefab	
            var finalMasteryRewardOub = ForgottenRoundRNG.ForgottenRoundRNGID;
            dungeon.BossMasteryTokenItemId = finalMasteryRewardOub; // Item ID for Third Floor Master Round. Replace with Item ID of your choosing. 
            return dungeon;
        }
    }
}