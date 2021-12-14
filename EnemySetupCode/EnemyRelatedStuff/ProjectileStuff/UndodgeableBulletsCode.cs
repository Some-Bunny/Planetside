using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;

namespace Planetside
{
	public class UndodgeableProjectile : Projectile
	{
		//	OtherTools.CopyFields<UndodgeableProjectile>(bulletObj.GetComponent<Projectile>());

		protected override void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
		{
			if (myRigidbody != null && myCollider != null && otherRigidbody != null && otherCollider != null)
            {
				if (otherRigidbody == Shooter && !allowSelfShooting)
				{
					PhysicsEngine.SkipCollision = true;
					return;
				}
				if (otherRigidbody.gameActor != null && otherRigidbody.gameActor is PlayerController && (!collidesWithPlayer || (otherRigidbody.gameActor as PlayerController).IsGhost || (otherRigidbody.gameActor as PlayerController).IsEthereal))
				{
					PhysicsEngine.SkipCollision = true;
					return;
				}
				if (otherRigidbody.aiActor)
				{
					if (Owner is PlayerController && !otherRigidbody.aiActor.IsNormalEnemy)
					{
						PhysicsEngine.SkipCollision = true;
						return;
					}
					if (Owner is AIActor && !collidesWithEnemies && otherRigidbody.aiActor.IsNormalEnemy && !otherRigidbody.aiActor.HitByEnemyBullets)
					{
						PhysicsEngine.SkipCollision = true;
						return;
					}
				}
				if (!GameManager.PVP_ENABLED && Owner is PlayerController && otherRigidbody.GetComponent<PlayerController>() != null && !allowSelfShooting)
				{
					PhysicsEngine.SkipCollision = true;
					return;
				}
				if (GameManager.Instance.InTutorial)
				{
					PlayerController component = otherRigidbody.GetComponent<PlayerController>();
					if (component)
					{
						if (component.spriteAnimator.QueryInvulnerabilityFrame())
						{
							GameManager.BroadcastRoomTalkDoerFsmEvent("playerDodgedBullet");
						}
						else if (component.IsDodgeRolling)
						{
							GameManager.BroadcastRoomTalkDoerFsmEvent("playerAlmostDodgedBullet");
						}
						else
						{
							GameManager.BroadcastRoomTalkDoerFsmEvent("playerDidNotDodgeBullet");
						}
					}
				}
				if (collidesWithProjectiles && collidesOnlyWithPlayerProjectiles && otherRigidbody.projectile && !(otherRigidbody.projectile.Owner is PlayerController))
				{
					PhysicsEngine.SkipCollision = true;
					return;
				}
			}
			
		}

