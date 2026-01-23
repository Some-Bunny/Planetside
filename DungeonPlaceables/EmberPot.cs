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
using Planetside.Controllers;

namespace Planetside
{
    public class EmberPot
    {
        public static void Init()
        {
            var EmberPot = PrefabBuilder.BuildObject("Ember Pot");
            EmberPot.layer = Layers.FG_Critical;
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
            breakable.goopType = FoolMode.isFoolish ? EasyGoopDefinitions.PoisonDef : EasyGoopDefinitions.FireDef;



            DebrisObject shardObject = BreakableAPI_Bundled.GenerateDebrisObject("emberpot_shard1", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 1, 5, 720, 540, null, 0.5f, null, null, 0, false);
            animator = shardObject.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("emberpot_shard");
            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[]
            {
                shardObject
            }, 0.35f, 1.2f, 6, 9, 0.8f);


            GameObject poofVFX = (PickupObjectDatabase.GetById(336) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;


        

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
