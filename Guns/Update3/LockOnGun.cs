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
using Alexandria.Misc;
using Alexandria.Assetbundle;
using Alexandria.Integrations;
using UnityEngine.EventSystems;
using static Planetside.Inquisitor;
using Planetside.Toolboxes;

namespace Planetside
{
	public class LockOnGun : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("T4-GTR", "lockongun");
			Game.Items.Rename("outdated_gun_mods:t4gtr", "psog:t4gtr");
			gun.gameObject.AddComponent<LockOnGun>();
			gun.SetShortDescription("Locked And Loaded");
			gun.SetLongDescription("Fire kinetic mines. Hold fire to activate tripwire mode on all kinetic mines and to redirect them. Release to reactivate speed.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "lockongun_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.reloadAnimation = "lockongun_reload";
            gun.idleAnimation = "lockongun_idle";
            gun.shootAnimation = "lockongun_fire";
            gun.chargeAnimation = "lockongun_charge";


            gun.spriteAnimator.GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_wpn_voidcannon_shot_01";
			gun.spriteAnimator.GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;

            gun.spriteAnimator.GetClipByName(gun.chargeAnimation).frames[4].eventAudio = "Play_OBJ_mine_beep_01";
            gun.spriteAnimator.GetClipByName(gun.chargeAnimation).frames[4].triggerEvent = true;
            gun.spriteAnimator.GetClipByName(gun.chargeAnimation).frames[1].eventAudio = "Play_WPN_gunbow_charge_01";
            gun.spriteAnimator.GetClipByName(gun.chargeAnimation).frames[1].triggerEvent = true;

            gun.spriteAnimator.GetClipByName(gun.reloadAnimation).frames[10].eventAudio = "Play_OBJ_mine_beep_01";
            gun.spriteAnimator.GetClipByName(gun.reloadAnimation).frames[10].triggerEvent = true;
            gun.spriteAnimator.GetClipByName(gun.reloadAnimation).frames[11].eventAudio = "Play_OBJ_mine_beep_01";
            gun.spriteAnimator.GetClipByName(gun.reloadAnimation).frames[11].triggerEvent = true;

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(345) as Gun, true, false);
			gun.gunSwitchGroup = Guns.Charge_Shot.gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2.2f;
			gun.DefaultModule.cooldownTime = .2f;
			gun.DefaultModule.numberOfShotsInClip = 12;
			gun.SetBaseMaxAmmo(216);
			gun.quality = PickupObject.ItemQuality.A;
			gun.DefaultModule.burstCooldownTime = 0.0833f;

			gun.DefaultModule.angleVariance = 18f;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 8.75f;
			projectile.baseData.speed = 20;
			projectile.shouldRotate = true;


			projectile.pierceMinorBreakables = true;
			var pp = projectile.gameObject.AddComponent<LockOnGunProjectile>();
            pp.baseData = new ProjectileData();
            pp.CopyFrom<Projectile>(projectile);
            pp.baseData.CopyFrom<ProjectileData>(projectile.baseData);
            pp.sprite = projectile.sprite;
            Destroy(projectile);

