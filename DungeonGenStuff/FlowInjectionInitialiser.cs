using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using GungeonAPI;
using SaveAPI;
using System.Text;

using Gungeon;

namespace Planetside
{
    public class FlowInjectionInitialiser : FlowDatabase
    {
		public static PrototypeDungeonRoom BrokenChamberRoomPrefab;
		public static SharedInjectionData ForgeData;
		public static SharedInjectionData BaseSharedInjectionData;
		public static ProceduralFlowModifierData BrokenChamberRoom;

		public static SharedInjectionData GungeonInjectionData;

		public static void InitialiseFlows()
        {
			AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");
			BaseSharedInjectionData = sharedAssets2.LoadAsset<SharedInjectionData>("Base Shared Injection Data");

			




						sharedAssets2 = null;
			AddVrokenChamberRoom(false);
			//AddShellraxRoom(false);
			AddMinesSWRoom(false);
			InitTimeTraderRooms(false);
		}

		public static void AddVrokenChamberRoom(bool refreshFlows = false)
		{
			float Weight = 0.75f;
			if (SaveAPIManager.GetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED) == true)
            {
				Weight = 0.05f;
            }	
			PrototypeDungeonRoom BrokenChamberRoomVar = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/BrokenChamberRoom.room").room;


			Vector2 offset = new Vector2(0, 0);
			Vector2 vector = new Vector2((float)(BrokenChamberRoomVar.Width / 2) + offset.x, (float)(BrokenChamberRoomVar.Height / 2) + offset.y);

			BrokenChamberRoomVar.placedObjectPositions.Add(vector);
			DungeonPrerequisite[] array = new DungeonPrerequisite[0];


			GameObject original;
			OldShrineFactory.builtShrines.TryGetValue("psog:brokenchambershrine", out original);
			PlanetsideModule.BrokenChamberShrineController broken = original.gameObject.AddComponent<PlanetsideModule.BrokenChamberShrineController>();
			broken.obj = original;

			BrokenChamberRoomVar.placedObjects.Add(new PrototypePlacedObjectData
			{

				contentsBasePosition = vector,
				fieldData = new List<PrototypePlacedObjectFieldData>(),
				instancePrerequisites = array,
				linkedTriggerAreaIDs = new List<int>(),
				placeableContents = new DungeonPlaceable
				{
					width = 2,
					height = 2,
					respectsEncounterableDifferentiator = true,
					variantTiers = new List<DungeonPlaceableVariant>
					{
						new DungeonPlaceableVariant
						{
							percentChance = 1f,
							nonDatabasePlaceable = original,
							prerequisites = array,
							materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0]
						}
					}
				}
			});

