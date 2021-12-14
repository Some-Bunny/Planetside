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
	public class Mop : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Mop", "mopgun");
			Game.Items.Rename("outdated_gun_mods:mop", "psog:mop");
			gun.gameObject.AddComponent<Mop>();
			gun.SetShortDescription("Honest Work");
			gun.SetLongDescription("A mop thats been left inside of a chest. Surely the goops found around the Gungeon could be mopped up to some benefit?");
			gun.SetupSprite(null, "mopgun_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 10);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 2);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(33) as Gun).gunSwitchGroup;

			for (int i = 0; i < 3; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(33) as Gun, true, false);

			}

			gun.Volley.projectiles[0].ammoCost = 1;
			gun.Volley.projectiles[0].shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.Volley.projectiles[0].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.Volley.projectiles[0].cooldownTime = 0.5f;
			gun.Volley.projectiles[0].angleVariance = 10f;
			gun.Volley.projectiles[0].numberOfShotsInClip = -1;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[0].projectiles[0]);
			projectile.gameObject.SetActive(false);
			gun.Volley.projectiles[0].projectiles[0] = projectile;
			projectile.baseData.damage = 6f;
			projectile.AdditionalScaleMultiplier *= 1.5f;
			projectile.baseData.speed *= 1.25f;
			projectile.gameObject.AddComponent<MopProjectile>();
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
			gun.Volley.projectiles[1].shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.Volley.projectiles[1].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.Volley.projectiles[1].cooldownTime = 0.5f;
			gun.Volley.projectiles[1].angleVariance = 10f;
			gun.Volley.projectiles[1].numberOfShotsInClip = -1;
			gun.gunClass = GunClass.SILLY;

			Projectile projectile1 = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[1].projectiles[0]);
			projectile1.gameObject.SetActive(false);
			gun.Volley.projectiles[1].projectiles[0] = projectile1;
			projectile1.gameObject.AddComponent<MopProjectile>();

			projectile1.baseData.damage = 3f;
			FakePrefab.MarkAsFakePrefab(projectile1.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile1);
			bool aa = gun.Volley.projectiles[1] != gun.DefaultModule;
			if (aa)
			{
				gun.Volley.projectiles[1].ammoCost = 0;
			}
			projectile1.transform.parent = gun.barrelOffset;

			gun.Volley.projectiles[2].ammoCost = 1;
			gun.Volley.projectiles[2].shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.Volley.projectiles[2].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.Volley.projectiles[2].cooldownTime = 0.5f;
			gun.Volley.projectiles[2].angleVariance = 10f;
			gun.Volley.projectiles[2].numberOfShotsInClip = -1;

			Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[2].projectiles[0]);
			projectile1.gameObject.SetActive(false);
			gun.Volley.projectiles[2].projectiles[0] = projectile1;

			projectile2.baseData.damage = 3f;
			projectile2.gameObject.AddComponent<MopProjectile>();
			FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile2);
			bool ee = gun.Volley.projectiles[2] != gun.DefaultModule;
			if (ee)
			{
				gun.Volley.projectiles[2].ammoCost = 0;
			}
			projectile1.transform.parent = gun.barrelOffset;


			gun.barrelOffset.transform.localPosition = new Vector3(0.5f, 0.5f, 0f);
			gun.reloadTime = 2.5f;
			gun.SetBaseMaxAmmo(25);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(417) as Gun).muzzleFlashEffects;
			gun.CanReloadNoMatterAmmo = true;
			//gun.GoopReloadsFree = true;


			gun.quality = PickupObject.ItemQuality.D;
			gun.encounterTrackable.EncounterGuid = "mop";
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			Mop.MopID = gun.PickupObjectId;

			gun.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);


			ItemIDs.AddToList(gun.PickupObjectId);

			tk2dSpriteAnimationClip fireClip = gun.sprite.spriteAnimator.GetClipByName(gun.shootAnimation);
			float[] offsetsX = new float[] { 1.25f, -1f, -0.875f, -0.6875f, -0.375f, -0.125f,  };
			float[] offsetsY = new float[] { 0.5f, -1.25f, -1f, -0.75f, -0.5f, -0.25f };
			for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < fireClip.frames.Length; i++)
			{
				int id = fireClip.frames[i].spriteId;
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY[i];
			}

			tk2dSpriteAnimationClip fireClip2 = gun.sprite.spriteAnimator.GetClipByName(gun.reloadAnimation);
			float[] offsetsX2 = new float[] { 0.25f , 0.375f, 0.875f, 1.25f, 1.25f, 1.25f, 1.25f, 1.25f, 1.25f, 1.25f, 1.25f, 1.25f, 1.25f, 1.25f, 1, 0.625f, 0.25f};
			float[] offsetsY2 = new float[] { 0f , -0.5f, -0.75f, -0.875f, -0.875f, -0.875f, -0.875f, -0.875f, -0.875f, -0.875f, -0.875f, -0.875f, -0.875f, -0.875f, -0.6825f, -0.4375f, 0};
			for (int i = 0; i < offsetsX2.Length && i < offsetsY2.Length && i < fireClip2.frames.Length; i++)
			{
				int id = fireClip2.frames[i].spriteId;
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY2[i];
			}
		}
		public static int MopID;
		public override void OnPostFired(PlayerController player, Gun bruhgun)
		{
			gun.PreventNormalFireAudio = true;
		}
		private bool HasReloaded;

		protected override void Update()
		{
			base.Update();
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
		protected override void OnPickup(PlayerController player)
		{
			IsBlob = false;
			IsCharm = false;
			IsCheese = false;
			Isfire = false;
			IsgreenFire = false;
			IsOil = false;
			IsPoison = false;
			IsWater = false;
			IsWeb = false;
			IsBlood = false;
			IsPoop = false;
			IsPossessive = false;
			IsFrail = false;
			base.OnPickup(player);
		}

		protected override void OnPostDrop(PlayerController player)
		{
			base.OnPostDrop(player);
		}

		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			
			if (gun.IsReloading && this.HasReloaded)
			{
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
			this.goop = player.CurrentGoop;
			IsBlob = false;
			IsCharm = false;
			IsCheese = false;
			Isfire = false;
			IsgreenFire = false;
			IsOil = false;
			IsPoison = false;
			IsWater = false;
			IsWeb = false;
			IsPossessive = false;
			IsPoop = false;
			IsFrail = false;
			IsBlood = false;

			if (this.goop != null)
            {
				gun.GainAmmo(Mathf.Max(0, gun.ClipCapacity - gun.ClipShotsRemaining));
				bool fire = this.goop == EasyGoopDefinitions.FireDef | this.goop == EasyGoopDefinitions.FireDef2 | this.goop.name == "HelicopterNapalmGoop" | this.goop.name == "Napalm Goop" | this.goop.name == "NapalmGoopShortLife" | this.goop.name == "BulletKingWineGoop" | this.goop.name == "DevilGoop" | this.goop.name == "FlameLineGoop" | this.goop.name == "DemonWallGoop" | this.goop.name == "DemonWallGoop"; 
				if (fire)
				{
					Isfire = true;
				}
				bool greenfire = this.goop == EasyGoopDefinitions.GreenFireDef | this.goop.name == "GreenNapalmGoopThatWorks";
				if (greenfire)
				{
					IsgreenFire = true;
				}
				bool blob = this.goop == EasyGoopDefinitions.BlobulonGoopDef | this.goop.name == "BlobulordGoop";
				if (blob)
				{
					IsBlob = true;
				}
				bool OIL = this.goop == EasyGoopDefinitions.OilDef | this.goop.name == "Oil Goop";
				if (OIL)
				{
					IsOil = true;
				}
				bool cheese = this.goop == EasyGoopDefinitions.CheeseDef;
				if (cheese)
				{
					IsCheese = true;
				}
				bool water = this.goop == EasyGoopDefinitions.WaterGoop | this.goop.name == "MimicSpitGoop";
				if (water)
				{
					IsWater = true;
				}
				bool charm = this.goop == EasyGoopDefinitions.CharmGoopDef;	
				if (charm)
				{
					IsCharm = true;
				}
				bool posion = this.goop == EasyGoopDefinitions.PoisonDef | this.goop.name == "ResourcefulRatPoisonGoop" | this.goop.name == "MeduziPoisonGoop";
				if (posion)
				{
					IsPoison = true;
				}
				bool web = this.goop == EasyGoopDefinitions.WebGoop;
				if (web)
				{
					IsWeb = true;
				}
				bool blood = this.goop.name == "PermanentBloodGoop" | this.goop.name == "BloodGoop" | this.goop.name == "BloodbulonGoop";
				if (blood)
				{
					IsBlood = true;
				}
				bool poop = this.goop.name == "PoopulonGoop";
				if (poop)
				{
					IsPoop = true;
				}
				bool possession = this.goop == DebuffLibrary.PossesedPuddle;
				if (possession)
				{

					IsPossessive = true;
				}
				bool frail = this.goop == DebuffLibrary.FrailPuddle;
				if (frail)
				{
					IsFrail = true;
				}
			}
			
			/*
			ETGModConsole.Log("================================================");
			ETGModConsole.Log("Blob:" + IsBlob.ToString());
			ETGModConsole.Log("Charm:" + IsCharm.ToString());
			ETGModConsole.Log("Cheese:" + IsCheese.ToString());
			ETGModConsole.Log("Fire:" + Isfire.ToString());
			ETGModConsole.Log("Green Fire:" + IsgreenFire.ToString());
			ETGModConsole.Log("Oil:" + IsOil.ToString());
			ETGModConsole.Log("Poison:" + IsPoison.ToString());
			ETGModConsole.Log("Water:" + IsWater.ToString());
			ETGModConsole.Log("Web:" + IsWeb.ToString());
			ETGModConsole.Log("================================================");
			*/
			DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(player.CenterPosition, 2f);
		}
		public static bool Isfire;
		public static bool IsOil;
		public static bool IsPoison;
		public static bool IsWeb;
		public static bool IsWater;
		public static bool IsCharm;
		public static bool IsgreenFire;
		public static bool IsCheese;
		public static bool IsBlob;
		public static bool IsBlood;
		public static bool IsPoop;
		public static bool IsPossessive;
		public static bool IsFrail;
		GoopDefinition goop;
	}
}