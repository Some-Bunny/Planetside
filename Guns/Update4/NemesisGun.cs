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
	public class NemesisGun : GunBehaviour
	{
		public class HippityHoppityTheProjectileDestroyModesIsAnUnserializedProperty : MonoBehaviour{public void Start(){this.GetComponent<Projectile>().DestroyMode = Projectile.ProjectileDestroyMode.BecomeDebris;}}


		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("NemesisGun", "nemesisgun");
			Game.Items.Rename("outdated_gun_mods:nemesisgun", "psog:nemesisgun");
			gun.gameObject.AddComponent<NemesisGun>();
			gun.SetShortDescription("Reaped By Death");
			gun.SetLongDescription("A simple, elegant and powerful revolver, capable of killing even behind cover.\n\nWielded by a particularly regretful undead Gungeoneer seeking an old friend...");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "nemesisgun_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 1;
            
			gun.m_defaultSpriteID = StaticSpriteDefinitions.Gun_2_Sheet_Data.GetSpriteIdByName("nemesisgun_idle_001");
            int num = (gun.DefaultSpriteID = StaticSpriteDefinitions.Gun_2_Sheet_Data.GetSpriteIdByName("nemesisgun_idle_001"));
            gun.sprite.SetSprite(StaticSpriteDefinitions.Gun_2_Sheet_Data, num);
            gun.m_sprite.SetSprite(StaticSpriteDefinitions.Gun_2_Sheet_Data, num);
			gun.m_sprite = gun.sprite;
            gun.reloadAnimation = "nemesisRevolver_reload";
            gun.idleAnimation = "nemesisRevolver_idle";
            gun.shootAnimation = "nemesisRevolver_fire";


            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(387) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 1.7f;
			gun.DefaultModule.cooldownTime = .5f;
			gun.DefaultModule.numberOfShotsInClip = 5;
			gun.SetBaseMaxAmmo(50);
			gun.quality = PickupObject.ItemQuality.EXCLUDED;
			gun.DefaultModule.angleVariance = 6f;
			gun.DefaultModule.burstShotCount = 1;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			projectile.baseData.damage = 35f;
			projectile.baseData.speed *= 1f;
			projectile.AdditionalScaleMultiplier *= 0.75f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.PenetratesInternalWalls = false;
			projectile.gameObject.AddComponent<RevenantProjectile>();
			projectile.gameObject.AddComponent<MaintainDamageOnPierce>();

			//projectile.ga.eObject.AddComponent<HippityHoppityTheProjectileDestroyModesIsAnUnserializedProperty>();
			//===
			//gun.muzzleOffset.transform.localPosition = new Vector3(15f / 16f, 8f / 16f, 0f);

			projectile.DestroyMode = Projectile.ProjectileDestroyMode.BecomeDebris;
			projectile.OnBecameDebris += (obj) => { };

			gun.DefaultModule.projectiles[0] = projectile;

			//projectile.debris = BreakAbleAPI.BreakableAPIToolbox.GenerateDebrisObject("Planetside/Resources/GunObjects/Clips/bigassmag.png", true, 1, 3, 60, 20, null, 2, "Play_ITM_Crisis_Stone_Impact_02", null, 1).gameObject;

			//===
			gun.gunClass = GunClass.NONE;

			/*
			gun.gameObject.transform.Find("Casing").transform.position = new Vector3(0.375f, 1f);
			gun.shellCasing = BreakAbleAPI.BreakableAPIToolbox.GenerateDebrisObject("Planetside/Resources/GunObjects/Casings/revenantcasing.png", true, 0.333f, 2, 1080, 360, null, 1.2f).gameObject;
			gun.shellsToLaunchOnFire = 0;
			gun.shellsToLaunchOnReload = 5;
			gun.reloadShellLaunchFrame = 6;
			*/
			gun.reloadClipLaunchFrame = 0;
			gun.clipsToLaunchOnReload = 0;

			Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
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

			PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
			spook.penetration = 3;
			spook.penetratesBreakables = true;
			gun.encounterTrackable.EncounterGuid = "LOOK WHAT I FOUND";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			NemesisGun.NemesisGunID = gun.PickupObjectId;

		



			//ItemIDs.AddToList(gun.PickupObjectId);
		}
		public static int NemesisGunID;
		
		private bool HasReloaded;


		public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
				AkSoundEngine.PostEvent("Play_WPN_rpg_reload_01", gameObject);
			}
		}
	}

}