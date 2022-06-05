using System;
using System.Collections.Generic;
using UnityEngine;
using Dungeonator;
using FullInspector;
using Planetside;
using System.Collections;
using ItemAPI;
using System.Linq;

public class CustomBeholsterLaserBehavior : BasicAttackBehavior
{

	public CustomBeholsterLaserBehavior()
    {
		RampHeight = 5;
		LocksFacingDirection = true;
		DoesSpeedLerp = false;
		DoesReverseSpeedLerp = false;
		FacesLaserAngle = false;
		AdditionalHeightOffset = 0;
		StopDuring = StopType.None;
	}

	public override void Start()
	{
		base.Start();
		this.m_aiActor.healthHaver.OnPreDeath += (obj) =>
		{
			if (this.m_laserBeam != null)
			{
				this.m_laserBeam.DestroyBeam();
				this.m_laserBeam = null;
			}
			if (this.BulletScript != null && this.m_bulletSource && !this.m_bulletSource.IsEnded)
			{
				this.m_bulletSource.ForceStop();
			}
		};
	}
	private bool ShowSpecificBeamShooter()
	{
		return this.beamSelection == ShootBeamBehavior.BeamSelection.Specify;
	}
	public Vector2 LaserFiringCenter
	{
		get
		{
			return this.m_aiActor.transform.position.XY();
		}
	}

	public bool FiringLaser
	{
		get
		{
			return this.IsfiringLaser;
		}
	}
	public bool LaserActive
	{
		get
		{
			return this.m_laserActive;
		}
	}
	public float LaserAngle
	{
		get
		{
			return this.AlaserAngle;
		}
	}
	public void SetLaserAngle(float alaserAngle)
	{
		this.AlaserAngle = alaserAngle;
		if (this.IsfiringLaser)
		{
			this.m_aiActor.aiAnimator.FacingDirection = alaserAngle;
		}
	}

	public BasicBeamController LaserBeam
	{
		get
		{
			return this.m_laserBeam;
		}
	}

	public bool m_laserActive;
	public bool IsfiringLaser;
	public float AlaserAngle;
	public BasicBeamController m_laserBeam;

	public VFXPool chargeUpVfx;
	public VFXPool chargeDownVfx;
	public ProjectileModule beamModule;
	public Transform beamTransform;

	public override void Upkeep()
	{
		base.Upkeep();
		if (this.m_aiActor.TargetRigidbody)
		{
			this.m_targetPosition = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
			this.m_backupTarget = this.m_aiActor.TargetRigidbody;
		}
		else if (this.m_backupTarget)
		{
			this.m_targetPosition = this.m_backupTarget.GetUnitCenter(ColliderType.HitBox);
		}
	}

	public override BehaviorResult Update()
	{
		base.Update();
		BehaviorResult behaviorResult = base.Update();
		if (behaviorResult != BehaviorResult.Continue)
		{
			return behaviorResult;
		}
		if (!this.IsReady())
		{
			return BehaviorResult.Continue;
		}
		this.PrechargeFiringLaser();
		this.HasTriggeredScript = false;
		this.m_state = CustomBeholsterLaserBehavior.State.PreCharging;
		if (LockInPlaceWhileAttacking == true)
        {
			this.m_aiActor.SuppressTargetSwitch = true;
			this.m_aiActor.ClearPath();
		}
		this.m_updateEveryFrame = true;

		this.m_allBeamShooters = new List<AIBeamShooter2>(this.m_aiActor.GetComponents<AIBeamShooter2>());
		if (this.beamSelection == ShootBeamBehavior.BeamSelection.All)
		{
			foreach (AIBeamShooter2 c in m_aiActor.gameObject.GetComponents(typeof(AIBeamShooter2)))
			{
				this.m_currentBeamShooters.Add(c);
			}
		}
		else if (this.beamSelection == ShootBeamBehavior.BeamSelection.Random)
		{
			this.m_currentBeamShooters.Add(BraveUtility.RandomElement<AIBeamShooter2>(this.m_allBeamShooters));
		}
		else if (this.beamSelection == ShootBeamBehavior.BeamSelection.Specify)
		{
			foreach (AIBeamShooter2 c in specificBeamShooters)
			{
				this.m_currentBeamShooters.Add(c);
			}
		}

		if (!string.IsNullOrEmpty(this.ChargeAnimation))
		{
			this.m_aiAnimator.PlayUntilCancelled(this.ChargeAnimation, true, null, -1f, false);

		}
		if (StopDuring == StopType.Charge | StopDuring == StopType.All)
        {
			this.StopMoving();
        }
		return BehaviorResult.RunContinuous;
	}

	public bool DoesSpeedLerp;
	public bool DoesReverseSpeedLerp;

	public float InitialStartingSpeed;
	public float TimeToReachFullSpeed;
	public float TimeToStayAtZeroSpeedAt;

	public float EndingSpeed;
	public float TimeToReachEndingSpeed;

	public bool FacesLaserAngle;

