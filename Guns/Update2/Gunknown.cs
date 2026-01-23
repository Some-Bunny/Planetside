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
using Alexandria.Assetbundle;

namespace Planetside
{
	public class UnknownGun : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Gunknown", "gunknown");
			Game.Items.Rename("outdated_gun_mods:gunknown", "psog:gunknown");
			var behav = gun.gameObject.AddComponent<UnknownGun>();
			gun.SetShortDescription("Eternal Strength");
			gun.SetLongDescription("The Gun-Ternal Power offers infinite strength, as long as it is sated.\n\nWorshipped by many a thing, both living and not, residing in an ancient monolithic construction.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "gunknown_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.idleAnimation = "gunknown_idle";
            gun.shootAnimation = "gunknown_fire";
            gun.reloadAnimation = "gunknown_reload";
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(21) as Gun).gunSwitchGroup;
            //gun.SetupSprite(null, "gunknown_idle_001", 11);
            //GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            //GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 1000);
            //GunExt.SetAnimationFPS(gun, gun.idleAnimation, 7);

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
                projectile.AddComponent<RecursionPreventer>();

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
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Gunknown", StaticSpriteDefinitions.PlanetsideClipUIAtlas, "gunknownclipfull", "gunknownclipemptyl");


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
			sprite_Soul.usesOverrideMaterial = true;
            Material mat = sprite_Soul.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite_Soul.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 177, 56, 255));
            mat.SetFloat("_EmissiveColorPower", 15);
            mat.SetFloat("_EmissivePower", 10);
            sprite_Soul.sprite.renderer.material = mat;
            UnknownGun.Soulprefab = gameObject_Soul;


            gun.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);

            Material materialGun = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            materialGun.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            materialGun.SetFloat("_EmissiveColorPower", 15f);
            materialGun.SetFloat("_EmissivePower", 10);
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
            behav.Material = material;


            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:gunknown",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "bracket_key"
            };
            CustomSynergies.Add(".null", mandatoryConsoleIDs, optionalConsoleIDs, false);

            UnknownGun.GunknownID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int GunknownID;
		public static GameObject GunknownGuon;
		public static GameObject Soulprefab;
		public Material Material;

		private bool HasReloaded;
		private bool HasFlipped = false;

        public override void Update()
		{
			base.Update();
			if (gun.CurrentOwner)
			{
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}
				var player = gun.CurrentOwner as PlayerController;

                if (!player.PlayerHasActiveSynergy(".null") && HasFlipped != false)
				{
					HasFlipped = false;
					Material.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
                    Material.SetFloat("_EmissiveColorPower", 1.55f);
					Material.SetFloat("_EmissivePower", 100);
					Material.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
					Material.SetTexture("_MainTex", gun.sprite.renderer.material.GetTexture("_MainTex"));
					gun.sprite.usesOverrideMaterial = true;
					gun.sprite.renderer.material = Material;

				}
				else if (player.PlayerHasActiveSynergy(".null") && HasFlipped == false)
				{
					HasFlipped = true;
					Material.SetColor("_EmissiveColor", new Color32(0, 227, 255, 255));
					Material.SetFloat("_EmissiveColorPower", 10f);
					Material.SetFloat("_EmissivePower", 5);
					Material.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
					Material.SetTexture("_MainTex", gun.sprite.renderer.material.GetTexture("_MainTex"));
					gun.sprite.usesOverrideMaterial = true;
					gun.sprite.renderer.material = Material;
				}
			}

		}
		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
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
			if (base.Owner != null && enemy.aiActor != null && fatal)
			{
                GameManager.Instance.Dungeon.StartCoroutine(this.HandleSoulSucc(enemy.aiActor,UnityEngine.Random.Range(0.5f, 1.5f)));

            }
        }
		private IEnumerator HandleSoulSucc(AIActor target, float DuartionForSteal)
		{
			if (gun.CurrentOwner != null)
			{
				PlayerController player = gun.CurrentOwner as PlayerController;
				tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>(UnknownGun.Soulprefab, target.sprite.WorldCenter, Quaternion.identity).GetComponent<tk2dSprite>();
				component.sprite.scale *= 0.5f;

				if (player.PlayerHasActiveSynergy(".null") )
				{
                    component.SetSprite(StaticSpriteDefinitions.Guon_Sheet_Data.GetSpriteIdByName("pointNullRune"));
                    Material mat = component.renderer.material;
                    mat.SetColor("_EmissiveColor", new Color32(0, 227, 255, 255));
                    mat.SetFloat("_EmissiveColorPower", 15);
                    mat.SetFloat("_EmissivePower", 10);
                }


                component.transform.parent = SpawnManager.Instance.VFX;
				GameObject gameObject2 = new GameObject("image parent");
				gameObject2.transform.position = component.WorldCenter;
				component.transform.parent = gameObject2.transform;

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
				if (copySprite)
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