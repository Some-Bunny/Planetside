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
using Brave.BulletScript;
using System.Collections;
using Alexandria.PrefabAPI;
using Alexandria.cAPI;
using Planetside.Static_Storage;

namespace Planetside
{

    public class BoxOfGrenadesController : MonoBehaviour
    {
        public MinorBreakable self;
        public void Start()
        {
            self = base.gameObject.GetComponent<MinorBreakable>();
            if (self != null)
            {
                self.OnBreak += OnBreak;
            }
        }
        public void OnBreak()
        {
            AkSoundEngine.PostEvent("Play_CHR_pit_fall_01", base.gameObject);
            Vector2 SpawnPos = self.sprite.WorldCenter;
            GameObject vfx = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.DragunBoulderLandVFX, SpawnPos, Quaternion.identity);
            tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
            component.PlaceAtPositionByAnchor(SpawnPos, tk2dBaseSprite.Anchor.MiddleCenter);
            component.HeightOffGround = 35f;
            component.UpdateZDepth();
            Destroy(component.gameObject, 2.5f);

            tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
            if (component2 != null)
            {
                component.usesOverrideMaterial = true;
                component2.ignoreTimeScale = true;
                component2.AlwaysIgnoreTimeScale = true;
                component2.AnimateDuringBossIntros = true;
                component2.alwaysUpdateOffscreen = true;
                component2.playAutomatically = true;
            }
            SpawnManager.SpawnBulletScript(null, SpawnPos + new Vector2(0, 0.25f), self.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(SpewGrenades)), StringTableManager.GetEnemiesString("#TRAP", -1));
        }
        public class SpewGrenades : Script
        {
            public override IEnumerator Top()
            {
                for (int i = 0; i < 4; i++)
                {
                    base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("880bbe4ce1014740ba6b4e2ea521e49d").bulletBank.GetBullet("grenade"));
                    float airTime = base.BulletBank.GetBullet("grenade").BulletObject.GetComponent<ArcProjectile>().GetTimeInFlight();
                    Vector2 vector = base.BulletBank.transform.PositionVector2() + MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(1, 8));
                    Bullet bullet2 = new Bullet("grenade", false, false, false);
                    float direction2 = (vector - base.Position).ToAngle();
                    base.Fire(new Direction(direction2, DirectionType.Absolute, -1f), new Speed(1f, SpeedType.Absolute), bullet2);
                    (bullet2.Projectile as ArcProjectile).AdjustSpeedToHit(vector);
                    bullet2.Projectile.ImmuneToSustainedBlanks = true;
                }       
                yield break;
            }
        }
    }
    public class BoxOfGrenades
    {
        public static void Init()
        {
            /*
            string defaultPath = "Planetside/Resources/DungeonObjects/GrenadeBox/";
            string[] idlePaths = new string[]
            {
                defaultPath+"grenadebox.png",
            };
            */
            //string shadowPath = "Planetside/Resources/DungeonObjects/GrenadeBox/grenadeboxshadow.png";
            //MinorBreakable breakable = BreakableAPIToolbox.GenerateMinorBreakable("Box_Of_Grenades", idlePaths, 1, idlePaths, 10, "Play_OBJ_boulder_break_01", true, 28, 14, 0, -4);

            var grenadeBox = PrefabBuilder.BuildObject("Grenade Box");

            var sprite = grenadeBox.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "grenadebox1");

            sprite.usesOverrideMaterial = true;
            Material mat = new Material(StaticShaders.Default_Object_Shader);
            sprite.renderer.material = mat;

            grenadeBox.CreateFastBody(new IntVector2(32, 24), new IntVector2(0, 0), CollisionLayer.LowObstacle);
            grenadeBox.CreateFastBody(new IntVector2(32, 24), new IntVector2(0, 0), CollisionLayer.BulletBlocker);

            var animator = grenadeBox.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("grenadebox");

            var breakable = grenadeBox.AddComponent<MinorBreakable>();

            breakable.stopsBullets = false;
            breakable.OnlyPlayerProjectilesCanBreak = false;
            breakable.OnlyBreaksOnScreen = false;
            breakable.resistsExplosions = false;
            breakable.canSpawnFairy = false;
            breakable.chanceToRain = 1;
            breakable.dropCoins = false;
            breakable.goopsOnBreak = false;
            breakable.breakStyle = MinorBreakable.BreakStyle.BURST;
            breakable.gameObject.AddComponent<TresspassLightController>();
            breakable.IgnoredForPotShotsModifier = true;

            breakable.amountToRain = 0;
            breakable.EmitStyle = GlobalSparksDoer.EmitRegionStyle.RANDOM;
            breakable.ParticleColor = Color.white;
            breakable.ParticleLifespan = 2;
            breakable.ParticleMagnitude = 1;
            breakable.ParticleMagnitudeVariance = 1;
            breakable.ParticleSize = 0.1f;
            breakable.ParticleType = GlobalSparksDoer.SparksType.FLOATY_CHAFF;
            breakable.hasParticulates = true;
            breakable.MaxParticlesOnBurst = 10;
            breakable.MinParticlesOnBurst = 4;

            breakable.sprite.SortingOrder = 0;
            breakable.sprite.HeightOffGround = -1;
            breakable.gameObject.layer = LayerMask.NameToLayer("FG_Critical");



            breakable.breakAudioEventName = "Play_obj_box_break_01";

            breakable.stopsBullets = true;
            breakable.OnlyPlayerProjectilesCanBreak = false;
            breakable.OnlyBreaksOnScreen = false;
            breakable.resistsExplosions = false;
            breakable.canSpawnFairy = false;
            breakable.chanceToRain = 1;
            breakable.dropCoins = false;
            breakable.goopsOnBreak = false;
            breakable.breakStyle = MinorBreakable.BreakStyle.BURST;
            breakable.gameObject.AddComponent<BoxOfGrenadesController>();
            breakable.gameObject.AddComponent<AIBulletBank>();

            /*
            string shardDefaultPath = "Planetside/Resources/DungeonObjects/GrenadeBox/";
            string[] shardPaths = new string[]
            {
                shardDefaultPath+"WoodShards/shard1.png",
                shardDefaultPath+"WoodShards/shard2.png",
                shardDefaultPath+"WoodShards/shard3.png",
                shardDefaultPath+"WoodShards/shard4.png",
                shardDefaultPath+"WoodShards/shard5.png",
                shardDefaultPath+"WoodShards/shard6.png",
                shardDefaultPath+"WoodShards/shard7.png",
                shardDefaultPath+"WoodShards/shard8.png",
                shardDefaultPath+"WoodShards/shard9.png",
                shardDefaultPath+"WoodShards/shard10.png",
                shardDefaultPath+"WoodShards/shard11.png",
                shardDefaultPath+"WoodShards/shard12.png",
                shardDefaultPath+"WoodShards/shard13.png",
                shardDefaultPath+"WoodShards/shard14.png",

            };
            DebrisObject[] shardObjects = BreakableAPIToolbox.GenerateDebrisObjects(shardPaths, true, 1, 5, 720, 540, null, 0.9f, null, null, 0, false);
            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(shardObjects, 1f, 3f, 10, 18, 0.9f);
            */

            DebrisObject shardObjects = BreakableAPI_Bundled.GenerateDebrisObject("grenadeshard1", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 1, 5, 720, 540, null, 0.6f, null, null, 0, false);
            var animatorShard = shardObjects.AddComponent<tk2dSpriteAnimator>();
            animatorShard.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animatorShard.playAutomatically = true;
            animatorShard.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("grenadeboxshard");
            animatorShard.sprite.usesOverrideMaterial = true;
            mat = new Material(StaticShaders.Default_Object_Shader);
            animator.sprite.renderer.material = mat;
            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shardObjects }, 0.7f, 1.2f, 12, 18, 0.6f);

            ShardCluster[] array = new ShardCluster[] { potShardCluster };
            breakable.shardClusters = array;
            //breakable.OnBreak += OnBroken; //Code that runs when the breakable is broken. If doesnt have any arguments so im not sure how useful it can be


            var grenadeBoxShadow = PrefabBuilder.BuildObject("Grenade Box Shadow");
            sprite = grenadeBoxShadow.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "grenadeboxshadow");
            sprite.transform.localPosition = new Vector3(0, -.125f);
            sprite.IsPerpendicular = false;

            grenadeBoxShadow.gameObject.transform.SetParent(breakable.transform, false);   

            StaticReferences.StoredRoomObjects.Add("box_of_grenades", breakable.gameObject);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:box_of_grenades", breakable.gameObject);

            //StaticReferences.StoredRoomObjects.Add("test", breakable.gameObject);
        }
    }
}
