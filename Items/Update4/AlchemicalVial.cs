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


namespace Planetside
{
    public class AlchemicalVial : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Projectile Transmutator";
            string resourceName = "Planetside/Resources/precursor1.png";
            GameObject obj = new GameObject(itemName);
            AlchemicalVial activeitem = obj.AddComponent<AlchemicalVial>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Completely Stabile";
            string longDesc = "Transmutes all of your projectiles, reload on a full clip to change ttransmutation.\n\nDespite what the label says, this is actually a bottle of lead paint. Don't drink it...";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 5f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.D;
            activeitem.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);
            AlchemicalVial.AlchemicalVialID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);
            

            AlchemicalVial.spriteIDs.Add("nAn", SpriteBuilder.AddSpriteToCollection(AlchemicalVial.spritePaths[0], activeitem.sprite.Collection));
            AlchemicalVial.spriteIDs.Add("fire", SpriteBuilder.AddSpriteToCollection(AlchemicalVial.spritePaths[1], activeitem.sprite.Collection));
            AlchemicalVial.spriteIDs.Add("poison", SpriteBuilder.AddSpriteToCollection(AlchemicalVial.spritePaths[2], activeitem.sprite.Collection));

            AlchemicalVial.spriteIDs.Add("frail", SpriteBuilder.AddSpriteToCollection(AlchemicalVial.spritePaths[3], activeitem.sprite.Collection));
            AlchemicalVial.spriteIDs.Add("cheese", SpriteBuilder.AddSpriteToCollection(AlchemicalVial.spritePaths[4], activeitem.sprite.Collection));

            AlchemicalVial.ActiveIDS.Add("fire");
            AlchemicalVial.ActiveIDS.Add("poison");

            CurrentCount = 1;
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:projectile_transmutator",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "elimentaler",
                "partially_eaten_cheese"
            };
            CustomSynergies.Add("cheemsburbger", mandatoryConsoleIDs, optionalConsoleIDs, true);
            List<string> optionalConsoleIDs2 = new List<string>
            {
                "psog:wither_lance",
                "psog:frailty_ammolet",
                "psog:frailty_rounds",

            };
            CustomSynergies.Add("frail", mandatoryConsoleIDs, optionalConsoleIDs2, true);
        }
        public static int AlchemicalVialID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReloadPressed += reloadPressed;
        }
        public void reloadPressed(PlayerController player, Gun gun)
        {
            if (gun.ClipShotsRemaining == gun.ClipCapacity)
            {
                CurrentCount++;
                if (CurrentCount == ActiveIDS.Count+1) { CurrentCount = 1; }
                int yes;
                string c = AlchemicalVial.ActiveIDS[CurrentCount - 1] != null ? AlchemicalVial.ActiveIDS[CurrentCount - 1] : "nAn";
                AlchemicalVial.spriteIDs.TryGetValue(c, out yes);
                base.sprite.SetSprite(yes);
                player.BloopItemAboveHead(base.sprite);
            }
        }
        public override void Update()
        {
            if (base.LastOwner)
            {
                if (base.LastOwner.PlayerHasActiveSynergy("cheemsburbger") && !ActiveIDS.Contains("cheese"))
                {ActiveIDS.Add("cheese");}
                else if (!base.LastOwner.PlayerHasActiveSynergy("cheemsburbger") && ActiveIDS.Contains("cheese"))
                { ActiveIDS.Remove("cheese"); }
                if (base.LastOwner.PlayerHasActiveSynergy("frail") && !ActiveIDS.Contains("frail"))
                { ActiveIDS.Add("frail");}
                else if (!base.LastOwner.PlayerHasActiveSynergy("frail") && ActiveIDS.Contains("frail"))
                {ActiveIDS.Remove("frail");}

                if (CurrentCount == ActiveIDS.Count + 1) 
                {
                    CurrentCount=1;
                    int yes;
                    AlchemicalVial.spriteIDs.TryGetValue(AlchemicalVial.ActiveIDS[CurrentCount-1], out yes);
                    base.sprite.SetSprite(yes);
                    base.LastOwner.BloopItemAboveHead(base.sprite);
                }
              
            }
        }
        protected override void OnPreDrop(PlayerController player)
        {
            player.OnReloadPressed -= reloadPressed;
            base.OnPreDrop(player);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (base.LastOwner != null)
            {
                base.LastOwner.OnReloadPressed -= reloadPressed;
            }
        }
        protected override void DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_OBJ_bottle_cork_01", base.gameObject);
            for (int i = 0; i < StaticReferenceManager.AllProjectiles.Count; i++)
            {
                Projectile proj = StaticReferenceManager.AllProjectiles[i];
                PlayerController player = proj.Owner as PlayerController;
                bool isBem = proj.GetComponent<BasicBeamController>() != null;
                if (isBem != true && proj.Owner != null && proj.Owner == player && proj != null)
                {
                    DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.FireDef).TimedAddGoopCircle(proj.transform.PositionVector2(), 2f, 0.33f, false);

                    proj.DieInAir();
                }
            }
                
        }
        private static Dictionary<string, int> spriteIDs = new Dictionary<string, int>();
        private static List<string> ActiveIDS = new List<string>();
        private static int CurrentCount;

        private static readonly string[] spritePaths = new string[]
        {
            "Planetside/Resources/precursor1.png",
            "Planetside/Resources/precursor2.png",
            "Planetside/Resources/precursor3.png",
            "Planetside/Resources/precursor4.png",
            "Planetside/Resources/precursor5.png"

        };
    }
}



