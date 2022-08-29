using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Pathfinding;
using Dungeonator;
using FullInspector;
using System.Collections;
using Brave.BulletScript;

namespace Planetside
{

	public class Mines : Script
	{
		protected override IEnumerator Top()
		{
			float f = UnityEngine.Random.Range(0, 72);
			for (int i = 0; i <= 5; i++)
			{
				base.PostWwiseEvent("Play_OBJ_mine_set_01", null);
				this.Fire(new Direction((float)(i * 72) + f, DirectionType.Aim, -1f), new Speed(5f, SpeedType.Absolute), new Mine());
			}
			yield break;
		}
		public class Mine : Bullet
		{
			public Mine() : base(StaticUndodgeableBulletEntries.undodgeableMine.Name, false, false, false)
			{

			}
			protected override IEnumerator Top()
			{
				HasDetonated = false;
				HeatIndicatorController radialIndicator = ((GameObject)UnityEngine.Object.Instantiate(StaticVFXStorage.RadialRing, this.Projectile.sprite.WorldCenter, Quaternion.identity, this.Projectile.transform)).GetComponent<HeatIndicatorController>();
				radialIndicator.CurrentColor = Color.cyan.WithAlpha(4f);
				radialIndicator.CurrentRadius = 3.33f;
				radialIndicator.IsFire = false;
				this.Ring = radialIndicator.gameObject;
				base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
				for (int i = 0; i <= 900; i++)
				{
					foreach (PlayerController player in GameManager.Instance.AllPlayers)
                    {
						if (Vector2.Distance(player.sprite.WorldCenter, base.Projectile.sprite.WorldCenter) < 3.1f)
						{ Detonate(); }
                    }
					yield return this.Wait(1f);
				}
				Detonate();
				yield break;
			}

			public void Detonate()
			{
				if (HasDetonated == false)
                {
					AkSoundEngine.PostEvent("Play_ENM_bulletking_slam_01", base.Projectile.gameObject);
					float f = UnityEngine.Random.Range(0, 72);
					for (int i = 0; i <= 20; i++)
					{
						this.Fire(new Direction((float)(i * 18) + f, DirectionType.Aim, -1f), new Speed(20f, SpeedType.Absolute), new Shrapnel(true));
					}
				}
				
				base.Vanish(true);
				if (Ring != null)
                {
					UnityEngine.Object.Destroy(Ring);
				}
			}
			public bool HasDetonated;
			public GameObject Ring;
		}

