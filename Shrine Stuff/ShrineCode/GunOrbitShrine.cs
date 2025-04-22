
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.ShrineFactory;
using Dungeonator;
using System.Collections;
using DaikonForge.Tween;

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
				text = "A shrine dedicated to a forgotten demi-lich, who practiced the art of gun slinging to a level of mastery previously unheard of.",
				spritePath = "Planetside/Resources/Shrines/GunOrbitShrinel.png",
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
            OHFUUCK.OnPreInteract += (player, shrineController) =>
            {

                if (shrineController.GetComponent<CustomShrineController>().numUses == 1)
				{
					shrineController.GetComponent<CustomShrineController>().text = "The spririts that have once inhabited this shrine have now departed.";
                    return;
				}


                shrineController.acceptText = "Grant an offering to bestow similar power.";
                Gun currentGun = player.CurrentGun;
                PickupObject.ItemQuality quality = currentGun.quality;
                int armorInt = Convert.ToInt32(player.healthHaver.Armor);
                float num = (player.stats.GetStatValue(PlayerStats.StatType.Health));

                if (currentGun.InfiniteAmmo == false)
                {

                    if (player.name == "PlayerShade(Clone)")
                    {
                        shrineController.acceptText = "Grant an offering to bestow similar power. <Reduce Max Ammo>";

                    }
                    else
                    {

                        bool DorC = currentGun.quality == PickupObject.ItemQuality.D | currentGun.quality == PickupObject.ItemQuality.C | currentGun.quality == PickupObject.ItemQuality.B;
                        if (DorC)
                        {

                            if (player.ForceZeroHealthState == true)
                            {
                                shrineController.acceptText = "Grant an offering to bestow similar power. <Lose 2 [sprite \"armor_money_icon_001\"]>";
                            }
                            else
                            {

                                shrineController.acceptText = "Grant an offering to bestow similar power. <Remove [sprite \"heart_big_idle_001\"]>";
                            }
                        }
                        bool Sonly = currentGun.quality == PickupObject.ItemQuality.A | currentGun.quality == PickupObject.ItemQuality.S;
                        if (Sonly)
                        {
                            if (player.ForceZeroHealthState == true)
                            {
                                shrineController.acceptText = "Grant an offering to bestow similar power. <Lose 4 [sprite \"armor_money_icon_001\"]>";
                            }
                            else
                            {

                                shrineController.acceptText = "Grant an offering to bestow similar power. <Remove 2 [sprite \"heart_big_idle_001\"]>";
                            }
                        }
                    }

                }
            };
            GunShrine = OHFUUCK.BuildWithoutBaseGameInterference();
			var t = GunShrine.AddComponent<SimpleShrine>();
            
			Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:gunorbitshrine", GunShrine);


			
		}
		public static GameObject GunShrine;


		

		public static bool CanUse(PlayerController player, GameObject shrine)
		{

			//[sprite \"armor_money_icon_001\"] --  [sprite \"heart_big_idle_001\"]
			var shrineController = shrine.GetComponent<CustomShrineController>();
			shrineController.acceptText = "Grant an offering to bestow similar power.";
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
                    shrineController.acceptText = "Grant an offering to bestow similar power. <Reduce Max Ammo>";

                }
                else
				{
					bool DorC = currentGun.quality == PickupObject.ItemQuality.D | currentGun.quality == PickupObject.ItemQuality.C | currentGun.quality == PickupObject.ItemQuality.B;
					if (DorC)
					{
						if (player.characterIdentity == PlayableCharacters.Robot)
						{
                            shrineController.acceptText = "Grant an offering to bestow similar power. <Lose 2 [sprite \"armor_money_icon_001\"]>";
                            return shrine.GetComponent<CustomShrineController>().numUses == 0 && armorInt > 2;
						}
						else if (player.characterIdentity == PlayableCharacters.Robot)
						{

						}
						else
						{
                            shrineController.acceptText = "Grant an offering to bestow similar power. <Remove [sprite \"heart_big_idle_001\"]>";
                            return shrine.GetComponent<CustomShrineController>().numUses == 0 && player.stats.GetStatValue(PlayerStats.StatType.Health) > 1;
						}
					}
					bool Sonly = currentGun.quality == PickupObject.ItemQuality.A | currentGun.quality == PickupObject.ItemQuality.S;
					if (Sonly)
					{
						if (player.characterIdentity == PlayableCharacters.Robot)
						{
                            shrineController.acceptText = "Grant an offering to bestow similar power. <Lose 4 [sprite \"armor_money_icon_001\"]>";
                            return shrine.GetComponent<CustomShrineController>().numUses == 0 && armorInt > 4;
						}
						else
						{
                            shrineController.acceptText = "Grant an offering to bestow similar power. <Remove 2 [sprite \"heart_big_idle_001\"]>";
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
			float affectstats = 1f;
			if (currentGun.quality == PickupObject.ItemQuality.D | currentGun.quality == PickupObject.ItemQuality.C | currentGun.quality == PickupObject.ItemQuality.B)
			{
				affectstats = 1;
			}
			if (currentGun.quality == PickupObject.ItemQuality.A | currentGun.quality == PickupObject.ItemQuality.S)
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
			hover.Trigger = HoveringGunController.FireType.ON_FIRED_GUN;
			hover.CooldownTime = gun.DefaultModule.cooldownTime * 1.75f;
			hover.ShootDuration = ((float)gun.DefaultModule.numberOfShotsInClip * gun.DefaultModule.cooldownTime) * 0.4f;
			hover.OnlyOnEmptyReload = false;
			hover.Initialize(gun, player);
			player.ownerlessStatModifiers.Add(item2);
			shrine.GetComponent<CustomShrineController>().numUses++;
			shrine.GetComponent<CustomShrineController>().GetRidOfMinimapIcon();
			player.inventory.DestroyCurrentGun();
			AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", shrine);
			if (player.name == "PlayerShade(Clone)")
            {
				ImprovedAfterImage yes = player.gameObject.AddComponent<ImprovedAfterImage>();
				yes.spawnShadows = true;
				yes.shadowLifetime = 0.5f;
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
			player.stats.RecalculateStats(player, false, false);
		}
	}
}


