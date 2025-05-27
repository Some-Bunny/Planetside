using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;
using Alexandria.PrefabAPI;
using Brave.BulletScript;

namespace Planetside
{
    public static class StaticVFXStorage
    {
        public static void Init()
        {
            RadialRing = (GameObject)ResourceCache.Acquire("Global VFX/HeatIndicator");
            TeleportDistortVFX = (PickupObjectDatabase.GetById(573) as ChestTeleporterItem).TeleportVFX;
            TeleportVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Teleport_Beam");

            var scarf = PickupObjectDatabase.GetById(436) as BlinkPassiveItem;
            ScarfObject = scarf.ScarfPrefab;
            BloodiedScarfPoofVFX = scarf.BlinkpoofVfx;

            var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("TheperkParticle"));//this is the name of the object which by default will be "Particle System"
            PerkParticleObject = partObj;
            FakePrefab.MarkAsFakePrefab(partObj);
            PerkParticleSystem = partObj.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(PerkParticleSystem.gameObject);
            var main = PerkParticleSystem.main;
            main.stopAction = ParticleSystemStopAction.None;
            main.maxParticles = 2000;
            main.duration = 1200;
            JammedDeathVFX = (GameObject)BraveResources.Load("Global VFX/VFX_BlackPhantomDeath", ".prefab");

            var PerfpartObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PerfectedParticles"));
            FakePrefab.MarkAsFakePrefab(PerfpartObj);
            PerfectedParticleSystem = PerfpartObj.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(PerfectedParticleSystem.gameObject);


            var mainperf = PerkParticleSystem.main;
            mainperf.stopAction = ParticleSystemStopAction.None;
            mainperf.maxParticles = 2000;
            mainperf.duration = 1200;


            var ceramicObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("CeramicParticles"));
            FakePrefab.MarkAsFakePrefab(ceramicObj);
            CeramicParticleSystem = ceramicObj.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(CeramicParticleSystem.gameObject);

