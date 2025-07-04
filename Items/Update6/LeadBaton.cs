using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Collections;
using SaveAPI;
using Brave.BulletScript;
using Alexandria.cAPI;
using Alexandria.PrefabAPI;
using Newtonsoft.Json.Linq;
using Alexandria.ItemAPI;
using static UnityEngine.UI.GridLayoutGroup;
using System.ComponentModel;
using static ETGMod;
using Planetside.DungeonPlaceables;

namespace Planetside
{
	public class LeadBaton : PassiveItem
	{
		public static void Init()
		{
			string name = "Lead Baton";
			GameObject gameObject = new GameObject(name);
            LeadBaton item = gameObject.AddComponent<LeadBaton>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemAPI.ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("leadBaton"), data, gameObject); 
			
			string shortDesc = "The Simplest Solution";
			string longDesc = "Swing an incredibly powerful baton on swapping weapons.\n\nSometimes the simplest, and best way to solve a problem is to hit it really, really hard.";

            ItemAPI.ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.D;
            LeadBaton.ItemID = item.PickupObjectId;
            ItemAPI.ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);


            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemAPI.ItemBuilder.AddSpriteToObjectAssetbundle("LeadBatonSlash", debuffCollection.GetSpriteIdByName("LeadBatonSwing_003"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            ItemAPI.FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.VFX_Animation_Data;
            animator.Library = StaticSpriteDefinitions.VFX_Animation_Data;

            animator.sprite.usesOverrideMaterial = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = animator.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 2f);
            mat.SetFloat("_EmissivePower", 2);
            animator.sprite.renderer.material = mat;

            animator.DefaultClipId = animator.GetClipIdByName("leadBatonSwing");
            animator.playAutomatically = true;
            StaticSpriteDefinitions.VFX_Animation_Data.GetClipByName("leadBatonSwing").ApplyOffsetToAnimation(new Vector2(-1, 0));

            var slash = ScriptableObject.CreateInstance<LeadBatonSlash>();
            slash.projInteractMode = CustomSlashDoer.ProjInteractMode.REFLECTANDPOSTPROCESS;
            slash.playerKnockbackForce = 10;
            slash.enemyKnockbackForce = 400;
            slash.doVFX = true;
            slash.doHitVFX = true;
            slash.slashRange = 4.25f;
            slash.slashDegrees = 90;
            slash.soundEvent = "Play_ENM_gunnut_swing_01";
            slash.damage = 75;
            slash.damagesBreakables = true;
            slash.VFX = BrokenArmorVFXObject.CreateQuickVFXPool();
            slash.hitVFX = (PickupObjectDatabase.GetById(539) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.enemy;//(PickupObjectDatabase.GetById(345) as Gun).DefaultModule.projectiles[0].hitEffects.enemy;
            slash.HitSecretRoomWalls = true;
            slash.OnHitMajorBreakable += (obj1) =>
            {
                var secretRoomDoor = (obj1 as MajorBreakable);
                if (secretRoomDoor != null)
                {
                    var HotPos = secretRoomDoor.sprite != null ? secretRoomDoor.sprite.WorldCenter : secretRoomDoor.transform.position.XY();
                    Impact.SpawnAtPosition(HotPos);
                    if (secretRoomDoor.IsSecretDoor | secretRoomDoor.GetComponent<Idol>())
                    {
                        AkSoundEngine.PostEvent("Play_BatonHit", secretRoomDoor.gameObject);
                        secretRoomDoor.gameObject.DoHitStop(0.5f);
                        secretRoomDoor.ApplyDamage(1E+10f, Vector2.zero, false, true, true);

                        for (int i = 0; i < 100; i++)
                        {

                            GlobalSparksDoer.DoRandomParticleBurst(1, HotPos + new Vector2(-0.5f, 0.5f), HotPos + new Vector2(0.5f, 0.5f),
                            BraveUtility.RandomVector2(new Vector2(-1, -1), new Vector2(1, 1)),
                            0f,
                            5f,
                            0.1f,
                            2.5f,
                            new Color(0.7f, 0.9f, 0.7f, 1),
                            GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                        }

                    }

                }
            };

            customSlash = slash;


        }

        private static Projectile CaseyBaseProjectileLaunch = (PickupObjectDatabase.GetById(541) as Gun).DefaultModule.chargeProjectiles[0].Projectile.GetComponent<KilledEnemiesBecomeProjectileModifier>().BaseProjectile;

        public static VFXPool Impact = (PickupObjectDatabase.GetById(539) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.enemy;
        public static string ImpactSFX = (PickupObjectDatabase.GetById(539) as Gun).DefaultModule.chargeProjectiles[0].Projectile.enemyImpactEventName;

        public static LeadBatonSlash customSlash;
        public static int ItemID;


        public class LeadBatonSlash : CustomSlashData
        {
            public override CustomSlashData ReturnClone()
            {

                LeadBatonSlash newData = ScriptableObject.CreateInstance<LeadBatonSlash>();
                newData.doVFX = this.doVFX;
                newData.VFX = this.VFX;
                newData.doHitVFX = this.doHitVFX;
                newData.hitVFX = this.hitVFX;
                newData.projInteractMode = this.projInteractMode;
                newData.playerKnockbackForce = this.playerKnockbackForce;
                newData.enemyKnockbackForce = this.enemyKnockbackForce;
                newData.statusEffects = this.statusEffects;
                newData.jammedDamageMult = this.jammedDamageMult;
                newData.bossDamageMult = this.bossDamageMult;
                newData.doOnSlash = this.doOnSlash;
                newData.doPostProcessSlash = this.doPostProcessSlash;
                newData.slashRange = this.slashRange;
                newData.slashDegrees = this.slashDegrees;
                newData.damage = this.damage;
                newData.damagesBreakables = this.damagesBreakables;
                newData.soundEvent = this.soundEvent;
                newData.OnHitTarget = this.OnHitTarget;
                newData.OnHitBullet = this.OnHitBullet;
                newData.OnHitMinorBreakable = this.OnHitMinorBreakable;
                newData.OnHitMajorBreakable = this.OnHitMajorBreakable;
                return newData;
            }

            public override void OnHitEnemy(GameActor gameActor, bool b, float Angle, Vector2 arcOrigin, Vector2 contact)
            {
                if (gameActor is AIActor myAiActor && b == true)
                {
                    if (myAiActor && myAiActor.IsNormalEnemy && myAiActor.healthHaver && !myAiActor.healthHaver.IsBoss)
                    {

                        AkSoundEngine.PostEvent("Play_BatonHit", myAiActor.gameObject);
                        myAiActor.gameObject.DoHitStop(0.3f);
                        Impact.SpawnAtPosition(contact);


                        myAiActor.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox));

                        SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate @delegate = null;
                        @delegate = (myRigidbody, myPixelCollider, otherRigidbody, otherPixelCollider) =>
                        {
                            if (otherRigidbody && otherRigidbody.aiActor && myRigidbody && myRigidbody.healthHaver)
                            {
                                AIActor aiActor = otherRigidbody.aiActor;
                                myRigidbody.OnPreRigidbodyCollision -= @delegate;
                                if (aiActor.IsNormalEnemy && aiActor.healthHaver)
                                {
                                    aiActor.healthHaver.ApplyDamage(myRigidbody.healthHaver.GetMaxHealth() * 2f, myRigidbody.Velocity, "Pinball", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                                }
                            }
                        };

                        for (int i = 0; i < 32; i++)
                        {

                           GlobalSparksDoer.DoRandomParticleBurst(1, myAiActor.sprite.WorldCenter + new Vector2(-0.5f, 0.5f), myAiActor.sprite.WorldCenter + new Vector2(0.5f, 0.5f),
                           BraveUtility.RandomVector2(new Vector2(-1, -1), new Vector2(1, 1)),
                           0f,
                           5f,
                           null,
                           1.25f,
                           new Color(0.7f, 0.9f, 0.7f, 1),
                           GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                        }
                    }
                }
            }



            public override void OnProjectileReflect(Projectile p, bool retargetReflectedBullet, GameActor newOwner, float minReflectedBulletSpeed, bool doPostProcessing = false, float scaleModifier = 1, float baseDamage = 10, float spread = 0, string sfx = null)
            {
                AkSoundEngine.PostEvent("Play_OBJ_metalskin_deflect_01", p.gameObject);

                p.RemoveBulletScriptControl();

                if ((bool)p.Owner && (bool)p.Owner.specRigidbody)
                {
                    p.specRigidbody.DeregisterSpecificCollisionException(p.Owner.specRigidbody);
                }

                p.Owner = newOwner;
                p.SetNewShooter(newOwner.specRigidbody);
                p.allowSelfShooting = false;
                if (newOwner is AIActor)
                {
                    p.collidesWithPlayer = true;
                    p.collidesWithEnemies = false;
                }
                else if (newOwner is PlayerController)
                {
                    p.collidesWithPlayer = false;
                    p.collidesWithEnemies = true;
                }
                SpawnManager.PoolManager.Remove(p.transform);
                float previousSpeed = p.baseData.speed;

                p.baseData.damage = 5f + (p.baseData.speed * 0.1f);
 
                p.baseData.speed = Mathf.Max(60, p.baseData.speed * 2);

                float speedMath = Mathf.Max(0, 90 - (previousSpeed * 3f));

                p.UpdateSpeed();
                if (newOwner is PlayerController)
                {
                    PlayerController playerController = newOwner as PlayerController;
                    if (playerController != null)
                    {
                        p.Direction = MathToolbox.GetUnitOnCircle(playerController.CurrentGun.CurrentAngle + UnityEngine.Random.Range(-speedMath, speedMath), 1);

                        p.baseData.damage *= playerController.stats.GetStatValue(PlayerStats.StatType.Damage);
                        p.baseData.speed *= playerController.stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                        p.UpdateSpeed();
                        p.baseData.force *= playerController.stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
                        p.baseData.range *= playerController.stats.GetStatValue(PlayerStats.StatType.RangeMultiplier);
                        p.BossDamageMultiplier *= playerController.stats.GetStatValue(PlayerStats.StatType.DamageToBosses);
                        p.RuntimeUpdateScale(playerController.stats.GetStatValue(PlayerStats.StatType.PlayerBulletScale));
                        playerController.DoPostProcessProjectile(p);
                    }
                }

                p.UpdateCollisionMask();
                p.Reflected();
                p.SendInDirection(p.Direction, resetDistance: true);
            }
        }
        private bool Active;
        private float Cooldown = 0;

        public override void Update()
		{
			base.Update();
			if (Owner)
			{
				if (Cooldown > 0)
				{
					Cooldown -= Time.deltaTime;
				}
                else if (Active == false)
                {
                    AkSoundEngine.PostEvent("Play_ITM_Folding_Table_Use_01", base.Owner.gameObject);
                    AkSoundEngine.PostEvent("Play_ITM_Folding_Table_Use_01", base.Owner.gameObject);

                    //m_WPN_baseball_charge_01
                    for (int i = 0; i < 16; i++)
                    {
                        GlobalSparksDoer.DoRandomParticleBurst(1, base.Owner.sprite.WorldCenter + new Vector2(-0.5f, 0.5f), base.Owner.sprite.WorldCenter + new Vector2(0.5f, 0.5f),
                            BraveUtility.RandomVector2(new Vector2(-1, -1), new Vector2(1, 1)),
                            0f,
                            0.5f,
                            null,
                            UnityEngine.Random.Range(0.8f, 1.4f),
                            new Color(0.7f, 0.9f, 0.7f, 1),
                            GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    }
                    Active = true;
                }
			}
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
            player.GunChanged += Player_GunChanged;
		}

        public void Player_GunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            if (Active == true)
            {
                Active = false;
                Cooldown = 10;
                this.Invoke("DoSwing", 0.05f);
            }
        }

        private void DoSwing()
        {
            CustomSlashDoer.DoSwordSlash(base.Owner.CurrentGun.PrimaryHandAttachPoint.position, base.Owner.CurrentGun.CurrentAngle, base.Owner, customSlash);

        }

       

        public override void OnDestroy()
		{
			if (Owner) Owner.GunChanged -= Player_GunChanged;

            base.OnDestroy();
		}
		public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
            player.GunChanged -= Player_GunChanged;

            return result;
		}
	}
}
