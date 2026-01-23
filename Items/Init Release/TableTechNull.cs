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
using System.Security.Cryptography.X509Certificates;
using Alexandria.Misc;
using UnityEngine.UI;
using Planetside.Controllers;
using Planetside.Toolboxes;

namespace Planetside
{
    public class TableTechNullReferenceException : PassiveItem
    {
        public static void Init()
        {
            bool troll = FoolMode.isFoolish | UnityEngine.Random.value < 0.05f;

            string itemName = troll ? "Table Tech Acrid" : "Table Tech Ignition";

            //tabletechacrid

            GameObject obj = new GameObject(itemName);

            TableTechNullReferenceException minigunrounds = obj.AddComponent<TableTechNullReferenceException>();

            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName(troll ? "tabletechacrid" : "tabletechnull"), data, obj);

            string shortDesc = troll ? "Acridic Flips" : "Burning Flips";
            string longDesc = troll ? "This ancient technique allows the user to create poison with a table flip.\n\nChapter 17 of the Table Sutra. May your toxic hatred of the flip be passed on to the flipped." : "This ancient technique allows the user to create fire with a table flip.\n\nChapter 17 of the Table Sutra. May your burning passion of the flip be passed on to the flipped.";

            minigunrounds.SetupItem(shortDesc, longDesc, "psog");
            minigunrounds.quality = PickupObject.ItemQuality.C;
            SynergyAPI.SynergyBuilder.AddItemToSynergy(minigunrounds, CustomSynergyType.PAPERWORK);

            if (troll)
            {
                ImprovedSynergySetup.Add("Hidden Tech Dissolution",
                    new List<PickupObject>() { minigunrounds }, new List<PickupObject>() { Items.Irradiated_Lead });

                ImprovedSynergySetup.Add("Trash Talks",
                    new List<PickupObject>() { minigunrounds }, new List<PickupObject>() { Guns.Trash_Cannon });
            }
            else
            {
                List<string> mandatoryConsoleIDs = new List<string>
                {
                    "psog:table_tech_ignition",
                    "hot_lead"
                };
                CustomSynergies.Add("Hidden Tech Incineration", mandatoryConsoleIDs, null, true);
                List<string> mandatoryConsoleIDs_1 = new List<string>
                {
                    "psog:table_tech_ignition",
                    "pitchfork"
                };
                CustomSynergies.Add("Tri-Forked", mandatoryConsoleIDs_1, null, true);
            }




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
            int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle);
            Vector2 direction = Vector2.right;
            switch (table.DirectionFlipped)
            {
                case DungeonData.Direction.NORTH:
                    direction = Vector2.up;
                    break;
                    case DungeonData.Direction.SOUTH:
                    direction = Vector2.down;
                    break;
                        case DungeonData.Direction.WEST:
                    direction = Vector2.left;
                        break;
                        case DungeonData.Direction.EAST:
                    direction = Vector2.right;
                        break;
            }
            var goop = FoolMode.isFoolish ? Alexandria.Misc.GoopUtility.PoisonDef : Alexandria.Misc.GoopUtility.FireDef;

            bool dissolve = Owner.PlayerHasActiveSynergy("Hidden Tech Dissolution");

            var cast = RaycastToolbox.ReturnRaycast(table.specRigidbody.UnitCenter, direction, rayMask, 1000, table.specRigidbody);
            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Owner.PlayerHasActiveSynergy("Hidden Tech Incineration") ? Alexandria.Misc.GoopUtility.GreenFireDef : goop).TimedAddGoopLine(table.sprite.WorldCenter + direction, cast.Contact, dissolve ? 2.5f : 1, 1f);
            
            
            if (Owner.PlayerHasActiveSynergy("Tri-Forked"))
            {
                float g = MathToolbox.ToAngle(direction) + 45;
                float g_1 = MathToolbox.ToAngle(direction) - 45;

                cast = RaycastToolbox.ReturnRaycast(table.specRigidbody.UnitCenter, MathsAndLogicHelper.DegreeToVector2(g), rayMask, 1000, table.specRigidbody);
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Owner.PlayerHasActiveSynergy("Hidden Tech Incineration") ? Alexandria.Misc.GoopUtility.GreenFireDef : goop).TimedAddGoopLine(table.sprite.WorldCenter + direction, cast.Contact, dissolve ? 2.5f : 1, 1f);

                cast = RaycastToolbox.ReturnRaycast(table.specRigidbody.UnitCenter, MathsAndLogicHelper.DegreeToVector2(g_1), rayMask, 1000, table.specRigidbody);
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Owner.PlayerHasActiveSynergy("Hidden Tech Incineration") ? Alexandria.Misc.GoopUtility.GreenFireDef : goop).TimedAddGoopLine(table.sprite.WorldCenter + direction, cast.Contact, dissolve ? 2.5f : 1, 1f);
            }

            if (Owner.PlayerHasActiveSynergy("Trash Talks"))
            {
                foreach (var gun in Owner.inventory.AllGuns)
                {
                    if (gun.PickupObjectId == Guns.Trash_Cannon.PickupObjectId)
                    {
                        gun.ammo++;
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    IntVector2 intVector2FromDirection = DungeonData.GetIntVector2FromDirection(table.DirectionFlipped);
                    GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(Guns.Trash_Cannon.DefaultModule.projectiles[0].gameObject, table.transform.position, Quaternion.Euler(0f, 0f, intVector2FromDirection.ToVector2().ToAngle() + UnityEngine.Random.Range(-25, 25)), true);
                    Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                    if (component != null)
                    {
                        component.baseData.damage *= 0.5f;
                        component.Owner = base.Owner;
                        component.Shooter = base.Owner.specRigidbody;
                        component.specRigidbody.RegisterTemporaryCollisionException(table.specRigidbody, 0.5f);
                    }
                }
            }

        }     
        public override void OnDestroy()
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