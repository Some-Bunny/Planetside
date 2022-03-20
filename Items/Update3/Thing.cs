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
            string resourceName = "Planetside/Resources/bluecasing.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Thing>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Used For... Something?";
            string longDesc = "An unusually blue casing." +
                "\n\nMaybe something might want it?";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            item.CanBeDropped = false;
            BlueCasingID = item.PickupObjectId;

        }
        public static int BlueCasingID;


        public override DebrisObject Drop(PlayerController player)
		{
        //    GungeonAPI.DungeonHooks.OnPostDungeonGeneration -= this.PlaceColoredShrines;
            //GameManager.Instance.OnNewLevelFullyLoaded -= this.EnableSW;
            DebrisObject result = base.Drop(player);
			return result;
		}
		public override void Pickup(PlayerController player)
		{
            SomethingWickedEventManager.currentSWState = SomethingWickedEventManager.States.ALLOWED;
            //SomethingWickedEventManager.ShouldDoSomethingWickedEvent = false;
            //GameManager.Instance.OnNewLevelFullyLoaded += this.EnableSW;
            base.Pickup(player);
		}
        private void EnableSW()
        {
            //GameManager.Instance.OnNewLevelFullyLoaded -= this.EnableSW;
            //TheMeagaSomethingWickedEnabler dark = base.Owner.gameObject.AddComponent<TheMeagaSomethingWickedEnabler>();
            //dark.player = base.Owner;
        }
        private void DisableSW()
        {
            /*
            GameManager.Instance.OnNewLevelFullyLoaded -= this.DisableSW;
            if (base.Owner.gameObject.GetComponent<TheMeagaSomethingWickedEnabler>() && base.Owner != null)
            {
                TheMeagaSomethingWickedEnabler SW = base.Owner.gameObject.GetComponent<TheMeagaSomethingWickedEnabler>();
                Destroy(SW);
                Pixelator.Instance.AdditionalCoreStackRenderPass = null;
            }
            */
        }
        protected override void OnDestroy()
		{
            base.OnDestroy();
		}

        //public static List<GameObject> ExtantShrines = new List<GameObject>();
        
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
            RedCasingID = item.PickupObjectId;

        }
        public static int RedCasingID;


        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject result = base.Drop(player);
            return result;
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        

        protected override void OnDestroy()
        {
            //SomethingWickedEventManager.ShouldDoSomethingWickedEvent = false;
            base.OnDestroy();
        }
    }
}

