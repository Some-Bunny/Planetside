
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.OldShrineFactory;
using Gungeon;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;


namespace Planetside
{
	public static class ShrineOfDarkness
	{

		public static void Add()
		{
			OldShrineFactory aa = new OldShrineFactory
			{

				name = "ShrineOfDarkness",
				modID = "psog",
				text = "A shrine with an orb that seems to absorb all light. Do you dare?",
				spritePath = "Planetside/Resources/Shrines/HellShrines/shrineofdarkness.png",
				//room = RoomFactory.BuildFromResource("Planetside/ShrineRooms/ShrineOfEvilShrineRoomHell.room").room,
				//RoomWeight = 200f,
				acceptText = "Dare.",
				declineText = "Leave",
				OnAccept = Accept,
				OnDecline = null,
				CanUse = CanUse,
				offset = new Vector3(-1, -1, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,


			};
			aa.Build();

			SpriteBuilder.AddSpriteToCollection(spriteDefinition1, SpriteBuilder.ammonomiconCollection);

		}
		public static string spriteDefinition1 = "Planetside/Resources/ShrineIcons/DarknessIcon";
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses <= 0;
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			SimpleShrine simple = shrine.gameObject.GetComponent<SimpleShrine>();
			simple.text = "The spirits that have once inhabited the shrine have departed.";
			LootEngine.DoDefaultPurplePoof(player.specRigidbody.UnitBottomCenter, false);
			OtherTools.Notify("You Obtained The", "Curse Of Darkness.", "Planetside/Resources/ShrineIcons/DarknessIcon");
			AkSoundEngine.PostEvent("Play_ENM_darken_world_01", shrine);
			shrine.GetComponent<CustomShrineController>().numUses++;
			DarknessTime dark = player.gameObject.AddComponent<DarknessTime>();
			dark.playeroue = player;
		}
		public class DarknessTime : BraveBehaviour
		{
			public DarknessTime()
            {
				this.playeroue = base.GetComponent<PlayerController>();
			}
			public void Start()
			{
				playeroue.OnRoomClearEvent += this.RoomCleared;
				playeroue.OnEnteredCombat += this.EnteredCombat;
			}
			public void RemoveSelf()
			{
				Destroy(this);
			}
			protected override void OnDestroy()
			{
				if (playeroue != null)
				{
					playeroue.OnRoomClearEvent -= this.RoomCleared;
					playeroue.OnEnteredCombat -= this.EnteredCombat;
				}
				base.OnDestroy();
			}
			private void EnteredCombat()
            {
				List<AIActor> activeEnemies = playeroue.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				bool flag = activeEnemies != null;
				if (flag)
				{
					if (UnityEngine.Random.value <= 0.66f)
					{
						AkSoundEngine.PostEvent("Play_ENM_darken_world_01", base.gameObject);
						NevernamedsDarknessHandler.EnableDarkness(37.5f, 2);
						IsDark = true;
					}
				}
				else{IsDark = false;}
			}

			private void Update()
            {
				if (IsDark == true && playeroue != null && playeroue.IsInCombat != true) {IsDark = false; NevernamedsDarknessHandler.DisableDarkness(1);}
			}
			private void RoomCleared(PlayerController obj)
			{
				if (IsDark == true) { AkSoundEngine.PostEvent("Play_ENM_lighten_world_01", base.gameObject); NevernamedsDarknessHandler.DisableDarkness(1);	}
				if (UnityEngine.Random.value <= 0.03f)
				{
					IntVector2 bestRewardLocation = playeroue.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
					Chest chest2 = GameManager.Instance.RewardManager.SpawnRewardChestAt(bestRewardLocation, -1f, PickupObject.ItemQuality.EXCLUDED);
					chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
				}
			}
			bool IsDark;
			public PlayerController playeroue;

		}
	}
}



