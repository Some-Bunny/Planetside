using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Planetside
{
    class FuckYoGunsUp : MonoBehaviour
    {
        public FuckYoGunsUp()
        {
        }
        public void Start(){
            /*
            if (self != null && Hasinited == false){
                ETGModConsole.Log("help");
                Hasinited = true;
                BannedIDs.Add(self.PickupObjectId);
                AddModule();

            }
            */
        }

        public void AddModule()
        {
            AmountOfModulesCurrently++;
            Gun randomGun = PickupObjectDatabase.GetRandomGunOfQualities(new System.Random(UnityEngine.Random.Range(1, 1000)), BannedIDs, new PickupObject.ItemQuality[] { self.quality });
            BannedIDs.Add(randomGun.PickupObjectId);
            ProjectileVolleyData projectileVolleyData = CombineVolleys(self, randomGun);
            ReconfigureVolley(projectileVolleyData);
            self.RawSourceVolley = projectileVolleyData;
            self.SetBaseMaxAmmo(self.GetBaseMaxAmmo() + randomGun.GetBaseMaxAmmo());
            self.GainAmmo(randomGun.CurrentAmmo);
        }
        private List<int> BannedIDs = new List<int>();
        protected static void ReconfigureVolley(ProjectileVolleyData newVolley)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            int num = 0;
            for (int i = 0; i < newVolley.projectiles.Count; i++)
            {
                if (newVolley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Automatic)
                {
                    flag = true;
                }
                if (newVolley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Beam)
                {
                    flag = true;
                }
                if (newVolley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Burst)
                {
                    flag4 = true;
                }
                if (newVolley.projectiles[i].shootStyle == ProjectileModule.ShootStyle.Charged)
                {
                    flag3 = true;
                    num++;
                }
            }
            if (!flag && !flag2 && !flag3 && !flag4)
            {
                return;
            }
            if (!flag && !flag2 && !flag3 && flag4)
            {
                return;
            }
            int num2 = 0;
            for (int j = 0; j < newVolley.projectiles.Count; j++)
            {
                if (newVolley.projectiles[j].shootStyle == ProjectileModule.ShootStyle.SemiAutomatic)
                {
                    newVolley.projectiles[j].shootStyle = ProjectileModule.ShootStyle.Automatic;
                }
                if (newVolley.projectiles[j].shootStyle == ProjectileModule.ShootStyle.Charged && num > 1)
                {
                    num2++;
                    if (num > 1)
                    {
                    }
                }
            }
        }
        private static ProjectileVolleyData CombineVolleys(Gun sourceGun, Gun mergeGun)
        {
            ProjectileVolleyData projectileVolleyData = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            if (sourceGun.RawSourceVolley != null)
            {
                projectileVolleyData.InitializeFrom(sourceGun.RawSourceVolley);
            }
            else
            {
                projectileVolleyData.projectiles = new List<ProjectileModule>();
                projectileVolleyData.projectiles.Add(ProjectileModule.CreateClone(sourceGun.singleModule, true, -1));
                projectileVolleyData.BeamRotationDegreesPerSecond = float.MaxValue;
            }
            if (mergeGun.RawSourceVolley != null)
            {
                for (int i = 0; i < mergeGun.RawSourceVolley.projectiles.Count; i++)
                {
                    ProjectileModule projectileModule = ProjectileModule.CreateClone(mergeGun.RawSourceVolley.projectiles[i], true, -1);
                    projectileModule.IsDuctTapeModule = true;
                    projectileModule.ignoredForReloadPurposes = (projectileModule.ammoCost <= 0 || projectileModule.numberOfShotsInClip <= 0);
                    projectileVolleyData.projectiles.Add(projectileModule);
                    if (!string.IsNullOrEmpty(mergeGun.gunSwitchGroup) && i == 0)
                    {
                        projectileModule.runtimeGuid = ((projectileModule.runtimeGuid == null) ? Guid.NewGuid().ToString() : projectileModule.runtimeGuid);
                        sourceGun.AdditionalShootSoundsByModule.Add(projectileModule.runtimeGuid, mergeGun.gunSwitchGroup);

                    }
                    if (mergeGun.RawSourceVolley.projectiles[i].runtimeGuid != null && mergeGun.AdditionalShootSoundsByModule.ContainsKey(mergeGun.RawSourceVolley.projectiles[i].runtimeGuid))
                    {
                        sourceGun.AdditionalShootSoundsByModule.Add(mergeGun.RawSourceVolley.projectiles[i].runtimeGuid, mergeGun.AdditionalShootSoundsByModule[mergeGun.RawSourceVolley.projectiles[i].runtimeGuid]);
                    }
                }
            }
            else
            {
                ProjectileModule projectileModule2 = ProjectileModule.CreateClone(mergeGun.singleModule, true, -1);
                projectileModule2.IsDuctTapeModule = true;
                projectileModule2.ignoredForReloadPurposes = (projectileModule2.ammoCost <= 0 || projectileModule2.numberOfShotsInClip <= 0);
                projectileVolleyData.projectiles.Add(projectileModule2);
                if (!string.IsNullOrEmpty(mergeGun.gunSwitchGroup))
                {
                    projectileModule2.runtimeGuid = ((projectileModule2.runtimeGuid == null) ? Guid.NewGuid().ToString() : projectileModule2.runtimeGuid);
                    sourceGun.AdditionalShootSoundsByModule.Add(projectileModule2.runtimeGuid, mergeGun.gunSwitchGroup);

                }
            }
            return projectileVolleyData;
        }

        public PlayerController player;
        public Gun self;
        public int AmountOfModulesCurrently;
    }

    class ChaoticShiftController : MonoBehaviour
    {
        public ChaoticShiftController()
        {
            this.AmountOfModules = 1;
            this.hasBeenPickedup = false;
        }
        public void Start() 
        { 
            this.hasBeenPickedup = true;
           
        }

        public void IncrementStack()
        {
            AmountOfModules++;
        }

        public void Update()
        {
            if(player)
            {
                List<Gun> gunList = player.inventory.AllGuns;
                for (int i = 0; i < gunList.Count; i++)
                {
                    Gun currentGunInList = gunList[i];
                    if (IsGunValid(currentGunInList) && currentGunInList != null)
                    {

                        if (currentGunInList.gameObject.GetComponent<FuckYoGunsUp>() != null)
                        {
                            FuckYoGunsUp guns = currentGunInList.gameObject.GetComponent<FuckYoGunsUp>();
                            if (guns.AmountOfModulesCurrently != AmountOfModules && AmountOfModules >= guns.AmountOfModulesCurrently) { guns.AddModule(); }
                        }
                        else
                        {
                            FuckYoGunsUp guns = currentGunInList.gameObject.AddComponent<FuckYoGunsUp>();
                            guns.self = currentGunInList;
                            guns.player = player;
                            guns.AddModule();
                        }
                    }
                    
                  
                }
               
            }
        }
        protected bool IsGunValid(Gun g)
        {
            return !g.InfiniteAmmo && g.CanActuallyBeDropped(g.CurrentOwner as PlayerController);
        }

        public bool hasBeenPickedup;
        public int AmountOfModules;
        public PlayerController player;
    }

    class ChaoticShift : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Chaotic Shift";
            string resourcePath = "Planetside/Resources/PerkThings/chaoticShift.png";
            GameObject gameObject = new GameObject(name);
            ChaoticShift item = gameObject.AddComponent<ChaoticShift>();
          
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Literally just an all stats up.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            ChaoticShift.ChaoticShiftID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = new Color(0, 255, 0);
            particles.ParticleSystemColor2 = new Color(216, 191, 216);
            OutlineColor = new Color(0f, 0.2f, 0f);

        }
        public static int ChaoticShiftID;
        private static Color OutlineColor;

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
            OtherTools.ApplyStat(player, PlayerStats.StatType.Damage, 0.75f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }
            ChaoticShiftController chaos = player.gameObject.GetOrAddComponent<ChaoticShiftController>();
            chaos.player = player;
            if (chaos.hasBeenPickedup == true)
            { chaos.IncrementStack(); }
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = chaos.hasBeenPickedup == true ? "Another Gun Added." : "All Weapons are Doubled.";
            OtherTools.Notify("Chaotic Shift", BlurbText, "Planetside/Resources/PerkThings/chaoticShift", UINotificationController.NotificationColor.GOLD);

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
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
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
