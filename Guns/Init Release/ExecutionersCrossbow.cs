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
using Alexandria.Assetbundle;

namespace Planetside
{
	public class ExecutionersCrossbow : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Executioners Crossbow", "executionerscrossbow");
			Game.Items.Rename("outdated_gun_mods:executioners_crossbow", "psog:executioners_crossbow");
			var behav = gun.gameObject.AddComponent<ExecutionersCrossbow>();
			GunExt.SetShortDescription(gun, "Hang In There!");
			GunExt.SetLongDescription(gun, "Charging loads a spectral arrow into the slot that marks enemies to be executed.\nOriginally a hunting crossbow made by a desparate team, the remains of the Gundead used in the weapon seek retaliation agasint their killers.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "executionerscrossbow_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.idleAnimation = "executionercrossbow_idle";
            gun.shootAnimation = "executionercrossbow_fire";
            gun.reloadAnimation = "executionercrossbow_reload";
            gun.chargeAnimation = "executionercrossbow_charge";


            //GunExt.SetupSprite(gun, null, "executionerscrossbow_idle_001", 8);
            //GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            //GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 3);
            //GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 3);
            //GunExt.SetAnimationFPS(gun, gun.idleAnimation, 3);

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(8) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(12) as Gun).gunSwitchGroup;

			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2f;
			gun.DefaultModule.cooldownTime = 0.2f;
			gun.DefaultModule.numberOfShotsInClip = 4;
			gun.SetBaseMaxAmmo(100);
			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.ARROW;

			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[1].eventAudio = "SND_WPN_crossbow_shot_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[1].triggerEvent = true;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_WPN_crossbow_reload_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;

			gun.barrelOffset.transform.localPosition = new Vector3(2.0f, 0.4375f, 0f);
			gun.carryPixelOffset += new IntVector2((int)4f, (int)0f);
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 2;
			gun.encounterTrackable.EncounterGuid = "Bow. Nothing funny, fuck you.";
			gun.gunClass = GunClass.CHARGE;

			Projectile normalDart = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            normalDart.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(normalDart.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(normalDart);
			gun.DefaultModule.projectiles[0] = normalDart;
            normalDart.baseData.damage = 30f;
            normalDart.baseData.speed = 40f;
            normalDart.baseData.force = 1f;
            normalDart.baseData.range = 100f;
            normalDart.AdditionalScaleMultiplier *= 1f;
            normalDart.HasDefaultTint = true;
            normalDart.shouldRotate = true;
            normalDart.SetProjectileSpriteRight("executionerscrossbowprojectile1", 17, 3, false, tk2dBaseSprite.Anchor.MiddleCenter, 17, 3);
            normalDart.baseData.UsesCustomAccelerationCurve = true;
            normalDart.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1, 0.25f, 0.75f);



            Projectile specialDart = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            specialDart.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(specialDart.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(specialDart);
			gun.DefaultModule.projectiles[0] = specialDart;
            specialDart.baseData.damage = 10f;
            specialDart.baseData.speed = 70f;
            specialDart.baseData.force = 3f;
            specialDart.baseData.range = 100f;
            specialDart.AdditionalScaleMultiplier *= 1.33f;
            specialDart.HasDefaultTint = true;
            specialDart.shouldRotate = true;
            specialDart.SetProjectileSpriteRight("executionerscrossbowprojectile2", 20, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 20, 5);
            specialDart.sprite.usesOverrideMaterial = true;

            specialDart.baseData.UsesCustomAccelerationCurve = true;
            specialDart.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1, 0.1f, 0.5f);

            HomingModifier HomingMod = specialDart.gameObject.GetOrAddComponent<HomingModifier>();
            HomingMod.AngularVelocity = 120;
            HomingMod.HomingRadius = 10;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = specialDart.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(208, 255, 223, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 100);
            specialDart.sprite.renderer.material = mat;
            specialDart.gameObject.AddComponent<ExecutionersCrossbowSpecial>();
			PierceProjModifier specialDartPierce = specialDart.gameObject.AddComponent<PierceProjModifier>();
            specialDartPierce.penetration = 30;
            specialDartPierce.penetratesBreakables = true;

            ImprovedAfterImage yes = specialDart.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 1f;
            yes.shadowTimeDelay = 0.02f;
            yes.dashColor = new Color(0f, 0.8f, 0.3f, 1.1f);



            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
			{
				Projectile = normalDart,
				ChargeTime = 0f,
				AmmoCost = 1,
                AdditionalWwiseEvent = "Play_WPN_crossbow_shot_01",
                UsedProperties = ProjectileModule.ChargeProjectileProperties.additionalWwiseEvent | ProjectileModule.ChargeProjectileProperties.ammo
            };
			ProjectileModule.ChargeProjectile item3 = new ProjectileModule.ChargeProjectile
			{
				Projectile = specialDart,
				ChargeTime = 1f,
				AmmoCost = 2,		
				AdditionalWwiseEvent = "Play_WPN_woodbow_shot_02",
				UsedProperties = ProjectileModule.ChargeProjectileProperties.additionalWwiseEvent | ProjectileModule.ChargeProjectileProperties.ammo
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
			{
				item2,
				item3
			};


			gun.quality = PickupObject.ItemQuality.B;
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			gun.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);

            ExecutionersCrossbow.ExecutionersCrossbowID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}

        public static int ExecutionersCrossbowID;
		private bool HasReloaded;


       

      
		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				HasReloaded = false;
				//AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				//AkSoundEngine.PostEvent("Play_WPN_duelingpistol_reload_01", gameObject);
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
		}

		public override void Update()
		{
			if (gun.CurrentOwner)
			{
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}
			}
		}
	}
}
