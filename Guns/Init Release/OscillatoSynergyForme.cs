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
	public class OscillatoSynergyForme : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Antarctic Oscillator", "oscillatosynergy");
			Game.Items.Rename("outdated_gun_mods:antarctic_oscillator", "psog:antarctic_oscillator");
			gun.gameObject.AddComponent<OscillatoSynergyForme>();
			gun.SetShortDescription("DA NANANANANANANANNANANANAN BAM");
			gun.SetLongDescription("If you see this you owe me 20 yen");
            //GunExt.SetupSprite(gun, null, "oscillatosynergy_idle_001", 11);	

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "oscillatorsynergy_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.reloadAnimation = "oscillatorsynergy_reload";
            gun.idleAnimation = "oscillatorsynergy_idle";
            gun.shootAnimation = "oscillatorsynergy_fire";


            //gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_golddoublebarrelshotgun_shot_01";
            //gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
            //GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
            //GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 7);
            //GunExt.SetAnimationFPS(gun, gun.idleAnimation, 5);
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);

			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);

			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(89) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2.9f;
			gun.DefaultModule.cooldownTime = .2f;
			gun.DefaultModule.numberOfShotsInClip = 60;
			gun.SetBaseMaxAmmo(600);
			gun.quality = PickupObject.ItemQuality.EXCLUDED;
			gun.DefaultModule.angleVariance = 8f;
			gun.DefaultModule.burstShotCount = 4;
			gun.DefaultModule.burstCooldownTime = 0.02f;


			gun.Volley.projectiles[1].ammoCost = 1;
			gun.Volley.projectiles[1].shootStyle = ProjectileModule.ShootStyle.Burst;
			gun.Volley.projectiles[1].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.Volley.projectiles[1].cooldownTime = 0.3f;
			gun.Volley.projectiles[1].angleVariance = 9f;
			gun.Volley.projectiles[1].numberOfShotsInClip = 60;
			gun.Volley.projectiles[1].angleFromAim = 14;
			gun.Volley.projectiles[1].burstCooldownTime = 0.025f;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[0].projectiles[0]);
			projectile.gameObject.SetActive(false);
			gun.Volley.projectiles[0].projectiles[0] = projectile;
			projectile.baseData.damage = 3f;
			projectile.baseData.speed *= 0.8f;
			projectile.gameObject.AddComponent<OscillatorProjectile>();
			PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
			spook.penetration = 1;


            int Length = 7;
            Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(projectile, "OscillatorProjectileSynergy", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "OscillatorProjectile",
             AnimateBullet.ConstructListOfSameValues<IntVector2>(new IntVector2(7, 5), Length),
            AnimateBullet.ConstructListOfSameValues(true, Length),
            AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, Length),
            AnimateBullet.ConstructListOfSameValues(true, Length),
            AnimateBullet.ConstructListOfSameValues(false, Length),
            AnimateBullet.ConstructListOfSameValues<Vector3?>(null, Length),
            AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(9, 7), Length),
            AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(-1, -1), Length),
            AnimateBullet.ConstructListOfSameValues<Projectile>(null, Length));

            /*
			projectile.AnimateProjectile(new List<string> {
				"oscillatosynergy_projectile_001",
				"oscillatosynergy_projectile_002",
				"oscillatosynergy_projectile_003",
				"oscillatosynergy_projectile_004",
				"oscillatosynergy_projectile_005",
				"oscillatosynergy_projectile_006",
				"oscillatosynergy_projectile_007"
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
			*/
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			bool flag = gun.Volley.projectiles[0] != gun.DefaultModule;
			if (flag)
			{
				gun.Volley.projectiles[0].ammoCost = 0;
			}
			projectile.transform.parent = gun.barrelOffset;





			gun.Volley.projectiles[1].ammoCost = 1;
			gun.Volley.projectiles[1].shootStyle = ProjectileModule.ShootStyle.Burst;
			gun.Volley.projectiles[1].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.Volley.projectiles[1].cooldownTime = 0.2f;
			gun.Volley.projectiles[1].angleVariance = 9f;
			gun.Volley.projectiles[1].numberOfShotsInClip = 60;
			gun.Volley.projectiles[1].angleFromAim = -14;
			gun.Volley.projectiles[1].burstCooldownTime = 0.025f;

			Projectile projectile1 = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[1].projectiles[0]);
			projectile1.gameObject.SetActive(false);
			gun.Volley.projectiles[1].projectiles[0] = projectile1;
			projectile1.baseData.damage = 3f;
			projectile1.baseData.speed *= 0.8f;
			projectile1.gameObject.AddComponent<OscillatorProjectile>();
			PierceProjModifier spookY = projectile1.gameObject.AddComponent<PierceProjModifier>();
			spookY.penetration = 1;

			projectile1.AnimateProjectile(new List<string> {
				"oscillatosynergy_projectile_001",
				"oscillatosynergy_projectile_002",
				"oscillatosynergy_projectile_003",
				"oscillatosynergy_projectile_004",
				"oscillatosynergy_projectile_005",
				"oscillatosynergy_projectile_006",
				"oscillatosynergy_projectile_007"
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

			FakePrefab.MarkAsFakePrefab(projectile1.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile1);

			bool EE = gun.Volley.projectiles[1] != gun.DefaultModule;
			if (EE)
			{
				gun.Volley.projectiles[1].ammoCost = 0;
			}
			projectile.transform.parent = gun.barrelOffset;
			gun.encounterTrackable.EncounterGuid = "https://www.youtube.com/watch?v=8KmiS1VyUgA";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			OscillatoSynergyForme.AeID = gun.PickupObjectId;
		}
		public static int AeID;
	}
}