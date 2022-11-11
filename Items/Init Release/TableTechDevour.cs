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
using SynergyAPI;
namespace Planetside
{
    public class TableTechDevour: PassiveItem
    {
        public static void Init()
        {
            string itemName = "Table Tech Devour";

            string resourceName = "Planetside/Resources/tabletechdevour.png";

            GameObject obj = new GameObject(itemName);

            TableTechDevour item = obj.AddComponent<TableTechDevour>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Hungering Flips";
            string longDesc = "This ancient technique allows the user to feed a table flipped, sating it.\n\nChapter 17 of the Table Sutra. Those flipped are sated in their desires.";

            item.SetupItem(shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.B;
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
            List<string> optionalConsoleIDs2 = new List<string>
            {
                "ring_of_chest_vampirism",
                "antibody",
                "pink_guon_stone"
            };
            CustomSynergies.Add("Blood For Wood", mandatoryConsoleIDs, optionalConsoleIDs2, true);
            TableTechDevour.TableTechDevourID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);


            //SynergyAPI.SynergyBuilder.AddItemToSynergy(minigunrounds, CustomSynergyType.PAPERWORK);



            item.AddItemToSynergy(CustomSynergyType.PAPERWORK);


            item.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);    

            Gun gun4 = PickupObjectDatabase.GetById(43) as Gun;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun4.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 22f;
            projectile.baseData.speed *= 0.8f;
            projectile.AdditionalScaleMultiplier = 1.6f;
            projectile.shouldRotate = true;
            projectile.pierceMinorBreakables = true;
            projectile.baseData.range = 1000f;

            BounceProjModifier bouncy = projectile.gameObject.AddComponent<BounceProjModifier>();
            bouncy.numberOfBounces = 2;

