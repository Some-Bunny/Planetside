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
using SynergyAPI;

namespace Planetside
{
	public class Sawcon : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Saw Controlled Dispenser", "sawcon");
			Game.Items.Rename("outdated_gun_mods:saw_controlled_dispenser", "psog:saw_controlled_dispenser");
			gun.gameObject.AddComponent<Sawcon>();
			gun.SetShortDescription("Sawing Conventionally");
			gun.SetLongDescription("'Who needs OSHA regulations when launching miniature saw blades is infinitely more fun?' said someone after creating this abomination.\n\nThey died shortly after due to sickness.");
			gun.SetupSprite(null, "sawcon_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 7);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 15);

            for (int i = 0; i < 1; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(43) as Gun, true, false);
			}

            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.shootAnimation, new Dictionary<int, string> { { 0, "Play_WPN_deck4rd_shot_03" }, { 1, "Play_BOSS_hatch_close_01" }, { 10, "Play_OBJ_lock_unlock_01" }, { 8, "Play_ENM_wizardred_swing_01" } });
            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> { { 4, "Play_OBJ_lock_unlock_01" }});

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
				projectile.baseData.damage = 11f;
				projectile.AdditionalScaleMultiplier = 1f;
				projectile.baseData.range *= 10;
				projectile.baseData.speed *= 2.5f;
                projectile.shouldRotate = true;
				projectile.gameObject.AddComponent<SawconProjectile>();

                BounceProjModifier bouncy = projectile.gameObject.AddComponent<BounceProjModifier>();
                bouncy.numberOfBounces = 3;

                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}
				projectile.objectImpactEventName = (PickupObjectDatabase.GetById(341) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
				projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(341) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

                projectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(377) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
                projectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(377) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);

                projectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(369) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
                projectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(43) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);

                ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
                yes.spawnShadows = true;
				yes.shadowLifetime = 0.5f;
                yes.shadowTimeDelay = 0.01f;
                yes.dashColor = new Color(0.78f, 0.78f, 0.8f, 1f);
                yes.name = "Gun Trail";

                projectile.baseData.UsesCustomAccelerationCurve = true;
				projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1, 0.5f, 1.15f);

                projectile.AnimateProjectile(new List<string> {
                "sawsmall_idle_001",
                "sawsmall_idle_002",
                "sawsmall_idle_003",
                "sawsmall_idle_004",
                "sawsmall_idle_005",
                "sawsmall_idle_006",
                "sawsmall_idle_007",
            }, 20, true, new List<IntVector2> {
				new IntVector2(14, 3),
                new IntVector2(14, 3),
                new IntVector2(14, 3),
                new IntVector2(14, 3),
                new IntVector2(14, 3),
                new IntVector2(14, 3),
                new IntVector2(14, 3),

            }, AnimateBullet.ConstructListOfSameValues(false, 8), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 8), AnimateBullet.ConstructListOfSameValues(true, 8), AnimateBullet.ConstructListOfSameValues(false, 8),
			   AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 8), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 8));
				projectile.SetProjectileSpriteRight("sawsmall_idle_001", 14, 3, false, tk2dBaseSprite.Anchor.MiddleCenter, 14, 3);
                
            }
            gun.barrelOffset.transform.localPosition = new Vector3(0.75f, 0.375f, 0f);
			gun.reloadTime = 2.5f;
			gun.SetBaseMaxAmmo(90);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(122) as Gun).muzzleFlashEffects;
			gun.gunClass = GunClass.SILLY;

			gun.carryPixelOffset += new IntVector2(9, -1);

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Sawcon", "Planetside/Resources/GunClips/Sawcon/sawfull", "Planetside/Resources/GunClips/Sawcon/sawemptyl");



            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(332) as Gun).gunSwitchGroup;

            gun.quality = PickupObject.ItemQuality.C;
			gun.encounterTrackable.EncounterGuid = "Sawcon these saws lmao";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			CanisterLauncher.CanisterLauncherID = gun.PickupObjectId;

			
            tk2dSpriteAnimationClip fireClip2 = gun.sprite.spriteAnimator.GetClipByName(gun.shootAnimation);
            float[] offsetsX2 = new float[] { 
			0f, //1
			0f, //2
			0f, //3
			0f, //4
			0f, //5
			0f, //6
			0f, //7
			0f, //8
			0.125f, //9 
			0.25f, //10
			0.0625f, //11
			-0.125f, //12
			-0.25f, //13
			-0.0625f, //14
			0.0625f, //15//=======
			0f  //16
			};
			
			float[] offsetsY2 = new float[]
			{ 0f, //1
			 0f,//2
			 0f, //3
			-0.875f, //4
			-1.0625f, //5
			-1.125f, //6
			-1.0625f, //7
			-1.25f,//8
			-0.9375f, //9 
			-0.125f,//10
			0.1875f,//11
			0.3125f, //12
			0.375f,//13 
			0.3125f,//14
			0.25f,//15//=======
			0.125f//16
			};
            
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

            List<string> yah = new List<string>
            {
                "psog:saw_controlled_dispenser",
                "bullet_bore"
            };
            CustomSynergies.Add("Screwdriver", yah, null, true);
            new Hook(typeof(CerebralBoreProjectile).GetMethod("HandleBoring", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Sawcon).GetMethod("HandleBoringHook"));


            List<string> two = new List<string>
            {
                "psog:saw_controlled_dispenser",
            };
            List<string> twoTwo = new List<string>
            {
                "buzzkill",
				"super_meat_gun"
            };
            CustomSynergies.Add("Saw Your Heart Out", two, twoTwo, true);

			Gun buzzkill = (PickupObjectDatabase.GetById(341) as Gun);
			buzzkill.DefaultModule.projectiles[0].gameObject.AddComponent<SawconMagnetAffected>();

            Gun superMeatGun = (PickupObjectDatabase.GetById(479) as Gun);
            superMeatGun.DefaultModule.projectiles[0].gameObject.AddComponent<SawconMagnetAffected>();


            //new Hook(typeof(CerebralBoreProjectile).GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Sawcon).GetMethod("OnDestroyHook"));

        }

        public static void HandleBoringHook(Action<CerebralBoreProjectile> orig, CerebralBoreProjectile self)
		{
			var player = self.Owner as PlayerController;
            if (player != null && player.PlayerHasActiveSynergy("Screwdriver") == true)
			{
				AIActor target =  PlanetsideReflectionHelper.ReflectGetField<AIActor>(typeof(CerebralBoreProjectile), "m_targetEnemy", self);
				if (target)
				{
                    target.healthHaver.ApplyDamage(20 * BraveTime.DeltaTime, self.transform.PositionVector2(), "Bored");
                    GlobalSparksDoer.DoSingleParticle(self.GetComponentInChildren<tk2dBaseSprite>().WorldCenter, UnityEngine.Random.insideUnitCircle * 3, null, null, null, GlobalSparksDoer.SparksType.BLOODY_BLOOD);
                }
            }
			orig(self);
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