			BrokenChamberRoom = new ProceduralFlowModifierData()
			{
				annotation = "Broken Chamber Spawn",
				DEBUG_FORCE_SPAWN = false,
				OncePerRun = true,
				placementRules = new List<ProceduralFlowModifierData.FlowModifierPlacementType>() {
					ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN
				},
				roomTable = null,
				exactRoom = BrokenChamberRoomVar,
				IsWarpWing = false,
				RequiresMasteryToken = false,
				chanceToLock = 0,
				selectionWeight = 2,
				chanceToSpawn = Weight,
				RequiredValidPlaceable = null,
				prerequisites = new DungeonPrerequisite[] {
					new DungeonPrerequisite
					{
						prerequisiteOperation = DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO,
						prerequisiteType = DungeonPrerequisite.PrerequisiteType.TILESET,
						requiredTileset = GlobalDungeonData.ValidTilesets.CASTLEGEON,
						requireTileset = true,
						comparisonValue = 1f,
						encounteredObjectGuid = string.Empty,
						maxToCheck = TrackedMaximums.MOST_KEYS_HELD,
						requireDemoMode = false,
						requireCharacter = false,
						requiredCharacter = PlayableCharacters.Pilot,
						requireFlag = false,
						useSessionStatValue = false,
						encounteredRoom = null,
						requiredNumberOfEncounters = -1,
						saveFlagToCheck = GungeonFlags.TUTORIAL_COMPLETED,
						statToCheck = TrackedStats.GUNBERS_MUNCHED,
						
						
					}
					
				},
				CanBeForcedSecret = true,
				RandomNodeChildMinDistanceFromEntrance = 0,
				exactSecondaryRoom = null,
				framedCombatNodes = 0,

			};
			BrokenChamberRoomPrefab = BrokenChamberRoomVar;
			BaseSharedInjectionData.InjectionData.Add(BrokenChamberRoom);



		}
		public static void AddShellraxRoom(bool refreshFlows = false)
		{
			AssetBundle shared_auto_001 = ResourceManager.LoadAssetBundle("shared_auto_001");
			GameObject iconPrefab =(shared_auto_001.LoadAsset("assets/data/prefabs/room icons/minimap_boss_icon.prefab") as GameObject);
			shared_auto_001 = null;
			Dungeon hellDungeon = DungeonDatabase.GetOrLoadByName("base_bullethell");
			ShellraxRoomPrefab = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/hellpain.room").room;
			ShellraxRoomPrefab.associatedMinimapIcon = iconPrefab;
			ShellraxRoom = new ProceduralFlowModifierData()
			{
				annotation = "Shellrax Room Yes yes yes",
				DEBUG_FORCE_SPAWN = true,
				OncePerRun = true,
				placementRules = new List<ProceduralFlowModifierData.FlowModifierPlacementType>() {
					ProceduralFlowModifierData.FlowModifierPlacementType.COMBAT_FRAME, ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN,ProceduralFlowModifierData.FlowModifierPlacementType.HUB_ADJACENT_CHAIN_START,ProceduralFlowModifierData.FlowModifierPlacementType.HUB_ADJACENT_NO_LINK,ProceduralFlowModifierData.FlowModifierPlacementType.RANDOM_NODE_CHILD,
				},
				exactRoom = ShellraxRoomPrefab,
				IsWarpWing = false,
				RequiresMasteryToken = false,
				chanceToLock = 0,
				selectionWeight = 2,
				chanceToSpawn = 2,
				RequiredValidPlaceable = null,
				prerequisites = new DungeonPrerequisite[] {
					new DungeonPrerequisite
					{
						prerequisiteOperation = DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO,
						prerequisiteType = DungeonPrerequisite.PrerequisiteType.TILESET,
						requiredTileset = GlobalDungeonData.ValidTilesets.HELLGEON,
						requireTileset = true,
						comparisonValue = 1f,
						encounteredObjectGuid = string.Empty,
						maxToCheck = TrackedMaximums.MOST_KEYS_HELD,
						requireDemoMode = false,
						requireCharacter = false,
						requiredCharacter = PlayableCharacters.Pilot,
						requireFlag = false,
						useSessionStatValue = false,
						encounteredRoom = null,
						requiredNumberOfEncounters = -1,
						saveFlagToCheck = GungeonFlags.TUTORIAL_COMPLETED,
						statToCheck = TrackedStats.GUNBERS_MUNCHED
					}
				},
				CanBeForcedSecret = false,
				RandomNodeChildMinDistanceFromEntrance = 0,
				exactSecondaryRoom = null,
				framedCombatNodes = 0,

			};
			//SharedInjectionData hellinjectiondata = null;
			BaseSharedInjectionData.InjectionData.Add(ShellraxRoom);




			//SharedInjectionData hellinjectiondata = hellDungeon.PatternSettings.flows[0].sharedInjectionData[0];
			//hellinjectiondata.InjectionData.Add(ShellraxRoom);

		}
		public static void AddMinesSWRoom(bool refreshFlows = false)
		{
			PrototypeDungeonRoom SWRoom = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/SWRooms/SomethingMines.room").room;
			
			Vector2 offset = new Vector2(-0.875f, -0.875f);
			Vector2 vector = new Vector2((float)(SWRoom.Width / 2) + offset.x, (float)(SWRoom.Height / 2) + offset.y);

			SWRoom.placedObjectPositions.Add(vector);
			DungeonPrerequisite[] array = new DungeonPrerequisite[0];


			GameObject original;
			OldShrineFactory.builtShrines.TryGetValue("psog:bluecasingshrine", out original);
			SWRoom.placedObjects.Add(new PrototypePlacedObjectData
			{

				contentsBasePosition = vector,
				fieldData = new List<PrototypePlacedObjectFieldData>(),
				instancePrerequisites = array,
				linkedTriggerAreaIDs = new List<int>(),
				placeableContents = new DungeonPlaceable
				{
					width = 2,
					height = 2,
					respectsEncounterableDifferentiator = true,
					variantTiers = new List<DungeonPlaceableVariant>
					{
						new DungeonPlaceableVariant
						{
							percentChance = 1f,
							nonDatabasePlaceable = original,
							prerequisites = array,
							materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0]
						}
					}
				}
			});
			
			SWMinesRoom = new ProceduralFlowModifierData()
			{
				annotation = "SW Mines",
				DEBUG_FORCE_SPAWN = false,
				OncePerRun = true,
				placementRules = new List<ProceduralFlowModifierData.FlowModifierPlacementType>() {
					ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN
				},
				roomTable = null,
				exactRoom = SWRoom,
				IsWarpWing = false,
				RequiresMasteryToken = false,
				chanceToLock = 0,
				selectionWeight = 2,
				chanceToSpawn = 1,
				RequiredValidPlaceable = null,
				prerequisites = new DungeonPrerequisite[] {
					new DungeonGenToolbox.AdvancedDungeonPrerequisite
					{
					   advancedAdvancedPrerequisiteType = DungeonGenToolbox.AdvancedDungeonPrerequisite.AdvancedAdvancedPrerequisiteType.SPEEDRUN_TIMER_BEFORE,
					   BeforeTimeInSeconds = 600,
					   requiredTileset = GlobalDungeonData.ValidTilesets.MINEGEON,
					   requireTileset = true
					},
				},
				CanBeForcedSecret = false,
				RandomNodeChildMinDistanceFromEntrance = 0,
				exactSecondaryRoom = null,
				framedCombatNodes = 0,

			};
			BaseSharedInjectionData.InjectionData.Add(SWMinesRoom);
			SWMinesRoomPrefab = SWRoom;
		}

		public static void InitTimeTraderRooms(bool refreshFlows = false)
        {			
			SharedInjectionData injector = ScriptableObject.CreateInstance<SharedInjectionData>();
			injector.UseInvalidWeightAsNoInjection = true;
			injector.PreventInjectionOfFailedPrerequisites = false;
			injector.IsNPCCell = false;
			injector.IgnoreUnmetPrerequisiteEntries = false;
			injector.OnlyOne = false;
			injector.ChanceToSpawnOne = 1f;
			injector.AttachedInjectionData = new List<SharedInjectionData>();
			injector.InjectionData = new List<ProceduralFlowModifierData>
			{
				GenerateNewProcDataForTimeTrader(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/ShopRooms/TimeTraderShopProper.room").room, GlobalDungeonData.ValidTilesets.GUNGEON, 240),
				GenerateNewProcDataForTimeTrader(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/ShopRooms/TimeTraderShopMines.room").room, GlobalDungeonData.ValidTilesets.MINEGEON, 510),
				GenerateNewProcDataForTimeTraderTooLate(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/ShopRooms/TimeTraderShopMines.room").room, GlobalDungeonData.ValidTilesets.MINEGEON, 510),
				GenerateNewProcDataForTimeTrader(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/ShopRooms/TimeTraderShopHollow.room").room, GlobalDungeonData.ValidTilesets.CATACOMBGEON, 840),
				GenerateNewProcDataForTimeTrader(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/ShopRooms/TimeTraderShopForge.room").room, GlobalDungeonData.ValidTilesets.FORGEGEON, 1230),
				GenerateNewProcDataForTimeTrader(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/ShopRooms/TimeTraderShopSewer.room").room, GlobalDungeonData.ValidTilesets.SEWERGEON, 270),
				GenerateNewProcDataForTimeTrader(RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/ShopRooms/TimeTraderShopAbbey.room").room, GlobalDungeonData.ValidTilesets.CATHEDRALGEON, 780),
			};
			injector.name = "Time Trader Rooms";
			SharedInjectionData BaseInjection = LoadHelper.LoadAssetFromAnywhere<SharedInjectionData>("Base Shared Injection Data");
			if (BaseInjection.AttachedInjectionData == null)
			{
				BaseInjection.AttachedInjectionData = new List<SharedInjectionData>();
			}
			BaseInjection.AttachedInjectionData.Add(injector);
		}
		
		public static ProceduralFlowModifierData GenerateNewProcDataForTimeTrader(PrototypeDungeonRoom RequiredRoom, GlobalDungeonData.ValidTilesets Tileset, float BeforeTimeInSeconds)
		{
			string name = RequiredRoom.name.ToString()+Tileset.ToString();
			if (RequiredRoom.name.ToString() == null)
			{
				name = "EmergencyAnnotationName";
			}

			Vector2 offset = new Vector2(-2.25f, -1.25f);
			Vector2 vector = new Vector2((float)(RequiredRoom.Width / 2) + offset.x, (float)(RequiredRoom.Height / 2) + offset.y);

			RequiredRoom.placedObjectPositions.Add(vector);
			DungeonPrerequisite[] array = new DungeonPrerequisite[0];

			GameObject original;
			NpcApi.ItsDaFuckinShopApi.builtShops.TryGetValue("psog:timedshop", out original);
			RequiredRoom.placedObjects.Add(new PrototypePlacedObjectData
			{

				contentsBasePosition = vector,
				fieldData = new List<PrototypePlacedObjectFieldData>(),
				instancePrerequisites = array,
				linkedTriggerAreaIDs = new List<int>(),
				placeableContents = new DungeonPlaceable
				{
					width = 2,
					height = 2,
					respectsEncounterableDifferentiator = true,
					variantTiers = new List<DungeonPlaceableVariant>
					{
						new DungeonPlaceableVariant
						{
							percentChance = 1f,
							nonDatabasePlaceable = original,
							prerequisites = array,
							materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0]
						}
					}
				}
			});

			ProceduralFlowModifierData SpecProcData = new ProceduralFlowModifierData()
			{
				annotation = name,
				DEBUG_FORCE_SPAWN = false,
				OncePerRun = false,
				placementRules = new List<ProceduralFlowModifierData.FlowModifierPlacementType>()
				{
					ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN
				},
				roomTable = null,
				exactRoom = RequiredRoom,
				IsWarpWing = false,
				RequiresMasteryToken = false,
				chanceToLock = 0,
				selectionWeight = 2,
				chanceToSpawn = 1,
				RequiredValidPlaceable = null,
				prerequisites = new DungeonPrerequisite[]
				{
					new DungeonGenToolbox.AdvancedDungeonPrerequisite
					{
					   advancedAdvancedPrerequisiteType = DungeonGenToolbox.AdvancedDungeonPrerequisite.AdvancedAdvancedPrerequisiteType.SPEEDRUN_TIMER_BEFORE,
					   BeforeTimeInSeconds = BeforeTimeInSeconds,
					   requiredTileset = Tileset,
					   requireTileset = true
					}
				},
				CanBeForcedSecret = false,
				RandomNodeChildMinDistanceFromEntrance = 0,
				exactSecondaryRoom = null,
				framedCombatNodes = 0,
			};
			return SpecProcData;
		}

		public static ProceduralFlowModifierData GenerateNewProcDataForTimeTraderTooLate(PrototypeDungeonRoom RequiredRoom, GlobalDungeonData.ValidTilesets Tileset, float BeforeTimeInSeconds)
		{
			string name = RequiredRoom.name.ToString() + Tileset.ToString();
			if (RequiredRoom.name.ToString() == null)
			{
				name = "EmergencyAnnotationName";
			}
			Vector2 offset = new Vector2(-0.75f, -0.75f);
			Vector2 vector = new Vector2((float)(RequiredRoom.Width / 2) + offset.x, (float)(RequiredRoom.Height / 2) + offset.y);

			RequiredRoom.placedObjectPositions.Add(vector);
			DungeonPrerequisite[] array = new DungeonPrerequisite[0];

			GameObject original;
			OldShrineFactory.builtShrines.TryGetValue("psog:toolate", out original);
			RequiredRoom.placedObjects.Add(new PrototypePlacedObjectData
			{

				contentsBasePosition = vector,
				fieldData = new List<PrototypePlacedObjectFieldData>(),
				instancePrerequisites = array,
				linkedTriggerAreaIDs = new List<int>(),
				placeableContents = new DungeonPlaceable
				{
					width = 2,
					height = 2,
					respectsEncounterableDifferentiator = true,
					variantTiers = new List<DungeonPlaceableVariant>
					{
						new DungeonPlaceableVariant
						{
							percentChance = 1f,
							nonDatabasePlaceable = original,
							prerequisites = array,
							materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0]
						}
					}
				}
			});

			ProceduralFlowModifierData SpecProcData = new ProceduralFlowModifierData()
			{
				annotation = name,
				DEBUG_FORCE_SPAWN = false,
				OncePerRun = false,
				placementRules = new List<ProceduralFlowModifierData.FlowModifierPlacementType>()
				{
					ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN
				},
				roomTable = null,
				exactRoom = RequiredRoom,
				IsWarpWing = false,
				RequiresMasteryToken = false,
				chanceToLock = 0,
				selectionWeight = 2,
				chanceToSpawn = 1,
				RequiredValidPlaceable = null,
				prerequisites = new DungeonPrerequisite[]
				{
					new DungeonGenToolbox.AdvancedDungeonPrerequisite
					{
					   advancedAdvancedPrerequisiteType = DungeonGenToolbox.AdvancedDungeonPrerequisite.AdvancedAdvancedPrerequisiteType.SPEEDRUN_TIMER_AFTER,
					   AfterTimeInSeconds = BeforeTimeInSeconds,
					   requiredTileset = Tileset,
					   requireTileset = true
					}
				},
				CanBeForcedSecret = false,
				RandomNodeChildMinDistanceFromEntrance = 0,
				exactSecondaryRoom = null,
				framedCombatNodes = 0,
			};
			return SpecProcData;
		}



		public static PrototypeDungeonRoom PrayerAmuletRoomPrefab;
		public static ProceduralFlowModifierData PrayerAmuletRoom;

		public static PrototypeDungeonRoom ShellraxRoomPrefab;
		public static ProceduralFlowModifierData ShellraxRoom;

		public static PrototypeDungeonRoom SWMinesRoomPrefab;
		public static ProceduralFlowModifierData SWMinesRoom;


		public static PrototypeDungeonRoom TimeTraderBaseRoom;

	}
}
