using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dungeonator;
using ItemAPI;
using UnityEngine;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Planetside;
using BreakAbleAPI;
using System.Collections;

namespace Planetside
{
    public class HolyChasmberShrineController : MonoBehaviour
    {
        public void Start()
        {
            self = base.gameObject.GetComponent<MajorBreakable>();
            if (base.gameObject != null)
            {
                self.InvulnerableToEnemyBullets = true;
                if (self != null)
                {
                    self.OnBreak += WhenBroken;
                }
            }
        }

        public void WhenBroken()
        {
            AkSoundEngine.PostEvent("Play_RockBreaking", self.gameObject);
            PlayerController player = GameManager.Instance.BestActivePlayer;
            if (player.GetComponent<HERETIC>() != null)
            {
                AkSoundEngine.PostEvent("Play_BOSS_lichB_grab_01", base.gameObject);
                GameObject hand = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.hellDragController.HellDragVFX);
                tk2dBaseSprite component1 = hand.GetComponent<tk2dBaseSprite>();
                component1.usesOverrideMaterial = true;
                component1.PlaceAtLocalPositionByAnchor(player.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.LowerCenter);
                component1.renderer.material.shader = ShaderCache.Acquire("Brave/Effects/StencilMasked");
                Pixelator.Instance.FadeToBlack(0.5f, false, 0f);
                base.StartCoroutine(this.HandleGrabbyGrab(player));
            }
            else
            {
                string header = "DEFILER!";
                string text = "The Gods Have Been Angered.";
                if (GameManager.Instance.PrimaryPlayer.HasPickupID(ETGMod.Databases.Items["Diamond Chamber"].PickupObjectId) || ((GameManager.Instance.PrimaryPlayer.HasPickupID(ETGMod.Databases.Items["Netherite Chamber"].PickupObjectId))))
                {
                    header = "YOU ARE FORGIVEN.";
                    text = "FOR NOW.";
                }
                else
                {
                    AkSoundEngine.PostEvent("Play_BOSS_lichB_intro_01", base.gameObject);
                    AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("jammed_guardian");
                    IntVector2 bestRewardLocation = GameManager.Instance.BestActivePlayer.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
                    AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, bestRewardLocation, GameManager.Instance.BestActivePlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Spawn, true);
                    PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
                    aiactor.IgnoreForRoomClear = true;
                    aiactor.HandleReinforcementFallIntoRoom(0f);
                }
                HolyChasmberShrineController.Notify(header, text);
            }
            GameManager.Instance.StartCoroutine(ShrineParticlesOnDestory(self.sprite.WorldBottomLeft, self.sprite.WorldTopRight));
        }
        private IEnumerator HandleGrabbyGrab(PlayerController grabbedPlayer)
        {
            Pixelator.Instance.FadeToBlack(0.5f, false, 0f);
            GameManager.Instance.LoadCustomLevel("tt_bullethell");
            yield break;
        }
        private static void Notify(string header, string text)
        {
            tk2dSpriteCollectionData encounterIconCollection = AmmonomiconController.Instance.EncounterIconCollection;
            int spriteIdByName = encounterIconCollection.GetSpriteIdByName("Planetside/Resources/shelltansblessing.png");
            GameUIRoot.Instance.notificationController.DoCustomNotification(header, text, null, spriteIdByName, UINotificationController.NotificationColor.SILVER, false, true);
        }
        private IEnumerator ShrineParticlesOnDestory(Vector2 BottmLeft, Vector2 TopRight)
        {
            Vector2 lol = TopRight.RoundToInt();
            float yes = lol.x.RoundToNearest(1);
            for (int j = 0; j < (300 - yes) / 4 ; j++)
            {
                Vector2 pos = BraveUtility.RandomVector2(BottmLeft, TopRight, new Vector2(0.025f, 0.025f));
                GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), pos, Quaternion.identity);
                tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
                component.PlaceAtPositionByAnchor(pos, tk2dBaseSprite.Anchor.MiddleCenter);
                component.HeightOffGround = 35f;
                component.UpdateZDepth();
                tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                if (component2 != null)
                {
                    component.scale *= UnityEngine.Random.Range(0.5f, 1.25f);
                    component2.ignoreTimeScale = true;
                    component2.AlwaysIgnoreTimeScale = true;
                    component2.AnimateDuringBossIntros = true;
                    component2.alwaysUpdateOffscreen = true;
                    component2.playAutomatically = true;
                    component2.sprite.usesOverrideMaterial = true;
                    component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                    component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    component2.sprite.renderer.material.SetFloat("_EmissivePower", 1);
                    component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.1f);
                    component2.sprite.renderer.material.SetColor("_OverrideColor", Color.gray);
                    component2.sprite.renderer.material.SetColor("_EmissiveColor", Color.gray);
                }
                yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.01f));
            }
            yield break;
        }
        public MajorBreakable self;
    }

    public class HolyChamberStatue
    {
        public static void Init()
        {
            /*
            string defaultPath = "Planetside/Resources/DungeonObjects/HolyChamberStatue/";
            string[] idlePaths = new string[]
            {
                defaultPath+"kalipedestal1.png",
            };
            Dictionary<float, string> prebreaks = new Dictionary<float, string>()
            {
                {99, defaultPath+"kalipedestal_prebreak1.png"},
                {60, defaultPath+"kalipedestal_prebreak2.png"},
                {25, defaultPath+"kalipedestal_prebreak3.png"},
            };
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("holy_statue", idlePaths, 1, null, 1, 75, true, 32, 32, 0 ,-4, true, null, null, true, null, prebreaks);
            BreakableAPIToolbox.GenerateShadow(defaultPath + "kalipedestalshadow.png", "pedestal_shadow", statue.gameObject.transform, new Vector3(0, -0.1875f));
            */
            var KaliStatue = Alexandria.PrefabAPI.PrefabBuilder.BuildObject("Kali Statue");
            var sprite = KaliStatue.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "kalipedestal1");


            var majorBreakable = KaliStatue.AddComponent<MajorBreakable>();
            majorBreakable.sprite = sprite;

            KaliStatue.CreateFastBody(new IntVector2(32, 40), new IntVector2(4, -4), CollisionLayer.HighObstacle);
            KaliStatue.CreateFastBody(new IntVector2(32, 40), new IntVector2(4, -4), CollisionLayer.BeamBlocker);
            KaliStatue.CreateFastBody(new IntVector2(32, 40), new IntVector2(4, -4), CollisionLayer.BulletBlocker);
            KaliStatue.CreateFastBody(new IntVector2(32, 40), new IntVector2(4, -4), CollisionLayer.EnemyBlocker);
            KaliStatue.CreateFastBody(new IntVector2(32, 40), new IntVector2(4, -4), CollisionLayer.PlayerBlocker);

            sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 242, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 70);
            sprite.renderer.material = mat;

            majorBreakable.gameObject.AddComponent<PushImmunity>();


            majorBreakable.InvulnerableToEnemyBullets = true;

            majorBreakable.minShardPercentSpeed = 0.5f;
            majorBreakable.maxShardPercentSpeed = 1.4f;

            majorBreakable.destroyedOnBreak = true;
            majorBreakable.handlesOwnPrebreakFrames = true;


            majorBreakable.prebreakFrames = new BreakFrame[]
            {
                new BreakFrame{healthPercentage = 99, sprite = "kalipedestal_prebreak1" },
                new BreakFrame{healthPercentage = 60, sprite = "kalipedestal_prebreak2" },
                new BreakFrame{healthPercentage = 30, sprite = "kalipedestal_prebreak3" }
            };

            DebrisObject shard_1 = BreakableAPI_Bundled.GenerateDebrisObject("emberpot_shard1", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 1, 5, 240, 60, null, 1.5f, null, null, 0, false);
            var animator = shard_1.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("kalistatue_1");

            DebrisObject shard_2 = BreakableAPI_Bundled.GenerateDebrisObject("emberpot_shard1", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 1, 5, 240, 60, null, 1.5f, null, null, 0, false);
            animator = shard_2.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("kalistatue_2");

            DebrisObject shard_3 = BreakableAPI_Bundled.GenerateDebrisObject("emberpot_shard1", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 1, 5, 240, 60, null, 1.5f, null, null, 0, false);
            animator = shard_3.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("kalistatue_3");

            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[]
            {
                shard_1,
                shard_2,
                shard_3
            }, 0.35f, 1.2f, 12, 22, 0.8f);


            /*
            string shardDefaultPath = "Planetside/Resources/DungeonObjects/HolyChamberStatue/Shards/";
            string[] shardPathsTop = new string[]
            {
                shardDefaultPath+"statueshard1.png",
                shardDefaultPath+"statueshard2.png",
                shardDefaultPath+"statueshard3.png",
                shardDefaultPath+"statueshard4.png",
                shardDefaultPath+"statueshard5.png",
            };
            string[] shardPathsGlowPart = new string[]
            {
                shardDefaultPath+"statueshardglow1.png",
                shardDefaultPath+"statueshardglow2.png",
                shardDefaultPath+"statueshardglow3.png",
                shardDefaultPath+"statueshardglow4.png",
                shardDefaultPath+"statueshardglow5.png",
                shardDefaultPath+"statueshardglow6.png",
                shardDefaultPath+"statueshardglow7.png",
            };
            string[] shardPathsGlowPedestal = new string[]
            {
                shardDefaultPath+"statueshardpedestal1.png",
                shardDefaultPath+"statueshardpedestal2.png",
                shardDefaultPath+"statueshardpedestal3.png",
                shardDefaultPath+"statueshardpedestal4.png",
                shardDefaultPath+"statueshardpedestal5.png",
                shardDefaultPath+"statueshardpedestal6.png",
                shardDefaultPath+"statueshardpedestal7.png",
                shardDefaultPath+"statueshardpedestal8.png",
            };
            DebrisObject[] shardObjectsTop = BreakableAPIToolbox.GenerateDebrisObjects(shardPathsTop, true, 1, 5, 720, 300, null, 2f, "Play_OBJ_rock_break_01", null, 1, false);
            DebrisObject[] shardObjectsPart = BreakableAPIToolbox.GenerateDebrisObjects(shardPathsGlowPart, true, 1, 5, 540, 180, null, 4f, "Play_OBJ_rock_break_01", null, 1, false);
            DebrisObject[] shardObjectsPedestal = BreakableAPIToolbox.GenerateDebrisObjects(shardPathsGlowPedestal, true, 1, 5, 480, 240, null, 3f, "Play_OBJ_rock_break_01", null, 0, false);

            ShardCluster shardObjectsTopCluster = BreakableAPIToolbox.GenerateShardCluster(shardObjectsTop, 0.2f, 1.1f, 1, 2, 1.4f);
            ShardCluster shardObjectsTopCluster1 = BreakableAPIToolbox.GenerateShardCluster(shardObjectsTop, 0.4f, 0.6f, 2, 4, 1f);
            ShardCluster shardObjectsTopCluster2 = BreakableAPIToolbox.GenerateShardCluster(shardObjectsTop, 0.9f, 0.1f, 1, 2, 0.8f);


            ShardCluster shardObjectsPartCluster = BreakableAPIToolbox.GenerateShardCluster(shardObjectsPart, 0.1f, 1f, 2, 4, 1f);
            ShardCluster shardObjectsPartCluster1 = BreakableAPIToolbox.GenerateShardCluster(shardObjectsPart, 0.5f, 0.7f, 2,4, 1.3f);
            ShardCluster shardObjectsPartCluster2 = BreakableAPIToolbox.GenerateShardCluster(shardObjectsPart, 1f, 0.2f, 2, 4, 1.7f);

            ShardCluster shardObjectsCluster = BreakableAPIToolbox.GenerateShardCluster(shardObjectsPedestal, 0.1f, 1f, 1, 3, 0.8f);
            ShardCluster shardObjectsCluster1 = BreakableAPIToolbox.GenerateShardCluster(shardObjectsPedestal, 0.4f, 0.6f, 1, 3, 1f);
            ShardCluster shardObjectsCluster2 = BreakableAPIToolbox.GenerateShardCluster(shardObjectsPedestal, 0.9f, 0.2f, 1, 3, 1.4f);
            */



            ShardCluster[] array = new ShardCluster[] { potShardCluster };
            majorBreakable.shardClusters = array;
            majorBreakable.gameObject.GetOrAddComponent<HolyChasmberShrineController>();

            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { majorBreakable.gameObject, 0.5f },
            };

            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("holychamberstatue", placeable);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:holychamberstatue", majorBreakable.gameObject);

        }
    }
}
