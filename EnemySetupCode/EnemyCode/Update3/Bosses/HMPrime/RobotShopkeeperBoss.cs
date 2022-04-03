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
using NpcApi;

using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Reflection;

using System.Text;


namespace Planetside
{
	public class RobotShopkeeperEngageDoer : CustomEngageDoer
	{
		public void Awake()
		{
			this.aiActor.gameObject.GetComponent<GenericIntroDoer>().enabled = false;
			this.aiActor.enabled = false;
			this.aiActor.CollisionDamage = 0;
			this.behaviorSpeculator.enabled = false;
			this.specRigidbody.enabled = true;
			this.aiActor.IgnoreForRoomClear = true;
			this.AllowedToDoIntro = true;
			this.AmountOfPurchases = 0;
			this.startedIntro = false;
		}
		private HeatIndicatorController ringObject;
		public CustomShopController shopToSellOut;

		public void Start()
        {
			this.aiActor.gameObject.GetComponent<GenericIntroDoer>().enabled = false;
			this.aiActor.enabled = false;
			this.aiActor.CollisionDamage = 0;
			this.behaviorSpeculator.enabled = false;
			this.specRigidbody.enabled = true;
			this.aiActor.IgnoreForRoomClear = true;
			this.AllowedToDoIntro = true;

			this.AmountOfPurchases = 0;
			this.aiActor.aiAnimator.OverrideIdleAnimation = "broken";
			//this.aiActor.aiAnimator.OverrideMoveAnimation = "broken";
			this.aiActor.aiAnimator.EndAnimation();

		}
		public void IncrenemtScaling()
        {
			if (AmountOfPurchases == 0)
            {
				Minimap.Instance.TemporarilyPreventMinimap = true;
				GameManager.Instance.StartCoroutine(SpawnRing(this.aiActor.sprite.WorldCenter));
				RoomHandler currentRoom = this.aiActor.GetAbsoluteParentRoom();
				currentRoom.SealRoom();
			}
			AmountOfPurchases++;
        }
		public float AmountOfPurchases;
		public void DoStartIntro()
		{
			base.StartCoroutine(this.DoIntro());
		}


