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
    public class Targets
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/Target/";
            string[] idlePaths1 = new string[]
            {
                defaultPath+"Idle/Target1_idle.png",
            };
            string[] breakPaths1 = new string[]
            {
                defaultPath+"Idle/Target1_idle.png",
            };

            string[] idlePaths2 = new string[]
            {
                defaultPath+"Idle/Target2_idle.png",
            };
            string[] breakPaths2 = new string[]
            {
                defaultPath+"Idle/Target2_idle.png",
            };
            string[] idlePaths3 = new string[]
            {
                defaultPath+"Idle/Target3_idle.png",
            };
            string[] breakPaths3 = new string[]
            {
                defaultPath+"Idle/Target3_idle.png",
            };
            string[] idlePaths4 = new string[]
            {
                defaultPath+"Idle/Target4_idle.png",
            };
            string[] breakPaths4 = new string[]
            {
                defaultPath+"Idle/Target4_idle.png",
            };




            string shadowPath = defaultPath+ "TargetShadow.png";
            MinorBreakable targetOne = BreakableAPIToolbox.GenerateMinorBreakable("Target_One", idlePaths1, 1, breakPaths1, 10, "Play_obj_box_break_01", shadowPath, 0, -0.125f, true, 14, 14, 1, 0);
            targetOne.stopsBullets = true;
            targetOne.OnlyPlayerProjectilesCanBreak = true;
            targetOne.OnlyBreaksOnScreen = false;
            targetOne.resistsExplosions = false;
            targetOne.canSpawnFairy = false;
            targetOne.breakStyle = MinorBreakable.BreakStyle.CONE;
            
            MinorBreakable targetTwo = BreakableAPIToolbox.GenerateMinorBreakable("Target_two", idlePaths2, 1, breakPaths2, 10, "Play_obj_box_break_01", shadowPath, 0, -0.125f, true, 14, 14, 1, 0);
            targetTwo.stopsBullets = true;
            targetTwo.OnlyPlayerProjectilesCanBreak = true;
            targetTwo.OnlyBreaksOnScreen = false;
            targetTwo.resistsExplosions = false;
            targetTwo.canSpawnFairy = false;
            targetTwo.breakStyle = MinorBreakable.BreakStyle.CONE;
            MinorBreakable targetThree = BreakableAPIToolbox.GenerateMinorBreakable("Target_three", idlePaths3, 1, breakPaths3, 10, "Play_obj_box_break_01", shadowPath, 0, -0.125f, true, 14, 14, 1, 0);
            targetThree.stopsBullets = true;
            targetThree.OnlyPlayerProjectilesCanBreak = true;
            targetThree.OnlyBreaksOnScreen = false;
            targetThree.resistsExplosions = false;
            targetThree.canSpawnFairy = false;
            targetThree.breakStyle = MinorBreakable.BreakStyle.CONE;
            MinorBreakable targetFour = BreakableAPIToolbox.GenerateMinorBreakable("Target_four", idlePaths4, 1, breakPaths4, 10, "Play_obj_box_break_01", shadowPath, 0, -0.125f, true, 14, 14, 1, 0);
            targetFour.stopsBullets = true;
            targetFour.OnlyPlayerProjectilesCanBreak = true;
            targetFour.OnlyBreaksOnScreen = false;
            targetFour.resistsExplosions = false;
            targetFour.canSpawnFairy = false;
            targetFour.breakStyle = MinorBreakable.BreakStyle.CONE;
            
            string defaultPathDebris = "Planetside/Resources/DungeonObjects/Target/Debris/";
            string[] woodPolePathsMedium = new string[]
            {
                defaultPathDebris+"TargetDebrisMedium1.png",
                defaultPathDebris+"TargetDebrisMedium2.png",
                defaultPathDebris+"TargetDebrisMedium3.png",
            };
            string[] woodPolePathsSmall = new string[]
            {
                defaultPathDebris+"TargetDebrisSmall1.png",
                defaultPathDebris+"TargetDebrisSmall2.png",
                defaultPathDebris+"TargetDebrisSmall3.png",
                defaultPathDebris+"TargetDebrisSmall4.png",
            };
            DebrisObject[] woodPoleSmall = BreakableAPIToolbox.GenerateDebrisObjects(woodPolePathsSmall, true, 5, 5, 720, 240, null, 1.5f, null, null, 2);
            DebrisObject[] woodPoleMedium = BreakableAPIToolbox.GenerateDebrisObjects(woodPolePathsMedium, true, 3, 5, 480, 120, null, 1f, null, null, 1);

            string[] paperTatter1 = new string[]
            {
                defaultPathDebris+"PaperTatter1_001.png",
                defaultPathDebris+"PaperTatter1_002.png",
                defaultPathDebris+"PaperTatter1_003.png",
                defaultPathDebris+"PaperTatter1_004.png",
            };
            string[] paperTatter2 = new string[]
            {
                defaultPathDebris+"PaperTatter2_001.png",
                defaultPathDebris+"PaperTatter2_002.png",
                defaultPathDebris+"PaperTatter2_003.png",
            };
            string[] paperTatter3 = new string[]
            {
                defaultPathDebris+"PaperTatter3_001.png",
                defaultPathDebris+"PaperTatter3_002.png",
                defaultPathDebris+"PaperTatter3_003.png",
            };
            DebrisObject paperTatter1Debris = BreakableAPIToolbox.GenerateAnimatedDebrisObject(paperTatter1, 4, tk2dSpriteAnimationClip.WrapMode.Once, true, 3, 9, 540, 90, null, 1.3f); //true, 3, 5, 480, 120, null, 1f, null, null, 0, false);
            DebrisObject paperTatter2Debris = BreakableAPIToolbox.GenerateAnimatedDebrisObject(paperTatter2, 3, tk2dSpriteAnimationClip.WrapMode.Once, true, 3, 9, 480, 60, null, 0.8f); //true, 3, 5, 480, 120, null, 1f, null, null, 0, false);
            DebrisObject paperTatter3Debris = BreakableAPIToolbox.GenerateAnimatedDebrisObject(paperTatter3, 2, tk2dSpriteAnimationClip.WrapMode.Once, true, 3, 9, 300, 100, null, 1f); //true, 3, 5, 480, 120, null, 1f, null, null, 0, false);
            DebrisObject[] paperArray = new DebrisObject[]
            {
                paperTatter1Debris,
                paperTatter2Debris,
                paperTatter3Debris
            };
            ShardCluster paperCluster = BreakableAPIToolbox.GenerateShardCluster(paperArray, 2f, 1f, 4, 11, 0.8f);
            ShardCluster woodClusterSmall = BreakableAPIToolbox.GenerateShardCluster(woodPoleSmall, 0.7f, 1.3f, 2, 5, 1.3f);
            ShardCluster woodClusterMedium = BreakableAPIToolbox.GenerateShardCluster(woodPoleMedium, 0.4f, 1.2f, 1, 3, 1.05f);


            targetOne.shardClusters = new ShardCluster[] {paperCluster, woodClusterSmall, woodClusterMedium };
            targetTwo.shardClusters = new ShardCluster[] { paperCluster, woodClusterSmall, woodClusterMedium };
            targetThree.shardClusters = new ShardCluster[] { paperCluster, woodClusterSmall, woodClusterMedium };
            targetFour.shardClusters = new ShardCluster[] { paperCluster, woodClusterSmall, woodClusterMedium };


            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { targetOne.gameObject, 1f },
                { targetTwo.gameObject, 0.75f },
                { targetThree.gameObject, 0.6f },
                { targetFour.gameObject, 0.4f },
            };

            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("targetPlaceable", placeable);
        }
    }
}
