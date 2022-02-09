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

using UnityEngine.Serialization;
using MonoMod.Utils;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;


namespace Planetside
{
    public class PortablePylon : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Portable Pylon";
            string resourceName = "Planetside/Resources/portablepylon.png";
            GameObject obj = new GameObject(itemName);
            PortablePylon activeitem = obj.AddComponent<PortablePylon>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Swing By";
            string longDesc = "A portable tesla pylon that runs a current through the air and into your body.\n\nScience genuinely cannot explain why it doesn't just instantly kill you.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 350f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.C;
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:portable_pylon",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "prototype_railgun",
                "armor_synthesizer",
                "blast_helmet",
                "grappling_hook",
                "bionic_leg"
            };
            CustomSynergies.Add("Loader Chassis", mandatoryConsoleIDs, optionalConsoleIDs, true);
            List<string> optionalConsoleID1s = new List<string>
            {
                "gungeon_blueprint"
            };
            CustomSynergies.Add("Sentry Goin' Up!", mandatoryConsoleIDs, optionalConsoleID1s, true);
            activeitem.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);

            PortablePylon.PortablePylonID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

        }
        public static int PortablePylonID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        public GameObject spawnedPlayerObject;
        public GameObject objectToSpawn;
        public override bool CanBeUsed(PlayerController user)
        {
            return user.IsInCombat;
        }
        protected override void DoEffect(PlayerController user)
        {
            Vector2 player = user.specRigidbody.UnitCenter;
            IntVector2? vector = (user as PlayerController).CurrentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 2), CellTypes.FLOOR | CellTypes.PIT, false, null);
            Vector2 RandomInRoom = vector.Value.ToVector2();
            Vector2 usedvector;
            int fuck = user.PlayerHasActiveSynergy("Sentry Goin' Up!") == true ? 2 : 1;
            for (int counter = 0; counter < fuck; counter++)
            {
                if (counter == 1)
                {
                    usedvector = RandomInRoom;
                }
                else
                {
                    usedvector = player;
                }

                bool flagE = user.PlayerHasActiveSynergy("Loader Chassis");
                if (flagE)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(LoaderPylonSynergyFormeController.TurretSynprefab, user.specRigidbody.UnitCenter, Quaternion.identity);
                    this.spawnedPlayerObject = gameObject;
                    tk2dBaseSprite component2 = gameObject.GetComponent<tk2dBaseSprite>();
                    if (component2 != null)
                    {
                        component2.PlaceAtPositionByAnchor(usedvector.ToVector3ZUp(component2.transform.position.z), tk2dBaseSprite.Anchor.MiddleCenter);
                        if (component2.specRigidbody != null)
                        {
                            component2.specRigidbody.RegisterGhostCollisionException(user.specRigidbody);
                        }
                    }
                    LoaderPylonSynergyFormeController yah = gameObject.AddComponent<LoaderPylonSynergyFormeController>();
                    if (yah != null)
                    {
                        yah.maxDuration = 120f;
                    }
                }
                else
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(LoaderPylonController.Turretprefab, user.specRigidbody.UnitCenter, Quaternion.identity);
                    this.spawnedPlayerObject = gameObject;
                    tk2dBaseSprite component2 = gameObject.GetComponent<tk2dBaseSprite>();
                    if (component2 != null)
                    {
                        component2.PlaceAtPositionByAnchor(usedvector.ToVector3ZUp(component2.transform.position.z), tk2dBaseSprite.Anchor.MiddleCenter);
                        if (component2.specRigidbody != null)
                        {
                            component2.specRigidbody.RegisterGhostCollisionException(user.specRigidbody);
                        }
                    }
                    LoaderPylonController yah = gameObject.AddComponent<LoaderPylonController>();
                    if (yah != null)
                    {
                        yah.maxDuration = 120f;
                    }
                }
            }
        }
    }
}



