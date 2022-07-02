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
using static EnemyBulletBuilder.BulletBuilderFakePrefabHooks;



namespace Planetside
{
    public class ContainmentBreachController : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("Starting ContainmentBreachController setup...");
            try
            {
                CurrentState = States.ENABLED;
                DungeonHooks.OnPostDungeonGeneration += this.ResetFloorSpecificData;
                new Hook(typeof(RewardManager).GetMethod("GenerationSpawnRewardChestAt", BindingFlags.Public | BindingFlags.Instance), typeof(ContainmentBreachController).GetMethod("ReplaceChestWithContainers", BindingFlags.Public | BindingFlags.Static));
                GameManager.Instance.OnNewLevelFullyLoaded += Instance_OnNewLevelFullyLoaded;
                ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.InfectionChanges));

                Debug.Log("Finished ContainmentBreachController setup without failure!");
            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish ContainmentBreachController setup!");
                Debug.Log(e);
            }
        }

        private void Instance_OnNewLevelFullyLoaded()
        {
            InfectionReplacement.InitSpecialMods();
        }

        public void InfectionChanges(AIActor target)
        {
            if (CurrentState == States.ENABLED)
            {
                if (EnemyIsValid(target) == true)
                {
                    if (target.healthHaver.IsBoss || target.healthHaver.IsSubboss)
                    {
                        target.ApplyEffect(DebuffLibrary.InfectedBossEffect);

                    }
                    else
                    {
                        target.ApplyEffect(DebuffLibrary.InfectedEnemyEffect);

                    }

                }
            }
        }

        private static bool EnemyIsValid(AIActor aI)
        {
            if (aI == null) { return false; }
            if (aI.IgnoreForRoomClear == true) { return false; }
            if (aI.gameObject.GetComponent<MirrorImageController>() != null) { return false; }
            if (aI.gameObject.GetComponent<DisplacedImageController>() != null) { return false; }
            if (aI.CompanionOwner != null) { return false; }
            if (aI.gameObject.GetComponent<CompanionController>() != null) { return false; }
            if (StaticInformation.ModderBulletGUIDs.Contains(aI.EnemyGuid)) { return false; }
            if (aI.EnemyGuid == "3f11bbbc439c4086a180eb0fb9990cb4") { return false; }
            if (EliteBlackListDefault.Contains(aI.EnemyGuid)) { return false; }
            if (aI.gameObject.GetComponent<ForgottenEnemyComponent>() != null) { return false; }
            return true;
        }

        private static List<string> EliteBlackListDefault = new List<string>()
        {
            "deturretleft_enemy",
            "deturret_enemy",
            "fodder_enemy",
            EnemyGuidDatabase.Entries["mine_flayers_bell"],
            EnemyGuidDatabase.Entries["gunreaper"],
            EnemyGuidDatabase.Entries["key_bullet_kin"],
            EnemyGuidDatabase.Entries["chance_bullet_kin"],
            EnemyGuidDatabase.Entries["grip_master"],
            EnemyGuidDatabase.Entries["mine_flayers_claymore"],

            EnemyGuidDatabase.Entries["chicken"],
            EnemyGuidDatabase.Entries["snake"],
            EnemyGuidDatabase.Entries["poopulons_corn"],
            EnemyGuidDatabase.Entries["rat"],
            EnemyGuidDatabase.Entries["rat_candle"],
            EnemyGuidDatabase.Entries["dragun_egg_slimeguy"],
        };


        public static Chest ReplaceChestWithContainers(Func<RewardManager, IntVector2, RoomHandler, PickupObject.ItemQuality?, float, Chest> orig, RewardManager self, IntVector2 positionInRoom, RoomHandler targetRoom, PickupObject.ItemQuality? targetQuality, float overrideMimicChance)
        {
            if (CurrentState == States.ALLOWED)
            {
                GameObject g = new GameObject();
                StaticReferences.StoredRoomObjects.TryGetValue("trespassContainer", out g);
                g.GetComponent<DungeonPlaceableBehaviour>().InstantiateObject(targetRoom, positionInRoom + new IntVector2(0, -1), true);
                targetRoom.RegisterInteractable(g.GetInterfaceInChildren<IPlayerInteractable>());
                return null;
            }
            else
            {
                return orig(self, positionInRoom, targetRoom, targetQuality, overrideMimicChance);
            }
        }

        public static States CurrentState;
        public enum States
        {
            ALLOWED,
            ENABLED,
            DISABLED
        };
        private void ResetFloorSpecificData()
        {
            if (CurrentState == States.ALLOWED)
            {
                CurrentState = States.ENABLED;

            }

            if (CurrentState == States.ENABLED)
            {
                //CurrentState = States.DISABLED;
            }
        }
    }
}
