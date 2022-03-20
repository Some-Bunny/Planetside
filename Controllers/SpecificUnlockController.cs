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
using System.Collections.ObjectModel;
using GungeonAPI;
using SaveAPI;

namespace Planetside
{
    public class SpecificUnlockController : MonoBehaviour
    {
        public void Start()
        {
			Debug.Log("Starting SpecificUnlockController setup...");
            try
            {
				new Hook(typeof(HealthHaver).GetMethod("Die"), typeof(SpecificUnlockController).GetMethod("OnHealthHaverDie"));
				Debug.Log("Finished SpecificUnlockController setup without failure!");

			}
			catch (Exception e)
            {
				Debug.Log("Unable to finish SpecificUnlockController setup!");
				Debug.Log(e);
			}
		}

        public static void OnHealthHaverDie(Action<HealthHaver, Vector2> orig, HealthHaver self, Vector2 finalDamageDir)
        {
            orig(self, finalDamageDir);
			if (self.aiActor != null)
            {
				UnlockChecklist(self.aiActor, self.aiActor.IsBlackPhantom, self.aiActor.EnemyGuid);
			}
		}

        public static void UnlockChecklist(AIActor enemy, bool IsJammed, string GUID)
        {
			if (enemy != null)
            {
				if (enemy != null && IsJammed == true && enemy.gameObject.GetComponent<UmbraController>() != null)
				{
					SaveAPIManager.RegisterStatChange(CustomTrackedStats.UMBRAL_ENEMIES_KILLED, 1f);
				}
				if (enemy != null && IsJammed == true && GUID == "arch_gunjurer")
				{
					SaveAPIManager.RegisterStatChange(CustomTrackedStats.JAMMED_ARCHGUNJURERS_KILLED, 1f);
				}
			}
			if (enemy.EnemyGuid == "7c5d5f09911e49b78ae644d2b50ff3bf")
			{
				bool LoopOn = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.LOOPING_ON);

				if (LoopOn == true)
				{
					float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
					if (Loop == 0 || Loop <= 0)
					{
						AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_LOOP_1, true);
						SaveAPIManager.SetStat(CustomTrackedStats.TIMES_LOOPED, 1);
					}
					else
					{
						if (Loop == 1)
						{
							AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_LOOP_1, true);
						}
						SaveAPIManager.RegisterStatChange(CustomTrackedStats.TIMES_LOOPED, 1);
					}
				}
				//Beat Lich With Broken Chamber
				foreach (PlayerController player in GameManager.Instance.AllPlayers)
                {
					if (player.HasPickupID(BrokenChamber.BrokenChamberID))
					{
						AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED, true);
					}
				}

					
			}
			//Dragun Kill Unlocks
			if (enemy.EnemyGuid == "465da2bb086a4a88a803f79fe3a27677")
			{
				foreach (PlayerController player in GameManager.Instance.AllPlayers)
                {
					float num = player.stats.GetStatValue(PlayerStats.StatType.Curse);
					if (num == 15 || num >= 14)
					{
						AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED, true);
					}
				}				
			}
		}
    }
}
