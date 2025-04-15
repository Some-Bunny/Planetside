using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;
using Pathfinding;

namespace Planetside
{
	public class Shellrax : AIActor
	{
		public static GameObject fuckyouprefab;
		public static readonly string guid = "Shellrax";
		public static GameObject shootpoint;
		public static GameObject shootpoint1;

		public static GameObject EyeScript;


		public static string TargetVFX;
		public static Texture2D ShellraxEyeTexture;

		public static void Init()
		{
			ShellraxEyeTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside/Resources2/ParticleTextures/shellraxeye.png");
			Shellrax.BuildPrefab();
		}

		public static void BuildPrefab()
		{

            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ShellraxCollection").GetComponent<tk2dSpriteCollectionData>();
            Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("shellrax material.mat");

            if (fuckyouprefab == null || !BossBuilder.Dictionary.ContainsKey(guid))
			{
				fuckyouprefab = BossBuilder.BuildPrefabBundle("Shellrax", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(8, 9) ,false, true);
				var companion = fuckyouprefab.AddComponent<EnemyBehavior>();
                EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, Collection, mat);


                companion.aiActor.knockbackDoer.weight = 200;
				companion.aiActor.MovementSpeed = 0f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0.05f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(525f);
				companion.aiActor.healthHaver.SetHealthMaximum(525f);
				companion.aiActor.CollisionKnockbackStrength = 2f;
				companion.aiActor.procedurallyOutlined = false;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();
				///	
				//PlanetsideModule.Strings.Enemies.Set("#SHELLRAX", "SHELLRAX");
				//PlanetsideModule.Strings.Enemies.Set("#????", "???");
				//PlanetsideModule.Strings.Enemies.Set("#SUBTITLE", "FAILED DEMI-LICH");
				//PlanetsideModule.Strings.Enemies.Set("#QUOTE", "");
				//companion.aiActor.healthHaver.overrideBossName = "#SHELLRAX";
				//companion.aiActor.OverrideDisplayName = "#SHELLRAX";
				//companion.aiActor.ActorName = "#SHELLRAX";
				//companion.aiActor.name = "#SHELLRAX";
				fuckyouprefab.name = companion.aiActor.OverrideDisplayName;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(1.625f, 0.25f), "shadowPos");


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
					ManualWidth = 36,
					ManualHeight = 40,
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
					ManualWidth = 36,
					ManualHeight = 40,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;
				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "idle",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};


