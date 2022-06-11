using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using ItemAPI;

namespace Planetside
{

	public static class TilesetToolbox
	{
		public static void SetupFloorSquare(ref DungeonMaterial material, TileIndexGrid[] tileGrid, float density = 0.2f)
		{
			material.floorSquares = tileGrid;
			material.floorSquareDensity = density;
		}


		public static void SetupBeyondRoomMaterial(ref DungeonMaterial material)
		{
			material.facewallLightStamps = new List<LightStampData>
			{
				new LightStampData
				{
					width = 1,
					height = 2,
					relativeWeight = 1,
					placementRule = DungeonTileStampData.StampPlacementRule.ON_LOWER_FACEWALL,
					occupySpace = DungeonTileStampData.StampSpace.WALL_SPACE,
					stampCategory = DungeonTileStampData.StampCategory.MUNDANE,
					preferredIntermediaryStamps = 0,
					intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
					requiresForcedMatchingStyle = false,
					opulence = Opulence.FINE,
					roomTypeData = new List<StampPerRoomPlacementSettings>(),
					indexOfSymmetricPartner = -1,
					preventRoomRepeats = false,
					objectReference = ModPrefabs.shared_auto_001.LoadAsset<GameObject>("DefaultTorchPurple"),
					CanBeCenterLight = true,
					CanBeTopWallLight = true,
					FallbackIndex = 0,
				}
			};

			material.sidewallLightStamps = new List<LightStampData>
			{
				new LightStampData
				{
					width = 1,
					height = 2,
					relativeWeight = 1,
					placementRule = DungeonTileStampData.StampPlacementRule.ON_LOWER_FACEWALL,
					occupySpace = DungeonTileStampData.StampSpace.WALL_SPACE,
					stampCategory = DungeonTileStampData.StampCategory.MUNDANE,
					preferredIntermediaryStamps = 0,
					intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
					requiresForcedMatchingStyle = false,
					opulence = Opulence.FINE,
					roomTypeData = new List<StampPerRoomPlacementSettings>(),
					indexOfSymmetricPartner = -1,
					preventRoomRepeats = false,
					objectReference = ModPrefabs.shared_auto_001.LoadAsset<GameObject>("DefaultTorchSidePurple"),
					CanBeCenterLight = true,
					CanBeTopWallLight = true,
					FallbackIndex = 0,
					
				}
			};

			material.lightPrefabs.elements = new List<WeightedGameObject>
			{
				new WeightedGameObject
				{
					additionalPrerequisites = new DungeonPrerequisite[0],
					forceDuplicatesPossible = false,
					pickupId = -1,
					rawGameObject = ModPrefabs.shared_auto_001.LoadAsset<GameObject>("Gungeon Light (Purple)"),
					weight = 1,
				}
			};
		}

		public static FacewallIndexGridDefinition CreateBlankFacewallIndexGridDefinitionIndexGrid(TileIndexGrid gridToUse)
        {
			var indexGrid = new FacewallIndexGridDefinition();
			indexGrid.canAcceptFloorDecoration = true;
			indexGrid.canAcceptWallDecoration = true;
			indexGrid.canBePlacedInExits = true;
			indexGrid.canExistInCorners = true;

			indexGrid.forcedStampMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY;
			indexGrid.grid = gridToUse;
			indexGrid.hasIntermediaries = false;
			indexGrid.maxIntermediaryBuffer = 1;
			indexGrid.maxIntermediaryLength = 1;

			indexGrid.maxWidth = 100;
			indexGrid.middleSectionSequential = true;
			indexGrid.minIntermediaryBuffer = 100;
			indexGrid.minIntermediaryLength = 100;
			indexGrid.minWidth = 100;
			indexGrid.perTileFailureRate = 0;
			indexGrid.topsMatchBottoms = false;

			return indexGrid;
		}


		public static TileIndexGrid CreateBlankIndexGrid()
		{
			var indexGrid = ScriptableObject.CreateInstance<TileIndexGrid>();
			var yes = new TileIndexList { indexWeights = new List<float> { 0.1f }, indices = new List<int> { -1 } };

			indexGrid.topLeftIndices = yes;
			indexGrid.topIndices = yes;
			indexGrid.topRightIndices = yes;
			indexGrid.leftIndices = yes;
			indexGrid.centerIndices = yes;
			indexGrid.rightIndices = yes;
			indexGrid.bottomLeftIndices = yes;
			indexGrid.bottomIndices = yes;
			indexGrid.bottomRightIndices = yes;
			indexGrid.horizontalIndices = yes;
			indexGrid.verticalIndices = yes;
			indexGrid.topCapIndices = yes;
			indexGrid.rightCapIndices = yes;
			indexGrid.bottomCapIndices = yes;
			indexGrid.leftCapIndices = yes;
			indexGrid.allSidesIndices = yes;
			indexGrid.topLeftNubIndices = yes;
			indexGrid.topRightNubIndices = yes;
			indexGrid.bottomLeftNubIndices = yes;
			indexGrid.bottomRightNubIndices = yes;

			indexGrid.extendedSet = false;

			indexGrid.topCenterLeftIndices = yes;
			indexGrid.topCenterIndices = yes;
			indexGrid.topCenterRightIndices = yes;
			indexGrid.thirdTopRowLeftIndices = yes;
			indexGrid.thirdTopRowCenterIndices = yes;
			indexGrid.thirdTopRowRightIndices = yes;
			indexGrid.internalBottomLeftCenterIndices = yes;
			indexGrid.internalBottomCenterIndices = yes;
			indexGrid.internalBottomRightCenterIndices = yes;

			indexGrid.borderTopNubLeftIndices = yes;
			indexGrid.borderTopNubRightIndices = yes;
			indexGrid.borderTopNubBothIndices = yes;
			indexGrid.borderRightNubTopIndices = yes;
			indexGrid.borderRightNubBottomIndices = yes;
			indexGrid.borderRightNubBothIndices = yes;
			indexGrid.borderBottomNubLeftIndices = yes;
			indexGrid.borderBottomNubRightIndices = yes;
			indexGrid.borderBottomNubBothIndices = yes;
			indexGrid.borderLeftNubTopIndices = yes;
			indexGrid.borderLeftNubBottomIndices = yes;
			indexGrid.borderLeftNubBothIndices = yes;
			indexGrid.diagonalNubsTopLeftBottomRight = yes;
			indexGrid.diagonalNubsTopRightBottomLeft = yes;
			indexGrid.doubleNubsTop = yes;
			indexGrid.doubleNubsRight = yes;
			indexGrid.doubleNubsBottom = yes;
			indexGrid.doubleNubsLeft = yes;
			indexGrid.quadNubs = yes;
			indexGrid.topRightWithNub = yes;
			indexGrid.topLeftWithNub = yes;
			indexGrid.bottomRightWithNub = yes;
			indexGrid.bottomLeftWithNub = yes;

			indexGrid.diagonalBorderNE = yes;
			indexGrid.diagonalBorderSE = yes;
			indexGrid.diagonalBorderSW = yes;
			indexGrid.diagonalBorderNW = yes;
			indexGrid.diagonalCeilingNE = yes;
			indexGrid.diagonalCeilingSE = yes;
			indexGrid.diagonalCeilingSW = yes;
			indexGrid.diagonalCeilingNW = yes;

			indexGrid.CenterCheckerboard = false;
			indexGrid.CheckerboardDimension = 1;
			indexGrid.CenterIndicesAreStrata = false;

			indexGrid.PitInternalSquareGrids = new List<TileIndexGrid>();

			indexGrid.PitInternalSquareOptions = new PitSquarePlacementOptions { CanBeFlushBottom = false, CanBeFlushLeft = false, CanBeFlushRight = false, PitSquareChance = -1 };

			indexGrid.PitBorderIsInternal = false;

			indexGrid.PitBorderOverridesFloorTile = false;

			indexGrid.CeilingBorderUsesDistancedCenters = false;

			indexGrid.UsesRatChunkBorders = false;
			indexGrid.RatChunkNormalSet = yes;
			indexGrid.RatChunkBottomSet = yes;

			indexGrid.PathFacewallStamp = null;
			indexGrid.PathSidewallStamp = null;

			indexGrid.PathPitPosts = yes;
			indexGrid.PathPitPostsBL = yes;
			indexGrid.PathPitPostsBR = yes;

			indexGrid.PathStubNorth = null;
			indexGrid.PathStubEast = null;
			indexGrid.PathStubSouth = null;
			indexGrid.PathStubWest = null;


			return indexGrid;
		}



