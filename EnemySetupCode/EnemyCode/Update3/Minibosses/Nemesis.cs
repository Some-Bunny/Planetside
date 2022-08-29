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
using NpcApi;

namespace Planetside
{
	public class Nemesis : AIActor
	{

		public class NemesisEngageDoer : CustomEngageDoer
		{
			public NemesisEngageDoer()
			{
				this.PortalLifeTime = 4.5f;
				this.PortalSize = 0.3f;
			}


			public void Update()
			{

			}

			public override void StartIntro()
			{
				if (this.m_isFinished)
				{
					return;
				}
				this.aiActor.enabled = true;
				m_isFinished = true;
				base.aiActor.GetComponent<GenericIntroDoer>().TriggerSequence(GameManager.Instance.BestActivePlayer);
			}

			private IEnumerator PortalDoer(MeshRenderer portal, bool DestroyWhenDone = false)
			{
				float elapsed = 0f;
				while (elapsed < PortalLifeTime)
				{
					elapsed += BraveTime.DeltaTime;
					float t = elapsed / PortalLifeTime;
					if (portal.gameObject == null) { yield break; }
					float throne1 = Mathf.Sin(t * (Mathf.PI));
					portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0, PortalSize, throne1));
					portal.material.SetFloat("_HoleEdgeDepth", Mathf.Lerp(12, 2, throne1));
					yield return null;
				}

				if (DestroyWhenDone == true)
				{
					Destroy(portal.gameObject);
				}
				yield break;
			}

			private IEnumerator DoIntro()
			{
				m_isFinished = true;
				this.aiActor.enabled = false;
				this.behaviorSpeculator.enabled = false;
				this.aiActor.ToggleRenderers(false);
				this.specRigidbody.enabled = false;
				this.aiActor.IgnoreForRoomClear = true;
				this.aiActor.IsGone = true;
				if (this.aiShooter)
				{
					this.aiShooter.ToggleGunAndHandRenderers(false, "GuardIsSpawning");
				}
				this.aiActor.ToggleRenderers(true);
				this.aiActor.healthHaver.PreventAllDamage = true;
				this.aiActor.enabled = true;
				this.specRigidbody.enabled = true;
				this.aiActor.IsGone = false;
				this.aiActor.IgnoreForRoomClear = false;
				this.aiAnimator.PlayDefaultAwakenedState();
				this.aiActor.State = AIActor.ActorState.Awakening;
				int playerMask = CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.PlayerHitBox);
				this.aiActor.specRigidbody.AddCollisionLayerIgnoreOverride(playerMask);

