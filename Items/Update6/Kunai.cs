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

namespace Planetside
{
	public class Kunai : PassiveItem
	{
		public static void Init()
		{
			string name = "Kunai";
			GameObject gameObject = new GameObject(name);
            Kunai item = gameObject.AddComponent<Kunai>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("kunai"), data, gameObject); 
			
			string shortDesc = "The Arts";
			string longDesc = "Toss multiple kunai when switching weapons.\n\nA favourite of a Gungeoneer lost to time.";
			
			
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.C;
            Kunai.ItemID = item.PickupObjectId;

            Gun gun4 = PickupObjectDatabase.GetById(43) as Gun;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun4.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 18f;
            projectile.baseData.speed = 50f;
			projectile.baseData.UsesCustomAccelerationCurve = true;
			projectile.baseData.CustomAccelerationCurveDuration = 0.5f;
			projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1, 1, 0.5f);

            projectile.shouldRotate = true;
            projectile.pierceMinorBreakables = true;
            projectile.baseData.range = 1000f;

            Alexandria.Assetbundle.ProjectileBuilders.SetProjectileCollisionRight(projectile, "kunaiprojectile", StaticSpriteDefinitions.Projectile_Sheet_Data, 16, 5, false, tk2dBaseSprite.Anchor.MiddleCenter);


            var tro = projectile.gameObject.AddChild("trail object");
            tro.transform.position = projectile.sprite.WorldCenter;// + new Vector2(.25f, 0.3125f);
            tro.transform.localPosition = projectile.sprite.WorldCenter;

            TrailRenderer tr = tro.AddComponent<TrailRenderer>();
            tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tr.receiveShadows = false;
            var mat = new Material(Shader.Find("Sprites/Default"));
            tr.material = mat;
            tr.minVertexDistance = 0.01f;
            tr.numCapVertices = 640;

            //======
            UnityEngine.Color color = new UnityEngine.Color(1, 0, 0, 1);
            mat.SetColor("_Color", color);
            tr.startColor = color;
            tr.endColor = new Color(0.5f, 0,0, 1);
            //======
            tr.time = 0.333f;
            //======
            tr.startWidth = 0.1875f;
            tr.endWidth = 0f;
            tr.autodestruct = false;

            var rend = projectile.gameObject.AddComponent<ProjectileTrailRendererController>();
            rend.trailRenderer = tr;
            rend.desiredLength = 8;


            var obj_1 = PrefabBuilder.BuildObject("KunaiImpact_HR");
            var spr_1 = obj_1.gameObject.AddComponent<tk2dSprite>();
            spr_1.SetSprite(StaticSpriteDefinitions.ProjectileImpact_Sheet_Data, StaticSpriteDefinitions.ProjectileImpact_Sheet_Data.GetSpriteIdByName("kunaiprojectile_impacthorizontal_right_001"));
            var animator_1 = spr_1.AddComponent<tk2dSpriteAnimator>();
            animator_1.playAutomatically = true;
            animator_1.library = StaticSpriteDefinitions.ProjectileImpact_Animation_Data;
            animator_1._sprite = spr_1;
            animator_1.DefaultClipId = StaticSpriteDefinitions.ProjectileImpact_Animation_Data.GetClipIdByName("kunai_hit");
            var vfx_1 = obj_1.CreateQuickVFXPool(true, true, VFXAlignment.NormalAligned, false);


            var obj_3 = PrefabBuilder.BuildObject("KunaiImpact_VB");
            var spr_3 = obj_3.gameObject.AddComponent<tk2dSprite>();
            spr_3.SetSprite(StaticSpriteDefinitions.ProjectileImpact_Sheet_Data, StaticSpriteDefinitions.ProjectileImpact_Sheet_Data.GetSpriteIdByName("kunaiprojectile_impactverticaldown_001"));
            var animator_3 = spr_3.AddComponent<tk2dSpriteAnimator>();
            animator_3.playAutomatically = true;
            animator_3.library = StaticSpriteDefinitions.ProjectileImpact_Animation_Data;
            animator_3._sprite = spr_3;
            animator_3.DefaultClipId = StaticSpriteDefinitions.ProjectileImpact_Animation_Data.GetClipIdByName("kunai_vb_impact");
            var vfx_3 = obj_3.CreateQuickVFXPool();
            

            projectile.hitEffects.tileMapHorizontal = vfx_1;
            projectile.hitEffects.deathTileMapHorizontal = vfx_1;


            projectile.hitEffects.tileMapVertical = vfx_1;
            projectile.hitEffects.deathTileMapVertical = vfx_1;


            projectile.gameObject.AddComponent<PierceDeadActors>();

            var excaluburProj = (PickupObjectDatabase.GetById(377) as Gun).DefaultModule.projectiles[0];

            projectile.hitEffects.deathEnemy = excaluburProj.hitEffects.deathEnemy;
            projectile.hitEffects.enemy = excaluburProj.hitEffects.enemy;

            projectile.enemyImpactEventName = excaluburProj.enemyImpactEventName;
            projectile.objectImpactEventName = excaluburProj.objectImpactEventName;


