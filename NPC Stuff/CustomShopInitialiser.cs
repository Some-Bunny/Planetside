using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NpcApi;
using UnityEngine;
using ItemAPI;
using GungeonAPI;
namespace Planetside
{
    class CustomShopInitialiser
    {
        public static void InitialiseCustomShops()
        {
            InitialiseTimeTrader();
            InitialiseTablert();
        }

        public static void InitialiseTablert()
        {
            string baseFilepath = "Planetside/Resources/NPCs/TableDude/";

            ETGMod.Databases.Strings.Core.AddComplex("#TABLERT_RUNBASEDMULTILINE_GENERIC", "Grrpahah...");
            ETGMod.Databases.Strings.Core.AddComplex("#TABLERT_RUNBASEDMULTILINE_GENERIC", "Grr...");
            ETGMod.Databases.Strings.Core.AddComplex("#TABLERT_RUNBASEDMULTILINE_GENERIC", "Grraphlaach!");
            ETGMod.Databases.Strings.Core.AddComplex("#TABLERT_RUNBASEDMULTILINE_GENERIC", "...");

            ETGMod.Databases.Strings.Core.Set("#TABLERT_RUNBASEDMULTILINE_STOPPER", "Hrrapng....");
            ETGMod.Databases.Strings.Core.Set("#TABLERT_RUNBASEDMULTILINE_PURCHASE", "Gah! Gah! Gah! Gah!");
            ETGMod.Databases.Strings.Core.Set("#TABLERT_RUNBASEDMULTILINE_FAILPURCHASE", "Hrrr......");
            ETGMod.Databases.Strings.Core.Set("#TABLERT_RUNBASEDMULTILINE_INTRO", "?!");
            ETGMod.Databases.Strings.Core.Set("#TABLERT_RUNBASEDMULTILINE_ATTACKED", "HARPAHALACH!");


            List<string> DancePaths = new List<string>()
            {
                baseFilepath + "tablert_dance_001.png",
                baseFilepath + "tablert_dance_002.png",
                baseFilepath + "tablert_dance_003.png",
                baseFilepath + "tablert_dance_004.png",
                baseFilepath + "tablert_dance_005.png",
                baseFilepath + "tablert_dance_006.png",
                baseFilepath + "tablert_dance_007.png",
                baseFilepath + "tablert_dance_008.png",
                baseFilepath + "tablert_dance_001.png",
                baseFilepath + "tablert_dance_002.png",
                baseFilepath + "tablert_dance_003.png",
                baseFilepath + "tablert_dance_004.png",
                baseFilepath + "tablert_dance_005.png",
                baseFilepath + "tablert_dance_006.png",
                baseFilepath + "tablert_dance_007.png",
                baseFilepath + "tablert_dance_008.png",
            };
            GameObject talbertObj = ItsDaFuckinShopApi.SetUpShop(
            "tabletechshop"
            ,"psog"
            , new List<string> { baseFilepath + "tablert_idle_001.png", baseFilepath + "tablert_idle_002.png", baseFilepath + "tablert_idle_003.png", baseFilepath + "tablert_idle_004.png", baseFilepath + "tablert_idle_005.png" }
            , 6
            , new List<string> { baseFilepath + "tablert_talk_001.png", baseFilepath + "tablert_talk_002.png", baseFilepath + "tablert_talk_003.png", baseFilepath + "tablert_talk_004.png", baseFilepath + "tablert_talk_005.png", }
            , 11
            , CustomLootTableInitialiser.TalbertKeeperTable
            , CustomShopItemController.ShopCurrencyType.COINS
            , "#TABLERT_RUNBASEDMULTILINE_GENERIC"
            , "#TABLERT_RUNBASEDMULTILINE_STOPPER"
            , "#TABLERT_RUNBASEDMULTILINE_PURCHASE"
            , "#TABLERT_RUNBASEDMULTILINE_FAILPURCHASE"
            , "#TABLERT_RUNBASEDMULTILINE_INTRO"
            , "#TABLERT_RUNBASEDMULTILINE_ATTACKED"
            , new Vector3(1.375f, 1f)
            , new Vector3(1.25f, 2.4375f, 5.9375f)
            , new Vector3[] { new Vector3(0.5f, 1.25f, 1), new Vector3(2f, 1.1f, 1), new Vector3(3.5f, 1.5625f, 1),  }
            , 0.8f
            , false
            , null
            , null
            , null
            , null
            , null
            , null
            , null
            , ""
            , true
            , true
            , baseFilepath + "talbertcarpet.png"
            , true
            , "Planetside/Resources/NPCs/TableDude/talbertIcon.png"
            , true
            , 0.2f
            , null
            , DancePaths);
           
            talbertObj.GetComponentInChildren<tk2dSpriteAnimator>().gameObject.AddComponent<SlideSurface>();
            EnemyToolbox.AddSoundsToAnimationFrame(talbertObj.GetComponentInChildren<tk2dSpriteAnimator>(), "dance", new Dictionary<int, string>() { { 4, "Play_OBJ_chest_shake_01" }, { 0, "Play_OBJ_chest_shake_01" } });
            EnemyToolbox.AddSoundsToAnimationFrame(talbertObj.GetComponentInChildren<tk2dSpriteAnimator>(), "talk", new Dictionary<int, string>() { { 3, "Play_VO_mimic_death_01" } });

            talbertObj.GetComponentInChildren<tk2dBaseSprite>().usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = talbertObj.GetComponentInChildren<tk2dBaseSprite>().sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 100);
            talbertObj.GetComponentInChildren<tk2dBaseSprite>().sprite.renderer.material = mat;

