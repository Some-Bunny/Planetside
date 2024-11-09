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
using Alexandria.Assetbundle;

namespace Planetside
{
	public class VeteranShotgun : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Veterans Shotgun", "veteranshotgun");
			Game.Items.Rename("outdated_gun_mods:veterans_shotgun", "psog:veterans_shotgun");
			gun.gameObject.AddComponent<VeteranShotgun>();
			gun.SetShortDescription("Old And Tested");
			gun.SetLongDescription("A simple shotgun.\n\nDespite this, there has been so many legal and ethical issues surrounding this gun, as all the variants of its ammunition is \'outright a war crime\' everywhere, and anyone that ships any always ships it mixed.\n\nPray that none of the shells are explosive.");

			GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "veteranshotgun_idle_001");
			gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
			gun.sprite.SortingOrder = 1;

			gun.reloadAnimation = "veteransshotgun_reload";
			gun.idleAnimation = "veteransshotgun_idle";
			gun.shootAnimation = "veteransshotgun_fire";


            gun.gameObject.transform.Find("Clip").transform.position = new Vector3(0.25f, 0.3125f);
            gun.clipObject =(PickupObjectDatabase.GetById(82) as Gun).shellCasing;
            gun.reloadClipLaunchFrame = 1;
            gun.clipsToLaunchOnReload = 2;

            gun.reloadClipLaunchFrame = 4;
            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> { { 7, "Play_OBJ_lock_unlock_01" }, { 11, "Play_OBJ_lock_unlock_01" }});


            tk2dSpriteAnimationClip fireClip2 = gun.sprite.spriteAnimator.GetClipByName(gun.reloadAnimation);
			float[] offsetsY2 = new float[] { 0f, 0f, -0.5625f, -0.875f, -0.875f, 0, 0, -0.875f, -0.875f, -0f, -0f, -0.875f, -0.875f, };
			for (int i = 0; i < offsetsY2.Length && i < fireClip2.frames.Length; i++)
			{
				int id = fireClip2.frames[i].spriteId;
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY2[i];
			}


			int e = 0;
			for (int i = 0; e < 7; i++, e++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(378) as Gun, true, false);
				gun.Volley.projectiles[e].ammoCost = 1;
				gun.Volley.projectiles[e].shootStyle = ProjectileModule.ShootStyle.Automatic;
				gun.Volley.projectiles[e].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				gun.Volley.projectiles[e].cooldownTime = 0.75f;
				gun.Volley.projectiles[e].angleVariance = 18f;
				gun.Volley.projectiles[e].numberOfShotsInClip = 2;
				gun.Volley.UsesShotgunStyleVelocityRandomizer = true;
				gun.Volley.DecreaseFinalSpeedPercentMin = 60f;
				gun.Volley.IncreaseFinalSpeedPercentMax = 130f;

				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(378) as Gun).DefaultModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				gun.Volley.projectiles[e].projectiles[0] = projectile;
				projectile.baseData.damage = 5f;
				projectile.baseData.range = 7 + e;
				projectile.baseData.speed = 10 + e;
				projectile.baseData.force = 5;
                var m = projectile.gameObject.AddComponent<VeteranShotgunProjectile>();
				m.projectile = projectile;
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = gun.Volley.projectiles[e] != gun.DefaultModule;
				if (flag)
				{
					gun.Volley.projectiles[e].ammoCost = 0;
				}
			}
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(82) as Gun).gunSwitchGroup;
            gun.gunClass = GunClass.SHOTGUN;
			gun.barrelOffset.transform.localPosition += new Vector3(0.25f, 0f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(329) as Gun).muzzleFlashEffects;
            gun.carryPixelOffset = new IntVector2(4, -2);


            gun.reloadTime = 2f;
			gun.SetBaseMaxAmmo(90);
			gun.quality = PickupObject.ItemQuality.D;

			gun.encounterTrackable.EncounterGuid = "Its a shtung";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.IRONSHOT);
			List<string> mandatoryConsoleIDs1 = new List<string>
			{
				"psog:veterans_shotgun"
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"ammo_belt",
				"cluster_mine",
				"portable_turret",
				"cog_of_battle",
				"c4",
				"air_strike",
				"napalm_strike"
			};
			CustomSynergies.Add("Old War", mandatoryConsoleIDs1, optionalConsoleIDs, false);
			VeteranShotgun.VeteranID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int VeteranID;
	}
}