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

namespace Planetside
{
	public class Capactior : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Capacitor", "capacitorpsog");
			Game.Items.Rename("outdated_gun_mods:capacitor", "psog:capacitor");
			gun.gameObject.AddComponent<Capactior>();
			gun.SetShortDescription("Overload");
			gun.SetLongDescription("A large battery pack given the ability to shoot. Can be rerouted into carried items to charge them up!\n\nStill, very, very dangerous though.");
			GunExt.SetupSprite(gun, null, "capacitorpsog_idle_001", 11);
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_BOSS_lichC_zap_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 7);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 1);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(390) as Gun, true, false);


			//gun.gunSwitchGroup = (PickupObjectDatabase.GetById(390) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 0f;
			gun.DefaultModule.cooldownTime = .333f;
			gun.DefaultModule.numberOfShotsInClip = -1;
			gun.SetBaseMaxAmmo(3);
			gun.quality = PickupObject.ItemQuality.B;
			gun.DefaultModule.angleVariance = 11f;
			gun.DefaultModule.burstShotCount = 1;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 200f;
			projectile.baseData.speed *= 1f;
			projectile.AdditionalScaleMultiplier *= 0.75f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.PenetratesInternalWalls = true;

			/*
			tk2dTiledSprite tiledSprite = projectile.gameObject.GetComponentInChildren<tk2dTiledSprite>();
			tiledSprite.sprite.usesOverrideMaterial = true;
			Material material = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
			Material sharedMaterial = tiledSprite.sprite.renderer.sharedMaterial;
			material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
			material.SetColor("_OverrideColor", new Color(1f, 0f, 0f, 1f));
			material.SetFloat("_EmissivePower", 4);
			material.SetFloat("_EmissiveColorPower", 2f);

			tiledSprite.sprite.renderer.material = material;
			*/
			PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
			spook.penetration = 3;
			spook.penetratesBreakables = true;
			CapacitorProjectile yah = projectile.gameObject.AddComponent<CapacitorProjectile>();
			gun.gunClass = GunClass.NONE;

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Battery", "Planetside/Resources/GunClips/Capacitor/capacitirfull", "Planetside/Resources/GunClips/Capacitor/capacitirempty");

			gun.encounterTrackable.EncounterGuid = "https://www.youtube.com/watch?v=_VF7G8T1Ajs";
			ETGMod.Databases.Items.Add(gun, null, "ANY");

			GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/CapacitorVFX/chargeupnonsyn", null, true);
			gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(gameObject);
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			GameObject gameObject2 = new GameObject("CapactiorStuff");
			tk2dSprite tk2dSprite = gameObject2.AddComponent<tk2dSprite>();
			tk2dSprite.SetSprite(gameObject.GetComponent<tk2dBaseSprite>().Collection, gameObject.GetComponent<tk2dBaseSprite>().spriteId);

			Capactior.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/CapacitorVFX/chargeupnonsyn", tk2dSprite.Collection));
			Capactior.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/CapacitorVFX/chargeupsyn", tk2dSprite.Collection));

			tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
			ForgiveMePlease.spriteIds.Add(tk2dSprite.spriteId);
			gameObject2.SetActive(false);

			tk2dSprite.SetSprite(Capactior.spriteIds[0]); //Non Synergy
			tk2dSprite.SetSprite(Capactior.spriteIds[1]); //Synergy

			FakePrefab.MarkAsFakePrefab(gameObject2);
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
			Capactior.ChargeUpPrefab = gameObject2;
			List<string> AAA = new List<string>
			{
				"psog:capacitor"
			};
			List<string> aee = new List<string>
			{
				"turkey",
				"broccoli",
				"gungeon_pepper",
				"meatbun",
				"weird_egg"
			};
			CustomSynergies.Add("Back For Seconds", AAA, aee, true);

			Capactior.CapacitorID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int CapacitorID;

		public static GameObject ChargeUpPrefab;
		public static List<int> spriteIds = new List<int>();

		public override void OnPostFired(PlayerController player, Gun gun)
		{

			if (player != null)
			{
				bool ee = player.activeItems == null;
				if (!ee)
				{
					foreach (PlayerItem playerItem in player.activeItems)
					{
						bool aa = playerItem == null;
						if (!aa)
						{
							float timeCooldown = playerItem.timeCooldown;
							float damageCooldown = playerItem.damageCooldown;
							try
							{
								float noom = (float)this.remainingTimeCooldown.GetValue(playerItem);
								float eee = (float)this.remainingDamageCooldown.GetValue(playerItem);
								bool flag3 = eee <= 0f || eee <= 0f;
								if (!flag3)
								{
									GameObject original = Capactior.ChargeUpPrefab;
									tk2dSprite ahfuck = original.GetComponent<tk2dSprite>();
									if (player.PlayerHasActiveSynergy("Back For Seconds"))
									{
										if (UnityEngine.Random.value < 0.5f)
                                        {
											original.GetComponent<tk2dBaseSprite>().SetSprite(Capactior.spriteIds[1]);
											SetCharge(false);
										}
										else
                                        {
											SetCharge(true);
											original.GetComponent<tk2dBaseSprite>().SetSprite(Capactior.spriteIds[0]);
										}
									}
									else
									{
										SetCharge(true);
										original.GetComponent<tk2dBaseSprite>().SetSprite(Capactior.spriteIds[0]);
									}
									player.BloopItemAboveHead(ahfuck, "");
									this.remainingTimeCooldown.SetValue(playerItem, noom- timeCooldown);
									this.remainingDamageCooldown.SetValue(playerItem, eee-damageCooldown);
									AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Player_Charge_01", base.gameObject);
								}
								else
                                {
									SetCharge(false);
								}
							}
							catch (Exception ex)
							{
								ETGModConsole.Log(ex.Message + ": " + ex.StackTrace, false);
							}
						}
					}
				}
			}
		}
		public bool Charger;
		public GameObject ChargeUpVFX;
		private FieldInfo remainingTimeCooldown = typeof(PlayerItem).GetField("remainingTimeCooldown", BindingFlags.Instance | BindingFlags.NonPublic);
		private FieldInfo remainingDamageCooldown = typeof(PlayerItem).GetField("remainingDamageCooldown", BindingFlags.Instance | BindingFlags.NonPublic);
		private bool HasReloaded;

		public bool IsChargeUp()
        {
			return Charger;
		}
		public void SetCharge(bool b)
		{
			Charger = b;
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