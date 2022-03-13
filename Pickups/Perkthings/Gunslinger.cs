using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using MonoMod.RuntimeDetour;
using System.Reflection;

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
                    hover1.DamageMultiplier = 0.8f;
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
                            if (aiactor != null && Vector2.Distance(aiactor.CenterPosition, self.transform.position) <  6)
                            {
                                if (!aiactor.healthHaver.IsBoss)
                                {
                                    Vector2 a = aiactor.transform.position - self.transform.position;
                                    aiactor.knockbackDoer.ApplyKnockback(-a, 20f * (Vector2.Distance(self.transform.position, aiactor.transform.position) + 0.005f), false);
                                }
                            }
                        }
                        GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(self.transform.position, 4, 0.1f, 11f, 0.5f));
                    }
                    Timer = 0;
                }
            }            
        }

        private float Timer;

        public DebrisObject self;
        public PlayerController player;
    }
    class  CharmController : MonoBehaviour
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
                if (Timer >= self.DefaultModule.cooldownTime + 0.03f)
                {
                    Timer = 0;
                    ProjectileModule defaultModule = self.DefaultModule;
                    Projectile currentProjectile = defaultModule.GetCurrentProjectile();
                    if (currentProjectile)
                    {
                        currentProjectile.baseData.damage *= 0.3f;
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
            source.DoPostProcessProjectile(component);
            ForceSpecificMotionModule mod = component.gameObject.AddComponent<ForceSpecificMotionModule>();
            mod.Module = new OrbitProjectileMotionModule()
            {
                usesAlternateOrbitTarget = true,
                alternateOrbitTarget = base.gameObject.GetComponent<SpeculativeRigidbody>(),
                ForceInvert = UnityEngine.Random.value > 0.5f ? true : false,
                lifespan = 10,             
            };
        }
        private float Timer;
        public Projectile proj;
        public PlayerController player;
        public Gun self;

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
            if (Timer >= 0.4f)
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
    class GunslingerController : MonoBehaviour
    {
        public GunslingerController()
        {
            this.DamageMult = 3.5f;
            this.hasBeenPickedup = false;
        }
        public void Start()
        {
            this.hasBeenPickedup = true; 
            if (player != null)
            {
                OtherTools.ApplyStat(player, PlayerStats.StatType.AmmoCapacityMultiplier, 0.4f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                player.PostProcessThrownGun += ThrownGunModifier;
            }
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
                        AoE.EffectProcChance = 1;
                        AoE.InflictsFreeze = true;
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
                                    component3.baseData.damage = Dmg / 7;
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
                        hover1.CooldownTime = 0.4f;
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
                        ExplosionData boomboom = StaticExplosionDatas.genericLargeExplosion;
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
                        pickupMover.moveIfRoomUnclear = true;
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
            DamageMult += 1.5f;
        }
        public float DamageMult;
        public PlayerController player;
        public bool hasBeenPickedup;
    }
    class Gunslinger : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Gunslinger";
            string resourcePath = "Planetside/Resources/PerkThings/somethingtoDoWithThrownGuns.png";
            GameObject gameObject = new GameObject(name);
            Gunslinger item = gameObject.AddComponent<Gunslinger>();
          
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Literally just an all stats up.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            Gunslinger.GunslingerID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.green;
            particles.ParticleSystemColor2 = Color.white;
            OutlineColor = new Color(0f, 1f, 0f);
            //new Hook(typeof(HoveringGunController).GetMethod("UpdatePosition", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Gunslinger).GetMethod("DisableFuses"));
        }
        public static int GunslingerID;
        private static Color OutlineColor;

        public new bool PrerequisitesMet()
        {
            EncounterTrackable component = base.GetComponent<EncounterTrackable>();
            return component == null || component.PrerequisitesMet();
        }
        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            m_hasBeenPickedUp = true;
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);


            GunslingerController slinger = player.gameObject.GetOrAddComponent<GunslingerController>();
            slinger.player = player;
            if (slinger.hasBeenPickedup == true)
            { slinger.IncrementStack(); }
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = slinger.hasBeenPickedup == true ? "Thrown Weapons Are Even Stronger." : "Massive Boost To Thrown Weapons.";
            OtherTools.Notify("Gunslinger", BlurbText, "Planetside/Resources/PerkThings/somethingtoDoWithThrownGuns", UINotificationController.NotificationColor.GOLD);

            UnityEngine.Object.Destroy(base.gameObject);
        }

        public float distortionMaxRadius = 30f;
        public float distortionDuration = 2f;
        public float distortionIntensity = 0.7f;
        public float distortionThickness = 0.1f;
        protected void Start()
        {
            try
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(this);
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            }
            catch (Exception er)
            {
                ETGModConsole.Log(er.Message, false);
            }
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite)
            {
                return 1000f;
            }
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return 1f;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (!interactor.CurrentRoom.IsRegistered(this) && !RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        private void Update()
        {
            if (!this.m_hasBeenPickedUp && !this.m_isBeingEyedByRat && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
            {
                GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
            }
        }

        public void Interact(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                RoomHandler.unassignedInteractableObjects.Remove(this);
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            this.Pickup(interactor);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        private bool m_hasBeenPickedUp;
    }
}
