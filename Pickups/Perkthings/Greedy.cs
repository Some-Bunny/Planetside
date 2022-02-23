using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections;

namespace Planetside
{

    class GreedController : MonoBehaviour
    {
        public GreedController()
        {
            this.LifeTime = 2.5f;
            this.StatIncrease = 1.10f;
            this.hasBeenPickedup = false;
        }
        public void Start() 
        { 
            this.hasBeenPickedup = true; 
            if(player)
            {
                player.OnKilledEnemyContext += this.OnKilledEnemy;
            }
        }
        public void OnKilledEnemy(PlayerController source, HealthHaver enemy)
        {
            if (enemy.aiActor != null && enemy.aiActor.IsHarmlessEnemy == false)
            {
                GameObject gameObject = SpawnManager.SpawnDebris(PickupObjectDatabase.GetById(68).gameObject, enemy.aiActor.transform.position, Quaternion.identity);
                CurrencyPickup component = gameObject.GetComponent<CurrencyPickup>();
                component.PreventPickup = true;
                PickupMover component2 = gameObject.GetComponent<PickupMover>();
                if (component2)
                {
                    component2.enabled = false;
                }
                DebrisObject orAddComponent = gameObject.GetOrAddComponent<DebrisObject>();
                DebrisObject debrisObject = orAddComponent;
                debrisObject.OnGrounded = (Action<DebrisObject>)Delegate.Combine(debrisObject.OnGrounded, new Action<DebrisObject>(delegate (DebrisObject sourceDebris)
                {
                    sourceDebris.GetComponent<CurrencyPickup>().PreventPickup = false;
                    sourceDebris.OnGrounded = null;
                    component.specRigidbody.OnTriggerCollision = (SpeculativeRigidbody.OnTriggerDelegate)Delegate.Combine(component.specRigidbody.OnTriggerCollision, new SpeculativeRigidbody.OnTriggerDelegate(delegate (SpeculativeRigidbody otherRigidbody, SpeculativeRigidbody rigidBody, CollisionData collisionData)
                    {
                        PlayerController player = otherRigidbody.GetComponent<PlayerController>();
                        if (player != null)
                        {
                            ExplosionData boomboom = StaticExplosionDatas.genericSmallExplosion;
                            boomboom.damageToPlayer = 0;
                            boomboom.preventPlayerForce = true;
                            boomboom.ignoreList.Add(player.specRigidbody);
                            Exploder.Explode(player.sprite.WorldCenter, boomboom, player.transform.PositionVector2());
                            player.StartCoroutine(GrantTemporaryBoost(player, this.StatIncrease));
                        }
                    }));
                }));
                orAddComponent.shouldUseSRBMotion = true;
                orAddComponent.angularVelocity = 0f;
                orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                orAddComponent.Trigger(new Vector3(0, 0), 0.05f, 1f);
                orAddComponent.canRotate = false;
                GameManager.Instance.Dungeon.StartCoroutine(HandleManualCoinSpawnLifespan(component, this.LifeTime));
            }
        }



        private static IEnumerator GrantTemporaryBoost(PlayerController user, float StatIncreaseValue)
        {
            StatModifier speed = new StatModifier
            {
                statToBoost = PlayerStats.StatType.MovementSpeed,
                amount = StatIncreaseValue,
                modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
            };
            StatModifier damage = new StatModifier
            {
                statToBoost = PlayerStats.StatType.Damage,
                amount = StatIncreaseValue,
                modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
            };
            user.ownerlessStatModifiers.Add(speed);
            user.ownerlessStatModifiers.Add(damage);
            user.stats.RecalculateStats(user, true, true);
            float elapsed = 0f;
            while (elapsed < 5)
            {
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            user.ownerlessStatModifiers.Remove(speed);
            user.ownerlessStatModifiers.Remove(damage);
            user.stats.RecalculateStats(user, true, true);
            yield break;
        }


        private static IEnumerator HandleManualCoinSpawnLifespan(CurrencyPickup coins, float lifeTime)
        {
            float elapsed = 0f;
            while (elapsed < lifeTime * 0.75f)
            {
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            float flickerTimer = 0f;
            while (elapsed < lifeTime)
            {
                elapsed += BraveTime.DeltaTime;
                flickerTimer += BraveTime.DeltaTime;
                if (coins != null && coins.renderer)
                {
                    bool enabled = flickerTimer % 0.2f > 0.15f;
                    coins.renderer.enabled = enabled;
                }
                else if (coins == null)
                {
                    yield break;
                }
                yield return null;
            }
            UnityEngine.Object.Destroy(coins.gameObject);
            yield break;
        }
        public void IncrementStack()
        {
            this.StatIncrease += 0.10f;
            this.LifeTime += 0.5f;
        }

        private float LifeTime;
        private float StatIncrease;
        public bool hasBeenPickedup;
        public PlayerController player;
    }
    class Greedy : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Greedy Perk";
            string resourcePath = "Planetside/Resources/PerkThings/Greedy.png";
            GameObject gameObject = new GameObject(name);
            Greedy item = gameObject.AddComponent<Greedy>();
          
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Makes you greedier.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            Greedy.GreedyID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.yellow;
            particles.ParticleSystemColor2 = new Color(255, 215, 0);
            OutlineColor = new Color(0.6f, 0.52f, 0.05f);

        }
        public static int GreedyID;
        private static Color OutlineColor;



        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            m_hasBeenPickedUp = true;
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);

            //player.OnKilledEnemyContext += this.OnKilledEnemy;

