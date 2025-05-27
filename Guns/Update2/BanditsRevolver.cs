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

namespace Planetside
{
	public class BanditsRevolver : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Bandits Revolver", "banditsrevolver");
			Game.Items.Rename("outdated_gun_mods:bandits_revolver", "psog:bandits_revolver");
			gun.gameObject.AddComponent<BanditsRevolver>();
			GunExt.SetShortDescription(gun, "Lights Out");
			GunExt.SetLongDescription(gun, "The thrill of landing the perfect shot is seeked out by many a Gunslinger, with some devoting their lives to landing their swan song shot.\n\nDon't waste your chance when your high-noon comes.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "banditsrevolver_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.idleAnimation = "banditsrevolver_idle";
            gun.shootAnimation = "banditsrevolver_fire";
            gun.reloadAnimation = "banditsrevolver_reload";
            gun.finalShootAnimation = "banditsrevolver_finalfire";
            gun.introAnimation = "banditsrevolver_draw";


            //GunExt.SetupSprite(gun, null, "banditsrevolver_idle_001", 8);
            //GunExt.SetAnimationFPS(gun, gun.shootAnimation, 24);
            //GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 12);
            //GunExt.SetAnimationFPS(gun, gun.idleAnimation, 3);

            //gun.finalShootAnimation = gun.shootAnimation;
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(62) as Gun, true, false);
			gun.SetBaseMaxAmmo(300);
			gun.ammo = 300;

			//gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_colt1851_shot_03";
			//gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;

			//gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.finalShootAnimation).frames[0].eventAudio = "Play_WPN_colt1851_shot_03";
			//gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.finalShootAnimation).frames[0].triggerEvent = true;

			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_WPN_SAA_reload_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;

