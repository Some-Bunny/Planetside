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
	public class Whistler : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Whistler", "whistler");
			Game.Items.Rename("outdated_gun_mods:whistler", "psog:whistler");
			gun.gameObject.AddComponent<Whistler>();
			GunExt.SetShortDescription(gun, "Can You Hear It?");
			GunExt.SetLongDescription(gun, "A gun invented by someone *particularly* lazy, with the desire to never meet their enemy face on. It got its legendary name due to its unique bullet structure, causing the rounds to seemingly whistle.");
            GunExt.SetupSprite(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "whistler_idle_001", 11);
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.idleAnimation = "whistler_idle";
            gun.shootAnimation = "whistler_fire";
            gun.reloadAnimation = "whistler_reload";
			gun.chargeAnimation = "whistler_charge";

            //GunExt.SetupSprite(gun, null, "whistler_idle_001", 8);



            //GunExt.SetAnimationFPS(gun, gun.shootAnimation, 24);
            //GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 6);
            //GunExt.SetAnimationFPS(gun, gun.idleAnimation, 1);
            //GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 3);

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);
			gun.SetBaseMaxAmmo(45);
			
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.reloadTime = 3.4f;
			gun.DefaultModule.cooldownTime = 0.1f;
			gun.DefaultModule.numberOfShotsInClip = 1;
			gun.DefaultModule.angleVariance = 14f;
			gun.barrelOffset.transform.localPosition = new Vector3(1f, 0.3125f, 0f);
			gun.quality = PickupObject.ItemQuality.C;
			gun.encounterTrackable.EncounterGuid = "SWEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE";
			gun.CanBeDropped = true;
			Gun gun4 = PickupObjectDatabase.GetById(334) as Gun;
			gun.muzzleFlashEffects = gun4.muzzleFlashEffects;
			gun.gunClass = GunClass.SILLY;
			Gun cricket = PickupObjectDatabase.GetById(180) as Gun;
			gun.gunSwitchGroup = cricket.gunSwitchGroup;

			//gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			//gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 3;
	
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]); projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.transform.parent = gun.barrelOffset;
			projectile.SetProjectileSpriteRight("whistler_projectile_001", 8, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 10, 7);

			ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
			yes.spawnShadows = true;
			yes.shadowLifetime = 0.7f;
			yes.shadowTimeDelay = 0.01f;
			yes.dashColor = new Color(1.2f, 0f, 2f, 1f);

			projectile.AdditionalScaleMultiplier *= 1;
			projectile.baseData.damage = 3.5f;
			projectile.baseData.speed = 30;
			projectile.pierceMinorBreakables = true;
			projectile.hitEffects.alwaysUseMidair = true;
			projectile.hitEffects.overrideMidairDeathVFX = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
			projectile.baseData.range = 1000000;

			projectile.objectImpactEventName = (PickupObjectDatabase.GetById(198) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
			projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(198) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

			PierceProjModifier spook = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
			spook.penetration = 10000;
			spook.penetratesBreakables = true;

			MaintainDamageOnPierce noDamageLoss = projectile.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
			noDamageLoss.damageMultOnPierce = 1.05f;

			HomingModifier HomingMod = projectile.gameObject.GetOrAddComponent<HomingModifier>();
			HomingMod.AngularVelocity = 210;
			HomingMod.HomingRadius = 20;

			BounceProjModifier BounceProjMod = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
			BounceProjMod.bouncesTrackEnemies = false;
			BounceProjMod.numberOfBounces = 1000;

			projectile.gameObject.GetOrAddComponent<WhistlerProjectile>();

			ProjectileModule.ChargeProjectile whistler1 = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile,
				ChargeTime = 0.6f,
				AdditionalWwiseEvent = "Play_OBJ_pastkiller_charge_01",
			};
			gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
			{
				whistler1
			};


			gun.sprite.usesOverrideMaterial = true;

			Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.SetColor("_EmissiveColor", new Color32(153, 0, 255, 255));
			mat.SetFloat("_EmissiveColorPower", 1.55f);
			mat.SetFloat("_EmissivePower", 100);
			mat.SetFloat("_EmissiveThresholdSensitivity", 0.2f);
			MeshRenderer component = gun.GetComponent<MeshRenderer>();
			if (!component)
			{
				return;
			}
			Material[] sharedMaterials = component.sharedMaterials;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				if (sharedMaterials[i].shader == mat)
				{
					return;
				}
			}
			Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
			Material material = new Material(mat);
			material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
			sharedMaterials[sharedMaterials.Length - 1] = material;
			component.sharedMaterials = sharedMaterials;

			ETGMod.Databases.Items.Add(gun, false, "ANY");
			Whistler.WhistlerID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);			
		}
		public static int WhistlerID;
	}
}
