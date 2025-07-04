using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SaveAPI;
using System.Linq;

namespace Planetside
{

    class GreedController : MonoBehaviour
    {
        public GreedController()
        {
            this.LifeTime = 3f;
            this.StatIncrease = 1.1f;
            this.StatIncreaseFlat = 0.1f;
            this.hasBeenPickedup = false;
        }
        public int Stack = 0;

        public void Start() 
        {
            Stack = 1;
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
                GameObject gameObject = SpawnManager.SpawnDebris(PickupObjectDatabase.GetById(68).gameObject, enemy.aiActor.sprite != null ? enemy.aiActor.sprite.WorldCenter : enemy.aiActor.transform.PositionVector2(), Quaternion.identity);
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
                            ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
                            boomboom.damageToPlayer = 0;
                            boomboom.preventPlayerForce = true;
                            GameManager.Instance.AllPlayers.ToList().ForEach(self => boomboom.ignoreList.Add(self.specRigidbody));
                            boomboom.ignoreList.Add(player.specRigidbody);
                            Exploder.Explode(player.sprite.WorldCenter, boomboom, player.transform.PositionVector2());
                            if (isNewStack != null) { player.StopCoroutine(isNewStack); }
                            isNewStack = player.StartCoroutine(GrantTemporaryBoost(player, this.StatIncrease));
                            AkSoundEngine.PostEvent("Play_OBJ_power_up_01", player.gameObject);

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

        private float Timer;
        private bool BuffsActive= false;
        private int ActiveBuffs = 0;
        private Coroutine isNewStack;
        private IEnumerator GrantTemporaryBoost(PlayerController user, float StatIncreaseValue)
        {
            Timer = 0 - ((float)ActiveBuffs * 0.0333f);
            ActiveBuffs++;
            if (user.ownerlessStatModifiers.Contains(Speed))
            { user.ownerlessStatModifiers.Remove(Speed); }
            if (user.ownerlessStatModifiers.Contains(Damage))
            {user.ownerlessStatModifiers.Remove(Damage);}
            Speed = new StatModifier
            {
                statToBoost = PlayerStats.StatType.MovementSpeed,
                amount = (((StatIncreaseValue-1) * ActiveBuffs)+1)*0.4f,
                modifyType = StatModifier.ModifyMethod.ADDITIVE
            };
            Damage = new StatModifier
            {
                statToBoost = PlayerStats.StatType.Damage,
                amount = ((StatIncreaseValue - 1) * ActiveBuffs) + 1,
                modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
            };
            user.ownerlessStatModifiers.Add(Speed);
            user.ownerlessStatModifiers.Add(Damage);
            user.stats.RecalculateStats(user, true, true);

            while (Timer < 6)
            {
               
                Timer += BraveTime.DeltaTime;
                yield return null;
            }
            user.ownerlessStatModifiers.Remove(Speed);
            user.ownerlessStatModifiers.Remove(Damage);
            user.stats.RecalculateStats(user, true, true);
            ActiveBuffs = 0;
            user.PlayEffectOnActor(StaticVFXStorage.MachoBraceBurstVFX, new Vector3(0, 0.25f));
            AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Fade_01", user.gameObject);
            yield break;
        }

        public StatModifier Damage;
        public StatModifier Speed;


        private static IEnumerator HandleManualCoinSpawnLifespan(CurrencyPickup coins, float lifeTime)
        {
            float elapsed = 0f;
            bool isCaught = false;


            while (elapsed < lifeTime * 0.75f)
            {
                if (coins == null) { yield break; }
                var Player = GameManager.Instance.GetActivePlayerClosestToPoint(coins.transform.position);
                if (MathToolbox.IsCloserThan(Player.transform.position, coins.transform.position, 3))
                {
                    if (isCaught == false)
                    {
                        PickupMover pickupMover = coins.gameObject.GetOrAddComponent<PickupMover>();
                        if (pickupMover.specRigidbody)
                        {
                            pickupMover.specRigidbody.CollideWithTileMap = false;
                        }
                        pickupMover.acceleration = 20f;
                        pickupMover.maxSpeed = 15f;
                        pickupMover.minRadius = 0.01f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = false;
                        isCaught = true;
                    }
                }
                else
                {
                    elapsed += BraveTime.DeltaTime;
                }


                //elapsed += BraveTime.DeltaTime;
                if (0.125f % 0.0625f == 0)
                {
                    DoParticlePoof(coins, elapsed / 2, new Vector2(-1, 1));
                    DoParticlePoof(coins, elapsed, Vector2.up);
                    DoParticlePoof(coins, elapsed / 2, new Vector2(1, 1));
                }
                yield return null;
            }
            float flickerTimer = 0f;


            while (elapsed < lifeTime)
            {
                if (coins == null) { yield break; }
                var Player = GameManager.Instance.GetActivePlayerClosestToPoint(coins.transform.position);
                if (MathToolbox.IsCloserThan(Player.transform.position, coins.transform.position, 3))
                {
                    if (isCaught == false)
                    {
                        PickupMover pickupMover = coins.gameObject.GetOrAddComponent<PickupMover>();
                        if (pickupMover.specRigidbody)
                        {
                            pickupMover.specRigidbody.CollideWithTileMap = false;
                        }
                        pickupMover.m_shouldPath = true;
                        pickupMover.acceleration = 20f;
                        pickupMover.maxSpeed = 5f;
                        pickupMover.minRadius = -1f;
                        pickupMover.moveIfRoomUnclear = true;
                        pickupMover.stopPathingOnContact = false;
                        isCaught = true;
                    }
                }
                else
                {
                    elapsed += BraveTime.DeltaTime;
                }

                flickerTimer += BraveTime.DeltaTime;
                if (coins != null && coins.renderer)
                {
                   
                    bool enabled = flickerTimer % 0.2f > 0.15f;
                    if (enabled)
                    {
                        DoParticlePoof(coins, elapsed / 2, new Vector2(-1, 1));
                        DoParticlePoof(coins, elapsed, Vector2.up);
                        DoParticlePoof(coins, elapsed / 2, new Vector2(1, 1));

                    }
                    coins.renderer.enabled = enabled;
                }
                else if (coins == null)
                {
                    yield break;
                }
                yield return null;
            }
            if (coins == null) { yield break; }
            UnityEngine.Object.Destroy(coins.gameObject);
            yield break;
        }

        public static void DoParticlePoof(CurrencyPickup c, float m, Vector2 Dir)
        {
            if (c == null) { return; }
            if (c.sprite == null) { return; }
            Vector3 vector = c.sprite.WorldBottomLeft.ToVector3ZisY(0);
            Vector3 vector2 = c.sprite.WorldTopRight.ToVector3ZisY(0);
            Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
            GlobalSparksDoer.DoSingleParticle(position, Dir * (3 * m) * 2, null, null, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
        }

        public void IncrementStack()
        {
            this.StatIncrease += 0.05f;
            this.StatIncreaseFlat += 0.05f;
            this.LifeTime += 0.5f;
            Stack++;
        }

        private float StatIncreaseFlat;

        private float LifeTime;
        private float StatIncrease;
        public bool hasBeenPickedup;
        public PlayerController player;
    }
    class Greedy : PerkPickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Greed";
            //string resourcePath = "Planetside/Resources/PerkThings/Greedy.png";
            GameObject gameObject = new GameObject(name);
            Greedy item = gameObject.AddComponent<Greedy>();

            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("Greedy"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Makes You Greedier.";
            string longDesc = "An Idol of Greed in its purest form, a Gungeoneers life taken converting themselves to gold, after selling themselves off bit by bit to hoard more of the treasures of the Gungeon to themself.\n\nBut... a little more couldn't hurt, right?";
            item.SetupItem(shortDesc, longDesc, "psog");
            Greedy.GreedyID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.yellow;
            particles.ParticleSystemColor2 = new Color(255, 215, 0);
            item.OutlineColor = new Color(0.6f, 0.52f, 0.05f);
            item.encounterTrackable.DoNotificationOnEncounter = false;

        }
        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 0,
                    LockedString = "\"Greed Is Good.\"",
                    UnlockedString = "\"Greed Is Good.\"",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 1,
                    LockedString = AlphabetController.ConvertString("Greed Is Good"),
                    UnlockedString = "Slain enemies drop glowing casings that fade over time.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 2,
                    LockedString = AlphabetController.ConvertString("That Greed Better"),
                    UnlockedString = "Picking up glowing casings temporarily increases movement speed and damage.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 4,
                    LockedString = AlphabetController.ConvertString("Stacking Is Good"),
                    UnlockedString = "Stacking increases the lifetime of glowing casings, and increases the amount of stats you get from them.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.GREEDY_FLAG_STACK
                },
        };
        public override CustomTrackedStats StatToIncreaseOnPickup => SaveAPI.CustomTrackedStats.AMOUNT_BOUGHT_GREEDY;


