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

		public enum TileType
		{
			WallTop,
			WallBottom,
			//WallRightTop,
			//WallLeftTop,
			//WallRightBottom,
			//WallLeftBottom,
		}

		public static void SetWallCollistion(this tk2dSpriteDefinition def, TileType type)
		{

			def.colliderType = tk2dSpriteDefinition.ColliderType.Box;
			switch (type)
			{
				case TileType.WallTop:
					def.colliderVertices = new Vector3[] { new Vector3(0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.1f) };
					def.collisionLayer = CollisionLayer.HighObstacle;
					break;
				case TileType.WallBottom:
					def.colliderVertices = new Vector3[] { new Vector3(0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.1f) };
					def.collisionLayer = CollisionLayer.LowObstacle;
					break;
			}
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
		public static void SetMaterial(this tk2dSpriteCollectionData collection, int spriteId, int matNum, bool textureChange = false)
		{
			if (textureChange == true)
			{
				collection.materials[matNum].mainTexture = collection.spriteDefinitions[spriteId].material.mainTexture;
			}
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
				"BasicSquareRoom.room",
				"LRoomAbyss.room",
				"RoomWithExtraSoos.room",
				"TunnelWays.room",
				"SmallRoompewPew.room",
				"ExtraSpicy.room",
				"SmallTunnelWithPew.room",
				"BigBloaty1.room",
				"EvilHAHAHAHAHA.room",
				"IDoALittleTrollingHopeYouGetGood.room",
				"TurretsBadGuysAndExplosions.room",
				"VIOLENCE.room",
				"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.room",
				"H.room",
				"deathdeath.room",
				"painpain.room",
				"GetNaenaed.room",
				"ROCKEDDON.room",
				"jknbfkjhfdskjhfdsjhkfdsjhkdfs.room",
				"jkhkjhdsfkjdfskjfdskjhfdskjhkjhfdkjsh.room",
				"IHateThePlayer.room",
				"hhhhhhhhhhhhhhhhhhhhhhhhhh.room",
				"GoodFuckingLuckLmao.room",
				"FUCK.room",
				"amogus.room",
				"afhjudsadhsadssdahjadjhs.room",
				"AABBADONNNN.room",
				"BasicWarpZone.room",
				"dfkfdkjkjdddddkkkkkkk.room",
				"evilevilevil.room",
				"HellHeheheHahaYe.room",
				"yeah.room",
				"whyarealltheseroomssquare.room",
				"yeah_2.room",
				"papersplease.room",
				"mhm.room",
                "amonge.room",
                "hardcorebuildup.room",
                "kjhisashis.room",
                "smalllllll.room",
                "yeayeyeyeye.room",
                "turrets_of_hate.room",
                "his_name_is_big.room",
                "cry_of_the_weeper.room",
                "teuyawe.room",
                "oh_so_basic.room",
                "super_basic_but_funnyspahe.room",
                "very_beautifil_very_powerful.room"
            };
			Mod_RoomList_HUB = new List<string>()
			{
				"BigAssmegaHubRoom.room",
				"ConnectoryRoomy.room",
				"BigHell.room",
				"AnotehrHubRoom.room",
				"CrossHouse.room",
				"forgefireroar.room",
                "chansomdautomne.room",
                "open_up_a_little.room"
            };

			Mod_Entrance_Room = DungeonRoomFactory.BuildFromResource("Planetside/Resources/AbyssRooms/AbyssEntry/TheDeepEntrance.room");
            Mod_Exit_Room = DungeonRoomFactory.BuildFromResource("Planetside/Resources/AbyssRooms/AbyssBossRoom/AbyssTestBossRoom.room");
            Mod_Entrance_Room.category = PrototypeDungeonRoom.RoomCategory.ENTRANCE;
			VoidMuncherRoom = DungeonRoomFactory.BuildFromResource("Planetside/Resources/AbyssRooms/AbyssSpecial/SpecialVoidMuncherRoom.room");

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

			List<PrototypeDungeonRoom> m_floorNameRoomsHUB = new List<PrototypeDungeonRoom>();
			foreach (string name in Mod_RoomList_HUB)
			{
				PrototypeDungeonRoom m_room = DungeonRoomFactory.BuildFromResource("Planetside/Resources/AbyssRooms/AbyssHubRooms/" + name);
				m_floorNameRoomsHUB.Add(m_room);

			}
			Mod_Rooms_HUB = m_floorNameRoomsHUB.ToArray();
			foreach (PrototypeDungeonRoom room in Mod_Rooms_HUB)
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

			Loop_De_Loop_Flow = AbyssFlows.BuildPrimaryFlow();


		}

		public static DungeonFlow Loop_De_Loop_Flow;


        public static WeightedRoom GenerateWeightedRoom(PrototypeDungeonRoom Room, float Weight = 1, bool LimitedCopies = true, int MaxCopies = 1, DungeonPrerequisite[] AdditionalPrerequisites = null)
        {
            if (Room == null) { return null; }
            if (AdditionalPrerequisites == null) { AdditionalPrerequisites = new DungeonPrerequisite[0]; }
            return new WeightedRoom() { room = Room, weight = Weight, limitedCopies = LimitedCopies, maxCopies = MaxCopies, additionalPrerequisites = AdditionalPrerequisites };
        }

		public static PrototypeDungeonRoom VoidMuncherRoom;

		public static PrototypeDungeonRoom Mod_Entrance_Room;
        public static PrototypeDungeonRoom Mod_Exit_Room;

        public static PrototypeDungeonRoom[] Mod_Rooms;
		public static PrototypeDungeonRoom[] Mod_Rooms_HUB;

		public static PrototypeDungeonRoom Mod_Boss;
        public static List<string> Mod_RoomList; // this will contain all of our mods rooms.
		public static List<string> Mod_RoomList_HUB; // this will contain all of our mods rooms.
	}


	class ModPrefabs
    {

		public static DungeonMaterial abyssMaterial;
		public static DungeonTileStampData AbyssStampData;

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



			AbyssStampData = ScriptableObject.CreateInstance<DungeonTileStampData>();
			AbyssStampData.name = "AbyssStampData";
			AbyssStampData.spriteStampWeight = 1f;
			AbyssStampData.objectStampWeight = 1f;
			AbyssStampData.tileStampWeight = 1f;

			AbyssStampData.objectStamps = new ObjectStampData[0];
			AbyssStampData.spriteStamps = new SpriteStampData[0];

			AbyssStampData.stamps = new TileStampData[]
			{
				new TileStampData
				{
					width = 1,
					height = 1,
					relativeWeight = 1,
					placementRule = DungeonTileStampData.StampPlacementRule.ON_LOWER_FACEWALL,
					occupySpace = DungeonTileStampData.StampSpace.WALL_SPACE,
					stampCategory = DungeonTileStampData.StampCategory.DECORATIVE,
					preferredIntermediaryStamps = 1,
					intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.PORTRAIT,
					requiresForcedMatchingStyle = false,
					opulence = Opulence.FINE,
					roomTypeData = new List<StampPerRoomPlacementSettings>
					{
						new StampPerRoomPlacementSettings
						{
							roomRelativeWeight = 1,
							roomSubType = 1
						}
					},
					indexOfSymmetricPartner = -1,
					preventRoomRepeats = true,
					stampTileIndices = new List<int>
					{
						131,
					}
				},
				new TileStampData
				{
					width = 1,
					height = 1,
					relativeWeight = 1,
					placementRule = DungeonTileStampData.StampPlacementRule.ON_UPPER_FACEWALL,
					occupySpace = DungeonTileStampData.StampSpace.WALL_SPACE,
					stampCategory = DungeonTileStampData.StampCategory.STRUCTURAL,
					preferredIntermediaryStamps = 1,
					intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.PORTRAIT,
					requiresForcedMatchingStyle = false,
					opulence = Opulence.FINE,
					roomTypeData = new List<StampPerRoomPlacementSettings>
					{
						new StampPerRoomPlacementSettings
						{
							roomRelativeWeight = 1,
							roomSubType = 1
						}
					},
					indexOfSymmetricPartner = -1,
					preventRoomRepeats = false,
					stampTileIndices = new List<int>
					{
						132,
					}
				},
				new TileStampData
				{
					width = 1,
					height = 1,
					relativeWeight = 0.03f,
					placementRule = DungeonTileStampData.StampPlacementRule.ON_LOWER_FACEWALL,
					occupySpace = DungeonTileStampData.StampSpace.WALL_SPACE,
					stampCategory = DungeonTileStampData.StampCategory.STRUCTURAL,
					preferredIntermediaryStamps = 6,
					intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
					requiresForcedMatchingStyle = false,
					opulence = Opulence.OPULENT,
					roomTypeData = new List<StampPerRoomPlacementSettings>
					{
						new StampPerRoomPlacementSettings
						{
							roomRelativeWeight = 1,
							roomSubType = 1
						}
					},
					indexOfSymmetricPartner = -1,
					preventRoomRepeats = false,
					
					stampTileIndices = new List<int>
					{
						133,
					}
				},
				new TileStampData
				{
					width = 1,
					height = 1,
					relativeWeight = 0.03f,
					placementRule = DungeonTileStampData.StampPlacementRule.ON_UPPER_FACEWALL,
					occupySpace = DungeonTileStampData.StampSpace.WALL_SPACE,
					stampCategory = DungeonTileStampData.StampCategory.STRUCTURAL,
					preferredIntermediaryStamps = 4,
					intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.PORTRAIT,
					requiresForcedMatchingStyle = false,
					opulence = Opulence.FINE,
					roomTypeData = new List<StampPerRoomPlacementSettings>
					{
						new StampPerRoomPlacementSettings
						{
							roomRelativeWeight = 1,
							roomSubType = 1
						}
					},
					indexOfSymmetricPartner = -1,
					preventRoomRepeats = false,
					stampTileIndices = new List<int>
					{
						134,
					}
				},
				new TileStampData
				{
					width = 1,
					height = 1,
					relativeWeight = 0.03f,
					placementRule = DungeonTileStampData.StampPlacementRule.ON_LOWER_FACEWALL,
					occupySpace = DungeonTileStampData.StampSpace.WALL_SPACE,
					stampCategory = DungeonTileStampData.StampCategory.STRUCTURAL,
					preferredIntermediaryStamps = 5,
					intermediaryMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
					requiresForcedMatchingStyle = false,
					opulence = Opulence.OPULENT,
					roomTypeData = new List<StampPerRoomPlacementSettings>
					{
						new StampPerRoomPlacementSettings
						{
							roomRelativeWeight = 1,
							roomSubType = 1
						}
					},
					indexOfSymmetricPartner = -1,
					preventRoomRepeats = false,
					stampTileIndices = new List<int>
					{
						135,
					}
				},
				
			};

			abyssMaterial = ScriptableObject.CreateInstance<DungeonMaterial>();
			abyssMaterial.supportsPits = true;
			abyssMaterial.doPitAO = false;
			abyssMaterial.useLighting = true;
			abyssMaterial.supportsDiagonalWalls = false;
			abyssMaterial.carpetIsMainFloor = false;
			abyssMaterial.carpetGrids = new TileIndexGrid[0];
			abyssMaterial.roomCeilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();
			abyssMaterial.additionalPitBorderFlatGrid = TilesetToolbox.CreateBlankIndexGrid();
			abyssMaterial.roomCeilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();
			string[] pathsSmall = new string[]
			{
				"Planetside/Resources/FloorStuff/WallShards/wallshard1_001.png",
				"Planetside/Resources/FloorStuff/WallShards/wallshard1_002.png",
				"Planetside/Resources/FloorStuff/WallShards/wallshard1_003.png",

			};
			string[] pathsLarge = new string[]
			{
				"Planetside/Resources/FloorStuff/WallShards/wallshard2_001.png",
				"Planetside/Resources/FloorStuff/WallShards/wallshard2_002.png",
				"Planetside/Resources/FloorStuff/WallShards/wallshard2_003.png",
				"Planetside/Resources/FloorStuff/WallShards/wallshard2_004.png",

			};
			abyssMaterial.wallShards = new WeightedGameObjectCollection()
			{
				elements = new List<WeightedGameObject>()
				{
					new WeightedGameObject(){ rawGameObject = BreakAbleAPI.BreakableAPIToolbox.GenerateAnimatedDebrisObject(pathsSmall, 4, tk2dSpriteAnimationClip.WrapMode.Once, true, 0.66f, 2f, 360, 180, null, 0.8f).gameObject, weight = 1},
					new WeightedGameObject(){ rawGameObject = BreakAbleAPI.BreakableAPIToolbox.GenerateAnimatedDebrisObject(pathsSmall, 2, tk2dSpriteAnimationClip.WrapMode.Once, true, 1f, 3f, 360, 180, null, 0.8f).gameObject, weight = 0.7f},
					new WeightedGameObject(){ rawGameObject = BreakAbleAPI.BreakableAPIToolbox.GenerateAnimatedDebrisObject(pathsSmall, 7, tk2dSpriteAnimationClip.WrapMode.Once, true, 0.33f, 1.5f, 360, 180, null, 0.8f).gameObject, weight = 0.5f},
				},
            };
			abyssMaterial.bigWallShardDamageThreshold = 20;
			abyssMaterial.bigWallShards = new WeightedGameObjectCollection()
			{
				elements = new List<WeightedGameObject>()
				{
					new WeightedGameObject(){ rawGameObject = BreakAbleAPI.BreakableAPIToolbox.GenerateAnimatedDebrisObject(pathsLarge, 4, tk2dSpriteAnimationClip.WrapMode.Once, true, 1f, 3f, 240, 120, null, 0.85f).gameObject, weight = 0.5f},
					new WeightedGameObject(){ rawGameObject = BreakAbleAPI.BreakableAPIToolbox.GenerateAnimatedDebrisObject(pathsLarge, 7, tk2dSpriteAnimationClip.WrapMode.Once, true, 1f, 3f, 240, 120, null, 1.2f).gameObject, weight = 0.2f},
					new WeightedGameObject(){ rawGameObject = BreakAbleAPI.BreakableAPIToolbox.GenerateAnimatedDebrisObject(pathsLarge, 1, tk2dSpriteAnimationClip.WrapMode.Once, true, 5f, 10f, 240, 120, null, 2f).gameObject, weight = 0.1f},
				},
			};
			abyssMaterial.secretRoomWallShardCollections = new List<GameObject>()
			{
				BreakAbleAPI.BreakableAPIToolbox.GenerateAnimatedDebrisObject(pathsLarge, 4, tk2dSpriteAnimationClip.WrapMode.Once, true, 1f, 3f, 240, 120, null, 0.85f).gameObject,
				BreakAbleAPI.BreakableAPIToolbox.GenerateAnimatedDebrisObject(pathsSmall, 4, tk2dSpriteAnimationClip.WrapMode.Once, true, 0.66f, 2f, 360, 180, null, 0.8f).gameObject
			};

			foreach (WeightedGameObject obj in abyssMaterial.wallShards.elements)
            {
				obj.rawGameObject.AddComponent<TresspassLightController>();
			}
			foreach (WeightedGameObject obj in abyssMaterial.bigWallShards.elements)
			{
				obj.rawGameObject.AddComponent<TresspassLightController>();
			}

			//==============================================================================================================================================================================================
			//==============================================================================================================================================================================================
			//==============================================================================================================================================================================================

			var pitBorderGridCave = TilesetToolbox.CreateBlankIndexGrid();
			pitBorderGridCave.topLeftIndices = new TileIndexList { indices = new List<int> { 70 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.topIndices = new TileIndexList { indices = new List<int> { 71 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.topRightIndices = new TileIndexList { indices = new List<int> { 72 }, indexWeights = new List<float> { 1f } };


			pitBorderGridCave.leftIndices = new TileIndexList { indices = new List<int> { 73 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.rightIndices = new TileIndexList { indices = new List<int> { 74 }, indexWeights = new List<float> { 1f } };


			pitBorderGridCave.bottomLeftIndices = new TileIndexList { indices = new List<int> { 75 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.bottomIndices = new TileIndexList { indices = new List<int> { 76 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.bottomRightIndices = new TileIndexList { indices = new List<int> { 77 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.horizontalIndices = new TileIndexList { indices = new List<int> { 78 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.verticalIndices = new TileIndexList { indices = new List<int> { 79 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.topCapIndices = new TileIndexList { indices = new List<int> { 80 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.rightCapIndices = new TileIndexList { indices = new List<int> { 81 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.bottomCapIndices = new TileIndexList { indices = new List<int> { 82 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.leftCapIndices = new TileIndexList { indices = new List<int> { 83 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.allSidesIndices = new TileIndexList { indices = new List<int> { 84 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.topLeftNubIndices = new TileIndexList { indices = new List<int> { 85 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.topRightNubIndices = new TileIndexList { indices = new List<int> { 88 }, indexWeights = new List<float> { 1f } };


			pitBorderGridCave.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 89 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 90 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.extendedSet = true;


			//pitBorderGridCave.topCenterLeftIndices = new TileIndexList { indices = new List<int> { 89 }, indexWeights = new List<float> { 1f } };
			//pitBorderGridCave.topCenterIndices = new TileIndexList { indices = new List<int> { 90 }, indexWeights = new List<float> { 1f } };


			pitBorderGridCave.topCenterRightIndices = new TileIndexList { indices = new List<int> { 91 }, indexWeights = new List<float> { 1f } };

			pitBorderGridCave.thirdTopRowLeftIndices = new TileIndexList { indices = new List<int> { 92 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.thirdTopRowCenterIndices = new TileIndexList { indices = new List<int> { 93 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.thirdTopRowRightIndices = new TileIndexList { indices = new List<int> { 94 }, indexWeights = new List<float> { 1f } };
			pitBorderGridCave.internalBottomLeftCenterIndices = new TileIndexList { indices = new List<int> { 95 }, indexWeights = new List<float> { 1f } };


			/*
            pitBorderGridCave.internalBottomCenterIndices = new TileIndexList { indices = new List<int> { 101 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.internalBottomRightCenterIndices = new TileIndexList { indices = new List<int> { 102 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderTopNubLeftIndices = new TileIndexList { indices = new List<int> { 103 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderTopNubRightIndices = new TileIndexList { indices = new List<int> { 104 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderTopNubBothIndices = new TileIndexList { indices = new List<int> { 105 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderRightNubTopIndices = new TileIndexList { indices = new List<int> { 106 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderRightNubBottomIndices = new TileIndexList { indices = new List<int> { 107 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderRightNubBothIndices = new TileIndexList { indices = new List<int> { 108 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderBottomNubLeftIndices = new TileIndexList { indices = new List<int> { 109 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderBottomNubRightIndices = new TileIndexList { indices = new List<int> { 110 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderBottomNubBothIndices = new TileIndexList { indices = new List<int> { 111 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderLeftNubTopIndices = new TileIndexList { indices = new List<int> { 112 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderLeftNubBottomIndices = new TileIndexList { indices = new List<int> { 113 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderLeftNubBothIndices = new TileIndexList { indices = new List<int> { 114 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalNubsTopLeftBottomRight = new TileIndexList { indices = new List<int> { 115 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalNubsTopRightBottomLeft = new TileIndexList { indices = new List<int> { 116 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.doubleNubsTop = new TileIndexList { indices = new List<int> { 117 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.doubleNubsRight = new TileIndexList { indices = new List<int> { 118 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.doubleNubsBottom = new TileIndexList { indices = new List<int> { 119 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.doubleNubsLeft = new TileIndexList { indices = new List<int> { 120 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.quadNubs = new TileIndexList { indices = new List<int> { 121 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topRightWithNub = new TileIndexList { indices = new List<int> { 122 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topLeftWithNub = new TileIndexList { indices = new List<int> { 123 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomRightWithNub = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomLeftWithNub = new TileIndexList { indices = new List<int> { 125 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalBorderNE = new TileIndexList { indices = new List<int> { 126 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalBorderSE = new TileIndexList { indices = new List<int> { 127 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalBorderSW = new TileIndexList { indices = new List<int> { 128 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalBorderNW = new TileIndexList { indices = new List<int> { 129 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalCeilingNE = new TileIndexList { indices = new List<int> { 130 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalCeilingSE = new TileIndexList { indices = new List<int> { 130 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalCeilingSW = new TileIndexList { indices = new List<int> { 130 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalCeilingNW = new TileIndexList { indices = new List<int> { 130 }, indexWeights = new List<float> { 1f } };
            */
			pitBorderGridCave.CenterCheckerboard = false;
			pitBorderGridCave.CheckerboardDimension = 0;

			pitBorderGridCave.CenterIndicesAreStrata = false;
			pitBorderGridCave.PitInternalSquareGrids = new List<TileIndexGrid>() { pitBorderGridCave };





			pitBorderGridCave.PitInternalSquareOptions = new PitSquarePlacementOptions() { CanBeFlushBottom = true, CanBeFlushLeft = true, CanBeFlushRight = true, PitSquareChance = 1 };
			pitBorderGridCave.PitBorderIsInternal = false;
			pitBorderGridCave.PitBorderOverridesFloorTile = false;
			pitBorderGridCave.CeilingBorderUsesDistancedCenters = false;
			pitBorderGridCave.UsesRatChunkBorders = false;
			pitBorderGridCave.RatChunkNormalSet = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
			pitBorderGridCave.RatChunkBottomSet = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
			//pitBorderGridCave.PathFacewallStamp = ob;
			//pitBorderGridCave.PathSidewallStamp =;
			pitBorderGridCave.PathPitPosts = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
			pitBorderGridCave.PathPitPostsBL = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
			pitBorderGridCave.PathPitPostsBR = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
			//pitBorderGridCave.PathStubNorth = ;
			//pitBorderGridCave.PathStubEast =;
			//pitBorderGridCave.PathStubSouth =;
			//pitBorderGridCave.PathStubWest =;
			abyssMaterial.roomFloorBorderGrid = pitBorderGridCave;
			//abyssMaterial.pitLayoutGrid = pitBorderGridCave;
			//abyssMaterial.pitBorderFlatGrid = pitBorderGridCave;
			//abyssMaterial.additionalPitBorderFlatGrid = pitBorderGridCave;
			//abyssMaterial.decalIndexGrid = pitBorderGridCave;

			//==============================================================================================================================================================================================
			//==============================================================================================================================================================================================
			//==============================================================================================================================================================================================

			var floorSquares = TilesetToolbox.CreateBlankIndexGrid();

			floorSquares.topLeftIndices = new TileIndexList { indices = new List<int> { 25 }, indexWeights = new List<float> { 1f } };
			floorSquares.topRightIndices = new TileIndexList { indices = new List<int> { 26 }, indexWeights = new List<float> { 1f } };
			floorSquares.bottomLeftIndices = new TileIndexList { indices = new List<int> { 27 }, indexWeights = new List<float> { 1f } };
			floorSquares.bottomRightIndices = new TileIndexList { indices = new List<int> { 28 }, indexWeights = new List<float> { 1f } };

			var floorSquares1 = TilesetToolbox.CreateBlankIndexGrid();
			floorSquares1.topLeftIndices = new TileIndexList { indices = new List<int> { 29 }, indexWeights = new List<float> { 0.5f } };
			floorSquares1.topRightIndices = new TileIndexList { indices = new List<int> { 30 }, indexWeights = new List<float> { 0.5f } };
			floorSquares1.bottomLeftIndices = new TileIndexList { indices = new List<int> { 31 }, indexWeights = new List<float> { 0.5f } };
			floorSquares1.bottomRightIndices = new TileIndexList { indices = new List<int> { 32 }, indexWeights = new List<float> { 0.5f } };

			abyssMaterial.floorSquares = new TileIndexGrid[] { floorSquares, floorSquares1 };
			//==============================================================================================================================================================================================
			//==============================================================================================================================================================================================
			//==============================================================================================================================================================================================
			{



				var ceilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();
				ceilingBorderGrid.topLeftIndices = new TileIndexList { indices = new List<int> { 60 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.topIndices = new TileIndexList { indices = new List<int> { 61 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.topRightIndices = new TileIndexList { indices = new List<int> { 62 }, indexWeights = new List<float> { 1f } };


				ceilingBorderGrid.leftIndices = new TileIndexList { indices = new List<int> { 38, 40 }, indexWeights = new List<float> { 1f, 0.9f } };

				//ceilingBorderGrid.centerIndices = new TileIndexList { indices = new List<int> { 96 }, indexWeights = new List<float> { 1f } };
				//new TileIndexList { indices = new List<int> { 96, 97, 98, 99 }, indexWeights = new List<float> { 1f, 0.06f, 0.06f, 0.06f } };

				ceilingBorderGrid.rightIndices = new TileIndexList { indices = new List<int> { 39, 41 }, indexWeights = new List<float> { 1f, 0.9f } };


				ceilingBorderGrid.bottomLeftIndices = new TileIndexList { indices = new List<int> { 66 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.bottomIndices = new TileIndexList { indices = new List<int> { 67 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.bottomRightIndices = new TileIndexList { indices = new List<int> { 68 }, indexWeights = new List<float> { 1f } };

				ceilingBorderGrid.verticalIndices = new TileIndexList { indices = new List<int> { 105 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.horizontalIndices = new TileIndexList { indices = new List<int> { 106 }, indexWeights = new List<float> { 1f } };

				//Caps
				ceilingBorderGrid.topCapIndices = new TileIndexList { indices = new List<int> { 103 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.leftCapIndices = new TileIndexList { indices = new List<int> { 101 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.rightCapIndices = new TileIndexList { indices = new List<int> { 102 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.bottomCapIndices = new TileIndexList { indices = new List<int> { 104 }, indexWeights = new List<float> { 1f } };

				//

				//========================================
				//Nubs
				ceilingBorderGrid.doubleNubsRight = new TileIndexList { indices = new List<int> { 80 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.doubleNubsLeft = new TileIndexList { indices = new List<int> { 81 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.doubleNubsTop = new TileIndexList { indices = new List<int> { 82 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.doubleNubsBottom = new TileIndexList { indices = new List<int> { 83 }, indexWeights = new List<float> { 1f } };

				ceilingBorderGrid.quadNubs = new TileIndexList { indices = new List<int> { 84 }, indexWeights = new List<float> { 1f } };
				///Corner Nubs
				ceilingBorderGrid.topRightWithNub = new TileIndexList { indices = new List<int> { 108 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.topLeftWithNub = new TileIndexList { indices = new List<int> { 107 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.bottomRightWithNub = new TileIndexList { indices = new List<int> { 109 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.bottomLeftWithNub = new TileIndexList { indices = new List<int> { 110 }, indexWeights = new List<float> { 1f } };
				//========================================

				ceilingBorderGrid.allSidesIndices = new TileIndexList { indices = new List<int> { 75 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.topLeftNubIndices = new TileIndexList { indices = new List<int> { 76 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.topRightNubIndices = new TileIndexList { indices = new List<int> { 77 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 78 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 79 }, indexWeights = new List<float> { 1f } };

				//ceilingBorderGrid.borderTopNubBothIndices = new TileIndexList { indices = new List<int> { 80 }, indexWeights = new List<float> { 1f } };
				//


				ceilingBorderGrid.borderTopNubLeftIndices = new TileIndexList { indices = new List<int> { 117 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.borderTopNubRightIndices = new TileIndexList { indices = new List<int> { 118 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.borderTopNubBothIndices= new TileIndexList { indices = new List<int> { 119 }, indexWeights = new List<float> { 1f } };


				ceilingBorderGrid.borderBottomNubLeftIndices = new TileIndexList { indices = new List<int> { 115 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.borderBottomNubRightIndices = new TileIndexList { indices = new List<int> { 116 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.borderBottomNubBothIndices = new TileIndexList { indices = new List<int> { 120 }, indexWeights = new List<float> { 1f } }; 


				ceilingBorderGrid.borderLeftNubTopIndices = new TileIndexList { indices = new List<int> { 111 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.borderLeftNubBottomIndices = new TileIndexList { indices = new List<int> { 112 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.borderLeftNubBothIndices = new TileIndexList { indices = new List<int> { 122 }, indexWeights = new List<float> { 1f } };


				ceilingBorderGrid.borderRightNubTopIndices = new TileIndexList { indices = new List<int> { 113 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.borderRightNubBottomIndices = new TileIndexList { indices = new List<int> { 114 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.borderRightNubBothIndices = new TileIndexList { indices = new List<int> { 121 }, indexWeights = new List<float> { 1f } };







				ceilingBorderGrid.diagonalNubsTopLeftBottomRight = new TileIndexList { indices = new List<int> { 90 }, indexWeights = new List<float> { 1f } };
				ceilingBorderGrid.diagonalNubsTopRightBottomLeft = new TileIndexList { indices = new List<int> { 91 }, indexWeights = new List<float> { 1f } };


				ceilingBorderGrid.CheckerboardDimension = 1;

				//ceilingBorderGrid.CeilingBorderUsesDistancedCenters = true;
				//ceilingBorderGrid.centerIndices = new TileIndexList { indices = new List<int> { 100, 101, 102, 103 }, indexWeights = new List<float> { 1f, 0.05f, 0.05f, 0.01f} };
				ceilingBorderGrid.centerIndices = new TileIndexList { indices = new List<int> { 100 }, indexWeights = new List<float> { 1f } };
				//ceilingBorderGrid.CeilingBorderUsesDistancedCenters = true;
				//ceilingBorderGrid.internalBottomCenterIndices = new TileIndexList { indices = new List<int> { 100 }, indexWeights = new List<float> { 1f } }; ;
				//ceilingBorderGrid.internalBottomLeftCenterIndices = new TileIndexList { indices = new List<int> { 100 }, indexWeights = new List<float> { 1f } }; ;
				//ceilingBorderGrid.internalBottomRightCenterIndices = new TileIndexList { indices = new List<int> { 100 }, indexWeights = new List<float> { 1f } }; ;
				//ceilingBorderGrid.UsesRatChunkBorders = true;
				//ceilingBorderGrid.RatChunkNormalSet = new TileIndexList { indices = new List<int> { 58 }, indexWeights = new List<float> { 1f } };
				//ceilingBorderGrid.RatChunkBottomSet = new TileIndexList { indices = new List<int> { 59 }, indexWeights = new List<float> { 1f } };


				abyssMaterial.roomCeilingBorderGrid = ceilingBorderGrid;
				//var borderBorderGrid = TilesetToolbox.CreateBlankIndexGrid();
				//borderBorderGrid.centerIndices = new TileIndexList { indices = new List<int> { 100 }, indexWeights = new List<float> { 1f } };

				//abyssMaterial.exteriorFacadeBorderGrid = borderBorderGrid;



			}

			//ModAssets = PlanetsideModule.ModAssets;
			AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            AssetBundle assetBundle2 = ResourceManager.LoadAssetBundle("shared_auto_002");
            shared_auto_001 = assetBundle;
            shared_auto_002 = assetBundle2;
            braveResources = ResourceManager.LoadAssetBundle("brave_resources_001");
			



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



			bool Experimental = false;


			//var dungeon = DungeonDatabase.GetOrLoadByName("base_castle");
			var orLoadByName = DungeonDatabase.GetOrLoadByName("Finalscenario_Soldier");

			GameObject gameObject = FakePrefab.Clone(orLoadByName.tileIndices.dungeonCollection.gameObject);
			gameObject.name = "AbyssCollection";
			ModPrefabs.AbyssTilesetCollection = gameObject.GetComponent<tk2dSpriteCollectionData>();
			GameObject gameObject2 = PlanetsideModule.TilesetAssets.LoadAsset<GameObject>(Experimental == false ? "AbyssTestCollection" : "ExperimentalCollection");
			//ETGModConsole.Log("obj done", false);
			foreach (Component component3 in gameObject2.GetComponents<Component>())
			{
				//ETGModConsole.Log(component3.ToString(), false);
				bool flag3 = component3.GetType().ToString().ToLower().Contains("tk2dspritecollectiondata");
				if (flag3)
				{
					//ETGModConsole.Log("comp done", false);
					tk2dSpriteDefinition[] array3 = (tk2dSpriteDefinition[])ReflectionHelper.GetValue(component3.GetType().GetField("spriteDefinitions"), component3);
					//ETGModConsole.Log(array3.Length.ToString(), false);
					Material material2 = PlanetsideModule.TilesetAssets.LoadAsset<Material>(Experimental == false ? "assets/sprites/Abyss Tileset/AbyssTestCollection Data/atlas0 material.mat" : "assets/sprites/Experimental Tileset/ExperimentalCollection Data/atlas0 material.mat");
					
					//ETGModConsole.Log("mat loaded", false);
					//ETGModConsole.Log(ModPrefabs.AbyssTilesetCollection.materials.Length.ToString(), false);

					Material material3 = new Material(ModPrefabs.AbyssTilesetCollection.materials[0]);
					Material material4 = new Material(ModPrefabs.AbyssTilesetCollection.materials[1]);
					Material material5 = new Material(ModPrefabs.AbyssTilesetCollection.materials[2]);
					Material material6 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
					material6.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
					material6.SetFloat("_EmissiveColorPower", 1.55f);
					material6.SetFloat("_EmissivePower", 30);
					ModPrefabs.AbyssTilesetCollection.material = material3;
					ModPrefabs.AbyssTilesetCollection.materials = new Material[]
					{
						material3,
						material4,
						material5,
						material6
					};
					//ETGModConsole.Log("mat done", false);
					Texture texture = material2.GetTexture("_MainTex");
					texture.filterMode = FilterMode.Point;
					material3.SetTexture("_MainTex", texture);
					material4.SetTexture("_MainTex", texture);
					material5.SetTexture("_MainTex", texture);
					//ETGModConsole.Log("tex loaded", false);
					ModPrefabs.AbyssTilesetCollection.textures = new Texture[]
					{
						texture
					};
					//ETGModConsole.Log("tex done", false);

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

					foreach (int A in walls)
					{
						TilesetToolbox.SetWallCollistion(ModPrefabs.AbyssTilesetCollection.spriteDefinitions[A], TilesetToolbox.TileType.WallBottom);
					}
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

					foreach (int A in topWalls)
                    {
						TilesetToolbox.SetWallCollistion(ModPrefabs.AbyssTilesetCollection.spriteDefinitions[A], TilesetToolbox.TileType.WallTop);
					}

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

					//ETGModConsole.Log("reorder done", false);
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

					ModPrefabs.AbyssTilesetCollection.SetMaterial(131, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(132, 0);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(133, 3, true);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(134, 3, true);
					ModPrefabs.AbyssTilesetCollection.SetMaterial(135, 3, true);

				
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
