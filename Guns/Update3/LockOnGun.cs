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
			gun.SetLongDescription("Directs micro-rockets towards the targetting reticle. Press Reload on a full clip to lock on to the currently targeted enemy.\n\nA shoulder-mounted rocket launcher, fitted with way too much computer circuitry.\nMakes up for it with being good at generating half-decent jokes and a perfect enemy lock-on and tracking system.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "lockongun_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.reloadAnimation = "lockongun_reload";
            gun.idleAnimation = "lockongun_idle";
            gun.shootAnimation = "lockongun_fire";


            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_wpn_voidcannon_shot_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;



			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(345) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(593) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2.2f;
			gun.DefaultModule.cooldownTime = .7f;
			gun.DefaultModule.numberOfShotsInClip = 4;
			gun.SetBaseMaxAmmo(120);
			gun.quality = PickupObject.ItemQuality.A;
			gun.DefaultModule.burstCooldownTime = 0.0833f;

			gun.DefaultModule.angleVariance = 13f;
			gun.DefaultModule.burstShotCount = 4;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 10f;
			projectile.baseData.speed *= 0.9f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.gameObject.AddComponent<LockOnGunProjectile>();
            Alexandria.Assetbundle.ProjectileBuilders.SetProjectileCollisionRight(projectile, "lockonprojectile", StaticSpriteDefinitions.Projectile_Sheet_Data, 9, 7, false, tk2dBaseSprite.Anchor.MiddleCenter);


            ExplosiveModifier explosiveModifier = projectile.gameObject.AddComponent<ExplosiveModifier>();
			explosiveModifier.doExplosion = true;
			explosiveModifier.explosionData = StaticExplosionDatas.explosiveRoundsExplosion;

			gun.gunClass = GunClass.EXPLOSIVE;

			PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
			spook.penetration = 0;
			spook.penetratesBreakables = true;
			gun.encounterTrackable.EncounterGuid = "haha funny big shot";
			ETGMod.Databases.Items.Add(gun, false, "ANY");

			GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/LockOnVFX/lockonvfx1", null, true);
			gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(gameObject);
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			GameObject gameObject2 = new GameObject("Lock On Gun VFX");
			tk2dSprite tk2dSprite = gameObject2.AddComponent<tk2dSprite>();
			tk2dSprite.SetSprite(gameObject.GetComponent<tk2dBaseSprite>().Collection, gameObject.GetComponent<tk2dBaseSprite>().spriteId);

			LockOnGun.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/LockOnVFX/lockonvfx1", tk2dSprite.Collection));
			LockOnGun.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/LockOnVFX/lockonvfx2", tk2dSprite.Collection));




			tk2dSprite.usesOverrideMaterial = true;
			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
			tk2dSprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
			tk2dSprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
			tk2dSprite.renderer.material.SetFloat("_EmissivePower", 30);
			tk2dSprite.renderer.material.SetFloat("_EmissiveColorPower", 2);

			LockOnGun.spriteIds.Add(tk2dSprite.spriteId);
			gameObject2.SetActive(false);

			tk2dSprite.SetSprite(LockOnGun.spriteIds[0]); //Unlocked
			tk2dSprite.SetSprite(LockOnGun.spriteIds[1]); //Locked

			FakePrefab.MarkAsFakePrefab(gameObject2);
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
			LockOnGun.LockOnPrefab = gameObject2;



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

			List<string> AAA1= new List<string>
			{
				"homing_bullets",
				"remote_bullets"
			};
			CustomSynergies.Add("No Virus Included", AAA, AAA1, false);

			List<string> AAA2 = new List<string>
			{
				"stinger",
				"rpg",
				"yari_launcher"
			};
			CustomSynergies.Add("The Mighty Budget", AAA, AAA2, false);


			List<string> yes = new List<string>
			{
				"psog:t4gtr",
				"rc_rocket"
			};
			CustomSynergies.Add("Double Trouble!", yes, null, false);


			LockOnGun.LockOnGunID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
            gun.AddToSubShop(ItemAPI.ItemBuilder.ShopType.Trorc, 1);

        }
        public static int LockOnGunID;

		public static GameObject LockOnPrefab;
		public static List<int> spriteIds = new List<int>();
		public static tk2dBaseSprite LockOnInstance;
		public static AIActor LockedOnEnemy;
		public bool IsLockedOn;

		private Vector2 aimpoint;
		//private float maxDistance = 15;
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
	}

}