		public void Update()
		{
			if (GameManager.Instance.BestActivePlayer != null && GameManager.Instance.IsLoadingLevel == false)
            {
				if (AmountOfPurchases > 0 && AllowedToDoIntro == true && Vector2.Distance(base.aiActor.sprite.WorldCenter, GameManager.Instance.BestActivePlayer.transform.PositionVector2()) > 5.5f)
				{
					base.StartCoroutine(this.DoIntro());
					this.AllowedToDoIntro = false;
				}
			}	
		
		}
		public override void StartIntro()
		{}
		private IEnumerator SpawnRing(Vector2 centre)
		{
			float elapsed = 0f;
			float duration = 3f;
			ringObject = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), centre, Quaternion.identity)).GetComponent<HeatIndicatorController>();
			ringObject.IsFire = false;
			ringObject.CurrentRadius = 0;
			while (elapsed < duration)
			{
				if (ringObject.gameObject == null) { break; }
				if (startedIntro == true) { break; }
				elapsed += BraveTime.DeltaTime;
				float t = elapsed / duration;
				float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
				ringObject.CurrentRadius = Mathf.Lerp(0, 5f, throne1);
				ringObject.CurrentColor = Color.green.WithAlpha(Mathf.Lerp(25, 75, throne1));
				yield return null;
			}
			yield break;
		}

		private IEnumerator DestroyRing()
		{
			float elapsed = 0f;
			float duration = 2f;
			float RingSize = ringObject.CurrentRadius;
			while (elapsed < duration)
			{
				if (ringObject.gameObject == null) { break; }
				elapsed += BraveTime.DeltaTime;
				float t = elapsed / duration;
				float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
				ringObject.CurrentRadius = Mathf.Lerp(RingSize, 0f, throne1);
				ringObject.CurrentColor = Color.green.WithAlpha(Mathf.Lerp(75, 10, throne1));
				yield return null;
			}
			if (ringObject != null)
            {
				Destroy(ringObject.gameObject);
            }
			elapsed = 0f;
			duration = 4.5f;
			while (elapsed < duration)
			{elapsed += BraveTime.DeltaTime;}
			if (shopToSellOut != null)
			{
				Destroy(shopToSellOut);
			}
			if (this.gameObject.GetComponent<TalkDoerLite>() != null)
			{
				this.gameObject.GetComponent<TalkDoerLite>().placeableWidth = 0;
				this.gameObject.GetComponent<TalkDoerLite>().placeableHeight = 0;
				this.gameObject.GetComponent<TalkDoerLite>().enabled = false;
			}
			if (this.gameObject.GetComponent<PlayMakerFSM>() != null)
			{
				this.gameObject.GetComponent<PlayMakerFSM>().enabled = false;
			}
			if (this.gameObject.GetComponent<CustomShopController>() != null)
			{
				this.gameObject.GetComponent<CustomShopController>().enabled = false;

			}
			if (ringObject.gameObject != null)
			{Destroy(ringObject.gameObject);}


			m_isFinished = false;
			this.specRigidbody.enabled = false;
			this.aiActor.IgnoreForRoomClear = true;
			this.aiActor.ToggleRenderers(false);

			this.aiActor.healthHaver.PreventAllDamage = true;
			this.aiActor.enabled = true;
			this.aiActor.specRigidbody.enabled = true;
			this.aiActor.IsGone = false;
			this.aiActor.IgnoreForRoomClear = false;
			this.aiActor.ToggleRenderers(true);
			this.aiActor.State = AIActor.ActorState.Awakening;
			int playerMask = CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.PlayerHitBox);
			this.aiActor.specRigidbody.AddCollisionLayerIgnoreOverride(playerMask);

			this.aiActor.aiAnimator.OverrideIdleAnimation = null;
			this.aiActor.aiAnimator.OverrideMoveAnimation = null;

			this.aiActor.healthHaver.PreventAllDamage = false;
			this.aiActor.behaviorSpeculator.enabled = true;

			this.aiActor.specRigidbody.RemoveCollisionLayerIgnoreOverride(playerMask);
			this.aiActor.HasBeenEngaged = true;
			this.aiActor.State = AIActor.ActorState.Normal;
			UltraFortunesFavor shield = this.aiActor.gameObject.GetComponent<UltraFortunesFavor>();
			Destroy(shield);



			GameManager.Instance.PreventPausing = false;
			this.aiActor.gameObject.GetComponent<GenericIntroDoer>().enabled = true;
			this.aiActor.gameObject.GetComponent<GenericIntroDoer>().TriggerSequence(GameManager.Instance.BestActivePlayer);
			this.aiActor.IsGone = false;
			yield break;
		}


		private IEnumerator DestroyRemaningItems()
        {

			this.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
			if (shopToSellOut != null)
			{
				FieldInfo _itemControllers = typeof(BaseShopController).GetField("m_itemControllers", BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (ShopItemController item in _itemControllers.GetValue(shopToSellOut as BaseShopController) as List<ShopItemController>)
				{
					if (item != null)
					{
						LootEngine.DoDefaultItemPoof(item.transform.PositionVector2());
						item.ForceOutOfStock();
						Destroy(item.gameObject);
					}
					yield return new WaitForSeconds(0.5f);
				}
			}
			yield break;
        }


		private IEnumerator DoIntro()
		{
			startedIntro = true;
			StaticReferenceManager.DestroyAllEnemyProjectiles();
			Minimap.Instance.ToggleMinimap(false, false);
			for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
			{
				if (GameManager.Instance.AllPlayers[j])
				{
					GameManager.Instance.AllPlayers[j].SetInputOverride("BossIntro");
				}
			}
			GameManager.Instance.PreventPausing = true;
			GameUIRoot.Instance.HideCoreUI(string.Empty);
			GameUIRoot.Instance.ToggleLowerPanels(false, false, string.Empty);
			CameraController m_camera = GameManager.Instance.MainCameraController;
			m_camera.StopTrackingPlayer();
			m_camera.SetManualControl(true, false);
			m_camera.OverridePosition = m_camera.transform.position;

			GameManager.Instance.StartCoroutine(DestroyRemaningItems());
			GameManager.Instance.StartCoroutine(DestroyRing());


			

			this.StartIntro();
			m_isFinished = true;
			yield break;
		}
		public override bool IsFinished
		{
			get
			{
				return this.m_isFinished;
			}
		}
		private bool startedIntro;

		public bool AllowedToDoIntro;
		private bool m_isFinished;
	}


	public class RobotShopkeeperBoss : AIActor
	{
		public static GameObject robotShopkeeperprefab;
		public static readonly string guid = "RobotShopkeeperBoss";
		private static tk2dSpriteCollectionData RobotShopkeeperCollection;
		public static GameObject shootpoint;
		public static GameObject shootpoint1;
		private static Texture2D BossCardTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside/Resources/BossCards/hmprime_bosscard.png");
		public static string TargetVFX;
		public static Texture _gradTexture;

		public static void Init()
		{

			RobotShopkeeperBoss.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			bool flag = robotShopkeeperprefab != null || BossBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				robotShopkeeperprefab = BossBuilder.BuildPrefab("RobotShopkeeperBoss", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false, true);
				var companion = robotShopkeeperprefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 10000;
				companion.aiActor.MovementSpeed = 1.8f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.aiAnimator.HitReactChance = 0.05f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(370f);
				companion.aiActor.healthHaver.SetHealthMaximum(370f);
				companion.aiActor.CollisionKnockbackStrength = 2f;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.gameObject.AddComponent<AfterImageTrailController>().spawnShadows = false;
				companion.aiActor.gameObject.AddComponent<tk2dSpriteAttachPoint>();
				companion.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();
				companion.aiActor.HasShadow = true;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(1.5f, 0.25f), "shadowPos");



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
					Type = DirectionalAnimation.DirectionType.FourWay,
					Prefix = "active",
					AnimNames = new string[]
                    {
						"active_top_right",
						"active_bottom_right",
						"active_bottom_left",
						"active_top_left",
					},
					Flipped = new DirectionalAnimation.FlipType[4]
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Prefix = "move",
					Type = DirectionalAnimation.DirectionType.FourWay,
					Flipped = new DirectionalAnimation.FlipType[4],
					AnimNames = new string[]
                    {
						"move_top_right",
						"move_bottom_right",
						"move_bottom_left",
						"move_top_left",
					}
				};


				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "intro", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "broken", new string[] { "broken_right", "broken_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "move", new string[1], new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "death", new string[1], new DirectionalAnimation.FlipType[1]);


				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				companion.aiActor.gameObject.AddComponent<RobotShopkeeperEngageDoer>();

				bool flag3 = RobotShopkeeperCollection == null;
				if (flag3)
				{
					RobotShopkeeperCollection = SpriteBuilder.ConstructCollection(robotShopkeeperprefab, "RobotShopkeeperCollection");
					UnityEngine.Object.DontDestroyOnLoad(RobotShopkeeperCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], RobotShopkeeperCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9
					}, "broken_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 4f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9
					}, "broken_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 4f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9
					}, "broken", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 4f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{

					65,
					66,
					67,
					68,
					}, "active_bottom_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{
					69,
					70,
					71,
					72,
					}, "active_bottom_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{
					73,
					74,
					75,
					76,
					}, "active_top_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{
					77,
					78,
					79,
					80,
					}, "active_top_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{
					81,
					81,
					81,
					82,
					83,				
					84,
					84,
					84,
					85,
					86,
					}, "move_bottom_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{
					87,
					87,
					87,
					88,
					89,
					90,
					90,
					90,
					91,
					92,
					}, "move_bottom_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{
					93,
					93,
					93,
					94,
					95,									
					96,
					96,
					96,
					97,
					98,
					}, "move_top_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{
					99,
					99,
					99,
					100,
					101,
					102,
					103,
					104,
					104,
					104
					}, "move_top_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 5f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{

					61,
					62,
					63,
					64,//79
					}, "death", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 8f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{

					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9
					}, "talk", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 4f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, RobotShopkeeperCollection, new List<int>
					{
					10,//0
					10,
					10,
					11,
					11,
					12,
					12,
					13,
					13,
					14,
					14,
					15,
					15,
					16,
					16,
					17,
					17,
					18,
					18,
					19,
					19,//20

					20,//21
					21,
					22,
					23,
					24,
					25,//26

					26,//27
					26,//28

					27,//29
					27,
					27,//31

					28,//32
					29,
					30,
					31,
					32,
					33,//37

					34,//38
					34,//39

					35,//40
					36,
					37,
					38,
					39,
					40,//45
					41,
					42,//47

					43,//48
					43,
					44,//50
					
					

					45,//51
					46,//52

					47,//53
					47,//54

					48,//55
					49,
					50,//57
					51,
					52,
					53,
					54,
					55,//62
					56,
					57,
					58,
					59,
					60,//67

					61,//Sits around for a bit // 68
					62,
					63,
					64,
					61,
					62,
					63,
					64,
					61,
					62,
					63,
					64,//79
					}, "intro", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7.5f;

				}

				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "intro", new Dictionary<int, string> { 
					{ 1, "Play_BOSS_RatMech_Target_01" },
					{ 20, "Play_BOSS_RatMech_Bomb_01" }, 
					{ 22, "Play_BOSS_RatMech_Squat_01" }, 
					{ 33, "Play_BOSS_RatMech_Barrel_01" } ,
					{ 43, "Play_BOSS_RatMech_Barrel_01" } ,
					{ 45, "Play_BOSS_RatMech_Stand_01" } ,
					{ 57, "Play_BOSS_RatMech_Eye_01" } ,
					{ 62, "Play_BOSS_RatMech_Target_01" } ,
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "intro", new Dictionary<int, string> { 
					{ 1, "IntroSpeak1" },
					{ 22, "IntroSpeak2" },
					{ 57, "IntroSpeak3" },
					{ 69, "IntroSpeak4" },
					{ 32, "DoStandupDustClouds" },
					{ 0, "StaticBlast" },
					{ 51, "DoublePoof" },
					{ 39, "GunPositionPoofs" },
				});

				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_bottom_left", new Dictionary<int, string> {
					{ 0, "StopVFX(RBDL)" },
					{ 1, "Stop" },
					{ 2, "Stop" },

					{ 3, "Start" },
					{ 4, "Start" },

					{ 5, "StopVFX(LBDL)" },
					{ 6, "Stop" },
					{ 7, "Stop" },

					{ 8, "Start" },
					{ 9, "Start" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_bottom_left", new Dictionary<int, string> {
					{ 2, "Play_CHR_robot_roll_01" },
					{ 7, "Play_CHR_robot_roll_01" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_bottom_right", new Dictionary<int, string> {
					{ 0, "StopVFX(LBDR)" },
					{ 1, "Stop" },
					{ 2, "Stop" },

					{ 3, "Start" },
					{ 4, "Start" },

					{ 5, "StopVFX(RBDR)" },
					{ 6, "Stop" },
					{ 7, "Stop" },

					{ 8, "Start" },
					{ 9, "Start" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_bottom_right", new Dictionary<int, string> {
					{ 2, "Play_CHR_robot_roll_01" },
					{ 7, "Play_CHR_robot_roll_01" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_top_left", new Dictionary<int, string> {
					{ 0, "StopVFX(LTDR)" },
					{ 1, "Stop" },
					{ 2, "Stop" },

					{ 3, "Start" },
					{ 4, "Start" },

					{ 5, "StopVFX(RTDR)" },
					{ 6, "Stop" },
					{ 7, "Stop" },

					{ 8, "Start" },
					{ 9, "Start" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_top_left", new Dictionary<int, string> {
					{ 2, "Play_CHR_robot_roll_01" },
					{ 7, "Play_CHR_robot_roll_01" },
				});
				EnemyToolbox.AddEventTriggersToAnimation(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_top_right", new Dictionary<int, string> {
					{ 0, "StopVFX(LTDL)" },
					{ 1, "Stop" },
					{ 2, "Stop" },

					{ 3, "Start" },
					{ 4, "Start" },

					{ 5, "StopVFX(RTDL)" },
					{ 6, "Stop" },
					{ 7, "Stop" },

					{ 8, "Start" },
					{ 9, "Start" },
				});
				EnemyToolbox.AddSoundsToAnimationFrame(robotShopkeeperprefab.GetComponent<tk2dSpriteAnimator>(), "move_top_right", new Dictionary<int, string> {
					{ 2, "Play_CHR_robot_roll_01" },
					{ 7, "Play_CHR_robot_roll_01" },
				});
				/*
				 * move_bottom_left
				 * move_bottom_right
				 * move_top_left
				 * move_top_right
				*/



				var bs = robotShopkeeperprefab.GetComponent<BehaviorSpeculator>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

				GameObject fuck = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.5f, 1.8125f), "LeftGun_Down(Left)");
				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(2.3125f, 1.5f), "RightGun_Down(Left)");

				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.75f, 1.4375f), "LeftGun_Down(Right)");
				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(2.5625f, 1.8125f), "RightGun_Down(Right)");

				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.375f, 1.625f), "LeftGun_Up(Left)");
				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(2.25f, 2.125f), "RightGun_Up(Left)");

				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.8125f, 2.125f), "LeftGun_Up(Right)");
				EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(2.5625f, 1.8125f), "RightGun_Up(Right)");

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
				bs.MovementBehaviors = new List<MovementBehaviorBase>
			{
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
			


				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>
				{
					
					new AttackBehaviorGroup.AttackGroupItem()
					{

					Probability = 01f,
					Behavior = new ShootBehavior{
					ShootPoint = fuck,
					BulletScript = new CustomBulletScriptSelector(typeof(RobotShopkeeperBoss.ShootGun)),
					LeadAmount = 0f,
					AttackCooldown = 1f,
					Cooldown = 2f,
					//TellAnimation = "bottletell",
					//FireAnimation = "bottle",
					RequiresLineOfSight = true,

					MultipleFireEvents = true,
					Uninterruptible = false,
					MaxEnemiesInRoom = 4,
						},
						NickName = "Bottle"

					},
					

				};



				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:robotshopkeeper", companion.aiActor);


				var shared_auto_001 = ResourceManager.LoadAssetBundle("shared_auto_001");
				var shared_auto_002 = ResourceManager.LoadAssetBundle("shared_auto_002");
				var SpeechPoint = new GameObject("SpeechPoint");
				SpeechPoint.transform.position = new Vector3(2,2);



				var npcObj = robotShopkeeperprefab;

				FakePrefab.MarkAsFakePrefab(npcObj);
				UnityEngine.Object.DontDestroyOnLoad(npcObj);
				npcObj.SetActive(false);

				var collection = npcObj.GetComponent<tk2dSprite>().Collection;
				SpeechPoint.transform.parent = npcObj.transform;

				FakePrefab.MarkAsFakePrefab(SpeechPoint);
				UnityEngine.Object.DontDestroyOnLoad(SpeechPoint);
				SpeechPoint.SetActive(true);


				TalkDoerLite talkDoer = npcObj.AddComponent<TalkDoerLite>();

				talkDoer.placeableWidth = 4;
				talkDoer.placeableHeight = 3;
				talkDoer.difficulty = 0;
				talkDoer.isPassable = true;
				talkDoer.usesOverrideInteractionRegion = false;
				talkDoer.overrideRegionOffset = Vector2.zero;
				talkDoer.overrideRegionDimensions = Vector2.zero;
				talkDoer.overrideInteractionRadius = -1;
				talkDoer.PreventInteraction = false;
				talkDoer.AllowPlayerToPassEventually = false;
				talkDoer.speakPoint = SpeechPoint.transform;
				talkDoer.SpeaksGleepGlorpenese = false;
				talkDoer.audioCharacterSpeechTag = "golem";
				talkDoer.playerApproachRadius = 5;
				talkDoer.conversationBreakRadius = 5;
				talkDoer.echo1 = null;
				talkDoer.echo2 = null;
				talkDoer.PreventCoopInteraction = false;
				talkDoer.IsPaletteSwapped = false;
				talkDoer.PaletteTexture = null;
				talkDoer.OutlineDepth = 0.5f;
				talkDoer.OutlineLuminanceCutoff = 0.05f;
				talkDoer.MovementSpeed = 3;
				talkDoer.PathableTiles = CellTypes.FLOOR;

				UltraFortunesFavor dreamLuck = npcObj.AddComponent<UltraFortunesFavor>();

				dreamLuck.goopRadius = 2;
				dreamLuck.beamRadius = 2;
				dreamLuck.bulletRadius = 2;
				dreamLuck.bulletSpeedModifier = 0.8f;

				dreamLuck.vfxOffset = 0.625f;
				dreamLuck.sparkOctantVFX = shared_auto_001.LoadAsset<GameObject>("FortuneFavor_VFX_Spark");


				AIAnimator aIAnimator = companion.aiAnimator;
				aIAnimator.spriteAnimator = companion.spriteAnimator;
				

				ETGMod.Databases.Strings.Core.Set("#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_GENERIC", "DETECTING HUMANOID LIFE-FORM : REQUESTING REPAIR");
				ETGMod.Databases.Strings.Core.Set("#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_STOPPER", "REQUESTING REPAIR : REQUESTING REPAIR");
				ETGMod.Databases.Strings.Core.Set("#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_PURCHASE", "DETECTING REPAIR REQUEST : REQUEST ACCEPTED");
				ETGMod.Databases.Strings.Core.Set("#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_FAILPURCHASE", "INSUFFICIENT MATERIALS FOR REPAIR : REQUESTING SUFFICIENT MATERIALS");
				ETGMod.Databases.Strings.Core.Set("#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_INTRO", "DETECTING HUMANOID LIFE-FORM : REQUESTING REPAIR");
				ETGMod.Databases.Strings.Core.AddComplex("#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_ATTACKED", "DETECTING AGGRESSION : LETHAL FORCE UNAUTHORIZED");
				ETGMod.Databases.Strings.Core.AddComplex("#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_ATTACKED", "DETECTING AGGRESSION : DENIED LETHAL FORCE");

				ETGMod.Databases.Strings.Core.AddComplex("#ROBOTSHOPKEEPER_COOP_REBUKE", "DETECTING SECONDARY LIFEFORM : SHIFTING TARGET");


				ETGMod.Databases.Strings.Core.AddComplex("#ROBOTSHOPKEEPER_ENGAGED_1", "---REBOOTING TARGETING CORE---");
				ETGMod.Databases.Strings.Core.AddComplex("#ROBOTSHOPKEEPER_ENGAGED_1", "---REBOOTING SYSTEMS---");

				ETGMod.Databases.Strings.Core.AddComplex("#ROBOTSHOPKEEPER_ENGAGED_2", "---ALL SYSTEMS NOMINAL---");
				ETGMod.Databases.Strings.Core.AddComplex("#ROBOTSHOPKEEPER_ENGAGED_2", "---PROCEEDING NEXT STAGE OF REBOOT---");

				ETGMod.Databases.Strings.Core.AddComplex("#ROBOTSHOPKEEPER_ENGAGED_3", "---P.R.I.M.E WEAPONS ONLINE---");
				ETGMod.Databases.Strings.Core.AddComplex("#ROBOTSHOPKEEPER_ENGAGED_3", "---ALL WEAPONS ARMED---");

				ETGMod.Databases.Strings.Core.AddComplex("#ROBOTSHOPKEEPER_ENGAGED_4", "---DISABLING SAFETIES---");
				ETGMod.Databases.Strings.Core.AddComplex("#ROBOTSHOPKEEPER_ENGAGED_4", "---TARGET LOCKED---");
				ETGMod.Databases.Strings.Core.AddComplex("#ROBOTSHOPKEEPER_ENGAGED_4", "---OVERRIDING TARGET BLACKLIST---");


				//			TextBoxManager.ShowTextBox(this.speakPoint.position + new Vector3(0f, 0f, -4f), this.speakPoint, -1f, words, string.Empty, true, slideOrientation, false, this.SpeaksGleepGlorpenese);



				var basenpc = ResourceManager.LoadAssetBundle("shared_auto_001").LoadAsset<GameObject>("Merchant_Key").transform.Find("NPC_Key").gameObject;

				PlayMakerFSM iHaveNoFuckingClueWhatThisIs = npcObj.AddComponent<PlayMakerFSM>();

				UnityEngine.JsonUtility.FromJsonOverwrite(UnityEngine.JsonUtility.ToJson(basenpc.GetComponent<PlayMakerFSM>()), iHaveNoFuckingClueWhatThisIs);

				FieldInfo fsmStringParams = typeof(ActionData).GetField("fsmStringParams", BindingFlags.NonPublic | BindingFlags.Instance);

				(fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[1].ActionData) as List<FsmString>)[0].Value = "#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_GENERIC";
				(fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[1].ActionData) as List<FsmString>)[1].Value = "#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_STOPPER";

				(fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[4].ActionData) as List<FsmString>)[0].Value = "#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_PURCHASE";

				(fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[5].ActionData) as List<FsmString>)[0].Value = "#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_FAILPURCHASE";

				(fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[7].ActionData) as List<FsmString>)[0].Value = "#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_INTRO";

				(fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[8].ActionData) as List<FsmString>)[0].Value = "#ROBOTSHOPKEEPER_RUNBASEDMULTILINE_ATTACKED";

				(fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[9].ActionData) as List<FsmString>)[0].Value = "#SUBSHOP_GENERIC_CAUGHT_STEALING";

				(fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[10].ActionData) as List<FsmString>)[0].Value = "#SHOP_GENERIC_NO_SALE_LABEL";

				(fsmStringParams.GetValue(iHaveNoFuckingClueWhatThisIs.FsmStates[12].ActionData) as List<FsmString>)[0].Value = "#ROBOTSHOPKEEPER_COOP_REBUKE";


				Vector3[] itemPositions = new Vector3[] { new Vector3(0f, -0.5f, 1), new Vector3(0f, 1.75f, 1), new Vector3(3f, -0.5f, 1), new Vector3(3f, 1.75f, 1) };

				var posList = new List<Transform>();
				for (int i = 0; i < itemPositions.Length; i++)
				{

					var ItemPoint = new GameObject("ItemPoint" + i);
					ItemPoint.transform.position = itemPositions[i];
					FakePrefab.MarkAsFakePrefab(ItemPoint);
					UnityEngine.Object.DontDestroyOnLoad(ItemPoint);
					ItemPoint.SetActive(true);
					posList.Add(ItemPoint.transform);
				}

				var ItemPoint1 = new GameObject("ItemPoint1");
				ItemPoint1.transform.position = new Vector3(1.125f, 2.125f, 1);
				FakePrefab.MarkAsFakePrefab(ItemPoint1);
				UnityEngine.Object.DontDestroyOnLoad(ItemPoint1);
				ItemPoint1.SetActive(true);
				var ItemPoint2 = new GameObject("ItemPoint2");
				ItemPoint2.transform.position = new Vector3(2.625f, 1f, 1);
				FakePrefab.MarkAsFakePrefab(ItemPoint2);
				UnityEngine.Object.DontDestroyOnLoad(ItemPoint2);
				ItemPoint2.SetActive(true);
				var ItemPoint3 = new GameObject("ItemPoint3");
				ItemPoint3.transform.position = new Vector3(4.125f, 2.125f, 1);
				FakePrefab.MarkAsFakePrefab(ItemPoint3);
				UnityEngine.Object.DontDestroyOnLoad(ItemPoint3);
				ItemPoint3.SetActive(true);


				var shopObj = robotShopkeeperprefab.AddComponent<CustomShopController>();
				FakePrefab.MarkAsFakePrefab(shopObj.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(shopObj.gameObject);

				shopObj.gameObject.SetActive(false);

				shopObj.currencyType = CustomShopItemController.ShopCurrencyType.CUSTOM;

				shopObj.ActionAndFuncSetUp(null, RobotShopkeeperRemoveCurrency, RobotShopkeeperCustomPrice, null, null);



				shopObj.customPriceSprite = ItsDaFuckinShopApi.AddCustomCurrencyType("Planetside/Resources/NPCs/CustomCurrencyIcons/creditMoment.png", $"{"psog"}:{"creditsAndMore"}");


				shopObj.canBeRobbed = false;

				shopObj.placeableHeight = 5;
				shopObj.placeableWidth = 5;
				shopObj.difficulty = 0;
				shopObj.isPassable = true;
				shopObj.baseShopType = BaseShopController.AdditionalShopType.TRUCK;//shopType;


				shopObj.FoyerMetaShopForcedTiers = false;
				shopObj.IsBeetleMerchant = false;
				shopObj.ExampleBlueprintPrefab = null;
				shopObj.poolType = CustomShopController.PoolType.DUPES_AND_NOEXCLUSION;

				GenericLootTable table = LootTableTools.CreateLootTable();
				table.AddItemsToPool(new Dictionary<int, float>() { { RepairNode.RepairNodeID, 1 } });
				shopObj.shopItems = table;
				shopObj.spawnPositions = posList.ToArray();//{ ItemPoint1.transform, ItemPoint2.transform, ItemPoint3.transform };

				foreach (var pos in shopObj.spawnPositions)
				{
					pos.parent = shopObj.gameObject.transform;
				}

				shopObj.shopItemsGroup2 = null;
				shopObj.spawnPositionsGroup2 = null;
				shopObj.spawnGroupTwoItem1Chance = 0.5f;
				shopObj.spawnGroupTwoItem2Chance = 0.5f;
				shopObj.spawnGroupTwoItem3Chance = 0.5f;
				shopObj.shopkeepFSM = npcObj.GetComponent<PlayMakerFSM>();
				shopObj.shopItemShadowPrefab = shared_auto_001.LoadAsset<GameObject>("Merchant_Key").GetComponent<BaseShopController>().shopItemShadowPrefab;
				shopObj.prerequisites = new DungeonPrerequisite[0];

				//shopObj.shopItemShadowPrefab = 

				shopObj.cat = null;

				/*
				if (hasMinimapIcon)
				{
					if (!string.IsNullOrEmpty(minimapIconSpritePath))
					{
						shopObj.OptionalMinimapIcon = SpriteBuilder.SpriteFromResource(minimapIconSpritePath);
						UnityEngine.Object.DontDestroyOnLoad(shopObj.OptionalMinimapIcon);
						FakePrefab.MarkAsFakePrefab(shopObj.OptionalMinimapIcon);
					}
					else
					{
						shopObj.OptionalMinimapIcon = ResourceCache.Acquire("Global Prefabs/Minimap_NPC_Icon") as GameObject;
					}
				}
				*/
				shopObj.ShopCostModifier = 1;
				shopObj.FlagToSetOnEncounter = GungeonFlags.NONE;

				shopObj.giveStatsOnPurchase = false;
				shopObj.statsToGive = null;

				npcObj.transform.parent = shopObj.gameObject.transform;
				npcObj.transform.position = new Vector3(0, 0);

				/*
				if (hasCarpet)
				{
					var carpetObj = SpriteBuilder.SpriteFromResource(carpetSpritePath, new GameObject(prefix + ":" + name + "_Carpet"));
					carpetObj.GetComponent<tk2dSprite>().SortingOrder = 2;
					FakePrefab.MarkAsFakePrefab(carpetObj);
					UnityEngine.Object.DontDestroyOnLoad(carpetObj);
					carpetObj.SetActive(true);

					carpetObj.transform.position = new Vector3(CarpetXOffset, CarpetYOffset, 1.7f);
					carpetObj.transform.parent = shopObj.gameObject.transform;
					carpetObj.layer = 20;
				}
				npcObj.SetActive(true);
				*/






				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/BulletBanker/bulletbanker_idle_001", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:robotshopkeeper";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/BulletBanker/bulletbanker_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\bankericon.png");
				PlanetsideModule.Strings.Enemies.Set("#HMPRIME_NAME", "H.M Prime");
				PlanetsideModule.Strings.Enemies.Set("#HMPRIME_SD", "Battle Tower");
				PlanetsideModule.Strings.Enemies.Set("#HMPRIME_LD", "Built by the Hegemony Of Man in preparation for the invasion of the Gungeon, they hoped that this one-core war machince could push back against the forces of the Gungeon.\n\nWhile it mostly failed, it gained the respect of the Gundead with its massive firepower, and left it in the confines of the Gungeon, albeit slightly re-programmed and disassembled.\n\nAny Gungeoneer who finds the machine in low power mode is heavily advised NOT to approach it.");

				companion.encounterTrackable.journalData.PrimaryDisplayName = "#HMPRIME_NAME";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#HMPRIME_SD";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#HMPRIME_LD";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:robotshopkeeper");
				EnemyDatabase.GetEntry("psog:robotshopkeeper").ForcedPositionInAmmonomicon = 201;
				EnemyDatabase.GetEntry("psog:robotshopkeeper").isInBossTab = true;
				EnemyDatabase.GetEntry("psog:robotshopkeeper").isNormalEnemy = true;

				GenericIntroDoer miniBossIntroDoer = robotShopkeeperprefab.AddComponent<GenericIntroDoer>();
				robotShopkeeperprefab.AddComponent<HMPrimeIntroController>();

				miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.BossTriggerZone;
				miniBossIntroDoer.initialDelay = 0.5f;
				miniBossIntroDoer.cameraMoveSpeed = 5;
				miniBossIntroDoer.specifyIntroAiAnimator = null;
				miniBossIntroDoer.BossMusicEvent = "Play_MUS_Boss_Theme_Lich";
				miniBossIntroDoer.PreventBossMusic = false;
				miniBossIntroDoer.InvisibleBeforeIntroAnim = false;
				miniBossIntroDoer.preIntroAnim = "broken";
				miniBossIntroDoer.preIntroDirectionalAnim = string.Empty;
				miniBossIntroDoer.introAnim = "intro";
				miniBossIntroDoer.introDirectionalAnim = string.Empty;
				miniBossIntroDoer.continueAnimDuringOutro = false;
				miniBossIntroDoer.cameraFocus = null;
				miniBossIntroDoer.roomPositionCameraFocus = Vector2.zero;
				miniBossIntroDoer.restrictPlayerMotionToRoom = false;
				miniBossIntroDoer.fusebombLock = false;
				miniBossIntroDoer.AdditionalHeightOffset = 0;
				PlanetsideModule.Strings.Enemies.Set("#QUOTE", "");

				miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
				{
					bossNameString = "#HMPRIME_NAME",
					bossSubtitleString = "#HMPRIME_SD",
					bossQuoteString = "#QUOTE",
					bossSpritePxOffset = IntVector2.Zero,
					topLeftTextPxOffset = IntVector2.Zero,
					bottomRightTextPxOffset = IntVector2.Zero,
					bgColor = Color.green
				};
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

				companion.aiActor.enabled = false;
				companion.aiActor.behaviorSpeculator.enabled = false;

				//==================
			}
		}
		public static bool RobotShopkeeperCustomCanBuy(CustomShopController shop, PlayerController player, int cost)
		{
			int total = (int)GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.META_CURRENCY);
			if (total > 24) { return true; }
			return false;
		}
		public static int RobotShopkeeperCustomPrice(CustomShopController shop, CustomShopItemController itemCont, PickupObject item)
		{
			return 25;
		}
		public static int RobotShopkeeperRemoveCurrency(CustomShopController shop, PlayerController user, int cost)
		{
			int MetaCost = -1;
			GameStatsManager.Instance.RegisterStatChange(TrackedStats.META_CURRENCY, MetaCost);
			shop.gameObject.GetComponentInChildren<RobotShopkeeperEngageDoer>().shopToSellOut = shop;
			shop.gameObject.GetComponentInChildren<RobotShopkeeperEngageDoer>().Invoke("IncrenemtScaling", 0);

			return 1;
		}


		public class ShootGun : Script
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));

				float amount = base.BulletBank.aiActor.GetComponent<RobotShopkeeperEngageDoer>().AmountOfPurchases;
				amount = amount * 6;
				for (int q = 0; q < 24 + amount; q++)
				{
					base.PostWwiseEvent("Play_ITM_Macho_Brace_Active_01", null);
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(true, base.AimDirection ,this));
					base.BulletBank.aiActor.StartCoroutine(QuickscopeNoob(false, base.AimDirection, this));
					yield return this.Wait(3);
				}
				yield return this.Wait(120);
				yield break;
			}
			private IEnumerator QuickscopeNoob(bool isLeft, float aimDir, ShootGun parent, float chargeTime = 0.5f)
			{
				Vector2 positionLeft = base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2();
				Vector2 positionRight = base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2();
				GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
				Vector2 sre = isLeft == true ? positionLeft : positionRight;

				component2.transform.position = new Vector3(sre.x, sre.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, aimDir);
				component2.dimensions = new Vector2(1000f, 1f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laser = new Color(1f, 0f, 0f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
				float elapsed = 0;
				float Time = chargeTime;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;

					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					if (component2 != null)
					{
						component2.transform.position = isLeft == true ? base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2() : base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2();

						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (100 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, base.AimDirection);
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				float die = base.AimDirection;
				elapsed = 0;
				Time = 0.25f;
				while (elapsed < Time)
				{
					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(component2.gameObject);
						yield break;
					}
					float t = (float)elapsed / (float)Time;
					if (component2 != null)
					{
						component2.transform.position = isLeft == true ? base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2() : base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2();
						component2.dimensions = new Vector2(1000f, 1f);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (100 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				Destroy(component2.gameObject);
				base.PostWwiseEvent("Play_CombineShot", null);
				base.Fire(Offset.OverridePosition(isLeft == true ? base.BulletBank.aiActor.transform.Find("LeftGun_Down(Left)").transform.PositionVector2() : base.BulletBank.aiActor.transform.Find("RightGun_Down(Right)").transform.PositionVector2()), new Direction(die + UnityEngine.Random.Range(-4, 4), DirectionType.Absolute, -1f), new Speed(36f, SpeedType.Absolute), new BasicBullet());
				yield break;
			}
			public class BasicBullet : Bullet
			{
				public BasicBullet() : base("directedfire", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					yield break;
				}
			}

		}


		public class EnemyBehavior : BraveBehaviour
		{
			public void LerpMaterialGlow(Material targetMaterial, float startGlow, float targetGlow, float duration)
			{
				base.StartCoroutine(this.LerpMaterialGlowCR(targetMaterial, startGlow, targetGlow, duration));
			}
			private IEnumerator LerpMaterialGlowCR(Material targetMaterial, float startGlow, float targetGlow, float duration)
			{
				float elapsed = 0f;
				while (elapsed < duration)
				{
					elapsed += BraveTime.DeltaTime; ;
					float t = elapsed / duration;
					if (targetMaterial != null)
					{
						targetMaterial.SetFloat("_EmissivePower", Mathf.Lerp(startGlow, targetGlow, t));
					}
					yield return null;
				}
				yield break;
			}

			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo.Contains("StaticBlast"))
                {
					for (int i = 0; i < 5; i++)
					{
						GameObject breakVFX = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(156) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects[0].effects[0].effect,  base.aiActor.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(1.25f, -1.25f) , UnityEngine.Random.Range(0.625f, -0.625f)), Quaternion.identity);
						tk2dBaseSprite component = breakVFX.GetComponent<tk2dBaseSprite>();
						component.PlaceAtPositionByAnchor(base.aiActor.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(1.25f, -1.25f), UnityEngine.Random.Range(0.625f, -1.25f)), tk2dBaseSprite.Anchor.MiddleCenter);
						component.HeightOffGround = 35f;
						component.UpdateZDepth();
						tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
						if (component2 != null)
						{
							component2.ignoreTimeScale = true;
							component2.AlwaysIgnoreTimeScale = true;
							component2.AnimateDuringBossIntros = true;
							component2.alwaysUpdateOffscreen = true;
							component2.playAutomatically = true;
						}
					}
					
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("DoStandupDustClouds"))
				{
					GameObject PoofVFX = GameManager.Instance.RewardManager.D_Chest.VFX_PreSpawn;
					PoofVFX.SetActive(true);
					GameObject obj = UnityEngine.Object.Instantiate<GameObject>(PoofVFX, base.aiActor.transform.position - new Vector3(0, 1), Quaternion.identity);
					tk2dBaseSprite component = obj.GetComponent<tk2dBaseSprite>();
					component.HeightOffGround = 35f;
					tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
					if (component2 != null)
					{
						component2.ignoreTimeScale = true;
						component2.AlwaysIgnoreTimeScale = true;
						component2.AnimateDuringBossIntros = true;
						component2.alwaysUpdateOffscreen = true;
						component2.playAutomatically = true;
					}
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("DoublePoof"))
				{
					for (int i = 0; i < 2; i++)
					{
						Vector2 OFsset = new Vector2(-1, -1);
						if (i == 0)
						{
							OFsset = new Vector2(1, -1);
						}
						LootEngine.DoDefaultItemPoof(base.aiActor.sprite.WorldCenter + OFsset, true, true);
					}
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("GunPositionPoofs"))
				{
					for (int i = 0; i < 2; i++)
					{
						Vector2 OFsset = new Vector2(-1.5f, 0);
						if (i == 0)
						{
							OFsset = new Vector2(1.5f, 0);
						}
						GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), base.aiActor.sprite.WorldCenter + OFsset, Quaternion.identity);
						tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
						component.PlaceAtPositionByAnchor(base.aiActor.sprite.WorldCenter + OFsset, tk2dBaseSprite.Anchor.MiddleCenter);
						component.HeightOffGround = 35f;
						component.UpdateZDepth();
						tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
						if (component2 != null)
						{
							component2.ignoreTimeScale = true;
							component2.AlwaysIgnoreTimeScale = true;
							component2.AnimateDuringBossIntros = true;
							component2.alwaysUpdateOffscreen = true;
							component2.playAutomatically = true;
							component2.sprite.usesOverrideMaterial = true;
							component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
							component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
							component2.sprite.renderer.material.SetFloat("_EmissivePower", 1);
							component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.1f);
							component2.sprite.renderer.material.SetColor("_OverrideColor", Color.gray);
							component2.sprite.renderer.material.SetColor("_EmissiveColor", Color.gray);
						}
					}
					
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("IntroSpeak"))
				{
					float time = 2;
					string TextToUse = "---BOOTING---";
					if (clip.GetFrame(frameIdx).eventInfo.Contains("1"))
                    {TextToUse = "#ROBOTSHOPKEEPER_ENGAGED_1"; time = 1; }
					if (clip.GetFrame(frameIdx).eventInfo.Contains("2"))
					{TextToUse = "#ROBOTSHOPKEEPER_ENGAGED_2"; time = 1.5f; }
					if (clip.GetFrame(frameIdx).eventInfo.Contains("3"))
					{TextToUse = "#ROBOTSHOPKEEPER_ENGAGED_3"; time = 1; }
					if (clip.GetFrame(frameIdx).eventInfo.Contains("4"))
					{TextToUse = "#ROBOTSHOPKEEPER_ENGAGED_4"; time = 1.25f; }					
					ShowBoxInternal(base.aiActor.GetComponent<TalkDoerLite>().transform.position + new Vector3(3,3), base.aiActor.transform, time, ETGMod.Databases.Strings.Core.Get(TextToUse), "TextBox", 0.5f,"golem", false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Start"))
                {
					base.aiActor.MovementSpeed = 1.8f; 
                }
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Stop"))
                {
					base.aiActor.MovementSpeed = 0f;
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("StopVFX"))
				{
					base.aiActor.MovementSpeed = 0f;
					ETGModConsole.Log(clip.GetFrame(frameIdx).eventInfo);
					Dictionary<Vector2, Vector2> positions = ReturnVFXPos(clip.GetFrame(frameIdx).eventInfo);
					if (positions != null)
					{
						foreach (var KeysAndValues in positions)
						{
                            {

								GameObject gameObject = SpawnManager.SpawnVFX(BraveResources.Load<GameObject>("Global VFX/VFX_DBZ_Charge", ".prefab"), false);
								gameObject.transform.position = base.aiActor.transform.position + KeysAndValues.Key.ToVector3ZisY();
								tk2dSpriteAnimator component2 = gameObject.GetComponent<tk2dSpriteAnimator>();
								if (component2 != null)
								{
									component2.ignoreTimeScale = true;
									component2.AlwaysIgnoreTimeScale = true;
									component2.AnimateDuringBossIntros = true;
									component2.alwaysUpdateOffscreen = true;
									component2.playAutomatically = true;
								}
							}
							{
								GameObject gameObject = SpawnManager.SpawnVFX(BraveResources.Load<GameObject>("Global VFX/VFX_DBZ_Charge", ".prefab"), false);
								gameObject.transform.position = base.aiActor.transform.position + KeysAndValues.Value.ToVector3ZisY();
								tk2dSpriteAnimator component2 = gameObject.GetComponent<tk2dSpriteAnimator>();
								if (component2 != null)
								{
									component2.ignoreTimeScale = true;
									component2.AlwaysIgnoreTimeScale = true;
									component2.AnimateDuringBossIntros = true;
									component2.alwaysUpdateOffscreen = true;
									component2.playAutomatically = true;
								}
							}

						}
					}
				}
			}

			private static Dictionary<Vector2, Vector2> ReturnVFXPos(string Key)
            {
				Dictionary<Vector2, Vector2> poses = new Dictionary<Vector2, Vector2>();
				if (Key.Contains("(LBDL)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(2.1875f, 0.375f), new Vector2(1.0625f, 0.8125f) } }; }
				if (Key.Contains("(RBDL)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(0.5625f, 0.5f), new Vector2(2.375f, 0.75f) } };}

				if (Key.Contains("(LBDR)")) { poses = new Dictionary<Vector2, Vector2> { {new Vector2(0.9375f, 0.3125f), new Vector2(2f, 0.8125f) } }; }
				if (Key.Contains("(RBDR)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(2.5625f, 0.5625f), new Vector2(0.75f, 0.75f) } }; }

				if (Key.Contains("(LTDR)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(0.5625f, 0.5f), new Vector2(2.375f, 0.75f) } }; }
				if (Key.Contains("(RTDR)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(2.1875f, 0.375f), new Vector2(1.0625f, 0.8125f) } }; }

				if (Key.Contains("(LTDL)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(2.5625f, 0.5625f), new Vector2(0.75f, 0.75f) } }; }
				if (Key.Contains("(RTDL)")) { poses = new Dictionary<Vector2, Vector2> { { new Vector2(0.9375f, 0.3125f), new Vector2(2f, 0.8125f) } }; }
				return poses; 
            }

			private static void ShowBoxInternal(Vector3 worldPosition, Transform parent, float duration, string text, string prefabName, float padding, string audioTag, bool instant, TextBoxManager.BoxSlideOrientation slideOrientation, bool showContinueText, bool UseAlienLanguage = false)
			{
				Vector2 dimensions = new Vector2(-1f, -1f);
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load(prefabName, ".prefab"));
				TextBoxManager component = gameObject.GetComponent<TextBoxManager>();
				component.IsScalingUp = true;

				FieldInfo ikjonhhuuh = typeof(TextBoxManager).GetField("boxPadding", BindingFlags.Instance | BindingFlags.NonPublic);
				ikjonhhuuh.SetValue(component, padding);


				FieldInfo leEnabler = typeof(TextBoxManager).GetField("audioTag", BindingFlags.Instance | BindingFlags.NonPublic);
				leEnabler.SetValue(component, audioTag);

				SetText(component ,text, worldPosition, instant, slideOrientation, true, UseAlienLanguage, prefabName == "ThoughtBubble");
				if (parent != null)
				{
					component.transform.parent = parent;
				}
				if (duration >= 0f)
				{
					component.StartCoroutine(TextBoxLifespanCR(gameObject, parent, duration, component));
				}
				if (showContinueText)
				{
					component.ShowContinueText();
				}
				component.StartCoroutine(HandleScaleUp(dimensions, component));
			}
			public static void SetText(TextBoxManager man,string text, Vector3 worldPosition, bool instant = true, TextBoxManager.BoxSlideOrientation slideOrientation = TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, bool showContinueText = true, bool UseAlienLanguage = false, bool clampThoughtBubble = false)
			{

				Transform boxSpriteTransform = PlanetsideReflectionHelper.ReflectGetField<Transform>(typeof(TextBoxManager), "boxSpriteTransform", man);
				tk2dSlicedSprite boxSprite = PlanetsideReflectionHelper.ReflectGetField<tk2dSlicedSprite>(typeof(TextBoxManager), "boxSprite", man);

				Transform textMeshTransform = PlanetsideReflectionHelper.ReflectGetField<Transform>(typeof(TextBoxManager), "textMeshTransform", man);
				tk2dTextMesh textMesh = PlanetsideReflectionHelper.ReflectGetField<tk2dTextMesh>(typeof(TextBoxManager), "textMesh", man);

				Transform continueTextMeshTransform = PlanetsideReflectionHelper.ReflectGetField<Transform>(typeof(TextBoxManager), "continueTextMeshTransform", man);
				tk2dTextMesh continueTextMesh = PlanetsideReflectionHelper.ReflectGetField<tk2dTextMesh>(typeof(TextBoxManager), "continueTextMeshTransform", man);

				float boxPadding = PlanetsideReflectionHelper.ReflectGetField<float>(typeof(TextBoxManager), "boxPadding", man);

				float additionalPaddingLeft = PlanetsideReflectionHelper.ReflectGetField<float>(typeof(TextBoxManager), "additionalPaddingLeft", man);
				float additionalPaddingRight = PlanetsideReflectionHelper.ReflectGetField<float>(typeof(TextBoxManager), "additionalPaddingRight", man);
				float additionalPaddingTop = PlanetsideReflectionHelper.ReflectGetField<float>(typeof(TextBoxManager), "additionalPaddingTop", man);
				float additionalPaddingBottom = PlanetsideReflectionHelper.ReflectGetField<float>(typeof(TextBoxManager), "additionalPaddingBottom", man);

				FieldInfo m_basePosition = typeof(TextBoxManager).GetField("m_basePosition", BindingFlags.Instance | BindingFlags.NonPublic);


				if (boxSpriteTransform == null)
				{
					boxSpriteTransform = boxSprite.transform;
				}
				if (textMeshTransform == null)
				{
					textMeshTransform = textMesh.transform;
				}
				if (continueTextMeshTransform == null && continueTextMesh)
				{
					continueTextMeshTransform = continueTextMesh.transform;
				}
				if (text == string.Empty)
				{
					return;
				}
				text = text.Replace("\\n", Environment.NewLine);
				float x = boxSpriteTransform.localPosition.x;
				float num = -x / (boxSprite.dimensions.x / 16f);
				string text2 = textMesh.GetStrippedWoobleString(text);
				if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE)
				{
					textMesh.LineSpacing = 0.125f;
				}
				else
				{
					textMesh.LineSpacing = 0f;
				}
				if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE)
				{
					textMesh.wordWrapWidth = 350;
				}
				else if (text2.Length < 25)
				{
					textMesh.wordWrapWidth = 250;
				}
				else
				{
					textMesh.wordWrapWidth = 200 + (text2.Length - 25) / 4;
					if (!text2.EndsWith(" "))
					{
						text2 += " ";
					}
				}
				if (Application.isPlaying)
				{
					bool flag = false;
					for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
					{
						flag |= GameManager.Instance.AllPlayers[i].UnderstandsGleepGlorp;
					}
					textMesh.font = GameManager.Instance.DefaultNormalConversationFont;

				}
				textMesh.text = text2;
				textMesh.CheckFontsForLanguage();
				textMesh.ForceBuild();
				Bounds trueBounds =textMesh.GetTrueBounds();
				float num2 = Mathf.Ceil((trueBounds.size.x + boxPadding * 2f + additionalPaddingLeft + additionalPaddingRight) * 16f) / 16f;
				float num3 = Mathf.Ceil((trueBounds.size.y + boxPadding * 2f + additionalPaddingTop + additionalPaddingBottom) * 16f) / 16f;
				if (showContinueText && continueTextMesh)
				{
					num2 += continueTextMesh.GetEstimatedMeshBoundsForString("...").extents.x * 2f;
				}
				float num4 = num2 * 16f;
				float num5 = num3 * 16f;
				if (clampThoughtBubble)
				{
					float num6 = 47f + (Mathf.Max(47f, num4) - 47f).Quantize(23f, VectorConversions.Floor);
					float num7 = 57f + (Mathf.Max(57f, num4) - 57f).Quantize(23f, VectorConversions.Floor);
					if (num6 < num4)
					{
						num6 += 23f;
					}
					if (num7 < num4)
					{
						num7 += 23f;
					}
					float num8 = Mathf.Abs(num6 - num4);
					float num9 = Mathf.Abs(num7 - num4);
					num4 = ((num8 >= num9) ? num7 : num6);
				}
				Vector3 lhs = new Vector3(0f, 0f);
				tk2dSpriteDefinition currentSpriteDef = boxSprite.GetCurrentSpriteDef();
				Vector3 boundsDataExtents = currentSpriteDef.boundsDataExtents;
				if (currentSpriteDef.texelSize.x != 0f && currentSpriteDef.texelSize.y != 0f && boundsDataExtents.x != 0f && boundsDataExtents.y != 0f)
				{
					lhs = new Vector3(boundsDataExtents.x / currentSpriteDef.texelSize.x, boundsDataExtents.y / currentSpriteDef.texelSize.y, 1f);
				}
				lhs = Vector3.Max(lhs, Vector3.one);
				num4 = Mathf.Max(num4, (boxSprite.borderLeft + boxSprite.borderRight) * lhs.x);
				num5 = Mathf.Max(num5, (boxSprite.borderTop + boxSprite.borderBottom) * lhs.y);
				boxSprite.dimensions = new Vector2(num4, num5);
				if (boxSprite.dimensions.x < (boxSprite.borderLeft +boxSprite.borderRight) * lhs.x || boxSprite.dimensions.y < (boxSprite.borderTop + boxSprite.borderBottom) * lhs.y)
				{
					boxSprite.BorderOnly = true;
				}
				else
				{
					boxSprite.BorderOnly = false;
				}
				boxSprite.ForceBuild();
				textMesh.color = man.textColor;
				if (instant)
				{
					textMesh.text = textMesh.PreprocessWoobleSignifiers(text);
					textMesh.Commit();
				}
				else
				{
					textMesh.text = string.Empty;
					textMesh.Commit();
					string text3 = textMesh.PreprocessWoobleSignifiers(text);
					man.StartCoroutine(RevealTextCharacters(man, text3));
				}
				float y = BraveMathCollege.QuantizeFloat(boxSprite.dimensions.y / 16f - boxPadding - additionalPaddingTop, 0.0625f);
				if (textMesh.anchor == TextAnchor.UpperLeft)
				{
					textMeshTransform.localPosition = new Vector3(boxPadding + additionalPaddingLeft, y, -0.1f);
				}
				else if (textMesh.anchor == TextAnchor.UpperCenter)
				{
					textMeshTransform.localPosition = new Vector3(num2 / 2f, y, -0.1f);
				}
				textMeshTransform.localPosition += new Vector3(0.0234375f, 0.0234375f, 0f);
				if (continueTextMesh)
				{
					if (showContinueText)
					{
						Bounds estimatedMeshBoundsForString = continueTextMesh.GetEstimatedMeshBoundsForString("...");
						continueTextMeshTransform.localPosition = new Vector3(num2 - man.continuePaddingRight - estimatedMeshBoundsForString.extents.x * 2f, man.continuePaddingBottom, -0.1f);
					}
					else
					{
						continueTextMesh.text = string.Empty;
						continueTextMesh.Commit();
					}
				}
				switch (slideOrientation)
				{
					case TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT:
						boxSpriteTransform.localPosition = boxSpriteTransform.localPosition.WithX(BraveMathCollege.QuantizeFloat(-1f * num * (boxSprite.dimensions.x / 16f), 0.0625f));
						break;
					case TextBoxManager.BoxSlideOrientation.FORCE_RIGHT:
						num = 0.1f;
						boxSpriteTransform.localPosition = boxSpriteTransform.localPosition.WithX(BraveMathCollege.QuantizeFloat(-1f * num * (boxSprite.dimensions.x / 16f), 0.0625f));
						break;
					case TextBoxManager.BoxSlideOrientation.FORCE_LEFT:
						num = 0.85f;
						boxSpriteTransform.localPosition = boxSpriteTransform.localPosition.WithX(BraveMathCollege.QuantizeFloat(-1f * num * (boxSprite.dimensions.x / 16f), 0.0625f));
						break;
					default:
						boxSpriteTransform.localPosition = boxSpriteTransform.localPosition.WithX(BraveMathCollege.QuantizeFloat(-1f * num * (boxSprite.dimensions.x / 16f), 0.0625f));
						break;
				}
				if (slideOrientation == TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT)
				{
					float num10 = (!Application.isPlaying) ? 0f : GameManager.Instance.MainCameraController.transform.position.x;
					if (worldPosition.x > num10)
					{
						boxSpriteTransform.localPosition = boxSpriteTransform.localPosition.WithX(-1f * (1f - num) * (boxSprite.dimensions.x / 16f));
					}
				}
				man.transform.position = worldPosition;
				man.transform.localScale = Vector3.one * (float)Mathf.Max(1, Mathf.FloorToInt(1f / GameManager.Instance.MainCameraController.CurrentZoomScale));
				m_basePosition.SetValue(man, worldPosition);
				man.UpdateForCameraPosition();
				man.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
			}
			private static IEnumerator RevealTextCharacters(TextBoxManager man, string strippedString)
			{
				Transform boxSpriteTransform = PlanetsideReflectionHelper.ReflectGetField<Transform>(typeof(TextBoxManager), "boxSpriteTransform", man);
				tk2dSlicedSprite boxSprite = PlanetsideReflectionHelper.ReflectGetField<tk2dSlicedSprite>(typeof(TextBoxManager), "boxSprite", man);
				if (boxSpriteTransform == null) { boxSpriteTransform = boxSprite.transform; }
				tk2dTextMesh textMesh = PlanetsideReflectionHelper.ReflectGetField<tk2dTextMesh>(typeof(TextBoxManager), "textMesh", man);
				if (boxSpriteTransform == null) { ETGModConsole.Log("boxSpriteTransform is NULL"); }
				if (textMesh == null) { ETGModConsole.Log("textMesh is NULL"); }

				FieldInfo m_isRevealingText = typeof(TextBoxManager).GetField("m_isRevealingText", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo skipTextReveal = typeof(TextBoxManager).GetField("skipTextReveal", BindingFlags.Instance | BindingFlags.NonPublic);

				string audioTag = PlanetsideReflectionHelper.ReflectGetField<string>(typeof(TextBoxManager), "audioTag", man);
				if (audioTag == null) { ETGModConsole.Log("audioTag is NULL"); }


				m_isRevealingText.SetValue(man, true);
				skipTextReveal.SetValue(man, false);

				while (man.IsScalingUp)
				{
					yield return null;
				}
				float elapsed = 0f;
				float duration = (float)strippedString.Length / 100;
				Renderer boxRenderer = boxSpriteTransform.GetComponent<Renderer>();
				textMesh.inlineStyling = true;
				textMesh.color = Color.black;
				textMesh.visibleCharacters = 0;
				int visibleCharacters = 0;
				if (duration > 0f)
				{
					while (elapsed < duration)
					{
						elapsed += 0.016f;
						float t = elapsed / duration;
						int numCharacters = Mathf.FloorToInt((float)strippedString.Length * t);
						if (numCharacters > 100000)
						{
							numCharacters = 0;
						}
						if (numCharacters > visibleCharacters && boxRenderer.isVisible)
						{
							visibleCharacters = numCharacters;
							if (!string.IsNullOrEmpty(audioTag))
							{
								AkSoundEngine.PostEvent("Play_CHR_" + audioTag + "_voice_01", man.gameObject);
							}
						}
						textMesh.visibleCharacters = visibleCharacters;
						textMesh.text = strippedString;
						textMesh.Commit();
						yield return null;
					}
				}
				textMesh.visibleCharacters = int.MaxValue;
				textMesh.text = strippedString;
				textMesh.Commit();
				m_isRevealingText.SetValue(man, false);
				skipTextReveal.SetValue(man, false);
				yield break;
			}
			private static IEnumerator TextBoxLifespanCR(GameObject target, Transform parent, float lifespan, TextBoxManager man)
			{
				yield return null;
				while (PlanetsideReflectionHelper.ReflectGetField<bool>(typeof(TextBoxManager), "m_isRevealingText", man) == true)
				{
					yield return null;
				}
				float ela = 0f;
				float dura = lifespan;
				while (ela < dura)
				{
					ela += 0.016f;
					yield return null;
				}
				if (parent != null && dura <= ela)
				{
					UnityEngine.Object.Destroy(target);
				}
				
				yield break;
			}
			private static IEnumerator HandleScaleUp(Vector2 prevBoxSize, TextBoxManager man)
			{
				man.IsScalingUp = true;
				if (prevBoxSize.x <= 0f || prevBoxSize.y <= 0f)
				{
					if (man == null) { ETGModConsole.Log("AEEEEEEEEEEEEEE"); }
					Transform targetTransform = man.transform;
					if (targetTransform == null) { ETGModConsole.Log("ada"); }
					float elapsed = 0f;
					float duration = 0.06f;
					while (elapsed < duration)
					{
						elapsed += GameManager.INVARIANT_DELTA_TIME;
						targetTransform.localScale = Vector3.Lerp(new Vector3(0.01f, 0.01f, 1f), Vector3.one * (float)Mathf.Max(1, Mathf.FloorToInt(1f / GameManager.Instance.MainCameraController.CurrentZoomScale)), Mathf.SmoothStep(0f, 1f, elapsed / duration));
						yield return null;
					}
				}
				else
				{
					if (PlanetsideReflectionHelper.ReflectGetField<tk2dSlicedSprite>(typeof(TextBoxManager), "boxSprite", man) == null) { ETGModConsole.Log("AAAAAAAAAAAAAAAAAAA"); }
					Vector2 targetdimensions = PlanetsideReflectionHelper.ReflectGetField<tk2dSlicedSprite>(typeof(TextBoxManager), "boxSprite", man).dimensions;
					if (targetdimensions == null) { ETGModConsole.Log("fuc"); }
					float elapsed2 = 0f;
					float durationModifier = Mathf.Clamp01(((targetdimensions - prevBoxSize).magnitude - 5f) / 10f);
					float duration2 = Mathf.Lerp(0.025f, 0.06f, durationModifier);
					while (elapsed2 < duration2)
					{
						elapsed2 += GameManager.INVARIANT_DELTA_TIME;
						PlanetsideReflectionHelper.ReflectGetField<tk2dSlicedSprite>(typeof(TextBoxManager), "boxSprite", man).dimensions = Vector2.Lerp(prevBoxSize, targetdimensions, Mathf.SmoothStep(0f, 1f, elapsed2 / duration2));
						yield return null;
					}
				}
				man.IsScalingUp = false;
				yield break;
			}



			private void Start()
			{

				if (!base.aiActor.IsBlackPhantom)
				{
					Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
					mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
					mat.SetColor("_EmissiveColor", new Color32(0, 255, 54, 255));
					mat.SetFloat("_EmissiveColorPower", 1.55f);
					mat.SetFloat("_EmissivePower", 100);
					mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);

					base.aiActor.sprite.renderer.material = mat;
				}
				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;

				//Important for not breaking basegame stuff!
				StaticReferenceManager.AllHealthHavers.Remove(base.aiActor.healthHaver);
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
				};
				base.healthHaver.healthHaver.OnDeath += (obj) =>
				{
					float itemsToSpawn = UnityEngine.Random.Range(2, 6);
					float spewItemDir = 360 / itemsToSpawn;
					AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BULLETBANK_DEFEATED, true);//Done
					for (int i = 0; i < itemsToSpawn; i++)
					{
						int id = BraveUtility.RandomElement<int>(Shellrax.Lootdrops);
						LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, base.aiActor.sprite.WorldCenter, new Vector2(spewItemDir * itemsToSpawn, spewItemDir * itemsToSpawn), 2.2f, false, true, false);
					}


					float value = UnityEngine.Random.Range(0.00f, 1.00f);
					if (value <= 0.4f)
					{
						Chest chest2 = GameManager.Instance.RewardManager.SpawnTotallyRandomChest(GameManager.Instance.PrimaryPlayer.CurrentRoom.GetRandomVisibleClearSpot(1, 1));
						chest2.IsLocked = false;
						chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
					}

				}; ;
				this.aiActor.knockbackDoer.SetImmobile(true, "nope.");
			}
		}

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
			//idle (broken)
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_001.png",//0
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_002.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_003.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_004.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_005.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_006.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_007.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_008.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_009.png",
			"Planetside/Resources/Bosses/HMPrime/hmprime_brokenidle_010.png",//9

			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro1.png",//10
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro2.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro3.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro4.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro5.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro6.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro7.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro8.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro9.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro10.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro11.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro12.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro13.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro14.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro15.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro16.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro17.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro18.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro19.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro20.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro21.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro22.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro23.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro24.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro25.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro26.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro27.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro28.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro29.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro30.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro31.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro32.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro33.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro34.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro35.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro36.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro37.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro38.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro39.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro40.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro41.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro42.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro43.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro44.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro45.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro46.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro47.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro48.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro49.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro50.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro51.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro52.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro53.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro54.png",
			"Planetside/Resources/Bosses/HMPrime/Intro/hmprime_intro55.png",//64

			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_down1.png",//65
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_down2.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_down3.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_down4.png",//68

			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_down1.png",//69
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_down2.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_down3.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_down4.png",//72

			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_top1.png",//73
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_top2.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_top3.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_left_top4.png",//76

			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_top1.png",//77
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_top2.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_top3.png",
			"Planetside/Resources/Bosses/HMPrime/Idle/hmprime_idle_right_top4.png",//80



			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_down1.png",//81
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_down2.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_down3.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_down4.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_down5.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_down6.png",//86

			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_down1.png",//87
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_down2.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_down3.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_down4.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_down5.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_down6.png",//92

			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_up1.png",//93
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_up2.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_up3.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_up4.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_up5.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_left_up6.png",//98

			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_up1.png",//99
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_up2.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_up3.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_up4.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_up5.png",
			"Planetside/Resources/Bosses/HMPrime/Walk/hmprime_walk_right_up6.png",//104

		};
	}
}








