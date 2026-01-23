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
using SaveAPI;
using Alexandria.Assetbundle;
using Planetside.Controllers;

namespace Planetside
{
	public class Funcannon : GunBehaviour
	{
		public static void Add()
		{

			string Name = FoolMode.isFoolish ? "Fungannon" : "Funcannon";
            string ShortName = FoolMode.isFoolish ? "fungannon" : "funcannon";

            Gun gun = ETGMod.Databases.Items.NewGun(Name, "funcannon");
			Game.Items.Rename($"outdated_gun_mods:{ShortName}", $"psog:{ShortName}");
			gun.gameObject.AddComponent<Funcannon>();
			gun.SetShortDescription("Fungal Warfare");
			gun.SetLongDescription("A mushroom that has completely enveloped an old pirate cannon.\n\nFolk tale claims of a hidden pirate-themed Chamber in the Gungeon, with this being crucial evidence.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "funcannon_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.reloadAnimation = "funcannon_reload";
            gun.idleAnimation = "funcannon_idle";
            gun.shootAnimation = "funcannon_fire";


            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(39) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(755) as Gun).gunSwitchGroup;

            gun.spriteAnimator.GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_ENM_cannonball_blast_01";
            gun.spriteAnimator.GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
            gun.spriteAnimator.GetClipByName(gun.shootAnimation).frames[1].eventAudio = "Play_PET_junk_splat_03";
            gun.spriteAnimator.GetClipByName(gun.shootAnimation).frames[1].triggerEvent = true;

            gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2.9f;
			gun.DefaultModule.cooldownTime = .25f;
			gun.DefaultModule.numberOfShotsInClip = 1;
			gun.SetBaseMaxAmmo(20);
			gun.quality = PickupObject.ItemQuality.C;
			gun.DefaultModule.angleVariance = 3f;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 20f;
			projectile.baseData.speed *= 1f;
			projectile.AdditionalScaleMultiplier *= 1f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;

            gun.gunClass = GunClass.EXPLOSIVE;

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Funcannon", "Planetside/Resources/GunClips/Funcannon/funcannonfull", "Planetside/Resources/GunClips/Funcannon/funcannonempty");

			FuncannonProjectileComponent crossbowHandler = projectile.gameObject.AddComponent<FuncannonProjectileComponent>();
            crossbowHandler.m_projectile = projectile;

            Alexandria.Assetbundle.ProjectileBuilders.SetProjectileCollisionRight(projectile, "funcannon_projectile_001", StaticSpriteDefinitions.Projectile_Sheet_Data, 18, 6, false, tk2dBaseSprite.Anchor.MiddleCenter, 16, 6);

			PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
			spook.penetratesBreakables = true;
			gun.encounterTrackable.EncounterGuid = "Big Fungus";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.DEFEAT_FUNGANNON, true);
			gun.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);


            FuncannonSpores = UnityEngine.Object.Instantiate<Projectile>(Guns.Pea_Shooter.DefaultModule.projectiles[0]);
            FakePrefab.MarkAsFakePrefab(FuncannonSpores.gameObject);
			var c = FuncannonSpores.gameObject.AddComponent<RecursionPreventer>();

            Funcannon.FuncannonID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);

        }
        public static int FuncannonID;
		public static Projectile FuncannonSpores;
	}
}

namespace Planetside
{
	public class FuncannonProjectileComponent : MonoBehaviour
	{
		private void Start()
		{
			this.m_projectile.OnDestruction += M_projectile_OnDestruction;
		}

		private void M_projectile_OnDestruction(Projectile obj)
		{
			for (int i = 0; i < 24; i++)
			{
                this.SpawnProjectile(this.m_projectile.sprite.WorldCenter, UnityEngine.Random.Range(-180, 180));
            }
        }

		private void Update()
		{
			this.elapsed += BraveTime.DeltaTime;
			if (this.elapsed > 0.0333f)
			{
				elapsed = 0;
                this.SpawnProjectile(this.m_projectile.sprite.WorldCenter,  UnityEngine.Random.Range(-180, 180));
			}
		}

		private void SpawnProjectile(Vector3 spawnPosition, float zRotation)
		{
			GameObject gameObject = SpawnManager.SpawnProjectile(Funcannon.FuncannonSpores.gameObject, spawnPosition, Quaternion.Euler(0f, 0f, zRotation), true);
			Projectile component = gameObject.GetComponent<Projectile>();
			if (component != null)
			{
                component.SpawnedFromOtherPlayerProjectile = true;

				if (this.m_projectile != null && this.m_projectile.Owner != null)
				{
                    PlayerController playerController = this.m_projectile.Owner as PlayerController;
					if (playerController != null)
					{
                        playerController.DoPostProcessProjectile(component);
                    }
                }

                component.baseData.damage = UnityEngine.Random.Range(1.8f, 4.2f);
				component.baseData.speed = UnityEngine.Random.Range(0.4f, 3.1f);
                component.UpdateSpeed();

                component.baseData.range = 100f;
				component.AdditionalScaleMultiplier *= UnityEngine.Random.Range(0.5f, 1.5f);
				component.baseData.UsesCustomAccelerationCurve = true;
                component.baseData.AccelerationCurve = AnimationCurve.EaseInOut(0, UnityEngine.Random.Range(1.1f, 0.5f), 1, UnityEngine.Random.Range(0.25f, 0.05f));
                component.baseData.CustomAccelerationCurveDuration = UnityEngine.Random.Range(0.5f, 5.2f);
				component.baseData.IgnoreAccelCurveTime = UnityEngine.Random.Range(0.05f, 2.1f);
                HomingModifier homing = component.gameObject.AddComponent<HomingModifier>();
				homing.HomingRadius = 12f;
				homing.AngularVelocity = 75;
			}
		}

		public Projectile m_projectile;
		private float elapsed;
	}
}