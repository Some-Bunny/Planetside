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

namespace Planetside
{
	public class HellLight : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Hell Light", "helllight");
			Game.Items.Rename("outdated_gun_mods:hell_light", "psog:hell_light");
			gun.gameObject.AddComponent<HellLight>();
			GunExt.SetShortDescription(gun, "You Dare Defy The Father?");
			GunExt.SetLongDescription(gun, "this is a synergy form, nerd.");
			GunExt.SetupSprite(gun, null, "helllight_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 24);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 6);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 6);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(62) as Gun, true, false);
			gun.SetBaseMaxAmmo(160);
			
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_woodbow_shot_02";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;

			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_BOSS_doormimic_vomit_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;

			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.reloadTime = 1.2f;
			gun.DefaultModule.cooldownTime = 0.3f;
			gun.DefaultModule.numberOfShotsInClip = 8;
			gun.DefaultModule.angleVariance = 8f;
			gun.barrelOffset.transform.localPosition = new Vector3(1.25f, 0.5f, 0f);
			gun.quality = PickupObject.ItemQuality.EXCLUDED;
			gun.encounterTrackable.EncounterGuid = "WEEEEWEWEWEWEWEHWEWEWEEWEWE";
			gun.CanBeDropped = true;
			Gun gun4 = PickupObjectDatabase.GetById(336) as Gun;
			gun.muzzleFlashEffects = gun4.muzzleFlashEffects;
			gun.gunClass = GunClass.PISTOL;

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Trident", "Planetside/Resources/GunClips/HellLight/hellfull", "Planetside/Resources/GunClips/HellLight/hellemptyl");


			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]); projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.transform.parent = gun.barrelOffset;
			projectile.SetProjectileSpriteRight("helllightprojectile_001", 9, 9, false, tk2dBaseSprite.Anchor.MiddleCenter, 9, 9);

			projectile.AdditionalScaleMultiplier *= 0.6f;
			projectile.baseData.damage = 10f;
			projectile.baseData.speed *= 4;
			projectile.pierceMinorBreakables = true;
			projectile.hitEffects.alwaysUseMidair = true;
			projectile.hitEffects.overrideMidairDeathVFX = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
			projectile.collidesWithPlayer = false;
			ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
			yes.spawnShadows = true;
			yes.shadowLifetime = 0.75f;
			yes.shadowTimeDelay = 0.01f;
			yes.dashColor = new Color(1f, 0f, 0f, 0.25f);



			PierceProjModifier spook = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
			spook.penetration = 10000;
			spook.penetratesBreakables = true;
			projectile.gameObject.AddComponent<HellLightProjectile>();

			Gun gunproj = PickupObjectDatabase.GetById(62) as Gun;
			Projectile projectile1 = UnityEngine.Object.Instantiate<Projectile>(gunproj.DefaultModule.projectiles[0]);
			projectile1.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile1.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile1);
			projectile1.SetProjectileSpriteRight("helllightprojectile_001", 9, 9, false, tk2dBaseSprite.Anchor.MiddleCenter, 9, 9);

			projectile1.collidesWithPlayer = false;

			projectile1.AdditionalScaleMultiplier *= 0.6f;
			projectile1.baseData.damage = 10f;
			projectile1.baseData.speed *= 4;
			projectile1.shouldRotate = true;
			projectile1.pierceMinorBreakables = true;
			projectile1.hitEffects.alwaysUseMidair = true;
			projectile1.hitEffects.overrideMidairDeathVFX = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;

			ImprovedAfterImage eye = projectile1.gameObject.AddComponent<ImprovedAfterImage>();
			eye.spawnShadows = true;
			eye.shadowLifetime = 0.75f;
			eye.shadowTimeDelay = 0.01f;
			eye.dashColor = new Color(1f, 0f, 0f, 0.25f);

			PierceProjModifier spook1 = projectile1.gameObject.GetOrAddComponent<PierceProjModifier>();
			spook1.penetration = 10000;
			spook1.penetratesBreakables = true;



			HellLight.HellLightProjectile = projectile1;




			ETGMod.Databases.Items.Add(gun, false, "ANY");
			HellLight.HellLightID = gun.PickupObjectId;
			//ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static Projectile HellLightProjectile;
		public static int HellLightID;


		private bool HasReloaded;

		public Vector3 projectilePos;

		public override void OnPickup(PlayerController player)
		{
			base.OnPickup(player);
			player.GunChanged += this.changedGun;
			flightCheck(player.CurrentGun);
		}
		public override void OnPostDrop(PlayerController player)
		{
			player.GunChanged -= this.changedGun;
			flightCheck(player.CurrentGun);
			base.OnPostDrop(player);
		}

		private void changedGun(Gun oldGun, Gun newGun, bool what)
		{
			//ETGModConsole.Log("Guns changed");
			flightCheck(newGun);
		}

		private bool GaveFlight;
		private bool hasBirdieNow;
		private bool hadBirdieLastWeChecked;

		private void flightCheck(Gun currentGun)
		{
			//ETGModConsole.Log("Flightchecked");
			PlayerController playerController = currentGun.CurrentOwner as PlayerController;
			if (currentGun == this.gun && !this.GaveFlight && playerController.PlayerHasActiveSynergy("Deliver Us From All Evil") && !playerController.PlayerHasActiveSynergy("BLASPHEMY AGAINST THE HOLY SPIRIT"))
			{
				base.Player.SetIsFlying(true, "Holy", false, false);
				base.Player.AdditionalCanDodgeRollWhileFlying.AddOverride("Holy", null);
				this.GaveFlight = true;
			}
			else
			{
				if ((currentGun != this.gun || !playerController.PlayerHasActiveSynergy("Deliver Us From All Evil")) && this.GaveFlight)
				{
					base.Player.SetIsFlying(false, "Holy", false, false);
					base.Player.AdditionalCanDodgeRollWhileFlying.RemoveOverride("Holy");
					this.GaveFlight = false;

				}
			}
		}

		public override void Update()
		{
			base.Update();
			gun.PreventNormalFireAudio = true;
			if (!gun.IsReloading && !HasReloaded)
			{
				this.HasReloaded = true;
			}
			PlayerController player = gun.CurrentOwner as PlayerController;
			hasBirdieNow = player.PlayerHasActiveSynergy("Deliver Us From All Evil");
			if (hasBirdieNow != hadBirdieLastWeChecked)
			{
				flightCheck(player.CurrentGun);
				hadBirdieLastWeChecked = hasBirdieNow;
			}
		}
	}
}
