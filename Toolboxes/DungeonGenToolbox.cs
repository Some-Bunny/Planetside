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
using Pathfinding;

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


		public static RoomHandler AddCustomRuntimeRoomWithTileSet(Dungeon dungeon2, PrototypeDungeonRoom prototype, bool addRoomToMinimap = true, bool addTeleporter = true, bool isSecretRatExitRoom = false, Action<RoomHandler> postProcessCellData = null, DungeonData.LightGenerationStyle lightStyle = DungeonData.LightGenerationStyle.STANDARD, bool allowProceduralDecoration = true, bool allowProceduralLightFixtures = true, int visualSubtype = -1)
		{
			Dungeon dungeon = GameManager.Instance.Dungeon;
			tk2dTileMap m_tilemap = dungeon.MainTilemap;

			if (m_tilemap == null)
			{
				ETGModConsole.Log("ERROR: TileMap object is null! Something seriously went wrong!");
				Debug.Log("ERROR: TileMap object is null! Something seriously went wrong!");
				return null;
			}

			ExpandTK2DDungeonAssembler assembler = new ExpandTK2DDungeonAssembler();
			assembler.Initialize(dungeon2.tileIndices);

			IntVector2 basePosition = IntVector2.Zero;
			IntVector2 basePosition2 = new IntVector2(50, 50);
			int num = basePosition2.x;
			int num2 = basePosition2.y;
			IntVector2 intVector = new IntVector2(int.MaxValue, int.MaxValue);
			IntVector2 intVector2 = new IntVector2(int.MinValue, int.MinValue);
			intVector = IntVector2.Min(intVector, basePosition);
			intVector2 = IntVector2.Max(intVector2, basePosition + new IntVector2(prototype.Width, prototype.Height));
			IntVector2 a = intVector2 - intVector;
			IntVector2 b = IntVector2.Min(IntVector2.Zero, -1 * intVector);
			a += b;
			IntVector2 intVector3 = new IntVector2(dungeon.data.Width + num, num);
			int newWidth = dungeon.data.Width + num * 2 + a.x;
			int newHeight = Mathf.Max(dungeon.data.Height, a.y + num * 2);
			CellData[][] array = BraveUtility.MultidimensionalArrayResize(dungeon.data.cellData, dungeon.data.Width, dungeon.data.Height, newWidth, newHeight);
			dungeon.data.cellData = array;
			dungeon.data.ClearCachedCellData();
			IntVector2 d = new IntVector2(prototype.Width, prototype.Height);
			IntVector2 b2 = basePosition + b;
			IntVector2 intVector4 = intVector3 + b2;
			CellArea cellArea = new CellArea(intVector4, d, 0);
			cellArea.prototypeRoom = prototype;
			RoomHandler targetRoom = new RoomHandler(cellArea);
			if (visualSubtype != -1)
			{
				targetRoom.AssignRoomVisualType(visualSubtype);
			}
			for (int k = -num; k < d.x + num; k++)
			{
				for (int l = -num; l < d.y + num; l++)
				{
					IntVector2 p = new IntVector2(k, l) + intVector4;
					if ((k >= 0 && l >= 0 && k < d.x && l < d.y) || array[p.x][p.y] == null)
					{
						CellData cellData = new CellData(p, CellType.WALL);
						cellData.positionInTilemap = cellData.positionInTilemap - intVector3 + new IntVector2(num2, num2);
						cellData.parentArea = cellArea;
						cellData.parentRoom = targetRoom;
						cellData.nearestRoom = targetRoom;
						cellData.distanceFromNearestRoom = 0f;
						array[p.x][p.y] = cellData;
					}
				}
			}
			dungeon.data.rooms.Add(targetRoom);
			try
			{
				targetRoom.WriteRoomData(dungeon.data);
			}
			catch (Exception)
			{
				ETGModConsole.Log("WARNING: Exception caused during WriteRoomData step on room: " + targetRoom.GetRoomName());
				return null;
			}
			try
			{
				GenerateLightsForRoomFromOtherTileset(dungeon2.decoSettings, targetRoom, GameObject.Find("_Lights").transform, dungeon, dungeon2, lightStyle);
				//dungeon.data.GenerateLightsForRoom(dungeon2.decoSettings, targetRoom, GameObject.Find("_Lights").transform, lightStyle);
			}
			catch (Exception)
			{
				ETGModConsole.Log("WARNING: Exception caused during GenernateLightsForRoom step on room: " + targetRoom.GetRoomName());
				return null;
			}
			postProcessCellData?.Invoke(targetRoom);

			if (targetRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET) { targetRoom.BuildSecretRoomCover(); }
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("RuntimeTileMap", ".prefab"));
			tk2dTileMap component = gameObject.GetComponent<tk2dTileMap>();
			string str = UnityEngine.Random.Range(10000, 99999).ToString();
			gameObject.name = "Glitch_" + "RuntimeTilemap_" + str;
			component.renderData.name = "Glitch_" + "RuntimeTilemap_" + str + " Render Data";
			component.Editor__SpriteCollection = dungeon2.tileIndices.dungeonCollection;
			try
			{
				TK2DDungeonAssembler.RuntimeResizeTileMap(component, a.x + num2 * 2, a.y + num2 * 2, m_tilemap.partitionSizeX, m_tilemap.partitionSizeY);
				IntVector2 intVector5 = new IntVector2(prototype.Width, prototype.Height);
				IntVector2 b3 = basePosition + b;
				IntVector2 intVector6 = intVector3 + b3;
				for (int num4 = -num2; num4 < intVector5.x + num2; num4++)
				{
					for (int num5 = -num2; num5 < intVector5.y + num2 + 2; num5++)
					{
						assembler.BuildTileIndicesForCell(dungeon, dungeon2, component, intVector6.x + num4, intVector6.y + num5);
					}
				}
				RenderMeshBuilder.CurrentCellXOffset = intVector3.x - num2;
				RenderMeshBuilder.CurrentCellYOffset = intVector3.y - num2;
				component.ForceBuild();
				RenderMeshBuilder.CurrentCellXOffset = 0;
				RenderMeshBuilder.CurrentCellYOffset = 0;
				component.renderData.transform.position = new Vector3(intVector3.x - num2, intVector3.y - num2, intVector3.y - num2);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				ETGModConsole.Log("WARNING: Exception occured during RuntimeResizeTileMap / RenderMeshBuilder steps!");
				Debug.Log("WARNING: Exception occured during RuntimeResizeTileMap/RenderMeshBuilder steps!");
				return null;
			}
			targetRoom.OverrideTilemap = component;
			if (allowProceduralLightFixtures)
			{
				for (int num7 = 0; num7 < targetRoom.area.dimensions.x; num7++)
				{
					for (int num8 = 0; num8 < targetRoom.area.dimensions.y + 2; num8++)
					{
						IntVector2 intVector7 = targetRoom.area.basePosition + new IntVector2(num7, num8);
						if (dungeon.data.CheckInBoundsAndValid(intVector7))
						{
							CellData currentCell = dungeon.data[intVector7];
							ExpandTK2DInteriorDecorator.PlaceLightDecorationForCell(dungeon, component, currentCell, intVector7);
						}
					}
				}
			}

			Pathfinder.Instance.InitializeRegion(dungeon.data, targetRoom.area.basePosition + new IntVector2(-3, -3), targetRoom.area.dimensions + new IntVector2(3, 3));

			if (prototype.usesProceduralDecoration && prototype.allowFloorDecoration && allowProceduralDecoration)
			{
				ExpandTK2DInteriorDecorator decorator = new ExpandTK2DInteriorDecorator(assembler);
				try
				{
					decorator.HandleRoomDecoration(targetRoom, dungeon, dungeon2, m_tilemap);
				}
				catch (Exception ex)
				{
					ETGModConsole.Log(ex.ToString());
					ETGModConsole.Log("WARNING: Exception occured during HandleRoomDecoration steps!");
					return null;
				}
			}

			targetRoom.PostGenerationCleanup();

			if (addRoomToMinimap)
			{
				targetRoom.visibility = RoomHandler.VisibilityStatus.VISITED;
				GameManager.Instance.StartCoroutine(Minimap.Instance.RevealMinimapRoomInternal(targetRoom, true, true, false));
				if (isSecretRatExitRoom) { targetRoom.visibility = RoomHandler.VisibilityStatus.OBSCURED; }
			}
			if (addTeleporter) { targetRoom.AddProceduralTeleporterToRoom(); }
			if (addRoomToMinimap) { Minimap.Instance.InitializeMinimap(dungeon.data); }
			DeadlyDeadlyGoopManager.ReinitializeData();
			return targetRoom;
		}

		public static void GenerateLightsForRoomFromOtherTileset(TilemapDecoSettings decoSettings, RoomHandler rh, Transform lightParent, Dungeon dungeon, Dungeon dungeon2, DungeonData.LightGenerationStyle style = DungeonData.LightGenerationStyle.STANDARD)
		{
			if (!dungeon2.roomMaterialDefinitions[rh.RoomVisualSubtype].useLighting) { return; }

			bool flag = decoSettings.lightCookies.Length > 0;
			List<Tuple<IntVector2, float>> list = new List<Tuple<IntVector2, float>>();
			bool flag2 = false;
			List<IntVector2> list2;
			int count;
			if (rh.area != null && !rh.area.IsProceduralRoom && !rh.area.prototypeRoom.usesProceduralLighting)
			{
				list2 = rh.GatherManualLightPositions();
				count = list2.Count;
			}
			else
			{
				flag2 = true;
				list2 = rh.GatherOptimalLightPositions(decoSettings);
				count = list2.Count;
				if (rh.area != null && rh.area.prototypeRoom != null) { PostprocessLightPositions(dungeon, list2, rh); }
			}
			if (rh.area.prototypeRoom != null)
			{
				for (int i = 0; i < rh.area.instanceUsedExits.Count; i++)
				{
					RuntimeRoomExitData runtimeRoomExitData = rh.area.exitToLocalDataMap[rh.area.instanceUsedExits[i]];
					RuntimeExitDefinition runtimeExitDefinition = rh.exitDefinitionsByExit[runtimeRoomExitData];
					if (runtimeRoomExitData.TotalExitLength > 4 && !runtimeExitDefinition.containsLight)
					{
						IntVector2 first = (!runtimeRoomExitData.jointedExit) ? runtimeExitDefinition.GetLinearMidpoint(rh) : (runtimeRoomExitData.ExitOrigin - IntVector2.One);
						list.Add(new Tuple<IntVector2, float>(first, 0.5f));
						runtimeExitDefinition.containsLight = true;
					}
				}
			}
			GlobalDungeonData.ValidTilesets tilesetId = dungeon2.tileIndices.tilesetId;
			float lightCullingPercentage = decoSettings.lightCullingPercentage;
			if (flag2 && lightCullingPercentage > 0f)
			{
				int num = Mathf.FloorToInt(list2.Count * lightCullingPercentage);
				int num2 = Mathf.FloorToInt(list.Count * lightCullingPercentage);
				if (num == 0 && num2 == 0 && list2.Count + list.Count > 4)
				{
					num = 1;
				} while (num > 0 && list2.Count > 0)
				{
					list2.RemoveAt(UnityEngine.Random.Range(0, list2.Count));
					num--;
				} while (num2 > 0 && list.Count > 0)
				{
					list.RemoveAt(UnityEngine.Random.Range(0, list.Count));
					num2--;
				}
			}
			int count2 = list2.Count;
			if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.NONE && (tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON || tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON || tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON) && (flag2 || rh.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.NORMAL || rh.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.HUB || rh.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.CONNECTOR))
			{
				list2.AddRange(rh.GatherPitLighting(decoSettings, list2));
			}
			for (int j = 0; j < list2.Count + list.Count; j++)
			{
				IntVector2 a = IntVector2.NegOne;
				float num3 = 1f;
				bool flag3 = false;
				if (j < list2.Count && j >= count2)
				{
					flag3 = true;
					num3 = 0.6f;
				}
				if (j < list2.Count)
				{
					a = rh.area.basePosition + list2[j];
				}
				else
				{
					a = rh.area.basePosition + list[j - list2.Count].First;
					num3 = list[j - list2.Count].Second;
				}
				bool flag4 = false;
				if (flag && flag2 && a == rh.GetCenterCell()) { flag4 = true; }
				IntVector2 intVector = a + IntVector2.Up;
				bool flag5 = j >= count;
				bool flag6 = false;
				Vector3 b = Vector3.zero;
				if (dungeon.data[a + IntVector2.Up].type == CellType.WALL)
				{
					dungeon.data[intVector].cellVisualData.lightDirection = DungeonData.Direction.NORTH;
					b = Vector3.down;
				}
				else if (dungeon.data[a + IntVector2.Right].type == CellType.WALL)
				{
					dungeon.data[intVector].cellVisualData.lightDirection = DungeonData.Direction.EAST;
				}
				else if (dungeon.data[a + IntVector2.Left].type == CellType.WALL)
				{
					dungeon.data[intVector].cellVisualData.lightDirection = DungeonData.Direction.WEST;
				}
				else if (dungeon.data[a + IntVector2.Down].type == CellType.WALL)
				{
					flag6 = true;
					dungeon.data[intVector].cellVisualData.lightDirection = DungeonData.Direction.SOUTH;
				}
				else
				{
					dungeon.data[intVector].cellVisualData.lightDirection = (DungeonData.Direction)(-1);
				}
				int num4 = rh.RoomVisualSubtype;
				float num5 = 0f;
				if (rh.area.prototypeRoom != null)
				{
					PrototypeDungeonRoomCellData prototypeDungeonRoomCellData = (j >= list2.Count) ? rh.area.prototypeRoom.ForceGetCellDataAtPoint(list[j - list2.Count].First.x, list[j - list2.Count].First.y) : rh.area.prototypeRoom.ForceGetCellDataAtPoint(list2[j].x, list2[j].y);
					if (prototypeDungeonRoomCellData != null && prototypeDungeonRoomCellData.containsManuallyPlacedLight)
					{
						num4 = prototypeDungeonRoomCellData.lightStampIndex;
						num5 = prototypeDungeonRoomCellData.lightPixelsOffsetY / 16f;
					}
				}
				if (num4 < 0 || num4 >= dungeon2.roomMaterialDefinitions.Length) { num4 = 0; }
				DungeonMaterial dungeonMaterial = dungeon2.roomMaterialDefinitions[num4];
				int num6 = -1;
				GameObject original;
				if (style == DungeonData.LightGenerationStyle.FORCE_COLOR || style == DungeonData.LightGenerationStyle.RAT_HALLWAY)
				{
					num6 = 0;
					original = dungeonMaterial.lightPrefabs.elements[0].gameObject;
				}
				else
				{
					original = dungeonMaterial.lightPrefabs.SelectByWeight(out num6, false);
				}
				if ((!dungeonMaterial.facewallLightStamps[num6].CanBeTopWallLight && flag6) || (!dungeonMaterial.facewallLightStamps[num6].CanBeCenterLight && flag5))
				{
					if (num6 >= dungeonMaterial.facewallLightStamps.Count) { num6 = 0; }
					num6 = dungeonMaterial.facewallLightStamps[num6].FallbackIndex;
					original = dungeonMaterial.lightPrefabs.elements[num6].gameObject;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(original, intVector.ToVector3(0f), Quaternion.identity);
				gameObject.transform.parent = lightParent;
				gameObject.transform.position = intVector.ToCenterVector3(intVector.y + decoSettings.lightHeight) + new Vector3(0f, num5, 0f) + b;
				ShadowSystem componentInChildren = gameObject.GetComponentInChildren<ShadowSystem>();
				Light componentInChildren2 = gameObject.GetComponentInChildren<Light>();
				if (componentInChildren2 != null) { componentInChildren2.intensity *= num3; }
				if (style == DungeonData.LightGenerationStyle.FORCE_COLOR || style == DungeonData.LightGenerationStyle.RAT_HALLWAY)
				{
					SceneLightManager component = gameObject.GetComponent<SceneLightManager>();
					if (component)
					{
						Color[] validColors = new Color[] { component.validColors[0] };
						component.validColors = validColors;
					}
				}
				if (flag3 && componentInChildren != null)
				{
					if (componentInChildren2)
					{
						componentInChildren2.range += (dungeon2.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CATACOMBGEON) ? 3 : 5;
					}
					componentInChildren.ignoreCustomFloorLight = true;
				}
				if (flag4 && flag && componentInChildren != null)
				{
					componentInChildren.uLightCookie = decoSettings.GetRandomLightCookie();
					componentInChildren.uLightCookieAngle = UnityEngine.Random.Range(0f, 6.28f);
					componentInChildren2.intensity *= 1.5f;
				}
				if (dungeon.data[intVector].cellVisualData.lightDirection == DungeonData.Direction.NORTH)
				{
					bool flag7 = true;
					for (int k = -2; k < 3; k++)
					{
						if (dungeon.data[intVector + IntVector2.Right * k].type == CellType.FLOOR)
						{
							flag7 = false;
							break;
						}
					}
					if (flag7 && componentInChildren)
					{
						GameObject original2 = (GameObject)BraveResources.Load("Global VFX/Wall_Light_Cookie", ".prefab");
						GameObject gameObject2 = UnityEngine.Object.Instantiate(original2);
						Transform transform = gameObject2.transform;
						transform.parent = gameObject.transform;
						transform.localPosition = Vector3.zero;
						componentInChildren.PersonalCookies.Add(gameObject2.GetComponent<Renderer>());
					}
				}
				CellData cellData = dungeon.data[intVector + new IntVector2(0, Mathf.RoundToInt(num5))];
				if (dungeon2.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)
				{
					dungeon.data[cellData.position + IntVector2.Down].cellVisualData.containsObjectSpaceStamp = true;
				}
				BraveUtility.DrawDebugSquare(cellData.position.ToVector2(), Color.magenta, 1000f);
				cellData.cellVisualData.containsLight = true;
				cellData.cellVisualData.lightObject = gameObject;
				LightStampData facewallLightStampData = dungeonMaterial.facewallLightStamps[num6];
				LightStampData sidewallLightStampData = dungeonMaterial.sidewallLightStamps[num6];
				cellData.cellVisualData.facewallLightStampData = facewallLightStampData;
				cellData.cellVisualData.sidewallLightStampData = sidewallLightStampData;
			}
		}

		public static void PostprocessLightPositions(Dungeon dungeon, List<IntVector2> positions, RoomHandler room)
		{
			CheckCellNeedsAdditionalLight(positions, room, dungeon.data[room.GetCenterCell()]);
			for (int i = 0; i < room.Cells.Count; i++)
			{
				CellData currentCell = dungeon.data[room.Cells[i]];
				CheckCellNeedsAdditionalLight(positions, room, currentCell);
			}
		}

		public static bool CheckCellNeedsAdditionalLight(List<IntVector2> positions, RoomHandler room, CellData currentCell)
		{
			int num = (!room.area.IsProceduralRoom) ? 10 : 20;
			if (currentCell.isExitCell) { return false; }
			if (currentCell.type == CellType.WALL) { return false; }
			bool flag = true;
			for (int i = 0; i < positions.Count; i++)
			{
				int num2 = IntVector2.ManhattanDistance(positions[i] + room.area.basePosition, currentCell.position);
				if (num2 <= num)
				{
					flag = false;
					break;
				}
			}
			if (flag) { positions.Add(currentCell.position - room.area.basePosition); }
			return flag;
		}


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



