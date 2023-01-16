using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;

namespace Planetside
{
    class BlastProjectilesCheck : MonoBehaviour 
    {
        public BlastProjectilesCheck()
        {
            this.Cap = 12;
            this.MinToSpawn = 2;
            this.hasBeenPickedup = false;
        }
        public void Start() {this.hasBeenPickedup=true;}
        public void IncrementStack()
        {
            Cap+=4;
            MinToSpawn++;
        }
        public bool hasBeenPickedup;
        public int Cap;
        public int MinToSpawn;
    }

    class BlastProjectiles : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Explosive Birth";
            //string resourcePath = "Planetside/Resources/PerkThings/projectileboom.png";
            GameObject gameObject = new GameObject(name);
            BlastProjectiles item = gameObject.AddComponent<BlastProjectiles>();
            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("projectileboom"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Literally just an all stats up.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            BlastProjectiles.BlastProjectilesID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.yellow;
            particles.ParticleSystemColor2 = new Color(255, 215, 0);
            OutlineColor = new Color(1f, 0.55f, 0f);

            Gun gun4 = PickupObjectDatabase.GetById(43) as Gun;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun4.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 15f;
            projectile.baseData.speed *= 1f;
            projectile.AdditionalScaleMultiplier = 1f;
            projectile.shouldRotate = true;
            projectile.pierceMinorBreakables = true;
            projectile.baseData.range = 30f;
            projectile.SetProjectileSpriteRight("vengbullet", 8, 5, false, tk2dBaseSprite.Anchor.MiddleCenter, 8, 5);

            OtherTools.EasyTrailComponent trail = projectile.gameObject.AddComponent<OtherTools.EasyTrailComponent>();

            trail.TrailPos = projectile.transform.position;
            trail.StartWidth = 0.125f;
            trail.EndWidth = 0;
            trail.LifeTime = 0.2f;
            trail.BaseColor = new Color(1f, 0f, 0f, 0.6f);
            trail.StartColor = new Color(1f, 1f, 0f, 0.6f);
            trail.EndColor = new Color(0.1f, 0f, 0f, 0f);


            VengefulProjectile = projectile;
        }
        public static int BlastProjectilesID;
        private static Color OutlineColor;
        public static Projectile VengefulProjectile;
        public new bool PrerequisitesMet()
        {
            EncounterTrackable component = base.GetComponent<EncounterTrackable>();
            return component == null || component.PrerequisitesMet();
        }

        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;

            m_hasBeenPickedUp = true;
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }
            BlastProjectilesCheck blast =player.gameObject.GetOrAddComponent<BlastProjectilesCheck>();
            if(blast.hasBeenPickedup==true)
            { blast.IncrementStack(); }

            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = blast.hasBeenPickedup == true ? "More Projectiles Birthed." : "All Explosions birth vengeful shells.";
            OtherTools.NotifyCustom("Explosive Birth", BlurbText, "projectileboom", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);

            //OtherTools.Notify("Explosive Birth", BlurbText, "Planetside/Resources/PerkThings/projectileboom", UINotificationController.NotificationColor.GOLD);
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
