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
	public class StatiBlast : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("StatiBlast", "statiblast");
			Game.Items.Rename("outdated_gun_mods:statiblast", "psog:statiblast");
			gun.gameObject.AddComponent<StatiBlast>();
			GunExt.SetShortDescription(gun, "Your Reminder");
			GunExt.SetLongDescription(gun, "Fires small electric arcs that burst into lightning. A failed prototype of the BSG, it is unable to contain its own stored energy for long.");
			GunExt.SetupSprite(gun, null, "statiblast_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 10);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 4);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(546) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(156) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 1.3f;
			gun.DefaultModule.cooldownTime = .133f;
			gun.DefaultModule.numberOfShotsInClip = 6;
			gun.SetBaseMaxAmmo(200);
			gun.quality = PickupObject.ItemQuality.B;
			gun.DefaultModule.angleVariance = 11f;
			gun.DefaultModule.burstShotCount = 1;
			gun.gunClass = GunClass.NONE;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 5.5f;
			projectile.baseData.speed *= 1f;
			projectile.AdditionalScaleMultiplier = 0.5f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.gameObject.AddComponent<StatiBlastProjectile>();
			PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
			spook.penetration = 3;
			gun.encounterTrackable.EncounterGuid = "and his music was electric...";
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			gun.barrelOffset.transform.localPosition = new Vector3(2.0f, 0.75f, 0f);
			gun.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);
			StatiBlast.StatiBlastID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
			LinkVFXPrefab = FakePrefab.Clone(Game.Items["shock_rounds"].GetComponent<ComplexProjectileModifier>().ChainLightningVFX);

			List<string> mandatoryConsoleIDs = new List<string>
			{
				"psog:statiblast",
				"bsg"
			};
			CustomSynergies.Add("Big Shocking Gun 9000", mandatoryConsoleIDs, null, true);

		}
		public static int StatiBlastID;
		public static GameObject LinkVFXPrefab;


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
		
		public GameObject ChainLightningVFX;

		public CoreDamageTypes ChainLightningDamageTypes;

		public float ChainLightningMaxLinkDistance = 15f;

		public float ChainLightningDamagePerHit = 6f;

		public float ChainLightningDamageCooldown = 1f;

		public GameObject ChainLightningDispersalParticles;
		public bool AddsChainLightning;
	}
}