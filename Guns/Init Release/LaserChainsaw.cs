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
using SaveAPI;

/*
namespace Planetside
{
	public class LaserChainsaw : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Laser Chainsaw", "laserchainsaw");
			Game.Items.Rename("outdated_gun_mods:laser_chainsaw", "psog:laser_chainsaw");
			gun.gameObject.AddComponent<LaserChainsaw>();
			gun.SetShortDescription("KILL KILL KILL");
			gun.SetLongDescription("SHRED EVERYTHING IN SIGHT.\n\nLEAVE NO WITNESSES.\n\nHOLD THE TRIGGER UNTIL ALL THAT'S LEFT IS BLOOD.");
			GunExt.SetupSprite(gun, null, "laserchainsaw_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 30);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 1);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 1);
			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);
			gun.SetAnimationFPS(gun.shootAnimation, 8);
			gun.isAudioLoop = true;
			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);
			gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2f, StatModifier.ModifyMethod.ADDITIVE);

			//GUN STATS
			gun.doesScreenShake = false;
			gun.DefaultModule.ammoCost = 10;
			gun.DefaultModule.angleVariance = 0;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Beam;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 1f;
			gun.muzzleFlashEffects.type = VFXPoolType.None;
			gun.DefaultModule.cooldownTime = 0.001f;
			gun.DefaultModule.numberOfShotsInClip = 1000;
			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.BEAM;
			gun.barrelOffset.transform.localPosition = new Vector3(0.93f, 0.18f, 0f);
			gun.SetBaseMaxAmmo(1000);
			gun.ammo = 1000;


			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 1;

			List<string> BeamAnimPaths = new List<string>()
			{
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_mid_001",
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_mid_002",
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_mid_003",
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_mid_004",
			};
			List<string> BeamEndPaths = new List<string>()
			{
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_end_001",
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_end_002",
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_end_003",
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_end_004",
			};

			//BULLET STATS
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

			BasicBeamController beamComp = projectile.GenerateBeamPrefab(
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_mid_001",
				new Vector2(5, 3),
				new Vector2(0, 1),
				BeamAnimPaths,
				10,
				//Impact
				null,
				10,
				null,
				null,
				//End
				BeamEndPaths,
				10,
				new Vector2(5, 3),
				new Vector2(0, 1),
				//Beginning
				null,
				10,
				null,
				null
				);

			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			projectile.baseData.damage =  300;
			projectile.baseData.force *= 0.1f;
			projectile.baseData.range = 3.5f;
			projectile.baseData.speed *= 1f;
			projectile.specRigidbody.CollideWithOthers = false;


			beamComp.penetration = 100;
			beamComp.boneType = BasicBeamController.BeamBoneType.Straight;
			beamComp.interpolateStretchedBones = false;
			beamComp.gameObject.AddComponent<EmmisiveBeams>(); ;
			gun.DefaultModule.projectiles[0] = projectile;

			gun.quality = PickupObject.ItemQuality.A; //D		
		gun.encounterTrackable.EncounterGuid = "https://enterthegungeon.gamepedia.com/Modding/Some_Bunny%27s_Content_Pack";
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND, true);

		}
		public override void PostProcessProjectile(Projectile projectile)
		{
			//projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHit));
			//projectile.OnWillKillEnemy = (Action<Projectile, SpeculativeRigidbody>)Delegate.Combine(projectile.OnWillKillEnemy, new Action<Projectile, SpeculativeRigidbody>(this.OnKill));

		}

		private void HandleHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
		{
			bool flag = arg2.aiActor != null && !arg2.healthHaver.IsBoss && !arg2.healthHaver.IsDead && arg2.aiActor.behaviorSpeculator && !arg2.aiActor.IsHarmlessEnemy && arg2.aiActor != null;
			if (flag)
			{
				this.teleporter = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>();
				UnityEngine.Object.Instantiate<GameObject>(this.teleporter.TelefragVFXPrefab, arg2.specRigidbody.UnitCenter, Quaternion.identity);
			}
		}
		private void OnKill(Projectile arg1, SpeculativeRigidbody arg2)
		{
			bool flag = !arg2.aiActor.healthHaver.IsDead;
			if (flag)
			{
				PlayerController player = arg1.Owner as PlayerController;
				float num = (player.stats.GetStatValue(PlayerStats.StatType.AmmoCapacityMultiplier));
				int ammo = 100*(int)num;
				if (this.gun.CurrentAmmo > (ammo-25))
                {
					this.gun.ammo = ammo;
				}
				else
                {
					this.gun.ammo += 25;
				}
			}
		}

		//private bool m_usedOverrideMaterial;
		private TeleporterPrototypeItem teleporter;
		private bool HasReloaded;

		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
		}
		public override void OnPostFired(PlayerController player, Gun bruhgun)
		{
			//TriggerRipAndTear = true;
			this.gun.ClipShotsRemaining = 100;

		}
		protected void Update()
		{
			PlayerController player = this.gun.CurrentOwner as PlayerController;
			if (gun.CurrentOwner)
			{
				/*
				if (this.gun.IsFiring)
                {
					TriggerRipAndTear = true;
				}
				else
                {
					TriggerRipAndTear = false;
				}
				if (TriggerRipAndTear == true && this.gun.CurrentAmmo != 0)
                {
					player.inventory.GunLocked.SetOverride("RIP", true, null);
					this.m_usedOverrideMaterial = player.sprite.usesOverrideMaterial;
					player.sprite.usesOverrideMaterial = true;
					player.SetOverrideShader(ShaderCache.Acquire("Brave/LitCutoutUberPhantom"));
					player.healthHaver.IsVulnerable = false;
				}
				else if (this.gun.CurrentAmmo == 0 || !this.gun.IsFiring)
                {
					TriggerRipAndTear = false;
					player.ClearOverrideShader();
					player.inventory.GunLocked.SetOverride("RIP", false, null);
					player.healthHaver.IsVulnerable = true;
				}
				*/
