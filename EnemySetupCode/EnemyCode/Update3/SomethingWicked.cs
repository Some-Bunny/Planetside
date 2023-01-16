using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dungeonator;
using UnityEngine;
//using ItemAPI;
using Gungeon;
using GungeonAPI;
using System.Collections;
using ItemAPI;

namespace Planetside
{
	class SomethingWickedEnemy : MonoBehaviour
	{

		public static void Init()
		{
			GameObject gameObject = ItemAPI.SpriteBuilder.SpriteFromResource("Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_001");
			tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 5;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "idle";
			GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_001", spriteObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 5; i++)
			{
				GameObject spriteForObject = new GameObject("spriteForObject");
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_00{i}", spriteForObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
				frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
				animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
			animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
			animator.DefaultClipId = animator.GetClipIdByName("idle");
			animator.playAutomatically = true;

			gameObject.AddComponent<SomethingWickedEnemy>();
			ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
			gameObject.SetActive(false);


			SpeculativeRigidbody specBody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(0, 0), new IntVector2(49, 56));
			specBody.PixelColliders.Clear();

			specBody.PixelColliders.Add(new PixelCollider
			{
				ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
				CollisionLayer = CollisionLayer.EnemyCollider,
				IsTrigger = false,
				BagleUseFirstFrameOnly = false,
				SpecifyBagelFrame = string.Empty,
				BagelColliderNumber = 0,
				ManualOffsetX = 15,
				ManualOffsetY = 8,
				ManualWidth = 29,
				ManualHeight = 43,
				ManualDiameter = 0,
				ManualLeftX = 0,
				ManualLeftY = 0,
				ManualRightX = 0,
				ManualRightY = 0,

			});

			specBody.PixelColliders.Add(new PixelCollider
			{

				ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
				CollisionLayer = CollisionLayer.EnemyHitBox,
				IsTrigger = false,
				BagleUseFirstFrameOnly = false,
				SpecifyBagelFrame = string.Empty,
				BagelColliderNumber = 0,
				ManualOffsetX = 15,
				ManualOffsetY = 8,
				ManualWidth = 29,
				ManualHeight = 43,
				ManualDiameter = 0,
				ManualLeftX = 0,
				ManualLeftY = 0,
				ManualRightX = 0,
				ManualRightY = 0,
			});



			specBody.CollideWithTileMap = false;
			specBody.CollideWithOthers = true;
			gameObject.GetComponent<tk2dBaseSprite>().sprite.usesOverrideMaterial = true;
			Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.mainTexture = gameObject.GetComponent<tk2dBaseSprite>().sprite.renderer.material.mainTexture;
			mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
			mat.SetFloat("_EmissiveColorPower", 1.55f);
			mat.SetFloat("_EmissivePower", 100);
			gameObject.GetComponent<tk2dBaseSprite>().sprite.renderer.material = mat;

			Planetside.ImprovedAfterImage yes = gameObject.AddComponent<Planetside.ImprovedAfterImage>();
			yes.spawnShadows = true;
			yes.shadowLifetime = 0.2f;
			yes.shadowTimeDelay = 0.001f;
			yes.dashColor = Color.black;
			yes.name = "SW trail";
			yes.enabled = true;

			SomethingWickedObject = gameObject;
		}


		public static GameObject prefab;
		public static readonly string guid = "something_wicked";
		private static tk2dSpriteCollectionData SWCollection;
		public static GameObject shootpoint;

