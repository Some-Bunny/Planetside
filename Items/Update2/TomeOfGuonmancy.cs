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
//Garbage Code Incoming
namespace Planetside
{
    public class BulletGuonMaker : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Tome Of Guonmancy";
            string resourceName = "Planetside/Resources/tomeofguonmancy.png";
            GameObject obj = new GameObject(itemName);
            BulletGuonMaker activeitem = obj.AddComponent<BulletGuonMaker>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Irony On Another Level";
            string longDesc = "Captures nearby bullets and turns them into defensive orbitals. Despite being long forgotten and Guonmancy virtually dying out, the tome still holds up.\n\nSome say that Guonmancy still lives on and is secretly practiced by some.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 300f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.B;
            activeitem.gameObject.AddComponent<IronsideItemPool>();

            List<string> TimeGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "blue_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Time", TimeGuon, null, true);
            List<string> HPUPGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "pink_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Love", HPUPGuon, null, true);
            List<string> ClearGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "clear_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Purity", ClearGuon, null, true);
            List<string> ShootGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "orange_guon_stone"
            };
            CustomSynergies.Add("Chapter Of War", ShootGuon, null, true);
            List<string> GreenGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "green_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Restoration", GreenGuon, null, true);
            List<string> GlassGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "glass_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Glass", GlassGuon, null, true);
            List<string> BlankGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "white_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Silence", BlankGuon, null, true);
            List<string> RedGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "red_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Speed", RedGuon, null, true);
            BulletGuonMaker.BuildBasePrefab();
            BulletGuonMaker.TomeOfGuonmancyID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

        }
        public static int TomeOfGuonmancyID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        public static void BuildBasePrefab()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/Guons/EnergyPlatedGuon/energyshiledguon.png");
            //gameObject.AddComponent<tk2dSprite>(sprite);
            gameObject.name = $"Bullet orbital";
            SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(12, 12));
            PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
            speculativeRigidbody.CollideWithTileMap = false;
            speculativeRigidbody.CollideWithOthers = true;
            speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
            orbitalPrefab.shouldRotate = false;
            orbitalPrefab.orbitRadius = 2f;

            orbitalPrefab.orbitDegreesPerSecond = 60;
            orbitalPrefab.SetOrbitalTier(0);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            FakePrefab.MarkAsFakePrefab(gameObject);
            gameObject.SetActive(false);
            BaseBulletGuon = gameObject;
            //GameObject gameobject2 = PlayerOrbitalItem.CreateOrbital(User, gameObject, false);
        }

        public static GameObject BaseBulletGuon;
        public override bool CanBeUsed(PlayerController user)
        {

            bool flag3 = user.CurrentRoom != null;
            if (flag3)
            {
                ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
                if (allProjectiles != null)
                {
                    foreach (Projectile proj in allProjectiles)
                    {
                        bool ae = Vector2.Distance(proj.sprite.WorldCenter, user.sprite.WorldCenter) < 3.25f && proj != null && proj.specRigidbody != null && user != null && proj.Owner != user;
                        if (ae)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return false;
            }
            return false;
        }
        protected override void DoEffect(PlayerController user)
        {

            bool flag3 = user.CurrentRoom != null;
            if (flag3)
            {

                
                ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
                if (allProjectiles != null)
                {
                    AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", base.gameObject);
                    if (base.LastOwner.PlayerHasActiveSynergy("Chapter Of Restoration"))
                    {
                        this.random = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (random <= 0.05f)
                        {
                            PlayableCharacters characterIdentity = user.characterIdentity;
                            bool flag = characterIdentity != PlayableCharacters.Robot;
                            if (flag)
                            {
                                user.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Healing_Sparkles_001") as GameObject, Vector3.zero, true, false, false);
                                user.healthHaver.ApplyHealing(0.5f);
                            }
                            else
                            {
                                bool flag2 = characterIdentity == PlayableCharacters.Robot;
                                if (flag2)
                                {
                                    user.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Healing_Sparkles_001") as GameObject, Vector3.zero, true, false, false);
                                    user.healthHaver.Armor = user.healthHaver.Armor + 1f;
                                }
                            }
                        }
                    }
                    if (base.LastOwner.PlayerHasActiveSynergy("Chapter Of Glass"))
                    {
                        this.random = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (random <= 0.08f)
                        {
                            LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(565).gameObject, user);
                        }
                    }
                    GameManager.Instance.Dungeon.StartCoroutine(this.HandleBulletDeletionFrames(user.sprite.WorldCenter, 3.25f, 0.5f));
                    
                }
                
            }
        }
        public float random;
        private IEnumerator HandleBulletDeletionFrames(Vector3 centerPosition, float bulletDeletionSqrRadius, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
                for (int i = allProjectiles.Count - 1; i >= 0; i--)
                {
                    Projectile projectile = allProjectiles[i];
                    if (projectile)
                    {
                        if (!(projectile.Owner is PlayerController))
                        {
                            Vector2 vector = (projectile.transform.position - centerPosition).XY();
                            if (projectile.CanBeKilledByExplosions && vector.sqrMagnitude < bulletDeletionSqrRadius)
                            {
                                GameManager.Instance.Dungeon.StartCoroutine(this.HandleBulletSuck(projectile));
                            }
                        }
                    }
                }
                yield return null;
            }
            yield break;

        }



        private IEnumerator HandleBulletSuck(Projectile target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            this.BuildPrefab(tk2dSprite, base.LastOwner);

            target.DieInAir(false, true, true, false);
            yield break;
        }
        public GameObject BuildPrefab(tk2dSprite sprite, PlayerController User)
        {
            float speed = 60f;
            if (base.LastOwner.PlayerHasActiveSynergy("Chapter Of Time"))
            {
                speed *= 2;
            }
            GameObject gameobject2 = PlayerOrbitalItem.CreateOrbital(User, BaseBulletGuon, false);
            PlayerOrbital orb = gameobject2.GetComponent<PlayerOrbital>();
            orb.orbitDegreesPerSecond = speed;

            tk2dSprite tk2dSprite = gameobject2.GetOrAddComponent<tk2dSprite>();
            tk2dSprite.SetSprite(sprite.sprite.Collection, sprite.sprite.spriteId);

            CreatedGuonBulletsController yes = gameobject2.AddComponent<CreatedGuonBulletsController>();
            yes.maxDuration = 10f;
            
            if (base.LastOwner.PlayerHasActiveSynergy("Chapter Of Love"))
            {
                yes.SpawnsCharmGoop = true;
            }
            if (base.LastOwner.PlayerHasActiveSynergy("Chapter Of Purity"))
            {
                yes.ClearsGoop = true;
            }
            if (base.LastOwner.PlayerHasActiveSynergy("Chapter Of War"))
            {
                yes.ShootsOnDestruction = true;
            }
            if (base.LastOwner.PlayerHasActiveSynergy("Chapter Of Silence"))
            {
                yes.ChanceToBlank = true;
            }
            if (base.LastOwner.PlayerHasActiveSynergy("Chapter Of Speed"))
            {
                yes.AddSpeed = true;
            }
            return gameobject2;
        }
    }
}



