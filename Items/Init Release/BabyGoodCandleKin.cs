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
			string longDesc = "A little living candle. It's warmth is given to it by an old friend of it.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.B;
			item.CompanionGuid = BabyGoodCandleKin.guid;
			BabyGoodCandleKin.BuildPrefab();
			BabyGoodCandleKin.BabyGoodCandleKinID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

		}
		public static int BabyGoodCandleKinID;

		public static void BuildPrefab()
		{
			bool flag = prefab != null || CompanionBuilder.companionDictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
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
				bool flag3 = CandleKinCollection == null;
				if (flag3)
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
				var bs = prefab.GetComponent<BehaviorSpeculator>();
				bs.AttackBehaviors.Add(new BabyCandleBehaviorAttack());
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
				bool flag3 = this.elapsed > 0.75f;
				if (flag3)
				{
					List<AIActor> activeEnemies = Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
					Vector2 centerPosition = base.aiActor.sprite.WorldCenter;
					if (activeEnemies != null)
					{
						foreach (AIActor aiactor in activeEnemies)
						{
							BulletStatusEffectItem Firecomponent = PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>();
							GameActorFireEffect gameActorFire = Firecomponent.FireModifierEffect;
							bool banko = Vector2.Distance(aiactor.CenterPosition, centerPosition) < 2.5f && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && Owner != null;
							if (banko)
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


		public class BabyCandleBehaviorAttack : AttackBehaviorBase
		{

			private PlayerController Owner;
			public override void Destroy()
			{
				base.Destroy();
			}

			public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
			{
				base.Init(gameObject, aiActor, aiShooter);
				this.Owner = this.m_aiActor.GetComponent<BabyGoodCandleKin.BabyCandleBehavior>().Owner;

			}

			public override BehaviorResult Update()
			{
				bool flag = this.attackTimer > 0f && this.isAttacking;
				if (flag)
				{
					base.DecrementTimer(ref this.attackTimer, false);
				}
				else
				{
					bool flag2 = this.attackCooldownTimer > 0f && !this.isAttacking;
					if (flag2)
					{
						base.DecrementTimer(ref this.attackCooldownTimer, false);
					}
				}
				bool flag3 = this.IsReady();
				bool flag4 = (!flag3 || this.attackCooldownTimer > 0f || this.attackTimer == 0f || this.m_aiActor.TargetRigidbody == null) && this.isAttacking;
				BehaviorResult result;
				if (flag4)
				{
					this.StopAttacking();
					result = BehaviorResult.Continue;
				}
				else
				{
					bool flag5 = flag3 && this.attackCooldownTimer == 0f && !this.isAttacking;
					if (flag5)
					{
						this.attackTimer = this.attackDuration;
						this.m_aiAnimator.PlayUntilFinished(this.attackAnimation, false, null, -1f, false);
						this.isAttacking = true;
					}
					bool flag6 = this.attackTimer > 0f && flag3;
					if (flag6)
					{
						GameManager.Instance.StartCoroutine(Attack());
						this.m_aiActor.MovementSpeed = 0f;
						result = BehaviorResult.SkipAllRemainingBehaviors;

					}
					else
					{
						result = BehaviorResult.Continue;
					}
				}
				return result;
			}

			private void StopAttacking()
			{
				this.isAttacking = false;
				this.attackTimer = 0f;
				this.attackCooldownTimer = this.attackCooldown;
			}

			public IEnumerator Attack()
			{
				yield return new WaitForSeconds(0.28f);
				bool flag = this.Owner == null;
				if (flag)
				{
					this.Owner = this.m_aiActor.GetComponent<BabyCandleBehavior>().Owner;
				}
				float num = -1f;

				List<AIActor> activeEnemies = this.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				bool flag2 = activeEnemies == null | activeEnemies.Count <= 0;
				if (!flag2)
				{
					AIActor nearestEnemy = this.Owner.CurrentRoom.GetRandomActiveEnemy(false);
					bool flag3 = nearestEnemy && num < 10f;
					if (flag3)
					{
						bool flag4 = this.IsInRange(nearestEnemy);
						if (flag4)
						{
							bool flag5 = !nearestEnemy.IsHarmlessEnemy && nearestEnemy.IsNormalEnemy && !nearestEnemy.healthHaver.IsDead && nearestEnemy != this.m_aiActor;
							if (flag5)
							{
								AkSoundEngine.PostEvent("Play_BOSS_lichB_charge_01", this.m_aiActor.gameObject);
								/*
								Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
								Vector2 unitCenter2 = nearestEnemy.specRigidbody.HitboxPixelCollider.UnitCenter;
								float z = BraveMathCollege.Atan2Degrees((unitCenter2 - unitCenter).normalized);
								Projectile projectile = ((Gun)ETGMod.Databases.Items[146]).DefaultModule.projectiles[0];
								GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, this.m_aiActor.sprite.WorldCenter, Quaternion.Euler(0f, 0f, z), true);
								Projectile component = gameObject.GetComponent<Projectile>();

								AoEDamageComponent aoe = projectile.GetComponent<AoEDamageComponent>();
								aoe.DealsDamage = false;
								aoe.InflictsFire = true;
								aoe.Radius = 3f;

								aoe.TimeBetweenDamageEvents = 0.25f;
								component.baseData.range = 30f;
								component.baseData.damage = 15f;
								component.baseData.force = 1f;
								component.collidesWithPlayer = false;
								component.AdditionalScaleMultiplier = 0.33f;
								component.baseData.speed = 0.20f;
								*/
								Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
								Vector2 unitCenter2 = nearestEnemy.specRigidbody.HitboxPixelCollider.UnitCenter;
								float z = BraveMathCollege.Atan2Degrees((unitCenter2 - unitCenter).normalized);
								Projectile projectile = ((Gun)ETGMod.Databases.Items[146]).DefaultModule.projectiles[0];
								GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, this.m_aiActor.sprite.WorldCenter, Quaternion.Euler(0f, 0f, z), true);
								Projectile component = gameObject.GetComponent<Projectile>();
								bool flag6 = component != null;
								bool flag7 = flag6;
								if (flag7)
								{
									component.baseData.damage = 40f;
									component.Owner = Owner;
									component.baseData.range = 30f;
									component.baseData.damage = 15f;
									component.baseData.force = 1f;
									component.collidesWithPlayer = false;
									component.AdditionalScaleMultiplier *= 0.5f;
									component.baseData.speed *= 0.20f;
									component.collidesWithPlayer = false;
								}
							}
						}
					}
				}
				this.m_aiActor.MovementSpeed = 4f;
			}



			public AIActor GetNearestEnemy(List<AIActor> activeEnemies, Vector2 position, out float nearestDistance, string[] filter)
			{
				AIActor aiactor = null;
				nearestDistance = float.MaxValue;
				bool flag = activeEnemies == null;
				bool flag2 = flag;
				bool flag3 = flag2;
				AIActor result;
				if (flag3)
				{
					result = null;
				}
				else
				{
					for (int i = 0; i < activeEnemies.Count; i++)
					{
						AIActor aiactor2 = activeEnemies[i];
						bool flag4 = aiactor2.healthHaver && aiactor2.healthHaver.IsVulnerable;
						bool flag5 = flag4;
						bool flag6 = flag5;
						if (flag6)
						{
							bool flag7 = !aiactor2.healthHaver.IsDead;
							bool flag8 = flag7;
							bool flag9 = flag8;
							if (flag9)
							{
								bool flag10 = filter == null || !filter.Contains(aiactor2.EnemyGuid);
								bool flag11 = flag10;
								bool flag12 = flag11;
								if (flag12)
								{
									float num = Vector2.Distance(position, aiactor2.CenterPosition);
									bool flag13 = num < nearestDistance;
									bool flag14 = flag13;
									bool flag15 = flag14;
									if (flag15)
									{
										nearestDistance = num;
										aiactor = aiactor2;
									}
								}
							}
						}
					}
					result = aiactor;
				}
				return result;
			}

			public bool IsInRange(AIActor enemy)
			{

				bool flag;
				if (enemy == null)
				{
					flag = true;
				}
				else
				{
					SpeculativeRigidbody specRigidbody = enemy.specRigidbody;
					Vector2? vector = (specRigidbody != null) ? new Vector2?(specRigidbody.UnitCenter) : null;
					flag = (vector == null);
				}
				bool flag2 = flag;
				return !flag2 && Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, enemy.specRigidbody.UnitCenter) <= this.GetMinReadyRange();
			}

			public override float GetMaxRange()
			{
				return 40f;
			}

			public override float GetMinReadyRange()
			{
				return 20f;
			}

			public override bool IsReady()
			{
				AIActor aiActor = this.m_aiActor;
				bool flag;
				if (aiActor == null)
				{
					flag = true;
				}
				else
				{
					SpeculativeRigidbody targetRigidbody = aiActor.TargetRigidbody;
					Vector2? vector = (targetRigidbody != null) ? new Vector2?(targetRigidbody.UnitCenter) : null;
					flag = (vector == null);
				}
				bool flag2 = flag;
				return !flag2 && Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, this.m_aiActor.TargetRigidbody.UnitCenter) <= this.GetMinReadyRange();
			}



			public string attackAnimation = "attack";

			private bool isAttacking;

			private float attackCooldown = 2f;

			private float attackDuration = 0.01f;

			private float attackTimer;

			private float attackCooldownTimer;


			public PlayerController o;

			public GameObject Rocket;
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
				bool flag = this.repathTimer > 0f;
				BehaviorResult result;
				if (flag)
				{
					result = ((overrideTarget == null) ? BehaviorResult.Continue : BehaviorResult.SkipRemainingClassBehaviors);
				}
				else
				{
					bool flag2 = overrideTarget == null;
					if (flag2)
					{
						this.PickNewTarget();
						result = BehaviorResult.Continue;
					}
					else
					{
						this.isInRange = (Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, overrideTarget.UnitCenter) <= this.DesiredDistance);
						bool flag3 = overrideTarget != null && !this.isInRange;
						if (flag3)
						{
							this.m_aiActor.PathfindToPosition(overrideTarget.UnitCenter, null, true, null, null, null, false);
							this.repathTimer = this.PathInterval;
							result = BehaviorResult.SkipRemainingClassBehaviors;
						}
						else
						{
							bool flag4 = overrideTarget != null && this.repathTimer >= 0f;
							if (flag4)
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