		public static void SetupTilesetSpriteDef(this tk2dSpriteDefinition def, bool wall = false, bool lower = false)
		{
			try
            {
				def.boundsDataCenter = new Vector3(0.5f, 0.5f, 0f);
				def.boundsDataExtents = new Vector3(1f, 1f, 0f);
				def.untrimmedBoundsDataCenter = new Vector3(0.5f, 0.5f, 0f);
				def.untrimmedBoundsDataExtents = new Vector3(1f, 1f, 0f);
				def.texelSize = new Vector2(0.625f, 0.625f);
				def.position0 = new Vector3(0f, 0f, 0f);
				def.position1 = new Vector3(1f, 0f, 0f);
				def.position2 = new Vector3(0f, 1f, 0f);
				def.position3 = new Vector3(1f, 1f, 0f);
				def.regionH = 16;
				def.regionW = 16;
				if (wall == true)
				{
					def.colliderType = tk2dSpriteDefinition.ColliderType.Box;
					def.collisionLayer = (lower ? CollisionLayer.LowObstacle : CollisionLayer.HighObstacle);
					def.colliderVertices = new Vector3[]
					{
					new Vector3(0f, 1f, -1f),
					new Vector3(0f, 1f, 1f),
					new Vector3(0f, 0f, -1f),
					new Vector3(0f, 0f, 1f),
					new Vector3(1f, 0f, -1f),
					new Vector3(1f, 0f, 1f),
					new Vector3(1f, 1f, -1f),
					new Vector3(1f, 1f, 1f)
					};
				}
			}
			catch (Exception e)
            {
				ETGModConsole.Log(e.ToString());
            }
			
		}
		public static void SetupTilesetSpriteDefForceCollision(this tk2dSpriteDefinition def, Vector3[] vector3s, tk2dSpriteDefinition.ColliderType type = tk2dSpriteDefinition.ColliderType.None, CollisionLayer collisionLayer = CollisionLayer.HighObstacle)
		{
			try
			{
				def.boundsDataCenter = new Vector3(0.5f, 0.5f, 0f);
				def.boundsDataExtents = new Vector3(1f, 1f, 0f);
				def.untrimmedBoundsDataCenter = new Vector3(0.5f, 0.5f, 0f);
				def.untrimmedBoundsDataExtents = new Vector3(1f, 1f, 0f);
				def.texelSize = new Vector2(0.625f, 0.625f);
				def.position0 = new Vector3(0f, 0f, 0f);
				def.position1 = new Vector3(1f, 0f, 0f);
				def.position2 = new Vector3(0f, 1f, 0f);
				def.position3 = new Vector3(1f, 1f, 0f);
				def.regionH = 16;
				def.regionW = 16;
				def.colliderType = type;
				def.collisionLayer = collisionLayer;
				if (vector3s == null)
                {
					def.colliderVertices = new Vector3[0];
				}
				else
                {
					def.colliderVertices = vector3s;
				}
			
			}
			catch (Exception e)
			{
				ETGModConsole.Log(e.ToString());
			}

		}
		public static void SetMaterial(this tk2dSpriteCollectionData collection, int spriteId, int matNum)
		{
			collection.spriteDefinitions[spriteId].material = collection.materials[matNum];
			collection.spriteDefinitions[spriteId].materialId = matNum;
		}

		public static void SetupTileMetaData(this TilesetIndexMetadata metadata, TilesetIndexMetadata.TilesetFlagType type, float weight = 1f, int dungeonRoomSubType = 0, int dungeonRoomSubType2 = -1, int dungeonRoomSubType3 = -1, bool animated = false, bool preventStamps = true)
		{
			metadata.type = type;
			metadata.weight = weight;
			metadata.dungeonRoomSubType = dungeonRoomSubType;
			metadata.secondRoomSubType = dungeonRoomSubType2;
			metadata.thirdRoomSubType = dungeonRoomSubType3;
			metadata.usesAnimSequence = animated;
			metadata.usesNeighborDependencies = false;
			metadata.preventWallStamping = preventStamps;
			metadata.usesPerTileVFX = false;
			metadata.tileVFXPlaystyle = TilesetIndexMetadata.VFXPlaystyle.CONTINUOUS;
			metadata.tileVFXChance = 0f;
			metadata.tileVFXPrefab = null;
			metadata.tileVFXOffset = Vector2.zero;
			metadata.tileVFXDelayTime = 1f;
			metadata.tileVFXDelayVariance = 0f;
			metadata.tileVFXAnimFrame = 0;
		}

	}


