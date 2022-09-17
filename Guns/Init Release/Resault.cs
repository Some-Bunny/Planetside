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
	public class Resault : AdvancedGunBehaviourMultiActive
	{
		public static void ResetMaxAmmo()
		{
			Gun gun = (PickupObjectDatabase.GetById(Resault.ResaultID) as Gun);
			gun.SetBaseMaxAmmo(200);
		}
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Re-Sault", "resault");
			Game.Items.Rename("outdated_gun_mods:resault", "psog:resault");
			gun.gameObject.AddComponent<Resault>();

			GunExt.SetShortDescription(gun, "Reduce, Reuse, Reload");
			GunExt.SetLongDescription(gun, "An automatic machine-gun that dispenses ammo capacity when fired, restores a portion of it on killing an enemy. Gungeonoligists still specualte *why* this gun dispenses its ammo capacity. Is it to reduce weight? To increase damage output? No-one really knows.");
			GunExt.SetupSprite(gun, null, "resault_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 36);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 7);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 4);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(96) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(2) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 1.8f;
			gun.DefaultModule.cooldownTime = .05f;
			gun.DefaultModule.numberOfShotsInClip = 50;
			gun.SetBaseMaxAmmo(200);
			gun.quality = PickupObject.ItemQuality.B;
			gun.DefaultModule.angleVariance = 4f;
			gun.DefaultModule.burstShotCount = 1;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 4.5f;
			projectile.baseData.speed *= 1f;
			projectile.AdditionalScaleMultiplier *= 1.1f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			//projectile.ignoreDamageCaps = true;
			gun.barrelOffset.transform.localPosition = new Vector3(2.375f, 0.75f, 0f);
			gun.carryPixelOffset = new IntVector2((int)2f, (int)-1.5f);
			gun.encounterTrackable.EncounterGuid = "IUJFDHUBJKHHGUJGHJUKGJHJGHJGJGJGJGJGHJGJHHDFBSFJSHFGJSDHKJHJJJJJJJJJJJJJJ";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			gun.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);
			gun.gunClass = GunClass.RIFLE;

			//+10
			Resault.Plus10AMMOVFXPrefab = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Resault/plus10ammo", null, false);
			Resault.Plus10AMMOVFXPrefab.name = "plus10ammo";
			UnityEngine.Object.DontDestroyOnLoad(Resault.Plus10AMMOVFXPrefab);
			FakePrefab.MarkAsFakePrefab(Resault.Plus10AMMOVFXPrefab);
			Resault.Plus10AMMOVFXPrefab.SetActive(false);
			//+20
			Resault.Plus20AMMOVFXPrefab = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Resault/plus20ammo", null, false);
			Resault.Plus20AMMOVFXPrefab.name = "plus20ammo";
            UnityEngine.Object.DontDestroyOnLoad(Resault.Plus20AMMOVFXPrefab);
			FakePrefab.MarkAsFakePrefab(Resault.Plus20AMMOVFXPrefab);
			Resault.Plus20AMMOVFXPrefab.SetActive(false);
			//+4
			Resault.Plus4AMMOVFXPrefab = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Resault/plus4ammo", null, false);
			Resault.Plus4AMMOVFXPrefab.name = "plus4ammo";
			UnityEngine.Object.DontDestroyOnLoad(Resault.Plus4AMMOVFXPrefab);
			FakePrefab.MarkAsFakePrefab(Resault.Plus4AMMOVFXPrefab);
			Resault.Plus4AMMOVFXPrefab.SetActive(false);

			//+16
			Resault.Plus16AMMOVFXPrefab = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Resault/plus16ammosyn", null, false);
			Resault.Plus16AMMOVFXPrefab.name = "plus16ammosyn";
			UnityEngine.Object.DontDestroyOnLoad(Resault.Plus16AMMOVFXPrefab);
			FakePrefab.MarkAsFakePrefab(Resault.Plus16AMMOVFXPrefab);
			Resault.Plus16AMMOVFXPrefab.SetActive(false);
			//+40
			Resault.Plus40AMMOVFXPrefab = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Resault/plus40ammosyn", null, false);
			Resault.Plus40AMMOVFXPrefab.name = "plus40ammosyn";
			UnityEngine.Object.DontDestroyOnLoad(Resault.Plus40AMMOVFXPrefab);
			FakePrefab.MarkAsFakePrefab(Resault.Plus40AMMOVFXPrefab);
			Resault.Plus40AMMOVFXPrefab.SetActive(false);
			//+80
			Resault.Plus80AMMOVFXPrefab = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Resault/plus80ammosyn", null, false);
			Resault.Plus80AMMOVFXPrefab.name = "plus80ammosyn";
			UnityEngine.Object.DontDestroyOnLoad(Resault.Plus80AMMOVFXPrefab);
			FakePrefab.MarkAsFakePrefab(Resault.Plus80AMMOVFXPrefab);
			Resault.Plus80AMMOVFXPrefab.SetActive(false);

			Resault.ResaultID = gun.PickupObjectId;
			List<string> yah = new List<string>
			{
				"psog:resault",
				"ancient_heros_bandana"
			};
			CustomSynergies.Add("Infinite Ammo?", yah, null, true);
			List<string> aaa = new List<string>
			{
				"psog:resault",
			};
			List<string> aw = new List<string>
			{
				"ammo_synthesizer",
				"ammo_belt",
				"turkey",
				"utility_belt"
			};
			CustomSynergies.Add("Recycling", aaa, aw, true);

			ItemIDs.AddToList(gun.PickupObjectId);

			new Hook(typeof(AmmoPickup).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(Resault).GetMethod("ammoPickupHookMethod"));

        }

		public static int ResaultID;
		private bool HasReloaded;

		public static void ammoPickupHookMethod(Action<AmmoPickup, PlayerController> orig, AmmoPickup self, PlayerController player)
		{
			bool canAmmo = false;
			orig(self, player);
			if (player.HasPickupID(ResaultID))
			{
				for (int i = 0; i < player.inventory.AllGuns.Count; i++)
				{
					Gun g = player.inventory.AllGuns[i];
                    if (g.PickupObjectId == ResaultID)
					{
                        if (canAmmo)
                        {
                            if (self.mode == AmmoPickup.AmmoPickupMode.FULL_AMMO && player.CurrentGun != null && player.CurrentGun == g)
                            {
                                DoVFX(g, 20, Resault.Plus20AMMOVFXPrefab, Resault.Plus80AMMOVFXPrefab, player);

                            }
                            else if (self.mode == AmmoPickup.AmmoPickupMode.SPREAD_AMMO && player.CurrentGun != null)
                            {
                                if (player.CurrentGun == g)
                                {
                                    DoVFX(g, 10, Resault.Plus10AMMOVFXPrefab, Resault.Plus40AMMOVFXPrefab, player);
                                }
                                else
                                {
                                    DoVFX(g, 4, Resault.Plus4AMMOVFXPrefab, Resault.Plus16AMMOVFXPrefab, player);
                                }
                            }
                            canAmmo = false;
                        }
                        else
                        {
                            canAmmo = true;
                        }
                    }
				}				
			}
		}

		public static void DoVFX(Gun g, int ammoAmount, GameObject VFX, GameObject SynergyVFX, PlayerController player)
		{
            AkSoundEngine.PostEvent("Play_OBJ_spears_clank_01", g.gameObject);
            player.CurrentGun.SetBaseMaxAmmo(player.CurrentGun.GetBaseMaxAmmo() + ammoAmount);
            player.BloopItemAboveHead(player.PlayerHasActiveSynergy("Infinite Ammo?") == false ? VFX.GetComponent<tk2dBaseSprite>() : SynergyVFX.GetComponent<tk2dBaseSprite>(), "");
        }

		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
		}
		public override void OnPostFired(PlayerController player, Gun bruhgun)
		{
            int currentAmmo = bruhgun.CurrentAmmo;
            if (currentAmmo >= 25)
            {
                player.CurrentGun.SetBaseMaxAmmo(player.CurrentGun.GetBaseMaxAmmo() - 1);
            }
		}
		public override void PostProcessProjectile(Projectile projectile)
		{
			PlayerController player = this.gun.CurrentOwner as PlayerController;
			if (player.PlayerHasActiveSynergy("Infinite Ammo?"))
            {
				projectile.baseData.damage *= 1.25f;
            }
			projectile.OnWillKillEnemy = (Action<Projectile, SpeculativeRigidbody>)Delegate.Combine(projectile.OnWillKillEnemy, new Action<Projectile, SpeculativeRigidbody>(this.OnKill));
		}
		private void OnKill(Projectile arg1, SpeculativeRigidbody arg2)
		{
			PlayerController player = arg1.Owner as PlayerController;

			bool flag = !arg2.aiActor.healthHaver.IsDead && arg2.aiActor != null && arg2.aiActor.GetComponent<MarkResault>() == null;
			if (flag)
			{
				arg2.aiActor.gameObject.AddComponent<MarkResault>();
				int AmmoRegained = 10;
				int ae = 5;
				AkSoundEngine.PostEvent("Play_OBJ_spears_clank_01", base.gameObject);
				GameObject original = Resault.Plus10AMMOVFXPrefab;
				if (player.PlayerHasActiveSynergy("Infinite Ammo?"))
                {
					original = Resault.Plus40AMMOVFXPrefab;
					ae *= 4;
				}

				tk2dSprite ahfuck = original.GetComponent<tk2dSprite>();


				this.gun.SetBaseMaxAmmo(gun.GetBaseMaxAmmo() + AmmoRegained);
                this.gun.ammo += ae;

                player.BloopItemAboveHead(ahfuck, "");

                if (player.PlayerHasActiveSynergy("Recycling"))
                {
					this.gun.ammo += ae;
				}
			}
		}
		public override void MidGameDeserialize(List<object> data, ref int i)
		{
			base.MidGameDeserialize(data, ref i);
			//Ammo = (int)data[i];
			i += 1;
		}


		private static GameObject Plus10AMMOVFXPrefab;
		private static GameObject Plus20AMMOVFXPrefab;
		private static GameObject Plus4AMMOVFXPrefab;

		private static GameObject Plus16AMMOVFXPrefab;
		private static GameObject Plus40AMMOVFXPrefab;
		private static GameObject Plus80AMMOVFXPrefab;

	}
	public class MarkResault : BraveBehaviour{}
}