			pp.shouldRotate = true;
            pp.baseData.UsesCustomAccelerationCurve = true;
            pp.baseData.CustomAccelerationCurveDuration = 0.5f;
            pp.baseData.AccelerationCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.05f);
            pp.hitEffects = Guns.Charge_Shot.DefaultModule.chargeProjectiles[0].Projectile.hitEffects;

            Alexandria.Assetbundle.ProjectileBuilders.SetProjectileCollisionRight(pp, "lockonprojectile", StaticSpriteDefinitions.Projectile_Sheet_Data, 19, 11, false, tk2dBaseSprite.Anchor.MiddleCenter);

            Material mat1 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat1.mainTexture = pp.sprite.renderer.material.mainTexture;
            mat1.SetColor("_EmissiveColor", new Color32(107, 255, 135, 255));
            mat1.SetFloat("_EmissiveColorPower", 1.55f);
            mat1.SetFloat("_EmissivePower", 100);
            pp.sprite.renderer.material = mat1;

            ImprovedAfterImage yes = pp.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.8f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0.2f, 1f, 0.55f, 0.01f);
            yes.name = "Gun Trail";

            //ExplosiveModifier explosiveModifier = projectile.gameObject.AddComponent<ExplosiveModifier>();
            //explosiveModifier.doExplosion = true;
            //explosiveModifier.explosionData = StaticExplosionDatas.explosiveRoundsExplosion;

            gun.gunClass = GunClass.EXPLOSIVE;
            pp.pierceMinorBreakables = true;

			gun.encounterTrackable.EncounterGuid = "haha funny big shot";
			ETGMod.Databases.Items.Add(gun, false, "ANY");


            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = pp,
                ChargeTime = 0f
            };
            ProjectileModule.ChargeProjectile item3 = new ProjectileModule.ChargeProjectile
            {
                Projectile = null,
                AdditionalWwiseEvent = "Play_BOSS_lichC_zap_01",
                ChargeTime = 0.2f,
                UsedProperties = ProjectileModule.ChargeProjectileProperties.ammo | ProjectileModule.ChargeProjectileProperties.additionalWwiseEvent,
				AmmoCost = 0
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>() { item2, item3 };
            gun.gunHandedness = GunHandedness.OneHanded;



            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.SetColor("_EmissiveColor", new Color32(107, 255, 135, 255));
			mat.SetFloat("_EmissiveColorPower", 1.55f);
			mat.SetFloat("_EmissivePower", 30);
			mat.SetFloat("_EmissiveThresholdSensitivity", 0.15f);
			MeshRenderer component = gun.GetComponent<MeshRenderer>();
			if (!component)
			{
				return;
			}
			Material[] sharedMaterials = component.sharedMaterials;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				if (sharedMaterials[i].shader == mat)
				{
					return;
				}
			}
			Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
			Material material = new Material(mat);
			material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
			sharedMaterials[sharedMaterials.Length - 1] = material;
			component.sharedMaterials = sharedMaterials;
			SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.HOMING_BOMBS);

            List<string> AAA = new List<string>
            {
                "psog:t4gtr",
            };

            List<string> AAA1 = new List<string>
            {
                "homing_bullets",
                "remote_bullets",
                "crutch"
            };
            CustomSynergies.Add("No Virus Included", AAA, AAA1, false).AddItemTip("T4-GTR mines directly track the cursor. Controller users have mines automatically aim to the nearest enemy.");
            List<string> AAA2 = new List<string>
            {
                "stinger",
                "rpg",
                "yari_launcher"
            };
            CustomSynergies.Add("Full Arsenal", AAA, AAA2, false).AddItemTip("Fires homing rockets while kinetic mines are aiming at enemies.");
            List<string> AAA3 = new List<string>
            {
                "stinger",
                "rpg",
                "yari_launcher"
            };
            CustomSynergies.Add("Full Arsenal", AAA, AAA2, false).AddItemTip("Fires homing rockets while kinetic mines are aiming at enemies.");
            ImprovedSynergySetup.Add("To The Point", new List<PickupObject>()
            {
                gun,
                Items.Laser_Sight,
            }).AddItemTip("Reduced mine speed when aiming at enemies, and now slow and damage enemies when aiming at them.");
            
            /*




			List<string> yes = new List<string>
			{
				"psog:t4gtr",
				"rc_rocket"
			};
			CustomSynergies.Add("Double Trouble!", yes, null, false).AddItemTip("T4-GTR and the RC Rocket and dual wielded.");
			*/

            LockOnGun.LockOnGunID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
            gun.AddToSubShop(ItemAPI.ItemBuilder.ShopType.Trorc, 1);
			gun.AddItemTip("Fires kinetic mines. Hold the fire button activate the tripwire system of all active kinetic mines, and to change the direction they face. Mines speed up if an enemy is currently standing in its tripwire.");

            gun.barrelOffset.transform.localPosition = new Vector3(2.25f, 0.625f, 0f);
			gun.muzzleFlashEffects = Guns.Bullet_Bore.muzzleFlashEffects;
        }
        public static int LockOnGunID;
		public bool ActiveCharge = false;
		private Vector2 PointingAt;
        private float t = 0;
        public override void Update()
		{

			if (gun.CurrentOwner is PlayerController player)
			{
                if (player.CurrentGun != null && player.CurrentGun == gun)
                {
                    BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(player.PlayerIDX);
                    if (instanceForPlayer.GetButton(GungeonActions.GungeonActionType.Shoot))
                    {
                        t += BraveTime.DeltaTime;
                    }
                    else if (ActiveCharge)
                    {
                        t = 0;
                        DisableRedirect(true);
                    }
                    if (t >= 0.2f)
                    {
                        if (ActiveCharge == false)
                        {
                            ActiveCharge = true;
                            AkSoundEngine.PostEvent("Play_OBJ_computer_boop_01", this.gameObject);
                            Exploder.DoDistortionWave(this.gun.sprite.WorldCenter, 2, 0.0333f, 100, 0.5f);
                            foreach (var item in LockOnGunProjectile.AllLockOns)
                            {
                                item.SetRedirectMode(true);
                            }
                        }
                        gun.ForceLaserSight = true;

                        Vector2 vector = gun.m_localAimPoint - gun.m_owner.specRigidbody.HitboxPixelCollider.UnitCenter; ;
                        foreach (var item in LockOnGunProjectile.AllLockOns)
                        {
                            item.SetPointTowards(vector);
                        }
                    }
                }
            }
            else if (ActiveCharge)
            {
                DisableRedirect(true);
            }

        }


		public void DisableRedirect(bool Audio = false)
		{
            gun.ForceLaserSight = false;
            ActiveCharge = false;
            foreach (var item in LockOnGunProjectile.AllLockOns)
            {
                item.SetRedirectMode(false);
            }
			if (Audio)
			{
                AkSoundEngine.PostEvent("Play_OBJ_supplydrop_activate_01", this.gameObject);
            }
        }

        public override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
            player.GunChanged += GunChanged;
        }
        public override void OnPostDrop(PlayerController player)
        {
            base.OnPostDrop(player);
            player.GunChanged -= GunChanged;
            DisableRedirect();

        }

        public void OnDestroy()
        {
            if (Owner && Owner is PlayerController P)
            {
				P.GunChanged -= GunChanged;
            }
        }


		public void GunChanged(Gun gun1, Gun gun2, bool b)
		{
			if (gun2.PickupObjectId != LockOnGunID)
			{
                DisableRedirect();
            }
        }


        /*
		public static GameObject LockOnPrefab;
		public static List<int> spriteIds = new List<int>();
		public static tk2dBaseSprite LockOnInstance;
		public static AIActor LockedOnEnemy;
		public bool IsLockedOn;

		private Vector2 aimpoint;
		private float m_currentAngle;
		private float m_currentDistance;
		private bool HasReloaded;


		public override void Update()
		{
			base.Update();
            PlayerController player = gun.CurrentOwner as PlayerController;
            if (player != null && gun != null)
			{
                gun.DefaultModule.burstShotCount = gun.DefaultModule.numberOfShotsInClip;
                float clip = (player.stats.GetStatValue(PlayerStats.StatType.AdditionalClipCapacityMultiplier));
                float num = (int)(4);
                if (player.PlayerHasActiveSynergy("The Mighty Budget")) { num *= 2; }
                float clipsize = num * clip;

                gun.DefaultModule.numberOfShotsInClip = (int)clipsize;
                if (LockOnInstance != null)
                {

                    if (IsLockedOn == true && LockOnInstance.spriteId != LockOnGun.spriteIds[1])
                    {
                        LockOnInstance.SetSprite(LockOnGun.spriteIds[1]);
                    }
                    else if (IsLockedOn == false && LockOnInstance.spriteId != LockOnGun.spriteIds[0])
                    {
                        LockOnInstance.SetSprite(LockOnGun.spriteIds[0]);
                    }
					if (player.CurrentRoom != null)
					{
                        List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                        if (activeEnemies != null)
                        {
							if (BraveInput.GetInstanceForPlayer(player.PlayerIDX).IsKeyboardAndMouse(false)) {
								foreach (AIActor aiactor in activeEnemies) {
									if (aiactor == null) {
										LockOnInstance.gameObject.transform.position = aimpoint; //ETGModConsole.Log(6);
									}
									if (Vector2.Distance(aiactor.CenterPosition, aimpoint) < 2f && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null && IsLockedOn == false) { LockedOnEnemy = aiactor; }
								}
							}
							else if (!IsLockedOn && player != null) {
								var closestDistance = float.MaxValue;
								foreach (AIActor aiactor in activeEnemies) {
									if (aiactor == null || aiactor.healthHaver.GetMaxHealth() <= 0f || aiactor.specRigidbody == null) {
										continue;
									}
									var distance = Vector2.Distance(aiactor.CenterPosition, player.CenterPosition);
									if (distance < closestDistance) {
										closestDistance = distance;
										LockedOnEnemy = aiactor;
									}
								}
							}
						}
                    }
                    

                }
                else
                {
                    tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>(LockOnPrefab, player.transform).GetComponent<tk2dSprite>();
                    if (component != null)
                    {
                        component.PlaceAtPositionByAnchor(aimpoint, tk2dBaseSprite.Anchor.MiddleCenter);
                        component.SetSprite(LockOnGun.spriteIds[0]);
                        component.HeightOffGround = -5;
                        LockOnInstance = component;
                    }
                }
                if (LockedOnEnemy != null && (!BraveInput.GetInstanceForPlayer(player.PlayerIDX).IsKeyboardAndMouse(false) || Vector2.Distance(LockedOnEnemy.CenterPosition, aimpoint) < 2f) && LockedOnEnemy.healthHaver.GetMaxHealth() > 0f && LockedOnEnemy != null && LockedOnEnemy.specRigidbody != null && player != null)
                {
                    LockOnInstance.transform.position = LockedOnEnemy.sprite.WorldCenter - new Vector2(0.625f, 0.625f);
                }
                else if (IsLockedOn == true && LockedOnEnemy != null)
                {
                    LockOnInstance.transform.position = LockedOnEnemy.sprite.WorldCenter - new Vector2(0.625f, 0.625f);
                }
                else
                {
                    LockOnInstance.transform.position = aimpoint;
                    IsLockedOn = false;

                }
                if (player != null)
                {
					if (GameManager.Instance.IsLoadingLevel == false)
					{
                        if (BraveInput.GetInstanceForPlayer(player.PlayerIDX).IsKeyboardAndMouse(false))
                        {
                            aimpoint = player.unadjustedAimPoint.XY();
                            if (LockOnInstance != null)
                            {
                                aimpoint = aimpoint - LockOnInstance.GetBounds().extents.XY();
                            }
                        }
                        else
                        {
							aimpoint = player.CenterPosition - new Vector2(0.625f, 0.625f);
						}
                    }
                }
                else
                {
                    aimpoint = new Vector2(0, 0);
                }


                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
			else 
            {
				if (LockOnInstance!=null){Destroy(LockOnInstance); LockedOnEnemy = null;}
			}
		}

		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
				AkSoundEngine.PostEvent("Play_WPN_plasmacell_reload_01", gameObject);
			}
			base.OnReloadPressed(player, gun, bSOMETHING);
			if (gun.ClipCapacity == gun.ClipShotsRemaining || gun.CurrentAmmo == gun.ClipShotsRemaining)
			{
				if (IsLockedOn != true && LockedOnEnemy != null)
				{
					AkSoundEngine.PostEvent("Play_OBJ_supplydrop_activate_01", gameObject);
					IsLockedOn = true;
				}
				else
				{
					AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", gameObject);
					LockedOnEnemy = null;
					IsLockedOn = false;
				}
			}
		}
		public override void OnPickup(PlayerController player)
		{
			player.GunChanged += this.OnGunChanged;
			base.OnPickup(player);
			if (LockOnInstance != null) { Destroy(LockOnInstance); LockedOnEnemy = null; }
		}

		public override void OnPostDrop(PlayerController player)
		{
			player.GunChanged -= this.OnGunChanged;
			base.OnPostDrop(player);
			if (LockOnInstance != null) { Destroy(LockOnInstance); LockedOnEnemy = null; }
		}
		private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
		{
			if (this.gun && this.gun.CurrentOwner)
			{
				if (newGun != this.gun)
				{
					if (LockOnInstance != null) { Destroy(LockOnInstance); LockedOnEnemy = null; }

				}
			}
		}
		*/
    }

}