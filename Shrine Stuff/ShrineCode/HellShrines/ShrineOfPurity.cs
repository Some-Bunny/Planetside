
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.ShrineFactory;
using Gungeon;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;
using SaveAPI;

namespace Planetside
{
	public static class ShrineOfPurity
	{

		public static void Add()
		{
			ShrineFactory aa = new ShrineFactory
			{

				name = "ShrineOfPurity",
				modID = "psog",
				text = "A shrine that purifies a hell-bound curse, at a cost.",
				spritePath = "Planetside/Resources/Shrines/HellShrines/shrineofpurity.png",
				acceptText = "Cleanse a single, unspecified Curse.",
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
		public static string spriteDefinition1 = "Planetside/Resources/ShrineIcons/PurityIcon";
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			if( CursesController.CheckIfAnyCurseActive() == true && !player.IsInCombat)//player.gameObject.GetComponent<ShrineOfDarkness.DarknessTime>() != null || player.gameObject.GetComponent<ShrineOfCurses.JamTime>() != null || player.gameObject.GetComponent<ShrineOfPetrification.PetrifyTime>() != null || player.gameObject.GetComponent<ShrineOfSomething.SomethingTime>() != null)
            {
				return shrine.GetComponent<CustomShrineController>().numUses <= 0;
			}
			else
            {
				return false;
            }
		}



		public static void Accept(PlayerController player, GameObject shrine)
		{
			AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", shrine.gameObject);
			for (int i = 0; i < 4; i++)
			{
				SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(538) as SilverBulletsPassiveItem).SynergyPowerVFX, player.sprite.WorldCenter.ToVector3ZisY(0f) + new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-0.25f, 0.25f), 100), Quaternion.identity).GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(player.sprite.WorldCenter.ToVector3ZisY(0f), tk2dBaseSprite.Anchor.MiddleCenter);
			}
			List<string> list = new List<string> { };
			if (CursesController.DarknessCurseState == CursesController.DarknessCurseStates.ENABLED)
            {
				list.Add("Darkness");
            }
			if (CursesController.JamnationCurseState == CursesController.JamnationCurseStates.ENABLED)
			{
				list.Add("Jam");
			}
			if (CursesController.PetrifyCurseState == CursesController.PetrifyCurseStates.ENABLED)
			{
				list.Add("Petrify");
			}
			if (CursesController.BolsterCurseState == CursesController.BolsterCurseStates.ENABLED)
			{
				list.Add("Bolster");
			}
			string ChosenCurse = BraveUtility.RandomElement<string>(list);
			if (ChosenCurse != null)
			{
				if (ChosenCurse == "Darkness")
				{
					OtherTools.Notify("Curse Of Darkness chosen", "Prove you're worthy.", "Planetside/Resources/ShrineIcons/PurityIcon");
					CursesController.DarknessCurseState = CursesController.DarknessCurseStates.UPGRADED_AND_ONEROOMLEFT;
					//ShrineOfDarkness.DarknessTime comp = player.GetComponent<ShrineOfDarkness.DarknessTime>();
					//comp.RemoveSelf();
					//UltraDarkness darkness = player.gameObject.AddComponent<UltraDarkness>();
					//darkness.playeroue = player;
				}
				if (ChosenCurse == "Jam")
				{
					OtherTools.Notify("Curse Of Jamnation chosen", "Prove you're worthy.", "Planetside/Resources/ShrineIcons/PurityIcon");
					CursesController.JamnationCurseState = CursesController.JamnationCurseStates.UPGRADED_AND_ONEROOMLEFT;

					//ShrineOfCurses.JamTime comp = player.GetComponent<ShrineOfCurses.JamTime>();
					//comp.RemoveSelf();
					//UltraJammed jammed = player.gameObject.AddComponent<UltraJammed>();
					//jammed.playeroue = player;
				}
				if (ChosenCurse == "Petrify")
				{
					OtherTools.Notify("Curse Of Petrification chosen", "Prove you're worthy.", "Planetside/Resources/ShrineIcons/PurityIcon");
					CursesController.PetrifyCurseState = CursesController.PetrifyCurseStates.UPGRADED_AND_ONEROOMLEFT;

					//ShrineOfPetrification.PetrifyTime comp = player.GetComponent<ShrineOfPetrification.PetrifyTime>();
					//comp.RemoveSelf();
					//UltraPetrify petrify = player.gameObject.AddComponent<UltraPetrify>();
					//petrify.playeroue = player;
				}
				if (ChosenCurse == "Bolster")
				{
					OtherTools.Notify("Curse Of Bolstering chosen", "Prove you're worthy.", "Planetside/Resources/ShrineIcons/PurityIcon");
					CursesController.BolsterCurseState = CursesController.BolsterCurseStates.UPGRADED_AND_ONEROOMLEFT;

					//ShrineOfSomething.SomethingTime comp = player.GetComponent<ShrineOfSomething.SomethingTime>();
					//comp.RemoveSelf();
					//UltraBolster bolster = player.gameObject.AddComponent<UltraBolster>();
					//bolster.playeroue = player;
				}
			}
			shrine.GetComponent<CustomShrineController>().numUses++;
		}
		public class UltraDarkness : BraveBehaviour
		{
			public UltraDarkness()
            {
				this.playeroue = base.GetComponent<PlayerController>();
			}
			public void Start()
			{
				playeroue.OnRoomClearEvent += this.RoomCleared;
				playeroue.OnEnteredCombat += this.EnteredCombat;
			}
			private void EnteredCombat()
            {
				AkSoundEngine.PostEvent("Play_ENM_darken_world_01", base.gameObject);
				NevernamedsDarknessHandler.EnableDarkness(15f, 1.5f);
				//RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
				//absoluteRoom.BecomeTerrifyingDarkRoom(1f, 0.15f, 0.3f, "Play_ENM_darken_world_01");
			}

			public override void OnDestroy()
			{
				if (playeroue != null)
				{
					playeroue.OnRoomClearEvent -= this.RoomCleared;
					playeroue.OnEnteredCombat -= this.EnteredCombat;
				}
				base.OnDestroy();
			}

			private void RoomCleared(PlayerController obj)
			{
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEDARKEN, true);
				AkSoundEngine.PostEvent("Play_ENM_lighten_world_01", base.gameObject);
				NevernamedsDarknessHandler.DisableDarkness(1.5f);
				//RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
				//absoluteRoom.EndTerrifyingDarkRoom();
				OtherTools.Notify("Curse Of Darkness cleansed", "You are free now.", "Planetside/Resources/ShrineIcons/PurityIcon");
				playeroue.OnRoomClearEvent -= this.RoomCleared;
				playeroue.OnEnteredCombat -= this.EnteredCombat;
				IntVector2 bestRewardLocation = playeroue.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.CameraCenter, true);
				Chest chest2 = GameManager.Instance.RewardManager.SpawnRewardChestAt(bestRewardLocation, -1f, PickupObject.ItemQuality.EXCLUDED);
				chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
				chest2.IsLocked = false;
				bool A = SaveAPIManager.GetFlag(CustomDungeonFlags.DEBOLSTER);
				bool S = SaveAPIManager.GetFlag(CustomDungeonFlags.DEJAM);
				bool D = SaveAPIManager.GetFlag(CustomDungeonFlags.DEPETRIFY);
				bool F = SaveAPIManager.GetFlag(CustomDungeonFlags.DEDARKEN);
				Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(chest2.sprite);
				if (outlineMaterial1 != null)
				{
					outlineMaterial1.SetColor("_OverrideColor", new Color(30f, 30f, 30f));
				}
				if (A == true && S == true && D == true && F == true)
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK, true);
				}
				Destroy(this);
			}
			public PlayerController playeroue;

		}

		public class UltraJammed : BraveBehaviour
		{
			public UltraJammed()
			{
				this.playeroue = base.GetComponent<PlayerController>();
			}
			public void Start()
			{
				playeroue.OnRoomClearEvent += this.RoomCleared;
				ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
			}

			public override void OnDestroy()
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
				if (!target.healthHaver.IsBoss)
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
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEJAM, true);
				OtherTools.Notify("Curse Of Jamnation cleansed", "You are free now.", "Planetside/Resources/ShrineIcons/PurityIcon");
				playeroue.OnRoomClearEvent -= this.RoomCleared;
				ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Remove(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
				IntVector2 bestRewardLocation = playeroue.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.CameraCenter, true);
				Chest chest2 = GameManager.Instance.RewardManager.SpawnRewardChestAt(bestRewardLocation, -1f, PickupObject.ItemQuality.EXCLUDED);
				chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
				chest2.IsLocked = false;
				bool A = SaveAPIManager.GetFlag(CustomDungeonFlags.DEBOLSTER);
				bool S = SaveAPIManager.GetFlag(CustomDungeonFlags.DEJAM);
				bool D = SaveAPIManager.GetFlag(CustomDungeonFlags.DEPETRIFY);
				bool F = SaveAPIManager.GetFlag(CustomDungeonFlags.DEDARKEN);
				Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(chest2.sprite);
				if (outlineMaterial1 != null)
				{
					outlineMaterial1.SetColor("_OverrideColor", new Color(30f, 30f, 30f));
				}
				if (A == true && S == true && D == true && F == true)
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK, true);
				}
				Destroy(this);
			}
			public PlayerController playeroue;

		}

		public class UltraPetrify : BraveBehaviour
		{
			public UltraPetrify()
			{
				this.playeroue = base.GetComponent<PlayerController>();
			}
			public void Start()
			{
				playeroue.OnRoomClearEvent += this.RoomCleared;
				ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
			}

			public override void OnDestroy()
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
				if (!target.healthHaver.IsBoss)
                {
					if (target != null && !OtherTools.BossBlackList.Contains(target.aiActor.encounterTrackable.EncounterGuid))
					{
						PetrifyThing pet = target.gameObject.AddComponent<PetrifyThing>();
						pet.Time = 17.5f;
					}
				}				
			}
			private void RoomCleared(PlayerController obj)
			{
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEPETRIFY, true);
				OtherTools.Notify("Curse Of Petrification cleansed", "You are free now.", "Planetside/Resources/ShrineIcons/PurityIcon");

				playeroue.OnRoomClearEvent -= this.RoomCleared;
				ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Remove(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
				IntVector2 bestRewardLocation = playeroue.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.CameraCenter, true);
				Chest chest2 = GameManager.Instance.RewardManager.SpawnRewardChestAt(bestRewardLocation, -1f, PickupObject.ItemQuality.EXCLUDED);
				chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
				
				chest2.IsLocked = false;
				bool A = SaveAPIManager.GetFlag(CustomDungeonFlags.DEBOLSTER);
				bool S = SaveAPIManager.GetFlag(CustomDungeonFlags.DEJAM);
				bool D = SaveAPIManager.GetFlag(CustomDungeonFlags.DEPETRIFY);
				bool F = SaveAPIManager.GetFlag(CustomDungeonFlags.DEDARKEN);

				Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(chest2.sprite);
				if (outlineMaterial1 != null)
                {
					outlineMaterial1.SetColor("_OverrideColor", new Color(30f, 30f, 30f));
				}
				if (A == true && S == true && D == true && F == true)
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK, true);
				}
				Destroy(this);
			}
			public PlayerController playeroue;

		}

		public class UltraBolster : BraveBehaviour
		{
			public UltraBolster()
			{
				this.playeroue = base.GetComponent<PlayerController>();
			}
			public void Start()
			{
				playeroue.OnRoomClearEvent += this.RoomCleared;
				ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
			}
			public void AIActorMods(AIActor target)
			{
				if (target != null && !OtherTools.BossBlackList.Contains(target.aiActor.encounterTrackable.EncounterGuid))
				{
					target.behaviorSpeculator.CooldownScale /= 0.25f;
					target.MovementSpeed *= 1.45f;

				}
			}
			public override void OnDestroy()
			{
				if (playeroue != null)
				{
					playeroue.OnRoomClearEvent -= this.RoomCleared;
				}
				ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Remove(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
				base.OnDestroy();
			}

			private void RoomCleared(PlayerController obj)
			{
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEBOLSTER, true);
				OtherTools.Notify("Curse Of Bolstering cleansed", "You are free now.", "Planetside/Resources/ShrineIcons/PurityIcon");
				playeroue.OnRoomClearEvent -= this.RoomCleared;
				ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Remove(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
				IntVector2 bestRewardLocation = playeroue.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.CameraCenter, true);
				Chest chest2 = GameManager.Instance.RewardManager.SpawnRewardChestAt(bestRewardLocation, -1f, PickupObject.ItemQuality.EXCLUDED);
				chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
				chest2.IsLocked = false;
				bool A = SaveAPIManager.GetFlag(CustomDungeonFlags.DEBOLSTER);
				bool S = SaveAPIManager.GetFlag(CustomDungeonFlags.DEJAM);
				bool D = SaveAPIManager.GetFlag(CustomDungeonFlags.DEPETRIFY);
				bool F = SaveAPIManager.GetFlag(CustomDungeonFlags.DEDARKEN);
				Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(chest2.sprite);
				if (outlineMaterial1 != null)
				{
					outlineMaterial1.SetColor("_OverrideColor", new Color(30f, 30f, 30f));
				}

				if (A ==true && S == true && D == true && F == true)
				{
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK, true);
				}
				Destroy(this);
			}
			public PlayerController playeroue;
		}

	}
}



