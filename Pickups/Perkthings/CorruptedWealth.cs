using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Planetside
{


    class CorruptedPickupController : PickupObject
    {
        public CorruptedPickupController()
        {
        }
        public void Start() 
        {
            this.sprite.usesOverrideMaterial = true;
            Material mat = this.sprite.renderer.material;     
            mat.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            mat.SetFloat("_EmissivePower", 100);
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetColor("_OverrideColor", new Color(0.02f, 0.4f, 1, 0.6f));
        }

        public void Update()
        {
            var d = this.GetComponent<PlayerOrbitalItem>();
            var D = this.GetComponentInChildren<PlayerOrbitalItem>();
            if (d != null)
            {
                d.OrbitalPrefab = CorruptedWealth.VoidGlassStonePrefab.GetComponent<PlayerOrbital>();
            }
            if (D != null)
            {
                D.OrbitalPrefab = CorruptedWealth.VoidGlassStonePrefab.GetComponent<PlayerOrbital>();
            }
        }

        public CorruptedWealthController GetController(PlayerController P)
        {
            return P.GetComponent<CorruptedWealthController>();
        }


        public override void Pickup(PlayerController player)
        {
            CorruptedWealthController c = GetController(player);
            switch (pickup)
            {

                case PickupType.HP:
                    if (c != null)
                    {
                        c.ProcessDamageModsRedHP(this.GetComponent<HealthPickup>().healAmount);
                        c.AmountOfArmorConsumed += this.GetComponent<HealthPickup>().armorAmount;
                    }
                    break;
                case PickupType.KEY:
                    break;
                case PickupType.AMMO:
                    break;
                case PickupType.BLANK:
                    break;
                case PickupType.GLASS_GUON:
                    break;
            }
        }
        public PickupType pickup;
        public enum PickupType
        {
            HP,//DONE
            KEY,//DONE
            AMMO,
            BLANK,//DONE
            ARMOR,//DONE
            GLASS_GUON
        };
    }


    class CorruptedWealthController : MonoBehaviour
    {
        public CorruptedWealthController()
        {
            this.hasBeenPickedup = false;
        }
        public void Start() 
        { 
            this.hasBeenPickedup = true; 
            StackCount = 1;
            AmountOfHPConsumed = 0;
            AmountOfArmorConsumed = 0;
            AmountOfCorruptBlanks = 0;

            player.healthHaver.OnDamaged += HealthHaver_OnDamaged;
            player.OnUsedBlank += Player_OnUsedBlank;
            player.LostArmor += LostArmorgus;
        }

        public void LostArmorgus()
        {
            if (AmountOfArmorConsumed > 0)
            {
                GameManager.Instance.StartCoroutine(this.SaveFlawless());
                AmountOfArmorConsumed--;
            }
        }
        private IEnumerator SaveFlawless()
        {
            yield return new WaitForSeconds(0.1f);
            if (player.CurrentRoom != null)
            {
                player.CurrentRoom.PlayerHasTakenDamageInThisRoom = false;
            }
            yield break;
        }

        private void Player_OnUsedBlank(PlayerController arg1, int arg2)
        {
            if (AmountOfCorruptBlanks > 0)
            {
                AmountOfCorruptBlanks--;
                GameManager.Instance.StartCoroutine(this.HandleSilence(arg1.sprite.WorldCenter, 30, 5, arg1));
            }
        }

        private IEnumerator HandleSilence(Vector2 centerPoint, float maxRadius, float additionalTimeAtMaxRadius, PlayerController user)
        {

            GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(536) as RelodestoneItem).ContinuousVFX, true);
            vfx.transform.position = user.sprite.WorldCenter;
            vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
            AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", user.gameObject);

            List<Projectile> doNotWreck = new List<Projectile>();
            float elapsed = 0f;
            while (elapsed < additionalTimeAtMaxRadius + (StackCount*1.25f))
            {
                elapsed += BraveTime.DeltaTime;
                bool destroysEnemyBullets = true;
                bool pvp_ENABLED = GameManager.PVP_ENABLED;
                float? previousRadius2 = new float?(maxRadius);
                doNotWreck = BlankHooks.DestroyBulletsInRangeSpecial(centerPoint, maxRadius, destroysEnemyBullets, pvp_ENABLED, doNotWreck,user, false, previousRadius2, false, null);
                yield return null;
            }
            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
            tk2dSpriteAnimator objanimator = silencerVFX.GetComponentInChildren<tk2dSpriteAnimator>();
            objanimator.ignoreTimeScale = true;
            objanimator.AlwaysIgnoreTimeScale = true;
            objanimator.AnimateDuringBossIntros = true;
            objanimator.alwaysUpdateOffscreen = true;
            objanimator.playAutomatically = true;
            ParticleSystem objparticles = silencerVFX.GetComponentInChildren<ParticleSystem>();
            var main = objparticles.main;
            main.useUnscaledTime = true;
            GameObject gameObject = GameObject.Instantiate(silencerVFX.gameObject, user.sprite.WorldCenter, Quaternion.identity);
            Destroy(gameObject, 2.5f);

            UnityEngine.Object.Destroy(vfx, 0);


            yield break;
        }


        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            if (AmountOfHPConsumed > 0f) { AmountOfHPConsumed -= 0.5f; }
            if (AmountOfHPConsumed < 0) { AmountOfArmorConsumed = 0; }

            RecalculateDamage();
            if (AmountOfHPConsumed > 0f)
            {
                AkSoundEngine.PostEvent("Play_BOSS_lichA_crack_01", player.gameObject);
                { AmountOfHPConsumed -= 0.5f; }
                player.healthHaver.ForceSetCurrentHealth(player.healthHaver.GetCurrentHealth() - 0.5f);
                RecalculateDamage();
            }
        }

        public int ActualHPPointsStored()
        {
            return Mathf.CeilToInt(AmountOfHPConsumed * 2);
        }


        public void Update()
        {
            if (player)
            {
                AkSoundEngine.PostEvent("Play_BOSS_Rat_Cheese_Jump_01", player.gameObject);

                GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(577) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects.First().effects.First().effect, true);
                vfx.transform.position = player.sprite.WorldCenter;
                vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                vfx.transform.localScale *= 2;
                vfx.transform.localRotation = Quaternion.Euler(0, 0, Vector2.up.ToAngle());
                UnityEngine.Object.Destroy(vfx, 2);


                if (AmountOfArmorConsumed > (int)player.healthHaver.Armor)
                { AmountOfArmorConsumed = (int)player.healthHaver.Armor; }

                if (LastStoredAmountOfHPConsumed != ActualHPPointsStored())
                {
                    LastStoredAmountOfHPConsumed = ActualHPPointsStored();
                    for (int i = 0; i < objects.Count; i++)
                    {
                        if (objects[i] != null) { Destroy(objects[i]); }
                    }
                    objects.Clear();
                    int UpRaise = 0;
                    int H = ActualHPPointsStored();
                    int r = ActualHPPointsStored();
                    for (int i = 0; i < H / 2; i++)
                    {
                        r -= 2;
                        GameObject gameObject = player.PlayEffectOnActor(CorruptedWealth.heartorbitalVFX, new Vector3(0f, player.specRigidbody.HitboxPixelCollider.UnitDimensions.y + 0.25f + (0.5f * UpRaise), 0f), true);
                        objects.Add(gameObject);
                        UpRaise++;
                    }
                    if (r > 0)
                    {
                        for (int i = 0; i < r; i++)
                        {
                            r--;
                            GameObject gameObject = player.PlayEffectOnActor(CorruptedWealth.halfheartorbitalvfx, new Vector3(0f, player.specRigidbody.HitboxPixelCollider.UnitDimensions.y + 0.25f + (0.5f * UpRaise), 0f), true);
                            objects.Add(gameObject);
                            UpRaise++;
                        }
                    }
                }
                if (player.carriedConsumables.KeyBullets == 0)
                {
                    AmountOfCorruptKeys = 0;
                }
            }      
        }

        public List<GameObject> objects = new List<GameObject>();

        public void ProcessDamageModsRedHP(float amount)
        {
            AmountOfHPConsumed += amount;
            RecalculateDamage();
        }
        private int LastStoredAmountOfHPConsumed;


        public void RecalculateDamage()
        {
            if (HPBasedDamageMod != null)
            {
                player.ownerlessStatModifiers.Remove(HPBasedDamageMod);
            }
            if (AmountOfHPConsumed == 0) { return; }
            StatModifier item = new StatModifier
            {
                statToBoost = PlayerStats.StatType.Damage,
                amount = (AmountOfHPConsumed/4) * (1+ (StackCount / 0.33f)),
                modifyType = StatModifier.ModifyMethod.ADDITIVE
            };
            HPBasedDamageMod = item;
        }


        public void IncrementStack()
        {
            StackCount++;
        }

        

        public int AmountOfCorruptBlanks;
        public int AmountOfCorruptKeys;

        public float AmountOfArmorConsumed;
        public float AmountOfHPConsumed;
        public PlayerController player;

        public int StackCount = 0;
        public bool hasBeenPickedup;
        private StatModifier HPBasedDamageMod;
        private Dictionary<int, GameObject> extantHearts = new Dictionary<int, GameObject>();

       
    }



    class CorruptedWealth : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Corrupted Wealth";
            string resourcePath = "Planetside/Resources/PerkThings/corrputed_wealth.png";
            GameObject gameObject = new GameObject(name);
            CorruptedWealth item = gameObject.AddComponent<CorruptedWealth>();
          
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Corrupts all pickups.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            CorruptedWealth.CorruptedWealthID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.blue;
            particles.ParticleSystemColor2 = Color.blue;
            OutlineColor = new Color(0f, 0.2f, 1f);

            SetupHeartVFX();
            SetupHalfHeartVFX();
            BuildVoidGlassStone();
        }

        public static void BuildVoidGlassStone()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/Guons/VoidglassGuon/voidglass_guon_stone.png");
            gameObject.name = $"Soul Guon";
            SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(7, 9));
            PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
            speculativeRigidbody.CollideWithTileMap = false;
            speculativeRigidbody.CollideWithOthers = true;
            speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.Projectile;
            orbitalPrefab.shouldRotate = true;
            orbitalPrefab.orbitRadius = 6.5f;
            orbitalPrefab.orbitDegreesPerSecond = 60;
            orbitalPrefab.SetOrbitalTier(0);

            Shader glowshader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");

            orbitalPrefab.sprite.usesOverrideMaterial = true;
            orbitalPrefab.sprite.sprite.renderer.material.shader = glowshader;
            orbitalPrefab.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            orbitalPrefab.sprite.renderer.material.SetFloat("_EmissivePower", 80);
            orbitalPrefab.sprite.renderer.material.SetFloat("_EmissiveColorPower", 10);
            gameObject.AddComponent<VoidGlassGuonStoneController>();


            VoidGlassStonePrefab = gameObject;
            UnityEngine.Object.DontDestroyOnLoad(VoidGlassStonePrefab);
            FakePrefab.MarkAsFakePrefab(VoidGlassStonePrefab);
            VoidGlassStonePrefab.SetActive(false);
        }

        public class VoidGlassGuonStoneController : MonoBehaviour
        {
            public PlayerOrbital self;
            public void Start()
            {
                self = GetComponent<PlayerOrbital>();
                
                SpeculativeRigidbody specRigidbody = self.specRigidbody;
                specRigidbody.OnPreRigidbodyCollision += this.OnPreCollision;
                foreach (PlayerController p in GameManager.Instance.AllPlayers)
                {
                    var GG = p.GetComponent<CorruptedWealthController>();
                    if (GG != null)
                    {
                        c = GG;
                    }
                }
            }

            private CorruptedWealthController c;

            private void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
            {
                
                PhysicsEngine.SkipCollision = true;
                RoomHandler currentRoom = GameManager.Instance.BestActivePlayer.CurrentRoom;
                AIActor component = otherRigidbody.GetComponent<AIActor>();
                if (component != null)
                {
                    if (component.healthHaver && component.CenterPosition.GetAbsoluteRoom() == currentRoom)
                    {
                        float M = 75 + c.StackCount * 25;
                        component.healthHaver.ApplyDamage(M * BraveTime.DeltaTime, Vector2.zero, "Void Glass", CoreDamageTypes.Void, DamageCategory.DamageOverTime, false, null, false);
                    }
                }
            }
        }


        public static GameObject VoidGlassStonePrefab;

        public static void SetupHeartVFX()
        {
            GameObject deathmark = ItemBuilder.AddSpriteToObject("corruptedHPVFX", "Planetside/Resources/Guons/PickupGuons/HeartGuon/heartguon_001", null);
            FakePrefab.MarkAsFakePrefab(deathmark);
            UnityEngine.Object.DontDestroyOnLoad(deathmark);
            tk2dSpriteAnimator animator = deathmark.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = deathmark.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(deathmark, ("Corrupted_Half_HP_Collection"));

            tk2dSpriteAnimationClip SpawnClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
            for (int i = 1; i < 5; i++)
            {
                tk2dSpriteCollectionData collection = DeathMarkcollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/Guons/PickupGuons/HeartGuon/heartguon_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            SpawnClip.frames = frames.ToArray();
            SpawnClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;

       
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { SpawnClip };
            animator.DefaultClipId = animator.GetClipIdByName("idle");
            animator.playAutomatically = true;




            deathmark.GetComponent<tk2dBaseSprite>().usesOverrideMaterial = true;
            Material mat = deathmark.GetComponent<tk2dBaseSprite>().renderer.material;
            mat.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            mat.SetFloat("_EmissivePower", 100);
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetColor("_OverrideColor", new Color(0.02f, 0.4f, 1, 0.6f));

            heartorbitalVFX = deathmark;
        }

        public static void SetupHalfHeartVFX()
        {
            GameObject deathmark = ItemBuilder.AddSpriteToObject("corruptedHPVFX_Half", "Planetside/Resources/Guons/PickupGuons/HalfheartGuon/halfheartguon_001", null);
            FakePrefab.MarkAsFakePrefab(deathmark);
            UnityEngine.Object.DontDestroyOnLoad(deathmark);
            tk2dSpriteAnimator animator = deathmark.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = deathmark.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(deathmark, ("Corrupted_Half_HP_Collection"));

            tk2dSpriteAnimationClip SpawnClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
            for (int i = 1; i < 5; i++)
            {
                tk2dSpriteCollectionData collection = DeathMarkcollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/Guons/PickupGuons/HalfheartGuon/halfheartguon_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            SpawnClip.frames = frames.ToArray();
            SpawnClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;

        
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { SpawnClip };
            animator.DefaultClipId = animator.GetClipIdByName("idle");
            animator.playAutomatically = true;


            deathmark.GetComponent<tk2dBaseSprite>().usesOverrideMaterial = true;
            Material mat = deathmark.GetComponent<tk2dBaseSprite>().renderer.material;
            mat.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            mat.SetFloat("_EmissivePower", 100);
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetColor("_OverrideColor", new Color(0.02f, 0.4f, 1, 0.6f));

            halfheartorbitalvfx = deathmark;
        }


        public static int CorruptedWealthID;
        private static Color OutlineColor;
        public static GameObject heartorbitalVFX;
        public static GameObject halfheartorbitalvfx;

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
            CorruptedWealthController blast = player.gameObject.GetOrAddComponent<CorruptedWealthController>();
            blast.player = player;
            if (blast.hasBeenPickedup == true)
            { blast.IncrementStack(); }

            //player.healthHaver.OnH

            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = blast.hasBeenPickedup == true ? "More pain, more gain." : "All pickups become corrupted.";
            OtherTools.Notify("Corrupted Wealth", BlurbText, "Planetside/Resources/PerkThings/corrputed_wealth", UINotificationController.NotificationColor.GOLD);
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
