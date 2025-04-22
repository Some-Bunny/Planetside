using Brave.BulletScript;
using Dungeonator;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using Planetside.EnemySetupCode.EnemyRelatedStuff.Behavior_Stuff;
using SaveAPI;
using System;
//using DirectionType = DirectionalAnimation.DirectionType;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static Planetside.AnnihiChamber.BigBall;

namespace Planetside
{
	public class AnnihiChamber : AIActor
	{
		public static GameObject fuckyouprefab;
		public static readonly string guid = "annihichamber";
		public static GameObject shootpoint;
		public static GameObject shootpoint1;

		/*
		public static GameObject Laser1;
		public static GameObject Laser2;
		public static GameObject Laser3;
		public static GameObject Laser4;
		public static GameObject Laser5;
		public static GameObject Laser6;
		*/

		//private static Texture2D BossCardTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside/Resources/BossCards/annihichamber_bosscard.png");
		public static string TargetVFX;

		public static Texture2D BloodParticleTexture;
		public static Texture2D CastTexture;


		public static void Init()
		{
			BloodParticleTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside/Resources2/ParticleTextures/bloodster.png");
			CastTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside/Resources2/ParticleTextures/breakcasts.png");

			AnnihiChamber.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("AnnihiChamberCollection").GetComponent<tk2dSpriteCollectionData>();
			Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("annihichamber material");
			if (fuckyouprefab == null || !BossBuilder.Dictionary.ContainsKey(guid))
			{
				fuckyouprefab = BossBuilder.BuildPrefabBundle("Annihi-Chamber", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(8, 9), false, true);

				var companion = fuckyouprefab.AddComponent<AnnihiChamberBehavior>();
				EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, Collection, mat, false);

				companion.aiActor.knockbackDoer.weight = 200;
				companion.aiActor.MovementSpeed = 1f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.IgnoreForRoomClear = false;
                //companion.aiActor.aiAnimator.HitReactChance = 0.1f;
                //companion.aiActor.aiAnimator.HitType = AIAnimator.HitStateType.Basic;
                //companion.aiActor.aiAnimator.sou

                companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = false;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(450f);
				companion.aiActor.healthHaver.SetHealthMaximum(450f);
				companion.aiActor.SetIsFlying(true, "Gamemode: Creative", true, true);
				companion.aiActor.PathableTiles = CellTypes.FLOOR | CellTypes.PIT;
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.gameObject.AddComponent<AfterImageTrailController>().spawnShadows = false;
				companion.aiActor.gameObject.AddComponent<tk2dSpriteAttachPoint>();
				companion.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();




				companion.aiActor.HasShadow = true;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.massiveShadow, new Vector2(2.1875f, 0.5f), "shadowPos");





				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider

				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 4,
					ManualOffsetY = 7,
					ManualWidth = 58,
					ManualHeight = 58,
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
					ManualOffsetX = 4,
					ManualOffsetY = 7,
					ManualWidth = 58,
					ManualHeight = 58,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				//companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;

				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					AnimNames = new string[]
					{
						"idle_left",
						"idle_right"

					},
					Flipped = new DirectionalAnimation.FlipType[2]
				};