		public class Shrapnel : Bullet
		{
			public Shrapnel(bool fires) : base(StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name, false, false, false)
			{
				FiresBullets = fires;
			}
			protected override IEnumerator Top()
			{
				for (int i = 0; i <= 10; i++)
                {
					if (FiresBullets == true) { this.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(0f, SpeedType.Absolute), new Shrapnel(false)); }
					yield return this.Wait(1f);
				}
				base.Vanish(true);
				yield break;
			}
			private bool FiresBullets;
		}
	}


	public class UseFakeActiveBehavior : BehaviorBase
	{
		public UseFakeActiveBehavior()
		{
			IsActive = false;
			this.ChanceTouse = 0.33f;
			this.m_consideredProjectiles = new List<Projectile>();
		}

		public override void Start()
		{
			this.Cooldown = ActiveCooldown();
			IsActive = false;
			IsTrulyActive = false;
			m_cooldownTimer = 8;
			base.Start();
			this.m_updateEveryFrame = true;
			this.m_ignoreGlobalCooldown = true;
			StaticReferenceManager.ProjectileAdded += this.ProjectileAdded;
			StaticReferenceManager.ProjectileRemoved += this.ProjectileRemoved;
            base.m_aiActor.healthHaver.OnPreDeath += PreDeath;
		}

        private void PreDeath(Vector2 obj)
        {
			IsTrulyActive = false;
			IsActive = false;
			SetOutline(Color.black);

		}

		public override void Upkeep()
		{
			base.Upkeep();
			base.DecrementTimer(ref this.m_cooldownTimer, false);
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

		public override bool OverrideOtherBehaviors()
		{
			if (Enabled == false)
			{
				return false;
			}
			return this.m_cooldownTimer <= 0f && this.ShouldUseActive() == true;
		}

		public override BehaviorResult Update()
		{
			base.Update();
			BehaviorResult behaviorResult = base.Update();
			if (IsTrulyActive == true)
            {
				SetOutline(new Color(10, 10, 10));
			}
			else
            {
				SetOutline(Color.black);
			}
			if (this.ShouldUseActive() == true && IsActive == false && this.m_cooldownTimer <= 0f)
			{
				base.m_aiActor.StartCoroutine(FakeUseActive());
				return BehaviorResult.Continue;
			}
			this.m_updateEveryFrame = true;
			return BehaviorResult.Continue;
		}


		public Dictionary<string, int> strToID = new Dictionary<string, int>()
		{
			{"gun_friendship", 174 },
			{"fortunes_favor", 105 },
			{"cluster", 308 },
			{"blast_shower", BlastShower.BlastShowerID },
		};

		public float ActiveTime()
        {
			switch (FakeActiveToUse())
            {
                case "gun_friendship":
					return 10;
				case "fortunes_favor":
					return 8;
				case "cluster":
					return 1;
				case "blast_shower":
					return 8;
			}
			return 7;
        }

		public float ActiveCooldown()
		{
			switch (FakeActiveToUse())
			{
				case "gun_friendship":
					return 18;
				case "fortunes_favor":
					return 20;
				case "cluster":
					return 15;
				case "blast_shower":
					return 11;
			}
			return 11;
		}

		public void SetOutline(Color color)
		{
			Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(base.m_aiActor.sprite);
			if (outlineMaterial1 != null)
			{

				if (base.m_aiActor.healthHaver != null && base.m_aiActor != null)
				{
					outlineMaterial1.SetColor("_OverrideColor", color);
				}
			}
		}


		public IEnumerator FakeUseActive()
		{
			int ID = 174;
			strToID.TryGetValue(FakeActiveToUse(), out ID);
			GameObject obj = ShowSpinDownHologram(ID, base.m_aiActor.gameObject);
			UnityEngine.Object.Destroy(obj, 3.5f);
			IsActive = true;
			float elapsed = 0;
			while (elapsed < 2)
			{
				float T = Mathf.Min(elapsed, 1);
				if (obj != null) { obj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, T); }
				elapsed += BraveTime.DeltaTime;
				yield return null;
			}
			IsTrulyActive = true;
			DoActiveAbility();
			AkSoundEngine.PostEvent("Play_OBJ_power_up_01", base.m_aiActor.gameObject);
			GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.MachoBraceBurstVFX, false);
			gameObject.transform.position = base.m_aiActor.sprite.WorldCenter - new Vector2(3.5f,0);
			gameObject.transform.localScale = Vector3.one * 2;
			UnityEngine.Object.Destroy(gameObject, 2);
			elapsed = 0;
			while (elapsed < ActiveTime())
			{
				float T = Mathf.Min(elapsed*2, 1);
				if (obj != null) { obj.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, T); }
				elapsed += BraveTime.DeltaTime;
				yield return null;
			}
			IsTrulyActive = false;
			AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Fade_01", base.m_aiActor.gameObject);
			GameObject aa = SpawnManager.SpawnVFX(StaticVFXStorage.MachoBraceDustupVFX, false);
			aa.transform.position = base.m_aiActor.sprite.WorldCenter - new Vector2(2, 2);
			UnityEngine.Object.Destroy(aa, 2);

			RemoveActiveAbility();
			m_cooldownTimer = ActiveCooldown();
			IsActive = false;
			SetOutline(Color.black);
			yield break;
		}

		public void DoActiveAbility()
        {
			switch (FakeActiveToUse())
			{
				case "gun_friendship":
					base.m_aiActor.gameObject.GetComponent<NemesisController>().GunSwitchTimer /= 5;
					base.m_aiActor.behaviorSpeculator.CooldownScale /= 2;
					return;
				case "fortunes_favor":
					AkSoundEngine.PostEvent("Play_OBJ_fortune_shield_01", base.m_aiActor.gameObject);
					UltraFortunesFavor fortunes = base.m_aiActor.gameObject.AddComponent<UltraFortunesFavor>();
					fortunes.bulletRadius = 3;
					fortunes.beamRadius = 3;
					fortunes.goopRadius = 3;
					fortunes.sparkOctantVFX = (PickupObjectDatabase.GetById(105) as FortuneFavorItem).sparkOctantVFX;
					return;
				case "cluster":
					SpawnManager.SpawnBulletScript(base.m_aiActor, base.m_aiActor.sprite.WorldCenter, base.m_aiActor.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(Mines)), StringTableManager.GetEnemiesString("#TRAP", -1));
					return;
				case "blast_shower":
					base.m_aiActor.EffectResistances = 
						(new List<ActorEffectResistance>() 
						{ 
							new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.Freeze },
							new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.Charm },
							new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.Poison },
							new ActorEffectResistance() { resistAmount = 1, resistType = EffectResistanceType.Fire },

						}).ToArray();
					base.m_aiActor.healthHaver.AllDamageMultiplier -= 0.33f;

					return;
			}
		}
		public void RemoveActiveAbility()
		{
			switch (FakeActiveToUse())
			{
				case "gun_friendship":
					base.m_aiActor.gameObject.GetComponent<NemesisController>().GunSwitchTimer *= 5;
					base.m_aiActor.behaviorSpeculator.CooldownScale *= 2;
					return;
				case "fortunes_favor":
					UltraFortunesFavor fortunes = base.m_aiActor.gameObject.GetComponent<UltraFortunesFavor>();
					UnityEngine.Object.Destroy(fortunes);
					return;
				case "cluster":
					return;
				case "blast_shower":
					base.m_aiActor.EffectResistances =
						(new List<ActorEffectResistance>()
						{

						}).ToArray();
					base.m_aiActor.healthHaver.AllDamageMultiplier += 0.33f;
					return;
			}
		}

		public GameObject ShowSpinDownHologram(int itemId, GameObject obj)
		{
			tk2dSpriteCollectionData collection = AmmonomiconController.ForceInstance.EncounterIconCollection;
			var pickupObject = PickupObjectDatabase.GetById(itemId);
			var spriteName = pickupObject?.encounterTrackable?.journalData?.AmmonomiconSprite;
			if (collection && pickupObject && !string.IsNullOrEmpty(spriteName))
			{

				var spriteId = collection.GetSpriteIdByName(spriteName);


				var m_hologramSprite = obj.transform.Find("spindown hologram")?.gameObject.GetComponent<tk2dSprite>();
				if (m_hologramSprite == null)
				{
					GameObject go = new GameObject("spindown hologram");
					m_hologramSprite = tk2dSprite.AddComponent(go, collection, spriteId);
					m_hologramSprite.transform.parent = obj.transform;
				}
				else
				{
					m_hologramSprite.SetSprite(collection, spriteId);
					m_hologramSprite.ForceUpdateMaterial();
				}
				m_hologramSprite.renderer.enabled = true;
				m_hologramSprite.usesOverrideMaterial = true;
				m_hologramSprite.PlaceAtPositionByAnchor(obj.GetComponent<tk2dSprite>() != null ? obj.GetComponent<tk2dSprite>().WorldTopCenter + new Vector2(0f, 0.25f) : (Vector2)obj.transform.position + new Vector2(0f, 0.5f), tk2dBaseSprite.Anchor.LowerCenter);
				m_hologramSprite.transform.localPosition = m_hologramSprite.transform.localPosition.Quantize(0.0625f);
				return m_hologramSprite.gameObject;
			}
			return null;
		}




		private List<Projectile> m_consideredProjectiles;

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

		public SpeculativeRigidbody GetOwnBody()
		{
			return base.m_aiActor.gameObject.GetComponent<SpeculativeRigidbody>();

		}

		private bool ShouldUseActive()
		{
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

							if (UnityEngine.Random.value <= this.ChanceTouse)
							{
								return true;
							}
						}
					}
					this.m_consideredProjectiles.Remove(projectile);
				}
			}
			return false;
		}

		public string FakeActiveToUse()
        {
			return base.m_aiActor.GetComponent<NemesisController>().HeldActive ?? "gun_friendship";
        }

		public bool Enabled;
		public float Cooldown;
		public float timeToHitThreshold;
		public float ChanceTouse;
		private float m_cooldownTimer;
		private bool IsActive;
		private bool IsTrulyActive;


	}


	public class CustomDodgeRollBehavior : OverrideBehaviorBase
	{
		public CustomDodgeRollBehavior()
		{
			this.Cooldown = 4f;
			this.timeToHitThreshold = 0.25f;
			this.dodgeChance = 0.5f;
			this.dodgeAnim = "dodgeroll";
			this.rollDistance = 3f;
			this.m_consideredProjectiles = new List<Projectile>();
		}
		public override void Start()
		{
			base.Start();
			this.m_updateEveryFrame = true;
			this.m_ignoreGlobalCooldown = true;
			StaticReferenceManager.ProjectileAdded += this.ProjectileAdded;
			StaticReferenceManager.ProjectileRemoved += this.ProjectileRemoved;
		}

		// Token: 0x06004BFA RID: 19450 RVA: 0x00195024 File Offset: 0x00193224
		public override void Upkeep()
		{
			base.Upkeep();
			base.DecrementTimer(ref this.m_cooldownTimer, false);
			this.m_cachedShouldDodge = null;
			this.m_cachedRollDirection = null;
		}

		// Token: 0x06004BFB RID: 19451 RVA: 0x00195064 File Offset: 0x00193264
		public override bool OverrideOtherBehaviors()
		{
			if (Enabled == false)
			{
				return false;
			}

			Vector2 vector;
			return this.m_cooldownTimer <= 0f && this.ShouldDodgeroll(out vector);
		}

		// Token: 0x06004BFC RID: 19452 RVA: 0x0019508C File Offset: 0x0019328C
		public override BehaviorResult Update()
		{
			base.Update();
			if (this.m_cooldownTimer > 0f)
			{
				return BehaviorResult.Continue;
			}
			Vector2 vector;
			if (this.ShouldDodgeroll(out vector) && base.m_aiActor.behaviorSpeculator.AttackCooldown > 0.1f && base.m_aiActor.behaviorSpeculator.IsStunned == false)
			{
				m_cooldownTimer = Cooldown;
				this.m_aiAnimator.LockFacingDirection = true;
				this.m_aiAnimator.FacingDirection = vector.ToAngle();
				this.m_aiAnimator.PlayUntilFinished(this.dodgeAnim, false, null, -1f, false);
				float currentClipLength = this.m_aiAnimator.CurrentClipLength;
				this.m_aiActor.BehaviorOverridesVelocity = true;
				this.m_aiActor.BehaviorVelocity = vector * (this.rollDistance / currentClipLength);
				return BehaviorResult.RunContinuous;
			}
			return BehaviorResult.Continue;
		}

		// Token: 0x06004BFD RID: 19453 RVA: 0x0019512A File Offset: 0x0019332A
		public override ContinuousBehaviorResult ContinuousUpdate()
		{
			base.ContinuousUpdate();
			if (!this.m_aiAnimator.IsPlaying(this.dodgeAnim))
			{
				this.m_aiActor.BehaviorOverridesVelocity = false;
				this.m_aiAnimator.LockFacingDirection = false;
				return ContinuousBehaviorResult.Finished;
			}
			return ContinuousBehaviorResult.Continue;
		}

		// Token: 0x06004BFE RID: 19454 RVA: 0x00195164 File Offset: 0x00193364
		public override void Destroy()
		{
			StaticReferenceManager.ProjectileAdded -= this.ProjectileAdded;
			StaticReferenceManager.ProjectileRemoved -= this.ProjectileRemoved;
			base.Destroy();
		}

		// Token: 0x06004BFF RID: 19455 RVA: 0x00195190 File Offset: 0x00193390
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

		// Token: 0x06004C00 RID: 19456 RVA: 0x001951E1 File Offset: 0x001933E1
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

								List<Vector2> list = new List<Vector2>();
								Vector2 unitCenter = GetOwnBody().UnitCenter;

								for (int j = 0; j < 8; j++)
								{

									bool flag2 = false;
									Vector2 normalized = IntVector2.CardinalsAndOrdinals[j].ToVector2().normalized;
									RaycastResult raycastResult;

									bool flag3 = PhysicsEngine.Instance.Raycast(unitCenter, normalized, 3f, out raycastResult, true, true, int.MaxValue, new CollisionLayer?(CollisionLayer.EnemyCollider), false, null, GetOwnBody());
									RaycastResult.Pool.Free(ref raycastResult);
									float num2 = 0.25f;
									float num3 = num2;

									while (num3 <= this.rollDistance && !flag2 && !flag3)
									{
										if (GameManager.Instance.Dungeon.ShouldReallyFall(unitCenter + num3 * normalized))
										{
											flag2 = true;
										}
										num3 += num2;
									}
									if (!flag3 && !flag2)
									{
										list.Add(normalized);
									}
								}
								if (list.Count != 0)
								{
									this.m_cachedShouldDodge = new bool?(true);
									this.m_cachedRollDirection = new Vector2?(BraveUtility.RandomElement<Vector2>(list));
									rollDirection = this.m_cachedRollDirection.Value;
									return this.m_cachedShouldDodge.Value;
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

		public bool Enabled;

		public float Cooldown;

		public float timeToHitThreshold;

		public float dodgeChance;

		public string dodgeAnim;

		public float rollDistance;

		private float m_cooldownTimer;

		private List<Projectile> m_consideredProjectiles;

		private bool? m_cachedShouldDodge;


		private Vector2? m_cachedRollDirection;
	}


	public class SansTeleportBehavior : OverrideBehaviorBase
	{
		public SansTeleportBehavior()
		{
			this.Cooldown = 3f;
			this.timeToHitThreshold = 0.25f;
			this.dodgeChance = 0.5f;
			this.rollDistance = 3f;
			this.m_consideredProjectiles = new List<Projectile>();
		}

		public override void Start()
		{
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
								m_cooldownTimer = Cooldown;
								AkSoundEngine.PostEvent("Play_ENM_highpriest_dash_01", base.m_aiActor.gameObject);
								GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.BlueSynergyPoofVFX, false);
								gameObject.transform.position = base.m_aiActor.sprite.WorldCenter;
								gameObject.transform.localScale = Vector3.one * 2;
								UnityEngine.Object.Destroy(gameObject, 2);
								DoTeleport();
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

		private void DoTeleport()
		{
			float minDistanceFromPlayerSquared = this.MinDistanceFromPlayer * this.MinDistanceFromPlayer;
			float maxDistanceFromPlayerSquared = this.MaxDistanceFromPlayer * this.MaxDistanceFromPlayer;
			Vector2 playerLowerLeft = Vector2.zero;
			Vector2 playerUpperRight = Vector2.zero;
			bool hasOtherPlayer = false;
			Vector2 otherPlayerLowerLeft = Vector2.zero;
			Vector2 otherPlayerUpperRight = Vector2.zero;
			bool hasDistChecks = (this.MinDistanceFromPlayer > 0f || this.MaxDistanceFromPlayer > 0f) && this.m_aiActor.TargetRigidbody;
			if (hasDistChecks)
			{
				playerLowerLeft = this.m_aiActor.TargetRigidbody.HitboxPixelCollider.UnitBottomLeft;
				playerUpperRight = this.m_aiActor.TargetRigidbody.HitboxPixelCollider.UnitTopRight;
				PlayerController playerController = GetOwnBody().behaviorSpeculator.PlayerTarget as PlayerController;
				if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && playerController)
				{
					PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(playerController);
					if (otherPlayer && otherPlayer.healthHaver.IsAlive)
					{
						hasOtherPlayer = true;
						otherPlayerLowerLeft = otherPlayer.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
						otherPlayerUpperRight = otherPlayer.specRigidbody.HitboxPixelCollider.UnitTopRight;
					}
				}
			}
			IntVector2 bottomLeft = IntVector2.Zero;
			IntVector2 topRight = IntVector2.Zero;
			if (this.StayOnScreen)
			{
				bottomLeft = new IntVector2((int)BraveUtility.ViewportToWorldpoint(new Vector2(0f, 0f), ViewportType.Gameplay).RoundToInt().x, (int)BraveUtility.ViewportToWorldpoint(new Vector2(0f, 0f), ViewportType.Gameplay).RoundToInt().y);
				topRight = new IntVector2((int)BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), ViewportType.Gameplay).x, (int)BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), ViewportType.Gameplay).y) - IntVector2.One;
			}
			CellValidator cellValidator = delegate (IntVector2 c)
			{
				for (int i = 0; i < this.m_aiActor.Clearance.x; i++)
				{
					int num = c.x + i;
					for (int j = 0; j < this.m_aiActor.Clearance.y; j++)
					{
						int num2 = c.y + j;
						if (GameManager.Instance.Dungeon.data.isTopWall(num, num2))
						{
							return false;
						}
						if (this.ManuallyDefineRoom && ((float)num < this.roomMin.x || (float)num > this.roomMax.x || (float)num2 < this.roomMin.y || (float)num2 > this.roomMax.y))
						{
							return false;
						}
					}
				}
				if (hasDistChecks)
				{
					PixelCollider hitboxPixelCollider = GetOwnBody().HitboxPixelCollider;
					Vector2 vector = new Vector2((float)c.x + 0.5f * ((float)this.m_aiActor.Clearance.x - hitboxPixelCollider.UnitWidth), (float)c.y);
					Vector2 aMax = vector + hitboxPixelCollider.UnitDimensions;
					if (this.MinDistanceFromPlayer > 0f)
					{
						if (BraveMathCollege.AABBDistanceSquared(vector, aMax, playerLowerLeft, playerUpperRight) < minDistanceFromPlayerSquared)
						{
							return false;
						}
						if (hasOtherPlayer && BraveMathCollege.AABBDistanceSquared(vector, aMax, otherPlayerLowerLeft, otherPlayerUpperRight) < minDistanceFromPlayerSquared)
						{
							return false;
						}
					}
					if (this.MaxDistanceFromPlayer > 0f)
					{
						if (BraveMathCollege.AABBDistanceSquared(vector, aMax, playerLowerLeft, playerUpperRight) > maxDistanceFromPlayerSquared)
						{
							return false;
						}
						if (hasOtherPlayer && BraveMathCollege.AABBDistanceSquared(vector, aMax, otherPlayerLowerLeft, otherPlayerUpperRight) > maxDistanceFromPlayerSquared)
						{
							return false;
						}
					}
				}
				if (this.StayOnScreen && (c.x < bottomLeft.x || c.y < bottomLeft.y || c.x + this.m_aiActor.Clearance.x - 1 > topRight.x || c.y + this.m_aiActor.Clearance.y - 1 > topRight.y))
				{
					return false;
				}
				if (this.AvoidWalls)
				{
					int k = -1;
					int l;
					for (l = -1; l < this.m_aiActor.Clearance.y + 1; l++)
					{
						if (GameManager.Instance.Dungeon.data.isWall(c.x + k, c.y + l))
						{
							return false;
						}
					}
					k = this.m_aiActor.Clearance.x;
					for (l = -1; l < this.m_aiActor.Clearance.y + 1; l++)
					{
						if (GameManager.Instance.Dungeon.data.isWall(c.x + k, c.y + l))
						{
							return false;
						}
					}
					l = -1;
					for (k = -1; k < this.m_aiActor.Clearance.x + 1; k++)
					{
						if (GameManager.Instance.Dungeon.data.isWall(c.x + k, c.y + l))
						{
							return false;
						}
					}
					l = this.m_aiActor.Clearance.y;
					for (k = -1; k < this.m_aiActor.Clearance.x + 1; k++)
					{
						if (GameManager.Instance.Dungeon.data.isWall(c.x + k, c.y + l))
						{
							return false;
						}
					}
				}
				return true;
			};
			Vector2 b = GetOwnBody().UnitBottomCenter - this.m_aiActor.transform.position.XY();
			//IntVector2? intVector = null;
			IntVector2? randomAvailableCell;
			randomAvailableCell = this.m_aiActor.ParentRoom.GetRandomAvailableCell(new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), false, cellValidator);

			if (randomAvailableCell != null)
			{
				GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.BlueSynergyPoofVFX, false);
				gameObject.transform.position = randomAvailableCell.Value.ToCenterVector3(99);
				gameObject.transform.localScale = Vector3.one * 2;
				UnityEngine.Object.Destroy(gameObject, 2);

				GetOwnBody().transform.position = Pathfinder.GetClearanceOffset(randomAvailableCell.Value, this.m_aiActor.Clearance).WithY((float)randomAvailableCell.Value.y) - b;
				GetOwnBody().Reinitialize();
				GetOwnBody().behaviorSpeculator.Stun(0.66f, false);
			}
			else
			{
				Debug.LogWarning("TELEPORT FAILED!", this.m_aiActor);
			}
		}
		public bool ManuallyDefineRoom;

		[InspectorShowIf("ManuallyDefineRoom")]
		[InspectorIndent]
		public Vector2 roomMin;

		[InspectorShowIf("ManuallyDefineRoom")]
		[InspectorIndent]
		public Vector2 roomMax;

		public bool AvoidWalls;
		public bool StayOnScreen;

		public float MinDistanceFromPlayer;
		public float MaxDistanceFromPlayer;

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


}
