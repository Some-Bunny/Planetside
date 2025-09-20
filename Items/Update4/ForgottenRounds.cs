using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using SynergyAPI;
using Alexandria.DungeonAPI;
using Alexandria.Misc;
using HarmonyLib;
using MonoMod.Cil;
using System.Reflection;

namespace Planetside
{
	public class ForgottenRoundOubliette : BasicStatPickup
	{
		public static void Init()
		{
			string name = "Forgotten Round 1";
			//string resourcePath = "Planetside/Resources/forgottenroundsewer.png";
			GameObject gameObject = new GameObject(name);
			ForgottenRoundOubliette warVase = gameObject.AddComponent<ForgottenRoundOubliette>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("forgottenroundsewer"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Forgotten First Chamber";
			string longDesc = "This rare, yet false artifact indicates mastery of the first hidden chamber.\n\nDespite being a false copy of a Master Round, its time spent within the mutagenic conditions of the Oubliette has bestowed it similar power to that of a real master round.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog" ,"Forgotten Round");
			warVase.quality = PickupObject.ItemQuality.SPECIAL;
			warVase.IsMasteryToken = true;
			warVase.ForcedPositionInAmmonomicon = 4;
            warVase.AddAsChamberGunMastery("PlanetsideOfGunymede", 4);
            Alexandria.DungeonAPI.MasteryOverrideHandler.RegisterFloorForMasterySpawn(MasteryOverrideHandler.ViableRegisterFloors.OUBLIETTE);

            //warVase.AddAsChamberGunMastery("PlanetsideOfGunymede", 4);
            SynergyAPI.SynergyBuilder.AddItemToSynergy(warVase, CustomSynergyType.MASTERS_CHAMBERS);
            ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.Damage, 0.1f, StatModifier.ModifyMethod.ADDITIVE);
            ForgottenRoundOubliette.ForgottenRoundOublietteID = warVase.PickupObjectId;
			EncounterDatabase.GetEntry(warVase.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            CustomActions.OnRewardPedestalDetermineContents += OnMasteryDetermineContents;
        }

