using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using tk2dRuntime.TileMap;
using UnityEngine;
using ItemAPI;
using FullInspector;

using Gungeon;

//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;

using Brave.BulletScript;
using GungeonAPI;

using System.Text;
using System.IO;
using System.Reflection;
using SaveAPI;

using MonoMod.RuntimeDetour;
using DaikonForge;


namespace Planetside
{
	public class LoadHelper
    {

		static LoadHelper()
		{
			BundlePrereqs = new string[]
			{
				"brave_resources_001",
				"dungeon_scene_001",
				"encounters_base_001",
				"enemies_base_001",
				"flows_base_001",
				"foyer_001",
				"foyer_002",
				"foyer_003",
				"shared_auto_001",
				"shared_auto_002",
				"shared_base_001",
				"dungeons/base_bullethell",
				"dungeons/base_castle",
				"dungeons/base_catacombs",
				"dungeons/base_cathedral",
				"dungeons/base_forge",
				"dungeons/base_foyer",
				"dungeons/base_gungeon",
				"dungeons/base_mines",
				"dungeons/base_nakatomi",
				"dungeons/base_resourcefulrat",
				"dungeons/base_sewer",
				"dungeons/base_tutorial",
				"dungeons/finalscenario_bullet",
				"dungeons/finalscenario_convict",
				"dungeons/finalscenario_coop",
				"dungeons/finalscenario_guide",
				"dungeons/finalscenario_pilot",
				"dungeons/finalscenario_robot",
				"dungeons/finalscenario_soldier"
			};
		}
		public static T LoadAssetFromAnywhere<T>(string path) where T : UnityEngine.Object
		{
			T obj = null;
			foreach (string name in BundlePrereqs)
			{
				try
				{
					obj = ResourceManager.LoadAssetBundle(name).LoadAsset<T>(path);
				}
				catch
				{
				}
				if (obj != null)
				{
					break;
				}
			}
			return obj;
		}
		private static string[] BundlePrereqs;
	}
}

namespace Planetside
{

    public static class DungeonGenToolbox
    {		
		public class AdvancedDungeonPrerequisite : CustomDungeonPrerequisite
		{
			public override bool CheckConditionsFulfilled()
			{
				if (advancedAdvancedPrerequisiteType != AdvancedAdvancedPrerequisiteType.NONE)
				{
					if (advancedAdvancedPrerequisiteType == AdvancedAdvancedPrerequisiteType.PASSIVE_ITEM_FLAG)
					{
						if (PassiveItem.IsFlagSetAtAll(requiredPassiveFlag) == true)
                        {
							return true;
						}
						else
                        {
							return false;
						}
					}
					if (advancedAdvancedPrerequisiteType == AdvancedAdvancedPrerequisiteType.SPEEDRUN_TIMER_BEFORE)
					{
						if (GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED) <= BeforeTimeInSeconds)
						{
							return true;
						}
						else
						{
							return false;
						}
					}
					if (advancedAdvancedPrerequisiteType == AdvancedAdvancedPrerequisiteType.UNLOCK)
					{
						if (SaveAPIManager.GetFlag(UnlockFlag) == UnlockRequirement)
						{
							return true;
						}
						else
						{
							return false;
						}
					}
					if (advancedAdvancedPrerequisiteType == AdvancedAdvancedPrerequisiteType.SPEEDRUN_TIMER_AFTER)
                    {
						if (GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED) >= AfterTimeInSeconds)
						{
							return true;
						}
						else
						{
							return false;
						}
					}
				}
				else
				{
					return base.CheckConditionsFulfilled();
				}
				return false;
			}

			public float BeforeTimeInSeconds;
			public float AfterTimeInSeconds;
			public bool UnlockRequirement;
			public CustomDungeonFlags UnlockFlag;

			public AdvancedAdvancedPrerequisiteType advancedAdvancedPrerequisiteType;

