using System;
using System.Collections;
using ItemAPI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
namespace Planetside
{
    public class TatteredRobes : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Tattered Robes";

            string resourceName = "Planetside/Resources/tatteredrobes.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<TatteredRobes>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "TO DO";
            string longDesc = "TO DO";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.EXCLUDED;

        }
        private void oof(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            bool flag3 = this.m_owner.CurrentRoom != null;
            if (flag3)
            {
                GameObject synergyObjectToSpawn = base.gameObject.GetComponent<SpawnObjectPlayerItem>().objectToSpawn;
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(synergyObjectToSpawn, base.Owner.specRigidbody.UnitCenter, Quaternion.identity);
                this.spawnedPlayerObject = gameObject;
                tk2dBaseSprite component2 = gameObject.GetComponent<tk2dBaseSprite>();
                if (component2 != null)
                {
                    component2.PlaceAtPositionByAnchor(base.Owner.specRigidbody.UnitCenter.ToVector3ZUp(component2.transform.position.z), tk2dBaseSprite.Anchor.MiddleCenter);
                    if (component2.specRigidbody != null)
                    {
                        component2.specRigidbody.RegisterGhostCollisionException(base.Owner.specRigidbody);
                    }
                }
                KageBunshinController component3 = gameObject.GetComponent<KageBunshinController>();
                if (component3)
                {
                    component3.InitializeOwner(base.Owner);
                }
                gameObject.transform.position = gameObject.transform.position.Quantize(0.0625f);
                if (this.spawnedPlayerObject)
                {
                    SpawnObjectItem componentInChildren2 = this.spawnedPlayerObject.GetComponentInChildren<SpawnObjectItem>();
                    if (componentInChildren2)
                    {
                        componentInChildren2.SpawningPlayer = base.Owner;
                    }
                }
            }
        }


        public override void Pickup(PlayerController player)
        {
            player.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.oof);
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.oof);
            DebrisObject debrisObject = base.Drop(player);
            return debrisObject;
        }
        public GameObject spawnedPlayerObject;
        public GameObject objectToSpawn;

    }
}