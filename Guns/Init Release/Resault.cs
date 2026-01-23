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
			GunExt.SetLongDescription(gun, "An automatic machine-gun that dispenses ammo capacity when fired, restores a portion of it on killing an enemy. Gungeonoligists still specualte *why* this gun dispenses its ammo capacity. Is it to reduce weight? Some idiot thought it would be an interesting gimmick? No-one really knows.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "resault_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 2;
            //GunExt.SetupSprite(gun, null, "resault_idle_001", 8);

            gun.idleAnimation = "resault_idle";
            gun.shootAnimation = "resault_fire";
            gun.reloadAnimation = "resault_reload";


            //GunExt.SetAnimationFPS(gun, gun.shootAnimation, 36);
            //GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 7);
            //GunExt.SetAnimationFPS(gun, gun.idleAnimation, 4);
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(96) as Gun, true, false);


            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(2) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 1.8f;
			gun.DefaultModule.cooldownTime = .05f;
			gun.DefaultModule.numberOfShotsInClip = 50;
			gun.SetBaseMaxAmmo(250);
			gun.quality = PickupObject.ItemQuality.B;
			gun.DefaultModule.angleVariance = 4f;
			gun.DefaultModule.burstShotCount = 1;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 6f;
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
            var Collection = StaticSpriteDefinitions.Oddments_Sheet_Data;
            var Plus10 = ItemBuilder.AddSpriteToObjectAssetbundle("Plus 10", Collection.GetSpriteIdByName("plus10ammo"), Collection);
            FakePrefab.MarkAsFakePrefab(Plus10);
            UnityEngine.Object.DontDestroyOnLoad(Plus10);
            Resault.Plus10AMMOVFXPrefab = Plus10;


            Resault.ResaultID = gun.PickupObjectId;
			List<string> yah = new List<string>
			{
				"psog:resault",
				"ancient_heros_bandana"
			};
			CustomSynergies.Add("Infinite Ammo?", yah, null, false);
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
			CustomSynergies.Add("Recycling", aaa, aw, false);

			ItemIDs.AddToList(gun.PickupObjectId);

			new Hook(typeof(AmmoPickup).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(Resault).GetMethod("ammoPickupHookMethod"));

        }

		public static int ResaultID;
		private bool HasReloaded;

		public static void ammoPickupHookMethod(Action<AmmoPickup, PlayerController> orig, AmmoPickup self, PlayerController player)
		{
			bool canAmmo = false;
			orig(self, player);
            for (int i = 0; i < player.inventory.AllGuns.Count; i++)
            {
                Gun g = player.inventory.AllGuns[i];
                if (g.PickupObjectId == ResaultID)
                {
                    if (canAmmo)
                    {
                        if (self.mode == AmmoPickup.AmmoPickupMode.FULL_AMMO && player.CurrentGun != null && player.CurrentGun == g)
                        {
                            DoVFX(g, 20, "plus20ammo", "plus80ammosyn", player);

                        }
                        else if (self.mode == AmmoPickup.AmmoPickupMode.SPREAD_AMMO && player.CurrentGun != null)
                        {
                            if (player.CurrentGun == g)
                            {
                                DoVFX(g, 10, "plus10ammo", "plus40ammosyn", player);
                            }
                            else
                            {
                                DoVFX(g, 4, "plus10ammo", "plus16ammosyn", player);
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

		public static void DoVFX(Gun g, int ammoAmount, string VFX, string SynergyVFX, PlayerController player)
		{
            AkSoundEngine.PostEvent("Play_OBJ_spears_clank_01", g.gameObject);
            player.CurrentGun.SetBaseMaxAmmo(player.CurrentGun.GetBaseMaxAmmo() + ammoAmount);
            player.BloopItemAboveHead(Resault.Plus10AMMOVFXPrefab.GetComponent<tk2dBaseSprite>() ,player.PlayerHasActiveSynergy("Infinite Ammo?") == false ? VFX : SynergyVFX);
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
			int currentMax = player.CurrentGun.GetBaseMaxAmmo();
            if (currentMax > 25)
            {
				int AmountOfAmmoToRemove = 1;
                if (isInfAmmoSynergy == true)
                {
					AmountOfAmmoToRemove *= 4;
                }
                player.CurrentGun.SetBaseMaxAmmo(Mathf.Max(currentMax - AmountOfAmmoToRemove, 25));

            }
        }
		public override void PostProcessProjectile(Projectile projectile)
		{
			projectile.OnWillKillEnemy = (Action<Projectile, SpeculativeRigidbody>)Delegate.Combine(projectile.OnWillKillEnemy, new Action<Projectile, SpeculativeRigidbody>(this.OnKill));
		}
		private void OnKill(Projectile arg1, SpeculativeRigidbody arg2)
		{
			PlayerController player = arg1.Owner as PlayerController;

			if (arg2.aiActor != null && arg2.aiActor.GetComponent<MarkResault>() == null)
			{
				arg2.aiActor.gameObject.AddComponent<MarkResault>();
				int AmmoRegained = 10;
				int ae = isInfAmmoSynergy == true ? 20 : 5;
				AkSoundEngine.PostEvent("Play_OBJ_spears_clank_01", base.gameObject);
				GameObject original = Resault.Plus10AMMOVFXPrefab;
				
				var spr = original.GetComponent<tk2dSprite>();
				this.gun.SetBaseMaxAmmo(gun.GetBaseMaxAmmo() + AmmoRegained);
                this.gun.ammo += player.PlayerHasActiveSynergy("Recycling") ? ae * 2 : ae;
                player.BloopItemAboveHead(spr, isInfAmmoSynergy == true ? "plus40ammosyn" : "plus10ammo");

			}
		}

        public override void Update()
        {
            base.Update();
			if (gun.CurrentOwner != null && gun.CurrentOwner is PlayerController player)
			{
				var b = player.PlayerHasActiveSynergy("Infinite Ammo?");
				if (b != isInfAmmoSynergy)
				{
					isInfAmmoSynergy = b;
					if (b == true)
					{
						gun.DefaultModule.ammoCost = 4;

                    }
					else
					{
                        gun.DefaultModule.ammoCost = 1;
                    }
                }
            }
        }

		private bool isInfAmmoSynergy = false;

        private static GameObject Plus10AMMOVFXPrefab;
	}
	public class MarkResault : MonoBehaviour{}
}

