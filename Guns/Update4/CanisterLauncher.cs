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
	public class CanisterLauncher : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Canister Launcher", "canisterlauncher");
			Game.Items.Rename("outdated_gun_mods:canister_launcher", "psog:canister_launcher");
			gun.gameObject.AddComponent<CanisterLauncher>();
			gun.SetShortDescription("Blow You Away");
			gun.SetLongDescription("Knocks around enemies and propels all nearly projectiles to dangerous speeds on collision. Originally a can-crushing device, it was repurposed when someone thought it would be funny to stick a can of compressed air into it.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "canisterlauncher_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.idleAnimation = "canisterlauncher_idle";
            gun.shootAnimation = "canisterlauncher_fire";
            gun.reloadAnimation = "canisterlauncher_reload";

            //GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
            //GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 7);
            //GunExt.SetAnimationFPS(gun, gun.idleAnimation, 7);



            for (int i = 0; i < 1; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(43) as Gun, true, false);
			}
            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.shootAnimation, new Dictionary<int, string> { { 0, "Play_Grenade_shot_01" }, { 5, "Play_OBJ_lock_unlock_01" } });

            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> { { 1, "Play_OBJ_hook_pull_03" }, { 4, "Play_OBJ_lock_unlock_01" },  { 8, "Play_WPN_pillow_reload_01" } });

            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = 0.75f;
				projectileModule.angleVariance = 5f;
				projectileModule.numberOfShotsInClip = 1;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 20f;
				projectile.AdditionalScaleMultiplier = 4f;
				projectile.baseData.range += 5;
				projectile.baseData.speed *= 0.75f;
                BounceProjModifier bouncy = projectile.gameObject.AddComponent<BounceProjModifier>();
                bouncy.numberOfBounces = 2;

                projectile.collidesWithProjectiles = true;

                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}
				projectile.objectImpactEventName = (PickupObjectDatabase.GetById(129) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
				projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(129) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

				projectile.baseData.UsesCustomAccelerationCurve = true;
				projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1, 1, 0.3f);


                projectile.gameObject.GetOrAddComponent<CanisterLauncherProjectile>();
                ExplosiveModifier explosiveModifier = projectile.gameObject.GetOrAddComponent<ExplosiveModifier>();
				explosiveModifier.explosionData = new ExplosionData(){
					breakSecretWalls = false,
					comprehensiveDelay = 0,
					damage = 15,
					damageRadius = 3,
					damageToPlayer = 0,
					debrisForce = 1000,
					doDamage = true,
					doDestroyProjectiles = false,
					doExplosionRing = false,
					doForce = true,
					doScreenShake = false,
					doStickyFriction = false,
					effect = null,
					explosionDelay = 0,
					force = 1000,
					forcePreventSecretWallDamage = false,
					forceUseThisRadius = true,
					freezeEffect = null,
					freezeRadius = 0,
					IsChandelierExplosion = false,
					isFreezeExplosion = false,
					playDefaultSFX = true,
					preventPlayerForce = false,
					pushRadius = 3,
					secretWallsRadius = 5,					
				};
				explosiveModifier.doExplosion = true;
				explosiveModifier.IgnoreQueues = true;


                projectile.AnimateProjectile(new List<string> {
                "canister_001",
                "canister_002",
                "canister_003",
                "canister_004",
                "canister_005",
                "canister_006",
                "canister_007",
                "canister_008",
            }, 13, true, new List<IntVector2> {
				new IntVector2(8, 8),
                new IntVector2(8, 8),       
                new IntVector2(8, 8),
                new IntVector2(8, 8),
                new IntVector2(8, 8),
                new IntVector2(8, 8),
                new IntVector2(8, 8),
                new IntVector2(8, 8),
            }, AnimateBullet.ConstructListOfSameValues(false, 8), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 8), AnimateBullet.ConstructListOfSameValues(true, 8), AnimateBullet.ConstructListOfSameValues(false, 8),
			   AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 8));
				projectile.SetProjectileSpriteRight("canister_001", 8, 8, false, tk2dBaseSprite.Anchor.MiddleCenter, 8, 8);
				projectile.shouldRotate = false;
                
                SpriteOutlineManager.AddOutlineToSprite(projectile.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);

            }
            gun.barrelOffset.transform.localPosition = new Vector3(0.75f, 0.375f, 0f);
			gun.reloadTime = 2f;
			gun.SetBaseMaxAmmo(60);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(122) as Gun).muzzleFlashEffects;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.gunClass = GunClass.EXPLOSIVE;

            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Canister", "Planetside/Resources/GunClips/Canister/canister_full", "Planetside/Resources/GunClips/Canister/canister_empty");

            gun.gunHandedness = GunHandedness.HiddenOneHanded;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(332) as Gun).gunSwitchGroup;

            gun.quality = PickupObject.ItemQuality.D;
			gun.encounterTrackable.EncounterGuid = "Can gun can gun, whacha gonna do";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			CanisterLauncher.CanisterLauncherID = gun.PickupObjectId;

		}
		public static int CanisterLauncherID;
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
			}
		}
	}
}