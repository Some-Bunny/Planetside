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

public class CoalletController : BraveBehaviour
{
	// Token: 0x06005D0A RID: 23818 RVA: 0x0023A27C File Offset: 0x0023847C
	public void Start()
	{
		base.healthHaver.OnDamaged += this.OnDamaged;
		base.healthHaver.OnPreDeath += this.OnPreDeath;
		m_StartRoom = aiActor.GetAbsoluteParentRoom();

	}
	private RoomHandler m_StartRoom;

	public void Update()
	{
		m_StartRoom = aiActor.GetAbsoluteParentRoom();
		if (!base.aiActor.HasBeenEngaged)
		{
			CheckPlayerRoom();
		}
	}
	private void CheckPlayerRoom()
	{
		if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
		{
			GameManager.Instance.StartCoroutine(LateEngage());
		}
		else
		{
			base.aiActor.HasBeenEngaged = false;
		}
	}
	private IEnumerator LateEngage()
	{
		yield return new WaitForSeconds(0.5f);
		if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom && base.aiActor.HasBeenEngaged == false)
		{
			base.StartCoroutine(SelfIgnite());
			base.aiActor.HasBeenEngaged = true;
		}
		yield break;
	}

	protected override void OnDestroy()
	{
		if (base.healthHaver)
		{
			base.healthHaver.OnDamaged -= this.OnDamaged;
			base.healthHaver.OnPreDeath -= this.OnPreDeath;
		}
		base.OnDestroy();
	}

	private IEnumerator SelfIgnite()
    {
		float RNG = UnityEngine.Random.Range(4, 20);
		ETGModConsole.Log(RNG.ToString());
		yield return new WaitForSeconds(RNG);
		if (base.healthHaver != null)
		{
			this.FIREFIREAAAAAAAAAA();
			base.healthHaver.OnDamaged -= this.OnDamaged;
			base.aiActor.MovementSpeed = 4;
		}
		yield break;
	}
	private void OnPreDeath(Vector2 obj)
	{
		//AkSoundEngine.PostEvent("Play_TRP_flame_torch_01", base.aiActor.gameObject);
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
		BulletStatusEffectItem Firecomponent = PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>();
		GameActorFireEffect gameActorFire = Firecomponent.FireModifierEffect;
		base.aiActor.ApplyEffect(gameActorFire, 1000000f, null);
		base.healthHaver.ApplyDamageModifiers(this.onFireDamageTypeModifiers);

		for(int i = 0; i < base.behaviorSpeculator.MovementBehaviors.Count; i++)
			{
			if (base.behaviorSpeculator.MovementBehaviors[i] is MoveErraticallyBehavior)
			{
				MoveErraticallyBehavior moveErraticallyBehavior = base.aiActor.behaviorSpeculator.MovementBehaviors[i] as MoveErraticallyBehavior;
				moveErraticallyBehavior.PointReachedPauseTime = 0;
				//moveErraticallyBehavior.ResetPauseTimer();
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
		//base.behaviorSpeculator.AttackCooldown = 0f;
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

	[FormerlySerializedAs("fireEffect2")]
	public GameActorFireEffect fireEffect;


	public float overrideMoveSpeed = -1f;

	public float overridePauseTime = -1f;

	public string overrideAnimation;

	public List<DamageTypeModifier> onFireDamageTypeModifiers;
}
