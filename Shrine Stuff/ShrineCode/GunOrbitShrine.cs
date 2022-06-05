
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.ShrineFactory;
using Dungeonator;
using System.Collections;

namespace Planetside
{
	public static class GunOrbitShrine
	{
		public static void Add()
		{
			ShrineFactory OHFUUCK = new ShrineFactory
			{

				name = "GunOrbitingShrine",
				modID = "psog",
				text = "A shrine dedicated to an old gunslinger, who could sling guns with such proficiency that they fired while in mid-air.",
				spritePath = "Planetside/Resources/Shrines/GunOrbitShrinel.png",
				//room = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/GunOrbitShrineRoom.room").room,
				RoomWeight = 2f,
				acceptText = "Grant an offering to bestow similar power.",
				declineText = "Leave",
				OnAccept = Accept,
				OnDecline = null,
				CanUse = CanUse,
				offset = new Vector3(-1f, -1f, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,
				shadowPath = "Planetside/Resources/Shrines/defaultShrineShadow.png",
				ShadowOffsetX = 0.3125f,
				ShadowOffsetY = -0.25f,

				preRequisites = new DungeonPrerequisite[0],
				ShrinePercentageChance = 0.2f,
			};
			//register shrine
			GunShrine = OHFUUCK.BuildWithoutBaseGameInterference();
		}
		public static GameObject GunShrine;


		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			Gun currentGun = player.CurrentGun;
			PickupObject.ItemQuality quality = currentGun.quality;
			int armorInt = Convert.ToInt32(player.healthHaver.Armor);	
			float num = 0f;
			num = (player.stats.GetStatValue(PlayerStats.StatType.Health));
			if (currentGun.InfiniteAmmo == false)
            {
				if (player.name == "PlayerShade(Clone)")
				{
					bool DorC = currentGun.quality == PickupObject.ItemQuality.D | currentGun.quality == PickupObject.ItemQuality.C | currentGun.quality == PickupObject.ItemQuality.B;
					if (DorC)
					{
						return shrine.GetComponent<CustomShrineController>().numUses == 0;
					}
					bool Sonly = currentGun.quality == PickupObject.ItemQuality.A | currentGun.quality == PickupObject.ItemQuality.S;
					if (Sonly)
					{
						return shrine.GetComponent<CustomShrineController>().numUses == 0;

					}
				}
				else
				{
					bool DorC = currentGun.quality == PickupObject.ItemQuality.D | currentGun.quality == PickupObject.ItemQuality.C | currentGun.quality == PickupObject.ItemQuality.B;
					if (DorC)
					{
						if (player.characterIdentity == PlayableCharacters.Robot)
						{
							return shrine.GetComponent<CustomShrineController>().numUses == 0 && armorInt > 2;
						}
						else if (player.characterIdentity == PlayableCharacters.Robot)
						{

						}
						else
						{
							return shrine.GetComponent<CustomShrineController>().numUses == 0 && player.stats.GetStatValue(PlayerStats.StatType.Health) > 1;
						}
					}
					bool Sonly = currentGun.quality == PickupObject.ItemQuality.A | currentGun.quality == PickupObject.ItemQuality.S;
					if (Sonly)
					{
						if (player.characterIdentity == PlayableCharacters.Robot)
						{
							return shrine.GetComponent<CustomShrineController>().numUses == 0 && armorInt > 4;
						}
						else
						{
							return shrine.GetComponent<CustomShrineController>().numUses == 0 && player.stats.GetStatValue(PlayerStats.StatType.Health) > 2;
						}
					}
				}
				
			}
			return false;
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{

			Gun currentGun = player.CurrentGun;
			float affectstats = 0f;
			bool DorC = currentGun.quality == PickupObject.ItemQuality.D | currentGun.quality == PickupObject.ItemQuality.C |currentGun.quality == PickupObject.ItemQuality.B ;
			if (DorC)
			{
				affectstats = 1;
			}
			bool Sonly = currentGun.quality == PickupObject.ItemQuality.A | currentGun.quality == PickupObject.ItemQuality.S;
			if (Sonly)
			{
				affectstats = 2;
			}
			if (player.name != "PlayerShade(Clone)")
			{
				StatModifier item = new StatModifier
				{
					statToBoost = PlayerStats.StatType.Health,
					amount = -affectstats,
					modifyType = StatModifier.ModifyMethod.ADDITIVE
				};
				player.ownerlessStatModifiers.Add(item);
			}
			StatModifier item2 = new StatModifier
			{
				statToBoost = PlayerStats.StatType.Curse,
				amount = affectstats,
				modifyType = StatModifier.ModifyMethod.ADDITIVE
			};
			if (player.name != "PlayerShade(Clone)")
            {
				if (player.characterIdentity == PlayableCharacters.Robot)
				{
					player.healthHaver.Armor -= affectstats * 2;
				}

			}
			Gun gun = player.CurrentGun;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, player.CenterPosition.ToVector3ZisY(0f), Quaternion.identity);
			gameObject.transform.parent = player.transform;
			HoveringGunController hover = gameObject.GetComponent<HoveringGunController>();
			hover.ConsumesTargetGunAmmo = false;
			hover.ChanceToConsumeTargetGunAmmo = 0f;
			hover.Position = HoveringGunController.HoverPosition.CIRCULATE;
			hover.Aim = HoveringGunController.AimType.PLAYER_AIM;
			hover.Trigger = HoveringGunController.FireType.ON_RELOAD;
			hover.CooldownTime = 1f;
			hover.ShootDuration = 1f;
			hover.OnlyOnEmptyReload = false;
			hover.Initialize(gun, player);
			player.ownerlessStatModifiers.Add(item2);
			player.stats.RecalculateStats(player, false, false);
			shrine.GetComponent<CustomShrineController>().numUses++;
			shrine.GetComponent<CustomShrineController>().GetRidOfMinimapIcon();
			player.inventory.DestroyCurrentGun();
			AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", shrine);
			if (player.name == "PlayerShade(Clone)")
            {
				ImprovedAfterImage yes = player.gameObject.AddComponent<ImprovedAfterImage>();
				yes.spawnShadows = true;
				yes.shadowLifetime = 0.66f;
				yes.shadowTimeDelay = 0.02f;
				yes.dashColor = Color.clear;
				StatModifier money = new StatModifier
				{
					statToBoost = PlayerStats.StatType.AmmoCapacityMultiplier,
					amount = 0.7f,
					modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
				};
				player.ownerlessStatModifiers.Add(money);

			}
		}
	}
}