		protected override HandleDamageResult HandleDamage(SpeculativeRigidbody rigidbody, PixelCollider hitPixelCollider, out bool killedTarget, PlayerController player, bool alreadyPlayerDelayed = false)
		{
			killedTarget = false;
			if (rigidbody.ReflectProjectiles)
			{
				return HandleDamageResult.NO_HEALTH;
			}
			if (!rigidbody.healthHaver)
			{
				return HandleDamageResult.NO_HEALTH;
			}
			if (!alreadyPlayerDelayed && s_delayPlayerDamage && player)
			{
				return HandleDamageResult.HEALTH;
			}
			bool flag = !rigidbody.healthHaver.IsDead;
			float num = ModifiedDamage;
			if (Owner is AIActor && rigidbody && rigidbody.aiActor && (Owner as AIActor).IsNormalEnemy)
			{
				num = ProjectileData.FixedFallbackDamageToEnemies;
				if (rigidbody.aiActor.HitByEnemyBullets)
				{
					num /= 4f;
				}
			}
			int healthHaverHitCount = (int)OtherTools.ProjectileHealthHaverHitCountInfo.GetValue(this);
			if (Owner is PlayerController && m_hasPierced && healthHaverHitCount >= 1)
			{
				int num2 = Mathf.Clamp(healthHaverHitCount - 1, 0, GameManager.Instance.PierceDamageScaling.Length - 1);
				num *= GameManager.Instance.PierceDamageScaling[num2];
			}
			if (OnWillKillEnemy != null && num >= rigidbody.healthHaver.GetCurrentHealth())
			{
				OnWillKillEnemy(this, rigidbody);
			}
			if (rigidbody.healthHaver.IsBoss)
			{
				num *= BossDamageMultiplier;
			}
			if (BlackPhantomDamageMultiplier != 1f && rigidbody.aiActor && rigidbody.aiActor.IsBlackPhantom)
			{
				num *= BlackPhantomDamageMultiplier;
			}
			bool flag2 = false;
			if (DelayedDamageToExploders)
			{
				flag2 = (rigidbody.GetComponent<ExplodeOnDeath>() && rigidbody.healthHaver.GetCurrentHealth() <= num);
			}
			if (!flag2)
			{
				HealthHaver healthHaver = rigidbody.healthHaver;
				float damage = num;
				Vector2 velocity = specRigidbody.Velocity;
				string ownerName = OwnerName;
				CoreDamageTypes coreDamageTypes = damageTypes;
				DamageCategory damageCategory = (!IsBlackBullet) ? DamageCategory.Normal : DamageCategory.BlackBullet;
				healthHaver.ApplyDamage(damage, velocity, ownerName, coreDamageTypes, damageCategory, true, hitPixelCollider, ignoreDamageCaps);
				if (player && player.OnHitByProjectile != null)
				{
					player.OnHitByProjectile(this, player);
				}
			}
			else
			{
				rigidbody.StartCoroutine((IEnumerator)OtherTools.ProjectileHandleDelayedDamageInfo.Invoke(this, new object[] { rigidbody, num, specRigidbody.Velocity, hitPixelCollider }));
			}
			if (Owner && Owner is AIActor && player)
			{
				(Owner as AIActor).HasDamagedPlayer = true;
			}
			killedTarget = (flag && rigidbody.healthHaver.IsDead);
			if (!killedTarget && rigidbody.gameActor != null)
			{
				if (AppliesPoison && UnityEngine.Random.value < PoisonApplyChance)
				{
					rigidbody.gameActor.ApplyEffect(healthEffect, 1f, null);
				}
				if (AppliesSpeedModifier && UnityEngine.Random.value < SpeedApplyChance)
				{
					rigidbody.gameActor.ApplyEffect(speedEffect, 1f, null);
				}
				if (AppliesCharm && UnityEngine.Random.value < CharmApplyChance)
				{
					rigidbody.gameActor.ApplyEffect(charmEffect, 1f, null);
				}
				if (AppliesFreeze && UnityEngine.Random.value < FreezeApplyChance)
				{
					rigidbody.gameActor.ApplyEffect(freezeEffect, 1f, null);
				}
				if (AppliesCheese && UnityEngine.Random.value < CheeseApplyChance)
				{
					rigidbody.gameActor.ApplyEffect(cheeseEffect, 1f, null);
				}
				if (AppliesBleed && UnityEngine.Random.value < BleedApplyChance)
				{
					rigidbody.gameActor.ApplyEffect(bleedEffect, -1f, this);
				}
				if (AppliesFire && UnityEngine.Random.value < FireApplyChance)
				{
					rigidbody.gameActor.ApplyEffect(fireEffect, 1f, null);
				}
				if (AppliesStun && UnityEngine.Random.value < StunApplyChance && rigidbody.gameActor.behaviorSpeculator)
				{
					rigidbody.gameActor.behaviorSpeculator.Stun(AppliedStunDuration, true);
				}
				for (int i = 0; i < statusEffectsToApply.Count; i++)
				{
					rigidbody.gameActor.ApplyEffect(statusEffectsToApply[i], 1f, null);
				}
			}
			OtherTools.ProjectileHealthHaverHitCountInfo.SetValue(this, healthHaverHitCount + 1);
			return (!killedTarget) ? HandleDamageResult.HEALTH : HandleDamageResult.HEALTH_AND_KILLED;
		}
	}
}
