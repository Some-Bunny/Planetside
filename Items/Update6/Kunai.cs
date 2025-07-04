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
            projectile.baseData.damage = 22.5f;
            projectile.baseData.speed = 50f;
			projectile.baseData.UsesCustomAccelerationCurve = true;
			projectile.baseData.CustomAccelerationCurveDuration = 0.5f;
			projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0, 1, 1, 0.5f);

            projectile.shouldRotate = true;
            projectile.pierceMinorBreakables = true;
            projectile.baseData.range = 1000f;

            projectile.SetProjectileCollisionRight("kunaiprojectile", StaticSpriteDefinitions.Projectile_Sheet_Data, 16, 5, false, tk2dBaseSprite.Anchor.MiddleCenter);


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
        }
        public static GameObject ThrowEffect;
        public static Projectile KunaiProjectile;
        public static int ItemID;


		public override void Update()
		{
			base.Update();
			if (Owner)
			{
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
			if (!KunaiCooldowns.ContainsKey(newGun.PickupObjectId))
			{
                base.Owner.StartCoroutine(DoYeet());
				KunaiCooldowns.Add(newGun.PickupObjectId, 5);
            }
        }

        public IEnumerator DoYeet()
        {
            var player = base.Owner;
            yield return new WaitForSeconds(0.05f);

            for (int i = 0; i < 3; i++)
            {
                AkSoundEngine.PostEvent("Play_BOSS_lichC_zap_01", player.gameObject);

                float angle = i == 0 ? player.CurrentGun.CurrentAngle : ProjSpawnHelper.GetAccuracyAngled(player.CurrentGun.CurrentAngle, 10, player);
                SpawnManager.SpawnVFX(ThrowEffect, player.gunAttachPoint.position, Quaternion.Euler(0,0, angle));

                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(KunaiProjectile.gameObject, player.gunAttachPoint.position, Quaternion.Euler(0f, 0f, angle), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {
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
	}
}
