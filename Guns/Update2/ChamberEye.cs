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

namespace Planetside
{
	public class EyeOfAnnihilation : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Eye OF Annihilation", "chambereye");
			Game.Items.Rename("outdated_gun_mods:eye_of_annihilation", "psog:eye_of_annihilation");
			gun.gameObject.AddComponent<EyeOfAnnihilation>();
			gun.SetShortDescription("ei bol");
			gun.SetLongDescription("Cool eye.");
			gun.SetupSprite(null, "chambereye_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 4);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 2);

			for (int i = 0; i < 1; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(43) as Gun, true, false);

			}
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = 0.0001f;
				projectileModule.angleVariance = 15f;
				projectileModule.numberOfShotsInClip = 8;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 7f;
				projectile.AdditionalScaleMultiplier = 0.9f;
				
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}
				projectile.transform.parent = gun.barrelOffset;
				projectile.objectImpactEventName = (PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
				projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;
				projectile.gameObject.AddComponent<ChamberEyeProjectile>();
				projectile.AnimateProjectile(new List<string> {
				"eyespear_projectile_001",
				"eyespear_projectile_002",
				"eyespear_projectile_003",
				"eyespear_projectile_004"
			}, 13, true, new List<IntVector2> {
				new IntVector2(20, 9), //1
                new IntVector2(20, 9), //2            All frames are 13x16 except select ones that are 11-14
                new IntVector2(20, 9), //3
                new IntVector2(20, 9),
			}, AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 7), AnimateBullet.ConstructListOfSameValues(true, 7), AnimateBullet.ConstructListOfSameValues(false, 7),
			AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 7));

				projectile.SetProjectileSpriteRight("eyespear_projectile_001", 19, 7, false, tk2dBaseSprite.Anchor.MiddleCenter, 19, 7);
				projectile.shouldRotate = true;

			}
			gun.barrelOffset.transform.localPosition = new Vector3(0.6875f, 0.4375f, 0f);
			gun.reloadTime = 0f;
			gun.SetBaseMaxAmmo(999);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(33) as Gun).muzzleFlashEffects;



			/*
			TrailRenderer tr;
			var tro = gun.gameObject.AddChild("trail object");
			tro.transform.position = gun.transform.position;
			tro.transform.localPosition = new Vector3(0.6875f, 0.4375f);

			tr = tro.AddComponent<TrailRenderer>();
			tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			tr.receiveShadows = false;
			var mat = new Material(Shader.Find("Sprites/Default"));
			mat.mainTexture = YES._gradTexture;
			tr.material = mat;
			tr.minVertexDistance = 0.1f;
			
			mat.SetColor("_Color", Color.white);
			tr.startColor = Color.white;
			tr.endColor = Color.red;
			
			tr.time = 1f;
			tr.startWidth = 0.3125f;
			tr.endWidth = 0;
			tr.enabled = true;
			*/

			/*
			Material mat1 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat1.mainTexture = gun.sprite.renderer.material.mainTexture;
			mat1.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
			mat1.SetFloat("_EmissiveColorPower", 1.55f);
			mat1.SetFloat("_EmissivePower", 100);
			mat1.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
			*/
			ImprovedAfterImage yes = gun.gameObject.AddComponent<ImprovedAfterImage>();
			yes.spawnShadows = true;
			yes.shadowLifetime = 0.5f;
			yes.shadowTimeDelay = 0.001f;
			yes.dashColor = Color.clear;
			yes.name = "Gun Trail";

			gun.quality = PickupObject.ItemQuality.EXCLUDED;
			gun.encounterTrackable.EncounterGuid = "eie";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			EyeOfAnnihilation.EyeOfAnnihilationID = gun.PickupObjectId;

		}
		public static int EyeOfAnnihilationID;
		public Texture _gradTexture;
		public override void OnPostFired(PlayerController player, Gun bruhgun)
		{
			gun.PreventNormalFireAudio = true;

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
				//AkSoundEngine.PostEvent("Play_WPN_pillow_reload_01", gameObject);
			}
		}
	}

}