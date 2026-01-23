using UnityEngine;
using Gungeon;
using ItemAPI;
using SaveAPI;
using System.Collections.Generic;

using System;
using System.Linq;
using Alexandria.Assetbundle;

namespace Planetside
{
    class Riftaker : GunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Riftaker", "riftaker");
            Game.Items.Rename("outdated_gun_mods:riftaker", "psog:riftaker");
            gun.gameObject.AddComponent<Riftaker>();
            gun.SetShortDescription("Breaking the 3rd Wall");
            gun.SetLongDescription("Rips holes in space-time, creating wormholes accessible to projectiles.\n\nGungeons unusual recursive properties, weaponized.");

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "riftakernew_idle_003");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.idleAnimation = "riftakernew_idle";
            gun.shootAnimation = "riftakernew_fire";
            gun.reloadAnimation = "riftakernew_reload";

            // gun.SetupSprite(null, "riftaker_idle_001", 8);
            //gun.SetAnimationFPS(gun.shootAnimation, 25);
            //gun.SetAnimationFPS(gun.idleAnimation, 7);
            //gun.SetAnimationFPS(gun.reloadAnimation,10);

            //gun.emptyReloadAnimation

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(35) as Gun, true, false);
            gun.SetBaseMaxAmmo(88);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_energy_accent_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
            //gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[2].eventAudio = "Play_WPN_plasmacell_reload_01";
            //gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[2].triggerEvent = true;
          
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(328) as Gun).gunSwitchGroup;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SMALL_BLASTER;
            gun.reloadTime = 3.4f;
            gun.DefaultModule.angleVariance = 3f;
            gun.DefaultModule.cooldownTime = .5f;
            gun.DefaultModule.numberOfShotsInClip = 2;
            gun.quality = PickupObject.ItemQuality.B;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.DefaultModule.angleVariance = 0f;
            gun.encounterTrackable.EncounterGuid = "what the dog doin";
            gun.sprite.IsPerpendicular = true;
            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 7f;
            //projectile.transform.parent = gun.barrelOffset;

            gun.barrelOffset = EnemyToolbox.GenerateShootPoint(gun.gameObject, new Vector2(1.25f, -1f), "CHICKEN JOKCEY").transform;
            gun.carryPixelOffset = new IntVector2(2, -2);

            projectile.baseData.range = 12;
            projectile.baseData.force *= 0f;
            Gun gun2 = PickupObjectDatabase.GetById(97) as Gun;
            projectile.hitEffects = gun.DefaultModule.projectiles[0].hitEffects;
            projectile.AdditionalScaleMultiplier *= 1;
            //projectile.gameObject.AddComponent<RiftakerProjectile>();
            //projectile.SetProjectileSpriteRight("bloopky_projectile_001", 15, 7, false, tk2dBaseSprite.Anchor.MiddleCenter, 15, 7);
            projectile.shouldRotate = true;


            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(47) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(47) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            projectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);

            PierceProjModifier spook = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            spook.penetration += 4;

            var riftaker_ = projectile.gameObject.AddComponent<RiftakerProjectile>();
            riftaker_.projectile = projectile;
            riftaker_.In = false;
            projectile.baseData.UsesCustomAccelerationCurve = true;
            projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1f, 1f, 0f);
            projectile.baseData.CustomAccelerationCurveDuration = 0.75f;

            projectile.baseData.speed = 3;

            Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(projectile, "Riftaker_Out", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "Riftaker_Out",
            new List<IntVector2>() { new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), },
            AnimateBullet.ConstructListOfSameValues(true, 15),
            AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.LowerLeft, 15), 
            AnimateBullet.ConstructListOfSameValues(true, 15),
            AnimateBullet.ConstructListOfSameValues(false, 15),
            AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 15),
            AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(1, 1), 15),
            new List<IntVector2?>() { new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), },
            AnimateBullet.ConstructListOfSameValues<Projectile>(null, 15));
            var rec = projectile.AddComponent<RecursionPreventer>();
            rec.ChanceToNotPreventRecursion = 0.05f;

            EnemyToolbox.AddSoundsToAnimationFrame(projectile.sprite.spriteAnimator, "Riftaker_Out", new Dictionary<int, string>()
            {
                //{ 10, "Play_WPN_blackhole_impact_01" }
            });

            //						AkSoundEngine.PostEvent("Play_WPN_blackhole_impact_01", base.gameObject);
            //
            BounceProjModifier bouncy = projectile.gameObject.AddComponent<BounceProjModifier>();
            bouncy.numberOfBounces = 100;

            gun.DefaultModule.usesOptionalFinalProjectile = true;


            gun.gunClass = GunClass.RIFLE;

            gun.sprite.usesOverrideMaterial = true;



            Projectile replacementProjectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(35) as Gun).DefaultModule.projectiles[0]);
            replacementProjectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(replacementProjectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(replacementProjectile);
            var riftaker = replacementProjectile.gameObject.AddComponent<RiftakerProjectile>();
            riftaker.projectile = replacementProjectile;
            riftaker.In = true;

            replacementProjectile.baseData.UsesCustomAccelerationCurve = true;
            replacementProjectile.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1f, 1f, 0f);
            replacementProjectile.baseData.CustomAccelerationCurveDuration = 2.5f;
            replacementProjectile.baseData.speed = 10;

            replacementProjectile.AddComponent<RecursionPreventer>();


            spook = replacementProjectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            spook.penetration += 100;

            bouncy = replacementProjectile.gameObject.AddComponent<BounceProjModifier>();
            bouncy.numberOfBounces = 100;

            Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(replacementProjectile, "Riftaker_In", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "Riftaker_In",
            new List<IntVector2>() { new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36),  },
            AnimateBullet.ConstructListOfSameValues(true, 13),
            AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.LowerLeft, 13),
            AnimateBullet.ConstructListOfSameValues(true, 13),
            AnimateBullet.ConstructListOfSameValues(false, 13),
            AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 13),
            AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(1, 1), 13),
            new List<IntVector2?>() { new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16),  },
            AnimateBullet.ConstructListOfSameValues<Projectile>(null, 13));


            Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(replacementProjectile, "Riftaker_In", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "Riftaker_Detonate",
            new List<IntVector2>() { new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), new IntVector2(36, 36), },
            AnimateBullet.ConstructListOfSameValues(true, 18),
            AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.LowerLeft, 18),
            AnimateBullet.ConstructListOfSameValues(true, 18),
            AnimateBullet.ConstructListOfSameValues(false, 18),
            AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 18),
            AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(1, 1), 18),
            new List<IntVector2?>() { new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), new IntVector2(16, 16), },
            AnimateBullet.ConstructListOfSameValues<Projectile>(null, 18));



            gun.DefaultModule.usesOptionalFinalProjectile = true;
            gun.DefaultModule.numberOfFinalProjectiles = 1;
            gun.DefaultModule.finalProjectile = replacementProjectile;
            gun.DefaultModule.finalCustomAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Riftaker(Out)", StaticSpriteDefinitions.PlanetsideClipUIAtlas, "RiftakerOut_001", "RiftakerOut_002");
            gun.DefaultModule.finalAmmoType = (PickupObjectDatabase.GetById(696) as Gun).DefaultModule.ammoType;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = (PickupObjectDatabase.GetById(35) as Gun).DefaultModule.customAmmoType;


            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Riftaker(In)", StaticSpriteDefinitions.PlanetsideClipUIAtlas, "RiftakerIn_001", "RiftakerIn_002");



            Projectile dart = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(35) as Gun).DefaultModule.projectiles[0]);
            dart.gameObject.SetActive(false);
            dart.AddComponent<RecursionPreventer>();

            FakePrefab.MarkAsFakePrefab(dart.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(dart);
            var riftaker_dart = dart.gameObject.AddComponent<RiftakerProjectile.RiftakerAffectedProjectile>();
            riftaker_dart.Projectile = dart;

            //dart.baseData.UsesCustomAccelerationCurve = true;
            //dart.baseData.AccelerationCurve = AnimationCurve.Linear(0.5f, 1f, 0.5f, 0.2f);

            spook = dart.gameObject.GetOrAddComponent<PierceProjModifier>();
            spook.penetration += 10;

            bouncy = dart.gameObject.AddComponent<BounceProjModifier>();
            bouncy.numberOfBounces = 10;

            dart.baseData.damage = 12;
            dart.baseData.speed = 12.5f;
            dart.baseData.force *= 0.1f;

            dart.shouldRotate = true;
            Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(dart, "Riftaker_Dart", StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, "Riftaker_Dart",
            new List<IntVector2>() { new IntVector2(12, 5), new IntVector2(12, 5), new IntVector2(12, 5), new IntVector2(12, 5), new IntVector2(12, 5), new IntVector2(12, 5), },
            AnimateBullet.ConstructListOfSameValues(true, 6),
            AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.LowerLeft, 6),
            AnimateBullet.ConstructListOfSameValues(true, 6),
            AnimateBullet.ConstructListOfSameValues(false, 6),
            AnimateBullet.ConstructListOfSameValues<Vector3?>(new Vector2(0, 0), 6),
            AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 6),
            AnimateBullet.ConstructListOfSameValues<IntVector2?>(new IntVector2(0, 0), 6),
            AnimateBullet.ConstructListOfSameValues<Projectile>(null, 6));

            ImprovedAfterImage yes = dart.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.1875f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0f, 0.2f, 1f, 1.5f);

            DartProjectile = dart;

            //Riftaker_In

            gun.gameObject.transform.Find("Clip").transform.position = new Vector3(1.75f, 0.5f);
            gun.clipObject = BreakAbleAPI.BreakableAPI_Bundled.GenerateDebrisObject("riftakernew_front_001", StaticSpriteDefinitions.Gun_Sheet_Data).gameObject;
            gun.reloadClipLaunchFrame = 6;
            gun.clipsToLaunchOnReload = 1;


            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(110, 250, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 8.5f);
            mat.SetFloat("_EmissivePower", 15);
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


            ETGMod.Databases.Items.Add(gun, false, "ANY");
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:riftaker",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "black_hole_gun",
                "singularity",
                "psog:immateria"
            };
            CustomSynergies.Add("Event Horizon", mandatoryConsoleIDs, optionalConsoleIDs, false);
            Riftaker.RiftakerID = gun.PickupObjectId;
            ItemIDs.AddToList(gun.PickupObjectId);




        }
        public static int RiftakerID;

        public static Projectile DartProjectile;
        private bool HasReloaded;

        public override void Update()
        {
            PlayerController player = gun.CurrentOwner as PlayerController;
            if (gun.CurrentOwner)
            {
                if (gun.CurrentOwner)
                {

                    if (!gun.IsReloading && !HasReloaded)
                    {
                        this.HasReloaded = true;
                    }
                }
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                base.OnReloadPressed(player, gun, bSOMETHING);
            }
            
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
        }

    }
}