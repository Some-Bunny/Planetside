using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace Planetside
{
    public class CustomValidTilesetsClass
    {
        public enum CustomValidTilesets
        {
            GUNGEON = 1,
            CASTLEGEON = 2,
            SEWERGEON = 4,
            CATHEDRALGEON = 8,
            MINEGEON = 16,
            CATACOMBGEON = 32,
            FORGEGEON = 64,
            HELLGEON = 128,
            SPACEGEON = 256,
            PHOBOSGEON = 512,
            WESTGEON = 1024,
            OFFICEGEON = 2048,
            BELLYGEON = 4096,
            JUNGLEGEON = 8192,
            FINALGEON = 16384,
            RATGEON = 32768,
            PLANETSIDEGEON = 20231101 //DO NOT USE THIS NUMBER TO AVOID CONFLICTS
        }
    }

    public class AbyssDungeon
    {
        public static GameLevelDefinition AbyssDefinition;
        public static GameObject GameManagerObject;
        public static tk2dSpriteCollectionData gofuckyourself;

        public static Dungeon GetOrLoadByNameHook(Func<string, Dungeon> orig, string name)
        {
            //string dungeonPrefabTemplate = "Base_ResourcefulRat";
            if (name.ToLower() == "base_abyss")//THIS STRING MUST BE THE NAME OF YOUR FLOORS DUNGEON PREFAB PATH
            {
                DebugTime.RecordStartTime();
                DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[] { name });
                return AbyssDungeon.AbyssGeon(GetOrLoadByName_Orig("Base_ResourcefulRat"));
            }
            else
            {
                return orig(name);
            }
        }

        public static Hook getOrLoadByName_Hook;
        public static void InitCustomDungeon()
        {
            getOrLoadByName_Hook = new Hook(
                typeof(DungeonDatabase).GetMethod("GetOrLoadByName", BindingFlags.Static | BindingFlags.Public),
                typeof(AbyssDungeon).GetMethod("GetOrLoadByNameHook", BindingFlags.Static | BindingFlags.Public)
            );


            AssetBundle braveResources = ResourceManager.LoadAssetBundle("brave_resources_001");
            GameManagerObject = braveResources.LoadAsset<GameObject>("_GameManager");

            AbyssDefinition = new GameLevelDefinition()
            {
                dungeonSceneName = "tt_abyss", //this is the name we will use whenever we want to load our dungeons scene
                dungeonPrefabPath = "base_abyss", //this is what we will use when we want to acess our dungeon prefab
                priceMultiplier = 1.5f, //multiplies how much things cost in the shop
                secretDoorHealthMultiplier = 1, //multiplies how much health secret room doors have, aka how many shots you will need to expose them
                enemyHealthMultiplier = 2, //multiplies how much health enemies have
                damageCap = 300, // damage cap for regular enemies
                bossDpsCap = 78, // damage cap for bosses
                flowEntries = new List<DungeonFlowLevelEntry>(0),
                predefinedSeeds = new List<int>(0)
            };

            // sets the level definition of the GameLevelDefinition in GameManager.Instance.customFloors if it exists
            foreach (GameLevelDefinition levelDefinition in GameManager.Instance.customFloors)
            {
                if (levelDefinition.dungeonSceneName == "tt_abyss") { AbyssDefinition = levelDefinition; }
            }

            GameManager.Instance.customFloors.Add(AbyssDefinition);
            GameManagerObject.GetComponent<GameManager>().customFloors.Add(AbyssDefinition);
        }
        public static Dungeon AbyssGeon(Dungeon dungeon)
        {
            var MinesDungeonPrefab = GetOrLoadByName_Orig("Base_Mines");
            var CatacombsPrefab = GetOrLoadByName_Orig("Base_Catacombs");
            var RatDungeonPrefab = GetOrLoadByName_Orig("Base_ResourcefulRat");
            var MarinePastPrefab = DungeonDatabase.GetOrLoadByName("Finalscenario_Soldier");




            /*
            var pitGridCave = TilesetToolbox.CreateBlankIndexGrid();
            pitGridCave.topLeftIndices = new TileIndexList { indices = new List<int> { 58 }, indexWeights = new List<float> { 1f } };
            pitGridCave.topIndices = new TileIndexList { indices = new List<int> { 58 }, indexWeights = new List<float> { 1f } };
            pitGridCave.topRightIndices = new TileIndexList { indices = new List<int> { 58 }, indexWeights = new List<float> { 1f } };

            pitGridCave.leftIndices = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };
            pitGridCave.centerIndices = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };
            pitGridCave.rightIndices = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };
            pitGridCave.bottomLeftIndices = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };
            pitGridCave.bottomIndices = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };
            pitGridCave.bottomRightIndices = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };


            pitGridCave.horizontalIndices = new TileIndexList { indices = new List<int> { 58 }, indexWeights = new List<float> { 1f } };
            pitGridCave.verticalIndices = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };
            pitGridCave.topCapIndices = new TileIndexList { indices = new List<int> { 58 }, indexWeights = new List<float> { 1f } };
            pitGridCave.rightCapIndices = new TileIndexList { indices = new List<int> { 58 }, indexWeights = new List<float> { 1f } };
            pitGridCave.leftCapIndices = new TileIndexList { indices = new List<int> { 58 }, indexWeights = new List<float> { 1f } };
            pitGridCave.allSidesIndices = new TileIndexList { indices = new List<int> { 58 }, indexWeights = new List<float> { 1f } };

            pitGridCave.extendedSet = true;
            pitGridCave.topCenterLeftIndices = new TileIndexList { indices = new List<int> { 80 }, indexWeights = new List<float> { 1f } };
            pitGridCave.topCenterIndices = new TileIndexList { indices = new List<int> { 80 }, indexWeights = new List<float> { 1f } };
            pitGridCave.topCenterRightIndices = new TileIndexList { indices = new List<int> { 80 }, indexWeights = new List<float> { 1f } };

            pitGridCave.thirdTopRowLeftIndices = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };
            pitGridCave.thirdTopRowCenterIndices = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };
            pitGridCave.thirdTopRowRightIndices = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };
            pitGridCave.CheckerboardDimension = 1;
            pitGridCave.PitInternalSquareOptions = new PitSquarePlacementOptions { CanBeFlushBottom = false, CanBeFlushLeft = false, CanBeFlushRight = false, PitSquareChance = 0.1f };




            FinalScenario_MainMaterial.pitLayoutGrid = pitGridCave;

            var pitBorderGridCave = TilesetToolbox.CreateBlankIndexGrid();

            pitBorderGridCave.topLeftIndices = new TileIndexList { indices = new List<int> { 94 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topIndices = new TileIndexList { indices = new List<int> { 95 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topRightIndices = new TileIndexList { indices = new List<int> { 96 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.leftIndices = new TileIndexList { indices = new List<int> { 116 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.rightIndices = new TileIndexList { indices = new List<int> { 118 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomLeftIndices = new TileIndexList { indices = new List<int> { 138 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomIndices = new TileIndexList { indices = new List<int> { 139 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomRightIndices = new TileIndexList { indices = new List<int> { 140 }, indexWeights = new List<float> { 1f } };

            pitBorderGridCave.topLeftNubIndices = new TileIndexList { indices = new List<int> { 120 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topRightNubIndices = new TileIndexList { indices = new List<int> { 119 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 98 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 97 }, indexWeights = new List<float> { 1f } };

            pitBorderGridCave.diagonalNubsTopLeftBottomRight = new TileIndexList { indices = new List<int> { 142 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalNubsTopRightBottomLeft = new TileIndexList { indices = new List<int> { 141 }, indexWeights = new List<float> { 1f } };

            pitBorderGridCave.PitBorderIsInternal = false;
            pitBorderGridCave.extendedSet = false;
            pitBorderGridCave.PitBorderOverridesFloorTile = false;
            FinalScenario_MainMaterial.pitBorderFlatGrid = pitBorderGridCave;
            FinalScenario_MainMaterial.supportsUpholstery = true;


            TilesetToolbox.SetupBeyondRoomMaterial(ref FinalScenario_MainMaterial);

            DungeonMaterial beyondBrickMaterial = ScriptableObject.CreateInstance<DungeonMaterial>();

            //Tools.SetupBeyondRoomMaterial(ref beyondBrickMaterial);
            beyondBrickMaterial.lightPrefabs = FinalScenario_MainMaterial.lightPrefabs;
            beyondBrickMaterial.facewallLightStamps = FinalScenario_MainMaterial.facewallLightStamps;
            beyondBrickMaterial.sidewallLightStamps = FinalScenario_MainMaterial.sidewallLightStamps;
            beyondBrickMaterial.wallShards = FinalScenario_MainMaterial.wallShards;
            beyondBrickMaterial.bigWallShards = FinalScenario_MainMaterial.bigWallShards;

            beyondBrickMaterial.bigWallShardDamageThreshold = 10;
            beyondBrickMaterial.fallbackVerticalTileMapEffects = FinalScenario_MainMaterial.fallbackVerticalTileMapEffects;
            beyondBrickMaterial.fallbackHorizontalTileMapEffects = FinalScenario_MainMaterial.fallbackHorizontalTileMapEffects;
            beyondBrickMaterial.pitfallVFXPrefab = FinalScenario_MainMaterial.pitfallVFXPrefab;
            beyondBrickMaterial.UsePitAmbientVFX = false;
            beyondBrickMaterial.AmbientPitVFX = new List<GameObject>();
            beyondBrickMaterial.PitVFXMinCooldown = 5;
            beyondBrickMaterial.PitVFXMaxCooldown = 30;
            beyondBrickMaterial.ChanceToSpawnPitVFXOnCooldown = 1;
            beyondBrickMaterial.UseChannelAmbientVFX = false;
            beyondBrickMaterial.ChannelVFXMinCooldown = 1;
            beyondBrickMaterial.ChannelVFXMaxCooldown = 15;
            beyondBrickMaterial.AmbientChannelVFX = new List<GameObject>();
            beyondBrickMaterial.stampFailChance = 0.2f;
            beyondBrickMaterial.overrideTableTable = null;
            beyondBrickMaterial.supportsPits = true;
            beyondBrickMaterial.doPitAO = true;
            beyondBrickMaterial.useLighting = true;
            beyondBrickMaterial.pitsAreOneDeep = false;
            beyondBrickMaterial.supportsDiagonalWalls = false;
            beyondBrickMaterial.supportsUpholstery = true;
            beyondBrickMaterial.carpetIsMainFloor = false;
            var carpetGridBrick = TilesetToolbox.CreateBlankIndexGrid();

            carpetGridBrick.topLeftIndices = new TileIndexList { indices = new List<int> { 278 }, indexWeights = new List<float> { 1f } };//
            carpetGridBrick.topIndices = new TileIndexList { indices = new List<int> { 279 }, indexWeights = new List<float> { 1f } };//
            carpetGridBrick.topRightIndices = new TileIndexList { indices = new List<int> { 280 }, indexWeights = new List<float> { 1f } };
            carpetGridBrick.leftIndices = new TileIndexList { indices = new List<int> { 300 }, indexWeights = new List<float> { 1f } };//
            carpetGridBrick.centerIndices = new TileIndexList { indices = new List<int> { 325, 325, 327, 328, 326 }, indexWeights = new List<float> { 1f, 1, 1, 1, 1 } };
            carpetGridBrick.rightIndices = new TileIndexList { indices = new List<int> { 302 }, indexWeights = new List<float> { 1f } };
            carpetGridBrick.bottomLeftIndices = new TileIndexList { indices = new List<int> { 322 }, indexWeights = new List<float> { 1f } };
            carpetGridBrick.bottomIndices = new TileIndexList { indices = new List<int> { 323 }, indexWeights = new List<float> { 1f } };//
            carpetGridBrick.bottomRightIndices = new TileIndexList { indices = new List<int> { 324 }, indexWeights = new List<float> { 1f } };
            carpetGridBrick.topLeftNubIndices = new TileIndexList { indices = new List<int> { 304 }, indexWeights = new List<float> { 1f } };
            carpetGridBrick.topRightNubIndices = new TileIndexList { indices = new List<int> { 303 }, indexWeights = new List<float> { 1f } };
            carpetGridBrick.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 282 }, indexWeights = new List<float> { 1f } };
            carpetGridBrick.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 281 }, indexWeights = new List<float> { 1f } };
            carpetGridBrick.extendedSet = true;
            carpetGridBrick.CenterIndicesAreStrata = true;

            beyondBrickMaterial.carpetGrids = new TileIndexGrid[]
            {
                carpetGridBrick,
            };
            //beyondBrickMaterial.carpetGrids = new TileIndexGrid[0];
            beyondBrickMaterial.supportsChannels = false;
            beyondBrickMaterial.minChannelPools = 0;
            beyondBrickMaterial.maxChannelPools = 3;
            beyondBrickMaterial.channelTenacity = 0.75f;
            beyondBrickMaterial.channelGrids = new TileIndexGrid[0];
            beyondBrickMaterial.supportsLavaOrLavalikeSquares = false;
            beyondBrickMaterial.lavaGrids = new TileIndexGrid[0];
            beyondBrickMaterial.supportsIceSquares = false;
            beyondBrickMaterial.iceGrids = new TileIndexGrid[0];

            var ceilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();
            ceilingBorderGrid.topLeftIndices = new TileIndexList { indices = new List<int> { 291 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.topIndices = new TileIndexList { indices = new List<int> { 379 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.topRightIndices = new TileIndexList { indices = new List<int> { 313 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.leftIndices = new TileIndexList { indices = new List<int> { 401 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.centerIndices = new TileIndexList { indices = new List<int> { 468 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.rightIndices = new TileIndexList { indices = new List<int> { 423 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomLeftIndices = new TileIndexList { indices = new List<int> { 335 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomIndices = new TileIndexList { indices = new List<int> { 445 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomRightIndices = new TileIndexList { indices = new List<int> { 357 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.horizontalIndices = new TileIndexList { indices = new List<int> { 555 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.verticalIndices = new TileIndexList { indices = new List<int> { 577 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.topCapIndices = new TileIndexList { indices = new List<int> { 467 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.rightCapIndices = new TileIndexList { indices = new List<int> { 511 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomCapIndices = new TileIndexList { indices = new List<int> { 533 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.leftCapIndices = new TileIndexList { indices = new List<int> { 489 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.allSidesIndices = new TileIndexList { indices = new List<int> { 599 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.topLeftNubIndices = new TileIndexList { indices = new List<int> { 493 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.topRightNubIndices = new TileIndexList { indices = new List<int> { 492 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 471 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 470 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.borderTopNubBothIndices = new TileIndexList { indices = new List<int> { 382 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.borderRightNubTopIndices = new TileIndexList { indices = new List<int> { 425 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.borderRightNubBottomIndices = new TileIndexList { indices = new List<int> { 424 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.borderRightNubBothIndices = new TileIndexList { indices = new List<int> { 426 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.borderBottomNubLeftIndices = new TileIndexList { indices = new List<int> { 446 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.borderBottomNubRightIndices = new TileIndexList { indices = new List<int> { 447 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.borderBottomNubBothIndices = new TileIndexList { indices = new List<int> { 448 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.borderLeftNubTopIndices = new TileIndexList { indices = new List<int> { 403 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.borderLeftNubBottomIndices = new TileIndexList { indices = new List<int> { 402 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.borderLeftNubBothIndices = new TileIndexList { indices = new List<int> { 404 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.diagonalNubsTopLeftBottomRight = new TileIndexList { indices = new List<int> { 469 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.diagonalNubsTopRightBottomLeft = new TileIndexList { indices = new List<int> { 491 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.doubleNubsTop = new TileIndexList { indices = new List<int> { 513 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.doubleNubsRight = new TileIndexList { indices = new List<int> { 514 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.doubleNubsBottom = new TileIndexList { indices = new List<int> { 512 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.doubleNubsLeft = new TileIndexList { indices = new List<int> { 515 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.quadNubs = new TileIndexList { indices = new List<int> { 490 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.topRightWithNub = new TileIndexList { indices = new List<int> { 314 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.topLeftWithNub = new TileIndexList { indices = new List<int> { 292 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomRightWithNub = new TileIndexList { indices = new List<int> { 358 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomLeftWithNub = new TileIndexList { indices = new List<int> { 336 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.CheckerboardDimension = 1;


            beyondBrickMaterial.roomCeilingBorderGrid = ceilingBorderGrid;

            var pitBorderGridBrick = TilesetToolbox.CreateBlankIndexGrid();

            pitBorderGridBrick.topLeftIndices = new TileIndexList { indices = new List<int> { 209 }, indexWeights = new List<float> { 1f } };
            pitBorderGridBrick.topIndices = new TileIndexList { indices = new List<int> { 210 }, indexWeights = new List<float> { 1f } };
            pitBorderGridBrick.topRightIndices = new TileIndexList { indices = new List<int> { 211 }, indexWeights = new List<float> { 1f } };
            pitBorderGridBrick.leftIndices = new TileIndexList { indices = new List<int> { 231 }, indexWeights = new List<float> { 1f } };
            pitBorderGridBrick.rightIndices = new TileIndexList { indices = new List<int> { 233 }, indexWeights = new List<float> { 1f } };
            pitBorderGridBrick.bottomLeftIndices = new TileIndexList { indices = new List<int> { 253 }, indexWeights = new List<float> { 1f } };
            pitBorderGridBrick.bottomIndices = new TileIndexList { indices = new List<int> { 254 }, indexWeights = new List<float> { 1f } };
            pitBorderGridBrick.bottomRightIndices = new TileIndexList { indices = new List<int> { 255 }, indexWeights = new List<float> { 1f } };

            pitBorderGridBrick.topLeftNubIndices = new TileIndexList { indices = new List<int> { 235 }, indexWeights = new List<float> { 1f } };
            pitBorderGridBrick.topRightNubIndices = new TileIndexList { indices = new List<int> { 234 }, indexWeights = new List<float> { 1f } };
            pitBorderGridBrick.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 213 }, indexWeights = new List<float> { 1f } };
            pitBorderGridBrick.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 212 }, indexWeights = new List<float> { 1f } };

            pitBorderGridBrick.diagonalNubsTopLeftBottomRight = new TileIndexList { indices = new List<int> { 257 }, indexWeights = new List<float> { 1f } };
            pitBorderGridBrick.diagonalNubsTopRightBottomLeft = new TileIndexList { indices = new List<int> { 256 }, indexWeights = new List<float> { 1f } };

            pitBorderGridBrick.PitBorderIsInternal = false;
            pitBorderGridBrick.extendedSet = false;
            pitBorderGridBrick.PitBorderOverridesFloorTile = false;

            beyondBrickMaterial.pitLayoutGrid = null;
            beyondBrickMaterial.pitBorderFlatGrid = pitBorderGridBrick;
            beyondBrickMaterial.pitBorderRaisedGrid = null;
            beyondBrickMaterial.additionalPitBorderFlatGrid = null;

            beyondBrickMaterial.roomFloorBorderGrid = pitBorderGridBrick;

            //beyondBrickMaterial.outerCeilingBorderGrid = ceilingBorderGrid;
            /// CastleDungeonPrefab.roomMaterialDefinitions[3]..outerCeilingBorderGrid.roomTypeRestriction = -1;
            //beyondBrickMaterial.outerCeilingBorderGrid = CastleDungeonPrefab.roomMaterialDefinitions[3].outerCeilingBorderGrid;
            beyondBrickMaterial.floorSquareDensity = 0.05f;
            beyondBrickMaterial.floorSquares = null;



            var brickFaceWallGrid = TilesetToolbox.CreateBlankIndexGrid();

            brickFaceWallGrid.roomTypeRestriction = -1;
            brickFaceWallGrid.topLeftIndices = new TileIndexList { indices = new List<int> { 25 }, indexWeights = new List<float> { 1f } };
            brickFaceWallGrid.topIndices = new TileIndexList { indices = new List<int> { 26, 27 }, indexWeights = new List<float> { 1f, 0.005f } };
            brickFaceWallGrid.topRightIndices = new TileIndexList { indices = new List<int> { 28 }, indexWeights = new List<float> { 1f } };
            brickFaceWallGrid.bottomLeftIndices = new TileIndexList { indices = new List<int> { 47 }, indexWeights = new List<float> { 1f } };
            brickFaceWallGrid.bottomIndices = new TileIndexList { indices = new List<int> { 48, 49 }, indexWeights = new List<float> { 1f } };
            brickFaceWallGrid.bottomRightIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };

            beyondBrickMaterial.facewallGrids = new FacewallIndexGridDefinition[]
            {
                new FacewallIndexGridDefinition
                {
                    minWidth = 3,
                    maxWidth = 6,
                    hasIntermediaries = false,
                    minIntermediaryBuffer = 0,
                    maxIntermediaryBuffer = 20,
                    maxIntermediaryLength = 1,
                    minIntermediaryLength = 1,
                    topsMatchBottoms = true,
                    middleSectionSequential = false,
                    canExistInCorners = false,
                    forceEdgesInCorners = false,
                    canAcceptWallDecoration = false,
                    canAcceptFloorDecoration = true,
                    forcedStampMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
                    canBePlacedInExits = false,
                    chanceToPlaceIfPossible = 0.025f,
                    perTileFailureRate = 0.5f,
                    grid = brickFaceWallGrid,
                }
            };
            beyondBrickMaterial.usesFacewallGrids = true;
            beyondBrickMaterial.usesProceduralMaterialTransitions = false;

            beyondBrickMaterial.internalMaterialTransitions = new RoomInternalMaterialTransition[0];
            beyondBrickMaterial.secretRoomWallShardCollections = new List<GameObject>();
            beyondBrickMaterial.overrideStoneFloorType = false;
            beyondBrickMaterial.overrideFloorType = CellVisualData.CellFloorType.Stone;
            beyondBrickMaterial.useLighting = true;
            beyondBrickMaterial.usesDecalLayer = false;
            beyondBrickMaterial.decalIndexGrid = null;
            beyondBrickMaterial.decalLayerStyle = TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS;
            beyondBrickMaterial.decalSize = 1;
            beyondBrickMaterial.decalSpacing = 1;
            beyondBrickMaterial.usesPatternLayer = false;
            beyondBrickMaterial.patternIndexGrid = null;

            beyondBrickMaterial.patternLayerStyle = TilemapDecoSettings.DecoStyle.GROW_FROM_WALLS;
            beyondBrickMaterial.patternSize = 1;
            beyondBrickMaterial.patternSpacing = 1;
            beyondBrickMaterial.forceEdgesDiagonal = false;
            beyondBrickMaterial.exteriorFacadeBorderGrid = null;
            beyondBrickMaterial.facadeTopGrid = null;
            beyondBrickMaterial.bridgeGrid = null;
            */




            /*
            abyssMaterial.patternLayerStyle = TilemapDecoSettings.DecoStyle.PERLIN_NOISE;
            abyssMaterial.patternSize = 1;
            abyssMaterial.patternSpacing = 1;
            abyssMaterial.forceEdgesDiagonal = false;
            abyssMaterial.exteriorFacadeBorderGrid = null;
            abyssMaterial.facadeTopGrid = null;
            abyssMaterial.bridgeGrid = null;
            abyssMaterial.supportsUpholstery = false;

            var floorSquares = TilesetToolbox.CreateBlankIndexGrid();

            floorSquares.topLeftIndices = new TileIndexList { indices = new List<int> { 5 }, indexWeights = new List<float> { 1f } };
            floorSquares.topRightIndices = new TileIndexList { indices = new List<int> { 6 }, indexWeights = new List<float> { 1f } };
            floorSquares.bottomLeftIndices = new TileIndexList { indices = new List<int> { 7 }, indexWeights = new List<float> { 1f } };
            floorSquares.bottomRightIndices = new TileIndexList { indices = new List<int> { 8 }, indexWeights = new List<float> { 1f } };

            var floorSquares1 = TilesetToolbox.CreateBlankIndexGrid();
            floorSquares1.topLeftIndices = new TileIndexList { indices = new List<int> { 9 }, indexWeights = new List<float> { 0.5f } };
            floorSquares1.topRightIndices = new TileIndexList { indices = new List<int> { 10 }, indexWeights = new List<float> { 0.5f } };
            floorSquares1.bottomLeftIndices = new TileIndexList { indices = new List<int> { 11 }, indexWeights = new List<float> { 0.5f } };
            floorSquares1.bottomRightIndices = new TileIndexList { indices = new List<int> {12 }, indexWeights = new List<float> { 0.5f } };

            abyssMaterial.floorSquares = new TileIndexGrid[] { floorSquares, floorSquares1 };
            abyssMaterial.floorSquareDensity = 0.03f;

            abyssMaterial.doPitAO = false;
            abyssMaterial.useLighting = true;
            abyssMaterial.supportsDiagonalWalls = true;

            abyssMaterial.useLighting = false;
            */

            //var brickFaceWallGrid = TilesetToolbox.CreateBlankIndexGrid();
            /*
            //OUTSIDE TILE SET UP
            {
                var ceilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();
                ceilingBorderGrid.topLeftIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topRightIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.leftIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.centerIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.rightIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomLeftIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomRightIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.horizontalIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.verticalIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topCapIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.rightCapIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomCapIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.leftCapIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.allSidesIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topLeftNubIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topRightNubIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderTopNubBothIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderRightNubTopIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderRightNubBottomIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderRightNubBothIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderBottomNubLeftIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderBottomNubRightIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderBottomNubBothIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderLeftNubTopIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderLeftNubBottomIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderLeftNubBothIndices = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.diagonalNubsTopLeftBottomRight = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.diagonalNubsTopRightBottomLeft = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.doubleNubsTop = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.doubleNubsRight = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.doubleNubsBottom = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.doubleNubsLeft = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.quadNubs = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topRightWithNub = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topLeftWithNub = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomRightWithNub = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomLeftWithNub = new TileIndexList { indices = new List<int> { 50 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.CheckerboardDimension = 0;

                abyssMaterial.roomCeilingBorderGrid = ceilingBorderGrid;

            }


            ModPrefabs.AbyssTilesetCollection.spriteDefinitions[33].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0, -1, -1, false, true);


            


            FacewallIndexGridDefinition def = new FacewallIndexGridDefinition
            {
                minWidth = 3,
                maxWidth = 6,
                hasIntermediaries = false,
                minIntermediaryBuffer = 0,
                maxIntermediaryBuffer = 20,
                maxIntermediaryLength = 1,
                minIntermediaryLength = 1,
                topsMatchBottoms = true,
                middleSectionSequential = false,
                canExistInCorners = false,
                forceEdgesInCorners = false,
                canAcceptWallDecoration = false,
                canAcceptFloorDecoration = true,
                forcedStampMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY,
                canBePlacedInExits = false,
                chanceToPlaceIfPossible = 0.025f,
                perTileFailureRate = 0.5f,
                grid = pitBorderGridCave,
            };

            abyssMaterial.facewallGrids = new FacewallIndexGridDefinition[] { def };

            abyssMaterial.usesFacewallGrids = false;
            */




            //FinalScenario_MainMaterial.lightPrefabs.elements[0].rawGameObject = MinesDungeonPrefab.roomMaterialDefinitions[0].lightPrefabs.elements[0].rawGameObject;
            // FinalScenario_MainMaterial.roomFloorBorderGrid = RatDungeonPrefab.roomMaterialDefinitions[0].roomFloorBorderGrid;
            // FinalScenario_MainMaterial.pitLayoutGrid = RatDungeonPrefab.roomMaterialDefinitions[0].pitLayoutGrid;
            // FinalScenario_MainMaterial.pitBorderFlatGrid = RatDungeonPrefab.roomMaterialDefinitions[0].pitBorderFlatGrid;


            /*
            ModPrefabs.AbyssTilesetCollection.spriteDefinitions[75].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0, -1);
            ModPrefabs.AbyssTilesetCollection.spriteDefinitions[82].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0, -1);
            ModPrefabs.AbyssTilesetCollection.spriteDefinitions[83].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0, -1);
            ModPrefabs.AbyssTilesetCollection.spriteDefinitions[84].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0, -1);

            ModPrefabs.AbyssTilesetCollection.spriteDefinitions[85].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0, -1);
            ModPrefabs.AbyssTilesetCollection.spriteDefinitions[92].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0, -1);
            ModPrefabs.AbyssTilesetCollection.spriteDefinitions[93].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0, -1);
            ModPrefabs.AbyssTilesetCollection.spriteDefinitions[94].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 1, 0, -1);
            */

            //DungeonMaterial FinalScenario_MainMaterial = UnityEngine.Object.Instantiate(MarinePastPrefab.roomMaterialDefinitions[0]);//UnityEngine.Object.Instantiate(RatDungeonPrefab.roomMaterialDefinitions[0]);
            DungeonMaterial abyssMaterial = ScriptableObject.CreateInstance<DungeonMaterial>();
            abyssMaterial.supportsPits = true;
            abyssMaterial.doPitAO = false;
            abyssMaterial.useLighting = true;
            abyssMaterial.supportsDiagonalWalls = false;
            abyssMaterial.carpetIsMainFloor = false;
            abyssMaterial.carpetGrids = new TileIndexGrid[0];
            abyssMaterial.roomCeilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();
            abyssMaterial.additionalPitBorderFlatGrid = TilesetToolbox.CreateBlankIndexGrid();
            abyssMaterial.roomCeilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();
            abyssMaterial.wallShards = RatDungeonPrefab.roomMaterialDefinitions[0].wallShards;

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
                //ceilingBorderGrid.borderRightNubTopIndices = new TileIndexList { indices = new List<int> { 81 }, indexWeights = new List<float> { 1f } };
                
                ceilingBorderGrid.borderRightNubBottomIndices = new TileIndexList { indices = new List<int> { 82 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderRightNubBothIndices = new TileIndexList { indices = new List<int> { 83 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderBottomNubLeftIndices = new TileIndexList { indices = new List<int> { 84 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderBottomNubRightIndices = new TileIndexList { indices = new List<int> { 85 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderBottomNubBothIndices = new TileIndexList { indices = new List<int> { 86 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderLeftNubTopIndices = new TileIndexList { indices = new List<int> { 87 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderLeftNubBottomIndices = new TileIndexList { indices = new List<int> { 88 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderLeftNubBothIndices = new TileIndexList { indices = new List<int> { 89 }, indexWeights = new List<float> { 1f } };
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
            //==============================================================================================================================================================================================
            //==============================================================================================================================================================================================
            //==============================================================================================================================================================================================

            DungeonTileStampData m_FloorNameStampData = ScriptableObject.CreateInstance<DungeonTileStampData>();
            m_FloorNameStampData.name = "ENV_Abyss_STAMP_DATA";
            m_FloorNameStampData.tileStampWeight = 0;
            m_FloorNameStampData.spriteStampWeight = 0;
            m_FloorNameStampData.objectStampWeight = 0;
            m_FloorNameStampData.stamps = new TileStampData[0];
            m_FloorNameStampData.spriteStamps = new SpriteStampData[0];
            m_FloorNameStampData.objectStamps = new ObjectStampData[0];//RatDungeonPrefab.stampData.objectStamps;
            m_FloorNameStampData.SymmetricFrameChance = 0.25f;
            m_FloorNameStampData.SymmetricCompleteChance = 0.6f;
            dungeon.gameObject.name = "Base_Abyss";
            dungeon.contentSource = ContentSource.CONTENT_UPDATE_03;
            dungeon.DungeonSeed = 0;
            dungeon.DungeonFloorName = "The Abyss."; // what shows up At the top when floor is loaded
            dungeon.DungeonShortName = "Abyss."; // no clue lol, just make it the same
            dungeon.DungeonFloorLevelTextOverride = "Gungeons Forgotten."; // what shows up below the floorname
            dungeon.LevelOverrideType = GameManager.LevelOverrideState.NONE;
            dungeon.debugSettings = new DebugDungeonSettings()
            {
                RAPID_DEBUG_DUNGEON_ITERATION_SEEKER = false,
                RAPID_DEBUG_DUNGEON_ITERATION = false,
                RAPID_DEBUG_DUNGEON_COUNT = 50,
                GENERATION_VIEWER_MODE = false,
                FULL_MINIMAP_VISIBILITY = false,
                COOP_TEST = false,
                DISABLE_ENEMIES = false,
                DISABLE_LOOPS = false,
                DISABLE_SECRET_ROOM_COVERS = false,
                DISABLE_OUTLINES = false,
                WALLS_ARE_PITS = false
            };
            dungeon.ForceRegenerationOfCharacters = false;
            dungeon.ActuallyGenerateTilemap = true;

            WeightedInt weightedInt = new WeightedInt();
            weightedInt.value = 1;
            weightedInt.weight = 1;
            weightedInt.additionalPrerequisites = new DungeonPrerequisite[0];
            weightedInt.annotation = "why";
            WeightedIntCollection intCollection = new WeightedIntCollection();
            intCollection.elements = new WeightedInt[] { weightedInt };
            dungeon.decoSettings.standardRoomVisualSubtypes = intCollection;
            var deco = dungeon.decoSettings;
            deco.ambientLightColor = new Color(0, 0.3f, 0.9f);
            deco.generateLights = false;


            if (gofuckyourself == null)
            {
                gofuckyourself = RatDungeonPrefab.tileIndices.dungeonCollection;
            }



            dungeon.tileIndices = new TileIndices()
            {
                tilesetId = (GlobalDungeonData.ValidTilesets)CustomValidTilesetsClass.CustomValidTilesets.PLANETSIDEGEON, //sets it to our floors CustomValidTileset

                //since the tileset im using here is a copy of the Rat dungeon tileset, the first variable in ReplaceDungeonCollection is RatDungeonPrefab.tileIndices.dungeonCollection,
                //otherwise we will use a different dungeon prefab
                dungeonCollection = ModPrefabs.AbyssTilesetCollection,//DungeonGenToolbox.ReplaceDungeonCollection(gofuckyourself),
                dungeonCollectionSupportsDiagonalWalls = false,
                aoTileIndices = RatDungeonPrefab.tileIndices.aoTileIndices,
                placeBorders = true,
                placePits = true,

                chestHighWallIndices = new List<TileIndexVariant>() {
                    new TileIndexVariant() {
                        index = 41,
                        likelihood = 0.5f,
                        overrideLayerIndex = 0,
                        overrideIndex = 0
                    }
                },
                decalIndexGrid = null,
                patternIndexGrid = TilesetToolbox.CreateBlankIndexGrid(),//pitBorderGridCave,//RatDungeonPrefab.tileIndices.patternIndexGrid,
                globalSecondBorderTiles = new List<int>(99),
                edgeDecorationTiles = null,
                
                
            };
            dungeon.tileIndices.dungeonCollection.name = "ENV_AbyssFloor_Collection";
            dungeon.roomMaterialDefinitions = new DungeonMaterial[] {
                abyssMaterial,
                abyssMaterial,


               // FinalScenario_MainMaterial
            };
            dungeon.dungeonWingDefinitions = new DungeonWingDefinition[0];

            //This section can be used to take parts from other floors and use them as our own.
            //we can make the running dust from one floor our own, the tables from another our own, 
            //we can use all of the stuff from the same floor, or if you want, you can make your own.
            dungeon.pathGridDefinitions = new List<TileIndexGrid>() { MinesDungeonPrefab.pathGridDefinitions[0] };



            dungeon.dungeonDustups = new DustUpVFX()
            {
                runDustup = MinesDungeonPrefab.dungeonDustups.runDustup,
                waterDustup = MinesDungeonPrefab.dungeonDustups.waterDustup,
                additionalWaterDustup = MinesDungeonPrefab.dungeonDustups.additionalWaterDustup,
                rollNorthDustup = MinesDungeonPrefab.dungeonDustups.rollNorthDustup,
                rollNorthEastDustup = MinesDungeonPrefab.dungeonDustups.rollNorthEastDustup,
                rollEastDustup = MinesDungeonPrefab.dungeonDustups.rollEastDustup,
                rollSouthEastDustup = MinesDungeonPrefab.dungeonDustups.rollSouthEastDustup,
                rollSouthDustup = MinesDungeonPrefab.dungeonDustups.rollSouthDustup,
                rollSouthWestDustup = MinesDungeonPrefab.dungeonDustups.rollSouthWestDustup,
                rollWestDustup = MinesDungeonPrefab.dungeonDustups.rollWestDustup,
                rollNorthWestDustup = MinesDungeonPrefab.dungeonDustups.rollNorthWestDustup,
                rollLandDustup = MinesDungeonPrefab.dungeonDustups.rollLandDustup
            };
            dungeon.PatternSettings = new SemioticDungeonGenSettings()
            {
                flows = new List<DungeonFlow>()
                {
                    AbyssFlows.FlowIStoleFromTheBeyond(),
                },
                mandatoryExtraRooms = new List<ExtraIncludedRoomData>(0),
                optionalExtraRooms = new List<ExtraIncludedRoomData>(0),
                MAX_GENERATION_ATTEMPTS = 250,
                DEBUG_RENDER_CANVASES_SEPARATELY = false
            };

            dungeon.damageTypeEffectMatrix = MinesDungeonPrefab.damageTypeEffectMatrix;
            dungeon.stampData = m_FloorNameStampData;
            dungeon.UsesCustomFloorIdea = false;
            dungeon.FloorIdea = new RobotDaveIdea()
            {
                ValidEasyEnemyPlaceables = new DungeonPlaceable[0],
                ValidHardEnemyPlaceables = new DungeonPlaceable[0],
                UseWallSawblades = false,
                UseRollingLogsVertical = true,
                UseRollingLogsHorizontal = true,
                UseFloorPitTraps = false,
                UseFloorFlameTraps = true,
                UseFloorSpikeTraps = true,
                UseFloorConveyorBelts = true,
                UseCaveIns = true,
                UseAlarmMushrooms = false,
                UseChandeliers = true,
                UseMineCarts = false,
                CanIncludePits = false
            };

            //more variable we can copy from other floors, or make our own
            dungeon.PlaceDoors = true;
            dungeon.doorObjects = CatacombsPrefab.doorObjects;
            dungeon.oneWayDoorObjects = MinesDungeonPrefab.oneWayDoorObjects;
            dungeon.oneWayDoorPressurePlate = MinesDungeonPrefab.oneWayDoorPressurePlate;
            dungeon.phantomBlockerDoorObjects = MinesDungeonPrefab.phantomBlockerDoorObjects;
            dungeon.UsesWallWarpWingDoors = false;
            dungeon.baseChestContents = CatacombsPrefab.baseChestContents;
            dungeon.SecretRoomSimpleTriggersFacewall = new List<GameObject>() { CatacombsPrefab.SecretRoomSimpleTriggersFacewall[0] };
            dungeon.SecretRoomSimpleTriggersSidewall = new List<GameObject>() { CatacombsPrefab.SecretRoomSimpleTriggersSidewall[0] };
            dungeon.SecretRoomComplexTriggers = new List<ComplexSecretRoomTrigger>(0);
            dungeon.SecretRoomDoorSparkVFX = CatacombsPrefab.SecretRoomDoorSparkVFX;
            dungeon.SecretRoomHorizontalPoofVFX = CatacombsPrefab.SecretRoomHorizontalPoofVFX;
            dungeon.SecretRoomVerticalPoofVFX = CatacombsPrefab.SecretRoomVerticalPoofVFX;
            dungeon.sharedSettingsPrefab = CatacombsPrefab.sharedSettingsPrefab;
            dungeon.NormalRatGUID = string.Empty;
            dungeon.BossMasteryTokenItemId = CatacombsPrefab.BossMasteryTokenItemId;
            dungeon.UsesOverrideTertiaryBossSets = false;
            dungeon.OverrideTertiaryRewardSets = new List<TertiaryBossRewardSet>(0);
            dungeon.defaultPlayerPrefab = MinesDungeonPrefab.defaultPlayerPrefab;
            dungeon.StripPlayerOnArrival = false;
            dungeon.SuppressEmergencyCrates = false;
            dungeon.SetTutorialFlag = false;
            dungeon.PlayerIsLight = true;
            dungeon.PlayerLightColor = CatacombsPrefab.PlayerLightColor;
            dungeon.PlayerLightIntensity = 4;
            dungeon.PlayerLightRadius = 4;
            dungeon.PrefabsToAutoSpawn = new GameObject[0];

            //include this for custom floor audio
            //dungeon.musicEventName = "play_sound"; 


            CatacombsPrefab = null;
            RatDungeonPrefab = null;
            MinesDungeonPrefab = null;
            MarinePastPrefab = null;
            return dungeon;
        }
        public static Dungeon GetOrLoadByName_Orig(string name)
        {
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("dungeons/" + name.ToLower());
            DebugTime.RecordStartTime();
            Dungeon component = assetBundle.LoadAsset<GameObject>(name).GetComponent<Dungeon>();
            DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[]
            {
                name
            });
            return component;
        }
    }

    public class FlowHelpers
    {
        public static void RemoveNodeConnectParentToChildren(DungeonFlowNode node)
        {
            string parentNodeGuid = node.parentNodeGuid;
            List<string> list = new List<string>(node.childNodeGuids);
            DungeonFlow flow = node.flow;
            node.flow.DeleteNode(node, false);
            foreach (string text in list)
            {
                bool flag = !string.IsNullOrEmpty(parentNodeGuid) && !string.IsNullOrEmpty(text);
                if (flag)
                {
                    DungeonFlowNode nodeFromGuid = flow.GetNodeFromGuid(parentNodeGuid);
                    DungeonFlowNode nodeFromGuid2 = flow.GetNodeFromGuid(text);
                    bool flag2 = nodeFromGuid != null && nodeFromGuid2 != null;
                    if (flag2) { flow.ConnectNodes(nodeFromGuid, nodeFromGuid2); }
                }
            }
        }

        public static DungeonFlow DuplicateDungeonFlow(DungeonFlow flow)
        {
            DungeonFlow dungeonFlow = ScriptableObject.CreateInstance<DungeonFlow>();
            dungeonFlow.name = flow.name;
            dungeonFlow.fallbackRoomTable = flow.fallbackRoomTable;
            dungeonFlow.phantomRoomTable = flow.phantomRoomTable;
            dungeonFlow.subtypeRestrictions = flow.subtypeRestrictions;
            dungeonFlow.evolvedRoomTable = flow.evolvedRoomTable;
            PlanetsideReflectionHelper.ReflectSetField(typeof(DungeonFlow), "m_firstNodeGuid", PlanetsideReflectionHelper.ReflectGetField<string>(typeof(DungeonFlow), "m_firstNodeGuid", flow), dungeonFlow);
            dungeonFlow.flowInjectionData = flow.flowInjectionData;
            dungeonFlow.sharedInjectionData = flow.sharedInjectionData;
            PlanetsideReflectionHelper.ReflectSetField(typeof(DungeonFlow), "m_nodeGuids", new List<string>(PlanetsideReflectionHelper.ReflectGetField<List<string>>(typeof(DungeonFlow), "m_nodeGuids", flow)), dungeonFlow);
            List<DungeonFlowNode> list = new List<DungeonFlowNode>();
            PlanetsideReflectionHelper.ReflectSetField(typeof(DungeonFlow), "m_nodes", list, dungeonFlow);
            foreach (DungeonFlowNode node in flow.AllNodes)
            {
                DungeonFlowNode dungeonFlowNode = DuplicateDungeonFlowNode(node);
                dungeonFlowNode.flow = dungeonFlow;
                list.Add(dungeonFlowNode);
            }
            return dungeonFlow;
        }

        public static DungeonFlowNode DuplicateDungeonFlowNode(DungeonFlowNode node)
        {
            DungeonFlowNode dungeonFlowNode = new DungeonFlowNode(node.flow);
            dungeonFlowNode.isSubchainStandin = node.isSubchainStandin;
            dungeonFlowNode.nodeType = node.nodeType;
            dungeonFlowNode.roomCategory = node.roomCategory;
            dungeonFlowNode.percentChance = node.percentChance;
            dungeonFlowNode.priority = node.priority;
            dungeonFlowNode.overrideExactRoom = node.overrideExactRoom;
            dungeonFlowNode.overrideRoomTable = node.overrideRoomTable;
            dungeonFlowNode.capSubchain = node.capSubchain;
            dungeonFlowNode.subchainIdentifier = node.subchainIdentifier;
            dungeonFlowNode.limitedCopiesOfSubchain = node.limitedCopiesOfSubchain;
            dungeonFlowNode.maxCopiesOfSubchain = node.maxCopiesOfSubchain;
            dungeonFlowNode.subchainIdentifiers = new List<string>(node.subchainIdentifiers);
            dungeonFlowNode.receivesCaps = node.receivesCaps;
            dungeonFlowNode.isWarpWingEntrance = node.isWarpWingEntrance;
            dungeonFlowNode.handlesOwnWarping = node.handlesOwnWarping;
            dungeonFlowNode.forcedDoorType = node.forcedDoorType;
            dungeonFlowNode.loopForcedDoorType = node.loopForcedDoorType;
            dungeonFlowNode.nodeExpands = node.nodeExpands;
            dungeonFlowNode.initialChainPrototype = node.initialChainPrototype;
            dungeonFlowNode.chainRules = new List<ChainRule>(node.chainRules.Count);
            foreach (ChainRule chainRule in node.chainRules)
            {
                dungeonFlowNode.chainRules.Add(new ChainRule { form = chainRule.form, target = chainRule.target, weight = chainRule.weight, mandatory = chainRule.mandatory });
            }
            dungeonFlowNode.minChainLength = node.minChainLength;
            dungeonFlowNode.maxChainLength = node.maxChainLength;
            dungeonFlowNode.minChildrenToBuild = node.minChildrenToBuild;
            dungeonFlowNode.maxChildrenToBuild = node.maxChildrenToBuild;
            dungeonFlowNode.canBuildDuplicateChildren = node.canBuildDuplicateChildren;
            dungeonFlowNode.parentNodeGuid = node.parentNodeGuid;
            dungeonFlowNode.childNodeGuids = new List<string>(node.childNodeGuids);
            dungeonFlowNode.loopTargetNodeGuid = node.loopTargetNodeGuid;
            dungeonFlowNode.loopTargetIsOneWay = node.loopTargetIsOneWay;
            dungeonFlowNode.guidAsString = node.guidAsString;
            dungeonFlowNode.flow = node.flow;
            return dungeonFlowNode;
        }

        /*public static void PrintBossManager() {
			Console.WriteLine(string.Format("{0}.PrintBossManager(): Currently loaded BossManager data below (incomplete output).", typeof(FlowHelpers)));
			Console.WriteLine(string.Format("Prior floor selected boss room: {0}", BossManager.PriorFloorSelectedBossRoom));
			foreach (BossFloorEntry bossFloorEntry in GameManager.Instance.BossManager.BossFloorData) {
				Console.WriteLine(string.Format("BossFloorEntry: {0} '{1}' '{2}'", bossFloorEntry.AssociatedTilesets, bossFloorEntry.Annotation, bossFloorEntry.ToString()));
				foreach (IndividualBossFloorEntry individualBossFloorEntry in bossFloorEntry.Bosses) {
					Console.WriteLine(string.Format(" Individual: '{0}' ({1}) (#prerequisites={2})", individualBossFloorEntry.ToString(), individualBossFloorEntry.BossWeight, individualBossFloorEntry.GlobalBossPrerequisites.Length));
					foreach (WeightedRoom weightedRoom in individualBossFloorEntry.TargetRoomTable.GetCompiledList()) {
						Console.WriteLine(string.Format("  Room: '{0}' (#prerequisites={1})", weightedRoom.room, weightedRoom.additionalPrerequisites.Length));
					}
				}
			}
		}

		public static void PrintFlow(DungeonFlow flow) {
			bool flag = flow == null;
			if (!flag) {
				try {
					Console.WriteLine(string.Format("DungeonFlow: '{0}' '{1}'", flow, flow.name));
					bool flag2 = flow.flowInjectionData != null;
					if (flag2) {
						foreach (ProceduralFlowModifierData arg in flow.flowInjectionData) {
							Console.WriteLine(string.Format(" ProceduralFlowModifierData: {0}", arg));
						}
					}
					bool flag3 = flow.sharedInjectionData != null;
					if (flag3) {
						foreach (SharedInjectionData arg2 in flow.sharedInjectionData) {
							Console.WriteLine(string.Format(" SharedInjectionData: {0}", arg2));
						}
					}
					bool flag4 = flow.phantomRoomTable != null;
					if (flag4) {
						foreach (WeightedRoom weightedRoom in flow.phantomRoomTable.GetCompiledList()) {
							string format = "  PhantomRoom: '{0}' '{1}'";
							object arg3 = weightedRoom;
							PrototypeDungeonRoom room = weightedRoom.room;
							Console.WriteLine(string.Format(format, arg3, (room != null) ? room.name : null));
						}
					}
					bool flag5 = flow.fallbackRoomTable != null;
					if (flag5) {
						foreach (WeightedRoom weightedRoom2 in flow.fallbackRoomTable.GetCompiledList()) {
							string format2 = "  FallbackRoom: '{0}' '{1}'";
							object arg4 = weightedRoom2;
							PrototypeDungeonRoom room2 = weightedRoom2.room;
							Console.WriteLine(string.Format(format2, arg4, (room2 != null) ? room2.name : null));
						}
					}
					foreach (DungeonFlowNode dungeonFlowNode in flow.AllNodes) {
						Console.WriteLine(string.Format(" Flow Node: {0} {1} (iswarpwingentrance={2}) ({3}) (globalboss={4}) (roomcategory={5}) (override={6})", new object[] {
							dungeonFlowNode.priority,
							dungeonFlowNode.guidAsString,
							dungeonFlowNode.isWarpWingEntrance,
							dungeonFlowNode.handlesOwnWarping,
							dungeonFlowNode.UsesGlobalBossData,
							dungeonFlowNode.roomCategory,
							dungeonFlowNode.overrideExactRoom
						}));
						bool flag6 = dungeonFlowNode.overrideRoomTable != null;
						if (flag6) {
							foreach (WeightedRoom weightedRoom3 in dungeonFlowNode.overrideRoomTable.GetCompiledList()) {
								string str = "  Possible Room Table: ";
								PrototypeDungeonRoom room3 = weightedRoom3.room;
								Console.WriteLine(str + ((room3 != null) ? room3.name : null));
							}
						}
						Console.WriteLine("  Parent: " + dungeonFlowNode.parentNodeGuid);
						foreach (string str2 in dungeonFlowNode.childNodeGuids) {
							Console.WriteLine("  Child: " + str2);
						}
						Console.WriteLine(string.Format("  Loop: {0} {1} {2}", dungeonFlowNode.loopForcedDoorType, dungeonFlowNode.loopTargetIsOneWay, dungeonFlowNode.loopTargetNodeGuid));
						Console.WriteLine(string.Format("  Subchain Identifiers: '{0}' #{1} ('{2}')", dungeonFlowNode.subchainIdentifier, dungeonFlowNode.subchainIdentifiers.Count, string.Join("','", dungeonFlowNode.subchainIdentifiers.ToArray())));
						Console.WriteLine(string.Format("  Door Types: {0} {1}", dungeonFlowNode.forcedDoorType, dungeonFlowNode.loopForcedDoorType));
						foreach (ChainRule chainRule in dungeonFlowNode.chainRules) {
							Console.WriteLine(string.Format("  Chain Rule: {0} '{1}' '{2}' {3}", new object[] {
								chainRule.mandatory,
								chainRule.form,
								chainRule.target,
								chainRule.weight
							}));
						}
					}
				} catch (Exception ex) {
					Console.WriteLine("Exception caught and will not cause errors. Just fix the printing code instead!");
					Console.WriteLine(ex.ToString());
				}
			}
		}*/
    }
    public class AbyssDungeonFlows
    {
        public static DungeonFlow LoadCustomFlow(Func<string, DungeonFlow> orig, string target)
        {
            string flowName = target;
            if (flowName.Contains("/")) { flowName = target.Substring(target.LastIndexOf("/") + 1); }
            if (KnownFlows != null && KnownFlows.Count > 0)
            {
                foreach (DungeonFlow flow in KnownFlows)
                {
                    if (flow.name != null && flow.name != string.Empty)
                    {
                        if (flowName.ToLower() == flow.name.ToLower())
                        {
                            DebugTime.RecordStartTime();
                            DebugTime.Log("AssetBundle.LoadAsset<DungeonFlow>({0})", new object[] { flowName });
                            return flow;
                        }
                    }
                }
            }
            return orig(target);
        }

        public static DungeonFlow LoadOfficialFlow(string target)
        {
            string flowName = target;
            if (flowName.Contains("/")) { flowName = target.Substring(target.LastIndexOf("/") + 1); }
            AssetBundle m_assetBundle_orig = ResourceManager.LoadAssetBundle("flows_base_001");
            DebugTime.RecordStartTime();
            DungeonFlow result = m_assetBundle_orig.LoadAsset<DungeonFlow>(flowName);
            DebugTime.Log("AssetBundle.LoadAsset<DungeonFlow>({0})", new object[] { flowName });
            if (result == null)
            {
                Debug.Log("ERROR: Requested DungeonFlow not found!\nCheck that you provided correct DungeonFlow name and that it actually exists!");
                m_assetBundle_orig = null;
                return null;
            }
            else
            {
                m_assetBundle_orig = null;
                return result;
            }
        }

        public static List<DungeonFlow> KnownFlows;

        public static DungeonFlow Foyer_Flow;

        // Default stuff to use with custom Flows
        public static SharedInjectionData BaseSharedInjectionData;
        public static SharedInjectionData GungeonInjectionData;
        public static SharedInjectionData SewersInjectionData;
        public static SharedInjectionData HollowsInjectionData;
        public static SharedInjectionData CastleInjectionData;

        public static ProceduralFlowModifierData SecretFloorNameEntranceInjector;

        public static DungeonFlowSubtypeRestriction BaseSubTypeRestrictions = new DungeonFlowSubtypeRestriction()
        {
            baseCategoryRestriction = PrototypeDungeonRoom.RoomCategory.NORMAL,
            normalSubcategoryRestriction = PrototypeDungeonRoom.RoomNormalSubCategory.TRAP,
            bossSubcategoryRestriction = PrototypeDungeonRoom.RoomBossSubCategory.FLOOR_BOSS,
            specialSubcategoryRestriction = PrototypeDungeonRoom.RoomSpecialSubCategory.UNSPECIFIED_SPECIAL,
            secretSubcategoryRestriction = PrototypeDungeonRoom.RoomSecretSubCategory.UNSPECIFIED_SECRET,
            maximumRoomsOfSubtype = 1
        };

        // Custom Room Table for Keep Shared Injection Data 
        public static GenericRoomTable m_KeepEntranceRooms;

        // Generate a DungeonFlowNode with a default configuration
        public static DungeonFlowNode GenerateDefaultNode(DungeonFlow targetflow, PrototypeDungeonRoom.RoomCategory roomType, PrototypeDungeonRoom overrideRoom = null, GenericRoomTable overrideTable = null, bool oneWayLoopTarget = false, bool isWarpWingNode = false, string nodeGUID = null, DungeonFlowNode.NodePriority priority = DungeonFlowNode.NodePriority.MANDATORY, float percentChance = 1, bool handlesOwnWarping = true)
        {
            try
            {
                if (string.IsNullOrEmpty(nodeGUID)) { nodeGUID = Guid.NewGuid().ToString(); }

                DungeonFlowNode m_CachedNode = new DungeonFlowNode(targetflow)
                {
                    isSubchainStandin = false,
                    nodeType = DungeonFlowNode.ControlNodeType.ROOM,
                    roomCategory = roomType,
                    percentChance = percentChance,
                    priority = priority,
                    overrideExactRoom = overrideRoom,
                    overrideRoomTable = overrideTable,
                    capSubchain = false,
                    subchainIdentifier = string.Empty,
                    limitedCopiesOfSubchain = false,
                    maxCopiesOfSubchain = 1,
                    subchainIdentifiers = new List<string>(0),
                    receivesCaps = false,
                    isWarpWingEntrance = isWarpWingNode,
                    handlesOwnWarping = handlesOwnWarping,
                    forcedDoorType = DungeonFlowNode.ForcedDoorType.NONE,
                    loopForcedDoorType = DungeonFlowNode.ForcedDoorType.NONE,
                    nodeExpands = false,
                    initialChainPrototype = "n",
                    chainRules = new List<ChainRule>(0),
                    minChainLength = 3,
                    maxChainLength = 8,
                    minChildrenToBuild = 1,
                    maxChildrenToBuild = 1,
                    canBuildDuplicateChildren = false,
                    guidAsString = nodeGUID,
                    parentNodeGuid = string.Empty,
                    childNodeGuids = new List<string>(0),
                    loopTargetNodeGuid = string.Empty,
                    loopTargetIsOneWay = oneWayLoopTarget,
                    flow = targetflow
                };



                return m_CachedNode;
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
                return null;
            }
        }


        // Retrieve sharedInjectionData from a specific floor if one is available
        public static List<SharedInjectionData> RetrieveSharedInjectionDataListFromCurrentFloor()
        {
            Dungeon dungeon = GameManager.Instance.CurrentlyGeneratingDungeonPrefab;

            if (dungeon == null)
            {
                dungeon = GameManager.Instance.Dungeon;
                if (dungeon == null) { return new List<SharedInjectionData>(0); }

            }

            if (dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON |
                dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.WESTGEON |
                dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FINALGEON |
                dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON |
                dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON |
                dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.RATGEON)
            {
                return new List<SharedInjectionData>(0);
            }

            List<SharedInjectionData> m_CachedInjectionDataList = new List<SharedInjectionData>(0);

            if (dungeon.PatternSettings != null && dungeon.PatternSettings.flows != null && dungeon.PatternSettings.flows.Count > 0)
            {
                if (dungeon.PatternSettings.flows[0].sharedInjectionData != null && dungeon.PatternSettings.flows[0].sharedInjectionData.Count > 0)
                {
                    m_CachedInjectionDataList = dungeon.PatternSettings.flows[0].sharedInjectionData;
                }
            }

            return m_CachedInjectionDataList;
        }

        public static ProceduralFlowModifierData RickRollSecretRoomInjector;

        public static SharedInjectionData CustomSecretFloorSharedInjectionData;


        // Initialize KnownFlows array with custom + official flows.
        public static void InitDungeonFlows(bool refreshFlows = false)
        {

            Dungeon TutorialPrefab = DungeonDatabase.GetOrLoadByName("Base_Tutorial");
            Dungeon CastlePrefab = DungeonDatabase.GetOrLoadByName("Base_Castle");
            Dungeon SewerPrefab = DungeonDatabase.GetOrLoadByName("Base_Sewer");
            Dungeon GungeonPrefab = DungeonDatabase.GetOrLoadByName("Base_Gungeon");
            Dungeon CathedralPrefab = DungeonDatabase.GetOrLoadByName("Base_Cathedral");
            Dungeon MinesPrefab = DungeonDatabase.GetOrLoadByName("Base_Mines");
            Dungeon ResourcefulRatPrefab = DungeonDatabase.GetOrLoadByName("Base_ResourcefulRat");
            Dungeon CatacombsPrefab = DungeonDatabase.GetOrLoadByName("Base_Catacombs");
            Dungeon NakatomiPrefab = DungeonDatabase.GetOrLoadByName("Base_Nakatomi");
            Dungeon ForgePrefab = DungeonDatabase.GetOrLoadByName("Base_Forge");
            Dungeon BulletHellPrefab = DungeonDatabase.GetOrLoadByName("Base_BulletHell");

            BaseSharedInjectionData = Planetside.ModPrefabs.shared_auto_002.LoadAsset<SharedInjectionData>("Base Shared Injection Data");
            GungeonInjectionData = GungeonPrefab.PatternSettings.flows[0].sharedInjectionData[1];
            SewersInjectionData = SewerPrefab.PatternSettings.flows[0].sharedInjectionData[1];
            HollowsInjectionData = CatacombsPrefab.PatternSettings.flows[0].sharedInjectionData[1];
            CastleInjectionData = CastlePrefab.PatternSettings.flows[0].sharedInjectionData[0];


            m_KeepEntranceRooms = ScriptableObject.CreateInstance<GenericRoomTable>();
            m_KeepEntranceRooms.includedRoomTables = new List<GenericRoomTable>(0);
            m_KeepEntranceRooms.includedRooms = new WeightedRoomCollection()
            {
                elements = new List<WeightedRoom>()
                {
                    //we will place the entrance to our floor here.
                }
            };


            SecretFloorNameEntranceInjector = new ProceduralFlowModifierData()
            {
                annotation = "Secret Floor Entrance Room",
                DEBUG_FORCE_SPAWN = false,
                OncePerRun = false,
                placementRules = new List<ProceduralFlowModifierData.FlowModifierPlacementType>() {
                    ProceduralFlowModifierData.FlowModifierPlacementType.RANDOM_NODE_CHILD
                },
                roomTable = m_KeepEntranceRooms,
                // exactRoom = SewersInjectionData.InjectionData[0].exactRoom,
                exactRoom = null,
                RequiresMasteryToken = false,
                chanceToLock = 0,
                selectionWeight = 1,
                chanceToSpawn = 1,
                RequiredValidPlaceable = null,
                prerequisites = new DungeonPrerequisite[0],
                CanBeForcedSecret = true,
                RandomNodeChildMinDistanceFromEntrance = 0,
                exactSecondaryRoom = null,
                framedCombatNodes = 0,
            };

            CastleInjectionData.InjectionData.Add(SecretFloorNameEntranceInjector);

            // Don't build/add flows until injection data is created!
            Foyer_Flow = Planetside.FlowHelpers.DuplicateDungeonFlow(Planetside.ModPrefabs.shared_auto_002.LoadAsset<DungeonFlow>("Foyer Flow"));

            // List<DungeonFlow> m_knownFlows = new List<DungeonFlow>();
            KnownFlows = new List<DungeonFlow>();

            //we will add our custom flow here soon.
            KnownFlows.Add(AbyssFlows.DefaultAbyssFlow());


            // Fix issues with nodes so that things other then MainMenu can load Foyer flow
            Foyer_Flow.name = "Foyer_Flow";
            Foyer_Flow.AllNodes[1].handlesOwnWarping = true;
            Foyer_Flow.AllNodes[2].handlesOwnWarping = true;
            Foyer_Flow.AllNodes[3].handlesOwnWarping = true;

            KnownFlows.Add(Foyer_Flow);

            // Add official flows to list (flows found in Dungeon asset bundles after AG&D)
            for (int i = 0; i < TutorialPrefab.PatternSettings.flows.Count; i++)
            {
                KnownFlows.Add(Planetside.FlowHelpers.DuplicateDungeonFlow(TutorialPrefab.PatternSettings.flows[i]));
            }
            for (int i = 0; i < CastlePrefab.PatternSettings.flows.Count; i++)
            {
                KnownFlows.Add(Planetside.FlowHelpers.DuplicateDungeonFlow(CastlePrefab.PatternSettings.flows[i]));
            }
            for (int i = 0; i < SewerPrefab.PatternSettings.flows.Count; i++)
            {
                KnownFlows.Add(Planetside.FlowHelpers.DuplicateDungeonFlow(SewerPrefab.PatternSettings.flows[i]));
            }
            for (int i = 0; i < GungeonPrefab.PatternSettings.flows.Count; i++)
            {
                KnownFlows.Add(Planetside.FlowHelpers.DuplicateDungeonFlow(GungeonPrefab.PatternSettings.flows[i]));
            }
            for (int i = 0; i < CathedralPrefab.PatternSettings.flows.Count; i++)
            {
                KnownFlows.Add(Planetside.FlowHelpers.DuplicateDungeonFlow(CathedralPrefab.PatternSettings.flows[i]));
            }
            for (int i = 0; i < MinesPrefab.PatternSettings.flows.Count; i++)
            {
                KnownFlows.Add(Planetside.FlowHelpers.DuplicateDungeonFlow(MinesPrefab.PatternSettings.flows[i]));
            }
            for (int i = 0; i < ResourcefulRatPrefab.PatternSettings.flows.Count; i++)
            {
                KnownFlows.Add(Planetside.FlowHelpers.DuplicateDungeonFlow(ResourcefulRatPrefab.PatternSettings.flows[i]));
            }
            for (int i = 0; i < CatacombsPrefab.PatternSettings.flows.Count; i++)
            {
                KnownFlows.Add(Planetside.FlowHelpers.DuplicateDungeonFlow(CatacombsPrefab.PatternSettings.flows[i]));
            }
            for (int i = 0; i < NakatomiPrefab.PatternSettings.flows.Count; i++)
            {
                KnownFlows.Add(Planetside.FlowHelpers.DuplicateDungeonFlow(NakatomiPrefab.PatternSettings.flows[i]));
            }
            for (int i = 0; i < ForgePrefab.PatternSettings.flows.Count; i++)
            {
                KnownFlows.Add(Planetside.FlowHelpers.DuplicateDungeonFlow(ForgePrefab.PatternSettings.flows[i]));
            }
            for (int i = 0; i < BulletHellPrefab.PatternSettings.flows.Count; i++)
            {
                KnownFlows.Add(Planetside.FlowHelpers.DuplicateDungeonFlow(BulletHellPrefab.PatternSettings.flows[i]));
            }


            TutorialPrefab = null;
            CastlePrefab = null;
            SewerPrefab = null;
            GungeonPrefab = null;
            CathedralPrefab = null;
            MinesPrefab = null;
            ResourcefulRatPrefab = null;
            CatacombsPrefab = null;
            NakatomiPrefab = null;
            ForgePrefab = null;
            BulletHellPrefab = null;
        }
    }



}




