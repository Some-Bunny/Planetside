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
	public class WitherLanceProjectile : MonoBehaviour
	{

        public enum Types
        {
            NORMAL,
            BLAST,
            GREEN,
            FAST,
            SPARKLY
        }

        public Types ownType;

		public WitherLanceProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile)
			{
				this.projectile.baseData.speed *= UnityEngine.Random.Range(0.7f, 1.7f);

				//this.projectile.collidesWithProjectiles = true;
				//this.projectile.collidesOnlyWithPlayerProjectiles = true;
                //this.projectile.UpdateCollisionMask();
                this.projectile.StartCoroutine(Wait());
                //this.projectile.specRigidbody.OnPreRigidbodyCollision += HandlePreCollision;
            }
        }

        public float AdditionalDamage = 1;
        public float SpeedMinimum = 2;
        public float SpeedMaximum = 4;


        private bool doMagic = false;

        public IEnumerator Wait()
        {
            float f = 0;
            while (f < 0.5f)
            {
                f += BraveTime.DeltaTime;
                yield return null;
            }
            doMagic = true;
            this.projectile.baseData.AccelerationCurve = null;
            this.projectile.baseData.UsesCustomAccelerationCurve = false;
            yield break;
        }

        public void OnDestroy()
        {
            if (this.gameObject)
            {
                var transform = this.gameObject.transform.Find("trail object");
                if (transform) 
                {
                    transform.parent = null;
                    Destroy(transform.gameObject, 2.5f);
                }
            }
        }

        private bool Detached = false;

        public void Update()
        {
            if (Active == true)
            {
                if (SpecialProjectile == null && Detached == false)
                {
                    Detached = true;
                    projectile.OverrideMotionModule = null;

                    var self = projectile;
                    switch (ownType)
                    {
                        case WitherLanceProjectile.Types.BLAST:
                            self.baseData.damage *= 1.2f;
                            self.damageTypes = CoreDamageTypes.Fire;

                            return;
                        case WitherLanceProjectile.Types.FAST:
                            HomingModifier HomingMod = self.gameObject.GetOrAddComponent<HomingModifier>();
                            HomingMod.AngularVelocity += 120;
                            HomingMod.HomingRadius += 3;
                            PierceProjModifier spook = self.gameObject.GetOrAddComponent<PierceProjModifier>();
                            spook.penetration += 2;
                            spook.penetratesBreakables = true;
                            return;
                        case WitherLanceProjectile.Types.NORMAL:
                            self.baseData.range += 5;
                            BounceProjModifier BounceProjMod = self.gameObject.GetOrAddComponent<BounceProjModifier>();
                            BounceProjMod.bouncesTrackEnemies = false;
                            BounceProjMod.numberOfBounces += 2;
                            return;
                        case WitherLanceProjectile.Types.SPARKLY:
                            self.PenetratesInternalWalls = true;
                            self.BlackPhantomDamageMultiplier *= 1.5f;
                            self.BossDamageMultiplier *= 1.2f;
                            return;
                        case WitherLanceProjectile.Types.GREEN:
                            WraparoundProjectile wrap = self.gameObject.GetOrAddComponent<WraparoundProjectile>();
                            wrap.Cap += 1;
                            wrap.OnWrappedAround = (proj, pos1, pos2) =>
                            {
                                var h = Instantiate((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect, pos1, Quaternion.identity);
                                var h2 = Instantiate((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect, pos1, Quaternion.identity);
                                Destroy(h, 2);
                                Destroy(h2, 2);
                            };
                            return;
                    }

                }
            }
            else
            {
                foreach (Projectile proj in StaticReferenceManager.AllProjectiles)
                {
                    if (proj.GetComponent<BasicBeamController>() != null && BeamToolbox.PosIsNearAnyBoneOnBeam(proj.GetComponent<BasicBeamController>(), proj.sprite.WorldCenter, 1.1f) && proj.Owner != null && proj.Owner is PlayerController)
                    {

                    }
                    else
                    {               
                        if (proj.GetComponent<WitherLanceProjectile>() == null && ReturnResult(proj) == true)
                        {
                            if (Vector2.Distance(proj.sprite.WorldCenter, projectile.sprite.WorldCenter) < 1.6f && proj.Owner != null && proj.Owner is PlayerController)
                            {
                                if (doMagic == true)
                                {
                                    var h = Instantiate(projectile.hitEffects.deathAny.effects.First().effects.First().effect, projectile.sprite.WorldCenter, Quaternion.identity);
                                    Destroy(h, 2);
                                    AkSoundEngine.PostEvent("Play_WPN_star_impact_01", projectile.gameObject);
                                    var attacher = proj.GetOrAddComponent<StarAttacherProjectileComponent>();
                                    attacher.ProcessAdd(this);
                                    proj.baseData.speed = Mathf.Max(proj.baseData.speed *= 0.8f, 6f);
                                    proj.UpdateSpeed();
                                    proj.baseData.damage *= 1.125f;
                                    proj.baseData.damage += 1f;
                                    HomingModifier HomingMod = proj.gameObject.GetOrAddComponent<HomingModifier>();
                                    HomingMod.AngularVelocity += 60;
                                    HomingMod.HomingRadius += 6;

                                    this.projectile.baseData.damage += AdditionalDamage;
                                    this.projectile.baseData.speed = 9;
                                    this.projectile.UpdateSpeed();

                                    Active = true;
                                    SpecialProjectile = proj.gameObject;
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool ReturnResult(Projectile p)
        {
            var comp = p.GetComponent<StarAttacherProjectileComponent>();
            if (comp == null)
            {
                return true;
            }
            else
            {
                return comp.Stars.Count < comp.AttachersCap;
            }
        }

        private GameObject SpecialProjectile;
        private bool Active = false;
        public Projectile projectile;
    }




    public class StarAttacherProjectileComponent : MonoBehaviour
    {
        public void Start()
        {
        }

        public void ProcessAdd(WitherLanceProjectile p)
        {

            var self = this.GetComponent<Projectile>();

            Stars.Add(p.projectile);
            p.projectile.OverrideMotionModule = new OrbitProjectileMotionModule()
            {
                alternateOrbitTarget = this.GetComponent<SpeculativeRigidbody>(),
                usesAlternateOrbitTarget = true,
                MaxRadius = 1.6f,
                MinRadius = 1.5f,
                lifespan = 9999,
            };
            for (int i = 0; i < Stars.Count; i++)
            {
                var pr = Stars[i];
                if (pr.OverrideMotionModule is OrbitProjectileMotionModule orbit)
                {
                    orbit.m_currentAngle = (360 / Stars.Count) * i;
                }
            }

           
        }
        public List<Projectile> Stars = new List<Projectile>();
        public int AttachersCap = 8;

    }
}

