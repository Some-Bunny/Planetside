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


namespace Planetside
{
    public class GunPrinter : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Gun-Printer";
            string resourceName = "Planetside/Resources/portablegunprinter.png";
            GameObject obj = new GameObject(itemName);
            GunPrinter activeitem = obj.AddComponent<GunPrinter>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Infinite Ammo?";
            string longDesc = "A small machine that prints out and fires a shot from the currently held gun, free of charge!\n\nA failed attempt at the creation of limitless ammunition, the energy required to power one of these exceeds just manufacturing more bullets.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 300f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.C;
            activeitem.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);
			GunPrinter.GunPrinterID = activeitem.PickupObjectId;
			ItemIDs.AddToList(activeitem.PickupObjectId);
		}

		public static int GunPrinterID;
		public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        protected override void DoEffect(PlayerController user)
        {
			Gun gun = user.CurrentGun;
			Vector2 AAAA =  new Vector2(user.CurrentGun.barrelOffset.position.x, user.CurrentGun.barrelOffset.position.y);
            AkSoundEngine.PostEvent("Play_ENM_kali_pop_01", base.gameObject);
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
				this.DoBurst1(user, AAAA, null);
			}
		}	
		public void DoBurst1(PlayerController source, Vector2? overrideSpawnPoint = null, Vector2? spawnPointOffset = null)
		{
			this.ImmediateBurst(this.ProjectileInterface.GetProjectile(source), source, overrideSpawnPoint, spawnPointOffset);

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
					source.StartCoroutine(this.HandleFireShortBeam(projectileToSpawn, source, targetAngle, 1f * (float)this.NumberWaves, overrideSpawnPoint, spawnPointOffset));
				}
				else
				{
					this.DoSingleProjectile(projectileToSpawn, source, targetAngle, overrideSpawnPoint, spawnPointOffset);
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
				float angle = 360f / (float)numToSpawn;;
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
			GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, vector, Quaternion.Euler(0f, 0f, targetAngle+UnityEngine.Random.Range(-10,10)), true);
			Projectile component = gameObject.GetComponent<Projectile>();
			component.Owner = source;
			component.Shooter = source.specRigidbody;
			source.DoPostProcessProjectile(component);
			if (this.CustomPostProcessProjectile != null)
			{
				this.CustomPostProcessProjectile(component);
			}
			this.InternalPostProcessProjectile(component);
		}

		// Token: 0x06007629 RID: 30249 RVA: 0x002F0650 File Offset: 0x002EE850
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