/*
			}
		}
		//private bool TriggerRipAndTear;
		public static bool UnderChainsawNoRoll;
	}
}


*/
namespace Planetside
{
	public class LaserChainsaw : AdvancedGunBehavior
	{
		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Laser Chainsaw", "laserchainsaw");
			Game.Items.Rename("outdated_gun_mods:laser_chainsaw", "psog:laser_chainsaw");
			var behav = gun.gameObject.AddComponent<LaserChainsaw>();
			//behav.overrideNormalFireAudio = "Play_ENM_shelleton_beam_01";
			behav.preventNormalFireAudio = true;
			behav.preventNormalReloadAudio = true;

			gun.SetShortDescription("KILL KILL KILL");
			gun.SetLongDescription("SHRED EVERYTHING IN SIGHT.\n\nLEAVE NO WITNESSES.\n\nHOLD THE TRIGGER UNTIL ALL THAT'S LEFT IS BLOOD.");

			gun.SetupSprite(null, "laserchainsaw_idle_001", 8);

			gun.SetAnimationFPS(gun.shootAnimation, 8);
			gun.isAudioLoop = true;
			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);
			gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2f, StatModifier.ModifyMethod.ADDITIVE);
			gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, 1.3f, StatModifier.ModifyMethod.MULTIPLICATIVE);

			//GUN STATS
			gun.doesScreenShake = false;
			gun.DefaultModule.ammoCost = 10;
			gun.DefaultModule.angleVariance = 0;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Beam;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 1f;
			gun.muzzleFlashEffects.type = VFXPoolType.None;
			gun.DefaultModule.cooldownTime = 0.001f;
			gun.DefaultModule.numberOfShotsInClip = 600;
			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.BEAM;
			gun.barrelOffset.transform.localPosition = new Vector3(0.93f, 0.18f, 0f);
			gun.SetBaseMaxAmmo(600);
			gun.ammo = 600;
			gun.gunClass = GunClass.BEAM;

			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 1;

			List<string> BeamAnimPaths = new List<string>()
			{
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_mid_001",
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_mid_002",
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_mid_003",
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_mid_004",
			};
			List<string> BeamEndPaths = new List<string>()
			{
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_end_001",
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_end_002",
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_end_003",
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_end_004",
			};

			//BULLET STATS
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

			BasicBeamController beamComp = projectile.GenerateBeamPrefab(
				"Planetside/Resources/Beams/LaserChainsaw/chainsaw_mid_001",
				new Vector2(5, 3),
				new Vector2(0, 1),
				BeamAnimPaths,
				60,
				//Impact
				null,
				60,
				null,
				null,
				//End
				BeamEndPaths,
				60,
				new Vector2(5, 3),
				new Vector2(0, 1),
				//Beginning
				null,
				60,
				null,
				null
				);

			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			projectile.baseData.damage = 200f;
			projectile.baseData.force *= 0.1f;
			projectile.baseData.range = 3.5f;
			projectile.baseData.speed *= 1f;

			beamComp.startAudioEvent = "Play_ENM_deathray_shot_01";
			beamComp.endAudioEvent = "Stop_ENM_deathray_loop_01";
			beamComp.penetration = 100;
			beamComp.boneType = BasicBeamController.BeamBoneType.Straight;
			beamComp.interpolateStretchedBones = false;
			beamComp.gameObject.AddComponent<LaserChainsawProjectile>();
			EmmisiveBeams emiss = beamComp.gameObject.AddComponent<EmmisiveBeams>();
			emiss.EmissiveColorPower = 1.7f;
			emiss.EmissivePower = 70;

			beamComp.SkipPostProcessing = false;
			gun.DefaultModule.projectiles[0] = projectile;

			gun.quality = PickupObject.ItemQuality.S; //D
			gun.encounterTrackable.EncounterGuid = "https://enterthegungeon.gamepedia.com/Modding/Some_Bunny%27s_Content_Pack";
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND, true);

			/*
			List<string> mandatoryConsoleIDs1 = new List<string>
			{
				"psog:laser_chainsaw"
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"psog:revenant"
			};
			CustomSynergies.Add("test chainsaw syn", mandatoryConsoleIDs1, optionalConsoleIDs, true);
			*/
			LaserChainsaw.LaserChainsawID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int LaserChainsawID;

		public override void PostProcessProjectile(Projectile proj)
		{
			//ETGModConsole.Log("AAAAAAAA");
		}
		public LaserChainsaw()
		{

		}
	}
}