			Gun gun3 = PickupObjectDatabase.GetById(223) as Gun;
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(62) as Gun).gunSwitchGroup;
			Gun gun2 = PickupObjectDatabase.GetById(378) as Gun;

			Projectile projectile1 = UnityEngine.Object.Instantiate<Projectile>(gun2.DefaultModule.projectiles[0]); projectile1.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile1.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile1);

			//gun.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));

			Projectile replacementProjectile = projectile1.projectile;
			replacementProjectile.baseData.damage = 25;
			replacementProjectile.gameObject.AddComponent<BanditsRevolverFinaleProjectile>();
			replacementProjectile.baseData.speed *= 1.75f;
            replacementProjectile.baseData.force *= 2;


            replacementProjectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(387) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            replacementProjectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(387) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            replacementProjectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(387) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            replacementProjectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(387) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);

			replacementProjectile.additionalStartEventName = "Play_WPN_golddoublebarrelshotgun_shot_01";
			replacementProjectile.pierceMinorBreakables = true;

            OtherTools.EasyTrailComponent trail = replacementProjectile.gameObject.AddComponent<OtherTools.EasyTrailComponent>();
			trail.TrailPos = replacementProjectile.transform.position;
			trail.StartColor = Color.white;
			trail.StartWidth = 0.2f;
			trail.EndWidth = 0;
			trail.LifeTime = 1f;
			trail.BaseColor = new Color(1f, 1f, 6f, 2f);
			trail.EndColor = new Color(0f, 0f, 1f, 0f);

			gun.DefaultModule.usesOptionalFinalProjectile = true;

			gun.DefaultModule.numberOfFinalProjectiles = 1;
			gun.DefaultModule.finalProjectile = replacementProjectile;
			gun.DefaultModule.finalCustomAmmoType = (PickupObjectDatabase.GetById(696) as Gun).DefaultModule.customAmmoType;

            gun.DefaultModule.finalAmmoType = (PickupObjectDatabase.GetById(696) as Gun).DefaultModule.ammoType;


            //696

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(35) as Gun).gunSwitchGroup;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = (PickupObjectDatabase.GetById(35) as Gun).DefaultModule.customAmmoType;



			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.damageModifier = 1;
			gun.reloadTime = 3f;
			gun.DefaultModule.cooldownTime = 0.166f;
			gun.DefaultModule.numberOfShotsInClip = 6;
			gun.DefaultModule.angleVariance = 2f;
			gun.barrelOffset.transform.localPosition = new Vector3(1.125f, 0.5f, 0f);
			gun.quality = PickupObject.ItemQuality.C;
			gun.encounterTrackable.EncounterGuid = "Another Goddamn Risk Of Rain reference ";
			gun.gunClass = GunClass.PISTOL;
			gun.CanBeDropped = true;
			Gun gun4 = PickupObjectDatabase.GetById(50) as Gun;
			gun.muzzleFlashEffects = gun4.muzzleFlashEffects;
			gun.gunClass = GunClass.PISTOL;



			ETGMod.Databases.Items.Add(gun, false, "ANY");
			SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.DODGELOAD);

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]); projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.transform.parent = gun.barrelOffset;
			projectile.AdditionalScaleMultiplier *= 1f;
			projectile.baseData.damage = 10f;
			projectile.baseData.speed *= 1.4f;

			gun.finalMuzzleFlashEffects = (PickupObjectDatabase.GetById(696) as Gun).muzzleFlashEffects;
            gun.alternateSwitchGroup = (PickupObjectDatabase.GetById(387) as Gun).gunSwitchGroup;


            BanditsRevolver.BanditsRevolverID = gun.PickupObjectId;

			ItemIDs.AddToList(gun.PickupObjectId);

            var debuffCollection = StaticSpriteDefinitions.Debuff_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("BanditRevolverExecute", debuffCollection.GetSpriteIdByName("cankill_005"), debuffCollection);
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.VFX_Animation_Data;
            animator.Library = StaticSpriteDefinitions.VFX_Animation_Data;

            animator.sprite.usesOverrideMaterial = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = animator.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(151, 251, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 2f);
            mat.SetFloat("_EmissivePower", 20);
            animator.sprite.renderer.material = mat;

            animator.DefaultClipId = animator.GetClipIdByName("cankillStart");
            animator.playAutomatically = true;
            CanKillRevolverEffect = BrokenArmorVFXObject;

            CanKillRevolverGameActorEffect = new GameActorDecorationEffect();
            CanKillRevolverGameActorEffect.AffectsEnemies = true;
            CanKillRevolverGameActorEffect.PlaysVFXOnActor = true;
            CanKillRevolverGameActorEffect.effectIdentifier = "BanditRevolverExecute";
            CanKillRevolverGameActorEffect.OverheadVFX = CanKillRevolverEffect;
			CanKillRevolverGameActorEffect.PlaysVFXOnActor = false;
			CanKillRevolverGameActorEffect.stackMode = GameActorEffect.EffectStackingMode.Refresh;
			CanKillRevolverGameActorEffect.duration = 3600;
			CanKillRevolverGameActorEffect.TintColor = new Color(0.6f, 0.94f, 1, 1);
			CanKillRevolverGameActorEffect.AppliesTint = true;

            gun.sprite.usesOverrideMaterial = true;

            Material mat_ = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat_.SetColor("_EmissiveColor", new Color32(151, 251, 255, 255));
            mat_.SetFloat("_EmissiveColorPower", 1.55f);
            mat_.SetFloat("_EmissivePower", 20);
            mat_.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
            MeshRenderer component = gun.GetComponent<MeshRenderer>();
            if (!component)
            {
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == mat_)
                {
                    return;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(mat_);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;

        }
        public static int BanditsRevolverID;
		public static GameObject CanKillRevolverEffect;
        public static GameActorDecorationEffect CanKillRevolverGameActorEffect;

        private bool HasReloaded;

		public Vector3 projectilePos;

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
				if (newGun != this.gun)
				{
                    List<AIActor> activeEnemies  = (this.gun.CurrentOwner as PlayerController).CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    if (activeEnemies != null)
                    {
                        foreach (AIActor aiactor in activeEnemies)
                        {
                            aiactor.RemoveEffect("BanditRevolverExecute");
                        }
                    }
                }
            }
		}
		public override void Update()
		{
			base.Update();
            if (gun.CurrentOwner)
            {
                if (gun.CurrentOwner as PlayerController)
                {

                    PlayerController player = gun.CurrentOwner as PlayerController;
                    List<AIActor> activeEnemies = player.CurrentRoom?.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    if (activeEnemies != null)
                    {
                        foreach (AIActor aiactor in activeEnemies)
                        {
                            if (aiactor != null)
                            {
                                float scale = 25;
                                if (player != null)
                                {
                                    scale = scale * player.stats.GetStatValue(PlayerStats.StatType.Damage);
                                }

                                if (aiactor.healthHaver != null)
                                {
                                    if (player.CurrentGun == gun)
                                    {
                                        if (aiactor.healthHaver.GetCurrentHealth() < scale)
                                        {
                                            aiactor.ApplyEffect(CanKillRevolverGameActorEffect);
                                        }
                                        else
                                        {
                                            aiactor.RemoveEffect("BanditRevolverExecute");

                                        }
                                    }
                                    else
                                    {
                                        aiactor.RemoveEffect("BanditRevolverExecute");
                                    }
                                }
                            }
                            
                        }
                    }



                    if (!gun.IsReloading && !HasReloaded)
                    {
                        this.HasReloaded = true;
                    }

                }
            }
               
		}
	}
}
