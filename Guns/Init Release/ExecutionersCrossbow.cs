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
	public class ExecutionersCrossbow : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Executioners Crossbow", "executionerscrossbow");
			Game.Items.Rename("outdated_gun_mods:executioners_crossbow", "psog:executioners_crossbow");
			var behav = gun.gameObject.AddComponent<ExecutionersCrossbow>();
			GunExt.SetShortDescription(gun, "Hang In There!");
			GunExt.SetLongDescription(gun, "This old and slightly worn crossbow still holds the souls on long gone Gundead from when a party of Gungeoneers ran out of ammo and made 'improv weapons'. Hold the arrow long enough and you just may be able to spot a soul enter it.");
			GunExt.SetupSprite(gun, null, "executionerscrossbow_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 3);
			GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 3);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 3);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(8) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(12) as Gun).gunSwitchGroup;

			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2f;
			gun.DefaultModule.cooldownTime = 0.2f;
			gun.DefaultModule.numberOfShotsInClip = 4;
			gun.SetBaseMaxAmmo(60);
			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.ARROW;

			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[1].eventAudio = "SND_WPN_crossbow_shot_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[1].triggerEvent = true;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "SND_WPN_crossbow_reload_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;

			gun.barrelOffset.transform.localPosition = new Vector3(2.0f, 0.4375f, 0f);
			gun.carryPixelOffset += new IntVector2((int)4f, (int)0f);
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 2;
			gun.encounterTrackable.EncounterGuid = "Bow. Nothing funny, fuck you.";
			gun.gunClass = GunClass.CHARGE;

			Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
			projectile2.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile2);
			gun.DefaultModule.projectiles[0] = projectile2;
			projectile2.baseData.damage = 30f;
			projectile2.baseData.speed = 40f;
			projectile2.baseData.force = 1f;
			projectile2.baseData.range = 100f;
			projectile2.AdditionalScaleMultiplier *= 1f;
			projectile2.transform.parent = gun.barrelOffset;
			projectile2.HasDefaultTint = true;
			projectile2.shouldRotate = true;
			projectile2.SetProjectileSpriteRight("executionerscrossbowprojectile1", 17, 3, false, tk2dBaseSprite.Anchor.MiddleCenter, 17, 3);
			projectile2.sprite.usesOverrideMaterial = true;

			Projectile projectile3 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
			projectile3.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile3.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile3);
			gun.DefaultModule.projectiles[0] = projectile3;
			projectile3.baseData.damage = 15f;
			projectile3.baseData.speed = 45f;
			projectile3.baseData.force = 3f;
			projectile3.baseData.range = 100f;
			projectile3.AdditionalScaleMultiplier *= 1.33f;
			projectile3.transform.parent = gun.barrelOffset;
			projectile3.HasDefaultTint = true;
			projectile3.shouldRotate = true;
			projectile3.SetProjectileSpriteRight("executionerscrossbowprojectile2", 20, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 20, 5);
			projectile3.sprite.usesOverrideMaterial = true;
			
			projectile3.gameObject.AddComponent<ExecutionersCrossbowSpecial>();

			PierceProjModifier aaaa = projectile3.gameObject.AddComponent<PierceProjModifier>();
			aaaa.penetration = 100;
			aaaa.penetratesBreakables = true;

			ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile2,
				ChargeTime = 0f,
				AmmoCost = 1
			};
			ProjectileModule.ChargeProjectile item3 = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile3,
				ChargeTime = 1f,
				AmmoCost = 2,
				
				
			};
			gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
			{
				item2,
				item3
			};
			gun.quality = PickupObject.ItemQuality.B;
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			gun.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);

			ExecutionersCrossbow.ExecutionersCrossbowID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int ExecutionersCrossbowID;

		private bool HasReloaded;

		public Vector3 projectilePos;
		public override void OnPostFired(PlayerController player, Gun flakcannon)
		{
			AkSoundEngine.PostEvent("Play_SND_WPN_crossbow_shot_01", base.gameObject);

		}
		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				HasReloaded = false;
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				AkSoundEngine.PostEvent("Play_WPN_duelingpistol_reload_01", gameObject);
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
		}

		public override void Update()
		{
			if (gun.CurrentOwner)
			{
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}
			}
		}
	}
}
