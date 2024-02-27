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

using Brave.BulletScript;
using FullInspector;
using Alexandria.Assetbundle;

namespace Planetside
{
	public class VeteranerShotgun : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Veteraner Shotgun", "veteranershotgun");
			Game.Items.Rename("outdated_gun_mods:veteraner_shotgun", "psog:veteraner_shotgun");
			gun.gameObject.AddComponent<VeteranerShotgun>();
			gun.SetShortDescription("Now you can be the run-ruiner");
			gun.SetLongDescription("A shotgun that oddly resembles shotguns wielded by the Shotgun Kin around the Gungeon, or maybe it's just the bullets throwing you off.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "veteranershotgun_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.reloadAnimation = "veteranershotgun_reload";
            gun.idleAnimation = "veteranershotgun_idle";
            gun.shootAnimation = "veteranershotgun_fire";
            /*
			gun.SetupSprite(null, "veteranershotgun_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 2);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 2);
			*/

            for (int i = 0; i < 5; i++)
			{
				float yah = i;
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(35) as Gun, true, false);
				gun.Volley.projectiles[i].ammoCost = 1;
				gun.Volley.projectiles[i].shootStyle = ProjectileModule.ShootStyle.Automatic;
				gun.Volley.projectiles[i].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				gun.Volley.projectiles[i].cooldownTime = 0.66f;
				gun.Volley.projectiles[i].angleVariance = 0f;
				gun.Volley.projectiles[i].numberOfShotsInClip = 4;
				gun.Volley.projectiles[i].angleFromAim = -12 + (6*i);
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[0].projectiles[0]);
				projectile.gameObject.SetActive(false);
				gun.Volley.projectiles[i].projectiles[0] = projectile;
				projectile.baseData.damage = 8;
				projectile.AdditionalScaleMultiplier = 0.75f;
				projectile.baseData.speed = 16f - (float)Mathf.Abs(yah - 2) * 0.5f;
				projectile.gameObject.AddComponent<VeteranShotgunProjectile>();
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = gun.Volley.projectiles[i] != gun.DefaultModule;
				if (flag)
				{
					gun.Volley.projectiles[i].ammoCost = 0;
				}
			}

			gun.barrelOffset.transform.localPosition += new Vector3(1.0f, 0.125f, 0f);
			gun.reloadTime = 2.3f;
			gun.SetBaseMaxAmmo(80);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(51) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.EXCLUDED;
			gun.encounterTrackable.EncounterGuid = "Its a supershtung";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			VeteranerShotgun.VeteranerID = gun.PickupObjectId;
		}
		public static int VeteranerID;
		public override void PostProcessProjectile(Projectile projectile)
		{
			
		}
		
		public override void OnPostFired(PlayerController player, Gun bruhgun)
		{
			gun.PreventNormalFireAudio = true;
			AkSoundEngine.PostEvent("Play_WPN_shotgun_shot_01", gameObject);

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
				AkSoundEngine.PostEvent("Play_WPN_shotgun_reload", gameObject);
			}
		}
	}

}