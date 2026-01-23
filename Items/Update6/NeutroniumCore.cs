using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Collections;
using SaveAPI;
using Brave.BulletScript;
using Alexandria.Misc;
using HutongGames.PlayMaker.Actions;
using static UnityEngine.UI.GridLayoutGroup;
using static Planetside.AoEBullets;

namespace Planetside
{
	public class NeutroniumCore : PassiveItem
	{
		public static void Init()
		{
			string name = "Neutronium Core";
			GameObject gameObject = new GameObject(name);
            NeutroniumCore item = gameObject.AddComponent<NeutroniumCore>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("neutroniumcore"), data, gameObject); 
			string shortDesc = "Ultradense Hyperbullets";
			string longDesc = "Emits a gravitational pull towards enemies.\n\nTiny chunks of neutronium are very carefully inserted into every bullet by severely underpaid neutron gnomes.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.C;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RangeMultiplier, 1.3f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 1.2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ID = item.PickupObjectId;
		}
		public static int ID;
		public override void Pickup(PlayerController player)
		{
            player.PostProcessBeam += PostProcessBeam;
            player.PostProcessProjectile += PPP;
            base.Pickup(player);
		}

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessBeam -= PostProcessBeam;
            player.PostProcessProjectile -= PPP;
            return base.Drop(player);
        }


        public override void OnDestroy()
        {
            if (Owner != null)
            {
                base.Owner.PostProcessProjectile -= this.PPP;
                base.Owner.PostProcessBeam -= PostProcessBeam;
            }
            base.OnDestroy();
        }


        private void PPP(Projectile arg1, float arg2)
        {
            if (arg1 != null)
            {
                arg1.baseData.speed *= 0.7f;
                arg1.UpdateSpeed();
                var well = arg1.gameObject.AddComponent<EnemyGravityWell>();
                var ___ = (arg1.sprite.GetBounds().size.x * 32) * (arg1.sprite.GetBounds().size.y * 32);
                float Pull = arg1.sprite == null ? 125f :Mathf.Max(256, Mathf.Sqrt(___) * 12);
                well.gravitationalForce = Pull * 0.04f;
                well.gravitationalForceActors = Pull;
                well.self = arg1;
                well.RadiusVisual = ___ * 0.025f;
            }
        }

        private void PostProcessBeam(BeamController beamC)
        {
            if (beamC != null)
            {
                beamC.projectile.baseData.speed *= 0.7f;
                beamC.projectile.UpdateSpeed();
                var well = beamC.projectile.gameObject.AddComponent<EnemyGravityWellBeam>();
                var ___ = (beamC.projectile.sprite.GetBounds().size.x * 32) * (beamC.projectile.sprite.GetBounds().size.y * 32);
                float Pull = beamC.projectile.sprite == null ? 125f : Mathf.Max(256, Mathf.Sqrt(___) * 4);
                well.gravitationalForce = Pull * 0.04f;
                well.gravitationalForceActors = Pull;
                well.self = beamC.projectile;
                well.RadiusVisual = ___ * 0.01f;
                well.basicBeamController = beamC.GetComponent<BasicBeamController>();
            }
        }



        public class EnemyGravityWell : MonoBehaviour
        {
            public Projectile self;
            public float radius = 50f;
            public float gravitationalForce = 10f;
            public float gravitationalForceActors = 150f;
            private float m_radiusSquared;
            public float RadiusVisual;
            public void Start()
            {
                cachedgravitationalForceActors = gravitationalForceActors;
                this.m_radiusSquared = radius * radius;
            }
            private float cachedgravitationalForceActors;

            private float Elapsed = -0.25f;
            float t = 0;
            public void Update()
            {
                Elapsed += BraveTime.DeltaTime;
                gravitationalForceActors = Mathf.Lerp(0, cachedgravitationalForceActors, Elapsed);
                if (gravitationalForceActors > 0)
                {
                    var room = this.transform.position.GetAbsoluteRoom();
                    if (room != null)
                    {
                        var enm = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                        if (enm != null)
                        {
                            for (int a = enm.Count - 1; a > -1; a--)
                            {
                                var entry = enm[a];
                                if (entry.State == AIActor.ActorState.Normal)
                                {

                                    var other = entry.specRigidbody;
                                    Vector2 __ = other.UnitCenter - (self.specRigidbody != null ? self.specRigidbody.UnitCenter : new Vector2(self.transform.position.x, self.transform.position.y));
                                    float num = Vector2.SqrMagnitude(__);
                                    if (num < this.m_radiusSquared)
                                    {
                                        float g = this.gravitationalForceActors;
                                        Vector2 velocity = other.Velocity;
                                        float M = Vector2.Distance(self.transform.position, other.transform.position) / radius;
                                        M = 1 - (M * M);
                                        Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(other.UnitCenter, Mathf.Sqrt(num), g) * M;
                                        float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
                                        Vector2 b = frameAccelerationForRigidbody * d;
                                        Vector2 vector = velocity + b;
                                        if (BraveTime.DeltaTime > 0.02f)
                                        {
                                            vector *= 0.02f / BraveTime.DeltaTime;
                                        }
                                        other.Velocity = vector;
                                    }
                                }
                            }
                        }
                    }



                    for (int j = 0; j < StaticReferenceManager.AllDebris.Count; j++)
                    {
                        this.AdjustDebrisVelocity(StaticReferenceManager.AllDebris[j]);
                    }
                    Vector3 _ = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(RadiusVisual, RadiusVisual * 1.4f));

                    t -= BraveTime.DeltaTime;
                    if (t < 0)
                    {
                        t = 0.1f;
                        ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = this.transform.position + _,
                            velocity = (this.transform.position - (this.transform.position + _)).normalized * (RadiusVisual * 2),
                            startColor = Color.white.WithAlpha(0.5f),
                            startLifetime = 0.5f,
                            startSize = 0.1f
                        });
                    }
                }
            }


            private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
            {
                Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
                return new Vector4(vector.x, vector.y, 0f, 0f);
            }

            private float GetDistanceToRigidbody(SpeculativeRigidbody other)
            {
                return Vector2.Distance(other.UnitCenter, self.specRigidbody.UnitCenter);
            }

            private Vector2 GetFrameAccelerationForRigidbody(Vector2 unitCenter, float currentDistance, float g)
            {
                Vector2 zero = Vector2.zero;
                float num = Mathf.Clamp01(1f - currentDistance / radius);
                float d = g * num * num;
                Vector2 normalized = (self.specRigidbody.UnitCenter - unitCenter).normalized;
                return normalized * d;
            }

            private bool AdjustDebrisVelocity(DebrisObject debris)
            {
                if (debris == null)
                {
                    return false;
                }

                if (debris.IsPickupObject)
                {
                    return false;
                }
                if (self == null)
                {
                    return false;
                }

                Vector2 a = debris.sprite.WorldCenter - (self.specRigidbody != null ? self.specRigidbody.UnitCenter : new Vector2(self.transform.position.x, self.transform.position.y));
                float num = Vector2.SqrMagnitude(a);
                if (num >= this.m_radiusSquared)
                {
                    return false;
                }
                float g = this.gravitationalForce;
                float num2 = Mathf.Sqrt(num);
                if (num2 < 1)
                {
                    UnityEngine.Object.Destroy(debris.gameObject);
                    return true;
                }
                Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(debris.sprite.WorldCenter, num2, g);
                float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
                if (debris.HasBeenTriggered)
                {
                    debris.ApplyVelocity(frameAccelerationForRigidbody * d);
                }
                else if (num2 < radius / 2f)
                {
                    debris.Trigger(frameAccelerationForRigidbody * d, 0.5f, 1f);
                }
                return true;
            }
        }

        public class EnemyGravityWellBeam : MonoBehaviour
        {
            public Projectile self;
            public float radius = 50f;
            public float gravitationalForce = 10f;
            public float gravitationalForceActors = 150f;
            private float m_radiusSquared;
            public float RadiusVisual;


            public Projectile projectile;
            public BasicBeamController basicBeamController;
            public PlayerController owner;
            private float cachedgravitationalForceActors;

            private void Start()
            {
                cachedgravitationalForceActors = gravitationalForceActors;
                this.m_radiusSquared = radius * radius;
            }

            private float Elapsed = -0.25f;
            float t = 0;
            public void Update()
            {
                DoTick();
                Elapsed += BraveTime.DeltaTime;
                gravitationalForceActors = Mathf.Lerp(0, cachedgravitationalForceActors, Elapsed);
                if (gravitationalForceActors > 0)
                {
                    var room = this.transform.position.GetAbsoluteRoom();
                    if (room != null)
                    {
                        var enm = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                        if (enm != null)
                        {
                            for (int a = enm.Count - 1; a > -1; a--)
                            {
                                var entry = enm[a];
                                if (entry.State == AIActor.ActorState.Normal)
                                {

                                    var other = entry.specRigidbody;
                                    Vector2 __ = other.UnitCenter - new Vector2(LastPosition.x, LastPosition.y);
                                    float num = Vector2.SqrMagnitude(__);
                                    if (num < this.m_radiusSquared)
                                    {
                                        float g = this.gravitationalForceActors;
                                        Vector2 velocity = other.Velocity;
                                        float M = Vector2.Distance(LastPosition, other.transform.position) / radius;
                                        M = 1 - (M * M);
                                        Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(other.UnitCenter, Mathf.Sqrt(num), g) * M;
                                        float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
                                        Vector2 b = frameAccelerationForRigidbody * d;
                                        Vector2 vector = velocity + b;
                                        if (BraveTime.DeltaTime > 0.02f)
                                        {
                                            vector *= 0.02f / BraveTime.DeltaTime;
                                        }
                                        other.Velocity = vector;
                                    }
                                }
                            }
                        }
                    }



                    for (int j = 0; j < StaticReferenceManager.AllDebris.Count; j++)
                    {
                        this.AdjustDebrisVelocity(StaticReferenceManager.AllDebris[j]);
                    }
                    Vector3 _ = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(RadiusVisual, RadiusVisual * 1.4f));

                    t -= BraveTime.DeltaTime;
                    if (t < 0)
                    {
                        t = 0.03f;
                        ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = LastPosition + _,
                            velocity = (LastPosition - (LastPosition + _)).normalized * (RadiusVisual * 2),
                            startColor = Color.white.WithAlpha(0.5f),
                            startLifetime = 0.5f,
                            startSize = 0.1f
                        });
                    }
                }
            }




            private Vector2 GetFrameAccelerationForRigidbody(Vector2 unitCenter, float currentDistance, float g)
            {
                Vector2 zero = Vector2.zero;
                float num = Mathf.Clamp01(1f - currentDistance / radius);
                float d = g * num * num;
                Vector2 normalized = (new Vector2(LastPosition.x, LastPosition.y) - unitCenter).normalized;
                return normalized * d;
            }

            private bool AdjustDebrisVelocity(DebrisObject debris)
            {
                if (debris == null)
                {
                    return false;
                }

                if (debris.IsPickupObject)
                {
                    return false;
                }
                if (self == null)
                {
                    return false;
                }

                Vector2 a = debris.sprite.WorldCenter - new Vector2(LastPosition.x, LastPosition.y);
                float num = Vector2.SqrMagnitude(a);
                if (num >= this.m_radiusSquared)
                {
                    return false;
                }
                float g = this.gravitationalForce;
                float num2 = Mathf.Sqrt(num);
                if (num2 < 1)
                {
                    UnityEngine.Object.Destroy(debris.gameObject);
                    return true;
                }
                Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(debris.sprite.WorldCenter, num2, g);
                float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
                if (debris.HasBeenTriggered)
                {
                    debris.ApplyVelocity(frameAccelerationForRigidbody * d);
                }
                else if (num2 < radius / 2f)
                {
                    debris.Trigger(frameAccelerationForRigidbody * d, 0.5f, 1f);
                }
                return true;
            }

            private void DoTick()
            {
                LinkedList<BasicBeamController.BeamBone> linkedList = PlanetsideReflectionHelper.ReflectGetField<LinkedList<BasicBeamController.BeamBone>>(typeof(BasicBeamController), "m_bones", this.basicBeamController);
                LinkedListNode<BasicBeamController.BeamBone> last = linkedList.Last;
                Vector2 bonePosition = this.basicBeamController.GetBonePosition(last.Value);
                LastPosition = bonePosition;
            }
            public Vector3 LastPosition;
        }

    }
}
