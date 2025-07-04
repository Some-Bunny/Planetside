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
	public class LilPew : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Air Blaster", "lilpew");
			Game.Items.Rename("outdated_gun_mods:air_blaster", "psog:air_blaster");
			gun.gameObject.AddComponent<LilPew>();
			gun.SetShortDescription("Pump-Action");
			gun.SetLongDescription("Fires slower shots the less shots you have in your clip. Reloading knocks foes back based on how full it was.\n\nThe future is now! Guns full of air, just like your local leaders!");
            
			GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "lilpew_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.reloadAnimation = "lilpew_reload";
            gun.idleAnimation = "lilpew_idle";
            gun.shootAnimation = "lilpew_fire";



            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[4].eventAudio = "Play_WPN_mailbox_reload_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[4].triggerEvent = true;
			
			for (int i = 0; i < 1; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(35) as Gun, true, false);
				gun.Volley.projectiles[i].ammoCost = 1;
				gun.Volley.projectiles[i].shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				gun.Volley.projectiles[i].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				gun.Volley.projectiles[i].cooldownTime = 0.1f;
				gun.Volley.projectiles[i].angleVariance = 7f;
				gun.Volley.projectiles[i].numberOfShotsInClip = 20;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[0].projectiles[0]);
				projectile.gameObject.SetActive(false);
				gun.Volley.projectiles[i].projectiles[0] = projectile;
				projectile.baseData.damage = 7f;
				projectile.baseData.speed = 12f;
				projectile.gameObject.AddComponent<AirBlasterProjectile>();
				projectile.SetProjectileSpriteRight("lilpew_projectile", 8, 3, false, tk2dBaseSprite.Anchor.MiddleCenter, 8, 3);
				projectile.shouldRotate = true;
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = gun.Volley.projectiles[i] != gun.DefaultModule;
				if (flag)
				{
					gun.Volley.projectiles[i].ammoCost = 0;
				}
			}
			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Lilpew", "Planetside/Resources/GunClips/Lilpew/lilpew_clipfull", "Planetside/Resources/GunClips/Lilpew/lilpew_clipempty");
			gun.gunClass = GunClass.PISTOL;
			gun.barrelOffset.transform.localPosition = new Vector3(0.8125f, 0.4375f, 0f);
			gun.reloadTime = 1.4f;
			gun.SetBaseMaxAmmo(320);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(150) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.D;
			gun.encounterTrackable.EncounterGuid = "air pew pewp pewp ewpe wpewpe wp";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			List<string> yes = new List<string>
			{
				"psog:air_blaster",
				"cigarettes"
			};
			CustomSynergies.Add("Puff", yes, null, false);
			/*
			List<string> syn = new List<string>
			{
				"psog:air_blaster",
			};
			List<string> ballin = new List<string>
			{
				""
			};
			CustomSynergies.Add("5th Day, Air Day", syn, ballin, true);
			*/
			LilPew.LilPewID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int LilPewID;

		
		public override void OnPostFired(PlayerController player, Gun bruhgun)
		{
			gun.PreventNormalFireAudio = true;
			AkSoundEngine.PostEvent("Play_WPN_dartgun_shot_01", gameObject);
			AkSoundEngine.PostEvent("Play_OBJ_ash_burst_01", gameObject);
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
				if (gun.ClipShotsRemaining != 0)
                {
					Exploder.DoRadialPush(base.gameObject.transform.PositionVector2(), 20 * gun.ClipShotsRemaining,5);
					Exploder.DoRadialKnockback(base.gameObject.transform.PositionVector2(), 20 * gun.ClipShotsRemaining, 5);
					Exploder.DoRadialMinorBreakableBreak(base.gameObject.transform.PositionVector2(), 5);
					GameObject PoofVFX = GameManager.Instance.RewardManager.D_Chest.VFX_PreSpawn;
					PoofVFX.SetActive(true);
					GameObject obj = UnityEngine.Object.Instantiate<GameObject>(PoofVFX, gun.CurrentOwner.transform.position - new Vector3(1.5f, 1), Quaternion.identity);
					obj.transform.parent = player.transform;
					if (player.PlayerHasActiveSynergy("Puff"))
					{
						float dmg = (player.stats.GetStatValue(PlayerStats.StatType.Damage));
                        float coolness = (player.stats.GetStatValue(PlayerStats.StatType.Coolness));

                        Exploder.DoRadialDamage((gun.ClipShotsRemaining*dmg + coolness) * 2.5f, base.gameObject.transform.PositionVector2(), 5.5f, false, true, true, null);
					}
					AkSoundEngine.PostEvent("Play_CHR_weapon_charged_01", gameObject);
				}
			}
		}
	}
}