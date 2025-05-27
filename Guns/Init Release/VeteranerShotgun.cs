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

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "veteranshotgunsynergy_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.reloadAnimation = "veteranershotgun_reload";
            gun.idleAnimation = "veteranershotgun_idle";
            gun.shootAnimation = "veteranershotgun_fire";


            gun.gameObject.transform.Find("Clip").transform.position = new Vector3(0.25f, 0.3125f);
            gun.clipObject = (PickupObjectDatabase.GetById(82) as Gun).shellCasing;
            gun.reloadClipLaunchFrame = 1;
            gun.clipsToLaunchOnReload = 2;

            gun.reloadClipLaunchFrame = 4;
            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> { { 7, "Play_OBJ_lock_unlock_01" }, { 11, "Play_OBJ_lock_unlock_01" } });

            for (int i = -3; i < 4; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(378) as Gun, true, false);
				gun.Volley.projectiles[i+3].ammoCost = 1;
				gun.Volley.projectiles[i + 3].shootStyle = ProjectileModule.ShootStyle.Automatic;
				gun.Volley.projectiles[i + 3].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				gun.Volley.projectiles[i + 3].cooldownTime = 0.75f;
				gun.Volley.projectiles[i + 3].angleVariance = 0f;
				gun.Volley.projectiles[i + 3].numberOfShotsInClip = 2;
				gun.Volley.projectiles[i + 3].angleFromAim = (4*i);
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(378) as Gun).DefaultModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectile.baseData.damage = 7.5f;
				projectile.baseData.speed = 24f - ((float)Mathf.Abs((float)i) * 0.9f);
			    var m = projectile.gameObject.AddComponent<VeteranShotgunProjectile>();
				m.projectile = projectile;
				m.Chance_1 = 0.1666f;
                m.Chance_2 = 0.01666f;
                m.Chance_3 = 0.001666f;

                gun.Volley.projectiles[i + 3].projectiles[0] = projectile;

                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				if (gun.Volley.projectiles[i + 3] != gun.DefaultModule)
				{
					gun.Volley.projectiles[i + 3].ammoCost = 0;
				}
			}

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(82) as Gun).gunSwitchGroup;
            gun.gunClass = GunClass.SHOTGUN;
            gun.barrelOffset.transform.localPosition += new Vector3(0.25f, 0f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(329) as Gun).muzzleFlashEffects;
            gun.carryPixelOffset = new IntVector2(4, -2);

            gun.reloadTime = 2f;
			gun.SetBaseMaxAmmo(90);
			gun.quality = PickupObject.ItemQuality.EXCLUDED;

            gun.encounterTrackable.EncounterGuid = "Its a supershtung";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			VeteranerShotgun.VeteranerID = gun.PickupObjectId;

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

        }
		public static int VeteranerID;
		public override void PostProcessProjectile(Projectile projectile)
		{
			
		}
	}
}