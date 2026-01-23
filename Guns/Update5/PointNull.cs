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
	public class PointNull : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("PointNull", "pointnull");
			Game.Items.Rename("outdated_gun_mods:pointnull", "psog:pointnull");
			gun.gameObject.AddComponent<PointNull>();
			gun.SetShortDescription("Eternal Strength");
			gun.SetLongDescription("The Gun-Ternal Power offers infinite strength, as long as it is sated.\n\nWorshipped by many a thing, both living and not, residing in an ancient monolithic construction.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "pointgunknown_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.idleAnimation = "pointnull_idle";
            gun.shootAnimation = "pointnull_fire";
            gun.reloadAnimation = "pointnull_reload";



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
				if (projectileModule != gun.DefaultModule)
				{
					projectileModule.ammoCost = 0;
				}
				projectile.transform.parent = gun.barrelOffset;				
			}
			gun.barrelOffset.transform.localPosition = new Vector3(0.6875f, 0.4375f, 0f);
			gun.reloadTime = 0f;
			gun.InfiniteAmmo = true;
			gun.SetBaseMaxAmmo(1000);
			gun.carryPixelOffset = new IntVector2(0, 12);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(21) as Gun).gunSwitchGroup;

            gun.ActiveItemStyleRechargeAmount = 750;
			gun.UsesRechargeLikeActiveItem = true;
			Gun duellaser = PickupObjectDatabase.GetById(370) as Gun;
			gun.DefaultModule.customAmmoType = duellaser.DefaultModule.customAmmoType;
			gun.gunHandedness = GunHandedness.NoHanded;
			gun.preventRotation = true;
			gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
			gun.gunClass = GunClass.CHARGE;

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("PointNull", StaticSpriteDefinitions.PlanetsideClipUIAtlas, "pointnullclip_001", "pointnullclip_002");


            gun.quality = PickupObject.ItemQuality.EXCLUDED;
			ETGMod.Databases.Items.Add(gun, false, "ANY");

            #region Guon
            //====================================================================================================================
            GameObject gameObject = PrefabBuilder.BuildObject("PointNullRune");

            tk2dSprite sprite = gameObject.AddComponent<tk2dSprite>();
            sprite.collection = StaticSpriteDefinitions.Guon_Sheet_Data;
            sprite.SetSprite(StaticSpriteDefinitions.Guon_Sheet_Data.GetSpriteIdByName("pointNullRune"));
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
            fuck.dashColor = new Color(0, 0.05f, 0.4f);

            orbitalPrefab.sprite.usesOverrideMaterial = true;
            Material mater = orbitalPrefab.sprite.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mater.mainTexture = orbitalPrefab.sprite.renderer.material.mainTexture;
            mater.SetColor("_EmissiveColor", new Color32(0, 227, 255, 255));
            mater.SetFloat("_EmissiveColorPower", 5f);
            mater.SetFloat("_EmissivePower", 2);
            orbitalPrefab.sprite.renderer.material = mater;

            UnityEngine.Object.DontDestroyOnLoad(gameObject);
			FakePrefab.MarkAsFakePrefab(gameObject);
			gameObject.SetActive(false);
            PointNullRune = gameObject;
			#endregion
            //================================================================================================================================





            Material materialGun = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            materialGun.SetColor("_EmissiveColor", new Color32(0, 227, 255, 255));
            materialGun.SetFloat("_EmissiveColorPower", 15f);
            materialGun.SetFloat("_EmissivePower", 20);
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



            Projectile dart = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(35) as Gun).DefaultModule.projectiles[0]);
            dart.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(dart.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(dart);

            dart.baseData.UsesCustomAccelerationCurve = true;
            dart.baseData.AccelerationCurve = AnimationCurve.Linear(0.25f, 0.1f, 1f, 1f);

            var spook = dart.gameObject.GetOrAddComponent<PierceProjModifier>();
            spook.penetration += 3;

            //var bouncy = dart.gameObject.AddComponent<BounceProjModifier>();
            //bouncy.numberOfBounces = 1;

            dart.baseData.damage = 6;
            dart.baseData.speed = 25;

            dart.shouldRotate = true;
            Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(dart, "pointnull_shard", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "pointnull_shard",
            new List<IntVector2>() { new IntVector2(23, 11), new IntVector2(23, 11), new IntVector2(23, 11), new IntVector2(23, 11), new IntVector2(23, 11), new IntVector2(23, 11), },
            AnimateBullet.ConstructListOfSameValues(true, 6),
            AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 6),
            AnimateBullet.ConstructListOfSameValues(true, 6),
            AnimateBullet.ConstructListOfSameValues(false, 6),
            AnimateBullet.ConstructListOfSameValues<Vector3?>(new Vector2(0, 0), 6),
            AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 6),
            AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(0, 0), 6),
            AnimateBullet.ConstructListOfSameValues<Projectile>(null, 6));

            ImprovedAfterImage yes = dart.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.33f;
            yes.shadowTimeDelay = 0.05f;
            yes.dashColor = new Color(0f, 0.2f, 1f, 1f);

            PointNullDart = dart;



            GameObject Shield = PrefabBuilder.BuildObject("PointNullShield");
			var shieldSprite = Shield.AddComponent<tk2dSprite>();
            shieldSprite.collection = StaticSpriteDefinitions.Gun_Sheet_Data;
            shieldSprite.SetSprite(StaticSpriteDefinitions.Gun_Sheet_Data.GetSpriteIdByName("pointgunknown_shield_005"));
            shieldSprite.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;

            var shieldAnimation = Shield.AddComponent<tk2dSpriteAnimator>();
			shieldAnimation.library = StaticSpriteDefinitions.Gun_Animation_Data;
			shieldAnimation.defaultClipId = StaticSpriteDefinitions.Gun_Animation_Data.GetClipIdByName("pointnull_shield_up");
			shieldAnimation.playAutomatically = true;

            shieldSprite.usesOverrideMaterial = true;
			shieldSprite.sprite.renderer.sortingLayerName = "Foreground";
            var shielldMat = new Material(ShaderCache.Acquire("Brave/Internal/DownwellAfterImage"));
            shielldMat.SetColor("_DashColor", Color.cyan);
            shielldMat.SetFloat("_EmissivePower", 12);
            shielldMat.SetFloat("_Opacity", 0.75f);

            shielldMat.mainTexture = shieldSprite.renderer.material.mainTexture;
            shieldSprite.renderer.material = shielldMat;
            PointNullShield = Shield;

            PointNull.PointNullID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);

            new Hook(typeof(DamageEnemiesInRadiusItem).GetMethod("DoEffect", BindingFlags.Instance | BindingFlags.NonPublic), typeof(PointNull).GetMethod("DoEffectHook"));

            AdvancedTransformGunSynergyProcessor hexaSyn = (PickupObjectDatabase.GetById(UnknownGun.GunknownID) as Gun).gameObject.AddComponent<AdvancedTransformGunSynergyProcessor>();
            hexaSyn.NonSynergyGunId = UnknownGun.GunknownID;
            hexaSyn.SynergyGunId = PointNull.PointNullID;
            hexaSyn.SynergyToCheck = ".null";

        }
        public static int PointNullID;
		public static GameObject PointNullRune;
        public static GameObject PointNullShield;

        public static Projectile PointNullDart;

        public static void DoEffectHook(Action<DamageEnemiesInRadiusItem, PlayerController> orig, DamageEnemiesInRadiusItem self, PlayerController user)
        {
			orig(self, user);
			if (user.PlayerHasActiveSynergy(".null") && self.PickupObjectId == 439)
			{
                AkSoundEngine.PostEvent("Play_BOSS_RatMech_Target_01", user.gameObject);
                user.StartCoroutine(DoShield(user));
			}
        }

		public static IEnumerator DoShield(PlayerController playerController)
		{
			var objec = UnityEngine.Object.Instantiate(PointNullShield, playerController.transform);
			objec.transform.position = playerController.sprite.WorldCenter.ToVector3ZUp(6) + new Vector3(0, -0.5f, 0);
            AkSoundEngine.PostEvent("Play_OBJ_cursepot_loop_01", objec.gameObject);
            var sprite = objec.GetComponent<tk2dBaseSprite>();
            playerController.healthHaver.PreventAllDamage = true;
            float e = 0;
            float e_ = 0;
            while (e < 15)
			{
				if (e_ <= 1)
				{
					e_ = 0;
					for (int i = 0; i < 2; i++)
					{
                        Vector3 vector = sprite.WorldBottomLeft.ToVector3ZisY(0);
                        Vector3 vector2 = sprite.WorldTopRight.ToVector3ZisY(0);
                        Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
                        ParticleSystem particleSystem = StaticVFXStorage.EliteParticleSystem;
                        var trails = particleSystem.trails;
                        trails.worldSpace = false;
                        var main = particleSystem.main;
                        main.startColor = new ParticleSystem.MinMaxGradient(Color.white,  Color.cyan);
						ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
						{
							position = position,
							randomSeed = (uint)UnityEngine.Random.Range(1, 1000),
							velocity = Vector3.zero,
							startSize = 0.33f
						};
                        var emission = particleSystem.emission;
                        emission.enabled = false;
                        particleSystem.gameObject.SetActive(true);
                        particleSystem.Emit(emitParams, 1);
                    }
                }
				objec.transform.position = objec.transform.position.WithZ(6);

                sprite.renderer.material.SetColor("_DashColor", Color.Lerp(Color.cyan* 5, Color.white * 5, Mathf.PingPong(e, 1)));
				e += Time.deltaTime * 3;
                e_ += Time.deltaTime * 3;
                yield return null;
			}
            AkSoundEngine.PostEvent("Stop_OBJ_cursepot_loop_01", objec.gameObject);

            objec.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("pointnull_shield_down");

            playerController.healthHaver.PreventAllDamage = false;
            AkSoundEngine.PostEvent("Play_BOSS_agunim_deflect_01", playerController.gameObject);
            yield break;
		}

		private bool HasReloaded;
		public override void Update()
		{
			base.Update();
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
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
		}
	
	}
}