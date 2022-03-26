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
using System.Collections.ObjectModel;
using SaveAPI;

namespace Planetside
{

	/// <summary>
	/// TO DO
	/// </summary>
	public class ArmWarmer : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Arm Warmer", "heartthing");
			Game.Items.Rename("outdated_gun_mods:arm_warmer", "psog:arm_warmer");
			gun.gameObject.AddComponent<ArmWarmer>();
			GunExt.SetShortDescription(gun, "Mmmm Tasty..");
			GunExt.SetLongDescription(gun, "A large, amalgam of living organs.\n\nIt hungers for enemy bullets...");
			GunExt.SetupSprite(gun, null, "heartthing_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 25);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 10);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 5);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(83) as Gun, true, false);
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_PET_wolf_bite_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(336) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2f;
			gun.DefaultModule.cooldownTime = .3f;
			gun.DefaultModule.numberOfShotsInClip = -1;
			gun.SetBaseMaxAmmo(1);
			gun.quality = PickupObject.ItemQuality.D;
			gun.DefaultModule.angleVariance = 0f;
			gun.DefaultModule.burstShotCount = 1;
			gun.CanReloadNoMatterAmmo = true;
			gun.Volley.projectiles[0].ammoCost = 0;
			gun.InfiniteAmmo = true;
			gun.gunClass = GunClass.SILLY;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 0f;
			projectile.baseData.speed *= 1f;
			projectile.AdditionalScaleMultiplier = 0.5f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.sprite.renderer.enabled = false;
			projectile.baseData.range = 1f;
			Gun gun4 = PickupObjectDatabase.GetById(83) as Gun;
			gun.muzzleFlashEffects = gun4.muzzleFlashEffects;

			gun.encounterTrackable.EncounterGuid = "OM NOMNOMNOMNONMNONMONMNONMNONMONMNONMNO";
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.CONTRAIL);

