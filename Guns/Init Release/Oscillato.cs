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
	public class Oscillato : GunBehaviour
	{



		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Oscillator", "oscillato");
			Game.Items.Rename("outdated_gun_mods:oscillator", "psog:oscillator");
			gun.gameObject.AddComponent<Oscillato>();
			gun.SetShortDescription("With Slowing Grace");
			gun.SetLongDescription("A limited edition burst fire weapon, who's funding was cut after all the ammunition for it was made on a tectonically unstable planet.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "oscillator_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.reloadAnimation = "oscillator_reload";
            gun.idleAnimation = "oscillator_idle";
            gun.shootAnimation = "oscillator_fire";


            //GunExt.SetupSprite(gun, null, "oscillato_idle_001", 11);	
            //GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
            //GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 7);
            //GunExt.SetAnimationFPS(gun, gun.idleAnimation, 5);


            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(89) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2.8f;
			gun.DefaultModule.cooldownTime = .25f;
			gun.DefaultModule.numberOfShotsInClip = 60;
			gun.SetBaseMaxAmmo(600);
			gun.quality = PickupObject.ItemQuality.C;
			gun.DefaultModule.angleVariance = 8f;
			gun.DefaultModule.burstShotCount = 4;
			gun.DefaultModule.burstCooldownTime = 0.025f;
			gun.gunClass = GunClass.FULLAUTO;


			Gun gun2 = PickupObjectDatabase.GetById(156) as Gun;
			gun.DefaultModule.ammoType = gun2.DefaultModule.ammoType;
			gun.DefaultModule.customAmmoType = gun2.DefaultModule.customAmmoType;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 4.5f;
			projectile.baseData.speed *= 0.7f;

            projectile.AdditionalScaleMultiplier *= 1f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.objectImpactEventName = (PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
			projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

			//projectile.gameObject.AddComponent<DestroyThyself>();
            
			


            projectile.gameObject.AddComponent<OscillatorProjectile>();
			projectile.AnimateProjectile(new List<string> {
				"oscillato_projectile_001",
				"oscillato_projectile_002",
				"oscillato_projectile_003",
				"oscillato_projectile_004",
				"oscillato_projectile_005",
				"oscillato_projectile_006",
				"oscillato_projectile_007"
			}, 13, true, new List<IntVector2> {
				new IntVector2(7, 5), //1
                new IntVector2(7, 5), //2            
                new IntVector2(7, 5), //3
                new IntVector2(7, 5),//4
                new IntVector2(7, 5),//5
                new IntVector2(7, 5),//6
                new IntVector2(7, 5),
            }, AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 7), AnimateBullet.ConstructListOfSameValues(true, 7), AnimateBullet.ConstructListOfSameValues(false, 7),
			AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 7));

			projectile.SetProjectileSpriteRight("oscillato_projectile_001", 7, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 7, 5);

			PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
			spook.penetration = 1;
			spook.penetratesBreakables = true;
			gun.encounterTrackable.EncounterGuid = "https://www.youtube.com/watch?v=P5ChKb_9JoY";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			List<string> AAA = new List<string>
			{
				"psog:oscillator",
				"psog:oscillating_bullets",
			};
			CustomSynergies.Add("Reverberation", AAA, null, false);
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
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(199) as Gun).muzzleFlashEffects;


			Oscillato.AAID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int AAID;
		public override void PostProcessProjectile(Projectile projectile)
		{
		}
		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			
		}
	}
}