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
using UnityEngine.UI;
using Alexandria.Assetbundle;
using Planetside.Toolboxes;

namespace Planetside
{
	public class StaticCharger : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Powerbank", "StaticCharger");
			Game.Items.Rename("outdated_gun_mods:powerbank", "psog:powerbank");
			gun.gameObject.AddComponent<StaticCharger>();
			GunExt.SetShortDescription(gun, "Batteries Included");
			GunExt.SetLongDescription(gun, "Grows in power for every room cleared without being fired. Using the weapon discharges all the power it had after a reload.\n\nCharges from the users static electricity. This thing should also literally not be able to shoot.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "bigCharger001_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.idleAnimation = "staticharger_idle_0";
            gun.shootAnimation = "staticharger_fire_0";
            gun.reloadAnimation = "staticcharger_reload";


            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(62) as Gun, true, false);
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(62) as Gun, true, false);
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(62) as Gun, true, false);
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(62) as Gun, true, false);
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(62) as Gun, true, false);
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(62) as Gun, true, false);


            gun.SetBaseMaxAmmo(500);
			gun.ammo = 500;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(13) as Gun).gunSwitchGroup;

            gun.Volley.ModulesAreTiers = true;


            for (int i = 0; i < gun.Volley.projectiles.Count; i++)
            {

                var entry = gun.Volley.projectiles[i];
                entry.ammoType = (PickupObjectDatabase.GetById(13) as Gun).DefaultModule.ammoType;
                entry.customAmmoType = (PickupObjectDatabase.GetById(13) as Gun).DefaultModule.customAmmoType;

                entry.ammoCost = 1;
                entry.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;

                entry.cooldownTime = 0.125f;
                entry.angleVariance = 6f;

                entry.numberOfShotsInClip = 40;

                switch (i)
                {
                    case 0:

                        entry.angleVariance = 8f;

                        Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0]);
                        projectile.gameObject.SetActive(false);
                        FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                        UnityEngine.Object.DontDestroyOnLoad(projectile);
                        projectile.baseData.damage = 4f;
                        projectile.baseData.speed = 25f;
                        projectile.AppliesFreeze = false;
                        projectile.baseData.range = 20;
                        projectile.baseData.force = 3;

                        var tro = projectile.gameObject.AddChild("trail object");
                        tro.transform.position = projectile.sprite.WorldCenter;// + new Vector2(.25f, 0.3125f);
                        tro.transform.localPosition = projectile.sprite.WorldCenter;

                        TrailRenderer tr = tro.AddComponent<TrailRenderer>();
                        tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        tr.receiveShadows = false;
                        var trailMat = new Material(Shader.Find("Sprites/Default"));
                        tr.material = trailMat;
                        tr.minVertexDistance = 0.01f;
                        tr.numCapVertices = 640;

                        //======
                        UnityEngine.Color color = new UnityEngine.Color(0, 1, 1, 1);
                        trailMat.SetColor("_Color", color);
                        tr.startColor = color;
                        tr.endColor = new Color(0, 0.2f, 0.5f, 0.5f);
                        //======
                        tr.time = 0.2f;
                        //======
                        tr.startWidth = 0.25f;
                        tr.endWidth = 0f;
                        tr.autodestruct = false;

                        var rend = projectile.gameObject.AddComponent<ProjectileTrailRendererController>();
                        rend.trailRenderer = tr;
                        rend.desiredLength = 5;

                        entry.projectiles[0] = projectile;
                        break;
                    case 1:
                        entry.cooldownTime = 0.15f;
                        entry.angleVariance = 5f;

                        Projectile projectile_1 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(18) as Gun).DefaultModule.projectiles[0]);
                        projectile_1.gameObject.SetActive(false);
                        FakePrefab.MarkAsFakePrefab(projectile_1.gameObject);
                        UnityEngine.Object.DontDestroyOnLoad(projectile_1);
                        projectile_1.baseData.damage = 7.5f;
                        projectile_1.baseData.speed  = 30f;
                        projectile_1.AppliesFreeze = false;
                        projectile_1.baseData.range = 25;
                        projectile_1.baseData.force = 5;
                        projectile_1.pierceMinorBreakables = true;

                        var tro_1 = projectile_1.gameObject.AddChild("trail object");
                        tro_1.transform.position = projectile_1.sprite.WorldCenter;// + new Vector2(.25f, 0.3125f);
                        tro_1.transform.localPosition = projectile_1.sprite.WorldCenter;

                        TrailRenderer tr_1 = tro_1.AddComponent<TrailRenderer>();
                        tr_1.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        tr_1.receiveShadows = false;
                        var trailMat_1 = new Material(Shader.Find("Sprites/Default"));
                        tr_1.material = trailMat_1;
                        tr_1.minVertexDistance = 0.01f;
                        tr_1.numCapVertices = 640;

                        //======
                        trailMat_1.SetColor("_Color", new Color(0, 0.7f, 1, 1));
                        tr_1.startColor = new Color(0, 0.7f, 1, 1);
                        tr_1.endColor = new Color(0, 0f, 0.5f, 0.5f);
                        //======
                        tr_1.time = 0.333f;
                        //======
                        tr_1.startWidth = 0.375f;
                        tr_1.endWidth = 0f;
                        tr_1.autodestruct = false;

                        var rend_1 = projectile_1.gameObject.AddComponent<ProjectileTrailRendererController>();
                        rend_1.trailRenderer = tr_1;
                        rend_1.desiredLength = 6.5f;

                        entry.projectiles[0] = projectile_1;

                        break;
                    case 2:
                        Projectile projectile_2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0]);
                        projectile_2.gameObject.SetActive(false);
                        FakePrefab.MarkAsFakePrefab(projectile_2.gameObject);
                        UnityEngine.Object.DontDestroyOnLoad(projectile_2);
                        projectile_2.baseData.damage = 13.5f;
                        projectile_2.baseData.speed = 33.3f;
                        projectile_2.baseData.range = 30;
                        projectile_2.baseData.force = 8;
                        projectile_2.pierceMinorBreakables = true;

                        ImprovedAfterImage yes = projectile_2.gameObject.AddComponent<ImprovedAfterImage>();
                        yes.spawnShadows = true;
                        yes.shadowLifetime = 0.333f;
                        yes.shadowTimeDelay = 0.0333f;
                        yes.dashColor = new Color(0f, 0.4f, 1, 0.4f);
                        yes.name = "Gun Trail";

                        entry.projectiles[0] = projectile_2;

                        break;
                    case 3:
                        entry.cooldownTime = 0.18f;
                        entry.angleVariance = 4f;
                        entry.numberOfShotsInClip = 30;


                        Projectile projectile_3 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(156) as Gun).DefaultModule.projectiles[0]);
                        projectile_3.gameObject.SetActive(false);
                        FakePrefab.MarkAsFakePrefab(projectile_3.gameObject);
                        UnityEngine.Object.DontDestroyOnLoad(projectile_3);
                        projectile_3.baseData.damage = 18.75f;
                        projectile_3.baseData.speed = 50f;
                        projectile_3.AppliesStun = true;
                        projectile_3.StunApplyChance = 0.05f;
                        projectile_3.AppliedStunDuration = 1.25f;
                        projectile_3.baseData.range = 40;
                        projectile_3.baseData.force = 10;
                        projectile_3.pierceMinorBreakables = true;

                        Destroy(projectile_3.GetComponent<PierceProjModifier>());

                        ImprovedAfterImage yes_1 = projectile_3.gameObject.AddComponent<ImprovedAfterImage>();
                        yes_1.spawnShadows = true;
                        yes_1.shadowLifetime = 0.5f;
                        yes_1.shadowTimeDelay = 0.05f;
                        yes_1.dashColor = new Color(0f, 0.4f, 1, 0.7f);
                        yes_1.name = "Gun Trail";

                        entry.projectiles[0] = projectile_3;
                        break;
                    case 4:
                        entry.cooldownTime = 0.2f;
                        entry.angleVariance = 4f;
                        entry.numberOfShotsInClip = 25;

                        Projectile projectile_4 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(383) as Gun).DefaultModule.projectiles[0]);
                        projectile_4.gameObject.SetActive(false);
                        FakePrefab.MarkAsFakePrefab(projectile_4.gameObject);
                        UnityEngine.Object.DontDestroyOnLoad(projectile_4);
                        projectile_4.baseData.damage = 25f;
                        projectile_4.AppliesStun = true;
                        projectile_4.StunApplyChance = 0.08f;
                        projectile_4.AppliedStunDuration = 1.5f;
                        projectile_4.baseData.range = 75;
                        projectile_4.baseData.force = 18;
                        projectile_4.pierceMinorBreakables = true;


                        entry.projectiles[0] = projectile_4;
                        break;
                    case 5:
                        entry.cooldownTime = 0.25f;
                        entry.angleVariance = 3f;
                        entry.numberOfShotsInClip = 20;
                        


                        Projectile projectile_5 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(153) as Gun).DefaultModule.projectiles[0]);
                        projectile_5.gameObject.SetActive(false);
                        FakePrefab.MarkAsFakePrefab(projectile_5.gameObject);
                        UnityEngine.Object.DontDestroyOnLoad(projectile_5);
                        projectile_5.baseData.damage = 33f;
                        projectile_5.AppliesStun = true;
                        projectile_5.StunApplyChance = 0.11f;
                        projectile_5.AppliedStunDuration = 2f;
                        projectile_5.baseData.range = 120;
                        projectile_5.baseData.force = 40;
                        projectile_5.pierceMinorBreakables = true;


                        entry.projectiles[0] = projectile_5;
                        break;
                }
            }



			gun.reloadTime = 1.9f;

			gun.barrelOffset.transform.localPosition = new Vector3(1.25f, 0.5625f, 0f);
			gun.quality = PickupObject.ItemQuality.D;
			gun.encounterTrackable.EncounterGuid = "Static Tuah";
			gun.CanBeDropped = true;
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(13) as Gun).muzzleFlashEffects;
			gun.gunClass = GunClass.CHARGE;

			ETGMod.Databases.Items.Add(gun, false, "ANY");
            StaticCharger.ItemID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);


            ImprovedSynergySetup.Add("Accumulator", 
             new List<PickupObject> { gun } 
            ,new List<PickupObject>() {
                Items.Battery_Bullets,
                Items.Shock_Rounds,
            }, true);

            ImprovedSynergySetup.Add("AA", new List<PickupObject> { gun, Guns.Shock_Rifle }, null, true);

            AdvancedDualWieldSynergyProcessor advancedDualWieldSynergyProcessor = (PickupObjectDatabase.GetById(gun.PickupObjectId) as Gun).gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            advancedDualWieldSynergyProcessor.PartnerGunID = Guns.Shock_Rifle.PickupObjectId;
            advancedDualWieldSynergyProcessor.SynergyNameToCheck = "AA";
            AdvancedDualWieldSynergyProcessor advancedDualWieldSynergyProcessor1 = (PickupObjectDatabase.GetById(Guns.Shock_Rifle.PickupObjectId) as Gun).gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            advancedDualWieldSynergyProcessor1.PartnerGunID = gun.PickupObjectId;
            advancedDualWieldSynergyProcessor1.SynergyNameToCheck = "AA";
        }
        public static int ItemID;



        public override void Start()
        {
            base.Start();
            gun.OnAutoReload += (_, __) =>
            {
                UpgradeLock = false;
                gun.CurrentStrengthTier = 0;
                AlterAnimations(true);
            };
            gun.OnReloadPressed += (_, __, ___) =>
            {
                if (___ == true)
                {
                    if (gun.CurrentOwner is PlayerController player && player.PlayerHasActiveSynergy("Accumulator"))
                    {
                        UpgradeLock = false;
                        gun.CurrentStrengthTier = Mathf.Max(0, gun.CurrentStrengthTier - 2);
                        AlterAnimations();
                    }
                    else
                    {
                        UpgradeLock = false;
                        gun.CurrentStrengthTier = 0;
                        AlterAnimations();
                    }
                }
            };
        }

        public void AlterAnimations(bool PlayIdle = false)
        {
            gun.idleAnimation = $"staticharger_idle_{gun.CurrentStrengthTier}";
            gun.shootAnimation = $"staticharger_fire_{gun.CurrentStrengthTier}";
            if (PlayIdle)
            {
                gun.PlayIdleAnimation();
            }

            switch (gun.CurrentStrengthTier)
            {
                case 0:
                    gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(13) as Gun).muzzleFlashEffects;
                    gun.gunSwitchGroup = (PickupObjectDatabase.GetById(13) as Gun).gunSwitchGroup;
                    gun.m_hasReinitializedAudioSwitch = false;
                    ParticlesPerSecond = 0;
                    break;
                case 1:
                    gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(13) as Gun).muzzleFlashEffects;
                    gun.gunSwitchGroup = (PickupObjectDatabase.GetById(13) as Gun).gunSwitchGroup;
                    gun.m_hasReinitializedAudioSwitch = false;
                    ParticlesPerSecond = 1.5f;

                    break;
                case 2:
                    gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(223) as Gun).muzzleFlashEffects;
                    gun.gunSwitchGroup = (PickupObjectDatabase.GetById(32) as Gun).gunSwitchGroup;
                    gun.m_hasReinitializedAudioSwitch = false;
                    ParticlesPerSecond = 2.5f;

                    break;
                case 3:
                    gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(199) as Gun).muzzleFlashEffects;
                    gun.gunSwitchGroup = (PickupObjectDatabase.GetById(153) as Gun).gunSwitchGroup;
                    gun.m_hasReinitializedAudioSwitch = false;
                    ParticlesPerSecond = 4f;

                    break;
                case 4:
                    gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(383) as Gun).muzzleFlashEffects;
                    gun.gunSwitchGroup = (PickupObjectDatabase.GetById(383) as Gun).gunSwitchGroup;
                    gun.m_hasReinitializedAudioSwitch = false;
                    ParticlesPerSecond = 6f;

                    break;
                case 5:
                    gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(180) as Gun).muzzleFlashEffects;
                    gun.gunSwitchGroup = (PickupObjectDatabase.GetById(156) as Gun).gunSwitchGroup;
                    gun.m_hasReinitializedAudioSwitch = false;
                    ParticlesPerSecond = 9f;
                    break;
            }
        }

        public override void OnPickup(PlayerController player)
		{
			base.OnPickup(player);
            gun.OnFinishAttack += OFA;
           
            player.OnRoomClearEvent += ORCE;
        }
        public override void OnPostDrop(PlayerController player)
        {
            base.OnPostDrop(player);
            gun.OnFinishAttack -= OFA;
            player.OnRoomClearEvent -= ORCE;
        }

        public void OFA(PlayerController playerController, Gun g)
        {
            if (playerController.IsInCombat)
            {
                UpgradeGunThisRoom = false;
                UpgradeLock = true;
            }
        }

        public void ORCE(PlayerController playerController)
        {
            if (UpgradeLock && gun.CurrentStrengthTier > 0) { return; }
            if (UpgradeGunThisRoom == true && gun.CurrentStrengthTier < 5)
            {
                int tier = gun.CurrentStrengthTier;
                gun.ForceImmediateReload(true);
                gun.CurrentStrengthTier = Mathf.Min(5, tier + 1);
                AlterAnimations(true);
                float amount = (gun.CurrentStrengthTier * 6 + 6);
                float ang = 360f / amount;
                for (int i = 0; i < amount; i++)
                {
                    GlobalSparksDoer.DoSingleParticle(playerController.sprite.WorldCenter, MathToolbox.GetUnitOnCircle(ang * i, 2 +  UnityEngine.Random.Range(ParticlesPerSecond * 0.2f, ParticlesPerSecond * 0.4f)), null, UnityEngine.Random.Range(0.5f, 1.25f), Color.cyan, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                }
                AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Player_Charge_01", playerController.gameObject);
            }
            UpgradeGunThisRoom = true;
        }
        public bool UpgradeLock;
        private bool UpgradeGunThisRoom = true;
        private float ParticlesPerSecond = 0;
        public override void Update()
        {
            base.Update();
            if (gun.CurrentOwner == null){ return;}
            if (ParticlesPerSecond > 0 && (gun.CurrentOwner is PlayerController player))
            {
                if (UnityEngine.Random.value < ParticlesPerSecond * Time.deltaTime)
                {
                    GlobalSparksDoer.DoRandomParticleBurst(1,
                        this.gun.barrelOffset.position + MathToolbox.GetUnitOnCircle3(BraveUtility.RandomAngle(), 0.25f),
                         this.gun.barrelOffset.position + MathToolbox.GetUnitOnCircle3(BraveUtility.RandomAngle(), 0.25f),
                         MathToolbox.GetUnitOnCircle(360 + (gun.CurrentAngle % 360), 2f + UnityEngine.Random.Range(ParticlesPerSecond * 0.1f, ParticlesPerSecond * 0.5f)),
                         0,
                         0,
                        0.1f, 
                        UnityEngine.Random.Range(0.6f, 1.5f), 
                        Color.cyan, 
                        GlobalSparksDoer.SparksType.DARK_MAGICKS);
                }
            }
        }

    }
}
