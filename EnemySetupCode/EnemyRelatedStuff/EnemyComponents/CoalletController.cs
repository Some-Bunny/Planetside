using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using Dungeonator;
using Gungeon;
using ItemAPI;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Brave.BulletScript;
using GungeonAPI;
using SpriteBuilder = ItemAPI.SpriteBuilder;
using DirectionType = DirectionalAnimation.DirectionType;
using static DirectionalAnimation;
using Planetside;

public class CoalletController : BraveBehaviour
{
	public void Start()
	{
		base.healthHaver.OnDamaged += this.OnDamaged;
		m_StartRoom = aiActor.GetAbsoluteParentRoom();

	}
	private RoomHandler m_StartRoom;

	public void Update()
	{
		m_StartRoom = aiActor.GetAbsoluteParentRoom();
	}


	private IEnumerator SelfIgnite()
    {
		float RNG = UnityEngine.Random.Range(6, 20);
		yield return new WaitForSeconds(RNG);
		if (base.healthHaver != null)
		{
			this.FIREFIREAAAAAAAAAA();
			base.healthHaver.OnDamaged -= this.OnDamaged;
			base.aiActor.MovementSpeed = 4;
		}
		yield break;
	}

	private void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
	{
		if ((damageTypes & CoreDamageTypes.Water) == CoreDamageTypes.Water)
		{
			return;
		}
		if ((damageTypes & CoreDamageTypes.Ice) == CoreDamageTypes.Ice)
		{
			return;
		}
		if (base.healthHaver)
		{
			this.FIREFIREAAAAAAAAAA();
			base.healthHaver.OnDamaged -= this.OnDamaged;
			base.aiActor.MovementSpeed = 4;
		}
	}


	private void FIREFIREAAAAAAAAAA()
	{
		GameActorFireEffect fire = new GameActorFireEffect();
		fire.CopyEffectFrom(DebuffStatics.hotLeadEffect);
		fire.duration = 100000;
		fire.DamagePerSecondToEnemies = 2;


        base.aiActor.ApplyEffect(fire, 1, null);
		base.healthHaver.ApplyDamageModifiers(this.onFireDamageTypeModifiers);

		for(int i = 0; i < base.behaviorSpeculator.MovementBehaviors.Count; i++)
		{
			if (base.behaviorSpeculator.MovementBehaviors[i] is MoveErraticallyBehavior)
			{
				MoveErraticallyBehavior moveErraticallyBehavior = base.aiActor.behaviorSpeculator.MovementBehaviors[i] as MoveErraticallyBehavior;
				moveErraticallyBehavior.PointReachedPauseTime = 0;
			}
		}
		base.aiActor.ClearPath();
		base.aiAnimator.EndAnimation();
		base.aiAnimator.OverrideIdleAnimation = "runfire";
		base.aiAnimator.OverrideMoveAnimation = "runfire";
		for (int j = 0; j < base.aiActor.behaviorSpeculator.AttackBehaviors.Count; j++)
		{
			if (base.behaviorSpeculator.AttackBehaviors[j] is AttackBehaviorGroup && base.behaviorSpeculator.AttackBehaviors[j] != null)
			{
				this.ProcessAttackGroup(base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup);
			}
		}
	}

	private void ProcessAttackGroup(AttackBehaviorGroup attackGroup)
	{
		for (int i = 0; i < attackGroup.AttackBehaviors.Count; i++)
		{
			AttackBehaviorGroup.AttackGroupItem attackGroupItem = attackGroup.AttackBehaviors[i];
			if (attackGroupItem.Behavior is ShootBehavior && attackGroup != null && attackGroupItem.NickName == "Cry About It")
			{
				attackGroupItem.Probability = 5f;
			}
		}
	}


	public float overrideMoveSpeed = -1f;

	public float overridePauseTime = -1f;

	public string overrideAnimation;

	public List<DamageTypeModifier> onFireDamageTypeModifiers = new List<DamageTypeModifier>() 
	{
		new DamageTypeModifier()
		{
			damageType = CoreDamageTypes.Fire,
			damageMultiplier = 0.1f,
		},
		new DamageTypeModifier()
		{
			damageType = CoreDamageTypes.Ice,
			damageMultiplier = 25f,
		},
        new DamageTypeModifier()
        {
            damageType = CoreDamageTypes.Water,
            damageMultiplier = 25f,
        }
    };
}
