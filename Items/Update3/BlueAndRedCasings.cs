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


namespace Planetside
{
    public class Thing : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Blue Casing";
            //string resourceName = "Planetside/Resources/bluecasing.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Thing>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("bluecasing"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Used For... Something?";
            string longDesc = "An unusually blue casing." +
                "\n\nMaybe something might want it?";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            item.CanBeDropped = false;
            item.RespawnsIfPitfall = true;
            BlueCasingID = item.PickupObjectId;
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
        }
        public static int BlueCasingID;
        public override DebrisObject Drop(PlayerController player)
		{
            player.healthHaver.OnPreDeath -= HealthHaver_OnPreDeath;
            DebrisObject result = base.Drop(player);
            Destroy(result.gameObject, 1);
			return result;
		}
		public override void Pickup(PlayerController player)
		{
            if (!this.m_pickedUpThisRun)
            {
                SomethingWickedEventManager.currentSWState = SomethingWickedEventManager.States.ALLOWED;
                player.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;
            }
            base.Pickup(player);
           
        }

        private void HealthHaver_OnPreDeath(Vector2 obj)
        {
            if (SomethingWickedEventManager.currentSWState == SomethingWickedEventManager.States.ALLOWED)
            {
                SomethingWickedEventManager.currentSWState = SomethingWickedEventManager.States.DISABLED;
            }
            base.Owner.RemovePassiveItem(BlueCasingID);
            SomethingWickedEnemy.PlayersToIgnore.Add(base.Owner);
            base.Owner.healthHaver.OnPreDeath -= HealthHaver_OnPreDeath;
            this.Drop(base.Owner);
        }

        public override void OnDestroy()
		{
            base.OnDestroy();
		}      
        
        
    }
}

namespace Planetside
{
    public class RedThing : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Red Casing";
            string resourceName = "Planetside/Resources/redcasing.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<RedThing>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Used For... Something?";
            string longDesc = "An unusually red casing." +
                "\n\nYou do not feel comfortable holding it.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            item.CanBeDropped = false;
            item.RespawnsIfPitfall = true;
            RedCasingID = item.PickupObjectId;
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
        }
        public static int RedCasingID;
        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.OnPreDeath -= HealthHaver_OnPreDeath;
            DebrisObject result = base.Drop(player);
            return result;
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;
        }
        private void HealthHaver_OnPreDeath(Vector2 obj)
        {
            SomethingWickedEnemy.PlayersToIgnore.Add(base.Owner);
            base.Owner.healthHaver.OnPreDeath -= HealthHaver_OnPreDeath;
            base.Owner.RemovePassiveItem(RedCasingID);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}