				DirectionalAnimation anim = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"ribopen",

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "ribopen",
						anim = anim
					}
				};
				DirectionalAnimation eee = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"attackandclose",
					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "attackandclose",
						anim = eee
					}
				};


				DirectionalAnimation anim3 = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					AnimNames = new string[]
					{
						"tellfist",

					},
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "tellfist",
						anim = anim3
					}
				};
				DirectionalAnimation Hurray = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "slamfist",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "slamfist",
						anim = Hurray
					}
				};
				DirectionalAnimation TelepertOut = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "TeleportOut",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "TeleportOut",
						anim = TelepertOut
					}
				};


				DirectionalAnimation TelepertIn = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "TeleportIn",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "TeleportIn",
						anim = TelepertIn
					}
				};


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
				DirectionalAnimation done = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "death",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "death",
						anim = done
					}
				};

				DirectionalAnimation eye1 = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "chargeeye",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "chargeeye",
						anim = eye1
					}
				};

				DirectionalAnimation eye2 = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "fireeye",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "fireeye",
						anim = eye1
					}
				};

				DirectionalAnimation ribbys = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "keepribsopen",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "keepribsopen",
						anim = ribbys
					}
				};
				//bool flag3 = ShellraxClooection == null;
				//if (flag3)
				{
					/*
					ShellraxClooection = SpriteBuilder.ConstructCollection(fuckyouprefab, "Shellrax-Clooection");
					UnityEngine.Object.DontDestroyOnLoad(ShellraxClooection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], ShellraxClooection);
					}
					*/
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5

					}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					6,
					7,
					8,
					9,
					10


					}, "ribopen", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					11,
					12,
					13,
					14,
					11,
					12,
					13,
					14,
					11,
					12,
					13,
					14,
					11,
					12,
					13,
					14,
					15,
					16,
					17


					}, "attackandclose", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3.5f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					11,
					12,
					13,
					14,
					11,
					12,
					13,
					14,
					11,
					12,
					13,
					14,
					11,
					12,
					13,
					14,


					}, "keepribsopen", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 3.5f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					18,
					19,
					20,
					21,
					21,
					22,
					22
					


					}, "tellfist", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					23,
					24,
					25,
					26,
					27

					}, "slamfist", tk2dSpriteAnimationClip.WrapMode.Once).fps = 15f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					28,
					29,
					30,
					31,
					32,
					33,
					34,
					35,
					36

					}, "TeleportOut", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					37,
					38,
					39,
					40,
					41,
					42,
					43,
					44,
					45,
					46,
					47

					}, "TeleportIn", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				48,
				49,
				50,
				51,
				52,
				53,
				54,
				55,
				56,
				57,
				58,
				59,
				60,
				61,
				62,
				63,
				64,
				65,
				93,
				94,
				95,
				96,
				97,
				98,
				99
				

					}, "intro", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				66,
				67,
				68,
				69,
				70,
				71,
				72,
				73,
				74,
				75,
				76,
				77,
				78,
				79,
				80,
				81,
				82,
				83,
				84

					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				85,
				86,
				87,
				88,
				89,
				90,
					}, "chargeeye", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				91,
				92,
				93
				
					}, "fireeye", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

				}

				var death = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("death");
				death.frames[1].eventInfo = "disableparticles";
				death.frames[1].triggerEvent = true;

				var teleportin = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("TeleportIn");
				teleportin.frames[8].eventInfo = "enableparticles";
				teleportin.frames[8].triggerEvent = true;

				var teleportout = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("TeleportOut");
				teleportout.frames[1].eventInfo = "disableparticles";
				teleportout.frames[1].triggerEvent = true;

				var idle = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("idle");
				idle.frames[0].eventInfo = "enableparticlesspecial";
				idle.frames[0].triggerEvent = true;

				var eaee = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro");
				eaee.frames[0].eventInfo = "disableparticlesspecial";
				eaee.frames[0].triggerEvent = true;

				var tpout = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("TeleportOut");
				tpout.frames[8].eventInfo = "Disablerender";
				tpout.frames[8].triggerEvent = true;
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("TeleportOut").frames[1].eventAudio = "Play_ENM_shells_gather_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("TeleportOut").frames[1].triggerEvent = true;

				var tpin = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("TeleportIn");
				tpin.frames[0].eventInfo = "EnableRender";
				tpin.frames[0].triggerEvent = true;
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("TeleportIn").frames[1].eventAudio = "Play_ENM_shells_gather_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("TeleportIn").frames[1].triggerEvent = true;

				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[1].eventAudio = "Play_ENM_shells_gather_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[1].triggerEvent = true;
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[2].eventAudio = "Play_BOSS_lichC_intro_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[2].triggerEvent = true;

				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[19].eventAudio = "Play_BOSS_lichA_crack_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[19].triggerEvent = true;


				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[21].eventAudio = "Play_BOSS_doormimic_blast_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[21].triggerEvent = true;

				var bs = fuckyouprefab.GetComponent<BehaviorSpeculator>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

				EnemyToolbox.GenerateShootPoint(companion.gameObject, companion.sprite.WorldBottomLeft + new Vector2(1.25f, 2.5f), "eye");




				EyeScript = ItemBuilder.AddSpriteToObject("EyeScript", "Planetside/Resources/suncolor.png", null);
				EyeScript.transform.parent = companion.transform;
				EyeScript.transform.position = companion.sprite.WorldBottomLeft + new Vector2(1.25f, 2.375f);
				GameObject eyeScript = companion.transform.Find("EyeScript").gameObject;

				shootpoint = ItemBuilder.AddSpriteToObject("attach", "Planetside/Resources/suncolor.png", null);
				shootpoint.GetComponent<tk2dSprite>().sprite.renderer.enabled = false;
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = companion.sprite.WorldCenter;
				GameObject m_CachedGunAttachPoint = companion.transform.Find("attach").gameObject;
				
				shootpoint1 = new GameObject("fuck");
				shootpoint1.transform.parent = companion.transform;
				shootpoint1.transform.position = companion.sprite.WorldBottomLeft;
				GameObject m_CachedGunAttachPoint1 = companion.transform.Find("fuck").gameObject;

				AIActor actor = EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b");
				BeholsterController beholsterbeam = actor.GetComponent<BeholsterController>();

				AIBeamShooter2 bholsterbeam1 = companion.gameObject.AddComponent<AIBeamShooter2>();
				bholsterbeam1.beamTransform = shootpoint.transform;
				bholsterbeam1.beamModule = beholsterbeam.beamModule;
				bholsterbeam1.beamProjectile = beholsterbeam.projectile;
				bholsterbeam1.firingEllipseCenter = shootpoint.transform.position;
				bholsterbeam1.name = "DEATH";
				bholsterbeam1.northAngleTolerance = 180;

				bs.TargetBehaviors = new List<TargetBehaviorBase>
			{
				new TargetPlayerBehavior
				{
					Radius = 35f,
					LineOfSight = false,
					ObjectPermanence = true,
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.25f
				}
			};

				bs.MovementBehaviors = new List<MovementBehaviorBase>() {
				new SeekTargetBehavior() {
					StopWhenInRange = true,
					CustomRange = 6,
					LineOfSight = true,
					ReturnToSpawn = true,
					SpawnTetherDistance = 0,
					PathInterval = 0.5f,
					SpecifyRange = false,
					MinActiveRange = 1,
					MaxActiveRange = 10
				}
			};
				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
				{

					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0f,
					Behavior = new CustomBeholsterLaserBehavior{
					UsesBeamProjectileWithoutModule = false,
					InitialCooldown = 0f,
					firingTime = 20f,
					ImmuneToBlanks = true,
					Cooldown = 100,
					AttackCooldown = 100f,
					RequiresLineOfSight = false,
					FiresDirectlyTowardsPlayer = false,
					UsesCustomAngle = true,

					chargeTime = 0f,
					UsesBaseSounds = true,
					LaserFiringSound = "Play_ENM_deathray_shot_01",
					StopLaserFiringSound = "Stop_ENM_deathray_loop_01",
					ChargeAnimation = "ribopen",
					FireAnimation = "keepribsopen",
					beamSelection = ShootBeamBehavior.BeamSelection.All,
					trackingType = CustomBeholsterLaserBehavior.TrackingType.ConstantTurn,
					BulletScript = new CustomBulletScriptSelector(typeof(OMEGADEATHSCRIPTOFDOOM)),
					ShootPoint = shootpoint.transform,
					unitCatchUpSpeed = 30f,
					maxTurnRate = 30f,
					turnRateAcceleration = 30f,
					useDegreeCatchUp = companion.transform,
					minDegreesForCatchUp = 1.8f,
					degreeCatchUpSpeed = 180,
					useUnitCatchUp = true,
					minUnitForCatchUp = 2,
					maxUnitForCatchUp = 2,
					useUnitOvershoot = true,
					minUnitForOvershoot = 1,
					},
						NickName = "Death Laser"

					},


					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0.8f,
					Behavior = new ShootBehavior{
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(SemiCirclesOfDoom)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 3f,
					TellAnimation = "ribopen",
					FireAnimation = "attackandclose",
					RequiresLineOfSight = true,
					MultipleFireEvents = true,
										InitialCooldown = 0.5f,
					//EnabledDuringAttack = new PowderSkullSpinBulletsBehavior(),
					//StopDuring = ShootBehavior.StopType.Attack,
					Uninterruptible = false,
						},
						NickName = "SemiCircles"

					},
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0.5f,
					Behavior = new ShootBehavior{
					ShootPoint = eyeScript,
					BulletScript = new CustomBulletScriptSelector(typeof(DropDownScript)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 7f,
					TellAnimation = "chargeeye",
					PostFireAnimation = "fireeye",
					RequiresLineOfSight = true,
										InitialCooldown = 0.5f,
					MultipleFireEvents = true,
					//EnabledDuringAttack = new PowderSkullSpinBulletsBehavior(),
					//StopDuring = ShootBehavior.StopType.Attack,
					Uninterruptible = false,
						},
						NickName = "Support"

					},
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0.8f,
					Behavior = new ShootBehavior{
					ShootPoint = m_CachedGunAttachPoint1,
					BulletScript = new CustomBulletScriptSelector(typeof(Slammo)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 2f,
					TellAnimation = "tellfist",
					PostFireAnimation = "slamfist",
					RequiresLineOfSight = true,
										InitialCooldown = 0.5f,
					MultipleFireEvents = true,
					//EnabledDuringAttack = new PowderSkullSpinBulletsBehavior(),
					//StopDuring = ShootBehavior.StopType.Attack,
					Uninterruptible = false,
						},
						NickName = "SpreadSlam"

					},
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0.6f,
					Behavior = new ShootBehavior{
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(CirclesWithOpenings)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 3f,
					TellAnimation = "ribopen",
					FireAnimation = "attackandclose",
					RequiresLineOfSight = true,
					InitialCooldown = 0.5f,

					MultipleFireEvents = true,
					//EnabledDuringAttack = new PowderSkullSpinBulletsBehavior(),
					//StopDuring = ShootBehavior.StopType.Attack,
					Uninterruptible = false,
						},
						NickName = "Circles&Snakes"

					},
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0.7f,
					Behavior = new TeleportBehavior{
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
					MinDistanceFromPlayer = 7f,
					MaxDistanceFromPlayer = 12f,
					teleportInAnim = "TeleportIn",
					teleportOutAnim = "TeleportOut",
					AttackCooldown = 1f,
					InitialCooldown = 0.5f,
					RequiresLineOfSight = false,
					roomMax = new Vector2(0,0),
					roomMin = new Vector2(0,0),
					//teleportInBulletScript = new CustomBulletScriptSelector(typeof(ShellRaxDropCursedAreas)),
					teleportOutBulletScript = new CustomBulletScriptSelector(typeof(ShellRaxDropCursedAreas)),
					GlobalCooldown = 0.5f,
					Cooldown = 6f,

					CooldownVariance = 0f,
					InitialCooldownVariance = 0f,
					goneAttackBehavior = null,
					IsBlackPhantom = false,
					GroupName = null,
					GroupCooldown = 0f,
					MinRange = 0,
					Range = 0,
					MinHealthThreshold = 0,
					
					//MaxEnemiesInRoom = 1,
					MaxUsages = 0,
					AccumulateHealthThresholds = true,
					//shadowInAnim = null,
					//shadowOutAnim = null,
					targetAreaStyle = null,
					HealthThresholds = new float[0],
					MinWallDistance = 0,
					},
					//resetCooldownOnDamage = null,
					//shadowSupport = (TeleportBehavior.ShadowSupport)1,

					},

				};
			
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:shellrax", companion.aiActor);

				//==================	
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(companion.aiActor.healthHaver);
				//==================

				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Shellrax/shellrax_idle_001", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:shellrax";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Shellrax/shellrax_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("shellraxicon");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\shellraxicon.png");
                PlanetsideModule.Strings.Enemies.Set("#SHELLAD", "Shellrax");
				PlanetsideModule.Strings.Enemies.Set("#SHELLAD_SHORTDESC", "Failed Demi-lich");
				PlanetsideModule.Strings.Enemies.Set("#SHELLAD_LONGDESC", "A prior student of the Lich, Shellrax was destined to become the heir of the Gungeon. However, insanity and power-creep caused Shellrax to retort against his Master to claim the throne before it was due.\n\nAlthough Shellrax was no match for the Lich, impressed with his strength, the Lich spared him, and left him to roam the halls of the Gungeon as a powerful guardian of Bullet Hell.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#SHELLAD";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#SHELLAD_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#SHELLAD_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:shellrax");
				EnemyDatabase.GetEntry("psog:shellrax").ForcedPositionInAmmonomicon = 200;
				EnemyDatabase.GetEntry("psog:shellrax").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:shellrax").isNormalEnemy = true;

				GenericIntroDoer miniBossIntroDoer = fuckyouprefab.AddComponent<GenericIntroDoer>();
				fuckyouprefab.AddComponent<ShellraxIntro>();
				miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
				miniBossIntroDoer.initialDelay = 0.15f;
				miniBossIntroDoer.cameraMoveSpeed = 14;
				miniBossIntroDoer.specifyIntroAiAnimator = null;
				miniBossIntroDoer.BossMusicEvent = "Play_MUS_Boss_Theme_Dragun_02";
				//miniBossIntroDoer.BossMusicEvent = "Play_MUS_Lich_Double_01";
				miniBossIntroDoer.PreventBossMusic = false;
				miniBossIntroDoer.InvisibleBeforeIntroAnim = true;
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


				PlanetsideModule.Strings.Enemies.Set("#SHELLRAX", "SHELLRAX");
				PlanetsideModule.Strings.Enemies.Set("#SHELLRAD_SHORTDESC", "FAILED DEMI-LICH");
				PlanetsideModule.Strings.Enemies.Set("#QUOTE", "");

				miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
				{
					bossNameString = "#SHELLRAX",
					bossSubtitleString = "#SHELLRAD_SHORTDESC",
					bossQuoteString = "#QUOTE",
					bossSpritePxOffset = IntVector2.Zero,
					topLeftTextPxOffset = IntVector2.Zero,
					bottomRightTextPxOffset = IntVector2.Zero,
					bgColor = Color.red
				};
				var BossCardTexture = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("shellrax_bosscard");
                if (BossCardTexture)
				{
					miniBossIntroDoer.portraitSlideSettings.bossArtSprite = BossCardTexture;
					miniBossIntroDoer.SkipBossCard = false;
					companion.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.SubbossBar;
				}
				else
				{
					miniBossIntroDoer.SkipBossCard = true;
					companion.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.SubbossBar;
				}
				miniBossIntroDoer.SkipFinalizeAnimation = true;
				miniBossIntroDoer.RegenerateCache();

				//companion.gameObject.layer = 25;

				//ShellraxEyeController = yes;

				//companion.aiActor.sprite.HeightOffGround = 60;

			}
		}

		//public static ParticleSystem ShellraxEyeController;
		public static List<int> Lootdrops = new List<int>
		{
			73,
			85,
			120,
			67,
			224,
			600,
			78
		};





		private static string[] spritePaths = new string[]
		{
			
			//idle
			"Planetside/Resources/Shellrax/shellrax_idle_001.png",
			"Planetside/Resources/Shellrax/shellrax_idle_002.png",
			"Planetside/Resources/Shellrax/shellrax_idle_003.png",
			"Planetside/Resources/Shellrax/shellrax_idle_004.png",
			"Planetside/Resources/Shellrax/shellrax_idle_005.png",
			"Planetside/Resources/Shellrax/shellrax_idle_006.png",
			//6
			//open ribs
			"Planetside/Resources/Shellrax/shellrax_ribopen_001.png",
			"Planetside/Resources/Shellrax/shellrax_ribopen_002.png",
			"Planetside/Resources/Shellrax/shellrax_ribopen_003.png",
			"Planetside/Resources/Shellrax/shellrax_ribopen_004.png",
			"Planetside/Resources/Shellrax/shellrax_ribopen_005.png",
			//11

			//ribs
			"Planetside/Resources/Shellrax/shellrax_ribs_001.png",
			"Planetside/Resources/Shellrax/shellrax_ribs_002.png",
			"Planetside/Resources/Shellrax/shellrax_ribs_003.png",
			"Planetside/Resources/Shellrax/shellrax_ribs_004.png",
			//ribsclose
			"Planetside/Resources/Shellrax/shellrax_ribs_005.png",
			"Planetside/Resources/Shellrax/shellrax_ribs_006.png",
			"Planetside/Resources/Shellrax/shellrax_ribs_007.png",
			//18

			//tell
			"Planetside/Resources/Shellrax/shellrax_tell1_001.png",
			"Planetside/Resources/Shellrax/shellrax_tell1_002.png",
			"Planetside/Resources/Shellrax/shellrax_tell1_003.png",
			"Planetside/Resources/Shellrax/shellrax_tell1_004.png",
			"Planetside/Resources/Shellrax/shellrax_tell1_005.png",
			//23

			//slam
			"Planetside/Resources/Shellrax/shellrax_slam_001.png",
			"Planetside/Resources/Shellrax/shellrax_slam_002.png",
			"Planetside/Resources/Shellrax/shellrax_slam_003.png",
			"Planetside/Resources/Shellrax/shellrax_slam_004.png",
			"Planetside/Resources/Shellrax/shellrax_slam_005.png",
			//28
			//Teleport out
			"Planetside/Resources/Shellrax/shellrax_warp_001.png",
			"Planetside/Resources/Shellrax/shellrax_warp_002.png",
			"Planetside/Resources/Shellrax/shellrax_warp_003.png",
			"Planetside/Resources/Shellrax/shellrax_warp_004.png",
			"Planetside/Resources/Shellrax/shellrax_warp_005.png",
			"Planetside/Resources/Shellrax/shellrax_warp_006.png",
			"Planetside/Resources/Shellrax/shellrax_warp_007.png",
			"Planetside/Resources/Shellrax/shellrax_warp_008.png",
			"Planetside/Resources/Shellrax/shellrax_warp_009.png",
			//37
			//Teleport in
			"Planetside/Resources/Shellrax/shellrax_warpin_001.png",
			"Planetside/Resources/Shellrax/shellrax_warpin_002.png",
			"Planetside/Resources/Shellrax/shellrax_warpin_003.png",
			"Planetside/Resources/Shellrax/shellrax_warpin_004.png",
			"Planetside/Resources/Shellrax/shellrax_warpin_005.png",
			"Planetside/Resources/Shellrax/shellrax_warpin_006.png",
			"Planetside/Resources/Shellrax/shellrax_warpin_007.png",
			"Planetside/Resources/Shellrax/shellrax_warpin_008.png",
			"Planetside/Resources/Shellrax/shellrax_warpin_009.png",
			"Planetside/Resources/Shellrax/shellrax_warpin_010.png",
			"Planetside/Resources/Shellrax/shellrax_warpin_011.png",
			//48
			//intro /
			"Planetside/Resources/Shellrax/shellrax_intro_001.png",
			"Planetside/Resources/Shellrax/shellrax_intro_002.png",
			"Planetside/Resources/Shellrax/shellrax_intro_003.png",
			"Planetside/Resources/Shellrax/shellrax_intro_004.png",
			"Planetside/Resources/Shellrax/shellrax_intro_005.png",
			"Planetside/Resources/Shellrax/shellrax_intro_006.png",
			"Planetside/Resources/Shellrax/shellrax_intro_007.png",
			"Planetside/Resources/Shellrax/shellrax_intro_008.png",
			"Planetside/Resources/Shellrax/shellrax_intro_009.png",
			"Planetside/Resources/Shellrax/shellrax_intro_010.png",
			"Planetside/Resources/Shellrax/shellrax_intro_011.png",
			"Planetside/Resources/Shellrax/shellrax_intro_012.png",
			"Planetside/Resources/Shellrax/shellrax_intro_013.png",
			"Planetside/Resources/Shellrax/shellrax_intro_014.png",
			"Planetside/Resources/Shellrax/shellrax_intro_015.png",
			"Planetside/Resources/Shellrax/shellrax_intro_016.png",
			"Planetside/Resources/Shellrax/shellrax_intro_017.png",
			"Planetside/Resources/Shellrax/shellrax_intro_018.png",
			//66

			//die /
			"Planetside/Resources/Shellrax/shellrax_death_001.png",
			"Planetside/Resources/Shellrax/shellrax_death_002.png",
			"Planetside/Resources/Shellrax/shellrax_death_003.png",
			"Planetside/Resources/Shellrax/shellrax_death_004.png",
			"Planetside/Resources/Shellrax/shellrax_death_005.png",
			"Planetside/Resources/Shellrax/shellrax_death_006.png",
			"Planetside/Resources/Shellrax/shellrax_death_007.png",
			"Planetside/Resources/Shellrax/shellrax_death_008.png",
			"Planetside/Resources/Shellrax/shellrax_death_009.png",
			"Planetside/Resources/Shellrax/shellrax_death_010.png",
			"Planetside/Resources/Shellrax/shellrax_death_011.png",
			"Planetside/Resources/Shellrax/shellrax_death_012.png",
			"Planetside/Resources/Shellrax/shellrax_death_013.png",
			"Planetside/Resources/Shellrax/shellrax_death_014.png",
			"Planetside/Resources/Shellrax/shellrax_death_015.png",
			"Planetside/Resources/Shellrax/shellrax_death_016.png",
			"Planetside/Resources/Shellrax/shellrax_death_017.png",
			"Planetside/Resources/Shellrax/shellrax_death_018.png",
			"Planetside/Resources/Shellrax/shellrax_death_019.png",
			//85

			"Planetside/Resources/Shellrax/shellrax_chargeeye_001.png",
			"Planetside/Resources/Shellrax/shellrax_chargeeye_002.png",
			"Planetside/Resources/Shellrax/shellrax_chargeeye_003.png",
			"Planetside/Resources/Shellrax/shellrax_chargeeye_004.png",
			"Planetside/Resources/Shellrax/shellrax_chargeeye_005.png",
			"Planetside/Resources/Shellrax/shellrax_chargeeye_006.png",
			//91
			"Planetside/Resources/Shellrax/shellrax_fireeye_001.png",
			"Planetside/Resources/Shellrax/shellrax_fireeye_002.png",
			"Planetside/Resources/Shellrax/shellrax_fireeye_003.png",
			//94
			"Planetside/Resources/Shellrax/shellrax_intro_020.png",
			"Planetside/Resources/Shellrax/shellrax_intro_021.png",
			"Planetside/Resources/Shellrax/shellrax_intro_022.png",
			"Planetside/Resources/Shellrax/shellrax_intro_023.png",
			"Planetside/Resources/Shellrax/shellrax_intro_024.png",
			"Planetside/Resources/Shellrax/shellrax_intro_025.png",
			"Planetside/Resources/Shellrax/shellrax_intro_026.png",

		};
	}

	public class DropDownScript : Script 
	{
		public override IEnumerator Top()
		{
			base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);
			base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("snakeBullet"));
			RoomHandler room = this.BulletBank.aiActor.ParentRoom;
			for (int i = 0; i < 2; i++)
            {
				Vector2 leftPos = BraveUtility.RandomVector2(new Vector2(room.area.UnitLeft + 2f, room.area.UnitBottom + 2f), new Vector2(room.area.UnitCenter.x - 2f, room.area.UnitTop - 2f));
				Vector2 rightPos = BraveUtility.RandomVector2(new Vector2(room.area.UnitCenter.x + 2f, room.area.UnitBottom + 2f), new Vector2(room.area.UnitRight - 2f, room.area.UnitTop - 2f));
				this.FireShield(leftPos - this.Position);
				this.FireShield(rightPos - this.Position);
			}
			//this.FireShield(this.BulletManager.PlayerPosition() - this.Position);
			yield break;
		}
		private void FireShield(Vector2 endOffset)
		{
			float time = UnityEngine.Random.Range(30, 300);
			this.FireExpandingLine(new Vector2(-0.5f, 0f), new Vector2(0.5f, 0f), 5, endOffset, time);
			this.FireExpandingLine(new Vector2(0f, -0.5f), new Vector2(0f, 0.5f), 5, endOffset, time);
			this.FireNonExpandingLine(new Vector2(0f, 0f), new Vector2(0f, 0f), 1, endOffset, time);

		}

		// Token: 0x06000896 RID: 2198 RVA: 0x00027A94 File Offset: 0x00025C94
		private void FireExpandingLine(Vector2 start, Vector2 end, int numBullets, Vector2 endOffset, float time)
		{
			start *= 0.5f;
			end *= 0.5f;
			for (int i = 0; i < numBullets; i++)
			{
				float t = (numBullets > 1) ? ((float)i / ((float)numBullets - 1f)) : 0.5f;
				Vector2 vector = Vector2.Lerp(start, end, t);
				vector.y *= -1f;
				base.Fire(new Offset(vector * 4f, 0f, string.Empty, DirectionType.Absolute), new Direction(vector.ToAngle(), DirectionType.Absolute, -1f), new DropDownScript.ShieldBullet(endOffset, false, time));
			}
		}
		private void FireNonExpandingLine(Vector2 start, Vector2 end, int numBullets, Vector2 endOffset, float time)
		{
			start *= 0f;
			end *= 0f;
			for (int i = 0; i < numBullets; i++)
			{
				Vector2 vector = Vector2.Lerp(start, end, 1);
				vector.y *= -1f;
				base.Fire(new Offset(new Vector2(0,0), 0f, string.Empty, DirectionType.Absolute), new Direction(vector.ToAngle(), DirectionType.Absolute, -1f), new DropDownScript.ShieldBullet(endOffset, true, time));
			}
		}

		public class ShieldBullet : Bullet
		{
			public ShieldBullet(Vector2 endOffset, bool drop, float time) : base("snakeBullet", false, false, false)
			{
				this.m_endOffset = endOffset;
				base.SuppressVfx = true;
				this.drops = drop;
				this.Waittime = time;
			}
			public override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("4d164ba3f62648809a4a82c90fc22cae").bulletBank.GetBullet("big_one"));
				this.ManualControl = true;
				yield return this.Wait(30);
				Vector2 start = this.Position;
				Vector2 end = this.Position + this.m_endOffset;
				this.ManualControl = true;

				for (int i = 0; i < 120; i++)
				{
					float t = (float)(i + 1) / 120f;
					this.Position = new Vector2(Mathf.SmoothStep(start.x, end.x, t), Mathf.SmoothStep(start.y, end.y, t));
					yield return this.Wait(1);
				}
				yield return this.Wait(Waittime);

				if (drops == true)
                {
					base.Fire(Offset.OverridePosition(base.Position + new Vector2(0f, 30f)), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new DropDownScript.BigBullet());
				}
				yield return this.Wait(60);
				this.Vanish(false);
				yield break;
			}
			private Vector2 m_endOffset;
			private bool drops;
			private float Waittime;

		}

		private class BigBullet : Bullet
		{
			public BigBullet() : base("big_one", false, false, false)
			{
			}

			public override void Initialize()
			{
				this.Projectile.spriteAnimator.StopAndResetFrameToDefault();
				base.Initialize();
			}

			public override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
				this.Projectile.specRigidbody.CollideWithTileMap = false;
				this.Projectile.specRigidbody.CollideWithOthers = false;
				yield return base.Wait(60);
				base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);
				this.Speed = 0f;
				this.Projectile.spriteAnimator.Play();
				base.Vanish(true);
				yield break;
			}

			public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				if (!preventSpawningProjectiles)
				{
					var Enemy = EnemyDatabase.GetOrLoadByGuid("e21ac9492110493baef6df02a2682a0d");
					AIActor.Spawn(Enemy.aiActor, this.Projectile.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Default, true);
					float num = base.RandomAngle();
					float Amount = 12;
					float Angle = 360 / Amount;
					for (int i = 0; i < Amount; i++)
					{
						base.Fire(new Direction(num + Angle * (float)i + 10, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new BurstBullet());
					}
					return;
				}
			}
			public class BurstBullet : Bullet
			{
				public BurstBullet() : base("reversible", false, false, false)
				{
				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
					yield return base.Wait(60);
					base.Vanish(false);
					yield break;
				}
			}

		}
	}


	public class ShellRaxDropCursedAreas : Script
	{
		public override IEnumerator Top()
		{
			DraGunController dragunController = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>();
			AkSoundEngine.PostEvent("Play_BOSS_wall_slam_01", base.BulletBank.aiActor.gameObject);
			int DropDowns = UnityEngine.Random.Range(2, 5);
			for (int i = 0; i < DropDowns; i++)
			{
				PlayerController player = (GameManager.Instance.PrimaryPlayer);

				IntVector2? vector = player.CurrentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 2), CellTypes.FLOOR | CellTypes.PIT, false, null);
				Vector2 vector2 = vector.Value.ToVector2();
				this.FireRocket(dragunController.skyBoulder, vector2);
				yield return base.Wait(10);
			}

			yield return base.Wait(60);
			base.PostWwiseEvent("Play_VO_lichA_chuckle_01", null);
			DropDowns = UnityEngine.Random.Range(1, 4);
			for (int u = 0; u < DropDowns; u++)
			{
				this.m_player = base.BulletBank.GetComponent<PlayerController>();
				PlayerController player = (GameManager.Instance.PrimaryPlayer);
				IntVector2? vector = player.CurrentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 2), CellTypes.FLOOR | CellTypes.PIT, false, null);
				Vector2 vector2 = vector.Value.ToVector2();
				this.FireRocket(dragunController.skyBoulder, vector2);
				yield return base.Wait(10);
			}
			yield break;
		}
		private void FireRocket(GameObject skyRocket, Vector2 target)
		{
			SkyRocket component = SpawnManager.SpawnProjectile(skyRocket, base.Position, Quaternion.identity, true).GetComponent<SkyRocket>();
			component.TargetVector2 = target;
			tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
			component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
			component.ExplosionData.ignoreList.Add(base.BulletBank.specRigidbody);
		}
		public int NumRockets = 3;
		public PlayerController m_player;
	}


	public class SemiCirclesOfDoom : Script 
	{
		public override IEnumerator Top()
		{
			if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").bulletBank.GetBullet("amuletRing"));
			}
			for (int u = 0; u < 4; u++)
            {
				base.PostWwiseEvent("Play_BOSS_lichB_charge_01", null);
				string BulletType = "amuletRing";
				float radius = UnityEngine.Random.Range(0.03f, 0.09f);
				float delta = 10f;
				float startDirection = AimDirection;
				for (int j = 0; j < 6; j++)
				{
					base.Fire(new Direction(-90f, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SemiCirclesOfDoom.TheGear(30,BulletType, this, startDirection + (float)j * delta, radius - 0.01f));
					base.Fire(new Direction(-90+delta, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SemiCirclesOfDoom.TheGear(30, BulletType, this, (startDirection + (float)j * delta), radius- 0.005f));
					base.Fire(new Direction(-90+(delta*2), DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SemiCirclesOfDoom.TheGear(30, BulletType, this, (startDirection + (float)j * delta), radius));
				}
				for (int j = 0; j < 6; j++)
				{
					base.Fire(new Direction(-90f, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SemiCirclesOfDoom.TheGear(30, BulletType, this, (startDirection + (float)j * delta)+180, radius - 0.01f));
					base.Fire(new Direction(-90 + delta, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SemiCirclesOfDoom.TheGear(30, BulletType, this, (startDirection + (float)j * delta) + 180, radius -0.005f));
					base.Fire(new Direction(-90 + (delta * 2), DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SemiCirclesOfDoom.TheGear(30, BulletType, this, (startDirection + (float)j * delta) + 180, radius));
				}
				BulletType = "bigBullet";
				base.Fire(new Direction(-90f, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SemiCirclesOfDoom.TheGear(0, BulletType, this, startDirection + (float)+0, 0));

				yield return base.Wait(60);
			}
			yield break;
		}
		public class TheGear : Bullet
		{
			public TheGear(float spinspeed, string BulletType, SemiCirclesOfDoom parent, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
			{
				this.m_parent = parent;
				this.m_angle = angle;
				this.m_radius = aradius;
				this.m_bulletype = BulletType;
				this.SuppressVfx = true;
				this.m_spinSpeed = spinspeed;
			}

			public override IEnumerator Top()
			{
				base.ManualControl = true;
				Vector2 centerPosition = base.Position;
				float radius = 0f;
				for (int i = 0; i < 300; i++)
				{
					if (i == 40)
					{
						base.ChangeSpeed(new Speed(base.Speed + 11f, SpeedType.Absolute), 120);
						base.ChangeDirection(new Direction(this.m_parent.GetAimDirection(1f, 10f), DirectionType.Absolute, -1f), 20);
						base.StartTask(this.ChangeSpinSpeedTask(180f, 240));
					}
					bool HasDiverted = false;
					if (i > 100 && i < 140 && UnityEngine.Random.value < 0.02f && m_bulletype == "bigBullet" && HasDiverted == false)
					{
						HasDiverted = true;
						float speed = base.Speed;
						this.Direction = base.AimDirection;
						this.Speed = speed * 0.75f;
						base.ManualControl = false;
						yield break;
					}
					base.UpdateVelocity();
					centerPosition += this.Velocity / 60f;
					if (i < 50)
					{
						radius += m_radius;
					}
					this.m_angle += this.m_spinSpeed / 60f;
					base.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);
					yield return base.Wait(1);
				}
				base.Vanish(false);
				yield break;
			}

			private IEnumerator ChangeSpinSpeedTask(float newSpinSpeed, int term)
			{
				float delta = (newSpinSpeed - this.m_spinSpeed) / (float)term;
				for (int i = 0; i < term; i++)
				{
					this.m_spinSpeed += delta;
					yield return base.Wait(1);
				}
				yield break;
			}
			private const float ExpandSpeed = 4.5f;
			private const float SpinSpeed = 40f;
			private SemiCirclesOfDoom m_parent;
			private float m_angle;
			private float m_spinSpeed;
			private float m_radius;
			private string m_bulletype;

		}

	}
	public class Slammo : Script
	{
		public override IEnumerator Top()
		{
			base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));
			base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("snakeBullet"));

			base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);
			Exploder.DoDistortionWave(base.BulletBank.sprite.WorldCenter, 2, 0.05f, 10, 0.7f);

			for (int e = 0; e < 4; e++)
            {
				Vector2 targetPos = base.GetPredictedTargetPosition((float)(((double)UnityEngine.Random.value >= 0.5) ? 1 : 0), 12f);
				float startingDirection = UnityEngine.Random.Range(-75, 75);
				for (int j = 0; j < 9; j++)
				{
					base.Fire(new Direction(startingDirection, DirectionType.Aim, -1f), new Speed(7f+e, SpeedType.Absolute), new Slammo.SnakeBullet(j * 2, targetPos));
				}
			}

			for (int e = 0; e < 11; e++)
			{
				Vector2 targetPos = base.GetPredictedTargetPosition((float)(((double)UnityEngine.Random.value >= 0.5) ? 1 : 0), 12f);
				float startingDirection = UnityEngine.Random.Range(-180, 180);
				for (int j = 0; j < 4; j++)
				{
					base.Fire(new Direction(startingDirection, DirectionType.Aim, -1f), new Speed(4f + e, SpeedType.Absolute), new Slammo.SnakeBullet(j * 2, targetPos));
				}
			}
			yield break;
		}
		public class SnakeBullet : Bullet
		{
			public SnakeBullet(int delay, Vector2 target) : base("snakeBullet", false, false, false)
			{
				this.delay = delay;
				this.target = target;
			}

			public override IEnumerator Top()
			{
				base.ManualControl = true;
				yield return base.Wait(this.delay);
				Vector2 truePosition = base.Position;
				for (int i = 0; i < 360; i++)
				{
					float offsetMagnitude = Mathf.SmoothStep(-0.9f, 0.9f, Mathf.PingPong(0.5f + (float)i / 60f * 3f, 1f));
					if (i > 20 && i < 60)
					{
						float num = (this.target - truePosition).ToAngle();
						float value = BraveMathCollege.ClampAngle180(num - this.Direction);
						this.Direction += Mathf.Clamp(value, -8f, 8f);
					}
					truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
					base.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude);
					yield return base.Wait(1);
				}
				base.Vanish(false);
				yield break;
			}


			private int delay;
			private Vector2 target;
		}
		public class BabyShot : Bullet
		{
			public BabyShot() : base("poundSmall", false, false, false)
			{
				base.SuppressVfx = true;

			}
			public override IEnumerator Top()
			{
				
				float speed = base.Speed;
				base.ChangeSpeed(new Speed(speed * 2, SpeedType.Absolute), 30);
				yield break;
			}

		}
		public class Yees : Bullet
		{
			public Yees() : base("snakeBullet", false, false, false)
			{
				base.SuppressVfx = true;

			}
			public override IEnumerator Top()
			{
				float speed = base.Speed;
				base.ChangeSpeed(new Speed(speed * 1.5f, SpeedType.Absolute), 45);
				yield break;
			}

		}
	}

	public class CirclesWithOpenings : Script
	{
		public override IEnumerator Top()
		{
			base.PostWwiseEvent("Play_BOSS_lichB_spew_01", null);
			base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"));
			base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("snakeBullet"));

			int AAmountToShoot = UnityEngine.Random.Range(4, 6);
			for (int i = 0; i < AAmountToShoot; i++)
            {
				Exploder.DoDistortionWave(base.BulletBank.sprite.WorldCenter, 2, 0.03f, 15, 0.7f);
				base.PostWwiseEvent("Play_ENM_kali_shockwave_01", null);
				this.Direction = base.AimDirection;

				List<float> list = new List<float> { };
				for (int e = 0; e < 40; e++)
                {
					list.Add(9 * e);
				}
				list.Shuffle<float>();
				for (int l = 0; l < 20; l++)
				{
					float yah = list[l];
					base.Fire(new Direction(yah, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new CirclesWithOpenings.Yees(30));
				}
				for (int l = 20; l < 40; l++)
				{
					float yah = list[l];
					base.Fire(new Direction(yah, DirectionType.Absolute, -1f), new Speed(9, SpeedType.Absolute), new CirclesWithOpenings.Yees(60));
				}
				yield return this.Wait(75);
			}
			yield break;
		}

		public class Yees : Bullet
		{
			public Yees(float Wait) : base("snakeBullet", false, false, false)
			{
				base.SuppressVfx = true;
				this.Waittime = Wait;
			}
			public override IEnumerator Top()
			{
				float speed = base.Speed;
				base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 30);
				//this.Direction += 180f;
				yield return this.Wait(Waittime);
				base.ChangeSpeed(new Speed(13f, SpeedType.Absolute), 45);
				yield break;
			}
			private float Waittime;

		}
		public class SnakeBullet : Bullet
		{
			public SnakeBullet(int delay) : base("snakeBullet", false, false, false)
			{
				this.delay = delay;
			}
			public override IEnumerator Top()
			{
				base.ManualControl = true;
				yield return base.Wait(this.delay);
				Vector2 truePosition = base.Position;
				for (int i = 0; i < 360; i++)
				{
					float offsetMagnitude = Mathf.SmoothStep(-0.5f, 0.5f, Mathf.PingPong(0.5f + (float)i / 60f * 3f, 1f));
					truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
					base.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude);
					yield return base.Wait(1);
				}
				base.Vanish(false);
				yield break;
			}

			private int delay;
		}
	}


	public class OMEGADEATHSCRIPTOFDOOM : Script
	{
		public override IEnumerator Top()
		{
			base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("da797878d215453abba824ff902e21b4").bulletBank.GetBullet("snakeBullet"));
			base.PostWwiseEvent("Play_BOSS_lichB_spew_01", null);
			Exploder.DoDistortionWave(base.BulletBank.sprite.WorldCenter, 2, 0.08f, 15, 1f);
			base.EndOnBlank = false;
			if (!base.BulletBank.aiActor.healthHaver.IsDead && base.BulletBank.aiActor != null && base.BulletBank.aiActor.healthHaver.GetCurrentHealth() >= 1)
			{
				float i = 0;
				for (; ; )
				{
					base.PostWwiseEvent("Play_ENM_kali_shockwave_01", null);
					this.Direction = base.AimDirection;
					float basevalue = Mathf.Lerp(14, 28, base.BulletBank.aiActor.healthHaver.GetCurrentHealthPercentage());
					if (i > 18)
					{
						basevalue = 12;

					}
					List<float> list = new List<float> { };
					for (int e = 0; e < 30; e++)
					{
						list.Add(12 * e);
					}
					list.Shuffle<float>();
					for (int l = 0; l < 15; l++)
					{
						float yah = list[l];
						base.Fire(new Direction(yah, DirectionType.Absolute, -1f), new Speed(9-(i/9), SpeedType.Absolute), new OMEGADEATHSCRIPTOFDOOM.DelayedBullet(basevalue));
					}
					for (int l = 15; l < 30; l++)
					{
						float yah = list[l];
						base.Fire(new Direction(yah, DirectionType.Absolute, -1f), new Speed(9-(i / 9), SpeedType.Absolute), new OMEGADEATHSCRIPTOFDOOM.DelayedBullet(basevalue * 2));
					}
					yield return this.Wait(basevalue * 2.5f * PlayerStats.GetTotalEnemyProjectileSpeedMultiplier());

					i++;
				}
			}
			
		}
		public class DelayedBullet : Bullet
		{
			public DelayedBullet(float Wait) : base("snakeBullet", false, false, false)
			{
				base.SuppressVfx = true;
				this.Waittime = Wait;
			}
			public override IEnumerator Top()
			{
				float speed = base.Speed;
				base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 30);
				yield return this.Wait(Waittime);
				base.ChangeSpeed(new Speed(speed*1.4f, SpeedType.Absolute), 45);
				yield break;
			}
			private float Waittime;

		}

	}



	public class EnemyBehavior : BraveBehaviour
	{

		private RoomHandler m_StartRoom;
		private bool HasTriggeredDesparation;
		private bool IsActuallyDead;

		private ParticleSystem mehaglow;


		public void Update()
		{

			bool flag = base.aiActor && base.aiActor.healthHaver;
			if (flag)
			{
				if (base.healthHaver.GetCurrentHealth() <= 1 && HasTriggeredDesparation == false)
				{
					HasTriggeredDesparation = true;
					ConvertToDark();
				}

			}

			m_StartRoom = aiActor.GetAbsoluteParentRoom();
			if (!base.aiActor.HasBeenEngaged)
			{
				CheckPlayerRoom();
			}
		}

		private void ConvertToDark()
		{
			StaticReferenceManager.DestroyAllEnemyProjectiles();
			base.aiActor.behaviorSpeculator.Interrupt();
			base.aiActor.healthHaver.IsVulnerable = false;
			for (int j = 0; j < base.aiActor.behaviorSpeculator.AttackBehaviors.Count; j++)
			{
				if (base.behaviorSpeculator.AttackBehaviors[j] is AttackBehaviorGroup && base.behaviorSpeculator.AttackBehaviors[j] != null)
				{
					this.NullifyAllAttacks(base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup);
				}
			}
			GameManager.Instance.StartCoroutine(LoseIFRames());
		}
		private IEnumerator LoseIFRames()
		{
			List<string> Text = new List<string>
			{
				"Ashes to ashes, dust to dust!",
				"You are not welcome here, mortal",
				"I will return you from whence you came!",
				"I will send you screaming into the night!",
				"I can smell your fear."
			};
			TextBoxManager.ShowTextBox(base.aiActor.sprite.WorldTopCenter, base.aiActor.transform, 1.5f, BraveUtility.RandomElement<string>(Text), string.Empty, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
			AkSoundEngine.PostEvent("Play_VO_lichA_chuckle_01", base.gameObject);

			yield return new WaitForSeconds(1f);
			isTelerting = true;
			base.aiAnimator.PlayUntilFinished("TeleportOut", true, null, -1f, false);
			base.aiActor.specRigidbody.CollideWithOthers = false;
			yield return new WaitForSeconds(1f);
			CellArea area = base.aiActor.ParentRoom.area;
			Vector2 Center = area.UnitCenter;
			Vector2 b = base.aiActor.specRigidbody.UnitBottomCenter - base.aiActor.transform.position.XY();
			IntVector2? CentreCell = Center.ToIntVector2();
			base.aiActor.transform.position = Pathfinder.GetClearanceOffset(CentreCell.Value - new IntVector2(2, 0), base.aiActor.Clearance).WithY((float)CentreCell.Value.y) - b;
			base.aiAnimator.PlayUntilFinished("TeleportIn", true, null, -1f, false);
			base.aiActor.specRigidbody.Reinitialize();
			base.aiActor.specRigidbody.CollideWithOthers = true;
			isTelerting = false;
			yield return new WaitForSeconds(1f);

			GameObject thing = base.aiActor.transform.Find("attach").gameObject;
			if (thing == null)
			{
				ETGModConsole.Log("why?");
			}
			thing.gameObject.layer = 23;
			base.aiActor.sprite.HeightOffGround = 0;
			tk2dSprite spriter = thing.GetComponent<tk2dSprite>();//.renderer.enabled = false;
			spriter.sprite.HeightOffGround = -100;
			spriter.sprite.renderer.sortingLayerName = "Foregound";
			spriter.renderer.enabled = false;
			ParticleSystem yes = thing.AddComponent<ParticleSystem>();
			//yes.CopyFrom<ParticleSystem>(particle);
			yes.Play();
			yes.name = "death glow";
			yes.transform.position = thing.transform.position;

			var main = yes.main;

			main.maxParticles = 10000;
			main.playOnAwake = false;
			main.duration = 1;
			main.loop = true;
			main.startLifetime = new ParticleSystem.MinMaxCurve(0.1f, 1f);
			main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 10f);
			main.startSize = new ParticleSystem.MinMaxCurve(0.075f, 0.25f);
			main.startColor = new ParticleSystem.MinMaxGradient(new Color32(255, 141, 0, 255), new Color32(255, 141, 0, 255));
			main.simulationSpace = ParticleSystemSimulationSpace.World;
			main.startRotation = new ParticleSystem.MinMaxCurve(-180, 180);
			main.randomizeRotationDirection = 2;
			main.gravityModifier = -4f;


			var emm = yes.emission;

			var colorOverLifetime = yes.colorOverLifetime;
			colorOverLifetime.enabled = true;
			var brightness = UnityEngine.Random.Range(0.2f, 1);
			var gradient = new Gradient();
			gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 0.9f) }, new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
			colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

			var vOL = yes.velocityOverLifetime;
			vOL.enabled = true;
			vOL.speedModifier = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f)));

			var sc = yes.shape;
			sc.shapeType = ParticleSystemShapeType.Circle;
			sc.radius = 0.1f;

			var tsa = yes.textureSheetAnimation;
			tsa.animation = ParticleSystemAnimationType.SingleRow;
			tsa.numTilesX = 3;
			tsa.numTilesY = 1;
			tsa.enabled = true;
			tsa.cycleCount = 1;
			tsa.frameOverTimeMultiplier = 1.3f;

			var vel = yes.inheritVelocity;

			vel.mode = ParticleSystemInheritVelocityMode.Initial;
			vel.curveMultiplier = 0.9f;

			var sizeOverLifetime = yes.sizeOverLifetime;
			sizeOverLifetime.enabled = true;
			sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0.4f, 1f), new Keyframe(1f, 0f)));


			var sbs = yes.sizeBySpeed;
			sbs.separateAxes = false;
			sbs.sizeMultiplier = 0.9f;
			sbs.size = new ParticleSystem.MinMaxCurve(1, 0);



			var particleRenderer = yes.gameObject.GetComponent<ParticleSystemRenderer>();
			particleRenderer.material = new Material(Shader.Find("Sprites/Default"));
			particleRenderer.material.mainTexture = Shellrax.ShellraxEyeTexture;
			particleRenderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;

			Material sharedMaterial = particleRenderer.sharedMaterial;
			Material material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
			material.SetColor("_EmissiveColor", new Color32(255, 141, 0, 255));
			material.SetFloat("_EmissiveColorPower", 5f);
			material.SetFloat("_EmissivePower", 25f);
			particleRenderer.material = material;

			particleRenderer.sortingLayerName = "Foregound";
			particleRenderer.maskInteraction = SpriteMaskInteraction.None;
			yes.gameObject.layer = 23;

			mehaglow = yes;

			base.aiActor.healthHaver.IsVulnerable = true;
			base.healthHaver.minimumHealth = 0;
			base.aiActor.healthHaver.SetHealthMaximum(base.aiActor.healthHaver.GetMaxHealth() * 3);
			base.healthHaver.FullHeal();
			for (int j = 0; j < base.aiActor.behaviorSpeculator.AttackBehaviors.Count; j++)
			{
				if (base.behaviorSpeculator.AttackBehaviors[j] is AttackBehaviorGroup && base.behaviorSpeculator.AttackBehaviors[j] != null)
				{
					this.WeightDeathLaser(base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup);
				}
			}
			List<string> Anger = new List<string>
			{
				"You little WORM!",
				"I will burn you to ash!",
				"I will tear your sinew from bone!",
			};

			List<string> Pain = new List<string>
			{
				"My pain will turn upon you tenfold!",
				"Agh! That stings!",
				"SUBMIT AND DIE!",
				"Curses upon you!",
				"You can't kill me!",
				"Bones break, but my hatred is eternal!"
			};

			TextBoxManager.ShowTextBox(base.aiActor.sprite.WorldTopCenter, base.aiActor.transform, 1f, BraveUtility.RandomElement<string>(Anger), string.Empty, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
			Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			for (int i = 0; i < 20; i++)
            {
				if (IsActuallyDead != true)
                {
					if (UnityEngine.Random.value <= 0.03f)
					{
						TextBoxManager.ShowTextBox(base.aiActor.sprite.WorldTopCenter, base.aiActor.transform, 1.5f, BraveUtility.RandomElement<string>(Pain), string.Empty, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
					}
					emm.rateOverTime = 15 * i;
					yield return new WaitForSeconds(1f);
					mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
					mat.SetColor("_EmissiveColor", new Color32(255, 141, 0, 255));
					mat.SetFloat("_EmissiveColorPower", 1.55f);
					mat.SetFloat("_EmissivePower", 100 + (10 * i));
					mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f * i);
					base.aiActor.sprite.renderer.material = mat;
					base.aiActor.healthHaver.ApplyDamage(base.healthHaver.GetMaxHealth() / 20, Vector2.zero, "Shellrax fucking dies", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
				}
			}
			yield break;
		}
		private void NullifyAllAttacks(AttackBehaviorGroup attackGroup)
		{
			for (int i = 0; i < attackGroup.AttackBehaviors.Count; i++)
			{
				AttackBehaviorGroup.AttackGroupItem attackGroupItem = attackGroup.AttackBehaviors[i];
				attackGroupItem.Probability = 0;
			}
		}
		private void WeightDeathLaser(AttackBehaviorGroup attackGroup)
		{
			for (int i = 0; i < attackGroup.AttackBehaviors.Count; i++)
			{
				AttackBehaviorGroup.AttackGroupItem attackGroupItem = attackGroup.AttackBehaviors[i];
				if (attackGroupItem.NickName == "Death Laser")
                {
					attackGroupItem.Probability = 100;
				}
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

		private ParticleSystem EyeParticle;

		private ParticleSystem MakeEyePartcile()
        {
			Transform thing = base.aiActor.transform.Find("eye");
			if (thing == null)
			{
				return null;
			}
			thing.gameObject.layer = 23;
			var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("ShellraxEye")); ;//this is the name of the object which by default will be "Particle System"
			partObj.transform.position = thing.transform.position;
			partObj.transform.parent = thing.transform;

			ParticleSystem system = partObj.GetComponent<ParticleSystem>();
			var particleRenderer = system.gameObject.GetComponent<ParticleSystemRenderer>();

			particleRenderer.sortingLayerName = "Foregound";
			particleRenderer.maskInteraction = SpriteMaskInteraction.None;
			particleRenderer.enabled = true;
			particleRenderer.gameObject.layer = 23;

			return system;
        }


		private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
		{
			
			if (clip.GetFrame(frameIdx).eventInfo == "enableparticles")
			{
				if (EyeParticle != null)
                {
					EyeParticle.Play();
				}
			}
			if (clip.GetFrame(frameIdx).eventInfo == "disableparticlesspecial")
			{
				if (EyeParticle != null)
				{
					EyeParticle.Stop();
					var particleRenderer = EyeParticle.gameObject.GetComponent<ParticleSystemRenderer>();
					particleRenderer.enabled = false;
				}
			}
			if (clip.GetFrame(frameIdx).eventInfo == "enableparticlesspecial")
			{
				if (isTelerting != true && EyeParticle != null)
                {
					EyeParticle.Play();

				}
			}
			if (clip.GetFrame(frameIdx).eventInfo == "disableparticles")
			{
				if (EyeParticle!= null)
                {
					EyeParticle.Stop();
				}
				if (mehaglow != null)
				{
					mehaglow.Stop();
				}
			}
			if (clip.GetFrame(frameIdx).eventInfo == "Disablerender")
			{
				SpriteOutlineManager.ToggleOutlineRenderers(base.aiActor.sprite, false);
				base.aiActor.sprite.renderer.enabled = false;
			}
			if (clip.GetFrame(frameIdx).eventInfo == "EnableRender")
			{
				base.aiActor.sprite.renderer.enabled = true;
				SpriteOutlineManager.ToggleOutlineRenderers(base.aiActor.sprite, true);
			}
		}
		public bool isTelerting;




		private void Start()
		{
            EyeParticle = MakeEyePartcile();

            IsActuallyDead = false;
			isTelerting = false;
			base.healthHaver.minimumHealth = 1;
			base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
			StaticReferenceManager.AllHealthHavers.Remove(base.aiActor.healthHaver);
			base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("4d164ba3f62648809a4a82c90fc22cae").bulletBank.GetBullet("big_one"));

			base.aiActor.gameObject.layer = 22;			
			base.aiActor.sprite.HeightOffGround = 0;
			

			if (!base.aiActor.IsBlackPhantom)
			{
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(255, 141, 0, 255));
				mat.SetFloat("_EmissiveColorPower", 1.55f);
				mat.SetFloat("_EmissivePower", 100);
				mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
				base.aiActor.sprite.renderer.material = mat;
			}

			base.aiActor.healthHaver.OnPreDeath += (obj) =>
			{
				IsActuallyDead = true;
				Exploder.DoDistortionWave(base.aiActor.sprite.WorldCenter, 2, 0.15f, 30, 1f);
				List<string> DEATH = new List<string>
				{
					"...to a mortal?... ugh...",
					"No! Not again!",
					"The creeping light...",
					"This is not my end... I will return!",
					"With my final breath, I curse you!"
				};
				TextBoxManager.ShowTextBox(base.aiActor.sprite.WorldTopCenter, base.aiActor.transform, 3, BraveUtility.RandomElement<string>(DEATH), string.Empty, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);		
				AkSoundEngine.PostEvent("Play_VO_lichC_death_01", base.gameObject);
			};
			base.healthHaver.healthHaver.OnDeath += (obj) =>
			{
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.SHELLRAX_DEFEATED, true);//Done
                float itemsToSpawn = UnityEngine.Random.Range(3, 6);
				float spewItemDir = 360 / itemsToSpawn;
				for (int i = 0; i < itemsToSpawn; i++)
				{
					int id = BraveUtility.RandomElement<int>(Shellrax.Lootdrops);
					LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, base.aiActor.sprite.WorldCenter, MathToolbox.GetUnitOnCircle(spewItemDir * i, 2), 2.2f, false, true, false);
				}

				Chest chest2 = GameManager.Instance.RewardManager.SpawnTotallyRandomChest(GameManager.Instance.PrimaryPlayer.CurrentRoom.GetRandomVisibleClearSpot(1, 1));
				chest2.IsLocked = false;
				chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());


			}; ;
			this.aiActor.knockbackDoer.SetImmobile(true, "nope.");

		}


	}

}







