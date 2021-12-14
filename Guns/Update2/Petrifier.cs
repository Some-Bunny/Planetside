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
	public class Petrifier : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Petrifier", "petrifier");
			Game.Items.Rename("outdated_gun_mods:petrifier", "psog:petrifier");
			gun.gameObject.AddComponent<Petrifier>();
			gun.SetShortDescription("Fear Is The Mindkiller");
			gun.SetLongDescription("Crude, yet powerful. Fires bursts of fast bolts. A primitive form of the railgun, designed by the insane and patented by the irrational.");
			gun.SetupSprite(null, "petrifier_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 4);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 2);

			for (int i = 0; i < 4; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(88) as Gun, true, false);

			}
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = 0.1f;
				projectileModule.angleVariance = 25f;
				projectileModule.numberOfShotsInClip = 2;


				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(370) as Gun).DefaultModule.chargeProjectiles[1].Projectile);
				//RandomProjectileReplacementItem component = PickupObjectDatabase.GetById(524).GetComponent<RandomProjectileReplacementItem>();
				//Projectile replacementProjectile = component.ReplacementProjectile;
				//Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(replacementProjectile);

				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 17f;
				projectile.AdditionalScaleMultiplier = 1f;
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}
				projectile.transform.parent = gun.barrelOffset;
			}
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(156) as Gun).gunSwitchGroup;
			gun.barrelOffset.transform.localPosition += new Vector3(0.75f, 0.5f, 0f);
			gun.reloadTime = 1.9f;
			gun.SetBaseMaxAmmo(80);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects;
			gun.gunClass = GunClass.SHOTGUN;

			gun.quality = PickupObject.ItemQuality.B;
			gun.encounterTrackable.EncounterGuid = "Fear.";
			ETGMod.Databases.Items.Add(gun, null, "ANY");

			Petrifier.PetrifierID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int PetrifierID;
		private bool HasReloaded;

		protected void Update()
		{
			if (gun.CurrentOwner)
			{
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}
			}

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


