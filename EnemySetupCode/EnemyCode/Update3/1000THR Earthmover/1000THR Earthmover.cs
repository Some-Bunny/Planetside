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
using GungeonAPI;
using SpriteBuilder = ItemAPI.SpriteBuilder;
using DirectionType = DirectionalAnimation.DirectionType;
using static DirectionalAnimation;
using EnemyBulletBuilder;
using SaveAPI;
namespace Planetside
{
	class THREarthmover : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "1000THR Earthmover";
		public static GameObject shootpoint;

		public static GameObject Cannon1;
		public static GameObject Cannon2;
		public static GameObject Cannon3;
		public static GameObject Cannon4;

		public static Texture2D BossCardTexture;


        public static List<int> spriteIds2 = new List<int>();

		public static void Init()
		{
            THREarthmover.BuildPrefab();
		}


		public static void BuildPrefab()
		{
            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ShellraxCollection").GetComponent<tk2dSpriteCollectionData>();
            //Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("fungannon material");

            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
			{
				prefab = BossBuilder.BuildPrefabBundle("1000THR Earthmover", guid, Collection, 48 ,new IntVector2(0, 0), new IntVector2(0, 0), false, true);
				var enemy = prefab.AddComponent<EnemyBehavior>();
                THRMEHAVB pain = prefab.AddComponent<THRMEHAVB>();
                //EnemyToolbox.QuickAssetBundleSpriteSetup(enemy.aiActor, Collection, null, false);


                AIAnimator aiAnimator = enemy.aiAnimator;

				enemy.aiActor.knockbackDoer.weight = 35;
				enemy.aiActor.MovementSpeed = 1.75f;
				enemy.aiActor.healthHaver.PreventAllDamage = false;
				enemy.aiActor.CollisionDamage = 1f;
				enemy.aiActor.IgnoreForRoomClear = false;
				enemy.aiActor.aiAnimator.HitReactChance = 0f;
				enemy.aiActor.specRigidbody.CollideWithOthers = true;
				enemy.aiActor.specRigidbody.CollideWithTileMap = true;
				enemy.aiActor.PreventFallingInPitsEver = false;
				enemy.aiActor.healthHaver.ForceSetCurrentHealth(1000000);
				enemy.aiActor.CollisionKnockbackStrength = 10f;
				enemy.aiActor.CanTargetPlayers = true;
				enemy.aiActor.healthHaver.SetHealthMaximum(1000000, null, false);
                enemy.aiActor.gameObject.GetOrAddComponent<ObjectVisibilityManager>();


                GameObject beetle = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Centaur_Midpoly_Riggeda_0"));
                beetle.transform.parent = enemy.aiActor.transform;

                beetle.transform.position = enemy.aiActor.transform.position;
				beetle.transform.localScale *= 7.5f;
                /*
                MeshRenderer renderer = beetle.GetComponentInChildren<MeshRenderer>();
                renderer.allowOcclusionWhenDynamic = true;
                beetle.transform.localScale = Vector3.one;
                beetle.name = "ShopPortal";
                beetle.transform.localScale *= 2;
                */
                beetle.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));

                beetle.transform.rotation = Quaternion.Euler(300, 180, 180);// 180, 180);

