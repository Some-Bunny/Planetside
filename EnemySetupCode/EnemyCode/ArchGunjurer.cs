using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using Pathfinding;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using static Planetside.ArchGunjurer.Attack_2;
using Alexandria;
using ChallengeAPI;
using static Planetside.Oppressor.HeartBurnOne;
using Alexandria.PrefabAPI;
using static UmbraController;
using Planetside.Controllers;

namespace Planetside
{
	public class ArchGunjurer : AIActor
	{

        public class RuneLockOnEffect : MonoBehaviour
        {
            float Offset = 0.25f;
            public float MyOffset = 0.25f;

            float Offset_2 = 0.25f;
            public float MyOffset_2 = 0.25f;

            public float DegreesRot_All;
            public float DegreesRot_Inner;
            public float DegreesRot_Inner_2;


            public tk2dSprite mainSprite;
            public List<tk2dSprite> tk2DSprites;
            public List<tk2dSprite> tk2DSprites_2;

            public Transform trans;
            public Transform trans_2;

            float e = 0;
            float e_ = 0;
            //public Vector2 LockInPos;

            public void SetState(bool State)
            {
                e_ = State ? 1 : 0;
            }

            public void SetToDestroy()
            {
                e_ = 0;
                willDie = true;
            }
            private bool willDie = false;
            public void Update()
            {
                //this.transform.position = LockInPos;
                e = Mathf.MoveTowards(e, e_, Time.deltaTime * 5);
                this.transform.localScale = Vector3.one * e;
                mainSprite.HeightOffGround = 20;
                if (willDie && e <= 0)
                {
                    Destroy(this.gameObject);
                }

                this.transform.Rotate(0, 0, DegreesRot_All * Time.deltaTime);
                if (trans)
                {
                    trans.transform.Rotate(0, 0, DegreesRot_Inner * Time.deltaTime);
                }
                if (trans_2)
                {
                    trans_2.transform.Rotate(0, 0, DegreesRot_Inner_2 * Time.deltaTime);
                }
                Offset = Mathf.MoveTowards(Offset, MyOffset, Time.deltaTime * 2);
                Offset_2 = Mathf.MoveTowards(Offset_2, MyOffset_2, Time.deltaTime * 2);

                MoveReticles(tk2DSprites, Offset);
                MoveReticles(tk2DSprites_2, Offset_2, true);

            }

            private void MoveReticles(List<tk2dSprite> _tk2DSprites, float _offset, bool v = false)
            {
                int slot = 0;
                if (_tk2DSprites == null) { return; }
                foreach (var entry in _tk2DSprites)
                {
                    Vector3 vector3 = Vector3.zero;
                    entry.HeightOffGround = 20;

                    switch (slot)
                    {
                        case 0:
                            if (v)
                            {
                                vector3 = new Vector2(0, -_offset);
                            }
                            else
                            {
                                vector3 = new Vector2(-_offset, -_offset);
                            }
                            break;
                        case 1:
                            if (v)
                            {
                                vector3 = new Vector2(0, _offset);
                            }
                            else
                            {
                                vector3 = new Vector2(-_offset, _offset);
                            }
                            break;
                        case 2:
                            if (v)
                            {
                                vector3 = new Vector2(_offset, 0);
                            }
                            else
                            {
                                vector3 = new Vector2(_offset, -_offset);
                            }
                            break;
                        case 3:
                            if (v)
                            {
                                vector3 = new Vector2(-_offset, 0);
                            }
                            else
                            {
                                vector3 = new Vector2(_offset, _offset);
                            }
                            break;
                    }
                    entry.transform.localPosition = vector3;
                    slot++;
                }
            }

        }

        public static GameObject prefab;
		public static readonly string guid = "arch_gunjurer";

        private static tk2dSprite CreateArchEffectSmall(GameObject parent, tk2dSpriteCollectionData data, string name, string SpriteName, bool flipX, bool flipY, Vector2 offset)
        {
            var UmbralLockEdge_BL = PrefabBuilder.BuildObject(name);
            var s = UmbralLockEdge_BL.AddComponent<tk2dSprite>();
            s.SetSprite(data, SpriteName);
            s.SortingOrder = 20;
            s.HeightOffGround = 20;
            s.FlipX = flipX;
            s.FlipY = flipY;
            s.gameObject.transform.SetParent(parent.transform, false);
            s.gameObject.transform.localPosition = offset;
            s.sprite.usesOverrideMaterial = true;

            var materialIKU = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
            materialIKU.mainTexture = data.material.mainTexture;
            materialIKU.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            materialIKU.SetFloat("_EmissivePower", 20);
            materialIKU.SetFloat("_EmissiveColorPower", 5);
            s.renderer.material = materialIKU;

            return s;
        }

        public static GameObject SmallRune;
        public static GameObject LargeRune;

