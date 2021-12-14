using System;
using System.Collections;
using ItemAPI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using SaveAPI;

namespace Planetside
{
    public class SoulboundSkull : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Soul-Bound Skull";
            string resourceName = "Planetside/Resources/soulboundskull.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<SoulboundSkull>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Spirit Vessel";
            string longDesc = "A skull of lead, wrapped in old prayer beads." +
                "\n\nIt seems to absorb souls only to shortly release them...\n\n LIII : XIX";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.B;
            SoulboundSkull.SoulboundSkullID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);

        }
        public static int SoulboundSkullID;
        private void OnEnemyDamaged(float damage, bool fatal, HealthHaver enemy)
        {
            if (base.Owner != null)
            {
                if (enemy.specRigidbody != null)
                {
                    bool flag = enemy.aiActor && fatal;
                    if (flag)
                    {
                        this.Spited(enemy.sprite.WorldCenter);

                    }
                    else if (enemy.aiActor && UnityEngine.Random.value <= 0.05f)
                    {
                        this.Spited(base.Owner.sprite.WorldCenter);
                    }
                }
            }
        }
        public void Spited(Vector3 position)
        {
            Projectile projectile = ((Gun)ETGMod.Databases.Items[378]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 359)));
            Projectile component2 = gameObject.GetComponent<Projectile>();
            component2.Owner = base.Owner;
            component2.Shooter = base.Owner.specRigidbody;
            component2.baseData.damage = 7.5f;
            component2.baseData.speed = 1f;
            component2.sprite.renderer.enabled = false;
            PierceProjModifier spook = component2.gameObject.AddComponent<PierceProjModifier>();
            spook.penetration = 1;
            HomingModifier homing = component2.gameObject.AddComponent<HomingModifier>();
            homing.HomingRadius = 250f;
            homing.AngularVelocity = 120f;

            /*
            OtherTools.EasyTrailBullet trail = projectile.gameObject.AddComponent<OtherTools.EasyTrailBullet>();
            trail.TrailPos = component2.transform.position;
            trail.StartColor = Color.white;
            trail.StartWidth = 0.1f;
            trail.EndWidth = 0;
            trail.LifeTime = 1f;
            trail.BaseColor = new Color(0f, 1f, 3f, 1f);
            trail.EndColor = new Color(0f, 1f, 3f, 0f);
            */
            
            TrailRenderer tr;
            var tro = component2.gameObject.AddChild("trail object");
            tro.transform.position = component2.transform.position;
            tro.transform.localPosition = new Vector3(0f, 0f, 0f);
            tr = tro.AddComponent<TrailRenderer>();
            tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tr.receiveShadows = false;
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.mainTexture = _gradTexture;
            mat.SetColor("_Color", new Color(0f, 1f, 3f, 0.7f));
            tr.material = mat;
            tr.time = 0.3f;
            tr.minVertexDistance = 0.1f;
            tr.startWidth = 0.1f;
            tr.endWidth = 0f;
            tr.startColor = Color.white;
            tr.endColor = new Color(0f, 1f, 3f, 0f);
            
            component2.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));
            base.StartCoroutine(this.Speed(component2));

        }
        public Texture _gradTexture;

        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            bool flag = otherRigidbody && otherRigidbody.healthHaver && otherRigidbody != null;
            if (flag)
            {
               
                float maxHealth = otherRigidbody.healthHaver.GetMaxHealth();
                float num = maxHealth * 0.50f;
                float currentHealth = otherRigidbody.healthHaver.GetCurrentHealth();
                bool flag2 = currentHealth < num;
                if (flag2)
                {
                    float damage = myRigidbody.projectile.baseData.damage;
                    myRigidbody.projectile.baseData.damage *= 2f;
                    GameManager.Instance.StartCoroutine(this.ChangeProjectileDamage(myRigidbody.projectile, damage));
                }
                if (base.Owner != null)
                {
                    bool ee = base.Owner.activeItems == null;
                    if (!ee)
                    {

                        foreach (PlayerItem playerItem in base.Owner.activeItems)
                        {
                            bool aa = playerItem == null;
                            if (!aa)
                            {
                                float timeCooldown = playerItem.timeCooldown;
                                float damageCooldown = playerItem.damageCooldown;
                                try
                                {
                                    float noom = (float)this.remainingTimeCooldown.GetValue(playerItem);
                                    float eee = (float)this.remainingDamageCooldown.GetValue(playerItem);
                                    bool flag3 = eee <= 0f || eee <= 0f;
                                    if (!flag3)
                                    {
                                        this.remainingTimeCooldown.SetValue(playerItem, noom - timeCooldown * 0.015f);
                                        this.remainingDamageCooldown.SetValue(playerItem, eee - damageCooldown * 0.015f);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ETGModConsole.Log(ex.Message + ": " + ex.StackTrace, false);
                                }
                            }

                        }
                    }
                }          
            }
        }
        private FieldInfo remainingDamageCooldown = typeof(PlayerItem).GetField("remainingDamageCooldown", BindingFlags.Instance | BindingFlags.NonPublic);
        private FieldInfo remainingTimeCooldown = typeof(PlayerItem).GetField("remainingTimeCooldown", BindingFlags.Instance | BindingFlags.NonPublic);

        private IEnumerator ChangeProjectileDamage(Projectile bullet, float oldDamage)
        {
            yield return new WaitForSeconds(0.1f);
            bool flag = bullet != null;
            if (flag)
            {
                bullet.baseData.damage = oldDamage;
            }
            yield break;
        }
        public IEnumerator Speed(Projectile projectile)
        {
            bool flag = base.Owner != null;
            bool flag3 = flag;
            if (flag3)
            {
                for (int i = 0; i < 15; i++)
                {
                    projectile.baseData.speed += 1f;
                    projectile.UpdateSpeed();
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield break;
        }
        public override DebrisObject Drop(PlayerController player)
		{
            player.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Remove(player.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            DebrisObject result = base.Drop(player);
			return result;
		}
        public override void Pickup(PlayerController player)
		{
            player.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Combine(player.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            base.Pickup(player);
		}
		protected override void OnDestroy()
		{
            if (base.Owner != null)
            {
                base.Owner.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Remove(base.Owner.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            }
            base.OnDestroy();
		}
	}
}