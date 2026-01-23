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
	public class ResaultBlue : AdvancedGunBehaviourMultiActive
	{
		public static void ResetMaxAmmo()
		{
			Gun gun = (PickupObjectDatabase.GetById(ResaultBlue.ID) as Gun);
			gun.SetBaseMaxAmmo(200);
		}
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Blue Re-Sault", "blueresault");
			Game.Items.Rename("outdated_gun_mods:blue_resault", "psog:blue_resault");
			gun.gameObject.AddComponent<Resault>();
			GunExt.SetShortDescription(gun, "Reduce, Reuse, Reload");
			GunExt.SetLongDescription(gun, "An automatic machine-gun that dispenses ammo capacity when fired, restores a portion of it on killing an enemy. Gungeonoligists still specualte *why* this gun dispenses its ammo capacity. Is it to reduce weight? Some idiot thought it would be an interesting gimmick? No-one really knows.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "resaultblue_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.idleAnimation = "resaultblue_idle";
            gun.shootAnimation = "resaultblue_fire";
            gun.reloadAnimation = "resaultblue_reload";


 
            GunExt.AddProjectileModuleFrom(gun, Guns.Hegemony_Rifle, true, false);
            GunExt.AddProjectileModuleFrom(gun, Guns.Hegemony_Rifle, true, false);
            GunExt.AddProjectileModuleFrom(gun, Guns.Hegemony_Rifle, true, false);
            GunExt.AddProjectileModuleFrom(gun, Guns.Hegemony_Rifle, true, false);

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 4f;
            projectile.baseData.speed *= 2.5f;
            projectile.baseData.force *= 0.5f;



            gun.gunSwitchGroup = Guns.Hegemony_Rifle.gunSwitchGroup;
            gun.reloadTime = 1.6f;

            projectile.shouldRotate = true;
            projectile.pierceMinorBreakables = true;

            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.5f;
            yes.shadowTimeDelay = 0.1f;
            yes.dashColor = new Color(0f, 0.1f, 1, 1);

            foreach (var entry in gun.Volley.projectiles)
			{
                entry.ammoCost = 1;
                entry.shootStyle = ProjectileModule.ShootStyle.Automatic;
                entry.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                entry.cooldownTime = .025f;
                entry.numberOfShotsInClip = 50;
                gun.SetBaseMaxAmmo(250);
                entry.angleVariance = 11f;
                entry.burstShotCount = 1;
                entry.projectiles[0] = projectile;
            }

            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            gun.barrelOffset.transform.localPosition = new Vector3(2.375f, 0.75f, 0f);
			gun.carryPixelOffset = new IntVector2((int)2f, (int)-1.5f);
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			gun.gunClass = GunClass.RIFLE;
            ID = gun.PickupObjectId;

            AdvancedTransformGunSynergyProcessor advancedTransformGunSynergyProcessor = (PickupObjectDatabase.GetById(Resault.ResaultID) as Gun).gameObject.AddComponent<AdvancedTransformGunSynergyProcessor>();
            advancedTransformGunSynergyProcessor.NonSynergyGunId = Resault.ResaultID;
            advancedTransformGunSynergyProcessor.SynergyGunId = ID;
            advancedTransformGunSynergyProcessor.SynergyToCheck = "Infinite Ammo?";

        }

		public static int ID;
		private bool HasReloaded;
	}
}

