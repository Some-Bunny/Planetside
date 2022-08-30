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
            Vector2 SpawnPos = self.sprite.WorldCenter;
            GameObject vfx = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.DragunBoulderLandVFX, SpawnPos, Quaternion.identity);
            tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
            component.PlaceAtPositionByAnchor(SpawnPos, tk2dBaseSprite.Anchor.MiddleCenter);
            component.HeightOffGround = 35f;
            component.UpdateZDepth();
            Destroy(component.gameActor, 2.5f);

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
            protected override IEnumerator Top()
            {
                for (int i = 0; i < 4; i++)
                {
                    base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("880bbe4ce1014740ba6b4e2ea521e49d").bulletBank.GetBullet("grenade"));
                    float airTime = base.BulletBank.GetBullet("grenade").BulletObject.GetComponent<ArcProjectile>().GetTimeInFlight();
                    Vector2 vector = base.BulletBank.transform.PositionVector2() + MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(3, 9));
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
            string defaultPath = "Planetside/Resources/DungeonObjects/GrenadeBox/";
            string[] idlePaths = new string[]
            {
                defaultPath+"grenadebox.png",
            };
          
            string shadowPath = "Planetside/Resources/DungeonObjects/GrenadeBox/grenadeboxshadow.png";
            MinorBreakable breakable = BreakableAPIToolbox.GenerateMinorBreakable("Box_Of_Grenades", idlePaths, 1, idlePaths, 10, "Play_OBJ_boulder_break_01", shadowPath, 0, -0.125f, true, 28, 14, 0, -4);
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
            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(shardObjects, 1f, 2f, 10, 18, 0.9f);

 
            ShardCluster[] array = new ShardCluster[] { potShardCluster};
            breakable.shardClusters = array;
            //breakable.OnBreak += OnBroken; //Code that runs when the breakable is broken. If doesnt have any arguments so im not sure how useful it can be



      
            StaticReferences.StoredRoomObjects.Add("box_of_grenades", breakable.gameObject);

            //StaticReferences.StoredRoomObjects.Add("test", breakable.gameObject);
        }
    }
}
