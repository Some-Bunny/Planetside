using Alexandria.ItemAPI;
using Dungeonator;
using UnityEngine;

namespace Items
{
    class TrueGunpowder : PassiveItem
    {
        public static int itemID;
        public static void Init()
        {
            string itemName = "True Gunpowder";

            string resourceName = "Items/Resources/ItemSprites/Passives/true_gunpowder.png";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<TrueGunpowder>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "A Weapon That Cuts On Its Own";
            string longDesc = "Every 120 damage dealt, a random enemy in the room is killed. Bosses take heavy damage instead.\n\n\"At last, I have found it: the gunpowder of the first gun fired within the Gungeon. With it, perhaps I can finally escape. But I must be quick. They know I have it. It's only a matter of time until they catch up.\"- Torn note.";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "cel");

            item.quality = ItemQuality.EXCLUDED;
            item.sprite.IsPerpendicular = true;
            item.CanBeDropped = false;
            item.CanBeSold = false;
            itemID = item.PickupObjectId;
        }
        private float cooldown = 0;
        private void GrabbyGrabTime(PlayerController player, float oof)
        {

            cooldown += oof;
            if (cooldown >= 80)
            {
                AIActor actor;
                RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
                actor = Owner.CurrentRoom.GetRandomActiveEnemy(true);
                var hand = (EnemyDatabase.GetOrLoadByGuid("cd88c3ce60c442e9aa5b3904d31652bc")).GetComponent<LichDeathController>().HellDragVFX.GetComponent<HellDraggerArbitrary>().HellDragVFX;
                //HellDraggerArbitrary component2 = UnityEngine.Object.Instantiate<GameObject>(hand).GetComponent<HellDraggerArbitrary>();

                AkSoundEngine.PostEvent("Play_BOSS_lichB_grab_01", gameObject);
                if (!actor.healthHaver.IsBoss)
                {
                    actor.PlayEffectOnActor(hand, new Vector3(0f, 0, 0f), false, false, false);
                    actor.CorpseObject = null;
                    actor.healthHaver.ApplyDamage(100000, Vector2.zero, "GetFuckedNerd", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);


                }
                else
                {
                    if (actor.healthHaver.IsBoss)
                    {
                        actor.PlayEffectOnActor(hand, new Vector3(0f, 0, 0f), false, false, false);
                        actor.healthHaver.ApplyDamage(120, Vector2.zero, "GetFuckedNerdButSlightlyLessThanNormalNerds", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);

                    }
                }

                cooldown = 0;
            }
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnDealtDamage += GrabbyGrabTime;
        }
        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            debrisObject.GetComponent<TrueGunpowder>().m_pickedUpThisRun = true;
            player.OnDealtDamage -= GrabbyGrabTime;
            return debrisObject;
        }
    }
}
