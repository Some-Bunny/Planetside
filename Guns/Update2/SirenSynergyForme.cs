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
	public class SirenSynergyForme : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Siren Can Roll", "fishcanroll");
			Game.Items.Rename("outdated_gun_mods:siren_can_roll", "psog:siren_can_roll");
			gun.gameObject.AddComponent<SirenSynergyForme>();
			gun.SetShortDescription(":)");
			gun.SetLongDescription("flashyn");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "fishcanroll_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.reloadAnimation = "sirencanroll_reload";
            gun.idleAnimation = "sirencanroll_idle";
            gun.shootAnimation = "sirencanroll_fire";


            for (int i = 0; i < 3; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(404) as Gun, true, false);

			}
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = 0.3f;
				projectileModule.angleVariance = 9f;
				projectileModule.numberOfShotsInClip = 6;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(404) as Gun).DefaultModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 13f;
				projectile.AdditionalScaleMultiplier = 1f;
				projectile.baseData.force *= 1;
				projectile.baseData.speed *= 1;
				
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}
				projectile.transform.parent = gun.barrelOffset;

                int Length = 4;
                Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(projectile, "fibsh", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "fibsh",
                 AnimateBullet.ConstructListOfSameValues<IntVector2>(new IntVector2(17, 17), Length),
                AnimateBullet.ConstructListOfSameValues(false, Length),
                AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, Length),
                AnimateBullet.ConstructListOfSameValues(true, Length),
                AnimateBullet.ConstructListOfSameValues(false, Length),
                AnimateBullet.ConstructListOfSameValues<Vector3?>(null, Length),
                AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(17, 17), Length),
                AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(0, 0), Length),
                AnimateBullet.ConstructListOfSameValues<Projectile>(null, Length));

                /*
				projectile.AnimateProjectile(new List<string> {
				"fishspin_projectile_001",
				"fishspin_projectile_002",
				"fishspin_projectile_003",
				"fishspin_projectile_004"
			}, 13, true, new List<IntVector2> {
				new IntVector2(17, 17), //1
                new IntVector2(17, 17), //2            All frames are 13x16 except select ones that are 11-14
                new IntVector2(17, 17), //3
                new IntVector2(17, 17),
			}, AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 7), AnimateBullet.ConstructListOfSameValues(true, 7), AnimateBullet.ConstructListOfSameValues(false, 7),
			AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 7));

				projectile.SetProjectileSpriteRight("fishspin_projectile_001", 17, 17, false, tk2dBaseSprite.Anchor.MiddleCenter, 15, 15);
				*/
            }
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(404) as Gun).gunSwitchGroup;
			gun.barrelOffset.transform.localPosition += new Vector3(0.75f, 0.5f, 0f);
			gun.reloadTime = 1.5f;
			gun.SetBaseMaxAmmo(200);
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects;
			gun.carryPixelOffset += new IntVector2((int)0.5f, (int)-1.125);

			gun.quality = PickupObject.ItemQuality.EXCLUDED;
			gun.encounterTrackable.EncounterGuid = "foosh smile :D";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			SirenSynergyForme.smileid = gun.PickupObjectId;
		}
		public static int smileid; 
		public override void PostProcessProjectile(Projectile projectile)
		{

		}
		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			
		}
		public override void OnPickup(PlayerController player)
		{
			base.OnPickup(player);
			player.GunChanged += this.OnGunChanged;
		}

		public override void OnPostDrop(PlayerController player)
		{
			player.GunChanged -= this.OnGunChanged;
			base.OnPostDrop(player);
		}

		private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
		{
			if (this.gun && this.gun.CurrentOwner)
			{
				PlayerController player = this.gun.CurrentOwner as PlayerController;
				if (newGun == this.gun)
				{
					player.ImmuneToPits.SetOverride("SirenSyn", true, null);
					player.ImmuneToAllEffects = true;
				}
				else 
				{
					player.ImmuneToPits.SetOverride("SirenSyn", false, null);
					player.ImmuneToAllEffects = false;
				}
			}

		}
		public override void Update()
		{
			base.Update();
		}
	}
}