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

namespace Planetside
{
	public class PurplePain : GunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("tets", "knightsword");
			Game.Items.Rename("outdated_gun_mods:tets", "psog:tets");
			gun.gameObject.AddComponent<PurplePain>();
			gun.SetShortDescription("Reaped By Death");
			gun.SetLongDescription("A simple, elegant and powerful revolver, capable of killing even behind cover.\n\nWielded by a particularly regretful undead Gungeoneer seeking an old friend...");
			GunExt.SetupSprite(gun, null, "knightsword_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 8);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 7);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 1);
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
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 30f;
			projectile.baseData.speed *= 3f;
			projectile.AdditionalScaleMultiplier *= 0.75f;
			projectile.shouldRotate = true;
			projectile.pierceMinorBreakables = true;
			projectile.PenetratesInternalWalls = true;
			gun.IsHeroSword = true;
			gun.HeroSwordDoesntBlank = false;
			PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
			spook.penetration = 3;
			//projectile.baseData.range = 5.8f;
			gun.encounterTrackable.EncounterGuid = "ree";
            ETGMod.Databases.Items.Add(gun, false, "ANY");

        }
        public override void PostProcessProjectile(Projectile projectile)
		{
		}


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





/*


using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{

    public class Baba : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Baba Yaga", "pbpp");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:baba_yaga", "ski:baba_yaga");
            gun.gameObject.AddComponent<Baba>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Para Bellum");
            gun.SetLongDescription("A pocket pistol bloodied by years of contract killing. When it's owner quit the trade this gun was lost to the gungeon. When reloading hitting an enemy will steal ammo from them. Luckily, the ammo is universal." +
                "\n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "pbpp_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 36);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = .1f;
            gun.DefaultModule.numberOfShotsInClip = 9;
            gun.SetBaseMaxAmmo(36);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            gun.encounterTrackable.EncounterGuid = "I guess I'm back";
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            //projectile.baseData allows you to modify the base properties of your projectile module.
            //In our case, our gun uses modified projectiles from the ak-47.
            //Setting static values for a custom gun's projectile stats prevents them from scaling with player stats and bullet modifiers (damage, shotspeed, knockback)
            //You have to multiply the value of the original projectile you're using instead so they scale accordingly. For example if the projectile you're using as a base has 10 damage and you want it to be 6 you use this
            //In our case, our projectile has a base damage of 5.5, so we multiply it by 1.1 so it does 10% more damage from the ak-47.
            projectile.baseData.damage *= 1f;
            projectile.baseData.speed *= 1f;
            projectile.transform.parent = gun.barrelOffset;

            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);

        }
        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
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
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_cata_reload_sfx", base.gameObject);


            }

            if (gun.ClipShotsRemaining < gun.ClipCapacity)
            {
                Projectile projectile1 = ((Gun)ETGMod.Databases.Items[3]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                component.baseData.damage = 3f;
                component.Owner = player;
                ProjectileSlashingBehaviour slasher = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashRange = 4f;
                slasher.SlashDimensions = 5;
                slasher.SlashVFX = swipeFlash.muzzleFlashEffects;
                slasher.DoSound = false;

                routinedoer(player);
            }
        }
        public void routinedoer(PlayerController player)
        {
            Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;

            GameManager.Instance.StartCoroutine(this.HandleSwing(player, vector, .001f, 4f));
        }

        private IEnumerator HandleSwing(PlayerController user, Vector2 aimVec, float rayDamage, float rayLength)
        {
            float elapsed = 0f;
            yield return new WaitForSecondsRealtime(.01f);
            while (elapsed < 1)
            {

                elapsed += BraveTime.DeltaTime;
                SpeculativeRigidbody hitRigidbody = this.IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
                if (hitRigidbody != null)
                {
                    AiactorSpecialStates state = hitRigidbody.aiActor.gameObject.GetOrAddComponent<AiactorSpecialStates>();
                    if (hitRigidbody && hitRigidbody.aiActor && hitRigidbody.aiActor.IsNormalEnemy && state.LootedByBaba == false)
                    {
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(78).gameObject, hitRigidbody.sprite.WorldCenter, aimVec * -1, 5f, false, false, false);
                        state.LootedByBaba = true;
                        hitRigidbody.aiActor.behaviorSpeculator.Stun(1f, true);
                        yield break;
                    }

                }


            }
            yield break;
        }

        public SpeculativeRigidbody IterativeRaycast(Vector2 rayOrigin, Vector2 rayDirection, float rayDistance, int collisionMask, SpeculativeRigidbody ignoreRigidbody)
        {
            int num = 0;
            RaycastResult raycastResult;
            while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out raycastResult, true, true, collisionMask, new CollisionLayer?(CollisionLayer.BulletBlocker), false, null, ignoreRigidbody))
            {
                num++;
                SpeculativeRigidbody speculativeRigidbody = raycastResult.SpeculativeRigidbody;
                if (num < 3 && speculativeRigidbody != null)
                {
                    MinorBreakable component = speculativeRigidbody.GetComponent<MinorBreakable>();
                    if (component != null)
                    {

                        component.Break(rayDirection.normalized * 3f);
                        RaycastResult.Pool.Free(ref raycastResult);
                        continue;
                    }
                }
                RaycastResult.Pool.Free(ref raycastResult);
                return speculativeRigidbody;
            }
            return null;
        }
    }
}
*/

/*

namespace Items
{
    class AK94 : AdvancedGunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("AK-94", "ak_94");
            Game.Items.Rename("outdated_gun_mods:ak-94", "cel:ak-94");
            gun.gameObject.AddComponent<AK94>();
            gun.SetShortDescription("Accept No SuuS oN tpeccA");
            gun.SetLongDescription("Some idiot decided to create this affront against God by taping two AK-47's together.");
            gun.SetupSprite(null, "ak_94_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 9);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            gun.AddProjectileModuleFrom("ak-47", true, false);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SMALL_BULLET;
            gun.DefaultModule.ammoCost = 2;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.angleVariance = 4;
            gun.DefaultModule.cooldownTime = .11f;
            gun.DefaultModule.numberOfShotsInClip = 60;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(15) as Gun).gunSwitchGroup;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(15) as Gun).muzzleFlashEffects;
            gun.SetBaseMaxAmmo(1000);
            gun.quality = PickupObject.ItemQuality.B;
            gun.encounterTrackable.EncounterGuid = "reverse, reverse";
            gun.sprite.IsPerpendicular = true;
            gun.barrelOffset.transform.localPosition = new Vector3(2.25f, 0.3125f, 0f);
            gun.gunClass = GunClass.FULLAUTO;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.transform.parent = gun.barrelOffset;
            projectile.baseData.damage *= 1f;
            projectile.baseData.speed *= 1f;
            projectile.baseData.force *= 1f;


            ETGMod.Databases.Items.Add(gun, null, "ANY");
            AK94.AK94ID = gun.PickupObjectId;
        }
        public static int AK94ID;

        private bool HasReloaded;
        private float Tracker = 0;
        public override void Update()
        {
            base.Update();
            if (gun.CurrentOwner)
            {

                if (gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
        }
        public override void OnPickup(GameActor owner)
        {
            base.OnPickup(owner);
            (owner as PlayerController).OnKilledEnemy += this.Transforming;
        }
        public override void OnPostDrop(GameActor owner)
        {
            base.OnPostDrop(owner);
            (owner as PlayerController).OnKilledEnemy -= this.Transforming;
        }

        private void Transforming(PlayerController player)
        {
            if (player != null)
            {
                this.Tracker++;
                if (Tracker >= 30)
                {
                    Gun ak94 = PickupObjectDatabase.GetById(AK94.AK94ID) as Gun;
                    Gun ak141 = PickupObjectDatabase.GetById(AK141.AK141ID) as Gun;

                    player.inventory.AddGunToInventory(ak141, true);
                    player.inventory.DestroyGun(ak94);
                }
            }
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

        private float revAngle = 180;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            float v1 = UnityEngine.Random.Range(-4f, 4f);
            Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile2.gameObject, base.Owner.CurrentGun.transform.position, Quaternion.Euler(0f, 0f, (base.Owner.CurrentGun == null) ? 0f : base.Owner.CurrentGun.CurrentAngle + revAngle + v1), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            if (component2 != null)
            {
                component2.Owner = base.Owner;
                component2.Shooter = base.Owner.specRigidbody;
                component2.baseData.speed *= player.stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                component2.baseData.force *= player.stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
                component2.baseData.damage *= player.stats.GetStatValue(PlayerStats.StatType.Damage);
                player.DoPostProcessProjectile(component2);
            }

        }

        public AK94()
        {

        }
    }
}


namespace Items
{
    class AK141 : AdvancedGunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("AK-141", "ak_141");
            Game.Items.Rename("outdated_gun_mods:ak-141", "cel:ak-141");
            gun.gameObject.AddComponent<AK141>();
            gun.SetShortDescription("What The Hell?");
            gun.SetLongDescription("How does it even work? DOES it work? How are you supposed to reload it?");
            gun.SetupSprite(null, "ak_141_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 9);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            gun.AddProjectileModuleFrom("ak-47", true, false);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SMALL_BULLET;
            gun.DefaultModule.ammoCost = 3;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.angleVariance = 4f;
            gun.DefaultModule.cooldownTime = .06f;
            gun.DefaultModule.numberOfShotsInClip = 90;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(15) as Gun).gunSwitchGroup;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(15) as Gun).muzzleFlashEffects;
            gun.SetBaseMaxAmmo(1500);
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.encounterTrackable.EncounterGuid = "why am i doing this";
            gun.sprite.IsPerpendicular = true;
            gun.barrelOffset.transform.localPosition = new Vector3(2.25f, 0.3125f, 0f);
            gun.gunClass = GunClass.FULLAUTO;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.transform.parent = gun.barrelOffset;
            projectile.baseData.damage *= 1f;
            projectile.baseData.speed *= 1f;
            projectile.baseData.force *= 1f;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            AK141.AK141ID = gun.PickupObjectId;
        }
        private bool HasReloaded;
        public static int AK141ID;

        public override void Update()
        {
            base.Update();
            if (gun.CurrentOwner)
            {

                if (gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
        }
        private float Tracker = 0;
        public override void OnPickup(GameActor owner)
        {
            base.OnPickup(owner);
            (owner as PlayerController).OnKilledEnemy += this.Transforming;


            Gun ak94 = PickupObjectDatabase.GetById(AK94.AK94ID) as Gun;

            if ((owner as PlayerController).HasGun(AK94.AK94ID))
            {
                (owner as PlayerController).inventory.DestroyGun(ak94);
            }
        }
        public override void OnPostDrop(GameActor owner)
        {
            base.OnPostDrop(owner);
            (owner as PlayerController).OnKilledEnemy -= this.Transforming;
        }
        private void Transforming(PlayerController player)
        {
            if (player != null)
            {
                this.Tracker++;
                if (Tracker >= 45)
                {
                    Gun ak141 = PickupObjectDatabase.GetById(AK141.AK141ID) as Gun;
                    Gun ak188 = PickupObjectDatabase.GetById(AK188.AK188ID) as Gun;

                    player.inventory.AddGunToInventory(ak188, true);
                    player.inventory.DestroyGun(ak141);
                }
            }
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

        private float revAngle = 180;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            for (int i = 0; i < 2; i++)
            {
                float v1 = UnityEngine.Random.Range(-4f, 4f);
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile2.gameObject, base.Owner.CurrentGun.transform.position, Quaternion.Euler(0f, 0f, (base.Owner.CurrentGun == null) ? 0f : base.Owner.CurrentGun.CurrentAngle + (i * 90) + v1 + 90), true);
                Projectile component2 = gameObject2.GetComponent<Projectile>();
                if (component2 != null)
                {
                    component2.Owner = base.Owner;
                    component2.Shooter = base.Owner.specRigidbody;
                    component2.baseData.speed *= player.stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                    component2.baseData.force *= player.stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
                    component2.baseData.damage *= player.stats.GetStatValue(PlayerStats.StatType.Damage);
                    player.DoPostProcessProjectile(component2);
                }
            }

        }

        public AK141()
        {

        }
    }
}


namespace Items
{
    class AK188 : AdvancedGunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("AK-188", "ak_188");
            Game.Items.Rename("outdated_gun_mods:ak-188", "cel:ak-188");
            gun.gameObject.AddComponent<AK188>();
            gun.SetShortDescription("No God Can Help You");
            gun.SetLongDescription("Good luck, kid.");
            gun.SetupSprite(null, "ak_188_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 9);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            gun.AddProjectileModuleFrom("ak-47", true, false);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SMALL_BULLET;
            gun.DefaultModule.ammoCost = 4;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2f;
            gun.CanBeDropped = false;
            gun.CanBeSold = false;
            gun.DefaultModule.angleVariance = 4f;
            gun.DefaultModule.cooldownTime = .06f;
            gun.DefaultModule.numberOfShotsInClip = 120;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(15) as Gun).gunSwitchGroup;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(15) as Gun).muzzleFlashEffects;
            gun.SetBaseMaxAmmo(2000);
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.encounterTrackable.EncounterGuid = "what the actual fuck";
            gun.sprite.IsPerpendicular = true;
            gun.barrelOffset.transform.localPosition = new Vector3(2.25f, 0.3125f, 0f);
            gun.gunClass = GunClass.FULLAUTO;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.transform.parent = gun.barrelOffset;
            projectile.baseData.damage *= 1f;
            projectile.baseData.speed *= 1f;
            projectile.baseData.force *= 1f;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            AK188.AK188ID = gun.PickupObjectId;
        }
        public static int AK188ID;
        private bool HasReloaded;

        public override void Update()
        {
            base.Update();
            if (gun.CurrentOwner)
            {

                if (gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
        }
        private float Tracker = 0;
        public override void OnPickup(GameActor owner)
        {
            base.OnPickup(owner);

            Gun ak141 = PickupObjectDatabase.GetById(AK141.AK141ID) as Gun;

            (owner as PlayerController).OnKilledEnemy += this.Transforming;
            if ((owner as PlayerController).HasGun(ak141.PickupObjectId))
            {
                (owner as PlayerController).inventory.DestroyGun(ak141);
            }
        }
        public override void OnPostDrop(GameActor owner)
        {
            base.OnPostDrop(owner);
            (owner as PlayerController).OnKilledEnemy -= this.Transforming;
        }
        private void Transforming(PlayerController player)
        {
            if (player != null)
            {
                this.Tracker++;
                if (Tracker >= 55)
                {
                    Gun ak188 = PickupObjectDatabase.GetById(AK188.AK188ID) as Gun;
                    Gun akinfinity = PickupObjectDatabase.GetById(InfiniteAK.AKINFID) as Gun;

                    player.inventory.AddGunToInventory(akinfinity, true);
                    player.inventory.DestroyGun(ak188);
                }
            }

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

        private float revAngle = 180;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            for (int i = 0; i < 3; i++)
            {
                float v1 = UnityEngine.Random.Range(-4f, 4f);
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile2.gameObject, base.Owner.CurrentGun.transform.position, Quaternion.Euler(0f, 0f, (base.Owner.CurrentGun == null) ? 0f : base.Owner.CurrentGun.CurrentAngle + (i * 90) + v1 + 90), true);
                Projectile component2 = gameObject2.GetComponent<Projectile>();
                if (component2 != null)
                {
                    component2.Owner = base.Owner;
                    component2.Shooter = base.Owner.specRigidbody;
                    component2.baseData.speed *= player.stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                    component2.baseData.force *= player.stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
                    component2.baseData.damage *= player.stats.GetStatValue(PlayerStats.StatType.Damage);
                    player.DoPostProcessProjectile(component2);
                }
            }
        }

        public AK188()
        {

        }
    }
}


namespace Items
{
    class InfiniteAK : AdvancedGunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Infinite AK", "infinite_ak");
            Game.Items.Rename("outdated_gun_mods:infinite_ak", "cel:infinite_ak");
            gun.gameObject.AddComponent<InfiniteAK>();
            gun.SetShortDescription("Witness Perfection");
            gun.SetLongDescription("Perfect, brilliant. This gun might not be the strongest around, but it is the most refined. Every aspect of it is flawless. Each bullet it shoots is symmetrical and expertly crafted. Be grateful to witness such beauty.");
            gun.SetupSprite(null, "infinite_ak_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 9);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            for (int i = 0; i < 1; i++)
            {
                GunExt.AddProjectileModuleFrom(gun, "ak-47", true, false);
            }
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectileModule.projectiles[0] = projectile;
                projectileModule.angleVariance = 4;
                projectile.transform.parent = gun.barrelOffset;
                gun.DefaultModule.projectiles[0] = projectile;
                projectile.baseData.damage *= 1.5f;
                projectile.baseData.speed *= 2f;
                projectileModule.numberOfShotsInClip = 1;
                projectileModule.cooldownTime = .08f;
                projectile.ignoreDamageCaps = true;
                projectileModule.ammoType = GameUIAmmoType.AmmoType.SMALL_BULLET;

                bool flag = projectileModule == gun.DefaultModule;
                if (flag)
                {
                    projectileModule.ammoCost = 1;
                }
                else
                {
                    projectileModule.ammoCost = 0;
                }

            }

            gun.reloadTime = -1f;
            gun.CanBeDropped = false;
            gun.CanBeSold = false;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.InfiniteAmmo = true;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(15) as Gun).gunSwitchGroup;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(15) as Gun).muzzleFlashEffects;
            gun.SetBaseMaxAmmo(2000);
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.encounterTrackable.EncounterGuid = "the condition, state, or quality of being free or as free as possible from all flaws or defects.";
            gun.sprite.IsPerpendicular = true;
            gun.barrelOffset.transform.localPosition = new Vector3(2.25f, 0.3125f, 0f);
            gun.gunClass = GunClass.FULLAUTO;

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            InfiniteAK.AKINFID = gun.PickupObjectId;
        }
        public static int AKINFID;
        private bool HasReloaded;

        public override void Update()
        {
            base.Update();
            if (gun.CurrentOwner)
            {

                if (gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);

        }
        public override void OnPickup(GameActor owner)
        {
            base.OnPickup(owner);

            Gun ak188 = PickupObjectDatabase.GetById(AK188.AK188ID) as Gun;

            if ((owner as PlayerController).HasGun(ak188.PickupObjectId))
            {
                (owner as PlayerController).inventory.DestroyGun(ak188);
            }
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

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            PlayerController x = this.gun.CurrentOwner as PlayerController;
            bool flag = x == null;
            bool flag2 = flag;
            if (flag2)
            {
                this.gun.ammo = this.gun.GetBaseMaxAmmo();
            }
            this.gun.ClipShotsRemaining = 2;
            this.gun.GainAmmo(2);
            gun.ClearReloadData();

        }

        public InfiniteAK()
        {

        }
    }
}
*/