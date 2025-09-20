using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dungeonator;
using ItemAPI;
using UnityEngine;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Planetside;
using BreakAbleAPI;
using Brave.BulletScript;
using System.Collections;
using UnityEngine.Playables;
using Alexandria.PrefabAPI;
using Planetside.Static_Storage;
using HutongGames.PlayMaker.Actions;

namespace Planetside
{

    public class TrespassSniperTurretsController : MonoBehaviour
    {
        public void Start()
        {
            shootPosition = this.transform.Find("laserPoint").transform;
            CurrentState = State.PRIMED;
            bulletBank = this.GetComponent<AIBulletBank>();
            currentRoom = this.transform.position.GetAbsoluteRoom();
            this.StartCoroutine(FrameDelay());
        }


        public Func<SpeculativeRigidbody, bool> Excluder = (SpeculativeRigidbody otherRigidbody) =>
        {
            if (otherRigidbody.minorBreakable) { return otherRigidbody.minorBreakable; }
            if (otherRigidbody.gameActor is PlayerController player)
            {
                return player.IsStealthed || player.IsEthereal || player.IsGhost;
            }
            return false;
        };

        public IEnumerator FrameDelay()
        {
            yield return new WaitForSeconds(0.5f);

            if (laserPointer == null)
            {
                laserPointer = SpawnManager.SpawnVFX((GameObject)BraveResources.Load("Global VFX/VFX_LaserSight_Enemy"), true);
                laserPointer.transform.position = shootPosition.transform.position;
                laserPointer.transform.parent = shootPosition.gameObject.transform;
                laserPointerTiledSprite = laserPointer.GetComponent<tk2dTiledSprite>();
                laserPointerTiledSprite.HeightOffGround = 11;
                laserPointerTiledSprite.renderer.enabled = true;
                laserPointerTiledSprite.transform.localRotation = Quaternion.Euler(0f, 0f, ReturnDirection().ToAngle());
                laserPointerTiledSprite.usesOverrideMaterial = true;
                laserPointerTiledSprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                laserPointerTiledSprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                laserPointerTiledSprite.renderer.material.SetFloat("_EmissivePower", 500);
                laserPointerTiledSprite.renderer.material.SetFloat("_EmissiveColorPower", 1);
                laserPointerTiledSprite.renderer.material.SetColor("_OverrideColor", Color.cyan);
                laserPointerTiledSprite.renderer.material.SetColor("_EmissiveColor", Color.cyan);
            }
            yield break;
        }

        public void Update()
        {
            if (laserPointer != null && this.gameObject != null)
            {
                if (TrapDefusalKit.TrapsShouldBeDefused() == true)
                {
                    laserPointerTiledSprite.dimensions = new Vector2(0, 0f);
                    return;
                }
                DoRayCast();
            }
        }

        public void DoRayCast()
        {
            float num9 = float.MaxValue;
            CollisionLayer layer2 = CollisionLayer.PlayerHitBox;
            int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle ,CollisionLayer.BulletBlocker, layer2);
            RaycastResult raycastResult2;
            if (PhysicsEngine.Instance.Raycast(shootPosition.transform.PositionVector2(), ReturnDirection(), 100, out raycastResult2, true, true, rayMask2, null, false, Excluder, null))
            {
                num9 = raycastResult2.Distance;
                if (raycastResult2.SpeculativeRigidbody && raycastResult2.OtherPixelCollider.CollisionLayer == layer2)
                {
                    if (CurrentState == State.PRIMED)
                    {
                        Shoot();
                    }            
                }
            }
            RaycastResult.Pool.Free(ref raycastResult2);
            laserPointerTiledSprite.dimensions = new Vector2((num9 / 0.0625f), 1f);
            laserPointerTiledSprite.ForceRotationRebuild();
            laserPointerTiledSprite.UpdateZDepth();
        }

        public void Shoot()
        {
            CurrentState = State.ABOUT_TO_FIRE;
            GameManager.Instance.StartCoroutine(StartShoot());
        }