			public enum AdvancedAdvancedPrerequisiteType
			{
				NONE,
				PASSIVE_ITEM_FLAG,
				SPEEDRUN_TIMER_BEFORE,
				SPEEDRUN_TIMER_AFTER,
				UNLOCK
			}
		}

		//======================================================================================================

		public static void AddObjectToRoom(PrototypeDungeonRoom room, Vector2 position, GameObject PlacableObject, int xOffset = 0, int yOffset = 0, int layer = 0, float SpawnChance = 1f)
		{
			if (room == null)
			{
				return;
			}
			if (room.placedObjects == null)
			{
				room.placedObjects = new List<PrototypePlacedObjectData>();
			}
			if (room.placedObjectPositions == null)
			{
				room.placedObjectPositions = new List<Vector2>();
			}

			DungeonPlaceableVariant dungeonPlaceableVariant = new DungeonPlaceableVariant();
			dungeonPlaceableVariant.percentChance = 1f;
			dungeonPlaceableVariant.unitOffset = new Vector2(0,0);
			dungeonPlaceableVariant.enemyPlaceableGuid = string.Empty;
			dungeonPlaceableVariant.pickupObjectPlaceableId = -1;
			dungeonPlaceableVariant.forceBlackPhantom = false;
			dungeonPlaceableVariant.addDebrisObject = false;
			dungeonPlaceableVariant.prerequisites = null;
			dungeonPlaceableVariant.materialRequirements = null;
			dungeonPlaceableVariant.nonDatabasePlaceable = PlacableObject;
			DungeonPlaceable dungeonPlaceable = ScriptableObject.CreateInstance<DungeonPlaceable>();
			dungeonPlaceable.variantTiers = new List<DungeonPlaceableVariant>
			{
					dungeonPlaceableVariant
			};

			PrototypePlacedObjectData item = new PrototypePlacedObjectData
			{

				placeableContents = dungeonPlaceable,
				nonenemyBehaviour = null,
				spawnChance = SpawnChance,
				unspecifiedContents = null,
				enemyBehaviourGuid = string.Empty,
				contentsBasePosition = position,
				layer = layer,
				xMPxOffset = xOffset,
				yMPxOffset = yOffset,
				fieldData = new List<PrototypePlacedObjectFieldData>(0),
				instancePrerequisites = new DungeonPrerequisite[0],
				linkedTriggerAreaIDs = new List<int>(0),
				assignedPathIDx = -1,
				assignedPathStartNode = 0
			};
			room.placedObjects.Add(item);
			room.placedObjectPositions.Add(position);
		}

		public static DungeonPlaceable GenerateDungeonPlacable(GameObject ObjectPrefab = null, bool spawnsEnemy = false, bool useExternalPrefab = false, bool spawnsItem = false, string EnemyGUID = "479556d05c7c44f3b6abb3b2067fc778", int itemID = 307, Vector2? CustomOffset = null, bool itemHasDebrisObject = true, float spawnChance = 1f)
		{
			AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
			ResourceManager.LoadAssetBundle("shared_auto_002");
			ResourceManager.LoadAssetBundle("brave_resources_001");
			GameObject nonDatabasePlaceable = assetBundle.LoadAsset<GameObject>("Chest_Wood_Two_Items");
			GameObject nonDatabasePlaceable2 = assetBundle.LoadAsset<GameObject>("chest_silver");
			GameObject nonDatabasePlaceable3 = assetBundle.LoadAsset<GameObject>("chest_green");
			GameObject nonDatabasePlaceable4 = assetBundle.LoadAsset<GameObject>("chest_synergy");
			GameObject nonDatabasePlaceable5 = assetBundle.LoadAsset<GameObject>("chest_red");
			GameObject nonDatabasePlaceable6 = assetBundle.LoadAsset<GameObject>("Chest_Black");
			GameObject nonDatabasePlaceable7 = assetBundle.LoadAsset<GameObject>("Chest_Rainbow");
			DungeonPlaceableVariant dungeonPlaceableVariant = new DungeonPlaceableVariant();
			dungeonPlaceableVariant.percentChance = 0.35f;
			dungeonPlaceableVariant.unitOffset = new Vector2(1f, 0.8f);
			dungeonPlaceableVariant.enemyPlaceableGuid = string.Empty;
			dungeonPlaceableVariant.pickupObjectPlaceableId = -1;
			dungeonPlaceableVariant.forceBlackPhantom = false;
			dungeonPlaceableVariant.addDebrisObject = false;
			dungeonPlaceableVariant.prerequisites = null;
			dungeonPlaceableVariant.materialRequirements = null;
			dungeonPlaceableVariant.nonDatabasePlaceable = nonDatabasePlaceable2;
			DungeonPlaceableVariant dungeonPlaceableVariant2 = new DungeonPlaceableVariant();
			dungeonPlaceableVariant2.percentChance = 0.28f;
			dungeonPlaceableVariant2.unitOffset = new Vector2(1f, 0.8f);
			dungeonPlaceableVariant2.enemyPlaceableGuid = string.Empty;
			dungeonPlaceableVariant2.pickupObjectPlaceableId = -1;
			dungeonPlaceableVariant2.forceBlackPhantom = false;
			dungeonPlaceableVariant2.addDebrisObject = false;
			dungeonPlaceableVariant2.prerequisites = null;
			dungeonPlaceableVariant2.materialRequirements = null;
			dungeonPlaceableVariant2.nonDatabasePlaceable = nonDatabasePlaceable;
			DungeonPlaceableVariant dungeonPlaceableVariant3 = new DungeonPlaceableVariant();
			dungeonPlaceableVariant3.percentChance = 0.25f;
			dungeonPlaceableVariant3.unitOffset = new Vector2(1f, 0.8f);
			dungeonPlaceableVariant3.enemyPlaceableGuid = string.Empty;
			dungeonPlaceableVariant3.pickupObjectPlaceableId = -1;
			dungeonPlaceableVariant3.forceBlackPhantom = false;
			dungeonPlaceableVariant3.addDebrisObject = false;
			dungeonPlaceableVariant3.prerequisites = null;
			dungeonPlaceableVariant3.materialRequirements = null;
			dungeonPlaceableVariant3.nonDatabasePlaceable = nonDatabasePlaceable3;
			DungeonPlaceableVariant dungeonPlaceableVariant4 = new DungeonPlaceableVariant();
			dungeonPlaceableVariant4.percentChance = 0.2f;
			dungeonPlaceableVariant4.unitOffset = new Vector2(1f, 0.8f);
			dungeonPlaceableVariant4.enemyPlaceableGuid = string.Empty;
			dungeonPlaceableVariant4.pickupObjectPlaceableId = -1;
			dungeonPlaceableVariant4.forceBlackPhantom = false;
			dungeonPlaceableVariant4.addDebrisObject = false;
			dungeonPlaceableVariant4.prerequisites = null;
			dungeonPlaceableVariant4.materialRequirements = null;
			dungeonPlaceableVariant4.nonDatabasePlaceable = nonDatabasePlaceable4;
			DungeonPlaceableVariant dungeonPlaceableVariant5 = new DungeonPlaceableVariant();
			dungeonPlaceableVariant5.percentChance = 0.15f;
			dungeonPlaceableVariant5.unitOffset = new Vector2(0.5f, 0.5f);
			dungeonPlaceableVariant5.enemyPlaceableGuid = string.Empty;
			dungeonPlaceableVariant5.pickupObjectPlaceableId = -1;
			dungeonPlaceableVariant5.forceBlackPhantom = false;
			dungeonPlaceableVariant5.addDebrisObject = false;
			dungeonPlaceableVariant5.prerequisites = null;
			dungeonPlaceableVariant5.materialRequirements = null;
			dungeonPlaceableVariant5.nonDatabasePlaceable = nonDatabasePlaceable5;
			DungeonPlaceableVariant dungeonPlaceableVariant6 = new DungeonPlaceableVariant();
			dungeonPlaceableVariant6.percentChance = 0.1f;
			dungeonPlaceableVariant6.unitOffset = new Vector2(0.5f, 0.5f);
			dungeonPlaceableVariant6.enemyPlaceableGuid = string.Empty;
			dungeonPlaceableVariant6.pickupObjectPlaceableId = -1;
			dungeonPlaceableVariant6.forceBlackPhantom = false;
			dungeonPlaceableVariant6.addDebrisObject = false;
			dungeonPlaceableVariant6.prerequisites = null;
			dungeonPlaceableVariant6.materialRequirements = null;
			dungeonPlaceableVariant6.nonDatabasePlaceable = nonDatabasePlaceable6;
			DungeonPlaceableVariant dungeonPlaceableVariant7 = new DungeonPlaceableVariant();
			dungeonPlaceableVariant7.percentChance = 0.005f;
			dungeonPlaceableVariant7.unitOffset = new Vector2(0.5f, 0.5f);
			dungeonPlaceableVariant7.enemyPlaceableGuid = string.Empty;
			dungeonPlaceableVariant7.pickupObjectPlaceableId = -1;
			dungeonPlaceableVariant7.forceBlackPhantom = false;
			dungeonPlaceableVariant7.addDebrisObject = false;
			dungeonPlaceableVariant7.prerequisites = null;
			dungeonPlaceableVariant7.materialRequirements = null;
			dungeonPlaceableVariant7.nonDatabasePlaceable = nonDatabasePlaceable7;
			DungeonPlaceableVariant dungeonPlaceableVariant8 = new DungeonPlaceableVariant();
			dungeonPlaceableVariant8.percentChance = spawnChance;
			if (CustomOffset != null)
			{
				dungeonPlaceableVariant8.unitOffset = CustomOffset.Value;
			}
			else
			{
				dungeonPlaceableVariant8.unitOffset = Vector2.zero;
			}
			dungeonPlaceableVariant8.enemyPlaceableGuid = string.Empty;
			dungeonPlaceableVariant8.pickupObjectPlaceableId = itemID;
			dungeonPlaceableVariant8.forceBlackPhantom = false;
			if (itemHasDebrisObject)
			{
				dungeonPlaceableVariant8.addDebrisObject = true;
			}
			else
			{
				dungeonPlaceableVariant8.addDebrisObject = false;
			}
			dungeonPlaceableVariant7.prerequisites = null;
			dungeonPlaceableVariant7.materialRequirements = null;
			List<DungeonPlaceableVariant> list = new List<DungeonPlaceableVariant>();
			list.Add(dungeonPlaceableVariant2);
			list.Add(dungeonPlaceableVariant);
			list.Add(dungeonPlaceableVariant3);
			list.Add(dungeonPlaceableVariant4);
			list.Add(dungeonPlaceableVariant5);
			list.Add(dungeonPlaceableVariant6);
			list.Add(dungeonPlaceableVariant7);
			DungeonPlaceableVariant dungeonPlaceableVariant9 = new DungeonPlaceableVariant();
			dungeonPlaceableVariant9.percentChance = spawnChance;
			dungeonPlaceableVariant9.unitOffset = Vector2.zero;
			dungeonPlaceableVariant9.enemyPlaceableGuid = EnemyGUID;
			dungeonPlaceableVariant9.pickupObjectPlaceableId = -1;
			dungeonPlaceableVariant9.forceBlackPhantom = false;
			dungeonPlaceableVariant9.addDebrisObject = false;
			dungeonPlaceableVariant9.prerequisites = null;
			dungeonPlaceableVariant9.materialRequirements = null;
			List<DungeonPlaceableVariant> list2 = new List<DungeonPlaceableVariant>();
			list2.Add(dungeonPlaceableVariant9);
			List<DungeonPlaceableVariant> list3 = new List<DungeonPlaceableVariant>();
			list3.Add(dungeonPlaceableVariant8);
			DungeonPlaceable dungeonPlaceable = ScriptableObject.CreateInstance<DungeonPlaceable>();
			dungeonPlaceable.name = "CustomChestPlacable";
			if (spawnsEnemy || useExternalPrefab)
			{
				dungeonPlaceable.width = 2;
				dungeonPlaceable.height = 2;
			}
			else if (spawnsItem)
			{
				dungeonPlaceable.width = 1;
				dungeonPlaceable.height = 1;
			}
			else
			{
				dungeonPlaceable.width = 4;
				dungeonPlaceable.height = 1;
			}
			dungeonPlaceable.roomSequential = false;
			dungeonPlaceable.respectsEncounterableDifferentiator = true;
			dungeonPlaceable.UsePrefabTransformOffset = false;
			dungeonPlaceable.isPassable = true;
			if (spawnsItem)
			{
				dungeonPlaceable.MarkSpawnedItemsAsRatIgnored = true;
			}
			else
			{
				dungeonPlaceable.MarkSpawnedItemsAsRatIgnored = false;
			}
			dungeonPlaceable.DebugThisPlaceable = false;
			if (useExternalPrefab && ObjectPrefab != null)
			{
				DungeonPlaceableVariant dungeonPlaceableVariant10 = new DungeonPlaceableVariant();
				dungeonPlaceableVariant10.percentChance = spawnChance;
				if (CustomOffset != null)
				{
					dungeonPlaceableVariant10.unitOffset = CustomOffset.Value;
				}
				else
				{
					dungeonPlaceableVariant10.unitOffset = Vector2.zero;
				}
				dungeonPlaceableVariant10.enemyPlaceableGuid = string.Empty;
				dungeonPlaceableVariant10.pickupObjectPlaceableId = -1;
				dungeonPlaceableVariant10.forceBlackPhantom = false;
				dungeonPlaceableVariant10.addDebrisObject = false;
				dungeonPlaceableVariant10.nonDatabasePlaceable = ObjectPrefab;
				dungeonPlaceable.variantTiers = new List<DungeonPlaceableVariant>
				{
					dungeonPlaceableVariant10
				};
			}
			else if (spawnsEnemy)
			{
				dungeonPlaceable.variantTiers = list2;
			}
			else if (spawnsItem)
			{
				dungeonPlaceable.variantTiers = list3;
			}
			else
			{
				dungeonPlaceable.variantTiers = list;
			}
			return dungeonPlaceable;
		}
	}
}