            GreedController greed = player.gameObject.GetOrAddComponent<GreedController>();
            greed.player = player;
            if (greed.hasBeenPickedup == true)
            { greed.IncrementStack(); }
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = greed.hasBeenPickedup == true ? "Greed Is Even Better." : "Greed Is Good.";
            OtherTools.Notify("Greedy", BlurbText, "Planetside/Resources/PerkThings/glass", UINotificationController.NotificationColor.GOLD);
            /*
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            OtherTools.Notify("Greedy", "Greed Is Good.", "Planetside/Resources/PerkThings/Greedy", UINotificationController.NotificationColor.GOLD);
            */
            UnityEngine.Object.Destroy(base.gameObject);
        }
        public void OnKilledEnemy(PlayerController source, HealthHaver enemy)
        {
            if (enemy.aiActor != null && enemy.aiActor.IsHarmlessEnemy == false)
            {
                GameObject gameObject = SpawnManager.SpawnDebris(PickupObjectDatabase.GetById(68).gameObject, enemy.aiActor.transform.position, Quaternion.identity);
                CurrencyPickup component = gameObject.GetComponent<CurrencyPickup>();
                component.PreventPickup = true;
                PickupMover component2 = gameObject.GetComponent<PickupMover>();
                if (component2)
                {
                    component2.enabled = false;
                }
                DebrisObject orAddComponent = gameObject.GetOrAddComponent<DebrisObject>();
                DebrisObject debrisObject = orAddComponent;
                debrisObject.OnGrounded = (Action<DebrisObject>)Delegate.Combine(debrisObject.OnGrounded, new Action<DebrisObject>(delegate (DebrisObject sourceDebris)   
                {
                    sourceDebris.GetComponent<CurrencyPickup>().PreventPickup = false;
                    sourceDebris.OnGrounded = null;
                    component.specRigidbody.OnTriggerCollision = (SpeculativeRigidbody.OnTriggerDelegate)Delegate.Combine(component.specRigidbody.OnTriggerCollision, new SpeculativeRigidbody.OnTriggerDelegate(delegate (SpeculativeRigidbody otherRigidbody, SpeculativeRigidbody rigidBody, CollisionData collisionData)
                    {
                        PlayerController player = otherRigidbody.GetComponent<PlayerController>();
                        if (player != null)
                        {
                            ExplosionData boomboom = StaticExplosionDatas.genericSmallExplosion;
                            boomboom.damageToPlayer = 0;
                            boomboom.preventPlayerForce = true;
                            boomboom.ignoreList.Add(player.specRigidbody);
                            Exploder.Explode(player.sprite.WorldCenter, StaticExplosionDatas.genericSmallExplosion, player.transform.PositionVector2());
                            player.StartCoroutine(GrantTemporaryBoost(player));
                        }
                    }));
                }));
                orAddComponent.shouldUseSRBMotion = true;
                orAddComponent.angularVelocity = 0f;
                orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                orAddComponent.Trigger(new Vector3(0,0), 0.05f, 1f);
                orAddComponent.canRotate = false;
                GameManager.Instance.Dungeon.StartCoroutine(HandleManualCoinSpawnLifespan(component, 2.5f));
            }
        }

      

        private static IEnumerator GrantTemporaryBoost(PlayerController user)
        {
            StatModifier speed = new StatModifier
            {
                statToBoost = PlayerStats.StatType.MovementSpeed,
                amount = 1.15f,
                modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
            };
            StatModifier damage = new StatModifier
            {
                statToBoost = PlayerStats.StatType.Damage,
                amount = 1.15f,
                modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
            };
            user.ownerlessStatModifiers.Add(speed);
            user.ownerlessStatModifiers.Add(damage);
            user.stats.RecalculateStats(user, true, true);
            float elapsed = 0f;
            while (elapsed < 5)
            {
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            user.ownerlessStatModifiers.Remove(speed);
            user.ownerlessStatModifiers.Remove(damage);
            user.stats.RecalculateStats(user, true, true);
            yield break;
        }


        private static IEnumerator HandleManualCoinSpawnLifespan(CurrencyPickup coins, float lifeTime)
        {
            float elapsed = 0f;
            while (elapsed < lifeTime * 0.75f)
            {
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            float flickerTimer = 0f;
            while (elapsed < lifeTime)
            {
                elapsed += BraveTime.DeltaTime;
                flickerTimer += BraveTime.DeltaTime;
                if (coins !=null && coins.renderer)
                {
                    bool enabled = flickerTimer % 0.2f > 0.15f;
                    coins.renderer.enabled = enabled;
                }
                else if (coins == null)
                {
                    yield break;
                }
                yield return null;
            }
            UnityEngine.Object.Destroy(coins.gameObject);
            yield break;
        }

        public float distortionMaxRadius = 30f;
        public float distortionDuration = 2f;
        public float distortionIntensity = 0.7f;
        public float distortionThickness = 0.1f;
        protected void Start()
        {
            try
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(this);
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            }
            catch (Exception er)
            {
                ETGModConsole.Log(er.Message, false);
            }
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite)
            {
                return 1000f;
            }
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return 1f;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (!interactor.CurrentRoom.IsRegistered(this) && !RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        private void Update()
        {
            if (!this.m_hasBeenPickedUp && !this.m_isBeingEyedByRat && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
            {
                GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
            }
        }

        public void Interact(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                RoomHandler.unassignedInteractableObjects.Remove(this);
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            this.Pickup(interactor);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        private bool m_hasBeenPickedUp;
    }
}
