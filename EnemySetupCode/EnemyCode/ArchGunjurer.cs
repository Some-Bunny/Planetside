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

namespace Planetside
{
	public class ArchGunjurer : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "arch_gunjurer";
		public static GameObject shootpoint;
		public static void Init()
		{
			ArchGunjurer.BuildPrefab();
		}

		public static void BuildPrefab()
		{
            tk2dSpriteCollectionData Collection = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("ArchGunjurerCollection").GetComponent<tk2dSpriteCollectionData>();
            Material mat = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Material>("archgunjurer material");

            bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefabBundle("Arch Gunjurer", guid, Collection, 0, new IntVector2(0, 0), new IntVector2(8, 9),false);
				var companion = prefab.AddComponent<EnemyBehavior>();

                EnemyToolbox.QuickAssetBundleSpriteSetup(companion.aiActor, Collection, mat);

                companion.aiActor.knockbackDoer.weight = 200;
				companion.aiActor.MovementSpeed = 1.25f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.IgnoreForRoomClear = false;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = false;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(55f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.healthHaver.SetHealthMaximum(55f, null, false);
				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.largeShadow, new Vector2(0.875f, 0.25f), "shadowPos");
				companion.aiActor.SetIsFlying(true, "Gamemode: Creative", true, true);
				companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;


				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 7,
					ManualOffsetY = 4,
					ManualWidth = 18,
					ManualHeight = 44,
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
					ManualOffsetX = 7,
					ManualOffsetY = 4,
					ManualWidth = 18,
					ManualHeight = 44,
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
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
					name = "attack",
					anim = new DirectionalAnimation
						{
							Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
							Flipped = new DirectionalAnimation.FlipType[2],
							AnimNames = new string[]
							{
					   "attack_right",
					   "attack_left",

							}

						}
					}
				};
				DirectionalAnimation done = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "warpout",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "warpout",
						anim = done
					}
				};
				DirectionalAnimation aa = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.Single,
					Prefix = "waprin",
					AnimNames = new string[1],
					Flipped = new DirectionalAnimation.FlipType[1]
				};
				aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>
				{
					new AIAnimator.NamedDirectionalAnimation
					{
						name = "waprin",
						anim = aa
					}
				};

				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);
				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;

				//bool flag3 = ArchGunjurerCholection == null;
				//if (flag3)
				{
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4


					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

					0,
					1,
					2,
					3,
					4


					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{


					0,
					1,
					2,
					3,
					4


					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

				 5,
				 6,
				 7,
				 8,
				 9,
				 10,
				 11




					}, "attack_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 4f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{


				 5,
				 6,
				 7,
				 8,
				 9,
				 10,
				 11

					}, "attack_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 4f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

				 12,
				 13,
				 14,
				 15,
				 16,
				 17,
				 18,
				 19,
				 20
				 




					}, "waprin", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

				 21,
				22,
				 23,
				 24,
				 25,
				 26,
				 27

					}, "warpout", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

				 28,
				 29,
				 30,
				 31,
				 32,
				 33,
				 34




					}, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{

				 28,
				 29,
				 30,
				 31,
				 32,
				 33,
				 34

					}, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, Collection, new List<int>
					{
				 12,
				 13,
				 14,
				 15,
				 16,
				 17,
				 18,
				 19,
				 20

					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;
				}
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
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.25f,
				}
			};
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBehavior() {
					ShootPoint = m_CachedGunAttachPoint,
					BulletScript = new CustomBulletScriptSelector(typeof(SalamanderScript)),
					LeadAmount = 0f,
					AttackCooldown = 0.5f,
					Cooldown = 2f,
					TellAnimation = "attack",
					FireAnimation = "attack",
					RequiresLineOfSight = true,
					MultipleFireEvents = true,
					Uninterruptible = false,


				},
				new TeleportBehavior()
				{

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
					MinDistanceFromPlayer = 4f,
					MaxDistanceFromPlayer = -1f,
					teleportInAnim = "waprin",
					teleportOutAnim = "warpout",
					AttackCooldown = 1f,
					InitialCooldown = 0f,
					RequiresLineOfSight = false,
					roomMax = new Vector2(0,0),
					roomMin = new Vector2(0,0),
					teleportInBulletScript = new CustomBulletScriptSelector(typeof(Wail)),
					teleportOutBulletScript = new CustomBulletScriptSelector(typeof(Wail)),
					GlobalCooldown = 0.5f,
					Cooldown = 2f,
					
					CooldownVariance = 1f,
					InitialCooldownVariance = 0f,
					goneAttackBehavior = null,
					IsBlackPhantom = false,
					GroupName = null,
					GroupCooldown = 0f,
					MinRange = 0,
					Range = 0,
					MinHealthThreshold = 0,
					MaxUsages = 0,
					AccumulateHealthThresholds = true,
					targetAreaStyle = null,
					HealthThresholds = new float[0],
					MinWallDistance = 0,
				}
				};
				bs.MovementBehaviors = new List<MovementBehaviorBase>
			{
				new MoveErraticallyBehavior
				{
				   PointReachedPauseTime = 0.1f,
					PathInterval = 0.2f,
					PreventFiringWhileMoving = false,
					StayOnScreen = false,
					AvoidTarget = true,

				}
			};
				/*
				bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootBeamBehavior() {
					//ShootPoint = m_CachedGunAttachPoint,

					firingTime = 3f,
					stopWhileFiring = true,
					//beam
					//BulletScript = new CustomBulletScriptSelector(typeof(SkellScript)),
					//LeadAmount = 0f,
					AttackCooldown = 5f,
					//InitialCooldown = 4f,
					RequiresLineOfSight = true,
					//StopDuring = ShootBehavior.StopType.Attack,
					//Uninterruptible = true,

				}
				};
				*/
				
				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:arch_gunjurer", companion.aiActor);

                Material mat2 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat2.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
                mat2.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                companion.aiActor.sprite.renderer.material = mat2;

                SpriteBuilder.AddSpriteToCollection("Planetside/Resources/ArchJurer/archjurer_idle_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:arch_gunjurer";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/ArchJurer/archjurer_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("archgunjurericon");// ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\archgunjurericon.png");
                PlanetsideModule.Strings.Enemies.Set("#THE_JURER", "Arch Gunjurer");
				PlanetsideModule.Strings.Enemies.Set("#THE_JURER_SHORTDESC", "Master Of Al-gun-y");
				PlanetsideModule.Strings.Enemies.Set("#THE_JURER_LONGDESC", "The highest rank of Gunjurer, these wizards have learned to create gunfire while beyond the Curtain.\n\nOld gungeoneer folk tales say that if one were to get a wand it would become truly unstoppable.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#THE_JURER";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#THE_JURER_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#THE_JURER_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:arch_gunjurer");
				EnemyDatabase.GetEntry("psog:arch_gunjurer").ForcedPositionInAmmonomicon = 58;
				EnemyDatabase.GetEntry("psog:arch_gunjurer").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:arch_gunjurer").isNormalEnemy = true;

                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));
            }
        }

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
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>
				{
				  AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Fade_01", base.aiActor.gameObject);
				};
			}

		}

		public class Wail : Script 
		{
			public override IEnumerator Top()
			{
				AkSoundEngine.PostEvent("Play_BOSS_Rat_Kunai_Prep_01", this.BulletBank.aiActor.gameObject);
				for (int k = 0; k < 12; k++)
				{
					this.Fire(new Direction(30*k, DirectionType.Aim, -1f), new Speed(5f, SpeedType.Absolute), new ReverseBullet());
				}
				yield break;
			}
		}
		public class ReverseBullet : Bullet
		{
			public ReverseBullet() : base("reversible", false, false, false)
			{
				base.SuppressVfx = true;
			}

			public override IEnumerator Top()
			{
				float speed = this.Speed;
				yield return this.Wait(30);
				this.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
				yield return this.Wait(30);
				this.Direction += 180f;
				this.Projectile.spriteAnimator.Play();
				yield return this.Wait(45);
				this.ChangeSpeed(new Speed(speed * 3f, SpeedType.Absolute), 60);
				yield break;
			}
		}


		public class SalamanderScript : Script
		{
			public override IEnumerator Top() 
			{
				base.PostWwiseEvent("Play_BOSS_RatMech_Wizard_Cast_01", null);
				yield return this.Wait(60);
				for (int i = 0; i < 3; i++)
				{
					base.PostWwiseEvent("Play_ENM_wizardred_appear_01", null);
					base.Fire(new Direction(120*i, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), new WallBullet());
					yield return this.Wait(10);
				}
				yield break;
			}
		}


		public class WallBullet : Bullet
		{
			public WallBullet() : base("bigBullet", false, false, false)
			{
			}
			public override IEnumerator Top()
			{
				base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
				yield return this.Wait(90);
				this.Direction = base.AimDirection;
				base.ChangeSpeed(new Speed(15f, SpeedType.Absolute), 60);
				yield break;
			}
			public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				if (!preventSpawningProjectiles)
				{
                    AkSoundEngine.PostEvent("Play_ENM_kali_burst_01", this.Projectile.gameObject);
					float num = base.RandomAngle();
					for (int i = 0; i < 8; i++)
					{
						base.Fire(new Direction((45 * i)+ num, DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new Bullet("reversible"));
					}
				}
			}
		}

	}
}





