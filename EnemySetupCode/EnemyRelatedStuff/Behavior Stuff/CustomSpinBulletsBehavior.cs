using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomSpinBulletsBehavior : BehaviorBase
{
	public override void Start()
	{
		base.Start();
		this.m_bulletBank = this.m_gameObject.GetComponent<AIBulletBank>();
	}

	public override void Upkeep()
	{
		base.Upkeep();
		base.DecrementTimer(ref this.m_regenTimer, false);
	}

	public override BehaviorResult Update()
	{
		base.Update();
		BehaviorResult behaviorResult = base.Update();
		if (behaviorResult != BehaviorResult.Continue)
		{
			return behaviorResult;
		}
		float num = float.MaxValue;
		if (this.m_aiActor && this.m_aiActor.TargetRigidbody)
		{
			num = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox)).magnitude;
		}
		for (int i = 0; i < this.NumBullets; i++)
		{
			float num2 = Mathf.Lerp(this.BulletMinRadius, this.BulletMaxRadius, (float)i / ((float)this.NumBullets - 1f));
			if (num2 * 2f > num)
			{
				for (int e = 0; e < AmountOFLines; e++)

                {
					this.m_projectiles.Add(new CustomSpinBulletsBehavior.ProjectileContainer
					{
						projectile = null,
						angle = (360f/AmountOFLines)*e,
						distFromCenter = num2
					});
				}
			}
			else
			{
				for (int e = 0; e < AmountOFLines; e++)
                {
					GameObject gameObject = this.m_bulletBank.CreateProjectileFromBank(this.GetBulletPosition(0f, num2), (360f / AmountOFLines)*e, this.OverrideBulletName, null, false, true, false);
					Projectile component = gameObject.GetComponent<Projectile>();
					component.specRigidbody.Velocity = Vector2.zero;
					component.ManualControl = true;
					if (this.BulletsIgnoreTiles)
					{
						component.specRigidbody.CollideWithTileMap = false;
					}
					this.m_projectiles.Add(new CustomSpinBulletsBehavior.ProjectileContainer
					{
						projectile = component,
						angle = (360f / AmountOFLines)*e,
						distFromCenter = num2
					});
				}
			}
		}
		this.m_updateEveryFrame = true;
		return BehaviorResult.RunContinuousInClass;
	}

	public override ContinuousBehaviorResult ContinuousUpdate()
	{

		IsEnabled = true;
		if (this.m_aiActor)
		{
			bool flag = this.m_aiActor.CanTargetEnemies && !this.m_aiActor.CanTargetPlayers;
			if (this.m_cachedCharm != flag)
			{
				for (int i = 0; i < this.m_projectiles.Count; i++)
				{
					if (this.m_projectiles[i] != null && this.m_projectiles[i].projectile && this.m_projectiles[i].projectile.gameObject.activeSelf)
					{
						this.m_projectiles[i].projectile.DieInAir(false, false, true, false);
						this.m_projectiles[i].projectile = null;
					}
				}
				this.m_cachedCharm = flag;
			}
		}
		for (int j = 0; j < this.m_projectiles.Count; j++)
		{
			if (!this.m_projectiles[j].projectile || !this.m_projectiles[j].projectile.gameObject.activeSelf)
			{
				this.m_projectiles[j].projectile = null;
			}
		}
		for (int k = 0; k < this.m_projectiles.Count; k++)
		{
			float angle = this.m_projectiles[k].angle + this.m_deltaTime * (float)this.BulletCircleSpeed;
			this.m_projectiles[k].angle = angle;
			Projectile projectile = this.m_projectiles[k].projectile;
			if (projectile)
			{
				projectile.ResetDistance();
				Vector2 bulletPosition = this.GetBulletPosition(angle, this.m_projectiles[k].distFromCenter);
				projectile.specRigidbody.Velocity = (bulletPosition - (Vector2)projectile.transform.position) / BraveTime.DeltaTime;
				if (projectile.shouldRotate)                          //NO IDEA WHY CHANGING THIS TO SPRITE.WORLDCENTER FIXES ERROR. SCARES ME
				{													  //I fixed it. Freedom.
					projectile.transform.rotation = Quaternion.Euler(0f, 0f, 180f + (Quaternion.Euler(0f, 0f, 90f) * (this.ShootPoint.transform.position.XY() - bulletPosition)).XY().ToAngle());
				}
			}
			else if (this.m_regenTimer <= 0f)
			{
				Vector2 bulletPosition2 = this.GetBulletPosition(this.m_projectiles[k].angle, this.m_projectiles[k].distFromCenter);
				if (GameManager.Instance.Dungeon.CellExists(bulletPosition2) && !GameManager.Instance.Dungeon.data.isWall((int)bulletPosition2.x, (int)bulletPosition2.y))
				{
					GameObject gameObject = this.m_bulletBank.CreateProjectileFromBank(bulletPosition2, 0f, this.OverrideBulletName, null, false, true, false);
					projectile = gameObject.GetComponent<Projectile>();
					projectile.specRigidbody.Velocity = Vector2.zero;
					projectile.ManualControl = true;
					if (this.BulletsIgnoreTiles)
					{
						projectile.specRigidbody.CollideWithTileMap = false;
					}
					this.m_projectiles[k].projectile = projectile;
					this.m_regenTimer = this.RegenTimer;
				}
			}
		}
		for (int l = 0; l < this.m_projectiles.Count; l++)
		{
			if (this.m_projectiles[l] != null && this.m_projectiles[l].projectile)
			{
				bool flag2 = this.m_aiActor && this.m_aiActor.CanTargetEnemies;
				this.m_projectiles[l].projectile.collidesWithEnemies = (this.m_projectiles[l].projectile.collidesWithEnemies || flag2);
			}
		}
		return ContinuousBehaviorResult.Continue;
	}

	public override void EndContinuousUpdate()
	{
		IsEnabled = false;
		base.EndContinuousUpdate();
		this.DestroyProjectiles();
		this.m_updateEveryFrame = false;
	}

	public void StartContinuousUpdate()
	{
		this.m_updateEveryFrame = true;
	}

	public override void OnActorPreDeath()
	{
		IsEnabled = false;
		base.OnActorPreDeath();
		this.DestroyProjectiles();
	}

	public override void Destroy()
	{
		IsEnabled = false;
		base.Destroy();
	}

	private Vector2 GetBulletPosition(float angle, float distFromCenter)
	{
		return this.ShootPoint.transform.position.XY() + BraveMathCollege.DegreesToVector(angle, distFromCenter);
	}

	public void DestroyProjectiles()
	{
		for (int i = 0; i < this.m_projectiles.Count; i++)
		{
			Projectile projectile = this.m_projectiles[i].projectile;
			if (projectile != null)
			{
				projectile.DieInAir(false, true, true, false);
			}
		}
		this.m_projectiles.Clear();
	}

	public bool IsEnabled;

	public string OverrideBulletName;

	public GameObject ShootPoint;

	public int NumBullets;

	public float BulletMinRadius;

	public float BulletMaxRadius;

	public int BulletCircleSpeed;

	public bool BulletsIgnoreTiles;

	public float RegenTimer;

	public float AmountOFLines;

	private readonly List<CustomSpinBulletsBehavior.ProjectileContainer> m_projectiles = new List<CustomSpinBulletsBehavior.ProjectileContainer>();

	private AIBulletBank m_bulletBank;

	private bool m_cachedCharm;

	private float m_regenTimer;

	private class ProjectileContainer
	{
		public Projectile projectile;
		public float angle;
		public float distFromCenter;
	}
}
