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

namespace Planetside
{
	public class Funcannon : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Funcannon", "funcannon");
			Game.Items.Rename("outdated_gun_mods:funcannon", "psog:funcannon");
			gun.gameObject.AddComponent<Funcannon>();
			gun.SetShortDescription("Fungal Warfare");
			gun.SetLongDescription("A mushroom that has completely enveloped an old pirate cannon.\n\nFolk tale claims of a hidden pirate-themed Chamber in the Gungeon, with this being crucial evidence.");
			GunExt.SetupSprite(gun, null, "funcannon_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 7);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 5);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(39) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(755) as Gun).gunSwitchGroup;
			/*
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_PET_junk_splat_03";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_OBJ_wax_splat_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;
			*/
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
			crossbowHandler.projectileToSpawn = (PickupObjectDatabase.GetById(197) as Gun).DefaultModule.projectiles[0];
			projectile.SetProjectileSpriteRight("funcannon_projectile_001", 18, 6, false, tk2dBaseSprite.Anchor.MiddleCenter, 16, 6);

			PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
			spook.penetratesBreakables = true;
			gun.encounterTrackable.EncounterGuid = "Big Fungus";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.DEFEAT_FUNGANNON, true);
			gun.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);



			Funcannon.FuncannonID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int FuncannonID;
	}
}

namespace Planetside
{
	public class FuncannonProjectileComponent : MonoBehaviour
	{
		private void Start()
		{
			this.m_projectile = base.GetComponent<Projectile>();
			this.m_projectile.OnDestruction += M_projectile_OnDestruction;
		}

		private void M_projectile_OnDestruction(Projectile obj)
		{
			for (int i = 0; i < 24; i++)
			{
                this.SpawnProjectile(this.projectileToSpawn, this.m_projectile.sprite.WorldCenter, UnityEngine.Random.Range(-180, 180), null);
            }
        }

		private void Update()
		{
			this.elapsed += BraveTime.DeltaTime;
			if (this.elapsed > 0.0333f)
			{
				elapsed = 0;
                this.SpawnProjectile(this.projectileToSpawn, this.m_projectile.sprite.WorldCenter,  UnityEngine.Random.Range(-180, 180), null);
			}
		}

		private void SpawnProjectile(Projectile proj, Vector3 spawnPosition, float zRotation, SpeculativeRigidbody collidedRigidbody = null)
		{
			GameObject gameObject = SpawnManager.SpawnProjectile(proj.gameObject, spawnPosition, Quaternion.Euler(0f, 0f, zRotation), true);
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

                component.gameObject.AddComponent<RecursionPreventer>();
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

		private Projectile m_projectile;
		public Projectile projectileToSpawn;
		private float elapsed;
	}
}