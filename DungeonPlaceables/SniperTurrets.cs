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
using PathologicalGames;

namespace Planetside
{

    public class SniperTurretsController : MonoBehaviour
    {
        public void Start()
        {
            shootPosition = this.transform.Find("laserPoint").transform;
            CurrentState = State.PRIMED;
            bulletBank = this.GetComponent<AIBulletBank>();
            currentRoom = this.transform.position.GetAbsoluteRoom();
            if (laserPointer == null)
            {
                laserPointer = SpawnManager.SpawnVFX((GameObject)BraveResources.Load(isProfessional == false ? "Global VFX/VFX_LaserSight_Enemy" : "Global VFX/VFX_LaserSight_Enemy_Green", ".prefab"), true);
                laserPointer.transform.position = shootPosition.transform.position;
                laserPointer.transform.parent = shootPosition.gameObject.transform;
                laserPointerTiledSprite = laserPointer.GetComponent<tk2dTiledSprite>();
                laserPointerTiledSprite.HeightOffGround = 11;
                laserPointerTiledSprite.renderer.enabled = true;
                laserPointerTiledSprite.transform.localRotation = Quaternion.Euler(0f, 0f, ReturnDirection().ToAngle());
            }
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
                else
                {
                    DoRayCast();
                }
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
                        if (isProfessional == true)
                        {
                            SniperTurretsController[] turrets = UnityEngine.Object.FindObjectsOfType<SniperTurretsController>();
                            for (int i = 0; i < turrets.Count(); i++)
                            {
                                if (turrets[i].isProfessional && turrets[i].currentRoom == this.currentRoom)
                                {
                                    turrets[i].Shoot();
                                }
                            }
                        }
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
                AkSoundEngine.PostEvent("Play_WPN_sniperrifle_shot_01", this.gameObject);
                GameObject gameObject = SpawnManager.SpawnProjectile(this.bulletBank.Bullets[0].BulletObject, shootPosition.position, Quaternion.Euler(0f, 0f, ReturnDirection().ToAngle()), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                component.baseData.speed = 50;
                component.UpdateSpeed();
                component.Shooter = this.GetComponent<SpeculativeRigidbody>();
                component.IgnoreTileCollisionsFor(0.03f);
                component.pierceMinorBreakables = true;
                component.collidesWithEnemies = false;
                SpawnManager.PoolManager.Remove(component.gameObject.transform);
                component.BulletScriptSettings.preventPooling = true;


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
               


                elaWait = 0f;
                duraWait = 2f;

                if (muzzleFlashPrefab != null) { 
                    GameObject vfx = SpawnManager.SpawnVFX(muzzleFlashPrefab, true);
                    vfx.transform.position = shootPosition.transform.position;
                    vfx.transform.localRotation = Quaternion.Euler(0f, 0f, ReturnDirection().ToAngle());
                    vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;

                    Destroy(vfx, 2);
                }
                CurrentState = State.POST_FIRE;
            }    
            while (elaWait < duraWait)
            {
                if (this == null) { yield break; }
                if (laserPointerTiledSprite.gameObject == null) { yield break; }
                if (this.gameObject == null) { yield break; }
                if (elaWait > (duraWait-0.5f)) 
                { laserPointerTiledSprite.renderer.enabled = true; 
                }
                elaWait += BraveTime.DeltaTime;
                CurrentState = State.PRE_PRIMED;
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
        public bool isProfessional;

        public float DirectionToFire;
        public State CurrentState;
        public enum State
        {
            PRIMED,
            ABOUT_TO_FIRE,
            POST_FIRE,
            PRE_PRIMED,
        }
    }
    public class SniperTurrets
	{
        public static void Init()
        {
            string defaultFrontPath = "Planetside/Resources/DungeonObjects/SniperTurret/Default/";
            string[] idlePaths = new string[]
            {
                defaultFrontPath+"sniperturret_front_idle1.png",
                defaultFrontPath+"sniperturret_front_idle2.png",
                defaultFrontPath+"sniperturret_front_idle1.png",
                defaultFrontPath+"sniperturret_front_idle4.png",
            };

            AIBulletBank.Entry entrySniper = StaticUndodgeableBulletEntries.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");



            MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
			EnemyToolbox.GenerateShootPoint(sniperTurretDefaultaFront.gameObject, new Vector2(0.3125f, 0.5625f), "laserPoint");
            AIBulletBank bulletBankLeft = sniperTurretDefaultaFront.gameObject.AddComponent<AIBulletBank>();
            SniperTurretsController  t = sniperTurretDefaultaFront.gameObject.AddComponent<SniperTurretsController>();
            t.DirectionToFire = Vector2.down.ToAngle();
            t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;

            bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
			bulletBankLeft.Bullets.Add(entrySniper);
			StaticReferences.StoredRoomObjects.Add("sniperTurretFront", sniperTurretDefaultaFront.gameObject);

            string defaultProfessionalPath = "Planetside/Resources/DungeonObjects/SniperTurret/Professional/";
            string[] idleProfPaths = new string[]
            {
                defaultProfessionalPath+"professionalsniperturret_front_idle1.png",
                defaultProfessionalPath+"professionalsniperturret_front_idle2.png",
                defaultProfessionalPath+"professionalsniperturret_front_idle3.png",
                defaultProfessionalPath+"professionalsniperturret_front_idle4.png",
            };

            MajorBreakable professionalTurretFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idleProfPaths, 5, idleProfPaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
            EnemyToolbox.GenerateShootPoint(professionalTurretFront.gameObject, new Vector2(0.3125f, 0.5625f), "laserPoint");
            AIBulletBank bulletBank = professionalTurretFront.gameObject.AddComponent<AIBulletBank>();
            SniperTurretsController turret = professionalTurretFront.gameObject.AddComponent<SniperTurretsController>();
            turret.isProfessional = true;
            turret.DirectionToFire = Vector2.down.ToAngle();
            turret.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;

            bulletBank.Bullets = new List<AIBulletBank.Entry>();
            bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
            StaticReferences.StoredRoomObjects.Add("professionalTurretFront", professionalTurretFront.gameObject);


            MakeLeft(entrySniper);
            MakeRight(entrySniper);
        }

        public static void MakeLeft(AIBulletBank.Entry entry)
        {
            string defaultFrontPath = "Planetside/Resources/DungeonObjects/SniperTurret/Default/";
            string[] idlePaths = new string[]
            {
                defaultFrontPath+"sniperturret_left_idle1.png",
            };

            MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000,  true, 0, 0, 0, 0, true, null, null, true, null);
            EnemyToolbox.GenerateShootPoint(sniperTurretDefaultaFront.gameObject, new Vector2(0.5f, 0.6875f), "laserPoint");
            AIBulletBank bulletBankLeft = sniperTurretDefaultaFront.gameObject.AddComponent<AIBulletBank>();
            SniperTurretsController t = sniperTurretDefaultaFront.gameObject.AddComponent<SniperTurretsController>();
            t.DirectionToFire = Vector2.left.ToAngle();
            t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;

            bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
            bulletBankLeft.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
            StaticReferences.StoredRoomObjects.Add("sniperTurretLeft", sniperTurretDefaultaFront.gameObject);

            string defaultProfessionalPath = "Planetside/Resources/DungeonObjects/SniperTurret/Professional/";
            string[] idleProfPaths = new string[]
            {
                defaultProfessionalPath+"professionalsniperturret_left_idle1.png",
            };

            MajorBreakable professionalTurretFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idleProfPaths, 5, idleProfPaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
            EnemyToolbox.GenerateShootPoint(professionalTurretFront.gameObject, new Vector2(0.5f, 0.875f), "laserPoint");
            AIBulletBank bulletBank = professionalTurretFront.gameObject.AddComponent<AIBulletBank>();
            SniperTurretsController turret = professionalTurretFront.gameObject.AddComponent<SniperTurretsController>();
            turret.isProfessional = true;
            turret.DirectionToFire = Vector2.left.ToAngle();
            turret.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;

            bulletBank.Bullets = new List<AIBulletBank.Entry>();
            bulletBank.Bullets.Add(entry);
            StaticReferences.StoredRoomObjects.Add("professionalTurretLeft", professionalTurretFront.gameObject);
        }
        public static void MakeRight(AIBulletBank.Entry entry)
        {
            string defaultFrontPath = "Planetside/Resources/DungeonObjects/SniperTurret/Default/";
            string[] idlePaths = new string[]
            {
                defaultFrontPath+"sniperturret_right_idle1.png",
                defaultFrontPath+"sniperturret_right_idle2.png",
                defaultFrontPath+"sniperturret_right_idle3.png",
                defaultFrontPath+"sniperturret_right_idle4.png",
            };
            MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
            EnemyToolbox.GenerateShootPoint(sniperTurretDefaultaFront.gameObject, new Vector2(0.5f, 0.875f), "laserPoint");
            AIBulletBank bulletBankLeft = sniperTurretDefaultaFront.gameObject.AddComponent<AIBulletBank>();
            SniperTurretsController t = sniperTurretDefaultaFront.gameObject.AddComponent<SniperTurretsController>();
            t.DirectionToFire = Vector2.right.ToAngle();
            t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;

            bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
            bulletBankLeft.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"));
            StaticReferences.StoredRoomObjects.Add("sniperTurretRight", sniperTurretDefaultaFront.gameObject);

            string defaultProfessionalPath = "Planetside/Resources/DungeonObjects/SniperTurret/Professional/";
            string[] idleProfPaths = new string[]
            {
                defaultProfessionalPath+"professionalsniperturret_right_idle1.png",
                defaultProfessionalPath+"professionalsniperturret_right_idle2.png",
                defaultProfessionalPath+"professionalsniperturret_right_idle3.png",
                defaultProfessionalPath+"professionalsniperturret_right_idle4.png",

            };

            MajorBreakable professionalTurretFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idleProfPaths, 5, idleProfPaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
            EnemyToolbox.GenerateShootPoint(professionalTurretFront.gameObject, new Vector2(0.5f, 0.6875f), "laserPoint");
            AIBulletBank bulletBank = professionalTurretFront.gameObject.AddComponent<AIBulletBank>();
            SniperTurretsController turret = professionalTurretFront.gameObject.AddComponent<SniperTurretsController>();
            turret.isProfessional = true;
            turret.DirectionToFire = Vector2.right.ToAngle();
            turret.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;

            bulletBank.Bullets = new List<AIBulletBank.Entry>();
            bulletBank.Bullets.Add(entry);
            StaticReferences.StoredRoomObjects.Add("professionalTurretRight", professionalTurretFront.gameObject);
        }
    }
}

