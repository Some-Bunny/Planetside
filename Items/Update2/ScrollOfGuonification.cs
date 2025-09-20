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
using Alexandria.PrefabAPI;
using Planetside.Toolboxes;
using static UnityEngine.UI.GridLayoutGroup;

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
            string longDesc = "Imbues pickups with orbital powers.\n\nA scroll written by a long-dead Guonmancer. Small pieces of paper that have torn off of the scroll due to age float around it passively.";
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

            List<PickupObject> ForCreditSynergy = new List<PickupObject>
            {
                Items.Briefcase_Of_Cash,
				Items.Brick_Of_Cash,
				Items.Coin_Crown,
				Items.Loot_Bag,
				Guns.Microtransaction_Gun
            };
            ImprovedSynergySetup.Add("More To Greed", new List<PickupObject>() { activeitem }, ForCreditSynergy, true);

            ResourceGuonMaker.ScrollOfGuonificationID = activeitem.PickupObjectId;
			ItemIDs.AddToList(activeitem.PickupObjectId);
            BuildGuonDummy();
        }
		public static int ScrollOfGuonificationID;




        public static void BuildGuonDummy()
        {
            GameObject gameObject = PrefabBuilder.BuildObject("Guon Pickup Orbital");

            tk2dSprite sprite = gameObject.AddComponent<tk2dSprite>();
            sprite.collection = StaticSpriteDefinitions.Guon_Sheet_Data;
            sprite.SetSprite(StaticSpriteDefinitions.Guon_Sheet_Data.GetSpriteIdByName("heartguon_001"));

            tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Guon_Animation_Data;
            animator.defaultClipId = StaticSpriteDefinitions.Guon_Animation_Data.GetClipIdByName("heartguon_idle");
            animator.playAutomatically = true;
            sprite.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;


            SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
            PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
            speculativeRigidbody.CollideWithTileMap = false;
            speculativeRigidbody.CollideWithOthers = true;
            speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
            orbitalPrefab.shouldRotate = false;
            orbitalPrefab.orbitRadius = 2f;
            orbitalPrefab.orbitDegreesPerSecond = 60;
            //orbitalPrefab.perfectOrbitalFactor = 1000f;
            orbitalPrefab.SetOrbitalTier(0);
			var pickup  = orbitalPrefab.AddComponent<PickupGuonComponent>();
			pickup.Orbital = orbitalPrefab;
            GuonDummy = gameObject;

        }



        public static GameObject GuonDummy;



        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

		private PickupObject _PickupNear = null;
        private HeartDispenser _Dispenser = null;
        private IounStoneOrbitalItem _Orbital = null;

		float E;

        public override void Update()
        {
            base.Update();
			if (base.LastOwner)
			{		
				if (_PickupNear)
                {
					E += 240 * BraveTime.DeltaTime;
                    var m =  MathToolbox.GetUnitOnCircle(E, 0.625f);
                    GlobalSparksDoer.DoSingleParticle(_PickupNear.sprite.WorldCenter + m, Vector3.up * UnityEngine.Random.Range(0.2f, 0.4f), 0.1f, 0.5f, Color.cyan, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    return;
				}
                if (_Orbital)
                {
                    E += 240 * BraveTime.DeltaTime;
                    var m = _Orbital.sprite.WorldCenter + MathToolbox.GetUnitOnCircle(E, 0.625f);
                    GlobalSparksDoer.DoSingleParticle(_PickupNear.sprite.WorldCenter + m, Vector3.up * UnityEngine.Random.Range(0.2f, 0.4f), 0.1f, 0.5f, Color.cyan, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    return;
                }
                if (_Dispenser)
                {
                    E += 120 * BraveTime.DeltaTime;
                    var m = MathToolbox.GetUnitOnCircle(E, 1.5f);
                    GlobalSparksDoer.DoSingleParticle(_Dispenser.sprite.WorldCenter + m, Vector3.up * UnityEngine.Random.Range(0.2f, 0.4f), 0.1f, 0.5f, Color.cyan, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    GlobalSparksDoer.DoSingleParticle(_Dispenser.sprite.WorldCenter + (m * -1), Vector3.up * UnityEngine.Random.Range(0.2f, 0.4f), 0.1f, 0.5f, Color.cyan, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    return;
                }
            }
			else
			{
				_PickupNear = null;
				_Dispenser = null;
				_Orbital = null;
            }
        }


        public override bool CanBeUsed(PlayerController user)
        {
			if (user == null) { return false; }	

			RoomHandler room = user.GetAbsoluteParentRoom();
			if (room == null)
			{
				return false;
			}
			else
			{
                List<DebrisObject> allDebris = StaticReferenceManager.AllDebris;
                if (allDebris != null)
                {
                    for (int i = 0; i < allDebris.Count; i++)
                    {
                        DebrisObject debrisObject = allDebris[i];
                        if (debrisObject && debrisObject.IsPickupObject)
                        {
                            if ((user.CenterPosition - debrisObject.transform.position.XY()).sqrMagnitude <= 2.5f)
                            {
                                var Pickup = debrisObject.GetComponent<PickupObject>();

                                if (Pickup is HealthPickup | Pickup is AmmoPickup | Pickup is KeyBulletPickup | Pickup is SilencerItem)
                                {
                                    _PickupNear = Pickup;
                                    return true;

                                }
                                if (Pickup is CurrencyPickup currency)
                                {
                                    if (currency.IsMetaCurrency)
                                    {
                                        _PickupNear = Pickup;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                _PickupNear = null;
                IPlayerInteractable lastInteractable = user.GetLastInteractable();
                if (lastInteractable != null && lastInteractable is HeartDispenser Dispenser)
                {
                    if (HeartDispenser.CurrentHalfHeartsStored > 0)
                    {
                        _Dispenser = Dispenser;
                        return true;
                    }
                }
				_Dispenser = null;

                if (lastInteractable is IounStoneOrbitalItem item)
                {
                    if (item.PickupObjectId == 565)
                    {
						_Orbital = item;
                        return true;
                    }
                }
				_Orbital = null;
                return false;
			}
		}

        public override void DoEffect(PlayerController user)
        {
			AkSoundEngine.PostEvent("Play_ENM_wizardred_vanish_01", base.gameObject);

			if (_Orbital != null)
			{
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.SHATTER_GLASSGUON, true);
                Exploder.DoDistortionWave(_Orbital.sprite.WorldTopCenter, 30, 2, 0.7f, 0.1f);
                LootEngine.DoDefaultSynergyPoof(_Orbital.transform.position, false);
                AkSoundEngine.PostEvent("Play_WPN_blackhole_shot_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_ENV_time_shatter_01", base.gameObject);
                var HoleObject = PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>();
                var synergyobject = HoleObject.objectToSpawn;
                BlackHoleDoer component2 = synergyobject.GetComponent<BlackHoleDoer>();
                GameObject onj = UnityEngine.Object.Instantiate<GameObject>(component2.HellSynergyVFX, new Vector3(base.transform.position.x + 1f, base.transform.position.y, base.transform.position.z + 5f), Quaternion.Euler(0f, 0f, 0f));
                MeshRenderer component3 = onj.GetComponent<MeshRenderer>();
                base.StartCoroutine(this.HoldPortalOpen(onj.GetComponent<MeshRenderer>(), _Orbital.transform.position, user));
                component3.material.SetTexture("_PortalTex", StaticTextures.NebulaTexture);
                UnityEngine.Object.Destroy(_Orbital.gameObject);
				_Orbital = null;
                return;
			}
			if (_Dispenser != null)
			{
                if (HeartDispenser.CurrentHalfHeartsStored > 1)
                {
                    HeartDispenser.CurrentHalfHeartsStored -= 2;

                    LootEngine.DoDefaultItemPoof(_Dispenser.transform.position, false, false);
                    GameObject orb = PlayerOrbitalItem.CreateOrbital(user, GuonDummy, false);
                    PickupGuonComponent pick = orb.GetComponent<PickupGuonComponent>();
                    pick.PlayerOwner = user;
                    pick.pickupType = PickupGuonComponent.PickupType.HEART;
                    pick.Orbital.SetOrbitalTier(21);

                }
                else
                {
                    HeartDispenser.CurrentHalfHeartsStored--;
                    LootEngine.DoDefaultItemPoof(_Dispenser.transform.position, false, false);
                    GameObject orb = PlayerOrbitalItem.CreateOrbital(user, GuonDummy, false);
                    PickupGuonComponent pick = orb.GetComponent<PickupGuonComponent>();
                    pick.PlayerOwner = user;
                    pick.pickupType = PickupGuonComponent.PickupType.HALF_HEART;
                    pick.Orbital.SetOrbitalTier(22);
                }
                return;
            }


			if (_PickupNear != null)
			{
				if (_PickupNear is HealthPickup health)
				{
                    LootEngine.DoDefaultItemPoof(_PickupNear.transform.position, false, false);
                    GameObject orb = PlayerOrbitalItem.CreateOrbital(user, GuonDummy, false);
                    PickupGuonComponent pick = orb.GetComponent<PickupGuonComponent>();
                    pick.PlayerOwner = user;
					pick.pickupType = health.armorAmount > 0 ? PickupGuonComponent.PickupType.ARMOR : health.healAmount > 0.5f ? PickupGuonComponent.PickupType.HEART : PickupGuonComponent.PickupType.HALF_HEART;
                    pick.Orbital.SetOrbitalTier(health.armorAmount > 0 ? 23: health.healAmount > 0.5f ? 21 : 22);
                    Destroy(_PickupNear.gameObject);
                }
                if (_PickupNear is AmmoPickup Ammo)
                {
                    LootEngine.DoDefaultItemPoof(_PickupNear.transform.position, false, false);
                    GameObject orb = PlayerOrbitalItem.CreateOrbital(user, GuonDummy, false);
                    PickupGuonComponent pick = orb.GetComponent<PickupGuonComponent>();
                    pick.PlayerOwner = user;
					pick.pickupType = Ammo.PickupObjectId == 600 ? PickupGuonComponent.PickupType.HALF_AMMO : PickupGuonComponent.PickupType.AMMO;
                    pick.Orbital.SetOrbitalTier(Ammo.PickupObjectId == 600 ? 25 : 24);
                    Destroy(_PickupNear.gameObject);
                }

                if (_PickupNear is KeyBulletPickup key)
                {
                    LootEngine.DoDefaultItemPoof(_PickupNear.transform.position, false, false);
                    GameObject orb = PlayerOrbitalItem.CreateOrbital(user, GuonDummy, false);
                    PickupGuonComponent pick = orb.GetComponent<PickupGuonComponent>();
                    pick.PlayerOwner = user;
					pick.pickupType = PickupGuonComponent.PickupType.KEY;
                    pick.Orbital.SetOrbitalTier(26);

                    Destroy(_PickupNear.gameObject);
                }
                if (_PickupNear is SilencerItem Blank)
                {
                    LootEngine.DoDefaultItemPoof(_PickupNear.transform.position, false, false);
                    GameObject orb = PlayerOrbitalItem.CreateOrbital(user, GuonDummy, false);
                    PickupGuonComponent pick = orb.GetComponent<PickupGuonComponent>();
                    pick.PlayerOwner = user;
					pick.pickupType = PickupGuonComponent.PickupType.BLANK;
                    pick.Orbital.SetOrbitalTier(27);

                    Destroy(_PickupNear.gameObject);
                }
                if (_PickupNear is CurrencyPickup Pickup)
                {
                    LootEngine.DoDefaultItemPoof(_PickupNear.transform.position, false, false);
                    GameObject orb = PlayerOrbitalItem.CreateOrbital(user, GuonDummy, false);
                    PickupGuonComponent pick = orb.GetComponent<PickupGuonComponent>();
                    pick.PlayerOwner = user;
					pick.pickupType = PickupGuonComponent.PickupType.CREDIT;
                    pick.Orbital.SetOrbitalTier(28);

                    Destroy(_PickupNear.gameObject);
                }
                return;
            }

          
        }
		private IEnumerator HoldPortalOpen(MeshRenderer portal, Vector2 pos ,PlayerController player) // this be closing coroutine
		{
			portal.material.SetFloat("_UVDistCutoff", 0.50f);
			float elapsed = 0f;
			float duration = 5;
			float t = 0f;
            float num = (player.stats.GetStatValue(PlayerStats.StatType.Damage));
            while (elapsed < duration)
			{
				if (portal != null)
                {
					t = Mathf.Clamp01(elapsed / 1.25f);
					portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0.0f + elapsed * 1.3f, 0f, t));
					float Rad = portal.material.GetFloat("_UVDistCutoff");
					List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
					Vector2 centerPosition = pos;
					if (activeEnemies != null)
					{
						foreach (AIActor aiactor in activeEnemies)
						{
							if (Vector2.Distance(aiactor.CenterPosition, centerPosition) < Rad * 20 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null)
							{
								aiactor.healthHaver.ApplyDamage((500f * num) * BraveTime.DeltaTime, Vector2.zero, "fwomp", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
								if (!aiactor.healthHaver.IsBoss)
								{
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


	}
}



