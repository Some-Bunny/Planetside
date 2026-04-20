using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;


using UnityEngine.Serialization;
using UnityEngine.UI;
using Alexandria.Assetbundle;
using Planetside.Toolboxes;
using static Planetside.UpturnedGun;
using Alexandria.Misc;
using System.ComponentModel;
using Brave.BulletScript;
using static UnityEngine.UI.GridLayoutGroup;
using HutongGames.PlayMaker.Actions;
using UnityEngine.Playables;
using Alexandria;
using static AIActor;
using static Planetside.Bloat;

namespace Planetside
{
	public class UpturnedGun : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Gun Of Power", "sillygun");
			Game.Items.Rename("outdated_gun_mods:gun_of_power", "psog:gun_of_power");
			gun.gameObject.AddComponent<UpturnedGun>();
			GunExt.SetShortDescription(gun, "The Ultimate Gun");
			GunExt.SetLongDescription(gun, "Materializes objects from thin air whe fired! \n\nAn oversized laser tag gun stolen from an old inn somewhere in the afterlife.");

            //upturned_detonante

            GunInt.SetupSpritePrebaked(gun, StaticSpriteDefinitions.Gun_2_Sheet_Data, "upturnedgun_idle_001");
            gun.spriteAnimator.Library = StaticSpriteDefinitions.Gun_2_Animation_Data;
            gun.sprite.SortingOrder = 2;