	public class ModRoomPrefabs
    {
        public static DungeonPlaceable GenerateDungeonPlacable(GameObject ObjectPrefab = null, bool spawnsEnemy = false, bool useExternalPrefab = false, bool spawnsItem = false, string EnemyGUID = "479556d05c7c44f3b6abb3b2067fc778", int itemID = 307, Vector2? CustomOffset = null, bool itemHasDebrisObject = true, float spawnChance = 1f)
        {
            AssetBundle m_assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            AssetBundle m_assetBundle2 = ResourceManager.LoadAssetBundle("shared_auto_002");
            AssetBundle m_resourceBundle = ResourceManager.LoadAssetBundle("brave_resources_001");

            // Used with custom DungeonPlacable        
            GameObject ChestBrownTwoItems = m_assetBundle.LoadAsset<GameObject>("Chest_Wood_Two_Items");
            GameObject Chest_Silver = m_assetBundle.LoadAsset<GameObject>("chest_silver");
            GameObject Chest_Green = m_assetBundle.LoadAsset<GameObject>("chest_green");
            GameObject Chest_Synergy = m_assetBundle.LoadAsset<GameObject>("chest_synergy");
            GameObject Chest_Red = m_assetBundle.LoadAsset<GameObject>("chest_red");
            GameObject Chest_Black = m_assetBundle.LoadAsset<GameObject>("Chest_Black");
            GameObject Chest_Rainbow = m_assetBundle.LoadAsset<GameObject>("Chest_Rainbow");
            // GameObject Chest_Rat = m_assetBundle.LoadAsset<GameObject>("Chest_Rat");

            m_assetBundle = null;
            m_assetBundle2 = null;
            m_resourceBundle = null;

            DungeonPlaceableVariant BlueChestVariant = new DungeonPlaceableVariant();
            BlueChestVariant.percentChance = 0.35f;
            BlueChestVariant.unitOffset = new Vector2(1, 0.8f);
            BlueChestVariant.enemyPlaceableGuid = string.Empty;
            BlueChestVariant.pickupObjectPlaceableId = -1;
            BlueChestVariant.forceBlackPhantom = false;
            BlueChestVariant.addDebrisObject = false;
            BlueChestVariant.prerequisites = null;
            BlueChestVariant.materialRequirements = null;
            BlueChestVariant.nonDatabasePlaceable = Chest_Silver;

            DungeonPlaceableVariant BrownChestVariant = new DungeonPlaceableVariant();
            BrownChestVariant.percentChance = 0.28f;
            BrownChestVariant.unitOffset = new Vector2(1, 0.8f);
            BrownChestVariant.enemyPlaceableGuid = string.Empty;
            BrownChestVariant.pickupObjectPlaceableId = -1;
            BrownChestVariant.forceBlackPhantom = false;
            BrownChestVariant.addDebrisObject = false;
            BrownChestVariant.prerequisites = null;
            BrownChestVariant.materialRequirements = null;
            BrownChestVariant.nonDatabasePlaceable = ChestBrownTwoItems;

            DungeonPlaceableVariant GreenChestVariant = new DungeonPlaceableVariant();
            GreenChestVariant.percentChance = 0.25f;
            GreenChestVariant.unitOffset = new Vector2(1, 0.8f);
            GreenChestVariant.enemyPlaceableGuid = string.Empty;
            GreenChestVariant.pickupObjectPlaceableId = -1;
            GreenChestVariant.forceBlackPhantom = false;
            GreenChestVariant.addDebrisObject = false;
            GreenChestVariant.prerequisites = null;
            GreenChestVariant.materialRequirements = null;
            GreenChestVariant.nonDatabasePlaceable = Chest_Green;

            DungeonPlaceableVariant SynergyChestVariant = new DungeonPlaceableVariant();
            SynergyChestVariant.percentChance = 0.2f;
            SynergyChestVariant.unitOffset = new Vector2(1, 0.8f);
            SynergyChestVariant.enemyPlaceableGuid = string.Empty;
            SynergyChestVariant.pickupObjectPlaceableId = -1;
            SynergyChestVariant.forceBlackPhantom = false;
            SynergyChestVariant.addDebrisObject = false;
            SynergyChestVariant.prerequisites = null;
            SynergyChestVariant.materialRequirements = null;
            SynergyChestVariant.nonDatabasePlaceable = Chest_Synergy;

            DungeonPlaceableVariant RedChestVariant = new DungeonPlaceableVariant();
            RedChestVariant.percentChance = 0.15f;
            RedChestVariant.unitOffset = new Vector2(0.5f, 0.5f);
            RedChestVariant.enemyPlaceableGuid = string.Empty;
            RedChestVariant.pickupObjectPlaceableId = -1;
            RedChestVariant.forceBlackPhantom = false;
            RedChestVariant.addDebrisObject = false;
            RedChestVariant.prerequisites = null;
            RedChestVariant.materialRequirements = null;
            RedChestVariant.nonDatabasePlaceable = Chest_Red;

            DungeonPlaceableVariant BlackChestVariant = new DungeonPlaceableVariant();
            BlackChestVariant.percentChance = 0.1f;
            BlackChestVariant.unitOffset = new Vector2(0.5f, 0.5f);
            BlackChestVariant.enemyPlaceableGuid = string.Empty;
            BlackChestVariant.pickupObjectPlaceableId = -1;
            BlackChestVariant.forceBlackPhantom = false;
            BlackChestVariant.addDebrisObject = false;
            BlackChestVariant.prerequisites = null;
            BlackChestVariant.materialRequirements = null;
            BlackChestVariant.nonDatabasePlaceable = Chest_Black;

            DungeonPlaceableVariant RainbowChestVariant = new DungeonPlaceableVariant();
            RainbowChestVariant.percentChance = 0.005f;
            RainbowChestVariant.unitOffset = new Vector2(0.5f, 0.5f);
            RainbowChestVariant.enemyPlaceableGuid = string.Empty;
            RainbowChestVariant.pickupObjectPlaceableId = -1;
            RainbowChestVariant.forceBlackPhantom = false;
            RainbowChestVariant.addDebrisObject = false;
            RainbowChestVariant.prerequisites = null;
            RainbowChestVariant.materialRequirements = null;
            RainbowChestVariant.nonDatabasePlaceable = Chest_Rainbow;

            DungeonPlaceableVariant ItemVariant = new DungeonPlaceableVariant();
            ItemVariant.percentChance = spawnChance;
            if (CustomOffset.HasValue)
            {
                ItemVariant.unitOffset = CustomOffset.Value;
            }
            else
            {
                ItemVariant.unitOffset = Vector2.zero;
            }
            // ItemVariant.unitOffset = new Vector2(0.5f, 0.8f);
            ItemVariant.enemyPlaceableGuid = string.Empty;
            ItemVariant.pickupObjectPlaceableId = itemID;
            ItemVariant.forceBlackPhantom = false;
            if (itemHasDebrisObject)
            {
                ItemVariant.addDebrisObject = true;
            }
            else
            {
                ItemVariant.addDebrisObject = false;
            }
            RainbowChestVariant.prerequisites = null;
            RainbowChestVariant.materialRequirements = null;

            List<DungeonPlaceableVariant> ChestTiers = new List<DungeonPlaceableVariant>();
            ChestTiers.Add(BrownChestVariant);
            ChestTiers.Add(BlueChestVariant);
            ChestTiers.Add(GreenChestVariant);
            ChestTiers.Add(SynergyChestVariant);
            ChestTiers.Add(RedChestVariant);
            ChestTiers.Add(BlackChestVariant);
            ChestTiers.Add(RainbowChestVariant);

            DungeonPlaceableVariant EnemyVariant = new DungeonPlaceableVariant();
            EnemyVariant.percentChance = spawnChance;
            EnemyVariant.unitOffset = Vector2.zero;
            EnemyVariant.enemyPlaceableGuid = EnemyGUID;
            EnemyVariant.pickupObjectPlaceableId = -1;
            EnemyVariant.forceBlackPhantom = false;
            EnemyVariant.addDebrisObject = false;
            EnemyVariant.prerequisites = null;
            EnemyVariant.materialRequirements = null;

            List<DungeonPlaceableVariant> EnemyTiers = new List<DungeonPlaceableVariant>();
            EnemyTiers.Add(EnemyVariant);

            List<DungeonPlaceableVariant> ItemTiers = new List<DungeonPlaceableVariant>();
            ItemTiers.Add(ItemVariant);

            DungeonPlaceable m_cachedCustomPlacable = ScriptableObject.CreateInstance<DungeonPlaceable>();
            m_cachedCustomPlacable.name = "CustomChestPlacable";
            if (spawnsEnemy | useExternalPrefab)
            {
                m_cachedCustomPlacable.width = 2;
                m_cachedCustomPlacable.height = 2;
            }
            else if (spawnsItem)
            {
                m_cachedCustomPlacable.width = 1;
                m_cachedCustomPlacable.height = 1;
            }
            else
            {
                m_cachedCustomPlacable.width = 4;
                m_cachedCustomPlacable.height = 1;
            }
            m_cachedCustomPlacable.roomSequential = false;
            m_cachedCustomPlacable.respectsEncounterableDifferentiator = true;
            m_cachedCustomPlacable.UsePrefabTransformOffset = false;
            m_cachedCustomPlacable.isPassable = true;
            if (spawnsItem)
            {
                m_cachedCustomPlacable.MarkSpawnedItemsAsRatIgnored = true;
            }
            else
            {
                m_cachedCustomPlacable.MarkSpawnedItemsAsRatIgnored = false;
            }

            m_cachedCustomPlacable.DebugThisPlaceable = false;
            if (useExternalPrefab && ObjectPrefab != null)
            {
                DungeonPlaceableVariant ExternalObjectVariant = new DungeonPlaceableVariant();
                ExternalObjectVariant.percentChance = spawnChance;
                if (CustomOffset.HasValue)
                {
                    ExternalObjectVariant.unitOffset = CustomOffset.Value;
                }
                else
                {
                    ExternalObjectVariant.unitOffset = Vector2.zero;
                }
                ExternalObjectVariant.enemyPlaceableGuid = string.Empty;
                ExternalObjectVariant.pickupObjectPlaceableId = -1;
                ExternalObjectVariant.forceBlackPhantom = false;
                ExternalObjectVariant.addDebrisObject = false;
                ExternalObjectVariant.nonDatabasePlaceable = ObjectPrefab;
                List<DungeonPlaceableVariant> ExternalObjectTiers = new List<DungeonPlaceableVariant>();
                ExternalObjectTiers.Add(ExternalObjectVariant);
                m_cachedCustomPlacable.variantTiers = ExternalObjectTiers;
            }
            else if (spawnsEnemy)
            {
                m_cachedCustomPlacable.variantTiers = EnemyTiers;
            }
            else if (spawnsItem)
            {
                m_cachedCustomPlacable.variantTiers = ItemTiers;
            }
            else
            {
                m_cachedCustomPlacable.variantTiers = ChestTiers;
            }
            return m_cachedCustomPlacable;
        }