        public static void Init()
		{
            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ArchGunjurerCollection").GetComponent<tk2dSpriteCollectionData>();

            Actions.PostDungeonTrueStart += (_) =>
			{
                ArchGunjurerController.RolledAttack = UnityEngine.Random.Range(0, 5);
            };

            SmallRune = PrefabBuilder.BuildObject("ArchGunjurerSmallRune");
            SmallRune.transform.localScale = Vector3.zero;
            var s = SmallRune.AddComponent<tk2dSprite>();
            s.SetSprite(Collection, "archgunjurer_rune_small_001");
            s.SortingOrder = 20;
            s.HeightOffGround = 20;
            s.sprite.usesOverrideMaterial = true;
            var materialIKU = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
            materialIKU.mainTexture = Collection.material.mainTexture;
            materialIKU.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            materialIKU.SetFloat("_EmissivePower", 20);
            materialIKU.SetFloat("_EmissiveColorPower", 5);
            s.renderer.material = materialIKU;

            var holder = PrefabBuilder.BuildObject("Holder");
            holder.transform.SetParent(SmallRune.transform, false);
            var lockOn = SmallRune.gameObject.AddComponent<RuneLockOnEffect>();
            lockOn.mainSprite = s;
            lockOn.trans = holder.transform;
            lockOn.tk2DSprites = new List<tk2dSprite>
            {
                CreateArchEffectSmall(holder, Collection, "rune1", "archgunjurer_rune_small_002", false, false, new Vector2(-0.25f, -0.25f)),
                CreateArchEffectSmall(holder, Collection, "rune2", "archgunjurer_rune_small_002", false, true, new Vector2(-0.25f, 0.25f)),
                CreateArchEffectSmall(holder, Collection, "rune3", "archgunjurer_rune_small_002", true, false, new Vector2(0.25f, -0.25f)),
                CreateArchEffectSmall(holder, Collection, "rune4", "archgunjurer_rune_small_002", true, true, new Vector2(0.25f, 0.25f)),
            };


            //============================================================
            //============================================================
            //============================================================

            LargeRune = PrefabBuilder.BuildObject("ArchGunjurerLargeRune");
            LargeRune.transform.localScale = Vector3.zero;

            s = LargeRune.AddComponent<tk2dSprite>();
            s.SetSprite(Collection, "archgunjurer_rune_large_003");
            s.SortingOrder = 20;
            s.HeightOffGround = 20;
            s.sprite.usesOverrideMaterial = true;
            materialIKU = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
            materialIKU.mainTexture = Collection.material.mainTexture;
            materialIKU.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            materialIKU.SetFloat("_EmissivePower", 12);
            materialIKU.SetFloat("_EmissiveColorPower", 12);
            s.renderer.material = materialIKU;


            holder = PrefabBuilder.BuildObject("Holder_1");
            holder.transform.SetParent(LargeRune.transform, false);
            lockOn = LargeRune.gameObject.AddComponent<RuneLockOnEffect>();
            lockOn.mainSprite = s;
            lockOn.trans = holder.transform;
            lockOn.tk2DSprites = new List<tk2dSprite>
            {
                CreateArchEffectSmall(holder, Collection, "rune1_1", "archgunjurer_rune_large_002", false, false, new Vector2(-0.25f, -0.25f)),
                CreateArchEffectSmall(holder, Collection, "rune2_1", "archgunjurer_rune_large_002", false, true, new Vector2(-0.25f, 0.25f)),
                CreateArchEffectSmall(holder, Collection, "rune3_1", "archgunjurer_rune_large_002", true, false, new Vector2(0.25f, -0.25f)),
                CreateArchEffectSmall(holder, Collection, "rune4_1", "archgunjurer_rune_large_002", true, true, new Vector2(0.25f, 0.25f)),
            };

            var holder_2 = PrefabBuilder.BuildObject("Holder_2");
            holder.transform.SetParent(LargeRune.transform, false);
            lockOn.mainSprite = s;
            lockOn.trans = holder.transform;
            lockOn.trans_2 = holder_2.transform;
            lockOn.tk2DSprites_2 = new List<tk2dSprite>
            {
                CreateArchEffectSmall(holder, Collection, "rune1_2", "archgunjurer_rune_large_004", false, false, new Vector2(0, -0.25f)),
                CreateArchEffectSmall(holder, Collection, "rune2_2", "archgunjurer_rune_large_004", false, true, new Vector2(0, 0.25f)),
                CreateArchEffectSmall(holder, Collection, "rune3_2", "archgunjurer_rune_large_005", true, false, new Vector2(0.25f, 0)),
                CreateArchEffectSmall(holder, Collection, "rune4_2", "archgunjurer_rune_large_005", false, false, new Vector2(-0.25f, 0)),
            };


            var h = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ArchGunjurerAnimation").GetComponent<tk2dSpriteAnimation>();
            var h1 = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ArchGunjurerAnimation_Face").GetComponent<tk2dSpriteAnimation>();
			DontDestroyOnLoad(h1);
			ArchGunjurerController.SecondaryLibrary = h1;

			if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
			{
				prefab = EnemyBuilder.BuildPrefabBundle("Arch Gunjurer", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(8, 9),false);
				var companion = prefab.AddComponent<ArchGunjurerController>();

                companion.gameObject.layer = 22;
                companion.sprite.SortingOrder = 2;


                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;

                companion.aiActor.knockbackDoer.weight = 200;
				companion.aiActor.MovementSpeed = 1.25f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = false;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(55f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.healthHaver.SetHealthMaximum(55f, null, false);
				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(0.875f, 0.25f), "shadowPos");
				companion.aiActor.SetIsFlying(true, "Gamemode: Creative", true, true);
				companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;

				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 1,
					ManualOffsetY = 3,
					ManualWidth = 24,
					ManualHeight = 45,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0
				});
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{

					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyHitBox,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
                    ManualOffsetX = 1,
                    ManualOffsetY = 3,
                    ManualWidth = 24,
                    ManualHeight = 45,
                    ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});


				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;


