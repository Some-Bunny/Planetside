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
using Planetside.Static_Storage;
using Planetside.Toolboxes;

namespace Planetside
{
	public class SawBladeGun : GunBehaviour
	{


		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Box Of Surprises", "sawgunthesenuts");
			Game.Items.Rename("outdated_gun_mods:box_of_surprises", "psog:box_of_surprises");
			gun.gameObject.AddComponent<SawBladeGun>();
			gun.SetShortDescription("Fun Inside!");
			gun.SetLongDescription("A box filled to the brim with VERY FUN and NOT LETHAL objects inside.\nPoint box towards whomever you want to be surprised, and pull the cord!\n\nPainted green because green is a VERY NICE and NOT DANGEROUS color.");
            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "boxofsurprises_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.idleAnimation = "boxofsurprises_idle";
            gun.shootAnimation = "boxofsurprises_fire";
            gun.reloadAnimation = "boxofsurprises_reload";

            gun.chargeAnimation = "boxofsurprises_charge";

            GunExt.AddProjectileModuleFrom(gun, Guns.Pea_Shooter, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(387) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 3f;
			gun.DefaultModule.cooldownTime = .5f;
			gun.DefaultModule.numberOfShotsInClip = 1;
			gun.SetBaseMaxAmmo(40);
			gun.quality = PickupObject.ItemQuality.A;
			gun.DefaultModule.angleVariance = 6f;
			gun.DefaultModule.burstShotCount = 1;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);





