using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;
using Random = System.Random;


namespace Planetside
{
    public class StickyProjectile : MonoBehaviour
    {
        public StickyProjectile()
        {
            this.sourceVector = Vector2.zero;
            this.shouldExplodeOnReload = false;
            this.maxLifeTime = 15;
            this.destroyOnGunChanged = false;
            explosionDamageBasedOnProjectileDamage = false;
            hasDetTimer = false;
            shouldExplode = false;
            explosionData = null;
        }
        private void Start()
        {
            sourceProjectile = base.GetComponent<Projectile>();
            if (sourceProjectile.Owner is PlayerController)
            {
                player = sourceProjectile.Owner as PlayerController;
            }
            sourceGun = player.CurrentGun;
            sourceProjectile.OnHitEnemy += OnHit;

        }
        private void OnHit(Projectile self, SpeculativeRigidbody enemy, bool fatal)
        {
            sourceVector = self.LastVelocity;
            if (enemy.aiActor && enemy.aiActor.healthHaver)
            {
                StickProjectileToEnemy sticky = enemy.aiActor.gameObject.AddComponent<StickProjectileToEnemy>();
                sticky.destroyOnGunChanged = destroyOnGunChanged;
                sticky.shouldExplodeOnReload = shouldExplodeOnReload;
                sticky.explosionDamageBasedOnProjectileDamage = explosionDamageBasedOnProjectileDamage;
                sticky.explosionData = explosionData;
                sticky.maxLifeTime = maxLifeTime;
                sticky.sourceProjectile = sourceProjectile;
                sticky.sourceVector = sourceVector;
                sticky.player = player;
                sticky.sourceGun = sourceGun;
                sticky.hasDetTimer = hasDetTimer;
                sticky.detTimer = detTimer;
                sticky.shouldExpolode = shouldExplode;
                sticky.AttachPosition = self.transform.PositionVector2();
            }
        }
        public bool destroyOnGunChanged;
        public bool shouldExplodeOnReload;
        public bool explosionDamageBasedOnProjectileDamage;
        public bool hasDetTimer;
        public bool shouldExplode;
        public ExplosionData explosionData;
        public float maxLifeTime;
        public float detTimer;
        private Projectile sourceProjectile;
        private Vector2 sourceVector;
        private PlayerController player;
        private Gun sourceGun;
    }
    public class StickProjectileToEnemy : MonoBehaviour
    {
        private void Start()
        {
            CreateSprite(sourceProjectile);
            /*
            Library.stickiesAlive.Add(this);
            if (Library.stickiesAlive.Count >= 15)
            {
                StickProjectileToEnemy oldestSticky = Library.stickiesAlive.First();
                if (oldestSticky.shouldExpolode)
                {
                    oldestSticky.Explode();
                }
                else
                {
                    Destroy(oldestSticky);
                }
            }
            */
            if (explosionData == null)
            {
                explosionData = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
                explosionData.damageToPlayer = 0;
                explosionData.preventPlayerForce = true;
            }

        }
        private void CreateSprite(Projectile source)
        {
            obj = new GameObject("sticky proj");

            obj.layer = source.gameObject.layer + 1;
            tk2dSprite sprite = obj.AddComponent<tk2dSprite>();
            sprite.SetSprite(source.sprite.Collection, source.sprite.spriteId);
            base.GetComponent<tk2dSprite>().AttachRenderer(sprite);

            sprite.IsPerpendicular = true;
            sprite.HeightOffGround = 0.1f;
            sprite.transform.rotation = Quaternion.Euler(0, 0, sourceVector.ToAngle());
            sprite.transform.parent = base.GetComponent<SpeculativeRigidbody>().transform;
            sprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
            sprite.depthUsesTrimmedBounds = true;
            sprite.UpdateZDepth();
            sprite.transform.position = base.transform.position + MathToolbox.GetUnitOnCircle(source.Direction.ToAngle(), 0.625f).ToVector3ZisY();
            stickyPrefab = UnityEngine.Object.Instantiate<GameObject>(obj, base.transform.position + MathToolbox.GetUnitOnCircle(source.Direction.ToAngle(), 0.625f).ToVector3ZisY(), Quaternion.Euler(0f, 0f, 0f));
            SpriteOutlineManager.AddOutlineToSprite(sprite, Color.black);
            base.GetComponent<tk2dBaseSprite>().UpdateZDepth();
            if (shouldExplodeOnReload)
            {
                if (!shouldExpolode) { shouldExpolode = true; }
                if (sourceGun != null)
                {
                    sourceGun.OnReloadPressed += OnPlayerReloaded;
                }
            }
            if (destroyOnGunChanged)
            {
                if (player != null)
                {
                    player.GunChanged += RemoveOnSwitch;
                }
            }
            this.StartCoroutine(Decease(stickyPrefab, base.GetComponent<SpeculativeRigidbody>()));
        }
        private IEnumerator Decease(GameObject stickyPrefab, SpeculativeRigidbody target)
        {
            float timer = 0;
            while (timer < maxLifeTime)
            {
                timer += BraveTime.DeltaTime;
                stickyPrefab.transform.position = AttachPosition;
                stickyPrefab.transform.rotation = Quaternion.Euler(0, 0, sourceVector.ToAngle());
                stickyPrefab.GetComponent<tk2dBaseSprite>().HeightOffGround = base.GetComponent<tk2dBaseSprite>().HeightOffGround + (target.UnitHeight / 3) * 2 + 0.8f;
                stickyPrefab.GetComponent<tk2dBaseSprite>().UpdateZDepth();
                yield return null;
            }
            Destroy(stickyPrefab);
            Destroy(this);
            yield break;
        }
        private void Disconnect()
        {
            if (destroyOnGunChanged)
            {
                if (player != null)
                {
                    player.GunChanged -= RemoveOnSwitch;
                }
            }
            if (shouldExplodeOnReload)
            {
                if (sourceGun != null)
                {
                    sourceGun.OnReloadPressed -= OnPlayerReloaded;
                }
            }
        }
        private void Explode()
        {
            Exploder.Explode(base.GetComponent<AIActor>().CenterPosition, explosionData, Vector2.zero, null, true);
            shouldExpolode = false;
            Destroy(this);
        }
        private void RemoveOnSwitch(Gun arg1, Gun arg2, bool newGun)
        {
            Disconnect();
            Destroy(this);
        }
        private void OnPlayerReloaded(PlayerController arg1, Gun arg2, bool actual)
        {
            Disconnect();
            Explode();
        }
        private void OnDestroy()
        {
            if (destroyOnGunChanged)
            {
                if (player != null)
                {
                    player.GunChanged -= RemoveOnSwitch;
                }
            }
            if (shouldExplodeOnReload)
            {
                if (sourceGun != null)
                {
                    sourceGun.OnReloadPressed -= OnPlayerReloaded;
                }
            }
            if (shouldExpolode)
            {
                Exploder.Explode(base.GetComponent<AIActor>().CenterPosition, explosionData, Vector2.zero, null, true);
            }
            //Library.stickiesAlive.Remove(this);
            Destroy(stickyPrefab);
        }
        public bool destroyOnGunChanged;
        public bool shouldExplodeOnReload;
        public bool explosionDamageBasedOnProjectileDamage;
        public bool hasDetTimer;
        public bool shouldExpolode;
        public ExplosionData explosionData;
        public float maxLifeTime;
        public float detTimer;
        public Projectile sourceProjectile;
        public Vector2 sourceVector;
        public Vector2 AttachPosition;
        public PlayerController player;
        public Gun sourceGun;
        private GameObject stickyPrefab;
        private GameObject obj;


    }
}