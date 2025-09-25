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
using Planetside.Controllers.ContainmentBreach.BossChanges.Misc;
using Alexandria.PrefabAPI;
using Planetside.Static_Storage;

namespace Planetside
{
    public class EmberPot
    {
        public static void Init()
        {
            /*
            string defaultPath = "Planetside/Resources/DungeonObjects/EmberPot/";
            string[] idlePaths = new string[]
            {
                defaultPath+"Pot/emberpot_idle_001.png",
                defaultPath+"Pot/emberpot_idle_002.png",
                defaultPath+"Pot/emberpot_idle_003.png",
                defaultPath+"Pot/emberpot_idle_004.png"
            };
            string[] breakPaths = new string[]
            {
                defaultPath+"Pot/emberpot_break_001.png",
            };
            string shadowPath = "Planetside/Resources/DungeonObjects/EmberPot/emberpotshadow.png";
            MinorBreakable breakable = BreakableAPIToolbox.GenerateMinorBreakable("Ember_Pot", idlePaths, 2, breakPaths, 4, "Play_OBJ_pot_shatter_01", true, 13, 14, 1, 1);
            */

            //BreakableAPIToolbox.GenerateShadow(shadowPath, "emberpot_shadow", breakable.gameObject.transform, new Vector3(0, -0.125f));

            var EmberPot = PrefabBuilder.BuildObject("Ember Pot");
            EmberPot.layer = 20;
            var sprite = EmberPot.AddComponent<tk2dSprite>();
            var animator = EmberPot.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("emberpot_idle");
            sprite.usesOverrideMaterial = true;
            Material mat = new Material(StaticShaders.Default_Object_Shader);
            sprite.renderer.material = mat;
            EmberPot.CreateFastBody(new IntVector2(13, 15), new IntVector2(1, 1), CollisionLayer.PlayerBlocker);
            EmberPot.CreateFastBody(new IntVector2(13, 15), new IntVector2(1, 1), CollisionLayer.EnemyBlocker);
            EmberPot.CreateFastBody(new IntVector2(13, 15), new IntVector2(1, 1), CollisionLayer.BulletBlocker);
            EmberPot.CreateFastBody(new IntVector2(13, 15), new IntVector2(1, 1), CollisionLayer.BeamBlocker);
            var breakable = EmberPot.AddComponent<MinorBreakable>();
            sprite.IsPerpendicular = false;

            breakable.breakAnimName = "emberpot_break";
            breakable.stopsBullets = true;
            breakable.OnlyPlayerProjectilesCanBreak = false;
            breakable.OnlyBreaksOnScreen = false;
            breakable.resistsExplosions = false;
            breakable.canSpawnFairy = false;
            breakable.chanceToRain = 1;
            breakable.dropCoins = false;
            breakable.goopsOnBreak = true;
            breakable.goopRadius = 3f;
            breakable.goopType = EasyGoopDefinitions.FireDef;

            /*
            string shardDefaultPath = "Planetside/Resources/DungeonObjects/EmberPot/";
            string[] shardPaths = new string[]
            {
                shardDefaultPath+"Shards/emberpot_shard1.png",
                shardDefaultPath+"Shards/emberpot_shard2.png",
                shardDefaultPath+"Shards/emberpot_shard3.png",
                shardDefaultPath+"Shards/emberpot_shard4.png",
                shardDefaultPath+"Shards/emberpot_shard5.png",
                shardDefaultPath+"Shards/emberpot_shard6.png"
            };
            */
            //DebrisObject[] shardObjects = BreakableAPI_Bundled.GenerateDebrisObjects(shardPaths, StaticSpriteDefinitions.RoomObject_Animation_Data, true, 1, 5, 720, 540, null, 1.5f, null, null, 0, false);
            /*
            var EmberShard = PrefabBuilder.BuildObject("Ember Pot Shard");
            EmberShard.layer = 20;
            sprite = EmberShard.AddComponent<tk2dSprite>();
            animator = EmberShard.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("emberpot_idle");
            sprite.usesOverrideMaterial = true;
            mat = new Material(StaticShaders.Default_Shader);
            sprite.renderer.material = mat;


            */

            DebrisObject shardObject = BreakableAPI_Bundled.GenerateDebrisObject("emberpot_shard1", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 1, 5, 720, 540, null, 0.5f, null, null, 0, false);
            animator = shardObject.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("emberpot_shard");
            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[]
            {
                shardObject
            }, 0.35f, 1.2f, 6, 9, 0.8f);

