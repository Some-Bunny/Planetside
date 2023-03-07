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
using HutongGames.PlayMaker.Actions;


namespace Planetside
{
	public class WitherLance : GunBehaviour
	{
		// FINISH GUN
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Heavens Call", "magicstaffofpower_idle_001");
			Game.Items.Rename("outdated_gun_mods:heavens_call", "psog:heavens_call");
			gun.gameObject.AddComponent<WitherLance>();
			GunExt.SetShortDescription(gun, "Star God");
			GunExt.SetLongDescription(gun, "Fires a barrage of weak stars that attach to own own projectiles, buffing them and themselves.\n\n");
            //GunExt.SetupSprite(gun, null, "magicstaffofpower_idle_001", 11);

            GunExt.SetupSprite(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "magicstaffofpower_idle_001", 11);
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.idleAnimation = "magicstaff_idle";
            gun.shootAnimation = "magicstaff_fire";
            gun.reloadAnimation = "magicstaff_reload";

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_PET_junk_spin_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(52) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2.1f;
			gun.DefaultModule.cooldownTime = .05f;
			gun.DefaultModule.numberOfShotsInClip = 100;
			gun.SetBaseMaxAmmo(2000);
			gun.ammo = 2000;
            //gun.additionalHandState = AdditionalHandState.HidePrimary;


            gun.quality = PickupObject.ItemQuality.A;
			gun.DefaultModule.angleVariance = 17f;
			gun.DefaultModule.burstShotCount = 1;
			gun.DefaultModule.projectiles.Clear();
			gun.DefaultModule.projectiles.Add(AddProjectile("spark_blue_idle", 2.5f, 5.5f, new Color(0, 10, 200), WitherLanceProjectile.Types.NORMAL, 223));
			
			var green = AddProjectile("spark_green_idle", 2f, 6f, new Color(1, 200, 10), WitherLanceProjectile.Types.GREEN, 504);
			var greenHoming = green.GetComponent<HomingModifier>();
			greenHoming.AngularVelocity += 120;
			greenHoming.HomingRadius += 10;
			gun.DefaultModule.projectiles.Add(green);

			var yellow = AddProjectile("spark_yellow_idle", 3f, 10f, new Color(255, 34, 0), WitherLanceProjectile.Types.FAST, 95, 2, 4, 7);
			yellow.GetComponent<PierceProjModifier>().penetration += 5;
            gun.DefaultModule.projectiles.Add(yellow);

            var pink = AddProjectile("spark_pink_idle", 5f, 5f, new Color(255, 150, 150), WitherLanceProjectile.Types.SPARKLY, 123, 3, 1, 2);
            gun.DefaultModule.projectiles.Add(pink);

            RedStar = AddProjectile("spark_red_idle", 3f, 7f, new Color(255, 10, 10), WitherLanceProjectile.Types.BLAST, 125, 1, 2, 6);
            ExplosiveModifier explosiveModifier = RedStar.gameObject.GetOrAddComponent<ExplosiveModifier>();
            explosiveModifier.explosionData = new ExplosionData()
            {
                breakSecretWalls = false,
                comprehensiveDelay = 0,
                damage = 3,
                damageRadius = 2f,
                damageToPlayer = 0,
                debrisForce = 10,
                doDamage = true,
                doDestroyProjectiles = false,
                doExplosionRing = false,
                doForce = true,
                doScreenShake = false,
                doStickyFriction = false,
                effect = (PickupObjectDatabase.GetById(328) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.overrideMidairDeathVFX,
                explosionDelay = 0,
                force = 2,
                forcePreventSecretWallDamage = false,
                forceUseThisRadius = true,
                freezeEffect = null,
                freezeRadius = 0,
                IsChandelierExplosion = false,
                isFreezeExplosion = false,
                playDefaultSFX = false,
                preventPlayerForce = false,
                pushRadius = 1,
                secretWallsRadius = 1,

            };
            explosiveModifier.doExplosion = true;
            explosiveModifier.IgnoreQueues = true;

            gun.DefaultModule.projectiles.Add(RedStar);

            gun.gunClass = GunClass.FULLAUTO;

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Wither Lance", "Planetside/Resources/GunClips/WitherLance/lancefull", "Planetside/Resources/GunClips/WitherLance/lanceempty");

            gun.barrelOffset.transform.localPosition = new Vector3(2.4375f, 0.375f, 0f);


            //projectile.baseData.range = 5.8f;
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
			gun.encounterTrackable.EncounterGuid = "Malachite Elite go BWOOOOOOOOOOOOOOOOOM";
			ETGMod.Databases.Items.Add(gun, false, "ANY");
			gun.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);
			WitherLance.WitherLanceID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);

