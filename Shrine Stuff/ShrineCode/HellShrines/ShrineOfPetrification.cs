
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
	public static class ShrineOfPetrification
	{

		public static void Add()
		{
			OldShrineFactory aa = new OldShrineFactory
			{

				name = "ShrineOfPetrification",
				modID = "psog",
				text = "A shrine with a petrified bullet kin standing on it. Do you dare?",
				spritePath = "Planetside/Resources/Shrines/HellShrines/shrineofressurection.png",
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
		public static string spriteDefinition1 = "Planetside/Resources/ShrineIcons/PetrifyIcon";
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses <= 0;
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			SimpleShrine simple = shrine.gameObject.GetComponent<SimpleShrine>();
			simple.text = "The spirits that have once inhabited the shrine have departed.";
			LootEngine.DoDefaultPurplePoof(player.specRigidbody.UnitBottomCenter, false);
			CursesController.EnablePetrification();

			//OtherTools.Notify("You Obtained The", "Curse Of Petrification.", "Planetside/Resources/ShrineIcons/PetrifyIcon");
			//AkSoundEngine.PostEvent("Play_ENM_darken_world_01", shrine);
			shrine.GetComponent<CustomShrineController>().numUses++;
			//PetrifyTime dark = player.gameObject.AddComponent<PetrifyTime>();
			//dark.player = player;
		}
		public class PetrifyTime : BraveBehaviour
		{
			public PetrifyTime()
            {this.player = GameManager.Instance.PrimaryPlayer;}
			public void Start()
			{
				player.OnRoomClearEvent += this.RoomCleared;
				ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
			}

			public void RemoveSelf()
			{
				Destroy(this);
			}
			protected override void OnDestroy()
			{
				if (player != null)
				{player.OnRoomClearEvent -= this.RoomCleared;}
				ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Remove(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
				base.OnDestroy();
			}
			public void AIActorMods(AIActor target)
			{
				if (target != null && !OtherTools.BossBlackList.Contains(target.aiActor.EnemyGuid) && !target.healthHaver.IsBoss)
				{target.gameObject.AddComponent<PetrifyThing>();}
			}
			private void RoomCleared(PlayerController obj)
			{
				if (UnityEngine.Random.value <= 0.04f)
				{
					IntVector2 bestRewardLocation = player.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);
					Chest chest2 = GameManager.Instance.RewardManager.SpawnRewardChestAt(bestRewardLocation, -1f, PickupObject.ItemQuality.EXCLUDED);
					chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
				}
			}
			public PlayerController player;
		}
	}
}



