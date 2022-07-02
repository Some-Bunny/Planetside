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
	internal class DivineLightProjectile : MonoBehaviour
	{
		public DivineLightProjectile()
		{
		}

        public void Start()
        {
            //GameObject original;
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile)
            {
                PlayerController playerController = this.projectile.Owner as PlayerController;
                if (playerController.PlayerHasActiveSynergy("Deliver Us From All Evil") )
                {
                    this.projectile.BlackPhantomDamageMultiplier *= 3;
                }
                this.projectile.collidesWithPlayer = false;
                SpeculativeRigidbody specRigidbody2 = this.projectile.specRigidbody;
                specRigidbody2.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(specRigidbody2.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreTileCollision));
            }
        }
        public IEnumerator ResetSpearTimer()
        {
            HasSpeared = true;
            yield return new WaitForSeconds(0.125f);
            HasSpeared = false;
            yield break;
        }
        private bool HasSpeared;

        private void OnPreTileCollision(SpeculativeRigidbody myrigidbody, PixelCollider mypixelcollider, PhysicsEngine.Tile tile, PixelCollider tilepixelcollider)
		{
            if (myrigidbody && HasSpeared != true)
            {
                PlayerController user = myrigidbody.GetComponent<Projectile>().Owner as PlayerController;
                if (user != null)
                {
                    GameObject projObj = DivineLight.DivineLightProjectile.gameObject;
                    GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(projObj, myrigidbody.sprite.WorldCenter, Quaternion.Euler(0f, 0f, 0f), true);
                    Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                    if (component != null)
                    {
                        component.Owner = user;
                        component.Shooter = user.specRigidbody;

                        component.baseData.damage = 10;
                        component.gameObject.AddComponent<MaintainDamageOnPierce>();
                        component.baseData.speed = 0;
                        PierceProjModifier spook = component.gameObject.GetOrAddComponent<PierceProjModifier>();
                        spook.penetration = 10000;
                        DivineLightObjectController divine = component.gameObject.AddComponent<DivineLightObjectController>();
                        divine.player = user;
                        divine.self = component;
                    }
                    GameManager.Instance.StartCoroutine(this.ResetSpearTimer());
                }       
            }
        }
        private Projectile projectile;
	}
}

namespace Planetside
{
    public class DivineLightObjectController : MonoBehaviour
    {
        public DivineLightObjectController()
        {
            this.Duration = 3f;
            this.player = GameManager.Instance.PrimaryPlayer;
        }
        public void Start()
        {
            self.baseData.speed = 0;
            self.UpdateSpeed();
            LootEngine.DoDefaultItemPoof(base.gameObject.transform.position, false, true);
            AkSoundEngine.PostEvent("Play_ENM_tutorialknight_appear_01", base.gameObject);
            self.collidesWithEnemies = false;
            self.IgnoreTileCollisionsFor(100);
            
            self.ModifyVelocity = (Func<Vector2, Vector2>)Delegate.Combine(self.ModifyVelocity, new Func<Vector2, Vector2>(this.ModifyVelocity));
        }

        public void Update()
        {
            this.elapsed += BraveTime.DeltaTime;
            if (elapsed >= Duration)
            {
                if (isdead != true)
                {
                    AkSoundEngine.PostEvent("Play_ENM_gunnut_swing_01", base.gameObject);
                    self.baseData.speed = DivineLight.DivineLightProjectile.baseData.speed;
                    self.UpdateSpeed();
                    self.collidesWithEnemies = true;
                    isdead = true;
                }
                bool ae = Vector2.Distance(player.sprite.WorldCenter, self.transform.PositionVector2()) < 2f && self != null;
                if (ae)
                {
                    Destroy(base.gameObject);
                }
            }
        }
        private Vector2 ModifyVelocity(Vector2 inVel)
        {
            Vector2 vector = inVel;
            float num = float.MaxValue;
            Vector2 vector2 = Vector2.zero;
            PlayerController x = null;
            Vector2 b = (!self.sprite) ? base.transform.position.XY() : self.sprite.WorldCenter;
            PlayerController aiactor = player;
            if (aiactor && !aiactor.IsGone)
            {
                Vector2 vector3 = aiactor.CenterPosition - b;
                float sqrMagnitude = vector3.sqrMagnitude;
                if (sqrMagnitude < num)
                {
                    vector2 = vector3;
                    num = sqrMagnitude;
                    x = aiactor;
                }
            }

            num = Mathf.Sqrt(num);
            if (num < this.HomingRadius && x != null)
            {
                float num2 = 1f - num / this.HomingRadius;
                float target = vector2.ToAngle();
                float num3 = inVel.ToAngle();
                float maxDelta = this.AngularVelocity * num2 * this.self.LocalDeltaTime;
                float num4 = Mathf.MoveTowardsAngle(num3, target, maxDelta);
                {
                    if (this.self.shouldRotate)
                    {
                        base.transform.rotation = Quaternion.Euler(0f, 0f, num4);
                    }
                    vector = BraveMathCollege.DegreesToVector(num4, inVel.magnitude);
                }
            }
            if (vector == Vector2.zero || float.IsNaN(vector.x) || float.IsNaN(vector.y))
            {
                return inVel;
            }
            return vector;
        }

        public float HomingRadius = 100;
        public float AngularVelocity = 10000;

        private bool isdead;
        private float elapsed;
        public Projectile self;
        public PlayerController player;
        public float Duration;
    }
}