        public static void OnMasteryDetermineContents(RewardPedestal pedestal, PlayerController determiner, CustomActions.ValidPedestalContents valids)
        {
            if (pedestal.ContainsMasteryTokenForCurrentLevel())
            {
                if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.SEWERGEON)
                {
                    valids.overrideItemPool.Add(new Tuple<int, float>(ForgottenRoundOubliette.ForgottenRoundOublietteID, 1.25f));
                }
            }
        }
        public static int ForgottenRoundOublietteID;	
	}
	public class ForgottenRoundAbbey : BasicStatPickup
	{
		public static void Init()
		{
			string name = "Forgotten Round 2";
			//string resourcePath = "Planetside/Resources/forgottenroundabbey.png";
			GameObject gameObject = new GameObject(name);
			ForgottenRoundAbbey warVase = gameObject.AddComponent<ForgottenRoundAbbey>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("forgottenroundabbey"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Forgotten Second Chamber";
			string longDesc = "This potent, yet false artifact indicates mastery of the second hidden chamber.\n\nDuring his time sealed away in the Abbey, this false round was bathed in the power of the god-like beings that reside in this Chamber.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog", "Forgotten Round");
			warVase.quality = PickupObject.ItemQuality.SPECIAL;
			warVase.IsMasteryToken = true;
			warVase.ForcedPositionInAmmonomicon = 4;
            warVase.AddAsChamberGunMastery("PlanetsideOfGunymede", 8);
            //warVase.AddAsChamberGunMastery("PlanetsideOfGunymede", 8);
            SynergyAPI.SynergyBuilder.AddItemToSynergy(warVase, CustomSynergyType.MASTERS_CHAMBERS);
            Alexandria.DungeonAPI.MasteryOverrideHandler.RegisterFloorForMasterySpawn(MasteryOverrideHandler.ViableRegisterFloors.ABBEY);

            ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.Damage, 0.1f, StatModifier.ModifyMethod.ADDITIVE);
            ForgottenRoundAbbey.ForgottenRoundAbbeyID = warVase.PickupObjectId;
			EncounterDatabase.GetEntry(warVase.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            CustomActions.OnRewardPedestalDetermineContents += OnMasteryDetermineContents;
        }

        public static void OnMasteryDetermineContents(RewardPedestal pedestal, PlayerController determiner, CustomActions.ValidPedestalContents valids)
        {
            if (pedestal.ContainsMasteryTokenForCurrentLevel())
            {
                if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATHEDRALGEON)
                {
                    valids.overrideItemPool.Add(new Tuple<int, float>(ForgottenRoundAbbey.ForgottenRoundAbbeyID, 1.25f));
                }
            }
        }
        public static int ForgottenRoundAbbeyID;

	}
    public class ForgottenRoundRat: BasicStatPickup
    {
        public static void Init()
        {
            string name = "Forgotten Round 3";
            GameObject gameObject = new GameObject(name);
            ForgottenRoundRat warVase = gameObject.AddComponent<ForgottenRoundRat>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("forgottenroundrat1"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Forgotten Third Chamber";
            string longDesc = "This exceptional, yet false artifact indicates mastery of the third hidden chamber.\n\nAll the power of this forgotten round comes from the piece of cheese adorned atop it.";
            ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog", "Forgotten Round");
            warVase.quality = PickupObject.ItemQuality.SPECIAL;
            warVase.IsMasteryToken = true;
            warVase.ForcedPositionInAmmonomicon = 4;
            warVase.AddAsChamberGunMastery("PlanetsideOfGunymede", 32768);
            SynergyAPI.SynergyBuilder.AddItemToSynergy(warVase, CustomSynergyType.MASTERS_CHAMBERS);

            ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.Damage, 0.1f, StatModifier.ModifyMethod.ADDITIVE);
            ForgottenRoundRat.ForgottenRoundRatID = warVase.PickupObjectId;
            EncounterDatabase.GetEntry(warVase.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            NoPunch = warVase.encounterTrackable.journalData.AmmonomiconFullEntry;
            Punch = "#PSOG_RAT_ROUND_PUNCHOUT_WIN";
            //item.encounterTrackable.journalData.AmmonomiconFullEntry

            SpriteBuilder.AddSpriteToCollection(StaticSpriteDefinitions.Passive_Item_Sheet_Data.GetSpriteDefinition("forgottenroundrat2"), SpriteBuilder.ammonomiconCollection);


            var dataItem = AmmonomiconAPI.HelperTools.CreateJournalEntryData("Forgotten Round", "forgottenroundrat2", "Forgotten Third Chamber", "This exceptional, yet false artifact indicates mastery of the third hidden chamber.\n\nA crown worthy of an unbeatable fighter.", false);
            var en = AmmonomiconAPI.HelperTools.CreateDummyEncounterDatabaseEntry(dataItem, "ratMasterySecondary");

            EncounterDatabase.Instance.Entries.Add(en);
        }
        public static string NoPunch;
        public static string Punch;

        public void SetSprite(bool isPunchedout)
        {
            if (isPunchedout)
            {
                this.sprite.SetSprite("forgottenroundrat2");
                this.encounterTrackable.journalData.AmmonomiconFullEntry = Punch;
                this.encounterTrackable.journalData.m_cachedAmmonomiconFullEntry = Punch;
                this.encounterTrackable.journalData.AmmonomiconSprite = "forgottenroundrat2";
                this.encounterTrackable.EncounterGuid = "ratMasterySecondary";
            }
            else
            {
                this.sprite.SetSprite("forgottenroundrat1");
                this.encounterTrackable.journalData.AmmonomiconFullEntry = NoPunch;
                this.encounterTrackable.journalData.m_cachedAmmonomiconFullEntry = NoPunch;
                this.encounterTrackable.journalData.AmmonomiconSprite = "forgottenroundrat2";
            }

        }
        /*
        [HarmonyPatch]
        private static class AmmonomiconPageRendererInitializeEquipmentPageLeft
        {
            [HarmonyPatch(typeof(AmmonomiconPageRenderer), nameof(AmmonomiconPageRenderer.InitializeEquipmentPageLeft))]
            [HarmonyILManipulator]
            private static void GameUIAmmoControllerUpdateUIGunIL(ILContext il)
            {
                ETGModConsole.Log("fraeak");

                ILCursor cursor = new ILCursor(il);
                if (!cursor.TryGotoNext(MoveType.After,
                    instr => instr.MatchLdstr("Passive Items Panel")))
                    return;



                ETGModConsole.Log("Holy  Molly!");
                if (!cursor.TryGotoNext(MoveType.Before,
                    instr => instr.MatchLdloc(2)))
                    return;
                ETGModConsole.Log("a");

                //cursor.Emit(OpCodes.Ldloc_3);
                //cursor.Emit(OpCodes.Call, typeof(PunchoutControllerPlaceNotePatch).GetMethod(nameof(ModifyNote), BindingFlags.Static | BindingFlags.NonPublic));
            }
            
        }
        */

        public static int ForgottenRoundRatID;
    }
    public class ForgottenRoundRNG : BasicStatPickup
	{
		public static void Init()
		{
			string name = "Forgotten Round 4";
			GameObject gameObject = new GameObject(name);
			ForgottenRoundRNG warVase = gameObject.AddComponent<ForgottenRoundRNG>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("forgottenroundrng"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Forgotten Fourth Chamber";
			string longDesc = "This extraordinary, yet false artifact indicates mastery of the fourth hidden chamber.\n\nWhile plotting his masters return, Agunim decorated this false round with his own likeness as a means to test his magics, and armanents, on potential sacrifices.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog", "Forgotten Round");
			warVase.quality = PickupObject.ItemQuality.SPECIAL;
			warVase.IsMasteryToken = true;
			warVase.ForcedPositionInAmmonomicon = 4;
            warVase.AddAsChamberGunMastery("PlanetsideOfGunymede", 2048);
			Alexandria.DungeonAPI.MasteryOverrideHandler.RegisterFloorForMasterySpawn(MasteryOverrideHandler.ViableRegisterFloors.RNG);

            SynergyAPI.SynergyBuilder.AddItemToSynergy(warVase, CustomSynergyType.MASTERS_CHAMBERS);

			ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.Damage, 0.1f, StatModifier.ModifyMethod.ADDITIVE);
			ForgottenRoundRNG.ForgottenRoundRNGID = warVase.PickupObjectId;
			EncounterDatabase.GetEntry(warVase.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            CustomActions.OnRewardPedestalDetermineContents += OnMasteryDetermineContents;
        }

        public static void OnMasteryDetermineContents(RewardPedestal pedestal, PlayerController determiner, CustomActions.ValidPedestalContents valids)
        {
            if (pedestal.ContainsMasteryTokenForCurrentLevel())
            {
                if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON)
                {
                    valids.overrideItemPool.Add(new Tuple<int, float>(ForgottenRoundRNG.ForgottenRoundRNGID, 1.25f));
                }
            }
        }
        public static int ForgottenRoundRNGID;
	}
}
