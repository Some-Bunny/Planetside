using UnityEngine;
using Gungeon;
using ItemAPI;
using SaveAPI;
using System.Collections.Generic;
using static UnityEngine.ParticleSystem;
using Dungeonator;
using System.Collections;
using HutongGames.PlayMaker.Actions;
using Brave.BulletScript;
using Planetside;
using static DirectionalAnimation;
using System.Reflection;

namespace Planetside
{
    class ParasiticHeart : GunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Parasitic Heart", "chambersoul");
            Game.Items.Rename("outdated_gun_mods:parasitic_heart", "psog:parasitic_heart");
            gun.gameObject.AddComponent<ParasiticHeart>();
            gun.SetShortDescription("Detached");
            gun.SetLongDescription("The heart of a parasitic beast that lurks, and once lurked in the Gungeon. The six souls it once housed are still yet to depart, and orbit the ever-beating heart.");
            gun.SetupSprite(null, "chambersoul_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.idleAnimation, 7);
            gun.SetAnimationFPS(gun.reloadAnimation, 9);

            gun.AddProjectileModuleFrom("38_special", true, false);
            gun.SetBaseMaxAmmo(200);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[2].eventAudio = "Play_Heartbeat";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[2].triggerEvent = true;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation).frames[2].eventAudio = "Play_Heartbeat";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation).frames[2].triggerEvent = true;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[3].eventAudio = "Play_Heartbeat";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[3].triggerEvent = true;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(33) as Gun).gunSwitchGroup;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.NAIL;
            gun.reloadTime = 3f;
            gun.DefaultModule.cooldownTime = .25f;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.numberOfShotsInClip = 25;
            gun.quality = PickupObject.ItemQuality.B;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.DefaultModule.angleVariance = 0f;
            gun.encounterTrackable.EncounterGuid = "hert";
            gun.sprite.IsPerpendicular = true;
            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage *= 0f;
            projectile.transform.parent = gun.barrelOffset;
            projectile.baseData.range *= 0;
            projectile.sprite.renderer.enabled = false;
            projectile.hitEffects.suppressMidairDeathVfx = true;
            projectile.AdditionalScaleMultiplier *= 0f;


            gun.gunClass = GunClass.NONE;

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            SynergyAPI.SynergyBuilder.AddItemToSynergy(gun, CustomSynergyType.CONTRAIL);
            ParasiticHeart.HeartID = gun.PickupObjectId;
            gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER, true);

            gun.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);

            ItemIDs.AddToList(gun.PickupObjectId);


            var ChamberObject = new GameObject("Chamber Soul");
            FakePrefab.MarkAsFakePrefab(ChamberObject);
            DontDestroyOnLoad(ChamberObject);

            var tk2d = ChamberObject.AddComponent<tk2dSprite>();
            var col = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("AnnihiChamberCollection").GetComponent<tk2dSpriteCollectionData>();
            tk2d.Collection = col;
            tk2d.SetSprite(col.GetSpriteIdByName("annihichamber_intro_014"));
            tk2d.SetColor(Color.white);

            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
            tk2d.renderer.material.SetFloat("_Fade", 0);

            var afterImage = tk2d.gameObject.AddComponent<ImprovedAfterImage>();
            afterImage.dashColor = new Color(0, 0, 0, 1);
            afterImage.shadowLifetime = 1;
            afterImage.shadowTimeDelay = 0.05f;


            SpeculativeRigidbody specBody = ChamberObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(0, 0), new IntVector2(49, 56));
            specBody.PixelColliders.Clear();
            specBody.CollideWithTileMap = false;
            specBody.PixelColliders.Add(new PixelCollider
            {

                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.EnemyHitBox,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = 8,
                ManualOffsetY = 11,
                ManualWidth = 50,
                ManualHeight = 50,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            specBody.PixelColliders.Add(new PixelCollider
            {

                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.Projectile,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = 8,
                ManualOffsetY = 11,
                ManualWidth = 50,
                ManualHeight = 50,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            chamberObject = ChamberObject;
        }
        public static GameObject chamberObject;
        public static int HeartID;

        private bool HasReloaded;

        public override void Update()
        {
            if (gun.CurrentOwner)
            {
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Play_ENM_beholster_intro_01", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                if (gun.ClipShotsRemaining == 0 && gun.CurrentAmmo != 0)
                {
                    var obj = Instantiate(chamberObject, player.sprite.WorldCenter - new Vector2(2,2), Quaternion.identity);
                    var that = obj.AddComponent<AnnihiChamberSoulBehavior>();
                    that.Angle = player.CurrentGun.CurrentAngle;
                }
            }
        }



        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
        }
    }

    public class AnnihiChamberSoulBehavior : MonoBehaviour
    {
        private Material ownMat;
        private RoomHandler room;
        private SpeculativeRigidbody Body;
        public float Angle = 0;


        public void Start()
        {
            room = this.transform.position.GetAbsoluteRoom();
            ownMat = this.gameObject.GetComponent<tk2dBaseSprite>().renderer.material;
            Body = this.gameObject.GetComponent<SpeculativeRigidbody>();
            Body.OnPreRigidbodyCollision += DoCollision;

            this.StartCoroutine(StartPew());
        }

        public IEnumerator StartPew()
        {
            float f = 0;
      
            while (f < 0.66f)
            {
                ownMat.SetFloat("_Fade", f);
                this.Body.Velocity = Vector2.Lerp(Vector2.zero, MathToolbox.GetUnitOnCircle(Angle, 30), MathToolbox.SinLerpTValue(f));
                f += BraveTime.DeltaTime;
                yield return null;
            }
            f = 0;         
            while (f < 2)
            {
               
                f += BraveTime.DeltaTime;
                yield return null;
            }
            f = 0;
            while (f < 0.5)
            {
                ownMat.SetFloat("_Fade", 0.5f -f );

                f += BraveTime.DeltaTime;
                yield return null;
            }
            Destroy(this.gameObject);

            yield break;
        }


        private void DoCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            PhysicsEngine.SkipCollision = true;
            if (otherRigidbody.gameObject.GetComponent<AIActor>())
            {
                if (otherRigidbody.aiActor.parentRoom == room)
                {
                    otherRigidbody.aiActor.healthHaver.ApplyDamage(2.25f, Vector2.zero, "CHAMBER", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                }
            }
        }
    }
}


