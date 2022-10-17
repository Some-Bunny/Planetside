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
	public class StormBringer : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Storm Bringer", "hammerofstorms");
			Game.Items.Rename("outdated_gun_mods:storm_bringer", "psog:storm_bringer");
			var behav = gun.gameObject.AddComponent<StormBringer>();
			GunExt.SetShortDescription(gun, "Royal");
			GunExt.SetLongDescription(gun, "A bad replica of the hammer of legend, capable of generating and expelling more than lethal amounts of energy.\n\nPoint away from face. Yours, to be precise.");
			GunExt.SetupSprite(gun, null, "hammerofstorms_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 20);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 5);
			GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 5);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 5);

			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(38) as Gun, true, false);
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 4f;
			gun.DefaultModule.cooldownTime = 1f;
			gun.DefaultModule.numberOfShotsInClip = 4;
			gun.SetBaseMaxAmmo(44);
			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SKULL;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(328) as Gun).gunSwitchGroup;
            gun.carryPixelOffset += new IntVector2(-1, -2);

            //EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(),gun.chargeAnimation, new Dictionary<int, string> { { 1, "Play_WPN_bountyhunterarm_charge_01" }, { 2, "Play_ENM_bigbulletboy_step_01" }, { 4, "Play_FS_bone_step_01" }, { 7, "Play_WPN_rpg_reload_01" }, { 15, "Play_OBJ_mine_beep_01" } });
            //EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.shootAnimation, new Dictionary<int, string> { { 0, "Play_wpn_chargelaser_shot_01" }, { 1, "Play_BOSS_lichB_charge_01" } });
            //EnemyToolbox.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> { { 9, "Play_OBJ_lock_unlock_01" }, { 10, "Play_BOSS_lichC_zap_01" }, { 11, "Play_OBJ_metalskin_end_01" } });
            gun.barrelOffset.transform.localPosition = new Vector3(3.6875f, 1.125f, 0f);

			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 5;
			gun.gunClass = GunClass.FIRE;


			//gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			//gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Big Flame", "Planetside/Resources/GunClips/BurningSun/burningsunfull", "Planetside/Resources/GunClips/BurningSun/burningsunempty");

			gun.encounterTrackable.EncounterGuid = "LORD OF LIGHTNING";

			
			Projectile smallLightning = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(546) as Gun).DefaultModule.projectiles[0]);
            smallLightning.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(smallLightning.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(smallLightning);
			gun.DefaultModule.projectiles[0] = smallLightning;
            smallLightning.baseData.damage = 40f;
            smallLightning.baseData.speed = 60f;
            smallLightning.baseData.force *= 0f;
            smallLightning.baseData.range = 100f;
			var smallLightningController = smallLightning.AddComponent<StormBringerProjectile>();
            var lightningData1 = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericLargeExplosion);
            lightningData1.effect = EnemyDatabase.GetOrLoadByGuid("dc3cd41623d447aeba77c77c99598426").GetComponent<BossFinalMarineDeathController>().bigExplosionVfx[0];
            lightningData1.damage = 40;
            lightningData1.damageRadius = 3;
			smallLightningController.Explosion = lightningData1;
			smallLightningController.MajorNodesMin = 2;
            smallLightningController.MajorNodesMax = 4;
            smallLightning.pierceMinorBreakables = true;

            smallLightning.baseData.UsesCustomAccelerationCurve = true;
            smallLightning.baseData.AccelerationCurve = AnimationCurve.Linear(0.25f, 1, 0.25f, 1.5f);

            defaultLightningExplosion = lightningData1;

            ImprovedAfterImage afterImage1 = smallLightning.gameObject.AddComponent<ImprovedAfterImage>();
            afterImage1.spawnShadows = true;
            afterImage1.shadowLifetime = 1f;
            afterImage1.shadowTimeDelay = 0.01f;
            afterImage1.dashColor = new Color(0f, 0.9f, 1f, 1f);


            Projectile largeLightning = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(546) as Gun).DefaultModule.projectiles[0]);
            largeLightning.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(largeLightning.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(largeLightning);
            gun.DefaultModule.projectiles[0] = largeLightning;
            largeLightning.baseData.damage = 60f;
            largeLightning.baseData.speed = 60f;
            largeLightning.baseData.force *= 0f;
            largeLightning.baseData.range = 100f;
            largeLightning.pierceMinorBreakables = true;

            var largeLightningController = largeLightning.AddComponent<StormBringerProjectile>();
            var lightningData2 = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericLargeExplosion);
            lightningData2.effect = EnemyDatabase.GetOrLoadByGuid("dc3cd41623d447aeba77c77c99598426").GetComponent<BossFinalMarineDeathController>().bigExplosionVfx[0];
            lightningData2.damage = 100;
            lightningData2.damageRadius = 3;
            largeLightningController.Explosion = lightningData2;
            largeLightningController.MajorNodesMin = 4;
            largeLightningController.MajorNodesMax = 7;
			largeLightningController.Thickness = 3;
			largeLightningController.splitoffChance = 0.5f;

            largeLightning.baseData.UsesCustomAccelerationCurve = true;
            largeLightning.baseData.AccelerationCurve = AnimationCurve.Linear(0.25f, 1, 0.25f, 1.5f);

            ImprovedAfterImage afterImage2 = largeLightning.gameObject.AddComponent<ImprovedAfterImage>();
            afterImage2.spawnShadows = true;
            afterImage2.shadowLifetime = 1f;
            afterImage2.shadowTimeDelay = 0.01f;
            afterImage2.dashColor = new Color(0f, 0.9f, 1f, 1f);

            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
			{
				Projectile = smallLightning,
				ChargeTime = 0.6f,
				
			};
            ProjectileModule.ChargeProjectile item3 = new ProjectileModule.ChargeProjectile
            {
                Projectile = largeLightning,
                ChargeTime = 1.5f,
				AmmoCost = 3,
				UsedProperties = ProjectileModule.ChargeProjectileProperties.ammo,
            };

            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
			{
				item2,
                item3
            };

			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(387) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.S;
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			

            EnemyToolbox.AddEventTriggersToAnimation(gun.spriteAnimator, gun.reloadAnimation, new Dictionary<int, string> { { 8, "Charge" } });



            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(0, 220, 255, 255));
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

            StormBringer.StormBringerID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
		}

        public static ExplosionData defaultLightningExplosion;

		public void Start()
		{
            gun.spriteAnimator.AnimationEventTriggered += (obj, obj1, obj2) =>
            {
                if (obj1.GetFrame(obj2).eventInfo.Contains("Charge"))
                {
                    LightningController c = new LightningController();
                    c.MajorNodesCount = UnityEngine.Random.Range(5, 10);
                    c.Thickness = 1;
                    c.MajorNodeSplitoffChance = 0;
                    c.OnPostStrike += (obj4) =>
                    {
                        AkSoundEngine.PostEvent("Play_Lightning", GameManager.Instance.BestActivePlayer.gameObject);
                        var lightObj = UnityEngine.Object.Instantiate(LightningMaker.lightObject);
                        lightObj.transform.position = obj4;
                        Destroy(lightObj, 0.25f);
                    };
                    c.LightningPreDelay = 0f;
                    c.GenerateLightning(obj.GetComponentInChildren<Gun>().barrelOffset.position + new Vector3(UnityEngine.Random.Range(-10, 10), 25), obj.GetComponentInChildren<Gun>().barrelOffset.position);
                }
            };
        }
     

        public static int StormBringerID;
		private bool HasReloaded;

		public Vector3 projectilePos;
		public override void OnPostFired(PlayerController player, Gun flakcannon)
		{
			gun.PreventNormalFireAudio = true;
		}
		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				HasReloaded = false;
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
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
	}
}
