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
	public class SwanOff : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Swan-Off", "swanoff");
			Game.Items.Rename("outdated_gun_mods:swanoff", "psog:swanoff");
			gun.gameObject.AddComponent<SwanOff>();
			gun.SetShortDescription("HONK!");
			gun.SetLongDescription("Accidentally conjured by a dyslexic wizard, this swan demands cuddles and love in exchange for firepower.");
			gun.SetupSprite(null, "swanoff_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 4);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 2);

			for (int i = 0; i < 4; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(7) as Gun, true, false);

			}
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = 0.9f;
				projectileModule.angleVariance = 22.5f;
				projectileModule.numberOfShotsInClip = 8;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 6f;
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
			gun.barrelOffset.transform.localPosition += new Vector3(2.5f, 0.25f, 0f);
			gun.reloadTime = 2.3f;
			gun.SetBaseMaxAmmo(240);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(404) as Gun).muzzleFlashEffects;
			gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };

			gun.gunClass = GunClass.SILLY;

			gun.quality = PickupObject.ItemQuality.C;
			gun.encounterTrackable.EncounterGuid = "Cheer up Bunny ^ᴗ^ (i dont want to change this, at the very least not remove it)ae";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			SwanOff.SwanOffID = gun.PickupObjectId;

			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int SwanOffID;
		public override void OnPostFired(PlayerController player, Gun bruhgun)
		{
			gun.PreventNormalFireAudio = true;
			AkSoundEngine.PostEvent("Play_ENM_bird_egg_01", bruhgun.gameObject);

		}
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
		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
				AkSoundEngine.PostEvent("Play_WPN_pillow_reload_01", bruhgun.gameObject);
			}
		}
	}

}