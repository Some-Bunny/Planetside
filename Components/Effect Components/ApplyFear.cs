using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;

using UnityEngine.Serialization;



namespace Planetside
{
	public class ApplyFear : MonoBehaviour
	{
		public ApplyFear()
		{
			this.chanceToslow = 1f;
		}

		private void Start()
		{
			this.m_projectile = base.GetComponent<Projectile>();
			bool flag = UnityEngine.Random.value <= this.chanceToslow;
			if (flag)
			{
				Projectile projectile = this.m_projectile;
				projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.ApplyStun));
			}
		}
		private static FleePlayerData fleeData;
		private void ApplyStun(Projectile bullet, SpeculativeRigidbody enemy, bool fatal)
		{
			bool r = enemy.behaviorSpeculator != null;
			if (r)
			{
				ApplyFear.fleeData = new FleePlayerData();
				ApplyFear.fleeData.Player = bullet.Owner as PlayerController;
				ApplyFear.fleeData.StartDistance = 100f;
				enemy.behaviorSpeculator.FleePlayerData = ApplyFear.fleeData;
				FleePlayerData fleePlayerData = new FleePlayerData();
				GameManager.Instance.StartCoroutine(ApplyFear.scare(enemy.aiActor));
			}
		}
		private static IEnumerator scare(AIActor aiactor)
		{
			yield return new WaitForSeconds(5f);
			aiactor.behaviorSpeculator.FleePlayerData = null;
			yield break;
		}
		private Projectile m_projectile;
		public float chanceToslow;
		public float SpeedMultiplier;

		public bool doTint;
		public Color Color;
		public float slowLength;
	}
}