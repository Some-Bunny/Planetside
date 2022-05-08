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
                CurrentState = States.DISABLED;
                DungeonHooks.OnPostDungeonGeneration += this.ResetFloorSpecificData;
                new Hook(typeof(RewardManager).GetMethod("GenerationSpawnRewardChestAt", BindingFlags.Public | BindingFlags.Instance), typeof(ContainmentBreachController).GetMethod("ReplaceChestWithMirror", BindingFlags.Public | BindingFlags.Static));
                Debug.Log("Finished ContainmentBreachController setup without failure!");
            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish ContainmentBreachController setup!");
                Debug.Log(e);
            }
        }

        public static Chest ReplaceChestWithMirror(Func<RewardManager, IntVector2, RoomHandler, PickupObject.ItemQuality?, float, Chest> orig, RewardManager self, IntVector2 positionInRoom, RoomHandler targetRoom, PickupObject.ItemQuality? targetQuality, float overrideMimicChance)
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
          
        }
    }
}
