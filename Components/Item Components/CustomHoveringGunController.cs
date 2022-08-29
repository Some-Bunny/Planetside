using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using Dungeonator;
using UnityEngine;

namespace Planetside
{
	public class CustomHoveringGunController : BraveBehaviour, IPlayerOrbital
	{
		public CustomHoveringGunController()
		{
			this.AimRotationAngularSpeed = 360f;
			this.ShootDuration = 2f;
			this.CooldownTime = 1f;
			this.ChanceToConsumeTargetGunAmmo = 1f;
			this.Radius = 2.5f;
			this.RotationSpeed = 120;
			this.DamageMultiplier = 1;
			this.MagnitudePower = 15;
		}

		public Material material;

		public void Initialize(Gun targetGun, PlayerController owner)
		{
			this.m_targetGun = targetGun;
			this.m_owner = owner;
			this.m_parentTransform = new GameObject("hover rotator").transform;
			this.m_parentTransform.parent = base.transform.parent;
			base.transform.parent = this.m_parentTransform;
			base.sprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.MiddleCenter);
			base.sprite.SetSprite(targetGun.sprite.Collection, targetGun.sprite.spriteId);
			SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
			this.m_shootPointTransform = new GameObject("shoot point").transform;
			this.m_shootPointTransform.parent = base.transform;
			this.m_shootPointTransform.localPosition = targetGun.barrelOffset.localPosition;
			if (material != null)
			{
                material.mainTexture = sprite.renderer.material.mainTexture;
                base.sprite.renderer.material = material;

            }
            if (this.Position == CustomHoveringGunController.HoverPosition.CIRCULATE)
			{
				this.SetOrbitalTier(PlayerOrbital.CalculateTargetTier(this.m_owner, this));
				this.SetOrbitalTierIndex(PlayerOrbital.GetNumberOfOrbitalsInTier(this.m_owner, this.GetOrbitalTier()));
				this.m_owner.orbitals.Add(this);
				this.m_ownerCenterAverage = attachObject != null ? attachObject.transform.PositionVector2() : this.m_owner.CenterPosition;
			}
			if (this.Trigger == CustomHoveringGunController.FireType.ON_DODGED_BULLET)
			{
				this.m_owner.OnDodgedProjectile += this.HandleDodgedProjectileFire;
			}
			if (this.Trigger == CustomHoveringGunController.FireType.ON_FIRED_GUN)
			{
				this.m_owner.PostProcessProjectile += this.HandleFiredGun;
			}
			if (this.Aim == CustomHoveringGunController.AimType.NEAREST_ENEMY)
			{
				this.m_fireCooldown = 0.25f;
			}
			this.UpdatePosition();
			LootEngine.DoDefaultSynergyPoof(base.sprite.WorldCenter, false);
			this.m_initialized = true;
		}

		private void HandleFiredGun(Projectile arg1, float arg2)
		{
			if (this.m_fireCooldown <= 0f)
			{
				this.Fire();
			}
		}

		private void HandleDodgedProjectileFire(Projectile sourceProjectile)
		{
			if (this.m_fireCooldown <= 0f && sourceProjectile.collidesWithPlayer)
			{
				this.Fire();
			}
		}

		public void LateUpdate()
		{
			if (!this.m_initialized)
			{
				return;
			}
			if (Dungeon.IsGenerating || GameManager.Instance.IsLoadingLevel)
			{
				return;
			}
			this.UpdatePosition();
			this.UpdateFiring();
		}

		private void AimAt(Vector2 point, bool instant = false)
		{
			Vector2 v = point - base.sprite.WorldCenter;
			float currentAimTarget = BraveMathCollege.Atan2Degrees(v);
			this.m_currentAimTarget = currentAimTarget;
			if (instant)
			{
				this.m_parentTransform.localRotation = Quaternion.Euler(0f, 0f, this.m_currentAimTarget);
			}
		}

