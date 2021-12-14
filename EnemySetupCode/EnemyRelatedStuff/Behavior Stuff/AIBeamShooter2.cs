using System;
using System.Collections;
using UnityEngine;

public class AIBeamShooter2 : BraveBehaviour
{
	public float LaserAngle
	{
		get
		{
			return this.m_laserAngle;
		}
	}
	public void SetLaserAngle(float alaserAngle)
	{
		this.m_laserAngle = alaserAngle;
		if (this.m_firingLaser)
		{
			base.aiAnimator.FacingDirection = alaserAngle;
		}
	}

	public bool IsFiringLaser
	{
		get
		{
			return this.m_firingLaser;
		}
	}

	public Vector2 LaserFiringCenter
	{
		get
		{
			return this.beamTransform.position.XY() + this.firingEllipseCenter;
		}
	}

	public AIAnimator CurrentAiAnimator
	{
		get
		{
			return (!this.specifyAnimator) ? base.aiAnimator : this.specifyAnimator;
		}
	}
	public float MaxBeamLength { get; set; }

	public BeamController LaserBeam
	{
		get
		{
			return this.m_laserBeam;
		}
	}
	public bool IgnoreAiActorPlayerChecks { get; set; }

	public void Start()
	{
		base.healthHaver.OnDamaged += this.OnDamaged;
		if (this.specifyAnimator)
		{
			this.m_bodyPart = this.specifyAnimator.GetComponent<BodyPartController>();
		}
	}
	public void Update()
	{
	}
	public void LateUpdate()
	{
		if (this.m_laserBeam && this.MaxBeamLength > 0f)
		{
			this.m_laserBeam.projectile.baseData.range = this.MaxBeamLength;
			this.m_laserBeam.ShowImpactOnMaxDistanceEnd = true;
		}
		if (this.m_firingLaser && this.m_laserBeam)
		{
			this.m_laserBeam.LateUpdatePosition(this.GetTrueLaserOrigin());
		}
		else if (this.m_laserBeam && this.m_laserBeam.State == BasicBeamController.BeamState.Dissipating)
		{
			this.m_laserBeam.LateUpdatePosition(this.GetTrueLaserOrigin());
		}
		else if (this.m_firingLaser && !this.m_laserBeam)
		{
			this.StopFiringLaser();
		}
	}

	protected override void OnDestroy()
	{
		if (this.m_laserBeam)
		{
			this.m_laserBeam.CeaseAttack();
		}
		base.OnDestroy();
	}

	public void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
	{
		if (resultValue <= 0f)
		{
			if (this.m_firingLaser)
			{
				this.chargeVfx.DestroyAll();
				this.StopFiringLaser();
			}
			if (this.m_laserBeam)
			{
				this.m_laserBeam.DestroyBeam();
				this.m_laserBeam = null;
			}
		}
	}

	public void StartFiringLaser(float laserAngle)
	{
		this.m_firingLaser = true;
		SetLaserAngle(laserAngle);

		//this.LaserAngle = laserAngle;
		if (this.m_bodyPart)
		{
			this.m_bodyPart.OverrideFacingDirection = true;
		}
		if (!string.IsNullOrEmpty(this.shootAnim))
		{
			this.CurrentAiAnimator.LockFacingDirection = true;
			this.CurrentAiAnimator.PlayUntilCancelled(this.shootAnim, true, null, -1f, false);
		}
		this.chargeVfx.DestroyAll();
		base.StartCoroutine(this.FireBeam((!this.beamProjectile) ? this.beamModule.GetCurrentProjectile() : this.beamProjectile));
	}

	public void StopFiringLaser()
	{
		this.m_firingLaser = false;
		if (this.m_bodyPart)
		{
			this.m_bodyPart.OverrideFacingDirection = false;
		}
		if (!string.IsNullOrEmpty(this.shootAnim))
		{
			this.CurrentAiAnimator.LockFacingDirection = false;
			this.CurrentAiAnimator.EndAnimationIf(this.shootAnim);
		}
	}

