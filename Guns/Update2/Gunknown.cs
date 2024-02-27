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
using Alexandria.PrefabAPI;

namespace Planetside
{
	public class UnknownGun : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Gunknown", "gunknown");
			Game.Items.Rename("outdated_gun_mods:gunknown", "psog:gunknown");
			gun.gameObject.AddComponent<UnknownGun>();
			gun.SetShortDescription("Eternal Strength");
			gun.SetLongDescription("The Gun-Ternal Power offers infinite strength, as long as it is sated.\n\nWorshipped by many a thing, both living and not, residing in an ancient monolithic construction.");
			gun.SetupSprite(null, "gunknown_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 1000);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 7);

			for (int i = 0; i < 1; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(156) as Gun, true, false);

			}
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = 0.0001f;
				projectileModule.angleVariance = 0f;
				projectileModule.numberOfShotsInClip = 1;

				Gun lol = PickupObjectDatabase.GetById(180) as Gun;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(lol.DefaultModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 4.5f;
				projectile.AdditionalScaleMultiplier = 0.8f;
				projectile.baseData.range = 0.00001f;
				projectile.sprite.renderer.enabled = false;
				projectile.gameObject.AddComponent<GunknownProjectile>();

				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}
				projectile.transform.parent = gun.barrelOffset;				
			}
			gun.barrelOffset.transform.localPosition = new Vector3(0.6875f, 0.4375f, 0f);
			gun.reloadTime = 0f;
			gun.InfiniteAmmo = true;
			gun.SetBaseMaxAmmo(1000);
			gun.ActiveItemStyleRechargeAmount = 750;
			gun.UsesRechargeLikeActiveItem = true;
			Gun duellaser = PickupObjectDatabase.GetById(370) as Gun;
			gun.DefaultModule.customAmmoType = duellaser.DefaultModule.customAmmoType;
			gun.gunHandedness = GunHandedness.NoHanded;
			gun.preventRotation = true;
			gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
			gun.gunClass = GunClass.CHARGE;

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Gunknown", "Planetside/Resources/GunClips/Gunknown/gunknownclipfull", "Planetside/Resources/GunClips/Gunknown/gunknownclipemptyl");


			gun.quality = PickupObject.ItemQuality.A;
			gun.encounterTrackable.EncounterGuid = "I. AM. ETERNAL.";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
            //====================================================================================================================
            GameObject gameObject = PrefabBuilder.BuildObject("Gun-Ternal Strength");

            tk2dSprite sprite = gameObject.AddComponent<tk2dSprite>();
            sprite.collection = StaticSpriteDefinitions.Guon_Sheet_Data;
            sprite.SetSprite(StaticSpriteDefinitions.Guon_Sheet_Data.GetSpriteIdByName("superguon"));
            sprite.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;

            SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(12, 12));
			PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
			speculativeRigidbody.CollideWithTileMap = false;
			speculativeRigidbody.CollideWithOthers = true;
			speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
			orbitalPrefab.shouldRotate = false;
			orbitalPrefab.orbitRadius = 0.1f;
			orbitalPrefab.orbitDegreesPerSecond = 0;
			orbitalPrefab.perfectOrbitalFactor = 1000f;
			orbitalPrefab.SetOrbitalTier(0);


            ImprovedAfterImage fuck = orbitalPrefab.gameObject.AddComponent<ImprovedAfterImage>();
            fuck.spawnShadows = true;
            fuck.shadowLifetime = 0.5f;
            fuck.shadowTimeDelay = 0.1f;
            fuck.dashColor = new Color(1, 0.5f, 0);

            orbitalPrefab.sprite.usesOverrideMaterial = true;
            Material mater = orbitalPrefab.sprite.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mater.mainTexture = orbitalPrefab.sprite.renderer.material.mainTexture;
            mater.SetColor("_EmissiveColor", new Color32(255, 177, 56, 255));
            mater.SetFloat("_EmissiveColorPower", 1.55f);
            mater.SetFloat("_EmissivePower", 100);
            orbitalPrefab.sprite.renderer.material = mater;

            UnityEngine.Object.DontDestroyOnLoad(gameObject);
			FakePrefab.MarkAsFakePrefab(gameObject);
			gameObject.SetActive(false);
			GunknownGuon = gameObject;

            //================================================================================================================================

            GameObject gameObject_Soul = PrefabBuilder.BuildObject("Gun-Ternal Soul");

            tk2dSprite sprite_Soul = gameObject_Soul.AddComponent<tk2dSprite>();
            sprite_Soul.collection = StaticSpriteDefinitions.Guon_Sheet_Data;
            sprite_Soul.SetSprite(StaticSpriteDefinitions.Guon_Sheet_Data.GetSpriteIdByName("superguon"));
            sprite_Soul.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;

            Material mat = sprite_Soul.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite_Soul.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(104, 182, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 100);
            sprite_Soul.sprite.renderer.material = mat;
            UnknownGun.Soulprefab = gameObject_Soul;


            gun.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);

            Material materialGun = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            materialGun.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            materialGun.SetFloat("_EmissiveColorPower", 1.55f);
            materialGun.SetFloat("_EmissivePower", 60);
            materialGun.SetFloat("_EmissiveThresholdSensitivity", 0.2f);
            MeshRenderer component = gun.GetComponent<MeshRenderer>();
            if (!component)
            {
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == materialGun)
                {
                    return;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(materialGun);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;


            UnknownGun.GunknownID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int GunknownID;
		public static GameObject GunknownGuon;
		public static GameObject Soulprefab;

		public override void OnPostFired(PlayerController player, Gun bruhgun)
		{
			AkSoundEngine.PostEvent("Play_ENM_statue_ring_01", base.gameObject);
			gun.PreventNormalFireAudio = true;
		}
		private bool HasReloaded;
		public override void Update()
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
		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
		}
		public override void OnPickup(PlayerController player)
		{		
			player.OnAnyEnemyReceivedDamage += OnEnemyDamaged;
		}

		public override void OnPostDrop(PlayerController player)
		{
			player.OnAnyEnemyReceivedDamage -= OnEnemyDamaged;
			base.OnPostDrop(player);
		}
		private void OnEnemyDamaged(float damage, bool fatal, HealthHaver enemy)
		{
			if (base.Owner != null)
			{
                if (enemy.aiActor != null && fatal)
                {
                    GameManager.Instance.Dungeon.StartCoroutine(this.HandleSoulSucc(enemy.aiActor, gun.CurrentOwner.sprite.WorldCenter, UnityEngine.Random.Range(0.5f, 1.5f)));
                }
            }
		}
		private IEnumerator HandleSoulSucc(AIActor target, Vector2 table, float DuartionForSteal)
		{
			if (gun.CurrentOwner != null)
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>(UnknownGun.Soulprefab, target.sprite.WorldCenter, Quaternion.identity).GetComponent<tk2dSprite>();
				component.sprite.scale *= 0.5f;

				component.transform.parent = SpawnManager.Instance.VFX;
				GameObject gameObject2 = new GameObject("image parent");
				gameObject2.transform.position = component.WorldCenter;
				component.transform.parent = gameObject2.transform;


				Material mat = component.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = component.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(255, 177, 56, 255));
				mat.SetFloat("_EmissiveColorPower", 1.55f);
				mat.SetFloat("_EmissivePower", 100);
				component.sprite.renderer.material = mat;

				Transform copySprite = gameObject2.transform;

				Vector3 startPosition = target.transform.position;
				float elapsed = 0f;
				float duration = DuartionForSteal;
				while (elapsed < duration)
				{
					elapsed += BraveTime.DeltaTime;
					bool flag3 = player && copySprite && player != null;
					if (flag3)
					{
						Vector3 position = player.sprite.WorldCenter;
						float t = elapsed / duration * (elapsed / duration);
						copySprite.position = Vector3.Lerp(startPosition, position, t);
						copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
						copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
						position = default(Vector3);
					}
					yield return null;
				}
				if (player == null && copySprite)
				{
					UnityEngine.Object.Destroy(copySprite.gameObject);
					yield break;
				}
				bool flag4 = copySprite;
				if (flag4)
				{
					UnityEngine.Object.Destroy(copySprite.gameObject);
					yield break;
				}
				yield break;
			}
			else
			{
				yield break;
			}
		}

	}
}