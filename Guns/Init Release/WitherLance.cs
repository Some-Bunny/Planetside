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


namespace Planetside
{
	public class WitherLance : GunBehaviour
	{
		// FINISH GUN
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Wither Lance", "witherlance");
			Game.Items.Rename("outdated_gun_mods:wither_lance", "psog:wither_lance");
			gun.gameObject.AddComponent<WitherLance>();
			GunExt.SetShortDescription(gun, "Frail and Soft");
			GunExt.SetLongDescription(gun, "Does not inflict physical damage, lowers the vitality of enemies hit with it.\n\nA primordial lance held together by dark energy. Should you be holding it?");
			GunExt.SetupSprite(gun, null, "witherlance_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 10);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 4);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(32) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(52) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2.1f;
			gun.DefaultModule.cooldownTime = .1f;
			gun.DefaultModule.numberOfShotsInClip = 10;
			gun.SetBaseMaxAmmo(200);
			gun.quality = PickupObject.ItemQuality.A;
			gun.DefaultModule.angleVariance = 4f;
			gun.DefaultModule.burstShotCount = 1;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 0.1f;
			projectile.baseData.speed *= 1f;
			projectile.AdditionalScaleMultiplier *= 1.1f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.DefaultTintColor = new Color(1f, 1f, 1f).WithAlpha(3f);
			projectile.HasDefaultTint = true;
			projectile.gameObject.AddComponent<WitherLancePROJECTILE>();
			gun.gunClass = GunClass.RIFLE;

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Wither Lance", "Planetside/Resources/GunClips/WitherLance/lancefull", "Planetside/Resources/GunClips/WitherLance/lanceempty");

			//projectile.baseData.range = 5.8f;
			gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
			gun.encounterTrackable.EncounterGuid = "Malachite Elite go BWOOOOOOOOOOOOOOOOOM";
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			gun.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);
			WitherLance.WitherLanceID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int WitherLanceID;

		private bool HasReloaded;


		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
		}
		public override void PostProcessProjectile(Projectile projectile)
		{
		}
	}
}