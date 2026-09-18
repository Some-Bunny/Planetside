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
using Alexandria.PrefabAPI;
using Alexandria.Integrations;
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
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 300f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.B;
            activeitem.gameObject.AddComponent<IronsideItemPool>();

            List<string> TimeGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "blue_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Time", TimeGuon, null, true).AddItemTip("Doubles orbit speed of the temporary orbitals.");
            List<string> HPUPGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "pink_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Love", HPUPGuon, null, true).AddItemTip("Spawns charming goop when temporary orbitals expire.");
            List<string> ClearGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "clear_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Purity", ClearGuon, null, true).AddItemTip("Temporary orbitals clear goop around them.");
            List<string> ShootGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "orange_guon_stone"
            };
            CustomSynergies.Add("Chapter Of War", ShootGuon, null, true).AddItemTip("Temporary orbitals fire a projectile when they expire.");
            List<string> GreenGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "green_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Restoration", GreenGuon, null, true).AddItemTip("Using the item has a small chance to heal the player.");
            List<string> GlassGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "glass_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Glass", GlassGuon, null, true).AddItemTip("Using the item has a small chance to give the player a Glass Guon Stone.");
            List<string> BlankGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "white_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Silence", BlankGuon, null, true).AddItemTip("Temporary orbitals have a chance to do a micro-blank when they expire.");
            List<string> RedGuon = new List<string>
            {
                "psog:tome_of_guonmancy",
                "red_guon_stone"
            };
            CustomSynergies.Add("Chapter Of Speed", RedGuon, null, true).AddItemTip("Grants the player a speed boost for every temporary orbital they have.");
            BulletGuonMaker.BuildBasePrefab();
            BulletGuonMaker.TomeOfGuonmancyID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);
            activeitem.AddItemTip("On use, temporarily turns nearby enemy projectiles into protective orbitals that last 10 seconds.");

        }
        public static int TomeOfGuonmancyID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        float E;
        private float CaptureRadius = 3.5f;
        public override void Update()
        {
            base.Update();
            if (LastOwner)
            {
                if (LastOwner.CurrentItem == this || Active == true)
                {
                    E += (Active ? 480 : 48) * BraveTime.DeltaTime;
                    var m = MathToolbox.GetUnitOnCircle(E, CaptureRadius);
                    var m1 = MathToolbox.GetUnitOnCircle(E + 120, CaptureRadius);
                    var m2 = MathToolbox.GetUnitOnCircle(E + 240, CaptureRadius);
                    float a = Active ? 0.2f : 0.5f;
                    var Col = Active ? new Color(1, 0.06f, 0.1f, 0.5f) : new Color(0.3f, 0.06f, 0, 0.125f);
                    GlobalSparksDoer.DoSingleParticle((LastOwner.sprite.WorldCenter) + m, Vector3.up * UnityEngine.Random.Range(0.2f, 0.4f), 0.1f, a, Col, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    GlobalSparksDoer.DoSingleParticle((LastOwner.sprite.WorldCenter) + m1, Vector3.up * UnityEngine.Random.Range(0.2f, 0.4f), 0.1f, a, Col, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    GlobalSparksDoer.DoSingleParticle((LastOwner.sprite.WorldCenter) + m2, Vector3.up * UnityEngine.Random.Range(0.2f, 0.4f), 0.1f, a, Col, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                }
            }
        }

        


        public static void BuildBasePrefab()
        {
            GameObject gameObject = PrefabBuilder.BuildObject("Bullet Orbital");

            tk2dSprite sprite = gameObject.AddComponent<tk2dSprite>();
            sprite.collection = StaticSpriteDefinitions.Guon_Sheet_Data;
            sprite.SetSprite(StaticSpriteDefinitions.Guon_Sheet_Data.GetSpriteIdByName("energyshiledguon_001"));


            sprite.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;
            sprite.usesOverrideMaterial = true;
            sprite.renderer.material.shader = ShaderCache.Acquire("tk2d/CutoutVertexColorTintableTilted");
            sprite.renderer.material.SetColor("_OverrideColor", Color.white);

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
                    user.StartCoroutine(this.HandleBulletDeletionFrames( CaptureRadius + 0.25f, 0.5f));   
                }         
            }
        }
        private bool Active = false;
        private IEnumerator HandleBulletDeletionFrames(float bulletDeletionSqrRadius, float duration)
        {
            Active = true;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (LastOwner == null)
                    break;

                elapsed += BraveTime.DeltaTime;
                ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
                for (int i = allProjectiles.Count - 1; i > -1; i--)
                {
                    Projectile projectile = allProjectiles[i];
                    if (projectile && projectile.gameObject.activeSelf)
                    {
                        if (!(projectile.Owner is PlayerController))
                        {

                            if (Vector2.Distance(LastOwner.sprite.WorldCenter, projectile.transform.position) <= bulletDeletionSqrRadius)
                            {
                                GameManager.Instance.Dungeon.StartCoroutine(this.HandleBulletSuck(projectile, elapsed));
                            }
                        }
                    }
                }
                yield return null;
            }
            Active = false;
            yield break;
        }
        private IEnumerator HandleBulletSuck(Projectile sprite, float ActiveTime)
        {

            GameObject gameobject2 = PlayerOrbitalItem.CreateOrbital(LastOwner, BaseBulletGuon, false);
            PlayerOrbital orb = gameobject2.GetComponent<PlayerOrbital>();
            orb.orbitDegreesPerSecond = base.LastOwner.PlayerHasActiveSynergy("Chapter Of Time") == true ? 120 : 60;

            tk2dSprite tk2dSprite = gameobject2.GetOrAddComponent<tk2dSprite>();
            tk2dSprite.SetSprite(sprite.sprite.Collection, sprite.sprite.spriteId);

            CreatedGuonBulletsController yes = gameobject2.AddComponent<CreatedGuonBulletsController>();
            yes.sourcePlayer = LastOwner;
            yes.maxDuration = 10f - ActiveTime;


            yes.SpawnsCharmGoop = LastOwner.PlayerHasActiveSynergy("Chapter Of Love");
            yes.ClearsGoop = LastOwner.PlayerHasActiveSynergy("Chapter Of Purity");
            yes.ShootsOnDestruction = LastOwner.PlayerHasActiveSynergy("Chapter Of War");
            yes.ChanceToBlank = LastOwner.PlayerHasActiveSynergy("Chapter Of Silence");
            yes.AddSpeed = LastOwner.PlayerHasActiveSynergy("Chapter Of Speed");

            sprite.DieInAir(false, true, true, false);
            yield break;
        }

    }
}



