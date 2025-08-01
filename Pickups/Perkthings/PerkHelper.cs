using Planetside.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tk2dRuntime.TileMap;

namespace Planetside
{
    public static class PerkHelper
    {
        public enum PerkPickupStatus
        {
            FIRST,
            STACK
        }

        public static int GetPerkStacksFromPlayer(this PlayerController player, int PerkID)
        {
            foreach (var pickup in player.passiveItems)
            {
                if (pickup is PerkPickupObject perk && pickup.PickupObjectId == PerkID && perk.isDummy == false)
                {
                    return perk.CurrentStack;
                }
            }
            return 0;
        }

        public static PerkPickupObject HasPerk(this PlayerController player, int PerkID)
        {
            foreach (var pickup in player.passiveItems)
            {
                if (pickup is PerkPickupObject perk && pickup.PickupObjectId == PerkID && perk.isDummy == false)
                {
                    return perk;
                }
            }
            return null;
        }


        public static int GetGlobalStacksFromAllPlayers(int PerkID)
        {
            int amount = 0;
            foreach (var player in GameManager.Instance.AllPlayers)
            {

                foreach (var pickup in player.passiveItems)
                {
                    if (pickup is PerkPickupObject perk && pickup.PickupObjectId == PerkID && perk.isDummy == false)
                    {
                        amount += perk.CurrentStack;
                    }
                }
            }
            return amount;
        }


        public static PerkPickupStatus AddPerkToPlayer(this PerkPickupObject perk, PlayerController playerController)
        {
            if (ConsumableStorage.PlayersWithConsumables[playerController].GetConsumableOfName(perk.itemName) == 0)
            {
                ConsumableStorage.PlayersWithConsumables[playerController].AddNewConsumable(perk.itemName, 1);
                return PerkPickupStatus.FIRST;
            }
            ConsumableStorage.PlayersWithConsumables[playerController].AddConsumableAmount(perk.itemName, 1);
            return PerkPickupStatus.STACK;

            /*
            var t = perk.PerkMonobehavior;
            var m = playerController.GetComponent(t);
            if (m != null)
            {
                //(m as PerkBaseController).IncreaseStack();
                return PerkPickupStatus.STACK;
            }
            m = playerController.gameObject.AddComponent(t);
            if (m != null)
            {
                //(m as PerkBaseController).Init(playerController, perk.PickupObjectId);
                return PerkPickupStatus.STACK;
            }
            return PerkPickupStatus.FIRST;
            */
        }

        

    }
}
