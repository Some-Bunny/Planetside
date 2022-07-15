using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Dungeonator;
using SaveAPI;
using Gungeon;
using ItemAPI;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using System.Collections;
using Brave.BulletScript;

namespace Planetside
{
    public static class Actions
    {
        public static void Init()
        {
            new Hook(typeof(PlayerController).GetMethod("HandleSpinfallSpawn", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Actions).GetMethod("HandleSpinfallSpawnHook"));

			Hook ifThisBreaksBlameTheGnomes = new Hook(typeof(Dungeon).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic),  typeof(Actions).GetMethod("StartHook", BindingFlags.Static | BindingFlags.Public));
			//PostDungeonTrueStart += PostGen;
		}

		public static void PostGen(Dungeon self)
        {
			var deco = self.decoSettings;
			deco.ambientLightColor = new Color(0, 0.3f, 0.9f);
			deco.generateLights = false;

			self.DungeonFloorName = "big burger";
			self.DungeonShortName = "with extra sauce";


			WeightedInt weightedInt = new WeightedInt();
			weightedInt.value = 1;
			weightedInt.weight = 1;
			weightedInt.additionalPrerequisites = new DungeonPrerequisite[0];
			weightedInt.annotation = "why";
			WeightedIntCollection intCollection = new WeightedIntCollection();
			intCollection.elements = new WeightedInt[] { weightedInt };
			self.decoSettings.standardRoomVisualSubtypes = intCollection;

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
			self.stampData = m_FloorNameStampData;
			self.tileIndices.dungeonCollection = ModPrefabs.AbyssTilesetCollection;
			self.dungeonWingDefinitions = new DungeonWingDefinition[0];

			//This section can be used to take parts from other floors and use them as our own.
			//we can make the running dust from one floor our own, the tables from another our own, 
			//we can use all of the stuff from the same floor, or if you want, you can make your own.
			var MinesDungeonPrefab = DungeonDatabase.GetOrLoadByName("Base_Mines");

			self.pathGridDefinitions = new List<TileIndexGrid>() { MinesDungeonPrefab.pathGridDefinitions[0] };
			self.roomMaterialDefinitions = new DungeonMaterial[] {
				ModPrefabs.abyssMaterial,
				ModPrefabs.abyssMaterial,


               // FinalScenario_MainMaterial
            };
			MinesDungeonPrefab = null;

		}

		public static IEnumerator StartHook(Func<Dungeon, IEnumerator> orig, Dungeon self)
		{
			IEnumerator origEnum = orig(self);
			while (origEnum.MoveNext())
			{
				object obj = origEnum.Current;
				yield return obj;
			}
		
			if (PostDungeonTrueStart != null)
			{
				PostDungeonTrueStart(self);
			}
			yield break;
		}
		public static Action<Dungeon> PostDungeonTrueStart;

		public static IEnumerator HandleSpinfallSpawnHook(Func<PlayerController, float, IEnumerator> orig, PlayerController self, float invisibleDelay)
		{
			IEnumerator origEnum = orig(self, invisibleDelay);
			while (origEnum.MoveNext())
			{
				object obj = origEnum.Current;
				yield return obj;
			}
			if (GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED) <= 0.33f)
			{
				if (OnRunStart != null)
				{
					OnRunStart(self);
				}
			}
			yield break;
		}

	

		public static Action<PlayerController> OnRunStart;
	}
}
    