            //make sure the animation name and variable names are correct, the program may have made the wrong decision 
            // it is better to be getting your clips like so "gun.sprite.spriteAnimator.GetClipByName(gun.shootAnimation);" and vary the animation name of coursetk2dSpriteAnimationClip animationclip = gun.sprite.spriteAnimator.GetClipByName(magicstaffofpower_reload_004);
            float[] offsetsX = new float[] { 0f, 0f, 0f, 0f, 0f, 0f};
            float[] offsetsY = new float[] { 0f, 0f, -1.5f, -1.5f, 0f, 0f, };
            var animationclip = gun.spriteAnimator.GetClipByName(gun.reloadAnimation);
            for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < animationclip.frames.Length; i++) 
            { int id = animationclip.frames[i].spriteId; animationclip.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX[i]; 
                animationclip.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY[i]; animationclip.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX[i]; 
                animationclip.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY[i]; animationclip.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX[i]; 
                animationclip.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY[i]; animationclip.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX[i]; 
                animationclip.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY[i]; 
            }

            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:heavens_call",
            };
            List<string> BlessedSynergy = new List<string>
            {
                "mourning_star"
            };
            CustomSynergies.Add("Cry Of The Sun", mandatoryConsoleIDs, BlessedSynergy, false);
            new Hook(typeof(HammerOfDawnController).GetMethod("ApplyBeamTickToEnemiesInRadius", BindingFlags.Instance | BindingFlags.NonPublic), typeof(WitherLance).GetMethod("ApplyBeamTickToEnemiesInRadiusHook"));

        }
        public static void ApplyBeamTickToEnemiesInRadiusHook(Action<HammerOfDawnController> orig, HammerOfDawnController self)
        {
            orig(self);
            Vector2 vector = self.transform.position.XY();
            RoomHandler absoluteRoom = vector.GetAbsoluteRoom();
            if (absoluteRoom == null)
            {
                return;
            }
            List<AIActor> activeEnemies = absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (activeEnemies == null)
            {
                return;
            }
            if (self.m_owner)
            {
                if (self.m_owner.PlayerHasActiveSynergy("Cry Of The Sun"))
                {
                    if (UnityEngine.Random.value < 0.05f)
                    {
                        GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(RedStar.gameObject, self.sprite.WorldCenter, Quaternion.Euler(0f, 0f, BraveUtility.RandomAngle()), true);
                        Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                        if (component != null)
                        {
                            component.Owner = self.m_owner;
                            component.Shooter = self.m_owner.specRigidbody;
                        }
                    }
                }
            }
        }


        public static Projectile RedStar;

		private static Projectile AddProjectile(string animationName, float Damage, float Speed, Color color, WitherLanceProjectile.Types type, int GunHitEffect, float AdditionalDamage = 1, float SpeedMinimum = 2, float SpeedMaximum = 4)
		{
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);

            projectile.baseData.damage = Damage;
            projectile.baseData.speed = Speed;
            projectile.baseData.range = 125;

            projectile.shouldRotate = false;
            projectile.pierceMinorBreakables = true;
            projectile.HasDefaultTint = true;
            var ef = projectile.gameObject.AddComponent<WitherLanceProjectile>();
            ef.ownType = type;
            ef.AdditionalDamage = AdditionalDamage;
			ef.SpeedMinimum = SpeedMinimum;
            ef.SpeedMaximum = SpeedMaximum;
            projectile.shouldRotate = false;

            projectile.baseData.UsesCustomAccelerationCurve = true;
            projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1, 0.6f, 0.1f);

            projectile.AnimateProjectileBundle(animationName, StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, animationName,
			new List<IntVector2>() { new IntVector2(7, 7), new IntVector2(7, 7), new IntVector2(7, 7), new IntVector2(7, 7), new IntVector2(7, 7) },
			AnimateBullet.ConstructListOfSameValues(true, 5), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.LowerLeft, 5), AnimateBullet.ConstructListOfSameValues(true, 5), AnimateBullet.ConstructListOfSameValues(false, 5),
			AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 5), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 5), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 5), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 5));


            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(61) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(61) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;
            

            projectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(GunHitEffect) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(GunHitEffect) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(GunHitEffect) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(GunHitEffect) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);

            HomingModifier HomingMod = projectile.gameObject.GetOrAddComponent<HomingModifier>();
            HomingMod.AngularVelocity = 240;
            HomingMod.HomingRadius = 10;

            PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
            spook.penetration = 3;
            spook.penetratesBreakables = true;

            var tro = projectile.gameObject.AddChild("trail object");
            tro.transform.position = projectile.sprite.WorldCenter + new Vector2(0, -0.0625f);
            tro.transform.localPosition = projectile.sprite.WorldCenter + new Vector2(0, -0.0625f);

            TrailRenderer tr = tro.AddComponent<TrailRenderer>();
            tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tr.receiveShadows = false;
            var mat = new Material(Shader.Find("Sprites/Default"));
            tr.material = mat;
            tr.minVertexDistance = 0.01f;
            tr.numCapVertices = 20;

            //======
            mat.SetColor("_Color", color * 0.7f);
            tr.startColor = Color.white;
            tr.endColor = color * 0.7f;
            //======
            tr.time = 0.1f;
            //======
            tr.startWidth = 0.25f;
            tr.endWidth = 0f;
            tr.autodestruct = false;

            var rend = projectile.gameObject.AddComponent<ProjectileTrailRendererController>();
            rend.trailRenderer = tr;
            rend.desiredLength = 0.625f;

            return projectile;
        }
		public static int WitherLanceID;
	}
}