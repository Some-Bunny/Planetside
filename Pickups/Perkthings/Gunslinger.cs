using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using MonoMod.RuntimeDetour;
using System.Reflection;
using System.Linq;
using SaveAPI;
using static tk2dSpriteCollectionDefinition;

namespace Planetside
{
    class ForceSpecificMotionModule : MonoBehaviour
    {
        public void Update()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile != null)
            { projectile.OverrideMotionModule = Module; }
        }
        private Projectile projectile;
        public ProjectileAndBeamMotionModule Module;
    }
    class SpawnDebrisObject : MonoBehaviour
    {
        public void OnDestroy()
        {
            if (PickupObjectDatabase.GetById(objIDToSpawn) != null)
            {
                DebrisObject obj = LootEngine.SpawnItem(PickupObjectDatabase.GetById(objIDToSpawn).gameObject, base.transform.position, Vector2.up, 0f, true, true, false);
                PickupMover pickupMover = obj.gameObject.AddComponent<PickupMover>();
                if (pickupMover.specRigidbody)
                {
                    pickupMover.specRigidbody.CollideWithTileMap = false;
                }
                obj.sprite.IsPerpendicular = true;
                obj.sprite.automaticallyManagesDepth = true;
                obj.sprite.ignoresTiltworldDepth = true;
                obj.FlagAsPickup();
                obj.canRotate = true;
                obj.motionMultiplier = 0;
                pickupMover.acceleration = 10f;
                pickupMover.maxSpeed = 6f;
                pickupMover.minRadius = 1f;
                pickupMover.moveIfRoomUnclear = true;
                pickupMover.stopPathingOnContact = true;

                Gun gunComp = obj.GetComponentInChildren<Gun>();
                if (gunComp != null)
                {
                    gunComp.ammo = 0;
                }
            }

        }
        public int objIDToSpawn;
    }
    class ShotgunController : MonoBehaviour
    {
        public void Start()
        {
            proj = base.gameObject.GetComponent<Projectile>();
            player = proj.Owner as PlayerController;
            if (proj != null && player != null)
            { proj.OnHitEnemy += OnHitEnemy; }
        }

        public void OnHitEnemy(Projectile projectile, SpeculativeRigidbody speculativeRigidbody, bool fatal)
        {
            if (speculativeRigidbody && speculativeRigidbody.aiActor)
            {
                AIActor aiActor = speculativeRigidbody.aiActor;

                Gun gunComp = projectile.GetComponentInChildren<Gun>();

                if (aiActor.IsNormalEnemy && !aiActor.IsHarmlessEnemy && gunComp != null)
                {
                    GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, player.CenterPosition.ToVector3ZisY(0f), Quaternion.identity);
                    gameObject1.transform.parent = speculativeRigidbody.transform;
                    HoveringGunController hoverToDest1 = gameObject1.GetComponent<HoveringGunController>();
                    Destroy(hoverToDest1);
                    CustomHoveringGunController hover1 = gameObject1.AddComponent<CustomHoveringGunController>();
                    hover1.attachObject = aiActor.gameObject;
                    hover1.ConsumesTargetGunAmmo = false;
                    hover1.ChanceToConsumeTargetGunAmmo = 0f;
                    hover1.Position = CustomHoveringGunController.HoverPosition.CIRCULATE;
                    hover1.Aim = CustomHoveringGunController.AimType.NEAREST_ENEMY;
                    hover1.Trigger = CustomHoveringGunController.FireType.ON_COOLDOWN;
                    hover1.CooldownTime = 0.9f;
                    hover1.ShootDuration = 0.2f;
                    hover1.OnlyOnEmptyReload = false;
                    hover1.Initialize(gunComp, player);
                    hover1.RotationSpeed = 120;
                    SpawnDebrisObject sDo = gameObject1.AddComponent<SpawnDebrisObject>();
                    sDo.objIDToSpawn = gunComp.PickupObjectId;

                    LootEngine.DoDefaultItemPoof(projectile.transform.PositionVector2(), true, true);
                    Destroy(projectile.gameObject);
                }
            }
        }


        public void Update()
        {

        }

        public Projectile proj;
        public PlayerController player;
    }
    class ChargeGunController : MonoBehaviour
    {

        public ChargeGunController()
        {player = GameManager.Instance.PrimaryPlayer; }
        public void Start()
        {
            self = base.gameObject.GetComponent<DebrisObject>();
        }

      
        public void Update()
        {
            if (self)
            {
                Timer += BraveTime.DeltaTime;
                if (Timer >= 1f)
                {
                    List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    bool flag = activeEnemies != null;
                    if (flag)
                    {
                        foreach (AIActor aiactor in activeEnemies)
                        {
                            if (aiactor != null && Vector2.Distance(aiactor.CenterPosition, self.transform.position) <  10)
                            {
                                if (!aiactor.healthHaver.IsBoss)
                                {
                                    Vector2 a = aiactor.transform.position - self.transform.position;
                                    aiactor.knockbackDoer.ApplyKnockback(-a, 30f * (Vector2.Distance(self.transform.position, aiactor.transform.position) + 0.01f), false);
                                }
                            }
                        }
                        GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(self.transform.position, 0.75f, 0.1f, 11f, 0.5f));
                    }
                    Timer = 0;
                }
            }            
        }

        private float Timer;

        public DebrisObject self;
        public PlayerController player;
    }
    class CharmController : MonoBehaviour
    {
        public void Start()
        {
            proj = base.gameObject.GetComponent<Projectile>();
            player = proj.Owner as PlayerController;
            if (proj != null && player != null)
            { proj.OnHitEnemy += OnHitEnemy; }
        }

        public void OnHitEnemy(Projectile projectile, SpeculativeRigidbody speculativeRigidbody, bool fatal)
        {
            if (speculativeRigidbody && speculativeRigidbody.aiActor)
            {
                AIActor aiActor = speculativeRigidbody.aiActor;
                if (aiActor.IsNormalEnemy && !aiActor.healthHaver.IsBoss && !aiActor.IsHarmlessEnemy && !aiActor.gameObject.GetComponent<MindControlEffect>() && aiActor.healthHaver.GetCurrentHealthPercentage() <= 0.5f )
                {
                    MindControlEffect orAddComponent = aiActor.gameObject.GetOrAddComponent<MindControlEffect>();
                    orAddComponent.owner = (projectile.Owner as PlayerController);
                }
            }
        }


        public void Update()
        {
           
        }

        public Projectile proj;
        public PlayerController player;
    }
    class FullAutoController : MonoBehaviour
    {
        public void Start()
        {
            proj = base.gameObject.GetComponent<Projectile>();
            player = proj.Owner as PlayerController;
            if (proj != null && player != null)
            { }
        }
        public void Update()
        {
            Timer += BraveTime.DeltaTime;
            if (self != null)
            {
                if (Timer >= self.DefaultModule.cooldownTime * 2f)
                {
                    Timer = 0;
                    ProjectileModule defaultModule = self.DefaultModule;
                    Projectile currentProjectile = defaultModule.GetCurrentProjectile();
                    if (currentProjectile)
                    {
                        float angleForShot = defaultModule.GetAngleForShot(1f, 1f, null);
                        bool flag = currentProjectile.GetComponent<BeamController>() != null;
                        if (!flag)
                        {
                            this.DoSingleProjectile(currentProjectile, player, self.transform.eulerAngles.z + angleForShot, new Vector2?(self.barrelOffset.transform.PositionVector2()), true);
                        }
                    }
                }
            }
           
        }
        private void DoSingleProjectile(Projectile projectileToSpawn, PlayerController source, float targetAngle, Vector2? overrideSpawnPoint, bool doAudio = false)
        {
            Vector2 v = (overrideSpawnPoint == null) ? source.specRigidbody.UnitCenter : overrideSpawnPoint.Value;
            GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, v, Quaternion.Euler(0f, 0f, targetAngle), true);
            gameObject.transform.parent = base.gameObject.transform;
            Projectile component = gameObject.GetComponent<Projectile>();
            component.Owner = source;
            component.Shooter = source.specRigidbody;
            component.specRigidbody.CollideWithTileMap = false;
            component.baseData.damage *= 0.7f;
            source.DoPostProcessProjectile(component);
            ForceSpecificMotionModule mod = component.gameObject.AddComponent<ForceSpecificMotionModule>();
            mod.Module = new OrbitProjectileMotionModule()
            {
                usesAlternateOrbitTarget = true,
                alternateOrbitTarget = base.gameObject.GetComponent<SpeculativeRigidbody>(),
                ForceInvert = UnityEngine.Random.value > 0.5f ? true : false,
                lifespan = 10,             
            };
            var dest = component.gameObject.AddComponent<DestroyMyBad>();
            dest.p = component;
            dest.Timer = 15;
        }
        private float Timer;
        public Projectile proj;
        public PlayerController player;
        public Gun self;

        public class DestroyMyBad : MonoBehaviour
        {
            public Projectile p;
            public float Timer = 10;
            private float e;
            public void Update()
            {
                if (e < Timer)
                {
                    e += BraveTime.DeltaTime;
                }
                else
                {
                    if (p != null)
                    {
                        p.DieInAir();
                    }
                }
            }

        }
    }
    class ExploCont : MonoBehaviour
    {
        public void Start()
        {
            proj = base.gameObject.GetComponent<Projectile>();
            player = proj.Owner as PlayerController;
        }

        public void Update()
        {
            Timer += BraveTime.DeltaTime;
            if (Timer >= 0.75f)
            {
                Timer = 0;
                UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(108).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.gameObject, base.gameObject.transform.PositionVector2(), Quaternion.identity);
            }
        }

        private float Timer;
        public Projectile proj;
        public PlayerController player;
    }

    class GlitterCont : MonoBehaviour 
    {
        public void Start()
        {
            proj = base.gameObject.GetComponent<Projectile>();
            player = proj.Owner as PlayerController;
            if (proj != null&& player != null)
            {proj.OnHitEnemy += OnHitEnemy;}
        }

        public void OnHitEnemy(Projectile projectile, SpeculativeRigidbody speculativeRigidbody, bool fatal)
        {
            if (speculativeRigidbody && speculativeRigidbody.aiActor && speculativeRigidbody.healthHaver)
            {

                FleePlayerData fleeData = new FleePlayerData();
                fleeData.StartDistance = 100f;
                fleeData.Player = player;
                speculativeRigidbody.behaviorSpeculator.FleePlayerData = fleeData;
                speculativeRigidbody.aiActor.ApplyGlitter();
            }
        }
       
        public void Update()
        {
            if (proj)
            {
                GameObject original = (GameObject)ResourceCache.Acquire(GlitterCont.confettiPaths[UnityEngine.Random.Range(0, 3)]);
                WaftingDebrisObject component = UnityEngine.Object.Instantiate<GameObject>(original).GetComponent<WaftingDebrisObject>();
                component.sprite.PlaceAtPositionByAnchor(proj.transform.PositionVector2().ToVector3ZUp(0f) + new Vector3(0.5f, 0.5f, 0f), tk2dBaseSprite.Anchor.MiddleCenter);
                Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                insideUnitCircle.y = -Mathf.Abs(insideUnitCircle.y);
                component.Trigger(insideUnitCircle.ToVector3ZUp(1.5f) * UnityEngine.Random.Range(0.5f, 2f), 0.5f, 0f);
            }
        }
        private static string[] confettiPaths = new string[]
        {
            "Global VFX/Confetti_Blue_001",
            "Global VFX/Confetti_Yellow_001",
            "Global VFX/Confetti_Green_001"
        };
        public Projectile proj;
        public PlayerController player;

    }
    /*
    class GunslingerController : MonoBehaviour
    {
        public int Stack = 0;
        public GunslingerController()
        {
            this.DamageMult = 2.5f;
            this.hasBeenPickedup = false;
        }
        public void Start()
        {
            Stack = 1;
            this.hasBeenPickedup = true; 
            if (player != null)
            {
                player.PostProcessThrownGun += ThrownGunModifier;
                player.GunChanged += OopsSwitch;
            }
            ProjectileInterface = new PlayerItemProjectileInterface();
        }
        public PlayerItemProjectileInterface ProjectileInterface;


        private float TimerToDeath = 0;
        public tk2dSpriteAnimator effectInst;
        public void Update()
        {
            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
                return;
            }

            if (player.CurrentGun != null)
            {
                if (player.CurrentGun.CurrentAmmo > 0 && player.CurrentGun.InfiniteAmmo == false && player.CurrentGun.LocalInfiniteAmmo == false)
                {
                    bool wasPressed = player.m_activeActions.ReloadAction.State;
                    if (wasPressed)
                    {
                        if (effectInst == null)
                        {
                            effectInst = UnityEngine.Object.Instantiate(Gunslinger.GunslingerMagDumpVFX, player.transform).GetComponent<tk2dSpriteAnimator>();
                            effectInst.gameObject.layer = 20;
                            AkSoundEngine.PostEvent("Play_ENM_Grip_Master_Lockon_01", player.gameObject);

                        }

                        TimerToDeath += Time.deltaTime;
                        if (TimerToDeath >= 3)
                        {
                            TimerToDeath = 0;
                            effectInst.PlayAndDestroyObject("chamber_vanish");
                            effectInst = null;
                            SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.GUNSLINGER_FLAG_MAGDUMP, true);

                            int amount = player.CurrentGun.ammo;

                            player.CurrentGun.ammo  = 0;
                            GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(365) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects.First().effects.First().effect, true);
                            vfx.transform.position = player.sprite.WorldCenter;
                            vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                            UnityEngine.Object.Destroy(vfx, 1);
                            AkSoundEngine.PostEvent("Play_ENM_Grip_Master_Eject_01", player.gameObject);
                            cooldown = 0.5f;

                            amount /= 5;
                            amount = Mathf.Max(1, amount);
                            amount = Mathf.Min(20, amount);


                            var proj = this.ProjectileInterface.GetProjectile(player);

                            if (proj.GetComponent<BeamController>() != null)
                            {
                                this.StartCoroutine(this.HandleFireShortBeam(proj, player, player.FacingDirection,(float)amount / 8f, null, null));
                            }
                            else
                            {
                                this.StartCoroutine(DoBurst(amount, proj, player.CurrentGun.Volley.projectiles.Count, player.CurrentGun.DefaultModule.cooldownTime, player.CurrentGun.DefaultModule.angleVariance));
                            }
                        }
                        if (effectInst)
                        {
                            effectInst.transform.position = player.sprite.WorldCenter;
                        }
                    }
                    else
                    {
                        if (TimerToDeath != 0)
                        {
                            TimerToDeath = 0;
                        }
                        if (effectInst)
                        {
                            AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Spit_01", player.gameObject);
                            effectInst.PlayAndDestroyObject("chamber_vanish");
                            effectInst = null;
                        }
                        cooldown = 0.5f;
                    }
                }
            }
            else
            {
                if (TimerToDeath != 0)
                {
                    TimerToDeath = 0;
                }
                if (effectInst)
                {
                    AkSoundEngine.PostEvent("Play_ENM_Grip_Master_Eject_01", player.gameObject);
                    effectInst.PlayAndDestroyObject("chamber_vanish");
                    effectInst = null;
                }
            }
        }
        private float cooldown = 0;

        public IEnumerator DoBurst(int amount, Projectile projectile, int Burst, float RoF, float Spread)
        {
            for (int i = 0; i < amount; i++)
            {
                for (int e = 0; e < Burst; e++)
                {
                    Vector3 vector = player.specRigidbody.UnitCenter;
                    GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, vector, Quaternion.Euler(0f, 0f, player.FacingDirection + UnityEngine.Random.Range(-Spread, Spread)), true);
                    Projectile component = gameObject.GetComponent<Projectile>();
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    player.DoPostProcessProjectile(component);
                    component.baseData.speed *= UnityEngine.Random.Range(0.5f, 1.5f);
                    component.UpdateSpeed();
                }
                yield return new WaitForSeconds(Mathf.Min(0.15f, RoF));
            }
            yield break;
        }


        public void OopsSwitch( Gun gun1, Gun gun2, bool b)
        {
            if (TimerToDeath != 0)
            {
                TimerToDeath = 0;
            }
            if (effectInst)
            {
                AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Spit_01", player.gameObject);
                effectInst.PlayAndDestroyObject("chamber_vanish");
                effectInst = null;
            }
            cooldown = 0.5f;
        }

        private IEnumerator HandleFireShortBeam(Projectile projectileToSpawn, PlayerController source, float targetAngle, float duration, Vector2? overrideSpawnPoint, Vector2? spawnPointOffset = null)
        {
            float elapsed = 0f;
            BeamController beam = this.BeginFiringBeam(projectileToSpawn, source, targetAngle, overrideSpawnPoint, spawnPointOffset);
            yield return null;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                this.ContinueFiringBeam(beam, source, overrideSpawnPoint, spawnPointOffset);
                yield return null;
            }
            beam.CeaseAttack();
            yield break;
        }

        private BeamController BeginFiringBeam(Projectile projectileToSpawn, PlayerController source, float targetAngle, Vector2? overrideSpawnPoint, Vector2? spawnPointOffset = null)
        {
            Vector2 vector = (overrideSpawnPoint == null) ? source.CenterPosition : overrideSpawnPoint.Value;
            vector = ((spawnPointOffset == null) ? vector : (vector + spawnPointOffset.Value));
            GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, vector, Quaternion.identity, true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.Owner = source;
            BeamController component2 = gameObject.GetComponent<BeamController>();
            component2.Owner = source;
            component2.HitsPlayers = false;
            component2.HitsEnemies = true;
            Vector3 v = BraveMathCollege.DegreesToVector(targetAngle, 1f);
            component2.Direction = v;
            component2.Origin = vector;
            return component2;
        }

        private void ContinueFiringBeam(BeamController beam, PlayerController source, Vector2? overrideSpawnPoint, Vector2? spawnPointOffset = null)
        {
            Vector2 vector = (overrideSpawnPoint == null) ? source.CenterPosition : overrideSpawnPoint.Value;
            vector = ((spawnPointOffset == null) ? vector : (vector + spawnPointOffset.Value));
            beam.Origin = vector;
            beam.Direction = MathToolbox.GetUnitOnCircle(source.FacingDirection, 1).normalized;
            beam.LateUpdatePosition(vector);
        }


        //---DONE
        //FULLAUTO
        //BEAM
        //PISTOL
        //POISON
        //FIRE
        //SILLY
        //RIFLE
        //EXPLOSIVE
        //NONE
        //CHARM
        //CHARGE
        //SHOTGUN
        //ICE


        //---NOT DONE
        //SHITTY


        private void ThrownGunModifier(Projectile obj)
        {
            PlayerController playerConb = player;
            HomingModifier homingModifier = obj.gameObject.GetComponent<HomingModifier>();
            obj.baseData.damage *= DamageMult;
            obj.pierceMinorBreakables = true;
            obj.IgnoreTileCollisionsFor(0.01f);
            CoinArbitraryDamageMultiplier cAdM = obj.GetOrAddComponent<CoinArbitraryDamageMultiplier>();
            cAdM.Multiplier = 3;

            obj.OnBecameDebris += HandleReturnLikeBoomerang;
            obj.OnBecameDebris = (Action<DebrisObject>)Delegate.Combine(obj.OnBecameDebris, new Action<DebrisObject>(this.HandleOnHitEffects));
            obj.StartCoroutine(this.ForceTeleportToPlayer(obj));
            Gun gunComp = obj.GetComponentInChildren<Gun>();
            if (gunComp != null)
            {
                switch (gunComp.gunClass)
                {
                    case GunClass.SHITTY:
                        MirrorProjectileModifier mirror = obj.gameObject.GetOrAddComponent<MirrorProjectileModifier>();
                        mirror.MirrorRadius = 1f;
                        mirror.m_projectile = obj;
                        break;

                    case GunClass.SHOTGUN:
                        ShotgunController shotgunCont = obj.gameObject.GetOrAddComponent<ShotgunController>();
                        shotgunCont.player = player;
                        shotgunCont.proj = obj;
                        break;
                    case GunClass.CHARGE:
                        obj.baseData.speed *= 2f;
                        obj.UpdateSpeed();
                        break;
                    case GunClass.ICE:
                        obj.baseData.speed *= 0.7f;
                        obj.UpdateSpeed();
                        AoEDamageComponent AoE = obj.gameObject.GetOrAddComponent<AoEDamageComponent>();
                        AoE.AreaIncreasesWithProjectileSizeStat = true;
                        AoE.DealsDamage = false;
                        AoE.debuffs.Add(DebuffStatics.frostBulletsEffect, 1f);

                        AoE.DealsDamage = false;
                        AoE.Radius = 4;
                        AoE.TimeBetweenDamageEvents = 0.33f;
                        break;
                    case GunClass.CHARM:
                        CharmController charmCont = obj.gameObject.GetOrAddComponent<CharmController>();
                        charmCont.player = player;
                        charmCont.proj = obj;
                        break;
                    case GunClass.FULLAUTO:
                        obj.baseData.speed *= 0.5f;
                        obj.UpdateSpeed();
                        FullAutoController fullAutoCont = obj.gameObject.GetOrAddComponent<FullAutoController>();
                        fullAutoCont.player = player;
                        fullAutoCont.proj = obj;
                        fullAutoCont.self = gunComp;
                        break;
                    case GunClass.BEAM:
                        if (gunComp.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Beam)
                        {
                            Projectile currentProjectile = gunComp.DefaultModule.GetCurrentProjectile();
                            bool flag = currentProjectile.GetComponent<BeamController>() != null;
                            obj.baseData.speed *= 0.5f;
                            obj.UpdateSpeed();
                            bool Flipped = UnityEngine.Random.value > 0.5f ? true : false;
                            if (flag)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    BeamController beamController3 = BeamToolbox.FreeFireBeamFromAnywhere(currentProjectile, player, obj.gameObject, Vector2.zero, false, 120f * i, 6f, true, true, Flipped ? -180 : 180);
                                    Projectile component3 = beamController3.GetComponent<Projectile>();
                                    float Dmg = component3.baseData.damage *= player != null ? player.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
                                    component3.baseData.damage = Dmg / 6;
                                }
                            }                  
                        }
                        break;
                    case GunClass.PISTOL:
                        for (int i = 0; i < 2; i++)
                        {
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, player.CenterPosition.ToVector3ZisY(0f), Quaternion.identity);
                            gameObject.transform.parent = obj.transform;
                            HoveringGunController hoverToDest = gameObject.GetComponent<HoveringGunController>();
                            Destroy(hoverToDest);
                            CustomHoveringGunController hover = gameObject.AddComponent<CustomHoveringGunController>();
                            hover.attachObject = obj.gameObject;
                            hover.ConsumesTargetGunAmmo = false;
                            hover.ChanceToConsumeTargetGunAmmo = 0f;
                            hover.Position = CustomHoveringGunController.HoverPosition.CIRCULATE;
                            hover.Aim = CustomHoveringGunController.AimType.NEAREST_ENEMY;
                            hover.Trigger = CustomHoveringGunController.FireType.ON_COOLDOWN;
                            hover.CooldownTime = 0.66f;
                            hover.ShootDuration = 0.33f;
                            hover.OnlyOnEmptyReload = false;
                            hover.Initialize(gunComp, player);

                        }
                        break;
                    case GunClass.POISON:
                        GoopDoer goopdoer = obj.gameObject.GetOrAddComponent<GoopDoer>();
                        goopdoer.defaultGoopRadius = 0.8f;
                        goopdoer.goopCenter = obj.gameObject;
                        goopdoer.goopDefinition = EasyGoopDefinitions.PoisonDef;
                        goopdoer.goopTime = 0.1f;
                        break;
                    case GunClass.FIRE:
                        GoopDoer goopdoer2 = obj.gameObject.GetOrAddComponent<GoopDoer>();
                        goopdoer2.defaultGoopRadius = 0.8f;
                        goopdoer2.goopCenter = obj.gameObject;
                        goopdoer2.goopDefinition = EasyGoopDefinitions.FireDef;
                        goopdoer2.goopTime = 0.1f;
                        break;
                    case GunClass.SILLY:
                        GlitterCont cont = obj.gameObject.GetOrAddComponent<GlitterCont>();
                        cont.player = player;
                        cont.proj = obj;
                        obj.OverrideMotionModule = new HelixProjectileMotionModule
                        {
                            helixAmplitude = 1f,
                            helixWavelength = 5f,
                            ForceInvert = UnityEngine.Random.value > 0.5f ? true : false
                        };
                        break;
                    case GunClass.RIFLE:
                        GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, player.CenterPosition.ToVector3ZisY(0f), Quaternion.identity);
                        gameObject1.transform.parent = obj.transform;
                        HoveringGunController hoverToDest1 = gameObject1.GetComponent<HoveringGunController>();
                        Destroy(hoverToDest1);
                        CustomHoveringGunController hover1 = gameObject1.AddComponent<CustomHoveringGunController>();
                        hover1.attachObject = obj.gameObject;
                        hover1.ConsumesTargetGunAmmo = false;
                        hover1.ChanceToConsumeTargetGunAmmo = 0f;
                        hover1.Position = CustomHoveringGunController.HoverPosition.OVERHEAD;
                        hover1.Aim = CustomHoveringGunController.AimType.PLAYER_AIM;
                        hover1.Trigger = CustomHoveringGunController.FireType.ON_COOLDOWN;
                        hover1.CooldownTime = obj.GetComponentInChildren<Gun>().DefaultModule.cooldownTime;
                        hover1.ShootDuration = 0.1f;
                        hover1.OnlyOnEmptyReload = false;
                        hover1.Initialize(gunComp, player);
                        break;
                    case GunClass.EXPLOSIVE:
                        ExploCont cont1 = obj.gameObject.GetOrAddComponent<ExploCont>();
                        cont1.player = player;
                        cont1.proj = obj;
                        break;
                    case GunClass.NONE:
                        obj.baseData.speed *= 4;
                        obj.UpdateSpeed();
                        break;
                }
            }
        }
        private IEnumerator ForceTeleportToPlayer(Projectile proj)
        {
            yield return null;
            bool hasTeleportedOnce = false;
            while (hasTeleportedOnce == false)
            {
                try
                {
                    proj.specRigidbody.Position = new Position(player.specRigidbody.UnitCenter);
                    hasTeleportedOnce = true;
                }
                catch (Exception e)
                {
                    ETGModConsole.Log(e.ToString());
                }
            }
            yield break;
        }

        private void HandleOnHitEffects(DebrisObject obj)
        {
           Gun gunComp = obj.GetComponentInChildren<Gun>();
            if (gunComp != null)
            {
                switch (gunComp.gunClass)
                {
                    case GunClass.NONE:
                        GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                        tk2dSpriteAnimator objanimator = silencerVFX.GetComponentInChildren<tk2dSpriteAnimator>();
                        objanimator.ignoreTimeScale = true;
                        objanimator.AlwaysIgnoreTimeScale = true;
                        objanimator.AnimateDuringBossIntros = true;
                        objanimator.alwaysUpdateOffscreen = true;
                        objanimator.playAutomatically = true;
                        List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                        Vector2 centerPosition = gunComp.sprite.WorldCenter;
                        if (activeEnemies != null)
                        {
                            foreach (AIActor aiactor in activeEnemies)
                            {
                                if (Vector2.Distance(aiactor.CenterPosition, centerPosition) < 4 && aiactor != null && aiactor.specRigidbody != null && player != null && !aiactor.healthHaver.IsBoss)
                                {
                                    aiactor.behaviorSpeculator.Stun(4);  
                                }
                            }
                        }
                        break;
                    case GunClass.CHARM:
                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.CharmGoopDef).TimedAddGoopCircle(gunComp.transform.PositionVector2(), 2.5f, 0.33f, false);
                        break;
                    case GunClass.FULLAUTO:
                        break;
                    case GunClass.BEAM:
                        break;
                    case GunClass.CHARGE:
                        ChargeGunController chargeCont = obj.gameObject.GetOrAddComponent<ChargeGunController>();
                        chargeCont.self = obj;
                        break;
                    case GunClass.RIFLE:
                        break;
                    case GunClass.PISTOL:
                        break;
                    case GunClass.FIRE:
                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.FireDef).TimedAddGoopCircle(gunComp.transform.PositionVector2(), 3.5f, 0.33f, false);
                        break;
                    case GunClass.POISON:
                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.PoisonDef).TimedAddGoopCircle(gunComp.transform.PositionVector2(), 3.5f, 0.33f, false);
                        break;
                    case GunClass.SHOTGUN:
                        break;
                    case GunClass.SILLY:
                        break;
                    case GunClass.ICE:
                        break;
                    case GunClass.SHITTY:
                        break;
                    case GunClass.EXPLOSIVE:
                        ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericLargeExplosion;
                        boomboom.damageToPlayer = 0;
                        boomboom.preventPlayerForce = true;
                        boomboom.ignoreList.Add(player.specRigidbody);
                        Exploder.Explode(gunComp.sprite.WorldCenter, boomboom, gunComp.transform.PositionVector2());
                        break;
                }
            }   
        }

        private void HandleReturnLikeBoomerang(DebrisObject obj)
        {
            obj.sprite.IsPerpendicular = true;
            obj.sprite.automaticallyManagesDepth = true;
            obj.sprite.ignoresTiltworldDepth = true;
            obj.FlagAsPickup();
            obj.canRotate = true;
            obj.motionMultiplier = 0;
            obj.OnGrounded = (Action<DebrisObject>)Delegate.Remove(obj.OnGrounded, new Action<DebrisObject>(this.HandleReturnLikeBoomerang));
            PickupMover pickupMover = obj.gameObject.AddComponent<PickupMover>();
            if (pickupMover.specRigidbody)
            {
                pickupMover.specRigidbody.CollideWithTileMap = false;
            }
            Gun gunComp = obj.GetComponentInChildren<Gun>();
            if (gunComp != null)
            {
                switch (gunComp.gunClass)
                {
                    case GunClass.NONE:
                        pickupMover.acceleration = 40f;
                        pickupMover.maxSpeed = 15f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.CHARM:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 6f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.FULLAUTO:
                        pickupMover.acceleration = 8f;
                        pickupMover.maxSpeed = 1f;
                        pickupMover.minRadius = 0.75f;
                        pickupMover.moveIfRoomUnclear = false;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.BEAM:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 2.5f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.CHARGE:
                        pickupMover.acceleration = 8f;
                        pickupMover.maxSpeed = 5f;
                        pickupMover.minRadius = 0.75f;
                        pickupMover.moveIfRoomUnclear = false;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.RIFLE:
                        pickupMover.acceleration = 12f;
                        pickupMover.maxSpeed = 3f;
                        pickupMover.minRadius = 0.75f;
                        pickupMover.moveIfRoomUnclear = false;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.PISTOL:
                        pickupMover.acceleration = 12f;
                        pickupMover.maxSpeed = 3f;
                        pickupMover.minRadius = 0.75f;
                        pickupMover.moveIfRoomUnclear = false;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.FIRE:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 4.5f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.POISON:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 4.5f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.SHOTGUN:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 6f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.SILLY:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 6f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.ICE:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 6f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.SHITTY:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 6f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.EXPLOSIVE:
                        pickupMover.acceleration = 25f;
                        pickupMover.maxSpeed = 10f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = false;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    default:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 6f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                }                    
            }
            else
            {
                pickupMover.acceleration = 10f;
                pickupMover.maxSpeed = 6f;
                pickupMover.minRadius = 1f;
                pickupMover.moveIfRoomUnclear = true;
                pickupMover.stopPathingOnContact = true;
            }

           
        }

        public void IncrementStack()
        {
            Stack++;
            DamageMult += 1.2f;
        }
        public float DamageMult;
        public PlayerController player;
        public bool hasBeenPickedup;
    }
    */
    class Gunslinger : PerkPickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Gunslinger";
            //string resourcePath = "Planetside/Resources/PerkThings/somethingtoDoWithThrownGuns.png";
            GameObject gameObject = new GameObject(name);
            Gunslinger item = gameObject.AddComponent<Gunslinger>();

            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("somethingtoDoWithThrownGuns"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Advanced Combat";
            string longDesc = "Despite Gun-Throwing as a combat technique being fully abandoned out of redundancy, some have harnessed its untapped potential, conquering the Gungeons depths without ever firing a single shot with their own hands.";
            item.SetupItem(shortDesc, longDesc, "psog");
            Gunslinger.GunslingerID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            item.encounterTrackable.DoNotificationOnEncounter = false;

            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.green;
            particles.ParticleSystemColor2 = Color.white;
            item.OutlineColor = new Color(0f, 1f, 0f);

            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("GunslingerMagDump", debuffCollection.GetSpriteIdByName("ChamberDoEffect_001"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.VFX_Animation_Data;
            animator.Library = StaticSpriteDefinitions.VFX_Animation_Data;

            animator.sprite.usesOverrideMaterial = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = animator.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(100, 255, 154, 255));
            mat.SetFloat("_EmissiveColorPower", 2f);
            mat.SetFloat("_EmissivePower", 20);
            animator.sprite.renderer.material = mat;

            animator.DefaultClipId = animator.GetClipIdByName("chamber_docooltrick");
            animator.playAutomatically = true;
            GunslingerMagDumpVFX = BrokenArmorVFXObject;


            item.InitialPickupNotificationText = "Massive Boost To Thrown Weapons.";
            item.StackPickupNotificationText = "Thrown Weapons Are Even Stronger.";
        }

        public static GameObject GunslingerMagDumpVFX;

        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 0,
                    LockedString = "\"Massive Boost To Thrown Weapons.\"",
                    UnlockedString = "\"Massive Boost To Thrown Weapons.\"",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 1,
                    LockedString = AlphabetController.ConvertString("THROWN GUNS GOOD"),
                    UnlockedString = "Massively Increases the power of thrown guns.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 2,
                    LockedString = AlphabetController.ConvertString("MANY EFFECTS"),
                    UnlockedString = "Thrown Guns can have one of 14 different unique effects, based on their Gun Class.",
                    requiresFlag = false
                },
                 new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 3,
                    LockedString = AlphabetController.ConvertString("THROW THEM PLEASE"),
                    UnlockedString = "Triples thrown gun damage.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 4,
                    LockedString = AlphabetController.ConvertString("AMMO DUMP"),
                    UnlockedString = "Holding reload while out of combat allows you to dump all of your current guns ammo.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.GUNSLINGER_FLAG_MAGDUMP
                },

                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 5,
                    LockedString = AlphabetController.ConvertString("Stack Increases Yeets"),
                    UnlockedString = "Stacking increases thrown gun damage even more.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.GUNSLINGER_FLAG_STACK
                },
        };
        public override CustomDungeonFlags FlagToSetOnStack => CustomDungeonFlags.GUNSLINGER_FLAG_STACK;


        public static int GunslingerID;

        
        /*
        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            base.HandleEncounterable(player);

            SaveAPI.AdvancedGameStatsManager.Instance.RegisterStatChange(StatToIncreaseOnPickup, 1);

            m_hasBeenPickedUp = true;
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);


            GunslingerController slinger = player.gameObject.GetOrAddComponent<GunslingerController>();
            slinger.player = player;
            if (slinger.hasBeenPickedup == true)
            { slinger.IncrementStack(); SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.GUNSLINGER_FLAG_STACK, true); }
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = slinger.hasBeenPickedup == true ? "Thrown Weapons Are Even Stronger." : "Massive Boost To Thrown Weapons.";
            OtherTools.NotifyCustom("Gunslinger", BlurbText, "somethingtoDoWithThrownGuns", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);

            UnityEngine.Object.Destroy(base.gameObject);
        }
        */


        private void HandleReturnLikeBoomerang(DebrisObject obj)
        {
            obj.sprite.IsPerpendicular = true;
            obj.sprite.automaticallyManagesDepth = true;
            obj.sprite.ignoresTiltworldDepth = true;
            obj.FlagAsPickup();
            obj.canRotate = true;
            obj.motionMultiplier = 0;
            obj.OnGrounded = (Action<DebrisObject>)Delegate.Remove(obj.OnGrounded, new Action<DebrisObject>(this.HandleReturnLikeBoomerang));
            PickupMover pickupMover = obj.gameObject.AddComponent<PickupMover>();
            if (pickupMover.specRigidbody)
            {
                pickupMover.specRigidbody.CollideWithTileMap = false;
            }
            Gun gunComp = obj.GetComponentInChildren<Gun>();
            if (gunComp != null)
            {
                switch (gunComp.gunClass)
                {
                    case GunClass.NONE:
                        pickupMover.acceleration = 40f;
                        pickupMover.maxSpeed = 15f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.CHARM:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 6f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.FULLAUTO:
                        pickupMover.acceleration = 8f;
                        pickupMover.maxSpeed = 1f;
                        pickupMover.minRadius = 0.75f;
                        pickupMover.moveIfRoomUnclear = false;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.BEAM:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 2.5f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.CHARGE:
                        pickupMover.acceleration = 8f;
                        pickupMover.maxSpeed = 5f;
                        pickupMover.minRadius = 0.75f;
                        pickupMover.moveIfRoomUnclear = false;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.RIFLE:
                        pickupMover.acceleration = 12f;
                        pickupMover.maxSpeed = 3f;
                        pickupMover.minRadius = 0.75f;
                        pickupMover.moveIfRoomUnclear = false;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.PISTOL:
                        pickupMover.acceleration = 12f;
                        pickupMover.maxSpeed = 3f;
                        pickupMover.minRadius = 0.75f;
                        pickupMover.moveIfRoomUnclear = false;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.FIRE:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 4.5f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.POISON:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 4.5f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.SHOTGUN:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 6f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.SILLY:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 6f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.ICE:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 6f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.SHITTY:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 6f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    case GunClass.EXPLOSIVE:
                        pickupMover.acceleration = 25f;
                        pickupMover.maxSpeed = 10f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = false;
                        pickupMover.stopPathingOnContact = true;
                        break;
                    default:
                        pickupMover.acceleration = 10f;
                        pickupMover.maxSpeed = 6f;
                        pickupMover.minRadius = 1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = true;
                        break;
                }
            }
            else
            {
                pickupMover.acceleration = 10f;
                pickupMover.maxSpeed = 6f;
                pickupMover.minRadius = 1f;
                pickupMover.moveIfRoomUnclear = true;
                pickupMover.stopPathingOnContact = true;
            }


        }

        private float TimerToDeath = 0;
        public tk2dSpriteAnimator effectInst;
        public override void Update()
        {
            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
                return;
            }

            if (_Owner.CurrentGun != null)
            {
                if (_Owner.CurrentGun.CurrentAmmo > 0 && _Owner.CurrentGun.InfiniteAmmo == false && _Owner.CurrentGun.LocalInfiniteAmmo == false)
                {
                    bool wasPressed = _Owner.m_activeActions.ReloadAction.State;
                    if (wasPressed)
                    {
                        if (effectInst == null)
                        {
                            effectInst = UnityEngine.Object.Instantiate(Gunslinger.GunslingerMagDumpVFX, _Owner.transform).GetComponent<tk2dSpriteAnimator>();
                            effectInst.gameObject.layer = 20;
                            AkSoundEngine.PostEvent("Play_ENM_Grip_Master_Lockon_01", _Owner.gameObject);

                        }

                        TimerToDeath += Time.deltaTime;
                        if (TimerToDeath >= 3)
                        {
                            TimerToDeath = 0;
                            effectInst.PlayAndDestroyObject("chamber_vanish");
                            effectInst = null;
                            SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.GUNSLINGER_FLAG_MAGDUMP, true);

                            int amount = _Owner.CurrentGun.ammo;

                            _Owner.CurrentGun.ammo = 0;
                            GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(365) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects.First().effects.First().effect, true);
                            vfx.transform.position = _Owner.sprite.WorldCenter;
                            vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                            UnityEngine.Object.Destroy(vfx, 1);
                            AkSoundEngine.PostEvent("Play_ENM_Grip_Master_Eject_01", _Owner.gameObject);
                            cooldown = 0.5f;

                            amount /= 5;
                            amount = Mathf.Max(1, amount);
                            amount = Mathf.Min(20, amount);


                            var proj = this.ProjectileInterface.GetProjectile(_Owner);

                            if (proj.GetComponent<BeamController>() != null)
                            {
                                this.StartCoroutine(this.HandleFireShortBeam(proj, _Owner, _Owner.FacingDirection, (float)amount / 8f, null, null));
                            }
                            else
                            {
                                this.StartCoroutine(DoBurst(amount, proj, _Owner.CurrentGun.Volley.projectiles.Count, _Owner.CurrentGun.DefaultModule.cooldownTime, _Owner.CurrentGun.DefaultModule.angleVariance));
                            }
                        }
                        if (effectInst)
                        {
                            effectInst.transform.position = _Owner.sprite.WorldCenter;
                        }
                    }
                    else
                    {
                        if (TimerToDeath != 0)
                        {
                            TimerToDeath = 0;
                        }
                        if (effectInst)
                        {
                            AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Spit_01", _Owner.gameObject);
                            effectInst.PlayAndDestroyObject("chamber_vanish");
                            effectInst = null;
                        }
                        cooldown = 0.5f;
                    }
                }
            }
            else
            {
                if (TimerToDeath != 0)
                {
                    TimerToDeath = 0;
                }
                if (effectInst)
                {
                    AkSoundEngine.PostEvent("Play_ENM_Grip_Master_Eject_01", _Owner.gameObject);
                    effectInst.PlayAndDestroyObject("chamber_vanish");
                    effectInst = null;
                }
            }
        }
        private float cooldown = 0;

        public IEnumerator DoBurst(int amount, Projectile projectile, int Burst, float RoF, float Spread)
        {
            for (int i = 0; i < amount; i++)
            {
                for (int e = 0; e < Burst; e++)
                {
                    Vector3 vector = _Owner.specRigidbody.UnitCenter;
                    GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, vector, Quaternion.Euler(0f, 0f, _Owner.FacingDirection + UnityEngine.Random.Range(-Spread, Spread)), true);
                    Projectile component = gameObject.GetComponent<Projectile>();
                    component.Owner = _Owner;
                    component.Shooter = _Owner.specRigidbody;
                    _Owner.DoPostProcessProjectile(component);
                    component.baseData.speed *= UnityEngine.Random.Range(0.5f, 1.5f);
                    component.UpdateSpeed();
                }
                yield return new WaitForSeconds(Mathf.Min(0.15f, RoF));
            }
            yield break;
        }


        public void OopsSwitch(Gun gun1, Gun gun2, bool b)
        {
            if (TimerToDeath != 0)
            {
                TimerToDeath = 0;
            }
            if (effectInst)
            {
                AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Spit_01", _Owner.gameObject);
                effectInst.PlayAndDestroyObject("chamber_vanish");
                effectInst = null;
            }
            cooldown = 0.5f;
        }

        private IEnumerator HandleFireShortBeam(Projectile projectileToSpawn, PlayerController source, float targetAngle, float duration, Vector2? overrideSpawnPoint, Vector2? spawnPointOffset = null)
        {
            float elapsed = 0f;
            BeamController beam = this.BeginFiringBeam(projectileToSpawn, source, targetAngle, overrideSpawnPoint, spawnPointOffset);
            yield return null;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                this.ContinueFiringBeam(beam, source, overrideSpawnPoint, spawnPointOffset);
                yield return null;
            }
            beam.CeaseAttack();
            yield break;
        }

        private BeamController BeginFiringBeam(Projectile projectileToSpawn, PlayerController source, float targetAngle, Vector2? overrideSpawnPoint, Vector2? spawnPointOffset = null)
        {
            Vector2 vector = (overrideSpawnPoint == null) ? source.CenterPosition : overrideSpawnPoint.Value;
            vector = ((spawnPointOffset == null) ? vector : (vector + spawnPointOffset.Value));
            GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, vector, Quaternion.identity, true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.Owner = source;
            BeamController component2 = gameObject.GetComponent<BeamController>();
            component2.Owner = source;
            component2.HitsPlayers = false;
            component2.HitsEnemies = true;
            Vector3 v = BraveMathCollege.DegreesToVector(targetAngle, 1f);
            component2.Direction = v;
            component2.Origin = vector;
            return component2;
        }

        private void ContinueFiringBeam(BeamController beam, PlayerController source, Vector2? overrideSpawnPoint, Vector2? spawnPointOffset = null)
        {
            Vector2 vector = (overrideSpawnPoint == null) ? source.CenterPosition : overrideSpawnPoint.Value;
            vector = ((spawnPointOffset == null) ? vector : (vector + spawnPointOffset.Value));
            beam.Origin = vector;
            beam.Direction = MathToolbox.GetUnitOnCircle(source.FacingDirection, 1).normalized;
            beam.LateUpdatePosition(vector);
        }


        //---DONE
        //FULLAUTO
        //BEAM
        //PISTOL
        //POISON
        //FIRE
        //SILLY
        //RIFLE
        //EXPLOSIVE
        //NONE
        //CHARM
        //CHARGE
        //SHOTGUN
        //ICE


        //---NOT DONE
        //SHITTY


        private void ThrownGunModifier(Projectile obj)
        {
            PlayerController playerConb = _Owner;
            HomingModifier homingModifier = obj.gameObject.GetComponent<HomingModifier>();
            obj.baseData.damage *= DamageMult;
            obj.pierceMinorBreakables = true;
            obj.IgnoreTileCollisionsFor(0.01f);
            CoinArbitraryDamageMultiplier cAdM = obj.GetOrAddComponent<CoinArbitraryDamageMultiplier>();
            cAdM.Multiplier = 3;

            obj.OnBecameDebris += HandleReturnLikeBoomerang;
            obj.OnBecameDebris = (Action<DebrisObject>)Delegate.Combine(obj.OnBecameDebris, new Action<DebrisObject>(this.HandleOnHitEffects));
            obj.StartCoroutine(this.ForceTeleportToPlayer(obj));
            Gun gunComp = obj.GetComponentInChildren<Gun>();
            if (gunComp != null)
            {
                switch (gunComp.gunClass)
                {
                    case GunClass.SHITTY:
                        MirrorProjectileModifier mirror = obj.gameObject.GetOrAddComponent<MirrorProjectileModifier>();
                        mirror.MirrorRadius = 1f;
                        mirror.m_projectile = obj;
                        break;

                    case GunClass.SHOTGUN:
                        ShotgunController shotgunCont = obj.gameObject.GetOrAddComponent<ShotgunController>();
                        shotgunCont.player = _Owner;
                        shotgunCont.proj = obj;
                        break;
                    case GunClass.CHARGE:
                        obj.baseData.speed *= 2f;
                        obj.UpdateSpeed();
                        break;
                    case GunClass.ICE:
                        obj.baseData.speed *= 0.7f;
                        obj.UpdateSpeed();
                        AoEDamageComponent AoE = obj.gameObject.GetOrAddComponent<AoEDamageComponent>();
                        AoE.AreaIncreasesWithProjectileSizeStat = true;
                        AoE.DealsDamage = false;
                        AoE.debuffs.Add(DebuffStatics.frostBulletsEffect, 1f);

                        AoE.DealsDamage = false;
                        AoE.Radius = 4;
                        AoE.TimeBetweenDamageEvents = 0.33f;
                        break;
                    case GunClass.CHARM:
                        CharmController charmCont = obj.gameObject.GetOrAddComponent<CharmController>();
                        charmCont.player = _Owner;
                        charmCont.proj = obj;
                        break;
                    case GunClass.FULLAUTO:
                        obj.baseData.speed *= 0.5f;
                        obj.UpdateSpeed();
                        FullAutoController fullAutoCont = obj.gameObject.GetOrAddComponent<FullAutoController>();
                        fullAutoCont.player = _Owner;
                        fullAutoCont.proj = obj;
                        fullAutoCont.self = gunComp;
                        break;
                    case GunClass.BEAM:
                        if (gunComp.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Beam)
                        {
                            Projectile currentProjectile = gunComp.DefaultModule.GetCurrentProjectile();
                            bool flag = currentProjectile.GetComponent<BeamController>() != null;
                            obj.baseData.speed *= 0.5f;
                            obj.UpdateSpeed();
                            bool Flipped = UnityEngine.Random.value > 0.5f ? true : false;
                            if (flag)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    BeamController beamController3 = BeamToolbox.FreeFireBeamFromAnywhere(currentProjectile, _Owner, obj.gameObject, Vector2.zero, false, 120f * i, 6f, true, true, Flipped ? -180 : 180);
                                    Projectile component3 = beamController3.GetComponent<Projectile>();
                                    float Dmg = component3.baseData.damage *= _Owner != null ? _Owner.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
                                    component3.baseData.damage = Dmg / 6;
                                }
                            }
                        }
                        break;
                    case GunClass.PISTOL:
                        for (int i = 0; i < 2; i++)
                        {
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, _Owner.CenterPosition.ToVector3ZisY(0f), Quaternion.identity);
                            gameObject.transform.parent = obj.transform;
                            HoveringGunController hoverToDest = gameObject.GetComponent<HoveringGunController>();
                            Destroy(hoverToDest);
                            CustomHoveringGunController hover = gameObject.AddComponent<CustomHoveringGunController>();
                            hover.attachObject = obj.gameObject;
                            hover.ConsumesTargetGunAmmo = false;
                            hover.ChanceToConsumeTargetGunAmmo = 0f;
                            hover.Position = CustomHoveringGunController.HoverPosition.CIRCULATE;
                            hover.Aim = CustomHoveringGunController.AimType.NEAREST_ENEMY;
                            hover.Trigger = CustomHoveringGunController.FireType.ON_COOLDOWN;
                            hover.CooldownTime = 0.66f;
                            hover.ShootDuration = 0.33f;
                            hover.OnlyOnEmptyReload = false;
                            hover.Initialize(gunComp, _Owner);

                        }
                        break;
                    case GunClass.POISON:
                        GoopDoer goopdoer = obj.gameObject.GetOrAddComponent<GoopDoer>();
                        goopdoer.defaultGoopRadius = 0.8f;
                        goopdoer.goopCenter = obj.gameObject;
                        goopdoer.goopDefinition = EasyGoopDefinitions.PoisonDef;
                        goopdoer.goopTime = 0.1f;
                        break;
                    case GunClass.FIRE:
                        GoopDoer goopdoer2 = obj.gameObject.GetOrAddComponent<GoopDoer>();
                        goopdoer2.defaultGoopRadius = 0.8f;
                        goopdoer2.goopCenter = obj.gameObject;
                        goopdoer2.goopDefinition = EasyGoopDefinitions.FireDef;
                        goopdoer2.goopTime = 0.1f;
                        break;
                    case GunClass.SILLY:
                        GlitterCont cont = obj.gameObject.GetOrAddComponent<GlitterCont>();
                        cont.player = _Owner;
                        cont.proj = obj;
                        obj.OverrideMotionModule = new HelixProjectileMotionModule
                        {
                            helixAmplitude = 1f,
                            helixWavelength = 5f,
                            ForceInvert = UnityEngine.Random.value > 0.5f ? true : false
                        };
                        break;
                    case GunClass.RIFLE:
                        GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, _Owner.CenterPosition.ToVector3ZisY(0f), Quaternion.identity);
                        gameObject1.transform.parent = obj.transform;
                        HoveringGunController hoverToDest1 = gameObject1.GetComponent<HoveringGunController>();
                        Destroy(hoverToDest1);
                        CustomHoveringGunController hover1 = gameObject1.AddComponent<CustomHoveringGunController>();
                        hover1.attachObject = obj.gameObject;
                        hover1.ConsumesTargetGunAmmo = false;
                        hover1.ChanceToConsumeTargetGunAmmo = 0f;
                        hover1.Position = CustomHoveringGunController.HoverPosition.OVERHEAD;
                        hover1.Aim = CustomHoveringGunController.AimType.PLAYER_AIM;
                        hover1.Trigger = CustomHoveringGunController.FireType.ON_COOLDOWN;
                        hover1.CooldownTime = obj.GetComponentInChildren<Gun>().DefaultModule.cooldownTime;
                        hover1.ShootDuration = 0.1f;
                        hover1.OnlyOnEmptyReload = false;
                        hover1.Initialize(gunComp, _Owner);
                        break;
                    case GunClass.EXPLOSIVE:
                        ExploCont cont1 = obj.gameObject.GetOrAddComponent<ExploCont>();
                        cont1.player = _Owner;
                        cont1.proj = obj;
                        break;
                    case GunClass.NONE:
                        obj.baseData.speed *= 4;
                        obj.UpdateSpeed();
                        break;
                }
            }
        }
        private IEnumerator ForceTeleportToPlayer(Projectile proj)
        {
            yield return null;
            bool hasTeleportedOnce = false;
            while (hasTeleportedOnce == false)
            {
                try
                {
                    proj.specRigidbody.Position = new Position(_Owner.specRigidbody.UnitCenter);
                    hasTeleportedOnce = true;
                }
                catch (Exception e)
                {
                    ETGModConsole.Log(e.ToString());
                }
            }
            yield break;
        }

        private void HandleOnHitEffects(DebrisObject obj)
        {
            Gun gunComp = obj.GetComponentInChildren<Gun>();
            if (gunComp != null)
            {
                switch (gunComp.gunClass)
                {
                    case GunClass.NONE:
                        GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                        tk2dSpriteAnimator objanimator = silencerVFX.GetComponentInChildren<tk2dSpriteAnimator>();
                        objanimator.ignoreTimeScale = true;
                        objanimator.AlwaysIgnoreTimeScale = true;
                        objanimator.AnimateDuringBossIntros = true;
                        objanimator.alwaysUpdateOffscreen = true;
                        objanimator.playAutomatically = true;
                        List<AIActor> activeEnemies = _Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                        Vector2 centerPosition = gunComp.sprite.WorldCenter;
                        if (activeEnemies != null)
                        {
                            foreach (AIActor aiactor in activeEnemies)
                            {
                                if (Vector2.Distance(aiactor.CenterPosition, centerPosition) < 4 && aiactor != null && aiactor.specRigidbody != null && _Owner != null && !aiactor.healthHaver.IsBoss)
                                {
                                    aiactor.behaviorSpeculator.Stun(4);
                                }
                            }
                        }
                        break;
                    case GunClass.CHARM:
                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.CharmGoopDef).TimedAddGoopCircle(gunComp.transform.PositionVector2(), 2.5f, 0.33f, false);
                        break;
                    case GunClass.FULLAUTO:
                        break;
                    case GunClass.BEAM:
                        break;
                    case GunClass.CHARGE:
                        ChargeGunController chargeCont = obj.gameObject.GetOrAddComponent<ChargeGunController>();
                        chargeCont.self = obj;
                        break;
                    case GunClass.RIFLE:
                        break;
                    case GunClass.PISTOL:
                        break;
                    case GunClass.FIRE:
                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.FireDef).TimedAddGoopCircle(gunComp.transform.PositionVector2(), 3.5f, 0.33f, false);
                        break;
                    case GunClass.POISON:
                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.PoisonDef).TimedAddGoopCircle(gunComp.transform.PositionVector2(), 3.5f, 0.33f, false);
                        break;
                    case GunClass.SHOTGUN:
                        break;
                    case GunClass.SILLY:
                        break;
                    case GunClass.ICE:
                        break;
                    case GunClass.SHITTY:
                        break;
                    case GunClass.EXPLOSIVE:
                        ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericLargeExplosion;
                        boomboom.damageToPlayer = 0;
                        boomboom.preventPlayerForce = true;
                        boomboom.ignoreList.Add(_Owner.specRigidbody);
                        Exploder.Explode(gunComp.sprite.WorldCenter, boomboom, gunComp.transform.PositionVector2());
                        break;
                }
            }
        }

        public override void OnInitialPickup(PlayerController player)
        {
            player.PostProcessThrownGun += ThrownGunModifier;
            player.GunChanged += OopsSwitch;
            ProjectileInterface = new PlayerItemProjectileInterface();
        }
        public PlayerItemProjectileInterface ProjectileInterface;

        public override void OnStack(PlayerController playerController)
        {
            DamageMult += 1f;

        }




        public float DamageMult = 2.5f;
    }
}
