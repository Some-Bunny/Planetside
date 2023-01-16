using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using GungeonAPI;

using System.Text;

using Gungeon;
using SaveAPI;

namespace Planetside
{
	public class PrayerAmulet : PassiveItem
	{
		public static void Init()
		{
			string name = "Prayer Amulet";
			//string resourcePath = "Planetside/Resources/prayeramulet.png";
			GameObject gameObject = new GameObject(name);
			PrayerAmulet warVase = gameObject.AddComponent<PrayerAmulet>();
			//ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Vow To The Cult";
			string longDesc = "Adds a shrine room to every floor.\n\nAn amulet similar to that worn by the High Priest, embedded with perfectly cut bullets.";
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("prayeramulet"), data, gameObject);
            ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog");
            warVase.quality = PickupObject.ItemQuality.C;
			PrayerAmulet.PrayerAmuletID = warVase.PickupObjectId;
			ItemIDs.AddToList(warVase.PickupObjectId);

            PrayerKeep = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/PrayerRooms/PrayerRoomKeep.room").room;
            PrayerProper = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/PrayerRooms/PrayerRoomProper.room").room;
            PrayerMines = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/PrayerRooms/PrayerRoomMines.room").room;
            PrayerHollow = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/PrayerRooms/PrayerRoomHollow.room").room;
            PrayerForge = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/PrayerRooms/PrayerRoomForge.room").room;
            PrayerHell = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/PrayerRooms/PrayerRoomHell.room").room;
            PrayerSewer = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/PrayerRooms/PrayerRoomOubliette.room").room;
            PrayerAbbey = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/PrayerRooms/PrayerRoomAbbey.room").room;
            warVase.SetupUnlockOnCustomStat(CustomTrackedStats.UMBRAL_ENEMIES_KILLED, 4, DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN);

            PrayerAmulet.InitRooms();

		}
		public static int PrayerAmuletID;
        public static SharedInjectionData CastleInjectionData;

        public static PrototypeDungeonRoom PrayerKeep;
        public static PrototypeDungeonRoom PrayerProper;
        public static PrototypeDungeonRoom PrayerMines;
        public static PrototypeDungeonRoom PrayerHollow;
        public static PrototypeDungeonRoom PrayerForge;
        public static PrototypeDungeonRoom PrayerHell;
        public static PrototypeDungeonRoom PrayerSewer;
        public static PrototypeDungeonRoom PrayerAbbey;


        public static void InitRooms()
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
                GenerateNewMrocData(PrayerKeep, GlobalDungeonData.ValidTilesets.CASTLEGEON),
                GenerateNewMrocData(PrayerProper, GlobalDungeonData.ValidTilesets.GUNGEON),
                GenerateNewMrocData(PrayerMines, GlobalDungeonData.ValidTilesets.MINEGEON),
                GenerateNewMrocData(PrayerHollow, GlobalDungeonData.ValidTilesets.CATACOMBGEON),
                GenerateNewMrocData(PrayerForge, GlobalDungeonData.ValidTilesets.FORGEGEON),
                GenerateNewMrocData(PrayerHell, GlobalDungeonData.ValidTilesets.HELLGEON),
                GenerateNewMrocData(PrayerSewer, GlobalDungeonData.ValidTilesets.SEWERGEON),
                GenerateNewMrocData(PrayerAbbey, GlobalDungeonData.ValidTilesets.CATHEDRALGEON),
            };
            injector.name = "The Prayer Amulet Shrine Rooms";
            SharedInjectionData BaseInjection = LoadHelper.LoadAssetFromAnywhere<SharedInjectionData>("Base Shared Injection Data");
            if (BaseInjection.AttachedInjectionData == null)
            {
                BaseInjection.AttachedInjectionData = new List<SharedInjectionData>();
            }
            BaseInjection.AttachedInjectionData.Add(injector);
        }

        public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
            PrayerAmulet.IncrementFlag(player, typeof(PrayerAmulet));
        }
        public override DebrisObject Drop(PlayerController player)
		{
            PrayerAmulet.DecrementFlag(player ,typeof(PrayerAmulet));
			DebrisObject result = base.Drop(player);
			return result;
		}
		protected override void OnDestroy()
		{
            if (base.Owner != null)
            {
                PrayerAmulet.DecrementFlag(GameManager.Instance.PrimaryPlayer, typeof(PrayerAmulet));
            }
            base.OnDestroy();
		}

        public static ProceduralFlowModifierData GenerateNewMrocData(PrototypeDungeonRoom RequiredRoom, GlobalDungeonData.ValidTilesets Tileset)
        {
            string name = RequiredRoom.name.ToString();
            if (RequiredRoom.name.ToString() == null)
            {
                name = "EmergencyAnnotationName";
            }
            ProceduralFlowModifierData PrayerRoomMines = new ProceduralFlowModifierData()
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
                       advancedAdvancedPrerequisiteType = DungeonGenToolbox.AdvancedDungeonPrerequisite.AdvancedAdvancedPrerequisiteType.PASSIVE_ITEM_FLAG,
                       requiredPassiveFlag = typeof(PrayerAmulet),
                       requiredTileset = Tileset,
                       requireTileset = true
                    }
                },
                CanBeForcedSecret = false,
                RandomNodeChildMinDistanceFromEntrance = 0,
                exactSecondaryRoom = null,
                framedCombatNodes = 0,
                
            };
            return PrayerRoomMines;
        }

    }
}
