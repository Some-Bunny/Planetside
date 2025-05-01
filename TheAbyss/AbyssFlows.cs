﻿using System;
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
    class AbyssFlows : AbyssDungeonFlows
    {

		public static DungeonFlow BuildPrimaryFlow()
		{
			try
			{

				DungeonFlow m_CachedFlow = ScriptableObject.CreateInstance<DungeonFlow>();

				DungeonFlowNode entranceNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.ENTRANCE, ModRoomPrefabs.Mod_Entrance_Room);
				DungeonFlowNode exitNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.EXIT, ModRoomPrefabs.Mod_Exit_Room);

				DungeonFlowNode VoidMuncherNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, ModRoomPrefabs.VoidMuncherRoom);
                DungeonFlowNode Abyss_Loot_Node = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, ModRoomPrefabs.AbyssSpawnLootRoom);
                DungeonFlowNode Abyss_Loot_Node_2 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, ModRoomPrefabs.AbyssSpawnLootRoom);


                //DungeonFlowNode bossfoyerNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.SPECIAL, overrideTable: ModPrefabs.boss_foyertable);
				//DungeonFlowNode bossNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.BOSS, ModRoomPrefabs.Mod_Boss);

				DungeonFlowNode defaultCombatNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);
				DungeonFlowNode defaultCombatNode1 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);
				DungeonFlowNode defaultCombatNode2 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);

				DungeonFlowNode defaultCombatNodeNonMain1_1 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);
				DungeonFlowNode defaultCombatNodeNonMain1_2 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);

				DungeonFlowNode defaultCombatNode_Branch_1 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);
				DungeonFlowNode defaultCombatNode_Branch_2 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);
				DungeonFlowNode defaultCombatNode_Branch_3 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);

                DungeonFlowNode defaultCombatNodeNonMain2_1 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);
                DungeonFlowNode defaultCombatNodeNonMain2_2 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);

                DungeonFlowNode defaultCombatNodeNonMain3_1 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);


                DungeonFlowNode defaultCombatNode_Branch2_1 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);
                DungeonFlowNode defaultHubNode3 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.HUB, overrideTable: ModPrefabs.FloorNameRoomTable);
                DungeonFlowNode defaultCombatNode_Branch2_2 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);
                DungeonFlowNode defaultCombatNode_Branch2_3 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, overrideTable: ModPrefabs.FloorNameRoomTable);

                DungeonFlowNode defaultHubNode1 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.HUB, overrideTable: ModPrefabs.FloorNameRoomTable);
				DungeonFlowNode defaultHubNode2 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.HUB, overrideTable: ModPrefabs.FloorNameRoomTable);



                m_CachedFlow.name = "LoopDeLoop";
				m_CachedFlow.fallbackRoomTable = ModPrefabs.FloorNameRoomTable;
				m_CachedFlow.phantomRoomTable = null;
				m_CachedFlow.phantomRoomTable = null;

				m_CachedFlow.subtypeRestrictions = new List<DungeonFlowSubtypeRestriction>(0);
				m_CachedFlow.flowInjectionData = new List<ProceduralFlowModifierData>(0);
				m_CachedFlow.sharedInjectionData = new List<SharedInjectionData>() { };//{ BaseSharedInjectionData };

				m_CachedFlow.Initialize();

				m_CachedFlow.AddNodeToFlow(entranceNode, null);


				m_CachedFlow.AddNodeToFlow(defaultCombatNode, entranceNode);
				m_CachedFlow.AddNodeToFlow(defaultCombatNode1, defaultCombatNode);
				m_CachedFlow.AddNodeToFlow(defaultCombatNode2, defaultCombatNode1);

				m_CachedFlow.AddNodeToFlow(defaultHubNode1, defaultCombatNode2);
				//Non-branching path
				m_CachedFlow.AddNodeToFlow(defaultCombatNodeNonMain1_1, defaultHubNode1);
				m_CachedFlow.AddNodeToFlow(defaultCombatNodeNonMain1_2, defaultCombatNodeNonMain1_1);
				m_CachedFlow.AddNodeToFlow(VoidMuncherNode, defaultCombatNodeNonMain1_2);
				//

				//defaultHubNode1 starts new branch
				m_CachedFlow.AddNodeToFlow(defaultCombatNode_Branch_1, defaultHubNode1);
				m_CachedFlow.AddNodeToFlow(defaultCombatNode_Branch_2, defaultCombatNode_Branch_1);
				m_CachedFlow.AddNodeToFlow(defaultCombatNode_Branch_3, defaultCombatNode_Branch_2);

				m_CachedFlow.AddNodeToFlow(defaultHubNode2, defaultCombatNode_Branch_3);
                //new shortish branch
                m_CachedFlow.AddNodeToFlow(defaultCombatNodeNonMain2_1, defaultHubNode2);
                m_CachedFlow.AddNodeToFlow(defaultCombatNodeNonMain2_2, defaultCombatNodeNonMain2_1);
                m_CachedFlow.AddNodeToFlow(Abyss_Loot_Node, defaultCombatNodeNonMain2_2);
                //Path off to main path
                m_CachedFlow.AddNodeToFlow(defaultCombatNode_Branch2_1, defaultHubNode2);
                m_CachedFlow.AddNodeToFlow(defaultHubNode3, defaultCombatNode_Branch2_1);
                //1 room detour
                m_CachedFlow.AddNodeToFlow(defaultCombatNodeNonMain3_1, defaultHubNode3);
                m_CachedFlow.AddNodeToFlow(Abyss_Loot_Node_2, defaultCombatNodeNonMain3_1);


                //main branch
                m_CachedFlow.AddNodeToFlow(defaultCombatNode_Branch2_2, defaultHubNode3);
                m_CachedFlow.AddNodeToFlow(defaultCombatNode_Branch2_3, defaultCombatNode_Branch2_2);





                m_CachedFlow.AddNodeToFlow(exitNode, defaultCombatNode_Branch2_3);



				m_CachedFlow.FirstNode = entranceNode;

				return m_CachedFlow;
			}
			catch (Exception e)
			{
				ETGModConsole.Log(e.ToString());
				return null;
			}



		}
        public static DungeonFlow ShitAbyssFlow()
		{
            DungeonFlow m_CachedFlow = ScriptableObject.CreateInstance<DungeonFlow>();
            DungeonFlowNode entranceNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.ENTRANCE, ModRoomPrefabs.Mod_Entrance_Room);
            m_CachedFlow.FirstNode = entranceNode;
            DungeonFlowNode exitNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.EXIT, ModRoomPrefabs.Mod_Exit_Room);
            m_CachedFlow.AddNodeToFlow(entranceNode, null);
            m_CachedFlow.AddNodeToFlow(exitNode, entranceNode);

            return m_CachedFlow;
        }


        public static DungeonFlow DefaultAbyssFlow()
		{
			try
			{

				DungeonFlow m_CachedFlow = ScriptableObject.CreateInstance<DungeonFlow>();

				DungeonFlowNode entranceNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.ENTRANCE, ModRoomPrefabs.Mod_Entrance_Room);
				DungeonFlowNode exitNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.EXIT, ModRoomPrefabs.Mod_Exit_Room);
				//DungeonFlowNode bossNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.BOSS, ModRoomPrefabs.Mod_Boss);

				DungeonFlowNode FloorNameShopNode = GenerateDefaultNode(m_CachedFlow, ModPrefabs.shop02.category, overrideTable: ModPrefabs.shop_room_table);
				DungeonFlowNode FloorNameRewardNode_01 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.CONNECTOR, ModPrefabs.gungeon_rewardroom_1);
				DungeonFlowNode FloorNameRewardNode_02 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.CONNECTOR, ModPrefabs.gungeon_rewardroom_1);


				DungeonFlowNode FloorNameRoomNode_01 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.HUB, oneWayLoopTarget: true);
				DungeonFlowNode FloorNameRoomNode_02 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_04 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_05 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_06 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.HUB, oneWayLoopTarget: true);
				DungeonFlowNode FloorNameRoomNode_07 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_09 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_10 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_11 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.HUB);
				DungeonFlowNode FloorNameRoomNode_12 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_13 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_14 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_16 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_17 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_18 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);

				m_CachedFlow.name = "AbyssFlowOne";
				m_CachedFlow.fallbackRoomTable = ModPrefabs.FloorNameRoomTable;
				m_CachedFlow.phantomRoomTable = null;
				m_CachedFlow.subtypeRestrictions = new List<DungeonFlowSubtypeRestriction>(0);
				m_CachedFlow.flowInjectionData = new List<ProceduralFlowModifierData>(0);
				m_CachedFlow.sharedInjectionData = new List<SharedInjectionData>() { BaseSharedInjectionData };

				m_CachedFlow.Initialize();

				m_CachedFlow.AddNodeToFlow(entranceNode, null);
				// First Looping branch
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_16, entranceNode);
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_01, FloorNameRoomNode_16);
				// Dead End
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_05, FloorNameRoomNode_01);
				// Start of Loop
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_02, FloorNameRoomNode_01);
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_04, FloorNameRoomNode_02);
				m_CachedFlow.AddNodeToFlow(FloorNameRewardNode_01, FloorNameRoomNode_04);
				// Connect End of Loop to first in chain
				m_CachedFlow.LoopConnectNodes(FloorNameRewardNode_01, FloorNameRoomNode_01);

				// Second Looping branch
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_17, entranceNode);
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_06, FloorNameRoomNode_17);
				// Dead End
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_10, FloorNameRoomNode_06);
				// Start of Loop
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_07, FloorNameRoomNode_06);
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_09, FloorNameRoomNode_07);
				m_CachedFlow.AddNodeToFlow(FloorNameRewardNode_02, FloorNameRoomNode_09);
				// Connect End of Loop to first in chain
				m_CachedFlow.LoopConnectNodes(FloorNameRewardNode_02, FloorNameRoomNode_06);

				// Splitting path to Shop or Boss
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_18, entranceNode);
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_11, FloorNameRoomNode_18);
				// Path To Boss
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_12, FloorNameRoomNode_11);
				// Path to Shop
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_13, FloorNameRoomNode_11);
				m_CachedFlow.AddNodeToFlow(FloorNameShopNode, FloorNameRoomNode_13);
				// Dead End
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_14, FloorNameRoomNode_11);


				m_CachedFlow.AddNodeToFlow(exitNode, FloorNameRoomNode_12);


				m_CachedFlow.FirstNode = entranceNode;

				return m_CachedFlow;
			}
			catch (Exception e)
			{
				ETGModConsole.Log(e.ToString());
				return null;
			}
		}
	}
}
