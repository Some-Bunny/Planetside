using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NpcApi;
using UnityEngine;
using ItemAPI;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using System.Collections;
using SaveAPI;

namespace Planetside
{
    class CustomShopInitialiser
    {
        public static void InitialiseCustomShops()
        {
            InitialiseTimeTrader();
            InitialiseTablert();
            Gregthly();
            InitMasteryTrader();


            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("DefaultLabelPanel", ".prefab"));
            FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            var dlm = gameObject.GetComponent<DefaultLabelController>();

            var mdlm = gameObject.AddComponent<ModifiedDefaultLabelManager>();
            mdlm.label = dlm.label;
            mdlm.m_cachedCache = dlm.m_cachedCache;
            mdlm.m_manager = dlm.m_manager;
            mdlm.offset = dlm.offset;
            mdlm.panel = dlm.panel;
            mdlm.targetObject = dlm.targetObject;
            UnityEngine.Object.Destroy(dlm);

            var label = mdlm.label;
            label.backgroundColor = new Color32(40, 1, 20, 180);
            label.colorizeSymbols = false;

            label.textScale = 3.33f;
            //label.textScaleMode = dfTextScaleMode.None;

            label.shadow = true;
            label.shadowColor = new Color32(20, 20, 50, 255);
            label.shadowOffset = new Vector2(0, -0.75f);
            //label.Height = 1000;

            label.anchorStyle = dfAnchorStyle.Top | dfAnchorStyle.Left;

            label.autoSize = true;
            label.autoHeight = true;
            label.wordWrap = false;

            var data = new dfRenderData()
            {
                Glitchy = false,
                Shader = ShaderCache.Acquire("Brave/Internal/HologramShader"),
            };
            label.renderData = data;

            PerkLabel = mdlm;

            ETGModConsole.Commands.AddGroup("psog_shope", args =>
            {
            });
            ETGModConsole.Commands.GetGroup("psog_shope").AddUnit("toggletest", ForceEnableMixedFloor);
            //new Hook(typeof(PlayMakerFSM).GetMethod("SendEvent", BindingFlags.Instance | BindingFlags.Public), typeof(CustomShopInitialiser).GetMethod("SendEventHook"));
        }

        public static void ForceEnableMixedFloor(string[] s)
        {
            SaveAPIManager.SetFlag(CustomDungeonFlags.SPECIAL_LOCK, !SaveAPIManager.GetFlag(CustomDungeonFlags.SPECIAL_LOCK));

        }

        public static ModifiedDefaultLabelManager PerkLabel;


   
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

