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
using Brave.BulletScript;

namespace Planetside
{
	// REMEMBER TO CALL base.Update(); in Update for your gun if you want to have a pickup method
	public class RebarPuncher : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Rebar Puncher", "rebarpuncher");
			Game.Items.Rename("outdated_gun_mods:rebar_puncher", "psog:rebar_puncher");
			var behav = gun.gameObject.AddComponent<RebarPuncher>();
			GunExt.SetShortDescription(gun, "Railed It");
			GunExt.SetLongDescription(gun, "A rebar puncher originally used by the Hegemony Of Man to set up fortifications during their attempted short-lived conquest of the Gungeon.\n\nThough after analysis of the weapon, you theorize of 'other' uses.");
			GunExt.SetupSprite(gun, null, "rebarpuncher_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 12);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 7);
			GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 5);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 3);

			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(88) as Gun, true, false);
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2f;
			gun.DefaultModule.cooldownTime = 0.5f;
			gun.DefaultModule.numberOfShotsInClip = 1;
			gun.SetBaseMaxAmmo(45);

			gun.DefaultModule.angleVariance = 0f;


			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Rebar", "Planetside/Resources/GunClips/RebarPuncher/rebarfull", "Planetside/Resources/GunClips/RebarPuncher/rebarempty");

			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_seriouscannon_shot_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[8].eventAudio = "Play_OBJ_lock_unlock_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[8].triggerEvent = true;

			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).frames[4].eventAudio = "Play_OBJ_lock_unlock_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).frames[4].triggerEvent = true;
			gun.barrelOffset.transform.localPosition = new Vector3(1.0f, 0.5625f, 0f);
			gun.carryPixelOffset = new IntVector2((int)4f, (int)0.25f);
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 5;
			gun.gunClass = GunClass.SILLY;

			gun.encounterTrackable.EncounterGuid = "railgun ooedoooo";
			Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(370) as Gun).DefaultModule.chargeProjectiles[1].Projectile);
			projectile2.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile2);
			gun.DefaultModule.projectiles[0] = projectile2;
			projectile2.baseData.damage = 45f;
			projectile2.baseData.speed *= 1f;
			projectile2.baseData.force *= 1f;
			projectile2.baseData.range *= 100f;
			projectile2.transform.parent = gun.barrelOffset;
			projectile2.HasDefaultTint = true;
			projectile2.angularVelocity = 0f;
			projectile2.OverrideTrailPoint = new Vector2(0, 0);

		

			ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile2,
				ChargeTime = 0.85f
			};
			gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
			{
				item2
			};
			ParticleSystemRenderer aaa = projectile2.gameObject.AddComponent<ParticleSystemRenderer>();
			aaa.enabled = true;
			gun.quality = PickupObject.ItemQuality.C;
			gun.DefaultModule.angleFromAim = 0f;

			ETGMod.Databases.Items.Add(gun, false, "ANY");
			RebarPuncher.RebarerID = gun.PickupObjectId;


            var Collection = StaticSpriteDefinitions.Oddments_Sheet_Data;
            var NailsUp = ItemBuilder.AddSpriteToObjectAssetbundle("Nails Up", Collection.GetSpriteIdByName("plusnails"), Collection);
            FakePrefab.MarkAsFakePrefab(NailsUp);
            UnityEngine.Object.DontDestroyOnLoad(NailsUp);
            RebarPuncher.PlusNailsPrefab = NailsUp;


            gun.AddToSubShop(ItemBuilder.ShopType.Trorc	, 1f);
			SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.THORNPRICK);

			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int RebarerID;


		public override void PostProcessProjectile(Projectile projectile)
		{
			Vector2 AAAA = new Vector2(this.gun.CurrentOwner.CurrentGun.barrelOffset.position.x, this.gun.CurrentOwner.CurrentGun.barrelOffset.position.y);
			if (HasCommitedWeaponSwitch == true)
			{
				for (int counter = -3; counter < 4; counter++)
				{

					PlayerController playerController = this.gun.CurrentOwner as PlayerController;
					float dmg = (playerController.stats.GetStatValue(PlayerStats.StatType.Damage));
					Projectile projectile1 = ((Gun)ETGMod.Databases.Items[26]).DefaultModule.projectiles[0];
					GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, AAAA, Quaternion.Euler(0f, 0f, ((this.gun.CurrentOwner.CurrentGun == null) ? 1.2f : this.gun.CurrentOwner.CurrentGun.CurrentAngle) + (counter * 7.5f)), true);
					Projectile component = gameObject.GetComponent<Projectile>();
					if (component != null)
					{
						component.SpawnedFromOtherPlayerProjectile = true;
						component.Shooter = this.gun.CurrentOwner.specRigidbody;
						component.Owner = playerController;
						component.Shooter = playerController.specRigidbody;
						component.baseData.speed = 20f;
						component.AdditionalScaleMultiplier *= 1.25f;
						component.SetOwnerSafe(this.gun.CurrentOwner, "Player");
						component.ignoreDamageCaps = true;
						component.baseData.damage = 8f * dmg;
						component.baseData.range *= 2f;
						component.baseData.speed *= 2f;
					}
					HasCommitedWeaponSwitch = false;
				}
			}
		}
		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				HasReloaded = false;
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
		}
		private bool HasReloaded;

		protected override void OnPickup(PlayerController player)
		{
			base.OnPickup(player);
			player.GunChanged += this.OnGunChanged;
		}

		protected override void OnPostDrop(PlayerController player)
		{
			player.GunChanged -= this.OnGunChanged;
			base.OnPostDrop(player);
		}

		private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
		{
			if (this.gun && this.gun.CurrentOwner)
			{
				PlayerController player = this.gun.CurrentOwner as PlayerController;
				if (newGun == this.gun && HasCommitedWeaponSwitch != true)
				{
					AkSoundEngine.PostEvent("Play_OBJ_spears_clank_01", gun.gameObject);
					player.BloopItemAboveHead(RebarPuncher.PlusNailsPrefab.GetComponent<tk2dSprite>(), "");
					HasCommitedWeaponSwitch = true;
				}
				else if (HasCommitedWeaponSwitch != true)
				{
					HasCommitedWeaponSwitch = false;
				}
			}
					
		}

		protected override void Update()
		{
			base.Update();
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
		public bool HasCommitedWeaponSwitch;
		private static GameObject PlusNailsPrefab;
	}
}
