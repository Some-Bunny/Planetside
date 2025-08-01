using Alexandria.DungeonAPI;
using Dungeonator;
using ItemAPI;
using SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetside
{
    class HollowWalls : PerkPickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Hollow Walls";
            GameObject gameObject = new GameObject(name);
            HollowWalls item = gameObject.AddComponent<HollowWalls>();

            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("geomancy"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Enigmancy";
            string longDesc = "After infinite cycles, the walls have learned to hide their own secrets from even the Gungeon itself.\n\nReveal them.";
            item.SetupItem(shortDesc, longDesc, "psog");
            item.encounterTrackable.DoNotificationOnEncounter = false;

            HollowWalls.ItemID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.yellow;
            particles.ParticleSystemColor2 = Color.yellow;
            item.OutlineColor = new Color(0.2f, 0.1f, 0f);

            item.StackPickupNotificationText = "The walls hide more secrets.";
            item.InitialPickupNotificationText = "More rewarding secrets.";


            MyFlowData = GenerateNewMrocData();

            Dungeon hell_ = DungeonDatabase.GetOrLoadByName("base_bullethell");
            var Hell_Injections = hell_.PatternSettings.flows[0].sharedInjectionData[0];


            MyInjectionData = new SharedInjectionData()
            {
                AttachedInjectionData = new List<SharedInjectionData> { },
                UseInvalidWeightAsNoInjection = false,
                ChanceToSpawnOne = 1000,
                IgnoreUnmetPrerequisiteEntries = false,
                InjectionData = new List<ProceduralFlowModifierData>()
                {

                },
                IsNPCCell = false,
                OnlyOne = false,
                PreventInjectionOfFailedPrerequisites = false,
            };

            Hell_Injections.AttachedInjectionData.Add(MyInjectionData);

            hell_ = null;

            AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");
            var BaseSharedInjection = sharedAssets2.LoadAsset<SharedInjectionData>("Base Shared Injection Data");
            BaseSharedInjection.AttachedInjectionData.Add(MyInjectionData);
        }

        private static SharedInjectionData MyInjectionData;
        private static ProceduralFlowModifierData MyFlowData;

        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 0,
                    LockedString = AlphabetController.ConvertString("Secrets"),
                    UnlockedString = "\"The walls hide more secrets.\"",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 1,
                    LockedString = AlphabetController.ConvertString("Secrets"),
                    UnlockedString = "Adds an extra secret room to every floor.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 3,
                    LockedString = AlphabetController.ConvertString("Excavation"),
                    UnlockedString = "Secret Rooms hold extra loot.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 3,
                    LockedString = AlphabetController.ConvertString("More Loot"),
                    UnlockedString = "Stacking adds more secret rooms.",
                    requiresFlag = true,
                    FlagToTrack = CustomDungeonFlags.HOLLOWWALLS_FLAG_STACK
                },
        };

        public override CustomDungeonFlags FlagToSetOnStack => CustomDungeonFlags.HOLLOWWALLS_FLAG_STACK;

        public static int ItemID;


        public static ProceduralFlowModifierData GenerateNewMrocData()
        {
            ProceduralFlowModifierData PrayerRoomMines = new ProceduralFlowModifierData()
            {
                annotation = "GIVE ME SECRETS",
                DEBUG_FORCE_SPAWN = false,
                OncePerRun = false,
                placementRules = new List<ProceduralFlowModifierData.FlowModifierPlacementType>()
                {
                    ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN,
                    ProceduralFlowModifierData.FlowModifierPlacementType.RANDOM_NODE_CHILD,
                },
                roomTable = GungeonAPI.StaticReferences.RoomTables["secret"],
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
                       advancedAdvancedPrerequisiteType = DungeonGenToolbox.AdvancedDungeonPrerequisite.AdvancedAdvancedPrerequisiteType.PASSIVE_ITEM_FLAG,
                       requiredPassiveFlag = typeof(HollowWalls),
                       requireTileset = false
                    }
                    
                },
                CanBeForcedSecret = true,
                RandomNodeChildMinDistanceFromEntrance = -1,
                exactSecondaryRoom = null,
                framedCombatNodes = 2,
            };
            return PrayerRoomMines;
        }


        public override void OnInitialPickup(PlayerController playerController)
        {
            MyInjectionData.InjectionData = new List<ProceduralFlowModifierData>();
            PrayerAmulet.IncrementFlag(playerController, typeof(HollowWalls));
            AddLoot();
            GameManager.Instance.OnNewLevelFullyLoaded += this.AddLoot;
        }

        public override void OnDestroy()
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= this.AddLoot;
            base.OnDestroy();
        }

        public override void OnStack(PlayerController player)
        {
            if (MyInjectionData.InjectionData.Count < 5)
            {
                MyInjectionData.InjectionData.Add(MyFlowData);
            }
            else
            {
                AmountOfPickups++;
            }

        }

        private void AddLoot()
        {
            for (int i = 0; i < GameManager.Instance.Dungeon.data.rooms.Count; i++)
            {
                RoomHandler roomHandler = GameManager.Instance.Dungeon.data.rooms[i];
                if (roomHandler.connectedRooms.Count != 0)
                {
                    if (roomHandler.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET&& roomHandler.RevealedOnMap == false)
                    {
                        for (int q = 0; q < AmountOfPickups; q++)
                        {
                            var pickup = LootEngine.SpawnItem(GameManager.Instance.RewardManager.CurrentRewardData.SingleItemRewardTable.SelectByWeight(false), roomHandler.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.Original).ToCenterVector3(0), Vector2.zero, 0);
                            pickup.GetComponent<PickupObject>().IgnoredByRat = true;
     
                            if (!StaticReferenceManager.AllDebris.Contains(pickup))
                            {
                                StaticReferenceManager.AllDebris.Add(pickup);
                            }
                        }

                    }         
                }
            }
        }
        private int AmountOfPickups = 2;
    }
}
