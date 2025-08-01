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
using HarmonyLib;

namespace Planetside
{
    public static class Actions
    {
        public static void Init()
        {
            //new Hook(typeof(PlayerController).GetMethod("HandleSpinfallSpawn", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Actions).GetMethod("HandleSpinfallSpawnHook"));

			new Hook(typeof(RoomHandler).GetMethod("TriggerReinforcementLayersOnEvent", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("TriggerReinforcementLayersOnEventHook"));

			Hook ifThisBreaksBlameTheGnomes = new Hook(typeof(Dungeon).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic),  typeof(Actions).GetMethod("StartHookDungeon", BindingFlags.Static | BindingFlags.Public));

            //new Hook(typeof(GameStatsManager).GetMethod("BeginNewSession", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("BeginNewSessionHook"));

            //new Hook(typeof(MetaInjectionData).GetMethod("PreprocessRun", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("PreprocessRunHook"));

            //new Hook(typeof(PlayerController).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("StartHook"));

            //new Hook(typeof(PlayerController).GetMethod("Awake", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("AwakeHook"));

            //new Hook(typeof(GameStatsManager).GetMethod("BeginNewSession", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("BeginNewSessionHook"));

            //new Hook(typeof(GameStatsManager).GetMethod("SetStat", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("SetStatHook"));

            //new Hook(typeof(GameStatsManager).GetMethod("RegisterStatChange", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("RegisterStatChangeHook"));

            new Hook(typeof(Dungeon).GetMethod("FloorReached", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("FloorReachedHook"));

        }

        public static void FloorReachedHook(Action<Dungeon> orig, Dungeon self)
		{
            orig(self);
            self.StartCoroutine(DelayChecks(self));
		}


        public static IEnumerator DelayChecks(Dungeon self)
        {
            yield return new WaitForSeconds(0.1f);
            var gameManager = GameManager.Instance;
            var gameStatsManager = GameStatsManager.Instance;
            if (gameManager != null && gameStatsManager != null && gameStatsManager.IsInSession == true)
            {
                if (gameStatsManager.GetSessionStatValue(TrackedStats.TIME_PLAYED) < 0.15f)
                {
                    if (OnRunStart != null)
                    {
                        OnRunStart(gameManager.PrimaryPlayer, gameManager.SecondaryPlayer, gameManager.CurrentGameMode);
                    }
                }
            }
            yield break;
        }

        public static Action<PlayerController, PlayerController, GameManager.GameMode> OnRunStart;



        public static void RegisterStatChangeHook(Action<GameStatsManager, TrackedStats, float> orig, GameStatsManager self, TrackedStats stat, float value)
        {
            if (stat == TrackedStats.RUNS_PLAYED_POST_FTA) { ETGModConsole.Log("lets go?"); ETGModConsole.Log(self.GetPlayerStatValue(stat)); }
            orig(self, stat, value);

        }

        public static void SetStatHook(Action<GameStatsManager, TrackedStats, float> orig, GameStatsManager self, TrackedStats stat, float value)
        {
            if (stat == TrackedStats.RUNS_PLAYED_POST_FTA) { ETGModConsole.Log(value); }
            orig(self, stat, value);
           
        }

       


        public static bool TriggerReinforcementLayersOnEventHook(Func<RoomHandler, RoomEventTriggerCondition, bool, bool> orig, RoomHandler self, RoomEventTriggerCondition condition, bool instant = false)
        {
			if (OnReinforcementWaveTriggered != null)
            {
				OnReinforcementWaveTriggered(self, condition);
			}
			return orig(self, condition, instant);
		}



        public static IEnumerator StartHookDungeon(Func<Dungeon, IEnumerator> orig, Dungeon self)
		{
            PreDungeonTrueStart?.Invoke(self);
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

		public static IEnumerator HandleSpinfallSpawnHook(Func<PlayerController, float, IEnumerator> orig, PlayerController self, float invisibleDelay)
		{
			IEnumerator origEnum = orig(self, invisibleDelay);
			while (origEnum.MoveNext())
			{
				object obj = origEnum.Current;
				yield return obj;
			}
			

            yield break;
		}

        #region On GameManager Update
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.Update))]
        public class Patch_GameManager_Update
        {
            [HarmonyPrefix]
            private static void Awake(GameManager __instance)
            {
                OnGameManagerUpdate?.Invoke(__instance);
            }
        }
        #endregion

        public static Action<GameManager> OnGameManagerUpdate;
        public static Action<RoomHandler, RoomEventTriggerCondition> OnReinforcementWaveTriggered;
        public static Action<Dungeon> PostDungeonTrueStart;
        public static Action<Dungeon> PreDungeonTrueStart;

    }
}
    

