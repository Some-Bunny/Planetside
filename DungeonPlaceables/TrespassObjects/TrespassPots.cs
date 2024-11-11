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

namespace Planetside
{
    public class TrespassPot
    {
        public static void Init()
        {

            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassPots/";
            string[] shardPaths = new string[]
            {
                defaultPath+"Shards/shard1.png",
                defaultPath+"Shards/shard2.png",
                defaultPath+"Shards/shard3.png",
                defaultPath+"Shards/shard4.png",
                defaultPath+"Shards/shard5.png",
                defaultPath+"Shards/shard6.png",
                defaultPath+"Shards/shard7.png",
                defaultPath+"Shards/shard8.png",
                defaultPath+"Shards/shard9.png",
            };
            DebrisObject[] shardObjects = BreakableAPIToolbox.GenerateDebrisObjects(shardPaths, true, 1, 5, 720, 540, null, 0.6f, null, null, 0, false);
            for (int e = 0; e < shardObjects.Length; e++)
            {
                shardObjects[e].gameObject.AddComponent<TresspassUnlitShaderController>();
            }

            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(shardObjects, 0.7f, 1.2f, 6, 9, 0.6f);
            string shadowPath = "Planetside/Resources/DungeonObjects/EmberPot/emberpotshadow.png";
            MinorBreakable breakable = BreakableAPIToolbox.GenerateMinorBreakable("TrespassPot1", new string[] {defaultPath+ "trespasspot1_idle.png" }, 2, new string[] { defaultPath + "trespasspot1_break.png" }, 10, "Play_OBJ_pot_shatter_01", true, 13, 14, 1, 1);
            BreakableAPIToolbox.GenerateShadow(shadowPath, "trespassPot_shadow", breakable.gameObject.transform, new Vector3(0, -0.125f));

            breakable.stopsBullets = true;
            breakable.OnlyPlayerProjectilesCanBreak = false;
            breakable.OnlyBreaksOnScreen = false;
            breakable.resistsExplosions = false;
            breakable.canSpawnFairy = false;
            breakable.chanceToRain = 1;
            breakable.dropCoins = false;
            breakable.goopsOnBreak = false;
            breakable.breakStyle = MinorBreakable.BreakStyle.BURST;
            breakable.gameObject.AddComponent<TresspassUnlitShaderController>();
            breakable.gameObject.AddComponent<TheGames.Marker>();

            ShardCluster[] array = new ShardCluster[] { potShardCluster };
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

            MinorBreakable breakable2 = BreakableAPIToolbox.GenerateMinorBreakable("TrespassPot2", new string[] { defaultPath + "trespasspot2_idle.png" }, 2, new string[] { defaultPath + "trespasspot2_break.png" }, 10, "Play_OBJ_pot_shatter_01", true, 13, 14, 1, 1);
            BreakableAPIToolbox.GenerateShadow(shadowPath, "trespassPot_shadow", breakable2.gameObject.transform, new Vector3(0, -0.125f));

            breakable2.stopsBullets = true;
            breakable2.OnlyPlayerProjectilesCanBreak = false;
            breakable2.OnlyBreaksOnScreen = false;
            breakable2.resistsExplosions = false;
            breakable2.canSpawnFairy = false;
            breakable2.chanceToRain = 1;
            breakable2.dropCoins = false;
            breakable2.goopsOnBreak = false;
            breakable2.breakStyle = MinorBreakable.BreakStyle.BURST;
            breakable2.shardClusters = array;
            breakable2.amountToRain = 30;
            breakable2.EmitStyle = GlobalSparksDoer.EmitRegionStyle.RANDOM;
            breakable2.ParticleColor = Color.white;
            breakable2.ParticleLifespan = 2;
            breakable2.ParticleMagnitude = 1;
            breakable2.ParticleMagnitudeVariance = 1;
            breakable2.ParticleSize = 0.1f;
            breakable2.ParticleType = GlobalSparksDoer.SparksType.FLOATY_CHAFF;
            breakable2.hasParticulates = true;
            breakable2.MaxParticlesOnBurst = 10;
            breakable2.MinParticlesOnBurst = 4;
            breakable2.gameObject.AddComponent<TresspassUnlitShaderController>();
            breakable2.gameObject.AddComponent<TheGames.Marker>();

            breakable2.sprite.SortingOrder = 0;
            breakable2.sprite.HeightOffGround = -1;
            breakable2.gameObject.layer = LayerMask.NameToLayer("FG_Critical");



            MinorBreakable breakable3 = BreakableAPIToolbox.GenerateMinorBreakable("TrespassPot3", new string[] { defaultPath + "trespasspot3_idle.png" }, 2, new string[] { defaultPath + "trespasspot3_break.png" }, 10, "Play_OBJ_pot_shatter_01", true, 13, 14, 1, 1);
            BreakableAPIToolbox.GenerateShadow(shadowPath, "trespassPot_shadow", breakable3.gameObject.transform, new Vector3(0, -0.125f));

            breakable3.stopsBullets = true;
            breakable3.OnlyPlayerProjectilesCanBreak = false;
            breakable3.OnlyBreaksOnScreen = false;
            breakable3.resistsExplosions = false;
            breakable3.canSpawnFairy = false;
            breakable3.chanceToRain = 1;
            breakable3.dropCoins = false;
            breakable3.goopsOnBreak = false;
            breakable3.breakStyle = MinorBreakable.BreakStyle.BURST;
            breakable3.shardClusters = array;
            breakable3.amountToRain = 30;
            breakable3.EmitStyle = GlobalSparksDoer.EmitRegionStyle.RANDOM;
            breakable3.ParticleColor = Color.white;
            breakable3.ParticleLifespan = 2;
            breakable3.ParticleMagnitude = 1;
            breakable3.ParticleMagnitudeVariance = 1;
            breakable3.ParticleSize = 0.1f;
            breakable3.ParticleType = GlobalSparksDoer.SparksType.FLOATY_CHAFF;
            breakable3.hasParticulates = true;
            breakable3.MaxParticlesOnBurst = 10;
            breakable3.MinParticlesOnBurst = 4;
            breakable3.gameObject.AddComponent<TresspassUnlitShaderController>();
            breakable3.gameObject.AddComponent<TheGames.Marker>();

            breakable3.sprite.SortingOrder = 0;
            breakable3.sprite.HeightOffGround = -1;
            breakable3.gameObject.layer = LayerMask.NameToLayer("FG_Critical");

            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { breakable.gameObject, 0.7f },
                { breakable2.gameObject, 0.6f },
                { breakable3.gameObject, 0.2f },
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("tresPassPots", placeable);
        }
    }
}