        public static int GreedyID;

        public new bool PrerequisitesMet()
        {
            EncounterTrackable component = base.GetComponent<EncounterTrackable>();
            return component == null || component.PrerequisitesMet();
        }
        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            base.HandleEncounterable(player);

            SaveAPI.AdvancedGameStatsManager.Instance.RegisterStatChange(StatToIncreaseOnPickup, 1);

            m_hasBeenPickedUp = true;
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);

            //player.OnKilledEnemyContext += this.OnKilledEnemy;

            GreedController greed = player.gameObject.GetOrAddComponent<GreedController>();
            greed.player = player;
            if (greed.hasBeenPickedup == true)
            { greed.IncrementStack(); SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.GREEDY_FLAG_STACK, true); }
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = greed.hasBeenPickedup == true ? "Greed Is Even Better." : "Greed Is Good.";
            OtherTools.NotifyCustom("Greedy", BlurbText, "Greedy", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);

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
                            ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
                            boomboom.damageToPlayer = 0;
                            boomboom.preventPlayerForce = true;
                            boomboom.ignoreList.Add(player.specRigidbody);
                            Exploder.Explode(player.sprite.WorldCenter, boomboom, player.transform.PositionVector2());
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
                amount = 1.025f,
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
            while (elapsed < 10)
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


        public void Start()
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

       

        private void Update()
        {
            if (!this.m_hasBeenPickedUp && !this.m_isBeingEyedByRat && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
            {
                GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
            }
        }


        private bool m_hasBeenPickedUp;
    }
}
