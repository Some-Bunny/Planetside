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
	public class Petrifier : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Petrifier", "petrifier");
			Game.Items.Rename("outdated_gun_mods:petrifier", "psog:petrifier");
			gun.gameObject.AddComponent<Petrifier>();
			gun.SetShortDescription("Fear Is The Mindkiller");
			gun.SetLongDescription("Crude, yet powerful. Fires bursts of fast bolts. A primitive form of the railgun, designed by the insane and patented by the irrational.");
            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "petrifier_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.reloadAnimation = "petrifier_reload";
            gun.idleAnimation = "petrifier_idle";
            gun.shootAnimation = "petrifier_fire";

            for (int i = 0; i < 4; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(88) as Gun, true, false);

			}
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


                projectile.AddTrailToProjectileBundle(StaticSpriteDefinitions.Beam_Sheet_Data, "petrifierstart_001",
				StaticSpriteDefinitions.Beam_Animation_Data,
                "petrifier_mid", new Vector2(7, 5), new Vector2(0, 1), false, "petrifier_start");


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


                int Length = 4;
                Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(projectile, "hotrod", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "hotrod",
                 AnimateBullet.ConstructListOfSameValues<IntVector2>(new IntVector2(12, 4), Length),
                AnimateBullet.ConstructListOfSameValues(true, Length),
                AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, Length),
                AnimateBullet.ConstructListOfSameValues(true, Length),
                AnimateBullet.ConstructListOfSameValues(false, Length),
                AnimateBullet.ConstructListOfSameValues<Vector3?>(null, Length),
                AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(12, 4), Length),
                AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(0, 0), Length),
                AnimateBullet.ConstructListOfSameValues<Projectile>(null, Length));


                projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 18f;
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
		private bool ReloadState;

		public void Start()
		{
			ReloadState = gun.IsReloading;
        }

		public override void Update()
		{
			if (gun.CurrentOwner)
			{
				if (ReloadState != gun.IsReloading)
				{
					ReloadState = gun.IsReloading;
					if (ReloadState == false)
					{
                        AddExtraFinalShots();
                    }
                }
			}
		}


        public override void OnPlayerPickup(PlayerController playerOwner)
        {
            base.OnPlayerPickup(playerOwner);
            playerOwner.inventory.OnGunChanged += GunSwapped;
        }
        public override void OnDropped()
        {
            if (this.PlayerOwner)
            {
                PlayerOwner.inventory.OnGunChanged -= GunSwapped;
            }
            base.OnDropped();
        }

        private void GunSwapped(Gun previous, Gun current, Gun previousSecondary, Gun currentSecondary, bool newGun)
        {
			if (current == this.gun)
			{
				this.Invoke("Delay", 0.05f);
            }
        }

		public void Delay()
		{
            foreach (ProjectileModule mod in this.gun.Volley.projectiles)
            {
                mod.numberOfShotsInClip = this.GetModNumberOfShotsInClipWithDefSize(this.gun.CurrentOwner as PlayerController, mod, 2) + ProcessedClipShots;
            }
        }




        public int ExtraClipBonus;
        public int ProcessedClipShots;

        public void AddExtraFinalShots()
        {
			if (this.gun.CurrentOwner as PlayerController == null) { return; }
			foreach (ProjectileModule mod in this.gun.Volley.projectiles)
            {
				mod.numberOfShotsInClip = this.GetModNumberOfShotsInClipWithDefSize(this.gun.CurrentOwner as PlayerController, mod, 2) + ExtraClipBonus;
			}
			ProcessedClipShots = ExtraClipBonus;
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


