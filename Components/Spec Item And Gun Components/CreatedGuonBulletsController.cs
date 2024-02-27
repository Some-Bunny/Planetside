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
	public class CreatedGuonBulletsController : MonoBehaviour
	{
		public CreatedGuonBulletsController()
		{
			this.SpawnsCharmGoop = false;
			this.ClearsGoop = false;
			this.ShootsOnDestruction = false;
			this.ChanceToBlank = false;
			this.AddSpeed = false;
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
			if (AddSpeed==true)
            {
				this.StartEffect(player);
			}
			LootEngine.DoDefaultItemPoof(actor.sprite.WorldCenter, false, true);
			actor.StartCoroutine(this.HandleTimedDestroy());
		}
		private void StartEffect(PlayerController user)
		{
			StatModifier item = new StatModifier
			{
				statToBoost = PlayerStats.StatType.MovementSpeed,
				amount = 0.175f,
				modifyType = StatModifier.ModifyMethod.ADDITIVE
			};
			this.cool = item;
			user.ownerlessStatModifiers.Add(item);
			user.stats.RecalculateStats(user, true, true);
		}
		private void EndEffect(PlayerController user)
		{
			user.ownerlessStatModifiers.Remove(this.cool);
			user.stats.RecalculateStats(user, true, true);
		}

		public void Update()
		{
			if (this.actor == null)
			{
				this.actor = base.GetComponent<PlayerOrbital>();
			}
			if (this.player == null)
			{
				this.player = base.GetComponent<PlayerController>();
			}
			if(ClearsGoop == true)
            {
				DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(this.actor.sprite.WorldCenter, 1.5f);
			}

		}

		public void NotifyDropped()
		{
			this.HandleRoomCleared();
		}
		public float random;

		private IEnumerator HandleTimedDestroy()
		{
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			yield return new WaitForSeconds(this.maxDuration);
			AkSoundEngine.PostEvent("Play_OBJ_cursepot_shatter_01", actor.gameObject);
			LootEngine.DoDefaultItemPoof(actor.sprite.WorldCenter, false, true);
			UnityEngine.Object.Destroy(base.gameObject);
			if (SpawnsCharmGoop == true)
            {
				DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.CharmGoopDef).TimedAddGoopCircle(this.actor.sprite.WorldCenter, 1.5f, 1f, false);
			}
			if (ChanceToBlank == true)
            {
				this.random = UnityEngine.Random.Range(0.0f, 1.0f);
				if (random <= 0.05f)
				{
					this.DoMicroBlank(this.actor.sprite.WorldCenter, 0f);
				}
			}
			if (AddSpeed == true)
			{
				this.EndEffect(player);
			}

			if (ShootsOnDestruction == true)
			{
				bool flag3 = player.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All);
				if (flag3)
				{
					bool flag4 = this.actor && this.actor != null;
					if (flag4)
					{
						float num2 = 10f;
						List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
						bool flag5 = activeEnemies == null | activeEnemies.Count <= 0;
						bool flag6 = !flag5;
						bool flag7 = flag6;
						if (flag7)
						{
							AIActor nearestEnemy = this.GetNearestEnemy(activeEnemies, this.actor.sprite.WorldCenter, out num2, null);
							bool flag8 = nearestEnemy && nearestEnemy != null;
							if (flag8)
							{
								{
									float dmg = (player.stats.GetStatValue(PlayerStats.StatType.Damage));

									Vector2 worldCenter3 = this.actor.sprite.WorldCenter;
									Vector2 unitCenter3 = nearestEnemy.specRigidbody.HitboxPixelCollider.UnitCenter;
									float z3 = BraveMathCollege.Atan2Degrees((unitCenter3 - worldCenter3).normalized);
									Projectile projectile3 = ((Gun)ETGMod.Databases.Items[43]).DefaultModule.projectiles[0];
									GameObject gameObject3 = SpawnManager.SpawnProjectile(projectile3.gameObject, worldCenter3, Quaternion.Euler(0f, 0f, z3), true);
									Projectile component3 = gameObject3.GetComponent<Projectile>();
									bool flag15 = component3 != null;
									bool flag16 = flag15;
									if (flag16)
									{
										component3.baseData.damage = 4f * dmg;
										component3.Owner = player;
										component3.baseData.range = 1000f;
										component3.pierceMinorBreakables = true;
										component3.collidesWithPlayer = false;
										component3.baseData.speed *= 0.5f;
										
										Material sharedMaterial = component3.sprite.renderer.sharedMaterial;
										component3.sprite.usesOverrideMaterial = true;
										Material material = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
										material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
										material.SetColor("_OverrideColor", new Color(1f, 1f, 1f, 1f));
										this.LerpMaterialGlow(material, 0f, 22f, 0.4f);
										material.SetFloat("_EmissiveColorPower", 8f);
										material.SetColor("_EmissiveColor", Color.red);
										SpriteOutlineManager.AddOutlineToSprite(component3.sprite, Color.red);
										component3.sprite.renderer.material = material;
										
									}
								}
							}
						}
					}
				}
			}
			yield break;
		}
		public void LerpMaterialGlow(Material targetMaterial, float startGlow, float targetGlow, float duration)
		{
			targetMaterial.SetFloat("_EmissivePower", Mathf.Lerp(startGlow, targetGlow, duration));

		}
		private void HandleRoomCleared()
		{
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			if (this.actor)
			{
				AkSoundEngine.PostEvent("Play_OBJ_cursepot_shatter_01", actor.gameObject);
				LootEngine.DoDefaultItemPoof(actor.sprite.WorldCenter, false, true);
				UnityEngine.Object.Destroy(base.gameObject);
				if (SpawnsCharmGoop == true)
				{
					DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.CharmGoopDef).TimedAddGoopCircle(this.actor.sprite.WorldCenter, 1.5f, 1f, false);
				}
				if (ChanceToBlank == true)
				{
					this.random = UnityEngine.Random.Range(0.0f, 1.0f);
					if (random <= 0.05f)
					{
						this.DoMicroBlank(this.actor.sprite.WorldCenter, 0f);
					}
				}
				if (AddSpeed == true)
				{
					this.EndEffect(player);
				}
			}
		}

		public AIActor GetNearestEnemy(List<AIActor> activeEnemies, Vector2 position, out float nearestDistance, string[] filter)
		{
			AIActor aiactor = null;
			nearestDistance = float.MaxValue;
			bool flag = activeEnemies == null;
			bool flag2 = flag;
			bool flag3 = flag2;
			bool flag4 = flag3;
			bool flag5 = flag4;
			AIActor result;
			if (flag5)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < activeEnemies.Count; i++)
				{
					AIActor aiactor2 = activeEnemies[i];
					bool flag6 = !aiactor2.healthHaver.IsDead && aiactor2.healthHaver.IsVulnerable;
					bool flag7 = flag6;
					if (flag7)
					{
						bool flag8 = filter == null || !filter.Contains(aiactor2.EnemyGuid);
						bool flag9 = flag8;
						if (flag9)
						{
							float num = Vector2.Distance(position, aiactor2.CenterPosition);
							bool flag10 = num < nearestDistance;
							bool flag11 = flag10;
							if (flag11)
							{
								nearestDistance = num;
								aiactor = aiactor2;
							}
						}
					}
				}
				result = aiactor;
			}
			return result;
		}
		private void DoMicroBlank(Vector2 center, float knockbackForce = 30f)
		{
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
			AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", base.gameObject);
			GameObject gameObject = new GameObject("silencer");
			SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
			float additionalTimeAtMaxRadius = 0.25f;
			silencerInstance.TriggerSilencer(center, 25f, 5f, silencerVFX, 0f, 3f, 3f, 3f, 250f, 5f, additionalTimeAtMaxRadius, player, false, false);
		}
		[NonSerialized]
		public PlayerController sourcePlayer;

		public float maxDuration = 10f;

		private PlayerOrbital actor;
		private PlayerController player;

		public bool ClearsGoop;
		public bool SpawnsCharmGoop;
		public bool ShootsOnDestruction;
		public bool ChanceToBlank;
		public bool AddSpeed;
		private StatModifier cool;
	}
}
