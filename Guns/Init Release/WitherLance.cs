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
using Alexandria.Assetbundle;
using Planetside.Toolboxes;
using HarmonyLib;

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
			GunExt.SetLongDescription(gun, "Creates a barrage of micro-stars that attach to your projectiles, buffing them and themselves.\n\n");
            //GunExt.SetupSprite(gun, null, "magicstaffofpower_idle_001", 11);

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_Sheet_Data, "magicstaffofpower_idle_001");
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
			gun.DefaultModule.projectiles.Add(AddProjectile("spark_blue_idle", 5f, 5.5f, new Color(0, 10, 200), WitherLanceProjectile.Types.NORMAL, 223));
			
			var green = AddProjectile("spark_green_idle", 3.5f, 6f, new Color(1, 200, 10), WitherLanceProjectile.Types.GREEN, 504);
			var greenHoming = green.GetComponent<HomingModifier>();
			greenHoming.AngularVelocity += 120;
			greenHoming.HomingRadius += 10;
			gun.DefaultModule.projectiles.Add(green);

			var yellow = AddProjectile("spark_yellow_idle", 4.5f, 10f, new Color(255, 34, 0), WitherLanceProjectile.Types.FAST, 95, 2, 4, 7);
			yellow.GetComponent<PierceProjModifier>().penetration += 5;
            gun.DefaultModule.projectiles.Add(yellow);

            var pink = AddProjectile("spark_pink_idle", 8f, 5f, new Color(255, 150, 150), WitherLanceProjectile.Types.SPARKLY, 123, 3, 1, 2);
            gun.DefaultModule.projectiles.Add(pink);

            RedStar = AddProjectile("spark_red_idle", 5f, 7f, new Color(255, 10, 10), WitherLanceProjectile.Types.BLAST, 125, 1, 2, 6);
            ExplosiveModifier explosiveModifier = RedStar.gameObject.GetOrAddComponent<ExplosiveModifier>();
            RedStarData = new ExplosionData()
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
                effect = Guns.Charge_Shot.DefaultModule.chargeProjectiles[0].Projectile.hitEffects.overrideMidairDeathVFX,
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
                ignoreList = new List<SpeculativeRigidbody>() { }
            };
            explosiveModifier.explosionData = RedStarData;
            explosiveModifier.doExplosion = true;
            explosiveModifier.IgnoreQueues = true;

            gun.DefaultModule.projectiles.Add(RedStar);

            gun.gunClass = GunClass.FULLAUTO;

			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Wither Lance", "Planetside/Resources/GunClips/WitherLance/lancefull", "Planetside/Resources/GunClips/WitherLance/lanceempty");

            gun.barrelOffset.transform.localPosition = new Vector3(2.4375f, 0.625f, 0f);


            //projectile.baseData.range = 5.8f;
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

            ImprovedSynergySetup.Add("Starsign",
            new List<PickupObject> { gun, Guns.Crescent_Crossbow }, null, true);
            var mod = Guns.Crescent_Crossbow.gameObject.AddComponent<CrescentModifier>();
            mod.self = Guns.Crescent_Crossbow;
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
        public static List<Projectile> AllStars = new List<Projectile>();
        public static ExplosionData RedStarData;
        private static Projectile AddProjectile(string animationName, float Damage, float Speed, Color color, WitherLanceProjectile.Types type, int GunHitEffect, float AdditionalDamage = 1, float SpeedMinimum = 2, float SpeedMaximum = 4)
		{
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(Guns.Marine_Sidearm.DefaultModule.projectiles[0]);
            


            var ef = projectile.gameObject.AddComponent<WitherLanceProjectile>();
            ef.baseData = new ProjectileData();
            ef.hitEffects = new ProjectileImpactVFXPool();
            ef.baseData.CopyFrom<ProjectileData>(projectile.baseData);
            ef.CopyFrom<Projectile>(projectile);
            ef.hitEffects.CopyFrom<ProjectileImpactVFXPool>(projectile.hitEffects);

            Destroy(projectile);

            ef.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(ef.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(ef);

            ef.baseData.damage = Damage;
            ef.baseData.speed = Speed;
            ef.baseData.range = 125;

            ef.shouldRotate = false;
            ef.pierceMinorBreakables = true;
            ef.HasDefaultTint = true;
            ef.ownType = type;
            ef.AdditionalDamage = AdditionalDamage;
			ef.SpeedMinimum = SpeedMinimum;
            ef.SpeedMaximum = SpeedMaximum;
            ef.shouldRotate = false;

            ef.baseData.UsesCustomAccelerationCurve = true;
            ef.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1, 0.6f, 0.1f);

            Alexandria.Assetbundle.ProjectileBuilders.AnimateProjectileBundle(ef, animationName, StaticSpriteDefinitions.Projectile_Sheet_Data, StaticSpriteDefinitions.Projectile_Animation_Data, animationName,
			new List<IntVector2>() { new IntVector2(7, 7), new IntVector2(7, 7), new IntVector2(7, 7), new IntVector2(7, 7), new IntVector2(7, 7) },
			AnimateBullet.ConstructListOfSameValues(true, 5), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.LowerLeft, 5), AnimateBullet.ConstructListOfSameValues(true, 5), AnimateBullet.ConstructListOfSameValues(false, 5),
			AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 5), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 5), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 5), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 5));


            ef.objectImpactEventName = (PickupObjectDatabase.GetById(61) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            ef.enemyImpactEventName = (PickupObjectDatabase.GetById(61) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;


            ef.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(GunHitEffect) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            ef.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(GunHitEffect) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            ef.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(GunHitEffect) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            ef.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(GunHitEffect) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);

            HomingModifier HomingMod = ef.gameObject.GetOrAddComponent<HomingModifier>();
            HomingMod.AngularVelocity = 240;
            HomingMod.HomingRadius = 10;

            PierceProjModifier spook = ef.gameObject.AddComponent<PierceProjModifier>();
            spook.penetration = 3;
            spook.penetratesBreakables = true;

            var tro = ef.gameObject.AddChild("trail object");
            tro.transform.position = ef.sprite.WorldCenter;
            tro.transform.localPosition = ef.sprite.WorldCenter;

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
            tr.endColor = (color * 0.4f).WithAlpha(0.4f);
            //======
            tr.time = 0.1f;
            //======
            tr.startWidth = 0.2125f;
            tr.endWidth = 0f;
            tr.autodestruct = false;

            var rend = ef.gameObject.AddComponent<ProjectileTrailRendererController>();
            rend.trailRenderer = tr;
            rend.desiredLength = 0.625f;
            AllStars.Add(ef);

            return ef;
        }
		public static int WitherLanceID;


        public class CrescentModifier : BraveBehaviour
        {
            public Gun self;

            public void Start()
            {
                self.PostProcessProjectile += PPP;
            }

            public void PPP(Projectile projectile)
            {
                if (projectile.Owner != null && projectile.Owner is PlayerController player && player.PlayerHasActiveSynergy("Starsign"))
                {
                    var s = projectile.GetComponent<SpawnProjModifier>();
                    if (s != null)
                    {
                        s.UsesMultipleCollisionSpawnProjectiles = true;
                        s.PostprocessSpawnedProjectiles = true;
                        s.collisionSpawnProjectiles = AllStars.ToArray();
                    }
                    int V = UnityEngine.Random.Range(0, 5);
                    switch (V)
                    {
                        case 0:
                            projectile.AdjustPlayerProjectileTint(Color.blue, 10);
                            projectile.baseData.damage *= 1.3f;
                            projectile.baseData.speed *= 0.7f;
                            projectile.UpdateSpeed();
                            break;
                        case 1:
                            projectile.AdjustPlayerProjectileTint(Color.green, 10);
                            WraparoundProjectile wrap = projectile.gameObject.GetOrAddComponent<WraparoundProjectile>();
                            wrap.Cap += 1;
                            wrap.OnWrappedAround = (proj, pos1, pos2) =>
                            {
                                var h = Instantiate((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect, pos1, Quaternion.identity);
                                var h2 = Instantiate((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect, pos1, Quaternion.identity);
                                Destroy(h, 2);
                                Destroy(h2, 2);
                            };
                            break;
                        case 2:
                            projectile.AdjustPlayerProjectileTint(Color.yellow, 10);
                            projectile.baseData.speed *= 2f;
                            projectile.UpdateSpeed();
                            projectile.baseData.damage *= 0.85f;
                            var pierce = projectile.GetOrAddComponent<PierceProjModifier>();
                            pierce.penetration += 5;
                            var modifier = projectile.GetOrAddComponent<HomingModifier>();
                            modifier.AngularVelocity += 240;
                            modifier.HomingRadius += 10;

                            break;
                        case 3:
                            projectile.AdjustPlayerProjectileTint(Color.red, 10);
                            projectile.baseData.speed *= 0.85f;
                            projectile.UpdateSpeed();
                            projectile.baseData.damage *= 1.2f;
                            ExplosiveModifier explosiveModifier = projectile.gameObject.GetOrAddComponent<ExplosiveModifier>();
                            explosiveModifier.explosionData = RedStarData;
                            explosiveModifier.explosionData.ignoreList = new List<SpeculativeRigidbody>();
                            explosiveModifier.doExplosion = true;
                            explosiveModifier.IgnoreQueues = true;
                            break;
                        case 4:
                            projectile.AdjustPlayerProjectileTint(Color.magenta, 10);
                            projectile.PenetratesInternalWalls = true;
                            projectile.BlackPhantomDamageMultiplier *= 1.5f;
                            projectile.BossDamageMultiplier *= 1.2f;
                            break;
                    }
                }
            }
        }



    }
}