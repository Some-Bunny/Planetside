using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;
using GungeonAPI;
using SaveAPI;

namespace Planetside
{
    public class CursesController : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("Starting CursesController setup...");
            try
            {
                IsDark = false;
                PetrifyCurseState = PetrifyCurseStates.DISABLED;
                DarknessCurseState = DarknessCurseStates.DISABLED;
                JamnationCurseState = JamnationCurseStates.DISABLED;
                BolsterCurseState = BolsterCurseStates.DISABLED;

                ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.CurseAIActorChanges));

                new Hook(
                typeof(PlayerController).GetMethod("OnRoomCleared", BindingFlags.Instance | BindingFlags.Public),
                typeof(CursesController).GetMethod("OnRoomClearedHook", BindingFlags.Static | BindingFlags.Public));

                new Hook(
                typeof(PlayerController).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(CursesController).GetMethod("LateUpdateHook", BindingFlags.Static | BindingFlags.Public));

                DungeonHooks.OnPostDungeonGeneration += this.ResetFloorSpecificData;
                Debug.Log("Finished CursesController setup without failure!");

            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish CursesController setup!");
                Debug.Log(e);
            }
        }

        public static void LateUpdateHook(Action<PlayerController> orig, PlayerController self)
        {
            if (self.CurrentRoom != null)
            {
                bool m_isInCombat = PlanetsideReflectionHelper.ReflectGetField<bool>(typeof(PlayerController), "m_isInCombat", self);
                bool isInCombat = m_isInCombat;
                m_isInCombat = self.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
                if (m_isInCombat && !isInCombat && DarknessCurseState != DarknessCurseStates.DISABLED)
                {
                    float ConeAngle = 37.5f;
                    float Speed = 2f;
                    if (DarknessCurseState == DarknessCurseStates.UPGRADED_AND_ONEROOMLEFT) { ConeAngle = 15; Speed = 1.5f; }
                    AkSoundEngine.PostEvent("Play_ENM_darken_world_01", self.gameObject);
                    NevernamedsDarknessHandler.EnableDarkness(ConeAngle, Speed);
                    IsDark = true;
                }
            }
            orig(self);
        }
        static bool IsDark;

        public static void OnRoomClearedHook(Action<PlayerController> orig, PlayerController self)
        {
            orig(self);
            if (IsDark == true)
            {
                IsDark = !IsDark;
                AkSoundEngine.PostEvent("Play_ENM_lighten_world_01", self.gameObject); 
                NevernamedsDarknessHandler.DisableDarkness(1);
            }
            if (self.CurrentRoom != null && UnityEngine.Random.value <= ChestDropChance())
            {
                IntVector2 bestRewardLocation = self.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
                Chest chest2 = GameManager.Instance.RewardManager.SpawnRewardChestAt(bestRewardLocation, -1f, PickupObject.ItemQuality.EXCLUDED);
                chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
            }
            
            if (PetrifyCurseState == PetrifyCurseStates.UPGRADED_AND_ONEROOMLEFT)
            {PetrifyCurseState = PetrifyCurseStates.DISABLED;
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEPETRIFY, true);
                OtherTools.Notify("Curse Of Petrification cleansed", "You are free now.", "Planetside/Resources/ShrineIcons/PurityIcon", UINotificationController.NotificationColor.GOLD);
                SpawnSpecialChest(self);
            }

            if (JamnationCurseState == JamnationCurseStates.UPGRADED_AND_ONEROOMLEFT)
            { JamnationCurseState = JamnationCurseStates.DISABLED;
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEJAM, true);
                OtherTools.Notify("Curse Of Jamnation cleansed", "You are free now.", "Planetside/Resources/ShrineIcons/PurityIcon", UINotificationController.NotificationColor.GOLD);
                SpawnSpecialChest(self);
            }

            if (DarknessCurseState == DarknessCurseStates.UPGRADED_AND_ONEROOMLEFT)
            { DarknessCurseState = DarknessCurseStates.DISABLED;
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEDARKEN, true);
                OtherTools.Notify("Curse Of Darkness cleansed", "You are free now.", "Planetside/Resources/ShrineIcons/PurityIcon", UINotificationController.NotificationColor.GOLD);
                SpawnSpecialChest(self);
            }

            if (BolsterCurseState == BolsterCurseStates.UPGRADED_AND_ONEROOMLEFT)
            { BolsterCurseState = BolsterCurseStates.DISABLED;
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEBOLSTER, true);
                OtherTools.Notify("Curse Of Bolstering cleansed", "You are free now.", "Planetside/Resources/ShrineIcons/PurityIcon", UINotificationController.NotificationColor.GOLD);
                SpawnSpecialChest(self);
            }
            if (CheckIfUnlocked() == true)
            {
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK, true);
            }

        }
        private static void SpawnSpecialChest(PlayerController player)
        {
            IntVector2 bestRewardLocation = player.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.CameraCenter, true);
            Chest chest2 = GameManager.Instance.RewardManager.SpawnRewardChestAt(bestRewardLocation, -1f, PickupObject.ItemQuality.EXCLUDED);
            chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
            chest2.IsLocked = false;
            Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(chest2.sprite);
            if (outlineMaterial1 != null)
            { outlineMaterial1.SetColor("_OverrideColor", new Color(30f, 30f, 30f)); }
        }

        private static bool CheckIfUnlocked()
        {
            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEJAM) == false) { return false; }
            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEBOLSTER) == false) { return false; }
            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEDARKEN) == false) { return false; }
            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEPETRIFY) == false) { return false; }
            return true;
        }

        public static bool CheckIfAnyCurseActive()
        {
            if (PetrifyCurseState == PetrifyCurseStates.ENABLED) { return true; }
            if (JamnationCurseState == JamnationCurseStates.ENABLED) { return true; }
            if (DarknessCurseState == DarknessCurseStates.ENABLED) { return true; }
            if (BolsterCurseState == BolsterCurseStates.ENABLED) { return true; }
            return false;
        }

        public void CurseAIActorChanges(AIActor target)
        {
            if (target != null)
            {
                if (JamnationCurseState != JamnationCurseStates.DISABLED)
                {
                    float Chance = 0.4f;
                    if (JamnationCurseState == JamnationCurseStates.UPGRADED_AND_ONEROOMLEFT) { Chance = 1; }
                    if (target != null && !OtherTools.BossBlackList.Contains(target.aiActor.encounterTrackable.EncounterGuid) && UnityEngine.Random.value <= Chance)
                    {
                        if (!target.IsBlackPhantom)
                        { target.BecomeBlackPhantom(); }
                        else
                        { target.gameObject.GetOrAddComponent<UmbraController>(); }
                    }
                }
                if (PetrifyCurseState != PetrifyCurseStates.DISABLED)
                {
                    if (target != null && !OtherTools.BossBlackList.Contains(target.aiActor.EnemyGuid) && !target.healthHaver.IsBoss)
                    {
                        float Time = PetrifyCurseState == PetrifyCurseStates.UPGRADED_AND_ONEROOMLEFT ? 12f : 7;
                        PetrifyThing petrifyComponent = target.gameObject.AddComponent<PetrifyThing>();
                        petrifyComponent.Time = Time;
                    }
                }
                if (BolsterCurseState != BolsterCurseStates.DISABLED)
                {
                    if (target != null && !OtherTools.BossBlackList.Contains(target.aiActor.encounterTrackable.EncounterGuid))
                    {
                        float CooldownScale = 0.7f;
                        float MovementSpeed = 1.2f;
                        if (BolsterCurseState == BolsterCurseStates.UPGRADED_AND_ONEROOMLEFT) { CooldownScale = 0.33f; MovementSpeed = 1.5f; }
                        if (target.behaviorSpeculator != null) { target.behaviorSpeculator.CooldownScale /= CooldownScale; }
                        target.MovementSpeed *= MovementSpeed;
                    }
                }
            }
           
        }

        public static void EnableDarkness(bool IsSuperPowered = false, string OverrideTextLineOne = "You Obtained The", string OverrideTextLineTwo = "Curse Of Darkness.")
        {
            if (IsSuperPowered == true)
            { DarknessCurseState = DarknessCurseStates.UPGRADED_AND_ONEROOMLEFT; }
            else { DarknessCurseState = DarknessCurseStates.ENABLED; }
            OtherTools.Notify(OverrideTextLineOne, OverrideTextLineTwo, "Planetside/Resources/ShrineIcons/DarknessIcon");
            if (GameManager.Instance != null && GameManager.Instance.BestActivePlayer != null)
            {AkSoundEngine.PostEvent("Play_ENM_darken_world_01", GameManager.Instance.BestActivePlayer.gameObject);}
        }

        public static void EnablePetrification(bool IsSuperPowered = false, string OverrideTextLineOne = "You Obtained The", string OverrideTextLineTwo = "Curse Of Petrification.")
        {
            if (IsSuperPowered == true)
            { PetrifyCurseState = PetrifyCurseStates.UPGRADED_AND_ONEROOMLEFT; }
            else { PetrifyCurseState = PetrifyCurseStates.ENABLED; }
            OtherTools.Notify(OverrideTextLineOne, OverrideTextLineTwo, "Planetside/Resources/ShrineIcons/PetrifyIcon");
            if (GameManager.Instance != null && GameManager.Instance.BestActivePlayer != null)
            { AkSoundEngine.PostEvent("Play_ENM_darken_world_01", GameManager.Instance.BestActivePlayer.gameObject); }
        }
        public static void EnableJamnation(bool IsSuperPowered = false, string OverrideTextLineOne = "You Obtained The", string OverrideTextLineTwo = "Curse Of Jamnation.")
        {
            if (IsSuperPowered == true)
            { JamnationCurseState = JamnationCurseStates.UPGRADED_AND_ONEROOMLEFT; }
            else { JamnationCurseState = JamnationCurseStates.ENABLED; }
            OtherTools.Notify(OverrideTextLineOne, OverrideTextLineTwo, "Planetside/Resources/ShrineIcons/JammedIcon");
            if (GameManager.Instance != null && GameManager.Instance.BestActivePlayer != null)
            { AkSoundEngine.PostEvent("Play_ENM_darken_world_01", GameManager.Instance.BestActivePlayer.gameObject); }
        }
        public static void EnableBolster(bool IsSuperPowered = false, string OverrideTextLineOne = "You Obtained The", string OverrideTextLineTwo = "Curse Of Bolstering.")
        {
            if (IsSuperPowered == true)
            { BolsterCurseState = BolsterCurseStates.UPGRADED_AND_ONEROOMLEFT; }
            else { BolsterCurseState = BolsterCurseStates.ENABLED; }
            OtherTools.Notify(OverrideTextLineOne, OverrideTextLineTwo, "Planetside/Resources/ShrineIcons/BolsterIcon");
            if (GameManager.Instance != null && GameManager.Instance.BestActivePlayer != null)
            { AkSoundEngine.PostEvent("Play_ENM_darken_world_01", GameManager.Instance.BestActivePlayer.gameObject); }
        }

        private static float ChestDropChance()
        {
            float Chance = 0;
            if (BolsterCurseState == BolsterCurseStates.ENABLED)
            { Chance += 0.045f; }
            if (PetrifyCurseState == PetrifyCurseStates.ENABLED)
            { Chance += 0.045f; }
            if (JamnationCurseState == JamnationCurseStates.ENABLED)
            { Chance += 0.045f; }
            if (DarknessCurseState == DarknessCurseStates.ENABLED)
            { Chance += 0.045f; }
            return Chance;
        }

        private void ResetFloorSpecificData()
        {
            PetrifyCurseState = PetrifyCurseStates.DISABLED;
            DarknessCurseState = DarknessCurseStates.DISABLED;
            JamnationCurseState = JamnationCurseStates.DISABLED;
            BolsterCurseState = BolsterCurseStates.DISABLED;
        }

        public static PetrifyCurseStates PetrifyCurseState;
        public static DarknessCurseStates DarknessCurseState;
        public static JamnationCurseStates JamnationCurseState;
        public static BolsterCurseStates BolsterCurseState;

        public enum PetrifyCurseStates
        {
            DISABLED,
            ENABLED,
            UPGRADED_AND_ONEROOMLEFT
        };
        public enum DarknessCurseStates
        {
            DISABLED,
            ENABLED,
            UPGRADED_AND_ONEROOMLEFT
        };
        public enum JamnationCurseStates
        {
            DISABLED,
            ENABLED,
            UPGRADED_AND_ONEROOMLEFT
        };
        public enum BolsterCurseStates
        {
            DISABLED,
            ENABLED,
            UPGRADED_AND_ONEROOMLEFT
        };
    }
}
