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
	public class CelBullet : AIActor
	{

		public static void Init()
		{
			AIActor cel = EnemyToolbox.CreateNewBulletBankerEnemy("unstablestrafe_bullet", "UnstableStrafe", 18, 22, new List<int> { 53, 54, 55, 56 }, new List<int> { 57, 58, 59, 60, 61, 62 }, null, null, 4f, 20, 5, 10, 10);
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
					MagazineCapacity = 3,
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
					Cooldown = 0.2f,
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
			PlayerHandController handObj = SpriteBuilder.SpriteFromResource("Planetside/Resources/Enemies/ModderBullets/cel/handObj", new GameObject("Cel Hand")).AddComponent<PlayerHandController>();
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
			EnemyBuilder.DuplicateAIShooterAndAIBulletBank(cel.gameObject, aIActor.aiShooter, aIActor.GetComponent<AIBulletBank>(), 38, yah.transform, null, handObj);
			EnemyToolbox.DestroyUnnecessaryHandObjects(cel.transform);


		}
		public class SkellScript : Script
		{
			public override IEnumerator Top() 
			{
				AkSoundEngine.PostEvent("Play_WPN_magnum_shot_01", this.BulletBank.aiActor.gameObject);
				if (this.BulletBank && this.BulletBank.aiActor && this.BulletBank.aiActor.TargetRigidbody)
				{
					base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("796a7ed4ad804984859088fc91672c7f").bulletBank.bulletBank.GetBullet("default"));
				}
				for (int i = -1; i <= 1; i++)
				{
					this.Fire(new Direction((float)(i * 3), DirectionType.Aim, -1f), new Speed(4f, SpeedType.Absolute), new SkellBullet());
				}
				yield break;
			}
		}
		

		public class SkellBullet : Bullet
		{
			public SkellBullet() : base("default", false, false, false)
			{

			}
			public override IEnumerator Top()
			{
				base.ChangeSpeed(new Speed(20f, SpeedType.Absolute), 90);
				yield break;
			}
		}
	}
}








