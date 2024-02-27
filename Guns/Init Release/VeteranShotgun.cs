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
	public class VeteranShotgun : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Veterans Shotgun", "veteranshotgun");
			Game.Items.Rename("outdated_gun_mods:veterans_shotgun", "psog:veterans_shotgun");
			gun.gameObject.AddComponent<VeteranShotgun>();
			gun.SetShortDescription("Old And Tested");
			gun.SetLongDescription("A shotgun that oddly resembles shotguns wielded by the Shotgun Kin around the Gungeon, or maybe it's just the bullets throwing you off.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "veteranshotgun_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.reloadAnimation = "veteransshotgun_reload";
            gun.idleAnimation = "veteransshotgun_idle";
            gun.shootAnimation = "veteransshotgun_fire";

            /*
			gun.SetupSprite(null, "veteranshotgun_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 2);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 2);
			*/

            int e = 0;
			for (int i = -2; e < 5; i++, e++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(35) as Gun, true, false);
				gun.Volley.projectiles[e].angleFromAim = 8 * i;

				gun.Volley.projectiles[e].ammoCost = 1;
				gun.Volley.projectiles[e].shootStyle = ProjectileModule.ShootStyle.Automatic;
				gun.Volley.projectiles[e].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				gun.Volley.projectiles[e].cooldownTime = 0.66f;
				gun.Volley.projectiles[e].angleVariance = 0f;
				gun.Volley.projectiles[e].numberOfShotsInClip = 4;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[0].projectiles[0]);
				projectile.gameObject.SetActive(false);
				gun.Volley.projectiles[e].projectiles[0] = projectile;
				projectile.baseData.damage = 7;
				projectile.AdditionalScaleMultiplier = 0.75f;
				projectile.baseData.speed = 12f;
				projectile.gameObject.AddComponent<VeteranShotgunProjectile>();

				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = gun.Volley.projectiles[e] != gun.DefaultModule;
				if (flag)
				{
					gun.Volley.projectiles[e].ammoCost = 0;
				}
			}

			gun.gunClass = GunClass.SHOTGUN;

			gun.barrelOffset.transform.localPosition += new Vector3(1.0f, 0.125f, 0f);
			gun.reloadTime = 2.6f;
			gun.SetBaseMaxAmmo(80);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(51) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.D;
			gun.encounterTrackable.EncounterGuid = "Its a shtung";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.IRONSHOT);
			List<string> mandatoryConsoleIDs1 = new List<string>
			{
				"psog:veterans_shotgun"
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"ammo_belt",
				"cluster_mine",
				"portable_turret",
				"cog_of_battle",
				"c4",
				"air_strike",
				"napalm_strike"
			};
			CustomSynergies.Add("Old War", mandatoryConsoleIDs1, optionalConsoleIDs, false);
			VeteranShotgun.VeteranID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int VeteranID;
		public override void PostProcessProjectile(Projectile projectile)
		{
			if (projectile.specRigidbody && projectile.sprite)
			{
				tk2dSpriteDefinition currentSpriteDef = projectile.sprite.GetCurrentSpriteDef();
				Bounds bounds = currentSpriteDef.GetBounds();
				float num = Mathf.Max(bounds.size.x, bounds.size.y);
				if (num < 0.5f)
				{
					float num2 = 0.5f / num;
					//UnityEngine.Debug.Log(num + "|" + num2);
					projectile.sprite.scale = new Vector3(num2, num2, num2);
					if (num2 != 1f && projectile.specRigidbody != null)
					{
						projectile.specRigidbody.UpdateCollidersOnScale = true;
						projectile.specRigidbody.ForceRegenerate(null, null);
					}
				}
			}
			if (projectile.sprite && projectile.sprite.renderer)
			{
				Material sharedMaterial = projectile.sprite.renderer.sharedMaterial;
				projectile.sprite.usesOverrideMaterial = true;
				Material material = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
				material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
				material.SetColor("_OverrideColor", new Color(1f, 1f, 1f, 1f));
				this.LerpMaterialGlow(material, 0f, 22f, 0.4f);
				material.SetFloat("_EmissiveColorPower", 8f);
				material.SetColor("_EmissiveColor", Color.red);
				SpriteOutlineManager.AddOutlineToSprite(projectile.sprite, Color.red);
				projectile.sprite.renderer.material = material;
			}
		}
		public void LerpMaterialGlow(Material targetMaterial, float startGlow, float targetGlow, float duration)
		{
			base.StartCoroutine(this.LerpMaterialGlowCR(targetMaterial, startGlow, targetGlow, duration));
		}
		private IEnumerator LerpMaterialGlowCR(Material targetMaterial, float startGlow, float targetGlow, float duration)
		{
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime; ;
				float t = elapsed / duration;
				if (targetMaterial != null)
				{
					targetMaterial.SetFloat("_EmissivePower", Mathf.Lerp(startGlow, targetGlow, t));
				}
				yield return null;
			}
			yield break;
		}
		public override void OnPostFired(PlayerController player, Gun bruhgun)
		{
			gun.PreventNormalFireAudio = true;
			AkSoundEngine.PostEvent("Play_WPN_shotgun_shot_01", gameObject);

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
				AkSoundEngine.PostEvent("Play_WPN_shotgun_reload", gameObject);
			}
		}
	}

}