		public static void InitDummyEnemy()
		{

			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Something Wicked", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 800;
				companion.aiActor.MovementSpeed = 0f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = true;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(2f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.healthHaver.SetHealthMaximum(2f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 10,
					ManualOffsetY = 3,
					ManualWidth = 15,
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
					ManualWidth = 15,
					ManualHeight = 20,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").CorpseObject;
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
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
					{
						"idle_left",
						"idle_right"
					}
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
						{
						"run_left",
						"run_right"
						}
				};
				bool flag3 = SWCollection == null;
				if (flag3)
				{
					SWCollection = SpriteBuilder.ConstructCollection(prefab, "FodderBoi_Collection");
					UnityEngine.Object.DontDestroyOnLoad(SWCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], SWCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SWCollection, new List<int>
					{

					0,
					1,
					2,
					3,

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SWCollection, new List<int>
					{
					0,
					1,
					2,
					3,


					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SWCollection, new List<int>
					{
					0,
					1,
					2,
					3,
					


					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SWCollection, new List<int>
					{

					0,
					1,
					2,
					3,


					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SWCollection, new List<int>
					{

				 0



					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, SWCollection, new List<int>
					{

				 0

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;

				}
				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
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
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.25f,
				}
				};
				
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:something_wicked", companion.aiActor);


				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_001", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:something_wicked";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("SomethingWickedAmmonomiconSheet");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\SomethingWickedAmmonomiconSheet.png");
                PlanetsideModule.Strings.Enemies.Set("#WICKEDNESS", "Something Wicked");
				PlanetsideModule.Strings.Enemies.Set("#WICKEDNESS_SHORTDESC", "Relic Of The Past");
				PlanetsideModule.Strings.Enemies.Set("#WICKEDNESS_LONGDESC", "The shade of a priest who resided in the Gungeon since before the Bullet struck, disgusted by the Lich's practices, sook to reclaim the Gungeon and restore it to its former glory.\n\n\n *Text illegible* \n\n\nNow, all that remains of him are the shrines scattered about the Gungeon, with those kneeling at it enacting the beliefs of the priest, unable to do so himself.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#WICKEDNESS";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#WICKEDNESS_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#WICKEDNESS_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:something_wicked");
				EnemyDatabase.GetEntry("psog:something_wicked").ForcedPositionInAmmonomicon = 10000;
				EnemyDatabase.GetEntry("psog:something_wicked").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:something_wicked").isNormalEnemy = true;
				

			}
		}

		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_001",
			"Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_002",
			"Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_003",
			"Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_004",

		};

		private void Start()
		{
			currentState = States.NONE;

			base.gameObject.GetComponent<tk2dSpriteAnimator>().Play("idle");
			Rigidbody = base.gameObject.GetComponent<SpeculativeRigidbody>();
			Rigidbody.OnPreRigidbodyCollision += DoCollision;
			base.StartCoroutine(LerpGlowToValue(0, 0, 0.1f));
			this.PlayerToTrack = GameManager.Instance.GetRandomActivePlayer();
			glowMaterial = base.gameObject.GetComponent<tk2dBaseSprite>().sprite.renderer.material;

		}
		bool HasPickedUpRedCasing;
		private bool HasRedCasing;


		private void Update()
		{
			HasRedCasing = PlayerToTrack.HasPickupID(RedThing.RedCasingID) ? true : false;
			if (HasPickedUpRedCasing != true && HasRedCasing == true)
            {
				HasPickedUpRedCasing = true;
			}
			if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
			{
				return;
			}
			if (BossKillCam.BossDeathCamRunning || GameManager.Instance.PreventPausing)
			{
				return;
			}
			if (TimeTubeCreditsController.IsTimeTubing)
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (currentState == States.NONE)
            { StartLurk(); }
			if (currentState == States.WATCH)
			{Watch();}
			if (currentState == States.HUNT)
			{ Hunt(); }
		}


		public void RollBreakableBreak()
		{
            if (UnityEngine.Random.value < 0.005)
            {
				if (StaticReferenceManager.AllMinorBreakables == null | StaticReferenceManager.AllMinorBreakables.Count == 0) { return; }
                var breakable = StaticReferenceManager.AllMinorBreakables[UnityEngine.Random.Range(0, StaticReferenceManager.AllMinorBreakables.Count)];
                if (breakable)
                {
                    foreach (PlayerController player in GameManager.Instance.AllPlayers)
                    {
                        if (Vector2.Distance(breakable.transform.PositionVector2(), player.transform.PositionVector2()) > 6 && Vector2.Distance(breakable.transform.PositionVector2(), player.transform.PositionVector2()) < 14)
						{
                            breakable.Break();
                        }
                    }
                }
            }
        }

		private void StartLurk()
        {
			currentState = States.LURK;
			Dungeon currentFloor = GameManager.Instance.Dungeon;
			do
			{
				RoomHandler roomSelected = GameManager.Instance.Dungeon.data.rooms[UnityEngine.Random.Range(0, GameManager.Instance.Dungeon.data.rooms.Count)];
				IntVector2? positionSelected = roomSelected.GetRandomAvailableCell();
				if (roomSelected != null)
                {
					RollBreakableBreak();



                    foreach (PlayerController player in GameManager.Instance.AllPlayers)
					{
						if (player.CurrentRoom != null && player.CurrentRoom != roomSelected && Vector2.Distance(positionSelected.Value.ToCenterVector2(), player.transform.position) > 35 && Vector2.Distance(positionSelected.Value.ToCenterVector2(), player.transform.position) < 50)
						{
							PlayerToTrack = player;
							base.gameObject.transform.position = positionSelected.Value.ToVector3();
							currentState = States.WATCH;
							base.StartCoroutine(LerpGlowToValue(0, 70, 0.1f));
							AkSoundEngine.PostEvent("Play_ambienthum", base.gameObject);
						}
					}
				}
			}
			while (currentState == States.LURK);
			bool flag = currentState == States.WATCH;
			if (flag)
			{

			}
        }

		private void Watch()
        {
            RollBreakableBreak();

            if (Vector2.Distance(base.transform.position, PlayerToTrack.transform.position) < 14)
            {
				currentState = States.PRE_HUNT;
				base.StartCoroutine(LerpGlowToValue(glowMaterial.GetFloat("_EmissivePower"), 70, 1f));
				base.StartCoroutine(StartHunt(UnityEngine.Random.Range(1, 4)));

			}
            else
            {
				if (this.PlayerToTrack.healthHaver.IsDead || this.PlayerToTrack.IsGhost)
				{
					this.PlayerToTrack = GameManager.Instance.GetRandomActivePlayer();
				}
				float Multiplier = HasPickedUpRedCasing ? 1.33f : 1;
				Vector2 centerPosition = this.PlayerToTrack.CenterPosition;
				Vector2 vector = centerPosition - Rigidbody.UnitCenter;
				float magnitude = vector.magnitude;
				float d = Mathf.Lerp(2f * Multiplier, 9, (magnitude - (9 * Multiplier)) / (40 - 5));
				Rigidbody.Velocity = vector.normalized * d;
				StoredVelocity = Rigidbody.Velocity;
			}
		}



		private IEnumerator StartHunt(float WaitTime)
		{
			//AkSoundEngine.PostEvent("Play_OBJ_lock_pick_01", base.gameObject);


			Vector2 savedVelocity = Rigidbody.Velocity;
			float ela = 0f;
			while (ela < WaitTime)
			{
				float t = ela / (WaitTime / 2);
				if (Rigidbody != null) { Rigidbody.Velocity = Vector2.Lerp(savedVelocity, Vector2.zero, t); }
				ela += BraveTime.DeltaTime;
				yield return null;
			}
			currentState = States.HUNT;
			yield break;
		}
		private void Hunt()
		{
			if (Vector2.Distance(base.transform.position, PlayerToTrack.transform.position) < 7)
			{
				currentState = States.CHARGE;
				base.StartCoroutine(DoCharge());
			}
			else
            {
				if (this.PlayerToTrack.healthHaver.IsDead || this.PlayerToTrack.IsGhost)
				{
					this.PlayerToTrack = GameManager.Instance.GetRandomActivePlayer();
				}
				float Multiplier = HasPickedUpRedCasing ? 1.33f : 1;
				Vector2 centerPosition = this.PlayerToTrack.CenterPosition;
				Vector2 vector = centerPosition - Rigidbody.UnitCenter;
				float magnitude = vector.magnitude;
				float d = Mathf.Lerp(5f * Multiplier, 12, (magnitude - (12 * Multiplier)) / (40 - 5));
				Rigidbody.Velocity = vector.normalized * d;
				StoredVelocity = Rigidbody.Velocity;
			}		
		}

		public void ReinitializeProeprly()
        {
			base.gameObject.GetComponent<tk2dBaseSprite>().sprite.renderer.enabled = true;
			currentState = States.NONE;
        }


		private IEnumerator DoCharge()
		{
			AkSoundEngine.PostEvent("Play_ENM_beholster_teleport_01", base.gameObject);
			Vector2 savedVelocity = Rigidbody.Velocity;
			Vector2 vector = this.PlayerToTrack.CenterPosition - Rigidbody.UnitCenter;
			Vector2 velocityToUse = vector.normalized * 12;
			float ela = 0f;
			base.StartCoroutine(LerpGlowToValue(0, 5000, 0.5f));
			while (ela < 1f)
			{
				float t = ela / 0.8f;
				if (Rigidbody != null) { Rigidbody.Velocity = Vector2.Lerp(savedVelocity, Vector2.zero, t); }
				ela += BraveTime.DeltaTime;
				yield return null;
			}
			ela = 0f;
			while (ela < 0.25f)
			{
				float t = ela / 0.25f;
				if (Rigidbody != null) { Rigidbody.Velocity = Vector2.Lerp(Vector2.zero, velocityToUse * 6, t); }
				ela += BraveTime.DeltaTime;
				yield return null;
			}
			ela = 0f;
			while (ela < 2f)
			{
				ela += BraveTime.DeltaTime;
				yield return null;
			}
			base.StartCoroutine(LerpGlowToValue(glowMaterial.GetFloat("_EmissivePower"), 0, 0.5f));
			Rigidbody.Velocity = Vector2.zero;
			currentState = States.WAIT;
			AkSoundEngine.PostEvent("Stop_ambienthum", base.gameObject);
			base.gameObject.GetComponent<tk2dBaseSprite>().sprite.renderer.enabled = false;
			base.Invoke("ReinitializeProeprly", UnityEngine.Random.Range(4, HasRedCasing == true? 11 : 30));
			yield break;
		}

	




		private void OnDestroy()
        {AkSoundEngine.PostEvent("Stop_ambienthum", base.gameObject);}

		private IEnumerator LerpGlowToValue(float PreviousValue, float NewValue, float Time = 0.25f)
		{
			float ela = 0f;
			float dura = Time;
			while (ela < dura)
			{
				float t = ela / dura;
				if (base.gameObject != null && glowMaterial != null)
                {
					glowMaterial.SetFloat("_EmissivePower", Mathf.Lerp(PreviousValue, NewValue, t));
				}
				ela += BraveTime.DeltaTime;
				yield return null;
			}
			yield break;
		}

		

		public enum States
        {
			NONE,
			LURK,
			WATCH,
			PRE_HUNT,
			HUNT,
			CHARGE,
			WAIT
        };

		public States currentState;

		private void DoMotion()
		{
			/*
			Rigidbody.Velocity = Vector2.zero;
			if (base.gameObject.GetComponent<tk2dSpriteAnimator>().IsPlaying("start"))
			{
				return;
			}
			if (this.PlayerToTrack.healthHaver.IsDead || this.PlayerToTrack.IsGhost)
			{
				this.PlayerToTrack = GameManager.Instance.GetRandomActivePlayer();
			}

			float Multiplier = HasPickedUpRedCasing ? 1.2f : 1;
			Vector2 centerPosition = HasTPed ? Rigidbody.UnitCenter + MathToolbox.GetUnitOnCircle(RanDir, 9) :this.PlayerToTrack.CenterPosition;	
			Vector2 vector =  centerPosition - Rigidbody.UnitCenter;
			float magnitude =vector.magnitude;
			float d = Mathf.Lerp(4.2f* Multiplier, MaxSpeed, (magnitude - (MaxSpeed*Multiplier)) / (40 - 5));
			Rigidbody.Velocity = !HasTPed ? vector.normalized * d : vector.normalized * (d*40);
			StoredVelocity = Rigidbody.Velocity;
			*/
		}

		Vector2 StoredVelocity;
		private void DoCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			PhysicsEngine.SkipCollision = true;
			bool isAgressive = currentState == States.CHARGE | currentState == States.HUNT;
			if (otherRigidbody.gameObject.GetComponent<PlayerController>() && isAgressive == true)
			{
				if (otherRigidbody.gameObject.GetComponent<PlayerController>().characterIdentity == PlayableCharacters.Robot)
                {
					otherRigidbody.gameObject.GetComponent<PlayerController>().healthHaver.Armor = 1;
					otherRigidbody.gameObject.GetComponent<PlayerController>().healthHaver.ApplyDamage(0.5f, Vector2.zero, "Something Wicked", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
				}
				else
                {
					otherRigidbody.gameObject.GetComponent<PlayerController>().healthHaver.ForceSetCurrentHealth(0.5f);
					otherRigidbody.gameObject.GetComponent<PlayerController>().healthHaver.Armor = 0;
					otherRigidbody.gameObject.GetComponent<PlayerController>().healthHaver.ApplyDamage(0.5f, Vector2.zero, "Something Wicked", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
				}
			}
		}
		//private float MaxSpeed;
		public static GameObject SomethingWickedObject = new GameObject();
		private SpeculativeRigidbody Rigidbody;
		private PlayerController PlayerToTrack;
		private Material glowMaterial;
	}
}