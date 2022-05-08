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
	public class ParticleCollapser : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Particle Collapser", "particlecollapser");
			Game.Items.Rename("outdated_gun_mods:particle_collapser", "psog:particle_collapser");
			gun.gameObject.AddComponent<ParticleCollapser>();
			GunExt.SetShortDescription(gun, "<   (  ( o )  )  >");
			GunExt.SetLongDescription(gun, "First shot in the clip fires a small rift, all shots after will be attracted to it.\n\nUsed by AI lumberjacks to cut down laser trees.");
			GunExt.SetupSprite(gun, null, "particlecollapser_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 30);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 6);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 2);
			GunExt.SetAnimationFPS(gun, gun.finalShootAnimation, 70);


			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);
			gun.SetBaseMaxAmmo(630);

			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.reloadTime = 2.6f;
			gun.DefaultModule.cooldownTime = 0.075f;
			gun.DefaultModule.numberOfShotsInClip = 21;
			gun.DefaultModule.angleVariance = 14f;
			gun.barrelOffset.transform.localPosition = new Vector3(1.125f, 0.5f, 0f);
			gun.quality = PickupObject.ItemQuality.A;
			gun.encounterTrackable.EncounterGuid = "abwehikygyuhj";
			gun.CanBeDropped = true;

			EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.shootAnimation, new Dictionary<int, string> { { 0, "Play_WPN_bsg_shot_01" } });
			EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.finalShootAnimation, new Dictionary<int, string> { { 0, "Play_WPN_looper_shot_01" } });


			Gun gun4 = PickupObjectDatabase.GetById(274) as Gun;
			gun.muzzleFlashEffects = gun4.muzzleFlashEffects;
			gun.gunClass = GunClass.FULLAUTO;
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(169) as Gun).gunSwitchGroup;
			gun.OverrideFinaleAudio = true;

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = (PickupObjectDatabase.GetById(169) as Gun).DefaultModule.customAmmoType;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]); projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			projectile.SetProjectileSpriteRight("collapserNeedle_001", 8, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 10, 7);

			ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
			yes.spawnShadows = true;
			yes.shadowLifetime = 0.375f;
			yes.shadowTimeDelay = 0.01f;
			yes.dashColor = new Color(1f, 0f, 0.66f, 1f);
			projectile.gameObject.AddComponent<ParticleCollapserSmallProjectile>();
			projectile.shouldRotate = true;
			projectile.baseData.range = 200;

			projectile.objectImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
			projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;
			PierceProjModifier spook = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
			spook.penetration = 5;
			spook.penetratesBreakables = true;
			MaintainDamageOnPierce noDamageLoss = projectile.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
			noDamageLoss.damageMultOnPierce = 1f;
			BounceProjModifier BounceProjMod = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
			BounceProjMod.bouncesTrackEnemies = false;
			BounceProjMod.numberOfBounces = 3;

			gun.DefaultModule.numberOfFinalProjectiles = 20;
			gun.DefaultModule.finalProjectile = projectile;
			gun.DefaultModule.finalAmmoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.finalCustomAmmoType = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.customAmmoType;
			gun.DefaultModule.ammoCost = 1;
			gun.finalMuzzleFlashEffects = (PickupObjectDatabase.GetById(334) as Gun).muzzleFlashEffects;
			gun.DefaultModule.usesOptionalFinalProjectile = true;

			Projectile CollapseProjectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]); 
			CollapseProjectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(CollapseProjectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(CollapseProjectile);
			gun.DefaultModule.projectiles[0] = CollapseProjectile;
			CollapseProjectile.AnimateProjectile(new List<string> {
				"colalsped_001",
				"colalsped_002",
				"colalsped_003",
				"colalsped_004",
				"colalsped_005",
				"colalsped_006",

			}, 11, true, new List<IntVector2> {
				new IntVector2(9, 9),
				new IntVector2(9, 9),
				new IntVector2(9, 9),
				new IntVector2(9, 9),
				new IntVector2(9, 9),
				new IntVector2(9, 9)

			}, AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 7), AnimateBullet.ConstructListOfSameValues(true, 7), AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 7));


			CollapseProjectile.AdditionalScaleMultiplier *= 1;
			CollapseProjectile.baseData.damage = 20f;
			CollapseProjectile.baseData.speed = 7;
			CollapseProjectile.pierceMinorBreakables = true;
			CollapseProjectile.hitEffects.alwaysUseMidair = true;
			CollapseProjectile.hitEffects.overrideMidairDeathVFX = (PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
			CollapseProjectile.baseData.range = 1000000;
			CollapseProjectile.objectImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
			CollapseProjectile.enemyImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;
			CollapseProjectile.gameObject.AddComponent<ParticleCollapserLargeProjectile>();
			CollapseProjectile.shouldRotate = true;
				PierceProjModifier spookCollapse = CollapseProjectile.gameObject.GetOrAddComponent<PierceProjModifier>();
			spookCollapse.penetration = 10;
			spookCollapse.penetratesBreakables = true;
				MaintainDamageOnPierce noDamageLossCollapse = CollapseProjectile.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
			noDamageLossCollapse.damageMultOnPierce = 1f;
				BounceProjModifier BounceProjModCollapse = CollapseProjectile.gameObject.GetOrAddComponent<BounceProjModifier>();
			BounceProjModCollapse.bouncesTrackEnemies = false;
			BounceProjModCollapse.numberOfBounces = 4;

			CollapseProjectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
			CollapseProjectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
			CollapseProjectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
			CollapseProjectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);

			CollapseProjectile.objectImpactEventName = (PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
			CollapseProjectile.enemyImpactEventName = (PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;


			gun.gameObject.transform.Find("Casing").transform.position = new Vector3(1.1875f, 0.4375f);
			gun.shellCasing = BreakAbleAPI.BreakableAPIToolbox.GenerateDebrisObject("Planetside/Resources/GunObjects/Casings/collapsercasing.png").gameObject;
			gun.shellsToLaunchOnFire = 1;
			gun.shellsToLaunchOnReload = 0;
			gun.reloadShellLaunchFrame = 1;
			gun.shellCasingOnFireFrameDelay = 0;


			gun.gameObject.transform.Find("Clip").transform.position = new Vector3(0.875f, 0.3125f);
			gun.clipObject = BreakAbleAPI.BreakableAPIToolbox.GenerateDebrisObject("Planetside/Resources/GunObjects/Clips/collasperClip.png").gameObject;
			gun.reloadClipLaunchFrame = 1;
			gun.clipsToLaunchOnReload = 1;


			GameObject lightObj = new GameObject("LightObj");
			FakePrefab.MarkAsFakePrefab(lightObj);
			lightObj.transform.parent = gun.transform;
			Light glow = lightObj.AddComponent<Light>();
			glow.color = new Color(2550, 0, 1550);
			glow.range = 2;
			glow.type = LightType.Area;
			glow.colorTemperature = 0.1f;
			glow.intensity = 10;
			gun.baseLightIntensity = 100;
			gun.light = glow;
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(338) as Gun).muzzleFlashEffects;

			ETGMod.Databases.Items.Add(gun, null, "ANY");
			ParticleCollapser.ParticleCollapserID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);			
		}
		public static int ParticleCollapserID;

	
		protected override void Update()
		{
			base.Update();
			
		}
		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			base.OnReloadPressed(player, gun, bSOMETHING);
			bool flag = gun.ClipCapacity == gun.ClipShotsRemaining || gun.CurrentAmmo == gun.ClipShotsRemaining;
			if (flag)
			{
				AkSoundEngine.PostEvent("Play_OBJ_supplydrop_activate_01", player.gameObject);
				for (int e = 0; e < allRifts.Count; e++)
                {
					Projectile rift = allRifts[e];
					if (rift != null)
                    {rift.DieInAir(); Exploder.DoDistortionWave(rift.transform.PositionVector2(), 1, 0.2f, 3, 0.4f);}
				}
			}
		}
		public static List<Projectile> allRifts = new List<Projectile>();
	}
}
