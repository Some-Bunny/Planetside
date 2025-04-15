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
using static Planetside.PrisonerSecondSubPhaseController;
using System.ComponentModel;

namespace Planetside
{
	internal class RiftakerProjectile : MonoBehaviour
	{
        public class RiftakerAffectedProjectile : MonoBehaviour
        {
            public void Start()
            {
                riftakerAffectedProjectiles.Add(this);

              
            }

            public void Warp(RiftakerProjectile In, bool sound)
            {

                if (PortalOut.Count > 0)
                {
                    var list_2 =  PortalOut.Where(self => self.currentRoom == In.currentRoom).ToList();
                    if (list_2.Count > 0)
                    {
                        if (Projectile.Owner is PlayerController)
                        {
                            var p = Projectile.Owner as PlayerController;
                            if (p.PlayerHasActiveSynergy("Event Horizon"))
                            {
                                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(90) as Gun).DefaultModule.projectiles[0].gameObject, this.Projectile.sprite.WorldCenter, Quaternion.Euler(0f, 0f, 0), true);
                                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                                if (component != null)
                                {
                                    component.Owner = p;
                                    component.Shooter = p.specRigidbody;
                                }
                                component.SendInDirection(MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 1), true, true);
                                component.baseData.damage = Projectile.baseData.damage * 0.333f;
                            }
                        }

                        //currentRoom
                        //AkSoundEngine.PostEvent("Play_WPN_beam_slash_01", base.gameObject);
                        //Play_WPN_blackhole_impact_01
                        if (sound)
                        {
                            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.BlackHoleImpact, Projectile.sprite.WorldCenter, Quaternion.Euler(0, 0, BraveUtility.RandomAngle()));
                            gameObject2.transform.localScale *= 0.75f;
                            Destroy(gameObject2, 2);
                        }


                        var outPortal = list_2[UnityEngine.Random.Range(0, list_2.Count)];
                        Projectile.specRigidbody.transform.position = outPortal.projectile.sprite.WorldCenter;
                        Projectile.specRigidbody.Reinitialize();
                        if (sound)
                        {
                            GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.BlackHoleImpact, outPortal.projectile.sprite.WorldCenter, Quaternion.Euler(0, 0, BraveUtility.RandomAngle()));
                            gameObject1.transform.localScale *= 0.75f;
                            Destroy(gameObject1, 2);
                        }

    


                        //StaticVFXStorage.SpiratTeleportVFX.SpawnAtPosition(outPortal.transform.position);

                        //Vector2 a2 = outPortal.transform.position - this.Projectile.specRigidbody.UnitCenter;
                        //float num2 = Vector2.SqrMagnitude(a);
                        //projectile.Speed = Mathf.Max(vector.magnitude * 0.5f, 12f);
                        Projectile.SkipDistanceElapsedCheck = false;


