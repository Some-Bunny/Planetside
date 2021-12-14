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


using UnityEngine.Serialization;

namespace Planetside
{
	public class BanditsRevolver : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Bandits Revolver", "banditsrevolver");
			Game.Items.Rename("outdated_gun_mods:bandits_revolver", "psog:bandits_revolver");
			gun.gameObject.AddComponent<BanditsRevolver>();
			GunExt.SetShortDescription(gun, "Lights Out");
			GunExt.SetLongDescription(gun, "The thrill of landing the perfect shot is seeked out by many a Gunslinger, with some devoting their lives to landing their swan song shot. Don't waste your chance when your high-noon comes.");
			GunExt.SetupSprite(gun, null, "banditsrevolver_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 24);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 12);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 3);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(62) as Gun, true, false);
			gun.SetBaseMaxAmmo(240);
			
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_colt1851_shot_03";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;

			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_WPN_SAA_reload_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;

			Gun gun3 = PickupObjectDatabase.GetById(223) as Gun;
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(62) as Gun).gunSwitchGroup;
			Gun gun2 = PickupObjectDatabase.GetById(378) as Gun;

			Projectile projectile1 = UnityEngine.Object.Instantiate<Projectile>(gun2.DefaultModule.projectiles[0]); projectile1.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile1.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile1);


			Projectile replacementProjectile = projectile1.projectile;
			replacementProjectile.baseData.damage = 15;
			replacementProjectile.gameObject.AddComponent<BanditsRevolverFinaleProjectile>();
			replacementProjectile.baseData.speed *= 5f;


			OtherTools.EasyTrailComponent trail = replacementProjectile.gameObject.AddComponent<OtherTools.EasyTrailComponent>();
			trail.TrailPos = replacementProjectile.transform.position;
			trail.StartColor = Color.white;
			trail.StartWidth = 0.1f;
			trail.EndWidth = 0;
			trail.LifeTime = 1f;
			trail.BaseColor = new Color(1f, 1f, 6f, 2f);
			trail.EndColor = new Color(0f, 0f, 1f, 0f);

			gun.DefaultModule.usesOptionalFinalProjectile = true;

			gun.DefaultModule.numberOfFinalProjectiles = 1;
			gun.DefaultModule.finalProjectile = replacementProjectile;
			gun.DefaultModule.finalCustomAmmoType = gun3.DefaultModule.customAmmoType;
			gun.DefaultModule.finalAmmoType = gun3.DefaultModule.ammoType;
			
			Gun gun5 = PickupObjectDatabase.GetById(383) as Gun;
			gun.finalMuzzleFlashEffects = gun5.muzzleFlashEffects;


			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.damageModifier = 1;
			gun.reloadTime = 2.5f;
			gun.DefaultModule.cooldownTime = 0.166f;
			gun.DefaultModule.numberOfShotsInClip = 6;
			gun.DefaultModule.angleVariance = 6f;
			gun.barrelOffset.transform.localPosition = new Vector3(1.125f, 0.5f, 0f);
			gun.quality = PickupObject.ItemQuality.C;
			gun.encounterTrackable.EncounterGuid = "Another Goddamn Risk Of Rain reference ";
			gun.gunClass = GunClass.PISTOL;
			gun.CanBeDropped = true;
			Gun gun4 = PickupObjectDatabase.GetById(50) as Gun;
			gun.muzzleFlashEffects = gun4.muzzleFlashEffects;
			gun.gunClass = GunClass.PISTOL;



			ETGMod.Databases.Items.Add(gun, null, "ANY");
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]); projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.transform.parent = gun.barrelOffset;
			projectile.AdditionalScaleMultiplier *= 1f;
			projectile.baseData.damage = 10f;
			projectile.baseData.speed *= 2;
			BanditsRevolver.BanditsRevolverID = gun.PickupObjectId;

			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int BanditsRevolverID;


		private bool HasReloaded;

		public Vector3 projectilePos;

		protected override void OnPickup(PlayerController player)
		{
			base.OnPickup(player);
			player.GunChanged += this.OnGunChanged;
			CanMark = true;
		}

		protected override void OnPostDrop(PlayerController player)
		{
			CanMark = true;
			player.GunChanged -= this.OnGunChanged;
			base.OnPostDrop(player);
			List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
			if (activeEnemies != null)
			{
				foreach (AIActor aiactor in activeEnemies)
				{
					MarkWithColorComponent yes = aiactor.GetComponent<MarkWithColorComponent>();
					if (yes != null)
					{
						Destroy(yes);
					}
				}
			}
		}

		private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
		{
			if (this.gun && this.gun.CurrentOwner)
			{
				PlayerController player = this.gun.CurrentOwner as PlayerController;
				if (newGun == this.gun)
				{
					CanMark = true;
				}
				else
                {
					CanMark = false;
					List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
					if (activeEnemies != null)
					{
						foreach (AIActor aiactor in activeEnemies)
						{
							MarkWithColorComponent yes = aiactor.GetComponent<MarkWithColorComponent>();
							if (yes != null)
                            {
								Destroy(yes);
							}
						}
					}
				}
			}
		}
		public bool CanMark;
		protected override void Update()
		{
			base.Update();
			if (gun.CurrentOwner as PlayerController)
			{
				PlayerController player = gun.CurrentOwner as PlayerController;
				if (player.CurrentGun == gun && CanMark == true)
				{
					List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
					if (activeEnemies != null)
					{
						foreach (AIActor aiactor in activeEnemies)
						{
							if (aiactor.gameObject.GetComponent<MarkWithColorComponent>() == null)
                            {
								MarkWithColorComponent mark = aiactor.gameObject.AddComponent<MarkWithColorComponent>();
								mark.ai = aiactor;
								mark.playa = player;
							}
						}
					}
				}
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}

			}
		}
	}
}
