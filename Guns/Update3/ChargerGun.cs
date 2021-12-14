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


using UnityEngine.Serialization;

namespace Planetside
{
	public class ChargerGun : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Protract-non", "chargergun");
			Game.Items.Rename("outdated_gun_mods:protract-non", "psog:protract-non");
			gun.gameObject.AddComponent<ChargerGun>();
			gun.SetShortDescription("Great For Measuring Angles!");
			gun.SetLongDescription("Fires more powerful shots, less accurately the more its charged up.\n\nA gun designed to be the complete polar opposite of a standard railgun, and as a form of crowd control in the war-torn sectors of the Hegemony.");
			gun.SetupSprite(null, "chargergun_idle_001", 11);
			gun.PreventNormalFireAudio = true;
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 4);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 9);
			GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 6);


			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_BOSS_RatMech_Cannon_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;


			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_WPN_frostgiant_reload_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;

			for (int i = 0; i < 20; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(88) as Gun, true, false);
			}
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.Charged;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = 0.1f;
				projectileModule.angleVariance = 0;
				projectileModule.numberOfShotsInClip = 1;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 4f;
				projectile.AdditionalScaleMultiplier = 2f;
				projectile.shouldRotate = true;
				projectile.baseData.range = 1000f;
				projectile.gameObject.AddComponent<ChargerGunProjectile>();
				projectile.SetProjectileSpriteRight("chargergun_projectile_001", 7, 3, false, tk2dBaseSprite.Anchor.MiddleCenter, 7, 3);

				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}
				projectile.transform.parent = gun.barrelOffset;

				ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
				{
					Projectile = projectile,
					ChargeTime = 0f
				};
				gun.DefaultModule.chargeProjectiles.Add(item2);


			}
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 5;
			gun.gunSwitchGroup = "Railgun";
			gun.gunClass = GunClass.CHARGE;


			gun.barrelOffset.transform.localPosition = new Vector3(1.875f, 0.5f, 0f);
			gun.reloadTime = 1.1f;
			gun.SetBaseMaxAmmo(45);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.A;
			gun.encounterTrackable.EncounterGuid = "It's just a protractor gun";
			ETGMod.Databases.Items.Add(gun, null, "ANY");


			gun.sprite.usesOverrideMaterial = true;

			Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.SetColor("_EmissiveColor", new Color32(255, 224, 163, 255));
			mat.SetFloat("_EmissiveColorPower", 1.55f);
			mat.SetFloat("_EmissivePower", 100);
			mat.SetFloat("_EmissiveThresholdSensitivity", 0.2f);
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

			AssetBundle bundle = ResourceManager.LoadAssetBundle("brave_resources_001");
			LaserReticle = bundle.LoadAsset("assets/resourcesbundle/global vfx/vfx_lasersight.prefab") as GameObject;
			bundle = null;

			/*
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 11;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "start";

			//GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/VFX/DeathMark/markedfordeathvfx_001", gun.gameObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = gun.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = ETGMod.Databases.Items.WeaponCollection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 9; i++)
			{
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/VFX/DeathMark/markedfordeathvfx_00{i}", gun.gameObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = gun.GetComponent<tk2dSprite>().spriteId;
				frame.spriteCollection = ETGMod.Databases.Items.WeaponCollection;
			animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			gun.GetComponent<tk2dSpriteAnimator>().Library = gun.GetComponent<tk2dSpriteAnimator>().Library;

			Array.Resize<tk2dSpriteAnimationClip>(ref gun.spriteAnimator.Library.clips, gun.spriteAnimator.Library.clips.Length + 1);
			gun.spriteAnimator.Library.clips[gun.spriteAnimator.Library.clips.Length + 1] = animationClip;
			//gun.GetComponent<tk2dSpriteAnimator>().DefaultClipId = gun.GetComponent<tk2dSpriteAnimator>().GetClipIdByName("idle");
			gun.GetComponent<tk2dSpriteAnimator>().playAutomatically = true;
			*/

		}


		private bool HasReloaded;
		private static List<GameObject> Chargerreticles = new List<GameObject>();
		private bool VFXActive;
		private float elapsed;
		//private static AssetBundle bundle = ResourceManager.LoadAssetBundle("brave_resources_001");
		private static GameObject LaserReticle;


		public void CleanupReticles()
		{
			for (int i = 0; i < ChargerGun.Chargerreticles.Count; i++)
			{
				SpawnManager.Despawn(ChargerGun.Chargerreticles[i]);
				Destroy(ChargerGun.Chargerreticles[i]);
			}
			ChargerGun.Chargerreticles.Clear();
		}

		protected override void Update()
		{
			base.Update();
			if (gun.CurrentOwner != null)
			{
				PlayerController player = gun.CurrentOwner as PlayerController;
				if (player != null)
                {
					if (gun.IsCharging)
					{
						float charge = player.stats.GetStatValue(PlayerStats.StatType.ChargeAmountMultiplier);
						if (this.elapsed <= 2)
                        {
							this.elapsed += (BraveTime.DeltaTime / charge);
						}
						if (VFXActive != true)
                        {
							VFXActive = true;							
							for (int i = 0; i < 7; i++)
							{
								float num2 = 16f;
								Vector2 zero = Vector2.zero;				
								if (BraveMathCollege.LineSegmentRectangleIntersection(this.gun.barrelOffset.transform.position, this.gun.barrelOffset.transform.position + BraveMathCollege.DegreesToVector(player.CurrentGun.CurrentAngle, 60f).ToVector3ZisY(-0.25f), new Vector2(-40, -40), new Vector2(40, 40), ref zero))
								{
									num2 = (zero - new Vector2(this.gun.barrelOffset.transform.position.x, this.gun.barrelOffset.transform.position.y)).magnitude;
								}
								GameObject gameObject = SpawnManager.SpawnVFX(ChargerGun.LaserReticle, false);
								tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
								component2.transform.position = new Vector3(this.gun.barrelOffset.transform.position.x, this.gun.barrelOffset.transform.position.y, 99999);
								component2.transform.localRotation = Quaternion.Euler(0f, 0f, player.CurrentGun.CurrentAngle);
								component2.dimensions = new Vector2((num2) * 2f, 1f);
								component2.UpdateZDepth();
								component2.HeightOffGround = -2;
								component2.renderer.enabled = true;
								component2.transform.position.WithZ(component2.transform.position.z + 99999);
								ChargerGun.Chargerreticles.Add(gameObject);
							}
						}
						float Accuracy = player.stats.GetStatValue(PlayerStats.StatType.Accuracy) * 15;
						for (int i = -3; i < 4; i++)
                        {
							GameObject obj = Chargerreticles[i + 3];
							if (obj != null)
                            {
								tk2dTiledSprite component2 = obj.GetComponent<tk2dTiledSprite>();

								if (i == -3 | i == 0 | i == 3)	
								{
									component2.dimensions = new Vector2((16) * 2f, 1f);
								}
								else
								{
									component2.dimensions = new Vector2((10) * 2f, 1f);
								}
								component2.usesOverrideMaterial = true;
								component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
								component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");

								component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
								component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
								Color laser = new Color(1f, 0.9f, 0.6f, 1f);
								component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
								component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);

								float AddaAngle = (elapsed * (Accuracy * i));
								float ix = obj.transform.localRotation.eulerAngles.x + player.CurrentGun.barrelOffset.transform.localRotation.eulerAngles.x;
								float wai = obj.transform.localRotation.eulerAngles.y + player.CurrentGun.barrelOffset.transform.localRotation.eulerAngles.y;
								float zee = obj.transform.localRotation.z + player.CurrentGun.transform.eulerAngles.z;
								obj.transform.localRotation = Quaternion.Euler(ix, wai, zee + AddaAngle + 3600);
								obj.transform.position = this.gun.barrelOffset.transform.position;
								component2.transform.position.WithZ(component2.transform.position.z + 99999);
								component2.UpdateZDepth();
								component2.HeightOffGround = -2;
								component2.renderer.enabled = true;
							}
						}
						foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
						{
							projectileModule.angleVariance = elapsed * (Accuracy*3);
							Projectile proj = projectileModule.GetCurrentProjectile();
							float dmg = player.stats.GetStatValue(PlayerStats.StatType.Damage);
							float spd = player.stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
							proj.baseData.damage = ((elapsed * 8) + 4)*dmg;
							proj.baseData.speed = UnityEngine.Random.Range(22f, 28f) * spd; 
						}
					}
					else
					{
						elapsed = 0;
						VFXActive = false;
						CleanupReticles();
					}
				}
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}
			}
			else
            {
				elapsed = 0;
				CleanupReticles();
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
		protected override void OnPickup(PlayerController player)
		{
			player.GunChanged += this.OnGunChanged;
			base.OnPickup(player);
			CleanupReticles();
		}

		protected override void OnPostDrop(PlayerController player)
		{
			player.GunChanged -= this.OnGunChanged;
			base.OnPostDrop(player);
			CleanupReticles();
		}
		private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
		{
			if (this.gun && this.gun.CurrentOwner)
			{
				PlayerController player = this.gun.CurrentOwner as PlayerController;
				if (newGun != this.gun)
				{
					CleanupReticles();
				}
				//else
                //{
					//gun.GetComponent<tk2dSpriteAnimator>().Play("start");
				//}
			}
		}
	}
}


