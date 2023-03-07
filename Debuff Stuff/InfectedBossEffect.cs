using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using Pathfinding;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;
namespace Planetside
{
	public class SpawnAdditionalBulletScript : OverrideBehaviorBase
	{
		public SpawnAdditionalBulletScript()
		{
			this.Cooldown = 10f;
			this.timeToHitThreshold = 0.25f;
			this.dodgeChance = 0.5f;
			this.rollDistance = 3f;
			this.m_consideredProjectiles = new List<Projectile>();
		}

		public override void Start()
		{
			CanShoot = true;
			base.Start();
			this.m_updateEveryFrame = true;
			this.m_ignoreGlobalCooldown = true;
			StaticReferenceManager.ProjectileAdded += this.ProjectileAdded;
			StaticReferenceManager.ProjectileRemoved += this.ProjectileRemoved;
		}

		public override void Upkeep()
		{
			base.Upkeep();
			base.DecrementTimer(ref this.m_cooldownTimer, false);
			this.m_cachedShouldDodge = null;
			this.m_cachedRollDirection = null;
		}

		public override bool OverrideOtherBehaviors()
		{
			if (Enabled == false)
			{
				return false;
			}
			Vector2 vector;
			return this.m_cooldownTimer <= 0f && this.ShouldDodgeroll(out vector);
		}

		public override BehaviorResult Update()
		{
			base.Update();
			if (this.m_cooldownTimer > 0f)
			{
				return BehaviorResult.Continue;
			}
			Vector2 vector;



			if (this.ShouldDodgeroll(out vector))
			{
				this.m_aiAnimator.LockFacingDirection = true;
				this.m_aiAnimator.FacingDirection = vector.ToAngle();
				float currentClipLength = this.m_aiAnimator.CurrentClipLength;
				this.m_aiActor.BehaviorOverridesVelocity = true;
				this.m_aiActor.BehaviorVelocity = vector * (this.rollDistance / currentClipLength);
				return BehaviorResult.RunContinuous;
			}
			return BehaviorResult.Continue;
		}

		public override ContinuousBehaviorResult ContinuousUpdate()
		{
			base.ContinuousUpdate();

			return ContinuousBehaviorResult.Continue;
		}

		public override void Destroy()
		{
			StaticReferenceManager.ProjectileAdded -= this.ProjectileAdded;
			StaticReferenceManager.ProjectileRemoved -= this.ProjectileRemoved;
			base.Destroy();
		}

		private void ProjectileAdded(Projectile p)
		{
			if (!p)
			{
				return;
			}
			if (!p.specRigidbody)
			{
				return;
			}
			if (p.specRigidbody.CanCollideWith(GetOwnBody()))
			{
				this.m_consideredProjectiles.Add(p);
			}
		}
		private void ProjectileRemoved(Projectile p)
		{
			this.m_consideredProjectiles.Remove(p);
		}

		public SpeculativeRigidbody GetOwnBody()
		{
			return base.m_aiActor.gameObject.GetComponent<SpeculativeRigidbody>();
		}