            KunaiProjectile = projectile;
            ThrowEffect = (PickupObjectDatabase.GetById(97) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;

            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:kunai",
                "katana_bullets"
            };
            Alexandria.ItemAPI.CustomSynergies.Add("Dragun Punch", mandatoryConsoleIDs, null, true);
            List<string> mandatoryConsoleIDs_2 = new List<string>
            {
                "psog:kunai",
                "kruller_glaive"
            };
            Alexandria.ItemAPI.CustomSynergies.Add("Convenient Option", mandatoryConsoleIDs_2, null, true);
            KatanaSlash = (PickupObjectDatabase.GetById(822) as ComplexProjectileModifier);



            var slash = ScriptableObject.CreateInstance<KunaiSpecialSlash>();
            slash.projInteractMode = CustomSlashDoer.ProjInteractMode.REFLECTANDPOSTPROCESS;
            slash.playerKnockbackForce = 0;
            slash.enemyKnockbackForce = 20;
            slash.doVFX = true;
            slash.doHitVFX = true;
            slash.slashRange = 2.5f;
            slash.slashDegrees = 72;
            slash.soundEvent = "Play_ENM_gunnut_swing_01";
            slash.damage = 25;
            slash.damagesBreakables = true;
            slash.VFX = KatanaSlash.LinearChainExplosionData.effect.CreateQuickVFXPool()
;
            slash.hitVFX = (PickupObjectDatabase.GetById(539) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.enemy;
            slash.HitSecretRoomWalls = true;
            customSlash = slash;
            GlaiveProjectile = (PickupObjectDatabase.GetById(656) as Gun).DefaultModule.chargeProjectiles[0].Projectile;
        }
        public static KunaiSpecialSlash customSlash;
        public static Projectile GlaiveProjectile;

        public static ComplexProjectileModifier KatanaSlash;

        public static GameObject ThrowEffect;
        public static Projectile KunaiProjectile;
        public static int ItemID;

        private float GlobalCooldown = 0;
		public override void Update()
		{
			base.Update();
			if (Owner)
			{
                if (GlobalCooldown > 0) { GlobalCooldown -= BraveTime.DeltaTime; }
				var l = KunaiCooldowns.Keys.ToList();
				for (int i = l.Count - 1; i > -1; i--)
				{
					KunaiCooldowns[l[i]] -= Time.deltaTime;
					if (KunaiCooldowns[l[i]] <= 0) {KunaiCooldowns.Remove(l[i]); }
                }
			}
		}

		private Dictionary<int, float> KunaiCooldowns = new Dictionary<int, float>();

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
            player.GunChanged += Player_GunChanged;
		}

        public void Player_GunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            if (GlobalCooldown > 0) { return; }
			if (!KunaiCooldowns.ContainsKey(newGun.PickupObjectId))
			{
                GlobalCooldown = 0.333f;
                base.Owner.StartCoroutine(DoYeet());
				KunaiCooldowns.Add(newGun.PickupObjectId, 5);
            }
        }

        public IEnumerator DoYeet()
        {
            var player = base.Owner;
            yield return new WaitForSeconds(0.05f);

            if (this.Owner && this.Owner.PlayerHasActiveSynergy("Dragun Punch"))
            {
                CustomSlashDoer.DoSwordSlash(base.Owner.CurrentGun.PrimaryHandAttachPoint.position, base.Owner.CurrentGun.CurrentAngle, base.Owner, customSlash);
            }

            for (int i = 0; i < 3; i++)
            {
                AkSoundEngine.PostEvent("Play_BOSS_lichC_zap_01", player.gameObject);

                float angle = i == 0 ? player.CurrentGun.CurrentAngle : ProjSpawnHelper.GetAccuracyAngled(player.CurrentGun.CurrentAngle, 10, player);
                SpawnManager.SpawnVFX(ThrowEffect, player.gunAttachPoint.position, Quaternion.Euler(0,0, angle));

                bool isGlaive = player.CurrentGun.PickupObjectId == 656;

                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(isGlaive ? GlaiveProjectile.gameObject : KunaiProjectile.gameObject, player.gunAttachPoint.position, Quaternion.Euler(0f, 0f, angle), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {
                    if (isGlaive)
                    {
                        component.baseData.damage *= 0.666f;
                        component.baseData.speed *= 1.5f;
                        component.UpdateSpeed();
                    }
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    player.DoPostProcessProjectile(component);
                }

                yield return new WaitForSeconds(0.0625f);
            }
            yield break;
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


        public class KunaiSpecialSlash : CustomSlashData
        {
            public override CustomSlashData ReturnClone()
            {

                KunaiSpecialSlash newData = ScriptableObject.CreateInstance<KunaiSpecialSlash>();
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
                    if (myAiActor && myAiActor.IsNormalEnemy && myAiActor.healthHaver)
                    {
                        myAiActor.behaviorSpeculator?.Stun(1.5f);
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

                p.baseData.damage = 2f + (p.baseData.speed * 0.1f);
                p.baseData.speed = p.baseData.speed * 1.4f;
                float speedMath = Mathf.Max(0, 60 - (previousSpeed * 3f));

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

              
                p.AdjustPlayerProjectileTint(new Color(1, 0.6f, 0), 10);
                p.UpdateCollisionMask();
                p.Reflected();
                p.SendInDirection(p.Direction, resetDistance: true);
            }
        }

    }
}
