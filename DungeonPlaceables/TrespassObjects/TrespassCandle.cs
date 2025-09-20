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
using Alexandria.PrefabAPI;
using Alexandria.cAPI;

namespace Planetside
{
    public class TrespassCandle
    {
        public static void Init()
        {
            DebrisObject shardObjects = BreakableAPI_Bundled.GenerateDebrisObject("trespasscandle_debris_001", StaticSpriteDefinitions.Trespass_Room_Object_Data, true, 1, 5, 720, 540, null, 0.6f, null, null, 0, false);
            var animatorShard = shardObjects.AddComponent<tk2dSpriteAnimator>();
            animatorShard.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animatorShard.playAutomatically = true;
            animatorShard.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName("candleDebris");
            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shardObjects  }, 0.7f, 1.2f, 6, 9, 0.6f);  
            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { GenerateThing(potShardCluster, "candle_slow").gameObject, 0.7f },
                { GenerateThing(potShardCluster, "candle_medium").gameObject, 0.8f },
                { GenerateThing(potShardCluster, "candle_fast").gameObject, 0.6f },
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("tresPassCandle", placeable);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("PSOG_tresPassCandle", placeable);
        }

        public static MinorBreakable GenerateThing(ShardCluster shardCluster, string animation)
        {
            var newCandle = PrefabBuilder.BuildObject($"{animation}_candle");

            var sprite = newCandle.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.Trespass_Room_Object_Data, "trespasscandle_idle_001");
            sprite.IsPerpendicular = false;

            sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 8f);
            mat.SetFloat("_EmissivePower", 4);
            sprite.renderer.material = mat;

            newCandle.CreateFastBody(new IntVector2(13, 14), new IntVector2(1, 1), CollisionLayer.LowObstacle);
            newCandle.CreateFastBody(new IntVector2(13, 14), new IntVector2(1, 1), CollisionLayer.BulletBlocker);
            newCandle.CreateFastBody(new IntVector2(13, 14), new IntVector2(1, 1), CollisionLayer.BeamBlocker);

            var animator = newCandle.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName(animation);

            var breakable = newCandle.AddComponent<MinorBreakable>();

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
            ShardCluster[] array = new ShardCluster[] { shardCluster };
            breakable.IgnoredForPotShotsModifier = true;

            breakable.shardClusters = array;
            breakable.amountToRain = 30;
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


            return breakable;
        }
    }
}