                aiAnimator.IdleAnimation = Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "idle", new string[] { "idle" }, new DirectionalAnimation.FlipType[1]);

                aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>() { };

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "death",
                    new string[] { "death" },
                    new DirectionalAnimation.FlipType[1]);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                       aiAnimator, "hand1",
                       new string[] { "hand1" },
                       new DirectionalAnimation.FlipType[1]);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                        aiAnimator, "hand2",
                        new string[] { "hand2" },
                        new DirectionalAnimation.FlipType[1]);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                        aiAnimator, "showhands",
                        new string[] { "showhands" },
                        new DirectionalAnimation.FlipType[1]);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                        aiAnimator, "hidehand",
                        new string[] { "hidehand" },
                        new DirectionalAnimation.FlipType[1]);


                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                        aiAnimator, "fadeout",
                        new string[] { "fadeout" },
                        new DirectionalAnimation.FlipType[1]);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(
                    aiAnimator, "fadein",
                    new string[] { "fadein" },
                    new DirectionalAnimation.FlipType[1]);


                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[] { DirectionalAnimation.FlipType.None }, DirectionalAnimation.DirectionType.Single);
                companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;



                EnemyToolbox.AddSoundsToAnimationFrame(h, "fadeout", new Dictionary<int, string>() { { 0, "Play_BOSS_doormimic_jump_01" }, { 6, "Play_ENM_wizard_summon_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h1, "fadeout", new Dictionary<int, string>() { { 0, "Play_BOSS_doormimic_jump_01" }, { 6, "Play_ENM_wizard_summon_01" } });


                EnemyToolbox.AddEventTriggersToAnimation(h, "fadeout", new Dictionary<int, string>() { { 3, "ParticleBurst" } });
                EnemyToolbox.AddEventTriggersToAnimation(h1, "fadeout", new Dictionary<int, string>() { { 3, "ParticleBurst" } });

                EnemyToolbox.AddSoundsToAnimationFrame(h, "fadein", new Dictionary<int, string>() { { 1, "Play_BOSS_doormimic_appear_01" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h1, "fadein", new Dictionary<int, string>() { { 1, "Play_BOSS_doormimic_appear_01" } });

                EnemyToolbox.AddEventTriggersToAnimation(h, "fadein", new Dictionary<int, string>() { { 8, "ParticleBurst" } });
                EnemyToolbox.AddEventTriggersToAnimation(h1, "fadein", new Dictionary<int, string>() { { 8, "ParticleBurst" } });

                EnemyToolbox.AddSoundsToAnimationFrame(h, "death", new Dictionary<int, string>() { { 1, "Play_VO_kali_death_01" }, { 6, "Play_Dodge" } });
                EnemyToolbox.AddSoundsToAnimationFrame(h1, "death", new Dictionary<int, string>() { { 1, "Play_VO_kali_death_01" }, { 6, "Play_Dodge" } });

                EnemyToolbox.AddEventTriggersToAnimation(h, "death", new Dictionary<int, string>() { { 3, "ParticleDeathBurst" }, { 4, "ParticleDeathBurst" }, { 5, "ParticleDeathBurst" }, });
                EnemyToolbox.AddEventTriggersToAnimation(h1, "death", new Dictionary<int, string>() { { 3, "ParticleDeathBurst" }, { 4, "ParticleDeathBurst" }, { 5, "ParticleDeathBurst" }, });

                var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;


				var shootpoint = new GameObject("fuck");
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = companion.sprite.WorldCenter;


				bs.TargetBehaviors = new List<TargetBehaviorBase>
				{
				new TargetPlayerBehavior
				{
					Radius = 35f,
					LineOfSight = true,
					ObjectPermanence = true,
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.25f,
				}
				};
				bs.AttackBehaviors = new List<AttackBehaviorBase>()
				{
					new AttackBehaviorGroup()
					{
						AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>()
						{
							new AttackBehaviorGroup.AttackGroupItem()
							{
								NickName = "0",
								Probability = 0,
								Behavior = new ShootBehavior() {
								ShootPoint = shootpoint,
								BulletScript = new CustomBulletScriptSelector(typeof(Attack_1)),
								LeadAmount = 0f,
								AttackCooldown = 1f,
								Cooldown = 2f,
								TellAnimation = "showhands",
								FireAnimation = "hand1",
								PostFireAnimation = "hidehand",
								RequiresLineOfSight = true,
								MultipleFireEvents = true,
								Uninterruptible = false,
							},
                            },

                            new AttackBehaviorGroup.AttackGroupItem()
                            {
                                NickName = "1",
                                Probability = 0,
                                Behavior = new ShootBehavior() {
                                ShootPoint = shootpoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(Attack_2)),
                                LeadAmount = 0f,
                                AttackCooldown = 1f,
                                Cooldown = 4f,
                                TellAnimation = "showhands",
                                FireAnimation = "hand2",
                                PostFireAnimation = "hidehand",
                                RequiresLineOfSight = false,
                                MultipleFireEvents = true,
                                Uninterruptible = false,
                            },
                            },
                            new AttackBehaviorGroup.AttackGroupItem()
                            {
                                NickName = "2",
                                Probability = 0,
                                Behavior = new ShootBehavior() {
                                ShootPoint = shootpoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(Attack_3)),
                                LeadAmount = 0f,
                                AttackCooldown = 1f,
                                Cooldown = 4f,
                                TellAnimation = "showhands",
                                FireAnimation = "hand1",
                                PostFireAnimation = "hidehand",
                                RequiresLineOfSight = false,
                                MultipleFireEvents = true,
                                Uninterruptible = false,
                            },
                            },
                            new AttackBehaviorGroup.AttackGroupItem()
                            {
                                NickName = "3",
                                Probability = 0,
                                Behavior = new ShootBehavior() {
                                ShootPoint = shootpoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(Attack_4)),
                                LeadAmount = 0f,
                                AttackCooldown = 1f,
                                Cooldown = 5f,
                                TellAnimation = "showhands",
                                FireAnimation = "hand2",
                                PostFireAnimation = "hidehand",
                                RequiresLineOfSight = false,
                                MultipleFireEvents = true,
                                Uninterruptible = false,
                            },
                            },
                            new AttackBehaviorGroup.AttackGroupItem()
                            {
                                NickName = "4",
                                Probability = 0,
                                Behavior = new ShootBehavior() {
                                ShootPoint = shootpoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(Attack_5)),
                                LeadAmount = 0f,
                                AttackCooldown = 1f,
                                Cooldown = 5f,
                                TellAnimation = "showhands",
                                FireAnimation = "hand1",
                                PostFireAnimation = "hidehand",
                                RequiresLineOfSight = false,
                                MultipleFireEvents = true,
                                Uninterruptible = false,
                            },
                            },
                        }
					},
					new TeleportBehavior()
					{

					AttackableDuringAnimation = true,
					AllowCrossRoomTeleportation = false,
					teleportRequiresTransparency = false,
					hasOutlinesDuringAnim = true,
					ManuallyDefineRoom = false,
					MaxHealthThreshold = 1f,
					StayOnScreen = true,
					AvoidWalls = true,
					GoneTime = 2f,
					OnlyTeleportIfPlayerUnreachable = false,
					MinDistanceFromPlayer = 4f,
					MaxDistanceFromPlayer = -1f,
					teleportInAnim = "fadein",
					teleportOutAnim = "fadeout",
					AttackCooldown = 1f,
					InitialCooldown = 0f,
					RequiresLineOfSight = false,
					roomMax = new Vector2(0,0),
					roomMin = new Vector2(0,0),
					teleportInBulletScript = new CustomBulletScriptSelector(typeof(Wail)),
					teleportOutBulletScript = new CustomBulletScriptSelector(typeof(Wail)),
					GlobalCooldown = 0.5f,
					Cooldown = 2f,
					
					CooldownVariance = 1f,
					InitialCooldownVariance = 0f,
					goneAttackBehavior = null,
					IsBlackPhantom = false,
					GroupName = null,
					GroupCooldown = 0f,
					MinRange = 0,
					Range = 0,
					MinHealthThreshold = 0,
					MaxUsages = 0,
					AccumulateHealthThresholds = true,
					targetAreaStyle = null,
					HealthThresholds = new float[0],
					MinWallDistance = 0,
					}
				};
				bs.MovementBehaviors = new List<MovementBehaviorBase>
				{
					new MoveErraticallyBehavior
					{
				    PointReachedPauseTime = 0.1f,
					PathInterval = 0.2f,
					PreventFiringWhileMoving = false,
					StayOnScreen = false,
					AvoidTarget = true,

					}
				};
				/*
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBeamBehavior() {
					//ShootPoint = m_CachedGunAttachPoint,

					firingTime = 3f,
					stopWhileFiring = true,
					//beam
					//BulletScript = new CustomBulletScriptSelector(typeof(SkellScript)),
					//LeadAmount = 0f,
					AttackCooldown = 5f,
					//InitialCooldown = 4f,
					RequiresLineOfSight = true,
					//StopDuring = ShootBehavior.StopType.Attack,
					//Uninterruptible = true,

				}
				};
				*/
				
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:arch_gunjurer", companion.aiActor);

                Material mat2 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat2.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
                mat2.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                companion.aiActor.sprite.renderer.material = mat2;

                SpriteBuilder.AddSpriteToCollection(Collection.GetSpriteDefinition(FoolMode.isFoolish ? "archgunjurernewface_idle_001" : "archgunjurernew_idle_001"), SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:arch_gunjurer";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = FoolMode.isFoolish ? "archgunjurernewface_idle_001" : "archgunjurernew_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>(FoolMode.isFoolish ? "archgunjurericonsilly" : "archgunjurericon");// ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\archgunjurericon.png");
                PlanetsideModule.Strings.Enemies.Set("#THE_JURER", "Arch Gunjurer");
				PlanetsideModule.Strings.Enemies.Set("#THE_JURER_SHORTDESC", "Master Of The Gun");
				PlanetsideModule.Strings.Enemies.Set("#THE_JURER_LONGDESC", "A very high ranking Gunjurer, they are able to create and cast bullets without the use of a wand. All Gungeoneers are advised to never, under any circumstance, give them a wand.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#THE_JURER";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#THE_JURER_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#THE_JURER_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:arch_gunjurer");
				EnemyDatabase.GetEntry("psog:arch_gunjurer").ForcedPositionInAmmonomicon = 58;
				EnemyDatabase.GetEntry("psog:arch_gunjurer").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:arch_gunjurer").isNormalEnemy = true;

                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("sweep"));

                Entry = StaticBulletEntries.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("1a4872dafdb34fd29fe8ac90bd2cea67").bulletBank.Bullets[0],
                    "BouncyGunjugs", "DNC", null, false);
				Entry.BulletObject.GetComponent<Projectile>().BulletScriptSettings.overrideMotion = false;
                companion.aiActor.bulletBank.Bullets.Add(Entry);

            }
        }
		private static AIBulletBank.Entry Entry;
		public class ArchGunjurerController : BraveBehaviour
		{
			public static tk2dSpriteAnimation SecondaryLibrary;
            public static int RolledAttack;
            public List<RuneLockOnEffect> runeLockOnEffects=new List<RuneLockOnEffect>();

			public void Update()
			{

			}

            public void KillAllRunes()
            {
                foreach (var entry in runeLockOnEffects)
                {
                    if (entry != null)
                    {
                        entry.SetToDestroy();
                    }
                }
                runeLockOnEffects.Clear();
            }

			private void Start()
			{
                base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;

                if (UnityEngine.Random.value < 0.005f | FoolMode.isFoolish)
				{
                    this.aiActor.spriteAnimator.library = SecondaryLibrary;
                    this.aiActor.spriteAnimator.Library = SecondaryLibrary;
				}
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
                    KillAllRunes();
                    AkSoundEngine.PostEvent("Play_ENM_wizardred_death_01", base.aiActor.gameObject);
				};
				
				int value = UnityEngine.Random.Range(0, 5);
				float v = UnityEngine.Random.value;
                int failsafe = 10;
                while (RolledAttack == value)
                {
                    if (failsafe <= 0) { break; }
                    failsafe--;
                    value = UnityEngine.Random.Range(0, 5);
                }

                var g = this.aiActor.behaviorSpeculator.AttackBehaviors[0] as AttackBehaviorGroup;
                foreach (var entry in g.AttackBehaviors)
                {
                    if (entry.NickName == (v > (FoolMode.isFoolish ? 0.25f : 0.04f) ? RolledAttack.ToString() : value.ToString()))
                    {
                        entry.Probability = 1; continue;
                    }
                    entry.Probability = 0;
                }
            }
            private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
            {

                if (clip.GetFrame(frameIdx).eventInfo.Contains("ParticleBurst"))
                {
                    for (int i = 0; i < 12; i++)
                    {
                        ParticleBase.EmitParticles("DarkMagics_BG", 1, new ParticleSystem.EmitParams()
                        {
                            position = BraveUtility.RandomVector2(this.aiActor.sprite.WorldBottomLeft, this.aiActor.sprite.WorldTopRight),
                            startLifetime = UnityEngine.Random.Range(2.5f, 3.75f),
                            startColor = new Color(1, 1, 1, 0.4f),
                            startSize = 0.1f
                        });
                    }
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("ParticleDeathBurst"))
                {
                    for (int i = 0; i < 6; i++)
                    {
                        ParticleBase.EmitParticles("DarkMagics_BG", 1, new ParticleSystem.EmitParams()
                        {
                            position = BraveUtility.RandomVector2(this.aiActor.sprite.WorldBottomLeft, this.aiActor.sprite.WorldTopRight),
                            startLifetime = UnityEngine.Random.Range(3, 6),
                            startColor = new Color(0, 0, 0, 0.4f),
                            startSize = 0.1f
                        });
                    }
                }
            }
        }

		public class Wail : Script 
		{
			public override IEnumerator Top()
			{
				AkSoundEngine.PostEvent("Play_BOSS_Rat_Kunai_Prep_01", this.BulletBank.aiActor.gameObject);
				for (int k = 0; k < 16; k++)
				{
					if (k % 4 == 0)
                    {
                        this.Fire(new Direction(22.5f * k, DirectionType.Aim, -1f), new Speed(2f, SpeedType.Absolute), new ReverseBullet());
                    }
                    this.Fire(new Direction(22.5f * k, DirectionType.Aim, -1f), new Speed(5f, SpeedType.Absolute), new ReverseBullet());
				}
				yield break;
			}
		}
		public class ReverseBullet : Bullet
		{
			public ReverseBullet() : base("reversible", false, false, false)
			{
				base.SuppressVfx = true;
			}

			public override IEnumerator Top()
			{
				float speed = this.Speed;
				yield return this.Wait(30);
				this.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
				yield return this.Wait(30);
				this.Direction += 180f;
				this.Projectile.spriteAnimator.Play();
				yield return this.Wait(45);
				this.ChangeSpeed(new Speed(speed * 3f, SpeedType.Absolute), 60);
				yield break;
			}
		}


		public class Attack_1 : Script
		{
			public override IEnumerator Top() 
			{
                var archGunjurer = this.BulletBank.aiActor.GetComponent<ArchGunjurerController>();
                var rune = base.BulletBank.aiActor.SmarterPlayEffectOnActor(LargeRune, Vector2.zero, true, true, false, false).GetComponent<RuneLockOnEffect>();
                rune.MyOffset = 0f;
                rune.MyOffset_2 = 0f;

                rune.DegreesRot_Inner = 120;
                rune.DegreesRot_Inner_2 = 120;

                rune.SetState(true);
                archGunjurer.runeLockOnEffects.Add(rune);

                base.PostWwiseEvent("Play_BOSS_RatMech_Wizard_Cast_01", null);
				yield return this.Wait(60);
                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = this.BulletBank.aiActor.sprite.WorldCenter,
                    startLifetime = 0.5f,
                    startColor = new Color(1, 0, 0, 0.25f),
                    startSize = 8f
                });
                foreach (var e in archGunjurer.runeLockOnEffects)
                {
                    e.MyOffset = 1;
                    e.MyOffset_2 = 1;

                }
                for (int i = 0; i < 3; i++)
				{
					base.PostWwiseEvent("Play_ENM_wizardred_appear_01", null);
					base.Fire(new Direction(120*i, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new WallBullet());
					yield return this.Wait(10);
				}
                foreach (var e in archGunjurer.runeLockOnEffects)
                {
                    e.MyOffset = 0.25f;
                    e.MyOffset_2 = 0.25f;

                }
                yield return this.Wait(120);
                archGunjurer.KillAllRunes();

                yield break;
			}
            public class WallBullet : Bullet
            {
                public WallBullet() : base("bigBullet", false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
                    yield return this.Wait(90);
                    this.Direction = base.AimDirection;
                    base.ChangeSpeed(new Speed(15f, SpeedType.Absolute), 60);
                    yield break;
                }
                public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
                    if (!preventSpawningProjectiles)
                    {
                        AkSoundEngine.PostEvent("Play_ENM_kali_burst_01", this.Projectile.gameObject);
                        float num = base.RandomAngle();
                        for (int i = 0; i < 8; i++)
                        {
                            base.Fire(new Direction((45 * i) + num, DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new Bullet("reversible"));
                        }
                    }
                }
            }
        }

		public class Attack_2 : Script
		{
			public override IEnumerator Top()
			{
                var archGunjurer = this.BulletBank.aiActor.GetComponent<ArchGunjurerController>();

                for (int i = 0; i < 6; i++)
                {
                    var rune = base.BulletBank.aiActor.SmarterPlayEffectOnActor(SmallRune, MathToolbox.GetUnitOnCircle(60 * i, 1.5f), true, true, false, false).GetComponent<RuneLockOnEffect>();
                    rune.MyOffset = 0f;
                    rune.DegreesRot_All = 120f * (i % 2 == 0 ? 1 : -1);
                    rune.SetState(true);
                    archGunjurer.runeLockOnEffects.Add(rune);
                }

                base.PostWwiseEvent("Play_BOSS_RatMech_Wizard_Cast_01", null);
				yield return this.Wait(75);
                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = this.BulletBank.aiActor.sprite.WorldCenter,
                    startLifetime = 0.5f,
                    startColor = new Color(1, 0, 0, 0.25f),
                    startSize = 8f
                });
                foreach (var e in archGunjurer.runeLockOnEffects)
                {
                    e.MyOffset = 0.25f;
                }
				for (int i = 0; i < 64; i++)
                {
					if (i % 4 == 0)
                    {
                        base.PostWwiseEvent("Play_ENM_ironmaiden_blast_01", null);
                    }
                    base.Fire(new Direction(BraveUtility.RandomAngle(), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(2, 5), SpeedType.Absolute), new SpeedChanger());
                    yield return this.Wait(3);
				}
                yield return this.Wait(15);
                archGunjurer.KillAllRunes();
                yield break;
			}

            public class SpeedChanger : Bullet
            {
                public SpeedChanger() : base("sweep", false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(1f, SpeedType.Absolute), 30);
                    yield return this.Wait(75);
                    base.ChangeSpeed(new Speed(12.5f, SpeedType.Absolute), 60);
                    yield break;
                }
            }
        }

        public class Attack_3 : Script
        {
            public override IEnumerator Top()
            {
                var archGunjurer = this.BulletBank.aiActor.GetComponent<ArchGunjurerController>();
                var rune = base.BulletBank.aiActor.SmarterPlayEffectOnActor(LargeRune, Vector2.zero, true, true, false, false).GetComponent<RuneLockOnEffect>();
                rune.MyOffset = 0f;
                rune.MyOffset_2 = 0f;
                rune.DegreesRot_Inner = 120;
                rune.DegreesRot_Inner_2 = -120;
                rune.SetState(true);
                archGunjurer.runeLockOnEffects.Add(rune);
                for (int i = 0; i < 2; i++)
                {
                    rune = base.BulletBank.aiActor.SmarterPlayEffectOnActor(SmallRune, MathToolbox.GetUnitOnCircle((180 * i)+90, 1.5f), true, true, false, false).GetComponent<RuneLockOnEffect>();
                    rune.MyOffset = 0f;
                    rune.DegreesRot_All = 120f * (i % 2 == 0 ? 1 : -1);
                    rune.SetState(true);
                    archGunjurer.runeLockOnEffects.Add(rune);
                }
                base.PostWwiseEvent("Play_BOSS_RatMech_Wizard_Cast_01", null);
                yield return this.Wait(60);
                base.PostWwiseEvent("Play_BOSS_mineflayer_ring_01", null);
                for (int i = 0; i < 4; i++)
				{
                    for (int f = -3; f < 4; f++)
                    {
                        base.Fire(new Direction((90 * i) + 15f * f, DirectionType.Aim, -1f), new Speed(4 + Mathf.Abs(f * 0.3f), SpeedType.Absolute), new Bullet("BouncyGunjugs"));
                    }
                }
                yield return this.Wait(45);
                archGunjurer.KillAllRunes();
                yield break;
            }
        }

        public class Attack_4 : Script
        {
            public float aimDirection { get; private set; }
            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_ENM_wizard_charge_01", null);
                var archGunjurer = this.BulletBank.aiActor.GetComponent<ArchGunjurerController>();

                for (int i = -1; i < 2; i++)
                {
                    var rune = base.BulletBank.aiActor.SmarterPlayEffectOnActor(LargeRune,new Vector3(-2 * i, 0), true, true, false, false).GetComponent<RuneLockOnEffect>();
                    rune.MyOffset = 0f;
                    rune.DegreesRot_Inner = 240 * i;
                    rune.SetState(true);
                    archGunjurer.runeLockOnEffects.Add(rune);
                }

                yield return this.Wait(75);
                base.PostWwiseEvent("Play_ENM_wizardred_shoot_02", null);
                base.PostWwiseEvent("Play_ENM_mummy_cast_01", null);

                //m_ENM_mummy_cast_01
                for (int i = 0; i < 6; i++)
				{
                    Vector2 vector = new Vector2(2, 0f).Rotate(30f + (60 * i));
                    Vector2 vector_2 = new Vector2(2, 0f).Rotate(90f + (60 * i));
                    this.FireExpandingLine(vector_2, vector, 8, true);
                }

                for (int i = 0; i < 6; i++)
                {
                    Vector2 vector = new Vector2(3.5f, 0f).Rotate((60 * i));
                    Vector2 vector_2 = new Vector2(3.5f, 0f).Rotate(60f + (60 * i));
                    this.FireExpandingLine(vector_2, vector, 10, false);
                }
                this.aimDirection = base.AimDirection;
                yield return base.Wait(30);
                float distanceToTarget = (this.BulletManager.PlayerPosition() - base.Position).magnitude;
                if (distanceToTarget > 4.5f)
                {
                    this.aimDirection = base.GetAimDirection(1f, 10f);
                }
                yield return this.Wait(60);
                archGunjurer.KillAllRunes();
                yield break;
            }
            private void FireExpandingLine(Vector2 start, Vector2 end, int numBullets, bool SpinLeft)
            {
                for (int i = 0; i < numBullets; i++)
                {
                    base.Fire(new ExpandingBullet(this, Vector2.Lerp(start, end, (float)i / ((float)numBullets - 1f)), SpinLeft));
                }
            }
            public class ExpandingBullet : Bullet
            {
                public ExpandingBullet(Attack_4 parent, Vector2 offset, bool s) : base("reversible", false, false, false)
                {
                    this.m_parent = parent;
                    this.m_offset = offset;
					Spin = s;
                }
				private bool Spin;

                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    Vector2 centerPosition = base.Position;
                    for (int i = 0; i < 30; i++)
                    {
                        base.UpdateVelocity();
                        centerPosition += this.Velocity / 60f;
                        Vector2 actualOffset =  Vector2.Lerp(Vector2.zero, this.m_offset, Easing.DoLerpT((float)i / 29f, Easing.Ease.OUT));
                        actualOffset = actualOffset.Rotate((Spin ? 1f : -1f) * (float)i);
                        base.Position = centerPosition + actualOffset;
                        yield return base.Wait(1);
                    }
					if (Projectile.spriteAnimator)
					{
						Projectile.spriteAnimator.Play();
                    }
                    this.Direction = this.m_parent.aimDirection;
                    for (float j = 0; j < 300; j++)
                    {
						this.Speed = Mathf.Lerp(0, Spin ? 17 : 12, Easing.DoLerpT(j / 120, Easing.Ease.IN));
                        base.UpdateVelocity();
                        centerPosition += this.Velocity / 60f;
                        base.Position = centerPosition + this.m_offset.Rotate((Spin ? 1f + j : -1f - j) + 30);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);

                    yield break;
                }

                private Attack_4 m_parent;
                private Vector2 m_offset;
            }
        }

		public class Attack_5 : Script
		{
			public override IEnumerator Top()
			{
                base.PostWwiseEvent("Play_BOSS_RatMech_Wizard_Cast_01", null);
                var archGunjurer = this.BulletBank.aiActor.GetComponent<ArchGunjurerController>();

                for (int i = -2; i < 3; i++)
                {
                    var rune = base.BulletBank.aiActor.SmarterPlayEffectOnActor(LargeRune, new Vector3(0, i), true, true, false, false).GetComponent<RuneLockOnEffect>();
                    rune.MyOffset = 0f;
                    rune.DegreesRot_Inner = 240 * i;
                    rune.SetState(true);
                    archGunjurer.runeLockOnEffects.Add(rune);
                }
                base.PostWwiseEvent("Play_BOSS_mineflayer_bong_01", null);
                yield return this.Wait(60);
                for (int i = 0; i < 5; i++)
                {
                    base.Fire(new Direction((72 * i) + 36f, DirectionType.Aim, -1f), new Speed(12f, SpeedType.Absolute), new WallBullet());
                }
                yield return this.Wait(30);
                archGunjurer.KillAllRunes();
                yield break;
			}
            public class WallBullet : Bullet
            {
                public WallBullet() : base("bigBullet", false, false, false)
                {
                }
                public override IEnumerator Top()
                {
					int a = 0;
					while (Projectile)
					{
						a++;
						if (a % 8 == 0)
						{
                            base.Fire(new Direction(90 + (a * 1), DirectionType.Relative, -1f), new Speed(0, SpeedType.Absolute), new SpeedUpBullet("sweep", 60));
                            base.Fire(new Direction(90 + (a * 1), DirectionType.Relative, -1f), new Speed(0, SpeedType.Absolute), new SpeedUpBullet("poundSmall", 66f));
                        }
                        yield return null;
					}
                    yield break;
                }
            }
            public class SpeedUpBullet : Bullet
            {
                public SpeedUpBullet(string Type, float Delay) : base(Type, false, false, false)
                {
					d = Delay;
                }
				private float d;
                public override IEnumerator Top()
                {
					yield return Wait(d);
					this.ChangeSpeed(new Brave.BulletScript.Speed(11), 90);
                    yield break;
                }
            }
        }
    }
}