				if (HasSpawnedPortal != true)
				{
					GameObject portalObj = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX, this.aiActor.sprite.WorldBottomCenter, Quaternion.Euler(0f, 0f, 0f));
					portalObj.layer = this.aiActor.gameObject.layer + (int)GameManager.Instance.MainCameraController.CurrentZOffset;
					portalObj.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
					MeshRenderer mesh = portalObj.GetComponent<MeshRenderer>();
					mesh.material.SetTexture("_PortalTex", StaticTextures.NebulaTexture);
					GameManager.Instance.StartCoroutine(PortalDoer(mesh, true));
					HasSpawnedPortal = true;
				}


				while (this.aiAnimator.IsPlaying("awaken"))
				{
					this.behaviorSpeculator.enabled = false;
					if (this.aiShooter)
					{
						this.aiShooter.ToggleGunAndHandRenderers(false, "GuardIsSpawning");
					}
					yield return null;
				}

				if (this.aiShooter)
				{
					this.aiShooter.ToggleGunAndHandRenderers(true, "GuardIsSpawning");
				}
				AkSoundEngine.PostEvent("Play_OBJ_weapon_pickup_01", base.aiActor.gameObject);
				GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.BlueSynergyPoofVFX, false);
				gameObject.transform.position = base.aiActor.transform.Find("GunAttachPoint").gameObject.transform.position;
				gameObject.transform.localScale *= 1.5f;
				Destroy(gameObject, 2);

				GameObject gameObjectTwo = SpawnManager.SpawnVFX(StaticVFXStorage.MachoBraceDustupVFX, false);
				gameObjectTwo.transform.position = base.aiActor.transform.position - new Vector3(1.25f, 1.25f);
				Destroy(gameObjectTwo, 2);

				yield return new WaitForSeconds(0.25f);
				this.aiActor.healthHaver.PreventAllDamage = false;
				this.behaviorSpeculator.enabled = true;
				this.aiActor.specRigidbody.RemoveCollisionLayerIgnoreOverride(playerMask);
				this.aiActor.HasBeenEngaged = true;
				this.aiActor.State = AIActor.ActorState.Normal;
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

			public float PortalLifeTime;
			public float PortalSize;

			private bool HasSpawnedPortal;
			private bool m_isFinished;
		}

		public static GameObject prefab;
		public static readonly string guid = "nemesis";
		private static tk2dSpriteCollectionData nemesisCollection;
		public static GameObject DummySpriteObject;

		private static Texture2D BossCardTexture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside/Resources/BossCards/nemesis_bosscard.png");

		public static void Init()
		{
			Nemesis.BuildPrefab();
		}



		public static void BuildPrefab()
		{
			
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{


				ETGMod.Databases.Strings.Core.Set("#CLOCKHAIR_SPECIAL_DEATH_1", "Attempting to Teabag");
				ETGMod.Databases.Strings.Core.Set("#CLOCKHAIR_SPECIAL_DEATH_2", "Intentional Sabotage?");
				ETGMod.Databases.Strings.Core.Set("#CLOCKHAIR_SPECIAL_DEATH_3", "Poor Spacing Discipline");
				ETGMod.Databases.Strings.Core.Set("#CLOCKHAIR_SPECIAL_DEATH_4", "Body Blocking");

				GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/Guons/EnergyPlatedGuon/energyshiledguon.png");
				gameObject.name = $"Bullet orbital";
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				FakePrefab.MarkAsFakePrefab(gameObject);
				gameObject.SetActive(false);
				DummySpriteObject = gameObject;

				prefab = EnemyBuilder.BuildPrefab("nemesis", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), true, true);
				var companion = prefab.AddComponent<EnemyBehavior>();
				prefab.AddComponent<NemesisController>();
				prefab.AddComponent<ForgottenEnemyComponent>();
				companion.aiActor.knockbackDoer.weight = 120;
				companion.aiActor.MovementSpeed = 4f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;

				companion.aiActor.PreventFallingInPitsEver = true;



				companion.aiActor.healthHaver.ForceSetCurrentHealth(200f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;

				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.defaultShadow, new Vector2(0.625f, 0.25f), "shadowPos");


				companion.aiActor.healthHaver.SetHealthMaximum(200f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 1,
					ManualOffsetY = 4,
					ManualWidth = 13,
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
					ManualOffsetX = 1,
					ManualOffsetY = 4,
					ManualWidth = 13,
					ManualHeight = 20,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,
					Enabled = true,
				});
				companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").CorpseObject;
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;

				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.FourWay,
					Prefix = "idle",
					AnimNames = new string[] { "idle_top_left", "idle_bottom_right", "idle_bottom_left", "idle_top_right" },
					Flipped = new DirectionalAnimation.FlipType[4]
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.FourWay,
					Flipped = new DirectionalAnimation.FlipType[4],
					AnimNames = new string[]
					{
						"run_top_left",
						"run_bottom_right",
						"run_bottom_left",
						"run_top_right",
					}
				};


				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "intro", new string[] { "intro" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

				//EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "dodgeroll", new string[] { "dodge_top_left", "dodge_bottom_right", "dodge_bottom_left", "dodge_top_right" }, new DirectionalAnimation.FlipType[4], DirectionalAnimation.DirectionType.FourWay);
			

				bool flag3 = nemesisCollection == null;
				if (flag3)
				{
					nemesisCollection = SpriteBuilder.ConstructCollection(prefab, "nemesisCollection");
					UnityEngine.Object.DontDestroyOnLoad(nemesisCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], nemesisCollection);
					}

					//=======================================================================================================================
					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					0,
					1,
					2,
					3,
					}, "idle_bottom_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					4,
					5,
					6,
					7
					}, "idle_bottom_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					8,
					9,
					10,
					11,
					}, "idle_top_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					12,
					13,
					14,
					15,
					}, "idle_top_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
					//=======================================================================================================================


					//=======================================================================================================================
					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					16,
					17,
					18,
					19,
					20,
					21
					}, "run_bottom_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					22,
					23,
					24,
					25,
					26,
					27
					}, "run_bottom_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					28,
					29,
					30,
					31,
					32,
					33
					}, "run_top_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					34,
					35,
					36,
					37,
					38,
					39
					}, "run_top_right", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 6f;
					//=======================================================================================================================

					EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "run_bottom_left", new Dictionary<int, string>() { { 2, "Play_NemesisStep" }, { 5, "Play_NemesisStep" } });
					EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "run_bottom_right", new Dictionary<int, string>() { { 2, "Play_NemesisStep" }, { 5, "Play_NemesisStep" } });
					EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "run_top_left", new Dictionary<int, string>() { { 2, "Play_NemesisStep" }, { 5, "Play_NemesisStep" } });
					EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "run_top_right", new Dictionary<int, string>() { { 2, "Play_NemesisStep" }, { 5, "Play_NemesisStep" } });

					//=======================================================================================================================
					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					40,
					41,
					42,
					43,
					44,
					45,
					46,
					47,
					48
					}, "dodge_bottom_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 12f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					49,
					50,
					51,
					52,
					53,
					54,
					55,
					56,
					57,
				
					}, "dodge_bottom_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 12f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					58,
					59,
					60,
					61,
					62,
					63,
					64,
					65,
					66,
					
					}, "dodge_top_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 12f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					67,
					68,
					69,
					70,
					71,
					72,
					73,
					74,
					75,		
					}, "dodge_top_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 12f;
					//=======================================================================================================================


					EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "dodge_top_right", new Dictionary<int, string>() { { 0, "Play_Dodge" } });
					EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "dodge_top_left", new Dictionary<int, string>() { { 0, "Play_Dodge" } });
					EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "dodge_bottom_right", new Dictionary<int, string>() { { 0, "Play_Dodge" } });
					EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "dodge_bottom_left", new Dictionary<int, string>() { {0, "Play_Dodge" } });

					EnemyToolbox.AddInvulnverabilityFramesToAnimation(companion.spriteAnimator, "dodge_top_right", new Dictionary<int, bool>() { { 0, true }, { 1, true }, { 2, true }, { 3, true }, { 4, true }, { 5, false }, { 6, false }, { 7, false }, { 8, false } } );
					EnemyToolbox.AddInvulnverabilityFramesToAnimation(companion.spriteAnimator, "dodge_top_left", new Dictionary<int, bool>() { { 0, true }, { 1, true }, { 2, true }, { 3, true }, { 4, true }, { 5, false }, { 6, false }, { 7, false }, { 8, false } });
					EnemyToolbox.AddInvulnverabilityFramesToAnimation(companion.spriteAnimator, "dodge_bottom_right", new Dictionary<int, bool>() { { 0, true }, { 1, true }, { 2, true }, { 3, true }, { 4, true }, { 5, false }, { 6, false }, { 7, false }, { 8, false } });
					EnemyToolbox.AddInvulnverabilityFramesToAnimation(companion.spriteAnimator, "dodge_bottom_left", new Dictionary<int, bool>() { { 0, true }, { 1, true }, { 2, true }, { 3, true }, { 4, true }, { 5, false }, { 6, false }, { 7, false }, { 8, false } });

					
					EnemyToolbox.AddEventTriggersToAnimation(companion.spriteAnimator, "dodge_top_right", new Dictionary<int, string>() { { 8, "StunTheIdiot" } });
					EnemyToolbox.AddEventTriggersToAnimation(companion.spriteAnimator, "dodge_top_left", new Dictionary<int, string>() { { 8, "StunTheIdiot" } });
					EnemyToolbox.AddEventTriggersToAnimation(companion.spriteAnimator, "dodge_bottom_right", new Dictionary<int, string>() { { 8, "StunTheIdiot" } });
					EnemyToolbox.AddEventTriggersToAnimation(companion.spriteAnimator, "dodge_bottom_left", new Dictionary<int, string>() { { 8, "StunTheIdiot" } });
					



					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					76,
					77,
					78,
					79,
					80,
					81,
					82,
					83,
					}, "death", tk2dSpriteAnimationClip.WrapMode.LoopSection).fps = 10f;
					companion.spriteAnimator.GetClipByName("death").loopStart = 7;
					EnemyToolbox.AddEventTriggersToAnimation(companion.spriteAnimator, "death", new Dictionary<int, string>() { { 6, "StartExecute" } });

					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
					84,
					85,
					86,
					87,
					88,
					88
					}, "thanosSnap", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					EnemyToolbox.AddEventTriggersToAnimation(companion.spriteAnimator, "thanosSnap", new Dictionary<int, string>() { { 5, "DestroySelf" } });

					
				
					

					SpriteBuilder.AddAnimation(companion.spriteAnimator, nemesisCollection, new List<int>
					{
						89,
					90,
					90,
					91,
					91,
					92,
					93,
					92,
					93,
					92,
					93,
					92,
					93,
					92,
					93,
					92,
					93,
					94,//Leaps //17
					95,
					96,
					97,
					98,
					99,
					100,
					101,
					102,
					103,
					104,
					105,//Lands //27
					106,
					4,
					5,
					6,
					7,
					4,
					5,
					6,
					7,
					4,//37
					5,
					6,
					7,
					4,
					5,
					6,
					7
					}, "intro", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

				}

				//EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "awaken", new Dictionary<int, string>() { { 1, "Play_EnergySwirl" } });

				EnemyToolbox.AddEventTriggersToAnimation(companion.spriteAnimator, "intro", new Dictionary<int, string>() { { 37, "EquipSelf" } });
				EnemyToolbox.AddSoundsToAnimationFrame(companion.spriteAnimator, "intro", new Dictionary<int, string>() { { 1, "Play_EnergySwirl" }, { 17, "Play_CHR_forever_fall_01" }, { 27, "Play_BOSS_dragun_stomp_01" } });

				//NemesisEngageDoer trespassEngager = companion.aiActor.gameObject.AddComponent<NemesisEngageDoer>();



				GameObject shootpoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.5f, 0.5f), "UnwillingShootpoint");

				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
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
					CustomRange = 5f,
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
						Probability = 1f,
						NickName = "RevolverOne",
						Behavior = new ShootGunBehavior() {
						GroupCooldownVariance = 0.2f,
						LineOfSight = true,
						WeaponType = WeaponType.BulletScript,
						BulletScript = new CustomBulletScriptSelector(typeof(RevolverOneScript)),
						FixTargetDuringAttack = true,
						StopDuringAttack = false,
						LeadAmount = 0.6f,
						LeadChance = 1,
						RespectReload = true,
						MagazineCapacity = 3,
						ReloadSpeed = 3f,
						EmptiesClip = true,
						SuppressReloadAnim = false,
						TimeBetweenShots = 0.3f,
						PreventTargetSwitching = true,
						OverrideBulletName = StaticUndodgeableBulletEntries.undodgeableSniper.Name,
						OverrideAnimation = null,
						OverrideDirectionalAnimation = null,
						HideGun = false,
						UseLaserSight = false,
						UseGreenLaser = false,
						PreFireLaserTime = -1,
						AimAtFacingDirectionWhenSafe = false,
						Cooldown = 2f,
						CooldownVariance = 0,
						AttackCooldown = 0,
						GlobalCooldown = 0,
						InitialCooldown = 0,
						InitialCooldownVariance = 0,
						GroupName = null,
						GroupCooldown = 0,
						MinRange = 0,
						Range = 16,
						MinWallDistance = 0,
						MaxEnemiesInRoom = 0,
						MinHealthThreshold = 0,
						MaxHealthThreshold = 1,
						HealthThresholds = new float[0],
						AccumulateHealthThresholds = true,
						targetAreaStyle = null,
						IsBlackPhantom = false,
						resetCooldownOnDamage = null,
						RequiresLineOfSight = true,
						MaxUsages = 0,
						}
					},

					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
						NickName = "RevolverTwo",
						Behavior = new ShootBehavior(){
						ShootPoint = shootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(RevolverTwoScript)),
						LeadAmount = 0f,
						AttackCooldown = 2f,
						InitialCooldown = 1f,
						RequiresLineOfSight = true,
						//StopDuring = ShootBehavior.StopType.Attack,
						Uninterruptible = true,
						HideGun =false,
						}
					},

					//===============================================
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 0f,
						NickName = "ShotgunOne",
						Behavior = new ShootGunBehavior() {
						GroupCooldownVariance = 0.2f,
						LineOfSight = true,
						WeaponType = WeaponType.BulletScript,
						BulletScript = new CustomBulletScriptSelector(typeof(ShotgunOneScript)),
						FixTargetDuringAttack = true,
						StopDuringAttack = false,
						LeadAmount = 0.6f,
						LeadChance = 1,
						RespectReload = true,
						MagazineCapacity = 3,
						ReloadSpeed = 3f,
						EmptiesClip = true,
						SuppressReloadAnim = false,
						TimeBetweenShots = 0.5f,
						PreventTargetSwitching = true,
						OverrideBulletName = StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name,
						OverrideAnimation = null,
						OverrideDirectionalAnimation = null,
						HideGun = false,
						UseLaserSight = false,
						UseGreenLaser = false,
						PreFireLaserTime = -1,
						AimAtFacingDirectionWhenSafe = false,
						Cooldown = 1f,
						CooldownVariance = 0,
						AttackCooldown = 2,
						GlobalCooldown = 0,
						InitialCooldown = 0,
						InitialCooldownVariance = 0,
						GroupName = null,
						GroupCooldown = 0,
						MinRange = 0,
						Range = 10,
						MinWallDistance = 0,
						MaxEnemiesInRoom = 0,
						MinHealthThreshold = 0,
						MaxHealthThreshold = 1,
						HealthThresholds = new float[0],
						AccumulateHealthThresholds = true,
						targetAreaStyle = null,
						IsBlackPhantom = false,
						resetCooldownOnDamage = null,
						RequiresLineOfSight = true,
						MaxUsages = 0,
						}
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 0f,
						NickName = "ShotgunTwo",
						Behavior = new ShootBehavior(){
						ShootPoint = shootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(ShotgunTwoScript)),
						LeadAmount = 0f,
						AttackCooldown = 3f,
						InitialCooldown = 1f,
						RequiresLineOfSight = true,
						//StopDuring = ShootBehavior.StopType.Attack,
						Uninterruptible = true,
						HideGun =false,
						}
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 0f,
						NickName = "RailgunOne",
						Behavior = new ShootBehavior(){
						ShootPoint = shootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(RailgunOneScript)),
						LeadAmount = 0f,
						AttackCooldown = 3f,
						InitialCooldown = 1f,
						Cooldown = 2,
						RequiresLineOfSight = true,
						//StopDuring = ShootBehavior.StopType.Attack,
						Uninterruptible = true,
						HideGun =false,
						}
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 0f,
						NickName = "RailgunTwo",
						Behavior = new ShootBehavior(){
						ShootPoint = shootpoint,
						BulletScript = new CustomBulletScriptSelector(typeof(RailgunTwoScript)),
						LeadAmount = 0f,
						AttackCooldown = 3f,
						InitialCooldown = 1f,
						Cooldown = 3,
						RequiresLineOfSight = true,
						//StopDuring = ShootBehavior.StopType.Attack,
						Uninterruptible = true,
						HideGun =false,
						}
					},
				};
				bs.OtherBehaviors = new List<BehaviorBase>()
				{
					new UseFakeActiveBehavior
					{
						timeToHitThreshold = 1f,
						ChanceTouse = 1,
						Enabled = true,
					}
				};

				bs.OverrideBehaviors = new List<OverrideBehaviorBase>()
				{
					new ModifySpeedBehavior()
					{
					minSpeed = 1.2f,
					minSpeedDistance = 4,
					maxSpeed = 3.2f,
					maxSpeedDistance = 9
					},

					new CustomDodgeRollBehavior
					{
						Cooldown = 3,
						dodgeChance = 1f,
						timeToHitThreshold = 0.25f,
						dodgeAnim = "dodgeroll",
						rollDistance = 5,
						Enabled = true,
					},		
				};


				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:nemesis", companion.aiActor);

				AIActor aIActor = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
				PlayerHandController handObj = SpriteBuilder.SpriteFromResource("Planetside/Resources/Enemies/Nemesis/nemesis_hand_001", new GameObject("Nemesis Hand")).AddComponent<PlayerHandController>();
				FakePrefab.MarkAsFakePrefab(handObj.gameObject);
				handObj.ForceRenderersOff = false;
				handObj.sprite.usesOverrideMaterial = true;
				handObj.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				handObj.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				handObj.sprite.renderer.material.SetFloat("_EmissivePower", 40);
				handObj.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.2f);


				UnityEngine.Object.DontDestroyOnLoad(handObj.gameObject);
				var yah = companion.transform.Find("GunAttachPoint").gameObject;
				yah.transform.position = companion.aiActor.transform.position;
				yah.transform.localPosition = new Vector2(0f, 0f);
				EnemyBuilder.DuplicateAIShooterAndAIBulletBank(companion.gameObject, aIActor.aiShooter, aIActor.GetComponent<AIBulletBank>(), NemesisGun.NemesisGunID, yah.transform, null, handObj);
				EnemyToolbox.DestroyUnnecessaryHandObjects(companion.transform);



				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Nemesis/nemesis_awaken_018.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:nemesis";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Nemesis/nemesis_awaken_018";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\nemesissheetTrespass.png");
				PlanetsideModule.Strings.Enemies.Set("#NEMESIS", "Nemesis");
				PlanetsideModule.Strings.Enemies.Set("#NEMESIS_SHORTDESC", "Versus");
				PlanetsideModule.Strings.Enemies.Set("#NEMESIS_LONGDESC", "Even those who could harness the 3rd dimensions power innately were doomed to succumb to its influence.\n\nMaking the perfect duelist.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#NEMESIS";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#NEMESIS_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#NEMESIS_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:nemesis");
				EnemyDatabase.GetEntry("psog:nemesis").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:nemesis").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:nemesis").isNormalEnemy = true;


				//companion.healthHaver.spawnBulletScript = true;
				//companion.healthHaver.chanceToSpawnBulletScript = 1f;
				//companion.healthHaver.bulletScriptType = HealthHaver.BulletScriptType.OnPreDeath;
				//companion.healthHaver.bulletScript = new CustomBulletScriptSelector(typeof(EatPants));

				companion.aiActor.sprite.usesOverrideMaterial = true;
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
				mat.SetFloat("_EmissiveColorPower", 3f);
				mat.SetFloat("_EmissivePower", 35);
				companion.aiActor.sprite.renderer.material = mat;

				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBouncyBatBullet);
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
				companion.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableMine);


				companion.aiActor.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().baseData.speed *= 2f;


				GenericIntroDoer miniBossIntroDoer = prefab.AddComponent<GenericIntroDoer>();
				prefab.AddComponent<NemesisIntroController>();
				miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
				miniBossIntroDoer.initialDelay = 0.1f;
				miniBossIntroDoer.cameraMoveSpeed = 25;
				miniBossIntroDoer.specifyIntroAiAnimator = null;
				miniBossIntroDoer.BossMusicEvent = "Play_MUS_Lich_Double_01";
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
				miniBossIntroDoer.HideGunAndHand = true;
				PlanetsideModule.Strings.Enemies.Set("#QUOTE", "");

				miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
				{
					bossNameString = "#NEMESIS",
					bossSubtitleString = "#NEMESIS_SHORTDESC",
					bossQuoteString = "#QUOTE",
					bossSpritePxOffset = IntVector2.Zero,
					topLeftTextPxOffset = IntVector2.Zero,
					bottomRightTextPxOffset = IntVector2.Zero,
					bgColor = Color.cyan
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
				//==================


			}
		}

		public class RailgunOneScript : Script
        {
			public Vector2 CurrentBarrelPosition()
			{ return base.BulletBank.aiShooter.CurrentGun.barrelOffset.transform.PositionVector2(); }

			public void AddObjectToList(GameObject obj)
			{
				base.BulletBank.aiActor.gameObject.GetComponent<NemesisController>().activeLines.Add(obj);
			}
			protected override IEnumerator Top()
            {
				for (int i = 0; i < 2; i++)
                {
					base.StartTask(SwipeLaser(0, this, 0.75f - (0.125f*i), true, 30 + (10*i)));
					base.StartTask(SwipeLaser(45, this, 0.75f - (0.125f * i), false, 30 + (10 * i)));
					base.StartTask(SwipeLaser(90, this, 0.75f - (0.125f * i), false, 30 + (10 * i)));
					base.StartTask(SwipeLaser(-45, this, 0.75f - (0.125f * i), false, 30 + (10 * i)));
					base.StartTask(SwipeLaser(-90, this, 0.75f - (0.125f * i), false, 30 + (10 * i)));
					yield return this.Wait(75 - (12.5f*i));

				}
				yield return this.Wait(45);

				yield break;
			}

			public float ReturnSpecialAngle(float Speed)
            {
				Vector2 PredictedPosition = BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition(), this.BulletManager.PlayerVelocity(), CurrentBarrelPosition(), Speed);
				return (PredictedPosition - CurrentBarrelPosition()).ToAngle();
			}

			private IEnumerator SwipeLaser(float AddOrSubtract, RailgunOneScript parent, float Time, bool Fires = false, float Speed = 30)
			{
				Vector2 PredictedPosition = BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition(), this.BulletManager.PlayerVelocity(), CurrentBarrelPosition(), Speed);
				base.PostWwiseEvent("Play_BOSS_omegaBeam_charge_01");
				GameObject reticle = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = reticle.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, (PredictedPosition - CurrentBarrelPosition()).ToAngle());
				component2.dimensions = new Vector2(1f, 1000f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color red = new Color(0f, 1f, 1f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", red);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", red);
				AddObjectToList(component2.gameObject);
				float Ang = 0;
				float elapsed = 0;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;
					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					if (component2.gameObject != null)
					{
						Ang = ReturnSpecialAngle(Speed);
						component2.transform.position = new Vector3(CurrentBarrelPosition().x, CurrentBarrelPosition().y, 0);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (20 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (1.2f * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(Ang - AddOrSubtract, Ang, t));
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.5f;
				base.BulletBank.aiActor.aiAnimator.LockFacingDirection = true; 
				while (elapsed < Time)
				{
					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				UnityEngine.Object.Destroy(component2.gameObject);
				base.PostWwiseEvent("Play_Railgun");
				if (Fires == true)
                {
					base.Fire(Offset.OverridePosition(CurrentBarrelPosition()), new Direction(Ang, DirectionType.Absolute, -1f), new Speed(Speed, SpeedType.Absolute), new UndodgeableBullshit());
				}
				base.BulletBank.aiActor.aiAnimator.LockFacingDirection = false;
				yield break;
			}
			public class UndodgeableBullshit : Bullet
			{
				public UndodgeableBullshit() : base("sniperUndodgeable", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					for (int i = 0; i < 100; i++)
					{
						base.Fire(new Direction(0, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new RailgunOneScript.UndodgeableSpore());
						yield return this.Wait(1f);
					}
					yield break;
				}
			}
			public class UndodgeableSpore : Bullet
			{
				public UndodgeableSpore() : base("undodgeableSpore", true, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					yield return this.Wait(90f);
					base.Vanish(false);
					yield break;
				}
			}
		}

		public class RailgunTwoScript : Script
		{
			public Vector2 CurrentBarrelPosition()
			{ return base.BulletBank.aiShooter.CurrentGun.barrelOffset.transform.PositionVector2(); }

			public void AddObjectToList(GameObject obj)
			{
				base.BulletBank.aiActor.gameObject.GetComponent<NemesisController>().activeLines.Add(obj);
			}
			protected override IEnumerator Top()
			{
				base.StartTask(SwipeLaser(0, this, 1.5f, true, 0));
				base.StartTask(SwipeLaser(15, this, 1.5f, false, 0.25f));
				base.StartTask(SwipeLaser(30, this, 1.5f, false, 0.5f));
				base.StartTask(SwipeLaser(45, this, 1.5f, false, 0.75f));
				base.StartTask(SwipeLaser(60, this, 1.5f, false, 1f));
				base.StartTask(SwipeLaser(75, this, 1.5f, false, 1.25f));
				base.StartTask(SwipeLaser(90, this, 1.5f, false, 1.5f));

				base.StartTask(SwipeLaser(-15, this, 1.5f, false, 0.25f));
				base.StartTask(SwipeLaser(-30, this, 1.5f, false, 0.5f));
				base.StartTask(SwipeLaser(-45, this, 1.5f, false, 0.75f));
				base.StartTask(SwipeLaser(-60, this, 1.5f, false, 1f));
				base.StartTask(SwipeLaser(-75, this, 1.5f, false, 1.25f));
				base.StartTask(SwipeLaser(-90, this, 1.5f, false, 1.5f));

				yield return this.Wait(200);

				yield break;
			}

			public float ReturnSpecialAngle()
			{
				Vector2 PredictedPosition = BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition(), this.BulletManager.PlayerVelocity(), CurrentBarrelPosition(), Speed);
				return (PredictedPosition - CurrentBarrelPosition()).ToAngle();
			}

			private IEnumerator SwipeLaser(float AddOrSubtract, RailgunTwoScript parent, float Time, bool Fires = false, float TimeUntilLinesLineUp = 0.5f)
			{
				Vector2 PredictedPosition = BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition(), this.BulletManager.PlayerVelocity(), CurrentBarrelPosition(), Speed);
				base.PostWwiseEvent("Play_BOSS_omegaBeam_charge_01");
				GameObject reticle = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = reticle.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, (PredictedPosition - CurrentBarrelPosition()).ToAngle());
				component2.dimensions = new Vector2(1f, 1000f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color red = new Color(0f, 1f, 1f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", red);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", red);
				AddObjectToList(component2.gameObject);
				float Ang = 0;
				float elapsed = 0;
				while (elapsed < Time)
				{
					float t = Mathf.Min((float)elapsed / (float)TimeUntilLinesLineUp, 1);
					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					if (component2.gameObject != null)
					{
						Ang = ReturnSpecialAngle();
						component2.transform.position = new Vector3(CurrentBarrelPosition().x, CurrentBarrelPosition().y, 0);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (5 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (1 * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(Ang - AddOrSubtract, Ang, t));
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.125f;
				base.BulletBank.aiActor.aiAnimator.LockFacingDirection = true;
				while (elapsed < Time)
				{
					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				UnityEngine.Object.Destroy(component2.gameObject);
				base.PostWwiseEvent("Play_Railgun");
				if (Fires == true)
				{
					base.Fire(Offset.OverridePosition(CurrentBarrelPosition()), new Direction(Ang, DirectionType.Absolute, -1f), new Speed(40f, SpeedType.Absolute), new SuperShot());
				}
				base.BulletBank.aiActor.aiAnimator.LockFacingDirection = false;
				yield break;
			}
			public class SuperShot : Bullet
			{
				public SuperShot() : base("sniperUndodgeable", false, false, false){}
				protected override IEnumerator Top()
				{
					bool H = true;
					for (int i = 0; i < 100; i++)
					{
						H = !H;
						base.Fire(new Direction(-90, Brave.BulletScript.DirectionType.Relative, -1f), new Speed(0f, SpeedType.Absolute), new RailgunTwoScript.UndodgeableSpore(H));
						base.Fire(new Direction(90, Brave.BulletScript.DirectionType.Relative, -1f), new Speed(0f, SpeedType.Absolute), new RailgunTwoScript.UndodgeableSpore(H));
						yield return this.Wait(1.5f);
					}
					yield break;
				}
			}
			public class UndodgeableSpore : Bullet
			{
				public UndodgeableSpore(bool iswait) : base("undodgeableSpore", true, false, false)
				{
					IsWait = iswait;
				}
				protected override IEnumerator Top()
				{
					float h = (IsWait == false ? 0 : 90);
					yield return this.Wait(30f + h);
					base.ChangeSpeed(new Speed(10f, SpeedType.Absolute), 150);
					yield return this.Wait(600f);

					base.Vanish(false);
					yield break;
				}
				private bool IsWait;
			}
		}

		public class ShotgunOneScript : Script
		{
			protected override IEnumerator Top()
			{
				AkSoundEngine.PostEvent("Play_WPN_deck4rd_shot_01", this.BulletBank.aiActor.gameObject);

				if (UnityEngine.Random.value > 0.5f)
                {
					for (int i = 0; i <= 4; i++)
					{
						this.Fire(new Direction((float)(i * 20) -25f, DirectionType.Aim, -1f), new Speed(3f, SpeedType.Absolute), new ShotgunBulletOne());
					}
				}
				else
                {
					for (int i = -2; i <= 3; i++)
					{
						this.Fire(new Direction((float)(i * 20), DirectionType.Aim, -1f), new Speed(3f, SpeedType.Absolute), new ShotgunBulletOne());
					}
				}
				yield break;
			}
			public class ShotgunBulletOne : Bullet
			{
				public ShotgunBulletOne() : base(StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name, false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(11f, SpeedType.Absolute), 75);
					yield break;
				}
			}
		}

		public class ShotgunTwoScript : Script
		{
			public Vector2 CurrentBarrelPosition()
			{ return base.BulletBank.aiShooter.CurrentGun.barrelOffset.transform.PositionVector2(); }
			protected override IEnumerator Top()
			{
				for (int e = 0; e <= 3; e++)
				{
					Vector2 PredictedPosition = BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition(), this.BulletManager.PlayerVelocity(), CurrentBarrelPosition(), Speed);
					AkSoundEngine.PostEvent("Play_WPN_deck4rd_shot_01", this.BulletBank.aiActor.gameObject);
					for (int i = 0; i <= 8; i++)
					{
						this.Fire(Offset.OverridePosition(CurrentBarrelPosition()), new Direction((PredictedPosition - CurrentBarrelPosition()).ToAngle() + UnityEngine.Random.Range(-25, 25), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(3, 10), SpeedType.Absolute), new ShotgunBulletOne());
					}
					yield return this.Wait(45);
				}

				yield break;
			}
			public class ShotgunBulletOne : Bullet
			{
				public ShotgunBulletOne() : base(UnityEngine.Random.value > 0.33f? StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name : StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name, false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					yield return this.Wait(30);
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 120);
					yield return this.Wait(UnityEngine.Random.Range(120, 300));
					base.Vanish(false);
					yield break;
				}
			}
		}



		public class RevolverOneScript : Script
		{
			protected override IEnumerator Top()
			{
				AkSoundEngine.PostEvent("Play_WPN_magnum_shot_01", this.BulletBank.aiActor.gameObject);
				for (int i = -1; i <= 1; i++)
				{
					this.Fire(new Direction((float)(i * 7), DirectionType.Aim, -1f), new Speed(5f, SpeedType.Absolute), new RevolverBulletOne());
				}
				yield break;
			}
			public class RevolverBulletOne : Bullet
			{
				public RevolverBulletOne() : base(StaticUndodgeableBulletEntries.undodgeableSniper.Name, false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(14f, SpeedType.Absolute), 75);
					yield break;
				}
			}
		}

		public class RevolverTwoScript : Script
		{	
			public Vector2 CurrentBarrelPosition()
            {return base.BulletBank.aiShooter.CurrentGun.barrelOffset.transform.PositionVector2();}


			public void AddObjectToList(GameObject obj)
            {
				base.BulletBank.aiActor.gameObject.GetComponent<NemesisController>().activeLines.Add(obj);
            }

			protected override IEnumerator Top()
			{
				base.BulletBank.aiActor.aiAnimator.LockFacingDirection = true;
				float r = UnityEngine.Random.Range(3, 5);
				for (int e = 0; e < r; e++)
				{
					base.StartTask(DoYeehaw(35 - (e*7.5f), this));
					yield return this.Wait(20);
				}
				base.BulletBank.aiActor.aiAnimator.LockFacingDirection = false;

				yield return this.Wait(45);
				yield break;
			}
			public IEnumerator DoYeehaw(float Speed, RevolverTwoScript parent)
            {
				Vector2 PredictedPosition = BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition(), this.BulletManager.PlayerVelocity(), CurrentBarrelPosition(), Speed);
				GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
				tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();

				component2.transform.position = new Vector3(CurrentBarrelPosition().x, CurrentBarrelPosition().y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, (PredictedPosition - CurrentBarrelPosition()).ToAngle());
				component2.dimensions = new Vector2(1000f, 1f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laser = new Color(0f, 1f, 1f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
				AddObjectToList(component2.gameObject);
				float elapsed = 0;
				float Time = 0.25f;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;

					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					if (component2 != null)
					{

						component2.transform.position = new Vector3(CurrentBarrelPosition().x, CurrentBarrelPosition().y, 0);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (25 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
						component2.transform.localRotation = Quaternion.Euler(0f, 0f, (PredictedPosition - CurrentBarrelPosition()).ToAngle());
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.dimensions = new Vector2(1000f, 1f);
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.25f;
				base.PostWwiseEvent("Play_FlashTell");
				while (elapsed < Time)
				{
					if (parent.IsEnded || parent.Destroyed)
					{
						UnityEngine.Object.Destroy(component2.gameObject);
						yield break;
					}
					float t = (float)elapsed / (float)Time;
					if (component2 != null)
					{
						component2.transform.position = new Vector3(CurrentBarrelPosition().x, CurrentBarrelPosition().y, 0);
						component2.dimensions = new Vector2(1000f, 1f);
						component2.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (60 * t));
						component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (20 * t));
						component2.HeightOffGround = -2;
						component2.renderer.gameObject.layer = 23;
						component2.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				UnityEngine.Object.Destroy(component2.gameObject);
				AkSoundEngine.PostEvent("Play_WPN_sniperrifle_shot_01", this.BulletBank.aiActor.gameObject);
				base.Fire(Offset.OverridePosition(CurrentBarrelPosition()), new Direction((PredictedPosition - CurrentBarrelPosition()).ToAngle(), DirectionType.Absolute, -1f), new Speed(40f, SpeedType.Absolute), new WallBulletNoDodge("sniperUndodgeable"));

				yield break;
            }
			public class WallBulletNoDodge : Bullet
			{public WallBulletNoDodge(string BulletType) : base(BulletType, true, false, false){}}

		}




		private static string[] spritePaths = new string[]
		{
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_bottom_left_001.png",//0
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_bottom_left_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_bottom_left_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_bottom_left_004.png",//3

			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_bottom_right_001.png",//4
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_bottom_right_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_bottom_right_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_bottom_right_004.png",//7

			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_top_left_001.png",//8
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_top_left_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_top_left_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_top_left_004.png",//11

			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_top_right_001.png",//12
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_top_right_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_top_right_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_idle_top_right_004.png",//15

			"Planetside/Resources/Enemies/Nemesis/nemesis_run_bottom_left_001.png",//16
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_bottom_left_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_bottom_left_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_bottom_left_004.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_bottom_left_005.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_bottom_left_006.png",//21

			"Planetside/Resources/Enemies/Nemesis/nemesis_run_bottom_right_001.png",//22
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_bottom_right_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_bottom_right_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_bottom_right_004.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_bottom_right_005.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_bottom_right_006.png",//27

			"Planetside/Resources/Enemies/Nemesis/nemesis_run_top_left_001.png",//28
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_top_left_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_top_left_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_top_left_004.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_top_left_005.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_top_left_006.png",//33

			"Planetside/Resources/Enemies/Nemesis/nemesis_run_top_right_001.png",//34
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_top_right_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_top_right_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_top_right_004.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_top_right_005.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_run_top_right_006.png",//39

			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_left_001.png",//40
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_left_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_left_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_left_004.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_left_005.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_left_006.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_left_007.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_left_008.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_left_009.png",//48

			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_right_001.png",//49
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_right_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_right_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_right_004.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_right_005.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_right_006.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_right_007.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_right_008.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_bottom_right_009.png",//57

			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_left_001.png",//58
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_left_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_left_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_left_004.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_left_005.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_left_006.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_left_007.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_left_008.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_left_009.png",//66

			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_right_001.png",//67
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_right_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_right_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_right_004.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_right_005.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_right_006.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_right_007.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_right_008.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_dodge_top_right_009.png",//75

			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_001.png",//76
			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_004.png",
			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_005.png",
			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_006.png",
			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_007.png",
			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_008.png",
			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_009.png",
			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_010.png",
			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_011.png",
			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_012.png",
			"Planetside/Resources/Enemies/Nemesis/nemesisd_death_013.png",//88

			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_001.png",//89
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_002.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_003.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_004.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_005.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_006.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_007.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_008.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_009.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_010.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_011.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_012.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_013.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_014.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_015.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_016.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_017.png",
			"Planetside/Resources/Enemies/Nemesis/nemesis_awaken_018.png",//106

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
			{
				//StartExecute	
				if (clip.GetFrame(frameIdx).eventInfo.Contains("StunTheIdiot"))
                {
					this.aiActor.BehaviorOverridesVelocity = false;
					this.aiActor.aiAnimator.LockFacingDirection = false;
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("StartExecute"))
				{
					GameManager.Instance.StartCoroutine(this.CauseDeath(base.aiActor));
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("DestroySelf"))
                {
					Destroy(base.aiActor.gameObject);
				}//ForceIntro
				if (clip.GetFrame(frameIdx).eventInfo.Contains("ForceIntro"))
                {
					base.aiActor.gameObject.GetComponent<GenericIntroDoer>().TriggerSequence(GameManager.Instance.BestActivePlayer);
                }
			}
			private IEnumerator CauseDeath(AIActor aiActor)
			{
				Pixelator.Instance.DoFinalNonFadedLayer = true;
				float elapsed = 0f;
				float duration = 0.8f;			
				GameObject clockhairObject = UnityEngine.Object.Instantiate<GameObject>(BraveResources.Load<GameObject>("Clockhair", ".prefab"));
				ClockhairController clockhair = clockhairObject.GetComponent<ClockhairController>();
				duration = clockhair.ClockhairInDuration;
				Vector3 clockhairTargetPosition = aiActor.sprite.WorldCenter;
				Vector3 clockhairStartPosition = clockhairTargetPosition + MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), 40).ToVector3ZisY();
				clockhair.renderer.enabled = false;
				clockhair.spriteAnimator.Play("clockhair_intro");
				clockhair.hourAnimator.Play("hour_hand_intro");
				clockhair.minuteAnimator.Play("minute_hand_intro");
				clockhair.secondAnimator.Play("second_hand_intro");
				bool hasWobbled = false;
				while (elapsed < duration)
				{
					bool flag = GameManager.INVARIANT_DELTA_TIME == 0f;
					if (flag)
					{
						elapsed += 0.05f;
					}
					elapsed += GameManager.INVARIANT_DELTA_TIME;
					float t2 = elapsed / duration;
					float smoothT = Mathf.SmoothStep(0f, 1f, t2);
					Vector3 currentPosition = Vector3.Slerp(clockhairStartPosition, clockhairTargetPosition, smoothT);
					clockhairObject.transform.position = currentPosition.WithZ(0f);
					bool flag2 = t2 > 0.5f;
					if (flag2)
					{
						clockhair.renderer.enabled = true;
						clockhair.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
					}
					bool flag3 = t2 > 0.75f;
					if (flag3)
					{
						clockhair.hourAnimator.GetComponent<Renderer>().enabled = true;
						clockhair.minuteAnimator.GetComponent<Renderer>().enabled = true;
						clockhair.secondAnimator.GetComponent<Renderer>().enabled = true;
						clockhair.hourAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
						clockhair.minuteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
						clockhair.secondAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
					}
					bool flag4 = !hasWobbled && clockhair.spriteAnimator.CurrentFrame == clockhair.spriteAnimator.CurrentClip.frames.Length - 1;
					if (flag4)
					{
						clockhair.spriteAnimator.Play("clockhair_wobble");
						hasWobbled = true;
					}
					clockhair.sprite.UpdateZDepth();
					yield return null;
					currentPosition = default(Vector3);
				}
				bool flag5 = !hasWobbled;
				if (flag5)
				{
					clockhair.spriteAnimator.Play("clockhair_wobble");
				}
				clockhair.SpinToSessionStart(clockhair.ClockhairSpinDuration);
				elapsed = 0f;
				duration = clockhair.ClockhairSpinDuration + clockhair.ClockhairPauseBeforeShot;
				while (elapsed < duration)
				{
					bool flag6 = GameManager.INVARIANT_DELTA_TIME == 0f;
					if (flag6)
					{
						elapsed += 0.05f;
					}
					elapsed += GameManager.INVARIANT_DELTA_TIME;
					clockhair.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
					yield return null;
				}
				elapsed = 0f;
				duration = 0.1f;
				clockhairStartPosition = clockhairObject.transform.position;
				clockhairTargetPosition = clockhairStartPosition + new Vector3(0f, 12f, 0f);
				clockhair.spriteAnimator.Play("clockhair_fire");
				clockhair.hourAnimator.GetComponent<Renderer>().enabled = false;
				clockhair.minuteAnimator.GetComponent<Renderer>().enabled = false;
				clockhair.secondAnimator.GetComponent<Renderer>().enabled = false;
				while (elapsed < duration)
				{
					bool flag7 = GameManager.INVARIANT_DELTA_TIME == 0f;
					if (flag7)
					{
						elapsed += 0.05f;
					}
					elapsed += GameManager.INVARIANT_DELTA_TIME;
					clockhair.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
					yield return null;
				}

				

				List<AIActor> activeEnemies = aiActor.GetAbsoluteParentRoom().GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				Vector2 centerPosition = aiActor.sprite.WorldCenter;
				if (activeEnemies != null && activeEnemies.Count > 0)
				{
					foreach (AIActor aiactor in activeEnemies)
					{
						bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < 0.75f && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null;
						if (ae)
						{
							aiactor.healthHaver.ApplyDamage(1000000, Vector2.zero, "Instant Kill", CoreDamageTypes.Void, DamageCategory.Normal, false, null, false);
						}
					}
				}

				foreach (PlayerController player in GameManager.Instance.AllPlayers)
				{
					if (Vector2.Distance(player.sprite.WorldCenter, aiActor.sprite.WorldCenter) < 0.75f)
					{
						//CLOCKHAIR_SPECIAL_DEATH

						WeightedIntCollection attackWeights = new WeightedIntCollection();
						attackWeights.elements = new WeightedInt[]
						{
							new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "1", value = 1, weight = 1},
							new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "2", value = 2, weight = 1f},
							new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "3", value = 3, weight = 1f},
							new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "4", value = 4, weight = 1f},
						};

						player.healthHaver.ApplyDamage(1000000, Vector2.zero, StringTableManager.GetLongString("#CLOCKHAIR_SPECIAL_DEATH_" + attackWeights.SelectByWeight().ToString()), CoreDamageTypes.Void, DamageCategory.Normal, false, null, false);
						player.healthHaver.Armor = 0;
						player.healthHaver.ForceSetCurrentHealth(0);
						player.healthHaver.Die(Vector2.zero);
					}
				}
				aiActor.aiAnimator.PlayUntilFinished("thanosSnap", false, null, -1f, false);

				elapsed = 0f;
				duration = 1f;
				while (elapsed < duration)
				{
					bool flag8 = GameManager.INVARIANT_DELTA_TIME == 0f;
					if (flag8)
					{
						elapsed += 0.05f;
					}
					elapsed += GameManager.INVARIANT_DELTA_TIME;
					bool flag9 = clockhair.spriteAnimator.CurrentFrame == clockhair.spriteAnimator.CurrentClip.frames.Length - 1;
					if (flag9)
					{
						clockhair.renderer.enabled = false;
					}
					else
					{
						clockhair.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
					}
					yield return null;
				}
				yield return new WaitForSeconds(0.5f);
				UnityEngine.Object.Destroy(clockhairObject);
				Pixelator.Instance.DoFinalNonFadedLayer = false;

				yield break;
			}

		}
	}
}





