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
	public class PlanetBlade : AdvancedGunBehavior
    {
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Crystalline", "planetblade");
			Game.Items.Rename("outdated_gun_mods:crystalline", "psog:crystalline");
			gun.gameObject.AddComponent<PlanetBlade>();
			gun.SetShortDescription("Tears Of Providence");
			gun.SetLongDescription("A pure, crystalline sword made for a demi-god to protect the people who looked up to it.\n\nAlthough dead, the tears of the demi-god that once carried it rain down from the heavens, causing the feeble and most vulnerable to stand for their guardian.");
			GunExt.SetupSprite(gun, null, "planetblade_idle_001", 11);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 23);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 27 );
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 1);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(387) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 1.7f;
			gun.DefaultModule.cooldownTime = .5f;
			gun.DefaultModule.numberOfShotsInClip = 5;
			gun.SetBaseMaxAmmo(50);
            gun.InfiniteAmmo = true;
			gun.quality = PickupObject.ItemQuality.EXCLUDED;
			gun.DefaultModule.angleVariance = 6f;
			gun.DefaultModule.burstShotCount = 1;
            gun.PreventNormalFireAudio = true;
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 26f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 1f;
            projectile.baseData.force = 5;
            projectile.name = "as weeng";
            projectile.gameObject.AddComponent<PlanetBladeProjectile>();

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_OBJ_katana_slash_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_PET_junk_spin_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;

            gun.gunClass = GunClass.ICE;
            gun.IsHeroSword = true;
			gun.HeroSwordDoesntBlank = false;
			gun.encounterTrackable.EncounterGuid = "Planetside Sword | Bottom Text";
            gun.muzzleFlashEffects = OtherTools.CreateMuzzleflash("planetSlash", new List<string> { "planetblade_slash_001", "planetblade_slash_002", "planetblade_slash_003", "planetblade_slash_004", "planetblade_slash_005" }, 30, new List<IntVector2> { new IntVector2(6, 16), new IntVector2(16, 96), new IntVector2(16, 96), new IntVector2(12, 72), new IntVector2(6, 54) }, new List<tk2dBaseSprite.Anchor> {
                tk2dBaseSprite.Anchor.MiddleLeft, tk2dBaseSprite.Anchor.MiddleLeft , tk2dBaseSprite.Anchor.MiddleLeft , tk2dBaseSprite.Anchor.MiddleLeft , tk2dBaseSprite.Anchor.MiddleLeft }, new List<Vector2> { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero }, false, false, false, false, 0, VFXAlignment.Fixed, true, new List<float> { 100f, 100f, 100f, 100f, 100f }, new List<Color> { new Color(0, 0.8f,1),
                new Color(0, 1f,1),
               new Color(1, 1f,1),
                new Color(0, 0.8f,1),
               new Color(0, 0.2f,1)});

            ETGMod.Databases.Items.Add(gun, false, "ANY");

            tk2dSpriteAnimationClip fireClip = gun.sprite.spriteAnimator.GetClipByName(gun.shootAnimation);
            float[] offsetsX = new float[] { -0.375f, -0.3125f, -1.625f, -1.75f, -1.8125f, -1.875f, -1.9125f, -1.875f, -1.625f ,-1.25f ,-1.0625f, -0.9125f, -0.75f, -0.125f, 1 };
            float[] offsetsY = new float[] { 1f, 0.875f, -1.625f ,-2.25f, -2.375f, -2f, -1.875f, -1.75f,-1.625f, -1.5625f -1.4375f, -1.375f, -1f, -0.75f, -0.25f};
            for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < fireClip.frames.Length; i++)
            {
                int id = fireClip.frames[i].spriteId;
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY[i];

            }

            tk2dSpriteAnimationClip fireClip2 = gun.sprite.spriteAnimator.GetClipByName(gun.reloadAnimation);
            float[] offsetsX2 = new float[] { 0.25f, 0f, -0.25f, -0.5f, -0.25f, 0f, .25f, .5f, .25f, 0f, 0f, 0f, 0f };
            float[] offsetsY2 = new float[] { -0.25f, -0.5f, -0.75f, -1f, -0.5f, 0f, 0.5f, 1f, 0.5f, 0.375f, 0.125f, 0f, 0f };
            for (int i = 0; i < offsetsX2.Length && i < offsetsY2.Length && i < fireClip2.frames.Length; i++)
            {
                int id = fireClip2.frames[i].spriteId;
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY2[i];
            }

            gun.sprite.usesOverrideMaterial = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 3f);
            mat.SetFloat("_EmissivePower", 4);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.15f);
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
        }

        private static List<GameObject> Chargerreticles = new List<GameObject>();
        //private static GameObject LaserReticle;
        private bool VFXActive;
        public void CleanupReticles()
        {
            for (int i = 0; i < PlanetBlade.Chargerreticles.Count; i++)
            {
                if (PlanetBlade.Chargerreticles[i] != null)
                {
                    SpawnManager.Despawn(PlanetBlade.Chargerreticles[i]);
                    Destroy(PlanetBlade.Chargerreticles[i]);
                }
            }
            PlanetBlade.Chargerreticles.Clear();
            
            
            PlanetsideWeatherController expandWeatherController = GameManager.Instance.Dungeon.gameObject.GetComponent<PlanetsideWeatherController>();
            foreach (Component item in GameManager.Instance.Dungeon.gameObject.GetComponents(typeof(Component)))
            {
                if (item is PlanetsideWeatherController laser)
                {
                    if (laser.name == "PlanetsideRainC")
                    {
                        expandWeatherController.ForceStopRain(true, "PlanetsideRainC");
                    }
                }
            }
            
        }

        private bool HasReloaded;
        public void OnDestroy()
        {
            ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Remove(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
            GameManager.Instance.OnNewLevelFullyLoaded -= this.OnNewFloor;
            CleanupReticles();
        }
        private void OnNewFloor() { VFXActive = false; CleanupReticles();   }


        protected override void Update()
		{
            base.Update();
            PlayerController player = gun.CurrentOwner as PlayerController;
            if (gun.CurrentOwner && player != null)
			{
                gun.PreventNormalFireAudio = true;
                if (VFXActive != true)
                {
                    
                    
                    bool HasMadeRain = false;
                    foreach (Component item in GameManager.Instance.Dungeon.gameObject.GetComponents(typeof(Component)))
                    {
                        if (item is PlanetsideWeatherController laser)
                        {
                            if (laser.name == "PlanetsideRainC")
                            {
                                HasMadeRain = true;
                            }
                        }
                    }
                    if (HasMadeRain == false)
                    {
                        PlanetsideWeatherController expandWeatherController = GameManager.Instance.Dungeon.gameObject.AddComponent<PlanetsideWeatherController>();
                        expandWeatherController.RainIntensity = 20;
                        expandWeatherController.useCustomIntensity = true;
                        expandWeatherController.enableLightning = false;
                        expandWeatherController.isSecretFloor = false;
                        expandWeatherController.name = "PlanetsideRainC";
                    }
                    
                    VFXActive = true;
                    for (int i = -2; i < 3; i++)
                    {
                        float num2 = 2f;
                        Vector2 zero = Vector2.zero;
                        if (BraveMathCollege.LineSegmentRectangleIntersection(player.transform.position, player.transform.position + BraveMathCollege.DegreesToVector(45*i, 60f).ToVector3ZisY(-0.25f), new Vector2(-40, -40), new Vector2(40, 40), ref zero))
                        {
                            num2 = (zero - new Vector2(player.transform.position.x, player.transform.position.y)).magnitude;
                        }
                        GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
                        tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();


                        Vector2 Point = MathToolbox.GetUnitOnCircle(45 * i, 10);

                        component2.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 99999);
                        component2.transform.localRotation = Quaternion.Euler(Point.magnitude, Point.magnitude, (45 * i) + 90 + 3600);
                        component2.dimensions = new Vector2((num2) * 2f, 1f);
                        component2.UpdateZDepth();
                        component2.HeightOffGround = -2;
                        component2.renderer.enabled = true;
                        component2.transform.position.WithZ(component2.transform.position.z + 99999);
                        PlanetBlade.Chargerreticles.Add(gameObject);
                    }
                }
                for (int i = -2; i < 3; i++)
                {
                    GameObject obj = Chargerreticles[i + 2];
                    if (obj != null)
                    {
                        tk2dTiledSprite component2 = obj.GetComponent<tk2dTiledSprite>();
                        component2.usesOverrideMaterial = true;
                        component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                        component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                        component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
                        component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
                        Color laser = new Color(0f, 1f, 1f, 1f);
                        component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                        component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
                        float ix = obj.transform.localRotation.eulerAngles.x;
                        float wai = obj.transform.localRotation.eulerAngles.y;
                        float zee = obj.transform.localRotation.z;
                        obj.transform.localRotation = Quaternion.Euler(ix, wai, zee + (45 * i) + 90 + 3600);

                        Vector2 Point1 = MathToolbox.GetUnitOnCircle((45 * i)+90, 1f);
                        obj.transform.position = player.sprite.WorldCenter + Point1;
                        component2.transform.position.WithZ(component2.transform.position.z + 99999);
                        component2.UpdateZDepth();
                        component2.HeightOffGround = -2;
                        component2.renderer.enabled = true;
                        component2.dimensions = new Vector2(3, 1f);
                    }
                }
                if (!gun.PreventNormalFireAudio)
				{
					this.gun.PreventNormalFireAudio = true;
				}
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}
			}
            else
            {
                VFXActive = false;
                CleanupReticles();
            }

		}
        
        protected override void OnPickup(PlayerController player)
        {
            GameManager.Instance.OnNewLevelFullyLoaded += this.OnNewFloor;
            ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
            VFXActive = false;
            player.GunChanged += this.OnGunChanged;
            base.OnPickup(player);
            CleanupReticles();
        }

        protected override void OnPostDrop(PlayerController player)
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= this.OnNewFloor;
            ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Remove(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
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
                    VFXActive = false;
                }
            }
        }
        public void AIActorMods(AIActor AIActor)
        {
            bool flag = AIActor && AIActor.aiActor && AIActor.aiActor.EnemyGuid != null;
            if (flag)
            {
                string text;
                if (AIActor == null)
                {
                    text = null;
                }
                else
                {
                    AIActor aiActor = AIActor.aiActor;
                    text = ((aiActor != null) ? aiActor.EnemyGuid : null);
                }
                string text2 = text;
                bool flag2 = !string.IsNullOrEmpty(text2);
                if (flag2)
                {
                    try
                    {
                        bool flag3 = PlanetBlade.weakEnemies.Contains(text2);
                        if (flag3)
                        {
                            if (AIActor.GetComponent<CompanionController>() == null && AIActor.GetComponent<MarkForDupe>() == null)
                            {
                                string guid = AIActor.EnemyGuid;
                                var enemyPrefab = EnemyDatabase.GetOrLoadByGuid(guid);
                                AIActor aiactor = AIActor.Spawn(enemyPrefab, AIActor.gameActor.CenterPosition.ToIntVector2(VectorConversions.Floor), AIActor.GetAbsoluteParentRoom(), true, AIActor.AwakenAnimationType.Default, true);

                                aiactor.gameObject.AddComponent<MarkForDupe>();
                                aiactor.procedurallyOutlined = true;
                                aiactor.aiAnimator.facingType = AIAnimator.FacingType.Movement;
                                aiactor.AssignedCurrencyToDrop = AIActor.AssignedCurrencyToDrop;
                                aiactor.AdditionalSafeItemDrops = AIActor.AdditionalSafeItemDrops;
                                aiactor.AdditionalSimpleItemDrops = AIActor.AdditionalSimpleItemDrops;
                                aiactor.CanTargetEnemies = AIActor.CanTargetEnemies;
                                aiactor.CanTargetPlayers = AIActor.CanTargetPlayers;
                                float HP = aiactor.healthHaver.GetMaxHealth();
                                aiactor.healthHaver.SetHealthMaximum(HP*3);
                                if (AIActor.IsInReinforcementLayer) aiactor.HandleReinforcementFallIntoRoom(0f);
                                if (aiactor.EnemyGuid == "22fc2c2c45fb47cf9fb5f7b043a70122")
                                {
                                    aiactor.CollisionDamage = 0f;
                                    aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox));
                                    aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.Projectile));
                                }
                                else if (aiactor.EnemyGuid == "249db525a9464e5282d02162c88e0357")
                                {
                                    if (aiactor.gameObject.GetComponent<SpawnEnemyOnDeath>())
                                    {
                                        UnityEngine.Object.Destroy(aiactor.gameObject.GetComponent<SpawnEnemyOnDeath>());
                                    }
                                }




                                PlayerController player = gun.CurrentOwner as PlayerController;
                                aiactor.CanTargetEnemies = true;
                                aiactor.CanTargetPlayers = true;
                                aiactor.IsHarmlessEnemy = true;
                                aiactor.IgnoreForRoomClear = true;
                                CompanionController yup = aiactor.gameObject.AddComponent<CompanionController>();
                                yup.companionID = CompanionController.CompanionIdentifier.NONE;
                                yup.CanCrossPits = true;
                                yup.Initialize(player);
                                Planetside.OtherTools.CompanionisedEnemyBulletModifiers yeehaw = yup.gameObject.AddComponent<Planetside.OtherTools.CompanionisedEnemyBulletModifiers>();
                                yeehaw.jammedDamageMultiplier *= 2.4f;
                                yeehaw.baseBulletDamage = 5f;
                                yeehaw.TintBullets = true;
                                yeehaw.TintColor = new Color(0, 0.6f, 0.6f);

                                bool flag4 = aiactor.gameObject.GetComponent<SpawnEnemyOnDeath>();
                                if (flag4)
                                {
                                    UnityEngine.Object.Destroy(aiactor.gameObject.GetComponent<SpawnEnemyOnDeath>());
                                }
                                var bs = aiactor.GetComponent<BehaviorSpeculator>();

                                foreach (MovementBehaviorBase att in aiactor.behaviorSpeculator.MovementBehaviors)
                                {
                                    if (att is SeekTargetBehavior)
                                    {
                                        SeekTargetBehavior tagr = att as SeekTargetBehavior;
                                        tagr.ReturnToSpawn = false;
                                        tagr.StopWhenInRange = false;
                                        tagr.CustomRange = 7;
                                        tagr.LineOfSight = true;
                                        tagr.SpawnTetherDistance = 0;
                                        tagr.PathInterval = 0.25f;
                                        tagr.SpecifyRange = false;
                                        tagr.MinActiveRange =  3;
                                        tagr.MaxActiveRange = 11;

                                    }
                                }

                                AIAnimator aiAnimator = aiactor.aiAnimator;

                                CompanionFollowPlayerBehavior comp = new CompanionFollowPlayerBehavior();
                                comp.CanRollOverPits = false;
                                comp.CatchUpOutAnimation = aiAnimator.MoveAnimation.Prefix;
                                comp.DisableInCombat = false;
                                comp.IdleAnimations = aiAnimator.IdleAnimation.AnimNames;
                                comp.PathInterval = 0.25f;
                                comp.IdealRadius = 3;
                                comp.CatchUpRadius = 8;
                                comp.CatchUpAccelTime = 5;
                                comp.CatchUpSpeed = aiactor.MovementSpeed *= 1.125f;
                                comp.CatchUpMaxSpeed = aiactor.MovementSpeed *= 1.3f;
                                comp.CatchUpAnimation = aiAnimator.MoveAnimation.Prefix;
                                comp.RollAnimation = aiAnimator.MoveAnimation.Prefix;
                                comp.TemporarilyDisabled = false;

                                bs.MovementBehaviors.Add(comp);


                                SeekTargetBehavior seek = new SeekTargetBehavior();
                                seek.ReturnToSpawn = false;
                                seek.StopWhenInRange = false;
                                seek.CustomRange = 7;
                                seek.LineOfSight = true;
                                seek.SpawnTetherDistance = 0;
                                seek.PathInterval = 0.25f;
                                seek.SpecifyRange = false;
                                seek.MinActiveRange = 3;
                                seek.MaxActiveRange = 11;

                                bs.MovementBehaviors.Add(seek);

                                aiactor.CompanionOwner = player;



                                aiactor.specRigidbody.Reinitialize();

                                Destroy(AIActor.gameObject);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ETGModConsole.Log(ex.Message, false);
                    }
                }
            }

        }

        public static List<string> weakEnemies = new List<string>
        {
            EnemyGuidDatabase.Entries["bullet_kin"],
            EnemyGuidDatabase.Entries["mutant_bullet_kin"],
            EnemyGuidDatabase.Entries["office_bullet_kin"],
            EnemyGuidDatabase.Entries["office_bullette_kin"],
            EnemyGuidDatabase.Entries["western_bullet_kin"],
            EnemyGuidDatabase.Entries["pirate_bullet_kin"],
            EnemyGuidDatabase.Entries["armored_bullet_kin"],
            EnemyGuidDatabase.Entries["skusket"],
            EnemyGuidDatabase.Entries["apprentice_gunjurer"],
            EnemyGuidDatabase.Entries["coaler"],
            EnemyGuidDatabase.Entries["fungun"],
            EnemyGuidDatabase.Entries["pot_fairy"],
            EnemyGuidDatabase.Entries["arrow_head"],
            EnemyGuidDatabase.Entries["gigi"],
            EnemyGuidDatabase.Entries["gunzookie"],
            EnemyGuidDatabase.Entries["beadie"],
        };
        public override void OnReloadPressed(PlayerController player, Gun bruhgun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				HasReloaded = false;
				base.OnReloadPressed(player, gun, bSOMETHING);
			}
		}
	}
    public class MarkForDupe : BraveBehaviour
    {
        public void Start()
        {
            if (base.aiActor != null)
            {
                CustomScarfDoer scorf = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.ScarfObject.gameObject).AddComponent<CustomScarfDoer>();
                scorf.AttachTarget = base.aiActor;
                scorf.ScarfMaterial = StaticVFXStorage.ScarfObject.ScarfMaterial;
                scorf.StartWidth = 0.0625f;
                scorf.EndWidth = 0.125f;
                scorf.AnimationSpeed = 30f;
                scorf.ScarfLength = 1.2f;
                scorf.AngleLerpSpeed = 30f;
                scorf.BackwardZOffset = -0.2f;
                scorf.CatchUpScale = 1.3f;
                scorf.SinSpeed = 9f;
                scorf.AmplitudeMod = 0.235f;
                scorf.WavelengthMod = 1.3f;

                scorf.ScarfMaterial.SetColor("_OverrideColor", new Color(0f, 0.6f, 0.6f));

                scorf.Initialize(base.aiActor);
            }
            if (base.aiActor.healthHaver != null && base.aiActor.aiActor != null)
            {
                Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(base.aiActor.sprite);
                if (!base.aiActor.healthHaver.IsDead && outlineMaterial1 != null)
                {
                    outlineMaterial1.SetColor("_OverrideColor", new Color(0,0,0));
                }
            }
        }
    }
}



