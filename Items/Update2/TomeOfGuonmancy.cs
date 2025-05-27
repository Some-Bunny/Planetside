﻿using System;
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
using Alexandria.PrefabAPI;
//Garbage Code Incoming
namespace Planetside
{
    public class BulletGuonMaker : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Tome Of Guonmancy";
            GameObject obj = new GameObject(itemName);
            BulletGuonMaker activeitem = obj.AddComponent<BulletGuonMaker>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("tomeofguonmancy"), data, obj);
            string shortDesc = "Irony On Another Level";
            string longDesc = "Captures nearby bullets and turns them into defensive orbitals. Despite being long forgotten and Guonmancy virtually dying out, the tome still holds up.\n\nSome say that Guonmancy still lives on and is secretly practiced by some.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 250f);
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
            GameObject gameObject = PrefabBuilder.BuildObject("Bullet Orbital");

            tk2dSprite sprite = gameObject.AddComponent<tk2dSprite>();
            sprite.collection = StaticSpriteDefinitions.Guon_Sheet_Data;
            sprite.SetSprite(StaticSpriteDefinitions.Guon_Sheet_Data.GetSpriteIdByName("energyshiledguon_001"));


            sprite.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;


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
                    for (int i = 0; i < allProjectiles.Count; i++)
                    {
                        Projectile proj = allProjectiles[i];
                        if (Vector2.Distance(proj.sprite.WorldCenter, user.sprite.WorldCenter) < 3.25f && proj != null && proj.specRigidbody != null && user != null && proj.Owner != user)
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
        public override void DoEffect(PlayerController user)
        {

            if (user.CurrentRoom != null)
            {        
                ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
                if (allProjectiles != null)
                {
                    AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", user.gameObject);
                    if (user.PlayerHasActiveSynergy("Chapter Of Restoration"))
                    {
                        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.07f)
                        {
                            if (user.characterIdentity != PlayableCharacters.Robot)
                            {
                                user.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Healing_Sparkles_001") as GameObject, Vector3.zero, true, false, false);
                                user.healthHaver.ApplyHealing(0.5f);
                            }
                            else
                            {
                                user.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Healing_Sparkles_001") as GameObject, Vector3.zero, true, false, false);
                                user.healthHaver.Armor++;
                            }
                        }
                    }
                    if (user.PlayerHasActiveSynergy("Chapter Of Glass"))
                    {
                        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f)
                        {
                            LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(565).gameObject, user);
                        }
                    }
                    user.StartCoroutine(this.HandleBulletDeletionFrames(user.sprite.WorldCenter, 3.25f, 0.5f));   
                }         
            }
        }
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
            this.BuildPrefab(tk2dSprite, base.LastOwner, target.baseData.speed);

            target.DieInAir(false, true, true, false);
            yield break;
        }
        public GameObject BuildPrefab(tk2dSprite sprite, PlayerController User, float Speed)
        {
            GameObject gameobject2 = PlayerOrbitalItem.CreateOrbital(User, BaseBulletGuon, false);
            PlayerOrbital orb = gameobject2.GetComponent<PlayerOrbital>();
            orb.orbitDegreesPerSecond = base.LastOwner.PlayerHasActiveSynergy("Chapter Of Time") == true ? 120 : 60;

            tk2dSprite tk2dSprite = gameobject2.GetOrAddComponent<tk2dSprite>();
            tk2dSprite.SetSprite(sprite.sprite.Collection, sprite.sprite.spriteId);

            CreatedGuonBulletsController yes = gameobject2.AddComponent<CreatedGuonBulletsController>();
            yes.sourcePlayer = User;
            yes.maxDuration = 15f;
            orb.orbitDegreesPerSecond = Mathf.Max(60, Speed * 5);

            yes.SpawnsCharmGoop = User.PlayerHasActiveSynergy("Chapter Of Love");
            yes.ClearsGoop = User.PlayerHasActiveSynergy("Chapter Of Purity");
            yes.ShootsOnDestruction = User.PlayerHasActiveSynergy("Chapter Of War");
            yes.ChanceToBlank = User.PlayerHasActiveSynergy("Chapter Of Silence");
            yes.AddSpeed = User.PlayerHasActiveSynergy("Chapter Of Speed");

            return gameobject2;
        }
    }
}



