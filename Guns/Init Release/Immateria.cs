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
	public class Immateria : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Immateria", "immateria");
			Game.Items.Rename("outdated_gun_mods:immateria", "psog:immateria");
			var behav = gun.gameObject.AddComponent<Immateria>();
			GunExt.SetShortDescription(gun, "The Void");
			GunExt.SetLongDescription(gun, "A cannon that fires unstable rifts that collapse on themselves. In an attempt to save their dying civilization, an alien race used these to go back in time to amend their mistakes.\n\nThe fact these weapons can be found clues at their fate.");
			GunExt.SetupSprite(gun, null, "immateria_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 12);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 4);
			GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 5);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 3);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2f;
			gun.DefaultModule.cooldownTime = 1f;
			gun.DefaultModule.numberOfShotsInClip = 1;
			gun.SetBaseMaxAmmo(8);
			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BLASTER;

			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_BOSS_doormimic_blast_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_WPN_blackhole_impact_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;


			gun.barrelOffset.transform.localPosition = new Vector3(2.0f, 0.4375f, 0f);
			gun.carryPixelOffset += new IntVector2((int)4f, (int)0f);
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 2;
			gun.encounterTrackable.EncounterGuid = "I wanna set the Universe On Fire.";
			gun.gunClass = GunClass.CHARGE;

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Immateria", "Planetside/Resources/GunClips/Immateria/immateriafull", "Planetside/Resources/GunClips/Immateria/immateriaempty");

			Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
			projectile2.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile2);
			gun.DefaultModule.projectiles[0] = projectile2;
			projectile2.baseData.damage = 20f;
			projectile2.baseData.speed = 15f;
			projectile2.baseData.force *= 1f;
			projectile2.baseData.range = 100f;
			projectile2.AdditionalScaleMultiplier *= 1f;
			projectile2.transform.parent = gun.barrelOffset;
			projectile2.HasDefaultTint = true;
			projectile2.shouldRotate = false;
			projectile2.SetProjectileSpriteRight("immateria_projectile_001", 24, 24, false, tk2dBaseSprite.Anchor.MiddleCenter, 22, 22);
			projectile2.sprite.usesOverrideMaterial = true;
			ImmateriaProjectile yah = projectile2.gameObject.AddComponent<ImmateriaProjectile>();
			yah.Pulses = 1;
			PierceProjModifier spook = projectile2.gameObject.AddComponent<PierceProjModifier>();
			spook.penetration = 100;
			spook.penetratesBreakables = true;

			Projectile projectile3 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
			projectile3.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile3.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile3);
			gun.DefaultModule.projectiles[0] = projectile3;
			projectile3.baseData.damage = 30f;
			projectile3.baseData.speed = 15f;
			projectile3.baseData.force *= 1f;
			projectile3.baseData.range = 100f;
			projectile3.AdditionalScaleMultiplier *= 1.33f;
			projectile3.transform.parent = gun.barrelOffset;
			projectile3.HasDefaultTint = true;
			projectile3.shouldRotate = false;
			projectile3.SetProjectileSpriteRight("immateria_projectile_001", 24, 24, false, tk2dBaseSprite.Anchor.MiddleCenter, 22, 22);
			projectile3.sprite.usesOverrideMaterial = true;
			ImmateriaProjectile ee = projectile3.gameObject.AddComponent<ImmateriaProjectile>();
			ee.Pulses = 2;
			PierceProjModifier aaaa = projectile3.gameObject.AddComponent<PierceProjModifier>();
			aaaa.penetration = 100;
			aaaa.penetratesBreakables = true;

			Projectile projectile4 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
			projectile4.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile4.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile4);
			gun.DefaultModule.projectiles[0] = projectile4;
			projectile4.baseData.damage = 40f;
			projectile4.baseData.speed = 15f;
			projectile4.baseData.force *= 1f;
			projectile4.baseData.range = 100f;
			projectile4.AdditionalScaleMultiplier *= 1.66f;
			projectile4.transform.parent = gun.barrelOffset;
			projectile4.HasDefaultTint = true;
			projectile4.shouldRotate = false;
			projectile4.SetProjectileSpriteRight("immateria_projectile_001", 24, 24, false, tk2dBaseSprite.Anchor.MiddleCenter, 22, 22);
			projectile4.sprite.usesOverrideMaterial = true;
			ImmateriaProjectile eea = projectile4.gameObject.AddComponent<ImmateriaProjectile>();
			eea.Pulses = 3;
			PierceProjModifier eaea = projectile4.gameObject.AddComponent<PierceProjModifier>();
			eaea.penetration = 100;
			eaea.penetratesBreakables = true;



			ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile2,
				ChargeTime = 1f,
				AdditionalWwiseEvent = "Play_OBJ_pastkiller_charge_01",
				//UsedProperties = ProjectileModule.ChargeProjectileProperties.additionalWwiseEvent

			};
			ProjectileModule.ChargeProjectile item3 = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile3,
				ChargeTime = 3f,
				AdditionalWwiseEvent = "Play_OBJ_pastkiller_charge_01",
				//UsedProperties = ProjectileModule.ChargeProjectileProperties.additionalWwiseEvent


			};
			ProjectileModule.ChargeProjectile item4 = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile4,
				ChargeTime = 5f,
				AdditionalWwiseEvent = "Play_OBJ_pastkiller_charge_01",
				//UsedProperties = ProjectileModule.ChargeProjectileProperties.additionalWwiseEvent
			};
			gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
			{
				item2,
				item3,
				item4
			};
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
			Immateria.ImmateriaID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int ImmateriaID;

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
				AkSoundEngine.PostEvent("Play_OBJ_metalskin_end_01", base.gameObject);
			}
		}

		public Texture _gradTexture;

		public override void PostProcessProjectile(Projectile projectile)
		{
			//base.StartCoroutine(this.Speed(projectile));
		}
		public IEnumerator Speed(Projectile projectile)
		{
			bool flag = this.gun.CurrentOwner != null;
			bool flag3 = flag;
			if (flag3)
			{
				float speed = projectile.baseData.speed / 15;
				for (int i = 0; i < 15; i++)
				{
					projectile.baseData.speed -= speed;
					projectile.UpdateSpeed();
					yield return new WaitForSeconds(0.05f);
				}
			}
			yield break;
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
