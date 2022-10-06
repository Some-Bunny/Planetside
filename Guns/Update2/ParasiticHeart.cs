using UnityEngine;
using Gungeon;
using ItemAPI;
using SaveAPI;
using System.Collections.Generic;


namespace Planetside
{
    class ParasiticHeart : GunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Parasitic Heart", "chambersoul");
            Game.Items.Rename("outdated_gun_mods:parasitic_heart", "psog:parasitic_heart");
            gun.gameObject.AddComponent<ParasiticHeart>();
            gun.SetShortDescription("Detached");
            gun.SetLongDescription("The heart of a parasitic beast that lurks, and once lurked in the Gungeon. The six souls it once housed are still yet to depart, and orbit the ever-beating heart.");
            gun.SetupSprite(null, "chambersoul_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.idleAnimation, 7);
            gun.SetAnimationFPS(gun.reloadAnimation,9);

            gun.AddProjectileModuleFrom("38_special", true, false);
            gun.SetBaseMaxAmmo(150);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[2].eventAudio = "Play_Heartbeat";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[2].triggerEvent = true;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation).frames[2].eventAudio = "Play_Heartbeat";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation).frames[2].triggerEvent = true;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[3].eventAudio = "Play_Heartbeat";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[3].triggerEvent = true;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(33) as Gun).gunSwitchGroup;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.NAIL;
            gun.reloadTime = 3.33f;
            gun.DefaultModule.cooldownTime = .25f;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.numberOfShotsInClip = 25;
            gun.quality = PickupObject.ItemQuality.B;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.DefaultModule.angleVariance = 0f;
            gun.encounterTrackable.EncounterGuid = "hert";
            gun.sprite.IsPerpendicular = true;
            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage *= 0f;
            projectile.transform.parent = gun.barrelOffset;
            projectile.baseData.range *= 0;
            projectile.sprite.renderer.enabled = false;
            projectile.hitEffects.suppressMidairDeathVfx = true;
            projectile.AdditionalScaleMultiplier *= 0f;


            gun.gunClass = GunClass.NONE;

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.CONTRAIL);
            ParasiticHeart.HeartID = gun.PickupObjectId;
            gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER, true);

            gun.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);

            ItemIDs.AddToList(gun.PickupObjectId);
        }
        public static int HeartID;

        private bool HasReloaded;

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

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Play_ENM_beholster_intro_01", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                if (gun.ClipShotsRemaining == 0 && gun.CurrentAmmo != 0)
                {
                    var list = new List<string>
                {
                "8bb5578fba374e8aae8e10b754e61d62",
                //"b67ffe82c66742d1985e5888fd8e6a03",
                "ec8ea75b557d4e7b8ceeaacdf6f8238c",
                "b54d89f9e802455cbb2b8a96a31e8259",
                "skullvenant"
                };
                    string guid = BraveUtility.RandomElement<string>(list);
                    var Enemy = EnemyDatabase.GetOrLoadByGuid(guid);
                    AIActor mhm = AIActor.Spawn(Enemy.aiActor, player.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Default, true);
                    CompanionController yup = mhm.gameObject.AddComponent<CompanionController>();
                    yup.companionID = CompanionController.CompanionIdentifier.NONE;
                    yup.Initialize(player);

                    Planetside.OtherTools.CompanionisedEnemyBulletModifiers yeehaw = mhm.gameObject.AddComponent<Planetside.OtherTools.CompanionisedEnemyBulletModifiers>();
                    yeehaw.jammedDamageMultiplier *= 3;
                    yeehaw.baseBulletDamage = 10f;
                    yeehaw.TintBullets = true;
                    yeehaw.TintColor = new Color(1f, 1f, 1f);
                    mhm.gameObject.AddComponent<FriendlyGhostEnemyComponent>();
                }
            }   
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
        }
    }
}