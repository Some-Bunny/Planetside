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
	
	internal class InitialiseSynergies
	{
		public static void DoInitialisation()
		{
			List<string> mandatoryConsoleIDs2 = new List<string>
		    {
			"psog:hardlight_nailgun",
			"nail_gun"
			};
			CustomSynergies.Add("Stop!", mandatoryConsoleIDs2, null, true);
			List<string> ee = new List<string>
			{
			"psog:shock-chain",
			"thunderclap"
			};

			CustomSynergies.Add("UNLIMITED POWER!!!", ee, null, true);

			List<string> SwanOff = new List<string>
			{
			"psog:swan-off"
			};
			List<string> SwanOff1 = new List<string>
			{
			"stuffed_star",
			"wax_wings",
			"silencer",
			"sponge"
			};

			CustomSynergies.Add("Ugly Duckling", SwanOff, SwanOff1, true);


		}
	}
}

