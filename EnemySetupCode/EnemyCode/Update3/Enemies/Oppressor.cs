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
using static Planetside.PrisonerSecondSubPhaseController;
using Planetside.Static_Storage;

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

               
                companion.aiActor.knockbackDoer.weight = 1000;
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


				companion.aiActor.healthHaver.ForceSetCurrentHealth(95f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(1f, 0.5f), "shadowPos");


				companion.aiActor.healthHaver.SetHealthMaximum(95f, null, false);
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

                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "charge_basic", new string[] { "charge_basic" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "fire_basic", new string[] { "fire_basic" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "uncharge_basic", new string[] { "uncharge_basic" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);


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
                    26,
                    26,
                    27
                    }, "charge_basic", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
                    EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "charge_basic", new Dictionary<int, string> { { 0, "Play_PrisonerLaugh" } });

                    SpriteBuilder.AddAnimation(companion.spriteAnimator, OppressorCollection, new List<int>
                    {
                    20,
                    21,
                    22,
                    23,
                    24,
                    25
                    }, "fire_basic", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 11f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, OppressorCollection, new List<int>
                    {
                    27,
                    27,
                    26
                    }, "uncharge_basic", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, OppressorCollection, new List<int>
                    {
                    5,
					6,
					
					7,
					
					8,
					8,
					9,				
					10,
                    9,
                    10,
					9,
					8,
					7,
					6,
					5
                    }, "pain", tk2dSpriteAnimationClip.WrapMode.Once).fps = 4f;

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
                    16,//11
                    15,
                    16,//13
                    15,
                    16,
                    15,
                    17,
					18,
					19,
					
					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 0, "Play_VO_lichB_death_01" }, { 4, "Play_VO_lichB_death_01" } });
                    EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 10, "ploompy" }, });
                    EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string> { { 16, "megaDie" } });

                    SpriteBuilder.AddAnimation(companion.spriteAnimator, OppressorCollection, new List<int>
                    {
                    28,
                    29,
                    30,
                    31,
                    32,
                    33,
                    34,
                    35,
                    36,
                    37,
                    38,
                    39,
                    40

                    }, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
                    EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "awaken", new Dictionary<int, string> { { 0, "Play_ENM_blobulord_charge_01" },{ 5, "Play_PrisonerLaugh" } });

                    EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "awaken", new Dictionary<int, string> { { 0, "hide_hands" }, { 12, "show_hands" } });

                }

                tk2dSpriteAnimationClip awakenClip = prefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("awaken");
                float[] offsetsX = new float[] { -0.6875f, -0.6875f, -0.6875f, -0.6875f, -0.6875f, -0.6875f, -0.6875f, -0.6875f, -0.6875f, -0.6875f, -0.6875f, -0.6875f, -0.6875f, -0.6875f };
                float[] offsetsY = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
                for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < awakenClip.frames.Length; i++)
                {
                    int id = awakenClip.frames[i].spriteId;
                    awakenClip.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX[i];
                    awakenClip.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY[i];
                    awakenClip.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX[i];
                    awakenClip.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY[i];
                    awakenClip.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX[i];
                    awakenClip.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY[i];
                    awakenClip.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX[i];
                    awakenClip.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY[i];
                }


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

                    tk2dSpriteAnimationClip fireClip = new tk2dSpriteAnimationClip() { name = "fire", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
                    List<tk2dSpriteAnimationFrame> fireFrames = new List<tk2dSpriteAnimationFrame>();
                    for (int i = 1; i < 4; i++)
                    {
                        tk2dSpriteCollectionData collection = DeathMarkcollection;
                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection(DefPath + $"arms/oppressor_leftarm_fire_00{i}", collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];

                        frameDef.materialInst = Handmat;
                        frameDef.material = Handmat;
                        frameDef.materialId = 0;

                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                        fireFrames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }

                    fireClip.frames = fireFrames.ToArray();
                    fireClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;


                    deathClip.frames = deathFrames.ToArray();
                    deathClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;

                    idleClip.frames = frames.ToArray();
                    idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;

                    animator.Library = animation;
                    animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip, deathClip, fireClip };
                    animator.DefaultClipId = animator.GetClipIdByName("idle");
                    animator.playAutomatically = true;

                    EnemyToolbox.AddSoundsToAnimationFrame(animator, "die", new Dictionary<int, string>() { {1, "Play_Big_Break" } });

                    AdvancedBodyPartController bodyPart = vfxObj.AddComponent<AdvancedBodyPartController>();
                    bodyPart.Name = "Left_Hand";
                    bodyPart.Render = false;
                    bodyPart.renderer.enabled = false;
                    bodyPart.gameObject.transform.localScale *= 0;

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

                    tk2dSpriteAnimationClip fireClip = new tk2dSpriteAnimationClip() { name = "fire", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
                    List<tk2dSpriteAnimationFrame> fireFrames = new List<tk2dSpriteAnimationFrame>();
                    for (int i = 1; i < 4; i++)
                    {
                        tk2dSpriteCollectionData collection = DeathMarkcollection;
                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection(DefPath + $"arms/oppressor_rightarm_fire_00{i}", collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];

                        frameDef.materialInst = Handmat;
                        frameDef.material = Handmat;
                        frameDef.materialId = 0;

                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                        fireFrames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }

                    fireClip.frames = fireFrames.ToArray();
                    fireClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;

                    deathClip.frames = deathFrames.ToArray();
                    deathClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;


                    animator.Library = animation;
                    animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip, deathClip, fireClip };

                    animator.DefaultClipId = animator.GetClipIdByName("idle");
                    animator.playAutomatically = true;

                    AdvancedBodyPartController bodyPart = vfxObj.AddComponent<AdvancedBodyPartController>();
					bodyPart.Name = "Right_Hand";
                    bodyPart.Render = false;
                    bodyPart.renderer.enabled = false;
                    bodyPart.gameObject.transform.localScale *= 0;

                    SpeculativeRigidbody body = vfxObj.AddComponent<SpeculativeRigidbody>();
                    EnemyToolbox.AddSoundsToAnimationFrame(animator, "die", new Dictionary<int, string>() { { 1, "Play_Big_Break" } });

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




				GameObject shootPoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.8125f, 1.4375f), "center");


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
                        NickName = "Heart_Burn_01",
                        Probability = 0f,
                        Behavior = new ShootBehavior{
                        ShootPoint = shootPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(HeartBurnOne)),
                        LeadAmount = 0f,
                        AttackCooldown = 1f,
                        Cooldown = 3,
                        InitialCooldown = 0f,
                        ChargeTime = 0f,
                        RequiresLineOfSight = false,
                        MultipleFireEvents = false,
                        Uninterruptible = true,
                        }
                    },
                     new AttackBehaviorGroup.AttackGroupItem()
                    {
                        NickName = "Heart_Burn_02",
                        Probability = 0f,
                        Behavior = new ShootBehavior{
                        ShootPoint = shootPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(HeartBurn)),
                        LeadAmount = 0f,
                        AttackCooldown = 1f,
                        Cooldown = 3,
                        InitialCooldown = 0f,
                        ChargeTime = 0f,
                        RequiresLineOfSight = false,
                        MultipleFireEvents = false,
                        Uninterruptible = true,
                        }
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        NickName = "Heart_Burn_03",
                        Probability = 0f,
                        Behavior = new ShootBehavior{
                        ShootPoint = shootPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(HeartBurnTwo)),
                        LeadAmount = 0f,
                        AttackCooldown = 1f,
                        Cooldown = 3,
                        InitialCooldown = 0f,
                        ChargeTime = 0.4f,
                        RequiresLineOfSight = false,
                        MultipleFireEvents = false,
                        Uninterruptible = true,
                        ChargeAnimation = "charge_basic",
                        FireAnimation = "fire_basic",
                        PostFireAnimation = "uncharge_basic"
                        }
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 2f,
                        NickName = "Def1",
                        Behavior = new ShootBehavior{
                        ShootPoint = shootPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Cannon)),
                        LeadAmount = 0f,
                        AttackCooldown = 2f,
                        Cooldown = 4,
                        InitialCooldown = 1f,
                        ChargeTime = 0f,
                        RequiresLineOfSight = false,
                        MultipleFireEvents = false,
                        Uninterruptible = true,
                        }
                    },

                new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
                        NickName = "Def2",
                        Behavior = new SequentialAttackBehaviorGroup() {
						RunInClass = false,
					    AttackBehaviors = new List<AttackBehaviorBase>()
						{
							
							new ShootBehavior()
							{
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(Minigun)),
                                LeadAmount = 0f,
                                AttackCooldown = 0.1f,
                                Cooldown = 0f,
                                InitialCooldown = 1f,
                                ChargeTime = 0.5f,
                                RequiresLineOfSight = false,
                                MultipleFireEvents = false,
                                Uninterruptible = true,
                                StopDuring = ShootBehavior.StopType.Attack,
                                ChargeAnimation = "charge_basic",
                                FireAnimation = "fire_basic"
                            },
                            new ShootBehavior()
                            {
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveLeft)),
                                LeadAmount = 0f,
                                AttackCooldown = 0.1f,
                                Cooldown = 0.1f,
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
                         NickName = "Def3",
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {
                            new ShootBehavior()
                            {
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveLeftHard)),
                                AttackCooldown = 0f,
                                Cooldown = 0.25f,
                                InitialCooldown = 0f,
                                ChargeTime = 0f,
                                RequiresLineOfSight = false,
                                MultipleFireEvents = false,
                                Uninterruptible = true,
                                StopDuring = ShootBehavior.StopType.Attack,
                                
                            },
                            new ShootBehavior()
                            {
                                ShootPoint = shootPoint,
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveRightHard)),
                                LeadAmount = 0f,
                                AttackCooldown = 0f,
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
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveLeftHard)),
                                LeadAmount = 0f,
                                AttackCooldown = 0f,
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
                                BulletScript = new CustomBulletScriptSelector(typeof(WaveRightHard)),
                                LeadAmount = 0f,
                                AttackCooldown = 2f,
                                Cooldown = 4f,
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


                companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
                companion.aiActor.reinforceType = ReinforceType.SkipVfx;
                Creationist.TrespassEnemyEngageDoer trespassEngager = companion.aiActor.gameObject.AddComponent<Creationist.TrespassEnemyEngageDoer>();
                trespassEngager.PortalLifeTime = 6;
                trespassEngager.PortalSize = 0.35f;


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

				SpriteBuilder.AddSpriteToCollection(DefPath + "oppressor_awaken_012.png", SpriteBuilder.ammonomiconCollection);
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
				companion.encounterTrackable.journalData.AmmonomiconSprite = DefPath + "oppressor_awaken_012";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("sheetOppressorTrespass");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\sheetOppressorTrespass.png");
                PlanetsideModule.Strings.Enemies.Set("#OPPRESSOR", "Oppressor");
				PlanetsideModule.Strings.Enemies.Set("#OPPRESSOR_SHORTDESC", "Overpowered");
				PlanetsideModule.Strings.Enemies.Set("#OPPRESSOR_LONGDESC", "The strongest of those who turned are those with the most determination.\n\nAnd they are determined to end you.");
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

			
				companion.aiActor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableBouncyBatBullet);
				companion.aiActor.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().baseData.speed *= 1.2f;
				companion.aiActor.bulletBank.Bullets[0].BulletObject.GetComponent<BounceProjModifier>().numberOfBounces += 2;

				companion.aiActor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableLargeSpore);
				companion.aiActor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableSmallSpore);

                companion.aiActor.bulletBank.Bullets.Add(StaticBulletEntries.UndodgeableDirectedfireSoundless);
                companion.aiActor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableQuickHoming);
                companion.aiActor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableBig);
                companion.aiActor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableBigBullet);

                companion.aiActor.bulletBank.Bullets.Add(StaticBulletEntries.undodgeableMineflayerBounce);

                companion.aiActor.bulletBank.Bullets.Add(StaticBulletEntries.UndodgeableDoorLordBurst);

            }
        }


        public class Obliterate : Script
        {
            public override IEnumerator Top()
            {
                for (int i = 0; i < 15; i++)
                {

                    for (int e = 0; e < 5; e++)
                    {
                        base.Fire(new Direction((24*i), DirectionType.Aim, -1f), new Speed(1+e, SpeedType.Absolute), new Creep());
                    }
              
                }
                yield break;
            }
            public class Creep : Bullet
            {
                public Creep() : base(StaticBulletEntries.undodgeableMineflayerBounce.Name, false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    base.ChangeSpeed(new Brave.BulletScript.Speed(11, SpeedType.Absolute), 180);
                    for (int i = 0; i < 120; i++)
                    {
                        float aim = base.GetAimDirection(1f, 16f);
                        float delta = BraveMathCollege.ClampAngle180(aim - this.Direction);

                        this.Direction += Mathf.MoveTowards(0f, delta, 0.1f);
                        yield return base.Wait(1);
                    }
                    yield break;
                }
            }
        }


        public class HeartBurnOne : Script
        {
          
            public override IEnumerator Top()
            {
                bool b = BraveUtility.RandomBool();
                for (int e = 0; e < 8; e++)
                {
                    base.Fire(new Direction(45*e, DirectionType.Aim, -1f), new Speed(3, SpeedType.Absolute), new Spin(b));
                }
                yield return this.Wait(40);
                b = !b;
                for (int e = 0; e < 10; e++)
                {
                    base.Fire(new Direction(36 * e, DirectionType.Aim, -1f), new Speed(3, SpeedType.Absolute), new Spin(b));
                }
                yield return this.Wait(40);
                b = !b;
                for (int e = 0; e < 12; e++)
                {
                    base.Fire(new Direction(30 * e, DirectionType.Aim, -1f), new Speed(3, SpeedType.Absolute), new Spin(b));
                }
                yield break;
            }
            public class Spin : Bullet
            {
                public Spin(bool a) : base(StaticBulletEntries.UndodgeableDoorLordBurst.Name, false, false, false)
                {
                    b = a;
                }
                public override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(25f, SpeedType.Absolute), 600);
                    base.ChangeDirection(new Direction(b == true ? 300 : -300, DirectionType.Relative), 90);

                    yield break;
                }
                private bool b;
            }
        }




        public class HeartBurn : Script
        {
            public override IEnumerator Top()
            {
                for (int i = 0; i < 15; i++)
                {
                    base.PostWwiseEvent("Play_Strafe_Shot");
                    for (int e = 0; e < 6; e++)
                    {
                        base.Fire(new Direction((60 * e)+ (24*i), DirectionType.Absolute, -1f), new Speed(3, SpeedType.Absolute), new SpeedChangingBullet(StaticBulletEntries.undodgeableSmallSpore.Name, 13, 120));

                        GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(228) as Gun).muzzleFlashEffects.effects[0].effects[0].effect, true);
                        vfx.transform.position = this.Position;
                        vfx.transform.localRotation = Quaternion.Euler(0f, 0f, (45 * e) + (24 * i));
                        vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                        Destroy(vfx, 1);
                    }
                    yield return this.Wait(8);
                }

                yield break;
            }
        }

        public class HeartBurnTwo : Script
        {
            public override IEnumerator Top()
            {
                for (int i = 0; i < 24; i++)
                {
                    base.PostWwiseEvent("Play_Vertebreak_Shot");

                    float a = BraveUtility.RandomAngle();
                    GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(228) as Gun).muzzleFlashEffects.effects[0].effects[0].effect, true);
                    vfx.transform.position = this.Position;
                    vfx.transform.localRotation = Quaternion.Euler(0f, 0f, a);
                    vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                    Destroy(vfx, 1);
                    base.Fire(new Direction(a, DirectionType.Aim, -1f), new Speed(3, SpeedType.Absolute), new Creep());
                    yield return this.Wait(5);
                }
                yield break;
            }
            public class Creep : Bullet
            {
                public Creep() : base(StaticBulletEntries.undodgeableMineflayerBounce.Name, false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    base.ChangeSpeed(new Brave.BulletScript.Speed(11, SpeedType.Absolute), 180);
                    for (int i = 0; i < 120; i++)
                    {
                        float aim = base.GetAimDirection(1f, 16f);
                        float delta = BraveMathCollege.ClampAngle180(aim - this.Direction);
                       
                        this.Direction += Mathf.MoveTowards(0f, delta, 0.8f);
                        yield return base.Wait(1);
                    }
                    yield break;
                }
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

                    return this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm.sprite.WorldBottomRight + new Vector2(-0.5f, 0.5f);
                }
            }




            public override IEnumerator Top()
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
                        float F = Vector2.left.ToAngle();
                        base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPositionLeft), new Direction(F +UnityEngine.Random.Range(-75 + h, 75-h), DirectionType.Absolute, -1f), new Speed(3, SpeedType.Absolute), new NormalBullet());
                        base.PostWwiseEvent("Play_BOSS_doormimic_flame_01");
                        if (e % 4 == 0)
                        {
                            float asf = BraveUtility.RandomAngle();
                            for (int l = 0; l < 3; l++)
                            {
                                base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPositionLeft), new Direction((120 * l) + asf, DirectionType.Aim, -1f), new Speed(0, SpeedType.Absolute), new NormalBullet());
                            }
                        }
                    }
                    if (ShootPositionRight != new Vector2(-1, -1))
                    {
                        float F = Vector2.right.ToAngle();
                        base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPositionRight), new Direction(F + UnityEngine.Random.Range(-75+h, 75-h), DirectionType.Absolute, -1f), new Speed(3, SpeedType.Absolute), new NormalBullet());
                        if (e % 4 == 0)
                        {
                            float asf = BraveUtility.RandomAngle();
                            for (int l = 0; l < 3; l++)
                            {
                                base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPositionRight), new Direction((120 * l)+ asf, DirectionType.Aim, -1f), new Speed(0, SpeedType.Absolute), new NormalBullet());
                            }
                        }

                        base.PostWwiseEvent("Play_BOSS_doormimic_flame_01");
                    }
                    yield return this.Wait(f);
				}
				
				yield break;
			}

            public class NormalBullet : Bullet
            {
                public NormalBullet() : base(StaticBulletEntries.UndodgeableDirectedfireSoundless.Name, false, false, false)
                {

                }
                public override IEnumerator Top()
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
            public override AdvancedBodyPartController part
            {
                get
                {
                    return this.BulletBank.aiActor.GetComponent<OppressorController>().leftArm;
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
            public override AdvancedBodyPartController part
            {
                get
                {
                    return this.BulletBank.aiActor.GetComponent<OppressorController>().leftArm;
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

                    return this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm.sprite.WorldBottomRight + new Vector2(-0.5f, 0.5f);
                }
            }
            public override AdvancedBodyPartController part
            {
                get
                {
                    return this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm;
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

                    return this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm.sprite.WorldBottomRight + new Vector2(-0.5f, 0.5f);
                }
            }
            public override bool IsHard
            {
                get
                {
                    return true;
                }
            }
            public override AdvancedBodyPartController part
            {
                get
                {
                    return this.BulletBank.aiActor.GetComponent<OppressorController>().rightArm;
                }
            }
        }



        public class Wave : Script
        {

            public virtual AdvancedBodyPartController part
            {
                get
                {
                    return null;
                }
            }

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

            public override IEnumerator Top()
            {
                float f = IsHard == true ? 40 : 20;
                if (ShootPosition != new Vector2(-1, -1))
                {

                    GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(365) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects.First().effects.First().effect, true);
                    vfx.transform.position = ShootPosition;
                    vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                    Destroy(vfx, 1);

                    base.PostWwiseEvent("Play_WPN_looper_shot_01");
                    for (int e = 0; e < 12; e++)
                    {
                        base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPosition), new Direction(30 * e, DirectionType.Aim, -1f), new Speed(10, SpeedType.Absolute), new NormalBullet());
                       
                    }
                    if (IsHard == true)
                    {
                        for (int e = -2; e < 3; e++)
                        {
                            base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPosition), new Direction((20 * e), DirectionType.Aim, -1f), new Speed(4f, SpeedType.Absolute), new NormalBullet());
                        }
                    }
                    if (part != null)
                    {
                        part.spriteAnimator.Play("fire");
                    }
                    yield return this.Wait(f);
                }
                yield return this.Wait(10);

                yield break;
            }

            public class NormalBullet : Bullet
            {
                public NormalBullet() : base(StaticBulletEntries.UndodgeableDirectedfireSoundless.Name, false, false, false)
                {

                }
                public override IEnumerator Top()
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

            public override IEnumerator Top()
            {
                if (ShootPositionLeft != new Vector2(-1,-1))
                {
                    base.PostWwiseEvent("Play_ENM_kali_shockwave_01");

                    GameObject gameObject = GameObject.Instantiate((GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost"), ShootPositionLeft, Quaternion.identity);
                    Destroy(gameObject, 2f);
                    base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPositionLeft), new Direction(0, DirectionType.Aim, -1f), new Speed(7, SpeedType.Absolute), new MegaBulletThatBreaks());
                }
                for (int e = 0; e < 3; e++)
                {
                    if (ShootPositionRight != new Vector2(-1, -1))
                    {
                        GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(365) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects.First().effects.First().effect, true);
                        vfx.transform.position = ShootPositionRight;
                        vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                        Destroy(vfx, 1);

                        base.PostWwiseEvent("Play_ENM_kali_burst_01");

                        base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPositionRight), new Direction(UnityEngine.Random.Range(-20, 20), DirectionType.Aim, -1f), new Speed(0, SpeedType.Absolute), new UndodgeableBullshit(120));
                        base.Fire(Brave.BulletScript.Offset.OverridePosition(ShootPositionRight), new Direction(UnityEngine.Random.Range(-20, 20), DirectionType.Aim, -1f), new Speed(2, SpeedType.Absolute), new UndodgeableBullshit(300));
                        yield return this.Wait(90);
                    }
                }             
                yield return this.Wait(60);
                yield break;
            }

            public class MegaBulletThatBreaks : Bullet
            {
                public MegaBulletThatBreaks() : base(StaticBulletEntries.undodgeableBig.Name, false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 150);
                    yield return this.Wait(360);
                    
                    base.PostWwiseEvent("Play_OBJ_nuke_blast_01");
                    for (int e = 0; e < 12; e++)
                    {
                        for (int l = 0; l < 3; l++)
                        {

                            base.Fire(new Direction(30f * e, DirectionType.Aim, -1f), new Speed(4+l, SpeedType.Absolute), new SpeedChangingBullet(StaticBulletEntries.undodgeableSmallSpore.Name, 12, 60));
                            base.Fire(new Direction((30f * e) + 15f, DirectionType.Aim, -1f), new Speed(4+l, SpeedType.Absolute), new SpeedChangingBullet(StaticBulletEntries.undodgeableLargeSpore.Name, 12, 180));
                        }
                    }
                    base.Vanish(false);
                    yield break;
                }
            }

            public class UndodgeableBullshit : Bullet
            {
                public UndodgeableBullshit(int s) : base(StaticBulletEntries.undodgeableBigBullet.Name, false, false, false)
                {
                    speedUp = s;
                }
                public override IEnumerator Top()
                {
                    base.ChangeSpeed(new Speed(25f, SpeedType.Absolute), speedUp);
                    base.ChangeDirection(new Brave.BulletScript.Direction(UnityEngine.Random.Range(-10, 10), DirectionType.Relative), 120);
                    while (this.Projectile)
                    {
                        base.Fire(new Direction(BraveUtility.RandomAngle(), DirectionType.Aim, -1f), new Speed(1, SpeedType.Absolute), new Bastard());
                        yield return this.Wait(3);
                    }
                    yield break;
                }
                private int speedUp;
            }
            public class Bastard : Bullet
            {
                public Bastard() : base(StaticBulletEntries.undodgeableLargeSpore.Name, false, false, false)
                {

                }
                public override IEnumerator Top()
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

            DefPath+"oppressor_firebasic_001.png",//20
            DefPath+"oppressor_firebasic_002.png",
            DefPath+"oppressor_firebasic_003.png",
            DefPath+"oppressor_firebasic_004.png",
            DefPath+"oppressor_firebasic_005.png",
            DefPath+"oppressor_firebasic_006.png",//25

            DefPath+"oppressor_chargebasic_001.png",//26
            DefPath+"oppressor_chargebasic_002.png",//27

            DefPath+"oppressor_awaken_001.png",//28
            DefPath+"oppressor_awaken_002.png",
            DefPath+"oppressor_awaken_003.png",
            DefPath+"oppressor_awaken_004.png",
            DefPath+"oppressor_awaken_005.png",
            DefPath+"oppressor_awaken_006.png",
            DefPath+"oppressor_awaken_007.png",
            DefPath+"oppressor_awaken_008.png",
            DefPath+"oppressor_awaken_009.png",
            DefPath+"oppressor_awaken_010.png",
            DefPath+"oppressor_awaken_011.png",
            DefPath+"oppressor_awaken_012.png",
            DefPath+"oppressor_awaken_013.png",//40


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
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
                {

                };
			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{//Surprise
				if (clip.GetFrame(frameIdx).eventInfo.Contains("ploompy"))
				{
                    StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(base.aiActor.sprite.WorldCenter);

                    /*
                    GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                    GameObject gameObject = GameObject.Instantiate(silencerVFX.gameObject, base.aiActor.sprite.WorldCenter, Quaternion.identity);
                    Destroy(gameObject, 2.5f);
                    */
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("megaDie"))
				{
                    AkSoundEngine.PostEvent("Play_PortalOpen", base.aiActor.gameObject);
                    AkSoundEngine.PostEvent("Play_BOSS_spacebaby_explode_01", base.aiActor.gameObject);

                    SpawnManager.SpawnBulletScript(base.aiActor, base.aiActor.sprite.WorldCenter, base.aiActor.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(Obliterate)), StringTableManager.GetEnemiesString("#TRAP", -1));

                    Exploder.DoDistortionWave(base.aiActor.sprite.WorldCenter, 10f, 0.5f, 15, 0.25f);
                    GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
					GameObject gameObject = GameObject.Instantiate(silencerVFX.gameObject, base.aiActor.sprite.WorldCenter, Quaternion.identity);
					Destroy(gameObject, 2.5f);
				}
			}
		}
	}
}





