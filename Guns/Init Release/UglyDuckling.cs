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
	public class UglyDuckling : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Ugly Duckling", "uglyduck");
			Game.Items.Rename("outdated_gun_mods:ugly_duckling", "psog:ugly_duckling");
			gun.gameObject.AddComponent<UglyDuckling>();
			gun.SetShortDescription("hjonk");
			gun.SetLongDescription("You Lost the Game.");
			gun.SetupSprite(null, "uglyduck_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 4);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 2);

			for (int i = 0; i < 1; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(7) as Gun, true, false);

			}
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = 0.9f;
				projectileModule.angleVariance = 0f;
				projectileModule.numberOfShotsInClip = 8;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 5f;
				projectile.AdditionalScaleMultiplier = 0.8f;
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
			gun.barrelOffset.transform.localPosition += new Vector3(1.5f, 0.5f, 0f);
			gun.reloadTime = 2.3f;
			gun.SetBaseMaxAmmo(69420);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(404) as Gun).muzzleFlashEffects;

			gun.quality = PickupObject.ItemQuality.EXCLUDED;
			gun.encounterTrackable.EncounterGuid = "stinky";
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			UglyDuckling.DuckyID = gun.PickupObjectId;

		}
		public static int DuckyID;
		public override void OnPostFired(PlayerController player, Gun bruhgun)
		{
			gun.PreventNormalFireAudio = true;
			AkSoundEngine.PostEvent("Play_ENM_bird_egg_01", gameObject);

		}
		private bool HasReloaded;

		protected void Update()
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
		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
				AkSoundEngine.PostEvent("Play_WPN_pillow_reload_01", gameObject);
			}
		}
	}

}