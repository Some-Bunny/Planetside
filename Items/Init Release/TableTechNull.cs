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
using System.Collections.ObjectModel;

namespace Planetside
{
    public class TableTechNullReferenceException : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Table Tech Null";

            string resourceName = "Planetside/Resources/tabletechnull.png";

            GameObject obj = new GameObject(itemName);

            TableTechNullReferenceException minigunrounds = obj.AddComponent<TableTechNullReferenceException>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Undefined Flips";
            string longDesc = "This ancient technique allows the user to vaporize enemy bullets from thin air, as if they never existed.\n\nChapter 17 of the Table Sutra. Nolla.";

            minigunrounds.SetupItem(shortDesc, longDesc, "psog");
            minigunrounds.quality = PickupObject.ItemQuality.C;
            TableTechNullReferenceException.TableTechNullID = minigunrounds.PickupObjectId;
            ItemIDs.AddToList(minigunrounds.PickupObjectId);

        }
        public static int TableTechNullID;

        public override void Pickup(PlayerController player)
        {
            player.OnTableFlipped = (Action<FlippableCover>)Delegate.Combine(player.OnTableFlipped, new Action<FlippableCover>(this.HandleFlip));
            base.Pickup(player);
        }
        private void HandleFlip(FlippableCover table)
        {
            PlayerController player = GameManager.Instance.PrimaryPlayer;
            if ((NullShrineRoom.Contains(player.CurrentRoom.GetRoomName()) && FlippedInShrineRoom == false))
            {
                FlippedInShrineRoom = true;
                IntVector2 bestRewardLocation2 = player.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
                Chest rainbow_Chest = GameManager.Instance.RewardManager.S_Chest;
                rainbow_Chest.IsLocked = false;
                Chest chest2 = Chest.Spawn(rainbow_Chest, bestRewardLocation2);
                chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
            }

            ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
            if (allProjectiles != null)
            {
                for (int i = 0; i < allProjectiles.Count; i++)
                {
                    if (allProjectiles[i].Owner != player)
                    {
                        Projectile proj = allProjectiles[i];
                        SpawnManager.Despawn(proj.gameObject);
                    }
                }
            }

            pickupsInRoom.Clear();
            DebrisObject[] shitOnGround = FindObjectsOfType<DebrisObject>();
            foreach (DebrisObject debris in shitOnGround)
            {
                bool isValid = DetermineIfValid(debris);
                if (isValid && debris.transform.position.GetAbsoluteRoom() == player.CurrentRoom)
                {
                    pickupsInRoom.Add(debris);
                }
            }
            if (pickupsInRoom.Count != 0)
            {
                pickupsInRoom.Shuffle();
                DoReroll(pickupsInRoom[0]);
            }
        }
        private void DoReroll(DebrisObject obj)
        {
            Vector2 pos = obj.transform.position;
            LootEngine.SpawnItem(PickupObjectDatabase.GetById(NullPickupInteractable.NollahID).gameObject, pos, Vector2.zero, 1f, false, true, false);
            Destroy(obj.gameObject);
        }
        private bool DetermineIfValid(DebrisObject thing)
        {
            PickupObject itemness = thing.gameObject.GetComponent<PickupObject>();
            if (itemness != null)
            {
                if (itemness.PickupObjectId == 127 || validIDs.Contains(itemness.PickupObjectId))
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }
        private List<int> validIDs = new List<int>()
        {
            78, //Ammo
            600, //Spread Ammo
            565, //Glass Guon Stone
            73, //Half Heart
            85, //Heart
            120, //Armor
            224, //Blank
            67, //Key
        };
        private List<DebrisObject> pickupsInRoom = new List<DebrisObject>();

        //Erase a random enemy from the room
        //Erase random bullets in the room
        //turns a random pickup in the room into a null pickup
        //Erases the table?

        public static List<string> NullShrineRoom = new List<string>()
        {
            "NullShrineRoom.room"
        };
        protected override void OnDestroy()
        {
            if (base.Owner != null)
            {
                base.Owner.OnTableFlipped = (Action<FlippableCover>)Delegate.Remove(base.Owner.OnTableFlipped, new Action<FlippableCover>(this.HandleFlip));
            }
            this.FlippedInShrineRoom = false;
            base.OnDestroy();
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnTableFlipped = (Action<FlippableCover>)Delegate.Remove(player.OnTableFlipped, new Action<FlippableCover>(this.HandleFlip));
            return base.Drop(player);
        }
        public bool FlippedInShrineRoom = false;
    }
}