                //EnemyToolbox.AddShadowToAIActor(enemy.aiActor, StaticEnemyShadows.massiveShadow, new Vector2(2.875f, 0.25f), "shadowPos");

				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "idle",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.renderer.enabled = false;

				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
	                {
						"moveright", //Good
						"moveleft",//Good


	                }
				};

				//=====================================================================================
				DirectionalAnimation anim = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"roar",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "roar",
						anim = anim
					}
				};
				//=====================================================================================
				DirectionalAnimation ctahge = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
	{
						"charge",

	},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "charge",
						anim = ctahge
					}
				};
				//=====================================================================================
				DirectionalAnimation BirdUp = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"chargecannon",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "chargecannon",
						anim = ctahge
					}
				};
				//=====================================================================================
				DirectionalAnimation eee = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
	                {
						"jump",

	                },
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "jump",
						anim = eee
					}
				};
				//=====================================================================================

				//=====================================================================================
				DirectionalAnimation anim3 = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"jumpland",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "jumpland",
						anim = anim3
					}
				};
				//=====================================================================================

				DirectionalAnimation almostdone = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "intro",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "intro",
						anim = almostdone
					}
				};



                SpriteBuilder.AddAnimation(enemy.spriteAnimator, Collection, new List<int>
                    {

                    0,
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    0,
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    0,
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    25,
                    26,
                    27,
                    28,
                    29,
                    30,
                    31,
                    32,
                    33,
                    34,
                    35,//
					36,
                    37,


                    }, "intro", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
             









				enemy.aiActor.specRigidbody.PixelColliders.Clear();
				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 17,
					ManualOffsetY = 0,
					ManualWidth = 66,
					ManualHeight = 66,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0
				});

				enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{

					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyHitBox,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 17,
					ManualOffsetY = 0,
					ManualWidth = 66,
					ManualHeight = 66,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});

				enemy.aiActor.PreventBlackPhantom = false;


				shootpoint = new GameObject("CentreOfAllCannons");
				shootpoint.transform.parent = enemy.transform;
				shootpoint.transform.position = new Vector2(2.875f, 2.875f);
				GameObject CentreOfAllCannons = enemy.transform.Find("CentreOfAllCannons").gameObject;

				Cannon1 = new GameObject("CannonNorth");
				Cannon1.transform.parent = enemy.transform;
				Cannon1.transform.position = new Vector2(2.875f, 2.75f);
				GameObject CannonNorth = enemy.transform.Find("CannonNorth").gameObject;

				Cannon2 = new GameObject("CannonEast");
				Cannon2.transform.parent = enemy.transform;
				Cannon2.transform.position = new Vector2(5.125f, 2.625f);
				GameObject CannonEast= enemy.transform.Find("CannonEast").gameObject;

				Cannon3 = new GameObject("CannonSouth");
				Cannon3.transform.parent = enemy.transform;
				Cannon3.transform.position = new Vector2(2.875f, 3.1875f);
				GameObject CannonSouth = enemy.transform.Find("CannonSouth").gameObject;

				Cannon4 = new GameObject("CannonWest");
				Cannon4.transform.parent = enemy.transform;
				Cannon4.transform.position = new Vector2(0.625f, 2.625f);
				GameObject CannonWest = enemy.transform.Find("CannonWest").gameObject;


				var bs = prefab.GetComponent<BehaviorSpeculator>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;

				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				bs.TargetBehaviors = new List<TargetBehaviorBase>
				{
					new TargetPlayerBehavior
					{
						Radius = 45f,
						LineOfSight = true,
						ObjectPermanence = true,
						SearchInterval = 0.25f,
						PauseOnTargetSwitch = false,
						PauseTime = 0.25f
					},

				};

				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>() { };

                bs.MovementBehaviors = new List<MovementBehaviorBase>() { };
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:earthmover", enemy.aiActor);
				var nur = enemy.aiActor;
				nur.EffectResistances = new ActorEffectResistance[]
                {
					new ActorEffectResistance()
					{
						resistAmount = 1,
						resistType = EffectResistanceType.Poison
					},
                                        new ActorEffectResistance()
                    {
                        resistAmount = 1,
                        resistType = EffectResistanceType.Fire
                    },
                                                            new ActorEffectResistance()
                    {
                        resistAmount = 1,
                        resistType = EffectResistanceType.Freeze
                    },
                };

				//SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Coallet/Idle/coallet_idle_006", SpriteBuilder.ammonomiconCollection);
				if (enemy.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
				}
				/*
				GenericIntroDoer miniBossIntroDoer = prefab.AddComponent<GenericIntroDoer>();
				prefab.AddComponent<FungannonIntroController>();

				miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
				miniBossIntroDoer.initialDelay = 0.15f;
				miniBossIntroDoer.cameraMoveSpeed = 14;
				miniBossIntroDoer.specifyIntroAiAnimator = null;
				miniBossIntroDoer.BossMusicEvent = "Play_MUS_Boss_Theme_Beholster";
				//miniBossIntroDoer.BossMusicEvent = "Play_MUS_Lich_Double_01";
				miniBossIntroDoer.PreventBossMusic = false;
				miniBossIntroDoer.InvisibleBeforeIntroAnim = false;
				miniBossIntroDoer.preIntroAnim = string.Empty;
				miniBossIntroDoer.preIntroDirectionalAnim = string.Empty;
				miniBossIntroDoer.introAnim = "intro";
				miniBossIntroDoer.introDirectionalAnim = string.Empty;
				miniBossIntroDoer.continueAnimDuringOutro = false;
				miniBossIntroDoer.cameraFocus = null;
				miniBossIntroDoer.roomPositionCameraFocus = Vector2.zero;
				miniBossIntroDoer.restrictPlayerMotionToRoom = false;
				miniBossIntroDoer.fusebombLock = false;
				miniBossIntroDoer.AdditionalHeightOffset = 0;
				`*/

				PlanetsideModule.Strings.Enemies.Set("#EARTHMOVER", "1000-THR \"EARTHMOVER\"");
				PlanetsideModule.Strings.Enemies.Set("#EARTHMOVER_SMALL", "1000-THR \"EARTHMOVER\"");
                BossCardTexture = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("earthmoverBosscard");

                PlanetsideModule.Strings.Enemies.Set("#WAR_WITHOUT_REASON", "...LIKE ANTENNAS TO HEAVEN");
				enemy.aiActor.OverrideDisplayName = "#EARTHMOVER_SMALL";
                /*
				miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
				{
					bossNameString = "#EARTHMOVER",
					bossSubtitleString = "WAR_WITHOUT_REASON",
					bossQuoteString = "#QUOTE",
					bossSpritePxOffset = IntVector2.Zero,
					topLeftTextPxOffset = IntVector2.Zero,
					bottomRightTextPxOffset = IntVector2.Zero,
					bgColor = Color.blue
				};
                var BossCardTexture = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("fungannon_bosscard");
                if (BossCardTexture)
                {
                    miniBossIntroDoer.portraitSlideSettings.bossArtSprite = BossCardTexture;
                    miniBossIntroDoer.SkipBossCard = false;
                    enemy.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
                }
                else
				{
					miniBossIntroDoer.SkipBossCard = true;
					enemy.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
				}
				*/


                //EnemyBuilder.AddEnemyToDatabase(enemy.gameObject, "psog:earthmover");
				//EnemyDatabase.GetEntry("psog:earthmover").ForcedPositionInAmmonomicon = 4;
				//EnemyDatabase.GetEntry("psog:earthmover").isInBossTab = true;
				//EnemyDatabase.GetEntry("psog:earthmover").isNormalEnemy = true;

				//miniBossIntroDoer.SkipFinalizeAnimation = true;
				//miniBossIntroDoer.RegenerateCache();

				//==================
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(enemy.aiActor.healthHaver);
				//==================

			}
		}


		public class THRMEHAVB : BraveBehaviour
		{
			private RoomHandler m_StartRoom;
			bool f = false;
			public void Update()
			{

				if (GameManager.Instance.PrimaryPlayer.CurrentRoom == m_StartRoom && f == false) 
				{
					f = true;
                    GameManager.Instance.StartCoroutine(LateEngage());
                }
            }

			private IEnumerator LateEngage()
			{

                yield return new WaitForSeconds(0.5f);
                base.aiActor.HasBeenEngaged = true;
                this.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
                //GameManager.Instance.boss

                CameraController m_camera = GameManager.Instance.MainCameraController;
                m_camera.StopTrackingPlayer();
                m_camera.SetManualControl(true, true);

                m_camera.OverrideZoomScale = 0.3f;// GameManager.Instance.PrimaryPlayer.CenterPosition;

                m_camera.OverrideRecoverySpeed = 200;
                m_camera.OverridePosition = this.sprite.WorldCenter + new Vector2(-20, 30);

                GameUIBossHealthController gameUIBossHealthController = GameUIRoot.Instance.bossController;
                gameUIBossHealthController.RegisterBossHealthHaver(this.aiActor.healthHaver, "1000-THR \"EARTHMOVER\"");
                GameManager.Instance.DungeonMusicController.EndBossMusicNoVictory();
                AkSoundEngine.PostEvent("Stop_MUS_All", GameManager.Instance.gameObject);
                yield return new WaitForSeconds(2f);


                AkSoundEngine.PostEvent("Play_CentaurSpotted", GameManager.Instance.gameObject);
				var s = StaticExplosionDatas.CopyFields(StaticExplosionDatas.customDynamiteExplosion).ss;
				s.magnitude *= 50;
                s.time *= 10;

               
                GameManager.Instance.MainCameraController.DoScreenShake(s, this.sprite.WorldCenter + new Vector2(-20, 30));
                yield return new WaitForSeconds(2.5f);
                AkSoundEngine.PostEvent("Play_CentaurFireApproaching", GameManager.Instance.gameObject);
                yield return new WaitForSeconds(1f);
                AkSoundEngine.PostEvent("Play_CentaurHurt", GameManager.Instance.gameObject);
                AkSoundEngine.PostEvent("Play_WARWITHOUTREASON", GameManager.Instance.gameObject);
                Pixelator.Instance.FadeToColor(1, Color.white, true, 1.5f);


                GameManager.Instance.MainCameraController.DoScreenShake(s,GameManager.Instance.PrimaryPlayer.CenterPosition);
                m_camera.SetManualControl(false, true);
                m_camera.StartTrackingPlayer();
                m_camera.OverrideZoomScale = 1f;

                var ss = StaticExplosionDatas.CopyFields(StaticExplosionDatas.customDynamiteExplosion).ss;
                ss.magnitude *= 10;
                ss.time *= 10;


                GameManager.Instance.MainCameraController.DoScreenShake(ss, GameManager.Instance.PrimaryPlayer.CenterPosition);

                yield return new WaitForSeconds(2.5f);

				var thing = new PortraitSlideSettings()
                {
                    bossNameString = "#EARTHMOVER",
                    bossSubtitleString = "#WAR_WITHOUT_REASON",
                    bossQuoteString = "#QUOTE",
                    bossSpritePxOffset = new IntVector2(450, 0),
                    topLeftTextPxOffset = IntVector2.Zero,
                    bottomRightTextPxOffset = IntVector2.Zero,
                    bgColor = Color.white,
                    bossArtSprite = BossCardTexture
                };

                GameObject instantiatedBossCardPrefab = UnityEngine.Object.Instantiate<GameObject>(gameUIBossHealthController.bossCardUIPrefab.gameObject, new Vector3(-100f, -100f, 0f), Quaternion.identity);
                BossCardUIController bosscard = instantiatedBossCardPrefab.GetComponent<BossCardUIController>();
                bosscard.InitializeTexts(thing);
                gameUIBossHealthController.m_extantBosscard = bosscard;
                bosscard.Initialize();
                bosscard.ToggleBoxing(true);
                GameUIRoot.Instance.HideCoreUI(string.Empty);
                GameUIRoot.Instance.ToggleUICamera(false);
                bosscard.RecalculateScales();
                bosscard.lightStreaksSprite.IsVisible = false;
                for (int i = 0; i < bosscard.parallaxSprites.Count; i++)
                {
                    bosscard.parallaxSprites[i].IsVisible = false;
                }
                if (bosscard.playerSprite)
                {
                    bosscard.playerSprite.IsVisible = false;
                }
                if (bosscard.coopSprite)
                {
                    bosscard.coopSprite.IsVisible = false;
                }
                Material targetMaterial = bosscard.m_pix.RenderMaterial;
                base.StartCoroutine(bosscard.FlashWhiteToBlack(targetMaterial, false));
                BraveMemory.HandleBossCardFlashAnticipation();
                yield return base.StartCoroutine(bosscard.InvariantWaitForSeconds(bosscard.FLASH_DURATION));
                bosscard.bossSprite.transform.position = bosscard.bossStart.position;
                bosscard.playerSprite.transform.position = bosscard.playerStart.position;
                if (bosscard.coopSprite.IsVisible)
                {
                    bosscard.coopSprite.transform.position = bosscard.playerSprite.transform.position + bosscard.GetCoopOffset();
                }
                bosscard.ToggleCoreVisiblity(true);
                if (bosscard.playerSprite)
                {
                    bosscard.playerSprite.IsVisible = false;
                }
                if (bosscard.coopSprite)
                {
                    bosscard.coopSprite.IsVisible = false;
                }
                base.StartCoroutine(bosscard.HandleLightStreaks());
                yield return base.StartCoroutine(bosscard.InvariantWaitForSeconds(bosscard.FLASH_DURATION));
                targetMaterial.SetColor("_OverrideColor", Color.clear);
                base.StartCoroutine(bosscard.WomboCombo(thing));
                yield return base.StartCoroutine(bosscard.InvariantWaitForSeconds(bosscard.FLASHBAR_CROSS_DURATION));
                base.StartCoroutine(bosscard.LerpTextsToTargets());
                float waitDuration = Mathf.Max(bosscard.FLASHBAR_WAIT_DURATION + bosscard.FLASHBAR_EXPAND_DURATION, bosscard.TEXT_IN_DURATION);
                yield return base.StartCoroutine(bosscard.InvariantWaitForSeconds(waitDuration));
                yield return base.StartCoroutine(bosscard.InvariantWaitForSeconds(0.1f));
                yield return base.StartCoroutine(bosscard.HandleCharacterSlides());
                base.StartCoroutine(bosscard.FlashWhiteToBlack(targetMaterial, true));
                yield return base.StartCoroutine(bosscard.InvariantWaitForSeconds(bosscard.FLASH_DURATION));
                bosscard.ToggleCoreVisiblity(false);
                bosscard.m_doLightStreaks = false;
                bosscard.ResetTextsToStart();
                GameUIRoot.Instance.ToggleUICamera(true);
                GameUIRoot.Instance.ShowCoreUI(string.Empty);
                bosscard.ToggleBoxing(false);
                bosscard.m_isPlaying = false;



                yield return new WaitForSeconds(11f);
                AkSoundEngine.PostEvent("Play_CentaurFireApproaching", GameManager.Instance.gameObject);
                yield return new WaitForSeconds(1f);

                AkSoundEngine.PostEvent("Play_CentaurFire", GameManager.Instance.gameObject);
                Pixelator.Instance.FadeToColor(1, Color.white, true, 100f);
                yield return new WaitForSeconds(0.25f);
                FuckingObliteratePlayer();
                yield break;
			}

			public void FuckingObliteratePlayer()
			{
				GameManager.Instance.PrimaryPlayer.healthHaver.ApplyDamage(1, Vector2.zero, "1000-THR \"EARTHMOVER\"");

                GameUIRoot.Instance.bossController.DisableBossHealth();
                GameUIRoot.Instance.bossController2.DisableBossHealth();
                GameUIRoot.Instance.bossControllerSide.DisableBossHealth();
                GameManager.Instance.MainCameraController.OverridePosition = GameManager.Instance.PrimaryPlayer.CenterPosition;
                GameManager.Instance.PauseRaw();
                BraveTime.RegisterTimeScaleMultiplier(0f, GameManager.Instance.gameObject);
                AkSoundEngine.PostEvent("Play_UI_gameover_start_01", base.gameObject);
                GameManager.Instance.DoGameOver(GameManager.Instance.PrimaryPlayer.healthHaver.lastIncurredDamageSource);
            }


            public void Start()
			{
				this.transform.position += new Vector3(-14, -6);
                m_StartRoom = aiActor.GetAbsoluteParentRoom();
                base.aiActor.sprite.renderer.enabled = false;
                base.aiActor.spriteAnimator.renderer.enabled = false;      
			}

		}


	}
}