            /*
            string emberDefPath = "Planetside/Resources/DungeonObjects/EmberPot/Embers/";
            string[] emberSmallPaths = new string[]
            {
                emberDefPath+"embersmall_001.png",
                emberDefPath+"embersmall_002.png",
                emberDefPath+"embersmall_003.png",
            };
            string[] emberTinyPaths = new string[]
            {
                emberDefPath+"embertiny_001.png",
                emberDefPath+"embertiny_002.png",
                emberDefPath+"embertiny_003.png",
            };
            string[] emberMediumPaths = new string[]
            {
                emberDefPath+"embermedium_001.png",
                emberDefPath+"embermedium_002.png",
                emberDefPath+"embermedium_003.png",
            };
            */
            GameObject poofVFX = (PickupObjectDatabase.GetById(336) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;


            /*
            DebrisObject smallEmber = BreakableAPIToolbox.GenerateAnimatedDebrisObject(emberSmallPaths, 6, tk2dSpriteAnimationClip.WrapMode.Loop, true, 0.5f, 3, 1080, 540, null, 0.7f, null, poofVFX, 0, false,null, 0.7f);
            DebrisObject tinyEmber = BreakableAPIToolbox.GenerateAnimatedDebrisObject(emberTinyPaths, 7, tk2dSpriteAnimationClip.WrapMode.Loop, true, 0.4f, 2, 1600, 800, null, 0.5f, null, poofVFX, 0, false, null, 0.5f);
            DebrisObject mediumEmber = BreakableAPIToolbox.GenerateAnimatedDebrisObject(emberMediumPaths, 5, tk2dSpriteAnimationClip.WrapMode.Loop, true, 0.5f, 3, 1080, 540, null, 0.7f, null, poofVFX, 0, false, null, 0.9f);
            DebrisObject smallEmberTwo = BreakableAPIToolbox.GenerateAnimatedDebrisObject(emberSmallPaths, 7, tk2dSpriteAnimationClip.WrapMode.Loop, true, 0.35f, 2.5f, 900, 450, null, 0.8f, null, poofVFX, 0, false, null, 0.9f);
            DebrisObject tinyEmberTwo = BreakableAPIToolbox.GenerateAnimatedDebrisObject(emberTinyPaths, 6, tk2dSpriteAnimationClip.WrapMode.Loop, true, 0.4f, 2, 1300, 650, null, 0.5f, null, poofVFX, 0, false, null, 0.5f);
            DebrisObject mediumEmberTwo = BreakableAPIToolbox.GenerateAnimatedDebrisObject(emberMediumPaths, 4, tk2dSpriteAnimationClip.WrapMode.Loop, true, 0.7f, 4, 1500, 600, null, 0.8f, null, poofVFX, 0, false, null, 1f);
            ShardCluster emberClusterOne = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { smallEmber, tinyEmber, mediumEmber, smallEmberTwo, tinyEmberTwo, mediumEmberTwo }, 1f, 1.5f, 4, 7, 0.8f);
            ShardCluster emberClusterTwo = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { smallEmber, tinyEmber, mediumEmber, smallEmberTwo, tinyEmberTwo, mediumEmberTwo }, 1f, 2f, 4, 7, 1f);
            ShardCluster emberClusterThree = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { smallEmber, tinyEmber, mediumEmber, smallEmberTwo, tinyEmberTwo, mediumEmberTwo }, 1f, 2.5f, 4, 7, 1.2f);
            */

            ShardCluster emberClusterOne = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] 
            {
                GenerateFastEmber("Ember_1", "embersmall_001", "ember_small"),
                GenerateFastEmber("Ember_2", "embertiny_001", "ember_tiny"),
                GenerateFastEmber("Ember_3", "embermedium_001", "ember_medium"),

            }, 1f, 1.5f, 13, 22, 0.8f);

            //SND_OBJ_pot_shatter_01
            EnemyToolbox.AddSoundsToAnimationFrame(breakable.spriteAnimator, "emberpot_break", new Dictionary<int, string> { { 0, "Play_OBJ_pot_shatter_01" } });

            ShardCluster[] array = new ShardCluster[] { potShardCluster, emberClusterOne};
            breakable.shardClusters = array;
            //breakable.OnBreak += OnBroken; //Code that runs when the breakable is broken. If doesnt have any arguments so im not sure how useful it can be
            breakable.amountToRain = 30;
            breakable.EmitStyle = GlobalSparksDoer.EmitRegionStyle.RANDOM;
            breakable.ParticleColor = Color.red;
            breakable.ParticleLifespan = 2;
            breakable.breakStyle = MinorBreakable.BreakStyle.BURST;
            //Attach ParticleSystem component to set particles
            breakable.ParticleMagnitude = 1;
            breakable.ParticleMagnitudeVariance = 1;
            breakable.ParticleSize = 0.1f;
            breakable.ParticleType = GlobalSparksDoer.SparksType.EMBERS_SWIRLING;
            breakable.hasParticulates = true;
            breakable.MaxParticlesOnBurst = 10;
            breakable.MinParticlesOnBurst = 4;
            breakable.gameObject.AddComponent<TheGames.Marker>();