	protected IEnumerator FireBeam(Projectile projectile)
	{
		GameObject beamObject = UnityEngine.Object.Instantiate<GameObject>(projectile.gameObject);
		this.m_laserBeam = beamObject.GetComponent<BasicBeamController>();
		this.m_laserBeam.Owner = base.aiActor;
		this.m_laserBeam.HitsPlayers = (projectile.collidesWithPlayer || (!this.IgnoreAiActorPlayerChecks && base.aiActor && base.aiActor.CanTargetPlayers));
		this.m_laserBeam.HitsEnemies = (projectile.collidesWithEnemies || (base.aiActor && base.aiActor.CanTargetEnemies));
		bool facingNorth = BraveMathCollege.AbsAngleBetween(this.LaserAngle, 90f) < this.northAngleTolerance;
		this.m_laserBeam.HeightOffset = this.heightOffset;
		this.m_laserBeam.RampHeightOffset = ((!facingNorth) ? this.otherRampHeight : this.northRampHeight);
		this.m_laserBeam.ContinueBeamArtToWall = !this.PreventBeamContinuation;
		bool firstFrame = true;
		while (this.m_laserBeam != null && this.m_firingLaser)
		{
			float clampedAngle = BraveMathCollege.ClampAngle360(this.LaserAngle);
			Vector2 dirVec = new Vector3(Mathf.Cos(clampedAngle * 0.0174532924f), Mathf.Sin(clampedAngle * 0.0174532924f)) * 10f;
			this.m_laserBeam.Origin = this.GetTrueLaserOrigin();
			this.m_laserBeam.Direction = dirVec;
			if (firstFrame)
			{
				yield return null;
				firstFrame = false;
			}
			else
			{
				facingNorth = (BraveMathCollege.AbsAngleBetween(this.LaserAngle, 90f) < this.northAngleTolerance);
				this.m_laserBeam.RampHeightOffset = ((!facingNorth) ? this.otherRampHeight : this.northRampHeight);
				yield return null;
				while (Time.timeScale == 0f)
				{
					yield return null;
				}
			}
		}
		if (!this.m_firingLaser && this.m_laserBeam != null)
		{
			this.m_laserBeam.CeaseAttack();
		}
		if (this.TurnDuringDissipation && this.m_laserBeam)
		{
			this.m_laserBeam.SelfUpdate = false;
			while (this.m_laserBeam)
			{
				this.m_laserBeam.Origin = this.GetTrueLaserOrigin();
				yield return null;
			}
		}
		this.m_laserBeam = null;
		yield break;
	}

	private Vector3 GetTrueLaserOrigin()
	{
		Vector2 vector = this.LaserFiringCenter;
		if (this.firingEllipseA != 0f && this.firingEllipseB != 0f)
		{
			float num = Mathf.Lerp(this.eyeballFudgeAngle, 0f, BraveMathCollege.AbsAngleBetween(90f, Mathf.Abs(BraveMathCollege.ClampAngle180(this.LaserAngle))) / 90f);
			vector = BraveMathCollege.GetEllipsePoint(vector, this.firingEllipseA, this.firingEllipseB, this.LaserAngle + num);
		}
		return vector;
	}

	public string shootAnim;
	public AIAnimator specifyAnimator;

	public Transform beamTransform;

	public VFXPool chargeVfx;

	public Projectile beamProjectile;

	public ProjectileModule beamModule;

	public bool TurnDuringDissipation = true;

	public bool PreventBeamContinuation;

	[Header("Depth")]
	public float heightOffset = 1.9f;

	public float northAngleTolerance = 90f;

	public float northRampHeight;

	public float otherRampHeight = 5f;

	public Vector2 firingEllipseCenter;

	public float firingEllipseA;

	public float firingEllipseB;

	public float eyeballFudgeAngle;

	private bool m_firingLaser;

	private float m_laserAngle;

	private BasicBeamController m_laserBeam;

	private BodyPartController m_bodyPart;

}
