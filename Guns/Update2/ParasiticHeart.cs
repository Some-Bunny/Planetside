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
using Alexandria.Assetbundle;

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
            gun.SetLongDescription("The heart of a parasitic beast that lurks deep within the Gungeon. The six souls it once housed are still yet to depart, and orbit the ever-beating heart.");


            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "chambersoul_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 1;

            gun.reloadAnimation = "parasiteheart_reload";
            gun.idleAnimation = "parasiteheart_idle";
            gun.shootAnimation = "parasiteheart_fire";

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


