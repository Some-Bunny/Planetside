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
			float duration = 2.5f;
			AkSoundEngine.PostEvent("Play_ENM_blobulord_charge_01", base.gameObject);
			while (elapsed < duration)
			{
				if (objectToLookOutFor.gameObject == null) { break; }
				elapsed += BraveTime.DeltaTime;
				float t = elapsed / duration;
				float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
				objectToLookOutFor.transform.localScale = Vector3.Lerp(currentscale, currentscale * 2f, throne1);
				yield return null;
			}
			ExplosionData data = StaticExplosionDatas.genericSmallExplosion;
			data.effect = (PickupObjectDatabase.GetById(368) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
			data.damage = 5.5f * (player != null ? player.stats.GetStatValue(PlayerStats.StatType.Damage) : 1);
			data.damageRadius = 3;
			data.doScreenShake = false;
			data.playDefaultSFX = false;
			data.force = 1;
			Exploder.Explode(objectToLookOutFor.transform.position, data, objectToLookOutFor.transform.position);
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
			GunExt.SetLongDescription(gun, "A large, amalgam of living flesh.\n\nIt seems to replicate very, *very* quickly...");
			GunExt.SetupSprite(gun, null, "heartthing_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 25);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 10);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 5);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(83) as Gun, true, false);
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2f;
			gun.DefaultModule.cooldownTime = .5f;
			gun.DefaultModule.numberOfShotsInClip = 111;
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
			projectile.baseData.damage = 1f;
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
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.CONTRAIL);

			gun.barrelOffset.transform.localPosition = new Vector3(1f, 0.5f, 0f);
			gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.BEAT_LOOP_1, true);

			gun.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("ArmWarmer", "Planetside/Resources/GunClips/ArmWarmer/flesfull", "Planetside/Resources/GunClips/ArmWarmer/flesempty");

			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(404) as Gun).gunSwitchGroup;
			gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };

			ArmWarmer.ArmWarmerID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int ArmWarmerID;


		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
            {
				AkSoundEngine.PostEvent("Play_BOSS_doormimic_vomit_01", base.gameObject);
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

		private void SpawnProjectile(float damage)
		{
			PlayerController player = this.gun.CurrentOwner as PlayerController;
			Vector2 position = new Vector2(player.CurrentGun.barrelOffset.position.x, player.CurrentGun.barrelOffset.position.y);
			GameObject gameObject = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(207) as Gun).DefaultModule.projectiles[0].gameObject, position, Quaternion.Euler(0f, 0f, ((player.CurrentGun == null) ? 1.2f : player.CurrentGun.CurrentAngle)), true);
			Projectile eatfarts = gameObject.GetComponent<Projectile>();
			bool flag12 = eatfarts != null;
			bool flag2 = flag12;
			if (flag2)
			{

				eatfarts.SpawnedFromOtherPlayerProjectile = true;
				eatfarts.Shooter = player.specRigidbody;
				eatfarts.Owner = player;
				//  eatfarts.Shooter = playerController1.specRigidbody;
				eatfarts.baseData.damage = (7.5f* damage)+7.5f;
				eatfarts.AdditionalScaleMultiplier = 0.66f;
				eatfarts.baseData.range = 15f;
				eatfarts.AdditionalScaleMultiplier *= 0.75f + (damage/25);
				eatfarts.SetOwnerSafe(player, "Player");
				eatfarts.ignoreDamageCaps = true;
				eatfarts.PenetratesInternalWalls = true;

			}
		}

		public override void PostProcessProjectile(Projectile projectile)
		{

		}
		private IEnumerator HandleBulletDeletionFrames(Vector3 centerPosition, float bulletDeletionSqrRadius, float duration)
		{
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
				for (int i = allProjectiles.Count - 1; i >= 0; i--)
				{
					Projectile projectile = allProjectiles[i];
					if (projectile)
					{
						if (!(projectile.Owner is PlayerController))
						{
							Vector2 vector = (projectile.transform.position - centerPosition).XY();
							if (projectile.CanBeKilledByExplosions && vector.sqrMagnitude < bulletDeletionSqrRadius)
							{
								GameManager.Instance.Dungeon.StartCoroutine(this.HandleBulletSucc(projectile));
								projectile.DieInAir();
							}
						}
					}
				}
				yield return null;
			}
			yield break;

		}
		private IEnumerator HandleBulletSucc(Projectile target)
		{
			EatenBullets += 1;
			PlayerController player = this.gun.CurrentOwner as PlayerController;
			Transform copySprite = this.CreateEmptySprite(target);
			Vector3 startPosition = copySprite.transform.position;
			float elapsed = 0f;
			float duration = 0.4f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				bool flag3 = player.CurrentGun && copySprite;
				if (flag3)
				{
					Vector3 position = player.CurrentGun.PrimaryHandAttachPoint.position;
					float t = elapsed / duration * (elapsed / duration);
					copySprite.position = Vector3.Lerp(startPosition, position, t);
					copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
					copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
					position = default(Vector3);
				}
				yield return null;
			}
			bool flag4 = copySprite;
			if (flag4)
			{
				UnityEngine.Object.Destroy(copySprite.gameObject);
			}
			yield break;
		}
		private Transform CreateEmptySprite(Projectile target)
		{
			GameObject gameObject = new GameObject("suck image");
			gameObject.layer = target.gameObject.layer;
			tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
			gameObject.transform.parent = SpawnManager.Instance.VFX;
			tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
			tk2dSprite.transform.position = target.sprite.transform.position;
			GameObject gameObject2 = new GameObject("image parent");
			gameObject2.transform.position = tk2dSprite.WorldCenter;
			tk2dSprite.transform.parent = gameObject2.transform;

			return gameObject2.transform;
		}
		public AIActor AIActor;
		public PlayerController player;


		public void ApplyActionToNearbyBullets(Vector2 position, float radius, Action<Projectile, float> lambda)
		{
			float num = radius * radius;
			if (this.activeBullets != null)
			{
				for (int i = 0; i < this.activeBullets.Count; i++)
				{
					if (this.activeBullets[i])
					{
						bool flag = radius < 0f;
						Vector2 vector = this.activeBullets[i].sprite.WorldCenter - position;
						if (!flag)
						{
							flag = (vector.sqrMagnitude < num);
						}
						if (flag)
						{
							lambda(this.activeBullets[i], vector.magnitude);
						}
					}
				}
			}
		}
		public List<Projectile> activeBullets;
		public int EatenBullets = 0;
	}
}