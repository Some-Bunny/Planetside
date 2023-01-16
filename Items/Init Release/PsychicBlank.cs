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
using static Planetside.Bloat;


namespace Planetside
{

    public class PushImmunity : MonoBehaviour { }

    public class PsychicBlank  : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Psychic Dust";
            //string resourceName = "Planetside/Resources/psychicblank.png";
            GameObject obj = new GameObject(itemName);
			PsychicBlank activeitem = obj.AddComponent<PsychicBlank>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("psychicblank"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Not What It Looks Like...";
            string longDesc = "Gives very mild, yet still useful psychic abilities.\n\nDespite its very uncanny resemblance to something else, this dust IS actually magical, so totally nothing wrong with injesting some of it.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 5f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.D;
            activeitem.AddToSubShop(ItemBuilder.ShopType.OldRed, 1f);

            PsychicBlank.PsychicBlankID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

        }
        public static int PsychicBlankID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        public float CurrentAimDirection()
        {
            return base.LastOwner.CurrentGun.CurrentAngle;
        }

        public Vector2 CurrentAimVector(Vector2 position)
        {
            Vector2 normalized = (base.LastOwner.unadjustedAimPoint.XY() - position).normalized;
            return normalized;
        }

        public void DoMagicSparkles(int amount, tk2dBaseSprite sprite, float direction, float MinMult = 1, float MaxMult = 3, float DevMinus = 0, float DevPlus = 0)
        {
            for (int i = 0; i < amount; i++) 
            {
                Vector2 push = MathToolbox.GetUnitOnCircle(direction + UnityEngine.Random.Range(DevMinus, DevPlus), 1f);
                GlobalSparksDoer.DoSingleParticle(sprite.WorldCenter, push * UnityEngine.Random.Range(MinMult, MaxMult), null, 2, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
            }
        }

        private bool AdjustDebrisVelocity(DebrisObject debris)
        {
            if (debris.IsPickupObject)
            {
                return false;
            }
            if (debris.GetComponent<BlackHoleDoer>() != null)
            {
                return false;
            }
            Vector2 a = debris.sprite.WorldCenter - base.LastOwner.sprite.WorldCenter;
            float num = Vector2.SqrMagnitude(a);
            if (num > 256)
            {
                return false;
            }
            float num2 = Mathf.Sqrt(num);
            Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(debris.sprite.WorldCenter, num2, 40);
            float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
            if (debris.HasBeenTriggered)
            {
                debris.ApplyVelocity(frameAccelerationForRigidbody * d);
            }
            else if (num2 < 20/ 2f)
            {
                debris.Trigger(frameAccelerationForRigidbody * d, 0.5f, 1f);
            }
            return true;
        }
        private Vector2 GetFrameAccelerationForRigidbody(Vector2 unitCenter, float currentDistance, float g)
        {
            float num = Mathf.Clamp01(1f - currentDistance / 20);
            float d = g * num * num;
            return (base.LastOwner.sprite.WorldCenter - unitCenter).normalized * d;
        }

        protected override void DoEffect(PlayerController user)
        {
            RoomHandler roomHandler = user.CurrentRoom;
            List<AIActor> activeEnemies = roomHandler.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            Vector2 push = MathToolbox.GetUnitOnCircle(CurrentAimDirection(), 1f);

            DoMagicSparkles(20, user.sprite, CurrentAimDirection(), 2, 5, -10, 10);
            for (int i = 0; i < StaticReferenceManager.AllDebris.Count; i++)
            {
                this.AdjustDebrisVelocity(StaticReferenceManager.AllDebris[i]);
            }
            if (roomHandler != null)
            {
                if (activeEnemies != null && activeEnemies.Count > 0)
                {
                    AIActor actor = BraveUtility.RandomElement<AIActor>(activeEnemies);
                    if (actor.knockbackDoer)
                    {
                        actor.knockbackDoer.ApplyKnockback(push, 70);
                        DoMagicSparkles(15, actor.sprite, CurrentAimDirection(), 0.5f, 3, -15, 15);
                    }
                    foreach (AIActor enemy in activeEnemies)
                    {
                        if (enemy != actor)
                        {
                            if (enemy.knockbackDoer)
                            {
                                enemy.knockbackDoer.ApplyKnockback(push, 30);
                                DoMagicSparkles(10, enemy.sprite, CurrentAimDirection(), 0.5f, 2, -10, 10);
                            }
                        }
                    }
                }
            }
            int debrisCount = 5;

            for (int i = 0; i < StaticReferenceManager.AllMinorBreakables.Count; i++)
            {
                MinorBreakable self = StaticReferenceManager.AllMinorBreakables[i];
                {
                    if (self && self.IsBroken == false && self.GetComponent<PushImmunity>() == null)
                    {
                        if (UnityEngine.Random.value < 0.1f + (debrisCount * .19f))
                        {
                            float Dist = Vector2.Distance(self.sprite.WorldCenter, user.sprite.WorldCenter);
                            if (Dist < 4)
                            {
                                if (UnityEngine.Random.value < Mathf.Lerp(0, 1, Dist / 2))
                                {
                                    debrisCount--;
                                    debrisCount = Mathf.Max(0, debrisCount);
                                    self.StartCoroutine(this.Spin(self, user));
                                }
                            }
                        }
                    }
                }
            }

            int bulletCount = 10;
            for (int i = 0; i < StaticReferenceManager.AllProjectiles.Count; i++)
            {
                Projectile self = StaticReferenceManager.AllProjectiles[i];
                {
                    if (self && self.GetComponent<PushImmunity>() == null)
                    {
                        float Dist = Vector2.Distance(self.sprite.WorldCenter, user.sprite.WorldCenter);
                        if (Dist < 3)
                        {
                            if (UnityEngine.Random.value < 0.05f + (bulletCount * .095f)){
                                if (UnityEngine.Random.value < Mathf.Lerp(0, 1, Dist / 2))
                                {
                                    bulletCount--;
                                    bulletCount = Mathf.Max(0, bulletCount);
                                    self.StartCoroutine(this.ProjectileMagic(self, user));
                                }
                            }
                        }
                    }
                }
            }
            int majorCount = 2;

            for (int i = 0; i < StaticReferenceManager.AllMajorBreakables.Count; i++)
            {
                MajorBreakable self = StaticReferenceManager.AllMajorBreakables[i];
                {
                    if (self && self.IsDestroyed == false && self.GetComponent<PushImmunity>() == null)
                    {
                        if (UnityEngine.Random.value < 0.05f + (majorCount * .475f))
                        {
                            float Dist = Vector2.Distance(self.GetAnySprite()?.WorldCenter ?? self.transform.PositionVector2(), user.sprite.WorldCenter);
                            if (Dist < 5)
                            {
                                majorCount--;
                                majorCount = Mathf.Max(0, majorCount);
                                self.StartCoroutine(this.DoPushWeak(self));
                            }
                        }
                    }
                }
            }
        }

        public IEnumerator DoPushWeak(MajorBreakable self)
        {
            float e = 0;
            float d = UnityEngine.Random.Range(0.7f, 3f);
            Vector2 lerpFrom = CurrentAimVector(self.sprite.WorldCenter);
            DoMagicSparkles(10, self.sprite, CurrentAimDirection(), 1f, 3, -20, 20);

            while (e < d)
            {
                if (self == null) { yield break; }
                e += BraveTime.DeltaTime;
                self.specRigidbody.Velocity = Vector2.Lerp(lerpFrom, Vector2.zero, e / d);
                yield return null;
            }
            yield break;
        }

        public IEnumerator ProjectileMagic(Projectile self, PlayerController user)
        {
            if (self == null) { yield break; }
            float e = 0;
            float d = UnityEngine.Random.Range(0.01f, 0.15f);
            float dmg = self.baseData.damage;

            while (e < d)
            {
                if (self == null) { yield break; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            if (self == null) { yield break; }
            DoMagicSparkles(6, self.sprite, CurrentAimDirection(), 1f, 3, -20, 20);
            AkSoundEngine.PostEvent("Play_WPN_skullgun_shot_03", self.gameObject);

            float sp = self.baseData.speed;
            LazyReflectBullet(self, user, self.baseData.speed);
            self.ResetDistance();
            self.baseData.UsesCustomAccelerationCurve = true;
            self.baseData.CustomAccelerationCurveDuration = 2f;
            self.baseData.range = 1000;
            self.baseData.AccelerationCurve = new AnimationCurve() 
            { 
                postWrapMode = WrapMode.ClampForever,
                
                keys = new Keyframe[] {
                new Keyframe(){time = 0, value = 1, inTangent = 0.75f, outTangent = 0.25f},
                new Keyframe(){time = 0.5f, value = 0},
                new Keyframe(){time = 0.95f, value = 3, inTangent = 0.75f, outTangent = 0.25f}
                }          
            };

            e = 0; d = 1f;
            while (e < d)
            {
                if (self == null) { yield break; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            self.ResetDistance();
            GameObject vfx = Instantiate(StaticVFXStorage.SpookySkullVFX, self.sprite.WorldCenter, Quaternion.identity); Destroy(vfx, 2);
            self.SendInDirection(CurrentAimVector(self.sprite.WorldCenter), true, true);
            
            self.baseData.damage = dmg * Mathf.Max(self.baseData.speed / 15, 1);

            yield break;
        }



        public IEnumerator Spin(MinorBreakable self, PlayerController user)
        {
            float e = 0;
            float d = UnityEngine.Random.Range(0.01f, 0.5f);
            while (e < d)
            {
                if (self == null) { yield break; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            if (self == null) { yield break; }
            DoMagicSparkles(6, self.sprite, CurrentAimDirection(), 1f, 3, -20, 20);
            float fuck = 1;
            AkSoundEngine.PostEvent("Play_WPN_skullgun_shot_03", self.gameObject);

            AIActor enemy = user.CurrentRoom.GetNearestEnemy(user.sprite.WorldCenter, out fuck, new List<AIActor>() { });


            Projectile projectile = self.gameObject.AddComponent<Projectile>();
            projectile.Shooter = user.specRigidbody;
            projectile.Owner = user;
            projectile.baseData.damage = 10;
            projectile.baseData.range = 1000f;
            projectile.baseData.speed = 30f;
            projectile.baseData.force = 50f;
            

            projectile.baseData.UsesCustomAccelerationCurve = true;
            projectile.baseData.AccelerationCurve = AnimationCurve.EaseInOut(0, 0, 1f, 1);
            projectile.baseData.CustomAccelerationCurveDuration = 1;
            projectile.shouldRotate = false;

            projectile.pierceMinorBreakables = true;
            projectile.baseData.force = 50f;
            if (projectile.specRigidbody == null) { Destroy(projectile.gameObject); }
            projectile.specRigidbody.CollideWithTileMap = true;
            projectile.specRigidbody.Reinitialize();


            projectile.collidesWithEnemies = true;

            PixelCollider p = new PixelCollider();
            p = projectile.specRigidbody.PrimaryPixelCollider;
            p.CollisionLayer = CollisionLayer.Projectile;
            projectile.specRigidbody.PixelColliders = new List<PixelCollider>();
            projectile.specRigidbody.PixelColliders.Add(p);

            projectile.specRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.Projectile;
            projectile.Start();
            projectile.projectileHitHealth = 20;
            projectile.UpdateCollisionMask();

            projectile.transform.parent = null;


            projectile.OnDestruction += Projectile_OnDestruction;

            projectile.DestroyMode = Projectile.ProjectileDestroyMode.Destroy;
            projectile.specRigidbody.CollideWithOthers = true;

            projectile.GetComponent<MinorBreakable>().enabled = false;

            GameObject vfx = Instantiate(StaticVFXStorage.SpookySkullVFX, projectile.sprite.WorldCenter, Quaternion.identity); Destroy(vfx, 2);


            // projectile.StartCoroutine(this.Spin(projectile));

            //Vector2 direction = MathToolbox.GetUnitOnCircle(CurrentAimDirection(), 1);
            //if (enemy != null) { direction = enemy.sprite.WorldCenter - self.sprite.WorldCenter; }
            //if (projectile.specRigidbody) projectile.specRigidbody.Initialize();

            projectile.SendInDirection(CurrentAimVector(projectile.sprite.WorldCenter), true, true);
            user.DoPostProcessProjectile(projectile);
            yield break;
        }


        private void Projectile_OnDestruction(Projectile obj)
        {
            var j = obj.GetComponent<MinorBreakable>();
            if (j)
            {
                obj.GetComponent<MinorBreakable>().enabled = true;
                j.Break(); GameObject vfx = Instantiate(StaticVFXStorage.SpookySkullVFX, obj.sprite.WorldCenter, Quaternion.identity); Destroy(vfx, 2); Destroy(j.gameObject, 1);
            
            
            }
        }

        public static void LazyReflectBullet(Projectile p, GameActor newOwner, float minReflectedBulletSpeed, float scaleModifier = 1f, float damageModifier = 1f, float spread = 0f)
        {
            p.RemoveBulletScriptControl();
            AkSoundEngine.PostEvent("Play_OBJ_metalskin_deflect_01", GameManager.Instance.gameObject);
            
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
            p.baseData.damage *= damageModifier;
            if (p.baseData.damage < 10f)
            {
                p.baseData.damage = 15f;
            }
            p.UpdateCollisionMask();
            p.ResetDistance();
            p.Reflected();
        }

    }
}



