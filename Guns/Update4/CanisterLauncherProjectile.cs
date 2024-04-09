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
	internal class CanisterLauncherProjectile : MonoBehaviour
	{
		public CanisterLauncherProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile != null)
            {
                projectile.OnDestruction += this.Kaboom;
                projectile.specRigidbody.OnPreRigidbodyCollision += OnPreCollision;

            }
        }

        private void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
        {
            Projectile p = otherRigidbody.projectile;
            if (p != null)
            {
                PhysicsEngine.SkipCollision = true;
                AIActor enemy = p.Owner as AIActor;
                PlayerController player = p.Owner as PlayerController;
                this.projectile.DieInAir(false);
                if (p.Owner == enemy)
                {

                    //DoReflect(p, GameManager.Instance.PrimaryPlayer, 30, BraveUtility.RandomAngle(), 1, 20);
                }
                else if (p.Owner == player)
                {

                }
            }           
        }

        private void Kaboom(Projectile projectile)
		{
            AkSoundEngine.PostEvent("Play_ENM_cannonball_blast_01", projectile.gameObject);

            GameObject gameObject = GameObject.Instantiate((GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost"), projectile.sprite.WorldCenter, Quaternion.identity);
            Destroy(gameObject, 2f);
            for (int i =0; i < StaticReferenceManager.AllProjectiles.Count; i++)
            {
                Projectile proj = StaticReferenceManager.AllProjectiles[i];
                if (proj != null)
                {
                    if (proj.GetComponent<BasicBeamController>() == null && proj.GetComponent<BeamController>() == null)
                    {
                        if (Vector2.Distance(proj.sprite ? proj.sprite.WorldCenter : proj.transform.PositionVector2(), base.gameObject.transform.PositionVector2()) < 2.5f && proj.Owner != null && proj.gameObject.GetComponent<MarkedProjectile>() == null)
                        {
                            AIActor enemy = proj.Owner as AIActor;
                            PlayerController player = proj.Owner as PlayerController;

                            if (proj.Owner == enemy)
                            {

                                SpawnManager.PoolManager.Remove(proj.gameObject.transform);
                                proj.BulletScriptSettings.preventPooling = true;

                                DoReflect(proj, GameManager.Instance.PrimaryPlayer, proj.Speed * 1.5f, BraveUtility.RandomAngle(), 1, 20);

                                BounceProjModifier bouncy = proj.gameObject.GetOrAddComponent<BounceProjModifier>();
                                bouncy.numberOfBounces = 1;

                              
                            }
                            else if (proj.Owner == player)
                            {
                                proj.baseData.damage *= 2;
                                proj.baseData.speed *= 2;
                                proj.UpdateSpeed();
                                proj.baseData.range += 5;

                                BounceProjModifier bouncy = proj.gameObject.GetOrAddComponent<BounceProjModifier>();
                                bouncy.numberOfBounces += 1;

                                ImprovedAfterImage yes = proj.gameObject.AddComponent<ImprovedAfterImage>();
                                yes.spawnShadows = true;
                                yes.shadowLifetime = 0.1f;
                                yes.shadowTimeDelay = 0.01f;
                                yes.dashColor = new Color(0.9f, 0.6f, 0f, 1f);
                            }
                        }
                    }
                }
            }
        }

        public static void DoReflect(Projectile p, GameActor newOwner, float minReflectedBulletSpeed, float ReflectAngle, float scaleModifier = 1f, float damageModifier = 10f, float spread = 0f)
        {
            p.RemoveBulletScriptControl();
            Vector2 Point1 = MathToolbox.GetUnitOnCircle(ReflectAngle, 1);
            p.Direction = Point1;

            if (spread != 0f)
            {
                p.Direction = p.Direction.Rotate(UnityEngine.Random.Range(-spread, spread));
            }
            if (p.Owner && p.Owner.specRigidbody)
            {
                p.specRigidbody.DeregisterSpecificCollisionException(p.Owner.specRigidbody);
            }
            p.Owner = newOwner;
            p.SetNewShooter(newOwner.specRigidbody);
            p.allowSelfShooting = false;
            if (newOwner is AIActor)
            {
                p.collidesWithPlayer = true;
                p.collidesWithEnemies = false;
            }
            else
            {
                p.collidesWithPlayer = false;
                p.collidesWithEnemies = true;
            }
            if (scaleModifier != 1f)
            {
                SpawnManager.PoolManager.Remove(p.transform);
                p.RuntimeUpdateScale(scaleModifier);
            }
            if (p.Speed < minReflectedBulletSpeed)
            {
                p.Speed = minReflectedBulletSpeed;
            }
            if (p.baseData.damage < ProjectileData.FixedFallbackDamageToEnemies)
            {
                p.baseData.damage = ProjectileData.FixedFallbackDamageToEnemies;
            }
            p.baseData.damage = damageModifier;
            p.UpdateCollisionMask();
            p.ResetDistance();
            p.Reflected();
        }

		private Projectile projectile;
	}
}

