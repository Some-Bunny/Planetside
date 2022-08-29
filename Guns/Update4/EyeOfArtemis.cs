using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;

namespace Planetside
{


    public class Tracker : AdvancedGunBehavior
    {

        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Eye Of Artemis", "tracker");
            Game.Items.Rename("outdated_gun_mods:eye_of_artemis", "psog:eye_of_artemis");
            Tracker wound = gun.gameObject.AddComponent<Tracker>();
            gun.SetShortDescription("Original AI");
            gun.SetLongDescription("Perfectly crafted, codename 'XS-01' was perfectly designed to locate, track, and fire at any given target it spots.\n\nUntil some idiot ripped it apart and assembled it into this weapon.");

            gun.SetupSprite(null, "tracker_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 16);
            gun.SetAnimationFPS(gun.idleAnimation, 3);
            gun.SetAnimationFPS(gun.chargeAnimation, 6);
            gun.SetAnimationFPS(gun.reloadAnimation, 16);



            gun.PreventNormalFireAudio = true;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(394) as Gun).gunSwitchGroup;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_AbyssBlast";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 6;

            for (int i = 0; i < 1; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(88) as Gun, true, false);
            }
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.ammoCost = 1;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.Charged;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.cooldownTime = 0.1f;
                projectileModule.angleVariance = 0;
                projectileModule.numberOfShotsInClip = 1;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                projectileModule.projectiles[0] = projectile;
                projectile.baseData.damage = 20f;
                projectile.AdditionalScaleMultiplier = 1.5f;
                projectile.shouldRotate = true;
                projectile.baseData.range = 14f;
                projectile.baseData.speed *= 1.66f;

                

                ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
                yes.spawnShadows = true;
                yes.shadowLifetime = 0.1f;
                yes.shadowTimeDelay = 0.01f;
                yes.dashColor = new Color(1f, 0.26f, 0f, 1f);


