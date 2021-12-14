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
	public class Polarity : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Polarity", "polarity");
			Game.Items.Rename("outdated_gun_mods:polarity", "psog:polarity");
			gun.gameObject.AddComponent<Polarity>();
			GunExt.SetShortDescription(gun, "Climate Contrast");
			GunExt.SetLongDescription(gun, "A weapon forged by two wanderers from polar-opposite climates. It is said that they imbued part of their magic into it so one element doesn't damage the other.");
			GunExt.SetupSprite(gun, null, "polarity_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 48);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 12);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 3);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(223) as Gun, true, false);
			gun.SetBaseMaxAmmo(450);
			
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_yarirocketlauncher_shot_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
			Gun gun2 = PickupObjectDatabase.GetById(336) as Gun;
			Gun gun3 = PickupObjectDatabase.GetById(146) as Gun;
			Projectile component = ((Gun)ETGMod.Databases.Items[336]).DefaultModule.projectiles[0];
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(16) as Gun).gunSwitchGroup;
			Projectile replacementProjectile = component.projectile;
			gun.DefaultModule.usesOptionalFinalProjectile = true;
			PolarityProjectile pol1 = replacementProjectile.gameObject.AddComponent<PolarityProjectile>();
			pol1.IsDown = true;
			gun.DefaultModule.numberOfFinalProjectiles = 15;
			gun.DefaultModule.finalProjectile = replacementProjectile;
			gun.DefaultModule.finalCustomAmmoType = gun3.DefaultModule.customAmmoType;
			gun.DefaultModule.finalAmmoType = gun3.DefaultModule.ammoType;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.damageModifier = 1;
			gun.reloadTime = 1.6f;
			gun.DefaultModule.cooldownTime = 0.05f;
			gun.DefaultModule.numberOfShotsInClip = 30;
			gun.DefaultModule.angleVariance = 4f;
			gun.barrelOffset.transform.localPosition += new Vector3(2.25f, 0.0625f, 0f);
			gun.quality = PickupObject.ItemQuality.S;
			gun.encounterTrackable.EncounterGuid = "opposites attract";
			gun.gunClass = GunClass.RIFLE;
			gun.CanBeDropped = true;
			Gun gun4 = PickupObjectDatabase.GetById(387) as Gun;
			gun.muzzleFlashEffects = gun4.muzzleFlashEffects;
			Gun gun5 = PickupObjectDatabase.GetById(384) as Gun;
			gun.finalMuzzleFlashEffects = gun5.muzzleFlashEffects;
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]); projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.transform.parent = gun.barrelOffset;
			projectile.AdditionalScaleMultiplier *= 1.33f;
			projectile.baseData.damage = 6.5f;
			PolarityProjectile aaaaaaa = projectile.gameObject.AddComponent<PolarityProjectile>();
			aaaaaaa.IsUp = true;
			Polarity.PolarityID = gun.PickupObjectId;

			List<string> mandatoryConsoleIDs1 = new List<string>
			{
				"psog:polarity"
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"hot_lead",
				"copper_ammolet",
				"napalm_strike",
				"ring_of_fire_resistance",
				"flame_hand",
				"phoenix",
				"pitchfork",
				"frost_bullets",
				"frost_ammolet",
				"heart_of_ice",
				"ice_cube",
				"ice_bomb",
				"glacier"
			};
			CustomSynergies.Add("Refridgeration", mandatoryConsoleIDs1, optionalConsoleIDs, true);
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int PolarityID;


		private bool HasReloaded;

		public Vector3 projectilePos;

		public override void PostProcessProjectile(Projectile projectile)
		{
			
		}
		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			this.HalfOFClip(player);
		}
		public class EyeProjUp : MonoBehaviour
		{
		}

		// Token: 0x020000E6 RID: 230
		public class EyeProjDown : MonoBehaviour
		{
		}
		protected void Update()
		{
			if (gun.CurrentOwner)
			{
				PlayerController player = gun.CurrentOwner as PlayerController;
				this.HalfOFClip(player);
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
		public override void OnInitializedWithOwner(GameActor actor)
		{
			base.OnInitializedWithOwner(actor);
			PlayerController player = actor as PlayerController;
			this.HalfOFClip(player);
		}
		public void HalfOFClip(PlayerController player)
		{
			float clip = 0f;
			clip = (player.stats.GetStatValue(PlayerStats.StatType.AdditionalClipCapacityMultiplier));
			int num = (int)(30 * clip);
			int num2 = 0;
			
			num2 = (int)(num /2);

			foreach (ProjectileModule projectileModule in this.gun.Volley.projectiles)
			{
				this.gun.DefaultModule.numberOfFinalProjectiles = num2;
			}

		}


	}
}