		private void UpdatePosition()
		{
			CustomHoveringGunController.AimType aim = this.Aim;
			if (aim != CustomHoveringGunController.AimType.NEAREST_ENEMY)
			{
				if (aim == CustomHoveringGunController.AimType.PLAYER_AIM)
				{
					this.AimAt(this.m_owner.unadjustedAimPoint.XY(), false);
				}
			}
			else
			{
				bool flag = false;
				if (this.m_owner && this.m_owner.CurrentRoom != null)
				{
					float num = -1f;
					AIActor nearestEnemy = this.m_owner.CurrentRoom.GetNearestEnemy(this.m_owner.CenterPosition, out num, true, false);
					if (nearestEnemy)
					{
						this.m_hasEnemyTarget = true;
						this.AimAt(nearestEnemy.CenterPosition, false);
						flag = true;
					}
				}
				if (!flag)
				{
					this.m_hasEnemyTarget = false;
					this.AimAt(this.m_owner.unadjustedAimPoint.XY(), false);
				}
			}
			this.m_parentTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.MoveTowardsAngle(this.m_parentTransform.localRotation.eulerAngles.z, this.m_currentAimTarget, this.AimRotationAngularSpeed * BraveTime.DeltaTime));
			bool flag2 = this.m_parentTransform.localRotation.eulerAngles.z > 90f && this.m_parentTransform.localRotation.eulerAngles.z < 270f;
			if (flag2 && !base.sprite.FlipY)
			{
				base.transform.localPosition += new Vector3(0f, base.sprite.GetUntrimmedBounds().extents.y, 0f);
				this.m_shootPointTransform.localPosition = this.m_shootPointTransform.localPosition.WithY(-this.m_shootPointTransform.localPosition.y);
				base.sprite.FlipY = true;
			}
			else if (!flag2 && base.sprite.FlipY)
			{
				base.sprite.FlipY = false;
				base.transform.localPosition -= new Vector3(0f, base.sprite.GetUntrimmedBounds().extents.y, 0f);
				this.m_shootPointTransform.localPosition = this.m_shootPointTransform.localPosition.WithY(-this.m_shootPointTransform.localPosition.y);
			}
			CustomHoveringGunController.HoverPosition position = this.Position;
			if (position == CustomHoveringGunController.HoverPosition.OVERHEAD)
			{
				this.m_parentTransform.position = (attachObject != null ? attachObject.transform.PositionVector2() + new Vector2(0, Radius): this.m_owner.CenterPosition + new Vector2(0, Radius)).ToVector3ZisY(0f);
				base.sprite.HeightOffGround = 2f;
				base.sprite.UpdateZDepth();
			}
			else if (position == CustomHoveringGunController.HoverPosition.CIRCULATE)
			{
				this.HandleOrbitalMotion();
			}
			else if (position == CustomHoveringGunController.HoverPosition.ROTATE_AROUND_POSITION)
			{
				this.HandlePivotMotion();
			}
		}

		private void HandlePivotMotion()
		{
			Vector2 centerPosition = attachObject != null ? attachObject.transform.PositionVector2() : this.m_owner.CenterPosition;
			if (Vector2.Distance(centerPosition, this.m_parentTransform.position.XY()) > 20f)
			{
				this.m_parentTransform.position = centerPosition.ToVector3ZUp(0f);
				this.m_ownerCenterAverage = centerPosition;
				if (base.specRigidbody)
				{
					base.specRigidbody.Reinitialize();
				}
			}
			Vector2 vector = centerPosition - this.m_ownerCenterAverage;
			float num = Mathf.Lerp(0.1f, MagnitudePower, vector.magnitude / 4);
			float d = Mathf.Min(num * BraveTime.DeltaTime, vector.magnitude);
			Vector2 vector2 = this.m_ownerCenterAverage + (centerPosition - this.m_ownerCenterAverage).normalized * d;
			Vector2 vector3 = vector2 + (Quaternion.Euler(0f, 0f, this.m_parentTransform.eulerAngles.z) * Vector3.right * this.GetOrbitalRadius()).XY();
			this.m_ownerCenterAverage = vector2;
			vector3 = vector3.Quantize(0.0625f);
			Vector2 velocity = (vector3 - this.m_parentTransform.position.XY()) / BraveTime.DeltaTime;
			if (base.specRigidbody)
			{
				base.specRigidbody.Velocity = velocity;
			}
			else
			{
				this.m_parentTransform.position = vector3.ToVector3ZisY(0f);
				base.sprite.HeightOffGround = 0.5f;
				base.sprite.UpdateZDepth();
			}
			this.m_orbitalAngle = vector3.ToAngle() % 360f;
		}


		private void HandleOrbitalMotion()
		{
			Vector2 centerPosition = attachObject != null ? attachObject.transform.PositionVector2() : this.m_owner.CenterPosition;
			if (Vector2.Distance(centerPosition, this.m_parentTransform.position.XY()) > 20f)
			{
				this.m_parentTransform.position = centerPosition.ToVector3ZUp(0f);
				this.m_ownerCenterAverage = centerPosition;
				if (base.specRigidbody)
				{
					base.specRigidbody.Reinitialize();
				}
			}
			Vector2 vector = centerPosition - this.m_ownerCenterAverage;
			float num = Mathf.Lerp(0.1f, MagnitudePower, vector.magnitude / 4);
			float d = Mathf.Min(num * BraveTime.DeltaTime, vector.magnitude);
			float num2 = 360f / (float)PlayerOrbital.GetNumberOfOrbitalsInTier(this.m_owner, this.GetOrbitalTier()) * (float)this.GetOrbitalTierIndex() + BraveTime.ScaledTimeSinceStartup * this.GetOrbitalRotationalSpeed();
			Vector2 vector2 = this.m_ownerCenterAverage + (centerPosition - this.m_ownerCenterAverage).normalized * d;
			Vector2 vector3 = vector2 + (Quaternion.Euler(0f, 0f, num2) * Vector3.right * this.GetOrbitalRadius()).XY();
			this.m_ownerCenterAverage = vector2;
			vector3 = vector3.Quantize(0.0625f);
			Vector2 velocity = (vector3 - this.m_parentTransform.position.XY()) / BraveTime.DeltaTime;
			if (base.specRigidbody)
			{
				base.specRigidbody.Velocity = velocity;
			}
			else
			{
				this.m_parentTransform.position = vector3.ToVector3ZisY(0f);
				base.sprite.HeightOffGround = 0.5f;
				base.sprite.UpdateZDepth();
			}
			this.m_orbitalAngle = num2 % 360f;
		}

		private void UpdateFiring()
		{
			if (this.m_fireCooldown <= 0f)
			{
				CustomHoveringGunController.FireType trigger = this.Trigger;
				if (trigger != CustomHoveringGunController.FireType.ON_RELOAD)
				{
					if (trigger != CustomHoveringGunController.FireType.ON_COOLDOWN)
					{
						if (trigger != CustomHoveringGunController.FireType.ON_DODGED_BULLET)
						{
						}
					}
					else if (this.Aim != CustomHoveringGunController.AimType.NEAREST_ENEMY || this.m_hasEnemyTarget)
					{
						this.Fire();
					}
				}
				else if (this.m_owner && this.m_owner.CurrentGun && this.m_owner.CurrentGun.IsReloading && (!this.OnlyOnEmptyReload || this.m_owner.CurrentGun.ClipShotsRemaining <= 0))
				{
					this.Fire();
				}
			}
			else
			{
				this.m_fireCooldown = (this.m_fireCooldown -= BraveTime.DeltaTime);
			}
		}

		private Vector2 ShootPoint
		{
			get
			{
				return this.m_shootPointTransform.position.XY();
			}
		}

		private void Fire()
		{
			this.m_fireCooldown = this.CooldownTime;
			Projectile currentProjectile = this.m_targetGun.DefaultModule.GetCurrentProjectile();
			bool flag = currentProjectile.GetComponent<BeamController>() != null;
			if (!string.IsNullOrEmpty(this.ShootAudioEvent))
			{
				AkSoundEngine.PostEvent(this.ShootAudioEvent, base.gameObject);
			}
			if (flag)
			{

				this.m_owner.StartCoroutine(this.HandleFireShortBeam(currentProjectile, this.m_owner, this.ShootDuration));
				this.m_fireCooldown = Mathf.Max(this.m_fireCooldown, this.ShootDuration);
			}
			else if (this.m_targetGun.Volley != null)
			{
				if (this.ShootDuration > 0f)
				{
					base.StartCoroutine(this.FireVolleyForDuration(this.m_targetGun.Volley, this.m_owner, this.ShootDuration));
				}
				else
				{
					this.FireVolley(this.m_targetGun.Volley, this.m_owner, this.m_parentTransform.eulerAngles.z, new Vector2?(this.ShootPoint));
				}
			}
			else
			{
				ProjectileModule defaultModule = this.m_targetGun.DefaultModule;
				Projectile currentProjectile2 = defaultModule.GetCurrentProjectile();
				if (currentProjectile2)
				{
					float angleForShot = defaultModule.GetAngleForShot(1f, 1f, null);
					if (!flag)
					{
						this.DoSingleProjectile(currentProjectile2, this.m_owner, this.m_parentTransform.eulerAngles.z + angleForShot, new Vector2?(this.ShootPoint), true);
					}
				}
			}
		}

		private IEnumerator HandleFireShortBeam(Projectile projectileToSpawn, PlayerController source, float duration)
		{
			float elapsed = 0f;
			if (projectileToSpawn != null) { projectileToSpawn.baseData.damage *= DamageMultiplier; }
			BeamController beam = this.BeginFiringBeam(projectileToSpawn, source, this.m_parentTransform.eulerAngles.z, new Vector2?(this.ShootPoint));
			RecursionPreventer recursion = beam.projectile.gameObject.AddComponent<RecursionPreventer>();
			recursion.IsProjectileFiredFromHoveringGun = true;
			yield return null;
			while (elapsed < duration)
			{
				if (!this.m_shootPointTransform || !this)
				{
					break;
				}
				if (!this.m_parentTransform)
				{
					break;
				}
				elapsed += BraveTime.DeltaTime;
				this.ContinueFiringBeam(beam, source, this.m_parentTransform.eulerAngles.z, new Vector2?(this.ShootPoint));
				yield return null;
			}
			this.CeaseBeam(beam);
			if (!string.IsNullOrEmpty(this.FinishedShootingAudioEvent) && this)
			{
				AkSoundEngine.PostEvent(this.FinishedShootingAudioEvent, this.gameObject);
			}
			yield break;
		}

		private IEnumerator FireVolleyForDuration(ProjectileVolleyData volley, PlayerController source, float duration)
		{
			float elapsed = 0f;
			float cooldown = 0f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				cooldown -= BraveTime.DeltaTime;
				if (cooldown <= 0f)
				{
					this.FireVolley(volley, source, this.m_parentTransform.eulerAngles.z, new Vector2?(this.ShootPoint));
					cooldown = this.m_targetGun.DefaultModule.cooldownTime;
					for (int i = 0; i < volley.projectiles.Count; i++)
					{
						if (volley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Charged)
						{
							cooldown = Mathf.Max(cooldown, volley.projectiles[i].maxChargeTime);
							cooldown = Mathf.Max(cooldown, 0.5f);
						}
					}
				}
				yield return null;
			}
			this.m_fireCooldown = this.CooldownTime;
			if (!string.IsNullOrEmpty(this.FinishedShootingAudioEvent))
			{
				AkSoundEngine.PostEvent(this.FinishedShootingAudioEvent, this.gameObject);
			}
			yield break;
		}

		private void FireVolley(ProjectileVolleyData volley, PlayerController source, float targetAngle, Vector2? overrideSpawnPoint)
		{
			if (!string.IsNullOrEmpty(this.OnEveryShotAudioEvent))
			{
				AkSoundEngine.PostEvent(this.OnEveryShotAudioEvent, base.gameObject);
			}
			for (int i = 0; i < volley.projectiles.Count; i++)
			{
				ProjectileModule projectileModule = volley.projectiles[i];
				Projectile currentProjectile = projectileModule.GetCurrentProjectile();
				if (currentProjectile)
				{
					float angleForShot = projectileModule.GetAngleForShot(1f, 1f, null);
					bool flag = currentProjectile.GetComponent<BeamController>() != null;
					if (!flag)
					{
						this.DoSingleProjectile(currentProjectile, source, targetAngle + angleForShot, overrideSpawnPoint, false);
					}
				}
			}
		}

		private void DoSingleProjectile(Projectile projectileToSpawn, PlayerController source, float targetAngle, Vector2? overrideSpawnPoint, bool doAudio = false)
		{
			if (doAudio && !string.IsNullOrEmpty(this.OnEveryShotAudioEvent))
			{
				AkSoundEngine.PostEvent(this.OnEveryShotAudioEvent, base.gameObject);
			}
			if (this.ConsumesTargetGunAmmo && this.m_targetGun && this.m_owner.inventory.AllGuns.Contains(this.m_targetGun))
			{
				if (this.m_targetGun.ammo == 0)
				{
					return;
				}
				if (UnityEngine.Random.value < this.ChanceToConsumeTargetGunAmmo)
				{
					this.m_targetGun.LoseAmmo(1);
				}
			}
			if (projectileToSpawn != null) { projectileToSpawn.baseData.damage *= DamageMultiplier; }

			Vector2 v = (overrideSpawnPoint == null) ? source.specRigidbody.UnitCenter : overrideSpawnPoint.Value;
			GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, v, Quaternion.Euler(0f, 0f, targetAngle), true);
			Projectile component = gameObject.GetComponent<Projectile>();
			component.Owner = source;
			component.Shooter = source.specRigidbody;
			RecursionPreventer recursion = gameObject.AddComponent<RecursionPreventer>();
			recursion.IsProjectileFiredFromHoveringGun = true; 
			
			source.DoPostProcessProjectile(component);


			BounceProjModifier component2 = component.GetComponent<BounceProjModifier>();
			if (component2)
			{
				component2.numberOfBounces = Mathf.Min(3, component2.numberOfBounces);
			}
		}

		private BeamController BeginFiringBeam(Projectile projectileToSpawn, PlayerController source, float targetAngle, Vector2? overrideSpawnPoint)
		{
			Vector2 vector = (overrideSpawnPoint == null) ? source.CenterPosition : overrideSpawnPoint.Value;
			GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, vector, Quaternion.identity, true);
			Projectile component = gameObject.GetComponent<Projectile>();
			component.Owner = source;
			BeamController component2 = gameObject.GetComponent<BeamController>();
			component2.Owner = source;
			component2.HitsPlayers = false;
			component2.HitsEnemies = true;
			Vector3 v = BraveMathCollege.DegreesToVector(targetAngle, 1f);
			component2.Direction = v;
			component2.Origin = vector;
			return component2;
		}

		private void ContinueFiringBeam(BeamController beam, PlayerController source, float angle, Vector2? overrideSpawnPoint)
		{
			Vector2 vector = (overrideSpawnPoint == null) ? source.CenterPosition : overrideSpawnPoint.Value;
			beam.Direction = BraveMathCollege.DegreesToVector(angle, 1f);
			beam.Origin = vector;
			beam.LateUpdatePosition(vector);
		}

		private void CeaseBeam(BeamController beam)
		{
			beam.CeaseAttack();
		}

		protected override void OnDestroy()
		{
			if (this.m_owner)
			{
				this.m_owner.OnDodgedProjectile -= this.HandleDodgedProjectileFire;
			}
			if (this.m_owner)
			{
				this.m_owner.PostProcessProjectile -= this.HandleFiredGun;
			}
			if (!string.IsNullOrEmpty(this.FinishedShootingAudioEvent))
			{
				AkSoundEngine.PostEvent(this.FinishedShootingAudioEvent, base.gameObject);
			}
			if (this.Position == CustomHoveringGunController.HoverPosition.CIRCULATE)
			{
				for (int i = 0; i < this.m_owner.orbitals.Count; i++)
				{
					if (this.m_owner.orbitals[i].GetOrbitalTier() == this.GetOrbitalTier() && this.m_owner.orbitals[i].GetOrbitalTierIndex() > this.GetOrbitalTierIndex())
					{
						this.m_owner.orbitals[i].SetOrbitalTierIndex(this.m_owner.orbitals[i].GetOrbitalTierIndex() - 1);
					}
				}
				this.m_owner.orbitals.Remove(this);
			}
			LootEngine.DoDefaultSynergyPoof(base.sprite.WorldCenter, false);
		}

		public void Reinitialize()
		{
			if (base.specRigidbody)
			{
				base.specRigidbody.Reinitialize();
			}
			this.m_ownerCenterAverage = attachObject != null ? attachObject.transform.PositionVector2() : this.m_owner.CenterPosition;
		}

		public Transform GetTransform()
		{
			return this.m_parentTransform;
		}

		public void ToggleRenderer(bool visible)
		{
			base.sprite.renderer.enabled = visible;
		}

		public int GetOrbitalTier()
		{
			return this.m_orbitalTier;
		}

		public void SetOrbitalTier(int tier)
		{
			this.m_orbitalTier = tier;
		}

		public int GetOrbitalTierIndex()
		{
			return this.m_orbitalTierIndex;
		}

		public void SetOrbitalTierIndex(int tierIndex)
		{
			this.m_orbitalTierIndex = tierIndex;
		}

		public float GetOrbitalRadius()
		{
			return Radius;
		}

		public float GetOrbitalRotationalSpeed()
		{
			return RotationSpeed;
		}


		public GameObject attachObject;
		public float Radius;
		public float RotationSpeed;
		public float DamageMultiplier;
		public float MagnitudePower;


		public CustomHoveringGunController.HoverPosition Position;

		public CustomHoveringGunController.FireType Trigger;

		public CustomHoveringGunController.AimType Aim;

		public float AimRotationAngularSpeed;

		public float ShootDuration;

		public float CooldownTime;

		public bool OnlyOnEmptyReload;

		public bool ConsumesTargetGunAmmo;

		public float ChanceToConsumeTargetGunAmmo;

		public string ShootAudioEvent;

		public string OnEveryShotAudioEvent;

		public string FinishedShootingAudioEvent;

		private bool m_initialized;

		private Transform m_parentTransform;

		private Transform m_shootPointTransform;

		private Gun m_targetGun;

		public PlayerController m_owner;

		private float m_currentAimTarget;

		private bool m_hasEnemyTarget;

		private float m_fireCooldown;

		private Vector2 m_ownerCenterAverage;

		private float m_orbitalAngle;

		private int m_orbitalTier;

		private int m_orbitalTierIndex;

		public enum HoverPosition
		{
			OVERHEAD,
			CIRCULATE,
			ROTATE_AROUND_POSITION
		}

		public enum FireType
		{
			ON_RELOAD,
			ON_COOLDOWN,
			ON_DODGED_BULLET,
			ON_FIRED_GUN
		}

		public enum AimType
		{
			NEAREST_ENEMY,
			PLAYER_AIM
		}
	}

}

