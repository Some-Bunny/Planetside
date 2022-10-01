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
	public class Petrifier : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Petrifier", "petrifier");
			Game.Items.Rename("outdated_gun_mods:petrifier", "psog:petrifier");
			gun.gameObject.AddComponent<Petrifier>();
			gun.SetShortDescription("Fear Is The Mindkiller");
			gun.SetLongDescription("Crude, yet powerful. Fires bursts of fast bolts. A primitive form of the railgun, designed by the insane and patented by the irrational.");
			gun.SetupSprite(null, "petrifier_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 4);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 2);

			for (int i = 0; i < 4; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(88) as Gun, true, false);

			}

			//            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(35) as Gun, true, false);

			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = 0.3f;
				projectileModule.angleVariance = 19f;
				projectileModule.numberOfShotsInClip = 2;
				projectileModule.customAmmoType = (PickupObjectDatabase.GetById(81) as Gun).DefaultModule.customAmmoType;
				projectileModule.ammoType = (PickupObjectDatabase.GetById(81) as Gun).DefaultModule.ammoType;


				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

				projectile.baseData.speed = 240f;
				projectile.baseData.range = 1000;

				projectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(53) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
				projectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(53) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
				projectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(53) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
				projectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(53) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
				projectile.hitEffects.CenterDeathVFXOnProjectile = false;

				List<string> BeamAnimPaths = new List<string>()
				{
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifiermid_001",
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifiermid_002",
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifiermid_003",
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifiermid_004",
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifiermid_005",
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifiermid_006",
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifiermid_007",

				};
				List<string> ImpactAnimPaths = new List<string>()
				{
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifierend_001",
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifierend_002",
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifierend_003",
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifierend_004",
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifierend_005",
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifierend_006",
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifierend_007",
				};

				projectile.AddTrailToProjectile(
				"Planetside/Resources2/ProjectileTrails/Petrifier/petrifiermid_001",
				new Vector2(7, 3),
				new Vector2(0, 2),
				BeamAnimPaths, 20,
				ImpactAnimPaths, 20,
				0.1f,
				30,
				30,
				true
				);

				EmmisiveTrail emis = projectile.gameObject.AddComponent<EmmisiveTrail>();
				emis.EmissiveColorPower = 10;
				emis.EmissivePower = 100;

				PetrifierProjectile petrifier = projectile.gameObject.AddComponent<PetrifierProjectile>();

				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
				mat.SetFloat("_EmissiveColorPower", 1.55f);
				mat.SetFloat("_EmissivePower", 200);
				projectile.sprite.renderer.material = mat;
				projectile.shouldRotate = true;


				projectile.AnimateProjectile(new List<string> {
				"hotrod1",
				"hotrod2",
				"hotrod3",
				"hotrod4",
			}, 7, true, new List<IntVector2> {
				new IntVector2(12, 4),
				new IntVector2(12, 4),
				new IntVector2(12, 4),
				new IntVector2(12, 4)

			}, AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 7), AnimateBullet.ConstructListOfSameValues(true, 7), AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 7));

				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 17f;
				projectile.AdditionalScaleMultiplier = 1f;
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}
			}
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(156) as Gun).gunSwitchGroup;
			gun.barrelOffset.transform.localPosition = new Vector3(1.75f, 0.5f, 0f);
			gun.reloadTime = 1.7f;
			gun.SetBaseMaxAmmo(120);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects;
			gun.gunClass = GunClass.SHOTGUN;

			gun.quality = PickupObject.ItemQuality.B;
			gun.encounterTrackable.EncounterGuid = "Fear.";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.IRONSHOT);

			Petrifier.PetrifierID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);

			Petrifier.fleeData = new FleePlayerData();
			Petrifier.fleeData.StartDistance = 100f;
		}
		public static int ThunderShotID;

		public static FleePlayerData fleeData;

		public static int PetrifierID;
		private bool HasReloaded;

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
		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
				AddExtraFinalShots();
			}
		}

		public int ExtraClipBonus;

		public void AddExtraFinalShots()
        {
			if (this.gun.CurrentOwner as PlayerController == null) { return; }
			foreach (ProjectileModule mod in this.gun.Volley.projectiles)
            {
				mod.numberOfShotsInClip = this.GetModNumberOfShotsInClipWithDefSize(this.gun.CurrentOwner as PlayerController, mod, 2) + ExtraClipBonus;
			}
			ExtraClipBonus = 0;
		}

		public int GetModNumberOfShotsInClipWithDefSize(GameActor owner, ProjectileModule mod,int clipSizeDefault)
		{
			if (mod.numberOfShotsInClip == 1)
			{
				return mod.numberOfShotsInClip;
			}
			if (!(owner != null) || !(owner is PlayerController))
			{
				return mod.numberOfShotsInClip;
			}
			PlayerController playerController = owner as PlayerController;
			float statValue = playerController.stats.GetStatValue(PlayerStats.StatType.AdditionalClipCapacityMultiplier);
			float statValue2 = playerController.stats.GetStatValue(PlayerStats.StatType.TarnisherClipCapacityMultiplier);
			int num = Mathf.FloorToInt((float)clipSizeDefault * statValue * statValue2);
			if (num < 0)
			{
				return num;
			}
			return Mathf.Max(num, 1);
		}
	}
}


