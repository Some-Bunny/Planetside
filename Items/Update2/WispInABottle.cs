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
    public class WispInABottle : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Wisp In A Bottle";
            string resourceName = "Planetside/Resources/WispInABottle.png";
            GameObject obj = new GameObject(itemName);
            WispInABottle testActive = obj.AddComponent<WispInABottle>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Unmatched Power";
            string longDesc = "A bottle that contains a whole living star inside of it.\n\nUse with caution.";
            testActive.SetupItem(shortDesc, longDesc, "psog");
            testActive.SetCooldownType(ItemBuilder.CooldownType.Damage, 900f);
            testActive.consumable = false;
            testActive.quality = PickupObject.ItemQuality.B;
            testActive.SetupUnlockOnCustomFlag(CustomDungeonFlags.DEFEAT_OPHANAIM, true);

            GameObject gameobject2 = ItemBuilder.AddSpriteToObject("SunWispObject", "Planetside/Resources/VFX/Sun/sunflare_chargeup_1", null);
            FakePrefab.MarkAsFakePrefab(gameobject2);
            UnityEngine.Object.DontDestroyOnLoad(gameobject2);
            tk2dSpriteAnimator animator = gameobject2.AddComponent<tk2dSpriteAnimator>();

            //=====================================
            
            tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
            animationClip.fps = 12;
            animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            animationClip.name = "spawn";
            GameObject spriteObject = new GameObject("SunSpawn");
            ItemBuilder.AddSpriteToObject("SunSpawn", $"Planetside/Resources/VFX/Sun/sunflare_chargeup_1", spriteObject);
            tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
            starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
            starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
            tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
            {
                starterFrame
            };
            animationClip.frames = frameArray;
            for (int i = 2; i < 13; i++)
            {
                GameObject spriteForObject = new GameObject("spriteForObject");
                ItemBuilder.AddSpriteToObject("SunSpawn", $"Planetside/Resources/VFX/Sun/sunflare_chargeup_{i}", spriteForObject);
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
                frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
                animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            
            //=====================================
            tk2dSpriteAnimationClip fireanim = new tk2dSpriteAnimationClip();
            fireanim.fps = 11;
            fireanim.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
            fireanim.name = "fire";
            GameObject spriteObject1 = new GameObject("SunFire");
            ItemBuilder.AddSpriteToObject("SunFire", $"Planetside/Resources/VFX/Sun/sunflare_fire_001", spriteObject1);
            tk2dSpriteAnimationFrame starterFrame1 = new tk2dSpriteAnimationFrame();
            starterFrame1.spriteId = spriteObject1.GetComponent<tk2dSprite>().spriteId;
            starterFrame1.spriteCollection = spriteObject1.GetComponent<tk2dSprite>().Collection;
            tk2dSpriteAnimationFrame[] frameArray1 = new tk2dSpriteAnimationFrame[]
            {
                starterFrame1
            };
            fireanim.frames = frameArray1;
            for (int i = 2; i < 5; i++)
            {
                GameObject spriteForObject1 = new GameObject("spriteForObject");
                ItemBuilder.AddSpriteToObject("SunFire", $"Planetside/Resources/VFX/Sun/sunflare_fire_00{i}", spriteForObject1);
                tk2dSpriteAnimationFrame frame1 = new tk2dSpriteAnimationFrame();
                frame1.spriteId = spriteForObject1.GetComponent<tk2dBaseSprite>().spriteId;
                frame1.spriteCollection = spriteForObject1.GetComponent<tk2dBaseSprite>().Collection;
                fireanim.frames = fireanim.frames.Concat(new tk2dSpriteAnimationFrame[] { frame1 }).ToArray();
            }
            //=====================================
            

            tk2dSpriteAnimationClip despawn = new tk2dSpriteAnimationClip();
            despawn.fps = 12;
            despawn.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            despawn.name = "despawn";
            GameObject spriteObject2 = new GameObject("SunDespawn");
            ItemBuilder.AddSpriteToObject("SunDespawn", $"Planetside/Resources/VFX/Sun/sunflare_chargeup_11", spriteObject2);
            tk2dSpriteAnimationFrame starterFrame2 = new tk2dSpriteAnimationFrame();
            starterFrame2.spriteId = spriteObject2.GetComponent<tk2dSprite>().spriteId;
            starterFrame2.spriteCollection = spriteObject2.GetComponent<tk2dSprite>().Collection;
            tk2dSpriteAnimationFrame[] frameArray2 = new tk2dSpriteAnimationFrame[]
            {
                starterFrame2
            };
            despawn.frames = frameArray2;
            for (int i = 10; i > 1; i--)
            {
                GameObject spriteForObject2 = new GameObject("spriteForObject");
                ItemBuilder.AddSpriteToObject("SunSpawn", $"Planetside/Resources/VFX/Sun/sunflare_chargeup_{i}", spriteForObject2);
                tk2dSpriteAnimationFrame frame2 = new tk2dSpriteAnimationFrame();
                frame2.spriteId = spriteForObject2.GetComponent<tk2dBaseSprite>().spriteId;
                frame2.spriteCollection = spriteForObject2.GetComponent<tk2dBaseSprite>().Collection;
                despawn.frames = despawn.frames.Concat(new tk2dSpriteAnimationFrame[] { frame2 }).ToArray();
            }
            //=====================================

            animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip , fireanim , despawn };
            animator.DefaultClipId = animator.GetClipIdByName("fire");
            animator.playAutomatically = false;

            WispInABottle.SunPrefab = gameobject2;

            testActive.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);
            SynergyAPI.SynergyBuilder.AddItemToSynergy(testActive, CustomSynergyType.FLAIR_GUN);
            SynergyAPI.SynergyBuilder.AddItemToSynergy(testActive, CustomSynergyType.SUNLIGHT_SOUL);

            WispInABottle.WispInABottleID = testActive.PickupObjectId;
            ItemIDs.AddToList(testActive.PickupObjectId);

        }
        public static int WispInABottleID;

        public static GameObject SunPrefab;
        //public static List<int> spriteIds = new List<int>();
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        protected override void DoEffect(PlayerController user)
        {
            GameObject original;
            original = WispInABottle.SunPrefab;
            tk2dSprite component = GameObject.Instantiate(original, user.specRigidbody.UnitTopCenter, Quaternion.identity, user.transform).GetComponent<tk2dSprite>();
            component.transform.position.WithZ(transform.position.z + 99999);
            component.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(user.CenterPosition, tk2dBaseSprite.Anchor.MiddleCenter);
            user.sprite.AttachRenderer(component.GetComponent<tk2dBaseSprite>());
            component.PlaceAtPositionByAnchor(base.LastOwner.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
            component.scale = Vector3.one;
            WispInABottleController wispCont = component.gameObject.AddComponent<WispInABottleController>();
            wispCont.player = user;
            wispCont.self = component;
        }
       
    }
}

