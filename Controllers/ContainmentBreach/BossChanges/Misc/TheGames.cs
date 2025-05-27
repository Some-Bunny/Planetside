using Dungeonator;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Planetside.Controllers.ContainmentBreach.BossChanges.Misc
{
    public class TheGames : MonoBehaviour
    {
        public class Marker : MonoBehaviour { }
        public void Start()
        {
            DungeonHooks.OnPostDungeonGeneration += this.ResetFloorSpecificData;
            new Hook(typeof(MinorBreakable).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic), typeof(TheGames).GetMethod("CoinChance"));
            GameManager.Instance.OnNewLevelFullyLoaded += this.DoEventChecks;
        }
        void DoEventChecks()
        {
            if (SomethingWickedEventManager.currentSWState == SomethingWickedEventManager.States.ALLOWED || SomethingWickedEventManager.currentSWState == SomethingWickedEventManager.States.ENABLED) { return; }
            if (ContainmentBreachController.CurrentState == ContainmentBreachController.States.ALLOWED || ContainmentBreachController.CurrentState == ContainmentBreachController.States.ENABLED) { return; }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON) 
            {
                if (Amount < 6) { return; }
                if (UnityEngine.Random.value < 0.0025f)
                {
                    Minimap.Instance.PreventAllTeleports = true;
                    Minimap.Instance.TemporarilyPreventMinimap = true;
                    NevernamedsDarknessHandler.EnableDarkness(0, 0);
                    TrapDefusalKit.AddTrapDefuseOverride("somethingwicked_trollface");
                    this.StartCoroutine(DoCheck());
                }
            }
        }


        public IEnumerator DoCheck()
        {
            while (GameManager.Instance.PrimaryPlayer.CurrentRoom == null)
            {
                AkSoundEngine.PostEvent("Stop_MUS_All", base.gameObject);
                yield return null;
            }
            while (GameManager.Instance.PrimaryPlayer.CurrentRoom == GameManager.Instance.Dungeon.data.Entrance)
            {
                AkSoundEngine.PostEvent("Stop_MUS_All", base.gameObject);
                yield return null;
            }
            Minimap.Instance.PreventAllTeleports = false;
            Minimap.Instance.TemporarilyPreventMinimap = false;
            TrapDefusalKit.RemoveTrapDefuseOverride("somethingwicked_trollface");
            NevernamedsDarknessHandler.DisableDarkness(0);
            ResetMusic(GameManager.Instance.Dungeon);
            yield break;
        }
        public void ResetMusic(Dungeon d)
        {
            bool flag = !string.IsNullOrEmpty(d.musicEventName);
            if (flag)
            {
                this.m_cachedMusicEventCore = d.musicEventName;
            }
            else
            {
                this.m_cachedMusicEventCore = "Play_MUS_Dungeon_Theme_01";
            }
            AkSoundEngine.PostEvent(m_cachedMusicEventCore, GameManager.Instance.gameObject);

        }
        private string m_cachedMusicEventCore;

        private void ResetFloorSpecificData()
        {
            Amount = SaveAPI.AdvancedGameStatsManager.Instance.GetPlayerStatValue(CustomTrackedStats.PORTALS_SKIPPED);
            
        }
        public static void CoinChance(Action<MinorBreakable> orig, MinorBreakable self)
        {
            orig(self);

            for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
            {
                PlayerController player = GameManager.Instance.AllPlayers[i];
                if (player != null)
                {
                    if (self.transform?.parent?.gameObject?.GetComponent<MirrorController>() == null && self.transform?.parent?.gameObject?.GetComponent<KickableObject>() == null)
                    {
                        if (self.GetComponent<MoneyPots.MoneyPotBehavior>() == null && self.GetComponent<Marker>() == null && UnityEngine.Random.value < Amount / 5000)
                        {
                            Vector2 position = self.transform.position;
                            DungeonPlaceable bom = ScriptableObject.CreateInstance<DungeonPlaceable>();
                            StaticReferences.StoredDungeonPlaceables.TryGetValue("tresPassPots", out bom);
                            var obj = bom.InstantiateObject(position.GetAbsoluteRoom(), position.ToIntVector2() - position.GetAbsoluteRoom().area.basePosition);
                            Destroy(self.gameObject);
                        }
                    }
                }
            }
        }
        public static float Amount = 0;
    }
}
