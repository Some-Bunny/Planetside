using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SaveAPI;

namespace Planetside
{
    public class PlanetsideCommands
    {
        public  static void Init()
        {
			global::ETGModConsole.Commands.AddGroup("psog", delegate (string[] args)
			{
				global::ETGModConsole.Log("Please specify a command.", false);
			});
			global::ETGModConsole.Commands.GetGroup("psog").AddUnit("toggleloops", delegate (string[] args)
			{
				bool LoopOn = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.LOOPING_ON);
				if (LoopOn == true)
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.LOOPING_ON, false);
					global::ETGModConsole.Log("Ouroborous Disabled.", false);
				}
				else
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.LOOPING_ON, true);
					float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
					global::ETGModConsole.Log("Ouroborous set to: " + Loop, false);
				}
			});
			global::ETGModConsole.Commands.GetGroup("psog").AddUnit("unlock_all", delegate (string[] args)
			{
				ETGModConsole.Log("<size=100><color=#ff0000ff>*Hits locks with oversized hammer*</color></size>", false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.SHELLRAX_DEFEATED, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BULLETBANK_DEFEATED, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_LOOP_1, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_FUNGANNON, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_OPHANAIM, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HAS_COMPLETED_SOMETHING_WICKED, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.TRESPASS_INTO_OTHER_PLACE, true);
				AdvancedGameStatsManager.Instance.SetStat(CustomTrackedStats.UMBRAL_ENEMIES_KILLED, 10);
				AdvancedGameStatsManager.Instance.SetStat(CustomTrackedStats.JAMMED_ARCHGUNJURERS_KILLED, 20);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED_T4, true);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED, false);

			});
			global::ETGModConsole.Commands.GetGroup("psog").AddUnit("surface", delegate (string[] args)
			{
				ETGModConsole.Log("<size=100><color=#ff0000ff>*Resurfacing...*</color></size>", false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HAS_TREADED_DEEPER, false);
			});

			global::ETGModConsole.Commands.GetGroup("psog").AddUnit("lock_all", delegate (string[] args)
			{
				ETGModConsole.Log("<size=100><color=#ff0000ff>Refitting the locks... don't hit them that hard next time, okay?</color></size>", false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.SHELLRAX_DEFEATED, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BULLETBANK_DEFEATED, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_LOOP_1, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_FUNGANNON, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_OPHANAIM, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HAS_COMPLETED_SOMETHING_WICKED, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.TRESPASS_INTO_OTHER_PLACE, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED_T4, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED, false);

				AdvancedGameStatsManager.Instance.SetStat(CustomTrackedStats.UMBRAL_ENEMIES_KILLED, 0);
				AdvancedGameStatsManager.Instance.SetStat(CustomTrackedStats.JAMMED_ARCHGUNJURERS_KILLED, 0);


				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEJAM, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEPETRIFY, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEDARKEN, false);
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEBOLSTER, false);

			});

			ETGModConsole.Commands.GetGroup("psog").AddUnit("reset_loop", delegate (string[] args)
			{
				float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
				SaveAPIManager.RegisterStatChange(CustomTrackedStats.TIMES_LOOPED, Loop - (Loop * 2));
				ETGModConsole.Log("Current Loop: " + SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED).ToString());
			});

			ETGModConsole.Commands.GetGroup("psog").AddUnit("set_loop", delegate (string[] args)
			{
				float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
				SaveAPIManager.RegisterStatChange(CustomTrackedStats.TIMES_LOOPED, float.Parse(args[0]) - Loop);
				ETGModConsole.Log("Current Loop: " + SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED).ToString());
			});
			ETGModConsole.Commands.GetGroup("psog").AddUnit("current_loop", delegate (string[] args)
			{
				ETGModConsole.Log("Current Loop: " + SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED).ToString());
			});


			global::ETGModConsole.Commands.GetGroup("psog").AddUnit("to_do_list", delegate (string[] args)
			{
				string a = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED) ? " Done!\n" : " -Defeat The Dragun At A Higher Curse.\n";
				string b = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED) ? " Done!\n" : " -Defeat The Guardian Of The Holy Chamber.\n";
				string c = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.SHELLRAX_DEFEATED) ? " Done!\n" : " -Defeat The Failed Demi-Lich\n";
				string d = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BULLETBANK_DEFEATED) ? " Done!\n" : " -Defeat The Banker Of Bullets.\n";
				string e = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED) ? " Done!\n" : " -Defeat The Lich With A Broken Remnant In Hand.\n";
				string f = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_LOOP_1) ? " Done!\n" : " -Beat The Game On Ouroborous Level 0.\n";
				string g = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND) ? " Done!\n" : " -Kill A Boss After Dealing 500 Damage Or More At Once.\n";
				string h = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEFEAT_FUNGANNON) ? " Done!\n" : " -Defeat The Fungal Beast Of The Sewers.\n";
				string i = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEFEAT_OPHANAIM) ? " Done!\n" : " -Defeat The Eternal Eye Of The Abbey.\n";
				string j = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER) ? " Done!\n" : " -Defeat A Ravenous, Violent Chamber.\n";
				string k = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK) ? " Done!\n" : " -Remove Each Hell-Bound Curse At Least Once.\n";
				string l = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HAS_COMPLETED_SOMETHING_WICKED) ? " Done!\n" : " -Survive An Encounter With Something Wicked.\n";
				string m = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.TRESPASS_INTO_OTHER_PLACE) ? " Done!\n" : " -Trespass Into Somewhere Else.\n";
				string n = AdvancedGameStatsManager.Instance.GetPlayerStatValue(CustomTrackedStats.UMBRAL_ENEMIES_KILLED) >= 4 ? " Done!\n" : " -Slay 5 Umbral Enemies.\n";
				string o = AdvancedGameStatsManager.Instance.GetPlayerStatValue(CustomTrackedStats.JAMMED_ARCHGUNJURERS_KILLED) >= 14 ? " Done!\n" : " -Defeat 15 Jammed Arch Gunjurers.\n";
				string p = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED) ? " Done!\n" : " -Perform Maintenance On The Damaged Robot.\n";
				string q = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.HM_PRIME_DEFEATED_T4) ? " Done!\n" : " -Perform the Highest Level Maintenance On The Damaged Robot.\n";

				string color1 = "9006FF";
				OtherTools.PrintNoID("Unlock List:\n" + a + b + c + d + e + f + g + h + i + j + k + l + m + n + o + p+q, color1);
			});

			ETGModConsole.Commands.GetGroup("psog").AddUnit("help", delegate (string[] args)
			{
				string color1 = "9006FF";
				OtherTools.PrintNoID("List Of Commands:", color1);
				OtherTools.PrintNoID("=========.", color1);
				OtherTools.PrintNoID("psog toggleloops" + ": Enables/Disables Ouroborous mode.", color1);
				OtherTools.PrintNoID("psog current_loop" + ": Displays the current player loop.", color1);
				OtherTools.PrintNoID("psog set_loop" + ": Sets the current loop to a given number.", color1);
				OtherTools.PrintNoID("psog reset_loop" + ": Sets the loop to 0.", color1);
				OtherTools.PrintNoID("=========.", color1);
				OtherTools.PrintNoID("psog to_do_list" + ": Displays all unlock conditions.", color1);
				OtherTools.PrintNoID("psog lock_all" + ": Forces all Planetside unlocks to be locked.", color1);
				OtherTools.PrintNoID("psog unlock_all" + ": Forces all Planetside unlocks to be unlocked.", color1);
				OtherTools.PrintNoID("=========.", color1);

				OtherTools.PrintNoID("psog set_item_weight" + ": Changes how often Planetside items and guns appear to a given value.", color1);

			});


			/*
			global::ETGModConsole.Commands.GetGroup("psog").AddUnit("uitoggle", delegate (string[] args)
			{
				if (!disabled)
				{
					ETGModConsole.Log("Ui is disabled");
					GameUIRoot.Instance.HideCoreUI("disabled");
					GameUIRoot.Instance.ForceHideGunPanel = true;
					GameUIRoot.Instance.ForceHideItemPanel = true;
				}
			});
			*/
		}
    }
}