	public override ContinuousBehaviorResult ContinuousUpdate()
	{
		base.ContinuousUpdate();
		if (this.m_state == CustomBeholsterLaserBehavior.State.PreCharging)
		{
			if (!this.LaserActive)
			{
				this.ChargeFiringLaser(this.chargeTime);
				this.m_timer = 0;
				this.m_state = CustomBeholsterLaserBehavior.State.Charging;
			}
		}
		else
		{
			if (this.m_state == CustomBeholsterLaserBehavior.State.Charging)
			{
				this.m_timer += this.m_deltaTime;
				if (this.m_timer >= this.chargeTime)
				{
					this.m_state = CustomBeholsterLaserBehavior.State.Firing;
					this.StartFiringTheLaser();
					this.m_timer = 0;
				}
				return ContinuousBehaviorResult.Continue;
			}
			if (this.m_state == CustomBeholsterLaserBehavior.State.Firing)
			{
				if (FacesLaserAngle == true)
                {
					this.m_aiAnimator.LockFacingDirection = true;
					this.m_aiAnimator.FacingDirection = AlaserAngle;
				}

				if (this.HasTriggeredScript != true)
				{
					this.HasTriggeredScript = true;
					if (this.BulletScript != null && !this.BulletScript.IsNull)
					{
						if (!this.m_bulletSource)
						{
							this.m_bulletSource = this.ShootPoint.gameObject.GetOrAddComponent<BulletScriptSource>();
						}
						this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
						this.m_bulletSource.BulletScript = this.BulletScript;
						this.m_bulletSource.Initialize();
					}
				}
				this.m_timer += this.m_deltaTime;
				if (this.m_timer >= firingTime || !this.FiringLaser)
				{
					if (this.m_bulletSource != null)
					{
						this.m_bulletSource.ForceStop();
					}
					this.m_aiAnimator.LockFacingDirection = false;
					return ContinuousBehaviorResult.Finished;
				}
				for (int i = 0; i < this.m_currentBeamShooters.Count; i++)
				{
					bool isFiringLaser = this.m_currentBeamShooters[0].IsFiringLaser;
					if (!isFiringLaser)
					{
						AIBeamShooter2 aibeamShooter = this.m_currentBeamShooters[i];
						Vector2 b = aibeamShooter.transform.position;
						bool flag11 = this.trackingType == CustomBeholsterLaserBehavior.TrackingType.Follow;
						float alaserAngle;
						if (flag11)
						{
							float num = Vector2.Distance(this.m_targetPosition, b);
							float num2 = (this.m_targetPosition - b).ToAngle();
							float num3 = BraveMathCollege.ClampAngle180(num2 - this.AlaserAngle);
							float f = num3 * num * 0.0174532924f;
							float num4 = this.maxTurnRate;
							float num5 = Mathf.Sign(num3);
							bool flag12 = this.m_unitOvershootTimer > 0f;
							if (flag12)
							{
								num5 = this.m_unitOvershootFixedDirection;
								this.m_unitOvershootTimer -= this.m_deltaTime;
								num4 = this.unitOvershootSpeed;
							}
							this.m_currentUnitTurnRate = Mathf.Clamp(this.m_currentUnitTurnRate + num5 * this.turnRateAcceleration * this.m_deltaTime, -num4, num4);
							float num6 = this.m_currentUnitTurnRate / num * 57.29578f;
							float num7 = 0f;
							bool flag13 = this.useDegreeCatchUp && Mathf.Abs(num3) > this.minDegreesForCatchUp;
							if (flag13)
							{
								float b2 = Mathf.InverseLerp(this.minDegreesForCatchUp, 180f, Mathf.Abs(num3)) * this.degreeCatchUpSpeed;
								num7 = Mathf.Max(num7, b2);
							}
							bool flag14 = this.useUnitCatchUp && Mathf.Abs(f) > this.minUnitForCatchUp;
							if (flag14)
							{
								float num8 = Mathf.InverseLerp(this.minUnitForCatchUp, this.maxUnitForCatchUp, Mathf.Abs(f)) * this.unitCatchUpSpeed;
								float b3 = num8 / num * 57.29578f;
								num7 = Mathf.Max(num7, b3);
							}
							bool flag15 = this.useUnitOvershoot && Mathf.Abs(f) < this.minUnitForOvershoot;
							if (flag15)
							{
								this.m_unitOvershootFixedDirection = (float)((this.m_currentUnitTurnRate <= 0f) ? -1 : 1);
								this.m_unitOvershootTimer = this.unitOvershootTime;
							}
							num7 *= Mathf.Sign(num3);
							alaserAngle = BraveMathCollege.ClampAngle360(this.AlaserAngle + (num6 + num7) * this.m_deltaTime);
						}
						else
						{
							float Mhm = this.maxTurnRate;
							if (DoesSpeedLerp == true)
                            {
								float t = ((m_timer / TimeToReachFullSpeed)- TimeToStayAtZeroSpeedAt);
								t = Mathf.Max(t, 0);
								Mhm = Mathf.Lerp(InitialStartingSpeed, maxTurnRate, Mathf.Min(t, 1));
							}
							if (DoesReverseSpeedLerp == true)
                            {
								float stir = firingTime - (TimeToReachEndingSpeed + TimeToStayAtZeroSpeedAt);
								if (m_timer > stir)
								{
									float t2 = (m_timer - stir / firingTime- TimeToStayAtZeroSpeedAt);
									t2 = Mathf.Max(t2, 0);
									Mhm = Mathf.Lerp(EndingSpeed, maxTurnRate, t2);
								}
							}

							alaserAngle = BraveMathCollege.ClampAngle360(this.AlaserAngle + Mhm * this.m_deltaTime);
						}
						bool flag16 = this.IsfiringLaser == true;
						if (flag16)
						{
							this.AlaserAngle = alaserAngle;
						}
						return ContinuousBehaviorResult.Continue;
					}
				}
			}
		}
		return ContinuousBehaviorResult.Continue;
	}

	public override void EndContinuousUpdate()
	{
		base.EndContinuousUpdate();
		if (m_bulletSource != null  && !this.m_bulletSource.IsEnded)
        {
			this.m_bulletSource.ForceStop();
		}
		if (!string.IsNullOrEmpty(this.FireAnimation))
		{
			this.m_aiAnimator.EndAnimationIf(this.FireAnimation);
		}
		this.StopFiringLaser();
		this.m_aiAnimator.LockFacingDirection = false;
		this.m_aiActor.SuppressTargetSwitch = false;
		this.m_updateEveryFrame = false;
		this.UpdateCooldowns();
	}

