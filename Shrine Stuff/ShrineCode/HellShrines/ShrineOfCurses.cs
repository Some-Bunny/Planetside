
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
	public static class ShrineOfCurses
	{

		public static void Add()
		{
			OldShrineFactory aa = new OldShrineFactory
			{

				name = "ShrineOfCurses",
				modID = "psog",
				text = "A shrine with an icon of the Jammed. Do you dare?",
				spritePath = "Planetside/Resources/Shrines/HellShrines/shrineofthejammed.png",
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
		public static string spriteDefinition1 = "Planetside/Resources/ShrineIcons/JammedIcon";
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses <= 0;
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			SimpleShrine simple = shrine.gameObject.GetComponent<SimpleShrine>();
			simple.text = "The spirits that have once inhabited the shrine have departed.";
			LootEngine.DoDefaultPurplePoof(player.specRigidbody.UnitBottomCenter, false);
			CursesController.EnableJamnation();


			//OtherTools.Notify("You Obtained The", "Curse Of Jamnation.", "Planetside/Resources/ShrineIcons/JammedIcon");
			//AkSoundEngine.PostEvent("Play_ENM_darken_world_01", shrine);
			shrine.GetComponent<CustomShrineController>().numUses++;
			//JamTime dark = player.gameObject.AddComponent<JamTime>();
			//dark.playeroue = player;
		}
		public class JamTime : BraveBehaviour
		{
			public JamTime()
            {
				this.playeroue = base.GetComponent<PlayerController>();
			}
			public void Start()
			{
				playeroue.OnRoomClearEvent += this.RoomCleared;
				ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
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
				}
				ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Remove(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
				base.OnDestroy();
			}
			public void AIActorMods(AIActor target)
			{
				if (target != null && !OtherTools.BossBlackList.Contains(target.aiActor.encounterTrackable.EncounterGuid) && UnityEngine.Random.value <= 0.45f)
				{
					if (!target.IsBlackPhantom)
					{
						target.BecomeBlackPhantom();
					}
					else
					{
						target.gameObject.AddComponent<UmbraController>();
					}
				}
			}
			private void RoomCleared(PlayerController obj)
			{
				if (UnityEngine.Random.value <= 0.03f)
				{
					IntVector2 bestRewardLocation = playeroue.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
					Chest chest2 = GameManager.Instance.RewardManager.SpawnRewardChestAt(bestRewardLocation, -1f, PickupObject.ItemQuality.EXCLUDED);
					chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
				}
			}
			//private RoomHandler Microwave;
			public PlayerController playeroue;
		}
	}
}