                //tonguestart
                EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "tonguestart", new string[] { "tonguestart" }, new DirectionalAnimation.FlipType[0]);
                EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "burp", new string[] { "burp" }, new DirectionalAnimation.FlipType[0]);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "dashprime", new string[] { "dashprime" }, new DirectionalAnimation.FlipType[0]);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "dashdash", new string[] { "dashdash" }, new DirectionalAnimation.FlipType[0]);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "vomitprime", new string[] { "vomitprime" }, new DirectionalAnimation.FlipType[0]);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "vomit", new string[] { "vomit" }, new DirectionalAnimation.FlipType[0]);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "unvomit", new string[] { "unvomit" }, new DirectionalAnimation.FlipType[0]);

				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "cloakdash_prime", new string[] { "cloakdash_prime" }, new DirectionalAnimation.FlipType[0]);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "cloakdash_charge", new string[] { "cloakdash_charge" }, new DirectionalAnimation.FlipType[0]);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "charge1", new string[] { "charge1" }, new DirectionalAnimation.FlipType[0]);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "attack1", new string[] { "attack1_right", "attack1_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "uncharge1", new string[] { "uncharge1_right", "uncharge1_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "cloak", new string[] { "cloak_right", "cloak_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "cloakidle", new string[] { "cloakidle_right", "cloakidle_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "TeleportOut", new string[] { "TeleportOut" }, new DirectionalAnimation.FlipType[0]);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "TeleportIn", new string[] { "TeleportIn" }, new DirectionalAnimation.FlipType[0]);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "intro", new string[] { "intro" }, new DirectionalAnimation.FlipType[0]);
				EnemyToolbox.AddNewDirectionAnimation(companion.aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[0]);



				{

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6

					}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{


					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14


					}, "burp", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					7,
					8,
					8,
					9,
					9,
					10,
					}, "dashprime", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					7,
					8,
										7,
					8,
										7,
					8,
										7,
					8,
					9,
					10,
					}, "dashdash", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					7,
					8,
					9,
					10,
					9,
					10,
					9,
					10,
					}, "vomitprime", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					7,
					7,
					7,
					8,
					7,
					8,
					8,
					9,
					9,
					10,
					}, "vomit", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					10,
					10,
					9,
					8,
					7,
					7
					}, "unvomit", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					15,
					16,
					17,
					18


					}, "charge1", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3.5f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					19,
					20

					}, "attack1_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					19,
					20

					}, "attack1_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					18,
					17,
					16,
					15


					}, "uncharge1_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3.5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

						18,
					17,
					16,
					15


					}, "uncharge1_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 3.5f;
					//=======================================================================================================================
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					21,
					22,
					23,
					24,
					25,
					26,
					27


					}, "cloak_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					21,
					22,
					23,
					24,
					25,
					26,
					27


					}, "cloak_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					21,
					22,
					22,
					23,
					23,
					23,
					24,
					24,
					24,
					24


					}, "cloakdash_prime", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,
						83,

					}, "cloakdash_charge", tk2dSpriteAnimationClip.WrapMode.Once).fps = 2f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					28,
					29,
					30,
					31,
					32,
					33,
					34
					}, "cloakidle_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					28,
					29,
					30,
					31,
					32,
					33,
					34
					}, "cloakidle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					35,
					36,
					37,
					38,
					39,
					40

					}, "TeleportOut", tk2dSpriteAnimationClip.WrapMode.Once).fps = 9f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
					41,
					42,
					43,
					44,
					45
					}, "TeleportIn", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;


					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				46,
				47,
				48,
				49,
				50,
				51,
				52,
				53,
				0,
				1,
				2,
				3,
				4,
				5,
				6,
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
				66,
				67
					}, "intro", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
                    SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
                    {
				57,
                58,
                59,
                60,
                61,
                62,
                63,
                64,
                65,
                66,
                67
                    }, "tonguestart", tk2dSpriteAnimationClip.WrapMode.Once).fps = 8f;

                    SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
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
				82,
				84,
				85,
				86,
				87,
				88,
				89,
				90,
				91,
				92,
				93,
				94,
				95,
				96,
				97,
				98,
				99,
				100,
				101

					}, "death", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

				}

				//m_BOSS_doormimic_lick_01

				EnemyToolbox.AddSoundsToAnimationFrame(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "tonguestart", new Dictionary<int, string>() { { 1, "Play_BOSS_doormimic_lick_01" } });

                fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[0].eventAudio = "Play_BOSS_doormimic_vanish_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[0].triggerEvent = true;

				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[20].eventAudio = "Play_BOSS_doormimic_lick_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro").frames[20].triggerEvent = true;

				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("dashprime").frames[1].eventAudio = "Play_BOSS_dragun_charge_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("dashprime").frames[1].triggerEvent = true;
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("dashdash").frames[0].eventAudio = "Play_ENM_beholster_intro_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("dashdash").frames[0].triggerEvent = true;

				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("cloakdash_prime").frames[1].eventAudio = "Play_BOSS_dragun_charge_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("cloakdash_prime").frames[1].triggerEvent = true;

				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("cloakdash_charge").frames[0].eventAudio = "Play_ENM_beholster_intro_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("cloakdash_charge").frames[0].triggerEvent = true;

				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("TeleportOut").frames[1].eventAudio = "Play_BOSS_lichA_turn_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("TeleportOut").frames[1].triggerEvent = true;

				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("TeleportIn").frames[1].eventAudio = "Play_BOSS_lichA_turn_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("TeleportIn").frames[1].triggerEvent = true;

				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("vomit").frames[0].eventAudio = "Play_BOSS_doormimic_vomit_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("vomit").frames[0].triggerEvent = true;



				EnemyToolbox.AddSoundsToAnimationFrame(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "death", new Dictionary<int, string>() { { 4, "Play_CHR_shadow_curse_01" }, { 5, "Play_CHR_shadow_curse_01" }, { 6, "Play_CHR_shadow_curse_01" }, { 7, "Play_CHR_shadow_curse_01" }, { 8, "Play_CHR_shadow_curse_01" }, { 9, "Play_CHR_shadow_curse_01" } });


				EnemyToolbox.AddEventTriggersToAnimation(fuckyouprefab.GetComponent<tk2dSpriteAnimator>(), "charge1", new Dictionary<int, string> { { 0, "qlaser" } });//, { 2, "spawnChargelaser" } });

				var intro = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("intro");
				intro.frames[17].eventInfo = "lolwhat";
				intro.frames[17].triggerEvent = true;

				var clip1 = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("dashdash");
				clip1.frames[0].eventInfo = "tempgaintrail";
				clip1.frames[0].triggerEvent = true;

				var clip2 = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("cloakdash_charge");
				clip2.frames[0].eventInfo = "tempgaintrail2";
				clip2.frames[0].triggerEvent = true;

				var cumt1 = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("dashprime");
				cumt1.frames[0].eventInfo = "spawnTell";
				cumt1.frames[0].triggerEvent = true;

				var cumt2 = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("cloakdash_prime");
				cumt2.frames[0].eventInfo = "spawnTell2";
				cumt2.frames[0].triggerEvent = true;


				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("death").frames[28].eventAudio = "Play_BOSS_DragunGold_Baby_Death_01";
				fuckyouprefab.GetComponent<tk2dSpriteAnimator>().GetClipByName("death").frames[28].triggerEvent = true;

				var clip3 = companion.aiActor.GetComponent<tk2dSpriteAnimator>().GetClipByName("death");

				clip3.frames[13].eventInfo = "crecj1";
				clip3.frames[13].triggerEvent = true;

				clip3.frames[15].eventInfo = "crecj2";
				clip3.frames[15].triggerEvent = true;

				clip3.frames[17].eventInfo = "crecj3";
				clip3.frames[17].triggerEvent = true;

				clip3.frames[19].eventInfo = "crecj4";
				clip3.frames[19].triggerEvent = true;

				clip3.frames[21].eventInfo = "crecj5";
				clip3.frames[21].triggerEvent = true;

				clip3.frames[23].eventInfo = "crecj6";
				clip3.frames[23].triggerEvent = true;

				clip3.frames[33].eventInfo = "eat dicks";
				clip3.frames[33].triggerEvent = true;




				var bs = fuckyouprefab.GetComponent<BehaviorSpeculator>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
				shootpoint = new GameObject("attach");
				shootpoint.transform.parent = companion.transform;
				shootpoint.transform.position = companion.sprite.WorldCenter;
				GameObject m_CachedGunAttachPoint = companion.transform.Find("attach").gameObject;


				shootpoint1 = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(2.0625f, 2.375f), "Centre");// = new GameObject("Centre");

				//======================
				var enemy = EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5");
				if (!enemy)
				{
					ETGModConsole.Log("enemy null");
				}
				Projectile beam = null;
				foreach (Component item in enemy.GetComponentsInChildren(typeof(Component)))
				{
					if (item is BossFinalRogueLaserGun laser)
					{
						if (laser.beamProjectile)
						{
							beam = laser.beamProjectile;
							break;
						}
					}
				}
				//====================================

				AIActor actor = EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b");
				BeholsterController beholsterbeam = actor.GetComponent<BeholsterController>();

                GameObject LaserFive = EnemyToolbox.GenerateShootPoint(companion.gameObject, companion.sprite.WorldBottomLeft + new Vector2(3.5625f, 3.0625f), "Laser2"); //TopRight
                AIBeamShooter2 bholsterbeam5 = companion.gameObject.AddComponent<AIBeamShooter2>();
                bholsterbeam5.beamTransform = LaserFive.transform;
                bholsterbeam5.beamModule = beholsterbeam.beamModule;
                bholsterbeam5.beamProjectile = beholsterbeam.projectile;
                bholsterbeam5.firingEllipseCenter = LaserFive.transform.position;
                bholsterbeam5.name = "60";
                bholsterbeam5.northAngleTolerance = 30;//-90 + 240 - 120;

                GameObject LaserFour = EnemyToolbox.GenerateShootPoint(companion.gameObject, companion.sprite.WorldBottomLeft + new Vector2(2.0625f, 3.8125f), "Laser3");//Top//CORRECT
                AIBeamShooter2 bholsterbeam4 = companion.gameObject.AddComponent<AIBeamShooter2>();
                bholsterbeam4.beamTransform = LaserFour.transform;
                bholsterbeam4.beamModule = beholsterbeam.beamModule;
                bholsterbeam4.beamProjectile = beholsterbeam.projectile;
                bholsterbeam4.firingEllipseCenter = LaserFour.transform.position;
                bholsterbeam4.name = "0";
                bholsterbeam4.northAngleTolerance = 90;//-90 +180

                GameObject LaserTwo = EnemyToolbox.GenerateShootPoint(companion.gameObject, companion.sprite.WorldBottomLeft + new Vector2(0.6875f, 3.0625f), "Laser6");//TopLeft
                AIBeamShooter2 bholsterbeam2 = companion.gameObject.AddComponent<AIBeamShooter2>();
                bholsterbeam2.beamTransform = LaserTwo.transform;
                bholsterbeam2.beamModule = beholsterbeam.beamModule;
                bholsterbeam2.beamProjectile = beholsterbeam.projectile;
                bholsterbeam2.firingEllipseCenter = LaserTwo.transform.position;
                bholsterbeam2.name = "300";
                bholsterbeam2.northAngleTolerance = 150;//-108;

                GameObject LaserOne = EnemyToolbox.GenerateShootPoint(companion.gameObject, companion.sprite.WorldBottomLeft + new Vector2(0.6875f, 1.5625f), "Laser4");//BottomLeft
                AIBeamShooter2 bholsterbeam1 = companion.gameObject.AddComponent<AIBeamShooter2>();
                bholsterbeam1.beamTransform = LaserOne.transform;
                bholsterbeam1.beamModule = beholsterbeam.beamModule;
                bholsterbeam1.beamProjectile = beholsterbeam.projectile;
                bholsterbeam1.firingEllipseCenter = LaserOne.transform.position;
                bholsterbeam1.name = "240";
                bholsterbeam1.northAngleTolerance = 210;//-108;


                GameObject LaserThree = EnemyToolbox.GenerateShootPoint(companion.gameObject, companion.sprite.WorldBottomLeft + new Vector2(2.0625f, 0.8125f), "Laser5");//Bottom//CORRECT
                AIBeamShooter2 bholsterbeam3 = companion.gameObject.AddComponent<AIBeamShooter2>();
                bholsterbeam3.beamTransform = LaserThree.transform;
                bholsterbeam3.beamModule = beholsterbeam.beamModule;
                bholsterbeam3.beamProjectile = beholsterbeam.projectile;
                bholsterbeam3.firingEllipseCenter = LaserThree.transform.position;
                bholsterbeam3.name = "180";
                bholsterbeam3.northAngleTolerance = 270;//-108;


                GameObject LaserSix = EnemyToolbox.GenerateShootPoint(companion.gameObject, companion.sprite.WorldBottomLeft + new Vector2(3.5625f, 1.5625f), "Laser1");//BottomRight
                AIBeamShooter2 bholsterbeam6 = companion.gameObject.AddComponent<AIBeamShooter2>();
                bholsterbeam6.beamTransform = LaserSix.transform;
                bholsterbeam6.beamModule = beholsterbeam.beamModule;
                bholsterbeam6.beamProjectile = beholsterbeam.projectile;
                bholsterbeam6.firingEllipseCenter = LaserSix.transform.position;
                bholsterbeam6.name = "120";
                bholsterbeam6.northAngleTolerance = 330;//-90 + 300 + 120



















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

				bs.OverrideBehaviors = new List<OverrideBehaviorBase>
			{
				new ModifySpeedBehavior()
				{
					minSpeed = 1.3f,
					minSpeedDistance = 3,
					maxSpeed = 2.4f,
					maxSpeedDistance = 8
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
					//Phase 1 CHrgeAAAAA
					new AttackBehaviorGroup.AttackGroupItem()//5
                    {
						Probability = 0.7f,
						NickName = "TripleCharge",
						Behavior = new SequentialAttackBehaviorGroup() {
						RunInClass = false,
						AttackBehaviors = new List<AttackBehaviorBase>()
						{

						new ModifiedChargeBehavior()
						{
						InitialCooldown = 1,
						chargeAcceleration = 75,
						chargeSpeed = 45,
						maxChargeDistance = 100,
						bulletScript = new CustomBulletScriptSelector(typeof(ChargeAttack2Attack)),
						ShootPoint = shootpoint1,
                                                endWhenChargeAnimFinishes = false,
                        chargeDamage = 0.5f,
						chargeKnockback = 100,
						collidesWithDodgeRollingPlayers = false,
						primeAnim = "dashprime",
						primeTime = 1f,
						chargeAnim = "dashdash",
						stopDuringPrime = true,
						stoppedByProjectiles = false,
						wallRecoilForce = 100,
						AttackCooldown = 0f,
						Cooldown = 0,
						endOnWallCollision = true
						},
						new ModifiedChargeBehavior()
						{
						InitialCooldown = 0,
						chargeAcceleration = 75,
						chargeSpeed = 45,
						maxChargeDistance = 100,
						bulletScript = new CustomBulletScriptSelector(typeof(ChargeAttack2Attack)),
						ShootPoint = shootpoint1,
                                                endWhenChargeAnimFinishes = false,
                        chargeDamage = 0.5f,
						chargeKnockback = 100,
						collidesWithDodgeRollingPlayers = false,
						primeAnim = "dashprime",
						primeTime = 1f,
						chargeAnim = "dashdash",
						stopDuringPrime = true,
						stoppedByProjectiles = false,
						wallRecoilForce = 100,
						AttackCooldown = 0.5f,
						Cooldown = 9,
						endOnWallCollision = true
						},
						}

						}

					},

                    new AttackBehaviorGroup.AttackGroupItem()//5
                    {
                        Probability = 0.25f,
                        NickName = "Phase 1 CHrgeAAAAA",
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {

                        new ModifiedChargeBehavior()
                        {
                        InitialCooldown = 1,
                        chargeAcceleration = 75,
                        chargeSpeed = 45,
                        maxChargeDistance = 100,
                        bulletScript = new CustomBulletScriptSelector(typeof(ChargeAttack2Attack)),
                                                endWhenChargeAnimFinishes = false,
                        ShootPoint = shootpoint1,
                        chargeDamage = 0.5f,
                        chargeKnockback = 100,
                        collidesWithDodgeRollingPlayers = false,
                        primeAnim = "dashprime",
                        primeTime = 1f,
                        chargeAnim = "dashdash",
                        stopDuringPrime = true,
                        stoppedByProjectiles = false,
                        wallRecoilForce = 100,
                        AttackCooldown = 0f,
                        Cooldown = 0,
						endOnWallCollision = true
                        },
                        new ModifiedChargeBehavior()
                        {
                        InitialCooldown = 0,
                        chargeAcceleration = 75,
                        chargeSpeed = 45,
                        maxChargeDistance = 100,
                        bulletScript = new CustomBulletScriptSelector(typeof(ChargeAttack2Attack)),
                        ShootPoint = shootpoint1,
                        endWhenChargeAnimFinishes = false,

                        chargeDamage = 0.5f,
                        chargeKnockback = 100,
                        collidesWithDodgeRollingPlayers = false,
                        primeAnim = "dashprime",
                        primeTime = 1f,
                        chargeAnim = "dashdash",
                        stopDuringPrime = true,
                        stoppedByProjectiles = false,
                        wallRecoilForce = 100,
                        AttackCooldown = 0.5f,
                        Cooldown = 0,
						endOnWallCollision = true
                        },
						new ModifiedChargeBehavior()
                        {
                        InitialCooldown = 0,
                        chargeAcceleration = 75,
                        chargeSpeed = 45,
                        maxChargeDistance = 100,
                        bulletScript = new CustomBulletScriptSelector(typeof(ChargeAttack2Attack)),
                        ShootPoint = shootpoint1,
                                                endWhenChargeAnimFinishes = false,
                        chargeDamage = 0.5f,
                        chargeKnockback = 100,
                        collidesWithDodgeRollingPlayers = false,
                        primeAnim = "dashprime",
                        primeTime = 1f,
                        chargeAnim = "dashdash",
                        stopDuringPrime = true,
                        stoppedByProjectiles = false,
                        wallRecoilForce = 100,
                        AttackCooldown = 0.5f,
                        Cooldown = 10,
						endOnWallCollision = true
                        },
                        }

                        }

                    },

                    new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 1f,
					Behavior = new CustomDashBehavior{
					//dashAnim = "wail",
					ShootPoint = shootpoint1,
					dashDistance = 19f,
					dashTime = 0.8f,
					AmountOfDashes = 2,
					WaitTimeBetweenDashes = 0f,
					enableShadowTrail = false,
					Cooldown = 4,
					dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
					warpDashAnimLength = true,
					hideShadow = true,
					fireAtDashStart = true,
					InitialCooldown = 2f,
					AttackCooldown = 0f,
					bulletScript = new CustomBulletScriptSelector(typeof(DashAttack)),
					RequiresLineOfSight = false,

					},
						NickName = "Phase 1 Dash"

					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
					Probability = 1f,
					Behavior =new CustomBeholsterLaserBehavior() {

					InitialCooldown = 0f,
					firingTime = 10f,
					AttackCooldown = 2f,
					Cooldown = 12,
					RequiresLineOfSight = true,
					UsesCustomAngle = true,
					RampHeight = 14,
					firingType = CustomBeholsterLaserBehavior.FiringType.ONLY_NORTHANGLEVARIANCE,
					chargeTime = 1.5f,
					UsesBaseSounds = false,
					AdditionalHeightOffset = 10,
					EnemyChargeSound = "Play_ENM_beholster_charging_01",
					LaserFiringSound = "Play_ENM_deathray_shot_01",
					StopLaserFiringSound = "Stop_ENM_deathray_loop_01",
					ChargeAnimation = "charge1",
					FireAnimation = "fire1",
					PostFireAnimation = "uncharge1",
					beamSelection = ShootBeamBehavior.BeamSelection.All,
					trackingType = CustomBeholsterLaserBehavior.TrackingType.ConstantTurn,

					StopDuring = CustomBeholsterLaserBehavior.StopType.Attack,
					LocksFacingDirection = false,

					 DoesSpeedLerp = true,
					InitialStartingSpeed = 0,
					TimeToStayAtZeroSpeedAt = 1f,
					TimeToReachFullSpeed = 1f,
                    
					DoesReverseSpeedLerp = true,
                    EndingSpeed = 0,
                    TimeToReachEndingSpeed = 1,
					

                    maxTurnRate = 40,
					maxUnitForCatchUp = 10f,

					minDegreesForCatchUp = 0,//120,
					minUnitForCatchUp = 2f,
					minUnitForOvershoot = 1,

					turnRateAcceleration = 5,

					unitCatchUpSpeed = 10,
					unitOvershootSpeed = 15,
					unitOvershootTime = 0.75f,

					degreeCatchUpSpeed = 18f,

					useDegreeCatchUp = enemy.transform,
					useUnitCatchUp = true,
					useUnitOvershoot = true,

					ShootPoint = shootpoint.transform,

					
				
					BulletScript = new CustomBulletScriptSelector(typeof(BigBall)),

				},
				NickName = "LASERZ"


					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
					Probability = 1,
					Behavior = new ModifiedChargeBehavior{
						InitialCooldown = 1,
						chargeAcceleration = -1,
						chargeSpeed = 35,
						maxChargeDistance = 100,
						bulletScript = new CustomBulletScriptSelector(typeof(ChargeAttack1Attack)),
						ShootPoint = shootpoint1,
						endWhenChargeAnimFinishes = false,
						chargeDamage = 0.5f,
						chargeKnockback = 100,
						collidesWithDodgeRollingPlayers = false,
						primeAnim = "dashprime",
						primeTime = 1.25f,
						chargeAnim = "dashdash",
						stopDuringPrime = false,
						stoppedByProjectiles = false,
						wallRecoilForce = 50,
						AttackCooldown = 1.5f,
						Cooldown = 3,
						endOnWallCollision = true
					},
					NickName = "Phase 1 CHrge"
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
					Probability = 1f,
					Behavior = new ShootBehavior()
					{
							TellAnimation = "tonguestart",
							FireAnimation = "vomit",
							PostFireAnimation = "unvomit",
							BulletScript = new CustomBulletScriptSelector(typeof(AnnihiChamberTongue)),
							LeadAmount = 0,
							StopDuring = ShootBehavior.StopType.Attack,
							AttackCooldown = 1f,
							Cooldown = 8f,
							RequiresLineOfSight = true,
							ShootPoint = shootpoint1,
							CooldownVariance = 0f,
							GlobalCooldown = 0,
							InitialCooldown = 0,
							InitialCooldownVariance = 0,
							GroupName = null,
							MinRange = 0,
							Range = 1000,
							MinWallDistance = 0,
							MaxEnemiesInRoom = -1,
							MinHealthThreshold = 0,
							MaxHealthThreshold = 1,
							HealthThresholds = new float[0],
							AccumulateHealthThresholds = true,
							targetAreaStyle = null,
							IsBlackPhantom = false,
							resetCooldownOnDamage = null,
							MaxUsages = 0,

					},
					NickName = "Vomit B O N E S"
					},



					new AttackBehaviorGroup.AttackGroupItem()//5
                    {
						Probability = 0f,
						NickName = "TripleFakeout",
						Behavior = new SequentialAttackBehaviorGroup() {
						RunInClass = false,
						AttackBehaviors = new List<AttackBehaviorBase>()
						{
						new CustomDashBehavior{
						ShootPoint = shootpoint1,
						dashDistance = 9f,
						dashTime = 0.75f,
						AmountOfDashes = 1,
						enableShadowTrail = false,
						Cooldown = 0f,
						AttackCooldown = 3f,
						dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
						warpDashAnimLength = true,
						hideShadow = true,
						fireAtDashStart = true,
						InitialCooldown = 0f,
						bulletScript = new CustomBulletScriptSelector(typeof(FakeOut2)),
						Range = 100,
						RequiresLineOfSight = false,
						},
						 
						new CustomDashBehavior{
						ShootPoint = shootpoint1,
						dashDistance = 9f,
						dashTime = 0.75f,
						AmountOfDashes = 1,
						enableShadowTrail = false,
						Cooldown = 0f,
						AttackCooldown = 1f,
						dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
						warpDashAnimLength = true,
						hideShadow = true,
						fireAtDashStart = true,
						InitialCooldown = 0f,

						bulletScript = new CustomBulletScriptSelector(typeof(FakeOut2)),
						Range = 100,
						RequiresLineOfSight = false,
						},
						 new ModifiedChargeBehavior{
						InitialCooldown = 0,
						chargeAcceleration = 60,
						chargeSpeed = 40f,
						maxChargeDistance = 100,
						bulletScript = new CustomBulletScriptSelector(typeof(FakeOutCharge2)),
						ShootPoint = shootpoint1,
						chargeDamage = 0.5f,
						chargeKnockback = 100,
						collidesWithDodgeRollingPlayers = false,
						primeAnim = "cloakdash_prime",
						primeTime = 1f,
						chargeAnim = "cloakdash_charge",
						stopDuringPrime = true,
						stoppedByProjectiles = false,
						Cooldown = 11f,
						AttackCooldown = 0.25f,
						wallRecoilForce = 50,
						Range = 100,
						endOnWallCollision = true

						},

						}
						}

					},



					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0f,
					Behavior = new TeleportBehavior{
					AttackableDuringAnimation = true,
					AllowCrossRoomTeleportation = false,
					teleportRequiresTransparency = false,
					hasOutlinesDuringAnim = true,
					ManuallyDefineRoom = false,
					MaxHealthThreshold = 1f,
					StayOnScreen = true,
					AvoidWalls = true,
					GoneTime = 0.66f,
					OnlyTeleportIfPlayerUnreachable = false,
					MinDistanceFromPlayer = 7f,
					MaxDistanceFromPlayer = 12f,
					teleportInAnim = "TeleportIn",
					teleportOutAnim = "TeleportOut",
					AttackCooldown = 0.25f,
					InitialCooldown = 0f,
					RequiresLineOfSight = false,
					roomMax = new Vector2(0,0),
					roomMin = new Vector2(0,0),
					//teleportInBulletScript = new CustomBulletScriptSelector(typeof(ShellRaxDropCursedAreas)),
					teleportInBulletScript = new CustomBulletScriptSelector(typeof(TeleportOut)),
					GlobalCooldown = 0f,
					Cooldown = 3f,

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
					targetAreaStyle = null,
					HealthThresholds = new float[0],
					MinWallDistance = 0,
					},
					NickName = "ShadePort"
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
					Probability = 0f,
					Behavior = new ModifiedChargeBehavior{
						InitialCooldown = 1,
						chargeAcceleration = -1,
						chargeSpeed = 25f,
						maxChargeDistance = 100,
						bulletScript = new CustomBulletScriptSelector(typeof(FakeOutCharge)),
						ShootPoint = shootpoint1,
						chargeDamage = 0.5f,
						chargeKnockback = 100,
						collidesWithDodgeRollingPlayers = false,
						primeAnim = "cloakdash_prime",
						primeTime = 1f,
						chargeAnim = "cloakdash_charge",
						stopDuringPrime = true,
						stoppedByProjectiles = false,
						Cooldown = 1f,
						AttackCooldown = 0.25f,
						wallRecoilForce = 50,
						Range = 10,
						endOnWallCollision = true
					},
					NickName = "Phase 2 CHrge"
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 0f,
					Behavior = new CustomDashBehavior{
					//dashAnim = "wail",
					ShootPoint = shootpoint1,
					dashDistance = 13f,
					dashTime = 0.45f,
					AmountOfDashes = 1,
					//doubleDashChance = 0,
					enableShadowTrail = false,
					Cooldown = 4f,
					AttackCooldown = 1.5f,
					dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
					warpDashAnimLength = true,
					hideShadow = true,
					fireAtDashStart = true,
					InitialCooldown = 1f,

					bulletScript = new CustomBulletScriptSelector(typeof(FakeOut)),
					//BulletScript = new CustomBulletScriptSelector(typeof(Wail)),
					//LeadAmount = 0f,
					//AttackCooldown = 5f,
					//InitialCooldown = 4f,
					//TellAnimation = "wail",
					//FireAnimation = "wail",
					Range = 13,
					RequiresLineOfSight = false,
					//Uninterruptible = true,
					//FireVfx = ,
					//ChargeVfx = ,
					//	MoveSpeedModifier = 0f,
						},
						NickName = "Phase 2 Dash"

					},
				};


				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:annihi-chamber", companion.aiActor);


				GameObject deathmark = ItemBuilder.AddSpriteToObject("deathmark_vfx", "Planetside/Resources/VFX/ConfusedChamber/confusedchamber1", null);
				FakePrefab.MarkAsFakePrefab(deathmark);
				UnityEngine.Object.DontDestroyOnLoad(deathmark);
				tk2dSpriteAnimator animator = deathmark.GetOrAddComponent<tk2dSpriteAnimator>();
				tk2dSpriteAnimation animation = deathmark.AddComponent<tk2dSpriteAnimation>();

				tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(deathmark, ("Confused_Collection"));

				tk2dSpriteAnimationClip SpawnClip = new tk2dSpriteAnimationClip() { name = "spawn", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
				List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
				for (int i = 1; i < 10; i++)
				{
					tk2dSpriteCollectionData collection = DeathMarkcollection;
					int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/ConfusedChamber/confusedchamber{i}", collection);
					tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
					frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
					frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
				}
				SpawnClip.frames = frames.ToArray();
				SpawnClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
				SpawnClip.loopStart = 11;

				animator.Library = animation;
				animator.Library.clips = new tk2dSpriteAnimationClip[] { SpawnClip };
				animator.DefaultClipId = animator.GetClipIdByName("spawn");
				animator.playAutomatically = true;
				animator.playAutomatically = true;
				animator.ignoreTimeScale = true;
				animator.AlwaysIgnoreTimeScale = true;
				animator.AnimateDuringBossIntros = true;
				ConfusedPrefab = deathmark;
				/*
				GameObject wat = ItemBuilder.AddSpriteToObject("confused", "Planetside/Resources/VFX/ConfusedChamber/confusedchamber1", null);
				FakePrefab.MarkAsFakePrefab(wat);
				UnityEngine.Object.DontDestroyOnLoad(wat);
				tk2dSpriteAnimator yees = wat.AddComponent<tk2dSpriteAnimator>();
				tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
				animationClip.fps = 11;
				animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
				animationClip.name = "start";

				GameObject spriteObject = new GameObject("spriteObject");
				ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/VFX/ConfusedChamber/confusedchamber1", spriteObject);
				tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
				starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
				starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
				tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
				{
				starterFrame
				};
				animationClip.frames = frameArray;
				for (int i = 2; i < 10; i++)
				{
					GameObject spriteForObject = new GameObject("spriteForObject");
					ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/VFX/ConfusedChamber/confusedchamber{i}", spriteForObject);
					tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
					frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
					frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
					animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
				}
				yees.Library = yees.gameObject.AddComponent<tk2dSpriteAnimation>();
				yees.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
				yees.DefaultClipId = yees.GetClipIdByName("start");
				yees.playAutomatically = true;
				yees.ignoreTimeScale = true;
				yees.AlwaysIgnoreTimeScale = true;
				yees.AnimateDuringBossIntros = true;
				ConfusedPrefab = wat;
				*/


				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/AnnihiChamber/annihichamber_idle_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:annihi-chamber";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/AnnihiChamber/annihichamber_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("annihichamberSheet");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\annihichamberSheet.png");
				PlanetsideModule.Strings.Enemies.Set("#ANNIHICHAMBER", "Annihi-Chamber");
				PlanetsideModule.Strings.Enemies.Set("#ANNIHICHAMBER_SHORTDESC", "Six Circles Of Hell");

				List<string> PotentialEntries = new List<string>
				{
					"Corroded, waste casts thrown into the depths of Bullet Hell, and taken over by a parasitic will fueled by six souls. Each eye is a separate voice, with a separate story, yearning for something they now hold dear.\n\nEVERY MOVEMENT, EVERY TWITCH SENDS ME INTO A RAGE. I WOULD TEAR EVERYTHING TO PIECES IF IT WERE NOT FOR THIS LIMITED FORM.\n\n...\n\n...\n\n...\n\n...\n\n...",
					"Corroded, waste casts thrown into the depths of Bullet Hell, and taken over by a parasitic will fueled by six souls. Each eye is a separate voice, with a separate story, yearning for something they now hold dear.\n\n...\n\nI miss him...\n\n...\n\n...\n\n...\n\n...",
					"Corroded, waste casts thrown into the depths of Bullet Hell, and taken over by a parasitic will fueled by six souls. Each eye is a separate voice, with a separate story, yearning for something they now hold dear.\n\n...\n\n...\n\nthis was not the afterlife i studied for this was not the afterlife i studied for this was not the afterlife i studied for this was not the afterlife i studied for\n\n...\n\n...\n\n...",
					"Corroded, waste casts thrown into the depths of Bullet Hell, and taken over by a parasitic will fueled by six souls. Each eye is a separate voice, with a separate story, yearning for something they now hold dear.\n\n...\n\n...\n\n...\n\nMY_MACHINE_SPIRIT_CALLS_FOR_THE_COMFORT_OF_MY_OLD_METAL,_EVEN_IF_MY_PURPOSE_WAS_SELF_SACRIFICE._IN_DEATH,_DO_I_VALUE_MY_OLD_LIFE.\n\n...\n\n...",
					"Corroded, waste casts thrown into the depths of Bullet Hell, and taken over by a parasitic will fueled by six souls. Each eye is a separate voice, with a separate story, yearning for something they now hold dear.\n\n...\n\n...\n\n...\n\n...\n\nTo see the Keep once more, oh how much I'd sacrifice... I hope this damned vessel drags itself there one day... May my old guard comrades slay this vessel so my soul is released to chance a ghostly form...\n\n...",
					"Corroded, waste casts thrown into the depths of Bullet Hell, and taken over by a parasitic will fueled by six souls. Each eye is a separate voice, with a separate story, yearning for something they now hold dear.\n\n...\n\n...\n\n...\n\n...\n\n...\n\nI shouldn't have stepped in...I wanted to help the Master, and paid with my life... Would He have spared his own greatest student... even with his insanity?",
					"THEIR SOULS AS MY EYES. I WILL NOT LET THEM DIE"
				};
				System.Random r = new System.Random();
				int index = r.Next(PotentialEntries.Count);
				string randomString = PotentialEntries[index];

				PlanetsideModule.Strings.Enemies.Set("#ANNIHICHAMBER_LONGDESC", randomString);
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#ANNIHICHAMBER";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#ANNIHICHAMBER_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#ANNIHICHAMBER_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:annihi-chamber");
				EnemyDatabase.GetEntry("psog:annihi-chamber").ForcedPositionInAmmonomicon = 202;
				EnemyDatabase.GetEntry("psog:annihi-chamber").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:annihi-chamber").isNormalEnemy = true;

				GenericIntroDoer miniBossIntroDoer = fuckyouprefab.AddComponent<GenericIntroDoer>();
				fuckyouprefab.AddComponent<AnnihiChamberIntroController>();
				miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
				miniBossIntroDoer.initialDelay = 0.15f;
				miniBossIntroDoer.cameraMoveSpeed = 14;
				miniBossIntroDoer.specifyIntroAiAnimator = null;
				miniBossIntroDoer.BossMusicEvent = "Play_MUS_Boss_Theme_Lich";
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
				PlanetsideModule.Strings.Enemies.Set("#ANNIHICHAMBER_NAME", "ANNIHI-CHAMBER");
				PlanetsideModule.Strings.Enemies.Set("#ANNIHICHAMBER_SHORTNAME", "SIX CIRCLES OF HELL");
				PlanetsideModule.Strings.Enemies.Set("#QUOTE", "");

				miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
				{
					bossNameString = "#ANNIHICHAMBER_NAME",
					bossSubtitleString = "#ANNIHICHAMBER_SHORTNAME",
					bossQuoteString = "#QUOTE",
					bossSpritePxOffset = IntVector2.Zero,
					topLeftTextPxOffset = IntVector2.Zero,
					bottomRightTextPxOffset = IntVector2.Zero,
					bgColor = Color.red
				};
				var BossCardTexture = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("annihichamber_bosscard");
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

				//==================
				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(companion.aiActor.healthHaver);
				//==================


				companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("41ee1c8538e8474a82a74c4aff99c712").bulletBank.GetBullet("big"));
				companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore2"));
				companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("f905765488874846b7ff257ff81d6d0c").bulletBank.GetBullet("spore1"));
				companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").bulletBank.GetBullet("teeth_football"));

				companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("044a9f39712f456597b9762893fbc19c").bulletBank.bulletBank.GetBullet("gross"));
				companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1b5810fafbec445d89921a4efb4e42b7").bulletBank.bulletBank.GetBullet("firehose"));
				companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("463d16121f884984abe759de38418e48").bulletBank.GetBullet("ball"));
				companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("463d16121f884984abe759de38418e48").bulletBank.GetBullet("link"));


                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("9189f46c47564ed588b9108965f975c9").bulletBank.GetBullet("teeth_wave"));

                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"));
                //burst

                Material mat2 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat2.mainTexture = companion.aiAnimator.sprite.renderer.material.mainTexture;
				mat2.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
				mat2.SetFloat("_EmissiveColorPower", 1.55f);
				mat2.SetFloat("_EmissivePower", 100);
				mat2.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
				companion.aiActor.sprite.renderer.material = mat2;


			}


		}
		public static Texture2D annihichambepatircles;
		public static ParticleSystem BloodParticle;

		private static GameObject ConfusedPrefab;


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

		public class AnnihiChamberBehavior : BraveBehaviour
		{

			private RoomHandler m_StartRoom;
			public void Update()
			{

				bool flag = base.aiActor && base.aiActor.healthHaver;
				if (flag)
				{
					if (Phase2AnnihiChamberCheck == false)
					{
						float maxHealth = base.aiActor.healthHaver.GetMaxHealth();
						float num = maxHealth * 0.3f;
						if (base.healthHaver.GetCurrentHealth() <= num && Phase2AnnihiChamberCheck == false)
						{
							ConvertToDark();
						}
						else if (Phase2AnnihiChamberCheck != true && LastStoredMaxHP != maxHealth)
						{
							LastStoredMaxHP = maxHealth;
							base.healthHaver.minimumHealth = num;
						}
					}


				}

			}
			private float LastStoredMaxHP;

			private void ConvertToDark()
			{
				base.aiActor.healthHaver.minimumHealth = 0f;
				StaticReferenceManager.DestroyAllEnemyProjectiles();
				Phase2AnnihiChamberCheck = true;

				base.aiActor.behaviorSpeculator.InterruptAndDisable();
				base.aiActor.aiAnimator.PlayUntilFinished("cloakidle", true, null, -1f, false);
				base.aiActor.aiAnimator.OverrideIdleAnimation = "cloak";
				base.aiActor.aiAnimator.OverrideMoveAnimation = "cloak";
				//base.aiAnimator.PlayUntilFinished("cloakidle_left", true, null, 1, false);
				base.aiActor.healthHaver.IsVulnerable = false;
				foreach (OtherTools.EasyTrailOnEnemy c in base.aiActor.gameObject.GetComponents(typeof(OtherTools.EasyTrailOnEnemy)))
				{
					c.SetMode(UnityEngine.Rendering.ShadowCastingMode.On);
				}
				for (int j = 0; j < base.aiActor.behaviorSpeculator.AttackBehaviors.Count; j++)
				{
					if (base.behaviorSpeculator.AttackBehaviors[j] is AttackBehaviorGroup && base.behaviorSpeculator.AttackBehaviors[j] != null)
					{
						this.ProcessAttackGroup(base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup);
					}
				}
				base.aiActor.MovementSpeed = 2f;
				m_StartRoom.BecomeTerrifyingDarkRoom(2f, 0.5f, 0.1f, "Play_ENM_darken_world_01");
				GameManager.Instance.StartCoroutine(LoseIFRames());
			}

			private IEnumerator LoseIFRames()
			{
				float e = 0;
				float d = 1;
				while (d > e)
				{
					e += BraveTime.DeltaTime;
					yield return null;
				}
				base.aiActor.behaviorSpeculator.enabled = true;
				base.aiActor.healthHaver.IsVulnerable = true;
				yield break;
			}
			private void ProcessAttackGroup(AttackBehaviorGroup attackGroup)
			{
				for (int i = 0; i < attackGroup.AttackBehaviors.Count; i++)
				{
					AttackBehaviorGroup.AttackGroupItem attackGroupItem = attackGroup.AttackBehaviors[i];
					if (attackGroup != null && attackGroupItem.NickName == "Phase 1 Dash")
					{
						attackGroupItem.Probability = 0f;
					}
					if (attackGroup != null && attackGroupItem.NickName == "TripleCharge")
					{
						attackGroupItem.Probability = 0f;
					}
					if (attackGroup != null && attackGroupItem.NickName == "LASERZ")
					{
						attackGroupItem.Probability = 0f;
					}
					if (attackGroup != null && attackGroupItem.NickName == "Vomit B O N E S")
					{
						attackGroupItem.Probability = 0f;
					}
					if (attackGroup != null && attackGroupItem.NickName == "Phase 1 CHrge")
					{
						attackGroupItem.Probability = 0f;
					}
                    if (attackGroup != null && attackGroupItem.NickName == "Phase 1 CHrgeAAAAA")
                    {
                        attackGroupItem.Probability = 0f;
                    }


                    else if (attackGroup != null && attackGroupItem.NickName == "ShadePort")
					{
						attackGroupItem.Probability = 0.7f;
					}
					else if (attackGroup != null && attackGroupItem.NickName == "Phase 2 CHrge")
					{
						attackGroupItem.Probability = 1f;
					}
					else if (attackGroup != null && attackGroupItem.NickName == "TripleFakeout")
					{
						attackGroupItem.Probability = 1.2f;
					}
					else if (attackGroup != null && attackGroupItem.NickName == "Phase 2 Dash")
					{
						attackGroupItem.Probability = 1.1f;
					}//TripleFakeout
				}
			}
			private void Start()
			{
				Phase2AnnihiChamberCheck = false;
                m_StartRoom = aiActor.GetAbsoluteParentRoom();


                float maxHealth = base.aiActor.healthHaver.GetMaxHealth();
				LastStoredMaxHP = maxHealth;
				float num = maxHealth * 0.30f;
				base.healthHaver.minimumHealth = num;


				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
				//firehose
				Phase2AnnihiChamberCheck = false;
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
					m_StartRoom.EndTerrifyingDarkRoom(1f, 0.1f, 1f, "Play_ENM_lighten_world_01");
				};
				base.healthHaver.healthHaver.OnDeath += (obj) =>
				{
					float itemsToSpawn = UnityEngine.Random.Range(2, 6);
					float spewItemDir = 360 / itemsToSpawn;
					for (int i = 0; i < itemsToSpawn; i++)
					{
						int id = BraveUtility.RandomElement<int>(Shellrax.Lootdrops);
						LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, base.aiActor.sprite.WorldCenter, new Vector2((spewItemDir * itemsToSpawn) * i, spewItemDir * itemsToSpawn), 2.2f, false, true, false);
					}

					if (UnityEngine.Random.Range(0.00f, 1.00f) <= 0.4f)
					{
						Chest chest2 = GameManager.Instance.RewardManager.SpawnTotallyRandomChest(GameManager.Instance.PrimaryPlayer.CurrentRoom.GetRandomVisibleClearSpot(1, 1));
						chest2.IsLocked = false;
						chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
					}
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER, true);


				};
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");

			}

			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo == "spawnCharge")
				{
					for (int i = 0; i < base.aiActor.transform.childCount; i++)
					{
						Transform child = base.aiActor.transform.GetChild(i);
						if (child.name.ToLower().Contains("laser"))
						{
							StaticVFXStorage.BeholsterChargeUpVFX.SpawnAtPosition(child.position, 0, child);
						}
					}
				}
				if (clip.GetFrame(frameIdx).eventInfo == "qlaser")
				{
					int e = 0;
					for (int i = 0; i < base.aiActor.transform.childCount; i++)
					{
						Transform child = base.aiActor.transform.GetChild(i);
						if (child.name.ToLower().Contains("laser"))
						{
							e++;
							StaticVFXStorage.BeholsterChargeUpVFX.SpawnAtPosition(child.position, 0, child);
							base.aiActor.StartCoroutine(SpawnDumbassLaserTelegraphTrail(Color.red, "charge1", child, 1, (60 * e)-30));
						}
					}
				}
				if (clip.GetFrame(frameIdx).eventInfo == "lolwhat")
				{
					PlayerController player = GameManager.Instance.PrimaryPlayer;

					if (player.HasGun(ParasiticHeart.HeartID))
					{
						AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.CONFUSED_THE_CHAMBER, true);
						GameObject lightning = base.aiActor.PlayEffectOnActor(ConfusedPrefab, new Vector3(0f, -1f, 0f));
						lightning.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(base.aiActor.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
						lightning.transform.position.WithZ(transform.position.z + 2);
						lightning.GetComponent<tk2dSpriteAnimator>().Play();
					}
				}
				if (clip.GetFrame(frameIdx).eventInfo == "tempgaintrail")
				{
					ImprovedAfterImage yes = base.aiActor.gameObject.AddComponent<ImprovedAfterImage>();
					yes.spawnShadows = true;
					yes.shadowLifetime = 0.5f;
					yes.shadowTimeDelay = 0.001f;
					yes.dashColor = Color.clear;
					yes.name = "Temp Trail";
					yes.sprite.HeightOffGround = 2;

					GameManager.Instance.StartCoroutine(WaitForTrail(base.aiActor));
				}
				if (clip.GetFrame(frameIdx).eventInfo == "tempgaintrail2")
				{
					ImprovedAfterImage yes = base.aiActor.gameObject.AddComponent<ImprovedAfterImage>();
					yes.spawnShadows = true;
					yes.shadowLifetime = 0.5f;
					yes.shadowTimeDelay = 0f;
					yes.dashColor = Color.white;
					yes.name = "Temp Trail2";
					yes.sprite.HeightOffGround = 2;

					GameManager.Instance.StartCoroutine(WaitForTrail(base.aiActor));
				}
				if (clip.GetFrame(frameIdx).eventInfo == "deathboom")
				{
					AkSoundEngine.PostEvent("Play_BigSlam", base.gameObject);
				}

				if (clip.GetFrame(frameIdx).eventInfo == "spawnTell")
				{
					//No, i can think of a better way to do this this as of writing, go fuck yourself if you ask me to change it
					base.aiActor.StartCoroutine(SpawnDumbassTelegraphTrail(Color.red, 90, "dashprime", 1.5f, 0.05f));
                    base.aiActor.StartCoroutine(SpawnDumbassTelegraphTrail(Color.red, -90, "dashprime", 1.5f, 0.05f));
                    base.aiActor.StartCoroutine(SpawnDumbassTelegraphTrail(Color.red, 90, "dashprime", 0.65f, 0.05f));
                    base.aiActor.StartCoroutine(SpawnDumbassTelegraphTrail(Color.red, -90, "dashprime", 0.65f, 0.05f));

				}
				if (clip.GetFrame(frameIdx).eventInfo == "spawnTell2")
				{
					base.aiActor.StartCoroutine(SpawnDumbassTelegraphTrail(Color.white, 90, "cloakdash_prime", 1.5f, 0.1f));
					base.aiActor.StartCoroutine(SpawnDumbassTelegraphTrail(Color.white, -90, "cloakdash_prime", 1.5f, 0.1f));

					base.aiActor.StartCoroutine(SpawnDumbassTelegraphTrail(Color.white, 90, "cloakdash_prime", 0.65f, 0.1f));
					base.aiActor.StartCoroutine(SpawnDumbassTelegraphTrail(Color.white, -90, "cloakdash_prime", 0.65f, 0.1f));
				}

				Vector2[] positions = new Vector2[] { new Vector2(0, 0), new Vector2(0.6875f, 3.0625f), new Vector2(2.0625f, 3.8125f), new Vector2(3.5625f, 3.0625f), new Vector2(3.5625f, 1.5625f), new Vector2(2.0625f, 0.8125f), new Vector2(0.6875f, 1.5625f) };

				string eventInfo = clip.GetFrame(frameIdx).eventInfo;
				Regex reg = new Regex(@"^(crecj)[1-6]+$");
				if (reg.IsMatch(eventInfo))
				{
					int index = int.Parse(eventInfo.Substring(5));

					AkSoundEngine.PostEvent("Play_OBJ_cursepot_shatter_01", base.gameObject);
					MakeParticleSystem(base.aiActor.gameObject, positions[index]);
				}



				if (clip.GetFrame(frameIdx).eventInfo == "eat dicks")
				{

					AkSoundEngine.PostEvent("Play_BOSS_lichB_grab_01", gameObject);

					AkSoundEngine.PostEvent("Play_BOSS_blobulord_burst_01", gameObject);
					GameObject hand = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.hellDragController.HellDragVFX);
					tk2dBaseSprite component1 = hand.GetComponent<tk2dBaseSprite>();
					component1.usesOverrideMaterial = true;
					component1.PlaceAtLocalPositionByAnchor(base.aiActor.specRigidbody.UnitBottomCenter, tk2dBaseSprite.Anchor.LowerCenter);
					component1.renderer.material.shader = ShaderCache.Acquire("Brave/Effects/StencilMasked");

					var pso = new GameObject("NANOMACHINES");
					pso.transform.position = hand.transform.position + new Vector3(8, 2f);
					pso.transform.localRotation = Quaternion.Euler(0f, 0f, 0);
					pso.transform.parent = hand.gameObject.transform;

					var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("BloodSplatter")); ;//this is the name of the object which by default will be "Particle System"
					partObj.transform.position = pso.transform.position;
					partObj.transform.parent = pso.transform;
					partObj.transform.localScale *= 2;


					GameObject portalObj = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX, this.aiActor.sprite.WorldBottomCenter, Quaternion.Euler(0f, 0f, 0f));
					portalObj.layer = this.aiActor.gameObject.layer + (int)GameManager.Instance.MainCameraController.CurrentZOffset;
					portalObj.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
					MeshRenderer mesh = portalObj.GetComponent<MeshRenderer>();
					mesh.material.SetTexture("_PortalTex", StaticTextures.Hell_Drag_Zone_Texture);
					GameManager.Instance.StartCoroutine(PortalDoer(mesh));

					Destroy(partObj, 5);


					Destroy(pso, 5);

					TeleporterPrototypeItem teleporter = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>();
					UnityEngine.Object.Instantiate<GameObject>(teleporter.TelefragVFXPrefab, base.aiActor.sprite.WorldCenter, Quaternion.identity);



				}
			}
			private IEnumerator PortalDoer(MeshRenderer portal)
			{
				float elapsed = 0f;
				while (elapsed < 3)
				{
					elapsed += BraveTime.DeltaTime;
					float t = elapsed / 3;
					if (portal.gameObject == null) { yield break; }
					float throne1 = Mathf.Sin(t * (Mathf.PI));
					portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0, 0.25f, throne1));
					portal.material.SetFloat("_HoleEdgeDepth", Mathf.Lerp(12, 2, throne1));
					yield return null;
				}
				Destroy(portal.gameObject);

				yield break;
			}




			private IEnumerator SpawnDumbassTelegraphTrail(Color telegraphColor, float AngleToMove, string animToWatchFor, float OffsetRadius = 1.5f, float GlowAmplifier = 1)
			{
				float Angle = base.aiActor.FacingDirection;
				GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(base.aiActor.Position.x, base.aiActor.Position.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
				component2.dimensions = new Vector2(1000f, 1f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", telegraphColor);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", telegraphColor);
				float elapsed = 0;
				float Time = 1.25f;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					float t2 = Mathf.Sin(t * (Mathf.PI / 2));
					float H = Mathf.Min(t2 * 1.33f, 1);
					if (!base.aiActor.spriteAnimator.IsPlaying(animToWatchFor))
					{
						Destroy(component2.gameObject);
						yield break;
					}
					if (component2 != null)
					{
						component2.transform.position = base.aiActor.Position + MathToolbox.GetUnitOnCircle(base.aiActor.FacingDirection + AngleToMove, Mathf.Lerp(0, OffsetRadius, H)).ToVector3ZisY();
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * ((250 * GlowAmplifier) * H));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + ((10 * GlowAmplifier) * H));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, base.aiActor.FacingDirection);
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 22;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
						if (elapsed > 0.625f)
						{
                            bool en = elapsed % 0.125 < 0.0625;
                            component2.renderer.enabled = en;
						}
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				if (component2.gameObject != null)
				{
					Destroy(component2.gameObject);
				}
				yield break;
			}
			private IEnumerator SpawnDumbassLaserTelegraphTrail(Color telegraphColor, string animToWatchFor, Transform child, float GlowAmplifier = 0.75f, float additionalOffset = 0)
			{
				float Angle = base.aiActor.FacingDirection + additionalOffset;
				GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
				component2.transform.position = child.position;
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
				component2.dimensions = new Vector2(1000f, 1f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", telegraphColor);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", telegraphColor);
				float elapsed = 0;
				float Time = 1.5f;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					float t2 = Mathf.Sin(t * (Mathf.PI / 2));
					float H = Mathf.Min(t2 * 1.33f, 1);

					if (component2 != null)
					{
						component2.transform.position = child.position;
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * ((250 * GlowAmplifier) * H));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + ((10 * GlowAmplifier) * H));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, additionalOffset);
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 22;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
						if (elapsed > 0.75f)
						{
							bool en = elapsed % 0.15f < 0.075f;
							component2.renderer.enabled = en;
						}
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				if (component2.gameObject != null)
				{
					Destroy(component2.gameObject);
				}
				yield break;
			}


			private IEnumerator WaitForTrail(AIActor actor)
			{
				yield return new WaitForSeconds(2.5f);
				if (actor != null)
				{
					foreach (ImprovedAfterImage c in actor.gameObject.GetComponents<ImprovedAfterImage>())
					{
						if (c.name == "Temp Trail" && c != null)
						{ Destroy(c); }
						if (c.name == "Temp Trail2" && c != null)
						{ Destroy(c); }
					}
				}
				yield break;
			}

			public static GameObject MakeParticleSystem(GameObject transform, Vector3 vector3)
			{
				var pso = new GameObject("CeramicParticles");
				pso.transform.position = transform.transform.position + vector3;
				pso.transform.localRotation = Quaternion.Euler(0f, 0f, 0);
				pso.transform.parent = transform.transform;


				var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("CeramicParticles")); ;//this is the name of the object which by default will be "Particle System"
				partObj.transform.position = pso.transform.position;
				partObj.transform.parent = pso.transform;
				Destroy(pso, 5);


				return pso.gameObject;
			}
			public static bool Phase2AnnihiChamberCheck;
		}





		private static string[] spritePaths = new string[]
		{
			"Planetside/Resources/AnnihiChamber/annihichamber_idle_001.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_idle_002.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_idle_003.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_idle_004.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_idle_005.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_idle_006.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_idle_007.png",

			"Planetside/Resources/AnnihiChamber/annihichamber_burp_001.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_burp_002.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_burp_003.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_burp_004.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_burp_005.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_burp_006.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_burp_007.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_burp_008.png",

			"Planetside/Resources/AnnihiChamber/annihichamber_charge1_001.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_charge1_002.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_charge1_003.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_charge1_004.png",

			"Planetside/Resources/AnnihiChamber/annihichamber_fire1_001.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_fire1_002.png",
			//=======================================================================================================

			"Planetside/Resources/AnnihiChamber/annihichamber_cloakidle_001.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_cloakidle_002.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_cloakidle_003.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_cloakidle_004.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_cloakidle_005.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_cloakidle_006.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_cloakidle_007.png",

			"Planetside/Resources/AnnihiChamber/annihichamber_cloak_001.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_cloak_002.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_cloak_003.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_cloak_004.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_cloak_005.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_cloak_006.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_cloak_007.png",

			"Planetside/Resources/AnnihiChamber/annihichamber_warpout_001.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_warpout_002.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_warpout_003.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_warpout_004.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_warpout_005.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_warpout_006.png",

			"Planetside/Resources/AnnihiChamber/annihichamber_warpin_001.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_warpin_002.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_warpin_003.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_warpin_004.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_warpin_005.png",
			//=======================================================================================================

			"Planetside/Resources/AnnihiChamber/annihichamber_intro_001.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_002.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_003.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_004.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_005.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_006.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_007.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_008.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_009.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_010.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_011.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_012.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_013.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_014.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_015.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_016.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_017.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_018.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_019.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_020.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_021.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_intro_022.png",

			"Planetside/Resources/AnnihiChamber/annihichamber_death_001.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_002.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_003.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_004.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_005.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_006.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_007.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_008.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_009.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_010.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_011.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_012.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_013.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_014.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_015.png",

			"Planetside/Resources/AnnihiChamber/annihichamber_charge_001.png",

			"Planetside/Resources/AnnihiChamber/annihichamber_death_016.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_017.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_018.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_019.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_020.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_021.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_022.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_023.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_024.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_025.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_026.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_027.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_028.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_029.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_030.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_031.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_032.png",
			"Planetside/Resources/AnnihiChamber/annihichamber_death_033.png",




		};


        public class AnnihiChamberTongue : Script
        {
            public override IEnumerator Top()
            {
                //m_BOSS_doormimic_vomit_01
                AkSoundEngine.PostEvent("Play_BOSS_doormimic_vomit_01", this.BulletBank.gameObject);

                base.EndOnBlank = true;
                AnnihiChamberTongue.HandBullet handBullet = null;
                handBullet = this.FireVolley((float)(42.5f));

                while (!handBullet.HasTrulyStopped)
                {
                    yield return base.Wait(1);
                }
                yield break;
            }

            private AnnihiChamberTongue.HandBullet FireVolley(float speed)
            {
                AnnihiChamberTongue.HandBullet handBullet = new AnnihiChamberTongue.HandBullet(this);
                base.Fire(new Direction(base.AimDirection, DirectionType.Absolute, -1f), new Speed(speed, SpeedType.Absolute), handBullet);
                for (int i = 0; i < 40; i++)
                {
                    base.Fire(new Direction(base.AimDirection, DirectionType.Absolute, -1f), new AnnihiChamberTongue.ArmBullet(this, handBullet, i));
                }
                return handBullet;
            }

            private const int NumArmBullets = 20;
            private const int NumVolley = 3;
            private const int FramesBetweenVolleys = 30;

            private class ArmBullet : Bullet
            {
                public ArmBullet(AnnihiChamberTongue parentScript, AnnihiChamberTongue.HandBullet handBullet, int index) : base("firehose", false, false, false)
                {
                    this.m_parentScript = parentScript;
                    this.m_handBullet = handBullet;
                    this.m_index = index;
                }

                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    while (!this.m_parentScript.IsEnded && !this.m_handBullet.IsEnded && !this.m_handBullet.HasStopped && base.BulletBank)
                    {
                        base.Position = Vector2.Lerp(this.m_parentScript.Position, this.m_handBullet.Position, (float)this.m_index / 40f);
                        yield return base.Wait(1);
                    }
                    if (this.m_parentScript.IsEnded)
                    {
                        base.Vanish(false);
                        yield break;
                    }
                    int delay = 20 - this.m_index - 5;
                    if (delay > 0)
                    {
                        yield return base.Wait(delay);
                    }
                    float currentOffset = 0f;
                    int halfWiggleTime = 10;
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 truePosition = Vector2.Lerp(this.m_parentScript.Position, this.m_handBullet.Position, (float)this.m_index / 40f);
                        if (i == 0 && delay < 0)
                        {
                            i = -delay;
                        }
                        float magnitude = 0.4f;
                        magnitude = Mathf.Min(magnitude, Mathf.Lerp(0.2f, 0.4f, (float)this.m_index / 8f));
                        magnitude = Mathf.Min(magnitude, Mathf.Lerp(0.2f, 0.4f, (float)(20 - this.m_index - 1) / 3f));
                        magnitude = Mathf.Lerp(magnitude, 0f, (float)i / (float)halfWiggleTime - 2f);
                        currentOffset = Mathf.SmoothStep(-magnitude, magnitude, Mathf.PingPong(0.5f + (float)i / (float)halfWiggleTime, 1f));
                        base.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, currentOffset);
                        yield return base.Wait(1);
                    }
					while (m_handBullet.HasTrulyStopped == false)
					{
                        base.Position = Vector2.Lerp(this.m_parentScript.Position, this.m_handBullet.Position, (float)this.m_index / 40f);
                        yield return base.Wait(1);
                    }
                    base.Vanish(false);
                    yield break;
                }

                public const int BulletDelay = 60;

                private const float WiggleMagnitude = 0.4f;

                public const int WiggleTime = 30;

                private const int NumBulletsToPreShake = 5;

                private AnnihiChamberTongue m_parentScript;


                private AnnihiChamberTongue.HandBullet m_handBullet;

                private int m_index;
            }

            private class HandBullet : Bullet
            {
                public HandBullet(AnnihiChamberTongue parentScript) : base("firehose", false, false, false)
                {
                    this.m_parentScript = parentScript;
                }

                public bool HasStopped { get; set; }
				public bool HasTrulyStopped = false;

                public override IEnumerator Top()
                {
                    this.Projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
                    this.Projectile.BulletScriptSettings.surviveTileCollisions = true;
                    SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
                    specRigidbody.OnCollision = (Action<CollisionData>)Delegate.Combine(specRigidbody.OnCollision, new Action<CollisionData>(this.OnCollision));
                    while (!this.m_parentScript.IsEnded && !this.HasStopped)
                    {
                        yield return base.Wait(1);
                    }
                    if (this.m_parentScript.IsEnded)
                    {
                        base.Vanish(false);
                        yield break;
                    }
                    yield return base.Wait(200);
                    base.Vanish(false);
                    yield break;
                }

                private void OnCollision(CollisionData collision)
                {
                    AkSoundEngine.PostEvent("Play_BOSS_lichA_stop_01", this.BulletBank.gameObject);

                    bool flag = collision.collisionType == CollisionData.CollisionType.TileMap;
                    SpeculativeRigidbody otherRigidbody = collision.OtherRigidbody;
                    if (otherRigidbody)
                    {
						//flag = (otherRigidbody.majorBreakable || otherRigidbody.PreventPiercing || otherRigidbody.ti || (!otherRigidbody.gameActor && !otherRigidbody.minorBreakable));

						if (otherRigidbody.GetComponent<PlayerController>() != null)
						{
							this.StartTask(DoPullShort());
							base.Position = collision.MyRigidbody.UnitCenter + PhysicsEngine.PixelToUnit(collision.NewPixelsToMove);
							this.Speed = 0f;
							this.HasStopped = true;
							this.Projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;

							PhysicsEngine.PostSliceVelocity = new Vector2?(new Vector2(0f, 0f));
							SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
							specRigidbody.OnCollision = (Action<CollisionData>)Delegate.Remove(specRigidbody.OnCollision, new Action<CollisionData>(this.OnCollision));
							return;
						}
						
                    }
                    if (flag)
                    {
                        this.StartTask(DoPull());

                        base.Position = collision.MyRigidbody.UnitCenter + PhysicsEngine.PixelToUnit(collision.NewPixelsToMove);
                        this.Speed = 0f;
                        this.HasStopped = true;
                        PhysicsEngine.PostSliceVelocity = new Vector2?(new Vector2(0f, 0f));
                        SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
                        specRigidbody.OnCollision = (Action<CollisionData>)Delegate.Remove(specRigidbody.OnCollision, new Action<CollisionData>(this.OnCollision));
                    }
                    else
                    {
                        this.HasStopped = true;
                        this.HasTrulyStopped = true;

                        PhysicsEngine.PostSliceVelocity = new Vector2?(collision.MyRigidbody.Velocity);
                    }
                }

                public IEnumerator DoPullShort()
                {
                    yield return base.Wait(30);
                    float direction = (this.Position - this.m_parentScript.Position).ToAngle();
					Vector2 Self = this.Position;
                    int h = 0;
                    while (Vector2.Distance(this.Position, m_parentScript.Position) > 6)
                    {
                        h++;

						this.Position = Vector2.Lerp(Self, m_parentScript.Position, Vector2.Distance(this.Position, m_parentScript.Position) / Vector2.Distance(Self, m_parentScript.Position) / 50);
                        this.BulletBank.aiActor.specRigidbody.Velocity += MathToolbox.GetUnitOnCircle(direction, (9f + (float)h)/2f);
                        yield return null;
                    }
                    AkSoundEngine.PostEvent("Play_BOSS_lichC_swing_03", this.BulletBank.gameObject);
                    base.Fire(Offset.OverridePosition(this.m_parentScript.Position), new Direction(0, DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new Skull());
                    for (int i = 0; i < 16; i++)
                    {
                        base.Fire(Offset.OverridePosition(this.m_parentScript.Position), new Direction(22.5f * i, DirectionType.Aim, -1f), new Speed(4f, SpeedType.Absolute), new SpeedChangingBullet("teeth_wave", 19, 120));
                        base.Fire(Offset.OverridePosition(this.m_parentScript.Position), new Direction((22.5f * i) + 7.5f, DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new SpeedChangingBullet("teeth_wave", 14, 90));
                    }

                    HasTrulyStopped = true;
                    base.Vanish(false);
                    yield break;
                }
                public class Skull : Bullet
                {
                    public Skull() : base("homing", false, false, true)
                    {
                    }
                }

                public IEnumerator DoPull()
				{
                    yield return base.Wait(60);
                    float direction = (this.Position - this.m_parentScript.Position).ToAngle();

					int h = 0;
					while (Vector2.Distance(this.Position, m_parentScript.Position) > 4)
					{
						h++;
                        this.BulletBank.aiActor.specRigidbody.Velocity += MathToolbox.GetUnitOnCircle(direction, (9 + h) / 1.4f);
						yield return null;
                    }
                    AkSoundEngine.PostEvent("Play_RockBreaking", base.BulletBank.aiActor.gameObject);
                    Exploder.DoDistortionWave(this.m_parentScript.Position, 5, 0.15f, 20, 0.5f);

                    for (int i = 0; i < 20; i++)
                    {
                        base.Fire(Offset.OverridePosition(this.m_parentScript.Position), new Direction(18 * i, DirectionType.Aim, -1f), new Speed(4f, SpeedType.Absolute), new SpeedChangingBullet("teeth_wave", 16, 120));
                        base.Fire(Offset.OverridePosition(this.m_parentScript.Position), new Direction((18 * i)+10f, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), new SpeedChangingBullet("teeth_wave", 24, 90));
                        base.Fire(Offset.OverridePosition(this.m_parentScript.Position), new Direction((18 * i) + 20f, DirectionType.Aim, -1f), new Speed(12f, SpeedType.Absolute), new SpeedChangingBullet("teeth_wave", 5, 180));
                    }



                    HasTrulyStopped = true;
                    base.Vanish(false);
                    yield break;
				}


                public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
                    if (this.Projectile)
                    {
                        SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
                        specRigidbody.OnCollision = (Action<CollisionData>)Delegate.Remove(specRigidbody.OnCollision, new Action<CollisionData>(this.OnCollision));
                    }
                    this.HasStopped = true;
                }

                private AnnihiChamberTongue m_parentScript;
            }
        }


        public class Ballin : Script
		{
			public override IEnumerator Top()
			{


				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").bulletBank.GetBullet("teeth_football"));
				//Adds the Bullet type into the scripts bullet bank. imprtant to have because if you dont have the bullet it will break the script

				//For ints used to create multiple bullets, but instead of starting at 0, i start at -1 to make the script fire from the correct angles much easier
				for (int k = -1; k < 2; k++)
				{
					//Direction adds an additional angle to where-ever it initially fires
					//DirectionType is used to control where the bullet will aim at, and *tehn* add the angle given by direction, so this script will fire bullets towards the player with an anglular offset of -30, 0  and 30
					base.Fire(new Direction(30 * k, DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new Ballin.TootthBullet());
					//The "new Ballin.TootthBullet());" lets you choose what bullet behavior oyu want to use for more advanced stuff by adding additional code to the new Bullet you created, such as making them last a certain amount of time by a given value, or by making them undodgeable
				}
				//Same principle applies as above, only with a check to make it not fire the bullet at the 10 * 0 angle, also the bullet type this party uses is of the UndodgeableBullert set up below
				for (int k = -2; k < 3; k++)
				{
					if (k != 0)
					{
						base.Fire(new Direction(10 * k, DirectionType.Aim, -1f), new Speed(10f, SpeedType.Absolute), new Ballin.UndodgeableBullert());
					}
				}
				yield break;
			}
			public class TootthBullet : Bullet
			{
				//The string with the bullert names are imprtant, if you dont have the bullet you need, it will break
				public TootthBullet() : base("teeth_football", false, false, false)
				{

				}
				public override IEnumerator Top()
				{
					//You can leave this empty as this bullet has no special properties needed
					yield break;
				}
			}
			public class UndodgeableBullert : Bullet
			{
				public UndodgeableBullert() : base("teeth_football", false, false, false)
				{

				}
				public override IEnumerator Top()
				{
					//Add Undodgeable Bullet Code here
					yield break;
				}
			}
		}

		public class DashAttack : Script
		{
			public override IEnumerator Top()
			{

				float floatDirection = base.AimDirection + UnityEngine.Random.Range(30f, -30f);
				Vector2 floatVelocity = BraveMathCollege.DegreesToVector(floatDirection, 5f);
				base.PostWwiseEvent("Play_BOSS_doormimic_vomit_01", null);

				for (int j = 0; j < 12; j++)
				{
					this.Fire(new Direction(this.SubdivideCircle(0, 12, j, 1f, false), DirectionType.Aim, -1f), new Speed(6.5f, SpeedType.Absolute), new DashAttack.BurstBullet(floatVelocity));
				}
                for (int j = 0; j < 8; j++)
                {
                    this.Fire(new Direction(this.SubdivideCircle(0, 8, j, 1f, false), DirectionType.Aim, -1f), new Speed(0.4f, SpeedType.Absolute), new DashAttack.BurstBullet(floatVelocity));
                }

                yield break;
			}
			public class BurstBullet : Bullet
			{
				public BurstBullet(Vector2 additionalVelocity) : base("gross", true, false, false)
				{
					this.m_addtionalVelocity = additionalVelocity;
				}

				public override IEnumerator Top()
				{
					this.ManualControl = true;
					for (int i = 0; i < 300; i++)
					{
						this.UpdateVelocity();
						this.Velocity += this.m_addtionalVelocity * Mathf.Min(10f, (float)i / 60f);
						this.UpdatePosition();
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}

				private Vector2 m_addtionalVelocity;
			}
		}

		public class Spit : Bullet
		{
			public Spit(int delay, Vector2 target, bool shouldHome) : base("gross", false, false, false)
			{
				this.m_delay = delay;
				this.m_target = target;
				this.m_shouldHome = shouldHome;
			}
			public override IEnumerator Top()
			{
				base.ManualControl = true;
				yield return base.Wait(this.m_delay);
				Vector2 truePosition = base.Position;
				for (int i = 0; i < 360; i++)
				{
					float offsetMagnitude = Mathf.SmoothStep(-0.75f, 0.75f, Mathf.PingPong(0.5f + (float)i / 60f * 3f, 1f));
					if (this.m_shouldHome && i > 20 && i < 60)
					{
						float num = (this.m_target - truePosition).ToAngle();
						float value = BraveMathCollege.ClampAngle180(num - this.Direction);
						this.Direction += Mathf.Clamp(value, -6f, 6f);
					}
					truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
					base.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude);
					yield return base.Wait(1);
				}
				base.Vanish(false);
				yield break;
			}
			private int m_delay;
			private Vector2 m_target;
			private bool m_shouldHome;
		}
		public class BigBall : Script
		{
            public override IEnumerator Top()
			{
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        base.Fire(new Direction(45* j, DirectionType.Aim, -1f), new Speed(4f, SpeedType.Absolute), new Homing(7));
                        base.Fire(new Direction((45f * j)+2f, DirectionType.Aim, -1f), new Speed(2f, SpeedType.Absolute), new Homing(10));
                        base.Fire(new Direction((45f * j) - 2f, DirectionType.Aim, -1f), new Speed(2f, SpeedType.Absolute), new Homing(10));
                    }
                    yield return base.Wait(75);
                }
                yield break;
            }

            public class Homing : Bullet
			{
				public float nextValue;
                public Homing(float nextValue) : base("teeth_wave", false, false, true)
                {
                   
                    this.nextValue = nextValue;
                }
                public override IEnumerator Top()
				{
                    yield return base.Wait(15);
                    base.ChangeSpeed(new Speed(nextValue, SpeedType.Absolute), 75);
                    yield return base.Wait(75);
                    yield break;
				}
            }


            public class Superball : Bullet
			{
				public Superball(bool bb) : base("homing", false, false, false)
				{
					b = bb;
				}
				private bool b;
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(17, SpeedType.Absolute), 180);

					base.ChangeDirection(new Brave.BulletScript.Direction(b == true ? 90 : -90, DirectionType.Relative), 120);
					for (int i = 0; i < 300; i++)
					{
						Vector2 Point2 = MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(0.11f, 0.66f));
						string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
						base.Fire(new Offset(Point2), new Direction(UnityEngine.Random.Range(170, 190), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(UnityEngine.Random.Range(0.1f, 2), SpeedType.Absolute), new BigBall.Spore(bankName, 30 + BraveUtility.RandomAngle()));
						yield return this.Wait(3f);

					}
					yield break;
				}
				public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
				{
					if (!preventSpawningProjectiles)
					{
						base.PostWwiseEvent("Play_BOSS_doormimic_vomit_01", null);
						for (int i = 0; i < 32; i++)
						{
							string bankName = (UnityEngine.Random.value > 0.33f) ? "spore2" : "spore1";
							base.Fire(new Direction(UnityEngine.Random.Range(-180, 180), Brave.BulletScript.DirectionType.Relative, -1f), new Speed(UnityEngine.Random.Range(3, 7), SpeedType.Absolute), new BigBall.Spore(bankName, UnityEngine.Random.Range(10, 60)));
						}
						return;
					}
				}
			}


			public class Spore : Bullet
			{
				public Spore(string bulletname, float Airtime) : base(bulletname, false, false, false)
				{
					this.BulletName = bulletname;
					this.AirTime = Airtime;
				}

				public override IEnumerator Top()
				{
					if (this.BulletName == "spore2")
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 120);
					}
					else
					{
						base.ChangeSpeed(new Speed(0, SpeedType.Absolute), 180);
					}
					yield return this.Wait(AirTime);
					base.Vanish(false);
					yield break;
				}
				public string BulletName;
				public float AirTime;
			}


			public class Break : Bullet
			{
				public Break() : base("firehose", false, false, false)
				{

				}
				public override IEnumerator Top()
				{

					yield break;
				}
				public Vector2 centerPoint;
				public bool yesToSpeenOneWay;
				public float startAngle;
				public float SpinSpeed;
			}
		}
		public class ChargeAttack1Attack : Script
		{
			public override IEnumerator Top()
			{
				float fard = base.AimDirection;
				int i = 0;
				bool t = false;
				for (; ; )
				{
					if (i % 4 == 0)
					{
						t = !t;
						Vector2 Point1 = MathToolbox.GetUnitOnCircle(fard + 90, 1.66f);
						Vector2 Point2 = MathToolbox.GetUnitOnCircle(fard - 90, 1.66f);
						int wait = t == true ? 120 : 210;

						base.Fire(new Offset(Point1), new Direction(fard + 95, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new SpeedChangingBullet("teeth_football", 12, wait));
						base.Fire(new Offset(Point2), new Direction(fard - 95, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new SpeedChangingBullet("teeth_football", 12, wait));

                        base.Fire(new Offset(Point1), new Direction(fard + 85, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new SpeedChangingBullet("teeth_football", 9, wait));
                        base.Fire(new Offset(Point2), new Direction(fard - 85, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new SpeedChangingBullet("teeth_football", 9, wait));
                    }
					yield return base.Wait(1);
					i++;
				}
			}
		}

		public class ChargeAttack2Attack : Script
		{
			public override IEnumerator Top()
			{
				float fard = base.AimDirection;
				int i = 0;
				for (; ; )
				{
					if (i % 4 == 0)
					{
						Vector2 Point1 = MathToolbox.GetUnitOnCircle(fard + 90, 1.5f);
						Vector2 Point2 = MathToolbox.GetUnitOnCircle(fard - 90, 1.5f);

						base.Fire(new Offset(Point1), new Direction(fard + 105, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new SpeedChangingBullet("teeth_football", 10, 120));
						base.Fire(new Offset(Point2), new Direction(fard - 105, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new SpeedChangingBullet("teeth_football", 10, 120));


                        base.Fire(new Offset(Point1), new Direction(fard + 75, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new SpeedChangingBullet("teeth_football", 10, 180));
                        base.Fire(new Offset(Point2), new Direction(fard - 75, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new SpeedChangingBullet("teeth_football", 10, 180));
                    }
					yield return base.Wait(1);
					i++;
				}
			}
		}

		public class VomitsGutsAndShit : Script
		{
			public override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				float aim = base.AimDirection;
				for (int i = 0; i < 144; i++)
				{
					float newAim = base.AimDirection;
					aim = Mathf.MoveTowardsAngle(aim, newAim, 1f);
					float t = Mathf.PingPong((float)i / 45f, 1f);
					Bullet bullet;
					bullet = new VomitsGutsAndShit.FirehoseBullet((float)((t >= 0.1f) ? 1 : -1));
					base.Fire(new Offset(MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(0.125f, 0.75f)), 0f, string.Empty, DirectionType.Absolute), new Direction(aim + Mathf.SmoothStep(-50f, 50f, t), DirectionType.Absolute, -1f), new Speed(6.5f, SpeedType.Absolute), bullet);
					base.Fire(new Offset(MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(0f, 0.25f)), 0f, string.Empty, DirectionType.Absolute), new Direction(aim + Mathf.SmoothStep(-50f, 50f, t), DirectionType.Absolute, -1f), new Speed(6.5f, SpeedType.Absolute), bullet);

					if (i % 8 == 0)
					{
						float Dir = UnityEngine.Random.Range(-120, 120);
						float fard = UnityEngine.Random.Range(1, 4);
						int BoneLength = UnityEngine.Random.Range(2, 5);
						if (fard == 1)
						{
							bool yeah = (UnityEngine.Random.value > 0.5f) ? false : true;
							base.Fire(new Direction(Dir, DirectionType.Aim, -1f), new Speed(10, SpeedType.Absolute), new VomitsGutsAndShit.HelixBullet(yeah, 0, "ball"));
							for (int e = 0; e < BoneLength; e++)
							{
								base.Fire(new Direction(Dir, DirectionType.Aim, -1f), new Speed(10, SpeedType.Absolute), new VomitsGutsAndShit.HelixBullet(yeah, (6 * e) + 6, "link"));
							}
						}
					}
					yield return base.Wait(2);
				}
				yield break;
			}

			public class HelixBullet : Bullet
			{
				public HelixBullet(bool reverse, int delay, string BulletType) : base(BulletType, false, false, false)
				{
					this.reverse = reverse;
					this.Delay = delay;
					this.bullettype = BulletType;
					base.SuppressVfx = true;
				}
				public override IEnumerator Top()
				{
					this.ManualControl = true;
					yield return base.Wait(this.Delay);
					Vector2 truePosition = this.Position;
					float startVal = 1;
					for (int i = 0; i < 360; i++)
					{
						float offsetMagnitude = Mathf.SmoothStep(-.75f, .75f, Mathf.PingPong(startVal + (float)i / 90f * 3f, 1f));
						truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 90f);
						this.Position = truePosition + (this.reverse ? BraveMathCollege.DegreesToVector(this.Direction + 90f, offsetMagnitude) : BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude));
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}
				private bool reverse;
				private int Delay;
				private string bullettype;
			}

			private void QuadShot(float direction, float offset, float speed)
			{
				for (int i = 0; i < 4; i++)
				{
					base.Fire(new Offset(offset, 0f, direction, string.Empty, DirectionType.Absolute), new Direction(direction, DirectionType.Absolute, -1f), new Speed(speed - (float)i * 1.5f, SpeedType.Absolute), new TheFart(speed, 120, -1));
				}
			}

			public class TheFart : Bullet
			{
				public TheFart(float newSpeed, int term, int destroyTimer = -1) : base("gross", false, false, false)
				{
					this.m_newSpeed = newSpeed;
					this.m_term = term;
					this.m_destroyTimer = destroyTimer;
				}

				public TheFart(string name, float newSpeed, int term, int destroyTimer = -1, bool suppressVfx = false) : base(name, suppressVfx, false, false)
				{
					this.m_newSpeed = newSpeed;
					this.m_term = term;
					this.m_destroyTimer = destroyTimer;
				}

				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(this.m_newSpeed, SpeedType.Absolute), this.m_term);
					if (this.m_destroyTimer < 0)
					{
						yield break;
					}
					yield return base.Wait(this.m_term + this.m_destroyTimer);
					base.Vanish(false);
					yield break;
				}

				private float m_newSpeed;

				private int m_term;
				private int m_destroyTimer;
			}
			public class FirehoseBullet : Bullet
			{
				public FirehoseBullet(float direction) : base("firehose", false, false, false)
				{
					this.m_direction = direction;
				}

				public override IEnumerator Top()
				{
					yield return base.Wait(UnityEngine.Random.Range(5, 30));
					this.Direction += this.m_direction * UnityEngine.Random.Range(10f, 25f);
					yield break;
				}
				private float m_direction;
			}

			public class CrossBullet : Bullet
			{
				public CrossBullet(Vector2 offset, int setupDelay, int setupTime) : base("default", false, false, false)
				{
					this.m_offset = offset;
					this.m_setupDelay = setupDelay;
					this.m_setupTime = setupTime;
				}
				public override IEnumerator Top()
				{
					base.ManualControl = true;
					this.m_offset = this.m_offset.Rotate(this.Direction);
					for (int i = 0; i < 360; i++)
					{
						if (i > this.m_setupDelay && i < this.m_setupDelay + this.m_setupTime)
						{
							base.Position += this.m_offset / (float)this.m_setupTime;
						}
						float speed = base.Speed;
						base.Position += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}

				private Vector2 m_offset;
				private int m_setupDelay;
				private int m_setupTime;
			}
		}
		public class FakeOut : Script
		{
			public override IEnumerator Top()
			{
				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				//base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("suck"));
				yield return base.Wait(3);
				Vector2 vel = base.BulletBank.aiActor.Velocity;
				float Angle = vel.ToAngle() - 180;

				//float num = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.Ground) - this.m_aiActor.specRigidbody.UnitCenter).ToAngle();
				//float[] array = new float[]
				//	{
				//num + 90f,
				//num - 90f
				//};
				//BraveUtility.RandomizeArray<float>(array, 0, -1);


				//float AddOn = UnityEngine.Random.value > 0.5f ? 50 : -50;
				float newAim = Angle;
				base.Fire(new Offset(0, 1.5f), new Direction(newAim, DirectionType.Aim, -1f), new Speed(20, SpeedType.Absolute), new FakeOut.Faker(this));
				base.Fire(new Offset(0, -1.5f), new Direction(newAim, DirectionType.Aim, -1f), new Speed(20, SpeedType.Absolute), new FakeOut.Faker(this));
				base.Fire(new Offset(-1.4375f, 0.75f), new Direction(newAim, DirectionType.Aim, -1f), new Speed(20, SpeedType.Absolute), new FakeOut.Faker(this));
				base.Fire(new Offset(1.4375f, 0.75f), new Direction(newAim, DirectionType.Aim, -1f), new Speed(20, SpeedType.Absolute), new FakeOut.Faker(this));
				base.Fire(new Offset(-1.4375f, -0.75f), new Direction(newAim, DirectionType.Aim, -1f), new Speed(20, SpeedType.Absolute), new FakeOut.Faker(this));
				base.Fire(new Offset(1.4375f, -0.75f), new Direction(newAim, DirectionType.Aim, -1f), new Speed(20, SpeedType.Absolute), new FakeOut.Faker(this));


				yield return base.Wait(30);
				this.aimDirection = (this.BulletManager.PlayerPosition() - base.Position).ToAngle();
				yield break;
			}
			public float aimDirection;
			public class Faker : Bullet
			{
				public Faker(FakeOut parent) : base("spore2", false, false, false)
				{
					this.parent = parent;
				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 42);
					yield return base.Wait(42);
					yield return base.Wait(UnityEngine.Random.Range(30, 180));
					base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);
					float rand = UnityEngine.Random.Range(-180, 180);
					for (int i = 0; i < 6; i++)
					{
						base.Fire(new Direction((60 * i) + rand, DirectionType.Aim, -1f), new Speed(0, SpeedType.Absolute), new FakeOut.LolOk());
					}
					base.Vanish(false);
				}
				private FakeOut parent;
			}
			public class LolOk : Bullet
			{
				public LolOk() : base("spore2", false, false, false)
				{
				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(14f, SpeedType.Absolute), 120);
					yield break;
				}
			}
		}

		public class FakeOut2 : Script
		{
			public override IEnumerator Top()
			{
				base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
				//base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("5729c8b5ffa7415bb3d01205663a33ef").bulletBank.GetBullet("suck"));
				yield return base.Wait(3);
				Vector2 vel = base.BulletBank.aiActor.Velocity;
				float Angle = vel.ToAngle() - 180;

				//float num = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.Ground) - this.m_aiActor.specRigidbody.UnitCenter).ToAngle();
				//float[] array = new float[]
				//	{
				//num + 90f,
				//num - 90f
				//};
				//BraveUtility.RandomizeArray<float>(array, 0, -1);


				//float AddOn = UnityEngine.Random.value > 0.5f ? 50 : -50;
				float newAim = Angle;
				base.Fire(new Offset(0, 1.5f), new Direction(newAim, DirectionType.Aim, -1f), new Speed(20, SpeedType.Absolute), new FakeOut2.Faker(this));
				base.Fire(new Offset(0, -1.5f), new Direction(newAim, DirectionType.Aim, -1f), new Speed(20, SpeedType.Absolute), new FakeOut2.Faker(this));
				base.Fire(new Offset(-1.4375f, 0.75f), new Direction(newAim, DirectionType.Aim, -1f), new Speed(20, SpeedType.Absolute), new FakeOut2.Faker(this));
				base.Fire(new Offset(1.4375f, 0.75f), new Direction(newAim, DirectionType.Aim, -1f), new Speed(20, SpeedType.Absolute), new FakeOut2.Faker(this));
				base.Fire(new Offset(-1.4375f, -0.75f), new Direction(newAim, DirectionType.Aim, -1f), new Speed(20, SpeedType.Absolute), new FakeOut2.Faker(this));
				base.Fire(new Offset(1.4375f, -0.75f), new Direction(newAim, DirectionType.Aim, -1f), new Speed(20, SpeedType.Absolute), new FakeOut2.Faker(this));


				yield return base.Wait(30);
				this.aimDirection = (this.BulletManager.PlayerPosition() - base.Position).ToAngle();
				yield break;
			}
			public float aimDirection;
			public class Faker : Bullet
			{
				public Faker(FakeOut2 parent) : base("spore2", false, false, false)
				{
					this.parent = parent;
				}
				public override IEnumerator Top()
				{
                    yield return base.Wait(20);
                    base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 10);
					yield return base.Wait(30);
					yield return base.Wait(UnityEngine.Random.Range(60, 270));
					base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);
					float rand = UnityEngine.Random.Range(-180, 180);
					for (int i = 0; i < 6; i++)
					{
						base.Fire(new Direction((60 * i) + rand, DirectionType.Aim, -1f), new Speed(0, SpeedType.Absolute), new FakeOut2.LolOk());
					}
					base.Vanish(false);
				}
				private FakeOut2 parent;
			}
			public class LolOk : Bullet
			{
				public LolOk() : base("spore2", false, false, false)
				{
				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(14f, SpeedType.Absolute), 120);
					yield break;
				}
			}
		}

		public class FakeOutCharge : Script
		{
			public override IEnumerator Top()
			{
				float newAim = base.AimDirection + 180;
				int i = 0;
				for (; ; )
				{

					if (i % 20 == 1)
					{
						float fard = base.AimDirection;
						base.Fire(new Offset(0, 1.5f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new FakeOutCharge.LolOk());
						base.Fire(new Offset(0, -1.5f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new FakeOutCharge.LolOk());
						base.Fire(new Offset(-1.4375f, 0.75f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new FakeOutCharge.LolOk());
						base.Fire(new Offset(1.4375f, 0.75f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new FakeOutCharge.LolOk());
						base.Fire(new Offset(-1.4375f, -0.75f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new FakeOutCharge.LolOk());
						base.Fire(new Offset(1.4375f, -0.75f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new FakeOutCharge.LolOk());
					}
					yield return base.Wait(1);
					i++;
				}
			}
			public float aimDirection;
			public class Faker : Bullet
			{
				public Faker(FakeOutCharge parent) : base("spore2", false, false, false)
				{
					this.parent = parent;
				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 90);
					yield return base.Wait(150);
					base.Vanish(false);
				}
				private FakeOutCharge parent;
			}
			public class LolOk : Bullet
			{
				public LolOk() : base("spore2", false, false, false)
				{
				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(10f, SpeedType.Absolute), 120);
					yield break;
				}
			}
		}
		public class FakeOutCharge2 : Script
		{
			public override IEnumerator Top()
			{
				float newAim = base.AimDirection + 180;
				int i = 0;
				for (; ; )
				{

					if (i % 45 == 1)
					{
						float fard = base.AimDirection;
						base.Fire(new Offset(0, 1.5f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new FakeOutCharge.LolOk());
						base.Fire(new Offset(0, -1.5f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new FakeOutCharge.LolOk());
						base.Fire(new Offset(-1.4375f, 0.75f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new FakeOutCharge.LolOk());
						base.Fire(new Offset(1.4375f, 0.75f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new FakeOutCharge.LolOk());
						base.Fire(new Offset(-1.4375f, -0.75f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new FakeOutCharge.LolOk());
						base.Fire(new Offset(1.4375f, -0.75f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new FakeOutCharge.LolOk());
					}
					yield return base.Wait(1);
					i++;
				}
			}
			public float aimDirection;
			public class Faker : Bullet
			{
				public Faker(FakeOutCharge parent) : base("spore2", false, false, false)
				{
					this.parent = parent;
				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 90);
					yield return base.Wait(150);
					base.Vanish(false);
				}
				private FakeOutCharge parent;
			}
			public class LolOk : Bullet
			{
				public LolOk() : base("spore2", false, false, false)
				{
				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(10f, SpeedType.Absolute), 120);
					yield break;
				}
			}
		}

		public class TeleportOut : Script
		{
			public override IEnumerator Top()
			{
				float fard = base.AimDirection;
				base.Fire(new Offset(0, 1.5f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new TeleportOut.LolOk());
				base.Fire(new Offset(0, -1.5f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new TeleportOut.LolOk());
				base.Fire(new Offset(-1.4375f, 0.75f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new TeleportOut.LolOk());
				base.Fire(new Offset(1.4375f, 0.75f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new TeleportOut.LolOk());
				base.Fire(new Offset(-1.4375f, -0.75f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new TeleportOut.LolOk());
				base.Fire(new Offset(1.4375f, -0.75f), new Direction(fard, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new TeleportOut.LolOk());
				yield break;

			}
			public float aimDirection;
			public class Faker : Bullet
			{
				public Faker(FakeOutCharge parent) : base("spore2", false, false, false)
				{
					this.parent = parent;
				}
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 0);
					yield return base.Wait(150);
					base.Vanish(false);
				}
				private FakeOutCharge parent;
			}
			public class LolOk : Bullet
			{
				public LolOk() : base("spore2", false, false, false)
				{
				}
				public override IEnumerator Top()
				{
                    yield return base.Wait(60);
                    base.ChangeSpeed(new Speed(16f, SpeedType.Absolute), 120);
					yield break;
				}
			}
		}

	}
}