        public static void InitCustomRooms()
        {
            Mod_RoomList = new List<string>()
            {
                "AbyssTestCombatRoom.room",
                "AbyssTestCombatRoom.room",
                "AbyssTestCombatRoom.room",
                "AbyssTestCombatRoom.room",
                "AbyssTestCombatRoom.room",
                "AbyssTestCombatRoom.room",
                "AbyssTestCombatRoom.room",
                "AbyssTestCombatRoom.room",
                "AbyssTestCombatRoom.room",
                "AbyssTestCombatRoom.room",
                "AbyssTestCombatRoom.room",
                "AbyssTestCombatRoom.room",
                "AbyssTestHubRoom.room",
                "AbyssTestHubRoom.room",
                "AbyssTestHubRoom.room",
                "AbyssTestHubRoom.room",
                "AbyssTestHubRoom.room",
                "AbyssTestHubRoom.room",

            };
            Mod_Entrance_Room = DungeonRoomFactory.BuildFromResource("Planetside/Resources/AbyssRooms/AbyssEntry/TheDeepEntrance.room");
            Mod_Exit_Room = DungeonRoomFactory.BuildFromResource("Planetside/Resources/AbyssRooms/AbyssBossRoom/AbyssTestBossRoom.room");
            Mod_Entrance_Room.category = PrototypeDungeonRoom.RoomCategory.ENTRANCE;



			List<PrototypeDungeonRoom> m_floorNameRooms = new List<PrototypeDungeonRoom>();

            foreach (string name in Mod_RoomList)
            {
                PrototypeDungeonRoom m_room = DungeonRoomFactory.BuildFromResource("Planetside/Resources/AbyssRooms/AbyssCombat/" + name);
                m_floorNameRooms.Add(m_room);

			}

            Mod_Rooms = m_floorNameRooms.ToArray();

            foreach (PrototypeDungeonRoom room in Mod_Rooms)
            {
                ModPrefabs.FloorNameRoomTable.includedRooms.elements.Add(GenerateWeightedRoom(room, 1));
            }

            Mod_Boss = DungeonRoomFactory.BuildFromResource("Planetside/Resources/AbyssRooms/AbyssBossRoom/AbyssTestBossRoom.room");

			Mod_Boss.category = PrototypeDungeonRoom.RoomCategory.BOSS;
            Mod_Boss.subCategoryBoss = PrototypeDungeonRoom.RoomBossSubCategory.FLOOR_BOSS;
            Mod_Boss.subCategoryNormal = PrototypeDungeonRoom.RoomNormalSubCategory.COMBAT;
            Mod_Boss.subCategorySpecial = PrototypeDungeonRoom.RoomSpecialSubCategory.STANDARD_SHOP;
            Mod_Boss.subCategorySecret = PrototypeDungeonRoom.RoomSecretSubCategory.UNSPECIFIED_SECRET;
            Mod_Boss.roomEvents = new List<RoomEventDefinition>() {
                new RoomEventDefinition(RoomEventTriggerCondition.ON_ENTER_WITH_ENEMIES, RoomEventTriggerAction.SEAL_ROOM),
                new RoomEventDefinition(RoomEventTriggerCondition.ON_ENEMIES_CLEARED, RoomEventTriggerAction.UNSEAL_ROOM),
            };
            Mod_Boss.associatedMinimapIcon = ModPrefabs.doublebeholsterroom01.associatedMinimapIcon;
            Mod_Boss.usesProceduralLighting = false;
            Mod_Boss.usesProceduralDecoration = false;
            Mod_Boss.rewardChestSpawnPosition = new IntVector2(25, 20); //Where the reward pedestal spawns, should be changed based on room size
            Mod_Boss.overriddenTilesets = GlobalDungeonData.ValidTilesets.JUNGLEGEON;

            //foreach (PrototypeRoomExit exit in Mod_Boss.exitData.exits) { exit.exitType = PrototypeRoomExit.ExitType.ENTRANCE_ONLY; }
                //RoomBuilder.AddExitToRoom(Mod_Boss, new Vector2(26, 37), DungeonData.Direction.NORTH, PrototypeRoomExit.ExitType.EXIT_ONLY, PrototypeRoomExit.ExitGroup.B);
        }

        public static WeightedRoom GenerateWeightedRoom(PrototypeDungeonRoom Room, float Weight = 1, bool LimitedCopies = true, int MaxCopies = 1, DungeonPrerequisite[] AdditionalPrerequisites = null)
        {
            if (Room == null) { return null; }
            if (AdditionalPrerequisites == null) { AdditionalPrerequisites = new DungeonPrerequisite[0]; }
            return new WeightedRoom() { room = Room, weight = Weight, limitedCopies = LimitedCopies, maxCopies = MaxCopies, additionalPrerequisites = AdditionalPrerequisites };
        }

        public static PrototypeDungeonRoom Mod_Entrance_Room;
        public static PrototypeDungeonRoom Mod_Exit_Room;
        public static PrototypeDungeonRoom[] Mod_Rooms;
        public static PrototypeDungeonRoom Mod_Boss;
        public static List<string> Mod_RoomList; // this will contain all of our mods rooms.

    }


    class ModPrefabs
    {
        public static AssetBundle shared_auto_002;
        public static AssetBundle shared_auto_001;
        //public static AssetBundle ModAssets;
        public static AssetBundle braveResources;

		/*
        private static Dungeon TutorialDungeonPrefab;
        private static Dungeon SewerDungeonPrefab;
        private static Dungeon MinesDungeonPrefab;
        private static Dungeon ratDungeon;
        private static Dungeon CathedralDungeonPrefab;
        private static Dungeon BulletHellDungeonPrefab;
        private static Dungeon ForgeDungeonPrefab;
        private static Dungeon CatacombsDungeonPrefab;
        private static Dungeon NakatomiDungeonPrefab;
		*/
        public static PrototypeDungeonRoom reward_room;
        public static PrototypeDungeonRoom gungeon_rewardroom_1;
        public static PrototypeDungeonRoom shop02;
        public static PrototypeDungeonRoom doublebeholsterroom01;

        public static GenericRoomTable shop_room_table;
        public static GenericRoomTable boss_foyertable;
        public static GenericRoomTable FloorNameRoomTable;
        public static GenericRoomTable SecretRoomTable;

        public static GenericRoomTable CastleRoomTable;
        public static GenericRoomTable Gungeon_RoomTable;
        public static GenericRoomTable SewersRoomTable;
        public static GenericRoomTable AbbeyRoomTable;
        public static GenericRoomTable MinesRoomTable;
        public static GenericRoomTable CatacombsRoomTable;
        public static GenericRoomTable ForgeRoomTable;
        public static GenericRoomTable BulletHellRoomTable;

		

