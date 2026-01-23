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
using Alexandria.Assetbundle;

namespace Planetside
{
	public class LaserChainsaw : AdvancedGunBehavior
	{
		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Laser Chainsaw", "laserchainsaw");
			Game.Items.Rename("outdated_gun_mods:laser_chainsaw", "psog:laser_chainsaw");
			var behav = gun.gameObject.AddComponent<LaserChainsaw>();
			behav.preventNormalFireAudio = true;
			behav.preventNormalReloadAudio = true;
			gun.SetShortDescription("KILL KILL KILL");
			gun.SetLongDescription("SHRED EVERYTHING IN SIGHT.\n\nLEAVE NO WITNESSES.\n\nHOLD THE TRIGGER UNTIL ALL THAT'S LEFT IS BLOOD.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "laserchainsaw_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.idleAnimation = "laserchainsaw_idle";
            gun.shootAnimation = "laserchainsaw_fire";
            gun.reloadAnimation = "laserchainsaw_reload";


            //gun.SetupSprite(null, "laserchainsaw_idle_001", 8);

            //gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.isAudioLoop = true;
			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);
			gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2f, StatModifier.ModifyMethod.ADDITIVE);
			gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, 1.3f, StatModifier.ModifyMethod.MULTIPLICATIVE);

			//GUN STATS
			gun.doesScreenShake = false;
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(370) as Gun).gunSwitchGroup;


            gun.DefaultModule.ammoCost = 10;
			gun.DefaultModule.angleVariance = 0;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Beam;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 1f;
			gun.muzzleFlashEffects.type = VFXPoolType.None;
			gun.DefaultModule.cooldownTime = 0.001f;
			gun.DefaultModule.numberOfShotsInClip = 500;
			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.BEAM;
			gun.barrelOffset.transform.localPosition = new Vector3(0.75f, 0.375f, 0f);
			gun.SetBaseMaxAmmo(500);
			gun.ammo = 500;
			gun.gunClass = GunClass.BEAM;





			//BULLET STATS
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);

            BasicBeamController beamComp = projectile.GenerateBeamPrefabBundle(
				"chainsaw_mid_001",
				StaticSpriteDefinitions.Beam_Sheet_Data,
				StaticSpriteDefinitions.Beam_Animation_Data,
                "laserchainsaw_mid", new Vector2(8, 6), new Vector2(0, 1), //Main
				null, null, null, //Impact
                "laserchainsaw_end", new Vector2(8, 6), new Vector2(0, 1), //End Of beam
				null, null, null, //Start Of Beam
				true);


            projectile.gameObject.SetActive(false);

			projectile.baseData.damage = 180f;
			projectile.baseData.force *= 0.1f;
			projectile.baseData.range = 4.5f;
			projectile.baseData.speed *= 1f;
			//projectile.ignoreDamageCaps = true;


            beamComp.startAudioEvent = "Play_ENM_deathray_shot_01";
			beamComp.endAudioEvent = "Stop_ENM_deathray_loop_01";
			beamComp.penetration = 100;
			beamComp.boneType = BasicBeamController.BeamBoneType.Straight;
			beamComp.interpolateStretchedBones = false;
			beamComp.gameObject.AddComponent<LaserChainsawProjectile>();
			EmmisiveBeams emiss = beamComp.gameObject.AddComponent<EmmisiveBeams>();
			emiss.EmissiveColorPower = 1.2f;
			emiss.EmissivePower = 25;

			beamComp.SkipPostProcessing = false;
			


            gun.DefaultModule.projectiles[0] = projectile;
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.gameObject.SetActive(false);
            gun.quality = PickupObject.ItemQuality.S; //D
			gun.encounterTrackable.EncounterGuid = "https://enterthegungeon.gamepedia.com/Modding/Some_Bunny%27s_Content_Pack";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND, true);

			Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.SetColor("_EmissiveColor", new Color32(0, 68, 255, 255));
			mat.SetFloat("_EmissiveColorPower", 1.55f);
			mat.SetFloat("_EmissivePower", 50);
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

			LaserChainsaw.LaserChainsawID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int LaserChainsawID;

		public LaserChainsaw(){}
	}
}