		private bool ShouldDodgeroll(out Vector2 rollDirection)
		{
			if (this.m_cachedShouldDodge != null)
			{
				rollDirection = this.m_cachedRollDirection.Value;
				return this.m_cachedShouldDodge.Value;
			}
			for (int i = 0; i < this.m_consideredProjectiles.Count; i++)
			{
				Projectile projectile = this.m_consideredProjectiles[i];

				float num = Vector2.Distance(projectile.specRigidbody.UnitCenter, GetOwnBody().UnitCenter) / projectile.Speed;

				if (num <= this.timeToHitThreshold)
				{
					IntVector2 pixelsToMove = PhysicsEngine.UnitToPixel(projectile.specRigidbody.Velocity * this.timeToHitThreshold * 1.1f);
					CollisionData collisionData;

					PhysicsEngine.Instance.RigidbodyCast(projectile.specRigidbody, pixelsToMove, out collisionData, true, true, null, false);

					if (collisionData != null)
					{

						if (collisionData.OtherRigidbody != null)
						{
							CollisionData.Pool.Free(ref collisionData);

							if (UnityEngine.Random.value <= this.dodgeChance)
							{
								if (CanShoot == true && EnemyIsVisible(base.m_aiActor) == true)
								{
									CanShoot = false;
									base.m_aiActor.StartCoroutine(Cooldownest(9 + returnCooldown(base.m_aiActor.healthHaver.GetMaxHealth())));
								}
							}
						}
					}
					this.m_consideredProjectiles.Remove(projectile);
				}
			}
			this.m_cachedShouldDodge = new bool?(false);
			this.m_cachedRollDirection = new Vector2?(Vector2.zero);
			rollDirection = this.m_cachedRollDirection.Value;
			return this.m_cachedShouldDodge.Value;
		}
		public IEnumerator Cooldownest(float Cooldown)
		{
			float elapsed = 0;
			float Time = 1f;
			StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(base.m_aiActor.sprite.WorldCenter);
			while (elapsed < Time)
			{
				elapsed += BraveTime.DeltaTime;
				yield return null;
			}
			AkSoundEngine.PostEvent("Play_ENM_cult_spew_01", base.m_aiActor.gameObject);
			SpawnManager.SpawnBulletScript(base.m_aiActor, base.m_aiActor.sprite.WorldCenter, base.m_aiActor.GetComponent<AIBulletBank>() ?? base.m_aiActor.transform.Find("tempObjInfection").GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(Retaliation)), StringTableManager.GetEnemiesString("#TRAP", -1));
			AkSoundEngine.PostEvent("Play_ENM_blobulord_bubble_01", base.m_aiActor.gameObject);
			GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.BlueSynergyPoofVFX, false);

			gameObject.transform.position = base.m_aiActor.sprite.WorldCenter;
			gameObject.transform.localScale = Vector3.one * 2;
			UnityEngine.Object.Destroy(gameObject, 2);

			elapsed = 0;
			Time = Cooldown;