            var bloodSplatterObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("BloodSplatter"));
            FakePrefab.MarkAsFakePrefab(bloodSplatterObj);
            BloodSplatterParticleSystem = bloodSplatterObj.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(BloodSplatterParticleSystem.gameObject);

            var shamberParticlesObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("ShamberParticles"));
            ShamberParticleSystemGameObject = shamberParticlesObj;
            FakePrefab.MarkAsFakePrefab(shamberParticlesObj);
            ShamberParticleSystem = shamberParticlesObj.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(ShamberParticleSystem.gameObject);

            var EliteVariantEnemyParticlesObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("EliteVariantEnemy"));
            EliteParticleSystemGameObject = EliteVariantEnemyParticlesObj;
            EliteParticleSystemGameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Reflection"));


            FakePrefab.MarkAsFakePrefab(EliteVariantEnemyParticlesObj);
            EliteParticleSystem = EliteVariantEnemyParticlesObj.GetComponent<ParticleSystem>();

            FakePrefab.MarkAsFakePrefab(EliteParticleSystem.gameObject);


            var portalParticleObject = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("PortalClose"));
            PortalParticleSystemGameObject = portalParticleObject;
            FakePrefab.MarkAsFakePrefab(portalParticleObject);
            PortalParticleSystem = portalParticleObject.GetComponent<ParticleSystem>();
            FakePrefab.MarkAsFakePrefab(PortalParticleSystem.gameObject);

            EnemySpawnVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_SpawnEnemy_Reticle");
            ShootGroundVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Bullet_Spawn");
            BlueSynergyPoofVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001");

            HealingSparklesVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Healing_Sparkles_001");

            GameObject ChallengeManagerReference = LoadHelper.LoadAssetFromAnywhere<GameObject>("_ChallengeManager");
            GorgunEyesVFX = (ChallengeManagerReference.GetComponent<ChallengeManager>().PossibleChallenges[20].challenge as FloorShockwaveChallengeModifier).EyesVFX;

            EnemyElectricLinkVFX = (ChallengeManagerReference.GetComponent<ChallengeManager>().PossibleChallenges[18].challenge as CircleBurstChallengeModifier).ChainLightningVFX;
            EnemyZappyTellVFX = (ChallengeManagerReference.GetComponent<ChallengeManager>().PossibleChallenges[18].challenge as CircleBurstChallengeModifier).tellVFX;

            GameObject dragunBoulder = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyBoulder;
            foreach (Component item in dragunBoulder.GetComponentsInChildren(typeof(Component)))
            {
                if (item is SkyRocket laser)
                {
                    DragunBoulderLandVFX = laser.ExplosionData.effect;       
                }
            }

            BeholsterController behCont = EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b").GetComponent<BeholsterController>();
            BeholsterChargeUpVFX = behCont.chargeUpVfx;
            BeholsterChargeDownVFX = behCont.chargeDownVfx;

            foreach (VFXComplex vFXComplex in behCont.chargeUpVfx.effects)
            {
                foreach (VFXObject vFXObject in vFXComplex.effects)
                {
                    VFXObject myVFX2 = StaticVFXStorage.CopyFields<VFXObject>(vFXObject);
                    GameObject yas = FakePrefab.Clone(myVFX2.effect);
                    myVFX2.effect = yas;

                    tk2dSprite sprite = myVFX2.effect.GetComponent<tk2dSprite>();
                    sprite.usesOverrideMaterial = true;

                    sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                    sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    sprite.renderer.material.SetFloat("_EmissivePower", 100);
                    sprite.renderer.material.SetFloat("_EmissiveColorPower", 10f);
                    sprite.renderer.material.SetColor("_OverrideColor", Color.cyan);
                    sprite.renderer.material.SetColor("_EmissiveColor", Color.cyan);

                    BeholsterChargeUpVFXInverse = new VFXPool();
                    BeholsterChargeUpVFXInverse.type = VFXPoolType.Single;
                    BeholsterChargeUpVFXInverse.effects = new VFXComplex[] { new VFXComplex() { effects = new VFXObject[] { myVFX2 } } };
                }
            }




            FriendlyElectricLinkVFX = FakePrefab.Clone(Game.Items["shock_rounds"].GetComponent<ComplexProjectileModifier>().ChainLightningVFX);

            /*
            VFXPool aaa = null;
            AIAnimator aiAnimator = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").aiAnimator;
            List<AIAnimator.NamedVFXPool> namedVFX = aiAnimator.OtherVFX;
            foreach (AIAnimator.NamedVFXPool pool in namedVFX)
            {
                if (pool.name == "mergo")
                {
                    aaa = pool.vfxPool;
                }
            }

            List<AIAnimator.NamedVFXPool> namedVFXList = yourAIanimator.OtherVFX;
            if (namedVFXList == null) { namedVFXList = new List<AIAnimator.NamedVFXPool>() { }; }
            namedVFXList.Add(new AIAnimator.NamedVFXPool()
            {
                name = "yourName",
                vfxPool = aaa
            });
            */

            AIAnimator aiAnimator = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").aiAnimator;
            List<AIAnimator.NamedVFXPool> namedVFX = aiAnimator.OtherVFX;
            foreach (AIAnimator.NamedVFXPool pool in namedVFX)
            {
                if (pool.name == "mergo")
                {
                    foreach (VFXComplex vFXComplex in pool.vfxPool.effects)
                    {
                        foreach (VFXObject vFXObject in vFXComplex.effects)
                        {
                            VFXObject myVFX = StaticVFXStorage.CopyFields<VFXObject>(vFXObject);
                            HighPriestClapVFX = new VFXPool();
                            HighPriestClapVFX.type = VFXPoolType.Single;
                            HighPriestClapVFX.effects = new VFXComplex[] {new VFXComplex() {effects = new VFXObject[] { myVFX } } };

                            VFXObject myVFX2 = StaticVFXStorage.CopyFields<VFXObject>(vFXObject);
                            GameObject yas=  FakePrefab.Clone(myVFX2.effect);
                            myVFX2.effect = yas;

                            tk2dSprite sprite = myVFX2.effect.GetComponent<tk2dSprite>();
                            sprite.usesOverrideMaterial = true;

                            sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                            sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                            sprite.renderer.material.SetFloat("_EmissivePower", 100);
                            sprite.renderer.material.SetFloat("_EmissiveColorPower", 10f);
                            sprite.renderer.material.SetColor("_OverrideColor", Color.cyan);
                            sprite.renderer.material.SetColor("_EmissiveColor", Color.cyan);

                            HighPriestClapVFXInverse = new VFXPool();
                            HighPriestClapVFXInverse.type = VFXPoolType.Single;
                            HighPriestClapVFXInverse.effects = new VFXComplex[] { new VFXComplex() { effects = new VFXObject[] { myVFX2 } } };
                        }
                    }
                }
            }
            GunFullyChargedVFX = BraveResources.Load<GameObject>("Global VFX/VFX_DBZ_Charge", ".prefab");
            DodgeRollImpactVFX = (GameObject)BraveResources.Load("Global VFX/VFX_DodgeRollHit", ".prefab");

            BlackHoleImpact = (PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect;

            var machoBrace = PickupObjectDatabase.GetById(665) as MachoBraceItem;
            MachoBraceDustupVFX = machoBrace.DustUpVFX;
            MachoBraceBurstVFX = machoBrace.BurstVFX;

            var relodestone = PickupObjectDatabase.GetById(536) as RelodestoneItem;
            RelodestoneContinuousSuckVFX = relodestone.ContinuousVFX;

            //var relodestone = PickupObjectDatabase.GetById(536) as RelodestoneItem;
            //KnifeShieldKnifeTileImpactVFX = relodestone.ContinuousVFX;

            //AIActor GuNut = EnemyDatabase.GetOrLoadByGuid("ec8ea75b557d4e7b8ceeaacdf6f8238c").aiActor;
            //if (GuNut.GetComponentInChildren<GunNutSlamVfx>() != null) { ETGModConsole.Log("fdfdfdaafddffddfadfadfdfa"); }




            MourningStarLaser = FakePrefab.Clone((PickupObjectDatabase.GetById(515) as Gun).DefaultModule.projectiles[0].bleedEffect.vfxExplosion);
            var mourningStarComp = MourningStarLaser.AddComponent<MourningStarVFXController>();
            var hODC= MourningStarLaser.GetComponent<HammerOfDawnController>();
            mourningStarComp.BeamSections = hODC.BeamSections;
            mourningStarComp.BurstSprite = hODC.BurstSprite;
            mourningStarComp.SectionStartAnimation = hODC.SectionStartAnimation;
            mourningStarComp.SectionAnimation = hODC.SectionAnimation;
            mourningStarComp.SectionEndAnimation = hODC.SectionEndAnimation;
            mourningStarComp.CapAnimation = hODC.CapAnimation;
            mourningStarComp.CapEndAnimation = hODC.CapEndAnimation;
            mourningStarComp.InitialImpactVFX = hODC.InitialImpactVFX;
            UnityEngine.Object.Destroy(hODC);


            var forgeDungeon = DungeonDatabase.GetOrLoadByName("Base_Forge");
            hellDragController = forgeDungeon.PatternSettings.flows[0].AllNodes.Where(node => node.overrideExactRoom != null && node.overrideExactRoom.name.Contains("EndTimes")).First().overrideExactRoom.placedObjects.Where(ppod => ppod != null && ppod.nonenemyBehaviour != null).First().nonenemyBehaviour.gameObject.GetComponentsInChildren<HellDragZoneController>()[0];
            forgeDungeon = null;

            SpiratTeleportVFX = EnemyDatabase.GetOrLoadByGuid("56fb939a434140308b8f257f0f447829").bulletBank.GetBullet("rogue").BulletObject.GetComponent<TeleportProjModifier>().teleportVfx;

            FearVFX = ResourceCache.Acquire("Global VFX/VFX_Fear") as GameObject;

            SpookySkullVFX = (PickupObjectDatabase.GetById(45) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;


            Pixel = PrefabBuilder.BuildObject("Pixel");
            Pixel.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            var sprite_pixel = Pixel.AddComponent<tk2dSprite>();
            sprite_pixel.SetSprite(StaticSpriteDefinitions.Debuff_Sheet_Data, "pixel_create_007");
            tk2dSpriteAnimator animator = Pixel.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.VFX_Animation_Data;
            animator.Library = StaticSpriteDefinitions.VFX_Animation_Data;
            animator.defaultClipId = StaticSpriteDefinitions.VFX_Animation_Data.GetClipIdByName("createPixel");
            animator.playAutomatically = true;

            animator.sprite.usesOverrideMaterial = true;
            Material mat = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
            mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            mat.SetFloat("_EmissivePower", 100);
            mat.SetFloat("_EmissiveColorPower", 10f);
            mat.SetColor("_OverrideColor", Color.cyan);
            mat.SetColor("_EmissiveColor", Color.cyan);
            mat.mainTexture = animator.sprite.renderer.material.mainTexture;
            animator.sprite.renderer.material = mat;


            var ParticleObject = FakePrefab.Clone(PickupObjectDatabase.GetById(597).transform.GetChild(6).transform.gameObject);
            ParticleObject.gameObject.SetActive(true);
            VoidParticleSystem = ParticleObject.GetComponent<ParticleSystem>();
            //ParticleObject.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));

            var sz = VoidParticleSystem.sizeOverLifetime;

            sz.enabled = true;

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 0.01f);
            curve.AddKey(0.3f, 1.0f);
            curve.AddKey(1f, 0);
            sz.size = new ParticleSystem.MinMaxCurve(1.5f, curve);

            //SilencerVFX = (PickupObjectDatabase.GetById(499) as ReusableBlankitem).silencerVFXPrefab;

            WoodBeamImpact = (PickupObjectDatabase.GetById(610) as Gun).DefaultModule.projectiles[0].GetComponent<BasicBeamController>().angularKnockbackTiers.Last().hitRigidbodyVFX.effects[0].effects[0].effect;
            TeleportTelefragVFX = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>().TelefragVFXPrefab;
        }
        public static GameObject Pixel;

        public static ParticleSystem VoidParticleSystem;
        public static GameObject SilencerVFX;
        public static GameObject WoodBeamImpact;
        public static GameObject SeriousCannonImpact = (PickupObjectDatabase.GetById(37) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.tileMapHorizontal.effects.First().effects.First().effect;
        public class PixelDisplayer
        {
            public PixelDisplayer(List<PixelItem> rows, bool Shuffle = false)
            {
                pixelRows = rows;
                if (Shuffle)
                {
                    pixelRows.Shuffle();
                }
            }
            public void Initialise()
            {
                GameManager.Instance.StartCoroutine(DoDisplay());
            }


            public void Clear()
            {
                foreach (var entry in pixelRows)
                {
                    if (entry.Instance)
                    {
                        UnityEngine.Object.Destroy(entry.Instance);
                    }
                }
            }

            public float TotalSpawnTime = 1;
            public float Scale;
            public Vector2 StartPosition;
            public List<PixelItem> pixelRows;
            public Action OnComplete;
            public IEnumerator DoDisplay()
            {
                for (int i = 0; i < pixelRows.Count; i++)
                {
                    var obj = UnityEngine.Object.Instantiate(Pixel, StartPosition + (pixelRows[i].Pos * Scale), Quaternion.identity);
                    obj.SetActive(true);
                    obj.transform.localScale *= Scale;
                    obj.GetComponent<tk2dSprite>().color = pixelRows[i].color;
                    pixelRows[i].Instance = obj;
                    yield return new WaitForSeconds(TotalSpawnTime / pixelRows.Count); 
                }
                yield return new WaitForSeconds(1);
                OnComplete?.Invoke();

                yield break;
            }
            public bool isDone = false;

            public class PixelItem
            {
                public Vector2 Pos
                {
                    get
                    {
                        return new Vector2(x, y);
                    }
                }
                public Color color = Color.white;
                int x;
                int y;
                public PixelItem(int x_, int y_, Color? color_ = null)
                {
                    x = x_;
                    y = y_;
                    if (color_ != null)
                    {
                        color = color_.Value;
                    }
                }
                public bool Spawns;
                public GameObject Instance;
                public float Delay;
            }
        }



        public class MourningStarVFXController : BraveBehaviour
        {
            public static MourningStarVFXController SpawnMourningStar(Vector2 position,float lifeTime = -1 ,Transform parent = null)
            {
                var h = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.MourningStarLaser, position, Quaternion.identity, parent).GetComponent<StaticVFXStorage.MourningStarVFXController>();
                if (lifeTime != -1 && lifeTime > 0) { h.Invoke("Dissipate", lifeTime); }
                return h;
            }


            private void Start()
            {
                isbeingTossed = false;
                TimeExtant = 0;
                for (int i = 0; i < this.BeamSections.Count; i++)
                {
                    tk2dSpriteAnimator spriteAnimator = this.BeamSections[i].spriteAnimator;
                    if (spriteAnimator)
                    {
                        spriteAnimator.alwaysUpdateOffscreen = true;
                        spriteAnimator.PlayForDuration(this.SectionStartAnimation, -1f, this.SectionAnimation, false);
                        if (DoesSound == true)
                        {
                            AkSoundEngine.PostEvent("Play_WPN_dawnhammer_loop_01", base.gameObject);
                            AkSoundEngine.PostEvent("Play_State_Volume_Lower_01", base.gameObject);
                        }
                    }
                }
                if (OnBeamStart != null) { OnBeamStart(this.gameObject); }
                base.spriteAnimator.alwaysUpdateOffscreen = true;
                this.BurstSprite.UpdateZDepth();
                base.sprite.renderer.enabled = false;
            }

            public void Update()
            {
                if (isbeingTossed == true) { return; }
                TimeExtant += BraveTime.DeltaTime;
                base.sprite.UpdateZDepth();
                for (int i = 0; i < this.BeamSections.Count; i++)
                {
                    this.BeamSections[i].UpdateZDepth();
                }
                this.BurstSprite.UpdateZDepth();
                if (!this.BurstSprite.renderer.enabled)
                {
                    base.sprite.renderer.enabled = true;
                    base.spriteAnimator.Play(this.CapAnimation);
                }
                if (DoesEmbers == true)
                {
                    if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.MEDIUM || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.HIGH)
                    {
                        int num4 = (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.HIGH) ? 50 : 125;
                        this.m_particleCounter += BraveTime.DeltaTime * (float)num4;
                        if (this.m_particleCounter > 1f)
                        {
                            GlobalSparksDoer.DoRadialParticleBurst(Mathf.FloorToInt(this.m_particleCounter), base.sprite.WorldBottomLeft, base.sprite.WorldTopRight, 30f, 2f, 1f, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                            this.m_particleCounter %= 1f;
                        }
                    }
                }
                

                if (OnBeamUpdate != null) { OnBeamUpdate(this.gameObject, TimeExtant); }

            }

            public void Dissipate()
            {
                isbeingTossed = true;
                if (OnBeamDie != null) { OnBeamDie(this.gameObject); }
                base.sprite.renderer.enabled = true;
                ParticleSystem componentInChildren = base.GetComponentInChildren<ParticleSystem>();
                if (componentInChildren)
                {
                    BraveUtility.EnableEmission(componentInChildren, false);
                }
                for (int i = 0; i < this.BeamSections.Count; i++)
                {
                    this.BeamSections[i].spriteAnimator.Play(this.SectionEndAnimation);
                }
                base.spriteAnimator.PlayAndDestroyObject(this.CapEndAnimation, null);
                UnityEngine.Object.Destroy(base.gameObject, 1f);
                if (DoesSound == true)
                {
                    AkSoundEngine.PostEvent("Stop_WPN_gun_loop_01", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_State_Volume_Lower_01", base.gameObject);
                }
            }

            public override void OnDestroy()
            {
                if (DoesSound == true)
                {
                    AkSoundEngine.PostEvent("Stop_WPN_gun_loop_01", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_State_Volume_Lower_01", base.gameObject);
                }
                base.OnDestroy();
            }

            public Action<GameObject> OnBeamStart;
            public Action<GameObject, float> OnBeamUpdate;
            public Action<GameObject> OnBeamDie;

            private float TimeExtant;

            public float TimeAlive(){ return TimeExtant; }

            public bool DoesSound = true;
            public bool DoesEmbers = true;

            private bool isbeingTossed;

            public List<tk2dSprite> BeamSections;
            public tk2dSprite BurstSprite;
            public GameObject InitialImpactVFX;

            public string SectionStartAnimation;
            public string SectionAnimation;
            public string SectionEndAnimation;
            public string CapAnimation;
            public string CapEndAnimation;

            private float m_particleCounter;
        }


        public static GameObject SmarterPlayEffectOnActor(this GameActor actor, GameObject effect, Vector3 offset, bool ignorePool = true, bool attached = true, bool alreadyMiddleCenter = false, bool useHitbox = false)
        {
            GameObject gameObject = SpawnManager.SpawnVFX(effect, ignorePool);
            tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
            Vector3 a = (!useHitbox || !actor.specRigidbody || actor.specRigidbody.HitboxPixelCollider == null) ? actor.sprite.WorldCenter.ToVector3ZUp(0f) : actor.specRigidbody.HitboxPixelCollider.UnitCenter.ToVector3ZUp(0f);
            if (!alreadyMiddleCenter)
            {
                component.PlaceAtPositionByAnchor(a + offset, tk2dBaseSprite.Anchor.MiddleCenter);
            }
            else
            {
                component.transform.position = a + offset;
            }
            if (attached)
            {
                gameObject.transform.parent = actor.transform;
                component.HeightOffGround = 0.2f;
                actor.sprite.AttachRenderer(component);
                if (actor is PlayerController)
                {
                    SmartOverheadVFXController component2 = gameObject.GetComponent<SmartOverheadVFXController>();
                    if (component2 != null)
                    {
                        component2.Initialize(actor as PlayerController, offset);
                    }
                }
            }
            if (!alreadyMiddleCenter)
            {
                gameObject.transform.localPosition = gameObject.transform.localPosition.QuantizeFloor(0.0625f);
            }
            return gameObject;
        }
    

        public static VFXObject CopyFields<T>(VFXObject sample2) where T : VFXObject
        {
            VFXObject sample = new VFXObject();
            sample.alignment = sample2.alignment;
            sample.attached = sample2.attached;
            sample.destructible = sample2.destructible;
            sample.effect = sample2.effect;
            sample.orphaned = sample2.orphaned;
            sample.persistsOnDeath = sample2.persistsOnDeath;
            sample.usesZHeight = sample2.usesZHeight;
            return sample;
        }

        public static GameObject RelodestoneContinuousSuckVFX;


        //public static GameObject KnifeShieldKnifeTileImpactVFX;

        public static GameObject SpookySkullVFX;

        public static GameObject FearVFX;

        public static VFXPool SpiratTeleportVFX;


        public static GameObject MourningStarLaser;

        public static GameObject MachoBraceDustupVFX;
        public static GameObject MachoBraceBurstVFX;


        public static VFXPool HighPriestClapVFX;
        public static VFXPool HighPriestClapVFXInverse;

        public static VFXPool BeholsterChargeUpVFXInverse;


        public static VFXPool BeholsterChargeUpVFX;
        public static VFXPool BeholsterChargeDownVFX;


        public static GameObject DodgeRollImpactVFX;


        public static GameObject DragunBoulderLandVFX;

        public static GameObject GunFullyChargedVFX;

        public static GameObject BloodiedScarfPoofVFX;


        public static GameObject GorgunEyesVFX;
        public static GameObject EnemyElectricLinkVFX;
        public static GameObject EnemyZappyTellVFX;
        public static GameObject FriendlyElectricLinkVFX;


        public static ScarfAttachmentDoer ScarfObject;
        public static GameObject RadialRing;
        public static GameObject TeleportDistortVFX;
        public static GameObject TeleportTelefragVFX;
        public static GameObject TeleportVFX;
        public static GameObject EnemySpawnVFX;
        public static GameObject ShootGroundVFX;
        public static GameObject BlueSynergyPoofVFX;
        public static GameObject HealingSparklesVFX;

        public static ParticleSystem PerkParticleSystem;
        public static ParticleSystem PerfectedParticleSystem;
        public static ParticleSystem PortalParticleSystem;



        public static GameObject PerkParticleObject;

        public static ParticleSystem CeramicParticleSystem;
        public static ParticleSystem BloodSplatterParticleSystem;
        public static ParticleSystem ShamberParticleSystem;
        public static ParticleSystem EliteParticleSystem;

        public static GameObject ShamberParticleSystemGameObject;
        public static GameObject EliteParticleSystemGameObject;
        public static GameObject PortalParticleSystemGameObject;


        public static GameObject JammedDeathVFX;

        public static HellDragZoneController hellDragController;

        public static GameObject BlackHoleImpact;

    }
}
