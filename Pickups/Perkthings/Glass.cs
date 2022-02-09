using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;

namespace Planetside
{
    class GlassComponent : MonoBehaviour
    {
        public GlassComponent()
        {
            this.DamageMult = 2.25f;
            this.DamageToGetFromOverHeal = 0.25f;
            this.hasBeenPickedup = false;
            this.LeniencyProtection = 0;
        }
        public void Start() 
        {
            GiveDamage();
            this.hasBeenPickedup = true; 
            if (player.ForceZeroHealthState == true){
                player.healthHaver.Armor = 2;
            }
            else{
                float HPtOremove = (player.stats.GetStatValue(PlayerStats.StatType.Health));
                OtherTools.ApplyStat(player, PlayerStats.StatType.Health, (-HPtOremove) + 1, StatModifier.ModifyMethod.ADDITIVE);
                if (player.healthHaver.Armor >= 1) { player.healthHaver.Armor = 1; }
            }
        }
        public void Update()
        {
            int ArmorAllowed = player.ForceZeroHealthState == true ? 5 : 3;
            ArmorAllowed -= LeniencyProtection;
            if (player.healthHaver.Armor >= ArmorAllowed){
                DoHurty();
                OtherTools.Notify("A Glass Curse", "Prevented Armor Increase!", "Planetside/Resources/PerkThings/glass", UINotificationController.NotificationColor.GOLD);
                player.healthHaver.Armor = ArmorAllowed-1; 
            }
            if ((player.stats.GetStatValue(PlayerStats.StatType.Health) != 1 && player.stats.GetStatValue(PlayerStats.StatType.Health) >= 1)){
                DoHurty();
                float HPtOremove = (player.stats.GetStatValue(PlayerStats.StatType.Health));
                OtherTools.ApplyStat(player, PlayerStats.StatType.Health, (-HPtOremove) + 1, StatModifier.ModifyMethod.ADDITIVE);
                OtherTools.Notify("A Glass Curse", "Prevented Health Increase!", "Planetside/Resources/PerkThings/glass", UINotificationController.NotificationColor.GOLD);
            }
        }

        public void DoHurty(bool IsBigHeart = false)
        {
            if (player){
                //if (IsBigHeart == true) { DamageMult += DamageToGetFromOverHeal*2; } else { DamageMult += DamageToGetFromOverHeal; }
                AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", player.gameObject);
                GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
                tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
                component.PlaceAtPositionByAnchor(player.transform.position + new Vector3(0.375f, 0.375f), tk2dBaseSprite.Anchor.MiddleCenter);
                //player.ownerlessStatModifiers.Remove(this.DamageStat);
                //GiveDamage();
            }
        }
        public void GiveDamage()
        {
            StatModifier item = new StatModifier
            {
                statToBoost = PlayerStats.StatType.Damage,
                amount = DamageMult,
                modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
            };
            this.DamageStat = item;
            player.ownerlessStatModifiers.Add(item);
            player.stats.RecalculateStats(player, true, true);
        }

        
        public void IncrementStack()
        {
            this.DamageToGetFromOverHeal += 0.25f;
            this.DamageMult += 0.5f;
            if (LeniencyProtection != -1) {LeniencyProtection = -1;}
        }
        private StatModifier DamageStat;
        public PlayerController player;
        public bool hasBeenPickedup;
        public float DamageMult;
        public int LeniencyProtection;
        public float DamageToGetFromOverHeal;
    }
    class Glass : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Glass Perk";
            string resourcePath = "Planetside/Resources/PerkThings/glass.png";
            GameObject gameObject = new GameObject(name);
            Glass item = gameObject.AddComponent<Glass>();
          
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Literally just an all stats up.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            Glass.GlassID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.cyan;
            particles.ParticleSystemColor2 = Color.blue;

            OutlineColor = new Color(0, 0.045f, 0.9f);
        }
        public static int GlassID;

        private static Color OutlineColor;



        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            if (!player.CurrentItem)
            {
                return;
            }
            m_hasBeenPickedUp = true;
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);
            GlassComponent glass = player.gameObject.GetOrAddComponent<GlassComponent>();
            glass.player = player;
            if (glass.hasBeenPickedup == true)
            { glass.IncrementStack(); }
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = glass.hasBeenPickedup == true ? "Even More Fatal." : "Fragile, yet Fatal.";
            OtherTools.Notify("Glass", BlurbText, "Planetside/Resources/PerkThings/glass", UINotificationController.NotificationColor.GOLD);
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
