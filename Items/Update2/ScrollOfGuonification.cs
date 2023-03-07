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

using UnityEngine.Serialization;
using MonoMod.Utils;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;

using System.IO;
using Planetside;
using FullInspector.Internal;

namespace Planetside
{
    public class ResourceGuonMaker : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Scroll Of Guonification";
            //string resourceName = "Planetside/Resources/scrollofguon.png";
            GameObject obj = new GameObject(itemName);
			ResourceGuonMaker activeitem = obj.AddComponent<ResourceGuonMaker>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("scrollofguon"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Protection Potential";
            string longDesc = "Imbues objects with orbiting, protective powers.\n\nA scroll written by a long-dead Guonmancer. Small pieces of paper that have torn off of the scroll due to age float around it passively.";
			activeitem.SetupItem(shortDesc, longDesc, "psog");
			activeitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 0.25f);
			activeitem.consumable = false;
			activeitem.quality = PickupObject.ItemQuality.C;
			activeitem.gameObject.AddComponent<IronsideItemPool>();

			List<string> mandatoryConsoleIDs = new List<string>
			{
				"psog:scroll_of_guonification",
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"heart_holster",
				"heart_lunchbox",
				"heart_locket",
				"heart_bottle",
				"heart_purse",
				"pink_guon_stone",
				"charming_rounds"
			};
			CustomSynergies.Add("More To Hearts", mandatoryConsoleIDs, optionalConsoleIDs, true);
			List<string> ForAmmoSunergy = new List<string>
			{
				"ancient_heros_bandana",
				"ammo_synthesizer",
				"utility_belt",
				"ammo_belt",
				"turkey",
				"holey_grail",
				"magazine_rack"
			};
			CustomSynergies.Add("More To Ammo", mandatoryConsoleIDs, ForAmmoSunergy, true);
			List<string> ForBlankSunergy = new List<string>
			{
				"white_guon_stone",
				"gold_ammolet",
				"chaos_ammolet",
				"lodestone_ammolet",
				"uranium_ammolet",
				"copper_ammolet",
				"frost_ammolet",
				"blank_companions_ring",
				"elder_blank",
				"blank_bullets"
			};
			CustomSynergies.Add("More To Blanks", mandatoryConsoleIDs, ForBlankSunergy, true);
			List<string> ForKeySynergy = new List<string>
			{
				"master_of_unlocking",
				"shelleton_key",
				"akey47",
				"book_of_chest_anatomy"
			};
			CustomSynergies.Add("More To Keys", mandatoryConsoleIDs, ForKeySynergy, true);
			List<string> ForArmorSynergy = new List<string>
			{
				"ac15",
				"armor_synthesizer",
				"gunknight_helmet",
				"gunknight_greaves",
				"gunknight_gauntlet",
				"gunknight_armor"
			};
			CustomSynergies.Add("More To Armor", mandatoryConsoleIDs, ForArmorSynergy, true);
			ResourceGuonMaker.ScrollOfGuonificationID = activeitem.PickupObjectId;
			ItemIDs.AddToList(activeitem.PickupObjectId);

		}
		public static int ScrollOfGuonificationID;
		public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        public override bool CanBeUsed(PlayerController user)
        {
			RoomHandler room = user.GetAbsoluteParentRoom();
			room.GetCenterCell();
			bool flag2 = !user;
			if (flag2)
			{
				return false;
			}
			else
			{
                {
					List<DebrisObject> allDebris = StaticReferenceManager.AllDebris;
					bool flag3 = allDebris != null;
					if (flag3)
					{
						for (int i = 0; i < allDebris.Count; i++)
						{
							DebrisObject debrisObject = allDebris[i];
							bool flag4 = debrisObject && debrisObject.IsPickupObject;
							if (flag4)
							{
								float sqrMagnitude = (user.CenterPosition - debrisObject.transform.position.XY()).sqrMagnitude;
								bool flag5 = sqrMagnitude <= 25f;
								if (flag5)
								{
									HealthPickup component = debrisObject.GetComponent<HealthPickup>();
									AmmoPickup component2 = debrisObject.GetComponent<AmmoPickup>();
									KeyBulletPickup component3 = debrisObject.GetComponent<KeyBulletPickup>();
									SilencerItem component4 = debrisObject.GetComponent<SilencerItem>();
									bool flag6 = (component && component.armorAmount == 0 && (component.healAmount == 0.5f || component.healAmount == 1f)) || component2 || component3 || component4 || component && component.armorAmount >= 0;// | guon.PickupObjectId == 565 && guon.GetComponent<IounStoneOrbitalItem>() != null;
									if (flag6)
									{
										float num = Mathf.Sqrt(sqrMagnitude);
										bool flag7 = num < 2f;
										if (flag7)
										{
											return true;
										}
									}
								}
							}
						}
					}
					bool flag8 = user;
					if (flag8)
					{
						IPlayerInteractable lastInteractable = user.GetLastInteractable();
						bool flag9 = lastInteractable is HeartDispenser;
						if (flag9)
						{
							HeartDispenser exists = lastInteractable as HeartDispenser;
							bool flag10 = exists && HeartDispenser.CurrentHalfHeartsStored > 0;
							if (flag10)
							{
								return true;
							}
						}
						else if (lastInteractable is IounStoneOrbitalItem)
                        {
							int ID = (lastInteractable as PickupObject).PickupObjectId;
							if (ID == 565)
							{
								return true;
							}
						}
					}
				}
				return false;
			}
		}

        public override void DoEffect(PlayerController user)
        {
			AkSoundEngine.PostEvent("Play_ENM_wizardred_vanish_01", base.gameObject);
			IPlayerInteractable lastInteractable = user.GetLastInteractable();
			bool flag2 = lastInteractable is HeartDispenser;
			if (flag2)
			{
				HeartDispenser exists = lastInteractable as HeartDispenser;
				bool flag3 = exists && HeartDispenser.CurrentHalfHeartsStored > 0;
				if (flag3)
				{
					bool flag4 = HeartDispenser.CurrentHalfHeartsStored > 1;
					if (flag4)
					{
						HeartDispenser.CurrentHalfHeartsStored -= 2;
					}
					else
					{
						HeartDispenser.CurrentHalfHeartsStored--;
					}
					return;
				}
			}
			else
            {
				if (lastInteractable is IounStoneOrbitalItem)
				{
					int ID = (lastInteractable as PickupObject).PickupObjectId;
					if (ID == 565)
					{
						AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.SHATTER_GLASSGUON, true);
						PickupObject pick = lastInteractable as PickupObject;
						Exploder.DoDistortionWave(pick.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
						AkSoundEngine.PostEvent("Play_WPN_blackhole_shot_01", base.gameObject);
						AkSoundEngine.PostEvent("Play_ENV_time_shatter_01", base.gameObject);
						LootEngine.DoDefaultSynergyPoof(pick.transform.position, false);

						ResourceGuonMaker component = base.gameObject.GetComponent<ResourceGuonMaker>();
						ResourceGuonMaker.HoleObject = PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>();

						component.synergyobject = ResourceGuonMaker.HoleObject.objectToSpawn;
						BlackHoleDoer component2 = this.synergyobject.GetComponent<BlackHoleDoer>();

						GameObject onj = UnityEngine.Object.Instantiate<GameObject>(component2.HellSynergyVFX, new Vector3(base.transform.position.x + 1f, base.transform.position.y, base.transform.position.z + 5f), Quaternion.Euler(0f, 0f, 0f));
						MeshRenderer component3 = onj.GetComponent<MeshRenderer>();
						base.StartCoroutine(this.HoldPortalOpen(component3, pick.transform.position, user));
						UnityEngine.Object.Destroy(pick.gameObject);
						component3.material.SetTexture("_PortalTex", StaticTextures.NebulaTexture);
						hole = component3;
					}
				}
			}
			bool flag5 = StaticReferenceManager.AllDebris != null;
			if (flag5)
			{
				DebrisObject debrisObject = null;
				float num = float.MaxValue;
				for (int i = 0; i < StaticReferenceManager.AllDebris.Count; i++)
				{
					DebrisObject debrisObject2 = StaticReferenceManager.AllDebris[i];
					bool isPickupObject = debrisObject2.IsPickupObject;
					if (isPickupObject)
					{
						float sqrMagnitude = (user.CenterPosition - debrisObject2.transform.position.XY()).sqrMagnitude;
						bool flag6 = sqrMagnitude <= 25f;
						if (flag6)
						{
							HealthPickup component = debrisObject2.GetComponent<HealthPickup>();
							AmmoPickup component2 = debrisObject2.GetComponent<AmmoPickup>();
							KeyBulletPickup component3 = debrisObject2.GetComponent<KeyBulletPickup>();
							SilencerItem component4 = debrisObject2.GetComponent<SilencerItem>();

							bool flag7 = (component && component.armorAmount == 0 && (component.healAmount == 0.5f || component.healAmount == 1f)) || component2 || component3 || component4 || component && component.armorAmount >= 0;// | guon.PickupObjectId == 565 && guon.GetComponent<IounStoneOrbitalItem>() != null;
							if (flag7)
							{
								float num2 = Mathf.Sqrt(sqrMagnitude);
								bool flag8 = num2 < num && num2 < 2f;
								if (flag8)
								{
									num = num2;
									debrisObject = debrisObject2;
								}
							}
						}
					}
				}
				bool flag9 = debrisObject;
				if (flag9)
				{
					HealthPickup component5 = debrisObject.GetComponent<HealthPickup>();
					AmmoPickup component6 = debrisObject.GetComponent<AmmoPickup>();
					KeyBulletPickup component7 = debrisObject.GetComponent<KeyBulletPickup>();
					SilencerItem component8 = debrisObject.GetComponent<SilencerItem>();
					bool flag10 = component5;
					if (flag10)
					{

						bool flag12 = component5.armorAmount == 0 && component5.healAmount == 0.5f;
						if (flag12)
						{
							LootEngine.DoDefaultItemPoof(component5.transform.position, false, false);
							GameObject orb = PlayerOrbitalItem.CreateOrbital(user, RandomPiecesOfStuffToInitialise.HalfheartGuon, false);
							PickupGuonComponent pick = orb.AddComponent<PickupGuonComponent>();
							pick.IsHalfHeart = true;
							pick.player = user;
							if (user.PlayerHasActiveSynergy("More To Hearts"))
							{
								pick.HitsBeforeDeath = 60;
							}
							else
							{
								pick.HitsBeforeDeath = 45;
							}
							UnityEngine.Object.Destroy(component5.gameObject);
						}
						else
						{
							bool flag13 = component5.armorAmount == 0 && component5.healAmount == 1f;
							if (flag13)
							{
								LootEngine.DoDefaultItemPoof(component5.transform.position, false, false);
								GameObject orb = PlayerOrbitalItem.CreateOrbital(user, RandomPiecesOfStuffToInitialise.HeartGuon, false);
								PickupGuonComponent pick = orb.AddComponent<PickupGuonComponent>();
								pick.IsHeart = true;
								pick.player = user;
								if (user.PlayerHasActiveSynergy("More To Hearts"))
								{
									pick.HitsBeforeDeath = 120;
								}
								else
								{
									pick.HitsBeforeDeath = 90;
								}
								UnityEngine.Object.Destroy(component5.gameObject);
							}
							else
							{
								bool h = component5.armorAmount >= 0;
								if (h)
								{
									LootEngine.DoDefaultItemPoof(component5.transform.position, false, false);
									GameObject orb = PlayerOrbitalItem.CreateOrbital(user, RandomPiecesOfStuffToInitialise.ArmorGuon, false);

									PickupGuonComponent pick = orb.AddComponent<PickupGuonComponent>();
									pick.IsArmor = true;
									pick.player = user;
									if (user.PlayerHasActiveSynergy("More To Armor"))
									{
										pick.HitsBeforeDeath = 200;
									}
									else
									{
										pick.HitsBeforeDeath = 150;
									}
									UnityEngine.Object.Destroy(component5.gameObject);
								}
							}
						}
					}
					else
					{
						bool flag14 = component6;
						if (flag14)
						{
							if (component6.PickupObjectId == 78)
							{
								LootEngine.DoDefaultItemPoof(component6.transform.position, false, false);
								GameObject orb = PlayerOrbitalItem.CreateOrbital(user, RandomPiecesOfStuffToInitialise.AmmoGuon, false);
								PickupGuonComponent pick = orb.AddComponent<PickupGuonComponent>();
								pick.IsAmmo = true;
								pick.player = user;

								if (user.PlayerHasActiveSynergy("More To Ammo"))
								{
									pick.HitsBeforeDeath = 100;
								}
								else
								{
									pick.HitsBeforeDeath = 75;
								}

								UnityEngine.Object.Destroy(component6.gameObject);
							}
							else if (component6.PickupObjectId == 600)
							{
								LootEngine.DoDefaultItemPoof(component6.transform.position, false, false);
								GameObject orb = PlayerOrbitalItem.CreateOrbital(user, RandomPiecesOfStuffToInitialise.HalfAmmoGuon, false);
								PickupGuonComponent pick = orb.AddComponent<PickupGuonComponent>();
								pick.player = user;
								pick.IsHalfAmmo = true;
								if (user.PlayerHasActiveSynergy("More To Ammo"))
								{
									pick.HitsBeforeDeath = 100;
								}
								else
								{
									pick.HitsBeforeDeath = 75;
								}
								UnityEngine.Object.Destroy(component6.gameObject);
							}
						}
						else
						{

							bool flag16 = component7;
							if (flag16)
							{
								LootEngine.DoDefaultItemPoof(component7.transform.position, false, false);
								GameObject orb = PlayerOrbitalItem.CreateOrbital(user, RandomPiecesOfStuffToInitialise.KeyGuon, false);
								PickupGuonComponent pick = orb.AddComponent<PickupGuonComponent>();
								pick.player = user;
								pick.IsKey = true;
								if (user.PlayerHasActiveSynergy("More To Keys"))
								{
									pick.HitsBeforeDeath = 160;
								}
								else
								{
									pick.HitsBeforeDeath = 120;
								}
								UnityEngine.Object.Destroy(component7.gameObject);
							}
							else
							{
								bool flag18 = component8;
								if (flag18)
								{
									LootEngine.DoDefaultItemPoof(component8.transform.position, false, false);
									GameObject orb = PlayerOrbitalItem.CreateOrbital(user, RandomPiecesOfStuffToInitialise.BlankGuon, false);
									PickupGuonComponent pick = orb.AddComponent<PickupGuonComponent>();
									pick.player = user;
									pick.IsBlank = true;
									if (user.PlayerHasActiveSynergy("More To Blanks"))
									{
										pick.HitsBeforeDeath = 133;
									}
									else
									{
										pick.HitsBeforeDeath = 100;
									}
									UnityEngine.Object.Destroy(component8.gameObject);
								}
							}
						}
					}
				}
			}
			
		}
		private IEnumerator HoldPortalOpen(MeshRenderer portal, Vector2 pos ,PlayerController player) // this be closing coroutine
		{
			portal.material.SetFloat("_UVDistCutoff", 0.50f);
			float elapsed = 0f;
			float duration = 5;
			float t = 0f;
			while (elapsed < duration)
			{
				if (portal != null)
                {
					t = Mathf.Clamp01(elapsed / 1.25f);
					portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0.0f + elapsed * 1.3f, 0f, t));
					float Rad = portal.material.GetFloat("_UVDistCutoff");
					float num = (player.stats.GetStatValue(PlayerStats.StatType.Damage));
					List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
					Vector2 centerPosition = pos;
					if (activeEnemies != null)
					{
						foreach (AIActor aiactor in activeEnemies)
						{
							bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < Rad * 20 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null;
							if (ae)
							{
								aiactor.healthHaver.ApplyDamage((500f * num) * BraveTime.DeltaTime, Vector2.zero, "fwomp", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
								if (!aiactor.healthHaver.IsBoss)
								{
									aiactor.knockbackDoer.weight = 150f;
									Vector2 a = aiactor.transform.position - portal.transform.position;
									aiactor.knockbackDoer.ApplyKnockback(-a, 1.33f * (Vector2.Distance(portal.transform.position, aiactor.transform.position) + 0.005f), false);
								}
							}
						}
					}
				}
				elapsed += BraveTime.DeltaTime;			
				yield return null;
			}
			if (portal != null)
            {
				Destroy(portal.gameObject);
			}
			yield break;
		}


		MeshRenderer hole;
		private GameObject synergyobject;
		private static SpawnObjectPlayerItem HoleObject;

		public float distortionMaxRadius = 30f;
		public float distortionDuration = 2f;
		public float distortionIntensity = 0.7f;
		public float distortionThickness = 0.1f;
	}
}