            projectile.AnimateProjectile(new List<string> {
                "goreProj_001",
                "goreProj_002",
                "goreProj_003",
                "goreProj_004",
                "goreProj_005",
                "goreProj_006",
                "goreProj_007",
                "goreProj_008",


            }, 12, true, new List<IntVector2> {
                new IntVector2(6, 6),
                new IntVector2(8, 4),
                new IntVector2(10, 4),
                new IntVector2(8, 4),
                new IntVector2(6, 6),
                new IntVector2(4, 8),
                new IntVector2(4, 10),
                new IntVector2(6, 8),
            }, AnimateBullet.ConstructListOfSameValues(false, 8), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 8), AnimateBullet.ConstructListOfSameValues(true, 8), AnimateBullet.ConstructListOfSameValues(false, 8),AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 8));
            projectile.hitEffects.alwaysUseMidair = true;
            projectile.hitEffects.overrideMidairDeathVFX = (PickupObjectDatabase.GetById(368) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
            goreProj = projectile;
            


        }
        public static Projectile goreProj;
        public static int TableTechDevourID;
        public override void Pickup(PlayerController player)
        {
            player.OnTableFlipped = (Action<FlippableCover>)Delegate.Combine(player.OnTableFlipped, new Action<FlippableCover>(this.HandleFlip));
            base.Pickup(player);
        }
        private void HandleFlip(FlippableCover table)
        {
           
            float num = 1f;
            GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
            if (lastLoadedLevelDefinition != null)
            {
                num = lastLoadedLevelDefinition.enemyHealthMultiplier;
            }
            List<AIActor> enemiesToSucc = new List<AIActor>();
            int projToSpawn = 1;
            Vector2 tabler = new Vector2(table.sprite.WorldCenter.x, table.sprite.WorldCenter.y);
            List<AIActor> activeEnemies = base.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            Vector2 centerPosition = tabler;
            if (activeEnemies != null)
            {
                foreach (AIActor aiactor in activeEnemies)
                {
                    GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(368) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
                    tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
                    component.PlaceAtPositionByAnchor(aiactor.sprite.WorldCenter.ToVector3ZisY(), tk2dBaseSprite.Anchor.MiddleCenter);
                    component.HeightOffGround = 35f;
                    component.UpdateZDepth();
                    tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                    if (component2 != null)
                    {
                        component2.ignoreTimeScale = true;
                        component2.AlwaysIgnoreTimeScale = true;
                        component2.AnimateDuringBossIntros = true;
                        component2.alwaysUpdateOffscreen = true;
                        component2.playAutomatically = true;
                    }
                    bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < 5f && aiactor.healthHaver.GetCurrentHealth() < 20* num && aiactor != null && aiactor.specRigidbody != null && base.Owner != null && !aiactor.healthHaver.IsBoss;
                    if (ae)
                    {
                        float count = aiactor.healthHaver.GetMaxHealth()/20;
                        projToSpawn += (int)(1+count);
                        enemiesToSucc.Add(aiactor);
                    }
                    else if (aiactor != null && aiactor.specRigidbody != null && base.Owner != null)
                    {
                        int HP = !aiactor.healthHaver.IsBoss == true ? 1 : 6;
                        projToSpawn += HP;
                    }
                }
                if (base.Owner.PlayerHasActiveSynergy("KILL KILL KILL"))
                {
                    AIActor randomActiveEnemy;
                    randomActiveEnemy = base.Owner.CurrentRoom.GetRandomActiveEnemy(true);
                    if (!randomActiveEnemy.healthHaver.IsBoss && !enemiesToSucc.Contains(randomActiveEnemy))
                    {
                        projToSpawn+=2;
                        enemiesToSucc.Add(randomActiveEnemy);
                    }
                }
            }
            
            GameManager.Instance.StartCoroutine(HandleEnemySuck(enemiesToSucc, tabler, table, projToSpawn));
        }

        private IEnumerator HandleEnemySuck(List<AIActor> targets, Vector2 table, FlippableCover tableActual, int AmountToSpawn)
        {
            if (targets != null || targets.Count >= 0)
            {
                Dictionary<Transform, Vector3> transforms = new Dictionary<Transform, Vector3>();
                foreach (AIActor aIActor in targets)
                {
                    Transform copySprite = this.CreateEmptySprite(aIActor);
                    Vector3 startPosition = copySprite.transform.position;
                    transforms.Add(copySprite, startPosition);
                    aIActor.EraseFromExistenceWithRewards(false);
                }

                float elapsed = 0f;
                float duration = 0.7f;
                while (elapsed < duration)
                {
                    elapsed += BraveTime.DeltaTime;
                    foreach (var Value in transforms)
                    {
                        bool flag3 = this.m_owner.CurrentGun && Value.Key;
                        if (flag3)
                        {
                            Vector3 position = table;
                            float t = elapsed / duration * (elapsed / duration);
                            Value.Key.position = Vector3.Lerp(Value.Value, position, t);
                            Value.Key.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * Value.Key.rotation;
                            Value.Key.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                            position = default(Vector3);
                        }
                    }
                    yield return null;
                }
                foreach (var dict in transforms)
                {
                    if (base.Owner != null && base.Owner.PlayerHasActiveSynergy("Blood For Wood") && (UnityEngine.Random.value > 0.0833f)){
                        AmountToSpawn++;
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(base.Owner.ForceZeroHealthState == false ? 73:120 ).gameObject,table.ToVector3ZisY(),new Vector2(0,0), 0);
                    }
                    if (dict.Key != null) { UnityEngine.Object.Destroy(dict.Key.gameObject); }
                }
                
            }
           
            IntVector2 intVector2FromDirection = DungeonData.GetIntVector2FromDirection(tableActual.DirectionFlipped);
            GameObject prefab = goreProj.gameObject;
            for (int i = 0; i < AmountToSpawn; i++)
            {
                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(prefab, table, Quaternion.Euler(0f, 0f, intVector2FromDirection.ToVector2().ToAngle() + UnityEngine.Random.Range(-25, 25)), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {
                    component.Owner = base.Owner;
                    component.Shooter = base.Owner.specRigidbody;
                    component.specRigidbody.RegisterTemporaryCollisionException(tableActual.specRigidbody, 0.5f);
                }
                
            }        
            AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Bite_01", base.gameObject);
            this.teleporter = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>();
            UnityEngine.Object.Instantiate<GameObject>(this.teleporter.TelefragVFXPrefab, table, Quaternion.identity);
           

           
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