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
			Gun gun = ETGMod.Databases.Items.NewGun("Capacitor", "capacitornew");
			Game.Items.Rename("outdated_gun_mods:capacitor", "psog:capacitor");
			gun.gameObject.AddComponent<Capactior>();
			gun.SetShortDescription("Overload");
			gun.SetLongDescription("A large battery pack given the ability to shoot. Hold fire to reroute charge into your held active item!\n\nStill, very, very dangerous though.");
            
            GunExt.SetupSprite(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "capacitornew_idle_001", 11);
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.shootAnimation = "capacitornew_fire";
            gun.idleAnimation = "capacitornew_idle";
            gun.reloadAnimation = "capacitornew_reload";
            gun.chargeAnimation = "capacitornew_charge";

            gun.carryPixelOffset += new IntVector2(-2, -2);

            //GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(390) as Gun, true, false);			
            //GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(13) as Gun, true, false);

            for (int i = 0; i < 3; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(13) as Gun, true, true);
            }

            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>();
			int p = 1;
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.ammoCost = p == 1 ? 1 : 0;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.Charged;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.cooldownTime = 0.4f;
                projectileModule.angleVariance = p == 1 ? 3 : 13;
                projectileModule.numberOfShotsInClip = 30;

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                projectile.baseData.damage = p == 1 ? 11 : 2.5f;
                projectile.baseData.speed = p == 1 ? 25 : 19f;

                projectile.AdditionalScaleMultiplier = p == 1 ? 1.3f : 0.9f;
                projectile.shouldRotate = true;
                projectile.baseData.range = p == 1 ? 25 : 20;
                if (p > 1)
                {
                    PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
                    spook.penetration = 2;
                    spook.penetratesBreakables = true;
                }

                projectileModule.projectiles[0] = projectile;
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                if (projectileModule != gun.DefaultModule)
                {
                    projectileModule.ammoCost = 0;
                }

                ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
                {
                    Projectile = projectile,
                    ChargeTime = 0f
                };
                ProjectileModule.ChargeProjectile item3 = new ProjectileModule.ChargeProjectile
                {
                    Projectile = projectile,
                    AdditionalWwiseEvent = "Play_BOSS_lichC_zap_01",
                    ChargeTime = 0.5f,
					VfxPool = (PickupObjectDatabase.GetById(390) as Gun).DefaultModule.chargeProjectiles[1].VfxPool,
                    UsedProperties = ProjectileModule.ChargeProjectileProperties.vfx | ProjectileModule.ChargeProjectileProperties.additionalWwiseEvent
                };
                projectileModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>() { item2, item3 };
				p++;
            }


            gun.SetBaseMaxAmmo(300);
            gun.quality = PickupObject.ItemQuality.B;
            gun.PreventNormalFireAudio = true;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(156) as Gun).gunSwitchGroup;
            gun.reloadTime = 1.2f;


            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = (PickupObjectDatabase.GetById(13) as Gun).DefaultModule.customAmmoType;//CustomClipAmmoTypeToolbox.AddCustomAmmoType("Battery", "Planetside/Resources/GunClips/Capacitor/capacitirfull", "Planetside/Resources/GunClips/Capacitor/capacitirempty");
            gun.gunClass = GunClass.CHARGE;


            gun.encounterTrackable.EncounterGuid = "https://www.youtube.com/watch?v=_VF7G8T1Ajs";
			
			
			ETGMod.Databases.Items.Add(gun, false, "ANY");

            var Collection = StaticSpriteDefinitions.Oddments_Sheet_Data;
            var ChargeUp = ItemBuilder.AddSpriteToObjectAssetbundle("Charge Up", Collection.GetSpriteIdByName("chargeupnonsyn"), Collection);
            FakePrefab.MarkAsFakePrefab(ChargeUp);
            UnityEngine.Object.DontDestroyOnLoad(ChargeUp);
            Capactior.ChargeUpPrefab = ChargeUp;
            var ChargeUpSynergy = ItemBuilder.AddSpriteToObjectAssetbundle("Charge Up Synergy", Collection.GetSpriteIdByName("chargeupsyn"), Collection);
            FakePrefab.MarkAsFakePrefab(ChargeUpSynergy);
            UnityEngine.Object.DontDestroyOnLoad(ChargeUpSynergy);
            Capactior.ChargeUpSynergyPrefab = ChargeUpSynergy;

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
        public static GameObject ChargeUpSynergyPrefab;

		public GameObject ChargeUpVFX;
		private FieldInfo remainingTimeCooldown = typeof(PlayerItem).GetField("remainingTimeCooldown", BindingFlags.Instance | BindingFlags.NonPublic);
		private FieldInfo remainingDamageCooldown = typeof(PlayerItem).GetField("remainingDamageCooldown", BindingFlags.Instance | BindingFlags.NonPublic);
		private bool HasReloaded;

		
		

		public override void Update()
		{
			if (gun.CurrentOwner)
			{
				if (gun.CurrentOwner != null)
				{
					PlayerController player = gun.CurrentOwner as PlayerController;
					if (player != null)
					{
                        if (player.CurrentItem != null)
                        {
                            if (player.CurrentItem.IsOnCooldown == true)
                            {
                                if (gun.IsCharging && gun.CurrentAmmo > 0)
                                {
                                    TickRate -= BraveTime.DeltaTime * player.stats.GetStatValue(PlayerStats.StatType.ChargeAmountMultiplier);
                                    if (TickRate < ActivationTick)
                                    {
                                        gun.CurrentAmmo--;
                                        TickRate = 1;
                                        ActivationTick = Mathf.Min(ActivationTick + 0.1f, 0.95f);
                                        float noom = (float)this.remainingTimeCooldown.GetValue(player.CurrentItem);
                                        float eee = (float)this.remainingDamageCooldown.GetValue(player.CurrentItem);
                                        if (eee > 0f || eee > 0f)
                                        {
                                            this.remainingDamageCooldown.SetValue(player.CurrentItem, eee - Mathf.Max(player.PlayerHasActiveSynergy("Back For Seconds") ? 20 : 15f, (player.CurrentItem.damageCooldown * 0.01f)));
                                        }
                                        if (noom > 0f || noom > 0f)
                                        {
                                            this.remainingTimeCooldown.SetValue(player.CurrentItem, noom - Mathf.Max(player.PlayerHasActiveSynergy("Back For Seconds") ? 20 : 15f, (player.CurrentItem.timeCooldown * 0.01f)));
                                        }
                                    }
                                }
                                else
                                {
                                    ActivationTick = 0;
                                    TickRate = player.PlayerHasActiveSynergy("Back For Seconds") ? cachedTickRate - 0.2f : cachedTickRate;
                                }
                            }             
                        }
					}
				}
			}
        }
        private float ActivationTick = 0f;
        private float TickRate = 0.5f;
        private float cachedTickRate = 0.5f;


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