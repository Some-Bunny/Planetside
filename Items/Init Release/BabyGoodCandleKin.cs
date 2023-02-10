using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.CompanionBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using GungeonAPI;
using System.Reflection;

namespace Planetside
{
	public class BabyGoodCandleKin : CompanionItem
	{
		public static GameObject prefab;
		public static readonly string guid = "baby good candle";
		private static tk2dSpriteCollectionData CandleKinCollection;


		public static void Init()
		{
			string name = "Baby Good Candle";
			string resourcePath = "Planetside/Resources/Companions/BabyCandle/candleman_idle_right_001";
			GameObject gameObject = new GameObject();
			BabyGoodCandleKin item = gameObject.AddComponent<BabyGoodCandleKin>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject, true);
			string shortDesc = "All Will Be Warm...";
			string longDesc = "A little living candle.\n\nIt's warmth is given to it by their old friend.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.B;
			BabyGoodCandleKin.BuildPrefab();

			item.CompanionGuid = BabyGoodCandleKin.guid;//new List<string>() { BabyGoodCandleKin.guid, BabyGoodCandleKin.guid };
			//item.CompanionComponentsToTransfer = new List<Component> { new PetInteractable(), new PetInteractable() };

			BabyGoodCandleKin.BabyGoodCandleKinID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);
			SynergyAPI.SynergyBuilder.AddItemToSynergy(item, CustomSynergyType.TEA_FOR_TWO);

		}
		public static int BabyGoodCandleKinID;

		public static void BuildPrefab()
		{
			if (prefab == null || !CompanionBuilder.companionDictionary.ContainsKey(guid))
			{
				prefab = CompanionBuilder.BuildPrefab("Baby Candle Kin", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9));
				var companion = prefab.AddComponent<BabyCandleBehavior>();
				companion.aiActor.MovementSpeed = 2f;
				companion.aiActor.healthHaver.PreventAllDamage = true;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.HasShadow = false;
				companion.aiActor.PreventFallingInPitsEver = false;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(30f);
				companion.aiActor.CollisionKnockbackStrength = 5f;
				companion.aiActor.CanTargetPlayers = false;
				companion.aiActor.CanTargetEnemies = true;
				companion.aiActor.IgnoreForRoomClear = true;
				AIAnimator aiAnimator = companion.aiAnimator;
				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
					{
						"idle_right",
						"idle_left"
					}
				};
				aiAnimator.MoveAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
						{
						"run_right",
						"run_left"
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
						   "attack_left"

							}

						}
					}
				};
				if (CandleKinCollection == null)
				{
					CandleKinCollection = SpriteBuilder.ConstructCollection(prefab, "BabyCandle_Collection");
					UnityEngine.Object.DontDestroyOnLoad(CandleKinCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], CandleKinCollection);
					}
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CandleKinCollection, new List<int>
					{
						0,
						1,
						2,
						3,
						4,
						5,
						6,
						7


					}, "attack_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CandleKinCollection, new List<int>
					{
						8,
						9,
						10,
						11,
						12,
						13,
						14,
						15

					}, "attack_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 10f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, CandleKinCollection, new List<int>
					{
					16,
					17,
					18,
					19

					}, "idle_left", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CandleKinCollection, new List<int>
					{
						20,
						21,
						22,
						23


					}, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CandleKinCollection, new List<int>
					{
						24,
						25,
						26,
						27,
						28,
						29,
						30


					}, "run_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 14f;
					SpriteBuilder.AddAnimation(companion.spriteAnimator, CandleKinCollection, new List<int>
					{
						31,
						32,
						33,
						34,
						35,
						36,
						37
						



					}, "run_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 14f;

				}

                AIBulletBank.Entry sample = new AIBulletBank.Entry();

                Projectile proj = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(146) as Gun).DefaultModule.projectiles[0].gameObject).GetComponent<Projectile>();
				proj.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(proj.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(proj);
				proj.baseData.speed *= 0.2f;
				proj.baseData.damage = 40;

                sample.BulletObject = proj.gameObject;
				sample.Name = "Shoot";



				AIBulletBank aIBulletBank = companion.gameObject.AddComponent<AIBulletBank>();
				aIBulletBank.Bullets = new List<AIBulletBank.Entry>() { sample };

                var bs = prefab.GetComponent<BehaviorSpeculator>();
				bs.AttackBehaviors.Add(new ShootBehavior() 
				{
					LeadAmount = 0.7f,
					AttackCooldown = 3.5f,
					FireAnimation = "attack",
					BulletName = "Shoot",
					ShootPoint = EnemyToolbox.GenerateShootPoint(companion.gameObject, new Vector2(0.5f, 0.5f), "candle_point"),
					StopDuring = ShootBehavior.StopType.Attack,
					MinRange = 9
                });
				bs.MovementBehaviors.Add(new BabyGoodCandleKin.ApproachEnemiesBehavior());
				bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior() { IdleAnimations = new string[] { "idle" } });

			}
		}





		private static string[] spritePaths = new string[]
		{
			
			//attacks
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_left_001",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_left_002",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_left_003",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_left_004",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_left_005",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_left_006",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_left_007",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_left_008",


			"Planetside/Resources/Companions/BabyCandle/candleman_fire_right_001",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_right_002",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_right_003",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_right_004",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_right_005",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_right_006",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_right_007",
			"Planetside/Resources/Companions/BabyCandle/candleman_fire_right_008",




			//idles
		"Planetside/Resources/Companions/BabyCandle/candleman_idle_left_001",
		"Planetside/Resources/Companions/BabyCandle/candleman_idle_left_002",
		"Planetside/Resources/Companions/BabyCandle/candleman_idle_left_003",
		"Planetside/Resources/Companions/BabyCandle/candleman_idle_left_004",


		"Planetside/Resources/Companions/BabyCandle/candleman_idle_right_001",
		"Planetside/Resources/Companions/BabyCandle/candleman_idle_right_002",
		"Planetside/Resources/Companions/BabyCandle/candleman_idle_right_003",
		"Planetside/Resources/Companions/BabyCandle/candleman_idle_right_004",

						//run
		"Planetside/Resources/Companions/BabyCandle/candleman_run_left_001",
		"Planetside/Resources/Companions/BabyCandle/candleman_run_left_002",
		"Planetside/Resources/Companions/BabyCandle/candleman_run_left_003",
		"Planetside/Resources/Companions/BabyCandle/candleman_run_left_004",
		"Planetside/Resources/Companions/BabyCandle/candleman_run_left_005",
		"Planetside/Resources/Companions/BabyCandle/candleman_run_left_006",
		"Planetside/Resources/Companions/BabyCandle/candleman_run_left_007",


		"Planetside/Resources/Companions/BabyCandle/candleman_run_right_001",
		"Planetside/Resources/Companions/BabyCandle/candleman_run_right_002",
		"Planetside/Resources/Companions/BabyCandle/candleman_run_right_003",
		"Planetside/Resources/Companions/BabyCandle/candleman_run_right_004",
		"Planetside/Resources/Companions/BabyCandle/candleman_run_right_005",
		"Planetside/Resources/Companions/BabyCandle/candleman_run_right_006",
		"Planetside/Resources/Companions/BabyCandle/candleman_run_right_007",


		};


		public class BabyCandleBehavior : CompanionController
		{
			private void Start()
			{
				base.spriteAnimator.Play("idle");
				this.Owner = this.m_owner;
			}
			public override void Update()
            {
				this.elapsed += BraveTime.DeltaTime;
				if (this.elapsed > 0.75f)
				{
					List<AIActor> activeEnemies = Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
					Vector2 centerPosition = base.aiActor.sprite.WorldCenter;
					if (activeEnemies != null)
					{
						foreach (AIActor aiactor in activeEnemies)
						{
							BulletStatusEffectItem Firecomponent = PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>();
							GameActorFireEffect gameActorFire = Firecomponent.FireModifierEffect;
							if (Vector2.Distance(aiactor.CenterPosition, centerPosition) < 2.5f && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && Owner != null)
							{
								aiactor.ApplyEffect(gameActorFire, 1f, null);

							}
						}
					}					
				}
					
			}
			private float elapsed;

			public PlayerController Owner;
		}


		public class ApproachEnemiesBehavior : MovementBehaviorBase
		{
			public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
			{
				base.Init(gameObject, aiActor, aiShooter);
			}

			public override void Upkeep()
			{
				base.Upkeep();
				base.DecrementTimer(ref this.repathTimer, false);
			}

			public override BehaviorResult Update()
			{
				SpeculativeRigidbody overrideTarget = this.m_aiActor.OverrideTarget;
				BehaviorResult result;
				if (this.repathTimer > 0f)
				{
					result = ((overrideTarget == null) ? BehaviorResult.Continue : BehaviorResult.SkipRemainingClassBehaviors);
				}
				else
				{
					if (overrideTarget == null)
					{
						this.PickNewTarget();
						result = BehaviorResult.Continue;
					}
					else
					{
						this.isInRange = (Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, overrideTarget.UnitCenter) <= this.DesiredDistance);
						if (overrideTarget != null && !this.isInRange)
						{
							this.m_aiActor.PathfindToPosition(overrideTarget.UnitCenter, null, true, null, null, null, false);
							this.repathTimer = this.PathInterval;
							result = BehaviorResult.SkipRemainingClassBehaviors;
						}
						else
						{
							if (overrideTarget != null && this.repathTimer >= 0f)
							{
								this.m_aiActor.ClearPath();
								this.repathTimer = -1f;
							}
							result = BehaviorResult.Continue;
						}
					}
				}
				return result;
			}

			private void PickNewTarget()
			{

				bool flag = this.m_aiActor == null;
				if (!flag)
				{
					bool flag2 = this.Owner == null;
					if (flag2)
					{
						this.Owner = this.m_aiActor.GetComponent<BabyCandleBehavior>().Owner;
					}
					this.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref this.roomEnemies);
					for (int i = 0; i < this.roomEnemies.Count; i++)
					{
						AIActor aiactor = this.roomEnemies[i];
						bool flag3 = aiactor.IsHarmlessEnemy || !aiactor.IsNormalEnemy || aiactor.healthHaver.IsDead || aiactor == this.m_aiActor || aiactor.EnemyGuid == "ba928393c8ed47819c2c5f593100a5bc";
						if (flag3)
						{

							this.roomEnemies.Remove(aiactor);

						}
					}
					bool flag4 = this.roomEnemies.Count == 0;
					if (flag4)
					{
						this.m_aiActor.OverrideTarget = null;
					}
					else
					{
						AIActor aiActor = this.m_aiActor;
						AIActor aiactor2 = this.roomEnemies[UnityEngine.Random.Range(0, this.roomEnemies.Count)];
						aiActor.OverrideTarget = ((aiactor2 != null) ? aiactor2.specRigidbody : null);
					}
				}
			}


			public float PathInterval = 0.25f;
			public float DesiredDistance = 5f;
			private float repathTimer;
			private List<AIActor> roomEnemies = new List<AIActor>();
			private bool isInRange;
			private PlayerController Owner;
		}
	}
}