        private IEnumerator StartShoot()
        {
            float elaWait = 0f;
            float duraWait = 1f;
         
            while (elaWait < duraWait)
            {
                bool enabled = elaWait % 0.33f > 0.16f;
                if (this== null) { yield break; }
                if (laserPointerTiledSprite.gameObject == null) { yield break; }
                if (this.gameObject == null) { yield break; }
                laserPointerTiledSprite.renderer.enabled = enabled;
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            if (this == null) { yield break; }
            if (laserPointerTiledSprite.gameObject == null) { yield break; }
            if (this.gameObject != null)
            {
                if (laserPointerTiledSprite.gameObject == null) { yield break; }
                laserPointerTiledSprite.renderer.enabled = false;
                for (int k = 0; k < 3; k++)
                {
                    GameObject gameObject = SpawnManager.SpawnProjectile(this.bulletBank.Bullets[k].BulletObject, shootPosition.position, Quaternion.Euler(0f, 0f, ReturnDirection().ToAngle()), true);
                    Projectile component = gameObject.GetComponent<ThirdDimensionalProjectile>();
                    component.baseData.speed = 30;
                    component.UpdateSpeed();
                    component.Shooter = this.GetComponent<SpeculativeRigidbody>();
                    component.IgnoreTileCollisionsFor(0.03f);
                    component.pierceMinorBreakables = true;
                    component.collidesWithEnemies = false;
                    SpawnManager.PoolManager.Remove(component.gameObject.transform);
                    component.BulletScriptSettings.preventPooling = true;


                    CurrentState = State.POST_FIRE;
                    AkSoundEngine.PostEvent("Play_AbyssBlast", this.gameObject);

                    CoinRicoshotComponent CrC = component.AddComponent<CoinRicoshotComponent>();
                    CrC.obj = this.gameObject;
                    CrC.OnReflected += (obj) =>
                    {
                        component.baseData.damage = 50f;
                        Vector2 Point1 = MathToolbox.GetUnitOnCircle(component.Direction.ToAngle() + 180, 0.1f);
                        component.Direction = Point1;

                        component.baseData.speed = 500;
                        component.UpdateSpeed();

                        ImprovedAfterImage yes = component.gameObject.AddComponent<ImprovedAfterImage>();
                        yes.spawnShadows = true;
                        yes.shadowLifetime = 0.4f;
                        yes.shadowTimeDelay = 0.01f;
                        yes.dashColor = new Color(0.9f, 0.5f, 0.3f, 1f);
                        CrC.OnDestroyed += () =>
                        {
                            AkSoundEngine.PostEvent("Play_RockBreaking", component.gameObject);
                            GameObject vfx = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.DragunBoulderLandVFX, component.transform.position, Quaternion.identity);
                            tk2dBaseSprite sprite = vfx.GetComponent<tk2dBaseSprite>();
                            sprite.PlaceAtPositionByAnchor(component.transform.position, tk2dBaseSprite.Anchor.MiddleCenter);
                            sprite.HeightOffGround = 35f;
                            sprite.UpdateZDepth();
                            Destroy(vfx.gameObject, 2.5f);
                            Destroy(this.gameObject);
                        };
                    };

                    if (muzzleFlashPrefab != null)
                    {
                        GameObject vfx = SpawnManager.SpawnVFX(muzzleFlashPrefab, true);
                        vfx.transform.position = shootPosition.transform.position;
                        vfx.transform.localRotation = Quaternion.Euler(0f, 0f, ReturnDirection().ToAngle());
                        vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                        Destroy(vfx, 2);
                    }

                    elaWait = 0f;
                    duraWait = 0.125f;
                    while (elaWait < duraWait)
                    {
                        if (this == null) { yield break; }
                        if (laserPointerTiledSprite.gameObject == null) { yield break; }
                        if (this.gameObject == null) { yield break; }
                        elaWait += BraveTime.DeltaTime;
                        yield return null;
                    }
                }
            }
            elaWait = 0f;
            duraWait = 4f;
            while (elaWait < duraWait)
            {
                if (this == null) { yield break; }
                if (laserPointerTiledSprite.gameObject == null) { yield break; }
                if (this.gameObject == null) { yield break; }
                if (elaWait > (duraWait-0.5f)) { laserPointerTiledSprite.renderer.enabled = true; }
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            CurrentState = State.PRIMED;
            yield break;
        }


        public Vector2 ReturnDirection()
        {       
            return MathToolbox.GetUnitOnCircle(DirectionToFire, 1);
        }

        public AIBulletBank bulletBank;
        public RoomHandler currentRoom;
        public GameObject muzzleFlashPrefab;

        public Transform shootPosition;
        public GameObject laserPointer;
        public tk2dTiledSprite laserPointerTiledSprite;
        public float SizeOffset = 1;

        public float DirectionToFire;
        public State CurrentState;
        public enum State
        {
            PRIMED,
            ABOUT_TO_FIRE,
            POST_FIRE,
        }
    }




    public class TrespassSniperTurrets
    {
        public static void Init()
        {


            AIBulletBank.Entry entry1 = StaticBulletEntries.CopyBulletBankEntry(StaticBulletEntries.undodgeableBigBullet, "turretShot1");
            AIBulletBank.Entry entry2 = StaticBulletEntries.CopyBulletBankEntry(StaticBulletEntries.undodgeableDefault, "turretShot2");
            AIBulletBank.Entry entry3 = StaticBulletEntries.CopyBulletBankEntry(StaticBulletEntries.undodgeableSmallSpore, "turretShot3");



            //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000,  true, 0, 0, 0, 0, true, null, null, true, null);
            var Turret_Front = PrefabBuilder.BuildObject("Trespass Turret Front");
            Turret_Front.layer = Layers.FG_Critical;
            var sprite = Turret_Front.AddComponent<tk2dSprite>();
            sprite.SortingOrder = 2;
            var animator = Turret_Front.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName("trespassTurret_front");
            sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 8f);
            mat.SetFloat("_EmissivePower", 4);
            sprite.renderer.material = mat;
            sprite.IsPerpendicular = false;

