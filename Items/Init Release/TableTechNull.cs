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

namespace Planetside
{
    public class TableTechNullReferenceException : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Table Tech Ignition";


            GameObject obj = new GameObject(itemName);

            TableTechNullReferenceException minigunrounds = obj.AddComponent<TableTechNullReferenceException>();

            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("tabletechnull"), data, obj);

            string shortDesc = "Burning Flips";
            string longDesc = "This ancient technique allows the user to create fire with a table flip.\n\nChapter 17 of the Table Sutra. May your burning passion of the flip be passed on to the flipped.";

            minigunrounds.SetupItem(shortDesc, longDesc, "psog");
            minigunrounds.quality = PickupObject.ItemQuality.C;
            SynergyAPI.SynergyBuilder.AddItemToSynergy(minigunrounds, CustomSynergyType.PAPERWORK);
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
            var cast = RaycastToolbox.ReturnRaycast(table.specRigidbody.UnitCenter, direction, rayMask, 1000, table.specRigidbody);
            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Owner.PlayerHasActiveSynergy("Hidden Tech Incineration") ? Alexandria.Misc.GoopUtility.GreenFireDef : Alexandria.Misc.GoopUtility.FireDef).TimedAddGoopLine(table.sprite.WorldCenter + direction, cast.Contact, 1, 1f);
            if (Owner.PlayerHasActiveSynergy("Tri-Forked"))
            {
                float g = MathToolbox.ToAngle(direction) + 45;
                float g_1 = MathToolbox.ToAngle(direction) - 45;

                cast = RaycastToolbox.ReturnRaycast(table.specRigidbody.UnitCenter, MathsAndLogicHelper.DegreeToVector2(g), rayMask, 1000, table.specRigidbody);
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Owner.PlayerHasActiveSynergy("Hidden Tech Incineration") ? Alexandria.Misc.GoopUtility.GreenFireDef : Alexandria.Misc.GoopUtility.FireDef).TimedAddGoopLine(table.sprite.WorldCenter + direction, cast.Contact, 1, 1f);

                cast = RaycastToolbox.ReturnRaycast(table.specRigidbody.UnitCenter, MathsAndLogicHelper.DegreeToVector2(g_1), rayMask, 1000, table.specRigidbody);
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Owner.PlayerHasActiveSynergy("Hidden Tech Incineration") ? Alexandria.Misc.GoopUtility.GreenFireDef : Alexandria.Misc.GoopUtility.FireDef).TimedAddGoopLine(table.sprite.WorldCenter + direction, cast.Contact, 1, 1f);
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