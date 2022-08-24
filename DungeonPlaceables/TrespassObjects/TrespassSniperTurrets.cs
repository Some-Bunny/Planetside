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

        public IEnumerator FrameDelay()
        {
            yield return null;
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
                if (TrapDefusalKit.IsHoldingDefusalItem == true)
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
            Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable;
            CollisionLayer layer2 = CollisionLayer.PlayerHitBox;
            int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle ,CollisionLayer.BulletBlocker, layer2);
            RaycastResult raycastResult2;
            if (PhysicsEngine.Instance.Raycast(shootPosition.transform.PositionVector2(), ReturnDirection(), 100, out raycastResult2, true, true, rayMask2, null, false, rigidbodyExcluder, null))
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
                    Projectile component = gameObject.GetComponent<Projectile>();
                    component.baseData.speed = 30;
                    component.UpdateSpeed();
                    component.Shooter = this.GetComponent<SpeculativeRigidbody>();
                    component.IgnoreTileCollisionsFor(0.03f);
                    component.pierceMinorBreakables = true;
                    component.collidesWithEnemies = false;
                    CurrentState = State.POST_FIRE;
                    AkSoundEngine.PostEvent("Play_AbyssBlast", this.gameObject);

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
            string defaultFrontPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassSniper/Down/";
            string[] idlePaths = new string[]
            {
                defaultFrontPath+"sniperturret_front_idle1.png",
                defaultFrontPath+"sniperturret_front_idle2.png",
                defaultFrontPath+"sniperturret_front_idle1.png",
                defaultFrontPath+"sniperturret_front_idle4.png",
            };

            MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, null, 0f, -0.1875f, true, 0, 0, 0, 0, true, null, null, true, null);
			EnemyToolbox.GenerateShootPoint(sniperTurretDefaultaFront.gameObject, new Vector2(0.4375f, 0.875f), "laserPoint");
            AIBulletBank bulletBankLeft = sniperTurretDefaultaFront.gameObject.AddComponent<AIBulletBank>();
            TrespassSniperTurretsController t = sniperTurretDefaultaFront.gameObject.AddComponent<TrespassSniperTurretsController>();
            sniperTurretDefaultaFront.gameObject.AddComponent<TresspassLightController>();
            t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(576) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;

            t.DirectionToFire = Vector2.down.ToAngle();
            bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
			bulletBankLeft.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBigBullet);
            bulletBankLeft.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
            bulletBankLeft.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
            StaticReferences.StoredRoomObjects.Add("TrespassSniperTurretFront", sniperTurretDefaultaFront.gameObject);

         


            MakeLeft();
            MakeRight();
        }

        public static void MakeLeft()
        {
            string defaultFrontPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassSniper/Left/";
            string[] idlePaths = new string[]
            {
                defaultFrontPath+"sniperturret_left_idle1.png",
                defaultFrontPath+"sniperturret_left_idle2.png",
                defaultFrontPath+"sniperturret_left_idle3.png",
                defaultFrontPath+"sniperturret_left_idle4.png",
            };
            MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, null, 0f, -0.1875f, true, 0, 0, 0, 0, true, null, null, true, null);
            EnemyToolbox.GenerateShootPoint(sniperTurretDefaultaFront.gameObject, new Vector2(0.5f, 0.875f), "laserPoint");
            AIBulletBank bulletBankLeft = sniperTurretDefaultaFront.gameObject.AddComponent<AIBulletBank>();
            TrespassSniperTurretsController t = sniperTurretDefaultaFront.gameObject.AddComponent<TrespassSniperTurretsController>();
            sniperTurretDefaultaFront.gameObject.AddComponent<TresspassLightController>();
            t.DirectionToFire = Vector2.right.ToAngle();
            t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(576) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;

            bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
            bulletBankLeft.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBigBullet);
            bulletBankLeft.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
            bulletBankLeft.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
            StaticReferences.StoredRoomObjects.Add("TrespassSniperTurretLeft", sniperTurretDefaultaFront.gameObject);
        }
        public static void MakeRight()
        {
            string defaultFrontPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassSniper/Right/";
            string[] idlePaths = new string[]
            {
                defaultFrontPath+"sniperturret_right_idle1.png",
                defaultFrontPath+"sniperturret_right_idle2.png",
                defaultFrontPath+"sniperturret_right_idle3.png",
                defaultFrontPath+"sniperturret_right_idle4.png",

            };
            MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, null, 0f, -0.1875f, true, 0, 0, 0, 0, true, null, null, true, null);
            EnemyToolbox.GenerateShootPoint(sniperTurretDefaultaFront.gameObject, new Vector2(0.5f, 0.875f), "laserPoint");
            AIBulletBank bulletBankLeft = sniperTurretDefaultaFront.gameObject.AddComponent<AIBulletBank>();
            TrespassSniperTurretsController t = sniperTurretDefaultaFront.gameObject.AddComponent<TrespassSniperTurretsController>();
            sniperTurretDefaultaFront.gameObject.AddComponent<TresspassLightController>();
            t.DirectionToFire = Vector2.left.ToAngle();
            t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(576) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;

            bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
            bulletBankLeft.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableBigBullet);
            bulletBankLeft.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableDefault);
            bulletBankLeft.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
            StaticReferences.StoredRoomObjects.Add("TrespassSniperTurretRight", sniperTurretDefaultaFront.gameObject);
        }
    }
}