                        list_2 = PortalIn.Where(self => self.currentRoom == In.currentRoom).ToList();
                        if (list_2.Count > 0)
                        {
                            outPortal = list_2[UnityEngine.Random.Range(0, list_2.Count)];

                            float e = Mathf.Min(90, (6f * Vector2.Distance(outPortal.transform.position, Projectile.transform.position)));


                            HeadTo = PortalIn[UnityEngine.Random.Range(0, PortalIn.Count)];

                            float finaldir = ProjSpawnHelper.GetAccuracyAngled(MathToolbox.ToAngle((Projectile.transform.position - outPortal.transform.position)), e, Projectile.Owner as PlayerController);
                            Projectile.SendInDirection(MathToolbox.GetUnitOnCircle(finaldir + 180, 1), true);
                        }
                        else
                        {
                            Projectile.SendInDirection(MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 1), true);

                        }

                    }
                    
                }

               

            }

            public void OnDestroy()
            {
                riftakerAffectedProjectiles.Remove(this);
            }
            public RiftakerProjectile HeadTo;
            public Projectile Projectile;
            public int Amount = 0;
            public bool isDart = false;
        }


        public RiftakerProjectile()
		{
            this.damageRadius = 7;
            this.gravitationalForce = 25;
            this.radius = 40;
            this.radiusSquared = radius * radius;
        }

        public bool In = false;

        public static List<RiftakerProjectile> PortalIn = new List<RiftakerProjectile>();
        public static List<RiftakerProjectile> PortalOut = new List<RiftakerProjectile>();

        public static List<RiftakerAffectedProjectile> riftakerAffectedProjectiles = new List<RiftakerAffectedProjectile>();

        public void Start()
        {
            //Riftaker_Detonate
            if (this.projectile != null)
            {
                currentRoom = this.projectile.transform.position.GetAbsoluteRoom();
                this.player = this.projectile.Owner as PlayerController;
                this.projectile.specRigidbody.CollideWithOthers = false;

                Exploder.DoDistortionWave(projectile.sprite.WorldCenter, 0.1f, 0.5f, 3, 0.25f);
                GameManager.Instance.StartCoroutine(OpenPortal());
                AkSoundEngine.PostEvent("Play_OBJ_cursepot_loop_01", base.gameObject);
                if (In)
                {
                    PortalIn.Add(this);

                }
                else
                {
                    PortalOut.Add(this);
                }
            }              
        }

        public RoomHandler currentRoom;

        private IEnumerator OpenPortal()
        {
            float elapsed = 0f;
            float duration = 1;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                float t = elapsed / duration ;
                if (projectile == null) { yield break; }
                yield return null;
            }
            if (In)
            {
                EffectWarp = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(536) as RelodestoneItem).ContinuousVFX, true);
                EffectWarp.transform.parent = projectile.transform;
                EffectWarp.transform.position = projectile.sprite.WorldCenter;
            }
            isGooning = true;
            yield break;
        }

        private IEnumerator Obliterate()
        {
            //m_BOSS_lichB_charge_02
            float elapsed = 0f;
            float duration = 1;
            AkSoundEngine.PostEvent("Play_BOSS_lichB_charge_02", base.gameObject);

            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime * 1.5f;
                float t = elapsed / duration;
                if (projectile == null) { yield break; }
                yield return null;
            }
            Exploder.DoDistortionWave(projectile.sprite.WorldCenter, 2f, 0.1f, 8, 0.35f);
            var onj = UnityEngine.Object.Instantiate(StaticVFXStorage.BlueSynergyPoofVFX, projectile.sprite.WorldCenter, Quaternion.Euler(0, 0, 0));
            Destroy(onj, 2);
            for (int i = 0; i < 64; i++)
            {
                StaticVFXStorage.VoidParticleSystem.Emit(new ParticleSystem.EmitParams()
                {
                    position = this.projectile.sprite.WorldCenter,
                    rotation = BraveUtility.RandomAngle(),
                    velocity = BraveUtility.RandomVector2(new Vector2(-8f, -8), new Vector2(8, 8)),
                    startLifetime = 5         
                }, 1);
            }
            AkSoundEngine.PostEvent("Play_BOSS_tank_bullet_01", base.gameObject);

            Destroy(EffectWarp);
            Destroy(this.gameObject);

            yield break;
        }

        private GameObject EffectWarp;

        private bool isGooning = false;

        public float AttackCooldown = 0.8f;
        public float CurrentAttackCooldown = 0;


        public float E = 21;
        private bool isDead = false;

        private float e_Sound = 0.3f;


        public void Update()
        {
            if (isDead) {return; }
            if (e_Sound > 0)
            {
                e_Sound -= Time.deltaTime;
            }
            if (E > 0)
            {
                E -= Time.deltaTime;
                
            }
            else if (isDead == false)
            {
                isDead = true;
                this.projectile.spriteAnimator.PlayAndDestroyObject("Riftaker_Detonate");
                this.StartCoroutine(Obliterate());
                if (PortalOut.Contains(this))
                {
                    PortalOut.Remove(this);
                }
                if (PortalIn.Contains(this))
                {
                    PortalIn.Remove(this);
                }
                AkSoundEngine.PostEvent("Stop_OBJ_cursepot_loop_01", base.gameObject);
            }

            StaticVFXStorage.VoidParticleSystem.Emit(new ParticleSystem.EmitParams()
            {
                position = this.projectile.sprite.WorldCenter,
                rotation = BraveUtility.RandomAngle(),
                velocity = BraveUtility.RandomVector2(new Vector2(-0.025f, -0.025f), new Vector2(0.025f, 0.025f))
            }, 1);

            if (!In)
            {
                if (isDead == false)
                {
                    CurrentAttackCooldown += Time.deltaTime;
                    if (CurrentAttackCooldown >= AttackCooldown)
                    {
                        //AkSoundEngine.PostEvent("Play_BOSS_agunim_orb_01", base.gameObject);

                        CurrentAttackCooldown = 0;
                        GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(Riftaker.DartProjectile.gameObject, this.projectile.sprite.WorldCenter, Quaternion.Euler(0f, 0f, 0), true);
                        Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                        if (component != null)
                        {
                            component.Owner = player;
                            component.Shooter = player.specRigidbody;
                        }
                        RiftakerAffectedProjectile component_ = spawnedBulletOBJ.GetComponent<RiftakerAffectedProjectile>();
                        component_.Projectile = component;
                        component_.HeadTo = null;
                        var list_2 = PortalIn.Where(self => self.currentRoom == this.currentRoom).ToList();
                        if (list_2.Count > 0)
                        {
                            var p = list_2[UnityEngine.Random.Range(0, list_2.Count)];
                            component_.HeadTo = p;

                            float e = Mathf.Min(90, (6f * Vector2.Distance(component_.transform.position, this.transform.position)));
                            float finaldir = ProjSpawnHelper.GetAccuracyAngled(MathToolbox.ToAngle((p.transform.position - component_.transform.position)), e, player);
                            component.SendInDirection(MathToolbox.GetUnitOnCircle(finaldir, 1), true, true);
                        }
                        else
                        {

                            component.SendInDirection(MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 1), true, true);
                        }
                    }
                }

            }
            else if (isDead == false)
            {
                for (int i = riftakerAffectedProjectiles.Count - 1; i > -1; i--)
                {
                    if (riftakerAffectedProjectiles[i].enabled)
                    {
                        this.AdjustRigidbodyVelocity(riftakerAffectedProjectiles[i].Projectile.specRigidbody);
                    }
                }
                for (int i = 0; i < StaticReferenceManager.AllProjectiles.Count; i++)
                {
                    if (StaticReferenceManager.AllProjectiles[i].gameObject.activeSelf)
                    {
                        if (StaticReferenceManager.AllProjectiles[i].enabled)
                        {
                            this.DoQuickCheck(StaticReferenceManager.AllProjectiles[i].specRigidbody);
                        }
                    }
                }
            }
        }

        public float damageRadius;
        public float gravitationalForce;
        public float radius;
        private float radiusSquared;

        private bool AdjustRigidbodyVelocity(SpeculativeRigidbody other)
        {
            Vector2 a = other.UnitCenter - this.projectile.sprite.WorldCenter;
            float num = Vector2.SqrMagnitude(a);
            if (num < this.radiusSquared)
            {
                float g = this.gravitationalForce;
                Vector2 velocity = other.Velocity;
                Projectile projectile = other.projectile;
                if (projectile)
                {
                    bool playerOwner = projectile.Owner is PlayerController;
                    if (playerOwner == false)
                    {
                        return false;
                    }
                    if (other.GetComponent<BlackHoleDoer>() != null)
                    {
                        return false;
                    }
                    if (other.GetComponent<ParticleCollapserSmallProjectile>() != null)
                    {
                        return false;
                    }
                    if (other.GetComponent<ParticleCollapserLargeProjectile>() != null)
                    {
                        return false;
                    }
                    if (other.GetComponent<RiftakerProjectile>() != null)
                    {
                        return false;
                    }
                    var c = other.GetComponent<RiftakerAffectedProjectile>();

                    if (c == null)
                    {
                        return false;
                    }

                    if (c.HeadTo == null)
                    {
                        c.HeadTo = this;
                    }
                    if (c.HeadTo != this)
                    {
                        return false;
                    }

                    if (velocity == Vector2.zero)
                    {
                        return false;
                    }
                    g = this.gravitationalForce;
                }

                Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(other.UnitCenter, Mathf.Sqrt(num), g, projectile.baseData.speed);
                float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
                Vector2 b = frameAccelerationForRigidbody * d;
                Vector2 vector = velocity + b;
                if (BraveTime.DeltaTime > 0.02f)
                {
                    vector *= 0.02f / BraveTime.DeltaTime;
                }
                //other.Velocity = vector;
                if (projectile != null)
                {
                    if (vector != Vector2.zero)
                    {
                        //projectile.Speed = Mathf.Max(vector.magnitude * 0.5f, 12f);
                        projectile.Direction = vector.normalized;
                        other.Velocity = projectile.Direction * projectile.baseData.speed;

                        if (num < 1.25f)
                        {
                            var c = projectile.gameObject.GetComponent<RiftakerAffectedProjectile>();
                            projectile.baseData.range += 25;
                            projectile.SkipDistanceElapsedCheck = true;
                            if (c != null)
                            {
                                c.Amount++;
                                c.Warp(this, e_Sound <= 0);
                                if (c.Amount >= 15)
                                {
                                    projectile.DieInAir();
                                }
                            }
                            if (e_Sound <= 0)
                            {
                                e_Sound = 0.666f;
                            }
                        }
                        if (projectile.shouldRotate && (vector.x != 0f || vector.y != 0f))
                        {
                            float num2 = BraveMathCollege.Atan2Degrees(projectile.Direction);
                            if (!float.IsNaN(num2) && !float.IsInfinity(num2))
                            {
                                Quaternion rotation = Quaternion.Euler(0f, 0f, num2);
                                if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y))
                                {
                                    projectile.transform.rotation = rotation;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public bool DoQuickCheck(SpeculativeRigidbody other)
        {
            Vector2 a = other.UnitCenter - this.projectile.sprite.WorldCenter;
            float num = Vector2.SqrMagnitude(a);
            if (num < this.radiusSquared)
            {
                float g = this.gravitationalForce;
                Vector2 velocity = other.Velocity;
                Projectile projectile = other.projectile;
                if (projectile)
                {
                    bool playerOwner = projectile.Owner is PlayerController;
                    if (playerOwner == false)
                    {
                        return false;
                    }
                    if (other.GetComponent<BlackHoleDoer>() != null)
                    {
                        return false;
                    }
                    if (other.GetComponent<ParticleCollapserSmallProjectile>() != null)
                    {
                        return false;
                    }
                    if (other.GetComponent<ParticleCollapserLargeProjectile>() != null)
                    {
                        return false;
                    }
                    if (other.GetComponent<RiftakerProjectile>() != null)
                    {
                        return false;
                    }
                    if (other.GetComponent<RiftakerAffectedProjectile>() != null)
                    {
                        return false;
                    }
                    if (velocity == Vector2.zero)
                    {
                        return false;
                    }
                    g = this.gravitationalForce;
                }

                Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(other.UnitCenter, Mathf.Sqrt(num), g, projectile.baseData.speed);
                float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
                Vector2 b = frameAccelerationForRigidbody * d;
                Vector2 vector = velocity + b;
                if (BraveTime.DeltaTime > 0.02f)
                {
                    vector *= 0.02f / BraveTime.DeltaTime;
                }
                //other.Velocity = vector;
                if (projectile != null)
                {
                    if (vector != Vector2.zero)
                    {
                        projectile.Direction = vector.normalized;
                        other.Velocity = projectile.Direction * projectile.baseData.speed;
                        //projectile.Speed = Mathf.Max(vector.magnitude * 0.5f, 12f);

                        //other.Velocity = projectile.Direction * projectile.Speed;

                        if (num < 1.25f)
                        {
                            var c = projectile.gameObject.GetComponent<RiftakerAffectedProjectile>();
                            projectile.baseData.range += 25;
                            projectile.SkipDistanceElapsedCheck = true;
                            if (c != null)
                            {
                                c.Amount++;
                                c.Warp(this, e_Sound <= 0);
                                if (c.Amount >= 15)
                                {
                                    projectile.DieInAir();
                                }
                            }
                            else
                            {
                                c = projectile.gameObject.AddComponent<RiftakerAffectedProjectile>();
                                c.Projectile = projectile;
                                c.Warp(this, e_Sound <= 0);
                            }

                            if (e_Sound <= 0)
                            {
                                e_Sound = 0.666f;
                            }

                        }

                        if (projectile.shouldRotate && (vector.x != 0f || vector.y != 0f))
                        {
                            float num2 = BraveMathCollege.Atan2Degrees(projectile.Direction);
                            if (!float.IsNaN(num2) && !float.IsInfinity(num2))
                            {
                                Quaternion rotation = Quaternion.Euler(0f, 0f, num2);
                                if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y))
                                {
                                    projectile.transform.rotation = rotation;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }


        private Vector2 GetFrameAccelerationForRigidbody(Vector2 unitCenter, float currentDistance, float g, float Speed)
        {
            Vector2 zero = Vector2.zero;
            float num = Mathf.Clamp01(1f - currentDistance / this.radius);
            float d = g * num * num;
            Vector2 normalized = (this.projectile.sprite.WorldCenter - unitCenter).normalized;
            return (normalized * d) * Mathf.Max(1, (Speed * 0.35f));
        }


        private IEnumerator HoldPortalOpen(MeshRenderer portal)
        {
            float elapsed = 0f;
            float duration = 2;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                float t = Mathf.Sin(elapsed * 0.67f);
                if (portal == null) { yield break; }
                if (portal.gameObject == null) { yield break; }
                portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(elapsed / 2f, 0, t));
                portal.material.SetFloat("_HoleEdgeDepth", Mathf.Lerp(12, 2, t));

                if (player != null)
                {
                    float Rad = portal.material.GetFloat("_UVDistCutoff");
                    float num = player != null ? player.stats.GetStatValue(PlayerStats.StatType.Damage) : 1;
                    List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    Vector2 centerPosition = this.transform.position;
                    if (activeEnemies != null && activeEnemies.Count >= 0)
                    {
                        for (int i = 0; i < activeEnemies.Count; i++)
                        {
                            /*
                            AIActor aiactor = activeEnemies[i];
                            bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < Rad * 16 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null;
                            if (ae)
                            {
                                aiactor.healthHaver.ApplyDamage((20f * num) * BraveTime.DeltaTime, Vector2.zero, "fwomp", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                                if (player.PlayerHasActiveSynergy("Event Horizon"))
                                {
                                    if (!aiactor.healthHaver.IsBoss)
                                    {
                                        aiactor.knockbackDoer.weight = 150f;
                                        Vector2 a = aiactor.transform.position - portal.transform.position;
                                        aiactor.knockbackDoer.ApplyKnockback(-a, 1.33f * (Vector2.Distance(portal.transform.position, aiactor.transform.position) + 0.005f), false);
                                    }
                                }
                            }
                            */
                        }
                    }
                }

                else
                {
                    if (portal.gameObject != null) { Destroy(portal.gameObject); }
                    yield break;
                }
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_WPN_blackhole_impact_01", portal.gameObject);
            if (portal.gameObject != null) { Destroy(portal.gameObject); }
            yield break;
        }

        public Projectile projectile;
        private PlayerController player;

    }
}

