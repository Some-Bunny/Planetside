using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;

namespace Planetside
{
	
	internal class InitialiseGTEE
	{
		public static void DoInitialisation()
		{
		List<string> OOHT = new List<string>
		{
				"heart_holster",
				"mimic_tooth_necklace",
				"bloodied_scarf",
				"backpack",
				"ammo_belt",
				"utility_belt",
				"resourceful_sack",
				"hip_holster"
		};
			Random r1 = new Random();
			int index1 = r1.Next(OOHT.Count);
			string OneToHoldIt = OOHT[index1];
		List<string> OOFT = new List<string>
		{
				"lichy_trigger_finger",
				"ring_of_triggers",
				"backup_gun",
				"bullet_idol",
				"hip_holster",
				"turkey",
				"sunglasses"

		};
			Random r2 = new Random();
			int index2 = r2.Next(OOFT.Count);
			string OneToFireIt = OOFT[index2];

		List<string> OOPT = new List<string>
		{
				"yellow_chamber",
				"drum_clip",
				"oiled_cylinder",
				"bionic_leg",
				"cog_of_battle",
				"bloody_9mm",
				"crisis_stone",
				"ballistic_boots"
				
		};
			Random r3 = new Random();
			int index3 = r3.Next(OOPT.Count);
			string OneToPrimeIt = OOPT[index3];

		List<string> OOTS = new List<string>
		{
			"barrel",
			"makarov",
			"derringer",
			"m1911",
			"saa",
			"mailbox",
			"origuni",
			"snowballer",
			"cactus",
			"bullet",
			"elimentaler"
		};

        List<int> OOTSS = new List<int>
        {
         7,
		 79,
		 378,
		 30,
		 50,
		 28,
		 477,
		 402,
		 124,
		 503,
		 626
        };


            Random r4 = new Random();
			int index4 = r4.Next(OOTS.Count);
			string OneToShootIt = OOTS[index4];
            int OneToShootItID = OOTSS[index4];




            List<string> eeeee = new List<string>
		{
			OneToHoldIt,
			OneToFireIt,
			OneToPrimeIt,
			OneToShootIt
		};

			HOneToShootIt = OneToShootItID;


            CustomSynergies.Add("End Of Everything", eeeee, null, true);


			AdvancedTransformGunSynergyProcessor advancedTransformGunSynergyProcessor = (PickupObjectDatabase.GetById(OneToShootItID) as Gun).gameObject.AddComponent<AdvancedTransformGunSynergyProcessor>();
			EndOfEverything aaaa = (PickupObjectDatabase.GetById(OneToShootItID) as Gun).gameObject.GetOrAddComponent<EndOfEverything>();

			advancedTransformGunSynergyProcessor.NonSynergyGunId = OneToShootItID;
			advancedTransformGunSynergyProcessor.SynergyGunId = GTEE.fuckinGhELL;
			advancedTransformGunSynergyProcessor.SynergyToCheck = "End Of Everything";
			ETGModConsole.Commands.AddUnit("gteecomponents", (args) =>
			{

				PlanetsideModule.Log(OneToHoldIt, TEXT_COLOR_GTEE);
				PlanetsideModule.Log(OneToFireIt, TEXT_COLOR_GTEE);
				PlanetsideModule.Log(OneToPrimeIt, TEXT_COLOR_GTEE);
				PlanetsideModule.Log(OneToShootIt, TEXT_COLOR_GTEE);
			});
			HOneToHoldIt = OneToHoldIt;
			HOneToFireIt = OneToFireIt;
			HOneToPrimeIt = OneToPrimeIt;
		}
        public static int HOneToShootIt;
        public static string HOneToHoldIt;
		public static string HOneToFireIt;
		public static string HOneToPrimeIt;
		public static readonly string TEXT_COLOR_GTEE = "#FFe400";
	}
}