		public static void InitCustomPrefabs()
        {
            //ModAssets = PlanetsideModule.ModAssets;
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            AssetBundle assetBundle2 = ResourceManager.LoadAssetBundle("shared_auto_002");
            shared_auto_001 = assetBundle;
            shared_auto_002 = assetBundle2;
            braveResources = ResourceManager.LoadAssetBundle("brave_resources_001");
			/*
            if (ModAssets is null)
            {
                ETGModConsole.Log("ModAssets is null!");
            }
            */
			/*
						TutorialDungeonPrefab = DungeonDatabase.GetOrLoadByName("Base_Tutorial");
						ratDungeon = DungeonDatabase.GetOrLoadByName("base_resourcefulrat");
			NakatomiDungeonPrefab = DungeonDatabase.GetOrLoadByName("base_nakatomi");

			*/



			reward_room = shared_auto_002.LoadAsset<PrototypeDungeonRoom>("reward room");
            gungeon_rewardroom_1 = shared_auto_002.LoadAsset<PrototypeDungeonRoom>("gungeon_rewardroom_1");
            shop_room_table = shared_auto_002.LoadAsset<GenericRoomTable>("Shop Room Table");
            shop02 = shared_auto_002.LoadAsset<PrototypeDungeonRoom>("shop02");
            boss_foyertable = shared_auto_002.LoadAsset<GenericRoomTable>("Boss Foyers");

            FloorNameRoomTable = ScriptableObject.CreateInstance<GenericRoomTable>();
            FloorNameRoomTable.includedRooms = new WeightedRoomCollection();
            FloorNameRoomTable.includedRooms.elements = new List<WeightedRoom>();
            FloorNameRoomTable.includedRoomTables = new List<GenericRoomTable>(0);

            SecretRoomTable = shared_auto_002.LoadAsset<GenericRoomTable>("secret_room_table_01");

            CastleRoomTable = shared_auto_002.LoadAsset<GenericRoomTable>("Castle_RoomTable");
            Gungeon_RoomTable = shared_auto_002.LoadAsset<GenericRoomTable>("Gungeon_RoomTable");

			var SewerDungeonPrefab = DungeonDatabase.GetOrLoadByName("Base_Sewer");
			var MinesDungeonPrefab = DungeonDatabase.GetOrLoadByName("Base_Mines");
			var CathedralDungeonPrefab = DungeonDatabase.GetOrLoadByName("Base_Cathedral");
			var BulletHellDungeonPrefab = DungeonDatabase.GetOrLoadByName("Base_BulletHell");
			var ForgeDungeonPrefab = DungeonDatabase.GetOrLoadByName("Base_Forge");
			var CatacombsDungeonPrefab = DungeonDatabase.GetOrLoadByName("Base_Catacombs");

			SewersRoomTable = SewerDungeonPrefab.PatternSettings.flows[0].fallbackRoomTable;
            AbbeyRoomTable = CathedralDungeonPrefab.PatternSettings.flows[0].fallbackRoomTable;
            MinesRoomTable = MinesDungeonPrefab.PatternSettings.flows[0].fallbackRoomTable;
            CatacombsRoomTable = CatacombsDungeonPrefab.PatternSettings.flows[0].fallbackRoomTable;
            ForgeRoomTable = ForgeDungeonPrefab.PatternSettings.flows[0].fallbackRoomTable;
            BulletHellRoomTable = BulletHellDungeonPrefab.PatternSettings.flows[0].fallbackRoomTable;

			SewerDungeonPrefab = null;
			MinesDungeonPrefab = null;
			CathedralDungeonPrefab = null;
			BulletHellDungeonPrefab = null;
			ForgeDungeonPrefab = null;
			CatacombsDungeonPrefab = null;
			doublebeholsterroom01 = AbyssDungeonFlows.LoadOfficialFlow("Secret_DoubleBeholster_Flow").AllNodes[2].overrideExactRoom;

			//ENV_Tileset_Abyss = PlanetsideModule.ModAssets.LoadAsset("ENV_Tileset_Abyss") as Texture2D;



			/*
			var beyondCollectionObject = FakePrefab.Clone(marinePastPrefab.tileIndices.dungeonCollection.gameObject);
			beyondCollectionObject.name = "BeyondCollection";
			AbyssTilesetCollection = beyondCollectionObject.GetComponent<tk2dSpriteCollectionData>();

			var baseObj = PlanetsideModule.TilesetAssets.LoadAsset<GameObject>("SpriteCollection");
			ETGModConsole.Log("obj done");
			foreach (Component component in baseObj.GetComponents<Component>())
			{
				ETGModConsole.Log(component.ToString());
				if (component.GetType().ToString().ToLower().Contains("tk2dspritecollectiondata"))
				{
					ETGModConsole.Log("comp done");
					tk2dSpriteDefinition[] uvArray = (tk2dSpriteDefinition[])ReflectionHelper.GetValue(component.GetType().GetField("spriteDefinitions"), component);

					ETGModConsole.Log(uvArray.Length.ToString());
					//var b = uvArray.ToList();
					//b.Sort();
					//uvArray = b.ToArray();

					var material = PlanetsideModule.TilesetAssets.LoadAsset<Material>("assets/sprites/AbyssTileset/SpriteCollection Data/abyssmat.mat");
					ETGModConsole.Log("mat loaded");
					ETGModConsole.Log(AbyssTilesetCollection.materials.Length.ToString());

					var mat1 = new Material(AbyssTilesetCollection.materials[0]);
					var mat2 = new Material(AbyssTilesetCollection.materials[1]);
					var mat3 = new Material(AbyssTilesetCollection.materials[2]);

					AbyssTilesetCollection.material = mat1;

					AbyssTilesetCollection.materials = new Material[] { mat1, mat2, mat3 };
					ETGModConsole.Log("mat done");
					var tex = material.GetTexture("_MainTex");
					tex.filterMode = FilterMode.Point;
					mat1.SetTexture("_MainTex", tex);
					mat2.SetTexture("_MainTex", tex);
					mat3.SetTexture("_MainTex", tex);

					ETGModConsole.Log("tex loaded");
					AbyssTilesetCollection.textures = new Texture[] { tex };
					ETGModConsole.Log("tex done");


					foreach (var def in uvArray)
					{
						bool isWall = (int.Parse(def.name) >= 22 && int.Parse(def.name) <= 28) || (int.Parse(def.name) >= 44 && int.Parse(def.name) <= 50) || (int.Parse(def.name) >= 44 && int.Parse(def.name) <= 50);
						AbyssTilesetCollection.spriteDefinitions[int.Parse(def.name)].uvs = def.uvs.ToArray();
						AbyssTilesetCollection.spriteDefinitions[int.Parse(def.name)].SetupTilesetSpriteDef(isWall, (int.Parse(def.name) >= 44 && int.Parse(def.name) <= 50));

						//beyondCollection.spriteDefinitions[] = def;
					}

					var backupDefs = AbyssTilesetCollection.spriteDefinitions;

					AbyssTilesetCollection.spriteDefinitions = new tk2dSpriteDefinition[704];

					foreach (var def in backupDefs)
					{
						def.name = def.name.Replace("Final_Scenario_Tileset_Pilot/", "");
						//ETGModConsole.Log(def.name);
						AbyssTilesetCollection.spriteDefinitions[int.Parse(def.name)] = def;


						def.name = "ENV_Beyond/" + def.name;
					}
					ETGModConsole.Log("reorder done");
					for (int i = 0; i < 704; i++)
					{
						AbyssTilesetCollection.spriteDefinitions[i].material = AbyssTilesetCollection.materials[0];
						AbyssTilesetCollection.spriteDefinitions[i].materialId = 0;
						//ETGModConsole.Log($"[{i}] {beyondCollection.materials[beyondCollection.spriteDefinitions[i].materialId].name}");

					}

					AbyssTilesetCollection.SetMaterial(0, 1);
					AbyssTilesetCollection.SetMaterial(1, 1);
					AbyssTilesetCollection.SetMaterial(2, 1);
					AbyssTilesetCollection.SetMaterial(3, 1);
					AbyssTilesetCollection.SetMaterial(4, 1);
					AbyssTilesetCollection.SetMaterial(5, 1);
					AbyssTilesetCollection.SetMaterial(6, 1);
					AbyssTilesetCollection.SetMaterial(7, 1);
					AbyssTilesetCollection.SetMaterial(8, 1);
					AbyssTilesetCollection.SetMaterial(9, 1);
					AbyssTilesetCollection.SetMaterial(10, 1);
					AbyssTilesetCollection.SetMaterial(11, 1);
					AbyssTilesetCollection.SetMaterial(12, 1);
					AbyssTilesetCollection.SetMaterial(13, 1);
					AbyssTilesetCollection.SetMaterial(14, 1);
					AbyssTilesetCollection.SetMaterial(15, 1);
					AbyssTilesetCollection.SetMaterial(16, 1);
					AbyssTilesetCollection.SetMaterial(17, 1);
					AbyssTilesetCollection.SetMaterial(18, 1);
					AbyssTilesetCollection.SetMaterial(19, 1);
					AbyssTilesetCollection.SetMaterial(20, 1);

					AbyssTilesetCollection.SetMaterial(42, 1);
					AbyssTilesetCollection.SetMaterial(43, 1);

					AbyssTilesetCollection.SetMaterial(64, 1);
					AbyssTilesetCollection.SetMaterial(65, 1);

					AbyssTilesetCollection.SetMaterial(86, 1);
					AbyssTilesetCollection.SetMaterial(87, 1);

					AbyssTilesetCollection.SetMaterial(242, 2);
					AbyssTilesetCollection.SetMaterial(243, 2);
					AbyssTilesetCollection.SetMaterial(264, 2);
					AbyssTilesetCollection.SetMaterial(265, 2);
					AbyssTilesetCollection.SetMaterial(286, 2);
					AbyssTilesetCollection.SetMaterial(308, 2);
					AbyssTilesetCollection.SetMaterial(330, 2);
					AbyssTilesetCollection.SetMaterial(352, 2);
					AbyssTilesetCollection.SetMaterial(374, 2);
					AbyssTilesetCollection.SetMaterial(396, 2);
					AbyssTilesetCollection.SetMaterial(418, 2);
					AbyssTilesetCollection.SetMaterial(528, 2);
					AbyssTilesetCollection.SetMaterial(529, 2);
					AbyssTilesetCollection.SetMaterial(530, 2);
					AbyssTilesetCollection.SetMaterial(550, 2);



					AbyssTilesetCollection.spriteDefinitions[25].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_LEFTEDGE, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[26].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[27].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, 0.05f, 1);
					AbyssTilesetCollection.spriteDefinitions[28].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_RIGHTEDGE, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[29].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, 0, 1);
					AbyssTilesetCollection.spriteDefinitions[30].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, 0, 1);
					AbyssTilesetCollection.spriteDefinitions[31].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, 0, 1);

					AbyssTilesetCollection.spriteDefinitions[27].metadata.usesAnimSequence = true;

					SimpleTilesetAnimationSequence wallEye = new SimpleTilesetAnimationSequence();

					wallEye.playstyle = SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.DELAYED_LOOP;
					wallEye.loopDelayMin = 6;
					wallEye.loopDelayMax = 25;
					wallEye.loopceptionTarget = -1;
					wallEye.loopceptionMin = 1;
					wallEye.loopceptionMax = 3;
					wallEye.coreceptionMin = 1;
					wallEye.coreceptionMax = 1;
					wallEye.randomStartFrame = true;
					wallEye.entries = new List<SimpleTilesetAnimationSequenceEntry>
					{
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 29,
							frameTime = 5f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 30,
							frameTime = 0.2f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 31,
							frameTime = 0.2f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 27,
							frameTime = 0.6f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 31,
							frameTime = 0.1f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 29,
							frameTime = 0.6f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 31,
							frameTime = 0.1f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 27,
							frameTime = 0.6f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 31,
							frameTime = 0.1f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 30,
							frameTime = 0.2f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 29,
							frameTime = 0.1f
						},
					};

					AbyssTilesetCollection.SpriteIDsWithAnimationSequences.Add(27);
					AbyssTilesetCollection.SpriteDefinedAnimationSequences.Add(wallEye);



					AbyssTilesetCollection.spriteDefinitions[47].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTEDGE, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[48].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[49].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[50].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTEDGE, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[58].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[59].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[60].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[61].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[80].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);
					//AbyssTilesetCollection.spriteDefinitions[80].metadata.usesAnimSequence = true;
					AbyssTilesetCollection.spriteDefinitions[81].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);
					AbyssTilesetCollection.spriteDefinitions[82].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);
					AbyssTilesetCollection.spriteDefinitions[83].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);

					AbyssTilesetCollection.spriteDefinitions[102].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);
					AbyssTilesetCollection.spriteDefinitions[103].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);
					AbyssTilesetCollection.spriteDefinitions[104].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);
					AbyssTilesetCollection.spriteDefinitions[105].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);

					AbyssTilesetCollection.spriteDefinitions[124].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);
					AbyssTilesetCollection.spriteDefinitions[125].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);
					AbyssTilesetCollection.spriteDefinitions[126].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);
					AbyssTilesetCollection.spriteDefinitions[127].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);

					//AbyssTilesetCollection.spriteDefinitions[58].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[111].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[112].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0.01f, 1);

					AbyssTilesetCollection.spriteDefinitions[112].metadata.usesAnimSequence = true;

					SimpleTilesetAnimationSequence floorEye = new SimpleTilesetAnimationSequence();

					floorEye.playstyle = SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.DELAYED_LOOP;
					floorEye.loopDelayMin = 6;
					floorEye.loopDelayMax = 25;
					floorEye.loopceptionTarget = -1;
					floorEye.loopceptionMin = 1;
					floorEye.loopceptionMax = 3;
					floorEye.coreceptionMin = 1;
					floorEye.coreceptionMax = 1;
					floorEye.randomStartFrame = true;
					floorEye.entries = new List<SimpleTilesetAnimationSequenceEntry>
					{
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 112,
							frameTime = 5f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 113,
							frameTime = 0.2f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 114,
							frameTime = 0.2f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 115,
							frameTime = 0.6f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 113,
							frameTime = 0.1f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 115,
							frameTime = 0.6f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 113,
							frameTime = 0.1f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 115,
							frameTime = 0.6f
						},

						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 114,
							frameTime = 0.1f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 113,
							frameTime = 0.2f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 112,
							frameTime = 0.1f
						},
					};

					AbyssTilesetCollection.SpriteIDsWithAnimationSequences.Add(112);
					AbyssTilesetCollection.SpriteDefinedAnimationSequences.Add(floorEye);

					AbyssTilesetCollection.spriteDefinitions[113].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0, 1);
					AbyssTilesetCollection.spriteDefinitions[114].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0, 1);
					AbyssTilesetCollection.spriteDefinitions[115].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0, 1);



					AbyssTilesetCollection.spriteDefinitions[286].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);
					AbyssTilesetCollection.spriteDefinitions[308].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);
					AbyssTilesetCollection.spriteDefinitions[330].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);
					AbyssTilesetCollection.spriteDefinitions[352].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0);


					AbyssTilesetCollection.spriteDefinitions[291].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[292].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[313].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[314].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[335].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[336].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[357].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[358].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[379].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[380].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[381].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[382].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[401].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[402].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[403].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[404].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[423].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[424].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[425].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[426].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[445].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[446].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[447].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[448].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[467].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[469].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[470].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[471].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[489].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[490].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[491].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[492].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[493].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[511].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[512].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[513].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[514].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[515].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					AbyssTilesetCollection.spriteDefinitions[533].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[555].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[577].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);
					AbyssTilesetCollection.spriteDefinitions[599].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 1);

					var waterMat = new Material(dungeon.tileIndices.dungeonCollection.materials[5]);
					waterMat.SetColor("_CausticColor", new Color(0.4f, 0.11f, 0.41f, 0.672f));
					waterMat.SetTexture("_MainTex", tex);
					waterMat.SetTexture("_MaskTex", PlanetsideModule.TilesetAssets.LoadAsset<Texture2D>("atlasrefl0"));

					AbyssTilesetCollection.spriteDefinitions[80].material = waterMat;
					AbyssTilesetCollection.spriteDefinitions[80].materialId = 3;

					AbyssTilesetCollection.spriteDefinitions[124].material = waterMat;
					AbyssTilesetCollection.spriteDefinitions[124].materialId = 3;

					var ihatearrays = AbyssTilesetCollection.materials.ToList();
					ihatearrays.Add(waterMat);
					AbyssTilesetCollection.materials = ihatearrays.ToArray();

					SimpleTilesetAnimationSequence waterAnim = new SimpleTilesetAnimationSequence();

					waterAnim.playstyle = SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.SIMPLE_LOOP;
					waterAnim.loopDelayMin = 1;
					waterAnim.loopDelayMax = 3;
					waterAnim.loopceptionTarget = -1;
					waterAnim.loopceptionMin = 1;
					waterAnim.loopceptionMax = 3;
					waterAnim.coreceptionMin = 1;
					waterAnim.coreceptionMax = 1;
					waterAnim.randomStartFrame = false;
					waterAnim.entries = new List<SimpleTilesetAnimationSequenceEntry>
					{
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 80,
							frameTime = 0.3f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 81,
							frameTime = 0.3f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 82,
							frameTime = 0.3f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 83,
							frameTime = 0.3f
						},
					};

					//beyondCollection.SpriteIDsWithAnimationSequences.Add(80);
					//beyondCollection.SpriteDefinedAnimationSequences.Add(waterAnim);




					break;
				}
			}
			*/


			bool Experimental = false;


			//var dungeon = DungeonDatabase.GetOrLoadByName("base_castle");
			var orLoadByName = DungeonDatabase.GetOrLoadByName("Finalscenario_Soldier");

			/*
			var AbbeyDungeon = DungeonDatabase.GetOrLoadByName("base_bullethell");
			var collection = AbbeyDungeon.tileIndices.dungeonCollection;
			//			collection.spriteDefinitions
			for (int e = 0; e < collection.spriteDefinitions.Length; e++)
            {
				ETGModConsole.Log("============", true);
				tk2dSpriteDefinition dev = collection.spriteDefinitions[e];
				ETGModConsole.Log("Position In List: " + e.ToString(), true);
				if (dev != null)
                {
					if (dev.colliderType != null)
					{
						ETGModConsole.Log("Collider Type: "+dev.colliderType.ToString(), true);
					}
					if (dev.collisionLayer != null)
					{
						ETGModConsole.Log("Collider Type: " + dev.collisionLayer.ToString(), true);
					}
					if (dev.colliderVertices != null)
					{
						ETGModConsole.Log("Collider Vertices Start: ", true);
						for (int n = 0; n < dev.colliderVertices.Length; n++)
                        {
							Vector3 vec3 = dev.colliderVertices[n];
							if (vec3 != null)
                            {
								ETGModConsole.Log("Vector3: "+ vec3.ToString() +"     in position: "+n.ToString(), true);

							}

						}
						ETGModConsole.Log("Collider Vertices End: ", true);
					}
				}
				ETGModConsole.Log("============", true);
			}
			AbbeyDungeon = null;
			*/

			GameObject gameObject = FakePrefab.Clone(orLoadByName.tileIndices.dungeonCollection.gameObject);
			gameObject.name = "AbyssCollection";
			ModPrefabs.AbyssTilesetCollection = gameObject.GetComponent<tk2dSpriteCollectionData>();
			GameObject gameObject2 = PlanetsideModule.TilesetAssets.LoadAsset<GameObject>(Experimental == false ? "AbyssTestCollection" : "ExperimentalCollection");
			ETGModConsole.Log("obj done", false);
			foreach (Component component3 in gameObject2.GetComponents<Component>())
			{
				ETGModConsole.Log(component3.ToString(), false);
				bool flag3 = component3.GetType().ToString().ToLower().Contains("tk2dspritecollectiondata");
				if (flag3)
				{
					ETGModConsole.Log("comp done", false);
					tk2dSpriteDefinition[] array3 = (tk2dSpriteDefinition[])ReflectionHelper.GetValue(component3.GetType().GetField("spriteDefinitions"), component3);
					ETGModConsole.Log(array3.Length.ToString(), false);
					Material material2 = PlanetsideModule.TilesetAssets.LoadAsset<Material>(Experimental == false ? "assets/sprites/Abyss Tileset/AbyssTestCollection Data/atlas0 material.mat" : "assets/sprites/Experimental Tileset/ExperimentalCollection Data/atlas0 material.mat");
					
					//ETGModConsole.Log("mat loaded", false);
					//ETGModConsole.Log(ModPrefabs.AbyssTilesetCollection.materials.Length.ToString(), false);

					Material material3 = new Material(ModPrefabs.AbyssTilesetCollection.materials[0]);
					Material material4 = new Material(ModPrefabs.AbyssTilesetCollection.materials[1]);
					Material material5 = new Material(ModPrefabs.AbyssTilesetCollection.materials[2]);
					ModPrefabs.AbyssTilesetCollection.material = material3;
					ModPrefabs.AbyssTilesetCollection.materials = new Material[]
					{
						material3,
						material4,
						material5
					};
					ETGModConsole.Log("mat done", false);
					Texture texture = material2.GetTexture("_MainTex");
					texture.filterMode = FilterMode.Point;
					material3.SetTexture("_MainTex", texture);
					material4.SetTexture("_MainTex", texture);
					material5.SetTexture("_MainTex", texture);
					ETGModConsole.Log("tex loaded", false);
					ModPrefabs.AbyssTilesetCollection.textures = new Texture[]
					{
						texture
					};
					ETGModConsole.Log("tex done", false);

					List<int> walls = new List<int>()
					{
						
						
						38,
						39,
						
						
						58,
						59,
						60,
						61,
						62,
						66,
						67,
						68,
					};
					List<int> topWalls = new List<int>()
					{
						1,
						2,
						3,
						35,
						36,
						37,
						46,
						47,
						48,
						49,
						50,
						53,
						54,
						55,
						56,
						57,
					};

					//bottom walls
					/*
					 * 61,
					 * 80,
					 * 62
					*/



					foreach (tk2dSpriteDefinition tk2dSpriteDefinition2 in array3)
					{
						bool wall = walls.Contains((int.Parse(tk2dSpriteDefinition2.name))); //|| (int.Parse(tk2dSpriteDefinition2.name) >= 44 && int.Parse(tk2dSpriteDefinition2.name) <= 50) || (int.Parse(tk2dSpriteDefinition2.name) >= 44 && int.Parse(tk2dSpriteDefinition2.name) <= 50);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[int.Parse(tk2dSpriteDefinition2.name)].uvs = tk2dSpriteDefinition2.uvs.ToArray<Vector2>();
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[int.Parse(tk2dSpriteDefinition2.name)].SetupTilesetSpriteDef(wall, topWalls.Contains(int.Parse(tk2dSpriteDefinition2.name)));
					}
				
					//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[38].SetupTilesetSpriteDefForceCollision(CollisionLayer.HighObstacle);
					//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[39].SetupTilesetSpriteDefForceCollision(CollisionLayer.HighObstacle);


					tk2dSpriteDefinition[] spriteDefinitions = ModPrefabs.AbyssTilesetCollection.spriteDefinitions;
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions = new tk2dSpriteDefinition[704];

					foreach (tk2dSpriteDefinition tk2dSpriteDefinition3 in spriteDefinitions)
					{
						tk2dSpriteDefinition3.name = tk2dSpriteDefinition3.name.Replace("Final_Scenario_Tileset_Pilot/", "");
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[int.Parse(tk2dSpriteDefinition3.name)] = tk2dSpriteDefinition3;
						tk2dSpriteDefinition3.name = "ENV_Beyond/" + tk2dSpriteDefinition3.name;
					}
					/*
					foreach (tk2dSpriteDefinition tk2dSpriteDefinition3 in spriteDefinitions)
					{
						tk2dSpriteDefinition3.SetupTilesetSpriteDefForceCollision(null);
					}
					*/

					ETGModConsole.Log("reorder done", false);
					for (int m = 0; m < 704; m++)
					{
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[m].material = ModPrefabs.AbyssTilesetCollection.materials[0];
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[m].materialId = 0;
					}
					


					//Top Wall Colliders
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[37].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[46].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[47].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[48].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[49].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[50].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					//
					//Bottom Wall Colliders
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[1].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[3].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[35].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[36].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[53].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[54].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[55].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[56].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[57].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);

					//Additional Shit
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[38].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[39].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[40].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[41].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);

					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[58].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[59].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);


					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[60].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[61].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[62].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);

					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[66].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[67].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[68].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);

					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[100].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[101].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[102].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[103].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[104].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[105].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[106].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[107].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[108].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[109].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[110].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle);

					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[80].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[81].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[82].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[83].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);

					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[76].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[77].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[78].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[79].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);


					for (int me = 1; me < ModPrefabs.AbyssTilesetCollection.Count; me++)
                    {
						ModPrefabs.AbyssTilesetCollection.SetMaterial(me, 1);
					}

					ModPrefabs.AbyssTilesetCollection.SetMaterial(5, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(6, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(7, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(8, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(9, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(10, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(13, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(14, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(33, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(34, 0);



					ModPrefabs.AbyssTilesetCollection.SetMaterial(96, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(97, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(98, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(99, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(100, 2);
					//ModPrefabs.AbyssTilesetCollection.SetMaterial(101, 2);
					//ModPrefabs.AbyssTilesetCollection.SetMaterial(102, 2);
					//ModPrefabs.AbyssTilesetCollection.SetMaterial(103, 2);

					//ModPrefabs.AbyssTilesetCollection.SetMaterial(10, 1);
					/*
					ModPrefabs.AbyssTilesetCollection.SetMaterial(0, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(1, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(2, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(3, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(4, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(5, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(6, 2);
					*/
					/*
					ModPrefabs.AbyssTilesetCollection.SetMaterial(3, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(4, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(5, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(6, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(7, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(8, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(9, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(10, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(11, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(12, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(13, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(14, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(15, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(16, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(17, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(18, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(19, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(20, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(42, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(43, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(64, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(65, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(86, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(87, 1);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(242, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(243, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(264, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(265, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(286, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(308, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(330, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(352, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(374, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(396, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(418, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(528, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(529, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(530, 2);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(550, 2);
					*/



					for (int me = 1; me < ModPrefabs.AbyssTilesetCollection.Count; me++)
					{
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[me].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 0f, 1, 0, -1, false, true);
					}
					//ModPrefabs.AbyssTilesetCollection.SetMaterial(5, 2);
					//ModPrefabs.AbyssTilesetCollection.SetMaterial(6, 2);


					try
					{

						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[17].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.CHEST_HIGH_WALL, 1f, 1, 0, -1, false, true);
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[18].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DECAL_TILE, 1f, 1, 0, -1, false, true);


						
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[40].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_TOP_NE, 1f, 1, 0, -1, false, true);
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[41].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_TOP_NW, 1f, 1, 0, -1, false, true);
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[44].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_UPPER_NE, 1f, 1, 0, -1, false, true);
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[45].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_UPPER_NW, 1f, 1, 0, -1, false, true);


						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[46].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, 1f, 1, 0, -1, false, true);//DONE

						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[47].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_LEFTCORNER, 1f, 1, 0, -1, false, true);

						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[48].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_LEFTEDGE, 1f, 1, 0, -1, false, true);

						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[49].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_RIGHTCORNER, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[50].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_RIGHTEDGE, 1f, 1, 0, -1, false, true);

						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[51].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_LOWER_NE, 1f, 1, 0, -1, false, true);
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[52].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_LOWER_NW, 1f, 1, 0, -1, false, true);



						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[53].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[54].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTCORNER, 1f, 1, 0, -1, false, true);


						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[55].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTEDGE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[56].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTCORNER, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[57].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTEDGE, 1f, 1, 0, -1, false, true);




						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[58].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.PATTERN_TILE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[44].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_EW, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[45].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_NS, 1f, 1, 0, -1, false, true);
						
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[21].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[22].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[23].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[24].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);


						/*
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[62].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[72].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[73].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[74].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);

						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[75].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0.5f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[82].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0.2f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[83].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0.1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[84].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0.5f, 1, 0, -1, false, true);
						*/
						/*
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[1].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0.7f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[3].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0.3f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[4].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0.03f, 1, 0, -1, false, true);
						*/
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[82].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 2, -1, false, true);
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[83].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 2, -1, false, true);
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[84].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 2, -1, false, true);



						/*
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[4].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0.03f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[13].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, 1f, 1, 0, -1, false, true);
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[14].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, 1f, 1, 0, -1, false, true);
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[15].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, 1f, 1, 0, -1, false, true);
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[16].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, 1f, 1, 0, -1, false, true);


						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[18].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_LEFTEDGE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[19].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_RIGHTEDGE, 1f, 1, 0, -1, false, true);
					


						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[24].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTEDGE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[25].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTEDGE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[26].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_LOWER_NW, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[27].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_LOWER_NE, 1f, 1, 0, -1, false, true);

						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[28].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTCORNER, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[29].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTCORNER, 1f, 1, 0, -1, false, true);

						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[30].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_UPPER_NE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[31].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_UPPER_NW, 1f, 1, 0, -1, false, true);

						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[32].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.CHEST_HIGH_WALL, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[33].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DECAL_TILE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[34].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.PATTERN_TILE, 1f, 1, 0, -1, false, true);


						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[35].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_EW, 1f, 1, 0, -1, false, true);
						*/
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[47].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_NS, 1f, 1, 0, -1, false, true);



						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[31].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);


					
						/*
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[32].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[33].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[34].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[35].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[36].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[37].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[38].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[39].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[40].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);

						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[40].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[41].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[42].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[43].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[44].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[45].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[46].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[47].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[48].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[49].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
						*/

						/*
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[4].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[5].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTEDGE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[6].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTEDGE, 1f, 1, 0, -1, false, true);



						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[7].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTEDGE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[8].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTEDGE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[8].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_LOWER_NW, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[8].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_LOWER_NE, 1f, 1, 0, -1, false, true);

						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[9].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTCORNER, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[10].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTCORNER, 1f, 1, 0, -1, false, true);

						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[10].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_UPPER_NE, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[10].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_UPPER_NW, 1f, 1, 0, -1, false, true);



						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[11].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_EW, 1f, 1, 0, -1, false, true);
						ModPrefabs.AbyssTilesetCollection.spriteDefinitions[12].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_NS, 1f, 1, 0, -1, false, true);
						*/

						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[5].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.a, 1f, 1, 0, -1, false, true);
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[5].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTEDGE, 1f, 1, 0, -1, false, true);
						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[5].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTEDGE, 1f, 1, 0, -1, false, true);


						//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.a
					}
					catch (Exception e)
					{
						ETGModConsole.Log(e.ToString());
					}


					//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[5].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.a, 1f, 1, -1, -1, false, true);



					/*
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.usesAnimSequence = true;

					}
					catch (Exception e)
					{
						ETGModConsole.Log(e.ToString());
					}



					//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.a, 1f, 1, -1, -1, false, true);
					/*
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.usesAnimSequence = true;
					SimpleTilesetAnimationSequence simpleTilesetAnimationSequence = new SimpleTilesetAnimationSequence();
					simpleTilesetAnimationSequence.playstyle = SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.DELAYED_LOOP;
					simpleTilesetAnimationSequence.loopDelayMin = 6f;
					simpleTilesetAnimationSequence.loopDelayMax = 25f;
					simpleTilesetAnimationSequence.loopceptionTarget = -1;
					simpleTilesetAnimationSequence.loopceptionMin = 1;
					simpleTilesetAnimationSequence.loopceptionMax = 3;
					simpleTilesetAnimationSequence.coreceptionMin = 1;
					simpleTilesetAnimationSequence.coreceptionMax = 1;
					simpleTilesetAnimationSequence.randomStartFrame = true;
					simpleTilesetAnimationSequence.entries = new List<SimpleTilesetAnimationSequenceEntry>
					{
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 29,
							frameTime = 5f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 30,
							frameTime = 0.2f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 31,
							frameTime = 0.2f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 27,
							frameTime = 0.6f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 31,
							frameTime = 0.1f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 29,
							frameTime = 0.6f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 31,
							frameTime = 0.1f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 27,
							frameTime = 0.6f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 31,
							frameTime = 0.1f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 30,
							frameTime = 0.2f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 29,
							frameTime = 0.1f
						}
					};
					*/


					//ModPrefabs.AbyssTilesetCollection.SpriteIDsWithAnimationSequences.Add(27);
					//ModPrefabs.AbyssTilesetCollection.SpriteDefinedAnimationSequences.Add(simpleTilesetAnimationSequence);

					
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					

					//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[0].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0.01f, 1, -1, -1, false, true);

					/*
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].metadata.usesAnimSequence = true;
					SimpleTilesetAnimationSequence simpleTilesetAnimationSequence2 = new SimpleTilesetAnimationSequence();
					simpleTilesetAnimationSequence2.playstyle = SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.DELAYED_LOOP;
					simpleTilesetAnimationSequence2.loopDelayMin = 6f;
					simpleTilesetAnimationSequence2.loopDelayMax = 25f;
					simpleTilesetAnimationSequence2.loopceptionTarget = -1;
					simpleTilesetAnimationSequence2.loopceptionMin = 1;
					simpleTilesetAnimationSequence2.loopceptionMax = 3;
					simpleTilesetAnimationSequence2.coreceptionMin = 1;
					simpleTilesetAnimationSequence2.coreceptionMax = 1;
					simpleTilesetAnimationSequence2.randomStartFrame = true;
					simpleTilesetAnimationSequence2.entries = new List<SimpleTilesetAnimationSequenceEntry>
					{
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 112,
							frameTime = 5f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 113,
							frameTime = 0.2f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 114,
							frameTime = 0.2f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 115,
							frameTime = 0.6f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 113,
							frameTime = 0.1f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 115,
							frameTime = 0.6f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 113,
							frameTime = 0.1f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 115,
							frameTime = 0.6f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 114,
							frameTime = 0.1f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 113,
							frameTime = 0.2f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 112,
							frameTime = 0.1f
						}
					};
					ModPrefabs.AbyssTilesetCollection.SpriteIDsWithAnimationSequences.Add(112);
					ModPrefabs.AbyssTilesetCollection.SpriteDefinedAnimationSequences.Add(simpleTilesetAnimationSequence2);
					*/

					//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[0].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0f, 1, -1, -1, false, true);
					//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[0].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0f, 1, -1, -1, false, true);
					//ModPrefabs.AbyssTilesetCollection.spriteDefinitions[0].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 0f, 1, -1, -1, false, true);

					/*
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[286].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[308].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[330].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[352].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 0, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[291].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[292].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[313].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[314].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[335].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[336].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[357].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[358].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[379].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[380].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[381].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[382].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[401].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[402].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[403].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[404].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[423].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[424].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[425].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[426].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[445].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[446].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[447].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[448].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[467].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[469].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[470].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[471].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[489].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[490].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[491].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[492].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[493].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[511].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[512].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[513].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[514].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[515].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[533].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[555].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[577].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					ModPrefabs.AbyssTilesetCollection.spriteDefinitions[599].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1f, 1, -1, -1, false, true);
					*/
					//Material material6 = new Material(dungeon.tileIndices.dungeonCollection.materials[5]);
					//material6.SetColor("_CausticColor", new Color(0.4f, 0.11f, 0.41f, 0.672f));
					//material6.SetTexture("_MainTex", texture);
					//material6.SetTexture("_MaskTex", PlanetsideModule.TilesetAssets.LoadAsset<Texture2D>("assets/sprites/Abyss Tileset/AbyssTestCollection Data/atlas0.png"));//PlanetsideModule.TilesetAssets.LoadAsset<Texture2D>("atlas0"));

					//		ModPrefabs.AbyssTilesetCollection.spriteDefinitions[1].material = material6;
					//	ModPrefabs.AbyssTilesetCollection.spriteDefinitions[1].materialId = 3;
					//		ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].material = material6;
					//	ModPrefabs.AbyssTilesetCollection.spriteDefinitions[2].materialId = 3;
					//List<Material> list7 = ModPrefabs.AbyssTilesetCollection.materials.ToList<Material>();
					//list7.Add(material6);
					//ModPrefabs.AbyssTilesetCollection.materials = list7.ToArray();

					/*
					SimpleTilesetAnimationSequence simpleTilesetAnimationSequence3 = new SimpleTilesetAnimationSequence();
					simpleTilesetAnimationSequence3.playstyle = SimpleTilesetAnimationSequence.TilesetSequencePlayStyle.SIMPLE_LOOP;
					simpleTilesetAnimationSequence3.loopDelayMin = 1f;
					simpleTilesetAnimationSequence3.loopDelayMax = 3f;
					simpleTilesetAnimationSequence3.loopceptionTarget = -1;
					simpleTilesetAnimationSequence3.loopceptionMin = 1;
					simpleTilesetAnimationSequence3.loopceptionMax = 3;
					simpleTilesetAnimationSequence3.coreceptionMin = 1;
					simpleTilesetAnimationSequence3.coreceptionMax = 1;
					simpleTilesetAnimationSequence3.randomStartFrame = false;
					simpleTilesetAnimationSequence3.entries = new List<SimpleTilesetAnimationSequenceEntry>
					{
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 80,
							frameTime = 0.3f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 81,
							frameTime = 0.3f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 82,
							frameTime = 0.3f
						},
						new SimpleTilesetAnimationSequenceEntry
						{
							entryIndex = 83,
							frameTime = 0.3f
						}
					};
					*/
					break;
				}
			}

			orLoadByName = null;
			//dungeon = null;
		}
		public static tk2dSpriteCollectionData AbyssTilesetCollection;
	}
}
