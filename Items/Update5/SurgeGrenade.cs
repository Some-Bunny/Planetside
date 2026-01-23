using ItemAPI;
using Planetside;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Planetside
{
    class SurgeGrenade : SpawnObjectPlayerItem
    {
        public static void Init()
        {
            string itemName = "Surge Grenade";

            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<SurgeGrenade>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("surge_grenade"), data, obj);
            string shortDesc = "Better Than Carpets";
            string longDesc = "A breakthrough in climate control technology, this grenade is capable of building up massive amounts of positive charge to release as a lightning strike.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 250);
            item.consumable = false;

            lightningData = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericLargeExplosion);
            lightningData.effect = EnemyDatabase.GetOrLoadByGuid("dc3cd41623d447aeba77c77c99598426").GetComponent<BossFinalMarineDeathController>().bigExplosionVfx[0];//(PickupObjectDatabase.GetById(328) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.overrideMidairDeathVFX;
            lightningData.damage = 125;
            lightningData.damageRadius = 3;

            item.objectToSpawn = BuildPrefab();

            item.tossForce = 7;
            item.canBounce = true;

            item.IsCigarettes = false;
            item.RequireEnemiesInRoom = false;

            item.SpawnRadialCopies = false;
            item.RadialCopiesToSpawn = 0;

            //item.AudioEvent = "Play_OBJ_bomb_fuse_01";
            item.IsKageBunshinItem = false;

            item.quality = PickupObject.ItemQuality.C;


            SurgeGrenadeID = item.PickupObjectId;
        }
        public static int SurgeGrenadeID;
        public static ExplosionData lightningData;

        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_toss_001.png", new GameObject("SurgeGrenade"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_toss_001.png", collection),
                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_toss_002.png", collection),
                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_toss_003.png", collection),
                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_toss_004.png", collection),
                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_toss_005.png", collection),
                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_toss_006.png", collection),
                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_toss_007.png", collection),
                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_toss_008.png", collection),
                

            }, "throw", tk2dSpriteAnimationClip.WrapMode.Loop);
            deployAnimation.fps = 16;
            deployAnimation.loopStart = 0;

            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_boom_001.png", collection),

            }, "explode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_primed_001.png", collection),
                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_primed_002.png", collection),
                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_primed_003.png", collection),
                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Tossables/SurgeGrenade/surge_grenade_primed_004.png", collection),
            }, "primed", tk2dSpriteAnimationClip.WrapMode.Once);
            armedAnimation.fps = 3f;
            armedAnimation.frames[1].eventInfo = "Charge";
            armedAnimation.frames[1].triggerEvent = true;
            foreach (var frame in armedAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var audioListener = bomb.AddComponent<AudioAnimatorListener>();
            audioListener.animationAudioEvents = new ActorAudioEvent[]
            {
                new ActorAudioEvent
                {
                    eventName = "Play_BOSS_omegaBeam_charge_01",
                    eventTag = "Charge"
                }
            };

            ProximityMine proximityMine = new ProximityMine
            {
                explosionData = lightningData,
                explosionStyle = ProximityMine.ExplosiveStyle.TIMED,
                detectRadius = 3f,
                explosionDelay = 0.5f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0f,
                deployAnimName = "throw",
                explodeAnimName = "primed",
                idleAnimName = "primed",
                
                MovesTowardEnemies = false,
                HomingTriggeredOnSynergy = false,
                HomingDelay = 3.25f,
                HomingRadius = 10,
                HomingSpeed = 4,

            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);
            bomb.AddComponent<SurgeGrenadeController>();

            return bomb;
        }

        public class SurgeGrenadeController : MonoBehaviour
        {
            public void OnDestroy()
            {
                var spr = this.GetComponent<tk2dSpriteAnimator>();
                LightningController c = new LightningController();
                c.MajorNodesCount = UnityEngine.Random.Range(4, 7);
                c.OnPostStrike += (obj) =>
                {
                    AkSoundEngine.PostEvent("Play_Lightning", GameManager.Instance.BestActivePlayer.gameObject);

                    var lightObj = UnityEngine.Object.Instantiate(LightningMaker.lightObject);
                    lightObj.transform.position = obj;
                    Destroy(lightObj, 0.25f);

                    for (int i = 0; i < CoinTosser.AllActiveCoins.Count; i++)
                    {
                        var coinLocal = CoinTosser.AllActiveCoins[i];
                        if (coinLocal != null)

                        {
                            var coinSprite = coinLocal.GetComponentInChildren<tk2dBaseSprite>();
                            if (Vector2.Distance(coinSprite.WorldCenter, obj) < 3.5f)
                            {
                                float angle = BraveUtility.RandomAngle();

                                for (int e = 0; e < 3; e++)
                                {
                                    LightningController c2 = new LightningController();
                                    c2.MajorNodeMaxAngleSpacing = 25;
                                    c2.MajorNodeMinAngleSpacing = 5;
                                    c2.MajorNodesCount = UnityEngine.Random.Range(2, 4);
                                    c2.MajorNodeSplitoffChance = 0;

                                    c2.OnPostStrike += (objLocal) =>
                                    {

                                        var lightObjlocal = UnityEngine.Object.Instantiate(LightningMaker.lightObject);
                                        lightObjlocal.transform.position = objLocal;
                                        Destroy(lightObjlocal, 0.25f);


                                        Exploder.Explode(objLocal, lightningData, objLocal);
                                        AkSoundEngine.PostEvent("Play_Lightning", GameManager.Instance.BestActivePlayer.gameObject);

                                    };

                                    c2.LightningPreDelay = 0f;
                                    int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.EnemyHitBox, CollisionLayer.BulletBreakable);

                                    var result = RaycastToolbox.ReturnRaycast(coinSprite.WorldCenter, MathToolbox.GetUnitOnCircle(angle + (120* e), 1), rayMask);
                                    //ETGModConsole.Log(result.Distance);
                                    //ETGModConsole.Log(result.Contact);
                                    //ETGModConsole.Log(coinSprite.WorldCenter);


                                    c2.GenerateLightning(coinSprite.WorldCenter, coinSprite.WorldCenter + MathToolbox.GetUnitOnCircle(angle + (120* e), result.Distance));
                                    RaycastResult.Pool.Free(ref result);

                                }
                                AkSoundEngine.PostEvent("Play_perfectshot", GameManager.Instance.BestActivePlayer.gameObject);

                                var coinProj = coinLocal.GetComponent<Projectile>();

                                coinProj.DieInAir();
                            }


                        }
                    }
                };
                c.LightningPreDelay = 0f;
                c.GenerateLightning(spr.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(-9, 9), 16), spr.sprite.WorldCenter);
            }
        }

       
        public static IEnumerator Wavy(Vector2 pos)
        {
            float elapsed = 0;
            var lightObj = UnityEngine.Object.Instantiate(LightningMaker.lightObject);
            AdditionalBraveLight braveLight = lightObj.GetComponent<AdditionalBraveLight>();
            lightObj.transform.position = pos;

            braveLight.LightIntensity = 0;
            braveLight.LightRadius = 0;
            while (elapsed < 2f)
            {
                elapsed += BraveTime.DeltaTime;
                float t = Mathf.Min((elapsed / 1.5f), 1);
                braveLight.LightIntensity = Mathf.Lerp(0, 10, t);
                braveLight.LightRadius = Mathf.Lerp(0, 4, t);
                GlobalSparksDoer.DoSingleParticle(pos + MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(0, Mathf.Lerp(0, 3, t))), Vector3.up * 3, null, null, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                yield return null;
            }
            Destroy(lightObj);
            yield break;
        }
    }
}