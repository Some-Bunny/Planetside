using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Planetside
{

    class UnbreakableSpiritController : MonoBehaviour
    {
        public UnbreakableSpiritController()
        {
            this.hasRespawnedOnce = false;
            this.hasBeenPickedup = false;
            this.AmountOfRespawns = 1;
        }
        public void Start() 
        { 
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
                //player.healthHaver.OnPreDeath -= SaveLife;
                if (player.ForceZeroHealthState == true)
                { player.healthHaver.Armor = 2; }
                else { player.healthHaver.FullHeal(); }
                if (hasRespawnedOnce != true)
                {
                    OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, .35f, StatModifier.ModifyMethod.ADDITIVE);
                    OtherTools.ApplyStat(player, PlayerStats.StatType.Accuracy, .5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    hasRespawnedOnce = true;
                }
                float HPtOremove = (player.stats.GetStatValue(PlayerStats.StatType.Health));
                OtherTools.ApplyStat(player, PlayerStats.StatType.Health, (-HPtOremove) + 1, StatModifier.ModifyMethod.ADDITIVE);
                GameManager.Instance.MainCameraController.SetManualControl(false, false);
                player.ToggleGunRenderers(true, "non-death");
                player.ToggleHandRenderers(true, "non-death");
                player.CurrentInputState = PlayerInputState.AllInput;
                OtherTools.Notify("One More Chance!", "Your Spirit Saved You!", "Planetside/Resources/PerkThings/unbreakablespirit", UINotificationController.NotificationColor.PURPLE);
                player.StartCoroutine(HandleShield(player));
                GameObject teleportVFX = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX);
                teleportVFX.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(player.transform.PositionVector2() + new Vector2(0f, -0.5f), tk2dBaseSprite.Anchor.LowerCenter);
                teleportVFX.transform.position = teleportVFX.transform.position.Quantize(0.0625f);
                teleportVFX.GetComponent<tk2dBaseSprite>().UpdateZDepth();
                teleportVFX.GetComponent<tk2dBaseSprite>().scale *= 2;
                ExplosionData boomboom = StaticExplosionDatas.genericSmallExplosion;
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
            ParticleSystem particleObject = UnityEngine.Object.Instantiate(StaticVFXStorage.PerkParticleObject).GetComponent<ParticleSystem>();
            AkSoundEngine.PostEvent("Play_BOSS_cyborg_eagle_01", player.gameObject);
            user.healthHaver.IsVulnerable = false;
            float elapsed = 0f;
            while (elapsed < 5)
            {
                if (player.sprite && !GameManager.Instance.IsPaused && (UnityEngine.Random.value > 0.5f))
                {
                    Vector3 vector = player.sprite.WorldBottomLeft.ToVector3ZisY(0);
                    Vector3 vector2 = player.sprite.WorldTopRight.ToVector3ZisY(0);
                    Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
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
                user.healthHaver.IsVulnerable = false;
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            Destroy(particleObject.gameObject, 5);
            user.healthHaver.IsVulnerable = true;
            yield break;
        }


        public void IncrementStack()
        {
            AmountOfRespawns++;
        }
        private bool hasRespawnedOnce;

        public int AmountOfRespawns;
        public PlayerController player;
        public bool hasBeenPickedup;
    }
    class UnbreakableSpirit : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Unbreakable Spirit Perk";
            string resourcePath = "Planetside/Resources/PerkThings/unbreakablespirit.png";
            GameObject gameObject = new GameObject(name);
            UnbreakableSpirit item = gameObject.AddComponent<UnbreakableSpirit>();
          
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Literally just an all stats up.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            UnbreakableSpirit.UnbreakableSpiritID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.white;
            particles.ParticleSystemColor2 = Color.yellow;
            OutlineColor = new Color(0.33f, 0.33f, 0.33f);

        }
        public static int UnbreakableSpiritID;
        private static Color OutlineColor;


        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            m_hasBeenPickedUp = true;
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);

            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            UnbreakableSpiritController spirit = player.gameObject.GetOrAddComponent<UnbreakableSpiritController>();
            spirit.player = player;
            if (spirit.hasBeenPickedup == true)
            { spirit.IncrementStack(); }
            string BlurbText = spirit.hasBeenPickedup == true ? "Another Chance." : "Come Back Stronger.";
            OtherTools.Notify("Unbreakable Spirit", BlurbText, "Planetside/Resources/PerkThings/unbreakablespirit", UINotificationController.NotificationColor.GOLD); 
            UnityEngine.Object.Destroy(base.gameObject);
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