			while (elapsed < Time)
			{
				float t = (float)elapsed / (float)Time;
				elapsed += BraveTime.DeltaTime;
				yield return null;
			}
			elapsed = 0;
			Time = 1f;
			Exploder.DoDistortionWave(base.m_aiActor.sprite.WorldCenter, 10f, 0.2f, 20, 0.5f);
			AkSoundEngine.PostEvent("Play_BOSS_doormimic_appear_01", base.m_aiActor.gameObject);
			while (elapsed < Time)
			{
				elapsed += BraveTime.DeltaTime;
				yield return null;
			}
			CanShoot = true;
			yield break;
		}

		public class Retaliation : Script
		{
			public override IEnumerator Top()
			{
				WeightedIntCollection attackWeights = new WeightedIntCollection();
				attackWeights.elements = new WeightedInt[]
				{
					new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "Attack1", value = 1, weight = 1},
					new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "Attack2", value = 2, weight = 0.7f},
					new WeightedInt(){additionalPrerequisites = new DungeonPrerequisite[0], annotation = "Attack3", value = 3, weight = 0.6f},
				};
				switch (attackWeights.SelectByWeight(new System.Random(UnityEngine.Random.Range(1, 100))))
				{
					case 1:
						bool LeftOrRight = (UnityEngine.Random.value > 0.5f) ? false : true;
						float RNGSPIN = LeftOrRight == true ? 11 : -11;
						float OffsetF = UnityEngine.Random.Range(0, 60);
						base.PostWwiseEvent("Play_ENM_cannonball_eyes_01", null);
						for (int e = 0; e < 6; e++)
						{
							base.Fire(Offset.OverridePosition(base.BulletBank.aiActor.sprite.WorldCenter), new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new Retaliation.RotatedBulletBasic(RNGSPIN, 0, 0, StaticUndodgeableBulletEntries.undodgeableBigBullet.Name, (e * 60) + OffsetF, 0.0875f));
						}
						yield return this.Wait(40);

						break;
					case 2:
						base.PostWwiseEvent("Play_BOSS_doormimic_vomit_01", null);
						for (int e = 0; e < 30; e++)
						{
							this.Fire(new Direction(UnityEngine.Random.Range(-12, 12), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(9f, 16), SpeedType.Absolute), new Spore());
						}
						break;
					case 3:
						base.PostWwiseEvent("Play_ENM_rock_blast_02", null);
						float Offseat = UnityEngine.Random.Range(0, 60);
						for (int e = 0; e < 8; e++)
						{
							base.Fire(Offset.OverridePosition(base.BulletBank.aiActor.sprite.WorldCenter), new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new Retaliation.RotatedBulletBasic(0, 0, 0, StaticUndodgeableBulletEntries.undodgeableBigBullet.Name, (e * 45) + Offseat, 0.12f));
						}
						break;
				}
				yield break;
			}


			public class RotatedBulletBasic : Bullet
			{
				public RotatedBulletBasic(float spinspeed, float RevUp, float StartSpeenAgain, string BulletType, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
				{
					this.m_spinSpeed = spinspeed;
					this.TimeToRevUp = RevUp;
					this.StartAgain = StartSpeenAgain;
					this.m_angle = angle;
					this.m_radius = aradius;
					this.m_bulletype = BulletType;
					this.SuppressVfx = true;
				}

				public override IEnumerator Top()
				{
					base.ManualControl = true;
					Vector2 centerPosition = base.Position;
					float radius = 0f;
					for (int i = 0; i < 2400; i++)
					{
						if (i % 10 == 0)
						{
							base.Fire(new Direction(0f, Brave.BulletScript.DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new WeakSpore());
						}
						radius += m_radius;
						centerPosition += this.Velocity / 60f;
						base.UpdateVelocity();
						this.m_angle += this.m_spinSpeed / 60f;
						base.Position = centerPosition + BraveMathCollege.DegreesToVector(this.m_angle, radius);
						yield return base.Wait(1);
					}
					base.Vanish(false);
					yield break;
				}
				private const float ExpandSpeed = 4.5f;
				private const float SpinSpeed = 40f;
				private float m_angle;
				private float m_spinSpeed;
				private float m_radius;
				private string m_bulletype;
				private float TimeToRevUp;
				private float StartAgain;


			}

			public class CrossBullet : Bullet
			{
				public CrossBullet(Vector2 offset, int setupDelay, int setupTime) : base(StaticUndodgeableBulletEntries.undodgeableDefault.Name, false, false, false)
				{
					this.m_offset = offset;
					this.m_setupDelay = setupDelay;
					this.m_setupTime = setupTime;
				}
				public override IEnumerator Top()
				{
					this.ManualControl = true;
					this.m_offset = this.m_offset.Rotate(this.Direction);
					for (int i = 0; i < 360; i++)
					{
						if (i > this.m_setupDelay && i < this.m_setupDelay + this.m_setupTime)
						{
							this.Position += this.m_offset / (float)this.m_setupTime;
						}
						this.Position += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
						yield return this.Wait(1);
					}
					this.Vanish(false);
					yield break;
				}
				private Vector2 m_offset;
				private int m_setupDelay;
				private int m_setupTime;
			}

			public class WeakSpore : Bullet
			{
				public WeakSpore() : base(StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name, false, false, false) { }
				public override IEnumerator Top()
				{
					yield return this.Wait(90);
					base.Vanish(false);
					yield break;
				}
			}

			public class Spore : Bullet
			{
				public Spore() : base(UnityEngine.Random.value > 0.33f ? StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name : StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name, false, false, false) { }
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), UnityEngine.Random.Range(60, 150));
					yield return this.Wait(360);
					base.Vanish(false);
					yield break;
				}
			}
		}



		public bool CanShoot;

		public bool EnemyIsVisible(AIActor enemyToCheck)
		{
			if (enemyToCheck == null) { return false; }
			if (enemyToCheck.sprite.renderer.enabled == false) { return false; }
			if (enemyToCheck.IsGone == true) { return false; }
			if (enemyToCheck.State == AIActor.ActorState.Awakening) { return false; }
			return true;
		}

		public static float returnCooldown(float HP)
		{
			float CC = 0;
			for (int i = 0; i < HP; i++)
			{
				if (i % 500 == 0)
				{ CC++; }
			}
			return CC;
		}

		public bool Enabled;
		public float Cooldown;
		public float timeToHitThreshold;
		public float dodgeChance;
		public float rollDistance;
		private float m_cooldownTimer;
		private List<Projectile> m_consideredProjectiles;
		private bool? m_cachedShouldDodge;
		private Vector2? m_cachedRollDirection;
	}


	public class InfectedBossController : BraveBehaviour
	{
		public void Start()
		{
			//base.aiActor.behaviorSpeculator.AttackCooldown *= 1.25f;
			if (base.aiActor.bulletBank == null)
			{

			}
			else
			{
				if (base.aiActor.bulletBank.Bullets == null) { base.aiActor.bulletBank.Bullets = new List<AIBulletBank.Entry>(); }

				base.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
				base.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
				base.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
				base.aiActor.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBigBullet);
			}

			base.aiActor.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;
		}

		private void HealthHaver_OnPreDeath(Vector2 obj)
		{
			SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.INFECTED_FLOOR_COMPLETED, true);
		}

		public void Update()
        {
			if (this.aiActor != null)
            {
				
            }
        }
	}


	public class InfectedBossEffect : GameActorFreezeEffect
	{

		public Color TintColorInfection = new Color(0.05f, 0.3f, 0.9f, 0.7f);

		public static List<GameObject> BuildVFX()
		{
            GeneratedInfectionCrystals = new List<GameObject>();
            GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob1", new List<string>()
            {
                "bigblob1_001",
                "bigblob1_002",
                "bigblob1_003",
                "bigblob1_004",

            }, 6));
            GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob2", new List<string>()
            {
                "bigblob1_001",
                "bigblob1_002",
                "bigblob1_003",
                "bigblob1_004",

            }, 3));

            GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob3", new List<string>()
            {
                "bigblob2_001",
                "bigblob2_002",
                "bigblob2_003",
                "bigblob2_004",

            }, 4));
            GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob4", new List<string>()
            {
                "bigblob2_001",
                "bigblob2_002",
                "bigblob2_003",
                "bigblob2_004",

            }, 7));
            GeneratedInfectionCrystals.Add(GenerateInfectionCrystalFromPath("Blob5", new List<string>()
            {
                "bigblob3_001",
                "bigblob3_002",
                "bigblob3_003",
                "bigblob3_004",
            }, 5));
			return GeneratedInfectionCrystals;
        }


		private static GameObject GenerateInfectionCrystalFromPath(string name, List<string> spritePaths, int FPS)
		{
            var debuffCollection = StaticSpriteDefinitions.Debuff_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle(name, debuffCollection.GetSpriteIdByName(spritePaths.First()), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            tk2dBaseSprite vfxSprite = BrokenArmorVFXObject.GetComponent<tk2dBaseSprite>();
            vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);

            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            var clip = SpriteBuilder.AddAnimation(animator, debuffCollection, new List<int>()
            {
				debuffCollection.GetSpriteIdByName(spritePaths[0]),
                debuffCollection.GetSpriteIdByName(spritePaths[1]),
                debuffCollection.GetSpriteIdByName(spritePaths[2]),
                debuffCollection.GetSpriteIdByName(spritePaths[3]),
            }, "start", tk2dSpriteAnimationClip.WrapMode.Loop, FPS);


            vfxSprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = vfxSprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 2f);
            mat.SetFloat("_EmissivePower", 35);
            vfxSprite.renderer.material = mat;

            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            return BrokenArmorVFXObject;
        }

		public static List<GameObject> GeneratedInfectionCrystals;

		public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			foreach (Tuple<GameObject, float> tuple in effectData.vfxObjects)
			{
				if (tuple.First != null)
				{
					tuple.First.GetComponent<tk2dSprite>().renderer.enabled = EnemyIsVisible(actor.aiActor);
				}
			}
			if (actor != null)
			{
				Elasped += BraveTime.DeltaTime;
				if (Elasped > 0.7f)
				{
					Elasped = 0;
					if (EnemyIsVisible(actor.aiActor) == true)
					{
						SpawnManager.SpawnBulletScript(actor, actor.sprite.WorldCenter, actor.GetComponent<AIBulletBank>() ?? actor.transform.Find("tempObjInfection").GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(SplatWeak)), StringTableManager.GetEnemiesString("#TRAP", -1));
					}
				}
			}
		}

		public float Elasped;


		public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
		{
			if (actor.aiActor != null)
			{
				if (this.FreezeCrystals.Count > 0)
				{
					if (effectData.vfxObjects == null)
					{
						effectData.vfxObjects = new List<Tuple<GameObject, float>>();
					}
					int num = this.crystalNum;
					if (effectData.actor && effectData.actor.specRigidbody && effectData.actor.specRigidbody.HitboxPixelCollider != null)
					{
						//float num2 = effectData.actor.specRigidbody.HitboxPixelCollider.UnitDimensions.x * effectData.actor.specRigidbody.HitboxPixelCollider.UnitDimensions.y;
						//num = Mathf.Max(this.crystalNum, (int)((float)this.crystalNum * (0.5f + num2 / 4f)));
					}
					for (int i = 0; i < num; i++)
					{
						GameObject prefab = BraveUtility.RandomElement<GameObject>(this.FreezeCrystals);
						Vector2 vector = actor.specRigidbody.HitboxPixelCollider.UnitCenter;
						Vector2 vector2 = BraveUtility.RandomVector2(-this.crystalVariation, this.crystalVariation);
						vector += vector2;
						float num3 = BraveMathCollege.QuantizeFloat(vector2.ToAngle(), 360f / (float)this.crystalRot);
						Quaternion rotation = Quaternion.Euler(0f, 0f, num3);
						GameObject gameObject = SpawnManager.SpawnVFX(prefab, vector, rotation, true);
						gameObject.transform.parent = actor.transform;
						tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
						if (component)
						{
							actor.sprite.AttachRenderer(component);
							component.HeightOffGround = 0.1f;
						}
						if (effectData.actor && effectData.actor.specRigidbody && effectData.actor.specRigidbody.HitboxPixelCollider != null)
						{
							Vector2 unitCenter = effectData.actor.specRigidbody.HitboxPixelCollider.UnitCenter;
							float num4 = (float)i * (360f / (float)num);
							Vector2 normalized = BraveMathCollege.DegreesToVector(num4, 1f).normalized;
							normalized.x *= effectData.actor.specRigidbody.HitboxPixelCollider.UnitDimensions.x / 2f;
							normalized.y *= effectData.actor.specRigidbody.HitboxPixelCollider.UnitDimensions.y / 2f;
							float magnitude = normalized.magnitude;
							Vector2 vector3 = unitCenter + normalized;
							vector3 += (unitCenter - vector3).normalized * (magnitude * UnityEngine.Random.Range(0.15f, 0.85f));
							gameObject.transform.position = vector3.ToVector3ZUp(0f);
							gameObject.transform.rotation = Quaternion.Euler(0f, 0f, num4);
						}
						effectData.vfxObjects.Add(Tuple.Create<GameObject, float>(gameObject, num3));
					}
				}

				actor.gameObject.GetOrAddComponent<InfectedBossController>();


				actor.RegisterOverrideColor(TintColorInfection, "Infection");

				actorToTrack = actor.aiActor;

				if (actorToTrack.bulletBank == null)
				{
					GameObject tempObj = new GameObject("tempObjInfection");

					AIBulletBank bulletBank = new AIBulletBank();
					bulletBank.Bullets = new List<AIBulletBank.Entry>();

					bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
					bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);

					bulletBank.FixedPlayerRigidbody = actorToTrack.specRigidbody;
					bulletBank.ActorName = actorToTrack.name ?? "Toddy";
					bulletBank.transforms = new List<Transform>() { actorToTrack.transform };
					tempObj.AddComponent(bulletBank);

					tempObj.transform.parent = actorToTrack.transform;
				}
				else
				{
					if (actorToTrack.bulletBank.Bullets == null) { actorToTrack.bulletBank.Bullets = new List<AIBulletBank.Entry>(); }

					actorToTrack.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
					actorToTrack.bulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
				}


			}
			base.OnEffectApplied(actor, effectData, partialAmount);

		}

		public bool EnemyIsVisible(AIActor enemyToCheck)
		{
			if (enemyToCheck == null) { return false; }
			if (enemyToCheck.sprite.renderer.enabled == false) { return false; }
			if (enemyToCheck.IsGone == true) { return false; }
			if (enemyToCheck.State == AIActor.ActorState.Awakening) { return false; }
			return true;
		}

		public AIActor actorToTrack;

		public class SplatWeak : Script
		{
			public override IEnumerator Top()
			{
				this.Fire(new Direction(UnityEngine.Random.Range(-180, 180), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(1f, 4), SpeedType.Absolute), new Spore());
				yield break;
			}
			public class Spore : Bullet
			{
				public Spore() : base(UnityEngine.Random.value > 0.33f ? StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name : StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name, false, false, false) { }
				public override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), UnityEngine.Random.Range(60, 180));
					yield return this.Wait(300);
					base.Vanish(false);
					yield break;
				}
			}
		}






		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			//base.OnEffectRemoved(actor, effectData);
		}
		public override bool IsFinished(GameActor actor, RuntimeGameActorEffectData effectData, float elapsedTime)
		{
			return false;
		}

	}

}
