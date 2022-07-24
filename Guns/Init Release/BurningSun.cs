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
	public class BurningSun : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Burning Sun", "burningsun");
			Game.Items.Rename("outdated_gun_mods:burning_sun", "psog:burning_sun");
			var behav = gun.gameObject.AddComponent<BurningSun>();
			//behav.preventNormalReloadAudio = true;
			//behav.overrideNormalReloadAudio = "Play_BOSS_doormimic_appear_01";
			GunExt.SetShortDescription(gun, "Ring Of Fire");
			GunExt.SetLongDescription(gun, "Despite its looks, this portable weapon can generate, and fire miniature stars. As one would expect from firing a star, it is dangerous to anything near it.");
			GunExt.SetupSprite(gun, null, "burningsun_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 8);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 10);
			GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 10);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 5);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(384) as Gun, true, false);
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2f;
			gun.DefaultModule.cooldownTime = 1f;
			gun.DefaultModule.numberOfShotsInClip = 1;
			gun.SetBaseMaxAmmo(50);
			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BLASTER;

			EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(),gun.chargeAnimation, new Dictionary<int, string> { { 1, "Play_WPN_bountyhunterarm_charge_01" }, { 2, "Play_ENM_bigbulletboy_step_01" }, { 4, "Play_FS_bone_step_01" }, { 7, "Play_WPN_rpg_reload_01" }, { 15, "Play_OBJ_mine_beep_01" } });
			EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.shootAnimation, new Dictionary<int, string> { { 0, "Play_wpn_chargelaser_shot_01" }, { 1, "Play_BOSS_lichB_charge_01" } });
			EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> { { 9, "Play_OBJ_lock_unlock_01" }, { 10, "Play_BOSS_lichC_zap_01" }, { 11, "Play_OBJ_metalskin_end_01" } });


			gun.barrelOffset.transform.localPosition = new Vector3(2.0f, 0.4375f, 0f);
			gun.carryPixelOffset += new IntVector2((int)4f, (int)0f);
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 16;
			gun.gunClass = GunClass.FIRE;


			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Big Flame", "Planetside/Resources/GunClips/BurningSun/burningsunfull", "Planetside/Resources/GunClips/BurningSun/burningsunempty");

			gun.encounterTrackable.EncounterGuid = "Here comes the sun, dudududu";
			Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(146) as Gun).DefaultModule.projectiles[0]);
			projectile2.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile2);
			gun.DefaultModule.projectiles[0] = projectile2;
			projectile2.baseData.damage = 35f;
			projectile2.baseData.speed *= 0.2f;
			projectile2.baseData.force *= 1f;
			projectile2.baseData.range = 100f;
			projectile2.AdditionalScaleMultiplier *= 1.33f;
			projectile2.transform.parent = gun.barrelOffset;
			projectile2.HasDefaultTint = true;

			projectile2.objectImpactEventName = (PickupObjectDatabase.GetById(336) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
			projectile2.enemyImpactEventName = (PickupObjectDatabase.GetById(336) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

			projectile2.AnimateProjectile(new List<string> {
				"burningsun_projectile_001",
				"burningsun_projectile_002",
				"burningsun_projectile_003",
				"burningsun_projectile_004"
			}, 5, true, new List<IntVector2> {
				new IntVector2(14, 14), 
                new IntVector2(14, 14),         
                new IntVector2(14, 14), 
                new IntVector2(14, 14),
			}, AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 7), AnimateBullet.ConstructListOfSameValues(true, 7), AnimateBullet.ConstructListOfSameValues(false, 7),AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 7));

			AoEDamageComponent Values = projectile2.gameObject.AddComponent<AoEDamageComponent>();
			Values.DamageperDamageEvent = 1;
			Values.Radius = 4;
			Values.TimeBetweenDamageEvents = 0.33f;
			Values.DealsDamage = true;
			Values.DamageperDamageEvent = 5;
			Values.DamageValuesAlsoScalesWithDamageStat = true;
			Values.InflictsFire = true;
			Values.HeatStrokeSynergy = true;
			Values.EffectProcChance = 1;
			projectile2.gameObject.AddComponent<BurningSunProjectile>();



			/*
			OtherTools.EasyTrailComponent trail = projectile2.gameObject.AddComponent<OtherTools.EasyTrailComponent>();
			trail.TrailPos = projectile2.transform.position;
			trail.StartColor = Color.white;
			trail.StartWidth = 0.7f;
			trail.EndWidth = 0;
			trail.LifeTime = 1f;
			trail.BaseColor = new Color(5f, 1f, 0f, 2f);
			trail.EndColor = new Color(5f, 1f, 1f, 0f);
			*/

			/*
			foreach (ProjectileModule.ChargeProjectile chargeProj in (PickupObjectDatabase.GetById(390) as Gun).DefaultModule.chargeProjectiles)
			{
				gun.DefaultModule.chargeProjectiles.Add(new ProjectileModule.ChargeProjectile
				{
					VfxPool = chargeProj.VfxPool
				});
			}
			*/
			ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile2,
				ChargeTime = 2.2f,
				
			};
			gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
			{
				item2
			};

			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.A;
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			List<string> mandatoryConsoleIDs = new List<string>
			{
				"psog:burning_sun",
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"psog:wisp_in_a_bottle",
				"sunlight_javelin",
				"old_knights_flask",
				"gun_soul"
			};


			CustomSynergies.Add("Praise The Gun!", mandatoryConsoleIDs, optionalConsoleIDs, true);
			BurningSun.BurningSunId = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int BurningSunId;


		private bool HasReloaded;

		public Vector3 projectilePos;
		public override void OnPostFired(PlayerController player, Gun flakcannon)
		{
			gun.PreventNormalFireAudio = true;
		}
		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				HasReloaded = false;
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
		}


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
	}
}