namespace Planetside
{
    public class WispInABottleController : MonoBehaviour
    {
        public WispInABottleController()
        {
            this.SpawnDuration = 1f;
            this.Duration = 10f;
            this.player = GameManager.Instance.PrimaryPlayer;
            this.self = WispInABottle.SunPrefab.GetComponent<tk2dBaseSprite>();
        }
        public void Start()
        {
            AkSoundEngine.PostEvent("Play_BOSS_lichB_charge_01", player.gameObject);
            self.GetComponent<tk2dSpriteAnimator>().PlayForDuration("spawn", SpawnDuration);

            Material sharedMaterial = self.sprite.renderer.sharedMaterial;
            self.sprite.usesOverrideMaterial = true;
            Material material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
            material.SetColor("_EmissiveColor", new Color32(255, 241, 204, 255));
            material.SetFloat("_EmissiveColorPower", 5f);
            material.SetFloat("_EmissivePower", 37.5f);
            self.sprite.renderer.material = material;

        }
        public void Update()
        {
            this.elapsed += BraveTime.DeltaTime;
            if (player == null)
            {
                ETGModConsole.Log("player is NULL");
            }
            if (self == null)
            {
                ETGModConsole.Log("object is NULL (how the hell?)");
            }
            if (this.elapsed >= SpawnDuration && this.elapsed <= SpawnDuration + Duration && DurationOver != true)
            {
                if (HasTriggeredFireAnim != true)
                {
                    HasTriggeredFireAnim = true;
                    self.GetComponent<tk2dSpriteAnimator>().Play("fire");
                    AkSoundEngine.PostEvent("Play_BOSS_doormimic_flame_01", player.gameObject);
                    AkSoundEngine.PostEvent("Play_Burn", player.gameObject);
                }
                this.secondaryElapsed += BraveTime.DeltaTime;
               
                if (secondaryElapsed >= 0.2f)
                {
                    secondaryElapsed = 0;
                    Exploder.DoDistortionWave(self.sprite.WorldCenter, 0.1f, 0.25f, 6, 0.5f);
                    List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    Vector2 centerPosition = player.sprite.WorldCenter;
                    if (activeEnemies != null)
                    {
                        for (int i = 0; i < activeEnemies.Count; i++)
                        {
                            AIActor aiactor = activeEnemies[i];
                            if (Vector2.Distance(aiactor.CenterPosition, centerPosition) < 20 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null)
                            {
                                aiactor.ApplyEffect(DebuffLibrary.HeatStroke, 1f, null);

                            }
                        }
                    }
                }
            }
            if (this.elapsed >= Duration + SpawnDuration && DurationOver != true)
            {
                DurationOver = true;
                AkSoundEngine.PostEvent("Stop_Burn", player.gameObject);
                AkSoundEngine.PostEvent("Play_BOSS_lichB_charge_01", player.gameObject);
                self.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("despawn");
            }
        }
        public void OnDestroy()
        {
            AkSoundEngine.PostEvent("Stop_Burn", player.gameObject);
        }

        public tk2dBaseSprite self;

        public PlayerController player;
        public float Duration;
        public float SpawnDuration;

        private bool DurationOver;
        private bool HasTriggeredFireAnim;
        

        private float elapsed;
        private float secondaryElapsed;
    }
}