            ETGMod.Databases.Strings.Core.Set("#TABLERT_RUNBASEDMULTILINE_STOLEN", "GRAAHHHPALACH!");

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
            , "#TABLERT_RUNBASEDMULTILINE_STOLEN"
            , new Vector3(1.375f, 1f)
            , new Vector3(1.25f, 2.4375f, 5.9375f)
            , ItsDaFuckinShopApi.VoiceBoxes.MONSTER_MANUEL
            , new Vector3[] { new Vector3(0.5f, 1.25f, 1), new Vector3(2f, 1.1f, 1), new Vector3(3.5f, 1.5625f, 1),  }
            , 0.66f
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
            ,0
            ,0
            , true
            , "Planetside/Resources/NPCs/TableDude/talbertIcon.png"
            , true
            , 0.15f
            , null
            );
            ItsDaFuckinShopApi.AddAdditionalAnimationsToShop(talbertObj, DancePaths, 12);


            talbertObj.GetComponentInChildren<tk2dSpriteAnimator>().gameObject.AddComponent<SlideSurface>();
            EnemyToolbox.AddSoundsToAnimationFrame(talbertObj.GetComponentInChildren<tk2dSpriteAnimator>(), "purchase", new Dictionary<int, string>() { { 4, "Play_OBJ_chest_shake_01" }, { 0, "Play_OBJ_chest_shake_01" } });
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
        public static void Gregthly()
        {
            string baseFilepath = "Planetside/Resources/NPCs/Gregthly/gregthly";

            ETGMod.Databases.Strings.Core.AddComplex("#GREGTHLY_RUNBASEDMULTILINE_GENERIC", "...");
            ETGMod.Databases.Strings.Core.AddComplex("#GREGTHLY_RUNBASEDMULTILINE_GENERIC", "...");
            ETGMod.Databases.Strings.Core.AddComplex("#GREGTHLY_RUNBASEDMULTILINE_GENERIC", "...");
            ETGMod.Databases.Strings.Core.AddComplex("#GREGTHLY_RUNBASEDMULTILINE_GENERIC", "...");

            ETGMod.Databases.Strings.Core.Set("#GREGTHLY_RUNBASEDMULTILINE_STOPPER", ". . .");
            ETGMod.Databases.Strings.Core.Set("#GREGTHLY_RUNBASEDMULTILINE_PURCHASE", "!!!");
            ETGMod.Databases.Strings.Core.Set("#GREGTHLY_RUNBASEDMULTILINE_FAILPURCHASE", ".....");
            ETGMod.Databases.Strings.Core.Set("#GREGTHLY_RUNBASEDMULTILINE_INTRO", "...");
            ETGMod.Databases.Strings.Core.Set("#GREGTHLY_RUNBASEDMULTILINE_ATTACKED", "???");

            ETGMod.Databases.Strings.Core.Set("#GREGTHLY_RUNBASEDMULTILINE_STOLEN", ">:(");

            GameObject timedShop = ItsDaFuckinShopApi.SetUpShop(
                  "gregthly"
                , "psog"
                , new List<string> { baseFilepath + "1.png", baseFilepath + "2.png", baseFilepath + "3.png", baseFilepath + "4.png" }
                , 4
                , new List<string> { baseFilepath + "1.png", baseFilepath + "2.png", baseFilepath + "3.png", baseFilepath + "4.png" }
                , 6
                , ShopInABox.shopInABoxPickupTable
                , CustomShopItemController.ShopCurrencyType.COINS
                , "#GREGTHLY_RUNBASEDMULTILINE_GENERIC"
                , "#GREGTHLY_RUNBASEDMULTILINE_STOPPER"
                , "#GREGTHLY_RUNBASEDMULTILINE_PURCHASE"
                , "#GREGTHLY_RUNBASEDMULTILINE_FAILPURCHASE"
                , "#GREGTHLY_RUNBASEDMULTILINE_INTRO"
                , "#GREGTHLY_RUNBASEDMULTILINE_ATTACKED"
                , "#GREGTHLY_RUNBASEDMULTILINE_STOLEN"
                , new Vector3(0.25f, 1f)
                , new Vector3(0.375f, 1.375f, 5.9375f)
                , ItsDaFuckinShopApi.VoiceBoxes.FOOL
                , new Vector3[] { new Vector3(0.75f, 0.875f, 1) }
                , 0.7f
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
                , 0
                , 0
                , true
                , null
                , false
                , 0.1f
                , null
                ,2, 
                 CustomShopController.PoolType.DEFAULT, 
                 true);
            ItsDaFuckinShopApi.AddAdditionalAnimationsToShop(timedShop, 
                new List<string> { baseFilepath + "squeeze_001.png", baseFilepath + "squeeze_002.png", baseFilepath + "squeeze_003.png", baseFilepath + "squeeze_004.png" }, 6,
                new List<string> { baseFilepath + "squeeze_002.png", baseFilepath + "squeeze_002.png", baseFilepath + "squeeze_002.png", baseFilepath + "squeeze_002.png" }, 2,
                new List<string> { baseFilepath + "squeeze_003.png", baseFilepath + "squeeze_003.png", baseFilepath + "squeeze_003.png", baseFilepath + "squeeze_003.png" }, 1
            );
 
            EnemyToolbox.AddSoundsToAnimationFrame(timedShop.GetComponentInChildren<tk2dSpriteAnimator>(), "purchase", new Dictionary<int, string>() { { 1, "Play_WPN_teddy_impact_03" } });
            EnemyToolbox.AddSoundsToAnimationFrame(timedShop.GetComponentInChildren<tk2dSpriteAnimator>(), "stolen", new Dictionary<int, string>() { { 0, "Play_ENM_wizardred_appear_01" } });


            //gregthyShop = timedShop;
            StaticReferences.StoredRoomObjects.Add("gregthlyShop", timedShop);


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
        public static void InitMasteryTrader()
        {
            string baseFilepath = "Planetside/Resources/NPCs/Gort/gort_idle_";
            string currencyPath = "Planetside/Resources/NPCs/CustomCurrencyIcons/";

            ETGMod.Databases.Strings.Core.AddComplex("MASTERYTRADER_RUNBASEDMULTILINE_GENERIC", "It's been... so long...");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_GENERIC", "...");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_GENERIC", "...");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_GENERIC", "...");

            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_STOPPER", "Leave me... be...");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_STOPPER", "You can't... free me... regardless...");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_STOPPER", "Let me... rest...");

            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_PURCHASE", "This should... aid you...");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_PURCHASE", "Good choice...");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_PURCHASE", "Good... form...");

            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_FAILPURCHASE", "Not enough... potential...");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_FAILPURCHASE", "Return more... proven...");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_FAILPURCHASE", "Bring more... vitality...");

            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_INTRO", "Kaliber have merc- Nevermind...");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_INTRO", "How did you... get here..?");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_INTRO", "Will you... free me..?");

            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_ATTACKED", "How could... you..?");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_ATTACKED", "No mercy... in you...");
            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_ATTACKED", "Why..?");

            ETGMod.Databases.Strings.Core.AddComplex("#MASTERYTRADER_RUNBASEDMULTILINE_STOLEN", "How..?");


            GenericLootTable MasteryTable = LootTableTools.CreateLootTable();
            MasteryTable.AddItemsToPool(new Dictionary<int, float> { {AllSeeingEye.AllSeeingEyeID, 1}, { AllStatsUp.AllStatsUpID, 1 }, { BlastProjectiles.BlastProjectilesID, 1 }, { ChaoticShift.ChaoticShiftID, 1 }, { Contract.ContractID, 1 }, { Glass.GlassID, 1 }, { Greedy.GreedyID, 1 }, { Gunslinger.GunslingerID, 1 }, { PitLordsPact.PitLordsPactID, 1 }, { UnbreakableSpirit.UnbreakableSpiritID, 1 }, { Patience.PatienceID, 1 }, { CorruptedWealth.CorruptedWealthID, 1 } });
            GameObject masteryShop = ItsDaFuckinShopApi.SetUpShop(
                  "masteryRewardTrader"
                , "psog"
                , new List<string> { baseFilepath + "001.png", baseFilepath + "002.png", baseFilepath + "003.png", baseFilepath + "004.png", baseFilepath + "005.png", baseFilepath + "006.png", baseFilepath + "007.png", baseFilepath + "008.png", baseFilepath + "009.png", baseFilepath + "010.png", baseFilepath + "011.png", baseFilepath + "012.png", }
                , 7
                , new List<string> { baseFilepath + "001.png", baseFilepath + "002.png", baseFilepath + "003.png", baseFilepath + "004.png", baseFilepath + "005.png", baseFilepath + "006.png", baseFilepath + "007.png", baseFilepath + "008.png", baseFilepath + "009.png", baseFilepath + "010.png", baseFilepath + "011.png", baseFilepath + "012.png", }
                , 8
                , MasteryTable
                , CustomShopItemController.ShopCurrencyType.CUSTOM
                , "#MASTERYTRADER_RUNBASEDMULTILINE_GENERIC"
                , "#MASTERYTRADER_RUNBASEDMULTILINE_STOPPER"
                , "#MASTERYTRADER_RUNBASEDMULTILINE_PURCHASE"
                , "#MASTERYTRADER_RUNBASEDMULTILINE_FAILPURCHASE"
                , "#MASTERYTRADER_RUNBASEDMULTILINE_INTRO"
                , "#MASTERYTRADER_RUNBASEDMULTILINE_ATTACKED"
                , "#MASTERYTRADER_RUNBASEDMULTILINE_STOLEN"
                , new Vector3(1.25f, 3.75f)
                , new Vector3(0f, 0f, 5.9375f)
                , ItsDaFuckinShopApi.VoiceBoxes.TONIC
                , new Vector3[] { new Vector3(0.5f, -1f, 1), new Vector3(2.25f, -1.5f, 1), new Vector3(4f, -1f, 1) }
                , 1f
                , false
                , null
                , MasterTraderCustomCanBuy //CustomCanBuy
                , MasterTraderRemoveCurrency //CustomRemoveCurrency
                , MasterTraderCustomPrice //Customprice
                , null //OnPurchase
                , null //OnSteal
                , currencyPath + "masterRoundCurrencyIcon.png" //CurrencyIconPath
                , "masteryrewards"//Currencyname
                , false //CanBeRobbed
                , true //HasCarpet
                , "Planetside/Resources/NPCs/Gort/pebbles.png" //caprte path
                ,0
                ,-3
                , false //has minimapicon
                , null //minimapiconspritepath
                , false //addtoNPCPool
                , 0 //chancetobeinmainnpcPoool
                , null //dungeonprereqwuisite
                ,2
                , CustomShopController.PoolType.DUPES_AND_NOEXCLUSION
                , true,
                  false);
            masteryShop.GetComponentInChildren<tk2dBaseSprite>().usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = masteryShop.GetComponentInChildren<tk2dBaseSprite>().sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 3f);
            mat.SetFloat("_EmissivePower", 80);
            masteryShop.GetComponentInChildren<tk2dBaseSprite>().sprite.renderer.material = mat;
            masteryShop.AddComponent<PerkShopController>();
            StaticReferences.StoredRoomObjects.Add("masteryRewardTrader", masteryShop);
        }

        public static int MasterTraderCustomPrice(CustomShopController shop, CustomShopItemController itemCont, PickupObject item)
        {
            return 1;

        }
        public static int MasterTraderRemoveCurrency(CustomShopController shop, PlayerController player, int cost)
        {
            List<BasicStatPickup> rounds = new List<BasicStatPickup>();
            foreach (PassiveItem item in player.passiveItems)
            {
                if (item is BasicStatPickup mastery)
                {
                    if (mastery.IsMasteryToken == true)
                    {
                        rounds.Add(mastery);
                    }
                }
            }
            rounds.Shuffle();
            player.RemovePassiveItem(rounds[0].PickupObjectId);
            SaveAPI.AdvancedGameStatsManager.Instance.RegisterStatChange(SaveAPI.CustomTrackedStats.PERKS_BOUGHT, 1);
            return 1;
        }
        public static bool MasterTraderCustomCanBuy(CustomShopController shop, PlayerController player, int cost)
        {
            bool HasMastery = false;
            foreach (PassiveItem item in player.passiveItems)
            {
                if (item is BasicStatPickup mastery)
                {
                    if (mastery.IsMasteryToken == true)
                    {
                        HasMastery = true;
                    }
                }
            }
            return HasMastery;
        }





        public static void InitialiseTimeTrader()
        {
            string baseFilepath = "Planetside/Resources/NPCs/TimeTrader/timetrader";

            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_GENERIC", "Been a while since I seen ya!");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_GENERIC", "Tell that blue haired gun-toter to stop trespassing where they shouldn't if you meet 'em, will ya?");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_GENERIC", "I swear I've seen that big rusty casing guy back home...");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_GENERIC", "The universe is nice, isn't it? Ya got my boss to thank for that!");

            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_STOPPER", "Ya' don't got much time, ya' know that?");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_STOPPER", "Universe ain't got much time ya know?");

            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_PURCHASE", "That'll save ya' some time!");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_PURCHASE", "That one will sure speed ya up!");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_PURCHASE", "Many thanks!");


            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_FAILPURCHASE", "Maybe some other time, pal.");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_FAILPURCHASE", "Sorry, not this time.");


            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_INTRO", "Just on time, pal!");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_INTRO", "Real speedy one ya are!");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_INTRO", "Long time no see pal!");

            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_ATTACKED", "Ya' bullets are in the wrong timeline pal!");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_ATTACKED", "That won't work like last time!");
            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_ATTACKED", "Whadd'ya expect to change from last time?");

            ETGMod.Databases.Strings.Core.AddComplex("#TIMETRADER_RUNBASEDMULTILINE_STOLEN", "You'll be paying with your time for this!");


            GameObject timedShop = ItsDaFuckinShopApi.SetUpShop(
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
                ,"#TIMETRADER_RUNBASEDMULTILINE_STOLEN"        
                , new Vector3(1.375f, 2f)
                , new Vector3(1.4375f, 3.4375f, 5.9375f)
                , ItsDaFuckinShopApi.VoiceBoxes.ALIEN
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
                , 0
                , 0
                , true
                , "Planetside/Resources/NPCs/TimeTrader/icon.png"
                , false
                , 0.1f
                , null);
            StaticReferences.StoredRoomObjects.Add("timedShop", timedShop);



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


    public class PerkShopController : MonoBehaviour
    {
        public void Start()
        {
            CustomShopController customShopController = this.GetComponent<CustomShopController>();
            var list = customShopController.m_itemControllers;
            foreach (var item in list)
            {
                if (item is CustomShopItemController customShopItem)
                {
                    if (customShopItem.item is PerkPickupObject perk)
                    {
                        customShopItem.OverrideText = (player, self, special) =>
                        {
                            string Text = StringTableManager.GetItemsString(perk.encounterTrackable.journalData.PrimaryDisplayName) + "  " + special + " " + customShopController.customPrice(customShopController, customShopItem, customShopItem.item);
                            foreach (var enrty in perk.perkDisplayContainers)
                            {
                                bool req = enrty.requiresFlag == false ? false : SaveAPI.AdvancedGameStatsManager.Instance.GetFlag(enrty.FlagToTrack);
                                bool req2 = enrty.requiresStack == false ? false : SaveAPI.AdvancedGameStatsManager.Instance.GetPlayerStatValue(perk.StatToIncreaseOnPickup) >= enrty.AmountToBuyBeforeReveal;

                                if (req == true || req2 == true)
                                {
                                    Text += "\n- " + enrty.UnlockedString;
                                }
                                else
                                {
                                    Text += "\n- " + enrty.LockedString;
                                }
                            }
                            customShopItem.safetyCoin = UIToolbox.GenerateText(this.transform, new Vector2(1.5f, -1f), 0.5f, Text, new Color32(0, 12, 50, 200));
                        };
                        customShopItem.OverrideExitText = (player, self) =>
                        {
                            if (customShopItem.safetyCoin) { customShopItem.safetyCoin.Inv(); }
                        };
                    }
                }
            }
        }
    }
    public class ModifiedDefaultLabelManager : DefaultLabelController
    {
        public void Trigger_C(float d)
        {
            base.StartCoroutine(this.Expand_CR_Custom(d));
        }

        public void Trigger_CustomTime(Transform aTarget, Vector3 anOffset, float duration, float AutoDestroyTimer = -1)
        {
            this.offset = anOffset;
            this.targetObject = aTarget;
            this.Trigger_C(duration);
            if (AutoDestroyTimer > 0)
            {
                this.Invoke("Inv", AutoDestroyTimer);
            }
        }

        public void Trigger_CustomDestroyTime(float duration)
        {
            base.StartCoroutine(this.Unexpand_CR_Custom(duration, true));
        }

        public void Inv()
        {
            base.StartCoroutine(this.Unexpand_CR_Custom(0.35f, true));
        }


        private IEnumerator Expand_CR_Custom(float duration)
        {
            float elapsed = 0f;
            float targetWidth = this.label.Width + 1f;
            float targetHeight = this.label.Height + 1f;
            panel.padding.bottom = (int)(targetHeight * -1);
            this.panel.Width = targetWidth;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                this.panel.Width = Mathf.Lerp(1f, targetWidth, MathToolbox.SinLerpTValue(elapsed / duration));
                yield return null;
            }
            yield break;
        }


        public Action<dfLabel, bool> MouseHover;

        private IEnumerator Unexpand_CR_Custom(float duration, bool willDestroy = false)
        {
            float elapsed = 0f;
            float targetWidth = this.label.Width + 1f;
            float targetHeight = this.label.Height + 1f;
            panel.padding.bottom = (int)(targetHeight * -1);
            this.panel.Width = targetWidth;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                this.panel.Width = Mathf.Lerp(targetWidth, 0, elapsed / duration);
                yield return null;
            }
            if (willDestroy == true)
            {
                Destroy(this.gameObject);
            }
            yield break;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }


    }
}
