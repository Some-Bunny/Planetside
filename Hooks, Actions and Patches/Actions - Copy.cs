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

			new Hook(typeof(RoomHandler).GetMethod("TriggerReinforcementLayersOnEvent", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("TriggerReinforcementLayersOnEventHook"));

			Hook ifThisBreaksBlameTheGnomes = new Hook(typeof(Dungeon).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic),  typeof(Actions).GetMethod("StartHook", BindingFlags.Static | BindingFlags.Public));
		}

		public static bool TriggerReinforcementLayersOnEventHook(Func<RoomHandler, RoomEventTriggerCondition, bool, bool> orig, RoomHandler self, RoomEventTriggerCondition condition, bool instant = false)
        {
			if (OnReinforcementWaveTriggered != null)
            {
				OnReinforcementWaveTriggered(self, condition);
			}
			return orig(self, condition, instant);
		}
		public static Action<RoomHandler, RoomEventTriggerCondition> OnReinforcementWaveTriggered;


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
    

