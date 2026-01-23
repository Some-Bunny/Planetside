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
using System.ComponentModel;

namespace Planetside
{
    public class WispInABottle : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Wisp In A Bottle";
            //string resourceName = "Planetside/Resources/WispInABottle.png";
            GameObject obj = new GameObject(itemName);
            WispInABottle testActive = obj.AddComponent<WispInABottle>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("WispInABottle"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Unmatched Power";
            string longDesc = "A bottle that contains a whole living star inside of it.\n\nUse with caution.";
            testActive.SetupItem(shortDesc, longDesc, "psog");
            testActive.SetCooldownType(ItemBuilder.CooldownType.Damage, 900f);
            testActive.consumable = false;
            testActive.quality = PickupObject.ItemQuality.B;
            testActive.SetupUnlockOnCustomFlag(CustomDungeonFlags.DEFEAT_OPHANAIM, true);

            testActive.AddToSubShop(ItemAPI.ItemBuilder.ShopType.Goopton, 1);


            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Wisp Sun", debuffCollection.GetSpriteIdByName("sunflare_fire_001"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            var sprite = BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.VFX_Animation_Data;
            animator.DefaultClipId = animator.GetClipIdByName("sun_start");
            animator.playAutomatically = true;
            sprite.usesOverrideMaterial = true;
            Material material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            material.SetTexture("_MainTex", sprite.renderer.material.GetTexture("_MainTex"));
            material.SetColor("_EmissiveColor", new Color32(255, 241, 204, 255));
            material.SetFloat("_EmissiveColorPower", 5f);
            material.SetFloat("_EmissivePower", 37.5f);
            sprite.renderer.material = material;

            WispInABottleController wispCont = BrokenArmorVFXObject.gameObject.AddComponent<WispInABottleController>();
            wispCont.Sprite = sprite;
            wispCont.Animator = animator;



            WispInABottle.SunPrefab = BrokenArmorVFXObject;

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

        public override void DoEffect(PlayerController user)
        {
            tk2dSprite component = GameObject.Instantiate(WispInABottle.SunPrefab, user.specRigidbody.UnitTopCenter, Quaternion.identity, user.transform).GetComponent<tk2dSprite>();
            component.transform.position.WithZ(transform.position.z + 99999);
            component.PlaceAtPositionByAnchor(user.CenterPosition, tk2dBaseSprite.Anchor.MiddleCenter);
            
            user.sprite.AttachRenderer(component);
            
            component.PlaceAtPositionByAnchor(user.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
            component.scale = Vector3.one;
            component.GetComponent<WispInABottleController>().player = user;
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
        }
        public void Start()
        {
            AkSoundEngine.PostEvent("Play_BOSS_lichB_charge_01", player.gameObject);
            Animator.PlayForDuration("sun_start", SpawnDuration);

        }
        public void Update()
        {
            this.elapsed += BraveTime.DeltaTime;
            if (this.elapsed >= SpawnDuration && this.elapsed <= SpawnDuration + Duration && DurationOver != true)
            {
                if (HasTriggeredFireAnim != true)
                {
                    HasTriggeredFireAnim = true;
                    Animator.Play("sun_aaa");
                    AkSoundEngine.PostEvent("Play_BOSS_doormimic_flame_01", player.gameObject);
                    AkSoundEngine.PostEvent("Play_Burn", player.gameObject);
                }
                this.secondaryElapsed += BraveTime.DeltaTime;
               
                if (secondaryElapsed >= 0.2f)
                {
                    secondaryElapsed = 0;
                    Exploder.DoDistortionWave(Sprite.WorldCenter, 0.1f, 0.25f, 6, 0.5f);
                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = Sprite.WorldTopCenter,
                        startSize = 12,
                        rotation = 0,
                        startLifetime = 0.5f,
                        startColor = new Color(1, 0.6f, 0f, 0.1f)
                    });


                    if (player.CurrentRoom != null)
                    {
                        List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                        Vector2 centerPosition = player.sprite.WorldCenter;
                        if (activeEnemies != null)
                        {
                            for (int i = 0; i < activeEnemies.Count; i++)
                            {
                                AIActor aiactor = activeEnemies[i];
                                if (Vector2.Distance(aiactor.CenterPosition, centerPosition) < 20 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null)
                                {
                                    aiactor.ApplyEffect(DebuffLibrary.HeatStroke, 1f, null);
                                }
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
                Animator.PlayAndDestroyObject("sun_stop");
            }
        }
        public void OnDestroy()
        {
            AkSoundEngine.PostEvent("Stop_Burn", player.gameObject);
        }

        public tk2dBaseSprite Sprite;
        public tk2dSpriteAnimator Animator;


        public PlayerController player;


        public float Duration;
        public float SpawnDuration;

        private bool DurationOver;
        private bool HasTriggeredFireAnim;
        

        private float elapsed;
        private float secondaryElapsed;
    }
}