/*
namespace Vammopires
{
    class VammopireLord
    {
        public static GameObject prefab;
        //Always make sure to give your enemy a unique guid. This is essentially the id of your enemy and is integral for many parts of EnemyAPI
        public static readonly string guid = "vammopirelord";
        private static tk2dSpriteCollectionData VLCollection;
        //This shootpoint gameObject determines well, where the enemy shoots from, I'll explain more when we get to the AttackBehaviors.

        public static void Init()
        {
            //As always don't forget to initalize your enemy. 
            VammopireLord.BuildPrefab();
        }
        public static void BuildPrefab()
        {

            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {
                //Sets up the prefab of the enemy. The spritepath, "VammopiresItems/Resources/VammopireLord/Idle/VammopireLord_idle_001", determines the setup sprite for your enemy. vvvv This bool right here determines whether or not an enemy has an AiShooter or not. AIShooters are necessary if you want your enemy to hold a gun for example. An example of this can be seen in Humphrey.
                prefab = EnemyBuilder.BuildPrefab("vammopirelord", guid, "Vammopires/Resources/VammopireLord/IdleLeft/ArchVammopire_Idle_01", new IntVector2(0, 0), new IntVector2(22, 45), false, true);
                ETGModConsole.Log("Fic 1");
                //This line extends a BraveBehavior called EnemyBehavior, this is a generic behavior I use for setting up things that can't be setup in BuildPrefab.
                var enemy = prefab.AddComponent<EnemyBehavior>();
                //Here you can setup various things like movement speed, weight, and health. There's a lot you can do with the AiActor parameter so feel free to experiment.
                enemy.aiActor.MovementSpeed = 1f;
                enemy.aiActor.knockbackDoer.weight = 40;
                enemy.aiActor.HasShadow = true;
                enemy.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("c00390483f394a849c36143eb878998f").ShadowObject;
                enemy.aiActor.IgnoreForRoomClear = true;
                enemy.aiActor.CollisionDamage = 1f;
                enemy.aiActor.CollisionKnockbackStrength = 1f;
                enemy.aiActor.PreventFallingInPitsEver = true;
                enemy.aiActor.CanTargetPlayers = true;
                enemy.aiActor.healthHaver.ForceSetCurrentHealth(60f);
                enemy.aiActor.healthHaver.SetHealthMaximum(100f, null, false);
                enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 0,
                    ManualOffsetY = 0,
                    ManualWidth = 22,
                    ManualHeight = 45,
                    ManualDiameter = 0,
                    ManualLeftX = 0,
                    ManualLeftY = 0,
                    ManualRightX = 0,
                    ManualRightY = 0
                });
                enemy.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyHitBox,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 0,
                    ManualOffsetY = 0,
                    ManualWidth = 22,
                    ManualHeight = 45,
                    ManualDiameter = 0,
                    ManualLeftX = 0,
                    ManualLeftY = 0,
                    ManualRightX = 0,
                    ManualRightY = 0
                });
                //This is where you setup your animations. Most animations need specific frame names to be recognized like idle or die. 
                //The AddAnimation lines gets sprites from the folder specified in second phrase of the this line. At the very least you need an animation that contains the word idle for the idle animations for example.
                //AnimationType determines what kind of animation your making. In Gungeon there are 7 different Animation Types: Move, Idle, Fidget, Flight, Hit, Talk, Other. For a majority of these animations, these play automatically, however specific animations need to be told when to play such as Attack.
                //DirectionType determines the amount of ways an animation can face. You'll have to change your animation names to correspond with the DirectionType. For example if you want an animation to face eight ways you'll have to name your animations something like ""attack_south_west", "attack_north_east",  "attack_east", "attack_south_east",  "attack_north",  "attack_south", "attack_west", "attack_north_west" and change DirectionType to  DirectionType.EightWayOrdinal.
                //I suggest looking at the sprites of base game enemies to determine the names for the different directions.

                bool flag3 = VLCollection == null;
                if (flag3)
                {
                    VLCollection = SpriteBuilder.ConstructCollection(prefab, "Vammopire_Lord_Collection");
                    UnityEngine.Object.DontDestroyOnLoad(VLCollection);
                    for (int i = 0; i < spritePaths.Length; i++)
                    {
                        SpriteBuilder.AddSpriteToCollection(spritePaths[i], VLCollection);
                    }
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, VLCollection, new List<int>
                    {
                0,
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                    }, "attack_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, VLCollection, new List<int>
                    {
                8,
                9,
                10,
                11,
                12,
                13,
                14,
                15
                    }, "attack_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, VLCollection, new List<int>
                    {
                16,
                17,
                18,
                19,
                20,
                21,
                22,
                23,
                24,
                25,
                26,
                27,
                    }, "die_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, VLCollection, new List<int>
                    {
                28,
                29,
                30,
                31,
                32,
                33,
                34,
                35,
                36,
                37,
                38,
                39
                    }, "die_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, VLCollection, new List<int>
                    {
                40,
                41,
                42,
                43,
                44
                    }, "idle_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, VLCollection, new List<int>
                    {
                45,
                46,
                47,
                48,
                49
                    }, "idle_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, VLCollection, new List<int>
                    {
                50,
                51,
                52,
                53,
                54,
                55,
                56,
                57,
                58,
                59,
                60
                    }, "summon_left", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;
                    SpriteBuilder.AddAnimation(enemy.spriteAnimator, VLCollection, new List<int>
                    {
                61,
                62,
                63,
                64,
                65,
                66,
                67,
                68,
                69,
                70,
                71
                    }, "summon_right", tk2dSpriteAnimationClip.WrapMode.Once).fps = 6f;

                }

                ETGModConsole.Log("Fic 2");
                //Here we create a new DirectionalAnimation for our enemy to pull from. 
                //Make sure the AnimNames correspong to the AddAnimation names.
                DirectionalAnimation attack = new DirectionalAnimation()
                {
                    AnimNames = new string[] { "attack_right", "attack_left" },
                    Flipped = new FlipType[] { FlipType.None, FlipType.None },
                    Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
                    Prefix = string.Empty
                };
                DirectionalAnimation summon = new DirectionalAnimation()
                {
                    AnimNames = new string[] { "summon_right", "summon_left" },
                    Flipped = new FlipType[] { FlipType.None, FlipType.None },
                    Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
                    Prefix = string.Empty
                };
                DirectionalAnimation die = new DirectionalAnimation()
                {
                    AnimNames = new string[] { "die_right", "die_left" },
                    Flipped = new FlipType[] { FlipType.None, FlipType.None },
                    Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
                    Prefix = string.Empty
                };
                DirectionalAnimation idle = new DirectionalAnimation()
                {
                    AnimNames = new string[] { "idle_right", "idle_left" },
                    Flipped = new FlipType[] { FlipType.None, FlipType.None },
                    Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
                    Prefix = string.Empty
                };
                ETGModConsole.Log("Fic 3");
                //Because Dodge Roll is Dodge Roll and there is no animation types for attack and death, we have to assign them to the Other category.
                enemy.aiAnimator.AssignDirectionalAnimation("attack", attack, AnimationType.Other);
                enemy.aiAnimator.AssignDirectionalAnimation("summon", summon, AnimationType.Other);
                enemy.aiAnimator.AssignDirectionalAnimation("die", die, AnimationType.Other);
                enemy.aiAnimator.IdleAnimation = idle;
                enemy.aiAnimator.MoveAnimation = idle;
                //This is where we get into the meat and potatoes of our enemy. This is where all the behaviors of our enemy are made.
                //This shootpoint block of code determines where our bullets will orginate from. In this case, the center of the enemy.

                //this line adds a BehaviorSpeculator to our enemy which is the base for adding behaviors on to.
                var bs = prefab.GetComponent<BehaviorSpeculator>();

                var m_CachedGunAttachPoint = new GameObject("attach");
                m_CachedGunAttachPoint.transform.parent = enemy.transform;
                m_CachedGunAttachPoint.transform.position = enemy.sprite.WorldCenter;
                //Here we will add some basic behaviors such as TargetPlayerBehavior and SeekTargetBehavior.
                //You can change many things in these behaviors so feel free to go nuts.
                ETGModConsole.Log("Fic 3.5");
                BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
                bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
                bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;
                bs.TargetBehaviors = new List<TargetBehaviorBase>
            {
                new TargetPlayerBehavior
                {
                    Radius = 35f,
                    LineOfSight = true,
                    ObjectPermanence = true,
                    SearchInterval = 0.25f,
                    PauseOnTargetSwitch = false,
                    PauseTime = 0.25f,


                }
            };
                ETGModConsole.Log("Fic 3.6");

                bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>()
                {

                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 1f,
                                                Behavior = new ShootBehavior{
                        ShootPoint = m_CachedGunAttachPoint,
                        ReaimOnFire = true,
                        BulletScript = new CustomBulletScriptSelector(typeof(VammopireLordScript2)),
                        LeadAmount = 0f,
                        AttackCooldown = 1.5f,
                        ChargeAnimation = "summon",
                        RequiresLineOfSight = false,
                        StopDuring = ShootBehavior.StopType.Attack,
                        Uninterruptible = true,

                        },
                        NickName = "spawnlesserbois"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0.8f,
                        Behavior = new ShootBehavior() {
                    ShootPoint = m_CachedGunAttachPoint,
					//This line selects our Bullet Script
					BulletScript = new CustomBulletScriptSelector(typeof(SwarmScript)),
                    LeadAmount = 0f,
                    AttackCooldown = 5f,
                    ChargeAnimation = "attack",
                    RequiresLineOfSight = true,
                    Uninterruptible = false,
                    ImmobileDuringStop = true,
                    StopDuring = ShootBehavior.StopType.Attack,
                        },
                        NickName = "Summonus Fuckyouius"
                    },
                };
                //Now this is one of the most important behaviors because it allows our enemy to shoot.



                ETGModConsole.Log("Fic 4");


                //Adds the enemy to MTG spawn pool and spawn command
                Game.Enemies.Add("dt:vammopire_lord", enemy.aiActor);
                ETGModConsole.Log("Fic 5");
                SpriteBuilder.AddSpriteToCollection("Vammopires/Resources/VammopireLord/ArchVammopire_ammo", SpriteBuilder.ammonomiconCollection);
                if (enemy.GetComponent<EncounterTrackable>() != null)
                {
                    UnityEngine.Object.Destroy(enemy.GetComponent<EncounterTrackable>());
                }
                enemy.encounterTrackable = enemy.gameObject.AddComponent<EncounterTrackable>();
                enemy.encounterTrackable.journalData = new JournalEntry();
                enemy.encounterTrackable.EncounterGuid = "dt:vammopire_lord";
                enemy.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
                enemy.encounterTrackable.journalData.SuppressKnownState = false;
                enemy.encounterTrackable.journalData.IsEnemy = true;
                enemy.encounterTrackable.journalData.SuppressInAmmonomicon = false;
                enemy.encounterTrackable.ProxyEncounterGuid = "";
                enemy.encounterTrackable.journalData.AmmonomiconSprite = "Vammopires/Resources/VammopireLord/ArchVammopire_ammo";
                enemy.encounterTrackable.journalData.enemyPortraitSprite = Alexandria.ItemAPI.ResourceExtractor.GetTextureFromResource("Vammopires\\Resources\\VammopireLord\\ArchVammopire_ammonomicon.png");
                Module.Strings.Enemies.Set("#THE_VAMMOPIRE_LORD", "Vammopire Lord");
                Module.Strings.Enemies.Set("#THE_VAMMOPIRE_LORD_SHORTDESC", "Quickdrawing Blood");
                Module.Strings.Enemies.Set("#THE_VAMMOPIRE_LORD_LONGDESC", "An aged Vammopire Lord that has been found worthy by Kaliber, the Vammopire Lord emerges from her shrines of blood with a body molded by her very hands. Where one Vammopire Lord Lord is found, lesser Vammopires are not far, serving their master in the hopes of one day being seen by Kaliber, and being chosen to become a Lord themselves.\n\nBeware, for this age-old creature has learned to saciate its ammo-lust by draining all weapons unfortunate enough to be brought near it. For its bloodlust, however, it employs even more unusual methods...");
                enemy.encounterTrackable.journalData.PrimaryDisplayName = "#THE_VAMMOPIRE_LORD";
                enemy.encounterTrackable.journalData.NotificationPanelDescription = "#THE_VAMMOPIRE_LORD_SHORTDESC";
                enemy.encounterTrackable.journalData.AmmonomiconFullEntry = "#THE_VAMMOPIRE_LORD_LONGDESC";
                EnemyBuilder.AddEnemyToDatabase(enemy.gameObject, "dt:vammopire_lord");
                EnemyDatabase.GetEntry("dt:vammopire_lord").ForcedPositionInAmmonomicon = 44;
                EnemyDatabase.GetEntry("dt:vammopire_lord").isInBossTab = false;
                EnemyDatabase.GetEntry("dt:vammopire_lord").isNormalEnemy = true;
                ETGModConsole.Log("Fic 6");
            }
        }
        private static string[] spritePaths = new string[]
        {
			//Attack Left
			"Vammopires/Resources/VammopireLord/AttackLeft/ArchVammopire_Attack_01",
            "Vammopires/Resources/VammopireLord/AttackLeft/ArchVammopire_Attack_02",
            "Vammopires/Resources/VammopireLord/AttackLeft/ArchVammopire_Attack_03",
            "Vammopires/Resources/VammopireLord/AttackLeft/ArchVammopire_Attack_04",
            "Vammopires/Resources/VammopireLord/AttackLeft/ArchVammopire_Attack_05",
            "Vammopires/Resources/VammopireLord/AttackLeft/ArchVammopire_Attack_06",
            "Vammopires/Resources/VammopireLord/AttackLeft/ArchVammopire_Attack_07",
            "Vammopires/Resources/VammopireLord/AttackLeft/ArchVammopire_Attack_08",

			//Attack Right
			"Vammopires/Resources/VammopireLord/AttackRight/ArchVammopire_Attack_01",
            "Vammopires/Resources/VammopireLord/AttackRight/ArchVammopire_Attack_02",
            "Vammopires/Resources/VammopireLord/AttackRight/ArchVammopire_Attack_03",
            "Vammopires/Resources/VammopireLord/AttackRight/ArchVammopire_Attack_04",
            "Vammopires/Resources/VammopireLord/AttackRight/ArchVammopire_Attack_05",
            "Vammopires/Resources/VammopireLord/AttackRight/ArchVammopire_Attack_06",
            "Vammopires/Resources/VammopireLord/AttackRight/ArchVammopire_Attack_07",
            "Vammopires/Resources/VammopireLord/AttackRight/ArchVammopire_Attack_08",

			//Die Left
			"Vammopires/Resources/VammopireLord/DieLeft/ArchVammopire_Death_01",
            "Vammopires/Resources/VammopireLord/DieLeft/ArchVammopire_Death_02",
            "Vammopires/Resources/VammopireLord/DieLeft/ArchVammopire_Death_03",
            "Vammopires/Resources/VammopireLord/DieLeft/ArchVammopire_Death_04",
            "Vammopires/Resources/VammopireLord/DieLeft/ArchVammopire_Death_05",
            "Vammopires/Resources/VammopireLord/DieLeft/ArchVammopire_Death_06",
            "Vammopires/Resources/VammopireLord/DieLeft/ArchVammopire_Death_07",
            "Vammopires/Resources/VammopireLord/DieLeft/ArchVammopire_Death_08",
            "Vammopires/Resources/VammopireLord/DieLeft/ArchVammopire_Death_09",
            "Vammopires/Resources/VammopireLord/DieLeft/ArchVammopire_Death_10",
            "Vammopires/Resources/VammopireLord/DieLeft/ArchVammopire_Death_11",
            "Vammopires/Resources/VammopireLord/DieLeft/ArchVammopire_Death_12",

			//Die Right
			"Vammopires/Resources/VammopireLord/DieRight/ArchVammopire_Death_01",
            "Vammopires/Resources/VammopireLord/DieRight/ArchVammopire_Death_02",
            "Vammopires/Resources/VammopireLord/DieRight/ArchVammopire_Death_03",
            "Vammopires/Resources/VammopireLord/DieRight/ArchVammopire_Death_04",
            "Vammopires/Resources/VammopireLord/DieRight/ArchVammopire_Death_05",
            "Vammopires/Resources/VammopireLord/DieRight/ArchVammopire_Death_06",
            "Vammopires/Resources/VammopireLord/DieRight/ArchVammopire_Death_07",
            "Vammopires/Resources/VammopireLord/DieRight/ArchVammopire_Death_08",
            "Vammopires/Resources/VammopireLord/DieRight/ArchVammopire_Death_09",
            "Vammopires/Resources/VammopireLord/DieRight/ArchVammopire_Death_10",
            "Vammopires/Resources/VammopireLord/DieRight/ArchVammopire_Death_11",
            "Vammopires/Resources/VammopireLord/DieRight/ArchVammopire_Death_12",

			//IdleLeft
			"Vammopires/Resources/VammopireLord/IdleLeft/ArchVammopire_Idle_01",
            "Vammopires/Resources/VammopireLord/IdleLeft/ArchVammopire_Idle_02",
            "Vammopires/Resources/VammopireLord/IdleLeft/ArchVammopire_Idle_03",
            "Vammopires/Resources/VammopireLord/IdleLeft/ArchVammopire_Idle_04",
            "Vammopires/Resources/VammopireLord/IdleLeft/ArchVammopire_Idle_05",

		    //IdleRight
			"Vammopires/Resources/VammopireLord/IdleRight/ArchVammopire_Idle_01",
            "Vammopires/Resources/VammopireLord/IdleRight/ArchVammopire_Idle_02",
            "Vammopires/Resources/VammopireLord/IdleRight/ArchVammopire_Idle_03",
            "Vammopires/Resources/VammopireLord/IdleRight/ArchVammopire_Idle_04",
            "Vammopires/Resources/VammopireLord/IdleRight/ArchVammopire_Idle_05",

			//SummonLeft
			"Vammopires/Resources/VammopireLord/SummonLeft/ArchVammopire_Summon_01",
            "Vammopires/Resources/VammopireLord/SummonLeft/ArchVammopire_Summon_02",
            "Vammopires/Resources/VammopireLord/SummonLeft/ArchVammopire_Summon_03",
            "Vammopires/Resources/VammopireLord/SummonLeft/ArchVammopire_Summon_04",
            "Vammopires/Resources/VammopireLord/SummonLeft/ArchVammopire_Summon_05",
            "Vammopires/Resources/VammopireLord/SummonLeft/ArchVammopire_Summon_06",
            "Vammopires/Resources/VammopireLord/SummonLeft/ArchVammopire_Summon_07",
            "Vammopires/Resources/VammopireLord/SummonLeft/ArchVammopire_Summon_08",
            "Vammopires/Resources/VammopireLord/SummonLeft/ArchVammopire_Summon_09",
            "Vammopires/Resources/VammopireLord/SummonLeft/ArchVammopire_Summon_10",
            "Vammopires/Resources/VammopireLord/SummonLeft/ArchVammopire_Summon_11",

			//SummonRight
			"Vammopires/Resources/VammopireLord/SummonRight/ArchVammopire_Summon_01",
            "Vammopires/Resources/VammopireLord/SummonRight/ArchVammopire_Summon_02",
            "Vammopires/Resources/VammopireLord/SummonRight/ArchVammopire_Summon_03",
            "Vammopires/Resources/VammopireLord/SummonRight/ArchVammopire_Summon_04",
            "Vammopires/Resources/VammopireLord/SummonRight/ArchVammopire_Summon_05",
            "Vammopires/Resources/VammopireLord/SummonRight/ArchVammopire_Summon_06",
            "Vammopires/Resources/VammopireLord/SummonRight/ArchVammopire_Summon_07",
            "Vammopires/Resources/VammopireLord/SummonRight/ArchVammopire_Summon_08",
            "Vammopires/Resources/VammopireLord/SummonRight/ArchVammopire_Summon_09",
            "Vammopires/Resources/VammopireLord/SummonRight/ArchVammopire_Summon_10",
            "Vammopires/Resources/VammopireLord/SummonRight/ArchVammopire_Summon_11",











        };

        public class EnemyBehavior : BraveBehaviour
        {
            //This determines that the enemy is active when a player is in the room
            private RoomHandler m_StartRoom;
            private void Update()
            {
                if (!base.aiActor.HasBeenEngaged) { CheckPlayerRoom(); }
                if (base.aiActor.HasBeenEngaged)
                {
                    foreach (PlayerController playerController in GameManager.Instance.AllPlayers)
                    {
                        if (playerController && Vector2.Distance(playerController.CenterPosition, base.aiActor.sprite.WorldCenter.ToVector3ZisY(0f)) < num)
                        {
                            if (!playerController.healthHaver.IsDead)
                            {
                                if (!playerController.spriteAnimator.QueryInvulnerabilityFrame() && playerController.healthHaver.IsVulnerable)
                                {
                                    playerController.CurrentStoneGunTimer = 1f;
                                    if (playerController.CurrentStoneGunTimer <= 0.1f)
                                    {
                                        //AkSoundEngine.PostEvent("Play_ENM_jammer_curse_01", base.gameObject);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            private void CheckPlayerRoom()
            {
                if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom) { base.aiActor.HasBeenEngaged = true; }
            }
            private void Start()
            {
                m_StartRoom = aiActor.GetAbsoluteParentRoom();
                aiActor.knockbackDoer.SetImmobile(true, "LyncSaysSo");
                aiActor.SetIsFlying(true, "enemyStart", true, true);
                var m_playerRadialIndicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), base.aiActor.sprite.WorldCenter.ToVector3ZisY(0f),
                Quaternion.identity, base.aiActor.transform)).GetComponent<HeatIndicatorController>();
                m_playerRadialIndicator.IsFire = false;
                m_playerRadialIndicator.CurrentRadius = 7.5f;
                m_playerRadialIndicator.CurrentColor = Color.red;
                num = m_playerRadialIndicator.CurrentRadius;
                //This line determines what happens when an enemy dies. For now it's something simple like playing a death sound.
                //A full list of all the sounds can be found in the SFX.txt document that comes with this github.
                base.aiActor.healthHaver.OnPreDeath += (obj) => { AkSoundEngine.PostEvent("Play_VO_kali_death_01", base.aiActor.gameObject); };

            }


        }

        static float num = 1;




    }
    public class VammopireLordScript2 : Script
    {

        public override IEnumerator Top()
        {


            IntVector2 bestLocation = base.BulletBank.aiActor.GetAbsoluteParentRoom().GetRandomVisibleClearSpot(2, 2);
            AIActor vampbullet = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid("lesservampire"), bestLocation, base.BulletBank.aiActor.GetAbsoluteParentRoom(), true);
            vampbullet.CollisionDamage = 0f;
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(vampbullet.specRigidbody, null, false);
            vampbullet.HandleReinforcementFallIntoRoom(0f);
            Wait(30f);

            IntVector2 bestLocation2 = base.BulletBank.aiActor.GetAbsoluteParentRoom().GetRandomVisibleClearSpot(2, 2);
            AIActor vampbullet2 = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid("lesservampire"), bestLocation, base.BulletBank.aiActor.GetAbsoluteParentRoom(), true);
            vampbullet2.CollisionDamage = 0f;
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(vampbullet2.specRigidbody, null, false);
            vampbullet2.HandleReinforcementFallIntoRoom(0f);

            Wait(30f);





            yield break;
        }
    }

    public class SwarmScript : Script
    {
        public override IEnumerator Top()
        {

            //In this line we're going to take the fiery bullet from Muzzle Flare and it to our enemy.
            //In this case, the giery bullet is the default bullet of the Muzzle Flare.
            //Try looking up the bullet scripts of other enemies in DnSpy to try to find thier bullet names.
            //The Guid of all enemies can be found in the "enemies" file of the github.
            if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody) { base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("2feb50a6a40f4f50982e89fd276f6f15").bulletBank.GetBullet("self")); }
            //Here the code is going to count each "i" and fire a bullet from each "i" found.

            //Here we can determine the direction of our bullet, speed, and assign a Bullet class. 
            //Bullets classes allow you to do some wacky shit, but for now, lets keep it simple.
            //Here the direction the bullet will be fired in will be a 40 degree angle * for each "i" away from the aim of our enemy, which a speed of 4.
            for (int i = 0; i <= 12; i++)
            {
                int speed = UnityEngine.Random.Range(1, 9);
                int offsetDir = UnityEngine.Random.Range(-50, 50);
                base.Fire(new Direction(0 + offsetDir, Brave.BulletScript.DirectionType.Aim, -1f), new Speed(speed, SpeedType.Absolute), new SwarmShots());
            }
            yield break;
        }


    }
    public class SwarmShots : Bullet
    {

        // Token: 0x06000A91 RID: 2705 RVA: 0x00030B38 File Offset: 0x0002ED38
        public SwarmShots() : base("self", false, false, false)
        {
        }


    }

}
*/