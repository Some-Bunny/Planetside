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
using System.Reflection;

namespace Planetside
{
	public class Bloat : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "bloat_isaac_reference";
		private static tk2dSpriteCollectionData FodderColection;
		public static GameObject shootpoint;
		public static void Init()
		{
			Bloat.BuildPrefab();
		}

		public static void BuildPrefab()
		{
			//
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Bloat", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false);
				var companion = prefab.AddComponent<EnemyBehavior>();
				companion.aiActor.knockbackDoer.weight = 100;
				companion.aiActor.MovementSpeed = 0.5f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = true;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(5f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.IgnoreForRoomClear = true;

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
					ManualOffsetX = 5,
					ManualOffsetY = 6,
					ManualWidth = 19,
					ManualHeight = 29,
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
					ManualOffsetX = 5,
					ManualOffsetY = 6,
					ManualWidth = 19,
					ManualHeight = 29,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
				});
				//companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "die", new string[] { "die" }, new DirectionalAnimation.FlipType[1]);
				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
					{
						"idle",
						"idle"
					}
				};

				
				bool flag3 = FodderColection == null;
				if (flag3)
				{
					FodderColection = SpriteBuilder.ConstructCollection(prefab, "FodderColection");
					UnityEngine.Object.DontDestroyOnLoad(FodderColection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], FodderColection);
					}


					/*
					GameObject FodderColectionObject = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("FodderCollection");
					tk2dSpriteCollectionData FodderColection = FodderColectionObject.GetComponent<tk2dSpriteCollectionData>();
					if (FodderColection != null && FodderColection != null)
					{
						UnityEngine.Object.DontDestroyOnLoad(FodderColectionObject);
						UnityEngine.Object.DontDestroyOnLoad(FodderColection);
						Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("assets/Enemies/Fodder/FodderCollection Data/atlas0 material.mat");

						UnityEngine.Object.DontDestroyOnLoad(mat);
						FodderColection.material = mat;

						Texture texture = mat.GetTexture("_MainTex");
						texture.filterMode = FilterMode.Point;

						//FodderColection.textures = new Texture[] { texture };

						FodderColection.materials = new Material[]
						{
							mat
						};
						companion.sprite.usesOverrideMaterial = true;
						companion.sprite.renderer.material = mat;
						companion.sprite.renderer.material.SetTexture("_MainTex", texture);
						*/

					SpriteBuilder.AddAnimation(companion.spriteAnimator, FodderColection, new List<int>
					{
					0,
					0,
					1,
					1,
					2,
					2,
					2,
					3,
					4,
					4,

					}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 4f;


					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "idle", new Dictionary<int, string> { { 7, "Play_ENM_blobulord_reform_01" } });

					/*
					 * 3,
					4,
					4,
					*/
					SpriteBuilder.AddAnimation(companion.spriteAnimator, FodderColection, new List<int>
					{
					0,
					0,
					1,
					1,
					2,
					2,
					2,
					
					}, "bonkcharge", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
					EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "bonkcharge", new string[] { "bonkcharge" }, new DirectionalAnimation.FlipType[0]);
					SpriteBuilder.AddAnimation(companion.spriteAnimator, FodderColection, new List<int>
					{
					3,
					4,
					4,

					}, "bonk", tk2dSpriteAnimationClip.WrapMode.Once).fps = 5f;
					EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "bonk", new string[] { "bonk" }, new DirectionalAnimation.FlipType[0]);
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "bonk", new Dictionary<int, string> { { 0, "Play_ENM_blobulord_reform_01" } });
					SpriteBuilder.AddAnimation(companion.spriteAnimator, FodderColection, new List<int>
					{
					5,
					5,
					6,
					6,
					7,
					8,
					9,
					10
					//17
					}, "die", tk2dSpriteAnimationClip.WrapMode.Once).fps =4f;
				}
				EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "die", new Dictionary<int, string> { { 3, "Play_ENM_blobulord_intro_01" }, { 1, "Play_BOSS_dragun_charge_01" },{6, "Play_ENM_blobulord_splash_01" } });
				EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "die", new Dictionary<int, string> { { 6, "Sploosh" } });
				//m_ENM_blobulord_splash_01
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
				bs.MovementBehaviors = new List<MovementBehaviorBase>
				{
				new ModifiedDashMoveBehavior
				{
				   dashDistance = 1.66f,
				   dashTime = 2,
				   Cooldown = 2,
				   preDashAnim = "bonkcharge",
				   DashAnim = "bonk"
				}
				};
				
				

				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:bloat", companion.aiActor);


				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Bloat/bigbloat_idle_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:bloat";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Bloat/bigbloat_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\sheetBolatTrespass.png");
				PlanetsideModule.Strings.Enemies.Set("#THE_BLOAT", "Bloat");
				PlanetsideModule.Strings.Enemies.Set("#THE_BLOAT_SHORTDESC", "Planted");
				PlanetsideModule.Strings.Enemies.Set("#THE_BLOAT_LONGDESC", "Carrying the remains of dozens that fell under its influence, any little amount of force will release all of the contents within.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#THE_BLOAT";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#THE_BLOAT_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#THE_BLOAT_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:bloat");
				EnemyDatabase.GetEntry("psog:bloat").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:bloat").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:bloat").isNormalEnemy = true;

				companion.aiActor.sprite.usesOverrideMaterial = true;
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
				mat.SetFloat("_EmissiveColorPower", 3f);
				mat.SetFloat("_EmissivePower", 40);
				companion.aiActor.sprite.renderer.material = mat;

				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.UndodgeableDirectedfireSoundless);
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
			}
		}



		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/Bloat/bigbloat_idle_001.png",
			"Planetside/Resources/Enemies/Bloat/bigbloat_idle_002.png",
			"Planetside/Resources/Enemies/Bloat/bigbloat_idle_003.png",
			"Planetside/Resources/Enemies/Bloat/bigbloat_idle_004.png",
			"Planetside/Resources/Enemies/Bloat/bigbloat_idle_005.png",

			"Planetside/Resources/Enemies/Bloat/bigbloat_death_001.png",
			"Planetside/Resources/Enemies/Bloat/bigbloat_death_002.png",
			"Planetside/Resources/Enemies/Bloat/bigbloat_death_003.png",
			"Planetside/Resources/Enemies/Bloat/bigbloat_death_004.png",
			"Planetside/Resources/Enemies/Bloat/bigbloat_death_005.png",
			"Planetside/Resources/Enemies/Bloat/bigbloat_death_006.png",


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
				this.aiActor.knockbackDoer.SetImmobile(true, "IM A BELL.");
				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;

				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{


				};
			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Sploosh"))
				{
					SpawnManager.SpawnBulletScript(base.aiActor, base.aiActor.sprite.WorldCenter, base.aiActor.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(DIE)), StringTableManager.GetEnemiesString("#THE_BLOAT", -1));
					Exploder.DoDistortionWave(base.aiActor.sprite.WorldCenter, 20f, 0.3f, 30, 0.5f);
				


				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("SpedC"))
				{
					//this.aiActor.StartCoroutine(SpeedLerp());
				}
			}
			private IEnumerator SpeedLerp()
			{
				float s = base.aiActor.MovementSpeed;
				float ela = 0;
				while (ela <2)
                {
					float t = ela / 1.5f;
					ela += BraveTime.DeltaTime;
					base.aiActor.MovementSpeed = Mathf.Lerp(s * 4f, s, t);
					yield return null;
				}
				yield break;
			}
		}

		public class DIE : Script 
		{
			protected override IEnumerator Top()
			{
				for (int i = 0; i <= 12; i++)
				{
					this.Fire(new Direction(i * 30, DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new Burst());
					this.Fire(new Direction(i * 30, DirectionType.Aim, -1f), new Speed(8f, SpeedType.Absolute), new Burst());
				}
				for (int i = 0; i <= 60; i++)
                {
					this.Fire(new Direction(base.RandomAngle(), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(3, 6), SpeedType.Absolute), new FUCKFUCKFUCKFUCKFUUCK(UnityEngine.Random.Range(30, 150)));
					if (i % 3 == 0)
                    {
						this.Fire(new Direction(base.RandomAngle(), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(8, 13), SpeedType.Absolute), new FUCKFUCKFUCKFUCKFUUCK(UnityEngine.Random.Range(60, 180)));
					}
				}
				yield break;
			}
		}



		public class FUCKFUCKFUCKFUCKFUUCK : Bullet
        {
			public FUCKFUCKFUCKFUCKFUUCK(int RNG) : base(UnityEngine.Random.value > 0.33f ? StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name : StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name, false, false, false) 
			{
				RNGSPeedChange = RNG;
			}
			protected override IEnumerator Top()
			{
				base.ChangeSpeed(new Brave.BulletScript.Speed(0, SpeedType.Absolute), RNGSPeedChange);
				yield return this.Wait(UnityEngine.Random.Range(240, 600));
				base.Vanish(false);
				yield break;
			}
			public int RNGSPeedChange;
		}

		public class Burst : Bullet
		{
			public Burst() : base(StaticUndodgeableBulletEntries.UndodgeableDirectedfireSoundless.Name, false, false, false){}
			protected override IEnumerator Top()
			{
				base.ChangeSpeed(new Brave.BulletScript.Speed(UnityEngine.Random.Range(14, 20), SpeedType.Absolute), 50);
				base.ChangeDirection(new Brave.BulletScript.Direction(UnityEngine.Random.Range(-20, 20), DirectionType.Relative), 40);
				yield break;
			}
		}

	}
}





