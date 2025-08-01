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
using Alexandria.EnemyAPI;

namespace Planetside
{
	internal class PunctureWoundProjectile : MonoBehaviour
	{
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile != null)
            {              
                projectile.statusEffectsToApply = new List<GameActorEffect> { DebuffLibrary.Corrosion };

                projectile.specRigidbody.OnRigidbodyCollision += (_) =>
                {
                    var p = (_ as CollisionData);
                    ApplyEffects(projectile, _.PostCollisionUnitCenter);
                };

                projectile.specRigidbody.OnTileCollision += (_) =>
                {
                    var p = (_ as CollisionData);
                    ApplyEffects(projectile, _.PostCollisionUnitCenter);
                };
                projectile.OnDestruction += (_) =>
                {
                    ApplyEffects(_, projectile.transform.position);
                };
            }
        }


        public void ApplyEffects(Projectile projectile, Vector2 HitPosition)
        {
            var room = projectile.transform.position.GetAbsoluteRoom();
            if (room != null)
            {
               var enm = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (enm != null)
                {
                    enm = enm.Where(self => Vector2.Distance(HitPosition, self.sprite.WorldCenter) < 3).ToList();
                    foreach (var enemy in enm)
                    {
                        enemy.healthHaver.ApplyDamage(projectile.ModifiedDamage * 0.333f, Vector2.zero, "");
                        enemy.ApplyEffect(DebuffLibrary.Corrosion);
                    }
                }
            }
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                startColor = Color.yellow.WithAlpha(0.5f),
                position = HitPosition,
                startSize = 5.5f,
                startLifetime = 0.25f
            });

            for (int i = 0; i < 24; i++)
            {
                ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = HitPosition,
                    startLifetime = 0.5f,
                    velocity = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(4, 8)),
                    
                });
            }
        }

        private Projectile projectile;
	}
}

