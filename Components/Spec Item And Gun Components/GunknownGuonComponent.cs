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
	public class GunknownGuonComponent : MonoBehaviour
	{
		public GunknownGuonComponent()
		{
			this.maxDuration = 10;
			this.TimeBetweenFiring = 0.5f;
			this.player = GameManager.Instance.PrimaryPlayer;
			this.Hits = 0;
		}

		public void Awake()
		{
			this.actor = base.GetComponent<PlayerOrbital>();
			this.player = base.GetComponent<PlayerController>();

		}

		public void Start()
		{
			actor.sprite.usesOverrideMaterial = true;
			actor.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
			CanFire = false;
			Material mat = actor.sprite.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.mainTexture = actor.sprite.renderer.material.mainTexture;
			mat.SetColor("_EmissiveColor", new Color32(255, 177, 56, 255));
			mat.SetFloat("_EmissiveColorPower", 1.55f);
			mat.SetFloat("_EmissivePower", 100);
			actor.sprite.renderer.material = mat;
			GunknownGuonComponent.guonHook = new Hook(typeof(PlayerOrbital).GetMethod("Initialize"), typeof(GunknownGuonComponent).GetMethod("GuonInit"));
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			if (this.actor == null)
            {
				this.actor = base.GetComponent<PlayerOrbital>();
			}
			if (this.player == null)
			{
				this.player = base.GetComponent<PlayerController>();
			}
			actor.StartCoroutine(this.HandleTimedDestroy());
			actor.StartCoroutine(this.ShiftIntoPlace());

			PlayerOrbital playerOrbital2 = actor;
			SpeculativeRigidbody specRigidbody = playerOrbital2.specRigidbody;
			specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision));

			ImprovedAfterImage fuck = actor.gameObject.AddComponent<ImprovedAfterImage>();
			fuck.spawnShadows = true;
			fuck.shadowLifetime = 0.5f;
			fuck.shadowTimeDelay = 0.001f;
			fuck.dashColor = new Color(1, 0.5f, 0);
		}
		private void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody other, PixelCollider otherCollider)
		{
			Hits++;
			if (Hits == 2)
            {
				float DmG = (player.stats.GetStatValue(PlayerStats.StatType.Damage));
				Projectile projectile = ((Gun)ETGMod.Databases.Items[508]).DefaultModule.projectiles[0];
				GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, actor.sprite.WorldCenter, Quaternion.Euler(0f, 0f, ((player.CurrentGun == null) ? 1.2f : player.CurrentGun.CurrentAngle)), true);
				Projectile component = gameObject.GetComponent<Projectile>();
				bool flag = component != null;
				bool r = flag;
				if (r)
				{
					component.Owner = player;
					component.Shooter = player.specRigidbody;
					component.baseData.speed = 100f;
					component.baseData.damage = 20f * DmG;
				}
				AkSoundEngine.PostEvent("Play_BOSS_doormimic_blast_01", actor.gameObject);
				LootEngine.DoDefaultItemPoof(actor.sprite.WorldCenter, false, true);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		public static void GuonInit(Action<PlayerOrbital, PlayerController> orig, PlayerOrbital self, PlayerController player)
		{
			orig(self, player);
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
			if (CanFire == true && actor != null)
            {
				this.elapsed += BraveTime.DeltaTime;
				float RoF = (player.stats.GetStatValue(PlayerStats.StatType.RateOfFire));
				float DmG = (player.stats.GetStatValue(PlayerStats.StatType.Damage));

				bool flag3 = this.elapsed > TimeBetweenFiring / RoF;
				if (flag3)
				{
					if (this.player != null)
					{
						Vector2 worldCenter3 = this.actor.sprite.WorldCenter;
						Vector2 unitCenter3 = player.sprite.WorldCenter;
						float z3 = BraveMathCollege.Atan2Degrees((unitCenter3 - worldCenter3).normalized);
						Projectile projectile3 = ((Gun)ETGMod.Databases.Items[43]).DefaultModule.projectiles[0];
						GameObject gameObject3 = SpawnManager.SpawnProjectile(projectile3.gameObject, worldCenter3, Quaternion.Euler(0f, 0f, z3-180), true);
						Projectile component3 = gameObject3.GetComponent<Projectile>();
						bool flag15 = component3 != null;
						bool flag16 = flag15;
						if (flag16)
						{
							component3.baseData.damage = 30f * DmG;
							component3.Owner = player;
							component3.baseData.range = 1000f;
							component3.pierceMinorBreakables = true;
							component3.collidesWithPlayer = false;
							component3.baseData.speed *= 0.3f;

							Material sharedMaterial = component3.sprite.renderer.sharedMaterial;
							component3.sprite.usesOverrideMaterial = true;
							Material material = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
							material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
							material.SetColor("_OverrideColor", new Color(1f, 1f, 1f, 1f));
							this.LerpMaterialGlow(material, 0f, 22f, 0.4f);
							material.SetFloat("_EmissiveColorPower", 8f);
							material.SetColor("_EmissiveColor", new Color(255, 69, 0, 255));
							SpriteOutlineManager.AddOutlineToSprite(component3.sprite, new Color(255, 69, 0, 255));
							component3.sprite.renderer.material = material;
						}
					}
					this.elapsed = 0f;	
				}
			}				
		}

		private IEnumerator ShiftIntoPlace()
        {
			float elapsed = 0f;
			float duration = 3f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				bool flag3 = actor;
				if (flag3)
				{
					actor.SetOrbitalTier(0);
					actor.orbitRadius = elapsed*3f;
					actor.orbitDegreesPerSecond = elapsed * 7.5f;
				}
				yield return null;
			}
			elapsed = 0f;
			duration = 2f;

			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				bool flag3 = actor;
				if (flag3)
				{
					actor.SetOrbitalTier(0);
					actor.orbitRadius = (9f - (elapsed* 3.5f));
					actor.orbitDegreesPerSecond = ((elapsed+2) * 2.5f)+22.5f;
				}
				yield return null;
			}
			CanFire = true;
			yield break;
		}
		private IEnumerator HandleTimedDestroy()
		{
			float DmG = (player.stats.GetStatValue(PlayerStats.StatType.Damage));
			yield return new WaitForSeconds(this.maxDuration);
			AkSoundEngine.PostEvent("Play_BOSS_doormimic_blast_01", actor.gameObject);
			LootEngine.DoDefaultItemPoof(actor.sprite.WorldCenter, false, true);
			UnityEngine.Object.Destroy(base.gameObject);
			Projectile projectile = ((Gun)ETGMod.Databases.Items[508]).DefaultModule.projectiles[0];
			GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, actor.sprite.WorldCenter, Quaternion.Euler(0f, 0f, ((player.CurrentGun == null) ? 1.2f : player.CurrentGun.CurrentAngle)), true);
			Projectile component = gameObject.GetComponent<Projectile>();
			bool flag = component != null;
			bool r = flag;
			if (r)
			{
				component.Owner = player;
				component.Shooter = player.specRigidbody;
				component.baseData.speed = 100f;
				component.baseData.damage = 60f* DmG;
			}
			yield break;
		}
		public void LerpMaterialGlow(Material targetMaterial, float startGlow, float targetGlow, float duration)
		{
			targetMaterial.SetFloat("_EmissivePower", Mathf.Lerp(startGlow, targetGlow, duration));
		}

		public PlayerController sourcePlayer;
		public float maxDuration = 10f;
		private PlayerOrbital actor;
		public PlayerController player;
		public float random;
		public float TimeBetweenFiring;
		private float elapsed;
		public bool CanFire;
		public static Hook guonHook;
		public int Hits;
	}
}
