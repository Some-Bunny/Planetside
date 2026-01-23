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
using Alexandria.PrefabAPI;

namespace Planetside
{
	public class Revenant : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Revenant", "revenant");
			Game.Items.Rename("outdated_gun_mods:revenant", "psog:revenant");
			gun.gameObject.AddComponent<Revenant>();
			gun.SetShortDescription("Reaped By Death");
			gun.SetLongDescription("A simple, elegant and powerful revolver, capable of killing from behind cover.\n\nWielded by a particularly regretful undead Gungeoneer seeking an old friend...");
            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "revenant_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.idleAnimation = "revenant_idle";
            gun.shootAnimation = "revenant_fire";
            gun.reloadAnimation = "revenant_reload";


            //GunExt.SetupSprite(gun, null, "revenant_idle_001", 11);


            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_golddoublebarrelshotgun_shot_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
			//GunExt.SetAnimationFPS(gun, gun.shootAnimation, 30);
			//GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 15);
			//GunExt.SetAnimationFPS(gun, gun.idleAnimation, 1);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(387) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 1.7f;
			gun.DefaultModule.cooldownTime = .75f;
			gun.DefaultModule.numberOfShotsInClip = 5;
			gun.SetBaseMaxAmmo(50);
			gun.quality = PickupObject.ItemQuality.A;
			gun.DefaultModule.angleVariance = 6f;
			gun.DefaultModule.burstShotCount = 1;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 45f;
			projectile.baseData.speed *= 3f;
			projectile.AdditionalScaleMultiplier *= 0.75f;
			projectile.baseData.force *= 3;

            projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.PenetratesInternalWalls = true;
			projectile.gameObject.AddComponent<RevenantProjectile>();
			projectile.gameObject.AddComponent<MaintainDamageOnPierce>();

            ImprovedAfterImage image = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            image.spawnShadows = true;
            image.shadowLifetime = 0.2f;
            image.shadowTimeDelay = 0.01f;
            image.dashColor = new Color(1, 0.7f, 0, 0.03f);

            gun.gunClass = GunClass.NONE;

			gun.gameObject.transform.Find("Casing").transform.position = new Vector3(0.375f, 1f);
			gun.shellCasing = BreakAbleAPI.BreakableAPIToolbox.GenerateDebrisObject("Planetside/Resources/GunObjects/Casings/revenantcasing.png", true, 0.333f, 2, 1080, 360, null, 1.2f).gameObject;
			gun.shellsToLaunchOnFire = 0;
			gun.shellsToLaunchOnReload = 5;
			gun.reloadShellLaunchFrame = 6;	

			gun.reloadClipLaunchFrame = 0;
			gun.clipsToLaunchOnReload = 0;

			PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
			spook.penetration = 3;
			spook.penetratesBreakables = true;


			gun.SneakAttackDamageMultiplier = 5;
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(365) as Gun).muzzleFlashEffects;


            gun.encounterTrackable.EncounterGuid = "https://www.youtube.com/watch?v=HKmYRsnMsOk";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			Revenant.RevenantID = gun.PickupObjectId;

			List<string> mandatoryConsoleIDs = new List<string>
			{
				"psog:revenant",
			};
			List<string> optionalConsoleIDs = new List<string>
			{
				"skull_spitter",
				"vertebraek47"
			};
			CustomSynergies.Add("Boring Eternity", mandatoryConsoleIDs, optionalConsoleIDs, false);

			ItemIDs.AddToList(gun.PickupObjectId);
			BuildSoulGuon();

        }
		public static int RevenantID;
		
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

        public static void BuildSoulGuon()
        {
            //GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/Guons/SoulGuon/guoner.png");
            //gameObject.name = $"Soul Guon";
            GameObject gameObject = PrefabBuilder.BuildObject("SoulGuon");
            var spriteGem = gameObject.AddComponent<tk2dSprite>();
            spriteGem.SetSprite(StaticSpriteDefinitions.Guon_Sheet_Data, "guoner");

            SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
            PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
            speculativeRigidbody.CollideWithTileMap = false;
            speculativeRigidbody.CollideWithOthers = true;
            speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
            orbitalPrefab.shouldRotate = false;
            orbitalPrefab.orbitRadius = 4f;
            orbitalPrefab.orbitDegreesPerSecond = 40;
            orbitalPrefab.SetOrbitalTier(0);



            ImprovedAfterImage image = gameObject.gameObject.AddComponent<ImprovedAfterImage>();
            image.spawnShadows = true;
            image.shadowLifetime = 0.3f;
            image.shadowTimeDelay = 0.01f;
            image.dashColor = new Color(0, 0.2f, 1, 0.03f);


            /*
            var tro_1 = gameObject.AddChild("trail object");
			tro_1.transform.position = spriteGem.WorldCenter;// + new Vector2(.25f, 0.3125f);
            tro_1.transform.localPosition = spriteGem.WorldCenter;
            TrailRenderer tr_1 = tro_1.AddComponent<TrailRenderer>();
            tr_1.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tr_1.receiveShadows = false;
            var mat_1 = new Material(Shader.Find("Sprites/Default"));
            tr_1.material = mat_1;
            tr_1.minVertexDistance = 0.01f;
            tr_1.numCapVertices = 640;

            //======
            UnityEngine.Color color = new UnityEngine.Color(0, 1, 2, 2);
            mat_1.SetColor("_Color", color);
            tr_1.startColor = color;
            tr_1.endColor = new Color(0f, 0, 1, 0.5f);
            //======
            tr_1.time = 0.25f;
            //======
            tr_1.startWidth = 0.3125f;
            tr_1.endWidth = 0f;
            tr_1.autodestruct = false;
			*/



            PlayerOrbital si = gameObject.GetComponent<PlayerOrbital>();
			si.PreventOutline = true;
            si.sprite.usesOverrideMaterial = true;
            si.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
            Material mat = si.sprite.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = si.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 100);
            si.sprite.renderer.material = mat;

            SoulSynergyGuon = gameObject;
            UnityEngine.Object.DontDestroyOnLoad(SoulSynergyGuon);
            FakePrefab.MarkAsFakePrefab(SoulSynergyGuon);
            SoulSynergyGuon.SetActive(false);
        }
        public static GameObject SoulSynergyGuon;

    }

}