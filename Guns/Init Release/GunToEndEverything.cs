﻿using System;
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
using Alexandria.Assetbundle;

namespace Planetside
{
	public class GTEE : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Gun To End Everything", "yourhonordeath");
			Game.Items.Rename("outdated_gun_mods:gun_to_end_everything", "@:gun_to_end_everything");
			gun.gameObject.AddComponent<GTEE>();
			gun.SetShortDescription("Broken Shell");
			gun.SetLongDescription("You are heavily advised not to fire this gun.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "yourhonordeath_fire_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.reloadAnimation = "GTEE_win";
            gun.idleAnimation = "GTEE_win";
            gun.shootAnimation = "GTEE_win";


            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_golddoublebarrelshotgun_shot_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;


			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(387) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 0f;
			gun.DefaultModule.cooldownTime = .01f;
			gun.DefaultModule.numberOfShotsInClip = 1;
			gun.SetBaseMaxAmmo(1);
			gun.quality = PickupObject.ItemQuality.EXCLUDED;
			gun.DefaultModule.angleVariance = 0f;
			gun.DefaultModule.burstShotCount = 1;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.finalCustomAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("GTEE", StaticSpriteDefinitions.PlanetsideUIAtlas, "EndOfEverythingClip_001", "EndOfEverythingClip_002");

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 1f;
			projectile.baseData.speed *= 1f;
			projectile.AdditionalScaleMultiplier *= 1f;
			projectile.shouldRotate = true;
			EndOfEverything fuck = projectile.GetComponent<EndOfEverything>();
			gun.encounterTrackable.EncounterGuid = "Haha gun go *ends world*";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			GTEE.fuckinGhELL = gun.PickupObjectId;

		}
		public static int fuckinGhELL;

		private bool HasReloaded;