            var grenadeBoxShadow = PrefabBuilder.BuildObject("Ember Pot Shadow");
            sprite = grenadeBoxShadow.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "emberpotshadow");
            sprite.transform.localPosition = new Vector3(0, -.125f);
            sprite.IsPerpendicular = false;
            sprite.HeightOffGround = breakable.sprite.HeightOffGround - 0.5f;
            grenadeBoxShadow.gameObject.transform.SetParent(breakable.transform, false);

            /*
            string defaultTablePath = "Planetside/Resources/DungeonObjects/megaTable/";
            string[] outlinePaths = new string[]
            {
                defaultTablePath+"megatable_outlineNorth_001.png",
                defaultTablePath+"megatable_outlineEast_001.png",
                defaultTablePath+"megatable_outlineWest_001.png",
                defaultTablePath+"megatable_outlineSouth_001.png"
            };
            string[] northFlipPaths = new string[]
            {
                defaultTablePath+"megatable_northflip_001.png",
                defaultTablePath+"megatable_northflip_002.png",
                defaultTablePath+"megatable_northflip_003.png",
                defaultTablePath+"megatable_northflip_004.png",
                defaultTablePath+"megatable_northflip_005.png",
                defaultTablePath+"megatable_northflip_006.png",
            };
            string[] southFlipPaths = new string[]
            {
                defaultTablePath+"megatable_southflip_001.png",
                defaultTablePath+"megatable_southflip_002.png",
                defaultTablePath+"megatable_southflip_003.png",
                defaultTablePath+"megatable_southflip_004.png",
                defaultTablePath+"megatable_southflip_005.png",
                defaultTablePath+"megatable_southflip_006.png",
            };
            string[] northBreakPaths = new string[]
            {
                defaultTablePath+"megatable_northflipbreakdown_001.png",
                defaultTablePath+"megatable_northflipbreakdown_002.png",
                defaultTablePath+"megatable_northflipbreakdown_001.png",
            };
            string[] southBreakPaths = new string[]
            {
                defaultTablePath+"megatable_southflipbreakdown_001.png",
                defaultTablePath+"megatable_southflipbreakdown_002.png",
                defaultTablePath+"megatable_southflipbreakdown_003.png",
            };
            string[] unflippedBreakPaths = new string[]
            {
                defaultTablePath+"megatable_idlebreakdown_001.png",
                defaultTablePath+"megatable_idlebreakdown_002.png",
                defaultTablePath+"megatable_idlebreakdown_003.png",
            };

            Dictionary<float, string> north = new Dictionary<float, string>()
            {
                {25, defaultPath+"megatable_flipbreak_001_north.png"}
            };
            Dictionary<float, string> south = new Dictionary<float, string>()
            {
                {25, defaultPath+"megatable_flipbreak_001_south.png"}
            };


            Dictionary<float, string> aTwo = new Dictionary<float, string>()
            {
                {25, defaultPath+"megatable_idlebreak_001.png"}
            };
            FlippableCover table = BreakableAPIToolbox.GenerateTable("Mega_Table", new string[] { defaultPath + "megatable_idle_001.png" }, outlinePaths, northFlipPaths, southFlipPaths, null, null, northBreakPaths, southBreakPaths, null, null, unflippedBreakPaths, 1, 10, 7, 7, true, 64, 30, 0, 8, 64, 5, 64, 5, FlippableCover.FlipStyle.ONLY_FLIPS_UP_DOWN, 300, null, north, south, null, null, aTwo, true, true, 1);
            */

            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { breakable.gameObject, 0.5f },
            };

            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("emberPot", placeable);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("psog:emberPot", placeable);

            //StaticReferences.StoredRoomObjects.Add("test", breakable.gameObject);
        }

        private static DebrisObject GenerateFastEmber(string Name, string SpriteName, string AnimationName)
        {
            DebrisObject shardObject = BreakableAPI_Bundled.GenerateDebrisObject(SpriteName, StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 1.5f, 5, 720, 540, null, 0.7f, null, null, 0, false);
            shardObject.gameObject.name = Name;
            var animator = shardObject.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName(AnimationName);
            return shardObject;
        }

    }
}