	public override bool IsOverridable()
	{
		return false;
	}

	public void PrechargeFiringLaser()
	{
		if (LocksFacingDirection == true) { base.m_aiActor.aiAnimator.LockFacingDirection = true; }
		if (EnemyChargeSound != null && UsesBaseSounds == false) { AkSoundEngine.PostEvent(EnemyChargeSound, base.m_aiActor.gameObject); }
		else if (UsesBaseSounds == true) { AkSoundEngine.PostEvent("Play_ENM_beholster_charging_01", base.m_aiActor.gameObject); }
	}

	public void ChargeFiringLaser(float time)
	{
		if (BeamChargingSound != null && UsesBaseSounds == false) { AkSoundEngine.PostEvent(BeamChargingSound, base.m_aiActor.gameObject); }
		else if (UsesBaseSounds == true) { AkSoundEngine.PostEvent("Play_ENM_beholster_charging_01", base.m_aiActor.gameObject); }
		this.m_laserActive = true;
	}

	public void StartFiringTheLaser()
	{

		if (StopDuring == StopType.Attack | StopDuring == StopType.All)
		{
			this.StopMoving(StopDuring == StopType.All);
		}
		MonoBehaviour yes = this.m_aiActor.GetComponent<MonoBehaviour>();
		if (!string.IsNullOrEmpty(this.FireAnimation))
		{
			this.m_aiAnimator.PlayUntilCancelled(this.FireAnimation, true, null, -1f, false);
		}
		float facingDirection = this.m_aiActor.aiAnimator.FacingDirection;
		if (LocksFacingDirection == true) { base.m_aiActor.aiAnimator.LockFacingDirection = true; }
		this.SetLaserAngle(facingDirection);
		this.IsfiringLaser = true;
		for (int i = 0; i < this.m_currentBeamShooters.Count; i++)
		{
			AIBeamShooter2 aibeamShooter2 = this.m_currentBeamShooters[i];
			yes.StartCoroutine(this.FireBeam(aibeamShooter2));
		}
	}


