using Dungeonator;
using ItemAPI;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planetside
{
    class BlastProjectilesCheck : MonoBehaviour 
    {
        public int Stack = 0;
        public BlastProjectilesCheck()
        {
            this.Cap = 12;
            this.MinToSpawn = 2;
            this.hasBeenPickedup = false;
        }
        public void Start() {this.hasBeenPickedup=true; Stack = 1; }
        public void IncrementStack()
        {
            Stack++;
            Cap+=4;
            MinToSpawn++;
        }
        public bool hasBeenPickedup;
        public int Cap;
        public int MinToSpawn;
    }

    class BlastProjectiles : PerkPickupObject, IPlayerInteractable
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
            string shortDesc = "From Death, Life";
            string longDesc = "From Life, Death.\n\nA perfect cycle.";
            item.SetupItem(shortDesc, longDesc, "psog");
            BlastProjectiles.BlastProjectilesID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.yellow;
            particles.ParticleSystemColor2 = new Color(255, 215, 0);
            item.OutlineColor = new Color(1f, 0.55f, 0f);
            item.encounterTrackable.DoNotificationOnEncounter = false;

            Gun gun4 = PickupObjectDatabase.GetById(43) as Gun;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun4.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 9f;
            projectile.baseData.speed *= 1f;
            projectile.AdditionalScaleMultiplier = 1f;
            projectile.shouldRotate = true;
            projectile.pierceMinorBreakables = true;
            projectile.baseData.range = 30f;
            ItemAPI.GunTools.SetProjectileCollisionRight(projectile, "vengbullet", StaticSpriteDefinitions.Projectile_Sheet_Data, 8, 5, false, tk2dBaseSprite.Anchor.MiddleCenter);

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

        public override CustomTrackedStats StatToIncreaseOnPickup => SaveAPI.CustomTrackedStats.AMOUNT_BOUGHT_EXPLOSIVEBIRTH;
        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 0,
                    LockedString = "\"All Explosions birth Vengeful shells.\"",
                    UnlockedString = "\"All Explosions birth Vengeful shells.\"",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 3,
                    LockedString = AlphabetController.ConvertString("Damage Scales Amount"),
                    UnlockedString = "Higher Explosion Damage Births More Shells.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 5,
                    LockedString = AlphabetController.ConvertString("Stacking Scales Amount"),
                    UnlockedString = "Stacking Increases amount of shells birthed.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.EXPLOSIVEBIRTH_FLAG_STACK
                },
        };


        public static int BlastProjectilesID;
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
            base.HandleEncounterable(player);

            SaveAPI.AdvancedGameStatsManager.Instance.RegisterStatChange(StatToIncreaseOnPickup, 1);

            m_hasBeenPickedUp = true;
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }
            BlastProjectilesCheck blast =player.gameObject.GetOrAddComponent<BlastProjectilesCheck>();
            if(blast.hasBeenPickedup==true)
            { blast.IncrementStack(); 
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.EXPLOSIVEBIRTH_FLAG_STACK, true); }

            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = blast.hasBeenPickedup == true ? "More Projectiles Birthed." : "Explosions Birth Vengeful Shells.";
            OtherTools.NotifyCustom("Explosive Birth", BlurbText, "projectileboom", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);

            //OtherTools.Notify("Explosive Birth", BlurbText, "Planetside/Resources/PerkThings/projectileboom", UINotificationController.NotificationColor.GOLD);
            UnityEngine.Object.Destroy(base.gameObject);
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
