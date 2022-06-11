
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


namespace Planetside
{
	public static class NullShrine
	{

		public static void Add()
		{
			ShrineFactory aa = new ShrineFactory
			{

				name = "NullShrine",
				modID = "psog",
				text = "A shrine of nothings. You feel like somethings missing, or maybe its intentional...",
				spritePath = "Planetside/Resources/Shrines/NullPedestal.png",
				//room = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/NullShrineRoom.room").room,
				RoomWeight = 1.5f,
				acceptText = "Bestow your nothings.",
				declineText = "Leave",
				OnAccept = Accept,
				OnDecline = null,
				CanUse = CanUse,
				offset = new Vector3(-1, -1, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,
				shadowPath = "Planetside/Resources/Shrines/defaultShrineShadow.png",
				ShadowOffsetX = 0f,
				ShadowOffsetY = -0.3125f,

				preRequisites = new DungeonPrerequisite[0],
				ShrinePercentageChance = 0.15f,
			};
			aa.BuildWithoutBaseGameInterference();
			SpriteBuilder.AddSpriteToCollection(spriteDefinition, SpriteBuilder.ammonomiconCollection);
			SpriteBuilder.AddSpriteToCollection(spriteDefinition1, SpriteBuilder.ammonomiconCollection);

		}
		public static string spriteDefinition1 = "Planetside/Resources/ShrineIcons/NullShrineIconKing";
		public static string spriteDefinition = "Planetside/Resources/ShrineIcons/NullShrineIcon";
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			bool canuse = (player.CurrentGun.CurrentAmmo == 0) || (player.carriedConsumables.KeyBullets == 0) || (player.carriedConsumables.Currency == 0) || (player.Blanks == 0);
			if (canuse)
			{
				return shrine.GetComponent<CustomShrineController>().numUses == 0;
			}
			else
            {
				return false;
			}
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			float ChanceToNullKing = 0;
			bool NoBlanks = false;
			bool NoAmmo = false;
			bool NoMoney = false;
			bool NoKeys = false;
			if (player.Blanks == 0)
			{
				StatModifier IncreaseBlanks = new StatModifier
				{
					statToBoost = PlayerStats.StatType.AdditionalBlanksPerFloor,
					amount = 1f,
					modifyType = StatModifier.ModifyMethod.ADDITIVE
				};
				player.ownerlessStatModifiers.Add(IncreaseBlanks);
				ChanceToNullKing += 0.0625f;
				NoBlanks = true;
			}
			if (player.carriedConsumables.Currency == 0)
			{
				StatModifier IncreaseGains = new StatModifier
				{
					statToBoost = PlayerStats.StatType.MoneyMultiplierFromEnemies,
					amount = .15f,
					modifyType = StatModifier.ModifyMethod.ADDITIVE
				};
				player.ownerlessStatModifiers.Add(IncreaseGains);
				ChanceToNullKing += 0.1f;
				NoMoney = true;
			}
			if (player.CurrentGun.CurrentAmmo == 0)
            {
				StatModifier IncreaseAmmo = new StatModifier
				{
					statToBoost = PlayerStats.StatType.AmmoCapacityMultiplier,
					amount = .15f,
					modifyType = StatModifier.ModifyMethod.ADDITIVE
				};
				player.ownerlessStatModifiers.Add(IncreaseAmmo);
				ChanceToNullKing += 0.0625f;
				NoAmmo = true;
			}
			if (player.carriedConsumables.KeyBullets == 0)
			{
				LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(67).gameObject, player);
				ChanceToNullKing += 0.025f;
				NoKeys = true;
			}
			player.stats.RecalculateStats(player, false, false);
			shrine.GetComponent<CustomShrineController>().numUses++;
			shrine.GetComponent<CustomShrineController>().GetRidOfMinimapIcon();
			if (NoKeys == true && NoMoney == true && NoBlanks == true && NoAmmo == true)
            {
				ChanceToNullKing *= 4;
            }

			random = UnityEngine.Random.Range(0.0f, 1.0f);
			if (random <= ChanceToNullKing)
			{
				OtherTools.Notify("You Feel Like The","King Of Nothing." ,"Planetside/Resources/ShrineIcons/NullShrineIconKing");
				player.gameObject.AddComponent<KingOfNulling>();
			}
			else
            {
				OtherTools.Notify("You Feel More", "Fulfilled.", "Planetside/Resources/ShrineIcons/NullShrineIcon");
			}
			AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", shrine);
		}
		private static float random;
		public static int spriteId;
		public class KingOfNulling : BraveBehaviour
		{
			public void Start()
			{
				this.Microwave = base.GetComponent<RoomHandler>();
				this.playeroue = base.GetComponent<PlayerController>();
                {
					PlayerController player = GameManager.Instance.PrimaryPlayer;
					player.OnRoomClearEvent += this.RoomCleared;
				}

			}
			public void Update()
			{

			}
			private void RoomCleared(PlayerController obj)
			{
				bool flag2 = OtherTools.Randomizer(0.1f);
				if (flag2)
				{
					IntVector2 bestRewardLocation2 = obj.CurrentRoom.GetBestRewardLocation(IntVector2.One * 3, RoomHandler.RewardLocationStyle.PlayerCenter, true);

					LootEngine.SpawnItem(PickupObjectDatabase.GetById(NullPickupInteractable.NollahID).gameObject, bestRewardLocation2.ToVector3(), Vector2.up, 1f, true, true, false);
				}
			}
			private RoomHandler Microwave;
			private PlayerController playeroue;

		}

	}
}