	public void StopFiringLaser()
	{
		if (!this.IsfiringLaser)
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.PostFireAnimation))
		{
			this.m_aiAnimator.PlayUntilFinished(this.PostFireAnimation, true, null, -1f, false);
		}
		//if (StopLaserFiringSound != null) {	AkSoundEngine.PostEvent(StopLaserFiringSound, base.m_aiActor.gameObject);}
		//else if (UsesBaseSounds == true) {	AkSoundEngine.PostEvent("Stop_ENM_deathray_loop_01", base.m_aiActor.gameObject);}
		this.m_behaviorSpeculator.PreventMovement = false;
		this.m_laserActive = false;
		this.IsfiringLaser = false;
		this.m_aiActor.aiAnimator.LockFacingDirection = false;
		this.m_currentBeamShooters.Clear();
	}

	protected IEnumerator FireBeam(AIBeamShooter2 aibeamShooter2)
	{
		GameObject beamObject = null;
		if (UsesBeamProjectileWithoutModule == true)
        {
			beamObject = UnityEngine.Object.Instantiate<GameObject>(aibeamShooter2.beamProjectile.gameObject);
		}
		else
        {
			beamObject = UnityEngine.Object.Instantiate<GameObject>(aibeamShooter2.beamModule.GetCurrentProjectile().gameObject);
		}
		if (beamObject == null) { ETGModConsole.Log("CANNOT FIND beamObject!"); StopFiringLaser(); yield break;}
		BasicBeamController beamCont = beamObject.GetComponent<BasicBeamController>();
		List<AIActor> activeEnemies = base.m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
		for (int i = 0; i < activeEnemies.Count; i++)
		{
			AIActor aiactor = activeEnemies[i];
			if (aiactor && aiactor != base.m_aiActor && aiactor.healthHaver)
			{
				beamCont.IgnoreRigidbodes.Add(aiactor.specRigidbody);
			}
		}
		beamCont.Owner = base.m_aiActor;
		beamCont.HitsPlayers = true;
		beamCont.HitsEnemies = true;

		if (UsesBaseSounds == true) { LaserFiringSound = "Play_ENM_deathray_shot_01"; }
		else if (LaserFiringSound != null && UsesBaseSounds == false) { beamCont.startAudioEvent = LaserFiringSound; }



		if (StopLaserFiringSound != null && UsesBaseSounds == false)
        {
			beamCont.endAudioEvent = StopLaserFiringSound;
		}
		else if (UsesBaseSounds == true) { beamCont.endAudioEvent = "Stop_ENM_deathray_loop_01"; }

		beamCont.HeightOffset = 0f;
		beamCont.RampHeightOffset = 0;

		beamCont.ContinueBeamArtToWall = true;
		float enemyTickCooldown = 0f;
		beamCont.OverrideHitChecks = delegate (SpeculativeRigidbody hitRigidbody, Vector2 dirVec)
		{
			Projectile currentProjectile = null;
			if (UsesBeamProjectileWithoutModule)
			{currentProjectile = aibeamShooter2.beamProjectile;}
			else
            {currentProjectile = aibeamShooter2.beamModule.GetCurrentProjectile();}
			HealthHaver healthHaver = (!hitRigidbody) ? null : hitRigidbody.healthHaver;
			if (hitRigidbody && hitRigidbody.projectile && hitRigidbody.GetComponent<BeholsterBounceRocket>())
			{
				BounceProjModifier component = hitRigidbody.GetComponent<BounceProjModifier>();
				if (component)
				{
					component.numberOfBounces = 0;
				}
				hitRigidbody.projectile.DieInAir(false, true, true, false);
			}
			if (healthHaver != null)
			{
				if (healthHaver.aiActor)
				{
					if (enemyTickCooldown <= 0f)
					{
						healthHaver.ApplyDamage(ProjectileData.FixedFallbackDamageToEnemies, dirVec, this.m_aiActor.GetActorName(), currentProjectile.damageTypes, DamageCategory.Normal, false, null, false);
						enemyTickCooldown = 0.1f;
					}
				}
				else
				{
					healthHaver.ApplyDamage(currentProjectile.baseData.damage, dirVec, this.m_aiActor.GetActorName(), currentProjectile.damageTypes, DamageCategory.Normal, false, null, false);
				}
			}
			if (hitRigidbody.majorBreakable)
			{
				hitRigidbody.majorBreakable.ApplyDamage(26f * BraveTime.DeltaTime, dirVec, false, false, false);
			}
		};
		float ddsasda = this.m_aiActor.aiAnimator.FacingDirection;
		if (firingType == FiringType.ONLY_NORTHANGLEVARIANCE)
		{
			AlaserAngle = 0;
		}
		bool firstFrame = true;
		while (beamCont != null && this.IsfiringLaser)
		{
			enemyTickCooldown = Mathf.Max(enemyTickCooldown - BraveTime.DeltaTime, 0f);
			float clampedAngle = this.m_aiActor.aiAnimator.FacingDirection;
			if (firingType == FiringType.TOWARDS_PLAYER)
            {
				clampedAngle = AlaserAngle;
			}
			else if (firingType == FiringType.TOWARDS_PLAYER_AND_NORTHANGLEVARIANCE)
			{
				clampedAngle = BraveMathCollege.ClampAngle360(aibeamShooter2.northAngleTolerance) + AlaserAngle;
			}
			else if (firingType == FiringType.ONLY_NORTHANGLEVARIANCE)
			{
				clampedAngle = BraveMathCollege.ClampAngle360(aibeamShooter2.northAngleTolerance) + AlaserAngle;
			}
			else if (firingType == FiringType.FACINGDIRECTION_AND_CLAMPED_ANGLE)
			{
				clampedAngle += BraveMathCollege.ClampAngle360(aibeamShooter2.northAngleTolerance);
			}

			//bool facingNorth = BraveMathCollege.ClampAngle180(beamCont.Direction.ToAngle()) > 0f;
			beamCont.RampHeightOffset = RampHeight;



			//SetLaserAngle(clampedAngle);

			/*
			if (flag4)
			{
				clampedAngle += BraveMathCollege.ClampAngle360((aibeamShooter2.northAngleTolerance + this.m_aiActor.aiAnimator.FacingDirection) + AlaserAngle);
			}
			else
			{
				bool usesCustomAngle = this.UsesCustomAngle;
				if (usesCustomAngle == true)
				{
					clampedAngle += BraveMathCollege.ClampAngle360((aibeamShooter2.northAngleTolerance) + AlaserAngle);
				}
				else
				{
					bool firesDirectlyTowardsPlayer = this.FiresDirectlyTowardsPlayer;
					if (firesDirectlyTowardsPlayer)
					{
						clampedAngle += BraveMathCollege.ClampAngle360((this.m_aiActor.aiAnimator.FacingDirection) + AlaserAngle);
					}
					else
					{
						clampedAngle += BraveMathCollege.ClampAngle360(this.LaserAngle);
					}
				}
			}
			*/
			Vector3 dirVec2 = new Vector3(Mathf.Cos(clampedAngle * 0.0174532924f), Mathf.Sin(clampedAngle * 0.0174532924f), 0f) * 10f;
			Vector2 startingPoint = aibeamShooter2.beamTransform.position;
			//aibeamShooter2.gameObject.layer = 30;

			//aibeamShooter2.sprite.allowDefaultLayer = false;

			//aibeamShooter2.gameObject.GetComponent<tk2dBaseSprite>().SortingOrder = 10;
			//aibeamShooter2.sprite.HeightOffGround = -20;
			//aibeamShooter2.sprite.allowDefaultLayer

			//ETGModConsole.Log("Enemy:" + m_aiActor.transform.position.x.ToString() + "," + m_aiActor.transform.position.y.ToString());
			//ETGModConsole.Log("Beam:" + aibeamShooter2.transform.position.x.ToString() + "," + aibeamShooter2.transform.position.y.ToString());
			//float tanAngle = Mathf.Tan(clampedAngle * 0.0174532924f);
			//float sign = (float)((clampedAngle <= 90f || clampedAngle >= 270f) ? 1 : -1);
			//float denominator = Mathf.Sqrt(this.firingEllipseB * this.firingEllipseB + this.firingEllipseA * this.firingEllipseA * (tanAngle * tanAngle));
			//startingPoint.x += sign * this.firingEllipseA * this.firingEllipseB / denominator;
			//startingPoint.y += sign * this.firingEllipseA * this.firingEllipseB * tanAngle / denominator;
			if (aibeamShooter2.firingEllipseA != 0 && aibeamShooter2.firingEllipseB != 0)
            {

				startingPoint = GetTrueLaserOrigin(aibeamShooter2, clampedAngle);
				/*
				float tanAngle = Mathf.Tan(clampedAngle * 0.017453292f);
				float sign = (float)((clampedAngle <= 90f || clampedAngle >= 270f) ? 1 : -1);
				float denominator = Mathf.Sqrt(aibeamShooter2.firingEllipseB * aibeamShooter2.firingEllipseB + aibeamShooter2.firingEllipseA * aibeamShooter2.firingEllipseA * (tanAngle * tanAngle));
				startingPoint.x += sign *aibeamShooter2.firingEllipseA * aibeamShooter2.firingEllipseB / denominator;
				startingPoint.y += sign * aibeamShooter2.firingEllipseA * aibeamShooter2.firingEllipseB * tanAngle / denominator;
				*/
			}
			bool facingNorth = BraveMathCollege.ClampAngle180(beamCont.Direction.ToAngle()) > 0f;
			beamCont.RampHeightOffset = (float)((!facingNorth) ? 5 : 0) + AdditionalHeightOffset;
			beamCont.Origin = startingPoint;
			beamCont.Direction = dirVec2;
			aibeamShooter2.m_laserBeam = beamCont;
			bool flag5 = firstFrame;
			if (flag5)
			{
				yield return null;
				firstFrame = false;
			}
			else
			{
				if (base.m_aiActor == null&& beamCont != null)
                {
					beamCont.CeaseAttack();
					aibeamShooter2.m_laserBeam = null;
					break;
				}
				beamCont.LateUpdatePosition(startingPoint);
				yield return null;
				if (this.IsfiringLaser && !beamCont)
				{
					//beamCont.CeaseAttack();
					this.StopFiringLaser();
					aibeamShooter2.m_laserBeam = null;
					break;
				}
				else if (!this.IsfiringLaser && beamCont)
                {
					beamCont.CeaseAttack();
					aibeamShooter2.m_laserBeam = null;
					this.StopFiringLaser();
					break;
				}
				while (Time.timeScale == 0f)
				{
					yield return null;
				}
			}
		}
		if (!this.IsfiringLaser && beamCont != null)
		{
			aibeamShooter2.m_laserBeam = null;
			beamCont.CeaseAttack();
			beamCont = null;
		}
		yield break;
	}

	private Vector2 GetTrueLaserOrigin(AIBeamShooter2 aIBeamShooter2, float laserAngle)
	{
		Vector2 vector = aIBeamShooter2.LaserFiringCenter - new Vector2(this.m_aiActor.sprite.GetBounds().size.x, this.m_aiActor.sprite.GetBounds().size.y) /2 ;
		if (aIBeamShooter2.firingEllipseA != 0f && aIBeamShooter2.firingEllipseB != 0f)
		{
			float num = Mathf.Lerp(aIBeamShooter2.eyeballFudgeAngle, 0f, BraveMathCollege.AbsAngleBetween(90f, Mathf.Abs(BraveMathCollege.ClampAngle180(laserAngle))) / 90f);
			vector = BraveMathCollege.GetEllipsePoint(vector, aIBeamShooter2.firingEllipseA, aIBeamShooter2.firingEllipseB, laserAngle + num);
		}
		return vector;
	}


	private void StopMoving(bool PreventMoveMent = true)
	{
		if (this.m_aiActor)
		{
			this.m_aiActor.ClearPath();
			this.m_behaviorSpeculator.PreventMovement = PreventMoveMent;
		}
	}


	public CustomBeholsterLaserBehavior.TrackingType trackingType;
	public CustomBeholsterLaserBehavior.FiringType firingType;

	public float initialAimOffset;

	public float AdditionalHeightOffset;

	public float chargeTime;

	public float firingTime;

	public float maxTurnRate;

	public float turnRateAcceleration;

	public bool useDegreeCatchUp;

	public float minDegreesForCatchUp;
	public float degreeCatchUpSpeed;

	public bool useUnitCatchUp;
	public float minUnitForCatchUp;

	public float maxUnitForCatchUp;
	public float unitCatchUpSpeed;

	public bool useUnitOvershoot;
	public float minUnitForOvershoot;

	public float unitOvershootTime;

	public float unitOvershootSpeed;

	private CustomBeholsterLaserBehavior.State m_state;

	private float m_timer;
	public bool LocksFacingDirection;

	private Vector2 m_targetPosition;

	private float m_currentUnitTurnRate;

	private float m_unitOvershootFixedDirection;

	private float m_unitOvershootTimer;

	private SpeculativeRigidbody m_backupTarget;

	private BulletScriptSource m_bulletSource;
	public bool HasTriggeredScript;

	public BulletScriptSelector BulletScript;
	public Transform ShootPoint;

	public CustomBeholsterLaserBehavior.StopType StopDuring;

	public enum StopType
	{
		None,
		All,
		Attack,
		Charge,
	}
	private enum State
	{
		PreCharging,
		Charging,
		Firing
	}
	public enum FiringType
    {
		TOWARDS_PLAYER,
		TOWARDS_PLAYER_AND_NORTHANGLEVARIANCE,
		ONLY_NORTHANGLEVARIANCE,
		FACINGDIRECTION_AND_CLAMPED_ANGLE
	}
	public enum TrackingType
	{
		Follow,
		ConstantTurn
	}
	//=====
	public bool FiresDirectlyTowardsPlayer;
	public bool UsesCustomAngle;
	public float RampHeight;
	//=====
	public string ChargeAnimation;
	public string FireAnimation;
	public string PostFireAnimation;

	public ShootBeamBehavior.BeamSelection beamSelection;
	public List<AIBeamShooter2> specificBeamShooters;
	private List<AIBeamShooter2> m_allBeamShooters;
	private readonly List<AIBeamShooter2> m_currentBeamShooters = new List<AIBeamShooter2>();

	public string LaserFiringSound;
	public string StopLaserFiringSound;
	public string EnemyChargeSound;
	public string BeamChargingSound;
	public bool UsesBaseSounds;

	public bool LockInPlaceWhileAttacking;

	public bool UsesBeamProjectileWithoutModule;
}



