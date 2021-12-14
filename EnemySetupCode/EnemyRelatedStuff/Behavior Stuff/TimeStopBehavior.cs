using System;
using System.Collections.Generic;
using UnityEngine;
using Dungeonator;
using FullInspector;
using Planetside;
using System.Collections;
using ItemAPI;

public class TimeStopBehavior : BasicAttackBehavior
{
	public override void Start()
	{
		base.Start();
		this.m_aiActor.healthHaver.OnDeath += (obj) =>
		{
			
		};
	}



	public bool TimeStopped
	{
		get
		{
			return this.IsTimeStopped;
		}
	}
	public bool LaserActive
	{
		get
		{
			return this.m_laserActive;
		}
	}




	public bool m_laserActive;
	public bool IsTimeStopped;
	public float AlaserAngle;


	public BasicBeamController m_laserBeam;
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
		this.m_state = TimeStopBehavior.State.PreCharging;
		this.m_aiActor.SuppressTargetSwitch = true;
		this.m_aiActor.ClearPath();
		this.m_updateEveryFrame = true;



		if (!string.IsNullOrEmpty(this.ChargeAnimation))
		{
			this.m_aiAnimator.PlayUntilCancelled(this.ChargeAnimation, true, null, -1f, false);

		}

		return BehaviorResult.RunContinuous;
	}

	public override ContinuousBehaviorResult ContinuousUpdate()
	{

		base.ContinuousUpdate();
		if (this.m_state == TimeStopBehavior.State.PreCharging)
		{
			if (!this.LaserActive)
			{
				this.ChargeFiringLaser(this.chargeTime);
				this.m_timer = this.chargeTime;
				this.m_state = TimeStopBehavior.State.Charging;
			}
		}
		else
		{
			if (this.m_state == TimeStopBehavior.State.Charging)
			{
				this.m_timer -= this.m_deltaTime;
				if (this.m_timer <= 0f)
				{
					this.m_state = TimeStopBehavior.State.TimeStopped;
					this.StartTimeStop();
					this.m_timer = this.firingTime;
				}
				return ContinuousBehaviorResult.Continue;
			}
			if (this.m_state == TimeStopBehavior.State.TimeStopped)
			{
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
				this.m_timer -= this.m_deltaTime;
				if (this.m_timer <= 0f || !this.TimeStopped)
				{
					if (this.m_bulletSource != null)
					{
						this.m_bulletSource.ForceStop();
					}
					return ContinuousBehaviorResult.Finished;
				}
			}
		}
		return ContinuousBehaviorResult.Continue;
	}

	public override void EndContinuousUpdate()
	{
		base.EndContinuousUpdate();
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

	}

	public void ChargeFiringLaser(float time)
	{

	}

	public void StartTimeStop()
	{
		AkSoundEngine.PostEvent("Play_OBJ_time_bell_01", this.m_aiActor.gameObject);
		RoomHandler room = this.m_aiActor.GetAbsoluteParentRoom();
		List<AIActor> activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
		bool flag = activeEnemies != null;
		if (flag)
		{
			foreach (AIActor aIActor in activeEnemies)
            {
				if (aIActor != null && aIActor != this.m_aiActor)
                {
					aIActor.LocalTimeScale = 0;
					aIActor.aiAnimator.FpsScale = 0;
					aIActor.behaviorSpeculator.LocalTimeScale = 0;
					EnemiesToDeFreeze.Add(aIActor);
				}
			}
		}
	}


	private List<AIActor> EnemiesToDeFreeze = new List<AIActor>();

	public void StopFiringLaser()
	{
		if (!this.TimeStopped)
		{
			return;
		}
		StopTimeStop();
		if (!string.IsNullOrEmpty(this.PostFireAnimation))
		{
			this.m_aiAnimator.PlayUntilFinished(this.PostFireAnimation, true, null, -1f, false);
		}
		this.m_laserActive = false;
		this.IsTimeStopped = false;
	}

	public void StopTimeStop()
    {
		foreach (AIActor actor in EnemiesToDeFreeze)
        {
			if (actor != null)
            {
				actor.LocalTimeScale = 1;
				actor.aiAnimator.FpsScale = 1;
				actor.behaviorSpeculator.LocalTimeScale = 1;
				EnemiesToDeFreeze.Remove(actor);
			}
        }
    }


	public float chargeTime;

	public float firingTime;




	private TimeStopBehavior.State m_state;

	private float m_timer;

	private Vector2 m_targetPosition;


	private SpeculativeRigidbody m_backupTarget;

	private BulletScriptSource m_bulletSource;
	public bool HasTriggeredScript;

	public BulletScriptSelector BulletScript;
	public Transform ShootPoint;

	private enum State
	{
		PreCharging,
		Charging,
		TimeStopped,
		TimeRestart
	}

	//=====
	//=====
	public string ChargeAnimation;
	public string FireAnimation;
	public string PostFireAnimation;

	public bool LockInPlaceWhileAttacking;
}
