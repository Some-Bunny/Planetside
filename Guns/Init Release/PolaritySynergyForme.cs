﻿using Alexandria.Assetbundle;
using Gungeon;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FakePrefab = ItemAPI.FakePrefab;
using Object = UnityEngine.Object;

namespace Planetside
{
	public class PolarityForme : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Balanced Polarity", "polarity1");
			Game.Items.Rename("outdated_gun_mods:balanced_polarity", "psog:balanced_polarity");
			gun.gameObject.AddComponent<PolarityForme>();
			GunExt.SetShortDescription(gun, "This was a fucking pain to make");
			GunExt.SetLongDescription(gun, "A weapon forged by two wanderers from polar-opposite climates. It is said that they imbued part of their magic into it so one element doesn't damage the other.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "polarity1_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.idleAnimation = "polarityalt_idle";
            gun.shootAnimation = "polarityalt_fire";
            gun.reloadAnimation = "polarityalt_reload";


            //GunExt.SetupSprite(gun, null, "polarity1_idle_001", 8);
            //GunExt.SetAnimationFPS(gun, gun.shootAnimation, 160);
            //GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 10);
            //GunExt.SetAnimationFPS(gun, gun.idleAnimation, 4);
            gun.SetBaseMaxAmmo(450);

			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(223) as Gun, true, false);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(336) as Gun, true, false);


			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_Yari";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;

			EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> { { 0, "Play_BOSS_tank_grenade_01" }, { 6, "Play_OBJ_lock_unlock_01" }, { 13, "Play_OBJ_lock_unlock_01" } });

			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(16) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.damageModifier = 1;
			gun.reloadTime = 1.6f;
			gun.DefaultModule.cooldownTime = 0.05f;
			gun.DefaultModule.numberOfShotsInClip = 30;
			gun.DefaultModule.angleVariance = 4f;

			gun.Volley.projectiles[0].ammoCost = 1;
			gun.Volley.projectiles[0].shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.Volley.projectiles[0].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.Volley.projectiles[0].cooldownTime = 0.05f;
			gun.Volley.projectiles[0].angleVariance = 0f;
			gun.Volley.projectiles[0].numberOfShotsInClip = 30;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[0].projectiles[0]);
			projectile.gameObject.SetActive(false);
			gun.Volley.projectiles[0].projectiles[0] = projectile;
			projectile.baseData.damage = 11;
			PolarityProjectile pol1 = projectile.gameObject.AddComponent<PolarityProjectile>();
			pol1.IsUp = false;

			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			if (gun.Volley.projectiles[0] != gun.DefaultModule)
			{
				gun.Volley.projectiles[0].ammoCost = 0;
			}


			projectile.transform.parent = gun.barrelOffset;

			gun.Volley.projectiles[1].ammoCost = 1;
			gun.Volley.projectiles[1].shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.Volley.projectiles[1].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.Volley.projectiles[1].cooldownTime = 0.05f;
			gun.Volley.projectiles[1].angleVariance = 0f;
			gun.Volley.projectiles[1].numberOfShotsInClip = 30;

			Projectile projectile1 = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[1].projectiles[0]);
			projectile1.gameObject.SetActive(false);
			gun.Volley.projectiles[1].projectiles[0] = projectile1;
			PolarityProjectile pol2 = projectile1.gameObject.AddComponent<PolarityProjectile>();
			pol2.IsUp = true;
			projectile1.baseData.damage = 11f;
			FakePrefab.MarkAsFakePrefab(projectile1.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile1);
			bool aa = gun.Volley.projectiles[1] != gun.DefaultModule;
			if (aa)
			{
				gun.Volley.projectiles[1].ammoCost = 0;
			}
			projectile1.transform.parent = gun.barrelOffset;


			gun.encounterTrackable.EncounterGuid = "cunt";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			gun.barrelOffset.transform.localPosition += new Vector3(2.25f, 0.0625f, 0f);
			Gun gun4 = PickupObjectDatabase.GetById(156) as Gun;
			gun.quality = PickupObject.ItemQuality.EXCLUDED;
			gun.muzzleFlashEffects = gun4.muzzleFlashEffects;
			PolarityForme.PolarityFormeID = gun.PickupObjectId;



		}
		public static int PolarityFormeID;

		public override void PostProcessProjectile(Projectile projectile)
		{			
			gun.PreventNormalFireAudio = true;
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
		public Vector3 projectilePos;
	}
}
