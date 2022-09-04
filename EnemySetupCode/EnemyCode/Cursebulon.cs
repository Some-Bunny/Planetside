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
using static tk2dSpriteDefinition;
using PathologicalGames;

namespace Planetside
{
	public class Cursebulon : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "cursebulon";
		private static tk2dSpriteCollectionData CurseblobCollection;
		public static GameObject shootpoint;
		public static void Init()
		{
			Cursebulon.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Cursebulon", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 25;
				companion.aiActor.MovementSpeed = 4.75f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = false;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(25f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.healthHaver.SetHealthMaximum(25f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 0,
					ManualOffsetY = 0,
					ManualWidth = 20,
					ManualHeight = 20,
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
					ManualOffsetX = 0,
					ManualOffsetY = 0,
					ManualWidth = 24,
					ManualHeight = 23,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});


				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "die",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{

						   "die_left",
						   "die_right"

							}

						}
					}
				};
				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.FourWay,
					Flipped = new DirectionalAnimation.FlipType[4],
					AnimNames = new string[]
					{
						"idle_back_left",
						"idle_front_right",
						"idle_front_left",
						"idle_back_right",

					}
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.FourWay,
					Flipped = new DirectionalAnimation.FlipType[4],
					AnimNames = new string[]
						{
						"run_back_left",
						"run_front_right",
						"run_front_left",
						"run_back_right",

						}
				};

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "pitfall", new string[] { "pitfall" }, new DirectionalAnimation.FlipType[0], DirectionalAnimation.DirectionType.Single);


				/*
				{
                    GameObject vfxObj = ItemBuilder.AddSpriteToObject("TarnishVFX", "Planetside/Resources/VFX/BrainHost/brainnerphehehoo1", null);

                    vfxObj.transform.parent = companion.gameObject.transform;
                    vfxObj.transform.position = new Vector3(1, 1);

                    tk2dSpriteAnimator animator = vfxObj.GetOrAddComponent<tk2dSpriteAnimator>();
                    tk2dSpriteAnimation animation = vfxObj.AddComponent<tk2dSpriteAnimation>();
                    AIAnimator aiAnimatorBody = vfxObj.AddComponent<AIAnimator>();
                    aiAnimatorBody.IdleAnimation = new DirectionalAnimation
                    {
                        Type = DirectionalAnimation.DirectionType.Single,
                        Flipped = new DirectionalAnimation.FlipType[1],
                        AnimNames = new string[]
                        {
                        "idle"
                        }
                    };


                    EnemyToolbox.AddNewDirectionAnimation(aiAnimatorBody, "die", new string[] { "die" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                    vfxObj.AddComponent<MeshFilter>();
                    vfxObj.AddComponent<MeshRenderer>();


                    tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(vfxObj, ("BrainHostVFX"));
                    tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
                    List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
                    for (int i = 1; i < 7; i++)
                    {
                        tk2dSpriteCollectionData collection = DeathMarkcollection;
                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/BrainHost/brainnerphehehoo{i}", collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                        frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }

                    tk2dSpriteAnimationClip deathClip = new tk2dSpriteAnimationClip() { name = "die", frames = new tk2dSpriteAnimationFrame[0], fps = 12 };
                    List<tk2dSpriteAnimationFrame> deathFrames = new List<tk2dSpriteAnimationFrame>();
                    for (int i = 1; i < 7; i++)
                    {
                        tk2dSpriteCollectionData collection = DeathMarkcollection;
                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_left_00{i}", collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                        deathFrames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }
                    animator.sprite.usesOverrideMaterial = true;
                    animator.sprite.renderer.material = EnemyDatabase.GetOrLoadByGuid("c4fba8def15e47b297865b18e36cbef8").sprite.renderer.material;

                    //SpriteOutlineManager.AddOutlineToSprite(vfxObj.GetComponent<tk2dBaseSprite>(), Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);




                    deathClip.frames = deathFrames.ToArray();
                    deathClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;

                    idleClip.frames = frames.ToArray();
                    idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
                    animator.Library = animation;
                    animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip, deathClip };
                    animator.DefaultClipId = animator.GetClipIdByName("idle");
                    animator.playAutomatically = true;

                    vfxObj.transform.localScale *= 2;

                    AdvancedBodyPartController bodyPart = vfxObj.AddComponent<AdvancedBodyPartController>();




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
                        ManualOffsetX = 0,
                        ManualOffsetY = 0,
                        ManualWidth = 12,
                        ManualHeight = 12,
                        ManualDiameter = 0,
                        ManualLeftX = 0,
                        ManualLeftY = 0,
                        ManualRightX = 0,
                        ManualRightY = 0,

                    });

                    HealthHaver healthHaver = vfxObj.AddComponent<HealthHaver>();
                    healthHaver.SetHealthMaximum(169);
                    healthHaver.ForceSetCurrentHealth(169);
                    healthHaver.flashesOnDamage = true;
                    vfxObj.GetOrAddComponent<GameActor>();

                    Material mat = animator.sprite.renderer.material;
                    mat.mainTexture = animator.sprite.renderer.material.mainTexture;

                    mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    mat.DisableKeyword("BRIGHTNESS_CLAMP_OFF");

                    bodyPart.ownBody = body;
                    bodyPart.ownHealthHaver = healthHaver;


                    FakePrefab.MarkAsFakePrefab(vfxObj);
                    UnityEngine.Object.DontDestroyOnLoad(vfxObj);
                }

                {
                    GameObject vfxObj = ItemBuilder.AddSpriteToObject("TarnishVFX", "Planetside/Resources/VFX/BrainHost/brainnerphehehoo1", null);

                    vfxObj.transform.parent = companion.gameObject.transform;
                    vfxObj.transform.position = new Vector3(-1, 1);

                    tk2dSpriteAnimator animator = vfxObj.GetOrAddComponent<tk2dSpriteAnimator>();
                    tk2dSpriteAnimation animation = vfxObj.AddComponent<tk2dSpriteAnimation>();
                    AIAnimator aiAnimatorBody = vfxObj.AddComponent<AIAnimator>();
                    aiAnimatorBody.IdleAnimation = new DirectionalAnimation
                    {
                        Type = DirectionalAnimation.DirectionType.Single,
                        Flipped = new DirectionalAnimation.FlipType[1],
                        AnimNames = new string[]
                        {
                        "idle"
                        }
                    };


                    EnemyToolbox.AddNewDirectionAnimation(aiAnimatorBody, "die", new string[] { "die" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                    vfxObj.AddComponent<MeshFilter>();
                    vfxObj.AddComponent<MeshRenderer>();


                    tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(vfxObj, ("BrainHostVFX"));
                    tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "idle", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
                    List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
                    for (int i = 1; i < 7; i++)
                    {
                        tk2dSpriteCollectionData collection = DeathMarkcollection;
                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/BrainHost/brainnerphehehoo{i}", collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                        frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }

                    tk2dSpriteAnimationClip deathClip = new tk2dSpriteAnimationClip() { name = "die", frames = new tk2dSpriteAnimationFrame[0], fps = 12 };
                    List<tk2dSpriteAnimationFrame> deathFrames = new List<tk2dSpriteAnimationFrame>();
                    for (int i = 1; i < 7; i++)
                    {
                        tk2dSpriteCollectionData collection = DeathMarkcollection;
                        int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_left_00{i}", collection);
                        tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                        frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
                        deathFrames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
                    }
                    animator.sprite.usesOverrideMaterial = true;
                    animator.sprite.renderer.material = EnemyDatabase.GetOrLoadByGuid("c4fba8def15e47b297865b18e36cbef8").sprite.renderer.material;

                    //SpriteOutlineManager.AddOutlineToSprite(vfxObj.GetComponent<tk2dBaseSprite>(), Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);

                    deathClip.frames = deathFrames.ToArray();
                    deathClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;

                    idleClip.frames = frames.ToArray();
                    idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
                    animator.Library = animation;
                    animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip, deathClip };
                    animator.DefaultClipId = animator.GetClipIdByName("idle");
                    animator.playAutomatically = true;

                    vfxObj.transform.localScale *= 1;

                    AdvancedBodyPartController bodyPart = vfxObj.AddComponent<AdvancedBodyPartController>();




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
                        ManualOffsetX = 0,
                        ManualOffsetY = 0,
                        ManualWidth = 12,
                        ManualHeight = 12,
                        ManualDiameter = 0,
                        ManualLeftX = 0,
                        ManualLeftY = 0,
                        ManualRightX = 0,
                        ManualRightY = 0,

                    });

                    HealthHaver healthHaver = vfxObj.AddComponent<HealthHaver>();
                    healthHaver.SetHealthMaximum(10);
                    healthHaver.ForceSetCurrentHealth(10);
                    healthHaver.flashesOnDamage = true;

					vfxObj.GetOrAddComponent<GameActor>();
                    Material mat = animator.sprite.renderer.material;
                    mat.mainTexture = animator.sprite.renderer.material.mainTexture;

                    mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    mat.DisableKeyword("BRIGHTNESS_CLAMP_OFF");

                    bodyPart.ownBody = body;
                    bodyPart.ownHealthHaver = healthHaver;


                    FakePrefab.MarkAsFakePrefab(vfxObj);
                    UnityEngine.Object.DontDestroyOnLoad(vfxObj);
                }
				*/







                /*
				AIActor dummy = EnemyDatabase.GetOrLoadByGuid("4d164ba3f62648809a4a82c90fc22cae");
                for (int i = 0; i < dummy.transform.childCount; i++)
				{
					GameObject t = dummy.transform.GetChild(i).gameObject;
					if (t != null)
					{
						if (t.GetComponent<BodyPartController>() != null)
						{
                            foreach (Component c in t.GetComponents(typeof(Component)))
                            {
                                ETGModConsole.Log(c.GetType().ToString());
                                ETGModConsole.Log(c.name.ToString());
                                ETGModConsole.Log("=====");

                            }
                        }
					}
                }
				*/
                /*
				foreach (Component c in EnemyDatabase.GetOrLoadByGuid("21dd14e5ca2a4a388adab5b11b69a1e1").GetComponentInChildren<BodyPartController>().gameObject.GetComponents(typeof(Component)))
				{
					ETGModConsole.Log(c.GetType().ToString());
					ETGModConsole.Log(c.name.ToString());
					ETGModConsole.Log("=====");

                }
				*/

                EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);
				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				bool flag3 = CurseblobCollection == null;
				if (flag3)
				{
					CurseblobCollection = SpriteBuilder.ConstructCollection(prefab, "Curseblob_Collection");
					UnityEngine.Object.DontDestroyOnLoad(CurseblobCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], CurseblobCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CurseblobCollection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,


					}, "idle_back_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CurseblobCollection, new List<int>
					{
					6,
					7,
					8,
					9,
					10,
					11


					}, "idle_back_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CurseblobCollection, new List<int>
					{

					12,
					13,
					14,
					15,
					16,
				    17

					}, "idle_front_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CurseblobCollection, new List<int>
					{
					18,
					19,
					20,
					21,
					22,
					23


					}, "idle_front_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CurseblobCollection, new List<int>
					{

					12,
					13,
					14,
					15,
					16,
					17


					}, "run_front_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CurseblobCollection, new List<int>
					{

					18,
					19,
					20,
					21,
					22,
					23


					}, "run_front_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CurseblobCollection, new List<int>
					{
					0,
					1,
					2,
					3,
					4,
					5,



					}, "run_back_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CurseblobCollection, new List<int>
					{

				    6,
					7,
					8,
					9,
					10,
					11


					}, "run_back_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CurseblobCollection, new List<int>
					{

				 32,
				 33,
				 34,
				 35,
				 36,
				 37,
				 38,
				 39




					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 15f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CurseblobCollection, new List<int>
					{

				 24,
				 25,
				 26,
				 27,
				 28,
				 29,
				 30,
				 31

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 15f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CurseblobCollection, new List<int>
					{

				 40,
				 41,
				 42,
				 43,
				 44

					}, "pitfall", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
				}

				SpriteBuilder.AddAnimation(companion.spriteAnimator, CurseblobCollection, new List<int>
				{
					18,
				}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 4f;
				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				shootpoint = new GameObject("fuck");
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = companion.sprite.WorldCenter;
				GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;
				bs.TargetBehaviors = new List<TargetBehaviorBase>
			{
				new TargetPlayerBehavior
				{
					Radius = 35f,
					LineOfSight = true,
					ObjectPermanence = true,
					SearchInterval = 0.2f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.2f,
				}
			};
				bs.MovementBehaviors = new List<MovementBehaviorBase>() {
				new SeekTargetBehavior() {
					StopWhenInRange = false,
					CustomRange = 6,
					LineOfSight = true,
					ReturnToSpawn = true,
					SpawnTetherDistance = 0,
					PathInterval = 0.5f,
					SpecifyRange = false,
					MinActiveRange = -0.25f,
					MaxActiveRange = 0
				}
				};	
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:cursebulon", companion.aiActor);

				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_right_006", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				GoopDoer goopdoer = companion.gameObject.GetOrAddComponent<GoopDoer>();
				goopdoer.defaultGoopRadius = 0.8f;
				goopdoer.goopCenter = companion.gameObject;
				goopdoer.goopDefinition = DebuffLibrary.CursebulonGoop;
				goopdoer.goopTime = 0.1f;

				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:cursebulon";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_right_006";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\cursebulonsmmonomiconportrait.png");
				PlanetsideModule.Strings.Enemies.Set("#THE_CURSEBULON", "Cursebulon");
				PlanetsideModule.Strings.Enemies.Set("#THE_CURSEBULON_SHORTDESC", "Jelly Or Jammed?");
				PlanetsideModule.Strings.Enemies.Set("#THE_CURSEBULON_LONGDESC", "A blobuloid that has consumed so many cursed objects the curse has become one with it.\n\nSlaying one of these lets free the curses it holds inside.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#THE_CURSEBULON";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#THE_CURSEBULON_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#THE_CURSEBULON_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:cursebulon");
				EnemyDatabase.GetEntry("psog:cursebulon").ForcedPositionInAmmonomicon = 44;
				EnemyDatabase.GetEntry("psog:cursebulon").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:cursebulon").isNormalEnemy = true;
			}
		}



		private static string[] spritePaths = new string[]
		{

			//idle_back_left
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_left_001.png",//0
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_left_002.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_left_003.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_left_004.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_left_005.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_left_006.png",//5
			//idle_back_right
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_right_001.png",//6
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_right_002.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_right_003.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_right_004.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_right_005.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_back_right_006.png",//11
			//idle_front_left
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_left_001.png",//12
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_left_002.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_left_003.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_left_004.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_left_005.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_left_006.png",//17
			//idle_front_right
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_right_001.png",//18
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_right_002.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_right_003.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_right_004.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_right_005.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_idle_front_right_006.png",//23
			//death_left
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_left_001.png",//24
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_left_002.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_left_003.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_left_004.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_left_005.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_left_006.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_left_007.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_left_008.png",//31
			//death_right
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_right_001.png",//32
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_right_002.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_right_003.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_right_004.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_right_005.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_right_006.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_right_007.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_die_right_008.png",//39
			//pitfall
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_pitfall_001.png",//40
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_pitfall_002.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_pitfall_003.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_pitfall_004.png",
			"Planetside/Resources/Enemies/Cursebulon/cursebulon_pitfall_005.png",//44
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

				for (int i = 0; i < base.aiActor.gameObject.transform.childCount; i++ )
				{
					var tr = base.aiActor.gameObject.transform.GetChild(i);

                    if (tr.gameObject != null && tr.gameObject.GetComponent<AdvancedBodyPartController>() != null)
					{
						var mhm = tr.gameObject.GetComponent<AdvancedBodyPartController>();
						if (mhm != null)
						{
                            mhm.OnBodyPartPreDeath += (obj1, obj2, obj3) =>
                            {
                                GameObject vfx = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.DragunBoulderLandVFX, this.aiActor.transform.position, Quaternion.identity);
                                tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
                                component.PlaceAtPositionByAnchor(this.aiActor.transform.position, tk2dBaseSprite.Anchor.MiddleCenter);
                                component.HeightOffGround = 35f;
                                component.UpdateZDepth();
                                Destroy(component.gameObject, 2.5f);
                            };
                            mhm.OnBodyPartDamaged += (obj1, obj2, obj3, obj4, obj5, obj6, obj7) =>
                            {
                                mhm.MainBody.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero, true, false, false);
                            };
                            tr.gameObject.SetActive(true);
                        }




                    }

				}

				base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					float RAD = 4f;
					if (base.aiActor.IsBlackPhantom)
                    {
						RAD += 1.5f;
                    }
					AkSoundEngine.PostEvent("Play_OBJ_trashbag_burst_01", base.aiActor.gameObject);
					AkSoundEngine.PostEvent("Play_CHR_shadow_curse_01", null);
					base.aiActor.GetAbsoluteParentRoom().ApplyActionToNearbyEnemies(base.aiActor.CenterPosition, RAD, new Action<AIActor, float>(this.ProcessEnemy));
					base.aiActor.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero, true, false, false);
					GameManager.Instance.StartCoroutine(SpawnRadialPoofs(base.aiActor.transform.PositionVector2(), RAD));
				};
			}
			private void ProcessEnemy(AIActor target, float distance)
			{
				bool jamnation = target.IsBlackPhantom;
				if (!jamnation)
				{
					target.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero, true, false, false);
					target.BecomeBlackPhantom();
				}
				else if (target.IsBlackPhantom)
                {
					target.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero, true, false, false);
					target.gameObject.AddComponent<UmbraController>();
				}
			}
			private static IEnumerator SpawnRadialPoofs(Vector2 centre, float radius)
            {
				float elapsed = 0f;
				float duration = 0.25f;
				while (elapsed < duration)
				{
					elapsed += BraveTime.DeltaTime;
					float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
					for (int i = 0; i < 3; i++)
                    {
						Vector2 Point = MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), Mathf.Lerp(0.1f, radius, t));
						GameObject obj = UnityEngine.Object.Instantiate<GameObject>(RandomPiecesOfStuffToInitialise.cursepoofvfx, centre + Point, Quaternion.identity);
						obj.transform.localScale *= 1.2f;
					}
					yield return null;
				}
				yield break;
			}
		}
	}
}





