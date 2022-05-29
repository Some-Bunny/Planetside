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
	public class LDCBullets : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Teleporting Gunfire";
			string resourceName = "Planetside/Resources/telecast.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<LDCBullets>();
			ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
			string shortDesc = "I cast gun!";
			string longDesc = "These bullets create a copy of themselves from the nearest enemy." +
				"\n\nOriginally used by an alchemist seeking the Gun who is now long gone, probably due to accidental backfire.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalShotPiercing, 1f, StatModifier.ModifyMethod.ADDITIVE);
			item.quality = PickupObject.ItemQuality.S;
			item.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);

			LDCBullets.TeleportingGunfireID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

		}
		public static int TeleportingGunfireID;
		private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
		{
			PlayerController owner = base.Owner;
			try
			{
				if (sourceProjectile.Owner = owner)
				{
					float num2 = 10f;
					{
						bool isInCombat = owner.IsInCombat;
						if (isInCombat)
						{
							{
								List<AIActor> activeEnemies = owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
								bool flag5 = activeEnemies == null | activeEnemies.Count <= 0;
								{
									AIActor nearestEnemy = this.GetNearestEnemy(activeEnemies, owner.sprite.WorldCenter, out num2, null, 8);
									bool flag8 = nearestEnemy && nearestEnemy != null;
									if (flag8)
									{
										Vector2 unitCenter3 = nearestEnemy.specRigidbody.HitboxPixelCollider.UnitCenter;
										this.DoBurst1(sourceProjectile, owner, unitCenter3, null);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
			}
		}
		private void PostProcessBeam(BeamController beam)
		{
			PlayerController owner = base.Owner;
			beam.transform.position = owner.unadjustedAimPoint;
		}
		private void PostProcessBeamTick(BeamController beam, SpeculativeRigidbody hitRigidbody, float tickRate)
		{
			PlayerController owner = base.Owner;
			beam.transform.position = owner.unadjustedAimPoint;
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			player.PostProcessProjectile -= this.PostProcessProjectile;
			return result;
		}
		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
		}

		protected override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
			}
			base.OnDestroy();
		}
		public AIActor GetNearestEnemy(List<AIActor> activeEnemies, Vector2 position, out float nearestDistance, string[] filter, float range)
		{
			AIActor aiactor = null;
			nearestDistance = range;
			AIActor result;
			if (activeEnemies == null || activeEnemies.Count == 0)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < activeEnemies.Count; i++)
				{
					AIActor aiactor2 = activeEnemies[i];
					if (!aiactor2.healthHaver.IsDead && aiactor2.healthHaver.IsVulnerable)
					{
						if (filter == null || !filter.Contains(aiactor2.EnemyGuid))
						{
							float num = Vector2.Distance(position, aiactor2.CenterPosition);
							if (num < nearestDistance)
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



		public void DoBurst1(Projectile bullet, PlayerController source, Vector2? overrideSpawnPoint = null, Vector2? spawnPointOffset = null)
		{
			this.ImmediateBurst(bullet, source, overrideSpawnPoint, spawnPointOffset);

		}

		private AIActor GetNearestEnemy(Vector2 sourcePoint)
		{
			RoomHandler absoluteRoom = sourcePoint.GetAbsoluteRoom();
			float num = 0f;
			return absoluteRoom.GetNearestEnemy(sourcePoint, out num, true, true);
		}

		private void ImmediateBurst(Projectile projectileToSpawn, PlayerController source, Vector2? overrideSpawnPoint, Vector2? spawnPointOffset = null)
		{
			if (projectileToSpawn == null)
			{
				return;
			}
			int num = UnityEngine.Random.Range(this.MinToSpawnPerWave, this.MaxToSpawnPerWave);
			int radialBurstLimit = projectileToSpawn.GetRadialBurstLimit(source);
			if (radialBurstLimit < num)
			{
				num = radialBurstLimit;
			}
			float num3 = source.CurrentGun.CurrentAngle;
			bool flag = projectileToSpawn.GetComponent<BeamController>() != null;
			for (int i = 0; i < num; i++)
			{
				float targetAngle = num3;
				if (flag)
				{
					source.StartCoroutine(this.HandleFireShortBeam(projectileToSpawn, source, UnityEngine.Random.Range(-180, 180), 1f * (float)this.NumberWaves, overrideSpawnPoint, spawnPointOffset));
				}
				else
				{
					this.DoSingleProjectile(projectileToSpawn, source, UnityEngine.Random.Range(-180, 180), overrideSpawnPoint, spawnPointOffset);
				}
			}
		}

		// Token: 0x06007626 RID: 30246 RVA: 0x002F051C File Offset: 0x002EE71C
		private IEnumerator HandleBurst(Projectile projectileToSpawn, PlayerController source, Vector2? overrideSpawnPoint, Vector2? spawnPointOffset = null)
		{
			if (projectileToSpawn == null)
			{
				yield break;
			}
			bool projectileIsBeam = projectileToSpawn.GetComponent<BeamController>() != null;
			bool projectileExplodes = projectileToSpawn.GetComponent<ExplosiveModifier>() != null;
			bool projectileSpawns = projectileToSpawn.GetComponent<SpawnProjModifier>() != null;
			bool reducedCountProjectile = projectileToSpawn.GetComponent<BlackHoleDoer>() != null;
			int limit = projectileToSpawn.GetRadialBurstLimit(source);
			int modWaves = this.NumberWaves;
			if (projectileIsBeam)
			{
				modWaves = 1;
			}
			if (projectileExplodes)
			{
				modWaves = 1;
			}
			if (projectileSpawns)
			{
				modWaves = 1;
			}
			if (reducedCountProjectile)
			{
				modWaves = 1;
			}
			if (limit > 0 && limit < 1000)
			{
				modWaves = 1;
			}
			int modSubwaves = Mathf.Max(1, this.NumberSubwaves);
			for (int w = 0; w < modWaves; w++)
			{
				int numToSpawn = UnityEngine.Random.Range(this.MinToSpawnPerWave, this.MaxToSpawnPerWave);
				if (limit < numToSpawn)
				{
					numToSpawn = limit;
				}
				if (reducedCountProjectile)
				{
					numToSpawn = 3;
				}
				float angle = 360f / (float)numToSpawn; ;
				float spiralDelay = this.TimeBetweenWaves / (float)numToSpawn;
				if (this.AlignFirstShot && source && source.CurrentGun)
				{
					angle = source.CurrentGun.CurrentAngle;
				}
				for (int i = 0; i < numToSpawn; i++)
				{
					for (int j = 0; j < modSubwaves; j++)
					{
						float targetAngle = angle;
						if (projectileIsBeam)
						{
							source.StartCoroutine(this.HandleFireShortBeam(projectileToSpawn, source, targetAngle, 1f, overrideSpawnPoint, spawnPointOffset));
						}
						else
						{
							this.DoSingleProjectile(projectileToSpawn, source, targetAngle, overrideSpawnPoint, spawnPointOffset);
						}
					}
					if (this.SpiralWaves)
					{
						yield return new WaitForSeconds(spiralDelay);
					}
				}
				if (!this.SpiralWaves)
				{
					yield return new WaitForSeconds(this.TimeBetweenWaves);
				}
			}
			yield break;
		}

		// Token: 0x06007627 RID: 30247 RVA: 0x002F0554 File Offset: 0x002EE754
		private IEnumerator HandleFireShortBeam(Projectile projectileToSpawn, PlayerController source, float targetAngle, float duration, Vector2? overrideSpawnPoint, Vector2? spawnPointOffset = null)
		{
			float elapsed = 0f;
			BeamController beam = this.BeginFiringBeam(projectileToSpawn, source, targetAngle, overrideSpawnPoint, spawnPointOffset);
			yield return null;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				if (this.SweepBeams)
				{
					beam.Direction = Quaternion.Euler(0f, 0f, BraveTime.DeltaTime / duration * this.BeamSweepDegrees) * beam.Direction;
				}
				this.ContinueFiringBeam(beam, source, overrideSpawnPoint, spawnPointOffset);
				yield return null;
			}
			this.CeaseBeam(beam);
			yield break;
		}

		// Token: 0x06007628 RID: 30248 RVA: 0x002F059C File Offset: 0x002EE79C
		private void DoSingleProjectile(Projectile projectileToSpawn, PlayerController source, float targetAngle, Vector2? overrideSpawnPoint, Vector2? spawnPointOffset = null)
		{
			Vector2 vector = (overrideSpawnPoint == null) ? source.specRigidbody.UnitCenter : overrideSpawnPoint.Value;
			vector = ((spawnPointOffset == null) ? vector : (vector + spawnPointOffset.Value));
			GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, vector, Quaternion.Euler(0f, 0f, targetAngle), true);
			Projectile component = gameObject.GetComponent<Projectile>();
			component.Owner = source;
			component.Shooter = source.specRigidbody;
			//source.DoPostProcessProjectile(component);
			if (this.CustomPostProcessProjectile != null)
			{
				this.CustomPostProcessProjectile(component);
			}
			this.InternalPostProcessProjectile(component);
		}

		private void InternalPostProcessProjectile(Projectile proj)
		{
			if (proj && !this.ForceAllowGoop)
			{
				GoopModifier component = proj.GetComponent<GoopModifier>();
				if (component)
				{
					UnityEngine.Object.Destroy(component);
				}
			}
			if (this.FixOverlapCollision && proj && proj.specRigidbody)
			{
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(proj.specRigidbody, null, false);
			}
		}

		// Token: 0x0600762A RID: 30250 RVA: 0x002F06CC File Offset: 0x002EE8CC
		private BeamController BeginFiringBeam(Projectile projectileToSpawn, PlayerController source, float targetAngle, Vector2? overrideSpawnPoint, Vector2? spawnPointOffset = null)
		{
			Vector2 vector = (overrideSpawnPoint == null) ? source.CenterPosition : overrideSpawnPoint.Value;
			vector = ((spawnPointOffset == null) ? vector : (vector + spawnPointOffset.Value));
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
			this.InternalPostProcessProjectile(component);
			return component2;
		}

		// Token: 0x0600762B RID: 30251 RVA: 0x002F0788 File Offset: 0x002EE988
		private void ContinueFiringBeam(BeamController beam, PlayerController source, Vector2? overrideSpawnPoint, Vector2? spawnPointOffset = null)
		{
			Vector2 vector = (overrideSpawnPoint == null) ? source.CenterPosition : overrideSpawnPoint.Value;
			vector = ((spawnPointOffset == null) ? vector : (vector + spawnPointOffset.Value));
			beam.Origin = vector;
			beam.LateUpdatePosition(vector);
		}

		// Token: 0x0600762C RID: 30252 RVA: 0x002F07E8 File Offset: 0x002EE9E8
		private void CeaseBeam(BeamController beam)
		{
			beam.CeaseAttack();
		}

		// Token: 0x040077ED RID: 30701
		public PlayerItemProjectileInterface ProjectileInterface;

		// Token: 0x040077EE RID: 30702
		public int MinToSpawnPerWave = 1;

		// Token: 0x040077EF RID: 30703
		public int MaxToSpawnPerWave = 1;

		// Token: 0x040077F0 RID: 30704
		public int NumberWaves = 1;

		// Token: 0x040077F1 RID: 30705
		public int NumberSubwaves = 1;

		// Token: 0x040077F2 RID: 30706
		public float TimeBetweenWaves = 1f;

		// Token: 0x040077F3 RID: 30707
		public bool SpiralWaves;

		// Token: 0x040077F4 RID: 30708
		public bool AlignFirstShot;

		// Token: 0x040077F5 RID: 30709
		public float AlignOffset;

		// Token: 0x040077F6 RID: 30710
		public bool SweepBeams;

		// Token: 0x040077F7 RID: 30711
		public float BeamSweepDegrees = 360f;

		// Token: 0x040077F8 RID: 30712
		public bool AimFirstAtNearestEnemy;

		// Token: 0x040077F9 RID: 30713
		public bool FixOverlapCollision;

		// Token: 0x040077FA RID: 30714
		public bool ForceAllowGoop;

		// Token: 0x040077FB RID: 30715
		public Action<Projectile> CustomPostProcessProjectile;

	}
}