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

namespace Planetside
{
    public class TrespassCandle
    {
        public static void Init()
        {

            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/Candle/";
            string[] animPaths = new string[]
            {
                defaultPath+"trespasscandle_idle_001.png",
                defaultPath+"trespasscandle_idle_002.png",
                defaultPath+"trespasscandle_idle_003.png",
                defaultPath+"trespasscandle_idle_004.png",
                defaultPath+"trespasscandle_idle_005.png",
                defaultPath+"trespasscandle_idle_006.png",
            };

            string[] shardPaths = new string[]
            {
                defaultPath+"Debris/trespasscandle_debris_001.png",
                defaultPath+"Debris/trespasscandle_debris_002.png",
                defaultPath+"Debris/trespasscandle_debris_003.png",
                defaultPath+"Debris/trespasscandle_debris_004.png",
                defaultPath+"Debris/trespasscandle_debris_005.png",
            };


            DebrisObject[] shardObjects = BreakableAPIToolbox.GenerateDebrisObjects(shardPaths, true, 1, 5, 720, 540, null, 0.6f, null, null, 0, false);
            for (int e = 0; e < shardObjects.Length; e++)
            {
                shardObjects[e].gameObject.AddComponent<TresspassUnlitShaderController>();
            }

            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(shardObjects, 0.7f, 1.2f, 6, 9, 0.6f);
         

            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { GenerateThing(potShardCluster, animPaths,defaultPath+"trespasscandle_idle_001.png" , 3).gameObject, 0.7f },
                { GenerateThing(potShardCluster, animPaths, defaultPath+"trespasscandle_idle_001.png",7).gameObject, 0.8f },
                { GenerateThing(potShardCluster, animPaths,defaultPath+"trespasscandle_idle_001.png", 5).gameObject, 0.6f },
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("tresPassCandle", placeable);
        }

        public static MinorBreakable GenerateThing(ShardCluster shardCluster, string[] animPaths,string breakStr, int FPS)
        {
            string shadowPath = "Planetside/Resources/DungeonObjects/EmberPot/emberpotshadow.png";
            MinorBreakable breakable = BreakableAPIToolbox.GenerateMinorBreakable("TrespassCandle", animPaths, FPS, new string[] { breakStr }, 10, "Play_OBJ_box_cover_01", shadowPath, 0, -0.125f, true, 13, 14, 1, 1);
            breakable.stopsBullets = true;
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
            return breakable;
        }
    }
}