/*

namespace CakeMod
{
	// Token: 0x02000D6E RID: 3438
	public class WizardSpinShootBehavior2 : BasicAttackBehavior
	{
		// Token: 0x0600489C RID: 18588 RVA: 0x00178D4E File Offset: 0x00176F4E
		public WizardSpinShootBehavior2()
		{
			this.LineOfSight = true;
			this.m_bulletPositions = new List<Tuple<Projectile, float>>();
		}

		// Token: 0x0600489D RID: 18589 RVA: 0x00178D68 File Offset: 0x00176F68
		public override void Start()
		{
			m_isCharmed = this.m_aiActor.IsHarmlessEnemy;
			base.Start();
			ShootPoint = m_aiActor.transform;
			IntVector2 intVector = PhysicsEngine.UnitToPixel(this.ShootPoint.position - this.m_aiActor.transform.position);
			int num = PhysicsEngine.UnitToPixel(this.BulletCircleRadius);
			this.m_bulletCatcher = new PixelCollider();
			this.m_bulletCatcher.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Circle;
			this.m_bulletCatcher.CollisionLayer = CollisionLayer.BulletBlocker;
			this.m_bulletCatcher.IsTrigger = true;
			this.m_bulletCatcher.ManualOffsetX = intVector.x - num;
			this.m_bulletCatcher.ManualOffsetY = intVector.y - num;
			this.m_bulletCatcher.ManualDiameter = num * 2;
			this.m_bulletCatcher.Regenerate(this.m_aiActor.transform, true, true);
			this.m_aiActor.specRigidbody.PixelColliders.Add(this.m_bulletCatcher);
			SpeculativeRigidbody specRigidbody = this.m_aiActor.specRigidbody;
			specRigidbody.OnTriggerCollision += this.OnTriggerCollision;
		}

		// Token: 0x0600489E RID: 18590 RVA: 0x00178EB0 File Offset: 0x001770B0
		private void OnTriggerCollision(SpeculativeRigidbody specRigidbody, SpeculativeRigidbody sourceSpecRigidbody, CollisionData collisionData)
		{

			if ((this.State == WizardSpinShootBehavior2.SpinShootState.Spawn || this.State == WizardSpinShootBehavior2.SpinShootState.Prefire) && collisionData.MyPixelCollider == this.m_bulletCatcher && collisionData.OtherRigidbody != null && collisionData.OtherRigidbody.projectile != null)
			{
				Projectile projectile = collisionData.OtherRigidbody.projectile;
				bool flag = (!this.m_isCharmed) ? (projectile.Owner is PlayerController) : (projectile.Owner is AIActor);
				if (flag && projectile.CanBeCaught)
				{
					projectile.specRigidbody.DeregisterSpecificCollisionException(projectile.Owner.specRigidbody);
					projectile.Shooter = this.m_aiActor.specRigidbody;
					projectile.Owner = this.m_aiActor;
					projectile.specRigidbody.Velocity = Vector2.zero;
					projectile.ManualControl = true;
					projectile.baseData.SetAll(EnemyDatabase.GetOrLoadByGuid("c4fba8def15e47b297865b18e36cbef8").bulletBank.GetBullet("default").ProjectileData);
					projectile.UpdateSpeed();
					projectile.specRigidbody.CollideWithTileMap = false;
					projectile.ResetDistance();
					projectile.collidesWithEnemies = this.m_isCharmed;
					projectile.collidesWithPlayer = true;
					projectile.UpdateCollisionMask();
					projectile.sprite.color = new Color(1f, 0.1f, 0.1f);
					projectile.MakeLookLikeEnemyBullet(true);
					projectile.RemovePlayerOnlyModifiers();
					float second = BraveMathCollege.ClampAngle360((collisionData.Contact - this.ShootPoint.position.XY()).ToAngle());
					this.m_bulletPositions.Insert(Mathf.Max(0, this.m_bulletPositions.Count - 1), Tuple.Create<Projectile, float>(projectile, second));
				}
			}
		}

		// Token: 0x0600489F RID: 18591 RVA: 0x00179070 File Offset: 0x00177270
		public override void Upkeep()
		{
			base.Upkeep();
			this.m_stateTimer -= this.m_deltaTime;
			if (this.m_isCharmed != this.m_aiActor.CanTargetEnemies)
			{
				this.m_isCharmed = this.m_aiActor.CanTargetEnemies;
				for (int i = 0; i < this.m_bulletPositions.Count; i++)
				{
					Projectile first = this.m_bulletPositions[i].First;
					if (!(first == null))
					{
						first.collidesWithEnemies = this.m_isCharmed;
						first.UpdateCollisionMask();
					}
				}
			}
		}

		// Token: 0x060048A0 RID: 18592 RVA: 0x00179110 File Offset: 0x00177310
		public override BehaviorResult Update()
		{
			base.Update();
			BehaviorResult behaviorResult = base.Update();
			if (behaviorResult != BehaviorResult.Continue)
			{
				return behaviorResult;
			}
			if (!this.IsReady())
			{
				return BehaviorResult.Continue;
			}
			if (!this.m_aiActor.HasLineOfSightToTarget)
			{
				return BehaviorResult.Continue;
			}
			this.bottleowhiskey(SpinShootState.Spawn);
			this.m_updateEveryFrame = true;
			return BehaviorResult.RunContinuous;
		}

		// Token: 0x060048A1 RID: 18593 RVA: 0x00179164 File Offset: 0x00177364
		public override ContinuousBehaviorResult ContinuousUpdate()
		{
			for (int i = this.m_bulletPositions.Count - 1; i >= 0; i--)
			{
				float num = this.m_bulletPositions[i].Second + this.m_deltaTime * (float)this.BulletCircleSpeed;
				this.m_bulletPositions[i].Second = num;
				Projectile first = this.m_bulletPositions[i].First;
				if (!(first == null))
				{
					if (!first)
					{
						this.m_bulletPositions[i] = null;
					}
					else
					{
						Vector2 bulletPosition = this.GetBulletPosition(num);
						first.specRigidbody.Velocity = (bulletPosition.ToVector3XUp() - first.transform.position) / BraveTime.DeltaTime;
						if (first.shouldRotate)
						{
							first.transform.rotation = Quaternion.Euler(0f, 0f, 180f + (Quaternion.Euler(0f, 0f, 90f) * (this.ShootPoint.position.XY() - bulletPosition)).XY().ToAngle());
						}
						first.ResetDistance();
					}
				}
			}
			if (this.State == WizardSpinShootBehavior2.SpinShootState.Spawn)
			{
				while (this.m_stateTimer <= 0f && this.State == WizardSpinShootBehavior2.SpinShootState.Spawn)
				{
					AIBulletBank.Entry bullet = (EnemyDatabase.GetOrLoadByGuid("c4fba8def15e47b297865b18e36cbef8").bulletBank.GetBullet("default"));
					GameObject bulletObject = bullet.BulletObject;
					float num2 = 0f;
					if (this.m_bulletPositions.Count > 0)
					{
						num2 = BraveMathCollege.ClampAngle360(this.m_bulletPositions[this.m_bulletPositions.Count - 1].Second - this.BulletAngleDelta);
					}
					GameObject gameObject = SpawnManager.SpawnProjectile(bulletObject, this.GetBulletPosition(num2), Quaternion.Euler(0f, 0f, 0f), true);
					Projectile component = gameObject.GetComponent<Projectile>();
					if (bullet != null && bullet.OverrideProjectile)
					{
						component.baseData.SetAll(bullet.ProjectileData);
					}
					component.Shooter = this.m_aiActor.specRigidbody;
					component.specRigidbody.Velocity = Vector2.zero;
					component.ManualControl = true;
					component.specRigidbody.CollideWithTileMap = false;
					component.collidesWithEnemies = this.m_isCharmed;
					component.UpdateCollisionMask();
					this.m_bulletPositions.Add(Tuple.Create<Projectile, float>(component, num2));
					this.m_stateTimer += this.SpawnDelay;
					if (this.m_bulletPositions.Count >= this.NumBullets)
					{
						this.bottleowhiskey(SpinShootState.Prefire);
					}
				}
			}
			else if (this.State == WizardSpinShootBehavior2.SpinShootState.Prefire)
			{
				if (this.m_stateTimer <= 0f)
				{
					this.bottleowhiskey(SpinShootState.Fire);
				}
			}
			else if (this.State == WizardSpinShootBehavior2.SpinShootState.Fire)
			{
				if (this.m_behaviorSpeculator.TargetBehaviors != null && this.m_behaviorSpeculator.TargetBehaviors.Count > 0)
				{
					this.m_behaviorSpeculator.TargetBehaviors[0].Update();
				}
				if (this.m_bulletPositions.All((Tuple<Projectile, float> t) => t.First == null))
				{
					return ContinuousBehaviorResult.Finished;
				}
				while (this.m_stateTimer <= 0f)
				{
					Vector2 vector = this.ShootPoint.position.XY();
					Vector2 b = vector + ((!this.m_aiActor.TargetRigidbody) ? Vector2.zero : (this.m_aiActor.TargetRigidbody.UnitCenter - vector)).normalized * this.BulletCircleRadius;
					int num3 = -1;
					float num4 = float.MaxValue;
					for (int j = 0; j < this.m_bulletPositions.Count; j++)
					{
						Projectile first2 = this.m_bulletPositions[j].First;
						if (!(first2 == null))
						{
							float sqrMagnitude = (first2.specRigidbody.UnitCenter - b).sqrMagnitude;
							if (sqrMagnitude < num4)
							{
								num4 = sqrMagnitude;
								num3 = j;
							}
						}
					}
					if (num3 >= 0)
					{
						Projectile first3 = this.m_bulletPositions[num3].First;
						first3.ManualControl = false;
						first3.specRigidbody.CollideWithTileMap = true;
						if (this.m_aiActor.TargetRigidbody)
						{
							Vector2 unitCenter = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
							float speed = first3.Speed;
							float d = Vector2.Distance(first3.specRigidbody.UnitCenter, unitCenter) / speed;
							Vector2 b2 = unitCenter + this.m_aiActor.TargetRigidbody.specRigidbody.Velocity * d;
							Vector2 a = Vector2.Lerp(unitCenter, b2, this.LeadAmount);
							first3.SendInDirection(a - first3.specRigidbody.UnitCenter, true, true);
						}
						first3.transform.rotation = Quaternion.Euler(0f, 0f, first3.specRigidbody.Velocity.ToAngle());
						this.m_bulletPositions[num3].First = null;
					}
					else
					{
						Debug.LogError("WizardSpinShootBehaviour.ContinuousUpdate(): This shouldn't happen!");
					}
					this.m_stateTimer += this.FireDelay;
					if (this.m_bulletPositions.All((Tuple<Projectile, float> t) => t.First == null))
					{
						return ContinuousBehaviorResult.Finished;
					}
				}
			}
			return ContinuousBehaviorResult.Continue;
		}

		// Token: 0x060048A2 RID: 18594 RVA: 0x0017971F File Offset: 0x0017791F
		public override void EndContinuousUpdate()
		{
			base.EndContinuousUpdate();
			this.FreeRemainingProjectiles();
			this.bottleowhiskey(SpinShootState.None);
			this.m_aiAnimator.EndAnimationIf("attack");
			this.UpdateCooldowns();
			this.m_updateEveryFrame = false;
		}

		// Token: 0x060048A3 RID: 18595 RVA: 0x00179752 File Offset: 0x00177952
		public override void OnActorPreDeath()
		{
			base.OnActorPreDeath();
			SpeculativeRigidbody specRigidbody = this.m_aiActor.specRigidbody;
			specRigidbody.OnTriggerCollision = (SpeculativeRigidbody.OnTriggerDelegate)Delegate.Remove(specRigidbody.OnTriggerCollision, new SpeculativeRigidbody.OnTriggerDelegate(this.OnTriggerCollision));
			this.FreeRemainingProjectiles();
		}

		// Token: 0x060048A4 RID: 18596 RVA: 0x0017978C File Offset: 0x0017798C
		public override void Destroy()
		{
			base.Destroy();
			this.bottleowhiskey(SpinShootState.None);
		}

		private float BulletAngleDelta
		{
			get
			{
				return 360f / (float)this.NumBullets;
			}
		}

		private Vector2 GetBulletPosition(float angle)
		{
			return this.ShootPoint.position.XY() + new Vector2(Mathf.Cos(angle * 0.0174532924f), Mathf.Sin(angle * 0.0174532924f)) * this.BulletCircleRadius;
		}
		private WizardSpinShootBehavior2.SpinShootState State
		{
			get
			{
				return this.m_state;
			}
		}

		public void bottleowhiskey(SpinShootState fuck)
		{
			this.EndState(this.m_state);
			this.m_state = fuck;
			this.BeginState(this.m_state);
		}

		// Token: 0x060048A9 RID: 18601 RVA: 0x00179814 File Offset: 0x00177A14
		private void BeginState(WizardSpinShootBehavior2.SpinShootState state)
		{
			if (state == WizardSpinShootBehavior2.SpinShootState.None)
			{
				this.m_bulletPositions.Clear();
			}
			if (state == WizardSpinShootBehavior2.SpinShootState.Spawn)
			{
				this.m_aiAnimator.PlayUntilCancelled("cast", true, null, -1f, false);
				this.m_stateTimer = this.FirstSpawnDelay;
				if (this.m_aiActor && this.m_aiActor.knockbackDoer)
				{
					this.m_aiActor.knockbackDoer.SetImmobile(true, "WizardSpinShootBehavior2");
				}
				this.m_aiActor.ClearPath();
			}
			else if (state == WizardSpinShootBehavior2.SpinShootState.Prefire)
			{
				this.m_aiAnimator.PlayUntilFinished("attack", true, null, -1f, false);
				this.m_stateTimer = this.PrefireDelay;
				if (this.PrefireUseAnimTime)
				{
					this.m_stateTimer += (float)this.m_aiAnimator.spriteAnimator.CurrentClip.frames.Length / this.m_aiAnimator.spriteAnimator.CurrentClip.fps;
				}
			}
			else if (state == WizardSpinShootBehavior2.SpinShootState.Fire)
			{
				this.m_stateTimer = this.FirstFireDelay;
			}
		}

		// Token: 0x060048AA RID: 18602 RVA: 0x001799A8 File Offset: 0x00177BA8
		private void EndState(WizardSpinShootBehavior2.SpinShootState state)
		{
			if (state == WizardSpinShootBehavior2.SpinShootState.Spawn)
			{
			}
			if (this.m_aiActor && this.m_aiActor.knockbackDoer)
			{
				this.m_aiActor.knockbackDoer.SetImmobile(false, "WizardSpinShootBehavior2");
			}
		}

		// Token: 0x060048AB RID: 18603 RVA: 0x00179A64 File Offset: 0x00177C64
		private void FreeRemainingProjectiles()
		{
			for (int i = 0; i < this.m_bulletPositions.Count; i++)
			{
				Projectile first = this.m_bulletPositions[i].First;
				if (!(first == null))
				{
					first.ManualControl = false;
					first.specRigidbody.CollideWithTileMap = true;
					first.SendInDirection((Quaternion.Euler(0f, 0f, 90f) * (first.specRigidbody.UnitCenter - this.ShootPoint.position.XY())).XY(), true, true);
					first.transform.rotation = Quaternion.Euler(0f, 0f, first.specRigidbody.Velocity.ToAngle());
					this.m_bulletPositions[i].First = null;
				}
			}
		}

		// Token: 0x04003C91 RID: 15505
		public bool LineOfSight;

		// Token: 0x04003C92 RID: 15506
		public string OverrideBulletName = "whydoineedthis";

		// Token: 0x04003C93 RID: 15507
		public bool CanHitEnemies = false;

		// Token: 0x04003C94 RID: 15508
		public Transform ShootPoint;

		// Token: 0x04003C95 RID: 15509
		public int NumBullets;

		public int BulletCircleSpeed = 3;

		public float BulletCircleRadius = 3f;

		public float FirstSpawnDelay = 0f;

		public float SpawnDelay = 0f;

		public bool PrefireUseAnimTime = true;

		public float PrefireDelay = 1f;

		public float FirstFireDelay = 1f;

		public float FireDelay = 1f;

		public float LeadAmount = 0f;


		private WizardSpinShootBehavior2.SpinShootState m_state;

		private float m_stateTimer = 5;

		private bool m_isCharmed;

		private List<Tuple<Projectile, float>> m_bulletPositions;

		private PixelCollider m_bulletCatcher;

		public enum SpinShootState
		{
			None,
			Spawn,
			Prefire,
			Fire
		}
	}
}
*/