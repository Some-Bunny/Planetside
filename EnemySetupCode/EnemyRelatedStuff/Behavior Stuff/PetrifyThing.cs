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
using Pathfinding;


namespace Planetside
{
	public class PetrifyThing : BraveBehaviour
	{
		public PetrifyThing()
        {
			this.Time = 10f;
		}

		public bool HasTriggered;
		public void Start()
		{
			base.healthHaver.OnPreDeath += this.OnPreDeath;
		}
		private void OnPreDeath(Vector2 obj)
		{
			AkSoundEngine.PostEvent("Play_BOSS_wall_slam_01", base.gameObject);
			GameObject dragunBoulder = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyBoulder;
			foreach (Component item in dragunBoulder.GetComponentsInChildren(typeof(Component)))
			{
				if (item is SkyRocket laser)
				{
					ExplosionData explosionData = laser.ExplosionData;	
					GameObject boom = UnityEngine.Object.Instantiate<GameObject>(explosionData.effect, base.transform.position, Quaternion.identity);
					Destroy(boom, 2);
					foreach (Component item2 in UnityEngine.Object.Instantiate<GameObject>(laser.SpawnObject, base.transform.position, Quaternion.identity).GetComponentsInChildren(typeof(Component)))
					{
						if (item2 is DraGunBoulderController laser2)
						{
							float MaxHP = base.aiActor.healthHaver.GetMaxHealth();
							if (base.aiActor.IsBlackPhantom == true) { MaxHP *= .33f; }
							laser2.LifeTime = base.aiActor != null ? ((MaxHP / 75f)* Time) +1: Time;
							float ZoneSize = (MaxHP / 50)+0.25f;
							GameManager.Instance.Dungeon.StartCoroutine(this.IncreaseInSize(laser2.CircleSprite, ZoneSize >= 2 ? 2 : ZoneSize));
							SpeculativeRigidbody body = laser2.GetComponentInChildren<SpeculativeRigidbody>();
							if (body) 
							{
								List <PixelCollider> colliders = body.PixelColliders.ToList();
								foreach (PixelCollider collider in colliders)
								{
									collider.ManualOffsetX = (int)(collider.ManualOffsetX * (ZoneSize >= 2 ? 2 : ZoneSize));
									collider.ManualOffsetY = (int)(collider.ManualOffsetY * (ZoneSize >= 2 ? 2 : ZoneSize));
									collider.ManualDiameter = (int)(collider.ManualDiameter * (ZoneSize >= 2 ? 2 : ZoneSize));
									collider.ManualHeight = (int)(collider.ManualHeight * (ZoneSize >= 2 ? 2 : ZoneSize));
								}
							}
						}
					}
				}
			}
		}
		private IEnumerator IncreaseInSize(tk2dSprite CircleSprite, float SizeMultiplier=1)
		{
			float elapsed = 0f;
			float duration = 0.75f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
				if (CircleSprite != null && CircleSprite.gameObject != null)
				{
					CircleSprite.scale = Vector3.Lerp(Vector3.zero, Vector3.one * SizeMultiplier, t);
				}
				yield return null;
			}
			yield break;
		}

		public float Time;
		public float minimumHealth;
		public float CheatDeath = 2f;
	}
}