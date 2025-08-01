using Dungeonator;
using ItemAPI;
using SaveAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections;
using static ChancebulonBlobProjectileAttack1;

namespace Planetside
{

    class Contract : PerkPickupObject, IPlayerInteractable
    {

        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 0,
                    LockedString = "\"Hire a Contract Killer.\"",
                    UnlockedString = "\"Hire a Contract Killer.\"",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 3,
                    LockedString = AlphabetController.ConvertString("Stacking Adds Contractors"),
                    UnlockedString = "Stacking hires another Contractor.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.CONTRACT_FLAG_STACK
                },
        };


        public static void Init()
        {
            string name = "Contractual Obligation";
            GameObject gameObject = new GameObject(name);
            Contract item = gameObject.AddComponent<Contract>();

            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("contract"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Everyone Has A Price.";
            string longDesc = "Even the Gundead have a price to turn against their own.\nLuckily, to your advantage.";
            item.SetupItem(shortDesc, longDesc, "psog");
            Contract.ContractID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            item.encounterTrackable.DoNotificationOnEncounter = false;

            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.white;
            particles.ParticleSystemColor2 = Color.cyan;
            item.OutlineColor = new Color(0.6f, 0.6f, 0.6f);

            item.StackPickupNotificationText = "Hire an Additional Killer.";
            item.InitialPickupNotificationText = "Hire a Friendly Contract Killer.";


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
            aiactor.healthHaver.SetHealthMaximum(HP * 50);
            aiactor.healthHaver.AllDamageMultiplier = 0.1f;
            aiactor.CanTargetEnemies = true;
            aiactor.CanTargetPlayers = true;
            aiactor.IsHarmlessEnemy = true;
            aiactor.IgnoreForRoomClear = true;
            aiactor.MovementSpeed *= 1.25f;
            CompanionController yup = aiactor.gameObject.AddComponent<CompanionController>();
            yup.companionID = CompanionController.CompanionIdentifier.GATLING_GULL;
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
            projectile2.baseData.damage = 7.5f;
            projectile2.baseData.speed *= 0.7f;
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
            comp.DisableInCombat = true;
            comp.PathInterval = 0.25f;
            comp.IdealRadius = 6;
            comp.CatchUpRadius = 8;
            comp.CatchUpAccelTime = 5;
            comp.CatchUpSpeed = aiactor.MovementSpeed *= 1.2f;
            comp.CatchUpMaxSpeed = aiactor.MovementSpeed *= 1.3f;
          
            comp.TemporarilyDisabled = true;

            bs.MovementBehaviors.Add(comp);
            Contractors.Add(aiactor);

            InitContractor2();
            InitContractor3();

        }
        public static void InitContractor2()
        {
            var actor = EnemyDatabase.GetOrLoadByGuid("70216cae6c1346309d86d4a0b4603045");
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
            aiactor.healthHaver.SetHealthMaximum(HP * 50);
            aiactor.healthHaver.AllDamageMultiplier = 0.1f;
            aiactor.CanTargetEnemies = true;
            aiactor.CanTargetPlayers = true;
            aiactor.IsHarmlessEnemy = true;
            aiactor.IgnoreForRoomClear = true;
            aiactor.MovementSpeed *= 1.7f;
            CompanionController yup = aiactor.gameObject.AddComponent<CompanionController>();
            yup.companionID = CompanionController.CompanionIdentifier.GATLING_GULL;
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
                    tagr.MagazineCapacity = 8;
                    tagr.ReloadSpeed = 3;
                    tagr.RespectReload = true;
                    tagr.EmptiesClip = true;
                }
            }

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(30) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            projectile2.baseData.damage = 10;
            projectile2.baseData.speed *= 1f;
            projectile2.baseData.range *= 3;

            AIBulletBank.Entry entry = new AIBulletBank.Entry();
            entry.Name = "nyeahh";
            entry.BulletObject = projectile2.gameObject;

            if (aiactor.bulletBank.Bullets == null) { aiactor.bulletBank.Bullets = new System.Collections.Generic.List<AIBulletBank.Entry>(); }
            aiactor.bulletBank.Bullets.Add(entry);

            foreach (AttackBehaviorBase att in aiactor.behaviorSpeculator.AttackBehaviors)
            {
                if (att.GetType() == typeof(ShootGunBehavior))
                {
                    ShootGunBehavior tagr = att as ShootGunBehavior;
                    tagr.LineOfSight = true;
                    tagr.LeadAmount = 0f;
                    tagr.MagazineCapacity = 6;
                    tagr.ReloadSpeed = 5;
                    tagr.RespectReload = true;
                    tagr.EmptiesClip = true;
                    tagr.UseLaserSight = true;
                    tagr.Cooldown = 3f;
                    tagr.TimeBetweenShots = 0.2f;
                    tagr.WeaponType = WeaponType.AIShooterProjectile;
                    tagr.OverrideBulletName = "nyeahh";
                }
            }

            AIShooter shooter = aiactor.aiShooter;
            shooter.Inventory.AddGunToInventory(PickupObjectDatabase.GetById(30) as Gun, true);
            shooter.equippedGunId = 30;
            FieldInfo leEnabler = typeof(AIShooter).GetField("m_hasCachedGun", BindingFlags.Instance | BindingFlags.NonPublic);
            leEnabler.SetValue(shooter, false);
            shooter.Inventory.DestroyAllGuns();
            Gun equippedGun = shooter.Inventory.AddGunToInventory(PickupObjectDatabase.GetById(30) as Gun, true);
            FieldInfo fieldInf = typeof(AIShooter).GetField("m_cachedGun", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInf.SetValue(shooter, equippedGun);
            FieldInfo m_currentGunField = typeof(GunInventory).GetField("m_currentGun", BindingFlags.Instance | BindingFlags.NonPublic);
            m_currentGunField.SetValue(shooter.Inventory, equippedGun);
            equippedGun.gameObject.SetActive(true);



            CompanionFollowPlayerBehavior comp = new CompanionFollowPlayerBehavior();
            comp.CanRollOverPits = false;
            comp.DisableInCombat = true;
            comp.PathInterval = 0.25f;
            comp.IdealRadius = 4;
            comp.CatchUpRadius = 8;
            comp.CatchUpAccelTime = 5;
            comp.CatchUpSpeed = aiactor.MovementSpeed *= 1.2f;
            comp.CatchUpMaxSpeed = aiactor.MovementSpeed *= 1.3f;

            comp.TemporarilyDisabled = true;

            bs.MovementBehaviors.Add(comp);
            Contractors.Add(aiactor);

        }
        public static void InitContractor3()
        {
            var actor = EnemyDatabase.GetOrLoadByGuid("df7fb62405dc4697b7721862c7b6b3cd");
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
            aiactor.healthHaver.SetHealthMaximum(HP * 50);
            aiactor.healthHaver.AllDamageMultiplier = 0.1f;
            aiactor.CanTargetEnemies = true;
            aiactor.CanTargetPlayers = true;
            aiactor.IsHarmlessEnemy = true;
            aiactor.IgnoreForRoomClear = true;
            aiactor.MovementSpeed *= 0.8f;
            CompanionController yup = aiactor.gameObject.AddComponent<CompanionController>();
            yup.companionID = CompanionController.CompanionIdentifier.GATLING_GULL;
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
                    tagr.LeadAmount = 1f;
                    tagr.MagazineCapacity = 1;
                    tagr.ReloadSpeed = 6;
                    tagr.RespectReload = true;
                    tagr.EmptiesClip = true;
                }
            }

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(49) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            projectile2.baseData.damage = 35;
            projectile2.baseData.speed *= 1f;
            projectile2.baseData.range *= 3;

            AIBulletBank.Entry entry = new AIBulletBank.Entry();
            entry.Name = "nyeahh";
            entry.BulletObject = projectile2.gameObject;

            if (aiactor.bulletBank.Bullets == null) { aiactor.bulletBank.Bullets = new System.Collections.Generic.List<AIBulletBank.Entry>(); }
            aiactor.bulletBank.Bullets.Add(entry);

            foreach (AttackBehaviorBase att in aiactor.behaviorSpeculator.AttackBehaviors)
            {
                if (att.GetType() == typeof(ShootGunBehavior))
                {
                    ShootGunBehavior tagr = att as ShootGunBehavior;
                    tagr.LineOfSight = true;
                    tagr.LeadAmount = 1f;
                    tagr.MagazineCapacity = 3;
                    tagr.ReloadSpeed = 6;
                    tagr.RespectReload = true;
                    tagr.EmptiesClip = true;
                    tagr.UseLaserSight = true;
                    tagr.Cooldown = 6f;
                    tagr.TimeBetweenShots = 0.5f;
                    tagr.WeaponType = WeaponType.AIShooterProjectile;
                    tagr.OverrideBulletName = "nyeahh";
                }
            }


            AIShooter shooter = aiactor.aiShooter;
            shooter.Inventory.AddGunToInventory(PickupObjectDatabase.GetById(49) as Gun, true);
            shooter.equippedGunId = 49;
            FieldInfo leEnabler = typeof(AIShooter).GetField("m_hasCachedGun", BindingFlags.Instance | BindingFlags.NonPublic);
            leEnabler.SetValue(shooter, false);
            shooter.Inventory.DestroyAllGuns();
            Gun equippedGun = shooter.Inventory.AddGunToInventory(PickupObjectDatabase.GetById(49) as Gun, true);
            FieldInfo fieldInf = typeof(AIShooter).GetField("m_cachedGun", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInf.SetValue(shooter, equippedGun);
            FieldInfo m_currentGunField = typeof(GunInventory).GetField("m_currentGun", BindingFlags.Instance | BindingFlags.NonPublic);
            m_currentGunField.SetValue(shooter.Inventory, equippedGun);
            equippedGun.gameObject.SetActive(true);



            CompanionFollowPlayerBehavior comp = new CompanionFollowPlayerBehavior();
            comp.CanRollOverPits = false;
            comp.DisableInCombat = true;
            comp.PathInterval = 0.25f;
            comp.IdealRadius = 9;
            comp.CatchUpRadius = 4;
            comp.CatchUpAccelTime = 3;
            comp.CatchUpSpeed = aiactor.MovementSpeed *= 1.6f;
            comp.CatchUpMaxSpeed = aiactor.MovementSpeed *= 1.9f;

            comp.TemporarilyDisabled = true;

            bs.MovementBehaviors.Add(comp);
            Contractors.Add(aiactor);

        }

        public static int ContractID;

        public static List<AIActor> Contractors = new List<AIActor>();
        public override CustomDungeonFlags FlagToSetOnStack => CustomDungeonFlags.CONTRACT_FLAG_STACK;




        public override void OnInitialPickup(PlayerController playerController)
        {
            GameManager.Instance.OnNewLevelFullyLoaded += this.OnNewFloorLoaded;
        }


        public override void OnStack(PlayerController playerController)
        {
            int h = UnityEngine.Random.Range(0, Contract.Contractors.Count);
            Color c = new Color(UnityEngine.Random.Range(0F, 1F), UnityEngine.Random.Range(0, 1F), UnityEngine.Random.Range(0, 1F));
            Boys.Add(new Tuple<int, Color>(h, c));
            if (playerController.CurrentRoom == null) { return; }
            SpawnTheBoys(h, c, playerController);
        }
        private Coroutine Coroutine;

        public List<Tuple<int, Color>> Boys = new List<Tuple<int, Color>>();

        private void OnNewFloorLoaded()
        {
            if (Coroutine != null) { this.StopCoroutine(Coroutine); Coroutine = null; }
            Coroutine = this.StartCoroutine(Wait());
        }
        private IEnumerator Wait()
        {
            while(_Owner.CurrentRoom == null)
            {
                yield return null;
            }
            ActiveBoys = new List<AIActor>();
            foreach (var entry in Boys)
            {
                SpawnTheBoys(entry.First, entry.Second, _Owner);
            }
            yield break;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GameManager.Instance.OnNewLevelFullyLoaded -= this.OnNewFloorLoaded;
        }

        private void SpawnTheBoys(int h, Color c, PlayerController playerController)
        {
            RoomHandler absoluteRoom = playerController.CurrentRoom;

            IntVector2? randomAvailableCell = absoluteRoom?.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 4), new CellTypes?(CellTypes.FLOOR), false, null);
            IntVector2? intVector = (randomAvailableCell == null) ? null : new IntVector2?(randomAvailableCell.GetValueOrDefault() + IntVector2.One);
            AIActor aiactor = AIActor.Spawn(Contract.Contractors[h], intVector != null ? intVector.Value : new IntVector2((int)playerController.transform.position.x, (int)playerController.transform.position.y), playerController.CurrentRoom, true, AIActor.AwakenAnimationType.Default, true);

            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX);
            gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(new Vector3(intVector.Value.x, intVector.Value.y), tk2dBaseSprite.Anchor.LowerCenter);
            gameObject2.transform.position = gameObject2.transform.position.Quantize(0.0625f);
            gameObject2.GetComponent<tk2dBaseSprite>().UpdateZDepth();


            CompanionController comp = aiactor.gameActor.GetComponent<CompanionController>();
            comp.Initialize(playerController);

            aiactor.CompanionOwner = playerController;
            aiactor.specRigidbody.Reinitialize();


            CustomScarfDoer scorf = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.ScarfObject.gameObject).AddComponent<CustomScarfDoer>();

            scorf.AttachTarget = aiactor;
            scorf.ScarfMaterial = new Material(StaticVFXStorage.ScarfObject.ScarfMaterial);
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

            scorf.ScarfMaterial.SetColor("_OverrideColor", c);

            scorf.Initialize(aiactor);
            ActiveBoys.Add(aiactor);
        }
        private List<AIActor> ActiveBoys = new List<AIActor>();

        public override void MidGameSerialize(List<object> data)
        {
            base.MidGameSerialize(data);
            if (isDummy) { return; }
            data.Add(Boys.Count);
            Debug.Log($"Cerealizing...");

            Debug.Log($"Count: {Boys.Count}");
            foreach (var entry in Boys)
            {
                data.Add(entry.First);
                Debug.Log($"Type: {entry.First}");

                string Color = $"{entry.Second.r}|{entry.Second.g}|{entry.Second.b}|1";
                Debug.Log($"Color: {Color}");

                data.Add(Color);
            }
            Debug.Log($"Cerealizing End.");

        }
        public override void MidGameDeserialize(List<object> data)
        {
            base.MidGameDeserialize(data);
            if (isDummy) { return; }
            Debug.Log($"Decerealizing...");

            int count = 0;
            int amountOfBoys = (int)data[count++];
            Debug.Log($"Count: {amountOfBoys}");

            List<int> BoyType = new List<int>();
            List<Color> BoyPride = new List<Color>();
            for (int i = 0; i < amountOfBoys; i++)
            {
                int Type = (int)data[count++];
                BoyType.Add(Type);
                Debug.Log($"Type: {Type}");

                Color c = new Color(1,1,1,1);
                string colorString = (string)data[count++];
                Debug.Log($"Color: {colorString}");

                int colorSplit = 0;
                foreach (var entry in colorString.Split('|'))
                {
                    float outInt;
                    bool parsedInt = float.TryParse(entry, out outInt);
                    if (parsedInt)
                    {
                        switch (colorSplit)
                        {
                            case 0:
                                c.r = outInt; break;
                            case 1:
                                c.g = outInt; break;
                            case 2:
                                c.b = outInt; break;
                            case 3:
                                c.a = outInt; break;
                        }
                    }
                    colorSplit++;
                }
                BoyPride.Add(c);
            }
            Debug.Log($"Decerealizing End.");

            SetStuffUp(amountOfBoys, BoyType, BoyPride);
        }
        public void SetStuffUp(int amountOfBoys, List<int> BoyType, List<Color> BoyPride)
        {
            Boys = new List<Tuple<int, Color>>();
            for (int i = 0; i < amountOfBoys; i++)
            {
                Boys.Add(new Tuple<int, Color>(BoyType[i], BoyPride[i]));
            }
        }
    }
}
