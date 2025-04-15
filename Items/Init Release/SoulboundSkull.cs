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
            //string resourceName = "Planetside/Resources/soulboundskull.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<SoulboundSkull>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("soulboundskull"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Spirit Vessel";
            string longDesc = "A skull of lead, wrapped in old prayer beads." +
                "\n\nIt seems to absorb souls only to shortly release them...\n\n LIII : XIX";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.B;
            SoulboundSkull.SoulboundSkullID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1.5f, StatModifier.ModifyMethod.ADDITIVE);


            Gun gun4 = PickupObjectDatabase.GetById(43) as Gun;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun4.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 8f;
            projectile.baseData.speed = 18f;
            projectile.sprite.renderer.enabled = false;
            PierceProjModifier spook = projectile.gameObject.AddComponent<PierceProjModifier>();
            spook.penetration = 2;

            projectile.gameObject.AddComponent<PierceDeadActors>();

            HomingModifier homing = projectile.gameObject.AddComponent<HomingModifier>();
            homing.HomingRadius = 250f;
            homing.AngularVelocity = 120f;

            projectile.baseData.UsesCustomAccelerationCurve = true;
            projectile.baseData.AccelerationCurve = AnimationCurve.EaseInOut(0, 0, 1.5f, 1);

            projectile.hitEffects.overrideMidairDeathVFX = StaticVFXStorage.SpookySkullVFX;


            TrailRenderer tr;
            var tro = projectile.gameObject.AddChild("trail object");
            tro.transform.position = projectile.transform.position;
            tro.transform.localPosition = new Vector3(0f, 0f, 0f);
            tr = tro.AddComponent<TrailRenderer>();
            tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tr.receiveShadows = false;
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetColor("_Color", new Color(0f, 1f, 3f, 0.7f));
            tr.material = mat;
            tr.time = 0.3f;
            tr.minVertexDistance = 0.1f;
            tr.startWidth = 0.1f;
            tr.endWidth = 0f;
            tr.startColor = Color.white;
            tr.endColor = new Color(0f, 1f, 3f, 0f);

            wispProjectile = projectile;
        }
        public static Projectile wispProjectile;

        public static int SoulboundSkullID;
        private void OnEnemyDamaged(float damage, bool fatal, HealthHaver enemy)
        {
            if (enemy.aiActor)
            {
                if (fatal == true) 
                { this.Spited(enemy.sprite.WorldCenter, true);
                    enemy.aiActor.PlayEffectOnActor(StaticVFXStorage.SpookySkullVFX, new Vector3());}
                else if (UnityEngine.Random.value <= Mathf.Min(0.5f, (damage * 0.01f)) && base.Owner)
                {
                    base.Owner.PlayEffectOnActor(StaticVFXStorage.SpookySkullVFX, new Vector3()); 
                    this.Spited(base.Owner.sprite.WorldCenter);
                }        
            }
        }
        public int MaxTokens = 12;
        public int currentTokens = 0;
        private float e = 0f;

        public override void Update()
        {
            base.Update();
            if (base.Owner)
            {
                if (currentTokens < MaxTokens)
                {
                    e += Time.deltaTime * 1.5f;
                    if (e > 1f)
                    {
                        e = 0f;
                        currentTokens++;
                    }
                }
            }
        }

        public void Spited(Vector3 position, bool Skip = false)
        {
            if (Skip == false)
            {
                if (currentTokens <= 0) { return; }
                currentTokens--;
            }
            GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(wispProjectile.gameObject, position, Quaternion.Euler(0f, 0f, BraveUtility.RandomAngle()), true);
            Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
            if (component != null)
            {
                component.Owner = base.Owner;
                component.Shooter = base.Owner.specRigidbody;
                component.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(component.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));
            }
        }
        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody && otherRigidbody.healthHaver && otherRigidbody != null)
            {     
                if (base.Owner != null)
                {
                    if (base.Owner.activeItems != null)
                    {

                        foreach (PlayerItem playerItem in base.Owner.activeItems)
                        {
                            if (playerItem != null)
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
                                        this.remainingTimeCooldown.SetValue(playerItem, noom - timeCooldown * 0.01f);
                                        this.remainingDamageCooldown.SetValue(playerItem, eee - damageCooldown * 0.01f);
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
		public override void OnDestroy()
		{
            if (base.Owner != null)
            {
                base.Owner.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Remove(base.Owner.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            }
            base.OnDestroy();
		}
	}
}