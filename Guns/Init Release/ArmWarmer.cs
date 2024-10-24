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
using SaveAPI;


namespace Planetside
{
	public class StickyArmWarmerProjectile : MonoBehaviour
	{ 
		public void Start()
        {
			currentObject = this.GetComponent<Projectile>();
			if (currentObject) 
			{
				currentObject.OnHitEnemy += HandleHit;
			}
		}
		private void HandleHit(Projectile projectile, SpeculativeRigidbody otherBody, bool fatal)
		{
			if (otherBody.aiActor != null && !otherBody.healthHaver.IsDead && otherBody.aiActor.behaviorSpeculator && !otherBody.aiActor.IsHarmlessEnemy)
			{
				if (base.GetComponent<PierceProjModifier>() != null)
                {
					if (base.GetComponent<PierceProjModifier>().penetration == 0)
                    {TransformToSticky(projectile, otherBody);}
				}
				else
                {TransformToSticky(projectile, otherBody);}
			}
		}

		private void TransformToSticky(Projectile projectile, SpeculativeRigidbody otherBody)
        {
			projectile.DestroyMode = Projectile.ProjectileDestroyMode.DestroyComponent;
			objectToLookOutFor = projectile.gameObject;
			objectToLookOutFor.transform.parent = otherBody.transform;
			player = projectile.Owner as PlayerController;
			GameManager.Instance.StartCoroutine(this.EnlargeTumors());
		}
		private void OnPlayerReloaded(PlayerController arg1, Gun arg2, bool actual)
		{
			GameManager.Instance.StartCoroutine(this.EnlargeTumors());
		}

		private IEnumerator EnlargeTumors()
		{
			if (objectToLookOutFor == null) { yield break; }
			Vector3 currentscale = objectToLookOutFor.transform.localScale;
			float elapsed = 0f;
			float duration = 5f;
			AkSoundEngine.PostEvent("Play_ENM_blobulord_charge_01", base.gameObject);
			while (elapsed < duration)
			{
				if (objectToLookOutFor == null) { yield break; }
				elapsed += BraveTime.DeltaTime;
				float t = elapsed / duration;
				float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
				objectToLookOutFor.transform.localScale = Vector3.Lerp(currentscale, currentscale * 2f, throne1);
				yield return null;
			}
            if (objectToLookOutFor == null) { yield break; }
            ExplosionData data = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
            data.effect = (PickupObjectDatabase.GetById(368) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
            data.damage = 8f * (player != null ? player.stats.GetStatValue(PlayerStats.StatType.Damage) : 1);
			data.damageRadius = 3;
			data.doScreenShake = false;
			data.playDefaultSFX = false;
			data.force = 2.5f;
            Exploder.Explode(objectToLookOutFor.transform.position, data, Vector2.zero);
            AkSoundEngine.PostEvent("Play_BOSS_blobulord_burst_01", base.gameObject);
            Destroy(objectToLookOutFor);

            yield break;
		}
		public PlayerController player;
		public Projectile currentObject;
		public GameObject objectToLookOutFor;
		public Material materialToCopy;
		public tk2dSprite objectSprite;
		public AIActor parent;
		public Gun ToCheckReloadFor;
	}
}


namespace Planetside
{



    public class ArmWarmer : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Arm Warmer", "heartthing");
			Game.Items.Rename("outdated_gun_mods:arm_warmer", "psog:arm_warmer");
			gun.gameObject.AddComponent<ArmWarmer>();
			GunExt.SetShortDescription(gun, "Mmmm, Tasty...");
			GunExt.SetLongDescription(gun, "A large blob of living flesh of an unknown origin.\n\nSeems to replicate very, *very* quickly...");
			GunExt.SetupSprite(gun, null, "heartthing_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 25);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 10);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 5);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(83) as Gun, true, false);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_ENM_blobulord_bubble_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;

			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(479) as Gun).gunSwitchGroup;
            gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2f;
			gun.DefaultModule.cooldownTime = .5f;
			gun.DefaultModule.numberOfShotsInClip = 33;
			gun.SetBaseMaxAmmo(333);
			gun.quality = PickupObject.ItemQuality.D;
			gun.DefaultModule.angleVariance = 18f;
			gun.DefaultModule.burstShotCount = 3;
			gun.DefaultModule.burstCooldownTime = 0.05f;
			gun.Volley.projectiles[0].ammoCost = 1;
			gun.InfiniteAmmo = false;

            gun.gunClass = GunClass.SILLY;
			Gun goreThing = PickupObjectDatabase.GetById(43) as Gun;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(goreThing.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			projectile.baseData.damage = 1.5f;
			projectile.baseData.speed *= 0.7f;
			projectile.AdditionalScaleMultiplier = 1f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.baseData.range = 1000f;
			BounceProjModifier bouncy = projectile.gameObject.AddComponent<BounceProjModifier>();
			bouncy.numberOfBounces = 1;
			projectile.AnimateProjectile(new List<string> {
				"meatorb_001",
				"meatorb_002",
				"meatorb_003",
				"meatorb_004",
				"meatorb_005",
			}, 2, true, new List<IntVector2> {
				new IntVector2(8, 8),
				new IntVector2(8, 8),
				new IntVector2(8, 8),
				new IntVector2(8, 8),
				new IntVector2(8, 8),
			}, AnimateBullet.ConstructListOfSameValues(false, 8), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 8), AnimateBullet.ConstructListOfSameValues(true, 8), AnimateBullet.ConstructListOfSameValues(false, 8), AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 8));
			projectile.hitEffects.alwaysUseMidair = true;
			projectile.hitEffects.overrideMidairDeathVFX = (PickupObjectDatabase.GetById(368) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
			projectile.gameObject.AddComponent<StickyArmWarmerProjectile>();
			projectile.objectImpactEventName = (PickupObjectDatabase.GetById(404) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
			projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(404) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;
			gun.DefaultModule.projectiles[0] = projectile;

			Gun gun4 = PickupObjectDatabase.GetById(83) as Gun;
			gun.muzzleFlashEffects = gun4.muzzleFlashEffects;

			gun.encounterTrackable.EncounterGuid = "OM NOMNOMNOMNONMNONMONMNONMNONMONMNONMNO";

			ETGMod.Databases.Items.Add(gun, false, "ANY");

			SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.CONTRAIL);

			gun.barrelOffset.transform.localPosition = new Vector3(1f, 0.5f, 0f);
			gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.BEAT_LOOP_1, true);

			gun.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("ArmWarmer", "Planetside/Resources/GunClips/ArmWarmer/flesfull", "Planetside/Resources/GunClips/ArmWarmer/flesempty");

			gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };

			ArmWarmer.ArmWarmerID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int ArmWarmerID;


		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
            {
				AkSoundEngine.PostEvent("Play_ENM_blobulord_reform_01", base.gameObject);
				this.HasReloaded = false;
			}
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
		private bool HasReloaded;
	}
}