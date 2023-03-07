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
using SaveAPI;

namespace Planetside
{
	public class Immateria : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Immateria", "immateria");
			Game.Items.Rename("outdated_gun_mods:immateria", "psog:immateria");
			var behav = gun.gameObject.AddComponent<Immateria>();
			GunExt.SetShortDescription(gun, "The Void");
			GunExt.SetLongDescription(gun, "Fires projectiles that bend the cosmos to their will.\n\nCreated by a bunch of jerks who got bored one day and decided to break physics. How inconsiderate of them!");
            
			GunExt.SetupSprite(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "immateria_idle_001", 11);
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

			gun.SetBaseMaxAmmo(180);

			//gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_BOSS_doormimic_blast_01";
			//gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[3].eventAudio = "Play_BOSS_dragun_spin_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[3].triggerEvent = true;


			gun.barrelOffset.transform.localPosition = new Vector3(2.0f, 0.4375f, 0f);
			gun.carryPixelOffset += new IntVector2((int)4f, (int)0f);
			gun.encounterTrackable.EncounterGuid = "I wanna set the Universe On Fire.";
			gun.gunClass = GunClass.SHOTGUN;


            for (int i = 0; i < 5; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(88) as Gun, true, true);
            }
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Immateria", "Planetside/Resources/GunClips/Immateria/immateriafull", "Planetside/Resources/GunClips/Immateria/immateriaempty");
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>();
			gun.gunHandedness = GunHandedness.OneHanded;
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(228) as Gun).gunSwitchGroup;

            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.ammoCost = 1;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.cooldownTime = 0.5f;
                projectileModule.angleVariance = 80;
                projectileModule.numberOfShotsInClip = 12;
                

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(88) as Gun).DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                projectile.baseData.damage = 12f;
                projectile.AdditionalScaleMultiplier = 1f;
                projectile.shouldRotate = false;
                projectile.baseData.range = 1000f;
				projectile.baseData.speed *= 0.4f;


				projectile.AnimateProjectileBundle("immateria_projectile_idle", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "immateria_projectile_idle", 
					new List<IntVector2>() {new IntVector2(12, 12), new IntVector2(12, 12), new IntVector2(12, 12), new IntVector2(12, 12), new IntVector2(12, 12), new IntVector2(12, 12), new IntVector2(12, 12)}, 
					AnimateBullet.ConstructListOfSameValues(true, 7), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.LowerLeft, 7), AnimateBullet.ConstructListOfSameValues(true, 7), AnimateBullet.ConstructListOfSameValues(false, 7),
					AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 7));

                projectileModule.projectiles[0] = projectile;

                PierceProjModifier spook = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                spook.penetration = 6;
                spook.penetratesBreakables = true;

                projectile.objectImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
                projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

                ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
                yes.spawnShadows = true;
                yes.shadowLifetime = 0.5f;
                yes.shadowTimeDelay = 0.03f;
                yes.dashColor = new Color(0.8f, 0f, 0.45f, 0.9f);
                yes.name = "Gun Trail";

                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                if (projectileModule != gun.DefaultModule)
                {
                    projectileModule.ammoCost = 0;
                }
				projectile.gameObject.AddComponent<ImmateriaProjectile>();
                 var a = projectile.gameObject.AddComponent<WraparoundProjectile>();
				a.Cap = 5;
            }
            gun.Volley.UsesShotgunStyleVelocityRandomizer = true;
			gun.Volley.DecreaseFinalSpeedPercentMin = 0.5f;
            gun.Volley.IncreaseFinalSpeedPercentMax = 1.5f;


            gun.reloadTime = 1.1f;

            gun.quality = PickupObject.ItemQuality.S;
			GameObject lightObj = new GameObject("LightObj");
			FakePrefab.MarkAsFakePrefab(lightObj);
			lightObj.transform.parent = gun.transform;
			Light glow = lightObj.AddComponent<Light>();
			glow.color = Color.cyan;
			glow.range = 2;
			glow.type = LightType.Area;
			glow.colorTemperature = 0.1f;
			glow.intensity = 10;
			gun.baseLightIntensity = 100;
			gun.light = glow;
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(228) as Gun).muzzleFlashEffects;

			ETGMod.Databases.Items.Add(gun, false, "ANY");
            gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.INFECTED_FLOOR_COMPLETED, true);

            Immateria.ImmateriaID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);

			ExplosionData = new ExplosionData()
			{
				breakSecretWalls = false,
				comprehensiveDelay = 0,
				damage = 10,
				damageRadius = 3f,
				damageToPlayer = 0,
				debrisForce = 10,
				doDamage = true,
				doDestroyProjectiles = false,
				doExplosionRing = false,
				doForce = false,
				doScreenShake = false,
				doStickyFriction = false,
				effect = (PickupObjectDatabase.GetById(593) as Gun).DefaultModule.projectiles[0].GetComponent<ExplosiveModifier>().explosionData.effect,
                explosionDelay = 0,
                force = 1,
                forcePreventSecretWallDamage = false,
                forceUseThisRadius = true,
                freezeEffect = null,
                freezeRadius = 0,
                IsChandelierExplosion = false,
                isFreezeExplosion = false,
                playDefaultSFX = false,
                preventPlayerForce = false,
                pushRadius = 3,
                secretWallsRadius = 1,
				ss = new ScreenShakeSettings()
				{
					direction = Vector2.zero,
					falloff = 0,
					magnitude = 0,
					simpleVibrationStrength = Vibration.Strength.UltraLight,
					simpleVibrationTime = Vibration.Time.Instant,
					speed = 0,
					time = 0,
					vibrationType = ScreenShakeSettings.VibrationType.None
				},
				ignoreList =  new List<SpeculativeRigidbody>(),
                overrideRangeIndicatorEffect = null,
				
            };
        }
        public static ExplosionData ExplosionData;
        public static int ImmateriaID;		
	}
}