			gun.barrelOffset.transform.localPosition = new Vector3(1f, 0.5f, 0f);
			gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.BEAT_LOOP_1, true);

			gun.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);

			ArmWarmer.ArmWarmerID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int ArmWarmerID;


		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
            {
				AkSoundEngine.PostEvent("Play_BOSS_doormimic_vomit_01", base.gameObject);
				int clipshotsremainingLast = gun.ClipShotsRemaining;
				gun.ClipShotsRemaining = gun.DefaultModule.numberOfShotsInClip - 1;
				gun.Reload();
				gun.ClipShotsRemaining = clipshotsremainingLast;
				if (EatenBullets >= 0)
				{
					float damage = EatenBullets;
					SpawnProjectile(damage);
					EatenBullets = 0;
				}
				this.HasReloaded = false;
			}
		}

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
		private bool HasReloaded;

		private void SpawnProjectile(float damage)
		{
			PlayerController player = this.gun.CurrentOwner as PlayerController;
			Vector2 position = new Vector2(player.CurrentGun.barrelOffset.position.x, player.CurrentGun.barrelOffset.position.y);
			GameObject gameObject = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(207) as Gun).DefaultModule.projectiles[0].gameObject, position, Quaternion.Euler(0f, 0f, ((player.CurrentGun == null) ? 1.2f : player.CurrentGun.CurrentAngle)), true);
			Projectile eatfarts = gameObject.GetComponent<Projectile>();
			bool flag12 = eatfarts != null;
			bool flag2 = flag12;
			if (flag2)
			{

				eatfarts.SpawnedFromOtherPlayerProjectile = true;
				eatfarts.Shooter = player.specRigidbody;
				eatfarts.Owner = player;
				//  eatfarts.Shooter = playerController1.specRigidbody;
				eatfarts.baseData.damage = (7.5f* damage)+7.5f;
				eatfarts.AdditionalScaleMultiplier = 0.66f;
				eatfarts.baseData.range = 15f;
				eatfarts.AdditionalScaleMultiplier *= 0.75f + (damage/25);
				eatfarts.SetOwnerSafe(player, "Player");
				eatfarts.ignoreDamageCaps = true;
				eatfarts.PenetratesInternalWalls = true;

			}
		}

		public override void PostProcessProjectile(Projectile projectile)
		{
			AkSoundEngine.PostEvent("Play_PET_wolf_bite_01", base.gameObject);
			projectile.baseData.range = 1f;
			PlayerController player = this.gun.CurrentOwner as PlayerController;
			Vector2 centerPosition = projectile.sprite.WorldCenter;
			bool flag3 = player.CurrentRoom != null;
			if (flag3)
			{
				ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
				if (allProjectiles != null)
				{
					GameManager.Instance.Dungeon.StartCoroutine(this.HandleBulletDeletionFrames(player.sprite.WorldCenter, 1.7f, 0.4f));
				}
			}
		}
		private IEnumerator HandleBulletDeletionFrames(Vector3 centerPosition, float bulletDeletionSqrRadius, float duration)
		{
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
				for (int i = allProjectiles.Count - 1; i >= 0; i--)
				{
					Projectile projectile = allProjectiles[i];
					if (projectile)
					{
						if (!(projectile.Owner is PlayerController))
						{
							Vector2 vector = (projectile.transform.position - centerPosition).XY();
							if (projectile.CanBeKilledByExplosions && vector.sqrMagnitude < bulletDeletionSqrRadius)
							{
								GameManager.Instance.Dungeon.StartCoroutine(this.HandleBulletSucc(projectile));
								projectile.DieInAir();
							}
						}
					}
				}
				yield return null;
			}
			yield break;

		}
		private IEnumerator HandleBulletSucc(Projectile target)
		{
			EatenBullets += 1;
			PlayerController player = this.gun.CurrentOwner as PlayerController;
			Transform copySprite = this.CreateEmptySprite(target);
			Vector3 startPosition = copySprite.transform.position;
			float elapsed = 0f;
			float duration = 0.4f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				bool flag3 = player.CurrentGun && copySprite;
				if (flag3)
				{
					Vector3 position = player.CurrentGun.PrimaryHandAttachPoint.position;
					float t = elapsed / duration * (elapsed / duration);
					copySprite.position = Vector3.Lerp(startPosition, position, t);
					copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
					copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
					position = default(Vector3);
				}
				yield return null;
			}
			bool flag4 = copySprite;
			if (flag4)
			{
				UnityEngine.Object.Destroy(copySprite.gameObject);
			}
			yield break;
		}
		private Transform CreateEmptySprite(Projectile target)
		{
			GameObject gameObject = new GameObject("suck image");
			gameObject.layer = target.gameObject.layer;
			tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
			gameObject.transform.parent = SpawnManager.Instance.VFX;
			tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
			tk2dSprite.transform.position = target.sprite.transform.position;
			GameObject gameObject2 = new GameObject("image parent");
			gameObject2.transform.position = tk2dSprite.WorldCenter;
			tk2dSprite.transform.parent = gameObject2.transform;

			return gameObject2.transform;
		}
		public AIActor AIActor;
		public PlayerController player;


		public void ApplyActionToNearbyBullets(Vector2 position, float radius, Action<Projectile, float> lambda)
		{
			float num = radius * radius;
			if (this.activeBullets != null)
			{
				for (int i = 0; i < this.activeBullets.Count; i++)
				{
					if (this.activeBullets[i])
					{
						bool flag = radius < 0f;
						Vector2 vector = this.activeBullets[i].sprite.WorldCenter - position;
						if (!flag)
						{
							flag = (vector.sqrMagnitude < num);
						}
						if (flag)
						{
							lambda(this.activeBullets[i], vector.magnitude);
						}
					}
				}
			}
		}
		public List<Projectile> activeBullets;
		public int EatenBullets = 0;
	}
}