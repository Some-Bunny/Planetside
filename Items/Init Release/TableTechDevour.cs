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
    public class TableTechDevour: PassiveItem
    {
        public static void Init()
        {
            string itemName = "Table Tech Devour";

            string resourceName = "Planetside/Resources/tabletechdevour.png";

            GameObject obj = new GameObject(itemName);

            TableTechDevour minigunrounds = obj.AddComponent<TableTechDevour>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Hungering Flips";
            string longDesc = "This ancient technique allows the user to feed a table flipped, sating it.\n\nChapter 17 of the Table Sutra. Those flipped are sated in their desires.";

            minigunrounds.SetupItem(shortDesc, longDesc, "psog");
            minigunrounds.quality = PickupObject.ItemQuality.B;
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:table_tech_devour",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "mutation",
                "tear_jerker",
                "super_meat_gun",
                "bloody_9mm"
            };
            CustomSynergies.Add("KILL KILL KILL", mandatoryConsoleIDs, optionalConsoleIDs, true);
            TableTechDevour.TableTechDevourID = minigunrounds.PickupObjectId;
            ItemIDs.AddToList(minigunrounds.PickupObjectId);

            minigunrounds.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);

        }
        public static int TableTechDevourID;
        public override void Pickup(PlayerController player)
        {
            player.OnTableFlipped = (Action<FlippableCover>)Delegate.Combine(player.OnTableFlipped, new Action<FlippableCover>(this.HandleFlip));
            base.Pickup(player);
        }
        private void HandleFlip(FlippableCover table)
        {
            Vector2 tabler = new Vector2(table.sprite.WorldCenter.x, table.sprite.WorldCenter.y);
            List<AIActor> activeEnemies = base.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            Vector2 centerPosition = tabler;
            if (activeEnemies != null)
            {
                foreach (AIActor aiactor in activeEnemies)
                {
                    bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < 4.5f && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && base.Owner != null && !aiactor.healthHaver.IsBoss;
                    if (ae)
                    {
                        GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(aiactor, tabler));
                        aiactor.EraseFromExistence(true);
                    }
                }
            }
            bool flagA = base.Owner.PlayerHasActiveSynergy("KILL KILL KILL");
            if (flagA)
            {
                bool flag = activeEnemies != null;
                if (flag)
                {
                    RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
                    AIActor randomActiveEnemy;
                    randomActiveEnemy = base.Owner.CurrentRoom.GetRandomActiveEnemy(true);
                    if (!randomActiveEnemy.healthHaver.IsBoss)
                    {
                        GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(randomActiveEnemy, tabler));
                        randomActiveEnemy.EraseFromExistence(true);
                    }
                }
            }
        }

        private IEnumerator HandleEnemySuck(AIActor target, Vector2 table)
        {
            bool Soundplayed = false;
            Transform copySprite = this.CreateEmptySprite(target);
            Vector3 startPosition = copySprite.transform.position;
            float elapsed = 0f;
            float duration = 0.7f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                bool flag3 = this.m_owner.CurrentGun && copySprite;
                if (flag3)
                {
                    Vector3 position = table;
                    float t = elapsed / duration * (elapsed / duration);
                    copySprite.position = Vector3.Lerp(startPosition, position, t);
                    copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                    copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                    position = default(Vector3);
                }
                if (elapsed >= 0.69f && Soundplayed == false)
                {
                    AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Bite_01", base.gameObject);
                    this.teleporter = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>();
                    UnityEngine.Object.Instantiate<GameObject>(this.teleporter.TelefragVFXPrefab, table, Quaternion.identity);
                    Soundplayed = true;
                }
                yield return null;
            }
            bool flag4 = copySprite;
            if (flag4)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
            }
            yield break;
        }
        private Transform CreateEmptySprite(AIActor target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            bool flag = target.optionalPalette != null;
            if (flag)
            {
                tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
            }
            return gameObject2.transform;
        }

        protected override void OnDestroy()
        {
            if (base.Owner != null)
            {
                base.Owner.OnTableFlipped = (Action<FlippableCover>)Delegate.Remove(base.Owner.OnTableFlipped, new Action<FlippableCover>(this.HandleFlip));
            }
            base.OnDestroy();
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnTableFlipped = (Action<FlippableCover>)Delegate.Remove(player.OnTableFlipped, new Action<FlippableCover>(this.HandleFlip));
            return base.Drop(player);
        }
        private TeleporterPrototypeItem teleporter;

    }
}