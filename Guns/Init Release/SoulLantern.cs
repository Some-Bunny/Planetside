﻿using System;
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

namespace Planetside
{
	public class SoulLantern : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Soul Lantern", "soullantern");
			Game.Items.Rename("outdated_gun_mods:soul_lantern", "psog:soul_lantern");
			gun.gameObject.AddComponent<SoulLantern>();

			GunExt.SetShortDescription(gun, "Light The Way");
			GunExt.SetLongDescription(gun, "A lantern filled with the souls that came before and after its time. Who knows how old it is, and if it even was manufactured by human hands...");
            GunInt.SetupSprite(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "soullantern_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.idleAnimation = "soullantern_idle";
            gun.shootAnimation = "soullantern_fire";
            gun.reloadAnimation = "soullantern_reload";

            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(378) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(370) as Gun).gunSwitchGroup;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_Life_Orb_Fade_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 0f;
			gun.DefaultModule.cooldownTime = .2f;
			gun.DefaultModule.numberOfShotsInClip = -1;
			gun.SetBaseMaxAmmo(200);
			gun.quality = PickupObject.ItemQuality.B;
			gun.DefaultModule.angleVariance = 30f;
			gun.gunClass = GunClass.POISON;


			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Soul Lantern", "Planetside/Resources/GunClips/SoulLantern/soullaternfull", "Planetside/Resources/GunClips/SoulLantern/soullaternempty");

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);

			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 3f;
			projectile.baseData.speed = 1f;
			projectile.baseData.UsesCustomAccelerationCurve = true;
			projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1, 2, 18);
			projectile.sprite.renderer.enabled = false;
			projectile.AdditionalScaleMultiplier *= 0.5f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.gameObject.AddComponent<SoulLanternProjectile>();
			
			//projectile.PoisonApplyChance = 100;

			//projectile.baseData.range = 5.8f;
			gun.carryPixelOffset = new IntVector2((int)2f, (int)2f);
			PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
			spook.penetration = 1;


			HomingModifier homing = projectile.gameObject.AddComponent<HomingModifier>();
			homing.HomingRadius = 250f;
			homing.AngularVelocity = 120f;

			OtherTools.EasyTrailComponent trail = projectile.gameObject.AddComponent<OtherTools.EasyTrailComponent>();

			trail.TrailPos = projectile.transform.position;
			//Where the trail attaches itself to. z`
			//You can input a custom Vector3 but its best to use the base preset, since it usually attaches directly to the center of the projectile, even ones with custom sprites. , unless fuckery ensues. (Namely"projectile.transform.position;")

			trail.StartWidth = 0.1f;
			//The Starting Width of your Trail

			trail.EndWidth = 0;
			//The Ending Width of your Trail. Not sure why youd want it to be something other than 0, but the options there.

			trail.LifeTime = 0.4f;
			//How much time your trail lives for

			trail.BaseColor = new Color(3f, 2f, 0f, 0.7f);
			//The Base Color of your trail. Your trail will completely/mostly consist of this color

			trail.StartColor = Color.white;
			//The Starting Color of your trail. Ive not really found much use of this if you have a Base Color set, but maybe it has a use 

			trail.EndColor = new Color(3f, 0.75f, 0f, 0f);
			//The End color of your trail. Having it different to the StartColor/BaseColor will make it transition from the Starting/Base Color to its End Color during its lifetime.

			gun.encounterTrackable.EncounterGuid = "The Jar";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			gun.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);
			gun.PreventNormalFireAudio = true;
			gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.BULLETBANK_DEFEATED, true);
			gun.barrelOffset.transform.localPosition -= new Vector3(0.25f, -0.125f, 0f);

			GameObject lightObj = new GameObject("LightObj");
			FakePrefab.MarkAsFakePrefab(lightObj);
			lightObj.transform.parent = gun.transform;
			Light glow = lightObj.AddComponent<Light>();
			glow.color = Color.yellow;
			glow.range = 2;
			glow.type = LightType.Area;
			glow.colorTemperature = 0.1f;
			glow.intensity = 10;
			gun.baseLightIntensity = 100;
			gun.light = glow;
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(372) as Gun).muzzleFlashEffects;//new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };


			SoulLantern.SoulLanternID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int SoulLanternID;

		private bool HasReloaded;

		public Texture _gradTexture;
		

		public override void OnPostFired(PlayerController player, Gun bruhgun)
		{
			bruhgun.PreventNormalFireAudio = true;
		}
		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
		}
	}
}