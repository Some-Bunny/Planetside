
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using ItemAPI;
using SaveAPI;

namespace Planetside
{
	// Token: 0x0200037A RID: 890
	public class MirrorProjectileModifier : MonoBehaviour
	{
		// Token: 0x06001204 RID: 4612 RVA: 0x000DA333 File Offset: 0x000D8533
		public MirrorProjectileModifier()
		{
			this.MirrorRadius = 3f;
		}

		// Token: 0x06001205 RID: 4613 RVA: 0x000DA348 File Offset: 0x000D8548
		private void Awake()
		{
			this.m_projectile = base.GetComponent<Projectile>();
			this.m_projectile.AdjustPlayerProjectileTint(Color.white, 2, 0f);
			this.m_projectile.collidesWithProjectiles = true;
			SpeculativeRigidbody specRigidbody = this.m_projectile.specRigidbody;
			specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x000DA3B4 File Offset: 0x000D85B4
		private void Update()
		{
			Vector2 b = this.m_projectile.transform.position.XY();
			for (int i = 0; i < StaticReferenceManager.AllProjectiles.Count; i++)
			{
				Projectile projectile = StaticReferenceManager.AllProjectiles[i];
				bool flag = projectile && projectile.Owner is AIActor;
				if (flag)
				{
					float sqrMagnitude = (projectile.transform.position.XY() - b).sqrMagnitude;
					bool flag2 = sqrMagnitude < this.MirrorRadius;
					if (flag2)
					{
					}
				}
			}
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x000DA454 File Offset: 0x000D8654
		private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			bool flag = otherRigidbody && otherRigidbody.projectile;
			if (flag)
			{
				bool flag2 = otherRigidbody.projectile.Owner is AIActor;
				if (flag2)
				{
					myRigidbody.projectile.DieInAir(false, true, true, false);
					FistComponent.FistReflectBullet(otherRigidbody.projectile, myRigidbody.projectile.Owner, otherRigidbody.projectile.Direction.ToAngle()+180, 10f, 1f, 1f, 0f);
				}
				PhysicsEngine.SkipCollision = true;
			}
		}

		// Token: 0x06001208 RID: 4616 RVA: 0x000DA4DC File Offset: 0x000D86DC
		public void ReflectBullet(Projectile p, bool retargetReflectedBullet, GameActor newOwner, float minReflectedBulletSpeed, float scaleModifier = 1f, float damageModifier = 1f, float spread = 0f)
		{
			p.RemoveBulletScriptControl();
			AkSoundEngine.PostEvent("Play_OBJ_metalskin_deflect_01", GameManager.Instance.gameObject);
			bool flag = retargetReflectedBullet && p.Owner && p.Owner.specRigidbody;
			bool flag2 = flag;
			if (flag2)
			{
				p.Direction = (p.Owner.specRigidbody.GetUnitCenter(ColliderType.HitBox) - p.specRigidbody.UnitCenter).normalized;
			}
			bool flag3 = spread != 0f;
			bool flag4 = flag3;
			if (flag4)
			{
				p.Direction = p.Direction.Rotate(UnityEngine.Random.Range(-spread, spread));
			}
			bool flag5 = p.Owner && p.Owner.specRigidbody;
			bool flag6 = flag5;
			if (flag6)
			{
				p.specRigidbody.DeregisterSpecificCollisionException(p.Owner.specRigidbody);
			}
			p.Owner = newOwner;
			p.SetNewShooter(newOwner.specRigidbody);
			p.allowSelfShooting = false;
			p.collidesWithPlayer = false;
			p.collidesWithEnemies = true;
			bool flag7 = scaleModifier != 1f;
			bool flag8 = flag7;
			if (flag8)
			{
				SpawnManager.PoolManager.Remove(p.transform);
				p.RuntimeUpdateScale(scaleModifier);
			}
			bool flag9 = p.Speed < minReflectedBulletSpeed;
			bool flag10 = flag9;
			if (flag10)
			{
				p.Speed = minReflectedBulletSpeed;
			}
			bool flag11 = p.baseData.damage < ProjectileData.FixedFallbackDamageToEnemies;
			bool flag12 = flag11;
			if (flag12)
			{
				p.baseData.damage = ProjectileData.FixedFallbackDamageToEnemies;
			}
			p.baseData.damage *= damageModifier;
			bool flag13 = p.baseData.damage < 10f;
			bool flag14 = flag13;
			if (flag14)
			{
				p.baseData.damage = 15f;
			}
			p.UpdateCollisionMask();
			p.Reflected();
			p.SendInDirection(p.Direction, true, true);
		}

		// Token: 0x04000B1D RID: 2845
		public float MirrorRadius;

		// Token: 0x04000B1E RID: 2846
		private Projectile m_projectile;
	}
}