		public override void Update()
		{
			if (gun.CurrentOwner)
			{

				if (!gun.PreventNormalFireAudio)
				{
					this.gun.PreventNormalFireAudio = true;
				}
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}
			}

		}
		public override void OnPostFired(PlayerController player, Gun flakcannon)
		{
			//ETGModConsole.Log("Beginning The End.");
			GameManager.Instance.StartCoroutine(GTEE.StartTheEnd());
		}

		private static IEnumerator StartTheEnd()
		{
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			Gun oldgun;
			oldgun = (Game.Items["@:gun_to_end_everything"] as Gun);
			player.inventory.DestroyGun(oldgun);
			GTEE gun = player.gameObject.GetComponent<GTEE>();;
			string header2 = "Those That Are Lost";
			string text2 = "Filler.";
			GTEE.Notify(header2, text2);
			string guid;
			guid = "fodder_enemy";
			PlayerController owner = player;
			for (int i = 0; i <= 20; i++)
            {
				for (int e = 0; e <= 2; e++)
				{
					AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
					IntVector2? intVector = new IntVector2?(owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
					AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
					aiactor.CanTargetEnemies = false;
					aiactor.CanTargetPlayers = true;
					PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
					aiactor.IsHarmlessEnemy = false;
					aiactor.IgnoreForRoomClear = true;
					aiactor.HandleReinforcementFallIntoRoom(-1f);
					var nur = aiactor.aiActor;
					nur.EffectResistances = new ActorEffectResistance[]
	                {  
					new ActorEffectResistance()
					{
						resistAmount = 1,
						resistType = EffectResistanceType.Fire
					},
	                };
				}
				yield return new WaitForSeconds(0.5f);
			}
			string ee = "The Spark";
			string eee = "Filler.";
			GTEE.Notify(ee, eee);
			AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
			GoopDefinition goopDef = assetBundle.LoadAsset<GoopDefinition>("assets/data/goops/napalmgoopquickignite.asset");
			DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDef);
			goopManagerForGoopType.TimedAddGoopCircle(player.sprite.WorldCenter, 30f, 0.35f, false);
			goopDef.damagesEnemies = false;
			for (int e = 0; e <= 3; e++)
			{
				guid = "wow_bullet";
				AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
				IntVector2? intVector = new IntVector2?(owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
				AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
				aiactor.CanTargetEnemies = false;
				aiactor.CanTargetPlayers = true;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
				aiactor.IsHarmlessEnemy = false;
				aiactor.IgnoreForRoomClear = true;
				aiactor.HandleReinforcementFallIntoRoom(-1f);
				aiactor.healthHaver.SetHealthMaximum(aiactor.healthHaver.GetMaxHealth() * 10);
			}
			yield return new WaitForSeconds(10f);
		    header2 = "Circle Of Stone";
			text2 = "Filler.";
			GTEE.Notify(header2, text2);
			GameObject Mines_Cave_In;

			for (int i = 0; i <= 20; i++)
            {
				RoomHandler currentRoom = player.CurrentRoom;
				AssetBundle assetBundle1 = ResourceManager.LoadAssetBundle("shared_auto_002");
				Mines_Cave_In = assetBundle1.LoadAsset<GameObject>("Mines_Cave_In");
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Mines_Cave_In, player.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(-7, 7), UnityEngine.Random.Range(-7, 7)), Quaternion.identity);
				HangingObjectController RockSlideController = gameObject.GetComponent<HangingObjectController>();
				RockSlideController.triggerObjectPrefab = null;
				GameObject[] additionalDestroyObjects = new GameObject[]
				{
				RockSlideController.additionalDestroyObjects[1]
				};
				RockSlideController.additionalDestroyObjects = additionalDestroyObjects;
				UnityEngine.Object.Destroy(gameObject.transform.Find("Sign").gameObject);
				RockSlideController.ConfigureOnPlacement(currentRoom);
				yield return new WaitForSeconds(0.5f);
				RockSlideController.Interact(player);
			}
			header2 = "Old Gods Rising";
			text2 = "Filler.";
			GTEE.Notify(header2, text2);
			for (int i = 0; i <= 1; i++)
			{
				guid = "jammed_guardian";
				AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
				IntVector2? intVector = new IntVector2?(owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
				AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
				aiactor.CanTargetEnemies = false;
				aiactor.CanTargetPlayers = true;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
				aiactor.IsHarmlessEnemy = false;
				aiactor.IgnoreForRoomClear = true;
				aiactor.HandleReinforcementFallIntoRoom(-1f);
				yield return new WaitForSeconds(5f);
			}
			header2 = "The Ones Who Will Seal All Off";
			text2 = "Filler.";
			GTEE.Notify(header2, text2);

			List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
			bool flag = activeEnemies != null;
			if (flag)
			{
				for (int i = 0; i < activeEnemies.Count; i++)
				{
					activeEnemies[i].PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero, true, false, false);
					activeEnemies[i].BecomeBlackPhantom();
				}

			}
			yield return new WaitForSeconds(10f);
			header2 = "Plague";
			text2 = "Filler.";
			GTEE.Notify(header2, text2);
			GoopDefinition goopDef1 = assetBundle.LoadAsset<GoopDefinition>("assets/data/goops/poison goop.asset");
			DeadlyDeadlyGoopManager goopManagerForGoopType1 = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDef1);
			goopManagerForGoopType1.TimedAddGoopCircle(player.sprite.WorldCenter, 30, 0.35f, false);
			goopDef.damagesEnemies = false;
			for (int i = 0; i <= 4; i++)
			{
				guid = "e61cab252cfb435db9172adc96ded75f";
				AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
				IntVector2? intVector = new IntVector2?(owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
				AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
				aiactor.CanTargetEnemies = false;
				aiactor.CanTargetPlayers = true;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
				aiactor.IsHarmlessEnemy = false;
				aiactor.IgnoreForRoomClear = true;
				aiactor.HandleReinforcementFallIntoRoom(-1f);
				aiactor.BecomeBlackPhantom();
			}
			yield return new WaitForSeconds(10f);
			header2 = "Great Ones Wrath";
			text2 = "Filler.";
			GTEE.Notify(header2, text2);
            {
				guid = "ec6b674e0acd4553b47ee94493d66422";
				AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
				IntVector2? intVector = new IntVector2?(owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
				AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
				aiactor.CanTargetEnemies = false;
				aiactor.CanTargetPlayers = true;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
				aiactor.IsHarmlessEnemy = false;
				aiactor.IgnoreForRoomClear = true;
				aiactor.HandleReinforcementFallIntoRoom(-1f);
				aiactor.BecomeBlackPhantom();
				aiactor.healthHaver.SetHealthMaximum(aiactor.healthHaver.GetMaxHealth() * 2);
				var nur = aiactor.aiActor;
				nur.EffectResistances = new ActorEffectResistance[]
				{
					new ActorEffectResistance()
					{
						resistAmount = 1,
						resistType = EffectResistanceType.Fire
					},
				};
				nur.EffectResistances = new ActorEffectResistance[]
				{
					new ActorEffectResistance()
					{
						resistAmount = 1,
						resistType = EffectResistanceType.Poison
					},
				};
				nur.EffectResistances = new ActorEffectResistance[]
				{
					new ActorEffectResistance()
					{
						resistAmount = 1,
						resistType = EffectResistanceType.Charm
					},
				};
				nur.EffectResistances = new ActorEffectResistance[]
	            {
					new ActorEffectResistance()
					{
						resistAmount = 1,
						resistType = EffectResistanceType.Freeze
					},
	            };
				aiactor.behaviorSpeculator.CooldownScale *= 10;
				
			}
			GameObject dragunRocket = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyRocket;

			for (int i = 0; i <= 50; i++)
			{
					IntVector2? vector = (player as PlayerController).CurrentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 2), CellTypes.FLOOR | CellTypes.PIT, false, null);
				Vector2 vector2 = vector.Value.ToVector2();
				SkyRocket component = SpawnManager.SpawnProjectile(dragunRocket, player.sprite.WorldCenter, Quaternion.identity, true).GetComponent<SkyRocket>();
				component.TargetVector2 = vector2;
				tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
				component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
				yield return new WaitForSeconds(0.2f);

			}
			header2 = "Once More Into The Breach";
			text2 = "Filler.";
			GTEE.Notify(header2, text2);
			guid = "fodder_enemy";
			GoopDefinition aa = assetBundle.LoadAsset<GoopDefinition>("assets/data/goops/napalmgoopquickignite.asset");
			DeadlyDeadlyGoopManager goopManagerForGoopType2 = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(aa);
			goopManagerForGoopType2.TimedAddGoopCircle(player.sprite.WorldCenter, 30f, 0.35f, false);
			aa.damagesEnemies = false;
			GameObject aaa = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyBoulder;
			for (int i = 0; i <= 100; i++)
			{
				IntVector2? vector = (player as PlayerController).CurrentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 2), CellTypes.FLOOR | CellTypes.PIT, false, null);
				Vector2 vector2 = vector.Value.ToVector2();
				SkyRocket component = SpawnManager.SpawnProjectile(aaa, player.sprite.WorldCenter, Quaternion.identity, true).GetComponent<SkyRocket>();
				component.TargetVector2 = vector2;
				tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
				component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
				yield return new WaitForSeconds(0.05f);
				RoomHandler currentRoom = player.CurrentRoom;
				AssetBundle assetBundle1 = ResourceManager.LoadAssetBundle("shared_auto_002");
				Mines_Cave_In = assetBundle1.LoadAsset<GameObject>("Mines_Cave_In");
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Mines_Cave_In, player.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(-7, 7), UnityEngine.Random.Range(-7, 7)), Quaternion.identity);
				HangingObjectController RockSlideController = gameObject.GetComponent<HangingObjectController>();
				RockSlideController.triggerObjectPrefab = null;
				GameObject[] additionalDestroyObjects = new GameObject[]
				{
				RockSlideController.additionalDestroyObjects[1]
				};
				RockSlideController.additionalDestroyObjects = additionalDestroyObjects;
				UnityEngine.Object.Destroy(gameObject.transform.Find("Sign").gameObject);
				RockSlideController.ConfigureOnPlacement(currentRoom);
				yield return new WaitForSeconds(0.05f);
				RockSlideController.Interact(player);
				AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
				IntVector2? intVector = new IntVector2?(owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
				AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
				aiactor.CanTargetEnemies = false;
				aiactor.CanTargetPlayers = true;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
				aiactor.IsHarmlessEnemy = false;
				aiactor.IgnoreForRoomClear = true;
				aiactor.HandleReinforcementFallIntoRoom(-1f);
				aiactor.BecomeBlackPhantom();
				var nur = aiactor.aiActor;
				nur.EffectResistances = new ActorEffectResistance[]
				{
					new ActorEffectResistance()
					{
						resistAmount = 1,
						resistType = EffectResistanceType.Fire
					},
				};
			}
			header2 = "For The Worthy";
			text2 = "Filler.";
			GTEE.Notify(header2, text2);
			for (int i = 0; i < 5; i++)
			{
				Chest rainbow_Chest = GameManager.Instance.RewardManager.Rainbow_Chest;
				rainbow_Chest.IsLocked = false;
				Chest chest2 = Chest.Spawn(rainbow_Chest, player.CurrentRoom.GetRandomVisibleClearSpot(1, 1));
				chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
			}

			yield break;
		}



		private static void Notify(string header, string text)
		{
			tk2dSpriteCollectionData encounterIconCollection = AmmonomiconController.Instance.EncounterIconCollection;
			int spriteIdByName = encounterIconCollection.GetSpriteIdByName("Planetside/Resources/shellheart.png");
			GameUIRoot.Instance.notificationController.DoCustomNotification(header, text, null, spriteIdByName, UINotificationController.NotificationColor.PURPLE, false, true);
		}

		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
		}
	}

}