                projectile.AnimateProjectile(new List<string> {
                "havingfallen_001",
                "havingfallen_002",
                "havingfallen_003",
                "havingfallen_004",
                "havingfallen_005",
                "havingfallen_006",
                "havingfallen_007",
                }, 9, true, new List<IntVector2> {
                new IntVector2(17, 7),
                new IntVector2(17, 7),
                new IntVector2(17, 7),
                new IntVector2(17, 7),
                new IntVector2(17, 7),
                new IntVector2(17, 7),
                new IntVector2(17, 7),
                }, AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 7), AnimateBullet.ConstructListOfSameValues(true, 7), AnimateBullet.ConstructListOfSameValues(false, 7), AnimateBullet.ConstructListOfSameValues<Vector3?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<IntVector2?>(null, 7), AnimateBullet.ConstructListOfSameValues<Projectile>(null, 7));

                projectile.objectImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
                projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                gun.DefaultModule.projectiles[0] = projectile;
                bool flag = projectileModule != gun.DefaultModule;
                if (flag)
                {
                    projectileModule.ammoCost = 0;
                }
                projectile.transform.parent = gun.barrelOffset;

                ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
                {
                    Projectile = projectile,
                    ChargeTime = 0f,
                };
                gun.DefaultModule.chargeProjectiles.Add(item2);
            }


            gun.gameObject.transform.Find("Clip").transform.position = new Vector3(0.875f, 0.4375f);
            gun.clipObject = BreakAbleAPI.BreakableAPIToolbox.GenerateDebrisObject("Planetside/Resources/GunObjects/Clips/smileycase.png").gameObject;
            gun.reloadClipLaunchFrame = 4;
            gun.clipsToLaunchOnReload = 1;

            gun.barrelOffset.transform.localPosition = new Vector3(2.125f, 0.75f, 0f);

            gun.reloadTime = 1f;
            gun.SetBaseMaxAmmo(200);
            gun.ammo = 200;

            gun.quality = PickupObject.ItemQuality.B;
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            TrackerID = gun.PickupObjectId;
            ItemIDs.AddToList(gun.PickupObjectId);
        }
        public static int TrackerID;
        private bool HasReloaded;

        private static List<TripleDeckerStorage> tripleDeckerStorages = new List<TripleDeckerStorage>();


        public void CleanupReticles()
        {
           if(tripleDeckerStorages.Count == 0) { return; }
           for (int i = 0; i < tripleDeckerStorages.Count; i++)
           {
                tripleDeckerStorages[i].enemy = null;
                if (tripleDeckerStorages[i].reticle != null) { Destroy(tripleDeckerStorages[i].reticle); }
                if (gun.Volley.projectiles.Contains(tripleDeckerStorages[i].module)) { gun.Volley.projectiles.Remove(tripleDeckerStorages[i].module); }
           }
            ProjectileVolleyData projectileVolleyData = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            projectileVolleyData.InitializeFrom(this.gun.Volley);
            gun.Volley = projectileVolleyData;

            tripleDeckerStorages.Clear();
        }
        public void ThingForBunny(Gun gun, AIActor enemy, GameObject reticle) //Thank you bot, very cool!
        {
            ProjectileVolleyData projectileVolleyData = ScriptableObject.CreateInstance<ProjectileVolleyData>();

            projectileVolleyData.InitializeFrom(gun.Volley);

            projectileVolleyData.projectiles.Clear();

          

            for (int i = 0; i < tripleDeckerStorages.Count; i++)
            {
                ProjectileModule mod = ProjectileModule.CreateClone(gun.DefaultModule);
                mod.projectiles = new List<Projectile>() { gun.DefaultModule.projectiles[0] };
                mod.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>() { gun.DefaultModule.chargeProjectiles[0] };
                projectileVolleyData.projectiles.Add(mod);
                tripleDeckerStorages[i].module = mod;
            }



            gun.Volley = projectileVolleyData;
            gun.ReinitializeModuleData(projectileVolleyData);
            gun.OnPrePlayerChange();

            if (gun.CurrentOwner != null && gun.CurrentOwner is PlayerController)
            {
                (gun.CurrentOwner as PlayerController).stats.RecalculateStats(gun.CurrentOwner as PlayerController);
                (gun.CurrentOwner as PlayerController).inventory.ChangeGun(0, false, false);
            }
            gun.Initialize(gun.CurrentOwner);
        }
        public GameObject SpawnLaser(PlayerController player, AIActor enemy)
        {
            if (enemy == null) { return null; }
            float num2 = 16f;
            Vector2 zero = Vector2.zero;
            if (BraveMathCollege.LineSegmentRectangleIntersection(this.gun.barrelOffset.transform.position, this.gun.barrelOffset.transform.position + BraveMathCollege.DegreesToVector(player.CurrentGun.CurrentAngle, 60f).ToVector3ZisY(-0.25f), new Vector2(-40, -40), new Vector2(40, 40), ref zero))
            {
                num2 = (zero - new Vector2(this.gun.barrelOffset.transform.position.x, this.gun.barrelOffset.transform.position.y)).magnitude;
            }
            GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
            tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
            component2.transform.position = new Vector3(this.gun.barrelOffset.transform.position.x, this.gun.barrelOffset.transform.position.y, 99999);
            component2.transform.localRotation = Quaternion.Euler(0f, 0f, (enemy.sprite.WorldCenter - this.gun.barrelOffset.transform.PositionVector2()).ToAngle());
            component2.dimensions = new Vector2((num2) * Vector2.Distance(this.gun.barrelOffset.transform.position, enemy.sprite.WorldCenter), 1f);
            component2.UpdateZDepth();
            component2.HeightOffGround = -2;
            component2.renderer.enabled = true;

            component2.usesOverrideMaterial = true;
            component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");

            component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
            component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
            Color laser = new Color(1f, 0.3f, 0f, 1f);
            component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
            component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);

            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", gun.CurrentOwner.gameObject);

            var t = new TripleDeckerStorage()
            {
                enemy = enemy,
                reticle = gameObject,
                module = null
            };
            tripleDeckerStorages.Add(t);

            ThingForBunny(gun.CurrentOwner.CurrentGun, enemy, gameObject);
            return gameObject;
        }

        public bool RayCastTowardsEnemy(float Angle)
        {
            bool f = false;
            Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable;
            CollisionLayer layer2 = CollisionLayer.EnemyHitBox;
            int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle ,CollisionLayer.BulletBlocker, layer2);
            RaycastResult raycastResult2;
            if (PhysicsEngine.Instance.Raycast(this.gun.barrelOffset.transform.PositionVector2(), MathToolbox.GetUnitOnCircle(Angle, 0.1f), 1000, out raycastResult2, true, true, rayMask2, null, false, rigidbodyExcluder, null))
            {
                if (raycastResult2.SpeculativeRigidbody && raycastResult2.SpeculativeRigidbody.aiActor)
                {
                    f = true;
                }
            }
            RaycastResult.Pool.Free(ref raycastResult2);
            return f;
        }

        public class TripleDeckerStorage
        {
            public ProjectileModule module;
            public GameObject reticle;
            public AIActor enemy;
        }


        public float ReturnAngle(AIActor enemy)
        {
            if (enemy == null) { return 0; }
            return (enemy.sprite.WorldCenter - this.gun.barrelOffset.transform.PositionVector2()).ToAngle();
        }

        public bool Is(AIActor enemy)
        {
            if (enemy == null) { return false; }
            return ReturnAngle(enemy).IsBetweenRange(gun.CurrentAngle - 24, gun.CurrentAngle + 24);

        }


        protected override void Update()
        {
            base.Update();
            if (gun.CurrentOwner != null)
            {
                PlayerController player = gun.CurrentOwner as PlayerController;
                if (player != null)
                {
                    if (gun.IsCharging == true)
                    {
                        for (int i = 0; i < tripleDeckerStorages.Count; i++)
                        {
                            GameObject reticle = tripleDeckerStorages[i].reticle;

                            AIActor enemy = tripleDeckerStorages[i].enemy;

                            if (enemy == null)
                            {
                                tripleDeckerStorages[i].enemy = null;
                                if (tripleDeckerStorages[i].reticle != null) { Destroy(tripleDeckerStorages[i].reticle); }
                                if (gun.Volley.projectiles.Contains(tripleDeckerStorages[i].module)) { gun.Volley.projectiles.Remove(tripleDeckerStorages[i].module); }
                                tripleDeckerStorages.Remove(tripleDeckerStorages[i]);
                            }

                            ProjectileModule h = gun.Volley.projectiles[i];
                            h.angleVariance = 0;
                            h.angleFromAim = ReturnAngle(enemy) - gun.CurrentAngle;

                            tk2dTiledSprite component2 = reticle.GetComponent<tk2dTiledSprite>();
                            component2.transform.localRotation = Quaternion.Euler(0f, 0f, ReturnAngle(enemy));
                            component2.dimensions = new Vector2(16 * Vector2.Distance(this.gun.barrelOffset.transform.position, enemy.sprite.WorldCenter), 1f);
                            component2.transform.position.WithZ(component2.transform.position.z + 99999);
                            component2.UpdateZDepth();
                            component2.HeightOffGround = -2;

                            reticle.transform.position = this.gun.barrelOffset.transform.position;


                            if (RayCastTowardsEnemy(ReturnAngle(enemy)) == false)
                            {
                                tripleDeckerStorages[i].enemy = null;
                                if (tripleDeckerStorages[i].reticle != null) { Destroy(tripleDeckerStorages[i].reticle); }
                                if (gun.Volley.projectiles.Contains(tripleDeckerStorages[i].module)) { gun.Volley.projectiles.Remove(tripleDeckerStorages[i].module); }
                                tripleDeckerStorages.Remove(tripleDeckerStorages[i]);
                            }
                            else if (Is(enemy) == false)
                            {
                                tripleDeckerStorages[i].enemy = null;
                                if (tripleDeckerStorages[i].reticle != null) { Destroy(tripleDeckerStorages[i].reticle); }
                                if (gun.Volley.projectiles.Contains(tripleDeckerStorages[i].module)) { gun.Volley.projectiles.Remove(tripleDeckerStorages[i].module); }
                                tripleDeckerStorages.Remove(tripleDeckerStorages[i]);

                            }
                        }
                       
                        List<AIActor> actor = player.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.RoomClear);
                        if (actor != null)
                        {
                            for (int i = 0; i < actor.Count; i++)
                            {
                                bool has = false;
                                foreach (TripleDeckerStorage tripleDeckerStorage in tripleDeckerStorages)
                                {
                                    if (tripleDeckerStorage.enemy == actor[i]){has = true;}
                                }
                                if (Is(actor[i]) == true)
                                {
                                    if (has == false && RayCastTowardsEnemy(ReturnAngle(actor[i])) == true)
                                    {
                                        SpawnLaser(player, actor[i]);
                                    }
                                }
                            }
                        }               
                    }
                    else
                    {
                        CleanupReticles();
                    }
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
            else
            {
                CleanupReticles();
            }
        }
        public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                base.OnReloadPressed(player, gun, bSOMETHING);
            }
        }
        protected override void OnPickup(PlayerController player)
        {
            player.GunChanged += this.OnGunChanged;
            base.OnPickup(player);
            CleanupReticles();
        }

        protected override void OnPostDrop(PlayerController player)
        {
            player.GunChanged -= this.OnGunChanged;
            base.OnPostDrop(player);
            CleanupReticles();
        }
        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            if (this.gun && this.gun.CurrentOwner)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;
                if (newGun != this.gun)
                {
                    CleanupReticles();
                }
            }
        }

    }
}

