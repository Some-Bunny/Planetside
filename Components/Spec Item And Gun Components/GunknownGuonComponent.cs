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
using System.ComponentModel;

namespace Planetside
{
	public class GunknownGuonComponent : MonoBehaviour
	{
		public GunknownGuonComponent()
		{
			this.maxDuration = 10;
			this.TimeBetweenFiring = 0.5f;
			this.sourcePlayer = GameManager.Instance.PrimaryPlayer;
			this.MaxHealth = 3;
		}



		public void Start()
		{
            CanFire = false;
            Orbital.StartCoroutine(this.HandleTimedDestroy());
            Orbital.StartCoroutine(this.ShiftIntoPlace());

			SpeculativeRigidbody specRigidbody = Orbital.specRigidbody;
			specRigidbody.OnPreRigidbodyCollision += (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision));
		}
		private void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody other, PixelCollider otherCollider)
		{
            if (Cooldown == false)
            {
                MaxHealth--;
                Cooldown = true;
                this.Invoke("C", 0.25f);
            }
            if (MaxHealth == 0)
            {
				float DmG = (sourcePlayer.stats.GetStatValue(PlayerStats.StatType.Damage));
				Projectile projectile = isSynergy ? ((Gun)ETGMod.Databases.Items[383]).DefaultModule.projectiles[0] : ((Gun)ETGMod.Databases.Items[508]).DefaultModule.projectiles[0];
				GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, Orbital.sprite.WorldCenter, Quaternion.Euler(0f, 0f, ((sourcePlayer.CurrentGun == null) ? 1.2f : sourcePlayer.CurrentGun.CurrentAngle)), true);
				Projectile component = gameObject.GetComponent<Projectile>();
				if (component != null)
				{
					component.Owner = sourcePlayer;
					component.Shooter = sourcePlayer.specRigidbody;
                    component.AdditionalScaleMultiplier = isSynergy ? 2 : 1;
                    component.baseData.damage = isSynergy ? 30 * DmG : 20f * DmG;
                    if (isSynergy)
                    {
                        var bouncy = component.gameObject.AddComponent<BounceProjModifier>();
                        bouncy.numberOfBounces = 3;
                    }
				}
				AkSoundEngine.PostEvent("Play_BOSS_doormimic_blast_01", Orbital.gameObject);
				LootEngine.DoDefaultItemPoof(Orbital.sprite.WorldCenter, false, true);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
        private bool Cooldown = false;
        public void C()
        {
            Cooldown = false;
        }

        public void Update()
		{
			if (CanFire == true && Orbital != null)
            {
				this.elapsed -= BraveTime.DeltaTime;
				if (this.elapsed < 0)
				{
                    float RoF = (sourcePlayer.stats.GetStatValue(PlayerStats.StatType.RateOfFire));
                    float DmG = (sourcePlayer.stats.GetStatValue(PlayerStats.StatType.Damage));
                    this.elapsed = (TimeBetweenFiring / RoF);
                    if (this.sourcePlayer != null)
					{

						if (isSynergy)
						{
                            AkSoundEngine.PostEvent("Play_Strafe_Shot", Orbital.gameObject);

                            GameObject gameObject3 = SpawnManager.SpawnProjectile(PointNull.PointNullDart.gameObject, Orbital.sprite.WorldCenter, Quaternion.Euler(0f, 0f, sourcePlayer.CurrentGun.CurrentAngle), true);
                            Projectile proj_ = gameObject3.GetComponent<Projectile>();

                            proj_.baseData.range = 125f;
							proj_.specRigidbody.RegisterSpecificCollisionException(Orbital.specRigidbody);
                            proj_.Owner = sourcePlayer;
                            proj_.Shooter = sourcePlayer.specRigidbody;

                        }
                        else
						{

                            Vector2 worldCenter3 = Orbital.sprite.WorldCenter;
                            Vector2 unitCenter3 = sourcePlayer.sprite.WorldCenter;
                            float z3 = BraveMathCollege.Atan2Degrees((unitCenter3 - worldCenter3).normalized);
                            Projectile projectile3 = ((Gun)ETGMod.Databases.Items[43]).DefaultModule.projectiles[0];
                            GameObject gameObject3 = SpawnManager.SpawnProjectile(projectile3.gameObject, worldCenter3, Quaternion.Euler(0f, 0f, z3 - 180), true);
                            Projectile component3 = gameObject3.GetComponent<Projectile>();
                            if (component3 != null)
                            {
                                component3.baseData.damage = 30f * DmG;
                                component3.Owner = sourcePlayer;
                                component3.baseData.range = 1000f;
                                component3.pierceMinorBreakables = true;
                                component3.collidesWithPlayer = false;
                                component3.baseData.speed *= 0.3f;
								component3.Owner = sourcePlayer;

                                Material sharedMaterial = component3.sprite.renderer.sharedMaterial;
                                component3.sprite.usesOverrideMaterial = true;
                                Material material = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
                                material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
                                material.SetColor("_OverrideColor", new Color(1f, 1f, 1f, 1f));
                                material.SetFloat("_EmissiveColorPower", 8f);
                                material.SetColor("_EmissiveColor", new Color(255, 69, 0, 255));
                                SpriteOutlineManager.AddOutlineToSprite(component3.sprite, new Color(255, 69, 0, 255));
                                component3.sprite.renderer.material = material;
                            }
                        }

						
					}
				}
			}				
		}

		private IEnumerator ShiftIntoPlace()
        {
            Orbital.orbitDegreesPerSecond = 90;

            if (isSynergy)
			{
                float elapsed = 0f;
                float duration = 1.5f;
                while (elapsed < duration)
                {
                    elapsed += BraveTime.DeltaTime;
                    if (Orbital == null) { yield break; }
                    Orbital.SetOrbitalTier(110);
                    Orbital.orbitRadius = elapsed * 1.25f;
                    yield return null;
                }
               
            }
			else
			{
                float elapsed = 0f;
                float duration = 3f;
                while (elapsed < duration)
                {
                    elapsed += BraveTime.DeltaTime;
                    if (Orbital == null) { yield break; }
                    Orbital.SetOrbitalTier(110);
                    Orbital.orbitRadius = elapsed * 3f;
                    yield return null;
                }
                elapsed = 0f;
                duration = 2f;
                float e = Orbital.orbitRadius;
                while (elapsed < duration)
                {
                    elapsed += BraveTime.DeltaTime;
                    if (Orbital == null) { yield break; }
                    Orbital.SetOrbitalTier(110);
                    Orbital.orbitRadius = e - (elapsed * 3.75f);
                    yield return null;
                }
            }

            yield return new WaitForSeconds(fireDelay);
			CanFire = true;
			yield break;
		}
		private IEnumerator HandleTimedDestroy()
		{
			float DmG = (sourcePlayer.stats.GetStatValue(PlayerStats.StatType.Damage));
			yield return new WaitForSeconds(this.maxDuration);
			AkSoundEngine.PostEvent("Play_BOSS_doormimic_blast_01", Orbital.gameObject);
			LootEngine.DoDefaultItemPoof(Orbital.sprite.WorldCenter, false, true);
			UnityEngine.Object.Destroy(base.gameObject);
            Projectile projectile = isSynergy ? ((Gun)ETGMod.Databases.Items[383]).DefaultModule.projectiles[0] : ((Gun)ETGMod.Databases.Items[508]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, Orbital.sprite.WorldCenter, Quaternion.Euler(0f, 0f, ((sourcePlayer.CurrentGun == null) ? 1.2f : sourcePlayer.CurrentGun.CurrentAngle)), true);
			Projectile component = gameObject.GetComponent<Projectile>();
			if (component != null)
			{
                component.Owner = sourcePlayer;
                component.Shooter = sourcePlayer.specRigidbody;
                //component.baseData.speed = 100f;
                component.AdditionalScaleMultiplier = isSynergy ? 2 : 1;
                component.baseData.damage = isSynergy ? 75 * DmG : 60f * DmG;
                if (isSynergy)
                {
                    component.ignoreDamageCaps = true;
                    var bouncy = component.gameObject.AddComponent<BounceProjModifier>();
                    bouncy.numberOfBounces = 3;
                }
            }
            sourcePlayer.RecalculateOrbitals();
            yield break;
		}


		public PlayerController sourcePlayer;
        public PlayerOrbital Orbital;

        public float fireDelay = 0;
        public float maxDuration = 10f;
		public float TimeBetweenFiring;
		private float elapsed;
		public bool CanFire;
		public int MaxHealth = 3;
		public bool isSynergy = false;
	}
}
