
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.OldShrineFactory;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Brave.BulletScript;
using System.Collections;
using SaveAPI;
using Gungeon;

namespace Planetside
{

	public static class TooLate
	{

		public static void Add()
		{
			OldShrineFactory iei = new OldShrineFactory
			{
				name = "TooLate",
				modID = "psog",
				text = "The note says \"Shop is closed, come back earlier!\"",
				spritePath = "Planetside/Resources/Shrines/toolatesign.png",
				acceptText = "Kick it!",
				declineText = "Leave.",
				OnAccept = Accept,
				OnDecline = null,

				CanUse = CanUse,
				offset = new Vector3(0, 0, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,

			};
			iei.Build();
		}
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses == 0;
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			AkSoundEngine.PostEvent("Play_BOSS_Punchout_Punch_Hit_01", shrine.gameObject);
			UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(178) as Gun).GetComponent<FireOnReloadSynergyProcessor>().DirectedBurstSettings.ProjectileInterface.SpecifiedProjectile.hitEffects.tileMapHorizontal.effects[0].effects[0].effect, shrine.transform.position, Quaternion.identity);
			shrine.GetComponent<CustomShrineController>().numUses++;
			float RNG = UnityEngine.Random.Range(1, 9);
			float RNG2 = UnityEngine.Random.Range(-180, 180);
			for (int i = 0; i < RNG; i++)
            {
				Vector2 Point = MathToolbox.GetUnitOnCircle(((360/RNG)*i)+RNG2, UnityEngine.Random.Range(0.5f, 3));
				LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, shrine.transform.PositionVector2(), Point, UnityEngine.Random.Range(0.5f, 3), false, false, false);
			}
			UnityEngine.GameObject.Destroy(shrine);
		}
	}
}



