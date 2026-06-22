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
using Alexandria.Assetbundle;
using Dungeonator;
using FullInspector;
using Pathfinding;
using Planetside.DungeonPlaceables;
using static UnityEngine.UI.CanvasScaler;
using static Planetside.Wailer;
using Alexandria.EnemyAPI;
using Alexandria.Misc;
using static Planetside.PrisonerSecondSubPhaseController;
using Alexandria;
using SynergyAPI;
using static ETGMod;

namespace Planetside
{


    public class BoscoDesignator : AdvancedGunBehavior
    {

        public static void Add()
        {
            Actions.PostDungeonTrueStart += (_) =>
            {
                BoscoCompanionBehavior.BodiesToIgnore.Clear();
            };
            Gun gun = ETGMod.Databases.Items.NewGun("Drone Control Unit", "dronecontrolunit");
            Game.Items.Rename("outdated_gun_mods:drone_control_unit", "psog:drone_control_unit");
            gun.gameObject.AddComponent<BoscoDesignator>();
            gun.SetShortDescription("Mine It!");
            gun.SetLongDescription("Deploys a commandable helper drone. Shoot the laser designator at certain objects to give commands!\n\nAll drones are trained on orange cat behavior.");


            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "boscotargeter_idle_001", "boscotargeter_ammonomicon");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.reloadAnimation = "boscotargeter_reload";
            gun.idleAnimation = "boscotargeter_idle";
            gun.shootAnimation = "boscotargeter_fire";



            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(86) as Gun, true, false);
            gun.gunSwitchGroup = Guns.Mourning_Star.gunSwitchGroup;
            gun.PreventNormalFireAudio = true;


            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_OBJ_mine_beep_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;

            //GUN STATS
            gun.muzzleFlashEffects = new VFXPool() { effects = new VFXComplex[0], type = VFXPoolType.None };//(PickupObjectDatabase.GetById(58) as Gun).muzzleFlashEffects;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0f;
            gun.DefaultModule.cooldownTime = 0.5f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.barrelOffset.transform.localPosition = new Vector3(13f / 16f, 8f / 16f, 0f);
            gun.SetBaseMaxAmmo(1000);
            gun.InfiniteAmmo = true;
            gun.gunClass = GunClass.CHARGE;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.BEAM;

            //BULLET STATS


            gun.DefaultModule.projectiles[0] = null;

            /*
            Projectile dud = UnityEngine.Object.Instantiate<Projectile>(Guns.Marine_Sidearm.DefaultModule.projectiles[0]);
            dud.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(dud.gameObject);
            dud.baseData.range = 0.001f;
            dud.baseData.damage = 0;
            dud.baseData.speed = 0.1f;
            dud.baseData.range = 0.1f;
            dud.hitEffects = new ProjectileImpactVFXPool() { suppressMidairDeathVfx = true, alwaysUseMidair = true, };
            dud.sprite.renderer.enabled = false;
            gun.DefaultModule.projectiles[0] = dud;
            */

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(Guns.Marine_Sidearm.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 0;
            projectile.baseData.force = 0f;
            projectile.baseData.speed = 150f;
            projectile.AppliesFire = false;
            projectile.baseData.range = 100000;
            projectile.sprite.renderer.enabled = false;
            /*
            projectile.hitEffects.tileMapHorizontal = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(32) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.tileMapVertical = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(32) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.enemy = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(32) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.deathAny = ObjectMakers.MakeObjectIntoVFX((PickupObjectDatabase.GetById(32) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
            gun.DefaultModule.angleVariance = 0f;
            projectile.hitEffects.CenterDeathVFXOnProjectile = false;
            */

            var collection = StaticSpriteDefinitions.Beam_Sheet_Data;
            var BoscoEffect = ItemBuilder.AddSpriteToObjectAssetbundle("BoscoTargetHitEffect", collection.GetSpriteIdByName("boscolaser_hit_005"), collection);
            FakePrefab.MarkAsFakePrefab(BoscoEffect);
            UnityEngine.Object.DontDestroyOnLoad(BoscoEffect);
            BoscoEffect.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator effectAnimator = BoscoEffect.GetOrAddComponent<tk2dSpriteAnimator>();
            effectAnimator.library = StaticSpriteDefinitions.Beam_Animation_Data;
            effectAnimator.playAutomatically = true;
            effectAnimator.defaultClipId = effectAnimator.library.GetClipIdByName("boscobeam_impact");

            SpriteAnimatorKiller kill = effectAnimator.gameObject.AddComponent<SpriteAnimatorKiller>();
            kill.fadeTime = -1f;
            kill.animator = effectAnimator;
            kill.delayDestructionTime = -1f;



            projectile.AddTrailToProjectileBundle(StaticSpriteDefinitions.Beam_Sheet_Data, "boscolaser_fire_001",
            StaticSpriteDefinitions.Beam_Animation_Data,
            "boscobeam_mid", new Vector2(9, 1), new Vector2(0, 4), false, "boscobeam_mid");
            Material projMat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            projMat.mainTexture = projectile.sprite.renderer.material.mainTexture;
            projMat.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
            projMat.SetFloat("_EmissiveColorPower", 1.55f);
            projMat.SetFloat("_EmissivePower", 200);
            projectile.sprite.renderer.material = projMat;
            projectile.shouldRotate = true;
            projectile.hitEffects = Guns.Void_Marshal.DefaultModule.projectiles[0].hitEffects;

            //projectile.hitEffects = new ProjectileImpactVFXPool() {  alwaysUseMidair = true, overrideMidairDeathVFX = BoscoEffect };

            Pingprojectile = projectile;


            BoscoProjectile = UnityEngine.Object.Instantiate<Projectile>(Guns.Thunderclap.DefaultModule.projectiles[0]);
            BoscoProjectile.gameObject.SetActive(false);
            BoscoProjectile.baseData.speed = 60;
            BoscoProjectile.baseData.damage = 4f;
            BoscoProjectile.baseData.range = 40;
            BoscoProjectile.AdditionalScaleMultiplier = 0.4f;
            BoscoProjectile.sprite.renderer.enabled = false;
            FakePrefab.MarkAsFakePrefab(BoscoProjectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(BoscoProjectile);

            var tro = BoscoProjectile.gameObject.AddChild("trail object");
            tro.transform.position = BoscoProjectile.sprite.WorldCenter;
            tro.transform.localPosition = BoscoProjectile.sprite.WorldCenter;

            TrailRenderer tr = tro.AddComponent<TrailRenderer>();
            tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tr.receiveShadows = false;
            var mat = new Material(Shader.Find("Sprites/Default"));
            tr.material = mat;
            tr.minVertexDistance = 0.01f;
            tr.numCapVertices = 640;

            //======
            mat.SetColor("_Color", Color.cyan * 4);
            tr.startColor = Color.cyan * 4;
            tr.endColor = new Color(0, 0f, 1, 1);
            //======
            tr.time = 0.125f;
            //======
            tr.startWidth = 0.1875f;
            tr.endWidth = 0f;
            tr.autodestruct = false;

            var rend = BoscoProjectile.gameObject.AddComponent<ProjectileTrailRendererController>();
            rend.trailRenderer = tr;
            rend.desiredLength = 8;


            gun.quality = PickupObject.ItemQuality.S;
            ETGMod.Databases.Items.Add(gun, false, "ANY");

            gun.AddItemToSynergy(CustomSynergyType.TEA_FOR_TWO);

            BuildPrefab();
        }

        public static Projectile Pingprojectile;
        public static Projectile BoscoProjectile;

        public override void Update()
        {
            base.Update();
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            bool Fart = false;
            var shot = SpawnManager.SpawnProjectile(Pingprojectile.gameObject, gun.barrelOffset.position, Quaternion.Euler(0, 0, gun.CurrentAngle));
            var p = shot.GetComponent<Projectile>();
            if (p != null)
            {
                p.Owner = gun.CurrentOwner;
                p.Shooter = gun.CurrentOwner.specRigidbody;

                p.specRigidbody.OnPreTileCollision += (myBody, myPixel, _Tile, __otherPixel) =>
                {
                    if (Fart)
                        return;
                    Fart = true;

                   
                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = _Tile.Position.ToVector3(),
                        startColor = Color.red.WithAlpha(0.33f),
                        startLifetime = 0.25f,
                        startSize = 5
                    });
                };

                p.specRigidbody.OnPreRigidbodyCollision += (myBody, myPixel, _otherBody, __otherPixel) =>
                {
                    if (_otherBody.minorBreakable)
                    {
                        return;
                    }

                    if (Fart)
                        return;

                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = p.sprite.WorldCenter,
                        startColor = Color.red.WithAlpha(0.33f),
                        startLifetime = 0.25f,
                        startSize = 5
                    });


