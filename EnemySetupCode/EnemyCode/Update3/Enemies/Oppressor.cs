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
using BreakAbleAPI;
using static Planetside.Nemesis;
using static Planetside.Tower;

namespace Planetside
{
	public class Oppressor : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "oppressor_psog";
		private static tk2dSpriteCollectionData OppressorCollection;

		public static void Init()
		{
            Oppressor.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("oppressor", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false, true);
				var companion = prefab.AddComponent<EnemyBehavior>();
				prefab.AddComponent<ForgottenEnemyComponent>();
                prefab.AddComponent<OppressorController>();

                

                companion.aiActor.knockbackDoer.weight = 300;
				companion.aiActor.MovementSpeed = 1.2f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;

				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;
				companion.aiActor.FallingProhibited =true;
				companion.aiActor.SetIsFlying(true, "I can fly", true, true);


				companion.aiActor.healthHaver.ForceSetCurrentHealth(88f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(1f, 0.5f), "shadowPos");


				companion.aiActor.healthHaver.SetHealthMaximum(88f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 6,
					ManualOffsetY = 17,
					ManualWidth = 21,
					ManualHeight = 34,
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
					ManualOffsetX = 6,
					ManualOffsetY = 17,
					ManualWidth = 21,
					ManualHeight = 34,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;

				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "idle",
					AnimNames = new string[] { "idle" },
					Flipped = new DirectionalAnimation.FlipType[1]
				};

			

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "pain", new string[] { "pain" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				//
				/*
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "charge_small", new string[] { "charge_small" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "charge_large", new string[] { "charge_large" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "fire", new string[] { "fire" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "firetwo", new string[] { "firetwo" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "precharge_small", new string[] { "precharge_small" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				*/
				//companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				//companion.aiActor.reinforceType = ReinforceType.SkipVfx;

				bool flag3 = OppressorCollection == null;
				if (flag3)
				{
                    OppressorCollection = SpriteBuilder.ConstructCollection(prefab, "CollectiveCollection");
					UnityEngine.Object.DontDestroyOnLoad(OppressorCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], OppressorCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, OppressorCollection, new List<int>
					{
					0,
					1,
					2,
					3,
					4,
					
					}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, OppressorCollection, new List<int>
                    {
                    5,
					6,
					6,
					7,
					7,
					8,
					8,
					9,
					9,
					10,
                    9,
                    10,
                    9,
                    10,
                    9,
                    10,
					9,
					8,
					7,
					6,
					5
                    }, "pain", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;

                    SpriteBuilder.AddAnimation(companion.spriteAnimator, OppressorCollection, new List<int>
					{
					11,
					11,
					11,
					12,
					12,
					12,
					13,
					13,
					14,
					14,
					15,
					16,
                    15,
                    16,
                    15,
                    16,
                    15,
                    16,
                    17,
					18,
					19,
					
					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					/*
					SpriteBuilder.AddAnimation(companion.spriteAnimator, OppressorCollection, new List<int>
					{
					31,
					32,
					33,
					34,
					35,
					36,
					37,
					38,
					39,
					40,
					41
					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
					*/
				}

                //Creationist.TrespassEnemyEngageDoerPortalless trespassEngager = companion.aiActor.gameObject.AddComponent<Creationist.TrespassEnemyEngageDoerPortalless>();


                //m_ENM_PhaseSpider_Weave_01
                /*
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_large", new Dictionary<int, string> { { 0, "PepsiRage" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_large", new Dictionary<int, string> { { 0, "Play_ENM_PhaseSpider_Weave_01" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "fire", new Dictionary<int, string> { { 0, "ORDER" } });

				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "precharge_small", new Dictionary<int, string> { { 2, "Play_BOSS_dragun_charge_01" } });

				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Surprise" }, { 6, "PepsiRage" } });

				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "charge_small", new Dictionary<int, string> { { 0, "PaPew" } });
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Play_VesselDeath" }, { 6, "Play_ENM_Tarnisher_Bite_01" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Surprise" }, {6, "PepsiRage" } });
				//m_ENM_blobulord_reform_01
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "awaken", new Dictionary<int, string> { { 5, "Play_ENM_blobulord_reform_01" } });

				GameObject shootpoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(1.5f, 2f), "CollectiveShootpoint");
				*/

				//arms

                {
                    GameObject vfxObj = ItemBuilder.AddSpriteToObject("Left_Hand", DefPath + $"arms/oppressor_leftarm_idle_001", null);
                    vfxObj.transform.parent = companion.gameObject.transform;
                    vfxObj.transform.position = new Vector3(-0.8125f, 0.75f);
                    tk2dSpriteAnimator animator = vfxObj.GetOrAddComponent<tk2dSpriteAnimator>();
                    tk2dSpriteAnimation animation = vfxObj.AddComponent<tk2dSpriteAnimation>();
                    AIAnimator aiAnimatorBody = vfxObj.AddComponent<AIAnimator>();

                    animator.sprite.usesOverrideMaterial = true;
                    Material Handmat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                    Handmat.mainTexture = animator.sprite.renderer.material.mainTexture;
                    Handmat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
                    Handmat.SetFloat("_EmissiveColorPower", 3f);
                    Handmat.SetFloat("_EmissivePower", 40);
                    animator.sprite.renderer.material = Handmat;

                    aiAnimatorBody.IdleAnimation = new DirectionalAnimation
                    {
                        Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
                        Flipped = new DirectionalAnimation.FlipType[2],
                        AnimNames = new string[]
                        {
                        "idle",
						"idle"
                        }
                    };
                    EnemyToolbox.AddNewDirectionAnimation(aiAnimatorBody, "die", new string[] { "die" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                    vfxObj.AddComponent<MeshFilter>();
                    vfxObj.AddComponent<MeshRenderer>();


                    tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(vfxObj, ("Oppressor_Left_Arm"));

                    tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
                    List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
                    for (int i = 1; i < 5; i++)
                    {
                        tk2dSpriteCollectionData collection = DeathMarkcollection;
                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection(DefPath + $"arms/oppressor_leftarm_idle_00{i}", collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                        frameDef.materialInst = Handmat;
                        frameDef.material = Handmat;
                        frameDef.materialId = 0;
                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                        frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }

                    tk2dSpriteAnimationClip deathClip = new tk2dSpriteAnimationClip() { name = "die", frames = new tk2dSpriteAnimationFrame[0], fps = 12 };
                    List<tk2dSpriteAnimationFrame> deathFrames = new List<tk2dSpriteAnimationFrame>();
                    for (int i = 1; i < 9; i++)
                    {
                        tk2dSpriteCollectionData collection = DeathMarkcollection;

                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection(DefPath + $"arms/oppressor_leftarm_die_00{i}", collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                        frameDef.materialInst = Handmat;
                        frameDef.material = Handmat;
                        frameDef.materialId = 0;
                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                        deathFrames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }


                    deathClip.frames = deathFrames.ToArray();
                    deathClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;

                    idleClip.frames = frames.ToArray();
                    idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;

                    animator.Library = animation;
                    animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip, deathClip };
                    animator.DefaultClipId = animator.GetClipIdByName("idle");
                    animator.playAutomatically = true;

                    AdvancedBodyPartController bodyPart = vfxObj.AddComponent<AdvancedBodyPartController>();
                    bodyPart.Name = "Left_Hand";

                    SpeculativeRigidbody body = vfxObj.AddComponent<SpeculativeRigidbody>();

                    body.CollideWithOthers = true;
                    body.CollideWithTileMap = false;

                    vfxObj.GetComponent<tk2dBaseSprite>().OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;

                    body.PixelColliders = new List<PixelCollider>();
                    body.PixelColliders.Add(new PixelCollider
                    {

                        ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                        CollisionLayer = CollisionLayer.EnemyHitBox,
                        IsTrigger = false,
                        BagleUseFirstFrameOnly = false,
                        SpecifyBagelFrame = string.Empty,
                        BagelColliderNumber = 0,
                        ManualOffsetX = 4,
                        ManualOffsetY = 4,
                        ManualWidth = 10,
                        ManualHeight = 16,
                        ManualDiameter = 0,
                        ManualLeftX = 0,
                        ManualLeftY = 0,
                        ManualRightX = 0,
                        ManualRightY = 0,

                    });
                    HealthHaver healthHaver = vfxObj.AddComponent<HealthHaver>();
                    healthHaver.SetHealthMaximum(35);
                    healthHaver.ForceSetCurrentHealth(35);
                    healthHaver.flashesOnDamage = true;
                    vfxObj.GetOrAddComponent<GameActor>();

                 


                    bodyPart.ownBody = body;
                    bodyPart.ownHealthHaver = healthHaver;
                    FakePrefab.MarkAsFakePrefab(vfxObj);
                    UnityEngine.Object.DontDestroyOnLoad(vfxObj);
                }

                {
                    GameObject vfxObj = ItemBuilder.AddSpriteToObject("Right_Hand", DefPath + $"arms/oppressor_rightarm_idle_001", null);
                    vfxObj.transform.parent = companion.gameObject.transform;
                    vfxObj.transform.position = new Vector3(1.375f, 0.75f);
                    tk2dSpriteAnimator animator = vfxObj.GetOrAddComponent<tk2dSpriteAnimator>();
                    tk2dSpriteAnimation animation = vfxObj.AddComponent<tk2dSpriteAnimation>();
                    AIAnimator aiAnimatorBody = vfxObj.AddComponent<AIAnimator>();

                    animator.sprite.usesOverrideMaterial = true;
                    Material Handmat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                    Handmat.mainTexture = animator.sprite.renderer.material.mainTexture;
                    Handmat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
                    Handmat.SetFloat("_EmissiveColorPower", 3f);
                    Handmat.SetFloat("_EmissivePower", 40);
                    animator.sprite.renderer.material = Handmat;

                    aiAnimatorBody.IdleAnimation = new DirectionalAnimation
                    {
                        Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
                        Flipped = new DirectionalAnimation.FlipType[2],
                        AnimNames = new string[]
                        {
                        "idle",
                        "idle"
                        }
                    };

                    EnemyToolbox.AddNewDirectionAnimation(aiAnimatorBody, "idle", new string[] { "idle" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                    EnemyToolbox.AddNewDirectionAnimation(aiAnimatorBody, "die", new string[] { "die" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                    vfxObj.AddComponent<MeshFilter>();
                    vfxObj.AddComponent<MeshRenderer>();


                    tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(vfxObj, ("Oppressor_Right_Arm"));
                    tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
                    List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
                    for (int i = 1; i < 5; i++)
                    {
                        tk2dSpriteCollectionData collection = DeathMarkcollection;
                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection(DefPath + $"arms/oppressor_rightarm_idle_00{i}", collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
						frameDef.materialInst = Handmat;
						frameDef.material = Handmat;
                        frameDef.materialId = 0;


                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                        frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }
                    idleClip.frames = frames.ToArray();
                    idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;


                    tk2dSpriteAnimationClip deathClip = new tk2dSpriteAnimationClip() { name = "die", frames = new tk2dSpriteAnimationFrame[0], fps = 12 };
                    List<tk2dSpriteAnimationFrame> deathFrames = new List<tk2dSpriteAnimationFrame>();
                    for (int i = 1; i < 9; i++)
                    {
                        tk2dSpriteCollectionData collection = DeathMarkcollection;
                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection(DefPath + $"arms/oppressor_rightarm_die_00{i}", collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];

                        frameDef.materialInst = Handmat;
                        frameDef.material = Handmat;
                        frameDef.materialId = 0;

                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                        deathFrames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }


                    deathClip.frames = deathFrames.ToArray();
                    deathClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;


                    animator.Library = animation;
                    animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip, deathClip };

                    animator.DefaultClipId = animator.GetClipIdByName("idle");
                    animator.playAutomatically = true;

                    AdvancedBodyPartController bodyPart = vfxObj.AddComponent<AdvancedBodyPartController>();
					bodyPart.Name = "Right_Hand";
                    SpeculativeRigidbody body = vfxObj.AddComponent<SpeculativeRigidbody>();

                    body.CollideWithOthers = true;
                    body.CollideWithTileMap = false;

                    vfxObj.GetComponent<tk2dBaseSprite>().OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;

                    body.PixelColliders = new List<PixelCollider>();
                    body.PixelColliders.Add(new PixelCollider
                    {

                        ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                        CollisionLayer = CollisionLayer.EnemyHitBox,
                        IsTrigger = false,
                        BagleUseFirstFrameOnly = false,
                        SpecifyBagelFrame = string.Empty,
                        BagelColliderNumber = 0,
                        ManualOffsetX = 4,
                        ManualOffsetY = 4,
                        ManualWidth = 10,
                        ManualHeight = 16,
                        ManualDiameter = 0,
                        ManualLeftX = 0,
                        ManualLeftY = 0,
                        ManualRightX = 0,
                        ManualRightY = 0,

                    });
                    HealthHaver healthHaver = vfxObj.AddComponent<HealthHaver>();
                    healthHaver.SetHealthMaximum(35);
                    healthHaver.ForceSetCurrentHealth(35);
                    healthHaver.flashesOnDamage = true;
                    vfxObj.GetOrAddComponent<GameActor>();

                  


                    bodyPart.ownBody = body;
                    bodyPart.ownHealthHaver = healthHaver;
                    FakePrefab.MarkAsFakePrefab(vfxObj);
                    UnityEngine.Object.DontDestroyOnLoad(vfxObj);
                }




				GameObject shootPoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0, 0), "center");


                var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
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
				bs.MovementBehaviors = new List<MovementBehaviorBase>
				{
				new SeekTargetBehavior
				{
					ExternalCooldownSource = true,
					StopWhenInRange = true,
					CustomRange = 7f,
					LineOfSight = false,
					ReturnToSpawn = false,
					SpawnTetherDistance = 0f,
					PathInterval = 0.125f,
					SpecifyRange = false,
					MinActiveRange = 0f,
					MaxActiveRange = 0f
				}
				};

				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
				{
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 10f,
                        Behavior = new ShootBehavior{
                        ShootPoint = shootPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Cannon)),
                        LeadAmount = 0f,
                        AttackCooldown = 1f,
                        Cooldown = 0f,
                        InitialCooldown = 2f,
                        ChargeTime = 0f,
                        RequiresLineOfSight = false,
                        MultipleFireEvents = false,
                        Uninterruptible = true,
                        }
                    },


                    new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
						Behavior = new SequentialAttackBehaviorGroup() {
						RunInClass = false,
						//OverrideCooldowns = new List<float>(){0.1f,0.5f,0.5f},
					    AttackBehaviors = new List<AttackBehaviorBase>()
						{
							
							new ShootBehavior()
							{
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(Minigun)),
                                LeadAmount = 0f,
                                AttackCooldown = 0.5f,
                                Cooldown = 0.5f,
                                InitialCooldown = 1f,
                                ChargeTime = 0f,
                                RequiresLineOfSight = false,
                                MultipleFireEvents = false,
                                Uninterruptible = true,
                                StopDuring = ShootBehavior.StopType.Attack
                            },
                            new ShootBehavior()
                            {
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveLeft)),
                                LeadAmount = 0f,
                                AttackCooldown = 0.25f,
                                Cooldown = 0.25f,
                                InitialCooldown = 0f,
                                ChargeTime = 0f,
                                RequiresLineOfSight = false,
                                MultipleFireEvents = false,
                                Uninterruptible = true,
                                StopDuring = ShootBehavior.StopType.Attack
                            },
                            new ShootBehavior()
                            {
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveRight)),
                                LeadAmount = 0f,
                                AttackCooldown = 3f,
                                Cooldown = 3f,
                                InitialCooldown = 0f,
                                ChargeTime = 0f,
                                RequiresLineOfSight = false,
                                MultipleFireEvents = false,
                                Uninterruptible = true,
                                StopDuring = ShootBehavior.StopType.Attack
                            },
                        }
						},

                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0.5f,
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        
                        //OverrideCooldowns = new List<float>(){0.1f ,0.1f,0.1f,0.1f,0.1f,0.1f },
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {
                            new ShootBehavior()
                            {
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveLeftHard)),
                                AttackCooldown = 0f,
                                Cooldown = 0.5f,
                                InitialCooldown = 0f,
                                ChargeTime = 0f,
                                RequiresLineOfSight = false,
                                MultipleFireEvents = false,
                                Uninterruptible = true,
                                StopDuring = ShootBehavior.StopType.Attack
                            },
                            new ShootBehavior()
                            {
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveRightHard)),
                                LeadAmount = 0f,
                                AttackCooldown = 0f,
                                Cooldown = 0.5f,
                                InitialCooldown = 0f,
                                ChargeTime = 0f,
                                RequiresLineOfSight = false,
                                MultipleFireEvents = false,
                                Uninterruptible = true,
                                StopDuring = ShootBehavior.StopType.Attack
                            },
                             new ShootBehavior()
                            {
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveLeftHard)),
                                LeadAmount = 0f,
                                AttackCooldown = 0f,
                                Cooldown = 0.5f,
                                InitialCooldown = 0f,
                                ChargeTime = 0f,
                                RequiresLineOfSight = false,
                                MultipleFireEvents = false,
                                Uninterruptible = true,
                                StopDuring = ShootBehavior.StopType.Attack
                            },
                            new ShootBehavior()
                            {
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveRightHard)),
                                LeadAmount = 0f,
                                AttackCooldown = 0f,
                                Cooldown = 0.5f,
                                InitialCooldown = 0f,
                                ChargeTime = 0f,
                                RequiresLineOfSight = false,
                                MultipleFireEvents = false,
                                Uninterruptible = true,
                                StopDuring = ShootBehavior.StopType.Attack
                            },
                             new ShootBehavior()
                            {
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveLeftHard)),
                                LeadAmount = 0f,
                                AttackCooldown = 0f,
                                Cooldown = 0.5f,
                                InitialCooldown = 0f,
                                ChargeTime = 0f,
                                RequiresLineOfSight = false,
                                MultipleFireEvents = false,
                                Uninterruptible = true,
                                StopDuring = ShootBehavior.StopType.Attack
                            },
                            new ShootBehavior()
                            {
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveRightHard)),
                                LeadAmount = 0f,
                                Cooldown = 4f,
                                AttackCooldown = 4,
                                InitialCooldown = 0f,
                                ChargeTime = 0f,
                                RequiresLineOfSight = false,
                                MultipleFireEvents = false,
                                Uninterruptible = true,
                                StopDuring = ShootBehavior.StopType.Attack
                            },
                        }
                        },

                    }
                };

                /*
                 * 
                 * 
				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
				{
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
						Behavior = new ShootBehavior{
						ShootPoint = shootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(BasicCreationistAttack)),
						LeadAmount = 0f,
						AttackCooldown = 1f,
						Cooldown = 4f,
						InitialCooldown = 3f,
						ChargeTime = 0.75f,
						RequiresLineOfSight = false,
						MultipleFireEvents = false,
						Uninterruptible = true,
						ChargeAnimation = "precharge_small",
						FireAnimation = "charge_small",
						PostFireAnimation = "firetwo",
						StopDuring = ShootBehavior.StopType.Attack
						}
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
						Behavior = new ShootBehavior{
						ShootPoint = shootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(CrowdControl)),
						LeadAmount = 0f,
						AttackCooldown = 2f,
						Cooldown = 4f,
						InitialCooldown = 0f,
						ChargeTime = 0f,
						RequiresLineOfSight = false,
						MultipleFireEvents = false,
						Uninterruptible = true,
						FireAnimation = "charge_large",
						PostFireAnimation = "fire",
						StopDuring = ShootBehavior.StopType.Attack
						}
					},
				};
				*/


                DebrisObject shoulder1 = BreakableAPIToolbox.GenerateDebrisObject(DefPath + "debris/oppressor_debris_001.png", true, 0.5f, 1, 140, 20, null, 0.9f, "Play_BOSS_lichA_crack_01", null, 0);
                DebrisObject shoulder2 = BreakableAPIToolbox.GenerateDebrisObject(DefPath + "debris/oppressor_debris_002.png", true, 0.5f, 1, 140, 20, null, 0.9f, "Play_BOSS_lichA_crack_01", null, 0);
                DebrisObject shoulder3 = BreakableAPIToolbox.GenerateDebrisObject(DefPath + "debris/oppressor_debris_003.png", true, 0.5f, 1, 140, 20, null, 0.9f, "Play_BOSS_lichA_crack_01", null, 0);
                DebrisObject shoulder4 = BreakableAPIToolbox.GenerateDebrisObject(DefPath + "debris/oppressor_debris_004.png", true, 0.5f, 0.5f, 240,60, null, 0.9f, null, null, 0);
                DebrisObject shoulder5 = BreakableAPIToolbox.GenerateDebrisObject(DefPath + "debris/oppressor_debris_005.png", true, 0.5f, 0.5f, 360, 100, null, 0.9f, null, null, 0);


                ShardCluster BONES = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shoulder1, shoulder2, shoulder3, shoulder4, shoulder5 }, 0.9f, 2f, 3, 6, 1f);

                SpawnShardsOnDeath BodyAndStuff = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
                BodyAndStuff.deathType = OnDeathBehavior.DeathType.Death;
                BodyAndStuff.breakStyle = MinorBreakable.BreakStyle.BURST;
                BodyAndStuff.verticalSpeed = 2f;
                BodyAndStuff.heightOffGround = 2f;
                BodyAndStuff.shardClusters = new ShardCluster[] { BONES };

                bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:oppressor", companion.aiActor);




				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Collective/collective_idle_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:oppressor";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Collective/collective_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\sheetcollectiveTrespass.png");
				PlanetsideModule.Strings.Enemies.Set("#OPPRESSOR", "Oppressor");
				PlanetsideModule.Strings.Enemies.Set("#OPPRESSOR_SHORTDESC", "Consciousness");
				PlanetsideModule.Strings.Enemies.Set("#OPPRESSOR_LONGDESC", "A mass of those affected by its influence. Unable to resist it by themselves, they merged together and try to overpower it through strength in numbers.\n\nThey never succeed.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#OPPRESSOR";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#OPPRESSOR_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#OPPRESSOR_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:oppressor");
				EnemyDatabase.GetEntry("psog:oppressor").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:oppressor").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:oppressor").isNormalEnemy = true;



				companion.aiActor.sprite.usesOverrideMaterial = true;
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
				mat.SetFloat("_EmissiveColorPower", 3f);
				mat.SetFloat("_EmissivePower", 40);
				companion.aiActor.sprite.renderer.material = mat;

				SpawnEnemyOnDeath spawnEnemy = companion.gameObject.AddComponent<SpawnEnemyOnDeath>();
				spawnEnemy.deathType = OnDeathBehavior.DeathType.DeathAnimTrigger;
				spawnEnemy.DoNormalReinforcement = false;
				spawnEnemy.spawnsCanDropLoot = false;
				spawnEnemy.spawnAnim = "idle";
				spawnEnemy.spawnRadius = 1.25f;
				spawnEnemy.minSpawnCount = 1;
				spawnEnemy.maxSpawnCount = 3;
				spawnEnemy.enemySelection = SpawnEnemyOnDeath.EnemySelection.Random;
				spawnEnemy.enemyGuidsToSpawn = new string[] { "unwilling", "unwilling", "unwilling", "unwilling" };
				spawnEnemy.triggerName = "spawnBaddies";
				spawnEnemy.spawnPosition = SpawnEnemyOnDeath.SpawnPosition.InsideRadius;

				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 7, "spawnBaddies" } });
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBouncyBatBullet);
				companion.aiActor.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().baseData.speed *= 1.2f;
				companion.aiActor.bulletBank.Bullets[0].BulletObject.GetComponent<BounceProjModifier>().numberOfBounces += 2;

				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);

                companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableDirectedfireSoundless);
                companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableQuickHoming);
                companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBig);
                companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBigBullet);

            }
        }
      





        public class Minigun : Script
		{
            public Vector2 ShootPositionLeft
            {
                get
                {
                    if (this.BulletBank.aiActor.GetComponent<OppressorController>().leftArm == null)
                    {
                        return new Vector2(-1, -1);
                    }
                    return this.BulletBank.aiActor.GetComponent<OppressorController>().leftArm.sprite.WorldBottomLeft + new Vector2(0.5f, 0.5f);
                }
            }
            public Vector2 ShootPositionRight
            {
                get
                {
                    if (this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm == null)
                    {
                        return new Vector2(-1, -1);
                    }

                    return this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm.sprite.WorldBottomRight - new Vector2(0.5f, 0.5f);
                }
            }




            protected override IEnumerator Top()
			{
				float h = 16;
                float f = 7.5f;

                if (ShootPositionLeft == new Vector2(-1, -1))
                {
                    h += 8;
                    f -= 1.5f;

                }
                if (ShootPositionRight == new Vector2(-1, -1))
				{
					h += 8;
                    f -= 1.5f;
                }
                for (int e = 0; e < h; e++)
				{
					if (ShootPositionLeft != new Vector2(-1, -1))
					{
                        base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPositionLeft), new Direction(UnityEngine.Random.Range(-45 + h, 45-h), DirectionType.Aim, -1f), new Speed(3, SpeedType.Absolute), new NormalBullet());
                        base.PostWwiseEvent("Play_BOSS_doormimic_flame_01");
                    }
                    if (ShootPositionRight != new Vector2(-1, -1))
                    {
                        base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPositionRight), new Direction(UnityEngine.Random.Range(-45+h, 45-h), DirectionType.Aim, -1f), new Speed(3, SpeedType.Absolute), new NormalBullet());
                        base.PostWwiseEvent("Play_BOSS_doormimic_flame_01");
                    }
                    yield return this.Wait(f);
				}
				
				yield break;
			}

            public class NormalBullet : Bullet
            {
                public NormalBullet() : base(StaticUndodgeableBulletEntries.UndodgeableDirectedfireSoundless.Name, false, false, false)
                {

                }
                protected override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 120);
                    yield break;
                }
            }
        }

        public class WaveLeft : Wave
        {
            public override Vector2 ShootPosition
            {
                get
                {
                    if (this.BulletBank.aiActor.GetComponent<OppressorController>().leftArm == null)
                    {
                        return new Vector2(-1, -1);
                    }
                    return this.BulletBank.aiActor.GetComponent<OppressorController>().leftArm.sprite.WorldBottomLeft + new Vector2(0.5f, 0.5f);
                }
            }

        }

        public class WaveLeftHard : Wave
        {
            public override Vector2 ShootPosition
            {
                get
                {
                    if (this.BulletBank.aiActor.GetComponent<OppressorController>().leftArm == null)
                    {
                        return new Vector2(-1, -1);
                    }
                    return this.BulletBank.aiActor.GetComponent<OppressorController>().leftArm.sprite.WorldBottomLeft + new Vector2(0.5f, 0.5f);
                }
            }
            public override bool IsHard
             {
                get
                {
                    return true;
                }
            }

        }
        public class WaveRight : Wave
        {
            public override Vector2 ShootPosition
            {
                get
                {
                    if (this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm == null)
                    {
                        return new Vector2(-1, -1);
                    }
                    if (this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm.sprite == null)
                    {
                        return new Vector2(-1, -1);
                    }

                    return this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm.sprite.WorldBottomRight - new Vector2(0.5f, 0.5f);
                }
            }
        }
        public class WaveRightHard : Wave
        {
            public override Vector2 ShootPosition
            {
                get
                {
                    if (this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm == null)
                    {
                        return new Vector2(-1, -1);
                    }
                    if (this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm.sprite == null)
                    {
                        return new Vector2(-1, -1);
                    }

                    return this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm.sprite.WorldBottomRight - new Vector2(0.5f, 0.5f);
                }
            }
            public override bool IsHard
            {
                get
                {
                    return true;
                }
            }
        }



        public class Wave : Script
        {

            public virtual Vector2 ShootPosition
            {
                get
                {
                    return new Vector2(-1, -1);
                }
            }
            public virtual bool IsHard
            {
                get
                {
                    return false;
                }
            }

            protected override IEnumerator Top()
            {
                float f = IsHard == true ? 30 : 20;
                if (ShootPosition != new Vector2(-1, -1))
                {
                    for (int e = -3; e < 4; e++)
                    {
                        base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPosition), new Direction(18 * e, DirectionType.Aim, -1f), new Speed(10, SpeedType.Absolute), new NormalBullet());
                        base.PostWwiseEvent("Play_BOSS_doormimic_flame_01");
                    }
                    if (IsHard == true)
                    {
                        for (int e = 0; e < 8; e++)
                        {
                            base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPosition), new Direction((18 * e) - 63f, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new NormalBullet());
                        }
                    }
                   
                    yield return this.Wait(f);
                }
                yield return this.Wait(10);

                yield break;
            }

            public class NormalBullet : Bullet
            {
                public NormalBullet() : base(StaticUndodgeableBulletEntries.UndodgeableDirectedfireSoundless.Name, false, false, false)
                {

                }
                protected override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(5f, SpeedType.Absolute), 90);
                    yield break;
                }
            }
        }

        public class Cannon : Script
        {

            public Vector2 ShootPositionLeft
            {
                get
                {
                    if (this.BulletBank.aiActor.GetComponent<OppressorController>().leftArm == null)
                    {
                        return new Vector2(-1, -1);
                    }
                    return this.BulletBank.aiActor.GetComponent<OppressorController>().leftArm.sprite.WorldBottomLeft + new Vector2(0.5f, 0.5f);
                }
            }
            public Vector2 ShootPositionRight
            {
                get
                {
                    if (this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm == null)
                    {
                        return new Vector2(-1, -1);
                    }

                    return this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm.sprite.WorldBottomRight + new Vector2(-0.5f, 0.5f);
                }
            }

            protected override IEnumerator Top()
            {
                if (ShootPositionLeft != new Vector2(-1,-1))
                {
                    base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPositionLeft), new Direction(0, DirectionType.Aim, -1f), new Speed(7, SpeedType.Absolute), new MegaBulletThatBreaks());
                }
                for (int e = 0; e < 3; e++)
                {
                    if (ShootPositionRight != new Vector2(-1, -1))
                    {
                        base.Fire(new Direction(UnityEngine.Random.Range(-20, 20), DirectionType.Aim, -1f), new Speed(0, SpeedType.Absolute), new UndodgeableBullshit());
                        base.Fire(new Direction(UnityEngine.Random.Range(-20, 20), DirectionType.Aim, -1f), new Speed(3, SpeedType.Absolute), new UndodgeableBullshit());
                        yield return this.Wait(90);
                    }
                }             
                yield return this.Wait(60);
                yield break;
            }

            public class MegaBulletThatBreaks : Bullet
            {
                public MegaBulletThatBreaks() : base(StaticUndodgeableBulletEntries.undodgeableBig.Name, false, false, false)
                {

                }
                protected override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 150);
                    yield return this.Wait(360);

                    for (int e = 0; e < 16; e++)
                    {
                        for (int l = 0; l < 3; l++)
                        {

                            base.Fire(new Direction(22.5f * e, DirectionType.Aim, -1f), new Speed(4+l, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name, 12, 60));
                            base.Fire(new Direction((22.5f * e) + 11.25f, DirectionType.Aim, -1f), new Speed(4+l, SpeedType.Absolute), new SpeedChangingBullet(StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name, 12, 180));
                        }
                    }
                    base.Vanish(false);
                    yield break;
                }
            }

            public class UndodgeableBullshit : Bullet
            {
                public UndodgeableBullshit() : base(StaticUndodgeableBulletEntries.undodgeableBigBullet.Name, false, false, false)
                {

                }
                protected override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(25f, SpeedType.Absolute), 150);
                    base.ChangeDirection(new Brave.BulletScript.Direction(UnityEngine.Random.Range(-10, 10), DirectionType.Relative), 120);
                    while (this.Projectile)
                    {
                        base.Fire(new Direction(BraveUtility.RandomAngle(), DirectionType.Aim, -1f), new Speed(1, SpeedType.Absolute), new Bastard());
                        yield return this.Wait(3);
                    }
                    yield break;
                }
            }
            public class Bastard : Bullet
            {
                public Bastard() : base(StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name, false, false, false)
                {

                }
                protected override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 90);
                    yield return this.Wait(210);
                    base.Vanish(false);
                    yield break;
                }

            }
        }




        private static string DefPath = "Planetside/Resources/Enemies/Oppressor/";

		private static string[] spritePathsArms = new string[]
		{
            DefPath+"arms/oppressor_leftarm_idle_001.png",//0
            DefPath+"arms/oppressor_leftarm_idle_002.png",
            DefPath+"arms/oppressor_leftarm_idle_003.png",
            DefPath+"arms/oppressor_leftarm_idle_004.png",//3

            DefPath+"arms/oppressor_rightarm_idle_001.png",//4
            DefPath+"arms/oppressor_rightarm_idle_002.png",
            DefPath+"arms/oppressor_rightarm_idle_003.png",
            DefPath+"arms/oppressor_rightarm_idle_004.png",//7

            DefPath+"arms/oppressor_leftarm_die_001.png",//8
            DefPath+"arms/oppressor_leftarm_die_002.png",
            DefPath+"arms/oppressor_leftarm_die_003.png",
            DefPath+"arms/oppressor_leftarm_die_004.png",
            DefPath+"arms/oppressor_leftarm_die_005.png",
            DefPath+"arms/oppressor_leftarm_die_006.png",
            DefPath+"arms/oppressor_leftarm_die_007.png",
            DefPath+"arms/oppressor_leftarm_die_008.png",//15

            DefPath+"arms/oppressor_rightarm_die_001.png",//16
            DefPath+"arms/oppressor_rightarm_die_002.png",
            DefPath+"arms/oppressor_rightarm_die_003.png",
            DefPath+"arms/oppressor_rightarm_die_004.png",
            DefPath+"arms/oppressor_rightarm_die_005.png",
            DefPath+"arms/oppressor_rightarm_die_006.png",
            DefPath+"arms/oppressor_rightarm_die_007.png",
            DefPath+"arms/oppressor_rightarm_die_008.png",//23

        };


        private static string[] spritePaths = new string[]
		{
			DefPath+"oppressor_idle_001.png",//0
            DefPath+"oppressor_idle_002.png",
            DefPath+"oppressor_idle_003.png",
            DefPath+"oppressor_idle_004.png",
            DefPath+"oppressor_idle_005.png",//4

            DefPath+"oppressor_hurt_001.png",//5
            DefPath+"oppressor_hurt_002.png",
            DefPath+"oppressor_hurt_003.png",
            DefPath+"oppressor_hurt_004.png",
            DefPath+"oppressor_hurt_005.png",
            DefPath+"oppressor_hurt_006.png",//10

            DefPath+"oppressor_die_001.png",//11
            DefPath+"oppressor_die_002.png",
            DefPath+"oppressor_die_003.png",
            DefPath+"oppressor_die_004.png",
            DefPath+"oppressor_die_005.png",
            DefPath+"oppressor_die_006.png",
            DefPath+"oppressor_die_007.png",
            DefPath+"oppressor_die_008.png",
            DefPath+"oppressor_die_009.png",//19

        };

		public class EnemyBehavior : BraveBehaviour
		{

			private RoomHandler m_StartRoom;

			public void Update()
			{
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				if (!base.aiActor.HasBeenEngaged)
				{
					CheckPlayerRoom();
				}
			}
			private void CheckPlayerRoom()
			{
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
				{
					GameManager.Instance.StartCoroutine(LateEngage());
				}
				else
				{
					base.aiActor.HasBeenEngaged = false;
				}
			}
			private IEnumerator LateEngage()
			{
				yield return new WaitForSeconds(0.5f);
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
				{
					base.aiActor.HasBeenEngaged = true;
				}
				yield break;
			}
			private void Start()
			{
				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>{};
			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{//Surprise
				if (clip.GetFrame(frameIdx).eventInfo.Contains("PepsiRage"))
                {
					StaticVFXStorage.BeholsterChargeUpVFXInverse.SpawnAtPosition(base.aiActor.transform.Find("CollectiveShootpoint").position);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Surprise"))
                {
					StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(base.aiActor.transform.Find("CollectiveShootpoint").position);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("ORDER"))
				{
					Exploder.DoDistortionWave(base.aiActor.transform.Find("CollectiveShootpoint").position, 10f, 0.1f, 10, 1f);

				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("PaPew"))
				{
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
					GameObject gameObject = GameObject.Instantiate(silencerVFX.gameObject, base.aiActor.transform.Find("CollectiveShootpoint").position, Quaternion.identity);
					Destroy(gameObject, 2.5f);
				}
			}
		}
	}
}