            BreakableAPI_Bundled.GenerateTransformObject(Turret_Front.gameObject, new Vector2(0.5f, 0.875f), "laserPoint");
            AIBulletBank bulletBankLeft = Turret_Front.gameObject.AddComponent<AIBulletBank>();
            TrespassSniperTurretsController t = Turret_Front.gameObject.AddComponent<TrespassSniperTurretsController>();
            Turret_Front.gameObject.AddComponent<TresspassLightController>();
            t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(576) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
            Turret_Front.gameObject.AddComponent<PushImmunity>();

            t.DirectionToFire = Vector2.down.ToAngle();
            bulletBankLeft.Bullets = new List<AIBulletBank.Entry>()
            {
                entry1,
                entry2,
                entry3
            };
            StaticReferences.StoredRoomObjects.Add("TrespassSniperTurretFront", Turret_Front.gameObject);

            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("PSOG_TrespassSniperTurretFront", Turret_Front.gameObject);



            MakeLeft(entry1, entry2, entry3);
            MakeRight(entry1, entry2, entry3);
        }

        public static void MakeLeft(AIBulletBank.Entry entry1, AIBulletBank.Entry entry2, AIBulletBank.Entry entry3)
        {
            /*
            string defaultFrontPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassSniper/Left/";
            string[] idlePaths = new string[]
            {
                defaultFrontPath+"sniperturret_left_idle1.png",
                defaultFrontPath+"sniperturret_left_idle2.png",
                defaultFrontPath+"sniperturret_left_idle3.png",
                defaultFrontPath+"sniperturret_left_idle4.png",
            };
            MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
            */
            var Turret_Left = PrefabBuilder.BuildObject("Trespass Turret Front");
            Turret_Left.layer = Layers.FG_Critical;
            var sprite = Turret_Left.AddComponent<tk2dSprite>();
            sprite.SortingOrder = 2;
            var animator = Turret_Left.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName("trespassTurret_left");
            sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 8f);
            mat.SetFloat("_EmissivePower", 4);
            sprite.renderer.material = mat;
            sprite.IsPerpendicular = false;
            BreakableAPI_Bundled.GenerateTransformObject(Turret_Left.gameObject, new Vector2(0.5f, 0.875f), "laserPoint");
            AIBulletBank bulletBankLeft = Turret_Left.gameObject.AddComponent<AIBulletBank>();
            TrespassSniperTurretsController t = Turret_Left.gameObject.AddComponent<TrespassSniperTurretsController>();
            Turret_Left.gameObject.AddComponent<TresspassLightController>();
            t.DirectionToFire = Vector2.right.ToAngle();
            t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(576) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;

            bulletBankLeft.Bullets = new List<AIBulletBank.Entry>()
            {
                entry1,
                entry2,
                entry3
            };
            Turret_Left.gameObject.AddComponent<PushImmunity>();

            StaticReferences.StoredRoomObjects.Add("TrespassSniperTurretLeft", Turret_Left.gameObject);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("PSOG_TrespassSniperTurretLeft", Turret_Left.gameObject);

        }
        public static void MakeRight(AIBulletBank.Entry entry1, AIBulletBank.Entry entry2, AIBulletBank.Entry entry3)
        {
            /*
            string defaultFrontPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassSniper/Right/";
            string[] idlePaths = new string[]
            {
                defaultFrontPath+"sniperturret_right_idle1.png",
                defaultFrontPath+"sniperturret_right_idle2.png",
                defaultFrontPath+"sniperturret_right_idle3.png",
                defaultFrontPath+"sniperturret_right_idle4.png",

            };
            MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
            */

            var Turret_Right = PrefabBuilder.BuildObject("Trespass Turret Front");
            Turret_Right.layer = Layers.FG_Critical;
            var sprite = Turret_Right.AddComponent<tk2dSprite>();
            sprite.SortingOrder = 2;
            var animator = Turret_Right.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName("trespassTurret_right");
            sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 8f);
            mat.SetFloat("_EmissivePower", 4);
            sprite.renderer.material = mat;
            sprite.IsPerpendicular = false;
            BreakableAPI_Bundled.GenerateTransformObject(Turret_Right.gameObject, new Vector2(0.5f, 0.875f), "laserPoint");


            AIBulletBank bulletBankLeft = Turret_Right.gameObject.AddComponent<AIBulletBank>();
            TrespassSniperTurretsController t = Turret_Right.gameObject.AddComponent<TrespassSniperTurretsController>();
            Turret_Right.gameObject.AddComponent<TresspassLightController>();
            t.DirectionToFire = Vector2.left.ToAngle();
            t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(576) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
           
            bulletBankLeft.Bullets = new List<AIBulletBank.Entry>()
            {
                entry1,
                entry2,
                entry3
            };
            Turret_Right.gameObject.AddComponent<PushImmunity>();

            StaticReferences.StoredRoomObjects.Add("TrespassSniperTurretRight", Turret_Right.gameObject);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("PSOG_TrespassSniperTurretRight", Turret_Right.gameObject);

        }
    }
}