                    Fart = true;
                    SpeculativeRigidbody otherBody = (_otherBody as SpeculativeRigidbody);
                    if (otherBody != null)
                    {
                        if (otherBody.aiActor)
                        {
                            if (otherBody.aiActor.healthHaver.IsBoss == true || otherBody.aiActor.healthHaver.IsSubboss == true)
                            {
                                BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                    new BoscoCompanionBehavior.BoscoPriorityTarget(
                                        BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.PriorityAttack,
                                        otherBody, 2, 6),
                                    gun.CurrentOwner as PlayerController);
                            }
                            else
                            {
                                if (otherBody.aiActor.EnemyGuid == EnemyGUIDs.Key_Bullet_Kin_GUID | otherBody.aiActor.EnemyGuid == EnemyGUIDs.Chance_Kin_GUID)
                                {
                                    BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                        new BoscoCompanionBehavior.BoscoPriorityTarget(
                                            BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.PriorityAttack,
                                            otherBody, 2, 6),
                                        gun.CurrentOwner as PlayerController);
                                }
                                else
                                {
                                    BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                    new BoscoCompanionBehavior.BoscoPriorityTarget(
                                        BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.Taze,
                                        otherBody, -1f, 1.25f),
                                    gun.CurrentOwner as PlayerController);
                                }
                            }
                        }
                        else
                        {
                            var c = otherBody.GetComponents(typeof(Component));
                            foreach (Component component in c)
                            {
                                if (component is Idol idol | component is EnemyBuffShrineController controller)
                                {
                                    BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                        new BoscoCompanionBehavior.BoscoPriorityTarget(
                                            BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.AreaDamage,
                                            otherBody, -1f, 1.5f),
                                        gun.CurrentOwner as PlayerController);
                                    break;
                                }

                                if (component is MajorBreakable breakable)
                                {
                                    if (breakable.IsSecretDoor)
                                    {
                                        BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                            new BoscoCompanionBehavior.BoscoPriorityTarget(
                                                BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.SecretRoomReveal,
                                                otherBody, 3, 6),
                                            gun.CurrentOwner as PlayerController);
                                        break;
                                    }
                                }
                                if (component is Chest chest)
                                {
                                    BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                                   new BoscoCompanionBehavior.BoscoPriorityTarget(
                                                       BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.ChestTamper,
                                                       otherBody, 0.5f, 1f, new Vector2(0, -0.5f)),
                                                   gun.CurrentOwner as PlayerController);
                                    break;

                                }
                                if (component is InteractableLock locked)
                                {
                                    if (!locked.IsBusted && locked.IsLocked && locked.lockMode == InteractableLock.InteractableLockMode.NORMAL)
                                    {
                                        BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                            new BoscoCompanionBehavior.BoscoPriorityTarget(
                                                BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.Unlock,
                                                otherBody, 0.25f, 0.75f, new Vector2(0, -0f)),
                                            gun.CurrentOwner as PlayerController);
                                        break;
                                    }
                                }


                                if (component is RewardPedestal pedestal)
                                {
                                    if (pedestal.IsMimic)
                                    {
                                        BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                            new BoscoCompanionBehavior.BoscoPriorityTarget(
                                            BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.PedestalMimicHoldDown,
                                            otherBody, 0.75f, 2f, new Vector2(0, -1)),
                                        gun.CurrentOwner as PlayerController);
                                        break;
                                    }
                                }
                            }

                            c = otherBody.GetComponentsInChildren(typeof(Component));
                            foreach (Component component in c)
                            {
                                //Debug.Log(component.GetType());
                                if (component is FireplaceController fireplace)
                                {
                                    if (fireplace.FireObject.activeSelf)
                                    {
                                        BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                            new BoscoCompanionBehavior.BoscoPriorityTarget(
                                                BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.FireplaceWater,
                                                otherBody, 0.75f, 2f, new Vector2(0, -0.25f)),
                                            gun.CurrentOwner as PlayerController);
                                        break;
                                    }
                                }
                                if (component is Idol idol | component is EnemyBuffShrineController controller)
                                {
                                    BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                        new BoscoCompanionBehavior.BoscoPriorityTarget(
                                            BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.AreaDamage,
                                            otherBody, -1f, 1.5f),
                                        gun.CurrentOwner as PlayerController);
                                    break;
                                }
                                if (component is InteractableLock locked)
                                {
                                    if (!locked.IsBusted && locked.IsLocked && locked.lockMode == InteractableLock.InteractableLockMode.NORMAL)
                                    {
                                        BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                            new BoscoCompanionBehavior.BoscoPriorityTarget(
                                                BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.Unlock,
                                                otherBody, 0.25f, 0.75f, new Vector2(0, -0f)),
                                            gun.CurrentOwner as PlayerController);
                                        break;
                                    }
                                }
                            }
                            c = otherBody.GetComponentsInParent(typeof(Component));
                            foreach (Component component in c)
                            {

                                if (component is FireplaceController fireplace)
                                {
                                    if (fireplace.FireObject.activeSelf)
                                    {
                                        BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                            new BoscoCompanionBehavior.BoscoPriorityTarget(
                                                BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.FireplaceWater,
                                                otherBody, 0.75f, 2f, new Vector2(0, -0.25f)),
                                            gun.CurrentOwner as PlayerController);
                                        break;
                                    }
                                }
                                if (component is InteractableLock locked)
                                {
                                    if (!locked.IsBusted && locked.IsLocked && locked.lockMode == InteractableLock.InteractableLockMode.NORMAL)
                                    {
                                        BoscoCompanionBehavior.AddPriorityToAllBoscos(
                                            new BoscoCompanionBehavior.BoscoPriorityTarget(
                                                BoscoCompanionBehavior.BoscoPriorityTarget.BoscoBehaviorType.Unlock,
                                                otherBody, 0.25f, 0.75f, new Vector2(0, -0f)),
                                            gun.CurrentOwner as PlayerController);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }




        private GameObject extantCompanion;

        public override void Start()
        {
            base.Start();
        }

        public override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
            DoCompanionCheck(player);
            player.OnNewFloorLoaded += DoCompanionCheck;
        }

        public override void OnPostDrop(PlayerController player)
        {
            base.OnPostDrop(player);
            DestroyOomfie();
            player.OnNewFloorLoaded -= DoCompanionCheck;

        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            //BoscoCompanionBehavior.AddPriorityToAllBoscos(null,gun.CurrentOwner as PlayerController);
            base.OnReloadPressed(player, gun, bSOMETHING);
        }

        public void DoCompanionCheck(PlayerController player)
        {
            if (extantCompanion == null)
            {
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("PSOG:Bosco");
                Vector3 vector = Owner.transform.position;
                if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                {
                    vector += new Vector3(1.125f, -0.3125f, 0f);
                }
                extantCompanion = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, vector, Quaternion.identity);
                CompanionController orAddComponent = extantCompanion.GetOrAddComponent<CompanionController>();
                orAddComponent.Initialize(gun.CurrentOwner as PlayerController);
                if (orAddComponent.specRigidbody)
                {
                    PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody, null, false);
                }
            }
        }

        public void DestroyOomfie()
        {
            if (extantCompanion != null)
            {
                var _ =UnityEngine.Object.Instantiate(StaticVFXStorage.BigShotgunExplosion, extantCompanion.GetComponent<tk2dSprite>().sprite.WorldCenter, Quaternion.identity);
                Destroy(_, 5);
                Destroy(extantCompanion);
            }
        }


        public static void BuildPrefab()
        {
            var companion = CompanionBuilder.BuildPrefabBundled("PSOG:Bosco", "PSOG:Bosco", new IntVector2(0, 0), new IntVector2(8, 8)).GetComponent<AIActor>();
            
            companion.aiActor.MovementSpeed = 3.6f;
            companion.aiActor.healthHaver.PreventAllDamage = true;
            companion.aiActor.CollisionDamage = 0f;
            companion.aiActor.HasShadow = false;
            companion.aiActor.PreventFallingInPitsEver = false;
            companion.aiActor.healthHaver.ForceSetCurrentHealth(30f);
            companion.aiActor.CollisionKnockbackStrength = 5f;
            companion.aiActor.CanTargetPlayers = false;
            companion.aiActor.CanTargetEnemies = true;
            companion.aiActor.IgnoreForRoomClear = true;
            AIAnimator aiAnimator = companion.aiAnimator;
            companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;
            companion.reinforceType = AIActor.ReinforceType.Instant;
            companion.AwakenAnimType = AIActor.AwakenAnimationType.Default;
            companion.aiActor.sprite.SetSprite(StaticSpriteDefinitions.Companion_Sheet_Data, 0);

            companion.aiActor.HasShadow = true;
            EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.defaultShadow, new Vector2(0.625f, 0.125f), "shadowPos");

            var animator = companion.gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Companion_Animation;

            CompanionController companionController = companion.AddComponent<CompanionController>();


            aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>();
            aiAnimator.IdleAnimation = new DirectionalAnimation
            {
                Type = DirectionalAnimation.DirectionType.SixWay,
                Flipped = new DirectionalAnimation.FlipType[6],
                AnimNames = new string[]
                {
                        "bosco_idle_back",
                        "bosco_idle_back_right",
                        "bosco_idle_front_right",
                        "bosco_idle_front",
                        "bosco_idle_front_left",
                        "bosco_idle_back_left"
                }
            };

            Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                aiAnimator, "taze",
                new string[] 
                {
                    "bosco_taze_back",
                    "bosco_taze_back_right",
                    "bosco_taze_front_right",
                    "bosco_taze_front",
                    "bosco_taze_front_left",
                    "bosco_taze_back_left",
                },
                new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.SixWay);


            Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                aiAnimator, "spawnin",
                new string[] { "bosco_spawn" },
                new DirectionalAnimation.FlipType[1]);


            Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                aiAnimator, "spawninend",
                new string[] { "bosco_spawn_end" },
                new DirectionalAnimation.FlipType[1]);


            Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                aiAnimator, "zzap_start",
                new string[] { "bosco_zzap_start" },
                new DirectionalAnimation.FlipType[1]);

            Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                aiAnimator, "zzap_loop",
                new string[] { "bosco_zzap_loop" },
                new DirectionalAnimation.FlipType[1]);

            Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                aiAnimator, "zzap_end",
                new string[] { "bosco_zzap_end" },
                new DirectionalAnimation.FlipType[1]);

            Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                aiAnimator, "bosco_yippie",
                new string[] { "bosco_yippie" },
                new DirectionalAnimation.FlipType[1]);

            AIBulletBank aIBulletBank = companion.gameObject.AddComponent<AIBulletBank>();
            aIBulletBank.Bullets = new List<AIBulletBank.Entry>() {  };

            var bs = companion.GetComponent<BehaviorSpeculator>();

            bs.MovementBehaviors = new List<MovementBehaviorBase>();
            bs.MovementBehaviors.Add(new BoscoCompanionBehavior() {  });

            List<int> Offsets = new List<int>();
            companion.spriteAnimator.GetClipByName("bosco_spawn").ApplyOffsetToAnimation(new Vector2(-0.5f, -0.25f), Offsets);
            companion.spriteAnimator.GetClipByName("bosco_spawn_end").ApplyOffsetToAnimation(new Vector2(-0.5f, -0.25f), Offsets);

            companion.spriteAnimator.GetClipByName("bosco_zzap_start").ApplyOffsetToAnimation(new Vector2(-0.25f, -0.1875f), Offsets);
            companion.spriteAnimator.GetClipByName("bosco_zzap_loop").ApplyOffsetToAnimation(new Vector2(-0.25f, -0.1875f), Offsets);
            companion.spriteAnimator.GetClipByName("bosco_zzap_end").ApplyOffsetToAnimation(new Vector2(-0.25f, -0.1875f), Offsets);

            var collection = StaticSpriteDefinitions.Companion_Sheet_Data;
            var BoscoEffect = ItemBuilder.AddSpriteToObjectAssetbundle("BoscoEffect", collection.GetSpriteIdByName("bosco_warn_001"), collection);
            FakePrefab.MarkAsFakePrefab(BoscoEffect);
            UnityEngine.Object.DontDestroyOnLoad(BoscoEffect);
            BoscoEffect.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator effectAnimator = BoscoEffect.GetOrAddComponent<tk2dSpriteAnimator>();
            effectAnimator.library = StaticSpriteDefinitions.Companion_Animation;
            effectAnimator.Library = StaticSpriteDefinitions.Companion_Animation;

            BoscoEffectInst = effectAnimator;

            companion.spriteAnimator.GetClipByName("bosco_spawn").frames[13].eventAudio = "Play_OBJ_item_throw_01";
            companion.spriteAnimator.GetClipByName("bosco_spawn").frames[13].triggerEvent = true;
            companion.spriteAnimator.GetClipByName("bosco_spawn").frames[15].eventAudio = "Play_OBJ_item_throw_01";
            companion.spriteAnimator.GetClipByName("bosco_spawn").frames[15].triggerEvent = true;

            companion.spriteAnimator.GetClipByName("bosco_zzap_loop").frames[0].eventAudio = "Play_obj_computer_break_01";
            companion.spriteAnimator.GetClipByName("bosco_zzap_loop").frames[0].triggerEvent = true;

        }
        public static tk2dSpriteAnimator BoscoEffectInst;




        public class BoscoCompanionBehavior : MovementBehaviorBase
        {

            public override float DesiredCombatDistance
            {
                get
                {
                    return 5f;
                }
            }
            public static List<BoscoCompanionBehavior> AllBoscos = new List<BoscoCompanionBehavior>();
            public override void Start()
            {
                base.Start();
                this.m_companionController = this.m_gameObject.GetComponent<CompanionController>();
                this.m_aiActor.StartCoroutine(DoSpawn());
                if (AllBoscos == null)
                    AllBoscos = new List<BoscoCompanionBehavior> { };
                AllBoscos.Add(this);      
            }


            public void DoBasicAttack()
            {
                if (InstCooldown > 0)
                {
                    return;
                }
                if (InstCooldownBurst > 0)
                {
                    return;
                }
                var p = SpawnManager.SpawnProjectile(BoscoDesignator.BoscoProjectile.gameObject, m_aiActor.sprite.WorldCenter - new Vector2(0, 0.375f), Quaternion.Euler(0, 0, (this.m_aiActor.TargetRigidbody.specRigidbody.UnitCenter - (this.m_aiActor.specRigidbody.UnitCenter - new Vector2(0, 0.375f))).ToAngle() + UnityEngine.Random.Range(-3f * PanicAtTheDisco, (3f * PanicAtTheDisco) + 1))).GetComponent<Projectile>();
                if (p != null)
                {        
                    p.SetOwnerSafe(m_companionController.m_owner, "Bosco");
                    m_companionController.m_owner.DoPostProcessProjectile(p);
                    p.baseData.speed *= PanicAtTheDisco;
                    p.UpdateSpeed();

                }
                AkSoundEngine.PostEvent("Play_Strafe_Shot", m_aiActor.gameObject);
                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = m_aiActor.sprite.WorldCenter,
                    startColor = Color.cyan.WithAlpha(0.3f),
                    startLifetime = 0.1f,
                    startSize = 2
                });
                currentStandardFireAmount++;
                InstCooldownBurst += CooldownBurst / PanicAtTheDisco;
                if (currentStandardFireAmount >= StandardFireAmount)
                {
                    InstCooldown = Cooldown / PanicAtTheDisco;
                    currentStandardFireAmount = 0;
                }
            }



            public IEnumerator DoSpawn()
            {
                this.TemporarilyDisabled = true;
                this.m_aiActor.aiAnimator.Play("bosco_spawn", AIAnimator.AnimatorState.StateEndType.Duration, 2.25f, -1, true, "");
                AkSoundEngine.PostEvent("Play_OBJ_item_spawn_01", m_aiActor.gameObject);
                while (this.m_aiActor.aiAnimator.IsPlaying("bosco_spawn"))
                {
                    yield return null;
                }
                this.m_aiActor.aiAnimator.Play("bosco_spawn_end", AIAnimator.AnimatorState.StateEndType.UntilFinished, -1, -1, true, "");
                while (this.m_aiActor.aiAnimator.IsPlaying("bosco_spawn_end"))
                {
                    yield return null;
                }
                TemporarilyDisabled = false;
                yield break;
            }


            public override void Upkeep()
            {
                base.Upkeep();

            }

            private void CatchUpMovementModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
            {
                if (this.DisableInCombat)
                {
                    PlayerController playerController = GameManager.Instance.PrimaryPlayer;
                    if (this.m_aiActor && this.m_aiActor.CompanionOwner)
                    {
                        playerController = this.m_aiActor.CompanionOwner;
                    }
                    if (playerController && playerController.IsInCombat && Vector2.Distance(playerController.CenterPosition, this.m_aiActor.CenterPosition) < this.CatchUpRadius)
                    {
                        this.m_isCatchingUp = false;
                        this.m_aiActor.MovementModifiers -= this.CatchUpMovementModifier;
                        return;
                    }
                }
                this.m_catchUpTime += this.m_aiActor.LocalDeltaTime;
                voluntaryVel = voluntaryVel.normalized * Mathf.Lerp(this.CatchUpSpeed, this.CatchUpMaxSpeed, this.m_catchUpTime / this.CatchUpAccelTime);
            }

            private void DoPriorityMovementModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
            {
                if (CurrentBoscoPriority.HasValue)
                {
                    if (CurrentBoscoPriority.Value.PriorityBody != null)
                    {
                        if (CurrentBoscoPriority.Value.PriorityBody.aiActor != null)
                        {
                            voluntaryVel = voluntaryVel.normalized * (Mathf.Max(CurrentBoscoPriority.Value.PriorityBody.aiActor.Velocity.magnitude * 2, 8));
                            return;
                        }
                    }
                }
                voluntaryVel *= 2.5f;
            }


            public override ContinuousBehaviorResult ContinuousUpdate()
            {
                return base.ContinuousUpdate();
            }

            public override void EndContinuousUpdate()
            {
                this.m_updateEveryFrame = false;
                this.m_aiActor.FallingProhibited = false;
                this.m_aiActor.BehaviorOverridesVelocity = false;
                base.EndContinuousUpdate();
            }


            private bool isDoingPriority = false;
            private bool isPerformingPriorityTask = false;

            public override BehaviorResult Update()
            {

                base.DecrementTimer(ref this.m_repathTimer, false);
                base.DecrementTimer(ref this.m_losTimer, false);

                base.DecrementTimer(ref this.InstCooldown, true);
                base.DecrementTimer(ref this.InstCooldownBurst, true);




                if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel)
                {
                    return BehaviorResult.SkipAllRemainingBehaviors;
                }
                if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
                {
                    this.m_aiActor.ClearPath();
                    return BehaviorResult.SkipAllRemainingBehaviors;
                }
                if (this.TemporarilyDisabled)
                {
                    return BehaviorResult.Continue;
                }
                PlayerController playerController = GameManager.Instance.PrimaryPlayer;
                if (this.m_aiActor && this.m_aiActor.CompanionOwner)
                {
                    playerController = this.m_aiActor.CompanionOwner;
                }
                if (isPerformingPriorityTask)
                {
                    this.m_aiActor.ClearPath();
                    return BehaviorResult.SkipRemainingClassBehaviors;
                }


                GetTarget();

                if (CurrentBoscoPriority != null)
                {
                    if (m_repathTimer > 0)
                    {
                        m_repathTimer = -1;
                    }

                    if (InAttackRange() && CurrentBoscoPriority.Value.boscoBehaviorType == BoscoPriorityTarget.BoscoBehaviorType.PriorityAttack)
                    {
                        PanicAtTheDisco = 2f;
                        DoBasicAttack();
                    }

                    if (this.m_isCatchingUp)
                    {
                        this.m_isCatchingUp = false;
                        this.m_aiActor.MovementModifiers -= this.CatchUpMovementModifier;
                    }
                    if (!isDoingPriority)
                    {
                        isDoingPriority = true;
                        this.m_aiActor.MovementModifiers += this.DoPriorityMovementModifier;
                    }


                    bool isFail = false;
                    var positionTotrackTo = CurrentBoscoPriority.Value.ToPriorityPosition(ref isFail);
                    if (isFail)
                    {
                        CurrentBoscoPriority = null;
                        return BehaviorResult.SkipRemainingClassBehaviors;
                    }
                    m_aiActor.OverrideTarget = CurrentBoscoPriority.Value.PriorityBody;

                    if (CurrentBoscoPriority.Value.isNear(m_aiActor.specRigidbody.UnitCenter, positionTotrackTo))
                    {
                        DoPerformPriorityBehavior();
                        return BehaviorResult.SkipRemainingClassBehaviors;
                    }
                    if (this.m_repathTimer <= 0f)
                    {
                        CellValidator cellValidator = null;
                        AIActor aiActor = this.m_aiActor;
                        CellValidator cellValidator2 = cellValidator;
                        aiActor.PathfindToPosition(positionTotrackTo, null, true, cellValidator2, null, CellTypes.FLOOR | CellTypes.PIT, false);
                        this.m_repathTimer = this.PathInterval;
                    }
                    return BehaviorResult.SkipRemainingClassBehaviors;
                }
                if (isDoingPriority)
                {
                    ResetIdle();
                    m_aiActor.OverrideTarget = null;
                    isDoingPriority = false;
                    this.m_aiActor.MovementModifiers -= this.DoPriorityMovementModifier;
                    this.m_aiActor.aiAnimator.OverrideIdleAnimation = "";
                    PanicAtTheDisco = 1;
                }

                if (this.m_aiActor.TargetRigidbody)
                {
                    
                    IntVector2 intVector2 = (!this.m_aiActor.specRigidbody) ? this.m_aiActor.transform.position.IntXY(VectorConversions.Floor) : this.m_aiActor.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
                    if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2) && !GameManager.Instance.Dungeon.data[intVector2].isExitCell)
                    {
                        if (this.m_isCatchingUp)
                        {
                            this.m_isCatchingUp = false;
                            this.m_aiActor.MovementModifiers -= this.CatchUpMovementModifier;
                        }
                    }

                    SpeculativeRigidbody targetRigidbody = this.m_aiActor.TargetRigidbody;
                    if (InAttackRange())
                    {
                        DoBasicAttack();
                    }

                    if (InRange() || !targetRigidbody)
                    {
                        //this.m_aiActor.aiAnimator.facingType = 
                        if (this.m_state == SeekTargetBehavior.State.PathingToTarget)
                        {
                            this.m_aiActor.ClearPath();
                            this.m_state = SeekTargetBehavior.State.Idle;
                        }
                        /*
                        else if (this.m_state == SeekTargetBehavior.State.Idle)
                        {
                            if (this.ReturnToSpawn && this.m_aiActor.GridPosition != this.m_aiActor.SpawnGridPosition && this.m_aiActor.PathComplete)
                            {
                                this.m_state = SeekTargetBehavior.State.ReturningToSpawn;
                            }
                        }
                        else if (this.m_state == SeekTargetBehavior.State.ReturningToSpawn && this.m_aiActor.PathComplete)
                        {
                            this.m_state = SeekTargetBehavior.State.Idle;
                        }
                        
                        //this.m_aiActor.PathfindToPosition(this.m_aiActor.SpawnPosition, null, true, null, null, null, false);
                        Debug.Log("3");
                        */
                        return BehaviorResult.SkipRemainingClassBehaviors;
                    }

                    bool hasLOS = this.m_aiActor.HasLineOfSightToTarget;
                    float desiredCombatDistance = this.m_aiActor.DesiredCombatDistance;
                    this.m_state = SeekTargetBehavior.State.PathingToTarget;
                    if (this.m_aiActor.TargetRigidbody && this.m_aiActor.TargetRigidbody.aiActor && !this.m_aiActor.TargetRigidbody.CollideWithOthers)
                    {
                        hasLOS = true;
                    }
                    if (this.StopWhenInRange && this.m_aiActor.DistanceToTarget <= desiredCombatDistance)
                    {
                        this.m_aiActor.ClearPath();
                        return BehaviorResult.SkipRemainingClassBehaviors;
                    }


                    
                    if (this.m_repathTimer <= 0f)
                    {
                        CellValidator cellValidator = null;

                        Vector2 unitCenter = targetRigidbody.UnitCenter;
                        AIActor aiActor = this.m_aiActor;
                        Vector2 targetPosition = unitCenter;
                        CellValidator cellValidator2 = cellValidator;
                        aiActor.PathfindToPosition(targetPosition, null, true, cellValidator2, null, CellTypes.FLOOR | CellTypes.PIT, false);
                        this.m_repathTimer = 1.5f;// this.PathInterval;
                    }
                    

                    //this.m_aiActor.PathfindToPosition(this.m_aiActor.SpawnPosition, null, true, null, null, null, false);
                    return BehaviorResult.SkipRemainingClassBehaviors;
                }



                #region Pathfind To Player If No Other Option

                base.DecrementTimer(ref this.m_idleTimer, false);


                this.m_aiActor.DustUpInterval = Mathf.Lerp(0.5f, 0.125f, this.m_aiActor.specRigidbody.Velocity.magnitude / this.CatchUpSpeed);




                IntVector2 intVector = this.m_aiActor.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
                CellData cellData = GameManager.Instance.Dungeon.data[intVector];
                if (cellData != null && cellData.IsPlayerInaccessible)
                {
                    if (this.m_repathTimer <= 0f)
                    {
                        this.m_repathTimer = this.PathInterval;
                        RoomHandler absoluteRoomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector);
                        if (absoluteRoomFromPosition != null)
                        {
                            IntVector2? nearestAvailableCell = absoluteRoomFromPosition.GetNearestAvailableCell(intVector.ToCenterVector2(), new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), false, (IntVector2 pos) => !GameManager.Instance.Dungeon.data[pos].IsPlayerInaccessible);
                            if (nearestAvailableCell != null)
                            {
                                this.m_aiActor.PathfindToPosition(nearestAvailableCell.Value.ToCenterVector2(), null, true, null, null, null, false);
                            }
                        }
                    }
                    return BehaviorResult.SkipRemainingClassBehaviors;
                }
                if (!playerController)
                {
                    return BehaviorResult.Continue;
                }


                float num = Vector2.Distance(playerController.CenterPosition, this.m_aiActor.CenterPosition);
                if (num <= this.IdealRadius)// && !flag)
                {
                    this.m_aiActor.ClearPath();
                    if (this.m_isCatchingUp)
                    {
                        this.m_isCatchingUp = false;
                        this.m_aiActor.MovementModifiers -= this.CatchUpMovementModifier;
                    }
                    return BehaviorResult.SkipRemainingClassBehaviors;
                }
                if (num > 30f)
                {
                    this.m_sequentialPathFails = 0;
                    this.m_aiActor.CompanionWarp(this.m_aiActor.CompanionOwner.CenterPosition);
                }
                else if (!this.m_isCatchingUp && num > this.CatchUpRadius)
                {
                    this.m_isCatchingUp = true;
                    this.m_catchUpTime = 0f;

                    this.m_aiActor.MovementModifiers += this.CatchUpMovementModifier;
                }
                this.m_idleTimer = Mathf.Max(this.m_idleTimer, 2f);
                if (this.m_repathTimer <= 0f && !playerController.IsOverPitAtAll && !playerController.IsInMinecart)
                {
                    this.m_repathTimer = this.PathInterval;
                    this.m_aiActor.FallingProhibited = false;

                    this.m_aiActor.PathfindToPosition(playerController.specRigidbody.UnitCenter, null, true, null, null, null, false);


                    if (this.m_aiActor.Path != null && this.m_aiActor.Path.InaccurateLength > 50f)
                    {
                        this.m_aiActor.ClearPath();
                        this.m_sequentialPathFails = 0;
                        this.m_aiActor.CompanionWarp(this.m_aiActor.CompanionOwner.CenterPosition);
                    }
                    else if (this.m_aiActor.Path != null && !this.m_aiActor.Path.WillReachFinalGoal)
                    {
                        bool flag2 = false;
                        this.m_aiActor.PathableTiles = (this.m_aiActor.PathableTiles | CellTypes.PIT);
                        this.m_aiActor.PathfindToPosition(playerController.specRigidbody.UnitCenter, null, true, null, null, null, false);
                        this.m_aiActor.PathableTiles = (this.m_aiActor.PathableTiles & ~CellTypes.PIT);
                        if (this.m_aiActor.Path != null && this.m_aiActor.Path.WillReachFinalGoal)
                        {
                            this.m_aiActor.FallingProhibited = true;
                            flag2 = true;
                        }
                        if (!flag2)
                        {
                            this.m_sequentialPathFails++;
                            IntVector2 key = this.m_aiActor.CompanionOwner.CenterPosition.ToIntVector2(VectorConversions.Floor);
                            CellData cellData2 = GameManager.Instance.Dungeon.data[key];
                            if (this.m_sequentialPathFails > 3 && cellData2 != null && cellData2.IsPassable)
                            {
                                this.m_sequentialPathFails = 0;
                                this.m_aiActor.CompanionWarp(this.m_aiActor.CompanionOwner.CenterPosition);
                            }
                        }
                    }
                    else
                    {
                        this.m_sequentialPathFails = 0;
                    }
                }
                return BehaviorResult.SkipRemainingClassBehaviors;

                #endregion

            }


            #region Companion Tracking Variables
            public float PathInterval = 0.25f;
            public bool DisableInCombat = true;
            public float IdealRadius = 3f;
            public float CatchUpRadius = 7f;
            public float CatchUpAccelTime = 5f;
            public float CatchUpSpeed = 7f;
            public float CatchUpMaxSpeed = 10f;
            private bool m_isCatchingUp;
            private float m_catchUpTime;
            [NonSerialized]
            public bool TemporarilyDisabled;
            private int m_sequentialPathFails;
            private float m_idleTimer = 2f;
            private float m_repathTimer;
            private CompanionController m_companionController;
            #endregion

            #region Pathfinding To Target Variables
            public bool StopWhenInRange = true;
            public float CustomRange = -1f;
            public bool LineOfSight = true;
            public float TargetPathInterval = 0.25f;
            private SeekTargetBehavior.State m_state;
            public float MinActiveRange = 1;
            public float MaxActiveRange = 4;

            protected bool InRange()
            {
                if (!this.m_aiActor.TargetRigidbody)
                {
                    return false;
                }
                float distanceToTarget = this.m_aiActor.DistanceToTarget;
                //Debug.Log($"{distanceToTarget} | {MinActiveRange} | {MaxActiveRange} | {distanceToTarget >= this.MinActiveRange && distanceToTarget <= this.MaxActiveRange} ");
                return distanceToTarget >= this.MinActiveRange && distanceToTarget <= this.MaxActiveRange;
            }
            protected bool InAttackRange()
            {
                if (!this.m_aiActor.TargetRigidbody)
                {
                    return false;
                }
                float distanceToTarget = this.m_aiActor.DistanceToTarget;
                //Debug.Log($"{distanceToTarget} | {MinActiveRange} | {MaxActiveRange} | {distanceToTarget >= this.MinActiveRange && distanceToTarget <= this.MaxActiveRange} ");
                return distanceToTarget >= 2 && distanceToTarget <= 10;
            }
            #endregion


            public void GetTarget()
            {
                if (this.m_losTimer > 0f)
                {
                    return;
                }
                this.m_losTimer = this.SearchInterval;
                if (this.m_aiActor.PlayerTarget)
                {
                    if (this.m_aiActor.PlayerTarget.healthHaver && this.m_aiActor.PlayerTarget.healthHaver.IsDead)
                    {
                        this.m_aiActor.PlayerTarget = null;
                        this.m_aiActor.ClearPath();
                    }
                }
                if (this.m_aiActor.PlayerTarget != null)
                {
                    return;
                }
                if (!this.m_aiActor.CanTargetEnemies)
                {
                    return;
                }
                var room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.m_aiActor.GridPosition);
                if (room != null)
                {
                    List<AIActor> activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    activeEnemies = Strip(activeEnemies);
                    if (activeEnemies != null && activeEnemies.Count > 0)
                    {
                        AIActor playerTarget = null;
                        float num = float.MaxValue;
                        for (int i = 0; i < activeEnemies.Count; i++)
                        {

                            AIActor aiactor = activeEnemies[i];
                            if (aiactor != this.m_aiActor)
                            {
                                if (aiactor.EnemyGuid != FodderEnemy.guid)
                                {
                                    float num2 = Vector2.Distance(this.m_aiActor.CenterPosition, aiactor.CenterPosition);
                                    if (num2 < 12)
                                    {
                                        playerTarget = aiactor;
                                        num = num2;
                                    }
                                }
                            }
                        //IL_258:;
                        }
                        //Debug.Log($":3 {playerTarget != null}");
                        this.m_aiActor.ClearPath();
                        this.m_aiActor.PlayerTarget = playerTarget;
                        return;
                    }
                }

                
                return;
            }

            public List<AIActor> Strip(List<AIActor> aIActors)
            {
                if (aIActors == null)
                    return aIActors;

                var room = new List<AIActor>();
                room.AddRange(aIActors);
                room.RemoveAll(self => self.healthHaver.IsDead);
                room.RemoveAll(self => self.healthHaver.vulnerable == false);
                room.RemoveAll(self => self.spriteAnimator.QueryInvulnerabilityFrame() == true);
                room.RemoveAll(x => x.EnemyGuid == FodderEnemy.guid | x.EnemyGuid == Shamber.guid);
                return room;
            }

            public bool ObjectPermanence = true;
            public float SearchInterval = 0.25f;
            private float m_losTimer;


            public struct BoscoPriorityTarget
            {
                public BoscoPriorityTarget(BoscoBehaviorType _boscoBehaviorType, Vector2 _Position, float MinDist, float MaxDist, Vector2? offset = null)
                {
                    boscoBehaviorType = _boscoBehaviorType;
                    Position = _Position;
                    isBody = false;
                    PriorityBody = null;
                    IsNearMinMax = new Vector2(MinDist, MaxDist);
                    Offset = offset ?? Vector2.zero;
                }
                public BoscoPriorityTarget(BoscoBehaviorType _boscoBehaviorType, SpeculativeRigidbody _Body, float MinDist, float MaxDist, Vector2? offset = null)
                {
                    boscoBehaviorType = _boscoBehaviorType;
                    Position = Vector2.zero;
                    isBody = true;
                    PriorityBody = _Body;
                    IsNearMinMax = new Vector2(MinDist, MaxDist);
                    Offset = offset ?? Vector2.zero;
                }


                public Vector2 ToPriorityPosition(ref bool FailedPath)
                {
                    if (isBody)
                    {
                        if (PriorityBody == null)
                        {
                            FailedPath = true;
                            return Vector2.zero;
                        }
                        return PriorityBody.specRigidbody.UnitCenter + Offset;
                    }
                    return Position + Offset;
                }

                private bool isBody;

                public SpeculativeRigidbody PriorityBody;
                public Vector2 Position;
                public BoscoBehaviorType boscoBehaviorType;
                public Vector2 IsNearMinMax;
                public Vector2 Offset;

                public enum BoscoBehaviorType
                {
                    Taze,
                    PriorityAttack,
                    SecretRoomReveal,
                    AreaDamage,
                    ChestTamper,
                    PedestalMimicHoldDown,
                    FireplaceWater,
                    Unlock
                }

                public bool isNear(Vector2 BoscoPos, Vector2 To)
                {
                    var d = Vector2.Distance(To, BoscoPos);
                    return d >= this.IsNearMinMax.x && d <= this.IsNearMinMax.y;
                }
            }

            public BoscoPriorityTarget? CurrentBoscoPriority;

            public static void AddPriorityToAllBoscos(BoscoPriorityTarget? boscoPriorityTarget, PlayerController playerController)
            {
                //Debug.Log(boscoPriorityTarget != null ? boscoPriorityTarget.Value.boscoBehaviorType.ToString() : "");
                AllBoscos.RemoveAll(x => x.m_aiActor == null);
                var _ = AllBoscos.Where(x => x.m_companionController.m_owner == playerController);
                if (_ != null && _.Count() > 0)
                {
                    foreach (var boscos in _)
                    {
                        if (boscos.ValidBosco(boscoPriorityTarget))
                        {
                            boscos.CurrentBoscoPriority = boscoPriorityTarget;
                            boscos.TargetAcquired();
                        }
                        else
                        {
                            boscos.FailedTarget();
                        }
                    }
                }
            }

            #region Perform Priority Behavior
            private GameObject instEffectOverhead;
            public void TargetAcquired()
            {
                if (instEffectOverhead != null)
                    UnityEngine.Object.Destroy(instEffectOverhead);
                instEffectOverhead = m_aiActor.SmarterPlayEffectOnActor(BoscoDesignator.BoscoEffectInst.gameObject, new Vector3(0, 1));
                instEffectOverhead.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("bosco_warn");
                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = m_aiActor.sprite.WorldCenter,
                    startColor = Color.red.WithAlpha(0.2f),
                    startLifetime = 0.2f,
                    startSize = 8
                });
                AkSoundEngine.PostEvent("Play_OBJ_supplydrop_activate_01", m_aiActor.gameObject);
            }
            public void FailedTarget()
            {
                if (instEffectOverhead != null)
                    UnityEngine.Object.Destroy(instEffectOverhead);
                instEffectOverhead = m_aiActor.SmarterPlayEffectOnActor(BoscoDesignator.BoscoEffectInst.gameObject, new Vector3(0, 1));
                instEffectOverhead.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("bosco_nope");
                AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", m_aiActor.gameObject);
            }

            public bool ValidBosco(BoscoPriorityTarget? boscoPriorityTarget)
            {
                if (boscoPriorityTarget == null)
                {
                    return true;
                }
                if (boscoPriorityTarget.Value.PriorityBody != null)
                {
                    if (BodiesToIgnore.Contains(boscoPriorityTarget.Value.PriorityBody))
                    {
                        return false;
                    }
                }
                return true;
            }

            public static List<SpeculativeRigidbody> BodiesToIgnore = new List<SpeculativeRigidbody>();

            private float DamageTick;

            public void DoPerformPriorityBehavior()
            {
                DecrementTimer(ref DamageTick, true);

                var priority = CurrentBoscoPriority.Value;

                switch (priority.boscoBehaviorType)
                {
                    case BoscoPriorityTarget.BoscoBehaviorType.Taze:
                        if (priority.PriorityBody.aiActor && priority.PriorityBody.aiActor.behaviorSpeculator)
                        {
                            priority.PriorityBody.aiActor.behaviorSpeculator.Stun(0.5f);
                            priority.PriorityBody.aiActor.behaviorSpeculator.UpdateStun(0.5f);
                            SetTazeIdle();
                            if (DamageTick <= 0)
                            {
                                AkSoundEngine.PostEvent("Play_ENV_puddle_zap_01", m_aiActor.gameObject);
                                DamageTick = 0.25f;
                                priority.PriorityBody.aiActor.healthHaver.ApplyDamage(2.5f, Vector2.zero, "Taze");
                                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                                {
                                    position = m_aiActor.sprite.WorldCenter,
                                    startColor = Color.cyan.WithAlpha(0.25f),
                                    startLifetime = 0.25f,
                                    startSize = 3
                                });

                                GameObject breakVFX = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.MildElectricImpactVFX, m_aiActor.sprite.WorldBottomCenter + new Vector2(UnityEngine.Random.Range(0.125f, -.125f), UnityEngine.Random.Range(0.125f, -0.125f)), Quaternion.identity);
                                tk2dBaseSprite component = breakVFX.GetComponent<tk2dBaseSprite>();
                                component.PlaceAtPositionByAnchor(m_aiActor.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(1.25f, -1.25f), UnityEngine.Random.Range(0.625f, -1.25f)), tk2dBaseSprite.Anchor.MiddleCenter);
                                component.HeightOffGround = 35f;
                                component.UpdateZDepth();
                                
                            }
                        }
                        break;
                    case BoscoPriorityTarget.BoscoBehaviorType.SecretRoomReveal:
                        if (!isPerformingPriorityTask)
                        {
                            isPerformingPriorityTask = true;
                            this.m_aiActor.StartCoroutine(DoBlankForSecret());
                        }
                        break;
                    case BoscoPriorityTarget.BoscoBehaviorType.ChestTamper:
                        if (!isPerformingPriorityTask)
                        {
                            isPerformingPriorityTask = true;
                            this.m_aiActor.StartCoroutine(DoChestTamper());
                        }
                        break;
                    case BoscoPriorityTarget.BoscoBehaviorType.PriorityAttack:
                        break;
                    case BoscoPriorityTarget.BoscoBehaviorType.PedestalMimicHoldDown:
                        var pedestal = CurrentBoscoPriority.Value.PriorityBody.GetComponent<RewardPedestal>();
                        if (pedestal.m_itemDisplaySprite)
                        {
                            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"));
                            tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
                            component.PlaceAtPositionByAnchor(pedestal.m_itemDisplaySprite.WorldCenter.ToVector3ZUp(0f), tk2dBaseSprite.Anchor.MiddleCenter);
                            component.HeightOffGround = 5f;
                            component.UpdateZDepth();
                        }
                        pedestal.sprite.UpdateZDepth();
                        IntVector2 intVector = pedestal.specRigidbody.UnitBottomLeft.ToIntVector2(VectorConversions.Floor);
                        IntVector2 intVector2 = pedestal.specRigidbody.UnitTopRight.ToIntVector2(VectorConversions.Floor);
                        for (int i = intVector.x; i <= intVector2.x; i++)
                        {
                            for (int j = intVector.y; j <= intVector2.y; j++)
                            {
                                GameManager.Instance.Dungeon.data[new IntVector2(i, j)].isOccupied = false;
                            }
                        }
                        if (!pedestal.pickedUp)
                        {
                            pedestal.pickedUp = true;
                            pedestal.m_room.DeregisterInteractable(pedestal);
                        }
                        if (pedestal.m_registeredIconRoom != null)
                        {
                            Minimap.Instance.DeregisterRoomIcon(pedestal.m_registeredIconRoom, pedestal.minimapIconInstance);
                        }
                        AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(pedestal.MimicGuid);
                        AIActor aiactor = AIActor.Spawn(orLoadByGuid, pedestal.transform.position.XY().ToIntVector2(VectorConversions.Floor), pedestal.GetAbsoluteParentRoom(), false, AIActor.AwakenAnimationType.Default, true);
                        aiactor.AdditionalSafeItemDrops.Add(pedestal.contents);
                        CurrentBoscoPriority = new BoscoPriorityTarget(BoscoPriorityTarget.BoscoBehaviorType.Taze, aiactor.specRigidbody, -1f, 1.25f);

                        PickupObject.ItemQuality itemQuality = (!BraveUtility.RandomBool()) ? PickupObject.ItemQuality.C : PickupObject.ItemQuality.D;
                        GenericLootTable genericLootTable = (!BraveUtility.RandomBool()) ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable;
                        PickupObject itemOfTypeAndQuality = LootEngine.GetItemOfTypeAndQuality<PickupObject>(itemQuality, genericLootTable, false);
                        if (itemOfTypeAndQuality)
                        {
                            aiactor.AdditionalSafeItemDrops.Add(itemOfTypeAndQuality);
                        }
                        aiactor.specRigidbody.Initialize();
                        Vector2 unitBottomLeft = aiactor.specRigidbody.UnitBottomLeft;
                        Vector2 unitBottomLeft2 = pedestal.specRigidbody.UnitBottomLeft;
                        aiactor.transform.position -= (unitBottomLeft - unitBottomLeft2).ToVector3XUp();
                        aiactor.transform.position += PhysicsEngine.PixelToUnit(pedestal.mimicOffset).ToVector3XUp();
                        aiactor.specRigidbody.Reinitialize();
                        aiactor.HasDonePlayerEnterCheck = true;
                        GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_HAS_BEEN_PEDESTAL_MIMICKED, true);
                        UnityEngine.Object.Destroy(pedestal.gameObject);
                        break;

                    case BoscoPriorityTarget.BoscoBehaviorType.AreaDamage:
                        if (!isPerformingPriorityTask)
                        {
                            isPerformingPriorityTask = true;
                            this.m_aiActor.StartCoroutine(DoAreaPush());
                        }
                        break;
                    case BoscoPriorityTarget.BoscoBehaviorType.FireplaceWater:
                        if (!isPerformingPriorityTask)
                        {
                            isPerformingPriorityTask = true;
                            this.m_aiActor.StartCoroutine(Piss());
                        }
                        break;
                    case BoscoPriorityTarget.BoscoBehaviorType.Unlock:
                        if (!isPerformingPriorityTask)
                        {
                            isPerformingPriorityTask = true;
                            this.m_aiActor.StartCoroutine(DoLockTamper());
                        }
                        break;
                    default:
                        break;
                }
            }
            public IEnumerator DoLockTamper()
            {
                var Chest = CurrentBoscoPriority.Value.PriorityBody.GetComponent<InteractableLock>();
                if (Chest == null)
                {
                    Chest = CurrentBoscoPriority.Value.PriorityBody.GetComponentInChildren<InteractableLock>();
                }


                if (Chest == null)
                {
                    isPerformingPriorityTask = false;
                    CurrentBoscoPriority = null;
                    m_aiActor.OverrideTarget = null;
                    yield break;
                }


                if (Chest.IsLocked == false)
                {
                    isPerformingPriorityTask = false;
                    CurrentBoscoPriority = null;
                    m_aiActor.OverrideTarget = null;
                    yield break;
                }

                SetTazeIdle();

                AkSoundEngine.PostEvent("Play_SawLoop", Chest.gameObject);
                AkSoundEngine.PostEvent("Play_SawStart", Chest.gameObject);

                float e = 0;
                while (e < 3)
                {

                    if (!Chest.IsLocked || Chest.IsBusted)
                    {
                        AkSoundEngine.PostEvent("Play_MetalImpactHit", m_aiActor.gameObject);
                        BodiesToIgnore.Add(Chest.specRigidbody);
                        CurrentBoscoPriority = null;
                        isPerformingPriorityTask = false;
                        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = m_aiActor.sprite.WorldCenter,
                            startColor = Color.yellow.WithAlpha(0.2f),
                            startLifetime = 0.2f,
                            startSize = 5
                        });
                        m_aiActor.OverrideTarget = null;
                        yield break;
                    }

                    if (Chest == null)
                    {
                        AkSoundEngine.PostEvent("Play_MetalImpactHit", m_aiActor.gameObject);
                        CurrentBoscoPriority = null;
                        isPerformingPriorityTask = false;
                        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = m_aiActor.sprite.WorldCenter,
                            startColor = Color.yellow.WithAlpha(0.2f),
                            startLifetime = 0.2f,
                            startSize = 5
                        });
                        m_aiActor.OverrideTarget = null;
                        yield break;
                    }



                    e += BraveTime.DeltaTime;
                    ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = Chest.sprite.WorldCenter,
                        rotation = 0,
                        startLifetime = 0.2f,
                        startSize = 0.125f,
                        startColor = Color.yellow,
                        velocity = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), Mathf.Min(e, 3) * UnityEngine.Random.Range(2.4f, 5.3f))
                    });
                    yield return null;
                }
                BodiesToIgnore.Add(Chest.specRigidbody);
                AkSoundEngine.PostEvent("Stop_SawLoop", Chest.gameObject);
                m_aiActor.OverrideTarget = null;

                if (UnityEngine.Random.value < 0.5)
                {
                    AkSoundEngine.PostEvent("Play_OBJ_lock_pick_01", m_aiActor.gameObject);
                    Chest.ForceUnlock();
                    ResetIdle();
                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = m_aiActor.sprite.WorldCenter,
                        startColor = Color.white.WithAlpha(0.5f),
                        startLifetime = 0.333f,
                        startSize = 8
                    });
                    this.m_aiActor.aiAnimator.Play("bosco_yippie", AIAnimator.AnimatorState.StateEndType.UntilFinished, 1.25f, -1, true, "");
                    while (this.m_aiActor.aiAnimator.IsPlaying("bosco_yippie"))
                    {
                        yield return null;
                    }
                }
                else
                {

                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = m_aiActor.sprite.WorldCenter,
                        startColor = Color.yellow.WithAlpha(0.5f),
                        startLifetime = 0.333f,
                        startSize = 8
                    });
                    AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", m_aiActor.gameObject);
                    AkSoundEngine.PostEvent("Play_MetalImpactHit", m_aiActor.gameObject);
                    ResetIdle();
                }
                CurrentBoscoPriority = null;
                isPerformingPriorityTask = false;
                yield break;
            }


            private IEnumerator DoBlankForSecret()
            {
                float t = 0.5f;
                float amount = 1;
                this.m_aiActor.aiAnimator.Play("bosco_zzap_start", AIAnimator.AnimatorState.StateEndType.Duration, 1.5f, -1, true, "");
                while (this.m_aiActor.aiAnimator.IsPlaying("bosco_zzap_start"))
                {
                    t -= BraveTime.DeltaTime;
                    if (t <= 0)
                    {
                        t = 0.125f;
                        ParticleBase.EmitParticles("WaveParticleInverse", 1, new ParticleSystem.EmitParams()
                        {
                            position = m_aiActor.sprite.WorldCenter,
                            startColor = Color.white.WithAlpha(0.2f),
                            startLifetime = 0.25f,
                            startSize = 4 * amount
                        });
                        amount += .25f;
                    }


                    yield return null;
                }

                var BlankVFXPrefab = (GameObject)BraveResources.Load("Global VFX/BlankVFX", ".prefab");
                AkSoundEngine.PostEvent("Play_OBJ_silenceblank_use_01", this.m_aiActor.gameObject);
                AkSoundEngine.PostEvent("Stop_ENM_attack_cancel_01", this.m_aiActor.gameObject);
                GameObject gameObject = new GameObject("silencer");
                SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
                silencerInstance.TriggerSilencer(this.m_aiActor.sprite.WorldCenter, 50f, 25, BlankVFXPrefab, 0.15f, 0.2f, 50, 10, 140f, 15, 0.5f, this.m_companionController.m_owner, true, false);
                this.m_companionController.m_owner.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);


                this.m_aiActor.aiAnimator.Play("bosco_zzap_loop", AIAnimator.AnimatorState.StateEndType.Duration, 0.25f, -1, true, "");
                while (this.m_aiActor.aiAnimator.IsPlaying("bosco_zzap_loop"))
                {
                    yield return null;
                }
                this.m_aiActor.aiAnimator.Play("bosco_zzap_end", AIAnimator.AnimatorState.StateEndType.UntilFinished, 0.25f, -1, true, "");
                while (this.m_aiActor.aiAnimator.IsPlaying("bosco_zzap_end"))
                {
                    yield return null;
                }
                m_aiActor.OverrideTarget = null;
                CurrentBoscoPriority = null;
                isPerformingPriorityTask = false;
                yield break;
            }
            public IEnumerator DoChestTamper()
            {
                var Chest = CurrentBoscoPriority.Value.PriorityBody.GetComponent<Chest>();
                if  (Chest == null )
                {
                    Chest = CurrentBoscoPriority.Value.PriorityBody.GetComponentInChildren<Chest>();
                }


                if (Chest == null)
                {
                    isPerformingPriorityTask = false;
                    CurrentBoscoPriority = null;
                    m_aiActor.OverrideTarget = null;
                    yield break;
                }
                if (Chest.IsMirrorChest)
                {
                    isPerformingPriorityTask = false;
                    CurrentBoscoPriority = null;
                    m_aiActor.OverrideTarget = null;
                    yield break;
                }

                if (Chest.IsMimic)
                {
                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = m_aiActor.sprite.WorldCenter,
                        startColor = Color.red.WithAlpha(0.2f),
                        startLifetime = 0.25f,
                        startSize = 8
                    });
                    isPerformingPriorityTask = false;
                    m_aiActor.OverrideTarget = null;
                    GameManager.Instance.platformInterface.AchievementUnlock(Achievement.PREFIRE_ON_MIMIC, 0);
                    Chest.DetermineContents(m_companionController.m_owner);
                    ConvertChestToMimicButWithImmediateBoscoPriority(Chest, Chest.contents);
                    yield break;

                }


                if (Chest.IsLocked == false)
                {
                    isPerformingPriorityTask = false;
                    CurrentBoscoPriority = null;
                    m_aiActor.OverrideTarget = null;
                    yield break;
                }

                SetTazeIdle();

                AkSoundEngine.PostEvent("Play_SawLoop", Chest.gameObject);
                AkSoundEngine.PostEvent("Play_SawStart", Chest.gameObject);

                float e = 0;
                while (e < 5)
                {

                    if (Chest.majorBreakable.IsDestroyed || !Chest.IsLocked)
                    {
                        AkSoundEngine.PostEvent("Play_MetalImpactHit", m_aiActor.gameObject);
                        BodiesToIgnore.Add(Chest.specRigidbody);
                        CurrentBoscoPriority = null;
                        isPerformingPriorityTask = false;
                        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = m_aiActor.sprite.WorldCenter,
                            startColor = Color.yellow.WithAlpha(0.2f),
                            startLifetime = 0.2f,
                            startSize = 5
                        });
                        m_aiActor.OverrideTarget = null;
                        yield break;
                    }

                    if (Chest == null)
                    {
                        AkSoundEngine.PostEvent("Play_MetalImpactHit", m_aiActor.gameObject);
                        CurrentBoscoPriority = null;
                        isPerformingPriorityTask = false;
                        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = m_aiActor.sprite.WorldCenter,
                            startColor = Color.yellow.WithAlpha(0.2f),
                            startLifetime = 0.2f,
                            startSize = 5
                        });
                        m_aiActor.OverrideTarget = null;
                        yield break;
                    }



                        e += BraveTime.DeltaTime;
                    ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = Chest.LockAnimator.sprite.WorldCenter,
                        rotation = 0,
                        startLifetime = 0.2f,
                        startSize = 0.125f,
                        startColor = Color.yellow,
                        velocity = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), Mathf.Min(e, 3) * UnityEngine.Random.Range(2.4f, 5.3f))
                    });
                    yield return null;
                }
                BodiesToIgnore.Add(Chest.specRigidbody);
                AkSoundEngine.PostEvent("Stop_SawLoop", Chest.gameObject);
                m_aiActor.OverrideTarget = null;

                if (UnityEngine.Random.value < 0.3)
                {
                    AkSoundEngine.PostEvent("Play_OBJ_lock_pick_01", m_aiActor.gameObject);
                    Chest.Unlock();
                    ResetIdle();
                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = m_aiActor.sprite.WorldCenter,
                        startColor = Color.white.WithAlpha(0.5f),
                        startLifetime = 0.333f,
                        startSize = 8
                    });
                    this.m_aiActor.aiAnimator.Play("bosco_yippie", AIAnimator.AnimatorState.StateEndType.UntilFinished, 1.25f, -1, true, "");
                    while (this.m_aiActor.aiAnimator.IsPlaying("bosco_yippie"))
                    {
                        yield return null;
                    }
                }
                else
                {

                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = m_aiActor.sprite.WorldCenter,
                        startColor = Color.yellow.WithAlpha(0.5f),
                        startLifetime = 0.333f,
                        startSize = 8
                    });
                    AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", m_aiActor.gameObject);
                    AkSoundEngine.PostEvent("Play_MetalImpactHit", m_aiActor.gameObject);
                    ResetIdle();
                }
                CurrentBoscoPriority = null;
                isPerformingPriorityTask = false;
                yield break;
            }
            private void ConvertChestToMimicButWithImmediateBoscoPriority(Chest chest, List<PickupObject> overrideDeathRewards)
            {
                IntVector2 intVector = chest.specRigidbody.UnitBottomLeft.ToIntVector2(VectorConversions.Floor);
                IntVector2 intVector2 = chest.specRigidbody.UnitTopRight.ToIntVector2(VectorConversions.Floor);
                for (int i = intVector.x; i <= intVector2.x; i++)
                {
                    for (int j = intVector.y; j <= intVector2.y; j++)
                    {
                        GameManager.Instance.Dungeon.data[new IntVector2(i, j)].isOccupied = false;
                    }
                }
                if (!chest.pickedUp)
                {
                    chest.pickedUp = true;
                    chest.m_room.DeregisterInteractable(chest);
                }
                if (chest.m_registeredIconRoom != null)
                {
                    Minimap.Instance.DeregisterRoomIcon(chest.m_registeredIconRoom, chest.minimapIconInstance);
                }
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(chest.MimicGuid);
                AIActor aiactor = AIActor.Spawn(orLoadByGuid, chest.transform.position.XY().ToIntVector2(VectorConversions.Floor), chest.GetAbsoluteParentRoom(), false, AIActor.AwakenAnimationType.Default, true);
                if (overrideDeathRewards != null)
                {
                    aiactor.AdditionalSafeItemDrops.AddRange(overrideDeathRewards);
                }
                aiactor.specRigidbody.Initialize();
                Vector2 unitBottomLeft = aiactor.specRigidbody.UnitBottomLeft;
                Vector2 unitBottomLeft2 = chest.specRigidbody.UnitBottomLeft;
                aiactor.transform.position -= (unitBottomLeft - unitBottomLeft2).ToVector3ZUp();
                aiactor.transform.position += PhysicsEngine.PixelToUnit(chest.mimicOffset).ToVector3ZUp();
                aiactor.specRigidbody.Reinitialize();
                aiactor.HasDonePlayerEnterCheck = true;

                CurrentBoscoPriority = new BoscoPriorityTarget(BoscoPriorityTarget.BoscoBehaviorType.Taze ,aiactor.specRigidbody, -1f, 1.25f);
                UnityEngine.Object.Destroy(chest.gameObject);
            }


            private IEnumerator DoAreaPush()
            {
                float t = 0.5f;
                float amount = 1;
                this.m_aiActor.aiAnimator.Play("bosco_zzap_start", AIAnimator.AnimatorState.StateEndType.Duration, 1f, -1, true, "");
                while (this.m_aiActor.aiAnimator.IsPlaying("bosco_zzap_start"))
                {
                    t -= BraveTime.DeltaTime;
                    if (t <= 0)
                    {
                        t = 0.125f;
                        ParticleBase.EmitParticles("WaveParticleInverse", 1, new ParticleSystem.EmitParams()
                        {
                            position = m_aiActor.sprite.WorldCenter,
                            startColor = Color.white.WithAlpha(0.2f),
                            startLifetime = 0.25f,
                            startSize = 3 * amount
                        });
                        amount += .25f;
                    }


                    yield return null;
                }

                var allNearby = StaticReferenceManager.AllMajorBreakables.Where(x => Vector2.Distance(x.majorBreakable.specRigidbody.UnitCenter, m_aiActor.specRigidbody.UnitCenter) < 3);
                foreach (var entry in allNearby)
                {
                    if (entry != null)
                    {
                        entry.ApplyDamage(1E+10f, Vector2.zero, false, true, true);
                        LeadBaton.Impact.SpawnAtPosition(entry.majorBreakable.specRigidbody.ClosestPointOnRigidBody(m_aiActor.specRigidbody.UnitCenter));
                    }
                }

                RoomHandler room = m_aiActor.transform.position.GetAbsoluteRoom();
                if (room != null)
                {
                    room.ApplyActionToNearbyEnemies(m_aiActor.specRigidbody.UnitCenter, 3.5f, (enemy, f) =>
                    {
                        enemy.healthHaver.ApplyDamage(30, Vector2.zero, "Bonk");
                        enemy.knockbackDoer.ApplyKnockback((enemy.majorBreakable.specRigidbody.UnitCenter - m_aiActor.specRigidbody.UnitCenter), 250);
                    });
                }
                AkSoundEngine.PostEvent("Play_WPN_Vorpal_Shot_Critical_01", m_aiActor.gameObject);

                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = m_aiActor.sprite.WorldCenter,
                    startColor = Color.white.WithAlpha(0.33f),
                    startLifetime = 0.125f,
                    startSize = 5
                });
                for (int i = 0; i < 64; i++)
                {
                    GlobalSparksDoer.DoRandomParticleBurst(1, m_aiActor.sprite.WorldCenter + new Vector2(-0.5f, 0.5f), m_aiActor.sprite.WorldCenter + new Vector2(0.5f, 0.5f),
                        BraveUtility.RandomVector2(new Vector2(-6, -6), new Vector2(6, 6)),
                        0f,
                        0.5f,
                        null,
                        UnityEngine.Random.Range(0.8f, 1.4f),
                        new Color(0.7f, 0.9f, 0.7f, 1),
                        GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                }


                this.m_aiActor.aiAnimator.Play("bosco_zzap_loop", AIAnimator.AnimatorState.StateEndType.Duration, 0.25f, -1, true, "");
                while (this.m_aiActor.aiAnimator.IsPlaying("bosco_zzap_loop"))
                {
                    yield return null;
                }
                this.m_aiActor.aiAnimator.Play("bosco_zzap_end", AIAnimator.AnimatorState.StateEndType.UntilFinished, 0.25f, -1, true, "");
                while (this.m_aiActor.aiAnimator.IsPlaying("bosco_zzap_end"))
                {
                    yield return null;
                }
                m_aiActor.OverrideTarget = null;
                CurrentBoscoPriority = null;
                isPerformingPriorityTask = false;
                yield break;
            }
            private IEnumerator Piss()
            {

                this.m_aiActor.aiAnimator.Play("bosco_zzap_start", AIAnimator.AnimatorState.StateEndType.Duration, 0.75f, -1, true, "");
                while (this.m_aiActor.aiAnimator.IsPlaying("bosco_zzap_start"))
                {
                    yield return null;
                }
               
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopUtility.WaterDef).TimedAddGoopCircle(m_aiActor.sprite.WorldBottomCenter, 3, 0.5f);


                AkSoundEngine.PostEvent(Actives.Coolant_Leak.AudioEvent, m_aiActor.gameObject);

                for (int i = 0; i < 32; i++)
                {
                    GlobalSparksDoer.DoRandomParticleBurst(1, m_aiActor.sprite.WorldCenter + new Vector2(-0.5f, 0.5f), m_aiActor.sprite.WorldCenter + new Vector2(0.5f, 0.5f),
                        BraveUtility.RandomVector2(new Vector2(-2, -2), new Vector2(2, 2)),
                        0f,
                        0.5f,
                        null,
                        UnityEngine.Random.Range(0.8f, 1.4f),
                        new Color(0.7f, 0.9f, 0.7f, 1),
                        GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                }


                this.m_aiActor.aiAnimator.Play("bosco_zzap_loop", AIAnimator.AnimatorState.StateEndType.Duration, 0.25f, -1, true, "");
                while (this.m_aiActor.aiAnimator.IsPlaying("bosco_zzap_loop"))
                {
                    yield return null;
                }
                this.m_aiActor.aiAnimator.Play("bosco_zzap_end", AIAnimator.AnimatorState.StateEndType.UntilFinished, 0.25f, -1, true, "");
                while (this.m_aiActor.aiAnimator.IsPlaying("bosco_zzap_end"))
                {
                    yield return null;
                }
                m_aiActor.OverrideTarget = null;
                CurrentBoscoPriority = null;
                isPerformingPriorityTask = false;
                yield break;
            }

            #endregion

            public void SetTazeIdle()
            {
                m_aiActor.aiAnimator.IdleAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.SixWay,
                    Flipped = new DirectionalAnimation.FlipType[6],
                    AnimNames = new string[]
                    {
                    "bosco_taze_back",
                    "bosco_taze_back_right",
                    "bosco_taze_front_right",
                    "bosco_taze_front",
                    "bosco_taze_front_left",
                    "bosco_taze_back_left",
                    }
                };
            }
            public void ResetIdle()
            {
                m_aiActor.aiAnimator.IdleAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.SixWay,
                    Flipped = new DirectionalAnimation.FlipType[6],
                    AnimNames = new string[]
                    {
                        "bosco_idle_back",
                        "bosco_idle_back_right",
                        "bosco_idle_front_right",
                        "bosco_idle_front",
                        "bosco_idle_front_left",
                        "bosco_idle_back_left"
                    }
                };

            }





            public int StandardFireAmount = 8;
            public int currentStandardFireAmount = 0;

            public float CooldownBurst = 0.02f;
            public float InstCooldownBurst = 0;


            public float Cooldown = 0.75f;
            public float InstCooldown = 0;

            public float PanicAtTheDisco = 1;

        }
    }
}

