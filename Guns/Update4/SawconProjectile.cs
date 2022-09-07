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
    public class SawconMagnetAffected : MonoBehaviour 
    {
        public void Start()
        {
            projectile = base.GetComponent<Projectile>();
            if (projectile != null)
            {
                player = projectile.Owner as PlayerController;
                if (player && player.PlayerHasActiveSynergy("Saw Your Heart Out"))
                {
                    projectile.baseData.damage *= 0.5f;
                    projectile.baseData.range *= 2f;
                    projectile.baseData.speed *= 1.2f;
                    projectile.AdditionalScaleMultiplier *= 0.7f;
                    projectile.UpdateSpeed();

                    BounceProjModifier bounce = projectile.GetOrAddComponent<BounceProjModifier>();
                    bounce.numberOfBounces += 10;

                    PierceProjModifier pierce = projectile.GetOrAddComponent<PierceProjModifier>();
                    pierce.penetration += 10;

                    ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
                    yes.spawnShadows = true;
                    yes.shadowLifetime = 0.125f;
                    yes.shadowTimeDelay = 0.01f;
                    yes.dashColor = new Color(0.78f, 0.78f, 0.8f, 0.2f);
                }
            }
        }
        public PlayerController player;
        private Projectile projectile;
    }


	internal class SawconProjectile : MonoBehaviour
	{

        public SawconProjectile()
        {
            this.damageRadius = 4;
            this.gravitationalForce = 25;
            this.radius = 40;
            this.radiusSquared = radius * radius;
        }

        public void Start()
        {
            projectile = base.GetComponent<Projectile>();
            if (projectile != null)
            {
                player = projectile.Owner as PlayerController;
                projectile.OnHitEnemy += HandleHit;

                if (player && player.PlayerHasActiveSynergy("Saw Your Heart Out"))
                {
                    projectile.baseData.speed *= 0.4f;
                    projectile.UpdateSpeed();

                    SawTime = 10;
                    projectile.baseData.damage *= 0.5f;

                    blueIndicator = (UnityEngine.Object.Instantiate(StaticVFXStorage.RadialRing, projectile.sprite.WorldCenter, Quaternion.identity, projectile.transform)).GetComponent<HeatIndicatorController>();
                    blueIndicator.CurrentColor = Color.cyan.WithAlpha(8f);
                    blueIndicator.IsFire = false;
                    blueIndicator.CurrentRadius = 4f;
                }
            }
        }

        public HeatIndicatorController blueIndicator;
    


        private void HandleHit(Projectile projectile, SpeculativeRigidbody otherBody, bool fatal)
        {
            if (otherBody.aiActor != null && !otherBody.healthHaver.IsDead && otherBody.aiActor.behaviorSpeculator && !otherBody.aiActor.IsHarmlessEnemy)
            {
                if (base.GetComponent<PierceProjModifier>() != null)
                {
                    if (base.GetComponent<PierceProjModifier>().penetration == 0)
                    { TransformToSticky(projectile, otherBody); }
                }
                else
                { TransformToSticky(projectile, otherBody); }
            }
        }

        private void TransformToSticky(Projectile projectile, SpeculativeRigidbody otherBody)
        {
            Damage = projectile.baseData.damage *= 0.5f;
            projectile.DestroyMode = Projectile.ProjectileDestroyMode.DestroyComponent;
            objectToLookOutFor = projectile.gameObject;
            objectToLookOutFor.transform.parent = otherBody.transform;
            parent = otherBody.GetComponent<AIActor>();

            AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Shield_01", objectToLookOutFor.gameObject);

            GameManager.Instance.StartCoroutine(this.ItsSawingTime(projectile.LastVelocity));
        }
        private IEnumerator ItsSawingTime(Vector2 lastVelocity)
        {
            float VelAngle = lastVelocity.ToAngle();

            if (objectToLookOutFor == null) { yield break; }
            Vector2 startPosition = MathToolbox.GetUnitOnCircle(VelAngle + 180, (Vector3.Distance(objectToLookOutFor.GetComponentInChildren<tk2dBaseSprite>().WorldCenter, parent.sprite.WorldCenter)/1.5f));
            AkSoundEngine.PostEvent("Play_OBJ_paydaydrill_loop_01", GameManager.Instance.gameObject);
            float elapsed = 0f;
            float duration = SawTime;
            Vector2 location = startPosition;
            while (elapsed < duration)
            {
                if (objectToLookOutFor.gameObject == null)
                {
                    AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", GameManager.Instance.gameObject);
                    break; }
                if (parent.gameObject == null)
                {
                    AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", GameManager.Instance.gameObject);
                    break; }

                elapsed += BraveTime.DeltaTime;
                float t = elapsed / duration;
                float tLerp = MathToolbox.SinLerpTValue(t);
                if (parent != null)
                {
                    location = Vector3.Lerp(parent.sprite.WorldCenter + startPosition, parent.sprite.WorldCenter, tLerp);

                    objectToLookOutFor.transform.position = location;
                    parent.healthHaver.ApplyDamage(Damage * BraveTime.DeltaTime, this.transform.PositionVector2(), "saw");
                    GlobalSparksDoer.DoSingleParticle(objectToLookOutFor.GetComponentInChildren<tk2dBaseSprite>().WorldCenter, MathToolbox.GetUnitOnCircle(VelAngle + UnityEngine.Random.Range(-10, 10), 10), null, null, null, GlobalSparksDoer.SparksType.BLOODY_BLOOD);
                    GlobalSparksDoer.DoSingleParticle(objectToLookOutFor.GetComponentInChildren<tk2dBaseSprite>().WorldCenter, MathToolbox.GetUnitOnCircle((VelAngle - 180) + UnityEngine.Random.Range(-10, 10), 10), null, null, null, GlobalSparksDoer.SparksType.BLOODY_BLOOD);
                }
                yield return null;
            }
            AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", GameManager.Instance.gameObject);

            if (blueIndicator != null)
            {
                blueIndicator.EndEffect();
            }

            ExplosionData data = StaticExplosionDatas.genericSmallExplosion;
            data.effect = (PickupObjectDatabase.GetById(601) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
            float damage = player != null ? player.PlayerHasActiveSynergy("Screwdriver") == true ? 20 : 6 : 6;
            data.damage = damage * (player != null ? player.stats.GetStatValue(PlayerStats.StatType.Damage) : 1);
            data.damageRadius = 3;
            data.doScreenShake = false;
            data.playDefaultSFX = false;
            data.force = 25;
            data.damageToPlayer = 0;
            Exploder.Explode(location, data, location);
            if (player && player.PlayerHasActiveSynergy("Screwdriver"))
            {
                GameObject gameObject = GameObject.Instantiate((GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost"), location, Quaternion.identity);
                Destroy(gameObject, 2f);
                List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (activeEnemies != null)
                {
                    for (int em = 0; em < activeEnemies.Count; em++)
                    {
                        AIActor aiactor = activeEnemies[em];
                        if (aiactor != null)
                        {
                            if (Vector2.Distance(aiactor.CenterPosition, location) < 4 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && aiactor.healthHaver.IsBoss == false && aiactor.healthHaver.IsSubboss == false)
                            {
                                aiactor.behaviorSpeculator.Stun(2.5f);
                            }
                        }
                    }
                }
            }
            if (objectToLookOutFor != null)
            {
                Destroy(objectToLookOutFor);
            }
            AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Shield_01", objectToLookOutFor.gameObject ?? GameManager.Instance.PrimaryPlayer.gameObject);

            yield break;
        }

        public void Update()
        {
            if (blueIndicator != null && base.gameObject != null)
            {

                blueIndicator.gameObject.layer = 22;

                blueIndicator.transform.position = base.gameObject.GetComponent<tk2dBaseSprite>() != null ? base.gameObject.GetComponent<tk2dBaseSprite>().WorldCenter : base.gameObject.GetComponentInChildren<tk2dBaseSprite>().WorldCenter;

            }

            if (player && player.PlayerHasActiveSynergy("Saw Your Heart Out") == true)
            {

                for (int i = 0; i < StaticReferenceManager.AllProjectiles.Count; i++)
                {

                    if (StaticReferenceManager.AllProjectiles[i].gameObject.activeSelf)
                    {

                        if (StaticReferenceManager.AllProjectiles[i].enabled)
                        {

                            if (StaticReferenceManager.AllProjectiles[i].GetComponent<SawconMagnetAffected>() != null | StaticReferenceManager.AllProjectiles[i].GetComponentInChildren<SawconMagnetAffected>() != null)
                            {
                                this.AdjustRigidbodyVelocity(StaticReferenceManager.AllProjectiles[i].specRigidbody);
                            }
                        }
                    }
                }
            }
        }

        public void OnDestroy()
        {
            if (blueIndicator != null)
            {
                blueIndicator.EndEffect();
            }
        }

        public float damageRadius;
        public float gravitationalForce;
        public float radius;
        private float radiusSquared;


        private bool AdjustRigidbodyVelocity(SpeculativeRigidbody other)
        {
            Vector2 h = this.gameObject.GetComponent<tk2dBaseSprite>() != null ? this.gameObject.GetComponent<tk2dBaseSprite>().WorldCenter : this.gameObject.GetComponentInChildren<tk2dBaseSprite>().WorldCenter;
            Vector2 a = other.UnitCenter - h;

            float num = Vector2.SqrMagnitude(a);
            if (num < this.radiusSquared)
            {
                float g = this.gravitationalForce;
                Vector2 velocity = other.Velocity;
                Projectile projectile = other.projectile;
                if (projectile)
                {
                    projectile.collidesWithPlayer = false;
                    if (other.GetComponent<BlackHoleDoer>() != null)
                    {
                        return false;
                    }
                    if (other.GetComponent<SawconProjectile>() != null)
                    {
                        return false;
                    }
                    if (velocity == Vector2.zero)
                    {
                        return false;
                    }
                    g = this.gravitationalForce;
                }

                Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(other.UnitCenter, Mathf.Sqrt(num), g);
                float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
                Vector2 b = frameAccelerationForRigidbody * d;
                Vector2 vector = velocity + b;
                if (BraveTime.DeltaTime > 0.02f)
                {
                    vector *= 0.02f / BraveTime.DeltaTime;
                }
                other.Velocity = vector;
                if (projectile != null)
                {
                    projectile.collidesWithPlayer = false;
                    if (projectile.IsBulletScript)
                    {
                        projectile.RemoveBulletScriptControl();
                    }
                    if (vector != Vector2.zero)
                    {
                        projectile.Direction = vector.normalized;
                        projectile.Speed = Mathf.Max(5f, vector.magnitude);
                        other.Velocity = projectile.Direction * projectile.Speed;
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
        private Vector2 GetFrameAccelerationForRigidbody(Vector2 unitCenter, float currentDistance, float g)
        {
            Vector2 h = this.gameObject.GetComponent<tk2dBaseSprite>() != null ? this.gameObject.GetComponent<tk2dBaseSprite>().WorldCenter : this.gameObject.GetComponentInChildren<tk2dBaseSprite>().WorldCenter;

            Vector2 zero = Vector2.zero;
            float num = Mathf.Clamp01(1f - currentDistance / this.radius);
            float d = g * num * num;
            Vector2 normalized = (h - unitCenter).normalized;
            return (normalized * d) * 9;
        }


        public float SawTime = 6;

        public PlayerController player;
        public float Damage;
        public GameObject objectToLookOutFor;
        public AIActor parent;
        private Projectile projectile;
	}
}