            ItsDaFuckinShopApi.GenerateOrAddToRigidBody(talbertObj.GetComponentInChildren<tk2dSpriteAnimator>().gameObject, CollisionLayer.LowObstacle, PixelCollider.PixelColliderGeneration.Manual, true, true, true, false, false, false, false, true, new IntVector2(32, 18), new IntVector2(5, -2));
            StaticReferences.StoredRoomObjects.Add("talber", talbertObj);
        }

     

        public static void InitialiseTimeTrader()
        {
            string baseFilepath = "Planetside/Resources/NPCs/TimeTrader/timetrader";

            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_GENERIC", "Been a while since I seen ya!");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_GENERIC", "Tell that blue haired gun-toter to stop trespassing where they shouldn't if you meet 'em, will ya?");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_GENERIC", "I swear I've seen that big rusty casing guy back home...");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_GENERIC", "The universe is nice, isn't it? Ya got my boss to thanl for that!");

            ETGMod.Databases.Strings.Core.Set("#TIMETRADER_RUNBASEDMULTILINE_STOPPER", "Ya' don't got much time, ya' know that?");
            ETGMod.Databases.Strings.Core.Set("#TIMETRADER_RUNBASEDMULTILINE_PURCHASE", "That'll save ya' some time!");
            ETGMod.Databases.Strings.Core.Set("#TIMETRADER_RUNBASEDMULTILINE_FAILPURCHASE", "Maybe some other time, pal.");
            ETGMod.Databases.Strings.Core.Set("#TIMETRADER_RUNBASEDMULTILINE_INTRO", "Just on time, pal!");
            ETGMod.Databases.Strings.Core.Set("#TIMETRADER_RUNBASEDMULTILINE_ATTACKED", "Ya' bullets are in the wrong timeline pal!");


            ItsDaFuckinShopApi.SetUpShop(
                  "timedshop"
                , "psog"
                , new List<string> { baseFilepath + "_idle_001.png", baseFilepath + "_idle_002.png", baseFilepath + "_idle_003.png", baseFilepath + "_idle_004.png" }
                , 4
                , new List<string> { baseFilepath + "_talk_001.png", baseFilepath + "_talk_002.png", baseFilepath + "_talk_003.png", baseFilepath + "_talk_004.png" }
                , 6
                , CustomLootTableInitialiser.TimeShopKeeperTable
                , CustomShopItemController.ShopCurrencyType.COINS
                , "#TIMETRADER_RUNBASEDMULTILINE_GENERIC"
                , "#TIMETRADER_RUNBASEDMULTILINE_STOPPER"
                , "#TIMETRADER_RUNBASEDMULTILINE_PURCHASE"
                , "#TIMETRADER_RUNBASEDMULTILINE_FAILPURCHASE"
                , "#TIMETRADER_RUNBASEDMULTILINE_INTRO"
                , "#TIMETRADER_RUNBASEDMULTILINE_ATTACKED"
                , new Vector3(1.375f, 2f)
                , new Vector3(1.4375f, 3.4375f, 5.9375f)
                , new Vector3[] { new Vector3(1.125f, 1.5625f, 1), new Vector3(2.625f, 0.875f, 1), new Vector3(4.125f, 1.5625f, 1), new Vector3(2.625f, 2.125f, 1) }
                , 0.6f
                , false
                , null
                , null
                , null
                , null
                , null
                , null
                , null
                , ""
                , true
                , true
                , baseFilepath + "carpet.png"
                , true
                , "Planetside/Resources/NPCs/TimeTrader/icon.png"
                , false
                , 0.1f
                , null);



            /*
            string baseFilepath = "Planetside/Resources/NPCs/TimeTrader/timetrader";
            ETGMod.Databases.Strings.Core.Set("#TIMETRADER_RUNBASEDMULTILINE_GENERIC", "Been a while since I seen ya!");
            ETGMod.Databases.Strings.Core.Set("#TIMETRADER_RUNBASEDMULTILINE_STOPPER", "Ya' Don't got much time, ya' know that?");
            ETGMod.Databases.Strings.Core.Set("#TIMETRADER_RUNBASEDMULTILINE_PURCHASE", "That'll save ya' some time!");
            ETGMod.Databases.Strings.Core.Set("#TIMETRADER_RUNBASEDMULTILINE_FAILPURCHASE", "Maybe some other time, pal.");
            ETGMod.Databases.Strings.Core.Set("#TIMETRADER_RUNBASEDMULTILINE_INTRO", "Just on time, pal!");
            ETGMod.Databases.Strings.Core.Set("#TIMETRADER_RUNBASEDMULTILINE_ATTACKED", "Ya' bullets are in the wrong timeline pal!");
            ItsDaFuckinOldShopApi.SetUpShop(
                "timedshop",
                "psog",
                new List<string> { baseFilepath+"_idle_001.png", baseFilepath + "_idle_002.png", baseFilepath + "_idle_003.png", baseFilepath + "_idle_004.png" },
                4,
                new List<string> { baseFilepath + "_talk_001.png", baseFilepath + "_talk_002.png", baseFilepath + "_talk_003.png", baseFilepath + "_talk_004.png" },
                6,
                CustomLootTableInitialiser.TimeShopKeeperTable,
                BaseShopController.AdditionalShopType.TRUCK,
                "#TIMETRADER_RUNBASEDMULTILINE_GENERIC",
                "#TIMETRADER_RUNBASEDMULTILINE_STOPPER",
                "#TIMETRADER_RUNBASEDMULTILINE_PURCHASE",
                "#TIMETRADER_RUNBASEDMULTILINE_FAILPURCHASE",
                "#TIMETRADER_RUNBASEDMULTILINE_INTRO",
                "#TIMETRADER_RUNBASEDMULTILINE_ATTACKED",
                true,
                baseFilepath+"carpet.png",
                0.45f,
                -0.3125f,
                3.25f);
            */
        }
    }
}
