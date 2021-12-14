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
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\SomethingWickedAmmonomiconSheet.png");
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
			MaxSpeed = 11;
			HasTPed = false;
			CanBeDMG = true;
			RanDir = 0;
			AkSoundEngine.PostEvent("Play_ambienthum", base.gameObject);
			Rigidbody = base.gameObject.GetComponent<SpeculativeRigidbody>();
			Rigidbody.OnPreRigidbodyCollision += DoCollision;
			base.gameObject.GetComponent<tk2dSpriteAnimator>().Play("idle");
			this.m_currentTargetPlayer = GameManager.Instance.GetRandomActivePlayer();

		}
		bool HasPickedUpRedCasing;
		private bool HasRedCasing;


		private void Update()
		{
			Glow();

			HasRedCasing = m_currentTargetPlayer.HasPickupID(RedThing.RedCasingID) ? true : false;
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
			DoMotion();

			foreach (Projectile proj in StaticReferenceManager.AllProjectiles)
			{
				PlayerController player = proj.Owner as PlayerController;
				bool isBem = proj.GetComponent<BasicBeamController>() != null;
				if (isBem == true && Planetside.BeamToolbox.PosIsNearAnyBoneOnBeam(proj.GetComponent<BasicBeamController>(), base.gameObject.GetComponent<tk2dBaseSprite>().sprite.WorldCenter, 6f) && proj.Owner != null && proj.Owner == player)
				{
					AkSoundEngine.PostEvent("Play_CHR_shadow_curse_01", base.gameObject);
					MaxSpeed += 1.5f;
					GameManager.Instance.StartCoroutine(this.TPCooldown());
					GameManager.Instance.StartCoroutine(this.DMGCooldown());
					RanDir = UnityEngine.Random.Range(-180, 180);
				}
				else 
				
				if (Vector2.Distance(proj.sprite.WorldCenter, base.gameObject.GetComponent<tk2dBaseSprite>().sprite.WorldCenter) < 6 && proj.Owner != null && proj.Owner == player && HasTPed != true && CanBeDMG == true)
				{
					AkSoundEngine.PostEvent("Play_CHR_shadow_curse_01", base.gameObject);
					MaxSpeed += 1.5f;
					GameManager.Instance.StartCoroutine(this.TPCooldown());
					GameManager.Instance.StartCoroutine(this.DMGCooldown());
					RanDir = UnityEngine.Random.Range(-180, 180);
				}
			}
		}
		private IEnumerator TPCooldown()
		{
			ImprovedAfterImage img = base.gameObject.GetComponent<ImprovedAfterImage>();
			img.enabled = false;
			base.gameObject.GetComponent<tk2dBaseSprite>().renderer.enabled = false;
			HasTPed = true;
			float Wait = HasPickedUpRedCasing ? 0.5f : 0.75f;
			float Divide = HasPickedUpRedCasing ? 26.66f : 20;
			yield return new WaitForSeconds((MaxSpeed / Divide) + Wait);
			img.enabled = true;
			base.gameObject.GetComponent<tk2dBaseSprite>().renderer.enabled = true;
			HasTPed = false;

			yield break;
		}
		private IEnumerator DMGCooldown()
        {
			CanBeDMG = false;
			float Mult = HasPickedUpRedCasing == true ? 2 : 1.4f;
			yield return new WaitForSeconds((MaxSpeed * Mult));
			CanBeDMG = true;
			yield break;
		}

		private void OnDestroy()
        {
			AkSoundEngine.PostEvent("Stop_ambienthum", base.gameObject);

		}
		void Glow()
        {
			if (m_currentTargetPlayer && CanBeDMG == true)
            {
				base.gameObject.GetComponent<tk2dBaseSprite>().sprite.renderer.material.SetFloat("_EmissivePower", 75);
			}
			else
            {
				base.gameObject.GetComponent<tk2dBaseSprite>().sprite.renderer.material.SetFloat("_EmissivePower", 0);
			}
		}
		private float RanDir;
		private bool HasTPed;
		private bool CanBeDMG;

		private void DoMotion()
		{
			Rigidbody.Velocity = Vector2.zero;
			if (base.gameObject.GetComponent<tk2dSpriteAnimator>().IsPlaying("start"))
			{
				return;
			}
			if (this.m_currentTargetPlayer.healthHaver.IsDead || this.m_currentTargetPlayer.IsGhost)
			{
				this.m_currentTargetPlayer = GameManager.Instance.GetRandomActivePlayer();
			}

			float Multiplier = HasPickedUpRedCasing ? 1.2f : 1;
			Vector2 centerPosition = HasTPed ? Rigidbody.UnitCenter + MathToolbox.GetUnitOnCircle(RanDir, 9) :this.m_currentTargetPlayer.CenterPosition;	
			Vector2 vector =  centerPosition - Rigidbody.UnitCenter;
			float magnitude =vector.magnitude;
			float d = Mathf.Lerp(4.2f* Multiplier, MaxSpeed, (magnitude - (MaxSpeed*Multiplier)) / (40 - 5));
			Rigidbody.Velocity = !HasTPed ? vector.normalized * d : vector.normalized * (d*40);
			StoredVelocity = Rigidbody.Velocity;
		}

		Vector2 StoredVelocity;
		private void DoCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			PhysicsEngine.SkipCollision = true;
			if (otherRigidbody.gameObject.GetComponent<PlayerController>() && HasTPed != true)
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
		private float MaxSpeed;
		public static GameObject SomethingWickedObject = new GameObject();
		private SpeculativeRigidbody Rigidbody;
		private PlayerController m_currentTargetPlayer;
	}
}