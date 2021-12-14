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
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using Brave.BulletScript;
using GungeonAPI;
using SpriteBuilder = ItemAPI.SpriteBuilder;
using DirectionType = DirectionalAnimation.DirectionType;
using static DirectionalAnimation;

namespace Planetside
{
	public class DamnedGuonComponent : MonoBehaviour
	{
		public DamnedGuonComponent()
		{

		}

		public void Awake()
		{
			this.actor = base.GetComponent<PlayerOrbital>();
			this.player = base.GetComponent<PlayerController>();

		}

		public void Start()
		{
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			if (this.actor == null)
            {
				this.actor = base.GetComponent<PlayerOrbital>();
			}
			if (this.player == null)
			{
				this.player = base.GetComponent<PlayerController>();
			}
		}
		private void Update()
		{
			bool flag2 = this.actor == null;
			if (flag2)
			{
				this.actor = base.GetComponent<PlayerOrbital>();
			}
			bool eiie = this.player == null;
			if (eiie)
			{
				this.player = base.GetComponent<PlayerController>();
			}
			this.elapsed += BraveTime.DeltaTime;
			bool flag3 = this.elapsed > 0.5;
			if (flag3)
			{
				if (this.actor != null)
				{
					bool flag = GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.HIGH && actor && actor.specRigidbody.HitboxPixelCollider != null;
					if (flag)
					{
						Vector2 unitBottomLeft = actor.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
						Vector2 unitTopRight = actor.specRigidbody.HitboxPixelCollider.UnitTopRight;
						this.m_emberCounter += 2f * BraveTime.DeltaTime;
						bool lol = this.m_emberCounter > 1f;
						if (lol)
						{
							int num = Mathf.FloorToInt(this.m_emberCounter);
							this.m_emberCounter -= (float)num;
							GlobalSparksDoer.DoRandomParticleBurst(num, unitBottomLeft, unitTopRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
						}
					}
				}
				this.elapsed = 0f;
			}
		}
		private float m_emberCounter;

		public float TimeBetweenRockFalls;
		private float elapsed;
		[NonSerialized]
		public PlayerController sourcePlayer;


		private PlayerOrbital actor;
		private PlayerController player;


	}
}