            gun.idleAnimation = "upturned_idle";
            gun.shootAnimation = "upturned_idle";
            gun.reloadAnimation = "upturned_idle";


            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(62) as Gun, true, false);


            gun.SetBaseMaxAmmo(10);
			gun.ammo = 10;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(13) as Gun).gunSwitchGroup;
            gun.InfiniteAmmo = true;


            var entry = gun.Volley.projectiles[0];
            entry.ammoType = Guns.BSG.DefaultModule.ammoType;
            entry.customAmmoType = Guns.BSG.DefaultModule.customAmmoType;

            entry.ammoCost = 1;
            entry.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;

            entry.cooldownTime = 0.75f;
            entry.angleVariance = 12f;

            entry.numberOfShotsInClip = 100;

            entry.angleVariance = 8f;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(Guns.Marine_Sidearm.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 0;
            projectile.baseData.speed = 0;
            projectile.baseData.range = 0.1f;
            projectile.baseData.force = 0;
            projectile.hitEffects = new ProjectileImpactVFXPool() { alwaysUseMidair = true, overrideMidairDeathVFX = null };
            entry.projectiles[0] = projectile;





            gun.reloadTime = 0;

			gun.barrelOffset.transform.localPosition = new Vector3(3.125f, 1f, 0f);
            gun.carryPixelOffset = new IntVector2(6, 0);

            gun.quality = PickupObject.ItemQuality.EXCLUDED;
			gun.CanBeDropped = false;
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(387) as Gun).muzzleFlashEffects;
			gun.gunClass = GunClass.SILLY;

			ETGMod.Databases.Items.Add(gun, false, "ANY");
            ItemID = gun.PickupObjectId;
			ItemIDs.AddToList(gun.PickupObjectId);
            Alexandria.DungeonAPI.StaticReferences.storedItemIDs.Add("PSOG:SuperGun", ItemID);
        }
        public static int ItemID;



        public override void Start()
        {
            base.Start();
            gun.OnPreFireProjectileModifier += DetermineSlashType;

            ShotsLeft = UnityEngine.Random.Range(12, 50);
            this.gun.DefaultModule.cooldownTime = UnityEngine.Random.Range(0.65f, 1.2f);

            float t = UnityEngine.Random.value;

            this.gun.DefaultModule.shootStyle = t < 0.3 ? ProjectileModule.ShootStyle.Burst :  BraveUtility.RandomBool() ? ProjectileModule.ShootStyle.SemiAutomatic : ProjectileModule.ShootStyle.Automatic;

            gun.SetBaseMaxAmmo(1000);
            gun.ammo = 1000;
            gun.DefaultModule.angleVariance = UnityEngine.Random.Range(3, 24);

            this.gun.DefaultModule.numberOfShotsInClip = ShotsLeft;     
            
            if (t < 0.3)
            {
                this.gun.DefaultModule.cooldownTime *= 0.05f;
                this.gun.DefaultModule.burstShotCount = 1000;
                this.gun.DefaultModule.burstCooldownTime = UnityEngine.Random.Range(0.01f, 0.04f);
            }
            Chaosness = UnityEngine.Random.value < 0.1f ? UnityEngine.Random.Range(0.333f, 0.9f) : UnityEngine.Random.Range(0.02f, 0.2f);
            //projectileType = UnityEngine.Random.Range(ProjectileType[0], ProjectileType)
            RollShootType();

        }
        public int _ProjectileType;
        public int ShotsLeft;
        public float Chaosness = 0.03f;

        /*
* 0: Tables
* 1: trash cannon shots
* 2: molotov launcher shots
* 3: kiln shots
* 4: beehive shots
* 5: bloody 9mm
* 6: ice cube proj
* 7: random rockets
* 8: sawblade gun
* 9: poison vial
* 10: bombs
11: cluster mines
12: bee jar
13: roll bombs
14: rat sack projectiles
        15: cigarettes with chance for blackhole
        16: pot
        17: ember pot
        18: toilet
        19: casings
        20: portable turrets
        21: chaff grenades
*/


        
        public override void PostProcessProjectile(Projectile projectile)
        {
            bool isEnemy = false;
            GameObject ToInstantiate = null;
            AIActor ToInstantiateEnemy = null;
            switch (_ProjectileType)
            {
                case 0:
                    
                    switch (UnityEngine.Random.Range(0, 3))
                    {
                        case 0:
                        ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.SteelTableHorizontal;
                            break;
                        case 1:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.SteelTableVertical;
                            break;
                        case 2:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.FoldingTable;
                            break;
                    }                      
                    break;
                case 9:
                    ToInstantiate = Actives.Poison_Vial.objectToSpawn;
                    break;
                case 10:
                    ToInstantiate = UnityEngine.Random.value < 0.12f ? Actives.Ice_Bomb.objectToSpawn: Actives.Bomb.objectToSpawn;
                    break;
                case 11:
                    ToInstantiate = Actives.Cluster_Mine.objectToSpawn;
                    break;
                case 12:
                    ToInstantiate = UnityEngine.Random.value < 0.05f ? Actives.Jar_Of_Bees.SynergyObjectToSpawn: Actives.Jar_Of_Bees.objectToSpawn;
                    break;
                case 15:
                    ToInstantiate = UnityEngine.Random.value < 0.05f ? Actives.Singularity.objectToSpawn : Actives.Cigarettes.objectToSpawn;
                    break;
                case 16:
                    ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.TanPot;
                    break;
                case 17:
                    ToInstantiate = Alexandria.DungeonAPI.StaticReferences.customPlaceables["psog:emberPot"].variantTiers[0].nonDatabasePlaceable;
                    break;
                case 18:
                    switch (UnityEngine.Random.Range(0, 3))
                    {
                        case 0:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.ToiletEast;
                            break;
                        case 1:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.ToiletNorth;
                            break;
                        case 2:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.ToiletWest;
                            break;
                    }
                    break;
                case 19:
                    if (UnityEngine.Random.value < 0.01)
                    {
                        ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.FiftyCasingPlaceable;
                        break;
                    }
                    if (UnityEngine.Random.value < 0.15)
                    {
                        ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.FiveCasingPlaceable;
                        break;
                    }
                    ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.SingleCasingPlaceable;
                    break;
                 case 20:
                    ToInstantiate = EnemyDatabase.GetOrLoadByGuid(Actives.Portable_Turret.enemyGuidToSpawn).gameObject;

                    break;
                case 21:
                    isEnemy = true;
                    ToInstantiateEnemy = EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Bullet_Kin_GUID);
                    break;
                case 22:
                    isEnemy = true;
                    ToInstantiateEnemy = UnityEngine.Random.value < 0.15 ? EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Nitra_GUID) : EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Pinhead_GUID);
                    //ToInstantiate = Actives.Chaff_Grenade.objectToSpawn;
                    break;
                case 23:
                    isEnemy = true;
                    ToInstantiateEnemy = EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Pot_Fairy_GUID);
                    break;
                case 24:
                    isEnemy = true;
                    ToInstantiateEnemy = EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Mine_Flayers_Claymore_GUID);
                    break;
                case 25:
                    isEnemy = true;
                    ToInstantiateEnemy = EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Poisbulon_GUID);
                    break;
                case 26:
                    isEnemy = true;
                    ToInstantiateEnemy = EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.Blobulon_Kin_GUID);
                    break;
                case 27:
                    switch (UnityEngine.Random.Range(0, 3))
                    {
                        case 0:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.CardboardBox1;
                            break;
                        case 1:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.CardboardBox2;
                            break;
                        case 2:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.CardboardBox3;
                            break;
                    }
                    break;
                case 28:
                    switch (UnityEngine.Random.Range(0, 3))
                    {
                        case 0:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.FullHeart;
                            break;
                        case 1:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.GlassGuonPlaceable;
                            break;
                        case 2:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.HalfHeart;
                            break;
                    }
                    break;
                case 29:
                    switch (UnityEngine.Random.Range(0, 2))
                    {
                        case 0:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.NonRatStealableAmmo;
                            break;
                        case 1:
                            ToInstantiate = Alexandria.DungeonAPI.SetupExoticObjects.NonRatStealableSpreadAmmo;
                            break;
                    }
                    break;
                case 30:
                    ToInstantiate = PickupObjectDatabase.GetById(LeSackPickup.SaccID).gameObject;
                    break;
                case 31:
                    ToInstantiate = PickupObjectDatabase.GetById(NullPickupInteractable.NollahID).gameObject;
                    break;
                case 32:
                    ToInstantiate = PickupObjectDatabase.GetById(AllSeeingEyeMiniPickup.MiniEye).gameObject;
                    break;
                case 33:
                    ToInstantiate = GungeonAPI.StaticReferences.StoredRoomObjects["hmprimeBattery"];
                    break;
                case 34:
                    ToInstantiate = Alexandria.DungeonAPI.StaticReferences.customPlaceables["PSOG_tresPassRandomCubesSmall"].variantTiers[0].nonDatabasePlaceable;

                    break;
                case 35:
                    ToInstantiate = Alexandria.DungeonAPI.StaticReferences.customObjects["PSOG_tresPassPots"];
                    break;
            }
            if (ToInstantiate != null | ToInstantiateEnemy != null)
            {
                var a_ = isEnemy ? AIActor.Spawn(ToInstantiateEnemy, projectile.transform.position, this.transform.position.GetAbsoluteRoom(), false, AIActor.AwakenAnimationType.Default).gameObject : Instantiate(ToInstantiate, projectile.transform.position, Quaternion.identity);
                ApplyPostProcess(a_, MathToolbox.GetUnitOnCircle(gun.CurrentAngle + UnityEngine.Random.Range(-gun.DefaultModule.angleVariance, gun.DefaultModule.angleVariance + 1), 1));
                projectile.DieInAir(true);
            }
            if (_ProjectileType == 31 | _ProjectileType == 32)
            {
                RollShootType();
            }
        }


        private bool LockUp = false;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            player.knockbackDoer.ApplyKnockback(MathToolbox.GetUnitOnCircle(gun.CurrentAngle + 180,1), 100, false);

            ShotsLeft--;
            AkSoundEngine.PostEvent("Play_FunnyGunSHoot", this.gameObject);
            AkSoundEngine.PostEvent("Play_FunnyGunCharge", this.gameObject);
            if (UnityEngine.Random.value < Chaosness)
            {
                Chaosness *= 0.5f;
                RollShootType();
            }
            if (ShotsLeft <= 0)
            {
                if (!LockUp)
                    LockUpGun();
            }
            base.OnPostFired(player, gun);
        }

        public void RollShootType()
        {
            _ProjectileType = UnityEngine.Random.Range(0, 36);
            Debug.Log(_ProjectileType);
        }

        public void LockUpGun()
        {
            LockUp = true;
            this.gun.DefaultModule.cooldownTime = 1000;
            (gun.CurrentOwner as PlayerController).inventory.RemoveGunFromInventory(gun);
            CustomGunThrow();
        }



        public override void OnPickup(PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_FunnyGunCharge", this.gameObject);
            base.OnPickup(player);       
        }
        public override void OnPostDrop(PlayerController player)
        {
            base.OnPostDrop(player);
        }

        private void CustomGunThrow()
        {
            /*
            gun.m_isThrown = true;
            gun.m_thrownOnGround = false;
            base.gameObject.SetActive(true);
            AkSoundEngine.PostEvent("Play_OBJ_item_throw_01", base.gameObject);
            Vector3 vector = gun.ThrowPrepTransform.parent.TransformPoint((gun.ThrowPrepPosition * -1f).WithX(0f));
            Vector2 vector2 = gun.m_localAimPoint - vector.XY();
            float z = BraveMathCollege.Atan2Degrees(vector2);
            GameObject gameObject = SpawnManager.SpawnProjectile("ThrownGunProjectile", vector, Quaternion.Euler(0f, 0f, z));
            Projectile component = gameObject.GetComponent<Projectile>();
            component.Shooter = gun.m_owner.specRigidbody;
            component.DestroyMode = Projectile.ProjectileDestroyMode.BecomeDebris;
            component.baseData.damage *= (gun.m_owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ThrownGunDamage);
            SpeculativeRigidbody component2 = gameObject.GetComponent<SpeculativeRigidbody>();
            SpeculativeRigidbody speculativeRigidbody = component2;
            speculativeRigidbody.OnTriggerCollision = (SpeculativeRigidbody.OnTriggerDelegate)Delegate.Combine(speculativeRigidbody.OnTriggerCollision, new SpeculativeRigidbody.OnTriggerDelegate(gun.OnTrigger));
            component2.sprite = gun.sprite;
            gun.m_sprite.scale = Vector3.one;
            base.transform.parent = gameObject.transform;
            base.transform.localRotation = Quaternion.identity;
            gun.m_sprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.MiddleCenter);
            if (gun.m_sprite.FlipY)
            {
                base.transform.localPosition = Vector3.Scale(new Vector3(-1f, 1f, 1f), base.transform.localPosition);
            }
            Bounds bounds = gun.sprite.GetBounds();
            component2.PrimaryPixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            component2.PrimaryPixelCollider.ManualOffsetX = -Mathf.RoundToInt(bounds.extents.x / 0.0625f);
            component2.PrimaryPixelCollider.ManualOffsetY = -Mathf.RoundToInt(bounds.extents.y / 0.0625f);
            component2.PrimaryPixelCollider.ManualWidth = Mathf.RoundToInt(bounds.size.x / 0.0625f);
            component2.PrimaryPixelCollider.ManualHeight = Mathf.RoundToInt(bounds.size.y / 0.0625f);
            component2.UpdateCollidersOnRotation = true;
            component2.UpdateCollidersOnScale = true;
            component.Reawaken();
            component.Owner = gun.CurrentOwner;
            component.Start();
            component.SendInDirection(vector2, true, false);

            Projectile projectile = component;
            projectile.OnBecameDebris = (Action<DebrisObject>)Delegate.Combine(projectile.OnBecameDebris, new Action<DebrisObject>(delegate (DebrisObject a)
            {
                if (gun.barrelOffset)
                {
                    gun.barrelOffset.localPosition = gun.m_originalBarrelOffsetPosition;
                }
                if (gun.muzzleOffset)
                {
                    gun.muzzleOffset.localPosition = gun.m_originalMuzzleOffsetPosition;
                }
                if (gun.chargeOffset)
                {
                    gun.chargeOffset.localPosition = gun.m_originalChargeOffsetPosition;
                }
                if (a)
                {
                    a.Priority = EphemeralObject.EphemeralPriority.Critical;
                    TrailRenderer componentInChildren = a.gameObject.GetComponentInChildren<TrailRenderer>();
                    if (componentInChildren)
                    {
                        UnityEngine.Object.Destroy(componentInChildren);
                    }
                    SpeculativeRigidbody component3 = a.GetComponent<SpeculativeRigidbody>();
                    if (component3)
                    {
                        component3.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.Projectile, CollisionLayer.EnemyHitBox));
                    }
                }
                Destroy(gun.gameObject);
                AkSoundEngine.PostEvent("Play_FunnyGunExplode", gun.gameObject);
            }));
            Projectile projectile2 = component;
            projectile2.OnBecameDebrisGrounded = (Action<DebrisObject>)Delegate.Combine(projectile2.OnBecameDebrisGrounded, new Action<DebrisObject>(gun.HandleThrownGunGrounded));
            component.angularVelocity = (float)((vector2.x <= 0f) ? 1080 : -1080);

            component2.ForceRegenerate(null, null);
            gun.m_owner = null;
            */
            gun.m_isThrown = true;
            gun.m_thrownOnGround = false;
            gun.gameObject.SetActive(true);
            AkSoundEngine.PostEvent("Play_OBJ_item_throw_01", gun.gameObject);
            Vector3 vector = gun.ThrowPrepTransform.parent.TransformPoint((gun.ThrowPrepPosition * -1f).WithX(0f));
            Vector2 vector2 = gun.m_localAimPoint - vector.XY();
            float z = BraveMathCollege.Atan2Degrees(vector2);
            GameObject gameObject = SpawnManager.SpawnProjectile("ThrownGunProjectile", vector, Quaternion.Euler(0f, 0f, z));
            Projectile component = gameObject.GetComponent<Projectile>();
            component.Shooter = gun.m_owner.specRigidbody;
            component.DestroyMode = Projectile.ProjectileDestroyMode.BecomeDebris;
            component.baseData.damage *= (gun.m_owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ThrownGunDamage);
            SpeculativeRigidbody component2 = gameObject.GetComponent<SpeculativeRigidbody>();
            SpeculativeRigidbody speculativeRigidbody = component2;
            speculativeRigidbody.OnTriggerCollision = (SpeculativeRigidbody.OnTriggerDelegate)Delegate.Combine(speculativeRigidbody.OnTriggerCollision, new SpeculativeRigidbody.OnTriggerDelegate(gun.OnTrigger));
            component2.sprite = gun.sprite;
            gun.m_sprite.scale = Vector3.one;
            gun.transform.parent = gameObject.transform;
            gun.transform.localRotation = Quaternion.identity;
            gun.m_sprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.MiddleCenter);
            if (gun.m_sprite.FlipY)
            {
                gun.transform.localPosition = Vector3.Scale(new Vector3(-1f, 1f, 1f), gun.transform.localPosition);
            }
            Bounds bounds = gun.sprite.GetBounds();
            component2.PrimaryPixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            component2.PrimaryPixelCollider.ManualOffsetX = -4;
            component2.PrimaryPixelCollider.ManualOffsetY = -4;
            component2.PrimaryPixelCollider.ManualWidth =8;
            component2.PrimaryPixelCollider.ManualHeight = 8;
            component2.UpdateCollidersOnRotation = true;
            component2.UpdateCollidersOnScale = true;
            component.Reawaken();
            component.Owner = gun.CurrentOwner;
            component.Start();
            component.SendInDirection(vector2, true, false);
            Projectile projectile = component;
            projectile.OnBecameDebris = (Action<DebrisObject>)Delegate.Combine(projectile.OnBecameDebris, new Action<DebrisObject>(delegate (DebrisObject a)
            {
                if (gun.barrelOffset)
                {
                    gun.barrelOffset.localPosition = gun.m_originalBarrelOffsetPosition;
                }
                if (gun.muzzleOffset)
                {
                    gun.muzzleOffset.localPosition = gun.m_originalMuzzleOffsetPosition;
                }
                if (gun.chargeOffset)
                {
                    gun.chargeOffset.localPosition = gun.m_originalChargeOffsetPosition;
                }
                if (a)
                {
                    a.Priority = EphemeralObject.EphemeralPriority.Critical;
                    TrailRenderer componentInChildren = a.gameObject.GetComponentInChildren<TrailRenderer>();
                    if (componentInChildren)
                    {
                        UnityEngine.Object.Destroy(componentInChildren);
                    }
                    SpeculativeRigidbody component3 = a.GetComponent<SpeculativeRigidbody>();
                    if (component3)
                    {
                        component3.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.Projectile, CollisionLayer.EnemyHitBox));
                    }
                }
            }));
            Projectile projectile2 = component;
            projectile2.OnBecameDebrisGrounded = (Action<DebrisObject>)Delegate.Combine(projectile2.OnBecameDebrisGrounded, new Action<DebrisObject>(gun.HandleThrownGunGrounded));
            component.angularVelocity = (float)((vector2.x <= 0f) ? 1080 : -1080);
            if (!RoomHandler.unassignedInteractableObjects.Contains(gun))
            {
                RoomHandler.unassignedInteractableObjects.Add(gun);
            }
            component2.ForceRegenerate(null, null);
            if (gun.m_owner)
            {
                (gun.m_owner as PlayerController).DoPostProcessThrownGun(component);
            }
            gun.m_owner = null;
            gun.PlayIfExists("upturned_detonante", true);
            GameManager.Instance.StartCoroutine(DestroyGun(component, gun.gameObject));
        }
        public IEnumerator DestroyGun(Projectile component, GameObject parent)
        {
            AkSoundEngine.PostEvent("Play_FunnyGunDetonate", parent.gameObject);
            yield return new WaitForSeconds(2f);
            Destroy(parent.transform.parent.gameObject);
            AkSoundEngine.PostEvent("Play_FunnyGunExplode", parent.gameObject);
            Exploder.Explode(parent.transform.position, StaticExplosionDatas.genericLargeExplosion, Vector2.zero);
            yield break;
        }



        
        public Projectile TransformObjectToProjectile(GameObject gameObject, Vector2 Dir)
        {
            var c = gameObject.GetComponent<SpeculativeRigidbody>();
            if (c != null)
            {
                c.CollideWithOthers = false;
            }
            var coin = gameObject.GetComponent<CurrencyPickup>();
            Projectile projectile = gameObject.AddComponent<Projectile>();


            projectile.Owner = gun.CurrentOwner;
            projectile.SetNewShooter(gun.CurrentOwner.specRigidbody);
            projectile.baseData.damage = 40;
            projectile.baseData.range = 1000f;
            projectile.baseData.speed = 30f;
            projectile.baseData.force = 50f;
            projectile.SendInDirection(Dir, true);
            projectile.specRigidbody = c;
            projectile.Start();
            projectile.Reawaken();

            projectile.baseData.UsesCustomAccelerationCurve = true;
            projectile.baseData.AccelerationCurve = AnimationCurve.EaseInOut(0, 1f, 1f, 0.6f);
            projectile.baseData.CustomAccelerationCurveDuration = 1;
            projectile.shouldRotate = false;

            projectile.pierceMinorBreakables = true;

            projectile.baseData.force = 50f;
            projectile.specRigidbody.CollideWithTileMap = true;
            projectile.specRigidbody.Reinitialize();


            projectile.collidesWithEnemies = true;
            projectile.specRigidbody.CollideWithOthers = true;

            PixelCollider p = new PixelCollider();
            p = projectile.specRigidbody.PrimaryPixelCollider;
            p.CollisionLayer = CollisionLayer.Projectile;
            p.ManualLeftX = 8;
            p.ManualLeftY = 8;

            projectile.specRigidbody.PixelColliders = new List<PixelCollider>();
            projectile.specRigidbody.PixelColliders.Add(p);

            projectile.specRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.Projectile;
            projectile.UpdateCollisionMask();

            projectile.transform.parent = null;



            projectile.DestroyMode = Projectile.ProjectileDestroyMode.DestroyComponent;
            var c1 = gameObject.GetComponent<MinorBreakable>();
            if (c1 != null)
            {
                c1.enabled = false;
            }
            projectile.OnDestruction += (_) =>
            {
                if (c1)
                {
                    c1.enabled = true;
                    c1.Break();
                    c1.OnBreakAnimationComplete();
                }
            };

            return projectile;
        }
        
        public void ApplyPostProcess(GameObject objec, Vector2 Dire)
        {
            switch (_ProjectileType)
            {
                case 0:
                    var _ = objec.GetComponent<FlippableCover>();
                    _.Start();
                    _.transform.position.XY().GetAbsoluteRoom().RegisterInteractable(_);
                    _.ConfigureOnPlacement(_.transform.position.XY().GetAbsoluteRoom());
                    GameManager.Instance.StartCoroutine(DoPushOfMajor(_.specRigidbody, Dire, 50, 
                        UnityEngine.Random.Range(2.1f, 3.2f), 
                        UnityEngine.Random.Range(25, 41)));
                    break;
                case 9:
                    DoDebrisToss(objec, Dire, 
                        UnityEngine.Random.Range(2.3f, 11.1f), 
                        UnityEngine.Random.value < 0.1f ? UnityEngine.Random.Range(3, 12) : UnityEngine.Random.Range(0, 1));
                    break;
                case 10:
                    DoDebrisToss(objec, Dire,
                        UnityEngine.Random.Range(1.4f, 4.1f),
                        UnityEngine.Random.value < 0.1f ? UnityEngine.Random.Range(3, 7) : UnityEngine.Random.Range(0, 1));
                    break;
                case 11:
                    DoDebrisToss(objec, Dire,
                        UnityEngine.Random.Range(2.4f, 7.1f),
                        UnityEngine.Random.Range(6, 12));
                    break;
                case 12:
                    DoDebrisToss(objec, Dire,
                        UnityEngine.Random.Range(6.4f, 12.1f),
                        UnityEngine.Random.Range(0, 12));
                    break;
                case 16:
                    var pot = TransformObjectToProjectile(objec, Dire);
                    pot.baseData.damage = 30;
                    pot.baseData.speed = UnityEngine.Random.Range(24, 49);
                    pot.SendInDirection(MathToolbox.GetUnitOnCircle(gun.CurrentAngle + UnityEngine.Random.Range(-gun.DefaultModule.angleVariance, gun.DefaultModule.angleVariance + 1), 1), true);
                    pot.UpdateSpeed();
                    break;
                case 17:
                    var ember = TransformObjectToProjectile(objec, Dire);
                    ember.baseData.damage = 12;
                    ember.baseData.speed = UnityEngine.Random.Range(24, 23);
                    ember.SendInDirection(MathToolbox.GetUnitOnCircle(gun.CurrentAngle + UnityEngine.Random.Range(-gun.DefaultModule.angleVariance, gun.DefaultModule.angleVariance + 1), 1), true);
                    ember.UpdateSpeed();
                    break;
                case 18:

                    var idk = TransformObjectToProjectile(objec, Dire);
                    idk.baseData.damage = 60;
                    idk.baseData.speed = UnityEngine.Random.Range(26, 53);
                    idk.SendInDirection(MathToolbox.GetUnitOnCircle(gun.CurrentAngle + UnityEngine.Random.Range(-gun.DefaultModule.angleVariance, gun.DefaultModule.angleVariance + 1), 1), true);
                    idk.UpdateSpeed();
                    break;
                case 19:

                    var d = TransformObjectToProjectile(objec, Dire);
                    d.SendInDirection(MathToolbox.GetUnitOnCircle(gun.CurrentAngle + UnityEngine.Random.Range(-gun.DefaultModule.angleVariance, gun.DefaultModule.angleVariance + 1), 1), true);
                    d.baseData.damage = 50;
                    break;
                case 15:
                    DoDebrisToss(objec, Dire,
                        UnityEngine.Random.Range(5.3f, 11.1f),
                        UnityEngine.Random.value < 0.1f ? UnityEngine.Random.Range(3, 12) : UnityEngine.Random.Range(0, 1));
                    break;

                case 20:
                    var knock1 = objec.GetOrAddComponent<KnockbackDoer>();
                    if (knock1)
                    {
                        knock1.ApplyKnockback(Dire, UnityEngine.Random.Range(5, 12), true);
                    }
                    break;
                case 21:
                    PushEnemy(objec.GetComponent<AIActor>(), Dire, UnityEngine.Random.Range(25, 64));
                    break;
                case 22:
                    PushEnemy(objec.GetComponent<AIActor>(), Dire, UnityEngine.Random.Range(45, 80));
                    break;
                case 23:
                    PushEnemy(objec.GetComponent<AIActor>(), Dire, UnityEngine.Random.Range(55, 100));
                    break;
                case 24:
                    PushEnemy(objec.GetComponent<AIActor>(), Dire, UnityEngine.Random.Range(70, 90));
                    break;
                case 25:
                    PushEnemy(objec.GetComponent<AIActor>(), Dire, UnityEngine.Random.Range(55, 90));
                    break;
                case 26:
                    PushEnemy(objec.GetComponent<AIActor>(), Dire, UnityEngine.Random.Range(55, 90));
                    break;
                case 27:
                    var box = TransformObjectToProjectile(objec, Dire);
                    box.baseData.damage = 20;
                    box.baseData.speed = UnityEngine.Random.Range(48, 72);
                    box.SendInDirection(MathToolbox.GetUnitOnCircle(gun.CurrentAngle + UnityEngine.Random.Range(-gun.DefaultModule.angleVariance, gun.DefaultModule.angleVariance + 1), 1), true);
                    box.UpdateSpeed();
                    break;
                case 28:
                    DoDebrisToss(objec, Dire,
                        UnityEngine.Random.Range(3.4f, 32.1f),
                        UnityEngine.Random.Range(0, 12));
                    break;
                case 29:
                    DoDebrisToss(objec, Dire,
                        UnityEngine.Random.Range(3.4f, 32.1f),
                        UnityEngine.Random.Range(0, 12));
                    break;
                case 30:
                    DoDebrisToss(objec, Dire,
                        UnityEngine.Random.Range(3.4f, 32.1f),
                        UnityEngine.Random.Range(0, 12));
                    break;
                case 31:
                    DoDebrisToss(objec, Dire,
                        UnityEngine.Random.Range(18.4f, 32.1f),
                        UnityEngine.Random.Range(0, 12));
                    break;
                case 32:
                    DoDebrisToss(objec, Dire,
                        UnityEngine.Random.Range(18.4f, 32.1f),
                        UnityEngine.Random.Range(0, 12));
                    break;
                case 33:
                    var a = TransformObjectToProjectile(objec, Dire);
                    a.baseData.damage = 100;
                    a.baseData.speed = UnityEngine.Random.Range(30, 40);
                    a.SendInDirection(MathToolbox.GetUnitOnCircle(gun.CurrentAngle + UnityEngine.Random.Range(-gun.DefaultModule.angleVariance, gun.DefaultModule.angleVariance + 1), 1), true);
                    a.UpdateSpeed();
                    break;
                case 34:
                    var aa = TransformObjectToProjectile(objec, Dire);
                    aa.baseData.damage = 5;
                    aa.baseData.speed = UnityEngine.Random.Range(32, 62);
                    aa.SendInDirection(MathToolbox.GetUnitOnCircle(gun.CurrentAngle + UnityEngine.Random.Range(-gun.DefaultModule.angleVariance, gun.DefaultModule.angleVariance + 1), 1), true);
                    aa.UpdateSpeed();
                    break;
                case 35:
                    var aaa = TransformObjectToProjectile(objec, Dire);
                    aaa.baseData.damage = 20;
                    aaa.baseData.speed = UnityEngine.Random.Range(22, 32);
                    aaa.SendInDirection(MathToolbox.GetUnitOnCircle(gun.CurrentAngle + UnityEngine.Random.Range(-gun.DefaultModule.angleVariance, gun.DefaultModule.angleVariance + 1), 1), true);
                    aaa.UpdateSpeed();
                    break;
            }
        }
        public void PushEnemy(AIActor aIActor, Vector2 Dire, float Power)
        {
            var knock2 = aIActor.knockbackDoer;
            if (knock2)
            {
                knock2.ApplyKnockback(Dire, Power, true);
            }
            aIActor.behaviorSpeculator.Stun(Power * 0.025f);
            aIActor.AwakenAnimType = AIActor.AwakenAnimationType.Default;
            aIActor.reinforceType = ReinforceType.Instant;
        }

        public IEnumerator DoPushOfMajor(SpeculativeRigidbody self, Vector2 Direction,float DamageToDealOnHit,  float Time, float Power)
        {
            float e = 0;
            self.Start();
            yield return null;
            bool Stop = false;

            SpeculativeRigidbody.OnPreTileCollisionDelegate del = null;
            SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate del2 = null;
            del = (a, aa, aaa, aaaa) =>
            {
                self.OnPreTileCollision -= del;
                self.OnPreRigidbodyCollision -= del2;
                self.Velocity = Vector2.zero;
                Stop = true;
            };
            self.OnPreTileCollision += del;

            del2 = (a, aa, aaa, aaaa) =>
            {
                self.OnPreTileCollision -= del;
                self.OnPreRigidbodyCollision -= del2;

                if (aaa.healthHaver != null)
                {
                    if (aaa.gameActor == null | aaa.gameActor is AIActor)
                    {
                        aaa.healthHaver.ApplyDamage(DamageToDealOnHit, Vector2.zero, "Bonk");
                    }
                }
                if (aaa.majorBreakable)
                    aaa.majorBreakable.ApplyDamage(DamageToDealOnHit, Vector2.zero, false);
                self.Velocity = Vector2.zero;

                Stop = true;
            };

            self.OnPreRigidbodyCollision += del2;
            while (e < Time)
            {
                if (Stop)
                    yield break;
                if (self == null)
                    yield break;

                e += BraveTime.DeltaTime;
                self.Velocity = Vector2.Lerp(Direction * Power, Vector2.zero, e / Time);

                yield return null;
            }
            self.OnPreTileCollision -= del;
            self.OnPreRigidbodyCollision -= del2;
            yield break;
        }
        public void DoDebrisToss(GameObject objec, Vector2 Dire, float TossPower, int Bounces)
        {
            var user = (gun.CurrentOwner as PlayerController);
            tk2dBaseSprite component4 = objec.GetComponent<tk2dBaseSprite>();
            DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(objec, objec.transform.position, Dire, TossPower, false, false, true, false);
            debrisObject.IsAccurateDebris = true;
            debrisObject.Priority = EphemeralObject.EphemeralPriority.Critical;
            debrisObject.bounceCount = Bounces;
        }







        #region Normal Projectile Spawning
        public Projectile DetermineSlashType(Gun gun, Projectile projectile, ProjectileModule projectileModule)
        {
            switch (_ProjectileType)
            {
                case 1:
                    return Guns.Trash_Cannon.DefaultModule.projectiles[0];
                case 2:
                    return UnityEngine.Random.value < 0.1f ? Guns.Molotov_Launcher__Special_Reserve.DefaultModule.projectiles[0] :  Guns.Molotov_Launcher.DefaultModule.projectiles[0];
                case 3:
                    return Guns.The_Kiln.DefaultModule.projectiles[0];
                case 4:
                    return UnityEngine.Random.value < 0.333f ? Guns.Bee_Hive__Apiary.DefaultModule.projectiles[0] : Guns.Bee_Hive.DefaultModule.projectiles[0];
                case 5:
                    return Items.Bloody_9mm.ReplacementProjectile;
                case 6:
                    return UnityEngine.Random.value < 0.2 ? Items.Heart_Of_Ice.synergyProjectile : Items.Heart_Of_Ice.projectileToSpawn;
                case 7:
                    switch(UnityEngine.Random.Range(0, 7))
                    {
                        case 0:
                            return Guns.RPG.DefaultModule.projectiles[0];
                        case 1:
                            return Guns.Com4nd0.DefaultModule.projectiles[0];
                        case 2:
                            return Guns.Yari_Launcher.DefaultModule.projectiles[0];
                        case 3:
                            return Guns.RC_Rocket.DefaultModule.projectiles[0];
                        case 4:
                            return Guns.Mahoguny.DefaultModule.projectiles[0];
                        case 5:
                            return Guns.The_Exotic.DefaultModule.projectiles[0];
                        case 6:
                            return Guns.Void_Core_Cannon.DefaultModule.projectiles[0];
                        default:
                            return Guns.RPG.DefaultModule.projectiles[0];
                    }
                case 8:
                    return ((PickupObjectDatabase.GetById(SawBladeGun.ID) as Gun).DefaultModule.chargeProjectiles[0].Projectile);
                case 13:
                    return Items.Roll_Bomb.Volley.projectiles[0].projectiles[0];
                case 14:
                    return Actives.Resourceful_Sack.Burst.ProjectileInterface.SpecifiedProjectile;
                default:
                    return projectile;
            }
        }
        #endregion
    
    

    }
}