            var hit = projectile.gameObject.AddComponent<SawBladeGunProjectile>();
            hit.baseData = new ProjectileData();
            hit.CopyFrom<Projectile>(projectile);
            hit.baseData.CopyFrom<ProjectileData>(projectile.baseData);
            hit.baseData.damage = 60f;
            hit.baseData.speed = 50f;
			hit.baseData.range *= 15;
            Destroy(projectile);
            Alexandria.Assetbundle.ProjectileBuilders.SetProjectileCollisionRight(projectile, "giantSaw_001", StaticSpriteDefinitions.Projectile_Sheet_Data, 64, 64, false, tk2dBaseSprite.Anchor.MiddleCenter, 4, 4, true, false, -2, -2);
			hit.gameObject.layer = Layers.FG_Critical;
			hit.sprite.HeightOffGround = 0.7f;
			hit.sprite.SortingOrder = 1;
			hit.sprite.IsPerpendicular = false;

            Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(hit, "saw_shot", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "saw_shot",
			AnimateBullet.ConstructListOfSameValues<IntVector2>(new IntVector2(64, 64), 10),
            AnimateBullet.ConstructListOfSameValues(false, 10),
            AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 10),
            AnimateBullet.ConstructListOfSameValues(true, 10),
            AnimateBullet.ConstructListOfSameValues(false, 10),
            AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 10),
            AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(4, 4), 10),
			AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(-2, -2), 10),
            AnimateBullet.ConstructListOfSameValues<Projectile>(null, 10));


            //saw_shot

            hit.sprite.usesOverrideMaterial = true;
            Material mat_1 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat_1.mainTexture = hit.sprite.renderer.material.mainTexture;
            mat_1.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
            mat_1.SetFloat("_EmissiveColorPower", 10);
            mat_1.SetFloat("_EmissivePower", 10);
            hit.sprite.renderer.material = mat_1;

            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.3f;
            yes.shadowTimeDelay = 0.025f;
            yes.dashColor = new Color(0.5f, 0.5f, 0.5f, 0);
			yes.targetHeight = 0.4f;
		
            projectile = hit;
            projectile.shouldRotate = false;
            projectile.pierceMinorBreakables = true;
            projectile.PenetratesInternalWalls = false;


            PierceProjModifier spook = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            spook.penetration = 100000;

			//hit._SpeculativeRigidbody = projectile.gameObject.CreateFastBody(new IntVector2(60, 60), new IntVector2(-30, -30), CollisionLayer.Projectile, true);

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("psog:BoxOfSurprises", StaticSpriteDefinitions.PlanetsideClipUIAtlas, "SawGun_001", "SawGun_002");

            //projectile.SetProjectileSpriteRight("saw_projectile_001", 32, 32, false, tk2dBaseSprite.Anchor.MiddleCenter, 16, 16, true, false, 8, 8);// true, 16, 16, 8, 8);

            gun.DefaultModule.projectiles[0] = projectile;

			//projectile.debris = BreakAbleAPI.BreakableAPIToolbox.GenerateDebrisObject("Planetside/Resources/GunObjects/Clips/bigassmag.png", true, 1, 3, 60, 20, null, 2, "Play_ITM_Crisis_Stone_Impact_02", null, 1).gameObject;

			//===
			gun.gunClass = GunClass.NONE;


			gun.reloadClipLaunchFrame = 0;
			gun.clipsToLaunchOnReload = 0;
			gun.gunSwitchGroup = Guns.Gunbow.gunSwitchGroup;
			

			gun.encounterTrackable.EncounterGuid = "minjlokikjhnikjhnjmnkkojhnilnoiloknjhkjhniloikjhlo";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			SawBladeGun.ID = gun.PickupObjectId;

            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 1.3f
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>() { item2 };
            gun.muzzleFlashEffects = Guns.Serious_Cannon.muzzleFlashEffects;

            //m_WPN_sling_shot_01
            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.chargeAnimation, new Dictionary<int, string> {
                { 3, "Play_BOSS_cannon_stop_01" }, { 13, "Play_BOSS_cannon_stop_01" }, { 23, "Play_BOSS_cannon_stop_01" },
                { 24, "Play_OBJ_chandelier_impact_01" } ,
            });
            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.shootAnimation, new Dictionary<int, string> {
                {0, "Play_Railgun" }, { 1, "Play_ENM_cannonball_blast_01" },
            });
            EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> {
                { 10, "Play_ENM_cannonball_blast_01" },
            });


            gun.AddSynergy("Blades Of Wrath", new List<PickupObject>() 
            {
                Guns.Buzzkill,
                Guns.Super_Meat_Gun,
            });

            Guns.Buzzkill.DefaultModule.projectiles[0].gameObject.AddComponent<FunSurprise>();
            Guns.Super_Meat_Gun.DefaultModule.projectiles[0].gameObject.AddComponent<FunSurprise>();
            gun.AddToSubShop(ItemAPI.ItemBuilder.ShopType.Cursula, 1);
            gun.AddToSubShop(ItemAPI.ItemBuilder.ShopType.Trorc, 1);

        }
        public static int ID;

        public class FunSurprise : MonoBehaviour
        {
            public void Start()
            {
                projectile = base.GetComponent<Projectile>();
                if (projectile != null)
                {
                    var player = projectile.Owner as PlayerController;
                    if (player && player.PlayerHasActiveSynergy("Blades Of Wrath"))
                    {
                        projectile.OnHitEnemy += (_, __, ___) =>
                        {
                            projectile.baseData.damage *= 1.05f;
                            projectile.baseData.speed *= 1.15f;
                            projectile.UpdateSpeed();

                            for (int i = 0; i < 12; i++)
                            {

                                ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                                {
                                    position = projectile.sprite.WorldCenter,
                                    rotation = 0,
                                    startLifetime = 0.5f,
                                    startColor = Color.red,
                                    velocity = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(projectile.baseData.speed * 0.05f, projectile.baseData.speed * 0.1f)) + projectile.LastVelocity * 0.1f
                                });
                            }
                            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Alexandria.Misc.GoopUtility.BloodDef).TimedAddGoopCircle(projectile.sprite.WorldBottomCenter, 1, 0.7f);
                        };
                    }
                }
            }
            private Projectile projectile;
        }
    }
}