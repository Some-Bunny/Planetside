using Dungeonator;
using ItemAPI;
using System;
using System.Reflection;
using UnityEngine;

namespace Planetside
{

    class ContractController : MonoBehaviour
    {
        public ContractController()
        {
            this.BoysToSpawn = 1;
            this.hasBeenPickedup = false;
        }
        public void Start() { 
            this.hasBeenPickedup = true;
            SpawnTheBoys();
            GameManager.Instance.OnNewLevelFullyLoaded += this.OnNewFloorLoaded;
        }
        public void IncrementStack()
        {
            SpawnTheBoys();
            BoysToSpawn++;
        }

        private void OnNewFloorLoaded(){
            for (int e = 0; e < BoysToSpawn; e++)
            {
                SpawnTheBoys();
            }
        }

        public void OnDestroy()
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= this.OnNewFloorLoaded;
        }

        private void SpawnTheBoys()
        {
            RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
            IntVector2? randomAvailableCell = absoluteRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 4), new CellTypes?(CellTypes.FLOOR), false, null);
            IntVector2? intVector = (randomAvailableCell == null) ? null : new IntVector2?(randomAvailableCell.GetValueOrDefault() + IntVector2.One);
            AIActor aiactor = AIActor.Spawn(Contract.Contractor, intVector.Value, player.GetAbsoluteParentRoom(), true, AIActor.AwakenAnimationType.Default, true);

            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX);
            gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(new Vector3(intVector.Value.x, intVector.Value.y), tk2dBaseSprite.Anchor.LowerCenter);
            gameObject2.transform.position = gameObject2.transform.position.Quantize(0.0625f);
            gameObject2.GetComponent<tk2dBaseSprite>().UpdateZDepth();


            CompanionController comp = aiactor.gameActor.GetComponent<CompanionController>();
            comp.Initialize(player);

            aiactor.CompanionOwner = player;
            aiactor.specRigidbody.Reinitialize();


            CustomScarfDoer scorf = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.ScarfObject.gameObject).AddComponent<CustomScarfDoer>();
            scorf.AttachTarget = aiactor;
            scorf.ScarfMaterial = StaticVFXStorage.ScarfObject.ScarfMaterial;
            scorf.StartWidth = 0.0625f;
            scorf.EndWidth = 0.125f;
            scorf.AnimationSpeed = 30f;
            scorf.ScarfLength = 0.5f;
            scorf.AngleLerpSpeed = 20;
            scorf.BackwardZOffset = -0.2f;
            scorf.CatchUpScale = 1.3f;
            scorf.SinSpeed = 9f;
            scorf.AmplitudeMod = 0.235f;
            scorf.WavelengthMod = 1.3f;

            scorf.ScarfMaterial.SetColor("_OverrideColor", new Color(1, 0.2f, 0.04f));

            scorf.Initialize(aiactor);
        }
        public bool hasBeenPickedup;
        public int BoysToSpawn;
        public PlayerController player;
    }


    class Contract : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "Contractual Obligation";
            string resourcePath = "Planetside/Resources/PerkThings/contract.png";
            GameObject gameObject = new GameObject(name);
            Contract item = gameObject.AddComponent<Contract>();
          
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Literally just an all stats up.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
            Contract.ContractID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;

            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.white;
            particles.ParticleSystemColor2 = Color.white;
            OutlineColor = new Color(0.6f, 0.6f, 0.6f);


            var actor = EnemyDatabase.GetOrLoadByGuid("5861e5a077244905a8c25c2b7b4d6ebb");
            GameObject ContractEnemy = GameObject.Instantiate(actor.gameObject);
            GameObject.DontDestroyOnLoad(ContractEnemy);
            FakePrefab.MarkAsFakePrefab(ContractEnemy);
            ContractEnemy.SetActive(false);
            AIActor aiactor = ContractEnemy.GetComponent<AIActor>();
            aiactor.procedurallyOutlined = true;
            aiactor.aiAnimator.facingType = AIAnimator.FacingType.Default;
            aiactor.AssignedCurrencyToDrop = 0;
            aiactor.AdditionalSafeItemDrops = new System.Collections.Generic.List<PickupObject>() { };
            aiactor.AdditionalSimpleItemDrops = new System.Collections.Generic.List<PickupObject>() { };
            float HP = aiactor.healthHaver.GetMaxHealth();
            aiactor.healthHaver.SetHealthMaximum(HP * 10);
            aiactor.CanTargetEnemies = true;
            aiactor.CanTargetPlayers = true;
            aiactor.IsHarmlessEnemy = true;
            aiactor.IgnoreForRoomClear = true;
            aiactor.MovementSpeed *= 1.25f;
            CompanionController yup = aiactor.gameObject.AddComponent<CompanionController>();
            yup.companionID = CompanionController.CompanionIdentifier.NONE;
            yup.CanCrossPits = true;
            yup.CanBePet = true;
            string[] anims = new string[]
            {
                aiactor.aiAnimator.IdleAnimation.AnimNames[2],
                aiactor.aiAnimator.IdleAnimation.AnimNames[2],
                aiactor.aiAnimator.IdleAnimation.AnimNames[1],
                aiactor.aiAnimator.IdleAnimation.AnimNames[1]
            };
            EnemyToolbox.AddNewDirectionAnimation(aiactor.aiAnimator, "pet", anims, new DirectionalAnimation.FlipType[4], DirectionalAnimation.DirectionType.FourWayCardinal);
            //yup.Initialize(player);


            var bs = aiactor.GetComponent<BehaviorSpeculator>();

            
            foreach (AttackBehaviorBase att in aiactor.behaviorSpeculator.AttackBehaviors)
            {

                if (att is ShootGunBehavior)
                {
                    ShootGunBehavior tagr = att as ShootGunBehavior;
                    tagr.LineOfSight = true;
                    tagr.LeadAmount = 0.4f;
                    tagr.MagazineCapacity = 15;
                    tagr.ReloadSpeed = 3;
                    tagr.RespectReload = true;
                    tagr.EmptiesClip = true;
                }
            }

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(2) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            projectile2.baseData.damage = 7;
            projectile2.baseData.speed *= 0.6f;
            projectile2.baseData.range *= 3;

            AIBulletBank.Entry entry = new AIBulletBank.Entry();
            entry.Name = "nyeahh";
            entry.BulletObject = projectile2.gameObject;

            if (aiactor.bulletBank.Bullets == null) { aiactor.bulletBank.Bullets = new System.Collections.Generic.List<AIBulletBank.Entry>(); }
            aiactor.bulletBank.Bullets.Add(entry);

            foreach (AttackBehaviorGroup.AttackGroupItem att in aiactor.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors)
            {
                if (att.Behavior.GetType() == typeof(ShootGunBehavior))
                {
                    ShootGunBehavior tagr = att.Behavior as ShootGunBehavior;
                    tagr.LineOfSight = true;
                    tagr.LeadAmount = 0f;
                    tagr.MagazineCapacity = 15;
                    tagr.ReloadSpeed = 3;
                    tagr.RespectReload = true;
                    tagr.EmptiesClip = true;
                    tagr.UseLaserSight = true;
                    tagr.Cooldown = 3f;
                    tagr.TimeBetweenShots = 0.0066f;
                    tagr.WeaponType = WeaponType.AIShooterProjectile;
                    tagr.OverrideBulletName = "nyeahh";
                }
            }


            AIAnimator aiAnimator = aiactor.aiAnimator;

            AIShooter shooter = aiactor.aiShooter;
            shooter.Inventory.AddGunToInventory(PickupObjectDatabase.GetById(2) as Gun, true);
            shooter.equippedGunId = 2;
            FieldInfo leEnabler = typeof(AIShooter).GetField("m_hasCachedGun", BindingFlags.Instance | BindingFlags.NonPublic);
            leEnabler.SetValue(shooter, false);
            shooter.Inventory.DestroyAllGuns();
            Gun equippedGun = shooter.Inventory.AddGunToInventory(PickupObjectDatabase.GetById(2) as Gun, true);
            FieldInfo fieldInf = typeof(AIShooter).GetField("m_cachedGun", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInf.SetValue(shooter, equippedGun);
            FieldInfo m_currentGunField = typeof(GunInventory).GetField("m_currentGun", BindingFlags.Instance | BindingFlags.NonPublic);
            m_currentGunField.SetValue(shooter.Inventory, equippedGun);
            equippedGun.gameObject.SetActive(true);


            
            CompanionFollowPlayerBehavior comp = new CompanionFollowPlayerBehavior();
            comp.CanRollOverPits = false;
            //comp.CatchUpOutAnimation = aiAnimator.MoveAnimation.Prefix;
            comp.DisableInCombat = true;
            //comp.IdleAnimations = aiAnimator.IdleAnimation.AnimNames;
            comp.PathInterval = 0.25f;
            comp.IdealRadius = 6;
            comp.CatchUpRadius = 8;
            comp.CatchUpAccelTime = 5;
            comp.CatchUpSpeed = aiactor.MovementSpeed *= 1.2f;
            comp.CatchUpMaxSpeed = aiactor.MovementSpeed *= 1.3f;
           
            //comp.CatchUpAnimation = aiAnimator.MoveAnimation.Prefix;
            //comp.RollAnimation = aiAnimator.MoveAnimation.Prefix;
            //comp.CatchUpOutAnimation = aiAnimator.MoveAnimation.Prefix;
            comp.TemporarilyDisabled = true;

            bs.MovementBehaviors.Add(comp);

            
            /*
            SeekTargetBehavior seek = new SeekTargetBehavior();
            seek.ReturnToSpawn = false;
            seek.StopWhenInRange = true;
            seek.CustomRange = 6;
            seek.LineOfSight = true;
            seek.SpawnTetherDistance = 0;
            seek.PathInterval = 0.25f;
            seek.SpecifyRange = false;
            seek.MinActiveRange = 6;
            seek.MaxActiveRange = 11;

            bs.MovementBehaviors.Add(seek);
            */

            Contractor = aiactor;
        }
        public static int ContractID;
        private static Color OutlineColor;

        public static AIActor Contractor;
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

            ContractController contract = player.gameObject.GetOrAddComponent<ContractController>();
            contract.player = player;
            if (contract.hasBeenPickedup == true)
            { contract.IncrementStack(); }

            string BlurbText = contract.hasBeenPickedup == true ? "Hire an Additional Killer.": "Hire a Friendly Contract Killer.";
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            OtherTools.Notify("Contract", BlurbText, "Planetside/Resources/PerkThings/contract", UINotificationController.NotificationColor.GOLD);
            player.BloopItemAboveHead(base.sprite, "");
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
