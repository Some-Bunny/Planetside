using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using GungeonAPI;
using System.Reflection;


namespace Planetside
{
	public class QadayBullet : AIActor
	{

		public static void Init()
		{
			AIActor cel = EnemyToolbox.CreateNewBulletBankerEnemy("qaday_bullet", "Qaday", 18, 22, new List<int> { 236, 237, 238, 237 }, new List<int> { 239, 240, 241, 242 }, null, null, 4f, 20, 5, 10, 10);

            var bs = cel.gameObject.GetComponent<BehaviorSpeculator>();

            bs.AttackBehaviors = new List<AttackBehaviorBase>() {
				new ShootGunBehavior() {
					GroupCooldownVariance = 0.2f,
					LineOfSight = false,
					WeaponType = WeaponType.BulletScript,
					OverrideBulletName = null,
					BulletScript = new CustomBulletScriptSelector(typeof(SkellScript)),
					FixTargetDuringAttack = true,
					StopDuringAttack = true,
					LeadAmount = 0,
					LeadChance = 1,
					RespectReload = true,
					MagazineCapacity = 2,
					ReloadSpeed = 5f,
					EmptiesClip = true,
					SuppressReloadAnim = false,
					TimeBetweenShots = -1,
					PreventTargetSwitching = true,
					OverrideAnimation = null,
					OverrideDirectionalAnimation = null,
					HideGun = false,
					UseLaserSight = false,
					UseGreenLaser = false,
					PreFireLaserTime = -1,
					AimAtFacingDirectionWhenSafe = false,
					Cooldown = 0.6f,
					CooldownVariance = 0,
					AttackCooldown = 0,
					GlobalCooldown = 0,
					InitialCooldown = 0,
					InitialCooldownVariance = 0,
					GroupName = null,
					GroupCooldown = 0,
					MinRange = 0,
					Range = 16,
					MinWallDistance = 0,
					MaxEnemiesInRoom = 0,
					MinHealthThreshold = 0,
					MaxHealthThreshold = 1,
					HealthThresholds = new float[0],
					AccumulateHealthThresholds = true,
					targetAreaStyle = null,
					IsBlackPhantom = false,
					resetCooldownOnDamage = null,
					RequiresLineOfSight = true,
					MaxUsages = 0,
					
				}
			};

            AIActor aIActor = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
			PlayerHandController handObj = SpriteBuilder.SpriteFromResource("Planetside/Resources/Enemies/ModderBullets/cel/handObj", new GameObject("Qaday Hand")).AddComponent<PlayerHandController>();
			FakePrefab.MarkAsFakePrefab(handObj.gameObject);
			handObj.ForceRenderersOff = false;
			handObj.sprite.usesOverrideMaterial = true;
			handObj.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
			handObj.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
			handObj.sprite.renderer.material.SetFloat("_EmissivePower", 40);
			handObj.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.2f);


            UnityEngine.Object.DontDestroyOnLoad(handObj.gameObject);
			var yah = cel.transform.Find("GunAttachPoint").gameObject;
			yah.transform.position = cel.aiActor.transform.position;
			yah.transform.localPosition = new Vector2(0f, 0f);
			EnemyBuilder.DuplicateAIShooterAndAIBulletBank(cel.gameObject, aIActor.aiShooter, aIActor.GetComponent<AIBulletBank>(), 93, yah.transform, null, handObj);
			EnemyToolbox.DestroyUnnecessaryHandObjects(cel.transform);
			cel.bulletBank.Bullets = new List<AIBulletBank.Entry>();
            cel.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));


        }
        public class SkellScript : Script
		{
			public override IEnumerator Top() 
			{
				AkSoundEngine.PostEvent("Play_DoubleBarrel", this.BulletBank.aiActor.gameObject);
				for (int i = 0; i < 15; i++)
				{
                    this.Fire(new Direction(UnityEngine.Random.Range(-24 - (1 * i), 24 + (1 * i)), DirectionType.Aim, -1f), new Speed(12f, SpeedType.Absolute), new SkellBullet());
                }
                yield break;
			}
		}
		

		public class SkellBullet : Bullet
		{
			public SkellBullet() : base("poundSmall", false, false, false)
			{

			}
			public override IEnumerator Top()
			{
				base.ChangeSpeed(new Speed(UnityEngine.Random.Range(4, 14), SpeedType.Absolute), 150);
				yield break;
			}
		}
	}
}








