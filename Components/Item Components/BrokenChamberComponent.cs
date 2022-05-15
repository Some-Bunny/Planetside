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

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Brave;
using HutongGames.PlayMaker.Actions;
using InControl;
using Pathfinding;
using tk2dRuntime.TileMap;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Planetside
{
	public class BrokenChamberComponent : MonoBehaviour
	{
		public BrokenChamberComponent()
		{
			this.TimeBetweenRockFalls = 7.5f;
		}

		private void Awake()
		{
			this.m_player = base.GetComponent<PlayerController>();
		}

		private void Update()
		{
			Dungeon dung = GameManager.Instance.Dungeon;
			bool eiie = this.m_player == null;
			if (eiie)
			{
				this.m_player = GameManager.Instance.PrimaryPlayer;
			}
			if (this.m_player != null)
			{
				if (dung.LevelOverrideType == GameManager.LevelOverrideState.CHARACTER_PAST)
                {
					if (m_player.GetComponent<BrokenChamberComponent>() != null)
                    {
						Destroy(this);
					}
				}
				
				if (!GameManager.Instance.IsPaused | !GameManager.Instance.IsLoadingLevel)
				{
					this.elapsed += BraveTime.DeltaTime;
				}
				bool flag3 = this.elapsed > TimeBetweenRockFalls;
				if (flag3)
				{
					this.elapsed = 0f;
					if (this.m_player != null && GameManager.Instance.IsLoadingLevel == false)// && GameManager.Instance.Lev)
					{
						AkSoundEngine.PostEvent("Play_BOSS_wall_slam_01", base.gameObject);
						GameObject dragunBoulder = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyBoulder;
						//GameObject dragunRocket = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyRocket;
						IntVector2? vector = (m_player as PlayerController).CurrentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 2), CellTypes.FLOOR | CellTypes.PIT, false, null);
						Vector2 vector2 = vector.Value.ToVector2();
						this.FireRocket(dragunBoulder, m_player.sprite.WorldCenter);
						this.FireRocket(dragunBoulder, vector2);
					}
				}
			}
		}
		private void FireRocket(GameObject skyRocket, Vector2 target)
		{
			PlayerController player = m_player;
			SkyRocket component = SpawnManager.SpawnProjectile(skyRocket, player.sprite.WorldCenter, Quaternion.identity, true).GetComponent<SkyRocket>();
			component.TargetVector2 = target;
			tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
			component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
			component.ExplosionData.ignoreList.Add(player.specRigidbody);
		}
		private PlayerController m_player;

		public float TimeBetweenRockFalls;
		private float elapsed;

		
	}
}