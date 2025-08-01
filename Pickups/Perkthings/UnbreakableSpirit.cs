using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SaveAPI;


namespace Planetside
{

    /*
    class UnbreakableSpiritController : MonoBehaviour
    {
        public int Stacks = 0;
        public UnbreakableSpiritController()
        {
            this.hasRespawnedOnce = false;
            this.hasBeenPickedup = false;
            this.AmountOfRespawns = 1;
        }
        public void Start() 
        {
            Stacks = 1;
            this.hasBeenPickedup = true; 
            if (player.healthHaver !=null)
            {
                player.healthHaver.OnPreDeath += SaveLife;
            }
        }

        private void SaveLife(Vector2 finalDamageDirection)
        {
            if (AmountOfRespawns !=0 && AmountOfRespawns >= 0)
            {
                AmountOfRespawns--;
                if (player.ForceZeroHealthState == true)
                { player.healthHaver.Armor = 4; }
                else 
                {
                    player.healthHaver.currentHealth = Mathf.Min(3, player.healthHaver.AdjustedMaxHealth);
                    //player.healthHaver.FullHeal(); 
                }
                if (hasRespawnedOnce != true)
                {
                    OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, .3f, StatModifier.ModifyMethod.ADDITIVE);
                    OtherTools.ApplyStat(player, PlayerStats.StatType.Accuracy, .5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    hasRespawnedOnce = true;
                }
                float HPtOremove = (player.stats.GetStatValue(PlayerStats.StatType.Health));
                OtherTools.ApplyStat(player, PlayerStats.StatType.Health, -2 , StatModifier.ModifyMethod.ADDITIVE);
                
                GameManager.Instance.MainCameraController.SetManualControl(false, false);
                player.ToggleGunRenderers(true, "non-death");
                player.ToggleHandRenderers(true, "non-death");
                player.CurrentInputState = PlayerInputState.AllInput;
                player.spriteAnimator.enabled = true;

                OtherTools.NotifyCustom("One More Chance!", "Your Spirit Saved You!", "unbreakablespirit", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);

                //OtherTools.Notify("One More Chance!", "Your Spirit Saved You!", "Planetside/Resources/PerkThings/unbreakablespirit", UINotificationController.NotificationColor.PURPLE);
                player.StartCoroutine(HandleShield(player));
                GameObject teleportVFX = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX);
                teleportVFX.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(player.transform.PositionVector2() + new Vector2(0f, -0.5f), tk2dBaseSprite.Anchor.LowerCenter);
                teleportVFX.transform.position = teleportVFX.transform.position.Quantize(0.0625f);
                teleportVFX.GetComponent<tk2dBaseSprite>().UpdateZDepth();
                teleportVFX.GetComponent<tk2dBaseSprite>().scale *= 2;
                ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
                boomboom.damageToPlayer = 0;
                boomboom.damageRadius = 50f;
                boomboom.damage = 3500;
                boomboom.preventPlayerForce = true;
                boomboom.ignoreList.Add(player.specRigidbody);
                boomboom.playDefaultSFX = false;
                boomboom.doExplosionRing = false;
                Exploder.Explode(player.sprite.WorldCenter, boomboom, player.transform.PositionVector2());
            }
        }

        private IEnumerator HandleShield(PlayerController user)
        {
            //ParticleSystem particleObject = UnityEngine.Object.Instantiate(StaticVFXStorage.PerkParticleObject).GetComponent<ParticleSystem>();
            AkSoundEngine.PostEvent("Play_BOSS_cyborg_eagle_01", player.gameObject);
            player.healthHaver.IsVulnerable = false;
            float elapsed = 0f;
            while (elapsed < 5)
            {
                if (player.sprite && !GameManager.Instance.IsPaused && (UnityEngine.Random.value > 0.5f))
                {
                    var particleObject = ParticleBase.ReturnParticleSystem("PerkParticle");
                    Vector3 vector = player.sprite.WorldBottomLeft.ToVector3ZisY(0);
                    Vector3 vector2 = player.sprite.WorldTopRight.ToVector3ZisY(0);
                    Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
                    
                    if (particleObject == null){ break;}

                    ParticleSystem particleSystem = particleObject;
                    var trails = particleSystem.trails;
                    trails.worldSpace = false;
                    var main = particleSystem.main;
                    main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow , Color.white);
                    ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                    {
                        position = position,
                        randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
                    };
                    var emission = particleSystem.emission;
                    emission.enabled = false;
                    particleSystem.gameObject.SetActive(true);
                    particleSystem.Emit(emitParams, 1);
                }
                if (user != null)
                {
                    user.healthHaver.IsVulnerable = false;
                }
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            if (user != null)
            {
                user.healthHaver.IsVulnerable = true;
            }
            yield break;
        }


        public void IncrementStack()
        {
            Stacks++;
            AmountOfRespawns++;
        }
        private bool hasRespawnedOnce;

        public int AmountOfRespawns;
        public PlayerController player;
        public bool hasBeenPickedup;
    }
    */
    class UnbreakableSpirit : PerkPickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = " Unbreakable Spirit";
            //string resourcePath = "Planetside/Resources/PerkThings/unbreakablespirit.png";
            GameObject gameObject = new GameObject(name);
            UnbreakableSpirit item = gameObject.AddComponent<UnbreakableSpirit>();

            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("unbreakablespirit"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "One Last Chance.";
            string longDesc = "Go out in a blaze of glory and be reborn with greater power.\n\nOnly the strongest of spirits can refuse death, to have one last chance.";
            item.SetupItem(shortDesc, longDesc, "psog");
            UnbreakableSpirit.UnbreakableSpiritID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = new Color(1, 0.3f, 0, 1);
            particles.ParticleSystemColor2 = Color.yellow;
            item.OutlineColor = new Color(0.33f, 0.33f, 0.33f);
            item.encounterTrackable.DoNotificationOnEncounter = false;

            item.InitialPickupNotificationText = "Come Back Stronger.";
            item.StackPickupNotificationText = "Another Chance.";

        }
        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 0,
                    LockedString = "\"Come Back Stronger.\"",
                    UnlockedString = "\"Come Back Stronger.\"",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 1,
                    LockedString = AlphabetController.ConvertString("Another Chance"),
                    UnlockedString = "Dying revives you, but reduces your HP.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 3,
                    LockedString = AlphabetController.ConvertString("All Blast"),
                    UnlockedString = "Reviving creates a massive explosion that deals massive damage to enemies.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 5,
                    LockedString = AlphabetController.ConvertString("All Damage"),
                    UnlockedString = "Reviving also grants a permanent damage up.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 7,
                    LockedString = AlphabetController.ConvertString("More Chances"),
                    UnlockedString = "Stacking grants more revives, but does not grant any additional damage after the first revive.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.UNBREAKABLESPIRIT_FLAG_STACK
                },
        };
        //public override SaveAPI.CustomTrackedStats StatToIncreaseOnPickup => SaveAPI.CustomTrackedStats.AMOUNT_BOUGHT_UNBREAKABLESPIRIT;

        public override CustomDungeonFlags FlagToSetOnStack => CustomDungeonFlags.UNBREAKABLESPIRIT_FLAG_STACK;

        public static int UnbreakableSpiritID;

        /*
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

                Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
                player.BloopItemAboveHead(base.sprite, "");
                UnbreakableSpiritController spirit = player.gameObject.GetOrAddComponent<UnbreakableSpiritController>();
                spirit.player = player;
                if (spirit.hasBeenPickedup == true)
                { spirit.IncrementStack(); SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.UNBREAKABLESPIRIT_FLAG_STACK, true); }
                string BlurbText = spirit.hasBeenPickedup == true ? "Another Chance." : "Come Back Stronger.";
                OtherTools.NotifyCustom("Unbreakable Spirit", BlurbText, "unbreakablespirit", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);
                UnityEngine.Object.Destroy(base.gameObject);
            }
            */

        public override void OnInitialPickup(PlayerController playerController)
        {
            playerController.healthHaver.ModifyDamage += CheatDeath;
        }

        public void CheatDeath(HealthHaver healthHaver, HealthHaver.ModifyDamageEventArgs modifyDamageEvent)
        {

            if (AmountOfRespawns == 0)
            {
                return;
            }
            if (Owner.ForceZeroHealthState)
            {
                if (modifyDamageEvent.InitialDamage >= healthHaver.currentArmor)
                {
                    SaveLife();
                }
                return;
            }

            if (modifyDamageEvent.InitialDamage >= healthHaver.currentHealth)
            {
                modifyDamageEvent.ModifiedDamage = 0;
                modifyDamageEvent.InitialDamage = 0;
                SaveLife();
            }
        }

        public override void OnStack(PlayerController playerController)
        {
            AmountOfRespawns++;
        }


        private void SaveLife()
        {
            AmountOfRespawns--;
            if (_Owner.ForceZeroHealthState == true)
            { _Owner.healthHaver.Armor = 4; }
            else
            {
                _Owner.healthHaver.currentHealth = Mathf.Min(3, _Owner.healthHaver.AdjustedMaxHealth);
            }
            if (hasRespawnedOnce != true)
            {
                OtherTools.ApplyStat(_Owner, PlayerStats.StatType.Damage, .3f, StatModifier.ModifyMethod.ADDITIVE);
                OtherTools.ApplyStat(_Owner, PlayerStats.StatType.Accuracy, .5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                hasRespawnedOnce = true;
            }

            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                startColor = Color.yellow,
                position = _Owner.sprite.WorldCenter,
                startSize = 32,
                startLifetime = 0.5f,
                velocity = Vector3.zero,
            });

            float HPtOremove = (_Owner.stats.GetStatValue(PlayerStats.StatType.Health));
            OtherTools.ApplyStat(_Owner, PlayerStats.StatType.Health, -2, StatModifier.ModifyMethod.ADDITIVE);

            GameManager.Instance.MainCameraController.SetManualControl(false, false);
            _Owner.ToggleGunRenderers(true, "non-death");
            _Owner.ToggleHandRenderers(true, "non-death");
            _Owner.CurrentInputState = PlayerInputState.AllInput;
            _Owner.spriteAnimator.enabled = true;

            OtherTools.NotifyCustom("One More Chance!", "Your Spirit Saved You!", "unbreakablespirit", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);

            //OtherTools.Notify("One More Chance!", "Your Spirit Saved You!", "Planetside/Resources/PerkThings/unbreakablespirit", UINotificationController.NotificationColor.PURPLE);
            _Owner.StartCoroutine(HandleShield(_Owner));
            GameObject teleportVFX = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX);
            teleportVFX.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(_Owner.transform.PositionVector2() + new Vector2(0f, -0.5f), tk2dBaseSprite.Anchor.LowerCenter);
            teleportVFX.transform.position = teleportVFX.transform.position.Quantize(0.0625f);
            teleportVFX.GetComponent<tk2dBaseSprite>().UpdateZDepth();
            teleportVFX.GetComponent<tk2dBaseSprite>().scale *= 2;
            ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
            boomboom.damageToPlayer = 0;
            boomboom.damageRadius = 50f;
            boomboom.damage = 3500;
            boomboom.preventPlayerForce = true;
            boomboom.ignoreList.Add(_Owner.specRigidbody);
            boomboom.playDefaultSFX = false;
            boomboom.doExplosionRing = false;
            Exploder.Explode(_Owner.sprite.WorldCenter, boomboom, _Owner.transform.PositionVector2());
        }

        private IEnumerator HandleShield(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_BOSS_cyborg_eagle_01", _Owner.gameObject);
            _Owner.healthHaver.IsVulnerable = false;
            float elapsed = 0f;
            while (elapsed < 5)
            {
                if (_Owner.sprite && !GameManager.Instance.IsPaused && (UnityEngine.Random.value > 0.5f))
                {
                    var particleObject = ParticleBase.ReturnParticleSystem("PerkParticle");
                    Vector3 vector = _Owner.sprite.WorldBottomLeft.ToVector3ZisY(0);
                    Vector3 vector2 = _Owner.sprite.WorldTopRight.ToVector3ZisY(0);
                    Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));

                    if (particleObject == null) { break; }

                    ParticleSystem particleSystem = particleObject;
                    var trails = particleSystem.trails;
                    trails.worldSpace = false;
                    var main = particleSystem.main;
                    main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow, Color.white);
                    ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                    {
                        position = position,
                        randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
                    };
                    var emission = particleSystem.emission;
                    emission.enabled = false;
                    particleSystem.gameObject.SetActive(true);
                    particleSystem.Emit(emitParams, 1);
                }
                if (user != null)
                {
                    user.healthHaver.IsVulnerable = false;
                }
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            if (user != null)
            {
                user.healthHaver.IsVulnerable = true;
            }
            yield break;
        }

        public override void MidGameDeserialize(List<object> data)
        {
            base.MidGameDeserialize(data);
            int i = 0;
            hasRespawnedOnce = (bool)data[i++];
            AmountOfRespawns = (int)data[i++];
        }


        public override void MidGameSerialize(List<object> data)
        {
            base.MidGameSerialize(data);
            if (isDummy) { return; }
            data.Add(hasRespawnedOnce);
            data.Add(AmountOfRespawns);
        }

        private bool hasRespawnedOnce = false;
        public int AmountOfRespawns = 0;
    }
}
